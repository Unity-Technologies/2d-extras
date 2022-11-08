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
}
