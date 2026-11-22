using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.Narrative
{
    public class NPCController : MonoBehaviour
    {
        [SerializeField] private NPCData npcData;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 2.1f;
        [SerializeField] private Vector2 helperStandPosition;

        private PlayerController2D _player;
        private GameInputReader _input;
        private bool _movedToFlowerbed;

        public NPCData Data => npcData;

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
            CheckMoveToFlowerbed();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value) CheckMoveToFlowerbed();
        }

        private void CheckMoveToFlowerbed()
        {
            if (_movedToFlowerbed || npcData == null || string.IsNullOrEmpty(npcData.helperReadyStoryFlag)) return;
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                GameRoot.Instance.Flags.HasFlag(npcData.helperReadyStoryFlag))
            {
                _movedToFlowerbed = true;
                if (helperStandPosition != Vector2.zero)
                {
                    transform.position = new Vector3(helperStandPosition.x, helperStandPosition.y, 0f);
                }
            }
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                TriggerDialogue();
            }
        }

        private void TriggerDialogue()
        {
            if (npcData == null || GameRoot.Instance == null || GameRoot.Instance.Dialogue == null) return;
            DialogueData chosen = ResolveDialogue();
            if (chosen != null)
            {
                GameRoot.Instance.Dialogue.StartDialogue(chosen);
            }
        }

        private DialogueData ResolveDialogue()
        {
            var flags = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            if (npcData.conditionalDialogues != null)
            {
                for (int i = 0; i < npcData.conditionalDialogues.Count; i++)
                {
                    var entry = npcData.conditionalDialogues[i];
                    if (entry == null || entry.dialogue == null) continue;

                    if (!string.IsNullOrEmpty(entry.requiredStoryFlag) && !flags.HasFlag(entry.requiredStoryFlag))
                        continue;
                    if (!string.IsNullOrEmpty(entry.blockedByStoryFlag) && flags.HasFlag(entry.blockedByStoryFlag))
                        continue;

                    // Kiểm tra nếu hội thoại yêu cầu nộp vật phẩm (VD: Bà Hana cần 4 Lúa Mạch)
                    if (entry.dialogue.consumesRequiredItem != null && entry.dialogue.consumesAmount > 0)
                    {
                        if (inv == null || !inv.HasItem(entry.dialogue.consumesRequiredItem.itemID, entry.dialogue.consumesAmount))
                            continue;
                    }

                    // Đặc biệt cho Ông Bách đọc thư: chỉ kích hoạt khi đã đủ 3 nhánh (4 trang sổ)
                    if (entry.dialogue.dialogueID == "dlg_bach_read_letters")
                    {
                        if (!flags.HasFlag("flag_hana_bread_done") || !flags.HasFlag("flag_toby_cat_saved") || !flags.HasFlag("flag_otis_mailbox_fixed"))
                            continue;
                    }

                    return entry.dialogue;
                }
            }
            return npcData.defaultDialogue;
        }
    }
}
