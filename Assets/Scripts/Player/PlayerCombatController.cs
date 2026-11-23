using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;

namespace MoonpetalValley.Player
{
    public class PlayerCombatController : MonoBehaviour
    {
        [SerializeField] private PlayerStatsSO statsConfig;
        [SerializeField] private PlayerController2D playerController;
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private WenSpriteAnimator spriteAnimator;
        [SerializeField] private Transform slashArcTransform;
        [SerializeField] private SpriteRenderer slashArcRenderer;
        [SerializeField] private Transform pulseRingTransform;
        [SerializeField] private SpriteRenderer pulseRingRenderer;

        /// <summary>(comboStep 1..3, isDashStrike, playerPos, facingDir)</summary>
        public static event Action<int, bool, Vector2, Vector2> OnPlayerComboSwing;

        /// <summary>(comboStep 1..3, isDashStrike, hitWorldPos, damageDealt)</summary>
        public static event Action<int, bool, Vector2, int> OnPlayerComboHit;

        /// <summary>(currentComboStep 0..3, isFinisher)</summary>
        public static event Action<int, bool> OnComboChainChanged;

        private float _attackCooldownTimer;
        private float _slashVisualTimer;
        private float _slashVisualDuration = 0.16f;
        private float _pulseCooldownTimer;
        private float _pulseVisualTimer;

        private int _comboStep;
        private float _comboWindowTimer;
        private float _attackBufferTimer;
        private int _lastDashStrikeSeqId = -1;

        private readonly HashSet<IDamageable> _hitBuffer = new HashSet<IDamageable>();

        public int CurrentComboStep => _comboStep;

        private void Start()
        {
            if (slashArcRenderer != null) slashArcRenderer.enabled = false;
            if (pulseRingRenderer != null) pulseRingRenderer.enabled = false;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            if (_attackCooldownTimer > 0f) _attackCooldownTimer -= dt;
            if (_pulseCooldownTimer > 0f) _pulseCooldownTimer -= dt;
            if (_attackBufferTimer > 0f) _attackBufferTimer -= dt;

            if (_comboWindowTimer > 0f)
            {
                _comboWindowTimer -= dt;
                if (_comboWindowTimer <= 0f && _comboStep != 0)
                {
                    _comboStep = 0;
                    OnComboChainChanged?.Invoke(0, false);
                }
            }

            UpdateVisualEffects();

            if (GameRoot.Instance == null || GameRoot.Instance.State == null || !GameRoot.Instance.State.IsGameplayActive)
            {
                return;
            }
            if (inputReader == null || playerController == null) return;

            bool comboEnabled = statsConfig == null || statsConfig.enableComboSystem;

            // 1. Ghi nhận input đánh Liềm Ánh Trăng (có Input Buffer khi bật Combo)
            if (inputReader.ActionPressed)
            {
                bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                if (!overUI && IsHoldingMoonSickle())
                {
                    if (!comboEnabled)
                    {
                        if (_attackCooldownTimer <= 0f)
                        {
                            PerformLegacySingleSlash();
                        }
                    }
                    else
                    {
                        float buf = statsConfig != null ? statsConfig.inputBufferDuration : 0.18f;
                        _attackBufferTimer = Mathf.Max(0.05f, buf);
                    }
                }
            }

            // 2. Thực thi đòn từ bộ đệm khi hết cooldown
            if (comboEnabled && _attackBufferTimer > 0f && _attackCooldownTimer <= 0f)
            {
                if (IsHoldingMoonSickle())
                {
                    _attackBufferTimer = 0f;
                    ExecuteNextComboAction();
                }
                else
                {
                    _attackBufferTimer = 0f;
                }
            }

            // 3. Xung Ánh Trăng của Lumi (phím Q)
            if (inputReader.LumiSkillPressed)
            {
                TryPerformLumiMoonPulse();
            }
        }

        private bool IsHoldingMoonSickle()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return false;
            var slot = GameRoot.Instance.Inventory.GetSelectedSlot();
            return slot != null && !slot.IsEmpty && slot.item.itemID == "item_moon_sickle";
        }

