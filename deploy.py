# deploy.py — Moonpetal Valley — Step 7
import os
import sys
import shutil
from pathlib import Path
from datetime import datetime

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

PROJECT_ROOT = Path.cwd()
ASSETS_DIR = PROJECT_ROOT / "Assets"
SETTINGS_DIR = PROJECT_ROOT / "ProjectSettings"

if not ASSETS_DIR.is_dir() or not SETTINGS_DIR.is_dir():
    print("[LỖI] Không tìm thấy thư mục 'Assets/' và 'ProjectSettings/' tại thư mục hiện tại.")
    sys.exit(1)

FILES = {
    # =========================================================================
    # 1. DIALOGUE MANAGER & UI VIEW
    # =========================================================================
    "Assets/Scripts/UI/DialogueUIView.cs": r'''using UnityEngine;
using TMPro;

namespace MoonpetalValley.UI
{
    public class DialogueUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text continueHintText;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void ShowLine(string speaker, string body, bool isLastLine)
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            if (speakerText != null) speakerText.text = speaker;
            if (bodyText != null) bodyText.text = body;
            if (continueHintText != null)
            {
                continueHintText.text = isLastLine ? "[E / Space] Kết thúc" : "[E / Space] Tiếp tục >>";
            }
        }

        public void Hide()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }
    }
}
''',

    "Assets/Scripts/Narrative/DialogueManager.cs": r'''using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.Narrative
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private DialogueUIView dialogueUI;

        private struct RuntimeLine
        {
            public string speaker;
            public string body;
        }

        private readonly List<RuntimeLine> _queue = new List<RuntimeLine>();
        private int _currentIndex;
        private DialogueData _activeAsset;
        private Action _onFinishCallback;
        private bool _isPlaying;
        private float _inputLockTimer;
        private GameInputReader _input;

        public bool IsDialogueActive => _isPlaying;

        public void StartDialogue(DialogueData data, Action onFinish = null)
        {
            if (data == null || data.lines == null || data.lines.Count == 0) return;

            _queue.Clear();
            for (int i = 0; i < data.lines.Count; i++)
            {
                string sp = ResolveText(data.lines[i].speakerNameKey);
                string tx = ResolveText(data.lines[i].textKey);
                _queue.Add(new RuntimeLine { speaker = sp, body = tx });
            }

            _activeAsset = data;
            _onFinishCallback = onFinish;
            BeginDisplay();
        }

        public void StartQuickDialogue(string speaker, string body, Action onFinish = null)
        {
            _queue.Clear();
            _queue.Add(new RuntimeLine { speaker = speaker, body = body });
            _activeAsset = null;
            _onFinishCallback = onFinish;
            BeginDisplay();
        }

        private void BeginDisplay()
        {
            _currentIndex = 0;
            _isPlaying = true;
            _inputLockTimer = 0.18f;

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.Dialogue);
            }

            ShowCurrentLine();
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (_inputLockTimer > 0f)
            {
                _inputLockTimer -= Time.unscaledDeltaTime;
                return;
            }

            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }

            bool advance = _input != null && (_input.InteractPressed || _input.DashPressed || _input.ActionPressed);
            if (advance)
            {
                _currentIndex++;
                if (_currentIndex >= _queue.Count)
                {
                    FinishDialogue();
                }
                else
                {
                    _inputLockTimer = 0.12f;
                    ShowCurrentLine();
                }
            }
        }

        private void ShowCurrentLine()
        {
            if (dialogueUI == null || _currentIndex < 0 || _currentIndex >= _queue.Count) return;
            var line = _queue[_currentIndex];
            dialogueUI.ShowLine(line.speaker, line.body, _currentIndex == _queue.Count - 1);
        }

        private void FinishDialogue()
        {
            _isPlaying = false;
            if (dialogueUI != null) dialogueUI.Hide();

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }

            if (_activeAsset != null && GameRoot.Instance != null)
            {
                if (_activeAsset.consumesRequiredItem != null && _activeAsset.consumesAmount > 0)
                {
                    GameRoot.Instance.Inventory?.RemoveItem(_activeAsset.consumesRequiredItem.itemID, _activeAsset.consumesAmount);
                }

                if (_activeAsset.givesItemOnFinish != null && _activeAsset.givesItemAmount > 0)
                {
                    GameRoot.Instance.Inventory?.AddItem(_activeAsset.givesItemOnFinish, _activeAsset.givesItemAmount, true);
                }

                if (_activeAsset.unlocksDiaryPage != null)
                {
                    GameRoot.Instance.Diary?.UnlockPage(_activeAsset.unlocksDiaryPage.pageID);
                }

                if (!string.IsNullOrEmpty(_activeAsset.setsStoryFlagOnFinish))
                {
                    GameRoot.Instance.Flags?.SetFlag(_activeAsset.setsStoryFlagOnFinish, true);
                }
            }

            var cb = _onFinishCallback;
            _activeAsset = null;
            _onFinishCallback = null;
            cb?.Invoke();
        }

        private string ResolveText(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (GameRoot.Instance != null && GameRoot.Instance.Loc != null)
            {
                string val = GameRoot.Instance.Loc.Get(key);
                if (!val.StartsWith("[")) return val;
            }
            return key;
        }
    }
}
''',

    # =========================================================================
    # 2. DIARY MANAGER (ISAVEABLE) & DIARY UI VIEW (KEY B)
    # =========================================================================
    "Assets/Scripts/Narrative/DiaryManager.cs": r'''using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Narrative
{
    public class DiaryManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private List<DiaryPageData> allPages = new List<DiaryPageData>();

        public event Action OnDiaryUpdated;
        private readonly HashSet<string> _unlockedPageIds = new HashSet<string>();

        public string SaveKey => "diary_pages";

        [Serializable]
        private class DiarySaveData
        {
            public List<string> unlockedIds = new List<string>();
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && allPages[i].unlockedByFlag == flagId)
                {
                    UnlockPage(allPages[i].pageID);
                }
            }
        }

        public void UnlockPage(string pageId)
        {
            if (string.IsNullOrEmpty(pageId)) return;
            if (_unlockedPageIds.Add(pageId))
            {
                DiaryPageData page = GetPageById(pageId);
                int num = page != null ? page.pageNumber : _unlockedPageIds.Count;
                GameEventBus.RaiseToastRequested("Đã mở khóa Sổ Tay Của Bà — Trang " + num + "! Bấm [B] để đọc.");
                OnDiaryUpdated?.Invoke();
            }
        }

        public bool HasPage(string pageId) => _unlockedPageIds.Contains(pageId);
        public int UnlockedCount => _unlockedPageIds.Count;
        public bool HasAllFourPages() => _unlockedPageIds.Count >= 4;

        public List<DiaryPageData> GetUnlockedPages()
        {
            var result = new List<DiaryPageData>();
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && _unlockedPageIds.Contains(allPages[i].pageID))
                {
                    result.Add(allPages[i]);
                }
            }
            return result;
        }

        private DiaryPageData GetPageById(string pageId)
        {
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && allPages[i].pageID == pageId) return allPages[i];
            }
            return null;
        }

        public string CaptureStateJson()
        {
            return JsonUtility.ToJson(new DiarySaveData { unlockedIds = new List<string>(_unlockedPageIds) });
        }

        public void RestoreStateJson(string json)
        {
            _unlockedPageIds.Clear();
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<DiarySaveData>(json);
            if (data != null && data.unlockedIds != null)
            {
                for (int i = 0; i < data.unlockedIds.Count; i++) _unlockedPageIds.Add(data.unlockedIds[i]);
            }
            OnDiaryUpdated?.Invoke();
        }
    }
}
''',

    "Assets/Scripts/UI/DiaryUIView.cs": r'''using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class DiaryUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text pageHeaderText;
        [SerializeField] private TMP_Text pageTitleText;
        [SerializeField] private TMP_Text pageBodyText;
        [SerializeField] private Button btnPrev;
        [SerializeField] private Button btnNext;
        [SerializeField] private Button btnClose;

        private GameInputReader _input;
        private bool _isOpen;
        private int _pageIndex;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (btnPrev != null) btnPrev.onClick.AddListener(() => ChangePage(-1));
            if (btnNext != null) btnNext.onClick.AddListener(() => ChangePage(1));
            if (btnClose != null) btnClose.onClick.AddListener(Close);
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input == null) return;

            if (_input.DiaryPressed)
            {
                if (!_isOpen && GameRoot.Instance.State.IsGameplayActive) Open();
                else if (_isOpen) Close();
            }
            else if (_isOpen && _input.PausePressed)
            {
                Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance.State.SetState(GameState.DiaryUI);
            RefreshPage();
        }

        public void Close()
        {
            _isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance.State.CurrentState == GameState.DiaryUI)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void ChangePage(int delta)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Diary == null) return;
            var pages = GameRoot.Instance.Diary.GetUnlockedPages();
            if (pages.Count == 0) return;
            _pageIndex = Mathf.Clamp(_pageIndex + delta, 0, pages.Count - 1);
            RefreshPage();
        }

        private void RefreshPage()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Diary == null) return;
            List<DiaryPageData> pages = GameRoot.Instance.Diary.GetUnlockedPages();

            if (pages.Count == 0)
            {
                if (pageHeaderText != null) pageHeaderText.text = "SỔ TAY CỦA BÀ (0/4 trang)";
                if (pageTitleText != null) pageTitleText.text = "Chưa có trang nhật ký nào";
                if (pageBodyText != null) pageBodyText.text = "Hãy dọn dẹp Nhà Kính và mở Chiếc Hộp Gỗ Của Bà để mở khóa Trang 1.";
                return;
            }

            _pageIndex = Mathf.Clamp(_pageIndex, 0, pages.Count - 1);
            DiaryPageData current = pages[_pageIndex];
            var loc = GameRoot.Instance.Loc;

            if (pageHeaderText != null) pageHeaderText.text = string.Format("SỔ TAY CỦA BÀ — Trang {0}/{1} (Đã mở: {2}/4)", current.pageNumber, 4, pages.Count);
            if (pageTitleText != null) pageTitleText.text = loc != null ? loc.Get(current.titleKey) : current.titleKey;
            if (pageBodyText != null) pageBodyText.text = loc != null ? loc.Get(current.bodyTextKey) : current.bodyTextKey;
        }
    }
}
''',

    # =========================================================================
    # 3. QUEST MANAGER & HUD TRACKER
    # =========================================================================
    "Assets/Scripts/Narrative/QuestManager.cs": r'''using System;
using System.Text;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Narrative
{
    public class QuestManager : MonoBehaviour
    {
        public event Action<string> OnObjectiveTextChanged;
        public string CurrentObjectiveSummary { get; private set; } = "";

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnItemChanged += HandleItemChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnItemChanged -= HandleItemChanged;
        }

        private void Start()
        {
            RecomputeObjective();
        }

        private void HandleFlagChanged(string _, bool __) => RecomputeObjective();
        private void HandleItemChanged(string _, int __) => RecomputeObjective();

        public void RecomputeObjective()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            var f = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            string text;
            if (!f.HasFlag("flag_intro_letter_read"))
            {
                text = "<b>MỞ ĐẦU — Thư Của Bà</b>\n- Đọc bức thư trên Bàn Gỗ trong Nhà Kính [E]";
            }
            else if (!f.HasFlag("flag_greenhouse_cleared"))
            {
                text = "<b>MỞ ĐẦU — Dọn Nhà Kính</b>\n- Dùng Cuốc (1) hoặc Rìu (3) dọn 3 cụm cỏ/đá vụn";
            }
            else if (!f.HasFlag("flag_lumi_awakened"))
            {
                text = "<b>MỞ ĐẦU — Hạt Giống Ánh Trăng</b>\n- Mở Hộp Gỗ Của Bà [E] để đánh thức Lumi";
            }
            else if (!f.HasFlag("flag_tutorial_done"))
            {
                text = "<b>MỞ ĐẦU — Thử Nghiệm Đầu Tiên</b>\n- Gieo thử hạt tại Bồn Ươm Nhỏ trong Nhà Kính [E]";
            }
            else if (!f.HasFlag("flag_met_bach_at_gate"))
            {
                text = "<b>NHIỆM VỤ 1 — Khu Vườn Bị Lãng Quên</b>\n- Ra Làng Chính, gặp Ông Bách ở cổng Vườn Phía Nam [E]";
            }
            else if (!f.HasFlag("flag_garden_slimes_cleared"))
            {
                text = "<b>NHIỆM VỤ 1 — Thanh Tẩy Vườn Xưa</b>\n- Cầm Liềm Ánh Trăng (4) diệt 5 Bọt Sương Đen trong Vườn Ông Bách";
            }
            else if (!f.HasFlag("flag_garden_bench_cleaned"))
            {
                text = "<b>NHIỆM VỤ 1 — Kỷ Niệm Cũ</b>\n- Bấm [E] lau sạch Chiếc Ghế Đôi Kỷ Niệm trong vườn";
            }
            else if (!f.HasFlag("flag_hana_bread_done") || !f.HasFlag("flag_toby_cat_saved") || !f.HasFlag("flag_otis_mailbox_fixed"))
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>NHIỆM VỤ 1 — Gom 3 Túi Hạt Di Sản</b>");
                if (!f.HasFlag("flag_hana_bread_done"))
                {
                    int barley = inv != null ? inv.GetItemCount("item_dew_barley") : 0;
                    sb.AppendLine("- Bà Hana: Giao 4 Lúa Mạch Sương Mai (" + barley + "/4)");
                }
                if (!f.HasFlag("flag_toby_cat_saved"))
                {
                    sb.AppendLine("- Cậu Bé Toby: Cứu mèo Miu ở Rìa Rừng Phía Đông");
                }
                if (!f.HasFlag("flag_otis_mailbox_fixed"))
                {
                    int wood = inv != null ? inv.GetItemCount("item_branch_wood") : 0;
                    sb.AppendLine("- Bác Otis: Sửa Hòm Thư bằng 10 Gỗ (" + wood + "/10)");
                }
                text = sb.ToString().TrimEnd();
            }
            else if (!f.HasFlag("flag_bach_letters_read"))
            {
                text = "<b>NHIỆM VỤ 1 — Lá Thư Muộn Màng</b>\n- Quay lại gặp Ông Bách đọc đủ 4 Trang Sổ Tay [E]";
            }
            else if (!f.HasFlag("flag_nv1_completed"))
            {
                text = "<b>NHIỆM VỤ 1 — Hoa Nở Lại Vườn Xưa</b>\n- Tới Bồn Hoa Trung Tâm Vườn, mở Khay Đóng Góp & Cùng Gieo Hạt [E]";
            }
            else
            {
                text = "<b>HOÀN THÀNH MVP!</b>\n- Nhận Cánh Hoa Hy Vọng, Sương Héo Úa đã tan biến!\n- Bấm [Q] dùng Xung Ánh Trăng của Lumi.";
            }

            CurrentObjectiveSummary = text;
            OnObjectiveTextChanged?.Invoke(CurrentObjectiveSummary);
        }
    }
}
''',

    "Assets/Scripts/UI/QuestTrackerHUDView.cs": r'''using UnityEngine;
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
''',

    # =========================================================================
    # 4. NPC CONTROLLER & STORY PROP INTERACTABLE
    # =========================================================================
    "Assets/Scripts/Narrative/NPCController.cs": r'''using UnityEngine;
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
''',

    "Assets/Scripts/Narrative/StoryPropInteractable.cs": r'''using UnityEngine;
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
''',

    # =========================================================================
    # 5. UPDATED GAMEROOT.CS (FULL FILE — ADDS DIALOGUE, DIARY, QUESTS)
    # =========================================================================
    "Assets/Scripts/Core/GameRoot.cs": r'''using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;
using MoonpetalValley.Narrative;

namespace MoonpetalValley.Core
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }

        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private StoryFlagManager storyFlagManager;
        [SerializeField] private VNLocalization localization;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private FloatingToastUI toastUI;
        [SerializeField] private RuntimeDebugOverlay debugOverlay;
        [SerializeField] private TimeOfDayManager timeManager;
        [SerializeField] private PlayerStatsController statsController;
        [SerializeField] private SleepAndKnockoutController sleepController;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private DiaryManager diaryManager;
        [SerializeField] private QuestManager questManager;

        private readonly List<ISaveable> _saveables = new List<ISaveable>();

        public GameStateManager State => gameStateManager;
        public StoryFlagManager Flags => storyFlagManager;
        public VNLocalization Loc => localization;
        public SceneLoader Scenes => sceneLoader;
        public FloatingToastUI Toast => toastUI;
        public RuntimeDebugOverlay DebugOverlay => debugOverlay;
        public TimeOfDayManager Time => timeManager;
        public PlayerStatsController Stats => statsController;
        public SleepAndKnockoutController Sleep => sleepController;
        public InventoryManager Inventory => inventoryManager;
        public DialogueManager Dialogue => dialogueManager;
        public DiaryManager Diary => diaryManager;
        public QuestManager Quests => questManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (localization != null) localization.Initialize();

            if (storyFlagManager != null) RegisterSaveable(storyFlagManager);
            if (timeManager != null) RegisterSaveable(timeManager);
            if (statsController != null) RegisterSaveable(statsController);
            if (inventoryManager != null) RegisterSaveable(inventoryManager);
            if (diaryManager != null) RegisterSaveable(diaryManager);

            Debug.Log("[MV] GameRoot khởi tạo thành công.");
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (saveable == null) return;
            if (!_saveables.Contains(saveable)) _saveables.Add(saveable);
        }

        public IReadOnlyList<ISaveable> GetSaveables() => _saveables;
    }
}
''',

    # =========================================================================
    # 6. UPDATED RUNTIMEDEBUGOVERLAY.CS (FULL FILE — ADDS SKIP PROLOGUE)
    # =========================================================================
    "Assets/Scripts/DebugTools/RuntimeDebugOverlay.cs": r'''using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Farming;

namespace MoonpetalValley.DebugTools
{
    public class RuntimeDebugOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button btnTestToast;
        [SerializeField] private Button btnToggleTestFlag;
        [SerializeField] private Button btnClearFlags;
        [SerializeField] private Button btnTogglePause;
        [SerializeField] private Button btnTpGreenhouse;
        [SerializeField] private Button btnTpVillage;
        [SerializeField] private Button btnTpGarden;
        [SerializeField] private Button btnTpForest;
        [SerializeField] private Button btnTimePlus2H;
        [SerializeField] private Button btnTimeSetDusk;
        [SerializeField] private Button btnTimeNextDay;
        [SerializeField] private Button btnRestoreStats;
        [SerializeField] private Button btnForceExhaust;
        [SerializeField] private Button btnDamageLightHP;
        [SerializeField] private Button btnGiveTestKit;
        [SerializeField] private Button btnWaterAllCrops;
        [SerializeField] private Button btnSkipPrologue;

        private bool _isOpen;
        private float _refreshTimer;
        private const string TestFlagId = "flag_debug_step1_verified";

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnTestToast != null) btnTestToast.onClick.AddListener(OnClickTestToast);
            if (btnToggleTestFlag != null) btnToggleTestFlag.onClick.AddListener(OnClickToggleTestFlag);
            if (btnClearFlags != null) btnClearFlags.onClick.AddListener(OnClickClearFlags);
            if (btnTogglePause != null) btnTogglePause.onClick.AddListener(OnClickTogglePause);
            if (btnTpGreenhouse != null) btnTpGreenhouse.onClick.AddListener(() => TeleportTo("Spawn_Greenhouse"));
            if (btnTpVillage != null) btnTpVillage.onClick.AddListener(() => TeleportTo("Spawn_Village"));
            if (btnTpGarden != null) btnTpGarden.onClick.AddListener(() => TeleportTo("Spawn_BachGarden"));
            if (btnTpForest != null) btnTpForest.onClick.AddListener(() => TeleportTo("Spawn_ForestEdge"));
            if (btnTimePlus2H != null) btnTimePlus2H.onClick.AddListener(() => { GameRoot.Instance?.Time?.AddHours(2f); RefreshStatusText(); });
            if (btnTimeSetDusk != null) btnTimeSetDusk.onClick.AddListener(() => { GameRoot.Instance?.Time?.SetHour(19.83f); RefreshStatusText(); });
            if (btnTimeNextDay != null) btnTimeNextDay.onClick.AddListener(() => { GameRoot.Instance?.Time?.AdvanceToNextDay(); RefreshStatusText(); });
            if (btnRestoreStats != null) btnRestoreStats.onClick.AddListener(() => { GameRoot.Instance?.Stats?.RestoreStats(100, 100, true); RefreshStatusText(); });
            if (btnForceExhaust != null) btnForceExhaust.onClick.AddListener(() => { GameRoot.Instance?.Stats?.ConsumeStamina(999, true); RefreshStatusText(); });
            if (btnDamageLightHP != null) btnDamageLightHP.onClick.AddListener(() => { GameRoot.Instance?.Stats?.TakeLightDamage(25, true); RefreshStatusText(); });
            if (btnGiveTestKit != null) btnGiveTestKit.onClick.AddListener(OnClickGiveTestKit);
            if (btnWaterAllCrops != null) btnWaterAllCrops.onClick.AddListener(() => { FarmGridManager.Instance?.WaterAllTilesDebug(); RefreshStatusText(); });
            if (btnSkipPrologue != null) btnSkipPrologue.onClick.AddListener(OnClickSkipPrologue);
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged += OnGameStateUpdated;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged -= OnGameStateUpdated;
        }

        private void Update()
        {
            if (!Application.isEditor && !Debug.isDebugBuild) return;
            if (WasF1PressedThisFrame()) TogglePanel();

            if (_isOpen)
            {
                _refreshTimer += Time.unscaledDeltaTime;
                if (_refreshTimer >= 0.25f) { _refreshTimer = 0f; RefreshStatusText(); }
            }
        }

        private bool WasF1PressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F1)) return true;
#endif
            return false;
        }

        public void TogglePanel()
        {
            _isOpen = !_isOpen;
            if (panelRoot != null) panelRoot.SetActive(_isOpen);
            if (_isOpen) RefreshStatusText();
        }

        private void TeleportTo(string spawnId)
        {
            if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo(spawnId);
                RefreshStatusText();
            }
        }

        private void OnClickGiveTestKit()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            inv.AddItemById("item_sunshine_bread", 2, false);
            inv.AddItemById("item_branch_wood", 10, false);
            inv.AddItemById("item_dew_barley", 4, false);
            inv.AddItemById("item_dew_barley_seed", 6, false);
            GameEventBus.RaiseToastRequested("Đã thêm Bộ Test (2 Bánh Mì, 10 Gỗ, 4 Lúa Mạch, 6 Hạt)!");
            RefreshStatusText();
        }

        private void OnClickSkipPrologue()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            var f = GameRoot.Instance.Flags;
            f.SetFlag("flag_intro_letter_read", true);
            f.SetFlag("flag_greenhouse_cleared", true);
            f.SetFlag("flag_lumi_awakened", true);
            f.SetFlag("flag_solo_plant_failed", true);
            f.SetFlag("flag_tutorial_done", true);
            GameRoot.Instance.Inventory?.AddItemById("item_moonpetal_seed_core", 1, false);
            GameEventBus.RaiseToastRequested("Đã bỏ qua Mở Đầu — sẵn sàng gặp Ông Bách làm Nhiệm Vụ 1!");
            RefreshStatusText();
        }

        private void OnClickTestToast()
        {
            GameEventBus.RaiseToastRequested("Kiểm thử thông báo tiếng Việt có dấu thành công!");
        }

        private void OnClickToggleTestFlag()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            GameRoot.Instance.Flags.SetFlag(TestFlagId, !GameRoot.Instance.Flags.HasFlag(TestFlagId));
            RefreshStatusText();
        }

        private void OnClickClearFlags() { GameRoot.Instance?.Flags?.ClearAllFlags(); RefreshStatusText(); }

        private void OnClickTogglePause()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            var next = GameRoot.Instance.State.IsGameplayActive ? GameState.PauseMenu : GameState.Gameplay;
            GameRoot.Instance.State.SetState(next);
            RefreshStatusText();
        }

        private void OnAnyStateUpdated(string _, bool __) => RefreshStatusText();
        private void OnGameStateUpdated(GameState _) => RefreshStatusText();

        public void RefreshStatusText()
        {
            if (statusText == null || GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("<b>BẢNG DEBUG MOONPETAL (F1)</b>");
            if (GameRoot.Instance.Time != null) sb.AppendLine(GameRoot.Instance.Time.GetFormattedClockString());
            if (GameRoot.Instance.Stats != null)
            {
                var st = GameRoot.Instance.Stats;
                sb.AppendLine(string.Format("HP: {0}/{1} | Stamina: {2}/{3}",
                    st.CurrentLightHP, st.MaxLightHP, st.CurrentStamina, st.EffectiveMaxStamina));
            }
            if (GameRoot.Instance.Diary != null)
            {
                sb.AppendLine("Sổ tay: " + GameRoot.Instance.Diary.UnlockedCount + "/4 trang | Flags: " + GameRoot.Instance.Flags.GetAllActiveFlags().Count);
            }
            statusText.text = sb.ToString();
        }
    }
}
''',

    # =========================================================================
    # 7. EDITOR — AUTOBUILDER STEP 7 (4 NPCS, DIALOGUES, 4 DIARY PAGES, 5 PROPS)
    # =========================================================================
    "Assets/Editor/AutoBuilder_Step7.cs": r'''using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step7
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 7/1. Build NPCs, Dialogues, Diary & Quests")]
        public static void BuildStep7()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 7 ===");

            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            List<DiaryPageData> diaryPages = Build4DiaryPages(table);
            var npcs = Build4NPCsAndDialogues(table, diaryPages);

            if (table != null) EditorUtility.SetDirty(table);

            UpgradeGameRootWithNarrativeUI(diaryPages);

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildNPCsInScene(npcs);
            BuildStoryPropsInScene();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 7! Chạy 'Moonpetal/Step 7/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 7/Validate")]
        public static void ValidateStep7()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 7 ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                string[] fields = { "dialogueManager", "diaryManager", "questManager" };
                foreach (string f in fields)
                {
                    if (rootSO.FindProperty(f).objectReferenceValue == null)
                    {
                        Debug.LogError("[MV] GameRoot thiếu: " + f);
                        errors++;
                    }
                }
            }

            var npcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            if (npcs.Length < 4)
            {
                Debug.LogError("[MV] Thiếu NPC trong scene (cần 4, hiện có " + npcs.Length + ").");
                errors++;
            }

            var props = UnityEngine.Object.FindObjectsByType<StoryPropInteractable>(FindObjectsInactive.Include);
            if (props.Length < 5)
            {
                Debug.LogError("[MV] Thiếu StoryPropInteractable trong scene (cần 5, hiện có " + props.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 7: PASS (0 lỗi). 4 NPC, Hội thoại phân nhánh, Sổ Tay 4 trang & 5 StoryProp sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 7: FAIL (" + errors + " lỗi).");
            }
        }

        private static List<DiaryPageData> Build4DiaryPages(StringTableSO table)
        {
            var list = new List<DiaryPageData>();
            list.Add(CreateDiaryPage(table, "diary_page_01", 1, "flag_lumi_awakened",
                "Trang 1 — Lời Dặn Trong Nhà Kính",
                "Wen yêu quý, Hoa Ánh Trăng không bao giờ nở vì một đôi tay đơn độc. Khi sương héo úa phủ lên thung lũng, hãy tìm lại những người hàng xóm xưa và lắng nghe câu chuyện của họ."));
            list.Add(CreateDiaryPage(table, "diary_page_02", 2, "flag_hana_bread_done",
                "Trang 2 — Mẻ Bánh Đầu Xuân Của Hana",
                "Mùa xuân năm ấy, cả làng tụ họp ở vườn ông Bách chia nhau những ổ Bánh Mì Nắng Ấm nóng hổi từ lúa mạch sương mai. Hương bánh làm tan đi cả cái lạnh đầu mùa."));
            list.Add(CreateDiaryPage(table, "diary_page_03", 3, "flag_toby_cat_saved",
                "Trang 3 — Tiếng Cười Quanh Vườn Phía Nam",
                "Ông Bách tuy ít nói nhưng luôn để dành một góc vườn cho lũ trẻ chạy nhảy và lũ mèo nằm sưởi nắng. Chỉ cần có người nhớ đến khu vườn, ông sẽ lại cầm cuốc."));
            list.Add(CreateDiaryPage(table, "diary_page_04", 4, "flag_otis_mailbox_fixed",
                "Trang 4 — Bức Thư Chưa Kịp Gửi Ông Bách",
                "Gửi ông bạn già Bách, cảm ơn ông đã giữ gìn chiếc ghế đôi và những hạt giống xưa. Nếu cháu gái tôi Wen tìm đến, xin ông hãy một lần nữa cùng cháu gieo mầm hy vọng nhé."));
            return list;
        }

        private static DiaryPageData CreateDiaryPage(StringTableSO table, string id, int num, string flag, string title, string body)
        {
            string path = "Assets/Data/Diary/DiaryPage_0" + num + ".asset";
            var page = AssetDatabase.LoadAssetAtPath<DiaryPageData>(path);
            if (page == null)
            {
                page = ScriptableObject.CreateInstance<DiaryPageData>();
                AssetDatabase.CreateAsset(page, path);
            }
            page.pageID = id;
            page.pageNumber = num;
            page.titleKey = "title_" + id;
            page.bodyTextKey = "body_" + id;
            page.unlockedByFlag = flag;

            table?.SetEntry(page.titleKey, title);
            table?.SetEntry(page.bodyTextKey, body);
            EditorUtility.SetDirty(page);
            return page;
        }

        private static Dictionary<string, NPCData> Build4NPCsAndDialogues(StringTableSO table, List<DiaryPageData> pages)
        {
            var result = new Dictionary<string, NPCData>();

            ItemData sickle = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_moon_sickle.asset");
            ItemData barley = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley.asset");
            ItemData bread = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_sunshine_bread.asset");
            ItemData bag1 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_seed_bag_1.asset");
            ItemData bag2 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_seed_bag_2.asset");

            // 1. ÔNG BÁCH
            var dlgBachIntro = CreateDialogue(table, "dlg_bach_intro",
                new[] { ("Ông Bách", "Cháu là Wen, cháu gái của bà cụ Nhà Kính phải không? Khu vườn phía nam này bị Bọt Sương Đen chiếm mất rồi..."),
                        ("Ông Bách", "Cầm lấy chiếc Liềm Ánh Trăng cũ này (ô số 4). Nếu cháu thanh tẩy hết 5 Bọt Sương Đen và lau sạch Chiếc Ghế Đôi, ta sẽ tin thung lũng này còn hy vọng.") },
                "flag_met_bach_at_gate", sickle, 1, null, 0, null);

            var dlgBachReadLetters = CreateDialogue(table, "dlg_bach_read_letters",
                new[] { ("Wen", "Ông Bách ơi, cháu đã tìm lại đủ 4 trang nhật ký và bức thư cuối bà gửi cho ông..."),
                        ("Ông Bách", "Bà ấy vẫn luôn nhớ chiếc ghế đôi này sao... Cảm ơn cháu, Wen. Ta sẽ đứng cạnh Bồn Hoa chờ mọi người cùng gieo hạt!") },
                "flag_bach_letters_read", null, 0, null, 0, null);

            var dlgBachDefault = CreateDialogue(table, "dlg_bach_default",
                new[] { ("Ông Bách", "Khu vườn này cần sự chung tay của cả làng. Cháu hãy giúp Bà Hana, cậu bé Toby và Bác Otis tìm đủ 3 Túi Hạt Di Sản nhé.") },
                "", null, 0, null, 0, null);

            result["npc_bach"] = CreateNPC(table, "npc_bach", "Ông Bách", "Người Làm Vườn Già",
                new Color(0.45f, 0.36f, 0.28f), new Color(0.86f, 0.86f, 0.90f),
                dlgBachDefault, "flag_bach_letters_read",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_tutorial_done", blockedByStoryFlag = "flag_met_bach_at_gate", dialogue = dlgBachIntro },
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_garden_bench_cleaned", blockedByStoryFlag = "flag_bach_letters_read", dialogue = dlgBachReadLetters }
                });

            // 2. BÀ HANA
            var dlgHanaComplete = CreateDialogue(table, "dlg_hana_complete",
                new[] { ("Bà Hana", "Ôi, 4 bó Lúa Mạch Sương Mai tươi quá! Mẻ bánh đầu xuân lại thơm lừng cả quảng trường rồi!"),
                        ("Bà Hana", "Tặng cháu 2 ổ Bánh Mì Nắng Ấm (ăn vào hồi lực & xóa mệt mỏi ngay) cùng Túi Hạt Di Sản Số 1 nhé. Cô ra Bồn Hoa chờ cháu!") },
                "flag_hana_bread_done", bag1, 1, barley, 4, pages[1]);

            var dlgHanaGiveBreadBonus = CreateDialogue(table, "dlg_hana_after",
                new[] { ("Bà Hana", "Cô đã sẵn sàng cùng cháu và Ông Bách gieo hạt tại Bồn Hoa Trung Tâm Vườn rồi đó!") },
                "", bread, 2, null, 0, null);

            var dlgHanaDefault = CreateDialogue(table, "dlg_hana_default",
                new[] { ("Bà Hana", "Chào Wen! Nếu cháu trồng được 4 cây Lúa Mạch Sương Mai trong Nhà Kính mang tới đây, cô sẽ nướng Bánh Mì Nắng Ấm và trao Túi Hạt Di Sản Số 1 cho cháu.") },
                "flag_hana_quest_started", null, 0, null, 0, null);

            result["npc_hana"] = CreateNPC(table, "npc_hana", "Bà Hana", "Chủ Tiệm Bánh",
                new Color(0.86f, 0.44f, 0.40f), new Color(0.98f, 0.92f, 0.80f),
                dlgHanaDefault, "flag_hana_bread_done",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_met_bach_at_gate", blockedByStoryFlag = "flag_hana_bread_done", dialogue = dlgHanaComplete },
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_hana_bread_done", blockedByStoryFlag = "flag_hana_bread_bonus_taken", dialogue = dlgHanaGiveBreadBonus }
                });
            dlgHanaGiveBreadBonus.setsStoryFlagOnFinish = "flag_hana_bread_bonus_taken";
            EditorUtility.SetDirty(dlgHanaGiveBreadBonus);

            // 3. CẬU BÉ TOBY
            var dlgTobyComplete = CreateDialogue(table, "dlg_toby_complete",
                new[] { ("Cậu Bé Toby", "Meo meo! Chị Wen đuổi hết 3 Bọt Sương Đen cứu bé Miu rồi! Em cảm ơn chị nhiều lắm!"),
                        ("Cậu Bé Toby", "Em gửi chị Túi Hạt Di Sản Số 2 và trang sổ em nhặt được. Em chạy ra Bồn Hoa phụ chị ngay đây!") },
                "flag_toby_cat_saved", bag2, 1, null, 0, pages[2]);

            var dlgTobyDefault = CreateDialogue(table, "dlg_toby_default",
                new[] { ("Cậu Bé Toby", "Chị Wen ơi, chú mèo Miu của em bị 3 con Bọt Sương Đen vây ở Rìa Rừng Phía Đông rồi! Chị cầm Liềm Ánh Trăng cứu bé giúp em với!") },
                "", null, 0, null, 0, null);

            result["npc_toby"] = CreateNPC(table, "npc_toby", "Cậu Bé Toby", "Cư Dân Làng",
                new Color(0.28f, 0.68f, 0.46f), new Color(0.95f, 0.78f, 0.36f),
                dlgTobyDefault, "flag_toby_cat_saved",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_toby_cat_slimes_cleared", blockedByStoryFlag = "flag_toby_cat_saved", dialogue = dlgTobyComplete }
                });

            // 4. BÁC OTIS
            var dlgOtisDone = CreateDialogue(table, "dlg_otis_done",
                new[] { ("Bác Otis", "Hòm thư chắc chắn như mới rồi! Cảm ơn cháu Wen, bác ra đứng cạnh Bồn Hoa chờ mọi người cùng gieo hạt đây!") },
                "", null, 0, null, 0, null);

            var dlgOtisDefault = CreateDialogue(table, "dlg_otis_default",
                new[] { ("Bác Otis", "Hòm thư gỗ ngay cạnh bác bị sương mục hỏng mất rồi. Cháu chặt 10 Gỗ Nhánh Cây quanh làng rồi bấm [E] vào Hòm Thư để sửa giúp bác nhé, bên trong có Túi Hạt Di Sản Số 3 và thư của bà cháu đấy!") },
                "", null, 0, null, 0, null);

            result["npc_otis"] = CreateNPC(table, "npc_otis", "Bác Otis", "Người Đưa Thư",
                new Color(0.26f, 0.46f, 0.76f), new Color(0.92f, 0.84f, 0.50f),
                dlgOtisDefault, "flag_otis_mailbox_fixed",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_otis_mailbox_fixed", blockedByStoryFlag = "", dialogue = dlgOtisDone }
                });

            return result;
        }

        private static DialogueData CreateDialogue(
            StringTableSO table, string id, (string speaker, string text)[] lines,
            string setFlag, ItemData giveItem, int giveAmt, ItemData consumeItem, int consumeAmt, DiaryPageData unlockPage)
        {
            string path = "Assets/Data/Dialogues/Dlg_" + id + ".asset";
            var dlg = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
            if (dlg == null)
            {
                dlg = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(dlg, path);
            }

            dlg.dialogueID = id;
            dlg.lines.Clear();
            for (int i = 0; i < lines.Length; i++)
            {
                string spKey = id + "_sp_" + i;
                string txKey = id + "_tx_" + i;
                table?.SetEntry(spKey, lines[i].speaker);
                table?.SetEntry(txKey, lines[i].text);
                dlg.lines.Add(new DialogueLine { speakerNameKey = spKey, textKey = txKey });
            }

            dlg.setsStoryFlagOnFinish = setFlag;
            dlg.givesItemOnFinish = giveItem;
            dlg.givesItemAmount = giveAmt;
            dlg.consumesRequiredItem = consumeItem;
            dlg.consumesAmount = consumeAmt;
            dlg.unlocksDiaryPage = unlockPage;
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static NPCData CreateNPC(
            StringTableSO table, string id, string vnName, string role,
            Color coatColor, Color hairColor, DialogueData defaultDlg, string helperFlag,
            List<ConditionalDialogueEntry> conditionals)
        {
            Sprite spr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_" + id + ".png", 16, 24, tex =>
            {
                PlaceholderGeneratorUtility.FillRect(tex, 5, 1, 2, 3, new Color(0.24f, 0.16f, 0.12f));
                PlaceholderGeneratorUtility.FillRect(tex, 9, 1, 2, 3, new Color(0.24f, 0.16f, 0.12f));
                PlaceholderGeneratorUtility.FillRect(tex, 4, 4, 8, 9, coatColor);
                PlaceholderGeneratorUtility.FillRect(tex, 4, 13, 8, 6, new Color(0.96f, 0.84f, 0.72f));
                tex.SetPixel(6, 15, Color.black);
                tex.SetPixel(9, 15, Color.black);
                PlaceholderGeneratorUtility.FillRect(tex, 3, 18, 10, 3, hairColor);
            });

            string path = "Assets/Data/NPCs/NPC_" + id + ".asset";
            var npc = AssetDatabase.LoadAssetAtPath<NPCData>(path);
            if (npc == null)
            {
                npc = ScriptableObject.CreateInstance<NPCData>();
                AssetDatabase.CreateAsset(npc, path);
            }
            npc.npcID = id;
            npc.displayNameKey = "name_" + id;
            npc.roleTitleKey = "role_" + id;
            npc.worldSprite = spr;
            npc.defaultDialogue = defaultDlg;
            npc.conditionalDialogues = conditionals;
            npc.isHelperForBundleID = "bundle_forgotten_garden";
            npc.helperReadyStoryFlag = helperFlag;

            table?.SetEntry(npc.displayNameKey, vnName);
            table?.SetEntry(npc.roleTitleKey, role);
            EditorUtility.SetDirty(npc);
            return npc;
        }

        private static void UpgradeGameRootWithNarrativeUI(List<DiaryPageData> diaryPages)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var dlgMgr = root.GetComponent<DialogueManager>() ?? root.AddComponent<DialogueManager>();
            var diaryMgr = root.GetComponent<DiaryManager>() ?? root.AddComponent<DiaryManager>();
            var questMgr = root.GetComponent<QuestManager>() ?? root.AddComponent<QuestManager>();

            var dSO = new SerializedObject(diaryMgr);
            var pagesProp = dSO.FindProperty("allPages");
            pagesProp.ClearArray();
            for (int i = 0; i < diaryPages.Count; i++)
            {
                pagesProp.InsertArrayElementAtIndex(i);
                pagesProp.GetArrayElementAtIndex(i).objectReferenceValue = diaryPages[i];
            }
            dSO.ApplyModifiedPropertiesWithoutUndo();

            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                // 1. Quest Tracker Panel (Top-Right below Clock)
                Transform oldQuest = hudCanvas.Find("QuestTrackerPanel");
                if (oldQuest != null) UnityEngine.Object.DestroyImmediate(oldQuest.gameObject);
                GameObject qPanel = CreatePanel("QuestTrackerPanel", hudCanvas, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -92f), new Vector2(380f, 128f), new Color(0.08f, 0.12f, 0.19f, 0.84f));
                qPanel.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
                var qTmp = CreateTMP("ObjectiveText", qPanel.transform, "Mục tiêu...", vnFont, 16, TextAlignmentOptions.TopLeft, Color.white);
                var qView = qPanel.AddComponent<QuestTrackerHUDView>();
                var qSO = new SerializedObject(qView);
                qSO.FindProperty("objectiveText").objectReferenceValue = qTmp;
                qSO.ApplyModifiedPropertiesWithoutUndo();

                // 2. Dialogue Box (Bottom-Center)
                Transform oldDlg = hudCanvas.Find("DialogueUIHost");
                if (oldDlg != null) UnityEngine.Object.DestroyImmediate(oldDlg.gameObject);
                GameObject dlgHost = new GameObject("DialogueUIHost");
                dlgHost.transform.SetParent(hudCanvas, false);
                var dhRect = dlgHost.AddComponent<RectTransform>();
                dhRect.anchorMin = Vector2.zero; dhRect.anchorMax = Vector2.one; dhRect.offsetMin = Vector2.zero; dhRect.offsetMax = Vector2.zero;

                GameObject dlgBox = CreatePanel("DialogueBox", dlgHost.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(980f, 190f), new Color(0.07f, 0.10f, 0.17f, 0.95f));
                dlgBox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                var spTxt = CreateTMP("SpeakerText", dlgBox.transform, "Nhân vật", vnFont, 24, TextAlignmentOptions.TopLeft, new Color(1f, 0.88f, 0.45f, 1f));
                spTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 130f); spTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -14f);
                var bdTxt = CreateTMP("BodyText", dlgBox.transform, "Nội dung...", vnFont, 21, TextAlignmentOptions.TopLeft, Color.white);
                bdTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 44f); bdTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -54f);
                var hintTxt = CreateTMP("ContinueHint", dlgBox.transform, "[E / Space] Tiếp tục", vnFont, 17, TextAlignmentOptions.BottomRight, new Color(0.70f, 0.92f, 0.96f, 1f));
                hintTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 12f); hintTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -140f);

                var dlgView = dlgHost.AddComponent<DialogueUIView>();
                var dvSO = new SerializedObject(dlgView);
                dvSO.FindProperty("panelRoot").objectReferenceValue = dlgBox;
                dvSO.FindProperty("speakerText").objectReferenceValue = spTxt;
                dvSO.FindProperty("bodyText").objectReferenceValue = bdTxt;
                dvSO.FindProperty("continueHintText").objectReferenceValue = hintTxt;
                dvSO.ApplyModifiedPropertiesWithoutUndo();

                var dmSO = new SerializedObject(dlgMgr);
                dmSO.FindProperty("dialogueUI").objectReferenceValue = dlgView;
                dmSO.ApplyModifiedPropertiesWithoutUndo();

                // 3. Diary UI Modal (Key B)
                Transform oldDiary = hudCanvas.Find("DiaryUIHost");
                if (oldDiary != null) UnityEngine.Object.DestroyImmediate(oldDiary.gameObject);
                GameObject diaryHost = new GameObject("DiaryUIHost");
                diaryHost.transform.SetParent(hudCanvas, false);
                var diRect = diaryHost.AddComponent<RectTransform>();
                diRect.anchorMin = Vector2.zero; diRect.anchorMax = Vector2.one; diRect.offsetMin = Vector2.zero; diRect.offsetMax = Vector2.zero;

                GameObject diaryBox = CreatePanel("DiaryBox", diaryHost.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 420f), new Color(0.16f, 0.13f, 0.10f, 0.96f));
                var pHead = CreateTMP("Header", diaryBox.transform, "SỔ TAY CỦA BÀ", vnFont, 20, TextAlignmentOptions.Top, new Color(0.95f, 0.82f, 0.52f, 1f));
                pHead.GetComponent<RectTransform>().offsetMax = new Vector2(-20f, -16f);
                var pTitle = CreateTMP("Title", diaryBox.transform, "Tiêu đề", vnFont, 25, TextAlignmentOptions.TopLeft, Color.white);
                pTitle.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 280f); pTitle.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -64f);
                var pBody = CreateTMP("Body", diaryBox.transform, "Nội dung", vnFont, 20, TextAlignmentOptions.TopLeft, new Color(0.96f, 0.92f, 0.84f, 1f));
                pBody.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 80f); pBody.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -112f);

                Button btnPrev = CreateModalButton("BtnPrev", diaryBox.transform, "< Trang Trước", new Vector2(-200f, 36f), vnFont);
                Button btnClose = CreateModalButton("BtnClose", diaryBox.transform, "Đóng [B]", new Vector2(0f, 36f), vnFont);
                Button btnNext = CreateModalButton("BtnNext", diaryBox.transform, "Trang Sau >", new Vector2(200f, 36f), vnFont);

                var diaryView = diaryHost.AddComponent<DiaryUIView>();
                var diSO = new SerializedObject(diaryView);
                diSO.FindProperty("panelRoot").objectReferenceValue = diaryBox;
                diSO.FindProperty("pageHeaderText").objectReferenceValue = pHead;
                diSO.FindProperty("pageTitleText").objectReferenceValue = pTitle;
                diSO.FindProperty("pageBodyText").objectReferenceValue = pBody;
                diSO.FindProperty("btnPrev").objectReferenceValue = btnPrev;
                diSO.FindProperty("btnNext").objectReferenceValue = btnNext;
                diSO.FindProperty("btnClose").objectReferenceValue = btnClose;
                diSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // Nối GameRoot
            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("dialogueManager").objectReferenceValue = dlgMgr;
            rootSO.FindProperty("diaryManager").objectReferenceValue = diaryMgr;
            rootSO.FindProperty("questManager").objectReferenceValue = questMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // Nút Debug Bỏ qua Mở Đầu
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 920f);
                Transform ex = debugPanel.Find("BtnSkipPrologue");
                Button btnSkip = ex != null ? ex.GetComponent<Button>() : CreateDebugButton(debugPanel, "BtnSkipPrologue", "Bỏ qua Mở Đầu (Vào NV1 ngay)", vnFont);
                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnSkipPrologue").objectReferenceValue = btnSkip;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildNPCsInScene(Dictionary<string, NPCData> npcs)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject old = GameObject.Find("VillageNPCs");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("VillageNPCs");

            SpawnNPC(group.transform, "NPC_Bach", npcs["npc_bach"], new Vector2(0.0f, -3.5f), new Vector2(-1.6f, -8.8f), "[E] Nói chuyện: Ông Bách", vnFont);
            SpawnNPC(group.transform, "NPC_Hana", npcs["npc_hana"], new Vector2(-6.0f, 3.0f), new Vector2(1.8f, -8.8f), "[E] Nói chuyện: Bà Hana", vnFont);
            SpawnNPC(group.transform, "NPC_Toby", npcs["npc_toby"], new Vector2(7.5f, 2.5f), new Vector2(-1.6f, -11.2f), "[E] Nói chuyện: Cậu Bé Toby", vnFont);
            SpawnNPC(group.transform, "NPC_Otis", npcs["npc_otis"], new Vector2(1.2f, 5.5f), new Vector2(1.8f, -11.2f), "[E] Nói chuyện: Bác Otis", vnFont);
        }

        private static void SpawnNPC(Transform parent, string name, NPCData data, Vector2 pos, Vector2 helperPos, string promptText, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.tag = "NPC";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = data.worldSprite;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);

            go.AddComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.7f);
            GameObject prompt = CreateWorldPrompt(go.transform, promptText, font);

            var ctrl = go.AddComponent<NPCController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("npcData").objectReferenceValue = data;
            so.FindProperty("promptObject").objectReferenceValue = prompt;
            so.FindProperty("interactRadius").floatValue = 2.1f;
            so.FindProperty("helperStandPosition").vector2Value = helperPos;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildStoryPropsInScene()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject old = GameObject.Find("StoryPropsGroup");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("StoryPropsGroup");

            Sprite tableSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_table.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 10, new Color(0.54f, 0.36f, 0.20f));
                PlaceholderGeneratorUtility.FillRect(t, 5, 6, 6, 5, new Color(0.98f, 0.95f, 0.85f));
            });
            Sprite boxSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_seedbox.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 10, new Color(0.42f, 0.28f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 6, 6, 4, 4, new Color(0.45f, 0.94f, 0.96f));
            });
            Sprite planterSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_planter.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 2, 12, 12, new Color(0.56f, 0.56f, 0.60f));
                PlaceholderGeneratorUtility.FillRect(t, 4, 4, 8, 8, new Color(0.34f, 0.24f, 0.18f));
            });
            Sprite mailboxSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_mailbox.png", 16, 20, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 7, 1, 2, 10, new Color(0.46f, 0.30f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 3, 10, 10, 7, new Color(0.28f, 0.48f, 0.76f));
                PlaceholderGeneratorUtility.FillRect(t, 12, 13, 2, 4, new Color(0.92f, 0.32f, 0.28f));
            });
            Sprite benchSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_bench.png", 24, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 4, 20, 4, new Color(0.56f, 0.38f, 0.22f));
                PlaceholderGeneratorUtility.FillRect(t, 2, 9, 20, 4, new Color(0.46f, 0.30f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 4, 1, 2, 12, new Color(0.32f, 0.22f, 0.14f));
                PlaceholderGeneratorUtility.FillRect(t, 18, 1, 2, 12, new Color(0.32f, 0.22f, 0.14f));
            });

            CreateStoryProp(group.transform, "GreenhouseLetterTable", "prop_letter_table", new Vector2(-15.5f, 12.5f), tableSpr,
                "[E] Đọc Thư Của Bà", "", "flag_intro_letter_read", "", "", 0, "", 0, 0,
                "Thư Của Bà", "Wen thân yêu, hãy dọn dẹp 3 cụm cỏ dại đá vụn trong Nhà Kính rồi mở Chiếc Hộp Gỗ Của Bà nhé!", "", vnFont);

            CreateStoryProp(group.transform, "GreenhouseSeedBox", "prop_seed_box", new Vector2(-17.2f, 12.5f), boxSpr,
                "[E] Mở Hộp Gỗ Hạt Giống", "flag_greenhouse_cleared", "flag_lumi_awakened", "", "", 0, "item_moonpetal_seed_core", 1, 0,
                "Tinh Linh Lumi", "Oa! Sương lạnh ngoài kia dày quá... Cảm ơn Wen đã đánh thức Lumi! Mình đã mở Trang 1 Sổ Tay Của Bà (bấm [B] để đọc) và tặng bạn 6 Hạt Lúa Mạch Sương Mai!",
                "Hãy dọn sạch 3 cụm cỏ dại/đá vụn trong Nhà Kính trước!", vnFont);

            CreateStoryProp(group.transform, "GreenhouseTestPlanter", "prop_test_planter", new Vector2(-13.8f, 11.2f), planterSpr,
                "[E] Gieo Thử Hạt Giống", "flag_lumi_awakened", "flag_solo_plant_failed", "flag_tutorial_done", "", 0, "", 0, 0,
                "Tinh Linh Lumi", "Hạt giống vừa chạm đất đã bị sương xám phủ kín... Đúng như lời bà dặn: một mình không thể làm hoa nở. Hãy ra Quảng Trường Làng Chính tìm Ông Bách nhé!",
                "Hãy mở Hộp Gỗ Của Bà đánh thức Lumi trước!", vnFont);

            CreateStoryProp(group.transform, "OtisBrokenMailbox", "prop_otis_mailbox", new Vector2(-0.5f, 5.5f), mailboxSpr,
                "[E] Sửa Hòm Thư (10 Gỗ)", "flag_met_bach_at_gate", "flag_otis_mailbox_fixed", "", "item_branch_wood", 10, "item_seed_bag_3", 1, 4,
                "Bác Otis", "Tuyệt vời! Hòm thư sửa xong rồi! Cháu nhận lấy Túi Hạt Di Sản Số 3 và Trang Sổ 4 (bức thư bà cháu chưa kịp gửi Ông Bách) nhé!",
                "Cần gặp Ông Bách trước và có đủ 10 Gỗ Nhánh Cây!", vnFont);

            CreateStoryProp(group.transform, "GardenMemoryBench", "prop_memory_bench", new Vector2(3.0f, -7.2f), benchSpr,
                "[E] Lau Sạch Chiếc Ghế Đôi", "flag_garden_slimes_cleared", "flag_garden_bench_cleaned", "", "", 0, "", 0, 2,
                "Wen", "Chiếc ghế đôi kỷ niệm của Ông Bách đã sạch bóng rêu phong! Giờ mình sẽ giúp Bà Hana, Toby và Bác Otis gom đủ 3 Túi Hạt Di Sản.",
                "Hãy thanh tẩy hết 5 Bọt Sương Đen trong Vườn Ông Bách trước!", vnFont);
        }

        private static void CreateStoryProp(
            Transform parent, string name, string id, Vector2 pos, Sprite spr, string promptLabel,
            string reqFlag, string doneFlag, string doneFlag2, string reqItem, int reqAmt, string rewItem, int rewAmt, int stamCost,
            string speaker, string body, string blocked, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.tag = "Interactable";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr; sr.sortingLayerName = "WorldObjects"; sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);
            go.AddComponent<BoxCollider2D>().size = new Vector2(0.95f, 0.85f);

            GameObject prompt = CreateWorldPrompt(go.transform, promptLabel, font);
            var comp = go.AddComponent<StoryPropInteractable>();
            var so = new SerializedObject(comp);
            so.FindProperty("propId").stringValue = id;
            so.FindProperty("requiredFlag").stringValue = reqFlag;
            so.FindProperty("completedFlag").stringValue = doneFlag;
            so.FindProperty("secondaryCompletedFlag").stringValue = doneFlag2;
            so.FindProperty("requiredItemId").stringValue = reqItem;
            so.FindProperty("requiredItemAmount").intValue = reqAmt;
            so.FindProperty("rewardItemId").stringValue = rewItem;
            so.FindProperty("rewardItemAmount").intValue = rewAmt;
            so.FindProperty("staminaCost").intValue = stamCost;
            so.FindProperty("dialogueSpeaker").stringValue = speaker;
            so.FindProperty("dialogueBody").stringValue = body;
            so.FindProperty("blockedHint").stringValue = blocked;
            so.FindProperty("promptObject").objectReferenceValue = prompt;
            so.FindProperty("interactRadius").floatValue = 1.95f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 aMin, Vector2 aMax, Vector2 pos, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = aMin; r.anchorMax = aMax; r.anchoredPosition = pos; r.sizeDelta = size;
            go.AddComponent<Image>().color = color;
            return go;
        }

        private static TMP_Text CreateTMP(string name, Transform parent, string text, TMP_FontAsset font, float size, TextAlignmentOptions align, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(12f, 8f); r.offsetMax = new Vector2(-12f, -8f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = align; tmp.color = color;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateModalButton(string name, Transform parent, string label, Vector2 anchoredPos, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0f); r.anchorMax = new Vector2(0.5f, 0f);
            r.anchoredPosition = anchoredPos; r.sizeDelta = new Vector2(170f, 40f);
            var img = go.AddComponent<Image>(); img.color = new Color(0.32f, 0.25f, 0.18f, 1f);
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 17, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static Button CreateDebugButton(Transform parent, string name, string label, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
            var img = go.AddComponent<Image>(); img.color = new Color(0.42f, 0.34f, 0.18f, 1f);
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 15, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static GameObject CreateWorldPrompt(Transform parent, string label, TMP_FontAsset font)
        {
            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(parent, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace; canvas.sortingLayerName = "UI"; canvas.sortingOrder = 50;
            promptGo.GetComponent<RectTransform>().sizeDelta = new Vector2(280f, 44f);
            promptGo.AddComponent<Image>().color = new Color(0.08f, 0.12f, 0.18f, 0.90f);
            var tmp = CreateTMP("Text", promptGo.transform, label, font, 20, TextAlignmentOptions.Center, new Color(1f, 0.94f, 0.65f, 1f));
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return promptGo;
        }
    }
}
''',
}

