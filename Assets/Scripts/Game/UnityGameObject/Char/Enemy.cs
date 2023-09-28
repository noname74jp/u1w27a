using Game.Logic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    public class Enemy : MonoBehaviour
    {
        #region variables

        /// <summary>
        /// 対象の<see cref="SpriteRenderer" />
        /// </summary>
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>
        /// ロジック。
        /// </summary>
        private EnemyLogic _logic;

        #endregion

        #region methods

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="logic">設定するロジック。</param>
        public void Initialize(EnemyLogic logic)
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
            if (_logic.Alive)
            {
                transform.localPosition = _logic.Location;
            }
        }

        #endregion
    }
}
