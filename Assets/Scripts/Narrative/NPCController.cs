using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.Narrative
{
    public class NPCController : MonoBehaviour, IInteractable
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

        private bool _movedToFlowerbed;
        private float _animTimer;
        private int _frameIndex;
        private float _cheerTimer;
        private string _cachedPromptLabel;

        private GameObject _bubbleRoot;
        private TMP_Text _bubbleText;
        private float _bubbleVisibleTimer;
        private float _bubbleCooldownTimer;
        private PlayerController2D _player;

        public NPCData Data => npcData;
        public bool CanInteract
        {
            get
            {
                if (!gameObject.activeInHierarchy || npcData == null) return false;
                if (LanternRelayManager.Instance != null && LanternRelayManager.Instance.IsRelayActive &&
                    (npcData.npcID == "npc_kael" || npcData.npcID == "npc_mara"))
                    return false;
                return true;
            }
        }
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition
        {
            get
            {
                float topY = spriteRenderer != null && spriteRenderer.sprite != null
                    ? spriteRenderer.bounds.max.y + 0.14f
                    : transform.position.y + 0.95f;
                return new Vector3(transform.position.x, topY, 0f);
            }
        }

        public string InteractionPromptLabel
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachedPromptLabel)) return _cachedPromptLabel;
                _cachedPromptLabel = "Nói chuyện: " + ResolveDisplayName();
                return _cachedPromptLabel;
            }
        }

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded;
            InteractionManager.Register(this);
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded;
            InteractionManager.Unregister(this);
        }

        private void Start()
        {
            HideLegacyPrompt();
            _animTimer = Random.Range(0f, 0.35f);
            _frameIndex = Random.Range(0, 4);
            _bubbleCooldownTimer = Random.Range(3f, 8f);
            CheckMoveToFlowerbed();
            UpdateSortingOrder();
        }

        public void HideLegacyPrompt()
        {
            if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
        }

        private string ResolveDisplayName()
        {
            if (promptObject != null)
            {
                var tmp = promptObject.GetComponentInChildren<TMP_Text>(true);
                if (tmp != null && !string.IsNullOrWhiteSpace(tmp.text))
                {
                    string raw = tmp.text.Trim();
                    if (raw.StartsWith("[E]")) raw = raw.Substring(3).Trim();
                    if (raw.StartsWith("Nói chuyện:")) raw = raw.Substring("Nói chuyện:".Length).Trim();
                    if (!string.IsNullOrEmpty(raw)) return raw;
                }
            }
            if (npcData != null && !string.IsNullOrEmpty(npcData.displayNameKey))
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Loc != null)
                {
                    string loc = GameRoot.Instance.Loc.Get(npcData.displayNameKey);
                    if (!string.IsNullOrEmpty(loc) && !loc.StartsWith("[")) return loc;
                }
                return npcData.displayNameKey;
            }
            return gameObject.name;
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value) CheckMoveToFlowerbed();
            if (value && flagId == "flag_nv1_completed") _cheerTimer = 8.0f;
        }

        private void HandleSaveLoaded()
        {
            _cheerTimer = 0f;
            CheckMoveToFlowerbed();
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
                spriteRenderer.sortingLayerName = "WorldObjects";
                float footY = spriteRenderer.sprite != null ? spriteRenderer.bounds.min.y : (transform.position.y - 0.45f);
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-footY * 100f);
            }
        }

        private void Update()
        {
            TickAnimation(Time.deltaTime);
            UpdateSortingOrder();
            TickAmbientWorldBubble(Time.deltaTime);
        }

        private void TickAmbientWorldBubble(float dt)
        {
            bool gameplay = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplay)
            {
                if (_bubbleRoot != null && _bubbleRoot.activeSelf) _bubbleRoot.SetActive(false);
                return;
            }

            if (_bubbleVisibleTimer > 0f)
            {
                _bubbleVisibleTimer -= dt;
                if (_bubbleVisibleTimer <= 0f && _bubbleRoot != null) _bubbleRoot.SetActive(false);
                return;
            }

            if (_bubbleCooldownTimer > 0f)
            {
                _bubbleCooldownTimer -= dt;
                return;
            }

            if (_player == null) _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (_player == null || npcData == null) return;

            float dist = Vector2.Distance(transform.position, _player.transform.position);
            if (dist <= 4.6f && dist >= 1.6f)
            {
                string bark = PickAmbientWorldBark();
                if (!string.IsNullOrEmpty(bark))
                {
                    ShowAmbientBubble(bark, 3.2f);
                    _bubbleCooldownTimer = 18f;
                }
            }
        }

        private string PickAmbientWorldBark()
        {
            string id = npcData.npcID ?? "";
            bool isNight = GameRoot.Instance != null && GameRoot.Instance.Time != null && GameRoot.Instance.Time.IsNight;
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            bool bloomed = flags != null && flags.HasFlag("flag_nv1_completed");
            bool ch2Done = flags != null && flags.HasFlag("flag_ch2_completed");
            bool echo2Done = flags != null && flags.HasFlag("flag_ch2_echo2_pier_done");
            bool swampUnlocked = flags != null && flags.HasFlag("flag_ch2_swamp_unlocked");

            if (ch2Done)
            {
                if (id == "npc_kael") return "Nhịp cầu lim này đứng vững thêm trăm năm nữa!";
                if (id == "npc_mara") return "Đèn bên cầu sáng ấm cả một khúc sông...";
                if (id == "npc_otis_ch2") return "Hai bờ sông lại thông thương rồi, mừng quá!";
                if (id == "npc_bach") return "Nghe tin cây cầu bờ sông nối lại rồi à? Khá lắm!";
                if (id == "npc_hana") return "Chiều nay cô sẽ mang bánh sang mừng ông Kael và bà Mara!";
                if (id == "npc_toby") return "Anh Wen ơi, mai cho em chạy qua cầu bờ sông xem nhé!";
            }
            if (id == "npc_kael")
            {
                if (echo2Done) return "Tối nay 18:30 ta cùng bà Mara đóng lại cây cầu!";
                if (swampUnlocked) return "Ngọn đèn dầu đã sáng, cẩn thận đám quái đầm lầy nhé cháu.";
                return isNight ? "Chậc... thanh dầm này vẫn hơi mục..." : "Đứng xa mố cầu ra, gỗ mục rồi.";
            }
            if (id == "npc_mara")
            {
                if (echo2Done) return "Tối nay 18:30 tôi sẽ mang đinh ra bờ Tây.";
                if (swampUnlocked) return "Lối lên Đầm Lầy Bờ Tây đã tan sương rồi, cháu cẩn thận.";
                return isNight ? "Gió sông lạnh quá... lại nhớ đêm bão 3 năm trước." : "Người bên kia sông gửi cháu sang à?";
            }
            if (id == "npc_otis_ch2")
                return echo2Done ? "Sự thật sáng tỏ rồi, chuẩn bị cho Đêm Chuyền Đèn nhé!" : "Cuốn sổ cũ này ghi đủ chuyện đêm bão 3 năm trước đấy.";

            if (bloomed)
            {
                if (id == "npc_bach") return "Đất ấm hẳn lên rồi...";
                if (id == "npc_hana") return "Mùi hoa thơm cả tiệm bánh!";
                if (id == "npc_toby") return "Hoa sáng quá đi mất!";
                if (id == "npc_otis") return "Thung lũng hồi sinh thật rồi.";
            }

            if (isNight)
            {
                if (id == "npc_bach") return "Sương đêm lại dày thêm rồi...";
                if (id == "npc_hana") return "Khuya rồi, Wen về ngủ sớm đi.";
                if (id == "npc_toby") return "Miu ơi, đừng đi xa nhé...";
                if (id == "npc_otis") return "E hèm, đèn lồng sáng cả rồi.";
            }

            if (id == "npc_bach") return "Hừm... đất chai hết cả rồi.";
            if (id == "npc_hana") return "Bột mì sắp hết nữa rồi...";
            if (id == "npc_toby") return "Ước gì ai đó giúp bé Miu...";
            if (id == "npc_otis") return "Cái bản lề hòm thư kẹt cứng...";
            return "";
        }

        private void ShowAmbientBubble(string text, float duration)
        {
            if (_bubbleRoot == null)
            {
                _bubbleRoot = new GameObject("AmbientBubble");
                _bubbleRoot.transform.SetParent(transform, false);
                _bubbleRoot.transform.localPosition = new Vector3(0f, 1.25f, 0f);
                _bubbleRoot.transform.localScale = new Vector3(0.011f, 0.011f, 1f);

                var cv = _bubbleRoot.AddComponent<Canvas>();
                cv.renderMode = RenderMode.WorldSpace;
                cv.sortingLayerName = "UI";
                cv.sortingOrder = 45;

                var rt = _bubbleRoot.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(280f, 46f);
                var bg = _bubbleRoot.AddComponent<Image>();
                bg.color = new Color(0.14f, 0.10f, 0.08f, 0.84f);

                GameObject tGo = new GameObject("Txt");
                tGo.transform.SetParent(_bubbleRoot.transform, false);
                var tr = tGo.AddComponent<RectTransform>();
                tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
                tr.offsetMin = new Vector2(10f, 4f); tr.offsetMax = new Vector2(-10f, -4f);

                _bubbleText = tGo.AddComponent<TextMeshProUGUI>();
                _bubbleText.fontSize = 19f;
                _bubbleText.alignment = TextAlignmentOptions.Center;
                _bubbleText.color = new Color32(252, 244, 222, 255);
                var existingTmp = promptObject != null ? promptObject.GetComponentInChildren<TMP_Text>(true) : null;
                if (existingTmp != null && existingTmp.font != null) _bubbleText.font = existingTmp.font;
            }

            _bubbleText.text = text;
            _bubbleRoot.SetActive(true);
            _bubbleVisibleTimer = duration;
        }

        public void Interact()
        {
            if (_bubbleRoot != null) _bubbleRoot.SetActive(false);
            TriggerDialogue();
        }

        private void TickAnimation(float dt)
        {
            if (spriteRenderer == null) return;
            if (_cheerTimer > 0f) _cheerTimer -= dt;
            bool playCheer = (_cheerTimer > 0f) && cheerFrames != null && cheerFrames.Length > 0 && cheerFrames[0] != null;

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

            var flags = GameRoot.Instance.Flags;

            // [Step Ch2-R1] Ngăn nhảy cóc Stage 0 -> Stage 1 trước khi Wen ra kiểm tra cầu gãy
            if (npcData.npcID == "npc_kael" && flags != null && !flags.HasFlag(Chapter2StateManager.FlagSoloBridgeFailed))
            {
                GameRoot.Instance.Dialogue.StartQuickLines(new[]
                {
                    ("Ông Kael", "Này cô bé mới tới! Ra phía Tây xem cây cầu gãy kia trước đi đã, rồi quay lại xưởng nói chuyện với tôi."),
                    ("Tinh Linh Lumi", "Mình ra đầu cầu phía Tây kiểm tra thử xem cầu hư hại tới mức nào nhé Wen!")
                });
                return;
            }

            DialogueData conditional = ResolveConditionalOnly();
            if (conditional != null)
            {
                GameRoot.Instance.Dialogue.StartDialogue(conditional);
                return;
            }

            if (npcData.npcID == "npc_bach" && flags != null &&
                flags.HasFlag("flag_otis_mailbox_fixed") && !flags.HasFlag("flag_bach_letters_read") &&
                (!flags.HasFlag("flag_hana_bread_done") || !flags.HasFlag("flag_toby_cat_saved")))
            {
                GameRoot.Instance.Dialogue.StartQuickLines(new[]
                {
                    ("Wen", "Ông Bach ơi, cháu tìm thấy lá thư bà chưa kịp gửi cho ông..."),
                    ("Ông Bach", "Cất đi. Một tờ giấy cũ có làm tiệm bánh thơm lại hay lũ trẻ quay về vườn được không?"),
                    ("Tinh Linh Lumi", "Ông ấy sợ đọc xong lại buồn đấy Wen... Mình gọi đủ Bà Hana và Toby về vườn rồi quay lại nhé.")
                });
                return;
            }

            if (TryTriggerReactiveDialogue()) return;

            if (npcData.defaultDialogue != null)
            {
                GameRoot.Instance.Dialogue.StartDialogue(npcData.defaultDialogue);
            }
        }

        private bool TryTriggerReactiveDialogue()
        {
            if (GameRoot.Instance == null || npcData == null) return false;
            string id = npcData.npcID ?? "";
            bool exhausted = GameRoot.Instance.Stats != null && GameRoot.Instance.Stats.IsExhaustedPenalty;
            bool isNight = GameRoot.Instance.Time != null && GameRoot.Instance.Time.IsNight;
            bool isDusk = GameRoot.Instance.Time != null && GameRoot.Instance.Time.CurrentPhase == DayPhase.Dusk;
            var flags = GameRoot.Instance.Flags;
            if (flags != null && flags.HasFlag("flag_ch2_completed"))
            {
                if (id == "npc_bach" && !flags.HasFlag("flag_ch2_congrats_bach"))
                {
                    flags.SetFlag("flag_ch2_congrats_bach", true);
                    GameRoot.Instance.Dialogue.StartQuickLines(new[]
                    {
                        ("Ông Bach", "Ta nghe bác Otis kể rồi. Cháu nối lại được cả cây cầu bờ sông lẫn tình nghĩa của hai lão già cứng đầu đó à?"),
                        ("Wen", "Hai bác ấy tự tay đóng tấm ván cuối cùng đó ạ, cháu chỉ giữ đèn sáng thôi."),
                        ("Ông Bach", "Hừm... giống hệt bà cháu hồi xưa. Nghỉ ngơi đi rồi hẵng tính chuyện lên phía Bắc.")
                    });
                    return true;
                }
                if (id == "npc_hana" && !flags.HasFlag("flag_ch2_congrats_hana"))
                {
                    flags.SetFlag("flag_ch2_congrats_hana", true);
                    GameRoot.Instance.Dialogue.StartQuickLines(new[]
                    {
                        ("Bà Hana", "Wen ơi! Cả làng nghe tin cầu bờ sông sáng đèn lại rồi ai cũng mừng rơn!"),
                        ("Bà Hana", "Hôm qua bà Mara còn gửi người sang hỏi công thức bánh nướng nữa đấy. Cảm ơn cháu nhiều lắm!")
                    });
                    return true;
                }
                if (id == "npc_toby" && !flags.HasFlag("flag_ch2_congrats_toby"))
                {
                    flags.SetFlag("flag_ch2_congrats_toby", true);
                    GameRoot.Instance.Dialogue.StartQuickLines(new[]
                    {
                        ("Toby", "Anh Wen siêu quá! Em nghe bảo anh vừa đánh bướm đêm vừa giữ sáng cả sáu cột đèn trên sông luôn!"),
                        ("Tinh Linh Lumi", "Hì hì, có cả công Ánh Soi cấp hai của Lumi nữa đấy Toby!")
                    });
                    return true;
                }
            }

            if (exhausted)
            {
                if (id == "npc_hana")
                {
                    GameRoot.Instance.Dialogue.StartQuickLines(new[]
                    {
                        ("Bà Hana", "Ôi chao, mặt cháu tái mét kìa! Hôm qua kiệt sức phải không? Ăn miếng Bánh Mì Nắng Ấm đi con!"),
                        ("Wen", "Cháu ổn mà cô... chắc tối qua cháu hơi cố quá sức.")
                    });
                    return true;
                }
                if (id == "npc_bach")
                {
                    GameRoot.Instance.Dialogue.StartQuickLines(new[]
                    {
                        ("Ông Bach", "Chậc, làm việc không biết lượng sức. Có bánh mì của Hana thì ăn đi, đứng đó run lẩy bẩy."),
                        ("Tinh Linh Lumi", "Đấy thấy chưa, Lumi đã bảo Wen đừng cố quá rồi mà!")
                    });
                    return true;
                }
            }

            if (isNight)
            {
                if (id == "npc_hana")
                {
                    GameRoot.Instance.Dialogue.StartQuickDialogue("Bà Hana", "Trời tối sương xuống rồi Wen ơi! Vào Nhà Kính ngủ sớm kẻo 2 giờ sáng gục ngoài đường đấy.");
                    return true;
                }
                if (id == "npc_bach")
                {
                    GameRoot.Instance.Dialogue.StartQuickDialogue("Ông Bach", "Khuya rồi còn lang thang gì nữa? Về giường đi, đất không chạy mất đâu mà lo.");
                    return true;
                }
            }
            else if (isDusk && id == "npc_otis")
            {
                GameRoot.Instance.Dialogue.StartQuickDialogue("Bác Otis", "Đèn lồng làng vừa bật rồi. Chiều muộn gió lạnh, cháu nhớ để ý thanh Thể lực nhé.");
                return true;
            }

            return false;
        }

        private DialogueData ResolveConditionalOnly()
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

                    if (entry.dialogue.dialogueID == "dlg_mara_s3_told_truth")
                    {
                        if (!flags.HasFlag(Chapter2StateManager.FlagKaelGaveLantern) || !flags.HasFlag("flag_ch2_choice_tell_truth"))
                            continue;
                    }

                    return entry.dialogue;
                }
            }
            return null;
        }
    }
}
