using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MoonpetalValley.UI
{
    public class InteractionPromptUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text promptText;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool _isVisible;
        private float _fadeAlpha;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (panelRoot == null || !panelRoot.activeSelf || canvasGroup == null) return;
            float target = _isVisible ? 1f : 0f;
            _fadeAlpha = Mathf.MoveTowards(_fadeAlpha, target, Time.unscaledDeltaTime * 9f);
            canvasGroup.alpha = _fadeAlpha;
        }

        public void ShowPrompt(string label, int index, int totalCount)
        {
            if (panelRoot == null || promptText == null) return;

            string text = "[E] " + label;
            if (totalCount > 1)
            {
                text += "   |   [R / Cuộn] Đổi (" + (index + 1) + "/" + totalCount + ")";
            }

            promptText.text = text;
            _isVisible = true;
            if (!panelRoot.activeSelf)
            {
                panelRoot.SetActive(true);
                _fadeAlpha = 0.25f;
                if (canvasGroup != null) canvasGroup.alpha = _fadeAlpha;
            }
        }

        public void HidePrompt()
        {
            _isVisible = false;
            _fadeAlpha = 0f;
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            if (panelRoot != null && panelRoot.activeSelf) panelRoot.SetActive(false);
        }
    }
}
