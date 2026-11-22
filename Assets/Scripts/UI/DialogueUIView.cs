using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.UI
{
    public class DialogueUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text continueHintText;

        [Header("Character Standing Portraits (Dual Left/Right)")]
        [SerializeField] private Image standingPortraitImage;
        [SerializeField] private RectTransform standingPortraitRect;
        [SerializeField] private Image rightPortraitImage;
        [SerializeField] private RectTransform rightPortraitRect;

        [Header("Choice Buttons (Step 32)")]
        [SerializeField] private GameObject choiceContainer;
        [SerializeField] private Button choiceButtonA;
        [SerializeField] private TMP_Text choiceLabelA;
        [SerializeField] private Button choiceButtonB;
        [SerializeField] private TMP_Text choiceLabelB;

        [Header("Dialogue Log Overlay [L] (Step 32)")]
        [SerializeField] private GameObject logPanelRoot;
        [SerializeField] private TMP_Text logBodyText;
        [SerializeField] private Button btnCloseLog;

        private const float CharsPerSecond = 44f;
        private readonly Color _activeTint = Color.white;
        private readonly Color _dimTint = new Color(0.62f, 0.60f, 0.64f, 0.92f);

        private string _fullBody = "";
        private string _activeVoiceId = "";
        private float _typeTimer;
        private int _visibleChars;
        private bool _isTyping;
        private bool _isLumiOnLeft;
        private float _bobPhase;

        private Action<int> _onChoicePicked;
        private bool _showingChoices;
        private bool _isLogOpen;

        public bool IsTyping => _isTyping;
        public bool IsShowingChoices => _showingChoices;
        public bool IsLogOpen => _isLogOpen;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (standingPortraitImage != null) standingPortraitImage.gameObject.SetActive(false);
            if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false);
            if (choiceContainer != null) choiceContainer.SetActive(false);
            if (logPanelRoot != null) logPanelRoot.SetActive(false);

            if (choiceButtonA != null) choiceButtonA.onClick.AddListener(() => PickChoice(0));
            if (choiceButtonB != null) choiceButtonB.onClick.AddListener(() => PickChoice(1));
            if (btnCloseLog != null) btnCloseLog.onClick.AddListener(CloseLog);
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;

            if (_isTyping && bodyText != null)
            {
                _typeTimer += dt * CharsPerSecond;
                int nextCount = Mathf.Min(_fullBody.Length, Mathf.FloorToInt(_typeTimer));
                if (nextCount > _visibleChars)
                {
                    _visibleChars = nextCount;
                    bodyText.text = _fullBody.Substring(0, _visibleChars);
                    if (_visibleChars > 0 && !char.IsWhiteSpace(_fullBody[_visibleChars - 1]) && !string.IsNullOrEmpty(_activeVoiceId))
                    {
                        GameRoot.Instance?.Audio?.PlayVoiceBlip(_activeVoiceId);
                    }
                }
                if (_visibleChars >= _fullBody.Length)
                {
                    _isTyping = false;
                }
            }

            if (_isLumiOnLeft && standingPortraitRect != null && standingPortraitImage != null && standingPortraitImage.gameObject.activeInHierarchy)
            {
                _bobPhase += dt * 2.6f;
                standingPortraitRect.anchoredPosition = new Vector2(18f, 18f + Mathf.Sin(_bobPhase) * 7f);
            }
        }

        public void CompleteTypewriterImmediate()
        {
            if (!_isTyping) return;
            _isTyping = false;
            _visibleChars = _fullBody.Length;
            if (bodyText != null) bodyText.text = _fullBody;
        }

        public void ShowLine(string speaker, string body, bool isLastLine)
        {
            ShowLine(speaker, body, null, false, false, isLastLine);
        }

        public void ShowLine(string speaker, string body, Sprite portrait, bool alignLeft, bool isFloatingSpirit, bool isLastLine, string voiceId = "", bool keepOtherSide = true)
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            HideChoices();

            if (speakerText != null) speakerText.text = speaker;
            _fullBody = body ?? "";
            _activeVoiceId = voiceId;
            _visibleChars = 0;
            _typeTimer = 0f;
            _isTyping = _fullBody.Length > 0;
            if (bodyText != null) bodyText.text = "";

            if (continueHintText != null)
            {
                continueHintText.text = isLastLine
                    ? "[L] Nhật ký  |  [E / Space] Kết thúc"
                    : "[L] Nhật ký  |  [E / Space] Tiếp tục >>";
            }

            ApplyDualPortraits(portrait, alignLeft, isFloatingSpirit, keepOtherSide);
        }

        private void ApplyDualPortraits(Sprite portrait, bool alignLeft, bool isFloatingSpirit, bool keepOtherSide)
        {
            if (portrait == null)
            {
                if (!keepOtherSide)
                {
                    if (standingPortraitImage != null) standingPortraitImage.gameObject.SetActive(false);
                    if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false);
                }
                else
                {
                    if (standingPortraitImage != null) standingPortraitImage.color = _dimTint;
                    if (rightPortraitImage != null) rightPortraitImage.color = _dimTint;
                }
                return;
            }

            if (alignLeft)
            {
                _isLumiOnLeft = isFloatingSpirit;
                if (standingPortraitImage != null && standingPortraitRect != null)
                {
                    standingPortraitRect.anchorMin = new Vector2(0f, 1f);
                    standingPortraitRect.anchorMax = new Vector2(0f, 1f);
                    standingPortraitRect.pivot = new Vector2(0f, 0f);
                    standingPortraitRect.sizeDelta = isFloatingSpirit ? new Vector2(290f, 285f) : new Vector2(315f, 375f);
                    standingPortraitRect.anchoredPosition = new Vector2(18f, isFloatingSpirit ? 18f : -4f);
                    standingPortraitImage.sprite = portrait;
                    standingPortraitImage.preserveAspect = true;
                    standingPortraitImage.color = _activeTint;
                    standingPortraitImage.gameObject.SetActive(true);
                }
                if (rightPortraitImage != null && rightPortraitImage.gameObject.activeSelf)
                {
                    if (keepOtherSide) rightPortraitImage.color = _dimTint;
                    else rightPortraitImage.gameObject.SetActive(false);
                }
            }
            else
            {
                if (rightPortraitImage != null && rightPortraitRect != null)
                {
                    rightPortraitRect.anchorMin = new Vector2(1f, 1f);
                    rightPortraitRect.anchorMax = new Vector2(1f, 1f);
                    rightPortraitRect.pivot = new Vector2(1f, 0f);
                    rightPortraitRect.sizeDelta = new Vector2(315f, 375f);
                    rightPortraitRect.anchoredPosition = new Vector2(-18f, -4f);
                    rightPortraitImage.sprite = portrait;
                    rightPortraitImage.preserveAspect = true;
                    rightPortraitImage.color = _activeTint;
                    rightPortraitImage.gameObject.SetActive(true);
                }
                if (standingPortraitImage != null && standingPortraitImage.gameObject.activeSelf)
                {
                    if (keepOtherSide) standingPortraitImage.color = _dimTint;
                    else standingPortraitImage.gameObject.SetActive(false);
                }
            }
        }

        public void ShowChoices(string labelA, string labelB, Action<int> onPicked)
        {
            _onChoicePicked = onPicked;
            _showingChoices = true;
            if (choiceContainer != null) choiceContainer.SetActive(true);
            if (choiceLabelA != null) choiceLabelA.text = "1. " + labelA;
            if (choiceLabelB != null) choiceLabelB.text = "2. " + labelB;
            if (choiceButtonA != null) choiceButtonA.gameObject.SetActive(!string.IsNullOrEmpty(labelA));
            if (choiceButtonB != null) choiceButtonB.gameObject.SetActive(!string.IsNullOrEmpty(labelB));
            if (continueHintText != null) continueHintText.text = "Chọn câu trả lời [1 / 2 hoặc Bấm chuột]";
        }

        public void HideChoices()
        {
            _showingChoices = false;
            _onChoicePicked = null;
            if (choiceContainer != null) choiceContainer.SetActive(false);
        }

        public void PickChoice(int idx)
        {
            if (!_showingChoices) return;
            var cb = _onChoicePicked;
            HideChoices();
            GameRoot.Instance?.Audio?.PlayUIClick();
            cb?.Invoke(idx);
        }

        public void ToggleLog(IReadOnlyList<string> recentLines)
        {
            if (_isLogOpen)
            {
                CloseLog();
                return;
            }
            if (logPanelRoot == null || logBodyText == null) return;
            var sb = new StringBuilder();
            if (recentLines == null || recentLines.Count == 0)
            {
                sb.AppendLine("Chưa có hội thoại nào gần đây.");
            }
            else
            {
                for (int i = 0; i < recentLines.Count; i++)
                {
                    sb.AppendLine(recentLines[i]);
                }
            }
            logBodyText.text = sb.ToString().TrimEnd();
            logPanelRoot.SetActive(true);
            _isLogOpen = true;
        }

        public void CloseLog()
        {
            _isLogOpen = false;
            if (logPanelRoot != null) logPanelRoot.SetActive(false);
        }

        public void Hide()
        {
            _isTyping = false;
            _isLumiOnLeft = false;
            HideChoices();
            CloseLog();
            if (standingPortraitImage != null) standingPortraitImage.gameObject.SetActive(false);
            if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false);
            if (panelRoot != null) panelRoot.SetActive(false);
        }
    }
}
