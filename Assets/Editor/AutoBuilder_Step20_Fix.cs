using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step20_Fix
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string UIRoot = "Assets/Art/Sprites/UI/";

        [MenuItem("Moonpetal/Step 20/2. Fix 9-Slice UI Proportions & Apply Scenic Main Menu")]
        public static void ApplyStep20Fix()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU STEP 20.FIX (SỬA TỶ LỆ 9-SLICE UI & NỀN MAIN MENU MỚI) ===");
            AssetDatabase.Refresh();
            FixUISpriteImportersPPU100();
            AssetDatabase.Refresh();

            FixGameRootUILayout();
            FixMainMenuLayout();
            FixWorldSpacePrompts();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 20.FIX! Hãy bấm Play từ MainMenu để xem giao diện mới sắc nét nhé. ===");
        }

        private static Sprite LoadUI(string name) => AssetDatabase.LoadAssetAtPath<Sprite>(UIRoot + name);

        private static void ImportUISprite(string file, float ppu, Vector4 border, FilterMode filter = FilterMode.Point)
        {
            string path = UIRoot + file;
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) return;
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.filterMode = filter;
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

        private static void FixUISpriteImportersPPU100()
        {
            // PPU = 100 matches CanvasScaler referencePixelsPerUnit = 100!
            // With pixelsPerUnitMultiplier = 0.5f, each 1 pixel of border renders cleanly as 2px on 1080p!
            ImportUISprite("ui_panel_wood_parchment.png", 100f, new Vector4(8, 8, 8, 8));
            ImportUISprite("ui_panel_dark_wood.png",      100f, new Vector4(8, 8, 8, 8));
            ImportUISprite("ui_slot_normal.png",          100f, new Vector4(4, 4, 4, 4));
            ImportUISprite("ui_slot_selected.png",        100f, new Vector4(4, 4, 4, 4));
            ImportUISprite("ui_button_wood.png",          100f, new Vector4(5, 5, 5, 5));
            ImportUISprite("ui_bar_frame.png",            100f, new Vector4(4, 4, 4, 4));
            ImportUISprite("ui_bar_fill_hp.png",          100f, Vector4.zero);
            ImportUISprite("ui_bar_fill_stamina.png",     100f, Vector4.zero);
            ImportUISprite("ui_icon_light_hp.png",        100f, Vector4.zero);
            ImportUISprite("ui_icon_stamina.png",         100f, Vector4.zero);
            ImportUISprite("ui_mainmenu_bg.png",          100f, Vector4.zero, FilterMode.Bilinear);
        }

        private static void ApplySliced(Image img, Sprite spr, Color tint, float ppuMult = 0.55f)
        {
            if (img == null || spr == null) return;
            img.sprite = spr;
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = ppuMult;
            img.color = tint;
        }

        private static void FixBarInset(Transform barTf, Sprite barFrame)
        {
            if (barTf == null) return;
            var r = barTf.GetComponent<RectTransform>();
            if (r != null) r.sizeDelta = new Vector2(340f, 34f);

            ApplySliced(barTf.GetComponent<Image>(), barFrame, Color.white, 0.55f);

            // Inset Fill cleanly inside the wooden frame so it NEVER bleeds over borders
            Transform fill = barTf.Find("Fill");
            if (fill != null)
            {
                var fr = fill.GetComponent<RectTransform>();
                if (fr != null)
                {
                    fr.offsetMin = new Vector2(7f, 6f);
                    fr.offsetMax = new Vector2(-7f, -6f);
                }
            }

            Transform txt = barTf.Find("Text");
            if (txt != null)
            {
                var tr = txt.GetComponent<RectTransform>();
                if (tr != null)
                {
                    tr.offsetMin = new Vector2(38f, 0f);
                    tr.offsetMax = new Vector2(-10f, 0f);
                }
                var tmp = txt.GetComponent<TMP_Text>();
                if (tmp != null)
                {
                    tmp.fontSize = 17;
                    tmp.color = Color.white;
                }
            }
        }

        private static void FixGameRootUILayout()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Sprite woodParch = LoadUI("ui_panel_wood_parchment.png");
            Sprite darkWood  = LoadUI("ui_panel_dark_wood.png");
            Sprite slotNorm  = LoadUI("ui_slot_normal.png");
            Sprite slotSel   = LoadUI("ui_slot_selected.png");
            Sprite btnWood   = LoadUI("ui_button_wood.png");
            Sprite barFrame  = LoadUI("ui_bar_frame.png");

            // 1. Toast Panel
            Transform toastPanel = root.transform.Find("SystemOverlayCanvas/ToastPanel");
            if (toastPanel != null)
            {
                ApplySliced(toastPanel.GetComponent<Image>(), darkWood, Color.white, 0.55f);
                var tr = toastPanel.Find("ToastText")?.GetComponent<RectTransform>();
                if (tr != null) { tr.offsetMin = new Vector2(22f, 10f); tr.offsetMax = new Vector2(-22f, -10f); }
            }

            Transform hud = root.transform.Find("ClockHUDCanvas");
            if (hud != null)
            {
                // 2. Clock Panel (crisp slender border, generous padding)
                Transform clockP = hud.Find("ClockBadgePanel");
                if (clockP != null)
                {
                    clockP.GetComponent<RectTransform>().sizeDelta = new Vector2(368f, 58f);
                    ApplySliced(clockP.GetComponent<Image>(), darkWood, Color.white, 0.55f);
                    var dotR = clockP.Find("PhaseDot")?.GetComponent<RectTransform>();
                    if (dotR != null) dotR.anchoredPosition = new Vector2(28f, 0f);
                    var txtR = clockP.Find("ClockText")?.GetComponent<RectTransform>();
                    if (txtR != null) { txtR.offsetMin = new Vector2(48f, 8f); txtR.offsetMax = new Vector2(-18f, -8f); }
                }

                // 3. Quest Tracker Panel (generous padding so text stays cleanly inside wood frame!)
                Transform questP = hud.Find("QuestTrackerPanel");
                if (questP != null)
                {
                    var qr = questP.GetComponent<RectTransform>();
                    qr.anchoredPosition = new Vector2(-24f, -90f);
                    qr.sizeDelta = new Vector2(396f, 146f);
                    ApplySliced(questP.GetComponent<Image>(), darkWood, Color.white, 0.55f);
                    var tr = questP.Find("ObjectiveText")?.GetComponent<RectTransform>();
                    if (tr != null) { tr.offsetMin = new Vector2(22f, 16f); tr.offsetMax = new Vector2(-22f, -16f); }
                }

                // 4. Stats Panel (HP & Stamina)
                Transform statsP = hud.Find("StatsPanel");
                if (statsP != null)
                {
                    statsP.GetComponent<RectTransform>().sizeDelta = new Vector2(376f, 126f);
                    ApplySliced(statsP.GetComponent<Image>(), darkWood, Color.white, 0.55f);

                    Transform hpBar = statsP.Find("LightHPBar");
                    if (hpBar != null)
                    {
                        hpBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(18f, -18f);
                        FixBarInset(hpBar, barFrame);
                    }
                    Transform stBar = statsP.Find("StaminaBar");
                    if (stBar != null)
                    {
                        stBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(18f, -60f);
                        FixBarInset(stBar, barFrame);
                    }
                    Transform exh = statsP.Find("ExhaustedBadge");
                    if (exh != null) exh.GetComponent<RectTransform>().anchoredPosition = new Vector2(18f, -98f);
                }

                // 5. Hotbar Slots (crisp 8px frame leaving 54x54 icon recess!)
                Transform hotbarP = hud.Find("HotbarPanel");
                if (hotbarP != null)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        Transform slotTf = hotbarP.Find("Slot_" + i);
                        if (slotTf == null) continue;
                        var sImg = slotTf.GetComponent<Image>();
                        ApplySliced(sImg, i == 1 ? slotSel : slotNorm, Color.white, 0.55f);

                        var iconR = slotTf.Find("Icon")?.GetComponent<RectTransform>();
                        if (iconR != null) { iconR.offsetMin = new Vector2(11f, 11f); iconR.offsetMax = new Vector2(-11f, -11f); }
                        var numR = slotTf.Find("Num")?.GetComponent<RectTransform>();
                        if (numR != null) { numR.offsetMin = new Vector2(8f, 6f); numR.offsetMax = new Vector2(-8f, -6f); }
                        var cntR = slotTf.Find("Count")?.GetComponent<RectTransform>();
                        if (cntR != null) { cntR.offsetMin = new Vector2(8f, 6f); cntR.offsetMax = new Vector2(-8f, -6f); }
                    }
                }

                // 6. Inventory Modal
                Transform invPanel = hud.Find("InventoryModalHost/InventoryPanelRoot");
                if (invPanel != null)
                {
                    ApplySliced(invPanel.GetComponent<Image>(), darkWood, Color.white, 0.50f);
                    for (int i = 0; i < 16; i++)
                    {
                        Transform slotTf = invPanel.Find("BagSlot_" + i);
                        if (slotTf == null) continue;
                        ApplySliced(slotTf.GetComponent<Image>(), slotNorm, Color.white, 0.55f);
                        var iconR = slotTf.Find("Icon")?.GetComponent<RectTransform>();
                        if (iconR != null) { iconR.offsetMin = new Vector2(10f, 10f); iconR.offsetMax = new Vector2(-10f, -10f); }
                    }
                }

                // 7. Dialogue Box (Warm Wood & Parchment!)
                Transform dlgBox = hud.Find("DialogueUIHost/DialogueBox");
                if (dlgBox != null)
                {
                    ApplySliced(dlgBox.GetComponent<Image>(), woodParch, Color.white, 0.48f);
                }

                // 8. Diary, Bundle, Victory & Pause Modals
                Transform diaryBox = hud.Find("DiaryUIHost/DiaryBox");
                if (diaryBox != null)
                {
                    ApplySliced(diaryBox.GetComponent<Image>(), woodParch, Color.white, 0.48f);
                    foreach (string bn in new[] { "BtnPrev", "BtnClose", "BtnNext" })
                        ApplySliced(diaryBox.Find(bn)?.GetComponent<Image>(), btnWood, Color.white, 0.50f);
                }

                Transform bundleP = hud.Find("BundleUIHost/BundlePanelRoot");
                if (bundleP != null)
                {
                    ApplySliced(bundleP.GetComponent<Image>(), darkWood, Color.white, 0.48f);
                    ApplySliced(bundleP.Find("BtnContribute")?.GetComponent<Image>(), btnWood, new Color(0.80f, 0.92f, 1f, 1f), 0.50f);
                    ApplySliced(bundleP.Find("BtnBloom")?.GetComponent<Image>(), btnWood, new Color(0.78f, 1f, 0.84f, 1f), 0.50f);
                    ApplySliced(bundleP.Find("BtnClose")?.GetComponent<Image>(), btnWood, new Color(1f, 0.82f, 0.82f, 1f), 0.50f);
                }

                Transform victoryP = hud.Find("BundleUIHost/VictoryPanelRoot");
                if (victoryP != null)
                {
                    ApplySliced(victoryP.GetComponent<Image>(), darkWood, Color.white, 0.48f);
                    ApplySliced(victoryP.Find("BtnVictoryClose")?.GetComponent<Image>(), btnWood, new Color(0.82f, 1f, 0.86f, 1f), 0.50f);
                }

                Transform pauseP = hud.Find("PauseMenuHost/PausePanelRoot");
                if (pauseP != null)
                {
                    ApplySliced(pauseP.GetComponent<Image>(), darkWood, Color.white, 0.48f);
                    foreach (string bn in new[] { "BtnResume", "BtnSave", "BtnLoad", "BtnMute", "BtnDeleteSave" })
                        ApplySliced(pauseP.Find(bn)?.GetComponent<Image>(), btnWood, Color.white, 0.50f);
                }
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã căn chuẩn tỷ lệ 9-slice & padding cho toàn bộ HUD và Modal trong GameRoot.prefab.");
        }

        private static void FixMainMenuLayout()
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
                var r = card.GetComponent<RectTransform>();
                r.sizeDelta = new Vector2(680f, 470f);
                ApplySliced(card.GetComponent<Image>(), darkWood, new Color(1f, 1f, 1f, 0.88f), 0.45f);

                var vlg = card.GetComponent<VerticalLayoutGroup>();
                if (vlg != null)
                {
                    vlg.padding = new RectOffset(56, 56, 42, 42);
                    vlg.spacing = 14f;
                }

                ApplySliced(card.transform.Find("BtnNewGame")?.GetComponent<Image>(), btnWood, new Color(0.76f, 1f, 0.84f, 1f), 0.50f);
                ApplySliced(card.transform.Find("BtnContinue")?.GetComponent<Image>(), btnWood, new Color(0.82f, 0.94f, 1f, 1f), 0.50f);
                ApplySliced(card.transform.Find("BtnQuit")?.GetComponent<Image>(), btnWood, new Color(1f, 0.80f, 0.80f, 1f), 0.50f);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV] Đã áp dụng nền Main Menu phong cảnh mới và căn gọn MenuCard + nút gỗ.");
        }

        private static void FixWorldSpacePrompts()
        {
            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            Sprite darkWood = LoadUI("ui_panel_dark_wood.png");

            int count = 0;
            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (allGos[i].name == "PromptCanvas" && darkWood != null)
                {
                    var img = allGos[i].GetComponent<Image>();
                    if (img != null)
                    {
                        ApplySliced(img, darkWood, new Color(1f, 1f, 1f, 0.94f), 1.0f);
                        var pr = allGos[i].GetComponent<RectTransform>();
                        if (pr != null && pr.sizeDelta.y < 48f) pr.sizeDelta = new Vector2(pr.sizeDelta.x + 20f, 48f);
                        var txtR = allGos[i].transform.Find("Text")?.GetComponent<RectTransform>();
                        if (txtR != null) { txtR.offsetMin = new Vector2(14f, 6f); txtR.offsetMax = new Vector2(-14f, -6f); }
                        EditorUtility.SetDirty(allGos[i]);
                        count++;
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[MV] Đã căn gọn 9-slice cho {count} thanh tương tác [E] trong scene thế giới.");
        }
    }
}
