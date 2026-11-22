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
        [SerializeField] private Button btnQuit;
        [SerializeField] private TMP_Text continueSubtext;

        private void Awake()
        {
            if (btnNewGame != null) btnNewGame.onClick.AddListener(OnClickNewGame);
            if (btnContinue != null) btnContinue.onClick.AddListener(OnClickContinue);
            if (btnQuit != null) btnQuit.onClick.AddListener(OnClickQuit);
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.PauseMenu);
            }

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
                    : "Chưa có bản lưu nào — Hãy chọn Chơi Mới";
            }
        }

        private void OnClickNewGame()
        {
            PendingLoadSaveOnWorldStart = false;
            GameRoot.Instance?.Audio?.PlayUIClick();
            GameRoot.Instance?.State?.ReturnToGameplay();
            GameRoot.Instance?.Scenes?.LoadScene("MoonpetalValley_MVP");
        }

        private void OnClickContinue()
        {
            PendingLoadSaveOnWorldStart = true;
            GameRoot.Instance?.Audio?.PlayUIClick();
            GameRoot.Instance?.State?.ReturnToGameplay();
            GameRoot.Instance?.Scenes?.LoadScene("MoonpetalValley_MVP");
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
