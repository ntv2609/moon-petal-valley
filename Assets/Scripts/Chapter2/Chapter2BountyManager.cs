using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2 {
    public class Chapter2BountyManager : MonoBehaviour {
        public static Chapter2BountyManager Instance { get; private set; }

        public struct BountyInfo {
            public int id;
            public string title;
            public string desc;
            public string rewardText;
            public string doneFlag;
            public string claimedFlag;
            public int target;
        }

        public static readonly BountyInfo[] Bounties = {
            new BountyInfo {
                id = 0, title = "1. Thu Hoạch Vườn Ươm",
                desc = "Thu hoạch 4 cây trồng tại Vườn Ươm Bờ Đông",
                rewardText = "Thưởng: 3 Phân Tinh Bụi + 2 Hạt Cánh Trăng",
                doneFlag = "flag_bounty_ch2_1_done", claimedFlag = "flag_bounty_ch2_1_claimed", target = 4
            },
            new BountyInfo {
                id = 1, title = "2. Dược Sĩ Ánh Trăng",
                desc = "Nấu 2 bình thuốc/bánh tại Vạc Vườn Ươm",
                rewardText = "Thưởng: 2 Tinh Chất Ánh Trăng + 2 Hạt Ớt Đom Đóm",
                doneFlag = "flag_bounty_ch2_2_done", claimedFlag = "flag_bounty_ch2_2_claimed", target = 2
            },
            new BountyInfo {
                id = 2, title = "3. Bùng Nổ Sóng Trăng",
                desc = "Kích hoạt Cơn Sốt Ánh Trăng (15 đòn) 1 lần",
                rewardText = "Thưởng: 4 Mảnh Sương Đen + 1 Bánh Lửa Đêm",
                doneFlag = "flag_bounty_ch2_3_done", claimedFlag = "flag_bounty_ch2_3_claimed", target = 1
            },
            new BountyInfo {
                id = 3, title = "4. Thợ Rèn Bờ Sông",
                desc = "Rèn 1 trang bị nâng cấp tại Lò Rèn Ông Kael",
                rewardText = "Thưởng: 2 Tinh Chất + 2 Bánh Lửa + 2 Phân Tinh Bụi",
                doneFlag = "flag_bounty_ch2_4_done", claimedFlag = "flag_bounty_ch2_4_claimed", target = 1
            }
        };

        private readonly int[] _progress = new int[4];
        private int _lastCropTotal = -1;
        private int _lastBrewTotal = -1;

        private void Awake() {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        private void OnEnable() {
            MoonlightFeverController.OnMoonlightFeverTriggered += HandleFeverTriggered;
            GameEventBus.OnItemChanged += HandleItemChanged;
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += SyncBaselines;
        }

        private void OnDisable() {
            MoonlightFeverController.OnMoonlightFeverTriggered -= HandleFeverTriggered;
            GameEventBus.OnItemChanged -= HandleItemChanged;
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= SyncBaselines;
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }
        private void Start() { SyncBaselines(); }

        private void SyncBaselines() {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            _lastCropTotal = CountCrops(inv);
            _lastBrewTotal = CountBrews(inv);
        }

        private static int CountCrops(MoonpetalValley.Inventory.InventoryManager inv) {
            return PlayerCombatBuffController.CountItem(inv, "item_moonbud_crop")
                 + PlayerCombatBuffController.CountItem(inv, "item_firefly_chili")
                 + PlayerCombatBuffController.CountItem(inv, "item_dew_barley");
        }

        private static int CountBrews(MoonpetalValley.Inventory.InventoryManager inv) {
            return PlayerCombatBuffController.CountItem(inv, "item_dew_tea")
                 + PlayerCombatBuffController.CountItem(inv, "item_moon_elixir")
                 + PlayerCombatBuffController.CountItem(inv, "item_nightfire_cake");
        }

        private void HandleItemChanged(string itemId, int newTotal) {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            if (itemId == "item_moonbud_crop" || itemId == "item_firefly_chili" || itemId == "item_dew_barley") {
                int now = CountCrops(inv);
                if (_lastCropTotal >= 0 && now > _lastCropTotal) AddProgress(0, now - _lastCropTotal);
                _lastCropTotal = now;
            } else if (itemId == "item_dew_tea" || itemId == "item_moon_elixir" || itemId == "item_nightfire_cake") {
                int now = CountBrews(inv);
                if (_lastBrewTotal >= 0 && now > _lastBrewTotal) AddProgress(1, now - _lastBrewTotal);
                _lastBrewTotal = now;
            }
        }

        private void HandleFeverTriggered() { AddProgress(2, 1); }

        private void HandleFlagChanged(string flagId, bool value) {
            if (!value) return;
            if (flagId == KaelForgeUIView.FlagSickleLv2 || flagId == KaelForgeUIView.FlagWateringLv2 || flagId == KaelForgeUIView.FlagNightwindBoots)
                AddProgress(3, 1);
        }

        public void AddProgress(int id, int delta) {
            if (id < 0 || id >= Bounties.Length || delta <= 0) return;
            var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags != null && (flags.HasFlag(Bounties[id].doneFlag) || flags.HasFlag(Bounties[id].claimedFlag))) return;

            _progress[id] = Mathf.Min(Bounties[id].target, _progress[id] + delta);
            if (_progress[id] >= Bounties[id].target && flags != null) {
                flags.SetFlag(Bounties[id].doneFlag, true);
                GameEventBus.RaiseToastRequested($"Hoàn thành nhiệm vụ phụ [{Bounties[id].title}]! Ghé Bảng Nhiệm Vụ Bờ Đông để nhận thưởng!");
            }
        }

        public int GetProgress(int id) {
            if (id < 0 || id >= Bounties.Length) return 0;
            var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags != null && (flags.HasFlag(Bounties[id].doneFlag) || flags.HasFlag(Bounties[id].claimedFlag)))
                return Bounties[id].target;
            return _progress[id];
        }

        public bool IsDone(int id) {
            var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            return flags != null && flags.HasFlag(Bounties[id].doneFlag);
        }

        public bool IsClaimed(int id) {
            var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            return flags != null && flags.HasFlag(Bounties[id].claimedFlag);
        }

        public bool TryClaimReward(int id) {
            if (id < 0 || id >= Bounties.Length) return false;
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null || GameRoot.Instance.Inventory == null) return false;
            var flags = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            if (!flags.HasFlag(Bounties[id].doneFlag) || flags.HasFlag(Bounties[id].claimedFlag)) return false;

            if (id == 0) { inv.AddItemById("item_star_fertilizer", 3, false); inv.AddItemById("item_moonbud_seed", 2, false); }
            else if (id == 1) { inv.AddItemById("item_moon_elixir", 2, false); inv.AddItemById("item_firefly_chili_seed", 2, false); }
            else if (id == 2) { inv.AddItemById("item_gloom_shard", 4, false); inv.AddItemById("item_nightfire_cake", 1, false); }
            else if (id == 3) { inv.AddItemById("item_moon_elixir", 2, false); inv.AddItemById("item_nightfire_cake", 2, false); inv.AddItemById("item_star_fertilizer", 2, false); }

            flags.SetFlag(Bounties[id].claimedFlag, true);
            if (GameRoot.Instance.Audio != null) GameRoot.Instance.Audio.PlayUIClick();
            GameEventBus.RaiseToastRequested($"Đã nhận thưởng [{Bounties[id].title}]! {Bounties[id].rewardText}");
            return true;
        }
    }
}
