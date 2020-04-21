# Tint Brush (Smooth)

This advanced Tint Brush interpolates tint color per-cell. This requires the use of a custom shader (see TintedTilemap.shader in the package contents) and the helper component __TileTextureGenerator__.

## Properties

| Property  | Function                                                     |
| --------- | ------------------------------------------------------------ |
| __Color__ | Select the color of the tint.                                |
| __Blend__ | Factor to blend the color of a Tile with this Brush's __Color__. |

## Usage

First set the [Tilemap Renderer](https://docs.unity3d.com/Manual/class-TilemapRenderer.html)'s __Material__ to the TintedTilemap.shader. Add the __Tile Texture Generator__ component to the Tilemap Renderer as well. Then select the desired tint color in the __Color__ property or pick the color from an existing Tile.

Adjust the __Blend__ property to blend the chosen color with the original color of the Tile. A lower value weighs the new color with the existing color of the Tile, while a higher value weighs the new color with the color of the Brush.

With the Brush selected, paint Tiles with the [Paintbbrush tool](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Brush) to change their color.

![Scene View with Tint Brush (Smooth)](images/TintBrushSmooth.png)

## Implementation

The TintBrushSmooth inherits from the GridBrushBase and implements the following overrides:

- It overrides the Paint method to set the color of a Tile. 
- It overrides the Erase method to be able to set the color of a Tile back to the default white color. 
- It overrides the Pick method to pick the color of a Tile.