using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MoonpetalValley.Chapter2
{
    public class LanternRelayHUDView : MonoBehaviour
    {
        [Header("UI Skin (Step 47A - Cozy Wood Style)")]
        [SerializeField] private Sprite panelDarkWoodSprite;
        [SerializeField] private Sprite barFrameSprite;
        [SerializeField] private TMP_FontAsset menuWarmFont;
        [SerializeField] private TMP_FontAsset hudCleanFont;

        private GameObject _panelRoot;
        private TMP_Text _lanternCountText, _siteLightText, _progressText, _objectiveText;
        private Image _siteLightFill, _progressFill;

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

        public void UpdateHUD(int litCount, int totalCount, int minRequired, float siteLightPct, float buildProgressPct, string objective)
        {
            EnsureUI();
            if (_panelRoot == null || !_panelRoot.activeSelf) return;

            bool healthy = litCount >= minRequired;
            if (_lanternCountText != null)
            {
                string col = healthy ? "#FFE680" : "#FF7870";
                _lanternCountText.text = $"Đèn Cầu: <color={col}><b>{litCount}/{totalCount}</b></color> (cần ≥ {minRequired})";
            }
            if (_siteLightText != null)
                _siteLightText.text = $"Ánh Sáng Công Trường: {Mathf.RoundToInt(siteLightPct)}%";
            if (_siteLightFill != null)
            {
                _siteLightFill.fillAmount = Mathf.Clamp01(siteLightPct / 100f);
                _siteLightFill.color = healthy ? new Color(1.0f, 0.82f, 0.30f, 1f) : new Color(0.95f, 0.36f, 0.30f, 1f);
            }
            if (_progressText != null)
                _progressText.text = $"Tiến Độ Sửa Cầu: {Mathf.RoundToInt(buildProgressPct)}%";
            if (_progressFill != null)
                _progressFill.fillAmount = Mathf.Clamp01(buildProgressPct / 100f);
            if (_objectiveText != null)
                _objectiveText.text = objective;
        }

        private void EnsureUI()
        {
            if (_panelRoot != null) return;
            Canvas targetCanvas = FindTargetCanvas();
            if (targetCanvas == null)
            {
                GameObject cGo = new GameObject("LanternRelayHUDCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                targetCanvas = cGo.GetComponent<Canvas>();
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                targetCanvas.sortingOrder = 45;
                CanvasScaler cs = cGo.GetComponent<CanvasScaler>();
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = new Vector2(1920f, 1080f);
            }

            TMP_FontAsset fallbackFont = ResolveFallbackFont();
            TMP_FontAsset titleFont = menuWarmFont != null ? menuWarmFont : fallbackFont;
            TMP_FontAsset bodyFont = hudCleanFont != null ? hudCleanFont : fallbackFont;

            _panelRoot = new GameObject("LanternRelayHUDPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            _panelRoot.transform.SetParent(targetCanvas.transform, false);

            RectTransform rt = _panelRoot.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -16f);
            rt.sizeDelta = new Vector2(660f, 166f);

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
                "<b>ĐÊM CHUYỀN ĐÈN - BẮC CẦU BỜ SÔNG</b>", 19f,
                new Color32(252, 224, 146, 255), new Vector2(0f, -14f), new Vector2(612f, 26f), TextAlignmentOptions.Center);
            AddTextShadow(title);

            _lanternCountText = CreateText(_panelRoot.transform, "LanternCount", bodyFont,
                "Đèn Cầu: 6/6", 15f, new Color32(248, 240, 220, 255),
                new Vector2(-150f, -44f), new Vector2(296f, 22f), TextAlignmentOptions.Left);
            _siteLightText = CreateText(_panelRoot.transform, "SiteLightText", bodyFont,
                "Ánh Sáng Công Trường: 100%", 15f, new Color32(255, 234, 174, 255),
                new Vector2(150f, -44f), new Vector2(296f, 22f), TextAlignmentOptions.Right);

            _siteLightFill = CreateBar(_panelRoot.transform, "SiteLightBar",
                new Vector2(0f, -70f), new Vector2(604f, 15f), new Color(1f, 0.82f, 0.30f, 1f));

            _progressText = CreateText(_panelRoot.transform, "ProgressText", bodyFont,
                "Tiến Độ Sửa Cầu: 0%", 14f, new Color32(180, 244, 226, 255),
                new Vector2(0f, -92f), new Vector2(604f, 20f), TextAlignmentOptions.Left);

            _progressFill = CreateBar(_panelRoot.transform, "ProgressBar",
                new Vector2(0f, -114f), new Vector2(604f, 15f), new Color(0.32f, 0.88f, 0.78f, 1f));

            _objectiveText = CreateText(_panelRoot.transform, "Objective", bodyFont,
                "Giữ đèn sáng và chuyền dụng cụ giữa Ông Kael & Bà Mara!", 14f,
                new Color32(252, 245, 226, 255), new Vector2(0f, -136f), new Vector2(612f, 22f), TextAlignmentOptions.Center);
            AddTextShadow(_objectiveText);
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
            fillImg.fillAmount = 1f;
            return fillImg;
        }
    }
}
