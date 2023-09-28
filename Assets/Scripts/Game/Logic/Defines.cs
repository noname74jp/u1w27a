using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// 定義。
    /// </summary>
    public static class Defines
    {
        #region constants

        /// <summary>
        /// フレームレート[frame/s]。
        /// </summary>
        public const double FramePerSec = 240.0;

        /// <summary>
        /// 1フレームあたりの時間[s/frame]。
        /// </summary>
        public const double SecondsPerFrame = 1.0 / FramePerSec;

        /// <summary>
        /// グリッドの数。
        /// </summary>
        public const int GridCount = 12;

        /// <summary>
        /// グリッドの横幅。
        /// </summary>
        public const float GridWidth = 72.0f;

        /// <summary>
        /// プレイヤーの有効な範囲。
        /// </summary>
        public static readonly Rect PlayerValidArea = new(-GridWidth * GridCount * 0.5f, -222.0f, GridWidth * GridCount, 444.0f);

        /// <summary>
        /// 弾の有効な範囲。
        /// </summary>
        public static readonly Rect BulletValidArea = new(-480.0f, -270.0f, 960.0f, 540.0f);

        /// <summary>
        /// 敵の有効な範囲。
        /// </summary>
        public static readonly Rect EnemyValidArea = new(-480.0f - 64.0f, -270.0f - 64.0f, 960.0f + 128.0f, 540.0f + 128.0f);

        #endregion
    }
}
