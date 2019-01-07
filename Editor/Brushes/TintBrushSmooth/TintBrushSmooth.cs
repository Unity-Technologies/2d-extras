using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
	[CustomGridBrush(false, false, false, "Tint Brush (Smooth)")]
	public class TintBrushSmooth : GridBrushBase
	{
		private TintTextureGenerator generator
		{
			get
			{
				TintTextureGenerator generator = FindObjectOfType<TintTextureGenerator>();
				if (generator == null)
				{
					// Note: Code assumes only one grid in scene
					Grid grid = FindObjectOfType<Grid>();
					if (grid != null)
					{
						generator = grid.gameObject.AddComponent<TintTextureGenerator>();
					}
				}
				return generator;
			}
		}

		public float m_Blend = 1f;
		public Color m_Color = Color.white;

		public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

			TintTextureGenerator generator = GetGenerator(grid);
			if (generator != null)
			{
				var oldColor = generator.GetColor(grid as Grid, position);
				var blendColor = oldColor * (1 - m_Blend) + m_Color * m_Blend;
				generator.SetColor(grid as Grid, position, blendColor);
			}
		}

		public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

			TintTextureGenerator generator = GetGenerator(grid);
			if (generator != null)
			{
				generator.SetColor(grid as Grid, position, Color.white);
			}
		}

		public override void Pick(GridLayout grid, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

			TintTextureGenerator generator = GetGenerator(grid);
			if (generator != null)
			{
				m_Color = generator.GetColor(grid as Grid, position.min);
			}
		}

		private TintTextureGenerator GetGenerator(GridLayout grid)
		{
			TintTextureGenerator generator = FindObjectOfType<TintTextureGenerator>();
			if (generator == null)
			{
				if (grid != null)
				{
					generator = grid.gameObject.AddComponent<TintTextureGenerator>();
				}
			}
			return generator;
		}
	}

	[CustomEditor(typeof(TintBrushSmooth))]
	public class TintBrushSmoothEditor : GridBrushEditorBase
	{
		public TintBrushSmooth brush { get { return target as TintBrushSmooth; } }

		public override GameObject[] validTargets
		{
			get
			{
				return GameObject.FindObjectsOfType<Tilemap>().Select(x => x.gameObject).ToArray();
			}
		}

		public override void OnPaintInspectorGUI()
		{
			brush.m_Color = EditorGUILayout.ColorField("Color", brush.m_Color);
			brush.m_Blend = EditorGUILayout.Slider("Blend", brush.m_Blend, 0f, 1f);
			GUILayout.Label("Note: Tilemap needs to use TintedTilemap.shader!");
		}
	}
}
