using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;

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

        private bool _isOpen;
        private const string TestFlagId = "flag_debug_step1_verified";

        private void Awake()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }

            if (btnTestToast != null) btnTestToast.onClick.AddListener(OnClickTestToast);
            if (btnToggleTestFlag != null) btnToggleTestFlag.onClick.AddListener(OnClickToggleTestFlag);
            if (btnClearFlags != null) btnClearFlags.onClick.AddListener(OnClickClearFlags);
            if (btnTogglePause != null) btnTogglePause.onClick.AddListener(OnClickTogglePause);
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

            if (WasF1PressedThisFrame())
            {
                TogglePanel();
            }
        }

        private bool WasF1PressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame)
            {
                return true;
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F1))
            {
                return true;
            }
#endif
            return false;
        }

        public void TogglePanel()
        {
            _isOpen = !_isOpen;
            if (panelRoot != null)
            {
                panelRoot.SetActive(_isOpen);
            }
            if (_isOpen)
            {
                RefreshStatusText();
            }
        }

        private void OnClickTestToast()
        {
            string msg = GameRoot.Instance != null && GameRoot.Instance.Loc != null
                ? GameRoot.Instance.Loc.Get("toast_step1_test")
                : "Kiểm thử thông báo tiếng Việt có dấu thành công!";
            GameEventBus.RaiseToastRequested(msg);
        }

        private void OnClickToggleTestFlag()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            bool next = !GameRoot.Instance.Flags.HasFlag(TestFlagId);
            GameRoot.Instance.Flags.SetFlag(TestFlagId, next);
            GameEventBus.RaiseToastRequested("Flag thử nghiệm: " + (next ? "BẬT" : "TẮT"));
            RefreshStatusText();
        }

        private void OnClickClearFlags()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            GameRoot.Instance.Flags.ClearAllFlags();
            GameEventBus.RaiseToastRequested("Đã xóa toàn bộ StoryFlag.");
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
            sb.AppendLine("Trạng thái: " + GameRoot.Instance.State.CurrentState);
            sb.AppendLine("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            sb.Append("StoryFlags đang bật: ");
            var flags = GameRoot.Instance.Flags.GetAllActiveFlags();
            if (flags.Count == 0)
            {
                sb.Append("(chưa có)");
            }
            else
            {
                bool first = true;
                foreach (var f in flags)
                {
                    if (!first) sb.Append(", ");
                    sb.Append(f);
                    first = false;
                }
            }
            statusText.text = sb.ToString();
        }
    }
}
