using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Audio
{
    [CreateAssetMenu(fileName = "MusicRules", menuName = "Moonpetal/Audio/Music Rules")]
    public class MusicRulesSO : ScriptableObject
    {
        [Header("Polling & Detection")]
        [Min(0.25f)] public float pollIntervalSeconds = 0.25f;
        public LayerMask enemyLayerMask = 1 << 9;
        [Min(0.5f)] public float combatDetectRadius = 6.5f;
        [Min(0f)] public float combatHoldSeconds = 6.0f;
        [Min(0f)] public float mistDetectPadding = 0.5f;
        [Min(0.5f)] public float mistFallbackRadius = 4.5f;
        public bool useDuskAsNight = true;

        [Header("Crossfade & Ducking")]
        [Min(0.1f)] public float defaultCrossfadeSeconds = 1.6f;
        [Range(0f, 1f)] public float dialogueDuckFactor = 0.45f;
        [Range(0f, 1f)] public float stingerDuckFactor = 0.30f;
        [Min(0.05f)] public float duckTransitionSeconds = 0.35f;
        [Min(0f)] public float saveLoadSuppressSeconds = 2.5f;
        [Min(0f)] public float stingerCooldownSeconds = 1.5f;

        [Header("Scene Routing")]
        public List<string> titleSceneNames = new List<string> { "MainMenu", "Bootstrap" };
        public List<string> villageSceneNames = new List<string> { "MoonpetalValley_MVP" };
        public List<string> riverSceneNames = new List<string> { "Riverbank_Chapter2", "WestSwamp_Chapter2" };

        [Header("Stinger Signal Rules")]
        public List<string> petalFlagIds = new List<string> { "flag_nv1_completed", "flag_ch2_completed" };
        public List<string> petalItemKeywords = new List<string> { "petal" };
        public List<string> questCompleteFlagIds = new List<string>
        {
            "flag_greenhouse_cleared", "flag_tutorial_done", "flag_garden_slimes_cleared",
            "flag_hana_bread_done", "flag_toby_cat_saved", "flag_otis_mailbox_fixed",
            "flag_bach_letters_read", "flag_ch2_inv_night1_done", "flag_ch2_inv_night2_done",
            "flag_ch2_inv_night3_done", "flag_ch2_swamp_unlocked", "flag_ch2_swamp_cleared",
            "flag_ch2_echo1_lantern_done", "flag_ch2_echo2_pier_done"
        };
    }
}
