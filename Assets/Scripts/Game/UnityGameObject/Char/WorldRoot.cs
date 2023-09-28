using Game.Logic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    /// <summary>
    /// ワールドのルート。
    /// </summary>
    public class WorldRoot : MonoBehaviour
    {
        /// <summary>
        /// 位置とスケールを更新する。
        /// </summary>
        /// <param name="logic">更新に使用するロジック。</param>
        public void UpdateStatus(PlayerLogic logic)
        {
            // スケールを計算
            // TODO: nn74: scaleの値は仮
            var scale = logic.Location.y >= 0.0f ? 1.0f : 1.0f - logic.Location.y * 0.0075f;

            // スケールを考慮した、プレイヤーが中央になる領域を作成
            var rect = new Rect((Defines.PlayerValidArea.position - logic.Location) * scale, Defines.PlayerValidArea.size * scale);

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
            transformAlias.localScale = Vector3.one * scale;
        }
    }
}
