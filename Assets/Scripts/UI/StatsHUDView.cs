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

        private void Awake()
        {
            if (exhaustedBadge != null)
            {
                var badgeTmp = exhaustedBadge.GetComponent<TMP_Text>();
                if (badgeTmp != null)
                {
                    badgeTmp.text = "[!] KIỆT SỨC: Trần Thể lực giảm còn 50% (Ăn Bánh Mì để xóa)";
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

        private void HandleLightHPChanged(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            if (lightHPFill != null) lightHPFill.fillAmount = ratio;
            if (lightHPText != null) lightHPText.text = string.Format("Máu Ánh Sáng: {0}/{1}", current, max);
        }

        private void HandleStaminaChanged(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            if (staminaFill != null) staminaFill.fillAmount = ratio;
            if (staminaText != null) staminaText.text = string.Format("Thể lực: {0}/{1}", current, max);
        }

        private void HandleExhaustionChanged(bool isExhausted)
        {
            if (exhaustedBadge != null)
            {
                exhaustedBadge.SetActive(isExhausted);
            }
            if (staminaFill != null)
            {
                staminaFill.color = isExhausted
                    ? new Color(0.92f, 0.44f, 0.24f, 1f)
                    : new Color(0.95f, 0.78f, 0.24f, 1f);
            }
        }
    }
}
