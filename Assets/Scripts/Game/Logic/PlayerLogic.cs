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
        private const float VerticalAccelerationOnAscent = -15.0f * (float)Defines.SecondsPerFrame;

        /// <summary>
        /// 下降時の垂直方向の加速度[pixel/frame]。
        /// </summary>
        private const float VerticalAccelerationOnDescent = -30.0f * (float)Defines.SecondsPerFrame;

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
        private const float VerticalMaxVelocity = VerticalInitialVelocity;

        /// <summary>
        /// 垂直方向の最小速度[pixel/frame]。
        /// </summary>
        private const float VerticalMinVelocity = -VerticalInitialVelocity;

        /// <summary>
        /// 移動可能範囲。
        /// </summary>
        private static readonly Rect LocationRect = new(-432.0f + PlayerSize * 0.5f, -222.0f + PlayerSize * 0.5f, 864.0f - PlayerSize, 444.0f - PlayerSize);

        /// <summary>
        /// 攻撃のインターバル。
        /// </summary>
        private const int AttackInterval = 4;

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
        /// <param name="isKeyPressed">キーが押されていればtrue。</param>
        /// <param name="playerBulletLogics">弾配列。</param>
        public void UpdateStatus(bool isKeyPressed, List<BulletLogic> playerBulletLogics)
        {
            // ジャンプ
            if (isKeyPressed && !_wasKeyPressed)
            {
                _flipped = !_flipped;
                _velocity = new Vector2(_flipped ? -HorizontalInitialVelocity : HorizontalInitialVelocity, VerticalInitialVelocity);
                _verticalAcceleration = VerticalAccelerationOnAscent;
            }

            // キーの押下状態を更新
            _wasKeyPressed = isKeyPressed;

            // 位置を更新
            _location += _velocity;

            // x方向の領域を越えた場合はx座標を制限してx方向の速度を0にする
            if (_location.x < LocationRect.xMin)
            {
                _location.x = LocationRect.xMin;
                _velocity.x = 0.0f;
            }
            else if (_location.x > LocationRect.xMax)
            {
                _location.x = LocationRect.xMax;
                _velocity.x = 0.0f;
            }

            // y方向の領域を越えた場合はy座標を制限して速度を0にする
            if (_location.y < LocationRect.yMin)
            {
                _location.y = LocationRect.yMin;
                _velocity = Vector2.zero;
            }
            else if (_location.y > LocationRect.yMax)
            {
                _location.y = LocationRect.yMax;
                _velocity = Vector2.zero;
            }

            // 速度を更新
            _velocity.y = Mathf.Clamp(_velocity.y + _verticalAcceleration, VerticalMinVelocity, VerticalMaxVelocity);

            // 加速度を更新
            // ボタンを離している、もしくは下降しているときは加速度が大きくなる
            _verticalAcceleration = isKeyPressed && _velocity.y >= 0.0f ? VerticalAccelerationOnAscent : VerticalAccelerationOnDescent;

            // 弾を撃つ
            Shoot(playerBulletLogics);
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

            Vector2 bulletVelocity = new((_flipped ? -480.0f : 480.0f) * (float)Defines.SecondsPerFrame, 0.0f);
            var bulletSize = 8.0f;
            bulletLogic.Create(_location, bulletVelocity, bulletSize);
        }

        #endregion
    }
}
