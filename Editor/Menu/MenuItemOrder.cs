using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    internal enum ETilesMenuItemOrder
    {
        AnimatedTile = 2,
        RuleTile = 100,
        IsometricRuleTile,
        HexagonalRuleTile,
        RuleOverrideTile,
        AdvanceRuleOverrideTile,
        CustomRuleTile,
        RandomTile = 200,
        WeightedRandomTile,
        PipelineTile,
        TerrainTile,
    }
    internal enum EBrushMenuItemOrder
    {
        RandomBrush = 3,
        PrefabBrush,
        PrefabRandomBrush
    }
    
    static internal partial class AssetCreation
    {
        
        [MenuItem("Assets/Create/2D/Tiles/Animated Tile",  priority = (int) ETilesMenuItemOrder.AnimatedTile)]
        static void CreateAnimatedTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<AnimatedTile>(), "New Animated Tile.asset");
        }
        
        [MenuItem( "Assets/Create/2D/Tiles/Hexagonal Rule Tile", priority = (int)ETilesMenuItemOrder.HexagonalRuleTile)]
        static void CreateHexagonalRuleTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<HexagonalRuleTile>(), "New Hexagonal Rule Tile.asset");
        }

        [MenuItem("Assets/Create/2D/Tiles/Isometric Rule Tile", priority = (int)ETilesMenuItemOrder.IsometricRuleTile)]
        static void CreateIsometricRuleTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<IsometricRuleTile>(), "New Isometric Rule Tile.asset");
        }

        [MenuItem("Assets/Create/2D/Tiles/Advanced Rule Override Tile", priority = (int)ETilesMenuItemOrder.AdvanceRuleOverrideTile)]
        static void CreateAdvancedRuleOverrideTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<AdvancedRuleOverrideTile>(), "New Advanced Rule Override Tile.asset");
        }

        [MenuItem("Assets/Create/Assets/Create/2D/Tiles/Rule Override Tile", priority = (int)ETilesMenuItemOrder.RuleOverrideTile)]
        static void CreateRuleOverrideTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<RuleOverrideTile>(), "New Rule Override Tile.asset");
        }
            
        [MenuItem("Assets/Create/2D/Tiles/Rule Tile", priority = (int)ETilesMenuItemOrder.RuleTile)]
        static void CreateRuleTile()
        {
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<RuleTile>(), "New Rule Tile.asset");
        }
    }
}
