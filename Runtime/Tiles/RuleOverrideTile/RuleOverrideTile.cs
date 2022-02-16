using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting.APIUpdating;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// Rule Override Tiles are Tiles which can override a subset of Rules for a given Rule Tile to provide specialised behaviour while keeping most of the Rules originally set in the Rule Tile.
    /// </summary>
    [MovedFrom(true, "UnityEngine")]
    [Serializable]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest/index.html?subfolder=/manual/RuleOverrideTile.html")]
    public class RuleOverrideTile : TileBase
    {

        /// <summary>
        /// A data structure storing the Sprite overriding the original RuleTile Sprite
        /// </summary>
        [Serializable]
        public class TileSpritePair
        {
            /// <summary>
            /// Original Sprite from the original RuleTile.
            /// </summary>
            public Sprite m_OriginalSprite;
            /// <summary>
            /// Overriding Sprite for the Original Sprite.
            /// </summary>
            public Sprite m_OverrideSprite;
        }

        /// <summary>
        /// A data structure storing the GameObject overriding the original RuleTile GameObject
        /// </summary>
        [Serializable]
        public class TileGameObjectPair
        {
            /// <summary>
            /// Original GameObject from the original RuleTile.
            /// </summary>
            public GameObject m_OriginalGameObject;
            /// <summary>
            /// Overriding GameObject for the Original Sprite.
            /// </summary>
            public GameObject m_OverrideGameObject;
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
        /// Gets the overriding GameObject of a given GameObject. 
        /// </summary>
        /// <param name="originalGameObject">The original GameObject that is overridden</param>
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

        private void CreateInstanceTile()
        {
            var t = m_Tile.GetType();
            RuleTile instanceTile = CreateInstance(t) as RuleTile;
            instanceTile.hideFlags = HideFlags.NotEditable;
            instanceTile.name = m_Tile.name + " (Override)";
            m_InstanceTile = instanceTile;

#if UNITY_EDITOR
            if(AssetDatabase.Contains(this))
                AssetDatabase.AddObjectToAsset(instanceTile, this);
            EditorUtility.SetDirty(this);
#endif            
        }
        
        /// <summary>
        /// Applies overrides to this
        /// </summary>
        /// <param name="overrides">A list of overrides to apply</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void ApplyOverrides(IList<KeyValuePair<Sprite, Sprite>> overrides)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

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
                throw new ArgumentNullException("overrides");

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

        /// <summary>
        /// Gets overrides for this
        /// </summary>
        /// <param name="overrides">A list of overrides to fill</param>
        /// <param name="validCount">Returns the number of valid overrides for Sprites</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void GetOverrides(List<KeyValuePair<Sprite, Sprite>> overrides, ref int validCount)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

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

            validCount = originalSprites.Count;

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
        /// <param name="validCount">Returns the number of valid overrides for GameObjects</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void GetOverrides(List<KeyValuePair<GameObject, GameObject>> overrides, ref int validCount)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

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

            validCount = originalGameObjects.Count;

            foreach (var pair in m_GameObjects)
                if (!originalGameObjects.Contains(pair.m_OriginalGameObject))
                    originalGameObjects.Add(pair.m_OriginalGameObject);

            foreach (GameObject gameObject in originalGameObjects)
                overrides.Add(new KeyValuePair<GameObject, GameObject>(gameObject, this[gameObject]));
        }

        /// <summary>
        /// Updates the Rules with the Overrides set for this RuleOverrideTile
        /// </summary>
        public virtual void Override()
        {
            if (!m_Tile)
                return;

            if (!m_InstanceTile)
                CreateInstanceTile();
            
            PrepareOverride();

            var tile = m_InstanceTile;

            tile.m_DefaultSprite = this[tile.m_DefaultSprite] ?? tile.m_DefaultSprite;
            tile.m_DefaultGameObject = this[tile.m_DefaultGameObject] ?? tile.m_DefaultGameObject;

            foreach (var rule in tile.m_TilingRules)
            {
                for (int i = 0; i < rule.m_Sprites.Length; i++)
                {
                    Sprite sprite = rule.m_Sprites[i];
                    rule.m_Sprites[i] = this[sprite] ?? sprite;
                }

                rule.m_GameObject = this[rule.m_GameObject] ?? rule.m_GameObject;
            }
        }

        /// <summary>
        /// Prepares the Overrides set for this RuleOverrideTile
        /// </summary>
        public void PrepareOverride()
        {
            // Create clone of instanceTile to keep data from collections being overridden by JsonUtility
            var tempTile = Instantiate(m_InstanceTile);
            
            var customData = m_InstanceTile.GetCustomFields(true)
                .ToDictionary(field => field, field => field.GetValue(tempTile));

            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(m_Tile), m_InstanceTile);

            foreach (var kvp in customData)
                kvp.Key.SetValue(m_InstanceTile, kvp.Value);
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
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            if (!m_InstanceTile)
                return;
            m_InstanceTile.RefreshTile(position, tilemap);
        }

        /// <summary>
        /// StartUp is called on the first frame of the running Scene.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="go">The GameObject instantiated for the Tile.</param>
        /// <returns>Whether StartUp was successful</returns>
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            if (!m_InstanceTile)
                return true;
            return m_InstanceTile.StartUp(position, tilemap, go);
        }

        /// <summary>
        /// Callback when the tile is enabled
        /// </summary>
        public void OnEnable()
        {
            if (m_Tile == null)
                return;

            if (m_InstanceTile == null)
                Override();
        }
    }
}
