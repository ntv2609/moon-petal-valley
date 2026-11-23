using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.Chapter2
{
    public class Chapter2NPCSchedule : MonoBehaviour
    {
        [Header("Schedule Positions")]
        [SerializeField] private Vector2 dayPosition;
        [SerializeField] private Vector2 nightPosition;
        [SerializeField] private float duskStartHour = 18.0f;
        [SerializeField] private float dawnEndHour = 6.0f;

        private bool _isNightSpot;

        private void Start()
        {
            ApplyPosition(true);
        }

        private void Update()
        {
            ApplyPosition(false);
        }

        public void ForceRefreshSchedule()
        {
            ApplyPosition(true);
        }

        private bool IsLockedByEvent()
        {
            if (Chapter2StateManager.Instance != null && Chapter2StateManager.Instance.IsRelayEventActive)
                return true;
            if (LanternRelayManager.Instance != null && LanternRelayManager.Instance.IsRelayActive)
                return true;
            if (Chapter2EndingController.Instance != null && Chapter2EndingController.Instance.IsPlaying)
                return true;
            return false;
        }

        private void ApplyPosition(bool force)
        {
            if (IsLockedByEvent()) return;
            if (!force && GameRoot.Instance != null && GameRoot.Instance.State != null &&
                GameRoot.Instance.State.CurrentState != GameState.Gameplay)
                return;

            TimeOfDayManager tm = GameRoot.Instance != null
                ? GameRoot.Instance.Time
                : UnityEngine.Object.FindAnyObjectByType<TimeOfDayManager>();
            float hour = tm != null ? tm.CurrentHourFloat : 10f;
            bool shouldBeNight = hour >= duskStartHour || hour < dawnEndHour;
            if (!force && shouldBeNight == _isNightSpot) return;

            _isNightSpot = shouldBeNight;
            Vector2 target = _isNightSpot ? nightPosition : dayPosition;
            transform.position = new Vector3(target.x, target.y, transform.position.z);
        }

        public void Configure(Vector2 dayPos, Vector2 nightPos, float duskHour = 18.0f)
        {
            dayPosition = dayPos;
            nightPosition = nightPos;
            duskStartHour = duskHour;
            ApplyPosition(true);
        }
    }
}
