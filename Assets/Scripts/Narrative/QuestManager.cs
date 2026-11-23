using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.Core;
using MoonpetalValley.Inventory;

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

            string text = f.HasFlag("flag_nv1_completed")
                ? ComputeChapter2Objective(f, inv)
                : ComputeChapter1Objective(f, inv);

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

            if (!f.HasFlag("flag_ch2_solo_bridge_failed"))
            {
                if (activeScene != "Riverbank_Chapter2")
                {
                    return "<b>CHƯƠNG 2: Ánh Đèn Bên Dòng Sông</b>\n- Đi lối biển chỉ dẫn rìa Tây-Nam Làng Chính sang Bờ Sông [E]";
                }
                return "<b>CHƯƠNG 2: Nhịp Cầu Đứt Đoạn</b>\n- Tiến về phía Tây tới đầu cây cầu gãy kiểm tra";
            }

            if (!f.HasFlag("flag_ch2_met_kael"))
            {
                return "<b>CHƯƠNG 2: Xưởng Mộc Bờ Đông</b>\n- Hỏi chuyện Ông Kael ở Xưởng Gỗ Bờ Đông [E]";
            }

            if (!f.HasFlag("flag_ch2_inv_night1_done"))
            {
                return "<b>CHƯƠNG 2: Điều Tra Đêm (1/3)</b>\n- Ngồi Ghế Đá Bờ Sông chờ 18:30 [E]\n- Kiểm tra Phoi bào gỗ lim gần đầu cầu Đông [E]";
            }

            if (!f.HasFlag("flag_ch2_inv_night2_done"))
            {
                if (!f.HasFlag("flag_ch2_met_mara"))
                {
                    return "<b>CHƯƠNG 2: Điều Tra Đêm (2/3)</b>\n- Chèo Đò Ngang phía Nam sang Bờ Tây gặp Bà Mara [E]\n- Kiểm tra Dấu ủng dính bùn gần cầu Tây ban đêm [E]";
                }
                return "<b>CHƯƠNG 2: Điều Tra Đêm (2/3)</b>\n- Kiểm tra Dấu ủng dính bùn gần mố cầu Tây vào ban đêm [E]";
            }

            if (!f.HasFlag("flag_ch2_inv_night3_done"))
            {
                return "<b>CHƯƠNG 2: Điều Tra Đêm (3/3)</b>\n- Kiểm tra Sổ ghi chép cũ của Bác Otis vào ban đêm [E]";
            }

            if (!f.HasFlag("flag_ch2_echo1_lantern_done"))
            {
                bool hasWestLantern = inv != null && inv.HasItem("item_storm_lantern_west", 1);
                if (!hasWestLantern)
                {
                    return "<b>CHƯƠNG 2: Hai Nửa Đèn Bão</b>\n- Tìm Nửa Đèn Bão (Kính Chắn Gió) ở Đầm Lầy Bờ Tây";
                }
                return "<b>CHƯƠNG 2: Dư Âm Đèn Bão</b>\n- Dùng Ánh Soi lên Chiếc Đèn Bão để xem Dư Âm quá khứ [E]";
            }

            if (!f.HasFlag("flag_ch2_echo2_pier_done"))
            {
                return "<b>CHƯƠNG 2: Dư Âm Đêm Bão</b>\n- Tới mố cầu gãy dùng Ánh Soi xem sự thật đêm bão [E]";
            }

            if (!f.HasFlag("flag_ch2_completed"))
            {
                return "<b>CHƯƠNG 2: Đêm Chuyền Đèn</b>\n- Giữ sáng 6 cột đèn & chuyền đồ cho Ông Kael và Bà Mara nối cầu";
            }

            return "<b>HOÀN THÀNH CHƯƠNG 2!</b>\n- Đã nhận Cánh Hoa Gắn Kết & nối thông hai bờ sông.\n- Sẵn sàng cho Chương 3: Chợ Đêm Đầu Tiên!";
        }

        private void HandleSaveLoaded_Step40()
        {
            RecomputeObjective();
        }
    }
}
