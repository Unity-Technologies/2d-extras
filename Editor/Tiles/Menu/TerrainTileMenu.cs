using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    static internal partial class AssetCreation
    {
            
        [MenuItem("Assets/Create/2D/Tiles/Terrain Tile", priority = (int)ETilesMenuItemOrder.TerrainTile)]
        static void CreateTerrainTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<TerrainTile>(), "New Terrain Tile.asset");
        }
    }
}