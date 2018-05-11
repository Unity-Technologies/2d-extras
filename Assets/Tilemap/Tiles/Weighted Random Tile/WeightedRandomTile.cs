using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps {
    [Serializable]
    public struct WeightedSprite {
        public Sprite Sprite;
        public int Weight;
    }

    [Serializable]
    public class WeightedRandomTile : Tile {
        [SerializeField] public WeightedSprite[] Sprites;

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData) {
            base.GetTileData(location, tileMap, ref tileData);
            
            if (Sprites == null || Sprites.Length <= 0) return;
            
            long hash = location.x;
            hash = hash + 0xabcd1234 + (hash << 15);
            hash = hash + 0x0987efab ^ (hash >> 11);
            hash ^= location.y;
            hash = hash + 0x46ac12fd + (hash << 7);
            hash = hash + 0xbe9730af ^ (hash << 11);
            Random.InitState((int) hash);

            // Get the cumulative weight of the sprites
            var cumulativeWeight = 0;
            foreach (var spriteInfo in Sprites) cumulativeWeight += spriteInfo.Weight;

            // Pick a random weight and choose a sprite depending on it
            var randomWeight = Random.Range(0, cumulativeWeight);
            foreach (var spriteInfo in Sprites) {
                randomWeight -= spriteInfo.Weight;
                if (randomWeight < 0) {
                    tileData.sprite = spriteInfo.Sprite;    
                    break;
                }
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Weighted Random Tile")]
        public static void CreateRandomTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save Weighted Random Tile", "New Weighted Random Tile", "asset",
                "Save Weighted Random Tile", "Assets");

            if (path == "") return;

            AssetDatabase.CreateAsset(CreateInstance<WeightedRandomTile>(), path);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(WeightedRandomTile))]
    public class WeightedRandomTileEditor : Editor {
        private WeightedRandomTile Tile {
            get { return target as WeightedRandomTile; }
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            int count = EditorGUILayout.DelayedIntField("Number of Sprites", Tile.Sprites != null ? Tile.Sprites.Length : 0);
            if (count < 0) count = 0;
            
            if (Tile.Sprites == null || Tile.Sprites.Length != count) {
                Array.Resize(ref Tile.Sprites, count);
            }

            if (count == 0) return;

            EditorGUILayout.LabelField("Place random sprites.");
            EditorGUILayout.Space();

            for (int i = 0; i < count; i++) {
                Tile.Sprites[i].Sprite = (Sprite) EditorGUILayout.ObjectField("Sprite " + (i + 1), Tile.Sprites[i].Sprite, typeof(Sprite), false, null);
                Tile.Sprites[i].Weight = EditorGUILayout.IntField("Weight " + (i + 1), Tile.Sprites[i].Weight);
            }

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(Tile);
        }
    }
#endif
}
