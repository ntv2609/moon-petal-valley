using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using MoonpetalValley.Audio;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepAudio
    {
        private const string RES_FOLDER = "Assets/Resources/Moonpetal/Audio";
        private const string TRACKS_FOLDER = "Assets/Resources/Moonpetal/Audio/Tracks";
        private const string SETTINGS_PATH = RES_FOLDER + "/MusicSettings.asset";
        private const string RULES_PATH = RES_FOLDER + "/MusicRules.asset";
        private const string LIBRARY_PATH = RES_FOLDER + "/MusicLibrary.asset";

        private struct ManifestRow
        {
            public string id, fileStem, folder, notes;
            public bool isStinger, dormant;
            public MusicContext context;
            public StingerType stingerType;
            public int priority;
            public TrackPlayMode playMode;
            public float restMin, restMax;
        }

        private static readonly ManifestRow[] MANIFEST = new[]
        {
            new ManifestRow { id="T01", fileStem="MV_Music_T01_MainTheme", folder="Assets/Audio/Music/Core", context=MusicContext.Title, priority=100, playMode=TrackPlayMode.Once, notes="Title (loop) & Cutscene_Opening (once)" },
            new ManifestRow { id="T02", fileStem="MV_Music_T02_VillageDay", folder="Assets/Audio/Music/Core", context=MusicContext.Safe_Day, priority=20, playMode=TrackPlayMode.CrossfadeSelf, restMin=8f, restMax=16f, notes="Safe daytime" },
            new ManifestRow { id="T03", fileStem="MV_Music_T03_VillageNight", folder="Assets/Audio/Music/Core", context=MusicContext.Safe_Night, priority=20, playMode=TrackPlayMode.CrossfadeSelf, restMin=10f, restMax=18f, notes="Safe dusk/night (18:00+)" },
            new ManifestRow { id="T04", fileStem="MV_Music_T04_MistZone", folder="Assets/Audio/Music/Core", context=MusicContext.Mist_Zone, priority=40, playMode=TrackPlayMode.CrossfadeSelf, notes="BlightFogZone proximity" },
            new ManifestRow { id="T05", fileStem="MV_Music_T05_Combat", folder="Assets/Audio/Music/Core", context=MusicContext.Combat, priority=80, playMode=TrackPlayMode.CrossfadeSelf, notes="Enemy Layer 9 nearby" },
            new ManifestRow { id="T06", fileStem="MV_Music_T06_Emotional", folder="Assets/Audio/Music/Core", context=MusicContext.Emotional, priority=60, playMode=TrackPlayMode.CrossfadeSelf, dormant=true, notes="Dormant until dialogue emotional flag" },
            new ManifestRow { id="S1", fileStem="MV_Stinger_S1_PetalObtained", folder="Assets/Audio/Stingers", isStinger=true, stingerType=StingerType.PetalObtained, playMode=TrackPlayMode.Once, notes="Petal flag or item gain" },
            new ManifestRow { id="S2", fileStem="MV_Stinger_S2_QuestComplete", folder="Assets/Audio/Stingers", isStinger=true, stingerType=StingerType.QuestComplete, playMode=TrackPlayMode.Once, notes="Milestone quest flags" },
            new ManifestRow { id="S3", fileStem="MV_Stinger_S3_KnockedOut", folder="Assets/Audio/Stingers", isStinger=true, stingerType=StingerType.KnockedOut, playMode=TrackPlayMode.Once, notes="Player KO or late-night pass out" },
            new ManifestRow { id="S4", fileStem="MV_Stinger_S4_NewDay", folder="Assets/Audio/Stingers", isStinger=true, stingerType=StingerType.NewDay, playMode=TrackPlayMode.Once, notes="Day increment outside save-load" },
            new ManifestRow { id="T07", fileStem="MV_Music_T07_RiverDay", folder="Assets/Audio/Music/Ch2", context=MusicContext.Zone_River_Day, priority=20, playMode=TrackPlayMode.CrossfadeSelf, restMin=8f, restMax=16f, dormant=true, notes="Ch2 Riverbank day" },
            new ManifestRow { id="T08", fileStem="MV_Music_T08_LanternRelay", folder="Assets/Audio/Music/Ch2", context=MusicContext.Set_LanternRelay, priority=100, playMode=TrackPlayMode.CrossfadeSelf, dormant=true, notes="Ch2 Lantern Relay" },
            new ManifestRow { id="T09", fileStem="MV_Music_T09_NightMarket", folder="Assets/Audio/Music/Ch3", context=MusicContext.Set_NightMarket, priority=100, playMode=TrackPlayMode.CrossfadeSelf, dormant=true, notes="Ch3 Night Market" },
            new ManifestRow { id="T10", fileStem="MV_Music_T10_NorthForest", folder="Assets/Audio/Music/Ch4", context=MusicContext.Zone_NorthForest, priority=30, playMode=TrackPlayMode.CrossfadeSelf, restMin=10f, restMax=18f, dormant=true, notes="Ch4 North Forest" },
            new ManifestRow { id="T11", fileStem="MV_Music_T11_Flashback", folder="Assets/Audio/Music/Ch4", context=MusicContext.Flashback_Memory, priority=100, playMode=TrackPlayMode.CrossfadeSelf, dormant=true, notes="Ch4 Flashback" },
            new ManifestRow { id="T12", fileStem="MV_Music_T12_BossMistWarden", folder="Assets/Audio/Music/Ch5", context=MusicContext.Boss_MistWarden, priority=100, playMode=TrackPlayMode.CrossfadeSelf, dormant=true, notes="Ch5 Boss" },
            new ManifestRow { id="T13", fileStem="MV_Music_T13_TheChair", folder="Assets/Audio/Music/Ch5", context=MusicContext.Scene_TheChair, priority=100, playMode=TrackPlayMode.Once, dormant=true, notes="Ch5 The Chair" },
            new ManifestRow { id="T14", fileStem="MV_Music_T14_Ending", folder="Assets/Audio/Music/Ch5", context=MusicContext.Ending, priority=100, playMode=TrackPlayMode.Once, dormant=true, notes="Ch5 Ending" }
        };

        [MenuItem("Moonpetal/Audio/Run All", false, 10)]
        public static void RunAll() { ConfigureImportSettings(); BuildLibrary(); Validate(); }

        [MenuItem("Moonpetal/Audio/1 - Configure Import Settings", false, 20)]
        public static void ConfigureImportSettings()
        {
            string[] roots = { "Assets/Audio/Music", "Assets/Audio/Stingers" };
            int configured = 0;
            for (int r = 0; r < roots.Length; r++)
            {
                if (!AssetDatabase.IsValidFolder(roots[r])) continue;
                bool isStinger = roots[r].EndsWith("Stingers", StringComparison.OrdinalIgnoreCase);
                string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { roots[r] });
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                    if (importer == null) continue;
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    bool longClip = clip != null && clip.length > 15f;

                    AudioImporterSampleSettings sample = importer.defaultSampleSettings;
                    sample.compressionFormat = AudioCompressionFormat.Vorbis;
                    sample.quality = isStinger ? 0.8f : 0.7f;
                    sample.loadType = (isStinger && !longClip) ? AudioClipLoadType.DecompressOnLoad : AudioClipLoadType.Streaming;
                    importer.defaultSampleSettings = sample;
                    importer.loadInBackground = true;
                    importer.SaveAndReimport();
                    configured++;
                }
            }
            Debug.Log("[MV][Audio] ConfigureImportSettings done. Clips configured: " + configured);
        }

        [MenuItem("Moonpetal/Audio/2 - Build Library", false, 21)]
        public static void BuildLibrary()
        {
            EnsureFolderPath(TRACKS_FOLDER);
            MusicSettingsSO settings = LoadOrCreateAsset<MusicSettingsSO>(SETTINGS_PATH);
            MusicRulesSO rules = LoadOrCreateAsset<MusicRulesSO>(RULES_PATH);
            MusicLibrarySO library = LoadOrCreateAsset<MusicLibrarySO>(LIBRARY_PATH);
            library.tracks = new List<MusicTrackSO>();

            int boundCount = 0, tempCapped = 0;
            for (int i = 0; i < MANIFEST.Length; i++)
            {
                ManifestRow row = MANIFEST[i];
                MusicTrackSO t = LoadOrCreateAsset<MusicTrackSO>(TRACKS_FOLDER + "/" + row.fileStem + ".asset");
                t.trackId = row.id; t.isStinger = row.isStinger; t.context = row.context;
                t.stingerType = row.stingerType; t.priority = row.priority; t.playMode = row.playMode;
                t.ambientRestMinSeconds = row.restMin; t.ambientRestMaxSeconds = row.restMax;
                t.isDormant = row.dormant; t.notes = row.notes;

                AudioClip clip = FindClipByStem(row.folder, row.fileStem);
                t.clip = clip;
                if (clip != null) boundCount++;
                if (row.isStinger)
                {
                    if (clip != null && clip.length > 15f) { t.maxPlaySeconds = 6.0f; t.fadeOutSeconds = 1.2f; tempCapped++; }
                    else { t.maxPlaySeconds = 0f; t.fadeOutSeconds = 0.25f; }
                }
                EditorUtility.SetDirty(t);
                library.tracks.Add(t);
            }
            EditorUtility.SetDirty(settings); EditorUtility.SetDirty(rules); EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log(string.Format("[MV][Audio] BuildLibrary done. Tracks:{0}, Bound:{1}, TempCappedStingers:{2}", library.tracks.Count, boundCount, tempCapped));
        }

        [MenuItem("Moonpetal/Audio/3 - Validate", false, 22)]
        public static void Validate()
        {
            MusicSettingsSO s = AssetDatabase.LoadAssetAtPath<MusicSettingsSO>(SETTINGS_PATH);
            MusicRulesSO r = AssetDatabase.LoadAssetAtPath<MusicRulesSO>(RULES_PATH);
            MusicLibrarySO lib = AssetDatabase.LoadAssetAtPath<MusicLibrarySO>(LIBRARY_PATH);
            int err = 0, warn = 0;
            if (s == null) { Debug.LogError("[MV][Audio] Missing MusicSettings.asset"); err++; }
            if (r == null) { Debug.LogError("[MV][Audio] Missing MusicRules.asset"); err++; }
            if (lib == null) { Debug.LogError("[MV][Audio] Missing MusicLibrary.asset"); err++; }
            if (lib != null && lib.tracks != null)
            {
                for (int i = 0; i < lib.tracks.Count; i++)
                {
                    MusicTrackSO t = lib.tracks[i];
                    if (t == null) { err++; continue; }
                    if (!t.isDormant && t.clip == null) { Debug.LogWarning("[MV][Audio] Active track missing clip: " + t.trackId); warn++; }
                    if (t.isStinger && t.maxPlaySeconds > 0f) { Debug.LogWarning("[MV][Audio] Stinger " + t.trackId + " using temp cap " + t.maxPlaySeconds + "s."); warn++; }
                }
            }
            if (File.Exists("Assets/Scripts/Audio/AudioManager.cs") && !File.ReadAllText("Assets/Scripts/Audio/AudioManager.cs").Contains("// [MV-AUDIO-BEGIN]"))
            {
                Debug.LogWarning("[MV][Audio] AudioManager.cs missing [MV-AUDIO-BEGIN] marker."); warn++;
            }
            Debug.Log(string.Format("[MV][Audio] Validate done. Errors:{0}, Warnings:{1}, SystemEnabled:{2}", err, warn, s != null && s.systemEnabled));
        }

        [MenuItem("Moonpetal/Audio/Disable Music System", false, 40)]
        public static void DisableMusicSystem() => SetSystemEnabled(false);

        [MenuItem("Moonpetal/Audio/Enable Music System", false, 41)]
        public static void EnableMusicSystem() => SetSystemEnabled(true);

        private static void SetSystemEnabled(bool enabled)
        {
            EnsureFolderPath(RES_FOLDER);
            MusicSettingsSO s = LoadOrCreateAsset<MusicSettingsSO>(SETTINGS_PATH);
            s.systemEnabled = enabled;
            EditorUtility.SetDirty(s); AssetDatabase.SaveAssets();
            Debug.Log("[MV][Audio] MusicSettingsSO.systemEnabled = " + enabled);
        }

        [MenuItem("Moonpetal/Debug/Audio/Print Current State", false, 60)]
        public static void DebugPrintState()
        {
            if (!Application.isPlaying || MusicManager.Instance == null) { Debug.Log("[MV][Audio] Enter Play Mode first."); return; }
            MusicManager m = MusicManager.Instance;
            Debug.Log(string.Format("[MV][Audio] Ctx={0}, Track={1}, Vol={2:0.00}, Duck={3:0.00}, Muted={4}", m.ActiveContext, m.ActiveTrackId, m.EffectiveMasterScale, m.CurrentDuckMultiplier, m.EffectiveMuted));
        }

        [MenuItem("Moonpetal/Debug/Audio/Test Stinger S1 (Petal)", false, 61)]
        public static void DebugTestStingerS1()
        {
            if (!Application.isPlaying || MusicManager.Instance == null) { Debug.Log("[MV][Audio] Enter Play Mode first."); return; }
            MusicManager.Instance.PlayStinger(StingerType.PetalObtained);
        }

        private static AudioClip FindClipByStem(string folder, string stem)
        {
            string[] exts = { ".wav", ".ogg", ".mp3" };
            for (int i = 0; i < exts.Length; i++)
            {
                AudioClip c = AssetDatabase.LoadAssetAtPath<AudioClip>(folder + "/" + stem + exts[i]);
                if (c != null) return c;
            }
            return null;
        }

        private static T LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            return asset;
        }

        private static void EnsureFolderPath(string folderPath)
        {
            string[] parts = folderPath.Split('/');
            string cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }
    }
}
