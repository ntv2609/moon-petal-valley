using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "TimeConfig", menuName = "Moonpetal/Config/TimeConfigSO")]
    public class TimeConfigSO : ScriptableObject
    {
        [Header("Mốc giờ trong ngày (06:00 -> 02:00 hôm sau = 26)")]
        public int dayStartHour = 6;
        public int duskStartHour = 18;
        public int nightStartHour = 20;
        public int passOutHour = 26;

        [Header("Tốc độ thời gian")]
        public float realSecondsPerGameDay = 300f;

        [Header("Màu ánh sáng môi trường")]
        public Color morningAmbientColor = new Color(1.0f, 0.96f, 0.88f, 1f);
        public Color noonAmbientColor = new Color(1.0f, 1.0f, 1.0f, 1f);
        public Color duskAmbientColor = new Color(0.95f, 0.65f, 0.48f, 1f);
        public Color nightAmbientColor = new Color(0.24f, 0.28f, 0.48f, 1f);

        [Header("Cường độ ánh sáng")]
        [Range(0f, 1.2f)] public float morningIntensity = 0.95f;
        [Range(0f, 1.2f)] public float noonIntensity = 1.0f;
        [Range(0f, 1.2f)] public float duskIntensity = 0.65f;
        [Range(0f, 1.2f)] public float nightIntensity = 0.28f;
    }
}
