using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Audio;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Combat;
using MoonpetalValley.Data;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step45
    {
        private const string ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string MothArtDir = "Assets/Art/Characters/enemy_lantern_moth";
        private const string LanternArtDir = "Assets/Art/Characters/actor_bridge_lantern";
        private const string MothDataPath = "Assets/Data/Enemies/Enemy_LanternMoth.asset";
        private const string MothPrefabPath = "Assets/Prefabs/Chapter2/Enemy_LanternMoth.prefab";
        private const string RelayConfigPath = "Assets/Data/Config/RelayLanternConfig.asset";
        private const string Ch2MusicDir = "Assets/Audio/Music/Ch2";
        private const string TracksDir = "Assets/Resources/Moonpetal/Audio/Tracks";
        private const string LibraryPath = "Assets/Resources/Moonpetal/Audio/MusicLibrary.asset";

        [MenuItem("Moonpetal/Step 45/1. Build Lantern Relay, Moth Prefab & Bind T07-T08 Audio", false, 450)]
        public static void BuildStep45()
        {
            Sprite[] mothSprites = DiscoverAndConfigureSprites(MothArtDir, 32f);
            Sprite[] lanternSprites = DiscoverAndConfigureSprites(LanternArtDir, 24f);
            if (mothSprites.Length == 0) { Debug.LogError("[MV][Step45][BLOCKED] Khong tim thay sprite trong " + MothArtDir + "!"); return; }
            if (lanternSprites.Length == 0) { Debug.LogError("[MV][Step45][BLOCKED] Khong tim thay sprite trong " + LanternArtDir + "!"); return; }

            EnemyData mothData = CreateOrUpdateMothData();
            LanternMothController mothPrefab = BuildMothPrefab(mothData, mothSprites);

            if (!File.Exists(ScenePath)) { Debug.LogError("[MV][Step45] Khong tim thay scene: " + ScenePath); return; }
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            BuildRelayInScene(mothPrefab, lanternSprites);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            int audioBound = BindChapter2Audio();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(string.Format("[MV][Step45] BUILD THANH CONG! MothFrames={0}, LanternFrames={1}, Ch2AudioBound={2}/2",
                mothSprites.Length, lanternSprites.Length, audioBound));
        }

        [MenuItem("Moonpetal/Step 45/2. Validate Step 45", false, 451)]
        public static void ValidateStep45()
        {
            int errors = 0, warnings = 0;
            GameObject prefabGo = AssetDatabase.LoadAssetAtPath<GameObject>(MothPrefabPath);
            if (prefabGo == null) { Debug.LogError("[MV][Step45] Thieu prefab " + MothPrefabPath); errors++; }
            else
            {
                if (prefabGo.GetComponentInChildren<Animator>(true) != null) { Debug.LogError("[MV][Step45] Enemy_LanternMoth van con Animator!"); errors++; }
                if (prefabGo.GetComponent<LanternMothController>() == null) { Debug.LogError("[MV][Step45] Enemy_LanternMoth thieu LanternMothController!"); errors++; }
            }

            string pcPath = "Assets/Scripts/Player/PlayerController2D.cs";
            if (!File.Exists(pcPath) || !File.ReadAllText(pcPath).Contains("[CR-CH2-DASH]"))
            {
                Debug.LogError("[MV][Step45] PlayerController2D.cs chua co patch [CR-CH2-DASH]!");
                errors++;
            }

            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            LanternRelayManager mgr = UnityEngine.Object.FindAnyObjectByType<LanternRelayManager>();
            BridgeLanternPost[] posts = UnityEngine.Object.FindObjectsByType<BridgeLanternPost>(FindObjectsInactive.Include);
            LanternRelayStation[] stations = UnityEngine.Object.FindObjectsByType<LanternRelayStation>(FindObjectsInactive.Include);
            if (mgr == null) { Debug.LogError("[MV][Step45] Riverbank_Chapter2 thieu LanternRelayManager!"); errors++; }
            if (posts.Length != 6) { Debug.LogError("[MV][Step45] Can dung 6 BridgeLanternPost, hien co: " + posts.Length); errors++; }
            if (stations.Length < 2) { Debug.LogError("[MV][Step45] Can 2 LanternRelayStation (Dong/Tay), hien co: " + stations.Length); errors++; }

            MusicTrackSO t07 = AssetDatabase.LoadAssetAtPath<MusicTrackSO>(TracksDir + "/MV_Music_T07_RiverDay.asset");
            MusicTrackSO t08 = AssetDatabase.LoadAssetAtPath<MusicTrackSO>(TracksDir + "/MV_Music_T08_LanternRelay.asset");
            if (t07 == null || t07.clip == null || t07.isDormant) { Debug.LogWarning("[MV][Step45] T07_RiverDay chua co clip (hoac dang dormant)."); warnings++; }
            if (t08 == null || t08.clip == null || t08.isDormant) { Debug.LogWarning("[MV][Step45] T08_LanternRelay chua co clip (hoac dang dormant)."); warnings++; }

            Debug.Log(string.Format("[MV][Step45] VALIDATE {0} | Errors={1}, Warnings={2}, Lanterns={3}, Stations={4}",
                errors == 0 ? "PASS" : "FAIL", errors, warnings, posts.Length, stations.Length));
        }

        private static Sprite[] DiscoverAndConfigureSprites(string folder, float ppu)
        {
            var list = new List<Sprite>();
            if (!AssetDatabase.IsValidFolder(folder)) return list.ToArray();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
            var paths = new List<string>();
            for (int i = 0; i < guids.Length; i++) paths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
            paths.Sort(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < paths.Count; i++)
            {
                TextureImporter ti = AssetImporter.GetAtPath(paths[i]) as TextureImporter;
                if (ti != null)
                {
                    bool dirty = false;
                    if (ti.textureType != TextureImporterType.Sprite) { ti.textureType = TextureImporterType.Sprite; dirty = true; }
                    if (ti.spriteImportMode != SpriteImportMode.Single) { ti.spriteImportMode = SpriteImportMode.Single; dirty = true; }
                    if (Mathf.Abs(ti.spritePixelsPerUnit - ppu) > 0.01f) { ti.spritePixelsPerUnit = ppu; dirty = true; }
                    if (ti.filterMode != FilterMode.Point) { ti.filterMode = FilterMode.Point; dirty = true; }
                    if (ti.textureCompression != TextureImporterCompression.Uncompressed) { ti.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
                    TextureImporterSettings settings = new TextureImporterSettings();
                    ti.ReadTextureSettings(settings);
                    if (settings.spriteMeshType != SpriteMeshType.FullRect) { settings.spriteMeshType = SpriteMeshType.FullRect; ti.SetTextureSettings(settings); dirty = true; }
                    if (dirty) ti.SaveAndReimport();
                }
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(paths[i]);
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }

        private static EnemyData CreateOrUpdateMothData()
        {
            EnsureFolder("Assets/Data/Enemies");
            EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(MothDataPath);
            if (data == null) { data = ScriptableObject.CreateInstance<EnemyData>(); AssetDatabase.CreateAsset(data, MothDataPath); }
            data.enemyID = "enemy_lantern_moth"; data.enemyNameKey = "name_enemy_lantern_moth"; data.attackPattern = EnemyAttackPattern.LanternDrain;
            data.maxHP = 30; data.contactDamage = 0; data.moveSpeed = 2.65f; data.detectionRadius = 12f; data.appliesSlowOnHit = false;
            EditorUtility.SetDirty(data);
            return data;
        }

        private static LanternMothController BuildMothPrefab(EnemyData data, Sprite[] frames)
        {
            EnsureFolder("Assets/Prefabs/Chapter2");
            GameObject root = new GameObject("Enemy_LanternMoth") { layer = 9 };
            Rigidbody2D rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; rb.freezeRotation = true; rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            CircleCollider2D col = root.AddComponent<CircleCollider2D>();
            col.isTrigger = true; col.radius = 0.45f;

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(root.transform, false); vis.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            SpriteRenderer sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = frames[0]; sr.sortingLayerName = "WorldObjects";

            GameObject shadow = CreateCircle("GroundShadow", root.transform, new Vector3(0f, -0.20f, 0f), new Vector3(0.55f, 0.24f, 1f), new Color(0f, 0f, 0f, 0.28f), -500);
            GameObject hpRoot = new GameObject("HPBar");
            hpRoot.transform.SetParent(root.transform, false); hpRoot.transform.localPosition = new Vector3(0f, 0.92f, 0f);
            CreateSquare("BG", hpRoot.transform, Vector3.zero, new Vector3(0.95f, 0.13f, 1f), new Color(0.12f, 0.12f, 0.18f, 0.85f), 1500);
            GameObject hpFill = CreateSquare("Fill", hpRoot.transform, Vector3.zero, new Vector3(0.90f, 0.09f, 1f), new Color(0.76f, 0.44f, 1f, 1f), 1501);
            hpRoot.SetActive(false);

            LanternMothController ctrl = root.AddComponent<LanternMothController>();
            SerializedObject so = new SerializedObject(ctrl);
            so.FindProperty("enemyData").objectReferenceValue = data;
            so.FindProperty("rb").objectReferenceValue = rb;
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.FindProperty("hpBarRoot").objectReferenceValue = hpRoot;
            so.FindProperty("hpBarFill").objectReferenceValue = hpFill.transform;
            so.FindProperty("groundShadow").objectReferenceValue = shadow;
            SerializedProperty arr = so.FindProperty("idleFrames");
            arr.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++) arr.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, MothPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved.GetComponent<LanternMothController>();
        }

        private static void BuildRelayInScene(LanternMothController mothPrefab, Sprite[] lanternSprites)
        {
            GameObject oldRoot = GameObject.Find("LanternRelaySystem");
            if (oldRoot != null) UnityEngine.Object.DestroyImmediate(oldRoot);

            GameObject sysRoot = new GameObject("LanternRelaySystem");
            LanternRelayManager mgr = sysRoot.AddComponent<LanternRelayManager>();
            LanternRelayHUDView hud = sysRoot.AddComponent<LanternRelayHUDView>();

            Sprite litSpr = PickByKeyword(lanternSprites, "lit", lanternSprites[0]);
            Sprite dimSpr = PickByKeyword(lanternSprites, "dim", litSpr);
            Sprite outSpr = PickByKeyword(lanternSprites, "out", dimSpr);

            float[] xPositions = { -6.5f, -3.9f, -1.3f, 1.3f, 3.9f, 6.5f };
            var posts = new List<BridgeLanternPost>();
            for (int i = 0; i < xPositions.Length; i++)
            {
                GameObject postGo = new GameObject("BridgeLantern_" + (i + 1));
                postGo.transform.SetParent(sysRoot.transform, false);
                postGo.transform.position = new Vector3(xPositions[i], 1.15f, 0f);
                SpriteRenderer sr = postGo.AddComponent<SpriteRenderer>();
                sr.sprite = litSpr; sr.sortingLayerName = "WorldObjects";

                GameObject glowGo = CreateCircle("Glow", postGo.transform, new Vector3(0f, 0.35f, 0f), new Vector3(2.2f, 2.2f, 1f), new Color(1f, 0.86f, 0.42f, 0.42f), -20);
                GameObject barRoot = new GameObject("LightBar");
                barRoot.transform.SetParent(postGo.transform, false); barRoot.transform.localPosition = new Vector3(0f, 1.05f, 0f);
                CreateSquare("BG", barRoot.transform, Vector3.zero, new Vector3(1.16f, 0.18f, 1f), new Color(0.10f, 0.12f, 0.18f, 0.88f), 1500);
                GameObject barFill = CreateSquare("Fill", barRoot.transform, Vector3.zero, new Vector3(1.10f, 0.12f, 1f), new Color(1f, 0.90f, 0.45f, 1f), 1501);
                barRoot.SetActive(false);

                BridgeLanternPost post = postGo.AddComponent<BridgeLanternPost>();
                SerializedObject pso = new SerializedObject(post);
                pso.FindProperty("spriteRenderer").objectReferenceValue = sr;
                pso.FindProperty("glowRenderer").objectReferenceValue = glowGo.GetComponent<SpriteRenderer>();
                pso.FindProperty("litSprite").objectReferenceValue = litSpr;
                pso.FindProperty("dimSprite").objectReferenceValue = dimSpr;
                pso.FindProperty("outSprite").objectReferenceValue = outSpr;
                pso.FindProperty("lightBarRoot").objectReferenceValue = barRoot;
                pso.FindProperty("lightBarFill").objectReferenceValue = barFill.transform;
                pso.FindProperty("lanternIndex").intValue = i;
                pso.FindProperty("interactRadius").floatValue = 2.1f;
                pso.ApplyModifiedPropertiesWithoutUndo();
                posts.Add(post);
            }

            CreateStation(sysRoot.transform, "RelayStation_East", new Vector3(8.8f, 0.5f, 0f), LanternRelayStation.BankSide.EastKael);
            CreateStation(sysRoot.transform, "RelayStation_West", new Vector3(-9.2f, 0.5f, 0f), LanternRelayStation.BankSide.WestMara);

            SerializedObject mso = new SerializedObject(mgr);
            mso.FindProperty("config").objectReferenceValue = AssetDatabase.LoadAssetAtPath<RelayLanternConfigSO>(RelayConfigPath);
            mso.FindProperty("mothPrefab").objectReferenceValue = mothPrefab;
            mso.FindProperty("hudView").objectReferenceValue = hud;
            SerializedProperty lp = mso.FindProperty("lanterns");
            lp.arraySize = posts.Count;
            for (int i = 0; i < posts.Count; i++) lp.GetArrayElementAtIndex(i).objectReferenceValue = posts[i];
            mso.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateStation(Transform parent, string name, Vector3 pos, LanternRelayStation.BankSide side)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false); go.transform.position = pos;
            LanternRelayStation st = go.AddComponent<LanternRelayStation>();
            SerializedObject so = new SerializedObject(st);
            so.FindProperty("side").enumValueIndex = (int)side;
            so.FindProperty("interactRadius").floatValue = 2.6f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static int BindChapter2Audio()
        {
            int bound = 0;
            if (BindSingleTrack("MV_Music_T07_RiverDay", new[] { "MV_Music_T07_RiverDay", "T07" })) bound++;
            if (BindSingleTrack("MV_Music_T08_LanternRelay", new[] { "MV_Music_T08_LanternRelay", "T08" })) bound++;
            MusicLibrarySO lib = AssetDatabase.LoadAssetAtPath<MusicLibrarySO>(LibraryPath);
            if (lib != null) EditorUtility.SetDirty(lib);
            return bound;
        }

        private static bool BindSingleTrack(string trackAssetStem, string[] clipStems)
        {
            AudioClip clip = null; string clipPath = null;
            string[] exts = { ".wav", ".ogg", ".mp3" };
            for (int s = 0; s < clipStems.Length && clip == null; s++)
                for (int e = 0; e < exts.Length && clip == null; e++)
                {
                    string candidate = Ch2MusicDir + "/" + clipStems[s] + exts[e];
                    clip = AssetDatabase.LoadAssetAtPath<AudioClip>(candidate);
                    if (clip != null) clipPath = candidate;
                }
            if (clip == null) return false;

            AudioImporter imp = AssetImporter.GetAtPath(clipPath) as AudioImporter;
            if (imp != null)
            {
                AudioImporterSampleSettings sample = imp.defaultSampleSettings;
                sample.compressionFormat = AudioCompressionFormat.Vorbis;
                sample.quality = 0.7f; sample.loadType = AudioClipLoadType.Streaming;
                imp.defaultSampleSettings = sample; imp.loadInBackground = true; imp.SaveAndReimport();
                clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);
            }

            MusicTrackSO track = AssetDatabase.LoadAssetAtPath<MusicTrackSO>(TracksDir + "/" + trackAssetStem + ".asset");
            if (track == null) return false;
            track.clip = clip; track.isDormant = false;
            EditorUtility.SetDirty(track);
            return true;
        }

        private static Sprite PickByKeyword(Sprite[] arr, string keyword, Sprite fallback)
        {
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] != null && arr[i].name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) return arr[i];
            return fallback;
        }

        private static GameObject CreateSquare(string name, Transform parent, Vector3 pos, Vector3 scale, Color color, int order)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false); go.transform.localPosition = pos; go.transform.localScale = scale;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = GetSolidSprite(); sr.color = color; sr.sortingLayerName = "WorldObjects"; sr.sortingOrder = order;
            return go;
        }

        private static GameObject CreateCircle(string name, Transform parent, Vector3 pos, Vector3 scale, Color color, int order)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false); go.transform.localPosition = pos; go.transform.localScale = scale;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = GetGlowSprite(); sr.color = color; sr.sortingLayerName = "WorldObjects"; sr.sortingOrder = order;
            return go;
        }

        private static Sprite GetSolidSprite()
        {
            const string path = "Assets/Art/Sprites/Generated/step45_solid_white.png";
            EnsureFolder("Assets/Art/Sprites/Generated");
            if (!File.Exists(path))
            {
                Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                Color[] px = new Color[16]; for (int i = 0; i < px.Length; i++) px[i] = Color.white;
                tex.SetPixels(px); tex.Apply(); File.WriteAllBytes(path, tex.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(path);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti != null) { ti.textureType = TextureImporterType.Sprite; ti.spritePixelsPerUnit = 4f; ti.filterMode = FilterMode.Point; ti.SaveAndReimport(); }
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite GetGlowSprite()
        {
            const string path = "Assets/Art/Sprites/Generated/step45_soft_glow.png";
            EnsureFolder("Assets/Art/Sprites/Generated");
            if (!File.Exists(path))
            {
                int sz = 32; Texture2D tex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
                Vector2 c = new Vector2((sz - 1) * 0.5f, (sz - 1) * 0.5f); float r = sz * 0.5f;
                for (int y = 0; y < sz; y++) for (int x = 0; x < sz; x++) { float a = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), c) / r); tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a)); }
                tex.Apply(); File.WriteAllBytes(path, tex.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(path);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti != null) { ti.textureType = TextureImporterType.Sprite; ti.spritePixelsPerUnit = 32f; ti.filterMode = FilterMode.Bilinear; ti.SaveAndReimport(); }
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void EnsureFolder(string folderPath)
        {
            string[] parts = folderPath.Split('/'); string cur = parts[0];
            for (int i = 1; i < parts.Length; i++) { string next = cur + "/" + parts[i]; if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]); cur = next; }
        }
    }
}
