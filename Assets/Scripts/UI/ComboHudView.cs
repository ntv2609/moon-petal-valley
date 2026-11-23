using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class ComboHudView : MonoBehaviour
    {
        [SerializeField] private float displayDuration = 1.45f;

        private Canvas _canvas;
        private CanvasGroup _group;
        private RectTransform _badgeRect;
        private Image _bgImage;
        private TextMeshProUGUI _label;
        private float _visibleTimer;
        private float _punchTimer;
        private bool _fontBound;

        private void Awake()
        {
            BuildRuntimeHud();
        }

        private void OnEnable()
        {
            PlayerCombatController.OnPlayerComboSwing += HandleComboSwing;
        }

        private void OnDisable()
        {
            PlayerCombatController.OnPlayerComboSwing -= HandleComboSwing;
        }

        private void OnDestroy()
        {
            if (_canvas != null)
            {
                Destroy(_canvas.gameObject);
            }
        }

        private void BuildRuntimeHud()
        {
            GameObject canvasGo = new GameObject("ComboHudCanvas_Runtime");
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 45;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject badgeGo = new GameObject("ComboBadge");
            badgeGo.transform.SetParent(canvasGo.transform, false);
            _badgeRect = badgeGo.AddComponent<RectTransform>();
            _badgeRect.anchorMin = new Vector2(1f, 0.5f);
            _badgeRect.anchorMax = new Vector2(1f, 0.5f);
            _badgeRect.pivot = new Vector2(1f, 0.5f);
            _badgeRect.anchoredPosition = new Vector2(-42f, -90f);
            _badgeRect.sizeDelta = new Vector2(310f, 54f);

            _group = badgeGo.AddComponent<CanvasGroup>();
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _group.interactable = false;

            _bgImage = badgeGo.AddComponent<Image>();
            _bgImage.color = new Color(0.08f, 0.13f, 0.20f, 0.78f);
            _bgImage.raycastTarget = false;

            GameObject textGo = new GameObject("ComboText");
            textGo.transform.SetParent(badgeGo.transform, false);
            RectTransform textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(14f, 4f);
            textRect.offsetMax = new Vector2(-14f, -4f);

            _label = textGo.AddComponent<TextMeshProUGUI>();
            _label.alignment = TextAlignmentOptions.Center;
            _label.fontSize = 24f;
            _label.fontStyle = FontStyles.Bold;
            _label.raycastTarget = false;
            _label.text = "";

            TryBindVietnameseFont();
        }

        private void TryBindVietnameseFont()
        {
            if (_fontBound || _label == null) return;
            if (JuiceEffectsManager.Instance != null && JuiceEffectsManager.Instance.VnFont != null)
            {
                _label.font = JuiceEffectsManager.Instance.VnFont;
                _fontBound = true;
            }
        }

        private void HandleComboSwing(int comboStep, bool isDashStrike, Vector2 _, Vector2 __)
        {
            if (_label == null || _group == null) return;
            TryBindVietnameseFont();

            if (isDashStrike)
            {
                ShowBadge("LƯỚT CHÉM!", new Color(0.72f, 0.98f, 1f, 1f), new Color(0.07f, 0.16f, 0.24f, 0.82f), 1.15f);
            }
            else if (comboStep == 2)
            {
                ShowBadge("2 HIT — NGUYỆT NGƯỢC", new Color(0.58f, 1f, 0.88f, 1f), new Color(0.07f, 0.18f, 0.20f, 0.82f), 1.15f);
            }
            else if (comboStep == 3)
            {
                ShowBadge("3 HIT — HOA TRĂNG!", new Color(1f, 0.91f, 0.48f, 1f), new Color(0.22f, 0.14f, 0.18f, 0.88f), 1.30f);
            }
        }

        private void ShowBadge(string text, Color textColor, Color bgColor, float punchScale)
        {
            _label.text = text;
            _label.color = textColor;
            if (_bgImage != null) _bgImage.color = bgColor;

            _visibleTimer = displayDuration;
            _punchTimer = 0.16f;
            if (_badgeRect != null) _badgeRect.localScale = new Vector3(punchScale, punchScale, 1f);
            _group.alpha = 1f;
        }

        private void Update()
        {
            if (_group == null || _badgeRect == null) return;

            if (_punchTimer > 0f)
            {
                _punchTimer -= Time.unscaledDeltaTime;
                float t = 1f - Mathf.Clamp01(_punchTimer / 0.16f);
                float sc = Mathf.Lerp(_badgeRect.localScale.x, 1f, t * 0.45f + 0.15f);
                _badgeRect.localScale = new Vector3(sc, sc, 1f);
            }

            if (_visibleTimer > 0f)
            {
                _visibleTimer -= Time.deltaTime;
                if (_visibleTimer < 0.35f)
                {
                    _group.alpha = Mathf.Clamp01(_visibleTimer / 0.35f);
                }
                else
                {
                    _group.alpha = 1f;
                }
            }
            else if (_group.alpha > 0f)
            {
                _group.alpha = 0f;
            }
        }
    }
}
