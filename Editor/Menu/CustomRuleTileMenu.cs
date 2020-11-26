using System;
using System.IO;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace UnityEditor
{
    static class CustomRuleTileMenu
    {
        private static string tempCustomRuleTilePath;
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

        [MenuItem("Assets/Create/2D/Tiles/Custom Rule Tile Script", false, (int)ETilesMenuItemOrder.CustomRuleTile)]
        static void CreateCustomRuleTile()
        {
            if (String.IsNullOrEmpty(tempCustomRuleTilePath) || !File.Exists(tempCustomRuleTilePath))
                tempCustomRuleTilePath = FileUtil.GetUniqueTempPathInProject();
            File.WriteAllText(tempCustomRuleTilePath, customRuleTileScript);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(tempCustomRuleTilePath, "NewCustomRuleTile.cs");
        }
    }
}
