using UnityEngine;
using UnityEditor;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepC2
    {
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";

        [MenuItem("Moonpetal/Step C2/1. Apply Combo Defaults to PlayerStatsSO")]
        public static void BuildStepC2()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][StepC2] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][StepC2] === BẮT ĐẦU AUTOBUILDER STEP C2 (CẤU HÌNH COMBO CHO PLAYERSTATSSO) ===");

            string[] guids = AssetDatabase.FindAssets("t:PlayerStatsSO");
            if (guids == null || guids.Length == 0)
            {
                Debug.LogError("[MV][StepC2] Không tìm thấy asset PlayerStatsSO nào trong dự án.");
                return;
            }

            int updated = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PlayerStatsSO so = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(path);
                if (so == null) continue;

                SerializedObject sObj = new SerializedObject(so);
                sObj.FindProperty("enableComboSystem").boolValue = true;
                sObj.FindProperty("enableDashStrike").boolValue = true;
                sObj.FindProperty("comboWindowDuration").floatValue = 0.62f;
                sObj.FindProperty("inputBufferDuration").floatValue = 0.18f;
                sObj.FindProperty("dashStrikeGraceWindow").floatValue = 0.12f;

                sObj.FindProperty("combo1Damage").intValue = 15;
                sObj.FindProperty("combo1Radius").floatValue = 1.60f;
                sObj.FindProperty("combo1ArcDegrees").floatValue = 120f;
                sObj.FindProperty("combo1Knockback").floatValue = 4.5f;
                sObj.FindProperty("combo1Cooldown").floatValue = 0.24f;
                sObj.FindProperty("combo1StaminaCost").intValue = 2;
                sObj.FindProperty("combo1LungeSpeed").floatValue = 4.2f;
                sObj.FindProperty("combo1LungeDuration").floatValue = 0.08f;

                sObj.FindProperty("combo2Damage").intValue = 19;
                sObj.FindProperty("combo2Radius").floatValue = 1.75f;
                sObj.FindProperty("combo2ArcDegrees").floatValue = 145f;
                sObj.FindProperty("combo2Knockback").floatValue = 5.5f;
                sObj.FindProperty("combo2Cooldown").floatValue = 0.26f;
                sObj.FindProperty("combo2StaminaCost").intValue = 2;
                sObj.FindProperty("combo2LungeSpeed").floatValue = 5.6f;
                sObj.FindProperty("combo2LungeDuration").floatValue = 0.09f;

                sObj.FindProperty("combo3Damage").intValue = 28;
                sObj.FindProperty("combo3Radius").floatValue = 2.15f;
                sObj.FindProperty("combo3ArcDegrees").floatValue = 360f;
                sObj.FindProperty("combo3Knockback").floatValue = 9.5f;
                sObj.FindProperty("combo3Cooldown").floatValue = 0.44f;
                sObj.FindProperty("combo3StaminaCost").intValue = 3;
                sObj.FindProperty("combo3LungeSpeed").floatValue = 6.2f;
                sObj.FindProperty("combo3LungeDuration").floatValue = 0.11f;

                sObj.FindProperty("dashStrikeDamage").intValue = 22;
                sObj.FindProperty("dashStrikeRadius").floatValue = 1.90f;
                sObj.FindProperty("dashStrikeArcDegrees").floatValue = 110f;
                sObj.FindProperty("dashStrikeKnockback").floatValue = 7.0f;
                sObj.FindProperty("dashStrikeCooldown").floatValue = 0.26f;
                sObj.FindProperty("dashStrikeStaminaCost").intValue = 2;
                sObj.FindProperty("dashStrikeLungeSpeed").floatValue = 8.5f;
                sObj.FindProperty("dashStrikeLungeDuration").floatValue = 0.11f;

                sObj.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(so);
                updated++;
                Debug.Log("[MV][StepC2] Đã cập nhật thông số Combo mặc định cho: " + path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][StepC2] === HOÀN TẤT STEP C2 (" + updated + " PlayerStatsSO)! Chạy 'Moonpetal/Step C2/Validate'. ===");
        }

        [MenuItem("Moonpetal/Step C2/Validate")]
        public static void ValidateStepC2()
        {
            int errors = 0;
            Debug.Log("[MV][StepC2] === BẮT ĐẦU VALIDATE STEP C2 ===");

            string[] guids = AssetDatabase.FindAssets("t:PlayerStatsSO");
            if (guids == null || guids.Length == 0)
            {
                Debug.LogError("[MV][StepC2] THIẾU asset PlayerStatsSO.");
                errors++;
            }
            else
            {
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    PlayerStatsSO so = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(path);
                    if (so == null) continue;
                    if (!so.enableComboSystem)
                    {
                        Debug.LogWarning("[MV][StepC2] enableComboSystem đang TẮT trên " + path);
                    }
                    if (so.combo3Damage <= so.combo1Damage || so.comboWindowDuration <= 0.1f)
                    {
                        Debug.LogError("[MV][StepC2] Thông số Combo chưa hợp lệ trên " + path + ". Hãy chạy lại Build Step C2.");
                        errors++;
                    }
                }
            }

            GameObject wenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (wenPrefab == null)
            {
                Debug.LogError("[MV][StepC2] THIẾU Player_Wen.prefab.");
                errors++;
            }
            else
            {
                if (wenPrefab.GetComponent<PlayerController2D>() == null)
                {
                    Debug.LogError("[MV][StepC2] Player_Wen.prefab thiếu PlayerController2D.");
                    errors++;
                }
                if (wenPrefab.GetComponent<PlayerCombatController>() == null)
                {
                    Debug.LogError("[MV][StepC2] Player_Wen.prefab thiếu PlayerCombatController.");
                    errors++;
                }
                if (wenPrefab.GetComponent<WenSpriteAnimator>() == null)
                {
                    Debug.LogError("[MV][StepC2] Player_Wen.prefab thiếu WenSpriteAnimator.");
                    errors++;
                }
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(wenPrefab) > 0)
                {
                    Debug.LogError("[MV][StepC2] Player_Wen.prefab có Missing Script.");
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV][StepC2] VALIDATE STEP C2: PASS (0 lỗi). Chuỗi 3 đòn Combo + Input Buffer + Micro-Lunge + Dash-Strike đã sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV][StepC2] VALIDATE STEP C2: FAIL (" + errors + " lỗi).");
            }
        }
    }
}
