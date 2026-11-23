using UnityEngine;

namespace MoonpetalValley.Audio
{
    public enum MusicContext
    {
        None = 0, Title = 1, Cutscene_Opening = 2, Safe_Day = 3, Safe_Night = 4,
        Mist_Zone = 5, Combat = 6, Emotional = 7, Zone_River_Day = 8, Set_LanternRelay = 9,
        Set_NightMarket = 10, Zone_NorthForest = 11, Flashback_Memory = 12,
        Boss_MistWarden = 13, Scene_TheChair = 14, Ending = 15
    }

    public enum StingerType { None = 0, PetalObtained = 1, QuestComplete = 2, KnockedOut = 3, NewDay = 4 }
    public enum TrackPlayMode { Once = 0, Loop = 1, CrossfadeSelf = 2 }

    [CreateAssetMenu(fileName = "MusicTrack", menuName = "Moonpetal/Audio/Music Track")]
    public class MusicTrackSO : ScriptableObject
    {
        public string trackId = "";
        public bool isStinger = false;
        public MusicContext context = MusicContext.None;
        public StingerType stingerType = StingerType.None;
        public AudioClip clip;
        public int priority = 20;
        public TrackPlayMode playMode = TrackPlayMode.CrossfadeSelf;
        [Range(0f, 1.5f)] public float baseVolume = 0.85f;
        [Min(0f)] public float fadeInSeconds = 1.5f;
        [Min(0f)] public float fadeOutSeconds = 1.5f;
        [Min(0f)] public float selfCrossfadeSeconds = 3.0f;
        [Min(0f)] public float ambientRestMinSeconds = 0f;
        [Min(0f)] public float ambientRestMaxSeconds = 0f;
        [Min(0f)] public float maxPlaySeconds = 0f;
        public bool isDormant = false;
        [TextArea(1, 3)] public string notes = "";
    }
}
