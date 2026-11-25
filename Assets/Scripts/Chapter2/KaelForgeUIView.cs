using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2 {
    public class KaelForgeUIView : MonoBehaviour {
        public static KaelForgeUIView Instance { get; private set; }

        public const string FlagSickleLv2 = "flag_forge_sickle_lv2";
        public const string FlagWateringLv2 = "flag_forge_watering_lv2";
        public const string FlagNightwindBoots = "flag_forge_nightwind_boots";

        private struct ForgeRecipe {
            public string flagId;
            public string resultItemId;
            public string title;
            public string desc;
            public string mat1Id;
            public string mat1Name;
            public int mat1Qty;
            public string mat2Id;
            public string mat2Name;
            public int mat2Qty;
        }

        private static readonly ForgeRecipe[] Recipes = {
            new ForgeRecipe {
                flagId = FlagSickleLv2, resultItemId = "item_moon_sickle_lv2",
                title = "Liềm Ánh Trăng Lv.2",
                desc = "+30% Sát thương Liềm & Sóng Trăng vĩnh viễn",
                mat1Id = "item_gloom_shard", mat1Name = "Mảnh Sương Đen", mat1Qty = 6,
                mat2Id = "item_branch_wood", mat2Name = "Gỗ Cành", mat2Qty = 4
            },
            new ForgeRecipe {
                flagId = FlagWateringLv2, resultItemId = "item_watering_can_lv2",
                title = "Bình Tưới Trăng 3x3",
                desc = "Tưới nước diện rộng 3x3 ô đất chỉ trong 1 lần bấm",
                mat1Id = "item_gloom_shard", mat1Name = "Mảnh Sương Đen", mat1Qty = 5,
                mat2Id = "item_starlight_dust", mat2Name = "Bụi Ánh Sao", mat2Qty = 3
            },
            new ForgeRecipe {
                flagId = FlagNightwindBoots, resultItemId = "item_nightwind_boots",
                title = "Giày Gió Đêm",
                desc = "+20% Tốc độ di chuyển vĩnh viễn trên mọi bản đồ",
                mat1Id = "item_gloom_shard", mat1Name = "Mảnh Sương Đen", mat1Qty = 5,
                mat2Id = "item_firefly_chili", mat2Name = "Ớt Đom Đóm", mat2Qty = 2
            }
        };

        [SerializeField] private TMP_FontAsset vnFont;

        private GameObject _panelRoot;
        private readonly List<TextMeshProUGUI> _statusTexts = new List<TextMeshProUGUI>();
        private readonly List<Button> _craftButtons = new List<Button>();
        private readonly List<TextMeshProUGUI> _btnLabels = new List<TextMeshProUGUI>();
        private bool _isOpen;

        private void Awake() {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        private void OnEnable() {
            GameEventBus.OnSaveLoaded += SyncForgePassiveModifiers;
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable() {
            GameEventBus.OnSaveLoaded -= SyncForgePassiveModifiers;
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }
        private void Start() { SyncForgePassiveModifiers(); }

        private void HandleFlagChanged(string flagId, bool value) {
            if (flagId == FlagSickleLv2 || flagId == FlagNightwindBoots || flagId == FlagWateringLv2)
                SyncForgePassiveModifiers();
        }

        public static void SyncForgePassiveModifiers() {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null || PlayerCombatBuffController.Instance == null)
                return;
            var flags = GameRoot.Instance.Flags;
            float dmgBonus = flags.HasFlag(FlagSickleLv2) ? 0.30f : 0f;
            float spdBonus = flags.HasFlag(FlagNightwindBoots) ? 0.20f : 0f;
            PlayerCombatBuffController.Instance.SetForgeModifiers(dmgBonus, spdBonus);
        }

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
            _panelRoot.SetActive(true);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.BundleUI);
            RefreshRows();
        }

        public void Close() {
            _isOpen = false;
            if (_panelRoot != null) _panelRoot.SetActive(false);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.Gameplay);
        }

        private void TryCraft(int idx) {
            if (idx < 0 || idx >= Recipes.Length) return;
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null || GameRoot.Instance.Flags == null) return;

            var r = Recipes[idx];
            var flags = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            if (flags.HasFlag(r.flagId)) {
                GameEventBus.RaiseToastRequested($"Bạn đã rèn {r.title} rồi!");
                return;
            }

            int have1 = PlayerCombatBuffController.CountItem(inv, r.mat1Id);
            int have2 = PlayerCombatBuffController.CountItem(inv, r.mat2Id);
            if (have1 < r.mat1Qty || have2 < r.mat2Qty) {
                GameEventBus.RaiseToastRequested($"Chưa đủ nguyên liệu để rèn {r.title}!");
                return;
            }

            inv.RemoveItem(r.mat1Id, r.mat1Qty);
            inv.RemoveItem(r.mat2Id, r.mat2Qty);
            inv.AddItemById(r.resultItemId, 1, true);
            flags.SetFlag(r.flagId, true);
            SyncForgePassiveModifiers();

            if (GameRoot.Instance.Audio != null) GameRoot.Instance.Audio.PlayUIClick();
            GameEventBus.RaiseToastRequested($"Rèn thành công {r.title}! {r.desc}!");
            RefreshRows();
        }

        private void RefreshRows() {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null || GameRoot.Instance.Flags == null) return;
            var inv = GameRoot.Instance.Inventory;
            var flags = GameRoot.Instance.Flags;

            for (int i = 0; i < Recipes.Length && i < _statusTexts.Count; i++) {
                var r = Recipes[i];
                bool owned = flags.HasFlag(r.flagId);
                int h1 = PlayerCombatBuffController.CountItem(inv, r.mat1Id);
                int h2 = PlayerCombatBuffController.CountItem(inv, r.mat2Id);
                bool canCraft = !owned && h1 >= r.mat1Qty && h2 >= r.mat2Qty;

                _statusTexts[i].text = owned
                    ? "ĐÃ RÈN VÀ KÍCH HOẠT VĨNH VIỄN"
                    : $"Cần: {r.mat1Name} ({h1}/{r.mat1Qty}) + {r.mat2Name} ({h2}/{r.mat2Qty})";
                _statusTexts[i].color = owned ? new Color(0.55f, 1f, 0.72f) : (canCraft ? new Color(1f, 0.92f, 0.56f) : new Color(0.95f, 0.62f, 0.58f));
                _craftButtons[i].interactable = canCraft;
                _btnLabels[i].text = owned ? "Đã Rèn" : "Rèn Ngay";
            }
        }

        private void EnsureUI() {
            if (_panelRoot != null) return;
            TMP_FontAsset fnt = vnFont != null ? vnFont : TMP_Settings.defaultFontAsset;

            GameObject cGo = new GameObject("KaelForgeCanvas");
            cGo.transform.SetParent(transform, false);
            var cv = cGo.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cv.sortingOrder = 160;
            var scaler = cGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            cGo.AddComponent<GraphicRaycaster>();

            _panelRoot = new GameObject("ForgeModal", typeof(RectTransform), typeof(Image));
            _panelRoot.transform.SetParent(cGo.transform, false);
            var bgRt = _panelRoot.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;
            _panelRoot.GetComponent<Image>().color = new Color(0.04f, 0.05f, 0.09f, 0.72f);

            GameObject box = new GameObject("Box", typeof(RectTransform), typeof(Image));
            box.transform.SetParent(_panelRoot.transform, false);
            var brt = box.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0.5f); brt.anchorMax = new Vector2(0.5f, 0.5f);
            brt.sizeDelta = new Vector2(820f, 520f);
            box.GetComponent<Image>().color = new Color(0.14f, 0.11f, 0.14f, 0.96f);
            var ol = box.AddComponent<Outline>();
            ol.effectColor = new Color(0.92f, 0.64f, 0.28f, 1f);
            ol.effectDistance = new Vector2(3f, -3f);

            MakeText(box.transform, "Title", "LÒ RÈN ÔNG KAEL - NÂNG CẤP TRANG BỊ", fnt, 26f, new Color(1f, 0.88f, 0.52f),
                new Vector2(0.5f, 1f), new Vector2(0f, -34f), new Vector2(760f, 44f), TextAlignmentOptions.Center);

            _statusTexts.Clear(); _craftButtons.Clear(); _btnLabels.Clear();
            for (int i = 0; i < Recipes.Length; i++) {
                var r = Recipes[i];
                GameObject row = new GameObject("Row_" + i, typeof(RectTransform), typeof(Image));
                row.transform.SetParent(box.transform, false);
                var rrt = row.GetComponent<RectTransform>();
                rrt.anchorMin = new Vector2(0.5f, 1f); rrt.anchorMax = new Vector2(0.5f, 1f);
                rrt.anchoredPosition = new Vector2(0f, -120f - i * 114f);
                rrt.sizeDelta = new Vector2(760f, 98f);
                row.GetComponent<Image>().color = new Color(0.20f, 0.16f, 0.21f, 0.95f);

                MakeText(row.transform, "Name", r.title, fnt, 20f, new Color(1f, 0.94f, 0.72f),
                    new Vector2(0f, 1f), new Vector2(20f, -20f), new Vector2(520f, 28f), TextAlignmentOptions.Left);
                MakeText(row.transform, "Desc", r.desc, fnt, 15.5f, new Color(0.84f, 0.90f, 0.98f),
                    new Vector2(0f, 1f), new Vector2(20f, -48f), new Vector2(520f, 24f), TextAlignmentOptions.Left);
                var stTxt = MakeText(row.transform, "Stat", "", fnt, 14.5f, Color.white,
                    new Vector2(0f, 1f), new Vector2(20f, -74f), new Vector2(520f, 24f), TextAlignmentOptions.Left);
                _statusTexts.Add(stTxt);

                int captured = i;
                var btn = MakeButton(row.transform, "CraftBtn", "Rèn Ngay", fnt, new Vector2(1f, 0.5f), new Vector2(-95f, 0f), new Vector2(150f, 52f),
                    new Color(0.76f, 0.44f, 0.16f, 1f), () => TryCraft(captured), out var lbl);
                _craftButtons.Add(btn);
                _btnLabels.Add(lbl);
            }

            MakeButton(box.transform, "CloseBtn", "Đóng [Esc]", fnt, new Vector2(0.5f, 0f), new Vector2(0f, 38f), new Vector2(180f, 44f),
                new Color(0.36f, 0.24f, 0.28f, 1f), Close, out _);

            _panelRoot.SetActive(false);
        }

        private static TextMeshProUGUI MakeText(Transform p, string n, string txt, TMP_FontAsset f, float sz, Color c, Vector2 anc, Vector2 pos, Vector2 size, TextAlignmentOptions align) {
            GameObject g = new GameObject(n, typeof(RectTransform), typeof(TextMeshProUGUI));
            g.transform.SetParent(p, false);
            var rt = g.GetComponent<RectTransform>();
            rt.anchorMin = anc; rt.anchorMax = anc;
            rt.pivot = new Vector2(anc.x, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size;
            var t = g.GetComponent<TextMeshProUGUI>();
            if (f != null) t.font = f;
            t.text = txt; t.fontSize = sz; t.color = c; t.alignment = align;
            return t;
        }

        private static Button MakeButton(Transform p, string n, string label, TMP_FontAsset f, Vector2 anc, Vector2 pos, Vector2 size, Color bg, UnityEngine.Events.UnityAction onClick, out TextMeshProUGUI lbl) {
            GameObject g = new GameObject(n, typeof(RectTransform), typeof(Image), typeof(Button));
            g.transform.SetParent(p, false);
            var rt = g.GetComponent<RectTransform>();
            rt.anchorMin = anc; rt.anchorMax = anc; rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size;
            g.GetComponent<Image>().color = bg;
            var btn = g.GetComponent<Button>();
            btn.onClick.AddListener(onClick);
            lbl = MakeText(g.transform, "Lbl", label, f, 17f, Color.white, new Vector2(0.5f, 0.5f), Vector2.zero, size, TextAlignmentOptions.Center);
            return btn;
        }
    }
}
