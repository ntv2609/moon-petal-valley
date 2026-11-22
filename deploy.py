#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Moonpetal Valley - Step 40.fix 4: Complete Verified Full-File Fixes (6 Audited Bugs)
Target: Unity 6.6 (6000.6.2f1) | Python 3 stdlib only | Idempotent
"""

import os
import shutil
from datetime import datetime

PROJECT_ROOT = os.path.abspath(os.getcwd())
BACKUP_DIR = os.path.join(
    PROJECT_ROOT, "_deploy_backup", "Step_40_fix4_" + datetime.now().strftime("%Y%m%d_%H%M%S")
)

FILES = {
    "Assets/Scripts/Narrative/MiuCatInteractable.cs": r"""using System.Collections;
using UnityEngine;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Narrative
{
    public class MiuCatInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueData reunionDialogue;
        [SerializeField] private float interactRadius = 2.15f;
        [SerializeField] private Vector3 homePositionNearToby = new Vector3(4.6f, 5.1f, 0f);
        [SerializeField] private float runSpeed = 4.8f;
        [SerializeField] private TMP_Text floatingPromptText;

        private SpriteRenderer _sr;
        private Collider2D _col;
        private Vector3 _basePos;
        private bool _isRunningHome;
        private bool _hasArrivedHome;

        public bool CanInteract => gameObject.activeInHierarchy && !_isRunningHome;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 0.85f, 0f);

        public string InteractionPromptLabel
        {
            get
            {
                if (_hasArrivedHome || HasFlag("flag_miu_comforted") || HasFlag("flag_toby_cat_saved"))
                    return "Vuốt ve Mèo Miu";
                if (HasFlag("flag_toby_cat_slimes_cleared"))
                    return "An ủi Mèo Miu";
                return "Mèo Miu (Đang sợ!)";
            }
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _basePos = transform.position;
            EnsureSolidCollider();
        }

        private void EnsureSolidCollider()
        {
            _col = GetComponent<Collider2D>();
            if (_col == null)
            {
                var cc = gameObject.AddComponent<CircleCollider2D>();
                cc.radius = 0.28f;
                cc.offset = new Vector2(0f, -0.08f);
                cc.isTrigger = false;
                _col = cc;
            }
            gameObject.layer = 11;
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += SyncWithStoryFlags;
            InteractionManager.Register(this);
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= SyncWithStoryFlags;
            InteractionManager.Unregister(this);
        }

        private void Start()
        {
            HideLegacyPrompt();
            SyncWithStoryFlags();
        }

        public void HideLegacyPrompt()
        {
            if (floatingPromptText != null && floatingPromptText.gameObject.activeSelf)
                floatingPromptText.gameObject.SetActive(false);
        }

        private void SyncWithStoryFlags()
        {
            HideLegacyPrompt();
            if (HasFlag("flag_miu_comforted") || HasFlag("flag_toby_cat_saved"))
            {
                SnapToTobyHome();
            }
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value && (flagId == "flag_miu_comforted" || flagId == "flag_toby_cat_saved"))
            {
                if (!_isRunningHome && !_hasArrivedHome)
                    SnapToTobyHome();
            }
        }

        private void Update()
        {
            if (_isRunningHome) return;
            AnimateIdleCat();
        }

        public void Interact()
        {
            if (!CanInteract) return;
            HandleInteract();
        }

        private void HandleInteract()
        {
            if (!HasFlag("flag_toby_cat_slimes_cleared"))
            {
                GameEventBus.RaiseToastRequested("Mèo Miu đang run rẩy! Hãy dùng Liềm Ánh Trăng (4) dọn sạch 3 Bọt Sương Đen quanh đây trước.");
                return;
            }

            if (!HasFlag("flag_miu_comforted") && !HasFlag("flag_toby_cat_saved"))
            {
                if (reunionDialogue != null && GameRoot.Instance?.Dialogue != null)
                    GameRoot.Instance.Dialogue.StartDialogue(reunionDialogue, OnReunionFinished);
                else
                    OnReunionFinished();
                return;
            }

            GameRoot.Instance?.Audio?.PlayUIClick();
            GameEventBus.RaiseToastRequested("Mèo Miu dụi đầu vào tay bạn: 'Meo~ rừ rừ...'");
        }

        private void OnReunionFinished()
        {
            GameRoot.Instance?.Flags?.SetFlag("flag_miu_comforted", true);
            GameEventBus.RaiseToastRequested("Mèo Miu hết sợ rồi và đang chạy về với Cậu Bé Toby! Hãy quay lại gặp Toby [E].");
            StartCoroutine(RunHomeToTobyRoutine());
        }

        private IEnumerator RunHomeToTobyRoutine()
        {
            _isRunningHome = true;
            if (_col != null) _col.enabled = false;
            Vector3 target = homePositionNearToby;
            if (_sr != null) _sr.flipX = target.x < transform.position.x;

            float hopTimer = 0f;
            while (Vector2.Distance(transform.position, target) > 0.15f)
            {
                hopTimer += Time.deltaTime;
                Vector3 next = Vector3.MoveTowards(transform.position, target, runSpeed * Time.deltaTime);
                float hop = Mathf.Abs(Mathf.Sin(hopTimer * 14f)) * 0.16f;
                transform.position = new Vector3(next.x, next.y + hop * Time.deltaTime * 4f, 0f);
                yield return null;
            }

            SnapToTobyHome();
            if (_col != null) _col.enabled = true;
            _isRunningHome = false;
        }

        private void SnapToTobyHome()
        {
            transform.position = homePositionNearToby;
            _basePos = homePositionNearToby;
            _hasArrivedHome = true;
            if (_sr != null) _sr.flipX = false;
            if (_col != null) _col.enabled = true;
        }

        private void AnimateIdleCat()
        {
            if (!HasFlag("flag_toby_cat_slimes_cleared"))
            {
                float shiver = Mathf.Sin(Time.time * 26f) * 0.03f;
                transform.position = new Vector3(_basePos.x + shiver, _basePos.y, _basePos.z);
            }
            else
            {
                float bob = Mathf.Sin(Time.time * 3.8f) * 0.04f;
                transform.position = new Vector3(_basePos.x, _basePos.y + bob, _basePos.z);
            }
        }

        private bool HasFlag(string flagId)
        {
            return GameRoot.Instance != null && GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag(flagId);
        }
    }
}
""",

    "Assets/Scripts/Narrative/NPCController.cs": r"""using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

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
        public bool CanInteract => gameObject.activeInHierarchy && npcData != null;
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
            bool isNight = GameRoot.Instance?.Time != null && GameRoot.Instance.Time.IsNight;
            bool bloomed = GameRoot.Instance?.Flags != null && GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");

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

            DialogueData conditional = ResolveConditionalOnly();
            if (conditional != null)
            {
                GameRoot.Instance.Dialogue.StartDialogue(conditional);
                return;
            }

            var flags = GameRoot.Instance.Flags;
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

                    return entry.dialogue;
                }
            }
            return null;
        }
    }
}
""",

    "Assets/Scripts/Player/LumiCompanionController.cs": r"""using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Player
{
    public class LumiCompanionController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer haloRenderer;
        [SerializeField] private Vector2 followOffset = new Vector2(-0.75f, 0.75f);
        [SerializeField] private float smoothSpeed = 6.5f;

        [Header("Pixel Art Animations (FullRect)")]
        [SerializeField] private Sprite[] floatFrames;
        [SerializeField] private Sprite[] pulseFrames;
        [SerializeField] private float floatFps = 6f;
        [SerializeField] private float pulseFps = 8f;
        [SerializeField] private float autoPulseInterval = 4.5f;

        private SpriteRenderer _shadowRenderer;
        private float _bobTime;
        private float _animTimer;
        private int _frameIdx;
        private float _pulseRemaining;
        private float _autoPulseCooldown = 2.5f;
        private Vector3 _lastPos;

        private bool _isAwakened;
        private bool _isPlayingAwakenAnim;
        private float _awakenTimer;
        private Vector3 _awakenStartPos;
        private const float AwakenDuration = 1.15f;

        private void Awake()
        {
            if (spriteRenderer == null && visualTransform != null)
                spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
            Transform sh = transform.Find("GroundShadow");
            if (sh != null) _shadowRenderer = sh.GetComponent<SpriteRenderer>();
            _lastPos = transform.position;
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded;
        }

        private void Start()
        {
            HandleSaveLoaded();
        }

        private void HandleSaveLoaded()
        {
            bool alreadyAwake = GameRoot.Instance != null &&
                                GameRoot.Instance.Flags != null &&
                                GameRoot.Instance.Flags.HasFlag("flag_lumi_awakened");
            _isAwakened = alreadyAwake;
            _isPlayingAwakenAnim = false;
            SetVisualsVisible(_isAwakened);

            bool bloomed = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                           GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            SetHaloRadiusScale(bloomed ? 3.2f : 1.0f);
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (flagId != "flag_lumi_awakened") return;

            if (value)
            {
                if (!_isAwakened && !_isPlayingAwakenAnim)
                {
                    _isAwakened = true;
                    SetVisualsVisible(true);
                    TriggerPulse(1.0f);
                }
            }
            else
            {
                _isAwakened = false;
                _isPlayingAwakenAnim = false;
                SetVisualsVisible(false);
            }
        }

        public void PlayAwakenFromBoxAnimation(Vector3 boxWorldPos)
        {
            _isAwakened = true;
            _isPlayingAwakenAnim = true;
            _awakenTimer = 0f;
            _awakenStartPos = boxWorldPos + new Vector3(0f, 0.15f, 0f);

            transform.position = _awakenStartPos;
            SetVisualsVisible(true);
            if (_shadowRenderer != null) _shadowRenderer.enabled = false;

            if (visualTransform != null)
            {
                visualTransform.localPosition = Vector3.zero;
                visualTransform.localScale = new Vector3(0.15f, 0.15f, 1f);
            }

            TriggerPulse(1.4f);
            JuiceEffectsManager.Instance?.TriggerCameraShake(0.07f, 0.18f);
        }

        private void SetVisualsVisible(bool visible)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = visible;
            if (haloRenderer != null) haloRenderer.enabled = visible;
            if (_shadowRenderer != null) _shadowRenderer.enabled = visible;
        }

        private void LateUpdate()
        {
            if (!_isAwakened && !_isPlayingAwakenAnim) return;

            if (followTarget == null)
            {
                var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (player != null) followTarget = player.transform;
                else return;
            }

            if (_isPlayingAwakenAnim)
            {
                UpdateAwakenAnimation();
                UpdateSpriteAnimation(Time.unscaledDeltaTime);
                return;
            }

            Vector3 desired = followTarget.position + new Vector3(followOffset.x, followOffset.y, 0f);
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smoothSpeed);

            float dx = transform.position.x - _lastPos.x;
            if (spriteRenderer != null && Mathf.Abs(dx) > 0.002f)
                spriteRenderer.flipX = dx < 0f;
            _lastPos = transform.position;

            _bobTime += Time.deltaTime * 3.2f;
            if (visualTransform != null)
            {
                visualTransform.localPosition = new Vector3(0f, Mathf.Sin(_bobTime) * 0.14f, 0f);
                visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, Vector3.one, Time.deltaTime * 10f);
            }

            if (haloRenderer != null)
            {
                float boost = _pulseRemaining > 0f ? 0.22f : 0f;
                float pulse = 0.85f + boost + Mathf.Sin(_bobTime * 1.5f) * 0.15f;
                haloRenderer.transform.localScale = new Vector3(pulse, pulse, 1f);
            }

            if (_pulseRemaining > 0f)
            {
                _pulseRemaining -= Time.deltaTime;
            }
            else if (autoPulseInterval > 0f && pulseFrames != null && pulseFrames.Length > 0)
            {
                _autoPulseCooldown -= Time.deltaTime;
                if (_autoPulseCooldown <= 0f)
                {
                    TriggerPulse(0.65f);
                    _autoPulseCooldown = autoPulseInterval;
                }
            }

            UpdateSpriteAnimation(Time.deltaTime);
        }

        private void UpdateAwakenAnimation()
        {
            float dt = Time.unscaledDeltaTime;
            _awakenTimer += dt;
            float t = Mathf.Clamp01(_awakenTimer / AwakenDuration);
            Vector3 targetShoulder = followTarget.position + new Vector3(followOffset.x, followOffset.y, 0f);

            if (t < 0.55f)
            {
                float p = t / 0.55f;
                float easeOut = 1f - Mathf.Pow(1f - p, 3f);
                float swirlX = Mathf.Sin(p * Mathf.PI * 2.5f) * 0.28f * (1f - p * 0.4f);
                float riseY = easeOut * 1.15f;
                transform.position = _awakenStartPos + new Vector3(swirlX, riseY, 0f);

                float sc = p < 0.65f
                    ? Mathf.Lerp(0.15f, 1.28f, p / 0.65f)
                    : Mathf.Lerp(1.28f, 1.05f, (p - 0.65f) / 0.35f);
                if (visualTransform != null)
                {
                    visualTransform.localScale = new Vector3(sc, sc, 1f);
                    visualTransform.localPosition = Vector3.zero;
                }
                if (haloRenderer != null)
                {
                    float hScale = Mathf.Lerp(0.2f, 2.35f, Mathf.Sin(p * Mathf.PI));
                    haloRenderer.transform.localScale = new Vector3(Mathf.Max(0.9f, hScale), Mathf.Max(0.9f, hScale), 1f);
                }
                if (spriteRenderer != null)
                {
                    Color c = Color.white;
                    c.a = Mathf.Clamp01(p * 2.6f);
                    spriteRenderer.color = c;
                }
            }
            else
            {
                float p = (t - 0.55f) / 0.45f;
                float smooth = p * p * (3f - 2f * p);
                Vector3 apex = _awakenStartPos + new Vector3(0f, 1.15f, 0f);
                transform.position = Vector3.Lerp(apex, targetShoulder, smooth);

                if (visualTransform != null)
                    visualTransform.localScale = new Vector3(Mathf.Lerp(1.05f, 1.0f, smooth), Mathf.Lerp(1.05f, 1.0f, smooth), 1f);
                if (haloRenderer != null)
                {
                    float hScale = Mathf.Lerp(1.5f, 1.0f, smooth);
                    haloRenderer.transform.localScale = new Vector3(hScale, hScale, 1f);
                }
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
            }

            if (_awakenTimer >= AwakenDuration)
            {
                _isPlayingAwakenAnim = false;
                if (visualTransform != null) visualTransform.localScale = Vector3.one;
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
                if (_shadowRenderer != null) _shadowRenderer.enabled = true;
                _lastPos = transform.position;
            }
        }

        private void UpdateSpriteAnimation(float dt)
        {
            if (spriteRenderer == null) return;
            bool isPulsing = (_pulseRemaining > 0f || _isPlayingAwakenAnim) && pulseFrames != null && pulseFrames.Length > 0;
            Sprite[] active = isPulsing ? pulseFrames : floatFrames;
            if (active == null || active.Length == 0) return;

            float fps = isPulsing ? pulseFps : floatFps;
            _animTimer += dt;
            float interval = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= interval)
            {
                _animTimer -= interval;
                _frameIdx = (_frameIdx + 1) % active.Length;
            }
            int safeIdx = _frameIdx % active.Length;
            if (active[safeIdx] != null) spriteRenderer.sprite = active[safeIdx];
        }

        public void PlayMoonPulseAnimation(float duration = 0.7f) => TriggerPulse(duration);

        public void TriggerPulse(float duration = 0.7f)
        {
            _pulseRemaining = Mathf.Max(_pulseRemaining, duration);
            _frameIdx = 0;
            _animTimer = 0f;
        }

        public void SetHaloRadiusScale(float scale)
        {
            if (haloRenderer != null)
                haloRenderer.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
""",

    "Assets/Scripts/Community/MoonpetalBloomVFX.cs": r"""using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Community
{
    public class MoonpetalBloomVFX : MonoBehaviour
    {
        [SerializeField] private int sparkleCount = 8;
        [SerializeField] private float orbitRadius = 1.05f;

        private readonly List<Transform> _sparkles = new List<Transform>();
        private readonly List<SpriteRenderer> _sparkleRenderers = new List<SpriteRenderer>();
        private readonly List<float> _phases = new List<float>();
        private bool _isBloomed;
        private Sprite _dotSprite;

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += RefreshState;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= RefreshState;
        }

        private void Start()
        {
            BuildSparkles();
            RefreshState();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == "flag_nv1_completed") RefreshState();
        }

        private void RefreshState()
        {
            BuildSparkles();
            _isBloomed = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                         GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            for (int i = 0; i < _sparkleRenderers.Count; i++)
            {
                if (_sparkleRenderers[i] != null) _sparkleRenderers[i].enabled = _isBloomed;
            }
        }

        private void Update()
        {
            if (!_isBloomed) return;
            float t = Time.time;
            for (int i = 0; i < _sparkles.Count; i++)
            {
                if (_sparkles[i] == null) continue;
                float p = _phases[i];
                float ang = t * 0.65f + p;
                float r = orbitRadius + Mathf.Sin(t * 1.6f + p * 2f) * 0.22f;
                float x = Mathf.Cos(ang) * r;
                float y = Mathf.Sin(ang) * r * 0.72f + 0.18f + Mathf.Sin(t * 2.4f + p) * 0.12f;
                _sparkles[i].localPosition = new Vector3(x, y, 0f);

                float pulse = 0.55f + 0.45f * Mathf.Sin(t * 3.4f + p * 3f);
                float scale = 0.16f + pulse * 0.14f;
                _sparkles[i].localScale = new Vector3(scale, scale, 1f);

                if (_sparkleRenderers[i] != null)
                {
                    Color c = _sparkleRenderers[i].color;
                    c.a = 0.35f + pulse * 0.65f;
                    _sparkleRenderers[i].color = c;
                }
            }
        }

        private void BuildSparkles()
        {
            if (_sparkles.Count > 0) return;
            _dotSprite = CreateGlowDotSprite();

            for (int i = 0; i < sparkleCount; i++)
            {
                GameObject go = new GameObject("MoonpetalSparkle_" + i);
                go.transform.SetParent(transform, false);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = _dotSprite;
                sr.sortingLayerName = "WeatherVFX";
                sr.sortingOrder = 12;
                sr.color = (i % 2 == 0)
                    ? new Color(0.68f, 0.94f, 1.0f, 0.85f)
                    : new Color(1.0f, 0.94f, 0.62f, 0.85f);
                sr.enabled = false;

                _sparkles.Add(go.transform);
                _sparkleRenderers.Add(sr);
                _phases.Add((Mathf.PI * 2f / sparkleCount) * i);
            }
        }

        private Sprite CreateGlowDotSprite()
        {
            const int s = 16;
            Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float half = (s - 1) * 0.5f;
            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float dx = (x - half) / half;
                    float dy = (y - half) / half;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(1f - d);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 16f);
        }
    }
}
""",

    "Assets/Scripts/Audio/AudioManager.cs": r"""using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip bgmCozyClip;
        [SerializeField] private AudioClip sfxUiClick;
        [SerializeField] private AudioClip sfxTool;
        [SerializeField] private AudioClip sfxSlash;
        [SerializeField] private AudioClip sfxHit;
        [SerializeField] private AudioClip sfxPickup;
        [SerializeField] private AudioClip sfxBloom;

        public bool IsMuted { get; private set; }

        private int _lastStamina = 100;
        private readonly Dictionary<string, AudioClip> _voiceBlips = new Dictionary<string, AudioClip>();
        private float _lastBlipTime;
        private float _bgmBaseVolume = 0.25f;
        private float _sfxVolumeScale = 1f;

        private void Awake()
        {
            BuildCharacterVoiceBlips();
        }

        private void OnEnable()
        {
            GameEventBus.OnItemChanged += HandleItemChanged;
            GameEventBus.OnEnemyDefeated += HandleEnemyDefeated;
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnStaminaChanged += HandleStaminaChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnItemChanged -= HandleItemChanged;
            GameEventBus.OnEnemyDefeated -= HandleEnemyDefeated;
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnStaminaChanged -= HandleStaminaChanged;
        }

        private void Start()
        {
            if (bgmSource != null && bgmCozyClip != null)
            {
                bgmSource.clip = bgmCozyClip;
                bgmSource.loop = true;
                bgmSource.volume = _bgmBaseVolume;
                bgmSource.Play();
            }
            GameSettingsManager.ApplyAudioAndFlags();
        }

        private void BuildCharacterVoiceBlips()
        {
            _voiceBlips["wen"]  = CreateSoftBlipClip("blip_wen",  520f, 0.045f);
            _voiceBlips["lumi"] = CreateSoftBlipClip("blip_lumi", 790f, 0.042f);
            _voiceBlips["bach"] = CreateSoftBlipClip("blip_bach", 215f, 0.055f);
            _voiceBlips["hana"] = CreateSoftBlipClip("blip_hana", 440f, 0.048f);
            _voiceBlips["toby"] = CreateSoftBlipClip("blip_toby", 630f, 0.040f);
            _voiceBlips["otis"] = CreateSoftBlipClip("blip_otis", 295f, 0.050f);
        }

        private static AudioClip CreateSoftBlipClip(string clipName, float freqHz, float durationSec)
        {
            int sampleRate = 22050;
            int samples = Mathf.Max(256, Mathf.RoundToInt(sampleRate * durationSec));
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float norm = (float)i / samples;
                float env = Mathf.Sin(norm * Mathf.PI);
                float tone = Mathf.Sin(2f * Mathf.PI * freqHz * t) * 0.7f
                           + Mathf.Sin(2f * Mathf.PI * (freqHz * 1.5f) * t) * 0.2f;
                data[i] = tone * env * 0.35f;
            }
            AudioClip clip = AudioClip.Create(clipName, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public void PlayVoiceBlip(string speakerId)
        {
            if (IsMuted || sfxSource == null) return;
            if (Time.unscaledTime - _lastBlipTime < 0.055f) return;
            _lastBlipTime = Time.unscaledTime;

            string key = string.IsNullOrEmpty(speakerId) ? "wen" : speakerId.ToLowerInvariant();
            if (!_voiceBlips.TryGetValue(key, out AudioClip clip))
                clip = _voiceBlips.TryGetValue("wen", out AudioClip fb) ? fb : null;
            if (clip == null) return;

            float prevPitch = sfxSource.pitch;
            sfxSource.pitch = Random.Range(0.94f, 1.07f);
            sfxSource.PlayOneShot(clip, 0.24f);
            sfxSource.pitch = prevPitch;
        }

        public void ToggleMute()
        {
            SetMuted(!IsMuted);
        }

        public void SetBgmVolume(float normalizedVolume)
        {
            _bgmBaseVolume = Mathf.Clamp01(normalizedVolume) * 0.40f;
            if (bgmSource != null) bgmSource.volume = _bgmBaseVolume;
        }

        public void SetSfxVolume(float normalizedVolume)
        {
            _sfxVolumeScale = Mathf.Clamp01(normalizedVolume);
            if (sfxSource != null) sfxSource.volume = _sfxVolumeScale;
        }

        public void SetMuted(bool muted)
        {
            IsMuted = muted;
            if (bgmSource != null) bgmSource.mute = IsMuted;
            if (sfxSource != null) sfxSource.mute = IsMuted;
        }

        public void PlaySFX(AudioClip clip, float volume = 0.55f)
        {
            if (IsMuted || sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        public void PlayUIClick() => PlaySFX(sfxUiClick, 0.45f);

        private void HandleItemChanged(string _, int __) => PlaySFX(sfxPickup, 0.45f);
        private void HandleEnemyDefeated(string _) => PlaySFX(sfxHit, 0.65f);

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            if (flagId == "flag_nv1_completed") PlaySFX(sfxBloom, 0.80f);
            else PlaySFX(sfxUiClick, 0.40f);
        }

        private void HandleStaminaChanged(int current, int max)
        {
            if (current < _lastStamina)
            {
                int diff = _lastStamina - current;
                PlaySFX(diff == 2 ? sfxSlash : sfxTool, 0.40f);
            }
            _lastStamina = current;
        }
    }
}
""",

    "Assets/Scripts/Narrative/TutorialManager.cs": r"""using System.Collections;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.Narrative
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [SerializeField] private TutorialHintHUDView hintView;
        [SerializeField] private float idleReminderDelay = 22f;

        private int _currentStep = -1;
        private float _idleTimer;
        private bool _remindedCurrentStep;
        private Vector3 _lastPlayerPos;
        private bool _hasLastPos;
        private PlayerController2D _player;
        private Coroutine _transitionRoutine;

        public const string FLAG_SKIPPED = "flag_tutorial_skipped";
        public const string FLAG_FINISHED = "flag_guided_tutorial_finished";

        public bool IsTutorialEnabled => !HasFlag(FLAG_SKIPPED) && !HasFlag(FLAG_FINISHED);

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnGameStateChanged += HandleGameStateChanged;
            GameEventBus.OnSaveLoaded += HandleSaveLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnGameStateChanged -= HandleGameStateChanged;
            GameEventBus.OnSaveLoaded -= HandleSaveLoaded;
        }

        private void Start()
        {
            if (hintView == null)
                hintView = UnityEngine.Object.FindAnyObjectByType<TutorialHintHUDView>(FindObjectsInactive.Include);
            RefreshStep(false);
        }

        private void HandleSaveLoaded()
        {
            RefreshStep(false);
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            bool gameplayActive = GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive || !IsTutorialEnabled || _currentStep < 0)
            {
                if (hintView != null) hintView.SetVisible(false);
                return;
            }
            if (hintView != null && !hintView.IsShowingCompletion)
                hintView.SetVisible(true);
            TrackIdleReminder();
        }

        private void TrackIdleReminder()
        {
            if (_remindedCurrentStep || _currentStep < 0) return;
            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player == null) return;
                _lastPlayerPos = _player.transform.position;
                _hasLastPos = true;
            }

            float moved = _hasLastPos ? Vector3.Distance(_player.transform.position, _lastPlayerPos) : 0f;
            if (moved > 0.35f)
            {
                _lastPlayerPos = _player.transform.position;
                _idleTimer = Mathf.Max(0f, _idleTimer - Time.deltaTime * 0.5f);
            }
            else
            {
                _idleTimer += Time.deltaTime;
            }

            if (_idleTimer >= idleReminderDelay)
            {
                _remindedCurrentStep = true;
                string reminder = GetIdleReminder(_currentStep);
                if (!string.IsNullOrEmpty(reminder))
                    GameEventBus.RaiseToastRequested(reminder);
            }
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (flagId == FLAG_SKIPPED || flagId == FLAG_FINISHED)
            {
                RefreshStep(false);
                return;
            }
            RefreshStep(true);
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (hintView == null) return;
            if (state != GameState.Gameplay) hintView.SetVisible(false);
            else RefreshStep(false);
        }

        public void SetTutorialSkipped(bool skip)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            GameRoot.Instance.Flags.SetFlag(FLAG_SKIPPED, skip);
            if (!skip && HasFlag(FLAG_FINISHED))
            {
                int next = ComputeActiveStep();
                if (next >= 0) GameRoot.Instance.Flags.SetFlag(FLAG_FINISHED, false);
            }
            GameEventBus.RaiseToastRequested(skip
                ? "Đã ẩn gợi ý hướng dẫn. Bạn có thể bật lại trong Pause Menu [Esc]."
                : "Đã bật lại gợi ý hướng dẫn.");
            RefreshStep(false);
        }

        public void ToggleTutorialEnabled() => SetTutorialSkipped(IsTutorialEnabled);

        public void RefreshStep(bool celebratePrevious)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            if (HasFlag(FLAG_SKIPPED))
            {
                _currentStep = -1;
                if (hintView != null) hintView.SetVisible(false);
                return;
            }

            int nextStep = ComputeActiveStep();
            if (nextStep < 0)
            {
                bool wasActive = _currentStep >= 0;
                _currentStep = -1;
                if (!HasFlag(FLAG_FINISHED))
                    GameRoot.Instance.Flags.SetFlag(FLAG_FINISHED, true);
                if (celebratePrevious && wasActive && hintView != null)
                    StartStepTransition("Đã nắm vững thao tác cơ bản!", -1);
                else if (hintView != null)
                    hintView.SetVisible(false);
                return;
            }

            bool stepAdvanced = celebratePrevious && _currentStep >= 0 && nextStep > _currentStep;
            _currentStep = nextStep;
            _idleTimer = 0f;
            _remindedCurrentStep = false;

            if (stepAdvanced && hintView != null)
                StartStepTransition("Xong rồi!", nextStep);
            else if (hintView != null)
                ApplyStepToView(nextStep);
        }

        private void StartStepTransition(string completeLabel, int nextStep)
        {
            if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);
            _transitionRoutine = StartCoroutine(TransitionRoutine(completeLabel, nextStep));
        }

        private IEnumerator TransitionRoutine(string completeLabel, int nextStep)
        {
            hintView.ShowCompletedFlash(completeLabel);
            yield return new WaitForSecondsRealtime(1.05f);
            if (nextStep >= 0 && IsTutorialEnabled) ApplyStepToView(nextStep);
            else hintView.SetVisible(false);
            _transitionRoutine = null;
        }

        private void ApplyStepToView(int step)
        {
            if (hintView == null) return;
            hintView.SetHint(GetStepHint(step));
            bool canShow = GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsGameplayActive;
            hintView.SetVisible(canShow);
        }

        private int ComputeActiveStep()
        {
            if (!HasFlag("flag_intro_letter_read")) return 1;
            if (!HasFlag("flag_greenhouse_cleared")) return 2;
            if (!HasFlag("flag_lumi_awakened")) return 3;
            if (!HasFlag("flag_tutorial_done")) return 4;
            if (!HasFlag("flag_met_bach_at_gate")) return 5;
            if (!HasFlag("flag_garden_slimes_cleared")) return 6;
            return -1;
        }

        private string GetStepHint(int step)
        {
            switch (step)
            {
                case 1: return "Di chuyển <b><color=#8B3A1E>WASD</color></b> tới Bàn Gỗ, bấm <b><color=#8B3A1E>E</color></b> đọc thư của bà";
                case 2: return "Chọn Cuốc <b><color=#8B3A1E>1</color></b> hoặc Rìu <b><color=#8B3A1E>3</color></b>, bấm <b><color=#8B3A1E>Chuột trái</color></b> dọn cỏ đá";
                case 3: return "Tới gần Hộp Gỗ Của Bà, bấm <b><color=#8B3A1E>E</color></b> mở hộp";
                case 4: return "Bấm <b><color=#8B3A1E>E</color></b> tại Bồn Ươm Nhỏ để gieo thử hạt";
                case 5: return "Ra cửa dưới Nhà Kính, gặp Ông Bach ở cổng Nam <b><color=#8B3A1E>E</color></b>";
                case 6: return "Cầm Liềm <b><color=#8B3A1E>4</color></b> chém Slime, bấm <b><color=#8B3A1E>Space</color></b> lướt né";
                default: return "";
            }
        }

        private string GetIdleReminder(int step)
        {
            switch (step)
            {
                case 1: return "Wen: Bức thư của bà đang đặt ngay trên bàn gỗ phía trên kia...";
                case 2: return "Wen: Mình cần chọn Cuốc [1] hoặc Rìu [3] rồi bấm chuột trái để dọn cỏ đá.";
                case 3: return "???: Khịt khịt... Mở nắp hộp gỗ ra giúp tớ với, ở trong này bí quá!";
                case 4: return "Lumi: Bồn ươm nhỏ nằm ngay bên phải trong Nhà Kính đó Wen, bấm [E] thử xem!";
                case 5: return "Lumi: Lối ra Làng Chính ở phía dưới Nhà Kính. Ông Bach đang đứng ở cổng Vườn Phía Nam!";
                case 6: return "Lumi: Cầm Liềm [4] lên! Khi Bọt Sương Đen lao tới nhớ bấm [Space] lướt né nhé!";
                default: return "";
            }
        }

        private bool HasFlag(string flagId)
        {
            return GameRoot.Instance != null && GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag(flagId);
        }
    }
}
""",
}


def main():
    print("=== STEP 40.fix 4: FULL VERIFIED OVERWRITE FOR 6 AUDITED SCRIPTS ===")
    for rel, content in FILES.items():
        full = os.path.join(PROJECT_ROOT, rel.replace("/", os.sep))
        if os.path.isfile(full):
            dst = os.path.join(BACKUP_DIR, rel.replace("/", os.sep))
            os.makedirs(os.path.dirname(dst), exist_ok=True)
            shutil.copy2(full, dst)
        with open(full, "w", encoding="utf-8", newline="\n") as f:
            f.write(content.replace("\r\n", "\n"))
        print("WROTE:", rel)
    print("\nHoan tat! Quay lai Unity doi compile 0 loi, bam Play tu MainMenu de kiem thu lai toan bo.")


if __name__ == "__main__":
    main()