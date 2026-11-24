using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Inventory;

namespace MoonpetalValley.Chapter3
{
    public class NightMarketServiceManager : MonoBehaviour
    {
        public static NightMarketServiceManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private NightMarketHUDView hudView;
        [SerializeField] private List<NightMarketCustomerSpot> customerSpots = new List<NightMarketCustomerSpot>();

        [Header("Cozy Balance")]
        [SerializeField] private float startingWarmth = 20f;
        [SerializeField] private float exactMatchWarmthGain = 18f;
        [SerializeField] private float substituteItemWarmthGain = 12f;
        [SerializeField] private float warmChatFallbackGain = 8f;
        [SerializeField] private int maxSimultaneousOrders = 3;

        private static readonly (string itemId, string displayName)[] DefaultMarketGoods =
        {
            ("item_ch3_seed_cake", "Bánh Hạt Mầm Bà Hana"),
            ("item_ch3_river_tea", "Trà Cúc Sông Ấm"),
            ("item_ch3_paper_fish", "Cá Giấy Phát Sáng"),
            ("item_ch3_wish_card", "Thiệp Ước Nguyện"),
            ("item_ch3_lantern_charm", "Bùa Đèn Lồng Nhỏ")
        };

        private bool _isServiceActive;
        private bool _isTransitioning;
        private bool _notifiedMaxWarmth;
        private float _remainingSeconds;
        private float _warmthScore;
        private int _ordersServed;
        private float _nextOrderCheckTimer;

        public bool IsServiceActive => _isServiceActive;
        public bool IsTransitioning => _isTransitioning;
        public float RemainingSeconds => _remainingSeconds;
        public float WarmthScore => _warmthScore;
        public int OrdersServed => _ordersServed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            ClearAllOrders();
            if (hudView != null) hudView.SetVisible(false);
        }

        private void Update()
        {
            if (!_isServiceActive) return;
            if (GameRoot.Instance != null && GameRoot.Instance.State != null &&
                !GameRoot.Instance.State.IsGameplayActive)
            {
                return;
            }

            _remainingSeconds -= Time.deltaTime;
            _nextOrderCheckTimer -= Time.deltaTime;
            if (_nextOrderCheckTimer <= 0f)
            {
                _nextOrderCheckTimer = 1.0f;
                EnsureActiveOrders();
            }

            UpdateHudNow();

            if (_remainingSeconds <= 0f)
            {
                _remainingSeconds = 0f;
                CompleteMarketService();
            }
        }

        public void RequestStartMarketService()
        {
            if (_isServiceActive || _isTransitioning) return;
            _isTransitioning = true;

            if (GameRoot.Instance != null && GameRoot.Instance.Time != null &&
                GameRoot.Instance.Time.CurrentHourFloat < 18f)
            {
                GameRoot.Instance.Time.SetHour(18.5f);
            }

            if (Chapter3StateManager.Instance != null)
            {
                Chapter3StateManager.Instance.SetPhase(Chapter3Phase.NightMarketActive);
                Chapter3StateManager.Instance.SetNightMarketMusicActive(true);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.Dialogue != null)
            {
                GameRoot.Instance.Dialogue.StartQuickLines(new (string, string)[]
                {
                    ("Bà Hana", "Đèn lồng đã sáng rực cả cây cầu rồi! Chợ Đêm đầu tiên của thung lũng chính thức khai hội!"),
                    ("Ông Bach", "Dân làng hai bờ đang tấp nập kéo tới. Wen ơi, cháu phụ tiếp món cho các gian hàng nhé!"),
                    ("Tinh Linh Lumi", "Cứ thong thả vui hội nha Wen! Dù hết món thủ công thì trò chuyện ấm áp vẫn làm chợ rộn ràng lắm!")
                }, BeginServiceLoop);
            }
            else
            {
                BeginServiceLoop();
            }
        }

