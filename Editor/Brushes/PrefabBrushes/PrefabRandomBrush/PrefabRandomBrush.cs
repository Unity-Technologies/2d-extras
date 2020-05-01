using System.Linq;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush instances and places a randomly selected Prefabs onto the targeted location and parents the instanced object to the paint target. Use this as an example to quickly place an assorted type of GameObjects onto structured locations.
    /// </summary>
    [CreateAssetMenu(fileName = "New Prefab Random Brush", menuName = "2D Extras/Brushes/Prefab Random Brush", order = 359)]
    [CustomGridBrush(false, true, false, "Prefab Random Brush")]
    public class PrefabRandomBrush : BasePrefabBrush
    {
        private const float k_PerlinOffset = 100000f;

        /// <summary>
        /// The selection of Prefabs to paint from
        /// </summary>
        public GameObject[] m_Prefabs;

        /// <summary>
        /// Factor for distribution of choice of Prefabs to paint
        /// </summary>
        public float m_PerlinScale = 0.5f;

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
            {
                return;
            }

            var objectsInCell = GetObjectsInCell(grid, brushTarget.transform, position);
            var existPrefabObjectInCell = objectsInCell.Any(objectInCell =>
            {
                return m_Prefabs.Any(prefab => PrefabUtility.GetCorrespondingObjectFromSource(objectInCell) == prefab);
            });

            if (!existPrefabObjectInCell)
            {
                var index = Mathf.Clamp(Mathf.FloorToInt(GetPerlinValue(position, m_PerlinScale, k_PerlinOffset) * m_Prefabs.Length), 0, m_Prefabs.Length - 1);
                var prefab = m_Prefabs[index];
                base.InstantiatePrefabOnGrid(grid, brushTarget, position, prefab);
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
            if (brushTarget.layer == 31 || brushTarget.transform == null)
            {
                return;
            }

            foreach (var objectInCell in GetObjectsInCell(grid, brushTarget.transform, position))
            {
                foreach (var prefab in m_Prefabs)
                {
                    if (PrefabUtility.GetCorrespondingObjectFromSource(objectInCell) == prefab)
                    {
                        Undo.DestroyObjectImmediate(objectInCell);
                    }
                }
            }
        }

        private static float GetPerlinValue(Vector3Int position, float scale, float offset)
        {
            return Mathf.PerlinNoise((position.x + offset)*scale, (position.y + offset)*scale);
        }
    }

    /// <summary>
    /// The Brush Editor for a Prefab Brush.
    /// </summary>
    [CustomEditor(typeof(PrefabRandomBrush))]
    public class PrefabRandomBrushEditor : BasePrefabBrushEditor
    {
        private PrefabRandomBrush prefabBrush { get { return target as PrefabRandomBrush; } }

        private SerializedProperty m_Prefabs;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            m_Prefabs = m_SerializedObject.FindProperty("m_Prefabs");
        }

        /// <summary>
        /// Callback for painting the inspector GUI for the PrefabBrush in the Tile Palette.
        /// The PrefabBrush Editor overrides this to have a custom inspector for this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            base.OnPaintInspectorGUI();
            m_SerializedObject.UpdateIfRequiredOrScript();
            prefabBrush.m_PerlinScale = EditorGUILayout.Slider("Perlin Scale", prefabBrush.m_PerlinScale, 0.001f, 0.999f);
            EditorGUILayout.PropertyField(m_Prefabs, true);
            m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}