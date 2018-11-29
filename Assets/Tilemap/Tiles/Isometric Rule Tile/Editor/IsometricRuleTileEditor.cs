using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(IsometricRuleTile), true)]
    [CanEditMultipleObjects]
    internal class IsometricRuleTileEditor : RuleTileEditor
    {
        private static readonly int[, ] s_Arrows =
        {
            { 3, 0, 1 },
            { 6, 9, 2 },
            { 7, 8, 5 },
        };

        protected override void DoRuleMatrixOnGUI(RuleTile ruleTile, Rect rect, RuleTile.TilingRule tilingRule)
        {
            var isoTile = (IsometricRuleTile) ruleTile;
            RuleMatrixOnGUI(isoTile, rect, tilingRule);
        }

        private static void RuleMatrixOnGUI(IsometricRuleTile isoTile, Rect rect, RuleTile.TilingRule tilingRule)
        {
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            int index = 0;
            float w = rect.width / 3f;
            float h = rect.height / 3f;
            
            // Grid
            for (int y = 0; y <= 3; y++)
            {
                float left = rect.xMin + (y * rect.width) / 6;
                float right = left + rect.width / 2;
                float bottom = rect.yMin + (y * rect.height) / 6;
                float top = bottom + rect.height / 2;
                Handles.DrawLine(new Vector3(left, top), new Vector3(right, bottom));
            }
            for (int x = 0; x <= 3; x++)
            {
                float left = rect.xMin + (x * rect.width) / 6;
                float right = left + rect.width / 2;
                float top = rect.yMax - (x * rect.height) / 6;
                float bottom = top - rect.height / 2;
                Handles.DrawLine(new Vector3(left, bottom), new Vector3(right, top));
            }
            Handles.color = Color.white;

            // Icons
            for (int y = 0; y <= 2; y++)
            {
                for (int x = 0; x <= 2; x++)
                {
                    Rect r = new Rect(
                        rect.xMin + ((x + y) * rect.width) / 6, 
                        rect.yMin + ((2 - x + y) * rect.height) / 6, 
                        w - 1, h - 1);
                    if (x != 1 || y != 1)
                    {
                        isoTile.RuleOnGUI(r, s_Arrows[y, x], tilingRule.m_Neighbors[index]);
                        if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
                        {
                            var allConsts = isoTile.m_NeighborType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                            var neighbors = allConsts.Select(c => (int)c.GetValue(null)).ToList();
                            neighbors.Sort();

                            int oldIndex = neighbors.IndexOf(tilingRule.m_Neighbors[index]);
                            int newIndex = (int)Mathf.Repeat(oldIndex + GetMouseChange(), neighbors.Count);
                            tilingRule.m_Neighbors[index] = neighbors[newIndex];
                            GUI.changed = true;
                            Event.current.Use();
                        }

                        index++;
                    }
                    else
                    {
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

                        if (Event.current.type == EventType.MouseDown && ContainsMousePosition(r))
                        {
                            tilingRule.m_RuleTransform = (RuleTile.TilingRule.Transform)(((int)tilingRule.m_RuleTransform + 1) % 4);
                            GUI.changed = true;
                            Event.current.Use();
                        }
                    }
                }
            }
        }

        private static bool ContainsMousePosition(Rect r)
        {
            var center = r.center;
            var halfWidth = r.width / 2;
            var halfHeight = r.height / 2;
            var mouseFromCenter = Event.current.mousePosition - center;
            var xAbs = Mathf.Abs(Vector2.Dot(mouseFromCenter, Vector2.right));
            var yAbs = Mathf.Abs(Vector2.Dot(mouseFromCenter, Vector2.up));
            return (xAbs / halfWidth + yAbs / halfHeight) <= 1;
        }
        
        private static int GetMouseChange()
        {
            return Event.current.button == 1 ? -1 : 1;
        }
    }
}
