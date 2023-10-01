using Game.Logic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    public class Enemy : MonoBehaviour
    {
        #region variables

        /// <summary>
        /// 対象の<see cref="SpriteRenderer" />。
        /// </summary>
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>
        /// <see cref="Sprite" />配列。
        /// </summary>
        [SerializeField] private Sprite[] sprites;

        /// <summary>
        /// <see cref="Color" />配列。
        /// </summary>
        [SerializeField] private Color[] colors;

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
            // 生成時の処理
            var transformCache = transform;
            if (_logic.Alive && !spriteRenderer.enabled)
            {
                transformCache.localScale = Vector3.one * (_logic.Size * 60.0f / 40.0f);
                var index = (int)_logic.Category;
                spriteRenderer.sprite = sprites[index];
                spriteRenderer.color = colors[index];
            }

            spriteRenderer.enabled = _logic.Alive;
            if (!_logic.Alive)
            {
                return;
            }

            transformCache.localPosition = _logic.Location;
            transformCache.localRotation = Quaternion.AngleAxis(_logic.Angle, Vector3.forward);
        }

        #endregion
    }
}
