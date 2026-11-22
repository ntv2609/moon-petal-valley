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

        [Header("Animation Frames")]
        [SerializeField] private Sprite[] floatFrames;
        [SerializeField] private Sprite[] moonPulseFrames;
        [SerializeField] private float floatFps = 7f;
        [SerializeField] private float pulseFps = 11f;

        private float _bobTime;
        private float _animTimer;
        private int _frameIndex;
        private bool _isPlayingPulse;
        private float _haloBaseScale = 1.0f;

        public void PlayMoonPulseAnimation()
        {
            if (moonPulseFrames == null || moonPulseFrames.Length == 0) return;
            _isPlayingPulse = true;
            _frameIndex = 0;
            _animTimer = 0f;
            if (spriteRenderer != null && moonPulseFrames[0] != null)
            {
                spriteRenderer.sprite = moonPulseFrames[0];
            }
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

            _bobTime += Time.deltaTime * 3.2f;
            if (visualTransform != null)
            {
                visualTransform.localPosition = new Vector3(0f, Mathf.Sin(_bobTime) * 0.12f, 0f);
            }

            if (haloRenderer != null)
            {
                float pulse = _haloBaseScale * (0.88f + Mathf.Sin(_bobTime * 1.5f) * 0.12f);
                haloRenderer.transform.localScale = new Vector3(pulse, pulse, 1f);
            }

            TickSpriteAnimation(Time.deltaTime);
        }

        private void TickSpriteAnimation(float dt)
        {
            if (spriteRenderer == null) return;

            Sprite[] active = _isPlayingPulse ? moonPulseFrames : floatFrames;
            if (active == null || active.Length == 0)
            {
                _isPlayingPulse = false;
                return;
            }

            float fps = _isPlayingPulse ? pulseFps : floatFps;
            float dur = fps > 0.01f ? (1f / fps) : 0.14f;
            _animTimer += dt;

            if (_animTimer >= dur)
            {
                _animTimer -= dur;
                _frameIndex++;
                if (_frameIndex >= active.Length)
                {
                    if (_isPlayingPulse)
                    {
                        _isPlayingPulse = false;
                        _frameIndex = 0;
                        active = floatFrames;
                    }
                    else
                    {
                        _frameIndex = 0;
                    }
                }
            }

            if (active != null && active.Length > 0)
            {
                int idx = Mathf.Clamp(_frameIndex, 0, active.Length - 1);
                if (active[idx] != null) spriteRenderer.sprite = active[idx];
            }
        }

        public void SetHaloRadiusScale(float scale)
        {
            _haloBaseScale = Mathf.Max(0.5f, scale);
            if (haloRenderer != null)
            {
                haloRenderer.transform.localScale = new Vector3(_haloBaseScale, _haloBaseScale, 1f);
            }
        }
    }
}
