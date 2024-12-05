using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Class containing utility methods for RuleTile Template
    /// </summary>
    public static class RuleTileTemplateUtility
    {
        /// <summary>
        /// Applies an RuleTileTemplate to an RuleTile with a source Texture2D 
        /// </summary>
        /// <param name="template">RuleTileTemplate to apply.</param>
        /// <param name="texture">Source Texture2D containing Sprites for the RuleTileTemplate.</param>
        /// <param name="ruleTile">RuleTile updated with RuleTileTemplate.</param>
        /// <param name="matchExact">Match Sprites from Source exactly with positional data from RuleTileTemplate
        /// or match based on relative positional size.</param>
        public static void ApplyTemplateToRuleTile(this RuleTileTemplate template
            , Texture2D texture
            , RuleTile ruleTile
            , bool matchExact = true)
        {
            if (template == null || texture == null || ruleTile == null)
                return;

            ruleTile.m_DefaultSprite = template.defaultSprite;
            ruleTile.m_DefaultGameObject = template.defaultGameObject;
            ruleTile.m_DefaultColliderType = template.defaultColliderType;
            if (ruleTile.m_TilingRules == null)
                ruleTile.m_TilingRules = new List<RuleTile.TilingRule>(template.rules.Count);

            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture));
            var j = 0;
            foreach (var rule in template.rules)
            {
                var copyRule = rule.tilingRule.Clone();
                for (var i = 0; i < rule.tilingRule.m_Sprites.Length; ++i)
                {
                    var spritePosition = rule.spritePositions[i];
                    foreach (var asset in assets)
                    {
                        var sprite = asset as Sprite;
                        if (sprite == null)
                            continue;
                    
                        var match = false;
                        if (matchExact)
                        {
                            match = Mathf.Approximately(spritePosition.x, sprite.rect.x)
                                    && Mathf.Approximately(spritePosition.y, sprite.rect.y);
                        }
                        else
                        {
                            match = Mathf.Approximately(spritePosition.x / template.textureWidth, sprite.rect.x / texture.width)
                                    && Mathf.Approximately(spritePosition.y / template.textureHeight, sprite.rect.y / texture.height);
                        }
                        if (match)
                        {
                            copyRule.m_Sprites[i] = sprite;;
                            break;
                        }
                    }                    
                }
                ruleTile.m_TilingRules.Add(copyRule);
                j++;
            }
        }

        /// <summary>
        /// Creates an RuleTileTemplate with the given parameters.
        /// </summary>
        /// <param name="ruleTile">RuleTile to save template with.</param>
        /// <returns>RuleTileTemplate generated with the given parameters.</returns>
        public static RuleTileTemplate CreateTemplate(RuleTile ruleTile)
        {
            var template = ScriptableObject.CreateInstance<RuleTileTemplate>();
            template.defaultSprite = ruleTile.m_DefaultSprite;
            template.defaultGameObject = ruleTile.m_DefaultGameObject;
            template.defaultColliderType = ruleTile.m_DefaultColliderType;

            var count = ruleTile.m_TilingRules.Count;
            template.rules = new List<RuleTileTemplate.RuleData>(count);
            for (var j = 0; j < count; j++)
            {
                var ruleData = new RuleTileTemplate.RuleData();
                ruleData.tilingRule = ruleTile.m_TilingRules[j];
                var spriteCount = ruleData.tilingRule.m_Sprites.Length;
                ruleData.spritePositions = new List<Vector2>(spriteCount);
                for (var i = 0; i < spriteCount; ++i)
                {
                    var sprite = ruleData.tilingRule.m_Sprites[i];
                    var position = sprite != null ? sprite.rect.position : Vector2.zero;
                    ruleData.spritePositions.Add(position);
                }
                template.rules.Add(ruleData);
            }
            return template;
        }
        
        /// <summary>
        /// Creates and saves an RuleTileTemplate with a FilePanel.
        /// </summary>
        /// <param name="ruleTile">RuleTile to save template with.</param>
        public static void SaveTemplateToFile(RuleTile ruleTile)
        {
            var template = CreateTemplate(ruleTile);
            var path = EditorUtility.SaveFilePanelInProject("Save RuleTile template", "New RuleTile Template", RuleTileTemplate.kExtension, "");
            if (!String.IsNullOrWhiteSpace(path))
                AssetDatabase.CreateAsset(template, path);
        }
    }
}