using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "RelayLanternConfig", menuName = "Moonpetal/Config/RelayLanternConfigSO")]
    public class RelayLanternConfigSO : ScriptableObject
    {
        [Header("Cột đèn trên cầu")]
        public int lanternCount = 6;
        public float passiveDrainPerSec = 2.5f;
        public float mothDrainPerSec = 12.0f;
        public float relightRadius = 2.1f;

        [Header("Ánh Sáng Công Trường & Điều kiện thắng/thua mềm")]
        public int minLitLanternsForWork = 4;
        public float siteLightDrainIfDark = 7.5f;
        public float siteLightRecoverPerSec = 4.0f;
        public bool freeDashDuringRelay = true;

        [Header("Bướm Hút Sáng")]
        public float mothSpawnInterval = 5.5f;
        public int maxActiveMoths = 4;
    }
}
