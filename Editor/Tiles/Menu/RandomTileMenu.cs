using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    internal static partial class AssetCreation
    {
        [MenuItem("Assets/Create/2D/Tiles/Random Tile", priority = (int)ETilesMenuItemOrder.RandomTile)]
        private static void CreateRandomTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<RandomTile>(), "New Random Tile.asset");
        }
    }
}