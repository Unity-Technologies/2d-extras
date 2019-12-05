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
        /// A data structure storing the GameObject overriding the original RuleTile GameObject
        /// </summary>
        [Serializable]
        public class TileGameObjectPair
        {
            public GameObject m_OriginalGameObject;
            public GameObject m_OverrideGameObject;
        }

        /// <summary>
        /// Gets the overriding Sprite of a given Sprite. 
        /// </summary>
        /// <param name="original">The original Sprite that is overridden</param>
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
        /// Gets the overriding GameObject of a given GameObject. 
        /// </summary>
        /// <param name="original">The original GameObject that is overridden</param>
        public GameObject this[GameObject originalGameObject]
        {
            get
            {
                foreach (TileGameObjectPair gameObjectPair in m_GameObjects)
                {
                    if (gameObjectPair.m_OriginalGameObject == originalGameObject)
                    {
                        return gameObjectPair.m_OverrideGameObject;
                    }
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    m_GameObjects = m_GameObjects.Where(gameObjectPair => gameObjectPair.m_OriginalGameObject != originalGameObject).ToList();
                }
                else
                {
                    foreach (TileGameObjectPair gameObjectPair in m_GameObjects)
                    {
                        if (gameObjectPair.m_OriginalGameObject == originalGameObject)
                        {
                            gameObjectPair.m_OverrideGameObject = value;
                            return;
                        }
                    }
                    m_GameObjects.Add(new TileGameObjectPair()
                    {
                        m_OriginalGameObject = originalGameObject,
                        m_OverrideGameObject = value,
                    });
                }
            }
        }

        /// <summary>
        /// The RuleTile to override
        /// </summary>
        public RuleTile m_Tile;
        /// <summary>
        /// A list of Sprite Overrides
        /// </summary>
        public List<TileSpritePair> m_Sprites = new List<TileSpritePair>();
        /// <summary>
        /// A list of GameObject Overrides
        /// </summary>
        public List<TileGameObjectPair> m_GameObjects = new List<TileGameObjectPair>();

        /// <summary>
        /// Returns the Rule Tile for retrieving TileData
        /// </summary>
        [HideInInspector] public RuleTile m_InstanceTile;
        [NonSerialized] public int m_MissingSpriteIndex = -1;
        [NonSerialized] public int m_MissingGameObjectIndex = -1;

        /// <summary>
        /// Applies overrides to this
        /// </summary>
        /// <param name="overrides">A list of overrides to apply</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void ApplyOverrides(IList<KeyValuePair<Sprite, Sprite>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

        /// <summary>
        /// Applies overrides to this
        /// </summary>
        /// <param name="overrides">A list of overrides to apply</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void ApplyOverrides(IList<KeyValuePair<GameObject, GameObject>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

        /// <summary>
        /// Gets overrides for this
        /// </summary>
        /// <param name="overrides">A list of overrides to fill</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void GetOverrides(List<KeyValuePair<Sprite, Sprite>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            overrides.Clear();

            List<Sprite> originalSprites = new List<Sprite>();

            if (m_Tile)
            {
                if (m_Tile.m_DefaultSprite)
                    originalSprites.Add(m_Tile.m_DefaultSprite);

                foreach (RuleTile.TilingRule rule in m_Tile.m_TilingRules)
                    foreach (Sprite sprite in rule.m_Sprites)
                        if (sprite && !originalSprites.Contains(sprite))
                            originalSprites.Add(sprite);
            }

            m_MissingSpriteIndex = originalSprites.Count;

            foreach (var pair in m_Sprites)
                if (!originalSprites.Contains(pair.m_OriginalSprite))
                    originalSprites.Add(pair.m_OriginalSprite);

            foreach (Sprite sprite in originalSprites)
                overrides.Add(new KeyValuePair<Sprite, Sprite>(sprite, this[sprite]));
        }

        /// <summary>
        /// Gets overrides for this
        /// </summary>
        /// <param name="overrides">A list of overrides to fill</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void GetOverrides(List<KeyValuePair<GameObject, GameObject>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            overrides.Clear();

            List<GameObject> originalGameObjects = new List<GameObject>();

            if (m_Tile)
            {
                if (m_Tile.m_DefaultGameObject)
                    originalGameObjects.Add(m_Tile.m_DefaultGameObject);

                foreach (RuleTile.TilingRule rule in m_Tile.m_TilingRules)
                    if (rule.m_GameObject && !originalGameObjects.Contains(rule.m_GameObject))
                        originalGameObjects.Add(rule.m_GameObject);
            }

            m_MissingGameObjectIndex = originalGameObjects.Count;

            foreach (var pair in m_GameObjects)
                if (!originalGameObjects.Contains(pair.m_OriginalGameObject))
                    originalGameObjects.Add(pair.m_OriginalGameObject);

            foreach (GameObject gameObject in originalGameObjects)
                overrides.Add(new KeyValuePair<GameObject, GameObject>(gameObject, this[gameObject]));
        }

        public virtual void Override()
        {
            if (!m_Tile || !m_InstanceTile)
                return;

            var tile = m_InstanceTile;

            tile.m_DefaultSprite = this[m_Tile.m_DefaultSprite] ?? m_Tile.m_DefaultSprite;
            tile.m_DefaultGameObject = this[m_Tile.m_DefaultGameObject] ?? m_Tile.m_DefaultGameObject;
            tile.m_DefaultColliderType = m_Tile.m_DefaultColliderType;
            tile.m_TilingRules.Clear();

            foreach (var originalRule in m_Tile.m_TilingRules)
            {
                var instanceRule = new RuleTile.TilingRule();
                CopyTilingRule(originalRule, instanceRule);

                for (int i = 0; i < instanceRule.m_Sprites.Length; i++)
                {
                    Sprite originalSprite = instanceRule.m_Sprites[i];
                    if (originalSprite)
                        instanceRule.m_Sprites[i] = this[originalSprite] ?? originalSprite;
                }

                instanceRule.m_GameObject = this[instanceRule.m_GameObject] ?? instanceRule.m_GameObject;

                tile.m_TilingRules.Add(instanceRule);
            }
        }

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
        /// Copies a Tiling Rule from a given Tiling Rule
        /// </summary>
        /// <param name="from">A Tiling Rule to copy from</param>
        /// <param name="to">A Tiling Rule to copy to</param>
        public static void CopyTilingRule(RuleTile.TilingRuleOutput from, RuleTile.TilingRuleOutput to)
        {
            to.m_Id = from.m_Id;
            to.m_Sprites = from.m_Sprites.Clone() as Sprite[];
            to.m_GameObject = from.m_GameObject;
            to.m_AnimationSpeed = from.m_AnimationSpeed;
            to.m_PerlinScale = from.m_PerlinScale;
            to.m_Output = from.m_Output;
            to.m_ColliderType = from.m_ColliderType;
            to.m_RandomTransform = from.m_RandomTransform;
        }
        public static void CopyTilingRule(RuleTile.TilingRule from, RuleTile.TilingRule to)
        {
            CopyTilingRule(from as RuleTile.TilingRuleOutput, to as RuleTile.TilingRuleOutput);

            to.m_Neighbors = from.m_Neighbors;
            to.m_NeighborPositions = from.m_NeighborPositions;
            to.m_RuleTransform = from.m_RuleTransform;
        }

    }
}
