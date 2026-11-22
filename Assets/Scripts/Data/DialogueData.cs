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
    }

    [CreateAssetMenu(fileName = "Dlg_New", menuName = "Moonpetal/Data/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public string dialogueID;
        public List<DialogueLine> lines = new List<DialogueLine>();

        [Header("Hiệu ứng khi kết thúc hội thoại")]
        public string setsStoryFlagOnFinish;
        public ItemData givesItemOnFinish;
        public int givesItemAmount = 1;
        public ItemData consumesRequiredItem;
        public int consumesAmount;
        public DiaryPageData unlocksDiaryPage;
    }
}
