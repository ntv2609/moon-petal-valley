using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.Core;
using MoonpetalValley.Inventory;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.Narrative
{
    public class QuestManager : MonoBehaviour
    {
        public event Action<string> OnObjectiveTextChanged;
        public string CurrentObjectiveSummary { get; private set; } = "";

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnItemChanged += HandleItemChanged;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded_Step40;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnItemChanged -= HandleItemChanged;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded_Step40;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start()
        {
            RecomputeObjective();
        }

        private void HandleFlagChanged(string _, bool __) => RecomputeObjective();
        private void HandleItemChanged(string _, int __) => RecomputeObjective();
        private void HandleSceneLoaded(Scene _, LoadSceneMode __) => RecomputeObjective();

        public void RecomputeObjective()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            StoryFlagManager f = GameRoot.Instance.Flags;
            InventoryManager inv = GameRoot.Instance.Inventory;

            string text = f.HasFlag("flag_ch2_completed")
                ? ComputeChapter3Objective(f, inv)
                : (f.HasFlag("flag_nv1_completed")
                    ? ComputeChapter2Objective(f, inv)
                    : ComputeChapter1Objective(f, inv));

            CurrentObjectiveSummary = text;
            OnObjectiveTextChanged?.Invoke(CurrentObjectiveSummary);
        }

        private static string ComputeChapter1Objective(StoryFlagManager f, InventoryManager inv)
        {
            if (!f.HasFlag("flag_intro_letter_read"))
            {
                return "<b>MỞ ĐẦU: Thư Của Bà</b>\n- Đọc bức thư trên Bàn Gỗ trong Nhà Kính [E]";
            }
            if (!f.HasFlag("flag_greenhouse_cleared"))
            {
                return "<b>MỞ ĐẦU: Dọn Nhà Kính</b>\n- Dùng Cuốc (1) hoặc Rìu (3) dọn 3 cụm cỏ/đá vụn";
            }
            if (!f.HasFlag("flag_lumi_awakened"))
            {
                return "<b>MỞ ĐẦU: Hạt Giống Ánh Trăng</b>\n- Mở Hộp Gỗ Của Bà [E] để đánh thức Lumi";
            }
            if (!f.HasFlag("flag_tutorial_done"))
            {
                return "<b>MỞ ĐẦU: Thử Nghiệm Đầu Tiên</b>\n- Gieo thử hạt tại Bồn Ươm Nhỏ trong Nhà Kính [E]";
            }
            if (!f.HasFlag("flag_met_bach_at_gate"))
            {
                return "<b>NHIỆM VỤ 1: Khu Vườn Bị Lãng Quên</b>\n- Ra Làng Chính, gặp Ông Bach ở cổng Vườn Phía Nam [E]";
            }
            if (!f.HasFlag("flag_garden_slimes_cleared"))
            {
                return "<b>NHIỆM VỤ 1: Thanh Tẩy Vườn Xưa</b>\n- Cầm Liềm Ánh Trăng (4) diệt 5 Bọt Sương Đen trong Vườn Ông Bach";
            }
            if (!f.HasFlag("flag_garden_bench_cleaned"))
            {
                return "<b>NHIỆM VỤ 1: Kỷ Niệm Cũ</b>\n- Bấm [E] lau sạch Chiếc Ghế Đôi Kỷ Niệm trong vườn";
            }
            if (!f.HasFlag("flag_hana_bread_done") || !f.HasFlag("flag_toby_cat_saved") || !f.HasFlag("flag_otis_mailbox_fixed"))
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>NHIỆM VỤ 1: Gom 3 Túi Hạt Di Sản</b>");
                if (!f.HasFlag("flag_hana_bread_done"))
                {
                    int barley = inv != null ? inv.GetItemCount("item_dew_barley") : 0;
                    sb.AppendLine("- Bà Hana: Giao 4 Lúa Mạch Sương Mai (" + barley + "/4)");
                }
                if (!f.HasFlag("flag_toby_cat_saved"))
                {
                    sb.AppendLine("- Cậu Bé Toby: Cứu mèo Miu ở Rìa Rừng Phía Đông");
                }
                if (!f.HasFlag("flag_otis_mailbox_fixed"))
                {
                    int wood = inv != null ? inv.GetItemCount("item_branch_wood") : 0;
                    sb.AppendLine("- Bác Otis: Sửa Hòm Thư bằng 10 Gỗ (" + wood + "/10)");
                }
                return sb.ToString().TrimEnd();
            }
            if (!f.HasFlag("flag_bach_letters_read"))
            {
                return "<b>NHIỆM VỤ 1: Lá Thư Muộn Màng</b>\n- Quay lại gặp Ông Bach đọc đủ 4 Trang Sổ Tay [E]";
            }
            return "<b>NHIỆM VỤ 1: Hoa Nở Lại Vườn Xưa</b>\n- Tới Bồn Hoa Trung Tâm Vườn, mở Khay Đóng Góp & Cùng Gieo Hạt [E]";
        }

        private static string ComputeChapter2Objective(StoryFlagManager f, InventoryManager inv)
        {
            string activeScene = SceneManager.GetActiveScene().name;

            // Stage 0: Arrival
            if (!f.HasFlag(Chapter2StateManager.FlagSoloBridgeFailed))
            {
                if (activeScene != Chapter2StateManager.Chapter2SceneName)
                {
                    return "<b>CHƯƠNG 2: Ánh Đèn Bên Dòng Sông</b>\n- Đi lối biển chỉ dẫn rìa Tây-Nam Làng Chính sang Bờ Sông [E]";
                }
                return "<b>CHƯƠNG 2 (1/8): Nhịp Cầu Đứt Đoạn</b>\n- Tiến về phía Tây tới đầu cây cầu gãy kiểm tra";
            }

            // Stage 1: Meet Kael
            if (!f.HasFlag(Chapter2StateManager.FlagMetKael))
            {
                return "<b>CHƯƠNG 2 (2/8): Xưởng Mộc Bờ Đông</b>\n- Gặp Ông Kael ở Xưởng Gỗ Bờ Đông hỏi chuyện cây cầu [E]";
            }

            // Stage 2: East Clue at Night (1/3)
            if (!f.HasFlag(Chapter2StateManager.FlagClue1Done))
            {
                return "<b>CHƯƠNG 2 (3/8): Điều Tra Bờ Đông (1/3)</b>\n- Ngồi Ghế Đá Bờ Sông chờ đến 18:30 [E]\n- Kiểm tra Phoi bào gỗ lim gần đầu cầu Đông [E]";
            }

            // Stage 3: Cross to West Bank, Meet Mara & West Clue (2/3)
            if (!f.HasFlag(Chapter2StateManager.FlagMetMara))
            {
                return "<b>CHƯƠNG 2 (4/8): Chuyến Đò Sang Bờ Tây</b>\n- Chèo Đò Ngang phía Nam sang Bờ Tây gặp Bà Mara [E]";
            }
            if (!f.HasFlag(Chapter2StateManager.FlagClue2Done))
            {
                return "<b>CHƯƠNG 2 (4/8): Điều Tra Bờ Tây (2/3)</b>\n- Kiểm tra Dấu ủng dính bùn gần mố cầu Tây vào ban đêm (18:30+) [E]";
            }

            // Stage 4: Otis Note (3/3) -> Kael Bronze Frame -> Mara Storm Lamp Oil
            if (!f.HasFlag(Chapter2StateManager.FlagClue3Done))
            {
                return "<b>CHƯƠNG 2 (5/8): Sổ Ghi Chép Năm Xưa (3/3)</b>\n- Quay về Bờ Đông kiểm tra Sổ ghi chép cũ của Bác Otis ban đêm [E]";
            }
            if (!f.HasFlag(Chapter2StateManager.FlagKaelGaveLantern))
            {
                return "<b>CHƯƠNG 2 (5/8): Khung Đèn Bão Cổ</b>\n- Gặp Ông Kael ở Bờ Đông nhận Khung Đồng Đèn Bão [E]";
            }
            if (!f.HasFlag(Chapter2StateManager.FlagSwampUnlocked))
            {
                return "<b>CHƯƠNG 2 (5/8): Ngọn Lửa Phá Sương</b>\n- Sang Bờ Tây nhờ Bà Mara châm Dầu Đèn Bão để mở lối vào Đầm Lầy [E]";
            }

            // Stage 5: West Swamp Combat & Echo 1
            if (!f.HasFlag(Chapter2StateManager.FlagSwampCleared))
            {
                return "<b>CHƯƠNG 2 (6/8): Đầm Lầy Bờ Tây</b>\n- Tiến vào Đầm Lầy phía Bắc Bờ Tây, thanh tẩy quái sương tìm Chao Kính Đèn Bão";
            }
            if (!f.HasFlag(Chapter2StateManager.FlagEcho1Done))
            {
                return "<b>CHƯƠNG 2 (6/8): Dư Âm Đèn Bão</b>\n- Ghép đủ 2 nửa Đèn Bão, dùng Ánh Soi xem Dư Âm quá khứ [E]";
            }

            // Stage 6: Echo 2 at Broken Pier
            if (!f.HasFlag(Chapter2StateManager.FlagEcho2Done))
            {
                return "<b>CHƯƠNG 2 (7/8): Dư Âm Đêm Bão</b>\n- Quay về mố cầu gãy Bờ Sông dùng Ánh Soi xem sự thật đêm bão 3 năm trước [E]";
            }

            // Stage 7: Agree with Kael & Lantern Relay Night
            if (!f.HasFlag(Chapter2StateManager.FlagRelayAgreed))
            {
                return "<b>CHƯƠNG 2 (8/8): Lời Hẹn Nối Cầu</b>\n- Gặp Ông Kael ở Bờ Đông thống nhất kế hoạch Đêm Chuyền Đèn [E]";
            }
            if (!f.HasFlag(Chapter2StateManager.FlagCh2Completed))
            {
                return "<b>CHƯƠNG 2 (8/8): Đêm Chuyền Đèn</b>\n- Ra đầu cầu Bờ Đông lúc 18:30 bắt đầu Đêm Chuyền Đèn nối cầu [E]";
            }

            return "<b>HOÀN THÀNH CHƯƠNG 2!</b>\n- Đã nhận Cánh Hoa Gắn Kết & nối thông hai bờ sông.\n- Sẵn sàng cho Chương 3: Chợ Đêm Đầu Tiên!";
        }

        private static string ComputeChapter3Objective(StoryFlagManager f, InventoryManager inv)
        {
            if (f.HasFlag("flag_ch3_completed"))
            {
                return "<b>HOÀN THÀNH CHƯƠNG 3!</b>\n- Đã lưu 7 Kỷ Vật & mở đường tới Rừng Phía Bắc (Chương 4).";
            }
            if (f.HasFlag("flag_ch3_market_done"))
            {
                int ks = MoonpetalValley.Chapter3.Chapter3StateManager.Instance != null
                    ? MoonpetalValley.Chapter3.Chapter3StateManager.Instance.CollectedKeepsakeCount : 0;
                return "<b>CHƯƠNG 3: Mười Đôi Tay & Cao Trào</b>\n- Ghé các gian hàng nhận đủ Kỷ Vật (" + ks + "/7) trước khi sương dâng";
            }
            if (f.HasFlag("flag_ch3_stalls_built_day3"))
            {
                return "<b>CHƯƠNG 3: Khai Hội Chợ Đêm</b>\n- Tới cây cầu đã nối ở Bờ Sông khi chiều buông để mở Chợ Đêm đầu tiên";
            }
            int crafted = MoonpetalValley.Chapter3.Chapter3StateManager.Instance != null
                ? MoonpetalValley.Chapter3.Chapter3StateManager.Instance.TotalCraftedItems : 0;
            int dayNum = MoonpetalValley.Chapter3.Chapter3StateManager.Instance != null
                ? Mathf.Max(1, MoonpetalValley.Chapter3.Chapter3StateManager.Instance.CurrentPrepDayNumber) : 1;
            return "<b>CHƯƠNG 3: Chuẩn Bị Chợ Đêm (Ngày " + dayNum + "/3)</b>\n- Hái Hoa Cúc Sông ở Bờ Sông & chế biến tại Bàn Chuẩn Bị (" + crafted + "/5 món)";
        }

        private void HandleSaveLoaded_Step40()
        {
            RecomputeObjective();
        }
    }
}
