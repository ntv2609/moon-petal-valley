using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.Chapter3
{
    public class NightMarketOverlayController : MonoBehaviour
    {
        [SerializeField] private GameObject visualsRoot;
        [SerializeField] private string requiredCh2Flag = "flag_ch2_completed";
        [SerializeField] private string requiredStallsFlag = "flag_ch3_stalls_built_day3";

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += RefreshVisibility;
            if (Chapter3StateManager.Instance != null)
                Chapter3StateManager.Instance.OnPhaseChanged += HandlePhaseChanged;
            if (Chapter2StateManager.Instance != null)
                Chapter2StateManager.Instance.OnBridgeStateChanged += HandleBridgeChanged;
            RefreshVisibility();
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= RefreshVisibility;
            if (Chapter3StateManager.Instance != null)
                Chapter3StateManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
            if (Chapter2StateManager.Instance != null)
                Chapter2StateManager.Instance.OnBridgeStateChanged -= HandleBridgeChanged;
        }

        private void Start()
        {
            if (Chapter3StateManager.Instance != null)
            {
                Chapter3StateManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
                Chapter3StateManager.Instance.OnPhaseChanged += HandlePhaseChanged;
            }
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.OnBridgeStateChanged -= HandleBridgeChanged;
                Chapter2StateManager.Instance.OnBridgeStateChanged += HandleBridgeChanged;
            }
            RefreshVisibility();
        }

        private void HandleFlagChanged(string flagId, bool _)
        {
            if (flagId == requiredCh2Flag || flagId == requiredStallsFlag ||
                flagId == Chapter3StateManager.FlagCh3MarketDone)
            {
                RefreshVisibility();
            }
        }

        private void HandlePhaseChanged(Chapter3Phase _)
        {
            RefreshVisibility();
        }

        private void HandleBridgeChanged(RiverBridgeState _)
        {
            RefreshVisibility();
        }

        public bool ShouldBeVisible()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null)
                return false;
            StoryFlagManager flags = GameRoot.Instance.Flags;
            if (!flags.HasFlag(requiredCh2Flag)) return false;
            if (!flags.HasFlag(requiredStallsFlag)) return false;

            if (Chapter2StateManager.Instance != null)
            {
                RiverBridgeState bst = Chapter2StateManager.Instance.BridgeState;
                if (bst == RiverBridgeState.Broken) return false;
            }
            return true;
        }

        public void RefreshVisibility()
        {
            bool visible = ShouldBeVisible();
            if (visualsRoot != null)
                visualsRoot.SetActive(visible);
        }
    }
}
