using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MoonpetalValley.UI
{
    public class MainMenuSettingsUI : MonoBehaviour
    {
        [SerializeField] private GameObject modalRoot;
        [SerializeField] private Slider sliderBgm;
        [SerializeField] private TMP_Text bgmValueText;
        [SerializeField] private Slider sliderSfx;
        [SerializeField] private TMP_Text sfxValueText;
        [SerializeField] private Button btnMute;
        [SerializeField] private TMP_Text muteValueText;
        [SerializeField] private Button btnFullscreen;
        [SerializeField] private TMP_Text fullscreenValueText;
        [SerializeField] private Button btnResolution;
        [SerializeField] private TMP_Text resolutionValueText;
        [SerializeField] private Button btnTutorial;
        [SerializeField] private TMP_Text tutorialValueText;
        [SerializeField] private TMP_Text savedFeedbackText;
        [SerializeField] private Button btnSave;
        [SerializeField] private Button btnClose;

        private bool _isOpen;
        private bool _suppressCallbacks;
        private Coroutine _feedbackRoutine;

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            if (modalRoot != null) modalRoot.SetActive(false);
            if (savedFeedbackText != null) savedFeedbackText.text = "";

            if (sliderBgm != null) sliderBgm.onValueChanged.AddListener(OnBgmChanged);
            if (sliderSfx != null) sliderSfx.onValueChanged.AddListener(OnSfxChanged);
            if (btnMute != null) btnMute.onClick.AddListener(OnToggleMute);
            if (btnFullscreen != null) btnFullscreen.onClick.AddListener(OnToggleFullscreen);
            if (btnResolution != null) btnResolution.onClick.AddListener(OnCycleResolution);
            if (btnTutorial != null) btnTutorial.onClick.AddListener(OnToggleTutorial);
            if (btnSave != null) btnSave.onClick.AddListener(OnClickSave);
            if (btnClose != null) btnClose.onClick.AddListener(Close);
        }

        private void Update()
        {
            if (!_isOpen) return;
            if (IsEscapePressed())
            {
                Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            if (modalRoot != null)
            {
                modalRoot.transform.SetAsLastSibling();
                modalRoot.SetActive(true);
            }
            if (savedFeedbackText != null) savedFeedbackText.text = "";
            RefreshUI();
        }

        public void OnClickSave()
        {
            GameSettingsManager.ApplyAllSettings(true);
            GameSettingsManager.Save();
            GameRoot.Instance?.Audio?.PlayUIClick();

            if (savedFeedbackText != null)
            {
                if (_feedbackRoutine != null) StopCoroutine(_feedbackRoutine);
                _feedbackRoutine = StartCoroutine(ShowSavedFeedback());
            }
        }

        public void Close()
        {
            _isOpen = false;
            GameRoot.Instance?.Audio?.PlayUIClick();
            if (modalRoot != null) modalRoot.SetActive(false);
        }

        private IEnumerator ShowSavedFeedback()
        {
            savedFeedbackText.text = "Đã lưu cài đặt!";
            yield return new WaitForSecondsRealtime(1.8f);
            if (savedFeedbackText != null) savedFeedbackText.text = "";
            _feedbackRoutine = null;
        }

        private void RefreshUI()
        {
            _suppressCallbacks = true;

            float bgm = GameSettingsManager.BgmVolume;
            float sfx = GameSettingsManager.SfxVolume;

            if (sliderBgm != null) sliderBgm.value = bgm;
            if (bgmValueText != null) bgmValueText.text = Mathf.RoundToInt(bgm * 100f) + "%";

            if (sliderSfx != null) sliderSfx.value = sfx;
            if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(sfx * 100f) + "%";

            if (muteValueText != null)
            {
                muteValueText.text = GameSettingsManager.IsMuted ? "Đang tắt tiếng" : "Đang bật";
            }

            if (fullscreenValueText != null)
            {
                fullscreenValueText.text = GameSettingsManager.IsFullscreen ? "Toàn màn hình" : "Cửa sổ";
            }

            if (resolutionValueText != null)
            {
                Vector2Int r = GameSettingsManager.SupportedResolutions[GameSettingsManager.ResolutionIndex];
                resolutionValueText.text = r.x + " x " + r.y;
            }

            if (tutorialValueText != null)
            {
                tutorialValueText.text = GameSettingsManager.TutorialHintsEnabled ? "Đang bật" : "Đã tắt";
            }

            _suppressCallbacks = false;
        }

        private void OnBgmChanged(float value)
        {
            if (_suppressCallbacks) return;
            GameSettingsManager.BgmVolume = value;
            if (bgmValueText != null) bgmValueText.text = Mathf.RoundToInt(value * 100f) + "%";
        }

        private void OnSfxChanged(float value)
        {
            if (_suppressCallbacks) return;
            GameSettingsManager.SfxVolume = value;
            if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(value * 100f) + "%";
        }

        private void OnToggleMute()
        {
            GameSettingsManager.IsMuted = !GameSettingsManager.IsMuted;
            if (!GameSettingsManager.IsMuted) GameRoot.Instance?.Audio?.PlayUIClick();
            RefreshUI();
        }

        private void OnToggleFullscreen()
        {
            GameSettingsManager.IsFullscreen = !GameSettingsManager.IsFullscreen;
            GameRoot.Instance?.Audio?.PlayUIClick();
            RefreshUI();
        }

        private void OnCycleResolution()
        {
            GameSettingsManager.ResolutionIndex = GameSettingsManager.ResolutionIndex + 1;
            GameRoot.Instance?.Audio?.PlayUIClick();
            RefreshUI();
        }

        private void OnToggleTutorial()
        {
            GameSettingsManager.TutorialHintsEnabled = !GameSettingsManager.TutorialHintsEnabled;
            GameRoot.Instance?.Audio?.PlayUIClick();
            RefreshUI();
        }

        private bool IsEscapePressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Escape)) return true;
#endif
            return false;
        }
    }
}
