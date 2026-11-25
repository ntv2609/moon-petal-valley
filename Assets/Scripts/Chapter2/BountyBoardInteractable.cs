using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2 {
    public class BountyBoardInteractable : MonoBehaviour, IInteractable {
        [SerializeField] private float interactRadius = 2.4f;

        public bool CanInteract => GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsGameplayActive;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + Vector3.up * 1.15f;
        public string InteractionPromptLabel => "Bảng Nhiệm Vụ Bờ Đông (4 Mini-Bounties)";

        private void OnEnable() { InteractionManager.Register(this); }
        private void OnDisable() { InteractionManager.Unregister(this); }
        public void Interact() { if (BountyBoardUIView.Instance != null) BountyBoardUIView.Instance.Open(); }
        public void HideLegacyPrompt() { }
    }
}
