using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2 {
    public class KaelForgeInteractable : MonoBehaviour, IInteractable {
        [SerializeField] private float interactRadius = 2.4f;

        public bool CanInteract => GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsGameplayActive;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + Vector3.up * 1.15f;
        public string InteractionPromptLabel => "Lò Rèn Ông Kael (Nâng Cấp Trang Bị)";

        private void OnEnable() { InteractionManager.Register(this); }
        private void OnDisable() { InteractionManager.Unregister(this); }

        public void Interact() {
            if (KaelForgeUIView.Instance != null)
                KaelForgeUIView.Instance.Open();
            else
                GameEventBus.RaiseToastRequested("Lò Rèn Ông Kael chưa sẵn sàng!");
        }

        public void HideLegacyPrompt() { }
    }
}
