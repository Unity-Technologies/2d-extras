using System;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush instances, places and manipulates GameObjects onto the scene.
    /// Use this as an example to create brushes which targets objects other than tiles for manipulation.
    /// </summary>
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest/index.html?subfolder=/manual/GameObjectBrush.html")]
    [CustomGridBrush(true, false, false, "GameObject Brush")]
    public class GameObjectBrush : GridBrushBase
    {
        [Serializable]
        internal class HiddenGridLayout
        {
            public Vector3 cellSize = Vector3.one;
            public Vector3 cellGap = Vector3.zero;
            public GridLayout.CellLayout cellLayout = GridLayout.CellLayout.Rectangle;
            public GridLayout.CellSwizzle cellSwizzle = GridLayout.CellSwizzle.XYZ;
        }

        [SerializeField]
        private BrushCell[] m_Cells;

        [SerializeField]
        private Vector3Int m_Size;

        [SerializeField]
        private Vector3Int m_Pivot;

        [SerializeField]
        [HideInInspector]
        private bool m_CanChangeZPosition;

        [SerializeField] 
        [HideInInspector] 
        internal HiddenGridLayout hiddenGridLayout = new HiddenGridLayout();

        /// <summary>
        /// GameObject used for painting onto the Scene root
        /// </summary>
        [HideInInspector]
        public GameObject hiddenGrid;

        /// <summary>
        /// Anchor Point of the Instantiated GameObject in the cell when painting
        /// </summary>
        public Vector3 m_Anchor = new Vector3(0.5f, 0.5f, 0.5f);
        /// <summary>Size of the brush in cells. </summary>
        public Vector3Int size { get { return m_Size; } set { m_Size = value; SizeUpdated(); } }
        /// <summary>Pivot of the brush. </summary>
        public Vector3Int pivot { get { return m_Pivot; } set { m_Pivot = value; } }
        /// <summary>All the brush cells the brush holds. </summary>
        public BrushCell[] cells { get { return m_Cells; } }
        /// <summary>Number of brush cells in the brush.</summary>
        public int cellCount { get { return m_Cells != null ? m_Cells.Length : 0; } }
        /// <summary>Number of brush cells based on size.</summary>
        public int sizeCount
        {
            get { return m_Size.x * m_Size.y * m_Size.y; }
        }
        
        /// <summary>
        /// This Brush instances, places and manipulates GameObjects onto the scene.
        /// </summary>
        public GameObjectBrush()
        {
            Init(Vector3Int.one, Vector3Int.zero);
            SizeUpdated();
        }

        private void OnEnable()
        {
            hiddenGrid = new GameObject();
            hiddenGrid.name = "(Paint on SceneRoot)";
            hiddenGrid.hideFlags = HideFlags.HideAndDontSave;
            hiddenGrid.transform.position = Vector3.zero;
            var grid = hiddenGrid.AddComponent<Grid>();
            grid.cellSize = hiddenGridLayout.cellSize;
            grid.cellGap = hiddenGridLayout.cellGap;
            grid.cellSwizzle = hiddenGridLayout.cellSwizzle;
            grid.cellLayout = hiddenGridLayout.cellLayout;
        }

        private void OnDisable()
        {
            DestroyImmediate(hiddenGrid);
        }

        /// <summary>
        /// Initializes the content of the GameObjectBrush.
        /// </summary>
        /// <param name="size">Size of the GameObjectBrush.</param>
        public void Init(Vector3Int size)
        {
            Init(size, Vector3Int.zero);
            SizeUpdated();
        }

        /// <summary>Initializes the content of the GameObjectBrush.</summary>
        /// <param name="size">Size of the GameObjectBrush.</param>
        /// <param name="pivot">Pivot point of the GameObjectBrush.</param>
        public void Init(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            m_Pivot = pivot;
            SizeUpdated();
        }

        /// <summary>
        /// Paints GameObjects into a given position within the selected layers.
        /// The GameObjectBrush overrides this to provide GameObject painting functionality.
        /// </summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, m_Size);
            
            if (brushTarget == hiddenGrid)
                brushTarget = null;
            // Do not allow editing palettes
            else if (brushTarget.layer == 31)
                return;
            BoxFill(gridLayout, brushTarget, bounds);
        }

        private void PaintCell(GridLayout grid, Vector3Int position, Transform parent, BrushCell cell)
        {
            if (cell.gameObject == null)
                return;

            var existingGO = GetObjectInCell(grid, parent, position);
            if (existingGO == null)
            {
                SetSceneCell(grid, parent, position, cell.gameObject, cell.offset, cell.scale, cell.orientation, m_Anchor);
            }
        }

        /// <summary>
        /// Erases GameObjects in a given position within the selected layers.
        /// The GameObjectBrush overrides this to provide GameObject erasing functionality.
        /// </summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to erase data from.</param>
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, m_Size);
            
            if (brushTarget == hiddenGrid)
                brushTarget = null;
            // Do not allow editing palettes
            else if (brushTarget.layer == 31)
                return;
            BoxErase(gridLayout, brushTarget, bounds);
        }

        private void EraseCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            ClearSceneCell(grid, parent, position);
        }

        /// <summary>
        /// Box fills GameObjects into given bounds within the selected layers.
        /// The GameObjectBrush overrides this to provide GameObject box-filling functionality.
        /// </summary>
        /// <param name="gridLayout">Grid to box fill data to.</param>
        /// <param name="brushTarget">Target of the box fill operation. By default the currently selected GameObject.</param>
        /// <param name="position">The bounds to box fill data into.</param>
        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == hiddenGrid)
                brushTarget = null;
            // Do not allow editing palettes
            else if (brushTarget != null && brushTarget.layer == 31)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell cell = m_Cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                PaintCell(gridLayout, location, brushTarget != null ? brushTarget.transform : null, cell);
            }
        }

        /// <summary>
        /// Erases GameObjects from given bounds within the selected layers.
        /// The GameObjectBrush overrides this to provide GameObject box-erasing functionality.
        /// </summary>
        /// <param name="gridLayout">Grid to erase data from.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The bounds to erase data from.</param>
        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == hiddenGrid)
                brushTarget = null;
            // Do not allow editing palettes
            else if (brushTarget != null && brushTarget.layer == 31)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                EraseCell(gridLayout, location, brushTarget != null ? brushTarget.transform : null);
            }
        }

        /// <summary>
        /// This is not supported but it should floodfill GameObjects starting from a given position within the selected layers.
        /// </summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the flood fill operation. By default the currently selected GameObject.</param>
        /// <param name="position">Starting position of the flood fill.</param>
        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Debug.LogWarning("FloodFill not supported");
        }

        /// <summary>
        /// Rotates the brush by 90 degrees in the given direction.
        /// </summary>
        /// <param name="direction">Direction to rotate by.</param>
        /// <param name="layout">Cell Layout for rotating.</param>
        public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        {
            Vector3Int oldSize = m_Size;
            BrushCell[] oldCells = m_Cells.Clone() as BrushCell[];
            size = new Vector3Int(oldSize.y, oldSize.x, oldSize.z);
            BoundsInt oldBounds = new BoundsInt(Vector3Int.zero, oldSize);

            foreach (Vector3Int oldPos in oldBounds.allPositionsWithin)
            {
                int newX = direction == RotationDirection.Clockwise ? oldSize.y - oldPos.y - 1 : oldPos.y;
                int newY = direction == RotationDirection.Clockwise ? oldPos.x : oldSize.x - oldPos.x - 1;
                int toIndex = GetCellIndex(newX, newY, oldPos.z);
                int fromIndex = GetCellIndex(oldPos.x, oldPos.y, oldPos.z, oldSize.x, oldSize.y, oldSize.z);
                m_Cells[toIndex] = oldCells[fromIndex];
            }

            int newPivotX = direction == RotationDirection.Clockwise ? oldSize.y - pivot.y - 1 : pivot.y;
            int newPivotY = direction == RotationDirection.Clockwise ? pivot.x : oldSize.x - pivot.x - 1;
            pivot = new Vector3Int(newPivotX, newPivotY, pivot.z);

            Matrix4x4 rotation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, direction == RotationDirection.Clockwise ? 90f : -90f), Vector3.one);
            Quaternion orientation = Quaternion.Euler(0f, 0f, direction == RotationDirection.Clockwise ? 90f : -90f);
            foreach (BrushCell cell in m_Cells)
            {
                cell.offset = rotation * cell.offset;
                cell.orientation = cell.orientation * orientation;
            }
        }

        /// <summary>Flips the brush in the given axis.</summary>
        /// <param name="flip">Axis to flip by.</param>
        /// <param name="layout">Cell Layout for flipping.</param>
        public override void Flip(FlipAxis flip, GridLayout.CellLayout layout)
        {
            if (flip == FlipAxis.X)
                FlipX();
            else
                FlipY();
        }

        /// <summary>
        /// Picks child GameObjects given the coordinates of the cells.
        /// The GameObjectBrush overrides this to provide GameObject picking functionality.
        /// </summary>
        /// <param name="gridLayout">Grid to pick data from.</param>
        /// <param name="brushTarget">Target of the picking operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cells to paint data from.</param>
        /// <param name="pivot">Pivot of the picking brush.</param>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
        {
            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), new Vector3Int(pivot.x, pivot.y, 0));

            if (brushTarget == hiddenGrid)
                brushTarget = null;
            // Do not allow editing palettes
            else if (brushTarget.layer == 31)
                return;

            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                PickCell(pos, brushPosition, gridLayout, brushTarget != null ? brushTarget.transform : null);
            }
        }

        private void PickCell(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
        {
            Vector3 cellCenter = grid.LocalToWorld(grid.CellToLocalInterpolated(position + m_Anchor));
            GameObject go = GetObjectInCell(grid, parent, position);

            if (go != null)
            {
                Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (prefab)
                {
                    SetGameObject(brushPosition, (GameObject) prefab);
                }
                else
                {
                    GameObject newInstance = Instantiate(go);
                    newInstance.hideFlags = HideFlags.HideAndDontSave;
                    newInstance.SetActive(false);
                    SetGameObject(brushPosition, newInstance);
                }

                SetOffset(brushPosition, go.transform.position - cellCenter);
                SetScale(brushPosition, go.transform.localScale);
                SetOrientation(brushPosition, go.transform.localRotation);
            }
        }

        /// <summary>
        /// MoveStart is called when user starts moving the area previously selected with the selection marquee.
        /// The GameObjectBrush overrides this to provide GameObject moving functionality.
        /// </summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the move operation. By default the currently selected GameObject.</param>
        /// <param name="position">Position where the move operation has started.</param>
        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), Vector3Int.zero);

            if (brushTarget == hiddenGrid)
                brushTarget = null;
            // Do not allow editing palettes
            else if (brushTarget.layer == 31)
                return;

            var targetTransform = brushTarget != null ? brushTarget.transform : null;
            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                PickCell(pos, brushPosition, gridLayout, targetTransform);
                ClearSceneCell(gridLayout, targetTransform, pos);
            }
        }

        /// <summary>
        /// MoveEnd is called when user has ended the move of the area previously selected with the selection marquee.
        /// The GameObjectBrush overrides this to provide GameObject moving functionality.
        /// </summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the move operation. By default the currently selected GameObject.</param>
        /// <param name="position">Position where the move operation has ended.</param>
        public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == hiddenGrid)
                brushTarget = null;
            // Do not allow editing palettes
            else if (brushTarget.layer == 31)
                return;

            Paint(gridLayout, brushTarget, position.min);
            Reset();
        }

        /// <summary>Clears all data of the brush.</summary>
        public void Reset()
        {
            foreach (var cell in m_Cells)
            {
                if (cell.gameObject != null && !EditorUtility.IsPersistent(cell.gameObject))
                {
                    DestroyImmediate(cell.gameObject);
                }
                cell.gameObject = null;
            }
            UpdateSizeAndPivot(Vector3Int.one, Vector3Int.zero);
        }

        private void FlipX()
        {
            BrushCell[] oldCells = m_Cells.Clone() as BrushCell[];
            BoundsInt oldBounds = new BoundsInt(Vector3Int.zero, m_Size);

            foreach (Vector3Int oldPos in oldBounds.allPositionsWithin)
            {
                int newX = m_Size.x - oldPos.x - 1;
                int toIndex = GetCellIndex(newX, oldPos.y, oldPos.z);
                int fromIndex = GetCellIndex(oldPos);
                m_Cells[toIndex] = oldCells[fromIndex];
            }

            int newPivotX = m_Size.x - pivot.x - 1;
            pivot = new Vector3Int(newPivotX, pivot.y, pivot.z);
            Matrix4x4 flip = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
            Quaternion orientation = Quaternion.Euler(0f, 0f, -180f);
            
            foreach (BrushCell cell in m_Cells)
            {
                Vector3 oldOffset = cell.offset;
                cell.offset = flip * oldOffset;
                cell.orientation = cell.orientation*orientation;
            }
        }

        private void FlipY()
        {
            BrushCell[] oldCells = m_Cells.Clone() as BrushCell[];
            BoundsInt oldBounds = new BoundsInt(Vector3Int.zero, m_Size);

            foreach (Vector3Int oldPos in oldBounds.allPositionsWithin)
            {
                int newY = m_Size.y - oldPos.y - 1;
                int toIndex = GetCellIndex(oldPos.x, newY, oldPos.z);
                int fromIndex = GetCellIndex(oldPos);
                m_Cells[toIndex] = oldCells[fromIndex];
            }

            int newPivotY = m_Size.y - pivot.y - 1;
            pivot = new Vector3Int(pivot.x, newPivotY, pivot.z);
            Matrix4x4 flip = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
            Quaternion orientation = Quaternion.Euler(0f, 0f, -180f);
            foreach (BrushCell cell in m_Cells)
            {
                Vector3 oldOffset = cell.offset;
                cell.offset = flip * oldOffset;
                cell.orientation = cell.orientation * orientation;
            }
        }

        /// <summary>Updates the size, pivot and the number of layers of the brush.</summary>
        /// <param name="size">New size of the brush.</param>
        /// <param name="pivot">New pivot of the brush.</param>
        public void UpdateSizeAndPivot(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            m_Pivot = pivot;
            SizeUpdated();
        }

        /// <summary>
        /// Sets a GameObject at the position in the brush.
        /// </summary>
        /// <param name="position">Position to set the GameObject in the brush.</param>
        /// <param name="go">GameObject to set in the brush.</param>
        public void SetGameObject(Vector3Int position, GameObject go)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].gameObject = go;
        }

        /// <summary>
        /// Sets a position offset at the position in the brush.
        /// </summary>
        /// <param name="position">Position to set the offset in the brush.</param>
        /// <param name="offset">Offset to set in the brush.</param>
        public void SetOffset(Vector3Int position, Vector3 offset)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].offset = offset;
        }

        /// <summary>
        /// Sets an orientation at the position in the brush.
        /// </summary>
        /// <param name="position">Position to set the orientation in the brush.</param>
        /// <param name="orientation">Orientation to set in the brush.</param>
        public void SetOrientation(Vector3Int position, Quaternion orientation)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].orientation = orientation;
        }

        /// <summary>
        /// Sets a scale at the position in the brush.
        /// </summary>
        /// <param name="position">Position to set the scale in the brush.</param>
        /// <param name="scale">Scale to set in the brush.</param>
        public void SetScale(Vector3Int position, Vector3 scale)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].scale = scale;
        }

        /// <summary>Gets the index to the GameObjectBrush::ref::BrushCell based on the position of the BrushCell.</summary>
        /// <param name="brushPosition">Position of the BrushCell.</param>
        /// <returns>The cell index for the position of the BrushCell.</returns>
        public int GetCellIndex(Vector3Int brushPosition)
        {
            return GetCellIndex(brushPosition.x, brushPosition.y, brushPosition.z);
        }

        /// <summary>Gets the index to the GameObjectBrush::ref::BrushCell based on the position of the BrushCell.</summary>
        /// <param name="x">X Position of the BrushCell.</param>
        /// <param name="y">Y Position of the BrushCell.</param>
        /// <param name="z">Z Position of the BrushCell.</param>
        /// <returns>The cell index for the position of the BrushCell.</returns>
        public int GetCellIndex(int x, int y, int z)
        {
            return x + m_Size.x * y + m_Size.x * m_Size.y * z;
        }

        /// <summary>Gets the index to the GameObjectBrush::ref::BrushCell based on the position of the BrushCell.</summary>
        /// <param name="x">X Position of the BrushCell.</param>
        /// <param name="y">Y Position of the BrushCell.</param>
        /// <param name="z">Z Position of the BrushCell.</param>
        /// <param name="sizex">X Size of Brush.</param>
        /// <param name="sizey">Y Size of Brush.</param>
        /// <param name="sizez">Z Size of Brush.</param>
        /// <returns>The cell index for the position of the BrushCell.</returns>
        public int GetCellIndex(int x, int y, int z, int sizex, int sizey, int sizez)
        {
            return x + sizex * y + sizex * sizey * z;
        }

        /// <summary>Gets the index to the GameObjectBrush::ref::BrushCell based on the position of the BrushCell. Wraps each coordinate if it is larger than the size of the GameObjectBrush.</summary>
        /// <param name="x">X Position of the BrushCell.</param>
        /// <param name="y">Y Position of the BrushCell.</param>
        /// <param name="z">Z Position of the BrushCell.</param>
        /// <returns>The cell index for the position of the BrushCell.</returns>
        public int GetCellIndexWrapAround(int x, int y, int z)
        {
            return (x % m_Size.x) + m_Size.x * (y % m_Size.y) + m_Size.x * m_Size.y * (z % m_Size.z);
        }

        private GameObject GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            int childCount = 0;
            GameObject[] sceneChildren = null;
            if (parent == null)
            {
                var scene = SceneManager.GetActiveScene();
                sceneChildren = scene.GetRootGameObjects();
                childCount = scene.rootCount;
            }
            else
            {
                childCount = parent.childCount;
            }
            var anchorCellOffset = Vector3Int.FloorToInt(m_Anchor);
            var cellSize = grid.cellSize;
            anchorCellOffset.x = cellSize.x == 0 ? 0 : anchorCellOffset.x;
            anchorCellOffset.y = cellSize.y == 0 ? 0 : anchorCellOffset.y;
            anchorCellOffset.z = cellSize.z == 0 ? 0 : anchorCellOffset.z;

            for (var i = 0; i < childCount; i++)
            {
                var child = sceneChildren == null ? parent.GetChild(i) : sceneChildren[i].transform;
                if (position == grid.WorldToCell(child.position) - anchorCellOffset)
                    return child.gameObject;
            }
            return null;
        }

        private bool ValidateCellPosition(Vector3Int position)
        {
            var valid =
                position.x >= 0 && position.x < size.x &&
                position.y >= 0 && position.y < size.y &&
                position.z >= 0 && position.z < size.z;
            if (!valid)
                throw new ArgumentException(string.Format("Position {0} is an invalid cell position. Valid range is between [{1}, {2}).", position, Vector3Int.zero, size));
            return valid;
        }

        internal void SizeUpdated(bool keepContents = false)
        {
            Array.Resize(ref m_Cells, sizeCount);
            BoundsInt bounds = new BoundsInt(Vector3Int.zero, m_Size);
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (keepContents || m_Cells[GetCellIndex(pos)] == null)
                    m_Cells[GetCellIndex(pos)] = new BrushCell();
            }
        }

        private static void SetSceneCell(GridLayout grid, Transform parent, Vector3Int position, GameObject go, Vector3 offset, Vector3 scale, Quaternion orientation, Vector3 anchor)
        {
            if (go == null)
                return;

            GameObject instance = null;
            if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                instance = (GameObject) PrefabUtility.InstantiatePrefab(go, parent != null ? parent.root.gameObject.scene : SceneManager.GetActiveScene());
                instance.transform.parent = parent;
            }
            else
            {
                instance = Instantiate(go, parent);
                instance.name = go.name;
                instance.SetActive(true);
                foreach (var renderer in instance.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = true;
                }
            }

            Undo.RegisterCreatedObjectUndo(instance, "Paint GameObject");
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, position.z) + anchor));
            instance.transform.localRotation = orientation;
            instance.transform.localScale = scale;
            instance.transform.Translate(offset);
        }

        private void ClearSceneCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            GameObject erased = GetObjectInCell(grid, parent, new Vector3Int(position.x, position.y, position.z));
            if (erased != null)
                Undo.DestroyObjectImmediate(erased);
        }

        /// <summary>
        /// Hashes the contents of the brush.
        /// </summary>
        /// <returns>A hash code of the brush</returns>
        public override int GetHashCode()
        {
            int hash = 0;
            unchecked
            {
                foreach (var cell in cells)
                {
                    hash = hash * 33 + cell.GetHashCode();
                }
            }
            return hash;
        }

        internal void UpdateHiddenGridLayout()
        {
            var grid = hiddenGrid.GetComponent<Grid>();
            hiddenGridLayout.cellSize = grid.cellSize;
            hiddenGridLayout.cellGap = grid.cellGap;
            hiddenGridLayout.cellSwizzle = grid.cellSwizzle;
            hiddenGridLayout.cellLayout = grid.cellLayout;
        }

        /// <summary>
        ///Brush Cell stores the data to be painted in a grid cell.
        /// </summary>
        [Serializable]
        public class BrushCell
        {
            /// <summary>
            /// GameObject to be placed when painting.
            /// </summary>
            public GameObject gameObject { get { return m_GameObject; } set { m_GameObject = value; } }
            /// <summary>
            /// Position offset of the GameObject when painted.
            /// </summary>
            public Vector3 offset { get { return m_Offset; } set { m_Offset = value; } }
            /// <summary>
            /// Scale of the GameObject when painted.
            /// </summary>
            public Vector3 scale { get { return m_Scale; } set { m_Scale = value; } }
            /// <summary>
            /// Orientatio of the GameObject when painted.
            /// </summary>
            public Quaternion orientation { get { return m_Orientation; } set { m_Orientation = value; } }
            
            [SerializeField]
            private GameObject m_GameObject;
            [SerializeField]
            Vector3 m_Offset = Vector3.zero;
            [SerializeField]
            Vector3 m_Scale = Vector3.one;
            [SerializeField]
            Quaternion m_Orientation = Quaternion.identity;

            /// <summary>
            /// Hashes the contents of the brush cell.
            /// </summary>
            /// <returns>A hash code of the brush cell.</returns>
            public override int GetHashCode()
            {
                int hash = 0;
                unchecked
                {
                    hash = gameObject != null ? gameObject.GetInstanceID() : 0;
                    hash = hash * 33 + m_Offset.GetHashCode();
                    hash = hash * 33 + m_Scale.GetHashCode();
                    hash = hash * 33 + m_Orientation.GetHashCode();
                }
                return hash;
            }
        }
    }

    /// <summary>
    /// The Brush Editor for a GameObject Brush.
    /// </summary>
    [CustomEditor(typeof(GameObjectBrush))]
    public class GameObjectBrushEditor : GridBrushEditorBase
    {
        private bool hiddenGridFoldout;
        private Editor hiddenGridEditor;

        /// <summary>
        /// The GameObjectBrush for this Editor
        /// </summary>
        public GameObjectBrush brush { get { return target as GameObjectBrush; } }

        /// <summary>
        /// Callback for painting the GUI for the GridBrush in the Scene View.
        /// The GameObjectBrush Editor overrides this to draw the preview of the brush when drawing lines.
        /// </summary>
        /// <param name="gridLayout">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">Target of the GameObjectBrush::ref::Tool operation. By default the currently selected GameObject.</param>
        /// <param name="position">Current selected location of the brush.</param>
        /// <param name="tool">Current GameObjectBrush::ref::Tool selected.</param>
        /// <param name="executing">Whether brush is being used.</param>
        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            BoundsInt gizmoRect = position;

            if (tool == GridBrushBase.Tool.Paint || tool == GridBrushBase.Tool.Erase)
                gizmoRect = new BoundsInt(position.min - brush.pivot, brush.size);
            
            base.OnPaintSceneGUI(gridLayout, brushTarget, gizmoRect, tool, executing);
        }

        /// <summary>
        /// Callback for painting the inspector GUI for the GameObjectBrush in the tilemap palette.
        /// The GameObjectBrush Editor overrides this to show the usage of this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck() && brush.cellCount != brush.sizeCount)
            {
                brush.SizeUpdated(true);
            }

            hiddenGridFoldout = EditorGUILayout.Foldout(hiddenGridFoldout, "SceneRoot Grid");
            if (hiddenGridFoldout)
            {
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(GridPaintingState.scenePaintTarget != brush.hiddenGrid))
                {
                    if (hiddenGridEditor == null)
                    {
                        hiddenGridEditor = Editor.CreateEditor(brush.hiddenGrid.GetComponent<Grid>());
                    }
                    brush.hiddenGrid.hideFlags = HideFlags.None;
                    EditorGUI.BeginChangeCheck();
                    hiddenGridEditor.OnInspectorGUI();
                    if (EditorGUI.EndChangeCheck())
                    {
                        brush.UpdateHiddenGridLayout();
                        EditorUtility.SetDirty(brush);
                        SceneView.RepaintAll();
                    }
                    brush.hiddenGrid.hideFlags = HideFlags.HideAndDontSave;
                }
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// The targets that the GameObjectBrush can paint on
        /// </summary>
        public override GameObject[] validTargets
        {
            get
            {
                StageHandle currentStageHandle = StageUtility.GetCurrentStageHandle();
                var results = currentStageHandle.FindComponentsOfType<GridLayout>().Where(x => x.gameObject.scene.isLoaded 
                    && x.gameObject.activeInHierarchy).Select(x => x.gameObject);
                return results.Prepend(brush.hiddenGrid).ToArray();
            }
        }

    }
}