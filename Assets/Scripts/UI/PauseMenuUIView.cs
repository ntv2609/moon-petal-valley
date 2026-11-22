using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class PauseMenuUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text saveStatusText;
        [SerializeField] private TMP_Text muteButtonText;
        [SerializeField] private Button btnResume;
        [SerializeField] private Button btnSave;
        [SerializeField] private Button btnLoad;
        [SerializeField] private Button btnMute;
        [SerializeField] private Button btnDeleteSave;

        [Header("Step 34 - Tutorial & Tips")]
        [SerializeField] private Button btnToggleTutorial;
        [SerializeField] private TMP_Text tutorialButtonText;
        [SerializeField] private Button btnOpenTips;
        [SerializeField] private GameObject tipsModalRoot;
        [SerializeField] private Button btnCloseTips;

        [Header("Step 36 - Main Menu & Quit")]
        [SerializeField] private Button btnMainMenu;
        [SerializeField] private Button btnQuitGame;

        private GameInputReader _input;
        private bool _isOpen;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (tipsModalRoot != null) tipsModalRoot.SetActive(false);

            if (btnResume != null) btnResume.onClick.AddListener(Close);
            if (btnSave != null) btnSave.onClick.AddListener(OnClickSave);
            if (btnLoad != null) btnLoad.onClick.AddListener(OnClickLoad);
            if (btnMute != null) btnMute.onClick.AddListener(OnClickMute);
            if (btnDeleteSave != null) btnDeleteSave.onClick.AddListener(OnClickDeleteSave);
            if (btnToggleTutorial != null) btnToggleTutorial.onClick.AddListener(OnClickToggleTutorial);
            if (btnOpenTips != null) btnOpenTips.onClick.AddListener(OnClickOpenTips);
            if (btnCloseTips != null) btnCloseTips.onClick.AddListener(OnClickCloseTips);
            if (btnMainMenu != null) btnMainMenu.onClick.AddListener(OnClickMainMenu);
            if (btnQuitGame != null) btnQuitGame.onClick.AddListener(OnClickQuitGame);
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input == null) return;

            if (_input.PausePressed)
            {
                if (_isOpen && tipsModalRoot != null && tipsModalRoot.activeSelf)
                {
                    OnClickCloseTips();
                    return;
                }
                if (!_isOpen && GameRoot.Instance.State.IsGameplayActive) Open();
                else if (_isOpen) Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            if (tipsModalRoot != null) tipsModalRoot.SetActive(false);
            GameRoot.Instance.State.SetState(GameState.PauseMenu);
            GameRoot.Instance.Audio?.PlayUIClick();
            RefreshLabels();
        }

        public void Close()
        {
            _isOpen = false;
            if (tipsModalRoot != null) tipsModalRoot.SetActive(false);
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance.State.CurrentState == GameState.PauseMenu)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void OnClickSave()
        {
            GameRoot.Instance?.SaveLoad?.SaveGame(false);
            RefreshLabels();
        }

        private void OnClickLoad()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.SaveLoad != null && GameRoot.Instance.SaveLoad.LoadGame())
            {
                Close();
            }
        }

        private void OnClickMute()
        {
            GameRoot.Instance?.Audio?.ToggleMute();
            RefreshLabels();
        }

        private void OnClickDeleteSave()
        {
            GameRoot.Instance?.SaveLoad?.DeleteSaveFile();
            RefreshLabels();
        }

        private void OnClickToggleTutorial()
        {
            GameRoot.Instance?.Audio?.PlayUIClick();
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.ToggleTutorialEnabled();
            }
            else if (GameRoot.Instance?.Flags != null)
            {
                bool currentlySkipped = GameRoot.Instance.Flags.HasFlag(TutorialManager.FLAG_SKIPPED);
                GameRoot.Instance.Flags.SetFlag(TutorialManager.FLAG_SKIPPED, !currentlySkipped);
            }
            RefreshLabels();
        }

        private void OnClickOpenTips()
        {
            GameRoot.Instance?.Audio?.PlayUIClick();
            if (tipsModalRoot != null) tipsModalRoot.SetActive(true);
        }

        private void OnClickCloseTips()
        {
            GameRoot.Instance?.Audio?.PlayUIClick();
            if (tipsModalRoot != null) tipsModalRoot.SetActive(false);
        }

        private void OnClickMainMenu()
        {
            GameRoot.Instance?.Audio?.PlayUIClick();
            _isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            GameRoot.Instance?.State?.ReturnToGameplay();
            if (GameRoot.Instance?.Scenes != null)
            {
                GameRoot.Instance.Scenes.LoadScene("MainMenu");
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        private void OnClickQuitGame()
        {
            GameRoot.Instance?.Audio?.PlayUIClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void RefreshLabels()
        {
            if (saveStatusText != null && GameRoot.Instance?.SaveLoad != null)
            {
                var sl = GameRoot.Instance.SaveLoad;
                saveStatusText.text = sl.HasSaveFile()
                    ? "Bản lưu gần nhất: " + sl.LastSavedTimestamp
                    : "Chưa có bản lưu game (Tự động lưu mỗi khi đi ngủ)";
            }
            if (muteButtonText != null && GameRoot.Instance?.Audio != null)
            {
                muteButtonText.text = GameRoot.Instance.Audio.IsMuted ? "Âm Thanh: ĐANG TẮT" : "Âm Thanh: ĐANG BẬT";
            }
            if (tutorialButtonText != null)
            {
                bool enabled = TutorialManager.Instance != null
                    ? TutorialManager.Instance.IsTutorialEnabled
                    : (GameRoot.Instance?.Flags == null || !GameRoot.Instance.Flags.HasFlag(TutorialManager.FLAG_SKIPPED));
                tutorialButtonText.text = enabled ? "Gợi Ý Hướng Dẫn: ĐANG BẬT" : "Gợi Ý Hướng Dẫn: ĐÃ TẮT";
            }
        }
    }
}
