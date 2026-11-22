using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Player
{
    public class LumiCompanionController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer haloRenderer;
        [SerializeField] private Vector2 followOffset = new Vector2(-0.75f, 0.75f);
        [SerializeField] private float smoothSpeed = 6.5f;

        [Header("Pixel Art Animations (FullRect)")]
        [SerializeField] private Sprite[] floatFrames;
        [SerializeField] private Sprite[] pulseFrames;
        [SerializeField] private float floatFps = 6f;
        [SerializeField] private float pulseFps = 8f;
        [SerializeField] private float autoPulseInterval = 4.5f;

        private SpriteRenderer _shadowRenderer;
        private float _bobTime;
        private float _animTimer;
        private int _frameIdx;
        private float _pulseRemaining;
        private float _autoPulseCooldown = 2.5f;
        private Vector3 _lastPos;

        private bool _isAwakened;
        private bool _isPlayingAwakenAnim;
        private float _awakenTimer;
        private Vector3 _awakenStartPos;
        private const float AwakenDuration = 1.15f;

        private void Awake()
        {
            if (spriteRenderer == null && visualTransform != null)
                spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
            Transform sh = transform.Find("GroundShadow");
            if (sh != null) _shadowRenderer = sh.GetComponent<SpriteRenderer>();
            _lastPos = transform.position;
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded;
        }

        private void Start()
        {
            HandleSaveLoaded();
        }

        private void HandleSaveLoaded()
        {
            bool alreadyAwake = GameRoot.Instance != null &&
                                GameRoot.Instance.Flags != null &&
                                GameRoot.Instance.Flags.HasFlag("flag_lumi_awakened");
            _isAwakened = alreadyAwake;
            _isPlayingAwakenAnim = false;
            SetVisualsVisible(_isAwakened);

            bool bloomed = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                           GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            SetHaloRadiusScale(bloomed ? 3.2f : 1.0f);
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (flagId != "flag_lumi_awakened") return;

            if (value)
            {
                if (!_isAwakened && !_isPlayingAwakenAnim)
                {
                    _isAwakened = true;
                    SetVisualsVisible(true);
                    TriggerPulse(1.0f);
                }
            }
            else
            {
                _isAwakened = false;
                _isPlayingAwakenAnim = false;
                SetVisualsVisible(false);
            }
        }

        public void PlayAwakenFromBoxAnimation(Vector3 boxWorldPos)
        {
            _isAwakened = true;
            _isPlayingAwakenAnim = true;
            _awakenTimer = 0f;
            _awakenStartPos = boxWorldPos + new Vector3(0f, 0.15f, 0f);

            transform.position = _awakenStartPos;
            SetVisualsVisible(true);
            if (_shadowRenderer != null) _shadowRenderer.enabled = false;

            if (visualTransform != null)
            {
                visualTransform.localPosition = Vector3.zero;
                visualTransform.localScale = new Vector3(0.15f, 0.15f, 1f);
            }

            TriggerPulse(1.4f);
            JuiceEffectsManager.Instance?.TriggerCameraShake(0.07f, 0.18f);
        }

        private void SetVisualsVisible(bool visible)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = visible;
            if (haloRenderer != null) haloRenderer.enabled = visible;
            if (_shadowRenderer != null) _shadowRenderer.enabled = visible;
        }

        private void LateUpdate()
        {
            if (!_isAwakened && !_isPlayingAwakenAnim) return;

            if (followTarget == null)
            {
                var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (player != null) followTarget = player.transform;
                else return;
            }

            if (_isPlayingAwakenAnim)
            {
                UpdateAwakenAnimation();
                UpdateSpriteAnimation(Time.unscaledDeltaTime);
                return;
            }

            Vector3 desired = followTarget.position + new Vector3(followOffset.x, followOffset.y, 0f);
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smoothSpeed);

            float dx = transform.position.x - _lastPos.x;
            if (spriteRenderer != null && Mathf.Abs(dx) > 0.002f)
                spriteRenderer.flipX = dx < 0f;
            _lastPos = transform.position;

            _bobTime += Time.deltaTime * 3.2f;
            if (visualTransform != null)
            {
                visualTransform.localPosition = new Vector3(0f, Mathf.Sin(_bobTime) * 0.14f, 0f);
                visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, Vector3.one, Time.deltaTime * 10f);
            }

            if (haloRenderer != null)
            {
                float boost = _pulseRemaining > 0f ? 0.22f : 0f;
                float pulse = 0.85f + boost + Mathf.Sin(_bobTime * 1.5f) * 0.15f;
                haloRenderer.transform.localScale = new Vector3(pulse, pulse, 1f);
            }

            if (_pulseRemaining > 0f)
            {
                _pulseRemaining -= Time.deltaTime;
            }
            else if (autoPulseInterval > 0f && pulseFrames != null && pulseFrames.Length > 0)
            {
                _autoPulseCooldown -= Time.deltaTime;
                if (_autoPulseCooldown <= 0f)
                {
                    TriggerPulse(0.65f);
                    _autoPulseCooldown = autoPulseInterval;
                }
            }

            UpdateSpriteAnimation(Time.deltaTime);
        }

        private void UpdateAwakenAnimation()
        {
            float dt = Time.unscaledDeltaTime;
            _awakenTimer += dt;
            float t = Mathf.Clamp01(_awakenTimer / AwakenDuration);
            Vector3 targetShoulder = followTarget.position + new Vector3(followOffset.x, followOffset.y, 0f);

            if (t < 0.55f)
            {
                float p = t / 0.55f;
                float easeOut = 1f - Mathf.Pow(1f - p, 3f);
                float swirlX = Mathf.Sin(p * Mathf.PI * 2.5f) * 0.28f * (1f - p * 0.4f);
                float riseY = easeOut * 1.15f;
                transform.position = _awakenStartPos + new Vector3(swirlX, riseY, 0f);

                float sc = p < 0.65f
                    ? Mathf.Lerp(0.15f, 1.28f, p / 0.65f)
                    : Mathf.Lerp(1.28f, 1.05f, (p - 0.65f) / 0.35f);
                if (visualTransform != null)
                {
                    visualTransform.localScale = new Vector3(sc, sc, 1f);
                    visualTransform.localPosition = Vector3.zero;
                }
                if (haloRenderer != null)
                {
                    float hScale = Mathf.Lerp(0.2f, 2.35f, Mathf.Sin(p * Mathf.PI));
                    haloRenderer.transform.localScale = new Vector3(Mathf.Max(0.9f, hScale), Mathf.Max(0.9f, hScale), 1f);
                }
                if (spriteRenderer != null)
                {
                    Color c = Color.white;
                    c.a = Mathf.Clamp01(p * 2.6f);
                    spriteRenderer.color = c;
                }
            }
            else
            {
                float p = (t - 0.55f) / 0.45f;
                float smooth = p * p * (3f - 2f * p);
                Vector3 apex = _awakenStartPos + new Vector3(0f, 1.15f, 0f);
                transform.position = Vector3.Lerp(apex, targetShoulder, smooth);

                if (visualTransform != null)
                    visualTransform.localScale = new Vector3(Mathf.Lerp(1.05f, 1.0f, smooth), Mathf.Lerp(1.05f, 1.0f, smooth), 1f);
                if (haloRenderer != null)
                {
                    float hScale = Mathf.Lerp(1.5f, 1.0f, smooth);
                    haloRenderer.transform.localScale = new Vector3(hScale, hScale, 1f);
                }
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
            }

            if (_awakenTimer >= AwakenDuration)
            {
                _isPlayingAwakenAnim = false;
                if (visualTransform != null) visualTransform.localScale = Vector3.one;
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
                if (_shadowRenderer != null) _shadowRenderer.enabled = true;
                _lastPos = transform.position;
            }
        }

        private void UpdateSpriteAnimation(float dt)
        {
            if (spriteRenderer == null) return;
            bool isPulsing = (_pulseRemaining > 0f || _isPlayingAwakenAnim) && pulseFrames != null && pulseFrames.Length > 0;
            Sprite[] active = isPulsing ? pulseFrames : floatFrames;
            if (active == null || active.Length == 0) return;

            float fps = isPulsing ? pulseFps : floatFps;
            _animTimer += dt;
            float interval = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= interval)
            {
                _animTimer -= interval;
                _frameIdx = (_frameIdx + 1) % active.Length;
            }
            int safeIdx = _frameIdx % active.Length;
            if (active[safeIdx] != null) spriteRenderer.sprite = active[safeIdx];
        }

        public void PlayMoonPulseAnimation(float duration = 0.7f) => TriggerPulse(duration);

        public void TriggerPulse(float duration = 0.7f)
        {
            _pulseRemaining = Mathf.Max(_pulseRemaining, duration);
            _frameIdx = 0;
            _animTimer = 0f;
        }

        public void SetHaloRadiusScale(float scale)
        {
            if (haloRenderer != null)
                haloRenderer.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
