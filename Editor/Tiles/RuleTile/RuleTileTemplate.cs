using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Template used to create a RuleTile from Texture2D and Sprites.
    /// </summary>
    [HelpURL(
        "https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest/index.html?subfolder=/manual/RuleTile.html")]
    public class RuleTileTemplate : TileTemplate
    {
        internal static string kExtension = "asset"; 
        
        /// <summary>
        /// Positional and Rule Data for detecting RuleTile Sprites.
        /// </summary>
        [Serializable]
        public struct RuleData
        {
            /// <summary>
            /// x, y positions on Texture2D.
            /// </summary>
            public List<Vector2> spritePositions;
            /// <summary>
            /// Tiling Rule for Rule at position.
            /// </summary>
            public RuleTile.TilingRule tilingRule;
        }
        /// <summary>
        /// List of Positional Data for detecting Sprites and Tiling Rules.
        /// </summary>
        public List<RuleData> rules;
        
        /// <summary>
        /// Original Width of the Template
        /// </summary>
        public int textureWidth;
        /// <summary>
        /// Original Height of the Template
        /// </summary>
        public int textureHeight;

        /// <summary>
        /// The Default Sprite set when creating a new Rule.
        /// </summary>
        public Sprite defaultSprite;

        /// <summary>
        /// The Default GameObject set when creating a new Rule.
        /// </summary>
        public GameObject defaultGameObject;

        /// <summary>
        /// The Default Collider Type set when creating a new Rule.
        /// </summary>
        public Tile.ColliderType defaultColliderType = Tile.ColliderType.Sprite;
        
        /// <summary>
        /// Creates a List of TileBase Assets with a RuleTile from Texture2D
        /// and Sprites with placement data onto a Tile Palette.
        /// </summary>
        /// <param name="texture2D">Texture2D to generate Tile Assets from.</param>
        /// <param name="sprites">Sprites to generate Tile Assets from.</param>
        /// <param name="tilesToAdd">RuleTile asset and placement data to generate.</param>
        public override void CreateTileAssets(
            Texture2D texture2D
            , IEnumerable<Sprite> sprites
            , ref List<TileChangeData> tilesToAdd)
        {
            if (texture2D == null)
                return;
            
            var ruleTile = ScriptableObject.CreateInstance<RuleTile>();
            ruleTile.name = $"{texture2D.name} RuleTile";
            
            this.ApplyTemplateToRuleTile(texture2D, ruleTile);
            
            var tileChangeData = new TileChangeData(
                Vector3Int.zero
                , ruleTile
                , Color.white
                , Matrix4x4.identity
            );
            tilesToAdd.Add(tileChangeData);
        }
    }
}