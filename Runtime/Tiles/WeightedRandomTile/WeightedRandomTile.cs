using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps 
{
    /// <summary>
    /// A Sprite with a Weight value for randomization.
    /// </summary>
    [Serializable]
    public struct WeightedSprite 
    {
        /// <summary>
        /// Sprite.
        /// </summary>
        public Sprite Sprite;
        /// <summary>
        /// Weight of the Sprite.
        /// </summary>
        public int Weight;
    }

    /// <summary>
    /// Weighted Random Tiles are tiles which randomly pick a sprite from a given list of sprites and a target location, and displays that sprite.
    /// The sprites can be weighted with a value to change its probability of appearing. The Sprite displayed for the Tile is randomized based on its location and will be fixed for that particular location.
    /// </summary>
    [Serializable]
    public class WeightedRandomTile : Tile 
    {
        /// <summary>
        /// The Sprites used for randomizing output.
        /// </summary>
        [SerializeField] public WeightedSprite[] Sprites;

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) 
        {
            base.GetTileData(position, tilemap, ref tileData);
            
            if (Sprites == null || Sprites.Length <= 0) return;
            
            var oldState = Random.state;
            long hash = position.x;
            hash = hash + 0xabcd1234 + (hash << 15);
            hash = hash + 0x0987efab ^ (hash >> 11);
            hash ^= position.y;
            hash = hash + 0x46ac12fd + (hash << 7);
            hash = hash + 0xbe9730af ^ (hash << 11);
            Random.InitState((int) hash);

            // Get the cumulative weight of the sprites
            var cumulativeWeight = 0;
            foreach (var spriteInfo in Sprites) cumulativeWeight += spriteInfo.Weight;

            // Pick a random weight and choose a sprite depending on it
            var randomWeight = Random.Range(0, cumulativeWeight);
            foreach (var spriteInfo in Sprites) 
            {
                randomWeight -= spriteInfo.Weight;
                if (randomWeight < 0) 
                {
                    tileData.sprite = spriteInfo.Sprite;    
                    break;
                }
            }
            Random.state = oldState;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(WeightedRandomTile))]
    public class WeightedRandomTileEditor : Editor 
    {
        private SerializedProperty m_Color;
        private SerializedProperty m_ColliderType;

        private WeightedRandomTile Tile {
            get { return target as WeightedRandomTile; }
        }

        /// <summary>
        /// OnEnable for WeightedRandomTile.
        /// </summary>
        public void OnEnable()
        {
            m_Color = serializedObject.FindProperty("m_Color");
            m_ColliderType = serializedObject.FindProperty("m_ColliderType");
        }

        /// <summary>
        /// Draws an Inspector for the WeightedRandomTile.
        /// </summary>
        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            int count = EditorGUILayout.DelayedIntField("Number of Sprites", Tile.Sprites != null ? Tile.Sprites.Length : 0);
            if (count < 0) 
                count = 0;

            if (Tile.Sprites == null || Tile.Sprites.Length != count) 
            {
                Array.Resize(ref Tile.Sprites, count);
            }

            if (count == 0) 
                return;

            EditorGUILayout.LabelField("Place random sprites.");
            EditorGUILayout.Space();

            for (int i = 0; i < count; i++) 
            {
                Tile.Sprites[i].Sprite = (Sprite) EditorGUILayout.ObjectField("Sprite " + (i + 1), Tile.Sprites[i].Sprite, typeof(Sprite), false, null);
                Tile.Sprites[i].Weight = EditorGUILayout.IntField("Weight " + (i + 1), Tile.Sprites[i].Weight);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.PropertyField(m_ColliderType);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Tile);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
