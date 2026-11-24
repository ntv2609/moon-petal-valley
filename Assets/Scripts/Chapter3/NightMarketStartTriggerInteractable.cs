using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter3
{
    public class NightMarketStartTriggerInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private float interactRadius = 2.0f;
        [SerializeField] private NightMarketServiceManager serviceManager;

        private void OnEnable()
        {
            InteractionManager.Register(this);
        }

        private void OnDisable()
        {
            InteractionManager.Unregister(this);
        }

        public bool CanInteract
        {
            get
            {
                if (!gameObject.activeInHierarchy) return false;
                if (GameRoot.Instance == null || GameRoot.Instance.State == null ||
                    GameRoot.Instance.Flags == null)
                    return false;
                if (!GameRoot.Instance.State.IsGameplayActive) return false;

                StoryFlagManager flags = GameRoot.Instance.Flags;
                if (!flags.HasFlag(Chapter3StateManager.FlagCh2Completed)) return false;
                if (!flags.HasFlag(Chapter3StateManager.FlagCh3StallsBuiltDay3)) return false;
                if (flags.HasFlag(Chapter3StateManager.FlagCh3MarketDone)) return false;

                NightMarketServiceManager mgr = ResolveManager();
                return mgr != null && !mgr.IsServiceActive && !mgr.IsTransitioning;
            }
        }

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 1.15f, 0f);
        public string InteractionPromptLabel => "Khai Hội Chợ Đêm Đầu Tiên [E]";

        public void Interact()
        {
            if (!CanInteract) return;
            NightMarketServiceManager mgr = ResolveManager();
            if (mgr != null)
                mgr.RequestStartMarketService();
        }

        public void HideLegacyPrompt() { }

        private NightMarketServiceManager ResolveManager()
        {
            if (serviceManager == null)
                serviceManager = NightMarketServiceManager.Instance;
            if (serviceManager == null)
                serviceManager = Object.FindAnyObjectByType<NightMarketServiceManager>();
            return serviceManager;
        }
    }
}
