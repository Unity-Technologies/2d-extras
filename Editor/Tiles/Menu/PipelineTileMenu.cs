using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    static internal partial class AssetCreation
    {
        [MenuItem("Assets/Create/2D/Tiles/Pipeline Tile", priority = (int)ETilesMenuItemOrder.PipelineTile)]
        static void CreatePipelineTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<PipelineTile>(), "New Pipeline Tile.asset");
        }
    }
}