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
        /// GameObject to instantiating
        /// </summary>
        protected GameObject Prefab = null;
        
        protected static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            var childCount = parent.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                if (position == grid.WorldToCell(child.position))
                {
                    return child;
                }
            }
            return null;
        }

        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget.layer == 31)
            {
                return;
            }
                
            var erased = GetObjectInCell(grid, brushTarget.transform, position);
            if (erased != null)
            {
                Undo.DestroyObjectImmediate(erased.gameObject);
            }
        }

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31 || brushTarget == null)
                return;
            
            var tileObject = GetObjectInCell(grid, brushTarget.transform, position);
            if (tileObject)
            {
                return;
            }
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);
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