using UnityEngine;

namespace MoonpetalValley.Combat
{
    public interface IDamageable
    {
        bool IsDefeated { get; }
        void TakePurifyDamage(int amount, Vector2 knockbackDir, float knockbackForce);
    }
}
