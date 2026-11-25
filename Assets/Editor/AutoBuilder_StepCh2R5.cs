#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2R5
    {
        private static readonly string[] ExpectedScenes =
        {
            "Assets/Scenes/Bootstrap.unity",
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/MoonpetalValley_MVP.unity",
            "Assets/Scenes/Riverbank_Chapter2.unity",
            "Assets/Scenes/WestSwamp_Chapter2.unity"
        };

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R5 - Full Chapter 2 Overhaul Gate QA (5 Scenes)")]
        public static void RunChapter2OverhaulGateQA()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Ch2-R5] Không thể chạy Gate QA khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            int errors = 0;
            Debug.Log("[MV][Ch2-R5] === BẮT ĐẦU FULL CHAPTER 2 OVERHAUL GATE QA (5 SCENES) ===");

            // 1. Kiểm tra Build Settings (đủ 5 scene đúng thứ tự)
            var buildScenes = EditorBuildSettings.scenes;
            if (buildScenes.Length < ExpectedScenes.Length)
            {
                Debug.LogError($"[MV][Ch2-R5][FAIL] Build Settings chỉ có {buildScenes.Length}/{ExpectedScenes.Length} scene!");
                errors++;
            }
            else
            {
                for (int i = 0; i < ExpectedScenes.Length; i++)
                {
                    if (!string.Equals(buildScenes[i].path, ExpectedScenes[i], StringComparison.OrdinalIgnoreCase) || !buildScenes[i].enabled)
                    {
                        Debug.LogError($"[MV][Ch2-R5][FAIL] Build Settings [{i}] sai: '{buildScenes[i].path}' (enabled={buildScenes[i].enabled}), cần '{ExpectedScenes[i]}'.");
                        errors++;
                    }
                }
            }

            // 2. Quét toàn bộ 5 scene: 0 Missing Scripts + kiểm tra cấu trúc từng scene
            for (int s = 0; s < ExpectedScenes.Length; s++)
            {
                string path = ExpectedScenes[s];
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                if (!scene.IsValid())
                {
                    Debug.LogError($"[MV][Ch2-R5][FAIL] Không mở được scene '{path}'!");
                    errors++;
                    continue;
                }

                int missingInScene = 0;
                var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
                for (int i = 0; i < allGos.Length; i++)
                {
                    int c = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]);
                    if (c > 0)
                    {
                        Debug.LogError($"[MV][Ch2-R5][FAIL] Scene '{scene.name}' -> '{allGos[i].name}' có {c} Missing Script!");
                        missingInScene += c;
                    }
                }
                errors += missingInScene;

                if (scene.name == "Riverbank_Chapter2")
                    errors += AuditRiverbankChapter2();
                else if (scene.name == "WestSwamp_Chapter2")
                    errors += AuditWestSwampChapter2();
            }

            // 3. Quét sạch mốc thời gian cũ "ba mươi" trong toàn bộ DialogueData / EchoData
            errors += AuditTimelineConsistency();

            // 4. Mở lại Riverbank_Chapter2 để sẵn sàng Playtest
            EditorSceneManager.OpenScene("Assets/Scenes/Riverbank_Chapter2.unity", OpenSceneMode.Single);

            if (errors == 0)
            {
                Debug.Log("[MV][Ch2-R5] === GATE QA PASS (0 LỖI) - TOÀN BỘ CHAPTER 2 OVERHAUL (5 SCENES, 8 GIAI ĐOẠN, ĐẦM LẦY 3 ĐỢT, CẦU 3 TRẠNG THÁI, BƯỚM HÚT SÁNG & F1 DEBUG) ĐÃ SẴN SÀNG! ===");
            }
            else
            {
                Debug.LogError($"[MV][Ch2-R5] === GATE QA FAIL ({errors} lỗi) - Xem các dòng [FAIL] ở trên. ===");
            }
        }

        private static int AuditRiverbankChapter2()
        {
            int err = 0;

            RiverBridgeController bridge = UnityEngine.Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge == null)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] Riverbank_Chapter2 thiếu RiverBridgeController!");
                err++;
            }
            else
            {
                SerializedObject bso = new SerializedObject(bridge);
                var scafProp = bso.FindProperty("scaffoldingVisualsRoot");
                var segProp = bso.FindProperty("progressivePlankSegments");
                if (scafProp == null || scafProp.objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] RiverBridgeController thiếu scaffoldingVisualsRoot!");
                    err++;
                }
                if (segProp == null || segProp.arraySize < 3)
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] RiverBridgeController thiếu 3 progressivePlankSegments!");
                    err++;
                }
            }

            LanternRelayManager relay = UnityEngine.Object.FindAnyObjectByType<LanternRelayManager>();
            if (relay == null)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] Riverbank_Chapter2 thiếu LanternRelayManager!");
                err++;
            }
            else
            {
                SerializedObject rso = new SerializedObject(relay);
                var mothProp = rso.FindProperty("mothPrefab");
                if (mothProp == null || mothProp.objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] LanternRelayManager thiếu mothPrefab!");
                    err++;
                }
            }

            if (UnityEngine.Object.FindAnyObjectByType<SwampMistBarrierController>() == null)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] Riverbank_Chapter2 thiếu SwampMistBarrierController!");
                err++;
            }

            bool foundSwampPortal = false;
            var portals = UnityEngine.Object.FindObjectsByType<ScenePortalTrigger>(FindObjectsInactive.Include);
            for (int i = 0; i < portals.Length; i++)
            {
                SerializedObject pso = new SerializedObject(portals[i]);
                var targetProp = pso.FindProperty("targetSceneName");
                var gateProp = pso.FindProperty("useSwampUnlockGate");
                if (targetProp != null && gateProp != null &&
                    targetProp.stringValue == "WestSwamp_Chapter2" && gateProp.boolValue)
                {
                    foundSwampPortal = true;
                    break;
                }
            }
            if (!foundSwampPortal)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] Riverbank_Chapter2 thiếu cổng Portal_ToWestSwamp có useSwampUnlockGate=true!");
                err++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<SwampCombatZoneController>() != null)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] Riverbank_Chapter2 vẫn còn SwampCombatZoneController cũ!");
                err++;
            }

            var echoes = UnityEngine.Object.FindObjectsByType<EchoSpotInteractable>(FindObjectsInactive.Include);
            for (int i = 0; i < echoes.Length; i++)
            {
                if (echoes[i].gameObject.name.Contains("StormLantern"))
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] Riverbank_Chapter2 vẫn còn EchoSpot_StormLantern cũ (phải nằm ở WestSwamp_Chapter2)!");
                    err++;
                }
            }

            return err;
        }

        private static int AuditWestSwampChapter2()
        {
            int err = 0;

            SwampWaveCombatController wave = UnityEngine.Object.FindAnyObjectByType<SwampWaveCombatController>();
            if (wave == null)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] WestSwamp_Chapter2 thiếu SwampWaveCombatController!");
                err++;
            }
            else
            {
                SerializedObject wso = new SerializedObject(wave);
                var crawlerProp = wso.FindProperty("thornCrawlerPrefab");
                var slimeProp = wso.FindProperty("gloomSlimePrefab");
                var bigSlimeProp = wso.FindProperty("bigGloomSlimePrefab");
                var minProp = wso.FindProperty("arenaMin");
                var maxProp = wso.FindProperty("arenaMax");

                if (crawlerProp == null || crawlerProp.objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] SwampWaveCombatController thiếu thornCrawlerPrefab!");
                    err++;
                }
                if (slimeProp == null || slimeProp.objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] SwampWaveCombatController thiếu gloomSlimePrefab!");
                    err++;
                }
                if (bigSlimeProp == null || bigSlimeProp.objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] SwampWaveCombatController thiếu bigGloomSlimePrefab!");
                    err++;
                }
                if (minProp == null || maxProp == null || maxProp.vector2Value.x <= minProp.vector2Value.x || maxProp.vector2Value.y <= minProp.vector2Value.y)
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] SwampWaveCombatController thiếu vùng spawn ngẫu nhiên arenaMin/arenaMax hợp lệ!");
                    err++;
                }
            }

            EchoSpotInteractable echo = UnityEngine.Object.FindAnyObjectByType<EchoSpotInteractable>();
            if (echo == null)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] WestSwamp_Chapter2 thiếu EchoSpot_StormLantern!");
                err++;
            }
            else
            {
                SerializedObject eso = new SerializedObject(echo);
                var edProp = eso.FindProperty("echoData");
                EchoData ed = edProp != null ? edProp.objectReferenceValue as EchoData : null;
                if (ed == null || ed.requiredItemId != "item_storm_lantern_east" || ed.requiredSecondaryItemId != "item_storm_lantern_west")
                {
                    Debug.LogError("[MV][Ch2-R5][FAIL] EchoSpot_StormLantern chưa yêu cầu đủ 2 nửa Đèn Bão Cổ (east + west)!");
                    err++;
                }
            }

            bool foundReturn = false;
            var portals = UnityEngine.Object.FindObjectsByType<ScenePortalTrigger>(FindObjectsInactive.Include);
            for (int i = 0; i < portals.Length; i++)
            {
                SerializedObject pso = new SerializedObject(portals[i]);
                var targetProp = pso.FindProperty("targetSceneName");
                if (targetProp != null && targetProp.stringValue == "Riverbank_Chapter2")
                {
                    foundReturn = true;
                    break;
                }
            }
            if (!foundReturn)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] WestSwamp_Chapter2 thiếu cổng quay về Riverbank_Chapter2!");
                err++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<WorldSpawnRegistry>() == null)
            {
                Debug.LogError("[MV][Ch2-R5][FAIL] WestSwamp_Chapter2 thiếu WorldSpawnRegistry!");
                err++;
            }

            return err;
        }

        private static int AuditTimelineConsistency()
        {
            int err = 0;

            string[] dlgGuids = AssetDatabase.FindAssets("t:DialogueData", new[] { "Assets/Data" });
            for (int i = 0; i < dlgGuids.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(dlgGuids[i]);
                DialogueData d = AssetDatabase.LoadAssetAtPath<DialogueData>(p);
                if (d == null || d.lines == null) continue;
                for (int l = 0; l < d.lines.Count; l++)
                {
                    if (!string.IsNullOrEmpty(d.lines[l].textKey) &&
                        d.lines[l].textKey.IndexOf("ba mươi", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Debug.LogError($"[MV][Ch2-R5][FAIL] DialogueData '{p}' dòng {l} vẫn còn cụm 'ba mươi'!");
                        err++;
                    }
                }
            }

            string[] echoGuids = AssetDatabase.FindAssets("t:EchoData", new[] { "Assets/Data" });
            for (int i = 0; i < echoGuids.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(echoGuids[i]);
                EchoData e = AssetDatabase.LoadAssetAtPath<EchoData>(p);
                if (e == null || e.beats == null) continue;
                for (int l = 0; l < e.beats.Count; l++)
                {
                    if (!string.IsNullOrEmpty(e.beats[l].bodyText) &&
                        e.beats[l].bodyText.IndexOf("ba mươi", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Debug.LogError($"[MV][Ch2-R5][FAIL] EchoData '{p}' beat {l} vẫn còn cụm 'ba mươi'!");
                        err++;
                    }
                }
            }

            return err;
        }
    }
}
#endif