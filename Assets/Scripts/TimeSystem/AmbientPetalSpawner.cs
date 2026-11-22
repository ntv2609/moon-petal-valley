using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.TimeSystem
{
    public class AmbientPetalSpawner : MonoBehaviour
    {
        [SerializeField] private Sprite petalSprite;
        [SerializeField] private int particleCount = 18;

        private Transform[] _petals;
        private SpriteRenderer[] _renderers;
        private Vector2[] _speeds;
        private float[] _phases;
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            _petals = new Transform[particleCount];
            _renderers = new SpriteRenderer[particleCount];
            _speeds = new Vector2[particleCount];
            _phases = new float[particleCount];

            for (int i = 0; i < particleCount; i++)
            {
                GameObject go = new GameObject("AmbientPetal_" + i);
                go.transform.SetParent(transform, false);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = petalSprite;
                sr.sortingLayerName = "WeatherVFX";
                sr.sortingOrder = 5;

                float scale = Random.Range(0.45f, 0.95f);
                go.transform.localScale = new Vector3(scale, scale, 1f);

                _petals[i] = go.transform;
                _renderers[i] = sr;
                _speeds[i] = new Vector2(Random.Range(-0.55f, -0.18f), Random.Range(-0.35f, -0.12f));
                _phases[i] = Random.Range(0f, 6.28f);

                RespawnNearCamera(i, true);
            }
        }

        private void Update()
        {
            if (_cam == null) _cam = Camera.main;
            if (_cam == null || _petals == null) return;

            bool isNight = GameRoot.Instance != null && GameRoot.Instance.Time != null && GameRoot.Instance.Time.IsDuskOrNight;
            Color dayColor = new Color(1.0f, 0.84f, 0.92f, 0.72f);
            Color nightColor = new Color(0.68f, 0.98f, 0.88f, 0.85f);

            Vector3 camPos = _cam.transform.position;
            for (int i = 0; i < _petals.Length; i++)
            {
                _phases[i] += Time.deltaTime * 2.2f;
                Vector3 pos = _petals[i].position;
                pos.x += (_speeds[i].x + Mathf.Sin(_phases[i]) * 0.25f) * Time.deltaTime;
                pos.y += (_speeds[i].y + (isNight ? 0.35f : 0f)) * Time.deltaTime;
                _petals[i].position = pos;

                if (_renderers[i] != null)
                {
                    Color target = isNight ? nightColor : dayColor;
                    target.a *= 0.65f + Mathf.Sin(_phases[i] * 1.6f) * 0.35f;
                    _renderers[i].color = target;
                }

                if (Mathf.Abs(pos.x - camPos.x) > 12f || Mathf.Abs(pos.y - camPos.y) > 8f)
                {
                    RespawnNearCamera(i, false);
                }
            }
        }

        private void RespawnNearCamera(int idx, bool randomAllOver)
        {
            Vector3 c = _cam != null ? _cam.transform.position : Vector3.zero;
            float rx = randomAllOver ? Random.Range(-11f, 11f) : Random.Range(-8f, 11.5f);
            float ry = randomAllOver ? Random.Range(-7f, 7f) : Random.Range(-6f, 7.5f);
            _petals[idx].position = new Vector3(c.x + rx, c.y + ry, 0f);
        }
    }
}
