using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Narrative
{
    public class StoryPropInteractable : MonoBehaviour
    {
        [SerializeField] private string propId;
        [SerializeField] private string requiredFlag;
        [SerializeField] private string completedFlag;
        [SerializeField] private string secondaryCompletedFlag;
        [SerializeField] private string requiredItemId;
        [SerializeField] private int requiredItemAmount;
        [SerializeField] private string rewardItemId;
        [SerializeField] private int rewardItemAmount;
        [SerializeField] private int staminaCost;
        [SerializeField] private string dialogueSpeaker;
        [SerializeField] private string dialogueBody;
        [SerializeField] private string blockedHint;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 1.9f;

        private PlayerController2D _player;
        private GameInputReader _input;

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            bool alreadyDone = !string.IsNullOrEmpty(completedFlag) && GameRoot.Instance != null &&
                               GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag(completedFlag);

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = !alreadyDone && Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                TryInteract();
            }
        }

        private void TryInteract()
        {
            if (GameRoot.Instance == null) return;
            var flags = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            if (!string.IsNullOrEmpty(requiredFlag) && !flags.HasFlag(requiredFlag))
            {
                GameEventBus.RaiseToastRequested(string.IsNullOrEmpty(blockedHint) ? "Chưa đủ điều kiện để tương tác." : blockedHint);
                return;
            }

            if (!string.IsNullOrEmpty(requiredItemId) && requiredItemAmount > 0)
            {
                if (inv == null || !inv.HasItem(requiredItemId, requiredItemAmount))
                {
                    int have = inv != null ? inv.GetItemCount(requiredItemId) : 0;
                    GameEventBus.RaiseToastRequested(blockedHint + " (Hiện có " + have + "/" + requiredItemAmount + ")");
                    return;
                }
            }

            if (staminaCost > 0)
            {
                GameRoot.Instance.Stats?.ConsumeStamina(staminaCost);
            }

            if (!string.IsNullOrEmpty(requiredItemId) && requiredItemAmount > 0)
            {
                inv?.RemoveItem(requiredItemId, requiredItemAmount);
            }

            GameRoot.Instance.Dialogue?.StartQuickDialogue(dialogueSpeaker, dialogueBody, () =>
            {
                if (!string.IsNullOrEmpty(rewardItemId) && rewardItemAmount > 0)
                {
                    inv?.AddItemById(rewardItemId, rewardItemAmount, true);
                }
                if (propId == "prop_seed_box")
                {
                    inv?.AddItemById("item_dew_barley_seed", 6, true);
                }
                if (!string.IsNullOrEmpty(completedFlag)) flags.SetFlag(completedFlag, true);
                if (!string.IsNullOrEmpty(secondaryCompletedFlag)) flags.SetFlag(secondaryCompletedFlag, true);
            });
        }
    }
}
