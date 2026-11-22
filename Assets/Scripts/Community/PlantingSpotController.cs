using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.Community
{
    public class PlantingSpotController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer flowerRenderer;
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite bloomedSprite;
        [SerializeField] private SpriteRenderer bloomAuraRenderer;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 2.4f;

        private PlayerController2D _player;
        private GameInputReader _input;
        private bool _isBloomed;

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
            RefreshBloomVisual();
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
            }
            if (bloomAuraRenderer != null)
            {
                bloomAuraRenderer.enabled = _isBloomed;
            }
        }

        private void Update()
        {
            if (_isBloomed && bloomAuraRenderer != null)
            {
                float s = 2.4f + Mathf.Sin(Time.time * 2.5f) * 0.2f;
                bloomAuraRenderer.transform.localScale = new Vector3(s, s, 1f);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
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
    }
}
