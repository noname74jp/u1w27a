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
            var rect = new Rect((Defines.ValidArea.position - logic.Location) * scale, Defines.ValidArea.size * scale);

            // x方向を制限
            if (rect.xMin > Defines.ValidArea.xMin)
            {
                rect.x += Defines.ValidArea.xMin - rect.xMin;
            }
            else if (rect.xMax < Defines.ValidArea.xMax)
            {
                rect.x += Defines.ValidArea.xMax - rect.xMax;
            }

            // y方向を制限
            if (rect.yMin > Defines.ValidArea.yMin)
            {
                rect.y += Defines.ValidArea.yMin - rect.yMin;
            }
            else if (rect.yMax < Defines.ValidArea.yMax)
            {
                rect.y += Defines.ValidArea.yMax - rect.yMax;
            }

            // transformを更新
            var transformAlias = transform;
            transformAlias.position = rect.center;
            transformAlias.localScale = Vector3.one * scale;
        }
    }
}
