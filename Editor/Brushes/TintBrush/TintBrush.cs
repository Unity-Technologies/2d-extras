using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    [CustomGridBrush(false, false, false, "Tint Brush")]
    public class TintBrush : GridBrushBase
    {
        public Color m_Color = Color.white;

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                SetColor(tilemap, position, m_Color);
            }
        }

        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                SetColor(tilemap, position, Color.white);
            }
        }

        private static void SetColor(Tilemap tilemap, Vector3Int position, Color color)
        {
            TileBase tile = tilemap.GetTile(position);
            if (tile != null)
            {
                if ((tilemap.GetTileFlags(position) & TileFlags.LockColor) != 0)
                {
                    if (tile is Tile)
                    {
                        Debug.LogWarning("Tint brush cancelled, because Tile (" + tile.name + ") has TileFlags.LockColor set. Unlock it from the Tile asset debug inspector.");
                    }
                    else
                    {
                        Debug.LogWarning("Tint brush cancelled. because Tile (" + tile.name + ") has TileFlags.LockColor set. Unset it in GetTileData().");
                    }
                }

                tilemap.SetColor(position, color);
            }
        }
    }


    [CustomEditor(typeof(TintBrush))]
    public class TintBrushEditor : GridBrushEditorBase
    {
        public override GameObject[] validTargets
        {
            get
            {
                return GameObject.FindObjectsOfType<Tilemap>().Select(x => x.gameObject).ToArray();
            }
        }
    }
}
