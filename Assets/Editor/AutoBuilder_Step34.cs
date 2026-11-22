using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Narrative;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step34
    {
        private const string SCENE_PATH = "Assets/Scenes/MoonpetalValley_MVP.unity";

        [MenuItem("Moonpetal/Build/Step 34 - Guided Tutorial & Pause Menu Tips")]
        public static void BuildStep34()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);

            TMP_FontAsset hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_HudCleanFont.asset");
            if (hudFont == null) hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_DynamicFont.asset");

            TMP_FontAsset storyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_StoryFont.asset");
            if (storyFont == null) storyFont = hudFont;

            Sprite parchmentWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_panel_wood_parchment.png");
            Sprite darkWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_panel_dark_wood.png");
            Sprite btnWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_button_wood.png");

            TutorialHintHUDView hintView = BuildOrUpdateTutorialHintCanvas(hudFont, parchmentWood);
            BuildOrUpdateTutorialManager(hintView);
            UpgradePauseMenuWithTutorialAndTips(hudFont, storyFont, darkWood, btnWood);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("[MV][Step34] Build thanh cong: The giay go thu cong + TutorialManager + Pause Menu Tips.");
            ValidateStep34();
        }

        private static TutorialHintHUDView BuildOrUpdateTutorialHintCanvas(TMP_FontAsset hudFont, Sprite parchmentWood)
        {
            GameObject existing = GameObject.Find("TutorialHintCanvas");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject canvasGo = new GameObject("TutorialHintCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 115;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject noteCard = CreateUIObject("TutorialNoteCard", canvasGo.transform);
            var cardRt = noteCard.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 1f);
            cardRt.anchorMax = new Vector2(0.5f, 1f);
            cardRt.pivot = new Vector2(0.5f, 1f);
            cardRt.anchoredPosition = new Vector2(0f, -14f);
            cardRt.sizeDelta = new Vector2(480f, 44f);

            var cg = noteCard.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            var bgImg = noteCard.AddComponent<Image>();
            if (parchmentWood != null)
            {
                bgImg.sprite = parchmentWood;
                bgImg.type = Image.Type.Sliced;
                bgImg.color = Color.white;
            }
            else
            {
                bgImg.color = new Color(0.94f, 0.89f, 0.78f, 0.96f);
            }
            bgImg.raycastTarget = false;

            GameObject hintGo = CreateUIObject("HintText", noteCard.transform);
            var hintRt = hintGo.GetComponent<RectTransform>();
            hintRt.anchorMin = new Vector2(0f, 0f);
            hintRt.anchorMax = new Vector2(1f, 1f);
            hintRt.offsetMin = new Vector2(20f, 4f);
            hintRt.offsetMax = new Vector2(-20f, -4f);

            var hintTmp = hintGo.AddComponent<TextMeshProUGUI>();
            hintTmp.font = hudFont;
            hintTmp.fontSize = 14.5f;
            hintTmp.alignment = TextAlignmentOptions.Center;
            hintTmp.textWrappingMode = TextWrappingModes.Normal;
            hintTmp.color = new Color(0.23f, 0.15f, 0.09f, 1f);
            hintTmp.raycastTarget = false;
            hintTmp.text = "";

            var view = canvasGo.AddComponent<TutorialHintHUDView>();
            var so = new SerializedObject(view);
            so.FindProperty("rootGroup").objectReferenceValue = cg;
            so.FindProperty("hintText").objectReferenceValue = hintTmp;
            so.ApplyModifiedPropertiesWithoutUndo();

            return view;
        }

        private static void BuildOrUpdateTutorialManager(TutorialHintHUDView hintView)
        {
            GameObject existing = GameObject.Find("TutorialManager");
            if (existing == null) existing = new GameObject("TutorialManager");

            var mgr = existing.GetComponent<TutorialManager>();
            if (mgr == null) mgr = existing.AddComponent<TutorialManager>();

            var so = new SerializedObject(mgr);
            so.FindProperty("hintView").objectReferenceValue = hintView;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void UpgradePauseMenuWithTutorialAndTips(TMP_FontAsset hudFont, TMP_FontAsset storyFont, Sprite darkWood, Sprite btnWood)
        {
            var pauseUI = Object.FindAnyObjectByType<PauseMenuUIView>(FindObjectsInactive.Include);
            if (pauseUI == null)
            {
                Debug.LogWarning("[MV][Step34] Khong tim thay PauseMenuUIView trong scene.");
                return;
            }

            var so = new SerializedObject(pauseUI);
            GameObject panelRoot = so.FindProperty("panelRoot").objectReferenceValue as GameObject;
            Button btnDelete = so.FindProperty("btnDeleteSave").objectReferenceValue as Button;
            if (panelRoot == null)
            {
                Debug.LogWarning("[MV][Step34] PauseMenuUIView thieu panelRoot.");
                return;
            }

            Transform oldTut = panelRoot.transform.Find("BtnToggleTutorial_Step34");
            if (oldTut != null) Object.DestroyImmediate(oldTut.gameObject);
            Transform oldTips = panelRoot.transform.Find("BtnOpenTips_Step34");
            if (oldTips != null) Object.DestroyImmediate(oldTips.gameObject);
            Transform oldModal = pauseUI.transform.Find("TipsModalPanel_Step34");
            if (oldModal != null) Object.DestroyImmediate(oldModal.gameObject);

            Transform buttonParent = btnDelete != null ? btnDelete.transform.parent : panelRoot.transform;
            bool hasLayout = buttonParent.GetComponent<VerticalLayoutGroup>() != null;

            Vector2 btnSize = new Vector2(280f, 38f);
            float baseY = -120f;
            if (btnDelete != null)
            {
                var delRt = btnDelete.GetComponent<RectTransform>();
                btnSize = delRt.sizeDelta;
                baseY = delRt.anchoredPosition.y;
            }

            if (!hasLayout)
            {
                var panelRt = panelRoot.GetComponent<RectTransform>();
                if (panelRt != null && panelRt.sizeDelta.y < 470f)
                {
                    panelRt.sizeDelta = new Vector2(panelRt.sizeDelta.x, 470f);
                }
            }

            Button btnOpenTips = CreatePauseButton(buttonParent, "BtnOpenTips_Step34", "Xem Lại Mẹo Chơi", hudFont, btnWood, btnSize,
                hasLayout, new Vector2(0f, baseY - (btnSize.y + 10f)), out _);

            Button btnToggleTut = CreatePauseButton(buttonParent, "BtnToggleTutorial_Step34", "Gợi Ý Hướng Dẫn: ĐANG BẬT", hudFont, btnWood, btnSize,
                hasLayout, new Vector2(0f, baseY - (btnSize.y + 10f) * 2f), out TMP_Text tutLabel);

            GameObject modal = CreateUIObject("TipsModalPanel_Step34", pauseUI.transform);
            StretchFull(modal.GetComponent<RectTransform>());
            modal.AddComponent<Image>().color = new Color(0.04f, 0.04f, 0.07f, 0.72f);

            GameObject card = CreateUIObject("TipsCard", modal.transform);
            var cardRt = card.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(680f, 470f);
            var cardImg = card.AddComponent<Image>();
            if (darkWood != null)
            {
                cardImg.sprite = darkWood;
                cardImg.type = Image.Type.Sliced;
            }
            cardImg.color = new Color(0.15f, 0.11f, 0.09f, 0.98f);

            GameObject titleGo = CreateUIObject("Title", card.transform);
            var titleRt = titleGo.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 1f);
            titleRt.anchorMax = new Vector2(1f, 1f);
            titleRt.pivot = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0f, -18f);
            titleRt.sizeDelta = new Vector2(0f, 36f);
            var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
            titleTmp.font = storyFont;
            titleTmp.fontSize = 24f;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = new Color(0.95f, 0.84f, 0.56f, 1f);
            titleTmp.text = "SỔ TAY MẸO CHƠI MOONPETAL VALLEY";

            GameObject bodyGo = CreateUIObject("TipsBody", card.transform);
            var bodyRt = bodyGo.GetComponent<RectTransform>();
            bodyRt.anchorMin = new Vector2(0f, 0f);
            bodyRt.anchorMax = new Vector2(1f, 1f);
            bodyRt.offsetMin = new Vector2(32f, 72f);
            bodyRt.offsetMax = new Vector2(-32f, -66f);
            var bodyTmp = bodyGo.AddComponent<TextMeshProUGUI>();
            bodyTmp.font = hudFont;
            bodyTmp.fontSize = 15f;
            bodyTmp.alignment = TextAlignmentOptions.TopLeft;
            bodyTmp.textWrappingMode = TextWrappingModes.Normal;
            bodyTmp.color = new Color(0.95f, 0.93f, 0.87f, 1f);
            bodyTmp.text =
                "<b>1. Di chuyển & Tương tác</b>\n" +
                "- [WASD]: Di chuyển  |  [Space]: Lướt né nhanh  |  [E]: Tương tác, nói chuyện, thu hoạch\n" +
                "- [R] hoặc Cuộn chuột: Đổi mục tiêu khi đứng gần nhiều vật thể\n\n" +
                "<b>2. Công cụ & Nông trại</b>\n" +
                "- [1] Cuốc đất  |  [2] Bình tưới (nạp nước tại Giếng [E])  |  [3] Rìu chặt gỗ\n" +
                "- [Chuột trái]: Dùng công cụ đang cầm  |  Hạt Lúa Mạch tưới đủ nước sẽ lớn qua từng ngày\n\n" +
                "<b>3. Chiến đấu & Sương Héo Úa</b>\n" +
                "- [4] Liềm Ánh Trăng: Quét sạch Bọt Sương Đen; nhớ lướt né [Space] khi chúng lao tới\n" +
                "- Sau khi Hoa Moonpetal nở, bấm [Q] kích hoạt Xung Ánh Trăng của Lumi\n\n" +
                "<b>4. Hội thoại, Nhật ký & Giấc ngủ</b>\n" +
                "- [L]: Xem lại 10 câu thoại gần nhất  |  [J]: Mở Sổ Tay Kỷ Niệm  |  [I]/[Tab]: Túi đồ\n" +
                "- Về Giường ngủ trong Nhà Kính [E] trước khi kiệt sức để hồi 100% Thể lực và Tự động lưu";

            Button btnCloseTips = CreatePauseButton(card.transform, "BtnCloseTips", "Đóng Sổ Tay", hudFont, btnWood, new Vector2(180f, 40f),
                false, new Vector2(0f, -195f), out _);

            modal.SetActive(false);

            so.FindProperty("btnToggleTutorial").objectReferenceValue = btnToggleTut;
            so.FindProperty("tutorialButtonText").objectReferenceValue = tutLabel;
            so.FindProperty("btnOpenTips").objectReferenceValue = btnOpenTips;
            so.FindProperty("tipsModalRoot").objectReferenceValue = modal;
            so.FindProperty("btnCloseTips").objectReferenceValue = btnCloseTips;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Button CreatePauseButton(Transform parent, string name, string label, TMP_FontAsset font, Sprite sprite,
            Vector2 size, bool useLayout, Vector2 anchoredPos, out TMP_Text textOut)
        {
            GameObject go = CreateUIObject(name, parent);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            if (!useLayout)
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = anchoredPos;
            }
            else
            {
                var le = go.AddComponent<LayoutElement>();
                le.preferredWidth = size.x;
                le.preferredHeight = size.y;
            }

            var img = go.AddComponent<Image>();
            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = Image.Type.Sliced;
            }
            img.color = new Color(0.95f, 0.90f, 0.82f, 1f);

            var btn = go.AddComponent<Button>();

            GameObject txtGo = CreateUIObject("Label", go.transform);
            StretchFull(txtGo.GetComponent<RectTransform>());
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = 15f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.18f, 0.12f, 0.08f, 1f);
            tmp.text = label;
            textOut = tmp;

            return btn;
        }

        [MenuItem("Moonpetal/Validate/Step 34")]
        public static void ValidateStep34()
        {
            int fails = 0;

            var mgr = Object.FindAnyObjectByType<TutorialManager>(FindObjectsInactive.Include);
            if (mgr == null)
            {
                Debug.LogError("[MV][Step34][FAIL] Thieu TutorialManager trong scene.");
                fails++;
            }

            var view = Object.FindAnyObjectByType<TutorialHintHUDView>(FindObjectsInactive.Include);
            if (view == null)
            {
                Debug.LogError("[MV][Step34][FAIL] Thieu TutorialHintHUDView trong scene.");
                fails++;
            }

            var pauseUI = Object.FindAnyObjectByType<PauseMenuUIView>(FindObjectsInactive.Include);
            if (pauseUI == null)
            {
                Debug.LogError("[MV][Step34][FAIL] Khong tim thay PauseMenuUIView.");
                fails++;
            }
            else
            {
                var so = new SerializedObject(pauseUI);
                if (so.FindProperty("btnToggleTutorial").objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Step34][FAIL] PauseMenuUIView thieu btnToggleTutorial.");
                    fails++;
                }
                if (so.FindProperty("tipsModalRoot").objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Step34][FAIL] PauseMenuUIView thieu tipsModalRoot.");
                    fails++;
                }
            }

            if (fails == 0)
            {
                Debug.Log("[MV][Step34] VALIDATE PASS (0 loi)!");
            }
            else
            {
                Debug.LogError("[MV][Step34] VALIDATE FAIL (" + fails + " loi).");
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
    }
}
