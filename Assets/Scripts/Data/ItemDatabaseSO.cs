using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Moonpetal/Config/ItemDatabaseSO")]
    public class ItemDatabaseSO : ScriptableObject
    {
        public List<ItemData> allItems = new List<ItemData>();

        public ItemData GetById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i] != null && allItems[i].itemID == itemId)
                {
                    return allItems[i];
                }
            }
            return null;
        }
    }
}
