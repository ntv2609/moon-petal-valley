using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MoonpetalValley.Chapter2
{
    public class Chapter2IntroCutscene : MonoBehaviour
    {
        [Header("UI Overlay")]
        [SerializeField] private CanvasGroup rootGroup;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private GameObject skipBarRoot;
        [SerializeField] private Image skipFillImage;

        [Header("Scene Props")]
        [SerializeField] private Transform tempPlankTransform;
        [SerializeField] private Vector3 riverFocusPos = new Vector3(1.5f, 0.5f, -10f);
        [SerializeField] private Vector3 workshopFocusPos = new Vector3(11.0f, 4.5f, -10f);
        [SerializeField] private Vector2 wenPullStandPos = new Vector2(5.1f, 0.5f);
        [SerializeField] private float skipHoldDuration = 1.0f;

        private bool _isPlaying;
        private bool _advanceRequested;
        private float _stepLockTimer;
        private float _skipHoldTimer;
        private float _spaceHeldTime;
        private bool _spaceWasDown;
        private Coroutine _routine;
        private Coroutine _pullAnimRoutine;
        private CameraFollow2D _cam;
        private PlayerController2D _player;
        private WenSpriteAnimator _wenAnim;
        private Vector3 _plankStartPos;

        private void Awake()
        {
            if (tempPlankTransform != null)
            {
                _plankStartPos = tempPlankTransform.position;
                tempPlankTransform.gameObject.SetActive(false);
            }
            HideUIImmediate();
        }

        private void HideUIImmediate()
        {
            if (rootGroup != null)
            {
                rootGroup.alpha = 0f;
                rootGroup.blocksRaycasts = false;
                rootGroup.interactable = false;
            }
            if (skipBarRoot != null) skipBarRoot.SetActive(false);
            if (skipFillImage != null) skipFillImage.fillAmount = 0f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isPlaying || !other.CompareTag("Player")) return;
            var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags != null && flags.HasFlag("flag_ch2_solo_bridge_failed")) return;

            StartCutscene();
        }

        public void StartCutscene()
        {
            if (_isPlaying) return;
            _isPlaying = true;
            _advanceRequested = false;
            _skipHoldTimer = 0f;
            _spaceHeldTime = 0f;
            _spaceWasDown = false;
            _stepLockTimer = 0.2f;
            _cam = UnityEngine.Object.FindAnyObjectByType<CameraFollow2D>();
            _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            _wenAnim = _player != null ? _player.GetComponent<WenSpriteAnimator>() : null;

            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(PlayRoutine());
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (_stepLockTimer > 0f) _stepLockTimer -= Time.unscaledDeltaTime;

            bool spaceHeld = IsSpaceHeld();
            bool escHeld = IsEscapeHeld();

            if (_stepLockTimer <= 0f && IsQuickAdvancePressed()) _advanceRequested = true;

            if (spaceHeld)
            {
                _spaceWasDown = true;
                _spaceHeldTime += Time.unscaledDeltaTime;
            }
            else if (_spaceWasDown)
            {
                if (_stepLockTimer <= 0f && _spaceHeldTime > 0f && _spaceHeldTime < 0.26f) _advanceRequested = true;
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

        private IEnumerator PlayRoutine()
        {
            GameRoot.Instance?.State?.SetState(GameState.Cutscene);
            if (rootGroup != null)
            {
                rootGroup.alpha = 1f;
                rootGroup.blocksRaycasts = true;
            }

            // Beat 1: Wen bước tới sát đầu nhịp cầu gãy và chuẩn bị kéo ván
            if (_cam != null) _cam.SetCutsceneTarget(riverFocusPos, 5.2f, 1.8f);
            SetLine("Wen", "Nhịp cầu chỉ hụt chừng ba sải tay. Mình kéo mấy tấm ván ven bờ ghép tạm chắc qua được.");
            yield return MoveWenToPullSpot();

            // Hiện tấm ván ngay trước tay Wen và diễn hoạt Wen kéo ván (Chore anim dùng chung)
            if (tempPlankTransform != null)
            {
                tempPlankTransform.position = new Vector3(wenPullStandPos.x - 0.9f, wenPullStandPos.y, 0f);
                tempPlankTransform.gameObject.SetActive(true);
            }
            _pullAnimRoutine = StartCoroutine(PlayWenPullingPlankLoop());

            SetLine("Tinh Linh Lumi", "Khịt khịt! Nước chảy xiết lắm Wen ơi, một mình kéo ván không nổi đâu!");
            yield return WaitOrAdvance(3.8f);

            // Beat 2: Trượt tay! Dừng kéo, Wen lùi nhẹ và ván bị nước cuốn trôi xuống Nam
            StopPullingAnim();
            if (_player != null)
            {
                _player.TeleportTo(new Vector2(wenPullStandPos.x + 0.45f, wenPullStandPos.y));
            }
            if (_wenAnim != null) _wenAnim.BeginCutsceneIdle(Vector2.left);

            if (tempPlankTransform != null)
            {
                StartCoroutine(WashPlankAway());
            }
            JuiceEffectsManager.Instance?.TriggerCameraShake(0.14f, 0.25f);
            GameRoot.Instance?.Audio?.PlayUIClick();
            SetLine("Wen", "Á—! Chết thật, trượt tay mất rồi...");
            yield return WaitOrAdvance(3.6f);

            SetLine("Tinh Linh Lumi", "Đấy! Lumi đã bảo rồi mà! Wen cứ hễ thấy hỏng là lao vào tự làm y như hồi Nhà Kính!");
            yield return WaitOrAdvance(4.0f);

            // Beat 3: Lia sang Xưởng Mộc Bờ Đông
            if (_cam != null) _cam.SetCutsceneTarget(workshopFocusPos, 4.9f, 1.6f);
            SetLine("Tinh Linh Lumi", "Ơ kìa, bác thợ mộc bên xưởng gỗ đứng nhìn nãy giờ mà chẳng thèm phụ một tay!");
            yield return WaitOrAdvance(4.0f);

            SetLine("Wen", "Chắc bác ấy có lý do. Để mình qua hỏi thử, rồi tìm chiếc đò phía Nam sang bờ bên kia.");
            yield return WaitOrAdvance(4.0f);

            FinishCutscene(false);
        }

        private IEnumerator MoveWenToPullSpot()
        {
            if (_player == null)
            {
                yield return WaitOrAdvance(2.4f);
                yield break;
            }

            Vector3 start = _player.transform.position;
            Vector3 target = new Vector3(wenPullStandPos.x, wenPullStandPos.y, 0f);
            Vector2 dir = ((Vector2)(target - start)).normalized;
            if (dir.sqrMagnitude < 0.01f) dir = Vector2.left;

            if (_wenAnim != null) _wenAnim.BeginCutsceneWalk(dir);

            float dist = Vector2.Distance(start, target);
            float dur = Mathf.Clamp(dist / 3.8f, 0.45f, 1.8f);
            float t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                Vector3 pos = Vector3.Lerp(start, target, Mathf.Clamp01(t / dur));
                _player.TeleportTo(pos);
                yield return null;
            }
            _player.TeleportTo(target);
            if (_wenAnim != null) _wenAnim.BeginCutsceneIdle(Vector2.left);
            yield return WaitOrAdvance(1.4f);
        }

        private IEnumerator PlayWenPullingPlankLoop()
        {
            if (_wenAnim != null) _wenAnim.BeginCutsceneChore(Vector2.left);

            Vector3 basePlank = tempPlankTransform != null
                ? tempPlankTransform.position
                : new Vector3(wenPullStandPos.x - 0.9f, wenPullStandPos.y, 0f);
            float t = 0f;
            while (true)
            {
                t += Time.unscaledDeltaTime;
                float tug = Mathf.Sin(t * 7.5f);
                if (_player != null)
                {
                    _player.TeleportTo(new Vector2(wenPullStandPos.x + tug * 0.08f, wenPullStandPos.y));
                }
                if (tempPlankTransform != null)
                {
                    tempPlankTransform.position = new Vector3(basePlank.x + tug * 0.14f, basePlank.y + Mathf.Abs(tug) * 0.03f, 0f);
                }
                yield return null;
            }
        }

        private void StopPullingAnim()
        {
            if (_pullAnimRoutine != null)
            {
                StopCoroutine(_pullAnimRoutine);
                _pullAnimRoutine = null;
            }
        }

        private IEnumerator WashPlankAway()
        {
            float t = 0f;
            while (t < 2.2f && tempPlankTransform != null)
            {
                t += Time.unscaledDeltaTime;
                tempPlankTransform.position += new Vector3(-0.9f, -4.0f, 0f) * Time.unscaledDeltaTime;
                tempPlankTransform.Rotate(0f, 0f, 55f * Time.unscaledDeltaTime);
                yield return null;
            }
            if (tempPlankTransform != null)
            {
                tempPlankTransform.rotation = Quaternion.identity;
                tempPlankTransform.gameObject.SetActive(false);
            }
        }

        private void SkipNow()
        {
            if (!_isPlaying) return;
            StopPullingAnim();
            if (_routine != null) StopCoroutine(_routine);
            FinishCutscene(true);
        }

        private void FinishCutscene(bool skipped)
        {
            _isPlaying = false;
            StopPullingAnim();
            if (_wenAnim != null) _wenAnim.EndCutsceneOverride();
            if (tempPlankTransform != null)
            {
                tempPlankTransform.rotation = Quaternion.identity;
                tempPlankTransform.position = _plankStartPos;
                tempPlankTransform.gameObject.SetActive(false);
            }
            HideUIImmediate();
            if (_cam != null) _cam.EndCutsceneOverride(skipped);

            GameRoot.Instance?.Flags?.SetFlag("flag_ch2_entered_riverbank", true);
            GameRoot.Instance?.Flags?.SetFlag("flag_ch2_solo_bridge_failed", true);
            GameRoot.Instance?.State?.ReturnToGameplay();
            GameEventBus.RaiseToastRequested("Cầu đã gãy! Hãy hỏi Ông Kael ở Xưởng Gỗ hoặc dùng Đò Ngang phía Nam.");
        }

        private void SetLine(string speaker, string body)
        {
            if (speakerText != null) speakerText.text = speaker;
            if (subtitleText != null) subtitleText.text = body;
            string voice = speaker.Contains("Lumi") ? "lumi" : "wen";
            GameRoot.Instance?.Audio?.PlayVoiceBlip(voice);
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
