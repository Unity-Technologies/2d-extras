using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    ///     This Brush changes the color of Tiles placed on a Tilemap.
    /// </summary>
    [CustomGridBrush(true, false, false, "Tint Brush")]
    public class TintBrush : GridBrushBase
    {
        /// <summary>
        ///     Color of the Tile to tint
        /// </summary>
        public Color m_Color = Color.white;

        /// <summary>
        ///     Tints tiles into a given position within the selected layers.
        ///     The TintBrush overrides this to set the color of the Tile to tint it.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap != null) SetColor(tilemap, position, m_Color);
        }

        /// <summary>
        ///     Resets the color of the tiles in a given position within the selected layers to White.
        ///     The TintBrush overrides this to set the color of the Tile to White.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to erase data from.</param>
        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap != null) SetColor(tilemap, position, Color.white);
        }

        private static void SetColor(Tilemap tilemap, Vector3Int position, Color color)
        {
            var tile = tilemap.GetTile(position);
            if (tile != null)
            {
                if ((tilemap.GetTileFlags(position) & TileFlags.LockColor) != 0)
                {
                    if (tile is Tile)
                        Debug.LogWarning("Tint brush cancelled, because Tile (" + tile.name +
                                         ") has TileFlags.LockColor set. Unlock it from the Tile asset debug inspector.");
                    else
                        Debug.LogWarning("Tint brush cancelled. because Tile (" + tile.name +
                                         ") has TileFlags.LockColor set. Unset it in GetTileData().");
                }

                tilemap.SetColor(position, color);
            }
        }
    }

    /// <summary>
    ///     The Brush Editor for a Tint Brush.
    /// </summary>
    [CustomEditor(typeof(TintBrush))]
    public class TintBrushEditor : GridBrushEditorBase
    {
        internal TintBrush brush => target as TintBrush;

        /// <summary>Returns all valid targets that the brush can edit.</summary>
        /// <remarks>Valid targets for the TintBrush are any GameObjects with a Tilemap component.</remarks>
        public override GameObject[] validTargets
        {
            get { return FindObjectsByType<Tilemap>(FindObjectsSortMode.None).Select(x => x.gameObject).ToArray(); }
        }

        /// <summary>
        ///     Creates a static preview of the RandomBrush with its current selection.
        /// </summary>
        /// <param name="assetPath">The asset to operate on.</param>
        /// <param name="subAssets">An array of all Assets at assetPath.</param>
        /// <param name="width">Width of the created texture.</param>
        /// <param name="height">Height of the created texture.</param>
        /// <returns>Generated texture or null.</returns>
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (brush == null)
                return null;

            var tex = new Texture2D(width, height);
            var data = tex.GetRawTextureData<Color32>();
            var index = 0;
            for (var y = 0; y < tex.height; y++)
            for (var x = 0; x < tex.width; x++)
                data[index++] = brush.m_Color;
            tex.Apply();
            return tex;
        }
    }
}