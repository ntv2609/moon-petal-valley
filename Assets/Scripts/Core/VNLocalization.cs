using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Data;

namespace MoonpetalValley.Core
{
    public class VNLocalization : MonoBehaviour
    {
        [SerializeField] private StringTableSO stringTable;
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private bool _isBuilt;

        public void Initialize()
        {
            _cache.Clear();
            if (stringTable != null && stringTable.entries != null)
            {
                for (int i = 0; i < stringTable.entries.Count; i++)
                {
                    var entry = stringTable.entries[i];
                    if (!string.IsNullOrEmpty(entry.key))
                    {
                        _cache[entry.key] = entry.vietnameseText;
                    }
                }
            }
            _isBuilt = true;
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (!_isBuilt) Initialize();

            if (_cache.TryGetValue(key, out string value))
            {
                return value;
            }
            Debug.LogWarning("[MV] Thiếu khóa bản địa hóa tiếng Việt: " + key);
            return "[" + key + "]";
        }

        public string Format(string key, params object[] args)
        {
            string template = Get(key);
            try
            {
                return string.Format(template, args);
            }
            catch
            {
                return template;
            }
        }
    }
}
