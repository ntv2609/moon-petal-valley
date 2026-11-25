using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2 {
    public class BrewingCauldronInteractable : MonoBehaviour, IInteractable {
        [SerializeField] private float interactRadius = 2.1f;

        private void OnEnable() => InteractionManager.Register(this);
        private void OnDisable() => InteractionManager.Unregister(this);

        public bool CanInteract => true;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + Vector3.up * 1.1f;
        public string InteractionPromptLabel => "Vạc Nấu Thuốc Vườn Ươm";

        public void Interact() {
            if (BrewingCauldronUIView.Instance != null)
                BrewingCauldronUIView.Instance.Open();
            else
                GameEventBus.RaiseToastRequested("Vạc Nấu Thuốc đang nhóm lửa!");
        }

        public void HideLegacyPrompt() { }
    }
}
