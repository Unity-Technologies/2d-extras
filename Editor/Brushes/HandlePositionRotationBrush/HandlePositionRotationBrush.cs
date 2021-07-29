using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Tilemaps;
#endif

namespace UnityEditor.Tilemaps
{
    [CustomGridBrush(true, false, true, "Handle Position Rotation Brush")]
    public class HandlePositionRotationBrush : GridBrush
    {
        public bool applyPosition;
        public bool applyRotation;
        public bool applyColor;

        private Vector3Int[] m_SetPositions = new Vector3Int[0];
        private TileBase[] m_SetTiles = new TileBase[0];

        private int m_CachedStep = -1;
        private Vector3Int m_OriginalSize;
        private Vector3Int m_OriginalPivot;
        private BrushCell[] m_OriginalCells = new BrushCell[0];
        private GridLayout.CellLayout m_PickedLayout = GridLayout.CellLayout.Rectangle;
        private int m_RotationStep = 0;

        /// <summary>Box fills tiles and GameObjects into given bounds within the selected layers.</summary>
        /// <param name="gridLayout">Grid to box fill data to.</param>
        /// <param name="brushTarget">Target of the box fill operation. By default the currently selected GameObject.</param>
        /// <param name="position">The bounds to box fill data into.</param>
        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null)
                return;

            var map = brushTarget.GetComponent<Tilemap>();
            if (map == null)
                return;

            CacheCellsFromRotationStep();

            var setPositions = m_SetPositions;
            var setTiles = m_SetTiles;
            var positionSize = position.size.x * position.size.y * position.size.z; 
            if (positionSize != setPositions.Length)
            {
                setPositions = new Vector3Int[positionSize];
                setTiles = new TileBase[positionSize];
            }
            
            int i = 0;
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell cell = cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (cell.tile == null)
                    continue;

                setPositions[i] = location;
                setTiles[i] = cell.tile;
                i++;
            }
            map.SetTiles(setPositions, setTiles);

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell cell = cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (cell.tile == null)
                    continue;

                if (applyPosition || applyRotation)
                {
                    var cellTransform = map.GetTransformMatrix(location);
                    var originalPosition = cellTransform.GetColumn(3);
                    if (applyPosition)
                        cellTransform.SetColumn(3, cell.matrix.GetColumn(3));
                    if (applyRotation && m_RotationStep > 0)
                    {
                        cellTransform *= Matrix4x4.TRS( Vector3.zero
                            , applyRotation ? Quaternion.Euler(0, 0, m_RotationStep * 90) : Quaternion.identity
                            ,Vector3.one); // Ignore scale for now
                        if (!applyPosition)
                            cellTransform.SetColumn(3, originalPosition);
                    }
                    map.SetTransformMatrix(location, cellTransform);
                }
                if (applyColor)
                    map.SetColor(location, cell.color);
            }
        }

        /// <summary>Rotates the brush by 90 degrees in the given direction.</summary>
        /// <param name="direction">Direction to rotate by.</param>
        /// <param name="layout">Cell Layout for rotating.</param>
        public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        {
            if (m_PickedLayout == GridLayout.CellLayout.Hexagon)
            {
                // Only handle rotation for 6 sides
                if (m_PickedLayout == layout)
                {
                    m_RotationStep = (6 + m_RotationStep + (direction == RotationDirection.Clockwise ? 1 : -1)) % 6;
                }
            }
            else
            {
                // Only handle rotation for 4 sides
                if (layout != GridLayout.CellLayout.Hexagon)
                {
                    m_RotationStep = (4 + m_RotationStep + (direction == RotationDirection.Clockwise ? 1 : -1)) % 4;
                }
            }
        }

        internal void CacheCellsFromRotationStep()
        {
            if (m_CachedStep == m_RotationStep)
                return;

            size = m_OriginalSize;
            pivot = m_OriginalPivot;
            CopyCells(m_OriginalCells, cells);

            for (int i = 0; i < m_RotationStep; ++i)
                base.Rotate(RotationDirection.Clockwise, m_PickedLayout);

            m_CachedStep = m_RotationStep;
        }

        /// <summary>Picks tiles from selected Tilemaps and child GameObjects, given the coordinates of the cells.</summary>
        /// <param name="gridLayout">Grid to pick data from.</param>
        /// <param name="brushTarget">Target of the picking operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cells to paint data from.</param>
        /// <param name="pickStart">Pivot of the picking brush.</param>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            base.Pick(gridLayout, brushTarget, position, pickStart);

            m_OriginalSize = size;
            m_OriginalPivot = pivot;
            m_OriginalCells = new BrushCell[cellCount];
            CopyCells(cells, m_OriginalCells);

            int setTileCount = 0;
            foreach (var cell in cells)
            {
                if (cell.tile != null)
                    setTileCount++;
            }

            // Reset internal position/rotation separation
            m_PickedLayout = gridLayout.cellLayout;
            m_RotationStep = 0;
            InvalidateCache();

            Array.Resize(ref m_SetPositions, setTileCount);
            Array.Resize(ref m_SetTiles, setTileCount);
        }

        private void CopyCells(BrushCell[] fromCells, BrushCell[] toCells)
        {
            int min = Math.Min(fromCells.Length, toCells.Length);
            for (int i = 0; i < min; ++i)
            {
                var fromCell = fromCells[i];
                var toCell = toCells[i];
                if (toCell == null)
                {
                    toCell = new BrushCell();
                    toCells[i] = toCell;
                }
                toCell.tile = fromCell.tile;
                toCell.matrix = fromCell.matrix;
                toCell.color = fromCell.color;
            }
        }

        internal void InvalidateCache()
        {
            m_CachedStep = -1;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(HandlePositionRotationBrush))]
    class HandlePositionRotationBrushEditor : GridBrushEditor
    {
        private new HandlePositionRotationBrush brush => target as HandlePositionRotationBrush;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
                brush.InvalidateCache();
        }

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget
            , BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            brush.CacheCellsFromRotationStep();
            base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
        }
    }
#endif
}
