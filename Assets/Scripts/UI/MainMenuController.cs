using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public static bool PendingLoadSaveOnWorldStart;

        [SerializeField] private Button btnNewGame;
        [SerializeField] private Button btnContinue;
        [SerializeField] private Button btnSettings;
        [SerializeField] private Button btnQuit;
        [SerializeField] private TMP_Text continueSubtext;
        [SerializeField] private MainMenuSettingsUI settingsUI;

        private void Awake()
        {
            if (btnNewGame != null) btnNewGame.onClick.AddListener(OnClickNewGame);
            if (btnContinue != null) btnContinue.onClick.AddListener(OnClickContinue);
            if (btnSettings != null) btnSettings.onClick.AddListener(OnClickSettings);
            if (btnQuit != null) btnQuit.onClick.AddListener(OnClickQuit);
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.PauseMenu);
            }

            GameSettingsManager.ApplyAudioAndFlags();

            bool hasSave = GameRoot.Instance != null && GameRoot.Instance.SaveLoad != null &&
                           GameRoot.Instance.SaveLoad.HasSaveFile();

            if (btnContinue != null)
            {
                btnContinue.interactable = hasSave;
            }
            if (continueSubtext != null)
            {
                continueSubtext.text = hasSave
                    ? "Bản lưu gần nhất: " + GameRoot.Instance.SaveLoad.LastSavedTimestamp
                    : "Chưa có bản lưu nào. Hãy chọn Chơi Mới.";
            }
        }

        private void OnClickNewGame()
        {
            GameRoot.Instance?.ResetForNewGame();
            PendingLoadSaveOnWorldStart = false;
            GameRoot.Instance?.Audio?.PlayUIClick();
            GameRoot.Instance?.State?.ReturnToGameplay();
            GameRoot.Instance?.Scenes?.LoadScene("MoonpetalValley_MVP");
        }

        private void OnClickContinue()
        {
            string targetScene = GameRoot.Instance != null && GameRoot.Instance.SaveLoad != null
                ? GameRoot.Instance.SaveLoad.GetSavedSceneName()
                : "MoonpetalValley_MVP";

            PendingLoadSaveOnWorldStart = true;
            GameRoot.Instance?.Audio?.PlayUIClick();
            GameRoot.Instance?.State?.ReturnToGameplay();
            GameRoot.Instance?.Scenes?.LoadScene(targetScene);
        }

        private void OnClickSettings()
        {
            GameRoot.Instance?.Audio?.PlayUIClick();
            if (settingsUI != null) settingsUI.Open();
        }

        private void OnClickQuit()
        {
            GameRoot.Instance?.Audio?.PlayUIClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
