using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step40
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal Valley/Step 40 - Apply Architecture Cleanup")]
        public static void BuildStep40()
        {
            int changes = 0;

            if (File.Exists(MainMenuScenePath))
            {
                var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
                bool dirty = false;

                var roots = scene.GetRootGameObjects();
                for (int i = 0; i < roots.Length; i++)
                {
                    var all = roots[i].GetComponentsInChildren<Transform>(true);
                    for (int j = all.Length - 1; j >= 0; j--)
                    {
                        if (all[j] != null && all[j].name == "LivingMainMenuLayer_Step36")
                        {
                            Object.DestroyImmediate(all[j].gameObject);
                            dirty = true;
                            changes++;
                            Debug.Log("[MV Step 40] Đã gỡ lớp hiệu ứng trùng LivingMainMenuLayer_Step36 khỏi MainMenu.unity.");
                        }
                    }
                }

                for (int i = 0; i < roots.Length; i++)
                {
                    var comps = roots[i].GetComponentsInChildren<MonoBehaviour>(true);
                    for (int j = comps.Length - 1; j >= 0; j--)
                    {
                        if (comps[j] == null) continue;
                        string typeName = comps[j].GetType().Name;
                        if (typeName == "LivingMainMenuAnimator" || typeName == "MainMenuCameraLock")
                        {
                            Object.DestroyImmediate(comps[j]);
                            dirty = true;
                            changes++;
                            Debug.Log("[MV Step 40] Đã gỡ component dư thừa: " + typeName);
                        }
                    }
                }

                if (dirty)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }
            }

            string[] deadScripts = new string[]
            {
                "Assets/Scripts/UI/LivingMainMenuAnimator.cs",
                "Assets/Scripts/UI/MainMenuCameraLock.cs"
            };
            for (int i = 0; i < deadScripts.Length; i++)
            {
                if (File.Exists(deadScripts[i]))
                {
                    AssetDatabase.DeleteAsset(deadScripts[i]);
                    changes++;
                    Debug.Log("[MV Step 40] Đã xóa script dư thừa: " + deadScripts[i]);
                }
            }

            if (File.Exists(GameRootPrefabPath))
            {
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
                bool prefabDirty = false;
                var texts = prefabRoot.GetComponentsInChildren<TMP_Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i] == null || string.IsNullOrEmpty(texts[i].text)) continue;
                    string original = texts[i].text;
                    string updated = original
                        .Replace("[J]: Mở Sổ Tay", "[B]: Mở Sổ Tay")
                        .Replace("[J] Mở Sổ Tay", "[B] Mở Sổ Tay")
                        .Replace("Phím [J]: Mở Sổ Tay", "Phím [B]: Mở Sổ Tay");
                    if (updated != original)
                    {
                        texts[i].text = updated;
                        prefabDirty = true;
                        changes++;
                        Debug.Log("[MV Step 40] Đã sửa phím tắt Sổ Tay [J] -> [B] trên: " + texts[i].name);
                    }
                }

                if (prefabDirty)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, GameRootPrefabPath);
                }
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV Step 40] Hoàn tất dọn dẹp kiến trúc! Tổng thay đổi trong Editor: " + changes);
        }

        [MenuItem("Moonpetal Valley/Step 40 - Validate Architecture Cleanup")]
        public static void ValidateStep40()
        {
            int errors = 0;

            void CheckFile(string path, string mustContain, string mustNotContain, string label)
            {
                if (!File.Exists(path))
                {
                    Debug.LogError("[MV Validate] Thiếu file: " + path);
                    errors++;
                    return;
                }
                string txt = File.ReadAllText(path);
                if (!string.IsNullOrEmpty(mustContain) && !txt.Contains(mustContain))
                {
                    Debug.LogError("[MV Validate] " + label + " thiếu '" + mustContain + "' trong " + path);
                    errors++;
                }
                if (!string.IsNullOrEmpty(mustNotContain) && txt.Contains(mustNotContain))
                {
                    Debug.LogError("[MV Validate] " + label + " vẫn còn mã cũ '" + mustNotContain + "' trong " + path);
                    errors++;
                }
            }

            CheckFile("Assets/Scripts/Core/GameEventBus.cs", "OnSaveLoaded", null, "EventBus");
            CheckFile("Assets/Scripts/Core/StoryFlagManager.cs", "IResettable", null, "StoryFlagManager");
            {
                string sfPath = "Assets/Scripts/Core/StoryFlagManager.cs";
                if (File.Exists(sfPath))
                {
                    string sfTxt = File.ReadAllText(sfPath);
                    int rIdx = sfTxt.IndexOf("RestoreStateJson");
                    if (rIdx >= 0 && sfTxt.Substring(rIdx).Contains("RaiseStoryFlagChanged"))
                    {
                        Debug.LogError("[MV Validate] StoryFlagManager.RestoreStateJson vẫn còn bắn RaiseStoryFlagChanged!");
                        errors++;
                    }
                }
            }
            CheckFile("Assets/Scripts/Core/SaveLoadManager.cs", "RaiseSaveLoaded", "ToggleMute", "SaveLoadManager");
            CheckFile("Assets/Scripts/Core/GameRoot.cs", "ResetForNewGame", null, "GameRoot");
            CheckFile("Assets/Scripts/Core/GameRoot.cs", "UnregisterSaveable", null, "GameRoot");
            CheckFile("Assets/Scripts/Combat/BlightEncounterZone.cs", "OnSaveLoaded", null, "BlightEncounterZone");
            CheckFile("Assets/Scripts/Combat/BlightFogZone.cs", "OnSaveLoaded", null, "BlightFogZone");
            CheckFile("Assets/Scripts/Farming/ResourceNodeController.cs", "flag_greenhouse_cleared", null, "ResourceNodeController");
            CheckFile("Assets/Scripts/Narrative/NPCController.cs", "OnSaveLoaded", "celebratingVictory", "NPCController");
            CheckFile("Assets/Scripts/Audio/AudioManager.cs", "SetBgmVolume", null, "AudioManager");

            if (File.Exists(GameRootPrefabPath))
            {
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
                var texts = prefabRoot.GetComponentsInChildren<TMP_Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i] != null && !string.IsNullOrEmpty(texts[i].text) &&
                        texts[i].text.Contains("[J]") && texts[i].text.Contains("Sổ Tay"))
                    {
                        Debug.LogError("[MV Validate] GameRoot.prefab vẫn còn typo [J] Sổ Tay trên " + texts[i].name);
                        errors++;
                    }
                }
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }

            if (errors == 0)
            {
                Debug.Log("[MV Step 40 VALIDATE] PASS 100%! Kiến trúc đã sạch, sẵn sàng mở rộng Chương 2.");
            }
            else
            {
                Debug.LogError("[MV Step 40 VALIDATE] Phát hiện " + errors + " lỗi cần kiểm tra.");
            }
        }
    }
}
