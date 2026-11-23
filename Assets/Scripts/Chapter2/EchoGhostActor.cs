using UnityEngine;

namespace MoonpetalValley.Chapter2
{
    public class EchoGhostActor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] actionFrames;
        [SerializeField] private float idleFps = 4f;
        [SerializeField] private float actionFps = 7f;
        [SerializeField] private Color ghostTint = new Color(0.76f, 0.94f, 1.0f, 0.82f);

        private bool _playAction;
        private float _animTimer;
        private int _frameIndex;
        private float _shimmerPhase;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && idleFrames != null && idleFrames.Length > 0 && idleFrames[0] != null)
            {
                spriteRenderer.sprite = idleFrames[0];
            }
        }

        public void SetActionMode(bool playAction)
        {
            if (_playAction == playAction) return;
            _playAction = playAction;
            _frameIndex = 0;
            _animTimer = 0f;
        }

        private void Update()
        {
            if (spriteRenderer == null) return;
            Sprite[] frames = (_playAction && actionFrames != null && actionFrames.Length > 0) ? actionFrames : idleFrames;
            if (frames == null || frames.Length == 0) return;

            float fps = _playAction ? actionFps : idleFps;
            _animTimer += Time.unscaledDeltaTime;
            float step = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= step)
            {
                _animTimer -= step;
                _frameIndex = (_frameIndex + 1) % frames.Length;
            }
            int idx = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
            if (frames[idx] != null) spriteRenderer.sprite = frames[idx];

            _shimmerPhase += Time.unscaledDeltaTime * 3.2f;
            Color c = ghostTint;
            c.a = ghostTint.a * (0.82f + 0.18f * Mathf.Sin(_shimmerPhase));
            spriteRenderer.color = c;
        }
    }
}
