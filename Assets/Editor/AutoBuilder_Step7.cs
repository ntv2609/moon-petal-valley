using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step7
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 7/1. Build NPCs, Dialogues, Diary & Quests")]
        public static void BuildStep7()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 7 ===");

            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            List<DiaryPageData> diaryPages = Build4DiaryPages(table);
            var npcs = Build4NPCsAndDialogues(table, diaryPages);

            if (table != null) EditorUtility.SetDirty(table);

            UpgradeGameRootWithNarrativeUI(diaryPages);

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildNPCsInScene(npcs);
            BuildStoryPropsInScene();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 7! Chạy 'Moonpetal/Step 7/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 7/Validate")]
        public static void ValidateStep7()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 7 ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                string[] fields = { "dialogueManager", "diaryManager", "questManager" };
                foreach (string f in fields)
                {
                    if (rootSO.FindProperty(f).objectReferenceValue == null)
                    {
                        Debug.LogError("[MV] GameRoot thiếu: " + f);
                        errors++;
                    }
                }
            }

            var npcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            if (npcs.Length < 4)
            {
                Debug.LogError("[MV] Thiếu NPC trong scene (cần 4, hiện có " + npcs.Length + ").");
                errors++;
            }

            var props = UnityEngine.Object.FindObjectsByType<StoryPropInteractable>(FindObjectsInactive.Include);
            if (props.Length < 5)
            {
                Debug.LogError("[MV] Thiếu StoryPropInteractable trong scene (cần 5, hiện có " + props.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 7: PASS (0 lỗi). 4 NPC, Hội thoại phân nhánh, Sổ Tay 4 trang & 5 StoryProp sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 7: FAIL (" + errors + " lỗi).");
            }
        }

        private static List<DiaryPageData> Build4DiaryPages(StringTableSO table)
        {
            var list = new List<DiaryPageData>();
            list.Add(CreateDiaryPage(table, "diary_page_01", 1, "flag_lumi_awakened",
                "Trang 1 — Lời Dặn Trong Nhà Kính",
                "Wen yêu quý, Hoa Ánh Trăng không bao giờ nở vì một đôi tay đơn độc. Khi sương héo úa phủ lên thung lũng, hãy tìm lại những người hàng xóm xưa và lắng nghe câu chuyện của họ."));
            list.Add(CreateDiaryPage(table, "diary_page_02", 2, "flag_hana_bread_done",
                "Trang 2 — Mẻ Bánh Đầu Xuân Của Hana",
                "Mùa xuân năm ấy, cả làng tụ họp ở vườn ông Bách chia nhau những ổ Bánh Mì Nắng Ấm nóng hổi từ lúa mạch sương mai. Hương bánh làm tan đi cả cái lạnh đầu mùa."));
            list.Add(CreateDiaryPage(table, "diary_page_03", 3, "flag_toby_cat_saved",
                "Trang 3 — Tiếng Cười Quanh Vườn Phía Nam",
                "Ông Bách tuy ít nói nhưng luôn để dành một góc vườn cho lũ trẻ chạy nhảy và lũ mèo nằm sưởi nắng. Chỉ cần có người nhớ đến khu vườn, ông sẽ lại cầm cuốc."));
            list.Add(CreateDiaryPage(table, "diary_page_04", 4, "flag_otis_mailbox_fixed",
                "Trang 4 — Bức Thư Chưa Kịp Gửi Ông Bách",
                "Gửi ông bạn già Bách, cảm ơn ông đã giữ gìn chiếc ghế đôi và những hạt giống xưa. Nếu cháu gái tôi Wen tìm đến, xin ông hãy một lần nữa cùng cháu gieo mầm hy vọng nhé."));
            return list;
        }

        private static DiaryPageData CreateDiaryPage(StringTableSO table, string id, int num, string flag, string title, string body)
        {
            string path = "Assets/Data/Diary/DiaryPage_0" + num + ".asset";
            var page = AssetDatabase.LoadAssetAtPath<DiaryPageData>(path);
            if (page == null)
            {
                page = ScriptableObject.CreateInstance<DiaryPageData>();
                AssetDatabase.CreateAsset(page, path);
            }
            page.pageID = id;
            page.pageNumber = num;
            page.titleKey = "title_" + id;
            page.bodyTextKey = "body_" + id;
            page.unlockedByFlag = flag;

            table?.SetEntry(page.titleKey, title);
            table?.SetEntry(page.bodyTextKey, body);
            EditorUtility.SetDirty(page);
            return page;
        }

        private static Dictionary<string, NPCData> Build4NPCsAndDialogues(StringTableSO table, List<DiaryPageData> pages)
        {
            var result = new Dictionary<string, NPCData>();

            ItemData sickle = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_moon_sickle.asset");
            ItemData barley = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley.asset");
            ItemData bread = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_sunshine_bread.asset");
            ItemData bag1 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_seed_bag_1.asset");
            ItemData bag2 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_seed_bag_2.asset");

            // 1. ÔNG BÁCH
            var dlgBachIntro = CreateDialogue(table, "dlg_bach_intro",
                new[] { ("Ông Bách", "Cháu là Wen, cháu gái của bà cụ Nhà Kính phải không? Khu vườn phía nam này bị Bọt Sương Đen chiếm mất rồi..."),
                        ("Ông Bách", "Cầm lấy chiếc Liềm Ánh Trăng cũ này (ô số 4). Nếu cháu thanh tẩy hết 5 Bọt Sương Đen và lau sạch Chiếc Ghế Đôi, ta sẽ tin thung lũng này còn hy vọng.") },
                "flag_met_bach_at_gate", sickle, 1, null, 0, null);

            var dlgBachReadLetters = CreateDialogue(table, "dlg_bach_read_letters",
                new[] { ("Wen", "Ông Bách ơi, cháu đã tìm lại đủ 4 trang nhật ký và bức thư cuối bà gửi cho ông..."),
                        ("Ông Bách", "Bà ấy vẫn luôn nhớ chiếc ghế đôi này sao... Cảm ơn cháu, Wen. Ta sẽ đứng cạnh Bồn Hoa chờ mọi người cùng gieo hạt!") },
                "flag_bach_letters_read", null, 0, null, 0, null);

            var dlgBachDefault = CreateDialogue(table, "dlg_bach_default",
                new[] { ("Ông Bách", "Khu vườn này cần sự chung tay của cả làng. Cháu hãy giúp Bà Hana, cậu bé Toby và Bác Otis tìm đủ 3 Túi Hạt Di Sản nhé.") },
                "", null, 0, null, 0, null);

            result["npc_bach"] = CreateNPC(table, "npc_bach", "Ông Bách", "Người Làm Vườn Già",
                new Color(0.45f, 0.36f, 0.28f), new Color(0.86f, 0.86f, 0.90f),
                dlgBachDefault, "flag_bach_letters_read",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_tutorial_done", blockedByStoryFlag = "flag_met_bach_at_gate", dialogue = dlgBachIntro },
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_garden_bench_cleaned", blockedByStoryFlag = "flag_bach_letters_read", dialogue = dlgBachReadLetters }
                });

            // 2. BÀ HANA
            var dlgHanaComplete = CreateDialogue(table, "dlg_hana_complete",
                new[] { ("Bà Hana", "Ôi, 4 bó Lúa Mạch Sương Mai tươi quá! Mẻ bánh đầu xuân lại thơm lừng cả quảng trường rồi!"),
                        ("Bà Hana", "Tặng cháu 2 ổ Bánh Mì Nắng Ấm (ăn vào hồi lực & xóa mệt mỏi ngay) cùng Túi Hạt Di Sản Số 1 nhé. Cô ra Bồn Hoa chờ cháu!") },
                "flag_hana_bread_done", bag1, 1, barley, 4, pages[1]);

            var dlgHanaGiveBreadBonus = CreateDialogue(table, "dlg_hana_after",
                new[] { ("Bà Hana", "Cô đã sẵn sàng cùng cháu và Ông Bách gieo hạt tại Bồn Hoa Trung Tâm Vườn rồi đó!") },
                "", bread, 2, null, 0, null);

            var dlgHanaDefault = CreateDialogue(table, "dlg_hana_default",
                new[] { ("Bà Hana", "Chào Wen! Nếu cháu trồng được 4 cây Lúa Mạch Sương Mai trong Nhà Kính mang tới đây, cô sẽ nướng Bánh Mì Nắng Ấm và trao Túi Hạt Di Sản Số 1 cho cháu.") },
                "flag_hana_quest_started", null, 0, null, 0, null);

            result["npc_hana"] = CreateNPC(table, "npc_hana", "Bà Hana", "Chủ Tiệm Bánh",
                new Color(0.86f, 0.44f, 0.40f), new Color(0.98f, 0.92f, 0.80f),
                dlgHanaDefault, "flag_hana_bread_done",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_met_bach_at_gate", blockedByStoryFlag = "flag_hana_bread_done", dialogue = dlgHanaComplete },
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_hana_bread_done", blockedByStoryFlag = "flag_hana_bread_bonus_taken", dialogue = dlgHanaGiveBreadBonus }
                });
            dlgHanaGiveBreadBonus.setsStoryFlagOnFinish = "flag_hana_bread_bonus_taken";
            EditorUtility.SetDirty(dlgHanaGiveBreadBonus);

            // 3. CẬU BÉ TOBY
            var dlgTobyComplete = CreateDialogue(table, "dlg_toby_complete",
                new[] { ("Cậu Bé Toby", "Meo meo! Chị Wen đuổi hết 3 Bọt Sương Đen cứu bé Miu rồi! Em cảm ơn chị nhiều lắm!"),
                        ("Cậu Bé Toby", "Em gửi chị Túi Hạt Di Sản Số 2 và trang sổ em nhặt được. Em chạy ra Bồn Hoa phụ chị ngay đây!") },
                "flag_toby_cat_saved", bag2, 1, null, 0, pages[2]);

            var dlgTobyDefault = CreateDialogue(table, "dlg_toby_default",
                new[] { ("Cậu Bé Toby", "Chị Wen ơi, chú mèo Miu của em bị 3 con Bọt Sương Đen vây ở Rìa Rừng Phía Đông rồi! Chị cầm Liềm Ánh Trăng cứu bé giúp em với!") },
                "", null, 0, null, 0, null);

            result["npc_toby"] = CreateNPC(table, "npc_toby", "Cậu Bé Toby", "Cư Dân Làng",
                new Color(0.28f, 0.68f, 0.46f), new Color(0.95f, 0.78f, 0.36f),
                dlgTobyDefault, "flag_toby_cat_saved",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_toby_cat_slimes_cleared", blockedByStoryFlag = "flag_toby_cat_saved", dialogue = dlgTobyComplete }
                });

            // 4. BÁC OTIS
            var dlgOtisDone = CreateDialogue(table, "dlg_otis_done",
                new[] { ("Bác Otis", "Hòm thư chắc chắn như mới rồi! Cảm ơn cháu Wen, bác ra đứng cạnh Bồn Hoa chờ mọi người cùng gieo hạt đây!") },
                "", null, 0, null, 0, null);

            var dlgOtisDefault = CreateDialogue(table, "dlg_otis_default",
                new[] { ("Bác Otis", "Hòm thư gỗ ngay cạnh bác bị sương mục hỏng mất rồi. Cháu chặt 10 Gỗ Nhánh Cây quanh làng rồi bấm [E] vào Hòm Thư để sửa giúp bác nhé, bên trong có Túi Hạt Di Sản Số 3 và thư của bà cháu đấy!") },
                "", null, 0, null, 0, null);

            result["npc_otis"] = CreateNPC(table, "npc_otis", "Bác Otis", "Người Đưa Thư",
                new Color(0.26f, 0.46f, 0.76f), new Color(0.92f, 0.84f, 0.50f),
                dlgOtisDefault, "flag_otis_mailbox_fixed",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_otis_mailbox_fixed", blockedByStoryFlag = "", dialogue = dlgOtisDone }
                });

            return result;
        }

        private static DialogueData CreateDialogue(
            StringTableSO table, string id, (string speaker, string text)[] lines,
            string setFlag, ItemData giveItem, int giveAmt, ItemData consumeItem, int consumeAmt, DiaryPageData unlockPage)
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
            for (int i = 0; i < lines.Length; i++)
            {
                string spKey = id + "_sp_" + i;
                string txKey = id + "_tx_" + i;
                table?.SetEntry(spKey, lines[i].speaker);
                table?.SetEntry(txKey, lines[i].text);
                dlg.lines.Add(new DialogueLine { speakerNameKey = spKey, textKey = txKey });
            }

            dlg.setsStoryFlagOnFinish = setFlag;
            dlg.givesItemOnFinish = giveItem;
            dlg.givesItemAmount = giveAmt;
            dlg.consumesRequiredItem = consumeItem;
            dlg.consumesAmount = consumeAmt;
            dlg.unlocksDiaryPage = unlockPage;
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static NPCData CreateNPC(
            StringTableSO table, string id, string vnName, string role,
            Color coatColor, Color hairColor, DialogueData defaultDlg, string helperFlag,
            List<ConditionalDialogueEntry> conditionals)
        {
            Sprite spr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_" + id + ".png", 16, 24, tex =>
            {
                PlaceholderGeneratorUtility.FillRect(tex, 5, 1, 2, 3, new Color(0.24f, 0.16f, 0.12f));
                PlaceholderGeneratorUtility.FillRect(tex, 9, 1, 2, 3, new Color(0.24f, 0.16f, 0.12f));
                PlaceholderGeneratorUtility.FillRect(tex, 4, 4, 8, 9, coatColor);
                PlaceholderGeneratorUtility.FillRect(tex, 4, 13, 8, 6, new Color(0.96f, 0.84f, 0.72f));
                tex.SetPixel(6, 15, Color.black);
                tex.SetPixel(9, 15, Color.black);
                PlaceholderGeneratorUtility.FillRect(tex, 3, 18, 10, 3, hairColor);
            });

            string path = "Assets/Data/NPCs/NPC_" + id + ".asset";
            var npc = AssetDatabase.LoadAssetAtPath<NPCData>(path);
            if (npc == null)
            {
                npc = ScriptableObject.CreateInstance<NPCData>();
                AssetDatabase.CreateAsset(npc, path);
            }
            npc.npcID = id;
            npc.displayNameKey = "name_" + id;
            npc.roleTitleKey = "role_" + id;
            npc.worldSprite = spr;
            npc.defaultDialogue = defaultDlg;
            npc.conditionalDialogues = conditionals;
            npc.isHelperForBundleID = "bundle_forgotten_garden";
            npc.helperReadyStoryFlag = helperFlag;

            table?.SetEntry(npc.displayNameKey, vnName);
            table?.SetEntry(npc.roleTitleKey, role);
            EditorUtility.SetDirty(npc);
            return npc;
        }

        private static void UpgradeGameRootWithNarrativeUI(List<DiaryPageData> diaryPages)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var dlgMgr = root.GetComponent<DialogueManager>() ?? root.AddComponent<DialogueManager>();
            var diaryMgr = root.GetComponent<DiaryManager>() ?? root.AddComponent<DiaryManager>();
            var questMgr = root.GetComponent<QuestManager>() ?? root.AddComponent<QuestManager>();

            var dSO = new SerializedObject(diaryMgr);
            var pagesProp = dSO.FindProperty("allPages");
            pagesProp.ClearArray();
            for (int i = 0; i < diaryPages.Count; i++)
            {
                pagesProp.InsertArrayElementAtIndex(i);
                pagesProp.GetArrayElementAtIndex(i).objectReferenceValue = diaryPages[i];
            }
            dSO.ApplyModifiedPropertiesWithoutUndo();

            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                // 1. Quest Tracker Panel (Top-Right below Clock)
                Transform oldQuest = hudCanvas.Find("QuestTrackerPanel");
                if (oldQuest != null) UnityEngine.Object.DestroyImmediate(oldQuest.gameObject);
                GameObject qPanel = CreatePanel("QuestTrackerPanel", hudCanvas, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -92f), new Vector2(380f, 128f), new Color(0.08f, 0.12f, 0.19f, 0.84f));
                qPanel.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
                var qTmp = CreateTMP("ObjectiveText", qPanel.transform, "Mục tiêu...", vnFont, 16, TextAlignmentOptions.TopLeft, Color.white);
                var qView = qPanel.AddComponent<QuestTrackerHUDView>();
                var qSO = new SerializedObject(qView);
                qSO.FindProperty("objectiveText").objectReferenceValue = qTmp;
                qSO.ApplyModifiedPropertiesWithoutUndo();

                // 2. Dialogue Box (Bottom-Center)
                Transform oldDlg = hudCanvas.Find("DialogueUIHost");
                if (oldDlg != null) UnityEngine.Object.DestroyImmediate(oldDlg.gameObject);
                GameObject dlgHost = new GameObject("DialogueUIHost");
                dlgHost.transform.SetParent(hudCanvas, false);
                var dhRect = dlgHost.AddComponent<RectTransform>();
                dhRect.anchorMin = Vector2.zero; dhRect.anchorMax = Vector2.one; dhRect.offsetMin = Vector2.zero; dhRect.offsetMax = Vector2.zero;

                GameObject dlgBox = CreatePanel("DialogueBox", dlgHost.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(980f, 190f), new Color(0.07f, 0.10f, 0.17f, 0.95f));
                dlgBox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                var spTxt = CreateTMP("SpeakerText", dlgBox.transform, "Nhân vật", vnFont, 24, TextAlignmentOptions.TopLeft, new Color(1f, 0.88f, 0.45f, 1f));
                spTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 130f); spTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -14f);
                var bdTxt = CreateTMP("BodyText", dlgBox.transform, "Nội dung...", vnFont, 21, TextAlignmentOptions.TopLeft, Color.white);
                bdTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 44f); bdTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -54f);
                var hintTxt = CreateTMP("ContinueHint", dlgBox.transform, "[E / Space] Tiếp tục", vnFont, 17, TextAlignmentOptions.BottomRight, new Color(0.70f, 0.92f, 0.96f, 1f));
                hintTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 12f); hintTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -140f);

                var dlgView = dlgHost.AddComponent<DialogueUIView>();
                var dvSO = new SerializedObject(dlgView);
                dvSO.FindProperty("panelRoot").objectReferenceValue = dlgBox;
                dvSO.FindProperty("speakerText").objectReferenceValue = spTxt;
                dvSO.FindProperty("bodyText").objectReferenceValue = bdTxt;
                dvSO.FindProperty("continueHintText").objectReferenceValue = hintTxt;
                dvSO.ApplyModifiedPropertiesWithoutUndo();

                var dmSO = new SerializedObject(dlgMgr);
                dmSO.FindProperty("dialogueUI").objectReferenceValue = dlgView;
                dmSO.ApplyModifiedPropertiesWithoutUndo();

                // 3. Diary UI Modal (Key B)
                Transform oldDiary = hudCanvas.Find("DiaryUIHost");
                if (oldDiary != null) UnityEngine.Object.DestroyImmediate(oldDiary.gameObject);
                GameObject diaryHost = new GameObject("DiaryUIHost");
                diaryHost.transform.SetParent(hudCanvas, false);
                var diRect = diaryHost.AddComponent<RectTransform>();
                diRect.anchorMin = Vector2.zero; diRect.anchorMax = Vector2.one; diRect.offsetMin = Vector2.zero; diRect.offsetMax = Vector2.zero;

                GameObject diaryBox = CreatePanel("DiaryBox", diaryHost.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 420f), new Color(0.16f, 0.13f, 0.10f, 0.96f));
                var pHead = CreateTMP("Header", diaryBox.transform, "SỔ TAY CỦA BÀ", vnFont, 20, TextAlignmentOptions.Top, new Color(0.95f, 0.82f, 0.52f, 1f));
                pHead.GetComponent<RectTransform>().offsetMax = new Vector2(-20f, -16f);
                var pTitle = CreateTMP("Title", diaryBox.transform, "Tiêu đề", vnFont, 25, TextAlignmentOptions.TopLeft, Color.white);
                pTitle.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 280f); pTitle.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -64f);
                var pBody = CreateTMP("Body", diaryBox.transform, "Nội dung", vnFont, 20, TextAlignmentOptions.TopLeft, new Color(0.96f, 0.92f, 0.84f, 1f));
                pBody.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 80f); pBody.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -112f);

                Button btnPrev = CreateModalButton("BtnPrev", diaryBox.transform, "< Trang Trước", new Vector2(-200f, 36f), vnFont);
                Button btnClose = CreateModalButton("BtnClose", diaryBox.transform, "Đóng [B]", new Vector2(0f, 36f), vnFont);
                Button btnNext = CreateModalButton("BtnNext", diaryBox.transform, "Trang Sau >", new Vector2(200f, 36f), vnFont);

                var diaryView = diaryHost.AddComponent<DiaryUIView>();
                var diSO = new SerializedObject(diaryView);
                diSO.FindProperty("panelRoot").objectReferenceValue = diaryBox;
                diSO.FindProperty("pageHeaderText").objectReferenceValue = pHead;
                diSO.FindProperty("pageTitleText").objectReferenceValue = pTitle;
                diSO.FindProperty("pageBodyText").objectReferenceValue = pBody;
                diSO.FindProperty("btnPrev").objectReferenceValue = btnPrev;
                diSO.FindProperty("btnNext").objectReferenceValue = btnNext;
                diSO.FindProperty("btnClose").objectReferenceValue = btnClose;
                diSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // Nối GameRoot
            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("dialogueManager").objectReferenceValue = dlgMgr;
            rootSO.FindProperty("diaryManager").objectReferenceValue = diaryMgr;
            rootSO.FindProperty("questManager").objectReferenceValue = questMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // Nút Debug Bỏ qua Mở Đầu
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 920f);
                Transform ex = debugPanel.Find("BtnSkipPrologue");
                Button btnSkip = ex != null ? ex.GetComponent<Button>() : CreateDebugButton(debugPanel, "BtnSkipPrologue", "Bỏ qua Mở Đầu (Vào NV1 ngay)", vnFont);
                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnSkipPrologue").objectReferenceValue = btnSkip;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildNPCsInScene(Dictionary<string, NPCData> npcs)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject old = GameObject.Find("VillageNPCs");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("VillageNPCs");

            SpawnNPC(group.transform, "NPC_Bach", npcs["npc_bach"], new Vector2(0.0f, -3.5f), new Vector2(-1.6f, -8.8f), "[E] Nói chuyện: Ông Bách", vnFont);
            SpawnNPC(group.transform, "NPC_Hana", npcs["npc_hana"], new Vector2(-6.0f, 3.0f), new Vector2(1.8f, -8.8f), "[E] Nói chuyện: Bà Hana", vnFont);
            SpawnNPC(group.transform, "NPC_Toby", npcs["npc_toby"], new Vector2(7.5f, 2.5f), new Vector2(-1.6f, -11.2f), "[E] Nói chuyện: Cậu Bé Toby", vnFont);
            SpawnNPC(group.transform, "NPC_Otis", npcs["npc_otis"], new Vector2(1.2f, 5.5f), new Vector2(1.8f, -11.2f), "[E] Nói chuyện: Bác Otis", vnFont);
        }

        private static void SpawnNPC(Transform parent, string name, NPCData data, Vector2 pos, Vector2 helperPos, string promptText, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.tag = "NPC";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = data.worldSprite;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);

            go.AddComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.7f);
            GameObject prompt = CreateWorldPrompt(go.transform, promptText, font);

            var ctrl = go.AddComponent<NPCController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("npcData").objectReferenceValue = data;
            so.FindProperty("promptObject").objectReferenceValue = prompt;
            so.FindProperty("interactRadius").floatValue = 2.1f;
            so.FindProperty("helperStandPosition").vector2Value = helperPos;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildStoryPropsInScene()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject old = GameObject.Find("StoryPropsGroup");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("StoryPropsGroup");

            Sprite tableSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_table.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 10, new Color(0.54f, 0.36f, 0.20f));
                PlaceholderGeneratorUtility.FillRect(t, 5, 6, 6, 5, new Color(0.98f, 0.95f, 0.85f));
            });
            Sprite boxSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_seedbox.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 10, new Color(0.42f, 0.28f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 6, 6, 4, 4, new Color(0.45f, 0.94f, 0.96f));
            });
            Sprite planterSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_planter.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 2, 12, 12, new Color(0.56f, 0.56f, 0.60f));
                PlaceholderGeneratorUtility.FillRect(t, 4, 4, 8, 8, new Color(0.34f, 0.24f, 0.18f));
            });
            Sprite mailboxSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_mailbox.png", 16, 20, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 7, 1, 2, 10, new Color(0.46f, 0.30f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 3, 10, 10, 7, new Color(0.28f, 0.48f, 0.76f));
                PlaceholderGeneratorUtility.FillRect(t, 12, 13, 2, 4, new Color(0.92f, 0.32f, 0.28f));
            });
            Sprite benchSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_bench.png", 24, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 4, 20, 4, new Color(0.56f, 0.38f, 0.22f));
                PlaceholderGeneratorUtility.FillRect(t, 2, 9, 20, 4, new Color(0.46f, 0.30f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 4, 1, 2, 12, new Color(0.32f, 0.22f, 0.14f));
                PlaceholderGeneratorUtility.FillRect(t, 18, 1, 2, 12, new Color(0.32f, 0.22f, 0.14f));
            });

            CreateStoryProp(group.transform, "GreenhouseLetterTable", "prop_letter_table", new Vector2(-15.5f, 12.5f), tableSpr,
                "[E] Đọc Thư Của Bà", "", "flag_intro_letter_read", "", "", 0, "", 0, 0,
                "Thư Của Bà", "Wen thân yêu, hãy dọn dẹp 3 cụm cỏ dại đá vụn trong Nhà Kính rồi mở Chiếc Hộp Gỗ Của Bà nhé!", "", vnFont);

            CreateStoryProp(group.transform, "GreenhouseSeedBox", "prop_seed_box", new Vector2(-17.2f, 12.5f), boxSpr,
                "[E] Mở Hộp Gỗ Hạt Giống", "flag_greenhouse_cleared", "flag_lumi_awakened", "", "", 0, "item_moonpetal_seed_core", 1, 0,
                "Tinh Linh Lumi", "Oa! Sương lạnh ngoài kia dày quá... Cảm ơn Wen đã đánh thức Lumi! Mình đã mở Trang 1 Sổ Tay Của Bà (bấm [B] để đọc) và tặng bạn 6 Hạt Lúa Mạch Sương Mai!",
                "Hãy dọn sạch 3 cụm cỏ dại/đá vụn trong Nhà Kính trước!", vnFont);

            CreateStoryProp(group.transform, "GreenhouseTestPlanter", "prop_test_planter", new Vector2(-13.8f, 11.2f), planterSpr,
                "[E] Gieo Thử Hạt Giống", "flag_lumi_awakened", "flag_solo_plant_failed", "flag_tutorial_done", "", 0, "", 0, 0,
                "Tinh Linh Lumi", "Hạt giống vừa chạm đất đã bị sương xám phủ kín... Đúng như lời bà dặn: một mình không thể làm hoa nở. Hãy ra Quảng Trường Làng Chính tìm Ông Bách nhé!",
                "Hãy mở Hộp Gỗ Của Bà đánh thức Lumi trước!", vnFont);

            CreateStoryProp(group.transform, "OtisBrokenMailbox", "prop_otis_mailbox", new Vector2(-0.5f, 5.5f), mailboxSpr,
                "[E] Sửa Hòm Thư (10 Gỗ)", "flag_met_bach_at_gate", "flag_otis_mailbox_fixed", "", "item_branch_wood", 10, "item_seed_bag_3", 1, 4,
                "Bác Otis", "Tuyệt vời! Hòm thư sửa xong rồi! Cháu nhận lấy Túi Hạt Di Sản Số 3 và Trang Sổ 4 (bức thư bà cháu chưa kịp gửi Ông Bách) nhé!",
                "Cần gặp Ông Bách trước và có đủ 10 Gỗ Nhánh Cây!", vnFont);

            CreateStoryProp(group.transform, "GardenMemoryBench", "prop_memory_bench", new Vector2(3.0f, -7.2f), benchSpr,
                "[E] Lau Sạch Chiếc Ghế Đôi", "flag_garden_slimes_cleared", "flag_garden_bench_cleaned", "", "", 0, "", 0, 2,
                "Wen", "Chiếc ghế đôi kỷ niệm của Ông Bách đã sạch bóng rêu phong! Giờ mình sẽ giúp Bà Hana, Toby và Bác Otis gom đủ 3 Túi Hạt Di Sản.",
                "Hãy thanh tẩy hết 5 Bọt Sương Đen trong Vườn Ông Bách trước!", vnFont);
        }

        private static void CreateStoryProp(
            Transform parent, string name, string id, Vector2 pos, Sprite spr, string promptLabel,
            string reqFlag, string doneFlag, string doneFlag2, string reqItem, int reqAmt, string rewItem, int rewAmt, int stamCost,
            string speaker, string body, string blocked, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.tag = "Interactable";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr; sr.sortingLayerName = "WorldObjects"; sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);
            go.AddComponent<BoxCollider2D>().size = new Vector2(0.95f, 0.85f);

            GameObject prompt = CreateWorldPrompt(go.transform, promptLabel, font);
            var comp = go.AddComponent<StoryPropInteractable>();
            var so = new SerializedObject(comp);
            so.FindProperty("propId").stringValue = id;
            so.FindProperty("requiredFlag").stringValue = reqFlag;
            so.FindProperty("completedFlag").stringValue = doneFlag;
            so.FindProperty("secondaryCompletedFlag").stringValue = doneFlag2;
            so.FindProperty("requiredItemId").stringValue = reqItem;
            so.FindProperty("requiredItemAmount").intValue = reqAmt;
            so.FindProperty("rewardItemId").stringValue = rewItem;
            so.FindProperty("rewardItemAmount").intValue = rewAmt;
            so.FindProperty("staminaCost").intValue = stamCost;
            so.FindProperty("dialogueSpeaker").stringValue = speaker;
            so.FindProperty("dialogueBody").stringValue = body;
            so.FindProperty("blockedHint").stringValue = blocked;
            so.FindProperty("promptObject").objectReferenceValue = prompt;
            so.FindProperty("interactRadius").floatValue = 1.95f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 aMin, Vector2 aMax, Vector2 pos, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = aMin; r.anchorMax = aMax; r.anchoredPosition = pos; r.sizeDelta = size;
            go.AddComponent<Image>().color = color;
            return go;
        }

        private static TMP_Text CreateTMP(string name, Transform parent, string text, TMP_FontAsset font, float size, TextAlignmentOptions align, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(12f, 8f); r.offsetMax = new Vector2(-12f, -8f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = align; tmp.color = color;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateModalButton(string name, Transform parent, string label, Vector2 anchoredPos, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0f); r.anchorMax = new Vector2(0.5f, 0f);
            r.anchoredPosition = anchoredPos; r.sizeDelta = new Vector2(170f, 40f);
            var img = go.AddComponent<Image>(); img.color = new Color(0.32f, 0.25f, 0.18f, 1f);
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 17, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static Button CreateDebugButton(Transform parent, string name, string label, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
            var img = go.AddComponent<Image>(); img.color = new Color(0.42f, 0.34f, 0.18f, 1f);
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 15, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static GameObject CreateWorldPrompt(Transform parent, string label, TMP_FontAsset font)
        {
            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(parent, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace; canvas.sortingLayerName = "UI"; canvas.sortingOrder = 50;
            promptGo.GetComponent<RectTransform>().sizeDelta = new Vector2(280f, 44f);
            promptGo.AddComponent<Image>().color = new Color(0.08f, 0.12f, 0.18f, 0.90f);
            var tmp = CreateTMP("Text", promptGo.transform, label, font, 20, TextAlignmentOptions.Center, new Color(1f, 0.94f, 0.65f, 1f));
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return promptGo;
        }
    }
}
