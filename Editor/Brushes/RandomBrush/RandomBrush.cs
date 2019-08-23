using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush helps to place random Tiles onto a Tilemap.
    /// Use this as an example to create brushes which store specific data per brush and to make brushes which randomize behaviour.
    /// </summary>
    [CustomGridBrush(false, false, false, "Random Brush")]
    [CreateAssetMenu(fileName = "New Random Brush", menuName = "Brushes/Random Brush")]
    public class RandomBrush : GridBrush
    {
        internal struct SizeEnumerator : IEnumerator<Vector3Int>
        {
            private readonly Vector3Int _min, _max, _delta;
            private Vector3Int _current;

            public SizeEnumerator(Vector3Int min, Vector3Int max, Vector3Int delta)
            {
                _min = _current = min;
                _max = max;
                _delta = delta;
                Reset();
            }

            public SizeEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                if (_current.z >= _max.z)
                    return false;

                _current.x += _delta.x;
                if (_current.x >= _max.x)
                {
                    _current.x = _min.x;
                    _current.y += _delta.y;
                    if (_current.y >= _max.y)
                    {
                        _current.y = _min.y;
                        _current.z += _delta.z;
                        if (_current.z >= _max.z)
                            return false;
                    }
                }
                return true;
            }

            public void Reset()
            {
                _current = _min;
                _current.x -= _delta.x;
            }

            public Vector3Int Current { get { return _current; } }

            object IEnumerator.Current { get { return Current; } }

            void IDisposable.Dispose() {}
        }

        /// <summary>
        /// A data structure for storing a set of Tiles used for randomization
        /// </summary>
        [Serializable]
        public struct RandomTileSet
        {
            // A set of tiles to be painted as a set
            public TileBase[] randomTiles;
        }

        /// <summary>
        /// The size of a RandomTileSet
        /// </summary>
        public Vector3Int randomTileSetSize = Vector3Int.one;

        /// <summary>
        /// An array of RandomTileSets to choose from when randomizing 
        /// </summary>
        public RandomTileSet[] randomTileSets;

        /// <summary>
        /// A flag to determine if picking will add new RandomTileSets 
        /// </summary>
        public bool pickRandomTiles;

        /// <summary>
        /// A flag to determine if picking will add to existing RandomTileSets 
        /// </summary>
        public bool addToRandomTiles;

        /// <summary>
        /// Paints RandomTileSets into a given position within the selected layers.
        /// The RandomBrush overrides this to provide randomized painting functionality.
        /// </summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (randomTileSets != null && randomTileSets.Length > 0)
            {
                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;
                
                Vector3Int min = position - pivot;
                foreach (var startLocation in new SizeEnumerator(min, min + size, randomTileSetSize))
                {
                    var randomTileSet = randomTileSets[(int) (randomTileSets.Length * UnityEngine.Random.value)];
                    var randomBounds = new BoundsInt(startLocation, randomTileSetSize);
                    tilemap.SetTilesBlock(randomBounds, randomTileSet.randomTiles);
                }
            }
            else
            {
                base.Paint(grid, brushTarget, position);
            }
        }

        /// <summary>
        /// Picks RandomTileSets given the coordinates of the cells.
        /// The RandomBrush overrides this to provide picking functionality for RandomTileSets.
        /// </summary>
        /// <param name="gridLayout">Grid to pick data from.</param>
        /// <param name="brushTarget">Target of the picking operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cells to paint data from.</param>
        /// <param name="pickStart">Pivot of the picking brush.</param>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds, Vector3Int pickStart)
        {
            base.Pick(gridLayout, brushTarget, bounds, pickStart);
            if (!pickRandomTiles)
                return;

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            int i = 0;
            int count = ((bounds.size.x + randomTileSetSize.x - 1) / randomTileSetSize.x)
                        * ((bounds.size.y + randomTileSetSize.y - 1) / randomTileSetSize.y)
                        * ((bounds.size.z + randomTileSetSize.z - 1) / randomTileSetSize.z);
            if (addToRandomTiles)
            {
                i = randomTileSets != null ? randomTileSets.Length : 0;
                count += i;
            }
            Array.Resize(ref randomTileSets, count);

            foreach (var startLocation in new SizeEnumerator(bounds.min, bounds.max, randomTileSetSize))
            {
                randomTileSets[i].randomTiles = new TileBase[randomTileSetSize.x * randomTileSetSize.y * randomTileSetSize.z];
                var randomBounds = new BoundsInt(startLocation, randomTileSetSize);
                int j = 0;
                foreach (Vector3Int pos in randomBounds.allPositionsWithin)
                {
                    var tile = (pos.x < bounds.max.x && pos.y < bounds.max.y && pos.z < bounds.max.z)
                        ? tilemap.GetTile(pos)
                        : null;
                    randomTileSets[i].randomTiles[j++] = tile;
                }
                i++;
            }
        }
    }

    /// <summary>
    /// The Brush Editor for a Random Brush.
    /// </summary>
    [CustomEditor(typeof(RandomBrush))]
    public class RandomBrushEditor : GridBrushEditor
    {
        private RandomBrush randomBrush { get { return target as RandomBrush; } }
        private GameObject lastBrushTarget;

        /// <summary>
        /// Paints preview data into a cell of a grid given the coordinates of the cell.
        /// The RandomBrush Editor overrides this to draw the preview of the brush for RandomTileSets
        /// </summary>
        /// <param name="gridLayout">Grid to paint data to.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (randomBrush.randomTileSets != null && randomBrush.randomTileSets.Length > 0)
            {
                base.PaintPreview(grid, null, position);
                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                Vector3Int min = position - randomBrush.pivot;
                foreach (var startLocation in new RandomBrush.SizeEnumerator(min, min + randomBrush.size, randomBrush.randomTileSetSize))
                {
                    var randomTileSet = randomBrush.randomTileSets[(int) (randomBrush.randomTileSets.Length * UnityEngine.Random.value)];
                    var randomBounds = new BoundsInt(startLocation, randomBrush.randomTileSetSize);
                    int j = 0;
                    foreach (Vector3Int pos in randomBounds.allPositionsWithin)
                    {
                        tilemap.SetEditorPreviewTile(pos, randomTileSet.randomTiles[j++]);
                    }
                }
                lastBrushTarget = brushTarget;
            }
            else
            {
                base.PaintPreview(grid, brushTarget, position);
            }
        }

        /// <summary>
        /// Clears all RandomTileSet previews.
        /// </summary>
        public override void ClearPreview()
        {
            if (lastBrushTarget != null)
            {
                var tilemap = lastBrushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;
                
                tilemap.ClearAllEditorPreviewTiles();
                
                lastBrushTarget = null;
            }
            else
            {
                base.ClearPreview();
            }
        }

        /// <summary>
        /// Callback for painting the inspector GUI for the RandomBrush in the Tile Palette.
        /// The RandomBrush Editor overrides this to have a custom inspector for this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            randomBrush.pickRandomTiles = EditorGUILayout.Toggle("Pick Random Tiles", randomBrush.pickRandomTiles);
            using (new EditorGUI.DisabledScope(!randomBrush.pickRandomTiles))
            {
                randomBrush.addToRandomTiles = EditorGUILayout.Toggle("Add To Random Tiles", randomBrush.addToRandomTiles);
            }

            EditorGUI.BeginChangeCheck();
            randomBrush.randomTileSetSize = EditorGUILayout.Vector3IntField("Tile Set Size", randomBrush.randomTileSetSize);
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < randomBrush.randomTileSets.Length; ++i)
                {
                    int sizeCount = randomBrush.randomTileSetSize.x * randomBrush.randomTileSetSize.y *
                                    randomBrush.randomTileSetSize.z;
                    randomBrush.randomTileSets[i].randomTiles = new TileBase[sizeCount];
                }
            }
            int randomTileSetCount = EditorGUILayout.DelayedIntField("Number of Tiles", randomBrush.randomTileSets != null ? randomBrush.randomTileSets.Length : 0);
            if (randomTileSetCount < 0)
                randomTileSetCount = 0;
            if (randomBrush.randomTileSets == null || randomBrush.randomTileSets.Length != randomTileSetCount)
            {
                Array.Resize(ref randomBrush.randomTileSets, randomTileSetCount);
                for (int i = 0; i < randomBrush.randomTileSets.Length; ++i)
                {
                    int sizeCount = randomBrush.randomTileSetSize.x * randomBrush.randomTileSetSize.y *
                                    randomBrush.randomTileSetSize.z;
                    if (randomBrush.randomTileSets[i].randomTiles == null
                        || randomBrush.randomTileSets[i].randomTiles.Length != sizeCount)
                        randomBrush.randomTileSets[i].randomTiles = new TileBase[sizeCount];
                }
            }

            if (randomTileSetCount > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Place random tiles.");

                for (int i = 0; i < randomTileSetCount; i++)
                {
                    EditorGUILayout.LabelField("Tile Set " + (i+1));
                    for (int j = 0; j < randomBrush.randomTileSets[i].randomTiles.Length; ++j)
                    {
                        randomBrush.randomTileSets[i].randomTiles[j] = (TileBase) EditorGUILayout.ObjectField("Tile " + (j+1), randomBrush.randomTileSets[i].randomTiles[j], typeof(TileBase), false, null);                        
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(randomBrush);
        }
    }
}