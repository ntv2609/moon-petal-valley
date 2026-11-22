using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;

namespace MoonpetalValley.Combat
{
    public class GloomSlimeController : MonoBehaviour, IDamageable
    {
        private enum SlimeState
        {
            Idle,
            Telegraph,
            Jumping,
            Cooldown,
            HitStun,
            Dying
        }

        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform hpBarFill;
        [SerializeField] private GameObject hpBarRoot;
        [SerializeField] private GameObject lootDropPrefab;
        [SerializeField] private BlightEncounterZone encounterZone;

        [Header("Animation Frames")]
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] telegraphFrames;
        [SerializeField] private Sprite[] jumpFrames;
        [SerializeField] private Sprite[] hitHurtFrames;
        [SerializeField] private Sprite[] purifyDieFrames;
        [SerializeField] private float idleFps = 6f;
        [SerializeField] private float telegraphFps = 9f;
        [SerializeField] private float jumpFps = 10f;
        [SerializeField] private float hitFps = 13f;
        [SerializeField] private float dieFps = 10f;

        private SlimeState _state = SlimeState.Idle;
        private SlimeState _lastAnimState = SlimeState.Idle;
        private int _currentHP;
        private float _stateTimer;
        private float _flashTimer;
        private float _animTimer;
        private int _frameIndex;
        private Vector2 _jumpVelocity;
        private Vector2 _knockbackVelocity;
        private PlayerController2D _player;
        private bool _isDefeated;

        public bool IsDefeated => _isDefeated;
        private bool HasSpriteAnimations => idleFrames != null && idleFrames.Length > 0;

        private void Start()
        {
            _currentHP = enemyData != null ? enemyData.maxHP : 45;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            ApplyAnimFrame();
        }

        private void Update()
        {
            if (_state == SlimeState.Dying)
            {
                TickAnimation(Time.deltaTime);
                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0f)
                {
                    Destroy(gameObject);
                }
                return;
            }

