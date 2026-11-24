using UnityEngine;
using MoonpetalValley.Core;
namespace MoonpetalValley.Chapter3
{
    public class RiverDaisyForageNode : MonoBehaviour, IInteractable {
        [SerializeField] private string itemId = "item_river_daisy";
        [SerializeField] private int yieldAmount = 2;
        [SerializeField] private float interactRadius = 1.6f;
        [SerializeField] private string requiredFlag = "flag_ch2_completed";
        private SpriteRenderer _sr;
        private Collider2D _col;
        private bool _harvestedToday;
        public bool CanInteract {
            get {
                if (_harvestedToday || !IsUnlocked()) return false;
                return GameRoot.Instance != null && GameRoot.Instance.State != null &&
                       GameRoot.Instance.State.IsGameplayActive;
            }
        }
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 0.7f, 0f);
        public string InteractionPromptLabel => "Hái Hoa Cúc Sông [E]";
        private void Awake() {
            _sr = GetComponent<SpriteRenderer>();
            _col = GetComponent<Collider2D>();
        }
        private void OnEnable() {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnDayStarted += HandleDayStarted;
            GameEventBus.OnSaveLoaded += SyncVisuals;
            SyncVisuals();
        }
        private void OnDisable() {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnDayStarted -= HandleDayStarted;
            GameEventBus.OnSaveLoaded -= SyncVisuals;
        }
        private void HandleFlagChanged(string flagId, bool _) {
            if (flagId == requiredFlag) SyncVisuals();
        }
        private void HandleDayStarted(int _) {
            _harvestedToday = false;
            SyncVisuals();
        }
        private bool IsUnlocked() {
            return GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                   GameRoot.Instance.Flags.HasFlag(requiredFlag);
        }
        private void SyncVisuals() {
            bool visible = IsUnlocked() && !_harvestedToday;
            if (_sr != null) _sr.enabled = visible;
            if (_col != null) _col.enabled = visible;
        }
        public void Interact() {
            if (!CanInteract) return;
            _harvestedToday = true;
            GameRoot.Instance?.Inventory?.AddItemById(itemId, yieldAmount, true);
            SyncVisuals();
        }
        public void HideLegacyPrompt() { }
    }
}
