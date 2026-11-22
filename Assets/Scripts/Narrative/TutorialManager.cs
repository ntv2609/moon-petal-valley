using System.Collections;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.Narrative
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [SerializeField] private TutorialHintHUDView hintView;
        [SerializeField] private float idleReminderDelay = 22f;

        private int _currentStep = -1;
        private float _idleTimer;
        private bool _remindedCurrentStep;
        private Vector3 _lastPlayerPos;
        private bool _hasLastPos;
        private PlayerController2D _player;
        private Coroutine _transitionRoutine;

        public const string FLAG_SKIPPED = "flag_tutorial_skipped";
        public const string FLAG_FINISHED = "flag_guided_tutorial_finished";

        public bool IsTutorialEnabled => !HasFlag(FLAG_SKIPPED) && !HasFlag(FLAG_FINISHED);

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnGameStateChanged += HandleGameStateChanged;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnGameStateChanged -= HandleGameStateChanged;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded;
        }

        private void Start()
        {
            if (hintView == null)
                hintView = UnityEngine.Object.FindAnyObjectByType<TutorialHintHUDView>(FindObjectsInactive.Include);
            RefreshStep(false);
        }

        private void HandleSaveLoaded()
        {
            RefreshStep(false);
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            bool gameplayActive = GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive || !IsTutorialEnabled || _currentStep < 0)
            {
                if (hintView != null) hintView.SetVisible(false);
                return;
            }
            if (hintView != null && !hintView.IsShowingCompletion)
                hintView.SetVisible(true);
            TrackIdleReminder();
        }

        private void TrackIdleReminder()
        {
            if (_remindedCurrentStep || _currentStep < 0) return;
            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player == null) return;
                _lastPlayerPos = _player.transform.position;
                _hasLastPos = true;
            }

            float moved = _hasLastPos ? Vector3.Distance(_player.transform.position, _lastPlayerPos) : 0f;
            if (moved > 0.35f)
            {
                _lastPlayerPos = _player.transform.position;
                _idleTimer = Mathf.Max(0f, _idleTimer - Time.deltaTime * 0.5f);
            }
            else
            {
                _idleTimer += Time.deltaTime;
            }

            if (_idleTimer >= idleReminderDelay)
            {
                _remindedCurrentStep = true;
                string reminder = GetIdleReminder(_currentStep);
                if (!string.IsNullOrEmpty(reminder))
                    GameEventBus.RaiseToastRequested(reminder);
            }
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (flagId == FLAG_SKIPPED || flagId == FLAG_FINISHED)
            {
                RefreshStep(false);
                return;
            }
            RefreshStep(true);
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (hintView == null) return;
            if (state != GameState.Gameplay) hintView.SetVisible(false);
            else RefreshStep(false);
        }

        public void SetTutorialSkipped(bool skip)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            GameRoot.Instance.Flags.SetFlag(FLAG_SKIPPED, skip);
            if (!skip && HasFlag(FLAG_FINISHED))
            {
                int next = ComputeActiveStep();
                if (next >= 0) GameRoot.Instance.Flags.SetFlag(FLAG_FINISHED, false);
            }
            GameEventBus.RaiseToastRequested(skip
                ? "Đã ẩn gợi ý hướng dẫn. Bạn có thể bật lại trong Pause Menu [Esc]."
                : "Đã bật lại gợi ý hướng dẫn.");
            RefreshStep(false);
        }

        public void ToggleTutorialEnabled() => SetTutorialSkipped(IsTutorialEnabled);

        public void RefreshStep(bool celebratePrevious)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            if (HasFlag(FLAG_SKIPPED))
            {
                _currentStep = -1;
                if (hintView != null) hintView.SetVisible(false);
                return;
            }

            int nextStep = ComputeActiveStep();
            if (nextStep < 0)
            {
                bool wasActive = _currentStep >= 0;
                _currentStep = -1;
                if (!HasFlag(FLAG_FINISHED))
                    GameRoot.Instance.Flags.SetFlag(FLAG_FINISHED, true);
                if (celebratePrevious && wasActive && hintView != null)
                    StartStepTransition("Đã nắm vững thao tác cơ bản!", -1);
                else if (hintView != null)
                    hintView.SetVisible(false);
                return;
            }

            bool stepAdvanced = celebratePrevious && _currentStep >= 0 && nextStep > _currentStep;
            _currentStep = nextStep;
            _idleTimer = 0f;
            _remindedCurrentStep = false;

            if (stepAdvanced && hintView != null)
                StartStepTransition("Xong rồi!", nextStep);
            else if (hintView != null)
                ApplyStepToView(nextStep);
        }

        private void StartStepTransition(string completeLabel, int nextStep)
        {
            if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);
            _transitionRoutine = StartCoroutine(TransitionRoutine(completeLabel, nextStep));
        }

        private IEnumerator TransitionRoutine(string completeLabel, int nextStep)
        {
            hintView.ShowCompletedFlash(completeLabel);
            yield return new WaitForSecondsRealtime(1.05f);
            if (nextStep >= 0 && IsTutorialEnabled) ApplyStepToView(nextStep);
            else hintView.SetVisible(false);
            _transitionRoutine = null;
        }

        private void ApplyStepToView(int step)
        {
            if (hintView == null) return;
            hintView.SetHint(GetStepHint(step));
            bool canShow = GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsGameplayActive;
            hintView.SetVisible(canShow);
        }

        private int ComputeActiveStep()
        {
            if (!HasFlag("flag_intro_letter_read")) return 1;
            if (!HasFlag("flag_greenhouse_cleared")) return 2;
            if (!HasFlag("flag_lumi_awakened")) return 3;
            if (!HasFlag("flag_tutorial_done")) return 4;
            if (!HasFlag("flag_met_bach_at_gate")) return 5;
            if (!HasFlag("flag_garden_slimes_cleared")) return 6;
            return -1;
        }

        private string GetStepHint(int step)
        {
            switch (step)
            {
                case 1: return "Di chuyển <b><color=#8B3A1E>WASD</color></b> tới Bàn Gỗ, bấm <b><color=#8B3A1E>E</color></b> đọc thư của bà";
                case 2: return "Chọn Cuốc <b><color=#8B3A1E>1</color></b> hoặc Rìu <b><color=#8B3A1E>3</color></b>, bấm <b><color=#8B3A1E>Chuột trái</color></b> dọn cỏ đá";
                case 3: return "Tới gần Hộp Gỗ Của Bà, bấm <b><color=#8B3A1E>E</color></b> mở hộp";
                case 4: return "Bấm <b><color=#8B3A1E>E</color></b> tại Bồn Ươm Nhỏ để gieo thử hạt";
                case 5: return "Ra cửa dưới Nhà Kính, gặp Ông Bach ở cổng Nam <b><color=#8B3A1E>E</color></b>";
                case 6: return "Cầm Liềm <b><color=#8B3A1E>4</color></b> chém Slime, bấm <b><color=#8B3A1E>Space</color></b> lướt né";
                default: return "";
            }
        }

        private string GetIdleReminder(int step)
        {
            switch (step)
            {
                case 1: return "Wen: Bức thư của bà đang đặt ngay trên bàn gỗ phía trên kia...";
                case 2: return "Wen: Mình cần chọn Cuốc [1] hoặc Rìu [3] rồi bấm chuột trái để dọn cỏ đá.";
                case 3: return "???: Khịt khịt... Mở nắp hộp gỗ ra giúp tớ với, ở trong này bí quá!";
                case 4: return "Lumi: Bồn ươm nhỏ nằm ngay bên phải trong Nhà Kính đó Wen, bấm [E] thử xem!";
                case 5: return "Lumi: Lối ra Làng Chính ở phía dưới Nhà Kính. Ông Bach đang đứng ở cổng Vườn Phía Nam!";
                case 6: return "Lumi: Cầm Liềm [4] lên! Khi Bọt Sương Đen lao tới nhớ bấm [Space] lướt né nhé!";
                default: return "";
            }
        }

        private bool HasFlag(string flagId)
        {
            return GameRoot.Instance != null && GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag(flagId);
        }
    }
}
