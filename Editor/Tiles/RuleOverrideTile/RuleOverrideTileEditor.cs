using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditorInternal;
using System.Collections.Generic;

namespace UnityEditor
{
    /// <summary>
    /// The Editor for a RuleOverrideTileEditor.
    /// </summary>
    [CustomEditor(typeof(RuleOverrideTile))]
    public class RuleOverrideTileEditor : Editor
    {
        private static class Styles
        {
            public static readonly GUIContent overrideTile = EditorGUIUtility.TrTextContent("Tile"
                , "The Rule Tile to override.");
        }

        /// <summary>
        /// The RuleOverrideTile being edited
        /// </summary>
        public RuleOverrideTile overrideTile => target as RuleOverrideTile;
        /// <summary>
        /// The RuleTileEditor for the overridden instance of the RuleTile
        /// </summary>
        public RuleTileEditor ruleTileEditor
        {
            get
            {
                if (m_RuleTileEditorTarget != overrideTile.m_Tile)
                {
                    DestroyImmediate(m_RuleTileEditor);
                    m_RuleTileEditor = Editor.CreateEditor(overrideTile.m_InstanceTile) as RuleTileEditor;
                    m_RuleTileEditorTarget = overrideTile.m_Tile;
                }
                return m_RuleTileEditor;
            }
        }

        RuleTileEditor m_RuleTileEditor;
        RuleTile m_RuleTileEditorTarget;

        /// <summary>
        /// List of Sprites and overriding Sprites
        /// </summary>
        public List<KeyValuePair<Sprite, Sprite>> m_Sprites = new List<KeyValuePair<Sprite, Sprite>>();
        /// <summary>
        /// List of GameObjects and overriding GameObjects
        /// </summary>
        public List<KeyValuePair<GameObject, GameObject>> m_GameObjects = new List<KeyValuePair<GameObject, GameObject>>();
        private ReorderableList m_SpriteList;
        private ReorderableList m_GameObjectList;
        private int m_MissingOriginalSpriteIndex;
        private int m_MissingOriginalGameObjectIndex;

        /// <summary>
        /// Height for a Sprite Element
        /// </summary>
        public static float k_SpriteElementHeight = 48;
        /// <summary>
        /// Height for a GameObject Element
        /// </summary>
        public static float k_GameObjectElementHeight = 16;
        /// <summary>
        /// Padding between Rule Elements
        /// </summary>
        public static float k_PaddingBetweenRules = 4;

        /// <summary>
        /// OnEnable for the RuleOverrideTileEditor
        /// </summary>
        public virtual void OnEnable()
        {
            if (m_SpriteList == null)
            {
                m_SpriteList = new ReorderableList(m_Sprites, typeof(KeyValuePair<Sprite, Sprite>), false, true, false, false);
                m_SpriteList.drawHeaderCallback = DrawSpriteListHeader;
                m_SpriteList.drawElementCallback = DrawSpriteElement;
                m_SpriteList.elementHeightCallback = GetSpriteElementHeight;
            }
            if (m_GameObjectList == null)
            {
                m_GameObjectList = new ReorderableList(m_GameObjects, typeof(KeyValuePair<Sprite, Sprite>), false, true, false, false);
                m_GameObjectList.drawHeaderCallback = DrawGameObjectListHeader;
                m_GameObjectList.drawElementCallback = DrawGameObjectElement;
                m_GameObjectList.elementHeightCallback = GetGameObjectElementHeight;
            }
        }

        /// <summary>
        /// OnDisable for the RuleOverrideTileEditor
        /// </summary>
        public virtual void OnDisable()
        {
            DestroyImmediate(ruleTileEditor);
            m_RuleTileEditorTarget = null;
        }

        /// <summary>
        /// Draws the Inspector GUI for the RuleOverrideTileEditor
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawTileField();
            DrawCustomFields();

            overrideTile.GetOverrides(m_Sprites, ref m_MissingOriginalSpriteIndex);
            overrideTile.GetOverrides(m_GameObjects, ref m_MissingOriginalGameObjectIndex);

            EditorGUI.BeginChangeCheck();
            m_SpriteList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                overrideTile.ApplyOverrides(m_Sprites);
                SaveTile();
            }

