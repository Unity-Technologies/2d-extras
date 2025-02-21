using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    [Serializable]
    internal class AutoTileEditorElement : VisualElement
    {
        private static readonly string s_StylesheetPath =
            "Packages/com.unity.2d.tilemap.extras/Editor/Tiles/AutoTile/UI/AutoTileEditor.uss";

        private static readonly float s_MaxSliderScale = 2.5f;
        
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

            var maskType = new EnumField("Mask Type");
            maskType.bindingPath = "m_MaskType";
            maskType.RegisterValueChangedCallback(MaskTypeChanged);
            
            defaultProperties.Add(maskType);
            
            Add(defaultProperties);
            
            m_TextureList = new ListView();
            m_TextureList.showAddRemoveFooter = true;
            m_TextureList.headerTitle = "Used Textures";
            m_TextureList.showBorder = true;
            m_TextureList.showFoldoutHeader = true;
            m_TextureList.horizontalScrollingEnabled = false;
            m_TextureList.makeItem = MakeTextureItem;
            m_TextureList.bindItem = BindTextureItem;
            m_TextureList.unbindItem = UnbindTextureItem;
            m_TextureList.itemsAdded += ItemListAdded;
            m_TextureList.itemsRemoved += ItemListRemoved;
            m_TextureList.itemsSourceChanged += TexturesChanged;
            Add(m_TextureList);
  
            m_TextureScroller = new ScrollView(ScrollViewMode.Vertical);
            Add(m_TextureScroller);

            var ss = EditorGUIUtility.Load(s_StylesheetPath) as StyleSheet;
            styleSheets.Add(ss);
        }

        private void LoadAutoTileData()
        {
            if (autoTile == null)
                return;

            m_TextureList.itemsSource = m_AutoTile.m_TextureList;
            m_TextureList.Rebuild();
            m_TextureList.RefreshItems();
            PopulateTextureScrollView();
        }
        
        private void LoadAutoTileMaskData()
        {
            if (autoTile == null)
                return;
            
            foreach (var pair in autoTile.m_AutoTileDictionary)
            {
                var mask = pair.Key;
                var autoTileData = pair.Value;
                foreach (var sprite in autoTileData.spriteList)
                {
                    var spriteTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(sprite));
                    if (textureToElementMap.TryGetValue(spriteTexture, out var at))
                    {
                        at.InitialiseSpriteMask(sprite, mask);
                    }
                }
            }
        }

        private void MaskTypeChanged(ChangeEvent<Enum> evt)
        {
            if (evt.previousValue == null || evt.newValue == null
                || ((evt.previousValue.Equals(AutoTile.AutoTileMaskType.Mask_2x2)
                   || evt.newValue.Equals(AutoTile.AutoTileMaskType.Mask_2x2))
                && !Equals(evt.previousValue, evt.newValue)))
            {
                TexturesChanged();
            }
        }
        
        private VisualElement MakeTextureItem()
        {
            var objField = new ObjectField();
            objField.objectType = typeof(Texture2D);
            objField.allowSceneObjects = false;
            return objField;
        }
        
        private void BindTextureItem(VisualElement ve, int index)
        {
            var of = ve.Q<ObjectField>();
            of.SetValueWithoutNotify(m_AutoTile.m_TextureList[index]);
            EventCallback<ChangeEvent<UnityEngine.Object>> callback = evt => TexturePropertyChanged(index, (Texture2D) evt.newValue);
            of.RegisterValueChangedCallback(callback);
            of.userData = callback;
        }

        private void UnbindTextureItem(VisualElement ve, int index)
        {
            var of = ve.Q<ObjectField>();
            of.UnregisterValueChangedCallback((EventCallback<ChangeEvent<UnityEngine.Object>>) of.userData);
        }

        private void TexturePropertyChanged(int index, Texture2D texture2D)
        {
            if (m_AutoTile.m_TextureList[index] == texture2D) 
                return;

            m_AutoTile.m_TextureList[index] = texture2D;
            m_AutoTile.m_TextureScaleList[index] = AutoTile.s_DefaultTextureScale;
            TexturesChanged();
        }

        private void PopulateTextureScrollView()
        {
            textureToElementMap.Clear();
            m_TextureScroller.Clear(); 
            
            if (m_TextureList.itemsSource == null)
                return;

            var count = Math.Min(m_AutoTile.m_TextureScaleList.Count, m_TextureList.itemsSource.Count);
            for (var i = 0; i < count; ++i)
            {
                var texture2D = m_TextureList.itemsSource[i] as Texture2D;
                if (texture2D == null)
                    continue;

                if (textureToElementMap.ContainsKey(texture2D))
                    continue;

                var ve = new VisualElement();
                var at = new AutoTileTextureSource(texture2D, autoTile.m_MaskType, MaskChanged, SaveTile);
                textureToElementMap.Add(texture2D, at);
                
                var he = new VisualElement();
                he.style.flexDirection = FlexDirection.Row;
                var label = new Label("Template");
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                he.Add(label);
                var loadButton = new Button(() =>
                {
                    var template = AutoTileTemplateUtility.LoadTemplateFromFile();
                    if (template != null)
                    {
                        if (autoTile.m_MaskType != template.maskType)
                        {
                            throw new InvalidOperationException($"AutoTile Mask '{autoTile.m_MaskType}' does not match Template Mask '{template.maskType}'");
                        }
                        autoTile.m_AutoTileDictionary.Clear();
                        at.ApplyAutoTileTemplate(template);
                        SaveTile();    
                    }
                    Resources.UnloadAsset(template);
                });
                loadButton.text = "Load";
                loadButton.userData = at;
                he.Add(loadButton);
                var saveButton = new Button(() =>
                {
                    AutoTileTemplateUtility.SaveTemplateToFile(texture2D.width
                        , texture2D.height
                        , autoTile.m_MaskType
                        , at.GetSpriteData());
                });
                saveButton.text = "Save";
                saveButton.userData = at;
                he.Add(saveButton);
                
                var minLength = Math.Max(texture2D.width, texture2D.height);
                var start = 256.0f / minLength;

                var sliderValue = Math.Min(Mathf.Max(start, m_AutoTile.m_TextureScaleList[i]), s_MaxSliderScale);
                
                var slider = new Slider("Scale", start, s_MaxSliderScale, SliderDirection.Horizontal, 0.1f);
                slider.style.flexGrow = 0.9f;
                slider.value = Mathf.Max(start, sliderValue);
                slider.userData = i;
                slider.RegisterValueChangedCallback(evt =>
                {
                    at.ChangeScale(evt.newValue);
                    m_AutoTile.m_TextureScaleList[(int) slider.userData] = evt.newValue;
                    SaveTile();
                });
                he.Add(slider);
                ve.Add(he);
                
                at.ChangeScale(sliderValue);
                
                ve.Add(at);
                
                m_TextureScroller.contentContainer.Add(ve);
            }
            LoadAutoTileMaskData();
        }
        
        private void MaskChanged(Sprite sprite, Texture2D sourceTexture, uint oldMask, uint newMask)
        {
            if (oldMask != 0)
            {
                var spriteList = autoTile.m_AutoTileDictionary[oldMask].spriteList;
                if (spriteList.Count > 2)
                {
                    if (textureToElementMap.TryGetValue(sourceTexture, out var at))
                    {
                        at.SetDuplicate(sprite, false);
                    }
                }
                if (spriteList.Count == 2)
                {
                    foreach (var autoTileSprite in spriteList)
                    {
                        if (textureToElementMap.TryGetValue(sourceTexture, out var at))
                        {
                            at.SetDuplicate(autoTileSprite, false);
                        }
                    }
                }
            }
            
            autoTile.RemoveSprite(sprite, oldMask);
            autoTile.AddSprite(sprite, sourceTexture, newMask);

            if (newMask != 0)
            {
                var spriteList = autoTile.m_AutoTileDictionary[newMask].spriteList;
                if (spriteList.Count < 2)
                    return;
                
                foreach (var autoTileSprite in spriteList)
                {
                    if (textureToElementMap.TryGetValue(sourceTexture, out var at))
                    {
                        at.SetDuplicate(autoTileSprite, true);
                    }
                }
            }
        }
        
        private void ItemListAdded(IEnumerable<int> insertions)
        {
            // Note: m_AutoTile.m_TextureList is increased before this method
            foreach (var i in insertions)
                m_AutoTile.m_TextureScaleList.Insert(i, AutoTile.s_DefaultTextureScale);
            SaveTile();
            m_TextureList.Rebuild();
            TexturesChanged();
        }
        
        private void ItemListRemoved(IEnumerable<int> removals)
        {
            // Note: m_AutoTile.m_TextureList is reduced after this method ends
            foreach (var i in removals)
                m_AutoTile.m_TextureScaleList.RemoveAt(i);
            SaveTile();
            m_TextureList.Rebuild();
            TexturesChanged();
        }
        
        private void TexturesChanged()
        {
            if (m_TextureList.itemsSource == null)
                return;
            
            autoTile.Validate();
            PopulateTextureScrollView();
        }
        
        private void SaveTile()
        {
            // Clear empty values
            var keys = new uint[autoTile.m_AutoTileDictionary.Count];
            autoTile.m_AutoTileDictionary.Keys.CopyTo(keys, 0);
            foreach (var key in keys)
            {
                if (autoTile.m_AutoTileDictionary.TryGetValue(key, out AutoTile.AutoTileData data))
                {
                    if (data.spriteList == null || data.spriteList.Count == 0)
                        autoTile.m_AutoTileDictionary.Remove(key);
                }
            }
            
            EditorUtility.SetDirty(autoTile);
            SceneView.RepaintAll();
        }
    }
}