using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter3
{
    public class NightMarketCustomerSpot : MonoBehaviour, IInteractable
    {
        [SerializeField] private string stallId;
        [SerializeField] private string villagerName;
        [SerializeField] private float interactRadius = 1.9f;
        [SerializeField] private SpriteRenderer requestBadgeRenderer;
        [SerializeField] private NightMarketServiceManager serviceManager;

        private bool _hasActiveOrder;
        private string _requestedItemId;
        private string _requestedItemName;
        private Vector3 _badgeBaseLocalPos;
        private bool _badgeInit;

        public string StallId => stallId;
        public string VillagerName => villagerName;
        public bool HasActiveOrder => _hasActiveOrder;
        public string RequestedItemId => _requestedItemId;
        public string RequestedItemName => _requestedItemName;

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
                if (!gameObject.activeInHierarchy || !_hasActiveOrder) return false;
                if (GameRoot.Instance == null || GameRoot.Instance.State == null) return false;
                if (!GameRoot.Instance.State.IsGameplayActive) return false;
                NightMarketServiceManager mgr = ResolveManager();
                return mgr != null && mgr.IsServiceActive;
            }
        }

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 1.1f, 0f);

        public string InteractionPromptLabel
        {
            get
            {
                if (!_hasActiveOrder) return villagerName;
                return "Phục vụ " + villagerName + " (" + _requestedItemName + ") [E]";
            }
        }

        private void Awake()
        {
            CacheBadgeOrigin();
            SetBadgeVisible(false);
        }

        private void Update()
        {
            if (_hasActiveOrder && requestBadgeRenderer != null && requestBadgeRenderer.enabled)
            {
                CacheBadgeOrigin();
                float bob = Mathf.Sin(Time.time * 4.2f) * 0.08f;
                requestBadgeRenderer.transform.localPosition = _badgeBaseLocalPos + new Vector3(0f, bob, 0f);
            }
        }

        public void SetOrder(string itemId, string itemDisplayName)
        {
            _hasActiveOrder = true;
            _requestedItemId = itemId;
            _requestedItemName = itemDisplayName;
            SetBadgeVisible(true);
        }

        public void ClearOrder()
        {
            _hasActiveOrder = false;
            _requestedItemId = "";
            _requestedItemName = "";
            SetBadgeVisible(false);
        }

        public void Interact()
        {
            if (!CanInteract) return;
            NightMarketServiceManager mgr = ResolveManager();
            if (mgr != null)
                mgr.ServeSpot(this);
        }

        public void HideLegacyPrompt() { }

        private void CacheBadgeOrigin()
        {
            if (_badgeInit || requestBadgeRenderer == null) return;
            _badgeBaseLocalPos = requestBadgeRenderer.transform.localPosition;
            _badgeInit = true;
        }

        private void SetBadgeVisible(bool visible)
        {
            if (requestBadgeRenderer != null)
                requestBadgeRenderer.enabled = visible;
        }

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
