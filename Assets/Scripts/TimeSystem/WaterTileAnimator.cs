using UnityEngine;

namespace MoonpetalValley.TimeSystem
{
    public class WaterTileAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] waterFrames;
        [SerializeField] private SpriteRenderer[] waterRenderers;
        [SerializeField] private float fps = 3.2f;

        private float _timer;
        private int _frameIndex;

        private void Update()
        {
            if (waterFrames == null || waterFrames.Length == 0) return;
            if (waterRenderers == null || waterRenderers.Length == 0) return;

            _timer += Time.deltaTime;
            float interval = 1f / Mathf.Max(0.5f, fps);
            if (_timer >= interval)
            {
                _timer -= interval;
                _frameIndex = (_frameIndex + 1) % waterFrames.Length;
                Sprite current = waterFrames[_frameIndex];
                for (int i = 0; i < waterRenderers.Length; i++)
                {
                    if (waterRenderers[i] != null)
                    {
                        waterRenderers[i].sprite = current;
                    }
                }
            }
        }
    }
}
