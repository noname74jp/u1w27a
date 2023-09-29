using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// ワールドのルートのロジック。
    /// </summary>
    public class WorldRootLogic
    {
        #region constants

        /// <summary>
        /// スケール速度[/frame]。
        /// </summary>
        private const float ScaleSpeed = 16.0f / 1024.0f;

        /// <summary>
        /// スケールの最大値。
        /// </summary>
        public const float MaxScale = 4.0f;

        /// <summary>
        /// スケールの最小値。
        /// </summary>
        public const float MinScale = 1.0f;

        #endregion

        #region properties

        /// <summary>
        /// 位置。
        /// </summary>
        public Vector2 Location => _playerLogic?.Location ?? Vector2.zero;

        /// <summary>
        /// スケール。
        /// </summary>
        public float Scale { get; private set; }

        #endregion

        #region variables

        /// <summary>
        /// プレイヤーのロジック。
        /// </summary>
        private readonly PlayerLogic _playerLogic;

        /// <summary>
        /// スケール。
        /// </summary>
        private float _targetScale;

        #endregion

        #region methods

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        public WorldRootLogic(PlayerLogic playerLogic)
        {
            _playerLogic = playerLogic;
            Scale = MaxScale;
            _targetScale = Scale;
        }

        /// <summary>
        /// 目標のスケール値を設定する。
        /// </summary>
        /// <param name="targetScale">目標のスケール値。</param>
        public void SetTargetScale(float targetScale)
        {
            _targetScale = targetScale;
        }

        /// <summary>
        /// 更新する。
        /// </summary>
        public void UpdateStatus()
        {
            // スケールアップ
            if (_targetScale > Scale)
            {
                Scale = Mathf.Min(Scale + ScaleSpeed, _targetScale);
            }
            // スケールダウン
            else if (_targetScale < Scale)
            {
                Scale = Mathf.Max(Scale - ScaleSpeed, _targetScale);
            }
        }

        #endregion
    }
}
