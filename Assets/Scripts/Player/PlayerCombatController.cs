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

        private float _attackCooldownTimer;
        private float _slashVisualTimer;
        private float _pulseCooldownTimer;
        private float _pulseVisualTimer;

        private void Start()
        {
            if (slashArcRenderer != null) slashArcRenderer.enabled = false;
            if (pulseRingRenderer != null) pulseRingRenderer.enabled = false;
        }

        private void Update()
        {
            if (_attackCooldownTimer > 0f) _attackCooldownTimer -= Time.deltaTime;
            if (_pulseCooldownTimer > 0f) _pulseCooldownTimer -= Time.deltaTime;

            UpdateVisualEffects();

            if (GameRoot.Instance == null || GameRoot.Instance.State == null || !GameRoot.Instance.State.IsGameplayActive)
            {
                return;
            }
            if (inputReader == null || playerController == null) return;

            // 1. Chém Liềm Ánh Trăng 120 độ
            if (inputReader.ActionPressed && _attackCooldownTimer <= 0f)
            {
                bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                if (!overUI && IsHoldingMoonSickle())
                {
                    PerformMoonSickleSlash();
                }
            }

            // 2. Xung Ánh Trăng của Lumi (phím Q)
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

        private void PerformMoonSickleSlash()
        {
            int dmg = statsConfig != null ? statsConfig.attackDamage : 15;
            float radius = statsConfig != null ? Mathf.Max(1.55f, statsConfig.attackRadius) : 1.55f;
            float arcDeg = statsConfig != null ? statsConfig.attackArcDegrees : 120f;
            float kb = statsConfig != null ? statsConfig.attackKnockbackForce : 6f;
            int cost = statsConfig != null ? statsConfig.swingStaminaCost : 2;
            _attackCooldownTimer = statsConfig != null ? statsConfig.attackCooldown : 0.4f;

            GameRoot.Instance?.Stats?.ConsumeStamina(cost);

            Vector2 facing = playerController.FacingDirection.sqrMagnitude > 0.01f
                ? playerController.FacingDirection.normalized
                : Vector2.down;

            if (spriteAnimator != null && spriteAnimator.HasValidSprites)
            {
                spriteAnimator.PlayMoonSickleSlash(facing);
            }

            if (slashArcTransform != null && slashArcRenderer != null)
            {
                float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
                slashArcTransform.localPosition = facing * 0.65f;
                slashArcTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
                slashArcRenderer.enabled = true;
                slashArcRenderer.color = new Color(0.68f, 0.98f, 1f, 0.95f);
                _slashVisualTimer = 0.16f;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
            for (int i = 0; i < hits.Length; i++)
            {
                var target = hits[i].GetComponent<IDamageable>();
                if (target == null || target.IsDefeated) continue;

                Vector2 toTarget = (Vector2)hits[i].transform.position - (Vector2)transform.position;
                float angleDiff = Vector2.Angle(facing, toTarget);
                if (toTarget.magnitude <= 0.6f || angleDiff <= arcDeg * 0.55f)
                {
                    target.TakePurifyDamage(dmg, toTarget.normalized, kb);
                }
            }
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

            GameRoot.Instance?.Stats?.ConsumeStamina(cost);
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
                    c.a = Mathf.Clamp01(_slashVisualTimer / 0.16f);
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
