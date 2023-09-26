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

        /// <summary>
        /// スコア。
        /// </summary>
        private int _score;

        #endregion

        #region methods

        /// <summary>
        /// Unityイベント関数Awake。
        /// </summary>
        private void Awake()
        {
            _score = -1;
            SetScore(0);
        }

        /// <summary>
        /// スコアを設定する。
        /// </summary>
        /// <param name="score">スコア。</param>
        public void SetScore(int score)
        {
            // スコアが一致していたら何もしない
            if (_score == score)
            {
                return;
            }

            // スコア更新
            _score = score;
            var value = score;
            var rendererEnabled = true;
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = rendererEnabled;
                spriteRenderer.sprite = sprites[value % 10];
                value /= 10;
                rendererEnabled = value != 0;
            }

            spriteRenderers[0].enabled = true;
        }

        #endregion
    }
}