            EditorGUI.BeginChangeCheck();
            m_GameObjectList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                overrideTile.ApplyOverrides(m_GameObjects);
                SaveTile();
            }
        }

        /// <summary>
        /// Draws the header for the Sprite list
        /// </summary>
        /// <param name="rect">GUI Rect to draw the header at</param>
        public void DrawSpriteListHeader(Rect rect)
        {
            float xMax = rect.xMax;
            rect.xMax = rect.xMax / 2.0f;
            GUI.Label(rect, "Original Sprite", EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax = xMax;
            GUI.Label(rect, "Override Sprite", EditorStyles.label);
        }

        /// <summary>
        /// Draws the header for the GameObject list
        /// </summary>
        /// <param name="rect">GUI Rect to draw the header at</param>
        public void DrawGameObjectListHeader(Rect rect)
        {
            float xMax = rect.xMax;
            rect.xMax = rect.xMax / 2.0f;
            GUI.Label(rect, "Original GameObject", EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax = xMax;
            GUI.Label(rect, "Override GameObject", EditorStyles.label);
        }

        /// <summary>
        /// Gets the GUI element height for a Sprite element with the given index
        /// </summary>
        /// <param name="index">Index of the Sprite element</param>
        /// <returns>GUI element height for the Sprite element</returns>
        public float GetSpriteElementHeight(int index)
        {
            float height = k_SpriteElementHeight + k_PaddingBetweenRules;

            bool isMissing = index >= m_MissingOriginalSpriteIndex;
            if (isMissing)
                height += 16;

            return height;
        }

        /// <summary>
        /// Gets the GUI element height for a GameObject element with the given index
        /// </summary>
        /// <param name="index">Index of the GameObject element</param>
        /// <returns>GUI element height for the GameObject element</returns>
        public float GetGameObjectElementHeight(int index)
        {
            float height = k_GameObjectElementHeight + k_PaddingBetweenRules;

            bool isMissing = index >= m_MissingOriginalGameObjectIndex;
            if (isMissing)
                height += 16;

            return height;
        }

        /// <summary>
        /// Draws the Sprite element for the RuleOverride list
        /// </summary>
        /// <param name="rect">Rect to draw the Sprite Element in</param>
        /// <param name="index">Index of the Sprite Element to draw</param>
        /// <param name="active">Whether the Sprite Element is active</param>
        /// <param name="focused">Whether the Sprite Element is focused</param>
        public void DrawSpriteElement(Rect rect, int index, bool active, bool focused)
        {
            bool isMissing = index >= m_MissingOriginalSpriteIndex;
            if (isMissing)
            {
                EditorGUI.HelpBox(new Rect(rect.xMin, rect.yMin, rect.width, 16), "Original Sprite missing", MessageType.Warning);
                rect.yMin += 16;
            }

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
                m_Sprites[index] = new KeyValuePair<Sprite, Sprite>(originalSprite, overrideSprite);
        }

        /// <summary>
        /// Draws the GameObject element for the RuleOverride list
        /// </summary>
        /// <param name="rect">Rect to draw the GameObject Element in</param>
        /// <param name="index">Index of the GameObject Element to draw</param>
        /// <param name="active">Whether the GameObject Element is active</param>
        /// <param name="focused">Whether the GameObject Element is focused</param>
        public void DrawGameObjectElement(Rect rect, int index, bool active, bool focused)
        {
            bool isMissing = index >= m_MissingOriginalGameObjectIndex;
            if (isMissing)
            {
                EditorGUI.HelpBox(new Rect(rect.xMin, rect.yMin, rect.width, 16), "Original GameObject missing", MessageType.Warning);
                rect.yMin += 16;
            }

            GameObject originalGameObject = m_GameObjects[index].Key;
            GameObject overrideGameObject = m_GameObjects[index].Value;

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

        /// <summary>
        /// Draws a field for the RuleTile be overridden
        /// </summary>
        public void DrawTileField()
        {
            EditorGUI.BeginChangeCheck();
            RuleTile tile = EditorGUILayout.ObjectField(Styles.overrideTile, overrideTile.m_Tile, typeof(RuleTile), false) as RuleTile;
            if (EditorGUI.EndChangeCheck())
            {
                if (!LoopCheck(tile))
                {
                    overrideTile.m_Tile = tile;
                    SaveTile();
                }
                else
                {
                    Debug.LogWarning("Circular tile reference");
                }
            }

            bool LoopCheck(RuleTile checkTile)
            {
                if (!overrideTile.m_InstanceTile)
                    return false;

                HashSet<RuleTile> renferenceTils = new HashSet<RuleTile>();
                Add(overrideTile.m_InstanceTile);

                return renferenceTils.Contains(checkTile);

                void Add(RuleTile ruleTile)
                {
                    if (renferenceTils.Contains(ruleTile))
                        return;

                    renferenceTils.Add(ruleTile);

                    var overrideTiles = RuleTileEditor.FindAffectedOverrideTiles(ruleTile);

                    foreach (var overrideTile in overrideTiles)
                        Add(overrideTile.m_InstanceTile);
                }
            }
        }

        /// <summary>
        /// Draw editor fields for custom properties for the RuleOverrideTile
        /// </summary>
        public void DrawCustomFields()
        {
            if (ruleTileEditor)
            {
                ruleTileEditor.target.hideFlags = HideFlags.None;
                ruleTileEditor.DrawCustomFields(true);
                ruleTileEditor.target.hideFlags = HideFlags.NotEditable;
            }
        }

        private void SaveInstanceTileAsset()
        {
            bool assetChanged = false;

            if (overrideTile.m_InstanceTile)
            {
                if (!overrideTile.m_Tile || overrideTile.m_InstanceTile.GetType() != overrideTile.m_Tile.GetType())
                {
                    DestroyImmediate(overrideTile.m_InstanceTile, true);
                    overrideTile.m_InstanceTile = null;
                    assetChanged = true;
                }
            }
            if (!overrideTile.m_InstanceTile)
            {
                if (overrideTile.m_Tile)
                {
                    var t = overrideTile.m_Tile.GetType();
                    RuleTile instanceTile = ScriptableObject.CreateInstance(t) as RuleTile;
                    instanceTile.hideFlags = HideFlags.NotEditable;
                    AssetDatabase.AddObjectToAsset(instanceTile, overrideTile);
                    overrideTile.m_InstanceTile = instanceTile;
                    assetChanged = true;
                }
            }

            if (overrideTile.m_InstanceTile)
            {
                string instanceTileName = overrideTile.m_Tile.name + " (Override)";
                if (overrideTile.m_InstanceTile.name != instanceTileName)
                {
                    overrideTile.m_InstanceTile.name = instanceTileName;
                    assetChanged = true;
                }
            }

            if (assetChanged)
            {
                EditorUtility.SetDirty(overrideTile.m_InstanceTile);
#if UNITY_2021_1       
                AssetDatabase.SaveAssets();
#else
                AssetDatabase.SaveAssetIfDirty(overrideTile.m_InstanceTile);
#endif   
            }
        }

        /// <summary>
        /// Saves any changes to the RuleOverrideTile
        /// </summary>
        public void SaveTile()
        {
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();

            SaveInstanceTileAsset();
        
            if (overrideTile.m_InstanceTile)
            {
                overrideTile.Override();
                RuleTileEditor.UpdateAffectedOverrideTiles(overrideTile.m_InstanceTile);
            }

            if (ruleTileEditor && ruleTileEditor.m_PreviewTilemaps != null)
            {
                foreach (var tilemap in ruleTileEditor.m_PreviewTilemaps)
                    tilemap.RefreshAllTiles();
            }
        }

        /// <summary>
        /// Renders a static preview Texture2D for a RuleOverrideTile asset
        /// </summary>
        /// <param name="assetPath">Asset path of the RuleOverrideTile</param>
        /// <param name="subAssets">Arrays of assets from the given Asset path</param>
        /// <param name="width">Width of the static preview</param>
        /// <param name="height">Height of the static preview </param>
        /// <returns>Texture2D containing static preview for the RuleOverrideTile asset</returns>
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (ruleTileEditor)
                return ruleTileEditor.RenderStaticPreview(assetPath, subAssets, width, height);

            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        /// <summary>
        /// Whether the RuleOverrideTile has a preview GUI
        /// </summary>
        /// <returns>True if RuleOverrideTile has a preview GUI. False if not.</returns>
        public override bool HasPreviewGUI()
        {
            if (ruleTileEditor)
                return ruleTileEditor.HasPreviewGUI();

            return false;
        }

        /// <summary>
        /// Updates preview settings for the RuleOverrideTile.
        /// </summary>
        public override void OnPreviewSettings()
        {
            if (ruleTileEditor)
                ruleTileEditor.OnPreviewSettings();
        }

        /// <summary>
        /// Draws the preview GUI for the RuleTile
        /// </summary>
        /// <param name="rect">Rect to draw the preview GUI</param>
        /// <param name="background">The GUIStyle of the background for the preview</param>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            if (ruleTileEditor)
                ruleTileEditor.OnPreviewGUI(rect, background);
        }
    }
}
