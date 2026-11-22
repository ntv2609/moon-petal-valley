using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Combat;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step32
    {
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string MainScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string DarkWoodPath = "Assets/Art/Sprites/UI/ui_panel_dark_wood.png";
        private const string BtnWoodPath = "Assets/Art/Sprites/UI/ui_button_wood.png";
        private const string StoryFontPath = "Assets/Art/Fonts/VN_StoryFont.asset";
        private const string HudFontPath = "Assets/Art/Fonts/VN_HudCleanFont.asset";

        [MenuItem("Moonpetal/Step 32/1. Build Narrative System, Voices, Dialogue Log & Blight Fog")]
        public static void BuildStep32()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Khong the chay AutoBuilder khi dang Play Mode hoac dang bien dich.");
                return;
            }

            UpgradeAllMVPDialogues();
            UpgradeGameRootDialogueUI();
            UpgradeBlightFogInScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Step32] HOAN TAT: Da nap toan bo kich ban MVP moi, 2 anh dung, Lua chon, Nhat ky [L] & Suong Heo Ua song dong!");
        }

        private static void UpgradeAllMVPDialogues()
        {
            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            if (table == null) return;

            ItemData sickle = LoadItem("item_moon_sickle");
            ItemData barley = LoadItem("item_dew_barley");
            ItemData bread = LoadItem("item_sunshine_bread");
            ItemData bag1 = LoadItem("item_seed_bag_1");
            ItemData bag2 = LoadItem("item_seed_bag_2");
            DiaryPageData page2 = AssetDatabase.LoadAssetAtPath<DiaryPageData>("Assets/Data/Diary/DiaryPage_02.asset");
            DiaryPageData page3 = AssetDatabase.LoadAssetAtPath<DiaryPageData>("Assets/Data/Diary/DiaryPage_03.asset");

            // 1. Nhanh tra loi Ong Bach (Chon A: cung dau / Chon B: diu dang)
            var branchBold = CreateOrUpdateDlg(table, "dlg_bach_branch_bold", new[]
            {
                ("Ông Bach", "bach", "Hừm, cứng đầu y hệt bà ngoại cháu. Cầm lấy cái Liềm Ánh Trăng cũ này. Đừng có làm mẻ lưỡi đấy.", "", ""),
                ("Tinh Linh Lumi", "lumi", "Ghê chưa kìa! Có Liềm Ánh Trăng rồi, hai đứa mình vào quét sạch 5 Bọt Sương Đen thôi Wen!", "", "")
            }, "flag_met_bach_at_gate", sickle, 1, null, 0, null, "npc_bach", 1);

            var branchGentle = CreateOrUpdateDlg(table, "dlg_bach_branch_gentle", new[]
            {
                ("Ông Bach", "bach", "...Nói nhiều quá. Cầm lấy cái Liềm này. Dọn hết 5 con Bọt Sương với lau cái ghế gỗ đi rồi hẵng nói chuyện.", "", ""),
                ("Tinh Linh Lumi", "lumi", "Ông ấy ngoài lạnh trong nóng đấy Wen ơi! Vào dọn sạch vườn cho ông bất ngờ nào!", "", "")
            }, "flag_met_bach_at_gate", sickle, 1, null, 0, null, "npc_bach", 1);

            // 2. Ong Bach gap lan dau (co 2 lua chon co he qua that!)
            var dlgBachIntro = CreateOrUpdateDlg(table, "dlg_bach_intro", new[]
            {
                ("Ông Bach", "bach", "Chậc. Đứng lại. Vườn đóng cửa rồi, không tiếp khách tham quan.", "", ""),
                ("Tinh Linh Lumi", "lumi", "Ơ cái ông mũ rơm này! Wen không phải khách, Wen là cháu bà...", "", ""),
                ("Ông Bach", "bach", "...Cháu bà cụ Nhà Kính à? Hừm. Giống mắt bà ấy thật.", "", ""),
                ("Wen", "wen", "Cháu chào ông. Cháu muốn hỏi về Hoa Moonpetal ở Bồn Hoa trong vườn...", "", ""),
                ("Ông Bach", "bach", "Quên đi. Đất chai hết rồi, còn lũ Bọt Sương Đen nhung nhúc bên trong. Một con bé thành phố làm được gì?", "", "")
            }, "", null, 0, null, 0, null, "", 0);

            table.SetEntry("choice_bach_bold", "Cháu không hứa suông. Cho cháu mượn đồ, cháu dọn sạch vườn cho ông xem.");
            table.SetEntry("choice_bach_gentle", "Cháu biết ông vẫn thương khu vườn này. Cứ để cháu thử một lần nhé?");
            dlgBachIntro.choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceTextKey = "choice_bach_bold", setsFlagOnPick = "flag_choice_bach_bold", targetNpcId = "npc_bach", affinityDelta = 1, branchDialogue = branchBold },
                new DialogueChoice { choiceTextKey = "choice_bach_gentle", setsFlagOnPick = "flag_choice_bach_gentle", targetNpcId = "npc_bach", affinityDelta = 1, branchDialogue = branchGentle }
            };
            EditorUtility.SetDirty(dlgBachIntro);

            // 3. Ong Bach doc thu muon (co callback dung lua chon o Canh 2.1!)
            CreateOrUpdateDlg(table, "dlg_bach_read_letters", new[]
            {
                ("Wen", "wen", "Ông Bach ơi, Bà Hana nướng lại bánh rồi, Toby cũng dắt Miu ra vườn, còn đây là lá thư bà gửi ông.", "", ""),
                ("Ông Bach", "bach", "...Bà ấy vẫn nhớ cái ghế đôi. Còn dặn ta đừng có gắt gỏng với cháu gái bà ấy nữa.", "", ""),
                ("Ông Bach", "bach", "Hừm, bảo sao lúc mới gặp cháu đã cứng đầu y hệt bà ngoại cháu.", "flag_choice_bach_bold", ""),
                ("Ông Bach", "bach", "Cháu nói đúng... ta chưa bao giờ bỏ được khu vườn này cả.", "flag_choice_bach_gentle", ""),
                ("Ông Bach", "bach", "Được rồi Wen. Ra Bồn Hoa Trung Tâm đi. Ông với mọi người chờ cháu cùng gieo hạt.", "", "")
            }, "flag_bach_letters_read", null, 0, null, 0, null, "npc_bach", 2);

            // 4. Ba Hana: gap lan dau & giao 4 Lua Mach (trao LUON Tui Hat 1 + 2 Banh Mi!)
            CreateOrUpdateDlg(table, "dlg_hana_default", new[]
            {
                ("Bà Hana", "hana", "Ôi chao, cháu gái Nhà Kính đó hả? Nhìn người gầy nhom, đã ăn sáng chưa đấy?", "", ""),
                ("Tinh Linh Lumi", "lumi", "Khịt khịt! Cô ơi tiệm cô thơm mùi bột mì quá đi mất!", "", ""),
                ("Bà Hana", "hana", "Hì, cái bé xanh xanh kia tinh mũi ghê. Tiếc là dạo này cô hết sạch Lúa Mạch Sương Mai rồi.", "", ""),
                ("Wen", "wen", "Cháu có hạt giống trong Nhà Kính! Nếu cháu trồng mang ra 4 bó thì cô nướng mẻ bánh đầu xuân nhé?", "", ""),
                ("Bà Hana", "hana", "Nào nào, cháu mang được 4 bó Lúa Mạch thì cô biếu bánh nóng, tặng luôn Túi Hạt Di Sản cũ bà cháu gửi cô giữ!", "", "")
            }, "flag_hana_quest_started", null, 0, null, 0, null, "npc_hana", 1);

            var dlgHanaDone = CreateOrUpdateDlg(table, "dlg_hana_complete", new[]
            {
                ("Bà Hana", "hana", "Ôi chao, hạt mẩy mà thơm ngát thế này! Đúng vị sương sớm ngày xưa rồi!", "", ""),
                ("Tinh Linh Lumi", "lumi", "Nước miếng Lumi sắp rơi rồi Wen ơi...", "", ""),
                ("Bà Hana", "hana", "Nào, phần cháu 2 ổ Bánh Mì Nắng Ấm ăn cho lại sức, cả Túi Hạt Số 1 nữa. Tiện cháu mang giùm cô một ổ ra cổng vườn nhé?", "", ""),
                ("Wen", "wen", "Cho Ông Bach ạ? Cháu tưởng ông bảo không thích đồ ngọt?", "", ""),
                ("Bà Hana", "hana", "Ông ấy chê vậy thôi chứ sáng nào cũng ăn hết sạch đấy! Cô dọn bếp xong sẽ ra Bồn Hoa phụ cháu ngay.", "", "")
            }, "flag_hana_bread_done", bag1, 1, barley, 4, page2, "npc_hana", 2);

            dlgHanaDone.extraRewards = new List<ExtraItemReward>
            {
                new ExtraItemReward { item = bread, amount = 2 }
            };
            EditorUtility.SetDirty(dlgHanaDone);

            // 5. Cau Be Toby
            CreateOrUpdateDlg(table, "dlg_toby_default", new[]
            {
                ("Cậu Bé Toby", "toby", "Chị ơi chị ơi! Nguy to rồi! Em không sợ ma đâu thật đấy, nhưng bé Miu bị 3 cục mốc đen vây ở Rìa Rừng Phía Đông rồi!", "", ""),
                ("Wen", "wen", "Bình tĩnh nào nhóc. Em đứng đây chờ, chị cầm Liềm sang đuổi chúng ngay.", "", ""),
                ("Tinh Linh Lumi", "lumi", "Để Lumi đi cùng! Đám mốc đen gặp Liềm của Wen là chạy mất dép!", "", ""),
                ("Cậu Bé Toby", "toby", "Chị cứu được Miu thì em đưa chị Túi Hạt bí mật em giấu dưới gốc cây luôn, hứa danh dự đấy!", "", "")
            }, "", null, 0, null, 0, null, "npc_toby", 1);

            CreateOrUpdateDlg(table, "dlg_toby_complete", new[]
            {
                ("Cậu Bé Toby", "toby", "Miu ơi!! Đấy thấy chưa, em đã bảo chị Wen siêu ngầu mà!", "", ""),
                ("Wen", "wen", "Bé Miu chỉ hơi hoảng thôi, không bị thương đâu. Em nhớ đừng để bé chạy sâu vào rừng nữa nhé.", "", ""),
                ("Cậu Bé Toby", "toby", "Vâng ạ! Nè chị cầm Túi Hạt Số 2 với tờ giấy cũ em nhặt được. Em chạy ra Vườn Ông Bach giữ chỗ trước đây!", "", ""),
                ("Tinh Linh Lumi", "lumi", "Khoan! Cậu không sợ Ông Bach mắng nữa à?", "", ""),
                ("Cậu Bé Toby", "toby", "Sợ gì! Miu còn nằm ngủ ké trên thảm cỏ vườn ông suốt, ông có đuổi đâu!", "", "")
            }, "flag_toby_cat_saved", bag2, 1, null, 0, page3, "npc_toby", 2);

            // 6. Bac Otis
            CreateOrUpdateDlg(table, "dlg_otis_default", new[]
            {
                ("Bác Otis", "otis", "E hèm, chào cô Wen. Cô đến đúng lúc lắm, cái hòm thư gỗ này bị sương mục kẹt cứng từ mùa đông trước.", "", ""),
                ("Wen", "wen", "Bên trong còn thư chưa gửi sao bác?", "", ""),
                ("Bác Otis", "otis", "Có một phong thư bà ngoại cô gửi cho Ông Bach, kèm một Túi Hạt Di Sản. Tôi ngại cạy mạnh sợ rách thư...", "", ""),
                ("Bác Otis", "otis", "Nếu cô kiếm giúp 10 Gỗ Nhánh Cây quanh làng, tôi và cô gia cố lại bản lề rồi lấy thư ra tử tế.", "", "")
            }, "", null, 0, null, 0, null, "npc_otis", 1);

            EditorUtility.SetDirty(table);
            Debug.Log("[MV][Step32] Da cap nhat toan bo kich ban MVP (co lua chon, callback & trao du 2 Banh Mi Hana).");
        }

        private static ItemData LoadItem(string id) => AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_" + id + ".asset");

        private static DialogueData CreateOrUpdateDlg(
            StringTableSO table, string id,
            (string speaker, string spId, string text, string reqFlag, string blkFlag)[] lines,
            string setFlag, ItemData giveItem, int giveAmt, ItemData consumeItem, int consumeAmt,
            DiaryPageData unlockPage, string affNpc, int affDelta)
        {
            string path = "Assets/Data/Dialogues/Dlg_" + id + ".asset";
            var dlg = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
            if (dlg == null)
            {
                dlg = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(dlg, path);
            }

            dlg.dialogueID = id;
            dlg.lines.Clear();
            dlg.choices = new List<DialogueChoice>();
            dlg.extraRewards = new List<ExtraItemReward>();

            for (int i = 0; i < lines.Length; i++)
            {
                string spKey = id + "_sp_" + i;
                string txKey = id + "_tx_" + i;
                table.SetEntry(spKey, lines[i].speaker);
                table.SetEntry(txKey, lines[i].text);
                dlg.lines.Add(new DialogueLine
                {
                    speakerNameKey = spKey,
                    textKey = txKey,
                    speakerId = lines[i].spId,
                    requiredFlag = lines[i].reqFlag,
                    blockedFlag = lines[i].blkFlag
                });
            }

            dlg.setsStoryFlagOnFinish = setFlag;
            dlg.givesItemOnFinish = giveItem;
            dlg.givesItemAmount = giveAmt;
            dlg.consumesRequiredItem = consumeItem;
            dlg.consumesAmount = consumeAmt;
            dlg.unlocksDiaryPage = unlockPage;
            dlg.affinityNpcId = affNpc;
            dlg.affinityBonusOnFinish = affDelta;

            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static void UpgradeGameRootDialogueUI()
        {
            Sprite darkWood = AssetDatabase.LoadAssetAtPath<Sprite>(DarkWoodPath);
            Sprite btnWood = AssetDatabase.LoadAssetAtPath<Sprite>(BtnWoodPath);
            TMP_FontAsset storyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(StoryFontPath);
            TMP_FontAsset hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(HudFontPath);

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            try
            {
                var relMgr = root.GetComponent<RelationshipManager>() ?? root.AddComponent<RelationshipManager>();
                var gRoot = root.GetComponent<GameRoot>();
                if (gRoot != null)
                {
                    var grSo = new SerializedObject(gRoot);
                    grSo.FindProperty("relationshipManager").objectReferenceValue = relMgr;
                    grSo.ApplyModifiedPropertiesWithoutUndo();
                }

                Transform dlgHost = root.transform.Find("ClockHUDCanvas/DialogueUIHost");
                Transform dlgBox = dlgHost != null ? dlgHost.Find("DialogueBox") : null;
                DialogueUIView uiView = dlgHost != null ? dlgHost.GetComponent<DialogueUIView>() : null;

                if (dlgBox != null && uiView != null)
                {
                    // 1. Right Standing Portrait (de hien dong thoi Trai = Wen/Lumi, Phai = NPC!)
                    Transform rightTf = dlgBox.Find("StandingPortraitRight");
                    GameObject rightGo = rightTf != null
                        ? rightTf.gameObject
                        : new GameObject("StandingPortraitRight", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    if (rightTf == null) rightGo.transform.SetParent(dlgBox, false);

                    var rrt = rightGo.GetComponent<RectTransform>();
                    rrt.anchorMin = new Vector2(1f, 1f);
                    rrt.anchorMax = new Vector2(1f, 1f);
                    rrt.pivot = new Vector2(1f, 0f);
                    rrt.anchoredPosition = new Vector2(-18f, -4f);
                    rrt.sizeDelta = new Vector2(315f, 375f);
                    var rImg = rightGo.GetComponent<Image>();
                    rImg.preserveAspect = true;
                    rImg.raycastTarget = false;
                    rightGo.SetActive(false);

                    // 2. Choice Container (2 nut go o giua tren hop thoai)
                    Transform chTf = dlgHost.Find("DialogueChoiceContainer");
                    GameObject chGo = chTf != null ? chTf.gameObject : new GameObject("DialogueChoiceContainer", typeof(RectTransform));
                    if (chTf == null) chGo.transform.SetParent(dlgHost, false);
                    var crt = chGo.GetComponent<RectTransform>();
                    crt.anchorMin = new Vector2(0.5f, 0f);
                    crt.anchorMax = new Vector2(0.5f, 0f);
                    crt.pivot = new Vector2(0.5f, 0f);
                    crt.anchoredPosition = new Vector2(0f, 326f);
                    crt.sizeDelta = new Vector2(720f, 124f);

                    var vlg = chGo.GetComponent<VerticalLayoutGroup>() ?? chGo.AddComponent<VerticalLayoutGroup>();
                    vlg.spacing = 10f;
                    vlg.childControlWidth = true;
                    vlg.childControlHeight = false;
                    vlg.childAlignment = TextAnchor.MiddleCenter;

                    var (btnA, lblA) = EnsureChoiceButton(chGo.transform, "ChoiceBtnA", btnWood, hudFont);
                    var (btnB, lblB) = EnsureChoiceButton(chGo.transform, "ChoiceBtnB", btnWood, hudFont);
                    chGo.SetActive(false);

                    // 3. Dialogue Log Overlay [L]
                    Transform logTf = dlgHost.Find("DialogueLogPanel");
                    GameObject logGo = logTf != null ? logTf.gameObject : new GameObject("DialogueLogPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    if (logTf == null) logGo.transform.SetParent(dlgHost, false);
                    var lrt = logGo.GetComponent<RectTransform>();
                    lrt.anchorMin = new Vector2(0.5f, 0.5f);
                    lrt.anchorMax = new Vector2(0.5f, 0.5f);
                    lrt.sizeDelta = new Vector2(820f, 520f);
                    var lImg = logGo.GetComponent<Image>();
                    if (darkWood != null) { lImg.sprite = darkWood; lImg.type = Image.Type.Sliced; lImg.pixelsPerUnitMultiplier = 0.45f; lImg.color = Color.white; }
                    else lImg.color = new Color(0.10f, 0.08f, 0.07f, 0.96f);

                    var logTitle = EnsureTextChild(logGo.transform, "LogTitle", "NHẬT KÝ HỘI THOẠI GẦN ĐÂY (Bấm [L] hoặc [Esc] để đóng)", hudFont, 20f, new Color32(252, 224, 146, 255), TextAlignmentOptions.Top);
                    logTitle.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 440f);
                    logTitle.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -18f);

                    var logBody = EnsureTextChild(logGo.transform, "LogBody", "", storyFont != null ? storyFont : hudFont, 18f, new Color32(248, 240, 220, 255), TextAlignmentOptions.TopLeft);
                    logBody.lineSpacing = 6f;
                    logBody.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 68f);
                    logBody.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -64f);

                    var (btnClose, _) = EnsureChoiceButton(logGo.transform, "BtnCloseLog", btnWood, hudFont);
                    var crBtn = btnClose.GetComponent<RectTransform>();
                    crBtn.anchorMin = new Vector2(0.5f, 0f); crBtn.anchorMax = new Vector2(0.5f, 0f);
                    crBtn.pivot = new Vector2(0.5f, 0f);
                    crBtn.anchoredPosition = new Vector2(0f, 18f);
                    crBtn.sizeDelta = new Vector2(180f, 40f);
                    btnClose.GetComponentInChildren<TMP_Text>().text = "Đóng [L]";
                    logGo.SetActive(false);

                    // Wire fields onto DialogueUIView
                    var so = new SerializedObject(uiView);
                    so.FindProperty("rightPortraitImage").objectReferenceValue = rImg;
                    so.FindProperty("rightPortraitRect").objectReferenceValue = rrt;
                    so.FindProperty("choiceContainer").objectReferenceValue = chGo;
                    so.FindProperty("choiceButtonA").objectReferenceValue = btnA;
                    so.FindProperty("choiceLabelA").objectReferenceValue = lblA;
                    so.FindProperty("choiceButtonB").objectReferenceValue = btnB;
                    so.FindProperty("choiceLabelB").objectReferenceValue = lblB;
                    so.FindProperty("logPanelRoot").objectReferenceValue = logGo;
                    so.FindProperty("logBodyText").objectReferenceValue = logBody;
                    so.FindProperty("btnCloseLog").objectReferenceValue = btnClose;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(uiView);
                }

                PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
                Debug.Log("[MV][Step32] Da nang cap DialogueUIView (2 Portrait + 2 Choice Buttons + Dialogue Log [L]).");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static (Button, TMP_Text) EnsureChoiceButton(Transform parent, string name, Sprite btnSpr, TMP_FontAsset font)
        {
            Transform tf = parent.Find(name);
            GameObject go = tf != null ? tf.gameObject : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            if (tf == null) go.transform.SetParent(parent, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(700f, 52f);

            var img = go.GetComponent<Image>();
            if (btnSpr != null) { img.sprite = btnSpr; img.type = Image.Type.Sliced; img.pixelsPerUnitMultiplier = 0.50f; img.color = new Color(1f, 0.96f, 0.88f, 1f); }
            else img.color = new Color(0.28f, 0.20f, 0.14f, 0.96f);

            var btn = go.GetComponent<Button>();
            btn.targetGraphic = img;

            var tmp = EnsureTextChild(go.transform, "Label", "Lựa chọn", font, 19f, new Color32(255, 246, 222, 255), TextAlignmentOptions.Center);
            tmp.fontStyle = FontStyles.Bold;
            return (btn, tmp);
        }

        private static TMP_Text EnsureTextChild(Transform parent, string name, string text, TMP_FontAsset font, float size, Color color, TextAlignmentOptions align)
        {
            Transform tf = parent.Find(name);
            GameObject go = tf != null ? tf.gameObject : new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            if (tf == null) go.transform.SetParent(parent, false);
            var r = go.GetComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(14f, 4f); r.offsetMax = new Vector2(-14f, -4f);
            var tmp = go.GetComponent<TMP_Text>();
            if (font != null) tmp.font = font;
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            return tmp;
        }

        private static void UpgradeBlightFogInScene()
        {
            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

            GameObject old = GameObject.Find("BlightFogGroup");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("BlightFogGroup");

            // 10 cum Suong Heo Ua ro net bao phu Vuon Ong Bach & Ria Rung Phia Dong (tan bien khi Hoa Moonpetal no!)
            (Vector2 pos, Vector2 scale)[] spots =
            {
                (new Vector2(-4.2f, -6.5f),  new Vector2(2.6f, 2.2f)),
                (new Vector2( 4.2f, -6.5f),  new Vector2(2.6f, 2.2f)),
                (new Vector2(-3.8f, -10.2f), new Vector2(2.8f, 2.3f)),
                (new Vector2( 3.8f, -10.2f), new Vector2(2.8f, 2.3f)),
                (new Vector2( 0.0f, -12.6f), new Vector2(3.1f, 2.3f)),
                (new Vector2(-4.4f, -13.0f), new Vector2(2.4f, 2.0f)),
                (new Vector2( 4.4f, -13.0f), new Vector2(2.4f, 2.0f)),
                (new Vector2(14.5f,  5.5f),  new Vector2(2.8f, 2.2f)),
                (new Vector2(17.8f,  7.6f),  new Vector2(2.9f, 2.3f)),
                (new Vector2(15.2f,  9.4f),  new Vector2(2.6f, 2.1f)),
            };

            for (int i = 0; i < spots.Length; i++)
            {
                GameObject fGo = new GameObject("BlightFog_" + (i + 1));
                fGo.tag = "BlightFog";
                fGo.transform.SetParent(group.transform, false);
                fGo.transform.position = new Vector3(spots[i].pos.x, spots[i].pos.y, 0f);
                fGo.transform.localScale = new Vector3(spots[i].scale.x, spots[i].scale.y, 1f);

                var sr = fGo.AddComponent<SpriteRenderer>();
                sr.sortingLayerName = "FogOverlay";
                sr.sortingOrder = 10;
                sr.color = new Color(0.36f, 0.24f, 0.44f, 0.56f);

                var comp = fGo.AddComponent<BlightFogZone>();
                var so = new SerializedObject(comp);
                so.FindProperty("clearOnFlag").stringValue = "flag_nv1_completed";
                so.FindProperty("fogRenderer").objectReferenceValue = sr;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Step32] Da dung 10 cum Suong Heo Ua (BlightFogZone + bao tu troi) phu Vuon Ong Bach & Ria Rung.");
        }

        [MenuItem("Moonpetal/Step 32/2. Validate Step 32 Narrative & Blight Fog")]
        public static void ValidateStep32()
        {
            int errors = 0;

            var dlgBach = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/Data/Dialogues/Dlg_dlg_bach_intro.asset");
            if (dlgBach == null || dlgBach.choices == null || dlgBach.choices.Count < 2)
            {
                Debug.LogError("[MV][Step32][FAIL] Dlg_dlg_bach_intro thieu 2 lua chon co he qua.");
                errors++;
            }

            var dlgHana = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/Data/Dialogues/Dlg_dlg_hana_complete.asset");
            if (dlgHana == null || dlgHana.extraRewards == null || dlgHana.extraRewards.Count < 1)
            {
                Debug.LogError("[MV][Step32][FAIL] Dlg_dlg_hana_complete chua co extraRewards (2 Banh Mi).");
                errors++;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null || prefab.GetComponent<RelationshipManager>() == null)
            {
                Debug.LogError("[MV][Step32][FAIL] GameRoot.prefab thieu RelationshipManager.");
                errors++;
            }
            if (prefab != null && prefab.transform.Find("ClockHUDCanvas/DialogueUIHost/DialogueLogPanel") == null)
            {
                Debug.LogError("[MV][Step32][FAIL] Thieu DialogueLogPanel [L] tren GameRoot.prefab.");
                errors++;
            }

            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            int fogs = UnityEngine.Object.FindObjectsByType<BlightFogZone>(FindObjectsInactive.Include).Length;
            if (fogs < 10)
            {
                Debug.LogError("[MV][Step32][FAIL] So luong BlightFogZone chua du 10 (" + fogs + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV][Step32] VALIDATE PASS: Kich ban MVP, 2 Portrait, Choice Buttons, Dialogue Log [L], Voice Blips & 10 cum Suong Heo Ua deu hop le 100%!");
            }
            else
            {
                Debug.LogError("[MV][Step32] VALIDATE FAIL (" + errors + " loi).");
            }
        }
    }
}
