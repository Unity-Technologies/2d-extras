using System;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    [ScriptedImporter(1, "att")]
    public class AutoTileTextureImporter : ScriptedImporter
    {
        private static int[] s_Elements = new[]
        {
            4, 5, 6, 3,
            4, 5, 2, 7,
            12, 5, 14, 3,
            4, 13, 2, 15,
            8, 9, 6, 3,
            8, 9, 2, 7,
            0, 5, 6, 3,
            4, 1, 2, 7, 

            4, 1, 6, 7,
            0, 5, 6, 7,
            12, 1, 14, 7,
            0, 13, 6, 15,
            4, 1, 10, 11,
            0, 5, 10, 11,
            16, 9, 14, 3,
            8, 17, 2, 15,
            
            0, 1, 2, 7,
            0, 1, 6, 3,
            0, 5, 2, 7,
            4, 1, 6, 3,
            0, 1, 10, 11,
            8, 9, 2, 3,
            12, 1, 18, 11,
            0, 13, 10, 19,
            
            0, 5, 2, 3,
            4, 1, 2, 3,
            4, 5, 2, 3,
            0, 1, 6, 7,
            12, 1, 14, 3,
            0, 13, 2, 15,
            0, 1, 2, 3,
            4, 5, 6, 7,
            
            16, 9, 14, 7,
            8, 17, 6, 15,
            12, 13, 14, 15,
            12, 5, 14, 7,
            4, 13, 6, 15,
            12, 13, 18, 19,
            16, 9, 18, 11,
            16, 17, 18, 19,
            
            12, 5, 18, 11,
            4, 13, 10, 19,
            8, 9, 10, 11,
            4, 5, 10, 11,
            8, 9, 6, 7,
            8, 17, 10, 19,
            16, 17, 14, 15,
        };

        private static uint[] s_Mask = new uint[]
        {
            442, 250, 434, 218, 440, 248, 443, 254,
            190, 187, 182, 155, 62, 59, 432, 216,
            255, 447, 251, 446, 63, 504, 54, 27,
            507, 510, 506, 191, 438, 219, 511, 186,
            176, 152, 146, 178, 154, 18, 48, 16,
            50, 26, 56, 58, 184, 24, 144,
        };
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var objects = InternalEditorUtility.LoadSerializedFileAndForget(ctx.assetPath);
            var autoTileTexture = objects[0] as AutoTileTexture;
            if (autoTileTexture == null)
            {
                Debug.LogError("Unable to load AutoTileTexture asset", objects[0]);
                return;
            }

            if (autoTileTexture.m_Texture == null)
                return;

            var sourceTexture = autoTileTexture.m_Texture;
            var texturePath = AssetDatabase.GetAssetPath(sourceTexture);
            ctx.DependsOnSourceAsset(texturePath);

            var size = new Vector2Int(sourceTexture.width, sourceTexture.height / 5);
            var paddedSize = size + 2 * Vector2Int.one;
            var cornerSize = size / 2;
            var paddedCornerSize = cornerSize + Vector2Int.one;
            
            var destTexture = new Texture2D(8 * paddedSize.x, 6 * paddedSize.y, sourceTexture.format, false);
            destTexture.name = "AutoTileTexture";
            destTexture.wrapMode = sourceTexture.wrapMode;
            destTexture.filterMode = sourceTexture.filterMode;
            destTexture.anisoLevel = sourceTexture.anisoLevel;

            var baseColors = new List<Color[]>(); 
            for (var y = 0; y < 5; ++y)
            {
                for (var i = 0; i < 4; ++i)
                {
                    var isTop = i / 2;
                    var isRight = i % 2;
                    var source = sourceTexture.GetPixels(isRight * cornerSize.x, isTop * cornerSize.y + y * size.y, cornerSize.x, cornerSize.y);
                    var dest = new Color[paddedCornerSize.x * paddedCornerSize.y];

                    if (isTop > 0)
                    {
                        Array.Copy(source, (cornerSize.y - 1) * cornerSize.x, dest, (cornerSize.y - 1) * cornerSize.x + isRight, cornerSize.x);
                    }
                    else
                    {
                        Array.Copy(source, 0, dest, 1 - isRight, cornerSize.x);
                    }
                    for (var j = 0; j < cornerSize.y; ++j)
                    {
                        if (isRight == 0)
                            Array.Copy(source, j * cornerSize.x, dest, (j + 1 - isTop) * paddedCornerSize.x, 1);    
                        Array.Copy(source, j * cornerSize.x, dest, (j + 1 - isTop) * paddedCornerSize.x + (1 - isRight), cornerSize.x);
                        if (isRight > 0)
                            Array.Copy(source, j * cornerSize.x + cornerSize.x - 1, dest, (j + 1 - isTop) * paddedCornerSize.x + cornerSize.x, 1);
                    }
                    baseColors.Add(dest);
                }
            }

            for (var k = 0; k < s_Elements.Length; k += 4)
            {
                var i = k / 4;
                var x = i % 8;
                var y = i / 8;
                destTexture.SetPixels(x * paddedSize.x, y * paddedSize.y, paddedCornerSize.x, paddedCornerSize.y, baseColors[s_Elements[k + 0]]);
                destTexture.SetPixels(x * paddedSize.x + paddedCornerSize.x, y * paddedSize.y, paddedCornerSize.x, paddedCornerSize.y, baseColors[s_Elements[k + 1]]);
                destTexture.SetPixels(x * paddedSize.x, y * paddedSize.y + paddedCornerSize.y, paddedCornerSize.x, paddedCornerSize.y, baseColors[s_Elements[k + 2]]);
                destTexture.SetPixels(x * paddedSize.x + paddedCornerSize.x, y * paddedSize.y + paddedCornerSize.y, paddedCornerSize.x, paddedCornerSize.y, baseColors[s_Elements[k + 3]]);
            }
            ctx.AddObjectToAsset("AutoTileTexture", destTexture);

            
            var autoTile = ScriptableObject.CreateInstance<AutoTile>();
            autoTile.m_TextureList = new List<Texture2D>();
            autoTile.m_TextureList.Add(destTexture);
            autoTile.m_MaskType = AutoTile.AutoTileMaskType.Mask_3x3;
            autoTile.m_DefaultColliderType = Tile.ColliderType.Sprite;

            for (var k = 0; k < s_Elements.Length; k += 4)
            {
                var i = k / 4;
                var x = i % 8;
                var y = i / 8;
                var sprite = Sprite.Create(destTexture,
                    new Rect(1 + x * paddedSize.x, 1 + y * paddedSize.y, size.x, size.y), new Vector2(0.5f, 0.5f),
                    size.x, 0, SpriteMeshType.FullRect, Vector4.zero, true);
                sprite.name = $"Sprite {i}";
                ctx.AddObjectToAsset(sprite.name, sprite);
                autoTile.AddSprite(sprite, destTexture, (uint) s_Mask[i]);

                if (i == 39)
                {
                    autoTile.m_DefaultSprite = sprite;
                }
            }
            ctx.AddObjectToAsset("AutoTile", autoTile);
            
            ctx.SetMainObject(autoTile);
        }
    }
}