using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush instances and places a selected prefab onto the targeted location and parents the instanced object to the paint target.
    /// </summary>
    [CreateAssetMenu(fileName = "Prefab brush", menuName = "Brushes/Prefab brush")]
    [CustomGridBrush(false, true, false, "Prefab Brush")]
    public class PrefabBrush : BasePrefabBrush
    {
        /// <summary>
        /// The selection of Prefab to paint from
        /// </summary>
        public GameObject m_Prefab;
        /// <summary>
        /// Use to remove all prefabs in the cell or just the one that is currently selected in m_Prefab
        /// </summary>
        public bool m_ForceDelete;
        /// <summary>
        /// Paints Prefabs into a given position within the selected layers.
        /// The PrefabBrush overrides this to provide Prefab painting functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            
           
            Prefab = m_Prefab;
            var tileObject = GetObjectInCell(grid, brushTarget.transform, position);
            if (tileObject == null || tileObject.name != m_Prefab.name)
            {
                base.Paint(grid, brushTarget, position);
            }
        }

        /// <summary>
        /// Erases all Prefabs in a given position within the selected layers if ForceDelete is true.
        /// Erase only selected Prefabs in a given position within the selected layers if ForceDelete is false.
        /// The PrefabBrush overrides this to provide Prefab erasing functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to erase data from.</param>
        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var erased = GetObjectInCell(grid, brushTarget.transform, position);
            if (erased == null)
            {
                return;
            }
            if (m_ForceDelete || (!m_ForceDelete && erased.gameObject.name == m_Prefab.name))
            {
                base.Erase(grid, brushTarget, position);
            }
        }
    }

    /// <summary>
    /// The Brush Editor for a Prefab Brush.
    /// </summary>
    [CustomEditor(typeof(PrefabBrush))]
    public class PrefabBrushEditor : BasePrefabBrushEditor
    {
        private SerializedProperty m_Prefab;
        private SerializedProperty m_ForceDelete;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Prefab = m_SerializedObject.FindProperty("m_Prefab");
            m_ForceDelete = m_SerializedObject.FindProperty("m_ForceDelete");
        }

        /// <summary>
        /// Callback for painting the inspector GUI for the PrefabBrush in the Tile Palette.
        /// The PrefabBrush Editor overrides this to have a custom inspector for this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            base.OnPaintInspectorGUI();
            m_SerializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(m_Prefab, true);
            EditorGUILayout.PropertyField(m_ForceDelete, true);
            m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
