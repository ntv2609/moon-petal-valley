# deploy.py — Moonpetal Valley — Step 3
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
    # =========================================================================
    # 1. TIME OF DAY MANAGER (06:00 -> 02:00, ISAVEABLE)
    # =========================================================================
    "Assets/Scripts/TimeSystem/TimeOfDayManager.cs": r'''using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.TimeSystem
{
    public enum DayPhase
    {
        Morning,
        Noon,
        Dusk,
        Night
    }

    public class TimeOfDayManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private TimeConfigSO timeConfig;

        public static event Action OnLateNightPassOut;

        public int CurrentDay { get; private set; } = 1;
        public float CurrentHourFloat { get; private set; } = 6f;

        private int _lastBroadcastTenMinBucket = -1;
        private bool _passOutTriggeredToday;

        public string SaveKey => "time_of_day";

        public DayPhase CurrentPhase
        {
            get
            {
                float dusk = timeConfig != null ? timeConfig.duskStartHour : 18f;
                float night = timeConfig != null ? timeConfig.nightStartHour : 20f;
                if (CurrentHourFloat < 12f) return DayPhase.Morning;
                if (CurrentHourFloat < dusk) return DayPhase.Noon;
                if (CurrentHourFloat < night) return DayPhase.Dusk;
                return DayPhase.Night;
            }
        }

        public bool IsDuskOrNight => CurrentPhase == DayPhase.Dusk || CurrentPhase == DayPhase.Night;
        public bool IsNight => CurrentPhase == DayPhase.Night;

        [Serializable]
        private class TimeSaveData
        {
            public int day;
            public float hourFloat;
        }

        private void Start()
        {
            CurrentHourFloat = timeConfig != null ? timeConfig.dayStartHour : 6f;
            BroadcastTimeImmediate();
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsTimePaused)
            {
                return;
            }

            float dayStart = timeConfig != null ? timeConfig.dayStartHour : 6f;
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            float realSec = timeConfig != null && timeConfig.realSecondsPerGameDay > 1f
                ? timeConfig.realSecondsPerGameDay
                : 300f;

            float gameHoursPerRealSecond = (passOut - dayStart) / realSec;
            CurrentHourFloat += Time.deltaTime * gameHoursPerRealSecond;

            int bucket = Mathf.FloorToInt(CurrentHourFloat * 6f);
            if (bucket != _lastBroadcastTenMinBucket)
            {
                _lastBroadcastTenMinBucket = bucket;
                GameEventBus.RaiseTimeChanged(CurrentDay, CurrentHourFloat);
            }

            if (CurrentHourFloat >= passOut && !_passOutTriggeredToday)
            {
                _passOutTriggeredToday = true;
                if (OnLateNightPassOut != null)
                {
                    OnLateNightPassOut.Invoke();
                }
                else
                {
                    GameEventBus.RaiseToastRequested("Đã 02:00 đêm! Bạn kiệt sức và thiếp đi tới sáng hôm sau...");
                    AdvanceToNextDay();
                }
            }
        }

        public void AddHours(float deltaHours)
        {
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            CurrentHourFloat = Mathf.Clamp(CurrentHourFloat + deltaHours, 6f, passOut);
            BroadcastTimeImmediate();
        }

        public void SetHour(float targetHour)
        {
            float dayStart = timeConfig != null ? timeConfig.dayStartHour : 6f;
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            CurrentHourFloat = Mathf.Clamp(targetHour, dayStart, passOut);
            _passOutTriggeredToday = false;
            BroadcastTimeImmediate();
        }

        public void AdvanceToNextDay()
        {
            CurrentDay++;
            CurrentHourFloat = timeConfig != null ? timeConfig.dayStartHour : 6f;
            _passOutTriggeredToday = false;
            GameEventBus.RaiseDayStarted(CurrentDay);
            BroadcastTimeImmediate();
        }

        public string GetFormattedClockString()
        {
            int totalMinutes = Mathf.FloorToInt(CurrentHourFloat * 60f);
            int displayHour = (totalMinutes / 60) % 24;
            int displayMin = (totalMinutes % 60 / 10) * 10;

            string phaseText = CurrentPhase switch
            {
                DayPhase.Morning => "Sáng",
                DayPhase.Noon => "Trưa",
                DayPhase.Dusk => "Chạng vạng",
                _ => "Đêm"
            };

            return string.Format("Ngày {0} — {1:00}:{2:00} ({3})", CurrentDay, displayHour, displayMin, phaseText);
        }

        private void BroadcastTimeImmediate()
        {
            _lastBroadcastTenMinBucket = Mathf.FloorToInt(CurrentHourFloat * 6f);
            GameEventBus.RaiseTimeChanged(CurrentDay, CurrentHourFloat);
        }

        public string CaptureStateJson()
        {
            var data = new TimeSaveData { day = CurrentDay, hourFloat = CurrentHourFloat };
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<TimeSaveData>(json);
            if (data != null)
            {
                CurrentDay = Mathf.Max(1, data.day);
                CurrentHourFloat = Mathf.Clamp(data.hourFloat, 6f, 25.9f);
                _passOutTriggeredToday = false;
                BroadcastTimeImmediate();
            }
        }
    }
}
''',

    # =========================================================================
    # 2. DAY / NIGHT AMBIENT LIGHTING & LUMI NIGHT GLOW
    # =========================================================================
    "Assets/Scripts/TimeSystem/DayNightLightingController.cs": r'''using UnityEngine;
using UnityEngine.UI;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.TimeSystem
{
    public class DayNightLightingController : MonoBehaviour
    {
        [SerializeField] private TimeConfigSO timeConfig;
        [SerializeField] private Image ambientTintImage;

        private Color _targetTint = new Color(1f, 0.94f, 0.78f, 0.06f);
        private float _targetLumiScale = 1.0f;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
            {
                HandleTimeChanged(GameRoot.Instance.Time.CurrentDay, GameRoot.Instance.Time.CurrentHourFloat);
            }
        }

        private void Update()
        {
            if (ambientTintImage != null)
            {
                ambientTintImage.color = Color.Lerp(ambientTintImage.color, _targetTint, Time.unscaledDeltaTime * 3.0f);
            }
        }

        private void HandleTimeChanged(int day, float hour)
        {
            float dusk = timeConfig != null ? timeConfig.duskStartHour : 18f;
            float night = timeConfig != null ? timeConfig.nightStartHour : 20f;

            Color morningTint = new Color(1.0f, 0.92f, 0.72f, 0.08f);
            Color noonTint = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            Color duskTint = new Color(0.88f, 0.42f, 0.16f, 0.24f);
            Color nightTint = new Color(0.06f, 0.09f, 0.24f, 0.50f);

            if (hour < 11f)
            {
                float t = Mathf.InverseLerp(6f, 11f, hour);
                _targetTint = Color.Lerp(morningTint, noonTint, t);
            }
            else if (hour < dusk)
            {
                float t = Mathf.InverseLerp(16f, dusk, hour);
                _targetTint = Color.Lerp(noonTint, duskTint, t);
            }
            else if (hour < night)
            {
                float t = Mathf.InverseLerp(dusk, night, hour);
                _targetTint = Color.Lerp(duskTint, nightTint, t);
            }
            else
            {
                _targetTint = nightTint;
            }

            bool upgraded = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                            GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            float nightScale = upgraded ? 3.2f : 2.0f;
            _targetLumiScale = hour >= dusk ? nightScale : 1.0f;

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null)
            {
                lumi.SetHaloRadiusScale(_targetLumiScale);
            }
        }
    }
}
''',

    # =========================================================================
    # 3. VILLAGE LANTERN (LIGHTS UP AT DUSK & NIGHT)
    # =========================================================================
    "Assets/Scripts/TimeSystem/VillageLantern.cs": r'''using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.TimeSystem
{
    public class VillageLantern : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer glowRenderer;

        private float _targetAlpha;
        private float _flickerPhase;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
        }

        private void Start()
        {
            _flickerPhase = Random.Range(0f, 6.28f);
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
            {
                HandleTimeChanged(GameRoot.Instance.Time.CurrentDay, GameRoot.Instance.Time.CurrentHourFloat);
            }
        }

        private void HandleTimeChanged(int day, float hour)
        {
            _targetAlpha = hour >= 18f ? 0.78f : 0f;
        }

        private void Update()
        {
            if (glowRenderer == null) return;
            _flickerPhase += Time.deltaTime * 4f;
            float flicker = _targetAlpha > 0.01f ? (Mathf.Sin(_flickerPhase) * 0.06f) : 0f;
            Color c = glowRenderer.color;
            c.a = Mathf.Lerp(c.a, Mathf.Clamp01(_targetAlpha + flicker), Time.deltaTime * 4f);
            glowRenderer.color = c;
        }
    }
}
''',

    # =========================================================================
    # 4. CLOCK HUD VIEW (TOP-RIGHT TIME DISPLAY)
    # =========================================================================
    "Assets/Scripts/UI/ClockHUDView.cs": r'''using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.UI
{
    public class ClockHUDView : MonoBehaviour
    {
        [SerializeField] private TMP_Text clockText;
        [SerializeField] private Image phaseBadgeImage;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
            GameEventBus.OnDayStarted += HandleDayStarted;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
            GameEventBus.OnDayStarted -= HandleDayStarted;
        }

        private void Start()
        {
            RefreshDisplay();
        }

        private void HandleTimeChanged(int day, float hour) => RefreshDisplay();
        private void HandleDayStarted(int day) => RefreshDisplay();

        public void RefreshDisplay()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Time == null) return;
            var tm = GameRoot.Instance.Time;

            if (clockText != null)
            {
                clockText.text = tm.GetFormattedClockString();
            }

            if (phaseBadgeImage != null)
            {
                phaseBadgeImage.color = tm.CurrentPhase switch
                {
                    DayPhase.Morning => new Color(0.95f, 0.80f, 0.35f, 1f),
                    DayPhase.Noon => new Color(0.98f, 0.90f, 0.42f, 1f),
                    DayPhase.Dusk => new Color(0.92f, 0.48f, 0.24f, 1f),
                    _ => new Color(0.36f, 0.50f, 0.88f, 1f)
                };
            }
        }
    }
}
''',

    # =========================================================================
    # 5. UPDATED GAMEROOT.CS (FULL FILE — ADDS TIMEMANAGER)
    # =========================================================================
    "Assets/Scripts/Core/GameRoot.cs": r'''using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;
using MoonpetalValley.TimeSystem;

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

        private readonly List<ISaveable> _saveables = new List<ISaveable>();

        public GameStateManager State => gameStateManager;
        public StoryFlagManager Flags => storyFlagManager;
        public VNLocalization Loc => localization;
        public SceneLoader Scenes => sceneLoader;
        public FloatingToastUI Toast => toastUI;
        public RuntimeDebugOverlay DebugOverlay => debugOverlay;
        public TimeOfDayManager Time => timeManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (localization != null)
            {
                localization.Initialize();
            }

            if (storyFlagManager != null) RegisterSaveable(storyFlagManager);
            if (timeManager != null) RegisterSaveable(timeManager);

            Debug.Log("[MV] GameRoot khởi tạo thành công.");
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (saveable == null) return;
            if (!_saveables.Contains(saveable))
            {
                _saveables.Add(saveable);
            }
        }

        public IReadOnlyList<ISaveable> GetSaveables()
        {
            return _saveables;
        }
    }
}
''',

    # =========================================================================
    # 6. UPDATED RUNTIMEDEBUGOVERLAY.CS (FULL FILE — ADDS 3 TIME CONTROLS)
    # =========================================================================
    "Assets/Scripts/DebugTools/RuntimeDebugOverlay.cs": r'''using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

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
        [SerializeField] private Button btnTpGreenhouse;
        [SerializeField] private Button btnTpVillage;
        [SerializeField] private Button btnTpGarden;
        [SerializeField] private Button btnTpForest;
        [SerializeField] private Button btnTimePlus2H;
        [SerializeField] private Button btnTimeSetDusk;
        [SerializeField] private Button btnTimeNextDay;

        private bool _isOpen;
        private float _refreshTimer;
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
            if (btnTpGreenhouse != null) btnTpGreenhouse.onClick.AddListener(() => TeleportTo("Spawn_Greenhouse"));
            if (btnTpVillage != null) btnTpVillage.onClick.AddListener(() => TeleportTo("Spawn_Village"));
            if (btnTpGarden != null) btnTpGarden.onClick.AddListener(() => TeleportTo("Spawn_BachGarden"));
            if (btnTpForest != null) btnTpForest.onClick.AddListener(() => TeleportTo("Spawn_ForestEdge"));

            if (btnTimePlus2H != null) btnTimePlus2H.onClick.AddListener(OnClickTimePlus2H);
            if (btnTimeSetDusk != null) btnTimeSetDusk.onClick.AddListener(OnClickTimeSetDusk);
            if (btnTimeNextDay != null) btnTimeNextDay.onClick.AddListener(OnClickTimeNextDay);
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

            if (_isOpen)
            {
                _refreshTimer += Time.unscaledDeltaTime;
                if (_refreshTimer >= 0.25f)
                {
                    _refreshTimer = 0f;
                    RefreshStatusText();
                }
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

        private void TeleportTo(string spawnId)
        {
            if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo(spawnId);
                RefreshStatusText();
            }
            else
            {
                GameEventBus.RaiseToastRequested("Chưa có WorldSpawnRegistry trong scene.");
            }
        }

        private void OnClickTimePlus2H()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Time == null) return;
            GameRoot.Instance.Time.AddHours(2f);
            GameEventBus.RaiseToastRequested("Tua thời gian +2 giờ: " + GameRoot.Instance.Time.GetFormattedClockString());
            RefreshStatusText();
        }

        private void OnClickTimeSetDusk()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Time == null) return;
            GameRoot.Instance.Time.SetHour(19.83f);
            GameEventBus.RaiseToastRequested("Đã đặt giờ Chạng vạng (19:50) chuẩn bị sang Đêm!");
            RefreshStatusText();
        }

        private void OnClickTimeNextDay()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Time == null) return;
            GameRoot.Instance.Time.AdvanceToNextDay();
            GameEventBus.RaiseToastRequested("Bắt đầu " + GameRoot.Instance.Time.GetFormattedClockString());
            RefreshStatusText();
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
            if (GameRoot.Instance.Time != null)
            {
                sb.AppendLine("Đồng hồ: " + GameRoot.Instance.Time.GetFormattedClockString());
            }

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                Vector3 p = player.transform.position;
                sb.AppendLine(string.Format("Vị trí Wen: ({0:0.0}, {1:0.0})", p.x, p.y));
            }

            sb.Append("Flags: ");
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

    # =========================================================================
    # 7. UPDATED PLACEHOLDERGENERATORUTILITY.CS (FULL FILE — ADDS LANTERN ART)
    # =========================================================================
    "Assets/Editor/PlaceholderGeneratorUtility.cs": r'''using System.IO;
