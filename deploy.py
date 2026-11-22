# deploy.py — Moonpetal Valley — Step 10
import os
import sys
import math
import wave
import struct
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
    # =========================================================================
    # 1. SAVE / LOAD MANAGER (JSON PERSISTENCE FOR ALL ISAVEABLES)
    # =========================================================================
    "Assets/Scripts/Core/SaveLoadManager.cs": r'''using System;
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
''',

    # =========================================================================
    # 2. AUDIO MANAGER (BGM & PROCEDURAL SFX)
    # =========================================================================
    "Assets/Scripts/Audio/AudioManager.cs": r'''using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip bgmCozyClip;
        [SerializeField] private AudioClip sfxUiClick;
        [SerializeField] private AudioClip sfxTool;
        [SerializeField] private AudioClip sfxSlash;
        [SerializeField] private AudioClip sfxHit;
        [SerializeField] private AudioClip sfxPickup;
        [SerializeField] private AudioClip sfxBloom;

        public bool IsMuted { get; private set; }

        private void OnEnable()
        {
            GameEventBus.OnItemChanged += HandleItemChanged;
            GameEventBus.OnEnemyDefeated += HandleEnemyDefeated;
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnStaminaChanged += HandleStaminaChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnItemChanged -= HandleItemChanged;
            GameEventBus.OnEnemyDefeated -= HandleEnemyDefeated;
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnStaminaChanged -= HandleStaminaChanged;
        }

        private int _lastStamina = 100;

        private void Start()
        {
            if (bgmSource != null && bgmCozyClip != null)
            {
                bgmSource.clip = bgmCozyClip;
                bgmSource.loop = true;
                bgmSource.volume = 0.25f;
                bgmSource.Play();
            }
        }

        public void ToggleMute()
        {
            IsMuted = !IsMuted;
            if (bgmSource != null) bgmSource.mute = IsMuted;
            if (sfxSource != null) sfxSource.mute = IsMuted;
        }

        public void PlaySFX(AudioClip clip, float volume = 0.55f)
        {
            if (IsMuted || sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        public void PlayUIClick() => PlaySFX(sfxUiClick, 0.45f);

        private void HandleItemChanged(string _, int __) => PlaySFX(sfxPickup, 0.45f);
        private void HandleEnemyDefeated(string _) => PlaySFX(sfxHit, 0.65f);

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            if (flagId == "flag_nv1_completed") PlaySFX(sfxBloom, 0.80f);
            else PlaySFX(sfxUiClick, 0.40f);
        }

        private void HandleStaminaChanged(int current, int max)
        {
            if (current < _lastStamina)
            {
                int diff = _lastStamina - current;
                PlaySFX(diff == 2 ? sfxSlash : sfxTool, 0.40f);
            }
            _lastStamina = current;
        }
    }
}
''',

    # =========================================================================
    # 3. PAUSE MENU UI VIEW (KEY ESC — SAVE / LOAD / MUTE / DELETE SAVE)
    # =========================================================================
    "Assets/Scripts/UI/PauseMenuUIView.cs": r'''using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class PauseMenuUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text saveStatusText;
        [SerializeField] private TMP_Text muteButtonText;
        [SerializeField] private Button btnResume;
        [SerializeField] private Button btnSave;
        [SerializeField] private Button btnLoad;
        [SerializeField] private Button btnMute;
        [SerializeField] private Button btnDeleteSave;

        private GameInputReader _input;
        private bool _isOpen;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnResume != null) btnResume.onClick.AddListener(Close);
            if (btnSave != null) btnSave.onClick.AddListener(OnClickSave);
            if (btnLoad != null) btnLoad.onClick.AddListener(OnClickLoad);
            if (btnMute != null) btnMute.onClick.AddListener(OnClickMute);
            if (btnDeleteSave != null) btnDeleteSave.onClick.AddListener(OnClickDeleteSave);
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input == null) return;

            if (_input.PausePressed)
            {
                if (!_isOpen && GameRoot.Instance.State.IsGameplayActive) Open();
                else if (_isOpen) Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance.State.SetState(GameState.PauseMenu);
            GameRoot.Instance.Audio?.PlayUIClick();
            RefreshLabels();
        }

        public void Close()
        {
            _isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance.State.CurrentState == GameState.PauseMenu)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void OnClickSave()
        {
            GameRoot.Instance?.SaveLoad?.SaveGame(false);
            RefreshLabels();
        }

        private void OnClickLoad()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.SaveLoad != null && GameRoot.Instance.SaveLoad.LoadGame())
            {
                Close();
            }
        }

        private void OnClickMute()
        {
            GameRoot.Instance?.Audio?.ToggleMute();
            RefreshLabels();
        }

        private void OnClickDeleteSave()
        {
            GameRoot.Instance?.SaveLoad?.DeleteSaveFile();
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            if (saveStatusText != null && GameRoot.Instance?.SaveLoad != null)
            {
                var sl = GameRoot.Instance.SaveLoad;
                saveStatusText.text = sl.HasSaveFile()
                    ? "Bản lưu gần nhất: " + sl.LastSavedTimestamp
                    : "Chưa có bản lưu game (Tự động lưu mỗi khi đi ngủ)";
            }
            if (muteButtonText != null && GameRoot.Instance?.Audio != null)
            {
                muteButtonText.text = GameRoot.Instance.Audio.IsMuted ? "Âm Thanh: ĐANG TẮT" : "Âm Thanh: ĐANG BẬT";
            }
        }
    }
}
''',

    # =========================================================================
    # 4. UPDATED GAMEROOT.CS (FULL FILE — ADDS SAVELOAD & AUDIO)
    # =========================================================================
    "Assets/Scripts/Core/GameRoot.cs": r'''using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;
using MoonpetalValley.Narrative;
using MoonpetalValley.Community;
using MoonpetalValley.Audio;

namespace MoonpetalValley.Core
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }

        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private StoryFlagManager storyFlagManager;
        [SerializeField] private VNLocalization localization;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private FloatingToastUI toastUI;
        [SerializeField] private RuntimeDebugOverlay debugOverlay;
        [SerializeField] private TimeOfDayManager timeManager;
        [SerializeField] private PlayerStatsController statsController;
        [SerializeField] private SleepAndKnockoutController sleepController;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private DiaryManager diaryManager;
        [SerializeField] private QuestManager questManager;
        [SerializeField] private BundleManager bundleManager;
        [SerializeField] private SaveLoadManager saveLoadManager;
        [SerializeField] private AudioManager audioManager;

        private readonly List<ISaveable> _saveables = new List<ISaveable>();

        public GameStateManager State => gameStateManager;
        public StoryFlagManager Flags => storyFlagManager;
        public VNLocalization Loc => localization;
        public SceneLoader Scenes => sceneLoader;
        public FloatingToastUI Toast => toastUI;
        public RuntimeDebugOverlay DebugOverlay => debugOverlay;
        public TimeOfDayManager Time => timeManager;
        public PlayerStatsController Stats => statsController;
        public SleepAndKnockoutController Sleep => sleepController;
        public InventoryManager Inventory => inventoryManager;
        public DialogueManager Dialogue => dialogueManager;
        public DiaryManager Diary => diaryManager;
        public QuestManager Quests => questManager;
        public BundleManager Bundle => bundleManager;
        public SaveLoadManager SaveLoad => saveLoadManager;
        public AudioManager Audio => audioManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (localization != null) localization.Initialize();

            if (storyFlagManager != null) RegisterSaveable(storyFlagManager);
            if (timeManager != null) RegisterSaveable(timeManager);
            if (statsController != null) RegisterSaveable(statsController);
            if (inventoryManager != null) RegisterSaveable(inventoryManager);
            if (diaryManager != null) RegisterSaveable(diaryManager);
            if (bundleManager != null) RegisterSaveable(bundleManager);

            Debug.Log("[MV] GameRoot khởi tạo thành công.");
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (saveable == null) return;
            if (!_saveables.Contains(saveable)) _saveables.Add(saveable);
        }

        public IReadOnlyList<ISaveable> GetSaveables() => _saveables;
    }
}
''',

    # =========================================================================
    # 5. EDITOR — AUTOBUILDER STEP 10 & FINAL MVP QA VALIDATOR
    # =========================================================================
    "Assets/Editor/AutoBuilder_Step10.cs": r'''using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Audio;
using MoonpetalValley.UI;
using MoonpetalValley.Player;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step10
    {
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 10/1. Build Save-Load, Audio & Pause Menu")]
        public static void BuildStep10()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 10 ===");
            AssetDatabase.Refresh();
            UpgradeGameRootWithSaveAudioAndPause();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 10! Chạy 'Moonpetal/Step 10/Validate (Final MVP QA)' để kiểm toán toàn bộ MVP. ===");
        }

        [MenuItem("Moonpetal/Step 10/Validate (Final MVP QA)")]
        public static void ValidateFinalMVP()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU FINAL MVP QA VALIDATE (STEP 1 -> STEP 10) ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab");
                errors++;
            }
            else
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                string[] reqFields =
                {
                    "gameStateManager", "storyFlagManager", "localization", "sceneLoader",
                    "toastUI", "debugOverlay", "timeManager", "statsController",
                    "sleepController", "inventoryManager", "dialogueManager", "diaryManager",
                    "questManager", "bundleManager", "saveLoadManager", "audioManager"
                };
                foreach (string f in reqFields)
                {
                    if (rootSO.FindProperty(f).objectReferenceValue == null)
                    {
                        Debug.LogError("[MV] GameRoot thiếu tham chiếu: " + f);
                        errors++;
                    }
                }
                if (prefab.GetComponentInChildren<PauseMenuUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu PauseMenuUIView trong GameRoot.prefab.");
                    errors++;
                }
            }

            string[] requiredAudio =
            {
                "Assets/Audio/BGM/bgm_valley_cozy.wav",
                "Assets/Audio/SFX/sfx_ui_click.wav",
                "Assets/Audio/SFX/sfx_tool.wav",
                "Assets/Audio/SFX/sfx_slash.wav",
                "Assets/Audio/SFX/sfx_hit.wav",
                "Assets/Audio/SFX/sfx_pickup.wav",
                "Assets/Audio/SFX/sfx_bloom.wav"
            };
            foreach (string path in requiredAudio)
            {
                if (AssetDatabase.LoadAssetAtPath<AudioClip>(path) == null)
                {
                    Debug.LogError("[MV] THIẾU AudioClip: " + path);
                    errors++;
                }
            }

            if (UnityEngine.Object.FindAnyObjectByType<PlayerController2D>() == null) { Debug.LogError("[MV] Thiếu Player_Wen."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<PlayerCombatController>() == null) { Debug.LogError("[MV] Thiếu PlayerCombatController."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include).Length < 12) { Debug.LogError("[MV] Thiếu 12 FarmTile."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include).Length < 4) { Debug.LogError("[MV] Thiếu 4 NPC."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include).Length < 8) { Debug.LogError("[MV] Thiếu 8 GloomSlime."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>() == null) { Debug.LogError("[MV] Thiếu PlantingSpotController."); errors++; }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 10 (FINAL MVP QA): PASS (0 lỗi). MOONPETAL VALLEY MVP ĐÃ HOÀN THIỆN 100%!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 10 (FINAL MVP QA): FAIL (" + errors + " lỗi).");
            }
        }

        [MenuItem("Moonpetal/Debug/Open Save Folder")]
        public static void OpenSaveFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        private static void UpgradeGameRootWithSaveAudioAndPause()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var saveMgr = root.GetComponent<SaveLoadManager>() ?? root.AddComponent<SaveLoadManager>();
            var audioMgr = root.GetComponent<AudioManager>() ?? root.AddComponent<AudioManager>();

            // Tạo 2 AudioSource con (BGM & SFX)
            Transform oldAudio = root.transform.Find("AudioSources");
            if (oldAudio != null) UnityEngine.Object.DestroyImmediate(oldAudio.gameObject);
            GameObject audioHost = new GameObject("AudioSources");
            audioHost.transform.SetParent(root.transform, false);
            var bgmSrc = audioHost.AddComponent<AudioSource>();
            bgmSrc.playOnAwake = false;
            var sfxSrc = audioHost.AddComponent<AudioSource>();
            sfxSrc.playOnAwake = false;

            var aSO = new SerializedObject(audioMgr);
            aSO.FindProperty("bgmSource").objectReferenceValue = bgmSrc;
            aSO.FindProperty("sfxSource").objectReferenceValue = sfxSrc;
            aSO.FindProperty("bgmCozyClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/bgm_valley_cozy.wav");
            aSO.FindProperty("sfxUiClick").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_ui_click.wav");
            aSO.FindProperty("sfxTool").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_tool.wav");
            aSO.FindProperty("sfxSlash").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_slash.wav");
            aSO.FindProperty("sfxHit").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_hit.wav");
            aSO.FindProperty("sfxPickup").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_pickup.wav");
            aSO.FindProperty("sfxBloom").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_bloom.wav");
            aSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("saveLoadManager").objectReferenceValue = saveMgr;
            rootSO.FindProperty("audioManager").objectReferenceValue = audioMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // Pause Menu UI trong ClockHUDCanvas
            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                Transform oldPause = hudCanvas.Find("PauseMenuHost");
                if (oldPause != null) UnityEngine.Object.DestroyImmediate(oldPause.gameObject);

                GameObject host = new GameObject("PauseMenuHost");
                host.transform.SetParent(hudCanvas, false);
                var hr = host.AddComponent<RectTransform>();
                hr.anchorMin = Vector2.zero; hr.anchorMax = Vector2.one;
                hr.offsetMin = Vector2.zero; hr.offsetMax = Vector2.zero;

                GameObject panel = new GameObject("PausePanelRoot");
                panel.transform.SetParent(host.transform, false);
                var pr = panel.AddComponent<RectTransform>();
                pr.anchorMin = new Vector2(0.5f, 0.5f); pr.anchorMax = new Vector2(0.5f, 0.5f);
                pr.sizeDelta = new Vector2(460f, 420f);
                panel.AddComponent<Image>().color = new Color(0.08f, 0.11f, 0.18f, 0.96f);

                var vlg = panel.AddComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(28, 28, 22, 22);
                vlg.spacing = 12f;
                vlg.childControlWidth = true;
                vlg.childControlHeight = false;

                var title = CreateText("Title", panel.transform, "TẠM DỪNG — MOONPETAL VALLEY", vnFont, 22, new Color(1f, 0.90f, 0.54f, 1f), 34f);
                var status = CreateText("SaveStatus", panel.transform, "Chưa có bản lưu", vnFont, 15, new Color(0.80f, 0.88f, 0.96f, 1f), 28f);

                var (btnResume, _) = CreateMenuBtn("BtnResume", panel.transform, "Tiếp Tục Chơi [Esc]", new Color(0.20f, 0.54f, 0.40f, 1f), vnFont);
                var (btnSave, _) = CreateMenuBtn("BtnSave", panel.transform, "Lưu Game (JSON)", new Color(0.22f, 0.42f, 0.66f, 1f), vnFont);
                var (btnLoad, _) = CreateMenuBtn("BtnLoad", panel.transform, "Tải Game Đã Lưu", new Color(0.22f, 0.42f, 0.66f, 1f), vnFont);
                var (btnMute, muteTxt) = CreateMenuBtn("BtnMute", panel.transform, "Âm Thanh: ĐANG BẬT", new Color(0.42f, 0.36f, 0.22f, 1f), vnFont);
                var (btnDel, _) = CreateMenuBtn("BtnDeleteSave", panel.transform, "Xóa File Lưu Game", new Color(0.54f, 0.22f, 0.24f, 1f), vnFont);

                var view = host.AddComponent<PauseMenuUIView>();
                var vSO = new SerializedObject(view);
                vSO.FindProperty("panelRoot").objectReferenceValue = panel;
                vSO.FindProperty("saveStatusText").objectReferenceValue = status;
                vSO.FindProperty("muteButtonText").objectReferenceValue = muteTxt;
                vSO.FindProperty("btnResume").objectReferenceValue = btnResume;
                vSO.FindProperty("btnSave").objectReferenceValue = btnSave;
                vSO.FindProperty("btnLoad").objectReferenceValue = btnLoad;
                vSO.FindProperty("btnMute").objectReferenceValue = btnMute;
                vSO.FindProperty("btnDeleteSave").objectReferenceValue = btnDel;
                vSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static TMP_Text CreateText(string name, Transform parent, string text, TMP_FontAsset font, float size, Color color, float height)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, height);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = TextAlignmentOptions.Center; tmp.color = color;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static (Button btn, TMP_Text label) CreateMenuBtn(string name, Transform parent, string text, Color bg, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 44f);
            var img = go.AddComponent<Image>(); img.color = bg;
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;

            GameObject tGo = new GameObject("Label");
            tGo.transform.SetParent(go.transform, false);
            var r = tGo.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
            var tmp = tGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = 18; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
            if (font != null) tmp.font = font;
            return (btn, tmp);
        }
    }
}
''',
}

