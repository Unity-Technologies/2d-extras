using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    internal class AutoTileTextureImporterElement : VisualElement
    {
        public Action onRevert;
        public Action onApply;

        public AutoTileTextureImporterElement()
        {
            var textureField = new PropertyField
            {
                bindingPath = "m_Texture"
            };
            Add(textureField);

            var applyRevertHe = new VisualElement();
            applyRevertHe.style.flexDirection = FlexDirection.RowReverse;

            var applyButton = new Button(OnApply);
            applyButton.text = "Apply";

            var revertButton = new Button(OnRevert);
            revertButton.text = "Revert";

            applyRevertHe.Add(applyButton);
            applyRevertHe.Add(revertButton);
            Add(applyRevertHe);
        }

        private void OnApply()
        {
            if (onApply != null)
                onApply();
        }

        private void OnRevert()
        {
            if (onRevert != null)
                onRevert();
        }
    }
}