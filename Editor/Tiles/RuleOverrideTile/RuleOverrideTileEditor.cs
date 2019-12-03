using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditorInternal;
using System.Collections.Generic;

namespace UnityEditor
{
    [CustomEditor(typeof(RuleOverrideTile))]
    public class RuleOverrideTileEditor : RuleOverrideTileBaseEditor
    {

        public new RuleOverrideTile overrideTile => target as RuleOverrideTile;

        List<KeyValuePair<Sprite, Sprite>> m_Sprites = new List<KeyValuePair<Sprite, Sprite>>();
        List<KeyValuePair<GameObject, GameObject>> m_GameObjects = new List<KeyValuePair<GameObject, GameObject>>();
        ReorderableList m_SpriteList;
        ReorderableList m_GameObjectList;

        static float k_SpriteElementHeight = 48;
        static float k_GameObjectElementHeight = 16;
        static float k_PaddingBetweenRules = 4;

        void OnEnable()
        {
            if (m_SpriteList == null)
            {
                overrideTile.GetOverrides(m_Sprites);

                m_SpriteList = new ReorderableList(m_Sprites, typeof(KeyValuePair<Sprite, Sprite>), false, true, false, false);
                m_SpriteList.drawHeaderCallback = DrawSpriteListHeader;
                m_SpriteList.drawElementCallback = DrawSpriteElement;
                m_SpriteList.elementHeightCallback = GetSpriteElementHeight;
            }
            if (m_GameObjectList == null)
            {
                overrideTile.GetOverrides(m_GameObjects);

                m_GameObjectList = new ReorderableList(m_GameObjects, typeof(KeyValuePair<Sprite, Sprite>), false, true, false, false);
                m_GameObjectList.drawHeaderCallback = DrawGameObjectListHeader;
                m_GameObjectList.drawElementCallback = DrawGameObjectElement;
                m_GameObjectList.elementHeightCallback = GetGameObjectElementHeight;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Sprite List
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
                    SaveTile();
                }
            }

            // GameObject List
            EditorGUI.BeginChangeCheck();
            overrideTile.GetOverrides(m_GameObjects);

            m_GameObjectList.list = m_GameObjects;
            m_GameObjectList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    RuleOverrideTile tile = targets[i] as RuleOverrideTile;
                    tile.ApplyOverrides(m_GameObjects);
                    SaveTile();
                }
            }
        }

        void DrawSpriteListHeader(Rect rect)
        {
            float xMax = rect.xMax;
            rect.xMax = rect.xMax / 2.0f;
            GUI.Label(rect, "Original Sprite", EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax = xMax;
            GUI.Label(rect, "Override Sprite", EditorStyles.label);
        }

        void DrawGameObjectListHeader(Rect rect)
        {
            float xMax = rect.xMax;
            rect.xMax = rect.xMax / 2.0f;
            GUI.Label(rect, "Original GameObject", EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax = xMax;
            GUI.Label(rect, "Override GameObject", EditorStyles.label);
        }

        float GetSpriteElementHeight(int index)
        {
            float height = k_SpriteElementHeight + k_PaddingBetweenRules;

            bool isMissing = index >= overrideTile.m_MissingSpriteIndex;
            if (isMissing)
                height += 16;

            return height;
        }

        float GetGameObjectElementHeight(int index)
        {
            float height = k_GameObjectElementHeight + k_PaddingBetweenRules;

            bool isMissing = index >= overrideTile.m_MissingGameObjectIndex;
            if (isMissing)
                height += 16;

            return height;
        }

        void DrawSpriteElement(Rect rect, int index, bool selected, bool focused)
        {
            bool isMissing = index >= overrideTile.m_MissingSpriteIndex;
            if (isMissing)
            {
                EditorGUI.HelpBox(new Rect(rect.xMin, rect.yMin, rect.width, 16), "Original Sprite missing", MessageType.Warning);
                rect.yMin += 16;
            }

            Sprite originalSprite = m_Sprites[index].Key;
            Sprite overrideSprite = m_Sprites[index].Value;
            Rect fullRect = rect;

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
                m_Sprites[index] = new KeyValuePair<Sprite, Sprite>(originalSprite, overrideSprite);
        }

        void DrawGameObjectElement(Rect rect, int index, bool selected, bool focused)
        {
            bool isMissing = index >= overrideTile.m_MissingSpriteIndex;
            if (isMissing)
            {
                EditorGUI.HelpBox(new Rect(rect.xMin, rect.yMin, rect.width, 16), "Original Game Object missing", MessageType.Warning);
                rect.yMin += 16;
            }

            GameObject originalGameObject = m_GameObjects[index].Key;
            GameObject overrideGameObject = m_GameObjects[index].Value;
            Rect fullRect = rect;

            rect.y += 2;
            rect.height -= k_PaddingBetweenRules;

            rect.xMax = rect.xMax / 2.0f;
            using (new EditorGUI.DisabledScope(true))
                EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.width, rect.height), originalGameObject, typeof(GameObject), false);
            rect.xMin = rect.xMax;
            rect.xMax *= 2.0f;

            EditorGUI.BeginChangeCheck();
            overrideGameObject = EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.width, rect.height), overrideGameObject, typeof(GameObject), false) as GameObject;
            if (EditorGUI.EndChangeCheck())
                m_GameObjects[index] = new KeyValuePair<GameObject, GameObject>(originalGameObject, overrideGameObject);
        }
    }
}
