using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Tilemaps
{ 
    /// <summary>
    /// This base class for PrefabBrushes that contains common functionality
    /// </summary>
    public class BasePrefabBrush : GridBrush
    {   /// <summary>
        /// Anchor Point of the Instantiated Prefab in the cell when painting
        /// </summary>
        public Vector3 m_Anchor = new Vector3(0.5f, 0.5f, 0.5f);

        /// <summary>
        /// Gets all children of the parent Transform which are within the given Grid's cell position.
        /// </summary>
        /// <param name="grid">Grid to determine cell position.</param>
        /// <param name="parent">Parent transform to get child Objects from.</param>
        /// <param name="position">Cell position to get Objects from.</param>
        /// <returns>A list of GameObjects within the given Grid's cell position.</returns>
        protected List<GameObject> GetObjectsInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            var results = new List<GameObject>();
            var childCount = parent.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                if (position == grid.WorldToCell(child.position))
                {
                    results.Add(child.gameObject);
                }
            }
            return results;
        }

        /// <summary>
        /// Instantiates a Prefab into the given Grid's cell position parented to the brush target.
        /// </summary>
        /// <param name="grid">Grid to determine cell position.</param>
        /// <param name="brushTarget">Target to instantiate child to.</param>
        /// <param name="position">Cell position to instantiate to.</param>
        /// <param name="prefab">Prefab to instantiate.</param>
        protected void InstantiatePrefabInCell(GridLayout grid, GameObject brushTarget, Vector3Int position, GameObject prefab, Quaternion rotation = default)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (instance != null)
            {
                Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
                Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(position + m_Anchor));
                instance.transform.rotation = rotation;
            }
        }
    }
    
    /// <summary>
    /// The Base Brush Editor for a Prefab Brush.
    /// </summary>
    public class BasePrefabBrushEditor :  GridBrushEditor
    {
        private SerializedProperty m_Anchor;

        /// <summary>
        /// SerializedObject representation of the target Prefab Brush
        /// </summary>
        protected SerializedObject m_SerializedObject;

        /// <summary>
        /// OnEnable for the BasePrefabBrushEditor
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            m_SerializedObject = new SerializedObject(target);
            m_Anchor = m_SerializedObject.FindProperty("m_Anchor");
        }

        /// <summary>
        /// Callback for painting the inspector GUI for the PrefabBrush in the Tile Palette.
        /// The PrefabBrush Editor overrides this to have a custom inspector for this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            m_SerializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(m_Anchor);
            m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}