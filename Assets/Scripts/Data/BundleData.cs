using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public struct ItemRequirement
    {
        public ItemData item;
        public int amount;
    }

    [CreateAssetMenu(fileName = "Bundle_New", menuName = "Moonpetal/Data/BundleData")]
    public class BundleData : ScriptableObject
    {
        public string bundleID;
        public string bundleTitleKey;
        public string bundleDescriptionKey;
        public List<ItemRequirement> requiredItems = new List<ItemRequirement>();
        public List<string> requiredHelperNPCIDs = new List<string>();
        public List<string> requiredHelperFlags = new List<string>();
        public ItemData rewardItem;
        public string rewardStoryFlag;
    }
}
