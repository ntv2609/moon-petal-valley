using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Community;

namespace MoonpetalValley.Core
{
    public class SaveLoadManager : MonoBehaviour
    {
        private const string SaveFileName = "moonpetal_save_v1.json";

        [Serializable]
        private struct SaveEntry
        {
            public string key;
            public string json;
        }

        [Serializable]
        private class SaveFilePayload
        {
            public int saveVersion = 1;
            public string savedTimestamp;
            public List<SaveEntry> entries = new List<SaveEntry>();
        }

        public string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        public string LastSavedTimestamp { get; private set; } = "";

        private void OnEnable()
        {
            GameEventBus.OnDayStarted += HandleDayStartedAutoSave;
        }

        private void OnDisable()
        {
            GameEventBus.OnDayStarted -= HandleDayStartedAutoSave;
        }

        private void Start()
        {
            ReadSavedTimestampPreview();
        }

        private void HandleDayStartedAutoSave(int newDay)
        {
            SaveGame(true);
        }

        public bool HasSaveFile() => File.Exists(SaveFilePath);

        public void SaveGame(bool isAutoSave = false)
        {
            if (GameRoot.Instance == null) return;

            var payload = new SaveFilePayload
            {
                saveVersion = 1,
                savedTimestamp = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
            };

            var saveables = GameRoot.Instance.GetSaveables();
            for (int i = 0; i < saveables.Count; i++)
            {
                if (saveables[i] == null) continue;
                payload.entries.Add(new SaveEntry
                {
                    key = saveables[i].SaveKey,
                    json = saveables[i].CaptureStateJson()
                });
            }

            try
            {
                string json = JsonUtility.ToJson(payload, true);
                File.WriteAllText(SaveFilePath, json);
                LastSavedTimestamp = payload.savedTimestamp;
                Debug.Log("[MV] Đã lưu game tại: " + SaveFilePath);
                if (!isAutoSave)
                {
                    GameEventBus.RaiseToastRequested("Đã lưu game thành công (" + LastSavedTimestamp + ")!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[MV] Lỗi lưu game: " + ex.Message);
                GameEventBus.RaiseToastRequested("Không thể lưu file game!");
            }
        }

        public bool LoadGame()
        {
            if (!HasSaveFile() || GameRoot.Instance == null)
            {
                GameEventBus.RaiseToastRequested("Chưa có file lưu game nào!");
                return false;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var payload = JsonUtility.FromJson<SaveFilePayload>(json);
                if (payload == null || payload.entries == null) return false;

                var map = new Dictionary<string, string>();
                for (int i = 0; i < payload.entries.Count; i++)
                {
                    map[payload.entries[i].key] = payload.entries[i].json;
                }

                var saveables = GameRoot.Instance.GetSaveables();
                for (int i = 0; i < saveables.Count; i++)
                {
                    if (saveables[i] != null && map.TryGetValue(saveables[i].SaveKey, out string stateJson))
                    {
                        saveables[i].RestoreStateJson(stateJson);
                    }
                }

                LastSavedTimestamp = payload.savedTimestamp;
                GameRoot.Instance.Quests?.RecomputeObjective();

                var spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
                if (spot != null) spot.RefreshBloomVisual();

                GameEventBus.RaiseToastRequested("Đã tải game thành công (" + LastSavedTimestamp + ")!");
                Debug.Log("[MV] Đã khôi phục trạng thái game từ: " + SaveFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("[MV] Lỗi tải game: " + ex.Message);
                GameEventBus.RaiseToastRequested("File lưu game bị lỗi!");
                return false;
            }
        }

        public void DeleteSaveFile()
        {
            if (HasSaveFile())
            {
                File.Delete(SaveFilePath);
                LastSavedTimestamp = "";
                GameEventBus.RaiseToastRequested("Đã xóa file lưu game.");
            }
        }

        private void ReadSavedTimestampPreview()
        {
            if (!HasSaveFile()) return;
            try
            {
                var payload = JsonUtility.FromJson<SaveFilePayload>(File.ReadAllText(SaveFilePath));
                if (payload != null) LastSavedTimestamp = payload.savedTimestamp;
            }
            catch { }
        }
    }
}
