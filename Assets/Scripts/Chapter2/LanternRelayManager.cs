using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.Chapter2
{
    public class LanternRelayManager : MonoBehaviour
    {
        public enum RelayStep
        {
            Inactive = 0,
            PickHammerEast = 1,
            DeliverHammerWest = 2,
            DeliverNailsEast = 3,
            DefeatAlphaAndFinish = 4,
            ClimaxCutscene = 5,
            Completed = 6
        }

        public const string FlagEcho2Done = "flag_ch2_echo2_pier_done";
        public const string FlagRelayDone = "flag_ch2_relay_done";
        public const string FlagCh2Completed = "flag_ch2_completed";
        public const string ItemHammerId = "item_carpenter_hammer";
        public const string ItemNailsId = "item_iron_nails_box";

        public static LanternRelayManager Instance { get; private set; }

        [Header("Config & Prefabs")]
        [SerializeField] private RelayLanternConfigSO config;
        [SerializeField] private LanternMothController mothPrefab;
        [SerializeField] private LanternRelayHUDView hudView;
        [SerializeField] private List<BridgeLanternPost> lanterns = new List<BridgeLanternPost>();

        [Header("NPC & Bridge Positions")]
        [SerializeField] private Vector3 kaelRelayPos = new Vector3(9.4f, 2.55f, 0f);
        [SerializeField] private Vector3 maraRelayPos = new Vector3(-9.8f, 2.55f, 0f);
        [SerializeField] private Vector3 kaelCenterPos = new Vector3(1.1f, 0.35f, 0f);
        [SerializeField] private Vector3 maraCenterPos = new Vector3(-1.1f, 0.35f, 0f);

        private readonly List<LanternMothController> _activeMoths = new List<LanternMothController>();
        private RelayStep _step = RelayStep.Inactive;
        private float _siteLight = 100f;
        private float _buildProgress = 0f;
        private float _mothSpawnTimer;
        private bool _isRunning;
        private bool _introPlaying;
        private bool _gust1Triggered;
        private bool _gust2Triggered;
        private bool _alphaSpawned;
        private bool _alphaDefeated;
        private Transform _kaelTr, _maraTr;
        private Vector3 _kaelOrigPos, _maraOrigPos;

        // Step Ch2-X5: Relay Chain Combo, Overcharge Pulse & Wind Guard
        private int _relayChainCount;
        private float _relayChainTimer;
        private int _lastRelitLanternIndex = -1;
        private float _overchargeCooldown;
        private float _drainFreezeTimer;
        private bool _windGuardWindowActive;
        private bool _windGuardSucceeded;
        private float _windTelegraphRemaining;
        private Coroutine _activeGustCoroutine;

        public bool IsRelayActive => _isRunning;
        public RelayStep CurrentStep => _step;
        public float MothDrainPerSec => config != null ? config.mothDrainPerSec : 13.0f;

        private void Awake() { if (Instance != null && Instance != this) { Destroy(this); return; } Instance = this; }
        private void OnEnable() { PlayerStatsController.OnPlayerKnockedOut += HandlePlayerKnockedOut; GameEventBus.OnSaveLoaded += RefreshBridgeLanternsFromState; }
        private void OnDisable() { PlayerStatsController.OnPlayerKnockedOut -= HandlePlayerKnockedOut; GameEventBus.OnSaveLoaded -= RefreshBridgeLanternsFromState; }
        private void OnDestroy() { if (Instance == this) Instance = null; }
        private void Start() { RefreshBridgeLanternsFromState(); }

        public void RefreshBridgeLanternsFromState()
        {
            if (lanterns == null) return;
            for (int i = 0; i < lanterns.Count; i++) if (lanterns[i] != null) lanterns[i].ApplyBridgeVisibility();
        }

        public bool CanInteractEastStation()
        {
            if (_introPlaying || _step == RelayStep.ClimaxCutscene) return false;
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags == null || flags.HasFlag(FlagRelayDone) || flags.HasFlag(FlagCh2Completed)) return false;
            if (!_isRunning) return flags.HasFlag(FlagEcho2Done);
            return _step == RelayStep.PickHammerEast || _step == RelayStep.DeliverNailsEast;
        }

        public bool CanInteractWestStation() => _isRunning && !_introPlaying && _step == RelayStep.DeliverHammerWest;

        public string GetEastStationPrompt()
        {
            if (!_isRunning) return "Bắt Đầu Đêm Chuyền Đèn (Ông Kael)";
            if (_step == RelayStep.PickHammerEast) return "Nhận Búa Thợ Mộc Của Ông Kael";
            if (_step == RelayStep.DeliverNailsEast) return "Giao Hộp Đinh Đồng Cho Ông Kael";
            return "Ông Kael";
        }

        public string GetWestStationPrompt() => _step == RelayStep.DeliverHammerWest ? "Giao Búa & Nhận Hộp Đinh Từ Bà Mara" : "Bà Mara";

        public void HandleEastStationInteract()
        {
            if (_introPlaying) return;
            if (!_isRunning) { BeginRelayFromEast(); return; }
            if (_step == RelayStep.PickHammerEast)
            {
                GiveRelayItem(ItemHammerId);
                _step = RelayStep.DeliverHammerWest;
                GameEventBus.RaiseToastRequested("Pha 1: Qua Cầu Khỉ Dây Thừng mang Búa sang Bà Mara! Giữ mạch đèn & chém Bướm!");
                SpawnMothWave(2, false);
            }
            else if (_step == RelayStep.DeliverNailsEast)
            {
                RemoveRelayItem(ItemNailsId);
                _buildProgress = Mathf.Min(82f, Mathf.Max(_buildProgress + 18f, 70f));
                _step = RelayStep.DefeatAlphaAndFinish;
                if (RiverBridgeController.Instance != null) RiverBridgeController.Instance.SetBuildProgress(_buildProgress);
                GameEventBus.RaiseToastRequested("Pha 3: Đàn Bướm Đêm kéo tới! Giữ >= 4/6 đèn sáng và hạ Bướm Chúa Hút Sáng!");
                SpawnMothWave(3, false);
            }
        }

        public void HandleWestStationInteract()
        {
            if (!_isRunning || _introPlaying || _step != RelayStep.DeliverHammerWest) return;
            RemoveRelayItem(ItemHammerId);
            GiveRelayItem(ItemNailsId);
            _buildProgress = Mathf.Min(68f, Mathf.Max(_buildProgress + 18f, 36f));
            _step = RelayStep.DeliverNailsEast;
            if (RiverBridgeController.Instance != null) RiverBridgeController.Instance.SetBuildProgress(_buildProgress);
            GameEventBus.RaiseToastRequested("Pha 2: Mang Hộp Đinh Đồng về cho Ông Kael! Coi chừng Gió Sông thổi tắt đèn (bấm [Q] chắn gió)!");
            SpawnMothWave(2, false);
        }

        public void NotifyLanternRelit(BridgeLanternPost post)
        {
            if (!_isRunning || _introPlaying || post == null) return;

            // 1. Relay Chain Combo (x2..x4) when lighting distinct lanterns within 7.5s
            if (_relayChainTimer > 0f && post.LanternIndex != _lastRelitLanternIndex)
                _relayChainCount = Mathf.Min(4, _relayChainCount + 1);
            else if (_relayChainCount <= 0)
                _relayChainCount = 1;

            _lastRelitLanternIndex = post.LanternIndex;
            _relayChainTimer = 7.5f;

            if (_relayChainCount >= 2 && JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.SpawnFloatingNumber(
                    post.transform.position + Vector3.up * 1.35f,
                    $"CHUỖI ĐÈN x{_relayChainCount}!",
                    new Color(1f, 0.86f, 0.34f));
            }

            // 2. Relighting a lantern during the Wind Gust Telegraph also triggers Perfect Wind Guard!
            if (_windGuardWindowActive && !_windGuardSucceeded)
                TriggerPerfectWindGuard(post.transform.position);
        }

        private void BeginRelayFromEast()
        {
            if (Chapter2StateManager.Instance != null && !Chapter2StateManager.Instance.IsRelayStartUnlocked())
            {
                GameEventBus.RaiseToastRequested("Hãy nói chuyện với Ông Kael trước để cùng thống nhất kế hoạch Đêm Chuyền Đèn!");
                return;
            }
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null && GameRoot.Instance.Time.CurrentHourFloat < 18.0f)
            {
                GameEventBus.RaiseToastRequested("Đêm Chuyền Đèn chỉ bắt đầu khi chiều buông (sau 18:00). Hãy tới Ghế Đá Chờ Chiều Buông trước!");
                return;
            }
            StartRelayEvent();
        }

        public void StartRelayEvent()
        {
            if (_isRunning || _introPlaying) return;
            StartCoroutine(PlayRopeCatwalkIntroRoutine());
        }

        private IEnumerator PlayRopeCatwalkIntroRoutine()
        {
            _introPlaying = true;
            _isRunning = true;
            _step = RelayStep.PickHammerEast;
            _siteLight = 100f;
            _buildProgress = 0f;
            _gust1Triggered = false;
            _gust2Triggered = false;
            _alphaSpawned = false;
            _alphaDefeated = false;
            _relayChainCount = 0;
            _relayChainTimer = 0f;
            _lastRelitLanternIndex = -1;
            _overchargeCooldown = 6f;
            _drainFreezeTimer = 0f;
            _windGuardWindowActive = false;
            _windGuardSucceeded = false;
            _windTelegraphRemaining = 0f;
            _mothSpawnTimer = 2.2f;
            ClearAllMoths();

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.Cutscene);

            if (GameRoot.Instance != null && GameRoot.Instance.Time != null && GameRoot.Instance.Time.CurrentHourFloat < 18.5f)
                GameRoot.Instance.Time.SetHour(18.5f);

            PositionNPCsForRelay();

            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.SetRelayEventActive(true);
                Chapter2StateManager.Instance.SetBridgeState(RiverBridgeState.RelayInProgress);
            }
            if (RiverBridgeController.Instance != null)
            {
                RiverBridgeController.Instance.ResetBuildProgress();
                RiverBridgeController.Instance.SetBuildProgress(0f);
                RiverBridgeController.Instance.SetCatwalkReveal(0f);
            }

            GameEventBus.RaiseToastRequested("Ông Kael & Bà Mara đang kéo dây thừng bắc Cầu Khỉ Tạm qua khoảng sông đứt!");
            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.SpawnFloatingNumber(new Vector3(4.2f, 1.5f, 0f), "KÉO DÂY THỪNG!", new Color(1f, 0.88f, 0.50f));

            float dur = 1.9f;
            float t = 0f;
            bool midPulse = false;
            while (t < dur)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / dur);
                if (RiverBridgeController.Instance != null)
                    RiverBridgeController.Instance.SetCatwalkReveal(p);
                if (!midPulse && p >= 0.5f)
                {
                    midPulse = true;
                    if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                        GameRoot.Instance.Audio.PlayUIClick();
                    if (JuiceEffectsManager.Instance != null)
                        JuiceEffectsManager.Instance.TriggerCameraShake(0.10f, 0.10f);
                }
                yield return null;
            }

            if (RiverBridgeController.Instance != null)
                RiverBridgeController.Instance.SetCatwalkReveal(1f);
            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                GameRoot.Instance.Audio.PlayUIClick();
            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.TriggerCameraShake(0.14f, 0.14f);
                JuiceEffectsManager.Instance.SpawnFloatingNumber(new Vector3(0f, 1.4f, 0f), "CẦU KHỈ TẠM SẴN SÀNG!", new Color(0.62f, 0.98f, 0.78f));
            }

            for (int i = 0; i < lanterns.Count; i++) if (lanterns[i] != null) lanterns[i].SetRelayActive(true, 96f);
            if (hudView != null) hudView.SetVisible(true);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.Gameplay);

            _introPlaying = false;
            SpawnMothWave(2, false);
            GameEventBus.RaiseToastRequested("ĐÊM CHUYỀN ĐÈN BẮT ĐẦU! Chuyền đèn tạo Chuỗi x2-x4, giữ 6/6 đèn >75% kích Xung Quá Tải!");
            PushHud();
        }

        private void Update()
        {
            if (!_isRunning || _introPlaying || _step == RelayStep.ClimaxCutscene) return;
            bool active = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!active) return;

            float dt = Time.deltaTime;
            if (_overchargeCooldown > 0f) _overchargeCooldown -= dt;
            if (_drainFreezeTimer > 0f) _drainFreezeTimer = Mathf.Max(0f, _drainFreezeTimer - dt);

            if (_relayChainTimer > 0f)
            {
                _relayChainTimer -= dt;
                if (_relayChainTimer <= 0f) { _relayChainCount = 0; _lastRelitLanternIndex = -1; }
            }

            // Check [Q] Perfect Wind Guard input during wind telegraph (Input System safe)
            if (_windGuardWindowActive && !_windGuardSucceeded)
            {
                bool qPressed = Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame;
                if (qPressed)
                {
                    var pc = Object.FindAnyObjectByType<PlayerController2D>();
                    TriggerPerfectWindGuard(pc != null ? pc.transform.position : Vector3.zero);
                }
            }

            float passiveDrain = _drainFreezeTimer > 0f ? 0f : (config != null ? config.passiveDrainPerSec : 2.35f);
            int minLit = 4;
            int litCount = 0;
            int highCount = 0;

            for (int i = 0; i < lanterns.Count; i++)
            {
                if (lanterns[i] == null) continue;
                lanterns[i].TickPassiveDrain(passiveDrain * dt);
                if (lanterns[i].IsLit) litCount++;
                if (lanterns[i].CurrentLight >= 75f) highCount++;
            }

            // 6/6 >75% Overcharge Pulse!
            if (lanterns.Count >= 6 && highCount >= lanterns.Count && _overchargeCooldown <= 0f)
                TriggerOverchargePulse();

            // Pha 2: 2 Cơn Gió Sông có cảnh báo 3 giây + Chắn Gió Hoàn Hảo
            if (!_gust1Triggered && _buildProgress >= 40f)
            {
                _gust1Triggered = true;
                _activeGustCoroutine = StartCoroutine(TelegraphedShadowGustRoutine(2, 1));
            }
            if (!_gust2Triggered && _buildProgress >= 56f)
            {
                _gust2Triggered = true;
                _activeGustCoroutine = StartCoroutine(TelegraphedShadowGustRoutine(3, 2));
            }

            // Pha 3: Triệu hồi Bướm Chúa Alpha ở mốc 74%
            if (_step == RelayStep.DefeatAlphaAndFinish && !_alphaSpawned && _buildProgress >= 74f)
            {
                _alphaSpawned = true;
                SpawnAlphaMoth();
            }

            if (litCount >= minLit)
            {
                _siteLight = Mathf.Min(100f, _siteLight + 4.5f * dt);
                float chainMult = 1f + Mathf.Max(0, _relayChainCount - 1) * 0.25f;
                float rate = (_step == RelayStep.DefeatAlphaAndFinish ? 1.85f : 1.15f) * chainMult;
                float cap = GetCurrentPhaseCap();
                _buildProgress = Mathf.Min(cap, _buildProgress + rate * dt);
            }
            else
            {
                _siteLight = Mathf.Max(0f, _siteLight - 6.8f * dt);
            }

            if (RiverBridgeController.Instance != null)
                RiverBridgeController.Instance.SetBuildProgress(_buildProgress);

            _activeMoths.RemoveAll(m => m == null || m.IsDefeated);
            int maxMoths = _step == RelayStep.DefeatAlphaAndFinish ? 6 : 4;
            float baseInterval = _step == RelayStep.DefeatAlphaAndFinish ? 2.4f : 3.2f;
            _mothSpawnTimer -= dt;
            if (_mothSpawnTimer <= 0f && _activeMoths.Count < maxMoths && mothPrefab != null)
            {
                _mothSpawnTimer = baseInterval;
                int batch = _step == RelayStep.DefeatAlphaAndFinish ? 2 : 1;
                SpawnMothWave(batch, false);
            }

            PushHud();

            if (_siteLight <= 0f)
            {
                FailAndRetryRelay("Ánh sáng công trường đã tắt lịm! Đêm Chuyền Đèn thất bại - hãy giữ >= 4/6 đèn sáng và thử lại!");
                return;
            }

            if (_buildProgress >= 100f && _step == RelayStep.DefeatAlphaAndFinish && _alphaDefeated)
            {
                StartCoroutine(PlayBridgeCompletionClimaxCutscene());
            }
        }

        private float GetCurrentPhaseCap()
        {
            if (_step == RelayStep.PickHammerEast) return 18f;
            if (_step == RelayStep.DeliverHammerWest) return 48f;
            if (_step == RelayStep.DeliverNailsEast) return 68f;
            return _alphaDefeated ? 100f : 94f;
        }

        private void TriggerOverchargePulse()
        {
            _overchargeCooldown = 14.0f;
            _drainFreezeTimer = 5.0f;
            _buildProgress = Mathf.Min(GetCurrentPhaseCap(), _buildProgress + 10f);

            for (int i = _activeMoths.Count - 1; i >= 0; i--)
            {
                var m = _activeMoths[i];
                if (m == null || m.IsDefeated) continue;
                Vector2 dir = ((Vector2)m.transform.position).normalized;
                if (dir.sqrMagnitude < 0.01f) dir = Vector2.up;
                m.TakePurifyDamage(m.IsAlphaMoth ? 25 : 999, dir, 7.5f);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                GameRoot.Instance.Audio.PlayUIClick();
            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.TriggerCameraShake(0.24f, 0.22f);
                JuiceEffectsManager.Instance.SpawnFloatingNumber(new Vector3(0f, 1.8f, 0f), "XUNG QUÁ TẢI 6 ĐÈN! (+10% Cầu)", new Color(1f, 0.94f, 0.42f));
            }
            GameEventBus.RaiseToastRequested("XUNG QUÁ TẢI 6/6 ĐÈN! Thổi bay đàn bướm, đóng băng hao đèn 5 giây & +10% tiến độ cầu!");
        }

        private IEnumerator TelegraphedShadowGustRoutine(int extinguishCount, int gustIndex)
        {
            _windGuardWindowActive = true;
            _windGuardSucceeded = false;
            _windTelegraphRemaining = 3.0f;

            GameEventBus.RaiseToastRequested($"CẢNH BÁO GIÓ SÔNG ĐỢT {gustIndex}! Sắp thổi tắt {extinguishCount} đèn sau 3 giây - Bấm [Q] hoặc thắp đèn để CHẮN GIÓ!");
            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.SpawnFloatingNumber(new Vector3(0f, 2.0f, 0f), "GIÓ SÔNG SẮP THỔI! [Q] CHẮN GIÓ!", new Color(0.55f, 0.95f, 1f));

            while (_windTelegraphRemaining > 0f && _isRunning && !_windGuardSucceeded)
            {
                _windTelegraphRemaining -= Time.deltaTime;
                yield return null;
            }

            _windGuardWindowActive = false;
            _windTelegraphRemaining = 0f;

            if (!_isRunning || _windGuardSucceeded) yield break;

            TriggerShadowGust(extinguishCount, $"CƠN GIÓ BÓNG TỐI ĐỢT {gustIndex}! {extinguishCount} cột đèn cầu vừa phụt tắt - chạy thắp lại ngay!");
        }

        private void TriggerPerfectWindGuard(Vector3 pos)
        {
            _windGuardSucceeded = true;
            _windGuardWindowActive = false;
            _windTelegraphRemaining = 0f;

            _buildProgress = Mathf.Min(GetCurrentPhaseCap(), _buildProgress + 8f);
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
                GameRoot.Instance.Stats.RestoreStats(999, 0, false);

            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                GameRoot.Instance.Audio.PlayUIClick();
            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.TriggerCameraShake(0.20f, 0.18f);
                JuiceEffectsManager.Instance.SpawnFloatingNumber(pos + Vector3.up * 1.2f, "CHẮN GIÓ HOÀN HẢO! +8% Cầu", new Color(0.45f, 1f, 0.92f));
            }
            GameEventBus.RaiseToastRequested("CHẮN GIÓ HOÀN HẢO! Bảo vệ toàn bộ cột đèn, đầy Thể Lực & +8% tiến độ cầu!");
        }

        private void TriggerShadowGust(int extinguishCount, string warningMsg)
        {
            var litIndices = new List<int>();
            for (int i = 0; i < lanterns.Count; i++)
                if (lanterns[i] != null && lanterns[i].IsLit) litIndices.Add(i);

            int count = Mathf.Min(extinguishCount, litIndices.Count);
            for (int k = 0; k < count; k++)
            {
                int pick = Random.Range(0, litIndices.Count);
                lanterns[litIndices[pick]].ExtinguishByGust();
                litIndices.RemoveAt(pick);
            }

            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.TriggerCameraShake(0.28f, 0.22f);
            GameEventBus.RaiseToastRequested(warningMsg);
            SpawnMothWave(2, false);
        }

        private void SpawnMothWave(int count, bool includeAlpha)
        {
            if (mothPrefab == null) return;
            for (int i = 0; i < count; i++)
            {
                bool fromTop = (i % 2 == 0) ? (Random.value > 0.5f) : (Random.value <= 0.5f);
                float spawnX = Random.Range(-6.2f, 6.2f);
                float spawnY = fromTop ? 2.7f : -2.2f;
                LanternMothController moth = Instantiate(mothPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
                moth.Initialize(this);
                _activeMoths.Add(moth);
            }
            if (includeAlpha) SpawnAlphaMoth();
        }

        private void SpawnAlphaMoth()
        {
            if (mothPrefab == null) return;
            LanternMothController alpha = Instantiate(mothPrefab, new Vector3(0f, 3.0f, 0f), Quaternion.identity);
            alpha.Initialize(this);
            alpha.ConfigureAsAlphaMoth();
            _activeMoths.Add(alpha);
            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.TriggerCameraShake(0.22f, 0.18f);
            GameEventBus.RaiseToastRequested("BƯỚM CHÚA HÚT SÁNG XUẤT HIỆN! Hạ nó để bùng nổ Ánh Sáng hoàn thiện nhịp cầu cuối!");
        }

        public BridgeLanternPost FindNearestLitLantern(Vector3 fromPos)
        {
            BridgeLanternPost best = null;
            float bestDist = float.MaxValue;
            for (int i = 0; i < lanterns.Count; i++)
            {
                if (lanterns[i] == null || !lanterns[i].IsLit) continue;
                float d = Vector2.Distance(fromPos, lanterns[i].transform.position);
                if (d < bestDist) { bestDist = d; best = lanterns[i]; }
            }
            return best;
        }

        public void NotifyMothDefeated(LanternMothController moth)
        {
            _activeMoths.Remove(moth);
            if (!_isRunning) return;

            if (moth != null && moth.IsAlphaMoth)
            {
                _alphaDefeated = true;
                for (int i = 0; i < lanterns.Count; i++)
                    if (lanterns[i] != null) lanterns[i].RestoreLight(100f);
                _siteLight = 100f;
                _buildProgress = 100f;
                if (RiverBridgeController.Instance != null)
                    RiverBridgeController.Instance.SetBuildProgress(100f);
                if (JuiceEffectsManager.Instance != null)
                    JuiceEffectsManager.Instance.TriggerCameraShake(0.25f, 0.22f);
                GameEventBus.RaiseToastRequested("BƯỚM CHÚA ĐÃ TAN BIẾN! Sóng Ánh Sáng thắp bừng toàn bộ 6 cột đèn cầu!");
                return;
            }

            if (moth != null)
            {
                BridgeLanternPost nearest = null;
                float bestD = float.MaxValue;
                for (int i = 0; i < lanterns.Count; i++)
                {
                    if (lanterns[i] == null) continue;
                    float d = Vector2.Distance(moth.transform.position, lanterns[i].transform.position);
                    if (d < bestD) { bestD = d; nearest = lanterns[i]; }
                }
                if (nearest != null) nearest.RestoreLight(18f);
            }
            _buildProgress = Mathf.Min(GetCurrentPhaseCap(), _buildProgress + 3.5f);
        }

        private IEnumerator PlayBridgeCompletionClimaxCutscene()
        {
            _step = RelayStep.ClimaxCutscene;
            if (_activeGustCoroutine != null) { StopCoroutine(_activeGustCoroutine); _activeGustCoroutine = null; }
            _windGuardWindowActive = false;
            ClearAllMoths();
            if (hudView != null) hudView.SetVisible(false);

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.Cutscene);

            if (RiverBridgeController.Instance != null)
                RiverBridgeController.Instance.SetBuildProgress(98f);

            GameEventBus.RaiseToastRequested("Ông Kael & Bà Mara cùng bước ra giữa cầu để đóng tấm ván nối cuối cùng!");

            float walkTime = 1.6f;
            float t = 0f;
            Vector3 kStart = _kaelTr != null ? _kaelTr.position : kaelRelayPos;
            Vector3 mStart = _maraTr != null ? _maraTr.position : maraRelayPos;
            while (t < walkTime)
            {
                t += Time.deltaTime;
                float p = Mathf.SmoothStep(0f, 1f, t / walkTime);
                if (_kaelTr != null) _kaelTr.position = Vector3.Lerp(kStart, kaelCenterPos, p);
                if (_maraTr != null) _maraTr.position = Vector3.Lerp(mStart, maraCenterPos, p);
                yield return null;
            }

            for (int hit = 1; hit <= 3; hit++)
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                    GameRoot.Instance.Audio.PlayUIClick();
                if (JuiceEffectsManager.Instance != null)
                {
                    JuiceEffectsManager.Instance.TriggerCameraShake(0.14f, 0.16f);
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(new Vector3(0f, 1.3f, 0f), hit < 3 ? "CỘP!" : "NỐI LIỀN CẦU!", new Color(1f, 0.90f, 0.42f));
                }
                if (hit == 1) GameEventBus.RaiseToastRequested("Ông Kael: \"Nhát thứ nhất, giữ chặt nhé Mara!\"");
                else if (hit == 2) GameEventBus.RaiseToastRequested("Bà Mara: \"Đinh khớp rồi! Chốt nốt nhát cuối cùng thôi!\"");
                else
                {
                    if (Chapter2StateManager.Instance != null)
                        Chapter2StateManager.Instance.SetBridgeState(RiverBridgeState.Repaired);
                    if (RiverBridgeController.Instance != null)
                        RiverBridgeController.Instance.ApplyState(RiverBridgeState.Repaired);
                    GameEventBus.RaiseToastRequested("CÂY CẦU BỜ SÔNG ĐÃ NỐI LIỀN! Hai bờ cuối cùng đã chạm vào nhau sau 3 năm xa cách!");
                }
                yield return new WaitForSeconds(1.1f);
            }

            yield return new WaitForSeconds(0.8f);
            CompleteRelayEvent();
        }

        private void CompleteRelayEvent()
        {
            _isRunning = false;
            _introPlaying = false;
            _step = RelayStep.Completed;
            RemoveRelayItem(ItemHammerId);
            RemoveRelayItem(ItemNailsId);
            RestoreNPCPositions();

            for (int i = 0; i < lanterns.Count; i++) if (lanterns[i] != null) lanterns[i].SetRelayActive(false, 100f);
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.SetRelayEventActive(false);
                Chapter2StateManager.Instance.SetBridgeState(RiverBridgeState.Repaired);
            }
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag(FlagRelayDone, true);
                GameRoot.Instance.Flags.SetFlag(FlagCh2Completed, true);
            }
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.Gameplay);
        }

        private void HandlePlayerKnockedOut(string _)
        {
            if (_isRunning) FailAndRetryRelay("Bạn kiệt sức giữa Đêm Chuyền Đèn! Hãy hồi phục và nói chuyện lại với Ông Kael.");
        }

        private void FailAndRetryRelay(string reason)
        {
            _isRunning = false;
            _introPlaying = false;
            _step = RelayStep.Inactive;
            if (_activeGustCoroutine != null) { StopCoroutine(_activeGustCoroutine); _activeGustCoroutine = null; }
            _windGuardWindowActive = false;
            ClearAllMoths();
            RemoveRelayItem(ItemHammerId);
            RemoveRelayItem(ItemNailsId);
            RestoreNPCPositions();
            for (int i = 0; i < lanterns.Count; i++) if (lanterns[i] != null) lanterns[i].SetRelayActive(false, 100f);
            if (hudView != null) hudView.SetVisible(false);
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.SetRelayEventActive(false);
                Chapter2StateManager.Instance.SetBridgeState(RiverBridgeState.Broken);
            }
            if (RiverBridgeController.Instance != null)
                RiverBridgeController.Instance.ResetBuildProgress();
            GameEventBus.RaiseToastRequested(reason);
        }

        private void PositionNPCsForRelay()
        {
            GameObject k = GameObject.Find("NPC_Kael");
            GameObject m = GameObject.Find("NPC_Mara");
            if (k != null) { _kaelTr = k.transform; _kaelOrigPos = k.transform.position; k.transform.position = kaelRelayPos; }
            if (m != null) { _maraTr = m.transform; _maraOrigPos = m.transform.position; m.transform.position = maraRelayPos; }
        }

        private void RestoreNPCPositions()
        {
            if (_kaelTr != null) _kaelTr.position = _kaelOrigPos;
            if (_maraTr != null) _maraTr.position = _maraOrigPos;
        }

        private void GiveRelayItem(string itemId)
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null && !GameRoot.Instance.Inventory.HasItem(itemId, 1))
                GameRoot.Instance.Inventory.AddItemById(itemId, 1, false);
        }

        private void RemoveRelayItem(string itemId)
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null && GameRoot.Instance.Inventory.HasItem(itemId, 1))
                GameRoot.Instance.Inventory.RemoveItem(itemId, 1);
        }

        private void ClearAllMoths()
        {
            for (int i = 0; i < _activeMoths.Count; i++) if (_activeMoths[i] != null) _activeMoths[i].ForceDespawnSilent();
            _activeMoths.Clear();
        }

        private void PushHud()
        {
            if (hudView == null) return;
            int lit = 0;
            for (int i = 0; i < lanterns.Count; i++) if (lanterns[i] != null && lanterns[i].IsLit) lit++;

            string obj;
            if (_windGuardWindowActive)
                obj = $"CẢNH BÁO GIÓ SÔNG ({_windTelegraphRemaining:0.0}s)! Bấm [Q] hoặc thắp đèn để CHẮN GIÓ!";
            else if (_step == RelayStep.PickHammerEast)
                obj = "Pha 1: Nhận Búa Thợ Mộc từ Ông Kael (Bờ Đông)";
            else if (_step == RelayStep.DeliverHammerWest)
                obj = "Pha 1: Qua Cầu Khỉ Dây Thừng mang Búa sang Bà Mara (Bờ Tây)";
            else if (_step == RelayStep.DeliverNailsEast)
                obj = "Pha 2: Mang Hộp Đinh về Ông Kael (Bấm [Q] chắn Gió Sông!)";
            else if (_alphaSpawned && !_alphaDefeated)
                obj = "Pha 3: HẠ BƯỚM CHÚA HÚT SÁNG & giữ >= 4/6 cột đèn sáng!";
            else
                obj = "Pha 3: Chém đàn Bướm Hút Sáng & giữ 6/6 đèn >75% kích Xung Quá Tải!";

            if (_relayChainCount >= 2) obj += $" [Chuỗi x{_relayChainCount}]";
            if (_drainFreezeTimer > 0f) obj += $" [Đóng băng {_drainFreezeTimer:0.0}s]";

            hudView.UpdateHUD(lit, lanterns.Count, 4, _siteLight, _buildProgress, obj);
        }
    }
}
