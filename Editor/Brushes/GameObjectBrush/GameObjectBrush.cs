using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    [CustomGridBrush(true, false, false, "GameObject Brush")]
    public class GameObjectBrush : GridBrushBase
    {
        [SerializeField]
        [HideInInspector]
        private BrushCell[] m_Cells;

        [SerializeField]
        [HideInInspector]
        private Vector3Int m_Size;

        [SerializeField]
        [HideInInspector]
        private Vector3Int m_Pivot;

        public Vector3Int size { get { return m_Size; } set { m_Size = value; SizeUpdated(); } }
        public Vector3Int pivot { get { return m_Pivot; } set { m_Pivot = value; } }
        public BrushCell[] cells { get { return m_Cells; } }
        public int cellCount { get { return m_Cells != null ? m_Cells.Length : 0; } }

        public GameObjectBrush()
        {
            Init(Vector3Int.one, Vector3Int.zero);
            SizeUpdated();
        }

        public void Init(Vector3Int size)
        {
            Init(size, Vector3Int.zero);
            SizeUpdated();
        }

        public void Init(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            m_Pivot = pivot;
            SizeUpdated();
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, m_Size);
            BoxFill(gridLayout, brushTarget, bounds);
        }

        private void PaintCell(GridLayout grid, Vector3Int position, Transform parent, BrushCell cell)
        {
            if (cell.gameObject != null)
            {
                SetSceneCell(grid, parent, position, cell.gameObject, cell.offset, cell.scale, cell.orientation);
            }
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, m_Size);
            BoxErase(gridLayout, brushTarget, bounds);
        }

        private void EraseCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            ClearSceneCell(grid, parent, position);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            if (brushTarget == null)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell cell = m_Cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                PaintCell(gridLayout, location, brushTarget.transform, cell);
            }
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            if (brushTarget == null)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                EraseCell(gridLayout, location, brushTarget.transform);
            }
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Debug.LogWarning("FloodFill not supported");
        }

        public override void Rotate(RotationDirection direction, Grid.CellLayout layout)
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

        public override void Flip(FlipAxis flip, Grid.CellLayout layout)
        {
            if (flip == FlipAxis.X)
                FlipX();
            else
                FlipY();
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), new Vector3Int(pickStart.x, pickStart.y, 0));

            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                PickCell(pos, brushPosition, gridLayout, brushTarget.transform);
            }
        }

        private void PickCell(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
        {
            if (parent != null)
            {
                Vector3 cellCenter = grid.LocalToWorld(grid.CellToLocalInterpolated(position + new Vector3(.5f, .5f, .5f)));
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
                        SetGameObject(brushPosition, newInstance);
                    }

                    SetOffset(brushPosition, go.transform.position - cellCenter);
                    SetScale(brushPosition, go.transform.localScale);
                    SetOrientation(brushPosition, go.transform.localRotation);
                }
            }
        }

        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), Vector3Int.zero);

            if (brushTarget != null)
            {
                foreach (Vector3Int pos in position.allPositionsWithin)
                {
                    Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                    PickCell(pos, brushPosition, gridLayout, brushTarget.transform);
                    ClearSceneCell(gridLayout, brushTarget.transform, brushPosition);
                }
            }
        }

        public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Paint(gridLayout, brushTarget, position.min);
            Reset();
        }

        public void Reset()
        {
            foreach (var cell in m_Cells)
            {
                if (cell.gameObject != null && !EditorUtility.IsPersistent(cell.gameObject))
                {
                    DestroyImmediate(cell.gameObject);
                }
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

        public void UpdateSizeAndPivot(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            m_Pivot = pivot;
            SizeUpdated();
        }

        public void SetGameObject(Vector3Int position, GameObject go)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].gameObject = go;
        }

        public void SetOffset(Vector3Int position, Vector3 offset)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].offset = offset;
        }

        public void SetOrientation(Vector3Int position, Quaternion orientation)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].orientation = orientation;
        }
        
        public void SetScale(Vector3Int position, Vector3 scale)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].scale = scale;
        }

        public int GetCellIndex(Vector3Int brushPosition)
        {
            return GetCellIndex(brushPosition.x, brushPosition.y, brushPosition.z);
        }

        public int GetCellIndex(int x, int y, int z)
        {
            return x + m_Size.x * y + m_Size.x * m_Size.y * z;
        }

        public int GetCellIndex(int x, int y, int z, int sizex, int sizey, int sizez)
        {
            return x + sizex * y + sizex * sizey * z;
        }

        public int GetCellIndexWrapAround(int x, int y, int z)
        {
            return (x % m_Size.x) + m_Size.x * (y % m_Size.y) + m_Size.x * m_Size.y * (z % m_Size.z);
        }

        private static GameObject GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            int childCount = parent.childCount;
            Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
            Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
            
            // Infinite bounds on Z for 2D convenience
            min = new Vector3(min.x, min.y, float.MinValue);
            max = new Vector3(max.x, max.y, float.MaxValue);

            Bounds bounds = new Bounds((max + min) * .5f, max - min);

            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (bounds.Contains(child.position))
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

        private void SizeUpdated()
        {
            m_Cells = new BrushCell[m_Size.x * m_Size.y * m_Size.z];
            BoundsInt bounds = new BoundsInt(Vector3Int.zero, m_Size);
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                m_Cells[GetCellIndex(pos)] = new BrushCell();
            }
        }

        private static void SetSceneCell(GridLayout grid, Transform parent, Vector3Int position, GameObject go, Vector3 offset, Vector3 scale, Quaternion orientation)
        {
            if (parent == null || go == null)
                return;

            GameObject instance = null;
            if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                instance = (GameObject) PrefabUtility.InstantiatePrefab(go);
            }
            else
            {
                instance = Instantiate(go);
                instance.hideFlags = HideFlags.None;
                instance.name = go.name;
            }

            Undo.RegisterCreatedObjectUndo(instance, "Paint GameObject");
            instance.transform.SetParent(parent);
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, position.z) + new Vector3(.5f, .5f, .5f)));
            instance.transform.localRotation = orientation;
            instance.transform.localScale = scale;
            instance.transform.Translate(offset);
        }

        private static void ClearSceneCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            if (parent == null)
                return;

            GameObject erased = GetObjectInCell(grid, parent, new Vector3Int(position.x, position.y, position.z));
            if (erased != null)
                Undo.DestroyObjectImmediate(erased);
        }

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

        [Serializable]
        public class BrushCell
        {
            public GameObject gameObject { get { return m_GameObject; } set { m_GameObject = value; } }
            public Vector3 offset { get { return m_Offset; } set { m_Offset = value; } }
            public Vector3 scale { get { return m_Scale; } set { m_Scale = value; } }
            public Quaternion orientation { get { return m_Orientation; } set { m_Orientation = value; } }
            
            [SerializeField]
            private GameObject m_GameObject;
            [SerializeField]
            Vector3 m_Offset = Vector3.zero;
            [SerializeField]
            Vector3 m_Scale = Vector3.one;
            [SerializeField]
            Quaternion m_Orientation = Quaternion.identity;

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

    [CustomEditor(typeof(GameObjectBrush))]
    public class GameObjectBrushEditor : GridBrushEditorBase
    {
        public GameObjectBrush brush { get { return target as GameObjectBrush; } }

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            BoundsInt gizmoRect = position;

            if (tool == GridBrushBase.Tool.Paint || tool == GridBrushBase.Tool.Erase)
                gizmoRect = new BoundsInt(position.min - brush.pivot, brush.size);
            
            base.OnPaintSceneGUI(gridLayout, brushTarget, gizmoRect, tool, executing);
        }

        public override void OnPaintInspectorGUI()
        {
            GUILayout.Label("Pick, paint and erase GameObject(s) in the scene.");
            GUILayout.Label("Limited to children of the currently selected GameObject.");
        }
    }
}
