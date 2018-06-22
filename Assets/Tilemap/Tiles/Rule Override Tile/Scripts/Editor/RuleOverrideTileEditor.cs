using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

namespace UnityEditor
{
	[CustomEditor(typeof(RuleOverrideTile))]
	internal class RuleOverrideTileEditor : Editor
	{

		public RuleOverrideTile overrideTile { get { return (target as RuleOverrideTile); } }

		private List<KeyValuePair<Sprite, Sprite>> m_Sprites;
		private List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>> m_Rules;

		ReorderableList m_SpriteList;
		ReorderableList m_RuleList;

		private float k_DefaultElementHeight { get { return RuleTileEditor.k_DefaultElementHeight; } }
		private float k_PaddingBetweenRules { get { return RuleTileEditor.k_PaddingBetweenRules; } }
		private float k_SingleLineHeight { get { return RuleTileEditor.k_SingleLineHeight; } }
		private float k_LabelWidth { get { return RuleTileEditor.k_LabelWidth; } }

		void OnEnable()
		{
			if (m_Sprites == null)
				m_Sprites = new List<KeyValuePair<Sprite, Sprite>>();

			if (m_Rules == null)
				m_Rules = new List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>>();

			if (m_SpriteList == null)
			{
				overrideTile.GetOverrides(m_Sprites);

				m_SpriteList = new ReorderableList(m_Sprites, typeof(KeyValuePair<Sprite, Sprite>), false, true, false, false);
				m_SpriteList.drawHeaderCallback = DrawSpriteHeader;
				m_SpriteList.drawElementCallback = DrawSpriteElement;
				m_SpriteList.elementHeight = k_DefaultElementHeight + k_PaddingBetweenRules;
			}
			if (m_RuleList == null)
			{
				overrideTile.GetOverrides(m_Rules);

				m_RuleList = new ReorderableList(m_Rules, typeof(KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>), false, true, false, false);
				m_RuleList.drawHeaderCallback = DrawRuleHeader;
				m_RuleList.drawElementCallback = DrawRuleElement;
				m_RuleList.elementHeightCallback = GetRuleElementHeight;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Tile"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Advanced"));
			serializedObject.ApplyModifiedProperties();

			if (!overrideTile.m_Advanced)
			{
				using (new EditorGUI.DisabledScope(overrideTile.m_Tile == null))
				{
					EditorGUI.BeginChangeCheck();
					overrideTile.GetOverrides(m_Sprites);

					m_SpriteList.list = m_Sprites;
					m_SpriteList.DoLayoutList();
					if (EditorGUI.EndChangeCheck())
					{
						for (int i = 0; i < targets.Length; i++)
						{
							RuleOverrideTile tile = targets[i] as RuleOverrideTile;
							tile.ApplyOverrides(m_Sprites);
						}
					}
				}
			}
			else
			{
				using (new EditorGUI.DisabledScope(overrideTile.m_Tile == null))
				{
					overrideTile.GetOverrides(m_Rules);

					m_RuleList.list = m_Rules;
					m_RuleList.DoLayoutList();
				}
			}
		}

		private void SaveTile()
		{
			EditorUtility.SetDirty(target);
			SceneView.RepaintAll();
		}

