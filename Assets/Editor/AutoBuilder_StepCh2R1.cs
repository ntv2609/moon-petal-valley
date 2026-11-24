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
    public static class AutoBuilder_StepCh2R1
    {
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string Ch2DataRoot = "Assets/Data/Chapter2";
        private const string CoreDlgDir = "Assets/Data/Chapter2/Dialogues";
        private const string StageDlgDir = "Assets/Data/Dialogues/Chapter2";
        private const string Echo1AssetPath = "Assets/Data/Chapter2/Echo_Ch2_StormLantern.asset";
        private const string Echo2AssetPath = "Assets/Data/Chapter2/Echo_Ch2_BrokenPier.asset";

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R1 - 1. Build Narrative Locks & Unified Script")]
        public static void BuildStepCh2R1()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Ch2-R1] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Ch2-R1] === BẮT ĐẦU STEP CH2-R1: KHÓA MẠCH TRUYỆN 8 GIAI ĐOẠN & THỐNG NHẤT KỊCH BẢN (3 NĂM TRƯỚC) ===");
            Directory.CreateDirectory(CoreDlgDir);
            Directory.CreateDirectory(StageDlgDir);

            ItemData eastLanternItem = FindItemById("item_storm_lantern_east");
            if (eastLanternItem == null)
            {
                Debug.LogWarning("[MV][Ch2-R1] Không tìm thấy ItemData 'item_storm_lantern_east'!");
            }

            // 1. Cập nhật các thoại gốc & manh mối (thống nhất mốc 3 năm trước, dẫn dắt rõ ràng)
            DialogueData kaelIntro = UpsertDialogue(CoreDlgDir, "Dlg_Ch2_Kael_Intro", "dlg_ch2_kael_intro",
                Chapter2StateManager.FlagMetKael, null, new[]
                {
                    ("Ông Kael", "kael", "Đứng xa mép cầu ra cô bé! Gỗ mục hết rồi, một mình cháu có đóng cả tuần cũng không nổi đâu."),
                    ("Wen", "wen", "Cháu là Wen. Cây cầu này bị gãy từ bao giờ thế ạ?"),
                    ("Ông Kael", "kael", "Đêm bão 3 năm trước cuốn phăng nhịp giữa. Có người bên kia sông cứ đòi vá tạm, tôi cản không nổi nên... bỏ luôn."),
                    ("Ông Kael", "kael", "Đừng hỏi thêm nữa. Nếu cháu rảnh thì ngồi ở Ghế Đá Bờ Sông chờ đến 18:30 tối, tự khắc sẽ thấy quanh bờ Đông có gì lạ.")
                });

            DialogueData maraIntro = UpsertDialogue(CoreDlgDir, "Dlg_Ch2_Mara_Intro", "dlg_ch2_mara_intro",
                Chapter2StateManager.FlagMetMara, null, new[]
                {
                    ("Bà Mara", "mara", "Cháu chèo đò từ Bờ Đông sang tận đây à? Lại nghe ông Kael kể xấu tôi gì đúng không?"),
                    ("Wen", "wen", "Không đâu bà Mara, cháu chỉ muốn hiểu chuyện gì đã xảy ra với cây cầu thôi ạ."),
                    ("Bà Mara", "mara", "Đêm bão 3 năm trước, nước dâng ngang bến. Đèn bão tắt ngúm mà ông ấy quay lưng cắt dây cầu, bỏ mặc bờ Tây tôi..."),
                    ("Bà Mara", "mara", "Còn lối lên Đầm Lầy phía Bắc kia sương mù dày đặc lắm, cháu tuyệt đối đừng bén mảng vào. Tối xuống cứ xem quanh mố cầu Tây rồi tự hiểu.")
                });

            DialogueData clue1Dlg = UpsertDialogue(CoreDlgDir, "Dlg_Ch2_Clue1_Shavings", "dlg_ch2_clue1",
                Chapter2StateManager.FlagClue1Done, null, new[]
                {
                    ("Wen", "wen", "Mấy phoi bào gỗ lim này còn rất mới... rõ ràng đêm nào cũng có người lén ra gia cố mố cầu Đông!"),
                    ("Tinh Linh Lumi", "lumi", "Mùi nhựa thông tươi bay từ xưởng ông Kael đấy! Mình chèo chiếc Đò Ngang phía Nam sang Bờ Tây hỏi bà Mara xem sao nhé Wen.")
                });

            DialogueData clue2Dlg = UpsertDialogue(CoreDlgDir, "Dlg_Ch2_Clue2_Footprints", "dlg_ch2_clue2",
                Chapter2StateManager.FlagClue2Done, null, new[]
                {
                    ("Wen", "wen", "Dấu ủng dính bùn đi từ bến thuyền thẳng ra mố cầu Tây... hóa ra đêm nào bà Mara cũng lén kiểm tra nhịp cầu!"),
                    ("Tinh Linh Lumi", "lumi", "Cả hai bác đều quan tâm cây cầu mà chẳng ai chịu nói! Mình quay lại Bờ Đông kiểm tra cuốn sổ cũ của bác Otis thử xem.")
                });

            DialogueData clue3Dlg = UpsertDialogueWithChoice(CoreDlgDir, "Dlg_Ch2_Clue3_OtisNote", "dlg_ch2_clue3",
                Chapter2StateManager.FlagClue3Done, new[]
                {
                    ("Bác Otis", "otis", "Cháu đọc sổ ghi chép đêm bão 3 năm trước rồi à? Đêm đó Kael cắt dây cầu để ván gãy không đập sập bến nhà Mara, rồi lao xuống cứu thuyền tới bị thương."),
                    ("Bác Otis", "otis", "Muốn hóa giải hiểu lầm suốt 3 năm nay, cháu cần tìm đủ Chiếc Đèn Bão Cổ hai người cùng rèn: Khung Đồng ở chỗ ông Kael, còn Chao Kính bị bão cuốn vào Đầm Lầy Bờ Tây."),
                    ("Wen", "wen", "Ra là vậy! Cháu sẽ qua gặp ông Kael xin Khung Đồng trước. Còn chuyện đêm bão, mình nên nói thẳng cho bà Mara hay để hai người tự thấy qua Ánh Soi?")
                },
                "Nói thẳng sự thật từ lời bác Otis", "flag_ch2_choice_tell_truth",
                "Giữ kín chuyện cũ, để họ tự thấy qua Ánh Soi", "flag_ch2_choice_keep_secret");

            // 2. Thoại theo 8 giai đoạn của ÔNG KAEL (xếp từ mốc cao nhất xuống thấp nhất)
            DialogueData kaelS5 = UpsertDialogue(StageDlgDir, "Dlg_Kael_Stage5_Completed", "dlg_kael_s5_completed", "", null, new[]
            {
                ("Ông Kael", "kael", "Cây cầu gỗ lim này giờ đi qua êm ru rồi đó Wen. Sáng nay bà Mara còn mang ấm trà sen sang tận xưởng mộc."),
                ("Wen", "wen", "Nhìn hai bác hóa giải được hiểu lầm suốt 3 năm qua, cháu mừng lắm ạ."),
                ("Ông Kael", "kael", "Cảm ơn cháu nhiều lắm. Nghe bác Otis bảo mây trên cánh đồng gió phía Bắc đang tan dần rồi đấy, sẵn sàng thì cháu cứ đi tiếp nhé.")
            });

            DialogueData kaelS4Agreed = UpsertDialogue(StageDlgDir, "Dlg_Kael_Stage4B_WaitingRelay", "dlg_kael_s4b_waiting_relay", "", null, new[]
            {
                ("Ông Kael", "kael", "Tôi đã đặt búa và ván lim sẵn ở đầu cầu Bờ Đông rồi. 18:30 tối nay cháu ra đầu cầu gọi tôi, ta bắt đầu Đêm Chuyền Đèn ngay!")
            });

            DialogueData kaelS4 = UpsertDialogue(StageDlgDir, "Dlg_Kael_Stage4_ReadyRelay", "dlg_kael_s4_ready_relay",
                Chapter2StateManager.FlagRelayAgreed, null, new[]
                {
                    ("Ông Kael", "kael", "Hóa ra đêm bão 3 năm trước... tôi và bà ấy đều sợ đối phương gánh tội nên mới im lặng tới tận hôm nay."),
                    ("Ông Kael", "kael", "Đủ rồi! Tối nay lúc 18:30, cháu ra đầu cầu Bờ Đông gọi tôi nhé Wen. Tôi mang búa và ván lim, ta cùng bà Mara nối lại cây cầu trong một đêm!")
                });

            DialogueData kaelS3BWaitOil = UpsertDialogue(StageDlgDir, "Dlg_Kael_Stage3B_WaitMaraOil", "dlg_kael_s3b_wait_oil", "", null, new[]
            {
                ("Ông Kael", "kael", "Khung Đồng Đèn Bão trong tay cháu vẫn chưa có dầu đâu. Sang Bờ Tây nhờ bà Mara châm chút Dầu Đèn Bão rồi mới vào Đầm Lầy được.")
            });

            DialogueData kaelS3 = UpsertDialogue(StageDlgDir, "Dlg_Kael_Stage3_AfterNight3", "dlg_kael_s3_after_night3",
                Chapter2StateManager.FlagKaelGaveLantern, eastLanternItem, new[]
                {
                    ("Ông Kael", "kael", "Bác Otis đưa cháu xem sổ ghi chép đêm bão 3 năm trước rồi sao... Chậc, đúng là đêm nào tôi cũng lén bào gỗ gia cố chân cầu thật."),
                    ("Ông Kael", "kael", "Cầm lấy Khung Đồng Đèn Bão này đi Wen. Chao Kính bị bão cuốn vào Đầm Lầy Bờ Tây rồi, nhưng sương trong đó dày lắm."),
                    ("Ông Kael", "kael", "Khung đèn này hết dầu rồi. Cháu mang sang Bờ Tây nhờ bà Mara châm chút Dầu Đèn Bão, ngọn lửa đó sẽ xua tan sương mù mở lối vào Đầm Lầy.")
                });

            DialogueData kaelS2 = UpsertDialogue(StageDlgDir, "Dlg_Kael_Stage2_Investigating", "dlg_kael_s2_investigating", "", null, new[]
            {
                ("Wen", "wen", "Ông Kael ơi, tối qua cháu thấy phoi bào gỗ lim mới tinh rơi ngay gần mố cầu..."),
                ("Ông Kael", "kael", "Khụ... thì xưởng tôi bào gỗ suốt ngày, gió thổi bay ra đó chứ có gì lạ đâu!"),
                ("Ông Kael", "kael", "Cháu cứ chèo đò sang Bờ Tây gặp bà Mara rồi kiểm tra nốt quanh bờ sông xem sao.")
            });

            DialogueData kaelS1 = UpsertDialogue(StageDlgDir, "Dlg_Kael_Stage1_AfterIntro", "dlg_kael_s1_after_intro", "", null, new[]
            {
                ("Ông Kael", "kael", "Tôi đã bảo mà, một mình cháu không ghép nổi nhịp cầu qua dòng nước xiết này đâu."),
                ("Ông Kael", "kael", "Muốn biết chuyện bờ Đông thì cứ ngồi ở Ghế Đá chờ đến 18:30 tối rồi đi quanh đầu cầu khắc thấy.")
            });

            // 3. Thoại theo 8 giai đoạn của BÀ MARA (gồm màn châm Dầu Đèn Bão mở khóa Đầm Lầy)
            DialogueData maraS6 = UpsertDialogue(StageDlgDir, "Dlg_Mara_Stage6_Completed", "dlg_mara_s6_completed", "", null, new[]
            {
                ("Bà Mara", "mara", "Ba năm mang cục đá nặng trong lòng, cuối cùng đêm qua cũng bỏ xuống được rồi."),
                ("Bà Mara", "mara", "Cảm ơn cháu và bé Lumi. Khi nào mệt mỏi, cứ chạy qua cầu ghé Bờ Tây uống chén trà sen nóng với tôi nhé.")
            });

            DialogueData maraS5 = UpsertDialogue(StageDlgDir, "Dlg_Mara_Stage5_ReadyRelay", "dlg_mara_s5_ready_relay", "", null, new[]
            {
                ("Bà Mara", "mara", "Dư Âm đêm bão 3 năm trước đã cho tôi thấy rõ... Kael đã cắt dây cáp để nhịp cầu gãy không kéo sụp bến nhà tôi."),
                ("Bà Mara", "mara", "Tôi đã chuẩn bị hộp đinh đồng tốt nhất rồi. Cháu sang Bờ Đông báo ông Kael một tiếng, 18:30 tối nay hai chúng tôi sẽ nối lại cầu!")
            });

            DialogueData maraS4 = UpsertDialogue(StageDlgDir, "Dlg_Mara_Stage4_SwampCleared", "dlg_mara_s4_swamp_cleared", "", null, new[]
            {
                ("Bà Mara", "mara", "Cháu dọn sạch đám quái trong Đầm Lầy Bờ Tây và tìm lại được Chao Kính Đèn Bão rồi sao?"),
                ("Bà Mara", "mara", "Ngọn đèn ghép đủ hai nửa rồi đấy. Cháu dùng Ánh Soi của bé Lumi soi Dư Âm Đèn Bão rồi ra mố cầu gãy xem chuyện cũ nhé.")
            });

            DialogueData maraS3CFueled = UpsertDialogue(StageDlgDir, "Dlg_Mara_Stage3C_SwampOpen", "dlg_mara_s3c_swamp_open", "", null, new[]
            {
                ("Bà Mara", "mara", "Ngọn lửa dầu đèn bão đang cháy sáng rồi. Lối lên Đầm Lầy Bờ Tây phía Bắc đã tan sương, cháu cẩn thận đám quái sương trong đó nhé!")
            });

            DialogueData maraS3Truth = UpsertDialogue(StageDlgDir, "Dlg_Mara_Stage3_ToldTruth", "dlg_mara_s3_told_truth",
                Chapter2StateManager.FlagSwampUnlocked, null, new[]
                {
                    ("Wen", "wen", "Bà Mara ơi, ông Kael gửi cháu Khung Đồng Đèn Bão. Bác Otis kể đêm bão 3 năm trước ông ấy cắt cáp là để cứu bến nhà bà!"),
                    ("Bà Mara", "mara", "Khung đèn năm xưa... Ông ấy vẫn giữ kỹ đến vậy sao? Để tôi châm chút Dầu Đèn Bão vào cho cháu."),
                    ("Bà Mara", "mara", "Đèn sáng rồi đấy! Ngọn lửa này đủ xua tan sương mù chặn lối lên Đầm Lầy phía Bắc. Cháu vào đó tìm Chao Kính về soi lại chuyện cũ, tôi sẽ tin.")
                });

            DialogueData maraS3Secret = UpsertDialogue(StageDlgDir, "Dlg_Mara_Stage3_KeptSecret", "dlg_mara_s3_kept_secret",
                Chapter2StateManager.FlagSwampUnlocked, null, new[]
                {
                    ("Wen", "wen", "Bà Mara ơi, ông Kael đưa cháu Khung Đồng Đèn Bão cổ. Bà cho cháu xin chút Dầu Đèn Bão để vào Đầm Lầy tìm phần kính còn lại nhé."),
                    ("Bà Mara", "mara", "Cháu vẫn giấu tôi chuyện gì bên Bờ Đông đúng không? Thôi, thấy chiếc khung đồng cũ này tôi cũng mủi lòng... để tôi châm dầu cho."),
                    ("Bà Mara", "mara", "Ngọn lửa dầu bão đã cháy! Sương mù lối lên Đầm Lầy phía Bắc đã tan bớt, cháu vào đó tìm Chao Kính đi rồi cẩn thận đám quái nhé.")
                });

            DialogueData maraS2WaitKael = UpsertDialogue(StageDlgDir, "Dlg_Mara_Stage2_WaitKaelLantern", "dlg_mara_s2_wait_kael", "", null, new[]
            {
                ("Bà Mara", "mara", "Cháu muốn vào Đầm Lầy phía Bắc tìm Chao Kính Đèn Bão à? Sương trong đó dày lắm, không có Khung Đèn Bão của ông Kael thì dầu nhà tôi cũng vô ích."),
                ("Bà Mara", "mara", "Cháu quay lại Bờ Đông kiểm tra sổ bác Otis rồi xin ông Kael chiếc Khung Đồng mang sang đây tôi châm dầu cho.")
            });

            DialogueData maraS1 = UpsertDialogue(StageDlgDir, "Dlg_Mara_Stage1_MetMara", "dlg_mara_s1_met_mara", "", null, new[]
            {
                ("Bà Mara", "mara", "Cháu đã sang tới Bờ Tây thì cứ đợi đến 18:30 tối, ra gần mố cầu Tây quan sát dấu vết rồi tự rút ra câu trả lời.")
            });

            // 4. Thoại theo giai đoạn của BÁC OTIS BỜ SÔNG
            DialogueData otisS3 = UpsertDialogue(StageDlgDir, "Dlg_OtisCh2_Stage3_Completed", "dlg_otis_ch2_s3_completed", "", null, new[]
            {
                ("Bác Otis", "otis", "Bác vừa ghi chép lại Đêm Chuyền Đèn vào biên niên sử của Thung Lũng Moonpetal rồi đấy Wen!"),
                ("Bác Otis", "otis", "Nhờ cháu mà cả làng lại được đi qua cây cầu bờ sông như thuở trước. Thật đáng tự hào!")
            });

            DialogueData otisS2 = UpsertDialogue(StageDlgDir, "Dlg_OtisCh2_Stage2_ReadyRelay", "dlg_otis_ch2_s2_ready_relay", "", null, new[]
            {
                ("Bác Otis", "otis", "Hai Dư Âm đã trả lại sự thật trọn vẹn cho cả Kael lẫn Mara sau 3 năm hiểu lầm rồi."),
                ("Bác Otis", "otis", "Cháu sang gặp ông Kael thống nhất kế hoạch, rồi 18:30 tối nay ra đầu cầu Đông bắt đầu Đêm Chuyền Đèn nhé!")
            });

            DialogueData otisS1 = UpsertDialogue(StageDlgDir, "Dlg_OtisCh2_Stage1_AfterNight3", "dlg_otis_ch2_s1_after_night3", "", null, new[]
            {
                ("Bác Otis", "otis", "Cháu qua gặp ông Kael ở Bờ Đông nhận Khung Đồng Đèn Bão trước nhé."),
                ("Bác Otis", "otis", "Sau đó mang khung đèn sang Bờ Tây nhờ bà Mara châm Dầu Đèn Bão để xua tan sương mù lối vào Đầm Lầy!")
            });

            // 5. Cập nhật Echo 1 & Echo 2 yêu cầu chặt chẽ (Echo 1 đòi đủ CẢ 2 NỬA ĐÈN BÃO)
            EchoData echo1 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo1AssetPath);
            if (echo1 != null)
            {
                echo1.echoID = Chapter2StateManager.Echo1Id;
                echo1.requiredFlag = Chapter2StateManager.FlagSwampCleared;
                echo1.requiredItemId = "item_storm_lantern_east";
                echo1.requiredSecondaryItemId = "item_storm_lantern_west";
                echo1.completedFlag = Chapter2StateManager.FlagEcho1Done;
                EditorUtility.SetDirty(echo1);
            }
            else Debug.LogError("[MV][Ch2-R1] Thiếu " + Echo1AssetPath);

            EchoData echo2 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo2AssetPath);
            if (echo2 != null)
            {
                echo2.echoID = Chapter2StateManager.Echo2Id;
                echo2.requiredFlag = Chapter2StateManager.FlagEcho1Done;
                echo2.requiredItemId = "";
                echo2.requiredSecondaryItemId = "";
                echo2.completedFlag = Chapter2StateManager.FlagEcho2Done;
                EditorUtility.SetDirty(echo2);
            }
            else Debug.LogError("[MV][Ch2-R1] Thiếu " + Echo2AssetPath);

            // 6. Mở Riverbank_Chapter2.unity để nối dây NPC, Ferry, Clues
            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R1] Không mở được scene " + Ch2ScenePath);
                return;
            }

            WireNpcConditional("NPC_Kael", kaelIntro, new List<ConditionalDialogueEntry>
            {
                Entry(Chapter2StateManager.FlagCh2Completed, "", kaelS5),
                Entry(Chapter2StateManager.FlagRelayAgreed, Chapter2StateManager.FlagCh2Completed, kaelS4Agreed),
                Entry(Chapter2StateManager.FlagEcho2Done, Chapter2StateManager.FlagRelayAgreed, kaelS4),
                Entry(Chapter2StateManager.FlagKaelGaveLantern, Chapter2StateManager.FlagEcho2Done, kaelS3BWaitOil),
                Entry(Chapter2StateManager.FlagClue3Done, Chapter2StateManager.FlagKaelGaveLantern, kaelS3),
                Entry(Chapter2StateManager.FlagClue1Done, Chapter2StateManager.FlagClue3Done, kaelS2),
                Entry(Chapter2StateManager.FlagMetKael, Chapter2StateManager.FlagClue1Done, kaelS1),
            });

            WireNpcConditional("NPC_Mara", maraIntro, new List<ConditionalDialogueEntry>
            {
                Entry(Chapter2StateManager.FlagCh2Completed, "", maraS6),
                Entry(Chapter2StateManager.FlagEcho2Done, Chapter2StateManager.FlagCh2Completed, maraS5),
                Entry(Chapter2StateManager.FlagSwampCleared, Chapter2StateManager.FlagEcho2Done, maraS4),
                Entry(Chapter2StateManager.FlagSwampUnlocked, Chapter2StateManager.FlagSwampCleared, maraS3CFueled),
                Entry(Chapter2StateManager.FlagKaelGaveLantern, Chapter2StateManager.FlagSwampUnlocked, maraS3Truth),
                Entry(Chapter2StateManager.FlagKaelGaveLantern, Chapter2StateManager.FlagSwampUnlocked, maraS3Secret),
                Entry(Chapter2StateManager.FlagClue2Done, Chapter2StateManager.FlagKaelGaveLantern, maraS2WaitKael),
                Entry(Chapter2StateManager.FlagMetMara, Chapter2StateManager.FlagClue2Done, maraS1),
            });

            WireNpcConditional("NPC_Otis_Ch2", null, new List<ConditionalDialogueEntry>
            {
                Entry(Chapter2StateManager.FlagCh2Completed, "", otisS3),
                Entry(Chapter2StateManager.FlagEcho2Done, Chapter2StateManager.FlagCh2Completed, otisS2),
                Entry(Chapter2StateManager.FlagClue3Done, Chapter2StateManager.FlagEcho2Done, otisS1),
            });

            // Cập nhật requiredFlag cho 2 đò ngang (khóa tới Stage 3 - sau Manh Mối 1)
            UpdateFerryFlag("FerryBoat_East", Chapter2StateManager.FlagClue1Done);
            UpdateFerryFlag("FerryBoat_West", Chapter2StateManager.FlagClue1Done);

            // Đảm bảo 3 Manh Mối nối đúng thứ tự & thoại mới
            UpdateClueInScene("Clue_WoodShavings", "Phoi bào gỗ lim mới", 1,
                Chapter2StateManager.FlagMetKael, Chapter2StateManager.FlagClue1Done, clue1Dlg, "");
            UpdateClueInScene("Clue_MudFootprints", "Dấu ủng dính bùn", 2,
                Chapter2StateManager.FlagMetMara, Chapter2StateManager.FlagClue2Done, clue2Dlg, "");
            UpdateClueInScene("Clue_OtisNote", "Sổ ghi chép cũ của Bác Otis", 3,
                Chapter2StateManager.FlagClue2Done, Chapter2StateManager.FlagClue3Done, clue3Dlg, "NPC_Otis_Ch2");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Ch2-R1] === HOÀN TẤT STEP CH2-R1! Chạy 'Step Ch2-R1 - 2. Validate Step Ch2-R1' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R1 - 2. Validate Step Ch2-R1")]
        public static void ValidateStepCh2R1()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-R1] === BẮT ĐẦU VALIDATE STEP CH2-R1 ===");

            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R1][FAIL] Không mở được " + Ch2ScenePath);
                return;
            }

            EchoData echo1 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo1AssetPath);
            if (echo1 == null || echo1.requiredItemId != "item_storm_lantern_east" || echo1.requiredSecondaryItemId != "item_storm_lantern_west")
            {
                Debug.LogError("[MV][Ch2-R1][FAIL] Echo_Ch2_StormLantern chưa yêu cầu đủ 2 nửa đèn bão!");
                errors++;
            }

            EchoData echo2 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo2AssetPath);
            if (echo2 == null || echo2.requiredFlag != Chapter2StateManager.FlagEcho1Done)
            {
                Debug.LogError("[MV][Ch2-R1][FAIL] Echo_Ch2_BrokenPier chưa khóa theo flag_ch2_echo1_lantern_done!");
                errors++;
            }

            errors += CheckNpcDialogues("NPC_Kael", 7);
            errors += CheckNpcDialogues("NPC_Mara", 7);
            errors += CheckNpcDialogues("NPC_Otis_Ch2", 3);

            string[] ferries = { "FerryBoat_East", "FerryBoat_West" };
            for (int i = 0; i < ferries.Length; i++)
            {
                GameObject fgo = FindSceneObjectEvenIfInactive(ferries[i]);
                FerryBoatInteractable fb = fgo != null ? fgo.GetComponent<FerryBoatInteractable>() : null;
                if (fb == null)
                {
                    Debug.LogError("[MV][Ch2-R1][FAIL] Thiếu FerryBoatInteractable trên " + ferries[i]);
                    errors++;
                }
            }

            // Kiểm tra không còn chuỗi "ba mươi năm" trong toàn bộ DialogueData Chapter 2
            string[] dlgGuids = AssetDatabase.FindAssets("t:DialogueData", new[] { CoreDlgDir, StageDlgDir });
            for (int i = 0; i < dlgGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(dlgGuids[i]);
                DialogueData d = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
                if (d == null || d.lines == null) continue;
                for (int j = 0; j < d.lines.Count; j++)
                {
                    string txt = d.lines[j] != null ? (d.lines[j].textKey ?? "") : "";
                    if (txt.ToLowerInvariant().Contains("ba mươi"))
                    {
                        Debug.LogError($"[MV][Ch2-R1][FAIL] Còn sót chuỗi 'ba mươi' trong {path}: {txt}");
                        errors++;
                    }
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-R1] VALIDATE PASS (0 lỗi) - Mạch truyện 8 giai đoạn đã khóa chặt, thống nhất mốc 3 năm trước & 2 nửa Đèn Bão!");
            else
                Debug.LogError($"[MV][Ch2-R1] VALIDATE FAIL ({errors} lỗi).");
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

        private static DialogueData UpsertDialogue(
            string folder, string fileName, string id, string flagOnFinish, ItemData giveItem,
            (string speaker, string speakerId, string text)[] rows)
        {
            string assetPath = $"{folder}/{fileName}.asset";
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
            dlg.choices = new List<DialogueChoice>();
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static DialogueData UpsertDialogueWithChoice(
            string folder, string fileName, string id, string flagOnFinish,
            (string speaker, string speakerId, string text)[] rows,
            string optA, string flagA, string optB, string flagB)
        {
            DialogueData dlg = UpsertDialogue(folder, fileName, id, flagOnFinish, null, rows);
            dlg.choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceTextKey = optA, setsFlagOnPick = flagA },
                new DialogueChoice { choiceTextKey = optB, setsFlagOnPick = flagB }
            };
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

        private static void WireNpcConditional(string npcGoName, DialogueData defaultDlg, List<ConditionalDialogueEntry> entries)
        {
            GameObject go = FindSceneObjectEvenIfInactive(npcGoName);
            if (go == null)
            {
                Debug.LogError("[MV][Ch2-R1] Không tìm thấy " + npcGoName + " trong scene.");
                return;
            }
            NPCController ctrl = go.GetComponent<NPCController>();
            if (ctrl == null || ctrl.Data == null)
            {
                Debug.LogError("[MV][Ch2-R1] " + npcGoName + " thiếu NPCController hoặc NPCData.");
                return;
            }
            if (defaultDlg != null) ctrl.Data.defaultDialogue = defaultDlg;
            ctrl.Data.conditionalDialogues = entries;
            EditorUtility.SetDirty(ctrl.Data);
            EditorUtility.SetDirty(go);
            Debug.Log($"[MV][Ch2-R1] Đã nối {entries.Count} mốc thoại giai đoạn cho {npcGoName}.");
        }

        private static void UpdateFerryFlag(string ferryName, string reqFlag)
        {
            GameObject go = FindSceneObjectEvenIfInactive(ferryName);
            if (go == null) return;
            FerryBoatInteractable fb = go.GetComponent<FerryBoatInteractable>();
            if (fb == null) return;
            SerializedObject so = new SerializedObject(fb);
            so.FindProperty("requiredFlag").stringValue = reqFlag;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(fb);
        }

        private static void UpdateClueInScene(string objName, string title, int stage, string prevFlag, string doneFlag, DialogueData dlg, string approachNpc)
        {
            GameObject go = FindSceneObjectEvenIfInactive(objName);
            if (go == null) return;
            InvestigationClueInteractable clue = go.GetComponent<InvestigationClueInteractable>();
            if (clue == null) return;
            clue.Configure(title, stage, prevFlag, doneFlag, dlg, true, approachNpc);
            EditorUtility.SetDirty(clue);
        }

        private static int CheckNpcDialogues(string npcGoName, int minCount)
        {
            GameObject go = FindSceneObjectEvenIfInactive(npcGoName);
            NPCController ctrl = go != null ? go.GetComponent<NPCController>() : null;
            if (ctrl == null || ctrl.Data == null)
            {
                Debug.LogError($"[MV][Ch2-R1][FAIL] Thiếu NPCController/NPCData trên {npcGoName}.");
                return 1;
            }
            int count = ctrl.Data.conditionalDialogues != null ? ctrl.Data.conditionalDialogues.Count : 0;
            if (count < minCount)
            {
                Debug.LogError($"[MV][Ch2-R1][FAIL] {npcGoName} chỉ có {count} conditionalDialogues (cần >= {minCount}).");
                return 1;
            }
            for (int i = 0; i < count; i++)
            {
                if (ctrl.Data.conditionalDialogues[i] == null || ctrl.Data.conditionalDialogues[i].dialogue == null)
                {
                    Debug.LogError($"[MV][Ch2-R1][FAIL] {npcGoName} có conditionalDialogues[{i}] null.");
                    return 1;
                }
            }
            return 0;
        }
    }
}
#endif
