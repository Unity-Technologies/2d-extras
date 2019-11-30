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

        List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>> m_Rules = new List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>>();
        KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>[] m_DefaultRules = { new KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>() };
        ReorderableList m_RuleList;
        ReorderableList m_DefaultRuleList;

        static float k_DefaultElementHeight { get { return RuleTileEditor.k_DefaultElementHeight; } }
        static float k_PaddingBetweenRules { get { return RuleTileEditor.k_PaddingBetweenRules; } }
        static float k_SingleLineHeight { get { return RuleTileEditor.k_SingleLineHeight; } }
        static float k_LabelWidth { get { return RuleTileEditor.k_LabelWidth; } }

        void OnEnable()
        {
            if (m_DefaultRuleList == null)
            {
                m_DefaultRules[0] = new KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>(overrideTile.m_OriginalDefaultTilingRule, overrideTile.m_OverrideDefaultTilingRule);

                m_DefaultRuleList = new ReorderableList(m_DefaultRules, typeof(KeyValuePair<RuleTile.TilingRuleOutput, RuleTile.TilingRuleOutput>), false, true, false, false);
                m_DefaultRuleList.drawHeaderCallback = DrawDefaultRulesHeader;
                m_DefaultRuleList.drawElementCallback = DrawDefaultRuleElement;
                m_DefaultRuleList.elementHeight = k_DefaultElementHeight + 4;
            }
            if (m_RuleList == null)
            {
                overrideTile.GetOverrides(m_Rules);

                m_RuleList = new ReorderableList(m_Rules, typeof(KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>), false, true, false, false);
                m_RuleList.drawHeaderCallback = DrawRulesHeader;
                m_RuleList.drawElementCallback = DrawRuleElement;
                m_RuleList.elementHeightCallback = GetRuleElementHeight;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            m_DefaultRules[0] = new KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>(overrideTile.m_OriginalDefaultTilingRule, overrideTile.m_OverrideDefaultTilingRule);
            m_DefaultRuleList.list = m_DefaultRules;
            m_DefaultRuleList.DoLayoutList();

            overrideTile.GetOverrides(m_Rules);
            m_RuleList.list = m_Rules;
            m_RuleList.DoLayoutList();
        }

        void DrawDefaultRulesHeader(Rect rect)
        {
            GUI.Label(rect, "Default Rule", EditorStyles.label);
        }

        void DrawRulesHeader(Rect rect)
        {
            GUI.Label(rect, "Tiling Rules", EditorStyles.label);
        }

        void DrawDefaultRuleElement(Rect rect, int index, bool selected, bool focused)
        {
            RuleTile.TilingRule originalRule = m_DefaultRules[index].Key;
            RuleTile.TilingRuleOutput overrideRule = m_DefaultRules[index].Value;
            DrawElementInternal(rect, originalRule, overrideRule, true);
        }

        void DrawRuleElement(Rect rect, int index, bool selected, bool focused)
        {
            RuleTile.TilingRule originalRule = m_Rules[index].Key;
            RuleTile.TilingRuleOutput overrideRule = m_Rules[index].Value;
            DrawElementInternal(rect, originalRule, overrideRule, false);
        }

        void DrawElementInternal(Rect rect, RuleTile.TilingRule originalRule, RuleTile.TilingRuleOutput overrideRule, bool isDefault)
        {
            DrawToggleInternal(new Rect(rect.xMin, rect.yMin, 16, rect.height));
            DrawRuleInternal(new Rect(rect.xMin + 16, rect.yMin, rect.width - 16, rect.height));

            void DrawToggleInternal(Rect r)
            {
                EditorGUI.BeginChangeCheck();

                bool enabled = EditorGUI.Toggle(new Rect(r.xMin, r.yMin, r.width, k_SingleLineHeight), overrideRule != null);

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

                bool isOverride = overrideRule != null;
                if (isDefault)
                    DrawDefaultRule(r, isOverride ? overrideRule : originalRule, isOverride);
                else
                    DrawRule(r, isOverride ? overrideRule : originalRule, isOverride, originalRule);

                if (EditorGUI.EndChangeCheck())
                    SaveTile();
            }
        }

        void DrawRule(Rect rect, RuleTile.TilingRuleOutput rule, bool isOverride, RuleTile.TilingRule originalRule)
        {
            using (new EditorGUI.DisabledScope(!isOverride))
            {
                float yPos = rect.yMin + 2f;
                float height = rect.height - k_PaddingBetweenRules;
                float matrixWidth = k_DefaultElementHeight;

                BoundsInt ruleBounds = originalRule.GetBounds();
                BoundsInt ruleGuiBounds = ruleTileEditor.GetRuleGUIBounds(ruleBounds, originalRule);
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
                ruleTileEditor.SpriteOnGUI(spriteRect, rule);

                using (new EditorGUI.DisabledScope(true))
                    ruleTileEditor.RuleMatrixOnGUI(overrideTile.m_InstanceTile, matrixRect, ruleGuiBounds, originalRule);
            }
        }

        void DrawDefaultRule(Rect rect, RuleTile.TilingRuleOutput rule, bool isOverride)
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

        void DefaultRuleOnGUI(Rect rect, RuleTile.TilingRuleOutput rule)
        {
            float y = rect.yMin;

            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Game Object");
            rule.m_GameObject = (GameObject)EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), "", rule.m_GameObject, typeof(GameObject), false);
            y += k_SingleLineHeight;

            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Collider");
            rule.m_ColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), rule.m_ColliderType);
            y += k_SingleLineHeight;
        }

        float GetRuleElementHeight(int index)
        {
            var originalRule = overrideTile.m_Tile.m_TilingRules[index];
            var overrideRule = overrideTile[originalRule];
            return overrideRule != null ? ruleTileEditor.GetElementHeight(overrideRule) : ruleTileEditor.GetElementHeight(originalRule);
        }
    }
}
