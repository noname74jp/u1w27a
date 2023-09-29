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

        #endregion

        #region methods

        /// <summary>
        /// 状態を更新する。
        /// </summary>
        /// <param name="logic">更新に使用するロジック。</param>
        public void UpdateStatus(CharLogicBase logic)
        {
            spriteRenderer.enabled = logic.Alive;
            transform.localPosition = logic.Location;
        }

        #endregion
    }
}
