using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Player
{
    public class PlayerStatsController : MonoBehaviour, ISaveable, IResettable
    {
        [SerializeField] private PlayerStatsSO statsConfig;

        public static event Action<string> OnPlayerKnockedOut;
        public static event Action<bool> OnExhaustionStateChanged;

        public int CurrentStamina { get; private set; } = 100;
        public int CurrentLightHP { get; private set; } = 100;
        public bool IsExhaustedPenalty { get; private set; }

        public int BaseMaxStamina => statsConfig != null ? statsConfig.maxStamina : 100;
        public int MaxLightHP => statsConfig != null ? statsConfig.maxLightHP : 100;
        public int EffectiveMaxStamina
        {
            get
            {
                if (!IsExhaustedPenalty) return BaseMaxStamina;
                float mult = statsConfig != null ? statsConfig.exhaustedStaminaMultiplier : 0.5f;
                return Mathf.Max(10, Mathf.RoundToInt(BaseMaxStamina * mult));
            }
        }

        public string SaveKey => "player_stats";

        [Serializable]
        private class StatsSaveData
        {
            public int stamina;
            public int lightHP;
            public bool exhausted;
        }

        private void Start()
        {
            CurrentStamina = EffectiveMaxStamina;
            CurrentLightHP = MaxLightHP;
            BroadcastAll();
        }

        public bool ConsumeStamina(int amount, bool bypassTutorialFloor = false)
        {
            if (amount <= 0) return true;

            bool inTutorial = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                              !GameRoot.Instance.Flags.HasFlag("flag_tutorial_done");

            int next = CurrentStamina - amount;
            if (inTutorial && !bypassTutorialFloor && next < 10)
            {
                next = 10;
            }

            CurrentStamina = Mathf.Clamp(next, 0, EffectiveMaxStamina);
            GameEventBus.RaiseStaminaChanged(CurrentStamina, EffectiveMaxStamina);

            if (CurrentStamina <= 0)
            {
                OnPlayerKnockedOut?.Invoke("Thể lực cạn kiệt! Bạn ngất xỉu và được dân làng đưa về Nhà Kính...");
                return false;
            }
            return true;
        }

        public void TakeLightDamage(int amount, bool ignoreInvulnerability = false)
        {
            if (amount <= 0) return;

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (!ignoreInvulnerability && player != null && player.IsInvulnerable)
            {
                return;
            }

            CurrentLightHP = Mathf.Clamp(CurrentLightHP - amount, 0, MaxLightHP);
            GameEventBus.RaiseLightHPChanged(CurrentLightHP, MaxLightHP);

            if (player != null && !ignoreInvulnerability)
            {
                float iFrame = statsConfig != null ? statsConfig.hitInvulnerabilityDuration : 0.75f;
                player.GrantInvulnerability(iFrame);
            }

            if (CurrentLightHP <= 0)
            {
                OnPlayerKnockedOut?.Invoke("Máu Ánh Sáng cạn kiệt! Lumi dìu bạn trở về Nhà Kính nghỉ ngơi...");
            }
        }

        public void RestoreStats(int staminaAmount, int lightHPAmount, bool clearExhaustionPenalty)
        {
            if (clearExhaustionPenalty && IsExhaustedPenalty)
            {
                IsExhaustedPenalty = false;
                OnExhaustionStateChanged?.Invoke(false);
            }

            CurrentStamina = Mathf.Clamp(CurrentStamina + staminaAmount, 0, EffectiveMaxStamina);
            CurrentLightHP = Mathf.Clamp(CurrentLightHP + lightHPAmount, 0, MaxLightHP);
            BroadcastAll();
        }

        public void ResetForNewDay(bool passedOutYesterday)
        {
            IsExhaustedPenalty = passedOutYesterday;
            CurrentStamina = EffectiveMaxStamina;
            CurrentLightHP = MaxLightHP;
            OnExhaustionStateChanged?.Invoke(IsExhaustedPenalty);
            BroadcastAll();
        }

        public void BroadcastAll()
        {
            GameEventBus.RaiseStaminaChanged(CurrentStamina, EffectiveMaxStamina);
            GameEventBus.RaiseLightHPChanged(CurrentLightHP, MaxLightHP);
            OnExhaustionStateChanged?.Invoke(IsExhaustedPenalty);
        }

        public string CaptureStateJson()
        {
            var data = new StatsSaveData
            {
                stamina = CurrentStamina,
                lightHP = CurrentLightHP,
                exhausted = IsExhaustedPenalty
            };
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<StatsSaveData>(json);
            if (data != null)
            {
                IsExhaustedPenalty = data.exhausted;
                CurrentStamina = Mathf.Clamp(data.stamina, 1, EffectiveMaxStamina);
                CurrentLightHP = Mathf.Clamp(data.lightHP, 1, MaxLightHP);
                BroadcastAll();
            }
        }
    
        public void ResetForNewGame()
        {
            IsExhaustedPenalty = false;
            CurrentStamina = EffectiveMaxStamina;
            CurrentLightHP = MaxLightHP;
            BroadcastAll();
        }
}
}
