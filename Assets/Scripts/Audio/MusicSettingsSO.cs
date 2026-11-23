using UnityEngine;

namespace MoonpetalValley.Audio
{
    [CreateAssetMenu(fileName = "MusicSettings", menuName = "Moonpetal/Audio/Music Settings")]
    public class MusicSettingsSO : ScriptableObject
    {
        public bool systemEnabled = true;
        [Range(0f, 1.5f)] public float masterVolume = 1.0f;
        public bool enableDebugOverlay = true;
        public bool verboseLogging = false;
    }
}
