# Pipeline Tile

A Pipeline Tile take into consideration its orthogonal neighboring tiles and displays a Sprite depending on whether the neighboring Tile is the same Tile as itself, and the number of Tiles bordering it.

## Properties

| Property  | Function                                                     |
| --------- | ------------------------------------------------------------ |
| __None__  | Displays this selected Sprite when no other Tiles border the Tile. |
| __One__   | Displays this selected Sprite when one Tile borders the Tile. |
| __Two__   | Displays this selected Sprite when two Tiles border the Tile. |
| __Three__ | Displays this selected Sprite when three Tiles border the Tile. |
| __Four__  | Displays this selected Sprite when four Tiles border the Tile. |

## Usage

Set up the Pipeline Tile by selecting the Sprites that are displayed depending on the number of Tiles bordering the Sprite.

![Pipeline Tile Editor](images/PipelineTileEditor.png)

Then paint the Pipeline Tile onto the Tilemap with the [Tile Palette](https://docs.unity3d.com/Manual/Tilemap-Palette.html) tools.

![Scene View with Pipeline Tile](images/PipelineTile.png)