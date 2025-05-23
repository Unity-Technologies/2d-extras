using System;
using System.Collections.Generic;
using UnityEditor.Tilemaps;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// Template used to create an AutoTile from Texture2D and Sprites.
    /// </summary>
    [HelpURL(
        "https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest/index.html?subfolder=/manual/AutoTile.html")]
    public class AutoTileTemplate : TileTemplate
    {
        internal static string kExtension = "asset";

        /// <summary>
        /// Positional Data for detecting AutoTile Sprites
        /// </summary>
        [Serializable]
        public struct SpriteData
        {
            /// <summary>
            /// x position on Texture2D.
            /// </summary>
            public float x;

            /// <summary>
            /// y position on Texture2D.
            /// </summary>
            public float y;

            /// <summary>
            /// Mask Rule for Sprite at position.
            /// </summary>
            public uint mask;
        }

        /// <summary>
        /// Original Width of the Template
        /// </summary>
        public int width;

        /// <summary>
        /// Original Height of the Template
        /// </summary>
        public int height;

        /// <summary>
        /// Mask Type for generated AutoTIle
        /// </summary>
        public AutoTile.AutoTileMaskType maskType;

        /// <summary>
        /// Positional Data for detecting AutoTile Sprites 
        /// </summary>
        public List<SpriteData> sprites;

        /// <summary>
        /// Creates a List of TileBase Assets with an AutoTile from Texture2D
        /// and Sprites with placement data onto a Tile Palette.
        /// </summary>
        /// <param name="texture2D">Texture2D to generate Tile Assets from.</param>
        /// <param name="sprites">Sprites to generate Tile Assets from.</param>
        /// <param name="tilesToAdd">AutoTile asset and placement data to generate.</param>
        public override void CreateTileAssets(
            Texture2D texture2D
            , IEnumerable<Sprite> sprites
            , ref List<TileChangeData> tilesToAdd)
        {
            if (texture2D == null)
                return;

            var autoTile = ScriptableObject.CreateInstance<AutoTile>();
            autoTile.name = $"{texture2D.name} AutoTile";
            this.ApplyTemplateToAutoTile(texture2D, sprites, autoTile);
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
