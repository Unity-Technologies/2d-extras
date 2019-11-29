using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditorInternal;
using System.Collections.Generic;

namespace UnityEditor
{
    [CustomEditor(typeof(AdvancedRuleOverrideTile))]
    public class AdvancedRuleOverrideTileEditor : RuleOverrideTileBaseEditor
    {

        public new AdvancedRuleOverrideTile overrideTile { get { return (target as AdvancedRuleOverrideTile); } }

        List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>> m_Rules = new List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>>();
        ReorderableList m_RuleList;

        static float k_DefaultElementHeight { get { return RuleTileEditor.k_DefaultElementHeight; } }
        static float k_PaddingBetweenRules { get { return RuleTileEditor.k_PaddingBetweenRules; } }
        static float k_SingleLineHeight { get { return RuleTileEditor.k_SingleLineHeight; } }
        static float k_LabelWidth { get { return RuleTileEditor.k_LabelWidth; } }

        void OnEnable()
        {
            if (m_RuleList == null)
            {
                overrideTile.GetOverrides(m_Rules);

                m_RuleList = new ReorderableList(m_Rules, typeof(KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>), false, true, false, false);
                m_RuleList.drawHeaderCallback = DrawHeader;
                m_RuleList.drawElementCallback = DrawElement;
                m_RuleList.elementHeightCallback = GetRuleElementHeight;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            overrideTile.GetOverrides(m_Rules);

            m_RuleList.list = m_Rules;
            m_RuleList.DoLayoutList();
        }

        void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Overrides", EditorStyles.label);
        }

        void DrawOverrideToggle(Rect rect, RuleTile.TilingRule originalRule, RuleTile.TilingRule overrideRule)
        {
            EditorGUI.BeginChangeCheck();

            bool enabled = EditorGUI.Toggle(new Rect(rect.xMin, rect.yMin, rect.width, k_SingleLineHeight), overrideRule != null);

            if (EditorGUI.EndChangeCheck())
            {
                if (enabled)
                    overrideTile[originalRule] = originalRule;
                else
                    overrideTile[originalRule] = null;
            }
        }

        void DrawElement(Rect rect, int index, bool selected, bool focused)
        {
            RuleTile.TilingRule originalRule = m_Rules[index].Key;
            RuleTile.TilingRule overrideRule = m_Rules[index].Value;

            float matrixWidth = k_DefaultElementHeight;

            float xMax = rect.xMax;
            rect.xMax = rect.xMin + 16f;

            DrawOverrideToggle(rect, originalRule, overrideRule);

            rect.xMin = rect.xMax;
            rect.xMax = xMax;

            EditorGUI.BeginChangeCheck();

            bool isOverride = overrideRule != null;
            bool isDefault = index == overrideTile.m_Tile.m_TilingRules.Count;
            if (isDefault)
                DrawDefaultRule(rect, isOverride ? overrideRule : originalRule, isOverride);
            else
                DrawRule(rect, isOverride ? overrideRule : originalRule, isOverride);

            if (EditorGUI.EndChangeCheck())
                SaveTile();
        }

        void DrawRule(Rect rect, RuleTile.TilingRule rule, bool isOverride)
        {
            using (new EditorGUI.DisabledScope(!isOverride))
            {
                float yPos = rect.yMin + 2f;
                float height = rect.height - k_PaddingBetweenRules;
                float matrixWidth = k_DefaultElementHeight;

                BoundsInt ruleBounds = rule.GetBounds();
                BoundsInt ruleGuiBounds = ruleTileEditor.GetRuleGUIBounds(ruleBounds, rule);
                Vector2 matrixSize = ruleTileEditor.GetMatrixSize(ruleGuiBounds);
                Vector2 matrixSizeRate = matrixSize / Mathf.Max(matrixSize.x, matrixSize.y);
                Vector2 matrixRectSize = new Vector2(matrixWidth * matrixSizeRate.x, k_DefaultElementHeight * matrixSizeRate.y);
                Vector2 matrixRectPosition = new Vector2(rect.xMax - matrixWidth * 2f - 10f, yPos);
                matrixRectPosition.x += (matrixWidth - matrixRectSize.x) * 0.5f;
                matrixRectPosition.y += (k_DefaultElementHeight - matrixRectSize.y) * 0.5f;

                Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth * 2f - 20f, height);
                Rect matrixRect = new Rect(matrixRectPosition, matrixRectSize);
                Rect spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, k_DefaultElementHeight);

                ruleTileEditor.RuleInspectorOnGUI(inspectorRect, rule);
                ruleTileEditor.RuleMatrixOnGUI(overrideTile.m_Tile, matrixRect, ruleGuiBounds, rule);
                ruleTileEditor.SpriteOnGUI(spriteRect, rule);
            }
        }

        void DrawDefaultRule(Rect rect, RuleTile.TilingRule rule, bool isOverride)
        {
            using (new EditorGUI.DisabledScope(!isOverride))
            {
                float yPos = rect.yMin + 2f;
                float height = rect.height - k_PaddingBetweenRules;
                float matrixWidth = k_DefaultElementHeight;

                Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth * 2f - 20f, height);
                Rect spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, k_DefaultElementHeight);

                DefaultRuleOnGUI(inspectorRect, rule);
                ruleTileEditor.SpriteOnGUI(spriteRect, rule);
            }
        }

        void DefaultRuleOnGUI(Rect rect, RuleTile.TilingRule rule)
        {
            float y = rect.yMin;

            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Rule");
            EditorGUI.LabelField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), "Default");
            y += k_SingleLineHeight;

            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Game Object");
            rule.m_GameObject = (GameObject)EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), "", rule.m_GameObject, typeof(GameObject), false);
            y += k_SingleLineHeight;

            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Collider");
            rule.m_ColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), rule.m_ColliderType);
            y += k_SingleLineHeight;
        }

        float GetRuleElementHeight(int index)
        {
            if (index == overrideTile.m_Tile.m_TilingRules.Count)
            {
                var originalRule = overrideTile.m_OriginalDefaultTilingRule;
                var overrideRule = overrideTile.m_OverrideDefaultTilingRule;
                return overrideRule != null ? GetRuleElementHeight(overrideRule) : GetRuleElementHeight(originalRule);
            }
            else
            {
                var originalRule = overrideTile.m_Tile.m_TilingRules[index];
                var overrideRule = overrideTile[originalRule];
                return overrideRule != null ? GetRuleElementHeight(overrideRule) : GetRuleElementHeight(originalRule);
            }
        }

        float GetRuleElementHeight(RuleTile.TilingRule rule)
        {
            float height = k_DefaultElementHeight + k_PaddingBetweenRules;
            if (rule != null)
            {
                switch (rule.m_Output)
                {
                    case RuleTile.TilingRule.OutputSprite.Random:
                        height = k_DefaultElementHeight + k_SingleLineHeight * (rule.m_Sprites.Length + 3) + k_PaddingBetweenRules;
                        break;
                    case RuleTile.TilingRule.OutputSprite.Animation:
                        height = k_DefaultElementHeight + k_SingleLineHeight * (rule.m_Sprites.Length + 2) + k_PaddingBetweenRules;
                        break;
                }
            }
            return height;
        }
    }
}
