using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    static internal partial class AssetCreation
    {

        [MenuItem("Assets/Create/2D/Tiles/Weighted Random Tile", priority = (int)ETilesMenuItemOrder.WeightedRandomTile)]
        static void CreateWeightedRandomTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<WeightedRandomTile>(), "New Weighted Random Tile.asset");
        }
    }
}