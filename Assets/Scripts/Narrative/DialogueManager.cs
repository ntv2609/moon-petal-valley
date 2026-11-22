using System;
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

        [Header("Character Standing Portraits")]
        [SerializeField] private Sprite portraitWen;
        [SerializeField] private Sprite portraitLumi;
        [SerializeField] private Sprite portraitBach;
        [SerializeField] private Sprite portraitHana;
        [SerializeField] private Sprite portraitToby;
        [SerializeField] private Sprite portraitOtis;

        private struct RuntimeLine
        {
            public string speaker;
            public string body;
            public string speakerId;
            public Sprite portraitOverride;
        }

        private readonly List<RuntimeLine> _queue = new List<RuntimeLine>();
        private readonly List<string> _recentLog = new List<string>();
        private const int MaxLogEntries = 10;

        private int _currentIndex;
        private DialogueData _activeAsset;
        private Action _onFinishCallback;
        private bool _isPlaying;
        private float _inputLockTimer;
        private GameInputReader _input;

        public bool IsDialogueActive => _isPlaying;
        public IReadOnlyList<string> RecentLog => _recentLog;

        public void StartDialogue(DialogueData data, Action onFinish = null)
        {
            if (data == null || data.lines == null || data.lines.Count == 0) return;

            var flags = GameRoot.Instance?.Flags;
            _queue.Clear();
            for (int i = 0; i < data.lines.Count; i++)
            {
                var line = data.lines[i];
                if (!string.IsNullOrEmpty(line.requiredFlag) && (flags == null || !flags.HasFlag(line.requiredFlag))) continue;
                if (!string.IsNullOrEmpty(line.blockedFlag) && flags != null && flags.HasFlag(line.blockedFlag)) continue;

                _queue.Add(new RuntimeLine
                {
                    speaker = ResolveText(line.speakerNameKey),
                    body = ResolveText(line.textKey),
                    speakerId = line.speakerId,
                    portraitOverride = line.portraitOverride
                });
            }

            if (_queue.Count == 0) return;

            _activeAsset = data;
            _onFinishCallback = onFinish;
            BeginDisplay(clearPortraits: true);
        }

        public void StartQuickDialogue(string speaker, string body, Action onFinish = null)
        {
            StartQuickLines(new[] { (speaker, body) }, onFinish);
        }

        public void StartQuickLines((string speaker, string body)[] lines, Action onFinish = null)
        {
            if (lines == null || lines.Length == 0) return;
            _queue.Clear();
            for (int i = 0; i < lines.Length; i++)
            {
                _queue.Add(new RuntimeLine
                {
                    speaker = lines[i].speaker,
                    body = lines[i].body,
                    speakerId = "",
                    portraitOverride = null
                });
            }
            _activeAsset = null;
            _onFinishCallback = onFinish;
            BeginDisplay(clearPortraits: true);
        }

        private void BeginDisplay(bool clearPortraits)
        {
            _currentIndex = 0;
            _isPlaying = true;
            _inputLockTimer = 0.16f;

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.Dialogue);
            }

            if (clearPortraits && dialogueUI != null)
            {
                dialogueUI.Hide();
            }

            ShowCurrentLine(keepOtherSide: false);
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (_inputLockTimer > 0f)
            {
                _inputLockTimer -= Time.unscaledDeltaTime;
                return;
            }

            // Phim L mo/dong Nhat ky 10 cau hoi thoai gan nhat
            if (WasLogKeyPressed())
            {
                dialogueUI?.ToggleLog(_recentLog);
                _inputLockTimer = 0.14f;
                return;
            }

            if (dialogueUI != null && dialogueUI.IsLogOpen)
            {
                if (_input != null && (_input.PausePressed || _input.InteractPressed))
                {
                    dialogueUI.CloseLog();
                    _inputLockTimer = 0.14f;
                }
                return;
            }

            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }

            if (dialogueUI != null && dialogueUI.IsShowingChoices)
            {
                if (_input != null && _input.HotbarSlotPressed == 0) dialogueUI.PickChoice(0);
                else if (_input != null && _input.HotbarSlotPressed == 1) dialogueUI.PickChoice(1);
                return;
            }

            bool advance = _input != null && (_input.InteractPressed || _input.DashPressed || _input.ActionPressed);
            if (advance)
            {
                if (dialogueUI != null && dialogueUI.IsTyping)
                {
                    dialogueUI.CompleteTypewriterImmediate();
                    _inputLockTimer = 0.10f;
                    return;
                }

                _currentIndex++;
                if (_currentIndex >= _queue.Count)
                {
                    if (_activeAsset != null && _activeAsset.choices != null && _activeAsset.choices.Count > 0)
                    {
                        PresentChoices(_activeAsset);
                    }
                    else
                    {
                        FinishDialogue();
                    }
                }
                else
                {
                    _inputLockTimer = 0.10f;
                    ShowCurrentLine(keepOtherSide: true);
                }
            }
        }

        private bool WasLogKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.lKey.wasPressedThisFrame)
                return true;
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.L)) return true;
#endif
            return false;
        }

        private void ShowCurrentLine(bool keepOtherSide)
        {
            if (dialogueUI == null || _currentIndex < 0 || _currentIndex >= _queue.Count) return;
            var line = _queue[_currentIndex];
            ResolvePortrait(line.speaker, line.speakerId, line.portraitOverride, out Sprite portrait, out bool alignLeft, out bool isFloating, out string voiceId);
            bool isLast = (_currentIndex == _queue.Count - 1) && (_activeAsset == null || _activeAsset.choices == null || _activeAsset.choices.Count == 0);

            RecordLog(line.speaker, line.body);
            dialogueUI.ShowLine(line.speaker, line.body, portrait, alignLeft, isFloating, isLast, voiceId, keepOtherSide);
        }

        private void RecordLog(string speaker, string body)
        {
            string entry = string.IsNullOrEmpty(speaker) ? body : ("<b>" + speaker + ":</b> " + body);
            _recentLog.Add(entry);
            while (_recentLog.Count > MaxLogEntries) _recentLog.RemoveAt(0);
        }

        private void PresentChoices(DialogueData asset)
        {
            if (dialogueUI == null) { FinishDialogue(); return; }
            string a = asset.choices.Count > 0 ? ResolveText(asset.choices[0].choiceTextKey) : "";
            string b = asset.choices.Count > 1 ? ResolveText(asset.choices[1].choiceTextKey) : "";

            dialogueUI.ShowChoices(a, b, idx =>
            {
                if (idx >= 0 && idx < asset.choices.Count)
                {
                    var picked = asset.choices[idx];
                    if (!string.IsNullOrEmpty(picked.setsFlagOnPick))
                        GameRoot.Instance?.Flags?.SetFlag(picked.setsFlagOnPick, true);
                    if (!string.IsNullOrEmpty(picked.targetNpcId) && picked.affinityDelta != 0)
                        GameRoot.Instance?.Relationships?.AddAffinity(picked.targetNpcId, picked.affinityDelta);

                    ApplyAssetRewards(asset);

                    if (picked.branchDialogue != null)
                    {
                        var cb = _onFinishCallback;
                        _activeAsset = null;
                        _onFinishCallback = null;
                        StartDialogue(picked.branchDialogue, cb);
                        return;
                    }
                }
                FinishDialogue(skipRewards: true);
            });
        }

        private void ResolvePortrait(string speaker, string speakerId, Sprite overrideSprite, out Sprite portrait, out bool alignLeft, out bool isFloating, out string voiceId)
        {
            portrait = overrideSprite;
            alignLeft = false;
            isFloating = false;
            voiceId = "";

            string key = !string.IsNullOrEmpty(speakerId)
                ? speakerId.Trim().ToLowerInvariant()
                : (string.IsNullOrEmpty(speaker) ? "" : speaker.Trim().ToLowerInvariant());

            if (key == "none" || key.Contains("thư của bà"))
            {
                portrait = null;
                return;
            }
            if (key.Contains("wen"))
            {
                alignLeft = true; voiceId = "wen";
                if (portrait == null) portrait = portraitWen;
                return;
            }
            if (key.Contains("lumi"))
            {
                alignLeft = true; isFloating = true; voiceId = "lumi";
                if (portrait == null) portrait = portraitLumi;
                return;
            }
            if (key.Contains("bach") || key.Contains("bách"))
            {
                alignLeft = false; voiceId = "bach";
                if (portrait == null) portrait = portraitBach;
                return;
            }
            if (key.Contains("hana"))
            {
                alignLeft = false; voiceId = "hana";
                if (portrait == null) portrait = portraitHana;
                return;
            }
            if (key.Contains("toby"))
            {
                alignLeft = false; voiceId = "toby";
                if (portrait == null) portrait = portraitToby;
                return;
            }
            if (key.Contains("otis"))
            {
                alignLeft = false; voiceId = "otis";
                if (portrait == null) portrait = portraitOtis;
                return;
            }
        }

        private void ApplyAssetRewards(DialogueData asset)
        {
            if (asset == null || GameRoot.Instance == null) return;

            if (asset.consumesRequiredItem != null && asset.consumesAmount > 0)
            {
                GameRoot.Instance.Inventory?.RemoveItem(asset.consumesRequiredItem.itemID, asset.consumesAmount);
            }
            if (asset.givesItemOnFinish != null && asset.givesItemAmount > 0)
            {
                GameRoot.Instance.Inventory?.AddItem(asset.givesItemOnFinish, asset.givesItemAmount, true);
            }
            if (asset.extraRewards != null)
            {
                for (int i = 0; i < asset.extraRewards.Count; i++)
                {
                    var r = asset.extraRewards[i];
                    if (r.item != null && r.amount > 0)
                    {
                        GameRoot.Instance.Inventory?.AddItem(r.item, r.amount, true);
                    }
                }
            }
            if (asset.unlocksDiaryPage != null)
            {
                GameRoot.Instance.Diary?.UnlockPage(asset.unlocksDiaryPage.pageID);
            }
            if (!string.IsNullOrEmpty(asset.setsStoryFlagOnFinish))
            {
                GameRoot.Instance.Flags?.SetFlag(asset.setsStoryFlagOnFinish, true);
            }
            if (!string.IsNullOrEmpty(asset.affinityNpcId) && asset.affinityBonusOnFinish != 0)
            {
                GameRoot.Instance.Relationships?.AddAffinity(asset.affinityNpcId, asset.affinityBonusOnFinish);
            }
        }

        private void FinishDialogue(bool skipRewards = false)
        {
            _isPlaying = false;
            if (dialogueUI != null) dialogueUI.Hide();

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }

            if (!skipRewards && _activeAsset != null)
            {
                ApplyAssetRewards(_activeAsset);
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
