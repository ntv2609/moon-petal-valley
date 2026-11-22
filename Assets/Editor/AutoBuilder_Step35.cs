using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.LowLevel;
using TMPro;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step35
    {
        private const string MAIN_MENU_SCENE = "Assets/Scenes/MainMenu.unity";
        private const string WARM_FONT_PATH = "Assets/Art/Fonts/VN_MenuWarmFont.asset";
        private const string VN_PREWARM_CHARS =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,;:!?()-+/%[]'" +
            "ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚÝàáâãèéêìíòóôõùúýĂăĐđĨĩŨũƠơƯư" +
            "ẠạẢảẤấẦầẨẩẪẫẬậẮắẰằẲẳẴẵẶặẸẹẺẻẼẽẾếỀềỂểỄễỆệỈỉỊị" +
            "ỌọỎỏỐốỒồỔổỖỗỘộỚớỜờỞởỠỡỢợỤụỦủỨứỪừỬửỮữỰựỲỳỴỵỶỷỸỹ";

        [MenuItem("Moonpetal/Build/Step 35 - Main Menu Font Fix & Settings")]
        public static void BuildStep35()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            EnsureFolder("Assets/Art", "Fonts");
            TMP_FontAsset warmFont = CreateOrUpdateWarmMenuFont();
            if (warmFont == null)
            {
                warmFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_StoryFont.asset");
            }

            TMP_FontAsset hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_HudCleanFont.asset");
            if (hudFont == null) hudFont = warmFont;

            Sprite btnWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_button_wood.png");
            Sprite darkWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_panel_dark_wood.png");
            Sprite parchmentWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_panel_wood_parchment.png");

            var scene = EditorSceneManager.OpenScene(MAIN_MENU_SCENE, OpenSceneMode.Single);

            var ctrl = Object.FindAnyObjectByType<MainMenuController>(FindObjectsInactive.Include);
            if (ctrl == null)
            {
                Debug.LogError("[MV][Step35] Khong tim thay MainMenuController trong MainMenu.unity.");
                return;
            }

            var so = new SerializedObject(ctrl);
            Button btnNew = so.FindProperty("btnNewGame").objectReferenceValue as Button;
            Button btnCont = so.FindProperty("btnContinue").objectReferenceValue as Button;
            Button btnQuit = so.FindProperty("btnQuit").objectReferenceValue as Button;
            TMP_Text subText = so.FindProperty("continueSubtext").objectReferenceValue as TMP_Text;

            Transform menuParent = btnNew != null ? btnNew.transform.parent : ctrl.transform;

            // Xoa ban cu cua Step 35 neu chay lai (idempotent)
            Transform oldBtnSettings = menuParent.Find("BtnSettings_Step35");
            if (oldBtnSettings != null) Object.DestroyImmediate(oldBtnSettings.gameObject);

            Canvas rootCanvas = ctrl.GetComponentInParent<Canvas>();
            Transform modalParent = rootCanvas != null ? rootCanvas.transform : ctrl.transform;
            Transform oldModal = modalParent.Find("SettingsModal_Step35");
            if (oldModal != null) Object.DestroyImmediate(oldModal.gameObject);

            // Tao nut Cai Dat
            Button btnSettings = CreateWoodMenuButton(menuParent, "BtnSettings_Step35", "Cài Đặt", warmFont, btnWood, new Color(0.84f, 0.70f, 0.48f, 1f));

            // Dong bo font + mau + kich thuoc + vi tri cho ca 4 nut
            Vector2 btnSize = new Vector2(272f, 40f);
            StyleExistingButton(btnNew, "Chơi Mới", warmFont, btnWood, new Color(0.95f, 0.80f, 0.48f, 1f), btnSize, new Vector2(0f, -48f));
            StyleExistingButton(btnCont, "Tiếp Tục Hành Trình", warmFont, btnWood, new Color(0.87f, 0.74f, 0.50f, 1f), btnSize, new Vector2(0f, -96f));
            StyleExistingButton(btnSettings, "Cài Đặt", warmFont, btnWood, new Color(0.82f, 0.68f, 0.47f, 1f), btnSize, new Vector2(0f, -144f));
            StyleExistingButton(btnQuit, "Thoát Game", warmFont, btnWood, new Color(0.82f, 0.54f, 0.36f, 1f), btnSize, new Vector2(0f, -192f));

            // Neu menuParent co VerticalLayoutGroup, dat thu tu sibling cho chuan
            if (btnNew != null) btnNew.transform.SetSiblingIndex(0);
            if (btnCont != null) btnCont.transform.SetSiblingIndex(1);
            if (btnSettings != null) btnSettings.transform.SetSiblingIndex(2);
            if (btnQuit != null) btnQuit.transform.SetSiblingIndex(3);

            // Dong bo font cho subtext (neu co)
            if (subText != null)
            {
                subText.font = hudFont;
                subText.fontSize = 13.5f;
                subText.color = new Color(0.96f, 0.92f, 0.82f, 0.92f);
            }

            // Cap nhat tat ca TMP_Text khac dang dung VN_MenuFont bi loi dau
            var allTexts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
            for (int i = 0; i < allTexts.Length; i++)
            {
                if (allTexts[i] == null) continue;
                if (allTexts[i].font != null && allTexts[i].font.name.Contains("VN_MenuFont"))
                {
                    allTexts[i].font = warmFont;
                }
            }

            // Dung bang Cai Dat
            MainMenuSettingsUI settingsUI = BuildSettingsModal(modalParent, warmFont, hudFont, darkWood, parchmentWood, btnWood);

            so.FindProperty("btnSettings").objectReferenceValue = btnSettings;
            so.FindProperty("settingsUI").objectReferenceValue = settingsUI;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("[MV][Step35] Build thanh cong: VN_MenuWarmFont (khong loi dau) + Nut Cai Dat + Bang Cai Dat.");
            ValidateStep35();
        }

        private static TMP_FontAsset CreateOrUpdateWarmMenuFont()
        {
            string[] candidates =
            {
                @"C:\Windows\Fonts\cambriab.ttf",
                @"C:\Windows\Fonts\timesbd.ttf",
                @"C:\Windows\Fonts\tahomabd.ttf",
                @"C:\Windows\Fonts\segoeuib.ttf",
                @"C:\Windows\Fonts\arialbd.ttf"
            };

            string chosenPath = null;
            for (int i = 0; i < candidates.Length; i++)
            {
                if (File.Exists(candidates[i]))
                {
                    chosenPath = candidates[i];
                    break;
                }
            }

            if (string.IsNullOrEmpty(chosenPath))
            {
                Debug.LogWarning("[MV][Step35] Khong tim thay font Windows TTF, dung VN_StoryFont du phong.");
                return null;
            }

            string localFontPath = "Assets/Art/Fonts/MenuWarmSource.ttf";
            File.Copy(chosenPath, Path.Combine(Directory.GetCurrentDirectory(), localFontPath), true);
            AssetDatabase.ImportAsset(localFontPath, ImportAssetOptions.ForceUpdate);

            Font srcFont = AssetDatabase.LoadAssetAtPath<Font>(localFontPath);
            if (srcFont == null) return null;

            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(WARM_FONT_PATH);
            if (existing != null)
            {
                AssetDatabase.DeleteAsset(WARM_FONT_PATH);
            }

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(srcFont, 58, 6, GlyphRenderMode.SDFAA, 1024, 1024);
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            fontAsset.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset>();
            fontAsset.TryAddCharacters(VN_PREWARM_CHARS);

            AssetDatabase.CreateAsset(fontAsset, WARM_FONT_PATH);
            if (fontAsset.material != null) AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            if (fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0 && fontAsset.atlasTextures[0] != null)
            {
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTextures[0], fontAsset);
            }

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            return fontAsset;
        }

        private static void StyleExistingButton(Button btn, string label, TMP_FontAsset font, Sprite woodSprite, Color tint, Vector2 size, Vector2 pos)
        {
            if (btn == null) return;

            var rt = btn.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.sizeDelta = size;
                if (btn.transform.parent.GetComponent<VerticalLayoutGroup>() == null)
                {
                    rt.anchorMin = new Vector2(0.5f, 0.5f);
                    rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = pos;
                }
            }

            var img = btn.GetComponent<Image>();
            if (img != null)
            {
                if (woodSprite != null)
                {
                    img.sprite = woodSprite;
                    img.type = Image.Type.Sliced;
                }
                img.color = tint;
            }

            var tmp = btn.GetComponentInChildren<TMP_Text>(true);
            if (tmp != null)
            {
                tmp.font = font;
                tmp.text = label;
                tmp.fontSize = 19f;
                tmp.fontStyle = FontStyles.Normal;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = new Color(1f, 0.96f, 0.86f, 1f);

                var shadow = tmp.GetComponent<Shadow>();
                if (shadow == null) shadow = tmp.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0.16f, 0.08f, 0.03f, 0.92f);
                shadow.effectDistance = new Vector2(1.4f, -1.6f);
            }
        }

        private static Button CreateWoodMenuButton(Transform parent, string name, string label, TMP_FontAsset font, Sprite woodSprite, Color tint)
        {
            GameObject go = CreateUIObject(name, parent);
            var img = go.AddComponent<Image>();
            if (woodSprite != null)
            {
                img.sprite = woodSprite;
                img.type = Image.Type.Sliced;
            }
            img.color = tint;

            var btn = go.AddComponent<Button>();

            GameObject txtGo = CreateUIObject("Label", go.transform);
            StretchFull(txtGo.GetComponent<RectTransform>());
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.text = label;
            tmp.fontSize = 19f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.96f, 0.86f, 1f);

            var shadow = txtGo.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.16f, 0.08f, 0.03f, 0.92f);
            shadow.effectDistance = new Vector2(1.4f, -1.6f);

            return btn;
        }

        private static MainMenuSettingsUI BuildSettingsModal(Transform canvasRoot, TMP_FontAsset titleFont, TMP_FontAsset hudFont,
            Sprite darkWood, Sprite parchmentWood, Sprite btnWood)
        {
            GameObject overlay = CreateUIObject("SettingsModal_Step35", canvasRoot);
            StretchFull(overlay.GetComponent<RectTransform>());
            overlay.AddComponent<Image>().color = new Color(0.05f, 0.04f, 0.08f, 0.72f);

            GameObject card = CreateUIObject("SettingsCard", overlay.transform);
            var cardRt = card.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(560f, 430f);

            var cardImg = card.AddComponent<Image>();
            if (parchmentWood != null)
            {
                cardImg.sprite = parchmentWood;
                cardImg.type = Image.Type.Sliced;
                cardImg.color = Color.white;
            }
            else if (darkWood != null)
            {
                cardImg.sprite = darkWood;
                cardImg.type = Image.Type.Sliced;
                cardImg.color = new Color(0.18f, 0.13f, 0.10f, 0.98f);
            }

            Color inkColor = parchmentWood != null ? new Color(0.24f, 0.14f, 0.08f, 1f) : new Color(0.96f, 0.93f, 0.85f, 1f);

            // Tieu de
            GameObject titleGo = CreateUIObject("Title", card.transform);
            var titleRt = titleGo.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 1f);
            titleRt.anchorMax = new Vector2(1f, 1f);
            titleRt.pivot = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0f, -20f);
            titleRt.sizeDelta = new Vector2(0f, 38f);
            var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
            titleTmp.font = titleFont;
            titleTmp.fontSize = 26f;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = inkColor;
            titleTmp.text = "CÀI ĐẶT";

            // Hang 1: Nhac nen (BGM)
            Slider sliderBgm = CreateSliderRow(card.transform, "RowBgm", "Nhạc nền (BGM)", 118f, hudFont, inkColor, out TMP_Text bgmVal);
            // Hang 2: Hieu ung (SFX)
            Slider sliderSfx = CreateSliderRow(card.transform, "RowSfx", "Hiệu ứng (SFX)", 66f, hudFont, inkColor, out TMP_Text sfxVal);
            // Hang 3: Tat tieng
            Button btnMute = CreateOptionRow(card.transform, "RowMute", "Âm thanh", 14f, hudFont, inkColor, btnWood, out TMP_Text muteVal);
            // Hang 4: Man hinh
            Button btnFs = CreateOptionRow(card.transform, "RowFullscreen", "Chế độ màn hình", -38f, hudFont, inkColor, btnWood, out TMP_Text fsVal);
            // Hang 5: Do phan giai
            Button btnRes = CreateOptionRow(card.transform, "RowResolution", "Độ phân giải", -90f, hudFont, inkColor, btnWood, out TMP_Text resVal);
            // Hang 6: Goi y huong dan
            Button btnTut = CreateOptionRow(card.transform, "RowTutorial", "Gợi ý hướng dẫn", -142f, hudFont, inkColor, btnWood, out TMP_Text tutVal);

            // Nut Dong & Luu
            Button btnClose = CreateWoodMenuButton(card.transform, "BtnCloseSettings", "Lưu & Đóng", titleFont, btnWood, new Color(0.86f, 0.72f, 0.45f, 1f));
            var closeRt = btnClose.GetComponent<RectTransform>();
            closeRt.anchorMin = new Vector2(0.5f, 0f);
            closeRt.anchorMax = new Vector2(0.5f, 0f);
            closeRt.pivot = new Vector2(0.5f, 0f);
            closeRt.anchoredPosition = new Vector2(0f, 22f);
            closeRt.sizeDelta = new Vector2(190f, 38f);

            var ui = canvasRoot.GetComponent<MainMenuSettingsUI>();
            if (ui == null) ui = canvasRoot.gameObject.AddComponent<MainMenuSettingsUI>();

            var so = new SerializedObject(ui);
            so.FindProperty("modalRoot").objectReferenceValue = overlay;
            so.FindProperty("sliderBgm").objectReferenceValue = sliderBgm;
            so.FindProperty("bgmValueText").objectReferenceValue = bgmVal;
            so.FindProperty("sliderSfx").objectReferenceValue = sliderSfx;
            so.FindProperty("sfxValueText").objectReferenceValue = sfxVal;
            so.FindProperty("btnMute").objectReferenceValue = btnMute;
            so.FindProperty("muteValueText").objectReferenceValue = muteVal;
            so.FindProperty("btnFullscreen").objectReferenceValue = btnFs;
            so.FindProperty("fullscreenValueText").objectReferenceValue = fsVal;
            so.FindProperty("btnResolution").objectReferenceValue = btnRes;
            so.FindProperty("resolutionValueText").objectReferenceValue = resVal;
            so.FindProperty("btnTutorial").objectReferenceValue = btnTut;
            so.FindProperty("tutorialValueText").objectReferenceValue = tutVal;
            so.FindProperty("btnClose").objectReferenceValue = btnClose;
            so.ApplyModifiedPropertiesWithoutUndo();

            overlay.SetActive(false);
            return ui;
        }

        private static Slider CreateSliderRow(Transform parent, string name, string label, float yPos, TMP_FontAsset font, Color inkColor, out TMP_Text valueTmp)
        {
            GameObject row = CreateUIObject(name, parent);
            var rt = row.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, yPos);
            rt.sizeDelta = new Vector2(470f, 36f);

            GameObject lblGo = CreateUIObject("Label", row.transform);
            var lblRt = lblGo.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0f, 0f);
            lblRt.anchorMax = new Vector2(0.42f, 1f);
            lblRt.offsetMin = Vector2.zero;
            lblRt.offsetMax = Vector2.zero;
            var lblTmp = lblGo.AddComponent<TextMeshProUGUI>();
            lblTmp.font = font;
            lblTmp.fontSize = 15.5f;
            lblTmp.fontStyle = FontStyles.Bold;
            lblTmp.alignment = TextAlignmentOptions.MidlineLeft;
            lblTmp.color = inkColor;
            lblTmp.text = label;

            GameObject sliderGo = CreateUIObject("Slider", row.transform);
            var sRt = sliderGo.GetComponent<RectTransform>();
            sRt.anchorMin = new Vector2(0.44f, 0.25f);
            sRt.anchorMax = new Vector2(0.84f, 0.75f);
            sRt.offsetMin = Vector2.zero;
            sRt.offsetMax = Vector2.zero;

            var bgGo = CreateUIObject("Background", sliderGo.transform);
            StretchFull(bgGo.GetComponent<RectTransform>());
            bgGo.AddComponent<Image>().color = new Color(0.32f, 0.22f, 0.14f, 0.45f);

            var fillArea = CreateUIObject("Fill Area", sliderGo.transform);
            StretchFull(fillArea.GetComponent<RectTransform>());
            var fillGo = CreateUIObject("Fill", fillArea.transform);
            StretchFull(fillGo.GetComponent<RectTransform>());
            var fillImg = fillGo.AddComponent<Image>();
            fillImg.color = new Color(0.75f, 0.48f, 0.18f, 0.95f);

            var handleArea = CreateUIObject("Handle Slide Area", sliderGo.transform);
            StretchFull(handleArea.GetComponent<RectTransform>());
            var handleGo = CreateUIObject("Handle", handleArea.transform);
            var hRt = handleGo.GetComponent<RectTransform>();
            hRt.sizeDelta = new Vector2(18f, 22f);
            var hImg = handleGo.AddComponent<Image>();
            hImg.color = new Color(0.42f, 0.24f, 0.12f, 1f);

            var slider = sliderGo.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.75f;
            slider.fillRect = fillGo.GetComponent<RectTransform>();
            slider.handleRect = hRt;
            slider.targetGraphic = hImg;

            GameObject valGo = CreateUIObject("ValueText", row.transform);
            var valRt = valGo.GetComponent<RectTransform>();
            valRt.anchorMin = new Vector2(0.86f, 0f);
            valRt.anchorMax = new Vector2(1f, 1f);
            valRt.offsetMin = Vector2.zero;
            valRt.offsetMax = Vector2.zero;
            valueTmp = valGo.AddComponent<TextMeshProUGUI>();
            valueTmp.font = font;
            valueTmp.fontSize = 15f;
            valueTmp.fontStyle = FontStyles.Bold;
            valueTmp.alignment = TextAlignmentOptions.MidlineRight;
            valueTmp.color = inkColor;
            valueTmp.text = "75%";

            return slider;
        }

        private static Button CreateOptionRow(Transform parent, string name, string label, float yPos, TMP_FontAsset font,
            Color inkColor, Sprite btnWood, out TMP_Text valueTmp)
        {
            GameObject row = CreateUIObject(name, parent);
            var rt = row.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, yPos);
            rt.sizeDelta = new Vector2(470f, 36f);

            GameObject lblGo = CreateUIObject("Label", row.transform);
            var lblRt = lblGo.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0f, 0f);
            lblRt.anchorMax = new Vector2(0.48f, 1f);
            lblRt.offsetMin = Vector2.zero;
            lblRt.offsetMax = Vector2.zero;
            var lblTmp = lblGo.AddComponent<TextMeshProUGUI>();
            lblTmp.font = font;
            lblTmp.fontSize = 15.5f;
            lblTmp.fontStyle = FontStyles.Bold;
            lblTmp.alignment = TextAlignmentOptions.MidlineLeft;
            lblTmp.color = inkColor;
            lblTmp.text = label;

            GameObject btnGo = CreateUIObject("OptionButton", row.transform);
            var bRt = btnGo.GetComponent<RectTransform>();
            bRt.anchorMin = new Vector2(0.52f, 0.05f);
            bRt.anchorMax = new Vector2(1f, 0.95f);
            bRt.offsetMin = Vector2.zero;
            bRt.offsetMax = Vector2.zero;

            var img = btnGo.AddComponent<Image>();
            if (btnWood != null)
            {
                img.sprite = btnWood;
                img.type = Image.Type.Sliced;
            }
            img.color = new Color(0.88f, 0.77f, 0.58f, 1f);

            var btn = btnGo.AddComponent<Button>();

            GameObject txtGo = CreateUIObject("Value", btnGo.transform);
            StretchFull(txtGo.GetComponent<RectTransform>());
            valueTmp = txtGo.AddComponent<TextMeshProUGUI>();
            valueTmp.font = font;
            valueTmp.fontSize = 14.5f;
            valueTmp.fontStyle = FontStyles.Bold;
            valueTmp.alignment = TextAlignmentOptions.Center;
            valueTmp.color = new Color(0.20f, 0.12f, 0.07f, 1f);
            valueTmp.text = "Đang bật";

            return btn;
        }

        [MenuItem("Moonpetal/Validate/Step 35")]
        public static void ValidateStep35()
        {
            int fails = 0;

            var warmFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(WARM_FONT_PATH);
            if (warmFont == null)
            {
                Debug.LogError("[MV][Step35][FAIL] Thieu VN_MenuWarmFont.asset.");
                fails++;
            }

            var ctrl = Object.FindAnyObjectByType<MainMenuController>(FindObjectsInactive.Include);
            if (ctrl == null)
            {
                Debug.LogError("[MV][Step35][FAIL] Khong tim thay MainMenuController.");
                fails++;
            }
            else
            {
                var so = new SerializedObject(ctrl);
                if (so.FindProperty("btnSettings").objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Step35][FAIL] MainMenuController thieu btnSettings.");
                    fails++;
                }
                if (so.FindProperty("settingsUI").objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Step35][FAIL] MainMenuController thieu settingsUI.");
                    fails++;
                }
            }

            if (fails == 0)
            {
                Debug.Log("[MV][Step35] VALIDATE PASS (0 loi)!");
            }
            else
            {
                Debug.LogError("[MV][Step35] VALIDATE FAIL (" + fails + " loi).");
            }
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string full = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(full))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
