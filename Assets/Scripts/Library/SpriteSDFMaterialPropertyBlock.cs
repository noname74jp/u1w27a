using UnityEngine;

namespace Library
{
    /// <summary>
    /// <see cref="Sprite" />で使用するSigned Distance Fieldのマテリアルのプロパティを制御するコンポーネント。
    /// </summary>
    [ExecuteInEditMode]
    public class SpriteSDFMaterialPropertyBlock : MonoBehaviour
    {
        #region constants

        /// <summary>
        /// アウトラインの色。
        /// </summary>
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

        /// <summary>
        /// アウトラインの幅。
        /// </summary>
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");

        /// <summary>
        /// アウトラインのソフトネス。
        /// </summary>
        private static readonly int OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");

        #endregion

        #region variables

        /// <summary>
        /// アウトラインの色。
        /// </summary>
        [SerializeField] private Color outlineColor;

        /// <summary>
        /// アウトラインの幅。
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float outlineWidth;

        /// <summary>
        /// アウトラインのソフトネス。
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float outlineSoftness;

        /// <summary>
        /// 対象の<see cref="MaterialPropertyBlock" />
        /// </summary>
        private MaterialPropertyBlock _materialPropertyBlock;

        /// <summary>
        /// 対象の<see cref="SpriteRenderer" />
        /// </summary>
        private SpriteRenderer _renderer;

        /// <summary>
        /// 更新が必要か
        /// </summary>
        private bool _needUpdate;

        #endregion

        #region methods

        /// <summary>
        /// Unityイベント関数Awake。
        /// </summary>
        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _materialPropertyBlock = new MaterialPropertyBlock();
            _needUpdate = true;
        }

        /// <summary>
        /// Unityイベント関数Update。
        /// </summary>
        private void Update()
        {
            // 更新が不要なら抜ける
            if (!_needUpdate || _materialPropertyBlock == null)
            {
                return;
            }

            // 更新フラグをおろす
            _needUpdate = false;

            // パラメーター設定
            _renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor(OutlineColor, outlineColor);
            _materialPropertyBlock.SetFloat(OutlineWidth, outlineWidth);
            _materialPropertyBlock.SetFloat(OutlineSoftness, outlineSoftness);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Unityイベント関数OnValidate。
        /// </summary>
        private void OnValidate()
        {
            _needUpdate = true;
        }
#endif

        #endregion
    }
}
