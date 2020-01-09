# Grid Information

A simple Component that stores and provides information based on Grid positions and keywords.



### Usage

Add this Component to a GameObject with a GridLayout Component.



To store information on to the GridInformation Component, use the following APIs:

```C#
public bool SetPositionProperty(Vector3Int position, String name, int positionProperty)

public bool SetPositionProperty(Vector3Int position, String name, string positionProperty)

public bool SetPositionProperty(Vector3Int position, String name, float positionProperty)

public bool SetPositionProperty(Vector3Int position, String name, double positionProperty)

public bool SetPositionProperty(Vector3Int position, String name, UnityEngine.Object positionProperty)

public bool SetPositionProperty(Vector3Int position, String name, Color positionProperty)
```



To get information from the GridInformation Component, use the following APIs:

```C#
public T GetPositionProperty<T>(Vector3Int position, String name, T defaultValue) where T : UnityEngine.Object

public int GetPositionProperty(Vector3Int position, String name, int defaultValue)
    
public string GetPositionProperty(Vector3Int position, String name, string defaultValue)
    
public float GetPositionProperty(Vector3Int position, String name, float defaultValue)
    
public double GetPositionProperty(Vector3Int position, String name, double defaultValue)
    
public Color GetPositionProperty(Vector3Int position, String name, Color defaultValue)
```



You can use this in combination with Scriptable Tiles to get the right Tile Data when laying out your Tilemap.