using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(IsometricRuleTile), true)]
    [CanEditMultipleObjects]
    public class IsometricRuleTileEditor : RuleTileEditor
    {

        private static readonly int[] s_Arrows = { 7, 8, 5, 6, -1, 2, 3, 0, 1 };

        public override int GetArrowIndex(Vector3Int position)
        {
            return s_Arrows[base.GetArrowIndex(position)];
        }

        public override Vector2 GetMatrixSize(BoundsInt bounds)
        {
            float p = Mathf.Pow(2, 0.5f);
            float w = (bounds.size.x / p + bounds.size.y / p) * k_SingleLineHeight;
            return new Vector2(w, w);
        }

        public override void RuleMatrixOnGUI(RuleTile ruleTile, Rect rect, BoundsInt bounds, RuleTile.TilingRule tilingRule)
        {
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            float w = rect.width / bounds.size.x;
            float h = rect.height / bounds.size.y;

            // Grid
            float d = rect.width / (bounds.size.x + bounds.size.y);
            for (int y = 0; y <= bounds.size.y; y++)
            {
                float left = rect.xMin + d * y;
                float top = rect.yMin + d * y;
                float right = rect.xMax - d * (bounds.size.y - y);
                float bottom = rect.yMax - d * (bounds.size.y - y);
                Handles.DrawLine(new Vector3(left, bottom), new Vector3(right, top));
            }
            for (int x = 0; x <= bounds.size.x; x++)
            {
                float left = rect.xMin + d * x;
                float top = rect.yMax - d * x;
                float right = rect.xMax - d * (bounds.size.x - x);
                float bottom = rect.yMin + d * (bounds.size.x - x);
                Handles.DrawLine(new Vector3(left, bottom), new Vector3(right, top));
            }
            Handles.color = Color.white;

            var neighbors = tilingRule.GetNeighbors();

            // Icons
            float iconSize = rect.width / (bounds.size.x + bounds.size.y);
            var rect2 = new Rect(rect);
            rect2.xMin += iconSize * 0.5f;
            rect2.yMin += iconSize * 0.5f;
            rect2.xMax -= iconSize * 0.5f;
            rect2.yMax -= iconSize * 0.5f;
            iconSize = rect2.width / (bounds.size.x + bounds.size.y - 1);
            float p = Mathf.Pow(2, 0.5f);

            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    Vector3Int offset = new Vector3Int(pos.x - bounds.xMin, pos.y - bounds.yMin, 0);
                    Rect r = new Rect(
                        rect2.xMin + iconSize * (offset.x + offset.y) - iconSize * 0.5f + d * 0.5f,
                        rect2.yMin + iconSize * (offset.y - offset.x) + rect2.height - bounds.size.y * iconSize,
                        iconSize, iconSize
                    );
                    Vector2 center = r.center;
                    r.size *= p;
                    r.center = center;
                    if (x != 0 || y != 0)
                    {
                        if (neighbors.ContainsKey(pos))
                        {
                            RuleOnGUI(r, pos, neighbors[pos]);
                            RuleTooltipOnGUI(r, neighbors[pos]);
                        }
                        if (RuleNeighborUpdate(r, tilingRule, neighbors, pos))
                        {
                            tile.UpdateNeighborPositions();
                        }
                    }
                    else
                    {
                        RuleTransformOnGUI(r, tilingRule.m_RuleTransform);
                        if (RuleTransformUpdate(r, tilingRule))
                        {
                            tile.UpdateNeighborPositions();
                        }
                    }
                }
            }
        }

        public override bool ContainsMousePosition(Rect rect)
        {
            var center = rect.center;
            var halfWidth = rect.width / 2;
            var halfHeight = rect.height / 2;
            var mouseFromCenter = Event.current.mousePosition - center;
            var xAbs = Mathf.Abs(Vector2.Dot(mouseFromCenter, Vector2.right));
            var yAbs = Mathf.Abs(Vector2.Dot(mouseFromCenter, Vector2.up));
            return (xAbs / halfWidth + yAbs / halfHeight) <= 1;
        }
    }
}