        public void DebugStartServiceImmediate()
        {
            if (_isServiceActive) return;
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null &&
                GameRoot.Instance.Time.CurrentHourFloat < 18f)
            {
                GameRoot.Instance.Time.SetHour(18.5f);
            }
            if (Chapter3StateManager.Instance != null)
            {
                Chapter3StateManager.Instance.SetPhase(Chapter3Phase.NightMarketActive);
                Chapter3StateManager.Instance.SetNightMarketMusicActive(true);
            }
            BeginServiceLoop();
        }

        public void DebugFinishService(float warmthOverride)
        {
            _warmthScore = Mathf.Clamp(warmthOverride, 0f, 100f);
            _remainingSeconds = 0f;
            _isServiceActive = true;
            CompleteMarketService();
        }

        private void BeginServiceLoop()
        {
            _isTransitioning = false;
            _isServiceActive = true;
            _notifiedMaxWarmth = false;
            _ordersServed = 0;
            _warmthScore = Mathf.Clamp(startingWarmth, 0f, 100f);
            _remainingSeconds = GetConfiguredDuration();
            _nextOrderCheckTimer = 0.2f;

            ClearAllOrders();
            EnsureActiveOrders();

            if (Chapter3StateManager.Instance != null)
                Chapter3StateManager.Instance.SetWarmthScore(_warmthScore);

            if (hudView != null) hudView.SetVisible(true);
            UpdateHudNow();
            GameEventBus.RaiseToastRequested("Chợ Đêm khai hội! Phục vụ dân làng dọc cây cầu trong " + Mathf.RoundToInt(_remainingSeconds) + " giây!");
        }

        public void ServeSpot(NightMarketCustomerSpot spot)
        {
            if (!_isServiceActive || spot == null || !spot.HasActiveOrder) return;

            InventoryManager inv = GameRoot.Instance != null ? GameRoot.Instance.Inventory : null;
            float gain = warmChatFallbackGain;
            string toastMsg;

            if (inv != null && !string.IsNullOrEmpty(spot.RequestedItemId) && inv.GetItemCount(spot.RequestedItemId) > 0)
            {
                inv.RemoveItem(spot.RequestedItemId, 1);
                gain = exactMatchWarmthGain;
                toastMsg = spot.VillagerName + ": \"Đúng món " + spot.RequestedItemName + " rồi! Ngon và ấm lòng quá!\" (+" + Mathf.RoundToInt(gain) + "% Ấm Áp)";
            }
            else if (TryConsumeAnyMarketGood(inv, out string subName))
            {
                gain = substituteItemWarmthGain;
                toastMsg = spot.VillagerName + ": \"Ồ, " + subName + " cũng tuyệt lắm! Cảm ơn Wen nhé!\" (+" + Mathf.RoundToInt(gain) + "% Ấm Áp)";
            }
            else
            {
                gain = warmChatFallbackGain;
                toastMsg = spot.VillagerName + ": \"Hết đồ cũng không sao, ngồi nghe tiếng đèn gió cùng cháu là vui rồi!\" (+" + Mathf.RoundToInt(gain) + "% Ấm Áp)";
            }

            _ordersServed++;
            _warmthScore = Mathf.Clamp(_warmthScore + gain, 0f, 100f);
            if (Chapter3StateManager.Instance != null)
                Chapter3StateManager.Instance.SetWarmthScore(_warmthScore);

            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                GameRoot.Instance.Audio.PlayUIClick();

            spot.ClearOrder();
            GameEventBus.RaiseToastRequested(toastMsg);

            if (_warmthScore >= 100f && !_notifiedMaxWarmth)
            {
                _notifiedMaxWarmth = true;
                GameEventBus.RaiseToastRequested("Không khí Chợ Đêm đã đạt 100% Ấm Áp! Thung lũng rộn ràng tiếng cười!");
            }

            EnsureActiveOrders();
            UpdateHudNow();
        }

        private void CompleteMarketService()
        {
            _isServiceActive = false;
            _isTransitioning = false;
            ClearAllOrders();
            if (hudView != null) hudView.SetVisible(false);

            float threshold = GetBonusThreshold();
            bool earnedBonus = _warmthScore >= threshold;

            if (Chapter3StateManager.Instance != null)
            {
                Chapter3StateManager.Instance.SetWarmthScore(_warmthScore);
                Chapter3StateManager.Instance.SetNightMarketMusicActive(true);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag(Chapter3StateManager.FlagCh3MarketDone, true);
                if (earnedBonus)
                    GameRoot.Instance.Flags.SetFlag(Chapter3StateManager.FlagCh3WarmthBonus, true);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.Quests != null)
                GameRoot.Instance.Quests.RecomputeObjective();

            if (GameRoot.Instance != null && GameRoot.Instance.Dialogue != null)
            {
                if (earnedBonus)
                {
                    GameRoot.Instance.Dialogue.StartQuickLines(new (string, string)[]
                    {
                        ("Bà Hana", "Hơn cả mong đợi! Độ ấm áp Chợ Đêm chạm mốc " + Mathf.RoundToInt(_warmthScore) + "% rồi Wen ơi!"),
                        ("Bé Toby", "Con chưa từng thấy hai bờ sông đông vui thế này! Cá giấy của con bay sáng rực cả mặt nước luôn!"),
                        ("Ông Kael", "Cây cầu này không chỉ nối hai bờ đất... đêm nay nó thật sự nối lại cả ngôi làng rồi."),
                        ("Bà Mara", "Cháu đi dạo một vòng 7 gian hàng nhé Wen. Mỗi người đều giữ sẵn một món Kỷ Vật muốn gửi tặng cháu đấy!")
                    }, null);
                }
                else
                {
                    GameRoot.Instance.Dialogue.StartQuickLines(new (string, string)[]
                    {
                        ("Bà Hana", "Phiên Chợ Đêm đầu tiên khép lại thật êm ấm! Cảm ơn cháu đã tất bật suốt tối nay, Wen."),
                        ("Ông Bach", "Cây cầu sáng đèn, ai nấy đều cười vui. Cháu thong thả ghé từng gian hàng nhận Kỷ Vật của mọi người nhé!")
                    }, null);
                }
            }
        }

        private void EnsureActiveOrders()
        {
            if (customerSpots == null || customerSpots.Count == 0) return;
            int activeCount = 0;
            for (int i = 0; i < customerSpots.Count; i++)
            {
                if (customerSpots[i] != null && customerSpots[i].HasActiveOrder)
                    activeCount++;
            }

            int target = Mathf.Clamp(maxSimultaneousOrders, 1, customerSpots.Count);
            int safety = 0;
            while (activeCount < target && safety < 24)
            {
                safety++;
                NightMarketCustomerSpot candidate = PickIdleSpot();
                if (candidate == null) break;
                PickGoodForOrder(out string itemId, out string displayName);
                candidate.SetOrder(itemId, displayName);
                activeCount++;
            }
        }

        private NightMarketCustomerSpot PickIdleSpot()
        {
            List<NightMarketCustomerSpot> idle = new List<NightMarketCustomerSpot>();
            for (int i = 0; i < customerSpots.Count; i++)
            {
                if (customerSpots[i] != null && !customerSpots[i].HasActiveOrder)
                    idle.Add(customerSpots[i]);
            }
            if (idle.Count == 0) return null;
            return idle[Random.Range(0, idle.Count)];
        }

        private void PickGoodForOrder(out string itemId, out string displayName)
        {
            NightMarketConfigSO cfg = Chapter3StateManager.Instance != null ? Chapter3StateManager.Instance.Config : null;
            if (cfg != null && cfg.recipes != null && cfg.recipes.Count > 0)
            {
                NightMarketRecipeDef r = cfg.recipes[Random.Range(0, cfg.recipes.Count)];
                if (r != null && !string.IsNullOrEmpty(r.outputItemId))
                {
                    itemId = r.outputItemId;
                    displayName = string.IsNullOrEmpty(r.displayName) ? r.outputItemId : r.displayName;
                    return;
                }
            }
            int idx = Random.Range(0, DefaultMarketGoods.Length);
            itemId = DefaultMarketGoods[idx].itemId;
            displayName = DefaultMarketGoods[idx].displayName;
        }

        private bool TryConsumeAnyMarketGood(InventoryManager inv, out string consumedName)
        {
            consumedName = "";
            if (inv == null) return false;
            for (int i = 0; i < DefaultMarketGoods.Length; i++)
            {
                if (inv.GetItemCount(DefaultMarketGoods[i].itemId) > 0)
                {
                    inv.RemoveItem(DefaultMarketGoods[i].itemId, 1);
                    consumedName = DefaultMarketGoods[i].displayName;
                    return true;
                }
            }
            return false;
        }

        private void ClearAllOrders()
        {
            if (customerSpots == null) return;
            for (int i = 0; i < customerSpots.Count; i++)
            {
                if (customerSpots[i] != null)
                    customerSpots[i].ClearOrder();
            }
        }

        private void UpdateHudNow()
        {
            if (hudView == null) return;
            string summary = BuildActiveRequestSummary();
            hudView.UpdateHUD(_remainingSeconds, _warmthScore, GetBonusThreshold(), _ordersServed, summary);
        }

        private string BuildActiveRequestSummary()
        {
            if (customerSpots == null) return "";
            List<string> parts = new List<string>();
            for (int i = 0; i < customerSpots.Count; i++)
            {
                NightMarketCustomerSpot sp = customerSpots[i];
                if (sp != null && sp.HasActiveOrder)
                    parts.Add(sp.VillagerName + ": " + sp.RequestedItemName);
            }
            if (parts.Count == 0)
                return "Ghé các gian hàng có biểu tượng trái tim để phục vụ dân làng!";
            return "Đang chờ: " + string.Join(" | ", parts);
        }

        private float GetConfiguredDuration()
        {
            NightMarketConfigSO cfg = Chapter3StateManager.Instance != null ? Chapter3StateManager.Instance.Config : null;
            return cfg != null && cfg.serviceDurationSeconds > 5f ? cfg.serviceDurationSeconds : 100f;
        }

        private float GetBonusThreshold()
        {
            NightMarketConfigSO cfg = Chapter3StateManager.Instance != null ? Chapter3StateManager.Instance.Config : null;
            return cfg != null && cfg.warmthBonusThreshold > 0f ? cfg.warmthBonusThreshold : 70f;
        }
    }
}
