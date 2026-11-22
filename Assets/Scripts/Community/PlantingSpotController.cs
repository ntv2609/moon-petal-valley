using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.Community
{
    public class PlantingSpotController : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer flowerRenderer;
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite bloomedSprite;
        [SerializeField] private SpriteRenderer bloomAuraRenderer;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 2.4f;

        private bool _isBloomed;

        public bool CanInteract => gameObject.activeInHierarchy;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => new Vector3(transform.position.x, transform.position.y + 1.05f, 0f);
        public string InteractionPromptLabel => _isBloomed
            ? "Ngắm Hoa Moonpetal Nở Rộ"
            : "Bồn Hoa Trung Tâm (Gói Ký Ức)";

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            InteractionManager.Register(this);
                    GameEventBus.OnSaveLoaded += HandleSaveLoaded_Step40;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            InteractionManager.Unregister(this);
                    GameEventBus.OnSaveLoaded -= HandleSaveLoaded_Step40;
        }

        private void Start()
        {
            HideLegacyPrompt();
            RefreshBloomVisual();
        }

        public void HideLegacyPrompt()
        {
            if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == "flag_nv1_completed") RefreshBloomVisual();
        }

        public void RefreshBloomVisual()
        {
            _isBloomed = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                         GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");

            if (flowerRenderer != null)
            {
                flowerRenderer.sprite = _isBloomed ? bloomedSprite : emptySprite;
                flowerRenderer.sortingLayerName = "GroundDecor";
                flowerRenderer.sortingOrder = 15;
            }
            if (bloomAuraRenderer != null)
            {
                bloomAuraRenderer.enabled = _isBloomed;
                bloomAuraRenderer.sortingLayerName = "WeatherVFX";
                bloomAuraRenderer.sortingOrder = 12;
            }
        }

        private void Update()
        {
            if (_isBloomed && bloomAuraRenderer != null)
            {
                float s = 2.4f + Mathf.Sin(Time.time * 2.5f) * 0.2f;
                bloomAuraRenderer.transform.localScale = new Vector3(s, s, 1f);
            }
        }

        public void Interact()
        {
            if (_isBloomed)
            {
                GameEventBus.RaiseToastRequested("Hoa Moonpetal đang nở rực rỡ! Sương Héo Úa đã tan khỏi Làng Chính.");
                return;
            }

            var bundleUI = UnityEngine.Object.FindAnyObjectByType<BundleUIView>();
            if (bundleUI != null)
            {
                bundleUI.Open(this);
            }
        }

        public void ExecuteBloomClimax()
        {
            if (_isBloomed || GameRoot.Instance == null) return;

            var bundle = GameRoot.Instance.Bundle?.ActiveBundle;
            if (bundle != null && bundle.rewardItem != null)
            {
                GameRoot.Instance.Inventory?.AddItem(bundle.rewardItem, 1, true);
            }

            GameRoot.Instance.Flags?.SetFlag("flag_nv1_completed", true);
            RefreshBloomVisual();

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null) lumi.SetHaloRadiusScale(3.2f);

            GameEventBus.RaiseToastRequested("HOA MOONPETAL ĐÃ NỞ! Nhận Cánh Hoa Hy Vọng & Mở khóa Xung Ánh Trăng [Q]!");
        }
    
        private void HandleSaveLoaded_Step40()
        {
            RefreshBloomVisual();
        }
}
}
