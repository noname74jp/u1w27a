using Game.Logic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    /// <summary>
    /// プレイヤーのオブジェクト。
    /// </summary>
    public class Player : MonoBehaviour
    {
        #region variables

        /// <summary>
        /// 対象の<see cref="SpriteRenderer" />
        /// </summary>
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>
        /// ロジック。
        /// </summary>
        private PlayerLogic _logic;

        #endregion

        #region methods

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="logic">ロジック。</param>
        public void Initialize(PlayerLogic logic)
        {
            _logic = logic;
            UpdateStatus();
        }

        /// <summary>
        /// 状態を更新する。
        /// </summary>
        public void UpdateStatus()
        {
            var transformCache = transform;
            transformCache.localPosition = _logic.Location;
            transformCache.localScale = Vector3.one * (_logic.Size * 60.0f / 40.0f);
            spriteRenderer.flipX = _logic.Velocity.x < 0.0f;
        }

        #endregion
    }
}
