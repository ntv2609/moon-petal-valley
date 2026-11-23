using System.Collections;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Audio;

namespace MoonpetalValley.Player
{
    public class WenComboVfxController : MonoBehaviour
    {
        [Header("Tham chiếu Wen")]
        [SerializeField] private WenSpriteAnimator spriteAnimator;
        [SerializeField] private SpriteRenderer legacySlashRenderer;

        [Header("Bộ VFX Vệt Chém Đa Khung Hình (Step C1)")]
        [SerializeField] private Sprite[] combo1Frames;
        [SerializeField] private Sprite[] combo2Frames;
        [SerializeField] private Sprite[] combo3Frames;
        [SerializeField] private Sprite[] dashStrikeFrames;
        [SerializeField] private Sprite[] hitSparkFrames;
        [SerializeField] private Sprite petalSprite;

        [Header("Tốc độ & Cảm giác đánh")]
        [SerializeField] private float slashVfxFps = 20f;
        [SerializeField] private float finisherVfxFps = 16f;
        [SerializeField] private float hitSparkFps = 22f;

        private AudioManager _cachedAudio;

        private void OnEnable()
        {
            PlayerCombatController.OnPlayerComboSwing += HandleComboSwing;
            PlayerCombatController.OnPlayerComboHit += HandleComboHit;
        }

        private void OnDisable()
        {
            PlayerCombatController.OnPlayerComboSwing -= HandleComboSwing;
            PlayerCombatController.OnPlayerComboHit -= HandleComboHit;
        }

        private void HandleComboSwing(int comboStep, bool isDashStrike, Vector2 playerPos, Vector2 facingDir)
        {
            if (legacySlashRenderer != null)
            {
                legacySlashRenderer.enabled = false;
            }

            // 1. Âm thanh chém tăng dần cao độ theo nhịp combo
            if (_cachedAudio == null) _cachedAudio = UnityEngine.Object.FindAnyObjectByType<AudioManager>();
            if (_cachedAudio != null)
            {
                _cachedAudio.PlayComboSlashSFX(comboStep, isDashStrike);
            }

            // 2. VFX vệt chém đa khung hình trên layer WeatherVFX
            Vector2 dir = facingDir.sqrMagnitude > 0.001f ? facingDir.normalized : Vector2.down;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (isDashStrike && HasFrames(dashStrikeFrames))
            {
                SpawnSpriteAnim(dashStrikeFrames, playerPos + dir * 0.55f, angle, 1.05f, slashVfxFps, Color.white, 33);
                StartCoroutine(SpawnAfterimageBurst(3, 0.035f, new Color(0.55f, 0.96f, 1f, 0.65f)));
            }
            else if (comboStep == 3 && HasFrames(combo3Frames))
            {
                SpawnSpriteAnim(combo3Frames, playerPos, 0f, 1.15f, finisherVfxFps, Color.white, 34);
                StartCoroutine(SpawnAfterimageBurst(3, 0.04f, new Color(1f, 0.88f, 0.55f, 0.65f)));
                SpawnPetalBurst(playerPos, 9);
            }
            else if (comboStep == 2 && HasFrames(combo2Frames))
            {
                SpawnSpriteAnim(combo2Frames, playerPos + dir * 0.45f, angle, 1.02f, slashVfxFps, Color.white, 33);
            }
            else if (HasFrames(combo1Frames))
            {
                SpawnSpriteAnim(combo1Frames, playerPos + dir * 0.42f, angle, 1.0f, slashVfxFps, Color.white, 32);
            }
        }

        private void HandleComboHit(int comboStep, bool isDashStrike, Vector2 hitPos, int damage)
        {
            // 1. Tia sáng trúng đòn (Hit-Spark)
            if (HasFrames(hitSparkFrames))
            {
                float rot = Random.Range(0f, 360f);
                float scale = (comboStep == 3) ? 1.35f : (isDashStrike ? 1.15f : 1.0f);
                Color tint = (comboStep == 3) ? new Color(1f, 0.94f, 0.68f, 1f) : Color.white;
                SpawnSpriteAnim(hitSparkFrames, hitPos, rot, scale, hitSparkFps, tint, 36);
            }

            // 2. Hitstop + Camera Shake theo sức nặng của đòn
            if (JuiceEffectsManager.Instance != null)
            {
                if (comboStep == 3)
                {
                    JuiceEffectsManager.Instance.TriggerHitstop(0.048f);
                    JuiceEffectsManager.Instance.TriggerCameraShake(0.18f, 0.16f);
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(hitPos + new Vector2(0f, 0.65f), "HOA TRĂNG!", new Color(1f, 0.90f, 0.42f, 1f));
                }
                else if (isDashStrike)
                {
                    JuiceEffectsManager.Instance.TriggerHitstop(0.028f);
                    JuiceEffectsManager.Instance.TriggerCameraShake(0.11f, 0.11f);
                }
                else if (comboStep == 2)
                {
                    JuiceEffectsManager.Instance.TriggerHitstop(0.024f);
                    JuiceEffectsManager.Instance.TriggerCameraShake(0.09f, 0.10f);
                }
                else
                {
                    JuiceEffectsManager.Instance.TriggerHitstop(0.020f);
                    JuiceEffectsManager.Instance.TriggerCameraShake(0.06f, 0.08f);
                }
            }
        }

        private bool HasFrames(Sprite[] arr)
        {
            return arr != null && arr.Length > 0 && arr[0] != null;
        }

