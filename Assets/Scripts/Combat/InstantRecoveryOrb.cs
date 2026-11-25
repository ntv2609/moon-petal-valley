using UnityEngine;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Combat
{
    public enum RecoveryOrbType
    {
        Stamina = 0,
        StaminaSurge = 1
    }

    public class InstantRecoveryOrb : MonoBehaviour
    {
        [SerializeField] private RecoveryOrbType orbType = RecoveryOrbType.Stamina;
        [SerializeField] private int restoreAmount = 20;
        [SerializeField] private float magnetRadius = 3.8f;
        [SerializeField] private float collectRadius = 0.78f;
        [SerializeField] private float moveSpeed = 8.4f;
        [SerializeField] private float lifeTimeSeconds = 26f;

        private Transform _playerTr;
        private Vector3 _basePos;
        private float _spawnTime;
        private float _age;
        private bool _collected;
        private SpriteRenderer _outerSr;
        private SpriteRenderer _innerSr;

        private static Sprite _cachedOrbSprite;
        private static Sprite _staminaSprite;
        private static Sprite _staminaSurgeSprite;
        private static TMP_FontAsset _cachedFont;

        public static void SetSharedAssets(Sprite orbSprite, TMP_FontAsset font)
        {
            if (orbSprite != null) _cachedOrbSprite = orbSprite;
            if (font != null) _cachedFont = font;
        }

        public static void SetCustomOrbSprites(Sprite staminaSprite, Sprite surgeSprite)
        {
            if (staminaSprite != null) _staminaSprite = staminaSprite;
            if (surgeSprite != null) _staminaSurgeSprite = surgeSprite;
        }

        public static void TrySpawnRandomDrop(Vector3 worldPos, float baseDropChance = 0.34f, bool guaranteedBossDrop = false)
        {
            if (guaranteedBossDrop)
            {
                SpawnOrb(worldPos + new Vector3(-0.50f, 0.25f, 0f), RecoveryOrbType.StaminaSurge, 45);
                SpawnOrb(worldPos + new Vector3(0.50f, 0.25f, 0f), RecoveryOrbType.Stamina, 20);
                return;
            }

            float stamRatio = 1f;
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                var st = GameRoot.Instance.Stats;
                stamRatio = st.EffectiveMaxStamina > 0 ? (float)st.CurrentStamina / st.EffectiveMaxStamina : 1f;
            }

            float effectiveChance = stamRatio < 0.45f ? Mathf.Max(0.55f, baseDropChance) : baseDropChance;
            if (Random.value > effectiveChance) return;

            // 8% ty le hiem rot Tinh The The Luc Bung No (hoi +45 Stamina + song day lui, KHONG hoi mau)
            if (Random.value < 0.08f)
            {
                SpawnOrb(worldPos + new Vector3(0f, 0.25f, 0f), RecoveryOrbType.StaminaSurge, 45);
                return;
            }

            Vector3 offset = new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(0.1f, 0.35f), 0f);
            SpawnOrb(worldPos + offset, RecoveryOrbType.Stamina, 20);
        }

        public static InstantRecoveryOrb SpawnOrb(Vector3 worldPos, RecoveryOrbType type, int amount = 20)
        {
            string goName = type == RecoveryOrbType.StaminaSurge ? "Orb_StaminaSurge" : "Orb_Stamina";
            GameObject go = new GameObject(goName);
            go.transform.position = worldPos;

            InstantRecoveryOrb orb = go.AddComponent<InstantRecoveryOrb>();
            orb.orbType = type;
            orb.restoreAmount = Mathf.Max(5, amount);
            orb.BuildVisuals();
            return orb;
        }

        private void Awake()
        {
            _basePos = transform.position;
            _spawnTime = Time.time;
        }

        private void Start()
        {
            if (_outerSr == null) BuildVisuals();
            FindPlayer();
        }

        private void FindPlayer()
        {
            PlayerController2D pc = Object.FindAnyObjectByType<PlayerController2D>();
            if (pc != null) _playerTr = pc.transform;
        }

        private static Sprite EnsureFallbackGlowSprite()
        {
            if (_cachedOrbSprite != null) return _cachedOrbSprite;
            Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            Color[] cols = new Color[16 * 16];
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    float dx = (x - 7.5f) / 7.5f;
                    float dy = (y - 7.5f) / 7.5f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    cols[y * 16 + x] = d <= 0.92f ? new Color(1f, 1f, 1f, Mathf.Clamp01(1.1f - d)) : Color.clear;
                }
            }
            tex.SetPixels(cols);
            tex.Apply();
            _cachedOrbSprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 32f);
            return _cachedOrbSprite;
        }

        private void BuildVisuals()
        {
            Sprite glowSpr = EnsureFallbackGlowSprite();
            Sprite customSpr = orbType == RecoveryOrbType.StaminaSurge ? _staminaSurgeSprite : _staminaSprite;

            Color outerCol;
            Color innerCol;
            float outerScale;
            float innerScale;
            if (orbType == RecoveryOrbType.StaminaSurge)
            {
                outerCol = new Color(1.0f, 0.86f, 0.24f, 0.62f);
                innerCol = customSpr != null ? Color.white : new Color(0.68f, 1.0f, 0.56f, 1.0f);
                outerScale = 0.88f;
                innerScale = customSpr != null ? 0.95f : 0.46f;
            }
            else
            {
                outerCol = new Color(0.45f, 0.96f, 0.42f, 0.46f);
                innerCol = customSpr != null ? Color.white : new Color(0.95f, 1.0f, 0.58f, 1.0f);
                outerScale = 0.65f;
                innerScale = customSpr != null ? 0.85f : 0.34f;
            }

            GameObject outer = new GameObject("OuterGlow");
            outer.transform.SetParent(transform, false);
            outer.transform.localScale = Vector3.one * outerScale;
            _outerSr = outer.AddComponent<SpriteRenderer>();
            _outerSr.sprite = glowSpr;
            _outerSr.color = outerCol;
            _outerSr.sortingLayerName = "WorldObjects";
            _outerSr.sortingOrder = 190;

            GameObject inner = new GameObject("Core");
            inner.transform.SetParent(transform, false);
            inner.transform.localScale = Vector3.one * innerScale;
            _innerSr = inner.AddComponent<SpriteRenderer>();
            _innerSr.sprite = customSpr != null ? customSpr : glowSpr;
            _innerSr.color = innerCol;
            _innerSr.sortingLayerName = "WorldObjects";
            _innerSr.sortingOrder = 191;
        }

        private void Update()
        {
            if (_collected) return;
            _age += Time.deltaTime;
            if (_age >= lifeTimeSeconds)
            {
                Destroy(gameObject);
                return;
            }

            if (_playerTr == null) FindPlayer();

            float bob = Mathf.Sin((Time.time - _spawnTime) * 5.2f) * 0.14f;
            float pulse = 1f + Mathf.Sin((Time.time - _spawnTime) * 7.5f) * 0.15f;
            if (_outerSr != null)
            {
                float baseS = orbType == RecoveryOrbType.StaminaSurge ? 0.88f : 0.65f;
                _outerSr.transform.localScale = Vector3.one * (baseS * pulse);
                if (_age > lifeTimeSeconds - 4f)
                {
                    Color c = _outerSr.color;
                    c.a = Mathf.Abs(Mathf.Sin(_age * 10f)) * 0.65f;
                    _outerSr.color = c;
                }
            }

            if (_playerTr != null)
            {
                Vector3 target = _playerTr.position + new Vector3(0f, 0.35f, 0f);
                float dist = Vector2.Distance(transform.position, target);
                if (dist <= collectRadius)
                {
                    Collect();
                    return;
                }
                if (dist <= magnetRadius && _age >= 0.18f)
                {
                    float speed = moveSpeed * (1f + (magnetRadius - dist) * 0.45f);
                    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                    _basePos = transform.position;
                    return;
                }
            }

            transform.position = new Vector3(_basePos.x, _basePos.y + bob, 0f);
        }

        private void Collect()
        {
            _collected = true;
            // CHINH SACH STAMINA-ONLY: Tham so 1 = staminaAmount, tham so 2 = lightHPAmount (LUON BANG 0)
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                GameRoot.Instance.Stats.RestoreStats(restoreAmount, 0, true);
                if (orbType == RecoveryOrbType.StaminaSurge)
                    TriggerStaminaSurgeShockwave(transform.position);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                GameRoot.Instance.Audio.PlayUIClick();

            if (orbType == RecoveryOrbType.StaminaSurge)
                SpawnFloatingText(transform.position + Vector3.up * 0.6f, $"+{restoreAmount} THỂ LỰC BÙNG NỔ!", new Color(0.88f, 1f, 0.42f, 1f));
            else
                SpawnFloatingText(transform.position + Vector3.up * 0.5f, $"+{restoreAmount} Thể Lực", new Color(0.75f, 1f, 0.45f, 1f));

            Destroy(gameObject);
        }

        private static void TriggerStaminaSurgeShockwave(Vector3 center)
        {
            float radius = 4.6f;
            var cols = Physics2D.OverlapCircleAll(center, radius);
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i] == null) continue;
                IDamageable dmg = cols[i].GetComponentInParent<IDamageable>();
                if (dmg == null || dmg.IsDefeated) continue;
                if (cols[i].GetComponentInParent<PlayerController2D>() != null) continue;
                Vector2 dir = ((Vector2)cols[i].transform.position - (Vector2)center).normalized;
                if (dir.sqrMagnitude < 0.01f) dir = Vector2.right;
                dmg.TakePurifyDamage(20, dir, 6.2f);
            }

            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.TriggerCameraShake(0.14f, 0.16f);

            GameObject wave = new GameObject("StaminaSurgeShockwave");
            wave.transform.position = center;
            var sr = wave.AddComponent<SpriteRenderer>();
            sr.sprite = EnsureFallbackGlowSprite();
            sr.color = new Color(0.82f, 1f, 0.42f, 0.72f);
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = 220;
            wave.AddComponent<StaminaSurgeRingVfx>();
        }

        private static void SpawnFloatingText(Vector3 pos, string msg, Color color)
        {
            GameObject go = new GameObject("OrbStaminaPopup");
            go.transform.position = pos;
            TextMeshPro tmp = go.AddComponent<TextMeshPro>();
            tmp.text = msg;
            tmp.fontSize = 3.6f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = color;
            tmp.sortingOrder = 260;
            if (_cachedFont != null) tmp.font = _cachedFont;
            else if (TMP_Settings.defaultFontAsset != null) tmp.font = TMP_Settings.defaultFontAsset;
            go.AddComponent<OrbFloatFade>();
        }

        private class OrbFloatFade : MonoBehaviour
        {
            private float _t;
            private TextMeshPro _tmp;
            private void Awake() => _tmp = GetComponent<TextMeshPro>();
            private void Update()
            {
                _t += Time.deltaTime;
                transform.position += Vector3.up * (1.2f * Time.deltaTime);
                if (_tmp != null)
                {
                    Color c = _tmp.color;
                    c.a = Mathf.Clamp01(1f - _t / 0.95f);
                    _tmp.color = c;
                }
                if (_t >= 0.95f) Destroy(gameObject);
            }
        }

        private class StaminaSurgeRingVfx : MonoBehaviour
        {
            private float _t;
            private SpriteRenderer _sr;
            private void Awake() => _sr = GetComponent<SpriteRenderer>();
            private void Update()
            {
                _t += Time.deltaTime;
                float p = Mathf.Clamp01(_t / 0.36f);
                float s = Mathf.Lerp(0.8f, 9.2f, p);
                transform.localScale = Vector3.one * s;
                if (_sr != null)
                {
                    Color c = _sr.color;
                    c.a = Mathf.Lerp(0.72f, 0f, p);
                    _sr.color = c;
                }
                if (_t >= 0.36f) Destroy(gameObject);
            }
        }
    }
}
