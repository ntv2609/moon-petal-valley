using UnityEngine;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.UI
{
    public class FloatingToastUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text toastText;
        [SerializeField] private float displayDuration = 2.6f;

        private float _timer;

        private void OnEnable()
        {
            GameEventBus.OnToastRequested += ShowToast;
        }

        private void OnDisable()
        {
            GameEventBus.OnToastRequested -= ShowToast;
        }

        private void Start()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        public void ShowToast(string message)
        {
            if (toastText != null)
            {
                toastText.text = message;
            }
            _timer = displayDuration;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private void Update()
        {
            if (_timer <= 0f) return;

            _timer -= Time.unscaledDeltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = _timer < 0.5f ? Mathf.Clamp01(_timer / 0.5f) : 1f;
            }
        }
    }
}
