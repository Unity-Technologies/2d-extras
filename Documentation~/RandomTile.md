# Random Tile

Random Tiles are Tiles which pick a Sprite from a given list of Sprites and a target location pseudo-randomly, and then displays that Sprite. The Sprite displayed for the Tile is randomized based on its location and remains fixed for that specific location.

### Properties

| Property              | Function                                                     |
| --------------------- | ------------------------------------------------------------ |
| __Number of Sprites__ | Set the number of Sprites to randomize from here.            |
| __Sprite *__          | Select the Sprite the Tile can randomly place.               |
| __Color__             | Set the color of the Tile.                                   |
| __Collider Type__     | Select the type of [Collider Shape](https://docs.unity3d.com/Manual/class-TilemapCollider2D.html) the Tile will generate. |

### Usage

First set the size of the list of Sprites to choose from by setting the value to the __Number of Sprites__ property in the Random Tile editor window. Then select the Sprites that the Tiles chooses from for each Sprite entry.

![Random Tile Editor](images/RandomTileEditor.png)

Paint the Random Tile onto the Tilemap with the [Tile Palette](https://docs.unity3d.com/Manual/Tilemap-Painting.html) tools.

![Scene View with Random Tile](images/RandomTile.png)