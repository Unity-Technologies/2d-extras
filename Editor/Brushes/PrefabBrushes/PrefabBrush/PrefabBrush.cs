using System.Linq;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush instances and places a containing prefab onto the targeted location and parents the instanced object to the paint target.
    /// </summary>
    [CustomGridBrush(false, true, false, "Prefab Brush")]
    public class PrefabBrush : BasePrefabBrush
    {
        #pragma warning disable 0649
        /// <summary>
        /// The selection of Prefab to paint from
        /// </summary>
        [SerializeField] GameObject m_Prefab;
        Quaternion m_Rotation = default;
        
        #pragma warning restore 0649

        /// <summary>
        /// If true, erases any GameObjects that are in a given position within the selected layers with Erasing.
        /// Otherwise, erases only GameObjects that are created from owned Prefab in a given position within the selected layers with Erasing.
        /// </summary>
        bool m_EraseAnyObjects;

        /// <summary>
        /// Rotates the brush in the given direction.
        /// </summary>
        /// <param name="direction">Direction to rotate by.</param>
        /// <param name="layout">Cell Layout for rotating.</param>
        public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        {
            var angle = layout == GridLayout.CellLayout.Hexagon ? 60f : 90f;
            m_Rotation = Quaternion.Euler(0f, 0f, direction == RotationDirection.Clockwise ? m_Rotation.eulerAngles.z + angle : m_Rotation.eulerAngles.z - angle);
        }

        /// <summary>
        /// Paints GameObject from containing Prefab into a given position within the selected layers.
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
                base.InstantiatePrefabInCell(grid, brushTarget, position, m_Prefab, m_Rotation);
            }
        }

        /// <summary>
        /// Paints the PrefabBrush instance's prefab into all positions specified by the box fill tool.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the box fill operation. By default the currently selected GameObject.</param>
        /// <param name="bounds">The cooridnate boundries to fill.</param>
        public override void BoxFill(GridLayout grid, GameObject brushTarget, BoundsInt bounds)
        {
            foreach(Vector3Int tilePosition in bounds.allPositionsWithin)
                Paint(grid, brushTarget, tilePosition);
        }

        public override void BoxErase(GridLayout grid, GameObject brushTarget, BoundsInt bounds)
        {
            foreach (Vector3Int tilePosition in bounds.allPositionsWithin)
                Erase(grid, brushTarget, tilePosition);
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
        /// Pick prefab from selected Tilemap, given the coordinates of the cells.
        /// </summary>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            if (brushTarget == null)
            {
                return;
            }
            foreach (var objectInCell in GetObjectsInCell(gridLayout, brushTarget.transform, position.position))
            {
                if (objectInCell)
                {
                    m_Prefab = PrefabUtility.GetCorrespondingObjectFromSource(objectInCell);
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

            /// <summary>
            /// OnEnable for the PrefabBrushEditor
            /// </summary>
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
                const string eraseAnyObjectsTooltip =
                    "If true, erases any GameObjects that are in a given position " +
                    "within the selected layers with Erasing. " +
                    "Otherwise, erases only GameObjects that are created " +
                    "from owned Prefab in a given position within the selected layers with Erasing.";

                base.OnPaintInspectorGUI();

                m_SerializedObject.UpdateIfRequiredOrScript();
                EditorGUILayout.PropertyField(m_Prefab, true);
                prefabBrush.m_EraseAnyObjects = EditorGUILayout.Toggle(
                    new GUIContent("Erase Any Objects", eraseAnyObjectsTooltip),
                    prefabBrush.m_EraseAnyObjects);

                m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
