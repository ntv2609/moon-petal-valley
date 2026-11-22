using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Community
{
    public class MoonpetalBloomVFX : MonoBehaviour
    {
        [SerializeField] private int sparkleCount = 8;
        [SerializeField] private float orbitRadius = 1.05f;

        private readonly List<Transform> _sparkles = new List<Transform>();
        private readonly List<SpriteRenderer> _sparkleRenderers = new List<SpriteRenderer>();
        private readonly List<float> _phases = new List<float>();
        private bool _isBloomed;
        private Sprite _dotSprite;

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += RefreshState;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= RefreshState;
        }

        private void Start()
        {
            BuildSparkles();
            RefreshState();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == "flag_nv1_completed") RefreshState();
        }

        private void RefreshState()
        {
            BuildSparkles();
            _isBloomed = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                         GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            for (int i = 0; i < _sparkleRenderers.Count; i++)
            {
                if (_sparkleRenderers[i] != null) _sparkleRenderers[i].enabled = _isBloomed;
            }
        }

        private void Update()
        {
            if (!_isBloomed) return;
            float t = Time.time;
            for (int i = 0; i < _sparkles.Count; i++)
            {
                if (_sparkles[i] == null) continue;
                float p = _phases[i];
                float ang = t * 0.65f + p;
                float r = orbitRadius + Mathf.Sin(t * 1.6f + p * 2f) * 0.22f;
                float x = Mathf.Cos(ang) * r;
                float y = Mathf.Sin(ang) * r * 0.72f + 0.18f + Mathf.Sin(t * 2.4f + p) * 0.12f;
                _sparkles[i].localPosition = new Vector3(x, y, 0f);

                float pulse = 0.55f + 0.45f * Mathf.Sin(t * 3.4f + p * 3f);
                float scale = 0.16f + pulse * 0.14f;
                _sparkles[i].localScale = new Vector3(scale, scale, 1f);

                if (_sparkleRenderers[i] != null)
                {
                    Color c = _sparkleRenderers[i].color;
                    c.a = 0.35f + pulse * 0.65f;
                    _sparkleRenderers[i].color = c;
                }
            }
        }

        private void BuildSparkles()
        {
            if (_sparkles.Count > 0) return;
            _dotSprite = CreateGlowDotSprite();

            for (int i = 0; i < sparkleCount; i++)
            {
                GameObject go = new GameObject("MoonpetalSparkle_" + i);
                go.transform.SetParent(transform, false);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = _dotSprite;
                sr.sortingLayerName = "WeatherVFX";
                sr.sortingOrder = 12;
                sr.color = (i % 2 == 0)
                    ? new Color(0.68f, 0.94f, 1.0f, 0.85f)
                    : new Color(1.0f, 0.94f, 0.62f, 0.85f);
                sr.enabled = false;

                _sparkles.Add(go.transform);
                _sparkleRenderers.Add(sr);
                _phases.Add((Mathf.PI * 2f / sparkleCount) * i);
            }
        }

        private Sprite CreateGlowDotSprite()
        {
            const int s = 16;
            Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float half = (s - 1) * 0.5f;
            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float dx = (x - half) / half;
                    float dy = (y - half) / half;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(1f - d);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 16f);
        }
    }
}
