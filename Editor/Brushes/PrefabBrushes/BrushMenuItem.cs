using UnityEngine;

namespace UnityEditor.Tilemaps
{
    internal static partial class AssetCreation
    {
        [MenuItem("Assets/Create/2D/Brushes/Prefab Brush", priority = (int)EBrushMenuItemOrder.PrefabBrush)]
        private static void CreatePrefabBrush()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<PrefabBrush>(), "New Prefab Brush.asset");
        }

        [MenuItem("Assets/Create/2D/Brushes/Prefab Random Brush",
            priority = (int)EBrushMenuItemOrder.PrefabRandomBrush)]
        private static void CreatePrefabRandomBrush()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<PrefabRandomBrush>(),
                "New Prefab Random Brush.asset");
        }

        [MenuItem("Assets/Create/2D/Brushes/Random Brush", priority = (int)EBrushMenuItemOrder.RandomBrush)]
        private static void CreateRandomBrush()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<RandomBrush>(), "New Random Brush.asset");
        }
    }
}