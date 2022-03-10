using System;
using System.Collections.Generic;

namespace UnityEngine.Tilemaps
{
    public class AutoTileTextureTemplate : ScriptableObject
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
        public List<SpriteData> sprites;
    }
}