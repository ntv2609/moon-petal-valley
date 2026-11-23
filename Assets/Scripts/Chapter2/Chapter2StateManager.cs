using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2
{
    public enum RiverBridgeState
    {
        Broken = 0,
        RelayInProgress = 1,
        Repaired = 2
    }

    public class Chapter2StateManager : MonoBehaviour, ISaveable, IResettable
    {
        public static Chapter2StateManager Instance { get; private set; }

        public RiverBridgeState BridgeState { get; private set; } = RiverBridgeState.Broken;
        public int InvestigationStage { get; private set; }
        public bool IsRelayEventActive { get; private set; }
        public string PendingSceneSpawnId { get; set; } = "";

        private readonly HashSet<string> _completedEchoes = new HashSet<string>();

        public event Action<RiverBridgeState> OnBridgeStateChanged;

        public string SaveKey => "chapter2_state";

        [Serializable]
        private class Chapter2SaveData
        {
            public int bridgeState;
            public int investigationStage;
            public List<string> completedEchoes = new List<string>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (GameRoot.Instance != null)
            {
                GameRoot.Instance.RegisterSaveable(this);
            }
            SyncFromFlags();
        }

        private void OnEnable()
        {
            GameEventBus.OnSaveLoaded += SyncFromFlags;
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnSaveLoaded -= SyncFromFlags;
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void OnDestroy()
        {
            if (GameRoot.Instance != null) GameRoot.Instance.UnregisterSaveable(this);
            if (Instance == this) Instance = null;
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            if (flagId == "flag_ch2_completed")
            {
                SetBridgeState(RiverBridgeState.Repaired);
            }
        }

        private void SyncFromFlags()
        {
            var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags == null) return;
            if (flags.HasFlag("flag_ch2_completed"))
            {
                SetBridgeState(RiverBridgeState.Repaired);
            }
        }

        public void SetBridgeState(RiverBridgeState next)
        {
            if (BridgeState == next) return;
            BridgeState = next;
            OnBridgeStateChanged?.Invoke(BridgeState);
        }

        public void SetInvestigationStage(int stage)
        {
            InvestigationStage = Mathf.Clamp(stage, 0, 3);
        }

        public void SetRelayEventActive(bool active)
        {
            IsRelayEventActive = active;
            if (active && BridgeState == RiverBridgeState.Broken)
            {
                SetBridgeState(RiverBridgeState.RelayInProgress);
            }
            else if (!active && BridgeState == RiverBridgeState.RelayInProgress)
            {
                bool done = GameRoot.Instance?.Flags != null && GameRoot.Instance.Flags.HasFlag("flag_ch2_completed");
                SetBridgeState(done ? RiverBridgeState.Repaired : RiverBridgeState.Broken);
            }
        }

        public bool IsEchoCompleted(string echoId)
        {
            return !string.IsNullOrEmpty(echoId) && _completedEchoes.Contains(echoId);
        }

        public void MarkEchoCompleted(string echoId)
        {
            if (!string.IsNullOrEmpty(echoId))
            {
                _completedEchoes.Add(echoId);
            }
        }

        public string CaptureStateJson()
        {
            var data = new Chapter2SaveData
            {
                bridgeState = (int)BridgeState,
                investigationStage = InvestigationStage,
                completedEchoes = new List<string>(_completedEchoes)
            };
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            _completedEchoes.Clear();
            IsRelayEventActive = false;
            PendingSceneSpawnId = "";
            if (string.IsNullOrEmpty(json))
            {
                BridgeState = RiverBridgeState.Broken;
                InvestigationStage = 0;
                return;
            }
            var data = JsonUtility.FromJson<Chapter2SaveData>(json);
            if (data != null)
            {
                BridgeState = (RiverBridgeState)Mathf.Clamp(data.bridgeState, 0, 2);
                InvestigationStage = Mathf.Clamp(data.investigationStage, 0, 3);
                if (data.completedEchoes != null)
                {
                    for (int i = 0; i < data.completedEchoes.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(data.completedEchoes[i]))
                            _completedEchoes.Add(data.completedEchoes[i]);
                    }
                }
            }
            OnBridgeStateChanged?.Invoke(BridgeState);
        }

        public void ResetForNewGame()
        {
            _completedEchoes.Clear();
            BridgeState = RiverBridgeState.Broken;
            InvestigationStage = 0;
            IsRelayEventActive = false;
            PendingSceneSpawnId = "";
            OnBridgeStateChanged?.Invoke(BridgeState);
        }
    }
}
