# Terrain Tile

Terrain Tiles take into consideration their orthogonal and diagonal neighbors and displays a Sprite depending on whether the neighboring Tile is the same Tile as itself. This is a similar behavior to [Pipeline Tiles](PipelineTile.md).

## Properties

![Terrain Tile editor](images/TerrainTileEditor.png)



The following properties describe the appearance of Sprites representing terrain or walls. Assign a Sprite that matches the description to each of these properties.

| Property                          | Function                                                     |
| --------------------------------- | ------------------------------------------------------------ |
| __Filled__                        | A Sprite with all sides filled.                              |
| __Three Sides__                   | A Sprite with three sides.                                   |
| __Two Sides and One Corner__      | A Sprite with two sides and one corner without adjacent sides. |
| __Two Adjacent Sides__            | A Sprite with two adjacent sides.                            |
| __Two Opposite Sides__            | A Sprite with two opposite sides across each other.          |
| __One Side and Two Corners__      | A Sprite with a single side and two corners without adjacent sides. |
| __One Side and One Lower Corner__ | A Sprite with one side and a corner in the lower half of the Sprite. |
| __One Side and One Upper Corner__ | A Sprite with one side and a corner in the upper half of the Sprite. |
| __One Side__                      | A Sprite with a single side.                                 |
| __Four Corners__                  | A Sprite with four unconnected corners without adjacent sides. |
| __Three Corners__                 | A Sprite with three corners without adjacent sides.          |
| __Two Adjacent Corners__          | A Sprite with two adjacent corners.                          |
| __Two Opposite Corners__          | A Sprite with two opposite corners across each other.        |
| __One Corner__                    | A Sprite with a single corner with no adjacent sides.        |
| __Empty__                         | A Sprite without any terrain.                                |

## Usage

Set up a Terrain Tile by select Sprites which fit the characteristics stated on the left of the Terrain Tile editor. As you paint with the Terrain Tile using the [Tile Palette](https://docs.unity3d.com/Manual/Tilemap-Palette.html) tools, the Tile Sprite automatically adjusts to the appropriate one relative to its position with neighboring Tiles.

![Scene View with Terrain Tile](images/TerrainTile.png)