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
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest/index.html?subfolder=/manual/RandomBrush.html")]
    [CustomGridBrush(false, false, false, "Random Brush")]
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
            /// <summary>
            /// A set of tiles to be painted as a set
            /// </summary>
            public TileBase[] randomTiles;
        }

        /// <summary>
        /// Container for storing Tile changes for Random Tile.
        /// </summary>
        [Serializable]
        public struct RandomTileChangeDataSet
        {
            /// <summary>
            /// A set of tiles to be painted as a set with transform and color data
            /// </summary>
            public TileChangeData[] randomTileChangeData;
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
        /// An array of RandomTileSets to choose from when randomizing 
        /// </summary>
        public RandomTileChangeDataSet[] randomTileChangeDataSets;
        
        /// <summary>
        /// A flag to determine if picking will add new RandomTileSets 
        /// </summary>
        public bool pickRandomTiles;

        /// <summary>
        /// A flag to determine if picking will add to existing RandomTileSets 
        /// </summary>
        public bool addToRandomTiles;
        
        private void OnEnable()
        {
            // Update brush from original randomTileSet
            if (randomTileSets == null 
                || (randomTileChangeDataSets != null
                && randomTileChangeDataSets.Length == randomTileSets.Length))
                return;
            
            randomTileChangeDataSets = new RandomTileChangeDataSet[randomTileSets.Length];
            for (var i = 0; i < randomTileSets.Length; ++i)
            {
                var sizeCount = randomTileSetSize.x * randomTileSetSize.y * randomTileSetSize.z;
                randomTileChangeDataSets[i].randomTileChangeData = new TileChangeData[sizeCount];
                for (var j = 0; j < sizeCount; ++j)
                {
                    randomTileChangeDataSets[i].randomTileChangeData[j].tile = randomTileSets[i].randomTiles[j];
                    randomTileChangeDataSets[i].randomTileChangeData[j].transform = Matrix4x4.identity;
                    randomTileChangeDataSets[i].randomTileChangeData[j].color = Color.white;
                }
            }
        }
        
        /// <summary>
        /// Paints RandomTileSets into a given position within the selected layers.
        /// The RandomBrush overrides this to provide randomized painting functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (randomTileChangeDataSets != null && randomTileChangeDataSets.Length > 0)
            {
                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;
                
                var min = position - pivot;
                foreach (var startLocation in new SizeEnumerator(min, min + size, randomTileSetSize))
                {
                    var randomTileChangeDataSet = randomTileChangeDataSets[(int) (randomTileChangeDataSets.Length * UnityEngine.Random.value)];
                    var randomBounds = new BoundsInt(startLocation, randomTileSetSize);
                    var i = 0;
                    foreach (var pos in randomBounds.allPositionsWithin)
                    {
                        randomTileChangeDataSet.randomTileChangeData[i++].position = pos;
                    }
                    tilemap.SetTiles(randomTileChangeDataSet.randomTileChangeData, false);
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
        /// <param name="bounds">The coordinates of the cells to paint data from.</param>
        /// <param name="pickStart">Pivot of the picking brush.</param>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds, Vector3Int pickStart)
        {
            base.Pick(gridLayout, brushTarget, bounds, pickStart);
            if (!pickRandomTiles)
                return;

            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            var i = 0;
            var count = ((bounds.size.x + randomTileSetSize.x - 1) / randomTileSetSize.x)
                        * ((bounds.size.y + randomTileSetSize.y - 1) / randomTileSetSize.y)
                        * ((bounds.size.z + randomTileSetSize.z - 1) / randomTileSetSize.z);
            if (addToRandomTiles)
            {
                i = randomTileSets != null ? randomTileSets.Length : 0;
                count += i;
            }
            Array.Resize(ref randomTileSets, count);
            Array.Resize(ref randomTileChangeDataSets, count);

            foreach (var startLocation in new SizeEnumerator(bounds.min, bounds.max, randomTileSetSize))
            {
                randomTileSets[i].randomTiles = new TileBase[randomTileSetSize.x * randomTileSetSize.y * randomTileSetSize.z];
                randomTileChangeDataSets[i].randomTileChangeData = new TileChangeData[randomTileSetSize.x * randomTileSetSize.y * randomTileSetSize.z];
                var randomBounds = new BoundsInt(startLocation, randomTileSetSize);
                var j = 0;
                foreach (var pos in randomBounds.allPositionsWithin)
                {
                    var inBounds = pos.x < bounds.max.x && pos.y < bounds.max.y && pos.z < bounds.max.z;
                    var tile = inBounds ? tilemap.GetTile(pos) : null;
                    randomTileSets[i].randomTiles[j] = tile;
                    randomTileChangeDataSets[i].randomTileChangeData[j].tile = tile;
                    randomTileChangeDataSets[i].randomTileChangeData[j].transform = inBounds ? tilemap.GetTransformMatrix(pos) : Matrix4x4.identity;
                    randomTileChangeDataSets[i].randomTileChangeData[j].color = inBounds ? tilemap.GetColor(pos) : Color.white;
                    j++;
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
        /// <param name="grid">Grid to paint data to.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (randomBrush.randomTileChangeDataSets != null && randomBrush.randomTileChangeDataSets.Length > 0)
            {
                base.PaintPreview(grid, null, position);
                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                var min = position - randomBrush.pivot;
                foreach (var startLocation in new RandomBrush.SizeEnumerator(min, min + randomBrush.size, randomBrush.randomTileSetSize))
                {
                    var randomTileChangeDataSet = randomBrush.randomTileChangeDataSets[(int) (randomBrush.randomTileChangeDataSets.Length * UnityEngine.Random.value)];
                    var randomBounds = new BoundsInt(startLocation, randomBrush.randomTileSetSize);
                    var j = 0;
                    foreach (Vector3Int pos in randomBounds.allPositionsWithin)
                    {
                        tilemap.SetEditorPreviewTile(pos, randomTileChangeDataSet.randomTileChangeData[j].tile);
                        tilemap.SetEditorPreviewTransformMatrix(pos, randomTileChangeDataSet.randomTileChangeData[j].transform);
                        tilemap.SetEditorPreviewColor(pos, randomTileChangeDataSet.randomTileChangeData[j].color);
                        j++;
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
                for (var i = 0; i < randomBrush.randomTileSets.Length; ++i)
                {
                    var sizeCount = randomBrush.randomTileSetSize.x * randomBrush.randomTileSetSize.y *
                                    randomBrush.randomTileSetSize.z;
                    randomBrush.randomTileSets[i].randomTiles = new TileBase[sizeCount];
                    randomBrush.randomTileChangeDataSets[i].randomTileChangeData = new TileChangeData[sizeCount];
                    for (var j = 0; j < sizeCount; ++j)
                    {
                        randomBrush.randomTileChangeDataSets[i].randomTileChangeData[j].tile = randomBrush.randomTileSets[i].randomTiles[j];
                        randomBrush.randomTileChangeDataSets[i].randomTileChangeData[j].transform = Matrix4x4.identity;
                        randomBrush.randomTileChangeDataSets[i].randomTileChangeData[j].color = Color.white;
                    }
                }
            }
            int randomTileSetCount = EditorGUILayout.DelayedIntField("Number of Tiles", randomBrush.randomTileSets != null ? randomBrush.randomTileSets.Length : 0);
            if (randomTileSetCount < 0)
                randomTileSetCount = 0;
            if (randomBrush.randomTileSets == null || randomBrush.randomTileSets.Length != randomTileSetCount)
            {
                Array.Resize(ref randomBrush.randomTileSets, randomTileSetCount);
                Array.Resize(ref randomBrush.randomTileChangeDataSets, randomTileSetCount);
                for (var i = 0; i < randomBrush.randomTileSets.Length; ++i)
                {
                    var sizeCount = randomBrush.randomTileSetSize.x * randomBrush.randomTileSetSize.y *
                                    randomBrush.randomTileSetSize.z;
                    if (randomBrush.randomTileSets[i].randomTiles == null
                        || randomBrush.randomTileSets[i].randomTiles.Length != sizeCount)
                    {
                        randomBrush.randomTileSets[i].randomTiles = new TileBase[sizeCount];
                        randomBrush.randomTileChangeDataSets[i].randomTileChangeData = new TileChangeData[sizeCount];
                        for (var j = 0; j < sizeCount; ++j)
                        {
                            randomBrush.randomTileChangeDataSets[i].randomTileChangeData[j].tile = randomBrush.randomTileSets[i].randomTiles[j];
                            randomBrush.randomTileChangeDataSets[i].randomTileChangeData[j].transform = Matrix4x4.identity;
                            randomBrush.randomTileChangeDataSets[i].randomTileChangeData[j].color = Color.white;
                        }
                    }
                }
            }

            if (randomTileSetCount > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Place random tiles.");

                for (var i = 0; i < randomTileSetCount; i++)
                {
                    EditorGUILayout.LabelField("Tile Set " + (i+1));
                    for (var j = 0; j < randomBrush.randomTileSets[i].randomTiles.Length; ++j)
                    {
                        randomBrush.randomTileSets[i].randomTiles[j] = (TileBase) EditorGUILayout.ObjectField("Tile " + (j+1), randomBrush.randomTileSets[i].randomTiles[j], typeof(TileBase), false, null);
                        if (randomBrush.randomTileChangeDataSets != null
                            && randomBrush.randomTileChangeDataSets.Length > i)
                        {
                            randomBrush.randomTileChangeDataSets[i].randomTileChangeData ??=
                                new TileChangeData[randomBrush.randomTileSets[i].randomTiles.Length];
                            randomBrush.randomTileChangeDataSets[i].randomTileChangeData[j].tile = randomBrush.randomTileSets[i].randomTiles[j];
                        }
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(randomBrush);
            }
        }
    }
}