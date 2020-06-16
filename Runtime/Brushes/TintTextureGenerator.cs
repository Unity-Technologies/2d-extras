using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A helper class for tinting a Grid component
/// </summary>
[ExecuteInEditMode]
public class TintTextureGenerator : MonoBehaviour
{
    /// <summary>
    /// Size of the Tint map in cells
    /// </summary>
    public int k_TintMapSize = 256;

    /// <summary>
    /// Callback when the TintTextureGenerator is loaded.
    /// Refreshes the Grid Component on this GameObject. 
    /// </summary>
    public void Start()
    {
        Refresh(GetComponent<Grid>());
    }

    private Texture2D m_TintTexture;
    private Texture2D tintTexture
    {
        get
        {
            if (m_TintTexture == null)
            {
                m_TintTexture = new Texture2D(k_TintMapSize, k_TintMapSize, TextureFormat.ARGB32, false);
                m_TintTexture.hideFlags = HideFlags.HideAndDontSave;
                m_TintTexture.wrapMode = TextureWrapMode.Clamp;
                m_TintTexture.filterMode = FilterMode.Bilinear;
                RefreshGlobalShaderValues();
            }
            return m_TintTexture;
        }
    }

    /// <summary>
    /// Refreshes the tint color of the Grid
    /// </summary>
    /// <param name="grid">Grid to refresh color</param>
    public void Refresh(Grid grid)
    {
        if (grid == null)
            return;

        int w = tintTexture.width;
        int h = tintTexture.height;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Vector3Int world = TextureToWorld(new Vector3Int(x, y, 0));
                tintTexture.SetPixel(x, y, GetGridInformation(grid).GetPositionProperty(world, "Tint", Color.white));
            }
        }
        tintTexture.Apply();
    }

    /// <summary>
    /// Refreshes the color of a position on a Grid
    /// </summary>
    /// <param name="grid">Grid to refresh color</param>
    /// <param name="position">Position of the Grid to refresh color</param>
    public void Refresh(Grid grid, Vector3Int position)
    {
        if (grid == null)
            return;

        RefreshGlobalShaderValues();
        Vector3Int texPosition = WorldToTexture(position);
        tintTexture.SetPixel(texPosition.x, texPosition.y, GetGridInformation(grid).GetPositionProperty(position, "Tint", Color.white));
        tintTexture.Apply();
    }

    /// <summary>
    /// Get the color of a position on a Grid
    /// </summary>
    /// <param name="grid">Grid to get color from</param>
    /// <param name="position">Position of the Grid to get color from</param>
    /// <returns>Color of a position on a Grid</returns>
    public Color GetColor(Grid grid, Vector3Int position)
    {
        if (grid == null)
            return Color.white;

        return GetGridInformation(grid).GetPositionProperty(position, "Tint", Color.white);
    }

    /// <summary>
    /// Set the color of a position on a Grid
    /// </summary>
    /// <param name="grid">Grid to set color to</param>
    /// <param name="position">Position of the Grid to set color to</param>
    /// <param name="color">Color to set to</param>
    public void SetColor(Grid grid, Vector3Int position, Color color)
    {
        if (grid == null)
            return;

        GetGridInformation(grid).SetPositionProperty(position, "Tint", color);
        Refresh(grid, position);
    }
    
    Vector3Int WorldToTexture(Vector3Int world)
    {
        return new Vector3Int(world.x + tintTexture.width / 2, world.y + tintTexture.height / 2, 0);
    }

    Vector3Int TextureToWorld(Vector3Int texpos)
    {
        return new Vector3Int(texpos.x - tintTexture.width / 2, texpos.y - tintTexture.height / 2, 0);
    }

    GridInformation GetGridInformation(Grid grid)
    {
        GridInformation gridInformation = grid.GetComponent<GridInformation>();

        if (gridInformation == null)
            gridInformation = grid.gameObject.AddComponent<GridInformation>();

        return gridInformation;
    }

    void RefreshGlobalShaderValues()
    {
        Shader.SetGlobalTexture("_TintMap", m_TintTexture);
        Shader.SetGlobalFloat("_TintMapSize", k_TintMapSize);
    }
}