def deploy():
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_root = PROJECT_ROOT / "_deploy_backup" / f"Step_7_{timestamp}"
    backed_up_count = 0

    print("=== MOONPETAL VALLEY — DEPLOY STEP 7 ===")
    for rel_path, content in FILES.items():
        target_path = PROJECT_ROOT / rel_path
        target_path.parent.mkdir(parents=True, exist_ok=True)

        status = "TẠO MỚI"
        if target_path.exists():
            backup_target = backup_root / rel_path
            backup_target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(target_path, backup_target)
            backed_up_count += 1
            status = "GHI ĐÈ "

        normalized = content.replace("\r\n", "\n")
        with open(target_path, "w", encoding="utf-8", newline="\n") as f:
            f.write(normalized)

        size_bytes = target_path.stat().st_size
        print(f"  [{status}] {rel_path} ({size_bytes} bytes)")

        if rel_path.endswith(".cs"):
            open_braces = normalized.count("{")
            close_braces = normalized.count("}")
            if open_braces != close_braces:
                print(f"  [CẢNH BÁO] Lệch dấu ngoặc trong {rel_path}: '{{'={open_braces}, '}}'={close_braces}")

    if backed_up_count > 0:
        print(f"[BACKUP] Đã sao lưu {backed_up_count} file cũ tại: {backup_root}")

    print("\n--- HƯỚNG DẪN TIẾP THEO ---")
    print("1. Quay lại Unity Editor (tắt Play Mode) và chờ compile xong (0 lỗi, 0 warning).")
    print("2. Chạy menu: Moonpetal/Step 7/1. Build NPCs, Dialogues, Diary & Quests.")
    print("3. Chạy menu: Moonpetal/Step 7/Validate rồi bấm Play trải nghiệm Phần Mở Đầu & 4 NPC!")

if __name__ == "__main__":
    deploy()