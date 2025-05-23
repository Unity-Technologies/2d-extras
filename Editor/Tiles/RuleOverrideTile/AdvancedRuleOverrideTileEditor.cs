using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
    /// <summary>
    ///     The Editor for an AdvancedRuleOverrideTileEditor.
    /// </summary>
    [CustomEditor(typeof(AdvancedRuleOverrideTile))]
    public class AdvancedRuleOverrideTileEditor : RuleOverrideTileEditor
    {
        private int m_MissingOriginalRuleIndex;
        private ReorderableList m_RuleList;

        private readonly List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>> m_Rules = new();
        private readonly HashSet<int> m_UniqueIds = new();

        /// <summary>
        ///     The AdvancedRuleOverrideTile being edited.
        /// </summary>
        public new AdvancedRuleOverrideTile overrideTile => target as AdvancedRuleOverrideTile;

        private static float k_DefaultElementHeight => RuleTileEditor.k_DefaultElementHeight;
        private static float k_SingleLineHeight => RuleTileEditor.k_SingleLineHeight;

        /// <summary>
        ///     OnEnable for the AdvancedRuleOverrideTileEditor
        /// </summary>
        public override void OnEnable()
        {
            if (m_RuleList == null)
            {
                m_RuleList = new ReorderableList(m_Rules,
                    typeof(KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>), false, true, false, false);
                m_RuleList.drawHeaderCallback = DrawRulesHeader;
                m_RuleList.drawElementCallback = DrawRuleElement;
                m_RuleList.elementHeightCallback = GetRuleElementHeight;
            }
        }

        /// <summary>
        ///     Draws the Inspector GUI for the AdvancedRuleOverrideTileEditor
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawTileField();

            EditorGUI.BeginChangeCheck();
            overrideTile.m_DefaultSprite =
                EditorGUILayout.ObjectField(Styles.defaultSprite, overrideTile.m_DefaultSprite, typeof(Sprite),
                    false) as Sprite;
            overrideTile.m_DefaultGameObject = EditorGUILayout.ObjectField(Styles.defaultGameObject,
                overrideTile.m_DefaultGameObject, typeof(GameObject), false) as GameObject;
            overrideTile.m_DefaultColliderType =
                (Tile.ColliderType)EditorGUILayout.EnumPopup(Styles.defaultCollider,
                    overrideTile.m_DefaultColliderType);
            if (EditorGUI.EndChangeCheck())
                SaveTile();

            DrawCustomFields();

            if (overrideTile.m_Tile)
            {
                ValidateRuleTile(overrideTile.m_Tile);
                overrideTile.GetOverrides(m_Rules, ref m_MissingOriginalRuleIndex);
            }

            m_RuleList.DoLayoutList();
        }

        private void ValidateRuleTile(RuleTile ruleTile)
        {
            // Ensure that each Tiling Rule in the RuleTile has a unique ID
            m_UniqueIds.Clear();
            var startId = 0;
            foreach (var rule in ruleTile.m_TilingRules)
            {
                if (m_UniqueIds.Contains(rule.m_Id))
                {
                    do
                    {
                        rule.m_Id = startId++;
                    } while (m_UniqueIds.Contains(rule.m_Id));

                    EditorUtility.SetDirty(ruleTile);
                }

                m_UniqueIds.Add(rule.m_Id);
                startId++;
            }
        }

        /// <summary>
        ///     Draws the Header for the Rule list
        /// </summary>
        /// <param name="rect">Rect to draw the header in</param>
        public void DrawRulesHeader(Rect rect)
        {
            GUI.Label(rect, "Tiling Rules", EditorStyles.label);
        }

        /// <summary>
        ///     Draws the Rule element for the Rule list
        /// </summary>
        /// <param name="rect">Rect to draw the Rule Element in</param>
        /// <param name="index">Index of the Rule Element to draw</param>
        /// <param name="active">Whether the Rule Element is active</param>
        /// <param name="focused">Whether the Rule Element is focused</param>
        public void DrawRuleElement(Rect rect, int index, bool active, bool focused)
        {
            var originalRule = m_Rules[index].Key;
            if (originalRule == null)
                return;

            var overrideRule = m_Rules[index].Value;
            var isMissing = index >= m_MissingOriginalRuleIndex;

            DrawToggleInternal(new Rect(rect.xMin, rect.yMin, 16, rect.height));
            DrawRuleInternal(new Rect(rect.xMin + 16, rect.yMin, rect.width - 16, rect.height));

            void DrawToggleInternal(Rect r)
            {
                EditorGUI.BeginChangeCheck();

                var enabled = EditorGUI.Toggle(new Rect(r.xMin, r.yMin, r.width, k_SingleLineHeight),
                    overrideRule != null);

                if (EditorGUI.EndChangeCheck())
                {
                    if (enabled)
                        overrideTile[originalRule] = originalRule;
                    else
                        overrideTile[originalRule] = null;

                    SaveTile();
                }
            }

            void DrawRuleInternal(Rect r)
            {
                EditorGUI.BeginChangeCheck();

                DrawRule(r, overrideRule ?? originalRule, overrideRule != null, originalRule, isMissing);

                if (EditorGUI.EndChangeCheck())
                    SaveTile();
            }
        }

        /// <summary>
        ///     Draw a Rule Override for the AdvancedRuleOverrideTileEditor
        /// </summary>
        /// <param name="rect">Rect to draw the Rule in</param>
        /// <param name="rule">The Rule Override to draw</param>
        /// <param name="isOverride">Whether the original Rule is being overridden</param>
        /// <param name="originalRule">Original Rule to override</param>
        /// <param name="isMissing">Whether the original Rule is missing</param>
        public void DrawRule(Rect rect, RuleTile.TilingRuleOutput rule, bool isOverride,
            RuleTile.TilingRule originalRule, bool isMissing)
        {
            if (isMissing)
            {
                EditorGUI.HelpBox(new Rect(rect.xMin, rect.yMin, rect.width, 16), "Original Tiling Rule missing",
                    MessageType.Warning);
                rect.yMin += 16;
            }

            using (new EditorGUI.DisabledScope(!isOverride))
            {
                var yPos = rect.yMin + 2f;
                var height = rect.height - k_PaddingBetweenRules;
                var matrixWidth = k_DefaultElementHeight;

                var ruleBounds = originalRule.GetBounds();
                var ruleGuiBounds = ruleTileEditor.GetRuleGUIBounds(ruleBounds, originalRule);
                var matrixSize = ruleTileEditor.GetMatrixSize(ruleGuiBounds);
                var matrixSizeRate = matrixSize / Mathf.Max(matrixSize.x, matrixSize.y);
                var matrixRectSize = new Vector2(matrixWidth * matrixSizeRate.x,
                    k_DefaultElementHeight * matrixSizeRate.y);
                var matrixRectPosition = new Vector2(rect.xMax - matrixWidth * 2f - 10f, yPos);
                matrixRectPosition.x += (matrixWidth - matrixRectSize.x) * 0.5f;
                matrixRectPosition.y += (k_DefaultElementHeight - matrixRectSize.y) * 0.5f;

                var inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth * 2f - 20f, height);
                var matrixRect = new Rect(matrixRectPosition, matrixRectSize);
                var spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, k_DefaultElementHeight);

                ruleTileEditor.RuleInspectorOnGUI(inspectorRect, rule);
                ruleTileEditor.SpriteOnGUI(spriteRect, rule);

                if (!isMissing)
                    using (new EditorGUI.DisabledScope(true))
                    {
                        ruleTileEditor.RuleMatrixOnGUI(overrideTile.m_InstanceTile, matrixRect, ruleGuiBounds,
                            originalRule);
                    }
            }
        }

        /// <summary>
        ///     Returns the height for an indexed Rule Element
        /// </summary>
        /// <param name="index">Index of the Rule Element</param>
        /// <returns>Height of the indexed Rule Element</returns>
        public float GetRuleElementHeight(int index)
        {
            var originalRule = m_Rules[index].Key;
            var overrideRule = m_Rules[index].Value;
            var height = overrideRule != null
                ? ruleTileEditor.GetElementHeight(overrideRule)
                : ruleTileEditor.GetElementHeight(originalRule);

            var isMissing = index >= m_MissingOriginalRuleIndex;
            if (isMissing)
                height += 16;

            return height;
        }

        private static class Styles
        {
            public static readonly GUIContent defaultSprite = EditorGUIUtility.TrTextContent("Default Sprite"
                , "Overrides the default Sprite for the original Rule Tile.");

            public static readonly GUIContent defaultGameObject = EditorGUIUtility.TrTextContent("Default GameObject"
                , "Overrides the default GameObject for the original Rule Tile.");

            public static readonly GUIContent defaultCollider = EditorGUIUtility.TrTextContent("Default Collider"
                , "Overrides the default Collider for the original Rule Tile.");
        }
    }
}
