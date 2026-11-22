using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step38
    {
        private const string GAMEROOT_PREFAB = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Build/Step 38 - Add Quit, Main Menu & Tips to Pause Menu Prefab")]
        public static void BuildStep38()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(GAMEROOT_PREFAB);
            if (root == null)
            {
                Debug.LogError("[MV][Step38] Khong tim thay GameRoot.prefab tai: " + GAMEROOT_PREFAB);
                return;
            }

            try
            {
                var pauseUI = root.GetComponentInChildren<PauseMenuUIView>(true);
                if (pauseUI == null)
                {
                    Debug.LogError("[MV][Step38] Khong tim thay PauseMenuUIView trong GameRoot.prefab.");
                    return;
                }

                TMP_FontAsset hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_HudCleanFont.asset");
                if (hudFont == null) hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_DynamicFont.asset");

                TMP_FontAsset storyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_StoryFont.asset");
                if (storyFont == null) storyFont = hudFont;

                Sprite darkWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_panel_dark_wood.png");
                Sprite btnWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_button_wood.png");

                var so = new SerializedObject(pauseUI);
                GameObject panelRoot = so.FindProperty("panelRoot").objectReferenceValue as GameObject;
                Button btnDel = so.FindProperty("btnDeleteSave").objectReferenceValue as Button;

                if (panelRoot == null || btnDel == null)
                {
                    Debug.LogError("[MV][Step38] PauseMenuUIView thieu panelRoot hoac btnDeleteSave.");
                    return;
                }

                Transform panelTf = panelRoot.transform;

                // Xoa cac nut/modal cu cua Step 34-38 neu chay lai (idempotent)
                string[] cleanupNames =
                {
                    "BtnOpenTips_Step38", "BtnToggleTutorial_Step38", "BtnMainMenu_Step38", "BtnQuitGame_Step38",
                    "BtnOpenTips_Step34", "BtnToggleTutorial_Step34", "BtnMainMenu_Step36", "BtnQuitGame_Step36"
                };
                for (int i = 0; i < cleanupNames.Length; i++)
                {
                    Transform t = panelTf.Find(cleanupNames[i]);
                    if (t != null) Object.DestroyImmediate(t.gameObject);
                }

                Transform oldModal = pauseUI.transform.Find("TipsModalPanel_Step38");
                if (oldModal != null) Object.DestroyImmediate(oldModal.gameObject);

                // Dieu chinh kich thuoc panel & layout cho 9 nut hien thi thoang dep
                var panelRt = panelRoot.GetComponent<RectTransform>();
                if (panelRt != null)
                {
                    panelRt.sizeDelta = new Vector2(Mathf.Max(panelRt.sizeDelta.x, 420f), 555f);
                }

                var vlg = panelRoot.GetComponent<VerticalLayoutGroup>();
                if (vlg != null)
                {
                    vlg.spacing = 8f;
                    vlg.padding = new RectOffset(24, 24, 18, 18);
                }

                // Cap nhat tieu de (bo em dash) va dong bo chieu cao cac nut cu
                Transform titleTf = panelTf.Find("Title");
                if (titleTf != null)
                {
                    var titleTmp = titleTf.GetComponent<TMP_Text>();
                    if (titleTmp != null)
                    {
                        titleTmp.text = "TẠM DỪNG - MOONPETAL VALLEY";
                        if (storyFont != null) titleTmp.font = storyFont;
                    }
                }

                var existingButtons = panelRoot.GetComponentsInChildren<Button>(true);
                for (int i = 0; i < existingButtons.Length; i++)
                {
                    var brt = existingButtons[i].GetComponent<RectTransform>();
                    if (brt != null) brt.sizeDelta = new Vector2(brt.sizeDelta.x, 38f);
                    var le = existingButtons[i].GetComponent<LayoutElement>();
                    if (le != null) le.preferredHeight = 38f;
                    var btmp = existingButtons[i].GetComponentInChildren<TMP_Text>(true);
                    if (btmp != null && hudFont != null) btmp.font = hudFont;
                }

                // Nhan ban truc tiep tu btnDel de giu nguyen style nut trong Pause Menu
                Button btnOpenTips = ClonePauseBtn(btnDel, "BtnOpenTips_Step38", "Xem Lại Mẹo Chơi", new Color(0.28f, 0.44f, 0.56f, 1f), hudFont);
                Button btnToggleTut = ClonePauseBtn(btnDel, "BtnToggleTutorial_Step38", "Gợi Ý Hướng Dẫn: ĐANG BẬT", new Color(0.34f, 0.42f, 0.30f, 1f), hudFont);
                Button btnMainMenu = ClonePauseBtn(btnDel, "BtnMainMenu_Step38", "Về Main Menu", new Color(0.46f, 0.36f, 0.24f, 1f), hudFont);
                Button btnQuitGame = ClonePauseBtn(btnDel, "BtnQuitGame_Step38", "Thoát Game", new Color(0.62f, 0.20f, 0.22f, 1f), hudFont);

                TMP_Text tutLabel = btnToggleTut.GetComponentInChildren<TMP_Text>(true);

                // Tao bang So Tay Meo Choi trong GameRoot.prefab
                GameObject tipsModal = CreateTipsModal(pauseUI.transform, hudFont, storyFont, darkWood, btnWood, out Button btnCloseTips);

                so.FindProperty("btnOpenTips").objectReferenceValue = btnOpenTips;
                so.FindProperty("btnToggleTutorial").objectReferenceValue = btnToggleTut;
                so.FindProperty("tutorialButtonText").objectReferenceValue = tutLabel;
                so.FindProperty("tipsModalRoot").objectReferenceValue = tipsModal;
                so.FindProperty("btnCloseTips").objectReferenceValue = btnCloseTips;
                so.FindProperty("btnMainMenu").objectReferenceValue = btnMainMenu;
                so.FindProperty("btnQuitGame").objectReferenceValue = btnQuitGame;
                so.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(root, GAMEROOT_PREFAB);
                Debug.Log("[MV][Step38] VALIDATE PASS (0 loi)! Da them nut Ve Main Menu, Thoat Game & Meo Choi vao GameRoot.prefab.");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static Button ClonePauseBtn(Button template, string newName, string label, Color tint, TMP_FontAsset font)
        {
            GameObject clone = Object.Instantiate(template.gameObject, template.transform.parent);
            clone.name = newName;
            var btn = clone.GetComponent<Button>();
            btn.onClick = new Button.ButtonClickedEvent();

            var img = clone.GetComponent<Image>();
            if (img != null) img.color = tint;

            var rt = clone.GetComponent<RectTransform>();
            if (rt != null) rt.sizeDelta = new Vector2(rt.sizeDelta.x, 38f);
            var le = clone.GetComponent<LayoutElement>();
            if (le != null) le.preferredHeight = 38f;

            var tmp = clone.GetComponentInChildren<TMP_Text>(true);
            if (tmp != null)
            {
                if (font != null) tmp.font = font;
                tmp.text = label;
                tmp.color = new Color(0.98f, 0.96f, 0.90f, 1f);
            }
            return btn;
        }

        private static GameObject CreateTipsModal(Transform parent, TMP_FontAsset hudFont, TMP_FontAsset storyFont,
            Sprite darkWood, Sprite btnWood, out Button btnClose)
        {
            GameObject modal = new GameObject("TipsModalPanel_Step38", typeof(RectTransform));
            modal.transform.SetParent(parent, false);
            StretchFull(modal.GetComponent<RectTransform>());
            modal.AddComponent<Image>().color = new Color(0.04f, 0.04f, 0.07f, 0.76f);

            GameObject card = new GameObject("TipsCard", typeof(RectTransform));
            card.transform.SetParent(modal.transform, false);
            var cardRt = card.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(680f, 460f);

            var cardImg = card.AddComponent<Image>();
            if (darkWood != null)
            {
                cardImg.sprite = darkWood;
                cardImg.type = Image.Type.Sliced;
            }
            cardImg.color = new Color(0.15f, 0.11f, 0.09f, 0.98f);

            GameObject titleGo = new GameObject("Title", typeof(RectTransform));
            titleGo.transform.SetParent(card.transform, false);
            var tRt = titleGo.GetComponent<RectTransform>();
            tRt.anchorMin = new Vector2(0f, 1f);
            tRt.anchorMax = new Vector2(1f, 1f);
            tRt.pivot = new Vector2(0.5f, 1f);
            tRt.anchoredPosition = new Vector2(0f, -18f);
            tRt.sizeDelta = new Vector2(0f, 36f);
            var tTmp = titleGo.AddComponent<TextMeshProUGUI>();
            tTmp.font = storyFont;
            tTmp.fontSize = 23f;
            tTmp.fontStyle = FontStyles.Bold;
            tTmp.alignment = TextAlignmentOptions.Center;
            tTmp.color = new Color(0.95f, 0.84f, 0.56f, 1f);
            tTmp.text = "SỔ TAY MẸO CHƠI MOONPETAL VALLEY";

            GameObject bodyGo = new GameObject("Body", typeof(RectTransform));
            bodyGo.transform.SetParent(card.transform, false);
            var bRt = bodyGo.GetComponent<RectTransform>();
            bRt.anchorMin = Vector2.zero;
            bRt.anchorMax = Vector2.one;
            bRt.offsetMin = new Vector2(32f, 70f);
            bRt.offsetMax = new Vector2(-32f, -64f);
            var bTmp = bodyGo.AddComponent<TextMeshProUGUI>();
            bTmp.font = hudFont;
            bTmp.fontSize = 15f;
            bTmp.alignment = TextAlignmentOptions.TopLeft;
            bTmp.textWrappingMode = TextWrappingModes.Normal;
            bTmp.color = new Color(0.95f, 0.93f, 0.87f, 1f);
            bTmp.text =
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

            GameObject closeGo = new GameObject("BtnCloseTips", typeof(RectTransform));
            closeGo.transform.SetParent(card.transform, false);
            var cRt = closeGo.GetComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0.5f, 0f);
            cRt.anchorMax = new Vector2(0.5f, 0f);
            cRt.pivot = new Vector2(0.5f, 0f);
            cRt.anchoredPosition = new Vector2(0f, 18f);
            cRt.sizeDelta = new Vector2(180f, 38f);
            var cImg = closeGo.AddComponent<Image>();
            if (btnWood != null)
            {
                cImg.sprite = btnWood;
                cImg.type = Image.Type.Sliced;
            }
            cImg.color = new Color(0.88f, 0.72f, 0.48f, 1f);
            btnClose = closeGo.AddComponent<Button>();

            GameObject cTxtGo = new GameObject("Label", typeof(RectTransform));
            cTxtGo.transform.SetParent(closeGo.transform, false);
            StretchFull(cTxtGo.GetComponent<RectTransform>());
            var cTmp = cTxtGo.AddComponent<TextMeshProUGUI>();
            cTmp.font = hudFont;
            cTmp.fontSize = 15f;
            cTmp.fontStyle = FontStyles.Bold;
            cTmp.alignment = TextAlignmentOptions.Center;
            cTmp.color = new Color(0.20f, 0.12f, 0.07f, 1f);
            cTmp.text = "Đóng Sổ Tay";

            modal.SetActive(false);
            return modal;
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
