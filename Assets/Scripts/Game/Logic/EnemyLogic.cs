using System;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// 敵のロジック。
    /// </summary>
    public class EnemyLogic : CharLogicBase
    {
        #region enums

        /// <summary>
        /// 敵の種類。
        /// </summary>
        public enum EnemyType
        {
            /// <summary>
            /// 横方向に直線運動。
            /// </summary>
            Enemy00
        }

        #endregion

        #region methods

        /// <summary>
        /// 生成する。
        /// </summary>
        /// <param name="enemyType">敵の種類。</param>
        /// <param name="location">初期座標。</param>
        public void Create(EnemyType enemyType, Vector2 location)
        {
            switch (enemyType)
            {
                case EnemyType.Enemy00:
                    const float velocityX = 100.0f;
                    base.Create(location, new Vector2(location.x > 0.0f ? -velocityX : velocityX, 0.0f), 20.0f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
            }
        }

        /// <summary>
        /// 更新する。
        /// </summary>
        public void UpdateStatus()
        {
            // 生存していなければ何もしない
            if (!Alive)
            {
                return;
            }

            // 移動
            _location += _velocity * (float)Defines.SecondsPerFrame;

            // 領域外に出たら破棄
            if (!Rect.Overlaps(Defines.EnemyValidArea))
            {
                Destroy();
            }
        }

        #endregion
    }
}
