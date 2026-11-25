using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoonpetalValley.UI;

namespace MoonpetalValley.Combat {
    public class MoonlightWaveProjectile : MonoBehaviour {
        private static Sprite _cachedCrescentSprite;

        private Vector2 _direction = Vector2.right;
        private float _speed = 11.8f;
        private float _lifetime = 0.62f;
        private float _elapsed;
        private float _hitRadius = 0.82f;
        private int _damage = 18;
        private float _knockback = 4.2f;
        private TMP_FontAsset _font;
        private SpriteRenderer _sr;
        private readonly HashSet<IDamageable> _hitTargets = new HashSet<IDamageable>();

        public static MoonlightWaveProjectile Spawn(Vector2 origin, Vector2 dir, int damage, TMP_FontAsset font = null) {
            GameObject go = new GameObject("MoonlightWaveProjectile");
            go.transform.position = new Vector3(origin.x, origin.y, 0f);
            var proj = go.AddComponent<MoonlightWaveProjectile>();
            proj.Initialize(dir, damage, font);
            return proj;
        }

        private void Initialize(Vector2 dir, int damage, TMP_FontAsset font) {
            _direction = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.right;
            _damage = Mathf.Max(6, damage);
            _font = font;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            transform.localScale = new Vector3(1.15f, 1.35f, 1f);

            _sr = gameObject.AddComponent<SpriteRenderer>();
            _sr.sprite = GetOrCreateCrescentSprite();
            _sr.color = new Color(0.58f, 0.96f, 1f, 0.96f);
            _sr.sortingLayerName = "WorldObjects";
            _sr.sortingOrder = 190;
        }

        private void Update() {
            float dt = Time.deltaTime;
            _elapsed += dt;
            if (_elapsed >= _lifetime) {
                Destroy(gameObject);
                return;
            }

            transform.position += (Vector3)(_direction * (_speed * dt));

            float p = _elapsed / _lifetime;
            float scaleBoost = 1f + p * 0.35f;
            transform.localScale = new Vector3(1.15f * scaleBoost, 1.35f * scaleBoost, 1f);

            if (_sr != null) {
                Color c = Color.Lerp(new Color(0.62f, 0.98f, 1f, 0.96f), new Color(1f, 0.92f, 0.56f, 0f), p * p);
                _sr.color = c;
            }

            CheckHits();
        }

        private void CheckHits() {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, _hitRadius);
            for (int i = 0; i < cols.Length; i++) {
                if (cols[i] == null) continue;
                IDamageable target = cols[i].GetComponent<IDamageable>();
                if (target == null) target = cols[i].GetComponentInParent<IDamageable>();
                if (target == null || target.IsDefeated || _hitTargets.Contains(target)) continue;

                _hitTargets.Add(target);
                target.TakePurifyDamage(_damage, _direction, _knockback);

                GameObject ftGo = new GameObject("MoonWaveDmg");
                ftGo.transform.position = cols[i].transform.position + new Vector3(0f, 0.55f, 0f);
                var ft = ftGo.AddComponent<FloatingDamageText>();
                ft.Initialize("-" + _damage, new Color(0.55f, 0.96f, 1f, 1f), _font);
            }
        }

        private static Sprite GetOrCreateCrescentSprite() {
            if (_cachedCrescentSprite != null) return _cachedCrescentSprite;

            const int w = 24;
            const int h = 32;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color[] px = new Color[w * h];
            float cy = (h - 1) * 0.5f;

            for (int y = 0; y < h; y++) {
                float ny = (y - cy) / (h * 0.48f);
                for (int x = 0; x < w; x++) {
                    float nx = (x - w * 0.42f) / (w * 0.52f);
                    float outer = nx * nx * 1.15f + ny * ny;
                    float inner = (nx + 0.42f) * (nx + 0.42f) * 1.25f + ny * ny * 0.92f;
                    Color c = Color.clear;
                    if (outer <= 1.0f && inner >= 0.62f && nx >= -0.35f) {
                        float edge = Mathf.Clamp01(1f - Mathf.Abs(outer - 0.86f) * 3.8f);
                        bool core = outer <= 0.92f && inner >= 0.78f;
                        c = core
                            ? new Color(0.95f, 1f, 1f, 0.98f)
                            : new Color(0.45f, 0.92f, 1f, Mathf.Clamp01(0.55f + edge * 0.45f));
                    }
                    px[y * w + x] = c;
                }
            }

            tex.SetPixels(px);
            tex.Apply();
            _cachedCrescentSprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 24f);
            return _cachedCrescentSprite;
        }
    }
}
