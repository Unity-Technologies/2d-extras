# Notice

## No more development on 2d-extras GitHub repository

We are changing how we develop additional 2D Tiles and Brushes for 2D Tilemap. An important part of this change involves ensuring a single point of entry for users to get the latest tiles and brushes for Tilemap. To this end we created the Tilemap Extras package which is accessible from Package Manager. Moving forward, the 2D-extras GitHub repository will be made read-only. You will still be able to clone and pull the contents of the repository as it is right now.

Tilemap Extras already includes Animated Tiles, RuleTile (Rectangular Isometric and Hexagonal), the Rule Override Tiles, Group Brush, Line Brush, Random Brush and GameObject Brush. It also includes 3 samples covering common use-cases of Rule Tiles and Animated Tiles. Read more about Tilemap Extras in the announcement thread.

If there is functionality in 2d-extras that is important to your team and your product please let us know in this thread or start a discussion in the 2D forums and we will consider adding it to the Tilemap Extras package.

For issues with the Tilemap Extras please use the standard Unity channels via the Unity Bug Reporter and/or starting a thread in our forums instead of the GitHub Repository Issues tracker.

Thank you for all your contributions over the years. As always, let us know about your needs and project plans and we will endeavor to make Tilemap Extras an even better tool for developing 2D games in Unity!


# 2d-extras

2d-extras is a repository containing helpful reusable scripts which you can use to make your games, with a slant towards 2D. Feel free to customise the behavior of the scripts to create new tools for your use case! 

Implemented examples using these scripts can be found in the sister repository [2d-techdemos](https://github.com/Unity-Technologies/2d-techdemos "2d-techdemos: Examples for 2d features").

All items in the repository are grouped by use for a feature and are listed below.

## How to use this

You can use this in two different ways: downloading this repository or adding it to your project's Package Manager manifest.

Alternatively, you can pick and choose the scripts that you want by placing only these scripts in your project's `Assets` folder.

### Download

#### Setup
Download or clone this repository into your project in the folder `Packages/com.unity.2d.tilemap.extras`.

### Package Manager Manifest

#### Requirements
[Git](https://git-scm.com/) must be installed and added to your path.

#### Setup
The following line needs to be added to your `Packages/manifest.json` file in your Unity Project under the `dependencies` section:

```json
"com.unity.2d.tilemap.extras": "https://github.com/Unity-Technologies/2d-extras.git#2020.3"
```

### Tilemap

For use with Unity `2020.1.1f1` onwards. 

Please use the `1.5.0-preview` tag for Unity 2019.2-2019.4 versions.

Please use the `2019.1` tag for Unity 2019.1 versions. 

Please use the `2018.3` branch or the `2018.3` tag for Unity 2018.3-2018.4 versions. 

Please use the `2018.2` branch or the `2018.2` tag for Unity 2018.2 versions. 

Please use the `2017` branch or the `2017` tag for earlier versions of Unity (from 2017.2 and up).

##### Brushes

- **Coordinate**: This Brush displays the cell coordinates it is targeting in the SceneView. Use this as an example to create brushes which have extra visualization features when painting onto a Tilemap.
- **Line**: This Brush helps draw lines of Tiles onto a Tilemap. The first click of the mouse sets the starting point of the line and the second click sets the ending point of the line and draws the lines of Tiles. Use this as an example to modify brush painting behaviour to making painting quicker with less actions.
- **Random**: This Brush helps to place random Tiles onto a Tilemap. Use this as an example to create brushes which store specific data per brush and to make brushes which randomize behaviour.
- **Prefab**: This Brush instances and places the containing Prefab onto the targeted location and parents the instanced object to the paint target. Use this as an example to quickly place an assorted type of GameObjects onto structured locations.
- **PrefabRandom**: This Brush instances and places a randomly selected Prefabs onto the targeted location and parents the instanced object to the paint target. Use this as an example to quickly place an assorted type of GameObjects onto structured locations.
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
- **Custom Rules for RuleTile**: This helps to create new custom Rules for the Rule Tile. Check the [Wiki](https://github.com/Unity-Technologies/2d-extras/wiki) or this great [video](https://youtu.be/FwOxLkJTXag) for more information on how to use this!

[![How to make Custom Rule Tiles in Unity Video](http://img.youtube.com/vi/FwOxLkJTXag/0.jpg)](http://www.youtube.com/watch?v=FwOxLkJTXag "How to make Custom Rule Tiles in Unity")

