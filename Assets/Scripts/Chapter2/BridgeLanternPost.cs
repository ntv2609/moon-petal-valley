using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2
{
    public class BridgeLanternPost : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer glowRenderer;
        [SerializeField] private Sprite litSprite;
        [SerializeField] private Sprite dimSprite;
        [SerializeField] private Sprite outSprite;
        [SerializeField] private GameObject lightBarRoot;
        [SerializeField] private Transform lightBarFill;
        [SerializeField] private int lanternIndex;
        [SerializeField] private float interactRadius = 2.35f;

        private float _currentLight = 100f;
        private bool _relayActive;
        private Vector3 _barBaseScale = Vector3.one;

        public float CurrentLight => _currentLight;
        public bool IsLit => _currentLight > 15f;
        public int LanternIndex => lanternIndex;
        public bool CanInteract => _relayActive && _currentLight < 92f;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + Vector3.up * 1.15f;
        public string InteractionPromptLabel => _currentLight <= 15f ? "Thắp Lại Đèn Cầu" : "Mồi Thêm Lửa";

        private void Awake()
        {
            if (lightBarFill != null) _barBaseScale = lightBarFill.localScale;
        }

        private void OnEnable()
        {
            InteractionManager.Register(this);
            ApplyBridgeVisibility();
        }

        private void OnDisable()
        {
            InteractionManager.Unregister(this);
        }

        public void SetRelayActive(bool active, float initialLight = 100f)
        {
            _relayActive = active;
            _currentLight = Mathf.Clamp(initialLight, 0f, 100f);
            if (lightBarRoot != null) lightBarRoot.SetActive(active);
            ApplyBridgeVisibility();
            RefreshVisual();
        }

        public void ApplyBridgeVisibility()
        {
            bool visible = _relayActive || (Chapter2StateManager.Instance != null &&
                Chapter2StateManager.Instance.BridgeState != RiverBridgeState.Broken);
            if (spriteRenderer != null) spriteRenderer.enabled = visible;
            if (glowRenderer != null) glowRenderer.enabled = visible && IsLit;
            if (lightBarRoot != null) lightBarRoot.SetActive(_relayActive);
        }

        public void TickPassiveDrain(float deltaLight)
        {
            if (!_relayActive) return;
            _currentLight = Mathf.Max(0f, _currentLight - Mathf.Max(0f, deltaLight));
            RefreshVisual();
        }

        public void DrainByMoth(float deltaLight)
        {
            if (!_relayActive) return;
            _currentLight = Mathf.Max(0f, _currentLight - Mathf.Max(0f, deltaLight));
            RefreshVisual();
        }

        public void ExtinguishByGust()
        {
            if (!_relayActive) return;
            _currentLight = 0f;
            RefreshVisual();
        }

        public void RestoreLight(float deltaLight)
        {
            if (!_relayActive) return;
            _currentLight = Mathf.Min(100f, _currentLight + Mathf.Max(0f, deltaLight));
            RefreshVisual();
        }

        public void Relight()
        {
            if (!_relayActive) return;
            _currentLight = 100f;
            RefreshVisual();
            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                GameRoot.Instance.Audio.PlayUIClick();
            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.9f, "Đèn Sáng!", new Color(1f, 0.92f, 0.45f));
        }

        public void Interact() { Relight(); }
        public void HideLegacyPrompt() { }

        private void RefreshVisual()
        {
            if (spriteRenderer != null)
            {
                if (_currentLight > 55f && litSprite != null) spriteRenderer.sprite = litSprite;
                else if (_currentLight > 15f && dimSprite != null) spriteRenderer.sprite = dimSprite;
                else if (outSprite != null) spriteRenderer.sprite = outSprite;
            }
            if (glowRenderer != null)
            {
                glowRenderer.enabled = IsLit && (spriteRenderer == null || spriteRenderer.enabled);
                Color c = glowRenderer.color;
                c.a = Mathf.Clamp01(_currentLight / 100f) * 0.48f;
                glowRenderer.color = c;
            }
            if (lightBarFill != null)
            {
                float ratio = Mathf.Clamp01(_currentLight / 100f);
                lightBarFill.localScale = new Vector3(_barBaseScale.x * ratio, _barBaseScale.y, _barBaseScale.z);
            }
        }
    }
}
