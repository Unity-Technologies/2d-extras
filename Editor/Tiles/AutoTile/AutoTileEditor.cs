using System;
using UnityEditor.UIElements;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    [CustomEditor(typeof(AutoTile))]
    public class AutoTileEditor : Editor
    {
        private AutoTile autoTile => target as AutoTile;

        public override VisualElement CreateInspectorGUI()
        {
            var autoTileEditorElement = new AutoTileEditorElement();
            autoTileEditorElement.Bind(serializedObject);
            autoTileEditorElement.autoTile = autoTile;
            return autoTileEditorElement;
        }
    }
}