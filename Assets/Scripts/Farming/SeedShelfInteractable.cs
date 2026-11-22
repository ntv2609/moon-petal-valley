using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Farming
{
    public class SeedShelfInteractable : MonoBehaviour
    {
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
                TryGiveBackupSeeds();
            }
        }

        private void TryGiveBackupSeeds()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            int seedsInBag = inv.GetItemCount("item_dew_barley_seed");
            int cropsInBag = inv.GetItemCount("item_dew_barley");
            int planted = FarmGridManager.Instance != null ? FarmGridManager.Instance.CountPlantedCrops("crop_dew_barley") : 0;
            int total = seedsInBag + cropsInBag + planted;

            if (total >= 6)
            {
                GameEventBus.RaiseToastRequested("Bạn đã có đủ Hạt/Cây Lúa Mạch Sương Mai rồi (" + total + "/6).");
                return;
            }

            int give = 6 - total;
            inv.AddItemById("item_dew_barley_seed", give, true);
        }
    }
}
