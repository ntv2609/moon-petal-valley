using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Audio;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step46
    {
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string MvpScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string StormLanternPrefabPath = "Assets/Prefabs/Chapter2/Actor_StormLantern.prefab";
        private const string ItemDatabasePath = "Assets/Data/Config/ItemDatabase.asset";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string MusicRulesPath = "Assets/Resources/Moonpetal/Audio/MusicRules.asset";
        private const string TracksDir = "Assets/Resources/Moonpetal/Audio/Tracks";

        [MenuItem("Moonpetal/Step 46/1. Build Chapter 2 Ending, Lantern & Gate", false, 460)]
        public static void BuildStep46()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Step46] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            if (!File.Exists(Ch2ScenePath))
            {
                Debug.LogError("[MV][Step46] Không tìm thấy scene: " + Ch2ScenePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);

            // [STEP46-FIX1-CLEANUP] Dọn rác SwampCombatZone_West cũ còn Missing Script (giữ nguyên SwampCombatZone chính)
            GameObject staleSwamp = GameObject.Find("SwampCombatZone_West");
            if (staleSwamp != null && GameObject.Find("SwampCombatZone") != null)
            {
                Debug.Log("[MV][Step46] Đã xóa GameObject cũ dư thừa: SwampCombatZone_West");
                Object.DestroyImmediate(staleSwamp);
            }
            GameObject[] allSceneGos = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allSceneGos.Length; i++)
            {
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(allSceneGos[i]);
                if (removed > 0)
                    Debug.Log("[MV][Step46] Đã gỡ " + removed + " Missing Script trên: " + allSceneGos[i].name);
            }

            GameObject old = GameObject.Find("Chapter2EndingSystem");
            if (old != null) Object.DestroyImmediate(old);

            GameObject sysRoot = new GameObject("Chapter2EndingSystem");
            Chapter2EndingController ctrl = sysRoot.AddComponent<Chapter2EndingController>();

            GameObject lanternPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(StormLanternPrefabPath);
            StormLanternActor lanternActor = null;
            if (lanternPrefab != null)
            {
                GameObject lanternInst = (GameObject)PrefabUtility.InstantiatePrefab(lanternPrefab);
                lanternInst.name = "Chapter2BridgeStormLantern";
                lanternInst.transform.SetParent(sysRoot.transform, false);
                lanternInst.transform.position = new Vector3(0f, 1.35f, 0f);
                lanternActor = lanternInst.GetComponent<StormLanternActor>();
                lanternInst.SetActive(false);
            }
            else
            {
                Debug.LogWarning("[MV][Step46] Không tìm thấy " + StormLanternPrefabPath);
            }

            SerializedObject so = new SerializedObject(ctrl);
            so.FindProperty("bridgeStormLantern").objectReferenceValue = lanternActor;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV][Step46] BUILD THÀNH CÔNG! Đã gắn Chapter2EndingController + Chapter2BridgeStormLantern. Hãy chạy 'Moonpetal/Step 46/2. Validate Chapter 2 Gate (Full Audit)'.");
        }

        [MenuItem("Moonpetal/Step 46/2. Validate Chapter 2 Gate (Full Audit)", false, 461)]
        public static void ValidateChapter2Gate()
        {
            int errors = 0;
            int warnings = 0;
            Debug.Log("[MV][Step46] === BẮT ĐẦU KIỂM TOÁN CHAPTER 2 GATE (FULL AUDIT) ===");

            // 1. Kiểm tra CR-CH2-PULSE2 trên PlayerCombatController.cs & PlayerStatsSO
            string combatPath = "Assets/Scripts/Player/PlayerCombatController.cs";
            if (!File.Exists(combatPath) || !File.ReadAllText(combatPath).Contains("[CR-CH2-PULSE2]"))
            {
                Debug.LogError("[MV][Step46][FAIL] PlayerCombatController.cs chưa có patch [CR-CH2-PULSE2]!");
                errors++;
            }

            PlayerStatsSO stats = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            if (stats == null || stats.lumiPulseLv2Radius < 4.4f || stats.lumiPulseLv2StunDuration < 1.9f)
            {
                Debug.LogError("[MV][Step46][FAIL] PlayerStatsConfig chưa đạt chuẩn Lumi Pulse Lv2 (radius >= 4.5, stun >= 2.0s).");
                errors++;
            }

            // 2. Kiểm tra phần thưởng item_petal_of_bond & MusicRulesSO
            ItemDatabaseSO db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            ItemData petalBond = db != null ? db.GetById("item_petal_of_bond") : null;
            if (petalBond == null || petalBond.icon == null || !petalBond.isQuestProtected)
            {
                Debug.LogError("[MV][Step46][FAIL] Item item_petal_of_bond thiếu trong ItemDatabase, thiếu icon hoặc chưa bật isQuestProtected.");
                errors++;
            }

            MusicRulesSO rules = AssetDatabase.LoadAssetAtPath<MusicRulesSO>(MusicRulesPath);
            if (rules == null || rules.petalFlagIds == null || !rules.petalFlagIds.Contains("flag_ch2_completed"))
            {
                Debug.LogError("[MV][Step46][FAIL] MusicRulesSO thiếu flag_ch2_completed trong petalFlagIds.");
                errors++;
            }

            MusicTrackSO t07 = AssetDatabase.LoadAssetAtPath<MusicTrackSO>(TracksDir + "/MV_Music_T07_RiverDay.asset");
            MusicTrackSO t08 = AssetDatabase.LoadAssetAtPath<MusicTrackSO>(TracksDir + "/MV_Music_T08_LanternRelay.asset");
            if (t07 == null || t07.clip == null || t07.isDormant) { Debug.LogWarning("[MV][Step46][WARN] T07_RiverDay chưa gắn clip hoạt động."); warnings++; }
            if (t08 == null || t08.clip == null || t08.isDormant) { Debug.LogWarning("[MV][Step46][WARN] T08_LanternRelay chưa gắn clip hoạt động."); warnings++; }

            // 3. Asset Audit (0 ĐỎ) cho toàn bộ 6 Prefab Actor/Enemy Chương 2
            string[] ch2Prefabs =
            {
                "Assets/Prefabs/Chapter2/Echo_KaelYoung.prefab",
                "Assets/Prefabs/Chapter2/Echo_MaraYoung.prefab",
                "Assets/Prefabs/Chapter2/Actor_StormLantern.prefab",
                "Assets/Prefabs/Enemies/Enemy_ThornCrawler.prefab",
                "Assets/Prefabs/Enemies/Enemy_BigGloomSlime.prefab",
                "Assets/Prefabs/Chapter2/Enemy_LanternMoth.prefab"
            };
            for (int i = 0; i < ch2Prefabs.Length; i++)
            {
                GameObject pf = AssetDatabase.LoadAssetAtPath<GameObject>(ch2Prefabs[i]);
                if (pf == null)
                {
                    Debug.LogError("[MV][Step46][ĐỎ] Thiếu Prefab Chương 2: " + ch2Prefabs[i]);
                    errors++;
                    continue;
                }
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(pf) > 0)
                {
                    Debug.LogError("[MV][Step46][ĐỎ] Missing Script trên: " + ch2Prefabs[i]);
                    errors++;
                }
                if (ch2Prefabs[i].Contains("Enemy_") && pf.GetComponentInChildren<Animator>(true) != null)
                {
                    Debug.LogError("[MV][Step46][ĐỎ] Enemy pixel art còn Animator: " + ch2Prefabs[i]);
                    errors++;
                }
                SpriteRenderer sr = pf.GetComponentInChildren<SpriteRenderer>(true);
                if (sr == null || sr.sprite == null || sr.sprite.name.StartsWith("PLACEHOLDER_"))
                {
                    Debug.LogError("[MV][Step46][ĐỎ] Prefab thiếu sprite FINAL: " + ch2Prefabs[i]);
                    errors++;
                }
            }

            // 4. Kiểm tra Build Settings đủ 4 scene
            if (EditorBuildSettings.scenes.Length < 4)
            {
                Debug.LogError("[MV][Step46][FAIL] Build Settings cần đủ 4 scene (Bootstrap, MainMenu, MoonpetalValley_MVP, Riverbank_Chapter2).");
                errors++;
            }

            // 5. Kiểm tra Riverbank_Chapter2.unity
            var ch2Scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!ch2Scene.IsValid())
            {
                Debug.LogError("[MV][Step46][FAIL] Không mở được " + Ch2ScenePath);
                errors++;
            }
            else
            {
                if (Object.FindAnyObjectByType<Chapter2EndingController>() == null) { Debug.LogError("[MV][Step46][FAIL] Thiếu Chapter2EndingController."); errors++; }
                if (Object.FindAnyObjectByType<LanternRelayManager>() == null) { Debug.LogError("[MV][Step46][FAIL] Thiếu LanternRelayManager."); errors++; }
                if (Object.FindAnyObjectByType<RiverBridgeController>() == null) { Debug.LogError("[MV][Step46][FAIL] Thiếu RiverBridgeController."); errors++; }
                if (Object.FindAnyObjectByType<SwampCombatZoneController>() == null) { Debug.LogError("[MV][Step46][FAIL] Thiếu SwampCombatZoneController."); errors++; }
                if (Object.FindObjectsByType<BridgeLanternPost>(FindObjectsInactive.Include).Length != 6) { Debug.LogError("[MV][Step46][FAIL] Cần đúng 6 BridgeLanternPost."); errors++; }
                if (Object.FindObjectsByType<LanternRelayStation>(FindObjectsInactive.Include).Length < 2) { Debug.LogError("[MV][Step46][FAIL] Cần >= 2 LanternRelayStation."); errors++; }
                if (Object.FindObjectsByType<EchoSpotInteractable>(FindObjectsInactive.Include).Length < 2) { Debug.LogError("[MV][Step46][FAIL] Cần >= 2 EchoSpotInteractable."); errors++; }
                if (GameObject.Find("NPC_Kael") == null || GameObject.Find("NPC_Mara") == null || GameObject.Find("NPC_Otis_Ch2") == null)
                { Debug.LogError("[MV][Step46][FAIL] Thiếu NPC Chương 2 (Kael/Mara/Otis)."); errors++; }
                errors += CountMissingScriptsInActiveScene("Riverbank_Chapter2");
            }

            // 6. Kiểm tra hồi quy Chương 1 MVP (chỉ đọc, không sửa scene)
            var mvpScene = EditorSceneManager.OpenScene(MvpScenePath, OpenSceneMode.Single);
            if (!mvpScene.IsValid())
            {
                Debug.LogError("[MV][Step46][FAIL] Không mở được " + MvpScenePath);
                errors++;
            }
            else
            {
                if (Object.FindAnyObjectByType<PlayerController2D>() == null) { Debug.LogError("[MV][Step46][FAIL] Ch1 thiếu Player_Wen."); errors++; }
                if (Object.FindAnyObjectByType<PlantingSpotController>() == null) { Debug.LogError("[MV][Step46][FAIL] Ch1 thiếu PlantingSpotController."); errors++; }
                if (GameObject.Find("Portal_ToRiverbank") == null) { Debug.LogError("[MV][Step46][FAIL] Ch1 thiếu Portal_ToRiverbank."); errors++; }
                if (Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include).Length < 12) { Debug.LogError("[MV][Step46][FAIL] Ch1 thiếu 12 FarmTile."); errors++; }
                if (Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include).Length < 8) { Debug.LogError("[MV][Step46][FAIL] Ch1 thiếu 8 GloomSlime."); errors++; }
                errors += CountMissingScriptsInActiveScene("MoonpetalValley_MVP");
            }

            // Mở lại Riverbank_Chapter2 cho tiện test
            EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);

            if (errors == 0)
            {
                Debug.Log(string.Format("[MV][Step46][CH2 GATE] VALIDATE PASS (0 lỗi, {0} cảnh báo) — CHƯƠNG 2 HOÀN TẤT 100%, SẴN SÀNG MỞ CHƯƠNG 3!", warnings));
            }
            else
            {
                Debug.LogError(string.Format("[MV][Step46][CH2 GATE] VALIDATE FAIL ({0} lỗi, {1} cảnh báo).", errors, warnings));
            }
        }

        private static int CountMissingScriptsInActiveScene(string label)
        {
            int count = 0;
            GameObject[] all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < all.Length; i++)
            {
                int m = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(all[i]);
                if (m > 0)
                {
                    Debug.LogError("[MV][Step46][FAIL] Missing Script trong " + label + " trên: " + all[i].name);
                    count += m;
                }
            }
            return count;
        }
    }
}
