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

            if (!isWen && !isLumi && !isSlime && !isNpc && !isEnv) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;

            if (isSlime) importer.spritePixelsPerUnit = 32f;
            else if (isEnv) importer.spritePixelsPerUnit = 16f;
            else importer.spritePixelsPerUnit = 20f;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spritePivot = new Vector2(0.5f, 0.5f);
            importer.SetTextureSettings(settings);
        }
    }
}
