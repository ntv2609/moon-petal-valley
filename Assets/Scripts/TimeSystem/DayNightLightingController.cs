using UnityEngine;
using UnityEngine.SceneManagement;
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

        private Color _targetTint = new Color(1f, 0.94f, 0.76f, 0.09f);
        private float _targetLumiScale = 1.0f;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start()
        {
            RefreshNow();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshNow();
        }

        private void RefreshNow()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
                HandleTimeChanged(GameRoot.Instance.Time.CurrentDay, GameRoot.Instance.Time.CurrentHourFloat);
            else
                HandleTimeChanged(1, 12f);
        }

        private void Update()
        {
            if (ambientTintImage != null)
            {
                ambientTintImage.color = Color.Lerp(ambientTintImage.color, _targetTint, Time.unscaledDeltaTime * 4.0f);
            }
        }

        private void HandleTimeChanged(int day, float hour)
        {
            string activeScene = SceneManager.GetActiveScene().name;
            bool inWestSwamp = activeScene == "WestSwamp_Chapter2";

            float dusk = timeConfig != null ? timeConfig.duskStartHour : 18f;
            float night = timeConfig != null ? timeConfig.nightStartHour : 20f;

            if (inWestSwamp)
            {
                // Tach biet anh sang Dam Lay khoi lop phu ngay/dem ngoai troi -> khong bi toi mu luc chang vang/dem
                // Giao toan quyen dieu tiet khong khi am thap cho SwampAtmosphereController
                float nightBlend = hour >= dusk ? Mathf.InverseLerp(dusk, night, hour) : 0f;
                _targetTint = Color.Lerp(
                    new Color(0.06f, 0.14f, 0.16f, 0.02f),
                    new Color(0.04f, 0.10f, 0.16f, 0.06f),
                    nightBlend);

                var lumiSwamp = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
                if (lumiSwamp != null) lumiSwamp.SetHaloRadiusScale(2.1f);
                return;
            }

            // Cozy Stardew-inspired sunlit warmth & clearer luminous moonlit night
            Color morningTint = new Color(1.0f, 0.94f, 0.76f, 0.09f);
            Color noonTint    = new Color(1.0f, 0.98f, 0.90f, 0.03f);
            Color duskTint    = new Color(0.92f, 0.48f, 0.22f, 0.22f);
            Color nightTint   = new Color(0.08f, 0.12f, 0.28f, 0.44f);

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
