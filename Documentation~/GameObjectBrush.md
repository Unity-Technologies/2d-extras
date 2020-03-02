# GameObject Brush

This Brush instances, places and manipulates GameObjects onto the Scene. Use this Brush as an example for creating custom Brushes which can target and manipulate other objects beside Tiles.

## Usage

First select the GameObject Brush from the Brush drop-down menu. With the Brush selected, then select the Pick Tool from the [Tile Palette](https://docs.unity3d.com/Manual/Tilemap-Painting.html) toolbar. Use the __Select Tool__ to select GameObjects from the Scene that you want the GameObject Brush to paint with. The GameObjects must be a child of the active Grid to be selectable.

When painting with the GameObject Brush, the GameObject Brush will instantiate GameObjects picked onto the Scene.

![Scene View with GameObject Brush](images/GameObjectBrush.png)

### Implementation

The GameObjectBrush inherits from the GridBrush. It overrides the Paint method to paint a GameObject. It overrides the Erase method to be able to erase the GameObjects from the Scene. It overrides the BoxFill method to paint a GameObject in each cell defined by the Box tool. It overrides the Move methods to be able to move GameObjects in the Scene. It overrides the Flip methods to be able to flip GameObjects in the picked selection.