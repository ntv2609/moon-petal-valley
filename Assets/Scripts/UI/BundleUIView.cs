using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Community;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class BundleUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text itemsStatusText;
        [SerializeField] private TMP_Text helpersStatusText;
        [SerializeField] private Button btnContribute;
        [SerializeField] private Button btnBloom;
        [SerializeField] private Button btnClose;
        [SerializeField] private GameObject victoryPanelRoot;
        [SerializeField] private Button btnVictoryClose;

        private PlantingSpotController _activeSpot;
        private GameInputReader _input;
        private bool _isOpen;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (victoryPanelRoot != null) victoryPanelRoot.SetActive(false);

            if (btnContribute != null) btnContribute.onClick.AddListener(OnClickContribute);
            if (btnBloom != null) btnBloom.onClick.AddListener(OnClickBloom);
            if (btnClose != null) btnClose.onClick.AddListener(Close);
            if (btnVictoryClose != null) btnVictoryClose.onClick.AddListener(CloseVictory);
        }

        private void Update()
        {
            if (!_isOpen) return;
            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input != null && _input.PausePressed) Close();
        }

        public void Open(PlantingSpotController spot)
        {
            _activeSpot = spot;
            _isOpen = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance?.State?.SetState(GameState.BundleUI);
            RefreshView();
        }

        public void Close()
        {
            _isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null &&
                GameRoot.Instance.State.CurrentState == GameState.BundleUI)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void CloseVictory()
        {
            if (victoryPanelRoot != null) victoryPanelRoot.SetActive(false);
            GameRoot.Instance?.State?.ReturnToGameplay();
        }

        private void OnClickContribute()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Bundle == null) return;
            int added = GameRoot.Instance.Bundle.ContributeAllAvailable();
            if (added > 0)
            {
                GameEventBus.RaiseToastRequested("Đã đóng góp " + added + " vật phẩm vào Gói Ký Ức Vườn Xưa!");
            }
            else
            {
                GameEventBus.RaiseToastRequested("Chưa có thêm vật phẩm cần nộp trong túi đồ.");
            }
            RefreshView();
        }

        private void OnClickBloom()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Bundle == null) return;
            if (!GameRoot.Instance.Bundle.CanBloomNow())
            {
                GameEventBus.RaiseToastRequested("Cần nộp đủ 5 vật phẩm VÀ có đủ 4/4 người chung tay!");
                return;
            }

            if (panelRoot != null) panelRoot.SetActive(false);
            _isOpen = false;

            if (_activeSpot != null) _activeSpot.ExecuteBloomClimax();

            if (victoryPanelRoot != null)
            {
                victoryPanelRoot.SetActive(true);
                GameRoot.Instance.State?.SetState(GameState.Cutscene);
            }
            else
            {
                GameRoot.Instance.State?.ReturnToGameplay();
            }
        }

        public void RefreshView()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Bundle == null) return;
            var mgr = GameRoot.Instance.Bundle;
            var bundle = mgr.ActiveBundle;
            var inv = GameRoot.Instance.Inventory;
            var flags = GameRoot.Instance.Flags;
            if (bundle == null) return;

            var sbItems = new StringBuilder();
            sbItems.AppendLine("<b>VẬT PHẨM ĐÓNG GÓP:</b>");
            for (int i = 0; i < bundle.requiredItems.Count; i++)
            {
                var req = bundle.requiredItems[i];
                if (req.item == null) continue;
                int deposited = mgr.GetContributedAmount(req.item.itemID);
                int inBag = inv != null ? inv.GetItemCount(req.item.itemID) : 0;
                string name = inv != null ? inv.GetLocalizedItemName(req.item) : req.item.itemID;
                string mark = deposited >= req.amount ? "[OK] " : "[..] ";
                sbItems.AppendLine(string.Format("{0}{1}: {2}/{3} (Túi: {4})", mark, name, deposited, req.amount, inBag));
            }
            if (itemsStatusText != null) itemsStatusText.text = sbItems.ToString();

            var sbHelpers = new StringBuilder();
            sbHelpers.AppendLine(string.Format("<b>NGƯỜI CHUNG TAY ({0}/{1}):</b>", mgr.GetReadyHelperCount(), mgr.GetTotalRequiredHelpers()));
            string[] names = { "Ông Bách (Đọc đủ 4 trang thư)", "Bà Hana (Giao 4 Lúa Mạch)", "Cậu Bé Toby (Cứu mèo Miu)", "Bác Otis (Sửa Hòm Thư)" };
            for (int i = 0; i < bundle.requiredHelperFlags.Count; i++)
            {
                bool ready = flags != null && flags.HasFlag(bundle.requiredHelperFlags[i]);
                string label = i < names.Length ? names[i] : bundle.requiredHelperFlags[i];
                sbHelpers.AppendLine((ready ? "[SẴN SÀNG] " : "[CHỜ] ") + label);
            }
            if (helpersStatusText != null) helpersStatusText.text = sbHelpers.ToString();

            if (btnBloom != null)
            {
                bool canBloom = mgr.CanBloomNow();
                btnBloom.interactable = canBloom;
                var img = btnBloom.GetComponent<Image>();
                if (img != null) img.color = canBloom ? new Color(0.18f, 0.62f, 0.42f, 1f) : new Color(0.28f, 0.30f, 0.36f, 0.8f);
            }
        }
    }
}
