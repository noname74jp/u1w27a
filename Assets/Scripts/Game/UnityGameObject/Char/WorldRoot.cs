using Game.Logic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    /// <summary>
    /// ワールドのルート。
    /// </summary>
    public class WorldRoot : MonoBehaviour
    {
        #region variables

        /// <summary>
        /// ロジック。
        /// </summary>
        private WorldRootLogic _logic;

        #endregion

        #region methods

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="logic">ロジック。</param>
        public void Initialize(WorldRootLogic logic)
        {
            _logic = logic;
            UpdateStatus();
        }

        /// <summary>
        /// 位置とスケールを更新する。
        /// </summary>
        public void UpdateStatus()
        {
            // スケールを考慮した、プレイヤーが中央になる領域を作成
            var rect = new Rect((Defines.PlayerValidArea.position - _logic.Location) * _logic.Scale, Defines.PlayerValidArea.size * _logic.Scale);

            // x方向を制限
            if (rect.xMin > Defines.PlayerValidArea.xMin)
            {
                rect.x += Defines.PlayerValidArea.xMin - rect.xMin;
            }
            else if (rect.xMax < Defines.PlayerValidArea.xMax)
            {
                rect.x += Defines.PlayerValidArea.xMax - rect.xMax;
            }

            // y方向を制限
            if (rect.yMin > Defines.PlayerValidArea.yMin)
            {
                rect.y += Defines.PlayerValidArea.yMin - rect.yMin;
            }
            else if (rect.yMax < Defines.PlayerValidArea.yMax)
            {
                rect.y += Defines.PlayerValidArea.yMax - rect.yMax;
            }

            // transformを更新
            var transformAlias = transform;
            transformAlias.position = rect.center;
            transformAlias.localScale = Vector3.one * _logic.Scale;
        }

        #endregion
    }
}
