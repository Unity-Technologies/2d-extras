using System;
using System.Collections.Generic;

namespace UnityEngine.Tilemaps
{
    [CreateAssetMenu]
    public class AutoTile : TileBase
    {
        [Serializable]
        public abstract class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
        {
            [SerializeField, HideInInspector]
            private List<TKey> keyData = new List<TKey>();
	
            [SerializeField, HideInInspector]
            private List<TValue> valueData = new List<TValue>();

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                Clear();
                for (int i = 0; i < keyData.Count && i < valueData.Count; i++)
                {
                    this[keyData[i]] = valueData[i];
                }
            }

            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                keyData.Clear();
                valueData.Clear();

                foreach (var item in this)
                {
                    keyData.Add(item.Key);
                    valueData.Add(item.Value);
                }
            }
        }

        [Serializable]
        public class AutoTileData
        {
            [SerializeField] public int test = 99;
            [SerializeField]
            public List<Sprite> spriteList = new List<Sprite>();
        }
        
        [Serializable]
        public class AutoTileDictionary : SerializedDictionary<uint, AutoTileData>
        {
        };

        #region Tile Data
        /// <summary>
        /// The Default Sprite set when creating a new Rule.
        /// </summary>
        public Sprite m_DefaultSprite;
        /// <summary>
        /// The Default GameObject set when creating a new Rule.
        /// </summary>
        public GameObject m_DefaultGameObject;
        /// <summary>
        /// The Default Collider Type set when creating a new Rule.
        /// </summary>
        public Tile.ColliderType m_DefaultColliderType = Tile.ColliderType.Sprite;
        [SerializeField]
        public AutoTileDictionary m_AutoTileDictionary = new AutoTileDictionary();
        #endregion

        #region Editor Data
        
        public List<Texture2D> m_TextureList;
        #endregion
        
        #region Runtime Data
        private readonly TileBase[] m_CachedTiles = new TileBase[9];
        #endregion

        /// <summary>
        /// This method is called when the tile is refreshed.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            for (var y = -1; y <= 1; ++y)
            {
                for (var x = -1; x <= 1; ++x)
                {
                    tilemap.RefreshTile(new Vector3Int(position.x + x, position.y + y, position.z));
                }   
            }
        }
        
        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int position, ITilemap itilemap, ref TileData tileData)
        {
            var iden = Matrix4x4.identity;

            tileData.sprite = m_DefaultSprite;
            tileData.gameObject = m_DefaultGameObject;
            tileData.colliderType = m_DefaultColliderType;
            tileData.flags = TileFlags.LockTransform;
            tileData.transform = iden;
            
            // Use Tilemap.GetTileBlockNonAlloc
            // var tilemap = itilemap.GetComponent<Tilemap>();
            // tilemap.GetTilesBlockNonAlloc(new BoundsInt(position.x - 1, position.y - 1, position.z, 3, 3, 1), m_CachedTiles);
            uint mask = 0;
            var index = 0;
            for (var y = -1; y <= 1; ++y)
            {
                for (var x = -1; x <= 1; ++x)
                {
                    var tilePosition = new Vector3Int(position.x + x, position.y + y, position.z);
                    m_CachedTiles[index] = itilemap.GetTile(tilePosition);
                    if (m_CachedTiles[index] == this) 
                        mask |= (uint) 1 << index;
                    index++;
                }   
            }
            // 64 128 256
            //  8  16  32
            //  1   2   4
            switch (mask)
            {
                // Left
                case 1 + 16 + 32:
                case 4 + 16 + 32:
                case 16 + 32 + 64:
                case 16 + 32 + 256:
                case 1 + 16 + 32 + 64:
                case 4 + 16 + 32 + 256:
                case 1 + 4 + 16 + 32:
                case 1 + 16 + 32 + 256:
                case 4 + 16 + 32 + 64:
                case 16 + 32 + 64 + 256:
                case 1 + 16 + 32 + 64 + 256:    
                case 1 + 4 + 16 + 32 + 64:
                case 1 + 4 + 16 + 32 + 256:
                case 4 + 16 + 32 + 64 + 256:
                case 1 + 4 + 16 + 32 + 64 + 256:
                {
                    mask = 16 + 32;
                    break;
                }
                // Right
                case 1 + 8 + 16:
                case 8 + 16 + 64:
                case 1 + 8 + 16 + 64:
                case 4 + 8 + 16:
                case 8 + 16 + 256:
                case 1 + 4 + 8 + 16:
                case 1 + 8 + 16 + 256:
                case 4 + 8 + 16 + 64:
                case 4 + 8 + 16 + 256:
                case 8 + 16 + 64 + 256:
                case 1 + 4 + 8 + 16 + 64:
                case 1 + 4 + 8 + 16 + 256:
                case 1 + 8 + 16 + 64 + 256:
                case 4 + 8 + 16 + 64 + 256:
                case 1 + 4 + 8 + 16 + 64 + 256:
                {
                    mask = 8 + 16;
                    break;
                }
                // Top
                case 1 + 2 + 16:
                case 2 + 4 + 16:
                case 1 + 2 + 4 + 16: 
                case 2 + 16 + 64:
                case 2 + 16 + 256:
                case 2 + 16 + 64 + 256:
                case 1 + 2 + 16 + 64:
                case 1 + 2 + 16 + 256:
                case 1 + 2 + 16 + 64 + 256:
                case 2 + 4 + 16 + 64:
                case 2 + 4 + 16 + 256:
                case 2 + 4 + 16 + 64 + 256:
                case 1 + 2 + 4 + 16 + 64:
                case 1 + 2 + 4 + 16 + 256:
                case 1 + 2 + 4 + 16 + 64 + 256:
                {
                    mask = 2 + 16;
                    break;
                }
                // Bottom
                case 16 + 64 + 128:
                case 16 + 128 + 256:
                case 16 + 64 + 128 + 256:
                case 1 + 16 + 64 + 128:
                case 4 + 16 + 64 + 128:
                case 1 + 4 + 16 + 64 + 128:
                case 1 + 16 + 128 + 256:
                case 4 + 16 + 128 + 256:
                case 1 + 4 + 16 + 128 + 256:
                case 1 + 16 + 64 + 128 + 256:
                case 4 + 16 + 64 + 128 + 256:
                case 1 + 4 + 16 + 64 + 128 + 256:
                case 1 + 16 + 128:
                case 4 + 16 + 128:
                case 1 + 4 + 16 + 128:
                {
                    mask = 16 + 128;
                    break;
                }
                // Vertical Straight
                case 1 + 2 + 16 + 128:
                case 2 + 4 + 16 + 128:
                case 1 + 2 + 4 + 16 + 128:
                case 2 + 16 + 64 + 128:
                case 2 + 16 + 128 + 256:
                case 2 + 16 + 64 + 128 + 256:
                case 1 + 2 + 16 + 64 + 128:
                case 1 + 2 + 16 + 128 + 256:
                case 2 + 4 + 16 + 64 + 128:
                case 2 + 4 + 16 + 128 + 256:
                case 1 + 2 + 16 + 64 + 128 + 256:    
                case 1 + 2 + 4 + 64 + 128 + 256:
                case 1 + 2 + 4 + 16 + 128 + 256:
                case 1 + 2 + 4 + 16 + 64 + 128:
                case 2 + 4 + 16 + 64 + 128 + 256:
                case 1 + 2 + 4 + 16 + 64 + 128 + 256:
                {
                    mask = 2 + 16 + 128;
                    break;           
                }
                // Horizontal Straight
                case 1 + 8 + 16 + 32:
                case 8 + 16 + 32 + 64:
                case 1 + 8 + 16 + 32 + 64:
                case 4 + 8 + 16 + 32:
                case 8 + 16 + 32 + 256:
                case 4 + 8 + 16 + 32 + 64:
                case 4 + 8 + 16 + 32 + 256:
                case 1 + 4 + 8 + 16 + 32:    
                case 1 + 8 + 16 + 32 + 256:
                case 8 + 16 + 32 + 64 + 256:    
                case 1 + 4 + 8 + 16 + 32 + 64:
                case 1 + 4 + 8 + 16 + 32 + 256:
                case 1 + 8 + 16 + 32 + 64 + 256:    
                case 4 + 8 + 16 + 32 + 64 + 256:    
                case 1 + 4 + 8 + 16 + 32 + 64 + 256:  
                {
                    mask = 8 + 16 + 32;
                    break;           
                }
                // Top Left Corner
                case 1 + 2 + 4 + 16 + 32:
                case 2 + 4 + 16 + 32 + 256: 
                case 2 + 4 + 16 + 32 + 64:
                case 1 + 2 + 4 + 16 + 32 + 256:
                case 1 + 2 + 4 + 16 + 32 + 64:
                case 2 + 4 + 16 + 32 + 64 + 256:
                case 1 + 2 + 4 + 16 + 32 + 64 + 256:
                {
                    mask = 2 + 4 + 16 + 32;
                    break;           
                }
                // Bottom Left Corner
                case 1 + 16 + 32 + 128 + 256:
                case 4 + 16 + 32 + 128 + 256:
                case 16 + 32 + 64 + 128 + 256:
                case 4 + 16 + 32 + 64 + 128 + 256: 
                case 1 + 4 + 16 + 32 + 128 + 256:
                case 1 + 16 + 32 + 64 + 128 + 256:
                case 1 + 4 + 16 + 32 + 64 + 128 + 256:
                {
                    mask = 16 + 32 + 128 + 256;
                    break;
                }
                // Top Right Corner
                case 1 + 2 + 4 + 8 + 16:
                case 1 + 2 + 8 + 16 + 64:
                case 1 + 2 + 8 + 16 + 256:
                case 1 + 2 + 4 + 8 + 16 + 64:
                case 1 + 2 + 8 + 16 + 64 + 256:
                case 1 + 2 + 4 + 8 + 16 + 256:
                case 1 + 2 + 4 + 8 + 16 + 64 + 256:
                {
                    mask = 1 + 2 + 8 + 16;
                    break;           
                }
                // Bottom Right Corner
                case 1 + 8 + 16 + 64 + 128:
                case 8 + 16 + 64 + 128 + 256:
                case 4 + 8 + 16 + 64 + 128:
                case 1 + 4 + 8 + 16 + 64 + 128:
                case 1 + 8 + 16 + 64 + 128 + 256:
                case 4 + 8 + 16 + 64 + 128 + 256:    
                case 1 + 4 + 8 + 16 + 64 + 128 + 256:
                {
                    mask = 8 + 16 + 64 + 128;
                    break;           
                }
                // Full Top
                case 1 + 2 + 4 + 8 + 16 + 32 + 64:
                case 1 + 2 + 4 + 8 + 16 + 32 + 256:
                case 1 + 2 + 4 + 8 + 16 + 32 + 64 + 256:
                {
                    mask = 1 + 2 + 4 + 8 + 16 + 32;
                    break;
                }
                // Full Bottom
                case 1 + 8 + 16 + 32 + 64 + 128 + 256:
                case 4 + 8 + 16 + 32 + 64 + 128 + 256:
                case 1 + 4 + 8 + 16 + 32 + 64 + 128 + 256:
                {
                    mask = 8 + 16 + 32 + 64 + 128 + 256;
                    break;
                }
                // Full Left
                case 1 + 2 + 4 + 16 + 32 + 128 + 256:
                case 2 + 4 + 16 + 32 + 64 + 128 + 256:
                case 1 + 2 + 4 + 16 + 32 + 64 + 128 + 256:
                {
                    mask = 2 + 4 + 16 + 32 + 128 + 256;
                    break;
                }
                // Full Right
                case 1 + 2 + 4 + 8 + 16 + 64 + 128:
                case 1 + 2 + 8 + 16 + 64 + 128 + 256:
                case 1 + 2 + 4 + 8 + 16 + 64 + 128 + 256:
                {
                    mask = 1 + 2 + 8 + 16 + 64 + 128;
                    break;
                }
                // Top Left Tricorner
                case 1 + 2 + 16 + 32:
                case 2 + 16 + 32 + 64:
                case 2 + 16 + 32 + 256:
                case 1 + 2 + 16 + 32 + 64:
                case 1 + 2 + 16 + 32 + 256:
                case 2 + 16 + 32 + 64 + 256:
                case 1 + 2 + 16 + 32 + 64 + 256:
                {
                    mask = 2 + 16 + 32;
                    break;
                }
                // Bottom Left Tricorner
                case 1 + 16 + 32 + 128:
                case 4 + 16 + 32 + 128:
                case 16 + 32 + 64 + 128:
                case 4 + 16 + 32 + 64 + 128:
                case 1 + 16 + 32 + 64 + 128:
                case 1 + 4 + 16 + 32 + 64 + 128:
                case 1 + 4 + 16 + 32 + 128:
                {
                    mask = 16 + 32 + 128;
                    break;
                }
                // Top Right Tricorner
                case 2 + 4 + 8 + 16:
                case 2 + 8 + 16 + 64:
                case 2 + 8 + 16 + 256:
                case 2 + 4 + 8 + 16 + 64:
                case 2 + 8 + 16 + 64 + 256:
                case 2 + 4 + 8 + 16 + 256:
                case 2 + 4 + 8 + 16 + 64 + 256:
                {
                    mask = 2 + 8 + 16;
                    break;
                }
                // Bottom Right Tricorner
                case 1 + 8 + 16 + 128:
                case 4 + 8 + 16 + 128:
                case 8 + 16 + 128 + 256:
                case 1 + 8 + 16 + 128 + 256:
                case 1 + 4 + 8 + 16 + 128:    
                case 4 + 8 + 16 + 128 + 256:
                case 1 + 4 + 8 + 16 + 128 + 256:
                {
                    mask = 8 + 16 + 128;
                    break;
                }
                // Three-way Left
                case 2 + 4 + 8 + 16 + 128:
                case 2 + 8 + 16 + 128 + 256:
                case 2 + 4 + 8 + 16 + 128 + 256:
                {
                    mask = 2 + 8 + 16 + 128;
                    break;
                }
                // Three-way Right
                case 1 + 2 + 16 + 32 + 128:
                case 2 + 16 + 32 + 64 + 128:
                case 1 + 2 + 16 + 32 + 64 + 128:
                {
                    mask = 2 + 16 + 32 + 128;
                    break;
                }
                // Three-way Top
                case 1 + 8 + 16 + 32 + 128:
                case 4 + 8 + 16 + 32 + 128:
                case 1 + 4 + 8 + 16 + 32 + 128:
                {
                    mask = 8 + 16 + 32 + 128;
                    break;
                }
                // Three-way Bottom
                case 2 + 8 + 16 + 32 + 64:
                case 2 + 8 + 16 + 32 + 256:
                case 2 + 8 + 16 + 32 + 64 + 256:
                {
                    mask = 2 + 8 + 16 + 32;
                    break;
                }
                // Three-corner Top Left
                case 2 + 4 + 8 + 16 + 32 + 64:
                case 2 + 4 + 8 + 16 + 32 + 256:
                case 2 + 4 + 8 + 16 + 32 + 64 + 256:
                {
                    mask = 2 + 4 + 8 + 16 + 32;
                    break;
                }
                // Three-corner Bottom Left
                case 1 + 8 + 16 + 32 + 128 + 256:
                case 4 + 8 + 16 + 32 + 128 + 256:
                case 1 + 4 + 8 + 16 + 32 + 128 + 256:
                {
                    mask = 8 + 16 + 32 + 128 + 256;
                    break;
                }
                // Three-corner Top Right
                case 1 + 2 + 8 + 16 + 32 + 64:
                case 1 + 2 + 8 + 16 + 32 + 256:
                case 1 + 2 + 8 + 16 + 32 + 64 + 256:
                {
                    mask = 1 + 2 + 8 + 16 + 32;
                    break;
                }
                // Three-corner Bottom Right
                case 1 + 8 + 16 + 32 + 64 + 128:
                case 4 + 8 + 16 + 32 + 64 + 128:
                case 1 + 4 + 8 + 16 + 32 + 64 + 128:
                {
                    mask = 8 + 16 + 32 + 64 + 128;
                    break;
                }
                // Left Side Top Right Corner
                case 1 + 2 + 4 + 16 + 32 + 128:
                case 2 + 4 + 16 + 32 + 64 + 128:
                case 1 + 2 + 4 + 16 + 32 + 64 + 128:
                {
                    mask = 2 + 4 + 16 + 32 + 128;
                    break;
                }
                // Left Side Bottom Right Corner
                case 1 + 2 + 16 + 32 + 128 + 256:
                case 2 + 16 + 32 + 64 + 128 + 256:
                case 1 + 2 + 16 + 32 + 64 + 128 + 256:
                {
                    mask = 2 + 16 + 32 + 128 + 256;
                    break;
                }
                // Right Side Top Left Corner
                case 1 + 2 + 4 + 8 + 16 + 128:
                case 1 + 2 + 8 + 16 + 128 + 256:
                case 1 + 2 + 4 + 8 + 16 + 128 + 256:
                {
                    mask = 1 + 2 + 8 + 16 + 128;
                    break;
                }
                // Right Side Bottom Left Corner
                case 2 + 4 + 8 + 16 + 64 + 128:
                case 2 + 8 + 16 + 64 + 128 + 256:
                case 2 + 4 + 8 + 16 + 64 + 128 + 256:
                {
                    mask = 2 + 8 + 16 + 64 + 128;
                    break;
                }
                // Single
                case 1 + 16:
                case 4 + 16:
                case 16 + 64:
                case 16 + 256:
                case 1 + 4 + 16:
                case 1 + 16 + 64:
                case 1 + 16 + 256:
                case 4 + 16 + 64:
                case 4 + 16 + 256:
                case 16 + 64 + 256:
                case 1 + 4 + 16 + 64:
                case 1 + 4 + 16 + 256:
                case 1 + 16 + 64 + 256:
                case 4 + 16 + 64 + 256:
                case 1 + 4 + 16 + 64 + 256:
                {
                    mask = 16;
                    break;
                }
            }
            
            if (m_AutoTileDictionary.TryGetValue(mask, out var autoTileData))
            {
                tileData.sprite = autoTileData.spriteList.Count > 0 ? autoTileData.spriteList[0] : m_DefaultSprite;
            }
        }

        public void AddSprite(Sprite sprite, uint mask)
        {
            if (!m_AutoTileDictionary.TryGetValue(mask, out var autoTileData))
            {
                autoTileData = new AutoTileData();
                m_AutoTileDictionary.Add(mask, autoTileData);
            }
            var isInList = false;
            foreach (var spriteData in autoTileData.spriteList)
            {
                isInList = spriteData == sprite;
                if (isInList)
                    break;
            }
            if (!isInList)
                autoTileData.spriteList.Add(sprite);
        }

        public void RemoveSprite(Sprite sprite, uint mask)
        {
            if (!m_AutoTileDictionary.TryGetValue(mask, out var autoTileData))
                return;

            autoTileData.spriteList.Remove(sprite);
        }

        public void Validate()
        {
            foreach (var pair in m_AutoTileDictionary)
            {
                var autoTileData = pair.Value;
                for (var i = 0; i < autoTileData.spriteList.Count;)
                {
                    var sprite = autoTileData.spriteList[i];
                    if (m_TextureList.Contains(sprite.texture))
                    {
                        ++i;
                    }
                    else
                    {
                        autoTileData.spriteList.RemoveAt(i);
                    }
                }
            }
        }
    }
}