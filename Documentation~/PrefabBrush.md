# Prefab Brush

###### *Contributions by:  Pepperized, superkerokero*

This Brush instances and places a randomly selected Prefabs onto the targeted location and parents the instanced object to the paint target. Use this as an example to quickly place an assorted type of GameObjects onto structured locations.

### Properties

| Property         | Function                                              |
| ---------------- | ----------------------------------------------------- |
| __Perlin Scale__ | Factor for distribution of choice of Prefabs to paint |
| __Prefabs__      | The selection of Prefabs to paint from                |

### Usage

To add Prefabs to paint, change the number of Prefabs in the __Prefabs__ property and add Prefab assets to the them.

To change the random distribution of Prefabs painted, change the __Perlin Scale__ property. This will change the distribution of Prefabs painted on a particular cell.

![Brush Editor with Prefab Brush](images/PrefabBrushEditor.png)

When painting with the Prefab Brush, the Prefab Brush will pick from the available Prefab selection based on the __Perlin Scale__ and instantiate it to the Scene.

![Scene View with Prefab Brush](images/PrefabBrush.png)

### Implementation

The PrefabBrush inherits from the GridBrush. It overrides the Paint method to paint a Prefab from the Prefab selection. It overrides the Erase method to be able to erase the instantiated Prefabs or other GameObjects from the Scene.