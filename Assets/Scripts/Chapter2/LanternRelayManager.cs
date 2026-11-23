using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.Chapter2
{
    public class LanternRelayManager : MonoBehaviour
    {
        public enum RelayStep { Inactive = 0, PickHammerEast = 1, DeliverHammerWest = 2, DeliverNailsEast = 3, HoldLightForFinalPlank = 4, ClimaxCutscene = 5, Completed = 6 }

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
        [SerializeField] private Vector3 kaelRelayPos = new Vector3(8.6f, 0.8f, 0f);
        [SerializeField] private Vector3 maraRelayPos = new Vector3(-9.2f, 0.8f, 0f);
        [SerializeField] private Vector3 kaelCenterPos = new Vector3(1.1f, 0.5f, 0f);
        [SerializeField] private Vector3 maraCenterPos = new Vector3(-1.1f, 0.5f, 0f);

        private readonly List<LanternMothController> _activeMoths = new List<LanternMothController>();
        private RelayStep _step = RelayStep.Inactive;
        private float _siteLight = 100f, _buildProgress = 0f, _mothSpawnTimer;
        private bool _isRunning;
        private Transform _kaelTr, _maraTr;
        private Vector3 _kaelOrigPos, _maraOrigPos;

        public bool IsRelayActive => _isRunning;
        public RelayStep CurrentStep => _step;
        public float MothDrainPerSec => config != null ? config.mothDrainPerSec : 12.0f;

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
            if (_step == RelayStep.ClimaxCutscene) return false;
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags == null || flags.HasFlag(FlagRelayDone) || flags.HasFlag(FlagCh2Completed)) return false;
            if (!_isRunning) return flags.HasFlag(FlagEcho2Done);
            return _step == RelayStep.PickHammerEast || _step == RelayStep.DeliverNailsEast;
        }

        public bool CanInteractWestStation() => _isRunning && _step == RelayStep.DeliverHammerWest;

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
            if (!_isRunning) { BeginRelayFromEast(); return; }
            if (_step == RelayStep.PickHammerEast)
            {
                GiveRelayItem(ItemHammerId);
                _step = RelayStep.DeliverHammerWest;
                GameEventBus.RaiseToastRequested("Ông Kael: \"Mang cây búa này sang cho bà Mara! Nhớ giữ đèn cầu đừng tắt!\"");
            }
            else if (_step == RelayStep.DeliverNailsEast)
            {
                RemoveRelayItem(ItemNailsId);
                _buildProgress = Mathf.Min(100f, _buildProgress + 28f);
                _step = RelayStep.HoldLightForFinalPlank;
                GameEventBus.RaiseToastRequested("Ông Kael: \"Đinh đồng tốt lắm! Nhắn bà Mara giữ nhịp, tôi đóng nốt ván cuối ngay!\"");
            }
        }

        public void HandleWestStationInteract()
        {
            if (!_isRunning || _step != RelayStep.DeliverHammerWest) return;
            RemoveRelayItem(ItemHammerId);
            GiveRelayItem(ItemNailsId);
            _buildProgress = Mathf.Min(100f, _buildProgress + 28f);
            _step = RelayStep.DeliverNailsEast;
            GameEventBus.RaiseToastRequested("Bà Mara: \"Búa vẫn chắc tay như xưa... Cháu mang hộp đinh đồng này về cho ông Kael!\"");
        }

        private void BeginRelayFromEast()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null && GameRoot.Instance.Time.CurrentHourFloat < 18f)
                GameRoot.Instance.Time.SetHour(18.5f);

            if (GameRoot.Instance != null && GameRoot.Instance.Dialogue != null)
            {
                GameRoot.Instance.Dialogue.StartQuickLines(new (string, string)[]
                {
                    ("Ông Kael", "Sương xuống rồi. Đèn bão bờ Tây của bà Mara cũng vừa sáng lên bên kia sông."),
                    ("Tinh Linh Lumi", "Sáu cột đèn trên thân cầu đã dựng xong! Wen ơi, giữ tối thiểu 4 ngọn đèn sáng để hai bác đủ ánh sáng đóng cầu nhé!"),
                    ("Ông Kael", "Nào cô bé, cầm lấy cây búa này chạy sang bờ Tây trước. Gió mạnh lắm, Dash lướt qua mà đừng sợ tốn sức!")
                }, StartRelayEvent);
            }
            else StartRelayEvent();
        }

        public void StartRelayEvent()
        {
            if (_isRunning) return;
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null && GameRoot.Instance.Time.CurrentHourFloat < 18f)
                GameRoot.Instance.Time.SetHour(18.5f);

            CacheAndPlaceNPCsForRelay();
            _isRunning = true;
            _siteLight = 100f;
            _buildProgress = 5f;
            _mothSpawnTimer = 2.0f;
            _step = RelayStep.DeliverHammerWest;
            GiveRelayItem(ItemHammerId);

            if (Chapter2StateManager.Instance != null) Chapter2StateManager.Instance.SetRelayEventActive(true);
            for (int i = 0; i < lanterns.Count; i++) if (lanterns[i] != null) lanterns[i].SetRelayActive(true, 100f);
            if (hudView != null) hudView.SetVisible(true);
            GameEventBus.RaiseToastRequested("ĐÊM CHUYỀN ĐÈN BẮT ĐẦU! Mang Búa sang Bờ Tây & giữ ≥ 4/6 đèn sáng!");
        }

        private void Update()
        {
            if (!_isRunning || _step == RelayStep.ClimaxCutscene) return;
            bool active = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!active) return;

            float passiveDrain = config != null ? config.passiveDrainPerSec : 2.5f;
            int minLit = config != null ? config.minLitLanternsForWork : 4;
            float siteDrain = config != null ? config.siteLightDrainIfDark : 7.5f;
            float siteRecover = config != null ? config.siteLightRecoverPerSec : 4.0f;
            float spawnInterval = config != null ? config.mothSpawnInterval : 5.5f;
            int maxMoths = config != null ? config.maxActiveMoths : 4;

            int litCount = 0;
            for (int i = 0; i < lanterns.Count; i++)
            {
                if (lanterns[i] == null) continue;
                lanterns[i].TickPassiveDrain(passiveDrain * Time.deltaTime);
                if (lanterns[i].IsLit) litCount++;
            }

            if (litCount >= minLit)
            {
                _siteLight = Mathf.Min(100f, _siteLight + siteRecover * Time.deltaTime);
                float rate = _step == RelayStep.HoldLightForFinalPlank ? 4.2f : 1.15f;
                float cap = _step == RelayStep.HoldLightForFinalPlank ? 100f : 85f;
                if (_buildProgress < cap) _buildProgress = Mathf.Min(cap, _buildProgress + rate * Time.deltaTime);
            }
            else _siteLight = Mathf.Max(0f, _siteLight - siteDrain * Time.deltaTime);

            CleanupDeadMoths();
            _mothSpawnTimer -= Time.deltaTime;
            if (_mothSpawnTimer <= 0f && _activeMoths.Count < maxMoths) { SpawnMoth(); _mothSpawnTimer = spawnInterval; }

            if (hudView != null) hudView.UpdateHUD(litCount, lanterns.Count, minLit, _siteLight, _buildProgress, GetCurrentObjectiveText());

            if (_siteLight <= 0f) { TriggerSoftFail("Ánh sáng công trường đã tắt lịm! Đêm mai chúng ta thử lại nhé."); return; }
            if (_step == RelayStep.HoldLightForFinalPlank && _buildProgress >= 100f) StartCoroutine(PlayFinalPlankClimaxRoutine());
        }

        private string GetCurrentObjectiveText()
        {
            switch (_step)
            {
                case RelayStep.PickHammerEast: return "Bước 1/3: Nhận Búa Thợ Mộc từ Ông Kael ở đầu cầu Đông.";
                case RelayStep.DeliverHammerWest: return "Bước 1/3: Mang Búa Thợ Mộc sang đầu cầu Tây cho Bà Mara (Dash miễn phí!).";
                case RelayStep.DeliverNailsEast: return "Bước 2/3: Mang Hộp Đinh Đồng từ Bà Mara về đầu cầu Đông cho Ông Kael.";
                case RelayStep.HoldLightForFinalPlank: return "Bước 3/3: Đánh đuổi Bướm Hút Sáng, giữ ≥ 4/6 đèn sáng để đóng tấm ván cuối!";
                default: return "Giữ ánh sáng trên cây cầu bờ sông.";
            }
        }

        private IEnumerator PlayFinalPlankClimaxRoutine()
        {
            _step = RelayStep.ClimaxCutscene;
            DespawnAllMoths();
            if (hudView != null) hudView.SetVisible(false);
            for (int i = 0; i < lanterns.Count; i++) if (lanterns[i] != null) lanterns[i].SetRelayActive(false, 100f);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null) GameRoot.Instance.State.SetState(GameState.Cutscene);

            float dur = 1.6f, elapsed = 0f;
            Vector3 kStart = _kaelTr != null ? _kaelTr.position : kaelRelayPos;
            Vector3 mStart = _maraTr != null ? _maraTr.position : maraRelayPos;
            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / dur);
                if (_kaelTr != null) _kaelTr.position = Vector3.Lerp(kStart, kaelCenterPos, t);
                if (_maraTr != null) _maraTr.position = Vector3.Lerp(mStart, maraCenterPos, t);
                yield return null;
            }

            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.TriggerCameraShake(0.16f, 0.14f);
                JuiceEffectsManager.Instance.SpawnFloatingNumber(new Vector3(0f, 1.2f, 0f), "CỘC! Tấm ván cuối đã khít!", new Color(1f, 0.92f, 0.55f));
            }
            yield return new WaitForSeconds(0.65f);

            bool dialogueDone = false;
            if (GameRoot.Instance != null && GameRoot.Instance.Dialogue != null)
            {
                GameRoot.Instance.Dialogue.StartQuickLines(new (string, string)[]
                {
                    ("Ông Kael", "Giữ chặt đầu ván nhé, Mara. Đinh cuối cùng đây... xong rồi."),
                    ("Bà Mara", "...Vẫn thẳng tắp như hồi ba mươi năm trước. Đêm bão năm ấy... tôi đã trách nhầm ông rồi, Kael."),
                    ("Ông Kael", "Tôi cũng cố chấp không nói một lời. Thôi, từ đêm nay cầu nối lại rồi, đèn trên sông sẽ không bao giờ tắt nữa."),
                    ("Tinh Linh Lumi", "Cây cầu đã sáng rực cả khúc sông rồi Wen ơi! Hai bác ấy làm hòa bằng chính đôi tay mình!")
                }, () => dialogueDone = true);
                while (!dialogueDone) yield return null;
            }

            _isRunning = false;
            _step = RelayStep.Completed;
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.SetBridgeState(RiverBridgeState.Repaired);
                Chapter2StateManager.Instance.SetRelayEventActive(false);
            }
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
                GameRoot.Instance.Flags.SetFlag(FlagRelayDone, true);

            RefreshBridgeLanternsFromState();
            bool ch2EndingPlaying = Chapter2EndingController.Instance != null && Chapter2EndingController.Instance.IsPlaying;
            if (!ch2EndingPlaying && GameRoot.Instance != null && GameRoot.Instance.State != null) GameRoot.Instance.State.SetState(GameState.Gameplay);
            GameEventBus.RaiseToastRequested("HOÀN THÀNH ĐÊM CHUYỀN ĐÈN! Cây cầu bờ sông đã thông suốt vĩnh viễn!");
        }

        private void HandlePlayerKnockedOut(string reason) { if (_isRunning) TriggerSoftFail(reason); }

        private void TriggerSoftFail(string reason)
        {
            if (!_isRunning) return;
            _isRunning = false;
            _step = RelayStep.Inactive;
            DespawnAllMoths();
            RemoveRelayItem(ItemHammerId);
            RemoveRelayItem(ItemNailsId);
            if (_kaelTr != null) _kaelTr.position = _kaelOrigPos;
            if (_maraTr != null) _maraTr.position = _maraOrigPos;
            if (hudView != null) hudView.SetVisible(false);
            for (int i = 0; i < lanterns.Count; i++) if (lanterns[i] != null) lanterns[i].SetRelayActive(false, 100f);
            if (Chapter2StateManager.Instance != null) Chapter2StateManager.Instance.SetRelayEventActive(false);

            SleepAndKnockoutController sleepCtrl = FindAnyObjectByType<SleepAndKnockoutController>();
            if (sleepCtrl != null) sleepCtrl.TriggerRelaySoftFail();
            else GameEventBus.RaiseToastRequested(reason);
        }

        public BridgeLanternPost FindNearestLitLantern(Vector3 fromPos)
        {
            BridgeLanternPost best = null;
            float bestDist = float.MaxValue;
            for (int i = 0; i < lanterns.Count; i++)
            {
                BridgeLanternPost post = lanterns[i];
                if (post == null || !post.IsLit) continue;
                float d = (post.transform.position - fromPos).sqrMagnitude;
                if (d < bestDist) { bestDist = d; best = post; }
            }
            return best;
        }

        private void SpawnMoth()
        {
            if (mothPrefab == null) return;
            float spawnX = Random.Range(-6.8f, 6.8f);
            float spawnY = Random.value > 0.5f ? 5.6f : -4.4f;
            LanternMothController moth = Instantiate(mothPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
            moth.Initialize(this);
            _activeMoths.Add(moth);
        }

        public void NotifyMothDefeated(LanternMothController moth) { _activeMoths.Remove(moth); }
        private void CleanupDeadMoths() { for (int i = _activeMoths.Count - 1; i >= 0; i--) if (_activeMoths[i] == null || _activeMoths[i].IsDefeated) _activeMoths.RemoveAt(i); }
        private void DespawnAllMoths() { for (int i = 0; i < _activeMoths.Count; i++) if (_activeMoths[i] != null) _activeMoths[i].ForceDespawnSilent(); _activeMoths.Clear(); }

        private void CacheAndPlaceNPCsForRelay()
        {
            GameObject kGo = GameObject.Find("NPC_Kael");
            GameObject mGo = GameObject.Find("NPC_Mara");
            if (kGo != null) { _kaelTr = kGo.transform; _kaelOrigPos = _kaelTr.position; _kaelTr.position = kaelRelayPos; }
            if (mGo != null) { _maraTr = mGo.transform; _maraOrigPos = _maraTr.position; _maraTr.position = maraRelayPos; }
        }

        private static void GiveRelayItem(string itemId)
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null && !GameRoot.Instance.Inventory.HasItem(itemId, 1))
                GameRoot.Instance.Inventory.AddItemById(itemId, 1, false);
        }

        private static void RemoveRelayItem(string itemId)
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                int count = GameRoot.Instance.Inventory.GetItemCount(itemId);
                if (count > 0) GameRoot.Instance.Inventory.RemoveItem(itemId, count);
            }
        }
    }
}
