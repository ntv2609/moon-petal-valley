using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Combat
{
    public class BlightFogZone : MonoBehaviour
    {
        [SerializeField] private string clearOnFlag = "flag_nv1_completed";
        [SerializeField] private SpriteRenderer fogRenderer;

        private bool _isClearing;
        private float _driftPhase;
        private Vector3 _startPos;
        private Vector3 _baseScale;
        private Transform[] _spores;
        private SpriteRenderer[] _sporeSrs;
        private float[] _sporePhases;

        private static Sprite _sharedWispFogSprite;
        private static Sprite _sharedSporeSprite;

        private void Awake()
        {
            _startPos = transform.position;
            _baseScale = transform.localScale;
            _driftPhase = Random.Range(0f, 6.28f);
            if (fogRenderer == null) fogRenderer = GetComponent<SpriteRenderer>();

            EnsureRichFogVisual();
            SpawnDriftingBlightSpores();
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded_Step40;
        }
        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded_Step40;
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                GameRoot.Instance.Flags.HasFlag(clearOnFlag))
            {
                gameObject.SetActive(false);
            }
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value && flagId == clearOnFlag) _isClearing = true;
        }

        private void EnsureRichFogVisual()
        {
            if (fogRenderer == null) return;
            if (_sharedWispFogSprite == null) _sharedWispFogSprite = CreateWitheredFogSprite();
            fogRenderer.sprite = _sharedWispFogSprite;
            fogRenderer.sortingLayerName = "FogOverlay";
            fogRenderer.sortingOrder = 10;
            fogRenderer.color = new Color(0.36f, 0.24f, 0.44f, 0.56f);
        }

        private void SpawnDriftingBlightSpores()
        {
            if (_sharedSporeSprite == null) _sharedSporeSprite = CreateSporeSprite();
            int count = 4;
            _spores = new Transform[count];
            _sporeSrs = new SpriteRenderer[count];
            _sporePhases = new float[count];

            for (int i = 0; i < count; i++)
            {
                GameObject go = new GameObject("BlightSpore_" + i);
                go.transform.SetParent(transform, false);
                go.transform.localPosition = new Vector3(Random.Range(-1.1f, 1.1f), Random.Range(-0.6f, 0.6f), 0f);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = _sharedSporeSprite;
                sr.sortingLayerName = "FogOverlay";
                sr.sortingOrder = 12;
                sr.color = new Color(0.64f, 0.44f, 0.78f, 0.72f);

                _spores[i] = go.transform;
                _sporeSrs[i] = sr;
                _sporePhases[i] = Random.Range(0f, 6.28f);
            }
        }

        private void Update()
        {
            _driftPhase += Time.deltaTime * 0.65f;
            transform.position = _startPos + new Vector3(Mathf.Sin(_driftPhase) * 0.55f, Mathf.Cos(_driftPhase * 0.75f) * 0.22f, 0f);
            float breath = 1f + Mathf.Sin(_driftPhase * 1.1f) * 0.06f;
            transform.localScale = new Vector3(_baseScale.x * breath, _baseScale.y * breath, 1f);

            if (_spores != null)
            {
                for (int i = 0; i < _spores.Length; i++)
                {
                    if (_spores[i] == null) continue;
                    _sporePhases[i] += Time.deltaTime * 1.4f;
                    Vector3 lp = _spores[i].localPosition;
                    lp.y += Time.deltaTime * 0.14f;
                    lp.x += Mathf.Sin(_sporePhases[i]) * Time.deltaTime * 0.18f;
                    if (lp.y > 0.85f) lp.y = -0.65f;
                    _spores[i].localPosition = lp;

                    if (_isClearing && _sporeSrs[i] != null)
                    {
                        Color sc = _sporeSrs[i].color;
                        sc.a = Mathf.MoveTowards(sc.a, 0f, Time.deltaTime * 0.6f);
                        _sporeSrs[i].color = sc;
                    }
                }
            }

            if (_isClearing && fogRenderer != null)
            {
                Color c = fogRenderer.color;
                c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * 0.42f);
                fogRenderer.color = c;
                if (c.a <= 0.01f) gameObject.SetActive(false);
            }
        }

        private static Sprite CreateWitheredFogSprite()
        {
            const int w = 128;
            const int h = 80;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Vector2[] lobes =
            {
                new Vector2(0.50f, 0.50f),
                new Vector2(0.32f, 0.46f),
                new Vector2(0.68f, 0.52f),
                new Vector2(0.44f, 0.64f),
                new Vector2(0.58f, 0.36f)
            };
            float[] radii = { 0.42f, 0.32f, 0.34f, 0.28f, 0.28f };

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float u = (float)x / (w - 1);
                    float v = (float)y / (h - 1);
                    float density = 0f;
                    for (int k = 0; k < lobes.Length; k++)
                    {
                        float dx = (u - lobes[k].x) / radii[k];
                        float dy = (v - lobes[k].y) / (radii[k] * 0.85f);
                        float d2 = dx * dx + dy * dy;
                        if (d2 < 1f)
                        {
                            float wgt = (1f - d2) * (1f - d2);
                            density = Mathf.Max(density, wgt);
                        }
                    }
                    // Withered violet-ash wispy texture
                    float swirl = 0.84f + 0.16f * Mathf.Sin(u * 14f + v * 9f);
                    float a = Mathf.Clamp01(density * swirl);
                    tex.SetPixel(x, y, new Color(0.55f, 0.42f, 0.66f, a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
        }

        private static Sprite CreateSporeSprite()
        {
            const int sz = 10;
            Texture2D tex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            for (int y = 0; y < sz; y++)
            {
                for (int x = 0; x < sz; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(4.5f, 4.5f)) / 4.5f;
                    float a = Mathf.Clamp01(1f - d);
                    tex.SetPixel(x, y, new Color(0.86f, 0.72f, 0.98f, a * a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, sz, sz), new Vector2(0.5f, 0.5f), 28f);
        }
    
        private void HandleSaveLoaded_Step40()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                GameRoot.Instance.Flags.HasFlag(clearOnFlag))
            {
                _isClearing = false;
                gameObject.SetActive(false);
            }
        }
}
}
