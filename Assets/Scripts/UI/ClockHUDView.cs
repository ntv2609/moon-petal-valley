using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.UI
{
    public class ClockHUDView : MonoBehaviour
    {
        [SerializeField] private TMP_Text clockText;
        [SerializeField] private Image phaseBadgeImage;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
            GameEventBus.OnDayStarted += HandleDayStarted;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
            GameEventBus.OnDayStarted -= HandleDayStarted;
        }

        private void Start()
        {
            RefreshDisplay();
        }

        private void HandleTimeChanged(int day, float hour) => RefreshDisplay();
        private void HandleDayStarted(int day) => RefreshDisplay();

        public void RefreshDisplay()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Time == null) return;
            var tm = GameRoot.Instance.Time;

            if (clockText != null)
            {
                clockText.text = tm.GetFormattedClockString();
            }

            if (phaseBadgeImage != null)
            {
                phaseBadgeImage.color = tm.CurrentPhase switch
                {
                    DayPhase.Morning => new Color(0.95f, 0.80f, 0.35f, 1f),
                    DayPhase.Noon => new Color(0.98f, 0.90f, 0.42f, 1f),
                    DayPhase.Dusk => new Color(0.92f, 0.48f, 0.24f, 1f),
                    _ => new Color(0.36f, 0.50f, 0.88f, 1f)
                };
            }
        }
    }
}
