using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Advanced tint brush for interpolated tint color per-cell. Requires the use of custom shader (see TintedTilemap.shader) and helper component TileTextureGenerator.
    /// </summary>
    [CustomGridBrush(false, false, false, "Tint Brush (Smooth)")]
    public class TintBrushSmooth : GridBrushBase
    {
        /// <summary>
        /// Factor to blend the Color of Tile with its color and this Brush's color
        /// </summary>
        public float m_Blend = 1f;
        /// <summary>
        /// Color of the Tile to tint
        /// </summary>
        public Color m_Color = Color.white;

        /// <summary>
        /// Tints tiles into a given position within the selected layers.
        /// The TintBrushSmooth overrides this to set the color of the Grid position to tint it.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            TintTextureGenerator generator = GetGenerator(grid);
            if (generator != null)
            {
                var oldColor = generator.GetColor(grid as Grid, position);
                var blendColor = oldColor * (1 - m_Blend) + m_Color * m_Blend;
                generator.SetColor(grid as Grid, position, blendColor);
            }
        }

        /// <summary>
        /// Resets the color of the tiles in a given position within the selected layers to White.
        /// The TintBrushSmooth overrides this to set the color of the Grid position to White.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to erase data from.</param>
        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            TintTextureGenerator generator = GetGenerator(grid);
            if (generator != null)
            {
                generator.SetColor(grid as Grid, position, Color.white);
            }
        }

        /// <summary>
        /// Picks the tint color given the coordinates of the cells.
        /// The TintBrushSmoot overrides this to provide color picking functionality.
        /// </summary>
        /// <param name="grid">Grid to pick data from.</param>
        /// <param name="brushTarget">Target of the picking operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cells to paint data from.</param>
        /// <param name="pivot">Pivot of the picking brush.</param>
        public override void Pick(GridLayout grid, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
        {
            TintTextureGenerator generator = GetGenerator(grid);
            if (generator != null)
            {
                m_Color = generator.GetColor(grid as Grid, position.min);
            }
        }

        private TintTextureGenerator GetGenerator(GridLayout grid)
        {
            TintTextureGenerator generator = FindObjectOfType<TintTextureGenerator>();
            if (generator == null)
            {
                if (grid != null)
                {
                    generator = grid.gameObject.AddComponent<TintTextureGenerator>();
                }
            }
            return generator;
        }
    }

    /// <summary>
    /// The Brush Editor for a Tint Brush Smooth.
    /// </summary>
    [CustomEditor(typeof(TintBrushSmooth))]
    public class TintBrushSmoothEditor : GridBrushEditorBase
    {
        /// <summary>
        /// The TintBrushSmooth for this Editor
        /// </summary>
        public TintBrushSmooth brush { get { return target as TintBrushSmooth; } }

        /// <summary>Returns all valid targets that the brush can edit.</summary>
        /// <remarks>Valid targets for the TintBrushSmooth are any GameObjects with a Tilemap component.</remarks>
        public override GameObject[] validTargets
        {
            get
            {
                return GameObject.FindObjectsOfType<Tilemap>().Select(x => x.gameObject).ToArray();
            }
        }

        /// <summary>
        /// Callback for painting the inspector GUI for the TintBrushSmooth in the Tile Palette.
        /// The TintBrushSmooth Editor overrides this to have a custom inspector for this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            brush.m_Color = EditorGUILayout.ColorField("Color", brush.m_Color);
            brush.m_Blend = EditorGUILayout.Slider("Blend", brush.m_Blend, 0f, 1f);
            GUILayout.Label("Note: Tilemap needs to use TintedTilemap.shader!");
        }
    }
}
