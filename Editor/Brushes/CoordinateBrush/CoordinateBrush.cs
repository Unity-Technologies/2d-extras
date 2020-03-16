using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush displays the cell coordinates it is targeting in the SceneView.
    /// Use this as an example to create brushes which have extra visualization features when painting onto a Tilemap.
    /// </summary>
    [CustomGridBrush(true, false, false, "Coordinate Brush")]
    public class CoordinateBrush : GridBrush 
    {
    }

    /// <summary>
    /// The Brush Editor for a Coordinate Brush.
    /// </summary>
    [CustomEditor(typeof(CoordinateBrush))]
    public class CoordinateBrushEditor : GridBrushEditor
    {
        /// <summary>
        /// Callback for painting the GUI for the GridBrush in the Scene View.
        /// The CoordinateBrush Editor overrides this to draw the current coordinates of the brush.
        /// </summary>
        /// <param name="gridLayout">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">Target of the GridBrushBase::ref::Tool operation. By default the currently selected GameObject.</param>
        /// <param name="position">Current selected location of the brush.</param>
        /// <param name="tool">Current GridBrushBase::ref::Tool selected.</param>
        /// <param name="executing">Whether brush is being used.</param>
        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);

            var labelText = "Pos: " + position.position;
            if (position.size.x > 1 || position.size.y > 1) {
                labelText += " Size: " + position.size;
            }

            Handles.Label(grid.CellToWorld(position.position), labelText);
        }
    }
}
