using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Player
{
    public class PlayerController2D : MonoBehaviour, ISaveable
    {
        [SerializeField] private PlayerStatsSO statsConfig;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform visualTransform;

        private Vector2 _facingDirection = Vector2.down;
        private Vector2 _dashDirection;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private float _invulnerableTimer;
        private float _slowTimer;
        private float _walkBobPhase;

        public Vector2 FacingDirection => _facingDirection;
        public bool IsDashing => _dashTimer > 0f;
        public bool IsInvulnerable => _invulnerableTimer > 0f;
        public string SaveKey => "player_position";

        [Serializable]
        private class PlayerPosSave
        {
            public float x;
            public float y;
        }

        private void Start()
        {
            if (GameRoot.Instance != null)
            {
                GameRoot.Instance.RegisterSaveable(this);
            }
        }

        private void Update()
        {
            if (_dashCooldownTimer > 0f) _dashCooldownTimer -= Time.deltaTime;
            if (_invulnerableTimer > 0f) _invulnerableTimer -= Time.deltaTime;
            if (_slowTimer > 0f) _slowTimer -= Time.deltaTime;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                UpdateVisualFeedback(Vector2.zero);
                return;
            }

            Vector2 move = inputReader != null ? inputReader.MoveInput : Vector2.zero;
            if (move.sqrMagnitude > 0.01f && !IsDashing)
            {
                _facingDirection = move.normalized;
            }

            if (inputReader != null && inputReader.DashPressed && CanDash())
            {
                StartDash(move.sqrMagnitude > 0.01f ? move.normalized : _facingDirection);
            }

            if (_dashTimer > 0f)
            {
                _dashTimer -= Time.deltaTime;
            }

            UpdateVisualFeedback(move);
            UpdateYSorting();
        }

        private void FixedUpdate()
        {
            if (rb == null) return;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            float baseSpeed = statsConfig != null ? statsConfig.moveSpeed : 4.5f;
            float dashSpeed = statsConfig != null ? statsConfig.dashSpeed : 11.5f;
            float slowMult = statsConfig != null ? statsConfig.slowedSpeedMultiplier : 0.55f;

            if (_dashTimer > 0f)
            {
                rb.linearVelocity = _dashDirection * dashSpeed;
            }
            else
            {
                Vector2 move = inputReader != null ? inputReader.MoveInput : Vector2.zero;
                float speed = _slowTimer > 0f ? baseSpeed * slowMult : baseSpeed;
                rb.linearVelocity = move * speed;
            }
        }

        private bool CanDash()
        {
            return _dashTimer <= 0f && _dashCooldownTimer <= 0f;
        }

        private void StartDash(Vector2 direction)
        {
            int dashCost = statsConfig != null ? statsConfig.dashStaminaCost : 5;
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                bool stillAwake = GameRoot.Instance.Stats.ConsumeStamina(dashCost);
                if (!stillAwake) return;
            }

            _dashDirection = direction.normalized;
            _dashTimer = statsConfig != null ? statsConfig.dashDuration : 0.18f;
            _dashCooldownTimer = statsConfig != null ? statsConfig.dashCooldown : 0.8f;
            float iFrame = statsConfig != null ? statsConfig.dashInvulnerabilityDuration : 0.2f;
            if (iFrame > _invulnerableTimer) _invulnerableTimer = iFrame;
        }

        public void ApplySlow(float durationOverride = -1f)
        {
            float dur = durationOverride > 0f ? durationOverride : (statsConfig != null ? statsConfig.slowDuration : 2f);
            _slowTimer = Mathf.Max(_slowTimer, dur);
        }

        public void ClearSlow()
        {
            _slowTimer = 0f;
        }

        public void GrantInvulnerability(float duration)
        {
            _invulnerableTimer = Mathf.Max(_invulnerableTimer, duration);
        }

        public void TeleportTo(Vector2 worldPos)
        {
            _dashTimer = 0f;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = worldPos;
            }
            transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
            UpdateYSorting();
        }

        private void UpdateVisualFeedback(Vector2 move)
        {
            if (spriteRenderer != null)
            {
                if (Mathf.Abs(_facingDirection.x) > 0.1f)
                {
                    spriteRenderer.flipX = _facingDirection.x < 0f;
                }

                if (IsDashing)
                {
                    spriteRenderer.color = new Color(0.70f, 0.95f, 1.0f, 0.88f);
                }
                else if (_slowTimer > 0f)
                {
                    spriteRenderer.color = new Color(0.75f, 0.65f, 0.95f, 1.0f);
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }
            }

            if (visualTransform != null)
            {
                if (move.sqrMagnitude > 0.01f && !IsDashing)
                {
                    _walkBobPhase += Time.deltaTime * 14f;
                    float bobY = Mathf.Abs(Mathf.Sin(_walkBobPhase)) * 0.08f;
                    visualTransform.localPosition = new Vector3(0f, bobY, 0f);
                }
                else
                {
                    _walkBobPhase = 0f;
                    visualTransform.localPosition = Vector3.Lerp(visualTransform.localPosition, Vector3.zero, Time.deltaTime * 12f);
                }
            }
        }

        private void UpdateYSorting()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
            }
        }

        public string CaptureStateJson()
        {
            var data = new PlayerPosSave { x = transform.position.x, y = transform.position.y };
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<PlayerPosSave>(json);
            if (data != null)
            {
                TeleportTo(new Vector2(data.x, data.y));
            }
        }
    }
}
