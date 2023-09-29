using UnityEngine;

namespace Game.UnityGameObject.UI
{
    /// <summary>
    /// スコアボード。
    /// </summary>
    public class ScoreBoard : MonoBehaviour
    {
        #region variables

        /// <summary>
        /// <see cref="Sprite" />
        /// </summary>
        [SerializeField] private Sprite[] sprites;

        /// <summary>
        /// <see cref="SpriteRenderer" />
        /// </summary>
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        #endregion

        #region methods

        /// <summary>
        /// スコアを設定する。
        /// </summary>
        /// <param name="score">スコア。</param>
        public void SetScore(int score)
        {
            var value = score;
            var rendererEnabled = true;
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = rendererEnabled;
                spriteRenderer.sprite = sprites[value % 10];
                value /= 10;
                rendererEnabled = value != 0;
            }
        }

        #endregion
    }
}
