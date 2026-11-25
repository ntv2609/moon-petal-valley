using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2
{
    public class Chapter2EndingController : MonoBehaviour
    {
        public const string FlagRelayDone = "flag_ch2_relay_done";
        public const string FlagCh2Completed = "flag_ch2_completed";
        public const string ItemPetalOfBondId = "item_petal_of_bond";

        public static Chapter2EndingController Instance { get; private set; }

        [Header("Scene References")]
        [SerializeField] private StormLanternActor bridgeStormLantern;
        [SerializeField] private Vector3 bridgeFocusPos = new Vector3(0f, 0.9f, -10f);
        [SerializeField] private Vector3 kaelEndPos = new Vector3(1.55f, 0.78f, 0f);
        [SerializeField] private Vector3 maraEndPos = new Vector3(-1.55f, 0.78f, 0f);
        [SerializeField] private Vector2 wenEndPos = new Vector2(0f, 0.45f);

        private bool _isPlaying;
        private GameObject _summaryPanelRoot;
        private CameraFollow2D _cam;

        public bool IsPlaying => _isPlaying;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnSaveLoaded += SyncBridgeLanternVisual;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnSaveLoaded -= SyncBridgeLanternVisual;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            SyncBridgeLanternVisual();
            RescuePlayerFromBridgeRailIfStuck();
            if (HasFlag(FlagRelayDone) && !HasFlag(FlagCh2Completed))
            {
                TriggerEndingSequence();
            }
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            if (flagId == FlagRelayDone && !HasFlag(FlagCh2Completed) && !_isPlaying)
            {
                TriggerEndingSequence();
            }
            else if (flagId == FlagCh2Completed)
            {
                SyncBridgeLanternVisual();
            }
        }

        public void SyncBridgeLanternVisual()
        {
            if (bridgeStormLantern == null) return;
            bool showLit = _isPlaying || HasFlag(FlagCh2Completed);
            bridgeStormLantern.gameObject.SetActive(showLit);
            if (showLit)
            {
                bridgeStormLantern.SetState(StormLanternVisualState.Lit);
            }
        }

        public void TriggerEndingSequence()
        {
            if (_isPlaying || HasFlag(FlagCh2Completed)) return;
            StartCoroutine(PlayEndingRoutine());
        }

        private IEnumerator PlayEndingRoutine()
        {
            _isPlaying = true;
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.Cutscene);
            }

            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.SetBridgeState(RiverBridgeState.Repaired);
                Chapter2StateManager.Instance.SetRelayEventActive(false);
            }

            // Đặt Wen, Kael, Mara và Đèn Bão đã ghép sáng rực giữa cầu
            PlayerController2D player = FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                Vector2 safeWenEnd = new Vector2(wenEndPos.x, Mathf.Clamp(Mathf.Max(wenEndPos.y, 0.45f), 0.40f, 0.95f));
                player.TeleportTo(safeWenEnd);
                player.SetFacingDirection(Vector2.up);
            }

            GameObject kaelGo = GameObject.Find("NPC_Kael");
            if (kaelGo != null) kaelGo.transform.position = kaelEndPos;
            GameObject maraGo = GameObject.Find("NPC_Mara");
            if (maraGo != null) maraGo.transform.position = maraEndPos;

            if (bridgeStormLantern != null)
            {
                bridgeStormLantern.gameObject.SetActive(true);
                bridgeStormLantern.SetState(StormLanternVisualState.Lit);
            }

            _cam = FindAnyObjectByType<CameraFollow2D>();
            if (_cam != null)
            {
                _cam.SetCutsceneTarget(bridgeFocusPos, 4.6f, 2.2f);
            }

            yield return new WaitForSecondsRealtime(0.45f);

            // Hiệu ứng nở Cánh Hoa Gắn Kết + Lumi tỏa sáng
            LumiCompanionController lumi = FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null) lumi.TriggerPulse(2.0f);

            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.TriggerCameraShake(0.14f, 0.22f);
                JuiceEffectsManager.Instance.SpawnFloatingNumber(
                    new Vector3(0f, 1.85f, 0f),
                    "Cánh Hoa Gắn Kết!",
                    new Color(1.0f, 0.90f, 0.56f, 1f));
            }

            // Trao phần thưởng Chương 2 & bật cờ hoàn thành (tự phát Stinger S1 + lưu trạng thái cầu)
            if (GameRoot.Instance != null)
            {
                if (GameRoot.Instance.Inventory != null && !GameRoot.Instance.Inventory.HasItem(ItemPetalOfBondId, 1))
                {
                    GameRoot.Instance.Inventory.AddItemById(ItemPetalOfBondId, 1, true);
                }
                if (GameRoot.Instance.Flags != null)
                {
                    if (!GameRoot.Instance.Flags.HasFlag("flag_nv1_completed"))
                        GameRoot.Instance.Flags.SetFlag("flag_nv1_completed", true);
                    GameRoot.Instance.Flags.SetFlag(FlagCh2Completed, true);
                }
            }

            yield return new WaitForSecondsRealtime(0.55f);

            bool dialogueDone = false;
            if (GameRoot.Instance != null && GameRoot.Instance.Dialogue != null)
            {
                GameRoot.Instance.Dialogue.StartQuickLines(new (string, string)[]
                {
                    ("Ông Kael", "Chiếc đèn bão năm xưa lại sáng rực giữa dòng sông rồi... Cảm ơn cháu, Wen."),
                    ("Bà Mara", "Nhờ cháu mà hai bờ không còn cách trở nữa. Cánh Hoa Gắn Kết kia xứng đáng thuộc về cháu."),
                    ("Tinh Linh Lumi", "Woa! Nhờ Cánh Hoa Gắn Kết mà Xung Ánh Trăng [Q] của Lumi đã thăng lên Cấp 2 rồi! Tầm tỏa rộng hơn và còn làm CHOÁNG quái tới 2 giây nữa đó!"),
                    ("Tinh Linh Lumi", "...Khoan đã Wen. Khi cánh hoa vừa sáng lên, Lumi cảm nhận một luồng gió lạnh từ phía Bắc... Tiếng khóc trong rừng lớn hơn trước..."),
                    ("Wen", "Tiếng khóc từ rừng phía Bắc sao? Trước khi đi xa hơn, mình nên về Làng Chính báo tin vui cho mọi người — nghe bác Otis bảo cả làng sắp mở Chợ Đêm Đầu Tiên.")
                }, () => dialogueDone = true);

                while (!dialogueDone) yield return null;
            }

            ShowCompletionModal();
        }

        private void ShowCompletionModal()
        {
            EnsureSummaryModal();
            if (_summaryPanelRoot != null)
            {
                _summaryPanelRoot.SetActive(true);
            }
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.Cutscene);
            }
        }

        private void CloseCompletionModal()
        {
            if (_summaryPanelRoot != null)
            {
                _summaryPanelRoot.SetActive(false);
            }
            if (_cam != null && _cam.IsCutsceneOverride)
            {
                _cam.EndCutsceneOverride(false);
            }
            _isPlaying = false;
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
            RescuePlayerFromBridgeRailIfStuck();
            var schedules = FindObjectsByType<Chapter2NPCSchedule>(FindObjectsInactive.Include);
            for (int i = 0; i < schedules.Length; i++) if (schedules[i] != null) schedules[i].ForceRefreshSchedule();
            GameEventBus.RaiseToastRequested("HOÀN THÀNH CHƯƠNG 2! Mở khóa Xung Ánh Trăng Cấp 2 [Q] (Choáng 2s)!");
        }

        private void EnsureSummaryModal()
        {
            if (_summaryPanelRoot != null) return;

            GameObject canvasGo = new GameObject("Ch2CompletionCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas cv = canvasGo.GetComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cv.sortingOrder = 230;
            CanvasScaler cs = canvasGo.GetComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920f, 1080f);
            cs.matchWidthOrHeight = 0.5f;

            TMP_FontAsset font = ResolveVnFont();

            _summaryPanelRoot = new GameObject("Ch2CompletionBackdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            _summaryPanelRoot.transform.SetParent(canvasGo.transform, false);
            RectTransform bgRt = _summaryPanelRoot.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;
            _summaryPanelRoot.GetComponent<Image>().color = new Color(0.04f, 0.06f, 0.11f, 0.72f);

            GameObject card = new GameObject("Card", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            card.transform.SetParent(_summaryPanelRoot.transform, false);
            RectTransform crt = card.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0.5f, 0.5f);
            crt.anchorMax = new Vector2(0.5f, 0.5f);
            crt.sizeDelta = new Vector2(820f, 430f);
            card.GetComponent<Image>().color = new Color(0.10f, 0.14f, 0.22f, 0.96f);

            CreateText(card.transform, "Title", font,
                "<b>CHÚC MỪNG HOÀN THÀNH CHƯƠNG 2!</b>", 30f,
                new Color(1.0f, 0.90f, 0.50f, 1f),
                new Vector2(0f, -26f), new Vector2(760f, 44f), TextAlignmentOptions.Center);

            CreateText(card.transform, "Subtitle", font,
                "Ánh Đèn Bên Dòng Sông — Cây cầu hai bờ đã nối thông vĩnh viễn", 19f,
                new Color(0.72f, 0.94f, 0.98f, 1f),
                new Vector2(0f, -76f), new Vector2(760f, 30f), TextAlignmentOptions.Center);

            CreateText(card.transform, "Body", font,
                "- Đã nhận: <b>Cánh Hoa Gắn Kết</b> (Cánh hoa Moonpetal thứ 2)\n" +
                "- Mở khóa: <b>Xung Ánh Trăng Cấp 2 [Q]</b> (Bán kính 4.5m & làm Choáng quái 2 giây)\n" +
                "- Ông Kael và Bà Mara đã hóa giải hiểu lầm, cầu bờ sông sáng đèn vĩnh viễn\n" +
                "- Hé lộ chặng kế tiếp: <i>\"Tiếng khóc trong rừng lớn hơn trước...\"</i> — Sẵn sàng cho Chương 3: Chợ Đêm Đầu Tiên!",
                20f, Color.white,
                new Vector2(0f, -124f), new Vector2(740f, 190f), TextAlignmentOptions.TopLeft);

            GameObject btnGo = new GameObject("BtnContinue", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(card.transform, false);
            RectTransform brt = btnGo.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0f);
            brt.anchorMax = new Vector2(0.5f, 0f);
            brt.pivot = new Vector2(0.5f, 0f);
            brt.anchoredPosition = new Vector2(0f, 32f);
            brt.sizeDelta = new Vector2(340f, 52f);
            btnGo.GetComponent<Image>().color = new Color(0.20f, 0.58f, 0.44f, 1f);
            btnGo.GetComponent<Button>().onClick.AddListener(CloseCompletionModal);

            CreateText(btnGo.transform, "Label", font, "<b>Tiếp tục khám phá Thung Lũng</b>", 20f,
                Color.white, Vector2.zero, new Vector2(320f, 44f), TextAlignmentOptions.Center, stretch: true);

            _summaryPanelRoot.SetActive(false);
        }

        private static TMP_FontAsset ResolveVnFont()
        {
            if (JuiceEffectsManager.Instance != null && JuiceEffectsManager.Instance.VnFont != null)
                return JuiceEffectsManager.Instance.VnFont;
            TMP_Text existing = FindAnyObjectByType<TMP_Text>();
            if (existing != null && existing.font != null) return existing.font;
            return TMP_Settings.defaultFontAsset;
        }

        private static TMP_Text CreateText(
            Transform parent, string name, TMP_FontAsset font, string content, float size,
            Color color, Vector2 pos, Vector2 sizeDelta, TextAlignmentOptions align, bool stretch = false)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            if (stretch)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            else
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = pos;
                rt.sizeDelta = sizeDelta;
            }
            TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.text = content;
            tmp.alignment = align;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            return tmp;
        }

        private void RescuePlayerFromBridgeRailIfStuck()
        {
            PlayerController2D player = FindAnyObjectByType<PlayerController2D>();
            if (player == null) return;
            Vector3 p = player.transform.position;
            // BridgeRail_South spans x=[-8.6..8.6], top edge y=-0.20 (min safe Wen pos.y = 0.36)
            if (p.x >= -8.6f && p.x <= 8.6f && p.y > -1.05f && p.y < 0.38f)
            {
                player.TeleportTo(new Vector2(p.x, 0.52f));
            }
        }

        private static bool HasFlag(string flagId)
        {
            return GameRoot.Instance != null && GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag(flagId);
        }
    }
}
