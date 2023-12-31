using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// プレイヤーのロジック。
    /// </summary>
    public class PlayerLogic : CharLogicBase
    {
        #region enums

        /// <summary>
        /// 攻撃タイプ。
        /// </summary>
        private enum ShootType
        {
            /// <summary>
            /// 前方に速射。
            /// </summary>
            RapidShot,

            /// <summary>
            /// 斜め前方に3way。
            /// </summary>
            ThreeWayA,

            /// <summary>
            /// 前上下に3way。
            /// </summary>
            ThreeWayB
        }

        #endregion

        #region constants

        /// <summary>
        /// プレイヤーのサイズ[pixel]。
        /// </summary>
        private const float PlayerSize = 20.0f;

        /// <summary>
        /// グライド時の垂直方向の加速度[pixel/frame]。
        /// </summary>
        private const float VerticalAccelerationInGlide = -600.0f * (float)Defines.SecondsPerFrame * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 落下時の垂直方向の加速度[pixel/frame]。
        /// </summary>
        private const float VerticalAccelerationAtFall = -1800.0f * (float)Defines.SecondsPerFrame * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// グライド時の水平方向の速度[pixel/frame]。
        /// </summary>
        private const float HorizontalVelocityInGlide = 320.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 落下時の水平方向の速度[pixel/frame]。
        /// </summary>
        private const float HorizontalVelocityAtFall = 160.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// ジャンプ開始時の垂直方向の初速度[pixel/frame]。
        /// </summary>
        private const float VerticalInitialVelocity = 480.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 垂直方向の最大速度[pixel/frame]。
        /// </summary>
        private const float VerticalMaxVelocity = 1440.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 垂直方向の最小速度[pixel/frame]。
        /// </summary>
        private const float VerticalMinVelocity = -1440.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 垂直方向の0に丸める速度[pixel/frame]。
        /// </summary>
        private const float VerticalRoundToZeroVelocity = 30.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 移動可能範囲。
        /// </summary>
        private static readonly Rect LocationRect = new(-432.0f + PlayerSize * 0.5f, -222.0f + PlayerSize * 0.5f, 864.0f - PlayerSize, 444.0f - PlayerSize);

        /// <summary>
        /// 反発係数。
        /// </summary>
        private const float CoefficientOfRestitution = 0.5f;

        /// <summary>
        /// 攻撃のインターバル[frame]。
        /// </summary>
        private const int AttackInterval = (int)(Defines.FramePerSec * 3.0 / 60.0);

        /// <summary>
        /// 弾の速度[pixel/frame]
        /// </summary>
        private const float BulletVelocityX = 480.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 左の折り返し位置。
        /// </summary>
        private const float LeftTurnAroundPoint = Defines.GridWidth * -4.0f;

        /// <summary>
        /// 右の折り返し位置。
        /// </summary>
        private const float RightTurnAroundPoint = Defines.GridWidth * 4.0f;

        #endregion

        #region variables

        /// <summary>
        /// 垂直方向の加速度。
        /// </summary>
        private float _verticalAcceleration;

        /// <summary>
        /// 反転しているか。
        /// </summary>
        private bool _flipped;

        /// <summary>
        /// 1フレーム前にキーが押されていたか。
        /// </summary>
        private bool _wasKeyPressed;

        /// <summary>
        /// 残り攻撃インターバル。
        /// </summary>
        private int _remainAttackInterval;

        /// <summary>
        /// 攻撃種類。
        /// </summary>
        private ShootType _shootType;

        #endregion

        #region methods

        /// <summary>
        /// 生成する。
        /// </summary>
        public void Create()
        {
            base.Create(new Vector2(LocationRect.center.x, LocationRect.yMin), Vector2.zero, PlayerSize);
            _verticalAcceleration = VerticalAccelerationAtFall;
            _flipped = false;
            _wasKeyPressed = false;
            _shootType = ShootType.RapidShot;
            _remainAttackInterval = AttackInterval;
        }

        /// <summary>
        /// 更新する。
        /// </summary>
        /// <param name="isKeyPressed">キーが押されているか。</param>
        /// <param name="playerBulletLogics">弾配列。</param>
        /// <param name="jumped">ジャンプしたか。</param>
        /// <param name="shooted">弾を撃ったか</param>
        public void UpdateStatus(bool isKeyPressed, List<BulletLogic> playerBulletLogics, out bool jumped, out bool shooted)
        {
            // ジャンプ
            jumped = !isKeyPressed && _wasKeyPressed;
            if (jumped)
            {
                var previousFlipped = _flipped;
                _flipped = _flipped switch
                {
                    // 折り返し点を越えていたら反転
                    true when _location.x <= LeftTurnAroundPoint => false,
                    false when _location.x >= RightTurnAroundPoint => true,
                    _ => _flipped
                };

                // 速度を設定
                _velocity = new Vector2(_flipped ? -HorizontalVelocityAtFall : HorizontalVelocityAtFall, VerticalInitialVelocity);

                // 攻撃切り替え
                if (_flipped != previousFlipped)
                {
                    ChangeShootType();
                }
            }

            // キーの押下状態を更新
            _wasKeyPressed = isKeyPressed;

            // 加速度を更新(ボタンを離しているときは加速度が大きくなる)
            _verticalAcceleration = isKeyPressed ? VerticalAccelerationInGlide : VerticalAccelerationAtFall;

            // 速度を更新
            var previousVelocity = _velocity;
            _velocity.x = (_flipped ? -1 : 1) * (isKeyPressed ? HorizontalVelocityInGlide : HorizontalVelocityAtFall);
            _velocity.y = isKeyPressed ? 0.0f : Mathf.Clamp(_velocity.y + _verticalAcceleration, VerticalMinVelocity, VerticalMaxVelocity);

            // 位置を更新
            _location += (_velocity + previousVelocity) * 0.5f;

            // x方向の領域を越えた場合
            if (_location.x < LocationRect.xMin)
            {
                _location.x = LocationRect.xMin;
            }
            else if (_location.x > LocationRect.xMax)
            {
                _location.x = LocationRect.xMax;
            }

            // y方向の領域を越えた場合
            if (_location.y < LocationRect.yMin)
            {
                _location.y = LocationRect.yMin;
                _velocity.y *= -CoefficientOfRestitution;
                if (Mathf.Abs(_velocity.y) < VerticalRoundToZeroVelocity)
                {
                    _velocity.y = 0.0f;
                }
            }
            else if (_location.y > LocationRect.yMax)
            {
                _location.y = LocationRect.yMax;
            }

            // 弾を撃つ
            shooted = Shoot(playerBulletLogics);
        }

        /// <summary>
        /// 攻撃を切り替える。
        /// </summary>
        private void ChangeShootType()
        {
            _shootType = _shootType switch
            {
                ShootType.RapidShot => ShootType.ThreeWayA,
                ShootType.ThreeWayA => ShootType.ThreeWayB,
                ShootType.ThreeWayB => ShootType.RapidShot,
                _ => _shootType
            };
        }

        /// <summary>
        /// 弾を撃つ
        /// </summary>
        /// <param name="playerBulletLogics">弾配列。</param>
        /// <returns>弾を撃ったらtrue、打たなかったらfalse。</returns>
        private bool Shoot(List<BulletLogic> playerBulletLogics)
        {
            _remainAttackInterval--;
            if (_remainAttackInterval > 0)
            {
                return false;
            }

            switch (_shootType)
            {
                case ShootType.RapidShot:
                {
                    _remainAttackInterval = AttackInterval;
                    Vector2 bulletVelocity = new(_flipped ? -BulletVelocityX : BulletVelocityX, 0.0f);
                    return Shoot(playerBulletLogics, bulletVelocity);
                }
                case ShootType.ThreeWayA:
                {
                    _remainAttackInterval = AttackInterval * 3;

                    Vector2 bulletVelocity = new(_flipped ? -BulletVelocityX : BulletVelocityX, 0.0f);
                    if (!Shoot(playerBulletLogics, bulletVelocity))
                    {
                        return false;
                    }

                    bulletVelocity.y = BulletVelocityX;
                    if (!Shoot(playerBulletLogics, bulletVelocity))
                    {
                        return true;
                    }

                    bulletVelocity.y = -BulletVelocityX;
                    Shoot(playerBulletLogics, bulletVelocity);
                    return true;
                }
                case ShootType.ThreeWayB:
                {
                    _remainAttackInterval = AttackInterval * 3;

                    Vector2 bulletVelocity = new(_flipped ? -BulletVelocityX : BulletVelocityX, 0.0f);
                    if (!Shoot(playerBulletLogics, bulletVelocity))
                    {
                        return false;
                    }

                    bulletVelocity.x = 0.0f;
                    bulletVelocity.y = BulletVelocityX;
                    if (!Shoot(playerBulletLogics, bulletVelocity))
                    {
                        return true;
                    }

                    bulletVelocity.y = -BulletVelocityX;
                    Shoot(playerBulletLogics, bulletVelocity);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 弾を撃つ
        /// </summary>
        /// <param name="playerBulletLogics">弾配列。</param>
        /// <param name="bulletVelocity">弾速。</param>
        /// <returns>弾を撃ったらtrue、打たなかったらfalse。</returns>
        private bool Shoot(IEnumerable<BulletLogic> playerBulletLogics, Vector2 bulletVelocity)
        {
            var bulletLogic = playerBulletLogics.FirstOrDefault(logic => !logic.Alive);
            if (bulletLogic == null)
            {
                return false;
            }

            bulletLogic.Create(_location, bulletVelocity);
            return true;
        }

        #endregion
    }
}
