using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Farming;

namespace MoonpetalValley.DebugTools
{
    public class RuntimeDebugOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button btnTestToast;
        [SerializeField] private Button btnToggleTestFlag;
        [SerializeField] private Button btnClearFlags;
        [SerializeField] private Button btnTogglePause;
        [SerializeField] private Button btnTpGreenhouse;
        [SerializeField] private Button btnTpVillage;
        [SerializeField] private Button btnTpGarden;
        [SerializeField] private Button btnTpForest;
        [SerializeField] private Button btnTimePlus2H;
        [SerializeField] private Button btnTimeSetDusk;
        [SerializeField] private Button btnTimeNextDay;
        [SerializeField] private Button btnRestoreStats;
        [SerializeField] private Button btnForceExhaust;
        [SerializeField] private Button btnDamageLightHP;
        [SerializeField] private Button btnGiveTestKit;
        [SerializeField] private Button btnWaterAllCrops;
        [SerializeField] private Button btnSkipPrologue;
        [SerializeField] private Button btnGiveSickle;
        [SerializeField] private Button btnPrepClimax;

        private bool _isOpen;
        private float _refreshTimer;
        private const string TestFlagId = "flag_debug_step1_verified";

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnTestToast != null) btnTestToast.onClick.AddListener(() => GameEventBus.RaiseToastRequested("Kiểm thử thông báo tiếng Việt có dấu thành công!"));
            if (btnToggleTestFlag != null) btnToggleTestFlag.onClick.AddListener(() => { GameRoot.Instance?.Flags?.SetFlag(TestFlagId, !GameRoot.Instance.Flags.HasFlag(TestFlagId)); RefreshStatusText(); });
            if (btnClearFlags != null) btnClearFlags.onClick.AddListener(() => { GameRoot.Instance?.Flags?.ClearAllFlags(); RefreshStatusText(); });
            if (btnTogglePause != null) btnTogglePause.onClick.AddListener(OnClickTogglePause);
            if (btnTpGreenhouse != null) btnTpGreenhouse.onClick.AddListener(() => TeleportTo("Spawn_Greenhouse"));
            if (btnTpVillage != null) btnTpVillage.onClick.AddListener(() => TeleportTo("Spawn_Village"));
            if (btnTpGarden != null) btnTpGarden.onClick.AddListener(() => TeleportTo("Spawn_BachGarden"));
            if (btnTpForest != null) btnTpForest.onClick.AddListener(() => TeleportTo("Spawn_ForestEdge"));
            if (btnTimePlus2H != null) btnTimePlus2H.onClick.AddListener(() => { GameRoot.Instance?.Time?.AddHours(2f); RefreshStatusText(); });
            if (btnTimeSetDusk != null) btnTimeSetDusk.onClick.AddListener(() => { GameRoot.Instance?.Time?.SetHour(19.83f); RefreshStatusText(); });
            if (btnTimeNextDay != null) btnTimeNextDay.onClick.AddListener(() => { GameRoot.Instance?.Time?.AdvanceToNextDay(); RefreshStatusText(); });
            if (btnRestoreStats != null) btnRestoreStats.onClick.AddListener(() => { GameRoot.Instance?.Stats?.RestoreStats(100, 100, true); RefreshStatusText(); });
            if (btnForceExhaust != null) btnForceExhaust.onClick.AddListener(() => { GameRoot.Instance?.Stats?.ConsumeStamina(999, true); RefreshStatusText(); });
            if (btnDamageLightHP != null) btnDamageLightHP.onClick.AddListener(() => { GameRoot.Instance?.Stats?.TakeLightDamage(25, true); RefreshStatusText(); });
            if (btnGiveTestKit != null) btnGiveTestKit.onClick.AddListener(OnClickGiveTestKit);
            if (btnWaterAllCrops != null) btnWaterAllCrops.onClick.AddListener(() => { FarmGridManager.Instance?.WaterAllTilesDebug(); RefreshStatusText(); });
            if (btnSkipPrologue != null) btnSkipPrologue.onClick.AddListener(OnClickSkipPrologue);
            if (btnGiveSickle != null) btnGiveSickle.onClick.AddListener(OnClickGiveSickle);
            if (btnPrepClimax != null) btnPrepClimax.onClick.AddListener(OnClickPrepClimax);
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged += OnGameStateUpdated;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged -= OnGameStateUpdated;
        }

        private void Update()
        {
            if (!Application.isEditor && !Debug.isDebugBuild) return;
            if (WasF1PressedThisFrame()) TogglePanel();

            if (_isOpen)
            {
                _refreshTimer += Time.unscaledDeltaTime;
                if (_refreshTimer >= 0.25f) { _refreshTimer = 0f; RefreshStatusText(); }
            }
        }

        private bool WasF1PressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F1)) return true;
