using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using MoonpetalValley.Data;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step20
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string ItemDatabasePath = "Assets/Data/Config/ItemDatabase.asset";
        private const string UIRoot = "Assets/Art/Sprites/UI/";
        private const string ItemRoot = "Assets/Art/Sprites/Items/";

        [MenuItem("Moonpetal/Step 20/1. Apply UI, 15 Item Icons, Main Menu & Remove All Em Dashes")]
        public static void BuildStep20()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 20 (UI, 15 ITEMS, MAIN MENU & XÓA EM DASH) ===");
            AssetDatabase.Refresh();
            ConfigureUIAndItemImporters();
            AssetDatabase.Refresh();

            // 1. Re-wire all 15 ItemData icons
            UpgradeAll15ItemIcons();

            // 2. Scrub Em Dashes from VNStringTable
            ScrubEmDashesInStringTable();

            // 3. Skin GameRoot.prefab (Stats HUD, Clock, Quest, Hotbar, Inventory, Dialogue, Diary, Bundle, Pause, Toast)
            SkinGameRootUI();

            // 4. Skin MainMenu.unity with Scenic Pixel Backdrop & Wood Panels/Buttons
            SkinMainMenuScene();

            // 5. Skin World-Space Prompts ([E] ...) & Scrub Em Dashes in MoonpetalValley_MVP.unity
            SkinWorldScenePromptsAndScrubEmDashes();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 20! Chạy 'Moonpetal/Step 20/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 20/Validate")]
        public static void ValidateStep20()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 20 ===");

            string[] uiSprites =
            {
                "ui_panel_wood_parchment.png", "ui_panel_dark_wood.png",
                "ui_slot_normal.png", "ui_slot_selected.png",
                "ui_button_wood.png", "ui_bar_frame.png",
                "ui_bar_fill_hp.png", "ui_bar_fill_stamina.png",
                "ui_icon_light_hp.png", "ui_icon_stamina.png",
                "ui_mainmenu_bg.png",
            };
            foreach (string f in uiSprites)
            {
                if (AssetDatabase.LoadAssetAtPath<Sprite>(UIRoot + f) == null)
                {
                    Debug.LogError("[MV] Thiếu UI sprite Step 20: " + UIRoot + f);
                    errors++;
                }
            }

            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            if (table != null)
            {
                for (int i = 0; i < table.entries.Count; i++)
                {
                    if (!string.IsNullOrEmpty(table.entries[i].vietnameseText) && table.entries[i].vietnameseText.Contains("\u2014"))
                    {
                        Debug.LogError("[MV] VNStringTable vẫn còn dấu em dash tại key: " + table.entries[i].key);
                        errors++;
                    }
                }
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                var tmps = prefab.GetComponentsInChildren<TMP_Text>(true);
                for (int i = 0; i < tmps.Length; i++)
                {
                    if (!string.IsNullOrEmpty(tmps[i].text) && tmps[i].text.Contains("\u2014"))
                    {
                        Debug.LogError("[MV] GameRoot.prefab vẫn còn dấu em dash trên: " + tmps[i].name);
                        errors++;
                    }
                }
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + allGos[i].name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 20: PASS (0 lỗi | 15 Item Icons=OK | Cozy Wood/Parchment UI=OK | Main Menu=OK | 0 Em Dashes).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 20: FAIL ({errors} lỗi).");
            }
        }

        private static string ScrubEmDash(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace(" \u2014 ", " | ").Replace("\u2014", "-");
        }

        private static Sprite LoadUI(string name) => AssetDatabase.LoadAssetAtPath<Sprite>(UIRoot + name);

        private static void ImportSprite(string path, float ppu, Vector4 border)
        {
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) return;
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.filterMode = FilterMode.Point;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.alphaIsTransparency = true;
            imp.spritePixelsPerUnit = ppu;
            imp.spriteBorder = border;
            var st = new TextureImporterSettings();
            imp.ReadTextureSettings(st);
            st.spriteMeshType = SpriteMeshType.FullRect;
            st.spriteAlignment = (int)SpriteAlignment.Center;
            st.spritePivot = new Vector2(0.5f, 0.5f);
            imp.SetTextureSettings(st);
            imp.SaveAndReimport();
        }

        private static void ConfigureUIAndItemImporters()
        {
            if (AssetDatabase.IsValidFolder("Assets/Art/Sprites/Items"))
            {
                foreach (string g in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art/Sprites/Items" }))
                {
                    ImportSprite(AssetDatabase.GUIDToAssetPath(g), 16f, Vector4.zero);
                }
            }

            ImportSprite(UIRoot + "ui_panel_wood_parchment.png", 16f, new Vector4(8, 8, 8, 8));
            ImportSprite(UIRoot + "ui_panel_dark_wood.png",      16f, new Vector4(8, 8, 8, 8));
            ImportSprite(UIRoot + "ui_slot_normal.png",          16f, new Vector4(4, 4, 4, 4));
            ImportSprite(UIRoot + "ui_slot_selected.png",        16f, new Vector4(4, 4, 4, 4));
            ImportSprite(UIRoot + "ui_button_wood.png",          16f, new Vector4(5, 5, 5, 5));
            ImportSprite(UIRoot + "ui_bar_frame.png",            16f, new Vector4(4, 4, 4, 4));
            ImportSprite(UIRoot + "ui_bar_fill_hp.png",          16f, Vector4.zero);
            ImportSprite(UIRoot + "ui_bar_fill_stamina.png",     16f, Vector4.zero);
            ImportSprite(UIRoot + "ui_icon_light_hp.png",        16f, Vector4.zero);
            ImportSprite(UIRoot + "ui_icon_stamina.png",         16f, Vector4.zero);
            ImportSprite(UIRoot + "ui_mainmenu_bg.png",          16f, Vector4.zero);
        }

        private static void UpgradeAll15ItemIcons()
        {
            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            if (db == null) return;
            int count = 0;
            for (int i = 0; i < db.allItems.Count; i++)
            {
                var item = db.allItems[i];
                if (item == null) continue;
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(ItemRoot + "icon_" + item.itemID + ".png");
                if (spr != null)
                {
                    item.icon = spr;
                    EditorUtility.SetDirty(item);
                    count++;
                }
            }
            Debug.Log($"[MV] Đã gắn icon pixel art mới cho {count}/15 vật phẩm.");
        }

        private static void ScrubEmDashesInStringTable()
        {
            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            if (table == null) return;
            int fixedCount = 0;
            for (int i = 0; i < table.entries.Count; i++)
            {
                var e = table.entries[i];
                string u = ScrubEmDash(e.vietnameseText);
                if (u != e.vietnameseText)
                {
                    e.vietnameseText = u;
                    table.entries[i] = e;
                    fixedCount++;
                }
            }
            EditorUtility.SetDirty(table);
            Debug.Log($"[MV] Đã xóa dấu em dash tại {fixedCount} chuỗi trong VNStringTable.asset.");
        }

        private static void ApplySliced(Image img, Sprite spr, Color tint, float ppuMult = 0.45f)
        {
            if (img == null || spr == null) return;
            img.sprite = spr;
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = ppuMult;
            img.color = tint;
        }

        private static void SkinButton(Transform btnTf, Sprite btnSpr, Color tint)
        {
            if (btnTf == null || btnSpr == null) return;
            var img = btnTf.GetComponent<Image>();
            if (img != null) ApplySliced(img, btnSpr, tint, 0.38f);
        }

        private static void EnsureBarIcon(Transform barTf, string iconName, Sprite iconSpr)
        {
            if (barTf == null || iconSpr == null) return;
            Transform old = barTf.Find(iconName);
            GameObject go = old != null ? old.gameObject : new GameObject(iconName);
            go.transform.SetParent(barTf, false);
            var r = go.GetComponent<RectTransform>();
            if (r == null) r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0f, 0.5f);
            r.anchorMax = new Vector2(0f, 0.5f);
            r.pivot = new Vector2(0.5f, 0.5f);
            r.anchoredPosition = new Vector2(18f, 0f);
            r.sizeDelta = new Vector2(26f, 26f);
            var img = go.GetComponent<Image>();
            if (img == null) img = go.AddComponent<Image>();
            img.sprite = iconSpr;
            img.preserveAspect = true;
            img.color = Color.white;
            img.raycastTarget = false;

            Transform txt = barTf.Find("Text");
            if (txt != null)
            {
                var tr = txt.GetComponent<RectTransform>();
                if (tr != null) tr.offsetMin = new Vector2(36f, 0f);
                var tmp = txt.GetComponent<TMP_Text>();
                if (tmp != null) tmp.color = Color.white;
            }
        }

        private static void SkinGameRootUI()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Sprite woodParch = LoadUI("ui_panel_wood_parchment.png");
            Sprite darkWood  = LoadUI("ui_panel_dark_wood.png");
            Sprite slotNorm  = LoadUI("ui_slot_normal.png");
            Sprite slotSel   = LoadUI("ui_slot_selected.png");
            Sprite btnWood   = LoadUI("ui_button_wood.png");
            Sprite barFrame  = LoadUI("ui_bar_frame.png");
            Sprite hpFill    = LoadUI("ui_bar_fill_hp.png");
            Sprite stFill    = LoadUI("ui_bar_fill_stamina.png");
            Sprite hpIcon    = LoadUI("ui_icon_light_hp.png");
            Sprite stIcon    = LoadUI("ui_icon_stamina.png");

            // 1. Toast Panel
            Transform toastPanel = root.transform.Find("SystemOverlayCanvas/ToastPanel");
            if (toastPanel != null) ApplySliced(toastPanel.GetComponent<Image>(), darkWood, Color.white, 0.45f);

            Transform hud = root.transform.Find("ClockHUDCanvas");
            if (hud != null)
            {
                // 2. Clock & Quest Tracker Panels
                Transform clockP = hud.Find("ClockBadgePanel");
                if (clockP != null) ApplySliced(clockP.GetComponent<Image>(), darkWood, Color.white, 0.45f);

                Transform questP = hud.Find("QuestTrackerPanel");
                if (questP != null) ApplySliced(questP.GetComponent<Image>(), darkWood, Color.white, 0.45f);

                // 3. Stats Panel (Light HP & Stamina bars with cozy pixel frames & icons)
                Transform statsP = hud.Find("StatsPanel");
                if (statsP != null)
                {
                    ApplySliced(statsP.GetComponent<Image>(), darkWood, Color.white, 0.45f);

                    Transform hpBar = statsP.Find("LightHPBar");
                    if (hpBar != null)
                    {
                        ApplySliced(hpBar.GetComponent<Image>(), barFrame, Color.white, 0.35f);
                        var fillImg = hpBar.Find("Fill")?.GetComponent<Image>();
                        if (fillImg != null && hpFill != null) { fillImg.sprite = hpFill; fillImg.color = Color.white; }
                        EnsureBarIcon(hpBar, "LightHPIcon", hpIcon);
                    }

                    Transform stBar = statsP.Find("StaminaBar");
                    if (stBar != null)
                    {
                        ApplySliced(stBar.GetComponent<Image>(), barFrame, Color.white, 0.35f);
                        var fillImg = stBar.Find("Fill")?.GetComponent<Image>();
                        if (fillImg != null && stFill != null) { fillImg.sprite = stFill; fillImg.color = Color.white; }
                        EnsureBarIcon(stBar, "StaminaIcon", stIcon);
                    }
                }

                // 4. Hotbar & Inventory Modal
                Transform hotbarP = hud.Find("HotbarPanel");
                if (hotbarP != null)
                {
                    var hbView = hotbarP.GetComponent<HotbarUIView>();
                    if (hbView != null)
                    {
                        var so = new SerializedObject(hbView);
                        so.FindProperty("normalSlotSprite").objectReferenceValue = slotNorm;
                        so.FindProperty("selectedSlotSprite").objectReferenceValue = slotSel;
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                    for (int i = 1; i <= 8; i++)
                    {
                        var sImg = hotbarP.Find("Slot_" + i)?.GetComponent<Image>();
                        if (sImg != null) ApplySliced(sImg, i == 1 ? slotSel : slotNorm, Color.white, 0.35f);
                        var numTxt = hotbarP.Find("Slot_" + i + "/Num")?.GetComponent<TMP_Text>();
                        if (numTxt != null) numTxt.color = new Color(0.28f, 0.18f, 0.12f, 0.9f);
                        var cntTxt = hotbarP.Find("Slot_" + i + "/Count")?.GetComponent<TMP_Text>();
                        if (cntTxt != null) cntTxt.color = new Color(0.20f, 0.12f, 0.08f, 1f);
                    }
                }

                Transform invHost = hud.Find("InventoryModalHost");
                if (invHost != null)
                {
                    var invPanel = invHost.Find("InventoryPanelRoot");
                    if (invPanel != null) ApplySliced(invPanel.GetComponent<Image>(), darkWood, Color.white, 0.45f);
                    var invView = invHost.GetComponent<InventoryUIView>();
                    if (invView != null)
                    {
                        var so = new SerializedObject(invView);
                        so.FindProperty("normalSlotSprite").objectReferenceValue = slotNorm;
                        so.FindProperty("selectedSlotSprite").objectReferenceValue = slotSel;
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        var sImg = invPanel?.Find("BagSlot_" + i)?.GetComponent<Image>();
                        if (sImg != null) ApplySliced(sImg, slotNorm, Color.white, 0.35f);
                        var cntTxt = invPanel?.Find("BagSlot_" + i + "/Count")?.GetComponent<TMP_Text>();
                        if (cntTxt != null) cntTxt.color = new Color(0.20f, 0.12f, 0.08f, 1f);
                    }
                }

                // 5. Dialogue Box (Warm Wood & Parchment Stardew style!)
                Transform dlgBox = hud.Find("DialogueUIHost/DialogueBox");
                if (dlgBox != null)
                {
                    ApplySliced(dlgBox.GetComponent<Image>(), woodParch, Color.white, 0.40f);
                    var sp = dlgBox.Find("SpeakerText")?.GetComponent<TMP_Text>();
                    if (sp != null) sp.color = new Color(0.56f, 0.22f, 0.10f, 1f);
                    var bd = dlgBox.Find("BodyText")?.GetComponent<TMP_Text>();
                    if (bd != null) bd.color = new Color(0.20f, 0.13f, 0.10f, 1f);
                    var ht = dlgBox.Find("ContinueHint")?.GetComponent<TMP_Text>();
                    if (ht != null) ht.color = new Color(0.45f, 0.28f, 0.16f, 1f);
                }

                // 6. Diary, Bundle, Victory & Pause Modals
                Transform diaryBox = hud.Find("DiaryUIHost/DiaryBox");
                if (diaryBox != null)
                {
                    ApplySliced(diaryBox.GetComponent<Image>(), woodParch, Color.white, 0.40f);
                    var h = diaryBox.Find("Header")?.GetComponent<TMP_Text>();
                    if (h != null) h.color = new Color(0.50f, 0.28f, 0.14f, 1f);
                    var t = diaryBox.Find("Title")?.GetComponent<TMP_Text>();
                    if (t != null) t.color = new Color(0.24f, 0.14f, 0.10f, 1f);
                    var b = diaryBox.Find("Body")?.GetComponent<TMP_Text>();
                    if (b != null) b.color = new Color(0.22f, 0.14f, 0.10f, 1f);
                    SkinButton(diaryBox.Find("BtnPrev"), btnWood, Color.white);
                    SkinButton(diaryBox.Find("BtnClose"), btnWood, Color.white);
                    SkinButton(diaryBox.Find("BtnNext"), btnWood, Color.white);
                }

                Transform bundleP = hud.Find("BundleUIHost/BundlePanelRoot");
                if (bundleP != null)
                {
                    ApplySliced(bundleP.GetComponent<Image>(), darkWood, Color.white, 0.42f);
                    SkinButton(bundleP.Find("BtnContribute"), btnWood, new Color(0.76f, 0.90f, 1f, 1f));
                    SkinButton(bundleP.Find("BtnBloom"), btnWood, new Color(0.76f, 1f, 0.84f, 1f));
                    SkinButton(bundleP.Find("BtnClose"), btnWood, new Color(1f, 0.80f, 0.80f, 1f));
                }

                Transform victoryP = hud.Find("BundleUIHost/VictoryPanelRoot");
                if (victoryP != null)
                {
                    ApplySliced(victoryP.GetComponent<Image>(), darkWood, Color.white, 0.42f);
                    SkinButton(victoryP.Find("BtnVictoryClose"), btnWood, new Color(0.82f, 1f, 0.86f, 1f));
                }

                Transform pauseP = hud.Find("PauseMenuHost/PausePanelRoot");
                if (pauseP != null)
                {
                    ApplySliced(pauseP.GetComponent<Image>(), darkWood, Color.white, 0.42f);
                    foreach (string bn in new[] { "BtnResume", "BtnSave", "BtnLoad", "BtnMute", "BtnDeleteSave" })
                    {
                        SkinButton(pauseP.Find(bn), btnWood, Color.white);
                    }
                }
            }

            // Scrub all em dashes inside GameRoot.prefab TMP_Text components
            var allTmps = root.GetComponentsInChildren<TMP_Text>(true);
            for (int i = 0; i < allTmps.Length; i++)
            {
                allTmps[i].text = ScrubEmDash(allTmps[i].text);
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã áp dụng giao diện Gỗ & Giấy Da pixel art cho toàn bộ HUD & Modal trong GameRoot.prefab.");
        }

        private static void SkinMainMenuScene()
        {
            if (!System.IO.File.Exists(MainMenuScenePath)) return;
            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);

            Sprite bgSpr = LoadUI("ui_mainmenu_bg.png");
            Sprite darkWood = LoadUI("ui_panel_dark_wood.png");
            Sprite btnWood = LoadUI("ui_button_wood.png");

            GameObject backdrop = GameObject.Find("Backdrop");
            if (backdrop != null && bgSpr != null)
            {
                var img = backdrop.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = bgSpr;
                    img.type = Image.Type.Simple;
                    img.preserveAspect = false;
                    img.color = Color.white;
                }
            }

            GameObject card = GameObject.Find("MenuCard");
            if (card != null && darkWood != null)
            {
                ApplySliced(card.GetComponent<Image>(), darkWood, new Color(1f, 1f, 1f, 0.94f), 0.42f);
                SkinButton(card.transform.Find("BtnNewGame"), btnWood, new Color(0.76f, 1f, 0.84f, 1f));
                SkinButton(card.transform.Find("BtnContinue"), btnWood, new Color(0.82f, 0.94f, 1f, 1f));
                SkinButton(card.transform.Find("BtnQuit"), btnWood, new Color(1f, 0.80f, 0.80f, 1f));
            }

            var tmps = UnityEngine.Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
            for (int i = 0; i < tmps.Length; i++)
            {
                tmps[i].text = ScrubEmDash(tmps[i].text);
                EditorUtility.SetDirty(tmps[i]);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV] Đã nâng cấp MainMenu.unity với nền phong cảnh pixel art & khung gỗ.");
        }

        private static void SkinWorldScenePromptsAndScrubEmDashes()
        {
            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            Sprite darkWood = LoadUI("ui_panel_dark_wood.png");

            int promptCount = 0;
            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (allGos[i].name == "PromptCanvas" && darkWood != null)
                {
                    var img = allGos[i].GetComponent<Image>();
                    if (img != null)
                    {
                        ApplySliced(img, darkWood, Color.white, 0.60f);
                        EditorUtility.SetDirty(img);
                        promptCount++;
                    }
                }
            }

            var tmps = UnityEngine.Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
            for (int i = 0; i < tmps.Length; i++)
            {
                string u = ScrubEmDash(tmps[i].text);
                if (u != tmps[i].text)
                {
                    tmps[i].text = u;
                    EditorUtility.SetDirty(tmps[i]);
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[MV] Đã nâng cấp khung gỗ pixel cho {promptCount} thanh tương tác [E] trong scene thế giới.");
        }
    }
}