using UnityEngine;
using UnityEditor;

namespace MoonpetalValley.EditorTools
{
    public static class PlaceholderGeneratorUtility
    {
        public static Sprite EnsureSprite(string assetPath, int width, int height, System.Action<Texture2D> drawAction)
        {
            if (!File.Exists(assetPath))
            {
                string dir = Path.GetDirectoryName(assetPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Point;
                Color clear = new Color(0f, 0f, 0f, 0f);
                Color[] pixels = new Color[width * height];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;
                tex.SetPixels(pixels);

                drawAction?.Invoke(tex);
                tex.Apply();

                byte[] png = tex.EncodeToPNG();
                File.WriteAllBytes(assetPath, png);
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }

            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
                if (Mathf.Abs(importer.spritePixelsPerUnit - 16f) > 0.01f) { importer.spritePixelsPerUnit = 16f; dirty = true; }
                if (importer.filterMode != FilterMode.Point) { importer.filterMode = FilterMode.Point; dirty = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
                if (dirty) importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        public static void FillRect(Texture2D tex, int x, int y, int w, int h, Color c)
        {
            for (int py = y; py < y + h; py++)
            {
                if (py < 0 || py >= tex.height) continue;
                for (int px = x; px < x + w; px++)
                {
                    if (px < 0 || px >= tex.width) continue;
                    tex.SetPixel(px, py, c);
                }
            }
        }

        public static void DrawWenSprite(Texture2D tex)
        {
            Color boots = new Color(0.32f, 0.20f, 0.12f);
            Color cloak = new Color(0.20f, 0.56f, 0.36f);
            Color cloakShade = new Color(0.14f, 0.42f, 0.27f);
            Color scarf = new Color(0.95f, 0.78f, 0.28f);
            Color skin = new Color(0.98f, 0.85f, 0.74f);
            Color hair = new Color(0.24f, 0.18f, 0.16f);
            Color eye = new Color(0.10f, 0.12f, 0.18f);
            Color sprout = new Color(0.55f, 0.92f, 0.42f);

            FillRect(tex, 5, 1, 2, 3, boots);
            FillRect(tex, 9, 1, 2, 3, boots);
            FillRect(tex, 4, 4, 8, 8, cloak);
            FillRect(tex, 4, 4, 2, 8, cloakShade);
            FillRect(tex, 4, 11, 8, 2, scarf);
            FillRect(tex, 4, 13, 8, 6, skin);
            tex.SetPixel(6, 15, eye);
            tex.SetPixel(9, 15, eye);
            FillRect(tex, 3, 18, 10, 3, hair);
            FillRect(tex, 3, 15, 1, 4, hair);
            FillRect(tex, 12, 15, 1, 4, hair);
            FillRect(tex, 7, 21, 2, 2, sprout);
            tex.SetPixel(9, 22, sprout);
        }

        public static void DrawLumiSprite(Texture2D tex)
        {
            Color outer = new Color(0.45f, 0.92f, 0.95f, 0.90f);
            Color inner = new Color(0.85f, 1.0f, 0.98f, 1.0f);
            Color eye = new Color(0.12f, 0.25f, 0.35f, 1.0f);

            FillRect(tex, 3, 2, 6, 7, outer);
            FillRect(tex, 4, 1, 4, 9, outer);
            FillRect(tex, 4, 3, 4, 5, inner);
            tex.SetPixel(4, 5, eye);
            tex.SetPixel(7, 5, eye);
            tex.SetPixel(5, 10, inner);
            tex.SetPixel(6, 10, inner);
        }

        public static void DrawRadialHalo(Texture2D tex)
        {
            Vector2 center = new Vector2(tex.width * 0.5f, tex.height * 0.5f);
            float maxR = tex.width * 0.5f;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float a = Mathf.Clamp01(1f - dist / maxR);
                    tex.SetPixel(x, y, new Color(0.55f, 0.96f, 0.92f, a * a * 0.42f));
                }
            }
        }

        public static void DrawWarmHalo(Texture2D tex)
        {
            Vector2 center = new Vector2(tex.width * 0.5f, tex.height * 0.5f);
            float maxR = tex.width * 0.5f;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float a = Mathf.Clamp01(1f - dist / maxR);
                    tex.SetPixel(x, y, new Color(1.0f, 0.80f, 0.36f, a * a * 0.65f));
                }
            }
        }

