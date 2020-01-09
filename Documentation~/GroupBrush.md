# Group Brush

This Brush helps to pick Tiles which are grouped together by position. Gaps can be set to identify if Tiles belong to a Group. Limits can be set to ensure that an over-sized Group will not be picked. Use this as an example to create brushes that have the ability to choose and pick whichever Tiles it is interested in. 

### Properties

| Property  | Function                                                     |
| --------- | ------------------------------------------------------------ |
| __Gap__   | The gap in cell count before stopping to consider a Tile in a Group |
| __Limit__ | The count in cells beyond the initial position before stopping to consider a Tile in a Group |

### Usage

To select a group of Tiles, select the Pick Tool and pick a position on the Tilemap. The GroupBrush will select a group of Tiles based on its properties and create a Group.

![Scene View with Group Brush](images/GroupBrush.png)

### Implementation

The GroupBrush inherits from the GridBrush. It overrides the Pick method to pick a group of Tiles based on their position.