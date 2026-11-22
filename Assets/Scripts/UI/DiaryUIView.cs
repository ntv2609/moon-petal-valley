using System.Collections.Generic;
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