		private void DrawSpriteElement(Rect rect, int index, bool selected, bool focused)
		{
			Sprite originalSprite = m_Sprites[index].Key;
			Sprite overrideSprite = m_Sprites[index].Value;

			rect.y += 2;
			rect.height -= k_PaddingBetweenRules;

			rect.xMax = rect.xMax / 2.0f;
			using (new EditorGUI.DisabledScope(true))
				EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.height, rect.height), originalSprite, typeof(Sprite), false);
			rect.xMin = rect.xMax;
			rect.xMax *= 2.0f;

			EditorGUI.BeginChangeCheck();
			overrideSprite = EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.height, rect.height), overrideSprite, typeof(Sprite), false) as Sprite;
			if (EditorGUI.EndChangeCheck())
			{
				m_Sprites[index] = new KeyValuePair<Sprite, Sprite>(originalSprite, overrideSprite);
				SaveTile();
			}
		}
		private void DrawSpriteHeader(Rect rect)
		{
			float xMax = rect.xMax;
			rect.xMax = rect.xMax / 2.0f;
			GUI.Label(rect, "Original", EditorStyles.label);
			rect.xMin = rect.xMax;
			rect.xMax = xMax;
			GUI.Label(rect, "Override", EditorStyles.label);
		}

		private void DrawRuleElement(Rect rect, int index, bool selected, bool focused)
		{
			RuleTile.TilingRule originalRule = m_Rules[index].Key;
			RuleTile.TilingRule overrideRule = m_Rules[index].Value;

			float matrixWidth = k_DefaultElementHeight;

			float xMax = rect.xMax;
			rect.xMax = rect.xMax / 2.0f + matrixWidth - 10f;

			if (index != overrideTile.m_Tile.m_TilingRules.Count)
				DrawOriginalRuleElement(rect, originalRule);
			else
				DrawOriginalRuleElement(rect, originalRule, true);

			rect.xMin = rect.xMax;
			rect.xMax = xMax;

			EditorGUI.BeginChangeCheck();
			if (index != overrideTile.m_Tile.m_TilingRules.Count)
				DrawOverrideElement(rect, originalRule);
			else
				DrawOverrideDefaultElement(rect, overrideRule);
			if (EditorGUI.EndChangeCheck())
				SaveTile();
		}
		public void DrawOriginalRuleElement(Rect rect, RuleTile.TilingRule originalRule, bool isDefault = false)
		{
			using (new EditorGUI.DisabledScope(true))
			{
				float yPos = rect.yMin + 2f;
				float height = rect.height - k_PaddingBetweenRules;
				float matrixWidth = k_DefaultElementHeight;

				Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth * 2f - 20f, height);
				Rect matrixRect = new Rect(rect.xMax - matrixWidth * 2f - 10f, yPos, matrixWidth, k_DefaultElementHeight);
				Rect spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, k_DefaultElementHeight);

				if (!isDefault)
					RuleTileEditor.RuleInspectorOnGUI(inspectorRect, originalRule);
				else
					RuleOriginalDefaultInspectorOnGUI(inspectorRect, originalRule);
				RuleTileEditor.RuleMatrixOnGUI(matrixRect, originalRule);
				RuleTileEditor.SpriteOnGUI(spriteRect, originalRule);
			}
		}
		private void DrawOverrideElement(Rect rect, RuleTile.TilingRule originalRule)
		{
			float yPos = rect.yMin + 2f;
			float height = rect.height - k_PaddingBetweenRules;
			float matrixWidth = k_DefaultElementHeight;

			Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth - 10f, height);
			Rect spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, k_DefaultElementHeight);

			RuleOverrideInspectorOnGUI(inspectorRect, originalRule);
			RuleTile.TilingRule overrideRule = overrideTile[originalRule];
			if (overrideRule != null)
				RuleTileEditor.SpriteOnGUI(spriteRect, overrideRule);
		}
		private void RuleOriginalDefaultInspectorOnGUI(Rect rect, RuleTile.TilingRule originalRule)
		{
			float y = rect.yMin;

			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Rule");
			EditorGUI.LabelField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), "Default");
			y += k_SingleLineHeight;

			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Collider");
			EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), originalRule.m_ColliderType);
			y += k_SingleLineHeight;
		}
		private void RuleOverrideInspectorOnGUI(Rect rect, RuleTile.TilingRule originalRule)
		{
			RuleTile.TilingRule overrideRule = overrideTile[originalRule];

			float y = rect.yMin;
			EditorGUI.BeginChangeCheck();

			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Enabled");
			bool enabled = EditorGUI.Toggle(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule != null);
			y += k_SingleLineHeight;

			if (EditorGUI.EndChangeCheck())
			{
				if (enabled)
					overrideTile[originalRule] = originalRule;
				else
					overrideTile[originalRule] = null;
				overrideRule = overrideTile[originalRule];
			}

			if (overrideRule == null)
				return;

			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Collider");
			overrideRule.m_ColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule.m_ColliderType);
			y += k_SingleLineHeight;
			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Output");
			overrideRule.m_Output = (RuleTile.TilingRule.OutputSprite)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule.m_Output);
			y += k_SingleLineHeight;

			if (overrideRule.m_Output == RuleTile.TilingRule.OutputSprite.Animation)
			{
				GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Speed");
				overrideRule.m_AnimationSpeed = EditorGUI.FloatField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule.m_AnimationSpeed);
				y += k_SingleLineHeight;
			}
			if (overrideRule.m_Output == RuleTile.TilingRule.OutputSprite.Random)
			{
				GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Noise");
				overrideRule.m_PerlinScale = EditorGUI.Slider(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule.m_PerlinScale, 0.001f, 0.999f);
				y += k_SingleLineHeight;

				GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Shuffle");
				overrideRule.m_RandomTransform = (RuleTile.TilingRule.Transform)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule.m_RandomTransform);
				y += k_SingleLineHeight;
			}

			if (overrideRule.m_Output != RuleTile.TilingRule.OutputSprite.Single)
			{
				GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Size");
				EditorGUI.BeginChangeCheck();
				int newLength = EditorGUI.DelayedIntField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule.m_Sprites.Length);
				if (EditorGUI.EndChangeCheck())
					Array.Resize(ref overrideRule.m_Sprites, Math.Max(newLength, 1));
				y += k_SingleLineHeight;

				for (int i = 0; i < overrideRule.m_Sprites.Length; i++)
				{
					overrideRule.m_Sprites[i] = EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule.m_Sprites[i], typeof(Sprite), false) as Sprite;
					y += k_SingleLineHeight;
				}
			}
		}
		private void DrawOverrideDefaultElement(Rect rect, RuleTile.TilingRule originalRule)
		{
			float yPos = rect.yMin + 2f;
			float height = rect.height - k_PaddingBetweenRules;
			float matrixWidth = k_DefaultElementHeight;

			Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth - 10f, height);
			Rect spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, k_DefaultElementHeight);

			RuleOverrideDefaultInspectorOnGUI(inspectorRect, originalRule);
			if (overrideTile.m_OverrideDefault.m_Enabled)
				RuleTileEditor.SpriteOnGUI(spriteRect, overrideTile.m_OverrideDefault.m_TilingRule);
		}
		private void RuleOverrideDefaultInspectorOnGUI(Rect rect, RuleTile.TilingRule overrideRule)
		{
			float y = rect.yMin;
			EditorGUI.BeginChangeCheck();

			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Enabled");
			bool enabled = EditorGUI.Toggle(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideTile.m_OverrideDefault.m_Enabled);
			y += k_SingleLineHeight;

			if (EditorGUI.EndChangeCheck())
			{
				overrideTile.m_OverrideDefault.m_Enabled = enabled;
				overrideTile.m_OverrideDefault.m_TilingRule = overrideTile.m_OriginalDefault;
			}

			if (!enabled)
				return;

			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Collider");
			overrideRule.m_ColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), overrideRule.m_ColliderType);
			y += k_SingleLineHeight;
		}
		private void DrawRuleHeader(Rect rect)
		{
			overrideTile.Override();

			float matrixWidth = k_DefaultElementHeight;

			float xMax = rect.xMax;
			rect.xMax = rect.xMax / 2.0f + matrixWidth - 10f;
			GUI.Label(rect, "Original", EditorStyles.label);
			rect.xMin = rect.xMax;
			rect.xMax = xMax;
			GUI.Label(rect, "Override", EditorStyles.label);
		}
		private float GetRuleElementHeight(int index)
		{
			if (index != overrideTile.m_Tile.m_TilingRules.Count)
			{
				var overrideRule = overrideTile[overrideTile.m_Tile.m_TilingRules[index]];
				float overrideHeight = GetRuleElementHeight(overrideRule);
				float originalHeight = GetRuleElementHeight(overrideTile.m_Tile.m_TilingRules[index]);
				return Mathf.Max(overrideHeight, originalHeight);
			}
			else
			{
				var overrideRule = overrideTile.m_OverrideDefault.m_Enabled ? overrideTile.m_OverrideDefault.m_TilingRule : null;
				float overrideHeight = GetRuleElementHeight(overrideRule);
				float originalHeight = GetRuleElementHeight(new RuleTile.TilingRule());
				return Mathf.Max(overrideHeight, originalHeight);
			}
		}
		private float GetRuleElementHeight(RuleTile.TilingRule rule)
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
