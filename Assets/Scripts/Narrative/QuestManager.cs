using System;
using System.Text;
using UnityEngine;
using MoonpetalValley.Core;

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
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnItemChanged -= HandleItemChanged;
        }

        private void Start()
        {
            RecomputeObjective();
        }

        private void HandleFlagChanged(string _, bool __) => RecomputeObjective();
        private void HandleItemChanged(string _, int __) => RecomputeObjective();

        public void RecomputeObjective()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            var f = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            string text;
            if (!f.HasFlag("flag_intro_letter_read"))
            {
                text = "<b>MỞ ĐẦU — Thư Của Bà</b>\n- Đọc bức thư trên Bàn Gỗ trong Nhà Kính [E]";
            }
            else if (!f.HasFlag("flag_greenhouse_cleared"))
            {
                text = "<b>MỞ ĐẦU — Dọn Nhà Kính</b>\n- Dùng Cuốc (1) hoặc Rìu (3) dọn 3 cụm cỏ/đá vụn";
            }
            else if (!f.HasFlag("flag_lumi_awakened"))
            {
                text = "<b>MỞ ĐẦU — Hạt Giống Ánh Trăng</b>\n- Mở Hộp Gỗ Của Bà [E] để đánh thức Lumi";
            }
            else if (!f.HasFlag("flag_tutorial_done"))
            {
                text = "<b>MỞ ĐẦU — Thử Nghiệm Đầu Tiên</b>\n- Gieo thử hạt tại Bồn Ươm Nhỏ trong Nhà Kính [E]";
            }
            else if (!f.HasFlag("flag_met_bach_at_gate"))
            {
                text = "<b>NHIỆM VỤ 1 — Khu Vườn Bị Lãng Quên</b>\n- Ra Làng Chính, gặp Ông Bách ở cổng Vườn Phía Nam [E]";
            }
            else if (!f.HasFlag("flag_garden_slimes_cleared"))
            {
                text = "<b>NHIỆM VỤ 1 — Thanh Tẩy Vườn Xưa</b>\n- Cầm Liềm Ánh Trăng (4) diệt 5 Bọt Sương Đen trong Vườn Ông Bách";
            }
            else if (!f.HasFlag("flag_garden_bench_cleaned"))
            {
                text = "<b>NHIỆM VỤ 1 — Kỷ Niệm Cũ</b>\n- Bấm [E] lau sạch Chiếc Ghế Đôi Kỷ Niệm trong vườn";
            }
            else if (!f.HasFlag("flag_hana_bread_done") || !f.HasFlag("flag_toby_cat_saved") || !f.HasFlag("flag_otis_mailbox_fixed"))
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>NHIỆM VỤ 1 — Gom 3 Túi Hạt Di Sản</b>");
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
                text = sb.ToString().TrimEnd();
            }
            else if (!f.HasFlag("flag_bach_letters_read"))
            {
                text = "<b>NHIỆM VỤ 1 — Lá Thư Muộn Màng</b>\n- Quay lại gặp Ông Bách đọc đủ 4 Trang Sổ Tay [E]";
            }
            else if (!f.HasFlag("flag_nv1_completed"))
            {
                text = "<b>NHIỆM VỤ 1 — Hoa Nở Lại Vườn Xưa</b>\n- Tới Bồn Hoa Trung Tâm Vườn, mở Khay Đóng Góp & Cùng Gieo Hạt [E]";
            }
            else
            {
                text = "<b>HOÀN THÀNH MVP!</b>\n- Nhận Cánh Hoa Hy Vọng, Sương Héo Úa đã tan biến!\n- Bấm [Q] dùng Xung Ánh Trăng của Lumi.";
            }

            CurrentObjectiveSummary = text;
            OnObjectiveTextChanged?.Invoke(CurrentObjectiveSummary);
        }
    }
}
