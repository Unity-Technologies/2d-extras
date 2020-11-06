# Prefab Brush

__Contributions by:__  [Pepperized](https://github.com/Pepperized), [superkerokero](https://github.com/superkerokero), [vladderb](https://github.com/vladderb), [RyotaMurohoshi](https://github.com/RyotaMurohoshi), [ManickYoj](https://github.com/ManickYoj), [Quickz](https://github.com/Quickz)

This Brush instances and places randomly selected Prefabs onto the targeted location and parents the instanced object to the paint target. Use this Brush as an example to create Brushes which can quickly place assorted types of GameObjects onto structured locations.

## Properties

| Property         | Function                                                     |
| ---------------- | ------------------------------------------------------------ |
| __Perlin Scale__ | Factor for the distribution of Prefabs chosen when painting. |
| __Prefabs__      | Set the number and selection of Prefabs to paint from here.  |

## Usage

First set the number of Prefabs to select from in the __Prefabs__ property, then add Prefab Assets to the list. Adjust the __Perlin Scale__ property to adjust the distribution of Prefabs painted on a particular cell. 

![Brush Editor with Prefab Brush](images/PrefabBrushEditor.png)

When painting with the Prefab Brush, the Prefab Brush will pick from the available Prefab selection based on the __Perlin Scale__ and instantiate it to the Scene.

![Scene View with Prefab Brush](images/PrefabBrush.png)

## Implementation

The PrefabBrush inherits from the GridBrush and implements the following overrides:

- It overrides the Paint method to paint a Prefab from the Prefab selection. 
- It overrides the Erase method to be able to erase the instantiated Prefabs or other GameObjects from the Scene.