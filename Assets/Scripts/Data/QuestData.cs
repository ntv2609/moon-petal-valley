using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    public enum QuestActionType
    {
        TalkToNPC,
        CollectItem,
        ClearBlightZone,
        CleanProp,
        HarvestCrop,
        RepairObject,
        ReadDiaryPages,
        CompleteBundle,
        BloomPlantingSpot
    }

    [Serializable]
    public class QuestStepDefinition
    {
        public string stepID;
        public string descriptionKey;
        public QuestActionType actionType;
        public string targetID;
        public int requiredCount = 1;
        public string completionStoryFlag;
        public DiaryPageData unlockedDiaryPage;
    }

    [CreateAssetMenu(fileName = "Quest_New", menuName = "Moonpetal/Data/QuestData")]
    public class QuestData : ScriptableObject
    {
        public string questID;
        public string questTitleKey;
        public string questSummaryKey;
        public List<QuestStepDefinition> steps = new List<QuestStepDefinition>();
        public ItemData completionRewardItem;
        public QuestData nextQuest;
    }
}
