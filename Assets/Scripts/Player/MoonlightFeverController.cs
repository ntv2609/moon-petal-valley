using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Combat;
using MoonpetalValley.UI;

namespace MoonpetalValley.Player {
    public class MoonlightFeverController : MonoBehaviour {
        public static MoonlightFeverController Instance { get; private set; }
        public static event Action OnMoonlightFeverTriggered;

        public const int FeverHitThreshold = 15;
        public const float FeverDurationSeconds = 10.0f;
        public const float StreakDecaySeconds = 4.5f;
        public const float FeverDamageBonus = 0.35f;
        public const float FeverCooldownMultiplier = 0.80f;

        [SerializeField] private TMP_FontAsset vnFont;

        private int _currentStreak;
        private float _streakDecayTimer;
        private bool _isFeverActive;
        private float _feverRemaining;
        private int _lastLightHP = -1;
        private int _totalFeversTriggered;
        private float _waveSpawnCooldown;

        private GameObject _auraGo;
        private SpriteRenderer _auraSr;
        private static Sprite _cachedAuraSprite;

        public int CurrentStreak => _currentStreak;
        public int FeverThreshold => FeverHitThreshold;
        public bool IsFeverActive => _isFeverActive;
        public float FeverRemaining => _feverRemaining;
        public float FeverDuration => FeverDurationSeconds;
        public float StreakDecayRatio => _isFeverActive ? 1f : Mathf.Clamp01(_streakDecayTimer / StreakDecaySeconds);
        public int TotalFeversTriggered => _totalFeversTriggered;

