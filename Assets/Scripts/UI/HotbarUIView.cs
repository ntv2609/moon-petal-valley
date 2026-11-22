using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Inventory;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class HotbarUIView : MonoBehaviour
    {
        [SerializeField] private Image[] slotBackgrounds;
        [SerializeField] private Image[] slotIcons;
        [SerializeField] private TMP_Text[] slotCounts;
        [SerializeField] private TMP_Text selectedItemLabel;

        private GameInputReader _input;
        private readonly Color _normalSlotColor = new Color(0.11f, 0.15f, 0.22f, 0.88f);
        private readonly Color _selectedSlotColor = new Color(0.86f, 0.70f, 0.24f, 0.96f);

        private void OnEnable()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged += RefreshUI;
            }
        }

        private void OnDisable()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged -= RefreshUI;
            }
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged -= RefreshUI;
                GameRoot.Instance.Inventory.OnInventoryChanged += RefreshUI;
            }
            RefreshUI();
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            if (GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive) return;

            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }

            if (_input != null)
            {
                if (_input.HotbarSlotPressed >= 0)
                {
                    GameRoot.Instance.Inventory.SelectHotbarSlot(_input.HotbarSlotPressed);
                }

                if (_input.ActionPressed)
                {
                    bool pointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                    if (!pointerOverUI)
                    {
                        var sel = GameRoot.Instance.Inventory.GetSelectedSlot();
                        if (sel != null && !sel.IsEmpty && sel.item.category == ItemCategory.Consumable)
                        {
                            GameRoot.Instance.Inventory.UseSelectedConsumable();
                        }
                    }
                }
            }

            // Cuộn chuột chuyển ô Hotbar
            float scroll = ReadMouseScroll();
            if (scroll > 0.1f) GameRoot.Instance.Inventory.CycleHotbar(-1);
            else if (scroll < -0.1f) GameRoot.Instance.Inventory.CycleHotbar(1);
        }

        private float ReadMouseScroll()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                return UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.mouseScrollDelta.y;
#endif
            return 0f;
        }

        public void RefreshUI()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            for (int i = 0; i < InventoryManager.HotbarSize; i++)
            {
                var slot = inv.GetSlot(i);
                bool isSel = i == inv.SelectedHotbarIndex;

                if (slotBackgrounds != null && i < slotBackgrounds.Length && slotBackgrounds[i] != null)
                {
                    slotBackgrounds[i].color = isSel ? _selectedSlotColor : _normalSlotColor;
                }

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
                    if (slot == null || slot.IsEmpty)
                    {
                        slotCounts[i].text = "";
                    }
                    else if (slot.item.itemID == "item_watering_can")
                    {
                        slotCounts[i].text = inv.WateringCanCharges + "/20";
                    }
                    else if (slot.amount > 1)
                    {
                        slotCounts[i].text = slot.amount.ToString();
                    }
                    else
                    {
                        slotCounts[i].text = "";
                    }
                }
            }

            if (selectedItemLabel != null)
            {
                var active = inv.GetSelectedSlot();
                if (active == null || active.IsEmpty)
                {
                    selectedItemLabel.text = "Ô trống (Phím 1–8 đổi công cụ | Tab mở Túi Đồ)";
                }
                else
                {
                    string name = inv.GetLocalizedItemName(active.item);
                    string hint = active.item.category == ItemCategory.Consumable ? " — [Chuột trái / J] để ăn" : "";
                    selectedItemLabel.text = name + hint;
                }
            }
        }
    }
}
