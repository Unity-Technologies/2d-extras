using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    internal class AutoTileTextureSource : ScrollView
    {
        private Dictionary<Sprite, AutoTileSpriteSource> m_SpriteToElementMap =
            new Dictionary<Sprite, AutoTileSpriteSource>();

        private Image m_TextureElement;
        private AutoTileSpriteSource.ClickState m_ClickState;
        private Action m_EditStopped;

        public AutoTileTextureSource(Texture2D texture2D, AutoTile.AutoTileMaskType maskType,
            Action<Sprite, Texture2D, uint, uint> maskChanged, Action editStopped) : base(ScrollViewMode
            .VerticalAndHorizontal)
        {
            m_TextureElement = new Image();
            Add(m_TextureElement);

            m_TextureElement.image = texture2D;
            m_TextureElement.style.width = texture2D.width;
            m_TextureElement.style.height = texture2D.height;

            m_EditStopped = editStopped;

            var assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture2D));
            m_ClickState = new AutoTileSpriteSource.ClickState();
            foreach (var assetAtPath in assetsAtPath)
            {
                var spriteAsset = assetAtPath as Sprite;
                if (spriteAsset == null)
                    continue;

                var spriteImage = new AutoTileSpriteSource(spriteAsset, texture2D, m_ClickState, maskType);
                spriteImage.maskChanged = maskChanged;
                m_TextureElement.Add(spriteImage);
                m_SpriteToElementMap.Add(spriteAsset, spriteImage);
            }

            RegisterCallback<PointerLeaveEvent>((evt) => StoppedClick());
            RegisterCallback<PointerUpEvent>((evt) => StoppedClick());
        }

        private void StoppedClick()
        {
            m_ClickState.isPointerDown = false;
            if (m_EditStopped != null)
                m_EditStopped();
        }

        public void ApplyAutoTileTemplate(AutoTileTemplate template, bool matchExact = false)
        {
            foreach (var item in m_SpriteToElementMap)
            {
                item.Value.InitialiseMask(0);
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
                        match = Mathf.Approximately(sprite.x / template.width,
                                    item.Key.rect.x / m_TextureElement.image.width)
                                && Mathf.Approximately(sprite.y / template.height,
                                    item.Key.rect.y / m_TextureElement.image.height);
                    }

                    if (match)
                    {
                        SetSpriteMask(item.Key, sprite.mask);
                        break;
                    }
                }
            }
        }

        public void ClearMaskForTextureSource()
        {
            foreach (var item in m_SpriteToElementMap)
            {
                item.Value.InitialiseMask(0);
                SetSpriteMask(item.Key, 0);
                break;
            }
        }

        public List<AutoTileTemplate.SpriteData> GetSpriteData()
        {
            var spriteData = new List<AutoTileTemplate.SpriteData>();
            foreach (var item in m_SpriteToElementMap)
            {
                if (item.Value.mask == 0)
                    continue;

                spriteData.Add(new AutoTileTemplate.SpriteData()
                {
                    x = item.Key.rect.x, y = item.Key.rect.y, mask = item.Value.mask
                });
            }

            return spriteData;
        }

        internal void InitialiseSpriteMask(Sprite sprite, uint mask)
        {
            if (m_SpriteToElementMap.TryGetValue(sprite, out var atss))
            {
                atss.InitialiseMask(mask);
            }
        }

        private void SetSpriteMask(Sprite sprite, uint mask)
        {
            if (m_SpriteToElementMap.TryGetValue(sprite, out var atss))
            {
                atss.SetMask(mask);
            }
        }

        internal void SetDuplicate(Sprite sprite, bool isDuplicate)
        {
            if (m_SpriteToElementMap.TryGetValue(sprite, out var atss))
            {
                atss.SetDuplicate(isDuplicate);
            }
        }

        public void ChangeScale(float newScale)
        {
            m_TextureElement.scaleMode = ScaleMode.StretchToFill;
            m_TextureElement.style.width = m_TextureElement.image.width * newScale;
            m_TextureElement.style.height = m_TextureElement.image.height * newScale;
            foreach (var item in m_SpriteToElementMap)
            {
                item.Value.ChangeScale(newScale);
            }
        }
    }
}
