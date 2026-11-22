using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "Crop_New", menuName = "Moonpetal/Data/CropData")]
    public class CropData : ScriptableObject
    {
        public string cropID;
        public string cropNameKey;
        public ItemData seedItem;
        public ItemData harvestedItem;
        public int growDays = 2;
        public int harvestYield = 1;
        public Sprite[] stageSprites = new Sprite[4];
    }
}
