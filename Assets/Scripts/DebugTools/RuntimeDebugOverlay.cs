using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Farming;
using MoonpetalValley.Player;
using MoonpetalValley.Chapter2;

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
        private GameObject _ch2ColumnRoot;
        private const string TestFlagId = "flag_debug_step1_verified";

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnTestToast != null) btnTestToast.onClick.AddListener(() => GameEventBus.RaiseToastRequested("Kiểm thử thông báo tiếng Việt có dấu thành công!"));
            if (btnToggleTestFlag != null) btnToggleTestFlag.onClick.AddListener(() =>
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
                {
                    GameRoot.Instance.Flags.SetFlag(TestFlagId, !GameRoot.Instance.Flags.HasFlag(TestFlagId));
                    RefreshStatusText();
                }
            });
            if (btnClearFlags != null) btnClearFlags.onClick.AddListener(() =>
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
                {
                    GameRoot.Instance.Flags.ClearAllFlags();
                    RefreshStatusText();
                }
            });
            if (btnTogglePause != null) btnTogglePause.onClick.AddListener(OnClickTogglePause);
            if (btnTpGreenhouse != null) btnTpGreenhouse.onClick.AddListener(() => TeleportTo("Spawn_Greenhouse"));
            if (btnTpVillage != null) btnTpVillage.onClick.AddListener(() => TeleportTo("Spawn_Village"));
            if (btnTpGarden != null) btnTpGarden.onClick.AddListener(() => TeleportTo("Spawn_BachGarden"));
            if (btnTpForest != null) btnTpForest.onClick.AddListener(() => TeleportTo("Spawn_ForestEdge"));
            if (btnTimePlus2H != null) btnTimePlus2H.onClick.AddListener(() => { if (GameRoot.Instance != null && GameRoot.Instance.Time != null) GameRoot.Instance.Time.AddHours(2f); RefreshStatusText(); });
            if (btnTimeSetDusk != null) btnTimeSetDusk.onClick.AddListener(() => { if (GameRoot.Instance != null && GameRoot.Instance.Time != null) GameRoot.Instance.Time.SetHour(19.83f); RefreshStatusText(); });
            if (btnTimeNextDay != null) btnTimeNextDay.onClick.AddListener(() => { if (GameRoot.Instance != null && GameRoot.Instance.Time != null) GameRoot.Instance.Time.AdvanceToNextDay(); RefreshStatusText(); });
            if (btnRestoreStats != null) btnRestoreStats.onClick.AddListener(() => { if (GameRoot.Instance != null && GameRoot.Instance.Stats != null) GameRoot.Instance.Stats.RestoreStats(100, 100, true); RefreshStatusText(); });
            if (btnForceExhaust != null) btnForceExhaust.onClick.AddListener(() => { if (GameRoot.Instance != null && GameRoot.Instance.Stats != null) GameRoot.Instance.Stats.ConsumeStamina(999, true); RefreshStatusText(); });
            if (btnDamageLightHP != null) btnDamageLightHP.onClick.AddListener(() => { if (GameRoot.Instance != null && GameRoot.Instance.Stats != null) GameRoot.Instance.Stats.TakeLightDamage(25, true); RefreshStatusText(); });
            if (btnGiveTestKit != null) btnGiveTestKit.onClick.AddListener(OnClickGiveTestKit);
            if (btnWaterAllCrops != null) btnWaterAllCrops.onClick.AddListener(() => { if (FarmGridManager.Instance != null) FarmGridManager.Instance.WaterAllTilesDebug(); RefreshStatusText(); });
            if (btnSkipPrologue != null) btnSkipPrologue.onClick.AddListener(OnClickSkipPrologue);
            if (btnGiveSickle != null) btnGiveSickle.onClick.AddListener(OnClickGiveSickle);
            if (btnPrepClimax != null) btnPrepClimax.onClick.AddListener(OnClickPrepClimax);

            BuildChapter2SideColumn();
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
            if (_ch2ColumnRoot != null) _ch2ColumnRoot.SetActive(_isOpen);
            if (_isOpen) RefreshStatusText();
        }

        private void BuildChapter2SideColumn()
        {
            if (panelRoot == null) return;

            Transform oldInline = panelRoot.transform.Find("Ch2DebugSection");
            if (oldInline != null) Destroy(oldInline.gameObject);
            Transform oldSide = panelRoot.transform.Find("DebugCh2SideColumn");
            if (oldSide != null) Destroy(oldSide.gameObject);

            RectTransform leftRect = panelRoot.GetComponent<RectTransform>();
            if (leftRect != null && leftRect.sizeDelta.y > 960f)
                leftRect.sizeDelta = new Vector2(leftRect.sizeDelta.x, 960f);
            VerticalLayoutGroup leftVlg = panelRoot.GetComponent<VerticalLayoutGroup>();
            if (leftVlg != null)
            {
                leftVlg.spacing = 4f;
                leftVlg.padding = new RectOffset(10, 10, 10, 10);
            }

            GameObject colGo = new GameObject("DebugCh2SideColumn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(LayoutElement));
            colGo.transform.SetParent(panelRoot.transform, false);
            _ch2ColumnRoot = colGo;

            LayoutElement ignoreLe = colGo.GetComponent<LayoutElement>();
            ignoreLe.ignoreLayout = true;

            RectTransform colRect = colGo.GetComponent<RectTransform>();
            colRect.anchorMin = new Vector2(1f, 1f);
            colRect.anchorMax = new Vector2(1f, 1f);
            colRect.pivot = new Vector2(0f, 1f);
            colRect.anchoredPosition = new Vector2(12f, 0f);
            colRect.sizeDelta = new Vector2(340f, 420f);

            Image bg = colGo.GetComponent<Image>();
            bg.color = new Color(0.07f, 0.09f, 0.14f, 0.94f);

            VerticalLayoutGroup vlg = colGo.GetComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(12, 12, 12, 12);
            vlg.spacing = 6f;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            ContentSizeFitter csf = colGo.GetComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            TMP_FontAsset font = statusText != null ? statusText.font : TMP_Settings.defaultFontAsset;

            GameObject headerGo = new GameObject("Ch2Header", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            headerGo.transform.SetParent(colGo.transform, false);
            TextMeshProUGUI headerTxt = headerGo.GetComponent<TextMeshProUGUI>();
            headerTxt.text = "<b>DEBUG CHƯƠNG 2 (BỜ SÔNG)</b>";
            headerTxt.font = font;
            headerTxt.fontSize = 16f;
            headerTxt.color = new Color(0.55f, 0.85f, 1f, 1f);
            headerTxt.alignment = TextAlignmentOptions.Left;
            LayoutElement headerLe = headerGo.GetComponent<LayoutElement>();
            headerLe.preferredHeight = 26f;

            Color cBlue = new Color(0.18f, 0.36f, 0.56f, 1f);
            Color cTeal = new Color(0.14f, 0.44f, 0.38f, 1f);
            Color cAmber = new Color(0.48f, 0.34f, 0.14f, 1f);
            Color cPurple = new Color(0.38f, 0.24f, 0.54f, 1f);

            AddCh2Button(colGo.transform, font, "Vào Bờ Sông (Riverbank_Chapter2)", cBlue, OnClickEnterRiverbank);
            AddCh2Button(colGo.transform, font, "Về Làng Chính (Không Intro)", cBlue, OnClickReturnToVillage);
            AddCh2Button(colGo.transform, font, "TP: Bờ Đông (Lều Nghỉ Tạm)", cTeal, () => TeleportOrFallback("Spawn_EastCamp", new Vector2(15.2f, -3.6f)));
            AddCh2Button(colGo.transform, font, "TP: Bờ Tây (Bến Bà Mara)", cTeal, () => TeleportOrFallback("Spawn_WestDock", new Vector2(-12.5f, -2.5f)));
            AddCh2Button(colGo.transform, font, "Đặt 18:30 Đêm (Điều Tra Ch2)", cAmber, OnClickSetCh2Night);
            AddCh2Button(colGo.transform, font, "Mở Khóa Đò Bờ Tây (Đã Nói Kael)", cAmber, OnClickUnlockFerry);
            AddCh2Button(colGo.transform, font, "Hoàn Thành 3 Đêm Điều Tra", cPurple, OnClickComplete3Nights);
            AddCh2Button(colGo.transform, font, "Cấp Bộ Đồ Ch2 (Đèn Bão + Ván)", cPurple, OnClickGiveCh2Kit);
        }

        private static void AddCh2Button(Transform parent, TMP_FontAsset font, string label, Color bgColor, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnGo = new GameObject("Btn_" + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
            btnGo.transform.SetParent(parent, false);

            RectTransform rt = btnGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0f, 34f);

            LayoutElement le = btnGo.GetComponent<LayoutElement>();
            le.preferredHeight = 34f;
            le.minHeight = 34f;

            Image img = btnGo.GetComponent<Image>();
            img.color = bgColor;

            Button btn = btnGo.GetComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
            cb.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            btn.colors = cb;
            btn.onClick.AddListener(onClick);

            GameObject txtGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtGo.transform.SetParent(btnGo.transform, false);
            RectTransform trt = txtGo.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = new Vector2(8f, 2f);
            trt.offsetMax = new Vector2(-8f, -2f);

            TextMeshProUGUI tmp = txtGo.GetComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.font = font;
            tmp.fontSize = 14f;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
        }

        private void OnClickEnterRiverbank()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag("flag_nv1_completed", true);
            }
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.PendingSceneSpawnId = "Spawn_EastEntrance";
            }
            SceneManager.LoadScene("Riverbank_Chapter2");
        }

        private void OnClickReturnToVillage()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag("flag_intro_cutscene_done", true);
                GameRoot.Instance.Flags.SetFlag("flag_tutorial_done", true);
            }
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.PendingSceneSpawnId = "Spawn_FromRiverbank";
            }
            SceneManager.LoadScene("MoonpetalValley_MVP");
        }

        private void TeleportOrFallback(string spawnId, Vector2 fallbackPos)
        {
            if (WorldSpawnRegistry.Instance != null && WorldSpawnRegistry.Instance.TeleportPlayerTo(spawnId))
            {
                RefreshStatusText();
                return;
            }
            PlayerController2D pc = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (pc != null)
            {
                pc.TeleportTo(fallbackPos);
            }
            else
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) player.transform.position = new Vector3(fallbackPos.x, fallbackPos.y, 0f);
            }
            RefreshStatusText();
        }

        private void OnClickSetCh2Night()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
                GameRoot.Instance.Flags.SetFlag("flag_ch2_intro_done", true);
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
                GameRoot.Instance.Time.SetHour(18.5f);
            GameEventBus.RaiseToastRequested("Đã chuyển sang 18:30 (Đêm Điều Tra Chương 2)!");
            RefreshStatusText();
        }

        private void OnClickUnlockFerry()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag("flag_ch2_intro_done", true);
                GameRoot.Instance.Flags.SetFlag("flag_ch2_talked_kael_day1", true);
                GameRoot.Instance.Flags.SetFlag("flag_ch2_ferry_unlocked", true);
            }
            GameEventBus.RaiseToastRequested("Đã mở khóa Đò sang Bờ Tây!");
            RefreshStatusText();
        }

        private void OnClickComplete3Nights()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag("flag_ch2_intro_done", true);
                GameRoot.Instance.Flags.SetFlag("flag_ch2_talked_kael_day1", true);
                GameRoot.Instance.Flags.SetFlag("flag_ch2_ferry_unlocked", true);
                GameRoot.Instance.Flags.SetFlag("flag_ch2_inv_night1_done", true);
                GameRoot.Instance.Flags.SetFlag("flag_ch2_inv_night2_done", true);
                GameRoot.Instance.Flags.SetFlag("flag_ch2_inv_night3_done", true);
            }
            GameEventBus.RaiseToastRequested("Đã hoàn thành 3 Đêm Điều Tra Chương 2!");
            RefreshStatusText();
        }

        private void OnClickGiveCh2Kit()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            inv.AddItemById("item_storm_lantern_west", 1, false);
            inv.AddItemById("item_river_timber", 3, false);
            inv.AddItemById("item_waterproof_resin", 2, false);
            GameEventBus.RaiseToastRequested("Đã cấp Đèn Bão Bờ Tây + 3 Gỗ Bờ Sông + 2 Nhựa Chống Nước!");
            RefreshStatusText();
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

        private void OnClickGiveSickle()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            if (!inv.HasItem("item_moon_sickle", 1)) inv.AddItemById("item_moon_sickle", 1, true);
            inv.SelectHotbarSlot(3);
            RefreshStatusText();
        }

        private void OnClickSkipPrologue()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            var f = GameRoot.Instance.Flags;
            f.SetFlag("flag_intro_cutscene_done", true);
            f.SetFlag("flag_intro_letter_read", true);
            f.SetFlag("flag_greenhouse_cleared", true);
            f.SetFlag("flag_lumi_awakened", true);
            f.SetFlag("flag_solo_plant_failed", true);
            f.SetFlag("flag_tutorial_done", true);
            if (GameRoot.Instance.Inventory != null)
                GameRoot.Instance.Inventory.AddItemById("item_moonpetal_seed_core", 1, false);
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

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Riverbank_Chapter2" && Chapter2StateManager.Instance != null)
            {
                sb.AppendLine("Ch2 | Cầu: " + Chapter2StateManager.Instance.BridgeState + " | Đêm: " + Chapter2StateManager.Instance.InvestigationStage + "/3");
            }
            else if (GameRoot.Instance.Bundle != null)
            {
                sb.AppendLine("Helpers: " + GameRoot.Instance.Bundle.GetReadyHelperCount() + "/4 | Bundle: " +
                              (GameRoot.Instance.Bundle.IsBundleItemsComplete() ? "ĐỦ" : "CHƯA"));
            }
            statusText.text = sb.ToString();
        }
    }
}
