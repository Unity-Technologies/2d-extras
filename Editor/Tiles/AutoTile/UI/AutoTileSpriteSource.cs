using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    internal class AutoTileSpriteSource : Image
    {
        public class ClickState
        {
            public bool isPointerDown;
            public bool toggleState;
        }
        
        private readonly Sprite m_Sprite;
        private readonly Texture2D m_SourceTexture;
        private readonly ClickState m_ClickState;
        private readonly int m_Range;

        public uint mask;
        public Action<Sprite, Texture2D, uint, uint> maskChanged;
        
        public AutoTileSpriteSource(Sprite spriteAsset
            , Texture2D sourceTexture
            , ClickState clickState
            , AutoTile.AutoTileMaskType maskType) : base()
        {
            m_Sprite = spriteAsset;
            m_SourceTexture = sourceTexture;
            m_ClickState = clickState;

            switch (maskType)
            {
                case AutoTile.AutoTileMaskType.Mask_2x2:
                    m_Range = 2;
                    break;
                default:
                    m_Range = 3;
                    break;
            }
            AddToClassList("ImageBase");
            
            style.position = Position.Absolute;
            style.flexShrink = 0;
            ChangeScale(1.0f);

            var index = 0;
            for (var y = 0; y < m_Range; y++)
            {
                var horizontal = new VisualElement();
                horizontal.style.flexDirection = FlexDirection.Row;
                horizontal.style.flexGrow = 1;
                for (var x = 0; x < m_Range; x++)
                {
                    var region = new VisualElement();
                    region.name = "Region";
                    region.AddToClassList("RegionHover");
                    region.userData = index++;
                    region.RegisterCallback<PointerEnterEvent>((evt) => PointerEnterEvent(evt, region) );
                    region.RegisterCallback<PointerLeaveEvent>((evt) => PointerLeaveEvent(region));
                    region.RegisterCallback<PointerDownEvent>((evt) => PointerDownEvent(region));
                    horizontal.Add(region);
                }   
                hierarchy.Insert(0, horizontal);
            }
        }

        public void ChangeScale(float newScale)
        {
            style.left = newScale * (m_Sprite.rect.x);
            style.top = newScale * (m_SourceTexture.height - m_Sprite.rect.height - m_Sprite.rect.y);
            style.width = newScale * (m_Sprite.rect.width);
            style.height = newScale * (m_Sprite.rect.height);
        }
        
        private void PointerEnterEvent(PointerEnterEvent evt, VisualElement element)
        {
            element.AddToClassList("PointerHover");
            if (m_ClickState.isPointerDown)
            {
                SetElement(element, m_ClickState.toggleState);
            }
        }

        private void PointerLeaveEvent(VisualElement element)
        {
            element.RemoveFromClassList("PointerHover");
        }
        
        private void PointerDownEvent(VisualElement element)
        {
            ToggleElement(element);
            m_ClickState.toggleState = element.ClassListContains("Clicked");
            m_ClickState.isPointerDown = true;
        }

        private void ToggleElement(VisualElement element)
        {
            var index = (int) element.userData;
            element.ToggleInClassList("Clicked");
            var clicked = element.ClassListContains("Clicked");
            UpdateMaskFromBit(index, clicked);
        }
        
        private void SetElement(VisualElement element, bool toggleState)
        {
            var index = (int) element.userData;
            element.EnableInClassList("Clicked", toggleState);
            UpdateMaskFromBit(index, toggleState);
        }

        private void UpdateMaskFromBit(int bit, bool state)
        {
            var oldMask = mask;
            var maskIndex = (uint) 1 << bit;
            if (state)
                mask |= maskIndex;
            else
                mask &= ~maskIndex;
            if (maskChanged != null)
                maskChanged.Invoke(m_Sprite, m_SourceTexture, oldMask, mask);
        }

        internal void InitialiseMask(uint newMask)
        {
            mask = newMask;
            foreach (var region in Children())
            {
                foreach (var visualElement in region.Children())
                {
                    var index = (int)visualElement.userData;
                    var toggled = ((mask & (uint)(1 << index)) > 0);
                    visualElement.EnableInClassList("Clicked", toggled);    
                }
            }
        }
        
        public void SetMask(uint newMask)
        {
            var oldMask = mask;
            mask = newMask;
            foreach (var region in Children())
            {
                foreach (var visualElement in region.Children())
                {
                    var index = (int)visualElement.userData;
                    var toggled = ((mask & (uint)(1 << index)) > 0);
                    visualElement.EnableInClassList("Clicked", toggled);    
                }
            }
            if (maskChanged != null)
                maskChanged.Invoke(m_Sprite, m_SourceTexture, oldMask, mask);
        }

        public void SetDuplicate(bool isDuplicate)
        {
            EnableInClassList("IsDuplicate", isDuplicate);
        }
    }
}