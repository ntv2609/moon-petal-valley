using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoonpetalValley.UI
{
    public class MainMenuLivingBackdrop : MonoBehaviour
    {
        [SerializeField] private RectTransform backdropRect;
        [SerializeField] private RectTransform logoRect;
        [SerializeField] private RectTransform fxContainer;

        private class ParticleItem
        {
            public RectTransform rect;
            public Image img;
            public Vector2 pos;
            public Vector2 vel;
            public float phase;
            public float speed;
            public float baseAlpha;
            public float baseScale;
            public int kind; // 0=petal, 1=firefly, 2=star, 3=smoke, 4=pondGlint
        }

        private readonly List<ParticleItem> _items = new List<ParticleItem>();
        private Sprite _softCircleSprite;
        private Sprite _pixelSquareSprite;
        private RectTransform _moonGlowRect;
        private Image _moonGlowImg;

        private Vector2 _logoBasePos;
        private Vector2 _parallaxCurrent;
        private float _elapsed;

        private void Start()
        {
            if (backdropRect == null)
            {
                Transform b = transform.Find("Backdrop");
                if (b != null) backdropRect = b.GetComponent<RectTransform>();
            }
            if (logoRect == null)
            {
                Transform l = transform.Find("MainMenuLogo");
                if (l != null) logoRect = l.GetComponent<RectTransform>();
            }
            if (logoRect != null) _logoBasePos = logoRect.anchoredPosition;

            EnsureFxContainer();
            BuildProceduralSprites();
            SpawnAllLivingFX();
        }

        private void EnsureFxContainer()
        {
            if (fxContainer == null)
            {
                Transform existing = transform.Find("LivingFXLayer");
                if (existing != null)
                {
                    fxContainer = existing.GetComponent<RectTransform>();
                }
                else
                {
                    GameObject go = new GameObject("LivingFXLayer");
                    go.transform.SetParent(transform, false);
                    fxContainer = go.AddComponent<RectTransform>();
                    fxContainer.anchorMin = Vector2.zero;
                    fxContainer.anchorMax = Vector2.one;
                    fxContainer.offsetMin = Vector2.zero;
                    fxContainer.offsetMax = Vector2.zero;
                }
            }
            // Place right above Backdrop (index 1) and behind MainMenuLogo & Buttons
            if (backdropRect != null)
            {
                fxContainer.SetSiblingIndex(backdropRect.GetSiblingIndex() + 1);
            }
        }

        private void BuildProceduralSprites()
        {
            // Soft radial glow sprite (32x32)
            Texture2D glowTex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            glowTex.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(15.5f, 15.5f)) / 15.5f;
                    float a = Mathf.Clamp01(1f - d);
                    glowTex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
                }
            }
            glowTex.Apply();
            _softCircleSprite = Sprite.Create(glowTex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 100f);

            // Crisp pixel petal/sparkle sprite (6x6)
            Texture2D sqTex = new Texture2D(6, 6, TextureFormat.RGBA32, false);
            sqTex.filterMode = FilterMode.Point;
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    bool corner = (x == 0 || x == 5) && (y == 0 || y == 5);
                    sqTex.SetPixel(x, y, corner ? Color.clear : Color.white);
                }
            }
            sqTex.Apply();
            _pixelSquareSprite = Sprite.Create(sqTex, new Rect(0, 0, 6, 6), new Vector2(0.5f, 0.5f), 100f);
        }

        private ParticleItem CreateFX(string name, Sprite spr, Vector2 size, Color col, Vector2 pos, Vector2 vel, int kind, float speed)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(fxContainer, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0.5f);
            r.anchorMax = new Vector2(0.5f, 0.5f);
            r.sizeDelta = size;
            r.anchoredPosition = pos;

            var img = go.AddComponent<Image>();
            img.sprite = spr;
            img.color = col;
            img.raycastTarget = false;

            var item = new ParticleItem
            {
                rect = r,
                img = img,
                pos = pos,
                vel = vel,
                phase = Random.Range(0f, 6.28f),
                speed = speed,
                baseAlpha = col.a,
                baseScale = 1f,
                kind = kind
            };
            _items.Add(item);
            return item;
        }

        private void SpawnAllLivingFX()
        {
            // 1. Soft breathing glow around the golden crescent moon (upper-right sky)
            GameObject mg = new GameObject("MoonBreathingGlow");
            mg.transform.SetParent(fxContainer, false);
            _moonGlowRect = mg.AddComponent<RectTransform>();
            _moonGlowRect.anchorMin = new Vector2(0.5f, 0.5f);
            _moonGlowRect.anchorMax = new Vector2(0.5f, 0.5f);
            _moonGlowRect.anchoredPosition = new Vector2(465f, 325f);
            _moonGlowRect.sizeDelta = new Vector2(260f, 260f);
            _moonGlowImg = mg.AddComponent<Image>();
            _moonGlowImg.sprite = _softCircleSprite;
            _moonGlowImg.color = new Color(1.0f, 0.94f, 0.72f, 0.24f);
            _moonGlowImg.raycastTarget = false;

            // 2. 14 Twinkling Twilight Stars (upper sky)
            for (int i = 0; i < 14; i++)
            {
                Vector2 p = new Vector2(Random.Range(-880f, 880f), Random.Range(140f, 490f));
                float sz = Random.Range(6f, 11f);
                CreateFX("Star_" + i, _softCircleSprite, new Vector2(sz, sz),
                    new Color(1f, 0.98f, 0.86f, Random.Range(0.45f, 0.90f)), p, Vector2.zero, 2, Random.Range(1.4f, 3.2f));
            }

            // 3. 28 Drifting Cherry Blossom Petals
            for (int i = 0; i < 28; i++)
            {
                Vector2 p = new Vector2(Random.Range(-980f, 980f), Random.Range(-520f, 540f));
                float sz = Random.Range(9f, 15f);
                Color col = (i % 3 == 0)
                    ? new Color(1f, 0.88f, 0.94f, 0.88f)
                    : new Color(0.98f, 0.68f, 0.82f, 0.85f);
                Vector2 vel = new Vector2(Random.Range(-52f, -24f), Random.Range(-32f, -14f));
                CreateFX("Petal_" + i, _pixelSquareSprite, new Vector2(sz, sz * 0.68f), col, p, vel, 0, Random.Range(1.6f, 3.0f));
            }

            // 4. 20 Glowing Moonpetal Fireflies (cyan & warm gold)
            for (int i = 0; i < 20; i++)
            {
                Vector2 p = new Vector2(Random.Range(-880f, 880f), Random.Range(-420f, 80f));
                float sz = Random.Range(14f, 24f);
                Color col = (i % 2 == 0)
                    ? new Color(0.58f, 0.96f, 1.0f, 0.72f)
                    : new Color(1.0f, 0.92f, 0.56f, 0.68f);
                Vector2 vel = new Vector2(Random.Range(-12f, 12f), Random.Range(8f, 22f));
                CreateFX("Firefly_" + i, _softCircleSprite, new Vector2(sz, sz), col, p, vel, 1, Random.Range(1.5f, 3.0f));
            }

            // 5. 6 Rising Bakery Chimney Smoke Puffs (right hill cottage chimney)
            for (int i = 0; i < 6; i++)
            {
                float progress = i / 6f;
                Vector2 p = new Vector2(795f + progress * 24f, -20f + progress * 190f);
                float sz = Mathf.Lerp(22f, 56f, progress);
                var item = CreateFX("ChimneySmoke_" + i, _softCircleSprite, new Vector2(sz, sz),
                    new Color(0.92f, 0.94f, 0.98f, 0.24f), p, new Vector2(12f, 36f), 3, 1.2f);
                item.phase = progress;
            }

            // 6. 10 Shimmering Moonlight Glints across the curved valley pond
            for (int i = 0; i < 10; i++)
            {
                Vector2 p = new Vector2(Random.Range(-190f, 160f), Random.Range(-410f, -315f));
                float w = Random.Range(14f, 28f);
                CreateFX("PondGlint_" + i, _pixelSquareSprite, new Vector2(w, 4f),
                    new Color(0.84f, 0.97f, 1.0f, 0.75f), p, Vector2.zero, 4, Random.Range(1.8f, 3.6f));
            }
        }

        private Vector2 ReadNormalizedMouse()
        {
            Vector2 pos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                pos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            pos = Input.mousePosition;
#endif
            float nx = Screen.width > 0 ? Mathf.Clamp((pos.x / Screen.width) * 2f - 1f, -1f, 1f) : 0f;
            float ny = Screen.height > 0 ? Mathf.Clamp((pos.y / Screen.height) * 2f - 1f, -1f, 1f) : 0f;
            return new Vector2(nx, ny);
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;
            _elapsed += dt;

            // 1. Ken Burns slow breathing zoom + smooth mouse parallax on Backdrop
            Vector2 targetParallax = ReadNormalizedMouse() * -18f;
            _parallaxCurrent = Vector2.Lerp(_parallaxCurrent, targetParallax, dt * 3.0f);

            if (backdropRect != null)
            {
                float zoom = 1.055f + Mathf.Sin(_elapsed * 0.28f) * 0.018f;
                backdropRect.localScale = new Vector3(zoom, zoom, 1f);
                backdropRect.anchoredPosition = _parallaxCurrent + new Vector2(Mathf.Sin(_elapsed * 0.18f) * 8f, Mathf.Cos(_elapsed * 0.15f) * 4f);
            }

            // 2. Gentle floating Stardew-style logo bob
            if (logoRect != null)
            {
                float bobY = Mathf.Sin(_elapsed * 1.45f) * 8f;
                float sc = 1.0f + Mathf.Sin(_elapsed * 0.9f) * 0.012f;
                logoRect.anchoredPosition = _logoBasePos + new Vector2(_parallaxCurrent.x * 0.25f, bobY + _parallaxCurrent.y * 0.25f);
                logoRect.localScale = new Vector3(sc, sc, 1f);
            }

            // 3. Breathing Moon Halo
            if (_moonGlowRect != null && _moonGlowImg != null)
            {
                float pulse = 1.0f + Mathf.Sin(_elapsed * 1.1f) * 0.12f;
                _moonGlowRect.localScale = new Vector3(pulse, pulse, 1f);
                Color mc = _moonGlowImg.color;
                mc.a = 0.20f + Mathf.Sin(_elapsed * 1.1f) * 0.07f;
                _moonGlowImg.color = mc;
            }

            // 4. Animate all particles
            for (int i = 0; i < _items.Count; i++)
            {
                var it = _items[i];
                if (it.rect == null || it.img == null) continue;

                if (it.kind == 0) // Cherry Blossom Petals
                {
                    it.phase += dt * it.speed;
                    it.pos.x += (it.vel.x + Mathf.Sin(it.phase) * 18f) * dt;
                    it.pos.y += (it.vel.y + Mathf.Cos(it.phase * 0.7f) * 8f) * dt;
                    if (it.pos.x < -1020f || it.pos.y < -580f)
                    {
                        it.pos = new Vector2(Random.Range(-300f, 1020f), Random.Range(460f, 600f));
                    }
                    it.rect.anchoredPosition = it.pos;
                    it.rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(it.phase) * 35f);
                }
                else if (it.kind == 1) // Glowing Fireflies
                {
                    it.phase += dt * it.speed;
                    it.pos.x += (it.vel.x + Mathf.Sin(it.phase) * 20f) * dt;
                    it.pos.y += it.vel.y * dt;
                    if (it.pos.y > 240f)
                    {
                        it.pos = new Vector2(Random.Range(-880f, 880f), Random.Range(-460f, -280f));
                    }
                    it.rect.anchoredPosition = it.pos;
                    Color c = it.img.color;
                    c.a = it.baseAlpha * (0.35f + 0.65f * (0.5f + 0.5f * Mathf.Sin(it.phase * 1.6f)));
                    it.img.color = c;
                }
                else if (it.kind == 2) // Twinkling Stars
                {
                    it.phase += dt * it.speed;
                    float tw = 0.25f + 0.75f * (0.5f + 0.5f * Mathf.Sin(it.phase));
                    Color c = it.img.color;
                    c.a = it.baseAlpha * tw;
                    it.img.color = c;
                    float s = 0.75f + tw * 0.45f;
                    it.rect.localScale = new Vector3(s, s, 1f);
                }
                else if (it.kind == 3) // Rising Chimney Smoke
                {
                    it.phase += dt * 0.22f;
                    if (it.phase >= 1f) it.phase -= 1f;
                    float p = it.phase;
                    float sx = 795f + p * 34f + Mathf.Sin(p * 6.28f + _elapsed) * 10f;
                    float sy = -22f + p * 200f;
                    it.rect.anchoredPosition = new Vector2(sx, sy);
                    float sz = Mathf.Lerp(20f, 68f, p);
                    it.rect.sizeDelta = new Vector2(sz, sz);
                    Color c = it.img.color;
                    c.a = Mathf.Sin(p * Mathf.PI) * 0.24f;
                    it.img.color = c;
                }
                else if (it.kind == 4) // Pond Moonlight Sparkles
                {
                    it.phase += dt * it.speed;
                    float gl = Mathf.Pow(0.5f + 0.5f * Mathf.Sin(it.phase), 3f);
                    Color c = it.img.color;
                    c.a = it.baseAlpha * gl;
                    it.img.color = c;
                    it.rect.anchoredPosition = it.pos + new Vector2(Mathf.Sin(it.phase * 0.5f) * 6f, 0f);
                }
            }
        }
    }
}
