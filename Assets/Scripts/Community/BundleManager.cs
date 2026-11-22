using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Community
{
    public class BundleManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private BundleData activeBundle;

        public event Action OnBundleUpdated;

        private readonly Dictionary<string, int> _contributed = new Dictionary<string, int>();

        public BundleData ActiveBundle => activeBundle;
        public string SaveKey => "community_bundle";

        [Serializable]
        private struct SavedContribution
        {
            public string itemId;
            public int amount;
        }

        [Serializable]
        private class BundleSaveData
        {
            public List<SavedContribution> items = new List<SavedContribution>();
        }

        public int GetContributedAmount(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && _contributed.TryGetValue(itemId, out int val) ? val : 0;
        }

        public int ContributeAllAvailable()
        {
            if (activeBundle == null || GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return 0;
            var inv = GameRoot.Instance.Inventory;
            int totalDeposited = 0;

            for (int i = 0; i < activeBundle.requiredItems.Count; i++)
            {
                var req = activeBundle.requiredItems[i];
                if (req.item == null) continue;

                int already = GetContributedAmount(req.item.itemID);
                int needed = req.amount - already;
                if (needed <= 0) continue;

                int have = inv.GetItemCount(req.item.itemID);
                int take = Mathf.Min(needed, have);
                if (take > 0 && inv.RemoveItem(req.item.itemID, take))
                {
                    _contributed[req.item.itemID] = already + take;
                    totalDeposited += take;
                }
            }

            if (totalDeposited > 0)
            {
                if (IsBundleItemsComplete() && !string.IsNullOrEmpty(activeBundle.rewardStoryFlag))
                {
                    GameRoot.Instance.Flags?.SetFlag(activeBundle.rewardStoryFlag, true);
                }
                OnBundleUpdated?.Invoke();
            }
            return totalDeposited;
        }

        public bool IsBundleItemsComplete()
        {
            if (activeBundle == null) return false;
            for (int i = 0; i < activeBundle.requiredItems.Count; i++)
            {
                var req = activeBundle.requiredItems[i];
                if (req.item == null) continue;
                if (GetContributedAmount(req.item.itemID) < req.amount) return false;
            }
            return true;
        }

        public int GetReadyHelperCount()
        {
            if (activeBundle == null || GameRoot.Instance == null || GameRoot.Instance.Flags == null) return 0;
            int count = 0;
            for (int i = 0; i < activeBundle.requiredHelperFlags.Count; i++)
            {
                if (GameRoot.Instance.Flags.HasFlag(activeBundle.requiredHelperFlags[i])) count++;
            }
            return count;
        }

        public int GetTotalRequiredHelpers()
        {
            return activeBundle != null ? activeBundle.requiredHelperFlags.Count : 4;
        }

        public bool AreAllHelpersReady() => GetReadyHelperCount() >= GetTotalRequiredHelpers();

        public bool CanBloomNow()
        {
            bool alreadyBloomed = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                                  GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            return !alreadyBloomed && IsBundleItemsComplete() && AreAllHelpersReady();
        }

        public string CaptureStateJson()
        {
            var data = new BundleSaveData();
            foreach (var kv in _contributed)
            {
                data.items.Add(new SavedContribution { itemId = kv.Key, amount = kv.Value });
            }
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            _contributed.Clear();
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<BundleSaveData>(json);
            if (data != null && data.items != null)
            {
                for (int i = 0; i < data.items.Count; i++)
                {
                    _contributed[data.items[i].itemId] = data.items[i].amount;
                }
            }
            OnBundleUpdated?.Invoke();
        }
    }
}
