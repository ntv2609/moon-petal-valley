using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
using MoonpetalValley.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MoonpetalValley.Narrative
{
    public class IntroCutsceneManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup cutsceneRootGroup;
        [SerializeField] private Image fadeOverlay;
        [SerializeField] private RectTransform topLetterbox;
        [SerializeField] private RectTransform bottomLetterbox;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private CanvasGroup subtitleGroup;
        [SerializeField] private GameObject skipBarRoot;
        [SerializeField] private Image skipFillImage;
        [SerializeField] private TMP_Text skipHintText;
        [SerializeField] private TMP_Text continueHintText;

        [Header("Scene Targets")]
        [SerializeField] private Transform seedBoxTransform;
        [SerializeField] private float letterboxHeight = 96f;
        [SerializeField] private float skipHoldDuration = 1.2f;

        private bool _isPlaying;
        private bool _advanceRequested;
        private float _spaceHeldTime;
        private bool _spaceWasDown;
        private float _skipHoldTimer;
        private float _stepLockTimer;
        private Coroutine _playRoutine;
        private Coroutine _subtitleFadeRoutine;
        private CameraFollow2D _camFollow;
        private readonly List<CanvasGroup> _hiddenHudGroups = new List<CanvasGroup>();
        private Vector3 _seedBoxOriginalPos;
        private bool _hasSeedBoxPos;
        private SpriteRenderer _seedBoxGlowRenderer;

        private const float TAP_THRESHOLD = 0.28f;
        private const float HOLD_VISUAL_DELAY = 0.22f;

        public bool IsPlaying => _isPlaying;

        private void Start()
        {
            if (cutsceneRootGroup != null)
            {
                cutsceneRootGroup.alpha = 0f;
                cutsceneRootGroup.blocksRaycasts = false;
                cutsceneRootGroup.interactable = false;
            }
            if (skipBarRoot != null) skipBarRoot.SetActive(false);

            if (seedBoxTransform == null)
            {
                GameObject boxGo = GameObject.Find("GreenhouseSeedBox");
                if (boxGo != null) seedBoxTransform = boxGo.transform;
            }

            if (seedBoxTransform != null)
            {
                _seedBoxOriginalPos = seedBoxTransform.position;
                _hasSeedBoxPos = true;
            }

            if (SaveLoadManager.IsPendingLoad)
            {
                return;
            }

            StartCoroutine(CheckAndAutoPlayAfterLoad());
        }

        private IEnumerator CheckAndAutoPlayAfterLoad()
        {
            // Cho 2 frame de SaveLoadManager.DelayedLoadRoutine() hoan tat nap 100% file luu
            float _waitGuard = 0f;
            while (SaveLoadManager.IsPendingLoad && _waitGuard < 2.0f)
            {
                _waitGuard += Time.unscaledDeltaTime;
                yield return null;
            }
            if (SaveLoadManager.IsPendingLoad) yield break;
            if (ShouldAutoPlayIntro())
            {
                StartIntroCutscene();
            }
        }

        private bool ShouldAutoPlayIntro()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return false;
            var flags = GameRoot.Instance.Flags;
            if (flags.HasFlag("flag_intro_cutscene_done")) return false;
            if (flags.HasFlag("flag_intro_letter_read")) return false;
            if (flags.HasFlag("flag_greenhouse_cleared")) return false;
            if (flags.HasFlag("flag_lumi_awakened")) return false;
            if (flags.HasFlag("flag_tutorial_done")) return false;
            if (flags.HasFlag("flag_nv1_completed")) return false;
            return true;
        }

        private void Update()
        {
            if (Application.isEditor || Debug.isDebugBuild)
            {
                if (!_isPlaying && IsReplayHotkeyPressed() &&
                    GameRoot.Instance != null && GameRoot.Instance.State != null &&
                    GameRoot.Instance.State.IsGameplayActive)
                {
                    StartIntroCutscene();
                    return;
                }
            }

            if (!_isPlaying) return;

            if (_stepLockTimer > 0f)
            {
                _stepLockTimer -= Time.unscaledDeltaTime;
            }

            if (continueHintText != null && continueHintText.gameObject.activeSelf)
            {
                float a = 0.45f + 0.35f * (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 3.0f));
                Color c = continueHintText.color;
                c.a = a;
                continueHintText.color = c;
            }

            HandleCutsceneInput();
        }

        private void HandleCutsceneInput()
        {
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
                if (_stepLockTimer <= 0f && _spaceHeldTime > 0f && _spaceHeldTime < TAP_THRESHOLD)
                {
                    _advanceRequested = true;
                }
                _spaceWasDown = false;
                _spaceHeldTime = 0f;
            }

            bool holdingSkip = escHeld || (spaceHeld && _spaceHeldTime >= HOLD_VISUAL_DELAY);
            if (holdingSkip)
            {
                _skipHoldTimer += Time.unscaledDeltaTime;
                float effectiveDuration = Mathf.Max(0.25f, skipHoldDuration - (escHeld ? 0f : HOLD_VISUAL_DELAY));
                float pct = Mathf.Clamp01(_skipHoldTimer / effectiveDuration);
                if (skipBarRoot != null && !skipBarRoot.activeSelf) skipBarRoot.SetActive(true);
                if (skipFillImage != null) skipFillImage.fillAmount = pct;
                if (pct >= 1f)
                {
                    SkipCutsceneNow();
                }
            }
            else if (_skipHoldTimer > 0f)
            {
                _skipHoldTimer = Mathf.Max(0f, _skipHoldTimer - Time.unscaledDeltaTime * 3.0f);
                if (skipFillImage != null)
                {
                    skipFillImage.fillAmount = Mathf.Clamp01(_skipHoldTimer / Mathf.Max(0.25f, skipHoldDuration));
                }
                if (_skipHoldTimer <= 0.001f && skipBarRoot != null && skipBarRoot.activeSelf)
                {
                    skipBarRoot.SetActive(false);
                }
            }
        }

        public void StartIntroCutscene()
        {
            if (_isPlaying) return;
            _camFollow = UnityEngine.Object.FindAnyObjectByType<CameraFollow2D>();
            _isPlaying = true;
            _advanceRequested = false;
            _spaceWasDown = false;
            _spaceHeldTime = 0f;
            _skipHoldTimer = 0f;
            _stepLockTimer = 0.25f;

            if (skipFillImage != null) skipFillImage.fillAmount = 0f;
            if (skipBarRoot != null) skipBarRoot.SetActive(false);

            if (_playRoutine != null) StopCoroutine(_playRoutine);
            _playRoutine = StartCoroutine(PlayCutsceneRoutine());
        }

        private IEnumerator PlayCutsceneRoutine()
        {
            GameRoot.Instance?.State?.SetState(GameState.Cutscene);
            SetGameplayHudVisible(false);

            if (cutsceneRootGroup != null)
            {
                cutsceneRootGroup.alpha = 1f;
                cutsceneRootGroup.blocksRaycasts = true;
            }
            if (fadeOverlay != null) fadeOverlay.color = new Color(0.05f, 0.04f, 0.07f, 1f);
            if (skipHintText != null) skipHintText.text = "Giữ [Space] / [Esc] để bỏ qua";
            if (continueHintText != null)
            {
                continueHintText.text = "[Space] Tiếp tục";
                continueHintText.gameObject.SetActive(false);
            }
            SetSubtitle("", "");
            SetLetterboxImmediate(0f);

            if (_camFollow != null)
            {
                _camFollow.BeginCutsceneOverride(new Vector3(0f, -10.0f, -10f), 6.2f);
                _camFollow.SetCutsceneTarget(new Vector3(0f, -1.2f, -10f), 5.6f, 0.35f);
            }

            StartCoroutine(AnimateLetterbox(0f, letterboxHeight, 0.65f));
            yield return FadeOverlayAlpha(1f, 0f, 1.3f);

            if (continueHintText != null) continueHintText.gameObject.SetActive(true);

            SetSubtitle("Wen", "Ba mùa thu trôi qua kể từ ngày bà mất. Tôi vẫn giữ chùm chìa khóa cũ của bà trong túi áo.");
            yield return WaitOrAdvance(6.5f);

            SetSubtitle("Wen", "Thung lũng Moonpetal ngày xưa ngập hương hoa. Giờ chỉ còn làn sương tím ẩm lạnh phủ kín lối đi.");
            yield return WaitOrAdvance(7.0f);

            if (_camFollow != null)
            {
                _camFollow.SetCutsceneTarget(new Vector3(1.5f, 4.5f, -10f), 5.5f, 0.85f);
            }
            SetSubtitle("Wen", "Lò bánh của bà Hana vẫn đỏ lửa mỗi sáng, nhưng chẳng còn mấy ai ghé ngang.");
            yield return WaitOrAdvance(7.5f);

            if (_camFollow != null)
            {
                _camFollow.SetCutsceneTarget(new Vector3(10.0f, 6.2f, -10f), 5.4f, 0.80f);
            }
            SetSubtitle("Wen", "Khu vườn phía Nam đóng cổng đã lâu. Người ta bảo hoa Moonpetal đã tắt lịm cùng mùa đông năm ấy.");
            yield return WaitOrAdvance(7.5f);

            if (_camFollow != null)
            {
                _camFollow.SetCutsceneTarget(new Vector3(-15.5f, 10.6f, -10f), 4.6f, 0.95f);
            }
            SetSubtitle("Wen", "Hôm qua, tôi nhận được phong thư viết sẵn của bà, gửi nhờ bác đưa thư Otis giữ suốt ba năm.");
            yield return WaitOrAdvance(8.0f);

            if (_camFollow != null)
            {
                _camFollow.SetCutsceneTarget(new Vector3(-15.5f, 12.2f, -10f), 4.1f, 1.0f);
            }
            SetSubtitle("Wen", "Bà chỉ dặn một câu: 'Nếu con thấy mệt với phố thị, hãy về mở cửa nhà kính cũ.'");
            yield return WaitOrAdvance(8.0f);

            Vector3 boxFocus = seedBoxTransform != null ? seedBoxTransform.position : new Vector3(-17.2f, 12.5f, 0f);
            if (_camFollow != null)
            {
                _camFollow.SetCutsceneTarget(new Vector3(boxFocus.x + 0.4f, boxFocus.y - 0.3f, -10f), 3.6f, 1.35f);
            }

            SetSubtitle("", "<i>Tiếng sột soạt khe khẽ vang lên từ chiếc hộp gỗ cũ trên kệ...</i>");
            yield return ShakeAndGlowSeedBoxOrAdvance(3.6f);

            GameRoot.Instance?.Audio?.PlayUIClick();
            SetSubtitle("???", "Khịt khịt... Mùi nắng mới? Hay là có người mở cửa nhà kính thật rồi?");
            yield return ShakeAndGlowSeedBoxOrAdvance(5.8f);

            SetSubtitle("Wen", "Khoan đã... Cái hộp gỗ của bà vừa rung lên sao?");
            yield return WaitOrAdvance(5.2f);

            if (continueHintText != null) continueHintText.gameObject.SetActive(false);

            Vector3 wenPos = new Vector3(-15.0f, 10.0f, 0f);
            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null) wenPos = player.transform.position;

            float baseOrtho = _camFollow != null ? _camFollow.DefaultOrthoSize : 5f;
            if (_camFollow != null)
            {
                _camFollow.SetCutsceneTarget(new Vector3(wenPos.x, wenPos.y, -10f), baseOrtho, 2.0f);
            }
            SetSubtitle("", "");
            yield return AnimateLetterbox(letterboxHeight, 0f, 0.75f);
            yield return WaitUnscaled(0.4f);

            FinishCutscene(false);
        }

        private void SkipCutsceneNow()
        {
            if (!_isPlaying) return;
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }
            FinishCutscene(true);
        }

        private void FinishCutscene(bool wasSkipped)
        {
            _isPlaying = false;
            _advanceRequested = false;
            _spaceWasDown = false;
            _spaceHeldTime = 0f;
            _skipHoldTimer = 0f;
            RestoreSeedBoxVisual();

            if (_camFollow != null)
            {
                _camFollow.EndCutsceneOverride(wasSkipped);
            }

            SetSubtitle("", "");
            SetLetterboxImmediate(0f);
            if (skipBarRoot != null) skipBarRoot.SetActive(false);
            if (continueHintText != null) continueHintText.gameObject.SetActive(false);
            if (fadeOverlay != null) fadeOverlay.color = new Color(0.05f, 0.04f, 0.07f, 0f);
            if (cutsceneRootGroup != null)
            {
                cutsceneRootGroup.alpha = 0f;
                cutsceneRootGroup.blocksRaycasts = false;
            }

            SetGameplayHudVisible(true);

            GameRoot.Instance?.Flags?.SetFlag("flag_intro_cutscene_done", true);
            GameRoot.Instance?.State?.ReturnToGameplay();

            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                !GameRoot.Instance.Flags.HasFlag("flag_intro_letter_read"))
            {
                GameEventBus.RaiseToastRequested("Chào mừng về Thung Lũng Moonpetal! Hãy đọc bức thư trên bàn gỗ [E].");
            }
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

        private IEnumerator ShakeAndGlowSeedBoxOrAdvance(float duration)
        {
            EnsureSeedBoxGlow();
            _advanceRequested = false;
            _stepLockTimer = 0.16f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (_advanceRequested)
                {
                    _advanceRequested = false;
                    GameRoot.Instance?.Audio?.PlayUIClick();
                    break;
                }
                elapsed += Time.unscaledDeltaTime;
                if (seedBoxTransform != null && _hasSeedBoxPos)
                {
                    float pulse = Mathf.Sin(elapsed * 28f) * 0.045f;
                    seedBoxTransform.position = _seedBoxOriginalPos + new Vector3(pulse, Mathf.Abs(pulse) * 0.4f, 0f);
                }
                if (_seedBoxGlowRenderer != null)
                {
                    _seedBoxGlowRenderer.enabled = true;
                    float a = 0.28f + Mathf.Sin(elapsed * 5.5f) * 0.16f;
                    _seedBoxGlowRenderer.color = new Color(0.68f, 0.88f, 1.0f, a);
                    float s = 1.35f + Mathf.Sin(elapsed * 4.2f) * 0.18f;
                    _seedBoxGlowRenderer.transform.localScale = new Vector3(s, s, 1f);
                }
                yield return null;
            }
            if (seedBoxTransform != null && _hasSeedBoxPos)
            {
                seedBoxTransform.position = _seedBoxOriginalPos;
            }
        }

        private void EnsureSeedBoxGlow()
        {
            if (_seedBoxGlowRenderer != null || seedBoxTransform == null) return;
            GameObject glowGo = new GameObject("CutsceneSeedBoxGlow");
            glowGo.transform.SetParent(seedBoxTransform, false);
            glowGo.transform.localPosition = new Vector3(0f, 0.1f, 0f);

            var sr = glowGo.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSoftGlowSprite();
            sr.sortingLayerName = "WeatherVFX";
            sr.sortingOrder = 15;
            sr.color = new Color(0.68f, 0.88f, 1.0f, 0f);
            _seedBoxGlowRenderer = sr;
        }

        private void RestoreSeedBoxVisual()
        {
            if (seedBoxTransform != null && _hasSeedBoxPos)
            {
                seedBoxTransform.position = _seedBoxOriginalPos;
            }
            if (_seedBoxGlowRenderer != null)
            {
                _seedBoxGlowRenderer.enabled = false;
            }
        }

        private Sprite CreateSoftGlowSprite()
        {
            const int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float half = (size - 1) * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - half) / half;
                    float dy = (y - half) / half;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(1f - d);
                    a = a * a;
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
        }

        private void SetGameplayHudVisible(bool visible)
        {
            if (!visible)
            {
                _hiddenHudGroups.Clear();
                string[] hudCanvasNames = { "ClockHUDCanvas", "StatsHUDCanvas", "HotbarCanvas", "QuestTrackerCanvas" };
                for (int i = 0; i < hudCanvasNames.Length; i++)
                {
                    GameObject go = GameObject.Find(hudCanvasNames[i]);
                    if (go == null) continue;
                    CanvasGroup cg = go.GetComponent<CanvasGroup>();
                    if (cg == null) cg = go.AddComponent<CanvasGroup>();
                    cg.alpha = 0f;
                    cg.interactable = false;
                    cg.blocksRaycasts = false;
                    _hiddenHudGroups.Add(cg);
                }
            }
            else
            {
                for (int i = 0; i < _hiddenHudGroups.Count; i++)
                {
                    if (_hiddenHudGroups[i] == null) continue;
                    _hiddenHudGroups[i].alpha = 1f;
                    _hiddenHudGroups[i].interactable = true;
                    _hiddenHudGroups[i].blocksRaycasts = true;
                }
                _hiddenHudGroups.Clear();
            }
        }

        private void SetSubtitle(string speaker, string body)
        {
            if (speakerText != null)
            {
                speakerText.text = string.IsNullOrEmpty(speaker) ? "" : speaker;
                speakerText.gameObject.SetActive(!string.IsNullOrEmpty(speaker));
            }
            if (subtitleText != null)
            {
                subtitleText.text = body ?? "";
            }
            if (subtitleGroup != null && !string.IsNullOrEmpty(body))
            {
                if (_subtitleFadeRoutine != null) StopCoroutine(_subtitleFadeRoutine);
                _subtitleFadeRoutine = StartCoroutine(FadeSubtitleIn());
            }
        }

        private IEnumerator FadeSubtitleIn()
        {
            subtitleGroup.alpha = 0f;
            float t = 0f;
            while (t < 0.28f)
            {
                t += Time.unscaledDeltaTime;
                subtitleGroup.alpha = Mathf.Clamp01(t / 0.28f);
                yield return null;
            }
            subtitleGroup.alpha = 1f;
        }

        private void SetLetterboxImmediate(float height)
        {
            if (topLetterbox != null) topLetterbox.sizeDelta = new Vector2(0f, height);
            if (bottomLetterbox != null) bottomLetterbox.sizeDelta = new Vector2(0f, height);
        }

        private IEnumerator AnimateLetterbox(float fromH, float toH, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
                SetLetterboxImmediate(Mathf.Lerp(fromH, toH, k));
                yield return null;
            }
            SetLetterboxImmediate(toH);
        }

        private IEnumerator FadeOverlayAlpha(float fromA, float toA, float duration)
        {
            if (fadeOverlay == null) yield break;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Lerp(fromA, toA, Mathf.Clamp01(t / duration));
                fadeOverlay.color = new Color(0.05f, 0.04f, 0.07f, a);
                yield return null;
            }
            fadeOverlay.color = new Color(0.05f, 0.04f, 0.07f, toA);
        }

        private IEnumerator WaitUnscaled(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
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
            if (Keyboard.current != null &&
                (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame))
                return true;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                return true;
#endif
            return false;
        }

        private bool IsReplayHotkeyPressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.f8Key.wasPressedThisFrame)
                return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F8))
                return true;
#endif
            return false;
        }
    }
}
