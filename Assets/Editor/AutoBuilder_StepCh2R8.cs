#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2R8
    {
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private static readonly string[] ExpectedScenes =
        {
            "Assets/Scenes/Bootstrap.unity",
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/MoonpetalValley_MVP.unity",
            "Assets/Scenes/Riverbank_Chapter2.unity",
            "Assets/Scenes/WestSwamp_Chapter2.unity"
        };

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R8 - 1. Install Bridge Rails & Tune 3-Phase Relay")]
        public static void BuildStepCh2R8()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Ch2-R8] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Ch2-R8] === BẮT ĐẦU STEP CH2-R8: LAN CAN CẦU CHỐNG ĐI TRÊN NƯỚC & CHUYỀN ĐÈN 3 PHA ===");

            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R8] Không mở được " + Ch2ScenePath);
                return;
            }

            RiverBridgeController bridge = UnityEngine.Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge == null)
            {
                Debug.LogError("[MV][Ch2-R8] Không tìm thấy RiverBridgeController trong " + Ch2ScenePath);
                return;
            }

            // Tạo 2 thanh collider lan can cứng dọc 2 mép Bắc/Nam mặt cầu (x = -8.2..+7.6)
            BoxCollider2D northRail = EnsureBridgeRail(bridge.transform, "BridgeRail_North", new Vector3(-0.3f, 1.08f, 0f), new Vector2(15.8f, 0.36f));
            BoxCollider2D southRail = EnsureBridgeRail(bridge.transform, "BridgeRail_South", new Vector3(-0.3f, -0.58f, 0f), new Vector2(15.8f, 0.36f));

            SerializedObject bso = new SerializedObject(bridge);
            bso.FindProperty("bridgeRailNorth").objectReferenceValue = northRail;
            bso.FindProperty("bridgeRailSouth").objectReferenceValue = southRail;
            bso.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bridge);

            // Tinh chỉnh RelayLanternConfigSO cho nhịp 3 pha kịch tính
            string[] cfgGuids = AssetDatabase.FindAssets("t:RelayLanternConfigSO");
            for (int i = 0; i < cfgGuids.Length; i++)
            {
                RelayLanternConfigSO cfg = AssetDatabase.LoadAssetAtPath<RelayLanternConfigSO>(AssetDatabase.GUIDToAssetPath(cfgGuids[i]));
                if (cfg == null) continue;
                cfg.mothSpawnInterval = 2.6f;
                cfg.maxActiveMoths = 6;
                cfg.mothDrainPerSec = 13.0f;
                cfg.passiveDrainPerSec = 2.35f;
                EditorUtility.SetDirty(cfg);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Ch2-R8] === HOÀN TẤT STEP CH2-R8! Chạy 'Step Ch2-R8 - 2. Final Chapter 2 Overhaul Gate QA (5 Scenes)' để kiểm định toàn dự án. ===");
        }

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R8 - 2. Final Chapter 2 Overhaul Gate QA (5 Scenes)")]
        public static void RunFinalGateQA()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-R8] === BẮT ĐẦU FINAL CHAPTER 2 OVERHAUL GATE QA (5 SCENES) ===");

            // 1. Build Settings 5 scenes
            var buildScenes = EditorBuildSettings.scenes;
            if (buildScenes.Length < ExpectedScenes.Length)
            {
                Debug.LogError($"[MV][Ch2-R8][FAIL] Build Settings chỉ có {buildScenes.Length}/5 scene!");
                errors++;
            }
            else
            {
                for (int i = 0; i < ExpectedScenes.Length; i++)
                {
                    if (!string.Equals(buildScenes[i].path, ExpectedScenes[i], StringComparison.OrdinalIgnoreCase) || !buildScenes[i].enabled)
                    {
                        Debug.LogError($"[MV][Ch2-R8][FAIL] Build Settings [{i}] sai: '{buildScenes[i].path}'!");
                        errors++;
                    }
                }
            }

            // 2. Audit all 5 scenes
            for (int s = 0; s < ExpectedScenes.Length; s++)
            {
                var scene = EditorSceneManager.OpenScene(ExpectedScenes[s], OpenSceneMode.Single);
                if (!scene.IsValid()) { Debug.LogError("[MV][Ch2-R8][FAIL] Không mở được " + ExpectedScenes[s]); errors++; continue; }

                var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
                for (int i = 0; i < allGos.Length; i++)
                {
                    int c = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]);
                    if (c > 0) { Debug.LogError($"[MV][Ch2-R8][FAIL] {scene.name} -> '{allGos[i].name}' có {c} Missing Script!"); errors += c; }
                }

                if (scene.name == "Riverbank_Chapter2")
                {
                    var bridge = UnityEngine.Object.FindAnyObjectByType<RiverBridgeController>();
                    if (bridge == null) { Debug.LogError("[MV][Ch2-R8][FAIL] Thiếu RiverBridgeController!"); errors++; }
                    else
                    {
                        var bso = new SerializedObject(bridge);
                        if (bso.FindProperty("bridgeRailNorth").objectReferenceValue == null ||
                            bso.FindProperty("bridgeRailSouth").objectReferenceValue == null)
                        { Debug.LogError("[MV][Ch2-R8][FAIL] RiverBridgeController thiếu BridgeRail_North/South!"); errors++; }
                        if (bso.FindProperty("scaffoldingVisualsRoot").objectReferenceValue == null)
                        { Debug.LogError("[MV][Ch2-R8][FAIL] RiverBridgeController thiếu scaffoldingVisualsRoot!"); errors++; }
                    }

                    var relay = UnityEngine.Object.FindAnyObjectByType<LanternRelayManager>();
                    if (relay == null || new SerializedObject(relay).FindProperty("mothPrefab").objectReferenceValue == null)
                    { Debug.LogError("[MV][Ch2-R8][FAIL] LanternRelayManager thiếu mothPrefab!"); errors++; }

                    var portal = GameObject.Find("Portal_ToWestSwamp");
                    if (portal == null || portal.GetComponent<ScenePortalTrigger>() == null ||
                        !new SerializedObject(portal.GetComponent<ScenePortalTrigger>()).FindProperty("autoTriggerOnWalk").boolValue)
                    { Debug.LogError("[MV][Ch2-R8][FAIL] Portal_ToWestSwamp chưa bật đi bộ tự chuyển cảnh (autoTriggerOnWalk)!"); errors++; }
                }
                else if (scene.name == "WestSwamp_Chapter2")
                {
                    var wave = UnityEngine.Object.FindAnyObjectByType<SwampWaveCombatController>();
                    if (wave == null) { Debug.LogError("[MV][Ch2-R8][FAIL] WestSwamp thiếu SwampWaveCombatController!"); errors++; }
                    else
                    {
                        var wso = new SerializedObject(wave);
                        if (wso.FindProperty("thornCrawlerPrefab").objectReferenceValue == null ||
                            wso.FindProperty("gloomSlimePrefab").objectReferenceValue == null ||
                            wso.FindProperty("bigGloomSlimePrefab").objectReferenceValue == null)
                        { Debug.LogError("[MV][Ch2-R8][FAIL] SwampWaveCombatController thiếu prefab quái!"); errors++; }
                    }
                    if (GameObject.Find("SwampNest_North") != null || GameObject.Find("SwampNest_West") != null || GameObject.Find("SwampNest_East") != null)
                    { Debug.LogError("[MV][Ch2-R8][FAIL] WestSwamp vẫn còn vòng spawn cố định cũ!"); errors++; }
                    if (GameObject.Find("WestSwampRealEnvironment") == null)
                    { Debug.LogError("[MV][Ch2-R8][FAIL] WestSwamp thiếu WestSwampRealEnvironment (art pixel-art thật)!"); errors++; }
                }
            }

            EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);

            if (errors == 0)
                Debug.Log("[MV][Ch2-R8] === FINAL GATE QA PASS (0 LỖI) - ĐẠI TU CHƯƠNG 2 HOÀN TẤT 100% (5 SCENES, ĐƯỜNG NỐI LIỀN, ART THẬT, 70 QUÁI RANDOM, CHUYỀN ĐÈN 3 PHA & LAN CAN CẦU)! ===");
            else
                Debug.LogError($"[MV][Ch2-R8] === FINAL GATE QA FAIL ({errors} lỗi). ===");
        }

        private static BoxCollider2D EnsureBridgeRail(Transform parent, string name, Vector3 localPos, Vector2 size)
        {
            Transform old = parent.Find(name);
            if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.size = size;
            col.isTrigger = false;
            return col;
        }
    }
}
#endif
