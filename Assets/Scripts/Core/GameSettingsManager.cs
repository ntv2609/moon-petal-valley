using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoonpetalValley.Core
{
    public static class GameSettingsManager
    {
        public const string KEY_BGM = "mv_bgm_vol";
        public const string KEY_SFX = "mv_sfx_vol";
        public const string KEY_MUTE = "mv_muted";
        public const string KEY_FULLSCREEN = "mv_fullscreen";
        public const string KEY_RES_INDEX = "mv_res_index";
        public const string KEY_TUTORIAL = "mv_tutorial_enabled";

        public static readonly Vector2Int[] SupportedResolutions = new[]
        {
            new Vector2Int(1920, 1080),
            new Vector2Int(1600, 900),
            new Vector2Int(1366, 768),
            new Vector2Int(1280, 720)
        };

        private static bool _initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitOnLoad()
        {
            if (!_initialized)
            {
                _initialized = true;
                SceneManager.sceneLoaded += (_, __) => ApplyAudioAndFlags();
            }
            ApplyAllSettings(false);
        }

        public static float BgmVolume
        {
            get => PlayerPrefs.GetFloat(KEY_BGM, 0.75f);
            set
            {
                PlayerPrefs.SetFloat(KEY_BGM, Mathf.Clamp01(value));
                ApplyAudioAndFlags();
            }
        }

        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(KEY_SFX, 0.85f);
            set
            {
                PlayerPrefs.SetFloat(KEY_SFX, Mathf.Clamp01(value));
                ApplyAudioAndFlags();
            }
        }

        public static bool IsMuted
        {
            get => PlayerPrefs.GetInt(KEY_MUTE, 0) == 1;
            set
            {
                PlayerPrefs.SetInt(KEY_MUTE, value ? 1 : 0);
                ApplyAudioAndFlags();
            }
        }

        public static bool IsFullscreen
        {
            get => PlayerPrefs.GetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;
            set
            {
                PlayerPrefs.SetInt(KEY_FULLSCREEN, value ? 1 : 0);
                ApplyDisplaySettings();
            }
        }

        public static int ResolutionIndex
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt(KEY_RES_INDEX, 3), 0, SupportedResolutions.Length - 1);
            set
            {
                int clamped = ((value % SupportedResolutions.Length) + SupportedResolutions.Length) % SupportedResolutions.Length;
                PlayerPrefs.SetInt(KEY_RES_INDEX, clamped);
                ApplyDisplaySettings();
            }
        }

        public static bool TutorialHintsEnabled
        {
            get => PlayerPrefs.GetInt(KEY_TUTORIAL, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(KEY_TUTORIAL, value ? 1 : 0);
                ApplyTutorialFlag();
            }
        }

        public static void ApplyAllSettings(bool applyDisplay)
        {
            ApplyAudioAndFlags();
            if (applyDisplay) ApplyDisplaySettings();
        }

        public static void ApplyAudioAndFlags()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
            {
                GameRoot.Instance.Audio.SetBgmVolume(BgmVolume);
                GameRoot.Instance.Audio.SetSfxVolume(SfxVolume);
                GameRoot.Instance.Audio.SetMuted(IsMuted);
            }
        }

        private static void ApplyTutorialFlag()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag("flag_tutorial_skipped", !TutorialHintsEnabled);
            }
        }

        public static void ApplyDisplaySettings()
        {
            Vector2Int res = SupportedResolutions[ResolutionIndex];
            Screen.SetResolution(res.x, res.y, IsFullscreen);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}
