using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2
{
    public class RiverBridgeController : MonoBehaviour
    {
        public static RiverBridgeController Instance { get; private set; }

        [SerializeField] private GameObject brokenVisualsRoot;
        [SerializeField] private GameObject scaffoldingVisualsRoot;
        [SerializeField] private GameObject[] progressivePlankSegments;
        [SerializeField] private GameObject repairedVisualsRoot;
        [SerializeField] private Collider2D bridgeGapBlocker;
        [SerializeField] private Collider2D bridgeRailNorth;
        [SerializeField] private Collider2D bridgeRailSouth;

        private float _buildProgress;
        private int _lastAnnouncedTier;
        private Transform _ropeCatwalkGroup;

        private void Awake() { Instance = this; }
        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void OnEnable()
        {
            if (Chapter2StateManager.Instance != null)
                Chapter2StateManager.Instance.OnBridgeStateChanged += ApplyState;
            GameEventBus.OnSaveLoaded += RefreshFromState;
        }

        private void OnDisable()
        {
            if (Chapter2StateManager.Instance != null)
                Chapter2StateManager.Instance.OnBridgeStateChanged -= ApplyState;
            GameEventBus.OnSaveLoaded -= RefreshFromState;
        }

        private void Start()
        {
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.OnBridgeStateChanged -= ApplyState;
                Chapter2StateManager.Instance.OnBridgeStateChanged += ApplyState;
            }
            RefreshFromState();
        }

        public void RefreshFromState()
        {
            RiverBridgeState st = Chapter2StateManager.Instance != null
                ? Chapter2StateManager.Instance.BridgeState
                : RiverBridgeState.Broken;
            ApplyState(st);
        }

        public void ResetBuildProgress()
        {
            _buildProgress = 0f;
            _lastAnnouncedTier = 0;
            ApplyProgressivePlanks();
            SetCatwalkReveal(1f);
        }

        public void SetCatwalkReveal(float t01)
        {
            if (scaffoldingVisualsRoot == null) return;
            if (_ropeCatwalkGroup == null)
                _ropeCatwalkGroup = scaffoldingVisualsRoot.transform.Find("RopeCatwalkSpan");
            if (_ropeCatwalkGroup == null) return;

            float clamped = Mathf.Clamp01(t01);
            int total = _ropeCatwalkGroup.childCount;
            if (total == 0) return;

            // Unfurl from east/west banks toward center (x = -0.5)
            float maxDist = 4.6f;
            float allowedDistFromBank = clamped * maxDist;
            for (int i = 0; i < total; i++)
            {
                Transform ch = _ropeCatwalkGroup.GetChild(i);
                float distFromCenter = Mathf.Abs(ch.localPosition.x + 0.5f);
                float distFromBank = Mathf.Max(0f, maxDist - distFromCenter);
                ch.gameObject.SetActive(clamped >= 0.99f || distFromBank <= allowedDistFromBank);
            }
        }

        public void SetBuildProgress(float progressPercent)
        {
            _buildProgress = Mathf.Clamp(progressPercent, 0f, 100f);
            ApplyProgressivePlanks();

            int tier = _buildProgress >= 95f ? 3 : (_buildProgress >= 70f ? 2 : (_buildProgress >= 35f ? 1 : 0));
            if (tier > _lastAnnouncedTier)
            {
                _lastAnnouncedTier = tier;
                if (JuiceEffectsManager.Instance != null)
                {
                    JuiceEffectsManager.Instance.TriggerCameraShake(0.16f, 0.14f);
                    Vector3 popPos = tier == 1 ? new Vector3(2.5f, 1.4f, 0f) : (tier == 2 ? new Vector3(-2.5f, 1.4f, 0f) : new Vector3(0f, 1.5f, 0f));
                    string popText = tier == 1 ? "GHÉP NHỊP ĐÔNG (35%)!" : (tier == 2 ? "GHÉP NHỊP TÂY (70%)!" : "CHỐT NHỊP GIỮA!");
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(popPos, popText, new Color(1f, 0.88f, 0.44f));
                }
                if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                    GameRoot.Instance.Audio.PlayUIClick();

                if (tier == 1)
                    GameEventBus.RaiseToastRequested("Nhịp ván gỗ Bờ Đông (35%) đã ghép vững lên Cầu Khỉ Dây Thừng!");
                else if (tier == 2)
                    GameEventBus.RaiseToastRequested("Nhịp ván gỗ Bờ Tây (70%) đã khớp khung! Chỉ còn khe giữa cuối cùng!");
                else if (tier == 3)
                    GameEventBus.RaiseToastRequested("Tấm ván trung tâm đã vào vị trí! Sẵn sàng nối liền cây cầu!");
            }
        }

        public void ApplyState(RiverBridgeState state)
        {
            bool isBroken = state == RiverBridgeState.Broken;
            bool isRelay = state == RiverBridgeState.RelayInProgress;
            bool isRepaired = state == RiverBridgeState.Repaired;

            if (brokenVisualsRoot != null) brokenVisualsRoot.SetActive(isBroken);
            if (scaffoldingVisualsRoot != null) scaffoldingVisualsRoot.SetActive(isRelay);
            if (repairedVisualsRoot != null) repairedVisualsRoot.SetActive(isRepaired);
            if (bridgeGapBlocker != null) bridgeGapBlocker.enabled = isBroken;

            if (bridgeRailNorth != null) bridgeRailNorth.enabled = true;
            if (bridgeRailSouth != null) bridgeRailSouth.enabled = true;

            if (isRelay) ApplyProgressivePlanks();
        }

        private void ApplyProgressivePlanks()
        {
            if (progressivePlankSegments == null || progressivePlankSegments.Length == 0) return;
            float[] thresholds = { 35f, 70f, 95f };
            for (int i = 0; i < progressivePlankSegments.Length; i++)
            {
                if (progressivePlankSegments[i] == null) continue;
                float th = i < thresholds.Length ? thresholds[i] : 95f;
                progressivePlankSegments[i].SetActive(_buildProgress >= th);
            }
        }
    }
}
