using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Farming
{
    public class SeedShelfInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 1.9f;

        public bool CanInteract => gameObject.activeInHierarchy;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => new Vector3(transform.position.x, transform.position.y + 0.9f, 0f);
        public string InteractionPromptLabel => "Lấy Hạt Giống Dự Phòng";

        private void OnEnable()
        {
            InteractionManager.Register(this);
        }

        private void OnDisable()
        {
            InteractionManager.Unregister(this);
        }

        private void Start()
        {
            HideLegacyPrompt();
        }

        public void HideLegacyPrompt()
        {
            if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
        }

        public void Interact()
        {
            TryGiveBackupSeeds();
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
