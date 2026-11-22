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
            HitStun
        }

        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform hpBarFill;
        [SerializeField] private GameObject hpBarRoot;
        [SerializeField] private GameObject lootDropPrefab;
        [SerializeField] private BlightEncounterZone encounterZone;

        private SlimeState _state = SlimeState.Idle;
        private int _currentHP;
        private float _stateTimer;
        private float _flashTimer;
        private Vector2 _jumpVelocity;
        private Vector2 _knockbackVelocity;
        private PlayerController2D _player;
        private bool _isDefeated;

        public bool IsDefeated => _isDefeated;

        private void Start()
        {
            _currentHP = enemyData != null ? enemyData.maxHP : 45;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
        }

        private void Update()
        {
            if (_isDefeated) return;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            }

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
                            spriteRenderer.transform.localScale = new Vector3(1.22f, 0.78f, 1f);
                            spriteRenderer.color = new Color(1f, 0.86f, 0.55f, 1f);
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
                            spriteRenderer.transform.localScale = new Vector3(0.88f, 1.18f, 1f);
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

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
            }
        }

        private void FixedUpdate()
        {
            if (rb == null || _isDefeated) return;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_state == SlimeState.HitStun)
            {
                rb.linearVelocity = _knockbackVelocity;
                _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 10f);
            }
            else if (_state == SlimeState.Jumping)
            {
                rb.linearVelocity = _jumpVelocity;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isDefeated) return;
            if (!other.CompareTag("Player")) return;

            var player = other.GetComponent<PlayerController2D>();
            if (player == null || player.IsInvulnerable) return;

            int dmg = enemyData != null ? enemyData.contactDamage : 10;
            GameRoot.Instance?.Stats?.TakeLightDamage(dmg, false);

            if (enemyData == null || enemyData.appliesSlowOnHit)
            {
                player.ApplySlow();
            }
        }

        public void TakePurifyDamage(int amount, Vector2 knockbackDir, float knockbackForce)
        {
            if (_isDefeated) return;

            int maxHP = enemyData != null ? enemyData.maxHP : 45;
            _currentHP = Mathf.Max(0, _currentHP - amount);

            _state = SlimeState.HitStun;
            _stateTimer = 0.22f;
            _knockbackVelocity = knockbackDir.normalized * knockbackForce;

            _flashTimer = 0.14f;
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.75f, 1f, 1f, 1f);

            if (hpBarRoot != null) hpBarRoot.SetActive(true);
            if (hpBarFill != null)
            {
                float ratio = Mathf.Clamp01((float)_currentHP / maxHP);
                hpBarFill.localScale = new Vector3(ratio, 1f, 1f);
            }

            if (_currentHP <= 0)
            {
                DieAndSpawnLoot();
            }
        }

        private void DieAndSpawnLoot()
        {
            _isDefeated = true;
            SpawnLoot();

            string id = enemyData != null ? enemyData.enemyID : "enemy_gloom_slime";
            GameEventBus.RaiseEnemyDefeated(id);

            if (encounterZone != null)
            {
                encounterZone.NotifySlimeDefeated(this);
            }

            Destroy(gameObject);
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
                            var drop = go.GetComponent<WorldLootDrop>();
                            if (drop != null) drop.Initialize(entry.item, 1);
                        }
                    }
                    else
                    {
                        GameRoot.Instance?.Inventory?.AddItem(entry.item, amt, true);
                    }
                }
            }
        }
    }
}
