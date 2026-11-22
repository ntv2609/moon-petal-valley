using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    public enum EnemyAttackPattern
    {
        Jump,
        Burrow,
        BossBeam
    }

    [Serializable]
    public struct LootDropEntry
    {
        public ItemData item;
        public int minAmount;
        public int maxAmount;
        [Range(0f, 1f)] public float dropChance;
    }

    [CreateAssetMenu(fileName = "Enemy_New", menuName = "Moonpetal/Data/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public string enemyID;
        public string enemyNameKey;
        public int maxHP = 45;
        public float moveSpeed = 2.8f;
        public int contactDamage = 10;
        public EnemyAttackPattern attackPattern = EnemyAttackPattern.Jump;
        public float detectionRadius = 4.5f;
        public float attackInterval = 1.1f;
        public bool appliesSlowOnHit = true;
        public List<LootDropEntry> dropTable = new List<LootDropEntry>();
    }
}