        public static void DrawLanternPostSprite(Texture2D tex)
        {
            Color wood = new Color(0.36f, 0.24f, 0.14f);
            Color iron = new Color(0.22f, 0.24f, 0.28f);
            Color lamp = new Color(1.0f, 0.86f, 0.42f);

            FillRect(tex, 7, 1, 2, 16, wood);
            FillRect(tex, 5, 1, 6, 2, wood);
            FillRect(tex, 7, 16, 6, 2, iron);
            FillRect(tex, 10, 11, 4, 5, lamp);
            FillRect(tex, 9, 15, 6, 1, iron);
            FillRect(tex, 9, 10, 6, 1, iron);
        }

        public static void DrawGrassTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.32f, 0.58f, 0.31f));
            Color light = new Color(0.40f, 0.68f, 0.36f);
            Color dark = new Color(0.26f, 0.48f, 0.26f);
            tex.SetPixel(3, 4, light); tex.SetPixel(3, 5, light);
            tex.SetPixel(11, 10, light); tex.SetPixel(11, 11, light);
            tex.SetPixel(7, 13, dark); tex.SetPixel(13, 3, dark);
        }

        public static void DrawPathTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.73f, 0.65f, 0.50f));
            Color pebble = new Color(0.62f, 0.54f, 0.41f);
            FillRect(tex, 2, 3, 3, 2, pebble);
            FillRect(tex, 10, 9, 4, 2, pebble);
            FillRect(tex, 5, 12, 2, 2, new Color(0.82f, 0.76f, 0.62f));
        }

        public static void DrawGreenhouseFloorTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.60f, 0.44f, 0.28f));
            Color line = new Color(0.46f, 0.33f, 0.20f);
            FillRect(tex, 0, 0, 16, 1, line);
            FillRect(tex, 0, 8, 16, 1, line);
            FillRect(tex, 7, 1, 1, 7, line);
            FillRect(tex, 13, 9, 1, 7, line);
        }

        public static void DrawWallTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.34f, 0.26f, 0.20f));
            FillRect(tex, 2, 4, 12, 9, new Color(0.56f, 0.78f, 0.84f));
            FillRect(tex, 7, 4, 2, 9, new Color(0.34f, 0.26f, 0.20f));
            FillRect(tex, 0, 14, 16, 2, new Color(0.24f, 0.18f, 0.14f));
        }

        public static void DrawFenceTile(Texture2D tex)
        {
            DrawGrassTile(tex);
            Color wood = new Color(0.54f, 0.36f, 0.20f);
            Color woodHi = new Color(0.68f, 0.48f, 0.28f);
            FillRect(tex, 2, 2, 3, 12, wood);
            FillRect(tex, 11, 2, 3, 12, wood);
            FillRect(tex, 0, 5, 16, 2, woodHi);
            FillRect(tex, 0, 10, 16, 2, woodHi);
        }

        public static void DrawTreeTile(Texture2D tex)
        {
            DrawGrassTile(tex);
            FillRect(tex, 6, 1, 4, 5, new Color(0.38f, 0.24f, 0.14f));
            FillRect(tex, 2, 5, 12, 9, new Color(0.14f, 0.36f, 0.22f));
            FillRect(tex, 4, 7, 8, 7, new Color(0.20f, 0.48f, 0.28f));
            FillRect(tex, 5, 10, 4, 3, new Color(0.28f, 0.58f, 0.34f));
        }

        public static void DrawWaterTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.22f, 0.52f, 0.78f));
            FillRect(tex, 3, 5, 5, 1, new Color(0.62f, 0.86f, 0.98f));
            FillRect(tex, 9, 11, 4, 1, new Color(0.62f, 0.86f, 0.98f));
        }

        public static void DrawFlowerbedTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.54f, 0.54f, 0.58f));
            FillRect(tex, 2, 2, 12, 12, new Color(0.36f, 0.25f, 0.18f));
            FillRect(tex, 6, 6, 4, 4, new Color(0.46f, 0.34f, 0.24f));
        }
    }
}
''',

    # =========================================================================
    # 8. EDITOR — AUTOBUILDER STEP 3
    # =========================================================================
    "Assets/Editor/AutoBuilder_Step3.cs": r'''using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step3
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string TimeConfigPath = "Assets/Data/Config/TimeConfig.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 3/1. Build Time & Day-Night Lighting")]
        public static void BuildStep3()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 3 ===");

            Sprite lanternSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_lantern.png", 16, 24, PlaceholderGeneratorUtility.DrawLanternPostSprite);
            Sprite warmHaloSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_warm_halo.png", 32, 32, PlaceholderGeneratorUtility.DrawWarmHalo);

            UpgradeGameRootWithTimeAndLighting();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildVillageLanterns(lanternSpr, warmHaloSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 3! Chạy 'Moonpetal/Step 3/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 3/Validate")]
        public static void ValidateStep3()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 3 ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab");
                errors++;
            }
            else
            {
                var root = prefab.GetComponent<GameRoot>();
                var rootSO = new SerializedObject(root);
                if (rootSO.FindProperty("timeManager").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu tham chiếu timeManager.");
                    errors++;
                }

                var tm = prefab.GetComponent<TimeOfDayManager>();
                if (tm == null || new SerializedObject(tm).FindProperty("timeConfig").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] TimeOfDayManager thiếu TimeConfigSO.");
                    errors++;
                }

                var lighting = prefab.GetComponentInChildren<DayNightLightingController>(true);
                if (lighting == null || new SerializedObject(lighting).FindProperty("ambientTintImage").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] DayNightLightingController thiếu ambientTintImage.");
                    errors++;
                }

                var clock = prefab.GetComponentInChildren<ClockHUDView>(true);
                if (clock == null || new SerializedObject(clock).FindProperty("clockText").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] ClockHUDView thiếu clockText.");
                    errors++;
                }
            }

            var lanterns = UnityEngine.Object.FindObjectsByType<VillageLantern>(FindObjectsInactive.Include);
            if (lanterns.Length < 4)
            {
                Debug.LogError("[MV] Thiếu đèn lồng làng (cần 4, hiện có " + lanterns.Length + ").");
                errors++;
            }

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
                Debug.Log("[MV] VALIDATE STEP 3: PASS (0 lỗi). Chu kỳ Ngày/Đêm, Đồng hồ HUD và Đèn lồng làng sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 3: FAIL (" + errors + " lỗi).");
            }
        }

        private static void UpgradeGameRootWithTimeAndLighting()
        {
            TimeConfigSO timeConfig = AssetDatabase.LoadAssetAtPath<TimeConfigSO>(TimeConfigPath);
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();

            // 1. TimeOfDayManager
            var tm = root.GetComponent<TimeOfDayManager>();
            if (tm == null) tm = root.AddComponent<TimeOfDayManager>();
            var tmSO = new SerializedObject(tm);
            tmSO.FindProperty("timeConfig").objectReferenceValue = timeConfig;
            tmSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("timeManager").objectReferenceValue = tm;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // 2. DayNightOverlayCanvas (SortingOrder = 5, raycastTarget = false)
            Transform oldLighting = root.transform.Find("DayNightOverlayCanvas");
            if (oldLighting != null) UnityEngine.Object.DestroyImmediate(oldLighting.gameObject);

            GameObject tintCanvasGo = new GameObject("DayNightOverlayCanvas");
            tintCanvasGo.transform.SetParent(root.transform, false);
            var tintCanvas = tintCanvasGo.AddComponent<Canvas>();
            tintCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            tintCanvas.sortingOrder = 5;

            GameObject tintImgGo = new GameObject("AmbientTintImage");
            tintImgGo.transform.SetParent(tintCanvasGo.transform, false);
            var tintRect = tintImgGo.AddComponent<RectTransform>();
            tintRect.anchorMin = Vector2.zero;
            tintRect.anchorMax = Vector2.one;
            tintRect.offsetMin = Vector2.zero;
            tintRect.offsetMax = Vector2.zero;

            var tintImg = tintImgGo.AddComponent<Image>();
            tintImg.color = new Color(1f, 0.92f, 0.72f, 0.08f);
            tintImg.raycastTarget = false;

            var lightCtrl = tintCanvasGo.AddComponent<DayNightLightingController>();
            var lightSO = new SerializedObject(lightCtrl);
            lightSO.FindProperty("timeConfig").objectReferenceValue = timeConfig;
            lightSO.FindProperty("ambientTintImage").objectReferenceValue = tintImg;
            lightSO.ApplyModifiedPropertiesWithoutUndo();

            // 3. Clock HUD (Top-Right inside SystemOverlayCanvas or dedicated HUDCanvas SortingOrder = 20)
            Transform oldClock = root.transform.Find("ClockHUDCanvas");
            if (oldClock != null) UnityEngine.Object.DestroyImmediate(oldClock.gameObject);

            GameObject hudCanvasGo = new GameObject("ClockHUDCanvas");
            hudCanvasGo.transform.SetParent(root.transform, false);
            var hudCanvas = hudCanvasGo.AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.sortingOrder = 20;
            var scaler = hudCanvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject badgePanel = new GameObject("ClockBadgePanel");
            badgePanel.transform.SetParent(hudCanvasGo.transform, false);
            var bpRect = badgePanel.AddComponent<RectTransform>();
            bpRect.anchorMin = new Vector2(1f, 1f);
            bpRect.anchorMax = new Vector2(1f, 1f);
            bpRect.pivot = new Vector2(1f, 1f);
            bpRect.anchoredPosition = new Vector2(-24f, -24f);
            bpRect.sizeDelta = new Vector2(360f, 58f);
            var bpImg = badgePanel.AddComponent<Image>();
            bpImg.color = new Color(0.09f, 0.13f, 0.20f, 0.88f);

            GameObject dotGo = new GameObject("PhaseDot");
            dotGo.transform.SetParent(badgePanel.transform, false);
            var dotRect = dotGo.AddComponent<RectTransform>();
            dotRect.anchorMin = new Vector2(0f, 0.5f);
            dotRect.anchorMax = new Vector2(0f, 0.5f);
            dotRect.anchoredPosition = new Vector2(24f, 0f);
            dotRect.sizeDelta = new Vector2(18f, 18f);
            var dotImg = dotGo.AddComponent<Image>();
            dotImg.color = new Color(0.95f, 0.80f, 0.35f, 1f);

            GameObject txtGo = new GameObject("ClockText");
            txtGo.transform.SetParent(badgePanel.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = new Vector2(46f, 4f);
            txtRect.offsetMax = new Vector2(-12f, -4f);
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "Ngày 1 — 06:00 (Sáng)";
            tmp.fontSize = 21;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = Color.white;
            if (vnFont != null) tmp.font = vnFont;

            var clockView = badgePanel.AddComponent<ClockHUDView>();
            var clockSO = new SerializedObject(clockView);
            clockSO.FindProperty("clockText").objectReferenceValue = tmp;
            clockSO.FindProperty("phaseBadgeImage").objectReferenceValue = dotImg;
            clockSO.ApplyModifiedPropertiesWithoutUndo();

            // 4. Add 3 Time Debug Buttons to DebugPanelRoot
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 650f);

                Button btnPlus2H = EnsureDebugButton(debugPanel, "BtnTimePlus2H", "Thời gian: +2 Giờ", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));
                Button btnDusk = EnsureDebugButton(debugPanel, "BtnTimeSetDusk", "Đặt 19:50 (Chạng vạng -> Đêm)", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));
                Button btnNextDay = EnsureDebugButton(debugPanel, "BtnTimeNextDay", "Qua Ngày Mới (06:00)", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));

                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnTimePlus2H").objectReferenceValue = btnPlus2H;
                dbgSO.FindProperty("btnTimeSetDusk").objectReferenceValue = btnDusk;
                dbgSO.FindProperty("btnTimeNextDay").objectReferenceValue = btnNextDay;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã nâng cấp GameRoot.prefab với TimeOfDayManager, DayNightLighting & ClockHUD.");
        }

        private static void BuildVillageLanterns(Sprite lanternSpr, Sprite warmHaloSpr)
        {
            GameObject group = GameObject.Find("VillageLanterns");
            if (group != null) UnityEngine.Object.DestroyImmediate(group);

            group = new GameObject("VillageLanterns");
            Vector2[] positions =
            {
                new Vector2(-13.5f, 4.5f),   // Trước cửa Nhà Kính
                new Vector2(-2.5f, 4.5f),    // Quảng Trường Làng Chính
                new Vector2(2.5f, -3.0f),    // Cổng Vườn Ông Bách
                new Vector2(12.5f, 7.5f)     // Đường vào Rìa Rừng
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject lanternGo = new GameObject("Lantern_" + (i + 1));
                lanternGo.transform.SetParent(group.transform, false);
                lanternGo.transform.position = new Vector3(positions[i].x, positions[i].y, 0f);

                var sr = lanternGo.AddComponent<SpriteRenderer>();
                sr.sprite = lanternSpr;
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = Mathf.RoundToInt(-positions[i].y * 100f);

                GameObject glowGo = new GameObject("WarmGlow");
                glowGo.transform.SetParent(lanternGo.transform, false);
                glowGo.transform.localPosition = new Vector3(0.15f, 0.35f, 0f);
                glowGo.transform.localScale = new Vector3(2.4f, 2.4f, 1f);
                var glowSr = glowGo.AddComponent<SpriteRenderer>();
                glowSr.sprite = warmHaloSpr;
                glowSr.sortingLayerName = "WeatherVFX";
                glowSr.sortingOrder = 15;
                glowSr.color = new Color(1f, 0.85f, 0.45f, 0f);

                var comp = lanternGo.AddComponent<VillageLantern>();
                var so = new SerializedObject(comp);
                so.FindProperty("glowRenderer").objectReferenceValue = glowSr;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static Button EnsureDebugButton(Transform parent, string name, string label, TMP_FontAsset font, Color bgColor)
        {
            Transform existing = parent.Find(name);
            if (existing != null) return existing.GetComponent<Button>();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 36f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            GameObject txtGo = new GameObject("Label");
            txtGo.transform.SetParent(go.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            if (font != null) tmp.font = font;

            return btn;
        }
    }
}
''',
}

def deploy():
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_root = PROJECT_ROOT / "_deploy_backup" / f"Step_3_{timestamp}"
    backed_up_count = 0

    print("=== MOONPETAL VALLEY — DEPLOY STEP 3 ===")
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
    print("1. Quay lại Unity Editor và chờ compile xong (0 lỗi đỏ, 0 warning).")
    print("2. Chạy menu: Moonpetal/Step 3/1. Build Time & Day-Night Lighting.")
    print("3. Chạy menu: Moonpetal/Step 3/Validate rồi bấm Play kiểm thử chu kỳ Ngày/Đêm!")

if __name__ == "__main__":
    deploy()