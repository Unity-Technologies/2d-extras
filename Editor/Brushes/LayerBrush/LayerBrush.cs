using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush targets multiple Tilemaps linked through the same GridLayout.
    /// Use this as an example to edit multiple Tilemaps at once.
    /// </summary>
    [CustomGridBrush(true, false, false, "Layer Brush")]
    public class LayerBrush : GridBrush
    {
        internal GridBrush[] gridBrushes;
        internal GameObject[] brushTargets;
        private GridLayout cachedGridLayout;

        internal bool ValidateAndCacheBrushTargetsFromGridLayout(GridLayout gridLayout)
        {
            if (cachedGridLayout == gridLayout)
                return true;

            if (gridBrushes == null)
                return false;

            var tilemaps = gridLayout.GetComponentsInChildren<Tilemap>();
            if (tilemaps.Length != gridBrushes.Length)
                return false;

            cachedGridLayout = gridLayout;
            CacheBrushTargets(tilemaps);
            return true;
        }

        private void CacheBrushTargets(Tilemap[] tilemaps)
        {
            if (brushTargets == null || brushTargets.Length != tilemaps.Length)
            {
                brushTargets = new GameObject[tilemaps.Length];
            }
            for (int i = 0; i < tilemaps.Length; ++i)
            {
                brushTargets[i] = tilemaps[i].gameObject;
            }
        }
        
        private void CacheGridLayout(GridLayout gridLayout)
        {
            if (cachedGridLayout == gridLayout)
                return;

            var tilemaps = gridLayout.gameObject.GetComponentsInChildren<Tilemap>();
            if (gridBrushes == null || gridBrushes.Length != tilemaps.Length)
            {
                gridBrushes = new GridBrush[tilemaps.Length];
                for (int i = 0; i < tilemaps.Length; ++i)
                {
                    gridBrushes[i] = ScriptableObject.CreateInstance<GridBrush>();
                }
            }
            else
            {
                foreach (var gridBrush in gridBrushes)
                    gridBrush.Reset();
            }

            cachedGridLayout = gridLayout;
            CacheBrushTargets(tilemaps);
        }

        public override void Select(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            CacheGridLayout(gridLayout);
            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].Select(gridLayout, brushTargets[i], position);
        }

        public override void Move(GridLayout gridLayout, GameObject brushTarget, BoundsInt from, BoundsInt to)
        {
            CacheGridLayout(gridLayout);
            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].Move(gridLayout, brushTargets[i], from, to);
        }

        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            CacheGridLayout(gridLayout);
            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].MoveStart(gridLayout, brushTargets[i], position);
        }

        public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            CacheGridLayout(gridLayout);
            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].MoveEnd(gridLayout, brushTargets[i], position);
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            CacheGridLayout(gridLayout);
            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].Paint(gridLayout, brushTargets[i], position);
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (!ValidateAndCacheBrushTargetsFromGridLayout(gridLayout))
                return;

            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].Erase(gridLayout, brushTargets[i], position);
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            CacheGridLayout(gridLayout);
            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].Pick(gridLayout, brushTargets[i], position, pickStart);
            base.Pick(gridLayout, brushTarget, position, pickStart);
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (!ValidateAndCacheBrushTargetsFromGridLayout(gridLayout))
                return;

            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].FloodFill(gridLayout, brushTargets[i], position);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (!ValidateAndCacheBrushTargetsFromGridLayout(gridLayout))
                return;

            for (int i = 0; i < gridBrushes.Length; ++i)
                gridBrushes[i].BoxFill(gridLayout, brushTargets[i], position);
        }

        public override void Flip(FlipAxis flip, GridLayout.CellLayout layout)
        {
            if (gridBrushes == null)
                return;

            foreach (var gridBrush in gridBrushes)
            {
                gridBrush.Flip(flip, layout);
            }
        }

        public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        {
            if (gridBrushes == null)
                return;

            foreach (var gridBrush in gridBrushes)
            {
                gridBrush.Rotate(direction, layout);
            }
        }

        public override void ChangeZPosition(int change)
        {
            if (gridBrushes == null)
                return;

            foreach (var gridBrush in gridBrushes)
            {
                gridBrush.ChangeZPosition(change);
            }
        }

        public override void ResetZPosition()
        {
            if (gridBrushes == null)
                return;

            foreach (var gridBrush in gridBrushes)
            {
                gridBrush.ResetZPosition();
            }
        }
    }

    /// <summary>
    /// The Brush Editor for a Layer Brush.
    /// </summary>
    [CustomEditor(typeof(LayerBrush))]
    public class LayerBrushEditor : GridBrushEditor
    {
        public LayerBrush layerBrush
        {
            get
            {
                return target as LayerBrush;
            }
        }

        private GridBrushEditor[] editors;

        private void CreateEditor()
        {
            if (layerBrush.gridBrushes != null
                && (editors == null
                || editors.Length != layerBrush.gridBrushes.Length))
            {
                editors = new GridBrushEditor[layerBrush.gridBrushes.Length];
                for (int i = 0; i < editors.Length; ++i)
                {
                    editors[i] = Editor.CreateEditor(layerBrush.gridBrushes[i]) as GridBrushEditor;
                }
            }
        }

        private bool ValidatePreview(GridLayout gridLayout)
        {
            if (editors != null
                && layerBrush.gridBrushes != null
                && layerBrush.brushTargets != null)
            {
                if (gridLayout is Tilemap)
                {
                    return layerBrush.ValidateAndCacheBrushTargetsFromGridLayout(((Tilemap) gridLayout).layoutGrid);
                }

                return layerBrush.ValidateAndCacheBrushTargetsFromGridLayout(gridLayout);
            }
            return false;
        }

        public override void RegisterUndo(GameObject brushTarget, GridBrushBase.Tool tool)
        {
            if (layerBrush.brushTargets == null 
                || layerBrush.brushTargets.Length == 0)
                return;

            var count = layerBrush.brushTargets.Length;
            var undoObjects = new UnityEngine.Object[count * 2];
            for (int i = 0; i < layerBrush.brushTargets.Length; i++)
            {
                undoObjects[i] = layerBrush.brushTargets[i];
                undoObjects[i + count] = layerBrush.brushTargets[i].GetComponent<Tilemap>();
            }
            Undo.RegisterCompleteObjectUndo(undoObjects, tool.ToString());
        }

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position,
            GridBrushBase.Tool tool, bool executing)
        {
            CreateEditor();
            if (ValidatePreview(gridLayout))
            {
                for (int i = 0; i < editors.Length; ++i)
                {
                    editors[i].OnPaintSceneGUI(gridLayout, layerBrush.brushTargets[i], position, tool, executing);
                }
            }
            else
            {
                base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
            }
        }

        public override void PaintPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            CreateEditor();
            if (ValidatePreview(gridLayout))

            {
                for (int i = 0; i < editors.Length; ++i)
                {
                    editors[i].PaintPreview(gridLayout, layerBrush.brushTargets[i], position);
                }
            }
        }

        public override void BoxFillPreview(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            CreateEditor();
            if (ValidatePreview(gridLayout))
            {
                for (int i = 0; i < editors.Length; ++i)
                {
                    editors[i].BoxFillPreview(gridLayout, layerBrush.brushTargets[i], position);
                }
            }
        }

        public override void FloodFillPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            CreateEditor();
            if (ValidatePreview(gridLayout))
            {
                for (int i = 0; i < editors.Length; ++i)
                {
                    editors[i].FloodFillPreview(gridLayout, layerBrush.brushTargets[i], position);
                }
            }
        }

        public override void ClearPreview()
        {
            CreateEditor();
            if (editors != null)
            {
                foreach (var editor in editors)
                {
                    editor.ClearPreview();
                }
            }
        }
    }
}