        private void Awake() {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        private void OnEnable() {
            PlayerCombatController.OnPlayerComboHit += HandlePlayerComboHit;
            PlayerCombatController.OnPlayerComboSwing += HandlePlayerComboSwing;
            GameEventBus.OnLightHPChanged += HandleLightHPChanged;
            GameEventBus.OnSaveLoaded += ResetAll;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable() {
            PlayerCombatController.OnPlayerComboHit -= HandlePlayerComboHit;
            PlayerCombatController.OnPlayerComboSwing -= HandlePlayerComboSwing;
            GameEventBus.OnLightHPChanged -= HandleLightHPChanged;
            GameEventBus.OnSaveLoaded -= ResetAll;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void OnDestroy() {
            if (Instance == this) Instance = null;
            DestroyAura();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) {
            ResetAll();
        }

        public void ResetAll() {
            _currentStreak = 0;
            _streakDecayTimer = 0f;
            _lastLightHP = -1;
            if (_isFeverActive) EndFever(false);
            DestroyAura();
        }

        private void Update() {
            float dt = Time.deltaTime;
            if (_waveSpawnCooldown > 0f) _waveSpawnCooldown -= dt;

            if (_isFeverActive) {
                _feverRemaining -= dt;
                UpdateAuraVisual();
                if (_feverRemaining <= 0f) {
                    EndFever(true);
                }
                return;
            }

            if (_currentStreak > 0) {
                _streakDecayTimer -= dt;
                if (_streakDecayTimer <= 0f) {
                    _currentStreak = 0;
                    _streakDecayTimer = 0f;
                }
            }
        }

        private void HandlePlayerComboHit(int comboStep, bool isDashStrike, Vector2 hitWorldPos, int damageDealt) {
            if (_isFeverActive) return;

            _currentStreak++;
            _streakDecayTimer = StreakDecaySeconds;

            if (_currentStreak >= FeverHitThreshold) {
                ActivateFever();
            }
        }

        private void HandlePlayerComboSwing(int comboStep, bool isDashStrike, Vector2 playerPos, Vector2 facingDir) {
            if (!_isFeverActive) return;
            if (_waveSpawnCooldown > 0f) return;
            _waveSpawnCooldown = 0.08f;

            float dmgMult = PlayerCombatBuffController.Instance != null
                ? PlayerCombatBuffController.Instance.DamageMultiplier
                : (1f + FeverDamageBonus);
            int baseWave = isDashStrike ? 18 : (comboStep >= 3 ? 17 : 14);
            int finalDmg = Mathf.RoundToInt(baseWave * dmgMult);

            Vector2 dir = facingDir.sqrMagnitude > 0.001f ? facingDir.normalized : Vector2.right;
            Vector2 spawnPos = playerPos + dir * 0.55f + new Vector2(0f, 0.25f);
            MoonlightWaveProjectile.Spawn(spawnPos, dir, finalDmg, vnFont);
        }

        private void HandleLightHPChanged(int current, int max) {
            if (_lastLightHP >= 0 && current < _lastLightHP) {
                if (current <= 0) {
                    ResetAll();
                } else if (!_isFeverActive && _currentStreak > 0) {
                    _currentStreak = _currentStreak / 2;
                    _streakDecayTimer = Mathf.Min(_streakDecayTimer, 2.2f);
                }
            }
            _lastLightHP = current;
        }

        private void ActivateFever() {
            _isFeverActive = true;
            _feverRemaining = FeverDurationSeconds;
            _currentStreak = FeverHitThreshold;
            _totalFeversTriggered++;

            if (PlayerCombatBuffController.Instance != null) {
                PlayerCombatBuffController.Instance.SetFeverModifiers(FeverDamageBonus, FeverCooldownMultiplier);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null) {
                GameRoot.Instance.Audio.PlayUIClick();
            }

            GameEventBus.RaiseToastRequested("CƠN SỐT ÁNH TRĂNG! (+35% Sát thương, -20% Hồi chiêu, chém bắn Sóng Trăng 10 giây!)");

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null) {
                GameObject ftGo = new GameObject("FeverBannerPop");
                ftGo.transform.position = player.transform.position + new Vector3(0f, 1.15f, 0f);
                var ft = ftGo.AddComponent<FloatingDamageText>();
                ft.Initialize("CƠN SỐT ÁNH TRĂNG!", new Color(0.58f, 0.98f, 1f, 1f), vnFont);
                EnsureAura(player.transform);
            }

            OnMoonlightFeverTriggered?.Invoke();
        }

        private void EndFever(bool showToast) {
            _isFeverActive = false;
            _feverRemaining = 0f;
            _currentStreak = 0;
            _streakDecayTimer = 0f;

            if (PlayerCombatBuffController.Instance != null) {
                PlayerCombatBuffController.Instance.SetFeverModifiers(0f, 1f);
            }

            DestroyAura();
            if (showToast) {
                GameEventBus.RaiseToastRequested("Cơn Sốt Ánh Trăng đã lắng xuống.");
            }
        }

        private void EnsureAura(Transform playerTr) {
            if (playerTr == null) return;
            if (_auraGo == null) {
                _auraGo = new GameObject("MoonlightFeverAura");
                _auraGo.transform.SetParent(playerTr, false);
                _auraGo.transform.localPosition = new Vector3(0f, 0.2f, 0f);
                _auraSr = _auraGo.AddComponent<SpriteRenderer>();
                _auraSr.sprite = GetOrCreateAuraSprite();
                _auraSr.sortingLayerName = "WorldObjects";
                _auraSr.sortingOrder = -2;
            }
        }

        private void UpdateAuraVisual() {
            if (_auraGo == null) {
                var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (player != null) EnsureAura(player.transform);
            }
            if (_auraGo == null || _auraSr == null) return;

            float pulse = 1.0f + Mathf.Sin(Time.time * 9.5f) * 0.14f;
            _auraGo.transform.localScale = new Vector3(1.55f * pulse, 1.15f * pulse, 1f);
            _auraGo.transform.Rotate(0f, 0f, 65f * Time.deltaTime);
            float alpha = _feverRemaining < 2f ? (0.35f + Mathf.Abs(Mathf.Sin(Time.time * 14f)) * 0.45f) : 0.62f;
            _auraSr.color = new Color(0.55f, 0.96f, 1f, alpha);
        }

        private void DestroyAura() {
            if (_auraGo != null) {
                Destroy(_auraGo);
                _auraGo = null;
                _auraSr = null;
            }
        }

        private static Sprite GetOrCreateAuraSprite() {
            if (_cachedAuraSprite != null) return _cachedAuraSprite;
            const int sz = 32;
            Texture2D tex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            Color[] px = new Color[sz * sz];
            float c = (sz - 1) * 0.5f;
            for (int y = 0; y < sz; y++) {
                for (int x = 0; x < sz; x++) {
                    float dx = (x - c) / c;
                    float dy = (y - c) / c;
                    float r = Mathf.Sqrt(dx * dx + dy * dy);
                    if (r >= 0.72f && r <= 0.98f) {
                        float a = 1f - Mathf.Abs(r - 0.85f) / 0.14f;
                        px[y * sz + x] = new Color(0.68f, 0.98f, 1f, Mathf.Clamp01(a));
                    } else if (r < 0.72f) {
                        px[y * sz + x] = new Color(0.45f, 0.88f, 1f, (1f - r / 0.72f) * 0.22f);
                    } else {
                        px[y * sz + x] = Color.clear;
                    }
                }
            }
            tex.SetPixels(px);
            tex.Apply();
            _cachedAuraSprite = Sprite.Create(tex, new Rect(0, 0, sz, sz), new Vector2(0.5f, 0.5f), 24f);
            return _cachedAuraSprite;
        }
    }
}
