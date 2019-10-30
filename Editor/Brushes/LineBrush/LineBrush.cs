using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush helps draw lines of Tiles onto a Tilemap.
    /// Use this as an example to modify brush painting behaviour to making painting quicker with less actions.
    /// </summary>
    [CustomGridBrush(true, false, false, "Line Brush")]
    [CreateAssetMenu(fileName = "New Line Brush", menuName = "Brushes/Line Brush")]
    public class LineBrush : GridBrush
    {
        /// <summary>
        /// Whether the Line Brush has started drawing a line.
        /// </summary>
        public bool lineStartActive;
        /// <summary>
        /// Ensures that there are orthogonal connections of Tiles from the start of the line to the end.
        /// </summary>
        public bool fillGaps;
        /// <summary>
        /// The current starting point of the line.
        /// </summary>
        public Vector3Int lineStart = Vector3Int.zero;

        /// <summary>
        /// Indicates whether the brush is currently
        /// moving something using the "Move selection with active brush" tool.
        /// </summary>
        public bool IsMoving { get; private set; }

        /// <summary>
        /// Paints tiles and GameObjects into a given position within the selected layers.
        /// The LineBrush overrides this to provide line painting functionality.
        /// The first paint action sets the starting point of the line.
        /// The next paint action sets the ending point of the line and paints Tile from start to end.
        /// </summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (lineStartActive)
            {
                Vector2Int startPos = new Vector2Int(lineStart.x, lineStart.y);
                Vector2Int endPos = new Vector2Int(position.x, position.y);
                if (startPos == endPos)
                    base.Paint(grid, brushTarget, position);    
                else
                {
                    foreach (var point in GetPointsOnLine(startPos, endPos, fillGaps))
                    {
                        Vector3Int paintPos = new Vector3Int(point.x, point.y, position.z);
                        base.Paint(grid, brushTarget, paintPos);
                    }
                }
                lineStartActive = false;
            }
            else if (IsMoving)
            {
                base.Paint(grid, brushTarget, position);
            }
            else
            {
                lineStart = position;
                lineStartActive = true;
            }
        }

        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            base.MoveStart(gridLayout, brushTarget, position);
            IsMoving = true;
        }

        public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            base.MoveEnd(gridLayout, brushTarget, position);
            IsMoving = false;
        }

        /// <summary>
        /// Added option to fill gaps for continuous lines.
        /// </summary>
        public static IEnumerable<Vector2Int> GetPointsOnLine(Vector2Int startPos, Vector2Int endPos, bool fillGaps)
        {
            var points = GetPointsOnLine(startPos, endPos);
            if (fillGaps)
            {
                var rise = endPos.y - startPos.y;
                var run = endPos.x - startPos.x;

                if (rise != 0 || run != 0)
                {
                    var extraStart = startPos;
                    var extraEnd = endPos;


                    if (Mathf.Abs(rise) >= Mathf.Abs(run))
                    {
                        // up
                        if (rise > 0)
                        {
                            extraStart.y += 1;
                            extraEnd.y += 1;
                        }
                        // down
                        else // rise < 0
                        {

                            extraStart.y -= 1;
                            extraEnd.y -= 1;
                        }
                    }
                    else // Mathf.Abs(rise) < Mathf.Abs(run)
                    {

                        // right
                        if (run > 0)
                        {
                            extraStart.x += 1;
                            extraEnd.x += 1;
                        }
                        // left
                        else // run < 0
                        {
                            extraStart.x -= 1;
                            extraEnd.x -= 1;
                        }
                    }

                    var extraPoints = GetPointsOnLine(extraStart, extraEnd);
                    extraPoints = extraPoints.Except(new[] { extraEnd });
                    points = points.Union(extraPoints);
                }

            }

            return points;
        }

        /// <summary>
        /// Gets an enumerable for all the cells directly between two points
        /// http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html
        /// </summary>
        /// <param name="p1">A starting point of a line</param>
        /// <param name="p2">An ending point of a line</param>
        /// <returns>Gets an enumerable for all the cells directly between two points</returns>
        public static IEnumerable<Vector2Int> GetPointsOnLine(Vector2Int p1, Vector2Int p2)
        {
            int x0 = p1.x;
            int y0 = p1.y;
            int x1 = p2.x;
            int y1 = p2.y;

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Vector2Int((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }
    }

    /// <summary>
    /// The Brush Editor for a Line Brush.
    /// </summary>
    [CustomEditor(typeof(LineBrush))]
    public class LineBrushEditor : GridBrushEditor
    {
        private LineBrush lineBrush { get { return target as LineBrush; } }
        private Tilemap lastTilemap;

        /// <summary>
        /// Callback for painting the GUI for the GridBrush in the Scene View.
        /// The CoordinateBrush Editor overrides this to draw the preview of the brush when drawing lines.
        /// </summary>
        /// <param name="gridLayout">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">Target of the GridBrushBase::ref::Tool operation. By default the currently selected GameObject.</param>
        /// <param name="position">Current selected location of the brush.</param>
        /// <param name="tool">Current GridBrushBase::ref::Tool selected.</param>
        /// <param name="executing">Whether brush is being used.</param>
        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
            if (lineBrush.lineStartActive)
            {
                Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap != null)
                    lastTilemap = tilemap;

                // Draw preview tiles for tilemap
                Vector2Int startPos = new Vector2Int(lineBrush.lineStart.x, lineBrush.lineStart.y);
                Vector2Int endPos = new Vector2Int(position.x, position.y);
                if (startPos == endPos)
                    PaintPreview(grid, brushTarget, position.min);
                else
                {
                    foreach (var point in LineBrush.GetPointsOnLine(startPos, endPos, lineBrush.fillGaps))
                    {
                        Vector3Int paintPos = new Vector3Int(point.x, point.y, position.z);
                        PaintPreview(grid, brushTarget, paintPos);
                    }
                }

                if (Event.current.type == EventType.Repaint)
                {
                    var min = lineBrush.lineStart;
                    var max = lineBrush.lineStart + position.size;

                    // Draws a box on the picked starting position
                    GL.PushMatrix();
                    GL.MultMatrix(GUI.matrix);
                    GL.Begin(GL.LINES);
                    Handles.color = Color.blue;
                    Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z));
                    Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z));
                    Handles.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z));
                    Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, min.y, min.z));
                    GL.End();
                    GL.PopMatrix();
                }
            }
        }

        /// <summary>
        /// Clears all line previews.
        /// </summary>
        public override void ClearPreview()
        {
            base.ClearPreview();
            if (lastTilemap != null)
            {
                lastTilemap.ClearAllEditorPreviewTiles();
                lastTilemap = null;
            }
        }
    }
}
