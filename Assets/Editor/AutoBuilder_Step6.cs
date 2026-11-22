using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Farming;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step6
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string CropDewBarleyPath = "Assets/Data/Crops/Crop_DewBarley.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LootDropPrefabPath = "Assets/Prefabs/Enemies/LootDrop.prefab";

        [MenuItem("Moonpetal/Step 6/1. Build Farming, Crops & Resources")]
        public static void BuildStep6()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 6 ===");

            // 1. Tạo sprite đất, 4 giai đoạn Lúa Mạch Sương Mai, Giếng, Kệ Hạt, Nhánh Cây, Rác Nhà Kính
            Sprite soilUntilled = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_untilled.png", 16, 16, DrawUntilledSoil);
            Sprite soilDry = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_dry.png", 16, 16, DrawTilledDrySoil);
            Sprite soilWet = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_wet.png", 16, 16, DrawTilledWetSoil);

            Sprite[] barleyStages = new Sprite[4];
            barleyStages[0] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_0.png", 16, 16, DrawBarleyStage0);
            barleyStages[1] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_1.png", 16, 16, DrawBarleyStage1);
            barleyStages[2] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_2.png", 16, 16, DrawBarleyStage2);
            barleyStages[3] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_3.png", 16, 16, DrawBarleyStage3);

            Sprite wellSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_well.png", 16, 16, DrawWellSprite);
            Sprite shelfSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_shelf.png", 16, 16, DrawShelfSprite);
            Sprite branchSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_branch.png", 16, 16, DrawBranchSprite);
            Sprite debrisSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_debris.png", 16, 16, DrawDebrisSprite);

            // 2. Tạo Crop_DewBarley.asset & nối vào Item_item_dew_barley_seed
            CropData barleyCrop = EnsureDewBarleyCrop(barleyStages);

            // 3. Gắn PlayerToolController vào Player_Wen.prefab
            UpgradePlayerPrefabWithTools();

            // 4. Nâng cấp GameRoot.prefab thêm nút Debug Tưới Toàn Bộ Ruộng
            UpgradeGameRootDebugWithWaterAll();

            // 5. Dựng 12 ô đất, Kệ Hạt, Giếng Nước, 6 Nhánh Cây Khô, 3 Rác Nhà Kính trong Scene
            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildFarmGridInGreenhouse(barleyCrop, soilUntilled, soilDry, soilWet);
            BuildSeedShelfAndWaterWell(shelfSpr, wellSpr);
            BuildResourceNodes(branchSpr, debrisSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 6! Chạy 'Moonpetal/Step 6/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 6/Validate")]
        public static void ValidateStep6()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 6 ===");

            var crop = AssetDatabase.LoadAssetAtPath<CropData>(CropDewBarleyPath);
            if (crop == null || crop.stageSprites == null || crop.stageSprites.Length < 4)
            {
                Debug.LogError("[MV] Crop_DewBarley.asset thiếu dữ liệu hoặc chưa đủ 4 sprite giai đoạn.");
                errors++;
            }

            var seedItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley_seed.asset");
            if (seedItem == null || seedItem.linkedCropData == null)
            {
                Debug.LogError("[MV] Item_item_dew_barley_seed chưa nối linkedCropData.");
                errors++;
            }

            var farmMgr = UnityEngine.Object.FindAnyObjectByType<FarmGridManager>();
            if (farmMgr == null)
            {
                Debug.LogError("[MV] Thiếu FarmGridManager trong scene.");
                errors++;
            }

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            if (tiles.Length < 12)
            {
                Debug.LogError("[MV] Thiếu ô đất Nhà Kính (cần 12, hiện có " + tiles.Length + ").");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<PlayerToolController>() == null)
            {
                Debug.LogError("[MV] Player_Wen thiếu PlayerToolController.");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<WaterWellInteractable>() == null)
            {
                Debug.LogError("[MV] Thiếu WaterWellInteractable.");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<SeedShelfInteractable>() == null)
            {
                Debug.LogError("[MV] Thiếu SeedShelfInteractable.");
                errors++;
            }

            var nodes = UnityEngine.Object.FindObjectsByType<ResourceNodeController>(FindObjectsInactive.Include);
            if (nodes.Length < 9)
            {
                Debug.LogError("[MV] Thiếu ResourceNodeController (cần 9 = 6 Nhánh cây + 3 Rác Nhà Kính, hiện có " + nodes.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 6: PASS (0 lỗi). 12 Ô đất canh tác, Lúa Mạch 4 giai đoạn, Giếng, Kệ Hạt & Gỗ tái tạo sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 6: FAIL (" + errors + " lỗi).");
            }
        }

        private static CropData EnsureDewBarleyCrop(Sprite[] stages)
        {
            var crop = AssetDatabase.LoadAssetAtPath<CropData>(CropDewBarleyPath);
            if (crop == null)
            {
                crop = ScriptableObject.CreateInstance<CropData>();
                AssetDatabase.CreateAsset(crop, CropDewBarleyPath);
            }

            var seedItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley_seed.asset");
            var harvestItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley.asset");

            crop.cropID = "crop_dew_barley";
            crop.cropNameKey = "name_item_dew_barley";
            crop.seedItem = seedItem;
            crop.harvestedItem = harvestItem;
            crop.growDays = 2;
            crop.harvestYield = 1;
            crop.stageSprites = stages;
            EditorUtility.SetDirty(crop);

            if (seedItem != null)
            {
                seedItem.linkedCropData = crop;
                EditorUtility.SetDirty(seedItem);
            }
            return crop;
        }

        private static void UpgradePlayerPrefabWithTools()
        {
            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null) return;

            var toolCtrl = root.GetComponent<PlayerToolController>();
            if (toolCtrl == null) toolCtrl = root.AddComponent<PlayerToolController>();

            var so = new SerializedObject(toolCtrl);
            so.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            so.FindProperty("playerController").objectReferenceValue = root.GetComponent<PlayerController2D>();
            so.FindProperty("inputReader").objectReferenceValue = root.GetComponent<GameInputReader>();
            so.FindProperty("visualTransform").objectReferenceValue = root.transform.Find("Visual");
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void UpgradeGameRootDebugWithWaterAll()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 880f);
                Transform existing = debugPanel.Find("BtnWaterAllCrops");
                Button btn;
                if (existing != null)
                {
                    btn = existing.GetComponent<Button>();
                }
                else
                {
                    GameObject go = new GameObject("BtnWaterAllCrops");
                    go.transform.SetParent(debugPanel, false);
                    go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
                    var img = go.AddComponent<Image>();
                    img.color = new Color(0.18f, 0.42f, 0.62f, 1f);
                    btn = go.AddComponent<Button>();
                    btn.targetGraphic = img;

                    GameObject txtGo = new GameObject("Label");
                    txtGo.transform.SetParent(go.transform, false);
                    var r = txtGo.AddComponent<RectTransform>();
                    r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
                    r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
                    var tmp = txtGo.AddComponent<TextMeshProUGUI>();
                    tmp.text = "Tưới Toàn Bộ Ruộng Nhà Kính";
                    tmp.fontSize = 15; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
                    if (vnFont != null) tmp.font = vnFont;
                }

                var so = new SerializedObject(debugCtrl);
                so.FindProperty("btnWaterAllCrops").objectReferenceValue = btn;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildFarmGridInGreenhouse(CropData barleyCrop, Sprite untilled, Sprite dry, Sprite wet)
        {
            GameObject old = GameObject.Find("GreenhouseFarmGrid");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject gridGo = new GameObject("GreenhouseFarmGrid");
            var mgr = gridGo.AddComponent<FarmGridManager>();
            var tileList = new List<FarmTileController>();

            // 4 cột x 3 hàng gọn gàng trong Nhà Kính (x: -17..-14, y: 7..9)
            int idx = 0;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    idx++;
                    float wx = -17.5f + c * 1.15f;
                    float wy = 7.5f + r * 1.15f;

                    GameObject tGo = new GameObject("FarmTile_" + idx);
                    tGo.tag = "FarmTile";
                    tGo.transform.SetParent(gridGo.transform, false);
                    tGo.transform.position = new Vector3(wx, wy, 0f);

                    var soilSr = tGo.AddComponent<SpriteRenderer>();
                    soilSr.sprite = untilled;
                    soilSr.sortingLayerName = "FarmTile";
                    soilSr.sortingOrder = 5;

                    GameObject cropGo = new GameObject("CropVisual");
                    cropGo.transform.SetParent(tGo.transform, false);
                    cropGo.transform.localPosition = new Vector3(0f, 0.15f, 0f);
                    var cropSr = cropGo.AddComponent<SpriteRenderer>();
                    cropSr.sortingLayerName = "WorldObjects";
                    cropSr.sortingOrder = Mathf.RoundToInt(-wy * 100f) + 5;
                    cropSr.enabled = false;

                    var ctrl = tGo.AddComponent<FarmTileController>();
                    var so = new SerializedObject(ctrl);
                    so.FindProperty("tileId").stringValue = "gh_tile_" + idx;
                    so.FindProperty("soilRenderer").objectReferenceValue = soilSr;
                    so.FindProperty("cropRenderer").objectReferenceValue = cropSr;
                    so.FindProperty("untilledSprite").objectReferenceValue = untilled;
                    so.FindProperty("tilledDrySprite").objectReferenceValue = dry;
                    so.FindProperty("tilledWetSprite").objectReferenceValue = wet;
                    so.ApplyModifiedPropertiesWithoutUndo();

                    tileList.Add(ctrl);
                }
            }

            var mgrSO = new SerializedObject(mgr);
            mgrSO.FindProperty("defaultCropData").objectReferenceValue = barleyCrop;
            var prop = mgrSO.FindProperty("tiles");
            prop.ClearArray();
            for (int i = 0; i < tileList.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = tileList[i];
            }
            mgrSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildSeedShelfAndWaterWell(Sprite shelfSpr, Sprite wellSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            // 1. Kệ Hạt Giống trong Nhà Kính
            GameObject oldShelf = GameObject.Find("GreenhouseSeedShelf");
            if (oldShelf != null) UnityEngine.Object.DestroyImmediate(oldShelf);
            GameObject shelfGo = new GameObject("GreenhouseSeedShelf");
            shelfGo.tag = "Interactable";
            shelfGo.layer = 11;
            shelfGo.transform.position = new Vector3(-12.5f, 12.5f, 0f);
            var sSr = shelfGo.AddComponent<SpriteRenderer>();
            sSr.sprite = shelfSpr; sSr.sortingLayerName = "WorldObjects"; sSr.sortingOrder = Mathf.RoundToInt(-12.5f * 100f);
            shelfGo.AddComponent<BoxCollider2D>().size = new Vector2(0.9f, 0.9f);
            GameObject shelfPrompt = CreateWorldPrompt(shelfGo.transform, "[E] Lấy Hạt Lúa Mạch", vnFont);
            var shelfComp = shelfGo.AddComponent<SeedShelfInteractable>();
            var sSO = new SerializedObject(shelfComp);
            sSO.FindProperty("promptObject").objectReferenceValue = shelfPrompt;
            sSO.FindProperty("interactRadius").floatValue = 1.9f;
            sSO.ApplyModifiedPropertiesWithoutUndo();

            // 2. Giếng Nước cạnh ao ngoài Nhà Kính
            GameObject oldWell = GameObject.Find("VillageWaterWell");
            if (oldWell != null) UnityEngine.Object.DestroyImmediate(oldWell);
            GameObject wellGo = new GameObject("VillageWaterWell");
            wellGo.tag = "Interactable";
            wellGo.layer = 11;
            wellGo.transform.position = new Vector3(-7.0f, 10.5f, 0f);
            var wSr = wellGo.AddComponent<SpriteRenderer>();
            wSr.sprite = wellSpr; wSr.sortingLayerName = "WorldObjects"; wSr.sortingOrder = Mathf.RoundToInt(-10.5f * 100f);
            wellGo.AddComponent<BoxCollider2D>().size = new Vector2(0.95f, 0.95f);
            GameObject wellPrompt = CreateWorldPrompt(wellGo.transform, "[E] Múc đầy Bình Tưới", vnFont);
            var wellComp = wellGo.AddComponent<WaterWellInteractable>();
            var wSO = new SerializedObject(wellComp);
            wSO.FindProperty("promptObject").objectReferenceValue = wellPrompt;
            wSO.FindProperty("interactRadius").floatValue = 2.0f;
            wSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildResourceNodes(Sprite branchSpr, Sprite debrisSpr)
        {
            GameObject oldGroup = GameObject.Find("ResourceNodesGroup");
            if (oldGroup != null) UnityEngine.Object.DestroyImmediate(oldGroup);
            GameObject group = new GameObject("ResourceNodesGroup");

            ItemData woodItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_branch_wood.asset");
            GameObject lootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath);

            // 6 Nhánh Cây Khô ngoài làng (tự mọc lại mỗi sáng)
            Vector2[] branchPositions =
            {
                new Vector2(-9.5f, 4.5f),
                new Vector2(-4.5f, 6.5f),
                new Vector2(4.5f, 6.5f),
                new Vector2(6.5f, -0.5f),
                new Vector2(-6.5f, -2.5f),
                new Vector2(12.5f, 4.5f)
            };

            for (int i = 0; i < branchPositions.Length; i++)
            {
                CreateResourceNode(group.transform, "DryBranch_" + (i + 1), branchPositions[i], branchSpr,
                    ResourceNodeType.DryBranch, "item_hatchet", 2, woodItem, 2, lootPrefab, true);
            }

            // 3 Cụm Cỏ Dại / Đá Vụn trong Nhà Kính (dọn cho Mở Đầu Step 7)
            Vector2[] debrisPositions =
            {
                new Vector2(-18.5f, 9.5f),
                new Vector2(-18.5f, 7.5f),
                new Vector2(-12.5f, 7.5f)
            };

            for (int i = 0; i < debrisPositions.Length; i++)
            {
                CreateResourceNode(group.transform, "GreenhouseDebris_" + (i + 1), debrisPositions[i], debrisSpr,
                    ResourceNodeType.GreenhouseDebris, "item_hoe", 1, woodItem, 1, lootPrefab, false);
            }
        }

        private static void CreateResourceNode(
            Transform parent, string name, Vector2 pos, Sprite spr,
            ResourceNodeType type, string toolId, int hits, ItemData dropItem, int dropAmt, GameObject lootPrefab, bool regrow)
        {
            GameObject go = new GameObject(name);
            go.tag = "ResourceNode";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.85f, 0.75f);

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(go.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);

            var comp = go.AddComponent<ResourceNodeController>();
            var so = new SerializedObject(comp);
            so.FindProperty("nodeType").enumValueIndex = (int)type;
            so.FindProperty("requiredToolId").stringValue = toolId;
            so.FindProperty("maxHits").intValue = hits;
            so.FindProperty("dropItem").objectReferenceValue = dropItem;
            so.FindProperty("dropAmount").intValue = dropAmt;
            so.FindProperty("lootDropPrefab").objectReferenceValue = lootPrefab;
            so.FindProperty("regrowEachDay").boolValue = regrow;
            so.FindProperty("visualRenderer").objectReferenceValue = sr;
            so.FindProperty("nodeCollider").objectReferenceValue = col;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreateWorldPrompt(Transform parent, string label, TMP_FontAsset font)
        {
            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(parent, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 50;

            promptGo.GetComponent<RectTransform>().sizeDelta = new Vector2(260f, 44f);
            promptGo.AddComponent<Image>().color = new Color(0.08f, 0.12f, 0.18f, 0.88f);

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(promptGo.transform, false);
            var tRect = txtGo.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero; tRect.anchorMax = Vector2.one;
            tRect.offsetMin = Vector2.zero; tRect.offsetMax = Vector2.zero;
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label; tmp.fontSize = 21; tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.94f, 0.65f, 1f);
            if (font != null) tmp.font = font;
            return promptGo;
        }

        // Procedural 16x16 Pixel Art Sprites for Step 6
        private static void DrawUntilledSoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.46f, 0.34f, 0.22f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 4, 2, 2, new Color(0.38f, 0.28f, 0.18f));
            PlaceholderGeneratorUtility.FillRect(t, 10, 10, 2, 2, new Color(0.52f, 0.40f, 0.26f));
        }

        private static void DrawTilledDrySoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.40f, 0.26f, 0.14f));
            Color furrow = new Color(0.30f, 0.19f, 0.10f);
            PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 7, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 11, 12, 2, furrow);
        }

        private static void DrawTilledWetSoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.24f, 0.16f, 0.10f));
            Color furrow = new Color(0.16f, 0.10f, 0.06f);
            PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 7, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 11, 12, 2, furrow);
        }

        private static void DrawBarleyStage0(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 5, 4, 2, 2, new Color(0.76f, 0.86f, 0.45f));
            PlaceholderGeneratorUtility.FillRect(t, 9, 6, 2, 2, new Color(0.76f, 0.86f, 0.45f));
        }

        private static void DrawBarleyStage1(Texture2D t)
        {
            Color g = new Color(0.34f, 0.78f, 0.36f);
            PlaceholderGeneratorUtility.FillRect(t, 7, 2, 2, 6, g);
            PlaceholderGeneratorUtility.FillRect(t, 5, 6, 6, 2, g);
        }

        private static void DrawBarleyStage2(Texture2D t)
        {
            Color g = new Color(0.30f, 0.76f, 0.40f);
            Color tip = new Color(0.68f, 0.88f, 0.44f);
            PlaceholderGeneratorUtility.FillRect(t, 5, 2, 2, 9, g);
            PlaceholderGeneratorUtility.FillRect(t, 9, 2, 2, 10, g);
            PlaceholderGeneratorUtility.FillRect(t, 4, 10, 8, 3, tip);
        }

        private static void DrawBarleyStage3(Texture2D t)
        {
            Color stem = new Color(0.78f, 0.64f, 0.24f);
            Color gold = new Color(0.98f, 0.86f, 0.34f);
            Color dew = new Color(0.68f, 0.96f, 1.0f);
            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 2, 10, stem);
            PlaceholderGeneratorUtility.FillRect(t, 7, 2, 2, 11, stem);
            PlaceholderGeneratorUtility.FillRect(t, 10, 2, 2, 10, stem);
            PlaceholderGeneratorUtility.FillRect(t, 3, 9, 10, 5, gold);
            PlaceholderGeneratorUtility.FillRect(t, 5, 13, 2, 2, dew);
            PlaceholderGeneratorUtility.FillRect(t, 10, 12, 2, 2, dew);
        }

        private static void DrawWellSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 2, 12, 12, new Color(0.54f, 0.56f, 0.60f));
            PlaceholderGeneratorUtility.FillRect(t, 4, 4, 8, 8, new Color(0.20f, 0.56f, 0.86f));
            PlaceholderGeneratorUtility.FillRect(t, 5, 8, 4, 2, new Color(0.66f, 0.92f, 1f));
        }

        private static void DrawShelfSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 2, 14, 12, new Color(0.52f, 0.34f, 0.18f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 8, 4, 4, new Color(0.86f, 0.72f, 0.38f));
            PlaceholderGeneratorUtility.FillRect(t, 9, 8, 4, 4, new Color(0.52f, 0.84f, 0.44f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 3, new Color(0.72f, 0.56f, 0.32f));
        }

        private static void DrawBranchSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 6, 12, 4, new Color(0.46f, 0.28f, 0.14f));
            PlaceholderGeneratorUtility.FillRect(t, 6, 9, 3, 4, new Color(0.56f, 0.36f, 0.20f));
            PlaceholderGeneratorUtility.FillRect(t, 10, 4, 3, 3, new Color(0.38f, 0.22f, 0.10f));
        }

        private static void DrawDebrisSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 8, new Color(0.42f, 0.46f, 0.44f));
            PlaceholderGeneratorUtility.FillRect(t, 5, 8, 6, 5, new Color(0.28f, 0.48f, 0.26f));
            PlaceholderGeneratorUtility.FillRect(t, 4, 5, 4, 4, new Color(0.54f, 0.56f, 0.58f));
        }
    }
}
