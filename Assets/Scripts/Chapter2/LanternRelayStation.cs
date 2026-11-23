using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2
{
    public class LanternRelayStation : MonoBehaviour, IInteractable
    {
        public enum BankSide { EastKael = 0, WestMara = 1 }

        [SerializeField] private BankSide side = BankSide.EastKael;
        [SerializeField] private float interactRadius = 2.6f;
        [SerializeField] private SpriteRenderer readyHighlightRenderer;

        private void OnEnable() => InteractionManager.Register(this);
        private void OnDisable() => InteractionManager.Unregister(this);

        private void Update()
        {
            if (readyHighlightRenderer == null) return;
            bool active = CanInteract;
            if (readyHighlightRenderer.enabled != active)
                readyHighlightRenderer.enabled = active;
            if (active)
            {
                float pulse = 0.32f + 0.24f * Mathf.Sin(Time.time * 4.5f);
                readyHighlightRenderer.color = new Color(1f, 0.88f, 0.42f, pulse);
            }
        }

        public bool CanInteract
        {
            get
            {
                LanternRelayManager mgr = LanternRelayManager.Instance;
                if (mgr == null) return false;
                return side == BankSide.EastKael ? mgr.CanInteractEastStation() : mgr.CanInteractWestStation();
            }
        }

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + Vector3.up * 1.15f;
        public string InteractionPromptLabel => LanternRelayManager.Instance == null
            ? "Chuyền Đồ"
            : (side == BankSide.EastKael ? LanternRelayManager.Instance.GetEastStationPrompt() : LanternRelayManager.Instance.GetWestStationPrompt());

        public void Interact()
        {
            if (LanternRelayManager.Instance == null) return;
            if (side == BankSide.EastKael) LanternRelayManager.Instance.HandleEastStationInteract();
            else LanternRelayManager.Instance.HandleWestStationInteract();
        }

        public void HideLegacyPrompt() { }
    }
}
