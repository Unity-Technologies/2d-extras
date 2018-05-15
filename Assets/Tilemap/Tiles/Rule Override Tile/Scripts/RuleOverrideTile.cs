using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

namespace UnityEngine
{

	[Serializable]
	public class TileSpritePair
	{
		public Sprite m_OriginalSprite;
		public Sprite m_OverrideSprite;
	}

	[Serializable]
	[CreateAssetMenu]
	public class RuleOverrideTile : RuleTile
	{

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

		public RuleTile m_Tile;
		public List<TileSpritePair> m_Sprites = new List<TileSpritePair>();

		public override void GetTileData(Vector3Int position, ITilemap tileMap, ref TileData tileData)
		{
			CopyRules();
			base.GetTileData(position, tileMap, ref tileData);
			tileData.sprite = tileData.sprite ? this[tileData.sprite] : null;
		}
		public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
		{
			CopyRules();
			bool match = base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
			if (tileAnimationData.animatedSprites != null)
				tileAnimationData.animatedSprites = tileAnimationData.animatedSprites.Select(sprite => sprite ? this[sprite] : null).ToArray();
			return match;
		}
		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
		{
			CopyRules();
			base.RefreshTile(location, tileMap);
		}

		public void ApplyOverrides(IList<KeyValuePair<Sprite, Sprite>> overrides)
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

		private void CopyRules()
		{
			if (!m_Tile)
				return;

			m_DefaultSprite = m_Tile.m_DefaultSprite;
			m_DefaultColliderType = m_Tile.m_DefaultColliderType;
			m_TilingRules = m_Tile.m_TilingRules;
		}
	}
}
