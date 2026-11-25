using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI {
    public class MoonlightFeverHUDView : MonoBehaviour {
        [SerializeField] private TMP_FontAsset vnFont;

        private Canvas _canvas;
        private GameObject _root;
        private Outline _outline;
        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _subText;
        private Image _mainFill;
        private Image _decayFill;

        private void Start() {
            EnsureUI();
            if (_root != null) _root.SetActive(false);
        }

        private void Update() {
            var ctrl = MoonlightFeverController.Instance;
            bool gameplay = GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsGameplayActive;
            bool shouldShow = gameplay && ctrl != null && (ctrl.CurrentStreak > 0 || ctrl.IsFeverActive);

            if (_root == null) EnsureUI();
            if (_root == null) return;

            if (_root.activeSelf != shouldShow) _root.SetActive(shouldShow);
            if (!shouldShow) return;

            if (ctrl.IsFeverActive) {
                float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 10f);
                if (_outline != null)
                    _outline.effectColor = Color.Lerp(new Color(0.45f, 0.96f, 1f, 0.95f), new Color(1f, 0.86f, 0.38f, 1f), pulse);
                _titleText.text = "CƠN SỐT ÁNH TRĂNG!";
                _titleText.color = Color.Lerp(new Color(0.62f, 0.98f, 1f), new Color(1f, 0.92f, 0.56f), pulse);
                _subText.text = $"Sóng Trăng Xuyên Thấu | {ctrl.FeverRemaining:0.0}s";
                _subText.color = new Color(0.92f, 0.98f, 1f);
                _mainFill.fillAmount = Mathf.Clamp01(ctrl.FeverRemaining / Mathf.Max(0.1f, ctrl.FeverDuration));
                _mainFill.color = Color.Lerp(new Color(0.32f, 0.88f, 1f), new Color(1f, 0.86f, 0.42f), pulse);
                _decayFill.fillAmount = 1f;
                _decayFill.color = new Color(0.55f, 0.96f, 1f, 0.85f);
            } else {
                if (_outline != null)
                    _outline.effectColor = new Color(0.58f, 0.40f, 0.92f, 0.92f);
                _titleText.text = $"CHUỖI THANH TẨY: {ctrl.CurrentStreak}/{ctrl.FeverThreshold}";
                _titleText.color = new Color(0.95f, 0.88f, 1f);
                int left = Mathf.Max(0, ctrl.FeverThreshold - ctrl.CurrentStreak);
                _subText.text = left <= 3 ? $"Còn {left} đòn nữa để bùng nổ!" : "Đánh liên tiếp không để đứt chuỗi";
                _subText.color = new Color(0.78f, 0.84f, 0.96f);
                _mainFill.fillAmount = Mathf.Clamp01((float)ctrl.CurrentStreak / ctrl.FeverThreshold);
                _mainFill.color = new Color(0.62f, 0.42f, 0.96f, 1f);
                _decayFill.fillAmount = ctrl.StreakDecayRatio;
                _decayFill.color = new Color(0.96f, 0.78f, 0.36f, 0.9f);
            }
        }

        private void EnsureUI() {
            if (_root != null) return;

            GameObject cGo = new GameObject("MoonlightFeverHUDCanvas");
            cGo.transform.SetParent(transform, false);
            _canvas = cGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 44;
            var scaler = cGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            _root = new GameObject("MoonlightFeverPanel", typeof(RectTransform), typeof(Image));
            _root.transform.SetParent(cGo.transform, false);
            var rt = _root.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-28f, 24f);
            rt.sizeDelta = new Vector2(286f, 88f);
            _root.GetComponent<Image>().color = new Color(0.10f, 0.09f, 0.16f, 0.90f);

            _outline = _root.AddComponent<Outline>();
            _outline.effectColor = new Color(0.58f, 0.40f, 0.92f, 0.92f);
            _outline.effectDistance = new Vector2(2f, -2f);

            TMP_FontAsset fnt = vnFont != null ? vnFont : TMP_Settings.defaultFontAsset;

            GameObject tGo = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            tGo.transform.SetParent(_root.transform, false);
            var trt = tGo.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0f, 0.56f); trt.anchorMax = new Vector2(1f, 0.94f);
            trt.offsetMin = new Vector2(12f, 0f); trt.offsetMax = new Vector2(-12f, 0f);
            _titleText = tGo.GetComponent<TextMeshProUGUI>();
            if (fnt != null) _titleText.font = fnt;
            _titleText.fontSize = 18f;
            _titleText.fontStyle = FontStyles.Bold;
            _titleText.alignment = TextAlignmentOptions.Left;

            GameObject sGo = new GameObject("Sub", typeof(RectTransform), typeof(TextMeshProUGUI));
            sGo.transform.SetParent(_root.transform, false);
            var srt = sGo.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0f, 0.30f); srt.anchorMax = new Vector2(1f, 0.58f);
            srt.offsetMin = new Vector2(12f, 0f); srt.offsetMax = new Vector2(-12f, 0f);
            _subText = sGo.GetComponent<TextMeshProUGUI>();
            if (fnt != null) _subText.font = fnt;
            _subText.fontSize = 13.5f;
            _subText.alignment = TextAlignmentOptions.Left;

            _mainFill = CreateBar(_root.transform, "MainBar", new Vector2(0.04f, 0.14f), new Vector2(0.96f, 0.26f), new Color(0.62f, 0.42f, 0.96f, 1f));
            _decayFill = CreateBar(_root.transform, "DecayBar", new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.11f), new Color(0.96f, 0.78f, 0.36f, 0.9f));
        }

        private Image CreateBar(Transform parent, string name, Vector2 aMin, Vector2 aMax, Color fillCol) {
            GameObject bg = new GameObject(name + "BG", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(parent, false);
            var brt = bg.GetComponent<RectTransform>();
            brt.anchorMin = aMin; brt.anchorMax = aMax;
            brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;
            bg.GetComponent<Image>().color = new Color(0.18f, 0.18f, 0.26f, 0.95f);

            GameObject fill = new GameObject(name + "Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(bg.transform, false);
            var frt = fill.GetComponent<RectTransform>();
            frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
            frt.offsetMin = Vector2.zero; frt.offsetMax = Vector2.zero;
            var img = fill.GetComponent<Image>();
            img.color = fillCol;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            return img;
        }
    }
}
