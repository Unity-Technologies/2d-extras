using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace UnityEngine
{
    public class HexagonalRuleTile<T> : HexagonalRuleTile
    {
        public sealed override Type m_NeighborType { get { return typeof(T); } }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "New Hexagonal Rule Tile", menuName = "Tiles/Hexagonal Rule Tile")]
    public class HexagonalRuleTile : RuleTile
    {
        private static readonly int[,] RotatedOrMirroredIndexes =
        {
            {3, 2, 1, 0, 5, 4}, // X, Pointed
            {0, 5, 4, 3, 2, 1}, // Y, Pointed
            {3, 4, 5, 0, 1, 2}, // XY
            {0, 5, 4, 3, 2, 1}, // X, FlatTop
            {3, 2, 1, 0, 5, 4}, // Y, FlatTop
        };

        private static readonly Vector3Int[,] PointedTopNeighborOffsets =
        {
            {
                new Vector3Int(1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0)
            },
            {
                new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0)
            }
        };
        private static readonly Vector3Int[,] FlatTopNeighborOffsets =
        {
            {
                new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0)
            },
            {
                new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0)
            }
        };

        private static readonly int NeighborCount = 6;
        public override int neighborCount
        {
            get { return NeighborCount; }
        }

        public bool m_FlatTop;

        public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            if (m_TilingRules != null && m_TilingRules.Count > 0)
            {
                for (int i = 0; i < neighborCount; ++i)
                {
                    base.RefreshTile(location + GetOffsetPosition(location, i), tileMap);
                }
            }
            base.RefreshTile(location, tileMap);
        }

        protected override bool RuleMatches(TilingRule rule, ref TileBase[] neighboringTiles, ref Matrix4x4 transform)
        {
            // Check rule against rotations of 0, 60, 120, 180, 240, 300
            for (int angle = 0; angle <= (rule.m_RuleTransform == TilingRule.Transform.Rotated ? 300 : 0); angle += 60)
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

        protected override Matrix4x4 ApplyRandomTransform(TilingRule.Transform type, Matrix4x4 original, float perlinScale, Vector3Int position)
        {
            float perlin = GetPerlinValue(position, perlinScale, 200000f);
            switch (type)
            {
                case TilingRule.Transform.MirrorX:
                    return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(perlin < 0.5 ? 1f : -1f, 1f, 1f));
                case TilingRule.Transform.MirrorY:
                    return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, perlin < 0.5 ? 1f : -1f, 1f));
                case TilingRule.Transform.Rotated:
                    int angle = Mathf.Clamp(Mathf.FloorToInt(perlin * neighborCount), 0, neighborCount - 1) * (360 / neighborCount);
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
            }
            return original;
        }

        protected override void GetMatchingNeighboringTiles(ITilemap tilemap, Vector3Int position, ref TileBase[] neighboringTiles)
        {
            if (neighboringTiles != null)
                return;

            if (m_CachedNeighboringTiles == null || m_CachedNeighboringTiles.Length < neighborCount)
                m_CachedNeighboringTiles = new TileBase[neighborCount];

            for (int index = 0; index < neighborCount; ++index)
            {
                Vector3Int tilePosition = position + GetOffsetPosition(position, index);
                m_CachedNeighboringTiles[index] = tilemap.GetTile(tilePosition);
            }
            neighboringTiles = m_CachedNeighboringTiles;
        }

        protected override int GetRotatedIndex(int original, int rotation)
        {
            return (original + rotation / 60) % neighborCount;
        }

        protected override int GetMirroredIndex(int original, bool mirrorX, bool mirrorY)
        {
            if (mirrorX && mirrorY)
            {
                return RotatedOrMirroredIndexes[2, original];
            }
            if (mirrorX)
            {
                return RotatedOrMirroredIndexes[m_FlatTop ? 3 : 0, original];
            }
            if (mirrorY)
            {
                return RotatedOrMirroredIndexes[m_FlatTop ? 4 : 1, original];
            }
            return original;
        }

        private Vector3Int GetOffsetPosition(Vector3Int location, int direction)
        {
            var parity = location.y & 1;
            return m_FlatTop ? FlatTopNeighborOffsets[parity, direction] : PointedTopNeighborOffsets[parity, direction];
        }
    }
}
