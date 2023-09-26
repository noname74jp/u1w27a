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
        /// 有効な範囲。
        /// </summary>
        public static readonly Rect ValidArea = new(-GridWidth * GridCount * 0.5f, -222.0f, GridWidth * GridCount, 444.0f);

        #endregion
    }
}
