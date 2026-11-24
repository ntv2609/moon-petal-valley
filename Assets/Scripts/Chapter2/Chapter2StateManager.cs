using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.Core;
using MoonpetalValley.Narrative;

namespace MoonpetalValley.Chapter2
{
    public enum RiverBridgeState
    {
        Broken = 0,
        RelayInProgress = 1,
        Repaired = 2
    }

    public enum Chapter2Stage
    {
        Arrival = 0,
        MeetKael = 1,
        EastClueNight = 2,
        CrossToMaraAndWestClue = 3,
        OtisAndStormLantern = 4,
        WestSwampAndEcho1 = 5,
        BrokenPierEcho2 = 6,
        LanternRelayNight = 7,
        Completed = 8
    }

    public class Chapter2StateManager : MonoBehaviour, ISaveable, IResettable
    {
        public const string MainVillageSceneName = "MoonpetalValley_MVP";
        public const string Chapter2SceneName = "Riverbank_Chapter2";
        public const string WestSwampSceneName = "WestSwamp_Chapter2";

        public const string FlagSoloBridgeFailed = "flag_ch2_solo_bridge_failed";
        public const string FlagMetKael = "flag_ch2_met_kael";
        public const string FlagClue1Done = "flag_ch2_inv_night1_done";
        public const string FlagMetMara = "flag_ch2_met_mara";
        public const string FlagClue2Done = "flag_ch2_inv_night2_done";
        public const string FlagOtisMovedToCh2 = "flag_ch2_otis_arrived";
        public const string FlagClue3Done = "flag_ch2_inv_night3_done";
        public const string FlagKaelGaveLantern = "flag_ch2_kael_gave_lantern";
        public const string FlagSwampUnlocked = "flag_ch2_swamp_unlocked";
        public const string FlagSwampCleared = "flag_ch2_swamp_cleared";
        public const string FlagEcho1Done = "flag_ch2_echo1_lantern_done";
        public const string FlagEcho2Done = "flag_ch2_echo2_pier_done";
        public const string FlagRelayAgreed = "flag_ch2_relay_agreed";
        public const string FlagRelayDone = "flag_ch2_relay_done";
        public const string FlagCh2Completed = "flag_ch2_completed";

        public const string Echo1Id = "echo_ch2_storm_lantern";
        public const string Echo2Id = "echo_ch2_broken_pier";

        public static Chapter2StateManager Instance { get; private set; }

        public RiverBridgeState BridgeState { get; private set; } = RiverBridgeState.Broken;
        public int InvestigationStage { get; private set; }
        public bool IsRelayEventActive { get; private set; }
        public string PendingSceneSpawnId { get; set; } = "";

        private readonly HashSet<string> _completedEchoes = new HashSet<string>();
        private Chapter2Stage _lastKnownStage = Chapter2Stage.Arrival;

        public event Action<RiverBridgeState> OnBridgeStateChanged;
        public event Action<Chapter2Stage> OnStageChanged;

        public string SaveKey => "chapter2_state";

        public Chapter2Stage CurrentStage => ComputeStageFromFlags();

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
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnSaveLoaded -= SyncFromFlags;
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (GameRoot.Instance != null) GameRoot.Instance.UnregisterSaveable(this);
            if (Instance == this) Instance = null;
        }

        private static bool HasFlag(string flagId)
        {
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            return flags != null && flags.HasFlag(flagId);
        }

        public Chapter2Stage ComputeStageFromFlags()
        {
            if (HasFlag(FlagCh2Completed)) return Chapter2Stage.Completed;
            if (HasFlag(FlagEcho2Done)) return Chapter2Stage.LanternRelayNight;
            if (HasFlag(FlagEcho1Done)) return Chapter2Stage.BrokenPierEcho2;
            if (HasFlag(FlagSwampUnlocked) || HasFlag(FlagSwampCleared)) return Chapter2Stage.WestSwampAndEcho1;
            if (HasFlag(FlagClue2Done)) return Chapter2Stage.OtisAndStormLantern;
            if (HasFlag(FlagClue1Done)) return Chapter2Stage.CrossToMaraAndWestClue;
            if (HasFlag(FlagMetKael)) return Chapter2Stage.EastClueNight;
            if (HasFlag(FlagSoloBridgeFailed)) return Chapter2Stage.MeetKael;
            return Chapter2Stage.Arrival;
        }

        public bool IsFerryUnlocked()
        {
            return HasFlag(FlagClue1Done) || HasFlag(FlagCh2Completed);
        }

        public bool IsWaitBenchUnlocked()
        {
            return HasFlag(FlagMetKael) || HasFlag(FlagCh2Completed);
        }

        public bool IsClueUnlocked(int clueStage)
        {
            if (clueStage == 1)
                return HasFlag(FlagMetKael) && !HasFlag(FlagClue1Done);
            if (clueStage == 2)
                return HasFlag(FlagClue1Done) && HasFlag(FlagMetMara) && !HasFlag(FlagClue2Done);
            if (clueStage == 3)
                return HasFlag(FlagClue2Done) && !HasFlag(FlagClue3Done);
            return false;
        }

        public bool IsSwampUnlocked()
        {
            return HasFlag(FlagSwampUnlocked) || HasFlag(FlagSwampCleared) ||
                   HasFlag(FlagEcho1Done) || HasFlag(FlagCh2Completed);
        }

