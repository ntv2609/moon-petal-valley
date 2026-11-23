using UnityEngine;

namespace MoonpetalValley.Chapter2
{
    public enum StormLanternVisualState
    {
        BrokenEast = 0,
        BrokenWest = 1,
        Lit = 2
    }

    public class StormLanternActor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private StormLanternVisualState initialState = StormLanternVisualState.BrokenEast;
        [SerializeField] private Sprite[] brokenEastFrames;
        [SerializeField] private Sprite[] brokenWestFrames;
        [SerializeField] private Sprite[] litFrames;
        [SerializeField] private float brokenFps = 3f;
        [SerializeField] private float litFps = 6f;

        private StormLanternVisualState _state;
        private float _animTimer;
        private int _frameIndex;

        public StormLanternVisualState CurrentState => _state;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            SetState(initialState);
        }

        public void SetState(StormLanternVisualState state)
        {
            _state = state;
            _frameIndex = 0;
            _animTimer = 0f;
            ApplyCurrentSprite();
        }

        private void Update()
        {
            Sprite[] frames = GetActiveFrames();
            if (frames == null || frames.Length == 0) return;

            float fps = _state == StormLanternVisualState.Lit ? litFps : brokenFps;
            _animTimer += Time.deltaTime;
            float step = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= step)
            {
                _animTimer -= step;
                _frameIndex = (_frameIndex + 1) % frames.Length;
                ApplyCurrentSprite();
            }
        }

        private Sprite[] GetActiveFrames()
        {
            return _state switch
            {
                StormLanternVisualState.BrokenWest => brokenWestFrames,
                StormLanternVisualState.Lit => litFrames,
                _ => brokenEastFrames
            };
        }

        private void ApplyCurrentSprite()
        {
            if (spriteRenderer == null) return;
            Sprite[] frames = GetActiveFrames();
            if (frames == null || frames.Length == 0) return;
            int idx = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
            if (frames[idx] != null) spriteRenderer.sprite = frames[idx];
        }
    }
}
