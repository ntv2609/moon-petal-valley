using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Narrative
{
    public class DiaryManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private List<DiaryPageData> allPages = new List<DiaryPageData>();

        public event Action OnDiaryUpdated;
        private readonly HashSet<string> _unlockedPageIds = new HashSet<string>();

        public string SaveKey => "diary_pages";

        [Serializable]
        private class DiarySaveData
        {
            public List<string> unlockedIds = new List<string>();
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && allPages[i].unlockedByFlag == flagId)
                {
                    UnlockPage(allPages[i].pageID);
                }
            }
        }

        public void UnlockPage(string pageId)
        {
            if (string.IsNullOrEmpty(pageId)) return;
            if (_unlockedPageIds.Add(pageId))
            {
                DiaryPageData page = GetPageById(pageId);
                int num = page != null ? page.pageNumber : _unlockedPageIds.Count;
                GameEventBus.RaiseToastRequested("Đã mở khóa Sổ Tay Của Bà — Trang " + num + "! Bấm [B] để đọc.");
                OnDiaryUpdated?.Invoke();
            }
        }

        public bool HasPage(string pageId) => _unlockedPageIds.Contains(pageId);
        public int UnlockedCount => _unlockedPageIds.Count;
        public bool HasAllFourPages() => _unlockedPageIds.Count >= 4;

        public List<DiaryPageData> GetUnlockedPages()
        {
            var result = new List<DiaryPageData>();
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && _unlockedPageIds.Contains(allPages[i].pageID))
                {
                    result.Add(allPages[i]);
                }
            }
            return result;
        }

        private DiaryPageData GetPageById(string pageId)
        {
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && allPages[i].pageID == pageId) return allPages[i];
            }
            return null;
        }

        public string CaptureStateJson()
        {
            return JsonUtility.ToJson(new DiarySaveData { unlockedIds = new List<string>(_unlockedPageIds) });
        }

        public void RestoreStateJson(string json)
        {
            _unlockedPageIds.Clear();
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<DiarySaveData>(json);
            if (data != null && data.unlockedIds != null)
            {
                for (int i = 0; i < data.unlockedIds.Count; i++) _unlockedPageIds.Add(data.unlockedIds[i]);
            }
            OnDiaryUpdated?.Invoke();
        }
    }
}
