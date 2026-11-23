using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.Combat;
using MoonpetalValley.Core;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.Audio
{
    public class MusicSignalProbe : MonoBehaviour
    {
        private const string OWNER_BASE = "probe_base";
        private const string OWNER_MIST = "probe_mist";
        private const string OWNER_COMBAT = "probe_combat";
        private const string OWNER_RELAY = "probe_relay";
        private const string OWNER_CUTSCENE = "probe_cutscene";

        private MusicManager _manager;
        private PlayerController2D _player;
        private IntroCutsceneManager _introCutscene;
        private BlightFogZone[] _fogZones = Array.Empty<BlightFogZone>();

        private readonly List<Collider2D> _overlapResults = new List<Collider2D>(16);
        private ContactFilter2D _enemyFilter;
        private readonly Dictionary<string, int> _itemCounts = new Dictionary<string, int>();

        private float _nextPollTime, _combatHoldUntil = -1f, _suppressStingersUntil = -1f;
        private int _lastSeenDay = -1;
        private bool _isCutsceneState, _isDialogueState;

        public string CurrentSceneName { get; private set; } = "";
        public bool IsInMist { get; private set; }
        public bool IsInCombat { get; private set; }
        public bool IsDialogueActive { get; private set; }
        public bool IsCutsceneActive { get; private set; }

        private void Awake()
        {
            _manager = GetComponent<MusicManager>();
            _enemyFilter = new ContactFilter2D { useTriggers = true, useLayerMask = true };
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            GameEventBus.OnGameStateChanged += HandleGameStateChanged;
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnItemChanged += HandleItemChanged;
            GameEventBus.OnQuestUpdated += HandleQuestUpdated;
            GameEventBus.OnDayStarted += HandleDayStarted;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded;
            PlayerStatsController.OnPlayerKnockedOut += HandlePlayerKnockedOut;
            TimeOfDayManager.OnLateNightPassOut += HandleLateNightPassOut;
            RefreshSceneCache(SceneManager.GetActiveScene().name);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            GameEventBus.OnGameStateChanged -= HandleGameStateChanged;
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnItemChanged -= HandleItemChanged;
            GameEventBus.OnQuestUpdated -= HandleQuestUpdated;
            GameEventBus.OnDayStarted -= HandleDayStarted;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded;
            PlayerStatsController.OnPlayerKnockedOut -= HandlePlayerKnockedOut;
            TimeOfDayManager.OnLateNightPassOut -= HandleLateNightPassOut;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) => RefreshSceneCache(scene.name);

        private void RefreshSceneCache(string sceneName)
        {
            CurrentSceneName = sceneName ?? "";
            _player = FindAnyObjectByType<PlayerController2D>();
            _introCutscene = FindAnyObjectByType<IntroCutsceneManager>();
            _fogZones = FindObjectsByType<BlightFogZone>(FindObjectsInactive.Include);
            _combatHoldUntil = -1f;
            _nextPollTime = 0f;
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null && _lastSeenDay < 0)
            {
                _lastSeenDay = GameRoot.Instance.Time.CurrentDay;
            }
            PollSignals();
        }

        private void Update()
        {
            if (_manager == null || _manager.Settings == null || !_manager.Settings.systemEnabled) return;
            float interval = _manager.Rules != null ? Mathf.Max(0.25f, _manager.Rules.pollIntervalSeconds) : 0.25f;
            if (Time.unscaledTime < _nextPollTime) return;
            _nextPollTime = Time.unscaledTime + interval;
            PollSignals();
        }

        private void PollSignals()
        {
            if (_manager == null || _manager.Rules == null) return;
            MusicRulesSO rules = _manager.Rules;

            if (ContainsIgnoreCase(rules.titleSceneNames, CurrentSceneName))
            {
                IsInMist = false; IsInCombat = false; IsCutsceneActive = false;
                _manager.PopContext(OWNER_MIST); _manager.PopContext(OWNER_COMBAT); _manager.PopContext(OWNER_RELAY); _manager.PopContext(OWNER_CUTSCENE);
                _manager.ClearDucking("dialogue");
                _manager.PushContext(OWNER_BASE, MusicContext.Title);
                return;
            }

            IsCutsceneActive = _isCutsceneState || (_introCutscene != null && _introCutscene.IsPlaying);
            if (IsCutsceneActive) _manager.PushContext(OWNER_CUTSCENE, MusicContext.Cutscene_Opening);
            else _manager.PopContext(OWNER_CUTSCENE);

            IsDialogueActive = _isDialogueState || (GameRoot.Instance != null && GameRoot.Instance.Dialogue != null && GameRoot.Instance.Dialogue.IsDialogueActive);
            if (IsDialogueActive) _manager.SetDucking("dialogue", rules.dialogueDuckFactor);
            else _manager.ClearDucking("dialogue");

            bool isNight = false;
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
            {
                isNight = rules.useDuskAsNight ? GameRoot.Instance.Time.IsDuskOrNight : GameRoot.Instance.Time.IsNight;
            }
            bool isRiver = ContainsIgnoreCase(rules.riverSceneNames, CurrentSceneName);
            MusicContext baseCtx = isNight ? MusicContext.Safe_Night : MusicContext.Safe_Day;
            if (isRiver && !isNight && _manager.Library != null && _manager.Library.GetTrackForContext(MusicContext.Zone_River_Day)?.clip != null)
                baseCtx = MusicContext.Zone_River_Day;
            _manager.PushContext(OWNER_BASE, baseCtx);

            bool relayOn = Chapter2StateManager.Instance != null && Chapter2StateManager.Instance.IsRelayEventActive;
            if (relayOn && _manager.Library != null && _manager.Library.GetTrackForContext(MusicContext.Set_LanternRelay)?.clip != null)
                _manager.PushContext(OWNER_RELAY, MusicContext.Set_LanternRelay);
            else
                _manager.PopContext(OWNER_RELAY);

            if (_player == null) _player = FindAnyObjectByType<PlayerController2D>();
            if (_player == null)
            {
                IsInMist = false; IsInCombat = false;
                _manager.PopContext(OWNER_MIST); _manager.PopContext(OWNER_COMBAT);
                return;
            }

            Vector2 pos = _player.transform.position;
            IsInMist = CheckMistZone(pos, rules);
            if (IsInMist) _manager.PushContext(OWNER_MIST, MusicContext.Mist_Zone);
            else _manager.PopContext(OWNER_MIST);

            bool enemyNear = CheckCombatNearby(pos, rules);
            if (enemyNear) _combatHoldUntil = Time.unscaledTime + rules.combatHoldSeconds;
            IsInCombat = enemyNear || Time.unscaledTime < _combatHoldUntil;
            if (IsInCombat) _manager.PushContext(OWNER_COMBAT, MusicContext.Combat);
            else _manager.PopContext(OWNER_COMBAT);
        }

        private bool CheckMistZone(Vector2 playerPos, MusicRulesSO rules)
        {
            if (_fogZones == null || _fogZones.Length == 0) return false;
            float pad = rules.mistDetectPadding;
            float fallbackSqr = rules.mistFallbackRadius * rules.mistFallbackRadius;
            for (int i = 0; i < _fogZones.Length; i++)
            {
                BlightFogZone z = _fogZones[i];
                if (z == null || !z.gameObject.activeInHierarchy) continue;
                SpriteRenderer sr = z.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Bounds b = sr.bounds;
                    b.Expand(new Vector3(pad * 2f, pad * 2f, 10f));
                    if (b.Contains(new Vector3(playerPos.x, playerPos.y, b.center.z))) return true;
                }
                else if (((Vector2)z.transform.position - playerPos).sqrMagnitude <= fallbackSqr) return true;
            }
            return false;
        }

        private bool CheckCombatNearby(Vector2 playerPos, MusicRulesSO rules)
        {
            _enemyFilter.SetLayerMask(rules.enemyLayerMask);
            _overlapResults.Clear();
            int count = Physics2D.OverlapCircle(playerPos, rules.combatDetectRadius, _enemyFilter, _overlapResults);
            for (int i = 0; i < count; i++)
            {
                Collider2D col = _overlapResults[i];
                if (col == null) continue;
                GloomSlimeController slime = col.GetComponentInParent<GloomSlimeController>();
                if (slime != null && slime.IsDefeated) continue;
                ThornCrawlerController tc = col.GetComponentInParent<ThornCrawlerController>(); if (tc != null && tc.IsDefeated) continue;
                BigGloomSlimeController bs = col.GetComponentInParent<BigGloomSlimeController>(); if (bs != null && bs.IsDefeated) continue;
                LanternMothController lm = col.GetComponentInParent<LanternMothController>(); if (lm != null && lm.IsDefeated) continue;
                return true;
            }
            return false;
        }

        private bool IsStingerSuppressed() => Time.unscaledTime < _suppressStingersUntil;

        private void HandleGameStateChanged(GameState state)
        {
            _isCutsceneState = state == GameState.Cutscene;
            _isDialogueState = state == GameState.Dialogue;
            _nextPollTime = 0f;
        }

        private void HandleSaveLoaded()
        {
            float dur = (_manager != null && _manager.Rules != null) ? _manager.Rules.saveLoadSuppressSeconds : 2.5f;
            _suppressStingersUntil = Time.unscaledTime + dur;
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null) _lastSeenDay = GameRoot.Instance.Time.CurrentDay;
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value || IsStingerSuppressed() || _manager == null || _manager.Rules == null || string.IsNullOrEmpty(flagId)) return;
            if (ContainsIgnoreCase(_manager.Rules.petalFlagIds, flagId)) { _manager.PlayStinger(StingerType.PetalObtained); return; }
            if (ContainsIgnoreCase(_manager.Rules.questCompleteFlagIds, flagId)) _manager.PlayStinger(StingerType.QuestComplete);
        }

        private void HandleItemChanged(string itemId, int newTotal)
        {
            if (string.IsNullOrEmpty(itemId)) return;
            int prev = _itemCounts.TryGetValue(itemId, out int oldVal) ? oldVal : 0;
            _itemCounts[itemId] = newTotal;
            if (newTotal <= prev || IsStingerSuppressed() || _manager == null || _manager.Rules == null) return;
            List<string> kws = _manager.Rules.petalItemKeywords;
            if (kws == null) return;
            for (int i = 0; i < kws.Count; i++)
            {
                if (!string.IsNullOrEmpty(kws[i]) && itemId.IndexOf(kws[i], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _manager.PlayStinger(StingerType.PetalObtained);
                    return;
                }
            }
        }

        private void HandleQuestUpdated(string questId)
        {
            if (!IsStingerSuppressed() && _manager != null && !string.IsNullOrEmpty(questId) &&
                questId.IndexOf("complete", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                _manager.PlayStinger(StingerType.QuestComplete);
            }
        }

        private void HandleDayStarted(int day)
        {
            bool isNew = _lastSeenDay > 0 && day > _lastSeenDay;
            _lastSeenDay = day;
            if (isNew && !IsStingerSuppressed() && _manager != null) _manager.PlayStinger(StingerType.NewDay);
        }

        private void HandlePlayerKnockedOut(string reason)
        {
            if (!IsStingerSuppressed() && _manager != null) _manager.PlayStinger(StingerType.KnockedOut);
        }

        private void HandleLateNightPassOut()
        {
            if (!IsStingerSuppressed() && _manager != null) _manager.PlayStinger(StingerType.KnockedOut);
        }

        private static bool ContainsIgnoreCase(List<string> list, string value)
        {
            if (list == null || string.IsNullOrEmpty(value)) return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i], value, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
    }
}
