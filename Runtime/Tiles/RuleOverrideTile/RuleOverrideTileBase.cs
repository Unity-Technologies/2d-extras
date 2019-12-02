namespace UnityEngine.Tilemaps
{

    public abstract class RuleOverrideTileBase : TileBase
    {

        /// <summary>
        /// The RuleTile to override
        /// </summary>
        public RuleTile m_Tile;
        /// <summary>
        /// Returns the Rule Tile for retrieving TileData
        /// </summary>
        [HideInInspector] public RuleTile m_InstanceTile;

        public abstract void Override();

        /// <summary>
        /// Retrieves any tile animation data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileAnimationData">Data to run an animation on the tile.</param>
        /// <returns>Whether the call was successful.</returns>
        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            if (!m_InstanceTile)
                return false;
            return m_InstanceTile.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        }

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            if (!m_InstanceTile)
                return;
            m_InstanceTile.GetTileData(position, tilemap, ref tileData);
        }

        /// <summary>
        /// This method is called when the tile is refreshed.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tileMap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            if (!m_InstanceTile)
                return;
            m_InstanceTile.RefreshTile(position, tilemap);
        }

        /// <summary>
        /// StartUp is called on the first frame of the running Scene.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="instantiateedGameObject">The GameObject instantiated for the Tile.</param>
        /// <returns>Whether StartUp was successful</returns>
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            if (!m_InstanceTile)
                return true;
            return m_InstanceTile.StartUp(position, tilemap, go);
        }

        /// <summary>
        /// Copies a Tiling Rule from a given Tiling Rule
        /// </summary>
        /// <param name="from">A Tiling Rule to copy from</param>
        /// <param name="to">A Tiling Rule to copy to</param>
        public static void CopyTilingRule(RuleTile.TilingRuleOutput from, RuleTile.TilingRuleOutput to)
        {
            to.m_Id = from.m_Id;
            to.m_Sprites = from.m_Sprites.Clone() as Sprite[];
            to.m_GameObject = from.m_GameObject;
            to.m_AnimationSpeed = from.m_AnimationSpeed;
            to.m_PerlinScale = from.m_PerlinScale;
            to.m_Output = from.m_Output;
            to.m_ColliderType = from.m_ColliderType;
            to.m_RandomTransform = from.m_RandomTransform;
        }
        public static void CopyTilingRule(RuleTile.TilingRule from, RuleTile.TilingRule to)
        {
            CopyTilingRule(from as RuleTile.TilingRuleOutput, to as RuleTile.TilingRuleOutput);

            to.m_Neighbors = from.m_Neighbors;
            to.m_NeighborPositions = from.m_NeighborPositions;
            to.m_RuleTransform = from.m_RuleTransform;
        }
    }
}
