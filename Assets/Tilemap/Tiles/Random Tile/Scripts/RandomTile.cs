using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityEngine.Tilemaps
{
	[Serializable]
	[CreateAssetMenu(fileName = "New Random Tile", menuName = "Tiles/Random Tile")]
	public class RandomTile : Tile
	{
		[SerializeField]
		public Sprite[] m_Sprites;

		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			base.GetTileData(location, tileMap, ref tileData);
			if ((m_Sprites != null) && (m_Sprites.Length > 0))
			{
				long hash = location.x;
				hash = (hash + 0xabcd1234) + (hash << 15);
				hash = (hash + 0x0987efab) ^ (hash >> 11);
				hash ^= location.y;
				hash = (hash + 0x46ac12fd) + (hash << 7);
				hash = (hash + 0xbe9730af) ^ (hash << 11);
				Random.InitState((int)hash);
				tileData.sprite = m_Sprites[(int) (m_Sprites.Length * Random.value)];
			}
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(RandomTile))]
	public class RandomTileEditor : Editor
	{
		private RandomTile tile { get { return (target as RandomTile); } }

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			int count = EditorGUILayout.DelayedIntField("Number of Sprites", tile.m_Sprites != null ? tile.m_Sprites.Length : 0);
			if (count < 0)
				count = 0;
			if (tile.m_Sprites == null || tile.m_Sprites.Length != count)
			{
				Array.Resize<Sprite>(ref tile.m_Sprites, count);
			}

			if (count == 0)
				return;

			EditorGUILayout.LabelField("Place random sprites.");
			EditorGUILayout.Space();

			for (int i = 0; i < count; i++)
			{
				tile.m_Sprites[i] = (Sprite) EditorGUILayout.ObjectField("Sprite " + (i+1), tile.m_Sprites[i], typeof(Sprite), false, null);
			}		
			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(tile);
		}
	}
#endif
}
