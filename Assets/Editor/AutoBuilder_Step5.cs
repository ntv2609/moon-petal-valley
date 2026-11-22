using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Inventory;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step5
    {
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string ItemDatabasePath = "Assets/Data/Config/ItemDatabase.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string LootDropPrefabPath = "Assets/Prefabs/Enemies/LootDrop.prefab";

        [MenuItem("Moonpetal/Step 5/1. Build Inventory, Hotbar & 15 Items")]
        public static void BuildStep5()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 5 ===");

            ItemDatabaseSO db = BuildAll15MVPItems();
            BuildLootDropPrefab();
            UpgradeGameRootWithInventoryAndUI(db);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 5! Chạy 'Moonpetal/Step 5/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 5/Validate")]
        public static void ValidateStep5()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 5 ===");

            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            if (db == null || db.allItems.Count < 15)
            {
                Debug.LogError("[MV] ItemDatabase thiếu vật phẩm (cần 15, hiện có " + (db == null ? 0 : db.allItems.Count) + ").");
                errors++;
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath) == null)
            {
                Debug.LogError("[MV] THIẾU LootDrop.prefab");
                errors++;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                if (rootSO.FindProperty("inventoryManager").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu inventoryManager.");
                    errors++;
                }
                if (prefab.GetComponentInChildren<HotbarUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu HotbarUIView trong GameRoot.prefab.");
                    errors++;
                }
                if (prefab.GetComponentInChildren<InventoryUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu InventoryUIView trong GameRoot.prefab.");
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 5: PASS (0 lỗi). 15 Vật phẩm MVP, Hotbar 8 ô & Túi Đồ 16 ô sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 5: FAIL (" + errors + " lỗi).");
            }
        }

        private static ItemDatabaseSO BuildAll15MVPItems()
        {
            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<ItemDatabaseSO>();
                AssetDatabase.CreateAsset(db, ItemDatabasePath);
            }
            db.allItems.Clear();

            AddOrUpdateItem(db, table, "item_hoe", "Cuốc Đất", "Dùng xới đất trồng cây trong Nhà Kính và đập đá vụn nhỏ (4 Thể lực).",
                ItemCategory.Tool, 1, 0, 0, false, 0, true, new Color(0.55f, 0.36f, 0.20f), new Color(0.74f, 0.78f, 0.84f), 0);

            AddOrUpdateItem(db, table, "item_watering_can", "Bình Tưới", "Tưới nước cho đất trồng (2 Thể lực, chứa 20 lần). Múc đầy lại tại Giếng Nước.",
                ItemCategory.Tool, 1, 0, 0, false, 0, true, new Color(0.26f, 0.56f, 0.84f), new Color(0.65f, 0.88f, 1f), 1);

            AddOrUpdateItem(db, table, "item_hatchet", "Rìu Nhỏ", "Chặt nhánh cây khô lấy Gỗ Nhánh Cây và dọn bụi gai (4 Thể lực).",
                ItemCategory.Tool, 1, 0, 0, false, 0, true, new Color(0.52f, 0.34f, 0.18f), new Color(0.82f, 0.85f, 0.90f), 2);

            AddOrUpdateItem(db, table, "item_moon_sickle", "Liềm Ánh Trăng", "Vũ khí thanh tẩy của Ông Bách: chém quét 120° gây 15 sát thương (2 Thể lực).",
                ItemCategory.Tool, 1, 0, 0, false, 0, true, new Color(0.32f, 0.24f, 0.48f), new Color(0.65f, 0.96f, 0.98f), 3);

            AddOrUpdateItem(db, table, "item_dew_barley_seed", "Hạt Lúa Mạch Sương Mai", "Gieo xuống ô đất đã cuốc và tưới nước 2 ngày để thu hoạch.",
                ItemCategory.Seed, 99, 0, 0, false, 2, true, new Color(0.62f, 0.46f, 0.28f), new Color(0.86f, 0.95f, 0.62f), 4);

            AddOrUpdateItem(db, table, "item_dew_barley", "Lúa Mạch Sương Mai", "Nông sản thơm dịu đọng sương sớm, nguyên liệu làm bánh của Bà Hana.",
                ItemCategory.Crop, 99, 0, 0, false, 0, true, new Color(0.92f, 0.78f, 0.30f), new Color(0.70f, 0.96f, 0.95f), 5);

            AddOrUpdateItem(db, table, "item_sunshine_bread", "Bánh Mì Nắng Ấm", "Hồi 50 Thể lực, 50 Máu Ánh Sáng và XÓA NGAY phạt kiệt sức 50%.",
                ItemCategory.Consumable, 99, 50, 50, true, 0, true, new Color(0.84f, 0.56f, 0.24f), new Color(0.98f, 0.84f, 0.52f), 6);

            AddOrUpdateItem(db, table, "item_branch_wood", "Gỗ Nhánh Cây", "Gỗ khô nhặt quanh làng, dùng sửa hòm thư cho Bác Otis.",
                ItemCategory.Material, 99, 0, 0, false, 0, false, new Color(0.48f, 0.30f, 0.16f), new Color(0.66f, 0.46f, 0.28f), 7);

            AddOrUpdateItem(db, table, "item_dew_sap", "Nhựa Sương", "Giọt nhựa ánh sáng rơi ra khi thanh tẩy Bọt Sương Đen.",
                ItemCategory.Material, 99, 0, 0, false, 0, false, new Color(0.32f, 0.72f, 0.92f), new Color(0.80f, 0.96f, 1f), 8);

            AddOrUpdateItem(db, table, "item_starlight_dust", "Bụi Tinh Quang", "Bụi lấp lánh giúp hồi sinh Bồn Hoa Cộng Đồng.",
                ItemCategory.Material, 99, 0, 0, false, 0, false, new Color(0.96f, 0.84f, 0.34f), new Color(1f, 0.98f, 0.78f), 9);

            AddOrUpdateItem(db, table, "item_moonpetal_seed_core", "Viên Pha Lê Hạt Giống", "Hạt giống di sản của bà, cộng hưởng cùng dân làng để hoa Moonpetal nở.",
                ItemCategory.KeyItem, 1, 0, 0, false, 0, true, new Color(0.45f, 0.88f, 0.96f), new Color(1f, 1f, 1f), 10);

            AddOrUpdateItem(db, table, "item_seed_bag_1", "Túi Hạt Di Sản 1 (Hana)", "Túi hạt giống kỷ niệm của Bà Hana để đóng góp vào Bồn Hoa Vườn Xưa.",
                ItemCategory.SeedBag, 1, 0, 0, false, 0, true, new Color(0.82f, 0.64f, 0.38f), new Color(0.96f, 0.46f, 0.42f), 11);

            AddOrUpdateItem(db, table, "item_seed_bag_2", "Túi Hạt Di Sản 2 (Toby)", "Túi hạt giống Toby gìn giữ sau khi chú mèo Miu được giải cứu.",
                ItemCategory.SeedBag, 1, 0, 0, false, 0, true, new Color(0.82f, 0.64f, 0.38f), new Color(0.42f, 0.84f, 0.56f), 11);

            AddOrUpdateItem(db, table, "item_seed_bag_3", "Túi Hạt Di Sản 3 (Otis)", "Túi hạt giống nằm trong hòm thư cũ được Bác Otis trao lại.",
                ItemCategory.SeedBag, 1, 0, 0, false, 0, true, new Color(0.82f, 0.64f, 0.38f), new Color(0.42f, 0.68f, 0.96f), 11);

            AddOrUpdateItem(db, table, "item_petal_of_hope", "Cánh Hoa Hy Vọng", "Cánh hoa Moonpetal đầu tiên xua tan Sương Héo Úa khỏi Làng Chính.",
                ItemCategory.MoonpetalShard, 1, 0, 0, false, 0, true, new Color(0.98f, 0.70f, 0.86f), new Color(1f, 0.96f, 0.98f), 12);

            if (table != null) EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(db);
            return db;
        }

        private static void AddOrUpdateItem(
            ItemDatabaseSO db, StringTableSO table, string id, string vnName, string vnDesc,
            ItemCategory cat, int maxStack, int stam, int hp, bool clearExh, int growDays, bool questProtected,
            Color primary, Color accent, int shapeType)
        {
            string iconPath = "Assets/Art/Sprites/Items/icon_" + id + ".png";
            Sprite icon = PlaceholderGeneratorUtility.EnsureSprite(iconPath, 16, 16, tex => DrawItemIcon(tex, primary, accent, shapeType));

            string assetPath = "Assets/Data/Items/Item_" + id + ".asset";
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemData>();
                AssetDatabase.CreateAsset(item, assetPath);
            }

            item.itemID = id;
            item.displayNameKey = "name_" + id;
            item.descriptionKey = "desc_" + id;
            item.icon = icon;
            item.category = cat;
            item.maxStack = maxStack;
            item.staminaRestore = stam;
            item.lightHPRestore = hp;
            item.clearsExhaustionPenalty = clearExh;
            item.growDays = growDays;
            item.isQuestProtected = questProtected;

            if (table != null)
            {
                table.SetEntry(item.displayNameKey, vnName);
                table.SetEntry(item.descriptionKey, vnDesc);
            }

            EditorUtility.SetDirty(item);
            db.allItems.Add(item);
        }

        private static void DrawItemIcon(Texture2D tex, Color primary, Color accent, int shape)
        {
            if (shape == 0) // Cuốc
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 2, 2, 11, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 11, 7, 3, accent);
            }
            else if (shape == 1) // Bình tưới
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 3, 7, 7, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 11, 6, 3, 2, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 10, 5, 2, accent);
            }
            else if (shape == 2) // Rìu
            {
                PlaceholderGeneratorUtility.FillRect(tex, 5, 2, 2, 12, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 7, 9, 5, 5, accent);
            }
            else if (shape == 3) // Liềm Ánh Trăng
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 2, 2, 8, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 9, 7, 3, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 10, 6, 3, 4, accent);
            }
            else if (shape == 6) // Bánh Mì Nắng Ấm
            {
                PlaceholderGeneratorUtility.FillRect(tex, 2, 4, 12, 7, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 4, 9, 8, 3, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 6, 2, 2, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 9, 6, 2, 2, accent);
            }
            else if (shape == 7) // Gỗ Nhánh Cây
            {
                PlaceholderGeneratorUtility.FillRect(tex, 2, 6, 12, 4, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 4, 8, 8, 2, accent);
            }
            else if (shape == 12) // Cánh Hoa Hy Vọng
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 3, 8, 10, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 6, 5, 4, 6, accent);
            }
            else // Hạt, Túi hạt, Giọt sương, Bụi, Pha lê
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 3, 8, 10, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 5, 6, 5, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 6, 12, 4, 2, accent);
            }
        }

        private static void BuildLootDropPrefab()
        {
            GameObject go = new GameObject("LootDrop");
            var drop = go.AddComponent<WorldLootDrop>();

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(go.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = 200;

            var so = new SerializedObject(drop);
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(go, LootDropPrefabPath);
            UnityEngine.Object.DestroyImmediate(go);
        }

        private static void UpgradeGameRootWithInventoryAndUI(ItemDatabaseSO db)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var invMgr = root.GetComponent<InventoryManager>();
            if (invMgr == null) invMgr = root.AddComponent<InventoryManager>();

            var invSO = new SerializedObject(invMgr);
            invSO.FindProperty("itemDatabase").objectReferenceValue = db;
            invSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("inventoryManager").objectReferenceValue = invMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                if (hudCanvas.GetComponent<GraphicRaycaster>() == null)
                {
                    hudCanvas.gameObject.AddComponent<GraphicRaycaster>();
                }

                // 1. Hotbar 8 ô ở đáy màn hình
                Transform oldHotbar = hudCanvas.Find("HotbarPanel");
                if (oldHotbar != null) UnityEngine.Object.DestroyImmediate(oldHotbar.gameObject);

                GameObject hotbarGo = new GameObject("HotbarPanel");
                hotbarGo.transform.SetParent(hudCanvas, false);
                var hbRect = hotbarGo.AddComponent<RectTransform>();
                hbRect.anchorMin = new Vector2(0.5f, 0f);
                hbRect.anchorMax = new Vector2(0.5f, 0f);
                hbRect.pivot = new Vector2(0.5f, 0f);
                hbRect.anchoredPosition = new Vector2(0f, 18f);
                hbRect.sizeDelta = new Vector2(640f, 82f);

                var bgs = new Image[8];
                var icons = new Image[8];
                var counts = new TMP_Text[8];

                for (int i = 0; i < 8; i++)
                {
                    GameObject slotGo = new GameObject("Slot_" + (i + 1));
                    slotGo.transform.SetParent(hotbarGo.transform, false);
                    var sRect = slotGo.AddComponent<RectTransform>();
                    sRect.anchorMin = new Vector2(0f, 0.5f);
                    sRect.anchorMax = new Vector2(0f, 0.5f);
                    sRect.anchoredPosition = new Vector2(44f + i * 78f, 0f);
                    sRect.sizeDelta = new Vector2(70f, 70f);
                    bgs[i] = slotGo.AddComponent<Image>();
                    bgs[i].color = new Color(0.11f, 0.15f, 0.22f, 0.88f);

                    icons[i] = CreateSlotIcon(slotGo.transform);
                    counts[i] = CreateCornerText(slotGo.transform, "Count", "", vnFont, 15, TextAlignmentOptions.BottomRight, Color.white);
                    CreateCornerText(slotGo.transform, "Num", (i + 1).ToString(), vnFont, 13, TextAlignmentOptions.TopLeft, new Color(0.8f, 0.86f, 0.92f, 0.85f));
                }

                // Nhãn tên vật phẩm đang chọn phía trên Hotbar
                GameObject labelGo = new GameObject("SelectedItemLabel");
                labelGo.transform.SetParent(hotbarGo.transform, false);
                var lRect = labelGo.AddComponent<RectTransform>();
                lRect.anchorMin = new Vector2(0.5f, 1f);
                lRect.anchorMax = new Vector2(0.5f, 1f);
                lRect.pivot = new Vector2(0.5f, 0f);
                lRect.anchoredPosition = new Vector2(0f, 6f);
                lRect.sizeDelta = new Vector2(640f, 30f);
                var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
                labelTmp.fontSize = 19;
                labelTmp.alignment = TextAlignmentOptions.Center;
                labelTmp.color = new Color(1f, 0.96f, 0.78f, 1f);
                if (vnFont != null) labelTmp.font = vnFont;

                var hbView = hotbarGo.AddComponent<HotbarUIView>();
                var hbSO = new SerializedObject(hbView);
                AssignArray(hbSO.FindProperty("slotBackgrounds"), bgs);
                AssignArray(hbSO.FindProperty("slotIcons"), icons);
                AssignArray(hbSO.FindProperty("slotCounts"), counts);
                hbSO.FindProperty("selectedItemLabel").objectReferenceValue = labelTmp;
                hbSO.ApplyModifiedPropertiesWithoutUndo();

                // 2. Modal Túi Đồ 16 ô (Tab / I)
                Transform oldInv = hudCanvas.Find("InventoryModalHost");
                if (oldInv != null) UnityEngine.Object.DestroyImmediate(oldInv.gameObject);

                GameObject invHost = new GameObject("InventoryModalHost");
                invHost.transform.SetParent(hudCanvas, false);
                var hostRect = invHost.AddComponent<RectTransform>();
                hostRect.anchorMin = Vector2.zero;
                hostRect.anchorMax = Vector2.one;
                hostRect.offsetMin = Vector2.zero;
                hostRect.offsetMax = Vector2.zero;

                GameObject modal = new GameObject("InventoryPanelRoot");
                modal.transform.SetParent(invHost.transform, false);
                var mRect = modal.AddComponent<RectTransform>();
                mRect.anchorMin = new Vector2(0.5f, 0.5f);
                mRect.anchorMax = new Vector2(0.5f, 0.5f);
                mRect.sizeDelta = new Vector2(620f, 440f);
                var mImg = modal.AddComponent<Image>();
                mImg.color = new Color(0.08f, 0.11f, 0.18f, 0.95f);

                var titleTmp = CreateCornerText(modal.transform, "Title", "TÚI ĐỒ CỦA WEN (Tab / Esc để đóng)", vnFont, 22, TextAlignmentOptions.Top, new Color(1f, 0.92f, 0.65f, 1f));
                titleTmp.GetComponent<RectTransform>().offsetMax = new Vector2(-12f, -12f);

                var invBtns = new Button[16];
                var invIcons = new Image[16];
                var invCounts = new TMP_Text[16];

                for (int i = 0; i < 16; i++)
                {
                    int row = i / 8;
                    int col = i % 8;
                    GameObject sGo = new GameObject("BagSlot_" + i);
                    sGo.transform.SetParent(modal.transform, false);
                    var r = sGo.AddComponent<RectTransform>();
                    r.anchorMin = new Vector2(0f, 1f);
                    r.anchorMax = new Vector2(0f, 1f);
                    r.anchoredPosition = new Vector2(58f + col * 72f, -92f - row * 76f);
                    r.sizeDelta = new Vector2(64f, 64f);
                    var sImg = sGo.AddComponent<Image>();
                    sImg.color = row == 0 ? new Color(0.18f, 0.24f, 0.34f, 1f) : new Color(0.14f, 0.17f, 0.24f, 1f);
                    invBtns[i] = sGo.AddComponent<Button>();
                    invBtns[i].targetGraphic = sImg;
                    invIcons[i] = CreateSlotIcon(sGo.transform);
                    invCounts[i] = CreateCornerText(sGo.transform, "Count", "", vnFont, 14, TextAlignmentOptions.BottomRight, Color.white);
                }

                var dName = CreateBoxText(modal.transform, "DetailName", new Vector2(0f, -230f), new Vector2(560f, 34f), vnFont, 21, new Color(1f, 0.94f, 0.68f, 1f));
                var dDesc = CreateBoxText(modal.transform, "DetailDesc", new Vector2(0f, -270f), new Vector2(560f, 100f), vnFont, 18, Color.white);

                var invView = invHost.AddComponent<InventoryUIView>();
                var ivSO = new SerializedObject(invView);
                ivSO.FindProperty("panelRoot").objectReferenceValue = modal;
                AssignArray(ivSO.FindProperty("slotButtons"), invBtns);
                AssignArray(ivSO.FindProperty("slotIcons"), invIcons);
                AssignArray(ivSO.FindProperty("slotCounts"), invCounts);
                ivSO.FindProperty("detailNameText").objectReferenceValue = dName;
                ivSO.FindProperty("detailDescText").objectReferenceValue = dDesc;
                ivSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // 3. Thêm nút Debug cho bộ test túi đồ
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 835f);
                Button btnKit = EnsureDebugButton(debugPanel, "BtnGiveTestKit", "Cho Bộ Test Đồ (Bánh Mì + 10 Gỗ + Túi Hạt)", vnFont, new Color(0.38f, 0.24f, 0.52f, 1f));
                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnGiveTestKit").objectReferenceValue = btnKit;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static Image CreateSlotIcon(Transform parent)
        {
            GameObject go = new GameObject("Icon");
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(8f, 8f);
            r.offsetMax = new Vector2(-8f, -8f);
            var img = go.AddComponent<Image>();
            img.preserveAspect = true;
            img.color = new Color(1f, 1f, 1f, 0f);
            return img;
        }

        private static TMP_Text CreateCornerText(Transform parent, string name, string text, TMP_FontAsset font, float size, TextAlignmentOptions align, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(4f, 2f);
            r.offsetMax = new Vector2(-4f, -2f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.color = color;
            tmp.raycastTarget = false;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static TMP_Text CreateBoxText(Transform parent, string name, Vector2 pos, Vector2 size, TMP_FontAsset font, float fontSize, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 1f);
            r.anchorMax = new Vector2(0.5f, 1f);
            r.pivot = new Vector2(0.5f, 1f);
            r.anchoredPosition = pos;
            r.sizeDelta = size;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static void AssignArray<T>(SerializedProperty prop, T[] items) where T : Object
        {
            prop.ClearArray();
            for (int i = 0; i < items.Length; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
            }
        }

        private static Button EnsureDebugButton(Transform parent, string name, string label, TMP_FontAsset font, Color bgColor)
        {
            Transform existing = parent.Find(name);
            if (existing != null) return existing.GetComponent<Button>();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 36f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var tmp = CreateCornerText(go.transform, "Label", label, font, 15, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }
    }
}
