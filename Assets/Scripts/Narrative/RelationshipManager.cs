using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Narrative
{
    public enum RelationshipTier
    {
        Stranger = 0,
        Friendly = 1,
        Trusted = 2
    }

    public class RelationshipManager : MonoBehaviour, ISaveable, IResettable
    {
        private readonly Dictionary<string, int> _affinity = new Dictionary<string, int>();

        public string SaveKey => "npc_relationships";

        [Serializable]
        private struct Entry
        {
            public string npcId;
            public int score;
        }

        [Serializable]
        private class SaveWrapper
        {
            public List<Entry> entries = new List<Entry>();
        }

        public int GetAffinity(string npcId)
        {
            if (string.IsNullOrEmpty(npcId)) return 0;
            return _affinity.TryGetValue(npcId, out int v) ? v : 0;
        }

        public RelationshipTier GetTier(string npcId)
        {
            int score = GetAffinity(npcId);
            if (score >= 4) return RelationshipTier.Trusted;
            if (score >= 2) return RelationshipTier.Friendly;
            return RelationshipTier.Stranger;
        }

        public void AddAffinity(string npcId, int delta)
        {
            if (string.IsNullOrEmpty(npcId) || delta == 0) return;
            int next = Mathf.Clamp(GetAffinity(npcId) + delta, 0, 10);
            _affinity[npcId] = next;
        }

        public string CaptureStateJson()
        {
            var w = new SaveWrapper();
            foreach (var kv in _affinity)
            {
                w.entries.Add(new Entry { npcId = kv.Key, score = kv.Value });
            }
            return JsonUtility.ToJson(w);
        }

        public void RestoreStateJson(string json)
        {
            _affinity.Clear();
            if (string.IsNullOrEmpty(json)) return;
            var w = JsonUtility.FromJson<SaveWrapper>(json);
            if (w != null && w.entries != null)
            {
                for (int i = 0; i < w.entries.Count; i++)
                {
                    _affinity[w.entries[i].npcId] = w.entries[i].score;
                }
            }
        }
    
        public void ResetForNewGame()
        {
            RestoreStateJson("{}");
        }
}
}
