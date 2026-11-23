using UnityEditor;
using UnityEngine;

namespace MoonpetalValley.EditorTools
{
    public class MoonpetalAssetPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            string p = assetPath.Replace("\\", "/");
            if (!p.EndsWith(".png")) return;

            bool isWen = p.Contains("/Art/Sprites/Animations/Wen/");
            bool isLumi = p.Contains("/Art/Sprites/Animations/Lumi/");
            bool isSlime = p.Contains("/Art/Sprites/Animations/GloomSlime/");
            bool isNpc = p.Contains("/Art/Sprites/Animations/NPCs/");
            bool isEnv = p.Contains("/Art/Sprites/Environment/");
            bool isCh2World = p.Contains("/Art/Sprites/Chapter2/");
            bool isCh2Mist = p.EndsWith("ch2_river_mist_strip.png");
            bool isPortrait = p.Contains("/Art/Sprites/Portraits/");
            bool isCh2Char = p.Contains("/Art/Characters/");

            if (!isWen && !isLumi && !isSlime && !isNpc && !isEnv && !isCh2World && !isPortrait && !isCh2Char)
                return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;

            if (isPortrait)
            {
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100f;
            }
            else if (isCh2Mist)
            {
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 16f;
            }
            else if (isCh2Char)
            {
                importer.filterMode = FilterMode.Point;
                if (p.Contains("/enemy_")) importer.spritePixelsPerUnit = 32f;
                else if (p.Contains("/actor_")) importer.spritePixelsPerUnit = 24f;
                else importer.spritePixelsPerUnit = 20f;
            }
            else
            {
                importer.filterMode = FilterMode.Point;
                if (isSlime) importer.spritePixelsPerUnit = 32f;
                else if (isEnv || isCh2World) importer.spritePixelsPerUnit = 16f;
                else importer.spritePixelsPerUnit = 20f;
            }

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spritePivot = new Vector2(0.5f, 0.5f);
            importer.SetTextureSettings(settings);
        }
    }
}
