using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2 {
    public class BountyBoardUIView : MonoBehaviour {
        public static BountyBoardUIView Instance { get; private set; }

        [SerializeField] private TMP_FontAsset vnFont;

        private GameObject _panelRoot;
        private readonly List<TextMeshProUGUI> _progTexts = new List<TextMeshProUGUI>();
        private readonly List<Button> _claimBtns = new List<Button>();
        private readonly List<TextMeshProUGUI> _btnLabels = new List<TextMeshProUGUI>();
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

        private void RefreshRows() {
            var mgr = Chapter2BountyManager.Instance;
            if (mgr == null) return;
            for (int i = 0; i < Chapter2BountyManager.Bounties.Length && i < _progTexts.Count; i++) {
                var b = Chapter2BountyManager.Bounties[i];
                bool claimed = mgr.IsClaimed(i);
                bool done = mgr.IsDone(i);
                int prog = mgr.GetProgress(i);

                _progTexts[i].text = claimed
                    ? "ĐÃ NHẬN THƯỞNG"
                    : (done ? $"HOÀN THÀNH ({b.target}/{b.target}) - {b.rewardText}" : $"Tiến độ: {prog}/{b.target} | {b.rewardText}");
                _progTexts[i].color = claimed ? new Color(0.62f, 0.72f, 0.68f) : (done ? new Color(0.55f, 1f, 0.72f) : new Color(1f, 0.90f, 0.62f));
                _claimBtns[i].interactable = done && !claimed;
                _btnLabels[i].text = claimed ? "Đã Nhận" : (done ? "Nhận Thưởng" : "Đang Làm");
            }
        }

        private void EnsureUI() {
            if (_panelRoot != null) return;
            TMP_FontAsset fnt = vnFont != null ? vnFont : TMP_Settings.defaultFontAsset;

            GameObject cGo = new GameObject("BountyBoardCanvas");
            cGo.transform.SetParent(transform, false);
            var cv = cGo.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cv.sortingOrder = 162;
            var scaler = cGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            cGo.AddComponent<GraphicRaycaster>();

            _panelRoot = new GameObject("BountyModal", typeof(RectTransform), typeof(Image));
            _panelRoot.transform.SetParent(cGo.transform, false);
            var bgRt = _panelRoot.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;
            _panelRoot.GetComponent<Image>().color = new Color(0.04f, 0.05f, 0.09f, 0.72f);

            GameObject box = new GameObject("Box", typeof(RectTransform), typeof(Image));
            box.transform.SetParent(_panelRoot.transform, false);
            var brt = box.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0.5f); brt.anchorMax = new Vector2(0.5f, 0.5f);
            brt.sizeDelta = new Vector2(860f, 610f);
            box.GetComponent<Image>().color = new Color(0.13f, 0.12f, 0.17f, 0.96f);
            var ol = box.AddComponent<Outline>();
            ol.effectColor = new Color(0.55f, 0.86f, 0.74f, 1f);
            ol.effectDistance = new Vector2(3f, -3f);

            MakeText(box.transform, "Title", "BẢNG NHIỆM VỤ BỜ ĐÔNG (4 MINI-BOUNTIES)", fnt, 25f, new Color(0.76f, 0.98f, 0.86f),
                new Vector2(0.5f, 1f), new Vector2(0f, -32f), new Vector2(800f, 40f), TextAlignmentOptions.Center);

            _progTexts.Clear(); _claimBtns.Clear(); _btnLabels.Clear();
            for (int i = 0; i < Chapter2BountyManager.Bounties.Length; i++) {
                var b = Chapter2BountyManager.Bounties[i];
                GameObject row = new GameObject("BountyRow_" + i, typeof(RectTransform), typeof(Image));
                row.transform.SetParent(box.transform, false);
                var rrt = row.GetComponent<RectTransform>();
                rrt.anchorMin = new Vector2(0.5f, 1f); rrt.anchorMax = new Vector2(0.5f, 1f);
                rrt.anchoredPosition = new Vector2(0f, -112f - i * 108f);
                rrt.sizeDelta = new Vector2(800f, 94f);
                row.GetComponent<Image>().color = new Color(0.18f, 0.17f, 0.24f, 0.95f);

                MakeText(row.transform, "Name", b.title, fnt, 19f, new Color(1f, 0.95f, 0.74f),
                    new Vector2(0f, 1f), new Vector2(18f, -18f), new Vector2(580f, 26f), TextAlignmentOptions.Left);
                MakeText(row.transform, "Desc", b.desc, fnt, 15f, new Color(0.86f, 0.92f, 0.98f),
                    new Vector2(0f, 1f), new Vector2(18f, -45f), new Vector2(580f, 22f), TextAlignmentOptions.Left);
                var pTxt = MakeText(row.transform, "Prog", "", fnt, 14f, Color.white,
                    new Vector2(0f, 1f), new Vector2(18f, -72f), new Vector2(580f, 22f), TextAlignmentOptions.Left);
                _progTexts.Add(pTxt);

                int captured = i;
                var btn = MakeButton(row.transform, "ClaimBtn", "Đang Làm", fnt, new Vector2(1f, 0.5f), new Vector2(-95f, 0f), new Vector2(156f, 50f),
                    new Color(0.24f, 0.62f, 0.48f, 1f), () => {
                        if (Chapter2BountyManager.Instance != null && Chapter2BountyManager.Instance.TryClaimReward(captured))
                            RefreshRows();
                    }, out var lbl);
                _claimBtns.Add(btn);
                _btnLabels.Add(lbl);
            }

            MakeButton(box.transform, "CloseBtn", "Đóng [Esc]", fnt, new Vector2(0.5f, 0f), new Vector2(0f, 36f), new Vector2(180f, 44f),
                new Color(0.32f, 0.26f, 0.36f, 1f), Close, out _);

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
            lbl = MakeText(g.transform, "Lbl", label, f, 16f, Color.white, new Vector2(0.5f, 0.5f), Vector2.zero, size, TextAlignmentOptions.Center);
            return btn;
        }
    }
}
