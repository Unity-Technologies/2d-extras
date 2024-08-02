using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    internal static partial class AssetCreation
    {
        [MenuItem("Assets/Create/2D/Tiles/Weighted Random Tile",
            priority = (int)ETilesMenuItemOrder.WeightedRandomTile)]
        private static void CreateWeightedRandomTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<WeightedRandomTile>(),
                "New Weighted Random Tile.asset");
        }
    }
}