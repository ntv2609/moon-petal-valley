using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
namespace MoonpetalValley.Chapter3
{
    public enum Chapter3Phase {
        Locked = 0,
        PrepDay1 = 1,
        PrepDay2 = 2,
        PrepDay3 = 3,
        NightMarketActive = 4,
        Climax = 5,
        Completed = 6
    }
    public class Chapter3StateManager : MonoBehaviour, ISaveable, IResettable {
        public const string FlagCh2Completed = "flag_ch2_completed";
        public const string FlagCh3Started = "flag_ch3_started";
        public const string FlagCh3HanaEchoDone = "flag_ch3_hana_echo_done";
        public const string FlagCh3WenExhaustedNight2 = "flag_ch3_wen_exhausted_night2";
        public const string FlagCh3StallsBuiltDay3 = "flag_ch3_stalls_built_day3";
        public const string FlagCh3MarketDone = "flag_ch3_market_done";
        public const string FlagCh3WarmthBonus = "flag_ch3_warmth_bonus";
        public const string FlagCh3ClimaxDone = "flag_ch3_climax_done";
        public const string FlagCh3Completed = "flag_ch3_completed";
        public static Chapter3StateManager Instance { get; private set; }
        [SerializeField] private NightMarketConfigSO config;
        public Chapter3Phase Phase { get; private set; } = Chapter3Phase.Locked;
        public int PrepStartDay { get; private set; } = -1;
        public int TotalCraftedItems { get; private set; }
        public float WarmthScore { get; private set; }
        public bool IsNightMarketMusicActive { get; private set; }
        private readonly HashSet<string> _collectedKeepsakes = new HashSet<string>();
        private readonly HashSet<string> _craftedRecipeIds = new HashSet<string>();
        public event Action<Chapter3Phase> OnPhaseChanged;
        public event Action<string> OnKeepsakeCollected;
        public event Action OnChapter3DataChanged;
        public NightMarketConfigSO Config {
            get {
                if (config == null)
                    config = Resources.Load<NightMarketConfigSO>("Moonpetal/Chapter3/NightMarketConfig");
                return config;
            }
        }
        public string SaveKey => "chapter3_state";
        public int CollectedKeepsakeCount => _collectedKeepsakes.Count;
        public int DistinctRecipesCraftedCount => _craftedRecipeIds.Count;
        public int CurrentPrepDayNumber {
            get {
                if (PrepStartDay <= 0 || GameRoot.Instance == null || GameRoot.Instance.Time == null)
                    return Phase == Chapter3Phase.Locked ? 0 : 1;
                return Mathf.Clamp((GameRoot.Instance.Time.CurrentDay - PrepStartDay) + 1, 1, 3);
            }
        }
        [Serializable]
        private class Chapter3SaveData {
            public int phase;
            public int prepStartDay;
            public int totalCraftedItems;
            public float warmthScore;
            public List<string> collectedKeepsakes = new List<string>();
            public List<string> craftedRecipeIds = new List<string>();
        }
        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        private void Start() {
            if (GameRoot.Instance != null)
                GameRoot.Instance.RegisterSaveable(this);
            SyncFromFlagsAndDay();
        }
        private void OnEnable() {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnDayStarted += HandleDayStarted;
            GameEventBus.OnSaveLoaded += SyncFromFlagsAndDay;
        }
        private void OnDisable() {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnDayStarted -= HandleDayStarted;
            GameEventBus.OnSaveLoaded -= SyncFromFlagsAndDay;
        }
        private void OnDestroy() {
            if (GameRoot.Instance != null)
                GameRoot.Instance.UnregisterSaveable(this);
            if (Instance == this) Instance = null;
        }
        private void HandleFlagChanged(string flagId, bool value) {
            if (flagId == FlagCh2Completed || flagId == FlagCh3Started ||
                flagId == FlagCh3StallsBuiltDay3 || flagId == FlagCh3MarketDone ||
                flagId == FlagCh3ClimaxDone || flagId == FlagCh3Completed) {
                SyncFromFlagsAndDay();
            }
        }
        private void HandleDayStarted(int newDay) {
            SyncFromFlagsAndDay();
        }
        public void SyncFromFlagsAndDay() {
            StoryFlagManager f = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (f == null) return;
            if (!f.HasFlag(FlagCh2Completed)) {
                SetPhaseInternal(Chapter3Phase.Locked);
                return;
            }
            if (PrepStartDay <= 0 && GameRoot.Instance.Time != null) { PrepStartDay = GameRoot.Instance.Time.CurrentDay; }
            if (f.HasFlag(FlagCh3Completed)) {
                SetPhaseInternal(Chapter3Phase.Completed);
                return;
            }
            if (f.HasFlag(FlagCh3ClimaxDone)) {
                SetPhaseInternal(Chapter3Phase.Climax);
                return;
            }
            if (f.HasFlag(FlagCh3MarketDone)) {
                SetPhaseInternal(Chapter3Phase.NightMarketActive);
                return;
            }
            int dayNum = CurrentPrepDayNumber;
            if (f.HasFlag(FlagCh3StallsBuiltDay3) || dayNum >= 3)
                SetPhaseInternal(Chapter3Phase.PrepDay3);
            else if (dayNum == 2)
                SetPhaseInternal(Chapter3Phase.PrepDay2);
            else
                SetPhaseInternal(Chapter3Phase.PrepDay1);
        }
        private void SetPhaseInternal(Chapter3Phase next) {
            if (Phase == next) return;
            Phase = next;
            OnPhaseChanged?.Invoke(Phase);
            OnChapter3DataChanged?.Invoke();
        }
        public void SetPhase(Chapter3Phase next) {
            SetPhaseInternal(next);
        }
        public void SetPrepStartDay(int day) {
            PrepStartDay = Mathf.Max(1, day);
            OnChapter3DataChanged?.Invoke();
        }
        public void SetNightMarketMusicActive(bool active) {
            IsNightMarketMusicActive = active;
        }
        public void SetWarmthScore(float score) {
            WarmthScore = Mathf.Clamp(score, 0f, 100f);
            OnChapter3DataChanged?.Invoke();
        }
        public void RecordCraftedRecipe(string recipeId) {
            TotalCraftedItems++;
            if (!string.IsNullOrEmpty(recipeId))
                _craftedRecipeIds.Add(recipeId);
            OnChapter3DataChanged?.Invoke();
            GameRoot.Instance?.Quests?.RecomputeObjective();
        }
        public bool HasKeepsake(string keepsakeId) {
            return !string.IsNullOrEmpty(keepsakeId) && _collectedKeepsakes.Contains(keepsakeId);
        }
        public bool CollectKeepsake(string keepsakeId, bool showToast = true) {
            if (string.IsNullOrEmpty(keepsakeId)) return false;
            if (!_collectedKeepsakes.Add(keepsakeId)) return false;
            KeepsakeDef def = Config != null ? Config.GetKeepsakeById(keepsakeId) : null;
            if (showToast) {
                string name = def != null ? def.displayName : keepsakeId;
                GameEventBus.RaiseToastRequested("Đã lưu vào Sổ Kỷ Vật: " + name + " (" + _collectedKeepsakes.Count + "/7)");
            }
            OnKeepsakeCollected?.Invoke(keepsakeId);
            OnChapter3DataChanged?.Invoke();
            GameRoot.Instance?.Quests?.RecomputeObjective();
            return true;
        }
        public IReadOnlyCollection<string> GetAllCollectedKeepsakes() => _collectedKeepsakes;
        public string CaptureStateJson() {
            var data = new Chapter3SaveData {
                phase = (int)Phase,
                prepStartDay = PrepStartDay,
                totalCraftedItems = TotalCraftedItems,
                warmthScore = WarmthScore,
                collectedKeepsakes = new List<string>(_collectedKeepsakes),
                craftedRecipeIds = new List<string>(_craftedRecipeIds)
            };
            return JsonUtility.ToJson(data);
        }
        public void RestoreStateJson(string json) {
            _collectedKeepsakes.Clear();
            _craftedRecipeIds.Clear();
            IsNightMarketMusicActive = false;
            if (string.IsNullOrEmpty(json)) {
                Phase = Chapter3Phase.Locked;
                PrepStartDay = -1;
                TotalCraftedItems = 0;
                WarmthScore = 0f;
                SyncFromFlagsAndDay();
                return;
            }
            var data = JsonUtility.FromJson<Chapter3SaveData>(json);
            if (data != null) {
                Phase = (Chapter3Phase)Mathf.Clamp(data.phase, 0, 6);
                PrepStartDay = data.prepStartDay;
                TotalCraftedItems = Mathf.Max(0, data.totalCraftedItems);
                WarmthScore = Mathf.Clamp(data.warmthScore, 0f, 100f);
                if (data.collectedKeepsakes != null) {
                    for (int i = 0; i < data.collectedKeepsakes.Count; i++)
                        if (!string.IsNullOrEmpty(data.collectedKeepsakes[i]))
                            _collectedKeepsakes.Add(data.collectedKeepsakes[i]);
                }
                if (data.craftedRecipeIds != null) {
                    for (int i = 0; i < data.craftedRecipeIds.Count; i++)
                        if (!string.IsNullOrEmpty(data.craftedRecipeIds[i]))
                            _craftedRecipeIds.Add(data.craftedRecipeIds[i]);
                }
            }
            SyncFromFlagsAndDay();
            OnChapter3DataChanged?.Invoke();
        }
        public void ResetForNewGame() {
            _collectedKeepsakes.Clear();
            _craftedRecipeIds.Clear();
            Phase = Chapter3Phase.Locked;
            PrepStartDay = -1;
            TotalCraftedItems = 0;
            WarmthScore = 0f;
            IsNightMarketMusicActive = false;
            OnPhaseChanged?.Invoke(Phase);
            OnChapter3DataChanged?.Invoke();
        }
    }
}
