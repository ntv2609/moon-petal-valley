using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2
{
    public class RiverBridgeController : MonoBehaviour
    {
        [SerializeField] private GameObject brokenVisualsRoot;
        [SerializeField] private GameObject repairedVisualsRoot;
        [SerializeField] private Collider2D bridgeGapBlocker;

        private void OnEnable()
        {
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.OnBridgeStateChanged += ApplyState;
            }
            GameEventBus.OnSaveLoaded += RefreshFromState;
        }

        private void OnDisable()
        {
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.OnBridgeStateChanged -= ApplyState;
            }
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

        public void ApplyState(RiverBridgeState state)
        {
            bool passable = state == RiverBridgeState.RelayInProgress || state == RiverBridgeState.Repaired;
            if (brokenVisualsRoot != null) brokenVisualsRoot.SetActive(!passable);
            if (repairedVisualsRoot != null) repairedVisualsRoot.SetActive(passable);
            if (bridgeGapBlocker != null) bridgeGapBlocker.enabled = !passable;
        }
    }
}
