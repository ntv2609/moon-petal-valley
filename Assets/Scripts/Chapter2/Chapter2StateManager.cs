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

    public class Chapter2StateManager : MonoBehaviour, ISaveable, IResettable
    {
        public const string MainVillageSceneName = "MoonpetalValley_MVP";
        public const string Chapter2SceneName = "Riverbank_Chapter2";
        public const string FlagOtisMovedToCh2 = "flag_ch2_otis_arrived";
        public const string FlagClue3Done = "flag_ch2_inv_night3_done";

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

        public bool HasOtisMovedToChapter2()
        {
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags == null) return false;
            return flags.HasFlag(FlagOtisMovedToCh2) || flags.HasFlag(FlagClue3Done);
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
                        bool shouldShow = !otisMoved;
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
                        bool shouldShow = otisMoved;
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
            if (flagId == FlagOtisMovedToCh2 || flagId == FlagClue3Done)
            {
                SyncOtisPresenceInActiveScene();
            }

            if (!value) return;
            if (flagId == "flag_ch2_completed")
            {
                SetBridgeState(RiverBridgeState.Repaired);
            }
        }

        private void SyncFromFlags()
        {
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags != null && flags.HasFlag("flag_ch2_completed"))
            {
                SetBridgeState(RiverBridgeState.Repaired);
            }
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
                bool done = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                            GameRoot.Instance.Flags.HasFlag("flag_ch2_completed");
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
            OnBridgeStateChanged?.Invoke(BridgeState);
            SyncOtisPresenceInActiveScene();
        }

        public void ResetForNewGame()
        {
            _completedEchoes.Clear();
            BridgeState = RiverBridgeState.Broken;
            InvestigationStage = 0;
            IsRelayEventActive = false;
            PendingSceneSpawnId = "";
            OnBridgeStateChanged?.Invoke(BridgeState);
            SyncOtisPresenceInActiveScene();
        }
    }
}