        private void ExecuteNextComboAction()
        {
            bool allowDashStrike = statsConfig == null || statsConfig.enableDashStrike;
            float grace = statsConfig != null ? statsConfig.dashStrikeGraceWindow : 0.12f;

            bool canDashStrike = allowDashStrike
                                 && playerController.IsInDashStrikeWindow(grace)
                                 && playerController.DashSequenceId != _lastDashStrikeSeqId;

            if (canDashStrike)
            {
                _lastDashStrikeSeqId = playerController.DashSequenceId;
                PerformDashStrike();
                return;
            }

            int nextStep = (_comboWindowTimer > 0f && _comboStep >= 1 && _comboStep < 3) ? (_comboStep + 1) : 1;
            PerformComboHit(nextStep);
        }

        private void PerformDashStrike()
        {
            int dmg = statsConfig != null ? statsConfig.dashStrikeDamage : 22;
            float radius = statsConfig != null ? statsConfig.dashStrikeRadius : 1.90f;
            float arcDeg = statsConfig != null ? statsConfig.dashStrikeArcDegrees : 110f;
            float kb = statsConfig != null ? statsConfig.dashStrikeKnockback : 7.0f;
            int cost = statsConfig != null ? statsConfig.dashStrikeStaminaCost : 2;
            float cooldown = statsConfig != null ? statsConfig.dashStrikeCooldown : 0.26f;
            float lungeSpeed = statsConfig != null ? statsConfig.dashStrikeLungeSpeed : 8.5f;
            float lungeDur = statsConfig != null ? statsConfig.dashStrikeLungeDuration : 0.11f;
            float window = statsConfig != null ? statsConfig.comboWindowDuration : 0.62f;

            _attackCooldownTimer = cooldown;
            _comboStep = 1; // Mở chuỗi mượt: Dash-Strike -> Đòn 2 -> Đòn 3
            _comboWindowTimer = window;

            Vector2 facing = ResolveAttackFacing(true);
            playerController.SetFacingDirection(facing);
            playerController.TriggerCombatLunge(facing, lungeSpeed, lungeDur, true);

            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                GameRoot.Instance.Stats.ConsumeStamina(cost);
            }

            if (spriteAnimator != null && spriteAnimator.HasValidSprites)
            {
                spriteAnimator.PlayDashStrike(facing);
            }

            ShowFallbackSlashVisual(facing, 1.25f, new Color(0.75f, 1f, 0.96f, 0.95f), 0.17f);
            OnPlayerComboSwing?.Invoke(1, true, transform.position, facing);
            OnComboChainChanged?.Invoke(1, false);

