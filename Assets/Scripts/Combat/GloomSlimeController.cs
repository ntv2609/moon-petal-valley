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
        [SerializeField] private GameObject groundShadow;

        [Header("Pixel Art Animations (FullRect)")]
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] telegraphFrames;
        [SerializeField] private Sprite[] jumpFrames;
        [SerializeField] private Sprite[] hitFrames;
        [SerializeField] private Sprite[] dieFrames;
        [SerializeField] private float idleFps = 6f;
        [SerializeField] private float telegraphFps = 7f;
        [SerializeField] private float jumpFps = 10f;
        [SerializeField] private float hitFps = 11f;
        [SerializeField] private float dieFps = 8f;

        private SlimeState _state = SlimeState.Idle;
        private SlimeState _lastAnimState = SlimeState.Idle;
        private int _currentHP;
        private float _stateTimer;
        private float _flashTimer;
        private float _animTimer;
        private int _animFrame;
        private Vector2 _jumpVelocity;
        private Vector2 _knockbackVelocity;
        private PlayerController2D _player;
        private bool _isDefeated;
        private bool _swarmAggro;
        private float _swarmDetectRadius = 60f;

        public bool IsDefeated => _isDefeated;

        public void ForceSwarmAggro(PlayerController2D target, float overrideDetectRadius = 60f)
        {
            _swarmAggro = true;
            _swarmDetectRadius = Mathf.Max(20f, overrideDetectRadius);
            if (target != null) _player = target;
            if (!_isDefeated && _state == SlimeState.Idle && _player != null)
            {
                _state = SlimeState.Telegraph;
                _stateTimer = 0.25f;
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(1f, 0.90f, 0.65f, 1f);
                    spriteRenderer.flipX = _player.transform.position.x < transform.position.x;
                }
            }
        }

        private void Start()
        {
            _currentHP = enemyData != null ? enemyData.maxHP : 45;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            if (groundShadow == null)
            {
                Transform sh = transform.Find("GroundShadow");
                if (sh != null) groundShadow = sh.gameObject;
            }
            if (spriteRenderer != null && idleFrames != null && idleFrames.Length > 0 && idleFrames[0] != null)
            {
                spriteRenderer.sprite = idleFrames[0];
            }
            UpdateFootYSorting();
        }

        private void Update()
        {
            if (_state == SlimeState.Dying)
            {
                _stateTimer -= Time.deltaTime;
                UpdateAnimation(false);
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

            float detectR = _swarmAggro ? _swarmDetectRadius : (enemyData != null ? enemyData.detectionRadius : 4.5f);
            float moveSpd = enemyData != null ? enemyData.moveSpeed : 2.8f;
            float interval = enemyData != null ? enemyData.attackInterval : 1.1f;

            switch (_state)
            {
                case SlimeState.Idle:
                    if (_player != null && Vector2.Distance(transform.position, _player.transform.position) <= detectR)
                    {
                        _state = SlimeState.Telegraph;
                        _stateTimer = 0.45f;
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.color = new Color(1f, 0.90f, 0.65f, 1f);
                            spriteRenderer.flipX = _player.transform.position.x < transform.position.x;
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
                            spriteRenderer.color = Color.white;
                            if (Mathf.Abs(dir.x) > 0.05f) spriteRenderer.flipX = dir.x < 0f;
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

            UpdateAnimation(true);
            UpdateFootYSorting();
        }

        private void UpdateFootYSorting()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "WorldObjects";
                float footY = spriteRenderer.sprite != null ? spriteRenderer.bounds.min.y : (transform.position.y - 0.35f);
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-footY * 100f);
            }
        }

        private void UpdateAnimation(bool loop)
        {
            if (spriteRenderer == null) return;

            if (_state != _lastAnimState)
            {
                _lastAnimState = _state;
                _animFrame = 0;
                _animTimer = 0f;
            }

            Sprite[] frames = idleFrames;
            float fps = idleFps;
            bool shouldLoop = loop;

            switch (_state)
            {
                case SlimeState.Telegraph:
                    frames = (telegraphFrames != null && telegraphFrames.Length > 0) ? telegraphFrames : idleFrames;
                    fps = telegraphFps;
                    shouldLoop = false;
                    break;
                case SlimeState.Jumping:
                    frames = (jumpFrames != null && jumpFrames.Length > 0) ? jumpFrames : idleFrames;
                    fps = jumpFps;
                    shouldLoop = true;
                    break;
                case SlimeState.HitStun:
                    frames = (hitFrames != null && hitFrames.Length > 0) ? hitFrames : idleFrames;
                    fps = hitFps;
                    shouldLoop = false;
                    break;
                case SlimeState.Dying:
                    frames = (dieFrames != null && dieFrames.Length > 0) ? dieFrames : idleFrames;
                    fps = dieFps;
                    shouldLoop = false;
                    break;
            }

            if (frames == null || frames.Length == 0) return;

            _animTimer += Time.deltaTime;
            float step = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= step)
            {
                _animTimer -= step;
                if (shouldLoop) _animFrame = (_animFrame + 1) % frames.Length;
                else _animFrame = Mathf.Min(_animFrame + 1, frames.Length - 1);
            }

            int idx = Mathf.Clamp(_animFrame, 0, frames.Length - 1);
            if (frames[idx] != null) spriteRenderer.sprite = frames[idx];
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
            _stateTimer = 0.28f;
            _knockbackVelocity = knockbackDir.normalized * knockbackForce;

            _flashTimer = 0.16f;
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
            _state = SlimeState.Dying;
            _stateTimer = (dieFrames != null && dieFrames.Length > 0) ? (dieFrames.Length / Mathf.Max(1f, dieFps)) + 0.05f : 0.05f;

            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            if (groundShadow != null) groundShadow.SetActive(false);
            if (spriteRenderer != null) spriteRenderer.color = Color.white;

            var cols = GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < cols.Length; i++) cols[i].enabled = false;

            SpawnLoot();
            string id = enemyData != null ? enemyData.enemyID : "enemy_gloom_slime";
            GameEventBus.RaiseEnemyDefeated(id);
            encounterZone?.NotifySlimeDefeated(this);
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
