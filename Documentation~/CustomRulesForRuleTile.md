# Custom Rules for Rule Tile

__Contribution by:__ [johnsoncodehk](https://github.com/johnsoncodehk)

Use this template script to create new custom [Rule Tiles](RuleTile.md) with matching options that differ from the Rule
Tile’s [default options](RuleTile.md#Usage) (namely **This** and **Not This**). This creates selectable options for each
Rule in your custom __Rule Tile__.

## Template features

- Inheritable Rule Tile.
- Customizable properties.
- Expand or rewrite both neighbor Rules and the GUI display of the Rules.
- Usable with by [RuleOverrideTile](RuleOverrideTile.md)
- Create from a template script.
- Neighbor Rules tooltips.
- Backward compatible.

## Creating a custom Rule Tile script

Create a Custom Rule Tile script by going to __Assets > Create > Custom Rule Tile Script__. Name the newly created file
when prompted. After creating the file, you can edit it to add new matching options and custom algorithms for testing
matches.

### Examples

- Custom properties:

```csharp
public class MyTile : RuleTile {
    public string tileId;
    public bool isWater;
}
```

- Custom rules:

```csharp
public class MyTile : RuleTile<MyTile.Neighbor> {
    public class Neighbor {
        public const int MyRule1 = 0;
        public const int MyRule2 = 1;
    }
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.MyRule1: return false;
            case Neighbor.MyRule2: return true;
        }
        return true;
    }
}
```

- Expansion rules

```csharp
public class MyTile : RuleTile<MyTile.Neighbor> {
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        // 0, 1, 2 is using in RuleTile.TilingRule.Neighbor
        public const int MyRule1 = 3;
        public const int MyRule2 = 4;
    }
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.MyRule1: return false;
            case Neighbor.MyRule2: return true;
        }
        return base.RuleMatch(neighbor, tile);
    }
}
```

- Siblings Tile 1

```csharp
public class MyTile : RuleTile<MyTile.Neighbor> {
    public List<TileBase> sibings = new List<TileBase>();
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Sibing = 3;
    }
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Sibing: return sibings.Contains(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }
}
```

- Siblings Tile 2

```csharp
public class MyTile : RuleTile<MyTile.Neighbor> {
    public int siblingGroup;
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Sibing = 3;
    }
    public override bool RuleMatch(int neighbor, TileBase tile) {
        MyTile myTile = tile as MyTile;
        switch (neighbor) {
            case Neighbor.Sibing: return myTile && myTile.siblingGroup == siblingGroup;
        }
        return base.RuleMatch(neighbor, tile);
    }
}
```