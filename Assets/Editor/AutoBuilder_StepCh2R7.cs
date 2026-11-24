#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Data;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2R7
    {
        private const string WestSwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";
        private const string CrawlerPrefabPath = "Assets/Prefabs/Enemies/Enemy_ThornCrawler.prefab";
        private const string SlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";
        private const string BigSlimePrefabPath = "Assets/Prefabs/Enemies/Enemy_BigGloomSlime.prefab";
        private const string WestLanternItemPath = "Assets/Data/Items/Item_StormLantern_West.asset";
        private const string OrbSpritePath = "Assets/Art/Sprites/UI/spr_pulse_ring.png";

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R7 - 1. Wire 70-Enemy Full-Map Swamp Combat")]
        public static void BuildStepCh2R7()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Ch2-R7] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Ch2-R7] === BẮT ĐẦU STEP CH2-R7: CHIẾN ĐẤU ĐẦM LẦY 70 QUÁI SPAWN NGẪU NHIÊN KHẮP MAP ===");

            var scene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R7] Không mở được " + WestSwampScenePath);
                return;
            }

            // Xóa sạch 3 vòng spawn cố định cũ
            string[] legacyNests = { "SwampNest_North", "SwampNest_West", "SwampNest_East" };
            for (int i = 0; i < legacyNests.Length; i++)
            {
                GameObject nest = GameObject.Find(legacyNests[i]);
                if (nest != null) Object.DestroyImmediate(nest);
            }

            SwampWaveCombatController wave = Object.FindAnyObjectByType<SwampWaveCombatController>();
            if (wave == null)
            {
                GameObject arenaGo = new GameObject("SwampWaveArena");
                wave = arenaGo.AddComponent<SwampWaveCombatController>();
            }

            GameObject crawler = LoadFirstPrefab(CrawlerPrefabPath, "Enemy_ThornCrawler");
            GameObject slime = LoadFirstPrefab(SlimePrefabPath, "GloomSlime");
            GameObject bigSlime = LoadFirstPrefab(BigSlimePrefabPath, "Enemy_BigGloomSlime");
            ItemData westItem = AssetDatabase.LoadAssetAtPath<ItemData>(WestLanternItemPath);
            Sprite orbSpr = AssetDatabase.LoadAssetAtPath<Sprite>(OrbSpritePath);
            TMP_FontAsset font = FindVietnameseFont();

            SerializedObject so = new SerializedObject(wave);
            so.FindProperty("zoneClearedFlag").stringValue = "flag_ch2_swamp_cleared";
            so.FindProperty("westLanternRewardItem").objectReferenceValue = westItem;
            so.FindProperty("thornCrawlerPrefab").objectReferenceValue = crawler;
            so.FindProperty("gloomSlimePrefab").objectReferenceValue = slime;
            so.FindProperty("bigGloomSlimePrefab").objectReferenceValue = bigSlime;
            so.FindProperty("arenaMin").vector2Value = new Vector2(-16.5f, -7.2f);
            so.FindProperty("arenaMax").vector2Value = new Vector2(15.5f, 8.2f);
            so.FindProperty("triggerEnterMinY").floatValue = -6.2f;
            so.FindProperty("minDistFromPlayer").floatValue = 4.6f;
            so.FindProperty("maxDistFromPlayer").floatValue = 11.2f;
            so.FindProperty("orbSprite").objectReferenceValue = orbSpr;
            so.FindProperty("hudFont").objectReferenceValue = font;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(wave);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Ch2-R7] === HOÀN TẤT STEP CH2-R7! Chạy 'Step Ch2-R7 - 2. Validate Step Ch2-R7' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R7 - 2. Validate Step Ch2-R7")]
        public static void ValidateStepCh2R7()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-R7] === BẮT ĐẦU VALIDATE STEP CH2-R7 ===");

            var scene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R7][FAIL] Không mở được " + WestSwampScenePath);
                return;
            }

            string[] legacyNests = { "SwampNest_North", "SwampNest_West", "SwampNest_East" };
            for (int i = 0; i < legacyNests.Length; i++)
            {
                if (GameObject.Find(legacyNests[i]) != null)
                {
                    Debug.LogError($"[MV][Ch2-R7][FAIL] Vẫn còn vòng spawn cố định '{legacyNests[i]}'!");
                    errors++;
                }
            }

            SwampWaveCombatController wave = Object.FindAnyObjectByType<SwampWaveCombatController>();
            if (wave == null)
            {
                Debug.LogError("[MV][Ch2-R7][FAIL] Thiếu SwampWaveCombatController!");
                errors++;
            }
            else
            {
                SerializedObject so = new SerializedObject(wave);
                if (so.FindProperty("thornCrawlerPrefab").objectReferenceValue == null) { Debug.LogError("[MV][Ch2-R7][FAIL] Thiếu thornCrawlerPrefab!"); errors++; }
                if (so.FindProperty("gloomSlimePrefab").objectReferenceValue == null) { Debug.LogError("[MV][Ch2-R7][FAIL] Thiếu gloomSlimePrefab!"); errors++; }
                if (so.FindProperty("bigGloomSlimePrefab").objectReferenceValue == null) { Debug.LogError("[MV][Ch2-R7][FAIL] Thiếu bigGloomSlimePrefab!"); errors++; }
            }

            var all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < all.Length; i++)
            {
                int c = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(all[i]);
                if (c > 0)
                {
                    Debug.LogError($"[MV][Ch2-R7][FAIL] '{all[i].name}' có {c} Missing Script!");
                    errors += c;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-R7] VALIDATE PASS (0 lỗi) - Chiến đấu Đầm Lầy 70 quái spawn ngẫu nhiên khắp bản đồ & Ngọc Sóng Ánh Sáng đã sẵn sàng!");
            else
                Debug.LogError($"[MV][Ch2-R7] VALIDATE FAIL ({errors} lỗi).");
        }

        private static GameObject LoadFirstPrefab(string primaryPath, string nameFilter)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(primaryPath);
            if (go != null) return go;
            string[] guids = AssetDatabase.FindAssets(nameFilter + " t:Prefab");
            for (int i = 0; i < guids.Length; i++)
            {
                GameObject cand = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[i]));
                if (cand != null) return cand;
            }
            return null;
        }

        private static TMP_FontAsset FindVietnameseFont()
        {
            string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            for (int i = 0; i < guids.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (p.IndexOf("BeVietnam", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    p.IndexOf("Noto", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    TMP_FontAsset f = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(p);
                    if (f != null) return f;
                }
            }
            return TMP_Settings.defaultFontAsset;
        }
    }
}
#endif
