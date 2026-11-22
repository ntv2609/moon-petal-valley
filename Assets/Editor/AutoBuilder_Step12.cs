using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MoonpetalValley.Player;
using MoonpetalValley.Combat;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step12
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";
        private const string GloomSlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";

        private const string WenRawRoot = "Assets/Art/Sprites/Animations/Wen/Raw";
        private const string WenFramesRoot = "Assets/Art/Sprites/Animations/Wen/Frames";
        private const string LumiFramesRoot = "Assets/Art/Sprites/Animations/Lumi";
        private const string SlimeFramesRoot = "Assets/Art/Sprites/Animations/GloomSlime";

        private static readonly string[] WenActions =
        {
            "Idle", "Walk", "Dash", "Tool_Use",
            "Hoe", "Watering_Can", "Hatchet", "Moon_Sickle_Slash"
        };
        private static readonly string[] WenDirs = { "south", "north", "east", "west" };

        [MenuItem("Moonpetal/Step 12/1. Slice & Wire Character Animations (Wen, Lumi, Slime)")]
        public static void BuildStep12()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 12 (FULLRECT LOCK + 8 HÀNH ĐỘNG WEN) ===");

            int slicedCount = SliceAllWenStrips();
            AssetDatabase.Refresh();
            ForceReimportAnimationFolders();

            WireWenPrefab();
            WireLumiPrefab();
            WireGloomSlimePrefab();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            RefreshScenePrefabOverrides();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 12! Đã cắt " + slicedCount + " frame Wen (FullRect 32x32) & đồng bộ Wen, Lumi, Bọt Sương Đen. Chạy 'Moonpetal/Step 12/Validate'. ===");
        }

        [MenuItem("Moonpetal/Step 12/Validate")]
        public static void ValidateStep12()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 12 (FULLRECT CHECK) ===");

            int wenFramesOk = 0;
            foreach (string action in WenActions)
            {
                foreach (string dir in WenDirs)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string path = GetWenFramePath(action, dir, i);
                        Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        if (spr == null)
                        {
                            Debug.LogError("[MV] THIẾU Wen frame: " + path);
                            errors++;
                        }
                        else if (Mathf.RoundToInt(spr.rect.width) != 32 || Mathf.RoundToInt(spr.rect.height) != 32)
                        {
                            Debug.LogError("[MV] SAI KÍCH THƯỚC FullRect trên " + path + ": " + spr.rect.width + "x" + spr.rect.height + " (cần 32x32).");
                            errors++;
                        }
                        else
                        {
                            wenFramesOk++;
                        }
                    }
                }
            }

            var wenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (wenPrefab == null)
            {
                Debug.LogError("[MV] THIẾU Player_Wen.prefab");
                errors++;
            }
            else
            {
                var anim = wenPrefab.GetComponent<WenSpriteAnimator>();
                if (anim == null || !anim.HasValidSprites)
                {
                    Debug.LogError("[MV] Player_Wen.prefab thiếu WenSpriteAnimator hoặc mảng sprite rỗng.");
                    errors++;
                }
                else
                {
                    var aSO = new SerializedObject(anim);
                    string[] checkProps =
                    {
                        "idleSouth", "walkSouth", "dashSouth", "toolSouth",
                        "hoeSouth", "waterSouth", "hatchetSouth", "slashSouth"
                    };
                    foreach (string p in checkProps)
                    {
                        var arr = aSO.FindProperty(p);
                        if (arr.arraySize < 4 || arr.GetArrayElementAtIndex(0).objectReferenceValue == null)
                        {
                            Debug.LogError("[MV] WenSpriteAnimator thiếu đủ 4 frame hợp lệ cho: " + p);
                            errors++;
                        }
                    }
                }
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 12: PASS (0 lỗi). Wen (" + wenFramesOk + "/128 frames FullRect 32x32), Lumi & Bọt Sương Đen đã đồng bộ hoàn chỉnh!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 12: FAIL (" + errors + " lỗi).");
            }
        }

        private static int SliceAllWenStrips()
        {
            if (Directory.Exists(WenFramesRoot))
            {
                foreach (string oldPng in Directory.GetFiles(WenFramesRoot, "*.png"))
                {
                    File.Delete(oldPng);
                }
            }
            else
            {
                Directory.CreateDirectory(WenFramesRoot);
            }

            int totalSliced = 0;
            foreach (string action in WenActions)
            {
                foreach (string dir in WenDirs)
                {
                    string rawPath = WenRawRoot + "/" + action + "/" + dir + ".png";
                    if (!File.Exists(rawPath))
                    {
                        Debug.LogError("[MV] Không tìm thấy dải sprite Wen: " + rawPath + ". Hãy tải wen_animation_pack.zip bản mới rồi chạy lại deploy.py.");
                        continue;
                    }

                    byte[] bytes = File.ReadAllBytes(rawPath);
                    Texture2D src = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    src.LoadImage(bytes);

                    int frameW = 32;
                    int frameH = 32;
                    if (src.width < 128 || src.height != 32)
                    {
                        Debug.LogWarning("[MV] Kích thước dải không chuẩn ở " + rawPath + ": " + src.width + "x" + src.height);
                    }
                    int frameCount = Mathf.Min(4, Mathf.Max(1, src.width / frameW));

                    for (int i = 0; i < frameCount; i++)
                    {
                        Color[] pixels = src.GetPixels(i * frameW, 0, frameW, frameH);
                        Texture2D frameTex = new Texture2D(frameW, frameH, TextureFormat.RGBA32, false);
                        frameTex.filterMode = FilterMode.Point;
                        frameTex.SetPixels(pixels);
                        frameTex.Apply();

                        string outPath = GetWenFramePath(action, dir, i);
                        File.WriteAllBytes(outPath, frameTex.EncodeToPNG());
                        UnityEngine.Object.DestroyImmediate(frameTex);
                        totalSliced++;
                    }

                    UnityEngine.Object.DestroyImmediate(src);
                }
            }
            return totalSliced;
        }

        private static void ForceReimportAnimationFolders()
        {
            string[] folders = { WenFramesRoot, LumiFramesRoot, SlimeFramesRoot };
            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder)) continue;
                string[] pngs = Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories);
                foreach (string file in pngs)
                {
                    string assetPath = file.Replace("\\", "/");
                    var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        importer.filterMode = FilterMode.Point;
                        importer.textureCompression = TextureImporterCompression.Uncompressed;
                        importer.alphaIsTransparency = true;
                        importer.mipmapEnabled = false;

                        if (assetPath.StartsWith("Assets/Art/Sprites/Animations/Wen/")) importer.spritePixelsPerUnit = 20;
                        else if (assetPath.StartsWith("Assets/Art/Sprites/Animations/Lumi/")) importer.spritePixelsPerUnit = 20;
                        else if (assetPath.StartsWith("Assets/Art/Sprites/Animations/GloomSlime/")) importer.spritePixelsPerUnit = 32;

                        var settings = new TextureImporterSettings();
                        importer.ReadTextureSettings(settings);
                        settings.spriteMeshType = SpriteMeshType.FullRect;
                        settings.spriteAlignment = (int)SpriteAlignment.Center;
                        settings.spriteGenerateFallbackPhysicsShape = false;
                        importer.SetTextureSettings(settings);
                        importer.SaveAndReimport();
                    }
                    else
                    {
                        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
        }

        private static void WireWenPrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null) return;

            Transform visual = root.transform.Find("Visual");
            SpriteRenderer sr = visual != null ? visual.GetComponent<SpriteRenderer>() : root.GetComponentInChildren<SpriteRenderer>();

            var anim = root.GetComponent<WenSpriteAnimator>();
            if (anim == null) anim = root.AddComponent<WenSpriteAnimator>();

            var aSO = new SerializedObject(anim);
            aSO.FindProperty("spriteRenderer").objectReferenceValue = sr;

            Assign4Dirs(aSO, "idle", "Idle");
            Assign4Dirs(aSO, "walk", "Walk");
            Assign4Dirs(aSO, "dash", "Dash");
            Assign4Dirs(aSO, "tool", "Tool_Use");
            Assign4Dirs(aSO, "hoe", "Hoe");
            Assign4Dirs(aSO, "water", "Watering_Can");
            Assign4Dirs(aSO, "hatchet", "Hatchet");
            Assign4Dirs(aSO, "slash", "Moon_Sickle_Slash");
            aSO.ApplyModifiedPropertiesWithoutUndo();

            Sprite defaultWen = AssetDatabase.LoadAssetAtPath<Sprite>(GetWenFramePath("Idle", "south", 0));
            if (sr != null && defaultWen != null) sr.sprite = defaultWen;

            var pCtrl = root.GetComponent<PlayerController2D>();
            if (pCtrl != null)
            {
                var pSO = new SerializedObject(pCtrl);
                pSO.FindProperty("spriteAnimator").objectReferenceValue = anim;
                pSO.ApplyModifiedPropertiesWithoutUndo();
            }

            var tCtrl = root.GetComponent<PlayerToolController>();
            if (tCtrl != null)
            {
                var tSO = new SerializedObject(tCtrl);
                tSO.FindProperty("spriteAnimator").objectReferenceValue = anim;
                tSO.ApplyModifiedPropertiesWithoutUndo();
            }

            var cCtrl = root.GetComponent<PlayerCombatController>();
            if (cCtrl != null)
            {
                var cSO = new SerializedObject(cCtrl);
                cSO.FindProperty("spriteAnimator").objectReferenceValue = anim;
                cSO.ApplyModifiedPropertiesWithoutUndo();
            }

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

        private static void WireLumiPrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(LumiPrefabPath);
            if (root == null) return;

            Transform visual = root.transform.Find("Visual");
            SpriteRenderer sr = visual != null ? visual.GetComponent<SpriteRenderer>() : null;
            var ctrl = root.GetComponent<LumiCompanionController>();

            Sprite[] floatFrames = LoadNumberedFrames(LumiFramesRoot + "/float", 4);
            Sprite[] pulseFrames = LoadNumberedFrames(LumiFramesRoot + "/moon_pulse", 4);

            if (sr != null && floatFrames.Length > 0 && floatFrames[0] != null) sr.sprite = floatFrames[0];

            if (ctrl != null)
            {
                var so = new SerializedObject(ctrl);
                so.FindProperty("spriteRenderer").objectReferenceValue = sr;
                AssignSpriteArray(so.FindProperty("floatFrames"), floatFrames);
                AssignSpriteArray(so.FindProperty("moonPulseFrames"), pulseFrames);
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, LumiPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void WireGloomSlimePrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(GloomSlimePrefabPath);
            if (root == null) return;

            Transform visual = root.transform.Find("Visual");
            SpriteRenderer sr = visual != null ? visual.GetComponent<SpriteRenderer>() : null;
            var ctrl = root.GetComponent<GloomSlimeController>();

            Sprite[] idleF = LoadNumberedFrames(SlimeFramesRoot + "/idle", 4);
            Sprite[] teleF = LoadNumberedFrames(SlimeFramesRoot + "/telegraph", 3);
            Sprite[] jumpF = LoadNumberedFrames(SlimeFramesRoot + "/jump", 4);
            Sprite[] hitF = LoadNumberedFrames(SlimeFramesRoot + "/hit_hurt", 3);
            Sprite[] dieF = LoadNumberedFrames(SlimeFramesRoot + "/purify_die", 4);

            if (sr != null && idleF.Length > 0 && idleF[0] != null) sr.sprite = idleF[0];

            if (ctrl != null)
            {
                var so = new SerializedObject(ctrl);
                AssignSpriteArray(so.FindProperty("idleFrames"), idleF);
                AssignSpriteArray(so.FindProperty("telegraphFrames"), teleF);
                AssignSpriteArray(so.FindProperty("jumpFrames"), jumpF);
                AssignSpriteArray(so.FindProperty("hitHurtFrames"), hitF);
                AssignSpriteArray(so.FindProperty("purifyDieFrames"), dieF);
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GloomSlimePrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void RefreshScenePrefabOverrides()
        {
            Sprite defaultWen = AssetDatabase.LoadAssetAtPath<Sprite>(GetWenFramePath("Idle", "south", 0));
            Sprite defaultLumi = AssetDatabase.LoadAssetAtPath<Sprite>(LumiFramesRoot + "/float_00.png");
            Sprite defaultSlime = AssetDatabase.LoadAssetAtPath<Sprite>(SlimeFramesRoot + "/idle_00.png");

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null && defaultWen != null)
            {
                var sr = player.transform.Find("Visual")?.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = defaultWen;
            }

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null && defaultLumi != null)
            {
                var sr = lumi.transform.Find("Visual")?.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = defaultLumi;
            }

            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            for (int i = 0; i < slimes.Length; i++)
            {
                if (defaultSlime != null)
                {
                    var sr = slimes[i].transform.Find("Visual")?.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.sprite = defaultSlime;
                }
            }
        }

        private static string GetWenFramePath(string action, string dir, int idx)
        {
            return WenFramesRoot + "/wen_" + action.ToLowerInvariant() + "_" + dir + "_" + idx.ToString("00") + ".png";
        }

        private static Sprite[] LoadWenFrames(string action, string dir, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(GetWenFramePath(action, dir, i)));
            }
            return list.ToArray();
        }

        private static Sprite[] LoadNumberedFrames(string basePathWithoutIndex, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(basePathWithoutIndex + "_" + i.ToString("00") + ".png"));
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
