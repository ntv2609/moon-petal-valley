using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
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

        private GameInputReader _input;
        private bool _isOpen;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnResume != null) btnResume.onClick.AddListener(Close);
            if (btnSave != null) btnSave.onClick.AddListener(OnClickSave);
            if (btnLoad != null) btnLoad.onClick.AddListener(OnClickLoad);
            if (btnMute != null) btnMute.onClick.AddListener(OnClickMute);
            if (btnDeleteSave != null) btnDeleteSave.onClick.AddListener(OnClickDeleteSave);
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
                if (!_isOpen && GameRoot.Instance.State.IsGameplayActive) Open();
                else if (_isOpen) Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance.State.SetState(GameState.PauseMenu);
            GameRoot.Instance.Audio?.PlayUIClick();
            RefreshLabels();
        }

        public void Close()
        {
            _isOpen = false;
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
        }
    }
}
