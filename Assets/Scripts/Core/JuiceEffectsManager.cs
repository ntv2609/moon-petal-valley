using UnityEngine;
using TMPro;
using MoonpetalValley.UI;
using MoonpetalValley.Player;

namespace MoonpetalValley.Core
{
    public class JuiceEffectsManager : MonoBehaviour
    {
        public static JuiceEffectsManager Instance { get; private set; }

        [SerializeField] private TMP_FontAsset vnFont;

        private bool _hitstopActive;
        private float _hitstopRemaining;
        private float _preHitstopTimeScale = 1f;

        public TMP_FontAsset VnFont => vnFont;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!_hitstopActive) return;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                // Nếu người chơi bấm Pause trong lúc Hitstop, không ghi đè Time.timeScale = 0 của Pause
                _hitstopActive = false;
                _hitstopRemaining = 0f;
                return;
            }

            _hitstopRemaining -= Time.unscaledDeltaTime;
            if (_hitstopRemaining <= 0f)
            {
                _hitstopActive = false;
                Time.timeScale = _preHitstopTimeScale > 0.05f ? _preHitstopTimeScale : 1f;
            }
        }

        private void OnDisable()
        {
            RestoreTimeScaleIfNeeded();
        }

        private void OnDestroy()
        {
            RestoreTimeScaleIfNeeded();
        }

        private void RestoreTimeScaleIfNeeded()
        {
            if (!_hitstopActive) return;
            _hitstopActive = false;
            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (gameplayActive && Time.timeScale < 0.1f)
            {
                Time.timeScale = _preHitstopTimeScale > 0.05f ? _preHitstopTimeScale : 1f;
            }
        }

        public void TriggerHitstop(float duration = 0.032f)
        {
            if (duration <= 0.001f) return;
            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive) return;

            if (!_hitstopActive)
            {
                if (Time.timeScale <= 0.05f) return;
                _preHitstopTimeScale = Time.timeScale;
                _hitstopActive = true;
                Time.timeScale = 0.04f;
            }
            _hitstopRemaining = Mathf.Max(_hitstopRemaining, Mathf.Clamp(duration, 0.01f, 0.12f));
        }

        public void SpawnFloatingNumber(Vector3 worldPos, string text, Color color)
        {
            GameObject go = new GameObject("FloatingNumber");
            go.transform.position = worldPos;
            var comp = go.AddComponent<FloatingDamageText>();
            comp.Initialize(text, color, vnFont);
        }

        public void TriggerCameraShake(float intensity = 0.09f, float duration = 0.12f)
        {
            var cam = UnityEngine.Object.FindAnyObjectByType<CameraFollow2D>();
            if (cam != null)
            {
                cam.TriggerShake(intensity, duration);
            }
        }
    }
}
