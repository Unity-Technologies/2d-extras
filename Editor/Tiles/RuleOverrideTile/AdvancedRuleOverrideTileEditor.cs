using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditorInternal;
using System.Collections.Generic;

namespace UnityEditor
{
    [CustomEditor(typeof(AdvancedRuleOverrideTile))]
    public class AdvancedRuleOverrideTileEditor : RuleOverrideTileEditor
    {

        public new AdvancedRuleOverrideTile overrideTile => target as AdvancedRuleOverrideTile;

        List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>> m_Rules = new List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>>();
        ReorderableList m_RuleList;
        int m_MissingOriginalRuleIndex;

        static float k_DefaultElementHeight { get { return RuleTileEditor.k_DefaultElementHeight; } }
        static float k_SingleLineHeight { get { return RuleTileEditor.k_SingleLineHeight; } }
        static float k_LabelWidth { get { return RuleTileEditor.k_LabelWidth; } }

        public override void OnEnable()
        {
            if (m_RuleList == null)
            {
                m_RuleList = new ReorderableList(m_Rules, typeof(KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>), false, true, false, false);
                m_RuleList.drawHeaderCallback = DrawRulesHeader;
                m_RuleList.drawElementCallback = DrawRuleElement;
                m_RuleList.elementHeightCallback = GetRuleElementHeight;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawTileField();

            EditorGUI.BeginChangeCheck();
            overrideTile.m_DefaultSprite = EditorGUILayout.ObjectField("Default Sprite", overrideTile.m_DefaultSprite, typeof(Sprite), false) as Sprite;
            overrideTile.m_DefaultGameObject = EditorGUILayout.ObjectField("Default Game Object", overrideTile.m_DefaultGameObject, typeof(GameObject), false) as GameObject;
            overrideTile.m_DefaultColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default Collider", overrideTile.m_DefaultColliderType);
            if (EditorGUI.EndChangeCheck())
                SaveTile();

            DrawCustomFields();

            if (overrideTile.m_Tile)
                overrideTile.GetOverrides(m_Rules, ref m_MissingOriginalRuleIndex);

            m_RuleList.DoLayoutList();
        }

        public void DrawRulesHeader(Rect rect)
        {
            GUI.Label(rect, "Tiling Rules", EditorStyles.label);
        }

        public void DrawRuleElement(Rect rect, int index, bool selected, bool focused)
        {
            RuleTile.TilingRule originalRule = m_Rules[index].Key;
            RuleTile.TilingRuleOutput overrideRule = m_Rules[index].Value;
            bool isMissing = index >= m_MissingOriginalRuleIndex;

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

                DrawRule(r, overrideRule ?? originalRule, overrideRule != null, originalRule, isMissing);

                if (EditorGUI.EndChangeCheck())
                    SaveTile();
            }
        }

        public void DrawRule(Rect rect, RuleTile.TilingRuleOutput rule, bool isOverride, RuleTile.TilingRule originalRule, bool isMissing)
        {
            if (isMissing)
            {
                EditorGUI.HelpBox(new Rect(rect.xMin, rect.yMin, rect.width, 16), "Original Tiling Rule missing", MessageType.Warning);
                rect.yMin += 16;
            }

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

                if (!isMissing)
                    using (new EditorGUI.DisabledScope(true))
                        ruleTileEditor.RuleMatrixOnGUI(overrideTile.m_InstanceTile, matrixRect, ruleGuiBounds, originalRule);
            }
        }

        public float GetRuleElementHeight(int index)
        {
            var originalRule = m_Rules[index].Key;
            var overrideRule = m_Rules[index].Value;
            float height = overrideRule != null ? ruleTileEditor.GetElementHeight(overrideRule) : ruleTileEditor.GetElementHeight(originalRule);

            bool isMissing = index >= m_MissingOriginalRuleIndex;
            if (isMissing)
                height += 16;

            return height;
        }
    }
}
