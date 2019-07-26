# Custom Rules for Rule Tile 

###### *Contribution by: johnsoncodehk*

This helps to create new custom Rule Tile with new matching options, instead of the default options: Don't Care, This and Not This for the default Rule Tile. Using this will create clickable options for each Rule in your custom Rule Tile.

### Features

- Inheritable RuleTile
- Customizable attributes
- Can expand or rewrite neighbor rules and GUI display
- Can be used by RuleOverrideTile
- Template script (Menu: Assets/Create/Custom Rule Tile Script)
- Neighbor rules tooltip
- Backward compatible

### Usage

From the Assets menu, select Create/Custom Rule Tile Script. This will prompt you to create a new file with a name. After creating the file, you can edit it to add new matching options and the algorithm for testing matches.

### Examples

- Custom Attributes:

```csharp
public class MyTile : RuleTile {
	public string tileId;
	public bool isWater;
}
```

- Custom Rules:

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

- Expansion Rules

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