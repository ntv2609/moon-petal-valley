using UnityEngine;

namespace MoonpetalValley.Combat
{
    public class StaminaOrbArtRegistrar : MonoBehaviour
    {
        [SerializeField] private Sprite staminaOrbSprite;
        [SerializeField] private Sprite staminaSurgeSprite;

        private void Awake()
        {
            InstantRecoveryOrb.SetCustomOrbSprites(staminaOrbSprite, staminaSurgeSprite);
        }
    }
}
