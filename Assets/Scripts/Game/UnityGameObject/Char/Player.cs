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
            transform.localPosition = logic.Location;
        }

        /// <summary>
        /// 状態を更新する。
        /// </summary>
        public void UpdateStatus()
        {
            var transformCache = transform;
            transformCache.localPosition = _logic.Location;
            transformCache.localScale = Vector3.one * (_logic.Size * 60.0f / 52.0f);
        }

        #endregion
    }
}
