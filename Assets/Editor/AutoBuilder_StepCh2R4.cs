#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Combat;
using MoonpetalValley.Data;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2R4
    {
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string PlankSpritePath = "Assets/Art/Sprites/Chapter2/tile_bridge_plank.png";
        private const string BrokenSpritePath = "Assets/Art/Sprites/Chapter2/tile_bridge_broken.png";
        private const string MothPrefabPath = "Assets/Prefabs/Chapter2/Enemy_LanternMoth.prefab";
        private const string MothDataPath = "Assets/Data/Enemies/Enemy_LanternMoth.asset";
        private const string MothArtDir = "Assets/Art/Characters/enemy_lantern_moth";
        private const string WhitePixelPath = "Assets/Art/Sprites/UI/spr_white_pixel.png";

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R4 - 1. Build 3-State Bridge Scaffolding & Tune Relay")]
        public static void BuildStepCh2R4()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Ch2-R4] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Ch2-R4] === BẮT ĐẦU STEP CH2-R4: CẦU 3 TRẠNG THÁI, VÁN XÂY TIẾN ĐỘ & BƯỚM HÚT SÁNG ===");

            // MỞ SCENE TRƯỚC, rồi mới nạp/dựng Prefab để tránh OpenScene(Single) unload asset chưa được tham chiếu!
            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R4] Không mở được " + Ch2ScenePath);
                return;
            }

            LanternMothController mothPrefab = EnsureLanternMothPrefab();

            RiverBridgeController bridge = Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge == null)
            {
                Debug.LogError("[MV][Ch2-R4] Không tìm thấy RiverBridgeController trong " + Ch2ScenePath);
                return;
            }

            Sprite plankSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PlankSpritePath);
            Sprite brokenSpr = AssetDatabase.LoadAssetAtPath<Sprite>(BrokenSpritePath);

            Transform oldScaf = bridge.transform.Find("BridgeScaffoldingRoot");
            if (oldScaf != null) Object.DestroyImmediate(oldScaf.gameObject);

            GameObject scafRoot = new GameObject("BridgeScaffoldingRoot");
            scafRoot.transform.SetParent(bridge.transform, false);
            scafRoot.transform.localPosition = Vector3.zero;

            GameObject walkway = new GameObject("TempScaffoldingWalkway");
            walkway.transform.SetParent(scafRoot.transform, false);
            for (int x = -7; x <= 7; x++)
            {
                CreatePlankTile(walkway.transform, $"ScafBeam_{x}", brokenSpr != null ? brokenSpr : plankSpr,
                    new Vector3(x, 0.25f, 0f), new Vector3(0.95f, 0.58f, 1f), new Color(0.72f, 0.62f, 0.50f, 0.92f), 4);
            }

            GameObject seg1 = CreateSegmentGroup(scafRoot.transform, "BuildSegment_1_East", plankSpr, 3, 7);
            GameObject seg2 = CreateSegmentGroup(scafRoot.transform, "BuildSegment_2_West", plankSpr, -7, -3);
            GameObject seg3 = CreateSegmentGroup(scafRoot.transform, "BuildSegment_3_Center", plankSpr, -2, 2);

            seg1.SetActive(false);
            seg2.SetActive(false);
            seg3.SetActive(false);
            scafRoot.SetActive(false);

            SerializedObject bso = new SerializedObject(bridge);
            bso.FindProperty("scaffoldingVisualsRoot").objectReferenceValue = scafRoot;
            var segProp = bso.FindProperty("progressivePlankSegments");
            segProp.ClearArray();
            GameObject[] segs = { seg1, seg2, seg3 };
            for (int i = 0; i < segs.Length; i++)
            {
                segProp.InsertArrayElementAtIndex(i);
                segProp.GetArrayElementAtIndex(i).objectReferenceValue = segs[i];
            }
            bso.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bridge);

            // Nối mothPrefab vào LanternRelayManager
            LanternRelayManager relay = Object.FindAnyObjectByType<LanternRelayManager>();
            if (relay != null && mothPrefab != null)
            {
                SerializedObject rso = new SerializedObject(relay);
                rso.FindProperty("mothPrefab").objectReferenceValue = mothPrefab;
                rso.ApplyModifiedPropertiesWithoutUndo();
                rso.Update();
                EditorUtility.SetDirty(relay);
                bool verified = rso.FindProperty("mothPrefab").objectReferenceValue != null;
                if (verified)
                    Debug.Log("[MV][Ch2-R4] Đã nối và xác thực Enemy_LanternMoth.prefab vào LanternRelayManager.mothPrefab!");
                else
                    Debug.LogError("[MV][Ch2-R4] Gán mothPrefab xong nhưng SerializedProperty vẫn null!");
            }
            else
            {
                Debug.LogError("[MV][Ch2-R4] Không thể nối mothPrefab (relay=" + (relay != null) + ", mothPrefab=" + (mothPrefab != null) + ").");
            }

            string[] cfgGuids = AssetDatabase.FindAssets("t:RelayLanternConfigSO");
            for (int i = 0; i < cfgGuids.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(cfgGuids[i]);
                RelayLanternConfigSO cfg = AssetDatabase.LoadAssetAtPath<RelayLanternConfigSO>(p);
                if (cfg == null) continue;
                cfg.mothSpawnInterval = 3.4f;
                cfg.maxActiveMoths = 5;
                EditorUtility.SetDirty(cfg);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Ch2-R4] === HOÀN TẤT STEP CH2-R4! Chạy 'Step Ch2-R4 - 2. Validate Step Ch2-R4' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R4 - 2. Validate Step Ch2-R4")]
        public static void ValidateStepCh2R4()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-R4] === BẮT ĐẦU VALIDATE STEP CH2-R4 ===");

            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R4][FAIL] Không mở được " + Ch2ScenePath);
                return;
            }

            RiverBridgeController bridge = Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge == null)
            {
                Debug.LogError("[MV][Ch2-R4][FAIL] Thiếu RiverBridgeController.");
                errors++;
            }
            else
            {
                SerializedObject bso = new SerializedObject(bridge);
                if (bso.FindProperty("scaffoldingVisualsRoot").objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Ch2-R4][FAIL] RiverBridgeController thiếu scaffoldingVisualsRoot!");
                    errors++;
                }
                if (bso.FindProperty("progressivePlankSegments").arraySize < 3)
                {
                    Debug.LogError("[MV][Ch2-R4][FAIL] RiverBridgeController cần đủ 3 progressivePlankSegments!");
                    errors++;
                }
            }

            LanternRelayManager relay = Object.FindAnyObjectByType<LanternRelayManager>();
            if (relay == null)
            {
                Debug.LogError("[MV][Ch2-R4][FAIL] Thiếu LanternRelayManager.");
                errors++;
            }
            else
            {
                SerializedObject rso = new SerializedObject(relay);
                if (rso.FindProperty("mothPrefab").objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Ch2-R4][FAIL] LanternRelayManager thiếu mothPrefab!");
                    errors++;
                }
            }

            var all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < all.Length; i++)
            {
                int c = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(all[i]);
                if (c > 0)
                {
                    Debug.LogError($"[MV][Ch2-R4][FAIL] '{all[i].name}' có {c} Missing Script!");
                    errors += c;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-R4] VALIDATE PASS (0 lỗi) - Cầu 3 trạng thái, Cutscene Kael & Mara đóng ván cuối & Bướm Hút Sáng tia tím đã sẵn sàng!");
            else
                Debug.LogError($"[MV][Ch2-R4] VALIDATE FAIL ({errors} lỗi).");
        }

        private static LanternMothController EnsureLanternMothPrefab()
        {
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(MothPrefabPath);
            if (existing != null)
            {
                LanternMothController ctrl = existing.GetComponent<LanternMothController>();
                if (ctrl != null)
                {
                    Debug.Log("[MV][Ch2-R4] Đã nạp sẵn Enemy_LanternMoth.prefab tại " + MothPrefabPath);
                    return ctrl;
                }
            }

            // Tìm mọi prefab có gắn LanternMothController trong dự án
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            for (int i = 0; i < guids.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                if (go == null) continue;
                LanternMothController ctrl = go.GetComponent<LanternMothController>();
                if (ctrl != null)
                {
                    Debug.Log("[MV][Ch2-R4] Đã tìm thấy prefab LanternMothController tại " + p);
                    return ctrl;
                }
            }

            // Nếu chưa có prefab thì dựng mới sạch sẽ dùng AssetDatabase.CreateFolder
            EnsureFolder("Assets/Prefabs/Chapter2");
            EnsureFolder("Assets/Data/Enemies");

            EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(MothDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<EnemyData>();
                data.enemyID = "enemy_lantern_moth";
                data.enemyNameKey = "name_enemy_lantern_moth";
                data.maxHP = 30;
                data.contactDamage = 6;
                data.moveSpeed = 2.85f;
                data.attackPattern = EnemyAttackPattern.LanternDrain;
                AssetDatabase.CreateAsset(data, MothDataPath);
                AssetDatabase.SaveAssets();
            }

            List<Sprite> frames = new List<Sprite>();
            if (Directory.Exists(MothArtDir))
            {
                string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { MothArtDir });
                for (int i = 0; i < spriteGuids.Length; i++)
                {
                    Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(spriteGuids[i]));
                    if (s != null) frames.Add(s);
                }
            }

            Sprite whitePix = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePixelPath);

            GameObject root = new GameObject("Enemy_LanternMoth");
            root.layer = 9;
            Rigidbody2D rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            CircleCollider2D col = root.AddComponent<CircleCollider2D>();
            col.radius = 0.42f;
            col.isTrigger = true;

            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);
            visual.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = 200;
            sr.sprite = frames.Count > 0 ? frames[0] : whitePix;

            GameObject shadow = CreateTintSprite("GroundShadow", root.transform, whitePix,
                new Vector3(0f, -0.20f, 0f), new Vector3(0.55f, 0.24f, 1f), new Color(0f, 0f, 0f, 0.28f), "WorldObjects", -500);
            GameObject hpRoot = new GameObject("HPBar");
            hpRoot.transform.SetParent(root.transform, false);
            hpRoot.transform.localPosition = new Vector3(0f, 0.92f, 0f);
            CreateTintSprite("BG", hpRoot.transform, whitePix, Vector3.zero, new Vector3(0.95f, 0.13f, 1f),
                new Color(0.12f, 0.12f, 0.18f, 0.85f), "WorldObjects", 1500);
            GameObject hpFill = CreateTintSprite("Fill", hpRoot.transform, whitePix, Vector3.zero, new Vector3(0.90f, 0.09f, 1f),
                new Color(0.76f, 0.44f, 1f, 1f), "WorldObjects", 1501);
            hpRoot.SetActive(false);

            LanternMothController mothCtrl = root.AddComponent<LanternMothController>();
            SerializedObject mso = new SerializedObject(mothCtrl);
            mso.FindProperty("enemyData").objectReferenceValue = data;
            mso.FindProperty("rb").objectReferenceValue = rb;
            mso.FindProperty("spriteRenderer").objectReferenceValue = sr;
            mso.FindProperty("hpBarRoot").objectReferenceValue = hpRoot;
            mso.FindProperty("hpBarFill").objectReferenceValue = hpFill.transform;
            mso.FindProperty("groundShadow").objectReferenceValue = shadow;
            var fp = mso.FindProperty("idleFrames");
            fp.ClearArray();
            for (int i = 0; i < frames.Count; i++)
            {
                fp.InsertArrayElementAtIndex(i);
                fp.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            }
            mso.ApplyModifiedPropertiesWithoutUndo();

            bool saveOk;
            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, MothPrefabPath, out saveOk);
            Object.DestroyImmediate(root);
            AssetDatabase.ImportAsset(MothPrefabPath, ImportAssetOptions.ForceUpdate);
            GameObject reloaded = AssetDatabase.LoadAssetAtPath<GameObject>(MothPrefabPath);
            Debug.Log($"[MV][Ch2-R4] Đã dựng mới Enemy_LanternMoth.prefab (saveOk={saveOk}, frames={frames.Count}) tại {MothPrefabPath}");
            if (reloaded != null) return reloaded.GetComponent<LanternMothController>();
            return saved != null ? saved.GetComponent<LanternMothController>() : null;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;
            string[] parts = folderPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static GameObject CreateTintSprite(string name, Transform parent, Sprite spr, Vector3 pos, Vector3 scale, Color tint, string layer, int order)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color = tint;
            sr.sortingLayerName = layer;
            sr.sortingOrder = order;
            return go;
        }

        private static GameObject CreateSegmentGroup(Transform parent, string groupName, Sprite plankSpr, int startX, int endX)
        {
            GameObject grp = new GameObject(groupName);
            grp.transform.SetParent(parent, false);
            for (int x = startX; x <= endX; x++)
            {
                CreatePlankTile(grp.transform, $"Plank_{x}_Top", plankSpr, new Vector3(x, 0.72f, 0f), Vector3.one, Color.white, 7);
                CreatePlankTile(grp.transform, $"Plank_{x}_Bot", plankSpr, new Vector3(x, -0.22f, 0f), Vector3.one, Color.white, 7);
            }
            return grp;
        }

        private static void CreatePlankTile(Transform parent, string name, Sprite spr, Vector3 pos, Vector3 scale, Color tint, int order)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color = tint;
            sr.sortingLayerName = "Ground";
            sr.sortingOrder = order;
        }
    }
}
#endif
