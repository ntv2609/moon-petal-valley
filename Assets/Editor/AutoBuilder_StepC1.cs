using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MoonpetalValley.Player;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepC1
    {
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string WenRawRoot = "Assets/Art/Sprites/Animations/Wen/Raw";
        private const string WenFramesRoot = "Assets/Art/Sprites/Animations/Wen/Frames";
        private const string VfxRawRoot = "Assets/Art/Sprites/VFX/Combat/Raw";
        private const string VfxFramesRoot = "Assets/Art/Sprites/VFX/Combat/Frames";
        private const string VfxRoot = "Assets/Art/Sprites/VFX/Combat";

        private static readonly string[] ComboActions =
        {
            "Combo2_Reverse",
            "Combo3_Spin",
            "Dash_Strike"
        };

        private static readonly string[] WenDirs = { "south", "north", "east", "west" };

        private struct VfxStripSpec
        {
            public string Name;
            public int FrameSize;
            public int FrameCount;
        }

        private static readonly VfxStripSpec[] VfxSpecs =
        {
            new VfxStripSpec { Name = "vfx_combo1_slash",    FrameSize = 48, FrameCount = 4 },
            new VfxStripSpec { Name = "vfx_combo2_reverse",  FrameSize = 56, FrameCount = 4 },
            new VfxStripSpec { Name = "vfx_combo3_finisher", FrameSize = 72, FrameCount = 4 },
            new VfxStripSpec { Name = "vfx_dash_strike",     FrameSize = 56, FrameCount = 4 },
            new VfxStripSpec { Name = "vfx_hit_spark",       FrameSize = 32, FrameCount = 4 }
        };

        [MenuItem("Moonpetal/Step C1/1. Slice & Wire Wen Combo Animations + Combat VFX")]
        public static void BuildStepC1()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][StepC1] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][StepC1] === BẮT ĐẦU AUTOBUILDER STEP C1 (WEN COMBO ANIMATIONS + COMBAT VFX) ===");

            int wenSliced = SliceWenComboStrips();
            int vfxSliced = SliceCombatVfxStrips();

            AssetDatabase.Refresh();
            ConfigureImporters();
            WireWenPrefabComboArrays();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV][StepC1] === HOÀN TẤT STEP C1! Đã cắt " + wenSliced + " frame Wen combo (32x32) và "
                      + vfxSliced + " frame Combat VFX, đồng bộ Player_Wen.prefab. Hãy chạy 'Moonpetal/Step C1/Validate'. ===");
        }

        [MenuItem("Moonpetal/Step C1/Validate")]
        public static void ValidateStepC1()
        {
            int errors = 0;
            Debug.Log("[MV][StepC1] === BẮT ĐẦU VALIDATE STEP C1 ===");

            int wenOk = 0;
            foreach (string action in ComboActions)
            {
                foreach (string dir in WenDirs)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string path = GetWenFramePath(action, dir, i);
                        Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        if (spr == null)
                        {
                            Debug.LogError("[MV][StepC1] THIẾU Wen combo frame: " + path);
                            errors++;
                        }
                        else if (Mathf.RoundToInt(spr.rect.width) != 32 || Mathf.RoundToInt(spr.rect.height) != 32)
                        {
                            Debug.LogError("[MV][StepC1] SAI KÍCH THƯỚC " + path + ": " + spr.rect.width + "x" + spr.rect.height + " (cần 32x32).");
                            errors++;
                        }
                        else
                        {
                            wenOk++;
                        }
                    }
                }
            }

            // Kiểm tra 1 frame gốc Step 12 còn nguyên vẹn
            Sprite origCheck = AssetDatabase.LoadAssetAtPath<Sprite>(GetWenFramePath("Moon_Sickle_Slash", "south", 0));
            if (origCheck == null)
            {
                Debug.LogError("[MV][StepC1] CẢNH BÁO: Frame gốc Moon_Sickle_Slash bị thiếu! Hãy chạy lại Step 12 nếu cần.");
                errors++;
            }

            int vfxOk = 0;
            foreach (var spec in VfxSpecs)
            {
                for (int i = 0; i < spec.FrameCount; i++)
                {
                    string path = GetVfxFramePath(spec.Name, i);
                    Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (spr == null)
                    {
                        Debug.LogError("[MV][StepC1] THIẾU Combat VFX frame: " + path);
                        errors++;
                    }
                    else if (Mathf.RoundToInt(spr.rect.width) != spec.FrameSize || Mathf.RoundToInt(spr.rect.height) != spec.FrameSize)
                    {
                        Debug.LogError("[MV][StepC1] SAI KÍCH THƯỚC VFX " + path + ": " + spr.rect.width + "x" + spr.rect.height);
                        errors++;
                    }
                    else
                    {
                        vfxOk++;
                    }
                }
            }

            Sprite petalSpr = AssetDatabase.LoadAssetAtPath<Sprite>(VfxRoot + "/vfx_petal_particle.png");
            if (petalSpr == null)
            {
                Debug.LogError("[MV][StepC1] THIẾU sprite: " + VfxRoot + "/vfx_petal_particle.png");
                errors++;
            }

            GameObject wenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (wenPrefab == null)
            {
                Debug.LogError("[MV][StepC1] THIẾU Player_Wen.prefab");
                errors++;
            }
            else
            {
                WenSpriteAnimator anim = wenPrefab.GetComponent<WenSpriteAnimator>();
                if (anim == null)
                {
                    Debug.LogError("[MV][StepC1] Player_Wen.prefab thiếu WenSpriteAnimator.");
                    errors++;
                }
                else
                {
                    SerializedObject aSO = new SerializedObject(anim);
                    string[] props =
                    {
                        "combo2South", "combo2North", "combo2East", "combo2West",
                        "combo3South", "combo3North", "combo3East", "combo3West",
                        "dashStrikeSouth", "dashStrikeNorth", "dashStrikeEast", "dashStrikeWest"
                    };
                    foreach (string p in props)
                    {
                        SerializedProperty arr = aSO.FindProperty(p);
                        if (arr == null || arr.arraySize < 4 || arr.GetArrayElementAtIndex(0).objectReferenceValue == null)
                        {
                            Debug.LogError("[MV][StepC1] WenSpriteAnimator thiếu 4 frame hợp lệ cho: " + p);
                            errors++;
                        }
                    }
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV][StepC1] VALIDATE STEP C1: PASS (0 lỗi). Wen Combo (" + wenOk + "/48 frames FullRect 32x32) & Combat VFX ("
                          + vfxOk + "/20 frames + petal) đã gắn xong vào Player_Wen.prefab!");
            }
            else
            {
                Debug.LogError("[MV][StepC1] VALIDATE STEP C1: FAIL (" + errors + " lỗi).");
            }
        }

        private static int SliceWenComboStrips()
        {
            if (!Directory.Exists(WenFramesRoot)) Directory.CreateDirectory(WenFramesRoot);

            int total = 0;
            foreach (string action in ComboActions)
            {
                foreach (string dir in WenDirs)
                {
                    string rawPath = WenRawRoot + "/" + action + "/" + dir + ".png";
                    if (!File.Exists(rawPath))
                    {
                        Debug.LogError("[MV][StepC1] Không tìm thấy dải sprite: " + rawPath);
                        continue;
                    }

                    byte[] bytes = File.ReadAllBytes(rawPath);
                    Texture2D src = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    src.LoadImage(bytes);

                    int frameCount = Mathf.Min(4, Mathf.Max(1, src.width / 32));
                    for (int i = 0; i < frameCount; i++)
                    {
                        Color[] pixels = src.GetPixels(i * 32, 0, 32, 32);
                        Texture2D frameTex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
                        frameTex.filterMode = FilterMode.Point;
                        frameTex.SetPixels(pixels);
                        frameTex.Apply();

                        string outPath = GetWenFramePath(action, dir, i);
                        File.WriteAllBytes(outPath, frameTex.EncodeToPNG());
                        UnityEngine.Object.DestroyImmediate(frameTex);
                        total++;
                    }
                    UnityEngine.Object.DestroyImmediate(src);
                }
            }
            return total;
        }

        private static int SliceCombatVfxStrips()
        {
            if (!Directory.Exists(VfxFramesRoot)) Directory.CreateDirectory(VfxFramesRoot);

            int total = 0;
            foreach (var spec in VfxSpecs)
            {
                string rawPath = VfxRawRoot + "/" + spec.Name + ".png";
                if (!File.Exists(rawPath))
                {
                    Debug.LogError("[MV][StepC1] Không tìm thấy dải VFX: " + rawPath);
                    continue;
                }

                byte[] bytes = File.ReadAllBytes(rawPath);
                Texture2D src = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                src.LoadImage(bytes);

                int count = Mathf.Min(spec.FrameCount, Mathf.Max(1, src.width / spec.FrameSize));
                for (int i = 0; i < count; i++)
                {
                    Color[] pixels = src.GetPixels(i * spec.FrameSize, 0, spec.FrameSize, spec.FrameSize);
                    Texture2D frameTex = new Texture2D(spec.FrameSize, spec.FrameSize, TextureFormat.RGBA32, false);
                    frameTex.filterMode = FilterMode.Point;
                    frameTex.SetPixels(pixels);
                    frameTex.Apply();

                    string outPath = GetVfxFramePath(spec.Name, i);
                    File.WriteAllBytes(outPath, frameTex.EncodeToPNG());
                    UnityEngine.Object.DestroyImmediate(frameTex);
                    total++;
                }
                UnityEngine.Object.DestroyImmediate(src);
            }
            return total;
        }

        private static void ConfigureImporters()
        {
            List<string> paths = new List<string>();
            foreach (string action in ComboActions)
            {
                foreach (string dir in WenDirs)
                {
                    for (int i = 0; i < 4; i++) paths.Add(GetWenFramePath(action, dir, i));
                }
            }
            foreach (var spec in VfxSpecs)
            {
                for (int i = 0; i < spec.FrameCount; i++) paths.Add(GetVfxFramePath(spec.Name, i));
            }
            paths.Add(VfxRoot + "/vfx_petal_particle.png");

            foreach (string assetPath in paths)
            {
                if (!File.Exists(assetPath)) continue;
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null)
                {
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                    importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                }
                if (importer == null) continue;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.spritePixelsPerUnit = 20f;

                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteAlignment = (int)SpriteAlignment.Center;
                settings.spritePivot = new Vector2(0.5f, 0.5f);
                settings.spriteGenerateFallbackPhysicsShape = false;
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }
        }

        private static void WireWenPrefabComboArrays()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null) return;

            WenSpriteAnimator anim = root.GetComponent<WenSpriteAnimator>();
            if (anim == null) anim = root.AddComponent<WenSpriteAnimator>();

            SerializedObject aSO = new SerializedObject(anim);
            Assign4Dirs(aSO, "combo2", "Combo2_Reverse");
            Assign4Dirs(aSO, "combo3", "Combo3_Spin");
            Assign4Dirs(aSO, "dashStrike", "Dash_Strike");
            aSO.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void Assign4Dirs(SerializedObject so, string propPrefix, string actionName)
        {
            AssignSpriteArray(so.FindProperty(propPrefix + "South"), LoadWenFrames(actionName, "south", 4));
            AssignSpriteArray(so.FindProperty(propPrefix + "North"), LoadWenFrames(actionName, "north", 4));
            AssignSpriteArray(so.FindProperty(propPrefix + "East"), LoadWenFrames(actionName, "east", 4));
            AssignSpriteArray(so.FindProperty(propPrefix + "West"), LoadWenFrames(actionName, "west", 4));
        }

        private static string GetWenFramePath(string action, string dir, int idx)
        {
            return WenFramesRoot + "/wen_" + action.ToLowerInvariant() + "_" + dir + "_" + idx.ToString("00") + ".png";
        }

        private static string GetVfxFramePath(string name, int idx)
        {
            return VfxFramesRoot + "/" + name + "_" + idx.ToString("00") + ".png";
        }

        private static Sprite[] LoadWenFrames(string action, string dir, int count)
        {
            List<Sprite> list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(GetWenFramePath(action, dir, i)));
            }
            return list.ToArray();
        }

        private static void AssignSpriteArray(SerializedProperty prop, Sprite[] sprites)
        {
            if (prop == null) return;
            prop.ClearArray();
            for (int i = 0; i < sprites.Length; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            }
        }
    }
}
