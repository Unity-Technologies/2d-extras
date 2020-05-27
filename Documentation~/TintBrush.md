# Tint Brush

This Brush changes the color of Tiles placed on a Tilemap to the color selected.

## Properties

| Property  | Function                      |
| --------- | ----------------------------- |
| __Color__ | Select the Color of the tint. |

## Usage

Select the color to tint a Tile with in the Brush properties. Then use the Paint tool with the Brush to change the color of the Tiles you paint over.

![Scene View with Tint Brush](images/TintBrush.png)

## Implementation

The TintBrush inherits from the GridBrush and implements the following overrides:

- It overrides the Paint method to set the color of a Tile. 
- It overrides the Erase method to be able to set the color of a Tile back to the default white color.