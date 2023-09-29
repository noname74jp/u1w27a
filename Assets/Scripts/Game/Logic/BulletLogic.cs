using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// 弾のロジック。
    /// </summary>
    public class BulletLogic : CharLogicBase
    {
        #region methods

        /// <summary>
        /// 生成する。
        /// </summary>
        /// <param name="location">初期座標。</param>
        /// <param name="velocity">初期速度。</param>
        public void Create(Vector2 location, Vector2 velocity)
        {
            const float size = 10.5f;
            base.Create(location, velocity, size);
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
            _location += _velocity;

            // 領域外に出たら破棄
            if (!Rect.Overlaps(Defines.BulletValidArea))
            {
                Destroy();
            }
        }

        #endregion
    }
}
