using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Inventory;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2 {
    public class BrewingCauldronUIView : MonoBehaviour {
        public static BrewingCauldronUIView Instance { get; private set; }

        [SerializeField] private TMP_FontAsset vnFont;

        private GameObject _rootPanel;
        private TextMeshProUGUI _statusText;
        private TextMeshProUGUI[] _recipeCounts = new TextMeshProUGUI[3];
        private int _lastFreeSeedDay = -1;
        private bool _isOpen;

        private void Awake() {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update() {
            if (!_isOpen) return;
            bool esc = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#if ENABLE_LEGACY_INPUT_MANAGER
            if (!esc) esc = Input.GetKeyDown(KeyCode.Escape);
#endif
            if (esc) Close();
        }

        public void Open() {
            EnsureUI();
            _isOpen = true;
            _rootPanel.SetActive(true);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.BundleUI);
            RefreshCounts();
        }

        public void Close() {
            if (!_isOpen) return;
            _isOpen = false;
            if (_rootPanel != null) _rootPanel.SetActive(false);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.Gameplay);
        }

        private void EnsureUI() {
            if (_rootPanel != null) return;
            GameObject cGo = new GameObject("BrewingCauldronCanvas");
            cGo.transform.SetParent(transform, false);
            var canvas = cGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 120;
            var scaler = cGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            cGo.AddComponent<GraphicRaycaster>();

            _rootPanel = new GameObject("CauldronModal");
            _rootPanel.transform.SetParent(cGo.transform, false);
            var bgRt = _rootPanel.AddComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one; bgRt.sizeDelta = Vector2.zero;
            var dim = _rootPanel.AddComponent<Image>();
            dim.color = new Color(0.05f, 0.04f, 0.07f, 0.72f);

            GameObject card = new GameObject("Card");
            card.transform.SetParent(_rootPanel.transform, false);
            var cardRt = card.AddComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f); cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(820f, 560f);
            var cardImg = card.AddComponent<Image>();
            cardImg.color = new Color(0.16f, 0.12f, 0.14f, 0.97f);
            var ol = card.AddComponent<Outline>();
            ol.effectColor = new Color(0.76f, 0.58f, 0.34f, 1f);
            ol.effectDistance = new Vector2(3f, -3f);

            CreateText(card.transform, "VẠC NẤU THUỐC VƯỜN ƯƠM", new Vector2(0f, 232f), new Vector2(760f, 44f), 28f, new Color(1f, 0.92f, 0.62f), TextAlignmentOptions.Center);
            CreateText(card.transform, "Nấu thuốc hồi máu & bánh buff từ nông sản rồi bấm [F] / [C] ngay trong trận đánh!", new Vector2(0f, 194f), new Vector2(760f, 30f), 17f, new Color(0.86f, 0.84f, 0.80f), TextAlignmentOptions.Center);

            _recipeCounts[0] = CreateRecipeRow(card.transform, 110f, "Trà Sương Ấm (+45 Máu, +15 Thể Lực)", "Cần: 1 Lúa Sương + 1 Nhựa Sương", () => TryBrew("item_dew_tea", "item_dew_barley", 1, "item_dew_sap", 1, "Trà Sương Ấm"));
            _recipeCounts[1] = CreateRecipeRow(card.transform, 20f, "Tinh Chất Ánh Trăng (+85 Máu + 2s Bất Tử)", "Cần: 1 Cánh Trăng Non + 1 Bụi Ánh Sao", () => TryBrew("item_moon_elixir", "item_moonbud_crop", 1, "item_starlight_dust", 1, "Tinh Chất Ánh Trăng"));
            _recipeCounts[2] = CreateRecipeRow(card.transform, -70f, "Bánh Lửa Đêm (+40% DMG & +25% Tốc Độ 45s)", "Cần: 1 Ớt Đom Đóm + 1 Lúa Sương", () => TryBrew("item_nightfire_cake", "item_firefly_chili", 1, "item_dew_barley", 1, "Bánh Lửa Đêm"));

            CreateButton(card.transform, "Nhận Hạt Giống & Phân Bón Hôm Nay (Miễn Phí)", new Vector2(-120f, -172f), new Vector2(460f, 52f), new Color(0.24f, 0.48f, 0.36f, 1f), ClaimDailyStarterSeeds);
            CreateButton(card.transform, "Đóng [Esc]", new Vector2(250f, -172f), new Vector2(190f, 52f), new Color(0.45f, 0.24f, 0.24f, 1f), Close);

            _statusText = CreateText(card.transform, "", new Vector2(0f, -234f), new Vector2(760f, 34f), 17f, new Color(0.65f, 0.96f, 0.78f), TextAlignmentOptions.Center);
            _rootPanel.SetActive(false);
        }

        private TextMeshProUGUI CreateRecipeRow(Transform parent, float y, string title, string req, Action onCraft) {
            GameObject row = new GameObject("Row_" + y);
            row.transform.SetParent(parent, false);
            var rt = row.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, y);
            rt.sizeDelta = new Vector2(750f, 76f);
            var bg = row.AddComponent<Image>();
            bg.color = new Color(0.22f, 0.17f, 0.20f, 0.95f);

            CreateText(row.transform, title, new Vector2(-100f, 15f), new Vector2(520f, 28f), 19f, new Color(1f, 0.95f, 0.80f), TextAlignmentOptions.Left);
            var cnt = CreateText(row.transform, req, new Vector2(-100f, -16f), new Vector2(520f, 24f), 15f, new Color(0.78f, 0.86f, 0.94f), TextAlignmentOptions.Left);
            CreateButton(row.transform, "Nấu Ngay", new Vector2(275f, 0f), new Vector2(150f, 48f), new Color(0.66f, 0.44f, 0.18f, 1f), onCraft);
            return cnt;
        }

        private void TryBrew(string outId, string ing1, int amt1, string ing2, int amt2, string label) {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            if (PlayerCombatBuffController.CountItem(inv, ing1) < amt1 || PlayerCombatBuffController.CountItem(inv, ing2) < amt2) {
                if (_statusText != null) _statusText.text = "Chưa đủ nguyên liệu để nấu " + label + "!";
                return;
            }
            inv.RemoveItem(ing1, amt1);
            inv.RemoveItem(ing2, amt2);
            inv.AddItemById(outId, 1, true);
            if (GameRoot.Instance.Audio != null) GameRoot.Instance.Audio.PlayUIClick();
            if (_statusText != null) _statusText.text = "Đã nấu thành công 1 " + label + "!";
            RefreshCounts();
        }

        private void ClaimDailyStarterSeeds() {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            int day = GameRoot.Instance.Time != null ? GameRoot.Instance.Time.CurrentDay : 1;
            if (_lastFreeSeedDay == day) {
                if (_statusText != null) _statusText.text = "Hôm nay bạn đã nhận túi hạt giống Vườn Ươm rồi!";
                return;
            }
            _lastFreeSeedDay = day;
            var inv = GameRoot.Instance.Inventory;
            inv.AddItemById("item_dew_barley_seed", 3, false);
            inv.AddItemById("item_moonbud_seed", 2, false);
            inv.AddItemById("item_firefly_chili_seed", 2, false);
            inv.AddItemById("item_star_fertilizer", 2, false);
            if (GameRoot.Instance.Audio != null) GameRoot.Instance.Audio.PlayUIClick();
            if (_statusText != null) _statusText.text = "Đã nhận: 3 Hạt Lúa Sương, 2 Hạt Cánh Trăng Non, 2 Hạt Ớt Đom Đóm & 2 Phân Bón Tinh Bụi!";
            RefreshCounts();
        }

        private void RefreshCounts() {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            int barley = PlayerCombatBuffController.CountItem(inv, "item_dew_barley");
            int sap = PlayerCombatBuffController.CountItem(inv, "item_dew_sap");
            int moonbud = PlayerCombatBuffController.CountItem(inv, "item_moonbud_crop");
            int dust = PlayerCombatBuffController.CountItem(inv, "item_starlight_dust");
            int chili = PlayerCombatBuffController.CountItem(inv, "item_firefly_chili");

            if (_recipeCounts[0] != null) _recipeCounts[0].text = "Cần: 1 Lúa Sương (có " + barley + ") + 1 Nhựa Sương (có " + sap + ")";
            if (_recipeCounts[1] != null) _recipeCounts[1].text = "Cần: 1 Cánh Trăng Non (có " + moonbud + ") + 1 Bụi Ánh Sao (có " + dust + ")";
            if (_recipeCounts[2] != null) _recipeCounts[2].text = "Cần: 1 Ớt Đom Đóm (có " + chili + ") + 1 Lúa Sương (có " + barley + ")";
        }

        private TextMeshProUGUI CreateText(Transform parent, string text, Vector2 pos, Vector2 size, float fontSize, Color col, TextAlignmentOptions align) {
            GameObject go = new GameObject("Txt");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            if (vnFont != null) tmp.font = vnFont;
            tmp.text = text; tmp.fontSize = fontSize; tmp.color = col; tmp.alignment = align;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            return tmp;
        }

        private void CreateButton(Transform parent, string label, Vector2 pos, Vector2 size, Color bgCol, Action onClick) {
            GameObject go = new GameObject("Btn_" + label);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size;
            var img = go.AddComponent<Image>(); img.color = bgCol;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => onClick?.Invoke());
            CreateText(go.transform, label, Vector2.zero, size, 17f, Color.white, TextAlignmentOptions.Center);
        }
    }
}
