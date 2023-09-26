using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// プレイヤーのロジック。
    /// </summary>
    public class PlayerLogic : LogicBase
    {
        #region constants

        /// <summary>
        /// プレイヤーのサイズ[pixel]。
        /// </summary>
        private const float PlayerSize = 32.0f;

        /// <summary>
        /// 上昇時の垂直方向の加速度[pixel/frame]。
        /// </summary>
        private const float VerticalAccelerationOnAscent = -600.0f * (float)Defines.SecondsPerFrame * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 下降時の垂直方向の加速度[pixel/frame]。
        /// </summary>
        private const float VerticalAccelerationOnDescent = -1800.0f * (float)Defines.SecondsPerFrame * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// ジャンプ開始時の水平方向の初速度[pixel/frame]。
        /// </summary>
        private const float HorizontalInitialVelocity = 240.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// ジャンプ開始時の垂直方向の初速度[pixel/frame]。
        /// </summary>
        private const float VerticalInitialVelocity = 480.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 垂直方向の最大速度[pixel/frame]。
        /// </summary>
        private const float VerticalMaxVelocity = 960.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 垂直方向の最小速度[pixel/frame]。
        /// </summary>
        private const float VerticalMinVelocity = -960.0f * (float)Defines.SecondsPerFrame;

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
        private const int AttackInterval = (int)(Defines.FramePerSec * 5.0 / 60.0);

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

        #endregion

        #region methods

        /// <summary>
        /// 生成する。
        /// </summary>
        public void Create()
        {
            base.Create(Vector2.zero, Vector2.zero, PlayerSize);
            _verticalAcceleration = VerticalAccelerationOnDescent;
            _flipped = false;
            _wasKeyPressed = false;
            _remainAttackInterval = AttackInterval;
        }

        /// <summary>
        /// 更新する。
        /// </summary>
        /// <param name="isKeyPressed">キーが押されているか。</param>
        /// <param name="playerBulletLogics">弾配列。</param>
        public void UpdateStatus(bool isKeyPressed, List<BulletLogic> playerBulletLogics)
        {
            // ジャンプ
            if (isKeyPressed && !_wasKeyPressed)
            {
                _flipped = _flipped switch
                {
                    // 折り返し点を越えていたら反転
                    true when _location.x <= LeftTurnAroundPoint => false,
                    false when _location.x >= RightTurnAroundPoint => true,
                    _ => _flipped
                };

                // 速度を設定
                _velocity = new Vector2(_flipped ? -HorizontalInitialVelocity : HorizontalInitialVelocity, VerticalInitialVelocity);
            }

            // キーの押下状態を更新
            _wasKeyPressed = isKeyPressed;

            // 加速度を更新(ボタンを離しているときは加速度が大きくなる)
            _verticalAcceleration = isKeyPressed ? VerticalAccelerationOnAscent : VerticalAccelerationOnDescent;

            // 速度を更新
            var previousVelocity = _velocity;
            _velocity.y = Mathf.Clamp(_velocity.y + _verticalAcceleration, VerticalMinVelocity, VerticalMaxVelocity);

            // 位置を更新
            _location += (_velocity + previousVelocity) * 0.5f;

            // x方向の領域を越えた場合
            if (_location.x < LocationRect.xMin)
            {
                FlipX(LocationRect.xMin);
            }
            else if (_location.x > LocationRect.xMax)
            {
                FlipX(LocationRect.xMax);
            }

            // y方向の領域を越えた場合
            if (_location.y < LocationRect.yMin)
            {
                FlipY(LocationRect.yMin, isKeyPressed);
            }
            else if (_location.y > LocationRect.yMax)
            {
                FlipY(LocationRect.yMax, isKeyPressed);
            }

            // 弾を撃つ
            Shoot(playerBulletLogics);
        }

        /// <summary>
        /// x方向の向きを反転する。
        /// </summary>
        /// <param name="locationX">反転時のx座標。</param>
        private void FlipX(float locationX)
        {
            _location.x = locationX;
            _velocity.x *= -1.0f;
            _flipped = !_flipped;
        }

        /// <summary>
        /// y方向の向きを反転する。
        /// </summary>
        /// <param name="locationY">反転時のy座標。</param>
        /// <param name="isKeyPressed">キーが押されているか。</param>
        private void FlipY(float locationY, bool isKeyPressed)
        {
            _location.y = locationY;
            _velocity.y *= isKeyPressed ? 0.0f : -CoefficientOfRestitution;
            if (Mathf.Abs(_velocity.y) < VerticalRoundToZeroVelocity)
            {
                _velocity.y = 0.0f;
            }
        }

        /// <summary>
        /// 弾を撃つ
        /// </summary>
        /// <param name="playerBulletLogics">弾配列。</param>
        private void Shoot(List<BulletLogic> playerBulletLogics)
        {
            _remainAttackInterval--;
            if (_remainAttackInterval > 0)
            {
                return;
            }

            _remainAttackInterval = AttackInterval;
            var bulletLogic = playerBulletLogics.FirstOrDefault(logic => !logic.Alive);
            if (bulletLogic == null)
            {
                return;
            }

            Vector2 bulletVelocity = new(_flipped ? -BulletVelocityX : BulletVelocityX, 0.0f);
            var bulletSize = 8.0f;
            bulletLogic.Create(_location, bulletVelocity, bulletSize);
        }

        #endregion
    }
}
