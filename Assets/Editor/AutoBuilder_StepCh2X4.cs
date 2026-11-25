using System.IO;
using UnityEditor;
using UnityEngine;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools {
    public static class AutoBuilder_StepCh2X4 {
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string ItemsDataDir = "Assets/Data/Items";
        private const string ItemsArtDir = "Assets/Art/Sprites/Items";
        private const string ItemDbPath = "Assets/Data/Config/ItemDatabase.asset";
        private const string VNTablePath = "Assets/Data/Localization/VNStringTable.asset";

        [MenuItem("Moonpetal/Step Ch2-X4/1. Build Moonlight Fever & Swamp Loot System", false, 550)]
        public static void ApplyStepCh2X4() {
            AssetDatabase.Refresh();
            EnsureLocalizationEntries();

            ItemData gloomShard = UpsertItem(
                "Item_GloomShard",
                "item_gloom_shard",
                "item.gloom_shard.name",
                "item.gloom_shard.desc",
                ItemsArtDir + "/icon_item_gloom_shard.png",
                ItemCategory.Material,
                99
            );
            RegisterItemInDatabase(gloomShard);
            AttachFeverToGameRoot();

            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Ch2-X4] HOAN TAT: Da tao Item_GloomShard, gan MoonlightFeverController + HUD len GameRoot & kich hoat loot roi dam lay!");
            ValidateStepCh2X4();
        }

        [MenuItem("Moonpetal/Step Ch2-X4/2. Validate Step Ch2-X4", false, 551)]
        public static void ValidateStepCh2X4() {
            int errors = 0;
            Debug.Log("[MV][Ch2-X4] === BAT DAU VALIDATE STEP CH2-X4 ===");

            ItemDatabaseSO db = FindItemDatabase();
            if (db == null || db.GetById("item_gloom_shard") == null) {
                Debug.LogError("[MV][Ch2-X4][FAIL] ItemDatabase thieu item_gloom_shard!");
                errors++;
            } else if (db.GetById("item_gloom_shard").icon == null) {
                Debug.LogError("[MV][Ch2-X4][FAIL] Item_GloomShard thieu icon_item_gloom_shard.png!");
                errors++;
            }

            GameObject gr = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (gr == null || gr.GetComponent<MoonlightFeverController>() == null || gr.GetComponent<MoonlightFeverHUDView>() == null) {
                Debug.LogError("[MV][Ch2-X4][FAIL] GameRoot.prefab thieu MoonlightFeverController hoac MoonlightFeverHUDView!");
                errors++;
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-X4] === VALIDATE PASS 100% (0 LOI) ===");
            else
                Debug.LogError("[MV][Ch2-X4] === VALIDATE FAIL (" + errors + " LOI) ===");
        }

        private static ItemData UpsertItem(string assetName, string id, string nameKey, string descKey, string iconPath, ItemCategory cat, int maxStack) {
            if (!Directory.Exists(ItemsDataDir)) Directory.CreateDirectory(ItemsDataDir);
            string path = ItemsDataDir + "/" + assetName + ".asset";
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item == null) {
                item = ScriptableObject.CreateInstance<ItemData>();
                AssetDatabase.CreateAsset(item, path);
            }
            item.itemID = id;
            item.displayNameKey = nameKey;
            item.descriptionKey = descKey;
            item.icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            item.category = cat;
            item.maxStack = maxStack;
            EditorUtility.SetDirty(item);
            return item;
        }

        private static ItemDatabaseSO FindItemDatabase() {
            ItemDatabaseSO db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDbPath);
            if (db != null) return db;
            string[] guids = AssetDatabase.FindAssets("t:ItemDatabaseSO", new[] { "Assets/Data" });
            return guids.Length > 0 ? AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(AssetDatabase.GUIDToAssetPath(guids[0])) : null;
        }

        private static void RegisterItemInDatabase(ItemData item) {
            ItemDatabaseSO db = FindItemDatabase();
            if (db == null || item == null) return;
            if (!db.allItems.Contains(item)) db.allItems.Add(item);
            EditorUtility.SetDirty(db);
        }

        private static void EnsureLocalizationEntries() {
            StringTableSO table = AssetDatabase.LoadAssetAtPath<StringTableSO>(VNTablePath);
            if (table == null) {
                string[] guids = AssetDatabase.FindAssets("t:StringTableSO", new[] { "Assets/Data" });
                if (guids.Length > 0) table = AssetDatabase.LoadAssetAtPath<StringTableSO>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            if (table == null) return;
            table.SetEntry("item.gloom_shard.name", "Mảnh Sương Đen");
            table.SetEntry("item.gloom_shard.desc", "Tinh thể sương cô đặc rơi từ quái đầm lầy. Dùng để rèn nâng cấp trang bị tại Lò Rèn của Ông Kael.");
            EditorUtility.SetDirty(table);
        }

        private static TMP_FontAsset FindVNFont() {
            string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets" });
            for (int i = 0; i < guids.Length; i++) {
                string p = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (p.ToLower().Contains("bevietnam") || p.ToLower().Contains("vn") || p.ToLower().Contains("noto"))
                    return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(p);
            }
            return guids.Length > 0 ? AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guids[0])) : null;
        }

        private static void AttachFeverToGameRoot() {
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;
            TMP_FontAsset vnFont = FindVNFont();

            var fever = root.GetComponent<MoonlightFeverController>();
            if (fever == null) fever = root.AddComponent<MoonlightFeverController>();
            if (vnFont != null) {
                var fso = new SerializedObject(fever);
                fso.FindProperty("vnFont").objectReferenceValue = vnFont;
                fso.ApplyModifiedPropertiesWithoutUndo();
            }

            var fhud = root.GetComponent<MoonlightFeverHUDView>();
            if (fhud == null) fhud = root.AddComponent<MoonlightFeverHUDView>();
            if (vnFont != null) {
                var hso = new SerializedObject(fhud);
                hso.FindProperty("vnFont").objectReferenceValue = vnFont;
                hso.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }
    }
}
