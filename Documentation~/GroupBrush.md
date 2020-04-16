# Group Brush

Use this Brush to pick Tiles which are grouped together by position. Set the __Gap__ value to identify which Tiles to a Group, and set the __Limit__ value to ensure that the picked group remains within the desired size. Use this Brush as an example to create Brushes that have the ability to choose and pick specific Tiles 

## Properties

| Property  | Function                                                     |
| --------- | ------------------------------------------------------------ |
| __Gap__   | The gap in cell count before stopping to consider a Tile in a Group |
| __Limit__ | The count in cells beyond the initial position before stopping to consider a Tile in a Group |

## Usage

Select the Group Brush, and use the [Picker Tool](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Picker) and pick a position on the Tilemap. The Group Brush selects a group of Tiles based on its set properties and creates a Group.

![Scene View with Group Brush](images/GroupBrush.png)

## Implementation

The Group Brush inherits from the Grid Brush. It overrides the Pick method when picking a group of Tiles based on their position and its set properties.