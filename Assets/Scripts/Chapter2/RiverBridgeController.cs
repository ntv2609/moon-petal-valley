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
        }

        public void SetBuildProgress(float progressPercent)
        {
            _buildProgress = Mathf.Clamp(progressPercent, 0f, 100f);
            ApplyProgressivePlanks();

            int tier = _buildProgress >= 95f ? 3 : (_buildProgress >= 66f ? 2 : (_buildProgress >= 33f ? 1 : 0));
            if (tier > _lastAnnouncedTier)
            {
                _lastAnnouncedTier = tier;
                if (tier == 1)
                    GameEventBus.RaiseToastRequested("Nhịp ván Bờ Đông (33%) đã được ghép lên giàn giáo!");
                else if (tier == 2)
                    GameEventBus.RaiseToastRequested("Nhịp ván Bờ Tây (66%) đã khớp khung cầu!");
                else if (tier == 3)
                    GameEventBus.RaiseToastRequested("Chỉ còn tấm ván trung tâm cuối cùng!");
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

            // Lan can cứng 2 mép Bắc/Nam luôn bật khi đang xây cầu hoặc cầu đã nối xong -> tuyệt đối không đi trên mặt nước!
            bool railsActive = isRelay || isRepaired;
            if (bridgeRailNorth != null) bridgeRailNorth.enabled = railsActive;
            if (bridgeRailSouth != null) bridgeRailSouth.enabled = railsActive;

            if (isRelay) ApplyProgressivePlanks();
        }

        private void ApplyProgressivePlanks()
        {
            if (progressivePlankSegments == null || progressivePlankSegments.Length == 0) return;
            float[] thresholds = { 33f, 66f, 95f };
            for (int i = 0; i < progressivePlankSegments.Length; i++)
            {
                if (progressivePlankSegments[i] == null) continue;
                float th = i < thresholds.Length ? thresholds[i] : 95f;
                progressivePlankSegments[i].SetActive(_buildProgress >= th);
            }
        }
    }
}
