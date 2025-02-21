using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Class containing utility methods for AutoTile Template
    /// </summary>
    public static class AutoTileTemplateUtility
    {
        /// <summary>
        /// Loads an AutoTileTemplate from an asset file with a File Panel.
        /// </summary>
        /// <returns>AutoTIleTemplate from asset file.</returns>
        public static AutoTileTemplate LoadTemplateFromFile()
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var getActiveFolderPath =
                projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            var obj = getActiveFolderPath.Invoke(null, new object[0]);
            var pathToCurrentFolder = obj.ToString();

            var templatePath = EditorUtility.OpenFilePanel("Load AutoTile template", pathToCurrentFolder,
                AutoTileTemplate.kExtension);
            var relativePath = FileUtil.GetProjectRelativePath(templatePath);
            var template = AssetDatabase.LoadAssetAtPath<AutoTileTemplate>(relativePath);
            if (template == null)
            {
                Debug.LogWarningFormat("{0} does not contain a valid AutoTileTemplate.", relativePath);
            }
            return template;
        }

        /// <summary>
        /// Applies an AutoTileTemplate to an AutoTile with a source Texture2D 
        /// </summary>
        /// <param name="template">AutoTileTemplate to apply.</param>
        /// <param name="texture">Source Texture2D containing Sprites for the AutoTileTemplate.</param>
        /// <param name="sprites">Source Sprites to be used in the AutoTile.</param>
        /// <param name="autoTile">AutoTile updated with AutoTileTemplate.</param>
        /// <param name="matchExact">Match Sprites from Source exactly with positional data from AutoTileTemplate
        /// or match based on relative positional size.</param>
        public static void ApplyTemplateToAutoTile(this AutoTileTemplate template
            , Texture2D texture
            , IEnumerable<Sprite> sprites
            , AutoTile autoTile
            , bool matchExact = false)
        {
            if (template == null || texture == null || autoTile == null)
                return;

            autoTile.m_MaskType = template.maskType;
            if (autoTile.m_TextureList == null)
                autoTile.m_TextureList = new List<Texture2D>();
            if (autoTile.m_TextureScaleList == null)
                autoTile.m_TextureScaleList = new List<float>();
            autoTile.m_TextureList.Add(texture);
            autoTile.m_TextureScaleList.Add(AutoTile.s_DefaultTextureScale);
            foreach (var sprite in sprites)
            {
                foreach (var templateSprite in template.sprites)
                {
                    var match = false;
                    if (matchExact)
                    {
                        match = Mathf.Approximately(templateSprite.x, sprite.rect.x)
                                && Mathf.Approximately(templateSprite.y, sprite.rect.y);
                    }
                    else
                    {
                        match = Mathf.Approximately(templateSprite.x / template.width, sprite.rect.x / texture.width)
                                && Mathf.Approximately(templateSprite.y / template.height, sprite.rect.y / texture.height);
                    }
                    if (match)
                    {
                        autoTile.AddSprite(sprite, texture, templateSprite.mask);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates an AutoTileTemplate with the given parameters.
        /// </summary>
        /// <param name="imageWidth">Width of original image.</param>
        /// <param name="imageHeight">Height of original image.</param>
        /// <param name="maskType">Mask Type to apply to AutoTile.</param>
        /// <param name="spriteData">Positional Data for AutoTileTemplate based on original image.</param>
        /// <returns>AutoTileTemplate generated with the given parameters.</returns>
        public static AutoTileTemplate CreateTemplate(int imageWidth
            , int imageHeight
            , AutoTile.AutoTileMaskType maskType
            , List<AutoTileTemplate.SpriteData> spriteData)
        {
            var template = ScriptableObject.CreateInstance<AutoTileTemplate>();
            template.width = imageWidth;
            template.height = imageHeight;
            template.maskType = maskType;
            template.sprites = spriteData;
            return template;
        }
        
        /// <summary>
        /// Creates and saves an AutoTileTemplate with a FilePanel.
        /// </summary>
        /// <param name="imageWidth">Width of original image.</param>
        /// <param name="imageHeight">Height of original image.</param>
        /// <param name="maskType">Mask Type to apply to AutoTile.</param>
        /// <param name="spriteData">Positional Data for AutoTileTemplate based on original image.</param>
        public static void SaveTemplateToFile(int imageWidth
            , int imageHeight
            , AutoTile.AutoTileMaskType maskType
            , List<AutoTileTemplate.SpriteData> spriteData)
        {
            var template = CreateTemplate(imageWidth, imageHeight, maskType, spriteData);
            var path = EditorUtility.SaveFilePanelInProject("Save AutoTile template", "New AutoTile Template", AutoTileTemplate.kExtension, "");
            if (!String.IsNullOrWhiteSpace(path))
                AssetDatabase.CreateAsset(template, path);
        }
    }
}