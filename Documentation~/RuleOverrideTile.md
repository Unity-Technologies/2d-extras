# Rule Override Tile

__Contributions by:__ [johnsoncodehk](https://github.com/johnsoncodehk), [Autofire](https://github.com/Autofire)

__Rule Override Tiles__ are Tiles which can override a subset of Rules for a given [Rule Tile](RuleTile.md) while keeping most of the others Rules originally set in the Rule Tile. This allows you to create Tiles that provide specialised behavior.

## Properties

| Property          | Function                                                     |
| ----------------- | ------------------------------------------------------------ |
| __Tile__          | The Rule Tile to override.                                   |
| __Override Self__ | Enable this so that this Tile only accepts instances of itself when matching Rules. |
| __Advanced__      | Enable Advanced Mode. Enable this if you want to specify which Rules to override. |

## Usage

First select the Rule Tile that the Rule Override Tile overrides by selecting it in the __Tile__ property. The Rule Override Tile editor then displays the different rules in the Rule Tile which you can override.

In normal mode, the editor displays all the Sprites that are used in the Rule Tile on the left. Select the override Sprites by selecting them on the right of the respective original Sprites. When the Rule Tile has a match outputting the Sprite on the left, it will instead output the Sprite specified on the right.

![Rule Override Tile Editor](images/RuleOverrideTileEditor.png)

In __Advanced__ mode, the editor displays all the Rules that are used in the Rule Tile on the left. To override a Rule, select the Enabled box and override the output by specifing the desired output instead.

![Rule Override Tile Editor in Advanced mode](images/RuleOverrideTileEditorAdvanced.png)

Paint with the Rule Override Tile using the [Tile Palette](https://docs.unity3d.com/Manual/Tilemap-Painting.html) tools.

![Scene View with Rule Override Tile](images/RuleOverrideTile.png)