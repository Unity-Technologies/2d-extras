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

        protected void InstantiatePrefabInCell(GridLayout grid, GameObject brushTarget, Vector3Int position, GameObject prefab)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (instance != null)
            {
                Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
                Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(position + m_Anchor));
            }
        }
    }
    
    public class BasePrefabBrushEditor :  GridBrushEditor
    {
        private SerializedProperty m_Anchor;
        protected SerializedObject m_SerializedObject;

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