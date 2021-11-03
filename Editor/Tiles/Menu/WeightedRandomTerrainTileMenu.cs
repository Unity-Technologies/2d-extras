using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    static internal partial class AssetCreation
    {
            
        [MenuItem("Assets/Create/2D/Tiles/Weighted Random Terrain Tile", priority = (int)ETilesMenuItemOrder.WeightedRandomTerrainTile)]
        static void CreateWeightedRandomTerrainTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<WeightedRandomTerrainTile>(), "New Weighted Random Terrain Tile.asset");
        }
    }
}