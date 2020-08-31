using System.Linq;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush instances and places a randomly selected Prefabs onto the targeted location and parents the instanced object to the paint target. Use this as an example to quickly place an assorted type of GameObjects onto structured locations.
    /// </summary>
    [CustomGridBrush(false, true, false, "Prefab Random Brush")]
    public class PrefabRandomBrush : BasePrefabBrush
    {
        private const float k_PerlinOffset = 100000f;

        #pragma warning disable 0649
        /// <summary>
        /// The selection of Prefabs to paint from
        /// </summary>
        [SerializeField] GameObject[] m_Prefabs;

        /// <summary>
        /// Factor for distribution of choice of Prefabs to paint
        /// </summary>
        [SerializeField] float m_PerlinScale = 0.5f;
        #pragma warning restore 0649

        /// <summary>
        /// If true, erases any GameObjects that are in a given position within the selected layers with Erasing.
        /// Otherwise, erases only GameObjects that are created from owned Prefab in a given position within the selected layers with Erasing.
        /// </summary>
        bool m_EraseAnyObjects;

        /// <summary>
        /// Paints GameObject from containg Prefabs with randomly into a given position within the selected layers.
        /// The PrefabRandomBrush overrides this to provide Prefab painting functionality.
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
                base.InstantiatePrefabInCell(grid, brushTarget, position, prefab);
            }
        }

        /// <summary>
        /// If "Erase Any Objects" is true, erases any GameObjects that are in a given position within the selected layers.
        /// If "Erase Any Objects" is false, erases only GameObjects that are created from owned Prefab in a given position within the selected layers.
        /// The PrefabRandomBrush overrides this to provide Prefab erasing functionality.
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
                    if (objectInCell != null && (m_EraseAnyObjects || PrefabUtility.GetCorrespondingObjectFromSource(objectInCell) == prefab))
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

        /// <summary>
        /// The Brush Editor for a Prefab Brush.
        /// </summary>
        [CustomEditor(typeof(PrefabRandomBrush))]
        public class PrefabRandomBrushEditor : BasePrefabBrushEditor
        {
            private PrefabRandomBrush prefabRandomBrush => target as PrefabRandomBrush;

            private SerializedProperty m_Prefabs;

            /// <summary>
            /// OnEnable for the PrefabRandomBrushEditor
            /// </summary>
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
                prefabRandomBrush.m_PerlinScale = EditorGUILayout.Slider("Perlin Scale", prefabRandomBrush.m_PerlinScale, 0.001f, 0.999f);
                EditorGUILayout.PropertyField(m_Prefabs, true);
                prefabRandomBrush.m_EraseAnyObjects = EditorGUILayout.Toggle("Erase Any Objects", prefabRandomBrush.m_EraseAnyObjects);
                m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}