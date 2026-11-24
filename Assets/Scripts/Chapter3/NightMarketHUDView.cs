using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MoonpetalValley.Chapter3
{
    public class NightMarketHUDView : MonoBehaviour
    {
        [Header("UI Skin (Cozy Wood Style)")]
        [SerializeField] private Sprite panelDarkWoodSprite;
        [SerializeField] private Sprite barFrameSprite;
        [SerializeField] private Sprite warmthHeartSprite;
        [SerializeField] private TMP_FontAsset menuWarmFont;
        [SerializeField] private TMP_FontAsset hudCleanFont;

        private GameObject _panelRoot;
        private TMP_Text _timerText;
        private TMP_Text _servedText;
        private TMP_Text _warmthText;
        private TMP_Text _requestText;
        private Image _warmthFill;
        private Image _heartIcon;

        private void Awake()
        {
            EnsureUI();
            if (_panelRoot != null) _panelRoot.SetActive(false);
        }

        public void SetVisible(bool visible)
        {
            EnsureUI();
            if (_panelRoot != null) _panelRoot.SetActive(visible);
        }

        public void UpdateHUD(float remainingSeconds, float warmthScore, float bonusThreshold, int ordersServed, string activeRequestsSummary)
        {
            EnsureUI();
            if (_panelRoot == null || !_panelRoot.activeSelf) return;

            int sec = Mathf.Max(0, Mathf.CeilToInt(remainingSeconds));
            if (_timerText != null)
            {
                string col = sec <= 15 ? "#FFE082" : "#F8F0DC";
                _timerText.text = "Thời Gian: <color=" + col + "><b>" + sec + "s</b></color>";
            }

            if (_servedText != null)
                _servedText.text = "Đã Phục Vụ: <b>" + ordersServed + "</b> lượt";

            bool reachedBonus = warmthScore >= bonusThreshold;
            int warmthInt = Mathf.RoundToInt(warmthScore);
            if (_warmthText != null)
            {
                if (reachedBonus)
                    _warmthText.text = "Độ Ấm Áp: <color=#FFE878><b>" + warmthInt + "%</b> (Đạt Mốc Thưởng!)</color>";
                else
                    _warmthText.text = "Độ Ấm Áp: <b>" + warmthInt + "%</b> (Mốc Thưởng: " + Mathf.RoundToInt(bonusThreshold) + "%)";
            }

            if (_warmthFill != null)
            {
                _warmthFill.fillAmount = Mathf.Clamp01(warmthScore / 100f);
                _warmthFill.color = reachedBonus
                    ? new Color(1.0f, 0.78f, 0.32f, 1f)
                    : new Color(0.96f, 0.48f, 0.44f, 1f);
            }

            if (_heartIcon != null)
            {
                float pulse = reachedBonus ? 1f + Mathf.Sin(Time.unscaledTime * 5f) * 0.08f : 1f;
                _heartIcon.rectTransform.localScale = new Vector3(pulse, pulse, 1f);
            }

            if (_requestText != null)
                _requestText.text = activeRequestsSummary;
        }

        private void EnsureUI()
        {
            if (_panelRoot != null) return;
            Canvas targetCanvas = FindTargetCanvas();
            if (targetCanvas == null)
            {
                GameObject cGo = new GameObject("NightMarketHUDCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                targetCanvas = cGo.GetComponent<Canvas>();
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                targetCanvas.sortingOrder = 46;
                CanvasScaler cs = cGo.GetComponent<CanvasScaler>();
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = new Vector2(1920f, 1080f);
            }

            TMP_FontAsset fallbackFont = ResolveFallbackFont();
            TMP_FontAsset titleFont = menuWarmFont != null ? menuWarmFont : fallbackFont;
            TMP_FontAsset bodyFont = hudCleanFont != null ? hudCleanFont : fallbackFont;

            _panelRoot = new GameObject("NightMarketHUDPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            _panelRoot.transform.SetParent(targetCanvas.transform, false);

            RectTransform rt = _panelRoot.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -16f);
            rt.sizeDelta = new Vector2(700f, 148f);

            Image bg = _panelRoot.GetComponent<Image>();
            if (panelDarkWoodSprite != null)
            {
                bg.sprite = panelDarkWoodSprite;
                bg.type = Image.Type.Sliced;
                bg.pixelsPerUnitMultiplier = 0.48f;
                bg.color = Color.white;
            }
            else
            {
                bg.color = new Color(0.11f, 0.08f, 0.06f, 0.94f);
            }

            Shadow panelShadow = _panelRoot.AddComponent<Shadow>();
            panelShadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
            panelShadow.effectDistance = new Vector2(0f, -4f);

            TMP_Text title = CreateText(_panelRoot.transform, "Title", titleFont,
                "<b>CHỢ ĐÊM ĐẦU TIÊN - CẦU BỜ SÔNG TRĂNG</b>", 19f,
                new Color32(252, 224, 146, 255), new Vector2(0f, -14f), new Vector2(650f, 26f), TextAlignmentOptions.Center);
            AddTextShadow(title);

            _timerText = CreateText(_panelRoot.transform, "TimerText", bodyFont,
                "Thời Gian: 100s", 15f, new Color32(248, 240, 220, 255),
                new Vector2(-165f, -44f), new Vector2(300f, 22f), TextAlignmentOptions.Left);

            _servedText = CreateText(_panelRoot.transform, "ServedText", bodyFont,
                "Đã Phục Vụ: 0 lượt", 15f, new Color32(255, 234, 174, 255),
                new Vector2(165f, -44f), new Vector2(300f, 22f), TextAlignmentOptions.Right);

            if (warmthHeartSprite != null)
            {
                GameObject heartGo = new GameObject("WarmthHeartIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                heartGo.transform.SetParent(_panelRoot.transform, false);
                RectTransform hrt = heartGo.GetComponent<RectTransform>();
                hrt.anchorMin = new Vector2(0.5f, 1f);
                hrt.anchorMax = new Vector2(0.5f, 1f);
                hrt.pivot = new Vector2(0.5f, 0.5f);
                hrt.anchoredPosition = new Vector2(-312f, -85f);
                hrt.sizeDelta = new Vector2(34f, 34f);
                _heartIcon = heartGo.GetComponent<Image>();
                _heartIcon.sprite = warmthHeartSprite;
                _heartIcon.preserveAspect = true;
            }

            _warmthText = CreateText(_panelRoot.transform, "WarmthText", bodyFont,
                "Độ Ấm Áp: 20%", 14f, new Color32(255, 214, 200, 255),
                new Vector2(18f, -68f), new Vector2(590f, 20f), TextAlignmentOptions.Left);

            _warmthFill = CreateBar(_panelRoot.transform, "WarmthBar",
                new Vector2(18f, -90f), new Vector2(590f, 16f), new Color(0.96f, 0.48f, 0.44f, 1f));

            _requestText = CreateText(_panelRoot.transform, "RequestSummary", bodyFont,
                "Ghé các gian hàng có biểu tượng trái tim để phục vụ dân làng!", 14f,
                new Color32(252, 245, 226, 255), new Vector2(0f, -114f), new Vector2(656f, 22f), TextAlignmentOptions.Center);
            AddTextShadow(_requestText);
        }

        private static Canvas FindTargetCanvas()
        {
            Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude);
            for (int i = 0; i < canvases.Length; i++)
            {
                if (canvases[i] != null && canvases[i].renderMode == RenderMode.ScreenSpaceOverlay && canvases[i].sortingOrder < 100)
                    return canvases[i];
            }
            return null;
        }

        private static TMP_FontAsset ResolveFallbackFont()
        {
            TMP_Text existing = Object.FindAnyObjectByType<TMP_Text>();
            if (existing != null && existing.font != null) return existing.font;
            return TMP_Settings.defaultFontAsset;
        }

        private static void AddTextShadow(TMP_Text txt)
        {
            if (txt == null) return;
            Shadow sh = txt.gameObject.AddComponent<Shadow>();
            sh.effectColor = new Color(0f, 0f, 0f, 0.65f);
            sh.effectDistance = new Vector2(1.5f, -1.5f);
        }

        private static TMP_Text CreateText(Transform parent, string name, TMP_FontAsset font, string content, float size, Color color, Vector2 pos, Vector2 sizeDelta, TextAlignmentOptions align)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.text = content;
            tmp.alignment = align;
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            return tmp;
        }

        private Image CreateBar(Transform parent, string name, Vector2 pos, Vector2 sizeDelta, Color fillColor)
        {
            GameObject bgGo = new GameObject(name + "_BG", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bgGo.transform.SetParent(parent, false);
            RectTransform brt = bgGo.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 1f);
            brt.anchorMax = new Vector2(0.5f, 1f);
            brt.pivot = new Vector2(0.5f, 1f);
            brt.anchoredPosition = pos;
            brt.sizeDelta = sizeDelta;

            Image bgImg = bgGo.GetComponent<Image>();
            if (barFrameSprite != null)
            {
                bgImg.sprite = barFrameSprite;
                bgImg.type = Image.Type.Sliced;
                bgImg.pixelsPerUnitMultiplier = 0.55f;
                bgImg.color = new Color(0.82f, 0.76f, 0.68f, 1f);
            }
            else
            {
                bgImg.color = new Color(0.16f, 0.12f, 0.10f, 0.95f);
            }

            GameObject fillGo = new GameObject(name + "_Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            fillGo.transform.SetParent(bgGo.transform, false);
            RectTransform frt = fillGo.GetComponent<RectTransform>();
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = new Vector2(3f, 3f);
            frt.offsetMax = new Vector2(-3f, -3f);

            Image fillImg = fillGo.GetComponent<Image>();
            fillImg.color = fillColor;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillOrigin = 0;
            fillImg.fillAmount = 0.2f;
            return fillImg;
        }
    }
}
