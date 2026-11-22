using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "DiaryPage_New", menuName = "Moonpetal/Data/DiaryPageData")]
    public class DiaryPageData : ScriptableObject
    {
        public string pageID;
        public int pageNumber = 1;
        public string titleKey;
        public string bodyTextKey;
        public Sprite illustration;
        public string unlockedByFlag;
    }
}