        private void SpawnSpriteAnim(Sprite[] frames, Vector2 pos, float rotZ, float scale, float fps, Color tint, int sortingOrder)
        {
            GameObject go = new GameObject("ComboVfxOneShot");
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            go.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
            go.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "WeatherVFX";
            sr.sortingOrder = sortingOrder;
            sr.color = tint;

            OneShotSpritePlayer player = go.AddComponent<OneShotSpritePlayer>();
            player.Play(sr, frames, fps);
        }

        private IEnumerator SpawnAfterimageBurst(int count, float interval, Color tint)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnSingleAfterimage(tint);
                yield return new WaitForSeconds(interval);
            }
        }

        private void SpawnSingleAfterimage(Color tint)
        {
            if (spriteAnimator == null) return;
            Sprite spr = spriteAnimator.GetCurrentSprite();
            SpriteRenderer srcSr = spriteAnimator.SpriteRendererRef;
            if (spr == null || srcSr == null) return;

            GameObject go = new GameObject("WenAfterimage");
            go.transform.position = srcSr.transform.position;
            go.transform.localScale = srcSr.transform.lossyScale;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.flipX = srcSr.flipX;
            sr.sortingLayerName = srcSr.sortingLayerName;
            sr.sortingOrder = srcSr.sortingOrder - 1;
            sr.color = tint;

            AfterimageFade fade = go.AddComponent<AfterimageFade>();
            fade.Initialize(sr, 0.19f);
        }

        private void SpawnPetalBurst(Vector2 center, int count)
        {
            if (petalSprite == null) return;
            for (int i = 0; i < count; i++)
            {
                float angleDeg = (360f / count) * i + Random.Range(-18f, 18f);
                float rad = angleDeg * Mathf.Deg2Rad;
                Vector2 vel = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * Random.Range(2.4f, 4.2f);

                GameObject go = new GameObject("MoonpetalParticle");
                go.transform.position = new Vector3(center.x + Mathf.Cos(rad) * 0.35f, center.y + Mathf.Sin(rad) * 0.35f, 0f);
                float sc = Random.Range(0.85f, 1.25f);
                go.transform.localScale = new Vector3(sc, sc, 1f);

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = petalSprite;
                sr.sortingLayerName = "WeatherVFX";
                sr.sortingOrder = 35;
                sr.color = (i % 3 == 0) ? new Color(1f, 0.94f, 0.72f, 1f) : Color.white;

                PetalDrift drift = go.AddComponent<PetalDrift>();
                drift.Initialize(sr, vel, Random.Range(0.36f, 0.52f), Random.Range(-320f, 320f));
            }
        }

        private class OneShotSpritePlayer : MonoBehaviour
        {
            private SpriteRenderer _sr;
            private Sprite[] _frames;
            private float _frameDur;
            private float _timer;
            private int _index;

            public void Play(SpriteRenderer sr, Sprite[] frames, float fps)
            {
                _sr = sr;
                _frames = frames;
                _frameDur = fps > 0.1f ? (1f / fps) : 0.05f;
                _index = 0;
                _timer = 0f;
                if (_sr != null && _frames != null && _frames.Length > 0)
                {
                    _sr.sprite = _frames[0];
                }
            }

            private void Update()
            {
                if (_sr == null || _frames == null || _frames.Length == 0)
                {
                    Destroy(gameObject);
                    return;
                }
                _timer += Time.unscaledDeltaTime;
                if (_timer >= _frameDur)
                {
                    _timer -= _frameDur;
                    _index++;
                    if (_index >= _frames.Length)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    _sr.sprite = _frames[_index];
                }
            }
        }

        private class AfterimageFade : MonoBehaviour
        {
            private SpriteRenderer _sr;
            private float _duration;
            private float _timer;
            private Color _startColor;

            public void Initialize(SpriteRenderer sr, float duration)
            {
                _sr = sr;
                _duration = Mathf.Max(0.05f, duration);
                _timer = 0f;
                _startColor = sr != null ? sr.color : Color.white;
            }

            private void Update()
            {
                if (_sr == null) { Destroy(gameObject); return; }
                _timer += Time.deltaTime;
                float t = Mathf.Clamp01(_timer / _duration);
                Color c = _startColor;
                c.a = Mathf.Lerp(_startColor.a, 0f, t);
                _sr.color = c;
                if (t >= 1f) Destroy(gameObject);
            }
        }

        private class PetalDrift : MonoBehaviour
        {
            private SpriteRenderer _sr;
            private Vector2 _velocity;
            private float _duration;
            private float _rotSpeed;
            private float _timer;
            private Color _startColor;

            public void Initialize(SpriteRenderer sr, Vector2 velocity, float duration, float rotSpeed)
            {
                _sr = sr;
                _velocity = velocity;
                _duration = Mathf.Max(0.1f, duration);
                _rotSpeed = rotSpeed;
                _startColor = sr != null ? sr.color : Color.white;
            }

            private void Update()
            {
                if (_sr == null) { Destroy(gameObject); return; }
                float dt = Time.deltaTime;
                _timer += dt;
                float t = Mathf.Clamp01(_timer / _duration);

                _velocity = Vector2.Lerp(_velocity, new Vector2(0f, -0.35f), dt * 4.5f);
                transform.position += new Vector3(_velocity.x * dt, _velocity.y * dt, 0f);
                transform.Rotate(0f, 0f, _rotSpeed * dt);

                Color c = _startColor;
                c.a = 1f - (t * t);
                _sr.color = c;

                if (t >= 1f) Destroy(gameObject);
            }
        }
    }
}
