using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MoonpetalValley.UI
{
    public class TutorialHintHUDView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup rootGroup;
        [SerializeField] private TMP_Text hintText;

        private bool _targetVisible;
        private bool _isShowingCompletion;

        private readonly Color _inkNormal = new Color(0.23f, 0.15f, 0.09f, 1f);
        private readonly Color _inkDone = new Color(0.15f, 0.37f, 0.23f, 1f);

        public bool IsShowingCompletion => _isShowingCompletion;

        private void Awake()
        {
            if (rootGroup != null)
            {
                rootGroup.alpha = 0f;
                rootGroup.blocksRaycasts = false;
                rootGroup.interactable = false;
            }
        }

        private void Update()
        {
            if (rootGroup == null) return;
            float target = _targetVisible ? 1f : 0f;
            if (!Mathf.Approximately(rootGroup.alpha, target))
            {
                rootGroup.alpha = Mathf.MoveTowards(rootGroup.alpha, target, Time.unscaledDeltaTime * 5.0f);
            }
        }

        public void SetHint(string hint)
        {
            _isShowingCompletion = false;
            if (hintText != null)
            {
                hintText.color = _inkNormal;
                hintText.text = hint;
            }
        }

        public void ShowCompletedFlash(string message)
        {
            _isShowingCompletion = true;
            _targetVisible = true;
            if (hintText != null)
            {
                hintText.color = _inkDone;
                hintText.text = "<b>" + message + "</b>";
            }
        }

        public void SetVisible(bool visible)
        {
            _targetVisible = visible;
            if (!visible) _isShowingCompletion = false;
        }
    }
}
