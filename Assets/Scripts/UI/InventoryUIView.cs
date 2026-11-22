using UnityEngine;
using UnityEngine.UI;
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

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (slotButtons != null)
            {
                for (int i = 0; i < slotButtons.Length; i++)
                {
                    int captured = i;
                    if (slotButtons[i] != null)
                    {
                        slotButtons[i].onClick.AddListener(() => OnSlotClicked(captured));
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
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance.State.SetState(GameState.InventoryUI);
            RefreshGrid();
            OnSlotClicked(GameRoot.Instance.Inventory.SelectedHotbarIndex);
        }

        public void Close()
        {
            _isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance.State.CurrentState == GameState.InventoryUI)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void RefreshGrid()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            for (int i = 0; i < InventoryManager.TotalSlots; i++)
            {
                var slot = inv.GetSlot(i);
                if (slotIcons != null && i < slotIcons.Length && slotIcons[i] != null)
                {
                    if (slot != null && !slot.IsEmpty && slot.item.icon != null)
                    {
                        slotIcons[i].sprite = slot.item.icon;
                        slotIcons[i].color = Color.white;
                    }
                    else
                    {
                        slotIcons[i].sprite = null;
                        slotIcons[i].color = new Color(1f, 1f, 1f, 0f);
                    }
                }
                if (slotCounts != null && i < slotCounts.Length && slotCounts[i] != null)
                {
                    slotCounts[i].text = (slot != null && !slot.IsEmpty && slot.amount > 1) ? slot.amount.ToString() : "";
                }
            }
        }

        private void OnSlotClicked(int index)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            var slot = inv.GetSlot(index);

            if (slot == null || slot.IsEmpty)
            {
                if (detailNameText != null) detailNameText.text = "Ô trống";
                if (detailDescText != null) detailDescText.text = "Bấm Tab / I hoặc Esc để đóng Túi Đồ.";
                return;
            }

            if (detailNameText != null)
            {
                string protectTag = slot.item.isQuestProtected ? " [Được bảo vệ]" : " [Nguyên liệu thường]";
                detailNameText.text = inv.GetLocalizedItemName(slot.item) + " (x" + slot.amount + ")" + protectTag;
            }
            if (detailDescText != null)
            {
                detailDescText.text = inv.GetLocalizedItemDesc(slot.item);
            }
        }
    }
}
