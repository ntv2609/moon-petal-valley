using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class StatsHUDView : MonoBehaviour
    {
        [SerializeField] private Image lightHPFill;
        [SerializeField] private TMP_Text lightHPText;
        [SerializeField] private Image staminaFill;
        [SerializeField] private TMP_Text staminaText;
        [SerializeField] private GameObject exhaustedBadge;

        private const float LowThreshold = 0.25f;

        private float _hpRatio = 1f;
        private float _staminaRatio = 1f;
        private bool _isExhausted;

        private readonly Color _hpBaseColor = Color.white;
        private readonly Color _hpDangerColor = new Color(1.0f, 0.36f, 0.36f, 1f);

        private readonly Color _staminaNormalColor = Color.white;
        private readonly Color _staminaExhaustedBase = new Color(0.95f, 0.56f, 0.28f, 1f);
        private readonly Color _staminaExhaustedSoft = new Color(1.0f, 0.78f, 0.44f, 1f);
        private readonly Color _staminaDangerColor = new Color(1.0f, 0.32f, 0.24f, 1f);

        private void Awake()
        {
            if (exhaustedBadge != null)
            {
                var badgeTmp = exhaustedBadge.GetComponentInChildren<TMP_Text>(true);
                if (badgeTmp != null)
                {
                    badgeTmp.text = "KIỆT SỨC: Trần Thể lực còn 50%\nĂn Bánh Mì Nắng Ấm để hồi phục";
                }
            }
        }

        private void OnEnable()
        {
            GameEventBus.OnLightHPChanged += HandleLightHPChanged;
            GameEventBus.OnStaminaChanged += HandleStaminaChanged;
            PlayerStatsController.OnExhaustionStateChanged += HandleExhaustionChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnLightHPChanged -= HandleLightHPChanged;
            GameEventBus.OnStaminaChanged -= HandleStaminaChanged;
            PlayerStatsController.OnExhaustionStateChanged -= HandleExhaustionChanged;
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                var st = GameRoot.Instance.Stats;
                HandleLightHPChanged(st.CurrentLightHP, st.MaxLightHP);
                HandleStaminaChanged(st.CurrentStamina, st.EffectiveMaxStamina);
                HandleExhaustionChanged(st.IsExhaustedPenalty);
            }
        }

        private void Update()
        {
            float t = Time.unscaledTime;

            if (lightHPFill != null)
            {
                if (_hpRatio <= LowThreshold)
                {
                    float pulse = 0.5f + 0.5f * Mathf.Sin(t * 7.8f);
                    lightHPFill.color = Color.Lerp(_hpBaseColor, _hpDangerColor, pulse);
                }
                else
                {
                    lightHPFill.color = _hpBaseColor;
                }
            }

            if (staminaFill != null)
            {
                if (_staminaRatio <= LowThreshold)
                {
                    float pulse = 0.5f + 0.5f * Mathf.Sin(t * 7.8f);
                    Color baseCol = _isExhausted ? _staminaExhaustedBase : _staminaNormalColor;
                    staminaFill.color = Color.Lerp(baseCol, _staminaDangerColor, pulse);
                }
                else if (_isExhausted)
                {
                    float softPulse = 0.5f + 0.5f * Mathf.Sin(t * 2.8f);
                    staminaFill.color = Color.Lerp(_staminaExhaustedBase, _staminaExhaustedSoft, softPulse);
                }
                else
                {
                    staminaFill.color = _staminaNormalColor;
                }
            }
        }

        private void HandleLightHPChanged(int current, int max)
        {
            _hpRatio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            if (lightHPFill != null) lightHPFill.fillAmount = _hpRatio;
            if (lightHPText != null) lightHPText.text = string.Format("Máu Ánh Sáng: {0}/{1}", current, max);
        }

        private void HandleStaminaChanged(int current, int max)
        {
            _staminaRatio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            if (staminaFill != null) staminaFill.fillAmount = _staminaRatio;
            if (staminaText != null) staminaText.text = string.Format("Thể lực: {0}/{1}", current, max);
        }

        private void HandleExhaustionChanged(bool isExhausted)
        {
            _isExhausted = isExhausted;
            if (exhaustedBadge != null)
            {
                exhaustedBadge.SetActive(isExhausted);
            }
        }
    }
}
