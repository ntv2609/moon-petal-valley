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
        [SerializeField] private Sprite[] litFrames;
        [SerializeField] private Sprite[] flickerFrames;
        [SerializeField] private Sprite[] offFrames;
        [SerializeField] private float animFps = 6f;
        [SerializeField] private GameObject lightBarRoot;
        [SerializeField] private Transform lightBarFill;
        [SerializeField] private int lanternIndex;
        [SerializeField] private float interactRadius = 2.35f;

        private float _currentLight = 100f;
        private bool _relayActive;
        private Vector3 _barBaseScale = Vector3.one;
        private float _animTimer;
        private int _animFrame;
        private int _lastAnimBucket = -1;

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
            _animTimer = (lanternIndex * 0.17f) % 0.6f;
        }

        private void OnEnable()
        {
            InteractionManager.Register(this);
            ApplyBridgeVisibility();
            RefreshVisual();
        }

        private void OnDisable()
        {
            InteractionManager.Unregister(this);
        }

        private void Update()
        {
            if (spriteRenderer == null || !spriteRenderer.enabled) return;

            int bucket = _currentLight > 55f ? 2 : (_currentLight > 15f ? 1 : 0);
            if (bucket != _lastAnimBucket)
            {
                _lastAnimBucket = bucket;
                _animFrame = 0;
                _animTimer = 0f;
                ApplyCurrentFrameSprite(bucket);
            }

            Sprite[] frames = GetFramesForBucket(bucket);
            if (frames != null && frames.Length > 1)
            {
                float fps = bucket == 1 ? animFps * 1.35f : (bucket == 0 ? animFps * 0.45f : animFps);
                _animTimer += Time.deltaTime;
                float step = 1f / Mathf.Max(1f, fps);
                if (_animTimer >= step)
                {
                    _animTimer -= step;
                    _animFrame = (_animFrame + 1) % frames.Length;
                    if (frames[_animFrame] != null) spriteRenderer.sprite = frames[_animFrame];
                }
            }

            if (glowRenderer != null && glowRenderer.enabled)
            {
                float pulse = bucket == 1
                    ? 0.72f + 0.28f * Mathf.Sin(Time.time * 13f + lanternIndex)
                    : 0.90f + 0.10f * Mathf.Sin(Time.time * 4.5f + lanternIndex);
                Color c = glowRenderer.color;
                c.a = Mathf.Clamp01(_currentLight / 100f) * 0.48f * pulse;
                glowRenderer.color = c;
            }
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
            if (LanternRelayManager.Instance != null) LanternRelayManager.Instance.NotifyLanternRelit(this);
        }

        public void Interact() { Relight(); }
        public void HideLegacyPrompt() { }

        private Sprite[] GetFramesForBucket(int bucket)
        {
            if (bucket == 2) return litFrames;
            if (bucket == 1) return flickerFrames;
            return offFrames;
        }

        private void ApplyCurrentFrameSprite(int bucket)
        {
            if (spriteRenderer == null) return;
            Sprite[] frames = GetFramesForBucket(bucket);
            if (frames != null && frames.Length > 0)
            {
                int idx = Mathf.Clamp(_animFrame, 0, frames.Length - 1);
                if (frames[idx] != null)
                {
                    spriteRenderer.sprite = frames[idx];
                    return;
                }
            }
            if (bucket == 2 && litSprite != null) spriteRenderer.sprite = litSprite;
            else if (bucket == 1 && dimSprite != null) spriteRenderer.sprite = dimSprite;
            else if (outSprite != null) spriteRenderer.sprite = outSprite;
        }

        private void RefreshVisual()
        {
            int bucket = _currentLight > 55f ? 2 : (_currentLight > 15f ? 1 : 0);
            _lastAnimBucket = bucket;
            ApplyCurrentFrameSprite(bucket);

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
