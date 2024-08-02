using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
    /// <summary>
    ///     The Editor for a HexagonalRuleTile.
    /// </summary>
    [CustomEditor(typeof(HexagonalRuleTile), true)]
    [CanEditMultipleObjects]
    public class HexagonalRuleTileEditor : RuleTileEditor
    {
        /// <summary>
        ///     The HexagonalRuleTile being edited.
        /// </summary>
        public HexagonalRuleTile hexTile => target as HexagonalRuleTile;

        /// <summary>
        ///     Gets the index for a Rule with the HexagonalRuleTile to display an arrow.
        /// </summary>
        /// <param name="position">The adjacent position of the arrow.</param>
        /// <returns>Returns the index for a Rule with the HexagonalRuleTile to display an arrow.</returns>
        public override int GetArrowIndex(Vector3Int position)
        {
            if (position.y % 2 != 0)
            {
                position *= 2;
                position.x += 1;
            }

            if (position.x == 0)
            {
                if (position.y > 0)
                    return hexTile.m_FlatTop ? 5 : 1;
                return hexTile.m_FlatTop ? 3 : 7;
            }

            if (position.y == 0)
            {
                if (position.x > 0)
                    return hexTile.m_FlatTop ? 1 : 5;
                return hexTile.m_FlatTop ? 7 : 3;
            }

            if (position.x < 0 && position.y > 0)
                return hexTile.m_FlatTop ? 8 : 0;
            if (position.x > 0 && position.y > 0)
                return hexTile.m_FlatTop ? 2 : 2;
            if (position.x < 0 && position.y < 0)
                return hexTile.m_FlatTop ? 6 : 6;
            if (position.x > 0 && position.y < 0)
                return hexTile.m_FlatTop ? 0 : 8;

            return -1;
        }

        /// <summary>
        ///     Get the GUI bounds for a Rule.
        /// </summary>
        /// <param name="bounds">Cell bounds of the Rule.</param>
        /// <param name="rule">Rule to get GUI bounds for.</param>
        /// <returns>The GUI bounds for a rule.</returns>
        public override BoundsInt GetRuleGUIBounds(BoundsInt bounds, RuleTile.TilingRule rule)
        {
            foreach (var n in rule.GetNeighbors())
                if (n.Key.x == bounds.xMax - 1 && n.Key.y % 2 != 0)
                {
                    bounds.xMax++;
                    break;
                }

            return base.GetRuleGUIBounds(bounds, rule);
        }

        /// <summary>
        ///     Gets the GUI matrix size for a Rule of a HexagonalRuleTile
        /// </summary>
        /// <param name="bounds">Cell bounds of the Rule.</param>
        /// <returns>Returns the GUI matrix size for a Rule of a HexagonalRuleTile.</returns>
        public override Vector2 GetMatrixSize(BoundsInt bounds)
        {
            var size = base.GetMatrixSize(bounds);
            return hexTile.m_FlatTop ? new Vector2(size.y, size.x) : size;
        }

        /// <summary>
        ///     Draws a Rule Matrix for the given Rule for a HexagonalRuleTile.
        /// </summary>
        /// <param name="tile">Tile to draw rule for.</param>
        /// <param name="rect">GUI Rect to draw rule at.</param>
        /// <param name="bounds">Cell bounds of the Rule.</param>
        /// <param name="tilingRule">Rule to draw Rule Matrix for.</param>
        public override void RuleMatrixOnGUI(RuleTile tile, Rect rect, BoundsInt bounds, RuleTile.TilingRule tilingRule)
        {
            var flatTop = hexTile.m_FlatTop;

            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            var w = rect.width / (flatTop ? bounds.size.y : bounds.size.x);
            var h = rect.height / (flatTop ? bounds.size.x : bounds.size.y);

            // Grid
            if (flatTop)
                for (var y = 0; y <= bounds.size.y; y++)
                {
                    var left = rect.xMin + y * w;
                    float offset = 0;

                    if (y == 0 && bounds.yMax % 2 == 0)
                        offset = h / 2;
                    else if (y == bounds.size.y && bounds.yMin % 2 != 0)
                        offset = h / 2;

                    Handles.DrawLine(new Vector3(left, rect.yMin + offset), new Vector3(left, rect.yMax - offset));

                    if (y < bounds.size.y)
                    {
                        var noOffset = (y + bounds.yMax) % 2 != 0;
                        for (var x = 0; x < (noOffset ? bounds.size.x + 1 : bounds.size.x); x++)
                        {
                            var top = rect.yMin + x * h + (noOffset ? 0 : h / 2);
                            Handles.DrawLine(new Vector3(left, top), new Vector3(left + w, top));
                        }
                    }
                }
            else
                for (var y = 0; y <= bounds.size.y; y++)
                {
                    var top = rect.yMin + y * h;
                    float offset = 0;

                    if (y == 0 && bounds.yMax % 2 == 0)
                        offset = w / 2;
                    else if (y == bounds.size.y && bounds.yMin % 2 != 0)
                        offset = w / 2;

                    Handles.DrawLine(new Vector3(rect.xMin + offset, top), new Vector3(rect.xMax - offset, top));

                    if (y < bounds.size.y)
                    {
                        var noOffset = (y + bounds.yMax) % 2 != 0;
                        for (var x = 0; x < (noOffset ? bounds.size.x + 1 : bounds.size.x); x++)
                        {
                            var left = rect.xMin + x * w + (noOffset ? 0 : w / 2);
                            Handles.DrawLine(new Vector3(left, top), new Vector3(left, top + h));
                        }
                    }
                }

            var neighbors = tilingRule.GetNeighbors();

            // Icons
            Handles.color = Color.white;
            for (var y = bounds.yMin; y < bounds.yMax; y++)
            {
                var xMax = y % 2 == 0 ? bounds.xMax : bounds.xMax - 1;
                for (var x = bounds.xMin; x < xMax; x++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var offset = new Vector2(x - bounds.xMin, -y + bounds.yMax - 1);
                    var r = flatTop
                        ? new Rect(rect.xMax - offset.y * w - w, rect.yMax - offset.x * h - h, w - 1, h - 1)
                        : new Rect(rect.xMin + offset.x * w, rect.yMin + offset.y * h, w - 1, h - 1);

                    if (y % 2 != 0)
                    {
                        if (flatTop)
                            r.y -= h / 2;
                        else
                            r.x += w / 2;
                    }

                    RuleMatrixIconOnGUI(tilingRule, neighbors, pos, r);
                }
            }
        }

        /// <summary>
        ///     Creates a Preview for the HexagonalRuleTile.
        /// </summary>
        protected override void CreatePreview()
        {
            base.CreatePreview();

            m_PreviewGrid.cellLayout = GridLayout.CellLayout.Hexagon;
            m_PreviewGrid.cellSize = new Vector3(0.8659766f, 1.0f, 1.0f);
            m_PreviewGrid.cellSwizzle = hexTile.m_FlatTop ? GridLayout.CellSwizzle.YXZ : GridLayout.CellSwizzle.XYZ;

            foreach (var tilemap in m_PreviewTilemaps)
            {
                tilemap.tileAnchor = Vector3.zero;
                tilemap.ClearAllTiles();
            }

            for (var x = -1; x <= 0; ++x)
            for (var y = -1; y <= 1; ++y)
                m_PreviewTilemaps[0].SetTile(new Vector3Int(x, y, 0), tile);

            m_PreviewTilemaps[1].SetTile(new Vector3Int(1, -1, 0), tile);
            m_PreviewTilemaps[1].SetTile(new Vector3Int(2, 0, 0), tile);
            m_PreviewTilemaps[1].SetTile(new Vector3Int(2, 1, 0), tile);

            for (var x = -1; x <= 1; x++)
                m_PreviewTilemaps[2].SetTile(new Vector3Int(x, -2, 0), tile);

            m_PreviewTilemaps[3].SetTile(new Vector3Int(1, 1, 0), tile);

            foreach (var tilemapRenderer in m_PreviewTilemapRenderers)
                tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight;

            m_PreviewTilemapRenderers[0].sortingOrder = 0;
            m_PreviewTilemapRenderers[1].sortingOrder = -1;
            m_PreviewTilemapRenderers[2].sortingOrder = 1;
            m_PreviewTilemapRenderers[3].sortingOrder = 0;
        }
    }
}