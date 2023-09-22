using Game.Logic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    /// <summary>
    /// プレイヤーのオブジェクト。
    /// </summary>
    public class Player : MonoBehaviour
    {
        #region methods

        /// <summary>
        /// 初期化する。
        /// </summary>
        public void Initialize()
        {
            transform.position = Vector3.zero;
        }

        /// <summary>
        /// 状態を更新する。
        /// </summary>
        /// <param name="logic">更新に使用するロジック。</param>
        public void UpdateStatus(PlayerLogic logic)
        {
            transform.position = logic.Location;
        }

        #endregion
    }
}