            ApplyMeleeHitbox(facing, radius, arcDeg, dmg, kb, 1, true);
        }

        private void PerformComboHit(int step)
        {
            int dmg;
            float radius;
            float arcDeg;
            float kb;
            int cost;
            float cooldown;
            float lungeSpeed;
            float lungeDur;
            float visualScale;
            Color visualColor;

            if (step == 2)
            {
                dmg = statsConfig != null ? statsConfig.combo2Damage : 19;
                radius = statsConfig != null ? statsConfig.combo2Radius : 1.75f;
                arcDeg = statsConfig != null ? statsConfig.combo2ArcDegrees : 145f;
                kb = statsConfig != null ? statsConfig.combo2Knockback : 5.5f;
                cost = statsConfig != null ? statsConfig.combo2StaminaCost : 2;
                cooldown = statsConfig != null ? statsConfig.combo2Cooldown : 0.26f;
                lungeSpeed = statsConfig != null ? statsConfig.combo2LungeSpeed : 5.6f;
                lungeDur = statsConfig != null ? statsConfig.combo2LungeDuration : 0.09f;
                visualScale = 1.20f;
                visualColor = new Color(0.55f, 1f, 0.88f, 0.96f);
            }
            else if (step == 3)
            {
                dmg = statsConfig != null ? statsConfig.combo3Damage : 28;
                radius = statsConfig != null ? statsConfig.combo3Radius : 2.15f;
                arcDeg = statsConfig != null ? statsConfig.combo3ArcDegrees : 360f;
                kb = statsConfig != null ? statsConfig.combo3Knockback : 9.5f;
                cost = statsConfig != null ? statsConfig.combo3StaminaCost : 3;
                cooldown = statsConfig != null ? statsConfig.combo3Cooldown : 0.44f;
                lungeSpeed = statsConfig != null ? statsConfig.combo3LungeSpeed : 6.2f;
                lungeDur = statsConfig != null ? statsConfig.combo3LungeDuration : 0.11f;
                visualScale = 1.55f;
                visualColor = new Color(1f, 0.92f, 0.58f, 0.98f);
            }
            else
            {
                step = 1;
                dmg = statsConfig != null ? statsConfig.combo1Damage : 15;
                radius = statsConfig != null ? statsConfig.combo1Radius : 1.60f;
                arcDeg = statsConfig != null ? statsConfig.combo1ArcDegrees : 120f;
                kb = statsConfig != null ? statsConfig.combo1Knockback : 4.5f;
                cost = statsConfig != null ? statsConfig.combo1StaminaCost : 2;
                cooldown = statsConfig != null ? statsConfig.combo1Cooldown : 0.24f;
                lungeSpeed = statsConfig != null ? statsConfig.combo1LungeSpeed : 4.2f;
                lungeDur = statsConfig != null ? statsConfig.combo1LungeDuration : 0.08f;
                visualScale = 1.0f;
                visualColor = new Color(0.68f, 0.98f, 1f, 0.95f);
            }

            float window = statsConfig != null ? statsConfig.comboWindowDuration : 0.62f;
            bool isFinisher = (step == 3);

            _attackCooldownTimer = cooldown;
            _comboStep = isFinisher ? 0 : step;
            _comboWindowTimer = isFinisher ? 0f : window;

            Vector2 facing = ResolveAttackFacing(false);
            playerController.SetFacingDirection(facing);
            playerController.TriggerCombatLunge(facing, lungeSpeed, lungeDur, false);

            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                GameRoot.Instance.Stats.ConsumeStamina(cost);
            }

            if (spriteAnimator != null && spriteAnimator.HasValidSprites)
            {
                spriteAnimator.PlayComboSlash(step, facing);
            }

            ShowFallbackSlashVisual(facing, visualScale, visualColor, isFinisher ? 0.22f : 0.16f);
            OnPlayerComboSwing?.Invoke(step, false, transform.position, facing);
            OnComboChainChanged?.Invoke(step, isFinisher);

            ApplyMeleeHitbox(facing, radius, arcDeg, dmg, kb, step, false);
        }

        private Vector2 ResolveAttackFacing(bool preferDashDir)
        {
            if (inputReader != null && inputReader.MoveInput.sqrMagnitude > 0.04f)
            {
                return inputReader.MoveInput.normalized;
            }
            if (preferDashDir && playerController != null && playerController.DashDirection.sqrMagnitude > 0.01f)
            {
                return playerController.DashDirection.normalized;
            }
            if (playerController != null && playerController.FacingDirection.sqrMagnitude > 0.01f)
            {
                return playerController.FacingDirection.normalized;
            }
            return Vector2.down;
        }

        private void ApplyMeleeHitbox(Vector2 facing, float radius, float arcDeg, int dmg, float kb, int comboStep, bool isDashStrike)
        {
            _hitBuffer.Clear();
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
            bool fullCircle = arcDeg >= 359f;

            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable target = hits[i].GetComponent<IDamageable>();
                if (target == null || target.IsDefeated || _hitBuffer.Contains(target)) continue;

                Vector2 targetPos = hits[i].transform.position;
                Vector2 toTarget = targetPos - (Vector2)transform.position;
                float angleDiff = Vector2.Angle(facing, toTarget);

                if (fullCircle || toTarget.magnitude <= 0.65f || angleDiff <= arcDeg * 0.55f)
                {
                    _hitBuffer.Add(target);
                    Vector2 pushDir = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : facing;
                    target.TakePurifyDamage(dmg, pushDir, kb);
                    OnPlayerComboHit?.Invoke(comboStep, isDashStrike, targetPos, dmg);
                }
            }
        }

        private void ShowFallbackSlashVisual(Vector2 facing, float scale, Color tint, float duration)
        {
            if (slashArcTransform == null || slashArcRenderer == null) return;

            // Khi Step C3 gắn ComboSlashVfx, slashArcRenderer gốc có thể được ẩn tự động nếu muốn;
            // hiện tại giữ hiển thị sạch để Step C2 chạy độc lập vẫn rõ nét.
            float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
            slashArcTransform.localPosition = facing * (scale > 1.4f ? 0.15f : 0.65f);
            slashArcTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            slashArcTransform.localScale = new Vector3(scale, scale, 1f);
            slashArcRenderer.enabled = true;
            slashArcRenderer.color = tint;
            _slashVisualDuration = Mathf.Max(0.05f, duration);
            _slashVisualTimer = _slashVisualDuration;
        }

        private void PerformLegacySingleSlash()
        {
            int dmg = statsConfig != null ? statsConfig.attackDamage : 15;
            float radius = statsConfig != null ? Mathf.Max(1.55f, statsConfig.attackRadius) : 1.55f;
            float arcDeg = statsConfig != null ? statsConfig.attackArcDegrees : 120f;
            float kb = statsConfig != null ? statsConfig.attackKnockbackForce : 6f;
            int cost = statsConfig != null ? statsConfig.swingStaminaCost : 2;
            _attackCooldownTimer = statsConfig != null ? statsConfig.attackCooldown : 0.4f;

            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                GameRoot.Instance.Stats.ConsumeStamina(cost);
            }

            Vector2 facing = playerController.FacingDirection.sqrMagnitude > 0.01f
                ? playerController.FacingDirection.normalized
                : Vector2.down;

            if (spriteAnimator != null && spriteAnimator.HasValidSprites)
            {
                spriteAnimator.PlayMoonSickleSlash(facing);
            }

            ShowFallbackSlashVisual(facing, 1f, new Color(0.68f, 0.98f, 1f, 0.95f), 0.16f);
            ApplyMeleeHitbox(facing, radius, arcDeg, dmg, kb, 1, false);
        }

        private void TryPerformLumiMoonPulse()
        {
            bool unlocked = GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            if (!unlocked)
            {
                GameEventBus.RaiseToastRequested("Kỹ năng Xung Ánh Trăng [Q] sẽ mở khóa sau khi nhận Cánh Hoa Hy Vọng!");
                return;
            }
            if (_pulseCooldownTimer > 0f)
            {
                GameEventBus.RaiseToastRequested("Xung Ánh Trăng đang hồi chiêu (" + Mathf.CeilToInt(_pulseCooldownTimer) + "s)...");
                return;
            }

            float radius = statsConfig != null ? statsConfig.lumiPulseRadius : 3.5f;
            int cost = statsConfig != null ? statsConfig.lumiPulseStaminaCost : 10;
            _pulseCooldownTimer = statsConfig != null ? statsConfig.lumiPulseCooldown : 8f;

            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                GameRoot.Instance.Stats.ConsumeStamina(cost);
            }
            playerController.ClearSlow();

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null) lumi.PlayMoonPulseAnimation();

            if (pulseRingTransform != null && pulseRingRenderer != null)
            {
                pulseRingRenderer.enabled = true;
                pulseRingTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
                pulseRingRenderer.color = new Color(0.60f, 0.98f, 0.95f, 0.9f);
                _pulseVisualTimer = 0.35f;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
            int pushed = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                var target = hits[i].GetComponent<IDamageable>();
                if (target != null && !target.IsDefeated)
                {
                    Vector2 dir = ((Vector2)hits[i].transform.position - (Vector2)transform.position).normalized;
                    target.TakePurifyDamage(10, dir, 9.5f);
                    pushed++;
                }
            }

            GameEventBus.RaiseToastRequested("Lumi phóng Xung Ánh Trăng! (Giải làm chậm & đẩy lùi " + pushed + " quái)");
        }

        private void UpdateVisualEffects()
        {
            if (_slashVisualTimer > 0f)
            {
                _slashVisualTimer -= Time.deltaTime;
                if (slashArcRenderer != null)
                {
                    Color c = slashArcRenderer.color;
                    c.a = Mathf.Clamp01(_slashVisualTimer / Mathf.Max(0.01f, _slashVisualDuration));
                    slashArcRenderer.color = c;
                    if (_slashVisualTimer <= 0f) slashArcRenderer.enabled = false;
                }
            }

            if (_pulseVisualTimer > 0f)
            {
                _pulseVisualTimer -= Time.deltaTime;
                float t = 1f - Mathf.Clamp01(_pulseVisualTimer / 0.35f);
                if (pulseRingTransform != null)
                {
                    float s = Mathf.Lerp(0.6f, 3.6f, t);
                    pulseRingTransform.localScale = new Vector3(s, s, 1f);
                }
                if (pulseRingRenderer != null)
                {
                    Color c = pulseRingRenderer.color;
                    c.a = 1f - t;
                    pulseRingRenderer.color = c;
                    if (_pulseVisualTimer <= 0f) pulseRingRenderer.enabled = false;
                }
            }
        }
    }
}
