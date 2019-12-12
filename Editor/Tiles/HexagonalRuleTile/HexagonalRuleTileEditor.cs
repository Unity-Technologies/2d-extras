using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(HexagonalRuleTile), true)]
    [CanEditMultipleObjects]
    public class HexagonalRuleTileEditor : RuleTileEditor
    {

        public HexagonalRuleTile hexTile => target as HexagonalRuleTile;

        public override int GetArrowIndex(Vector3Int position)
        {
            if (position.y % 2 != 0)
            {
                position *= 2;
                position.x += 1;
            }

            if (position.x == 0)
            {
                if (position.y > 0)
                    return hexTile.m_FlatTop ? 3 : 1;
                else
                    return hexTile.m_FlatTop ? 5 : 7;
            }
            else if (position.y == 0)
            {
                if (position.x > 0)
                    return hexTile.m_FlatTop ? 1 : 5;
                else
                    return hexTile.m_FlatTop ? 7 : 3;
            }
            else
            {
                if (position.x < 0 && position.y > 0)
                    return hexTile.m_FlatTop ? 6 : 0;
                else if (position.x > 0 && position.y > 0)
                    return hexTile.m_FlatTop ? 0 : 2;
                else if (position.x < 0 && position.y < 0)
                    return hexTile.m_FlatTop ? 8 : 6;
                else if (position.x > 0 && position.y < 0)
                    return hexTile.m_FlatTop ? 2 : 8;
            }

            return -1;
        }

        public override BoundsInt GetRuleGUIBounds(BoundsInt bounds, RuleTile.TilingRule rule)
        {
            foreach (var n in rule.GetNeighbors())
            {
                if (n.Key.x == bounds.xMax - 1 && n.Key.y % 2 != 0)
                {
                    bounds.xMax++;
                    break;
                }
            }
            return base.GetRuleGUIBounds(bounds, rule);
        }

        public override Vector2 GetMatrixSize(BoundsInt bounds)
        {
            Vector2 size = base.GetMatrixSize(bounds);
            return hexTile.m_FlatTop ? new Vector2(size.y, size.x) : size;
        }

        public override void RuleMatrixOnGUI(RuleTile tile, Rect rect, BoundsInt bounds, RuleTile.TilingRule tilingRule)
        {
            bool flatTop = hexTile.m_FlatTop;

            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            float w = rect.width / (flatTop ? bounds.size.y : bounds.size.x);
            float h = rect.height / (flatTop ? bounds.size.x : bounds.size.y);

            // Grid
            if (flatTop)
            {
                for (int y = 0; y <= bounds.size.y; y++)
                {
                    float left = rect.xMin + y * w;
                    float offset = 0;

                    if (y == 0 && bounds.yMax % 2 == 0)
                        offset = h / 2;
                    else if (y == bounds.size.y && bounds.yMin % 2 != 0)
                        offset = h / 2;

                    Handles.DrawLine(new Vector3(left, rect.yMin + offset), new Vector3(left, rect.yMax - offset));

                    if (y < bounds.size.y)
                    {
                        bool noOffset = (y + bounds.yMax) % 2 != 0;
                        for (int x = 0; x < (noOffset ? (bounds.size.x + 1) : bounds.size.x); x++)
                        {
                            float top = rect.yMin + x * h + (noOffset ? 0 : h / 2);
                            Handles.DrawLine(new Vector3(left, top), new Vector3(left + w, top));
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y <= bounds.size.y; y++)
                {
                    float top = rect.yMin + y * h;
                    float offset = 0;

                    if (y == 0 && bounds.yMax % 2 == 0)
                        offset = w / 2;
                    else if (y == bounds.size.y && bounds.yMin % 2 != 0)
                        offset = w / 2;

                    Handles.DrawLine(new Vector3(rect.xMin + offset, top), new Vector3(rect.xMax - offset, top));

                    if (y < bounds.size.y)
                    {
                        bool noOffset = (y + bounds.yMax) % 2 != 0;
                        for (int x = 0; x < (noOffset ? (bounds.size.x + 1) : bounds.size.x); x++)
                        {
                            float left = rect.xMin + x * w + (noOffset ? 0 : w / 2);
                            Handles.DrawLine(new Vector3(left, top), new Vector3(left, top + h));
                        }
                    }
                }
            }

            var neighbors = tilingRule.GetNeighbors();

            // Icons
            Handles.color = Color.white;
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                int xMax = y % 2 == 0 ? bounds.xMax : (bounds.xMax - 1);
                for (int x = bounds.xMin; x < xMax; x++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    Vector2 offset = new Vector2(x - bounds.xMin, -y + bounds.yMax - 1);
                    Rect r = flatTop ? new Rect(rect.xMin + offset.y * w, rect.yMax - offset.x * h - h, w - 1, h - 1)
                        : new Rect(rect.xMin + offset.x * w, rect.yMin + offset.y * h, w - 1, h - 1);

                    if (y % 2 != 0)
                    {
                        if (flatTop)
                            r.y -= h / 2;
                        else
                            r.x += w / 2;
                    }

                    RuleMatrixIconOnGUI(tilingRule, neighbors, pos, r);
                }
            }
        }

        public override bool HasPreviewGUI()
        {
            return false;
        }
    }
}
