using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.Inventory
{
    [Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int amount;

        public bool IsEmpty => item == null || amount <= 0;
    }

    public class InventoryManager : MonoBehaviour, ISaveable
    {
        public const int TotalSlots = 16;
        public const int HotbarSize = 8;
        public const int MaxWaterCharges = 20;

        [SerializeField] private ItemDatabaseSO itemDatabase;

        public event Action OnInventoryChanged;
        public event Action<int> OnSelectedSlotChanged;

        private readonly InventorySlot[] _slots = new InventorySlot[TotalSlots];
        public int SelectedHotbarIndex { get; private set; }
        public int WateringCanCharges { get; private set; } = MaxWaterCharges;

        public string SaveKey => "inventory";

        [Serializable]
        private struct SavedSlot
        {
            public int index;
            public string itemId;
            public int amount;
        }

        [Serializable]
        private class InventorySaveData
        {
            public int selectedIndex;
            public int waterCharges;
            public List<SavedSlot> slots = new List<SavedSlot>();
        }

        private void Awake()
        {
            for (int i = 0; i < TotalSlots; i++)
            {
                _slots[i] = new InventorySlot();
            }
        }

        private void OnEnable()
        {
            SleepAndKnockoutController.OnPassOutMaterialPenalty += ApplyPassOutMaterialDrop;
        }

        private void OnDisable()
        {
            SleepAndKnockoutController.OnPassOutMaterialPenalty -= ApplyPassOutMaterialDrop;
        }

        private void Start()
        {
            // Khởi tạo 3 nông cụ cơ bản nếu túi đang trống hoàn toàn
            if (IsCompletelyEmpty())
            {
                AddItemById("item_hoe", 1, false);
                AddItemById("item_watering_can", 1, false);
                AddItemById("item_hatchet", 1, false);
                SelectHotbarSlot(0);
            }
        }

        public bool IsCompletelyEmpty()
        {
            for (int i = 0; i < TotalSlots; i++)
            {
                if (!_slots[i].IsEmpty) return false;
            }
            return true;
        }

        public InventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= TotalSlots) return null;
            return _slots[index];
        }

        public InventorySlot GetSelectedSlot() => _slots[SelectedHotbarIndex];

        public ItemData GetItemDataById(string itemId)
        {
            return itemDatabase != null ? itemDatabase.GetById(itemId) : null;
        }

        public void SelectHotbarSlot(int index)
        {
            int clamped = Mathf.Clamp(index, 0, HotbarSize - 1);
            SelectedHotbarIndex = clamped;
            OnSelectedSlotChanged?.Invoke(SelectedHotbarIndex);
            OnInventoryChanged?.Invoke();
        }

        public void CycleHotbar(int delta)
        {
            int next = (SelectedHotbarIndex + delta) % HotbarSize;
            if (next < 0) next += HotbarSize;
            SelectHotbarSlot(next);
        }

        public bool SwapOrMergeSlots(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= TotalSlots || toIndex < 0 || toIndex >= TotalSlots) return false;
            if (fromIndex == toIndex) return false;

            InventorySlot fromSlot = _slots[fromIndex];
            InventorySlot toSlot = _slots[toIndex];
            if (fromSlot.IsEmpty && toSlot.IsEmpty) return false;

            // Tự động gộp stack nếu 2 ô chứa cùng loại vật phẩm có thể cộng dồn
            if (!fromSlot.IsEmpty && !toSlot.IsEmpty &&
                fromSlot.item.itemID == toSlot.item.itemID &&
                toSlot.item.maxStack > 1 &&
                toSlot.amount < toSlot.item.maxStack)
            {
                int space = toSlot.item.maxStack - toSlot.amount;
                int moveAmt = Mathf.Min(space, fromSlot.amount);
                toSlot.amount += moveAmt;
                fromSlot.amount -= moveAmt;
                if (fromSlot.amount <= 0)
                {
                    fromSlot.item = null;
                    fromSlot.amount = 0;
                }
            }
            else
            {
                ItemData tmpItem = toSlot.item;
                int tmpAmount = toSlot.amount;

                toSlot.item = fromSlot.item;
                toSlot.amount = fromSlot.amount;

                fromSlot.item = tmpItem;
                fromSlot.amount = tmpAmount;
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool AddItemById(string itemId, int amount = 1, bool showToast = true)
        {
            ItemData data = GetItemDataById(itemId);
            if (data == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy ItemData với ID: " + itemId);
                return false;
            }
            return AddItem(data, amount, showToast);
        }

        public bool AddItem(ItemData data, int amount = 1, bool showToast = true)
        {
            if (data == null || amount <= 0) return false;

            int remaining = amount;

            if (data.maxStack > 1)
            {
                for (int i = 0; i < TotalSlots && remaining > 0; i++)
                {
                    if (!_slots[i].IsEmpty && _slots[i].item.itemID == data.itemID && _slots[i].amount < data.maxStack)
                    {
                        int add = Mathf.Min(remaining, data.maxStack - _slots[i].amount);
                        _slots[i].amount += add;
                        remaining -= add;
                    }
                }
            }

            for (int i = 0; i < TotalSlots && remaining > 0; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    int add = Mathf.Min(remaining, Mathf.Max(1, data.maxStack));
                    _slots[i].item = data;
                    _slots[i].amount = add;
                    remaining -= add;
                }
            }

            int added = amount - remaining;
            if (added > 0)
            {
                OnInventoryChanged?.Invoke();
                GameEventBus.RaiseItemChanged(data.itemID, GetItemCount(data.itemID));
                if (showToast)
                {
                    string itemName = GetLocalizedItemName(data);
                    GameEventBus.RaiseToastRequested(string.Format("Nhận +{0} {1}", added, itemName));
                }
            }
            return remaining == 0;
        }

        public bool RemoveItem(string itemId, int amount = 1)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;
            if (GetItemCount(itemId) < amount) return false;

            int remaining = amount;
            for (int i = TotalSlots - 1; i >= 0 && remaining > 0; i--)
            {
                if (!_slots[i].IsEmpty && _slots[i].item.itemID == itemId)
                {
                    int take = Mathf.Min(remaining, _slots[i].amount);
                    _slots[i].amount -= take;
                    remaining -= take;
                    if (_slots[i].amount <= 0)
                    {
                        _slots[i].item = null;
                        _slots[i].amount = 0;
                    }
                }
            }

            OnInventoryChanged?.Invoke();
            GameEventBus.RaiseItemChanged(itemId, GetItemCount(itemId));
            return true;
        }

        public int GetItemCount(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return 0;
            int total = 0;
            for (int i = 0; i < TotalSlots; i++)
            {
                if (!_slots[i].IsEmpty && _slots[i].item.itemID == itemId)
                {
                    total += _slots[i].amount;
                }
            }
            return total;
        }

        public bool HasItem(string itemId, int amount = 1) => GetItemCount(itemId) >= amount;

        public bool UseSelectedConsumable()
        {
            var slot = GetSelectedSlot();
            if (slot == null || slot.IsEmpty || slot.item.category != ItemCategory.Consumable)
            {
                return false;
            }

            ItemData food = slot.item;
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                GameRoot.Instance.Stats.RestoreStats(food.staminaRestore, food.lightHPRestore, food.clearsExhaustionPenalty);
            }

            RemoveItem(food.itemID, 1);
            string foodName = GetLocalizedItemName(food);
            string extra = food.clearsExhaustionPenalty ? " & xóa phạt kiệt sức!" : "!";
            GameEventBus.RaiseToastRequested(string.Format("Đã ăn {0}: +{1} Thể lực, +{2} Máu Ánh Sáng{3}",
                foodName, food.staminaRestore, food.lightHPRestore, extra));
            return true;
        }

        public bool ConsumeWaterCharge()
        {
            if (WateringCanCharges <= 0)
            {
                GameEventBus.RaiseToastRequested("Bình Tưới đã hết nước! Hãy ra Giếng Nước bấm [E] để múc đầy.");
                return false;
            }
            WateringCanCharges--;
            OnInventoryChanged?.Invoke();
            return true;
        }

        public void RefillWateringCan()
        {
            WateringCanCharges = MaxWaterCharges;
            OnInventoryChanged?.Invoke();
            GameEventBus.RaiseToastRequested("Đã múc đầy Bình Tưới (20/20).");
        }

        public string GetLocalizedItemName(ItemData data)
        {
            if (data == null) return string.Empty;
            if (GameRoot.Instance != null && GameRoot.Instance.Loc != null)
            {
                return GameRoot.Instance.Loc.Get(data.displayNameKey);
            }
            return data.itemID;
        }

        public string GetLocalizedItemDesc(ItemData data)
        {
            if (data == null) return string.Empty;
            if (GameRoot.Instance != null && GameRoot.Instance.Loc != null)
            {
                return GameRoot.Instance.Loc.Get(data.descriptionKey);
            }
            return string.Empty;
        }

        private void ApplyPassOutMaterialDrop()
        {
            int totalDropped = 0;
            for (int i = 0; i < TotalSlots; i++)
            {
                var slot = _slots[i];
                if (slot.IsEmpty || slot.item.isQuestProtected) continue;
                if (slot.amount < 5) continue;

                int dropCount = Mathf.Max(1, Mathf.FloorToInt(slot.amount * 0.10f));
                slot.amount -= dropCount;
                totalDropped += dropCount;
                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                }
            }

            if (totalDropped > 0)
            {
                OnInventoryChanged?.Invoke();
                Debug.Log("[MV] Ngất xỉu làm rơi " + totalDropped + " nguyên liệu thường (vật phẩm nhiệm vụ được bảo vệ).");
            }
        }

        public string CaptureStateJson()
        {
            var data = new InventorySaveData
            {
                selectedIndex = SelectedHotbarIndex,
                waterCharges = WateringCanCharges
            };
            for (int i = 0; i < TotalSlots; i++)
            {
                if (!_slots[i].IsEmpty)
                {
                    data.slots.Add(new SavedSlot
                    {
                        index = i,
                        itemId = _slots[i].item.itemID,
                        amount = _slots[i].amount
                    });
                }
            }
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            for (int i = 0; i < TotalSlots; i++)
            {
                _slots[i].item = null;
                _slots[i].amount = 0;
            }
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonUtility.FromJson<InventorySaveData>(json);
            if (data == null) return;

            SelectedHotbarIndex = Mathf.Clamp(data.selectedIndex, 0, HotbarSize - 1);
            WateringCanCharges = Mathf.Clamp(data.waterCharges, 0, MaxWaterCharges);

            if (data.slots != null)
            {
                for (int i = 0; i < data.slots.Count; i++)
                {
                    var s = data.slots[i];
                    if (s.index >= 0 && s.index < TotalSlots)
                    {
                        _slots[s.index].item = GetItemDataById(s.itemId);
                        _slots[s.index].amount = s.amount;
                    }
                }
            }
            OnInventoryChanged?.Invoke();
        }
    }
}
