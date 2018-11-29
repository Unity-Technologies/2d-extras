using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(HexagonalRuleTile), true)]
    [CanEditMultipleObjects]
    internal class HexagonalRuleTileEditor : RuleTileEditor
    {
        private static readonly Vector2[] s_PointedTopPositions =
        {
            new Vector2(2f, 1f), new Vector2(1.5f, 2f), new Vector2(0.5f, 2f), new Vector2(0f, 1f), new Vector2(0.5f, 0f), new Vector2(1.5f, 0f)
        };
        private static readonly int[] s_PointedTopArrows = {5, 8, 6, 3, 0, 2};
        private static readonly Vector2[] s_FlatTopPositions =
        {
            new Vector2(1f, 0f), new Vector2(2f, 0.5f), new Vector2(2f, 1.5f), new Vector2(1f, 2f), new Vector2(0f, 1.5f), new Vector2(0f, 0.5f)
        };
        private static readonly int[] s_FlatTopArrows = {1, 2, 8, 7, 6, 0};

        private static Texture2D[] arrows
        {
            get { return RuleTile.arrows; }
        }

        protected override void DoRuleMatrixOnGUI(RuleTile ruleTile, Rect rect, RuleTile.TilingRule tilingRule)
        {
            var hexTile = (HexagonalRuleTile) ruleTile;
            RuleMatrixOnGUI(hexTile, rect, tilingRule, hexTile.m_FlatTop);
        }

        private static void RuleMatrixOnGUI(HexagonalRuleTile hexTile, Rect rect, RuleTile.TilingRule tilingRule, bool flatTop)
        {
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            float w = rect.width / 3f;
            float h = rect.height / 3f;
            
            // Grid
            if (flatTop)
            {
                for (int x = 0; x <= 3; x++)
                {
                    float left = rect.xMin + x * w;
                    float offset = x % 3 > 0 ? 0 : h / 2;
                    Handles.DrawLine(new Vector3(left, rect.yMin + offset), new Vector3(left, rect.yMax - offset));

                    if (x < 3)
                    {
                        bool noOffset = x % 2 > 0;
                        for (int y = 0; y < (noOffset ? 4 : 3); y++)
                        {
                            float top = rect.yMin + y * h + (noOffset ? 0 : h / 2);
                            Handles.DrawLine(new Vector3(left, top), new Vector3(left + w, top));
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y <= 3; y++)
                {
                    float top = rect.yMin + y * h;
                    float offset = y % 3 > 0 ? 0 : w / 2;
                    Handles.DrawLine(new Vector3(rect.xMin + offset, top), new Vector3(rect.xMax - offset, top));

                    if (y < 3)
                    {
                        bool noOffset = y % 2 > 0;
                        for (int x = 0; x < (noOffset ? 4 : 3); x++)
                        {
                            float left = rect.xMin + x * w + (noOffset ? 0 : w / 2);
                            Handles.DrawLine(new Vector3(left, top), new Vector3(left, top + h));
                        }
                    }
                }
            }
            
            // Icons
            Handles.color = Color.white;
            for (int index = 0; index < hexTile.neighborCount; ++index)
            {
                Vector2 position = flatTop ? s_FlatTopPositions[index] : s_PointedTopPositions[index];
                int arrowIndex = flatTop ? s_FlatTopArrows[index] : s_PointedTopArrows[index];
                Rect r = new Rect(rect.xMin + position.x * w, rect.yMin + position.y * h, w - 1, h - 1);
                hexTile.RuleOnGUI(r, arrowIndex, tilingRule.m_Neighbors[index]);
                if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
                {
                    var allConsts = hexTile.m_NeighborType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    var neighbors = allConsts.Select(c => (int)c.GetValue(null)).ToList();
                    neighbors.Sort();

                    int oldIndex = neighbors.IndexOf(tilingRule.m_Neighbors[index]);
                    int newIndex = (int)Mathf.Repeat(oldIndex + GetMouseChange(), neighbors.Count);
                    tilingRule.m_Neighbors[index] = neighbors[newIndex];
                    GUI.changed = true;
                    Event.current.Use();
                }
            }
            // Center
            {
                Rect r = new Rect(rect.xMin + w, rect.yMin + h, w - 1, h - 1);
                switch (tilingRule.m_RuleTransform)
                {
                    case RuleTile.TilingRule.Transform.Rotated:
                        GUI.DrawTexture(r, autoTransforms[0]);
                        break;
                    case RuleTile.TilingRule.Transform.MirrorX:
                        GUI.DrawTexture(r, autoTransforms[1]);
                        break;
                    case RuleTile.TilingRule.Transform.MirrorY:
                        GUI.DrawTexture(r, autoTransforms[2]);
                        break;
                }
                if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
                {
                    tilingRule.m_RuleTransform = (RuleTile.TilingRule.Transform)(((int)tilingRule.m_RuleTransform + GetMouseChange()) % 4);
                    GUI.changed = true;
                    Event.current.Use();
                }
            }
        }

        private static int GetMouseChange()
        {
            return Event.current.button == 1 ? -1 : 1;
        }
    }
}
