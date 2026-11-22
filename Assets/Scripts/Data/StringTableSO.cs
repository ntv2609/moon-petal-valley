using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public struct StringEntry
    {
        public string key;
        [TextArea(1, 4)] public string vietnameseText;
    }

    [CreateAssetMenu(fileName = "VNStringTable", menuName = "Moonpetal/Localization/StringTableSO")]
    public class StringTableSO : ScriptableObject
    {
        public List<StringEntry> entries = new List<StringEntry>();

        public string GetText(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].key == key)
                {
                    return entries[i].vietnameseText;
                }
            }
            return "[" + key + "]";
        }

        public void SetEntry(string key, string vietnameseText)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].key == key)
                {
                    entries[i] = new StringEntry { key = key, vietnameseText = vietnameseText };
                    return;
                }
            }
            entries.Add(new StringEntry { key = key, vietnameseText = vietnameseText });
        }
    }
}
