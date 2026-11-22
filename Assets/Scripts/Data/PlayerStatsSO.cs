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

        [Header("Chiến đấu (Liềm Ánh Trăng 120 độ)")]
        public int attackDamage = 15;
        public float attackArcDegrees = 120f;
        public float attackRadius = 1.35f;
        public float attackCooldown = 0.4f;
        public float attackKnockbackForce = 6.0f;

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
    }
}
