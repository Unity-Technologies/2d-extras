using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    public class AutoTileTextureSource : ScrollView
    {
        private Dictionary<Sprite, AutoTileSpriteSource> spriteToElementMap =
            new Dictionary<Sprite, AutoTileSpriteSource>();

        private Image m_TextureElement;
        private AutoTileSpriteSource.ClickState m_ClickState;
        
        public AutoTileTextureSource(Texture2D texture2D, AutoTile.AutoTileMaskType maskType, Action<Sprite, uint, uint> maskChanged) : base(ScrollViewMode.VerticalAndHorizontal)
        {
            m_TextureElement = new Image();
            Add(m_TextureElement);
            
            m_TextureElement.image = texture2D;
            m_TextureElement.style.width = texture2D.width;
            m_TextureElement.style.height = texture2D.height;

            var assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture2D));
            m_ClickState = new AutoTileSpriteSource.ClickState();
            foreach (var assetAtPath in assetsAtPath)
            {
                var spriteAsset = assetAtPath as Sprite;
                if (spriteAsset == null)
                    continue;
                
                var spriteImage = new AutoTileSpriteSource(spriteAsset, m_ClickState, maskType);
                spriteImage.maskChanged = maskChanged;
                m_TextureElement.Add(spriteImage);
                spriteToElementMap.Add(spriteAsset, spriteImage);
            }

            RegisterCallback<PointerLeaveEvent>((evt) => m_ClickState.isPointerDown = false);
            RegisterCallback<PointerUpEvent>((evt) => m_ClickState.isPointerDown = false);
        }

        internal void InitialiseSpriteMask(Sprite sprite, uint mask)
        {
            if (spriteToElementMap.TryGetValue(sprite, out var atss))
            {
                atss.InitialiseMask(mask);
            }
        }

        private void SetSpriteMask(Sprite sprite, uint mask)
        {
            if (spriteToElementMap.TryGetValue(sprite, out var atss))
            {
                atss.SetMask(mask);
            }
        }

        internal void SetDuplicate(Sprite sprite, bool isDuplicate)
        {
            if (spriteToElementMap.TryGetValue(sprite, out var atss))
            {
                atss.SetDuplicate(isDuplicate);
            }
        }
        
        public void ChangeScale(float newScale)
        {
            m_TextureElement.style.scale = new Vector2(newScale, newScale);
            m_TextureElement.style.width = m_TextureElement.image.width * newScale;
            m_TextureElement.style.height = m_TextureElement.image.height * newScale;
            foreach (var item in spriteToElementMap)
            {
                item.Value.ChangeScale(newScale);
            }
        }
        
        public void LoadTemplateFromFile()
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            var obj = getActiveFolderPath.Invoke(null, new object[0]);
            var pathToCurrentFolder = obj.ToString();

            var templatePath = EditorUtility.OpenFilePanel("Load AutoTile template", pathToCurrentFolder, AutoTileTextureTemplate.kExtension);
            var relativePath = FileUtil.GetProjectRelativePath(templatePath);
            var template = AssetDatabase.LoadAssetAtPath<AutoTileTextureTemplate>(relativePath);
            if (template == null)
                return;

            var matchExact = false;
            foreach (var item in spriteToElementMap)
            {
                foreach (var sprite in template.sprites)
                {
                    var match = false;
                    if (matchExact)
                    {
                        match = Mathf.Approximately(sprite.x, item.Key.rect.x)
                                && Mathf.Approximately(sprite.y, item.Key.rect.y);
                    }
                    else
                    {
                        match = Mathf.Approximately(sprite.x / template.width, item.Key.rect.x / m_TextureElement.image.width)
                                && Mathf.Approximately(sprite.y / template.height, item.Key.rect.y / m_TextureElement.image.height);
                    }
                    if (match)
                    {
                        SetSpriteMask(item.Key, sprite.mask);
                        break;
                    }
                }
            }
        }
        
        public void SaveTemplateToFile()
        {
            var template = ScriptableObject.CreateInstance<AutoTileTextureTemplate>();
            template.width = m_TextureElement.image.width;
            template.height = m_TextureElement.image.height;
            template.sprites = new List<AutoTileTextureTemplate.SpriteData>();
            foreach (var item in spriteToElementMap)
            {
                template.sprites.Add( new AutoTileTextureTemplate.SpriteData()
                {
                    x = item.Key.rect.x,
                    y = item.Key.rect.y,
                    mask = item.Value.mask
                });
            }
            var path = EditorUtility.SaveFilePanelInProject("Save AutoTile template", "New AutoTile Template", AutoTileTextureTemplate.kExtension, "");
            AssetDatabase.CreateAsset(template, path);
        }
    }
}