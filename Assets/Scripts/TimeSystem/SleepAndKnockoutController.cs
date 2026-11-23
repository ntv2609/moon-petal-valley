using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.TimeSystem
{
    public class SleepAndKnockoutController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private TMP_Text transitionTitleText;
        [SerializeField] private TMP_Text transitionText;

        public static event Action OnPassOutMaterialPenalty;

        private bool _isTransitioning;

        private void OnEnable()
        {
            TimeOfDayManager.OnLateNightPassOut += HandleLateNightPassOut;
            PlayerStatsController.OnPlayerKnockedOut += HandleStatKnockout;
        }

        private void OnDisable()
        {
            TimeOfDayManager.OnLateNightPassOut -= HandleLateNightPassOut;
            PlayerStatsController.OnPlayerKnockedOut -= HandleStatKnockout;
        }

        private void Start()
        {
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }

        private void HandleLateNightPassOut()
        {
            TriggerKnockout("Đã 02:00 đêm! Bạn thiếp đi vì quá mệt mỏi...\nSáng hôm sau trần Thể lực giảm còn 50% (ăn Bánh Mì Nắng Ấm để hồi phục).");
        }

        private void HandleStatKnockout(string reason)
        {
            TriggerKnockout(reason + "\nSáng hôm sau trần Thể lực giảm còn 50% (ăn Bánh Mì Nắng Ấm để hồi phục).");
        }

        public void TriggerNormalSleep()
        {
            if (_isTransitioning) return;
            bool inCh2 = SceneManager.GetActiveScene().name == "Riverbank_Chapter2";
            string title = inCh2 ? "NGHỈ NGƠI TẠI LỀU TẠM BỜ ĐÔNG" : "NGHỈ NGƠI TRONG NHÀ KÍNH";
            string msg = inCh2
                ? "Bạn nghỉ ngơi yên bình bên tiếng sông rì rào...\nThể lực hồi đầy 100%, đang đón ánh bình minh ngày mới."
                : "Bạn nghỉ ngơi yên bình dưới ánh trăng...\nĐang đón ánh bình minh ngày mới.";
            StartCoroutine(DayTransitionRoutine(false, title, msg, false));
        }

        public void TriggerRelaySoftFail()
        {
            if (_isTransitioning) return;
            StartCoroutine(DayTransitionRoutine(false, "ĐÊM CHUYỀN ĐÈN TẠM DỪNG", "Sương sông quá dày, Ông Kael và Bà Mara dìu bạn vào lều nghỉ ngơi.\nKhông mất vật phẩm — tối mai có thể thử lại ngay!", true));
        }

        public void TriggerKnockout(string message)
        {
            if (_isTransitioning) return;
            if (Chapter2StateManager.Instance != null && Chapter2StateManager.Instance.IsRelayEventActive)
            {
                TriggerRelaySoftFail();
                return;
            }
            string activeScene = SceneManager.GetActiveScene().name;
            string finalMsg = activeScene == "Riverbank_Chapter2"
                ? "Ông Kael và Bác Otis tìm thấy bạn ngoài Bờ Sông và đưa về Lều Tạm Bờ Đông nghỉ ngơi...\nSáng hôm sau trần Thể lực giảm còn 50% (vật phẩm nhiệm vụ an toàn 100%)."
                : message;
            StartCoroutine(DayTransitionRoutine(true, "BẠN ĐÃ KIỆT SỨC...", finalMsg, false));
        }

        private IEnumerator DayTransitionRoutine(bool passedOut, string title, string message, bool isRelaySoftFail)
        {
            _isTransitioning = true;

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.SetState(GameState.DayTransition);

            if (transitionTitleText != null)
            {
                transitionTitleText.text = title;
                transitionTitleText.color = passedOut
                    ? new Color32(255, 168, 112, 255)
                    : new Color32(252, 224, 146, 255);
            }
            if (transitionText != null) transitionText.text = message;
            if (fadeCanvasGroup != null) fadeCanvasGroup.blocksRaycasts = true;

            float t = 0f;
            while (t < 0.45f)
            {
                t += Time.unscaledDeltaTime;
                if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = Mathf.Clamp01(t / 0.45f);
                yield return null;
            }

            if (passedOut && !isRelaySoftFail)
                OnPassOutMaterialPenalty?.Invoke();

            if (isRelaySoftFail && Chapter2StateManager.Instance != null)
                Chapter2StateManager.Instance.SetRelayEventActive(false);

            if (GameRoot.Instance != null)
            {
                if (GameRoot.Instance.Time != null) GameRoot.Instance.Time.AdvanceToNextDay();
                if (GameRoot.Instance.Stats != null) GameRoot.Instance.Stats.ResetForNewDay(passedOut && !isRelaySoftFail);
            }

            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "Riverbank_Chapter2")
            {
                if (WorldSpawnRegistry.Instance != null)
                    WorldSpawnRegistry.Instance.TeleportPlayerTo("Spawn_EastCamp");
            }
            else if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo("Spawn_Greenhouse");
            }

            yield return new WaitForSecondsRealtime(1.1f);

            t = 0f;
            while (t < 0.45f)
            {
                t += Time.unscaledDeltaTime;
                if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(t / 0.45f);
                yield return null;
            }

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
                GameRoot.Instance.State.ReturnToGameplay();

            string toast = isRelaySoftFail
                ? "Sáng hôm sau tại Lều Tạm Bờ Đông. Tối nay bạn có thể bắt đầu lại Đêm Chuyền Đèn!"
                : (passedOut
                    ? "Tỉnh dậy sau khi kiệt sức! Trần Thể lực hôm nay còn 50%."
                    : "Chào buổi sáng! Thể lực và Máu Ánh Sáng đã hồi đầy 100%.");
            GameEventBus.RaiseToastRequested(toast);

            _isTransitioning = false;
        }
    }
}
