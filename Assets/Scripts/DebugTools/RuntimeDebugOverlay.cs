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

        private bool _isOpen;
        private float _refreshTimer;
        private const string TestFlagId = "flag_debug_step1_verified";

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnTestToast != null) btnTestToast.onClick.AddListener(OnClickTestToast);
            if (btnToggleTestFlag != null) btnToggleTestFlag.onClick.AddListener(OnClickToggleTestFlag);
            if (btnClearFlags != null) btnClearFlags.onClick.AddListener(OnClickClearFlags);
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
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            inv.AddItemById("item_sunshine_bread", 2, false);
            inv.AddItemById("item_branch_wood", 10, false);
            inv.AddItemById("item_dew_barley", 4, false);
            inv.AddItemById("item_dew_barley_seed", 6, false);
            GameEventBus.RaiseToastRequested("Đã thêm Bộ Test (2 Bánh Mì, 10 Gỗ, 4 Lúa Mạch, 6 Hạt)!");
            RefreshStatusText();
        }

        private void OnClickSkipPrologue()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            var f = GameRoot.Instance.Flags;
            f.SetFlag("flag_intro_letter_read", true);
            f.SetFlag("flag_greenhouse_cleared", true);
            f.SetFlag("flag_lumi_awakened", true);
            f.SetFlag("flag_solo_plant_failed", true);
            f.SetFlag("flag_tutorial_done", true);
            GameRoot.Instance.Inventory?.AddItemById("item_moonpetal_seed_core", 1, false);
            GameEventBus.RaiseToastRequested("Đã bỏ qua Mở Đầu — sẵn sàng gặp Ông Bách làm Nhiệm Vụ 1!");
            RefreshStatusText();
        }

        private void OnClickTestToast()
        {
            GameEventBus.RaiseToastRequested("Kiểm thử thông báo tiếng Việt có dấu thành công!");
        }

        private void OnClickToggleTestFlag()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            GameRoot.Instance.Flags.SetFlag(TestFlagId, !GameRoot.Instance.Flags.HasFlag(TestFlagId));
            RefreshStatusText();
        }

        private void OnClickClearFlags() { GameRoot.Instance?.Flags?.ClearAllFlags(); RefreshStatusText(); }

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
            if (GameRoot.Instance.Stats != null)
            {
                var st = GameRoot.Instance.Stats;
                sb.AppendLine(string.Format("HP: {0}/{1} | Stamina: {2}/{3}",
                    st.CurrentLightHP, st.MaxLightHP, st.CurrentStamina, st.EffectiveMaxStamina));
            }
            if (GameRoot.Instance.Diary != null)
            {
                sb.AppendLine("Sổ tay: " + GameRoot.Instance.Diary.UnlockedCount + "/4 trang | Flags: " + GameRoot.Instance.Flags.GetAllActiveFlags().Count);
            }
            statusText.text = sb.ToString();
        }
    }
}
