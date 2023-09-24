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
        /// フレームレート[frame]。
        /// </summary>
        public const double FramePerSec = 60.0;

        /// <summary>
        /// 1フレームあたりの時間[s]。
        /// </summary>
        public const double SecondsPerFrame = 1.0 / FramePerSec;

        /// <summary>
        /// 有効な範囲。
        /// </summary>
        public static readonly Rect ValidArea = new(-432.0f, -222.0f, 864.0f, 444.0f);

        #endregion
    }
}