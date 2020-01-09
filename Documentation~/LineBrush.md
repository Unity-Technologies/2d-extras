# Line Brush

###### *Contribution by:  CraigGraff*

This Brush helps draw lines of Tiles onto a Tilemap. Use this as an example to modify brush painting behaviour to making painting quicker with less actions.

### Properties

| Property              | Function                                                     |
| --------------------- | ------------------------------------------------------------ |
| __Line Start Active__ | Whether the Line Brush has started drawing a line.           |
| __Fill Gaps__         | Ensures that there are orthogonal connections of Tiles from the start of the line to the end. |
| __Line Start__        | The current starting point of the line.                      |

### Usage
The first click of the mouse sets the starting point of the line and the second click sets the ending point of the line and draws the lines of Tiles. When active, a blue outline will indicate the starting point of the line.

![Scene View with Line Brush](images/LineBrush.png)

If you want to have Tiles which are orthogonally connected from start to end, you can enable the __Fill Gaps__ property in the Brush Editor.

![Scene View with Line Brush with Fill Gaps](images/LineBrushFillGaps.png)

### Implementation

The LineBrush inherits from the GridBrush and overrides the Paint method to have the line painting functionality.