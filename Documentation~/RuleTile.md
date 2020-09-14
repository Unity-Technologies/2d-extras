#  Rule Tile

__Contributions by:__ [johnsoncodehk](https://github.com/johnsoncodehk), [DreadBoy](https://github.com/DreadBoy), [AVChemodanov](https://github.com/AVChemodanov), [DoctorShinobi](https://github.com/DoctorShinobi)

This is a generic visual Tile that other Tiles such as Terrain Tiles, Pipeline Tile, Random Tile or [Animated Tiles](AnimatedTile.md) are based on. The Rule Tile is for the Rectangle Grid type, the Hexagonal Rule Tile is for the Hexagonal Grid type, and the Isometric Rule Tile is for the Isometric Grid types. All the different types of Rule Tiles possess the same properties.

## Properties

| Property                | Function                                                |
| ----------------------- | ------------------------------------------------------- |
| __Default Sprite__      | The default Sprite set when creating a new Rule.        |
| __Default GameObject__ | The default GameObject set when creating a new Rule.    |
| __Default Collider__    | The default Collider Type set when creating a new Rule. |

### Tiling Rules

| Property        | Function                                            |
| --------------- | --------------------------------------------------- |
| __Rule__        | The Rule Type for this Rule.                        |
| __Game Object__ | The GameObject for the Tile which fits this Rule.   |
| __Collider__    | The Collider Type for the Tile which fits this Rule |
| __Output__      | The Output for the Tile which fits this Rule.       |

### Output: Fixed

| Property   | Function                                      |
| ---------- | --------------------------------------------- |
| __Sprite__ | The Sprite for the Tile which fits this Rule. |

### Output: Random

| Property    | Function                                                     |
| ----------- | ------------------------------------------------------------ |
| __Noise__   | The Perlin Noise factor when placing the Tile.               |
| __Shuffle__ | The randomized transform given to the Tile when placing it.  |
| __Size__    | The number of Sprites to randomize from.                     |
| __Sprite__  | The Sprite for the Tile which fits this Rule. A random Sprite will be chosen out of this when placing the Tile. |

### Output: Animation

| Property   | Function                                                     |
| ---------- | ------------------------------------------------------------ |
| __Speed__  | The speed at which the animation is played.                  |
| __Size__   | The number of Sprites in the animation.                      |
| __Sprite__ | The Sprite for the Tile which fits this Rule. Sprites will be shown in sequence based on the order of the list. |

## Editor Properties

| Property                | Function                                                |
| ----------------------- | ------------------------------------------------------- |
| __Extend Neighbor__     | Enabling this allows you to increase the range of neighbors beyond the 3x3 box.        |


## <a name="Usage"></a>Usage

Set up the Rule Tile with the required rules with the __Rule Tile editor__. In the Rule Tile editor, you can change, add or remove Rules in the Tiling Rules list. Click on the + or - buttons to add or remove Rules. Select and hold the top left corner of each row to drag them up or down to change the order of the Rules in the list.

![Rule Tile Editor](images/RuleTileEditor.png)

When you add a new Rule, the Rule editor displays the following: the list of Rule properties, a 3x3 box that visualizes the Rules set, and a Sprite selector.  

![Rule Editor](images/RuleTileRule.png)

Edit the 3x3 box to set up the Rule the Tile must match. The 3x3 box represents the neighbors a Tile can have, where the center represents the Tile itself, and the eight bordering cells are the neighboring Tiles in those positions relative to the Tile. Each of the neighboring cells can be set with one of three options: Don't Care, This and Not This. These define the behavior of the Rule Tile towards these Tiles.

| Options        | Rule Tile behavior                                           |
| -------------- | ------------------------------------------------------------ |
| __Don't Care__ | The Rule Tile ignores the contents in this cell.             |
| __This__       | The Rule Tile checks if the contents of this cell is an instance of this Rule Tile. If it is an instance, the rule passes. If it is not an instance, the rule fails. |
| __Not This__   | The Rule Tile checks if the contents of this cell is not an instance of this Rule Tile. If it is not an instance, the rule passes. If it is an instance, the rule fails. |

If all of the neighbors of the Rule Tile match the options set for their respective directions, then the Rule is considered matched and the rest of the Rule properties are applied.

When the Rule is set to Fixed, the Rule will only match exactly the conditions set for its neighbours. The example below will only match if there are 

![Rule Tile with Fixed Rule](images/RuleTileRuleFixed.png)

When the Rule is set to Rotated, the 3x3 box will be rotated 90 degrees each time the Rule fails to match and it will try to match again with this rotated 3x3 box. If the Rule now matches, the contents of this Rule will be applied as well as the rotation required to match the Rule. Use this if you want the Rule to match for the four 90 degree rotations if rotation is possible.

![Rule Tile with Rotated Rule](images/RuleTileRuleRotated.png)

When the Rule is set to Mirror X, Mirror Y or Mirror XY, the 3x3 box will be mirrored in that axis each time the Rule fails to match and it will try to match again with this mirrored 3x3 box. If the Rule now matches, the contents of this Rule will be applied as well as the mirroring required to match the Rule. Use this if you want the Rule to match for the mirrored locations if mirroring is possible.

![Rule Tile with Mirror XY Rule](images/RuleTileRuleMirror.png)

If you want the Rule Tile to have a Random output, you can set the Output to Random. This will allow you to specify a number of input Sprites to randomize from. The rotation of the Sprites can be randomized as well by changing the __Shuffle__ property.

![Rule Tile with Random Output](images/RuleTileOutputRandom.png)

If you want the Rule Tile to output a Sprite Animation, you can set the Output to Animation. This will allow you to specify a number of Sprites to animate sequentially. The speed of the Animation can be randomized as well by changing the __Speed__ property.

![Rule Tile with Animation Output](images/RuleTileOutputAnimation.png)

When <b>Extend Neighbors</b> is enabled, the 3x3 box can be extended to allow for more specific neighbor matching. The Transform rule matching (eg. Rotated, Mirror) will apply for the extended neighbors set.

![Rule Tile with Animation Output](images/RuleTileRuleExtendNeighbor.png)

Paint the Rule Tile using the Tile Palette tools.

![Scene View with Rule Tile](images/RuleTile.png)

For optimization, please set the most common Rule at the top of the list of Rules and follow with next most common Rule and so on. When matching Rules during the placement of the Tile, the Rule Tile algorithm will check the first Rule first, before proceeding with the next Rules.