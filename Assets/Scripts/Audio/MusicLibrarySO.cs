using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Audio
{
    [CreateAssetMenu(fileName = "MusicLibrary", menuName = "Moonpetal/Audio/Music Library")]
    public class MusicLibrarySO : ScriptableObject
    {
        public List<MusicTrackSO> tracks = new List<MusicTrackSO>();

        public MusicTrackSO GetTrackForContext(MusicContext context)
        {
            if (context == MusicContext.None || tracks == null) return null;
            for (int i = 0; i < tracks.Count; i++)
            {
                MusicTrackSO t = tracks[i];
                if (t != null && !t.isStinger && !t.isDormant && t.context == context) return t;
            }
            return context == MusicContext.Cutscene_Opening ? GetTrackById("T01") : null;
        }

        public MusicTrackSO GetStinger(StingerType stingerType)
        {
            if (stingerType == StingerType.None || tracks == null) return null;
            for (int i = 0; i < tracks.Count; i++)
            {
                MusicTrackSO t = tracks[i];
                if (t != null && t.isStinger && !t.isDormant && t.stingerType == stingerType) return t;
            }
            return null;
        }

        public MusicTrackSO GetTrackById(string trackId)
        {
            if (string.IsNullOrEmpty(trackId) || tracks == null) return null;
            for (int i = 0; i < tracks.Count; i++)
            {
                MusicTrackSO t = tracks[i];
                if (t != null && string.Equals(t.trackId, trackId, StringComparison.OrdinalIgnoreCase)) return t;
            }
            return null;
        }
    }
}
