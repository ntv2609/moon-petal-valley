using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2
{
    /// <summary>
    /// He thong khong khi Dam Lay am thap, uot at & sac net:
    /// 1. Tam man hinh trong treo 100% (centerDarknessAlpha = 0) quanh Wen/Lumi de chien dau ro net o moi khung gio.
    /// 2. Vien tan cay toi diu mat (edgeDarknessAlpha = 0.34) tao chieu sau rung sau khong bi bet mau.
    /// 3. Mua phun nhe + vong song nuoc lan tren mat dat uot + bao tu phat sang lo lung.
    /// </summary>
    public class SwampAtmosphereController : MonoBehaviour
    {
        [SerializeField] private Color gloomTint = new Color(0.03f, 0.09f, 0.12f, 1f);
        [SerializeField] private float edgeDarknessAlpha = 0.34f;
        [SerializeField] private float centerDarknessAlpha = 0.00f;
        [SerializeField] private float overlayScale = 34f;

        private SpriteRenderer _overlaySr;
        private Transform _followTarget;
        private Camera _mainCam;

        private struct DrizzleDrop { public Transform tr; public SpriteRenderer sr; public float speed; public float life; public float maxLife; }
        private struct GroundRipple { public Transform tr; public SpriteRenderer sr; public float t; public float dur; }
        private struct DampSpore { public Transform tr; public SpriteRenderer sr; public Vector3 baseOffset; public float phase; public float speed; }

        private readonly List<DrizzleDrop> _drops = new List<DrizzleDrop>();
        private readonly List<GroundRipple> _ripples = new List<GroundRipple>();
        private readonly List<DampSpore> _spores = new List<DampSpore>();

        private void Awake()
        {
            BuildVignetteOverlay();
            BuildWetWeatherParticles();
        }

        private void Start()
        {
            FindTargets();
        }

        private void FindTargets()
        {
            if (_followTarget == null)
            {
                PlayerController2D pc = Object.FindAnyObjectByType<PlayerController2D>();
                if (pc != null) _followTarget = pc.transform;
            }
            if (_mainCam == null) _mainCam = Camera.main;
        }

        private void BuildVignetteOverlay()
        {
            Transform existing = transform.Find("SwampCanopyDarkness");
            GameObject go = existing != null ? existing.gameObject : new GameObject("SwampCanopyDarkness");
            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.one * overlayScale;

            _overlaySr = go.GetComponent<SpriteRenderer>();
            if (_overlaySr == null) _overlaySr = go.AddComponent<SpriteRenderer>();

            const int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            Color[] pixels = new Color[size * size];
            float half = (size - 1) * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - half) / half;
                    float dy = (y - half) / half;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    // Vung trong treo rong (32% ban kinh) quanh Wen & Lumi, toi dan diu mat ve mep man hinh
                    float t = Mathf.SmoothStep(0.32f, 0.96f, dist);
                    float a = Mathf.Lerp(centerDarknessAlpha, edgeDarknessAlpha, t);
                    pixels[y * size + x] = new Color(gloomTint.r, gloomTint.g, gloomTint.b, a);
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();

            Sprite spr = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            _overlaySr.sprite = spr;
            _overlaySr.color = Color.white;
            _overlaySr.sortingLayerName = "WorldObjects";
            _overlaySr.sortingOrder = 320;
        }

        private void BuildWetWeatherParticles()
        {
            Sprite streakSpr = MakeStreakSprite();
            Sprite ringSpr = MakeRingSprite();
            Sprite dotSpr = MakeDotSprite();

            GameObject fxRoot = new GameObject("SwampWetFX");
            fxRoot.transform.SetParent(transform, false);

            // 26 hat mua phun nghieng nhe
            for (int i = 0; i < 26; i++)
            {
                GameObject g = new GameObject($"Drizzle_{i}");
                g.transform.SetParent(fxRoot.transform, false);
                g.transform.rotation = Quaternion.Euler(0f, 0f, -14f);
                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = streakSpr;
                sr.color = new Color(0.72f, 0.92f, 0.90f, 0.34f);
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = 305;
                _drops.Add(new DrizzleDrop
                {
                    tr = g.transform,
                    sr = sr,
                    speed = Random.Range(7.5f, 11.5f),
                    life = Random.Range(0f, 1.2f),
                    maxLife = Random.Range(0.75f, 1.25f)
                });
            }

            // 12 vong song nuoc lan tren mat dat am uot
            for (int i = 0; i < 12; i++)
            {
                GameObject g = new GameObject($"Ripple_{i}");
                g.transform.SetParent(fxRoot.transform, false);
                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = ringSpr;
                sr.color = new Color(0.74f, 0.96f, 0.92f, 0.42f);
                sr.sortingLayerName = "GroundDecor";
                sr.sortingOrder = 18;
                _ripples.Add(new GroundRipple
                {
                    tr = g.transform,
                    sr = sr,
                    t = Random.Range(0f, 1.4f),
                    dur = Random.Range(0.95f, 1.55f)
                });
            }

            // 14 bao tu am phat sang nhe lo lung
            for (int i = 0; i < 14; i++)
            {
                GameObject g = new GameObject($"Spore_{i}");
                g.transform.SetParent(fxRoot.transform, false);
                g.transform.localScale = Vector3.one * Random.Range(0.22f, 0.38f);
                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = dotSpr;
                sr.color = new Color(0.62f, 1.0f, 0.78f, 0.48f);
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = 290;
                _spores.Add(new DampSpore
                {
                    tr = g.transform,
                    sr = sr,
                    baseOffset = new Vector3(Random.Range(-11f, 11f), Random.Range(-6.5f, 6.5f), 0f),
                    phase = Random.Range(0f, 6.28f),
                    speed = Random.Range(0.6f, 1.3f)
                });
            }
        }

        private void LateUpdate()
        {
            if (_followTarget == null || _mainCam == null) FindTargets();
            Vector3 anchor = _followTarget != null
                ? _followTarget.position + new Vector3(0f, 0.3f, 0f)
                : (_mainCam != null ? _mainCam.transform.position : Vector3.zero);

            if (_overlaySr != null)
                _overlaySr.transform.position = new Vector3(anchor.x, anchor.y, 0f);

            float dt = Time.deltaTime;
            // Cap nhat mua phun
            for (int i = 0; i < _drops.Count; i++)
            {
                var d = _drops[i];
                d.life += dt;
                if (d.life >= d.maxLife)
                {
                    d.life = 0f;
                    d.tr.position = new Vector3(anchor.x + Random.Range(-13f, 13f), anchor.y + Random.Range(-3f, 9f), 0f);
                }
                else
                {
                    d.tr.position += new Vector3(-0.22f, -1f, 0f) * (d.speed * dt);
                }
                float alpha = Mathf.Sin((d.life / d.maxLife) * Mathf.PI) * 0.34f;
                d.sr.color = new Color(0.72f, 0.92f, 0.90f, alpha);
                _drops[i] = d;
            }

            // Cap nhat vong song nuoc lan tren mat dat
            for (int i = 0; i < _ripples.Count; i++)
            {
                var r = _ripples[i];
                r.t += dt;
                if (r.t >= r.dur)
                {
                    r.t = 0f;
                    r.tr.position = new Vector3(anchor.x + Random.Range(-10.5f, 10.5f), anchor.y + Random.Range(-6.0f, 6.0f), 0f);
                }
                float p = Mathf.Clamp01(r.t / r.dur);
                float sx = Mathf.Lerp(0.18f, 0.85f, p);
                r.tr.localScale = new Vector3(sx, sx * 0.62f, 1f);
                r.sr.color = new Color(0.72f, 0.96f, 0.92f, (1f - p) * 0.42f);
                _ripples[i] = r;
            }

            // Cap nhat bao tu lo lung
            float now = Time.time;
            for (int i = 0; i < _spores.Count; i++)
            {
                var s = _spores[i];
                float ox = Mathf.Sin(now * s.speed + s.phase) * 0.7f;
                float oy = Mathf.Cos(now * (s.speed * 0.8f) + s.phase) * 0.55f;
                s.tr.position = new Vector3(anchor.x + s.baseOffset.x + ox, anchor.y + s.baseOffset.y + oy, 0f);
                float a = 0.22f + 0.32f * (0.5f + 0.5f * Mathf.Sin(now * 2.1f + s.phase));
                s.sr.color = new Color(0.62f, 1.0f, 0.78f, a);
            }
        }

        private static Sprite MakeStreakSprite()
        {
            Texture2D t = new Texture2D(2, 12, TextureFormat.RGBA32, false);
            t.filterMode = FilterMode.Point;
            for (int y = 0; y < 12; y++)
            {
                float a = (float)y / 11f;
                Color c = new Color(1f, 1f, 1f, a);
                t.SetPixel(0, y, c);
                t.SetPixel(1, y, new Color(1f, 1f, 1f, a * 0.5f));
            }
            t.Apply();
            return Sprite.Create(t, new Rect(0, 0, 2, 12), new Vector2(0.5f, 0.5f), 16f);
        }

        private static Sprite MakeRingSprite()
        {
            const int s = 24;
            Texture2D t = new Texture2D(s, s, TextureFormat.RGBA32, false);
            t.filterMode = FilterMode.Point;
            float h = (s - 1) * 0.5f;
            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float d = Mathf.Sqrt(((x - h) / h) * ((x - h) / h) + ((y - h) / h) * ((y - h) / h));
                    bool ring = d >= 0.76f && d <= 0.94f;
                    t.SetPixel(x, y, ring ? Color.white : Color.clear);
                }
            }
            t.Apply();
            return Sprite.Create(t, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 16f);
        }

        private static Sprite MakeDotSprite()
        {
            const int s = 12;
            Texture2D t = new Texture2D(s, s, TextureFormat.RGBA32, false);
            t.filterMode = FilterMode.Bilinear;
            float h = (s - 1) * 0.5f;
            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float d = Mathf.Sqrt(((x - h) / h) * ((x - h) / h) + ((y - h) / h) * ((y - h) / h));
                    t.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(1f - d)));
                }
            }
            t.Apply();
            return Sprite.Create(t, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 16f);
        }
    }
}
