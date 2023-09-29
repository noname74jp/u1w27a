using Game.Logic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    /// <summary>
    /// 弾のオブジェクト。
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        #region variables

        /// <summary>
        /// 対象の<see cref="SpriteRenderer" />
        /// </summary>
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>
        /// ロジック。
        /// </summary>
        private BulletLogic _logic;

        #endregion

        #region methods

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="logic">設定するロジック。</param>
        public void Initialize(BulletLogic logic)
        {
            _logic = logic;
            spriteRenderer.enabled = false;
        }

        /// <summary>
        /// 状態を更新する。
        /// </summary>
        public void UpdateStatus()
        {
            spriteRenderer.enabled = _logic.Alive;
            if (!_logic.Alive)
            {
                return;
            }

            var transformCache = transform;
            transformCache.localPosition = _logic.Location;
            transformCache.localScale = Vector3.one * (_logic.Size * 60.0f / 52.0f);
        }

        #endregion
    }
}