def write_wav(path: Path, samples, sample_rate=22050):
    path.parent.mkdir(parents=True, exist_ok=True)
    with wave.open(str(path), "w") as wf:
        wf.setnchannels(1)
        wf.setsampwidth(2)
        wf.setframerate(sample_rate)
        frames = bytearray()
        for s in samples:
            val = max(-1.0, min(1.0, s))
            frames.extend(struct.pack("<h", int(val * 32767)))
        wf.writeframes(bytes(frames))

def generate_procedural_audio():
    sr = 22050

    # 1. Cozy pentatonic music-box BGM (8s seamless loop)
    notes = [523.25, 659.25, 783.99, 659.25, 587.33, 659.25, 880.00, 783.99,
             523.25, 659.25, 783.99, 1046.50, 880.00, 783.99, 659.25, 587.33]
    note_dur = 0.5
    bgm = []
    for freq in notes:
        n_samples = int(sr * note_dur)
        for i in range(n_samples):
            t = i / sr
            env = math.exp(-t * 3.8) * min(1.0, t * 40.0)
            s = (math.sin(2 * math.pi * freq * t) * 0.55 +
                 math.sin(2 * math.pi * (freq * 0.5) * t) * 0.25) * env * 0.35
            bgm.append(s)
    write_wav(PROJECT_ROOT / "Assets/Audio/BGM/bgm_valley_cozy.wav", bgm, sr)

    # 2. UI Click (0.09s soft wood chime)
    ui = [math.sin(2 * math.pi * 660 * (i / sr)) * math.exp(-(i / sr) * 28) * 0.45 for i in range(int(sr * 0.09))]
    write_wav(PROJECT_ROOT / "Assets/Audio/SFX/sfx_ui_click.wav", ui, sr)

    # 3. Tool Thud/Chop (0.14s low pitch drop)
    tool = [math.sin(2 * math.pi * (180 - 90 * (i / (sr * 0.14))) * (i / sr)) * math.exp(-(i / sr) * 18) * 0.6 for i in range(int(sr * 0.14))]
    write_wav(PROJECT_ROOT / "Assets/Audio/SFX/sfx_tool.wav", tool, sr)

    # 4. Moon Sickle Slash (0.18s shimmering sweep)
    slash = [math.sin(2 * math.pi * (420 + 520 * (i / (sr * 0.18))) * (i / sr)) * math.exp(-(i / sr) * 14) * 0.45 for i in range(int(sr * 0.18))]
    write_wav(PROJECT_ROOT / "Assets/Audio/SFX/sfx_slash.wav", slash, sr)

    # 5. Slime Purify Pop (0.20s pop + sparkle)
    hit = [(math.sin(2 * math.pi * 240 * (i / sr)) * 0.5 + math.sin(2 * math.pi * 880 * (i / sr)) * 0.3) * math.exp(-(i / sr) * 15) * 0.6 for i in range(int(sr * 0.20))]
    write_wav(PROJECT_ROOT / "Assets/Audio/SFX/sfx_hit.wav", hit, sr)

    # 6. Pickup Chime (0.22s rising two-note)
    pickup = []
    for f in (587.33, 880.0):
        for i in range(int(sr * 0.11)):
            t = i / sr
            pickup.append(math.sin(2 * math.pi * f * t) * math.exp(-t * 16) * 0.45)
    write_wav(PROJECT_ROOT / "Assets/Audio/SFX/sfx_pickup.wav", pickup, sr)

    # 7. Moonpetal Bloom Fanfare (1.2s ascending chime chord)
    bloom = []
    for f in (523.25, 659.25, 783.99, 1046.50, 1318.51, 1567.98):
        for i in range(int(sr * 0.20)):
            t = i / sr
            bloom.append(math.sin(2 * math.pi * f * t) * math.exp(-t * 6.5) * 0.55)
    write_wav(PROJECT_ROOT / "Assets/Audio/SFX/sfx_bloom.wav", bloom, sr)
    print("  [AUDIO  ] Đã tạo 7 file .wav (1 BGM + 6 SFX) trong Assets/Audio/.")

def deploy():
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_root = PROJECT_ROOT / "_deploy_backup" / f"Step_10_{timestamp}"
    backed_up_count = 0

    print("=== MOONPETAL VALLEY — DEPLOY STEP 10 (FINAL MVP) ===")
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

    generate_procedural_audio()

    if backed_up_count > 0:
        print(f"[BACKUP] Đã sao lưu {backed_up_count} file cũ tại: {backup_root}")

    print("\n--- HƯỚNG DẪN TIẾP THEO ---")
    print("1. Quay lại Unity Editor (tắt Play Mode) và chờ compile xong (0 lỗi, 0 warning).")
    print("2. Chạy menu: Moonpetal/Step 10/1. Build Save-Load, Audio & Pause Menu.")
    print("3. Chạy menu: Moonpetal/Step 10/Validate (Final MVP QA) rồi bấm Play trải nghiệm bản MVP hoàn chỉnh!")

if __name__ == "__main__":
    deploy()