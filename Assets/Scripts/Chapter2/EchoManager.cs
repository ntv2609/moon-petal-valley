using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MoonpetalValley.Chapter2
{
    public class EchoManager : MonoBehaviour
    {
        public static EchoManager Instance { get; private set; }

        [Header("UI Overlay")]
        [SerializeField] private CanvasGroup overlayGroup;
        [SerializeField] private Image tintOverlay;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private GameObject skipBarRoot;
        [SerializeField] private Image skipFillImage;

        [Header("Echo Ghost Prefabs (FINAL)")]
        [SerializeField] private GameObject kaelGhostPrefab;
        [SerializeField] private GameObject maraGhostPrefab;

        [Header("Config")]
        [SerializeField] private float skipHoldDuration = 1.0f;

        private bool _isPlaying;
        private bool _advanceRequested;
        private float _skipHoldTimer;
        private float _stepLockTimer;
        private float _spaceHeldTime;
        private bool _spaceWasDown;
        private Coroutine _playRoutine;
        private Action _onFinishCallback;
        private EchoData _activeEcho;
        private GameObject _spawnedKael;
        private GameObject _spawnedMara;

        public bool IsPlaying => _isPlaying;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            HideImmediate();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void HideImmediate()
        {
            if (overlayGroup != null)
            {
                overlayGroup.alpha = 0f;
                overlayGroup.blocksRaycasts = false;
                overlayGroup.interactable = false;
            }
            if (skipBarRoot != null) skipBarRoot.SetActive(false);
            if (skipFillImage != null) skipFillImage.fillAmount = 0f;
        }

        public void PlayEcho(EchoData data, Vector3 worldAnchor, Action onFinish = null)
        {
            if (_isPlaying || data == null || data.beats == null || data.beats.Count == 0) return;

            _activeEcho = data;
            _onFinishCallback = onFinish;
            _isPlaying = true;
            _advanceRequested = false;
            _skipHoldTimer = 0f;
            _spaceHeldTime = 0f;
            _spaceWasDown = false;
            _stepLockTimer = 0.22f;

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null) lumi.TriggerPulse(1.5f);

            SpawnGhosts(data, worldAnchor);

            if (_playRoutine != null) StopCoroutine(_playRoutine);
            _playRoutine = StartCoroutine(PlayEchoRoutine(data));
        }

        private void SpawnGhosts(EchoData data, Vector3 anchor)
        {
            ClearGhosts();
            GameObject prefabA = data.customGhostAPrefab != null ? data.customGhostAPrefab : kaelGhostPrefab; // Step50
            GameObject prefabB = data.customGhostBPrefab != null ? data.customGhostBPrefab : maraGhostPrefab;
            if (data.showKaelGhost && prefabA != null)
            {
                _spawnedKael = Instantiate(prefabA, anchor + new Vector3(-0.75f, 0.35f, 0f), Quaternion.identity);
            }
            if (data.showMaraGhost && prefabB != null)
            {
                _spawnedMara = Instantiate(prefabB, anchor + new Vector3(0.75f, 0.35f, 0f), Quaternion.identity);
            }
        }

        private void ClearGhosts()
        {
            if (_spawnedKael != null) Destroy(_spawnedKael);
            if (_spawnedMara != null) Destroy(_spawnedMara);
            _spawnedKael = null;
            _spawnedMara = null;
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (_stepLockTimer > 0f) _stepLockTimer -= Time.unscaledDeltaTime;

            bool spaceHeld = IsSpaceHeld();
            bool escHeld = IsEscapeHeld();

            if (_stepLockTimer <= 0f && IsQuickAdvancePressed())
            {
                _advanceRequested = true;
            }

            if (spaceHeld)
            {
                _spaceWasDown = true;
                _spaceHeldTime += Time.unscaledDeltaTime;
            }
            else if (_spaceWasDown)
            {
                if (_stepLockTimer <= 0f && _spaceHeldTime > 0f && _spaceHeldTime < 0.26f)
                {
                    _advanceRequested = true;
                }
                _spaceWasDown = false;
                _spaceHeldTime = 0f;
            }

            bool holdingSkip = escHeld || (spaceHeld && _spaceHeldTime >= 0.22f);
            if (holdingSkip)
            {
                _skipHoldTimer += Time.unscaledDeltaTime;
                float pct = Mathf.Clamp01(_skipHoldTimer / Mathf.Max(0.25f, skipHoldDuration));
                if (skipBarRoot != null && !skipBarRoot.activeSelf) skipBarRoot.SetActive(true);
                if (skipFillImage != null) skipFillImage.fillAmount = pct;
                if (pct >= 1f) SkipNow();
            }
            else if (_skipHoldTimer > 0f)
            {
                _skipHoldTimer = Mathf.Max(0f, _skipHoldTimer - Time.unscaledDeltaTime * 3f);
                if (skipFillImage != null) skipFillImage.fillAmount = Mathf.Clamp01(_skipHoldTimer / Mathf.Max(0.25f, skipHoldDuration));
                if (_skipHoldTimer <= 0.001f && skipBarRoot != null) skipBarRoot.SetActive(false);
            }
        }

        private IEnumerator PlayEchoRoutine(EchoData data)
        {
            GameRoot.Instance?.State?.SetState(GameState.Cutscene);
            if (titleText != null) titleText.text = "DƯ ÂM: " + data.echoTitle;

            yield return FadeGroup(0f, 1f, 0.35f);

            var kaelActor = _spawnedKael != null ? _spawnedKael.GetComponent<EchoGhostActor>() : null;
            var maraActor = _spawnedMara != null ? _spawnedMara.GetComponent<EchoGhostActor>() : null;

            for (int i = 0; i < data.beats.Count; i++)
            {
                EchoBeat beat = data.beats[i];
                if (tintOverlay != null) tintOverlay.color = beat.overlayTint;
                if (speakerText != null) speakerText.text = beat.speakerName;
                if (bodyText != null) bodyText.text = beat.bodyText;

                if (kaelActor != null) kaelActor.SetActionMode(beat.kaelPlaysAction);
                if (maraActor != null) maraActor.SetActionMode(beat.maraPlaysAction);

                if (!string.IsNullOrEmpty(beat.speakerId))
                {
                    GameRoot.Instance?.Audio?.PlayVoiceBlip(beat.speakerId);
                }

                yield return WaitOrAdvance(Mathf.Max(1.5f, beat.durationSeconds));
            }

            yield return FadeGroup(1f, 0f, 0.30f);
            FinishEcho();
        }

        private void SkipNow()
        {
            if (!_isPlaying) return;
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }
            FinishEcho();
        }

        private void FinishEcho()
        {
            _isPlaying = false;
            HideImmediate();
            ClearGhosts();

            if (_activeEcho != null)
            {
                Chapter2StateManager.Instance?.MarkEchoCompleted(_activeEcho.echoID);
                if (!string.IsNullOrEmpty(_activeEcho.completedFlag))
                {
                    GameRoot.Instance?.Flags?.SetFlag(_activeEcho.completedFlag, true);
                }
            }

            GameRoot.Instance?.State?.ReturnToGameplay();

            var cb = _onFinishCallback;
            _activeEcho = null;
            _onFinishCallback = null;
            cb?.Invoke();
        }

        private IEnumerator FadeGroup(float from, float to, float duration)
        {
            if (overlayGroup == null) yield break;
            overlayGroup.blocksRaycasts = to > 0.5f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                overlayGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
                yield return null;
            }
            overlayGroup.alpha = to;
        }

        private IEnumerator WaitOrAdvance(float seconds)
        {
            _advanceRequested = false;
            _stepLockTimer = 0.16f;
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                if (_advanceRequested)
                {
                    _advanceRequested = false;
                    GameRoot.Instance?.Audio?.PlayUIClick();
                    yield break;
                }
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        private bool IsSpaceHeld()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.Space)) return true;
#endif
            return false;
        }

        private bool IsEscapeHeld()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.isPressed) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.Escape)) return true;
#endif
            return false;
        }

        private bool IsQuickAdvancePressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)) return true;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) return true;
#endif
            return false;
        }
    }
}
