using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.TimeSystem
{
    public class VillageLantern : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer glowRenderer;

        private float _targetAlpha;
        private float _flickerPhase;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
        }

        private void Start()
        {
            _flickerPhase = Random.Range(0f, 6.28f);
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
            {
                HandleTimeChanged(GameRoot.Instance.Time.CurrentDay, GameRoot.Instance.Time.CurrentHourFloat);
            }
        }

        private void HandleTimeChanged(int day, float hour)
        {
            _targetAlpha = hour >= 18f ? 0.78f : 0f;
        }

        private void Update()
        {
            if (glowRenderer == null) return;
            _flickerPhase += Time.deltaTime * 4f;
            float flicker = _targetAlpha > 0.01f ? (Mathf.Sin(_flickerPhase) * 0.06f) : 0f;
            Color c = glowRenderer.color;
            c.a = Mathf.Lerp(c.a, Mathf.Clamp01(_targetAlpha + flicker), Time.deltaTime * 4f);
            glowRenderer.color = c;
        }
    }
}
