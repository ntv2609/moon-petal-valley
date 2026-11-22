using UnityEngine;
using TMPro;

namespace MoonpetalValley.UI
{
    public class DialogueUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text continueHintText;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void ShowLine(string speaker, string body, bool isLastLine)
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            if (speakerText != null) speakerText.text = speaker;
            if (bodyText != null) bodyText.text = body;
            if (continueHintText != null)
            {
                continueHintText.text = isLastLine ? "[E / Space] Kết thúc" : "[E / Space] Tiếp tục >>";
            }
        }

        public void Hide()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }
    }
}
