using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public class DialogueLine
    {
        public string speakerNameKey;
        public string textKey;
        public Sprite portraitOverride;

        [Header("Step 32: Callback & Voice")]
        public string speakerId;
        public string requiredFlag;
        public string blockedFlag;
    }

    [Serializable]
    public class DialogueChoice
    {
        public string choiceTextKey;
        public string setsFlagOnPick;
        public string targetNpcId;
        public int affinityDelta;
        public DialogueData branchDialogue;
    }

    [Serializable]
    public struct ExtraItemReward
    {
        public ItemData item;
        public int amount;
    }

    [CreateAssetMenu(fileName = "Dlg_New", menuName = "Moonpetal/Data/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public string dialogueID;
        public List<DialogueLine> lines = new List<DialogueLine>();

        [Header("Lựa chọn có hệ quả (tối đa 2)")]
        public List<DialogueChoice> choices = new List<DialogueChoice>();

        [Header("Hiệu ứng khi kết thúc hội thoại")]
        public string setsStoryFlagOnFinish;
        public ItemData givesItemOnFinish;
        public int givesItemAmount = 1;
        public List<ExtraItemReward> extraRewards = new List<ExtraItemReward>();
        public ItemData consumesRequiredItem;
        public int consumesAmount;
        public DiaryPageData unlocksDiaryPage;
        public string affinityNpcId;
        public int affinityBonusOnFinish;
    }
}
