# GameObject Brush

This Brush instances, places and manipulates GameObjects onto the scene. Use this as an example to create brushes which targets objects other than tiles for manipulation.

### Usage

To pick GameObjects, change to the Pick tool and pick GameObjects from the Scene. Note that the GameObjects must be a child of the active Grid.

When painting with the GameObject Brush, the GameObject Brush will instantiate GameObjects picked onto the Scene.

![Scene View with GameObject Brush](images/GameObjectBrush.png)

### Implementation

The GameObjectBrush inherits from the GridBrush. It overrides the Paint method to paint a GameObject. It overrides the Erase method to be able to erase the GameObjects from the Scene. It overrides the BoxFill method to paint a GameObject in each cell defined by the Box tool. It overrides the Move methods to be able to move GameObjects in the Scene. It overrides the Flip methods to be able to flip GameObjects in the picked selection.