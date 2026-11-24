using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Inventory;
namespace MoonpetalValley.Chapter3
{
    public class PrepWorkbenchUIView : MonoBehaviour {
        public static PrepWorkbenchUIView Instance { get; private set; }
        [SerializeField] private TMP_FontAsset vnFont;
        [SerializeField] private Sprite panelWoodSprite;
        [SerializeField] private Sprite buttonWoodSprite;
        private Canvas _canvas;
        private GameObject _rootPanel;
        private TextMeshProUGUI _subtitleText;
        private Transform _listContainer;
        private readonly List<GameObject> _rows = new List<GameObject>();
        public bool IsOpen => _rootPanel != null && _rootPanel.activeSelf;
        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        private void OnDestroy() {
            if (Instance == this) Instance = null;
        }
        private void Update() {
            if (!IsOpen) return;
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) { Close(); }
        }
        public void Open() {
            EnsureUI();
            if (_rootPanel == null) return;
            _rootPanel.SetActive(true);
            GameRoot.Instance?.State?.SetState(GameState.BundleUI);
            RefreshRows();
        }
        public void Close() {
            if (_rootPanel != null) _rootPanel.SetActive(false);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null &&
                GameRoot.Instance.State.CurrentState == GameState.BundleUI) {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }
        private void EnsureUI() {
            if (_rootPanel != null) return;
            GameObject canvasGo = new GameObject("Ch3PrepWorkbenchCanvas");
            canvasGo.transform.SetParent(transform, false);
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 140;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            canvasGo.AddComponent<GraphicRaycaster>();
            GameObject dim = new GameObject("DimOverlay");
            dim.transform.SetParent(canvasGo.transform, false);
            Image dimImg = dim.AddComponent<Image>();
            dimImg.color = new Color(0.06f, 0.04f, 0.03f, 0.68f);
            RectTransform dimRt = dim.GetComponent<RectTransform>();
            dimRt.anchorMin = Vector2.zero;
            dimRt.anchorMax = Vector2.one;
            dimRt.offsetMin = Vector2.zero;
            dimRt.offsetMax = Vector2.zero;
            _rootPanel = dim;
            GameObject panel = new GameObject("MainPanel");
            panel.transform.SetParent(dim.transform, false);
            Image panelImg = panel.AddComponent<Image>();
            if (panelWoodSprite != null) {
                panelImg.sprite = panelWoodSprite;
                panelImg.type = Image.Type.Sliced;
                panelImg.color = Color.white;
            }
            else { panelImg.color = new Color(0.22f, 0.14f, 0.09f, 0.96f); }
            RectTransform pRt = panel.GetComponent<RectTransform>();
            pRt.anchorMin = new Vector2(0.5f, 0.5f);
            pRt.anchorMax = new Vector2(0.5f, 0.5f);
            pRt.sizeDelta = new Vector2(760f, 520f);
            TextMeshProUGUI title = CreateLabel(panel.transform, "Title", "BÀN CHUẨN BỊ CHỢ ĐÊM", 26, new Color(0.99f, 0.91f, 0.72f));
            RectTransform tRt = title.rectTransform;
            tRt.anchorMin = new Vector2(0f, 1f);
            tRt.anchorMax = new Vector2(1f, 1f);
            tRt.pivot = new Vector2(0.5f, 1f);
            tRt.anchoredPosition = new Vector2(0f, -18f);
            tRt.sizeDelta = new Vector2(-40f, 36f);
            title.alignment = TextAlignmentOptions.Center;
            _subtitleText = CreateLabel(panel.transform, "Subtitle", "Chuẩn bị 5 món quà ấm áp cho Chợ Đêm đầu tiên", 15, new Color(0.92f, 0.85f, 0.74f));
            RectTransform sRt = _subtitleText.rectTransform;
            sRt.anchorMin = new Vector2(0f, 1f);
            sRt.anchorMax = new Vector2(1f, 1f);
            sRt.pivot = new Vector2(0.5f, 1f);
            sRt.anchoredPosition = new Vector2(0f, -54f);
            sRt.sizeDelta = new Vector2(-40f, 26f);
            _subtitleText.alignment = TextAlignmentOptions.Center;
            GameObject listGo = new GameObject("RecipeList");
            listGo.transform.SetParent(panel.transform, false);
            RectTransform lRt = listGo.AddComponent<RectTransform>();
            lRt.anchorMin = new Vector2(0f, 0f);
            lRt.anchorMax = new Vector2(1f, 1f);
            lRt.offsetMin = new Vector2(28f, 68f);
            lRt.offsetMax = new Vector2(-28f, -92f);
            VerticalLayoutGroup vlg = listGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            _listContainer = listGo.transform;
            Button closeBtn = CreateButton(panel.transform, "CloseBtn", "Đóng [Esc]", new Vector2(160f, 40f));
            RectTransform cRt = closeBtn.GetComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0.5f, 0f);
            cRt.anchorMax = new Vector2(0.5f, 0f);
            cRt.pivot = new Vector2(0.5f, 0f);
            cRt.anchoredPosition = new Vector2(0f, 18f);
            closeBtn.onClick.AddListener(Close);
            _rootPanel.SetActive(false);
        }
        public void RefreshRows() {
            for (int i = 0; i < _rows.Count; i++)
                if (_rows[i] != null) Destroy(_rows[i]);
            _rows.Clear();
            Chapter3StateManager st = Chapter3StateManager.Instance;
            NightMarketConfigSO cfg = st != null ? st.Config : null;
            if (cfg == null || _listContainer == null) return;
            int crafted = st.TotalCraftedItems;
            int target = cfg.targetPrepItems;
            if (_subtitleText != null) {
                _subtitleText.text = string.Format("Ngày chuẩn bị {0}/3  |  Đã chuẩn bị: {1}/{2} món Chợ Đêm",
                    Mathf.Max(1, st.CurrentPrepDayNumber), crafted, target);
            }
            InventoryManager inv = GameRoot.Instance != null ? GameRoot.Instance.Inventory : null;
            for (int i = 0; i < cfg.recipes.Count; i++) {
                NightMarketRecipeDef r = cfg.recipes[i];
                if (r == null) continue;
                CreateRecipeRow(r, inv);
            }
        }
        private void CreateRecipeRow(NightMarketRecipeDef recipe, InventoryManager inv) {
            GameObject row = new GameObject("Row_" + recipe.recipeId);
            row.transform.SetParent(_listContainer, false);
            Image bg = row.AddComponent<Image>();
            bg.color = new Color(0.16f, 0.11f, 0.08f, 0.85f);
            LayoutElement le = row.AddComponent<LayoutElement>();
            le.preferredHeight = 64f;
            GameObject iconGo = new GameObject("Icon");
            iconGo.transform.SetParent(row.transform, false);
            Image iconImg = iconGo.AddComponent<Image>();
            iconImg.preserveAspect = true;
            ItemData outItem = inv != null ? inv.GetItemDataById(recipe.outputItemId) : null;
            if (outItem != null && outItem.icon != null) iconImg.sprite = outItem.icon;
            RectTransform iRt = iconGo.GetComponent<RectTransform>();
            iRt.anchorMin = new Vector2(0f, 0.5f);
            iRt.anchorMax = new Vector2(0f, 0.5f);
            iRt.pivot = new Vector2(0f, 0.5f);
            iRt.anchoredPosition = new Vector2(10f, 0f);
            iRt.sizeDelta = new Vector2(48f, 48f);
            string ingSummary = "";
            for (int j = 0; j < recipe.ingredients.Count; j++) {
                RecipeIngredient ing = recipe.ingredients[j];
                int have = inv != null ? inv.GetItemCount(ing.itemId) : 0;
                if (j > 0) ingSummary += "  •  ";
                ingSummary += string.Format("{0} ({1}/{2})", ing.displayName, have, ing.amount);
            }
            ingSummary += string.Format("  |  -{0} Thể lực", recipe.staminaCost);
            TextMeshProUGUI nameLbl = CreateLabel(row.transform, "Name", recipe.displayName, 17, new Color(0.99f, 0.94f, 0.82f));
            RectTransform nRt = nameLbl.rectTransform;
            nRt.anchorMin = new Vector2(0f, 0.5f);
            nRt.anchorMax = new Vector2(1f, 1f);
            nRt.offsetMin = new Vector2(68f, 2f);
            nRt.offsetMax = new Vector2(-132f, -4f);
            TextMeshProUGUI subLbl = CreateLabel(row.transform, "Ingredients", ingSummary, 13, new Color(0.84f, 0.78f, 0.68f));
            RectTransform subRt = subLbl.rectTransform;
            subRt.anchorMin = new Vector2(0f, 0f);
            subRt.anchorMax = new Vector2(1f, 0.5f);
            subRt.offsetMin = new Vector2(68f, 4f);
            subRt.offsetMax = new Vector2(-132f, -2f);
            bool canCraft = NightMarketCraftingManager.Instance != null &&
                            NightMarketCraftingManager.Instance.CanCraft(recipe, out string _);
            Button craftBtn = CreateButton(row.transform, "CraftBtn", canCraft ? "Làm món" : "Thiếu đồ", new Vector2(112f, 38f));
            RectTransform bRt = craftBtn.GetComponent<RectTransform>();
            bRt.anchorMin = new Vector2(1f, 0.5f);
            bRt.anchorMax = new Vector2(1f, 0.5f);
            bRt.pivot = new Vector2(1f, 0.5f);
            bRt.anchoredPosition = new Vector2(-10f, 0f);
            craftBtn.interactable = canCraft;
            string rid = recipe.recipeId;
            craftBtn.onClick.AddListener(() => {
                if (NightMarketCraftingManager.Instance != null && NightMarketCraftingManager.Instance.TryCraft(rid)) {
                    RefreshRows();
                }
            });
            _rows.Add(row);
        }
        private TextMeshProUGUI CreateLabel(Transform parent, string name, string text, float size, Color color) {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            if (vnFont != null) tmp.font = vnFont;
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            return tmp;
        }
        private Button CreateButton(Transform parent, string name, string label, Vector2 size) {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image img = go.AddComponent<Image>();
            if (buttonWoodSprite != null) {
                img.sprite = buttonWoodSprite;
                img.type = Image.Type.Sliced;
                img.color = Color.white;
            }
            else { img.color = new Color(0.45f, 0.28f, 0.16f, 1f); }
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            Button btn = go.AddComponent<Button>();
            TextMeshProUGUI txt = CreateLabel(go.transform, "Label", label, 15, new Color(0.99f, 0.95f, 0.86f));
            txt.alignment = TextAlignmentOptions.Center;
            RectTransform trt = txt.rectTransform;
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;
            return btn;
        }
    }
}
