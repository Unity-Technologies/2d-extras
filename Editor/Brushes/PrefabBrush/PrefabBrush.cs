using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush instances and places a selected prefab onto the targeted location and parents the instanced object to the paint target.
    /// </summary>
    [CreateAssetMenu(fileName = "Prefab brush", menuName = "Brushes/Prefab brush")]
    [CustomGridBrush(false, true, false, "Prefab Brush")]
    public class PrefabBrush : GridBrush
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
        /// Factor for distribution of choice of Prefabs to paint
        /// </summary>
        public float m_PerlinScale = 0.5f;
        /// <summary>
        /// Anchor Point of the Instantiated Prefab in the cell when painting
        /// </summary>
        public Vector3 m_Anchor = new Vector3(0.5f, 0.5f, 0.5f);
        /// <summary>
        /// Paints Prefabs into a given position within the selected layers.
        /// The PrefabBrush overrides this to provide Prefab painting functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31 || brushTarget == null)
                return;

            GameObject prefab = m_Prefab;
            var tileObject = GetObjectInCell(grid, brushTarget.transform, position);
            if (tileObject == null || tileObject.name != m_Prefab.name)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
                Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(position + m_Anchor));
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
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Transform erased = GetObjectInCell(grid, brushTarget.transform, position);
            if (erased == null)
            {
                return;
            }
            if (m_ForceDelete)
            {
                Undo.DestroyObjectImmediate(erased.gameObject);
            }
            else if (erased.gameObject.name == m_Prefab.name)
            {
                Undo.DestroyObjectImmediate(erased.gameObject);
            }
        }

        private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (position == grid.WorldToCell(child.position))
                    return child;
            }
            return null;
        }
    }

    /// <summary>
    /// The Brush Editor for a Prefab Brush.
    /// </summary>
    [CustomEditor(typeof(PrefabBrush))]
    public class PrefabBrushEditor : GridBrushEditor
    {
        private SerializedProperty m_Prefab;
        private SerializedProperty m_Anchor;
        private SerializedProperty m_ForceDelete;
        private SerializedObject m_SerializedObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_SerializedObject = new SerializedObject(target);
            m_Prefab = m_SerializedObject.FindProperty("m_Prefab");
            m_Anchor = m_SerializedObject.FindProperty("m_Anchor");
            m_ForceDelete = m_SerializedObject.FindProperty("m_ForceDelete");
        }

        /// <summary>
        /// Callback for painting the inspector GUI for the PrefabBrush in the Tile Palette.
        /// The PrefabBrush Editor overrides this to have a custom inspector for this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            m_SerializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(m_Prefab, true);
            EditorGUILayout.PropertyField(m_ForceDelete, true);
            EditorGUILayout.PropertyField(m_Anchor);
            m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
