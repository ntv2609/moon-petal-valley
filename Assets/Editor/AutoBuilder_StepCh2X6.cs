using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;
using MoonpetalValley.Player;
using MoonpetalValley.UI;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools {
    public static class AutoBuilder_StepCh2X6 {
        private const string RiverScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string SwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string ItemsDataDir = "Assets/Data/Items";
        private const string ItemsArtDir = "Assets/Art/Sprites/Items";
        private const string PropsArtDir = "Assets/Art/Sprites/Chapter2/Props";
        private const string ItemDbPath = "Assets/Data/Config/ItemDatabase.asset";
        private const string VNTablePath = "Assets/Data/Localization/VNStringTable.asset";

        [MenuItem("Moonpetal/Step Ch2-X6/1. Build Kael Forge, 4 Mini-Bounties & Run 5-Scene QA", false, 570)]
        public static void ApplyStepCh2X6() {
            ConfigurePropSprite(PropsArtDir + "/prop_kael_forge.png");
            ConfigurePropSprite(PropsArtDir + "/prop_bounty_board.png");
            AssetDatabase.Refresh();

            EnsureLocalizationEntries();

            ItemData sickleLv2 = UpsertItem("Item_MoonSickleLv2", "item_moon_sickle_lv2", "item.moon_sickle_lv2.name", "item.moon_sickle_lv2.desc",
                ItemsArtDir + "/icon_item_moon_sickle_lv2.png", ItemCategory.Tool, 1);
            ItemData waterLv2 = UpsertItem("Item_WateringCanLv2", "item_watering_can_lv2", "item.watering_can_lv2.name", "item.watering_can_lv2.desc",
                ItemsArtDir + "/icon_item_watering_can_lv2.png", ItemCategory.Tool, 1);
            ItemData boots = UpsertItem("Item_NightwindBoots", "item_nightwind_boots", "item.nightwind_boots.name", "item.nightwind_boots.desc",
                ItemsArtDir + "/icon_item_nightwind_boots.png", ItemCategory.Material, 1);

            RegisterItemsInDatabase(new[] { sickleLv2, waterLv2, boots });
            AttachManagersToGameRoot();
            PlaceForgeAndBountyBoardInRiverbank();

            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Ch2-X6] HOAN TAT: Da dung Lo Ren Ong Kael, Bang 4 Mini-Bounties & 3 trang bi nang cap!");
            ValidateAllFiveScenesGateQA();
        }

        [MenuItem("Moonpetal/Step Ch2-X6/2. Full 5-Scene Gate QA Validator", false, 571)]
        public static void ValidateAllFiveScenesGateQA() {
            int errors = 0;
            Debug.Log("[MV][Ch2-X6] === BAT DAU FULL 5-SCENE GATE QA ===");

            ItemDatabaseSO db = FindItemDatabase();
            string[] allNewIds = {
                "item_moonbud_seed", "item_moonbud_crop", "item_firefly_chili_seed", "item_firefly_chili",
                "item_dew_tea", "item_moon_elixir", "item_nightfire_cake", "item_star_fertilizer",
                "item_gloom_shard", "item_moon_sickle_lv2", "item_watering_can_lv2", "item_nightwind_boots"
            };
            for (int i = 0; i < allNewIds.Length; i++) {
                var it = db != null ? db.GetById(allNewIds[i]) : null;
                if (it == null || it.icon == null) {
                    Debug.LogError("[MV][QA][FAIL] ItemDatabase thieu hoac mat icon: " + allNewIds[i]);
                    errors++;
                }
            }

            GameObject gr = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (gr == null
                || gr.GetComponent<PlayerCombatBuffController>() == null
                || gr.GetComponent<QuickConsumableHUDView>() == null
                || gr.GetComponent<BrewingCauldronUIView>() == null
                || gr.GetComponent<MoonlightFeverController>() == null
                || gr.GetComponent<MoonlightFeverHUDView>() == null
                || gr.GetComponent<KaelForgeUIView>() == null
                || gr.GetComponent<Chapter2BountyManager>() == null
                || gr.GetComponent<BountyBoardUIView>() == null) {
                Debug.LogError("[MV][QA][FAIL] GameRoot.prefab thieu component X3/X4/X6!");
                errors++;
            }

            EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);
            if (Object.FindAnyObjectByType<RiverBridgeController>() == null) { Debug.LogError("[MV][QA][FAIL] Riverbank thieu RiverBridgeController!"); errors++; }
            if (Object.FindAnyObjectByType<LanternRelayManager>() == null) { Debug.LogError("[MV][QA][FAIL] Riverbank thieu LanternRelayManager!"); errors++; }
            if (Object.FindAnyObjectByType<BrewingCauldronInteractable>() == null) { Debug.LogError("[MV][QA][FAIL] Riverbank thieu BrewingCauldron!"); errors++; }
            if (Object.FindAnyObjectByType<KaelForgeInteractable>() == null) { Debug.LogError("[MV][QA][FAIL] Riverbank thieu KaelForgeInteractable!"); errors++; }
            if (Object.FindAnyObjectByType<BountyBoardInteractable>() == null) { Debug.LogError("[MV][QA][FAIL] Riverbank thieu BountyBoardInteractable!"); errors++; }
            if (Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include).Length < 12) { Debug.LogError("[MV][QA][FAIL] Riverbank thieu 12 o ruong Vuon Uom!"); errors++; }

            if (File.Exists(SwampScenePath)) {
                EditorSceneManager.OpenScene(SwampScenePath, OpenSceneMode.Single);
                if (Object.FindAnyObjectByType<SwampWaveCombatController>() == null) {
                    Debug.LogError("[MV][QA][FAIL] WestSwamp_Chapter2 thieu SwampWaveCombatController!");
                    errors++;
                }
            }

            var buildScenes = EditorBuildSettings.scenes;
            int scanned = 0;
            for (int s = 0; s < buildScenes.Length; s++) {
                if (!buildScenes[s].enabled || !File.Exists(buildScenes[s].path)) continue;
                Scene sc = EditorSceneManager.OpenScene(buildScenes[s].path, OpenSceneMode.Single);
                scanned++;
                GameObject[] roots = sc.GetRootGameObjects();
                for (int r = 0; r < roots.Length; r++) {
                    Transform[] all = roots[r].GetComponentsInChildren<Transform>(true);
                    for (int t = 0; t < all.Length; t++) {
                        int miss = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(all[t].gameObject);
                        if (miss > 0) {
                            Debug.LogError($"[MV][QA][FAIL] Scene {sc.name} -> {all[t].name} co {miss} Missing Script!");
                            errors++;
                        }
                    }
                }
            }

            EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);
            if (errors == 0)
                Debug.Log($"[MV][Ch2-X6] === FULL GATE QA PASS 100% (0 LOI | Da quet {scanned} scenes trong Build Settings) ===");
            else
                Debug.LogError($"[MV][Ch2-X6] === FULL GATE QA FAIL ({errors} LOI) ===");
        }

        private static void PlaceForgeAndBountyBoardInRiverbank() {
            Scene river = EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);

            GameObject oldForge = GameObject.Find("KaelForge_EastWorkshop");
            if (oldForge != null) Object.DestroyImmediate(oldForge);
            GameObject forgeGo = new GameObject("KaelForge_EastWorkshop");
            forgeGo.transform.position = new Vector3(12.2f, 3.8f, 0f);
            var fsr = forgeGo.AddComponent<SpriteRenderer>();
            fsr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PropsArtDir + "/prop_kael_forge.png");
            fsr.sortingLayerName = "WorldObjects";
            fsr.sortingOrder = Mathf.RoundToInt(-forgeGo.transform.position.y * 100f);
            var fcol = forgeGo.AddComponent<BoxCollider2D>();
            fcol.size = new Vector2(0.9f, 0.55f);
            fcol.offset = new Vector2(0f, 0.35f);
            forgeGo.AddComponent<KaelForgeInteractable>();

            GameObject oldBoard = GameObject.Find("BountyBoard_EastBank");
            if (oldBoard != null) Object.DestroyImmediate(oldBoard);
            GameObject boardGo = new GameObject("BountyBoard_EastBank");
            boardGo.transform.position = new Vector3(14.2f, 4.1f, 0f);
            var bsr = boardGo.AddComponent<SpriteRenderer>();
            bsr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PropsArtDir + "/prop_bounty_board.png");
            bsr.sortingLayerName = "WorldObjects";
            bsr.sortingOrder = Mathf.RoundToInt(-boardGo.transform.position.y * 100f);
            var bcol = boardGo.AddComponent<BoxCollider2D>();
            bcol.size = new Vector2(0.85f, 0.45f);
            bcol.offset = new Vector2(0f, 0.30f);
            boardGo.AddComponent<BountyBoardInteractable>();

            EditorSceneManager.MarkSceneDirty(river);
            EditorSceneManager.SaveScene(river);
        }

        private static void AttachManagersToGameRoot() {
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;
            TMP_FontAsset vnFont = FindVNFont();

            var forgeUi = root.GetComponent<KaelForgeUIView>();
            if (forgeUi == null) forgeUi = root.AddComponent<KaelForgeUIView>();
            if (vnFont != null) {
                var so = new SerializedObject(forgeUi);
                so.FindProperty("vnFont").objectReferenceValue = vnFont;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            if (root.GetComponent<Chapter2BountyManager>() == null)
                root.AddComponent<Chapter2BountyManager>();

            var bountyUi = root.GetComponent<BountyBoardUIView>();
            if (bountyUi == null) bountyUi = root.AddComponent<BountyBoardUIView>();
            if (vnFont != null) {
                var bso = new SerializedObject(bountyUi);
                bso.FindProperty("vnFont").objectReferenceValue = vnFont;
                bso.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
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

        private static void RegisterItemsInDatabase(ItemData[] items) {
            ItemDatabaseSO db = FindItemDatabase();
            if (db == null) return;
            for (int i = 0; i < items.Length; i++) {
                if (items[i] != null && !db.allItems.Contains(items[i])) db.allItems.Add(items[i]);
            }
            EditorUtility.SetDirty(db);
        }

        private static void EnsureLocalizationEntries() {
            StringTableSO table = AssetDatabase.LoadAssetAtPath<StringTableSO>(VNTablePath);
            if (table == null) {
                string[] guids = AssetDatabase.FindAssets("t:StringTableSO", new[] { "Assets/Data" });
                if (guids.Length > 0) table = AssetDatabase.LoadAssetAtPath<StringTableSO>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            if (table == null) return;
            table.SetEntry("item.moon_sickle_lv2.name", "Liềm Ánh Trăng Lv.2");
            table.SetEntry("item.moon_sickle_lv2.desc", "Liềm rèn từ Mảnh Sương Đen: +30% Sát thương Liềm & Sóng Trăng vĩnh viễn.");
            table.SetEntry("item.watering_can_lv2.name", "Bình Tưới Trăng 3x3");
            table.SetEntry("item.watering_can_lv2.desc", "Tưới nước cùng lúc vùng 3x3 ô đất chỉ trong 1 lần bấm.");
            table.SetEntry("item.nightwind_boots.name", "Giày Gió Đêm");
            table.SetEntry("item.nightwind_boots.desc", "Tăng vĩnh viễn +20% tốc độ di chuyển trên mọi bản đồ.");
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

        private static void ConfigurePropSprite(string path) {
            if (!File.Exists(path)) return;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) return;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = 32f;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.alphaIsTransparency = true;
            var s = new TextureImporterSettings();
            ti.ReadTextureSettings(s);
            s.spriteMeshType = SpriteMeshType.FullRect;
            s.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            s.spritePivot = new Vector2(0.5f, 0f);
            ti.SetTextureSettings(s);
            ti.SaveAndReimport();
        }
    }
}
