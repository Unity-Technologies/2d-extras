using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// Rule Override Tiles are Tiles which can override a subset of Rules for a given Rule Tile to provide specialised behaviour while keeping most of the Rules originally set in the Rule Tile.
    /// </summary>
    [MovedFrom(true, "UnityEngine")]
    [Serializable]
    [CreateAssetMenu(fileName = "New Rule Override Tile", menuName = "Tiles/Rule Override Tile")]
    public class RuleOverrideTile : TileBase
    {
        /// <summary>
        /// A data structure storing the Sprite overriding the original RuleTile Sprite
        /// </summary>
        [Serializable]
        public class TileSpritePair
        {
            public Sprite m_OriginalSprite;
            public Sprite m_OverrideSprite;
        }

        /// <summary>
        /// A data structure storing the overriding Tiling Rule and its status  
        /// </summary>
        [Serializable]
        public class OverrideTilingRule
        {
            public bool m_Enabled;
            public RuleTile.TilingRule m_TilingRule = new RuleTile.TilingRule();
        }

        /// <summary>
        /// Gets the overriding Sprite of a given Sprite. 
        /// </summary>
        /// <param name="originalSprite">The original Sprite that is overridden</param>
        public Sprite this[Sprite originalSprite]
        {
            get
            {
                foreach (TileSpritePair spritePair in m_Sprites)
                {
                    if (spritePair.m_OriginalSprite == originalSprite)
                    {
                        return spritePair.m_OverrideSprite;
                    }
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    m_Sprites = m_Sprites.Where(spritePair => spritePair.m_OriginalSprite != originalSprite).ToList();
                }
                else
                {
                    foreach (TileSpritePair spritePair in m_Sprites)
                    {
                        if (spritePair.m_OriginalSprite == originalSprite)
                        {
                            spritePair.m_OverrideSprite = value;
                            return;
                        }
                    }
                    m_Sprites.Add(new TileSpritePair()
                    {
                        m_OriginalSprite = originalSprite,
                        m_OverrideSprite = value,
                    });
                }
            }
        }

        /// <summary>
        /// Gets the overriding Tiling Rule of a given Tiling Rule.
        /// </summary>
        /// <param name="originalRule">The original Tiling Rule that is overridden</param>
        public RuleTile.TilingRule this[RuleTile.TilingRule originalRule]
        {
            get
            {
                if (!m_Tile)
                    return null;

                int index = m_Tile.m_TilingRules.IndexOf(originalRule);
                if (index == -1)
                    return null;
                if (m_OverrideTilingRules.Count < index + 1)
                    return null;

                return m_OverrideTilingRules[index].m_Enabled ? m_OverrideTilingRules[index].m_TilingRule : null;
            }
            set
            {
                if (!m_Tile)
                    return;

                int index = m_Tile.m_TilingRules.IndexOf(originalRule);
                if (index == -1)
                    return;

                if (value == null)
                {
                    if (m_OverrideTilingRules.Count < index + 1)
                        return;
                    m_OverrideTilingRules[index].m_Enabled = false;
                    while (m_OverrideTilingRules.Count > 0 && !m_OverrideTilingRules[m_OverrideTilingRules.Count - 1].m_Enabled)
                        m_OverrideTilingRules.RemoveAt(m_OverrideTilingRules.Count - 1);
                }
                else
                {
                    while (m_OverrideTilingRules.Count < index + 1)
                        m_OverrideTilingRules.Add(new OverrideTilingRule());
                    m_OverrideTilingRules[index].m_Enabled = true;
                    m_OverrideTilingRules[index].m_TilingRule = CopyTilingRule(value, new RuleTile.TilingRule(), true);
                    m_OverrideTilingRules[index].m_TilingRule.m_Neighbors = null;
                }
            }
        }

        /// <summary>
        /// The RuleTile to override
        /// </summary>
        public RuleTile m_Tile;
        /// <summary>
        /// Enable Advanced Mode. Enable this if you want to specify which Rules to override.
        /// </summary>
        public bool m_Advanced;
        /// <summary>
        /// A list of Sprite Overrides
        /// </summary>
        public List<TileSpritePair> m_Sprites = new List<TileSpritePair>();
        /// <summary>
        /// A list of Tiling Rule Overrides
        /// </summary>
        public List<OverrideTilingRule> m_OverrideTilingRules = new List<OverrideTilingRule>();
        /// <summary>
        /// The default overriding Tiling Rule
        /// </summary>
        public OverrideTilingRule m_OverrideDefault = new OverrideTilingRule();
        /// <summary>
        /// The default original Tiling Rule
        /// </summary>
        public RuleTile.TilingRule m_OriginalDefault
        {
            get
            {
                return new RuleTile.TilingRule()
                {
                    m_Sprites = new Sprite[] { m_Tile != null ? m_Tile.m_DefaultSprite : null },
                    m_ColliderType = m_Tile != null ? m_Tile.m_DefaultColliderType : Tile.ColliderType.None,
                };
            }
        }

        /// <summary>
        /// Returns the Rule Tile for retrieving TileData
        /// </summary>
        [HideInInspector] public RuleTile m_InstanceTile;

        /// <summary>
        /// Retrieves any tile animation data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileAnimationData">Data to run an animation on the tile.</param>
        /// <returns>Whether the call was successful.</returns>
        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            if (!m_InstanceTile)
                return false;
            return m_InstanceTile.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        }

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            if (!m_InstanceTile)
                return;
            m_InstanceTile.GetTileData(position, tilemap, ref tileData);
        }

        /// <summary>
        /// This method is called when the tile is refreshed.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tileMap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            if (!m_InstanceTile)
                return;
            m_InstanceTile.RefreshTile(position, tilemap);
        }

        /// <summary>
        /// StartUp is called on the first frame of the running Scene.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="instantiateedGameObject">The GameObject instantiated for the Tile.</param>
        /// <returns>Whether StartUp was successful</returns>
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            if (!m_InstanceTile)
                return true;
            return m_InstanceTile.StartUp(position, tilemap, go);
        }

        /// <summary>
        /// Applies Sprite overrides to this
        /// </summary>
        /// <param name="overrides">A list of Sprite overrides to apply</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void ApplyOverrides(IList<KeyValuePair<Sprite, Sprite>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

        /// <summary>
        /// Gets Sprite overrides for this
        /// </summary>
        /// <param name="overrides">A list of Sprite overrides to fill</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void GetOverrides(List<KeyValuePair<Sprite, Sprite>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            overrides.Clear();

            if (!m_Tile)
                return;

            List<Sprite> originalSprites = new List<Sprite>();

            if (m_Tile.m_DefaultSprite)
                originalSprites.Add(m_Tile.m_DefaultSprite);

            foreach (RuleTile.TilingRule rule in m_Tile.m_TilingRules)
                foreach (Sprite sprite in rule.m_Sprites)
                    if (sprite && !originalSprites.Contains(sprite))
                        originalSprites.Add(sprite);

            foreach (Sprite sprite in originalSprites)
                overrides.Add(new KeyValuePair<Sprite, Sprite>(sprite, this[sprite]));
        }

        /// <summary>
        /// Applies Tiling Rule overrides to this
        /// </summary>
        /// <param name="overrides">A list of Tiling Rule overrides to apply</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void ApplyOverrides(IList<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

        /// <summary>
        /// Gets Tiling Rule overrides for this
        /// </summary>
        /// <param name="overrides">A list of Tiling Rule overrides to fill</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void GetOverrides(List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            overrides.Clear();

            if (!m_Tile)
                return;

            foreach (var originalRule in m_Tile.m_TilingRules)
            {
                RuleTile.TilingRule overrideRule = this[originalRule];
                overrides.Add(new KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>(originalRule, overrideRule));
            }
            overrides.Add(new KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRule>(m_OriginalDefault, m_OverrideDefault.m_TilingRule));
        }

        public void Override()
        {
            if (!m_Tile || !m_InstanceTile)
                return;

            var tile = m_InstanceTile;

            tile.m_DefaultSprite = m_Tile.m_DefaultSprite;
            tile.m_DefaultGameObject = m_Tile.m_DefaultGameObject;
            tile.m_DefaultColliderType = m_Tile.m_DefaultColliderType;

            tile.m_TilingRules.Clear();
            foreach (var rule in m_Tile.m_TilingRules)
                tile.m_TilingRules.Add(CopyTilingRule(rule, new RuleTile.TilingRule(), true));

            if (!m_Advanced)
            {
                tile.m_DefaultSprite = this[m_Tile.m_DefaultSprite];

                foreach (RuleTile.TilingRule rule in tile.m_TilingRules)
                    for (int i = 0; i < rule.m_Sprites.Length; i++)
                        if (rule.m_Sprites[i])
                            rule.m_Sprites[i] = this[rule.m_Sprites[i]];
            }
            else
            {
                if (m_OverrideDefault.m_Enabled)
                {
                    tile.m_DefaultSprite = m_OverrideDefault.m_TilingRule.m_Sprites.Length > 0 ? m_OverrideDefault.m_TilingRule.m_Sprites[0] : null;
                    tile.m_DefaultGameObject = m_OverrideDefault.m_TilingRule.m_GameObject;
                    tile.m_DefaultColliderType = m_OverrideDefault.m_TilingRule.m_ColliderType;
                }
                for (int i = 0; i < tile.m_TilingRules.Count; i++)
                {
                    RuleTile.TilingRule originalRule = tile.m_TilingRules[i];
                    RuleTile.TilingRule overrideRule = this[m_Tile.m_TilingRules[i]];
                    if (overrideRule == null)
                        continue;
                    CopyTilingRule(overrideRule, originalRule, false);
                }
            }
        }

        /// <summary>
        /// Copies a Tiling Rule from a given Tiling Rule
        /// </summary>
        /// <param name="from">A Tiling Rule to copy from</param>
        /// <param name="to">A Tiling Rule to copy to</param>
        /// <param name="copyRule"></param>
        public RuleTile.TilingRule CopyTilingRule(RuleTile.TilingRule from, RuleTile.TilingRule to, bool copyRule)
        {
            if (from == null)
                return null;

            if (copyRule)
            {
                to.m_Neighbors = from.m_Neighbors;
                to.m_NeighborPositions = from.m_NeighborPositions;
                to.m_RuleTransform = from.m_RuleTransform;
            }
            to.m_Sprites = from.m_Sprites.Clone() as Sprite[];
            to.m_GameObject = from.m_GameObject;
            to.m_AnimationSpeed = from.m_AnimationSpeed;
            to.m_PerlinScale = from.m_PerlinScale;
            to.m_Output = from.m_Output;
            to.m_ColliderType = from.m_ColliderType;
            to.m_RandomTransform = from.m_RandomTransform;

            return to;
        }
    }
}
