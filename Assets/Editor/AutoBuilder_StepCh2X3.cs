using System.Collections.Generic;
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
    public static class AutoBuilder_StepCh2X3 {
        private const string RiverScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string ItemsDataDir = "Assets/Data/Items";
        private const string CropsDataDir = "Assets/Data/Crops";
        private const string ItemsArtDir = "Assets/Art/Sprites/Items";
        private const string Ch2FarmArtDir = "Assets/Art/Sprites/Chapter2/Farming";
        private const string ItemDbPath = "Assets/Data/Config/ItemDatabase.asset";
        private const string VNTablePath = "Assets/Data/Localization/VNStringTable.asset";

        [MenuItem("Moonpetal/Step Ch2-X3/1. Build Combat Farming, Brewing Cauldron & Quick-Use HUD", false, 540)]
        public static void ApplyStepCh2X3() {
            ConfigureNewFarmArt();
            AssetDatabase.Refresh();

            EnsureLocalizationEntries();

            CropData barleyCrop = AssetDatabase.LoadAssetAtPath<CropData>(CropsDataDir + "/Crop_DewBarley.asset");
            if (barleyCrop == null) {
                string[] cGuids = AssetDatabase.FindAssets("t:CropData", new[] { "Assets/Data" });
                if (cGuids.Length > 0) barleyCrop = AssetDatabase.LoadAssetAtPath<CropData>(AssetDatabase.GUIDToAssetPath(cGuids[0]));
            }

            ItemData moonbudCropItem = UpsertItem("Item_MoonbudCrop", "item_moonbud_crop", "item.moonbud_crop.name", "item.moonbud_crop.desc",
                ItemsArtDir + "/icon_item_moonbud_crop.png", ItemCategory.Crop, 99, 10, 15, false, 0, null);
            ItemData chiliCropItem = UpsertItem("Item_FireflyChili", "item_firefly_chili", "item.firefly_chili.name", "item.firefly_chili.desc",
                ItemsArtDir + "/icon_item_firefly_chili.png", ItemCategory.Crop, 99, 10, 0, false, 0, null);

            CropData moonbudCrop = UpsertCrop("Crop_Moonbud", "crop_moonbud", "crop.moonbud.name", null, moonbudCropItem, 2, 1, "crop_moonbud_stage_");
            CropData chiliCrop = UpsertCrop("Crop_FireflyChili", "crop_firefly_chili", "crop.firefly_chili.name", null, chiliCropItem, 2, 1, "crop_firefly_chili_stage_");

            ItemData moonbudSeed = UpsertItem("Item_MoonbudSeed", "item_moonbud_seed", "item.moonbud_seed.name", "item.moonbud_seed.desc",
                ItemsArtDir + "/icon_item_moonbud_seed.png", ItemCategory.Seed, 99, 0, 0, false, 2, moonbudCrop);
            ItemData chiliSeed = UpsertItem("Item_FireflyChiliSeed", "item_firefly_chili_seed", "item.firefly_chili_seed.name", "item.firefly_chili_seed.desc",
                ItemsArtDir + "/icon_item_firefly_chili_seed.png", ItemCategory.Seed, 99, 0, 0, false, 2, chiliCrop);

            moonbudCrop.seedItem = moonbudSeed; EditorUtility.SetDirty(moonbudCrop);
            chiliCrop.seedItem = chiliSeed; EditorUtility.SetDirty(chiliCrop);

            ItemData dewTea = UpsertItem("Item_DewTea", "item_dew_tea", "item.dew_tea.name", "item.dew_tea.desc",
                ItemsArtDir + "/icon_item_dew_tea.png", ItemCategory.Consumable, 20, 15, 45, true, 0, null);
            ItemData moonElixir = UpsertItem("Item_MoonElixir", "item_moon_elixir", "item.moon_elixir.name", "item.moon_elixir.desc",
                ItemsArtDir + "/icon_item_moon_elixir.png", ItemCategory.Consumable, 20, 20, 85, true, 0, null);
            ItemData nightfireCake = UpsertItem("Item_NightfireCake", "item_nightfire_cake", "item.nightfire_cake.name", "item.nightfire_cake.desc",
                ItemsArtDir + "/icon_item_nightfire_cake.png", ItemCategory.Consumable, 20, 20, 10, false, 0, null);
            ItemData starFert = UpsertItem("Item_StarFertilizer", "item_star_fertilizer", "item.star_fertilizer.name", "item.star_fertilizer.desc",
                ItemsArtDir + "/icon_item_star_fertilizer.png", ItemCategory.Material, 99, 0, 0, false, 0, null);

            RegisterItemsInDatabase(new[] { moonbudCropItem, chiliCropItem, moonbudSeed, chiliSeed, dewTea, moonElixir, nightfireCake, starFert });
            AttachManagersToGameRoot();
            BuildEastNurseryFarmAndCauldron(barleyCrop, moonbudCrop, chiliCrop);

            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Ch2-X3] HOAN TAT: Da xay dung 12 o ruong Vuon Uom, Vac Nau Thuoc, 8 ItemData/2 CropData & phim nong [F]/[C]!");
            ValidateStepCh2X3();
        }

        [MenuItem("Moonpetal/Step Ch2-X3/2. Validate Step Ch2-X3", false, 541)]
        public static void ValidateStepCh2X3() {
            int errors = 0;
            Debug.Log("[MV][Ch2-X3] === BAT DAU VALIDATE STEP CH2-X3 ===");

            ItemDatabaseSO db = FindItemDatabase();
            string[] mustIds = { "item_moonbud_seed", "item_moonbud_crop", "item_firefly_chili_seed", "item_firefly_chili", "item_dew_tea", "item_moon_elixir", "item_nightfire_cake", "item_star_fertilizer" };
            for (int i = 0; i < mustIds.Length; i++) {
                if (db == null || db.GetById(mustIds[i]) == null) { Debug.LogError("[MV][Ch2-X3][FAIL] ItemDatabase thieu: " + mustIds[i]); errors++; }
            }

            GameObject gr = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (gr == null || gr.GetComponent<PlayerCombatBuffController>() == null || gr.GetComponent<QuickConsumableHUDView>() == null || gr.GetComponent<BrewingCauldronUIView>() == null) { Debug.LogError("[MV][Ch2-X3][FAIL] GameRoot.prefab thieu PlayerCombatBuffController / QuickConsumableHUDView / BrewingCauldronUIView!"); errors++; }

            Scene river = EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);
            FarmGridManager fgm = Object.FindAnyObjectByType<FarmGridManager>();
            FarmTileController[] tiles = Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            BrewingCauldronInteractable cauldron = Object.FindAnyObjectByType<BrewingCauldronInteractable>();
            if (fgm == null || tiles.Length < 12) { Debug.LogError("[MV][Ch2-X3][FAIL] Riverbank_Chapter2 can FarmGridManager + >=12 o ruong, hien co: " + tiles.Length); errors++; }
            if (cauldron == null) { Debug.LogError("[MV][Ch2-X3][FAIL] Riverbank_Chapter2 thieu BrewingCauldron_EastNursery!"); errors++; }

            if (errors == 0)
                Debug.Log("[MV][Ch2-X3] === VALIDATE PASS 100% (0 LOI) ===");
            else
                Debug.LogError("[MV][Ch2-X3] === VALIDATE FAIL (" + errors + " LOI) ===");
        }

        private static void ConfigureNewFarmArt() {
            if (!Directory.Exists(Ch2FarmArtDir)) return;
            string[] files = Directory.GetFiles(Ch2FarmArtDir, "*.png");
            for (int i = 0; i < files.Length; i++) {
                string path = files[i].Replace("\\", "/");
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;
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

        private static ItemData UpsertItem(string assetName, string id, string nameKey, string descKey, string iconPath,
            ItemCategory cat, int maxStack, int stam, int hp, bool clearExh, int growDays, CropData crop) {
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
            item.staminaRestore = stam;
            item.lightHPRestore = hp;
            item.clearsExhaustionPenalty = clearExh;
            item.growDays = growDays;
            item.linkedCropData = crop;
            EditorUtility.SetDirty(item);
            return item;
        }

        private static CropData UpsertCrop(string assetName, string id, string nameKey, ItemData seed, ItemData harvest, int growDays, int yieldAmt, string stagePrefix) {
            if (!Directory.Exists(CropsDataDir)) Directory.CreateDirectory(CropsDataDir);
            string path = CropsDataDir + "/" + assetName + ".asset";
            CropData crop = AssetDatabase.LoadAssetAtPath<CropData>(path);
            if (crop == null) {
                crop = ScriptableObject.CreateInstance<CropData>();
                AssetDatabase.CreateAsset(crop, path);
            }
            crop.cropID = id;
            crop.cropNameKey = nameKey;
            crop.seedItem = seed;
            crop.harvestedItem = harvest;
            crop.growDays = growDays;
            crop.harvestYield = yieldAmt;
            crop.stageSprites = new Sprite[4];
            for (int i = 0; i < 4; i++)
                crop.stageSprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2FarmArtDir + "/" + stagePrefix + i + ".png");
            EditorUtility.SetDirty(crop);
            return crop;
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
                if (items[i] == null) continue;
                if (!db.allItems.Contains(items[i])) db.allItems.Add(items[i]);
            }
            EditorUtility.SetDirty(db);
        }

        private static void EnsureLocalizationEntries() {
            StringTableSO table = AssetDatabase.LoadAssetAtPath<StringTableSO>(VNTablePath);
            if (table == null) return;

            var pairs = new (string k, string v)[] {
                ("item.moonbud_seed.name", "Hạt Cánh Trăng Non"),
                ("item.moonbud_seed.desc", "Gieo ở Vườn Ươm 2 ngày để thu hoạch Cánh Trăng Non nấu thuốc hồi máu mạnh."),
                ("item.moonbud_crop.name", "Cánh Trăng Non"),
                ("item.moonbud_crop.desc", "Hoa phát sáng dịu dùng nấu Tinh Chất Ánh Trăng (+85 Máu + 2s Bất Tử)."),
                ("item.firefly_chili_seed.name", "Hạt Ớt Đom Đóm"),
                ("item.firefly_chili_seed.desc", "Gieo 2 ngày thu hoạch Ớt Đom Đóm làm Bánh Lửa Đêm tăng sát thương."),
                ("item.firefly_chili.name", "Ớt Đom Đóm"),
                ("item.firefly_chili.desc", "Trái ớt cay phát sáng dùng nướng Bánh Lửa Đêm."),
                ("item.dew_tea.name", "Trà Sương Ấm"),
                ("item.dew_tea.desc", "Bấm [F] để uống nhanh: hồi +45 Máu Ánh Sáng & +15 Thể Lực."),
                ("item.moon_elixir.name", "Tinh Chất Ánh Trăng"),
                ("item.moon_elixir.desc", "Bấm [F] để uống nhanh: hồi +85 Máu Ánh Sáng, +20 Thể Lực & 2s Bất Tử."),
                ("item.nightfire_cake.name", "Bánh Lửa Đêm"),
                ("item.nightfire_cake.desc", "Bấm [C] để ăn nhanh: +40% Sát Thương & +25% Tốc Độ trong 45 giây."),
                ("item.star_fertilizer.name", "Phân Bón Tinh Bụi"),
                ("item.star_fertilizer.desc", "Rắc lên cây đang trồng để chín tức thì và hóa Phát Sáng x2 sản lượng!"),
                ("crop.moonbud.name", "Cánh Trăng Non"),
                ("crop.firefly_chili.name", "Ớt Đom Đóm")
            };

            for (int p = 0; p < pairs.Length; p++) {
                table.SetEntry(pairs[p].k, pairs[p].v);
            }
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

        private static void AttachManagersToGameRoot() {
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;
            TMP_FontAsset vnFont = FindVNFont();

            if (root.GetComponent<PlayerCombatBuffController>() == null)
                root.AddComponent<PlayerCombatBuffController>();

            var qhud = root.GetComponent<QuickConsumableHUDView>();
            if (qhud == null) qhud = root.AddComponent<QuickConsumableHUDView>();
            if (vnFont != null) {
                var qso = new SerializedObject(qhud);
                qso.FindProperty("vnFont").objectReferenceValue = vnFont;
                qso.ApplyModifiedPropertiesWithoutUndo();
            }

            var cui = root.GetComponent<BrewingCauldronUIView>();
            if (cui == null) cui = root.AddComponent<BrewingCauldronUIView>();
            if (vnFont != null) {
                var cso = new SerializedObject(cui);
                cso.FindProperty("vnFont").objectReferenceValue = vnFont;
                cso.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static Sprite FindSpriteByKeywords(string[] keywords) {
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Art" });
            for (int k = 0; k < keywords.Length; k++) {
                string kw = keywords[k].ToLower();
                for (int i = 0; i < guids.Length; i++) {
                    string p = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if (p.ToLower().Contains(kw))
                        return AssetDatabase.LoadAssetAtPath<Sprite>(p);
                }
            }
            return null;
        }

        private static void BuildEastNurseryFarmAndCauldron(CropData barleyCrop, CropData moonbudCrop, CropData chiliCrop) {
            Scene river = EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);

            GameObject oldFarm = GameObject.Find("EastNursery_FarmPlots");
            if (oldFarm != null) Object.DestroyImmediate(oldFarm);
            GameObject oldCaul = GameObject.Find("BrewingCauldron_EastNursery");
            if (oldCaul != null) Object.DestroyImmediate(oldCaul);

            Sprite sprUntilled = FindSpriteByKeywords(new[] { "soil_untilled", "farm_untilled", "tilled_dry" });
            Sprite sprDry = FindSpriteByKeywords(new[] { "soil_tilled_dry", "tilled_dry", "soil_dry" });
            Sprite sprWet = FindSpriteByKeywords(new[] { "soil_tilled_wet", "tilled_wet", "soil_wet" });

            GameObject farmRoot = new GameObject("EastNursery_FarmPlots");
            FarmGridManager mgr = farmRoot.AddComponent<FarmGridManager>();

            List<FarmTileController> tiles = new List<FarmTileController>();
            float startX = 16.5f;
            float startY = 1.4f;
            int idx = 0;
            for (int r = 0; r < 3; r++) {
                for (int c = 0; c < 4; c++) {
                    idx++;
                    GameObject tGo = new GameObject("Ch2_FarmTile_" + idx.ToString("00"));
                    tGo.transform.SetParent(farmRoot.transform, false);
                    tGo.transform.position = new Vector3(startX + c * 1.15f, startY + r * 1.05f, 0f);

                    var soilSr = tGo.AddComponent<SpriteRenderer>();
                    soilSr.sprite = sprUntilled;
                    soilSr.sortingLayerName = "GroundDecor";
                    soilSr.sortingOrder = 15;

                    GameObject cropGo = new GameObject("CropVisual");
                    cropGo.transform.SetParent(tGo.transform, false);
                    cropGo.transform.localPosition = new Vector3(0f, -0.35f, 0f);
                    var cropSr = cropGo.AddComponent<SpriteRenderer>();
                    cropSr.sortingLayerName = "WorldObjects";
                    cropSr.sortingOrder = Mathf.RoundToInt(-tGo.transform.position.y * 100f);
                    cropSr.enabled = false;

                    var tile = tGo.AddComponent<FarmTileController>();
                    var tso = new SerializedObject(tile);
                    tso.FindProperty("tileId").stringValue = "ch2_nursery_" + idx.ToString("00");
                    tso.FindProperty("soilRenderer").objectReferenceValue = soilSr;
                    tso.FindProperty("cropRenderer").objectReferenceValue = cropSr;
                    tso.FindProperty("untilledSprite").objectReferenceValue = sprUntilled;
                    tso.FindProperty("tilledDrySprite").objectReferenceValue = sprDry;
                    tso.FindProperty("tilledWetSprite").objectReferenceValue = sprWet;
                    tso.ApplyModifiedPropertiesWithoutUndo();
                    tiles.Add(tile);
                }
            }

            var mso = new SerializedObject(mgr);
            var tilesProp = mso.FindProperty("tiles");
            tilesProp.ClearArray();
            for (int i = 0; i < tiles.Count; i++) {
                tilesProp.InsertArrayElementAtIndex(i);
                tilesProp.GetArrayElementAtIndex(i).objectReferenceValue = tiles[i];
            }
            mso.FindProperty("defaultCropData").objectReferenceValue = barleyCrop;
            var catProp = mso.FindProperty("cropCatalog");
            catProp.ClearArray();
            CropData[] allCrops = { barleyCrop, moonbudCrop, chiliCrop };
            int ci = 0;
            for (int i = 0; i < allCrops.Length; i++) {
                if (allCrops[i] == null) continue;
                catProp.InsertArrayElementAtIndex(ci);
                catProp.GetArrayElementAtIndex(ci).objectReferenceValue = allCrops[i];
                ci++;
            }
            mso.ApplyModifiedPropertiesWithoutUndo();

            // Dat Vac Nau Thuoc ben canh Vuon Uom Bo Dong
            GameObject caul = new GameObject("BrewingCauldron_EastNursery");
            caul.transform.position = new Vector3(15.0f, 3.2f, 0f);
            var csr = caul.AddComponent<SpriteRenderer>();
            csr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2FarmArtDir + "/prop_brewing_cauldron.png");
            csr.sortingLayerName = "WorldObjects";
            csr.sortingOrder = Mathf.RoundToInt(-caul.transform.position.y * 100f);
            var col = caul.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.85f, 0.55f);
            col.offset = new Vector2(0f, 0.25f);
            caul.AddComponent<BrewingCauldronInteractable>();

            EditorSceneManager.MarkSceneDirty(river);
            EditorSceneManager.SaveScene(river);
        }
    }
}
