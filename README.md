# 2d-extras

2d-extras is a repository containing helpful reusable scripts which you can use to make your games, with a slant towards 2D. Feel free to customise the behavior of the scripts to create new tools for your use case! 

Implemented examples using these scripts can be found in the sister repository [2d-techdemos](https://github.com/Unity-Technologies/2d-techdemos "2d-techdemos: Examples for 2d features").

All items in the repository are grouped by use for a feature and are listed below.

### Tilemap

For use with Unity `2019.1.0f2` onwards.

Please use the `2018.3` branch for Unity 2018.3 versions. 

Please use the `2018.2` branch for Unity 2018.2 versions. 

Please use the `2017` branch for earlier versions of Unity (from 2017.2 and up).

Please use the `package` branch for Unity `2018.3.0f2` but as a Unity Package instead of a Unity Project. View the README there for more details.

##### Brushes

- **Coordinate**: This Brush displays the cell coordinates it is targeting in the SceneView. Use this as an example to create brushes which have extra visualization features when painting onto a Tilemap.
- **Line**: This Brush helps draw lines of Tiles onto a Tilemap. The first click of the mouse sets the starting point of the line and the second click sets the ending point of the line and draws the lines of Tiles. Use this as an example to modify brush painting behaviour to making painting quicker with less actions.
- **Random**: This Brush helps to place random Tiles onto a Tilemap. Use this as an example to create brushes which store specific data per brush and to make brushes which randomize behaviour.
- **Prefab**: This Brush instances and places a randomly selected Prefabs onto the targeted location and parents the instanced object to the paint target. Use this as an example to quickly place an assorted type of GameObjects onto structured locations.
- **GameObject**: This Brush instances, places and manipulates GameObjects onto the scene. Use this as an example to create brushes which targets objects other than tiles for manipulation.
- **TintBrush**: Brush to edit Tilemap per-cell tint colors.
- **TintBrushSmooth**: Advanced tint brush for interpolated tint color per-cell. Requires the use of custom shader (see TintedTilemap.shader) and helper component TileTextureGenerator.
- **Group**: This Brush helps to pick Tiles which are grouped together by position. Gaps can be set to identify if Tiles belong to a Group. Limits can be set to ensure that an over-sized Group will not be picked. Use this as an example to create brushes that have the ability to choose and pick whichever Tiles it is interested in. 

##### Tiles

- **Animated**: Animated Tiles are tiles which run through and display a list of sprites in sequence.
- **Pipeline**: Pipeline Tiles are tiles which take into consideration its orthogonal neighboring tiles and displays a sprite depending on whether the neighboring tile is the same tile.
- **Random**: Random Tiles are tiles which pseudo-randomly pick a sprite from a given list of sprites and a target location, and displays that sprite.
- **Terrain**: Terrain Tiles, similar to Pipeline Tiles, are tiles which take into consideration its orthogonal and diagonal neighboring tiles and displays a sprite depending on whether the neighboring tile is the same tile.
- **RuleTile**: Generic visual tile for creating different tilesets like terrain, pipeline, random or animated tiles.
- **Hexagonal Rule Tile**: A Rule Tile for use with Hexagonal Grids. Enable Flat Top for Flat Top Hexagonal Grids and disable for Pointed Top Hexagonal Grids.
- **Isometric Rule Tile**: A Rule Tile for use with Isometric Grids.
- **RuleOverrideTile**: Rule Override Tiles are Tiles which can override a subset of Rules for a given Rule Tile to provide specialised behaviour while keeping most of the Rules originally set in the Rule Tile.
- **Weighted Random**: Weighted Random Tiles are tiles which randomly pick a sprite from a given list of sprites and a target location, and displays that sprite. The sprites can be weighted with a value to change its probability of appearing.

##### Other

- **GridInformation**: A simple MonoBehaviour that stores and provides information based on Grid positions and keywords.
- **Custom Rules for RuleTile**: This helps to create new custom Rules for the Rule Tile. Check the [Wiki](https://github.com/Unity-Technologies/2d-extras/wiki) for more information on how to use this.