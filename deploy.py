# deploy.py — Moonpetal Valley — Step 1.fix 1
import os
import sys
import shutil
from pathlib import Path
from datetime import datetime

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

PROJECT_ROOT = Path.cwd()
ASSETS_DIR = PROJECT_ROOT / "Assets"
SETTINGS_DIR = PROJECT_ROOT / "ProjectSettings"

if not ASSETS_DIR.is_dir() or not SETTINGS_DIR.is_dir():
    print("[LỖI] Không tìm thấy thư mục 'Assets/' và 'ProjectSettings/' tại thư mục hiện tại.")
    sys.exit(1)

FILES = {
    "Assets/Scripts/DebugTools/RuntimeDebugOverlay.cs": r'''using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.DebugTools
{
    public class RuntimeDebugOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button btnTestToast;
        [SerializeField] private Button btnToggleTestFlag;
        [SerializeField] private Button btnClearFlags;
        [SerializeField] private Button btnTogglePause;

        private bool _isOpen;
        private const string TestFlagId = "flag_debug_step1_verified";

        private void Awake()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }

            if (btnTestToast != null) btnTestToast.onClick.AddListener(OnClickTestToast);
            if (btnToggleTestFlag != null) btnToggleTestFlag.onClick.AddListener(OnClickToggleTestFlag);
            if (btnClearFlags != null) btnClearFlags.onClick.AddListener(OnClickClearFlags);
            if (btnTogglePause != null) btnTogglePause.onClick.AddListener(OnClickTogglePause);
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged += OnGameStateUpdated;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged -= OnGameStateUpdated;
        }

        private void Update()
        {
            if (!Application.isEditor && !Debug.isDebugBuild) return;

            if (WasF1PressedThisFrame())
            {
                TogglePanel();
            }
        }

        private bool WasF1PressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame)
            {
                return true;
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F1))
            {
                return true;
            }
#endif
            return false;
        }

        public void TogglePanel()
        {
            _isOpen = !_isOpen;
            if (panelRoot != null)
            {
                panelRoot.SetActive(_isOpen);
            }
            if (_isOpen)
            {
                RefreshStatusText();
            }
        }

        private void OnClickTestToast()
        {
            string msg = GameRoot.Instance != null && GameRoot.Instance.Loc != null
                ? GameRoot.Instance.Loc.Get("toast_step1_test")
                : "Kiểm thử thông báo tiếng Việt có dấu thành công!";
            GameEventBus.RaiseToastRequested(msg);
        }

        private void OnClickToggleTestFlag()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            bool next = !GameRoot.Instance.Flags.HasFlag(TestFlagId);
            GameRoot.Instance.Flags.SetFlag(TestFlagId, next);
            GameEventBus.RaiseToastRequested("Flag thử nghiệm: " + (next ? "BẬT" : "TẮT"));
            RefreshStatusText();
        }

        private void OnClickClearFlags()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            GameRoot.Instance.Flags.ClearAllFlags();
            GameEventBus.RaiseToastRequested("Đã xóa toàn bộ StoryFlag.");
            RefreshStatusText();
        }

        private void OnClickTogglePause()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            var next = GameRoot.Instance.State.IsGameplayActive ? GameState.PauseMenu : GameState.Gameplay;
            GameRoot.Instance.State.SetState(next);
            RefreshStatusText();
        }

        private void OnAnyStateUpdated(string _, bool __) => RefreshStatusText();
        private void OnGameStateUpdated(GameState _) => RefreshStatusText();

        public void RefreshStatusText()
        {
            if (statusText == null || GameRoot.Instance == null) return;

            var sb = new StringBuilder();
            sb.AppendLine("<b>BẢNG DEBUG MOONPETAL (F1)</b>");
            sb.AppendLine("Trạng thái: " + GameRoot.Instance.State.CurrentState);
            sb.AppendLine("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            sb.Append("StoryFlags đang bật: ");
            var flags = GameRoot.Instance.Flags.GetAllActiveFlags();
            if (flags.Count == 0)
            {
                sb.Append("(chưa có)");
            }
            else
            {
                bool first = true;
                foreach (var f in flags)
                {
                    if (!first) sb.Append(", ");
                    sb.Append(f);
                    first = false;
                }
            }
            statusText.text = sb.ToString();
        }
    }
}
''',

    "Assets/Editor/AutoBuilder_Step1.cs": r'''using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step1
    {
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string FontCopyTtfPath = "Assets/Art/Fonts/VN_SourceFont.ttf";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string TimeConfigPath = "Assets/Data/Config/TimeConfig.asset";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";

        private static readonly string[] RequiredTags =
        {
            "NPC", "Enemy", "Interactable", "FarmTile", "ResourceNode", "BlightFog"
        };

        private static readonly (int index, string name)[] RequiredLayers =
        {
            (8, "Player"),
            (9, "Enemy"),
            (10, "Obstacle"),
            (11, "Interactable"),
            (12, "PlayerHitbox"),
            (13, "EnemyHitbox"),
            (14, "TriggerZone")
        };

        private static readonly string[] RequiredSortingLayers =
        {
            "Ground", "FloorDecal", "FarmTile", "Shadows",
            "WorldObjects", "FogOverlay", "WeatherVFX", "UI"
        };

        [MenuItem("Moonpetal/Step 1/1. Build Foundation & Scenes")]
        public static void BuildStep1()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 1 ===");
            CheckInputHandlingConfiguration();
            EnsureFolderStructure();
            ConfigureTagsLayersAndSortingLayers();
            ConfigurePhysics2DMatrix();

            TMP_FontAsset vnFont = EnsureVietnameseTMPFont();
            EnsurePlayerStatsAsset();
            EnsureTimeConfigAsset();
            StringTableSO stringTable = EnsureStringTableAsset();

            GameObject gameRootPrefab = BuildOrUpdateGameRootPrefab(stringTable, vnFont);
            BuildBootstrapScene(gameRootPrefab);
            BuildWorldPlaceholderScene(gameRootPrefab, vnFont);
            ConfigureBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 1! Hãy chạy 'Moonpetal/Step 1/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 1/Validate")]
        public static void ValidateStep1()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 1 ===");

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");
            foreach (string reqTag in RequiredTags)
            {
                if (!PropertyArrayContainsString(tagsProp, reqTag))
                {
                    Debug.LogError("[MV] THIẾU Tag: " + reqTag);
                    errors++;
                }
            }

            var layersProp = tagManager.FindProperty("layers");
            foreach (var pair in RequiredLayers)
            {
                string actual = layersProp.GetArrayElementAtIndex(pair.index).stringValue;
                if (actual != pair.name)
                {
                    Debug.LogError("[MV] SAI Layer " + pair.index + ": cần '" + pair.name + "', hiện tại '" + actual + "'");
                    errors++;
                }
            }

            errors += CheckAssetExists<TMP_FontAsset>(FontAssetPath);
            errors += CheckAssetExists<PlayerStatsSO>(PlayerStatsPath);
            errors += CheckAssetExists<TimeConfigSO>(TimeConfigPath);
            errors += CheckAssetExists<StringTableSO>(StringTablePath);
            errors += CheckAssetExists<GameObject>(GameRootPrefabPath);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                var root = prefab.GetComponent<GameRoot>();
                if (root == null)
                {
                    Debug.LogError("[MV] GameRoot.prefab thiếu component GameRoot.");
                    errors++;
                }
                else
                {
                    var so = new SerializedObject(root);
                    string[] fields = { "gameStateManager", "storyFlagManager", "localization", "sceneLoader", "toastUI", "debugOverlay" };
                    foreach (string f in fields)
                    {
                        var p = so.FindProperty(f);
                        if (p == null || p.objectReferenceValue == null)
                        {
                            Debug.LogError("[MV] GameRoot thiếu tham chiếu SerializeField: " + f);
                            errors++;
                        }
                    }
                }
            }

            if (EditorBuildSettings.scenes.Length < 2 ||
                EditorBuildSettings.scenes[0].path != BootstrapScenePath ||
                EditorBuildSettings.scenes[1].path != WorldScenePath)
            {
                Debug.LogError("[MV] Build Settings chưa đúng thứ tự Bootstrap (0) -> MoonpetalValley_MVP (1).");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (missing > 0)
                {
                    Debug.LogError("[MV] GameObject '" + go.name + "' có " + missing + " Missing Script!");
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 1: PASS (0 lỗi). Toàn bộ nền tảng, Font VN, Data SO và Scene đã sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 1: FAIL (" + errors + " lỗi). Xem các dòng lỗi ở trên.");
            }
        }

        private static int CheckAssetExists<T>(string path) where T : UnityEngine.Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                Debug.LogError("[MV] THIẾU Asset: " + path);
                return 1;
            }
            return 0;
        }

        private static void CheckInputHandlingConfiguration()
        {
#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            Debug.Log("[MV] Input Handling: Both (Input System + Legacy) — Tương thích 100%.");
#elif ENABLE_INPUT_SYSTEM
            Debug.Log("[MV] Input Handling: New Input System Only — Tự động gắn InputSystemUIInputModule.");
#elif ENABLE_LEGACY_INPUT_MANAGER
            Debug.Log("[MV] Input Handling: Legacy Input Manager Only — Tự động gắn StandaloneInputModule.");
#else
            Debug.LogWarning("[MV] Cảnh báo: Không phát hiện Input Backend nào đang bật trong PlayerSettings!");
#endif
        }

        private static void EnsureFolderStructure()
        {
            string[] folders =
            {
                "Assets/Art/Fonts",
                "Assets/Art/Sprites/Characters",
                "Assets/Art/Sprites/Enemies",
                "Assets/Art/Sprites/Crops",
                "Assets/Art/Sprites/Environment",
                "Assets/Art/Sprites/Items",
                "Assets/Art/Sprites/UI",
                "Assets/Audio/BGM",
                "Assets/Audio/SFX",
                "Assets/Data/Config",
                "Assets/Data/Localization",
                "Assets/Data/Items",
                "Assets/Data/Crops",
                "Assets/Data/Bundles",
                "Assets/Data/Enemies",
                "Assets/Data/NPCs",
                "Assets/Data/Dialogues",
                "Assets/Data/Diary",
                "Assets/Data/Quests",
                "Assets/Prefabs/Core",
                "Assets/Prefabs/Characters",
                "Assets/Prefabs/Enemies",
                "Assets/Prefabs/World",
                "Assets/Prefabs/UI",
                "Assets/Scenes"
            };

            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            AssetDatabase.Refresh();
        }

        private static void ConfigureTagsLayersAndSortingLayers()
        {
            var tagManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            var so = new SerializedObject(tagManagerAsset);

            var tagsProp = so.FindProperty("tags");
            foreach (string tag in RequiredTags)
            {
                if (!PropertyArrayContainsString(tagsProp, tag))
                {
                    int idx = tagsProp.arraySize;
                    tagsProp.InsertArrayElementAtIndex(idx);
                    tagsProp.GetArrayElementAtIndex(idx).stringValue = tag;
                }
            }

            var layersProp = so.FindProperty("layers");
            foreach (var pair in RequiredLayers)
            {
                var elem = layersProp.GetArrayElementAtIndex(pair.index);
                elem.stringValue = pair.name;
            }

            var sortingProp = so.FindProperty("m_SortingLayers");
            foreach (string slName in RequiredSortingLayers)
            {
                bool exists = false;
                for (int i = 0; i < sortingProp.arraySize; i++)
                {
                    if (sortingProp.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == slName)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    int idx = sortingProp.arraySize;
                    sortingProp.InsertArrayElementAtIndex(idx);
                    var entry = sortingProp.GetArrayElementAtIndex(idx);
                    entry.FindPropertyRelative("name").stringValue = slName;
                    entry.FindPropertyRelative("uniqueID").intValue = Mathf.Abs(slName.GetHashCode()) + idx + 1000;
                }
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(tagManagerAsset);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] Đã ghi bền vững Tags, Layers (8-14) và Sorting Layers vào TagManager.asset.");
        }

        private static void ConfigurePhysics2DMatrix()
        {
            Physics2D.IgnoreLayerCollision(8, 12, true);
            Physics2D.IgnoreLayerCollision(9, 13, true);
            Physics2D.IgnoreLayerCollision(12, 13, true);
            Physics2D.IgnoreLayerCollision(12, 10, true);
            Physics2D.IgnoreLayerCollision(13, 10, true);

            var physAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/Physics2DSettings.asset");
            if (physAssets != null && physAssets.Length > 0)
            {
                EditorUtility.SetDirty(physAssets[0]);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] Đã cấu hình và lưu Ma trận va chạm Physics2D.");
        }

        private static TMP_FontAsset EnsureVietnameseTMPFont()
        {
            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            if (existing != null)
            {
                return existing;
            }

            string customTtf = "Assets/Art/Fonts/VNFont.ttf";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(customTtf);

            if (sourceFont == null)
            {
                string[] windowsCandidates =
                {
                    "C:/Windows/Fonts/segoeui.ttf",
                    "C:/Windows/Fonts/arial.ttf",
                    "C:/Windows/Fonts/tahoma.ttf"
                };

                foreach (string sysPath in windowsCandidates)
                {
                    if (File.Exists(sysPath))
                    {
                        File.Copy(sysPath, FontCopyTtfPath, true);
                        AssetDatabase.ImportAsset(FontCopyTtfPath, ImportAssetOptions.ForceUpdate);
                        sourceFont = AssetDatabase.LoadAssetAtPath<Font>(FontCopyTtfPath);
                        if (sourceFont != null)
                        {
                            Debug.Log("[MV] Đã sao chép font Unicode hệ thống: " + sysPath);
                            break;
                        }
                    }
                }
            }

            if (sourceFont == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy font TTF hệ thống. Hãy chép 1 file font hỗ trợ tiếng Việt vào Assets/Art/Fonts/VNFont.ttf rồi chạy lại menu Step 1.");
                return TMP_Settings.defaultFontAsset;
            }

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont);
            if (fontAsset == null)
            {
                Debug.LogError("[MV] Không thể khởi tạo TMP_FontAsset Dynamic từ " + sourceFont.name);
                return null;
            }

            fontAsset.name = "VN_DynamicFont";
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;

            AssetDatabase.CreateAsset(fontAsset, FontAssetPath);
            if (fontAsset.material != null)
            {
                fontAsset.material.name = "VN_DynamicFont Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, FontAssetPath);
            }
            if (fontAsset.atlasTexture != null)
            {
                fontAsset.atlasTexture.name = "VN_DynamicFont Atlas";
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, FontAssetPath);
            }

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] Đã tạo thành công Font tiếng Việt Dynamic: " + FontAssetPath);
            return fontAsset;
        }

        private static PlayerStatsSO EnsurePlayerStatsAsset()
        {
            var asset = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<PlayerStatsSO>();
                AssetDatabase.CreateAsset(asset, PlayerStatsPath);
            }
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static TimeConfigSO EnsureTimeConfigAsset()
        {
            var asset = AssetDatabase.LoadAssetAtPath<TimeConfigSO>(TimeConfigPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<TimeConfigSO>();
                AssetDatabase.CreateAsset(asset, TimeConfigPath);
            }
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static StringTableSO EnsureStringTableAsset()
        {
            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            if (table == null)
            {
                table = ScriptableObject.CreateInstance<StringTableSO>();
                AssetDatabase.CreateAsset(table, StringTablePath);
            }

            table.SetEntry("ui_game_title", "Thung Lũng Hoa Ánh Trăng");
            table.SetEntry("ui_step1_banner", "Thung Lũng Hoa Ánh Trăng — Nền tảng Step 1 sẵn sàng!\nBấm phím F1 để mở bảng Debug kiểm thử tiếng Việt & StoryFlag.");
            table.SetEntry("toast_step1_test", "Chào mừng Wen đến với Thung Lũng Hoa Ánh Trăng! (Tiếng Việt có dấu OK)");
            table.SetEntry("ui_stamina_label", "Thể lực");
            table.SetEntry("ui_lighthp_label", "Máu Ánh Sáng");
            table.SetEntry("ui_day_format", "Ngày {0} — {1:00}:{2:00}");

            EditorUtility.SetDirty(table);
            return table;
        }

        private static GameObject BuildOrUpdateGameRootPrefab(StringTableSO stringTable, TMP_FontAsset vnFont)
        {
            GameObject rootObj = new GameObject("GameRoot");
            var gameRoot = rootObj.AddComponent<GameRoot>();
            var stateMgr = rootObj.AddComponent<GameStateManager>();
            var flagMgr = rootObj.AddComponent<StoryFlagManager>();
            var loc = rootObj.AddComponent<VNLocalization>();
            var sceneLoader = rootObj.AddComponent<SceneLoader>();

            var locSO = new SerializedObject(loc);
            locSO.FindProperty("stringTable").objectReferenceValue = stringTable;
            locSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject overlayCanvasGo = CreateUICanvas("SystemOverlayCanvas", rootObj.transform, 100);

            GameObject toastPanel = CreateUIPanel("ToastPanel", overlayCanvasGo.transform,
                new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.88f), new Vector2(760f, 68f),
                new Color(0.10f, 0.14f, 0.22f, 0.90f));
            var toastGroup = toastPanel.AddComponent<CanvasGroup>();
            var toastText = CreateTMPText("ToastText", toastPanel.transform, "Thông báo", vnFont, 24, TextAlignmentOptions.Center);
            var toastUI = toastPanel.AddComponent<FloatingToastUI>();
            var toastSO = new SerializedObject(toastUI);
            toastSO.FindProperty("canvasGroup").objectReferenceValue = toastGroup;
            toastSO.FindProperty("toastText").objectReferenceValue = toastText;
            toastSO.FindProperty("displayDuration").floatValue = 2.6f;
            toastSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject debugPanel = CreateUIPanel("DebugPanelRoot", overlayCanvasGo.transform,
                new Vector2(0.02f, 0.96f), new Vector2(0.02f, 0.96f), new Vector2(460f, 320f),
                new Color(0.07f, 0.09f, 0.14f, 0.94f));
            var debugRect = debugPanel.GetComponent<RectTransform>();
            debugRect.pivot = new Vector2(0f, 1f);

            var vlg = debugPanel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(16, 16, 14, 14);
            vlg.spacing = 8f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;

            var statusText = CreateTMPText("DebugStatusText", debugPanel.transform, "BẢNG DEBUG MOONPETAL (F1)", vnFont, 18, TextAlignmentOptions.TopLeft);
            statusText.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 110f);

            var btnToast = CreateUIButton("BtnTestToast", debugPanel.transform, "1. Test Toast Tiếng Việt", vnFont);
            var btnFlag = CreateUIButton("BtnToggleFlag", debugPanel.transform, "2. Bật/Tắt StoryFlag Thử Nghiệm", vnFont);
            var btnClear = CreateUIButton("BtnClearFlags", debugPanel.transform, "3. Xóa Toàn Bộ StoryFlag", vnFont);
            var btnPause = CreateUIButton("BtnTogglePause", debugPanel.transform, "4. Đổi Trạng Thái Gameplay / Pause", vnFont);

            var debugCtrl = rootObj.AddComponent<RuntimeDebugOverlay>();
            var debugSO = new SerializedObject(debugCtrl);
            debugSO.FindProperty("panelRoot").objectReferenceValue = debugPanel;
            debugSO.FindProperty("statusText").objectReferenceValue = statusText;
            debugSO.FindProperty("btnTestToast").objectReferenceValue = btnToast;
            debugSO.FindProperty("btnToggleTestFlag").objectReferenceValue = btnFlag;
            debugSO.FindProperty("btnClearFlags").objectReferenceValue = btnClear;
            debugSO.FindProperty("btnTogglePause").objectReferenceValue = btnPause;
            debugSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("gameStateManager").objectReferenceValue = stateMgr;
            rootSO.FindProperty("storyFlagManager").objectReferenceValue = flagMgr;
            rootSO.FindProperty("localization").objectReferenceValue = loc;
            rootSO.FindProperty("sceneLoader").objectReferenceValue = sceneLoader;
            rootSO.FindProperty("toastUI").objectReferenceValue = toastUI;
            rootSO.FindProperty("debugOverlay").objectReferenceValue = debugCtrl;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(rootObj, GameRootPrefabPath);
            UnityEngine.Object.DestroyImmediate(rootObj);
            Debug.Log("[MV] Đã tạo/cập nhật Prefab: " + GameRootPrefabPath);
            return savedPrefab;
        }

        private static void BuildBootstrapScene(GameObject gameRootPrefab)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PrefabUtility.InstantiatePrefab(gameRootPrefab);

            GameObject loaderGo = new GameObject("BootstrapLoader");
            var loader = loaderGo.AddComponent<BootstrapLoader>();
            var so = new SerializedObject(loader);
            so.FindProperty("targetSceneName").stringValue = "MoonpetalValley_MVP";
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, BootstrapScenePath);
            Debug.Log("[MV] Đã tạo Scene: " + BootstrapScenePath);
        }

        private static void BuildWorldPlaceholderScene(GameObject gameRootPrefab, TMP_FontAsset vnFont)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.backgroundColor = new Color(0.11f, 0.16f, 0.22f, 1f);
            camGo.AddComponent<AudioListener>();

            CreateMatchingEventSystem();
            PrefabUtility.InstantiatePrefab(gameRootPrefab);

            GameObject canvasGo = CreateUICanvas("Step1VerificationCanvas", null, 10);
            GameObject panel = CreateUIPanel("BannerPanel", canvasGo.transform,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(920f, 180f),
                new Color(0.13f, 0.20f, 0.28f, 0.92f));
            CreateTMPText("BannerText", panel.transform,
                "Thung Lũng Hoa Ánh Trăng — Nền tảng Step 1 sẵn sàng!\nBấm phím F1 để mở bảng Debug kiểm thử tiếng Việt & StoryFlag.",
                vnFont, 28, TextAlignmentOptions.Center);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, WorldScenePath);
            Debug.Log("[MV] Đã tạo Scene: " + WorldScenePath);
        }

        private static void CreateMatchingEventSystem()
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Type inputSysModuleType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSysModuleType != null)
            {
                esGo.AddComponent(inputSysModuleType);
                return;
            }
#endif
            esGo.AddComponent<StandaloneInputModule>();
        }

        private static void ConfigureBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(WorldScenePath, true)
            };
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[MV] Đã cập nhật Build Settings: [0] Bootstrap, [1] MoonpetalValley_MVP.");
        }

        private static GameObject CreateUICanvas(string name, Transform parent, int sortOrder)
        {
            GameObject go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        private static GameObject CreateUIPanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Color bgColor)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = sizeDelta;
            rect.anchoredPosition = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = bgColor;
            return go;
        }

        private static TMP_Text CreateTMPText(string name, Transform parent, string text, TMP_FontAsset font, float fontSize, TextAlignmentOptions align)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(12f, 8f);
            rect.offsetMax = new Vector2(-12f, -8f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = Color.white;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateUIButton(string name, Transform parent, string label, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 38f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.22f, 0.36f, 0.52f, 1f);

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            CreateTMPText("Label", go.transform, label, font, 17, TextAlignmentOptions.Center);
            return btn;
        }

        private static bool PropertyArrayContainsString(SerializedProperty arrayProp, string target)
        {
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                if (arrayProp.GetArrayElementAtIndex(i).stringValue == target) return true;
            }
            return false;
        }
    }
}
''',
}

