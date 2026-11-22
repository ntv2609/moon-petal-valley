using System.IO;
using UnityEngine;
using UnityEditor;

namespace MoonpetalValley.EditorTools
{
    public static class PlaceholderGeneratorUtility
    {
        public static Sprite EnsureSprite(string assetPath, int width, int height, System.Action<Texture2D> drawAction)
        {
            if (!File.Exists(assetPath))
            {
                string dir = Path.GetDirectoryName(assetPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Point;
                Color clear = new Color(0f, 0f, 0f, 0f);
                Color[] pixels = new Color[width * height];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;
                tex.SetPixels(pixels);

                drawAction?.Invoke(tex);
                tex.Apply();

                byte[] png = tex.EncodeToPNG();
                File.WriteAllBytes(assetPath, png);
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }

            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
                if (Mathf.Abs(importer.spritePixelsPerUnit - 16f) > 0.01f) { importer.spritePixelsPerUnit = 16f; dirty = true; }
                if (importer.filterMode != FilterMode.Point) { importer.filterMode = FilterMode.Point; dirty = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
                if (dirty) importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        public static void FillRect(Texture2D tex, int x, int y, int w, int h, Color c)
        {
            for (int py = y; py < y + h; py++)
            {
                if (py < 0 || py >= tex.height) continue;
                for (int px = x; px < x + w; px++)
                {
                    if (px < 0 || px >= tex.width) continue;
                    tex.SetPixel(px, py, c);
                }
            }
        }

        public static void DrawWenSprite(Texture2D tex)
        {
            Color boots = new Color(0.32f, 0.20f, 0.12f);
            Color cloak = new Color(0.20f, 0.56f, 0.36f);
            Color cloakShade = new Color(0.14f, 0.42f, 0.27f);
            Color scarf = new Color(0.95f, 0.78f, 0.28f);
            Color skin = new Color(0.98f, 0.85f, 0.74f);
            Color hair = new Color(0.24f, 0.18f, 0.16f);
            Color eye = new Color(0.10f, 0.12f, 0.18f);
            Color sprout = new Color(0.55f, 0.92f, 0.42f);

            FillRect(tex, 5, 1, 2, 3, boots);
            FillRect(tex, 9, 1, 2, 3, boots);
            FillRect(tex, 4, 4, 8, 8, cloak);
            FillRect(tex, 4, 4, 2, 8, cloakShade);
            FillRect(tex, 4, 11, 8, 2, scarf);
            FillRect(tex, 4, 13, 8, 6, skin);
            tex.SetPixel(6, 15, eye);
            tex.SetPixel(9, 15, eye);
            FillRect(tex, 3, 18, 10, 3, hair);
            FillRect(tex, 3, 15, 1, 4, hair);
            FillRect(tex, 12, 15, 1, 4, hair);
            FillRect(tex, 7, 21, 2, 2, sprout);
            tex.SetPixel(9, 22, sprout);
        }

        public static void DrawLumiSprite(Texture2D tex)
        {
            Color outer = new Color(0.45f, 0.92f, 0.95f, 0.90f);
            Color inner = new Color(0.85f, 1.0f, 0.98f, 1.0f);
            Color eye = new Color(0.12f, 0.25f, 0.35f, 1.0f);

            FillRect(tex, 3, 2, 6, 7, outer);
            FillRect(tex, 4, 1, 4, 9, outer);
            FillRect(tex, 4, 3, 4, 5, inner);
            tex.SetPixel(4, 5, eye);
            tex.SetPixel(7, 5, eye);
            tex.SetPixel(5, 10, inner);
            tex.SetPixel(6, 10, inner);
        }

        public static void DrawRadialHalo(Texture2D tex)
        {
            Vector2 center = new Vector2(tex.width * 0.5f, tex.height * 0.5f);
            float maxR = tex.width * 0.5f;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float a = Mathf.Clamp01(1f - dist / maxR);
                    tex.SetPixel(x, y, new Color(0.55f, 0.96f, 0.92f, a * a * 0.42f));
                }
            }
        }

        public static void DrawWarmHalo(Texture2D tex)
        {
            Vector2 center = new Vector2(tex.width * 0.5f, tex.height * 0.5f);
            float maxR = tex.width * 0.5f;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float a = Mathf.Clamp01(1f - dist / maxR);
                    tex.SetPixel(x, y, new Color(1.0f, 0.80f, 0.36f, a * a * 0.65f));
                }
            }
        }

