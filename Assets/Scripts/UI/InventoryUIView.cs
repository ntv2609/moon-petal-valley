using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Inventory;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class InventoryUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Button[] slotButtons;
        [SerializeField] private Image[] slotIcons;
        [SerializeField] private TMP_Text[] slotCounts;
        [SerializeField] private TMP_Text detailNameText;
        [SerializeField] private TMP_Text detailDescText;

        private GameInputReader _input;
        private bool _isOpen;
        private int _pickedSlotIndex = -1;
        private int _draggingSlotIndex = -1;
        private Image _dragGhostImage;
        private RectTransform _dragGhostRect;

        private readonly Color _hotbarBgColor = new Color(0.18f, 0.24f, 0.34f, 1f);
        private readonly Color _bagBgColor = new Color(0.14f, 0.17f, 0.24f, 1f);
        private readonly Color _pickedBgColor = new Color(0.84f, 0.68f, 0.24f, 1f);

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            CreateDragGhost();

            if (slotButtons != null)
            {
                for (int i = 0; i < slotButtons.Length; i++)
                {
                    int captured = i;
                    if (slotButtons[i] != null)
                    {
                        slotButtons[i].onClick.AddListener(() => OnSlotClicked(captured));
                        var dragHandler = slotButtons[i].gameObject.GetComponent<InventorySlotDragHandler>();
                        if (dragHandler == null) dragHandler = slotButtons[i].gameObject.AddComponent<InventorySlotDragHandler>();
                        dragHandler.Initialize(this, captured);
                    }
                }
            }
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged += RefreshGrid;
            }
        }

        private void OnDestroy()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged -= RefreshGrid;
            }
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;

            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input == null) return;

            if (_input.InventoryPressed)
            {
                if (!_isOpen && GameRoot.Instance.State.IsGameplayActive)
                {
                    Open();
                }
                else if (_isOpen)
                {
                    Close();
                }
            }
            else if (_isOpen && _input.PausePressed)
            {
                Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            _pickedSlotIndex = -1;
            CancelDrag();
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance.State.SetState(GameState.InventoryUI);
            RefreshGrid();
            ShowSlotDetails(GameRoot.Instance.Inventory.SelectedHotbarIndex);
        }

        public void Close()
        {
            _isOpen = false;
            _pickedSlotIndex = -1;
            CancelDrag();
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance.State.CurrentState == GameState.InventoryUI)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void CreateDragGhost()
        {
            GameObject go = new GameObject("InventoryDragGhost");
            go.transform.SetParent(transform, false);
            _dragGhostRect = go.AddComponent<RectTransform>();
            _dragGhostRect.sizeDelta = new Vector2(54f, 54f);
            _dragGhostImage = go.AddComponent<Image>();
            _dragGhostImage.preserveAspect = true;
            _dragGhostImage.raycastTarget = false;
            go.SetActive(false);
        }

        internal void HandleBeginDrag(int slotIndex, PointerEventData eventData)
        {
            if (!_isOpen || GameRoot.Instance?.Inventory == null) return;
            var slot = GameRoot.Instance.Inventory.GetSlot(slotIndex);
            if (slot == null || slot.IsEmpty) return;

            _draggingSlotIndex = slotIndex;
            _pickedSlotIndex = -1;

            if (_dragGhostImage != null)
            {
                _dragGhostImage.sprite = slot.item.icon;
                _dragGhostImage.color = new Color(1f, 1f, 1f, 0.92f);
                _dragGhostImage.gameObject.SetActive(true);
                _dragGhostImage.transform.SetAsLastSibling();
                _dragGhostRect.position = eventData.position;
            }
            RefreshGrid();
        }

        internal void HandleDrag(PointerEventData eventData)
        {
            if (_draggingSlotIndex < 0 || _dragGhostRect == null) return;
            _dragGhostRect.position = eventData.position;
        }

        internal void HandleDrop(int targetSlotIndex)
        {
            if (!_isOpen || _draggingSlotIndex < 0 || GameRoot.Instance?.Inventory == null) return;
            int source = _draggingSlotIndex;
            CancelDrag();
            if (source != targetSlotIndex)
            {
                GameRoot.Instance.Inventory.SwapOrMergeSlots(source, targetSlotIndex);
                GameRoot.Instance.Audio?.PlayUIClick();
            }
            ShowSlotDetails(targetSlotIndex);
            RefreshGrid();
        }

        internal void HandleEndDrag()
        {
            CancelDrag();
            RefreshGrid();
        }

        private void CancelDrag()
        {
            _draggingSlotIndex = -1;
            if (_dragGhostImage != null) _dragGhostImage.gameObject.SetActive(false);
        }

        private void RefreshGrid()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            for (int i = 0; i < InventoryManager.TotalSlots; i++)
            {
                var slot = inv.GetSlot(i);

                if (slotButtons != null && i < slotButtons.Length && slotButtons[i] != null)
                {
                    var bg = slotButtons[i].GetComponent<Image>();
                    if (bg != null)
                    {
                        if (i == _pickedSlotIndex || i == _draggingSlotIndex) bg.color = _pickedBgColor;
                        else bg.color = i < InventoryManager.HotbarSize ? _hotbarBgColor : _bagBgColor;
                    }
                }

                if (slotIcons != null && i < slotIcons.Length && slotIcons[i] != null)
                {
                    if (slot != null && !slot.IsEmpty && slot.item.icon != null)
                    {
                        slotIcons[i].sprite = slot.item.icon;
                        slotIcons[i].color = (i == _draggingSlotIndex) ? new Color(1f, 1f, 1f, 0.35f) : Color.white;
                    }
                    else
                    {
                        slotIcons[i].sprite = null;
                        slotIcons[i].color = new Color(1f, 1f, 1f, 0f);
                    }
                }

                if (slotCounts != null && i < slotCounts.Length && slotCounts[i] != null)
                {
                    if (slot == null || slot.IsEmpty) slotCounts[i].text = "";
                    else if (slot.item.itemID == "item_watering_can") slotCounts[i].text = inv.WateringCanCharges + "/20";
                    else if (slot.amount > 1) slotCounts[i].text = slot.amount.ToString();
                    else slotCounts[i].text = "";
                }
            }
        }

        private void OnSlotClicked(int index)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            // Click-to-Swap: chọn ô 1 -> bấm ô 2 để hoán đổi / gộp stack
            if (_pickedSlotIndex < 0)
            {
                var clickedSlot = inv.GetSlot(index);
                if (clickedSlot != null && !clickedSlot.IsEmpty)
                {
                    _pickedSlotIndex = index;
                }
                ShowSlotDetails(index);
                RefreshGrid();
                return;
            }

            if (_pickedSlotIndex == index)
            {
                _pickedSlotIndex = -1;
                ShowSlotDetails(index);
                RefreshGrid();
                return;
            }

            int src = _pickedSlotIndex;
            _pickedSlotIndex = -1;
            inv.SwapOrMergeSlots(src, index);
            GameRoot.Instance.Audio?.PlayUIClick();
            ShowSlotDetails(index);
            RefreshGrid();
        }

        private void ShowSlotDetails(int index)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            var slot = inv.GetSlot(index);

            if (slot == null || slot.IsEmpty)
            {
                if (detailNameText != null) detailNameText.text = "Ô trống";
                if (detailDescText != null) detailDescText.text = "Kéo-thả hoặc bấm chọn 2 ô để chuyển vật phẩm giữa Hotbar (hàng trên) và Túi đồ (hàng dưới).";
                return;
            }

            if (detailNameText != null)
            {
                string protectTag = slot.item.isQuestProtected ? " [Được bảo vệ]" : " [Nguyên liệu thường]";
                string pickHint = _pickedSlotIndex == index ? " — [ĐANG CHỌN: bấm ô khác để chuyển]" : "";
                detailNameText.text = inv.GetLocalizedItemName(slot.item) + " (x" + slot.amount + ")" + protectTag + pickHint;
            }
            if (detailDescText != null)
            {
                detailDescText.text = inv.GetLocalizedItemDesc(slot.item);
            }
        }
    }

    internal class InventorySlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        private InventoryUIView _owner;
        private int _slotIndex;

        public void Initialize(InventoryUIView owner, int slotIndex)
        {
            _owner = owner;
            _slotIndex = slotIndex;
        }

        public void OnBeginDrag(PointerEventData eventData) => _owner?.HandleBeginDrag(_slotIndex, eventData);
        public void OnDrag(PointerEventData eventData) => _owner?.HandleDrag(eventData);
        public void OnDrop(PointerEventData eventData) => _owner?.HandleDrop(_slotIndex);
        public void OnEndDrag(PointerEventData eventData) => _owner?.HandleEndDrag();
    }
}
