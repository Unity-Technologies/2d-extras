# Random Brush

This Brush helps to place random Tiles onto a Tilemap. Use this as an example to create brushes which store specific data per brush and to make brushes which randomize behaviour.

### Properties

| Property                | Function                                                     |
| ----------------------- | ------------------------------------------------------------ |
| __Pick Random Tiles__   | When picking, pick the Tiles from the selection as a random Tile set. |
| __Add To Random Tiles__ | When picking, add picked Tile sets to existing Tile sets instead of replacing existing Tile sets. |
| __Tile Set Size__       | The size of Tile sets.                                       |
| __Number of Tiles__     | The number of Tile sets.                                     |
| __Tile Set__            | A Tile set to randomize from                                 |
| __Tiles__               | The Tiles in the Tile Set.                                   |

### Usage

To create Tile sets, you will need to define the size of the Tile set you want to paint with using the Tile Set Size property. After that, you can add them manually with the Brush Editor or select them from an existing Tile Palette.

To select them from an existing Tile Palette, enable the __Pick Random Tiles__ property and select the Tile Sets using the Pick Tool. This will create a Tile set in the Brush Editor or sets if the Picking size is larger than the Tile Set size. Enable the __Add To Random Tiles__ property to add on the new Tile set/s to existing Tile sets.

![Brush Editor with Random Brush](images/RandomBrushTileSet.png)

When painting with the Random Brush, the Random Brush will randomly pick from the available Tile sets and fill it 

![Scene View with Random Brush](images/RandomBrush.png)

### Implementation

The RandomBrush inherits from the GridBrush. It overrides the Paint method to paint random selections of Tiles from chosen Tile sets. It overrides the Pick method to be able to pick selections of Tiles for the random Tile sets.