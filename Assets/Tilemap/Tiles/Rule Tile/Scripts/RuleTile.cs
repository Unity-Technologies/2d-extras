using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace UnityEngine
{
	[Serializable]
	[CreateAssetMenu]
	public class RuleTile : TileBase
	{
		private static readonly int[,] RotatedOrMirroredIndexes =
		{
			{2, 4, 7, 1, 6, 0, 3, 5}, // 90
			{7, 6, 5, 4, 3, 2, 1, 0}, // 180, XY
			{5, 3, 0, 6, 1, 7, 4, 2}, // 270
			{2, 1, 0, 4, 3, 7, 6, 5}, // X
			{5, 6, 7, 3, 4, 0, 1, 2}, // Y
		};
		private static readonly int NeighborCount = 8;
		
		public Sprite m_DefaultSprite;
		public Tile.ColliderType m_DefaultColliderType = Tile.ColliderType.Sprite;
		
		private TileBase[] m_CachedNeighboringTiles = new TileBase[NeighborCount];

		[Serializable]
		public class TilingRule
		{
			public Neighbor[] m_Neighbors;
			public Sprite[] m_Sprites;
			public float m_AnimationSpeed;
			public float m_PerlinScale;
			public Transform m_RuleTransform;
			public OutputSprite m_Output;
			public Tile.ColliderType m_ColliderType;
			public Transform m_RandomTransform;
			
			public TilingRule()
			{
				m_Output = OutputSprite.Single;
				m_Neighbors = new Neighbor[NeighborCount];
				m_Sprites = new Sprite[1];
				m_AnimationSpeed = 1f;
				m_PerlinScale = 0.5f;
				m_ColliderType = Tile.ColliderType.Sprite;

				for (int i=0; i<m_Neighbors.Length; i++)
					m_Neighbors[i] = Neighbor.DontCare;
			}

			public enum Transform { Fixed, Rotated, MirrorX, MirrorY }
			public enum Neighbor { DontCare, This, NotThis }
			public enum OutputSprite { Single, Random, Animation }
		}

		[HideInInspector] public List<TilingRule> m_TilingRules;

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			TileBase[] neighboringTiles = null;
			GetMatchingNeighboringTiles(tilemap, position, ref neighboringTiles);
			var iden = Matrix4x4.identity;
			
			tileData.sprite = m_DefaultSprite;
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
					tileData.colliderType = rule.m_ColliderType;
					break;
				}
			}
		}

		private static float GetPerlinValue(Vector3Int position, float scale, float offset)
		{
			return Mathf.PerlinNoise((position.x + offset) * scale, (position.y + offset) * scale);
		}

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

		public bool RuleMatches(TilingRule rule, ref TileBase[] neighboringTiles, ref Matrix4x4 transform)
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

		private static Matrix4x4 ApplyRandomTransform(TilingRule.Transform type, Matrix4x4 original, float perlinScale, Vector3Int position)
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

		public bool RuleMatches(TilingRule rule, ref TileBase[] neighboringTiles, int angle)
		{
			for (int i = 0; i < NeighborCount; ++i)
			{
				int index = GetRotatedIndex(i, angle);
				TileBase tile = neighboringTiles[index];
				if (rule.m_Neighbors[i] == TilingRule.Neighbor.This && tile != this || rule.m_Neighbors[i] == TilingRule.Neighbor.NotThis && tile == this)
				{
					return false;
				}	
			}
			return true;
		}

		public bool RuleMatches(TilingRule rule, ref TileBase[] neighboringTiles, bool mirrorX, bool mirrorY)
		{
			for (int i = 0; i < NeighborCount; ++i)
			{
				int index = GetMirroredIndex(i, mirrorX, mirrorY);
				TileBase tile = neighboringTiles[index];
				if (rule.m_Neighbors[i] == TilingRule.Neighbor.This && tile != this || rule.m_Neighbors[i] == TilingRule.Neighbor.NotThis && tile == this)
				{
					return false;
				}
			}
			return true;
		}

		private void GetMatchingNeighboringTiles(ITilemap tilemap, Vector3Int position, ref TileBase[] neighboringTiles)
		{
			if (neighboringTiles != null)
				return;

			if (m_CachedNeighboringTiles == null || m_CachedNeighboringTiles.Length < NeighborCount)
				m_CachedNeighboringTiles = new TileBase[NeighborCount];

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

		private int GetRotatedIndex(int original, int rotation)
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

		private int GetMirroredIndex(int original, bool mirrorX, bool mirrorY)
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
		
		private int GetIndexOfOffset(Vector3Int offset)
		{
			int result = offset.x + 1 + (-offset.y + 1) * 3;
			if (result >= 4)
				result--;
			return result;
		}

		public Vector3Int GetRotatedPos(Vector3Int original, int rotation)
		{
			switch (rotation)
			{
				case 0:
					return original;
				case 90:
					return new Vector3Int(-original.y, original.x, original.z);
				case 180:
					return new Vector3Int(-original.x, -original.y, original.z);
				case 270:
					return new Vector3Int(original.y, -original.x, original.z);
			}
			return original;
		}

		public Vector3Int GetMirroredPos(Vector3Int original, bool mirrorX, bool mirrorY)
		{
			return new Vector3Int(original.x * (mirrorX ? -1 : 1), original.y * (mirrorY ? -1 : 1), original.z);
		}
	}
}