        public bool IsEchoUnlocked(string echoId)
        {
            if (string.IsNullOrEmpty(echoId)) return true;
            if (echoId == Echo1Id)
                return HasFlag(FlagSwampCleared) || HasFlag(FlagEcho1Done) || HasFlag(FlagCh2Completed);
            if (echoId == Echo2Id)
                return HasFlag(FlagEcho1Done) || HasFlag(FlagEcho2Done) || HasFlag(FlagCh2Completed);
            return true;
        }

        public bool IsRelayStartUnlocked()
        {
            if (HasFlag(FlagRelayDone) || HasFlag(FlagCh2Completed)) return false;
            return HasFlag(FlagEcho2Done) && HasFlag(FlagRelayAgreed);
        }

        public bool HasOtisMovedToChapter2()
        {
            return HasFlag(FlagOtisMovedToCh2) || HasFlag(FlagClue3Done);
        }

        public void MarkOtisMovedToChapter2()
        {
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags != null && !flags.HasFlag(FlagOtisMovedToCh2))
            {
                flags.SetFlag(FlagOtisMovedToCh2, true);
            }
            SyncOtisPresenceInActiveScene();
        }

        public void SyncOtisPresenceInActiveScene()
        {
            string activeScene = SceneManager.GetActiveScene().name;
            bool otisMoved = HasOtisMovedToChapter2();
            bool ch2Done = HasFlag(FlagCh2Completed);

            if (activeScene != MainVillageSceneName && activeScene != Chapter2SceneName)
                return;

            NPCController[] npcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            for (int i = 0; i < npcs.Length; i++)
            {
                NPCController npc = npcs[i];
                if (npc == null) continue;

                string objName = npc.gameObject.name;
                string npcId = npc.Data != null ? npc.Data.npcID : "";

                if (activeScene == MainVillageSceneName)
                {
                    if (objName == "NPC_Otis" || npcId == "npc_otis")
                    {
                        bool shouldShow = !otisMoved || ch2Done;
                        if (npc.gameObject.activeSelf != shouldShow)
                        {
                            npc.gameObject.SetActive(shouldShow);
                        }
                    }
                }
                else if (activeScene == Chapter2SceneName)
                {
                    if (objName == "NPC_Otis_Ch2" || npcId == "npc_otis_ch2")
                    {
                        bool shouldShow = otisMoved && !ch2Done;
                        if (npc.gameObject.activeSelf != shouldShow)
                        {
                            npc.gameObject.SetActive(shouldShow);
                        }
                    }
                }
            }
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SyncOtisPresenceInActiveScene();
            StartCoroutine(SyncOtisNextFrame());

            if (!string.IsNullOrEmpty(PendingSceneSpawnId))
            {
                string spawnId = PendingSceneSpawnId;
                PendingSceneSpawnId = "";
                StartCoroutine(TeleportAfterSceneReady(spawnId));
            }
        }

        private IEnumerator SyncOtisNextFrame()
        {
            yield return null;
            SyncOtisPresenceInActiveScene();
        }

        private IEnumerator TeleportAfterSceneReady(string spawnId)
        {
            yield return null;
            if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo(spawnId);
            }
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == FlagOtisMovedToCh2 || flagId == FlagClue3Done || flagId == FlagCh2Completed)
            {
                SyncOtisPresenceInActiveScene();
            }

            if (!string.IsNullOrEmpty(flagId) && flagId.StartsWith("flag_ch2_"))
            {
                NotifyStageIfChanged();
            }

            if (!value) return;
            if (flagId == FlagCh2Completed)
            {
                SetBridgeState(RiverBridgeState.Repaired);
            }
        }

        private void NotifyStageIfChanged()
        {
            Chapter2Stage current = ComputeStageFromFlags();
            if (current != _lastKnownStage)
            {
                _lastKnownStage = current;
                OnStageChanged?.Invoke(current);
            }
        }

        private void SyncFromFlags()
        {
            if (HasFlag(FlagCh2Completed))
            {
                SetBridgeState(RiverBridgeState.Repaired);
            }
            _lastKnownStage = ComputeStageFromFlags();
            OnStageChanged?.Invoke(_lastKnownStage);
            SyncOtisPresenceInActiveScene();
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
                bool done = HasFlag(FlagCh2Completed);
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
                _lastKnownStage = ComputeStageFromFlags();
                SyncOtisPresenceInActiveScene();
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
            _lastKnownStage = ComputeStageFromFlags();
            OnBridgeStateChanged?.Invoke(BridgeState);
            OnStageChanged?.Invoke(_lastKnownStage);
            SyncOtisPresenceInActiveScene();
        }

        public void ResetForNewGame()
        {
            _completedEchoes.Clear();
            BridgeState = RiverBridgeState.Broken;
            InvestigationStage = 0;
            IsRelayEventActive = false;
            PendingSceneSpawnId = "";
            _lastKnownStage = Chapter2Stage.Arrival;
            OnBridgeStateChanged?.Invoke(BridgeState);
            OnStageChanged?.Invoke(_lastKnownStage);
            SyncOtisPresenceInActiveScene();
        }
    }
}
