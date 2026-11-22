using UnityEngine;

namespace MoonpetalValley.Data
{
    public enum ItemCategory
    {
        Tool,
        Seed,
        Crop,
        Material,
        Consumable,
        SeedBag,
        MoonpetalShard,
        KeyItem
    }

    [CreateAssetMenu(fileName = "Item_New", menuName = "Moonpetal/Data/ItemData")]
    public class ItemData : ScriptableObject
    {
        [Header("Định danh & Hiển thị")]
        public string itemID;
        public string displayNameKey;
        public string descriptionKey;
        public Sprite icon;
        public ItemCategory category;
        public int maxStack = 99;

        [Header("Hồi phục (Thực phẩm)")]
        public int staminaRestore;
        public int lightHPRestore;
        public bool clearsExhaustionPenalty;

        [Header("Canh tác (Hạt giống)")]
        public int growDays;
        public CropData linkedCropData;

        [Header("Bảo vệ nhiệm vụ")]
        public bool isQuestProtected;
    }
}
