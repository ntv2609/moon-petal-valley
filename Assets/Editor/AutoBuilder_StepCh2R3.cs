#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;
using MoonpetalValley.Data;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2R3
    {
        private const string WestSwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";
        private const string ThornCrawlerPrefabPath = "Assets/Prefabs/Enemies/Enemy_ThornCrawler.prefab";
        private const string GloomSlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";
        private const string BigSlimePrefabPath = "Assets/Prefabs/Enemies/Enemy_BigGloomSlime.prefab";
        private const string PulseRingPath = "Assets/Art/Sprites/UI/spr_pulse_ring.png";
        private const string VNFontPath = "Assets/Data/Fonts/VN_DynamicFont.asset";

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R3 - 1. Build 3-Wave Swamp Combat & Recovery Orbs")]
        public static void BuildStepCh2R3()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Ch2-R3] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Ch2-R3] === BẮT ĐẦU STEP CH2-R3: LẮP ĐẤU TRƯỜNG 3 ĐỢT & CẦU HỒI PHỤC VÀO WESTSWAMP_CHAPTER2 ===");

            var scene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R3] Không mở được " + WestSwampScenePath + ". Hãy chạy Step Ch2-R2 trước!");
                return;
            }

            GameObject oldArena = GameObject.Find("SwampWaveArena");
            if (oldArena != null) Object.DestroyImmediate(oldArena);

            GameObject arenaGo = new GameObject("SwampWaveArena");
            SwampWaveCombatController ctrl = arenaGo.AddComponent<SwampWaveCombatController>();

            GameObject thornPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ThornCrawlerPrefabPath);
            GameObject slimePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GloomSlimePrefabPath);
            GameObject bigSlimePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BigSlimePrefabPath);
            ItemData westLantern = FindItemById("item_storm_lantern_west");
            Sprite orbSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(VNFontPath);
            if (vnFont == null) vnFont = FindAnyTMPFont();

            List<Transform> nests = new List<Transform>();
            string[] nestNames = { "SwampNest_North", "SwampNest_West", "SwampNest_East" };
            for (int i = 0; i < nestNames.Length; i++)
            {
                GameObject n = GameObject.Find(nestNames[i]);
                if (n != null) nests.Add(n.transform);
            }

            SerializedObject so = new SerializedObject(ctrl);
            so.FindProperty("zoneClearedFlag").stringValue = Chapter2StateManager.FlagSwampCleared;
            so.FindProperty("westLanternRewardItem").objectReferenceValue = westLantern;
            so.FindProperty("thornCrawlerPrefab").objectReferenceValue = thornPrefab;
            so.FindProperty("gloomSlimePrefab").objectReferenceValue = slimePrefab;
            so.FindProperty("bigGloomSlimePrefab").objectReferenceValue = bigSlimePrefab;
            so.FindProperty("orbSprite").objectReferenceValue = orbSpr;
            so.FindProperty("hudFont").objectReferenceValue = vnFont;

            var listProp = so.FindProperty("spawnNests");
            listProp.ClearArray();
            for (int i = 0; i < nests.Count; i++)
            {
                listProp.InsertArrayElementAtIndex(i);
                listProp.GetArrayElementAtIndex(i).objectReferenceValue = nests[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ctrl);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Ch2-R3] === HOÀN TẤT STEP CH2-R3! Chạy 'Step Ch2-R3 - 2. Validate Step Ch2-R3' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R3 - 2. Validate Step Ch2-R3")]
        public static void ValidateStepCh2R3()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-R3] === BẮT ĐẦU VALIDATE STEP CH2-R3 ===");

            var scene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R3][FAIL] Không mở được " + WestSwampScenePath);
                return;
            }

            SwampWaveCombatController ctrl = Object.FindAnyObjectByType<SwampWaveCombatController>();
            if (ctrl == null)
            {
                Debug.LogError("[MV][Ch2-R3][FAIL] WestSwamp_Chapter2 thiếu SwampWaveCombatController!");
                errors++;
            }
            else
            {
                SerializedObject so = new SerializedObject(ctrl);
                if (so.FindProperty("thornCrawlerPrefab").objectReferenceValue == null) { Debug.LogError("[MV][Ch2-R3][FAIL] Thiếu thornCrawlerPrefab."); errors++; }
                if (so.FindProperty("gloomSlimePrefab").objectReferenceValue == null) { Debug.LogError("[MV][Ch2-R3][FAIL] Thiếu gloomSlimePrefab."); errors++; }
                if (so.FindProperty("bigGloomSlimePrefab").objectReferenceValue == null) { Debug.LogError("[MV][Ch2-R3][FAIL] Thiếu bigGloomSlimePrefab."); errors++; }
                if (so.FindProperty("westLanternRewardItem").objectReferenceValue == null) { Debug.LogError("[MV][Ch2-R3][FAIL] Thiếu westLanternRewardItem."); errors++; }
                if (so.FindProperty("spawnNests").arraySize < 3) { Debug.LogError("[MV][Ch2-R3][FAIL] Cần đủ 3 spawnNests."); errors++; }
            }

            var all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < all.Length; i++)
            {
                int c = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(all[i]);
                if (c > 0)
                {
                    Debug.LogError($"[MV][Ch2-R3][FAIL] '{all[i].name}' có {c} Missing Script!");
                    errors += c;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-R3] VALIDATE PASS (0 lỗi) - Đấu trường 3 đợt (18 quái + BigGloomSlime) & Cầu Hồi Máu/Thể Lực sẵn sàng!");
            else
                Debug.LogError($"[MV][Ch2-R3] VALIDATE FAIL ({errors} lỗi).");
        }

        private static ItemData FindItemById(string itemId)
        {
            string[] guids = AssetDatabase.FindAssets("t:ItemData");
            for (int i = 0; i < guids.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(guids[i]);
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(p);
                if (item != null && item.itemID == itemId) return item;
            }
            return null;
        }

        private static TMP_FontAsset FindAnyTMPFont()
        {
            string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            for (int i = 0; i < guids.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (p.Contains("VN_")) return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(p);
            }
            return guids.Length > 0 ? AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guids[0])) : null;
        }
    }
}
#endif
