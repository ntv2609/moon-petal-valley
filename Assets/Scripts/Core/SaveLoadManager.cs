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
        private const string DefaultWorldScene = "MoonpetalValley_MVP";

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
            public string savedSceneName = DefaultWorldScene;
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

        public static bool IsPlayableWorldScene(string sceneName)
        {
            return sceneName == "MoonpetalValley_MVP" ||
                   sceneName == "Riverbank_Chapter2" ||
                   sceneName == "WestSwamp_Chapter2";
        }

        public string GetSavedSceneName()
        {
            if (!HasSaveFile()) return DefaultWorldScene;
            try
            {
                var payload = JsonUtility.FromJson<SaveFilePayload>(File.ReadAllText(SaveFilePath));
                if (payload != null && IsPlayableWorldScene(payload.savedSceneName))
                    return payload.savedSceneName;
            }
            catch { }
            return DefaultWorldScene;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (IsPlayableWorldScene(scene.name) && MainMenuController.PendingLoadSaveOnWorldStart)
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
                LoadGameInternal(allowSceneRedirect: false);
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

            string activeScene = SceneManager.GetActiveScene().name;
            string sceneToSave = IsPlayableWorldScene(activeScene) ? activeScene : DefaultWorldScene;

            var payload = new SaveFilePayload
            {
                saveVersion = 1,
                savedTimestamp = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"),
                savedSceneName = sceneToSave
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
            return LoadGameInternal(allowSceneRedirect: true);
        }

        private bool LoadGameInternal(bool allowSceneRedirect)
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

                string targetScene = IsPlayableWorldScene(payload.savedSceneName)
                    ? payload.savedSceneName
                    : DefaultWorldScene;

                if (allowSceneRedirect && SceneManager.GetActiveScene().name != targetScene)
                {
                    MainMenuController.PendingLoadSaveOnWorldStart = true;
                    if (GameRoot.Instance.State != null) GameRoot.Instance.State.ReturnToGameplay();
                    SceneManager.LoadScene(targetScene);
                    return true;
                }

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
                if (GameRoot.Instance.Quests != null) GameRoot.Instance.Quests.RecomputeObjective();

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
