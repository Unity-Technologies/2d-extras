using System;
using System.Collections.Generic;
using UnityEditor.Tilemaps;

namespace UnityEngine.Tilemaps
{
    public class AutoTileTemplate : TileTemplate
    {
        public static string kExtension = "asset"; 
        
        [Serializable]
        public struct SpriteData
        {
            public float x;
            public float y;
            public uint mask;
        }
        
        public int width;
        public int height;
        public AutoTile.AutoTileMaskType maskType;
        public List<SpriteData> sprites;
        
        public override void CreateTileAssets(
            Texture2D texture2D
            , IEnumerable<Sprite> sprites
            , ref List<TileChangeData> tilesToAdd)
        {
            if (texture2D == null)
                return;
            
            var autoTile = ScriptableObject.CreateInstance<AutoTile>();
            autoTile.name = $"{texture2D.name} AutoTile";
            this.ApplyTemplateToAutoTile(texture2D, autoTile);
            var tileChangeData = new TileChangeData(
                Vector3Int.zero
                , autoTile
                , Color.white
                , Matrix4x4.identity
            );
            tilesToAdd.Add(tileChangeData);
        }
    }
}