using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.Combat
{
    public class LanternMothController : MonoBehaviour, IDamageable, IStunnable
    {
        private enum MothState { Seeking, Draining, HitStun, Stunned, Dying }

        [Header("Core References")]
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform hpBarFill;
        [SerializeField] private GameObject hpBarRoot;
        [SerializeField] private GameObject groundShadow;

        [Header("Pixel Art Sprites (PPU 32, FullRect, khong dung Animator)")]
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private float animFps = 8f;

        private MothState _state = MothState.Seeking;
        private int _currentHP;
        private int _maxHP = 30;
        private float _stateTimer, _stateDuration = 0.01f, _flashTimer, _animTimer, _hoverPhase;
        private int _animFrame;
        private Vector2 _knockbackVelocity;
        private Vector3 _baseVisualScale = Vector3.one;
        private Vector3 _hpBarBaseScale = new Vector3(0.90f, 0.09f, 1f);
        private BridgeLanternPost _targetLantern;
        private LanternRelayManager _relayManager;
        private LineRenderer _drainBeam;
        private bool _isDefeated;
        private bool _isAlphaMoth;

        public bool IsDefeated => _isDefeated;
        public bool IsStunned => _state == MothState.Stunned;
        public bool IsAlphaMoth => _isAlphaMoth;

        public void Initialize(LanternRelayManager manager) { _relayManager = manager; }

        public void ConfigureAsAlphaMoth()
        {
            _isAlphaMoth = true;
            _maxHP = 95;
            _currentHP = _maxHP;
            transform.localScale = Vector3.one * 1.55f;
            if (hpBarRoot != null) hpBarRoot.SetActive(true);
            if (_drainBeam != null)
            {
                _drainBeam.startColor = new Color(1.0f, 0.22f, 0.65f, 0.95f);
                _drainBeam.endColor = new Color(1.0f, 0.80f, 0.25f, 0.90f);
            }
        }

        private void Start()
        {
            if (!_isAlphaMoth)
            {
                _maxHP = enemyData != null ? enemyData.maxHP : 30;
                _currentHP = _maxHP;
            }
            _hoverPhase = Random.Range(0f, Mathf.PI * 2f);
            if (hpBarFill != null) _hpBarBaseScale = hpBarFill.localScale;
            if (hpBarRoot != null) hpBarRoot.SetActive(_isAlphaMoth);
            if (spriteRenderer != null)
            {
                _baseVisualScale = spriteRenderer.transform.localScale;
                if (idleFrames != null && idleFrames.Length > 0 && idleFrames[0] != null)
                    spriteRenderer.sprite = idleFrames[0];
            }
            EnsureDrainBeam();
            UpdateFootYSorting();
        }

        private void EnsureDrainBeam()
        {
            if (_drainBeam != null) return;
            GameObject beamGo = new GameObject("DrainLightBeam");
            beamGo.transform.SetParent(transform, false);
            _drainBeam = beamGo.AddComponent<LineRenderer>();
            _drainBeam.positionCount = 2;
            _drainBeam.startWidth = _isAlphaMoth ? 0.16f : 0.08f;
            _drainBeam.endWidth = _isAlphaMoth ? 0.26f : 0.15f;
            _drainBeam.useWorldSpace = true;
            _drainBeam.sortingLayerName = "WorldObjects";
            _drainBeam.sortingOrder = 210;
            Shader sh = Shader.Find("Sprites/Default");
            if (sh != null) _drainBeam.material = new Material(sh);
            _drainBeam.startColor = _isAlphaMoth ? new Color(1.0f, 0.22f, 0.65f, 0.95f) : new Color(0.75f, 0.35f, 1.0f, 0.92f);
            _drainBeam.endColor = new Color(1.0f, 0.88f, 0.42f, 0.85f);
            _drainBeam.enabled = false;
        }

        private void Update()
        {
            if (_state == MothState.Dying)
            {
                _stateTimer -= Time.deltaTime;
                UpdateDyingVisual();
                if (_stateTimer <= 0f) Destroy(gameObject);
                return;
            }

            bool active = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!active) return;

            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
                if (spriteRenderer != null && _flashTimer <= 0f) ApplyStateTint();
            }

            if (_state == MothState.HitStun || _state == MothState.Stunned)
            {
                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0f) EnterState(MothState.Seeking, 0f);
            }
            else
            {
                if (_targetLantern == null || !_targetLantern.IsLit)
                    _targetLantern = _relayManager != null ? _relayManager.FindNearestLitLantern(transform.position) : null;

                if (_targetLantern != null)
                {
                    // Bay đậu ngay phía trên mặt ván cầu (y - 0.45) để Wen đứng trên cầu chém trúng 100% không cần lao ra mặt nước!
                    Vector2 targetPos = (Vector2)_targetLantern.transform.position + new Vector2(0f, -0.45f);
                    if (Vector2.Distance(transform.position, targetPos) <= 0.65f)
                    {
                        if (_state != MothState.Draining) EnterState(MothState.Draining, 0f);
                        float drainRate = _relayManager != null ? _relayManager.MothDrainPerSec : 13.0f;
                        if (_isAlphaMoth) drainRate *= 2.0f;
                        _targetLantern.DrainByMoth(drainRate * Time.deltaTime);
                    }
                    else if (_state != MothState.Seeking) EnterState(MothState.Seeking, 0f);
                }
                else if (_state != MothState.Seeking) EnterState(MothState.Seeking, 0f);
            }

            UpdateDrainBeam();
            UpdateVisualFeedback();
            UpdateAnimation();
            UpdateFootYSorting();
        }

        private void UpdateDrainBeam()
        {
            if (_drainBeam == null) return;
            bool draining = _state == MothState.Draining && _targetLantern != null && _targetLantern.IsLit;
            _drainBeam.enabled = draining;
            if (draining)
            {
                float baseW = _isAlphaMoth ? 0.16f : 0.08f;
                float w = baseW + 0.04f * Mathf.Sin(Time.time * 16f);
                _drainBeam.startWidth = w;
                _drainBeam.endWidth = w * 1.6f;
                _drainBeam.SetPosition(0, transform.position + new Vector3(0f, 0.35f, 0f));
                _drainBeam.SetPosition(1, _targetLantern.transform.position + new Vector3(0f, 0.55f, 0f));
            }
        }

        private void FixedUpdate()
        {
            if (rb == null || _isDefeated || _state == MothState.Dying) { if (rb != null) rb.linearVelocity = Vector2.zero; return; }
            bool active = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!active) { rb.linearVelocity = Vector2.zero; return; }

            if (_state == MothState.HitStun)
            {
                rb.linearVelocity = _knockbackVelocity;
                _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 10f);
            }
            else if (_state == MothState.Seeking && _targetLantern != null)
            {
                float speed = enemyData != null ? enemyData.moveSpeed : 2.95f;
                if (_isAlphaMoth) speed *= 0.88f;
                Vector2 targetPos = (Vector2)_targetLantern.transform.position + new Vector2(0f, -0.45f);
                Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
                rb.linearVelocity = dir * speed;
                if (spriteRenderer != null && Mathf.Abs(dir.x) > 0.05f) spriteRenderer.flipX = dir.x < 0f;
            }
            else rb.linearVelocity = Vector2.zero;
        }

        private void EnterState(MothState next, float duration)
        {
            _state = next;
            _stateTimer = duration;
            _stateDuration = Mathf.Max(0.01f, duration);
            if (_flashTimer <= 0f) ApplyStateTint();
        }

        private void ApplyStateTint()
        {
            if (spriteRenderer == null) return;
            if (_isAlphaMoth) spriteRenderer.color = new Color(1.0f, 0.55f, 0.92f, 1f);
            else if (_state == MothState.Draining) spriteRenderer.color = new Color(0.92f, 0.65f, 1.0f, 1f);
            else if (_state == MothState.Stunned) spriteRenderer.color = new Color(1.0f, 0.96f, 0.55f, 1f);
            else spriteRenderer.color = Color.white;
        }

        private void UpdateVisualFeedback()
        {
            if (spriteRenderer == null) return;
            _hoverPhase += Time.deltaTime * (_state == MothState.Draining ? 12f : 7f);
            spriteRenderer.transform.localPosition = new Vector3(0f, 0.35f + Mathf.Sin(_hoverPhase) * 0.12f, 0f);
            if (_state == MothState.Draining)
            {
                float pulse = 1f + 0.12f * Mathf.Sin(Time.time * 14f);
                spriteRenderer.transform.localScale = new Vector3(_baseVisualScale.x * pulse, _baseVisualScale.y * pulse, 1f);
            }
            else if (_state == MothState.HitStun)
            {
                float t = Mathf.Clamp01(_stateTimer / _stateDuration);
                spriteRenderer.transform.localScale = new Vector3(_baseVisualScale.x * (1f + 0.18f * t), _baseVisualScale.y * (1f - 0.15f * t), 1f);
            }
            else spriteRenderer.transform.localScale = Vector3.Lerp(spriteRenderer.transform.localScale, _baseVisualScale, Time.deltaTime * 12f);
        }

        private void UpdateDyingVisual()
        {
            if (spriteRenderer == null) return;
            float norm = 1f - Mathf.Clamp01(_stateTimer / Mathf.Max(0.05f, _stateDuration));
            float s = Mathf.Lerp(1f, 0.15f, norm);
            spriteRenderer.transform.localScale = new Vector3(_baseVisualScale.x * s, _baseVisualScale.y * s, 1f);
            spriteRenderer.color = new Color(0.82f, 0.96f, 1f, Mathf.Lerp(1f, 0f, norm));
        }

        private void UpdateAnimation()
        {
            if (spriteRenderer == null || idleFrames == null || idleFrames.Length == 0) return;
            _animTimer += Time.deltaTime;
            float step = 1f / Mathf.Max(1f, animFps);
            if (_animTimer >= step) { _animTimer -= step; _animFrame = (_animFrame + 1) % idleFrames.Length; }
            Sprite s = idleFrames[Mathf.Clamp(_animFrame, 0, idleFrames.Length - 1)];
            if (s != null) spriteRenderer.sprite = s;
        }

        private void UpdateFootYSorting()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingLayerName = "WorldObjects";
            int yOrder = Mathf.RoundToInt(-(transform.position.y - 0.25f) * 100f) + 20;
            spriteRenderer.sortingOrder = Mathf.Max(195, yOrder);
        }

        public void ApplyStun(float duration)
        {
            if (_isDefeated || _state == MothState.Dying) return;
            EnterState(MothState.Stunned, Mathf.Max(0.5f, duration));
            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.7f, "Choáng!", new Color(1f, 0.94f, 0.45f));
        }

        public void TakePurifyDamage(int amount, Vector2 knockbackDir, float knockbackForce)
        {
            if (_isDefeated || _state == MothState.Dying) return;
            _currentHP = Mathf.Max(0, _currentHP - amount);
            EnterState(MothState.HitStun, _isAlphaMoth ? 0.15f : 0.24f);
            _knockbackVelocity = knockbackDir.normalized * knockbackForce * (_isAlphaMoth ? 0.55f : 1.15f);
            _flashTimer = 0.14f;
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.75f, 1f, 1f, 1f);

            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.6f, "-" + amount, new Color(0.70f, 0.98f, 1f));
                JuiceEffectsManager.Instance.TriggerCameraShake(0.06f, 0.08f);
            }
            if (hpBarRoot != null) hpBarRoot.SetActive(true);
            if (hpBarFill != null)
            {
                float ratio = Mathf.Clamp01((float)_currentHP / Mathf.Max(1, _maxHP));
                hpBarFill.localScale = new Vector3(_hpBarBaseScale.x * ratio, _hpBarBaseScale.y, 1f);
            }
            if (_currentHP <= 0) Die();
        }

        public void ForceDespawnSilent() { _isDefeated = true; Destroy(gameObject); }

        private void Die()
        {
            _isDefeated = true;
            if (_drainBeam != null) _drainBeam.enabled = false;
            EnterState(MothState.Dying, 0.35f);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            if (groundShadow != null) groundShadow.SetActive(false);
            var cols = GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < cols.Length; i++) cols[i].enabled = false;
            InstantRecoveryOrb.TrySpawnRandomDrop(transform.position, _isAlphaMoth ? 1.0f : 0.55f, _isAlphaMoth);
            GameEventBus.RaiseEnemyDefeated(enemyData != null ? enemyData.enemyID : "enemy_lantern_moth");
            if (_relayManager != null) _relayManager.NotifyMothDefeated(this);
        }
    }
}
