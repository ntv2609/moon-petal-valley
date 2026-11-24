using UnityEngine;
using MoonpetalValley.Core;
namespace MoonpetalValley.Chapter3
{
    public class Ch3PrepWorkbenchInteractable : MonoBehaviour, IInteractable {
        [SerializeField] private float interactRadius = 1.8f;
        [SerializeField] private string requiredFlag = "flag_ch2_completed";
        private SpriteRenderer _sr;
        private Collider2D _col;
        public bool CanInteract {
            get {
                if (!IsUnlocked()) return false;
                return GameRoot.Instance != null && GameRoot.Instance.State != null &&
                       GameRoot.Instance.State.IsGameplayActive;
            }
        }
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 0.9f, 0f);
        public string InteractionPromptLabel => "Bàn Chuẩn Bị Chợ Đêm [E]";
        private void Awake() {
            _sr = GetComponent<SpriteRenderer>();
            _col = GetComponent<Collider2D>();
        }
        private void OnEnable() {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += SyncVisibility;
            SyncVisibility();
        }
        private void OnDisable() {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= SyncVisibility;
        }
        private void HandleFlagChanged(string flagId, bool _) {
            if (flagId == requiredFlag) SyncVisibility();
        }
        private bool IsUnlocked() {
            return GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                   GameRoot.Instance.Flags.HasFlag(requiredFlag);
        }
        public void SyncVisibility() {
            bool unlocked = IsUnlocked();
            if (_sr != null) _sr.enabled = unlocked;
            if (_col != null) _col.enabled = unlocked;
        }
        public void Interact() {
            if (!CanInteract) return;
            if (PrepWorkbenchUIView.Instance != null)
                PrepWorkbenchUIView.Instance.Open();
            else
                GameEventBus.RaiseToastRequested("Bàn chuẩn bị Chợ Đêm chưa sẵn sàng.");
        }
        public void HideLegacyPrompt() { }
    }
}
