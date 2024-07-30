using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
///     A helper class for tinting a Grid component
/// </summary>
[ExecuteInEditMode]
public class TintTextureGenerator : MonoBehaviour
{
    /// <summary>
    ///     Size of the Tint map in cells
    /// </summary>
    public int k_TintMapSize = 256;

    /// <summary>
    ///     Mapping scale for cells to Tint map texture
    /// </summary>
    /// ///
    /// <remarks>
    ///     Adjust to get better definition.
    /// </remarks>
    public int k_ScaleFactor = 1;

    private Grid m_Grid;

    private Texture2D m_TintTexture;

    /// <summary>
    ///     Size of the Tint map texture in pixels
    /// </summary>
    public int k_TintMapTextureSize => k_TintMapSize * k_ScaleFactor;

    /// <summary>
    ///     Tint texture generated from Grid values
    /// </summary>
    public Texture2D tintTexture
    {
        get
        {
            if (m_TintTexture == null || m_TintTexture.width != k_TintMapTextureSize)
            {
                m_TintTexture = new Texture2D(k_TintMapTextureSize, k_TintMapTextureSize, TextureFormat.ARGB32, false);
                m_TintTexture.hideFlags = HideFlags.HideAndDontSave;
                m_TintTexture.wrapMode = TextureWrapMode.Clamp;
                m_TintTexture.filterMode = FilterMode.Bilinear;
                RefreshGlobalShaderValues();
                Refresh(m_Grid);
            }

            return m_TintTexture;
        }
    }


    /// <summary>
    ///     Callback when the TintTextureGenerator is loaded.
    ///     Refreshes the Grid Component on this GameObject.
    /// </summary>
    public void Start()
    {
        m_Grid = GetComponent<Grid>();
        Refresh(m_Grid);
    }

    /// <summary>
    ///     Refreshes the tint color of the Grid
    /// </summary>
    /// <param name="grid">Grid to refresh color</param>
    public void Refresh(Grid grid)
    {
        if (grid == null)
            return;

        var gi = GetGridInformation(grid);
        var w = tintTexture.width;
        var h = tintTexture.height;
        for (var y = 0; y < h; y++)
        for (var x = 0; x < w; x++)
        {
            var worldPos = TextureToWorld(new Vector3Int(x, y, 0));
            var cellPos = grid.WorldToCell(worldPos);
            tintTexture.SetPixel(x, y, gi.GetPositionProperty(cellPos, "Tint", Color.white));
        }

        tintTexture.Apply();
    }

    /// <summary>
    ///     Refreshes the color of a position on a Grid
    /// </summary>
    /// <param name="grid">Grid to refresh color</param>
    /// <param name="position">Position of the Grid to refresh color</param>
    public void Refresh(Grid grid, Vector3Int position)
    {
        if (grid == null)
            return;

        RefreshGlobalShaderValues();
        var worldPosition = grid.CellToWorld(position);
        var texPosition = WorldToTexture(worldPosition);
        var color = GetGridInformation(grid).GetPositionProperty(position, "Tint", Color.white);
        var scale = Math.Max(0, k_ScaleFactor - 2);
        var radius = new Vector2Int(Mathf.RoundToInt(scale * grid.cellSize.x),
            Mathf.RoundToInt(scale * grid.cellSize.y));
        for (var y = -radius.y; y <= radius.y; ++y)
        for (var x = -radius.x; x <= radius.x; ++x)
            tintTexture.SetPixel(texPosition.x + x, texPosition.y + y, color);
        tintTexture.Apply();
    }

    /// <summary>
    ///     Get the color of a position on a Grid
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
    ///     Set the color of a position on a Grid
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

    private Vector3Int WorldToTexture(Vector3 worldPos)
    {
        return new Vector3Int(Mathf.FloorToInt(worldPos.x * k_ScaleFactor + tintTexture.width / 2f)
            , Mathf.FloorToInt(worldPos.y * k_ScaleFactor + tintTexture.height / 2f), 0);
    }

    private Vector3 TextureToWorld(Vector3Int texPos)
    {
        var inv = 1 / (k_ScaleFactor == 0 ? 1 : k_ScaleFactor);
        return new Vector3((texPos.x - tintTexture.width / 2) * inv
            , (texPos.y - tintTexture.height / 2) * inv, 0);
    }

    private GridInformation GetGridInformation(Grid grid)
    {
        var gridInformation = grid.GetComponent<GridInformation>();

        if (gridInformation == null)
            gridInformation = grid.gameObject.AddComponent<GridInformation>();

        return gridInformation;
    }

    private void RefreshGlobalShaderValues()
    {
        Shader.SetGlobalTexture("_TintMap", m_TintTexture);
        Shader.SetGlobalFloat("_TintMapSize", k_TintMapSize);
    }
}