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
    public class RuleOverrideTile : RuleOverrideTileBase, IRuleOverrideTile<Sprite, Sprite>, IRuleOverrideTile<GameObject, GameObject>
    {

        [Serializable]
        public class TileSpritePair
        {
            public Sprite m_OriginalSprite;
            public Sprite m_OverrideSprite;
        }

        [Serializable]
        public class TileGameObjectPair
        {
            public GameObject m_OriginalGameObject;
            public GameObject m_OverrideGameObject;
        }

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

        public List<TileSpritePair> m_Sprites = new List<TileSpritePair>();
        public List<TileGameObjectPair> m_GameObjects = new List<TileGameObjectPair>();
        [NonSerialized] public int m_MissingSpriteIndex = -1;
        [NonSerialized] public int m_MissingGameObjectIndex = -1;

        public void ApplyOverrides(IList<KeyValuePair<Sprite, Sprite>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

        public void ApplyOverrides(IList<KeyValuePair<GameObject, GameObject>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

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

        public override void Override()
        {
            if (!m_Tile || !m_InstanceTile)
                return;

            var tile = m_InstanceTile;

            tile.m_DefaultSprite = this[m_Tile.m_DefaultSprite];
            tile.m_DefaultGameObject = this[m_Tile.m_DefaultGameObject];
            tile.m_DefaultColliderType = m_Tile.m_DefaultColliderType;
            tile.m_TilingRules.Clear();

            foreach (var originalRule in m_Tile.m_TilingRules)
            {
                var instanceRule = new RuleTile.TilingRule();
                CopyTilingRule(originalRule, instanceRule);

                for (int i = 0; i < instanceRule.m_Sprites.Length; i++)
                    if (instanceRule.m_Sprites[i])
                        instanceRule.m_Sprites[i] = this[instanceRule.m_Sprites[i]];

                instanceRule.m_GameObject = this[instanceRule.m_GameObject];

                tile.m_TilingRules.Add(instanceRule);
            }
        }
    }
}
