using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace UnityEditor
{

	[CustomEditor(typeof(RuleOverrideTile))]
	internal class RuleOverrideTileEditor : Editor
	{

		private List<KeyValuePair<Sprite, Sprite>> m_Sprites;

		ReorderableList m_SpriteList;

		void OnEnable()
		{
			RuleOverrideTile ruleOverrideTile = target as RuleOverrideTile;


			if (m_Sprites == null)
				m_Sprites = new List<KeyValuePair<Sprite, Sprite>>();

			if (m_SpriteList == null)
			{
				ruleOverrideTile.GetOverrides(m_Sprites);

				m_SpriteList = new ReorderableList(m_Sprites, typeof(KeyValuePair<Sprite, Sprite>), false, true, false, false);
				m_SpriteList.drawElementCallback = DrawSpriteElement;
				m_SpriteList.drawHeaderCallback = DrawSpriteHeader;
				m_SpriteList.onSelectCallback = SelectSprite;
				m_SpriteList.elementHeight = 64;
			}
		}

		void OnDisable()
		{
			RuleOverrideTile ruleOverrideTile = target as RuleOverrideTile;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();

			RuleOverrideTile ruleOverrideTile = target as RuleOverrideTile;
			RuleTile ruleTile = ruleOverrideTile.m_Tile;

			ruleOverrideTile.m_Tile = EditorGUILayout.ObjectField("Tile", ruleTile, typeof(RuleTile), false) as RuleTile;

			using (new EditorGUI.DisabledScope(ruleTile == null))
			{
				EditorGUI.BeginChangeCheck();
				ruleOverrideTile.GetOverrides(m_Sprites);

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
		private void DrawSpriteElement(Rect rect, int index, bool selected, bool focused)
		{
			Sprite originalSprite = m_Sprites[index].Key;
			Sprite overrideSprite = m_Sprites[index].Value;

			rect.xMax = rect.xMax / 2.0f;
			GUI.enabled = false;
			EditorGUI.ObjectField(rect, originalSprite.name, originalSprite, typeof(Sprite), false);
			GUI.enabled = true;
			rect.xMin = rect.xMax;
			rect.xMax *= 2.0f;

			EditorGUI.BeginChangeCheck();
			overrideSprite = EditorGUI.ObjectField(rect, overrideSprite ? overrideSprite.name : "", overrideSprite, typeof(Sprite), false) as Sprite;
			if (EditorGUI.EndChangeCheck())
				m_Sprites[index] = new KeyValuePair<Sprite, Sprite>(originalSprite, overrideSprite);
		}

		private void DrawSpriteHeader(Rect rect)
		{
			rect.xMax = rect.xMax / 2.0f;
			GUI.Label(rect, "Original", EditorStyles.label);
			rect.xMin = rect.xMax;
			rect.xMax *= 2.0f;
			GUI.Label(rect, "Override", EditorStyles.label);
		}

		private void SelectSprite(ReorderableList list)
		{
			if (0 <= list.index && list.index < m_Sprites.Count)
				EditorGUIUtility.PingObject(m_Sprites[list.index].Key);
		}
	}
}
