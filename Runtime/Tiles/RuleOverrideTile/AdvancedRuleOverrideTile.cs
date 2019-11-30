using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// Rule Override Tiles are Tiles which can override a subset of Rules for a given Rule Tile to provide specialised behaviour while keeping most of the Rules originally set in the Rule Tile.
    /// </summary>
    [MovedFrom(true, "UnityEngine")]
    [Serializable]
    [CreateAssetMenu(fileName = "New Advanced Rule Override Tile", menuName = "Tiles/Advanced Rule Override Tile")]
    public class AdvancedRuleOverrideTile : RuleOverrideTileBase, IRuleOverrideTile<RuleTile.TilingRule, RuleTile.TilingRuleOutput>
    {

        public RuleTile.TilingRuleOutput this[RuleTile.TilingRule originalRule]
        {
            get
            {
                foreach (var overrideRule in m_OverrideTilingRules)
                    if (overrideRule.m_InstanceID == originalRule.m_InstanceID)
                        return overrideRule;

                return null;
            }
            set
            {
                for (int i = m_OverrideTilingRules.Count - 1; i >= 0; i--)
                {
                    if (m_OverrideTilingRules[i].m_InstanceID == originalRule.m_InstanceID)
                    {
                        m_OverrideTilingRules.RemoveAt(i);
                        break;
                    }
                }
                if (value != null)
                {
                    var overrideRule = new RuleTile.TilingRuleOutput();
                    CopyTilingRule(value, overrideRule);
                    m_OverrideTilingRules.Add(overrideRule);
                }
            }
        }

        public Sprite m_DefaultSprite;
        public GameObject m_DefaultGameObject;
        public Tile.ColliderType m_DefaultColliderType = Tile.ColliderType.Sprite;
        public List<RuleTile.TilingRuleOutput> m_OverrideTilingRules = new List<RuleTile.TilingRuleOutput>();

        public void ApplyOverrides(IList<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            m_OverrideTilingRules.Clear();

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

        public void GetOverrides(List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>> overrides)
        {
            if (overrides == null)
                throw new System.ArgumentNullException("overrides");

            overrides.Clear();

            if (m_Tile)
            {
                foreach (var originalRule in m_Tile.m_TilingRules)
                {
                    RuleTile.TilingRuleOutput overrideRule = this[originalRule];
                    overrides.Add(new KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>(originalRule, overrideRule));
                }
            }
        }

        public override void Override()
        {
            if (!m_Tile || !m_InstanceTile)
                return;

            var tile = m_InstanceTile;

            tile.m_DefaultSprite = m_DefaultSprite;
            tile.m_DefaultGameObject = m_DefaultGameObject;
            tile.m_DefaultColliderType = m_DefaultColliderType;
            tile.m_TilingRules.Clear();

            foreach (var originalRule in m_Tile.m_TilingRules)
            {
                var overrideRule = this[originalRule];
                var instanceRule = new RuleTile.TilingRule();
                CopyTilingRule(originalRule, instanceRule);
                if (overrideRule != null)
                    CopyTilingRule(overrideRule, instanceRule);
                tile.m_TilingRules.Add(instanceRule);
            }
        }
    }
}
