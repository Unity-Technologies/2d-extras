using UnityEditor.UIElements;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Editor for AutoTile.
    /// </summary>
    [CustomEditor(typeof(AutoTile))]
    public class AutoTileEditor : Editor
    {
        private AutoTile autoTile => target as AutoTile;

        /// <summary>
        /// Creates a VisualElement for AutoTile Editor.
        /// </summary>
        /// <returns>A VisualElement for AutoTile Editor.</returns>
        public override VisualElement CreateInspectorGUI()
        {
            var autoTileEditorElement = new AutoTileEditorElement();
            autoTileEditorElement.Bind(serializedObject);
            autoTileEditorElement.autoTile = autoTile;
            return autoTileEditorElement;
        }
    }
}