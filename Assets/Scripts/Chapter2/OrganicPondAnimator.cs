using UnityEngine;

namespace MoonpetalValley.Chapter2
{
    public class OrganicPondAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] frames;
        [SerializeField] private float fps = 3.5f;

        private SpriteRenderer _sr;
        private float _timer;
        private int _index;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_sr == null || frames == null || frames.Length == 0) return;
            _timer += Time.deltaTime;
            float interval = fps > 0.1f ? (1f / fps) : 0.3f;
            if (_timer >= interval)
            {
                _timer -= interval;
                _index = (_index + 1) % frames.Length;
                if (frames[_index] != null) _sr.sprite = frames[_index];
            }
        }
    }
}
