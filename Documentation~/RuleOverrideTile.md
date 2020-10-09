# Rule Override Tile

__Contributions by:__ [johnsoncodehk](https://github.com/johnsoncodehk), [Autofire](https://github.com/Autofire)

__Rule Override Tiles__ are Tiles which can override a subset of Rules for a given [Rule Tile](RuleTile.md) while maintaining most of the other set Rules of the Rule Tile. This allows you to create Tiles that provide specialized behavior in specific scenarios.

## Properties

| Property          | Function                                                     |
| ----------------- | ------------------------------------------------------------ |
| __Tile__          | The Rule Tile to override.                                   |
| __Override Self__ | Enable this to have this Tile only accept instances of itself when matching Rules. |
| __Advanced__      | Enable Advanced Mode. Enable this if you want to specify which Rules to override. |

## Usage

First select the Rule Tile to be overridden in the __Tile__ property. The Rule Override Tile editor then displays the different rules in the selected Rule Tile which you can override.

In its default mode, the editor displays the original Sprites that are used in the Rule Tile in the left column. Select the Sprites that override each of the respective original Sprites on the right ‘Override’ column. When the Rule Tile has a match that would usually output the original Sprite, it will instead output the override Sprite.

![Rule Override Tile Editor](images/RuleOverrideTileEditor.png)

In __Advanced__ mode, the editor additionally displays all the Rules that are used in the Rule Tile in the ‘Original’ column along with the respective Sprites. To override a Rule, select **Enabled** and override the output by specifying the desired output and its properties.

![Rule Override Tile Editor in Advanced mode](images/RuleOverrideTileEditorAdvanced.png)

Paint with the Rule Override Tile using the [Tile Palette](https://docs.unity3d.com/Manual/Tilemap-Painting.html) tools.

![Scene View with Rule Override Tile](images/RuleOverrideTile.png)