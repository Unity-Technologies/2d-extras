using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    static internal partial class AssetCreation
    {
        [MenuItem("Assets/Create/2D/Tiles/Auto Tile", priority = (int)ETilesMenuItemOrder.AutoTile)]
        static void CreateAutoTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<AutoTile>(), "New Auto Tile.asset");
        }
    }
}