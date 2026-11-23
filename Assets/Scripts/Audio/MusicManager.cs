using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Audio
{
    public class MusicManager : MonoBehaviour
    {
        private const string RES_ROOT = "Moonpetal/Audio/";

        private struct ContextEntry
        {
            public string ownerId;
            public MusicContext context;
            public int priority;
            public int sequence;
        }

        public static MusicManager Instance { get; private set; }

        private MusicSettingsSO _settings;
        private MusicLibrarySO _library;
        private MusicRulesSO _rules;
        private AudioSource _deckA, _deckB, _stingerSource;
        private bool _usingDeckA = true;

        private readonly List<ContextEntry> _contextStack = new List<ContextEntry>();
        private readonly Dictionary<string, float> _duckMap = new Dictionary<string, float>();
        private readonly HashSet<string> _warnedMissingIds = new HashSet<string>();

        private int _nextSeq = 1;
        private MusicContext _activeContext = MusicContext.None;
        private MusicTrackSO _activeTrack;
        private float _deckAWeight, _deckBWeight, _deckATargetWeight, _deckBTargetWeight;
        private float _fadeSpeed = 1f, _currentDuck = 1f;
        private bool _isResting, _selfXfadeScheduled, _stingerActive;
        private float _restEndTime, _selfXfadeDuration, _stingerElapsed;
        private double _selfXfadeDspStart;
        private MusicTrackSO _activeStingerTrack;
        private float _stingerFadeStart = -1f, _stingerFadeDur = 0.5f, _lastStingerUnscaledTime = -99f;

        public MusicSettingsSO Settings => _settings;
        public MusicLibrarySO Library => _library;
        public MusicRulesSO Rules => _rules;
        public MusicContext ActiveContext => _activeContext;
        public string ActiveTrackId => _activeTrack != null ? _activeTrack.trackId : "None";
        public bool IsResting => _isResting;
        public float CurrentDuckMultiplier => _currentDuck;
        public int ContextStackCount => _contextStack.Count;
        public bool EffectiveMuted { get; private set; }
        public float EffectiveMasterScale { get; private set; }

        public static bool IsSystemEnabled()
        {
            try
            {
                if (Instance != null && Instance._settings != null) return Instance._settings.systemEnabled;
                MusicSettingsSO s = Resources.Load<MusicSettingsSO>(RES_ROOT + "MusicSettings");
                return s != null && s.systemEnabled;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[MV][Audio] IsSystemEnabled check failed: " + ex.Message);
                return false;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            try
            {
                if (Instance != null) return;
                MusicSettingsSO settings = Resources.Load<MusicSettingsSO>(RES_ROOT + "MusicSettings");
                if (settings == null)
                {
                    Debug.LogWarning("[MV][Audio] MusicSettings missing in Resources/Moonpetal/Audio/. Inactive.");
                    return;
                }
                if (!settings.systemEnabled) return;

                GameObject go = new GameObject("MV_MusicManager");
                DontDestroyOnLoad(go);
                MusicManager mgr = go.AddComponent<MusicManager>();
                mgr.Initialize(settings);
                go.AddComponent<MusicSignalProbe>();
                if (settings.enableDebugOverlay && (Application.isEditor || Debug.isDebugBuild))
                {
                    go.AddComponent<MusicDebugOverlay>();
                }
            }
            catch (Exception ex) { Debug.LogWarning("[MV][Audio] Bootstrap failed cleanly: " + ex.Message); }
        }

        private void Initialize(MusicSettingsSO settings)
        {
            Instance = this;
            _settings = settings;
            _library = Resources.Load<MusicLibrarySO>(RES_ROOT + "MusicLibrary");
            _rules = Resources.Load<MusicRulesSO>(RES_ROOT + "MusicRules");
            if (_library == null) Debug.LogWarning("[MV][Audio] MusicLibrary missing in Resources/Moonpetal/Audio/.");
            if (_rules == null) _rules = ScriptableObject.CreateInstance<MusicRulesSO>();

            _deckA = CreateAudioSource("BGM_DeckA");
            _deckB = CreateAudioSource("BGM_DeckB");
            _stingerSource = CreateAudioSource("Stinger_Channel");
        }

        private AudioSource CreateAudioSource(string childName)
        {
            GameObject child = new GameObject(childName);
            child.transform.SetParent(transform, false);
            AudioSource src = child.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;
            src.volume = 0f;
            return src;
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        public void PushContext(string ownerId, MusicContext context, int priorityOverride = -1)
        {
            if (_settings == null || !_settings.systemEnabled) return;
            if (string.IsNullOrEmpty(ownerId) || context == MusicContext.None) return;

            int prio = priorityOverride >= 0 ? priorityOverride : ResolveContextPriority(context);
            for (int i = 0; i < _contextStack.Count; i++)
            {
                if (_contextStack[i].ownerId == ownerId)
                {
                    ContextEntry e = _contextStack[i];
                    if (e.context == context && e.priority == prio) return;
                    e.context = context; e.priority = prio; e.sequence = _nextSeq++;
                    _contextStack[i] = e;
                    EvaluateTopContext();
                    return;
                }
            }
            _contextStack.Add(new ContextEntry { ownerId = ownerId, context = context, priority = prio, sequence = _nextSeq++ });
            EvaluateTopContext();
        }

        public void PopContext(string ownerId)
        {
            if (string.IsNullOrEmpty(ownerId)) return;
            bool removed = false;
            for (int i = _contextStack.Count - 1; i >= 0; i--)
            {
                if (_contextStack[i].ownerId == ownerId) { _contextStack.RemoveAt(i); removed = true; }
            }
            if (removed) EvaluateTopContext();
        }

        public void SetDucking(string key, float volumeMultiplier)
        {
            if (!string.IsNullOrEmpty(key)) _duckMap[key] = Mathf.Clamp01(volumeMultiplier);
        }

        public void ClearDucking(string key)
        {
            if (!string.IsNullOrEmpty(key)) _duckMap.Remove(key);
        }

        public void PlayStinger(StingerType stingerType)
        {
            if (_settings == null || !_settings.systemEnabled || _library == null || stingerType == StingerType.None) return;
            float cooldown = _rules != null ? _rules.stingerCooldownSeconds : 1.5f;
            if (Time.unscaledTime - _lastStingerUnscaledTime < cooldown) return;

            MusicTrackSO track = _library.GetStinger(stingerType);
            if (track == null || track.clip == null) { WarnMissingOnce("Stinger_" + stingerType); return; }

            _lastStingerUnscaledTime = Time.unscaledTime;
            _activeStingerTrack = track;
            _stingerActive = true;
            _stingerElapsed = 0f;
            _stingerFadeStart = -1f;
            _stingerFadeDur = Mathf.Max(0.15f, track.fadeOutSeconds);
            SetDucking("stinger", _rules != null ? _rules.stingerDuckFactor : 0.30f);

            _stingerSource.Stop();
            _stingerSource.clip = track.clip;
            _stingerSource.loop = false;
            _stingerSource.time = 0f;
            _stingerSource.Play();
            if (_settings.verboseLogging) Debug.Log("[MV][Audio] PlayStinger: " + track.trackId);
        }

        private int ResolveContextPriority(MusicContext ctx)
        {
            MusicTrackSO t = _library != null ? _library.GetTrackForContext(ctx) : null;
            return t != null ? t.priority : 10;
        }

        private void EvaluateTopContext()
        {
            if (_library == null) return;
            MusicContext bestContext = MusicContext.None;
            int bestPrio = int.MinValue, bestSeq = int.MinValue;
            for (int i = 0; i < _contextStack.Count; i++)
            {
                ContextEntry e = _contextStack[i];
                if (e.priority > bestPrio || (e.priority == bestPrio && e.sequence > bestSeq))
                {
                    bestPrio = e.priority; bestSeq = e.sequence; bestContext = e.context;
                }
            }
            if (bestContext == _activeContext) return;
            _activeContext = bestContext;

            if (bestContext == MusicContext.None) { FadeOutAll(); return; }
            MusicTrackSO target = _library.GetTrackForContext(bestContext);
            if (target == null || target.clip == null) { WarnMissingOnce("Context_" + bestContext); return; }
            if (_activeTrack == target && GetActiveDeck().isPlaying && !_isResting) return;
            SwitchToTrack(target, bestContext);
        }

        private void FadeOutAll()
        {
            _activeTrack = null; _isResting = false; _selfXfadeScheduled = false;
            _deckATargetWeight = 0f; _deckBTargetWeight = 0f;
            _fadeSpeed = 1f / Mathf.Max(0.1f, _rules != null ? _rules.defaultCrossfadeSeconds : 1.5f);
        }

        private void SwitchToTrack(MusicTrackSO track, MusicContext ctx)
        {
            _activeTrack = track; _isResting = false; _selfXfadeScheduled = false;
            _usingDeckA = !_usingDeckA;
            AudioSource incoming = GetActiveDeck();
            incoming.Stop();
            incoming.clip = track.clip;
            incoming.time = 0f;
            incoming.loop = (ctx == MusicContext.Title) || (track.playMode == TrackPlayMode.Loop);
            incoming.Play();

            if (_usingDeckA) { _deckAWeight = 0f; _deckATargetWeight = 1f; _deckBTargetWeight = 0f; }
            else { _deckBWeight = 0f; _deckBTargetWeight = 1f; _deckATargetWeight = 0f; }

            float dur = track.fadeInSeconds > 0.05f ? track.fadeInSeconds : (_rules != null ? _rules.defaultCrossfadeSeconds : 1.5f);
            _fadeSpeed = 1f / Mathf.Max(0.1f, dur);
            if (_settings != null && _settings.verboseLogging) Debug.Log("[MV][Audio] Track -> " + track.trackId + " (" + ctx + ")");
        }

        private AudioSource GetActiveDeck() => _usingDeckA ? _deckA : _deckB;
        private AudioSource GetInactiveDeck() => _usingDeckA ? _deckB : _deckA;

        private void Update()
        {
            if (_settings == null || !_settings.systemEnabled) return;
            float dt = Time.unscaledDeltaTime;
            bool audioMgrMuted = GameRoot.Instance != null && GameRoot.Instance.Audio != null && GameRoot.Instance.Audio.IsMuted;
            EffectiveMuted = GameSettingsManager.IsMuted || audioMgrMuted;
            EffectiveMasterScale = Mathf.Max(0f, _settings.masterVolume) * Mathf.Clamp01(GameSettingsManager.BgmVolume);

            UpdateDucking(dt);
            UpdateCrossfadeWeights(dt);
            UpdateSelfCrossfadeOrRest();
            UpdateStinger(dt);
            ApplyVolumesToSources();
        }

        private void UpdateDucking(float dt)
        {
            float targetDuck = 1f;
            foreach (KeyValuePair<string, float> kv in _duckMap)
            {
                if (kv.Value < targetDuck) targetDuck = kv.Value;
            }
            float trans = _rules != null ? _rules.duckTransitionSeconds : 0.35f;
            _currentDuck = Mathf.MoveTowards(_currentDuck, targetDuck, (1f / Mathf.Max(0.05f, trans)) * dt);
        }

        private void UpdateCrossfadeWeights(float dt)
        {
            if (_selfXfadeScheduled)
            {
                double now = AudioSettings.dspTime;
                if (now >= _selfXfadeDspStart)
                {
                    float t = Mathf.Clamp01((float)((now - _selfXfadeDspStart) / Mathf.Max(0.1f, _selfXfadeDuration)));
                    _deckAWeight = _usingDeckA ? t : (1f - t);
                    _deckBWeight = _usingDeckA ? (1f - t) : t;
                    if (t >= 1f)
                    {
                        _selfXfadeScheduled = false;
                        _deckATargetWeight = _usingDeckA ? 1f : 0f;
                        _deckBTargetWeight = _usingDeckA ? 0f : 1f;
                        AudioSource oldDeck = GetInactiveDeck();
                        if (oldDeck != null && oldDeck.isPlaying) oldDeck.Stop();
                    }
                }
                return;
            }
            _deckAWeight = Mathf.MoveTowards(_deckAWeight, _deckATargetWeight, _fadeSpeed * dt);
            _deckBWeight = Mathf.MoveTowards(_deckBWeight, _deckBTargetWeight, _fadeSpeed * dt);
            if (_deckAWeight <= 0.001f && _deckATargetWeight <= 0f && _deckA != null && _deckA.isPlaying) _deckA.Stop();
            if (_deckBWeight <= 0.001f && _deckBTargetWeight <= 0f && _deckB != null && _deckB.isPlaying) _deckB.Stop();
        }

        private void UpdateSelfCrossfadeOrRest()
        {
            if (_activeTrack == null || _activeTrack.clip == null || _activeTrack.playMode != TrackPlayMode.CrossfadeSelf) return;
            if (_isResting)
            {
                if (Time.unscaledTime >= _restEndTime) { _isResting = false; SwitchToTrack(_activeTrack, _activeContext); }
                return;
            }
            if (_selfXfadeScheduled) return;

            AudioSource cur = GetActiveDeck();
            if (cur == null || !cur.isPlaying || cur.clip == null) return;
            float rem = cur.clip.length - cur.time;

            if (_activeTrack.ambientRestMaxSeconds > 0.05f)
            {
                float fadeOut = Mathf.Max(0.5f, _activeTrack.fadeOutSeconds);
                if (rem <= fadeOut)
                {
                    _isResting = true;
                    float minR = Mathf.Max(0f, _activeTrack.ambientRestMinSeconds);
                    float maxR = Mathf.Max(minR, _activeTrack.ambientRestMaxSeconds);
                    _restEndTime = Time.unscaledTime + fadeOut + UnityEngine.Random.Range(minR, maxR);
                    _deckATargetWeight = 0f; _deckBTargetWeight = 0f; _fadeSpeed = 1f / fadeOut;
                }
            }
            else
            {
                float xfade = Mathf.Clamp(_activeTrack.selfCrossfadeSeconds, 0.5f, Mathf.Max(0.5f, cur.clip.length * 0.4f));
                if (rem <= xfade + 0.15f)
                {
                    double startDsp = AudioSettings.dspTime + Math.Max(0.05, rem - xfade);
                    _usingDeckA = !_usingDeckA;
                    AudioSource nextDeck = GetActiveDeck();
                    nextDeck.Stop(); nextDeck.clip = _activeTrack.clip; nextDeck.time = 0f; nextDeck.loop = false; nextDeck.volume = 0f;
                    nextDeck.PlayScheduled(startDsp);
                    _selfXfadeScheduled = true; _selfXfadeDspStart = startDsp; _selfXfadeDuration = xfade;
                }
            }
        }

        private void UpdateStinger(float dt)
        {
            if (!_stingerActive) return;
            _stingerElapsed += dt;
            if (_stingerSource == null || !_stingerSource.isPlaying) { FinishStinger(); return; }

            float cap = _activeStingerTrack != null ? _activeStingerTrack.maxPlaySeconds : 0f;
            if (cap > 0.1f && _stingerFadeStart < 0f && _stingerElapsed >= cap) _stingerFadeStart = _stingerElapsed;
            if (_stingerFadeStart >= 0f && (_stingerElapsed - _stingerFadeStart) >= Mathf.Max(0.1f, _stingerFadeDur))
            {
                _stingerSource.Stop();
                FinishStinger();
            }
        }

        private void FinishStinger()
        {
            _stingerActive = false; _activeStingerTrack = null; _stingerFadeStart = -1f;
            ClearDucking("stinger");
        }

        private void ApplyVolumesToSources()
        {
            float baseVol = _activeTrack != null ? _activeTrack.baseVolume : 0.85f;
            float bgmScale = baseVol * EffectiveMasterScale * _currentDuck;
            if (_deckA != null) { _deckA.mute = EffectiveMuted; _deckA.volume = Mathf.Clamp01(bgmScale * _deckAWeight); }
            if (_deckB != null) { _deckB.mute = EffectiveMuted; _deckB.volume = Mathf.Clamp01(bgmScale * _deckBWeight); }
            if (_stingerSource != null)
            {
                _stingerSource.mute = EffectiveMuted;
                float stBase = _activeStingerTrack != null ? _activeStingerTrack.baseVolume : 0.9f;
                float fadeF = _stingerFadeStart >= 0f ? 1f - Mathf.Clamp01((_stingerElapsed - _stingerFadeStart) / Mathf.Max(0.1f, _stingerFadeDur)) : 1f;
                _stingerSource.volume = Mathf.Clamp01(stBase * EffectiveMasterScale * fadeF);
            }
        }

        public float GetActiveClipTime() { AudioSource s = GetActiveDeck(); return (s != null && s.isPlaying) ? s.time : 0f; }
        public float GetActiveClipLength() { AudioSource s = GetActiveDeck(); return (s != null && s.clip != null) ? s.clip.length : 0f; }
        private void WarnMissingOnce(string key) { if (_warnedMissingIds.Add(key)) Debug.LogWarning("[MV][Audio] Missing clip/track: " + key); }
    }
}
