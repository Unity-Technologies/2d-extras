using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// Terrain Tiles, similar to Pipeline Tiles, are tiles which take into consideration its orthogonal and diagonal neighboring tiles and displays a sprite depending on whether the neighboring tile is the same tile.
    /// </summary>
    [Serializable]
    public class WeightedRandomTerrainTile : TileBase
    {
        [Serializable]
        public struct WeightedSprites
        {
            public List<WeightedSprite> m_Sprites;
        }
        
        /// <summary>
        /// The Sprites used for defining the Terrain.
        /// </summary>
        public List<WeightedSprites> m_WeightedList;
        
        /// <summary>
        /// This method is called when the tile is refreshed.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            for (int yd = -1; yd <= 1; yd++)
                for (int xd = -1; xd <= 1; xd++)
                {
                    Vector3Int pos = new Vector3Int(position.x + xd, position.y + yd, position.z);
                    if (TileValue(tilemap, pos))
                        tilemap.RefreshTile(pos);
                }
        }

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            UpdateTile(position, tilemap, ref tileData);
        }

        private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;

            int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

            byte original = (byte)mask;
            if ((original | 254) < 255) { mask = mask & 125; }
            if ((original | 251) < 255) { mask = mask & 245; }
            if ((original | 239) < 255) { mask = mask & 215; }
            if ((original | 191) < 255) { mask = mask & 95; }

            int index = GetIndex((byte)mask);
            if (index >= 0 && index < m_WeightedList.Count && TileValue(tileMap, location))
            {
                tileData.sprite = PickSprite(index, location);
                tileData.transform = GetTransform((byte)mask);
                tileData.color = Color.white;
                tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
                tileData.colliderType = Tile.ColliderType.Sprite;
            }
        }

        private Sprite PickSprite(int index, Vector3Int location)
        {
            if (m_WeightedList.Count <= index)
                return null;
            
            var weightedSprite = m_WeightedList[index];
            if (weightedSprite.m_Sprites.Count == 0)
                return null;
            
            var sprite = weightedSprite.m_Sprites[0].Sprite;
            
            var oldState = Random.state;
            long hash = location.x;
            hash = hash + 0xabcd1234 + (hash << 15);
            hash = hash + 0x0987efab ^ (hash >> 11);
            hash ^= location.y;
            hash = hash + 0x46ac12fd + (hash << 7);
            hash = hash + 0xbe9730af ^ (hash << 11);
            Random.InitState((int) hash);

            // Get the cumulative weight of the sprites
            var cumulativeWeight = 0;
            foreach (var spriteInfo in weightedSprite.m_Sprites) cumulativeWeight += spriteInfo.Weight;

            // Pick a random weight and choose a sprite depending on it
            var randomWeight = Random.Range(0, cumulativeWeight);
            foreach (var spriteInfo in weightedSprite.m_Sprites) 
            {
                randomWeight -= spriteInfo.Weight;
                if (randomWeight < 0) 
                {
                    sprite = spriteInfo.Sprite;    
                    break;
                }
            }
            Random.state = oldState;

            return sprite;
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
                case 1:
                case 4:
                case 16:
                case 64: return 1;
                case 5:
                case 20:
                case 80:
                case 65: return 2;
                case 7:
                case 28:
                case 112:
                case 193: return 3;
                case 17:
                case 68: return 4;
                case 21:
                case 84:
                case 81:
                case 69: return 5;
                case 23:
                case 92:
                case 113:
                case 197: return 6;
                case 29:
                case 116:
                case 209:
                case 71: return 7;
                case 31:
                case 124:
                case 241:
                case 199: return 8;
                case 85: return 9;
                case 87:
                case 93:
                case 117:
                case 213: return 10;
                case 95:
                case 125:
                case 245:
                case 215: return 11;
                case 119:
                case 221: return 12;
                case 127:
                case 253:
                case 247:
                case 223: return 13;
                case 255: return 14;
            }
            return -1;
        }

        private Matrix4x4 GetTransform(byte mask)
        {
            switch (mask)
            {
                case 4:
                case 20:
                case 28:
                case 68:
                case 84:
                case 92:
                case 116:
                case 124:
                case 93:
                case 125:
                case 221:
                case 253:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);
                case 16:
                case 80:
                case 112:
                case 81:
                case 113:
                case 209:
                case 241:
                case 117:
                case 245:
                case 247:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -180f), Vector3.one);
                case 64:
                case 65:
                case 193:
                case 69:
                case 197:
                case 71:
                case 199:
                case 213:
                case 215:
                case 223:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -270f), Vector3.one);
            }
            return Matrix4x4.identity;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(WeightedRandomTerrainTile))]
    public class WeightedRandomTerrainTileEditor : Editor
    {
        private static readonly string[] fieldNames = new[]
        {
            "Filled",
            "Three Sides",
            "Two Sides and One Corner",
            "Two Adjacent Sides",
            "Two Opposite Sides",
            "One Side and Two Corners",
            "One Side and One Lower Corner",
            "One Side and One Upper Corner",
            "One Side",
            "Four Corners",
            "Three Corners",
            "Two Adjacent Corners",
            "Two Opposite Corners",
            "One Corner",
            "Empty"
        };

        private WeightedRandomTerrainTile tile { get { return (target as WeightedRandomTerrainTile); } }

        private SerializedProperty m_WeightedList;
        
        
        /// <summary>
        /// OnEnable for TerrainTile.
        /// </summary>
        public void OnEnable()
        {
            m_WeightedList = serializedObject.FindProperty("m_WeightedList");
            if (m_WeightedList.arraySize != 15)
            {
                m_WeightedList.arraySize = 15;
                for (int i = 0; i < 15; ++i)
                {
                    var weightedSprites = m_WeightedList.GetArrayElementAtIndex(i);
                    var spritesList = weightedSprites.FindPropertyRelative("m_Sprites");
                    if (spritesList.arraySize == 0)
                        spritesList.arraySize = 1;
                }
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        /// <summary>
        /// Draws an Inspector for the Terrain Tile.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(tile, $"Update {tile.name}");

            EditorGUILayout.LabelField("Place sprites shown based on the contents of the sprite.");
            EditorGUILayout.Space();

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();
            for (int idx = 0; idx < 15; ++idx)
            {
                var weightedSprites = m_WeightedList.GetArrayElementAtIndex(idx);
                var spritesList = weightedSprites.FindPropertyRelative("m_Sprites");

                EditorGUILayout.LabelField(fieldNames[idx], EditorStyles.boldLabel);
                int count = EditorGUILayout.DelayedIntField("Number of Sprites", spritesList.arraySize);
                if (count < 1) 
                    count = 1;

                if (spritesList.arraySize != count)
                {
                    spritesList.arraySize = count;
                }

                spritesList.isExpanded = true;
                EditorGUILayout.PropertyField(spritesList, true);
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(tile);
            }
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
#endif
}
