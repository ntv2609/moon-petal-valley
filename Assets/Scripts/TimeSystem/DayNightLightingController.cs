using UnityEngine;
using UnityEngine.UI;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.TimeSystem
{
    public class DayNightLightingController : MonoBehaviour
    {
        [SerializeField] private TimeConfigSO timeConfig;
        [SerializeField] private Image ambientTintImage;

        private Color _targetTint = new Color(1f, 0.94f, 0.78f, 0.06f);
        private float _targetLumiScale = 1.0f;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
            {
                HandleTimeChanged(GameRoot.Instance.Time.CurrentDay, GameRoot.Instance.Time.CurrentHourFloat);
            }
        }

        private void Update()
        {
            if (ambientTintImage != null)
            {
                ambientTintImage.color = Color.Lerp(ambientTintImage.color, _targetTint, Time.unscaledDeltaTime * 3.0f);
            }
        }

        private void HandleTimeChanged(int day, float hour)
        {
            float dusk = timeConfig != null ? timeConfig.duskStartHour : 18f;
            float night = timeConfig != null ? timeConfig.nightStartHour : 20f;

            Color morningTint = new Color(1.0f, 0.92f, 0.72f, 0.08f);
            Color noonTint = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            Color duskTint = new Color(0.88f, 0.42f, 0.16f, 0.24f);
            Color nightTint = new Color(0.06f, 0.09f, 0.24f, 0.50f);

            if (hour < 11f)
            {
                float t = Mathf.InverseLerp(6f, 11f, hour);
                _targetTint = Color.Lerp(morningTint, noonTint, t);
            }
            else if (hour < dusk)
            {
                float t = Mathf.InverseLerp(16f, dusk, hour);
                _targetTint = Color.Lerp(noonTint, duskTint, t);
            }
            else if (hour < night)
            {
                float t = Mathf.InverseLerp(dusk, night, hour);
                _targetTint = Color.Lerp(duskTint, nightTint, t);
            }
            else
            {
                _targetTint = nightTint;
            }

            bool upgraded = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                            GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            float nightScale = upgraded ? 3.2f : 2.0f;
            _targetLumiScale = hour >= dusk ? nightScale : 1.0f;

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null)
            {
                lumi.SetHaloRadiusScale(_targetLumiScale);
            }
        }
    }
}
