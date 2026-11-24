using System.Collections;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.Chapter3
{
    public class Chapter3PrepController : MonoBehaviour
    {
        public static Chapter3PrepController Instance { get; private set; }

        private bool _introHintShownThisSession;
        private bool _reversalRunning;
        private Coroutine _reversalCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEventBus.OnDayStarted += HandleDayStarted;
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnDayStarted -= HandleDayStarted;
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            EvaluatePrepEvents();
        }

        private void HandleDayStarted(int newDay)
        {
            EvaluatePrepEvents();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == Chapter3StateManager.FlagCh2Completed && value)
            {
                EvaluatePrepEvents();
            }
        }

        private void HandleSaveLoaded()
        {
            _introHintShownThisSession = false;
            EvaluatePrepEvents();
        }

        public void EvaluatePrepEvents()
        {
            var root = GameRoot.Instance;
            var ch3 = Chapter3StateManager.Instance;
            if (root == null || root.Flags == null || ch3 == null) return;

            if (!root.Flags.HasFlag(Chapter3StateManager.FlagCh2Completed)) return;
            if (root.Flags.HasFlag(Chapter3StateManager.FlagCh3Completed)) return;

            ch3.SyncFromFlagsAndDay();

            if (!root.Flags.HasFlag(Chapter3StateManager.FlagCh3Started) && !_introHintShownThisSession)
            {
                _introHintShownThisSession = true;
                GameEventBus.RaiseToastRequested("Chương 3: Gặp Linh ở quảng trường làng và soi Dư Âm lò bánh bà Hana!");
            }

            bool readyForReversal = !root.Flags.HasFlag(Chapter3StateManager.FlagCh3StallsBuiltDay3)
                && !root.Flags.HasFlag(Chapter3StateManager.FlagCh3MarketDone)
                && ch3.CurrentPrepDayNumber >= 3;

            if (readyForReversal && !_reversalRunning)
            {
                TriggerDay3ReversalNow();
            }
        }

        public void TriggerDay3ReversalNow()
        {
            if (_reversalRunning) return;
            if (_reversalCoroutine != null) StopCoroutine(_reversalCoroutine);
            _reversalCoroutine = StartCoroutine(PlayNight2ToDay3ReversalRoutine());
        }

        private IEnumerator PlayNight2ToDay3ReversalRoutine()
        {
            _reversalRunning = true;
            yield return new WaitForSecondsRealtime(0.45f);

            float waitTimeout = 8f;
            while (waitTimeout > 0f)
            {
                var root = GameRoot.Instance;
                bool busy = root == null
                    || root.Dialogue == null
                    || root.Dialogue.IsDialogueActive
                    || (EchoManager.Instance != null && EchoManager.Instance.IsPlaying)
                    || (root.State != null && root.State.CurrentState != GameState.Gameplay);
                if (!busy) break;
                waitTimeout -= Time.unscaledDeltaTime;
                yield return null;
            }

            var gr = GameRoot.Instance;
            if (gr == null || gr.Flags == null || gr.Dialogue == null)
            {
                _reversalRunning = false;
                yield break;
            }

            if (gr.Flags.HasFlag(Chapter3StateManager.FlagCh3StallsBuiltDay3))
            {
                _reversalRunning = false;
                yield break;
            }

            gr.Flags.SetFlag(Chapter3StateManager.FlagCh3WenExhaustedNight2, true);

            var lines = new (string speaker, string body)[]
            {
                ("Lumi", "Đêm qua cậu thức đóng kệ gỗ tới tận khuya rồi gục luôn bên bàn chế tác... tớ lo lắm đó Wen!"),
                ("Wen", "Ưm... sáng rồi sao? Bảy gian hàng vẫn chưa dựng xong, tối nay làm sao kịp mở Chợ Đêm..."),
                ("Bà Hana", "Ngủ thêm chút nữa đi cháu, có mùi bánh mật ong mới ra lò rồi kìa!"),
                ("Ông Kael", "Nửa đêm thấy đèn nhà cháu còn sáng, cả làng kéo nhau ra cầu từ lúc gà chưa gáy."),
                ("Bà Mara", "Ta căng dây đèn lồng, ông Kael đóng nốt bảy khung sạp gỗ, bác Otis xếp giỏ hoa."),
                ("Linh", "Em với Toby còn treo sẵn băng rôn Chợ Đêm ở giữa cầu rồi. Mọi thứ chờ mỗi chị Wen thôi!"),
                ("Ông Bach", "Thung lũng này không bao giờ để cháu gánh một mình đâu. Ra cầu xem thành quả nào!"),
                ("Wen", "Mọi người... cảm ơn cả làng nhiều lắm! Tối nay chúng ta sẽ có một Chợ Đêm thật rực rỡ!")
            };

            gr.Dialogue.StartQuickLines(lines, () =>
            {
                gr.Flags.SetFlag(Chapter3StateManager.FlagCh3StallsBuiltDay3, true);
                gr.Stats?.RestoreStats(100, 100, true);
                Chapter3StateManager.Instance?.SyncFromFlagsAndDay();
                gr.Quests?.RecomputeObjective();
                GameEventBus.RaiseToastRequested("7 gian hàng Chợ Đêm đã dựng xong trên cầu bờ sông!");
                _reversalRunning = false;
            });
        }
    }
}
