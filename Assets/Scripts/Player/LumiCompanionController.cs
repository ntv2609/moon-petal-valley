using UnityEngine;

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

        private float _bobTime;
        private float _animTimer;
        private int _frameIdx;
        private float _pulseRemaining;
        private float _autoPulseCooldown = 2.5f;
        private Vector3 _lastPos;

        private void Awake()
        {
            if (spriteRenderer == null && visualTransform != null)
            {
                spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
            }
            _lastPos = transform.position;
        }

        private void LateUpdate()
        {
            if (followTarget == null)
            {
                var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (player != null) followTarget = player.transform;
                else return;
            }

            Vector3 desired = followTarget.position + new Vector3(followOffset.x, followOffset.y, 0f);
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smoothSpeed);

            float dx = transform.position.x - _lastPos.x;
            if (spriteRenderer != null && Mathf.Abs(dx) > 0.002f)
            {
                spriteRenderer.flipX = dx < 0f;
            }
            _lastPos = transform.position;

            _bobTime += Time.deltaTime * 3.2f;
            if (visualTransform != null)
            {
                visualTransform.localPosition = new Vector3(0f, Mathf.Sin(_bobTime) * 0.14f, 0f);
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

            UpdateSpriteAnimation();
        }

        private void UpdateSpriteAnimation()
        {
            if (spriteRenderer == null) return;

            bool isPulsing = _pulseRemaining > 0f && pulseFrames != null && pulseFrames.Length > 0;
            Sprite[] active = isPulsing ? pulseFrames : floatFrames;
            if (active == null || active.Length == 0) return;

            float fps = isPulsing ? pulseFps : floatFps;
            _animTimer += Time.deltaTime;
            float interval = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= interval)
            {
                _animTimer -= interval;
                _frameIdx = (_frameIdx + 1) % active.Length;
            }

            int safeIdx = _frameIdx % active.Length;
            if (active[safeIdx] != null)
            {
                spriteRenderer.sprite = active[safeIdx];
            }
        }

        public void PlayMoonPulseAnimation(float duration = 0.7f)
        {
            TriggerPulse(duration);
        }

        public void TriggerPulse(float duration = 0.7f)
        {
            _pulseRemaining = Mathf.Max(_pulseRemaining, duration);
            _frameIdx = 0;
            _animTimer = 0f;
        }

        public void SetHaloRadiusScale(float scale)
        {
            if (haloRenderer != null)
            {
                haloRenderer.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
    }
}
