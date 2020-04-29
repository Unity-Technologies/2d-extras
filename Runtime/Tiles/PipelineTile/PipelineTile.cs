using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// Pipeline Tiles are tiles which take into consideration its orthogonal neighboring tiles and displays a sprite depending on whether the neighboring tile is the same tile.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Pipeline Tile", menuName = "2D Extras/Tiles/Pipeline Tile", order = 359)]
    public class PipelineTile : TileBase
    {
        /// <summary>
        /// The Sprites used for defining the Pipeline.
        /// </summary>
        [SerializeField]
        public Sprite[] m_Sprites;

        /// <summary>
        /// This method is called when the tile is refreshed.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tileMap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            for (int yd = -1; yd <= 1; yd++)
                for (int xd = -1; xd <= 1; xd++)
                {
                    Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                    if (TileValue(tileMap, position))
                        tileMap.RefreshTile(position);
                }
        }

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            UpdateTile(location, tileMap, ref tileData);
        }

        private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;

            int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 2 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 4 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 8 : 0;

            int index = GetIndex((byte)mask);
            if (index >= 0 && index < m_Sprites.Length && TileValue(tileMap, location))
            {
                tileData.sprite = m_Sprites[index];
                tileData.transform = GetTransform((byte)mask);
                tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
                tileData.colliderType = Tile.ColliderType.Sprite;
            }
        }

        private bool TileValue(ITilemap tileMap, Vector3Int position)
        {
            TileBase tile = tileMap.GetTile(position);
            return (tile != null && tile == this);
        }

        private int GetIndex(byte mask)
        {
            switch (mask)
            {
                case 0: return 0;
                case 3:
                case 6:
                case 9:
                case 12: return 1;
                case 1:
                case 2:
                case 4:
                case 5:
                case 10:
                case 8: return 2;
                case 7:
                case 11:
                case 13:
                case 14: return 3;
                case 15: return 4;
            }
            return -1;
        }

        private Matrix4x4 GetTransform(byte mask)
        {
            switch (mask)
            {
                case 9:
                case 10:
                case 7:
                case 2:
                case 8:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);
                case 3: 
                case 14:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -180f), Vector3.one);
                case 6: 
                case 13:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -270f), Vector3.one);
            }
            return Matrix4x4.identity;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(PipelineTile))]
    public class PipelineTileEditor : Editor
    {
        private PipelineTile tile { get { return (target as PipelineTile); } }

        public void OnEnable()
        {
            if (tile.m_Sprites == null || tile.m_Sprites.Length != 5)
                tile.m_Sprites = new Sprite[5];
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Place sprites shown based on the number of tiles bordering it.");
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            tile.m_Sprites[0] = (Sprite) EditorGUILayout.ObjectField("None", tile.m_Sprites[0], typeof(Sprite), false, null);
            tile.m_Sprites[2] = (Sprite) EditorGUILayout.ObjectField("One", tile.m_Sprites[2], typeof(Sprite), false, null);
            tile.m_Sprites[1] = (Sprite) EditorGUILayout.ObjectField("Two", tile.m_Sprites[1], typeof(Sprite), false, null);
            tile.m_Sprites[3] = (Sprite) EditorGUILayout.ObjectField("Three", tile.m_Sprites[3], typeof(Sprite), false, null);
            tile.m_Sprites[4] = (Sprite) EditorGUILayout.ObjectField("Four", tile.m_Sprites[4], typeof(Sprite), false, null);
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tile);
        }
    }
#endif
}
