using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Combat;

namespace MoonpetalValley.Chapter2
{
    public class BridgeLanternPost : MonoBehaviour, IInteractable
    {
        [Header("Visuals (actor_bridge_lantern, PPU 24)")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer glowRenderer;
        [SerializeField] private Sprite litSprite;
        [SerializeField] private Sprite dimSprite;
        [SerializeField] private Sprite outSprite;
        [SerializeField] private Transform lightBarFill;
        [SerializeField] private GameObject lightBarRoot;

        [Header("Settings")]
        [SerializeField] private int lanternIndex;
        [SerializeField] private float interactRadius = 2.1f;

        private float _currentLight = 100f;
        private float _mothWarningTimer;
        private bool _relayActive;
        private Vector3 _barBaseScale = new Vector3(1.1f, 0.12f, 1f);

        public float CurrentLight => _currentLight;
        public bool IsLit => _currentLight > 15f;
        public int LanternIndex => lanternIndex;

        public bool CanInteract => _relayActive && _currentLight < 92f;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + Vector3.up * 1.15f;
        public string InteractionPromptLabel => _currentLight <= 15f ? "Thắp Lại Đèn Cầu" : "Mồi Thêm Lửa";

        private void Awake() { if (lightBarFill != null) _barBaseScale = lightBarFill.localScale; }
        private void OnEnable() => InteractionManager.Register(this);
        private void OnDisable() => InteractionManager.Unregister(this);
        private void Start() { ApplyBridgeVisibility(); }

        public void SetRelayActive(bool active, float initialLight = 100f)
        {
            _relayActive = active;
            _currentLight = Mathf.Clamp(initialLight, 0f, 100f);
            _mothWarningTimer = 0f;
            if (lightBarRoot != null) lightBarRoot.SetActive(active);
            ApplyBridgeVisibility();
            RefreshVisual();
        }

        public void ApplyBridgeVisibility()
        {
            RiverBridgeState st = Chapter2StateManager.Instance != null
                ? Chapter2StateManager.Instance.BridgeState : RiverBridgeState.Broken;
            bool visible = _relayActive || st == RiverBridgeState.RelayInProgress || st == RiverBridgeState.Repaired;
            if (spriteRenderer != null) spriteRenderer.enabled = visible;
            if (glowRenderer != null) glowRenderer.enabled = visible && (IsLit || st == RiverBridgeState.Repaired);
            if (lightBarRoot != null) lightBarRoot.SetActive(_relayActive);
            if (st == RiverBridgeState.Repaired && !_relayActive) { _currentLight = 100f; RefreshVisual(); }
        }

        public void TickPassiveDrain(float deltaLight)
        {
            if (!_relayActive || _currentLight <= 0f) return;
            _currentLight = Mathf.Max(0f, _currentLight - deltaLight);
            if (_mothWarningTimer > 0f) _mothWarningTimer -= Time.deltaTime;
            RefreshVisual();
        }

        public void DrainByMoth(float deltaLight)
        {
            if (!_relayActive || _currentLight <= 0f) return;
            _currentLight = Mathf.Max(0f, _currentLight - deltaLight);
            _mothWarningTimer = 0.25f;
            RefreshVisual();
        }

        public void Relight()
        {
            if (!_relayActive) return;
            bool wasOut = !IsLit;
            _currentLight = 100f;
            _mothWarningTimer = 0f;
            RefreshVisual();
            if (JuiceEffectsManager.Instance != null)
            {
                string label = wasOut ? "Đã thắp lại đèn!" : "Mồi đèn đầy!";
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.9f, label, new Color(1.0f, 0.90f, 0.45f));
            }
        }

        public void Interact() { Relight(); }
        public void HideLegacyPrompt() { }

        private void RefreshVisual()
        {
            if (spriteRenderer != null)
            {
                if (_currentLight > 45f && litSprite != null) spriteRenderer.sprite = litSprite;
                else if (_currentLight > 15f && dimSprite != null) spriteRenderer.sprite = dimSprite;
                else if (outSprite != null) spriteRenderer.sprite = outSprite;
                else if (litSprite != null) spriteRenderer.sprite = litSprite;

                if (_mothWarningTimer > 0f) spriteRenderer.color = Color.Lerp(Color.white, new Color(0.80f, 0.55f, 1.0f, 1f), 0.55f);
                else if (_currentLight <= 15f) spriteRenderer.color = new Color(0.55f, 0.58f, 0.68f, 1f);
                else if (_currentLight <= 45f) spriteRenderer.color = new Color(0.92f, 0.85f, 0.72f, 1f);
                else spriteRenderer.color = Color.white;

                spriteRenderer.sortingLayerName = "WorldObjects";
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-(transform.position.y - 0.3f) * 100f);
            }

            if (glowRenderer != null)
            {
                float ratio = Mathf.Clamp01(_currentLight / 100f);
                bool showGlow = spriteRenderer != null && spriteRenderer.enabled && ratio > 0.15f;
                glowRenderer.enabled = showGlow;
                if (showGlow)
                {
                    float flicker = 0.92f + 0.08f * Mathf.Sin(Time.time * 7f + lanternIndex * 1.3f);
                    glowRenderer.color = new Color(1.0f, 0.86f, 0.42f, Mathf.Lerp(0.12f, 0.48f, ratio) * flicker);
                    float s = Mathf.Lerp(1.1f, 2.3f, ratio) * flicker;
                    glowRenderer.transform.localScale = new Vector3(s, s, 1f);
                }
            }

            if (lightBarFill != null)
            {
                float ratio = Mathf.Clamp01(_currentLight / 100f);
                lightBarFill.localScale = new Vector3(_barBaseScale.x * ratio, _barBaseScale.y, 1f);
                SpriteRenderer barSr = lightBarFill.GetComponent<SpriteRenderer>();
                if (barSr != null)
                {
                    if (ratio <= 0.15f) barSr.color = new Color(0.95f, 0.25f, 0.25f, 1f);
                    else if (ratio <= 0.45f) barSr.color = new Color(1.0f, 0.72f, 0.22f, 1f);
                    else barSr.color = new Color(1.0f, 0.92f, 0.48f, 1f);
                }
            }
        }
    }
}
