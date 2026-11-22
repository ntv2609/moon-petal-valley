using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.UI;

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

        private static bool _isLoadingSave;

        public static bool IsPendingLoad =>
            MainMenuController.PendingLoadSaveOnWorldStart || _isLoadingSave;

        public string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        public string LastSavedTimestamp { get; private set; } = "";

        private void OnEnable()
        {
            GameEventBus.OnDayStarted += HandleDayStartedAutoSave;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnDayStarted -= HandleDayStartedAutoSave;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start()
        {
            ReadSavedTimestampPreview();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MoonpetalValley_MVP" && MainMenuController.PendingLoadSaveOnWorldStart)
            {
                _isLoadingSave = true;
                MainMenuController.PendingLoadSaveOnWorldStart = false;
                StartCoroutine(DelayedLoadRoutine());
            }
        }

        private IEnumerator DelayedLoadRoutine()
        {
            yield return null;
            try
            {
                LoadGame();
            }
            finally
            {
                _isLoadingSave = false;
            }
        }

        private void HandleDayStartedAutoSave(int newDay) => SaveGame(true);
        public bool HasSaveFile() => File.Exists(SaveFilePath);

        private static bool IsAliveSaveable(ISaveable saveable)
        {
            if (saveable == null) return false;
            if (saveable is UnityEngine.Object unityObj && unityObj == null) return false;
            return true;
        }

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
                if (!IsAliveSaveable(saveables[i])) continue;
                payload.entries.Add(new SaveEntry
                {
                    key = saveables[i].SaveKey,
                    json = saveables[i].CaptureStateJson()
                });
            }

            try
            {
                File.WriteAllText(SaveFilePath, JsonUtility.ToJson(payload, true));
                LastSavedTimestamp = payload.savedTimestamp;
                if (!isAutoSave)
                {
                    GameEventBus.RaiseToastRequested("Đã lưu game thành công (" + LastSavedTimestamp + ")!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[MV] Lỗi lưu game: " + ex.Message);
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
                var payload = JsonUtility.FromJson<SaveFilePayload>(File.ReadAllText(SaveFilePath));
                if (payload == null || payload.entries == null) return false;

                var map = new Dictionary<string, string>();
                for (int i = 0; i < payload.entries.Count; i++)
                {
                    map[payload.entries[i].key] = payload.entries[i].json;
                }

                var saveables = GameRoot.Instance.GetSaveables();
                for (int i = 0; i < saveables.Count; i++)
                {
                    if (!IsAliveSaveable(saveables[i])) continue;
                    if (map.TryGetValue(saveables[i].SaveKey, out string stateJson))
                    {
                        saveables[i].RestoreStateJson(stateJson);
                    }
                }

                LastSavedTimestamp = payload.savedTimestamp;

                GameEventBus.RaiseSaveLoaded();
                GameRoot.Instance.Quests?.RecomputeObjective();

                GameEventBus.RaiseToastRequested("Đã tải game thành công (" + LastSavedTimestamp + ")!");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("[MV] Lỗi tải game: " + ex.Message);
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
