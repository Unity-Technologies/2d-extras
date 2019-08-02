using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace UnityEngine
{
    /// <summary>
    /// Generic visual tile for creating different tilesets like terrain, pipeline, random or animated tiles.
    /// This is templated to accept a Neighbor Rule Class for Custom Rules.
    /// </summary>
    /// <typeparam name="T">Neighbor Rule Class for Custom Rules</typeparam>
    public class RuleTile<T> : RuleTile
    {
        /// <summary>
        /// Returns the Neighbor Rule Class type for this Rule Tile.
        /// </summary>
        public sealed override Type m_NeighborType { get { return typeof(T); } }
    }

    /// <summary>
    /// Generic visual tile for creating different tilesets like terrain, pipeline, random or animated tiles.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Rule Tile", menuName = "Tiles/Rule Tile")]
    public class RuleTile : TileBase
    {
        /// <summary>
        /// Returns the default Neighbor Rule Class type.
        /// </summary>
        public virtual Type m_NeighborType { get { return typeof(TilingRule.Neighbor); } }

        private static readonly int[,] RotatedOrMirroredIndexes =
        {
            {2, 4, 7, 1, 6, 0, 3, 5}, // 90
            {7, 6, 5, 4, 3, 2, 1, 0}, // 180, XY
            {5, 3, 0, 6, 1, 7, 4, 2}, // 270
            {2, 1, 0, 4, 3, 7, 6, 5}, // X
            {5, 6, 7, 3, 4, 0, 1, 2}, // Y
        };
        private static readonly int NeighborCount = 8;

        /// <summary>
        /// Returns the number of neighbors a Rule Tile can have.
        /// </summary>
        public virtual int neighborCount
        {
            get { return NeighborCount; }
        }

        /// <summary>
        /// The Default Sprite set when creating a new Rule.
        /// </summary>
        public Sprite m_DefaultSprite;
        /// <summary>
        /// The Default GameObject set when creating a new Rule.
        /// </summary>
        public GameObject m_DefaultGameObject;
        /// <summary>
        /// The Default Collider Type set when creating a new Rule.
        /// </summary>
        public Tile.ColliderType m_DefaultColliderType = Tile.ColliderType.Sprite;
        /// <summary>
        /// Returns the instance of this Tile when matching Rules.
        /// </summary>
        public TileBase m_Self
        {
            get { return m_OverrideSelf ? m_OverrideSelf : this; }
            set { m_OverrideSelf = value; }
        }

        /// <summary>
        /// A cache for the neighboring Tiles when matching Rules.
        /// </summary>
        protected TileBase[] m_CachedNeighboringTiles = new TileBase[NeighborCount];
        private TileBase m_OverrideSelf;
        private Quaternion m_GameObjectQuaternion;

        /// <summary>
        /// The data structure holding the Rule information for matching Rule Tiles with
        /// its neighbors.
        /// </summary>
        [Serializable]
        public class TilingRule
        {
            /// <summary>
            /// The matching Rule conditions for each of its neighboring Tiles.
            /// </summary>
            public int[] m_Neighbors;
            /// <summary>
            /// The output Sprites for this Rule.
            /// </summary>
            public Sprite[] m_Sprites;
            /// <summary>
            /// The output GameObject for this Rule.
            /// </summary>
            public GameObject m_GameObject;
            /// <summary>
            /// The output Animation Speed for this Rule.
            /// </summary>
            public float m_AnimationSpeed;
            /// <summary>
            /// The perlin scale factor for this Rule.
            /// </summary>
            public float m_PerlinScale;
            /// <summary>
            /// The transform matching Rule for this Rule.
            /// </summary>
            public Transform m_RuleTransform;
            /// <summary>
            /// The output type for this Rule.
            /// </summary>
            public OutputSprite m_Output;
            /// <summary>
            /// The output Collider Type for this Rule.
            /// </summary>
            public Tile.ColliderType m_ColliderType;
            /// <summary>
            /// The randomized transform output for this Rule.
            /// </summary>
            public Transform m_RandomTransform;

            /// <summary>
            /// Constructor for Tiling Rule. This defaults to a Single Output.
            /// </summary>
            public TilingRule()
            {
                m_Output = OutputSprite.Single;
                m_Neighbors = new int[NeighborCount];
                m_Sprites = new Sprite[1];
                m_GameObject = null;
                m_AnimationSpeed = 1f;
                m_PerlinScale = 0.5f;
                m_ColliderType = Tile.ColliderType.Sprite;

                for (int i = 0; i < m_Neighbors.Length; i++)
                    m_Neighbors[i] = Neighbor.DontCare;
            }

            /// <summary>
            /// The enumeration for matching Neighbors when matching Rule Tiles
            /// </summary>
            public class Neighbor
            {
                /// <summary>
                /// The Rule Tile will not care about the contents of the cell in that direction
                /// </summary>
                public const int DontCare = 0;
                /// <summary>
                /// The Rule Tile will check if the contents of the cell in that direction is an instance of this Rule Tile.
                /// If not, the rule will fail.
                /// </summary>
                public const int This = 1;
                /// <summary>
                /// The Rule Tile will check if the contents of the cell in that direction is not an instance of this Rule Tile.
                /// If it is, the rule will fail.
                /// </summary>
                public const int NotThis = 2;
            }

            /// <summary>
            /// The enumeration for the transform rule used when matching Rule Tiles.
            /// </summary>
            public enum Transform
            {
                /// <summary>
                /// The Rule Tile will match Tiles exactly as laid out in its neighbors.
                /// </summary>
                Fixed,
                /// <summary>
                /// The Rule Tile will rotate and match its neighbors.
                /// </summary>
                Rotated,
                /// <summary>
                /// The Rule Tile will mirror in the X axis and match its neighbors.
                /// </summary>
                MirrorX,
                /// <summary>
                /// The Rule Tile will mirror in the Y axis and match its neighbors.
                /// </summary>
                MirrorY
            }

            /// <summary>
            /// The Output for the Tile which fits this Rule.
            /// </summary>
            public enum OutputSprite
            {
                /// <summary>
                /// A Single Sprite will be output.
                /// </summary>
                Single,
                /// <summary>
                /// A Random Sprite will be output.
                /// </summary>
                Random, 
                /// <summary>
                /// A Sprite Animation will be output.
                /// </summary>
                Animation
            }
        }

        /// <summary>
        /// A list of Tiling Rules for the Rule Tile.
        /// </summary>
        [HideInInspector] public List<TilingRule> m_TilingRules;

        /// <summary>
        /// StartUp is called on the first frame of the running Scene.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="instantiatedGameObject">The GameObject instantiated for the Tile.</param>
        /// <returns>Whether StartUp was successful</returns>
        public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject instantiatedGameObject)
        {
            if (instantiatedGameObject != null)
            {
                Tilemap tmpMap = tilemap.GetComponent<Tilemap>();
                instantiatedGameObject.transform.position = tmpMap.LocalToWorld(tmpMap.CellToLocalInterpolated(location + tmpMap.tileAnchor));
                instantiatedGameObject.transform.rotation = m_GameObjectQuaternion;
            }

            return true;
        }

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            TileBase[] neighboringTiles = null;
            GetMatchingNeighboringTiles(tilemap, position, ref neighboringTiles);
            var iden = Matrix4x4.identity;

            tileData.sprite = m_DefaultSprite;
            tileData.gameObject = m_DefaultGameObject;
            tileData.colliderType = m_DefaultColliderType;
            tileData.flags = TileFlags.LockTransform;
            tileData.transform = iden;

            foreach (TilingRule rule in m_TilingRules)
            {
                Matrix4x4 transform = iden;
                if (RuleMatches(rule, ref neighboringTiles, ref transform))
                {
                    switch (rule.m_Output)
                    {
                        case TilingRule.OutputSprite.Single:
                        case TilingRule.OutputSprite.Animation:
                            tileData.sprite = rule.m_Sprites[0];
                            break;
                        case TilingRule.OutputSprite.Random:
                            int index = Mathf.Clamp(Mathf.FloorToInt(GetPerlinValue(position, rule.m_PerlinScale, 100000f) * rule.m_Sprites.Length), 0, rule.m_Sprites.Length - 1);
                            tileData.sprite = rule.m_Sprites[index];
                            if (rule.m_RandomTransform != TilingRule.Transform.Fixed)
                                transform = ApplyRandomTransform(rule.m_RandomTransform, transform, rule.m_PerlinScale, position);
                            break;
                    }
                    tileData.transform = transform;
                    tileData.gameObject = rule.m_GameObject;
                    tileData.colliderType = rule.m_ColliderType;

                    // Converts the tile's rotation matrix to a quaternion to be used by the instantiated Game Object
                    m_GameObjectQuaternion = Quaternion.LookRotation(new Vector3(transform.m02, transform.m12, transform.m22), new Vector3(transform.m01, transform.m11, transform.m21));
                    break;
                }
            }
        }

        /// <summary>
        /// Returns a Perlin Noise value based on the given inputs.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="scale">The Perlin Scale factor of the Tile.</param>
        /// <param name="offset">Offset of the Tile on the Tilemap.</param>
        /// <returns>A Perlin Noise value based on the given inputs.</returns>
        protected static float GetPerlinValue(Vector3Int position, float scale, float offset)
        {
            return Mathf.PerlinNoise((position.x + offset) * scale, (position.y + offset) * scale);
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
            TileBase[] neighboringTiles = null;
            var iden = Matrix4x4.identity;
            foreach (TilingRule rule in m_TilingRules)
            {
                if (rule.m_Output == TilingRule.OutputSprite.Animation)
                {
                    Matrix4x4 transform = iden;
                    GetMatchingNeighboringTiles(tilemap, position, ref neighboringTiles);
                    if (RuleMatches(rule, ref neighboringTiles, ref transform))
                    {
                        tileAnimationData.animatedSprites = rule.m_Sprites;
                        tileAnimationData.animationSpeed = rule.m_AnimationSpeed;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This method is called when the tile is refreshed.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tileMap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            if (m_TilingRules != null && m_TilingRules.Count > 0)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        base.RefreshTile(location + new Vector3Int(x, y, 0), tileMap);
                    }
                }
            }
            else
            {
                base.RefreshTile(location, tileMap);
            }
        }

        /// <summary>
        /// Does a Rule Match given a Tiling Rule and neighboring Tiles.
        /// </summary>
        /// <param name="rule">The Tiling Rule to match with.</param>
        /// <param name="neighboringTiles">The neighboring Tiles to match with.</param>
        /// <param name="transform">A transform matrix which will match the Rule.</param>
        /// <returns>True if there is a match, False if not.</returns>
        protected virtual bool RuleMatches(TilingRule rule, ref TileBase[] neighboringTiles, ref Matrix4x4 transform)
        {
            // Check rule against rotations of 0, 90, 180, 270
            for (int angle = 0; angle <= (rule.m_RuleTransform == TilingRule.Transform.Rotated ? 270 : 0); angle += 90)
            {
                if (RuleMatches(rule, ref neighboringTiles, angle))
                {
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
                    return true;
                }
            }

            // Check rule against x-axis mirror
            if ((rule.m_RuleTransform == TilingRule.Transform.MirrorX) && RuleMatches(rule, ref neighboringTiles, true, false))
            {
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
                return true;
            }

            // Check rule against y-axis mirror
            if ((rule.m_RuleTransform == TilingRule.Transform.MirrorY) && RuleMatches(rule, ref neighboringTiles, false, true))
            {
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a random transform matrix given the random transform rule.
        /// </summary>
        /// <param name="type">Random transform rule.</param>
        /// <param name="original">The original transform matrix.</param>
        /// <param name="perlinScale">The Perlin Scale factor of the Tile.</param>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <returns>A random transform matrix.</returns>
        protected virtual Matrix4x4 ApplyRandomTransform(TilingRule.Transform type, Matrix4x4 original, float perlinScale, Vector3Int position)
        {
            float perlin = GetPerlinValue(position, perlinScale, 200000f);
            switch (type)
            {
                case TilingRule.Transform.MirrorX:
                    return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(perlin < 0.5 ? 1f : -1f, 1f, 1f));
                case TilingRule.Transform.MirrorY:
                    return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, perlin < 0.5 ? 1f : -1f, 1f));
                case TilingRule.Transform.Rotated:
                    int angle = Mathf.Clamp(Mathf.FloorToInt(perlin * 4), 0, 3) * 90;
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
            }
            return original;
        }

        /// <summary>
        /// Checks if there is a match given the neighbor matching rule and a Tile.
        /// </summary>
        /// <param name="neighbor">Neighbor matching rule.</param>
        /// <param name="tile">Tile to match.</param>
        /// <returns>True if there is a match, False if not.</returns>
        public virtual bool RuleMatch(int neighbor, TileBase tile)
        {
            if (tile is RuleOverrideTile)
                tile = (tile as RuleOverrideTile).runtimeTile.m_Self;
            else if (tile is RuleTile)
                tile = (tile as RuleTile).m_Self;

            switch (neighbor)
            {
                case TilingRule.Neighbor.This: return tile == m_Self;
                case TilingRule.Neighbor.NotThis: return tile != m_Self;
            }
            return true;
        }

        /// <summary>
        /// Checks if there is a match given the neighbor matching rule and a Tile with a rotation angle.
        /// </summary>
        /// <param name="rule">Neighbor matching rule.</param>
        /// <param name="neighboringTiles">Tile to match.</param>
        /// <param name="angle">Rotation angle for matching.</param>
        /// <returns>True if there is a match, False if not.</returns>
        protected bool RuleMatches(TilingRule rule, ref TileBase[] neighboringTiles, int angle)
        {
            for (int i = 0; i < neighborCount; ++i)
            {
                int index = GetRotatedIndex(i, angle);
                TileBase tile = neighboringTiles[index];
                if (!RuleMatch(rule.m_Neighbors[i], tile))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if there is a match given the neighbor matching rule and a Tile with mirrored axii.
        /// </summary>
        /// <param name="rule">Neighbor matching rule.</param>
        /// <param name="neighboringTiles">Tile to match.</param>
        /// <param name="mirrorX">Mirror X Axis for matching.</param>
        /// <param name="mirrorY">Mirror Y Axis for matching.</param>
        /// <returns>True if there is a match, False if not.</returns>
        protected bool RuleMatches(TilingRule rule, ref TileBase[] neighboringTiles, bool mirrorX, bool mirrorY)
        {
            for (int i = 0; i < neighborCount; ++i)
            {
                int index = GetMirroredIndex(i, mirrorX, mirrorY);
                TileBase tile = neighboringTiles[index];
                if (!RuleMatch(rule.m_Neighbors[i], tile))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets and caches the neighboring Tiles around the given Tile on the Tilemap.
        /// </summary>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="neighboringTiles">An array storing the neighboring Tiles</param>
        protected virtual void GetMatchingNeighboringTiles(ITilemap tilemap, Vector3Int position, ref TileBase[] neighboringTiles)
        {
            if (neighboringTiles != null)
                return;

            if (m_CachedNeighboringTiles == null || m_CachedNeighboringTiles.Length < neighborCount)
                m_CachedNeighboringTiles = new TileBase[neighborCount];

            int index = 0;
            for (int y = 1; y >= -1; y--)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x != 0 || y != 0)
                    {
                        Vector3Int tilePosition = new Vector3Int(position.x + x, position.y + y, position.z);
                        m_CachedNeighboringTiles[index++] = tilemap.GetTile(tilePosition);
                    }
                }
            }
            neighboringTiles = m_CachedNeighboringTiles;
        }

        /// <summary>
        /// Gets a rotated index given its original index and the rotation in degrees. 
        /// </summary>
        /// <param name="original">Original index of Tile.</param>
        /// <param name="rotation">Rotation in degrees.</param>
        /// <returns>Rotated Index of Tile.</returns>
        protected virtual int GetRotatedIndex(int original, int rotation)
        {
            switch (rotation)
            {
                case 0:
                    return original;
                case 90:
                    return RotatedOrMirroredIndexes[0, original];
                case 180:
                    return RotatedOrMirroredIndexes[1, original];
                case 270:
                    return RotatedOrMirroredIndexes[2, original];
            }
            return original;
        }

        /// <summary>
        /// Gets a mirrored index given its original index and the mirroring axii.
        /// </summary>
        /// <param name="original">Original index of Tile.</param>
        /// <param name="mirrorX">Mirror in the X Axis.</param>
        /// <param name="mirrorY">Mirror in the Y Axis.</param>
        /// <returns>Mirrored Index of Tile.</returns>
        protected virtual int GetMirroredIndex(int original, bool mirrorX, bool mirrorY)
        {
            if (mirrorX && mirrorY)
            {
                return RotatedOrMirroredIndexes[1, original];
            }
            if (mirrorX)
            {
                return RotatedOrMirroredIndexes[3, original];
            }
            if (mirrorY)
            {
                return RotatedOrMirroredIndexes[4, original];
            }
            return original;
        }
    }
}