#endif
            return false;
        }

        public void TogglePanel()
        {
            _isOpen = !_isOpen;
            if (panelRoot != null) panelRoot.SetActive(_isOpen);
            if (_isOpen) RefreshStatusText();
        }

        private void TeleportTo(string spawnId)
        {
            if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo(spawnId);
                RefreshStatusText();
            }
        }

        private void OnClickGiveTestKit()
        {
            var inv = GameRoot.Instance?.Inventory;
            if (inv == null) return;
            inv.AddItemById("item_sunshine_bread", 2, false);
            inv.AddItemById("item_branch_wood", 10, false);
            inv.AddItemById("item_dew_barley", 4, false);
            inv.AddItemById("item_dew_barley_seed", 6, false);
            GameEventBus.RaiseToastRequested("Đã thêm Bộ Test (2 Bánh Mì, 10 Gỗ, 4 Lúa Mạch, 6 Hạt)!");
            RefreshStatusText();
        }

        private void OnClickGiveSickle()
        {
            var inv = GameRoot.Instance?.Inventory;
            if (inv == null) return;
            if (!inv.HasItem("item_moon_sickle", 1)) inv.AddItemById("item_moon_sickle", 1, true);
            inv.SelectHotbarSlot(3);
            RefreshStatusText();
        }

        private void OnClickSkipPrologue()
        {
            var f = GameRoot.Instance?.Flags;
            if (f == null) return;
            f.SetFlag("flag_intro_letter_read", true);
            f.SetFlag("flag_greenhouse_cleared", true);
            f.SetFlag("flag_lumi_awakened", true);
            f.SetFlag("flag_solo_plant_failed", true);
            f.SetFlag("flag_tutorial_done", true);
            GameRoot.Instance.Inventory?.AddItemById("item_moonpetal_seed_core", 1, false);
            GameEventBus.RaiseToastRequested("Đã bỏ qua Mở Đầu!");
            RefreshStatusText();
        }

        private void OnClickPrepClimax()
        {
            if (GameRoot.Instance == null) return;
            OnClickSkipPrologue();
            var f = GameRoot.Instance.Flags;
            f.SetFlag("flag_met_bach_at_gate", true);
            f.SetFlag("flag_garden_slimes_cleared", true);
            f.SetFlag("flag_garden_bench_cleaned", true);
            f.SetFlag("flag_hana_bread_done", true);
            f.SetFlag("flag_toby_cat_slimes_cleared", true);
            f.SetFlag("flag_toby_cat_saved", true);
            f.SetFlag("flag_otis_mailbox_fixed", true);
            f.SetFlag("flag_bach_letters_read", true);

            var inv = GameRoot.Instance.Inventory;
            if (inv != null)
            {
                if (!inv.HasItem("item_seed_bag_1", 1)) inv.AddItemById("item_seed_bag_1", 1, false);
                if (!inv.HasItem("item_seed_bag_2", 1)) inv.AddItemById("item_seed_bag_2", 1, false);
                if (!inv.HasItem("item_seed_bag_3", 1)) inv.AddItemById("item_seed_bag_3", 1, false);
                if (inv.GetItemCount("item_dew_sap") < 5) inv.AddItemById("item_dew_sap", 5, false);
                if (inv.GetItemCount("item_starlight_dust") < 3) inv.AddItemById("item_starlight_dust", 3, false);
            }

            TeleportTo("Spawn_BachGarden");
            GameEventBus.RaiseToastRequested("Đã gọi đủ 4 Helpers & cấp đủ đồ Bundle! Hãy tới Bồn Hoa bấm [E]!");
            RefreshStatusText();
        }

        private void OnClickTogglePause()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            var next = GameRoot.Instance.State.IsGameplayActive ? GameState.PauseMenu : GameState.Gameplay;
            GameRoot.Instance.State.SetState(next);
            RefreshStatusText();
        }

        private void OnAnyStateUpdated(string _, bool __) => RefreshStatusText();
        private void OnGameStateUpdated(GameState _) => RefreshStatusText();

        public void RefreshStatusText()
        {
            if (statusText == null || GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("<b>BẢNG DEBUG MOONPETAL (F1)</b>");
            if (GameRoot.Instance.Time != null) sb.AppendLine(GameRoot.Instance.Time.GetFormattedClockString());
            if (GameRoot.Instance.Bundle != null)
            {
                sb.AppendLine("Helpers: " + GameRoot.Instance.Bundle.GetReadyHelperCount() + "/4 | Bundle: " +
                              (GameRoot.Instance.Bundle.IsBundleItemsComplete() ? "ĐỦ" : "CHƯA"));
            }
            statusText.text = sb.ToString();
        }
    }
}
