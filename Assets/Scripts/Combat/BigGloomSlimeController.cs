using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.Combat
{
    public class BigGloomSlimeController : MonoBehaviour, IDamageable, IStunnable
    {
        private enum BigSlimeState { Idle, Approaching, Telegraph, MudWave, Cooldown, HitStun, Stunned, Dying }

        [Header("Core References")]
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform hpBarFill;
        [SerializeField] private GameObject hpBarRoot;
        [SerializeField] private GameObject lootDropPrefab;
        [SerializeField] private GameObject miniSlimePrefab;
        [SerializeField] private GameObject groundShadow;
        [SerializeField] private SpriteRenderer waveRingRenderer;
        [SerializeField] private SwampCombatZoneController combatZone;

        [Header("Pixel Art Animations (PPU 32, FullRect, khong dung Animator)")]
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] moveFrames;
        [SerializeField] private Sprite[] telegraphFrames;
        [SerializeField] private Sprite[] attackFrames;
        [SerializeField] private Sprite[] hitFrames;
        [SerializeField] private Sprite[] dieFrames;
        [SerializeField] private float idleFps = 6f;
        [SerializeField] private float moveFps = 8f;
        [SerializeField] private float telegraphFps = 7f;
        [SerializeField] private float attackFps = 10f;
        [SerializeField] private float hitFps = 11f;
        [SerializeField] private float dieFps = 8f;

        private BigSlimeState _state = BigSlimeState.Idle;
        private BigSlimeState _lastAnimState = BigSlimeState.Idle;
        private int _currentHP;
        private float _stateTimer;
        private float _stateDuration = 0.01f;
        private float _flashTimer;
        private float _animTimer;
        private int _animFrame;
        private Vector2 _moveVelocity;
        private Vector2 _knockbackVelocity;
        private Vector3 _baseVisualScale = Vector3.one;
        private Vector3 _hpBarBaseScale = new Vector3(4.2f, 0.36f, 1f);
        private PlayerController2D _player;
        private bool _isDefeated;
        private bool _hasSplit;
        private bool _swarmAggro;
        private float _swarmDetectRadius = 60f;

        public bool IsDefeated => _isDefeated;
        public bool IsStunned => _state == BigSlimeState.Stunned;
        public void SetCombatZone(SwampCombatZoneController zone) { combatZone = zone; }

        public void ForceSwarmAggro(PlayerController2D target, float overrideDetectRadius = 60f)
        {
            _swarmAggro = true;
            _swarmDetectRadius = Mathf.Max(20f, overrideDetectRadius);
            if (target != null) _player = target;
            if (!_isDefeated && _state == BigSlimeState.Idle && _player != null)
            {
                EnterState(BigSlimeState.Approaching, 1.3f);
            }
        }

        private void Start()
        {
            _currentHP = enemyData != null ? enemyData.maxHP : 120;
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
            if (waveRingRenderer != null) waveRingRenderer.enabled = false;
            UpdateFootYSorting();
        }

        private void Update()
        {
            if (_state == BigSlimeState.Dying)
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
            float detectR = _swarmAggro ? _swarmDetectRadius : (enemyData != null ? enemyData.detectionRadius : 6.0f);
            float moveSpd = enemyData != null ? enemyData.moveSpeed : 1.75f;
            float teleDur = enemyData != null ? enemyData.telegraphDuration : 0.70f;
            float waveR = enemyData != null ? enemyData.mudWaveRadius : 2.5f;
            float interval = enemyData != null ? enemyData.attackInterval : 1.6f;

            switch (_state)
            {
                case BigSlimeState.Idle:
                    if (_player != null)
                    {
                        float dist = Vector2.Distance(transform.position, _player.transform.position);
                        if (dist <= waveR * 0.95f) EnterState(BigSlimeState.Telegraph, teleDur);
                        else if (dist <= detectR) EnterState(BigSlimeState.Approaching, 1.3f);
                    }
                    break;
                case BigSlimeState.Approaching:
                    if (_player != null)
                    {
                        Vector2 toP = (Vector2)_player.transform.position - (Vector2)transform.position;
                        if (spriteRenderer != null && Mathf.Abs(toP.x) > 0.05f) spriteRenderer.flipX = toP.x < 0f;
                        if (toP.magnitude <= waveR * 0.9f || _stateTimer <= 0f)
                        {
                            _moveVelocity = Vector2.zero;
                            EnterState(BigSlimeState.Telegraph, teleDur);
                        }
                        else _moveVelocity = toP.normalized * moveSpd;
                    }
                    else { _moveVelocity = Vector2.zero; EnterState(BigSlimeState.Idle, 0.2f); }
                    break;
                case BigSlimeState.Telegraph:
                    if (_player != null && spriteRenderer != null) spriteRenderer.flipX = _player.transform.position.x < transform.position.x;
                    if (_stateTimer <= 0f) { EnterState(BigSlimeState.MudWave, 0.42f); ExecuteMudWave(waveR); }
                    break;
                case BigSlimeState.MudWave:
                    if (_stateTimer <= 0f) EnterState(BigSlimeState.Cooldown, interval);
                    break;
                case BigSlimeState.Cooldown:
                case BigSlimeState.HitStun:
                case BigSlimeState.Stunned:
                    if (_stateTimer <= 0f) EnterState(BigSlimeState.Idle, 0f);
                    break;
            }

            UpdateVisualFeedback(waveR);
            UpdateAnimation(true);
            UpdateFootYSorting();
        }

        private void EnterState(BigSlimeState next, float duration)
        {
            _state = next;
            _stateTimer = duration;
            _stateDuration = Mathf.Max(0.01f, duration);
            if (waveRingRenderer != null) waveRingRenderer.enabled = (next == BigSlimeState.Telegraph || next == BigSlimeState.MudWave);
            if (_flashTimer <= 0f) ApplyStateTint();
        }

        private void ApplyStateTint()
        {
            if (spriteRenderer == null) return;
            if (_state == BigSlimeState.Telegraph) spriteRenderer.color = new Color(1f, 0.85f, 0.62f, 1f);
            else if (_state == BigSlimeState.Stunned) spriteRenderer.color = new Color(1f, 0.96f, 0.55f, 1f);
            else spriteRenderer.color = Color.white;
        }

        private void ExecuteMudWave(float waveR)
        {
            if (JuiceEffectsManager.Instance != null) JuiceEffectsManager.Instance.TriggerCameraShake(0.16f, 0.18f);
            if (_player == null || _player.IsInvulnerable) return;
            if (Vector2.Distance(transform.position, _player.transform.position) <= waveR)
            {
                int dmg = enemyData != null ? enemyData.contactDamage : 15;
                if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
                    GameRoot.Instance.Stats.TakeLightDamage(dmg, false);
                if (JuiceEffectsManager.Instance != null)
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(_player.transform.position + Vector3.up * 0.7f, "-" + dmg, new Color(1f, 0.42f, 0.36f));
                if (enemyData == null || enemyData.appliesSlowOnHit) _player.ApplySlow();
            }
        }

        private void UpdateVisualFeedback(float waveR)
        {
            if (spriteRenderer == null) return;
            Transform vt = spriteRenderer.transform;
            if (_state == BigSlimeState.Telegraph)
            {
                float p = 1f - Mathf.Clamp01(_stateTimer / _stateDuration);
                vt.localScale = new Vector3(_baseVisualScale.x * Mathf.Lerp(1.06f, 1.28f, p), _baseVisualScale.y * Mathf.Lerp(0.94f, 0.74f, p), 1f);
                if (waveRingRenderer != null && waveRingRenderer.enabled)
                {
                    float s = Mathf.Lerp(0.6f, waveR * 1.15f, p);
                    waveRingRenderer.transform.localScale = new Vector3(s, s, 1f);
                    waveRingRenderer.color = new Color(0.82f, 0.36f, 0.92f, Mathf.Lerp(0.25f, 0.78f, p));
                }
            }
            else if (_state == BigSlimeState.MudWave)
            {
                float p = 1f - Mathf.Clamp01(_stateTimer / _stateDuration);
                vt.localScale = new Vector3(_baseVisualScale.x * Mathf.Lerp(0.84f, 1f, p), _baseVisualScale.y * Mathf.Lerp(1.25f, 1f, p), 1f);
                if (waveRingRenderer != null && waveRingRenderer.enabled)
                {
                    float s = Mathf.Lerp(waveR * 0.9f, waveR * 1.65f, p);
                    waveRingRenderer.transform.localScale = new Vector3(s, s, 1f);
                    waveRingRenderer.color = new Color(0.68f, 0.24f, 0.88f, Mathf.Lerp(0.92f, 0f, p));
                }
            }
            else if (_state == BigSlimeState.HitStun)
            {
                float t = Mathf.Clamp01(_stateTimer / _stateDuration);
                vt.localScale = new Vector3(_baseVisualScale.x * (1f + 0.14f * t), _baseVisualScale.y * (1f - 0.12f * t), 1f);
            }
            else vt.localScale = Vector3.Lerp(vt.localScale, _baseVisualScale, Time.deltaTime * 12f);
        }

        private void UpdateDyingVisual()
        {
            if (spriteRenderer == null) return;
            float norm = 1f - Mathf.Clamp01(_stateTimer / Mathf.Max(0.05f, _stateDuration));
            if (dieFrames == null || dieFrames.Length <= 1)
            {
                float s = Mathf.Lerp(1f, 0.18f, norm);
                spriteRenderer.transform.localScale = new Vector3(_baseVisualScale.x * s, _baseVisualScale.y * s, 1f);
            }
            spriteRenderer.color = new Color(0.75f, 1f, 1f, Mathf.Lerp(1f, 0f, norm));
        }

        private void UpdateFootYSorting()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingLayerName = "WorldObjects";
            float footY = spriteRenderer.sprite != null ? spriteRenderer.bounds.min.y : (transform.position.y - 0.55f);
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
                case BigSlimeState.Approaching: frames = HasFrames(moveFrames) ? moveFrames : idleFrames; fps = moveFps; shouldLoop = true; break;
                case BigSlimeState.Telegraph: frames = HasFrames(telegraphFrames) ? telegraphFrames : idleFrames; fps = telegraphFps; shouldLoop = false; break;
                case BigSlimeState.MudWave: frames = HasFrames(attackFrames) ? attackFrames : idleFrames; fps = attackFps; shouldLoop = false; break;
                case BigSlimeState.HitStun: frames = HasFrames(hitFrames) ? hitFrames : idleFrames; fps = hitFps; shouldLoop = false; break;
                case BigSlimeState.Dying: frames = HasFrames(dieFrames) ? dieFrames : idleFrames; fps = dieFps; shouldLoop = false; break;
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
            if (rb == null || _isDefeated || _state == BigSlimeState.Dying) { if (rb != null) rb.linearVelocity = Vector2.zero; return; }
            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive) { rb.linearVelocity = Vector2.zero; return; }

            if (_state == BigSlimeState.HitStun)
            {
                rb.linearVelocity = _knockbackVelocity;
                _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 10f);
            }
            else if (_state == BigSlimeState.Approaching) rb.linearVelocity = _moveVelocity;
            else rb.linearVelocity = Vector2.zero;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isDefeated || _state == BigSlimeState.Dying || _state == BigSlimeState.Stunned) return;
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController2D>();
            if (player == null || player.IsInvulnerable) return;

            int dmg = enemyData != null ? enemyData.contactDamage : 15;
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
                GameRoot.Instance.Stats.TakeLightDamage(dmg, false);
            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.SpawnFloatingNumber(player.transform.position + Vector3.up * 0.7f, "-" + dmg, new Color(1f, 0.42f, 0.36f));
                JuiceEffectsManager.Instance.TriggerCameraShake(0.14f, 0.16f);
            }
            if (enemyData == null || enemyData.appliesSlowOnHit) player.ApplySlow();
        }

        public void ApplyStun(float duration)
        {
            if (_isDefeated || _state == BigSlimeState.Dying) return;
            _moveVelocity = Vector2.zero;
            EnterState(BigSlimeState.Stunned, Mathf.Max(0.4f, duration));
            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.85f, "Choáng!", new Color(1f, 0.94f, 0.45f));
        }

        public void TakePurifyDamage(int amount, Vector2 knockbackDir, float knockbackForce)
        {
            if (_isDefeated || _state == BigSlimeState.Dying) return;
            int maxHP = enemyData != null ? enemyData.maxHP : 120;
            _currentHP = Mathf.Max(0, _currentHP - amount);

            EnterState(BigSlimeState.HitStun, 0.26f);
            _knockbackVelocity = knockbackDir.normalized * (knockbackForce * 0.65f);
            _flashTimer = 0.16f;
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.75f, 1f, 1f, 1f);

            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.78f, "-" + amount, new Color(0.70f, 0.98f, 1f));
                JuiceEffectsManager.Instance.TriggerCameraShake(0.09f, 0.10f);
            }
            if (hpBarRoot != null) hpBarRoot.SetActive(true);
            if (hpBarFill != null)
            {
                float ratio = Mathf.Clamp01((float)_currentHP / Mathf.Max(1, maxHP));
                hpBarFill.localScale = new Vector3(_hpBarBaseScale.x * ratio, _hpBarBaseScale.y, 1f);
            }

            float splitThreshold = enemyData != null ? enemyData.splitHpRatio : 0.5f;
            if (!_hasSplit && _currentHP > 0 && _currentHP <= Mathf.RoundToInt(maxHP * splitThreshold))
                TrySplitMiniSlimes();
            if (_currentHP <= 0) DieAndSpawnLoot();
        }

        private void TrySplitMiniSlimes()
        {
            _hasSplit = true;
            if (JuiceEffectsManager.Instance != null)
            {
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 1.05f, "Phân tách!", new Color(0.86f, 0.58f, 1f));
                JuiceEffectsManager.Instance.TriggerCameraShake(0.12f, 0.14f);
            }
            if (miniSlimePrefab == null) return;
            GameObject m1 = Instantiate(miniSlimePrefab, transform.position + new Vector3(-0.9f, -0.35f, 0f), Quaternion.identity);
            GameObject m2 = Instantiate(miniSlimePrefab, transform.position + new Vector3(0.9f, -0.35f, 0f), Quaternion.identity);
            if (_swarmAggro)
            {
                GloomSlimeController gs1 = m1 != null ? m1.GetComponent<GloomSlimeController>() : null;
                if (gs1 != null) gs1.ForceSwarmAggro(_player, _swarmDetectRadius);
                GloomSlimeController gs2 = m2 != null ? m2.GetComponent<GloomSlimeController>() : null;
                if (gs2 != null) gs2.ForceSwarmAggro(_player, _swarmDetectRadius);
            }
        }

        private void DieAndSpawnLoot()
        {
            _isDefeated = true;
            float dieDur = (dieFrames != null && dieFrames.Length > 1) ? (dieFrames.Length / Mathf.Max(1f, dieFps)) + 0.05f : 0.55f;
            EnterState(BigSlimeState.Dying, dieDur);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            if (groundShadow != null) groundShadow.SetActive(false);
            if (waveRingRenderer != null) waveRingRenderer.enabled = false;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;

            var cols = GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < cols.Length; i++) cols[i].enabled = false;

            SpawnLoot();
            GameEventBus.RaiseEnemyDefeated(enemyData != null ? enemyData.enemyID : "enemy_big_gloom_slime");
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
                        Vector2 off = new Vector2(Random.Range(-0.45f, 0.45f), Random.Range(-0.35f, 0.35f));
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
