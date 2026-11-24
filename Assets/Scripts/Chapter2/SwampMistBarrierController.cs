using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2
{
    public class SwampMistBarrierController : MonoBehaviour
    {
        [SerializeField] private GameObject mistVisualsRoot;
        [SerializeField] private Collider2D mistBlockerCollider;
        [SerializeField] private GameObject unlockedLanternGlow;

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += RefreshBarrierState;
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.OnStageChanged += HandleStageChanged;
            }
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= RefreshBarrierState;
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.OnStageChanged -= HandleStageChanged;
            }
        }

        private void Start()
        {
            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.OnStageChanged -= HandleStageChanged;
                Chapter2StateManager.Instance.OnStageChanged += HandleStageChanged;
            }
            RefreshBarrierState();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == Chapter2StateManager.FlagSwampUnlocked ||
                flagId == Chapter2StateManager.FlagSwampCleared ||
                flagId == Chapter2StateManager.FlagCh2Completed)
            {
                RefreshBarrierState();
            }
        }

        private void HandleStageChanged(Chapter2Stage _)
        {
            RefreshBarrierState();
        }

        public void RefreshBarrierState()
        {
            bool unlocked = Chapter2StateManager.Instance != null && Chapter2StateManager.Instance.IsSwampUnlocked();
            if (mistVisualsRoot != null && mistVisualsRoot.activeSelf == unlocked)
            {
                mistVisualsRoot.SetActive(!unlocked);
            }
            if (mistBlockerCollider != null)
            {
                mistBlockerCollider.enabled = !unlocked;
            }
            if (unlockedLanternGlow != null && unlockedLanternGlow.activeSelf != unlocked)
            {
                unlockedLanternGlow.SetActive(unlocked);
            }
        }
    }
}
