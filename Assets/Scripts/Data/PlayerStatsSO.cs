using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "PlayerStatsConfig", menuName = "Moonpetal/Config/PlayerStatsSO")]
    public class PlayerStatsSO : ScriptableObject
    {
        [Header("Thể lực & Máu Ánh Sáng")]
        public int maxStamina = 100;
        public int maxLightHP = 100;
        [Range(0.1f, 1f)] public float exhaustedStaminaMultiplier = 0.5f;
        [Range(0f, 0.5f)] public float rawMaterialDropPercentOnPassOut = 0.1f;

        [Header("Di chuyển & Làm chậm")]
        public float moveSpeed = 4.5f;
        [Range(0.1f, 1f)] public float slowedSpeedMultiplier = 0.55f;
        public float slowDuration = 2.0f;

        [Header("Chiến đấu (Liềm Ánh Trăng gốc)")]
        public int attackDamage = 15;
        public float attackArcDegrees = 120f;
        public float attackRadius = 1.35f;
        public float attackCooldown = 0.4f;
        public float attackKnockbackForce = 6.0f;

        [Header("Chuỗi Combo Liềm Ánh Trăng (Step C2)")]
        public bool enableComboSystem = true;
        public bool enableDashStrike = true;
        public float comboWindowDuration = 0.62f;
        public float inputBufferDuration = 0.18f;
        public float dashStrikeGraceWindow = 0.12f;

        [Header("Đòn 1: Trăng Lưỡi Liềm")]
        public int combo1Damage = 15;
        public float combo1Radius = 1.60f;
        public float combo1ArcDegrees = 120f;
        public float combo1Knockback = 4.5f;
        public float combo1Cooldown = 0.24f;
        public int combo1StaminaCost = 2;
        public float combo1LungeSpeed = 4.2f;
        public float combo1LungeDuration = 0.08f;

        [Header("Đòn 2: Nguyệt Ngược")]
        public int combo2Damage = 19;
        public float combo2Radius = 1.75f;
        public float combo2ArcDegrees = 145f;
        public float combo2Knockback = 5.5f;
        public float combo2Cooldown = 0.26f;
        public int combo2StaminaCost = 2;
        public float combo2LungeSpeed = 5.6f;
        public float combo2LungeDuration = 0.09f;

        [Header("Đòn 3: Hoa Trăng Xoay 360")]
        public int combo3Damage = 28;
        public float combo3Radius = 2.15f;
        public float combo3ArcDegrees = 360f;
        public float combo3Knockback = 9.5f;
        public float combo3Cooldown = 0.44f;
        public int combo3StaminaCost = 3;
        public float combo3LungeSpeed = 6.2f;
        public float combo3LungeDuration = 0.11f;

        [Header("Lướt Chém (Dash-Strike)")]
        public int dashStrikeDamage = 22;
        public float dashStrikeRadius = 1.90f;
        public float dashStrikeArcDegrees = 110f;
        public float dashStrikeKnockback = 7.0f;
        public float dashStrikeCooldown = 0.26f;
        public int dashStrikeStaminaCost = 2;
        public float dashStrikeLungeSpeed = 8.5f;
        public float dashStrikeLungeDuration = 0.11f;

        [Header("Lướt né (Dash) & Miễn nhiễm sát thương")]
        public float dashSpeed = 11.5f;
        public float dashDuration = 0.18f;
        public float dashCooldown = 0.8f;
        public float dashInvulnerabilityDuration = 0.20f;
        public float hitInvulnerabilityDuration = 0.75f;

        [Header("Tiêu hao Stamina theo hành động")]
        public int hoeStaminaCost = 4;
        public int waterStaminaCost = 2;
        public int chopStaminaCost = 4;
        public int swingStaminaCost = 2;
        public int dashStaminaCost = 5;
        public int lumiPulseStaminaCost = 10;

        [Header("Tinh linh Lumi")]
        public float lumiBaseLightRadius = 2.5f;
        public float lumiUpgradedLightRadius = 5.5f;
        public float lumiPulseRadius = 3.5f;
        public float lumiPulseCooldown = 8.0f;

        [Header("Chương 2: Xung Ánh Trăng Cấp 2 (Cộng Hưởng)")]
        public float lumiPulseLv2Radius = 4.5f;
        public float lumiPulseLv2StunDuration = 2.0f;
    }
}
