using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.TimeSystem
{
    public enum DayPhase
    {
        Morning,
        Noon,
        Dusk,
        Night
    }

    public class TimeOfDayManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private TimeConfigSO timeConfig;

        public static event Action OnLateNightPassOut;

        public int CurrentDay { get; private set; } = 1;
        public float CurrentHourFloat { get; private set; } = 6f;

        private int _lastBroadcastTenMinBucket = -1;
        private bool _passOutTriggeredToday;

        public string SaveKey => "time_of_day";

        public DayPhase CurrentPhase
        {
            get
            {
                float dusk = timeConfig != null ? timeConfig.duskStartHour : 18f;
                float night = timeConfig != null ? timeConfig.nightStartHour : 20f;
                if (CurrentHourFloat < 12f) return DayPhase.Morning;
                if (CurrentHourFloat < dusk) return DayPhase.Noon;
                if (CurrentHourFloat < night) return DayPhase.Dusk;
                return DayPhase.Night;
            }
        }

        public bool IsDuskOrNight => CurrentPhase == DayPhase.Dusk || CurrentPhase == DayPhase.Night;
        public bool IsNight => CurrentPhase == DayPhase.Night;

        [Serializable]
        private class TimeSaveData
        {
            public int day;
            public float hourFloat;
        }

        private void Start()
        {
            CurrentHourFloat = timeConfig != null ? timeConfig.dayStartHour : 6f;
            BroadcastTimeImmediate();
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsTimePaused)
            {
                return;
            }

            float dayStart = timeConfig != null ? timeConfig.dayStartHour : 6f;
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            float realSec = timeConfig != null && timeConfig.realSecondsPerGameDay > 1f
                ? timeConfig.realSecondsPerGameDay
                : 300f;

            float gameHoursPerRealSecond = (passOut - dayStart) / realSec;
            CurrentHourFloat += Time.deltaTime * gameHoursPerRealSecond;

            int bucket = Mathf.FloorToInt(CurrentHourFloat * 6f);
            if (bucket != _lastBroadcastTenMinBucket)
            {
                _lastBroadcastTenMinBucket = bucket;
                GameEventBus.RaiseTimeChanged(CurrentDay, CurrentHourFloat);
            }

            if (CurrentHourFloat >= passOut && !_passOutTriggeredToday)
            {
                _passOutTriggeredToday = true;
                if (OnLateNightPassOut != null)
                {
                    OnLateNightPassOut.Invoke();
                }
                else
                {
                    GameEventBus.RaiseToastRequested("Đã 02:00 đêm! Bạn kiệt sức và thiếp đi tới sáng hôm sau...");
                    AdvanceToNextDay();
                }
            }
        }

        public void AddHours(float deltaHours)
        {
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            CurrentHourFloat = Mathf.Clamp(CurrentHourFloat + deltaHours, 6f, passOut);
            BroadcastTimeImmediate();
        }

        public void SetHour(float targetHour)
        {
            float dayStart = timeConfig != null ? timeConfig.dayStartHour : 6f;
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            CurrentHourFloat = Mathf.Clamp(targetHour, dayStart, passOut);
            _passOutTriggeredToday = false;
            BroadcastTimeImmediate();
        }

        public void AdvanceToNextDay()
        {
            CurrentDay++;
            CurrentHourFloat = timeConfig != null ? timeConfig.dayStartHour : 6f;
            _passOutTriggeredToday = false;
            GameEventBus.RaiseDayStarted(CurrentDay);
            BroadcastTimeImmediate();
        }

        public string GetFormattedClockString()
        {
            int totalMinutes = Mathf.FloorToInt(CurrentHourFloat * 60f);
            int displayHour = (totalMinutes / 60) % 24;
            int displayMin = (totalMinutes % 60 / 10) * 10;

            string phaseText = CurrentPhase switch
            {
                DayPhase.Morning => "Sáng",
                DayPhase.Noon => "Trưa",
                DayPhase.Dusk => "Chạng vạng",
                _ => "Đêm"
            };

            return string.Format("Ngày {0} — {1:00}:{2:00} ({3})", CurrentDay, displayHour, displayMin, phaseText);
        }

        private void BroadcastTimeImmediate()
        {
            _lastBroadcastTenMinBucket = Mathf.FloorToInt(CurrentHourFloat * 6f);
            GameEventBus.RaiseTimeChanged(CurrentDay, CurrentHourFloat);
        }

        public string CaptureStateJson()
        {
            var data = new TimeSaveData { day = CurrentDay, hourFloat = CurrentHourFloat };
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<TimeSaveData>(json);
            if (data != null)
            {
                CurrentDay = Mathf.Max(1, data.day);
                CurrentHourFloat = Mathf.Clamp(data.hourFloat, 6f, 25.9f);
                _passOutTriggeredToday = false;
                BroadcastTimeImmediate();
            }
        }
    }
}
