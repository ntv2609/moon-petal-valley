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