def deploy():
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_root = PROJECT_ROOT / "_deploy_backup" / f"Step_1_fix_1_{timestamp}"
    backed_up_count = 0

    print("=== MOONPETAL VALLEY — DEPLOY STEP 1.FIX 1 ===")
    for rel_path, content in FILES.items():
        target_path = PROJECT_ROOT / rel_path
        target_path.parent.mkdir(parents=True, exist_ok=True)

        status = "TẠO MỚI"
        if target_path.exists():
            backup_target = backup_root / rel_path
            backup_target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(target_path, backup_target)
            backed_up_count += 1
            status = "GHI ĐÈ "

        normalized = content.replace("\r\n", "\n")
        with open(target_path, "w", encoding="utf-8", newline="\n") as f:
            f.write(normalized)

        size_bytes = target_path.stat().st_size
        print(f"  [{status}] {rel_path} ({size_bytes} bytes)")

        if rel_path.endswith(".cs"):
            open_braces = normalized.count("{")
            close_braces = normalized.count("}")
            if open_braces != close_braces:
                print(f"  [CẢNH BÁO] Lệch dấu ngoặc trong {rel_path}: '{{'={open_braces}, '}}'={close_braces}")

    if backed_up_count > 0:
        print(f"[BACKUP] Đã sao lưu {backed_up_count} file cũ tại: {backup_root}")

    print("\n--- HƯỚNG DẪN TIẾP THEO ---")
    print("1. Quay lại Unity Editor, chờ compile xong (kiểm tra Console sạch 0 lỗi, 0 warning từ code dự án).")
    print("2. Chạy menu: Moonpetal/Step 1/1. Build Foundation & Scenes.")
    print("3. Chạy menu: Moonpetal/Step 1/Validate rồi bấm Play để kiểm thử.")

if __name__ == "__main__":
    deploy()