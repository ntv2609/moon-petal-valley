using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.Chapter2
{
    public class InvestigationClueInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string clueTitle = "Dấu vết bờ sông";
        [SerializeField] private int investigationStage = 1;
        [SerializeField] private string requiredPrevFlag = "";
        [SerializeField] private string completionFlag = "";
        [SerializeField] private bool requireNight = true;
        [SerializeField] private float nightStartHour = 18.0f;
        [SerializeField] private float interactRadius = 1.9f;
        [SerializeField] private DialogueData clueDialogue;
        [SerializeField] private string approachNpcName = "";

        private bool _isTriggering;

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 0.8f, 0f);
        public string InteractionPromptLabel => "Kiểm tra: " + clueTitle;

        public bool CanInteract
        {
            get
            {
                if (_isTriggering || !gameObject.activeInHierarchy) return false;
                if (Chapter2StateManager.Instance != null)
                {
                    if (Chapter2StateManager.Instance.IsRelayEventActive) return false;
                    return Chapter2StateManager.Instance.IsClueUnlocked(investigationStage);
                }
                StoryFlagManager sf = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
                if (sf != null && !string.IsNullOrEmpty(completionFlag) && sf.HasFlag(completionFlag))
                    return false;
                if (sf != null && !string.IsNullOrEmpty(requiredPrevFlag) && !sf.HasFlag(requiredPrevFlag))
                    return false;
                return true;
            }
        }

        private void OnEnable()
        {
            InteractionManager.Register(this);
        }

        private void OnDisable()
        {
            InteractionManager.Unregister(this);
        }

        public void HideLegacyPrompt() { }

        public void Interact()
        {
            if (_isTriggering || !CanInteract) return;

            StoryFlagManager sf = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (requireNight)
            {
                TimeOfDayManager tm = GameRoot.Instance != null
                    ? GameRoot.Instance.Time
                    : UnityEngine.Object.FindAnyObjectByType<TimeOfDayManager>();
                if (tm != null && tm.CurrentHourFloat < nightStartHour && tm.CurrentHourFloat >= 6.0f)
                {
                    GameEventBus.RaiseToastRequested("Ban ngày khó thấy rõ dấu vết mới - hãy ngồi ở Ghế Đá Bờ Sông chờ đến 18:30.");
                    return;
                }
            }

            string targetNpc = ResolveApproachNpcName();
            if (!string.IsNullOrEmpty(targetNpc))
            {
                GameObject npcGo = FindNpcEvenIfInactive(targetNpc);
                if (npcGo != null)
                {
                    _isTriggering = true;
                    bool wasInactive = !npcGo.activeSelf;

                    NPCController npcCtrl = npcGo.GetComponent<NPCController>();
                    bool isOtis = npcGo.name == "NPC_Otis_Ch2" ||
                                  (npcCtrl != null && npcCtrl.Data != null && npcCtrl.Data.npcID == "npc_otis_ch2");

                    if (isOtis)
                    {
                        if (Chapter2StateManager.Instance != null)
                        {
                            Chapter2StateManager.Instance.MarkOtisMovedToChapter2();
                        }
                        else if (sf != null)
                        {
                            sf.SetFlag(Chapter2StateManager.FlagOtisMovedToCh2, true);
                        }
                    }

                    if (!npcGo.activeSelf)
                    {
                        npcGo.SetActive(true);
                    }

                    NPCApproachOnInteract approach = npcGo.GetComponent<NPCApproachOnInteract>();
                    if (approach == null)
                    {
                        approach = npcGo.AddComponent<NPCApproachOnInteract>();
                    }

                    approach.RunToPlayerAndExecute(() =>
                    {
                        RunClueDialogueAndFinish(sf, approach);
                    }, spawnFromOffscreen: wasInactive);
                    return;
                }
            }

            RunClueDialogueAndFinish(sf, null);
        }

        private string ResolveApproachNpcName()
        {
            if (!string.IsNullOrEmpty(approachNpcName))
                return approachNpcName;

            if (gameObject.name == "Clue_OtisNote" ||
                completionFlag == Chapter2StateManager.FlagClue3Done ||
                DialogueHasSpeaker(clueDialogue, "otis"))
            {
                return "NPC_Otis_Ch2";
            }

            return "";
        }

        private static bool DialogueHasSpeaker(DialogueData dlg, string speakerKey)
        {
            if (dlg == null || dlg.lines == null) return false;
            for (int i = 0; i < dlg.lines.Count; i++)
            {
                DialogueLine line = dlg.lines[i];
                if (line == null) continue;
                if (!string.IsNullOrEmpty(line.speakerId) &&
                    line.speakerId.ToLowerInvariant().Contains(speakerKey))
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(line.speakerNameKey) &&
                    line.speakerNameKey.ToLowerInvariant().Contains(speakerKey))
                {
                    return true;
                }
            }
            return false;
        }

        private void RunClueDialogueAndFinish(StoryFlagManager sf, NPCApproachOnInteract approach)
        {
            var dm = GameRoot.Instance != null ? GameRoot.Instance.Dialogue : null;
            if (dm != null && clueDialogue != null)
            {
                dm.StartDialogue(clueDialogue, () =>
                {
                    CompleteClue(sf, approach);
                });
            }
            else
            {
                CompleteClue(sf, approach);
            }
        }

        private void CompleteClue(StoryFlagManager sf, NPCApproachOnInteract approach)
        {
            _isTriggering = false;
            if (sf != null && !string.IsNullOrEmpty(completionFlag))
            {
                sf.SetFlag(completionFlag, true);
            }
            if (Chapter2StateManager.Instance != null && investigationStage > 0)
            {
                if (investigationStage > Chapter2StateManager.Instance.InvestigationStage)
                {
                    Chapter2StateManager.Instance.SetInvestigationStage(investigationStage);
                }
            }
            if (approach != null)
            {
                approach.ReturnHome();
            }
        }

        private static GameObject FindNpcEvenIfInactive(string targetName)
        {
            NPCController[] allNpcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            for (int i = 0; i < allNpcs.Length; i++)
            {
                NPCController npc = allNpcs[i];
                if (npc == null) continue;
                if (npc.gameObject.name == targetName)
                    return npc.gameObject;
                if (npc.Data != null && npc.Data.npcID == targetName)
                    return npc.gameObject;
            }
            return GameObject.Find(targetName);
        }

        public void Configure(
            string title,
            int stage,
            string prevFlag,
            string doneFlag,
            DialogueData dlg,
            bool nightOnly = true,
            string approachNpc = "")
        {
            clueTitle = title;
            investigationStage = Mathf.Clamp(stage, 0, 3);
            requiredPrevFlag = prevFlag;
            completionFlag = doneFlag;
            clueDialogue = dlg;
            requireNight = nightOnly;
            approachNpcName = approachNpc;
        }

        public void Configure(
            string title,
            string prevFlag,
            string doneFlag,
            DialogueData dlg,
            bool nightOnly = true,
            string approachNpc = "")
        {
            Configure(title, investigationStage, prevFlag, doneFlag, dlg, nightOnly, approachNpc);
        }
    }
}
