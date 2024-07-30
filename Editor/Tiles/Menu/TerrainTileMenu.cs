using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    internal static partial class AssetCreation
    {
        [MenuItem("Assets/Create/2D/Tiles/Terrain Tile", priority = (int)ETilesMenuItemOrder.TerrainTile)]
        private static void CreateTerrainTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<TerrainTile>(), "New Terrain Tile.asset");
        }
    }
}