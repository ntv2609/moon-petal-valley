using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip bgmCozyClip;
        [SerializeField] private AudioClip sfxUiClick;
        [SerializeField] private AudioClip sfxTool;
        [SerializeField] private AudioClip sfxSlash;
        [SerializeField] private AudioClip sfxHit;
        [SerializeField] private AudioClip sfxPickup;
        [SerializeField] private AudioClip sfxBloom;

        public bool IsMuted { get; private set; }

        private int _lastStamina = 100;
        private readonly Dictionary<string, AudioClip> _voiceBlips = new Dictionary<string, AudioClip>();
        private float _lastBlipTime;
        private float _bgmBaseVolume = 0.25f;
        private float _sfxVolumeScale = 1f;
        private float _suppressStaminaSlashUntil;

        private void Awake()
        {
            BuildCharacterVoiceBlips();
        }

        private void OnEnable()
        {
            GameEventBus.OnItemChanged += HandleItemChanged;
            GameEventBus.OnEnemyDefeated += HandleEnemyDefeated;
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnStaminaChanged += HandleStaminaChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnItemChanged -= HandleItemChanged;
            GameEventBus.OnEnemyDefeated -= HandleEnemyDefeated;
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnStaminaChanged -= HandleStaminaChanged;
        }

        private void Start()
        {
            // [MV-AUDIO-BEGIN]
            if (MusicManager.IsSystemEnabled()) { GameSettingsManager.ApplyAudioAndFlags(); return; }
            // [MV-AUDIO-END]
            if (bgmSource != null && bgmCozyClip != null)
            {
                bgmSource.clip = bgmCozyClip;
                bgmSource.loop = true;
                bgmSource.volume = _bgmBaseVolume;
                bgmSource.Play();
            }
            GameSettingsManager.ApplyAudioAndFlags();
        }

        private void BuildCharacterVoiceBlips()
        {
            _voiceBlips["wen"]  = CreateSoftBlipClip("blip_wen",  520f, 0.045f);
            _voiceBlips["lumi"] = CreateSoftBlipClip("blip_lumi", 790f, 0.042f);
            _voiceBlips["bach"] = CreateSoftBlipClip("blip_bach", 215f, 0.055f);
            _voiceBlips["hana"] = CreateSoftBlipClip("blip_hana", 440f, 0.048f);
            _voiceBlips["toby"] = CreateSoftBlipClip("blip_toby", 630f, 0.040f);
            _voiceBlips["otis"] = CreateSoftBlipClip("blip_otis", 295f, 0.050f);
            _voiceBlips["kael"] = CreateSoftBlipClip("blip_kael", 190f, 0.058f);
            _voiceBlips["mara"] = CreateSoftBlipClip("blip_mara", 365f, 0.046f);
            _voiceBlips["linh"] = CreateSoftBlipClip("blip_linh", 560f, 0.044f); // Step50
        }

        private static AudioClip CreateSoftBlipClip(string clipName, float freqHz, float durationSec)
        {
            int sampleRate = 22050;
            int samples = Mathf.Max(256, Mathf.RoundToInt(sampleRate * durationSec));
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float norm = (float)i / samples;
                float env = Mathf.Sin(norm * Mathf.PI);
                float tone = Mathf.Sin(2f * Mathf.PI * freqHz * t) * 0.7f
                           + Mathf.Sin(2f * Mathf.PI * (freqHz * 1.5f) * t) * 0.2f;
                data[i] = tone * env * 0.35f;
            }
            AudioClip clip = AudioClip.Create(clipName, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public void PlayVoiceBlip(string speakerId)
        {
            if (IsMuted || sfxSource == null) return;
            if (Time.unscaledTime - _lastBlipTime < 0.055f) return;
            _lastBlipTime = Time.unscaledTime;

            string key = string.IsNullOrEmpty(speakerId) ? "wen" : speakerId.ToLowerInvariant();
            if (!_voiceBlips.TryGetValue(key, out AudioClip clip))
                clip = _voiceBlips.TryGetValue("wen", out AudioClip fb) ? fb : null;
            if (clip == null) return;

            float prevPitch = sfxSource.pitch;
            sfxSource.pitch = Random.Range(0.94f, 1.07f);
            sfxSource.PlayOneShot(clip, 0.24f);
            sfxSource.pitch = prevPitch;
        }

        public void ToggleMute()
        {
            SetMuted(!IsMuted);
        }

        public void SetBgmVolume(float normalizedVolume)
        {
            _bgmBaseVolume = Mathf.Clamp01(normalizedVolume) * 0.40f;
            if (bgmSource != null) bgmSource.volume = _bgmBaseVolume;
        }

        public void SetSfxVolume(float normalizedVolume)
        {
            _sfxVolumeScale = Mathf.Clamp01(normalizedVolume);
            if (sfxSource != null) sfxSource.volume = _sfxVolumeScale;
        }

        public void SetMuted(bool muted)
        {
            IsMuted = muted;
            if (bgmSource != null) bgmSource.mute = IsMuted;
            if (sfxSource != null) sfxSource.mute = IsMuted;
        }

        public void PlaySFX(AudioClip clip, float volume = 0.55f)
        {
            if (IsMuted || sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        public void PlayComboSlashSFX(int comboStep, bool isDashStrike)
        {
            // Chặn HandleStaminaChanged phát trùng hoặc phát nhầm tiếng cuốc đất (sfxTool) khi đòn 3 tốn 3 Stamina
            _suppressStaminaSlashUntil = Time.unscaledTime + 0.09f;
            if (IsMuted || sfxSource == null || sfxSlash == null) return;

            float pitch;
            float vol;
            if (isDashStrike)
            {
                pitch = 1.22f;
                vol = 0.50f;
            }
            else if (comboStep == 2)
            {
                pitch = 1.12f;
                vol = 0.48f;
            }
            else if (comboStep == 3)
            {
                pitch = 1.30f;
                vol = 0.62f;
            }
            else
            {
                pitch = 0.96f;
                vol = 0.42f;
            }

            float prevPitch = sfxSource.pitch;
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(sfxSlash, vol);
            if (comboStep == 3 && sfxBloom != null)
            {
                sfxSource.pitch = 1.18f;
                sfxSource.PlayOneShot(sfxBloom, 0.24f);
            }
            sfxSource.pitch = prevPitch;
        }

        public void PlayUIClick() => PlaySFX(sfxUiClick, 0.45f);

        private void HandleItemChanged(string _, int __) => PlaySFX(sfxPickup, 0.45f);
        private void HandleEnemyDefeated(string _) => PlaySFX(sfxHit, 0.65f);

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            if (flagId == "flag_nv1_completed" || flagId == "flag_ch2_completed") PlaySFX(sfxBloom, 0.80f);
            else PlaySFX(sfxUiClick, 0.40f);
        }

        private void HandleStaminaChanged(int current, int max)
        {
            if (current < _lastStamina)
            {
                if (Time.unscaledTime >= _suppressStaminaSlashUntil)
                {
                    int diff = _lastStamina - current;
                    PlaySFX(diff == 2 ? sfxSlash : sfxTool, 0.40f);
                }
            }
            _lastStamina = current;
        }
    }
}
