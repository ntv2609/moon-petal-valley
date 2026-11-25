using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Inventory;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI {
    public class QuickConsumableHUDView : MonoBehaviour {
        [SerializeField] private TMP_FontAsset vnFont;

        private Canvas _canvas;
        private GameObject _root;
        private Image _healIcon;
        private TextMeshProUGUI _healCountText;
        private Image _buffIcon;
        private TextMeshProUGUI _buffCountText;
        private TextMeshProUGUI _buffTimerText;
        private float _refreshTimer;

        private void Start() {
            EnsureUI();
            RefreshUI();
        }

        private void Update() {
            bool show = GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsGameplayActive;
            if (_root != null && _root.activeSelf != show) _root.SetActive(show);
            if (!show) return;

            _refreshTimer -= Time.deltaTime;
            if (_refreshTimer <= 0f) {
                _refreshTimer = 0.2f;
                RefreshUI();
            }
        }

        private void EnsureUI() {
            if (_root != null) return;
            GameObject cGo = new GameObject("QuickConsumableHUDCanvas");
            cGo.transform.SetParent(transform, false);
            _canvas = cGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 42;
            var scaler = cGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            cGo.AddComponent<GraphicRaycaster>();

            _root = new GameObject("QuickConsumableBar");
            _root.transform.SetParent(cGo.transform, false);
            var rt = _root.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0f, 0f);
            rt.anchoredPosition = new Vector2(28f, 24f);
            rt.sizeDelta = new Vector2(280f, 96f);

            BuildSlot(_root.transform, "HealSlot", new Vector2(0f, 0f), "[F] Hồi Máu", new Color(0.24f, 0.56f, 0.44f, 0.92f), out _healIcon, out _healCountText, out _);
            BuildSlot(_root.transform, "BuffSlot", new Vector2(138f, 0f), "[C] Bánh Lửa", new Color(0.68f, 0.36f, 0.16f, 0.92f), out _buffIcon, out _buffCountText, out _buffTimerText);
        }

        private void BuildSlot(Transform parent, string name, Vector2 pos, string keyLabel, Color accent, out Image iconImg, out TextMeshProUGUI countTxt, out TextMeshProUGUI timerTxt) {
            GameObject box = new GameObject(name);
            box.transform.SetParent(parent, false);
            var rt = box.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.zero; rt.pivot = Vector2.zero;
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(128f, 90f);
            var bg = box.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.10f, 0.12f, 0.88f);

            var border = box.AddComponent<Outline>();
            border.effectColor = accent;
            border.effectDistance = new Vector2(2f, -2f);

            GameObject icGo = new GameObject("Icon");
            icGo.transform.SetParent(box.transform, false);
            var icRt = icGo.AddComponent<RectTransform>();
            icRt.anchorMin = new Vector2(0.5f, 1f); icRt.anchorMax = new Vector2(0.5f, 1f); icRt.pivot = new Vector2(0.5f, 1f);
            icRt.anchoredPosition = new Vector2(0f, -8f);
            icRt.sizeDelta = new Vector2(46f, 46f);
            iconImg = icGo.AddComponent<Image>();
            iconImg.preserveAspect = true;

            GameObject cntGo = new GameObject("Count");
            cntGo.transform.SetParent(box.transform, false);
            var cntRt = cntGo.AddComponent<RectTransform>();
            cntRt.anchorMin = new Vector2(1f, 1f); cntRt.anchorMax = new Vector2(1f, 1f); cntRt.pivot = new Vector2(1f, 1f);
            cntRt.anchoredPosition = new Vector2(-8f, -6f);
            cntRt.sizeDelta = new Vector2(42f, 24f);
            countTxt = cntGo.AddComponent<TextMeshProUGUI>();
            if (vnFont != null) countTxt.font = vnFont;
            countTxt.fontSize = 18f;
            countTxt.alignment = TextAlignmentOptions.TopRight;
            countTxt.color = new Color(1f, 0.94f, 0.72f);

            GameObject lblGo = new GameObject("Label");
            lblGo.transform.SetParent(box.transform, false);
            var lblRt = lblGo.AddComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0f, 0f); lblRt.anchorMax = new Vector2(1f, 0f); lblRt.pivot = new Vector2(0.5f, 0f);
            lblRt.anchoredPosition = new Vector2(0f, 6f);
            lblRt.sizeDelta = new Vector2(0f, 24f);
            var lblTxt = lblGo.AddComponent<TextMeshProUGUI>();
            if (vnFont != null) lblTxt.font = vnFont;
            lblTxt.text = keyLabel;
            lblTxt.fontSize = 15f;
            lblTxt.alignment = TextAlignmentOptions.Center;
            lblTxt.color = Color.white;

            GameObject tGo = new GameObject("Timer");
            tGo.transform.SetParent(box.transform, false);
            var tRt = tGo.AddComponent<RectTransform>();
            tRt.anchorMin = new Vector2(0f, 1f); tRt.anchorMax = new Vector2(0f, 1f); tRt.pivot = new Vector2(0f, 1f);
            tRt.anchoredPosition = new Vector2(6f, -6f);
            tRt.sizeDelta = new Vector2(56f, 22f);
            timerTxt = tGo.AddComponent<TextMeshProUGUI>();
            if (vnFont != null) timerTxt.font = vnFont;
            timerTxt.fontSize = 14f;
            timerTxt.alignment = TextAlignmentOptions.TopLeft;
            timerTxt.color = new Color(1f, 0.72f, 0.30f);
            timerTxt.text = "";
        }

        private void RefreshUI() {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            int tea = PlayerCombatBuffController.CountItem(inv, PlayerCombatBuffController.ItemDewTeaId);
            int elixir = PlayerCombatBuffController.CountItem(inv, PlayerCombatBuffController.ItemMoonElixirId);
            int bread = PlayerCombatBuffController.CountItem(inv, PlayerCombatBuffController.ItemSunshineBreadId);
            int totalHeal = tea + elixir + bread;
            string healId = elixir > 0 && tea == 0 ? PlayerCombatBuffController.ItemMoonElixirId
                : (tea > 0 ? PlayerCombatBuffController.ItemDewTeaId : PlayerCombatBuffController.ItemMoonElixirId);
            var healData = inv.GetItemDataById(healId);
            if (_healIcon != null && healData != null && healData.icon != null) {
                _healIcon.sprite = healData.icon;
                _healIcon.color = totalHeal > 0 ? Color.white : new Color(1f, 1f, 1f, 0.32f);
            }
            if (_healCountText != null) _healCountText.text = "x" + totalHeal;

            int cake = PlayerCombatBuffController.CountItem(inv, PlayerCombatBuffController.ItemNightfireCakeId);
            var cakeData = inv.GetItemDataById(PlayerCombatBuffController.ItemNightfireCakeId);
            if (_buffIcon != null && cakeData != null && cakeData.icon != null) {
                _buffIcon.sprite = cakeData.icon;
                _buffIcon.color = cake > 0 ? Color.white : new Color(1f, 1f, 1f, 0.32f);
            }
            if (_buffCountText != null) _buffCountText.text = "x" + cake;

            if (_buffTimerText != null) {
                float rem = PlayerCombatBuffController.Instance != null ? PlayerCombatBuffController.Instance.NightfireRemaining : 0f;
                _buffTimerText.text = rem > 0.1f ? (Mathf.CeilToInt(rem) + "s") : "";
            }
        }
    }
}
