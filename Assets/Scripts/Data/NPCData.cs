using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public class ConditionalDialogueEntry
    {
        public string requiredStoryFlag;
        public string blockedByStoryFlag;
        public DialogueData dialogue;
    }

    [CreateAssetMenu(fileName = "NPC_New", menuName = "Moonpetal/Data/NPCData")]
    public class NPCData : ScriptableObject
    {
        public string npcID;
        public string displayNameKey;
        public string roleTitleKey;
        public Sprite portraitSprite;
        public Sprite worldSprite;
        public DialogueData defaultDialogue;
        public List<ConditionalDialogueEntry> conditionalDialogues = new List<ConditionalDialogueEntry>();
        public string isHelperForBundleID;
        public string helperReadyStoryFlag;
    }
}