        public static void DrawLanternPostSprite(Texture2D tex)
        {
            Color wood = new Color(0.36f, 0.24f, 0.14f);
            Color iron = new Color(0.22f, 0.24f, 0.28f);
            Color lamp = new Color(1.0f, 0.86f, 0.42f);

            FillRect(tex, 7, 1, 2, 16, wood);
            FillRect(tex, 5, 1, 6, 2, wood);
            FillRect(tex, 7, 16, 6, 2, iron);
            FillRect(tex, 10, 11, 4, 5, lamp);
            FillRect(tex, 9, 15, 6, 1, iron);
            FillRect(tex, 9, 10, 6, 1, iron);
        }

        public static void DrawBedSprite(Texture2D tex)
        {
            Color wood = new Color(0.45f, 0.28f, 0.16f);
            Color pillow = new Color(0.96f, 0.94f, 0.86f);
            Color blanket = new Color(0.26f, 0.54f, 0.62f);
            Color trim = new Color(0.92f, 0.78f, 0.36f);

            FillRect(tex, 1, 1, 14, 22, wood);
            FillRect(tex, 2, 16, 12, 5, pillow);
            FillRect(tex, 2, 3, 12, 13, blanket);
            FillRect(tex, 2, 14, 12, 2, trim);
        }

        public static void DrawGrassTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.32f, 0.58f, 0.31f));
            Color light = new Color(0.40f, 0.68f, 0.36f);
            Color dark = new Color(0.26f, 0.48f, 0.26f);
            tex.SetPixel(3, 4, light); tex.SetPixel(3, 5, light);
            tex.SetPixel(11, 10, light); tex.SetPixel(11, 11, light);
            tex.SetPixel(7, 13, dark); tex.SetPixel(13, 3, dark);
        }

        public static void DrawPathTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.73f, 0.65f, 0.50f));
            Color pebble = new Color(0.62f, 0.54f, 0.41f);
            FillRect(tex, 2, 3, 3, 2, pebble);
            FillRect(tex, 10, 9, 4, 2, pebble);
            FillRect(tex, 5, 12, 2, 2, new Color(0.82f, 0.76f, 0.62f));
        }

        public static void DrawGreenhouseFloorTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.60f, 0.44f, 0.28f));
            Color line = new Color(0.46f, 0.33f, 0.20f);
            FillRect(tex, 0, 0, 16, 1, line);
            FillRect(tex, 0, 8, 16, 1, line);
            FillRect(tex, 7, 1, 1, 7, line);
            FillRect(tex, 13, 9, 1, 7, line);
        }

        public static void DrawWallTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.34f, 0.26f, 0.20f));
            FillRect(tex, 2, 4, 12, 9, new Color(0.56f, 0.78f, 0.84f));
            FillRect(tex, 7, 4, 2, 9, new Color(0.34f, 0.26f, 0.20f));
            FillRect(tex, 0, 14, 16, 2, new Color(0.24f, 0.18f, 0.14f));
        }

        public static void DrawFenceTile(Texture2D tex)
        {
            DrawGrassTile(tex);
            Color wood = new Color(0.54f, 0.36f, 0.20f);
            Color woodHi = new Color(0.68f, 0.48f, 0.28f);
            FillRect(tex, 2, 2, 3, 12, wood);
            FillRect(tex, 11, 2, 3, 12, wood);
            FillRect(tex, 0, 5, 16, 2, woodHi);
            FillRect(tex, 0, 10, 16, 2, woodHi);
        }

        public static void DrawTreeTile(Texture2D tex)
        {
            DrawGrassTile(tex);
            FillRect(tex, 6, 1, 4, 5, new Color(0.38f, 0.24f, 0.14f));
            FillRect(tex, 2, 5, 12, 9, new Color(0.14f, 0.36f, 0.22f));
            FillRect(tex, 4, 7, 8, 7, new Color(0.20f, 0.48f, 0.28f));
            FillRect(tex, 5, 10, 4, 3, new Color(0.28f, 0.58f, 0.34f));
        }

        public static void DrawWaterTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.22f, 0.52f, 0.78f));
            FillRect(tex, 3, 5, 5, 1, new Color(0.62f, 0.86f, 0.98f));
            FillRect(tex, 9, 11, 4, 1, new Color(0.62f, 0.86f, 0.98f));
        }

        public static void DrawFlowerbedTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.54f, 0.54f, 0.58f));
            FillRect(tex, 2, 2, 12, 12, new Color(0.36f, 0.25f, 0.18f));
            FillRect(tex, 6, 6, 4, 4, new Color(0.46f, 0.34f, 0.24f));
        }
    }
}
