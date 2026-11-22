using UnityEngine;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.UI
{
    public class QuestTrackerHUDView : MonoBehaviour
    {
        [SerializeField] private TMP_Text objectiveText;

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Quests != null)
            {
                GameRoot.Instance.Quests.OnObjectiveTextChanged += UpdateText;
                UpdateText(GameRoot.Instance.Quests.CurrentObjectiveSummary);
            }
        }

        private void OnDestroy()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Quests != null)
            {
                GameRoot.Instance.Quests.OnObjectiveTextChanged -= UpdateText;
            }
        }

        private void UpdateText(string text)
        {
            if (objectiveText != null) objectiveText.text = text;
        }
    }
}