            if (_isDefeated) return;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_player == null) _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();

            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
                if (spriteRenderer != null && _flashTimer <= 0f) spriteRenderer.color = Color.white;
            }

            _stateTimer -= Time.deltaTime;

            float detectR = enemyData != null ? enemyData.detectionRadius : 4.5f;
            float moveSpd = enemyData != null ? enemyData.moveSpeed : 2.8f;
            float interval = enemyData != null ? enemyData.attackInterval : 1.1f;

            switch (_state)
            {
                case SlimeState.Idle:
                    if (_player != null && Vector2.Distance(transform.position, _player.transform.position) <= detectR)
                    {
                        _state = SlimeState.Telegraph;
                        _stateTimer = 0.35f;
                        if (spriteRenderer != null)
                        {
                            if (!HasSpriteAnimations) spriteRenderer.transform.localScale = new Vector3(1.22f, 0.78f, 1f);
                            spriteRenderer.color = new Color(1f, 0.90f, 0.65f, 1f);
                        }
                    }
                    break;

                case SlimeState.Telegraph:
                    if (_stateTimer <= 0f)
                    {
                        Vector2 dir = _player != null ? ((Vector2)_player.transform.position - (Vector2)transform.position).normalized : Vector2.down;
                        _jumpVelocity = dir * (moveSpd * 1.45f);
                        _state = SlimeState.Jumping;
                        _stateTimer = 0.42f;
                        if (spriteRenderer != null)
                        {
                            if (!HasSpriteAnimations) spriteRenderer.transform.localScale = new Vector3(0.88f, 1.18f, 1f);
                            spriteRenderer.color = Color.white;
                        }
                    }
                    break;

                case SlimeState.Jumping:
                    if (_stateTimer <= 0f)
                    {
                        _state = SlimeState.Cooldown;
                        _stateTimer = interval;
                        if (spriteRenderer != null) spriteRenderer.transform.localScale = Vector3.one;
                    }
                    break;

                case SlimeState.Cooldown:
                case SlimeState.HitStun:
                    if (_stateTimer <= 0f)
                    {
                        _state = SlimeState.Idle;
                        if (spriteRenderer != null) spriteRenderer.transform.localScale = Vector3.one;
                    }
                    break;
            }

            TickAnimation(Time.deltaTime);

            if (spriteRenderer != null) spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
        }

        private void FixedUpdate()
        {
            if (rb == null || _isDefeated || _state == SlimeState.Dying)
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
                return;
            }

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive) { rb.linearVelocity = Vector2.zero; return; }

            if (_state == SlimeState.HitStun)
            {
                rb.linearVelocity = _knockbackVelocity;
                _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 10f);
            }
            else if (_state == SlimeState.Jumping) rb.linearVelocity = _jumpVelocity;
            else rb.linearVelocity = Vector2.zero;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isDefeated || _state == SlimeState.Dying || !other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController2D>();
            if (player == null || player.IsInvulnerable) return;

            int dmg = enemyData != null ? enemyData.contactDamage : 10;
            GameRoot.Instance?.Stats?.TakeLightDamage(dmg, false);
            JuiceEffectsManager.Instance?.SpawnFloatingNumber(player.transform.position + Vector3.up * 0.7f, "-" + dmg, new Color(1f, 0.42f, 0.36f));
            JuiceEffectsManager.Instance?.TriggerCameraShake(0.14f, 0.16f);

            if (enemyData == null || enemyData.appliesSlowOnHit) player.ApplySlow();
        }

        public void TakePurifyDamage(int amount, Vector2 knockbackDir, float knockbackForce)
        {
            if (_isDefeated || _state == SlimeState.Dying) return;

            int maxHP = enemyData != null ? enemyData.maxHP : 45;
            _currentHP = Mathf.Max(0, _currentHP - amount);

            _state = SlimeState.HitStun;
            _stateTimer = 0.22f;
            _knockbackVelocity = knockbackDir.normalized * knockbackForce;

            _flashTimer = 0.14f;
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.75f, 1f, 1f, 1f);

            JuiceEffectsManager.Instance?.SpawnFloatingNumber(transform.position + Vector3.up * 0.55f, "-" + amount, new Color(0.70f, 0.98f, 1f));
            JuiceEffectsManager.Instance?.TriggerCameraShake(0.08f, 0.09f);

            if (hpBarRoot != null) hpBarRoot.SetActive(true);
            if (hpBarFill != null) hpBarFill.localScale = new Vector3(Mathf.Clamp01((float)_currentHP / maxHP), 1f, 1f);

            if (_currentHP <= 0) DieAndSpawnLoot();
        }

        private void DieAndSpawnLoot()
        {
            _isDefeated = true;
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);

            var cols = GetComponentsInChildren<Collider2D>(true);
            for (int i = 0; i < cols.Length; i++) cols[i].enabled = false;

            SpawnLoot();
            string id = enemyData != null ? enemyData.enemyID : "enemy_gloom_slime";
            GameEventBus.RaiseEnemyDefeated(id);
            encounterZone?.NotifySlimeDefeated(this);

            if (purifyDieFrames != null && purifyDieFrames.Length > 0 && spriteRenderer != null)
            {
                _state = SlimeState.Dying;
                _lastAnimState = SlimeState.Dying;
                _frameIndex = 0;
                _animTimer = 0f;
                spriteRenderer.color = Color.white;
                spriteRenderer.transform.localScale = Vector3.one;
                ApplyAnimFrame();
                _stateTimer = (purifyDieFrames.Length / Mathf.Max(1f, dieFps)) + 0.05f;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void TickAnimation(float dt)
        {
            if (!HasSpriteAnimations || spriteRenderer == null) return;

            SlimeState visualState = _state == SlimeState.Cooldown ? SlimeState.Idle : _state;
            if (visualState != _lastAnimState)
            {
                _lastAnimState = visualState;
                _frameIndex = 0;
                _animTimer = 0f;
                ApplyAnimFrame();
                return;
            }

            Sprite[] frames = GetFramesForState(visualState);
            if (frames == null || frames.Length == 0) return;

            float fps = GetFpsForState(visualState);
            float dur = fps > 0.01f ? (1f / fps) : 0.15f;
            _animTimer += dt;

            if (_animTimer >= dur)
            {
                _animTimer -= dur;
                _frameIndex++;
                if (_frameIndex >= frames.Length)
                {
                    _frameIndex = visualState == SlimeState.Idle ? 0 : frames.Length - 1;
                }
                ApplyAnimFrame();
            }
        }

        private void ApplyAnimFrame()
        {
            if (!HasSpriteAnimations || spriteRenderer == null) return;
            SlimeState visualState = _state == SlimeState.Cooldown ? SlimeState.Idle : _state;
            Sprite[] frames = GetFramesForState(visualState);
            if (frames == null || frames.Length == 0) return;

            int idx = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
            if (frames[idx] != null) spriteRenderer.sprite = frames[idx];
        }

        private Sprite[] GetFramesForState(SlimeState st)
        {
            return st switch
            {
                SlimeState.Telegraph => (telegraphFrames != null && telegraphFrames.Length > 0) ? telegraphFrames : idleFrames,
                SlimeState.Jumping => (jumpFrames != null && jumpFrames.Length > 0) ? jumpFrames : idleFrames,
                SlimeState.HitStun => (hitHurtFrames != null && hitHurtFrames.Length > 0) ? hitHurtFrames : idleFrames,
                SlimeState.Dying => (purifyDieFrames != null && purifyDieFrames.Length > 0) ? purifyDieFrames : idleFrames,
                _ => idleFrames
            };
        }

        private float GetFpsForState(SlimeState st)
        {
            return st switch
            {
                SlimeState.Telegraph => telegraphFps,
                SlimeState.Jumping => jumpFps,
                SlimeState.HitStun => hitFps,
                SlimeState.Dying => dieFps,
                _ => idleFps
            };
        }

        private void SpawnLoot()
        {
            if (enemyData == null || enemyData.dropTable == null) return;
            for (int i = 0; i < enemyData.dropTable.Count; i++)
            {
                var entry = enemyData.dropTable[i];
                if (entry.item == null) continue;
                if (Random.value <= entry.dropChance)
                {
                    int amt = Random.Range(Mathf.Max(1, entry.minAmount), Mathf.Max(entry.minAmount, entry.maxAmount) + 1);
                    if (lootDropPrefab != null)
                    {
                        for (int k = 0; k < amt; k++)
                        {
                            Vector2 off = new Vector2(Random.Range(-0.4f, 0.4f), Random.Range(-0.3f, 0.3f));
                            GameObject go = Instantiate(lootDropPrefab, transform.position + (Vector3)off, Quaternion.identity);
                            go.GetComponent<WorldLootDrop>()?.Initialize(entry.item, 1);
                        }
                    }
                    else GameRoot.Instance?.Inventory?.AddItem(entry.item, amt, true);
                }
            }
        }
    }
}
