using System;
using System.Collections;
using UnityEngine;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.TimeSystem
{
    public class SleepAndKnockoutController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeCanvasGroup;
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
            TriggerKnockout("Đã 02:00 đêm! Bạn thiếp đi vì quá mệt mỏi...\n(Sáng hôm sau trần Thể lực giảm còn 50% trừ khi ăn Bánh Mì Nắng Ấm)");
        }

        private void HandleStatKnockout(string reason)
        {
            TriggerKnockout(reason + "\n(Sáng hôm sau trần Thể lực giảm còn 50% trừ khi ăn Bánh Mì Nắng Ấm)");
        }

        public void TriggerNormalSleep()
        {
            if (_isTransitioning) return;
            StartCoroutine(DayTransitionRoutine(false, "Bạn nghỉ ngơi yên bình trong Nhà Kính...\nĐang đón ánh bình minh ngày mới."));
        }

        public void TriggerKnockout(string message)
        {
            if (_isTransitioning) return;
            StartCoroutine(DayTransitionRoutine(true, message));
        }

        private IEnumerator DayTransitionRoutine(bool passedOut, string message)
        {
            _isTransitioning = true;

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.DayTransition);
            }

            if (transitionText != null) transitionText.text = message;
            if (fadeCanvasGroup != null) fadeCanvasGroup.blocksRaycasts = true;

            // Fade to black
            float t = 0f;
            while (t < 0.45f)
            {
                t += Time.unscaledDeltaTime;
                if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = Mathf.Clamp01(t / 0.45f);
                yield return null;
            }

            // Qua ngày mới, hồi stats hoặc áp phạt kiệt sức, đưa Wen về Nhà Kính
            if (passedOut)
            {
                OnPassOutMaterialPenalty?.Invoke();
            }

            if (GameRoot.Instance != null)
            {
                if (GameRoot.Instance.Time != null) GameRoot.Instance.Time.AdvanceToNextDay();
                if (GameRoot.Instance.Stats != null) GameRoot.Instance.Stats.ResetForNewDay(passedOut);
            }

            if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo("Spawn_Greenhouse");
            }

            yield return new WaitForSecondsRealtime(1.1f);

            // Fade back in
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
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }

            string toast = passedOut
                ? "Tỉnh dậy sau khi kiệt sức! Trần Thể lực hôm nay còn 50%."
                : "Chào buổi sáng! Thể lực và Máu Ánh Sáng đã hồi đầy.";
            GameEventBus.RaiseToastRequested(toast);

            _isTransitioning = false;
        }
    }
}
