using System;

namespace UnityEngine
{
    /// <summary>
    /// Generic visual tile for creating different tilesets like terrain, pipeline, random or animated tiles.
    /// This is templated to accept a Neighbor Rule Class for Custom Rules.
    /// Use this for Hexagonal Grids. 
    /// </summary>
    /// <typeparam name="T">Neighbor Rule Class for Custom Rules</typeparam>
    public class HexagonalRuleTile<T> : HexagonalRuleTile
    {
        /// <summary>
        /// Returns the Neighbor Rule Class type for this Rule Tile.
        /// </summary>
        public sealed override Type m_NeighborType => typeof(T);
    }

    /// <summary>
    /// Generic visual tile for creating different tilesets like terrain, pipeline, random or animated tiles.
    /// Use this for Hexagonal Grids.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Hexagonal Rule Tile", menuName = "2D Extras/Tiles/Hexagonal Rule Tile", order = 359)]
    public class HexagonalRuleTile : RuleTile
    {

        public override int m_RotationAngle => 60;

        private static float[] m_CosAngleArr1 = {
            Mathf.Cos(0 * Mathf.Deg2Rad),
            Mathf.Cos(-60 * Mathf.Deg2Rad),
            Mathf.Cos(-120 * Mathf.Deg2Rad),
            Mathf.Cos(-180 * Mathf.Deg2Rad),
            Mathf.Cos(-240 * Mathf.Deg2Rad),
            Mathf.Cos(-300 * Mathf.Deg2Rad),
        };
        private static float[] m_SinAngleArr1 = {
            Mathf.Sin(0 * Mathf.Deg2Rad),
            Mathf.Sin(-60 * Mathf.Deg2Rad),
            Mathf.Sin(-120 * Mathf.Deg2Rad),
            Mathf.Sin(-180 * Mathf.Deg2Rad),
            Mathf.Sin(-240 * Mathf.Deg2Rad),
            Mathf.Sin(-300 * Mathf.Deg2Rad),
        };
        private static float[] m_CosAngleArr2 = {
            Mathf.Cos(0 * Mathf.Deg2Rad),
            Mathf.Cos(60 * Mathf.Deg2Rad),
            Mathf.Cos(120 * Mathf.Deg2Rad),
            Mathf.Cos(180 * Mathf.Deg2Rad),
            Mathf.Cos(240 * Mathf.Deg2Rad),
            Mathf.Cos(300 * Mathf.Deg2Rad),
        };
        private static float[] m_SinAngleArr2 = {
            Mathf.Sin(0 * Mathf.Deg2Rad),
            Mathf.Sin(60 * Mathf.Deg2Rad),
            Mathf.Sin(120 * Mathf.Deg2Rad),
            Mathf.Sin(180 * Mathf.Deg2Rad),
            Mathf.Sin(240 * Mathf.Deg2Rad),
            Mathf.Sin(300 * Mathf.Deg2Rad),
        };

        /// <summary>
        /// Whether this is a flat top Hexagonal Tile
        /// </summary>
        [DontOverride] public bool m_FlatTop;

        static float m_TilemapToWorldYScale = Mathf.Pow(1 - Mathf.Pow(0.5f, 2f), 0.5f);

        public static Vector3 TilemapPositionToWorldPosition(Vector3Int tilemapPosition)
        {
            Vector3 worldPosition = new Vector3(tilemapPosition.x, tilemapPosition.y);
            if (tilemapPosition.y % 2 != 0)
                worldPosition.x += 0.5f;
            worldPosition.y *= m_TilemapToWorldYScale;
            return worldPosition;
        }

        public static Vector3Int WorldPositionToTilemapPosition(Vector3 worldPosition)
        {
            worldPosition.y /= m_TilemapToWorldYScale;
            Vector3Int tilemapPosition = new Vector3Int();
            tilemapPosition.y = Mathf.RoundToInt(worldPosition.y);
            if (tilemapPosition.y % 2 != 0)
                tilemapPosition.x = Mathf.RoundToInt(worldPosition.x - 0.5f);
            else
                tilemapPosition.x = Mathf.RoundToInt(worldPosition.x);
            return tilemapPosition;
        }

        public override Vector3Int GetOffsetPosition(Vector3Int location, Vector3Int offset)
        {
            Vector3Int position = location + offset;

            if (offset.y % 2 != 0 && location.y % 2 != 0)
                position.x += 1;

            return position;
        }

        public override Vector3Int GetOffsetPositionReverse(Vector3Int position, Vector3Int offset)
        {
            Vector3Int location = position - offset;

            if (offset.y % 2 != 0 && location.y % 2 != 0)
                location.x -= 1;

            return location;
        }

        /// <summary>
        /// Gets a rotated position given its original position and the rotation in degrees. 
        /// </summary>
        /// <param name="position">Original position of Tile.</param>
        /// <param name="rotation">Rotation in degrees.</param>
        /// <returns>Rotated position of Tile.</returns>
        public override Vector3Int GetRotatedPosition(Vector3Int position, int rotation)
        {
            if (rotation != 0)
            {
                Vector3 worldPosition = TilemapPositionToWorldPosition(position);

                int index = rotation / 60;
                if (m_FlatTop)
                {
                    worldPosition = new Vector3(
                        worldPosition.x * m_CosAngleArr2[index] - worldPosition.y * m_SinAngleArr2[index],
                        worldPosition.x * m_SinAngleArr2[index] + worldPosition.y * m_CosAngleArr2[index]
                    );
                }
                else
                {
                    worldPosition = new Vector3(
                        worldPosition.x * m_CosAngleArr1[index] - worldPosition.y * m_SinAngleArr1[index],
                        worldPosition.x * m_SinAngleArr1[index] + worldPosition.y * m_CosAngleArr1[index]
                    );
                }

                position = WorldPositionToTilemapPosition(worldPosition);
            }
            return position;
        }

        /// <summary>
        /// Gets a mirrored position given its original position and the mirroring axii.
        /// </summary>
        /// <param name="position">Original position of Tile.</param>
        /// <param name="mirrorX">Mirror in the X Axis.</param>
        /// <param name="mirrorY">Mirror in the Y Axis.</param>
        /// <returns>Mirrored position of Tile.</returns>
        public override Vector3Int GetMirroredPosition(Vector3Int position, bool mirrorX, bool mirrorY)
        {
            if (mirrorX || mirrorY)
            {
                Vector3 worldPosition = TilemapPositionToWorldPosition(position);

                if (m_FlatTop)
                {
                    if (mirrorX)
                        worldPosition.y *= -1;
                    if (mirrorY)
                        worldPosition.x *= -1;
                }
                else
                {
                    if (mirrorX)
                        worldPosition.x *= -1;
                    if (mirrorY)
                        worldPosition.y *= -1;
                }

                position = WorldPositionToTilemapPosition(worldPosition);
            }
            return position;
        }
    }
}
