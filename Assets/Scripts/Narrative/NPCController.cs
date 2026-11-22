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

        [Header("Animation Frames (32x32 FullRect)")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] cheerFrames;
        [SerializeField] private float idleFps = 2.4f;
        [SerializeField] private float cheerFps = 6.0f;

        private PlayerController2D _player;
        private GameInputReader _input;
        private bool _movedToFlowerbed;
        private float _animTimer;
        private int _frameIndex;
        private float _cheerTimer;

        public NPCData Data => npcData;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

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
            // Lệch pha ngẫu nhiên để 4 NPC không chớp mắt cùng lúc
            _animTimer = Random.Range(0f, 0.35f);
            _frameIndex = Random.Range(0, 4);
            CheckMoveToFlowerbed();
            UpdateSortingOrder();
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
                    UpdateSortingOrder();
                }
            }
        }

        private void UpdateSortingOrder()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
            }
        }

        private void Update()
        {
            TickAnimation(Time.deltaTime);

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

        private void TickAnimation(float dt)
        {
            if (spriteRenderer == null) return;

            if (_cheerTimer > 0f) _cheerTimer -= dt;
            bool celebratingVictory = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                                      GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            bool playCheer = (_cheerTimer > 0f || celebratingVictory) && cheerFrames != null && cheerFrames.Length > 0 && cheerFrames[0] != null;

            Sprite[] frames = playCheer ? cheerFrames : idleFrames;
            if (frames == null || frames.Length == 0 || frames[0] == null) return;

            float fps = playCheer ? cheerFps : idleFps;
            float dur = fps > 0.01f ? (1f / fps) : 0.35f;
            _animTimer += dt;

            if (_animTimer >= dur)
            {
                _animTimer -= dur;
                _frameIndex = (_frameIndex + 1) % frames.Length;
            }

            int idx = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
            if (frames[idx] != null) spriteRenderer.sprite = frames[idx];
        }

        private void TriggerDialogue()
        {
            if (npcData == null || GameRoot.Instance == null || GameRoot.Instance.Dialogue == null) return;
            _cheerTimer = 2.0f;
            _frameIndex = 0;
            _animTimer = 0f;

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

                    if (entry.dialogue.consumesRequiredItem != null && entry.dialogue.consumesAmount > 0)
                    {
                        if (inv == null || !inv.HasItem(entry.dialogue.consumesRequiredItem.itemID, entry.dialogue.consumesAmount))
                            continue;
                    }

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
