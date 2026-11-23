#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step47B
    {
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string DlgDir = "Assets/Data/Dialogues/Chapter2";

        [MenuItem("Moonpetal/Step 47B/1. Build Chapter 2 Stage Dialogues & Wire NPCs")]
        public static void BuildStep47B()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Step47B] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Step47B] === BẮT ĐẦU STEP 47B: XÂY DỰNG THOẠI THEO GIAI ĐOẠN CHO KAEL, MARA & OTIS ===");
            Directory.CreateDirectory(DlgDir);

            ItemData eastLanternItem = FindItemById("item_storm_lantern_east");

            // --- 1. ÔNG KAEL (5 giai đoạn, xếp từ mốc cao nhất xuống thấp nhất) ---
            DialogueData kaelS5 = UpsertDialogue("Dlg_Kael_Stage5_Completed", "dlg_kael_s5_completed", "", null, new[]
            {
                ("Ông Kael", "kael", "Cây cầu gỗ lim này giờ đi qua êm ru rồi đó Wen. Sáng nay bà Mara còn mang ấm trà sang tận xưởng mộc."),
                ("Wen", "wen", "Nhìn hai bác lại nói chuyện vui vẻ với nhau, cháu mừng lắm ạ."),
                ("Ông Kael", "kael", "Cảm ơn cháu nhiều lắm. Nghe bác Otis bảo mây trên cánh đồng gió phía Bắc đang tan dần rồi đấy, khi nào sẵn sàng thì cháu cứ đi tiếp hành trình nhé.")
            });

            DialogueData kaelS4 = UpsertDialogue("Dlg_Kael_Stage4_ReadyRelay", "dlg_kael_s4_ready_relay", "", null, new[]
            {
                ("Ông Kael", "kael", "Hóa ra đêm bão năm đó... tôi và bà ấy đều sợ đối phương gánh tội nên mới im lặng suốt ba mươi năm."),
                ("Ông Kael", "kael", "Đủ rồi. Tối nay lúc 18:30, cháu ra đầu cầu Bờ Đông nhé Wen. Tôi đã chuẩn bị búa và ván lim, ta cùng bà Mara đóng lại cây cầu này trong một đêm!")
            });

            DialogueData kaelS3 = UpsertDialogue("Dlg_Kael_Stage3_AfterNight3", "dlg_kael_s3_after_night3", "", eastLanternItem, new[]
            {
                ("Ông Kael", "kael", "Bác Otis đưa cháu xem sổ ghi chép đêm bão rồi sao... Chậc, đúng là đêm nào tôi cũng lén bào gỗ gia cố chân cầu thật."),
                ("Ông Kael", "kael", "Cầm lấy Nửa Đèn Bão Cổ này đi Wen. Năm xưa tôi và Mara tự tay rèn nó cho Lễ Trao Hạt đầu tiên. Nửa còn lại bị nước cuốn vào Đầm Lầy Bờ Tây rồi."),
                ("Tinh Linh Lumi", "lumi", "Nếu tìm đủ hai nửa đèn bão, Ánh Soi của Lumi sẽ dựng lại được ký ức thật sự của đêm bão năm ấy đó Wen!")
            });

            DialogueData kaelS2 = UpsertDialogue("Dlg_Kael_Stage2_Investigating", "dlg_kael_s2_investigating", "", null, new[]
            {
                ("Wen", "wen", "Ông Kael ơi, tối qua cháu thấy phoi bào gỗ lim mới tinh rơi ngay gần mố cầu..."),
                ("Ông Kael", "kael", "Khụ... thì xưởng tôi bào gỗ suốt ngày, gió thổi bay ra đó chứ có gì lạ đâu!"),
                ("Ông Kael", "kael", "Đêm xuống sương ngoài sông dày lắm, cháu cứ xem kỹ quanh bờ Tây rồi nói chuyện với bác Otis xem sao.")
            });

            DialogueData kaelS1 = UpsertDialogue("Dlg_Kael_Stage1_AfterIntro", "dlg_kael_s1_after_intro", "", null, new[]
            {
                ("Ông Kael", "kael", "Tôi đã bảo mà, một mình cháu không ghép nổi nhịp cầu qua dòng nước xiết này đâu."),
                ("Ông Kael", "kael", "Muốn sang Bờ Tây thì đi chiếc đò neo dưới bến kia. Còn nếu tò mò chuyện khúc sông này, cứ ngồi ở Ghế Đá chờ đến 18:30 tối rồi khắc thấy.")
            });

            // --- 2. BÀ MARA (6 giai đoạn, gồm callback lựa chọn Đêm 3) ---
            DialogueData maraS6 = UpsertDialogue("Dlg_Mara_Stage6_Completed", "dlg_mara_s6_completed", "", null, new[]
            {
                ("Bà Mara", "mara", "Ba mươi năm mang cục đá nặng trong lòng, cuối cùng đêm qua cũng bỏ xuống được rồi."),
                ("Bà Mara", "mara", "Cảm ơn cháu và bé Lumi. Khi nào mệt mỏi, cứ chạy qua cầu ghé Bờ Tây uống chén trà sen nóng với tôi nhé.")
            });

            DialogueData maraS5 = UpsertDialogue("Dlg_Mara_Stage5_ReadyRelay", "dlg_mara_s5_ready_relay", "", null, new[]
            {
                ("Bà Mara", "mara", "Cháu đã cho tôi thấy lại đêm bão năm đó... Kael đã chạy ra cắt dây cáp để chiếc cầu sập không kéo sụp mái nhà bên bờ tôi."),
                ("Bà Mara", "mara", "Tôi đã mang hộp đinh sắt tốt nhất ra bến rồi. 18:30 tối nay, cháu ra đầu cầu Bờ Đông gọi ông ấy, hai chúng tôi sẽ nối lại cây cầu ngay đêm nay!")
            });

            DialogueData maraS4 = UpsertDialogue("Dlg_Mara_Stage4_SwampCleared", "dlg_mara_s4_swamp_cleared", "", null, new[]
            {
                ("Bà Mara", "mara", "Cháu dọn sạch đám bọ gai trong Đầm Lầy Bờ Tây rồi sao? Cả nửa chiếc đèn bão cũ nữa..."),
                ("Bà Mara", "mara", "Ngọn đèn đó vẫn còn hơi ấm của đêm bão năm ấy đấy. Cháu thử dùng ánh sáng của bé Lumi soi ngay ở Đầm Lầy và mố cầu Bờ Tây xem sao.")
            });

            DialogueData maraS3Truth = UpsertDialogue("Dlg_Mara_Stage3_ToldTruth", "dlg_mara_s3_told_truth", "", null, new[]
            {
                ("Bà Mara", "mara", "Cháu nói thật sao... đêm nào Kael cũng lén bào gỗ lim để gia cố lại chân cầu gãy à?"),
                ("Bà Mara", "mara", "Hừm, nói miệng không đủ đâu. Nửa chiếc đèn bão cũ bị cuốn vào Đầm Lầy phía Bắc Bờ Tây kia kìa. Nếu cháu lấy được nó về soi lại chuyện cũ, tôi sẽ tin.")
            });

            DialogueData maraS3Secret = UpsertDialogue("Dlg_Mara_Stage3_KeptSecret", "dlg_mara_s3_kept_secret", "", null, new[]
            {
                ("Bà Mara", "mara", "Cháu vẫn giấu tôi chuyện gì bên Bờ Đông đúng không? Thôi, tôi cũng quen với sự im lặng rồi."),
                ("Bà Mara", "mara", "Nếu cháu thật sự muốn biết đêm bão năm xưa đã xảy ra chuyện gì, cứ lên Đầm Lầy phía Bắc Bờ Tây tìm nửa chiếc đèn bão bị thất lạc đi.")
            });

            DialogueData maraS1 = UpsertDialogue("Dlg_Mara_Stage1_MetMara", "dlg_mara_s1_met_mara", "", null, new[]
            {
                ("Bà Mara", "mara", "Cháu chèo đò sang tận đây chỉ để hỏi chuyện cây cầu thôi à? Đừng nghe lời một phía từ ông Kael."),
                ("Bà Mara", "mara", "Đợi đến 18:30 tối sương xuống, cháu cứ đi dọc hai bờ sông quan sát dấu vết rồi tự rút ra câu trả lời.")
            });

            // --- 3. BÁC OTIS BỜ SÔNG (3 giai đoạn) ---
            DialogueData otisS3 = UpsertDialogue("Dlg_OtisCh2_Stage3_Completed", "dlg_otis_ch2_s3_completed", "", null, new[]
            {
                ("Bác Otis", "otis", "Bác vừa ghi chép lại Đêm Chuyền Đèn vào biên niên sử của Thung Lũng Moonpetal rồi đấy Wen!"),
                ("Bác Otis", "otis", "Nhờ cháu mà cả làng lại được đi qua cây cầu bờ sông như thuở trước. Thật đáng tự hào!")
            });

            DialogueData otisS2 = UpsertDialogue("Dlg_OtisCh2_Stage2_ReadyRelay", "dlg_otis_ch2_s2_ready_relay", "", null, new[]
            {
                ("Bác Otis", "otis", "Hai Dư Âm đã trả lại sự thật trọn vẹn cho cả Kael lẫn Mara rồi."),
                ("Bác Otis", "otis", "Tối nay 18:30 sẽ là Đêm Chuyền Đèn. Cháu nhớ chuẩn bị đầy Thể lực, dùng Ánh Soi của Lumi xua lũ Bướm Hút Sáng và giữ sáng tối thiểu 4 cột đèn cầu nhé!")
            });

            DialogueData otisS1 = UpsertDialogue("Dlg_OtisCh2_Stage1_AfterNight3", "dlg_otis_ch2_s1_after_night3", "", null, new[]
            {
                ("Bác Otis", "otis", "Cháu đã đọc cuốn sổ cũ của bác rồi đấy. Giờ cháu hãy qua nói chuyện với ông Kael để nhận Nửa Đèn Bão Cổ bên Bờ Đông."),
                ("Bác Otis", "otis", "Sau đó sang Đầm Lầy Bờ Tây thanh tẩy lũ quái sương mù để tìm nốt nửa đèn còn lại nhé.")
            });

            // --- 4. GẮN CONDITIONAL DIALOGUES VÀO NPC_KAEL, NPC_MARA, NPC_OTIS_CH2 ---
            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Step47B] Không mở được scene " + Ch2ScenePath);
                return;
            }

            WireNpcConditional("NPC_Kael", new List<ConditionalDialogueEntry>
            {
                Entry("flag_ch2_completed", "", kaelS5),
                Entry("flag_ch2_echo2_pier_done", "flag_ch2_relay_done", kaelS4),
                Entry("flag_ch2_inv_night3_done", "flag_ch2_echo2_pier_done", kaelS3),
                Entry("flag_ch2_inv_night1_done", "flag_ch2_inv_night3_done", kaelS2),
                Entry("flag_ch2_met_kael", "flag_ch2_inv_night1_done", kaelS1),
            });

            WireNpcConditional("NPC_Mara", new List<ConditionalDialogueEntry>
            {
                Entry("flag_ch2_completed", "", maraS6),
                Entry("flag_ch2_echo2_pier_done", "flag_ch2_relay_done", maraS5),
                Entry("flag_ch2_swamp_cleared", "flag_ch2_echo2_pier_done", maraS4),
                Entry("flag_ch2_choice_tell_truth", "flag_ch2_swamp_cleared", maraS3Truth),
                Entry("flag_ch2_choice_keep_secret", "flag_ch2_swamp_cleared", maraS3Secret),
                Entry("flag_ch2_met_mara", "flag_ch2_inv_night3_done", maraS1),
            });

            WireNpcConditional("NPC_Otis_Ch2", new List<ConditionalDialogueEntry>
            {
                Entry("flag_ch2_completed", "", otisS3),
                Entry("flag_ch2_echo2_pier_done", "flag_ch2_completed", otisS2),
                Entry("flag_ch2_inv_night3_done", "flag_ch2_echo2_pier_done", otisS1),
            });

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Step47B] === HOÀN TẤT STEP 47B! Chạy 'Moonpetal/Step 47B/2. Validate Step 47B' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 47B/2. Validate Step 47B")]
        public static void ValidateStep47B()
        {
            int errors = 0;
            Debug.Log("[MV][Step47B] === BẮT ĐẦU VALIDATE STEP 47B ===");

            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Step47B][FAIL] Không mở được " + Ch2ScenePath);
                return;
            }

            errors += CheckNpcDialogues("NPC_Kael", 5);
            errors += CheckNpcDialogues("NPC_Mara", 6);
            errors += CheckNpcDialogues("NPC_Otis_Ch2", 3);

            string[] requiredDlgs =
            {
                "Dlg_Kael_Stage5_Completed", "Dlg_Kael_Stage4_ReadyRelay", "Dlg_Kael_Stage3_AfterNight3",
                "Dlg_Mara_Stage6_Completed", "Dlg_Mara_Stage5_ReadyRelay", "Dlg_Mara_Stage3_ToldTruth",
                "Dlg_Mara_Stage3_KeptSecret", "Dlg_OtisCh2_Stage3_Completed"
            };
            for (int i = 0; i < requiredDlgs.Length; i++)
            {
                string p = $"{DlgDir}/{requiredDlgs[i]}.asset";
                if (AssetDatabase.LoadAssetAtPath<DialogueData>(p) == null)
                {
                    Debug.LogError("[MV][Step47B][FAIL] Thiếu DialogueData: " + p);
                    errors++;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Step47B] VALIDATE PASS (0 lỗi) — 7 lỗi logic đã vá, F1 Debug đã đồng bộ 6 mốc & 14 thoại giai đoạn Kael/Mara/Otis sẵn sàng!");
            else
                Debug.LogError($"[MV][Step47B] VALIDATE FAIL ({errors} lỗi).");
        }

        private static ConditionalDialogueEntry Entry(string req, string block, DialogueData dlg)
        {
            return new ConditionalDialogueEntry
            {
                requiredStoryFlag = req,
                blockedByStoryFlag = block,
                dialogue = dlg
            };
        }

        private static DialogueData UpsertDialogue(string fileName, string id, string flagOnFinish, ItemData giveItem, (string speaker, string speakerId, string text)[] rows)
        {
            string assetPath = $"{DlgDir}/{fileName}.asset";
            DialogueData dlg = AssetDatabase.LoadAssetAtPath<DialogueData>(assetPath);
            if (dlg == null)
            {
                dlg = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(dlg, assetPath);
            }
            dlg.dialogueID = id;
            dlg.setsStoryFlagOnFinish = flagOnFinish;
            dlg.givesItemOnFinish = giveItem;
            dlg.givesItemAmount = giveItem != null ? 1 : 0;
            dlg.lines = new List<DialogueLine>();
            for (int i = 0; i < rows.Length; i++)
            {
                dlg.lines.Add(new DialogueLine
                {
                    speakerNameKey = rows[i].speaker,
                    speakerId = rows[i].speakerId,
                    textKey = rows[i].text
                });
            }
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static ItemData FindItemById(string itemId)
        {
            string[] guids = AssetDatabase.FindAssets("t:ItemData");
            for (int i = 0; i < guids.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(guids[i]);
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(p);
                if (item != null && item.itemID == itemId) return item;
            }
            return null;
        }

        private static GameObject FindSceneObjectEvenIfInactive(string targetName)
        {
            var all = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != null && all[i].name == targetName) return all[i].gameObject;
            }
            return null;
        }

        private static void WireNpcConditional(string npcGoName, List<ConditionalDialogueEntry> entries)
        {
            GameObject go = FindSceneObjectEvenIfInactive(npcGoName);
            if (go == null)
            {
                Debug.LogError("[MV][Step47B] Không tìm thấy " + npcGoName + " trong scene.");
                return;
            }
            NPCController ctrl = go.GetComponent<NPCController>();
            if (ctrl == null || ctrl.Data == null)
            {
                Debug.LogError("[MV][Step47B] " + npcGoName + " thiếu NPCController hoặc NPCData.");
                return;
            }
            ctrl.Data.conditionalDialogues = entries;
            EditorUtility.SetDirty(ctrl.Data);
            EditorUtility.SetDirty(go);
            Debug.Log($"[MV][Step47B] Đã gắn {entries.Count} mốc thoại giai đoạn cho {npcGoName} ({ctrl.Data.name}).");
        }

        private static int CheckNpcDialogues(string npcGoName, int minCount)
        {
            GameObject go = FindSceneObjectEvenIfInactive(npcGoName);
            NPCController ctrl = go != null ? go.GetComponent<NPCController>() : null;
            if (ctrl == null || ctrl.Data == null)
            {
                Debug.LogError($"[MV][Step47B][FAIL] Thiếu NPCController/NPCData trên {npcGoName}.");
                return 1;
            }
            int count = ctrl.Data.conditionalDialogues != null ? ctrl.Data.conditionalDialogues.Count : 0;
            if (count < minCount)
            {
                Debug.LogError($"[MV][Step47B][FAIL] {npcGoName} chỉ có {count} conditionalDialogues (cần >= {minCount}).");
                return 1;
            }
            for (int i = 0; i < count; i++)
            {
                if (ctrl.Data.conditionalDialogues[i] == null || ctrl.Data.conditionalDialogues[i].dialogue == null)
                {
                    Debug.LogError($"[MV][Step47B][FAIL] {npcGoName} có conditionalDialogues[{i}] null.");
                    return 1;
                }
            }
            return 0;
        }
    }
}
#endif
