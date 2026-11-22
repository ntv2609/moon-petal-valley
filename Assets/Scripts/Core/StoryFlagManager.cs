using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Core
{
    public class StoryFlagManager : MonoBehaviour, ISaveable
    {
        private readonly HashSet<string> _flags = new HashSet<string>();

        [Serializable]
        private class FlagSaveWrapper
        {
            public List<string> activeFlags = new List<string>();
        }

        public string SaveKey => "story_flags";

        public bool HasFlag(string flagId)
        {
            return !string.IsNullOrEmpty(flagId) && _flags.Contains(flagId);
        }

        public void SetFlag(string flagId, bool value = true)
        {
            if (string.IsNullOrEmpty(flagId)) return;

            bool changed = value ? _flags.Add(flagId) : _flags.Remove(flagId);
            if (changed)
            {
                Debug.Log("[MV] StoryFlag: " + flagId + " = " + value);
                GameEventBus.RaiseStoryFlagChanged(flagId, value);
            }
        }

        public void ClearAllFlags()
        {
            var copy = new List<string>(_flags);
            _flags.Clear();
            for (int i = 0; i < copy.Count; i++)
            {
                GameEventBus.RaiseStoryFlagChanged(copy[i], false);
            }
        }

        public IReadOnlyCollection<string> GetAllActiveFlags()
        {
            return _flags;
        }

        public string CaptureStateJson()
        {
            var wrapper = new FlagSaveWrapper { activeFlags = new List<string>(_flags) };
            return JsonUtility.ToJson(wrapper);
        }

        public void RestoreStateJson(string json)
        {
            _flags.Clear();
            if (string.IsNullOrEmpty(json)) return;
            var wrapper = JsonUtility.FromJson<FlagSaveWrapper>(json);
            if (wrapper != null && wrapper.activeFlags != null)
            {
                for (int i = 0; i < wrapper.activeFlags.Count; i++)
                {
                    _flags.Add(wrapper.activeFlags[i]);
                }
            }
        }
    }
}
