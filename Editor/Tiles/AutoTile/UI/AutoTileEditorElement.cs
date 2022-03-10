using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    [Serializable]
    public class AutoTileEditorElement : VisualElement
    {
        private static readonly string s_StylesheetPath =
            "Packages/com.unity.2d.tilemap.extras/Editor/Tiles/AutoTile/UI/AutoTileEditor.uss";
        
        private ListView m_TextureList;
        private ScrollView m_TextureScroller;
        private Dictionary<Texture2D, AutoTileTextureSource> textureToElementMap =
            new Dictionary<Texture2D, AutoTileTextureSource>();
        private AutoTile m_AutoTile;
        
        public AutoTile autoTile
        {
            get => m_AutoTile;
            internal set
            {
                m_AutoTile = value;
                LoadAutoTileData();
            }
        }
        
        public AutoTileEditorElement()
        {
            var defaultProperties = new VisualElement();
            var defaultSprite = new ObjectField("Default Sprite");
            defaultSprite.objectType = typeof(Sprite);
            defaultSprite.bindingPath = "m_DefaultSprite";
            defaultProperties.Add(defaultSprite);

            var defaultGameObject = new ObjectField("Default GameObject");
            defaultGameObject.objectType = typeof(GameObject);
            defaultGameObject.bindingPath = "m_DefaultGameObject";
            defaultProperties.Add(defaultGameObject);

            var tileColliderType = new EnumField("Tile Collider");
            tileColliderType.bindingPath = "m_DefaultColliderType";
            defaultProperties.Add(tileColliderType);
            Add(defaultProperties);

            m_TextureList = new ListView();
            m_TextureList.bindingPath = "m_TextureList";
            m_TextureList.showAddRemoveFooter = true;
            m_TextureList.headerTitle = "Used Textures";
            m_TextureList.showBorder = true;
            m_TextureList.showFoldoutHeader = true;
            m_TextureList.horizontalScrollingEnabled = false;
            m_TextureList.itemsSourceChanged += TexturesChanged;
            //m_TextureList.makeItem = MakeTextureItem;
            m_TextureList.bindItem = BindTextureItem;
            m_TextureList.itemsAdded += ItemListChanged;
            m_TextureList.itemsRemoved += ItemListChanged;
            Add(m_TextureList);
  
            m_TextureScroller = new ScrollView(ScrollViewMode.Vertical);
            Add(m_TextureScroller);

            var ss = EditorGUIUtility.Load(s_StylesheetPath) as StyleSheet;
            styleSheets.Add(ss);
        }

        private void ItemListChanged(IEnumerable<int> obj)
        {
            m_TextureList.Rebuild();
            m_TextureList.RefreshItems();
        }

        private VisualElement MakeTextureItem()
        {
            return new PropertyField();
        }
        
        private void BindTextureItem(VisualElement ve, int index)
        {
            var pf = ve.Q<PropertyField>();
            pf.bindingPath = $"m_TextureList.Array.data[{index}]";
            pf.RegisterValueChangeCallback(TexturePropertyChanged);
            pf.MarkDirtyRepaint();
        }

        private void TexturePropertyChanged(SerializedPropertyChangeEvent evt)
        {
            TexturesChanged();
        }

        private void PopulateTextureScrollView()
        {
            textureToElementMap.Clear();
            m_TextureScroller.contentContainer.Clear();
            foreach (var item in m_TextureList.itemsSource)
            {
                var sp = item as SerializedProperty;
                if (sp == null)
                    return;

                var texture2D = sp.objectReferenceValue as Texture2D;
                if (texture2D == null)
                    continue;

                if (textureToElementMap.ContainsKey(texture2D))
                    continue;

                var ve = new VisualElement();
                var at = new AutoTileTextureSource(texture2D, MaskChanged);
                textureToElementMap.Add(texture2D, at);
                
                var he = new VisualElement();
                he.style.flexDirection = FlexDirection.Row;
                var label = new Label("Template");
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                he.Add(label);
                var loadButton = new Button(() =>
                {
                    at.LoadTemplateFromFile();
                    SaveTile();
                });
                loadButton.text = "Load";
                loadButton.userData = at;
                he.Add(loadButton);
                var saveButton = new Button(() => at.SaveTemplateToFile());
                saveButton.text = "Save";
                saveButton.userData = at;
                he.Add(saveButton);

                var slider = new Slider("Scale", 0.1f, 2.0f, SliderDirection.Horizontal, 0.1f);
                slider.style.flexGrow = 0.8f;
                slider.value = 1.0f;
                slider.RegisterValueChangedCallback(evt => at.ChangeScale(evt.newValue));
                he.Add(slider);
                ve.Add(he);
                
                ve.Add(at);
                
                m_TextureScroller.contentContainer.Add(ve);
            }
            LoadAutoTileData();
        }

        private void MaskChanged(Sprite sprite, uint oldMask, uint newMask)
        {
            if (oldMask != 0)
            {
                var spriteList = autoTile.m_AutoTileDictionary[oldMask].spriteList;
                if (spriteList.Count > 2)
                {
                    if (textureToElementMap.TryGetValue(sprite.texture, out var at))
                    {
                        at.SetDuplicate(sprite, false);
                    }
                }
                if (spriteList.Count == 2)
                {
                    foreach (var autoTileSprite in spriteList)
                    {
                        if (textureToElementMap.TryGetValue(autoTileSprite.texture, out var at))
                        {
                            at.SetDuplicate(autoTileSprite, false);
                        }
                    }
                }
            }
            
            autoTile.RemoveSprite(sprite, oldMask);
            autoTile.AddSprite(sprite, newMask);
            SaveTile();

            if (newMask != 0)
            {
                var spriteList = autoTile.m_AutoTileDictionary[newMask].spriteList;
                if (spriteList.Count < 2)
                    return;
                
                foreach (var autoTileSprite in spriteList)
                {
                    if (textureToElementMap.TryGetValue(autoTileSprite.texture, out var at))
                    {
                        at.SetDuplicate(autoTileSprite, true);
                    }
                }
            }
        }
        
        private void TexturesChanged()
        {
            if (m_TextureList.itemsSource == null)
                return;
            
            autoTile.Validate();
            PopulateTextureScrollView();
        }

        private void LoadAutoTileData()
        {
            if (autoTile == null)
                return;
            
            foreach (var pair in autoTile.m_AutoTileDictionary)
            {
                var mask = pair.Key;
                var autoTileData = pair.Value;
                foreach (var sprite in autoTileData.spriteList)
                {
                    if (textureToElementMap.TryGetValue(sprite.texture, out var at))
                    {
                        at.InitialiseSpriteMask(sprite, mask);
                    }
                }
            }
        }
        
        private void SaveTile()
        {
            EditorUtility.SetDirty(autoTile);
            SceneView.RepaintAll();
        }
    }
}