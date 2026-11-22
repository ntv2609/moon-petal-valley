using UnityEditor;
using UnityEngine;

namespace MoonpetalValley.EditorTools
{
    public class MoonpetalAssetPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Art/Sprites/")) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;

            bool isAnimSprite = assetPath.StartsWith("Assets/Art/Sprites/Animations/");
            if (assetPath.StartsWith("Assets/Art/Sprites/Animations/Wen/") ||
                assetPath.StartsWith("Assets/Art/Sprites/Animations/Lumi/") ||
                assetPath.StartsWith("Assets/Art/Sprites/Animations/NPCs/"))
            {
                importer.spritePixelsPerUnit = 20;
            }
            else if (assetPath.StartsWith("Assets/Art/Sprites/Animations/GloomSlime/"))
            {
                importer.spritePixelsPerUnit = 32;
            }
            else
            {
                importer.spritePixelsPerUnit = 16;
            }

            if (isAnimSprite)
            {
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteAlignment = (int)SpriteAlignment.Center;
                settings.spriteGenerateFallbackPhysicsShape = false;
                importer.SetTextureSettings(settings);
            }
        }
    }
}
