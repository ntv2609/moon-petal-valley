using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.Combat
{
    public class ThornCrawlerController : MonoBehaviour, IDamageable, IStunnable
    {
        private enum CrawlerState { Idle, Burrowing, Telegraph, Attacking, Cooldown, HitStun, Stunned, Dying }

        [Header("Core References")]
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform hpBarFill;
        [SerializeField] private GameObject hpBarRoot;
        [SerializeField] private GameObject lootDropPrefab;
        [SerializeField] private GameObject groundShadow;
        [SerializeField] private SpriteRenderer telegraphRingRenderer;
        [SerializeField] private SwampCombatZoneController combatZone;

        [Header("Pixel Art Animations (PPU 32, FullRect, khong dung Animator)")]
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] burrowFrames;
        [SerializeField] private Sprite[] telegraphFrames;
        [SerializeField] private Sprite[] attackFrames;
        [SerializeField] private Sprite[] hitFrames;
        [SerializeField] private Sprite[] dieFrames;
        [SerializeField] private float idleFps = 6f;
        [SerializeField] private float burrowFps = 8f;
        [SerializeField] private float telegraphFps = 7f;
        [SerializeField] private float attackFps = 10f;
        [SerializeField] private float hitFps = 11f;
        [SerializeField] private float dieFps = 8f;

        private CrawlerState _state = CrawlerState.Idle;
        private CrawlerState _lastAnimState = CrawlerState.Idle;
        private int _currentHP;
        private float _stateTimer;
        private float _stateDuration = 0.01f;
        private float _flashTimer;
        private float _animTimer;
        private int _animFrame;
        private Vector2 _moveVelocity;
        private Vector2 _knockbackVelocity;
        private Vector3 _baseVisualScale = Vector3.one;
        private Vector3 _hpBarBaseScale = new Vector3(3.0f, 0.32f, 1f);
        private PlayerController2D _player;
        private bool _isDefeated;
        private bool _attackHitDone;
        private bool _swarmAggro;
        private float _swarmDetectRadius = 60f;

        public bool IsDefeated => _isDefeated;
        public bool IsStunned => _state == CrawlerState.Stunned;
        public void SetCombatZone(SwampCombatZoneController zone) { combatZone = zone; }

        public void ForceSwarmAggro(PlayerController2D target, float overrideDetectRadius = 60f)
        {
            _swarmAggro = true;
            _swarmDetectRadius = Mathf.Max(20f, overrideDetectRadius);
            if (target != null) _player = target;
            if (!_isDefeated && _state == CrawlerState.Idle && _player != null)
            {
                float moveSpd = enemyData != null ? enemyData.moveSpeed : 2.6f;
                float burrowDur = enemyData != null ? enemyData.burrowDuration : 1.15f;
                Vector2 dir = ((Vector2)_player.transform.position - (Vector2)transform.position).normalized;
                _moveVelocity = dir * (moveSpd * 1.35f);
                EnterState(CrawlerState.Burrowing, burrowDur);
                if (spriteRenderer != null && Mathf.Abs(dir.x) > 0.05f) spriteRenderer.flipX = dir.x < 0f;
            }
        }

        private void Start()
        {
            _currentHP = enemyData != null ? enemyData.maxHP : 55;
            if (hpBarFill != null) _hpBarBaseScale = hpBarFill.localScale;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            if (groundShadow == null)
            {
                Transform sh = transform.Find("GroundShadow");
                if (sh != null) groundShadow = sh.gameObject;
            }
            if (spriteRenderer != null)
            {
                _baseVisualScale = spriteRenderer.transform.localScale;
                if (idleFrames != null && idleFrames.Length > 0 && idleFrames[0] != null)
                    spriteRenderer.sprite = idleFrames[0];
            }
            if (telegraphRingRenderer != null) telegraphRingRenderer.enabled = false;
            UpdateFootYSorting();
        }

        private void Update()
        {
            if (_state == CrawlerState.Dying)
            {
                _stateTimer -= Time.deltaTime;
                UpdateAnimation(false);
                UpdateDyingVisual();
                if (_stateTimer <= 0f) Destroy(gameObject);
                return;
            }
            if (_isDefeated) return;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive) { if (rb != null) rb.linearVelocity = Vector2.zero; return; }

            if (_player == null) _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
                if (spriteRenderer != null && _flashTimer <= 0f) ApplyStateTint();
            }

            _stateTimer -= Time.deltaTime;
            float detectR = _swarmAggro ? _swarmDetectRadius : (enemyData != null ? enemyData.detectionRadius : 5.2f);
            float moveSpd = enemyData != null ? enemyData.moveSpeed : 2.6f;
            float burrowDur = enemyData != null ? enemyData.burrowDuration : 1.15f;
            float teleDur = enemyData != null ? enemyData.telegraphDuration : 0.52f;
            float interval = enemyData != null ? enemyData.attackInterval : 1.25f;

            switch (_state)
            {
                case CrawlerState.Idle:
                    if (_player != null && Vector2.Distance(transform.position, _player.transform.position) <= detectR)
                    {
                        Vector2 dir = ((Vector2)_player.transform.position - (Vector2)transform.position).normalized;
                        _moveVelocity = dir * (moveSpd * 1.35f);
                        EnterState(CrawlerState.Burrowing, burrowDur);
                        if (spriteRenderer != null && Mathf.Abs(dir.x) > 0.05f) spriteRenderer.flipX = dir.x < 0f;
                    }
                    break;
                case CrawlerState.Burrowing:
                    if (_player != null)
                    {
                        Vector2 toP = (Vector2)_player.transform.position - (Vector2)transform.position;
                        if (toP.magnitude <= 1.15f || _stateTimer <= 0f)
                        {
                            _moveVelocity = Vector2.zero;
                            EnterState(CrawlerState.Telegraph, teleDur);
                        }
                        else
                        {
                            _moveVelocity = toP.normalized * (moveSpd * 1.35f);
                            if (spriteRenderer != null && Mathf.Abs(toP.x) > 0.05f) spriteRenderer.flipX = toP.x < 0f;
                        }
                    }
                    else if (_stateTimer <= 0f) EnterState(CrawlerState.Idle, 0.3f);
                    break;
                case CrawlerState.Telegraph:
                    if (_stateTimer <= 0f)
                    {
                        _attackHitDone = false;
                        Vector2 lungeDir = _player != null ? ((Vector2)_player.transform.position - (Vector2)transform.position).normalized : Vector2.down;
                        _moveVelocity = lungeDir * (moveSpd * 1.55f);
                        EnterState(CrawlerState.Attacking, 0.34f);
                        TryDealEmergeDamage();
                    }
                    break;
                case CrawlerState.Attacking:
                    if (!_attackHitDone) TryDealEmergeDamage();
                    if (_stateTimer <= 0f) { _moveVelocity = Vector2.zero; EnterState(CrawlerState.Cooldown, interval); }
                    break;
                case CrawlerState.Cooldown:
                case CrawlerState.HitStun:
                case CrawlerState.Stunned:
                    if (_stateTimer <= 0f) EnterState(CrawlerState.Idle, 0f);
                    break;
            }

            UpdateVisualFeedback();
            UpdateAnimation(true);
            UpdateFootYSorting();
        }

        private void EnterState(CrawlerState next, float duration)
        {
            _state = next;
            _stateTimer = duration;
            _stateDuration = Mathf.Max(0.01f, duration);
            if (telegraphRingRenderer != null) telegraphRingRenderer.enabled = (next == CrawlerState.Telegraph);
            if (_flashTimer <= 0f) ApplyStateTint();
        }

        private void ApplyStateTint()
        {
            if (spriteRenderer == null) return;
            if (_state == CrawlerState.Burrowing) spriteRenderer.color = new Color(0.55f, 0.50f, 0.45f, 0.30f);
            else if (_state == CrawlerState.Telegraph) spriteRenderer.color = new Color(1f, 0.88f, 0.58f, 1f);
            else if (_state == CrawlerState.Stunned) spriteRenderer.color = new Color(1f, 0.96f, 0.55f, 1f);
            else spriteRenderer.color = Color.white;
        }

        private void UpdateVisualFeedback()
        {
            if (spriteRenderer == null) return;
            Transform vt = spriteRenderer.transform;
            if (_state == CrawlerState.Burrowing)
            {
                float bob = 1f + 0.08f * Mathf.Sin(Time.time * 16f);
                vt.localScale = new Vector3(_baseVisualScale.x * bob, _baseVisualScale.y * 0.65f, 1f);
            }
            else if (_state == CrawlerState.Telegraph)
            {
                float p = 1f - Mathf.Clamp01(_stateTimer / _stateDuration);
                vt.localScale = new Vector3(_baseVisualScale.x * Mathf.Lerp(1.08f, 1.24f, p), _baseVisualScale.y * Mathf.Lerp(0.92f, 0.78f, p), 1f);
                if (telegraphRingRenderer != null && telegraphRingRenderer.enabled)
                {
                    float rs = Mathf.Lerp(0.45f, 1.35f, p);
                    telegraphRingRenderer.transform.localScale = new Vector3(rs, rs, 1f);
                    telegraphRingRenderer.color = new Color(1f, 0.55f, 0.22f, Mathf.Lerp(0.35f, 0.85f, p));
                }
            }
            else if (_state == CrawlerState.Attacking)
            {
                float p = 1f - Mathf.Clamp01(_stateTimer / _stateDuration);
                vt.localScale = new Vector3(_baseVisualScale.x * Mathf.Lerp(0.84f, 1f, p), _baseVisualScale.y * Mathf.Lerp(1.26f, 1f, p), 1f);
            }
            else if (_state == CrawlerState.HitStun)
            {
                float t = Mathf.Clamp01(_stateTimer / _stateDuration);
                vt.localScale = new Vector3(_baseVisualScale.x * (1f + 0.16f * t), _baseVisualScale.y * (1f - 0.14f * t), 1f);
            }
            else
            {
                vt.localScale = Vector3.Lerp(vt.localScale, _baseVisualScale, Time.deltaTime * 14f);
            }
        }

        private void UpdateDyingVisual()
        {
            if (spriteRenderer == null) return;
            float norm = 1f - Mathf.Clamp01(_stateTimer / Mathf.Max(0.05f, _stateDuration));
            if (dieFrames == null || dieFrames.Length <= 1)
            {
                float s = Mathf.Lerp(1f, 0.15f, norm);
                spriteRenderer.transform.localScale = new Vector3(_baseVisualScale.x * s, _baseVisualScale.y * s, 1f);
            }
            spriteRenderer.color = new Color(0.75f, 1f, 1f, Mathf.Lerp(1f, 0f, norm));
        }

        private void TryDealEmergeDamage()
        {
            if (_player == null || _player.IsInvulnerable) return;
            if (Vector2.Distance(transform.position, _player.transform.position) > 1.35f) return;
            _attackHitDone = true;
            DealContactDamageToPlayer(_player);
        }

        private void DealContactDamageToPlayer(PlayerController2D player)
        {
            int dmg = enemyData != null ? enemyData.contactDamage : 12;
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
                GameRoot.Instance.Stats.TakeLightDamage(dmg, false);
            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.SpawnFloatingNumber(player.transform.position + Vector3.up * 0.7f, "-" + dmg, new Color(1f, 0.42f, 0.36f));
                JuiceEffectsManager.Instance.TriggerCameraShake(0.14f, 0.16f);
            }
            if (enemyData == null || enemyData.appliesSlowOnHit) player.ApplySlow();
        }

        private void UpdateFootYSorting()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingLayerName = "WorldObjects";
            float footY = spriteRenderer.sprite != null ? spriteRenderer.bounds.min.y : (transform.position.y - 0.35f);
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-footY * 100f);
        }

        private void UpdateAnimation(bool loop)
        {
            if (spriteRenderer == null) return;
            if (_state != _lastAnimState) { _lastAnimState = _state; _animFrame = 0; _animTimer = 0f; }

            Sprite[] frames = idleFrames;
            float fps = idleFps;
            bool shouldLoop = loop;
            switch (_state)
            {
                case CrawlerState.Burrowing: frames = HasFrames(burrowFrames) ? burrowFrames : idleFrames; fps = burrowFps; shouldLoop = true; break;
                case CrawlerState.Telegraph: frames = HasFrames(telegraphFrames) ? telegraphFrames : idleFrames; fps = telegraphFps; shouldLoop = false; break;
                case CrawlerState.Attacking: frames = HasFrames(attackFrames) ? attackFrames : idleFrames; fps = attackFps; shouldLoop = false; break;
                case CrawlerState.HitStun: frames = HasFrames(hitFrames) ? hitFrames : idleFrames; fps = hitFps; shouldLoop = false; break;
                case CrawlerState.Dying: frames = HasFrames(dieFrames) ? dieFrames : idleFrames; fps = dieFps; shouldLoop = false; break;
            }
            if (!HasFrames(frames)) return;

            _animTimer += Time.deltaTime;
            float step = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= step)
            {
                _animTimer -= step;
                _animFrame = shouldLoop ? ((_animFrame + 1) % frames.Length) : Mathf.Min(_animFrame + 1, frames.Length - 1);
            }
            Sprite s = frames[Mathf.Clamp(_animFrame, 0, frames.Length - 1)];
            if (s != null) spriteRenderer.sprite = s;
        }

        private static bool HasFrames(Sprite[] arr) { return arr != null && arr.Length > 0; }

        private void FixedUpdate()
        {
            if (rb == null || _isDefeated || _state == CrawlerState.Dying) { if (rb != null) rb.linearVelocity = Vector2.zero; return; }
            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive) { rb.linearVelocity = Vector2.zero; return; }

            if (_state == CrawlerState.HitStun)
            {
                rb.linearVelocity = _knockbackVelocity;
                _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 10f);
            }
            else if (_state == CrawlerState.Burrowing || _state == CrawlerState.Attacking) rb.linearVelocity = _moveVelocity;
            else rb.linearVelocity = Vector2.zero;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isDefeated || _state == CrawlerState.Dying || _state == CrawlerState.Burrowing || _state == CrawlerState.Stunned) return;
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController2D>();
            if (player == null || player.IsInvulnerable) return;
            DealContactDamageToPlayer(player);
        }

        public void ApplyStun(float duration)
        {
            if (_isDefeated || _state == CrawlerState.Dying) return;
            _moveVelocity = Vector2.zero;
            EnterState(CrawlerState.Stunned, Mathf.Max(0.4f, duration));
            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.65f, "Choáng!", new Color(1f, 0.94f, 0.45f));
        }

        public void TakePurifyDamage(int amount, Vector2 knockbackDir, float knockbackForce)
        {
            if (_isDefeated || _state == CrawlerState.Dying) return;
            int appliedDmg = _state == CrawlerState.Burrowing ? Mathf.Max(1, amount / 2) : amount;
            int maxHP = enemyData != null ? enemyData.maxHP : 55;
            _currentHP = Mathf.Max(0, _currentHP - appliedDmg);

            EnterState(CrawlerState.HitStun, 0.28f);
            _knockbackVelocity = knockbackDir.normalized * knockbackForce;
            _flashTimer = 0.16f;
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.75f, 1f, 1f, 1f);

            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.55f, "-" + appliedDmg, new Color(0.70f, 0.98f, 1f));
                JuiceEffectsManager.Instance.TriggerCameraShake(0.08f, 0.09f);
            }
            if (hpBarRoot != null) hpBarRoot.SetActive(true);
            if (hpBarFill != null)
            {
                float ratio = Mathf.Clamp01((float)_currentHP / Mathf.Max(1, maxHP));
                hpBarFill.localScale = new Vector3(_hpBarBaseScale.x * ratio, _hpBarBaseScale.y, 1f);
            }
            if (_currentHP <= 0) DieAndSpawnLoot();
        }

        private void DieAndSpawnLoot()
        {
            _isDefeated = true;
            float dieDur = (dieFrames != null && dieFrames.Length > 1) ? (dieFrames.Length / Mathf.Max(1f, dieFps)) + 0.05f : 0.45f;
            EnterState(CrawlerState.Dying, dieDur);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            if (groundShadow != null) groundShadow.SetActive(false);
            if (telegraphRingRenderer != null) telegraphRingRenderer.enabled = false;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;

            var cols = GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < cols.Length; i++) cols[i].enabled = false;

            SpawnLoot();
            GameEventBus.RaiseEnemyDefeated(enemyData != null ? enemyData.enemyID : "enemy_thorn_crawler");
            if (combatZone != null) combatZone.NotifyEnemyDefeated(gameObject);
        }

        private void SpawnLoot()
        {
            if (enemyData == null || enemyData.dropTable == null) return;
            for (int i = 0; i < enemyData.dropTable.Count; i++)
            {
                var entry = enemyData.dropTable[i];
                if (entry.item == null || Random.value > entry.dropChance) continue;
                int amt = Random.Range(Mathf.Max(1, entry.minAmount), Mathf.Max(entry.minAmount, entry.maxAmount) + 1);
                if (lootDropPrefab != null)
                {
                    for (int k = 0; k < amt; k++)
                    {
                        Vector2 off = new Vector2(Random.Range(-0.4f, 0.4f), Random.Range(-0.3f, 0.3f));
                        GameObject go = Instantiate(lootDropPrefab, transform.position + (Vector3)off, Quaternion.identity);
                        var drop = go.GetComponent<WorldLootDrop>();
                        if (drop != null) drop.Initialize(entry.item, 1);
                    }
                }
                else if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
                    GameRoot.Instance.Inventory.AddItem(entry.item, amt, true);
            }
        }
    }
}
