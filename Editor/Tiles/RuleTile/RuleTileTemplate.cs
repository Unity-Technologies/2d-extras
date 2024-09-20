using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    public class RuleTileTemplate : TileTemplate
    {
        public static string kExtension = "asset"; 
        
        [Serializable]
        public struct RuleData
        {
            public List<Vector2> spritePositions;
            public RuleTile.TilingRule tilingRule;
        }
        public List<RuleData> rules;
        public int textureWidth;
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