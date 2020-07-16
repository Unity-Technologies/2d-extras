# 2D Tilemap Extras

The 2D Tilemap Extras package contains reusable 2D and __Tilemap Editor__ scripts which you can use for your own Projects. You can freely customize the behavior of the scripts to create new Brushes that suit different scenarios. 

To find the additional Brushes, open the Tile Palette window (menu: __Window > 2D > Tile Palette__) and open the Brush drop-down menu near the bottom of the editor. Select from the available Brush options for different effects.

![](images/BrushDropdown.png)

The source code for these scripts can be found in the repository [2d-extras](https://github.com/Unity-Technologies/2d-extras "2d-extras: Extras for 2d features"), and examples of the implemented scripts can be found in the sister repository [2d-techdemos](https://github.com/Unity-Technologies/2d-techdemos "2d-techdemos: Examples for 2d features").

## Brushes

- [GameObject](GameObjectBrush.md): This Brush instances, places and manipulates GameObjects onto the Scene. Use this as an example to create Brushes which targets GameObjects other than Tiles for instancing and manipulation.

- [Group](GroupBrush.md): This Brush picks groups of Tiles based on their positions relative to each other. Adjust the size of groups the Brush picks by setting the Gap and Limit properties. Use this Brush as an example to create Brushes that pick Tiles based on specific criteria.

- [Line](LineBrush.md): This Brush draws a line of Tiles between two points onto a Tilemap. Use this as an example to modify Brush painting behavior to make painting more efficient.

- [Random](RandomBrush.md): This Brush places random Tiles onto a Tilemap. Use this as an example to create Brushes which store specific data per Brush and to make Brushes which randomize behavior.

## Tiles

You can create (menu: __Create > Tiles__ ) the following additional Tile types that are included with this package.

- [Animated](AnimatedTile.md): This Tile runs through and displays a list of Sprites in sequence to create a frame-by-frame animation.
- [Rule Tile](RuleTile.md): This is a generic visual Tile that accepts rules you create with the __Tiling Rules__ to create different Tilesets. Rule Tiles are the basis of the Terrain, Pipeline, Random or Animated Tiles. This is the default Rule Tile and is only used with the Rectangle Grid type Tilemap. Use the Hexagonal and Isometric Rule Tiles instead for their respective Grid types.
- __Hexagonal Rule Tile__: A Rule Tile for [Hexagonal Grids](https://docs.unity3d.com/2018.3/Documentation/Manual/Tilemap-Hexagonal.html). Enable the ‘Flat Top’ property for a Flat Top Hexagonal Grid, or clear it for a Pointed Top Hexagonal Grid.
- __Isometric Rule Tile__: A Rule Tile for use with [Isometric Grids](https://docs.unity3d.com/2018.3/Documentation/Manual/Tilemap-Isometric-CreateIso.html).
- [Rule Override Tile](RuleOverrideTile.md): This Tile can override a subset of Rules for a given [Rule Tile](RuleTile.md) to provide specialized behavior, while keeping the rest of the original Rules intact.

## Other

- [GridInformation](GridInformation.md): A simple MonoBehavior that stores and provides information based on Grid positions and keywords.
- [Custom Rules for RuleTile](CustomRulesForRuleTile.md): This helps to create new custom Rules for the Rule Tile with more options.