using System.IO;
using UnityEditor.Tilemaps;

namespace UnityEditor
{
    internal static class CustomRuleTileMenu
    {
        private const string customRuleTileScript =
            @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class #SCRIPTNAME# : RuleTile<#SCRIPTNAME#.Neighbor> {
    public bool customField;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
}";

        private static string tempCustomRuleTilePath;

        [MenuItem("Assets/Create/2D/Tiles/Custom Rule Tile Script", false, (int)ETilesMenuItemOrder.CustomRuleTile)]
        private static void CreateCustomRuleTile()
        {
            if (string.IsNullOrEmpty(tempCustomRuleTilePath) || !File.Exists(tempCustomRuleTilePath))
                tempCustomRuleTilePath = FileUtil.GetUniqueTempPathInProject();
            File.WriteAllText(tempCustomRuleTilePath, customRuleTileScript);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(tempCustomRuleTilePath, "NewCustomRuleTile.cs");
        }
    }
}
