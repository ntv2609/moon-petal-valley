using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepC3
    {
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string VfxFramesRoot = "Assets/Art/Sprites/VFX/Combat/Frames";
        private const string PetalSpritePath = "Assets/Art/Sprites/VFX/Combat/vfx_petal_particle.png";

        [MenuItem("Moonpetal/Step C3/1. Wire Combat Juice, Multi-Frame VFX & Combo HUD")]
        public static void BuildStepC3()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][StepC3] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][StepC3] === BẮT ĐẦU AUTOBUILDER STEP C3 (COMBAT JUICE + VFX + COMBO HUD) ===");

            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null)
            {
                Debug.LogError("[MV][StepC3] Không tìm thấy " + PlayerPrefabPath);
                return;
            }

            WenSpriteAnimator anim = root.GetComponent<WenSpriteAnimator>();
            SpriteRenderer legacySlashSr = null;
            Transform slashTf = root.transform.Find("SlashArcVisual");
            if (slashTf != null) legacySlashSr = slashTf.GetComponent<SpriteRenderer>();

            WenComboVfxController vfx = root.GetComponent<WenComboVfxController>();
            if (vfx == null) vfx = root.AddComponent<WenComboVfxController>();

            SerializedObject vSO = new SerializedObject(vfx);
            vSO.FindProperty("spriteAnimator").objectReferenceValue = anim;
            vSO.FindProperty("legacySlashRenderer").objectReferenceValue = legacySlashSr;

            AssignSpriteArray(vSO.FindProperty("combo1Frames"), LoadVfxFrames("vfx_combo1_slash", 4));
            AssignSpriteArray(vSO.FindProperty("combo2Frames"), LoadVfxFrames("vfx_combo2_reverse", 4));
            AssignSpriteArray(vSO.FindProperty("combo3Frames"), LoadVfxFrames("vfx_combo3_finisher", 4));
            AssignSpriteArray(vSO.FindProperty("dashStrikeFrames"), LoadVfxFrames("vfx_dash_strike", 4));
            AssignSpriteArray(vSO.FindProperty("hitSparkFrames"), LoadVfxFrames("vfx_hit_spark", 4));
            vSO.FindProperty("petalSprite").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(PetalSpritePath);
            vSO.ApplyModifiedPropertiesWithoutUndo();

            ComboHudView hud = root.GetComponent<ComboHudView>();
            if (hud == null) hud = root.AddComponent<ComboHudView>();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][StepC3] === HOÀN TẤT STEP C3! Đã gắn WenComboVfxController & ComboHudView lên Player_Wen.prefab. Chạy 'Moonpetal/Step C3/Validate'. ===");
        }

        [MenuItem("Moonpetal/Step C3/Validate")]
        public static void ValidateStepC3()
        {
            int errors = 0;
            Debug.Log("[MV][StepC3] === BẮT ĐẦU VALIDATE STEP C3 ===");

            GameObject wenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (wenPrefab == null)
            {
                Debug.LogError("[MV][StepC3] THIẾU Player_Wen.prefab.");
                errors++;
            }
            else
            {
                WenComboVfxController vfx = wenPrefab.GetComponent<WenComboVfxController>();
                if (vfx == null)
                {
                    Debug.LogError("[MV][StepC3] Player_Wen.prefab thiếu WenComboVfxController.");
                    errors++;
                }
                else
                {
                    SerializedObject vSO = new SerializedObject(vfx);
                    string[] arrays = { "combo1Frames", "combo2Frames", "combo3Frames", "dashStrikeFrames", "hitSparkFrames" };
                    foreach (string name in arrays)
                    {
                        SerializedProperty prop = vSO.FindProperty(name);
                        if (prop == null || prop.arraySize < 4 || prop.GetArrayElementAtIndex(0).objectReferenceValue == null)
                        {
                            Debug.LogError("[MV][StepC3] WenComboVfxController thiếu 4 frame hợp lệ cho: " + name);
                            errors++;
                        }
                    }
                    if (vSO.FindProperty("petalSprite").objectReferenceValue == null)
                    {
                        Debug.LogError("[MV][StepC3] WenComboVfxController thiếu petalSprite.");
                        errors++;
                    }
                }

                if (wenPrefab.GetComponent<ComboHudView>() == null)
                {
                    Debug.LogError("[MV][StepC3] Player_Wen.prefab thiếu ComboHudView.");
                    errors++;
                }

                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(wenPrefab) > 0)
                {
                    Debug.LogError("[MV][StepC3] Player_Wen.prefab có Missing Script.");
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV][StepC3] VALIDATE STEP C3: PASS (0 lỗi). Toàn bộ hệ thống Wen Combat Combo (C1 + C2 + C3) đã hoàn thiện đồng bộ cả Chương 1 & Chương 2!");
            }
            else
            {
                Debug.LogError("[MV][StepC3] VALIDATE STEP C3: FAIL (" + errors + " lỗi).");
            }
        }

        private static Sprite[] LoadVfxFrames(string baseName, int count)
        {
            List<Sprite> list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                string path = VfxFramesRoot + "/" + baseName + "_" + i.ToString("00") + ".png";
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(path));
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
