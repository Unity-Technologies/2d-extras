using System.Linq;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush instances and places a containing prefab onto the targeted location and parents the instanced object to the paint target.
    /// </summary>
    [CreateAssetMenu(fileName = "New Prefab Brush", menuName = "2D Extras/Brushes/Prefab Brush", order = 359)]
    [CustomGridBrush(false, true, false, "Prefab Brush")]
    public class PrefabBrush : BasePrefabBrush
    {
        #pragma warning disable 0649
        /// <summary>
        /// The selection of Prefab to paint from
        /// </summary>
        [SerializeField] GameObject m_Prefab;
        #pragma warning restore 0649

        /// <summary>
        /// If true, erases any GameObjects that are in a given position within the selected layers with Erasing.
        /// Otherwise, erases only GameObjects that are created from owned Prefab in a given position within the selected layers with Erasing.
        /// </summary>
        bool m_EraseAnyObjects;

        /// <summary>
        /// Paints GameObject from containg Prefab into a given position within the selected layers.
        /// The PrefabBrush overrides this to provide Prefab painting functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31 || brushTarget == null)
            {
                return;
            }

            var objectsInCell = GetObjectsInCell(grid, brushTarget.transform, position);
            var existPrefabObjectInCell = objectsInCell.Any(objectInCell => PrefabUtility.GetCorrespondingObjectFromSource(objectInCell) == m_Prefab);

            if (!existPrefabObjectInCell)
            {
                base.InstantiatePrefabInCell(grid, brushTarget, position, m_Prefab);
            }
        }

        /// <summary>
        /// If "Erase Any Objects" is true, erases any GameObjects that are in a given position within the selected layers.
        /// If "Erase Any Objects" is false, erases only GameObjects that are created from owned Prefab in a given position within the selected layers.
        /// The PrefabBrush overrides this to provide Prefab erasing functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to erase data from.</param>
        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget.layer == 31 || brushTarget.transform == null)
            {
                return;
            }

            foreach (var objectInCell in GetObjectsInCell(grid, brushTarget.transform, position))
            {
                if (m_EraseAnyObjects || PrefabUtility.GetCorrespondingObjectFromSource(objectInCell) == m_Prefab)
                {
                    Undo.DestroyObjectImmediate(objectInCell);
                }
            }
        }

        /// <summary>
        /// The Brush Editor for a Prefab Brush.
        /// </summary>
        [CustomEditor(typeof(PrefabBrush))]
        public class PrefabBrushEditor : BasePrefabBrushEditor
        {
            private PrefabBrush prefabBrush => target as PrefabBrush;
            private SerializedProperty m_Prefab;

            protected override void OnEnable()
            {
                base.OnEnable();
                m_Prefab = m_SerializedObject.FindProperty(nameof(m_Prefab));
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
                prefabBrush.m_EraseAnyObjects = EditorGUILayout.Toggle("Erase Any Objects", prefabBrush.m_EraseAnyObjects);
                m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
