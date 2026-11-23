using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Chapter2
{
    public class EchoSpotInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private EchoData echoData;
        [SerializeField] private string promptLabel = "Dùng Ánh Soi (Xem Dư Âm)";
        [SerializeField] private string blockedHint = "Chưa đủ điều kiện để soi Dư Âm này.";
        [SerializeField] private float interactRadius = 2.2f;
        [SerializeField] private DialogueData postEchoDialogue;
        [SerializeField] private SpriteRenderer shimmerRenderer;

        private bool IsAlreadyCompleted()
        {
            if (echoData == null || string.IsNullOrEmpty(echoData.completedFlag)) return false;
            var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            return flags != null && flags.HasFlag(echoData.completedFlag);
        }

        public bool CanInteract
        {
            get
            {
                if (!gameObject.activeInHierarchy || echoData == null) return false;
                if (EchoManager.Instance != null && EchoManager.Instance.IsPlaying) return false;
                var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
                if (flags == null || !flags.HasFlag("flag_nv1_completed")) return false;
                if (!string.IsNullOrEmpty(echoData.requiredFlag) && !flags.HasFlag(echoData.requiredFlag)) return false;
                return true;
            }
        }

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 1.0f, 0f);

        public string InteractionPromptLabel
        {
            get
            {
                if (IsAlreadyCompleted() && echoData != null)
                    return "Xem lại Dư Âm: " + echoData.echoTitle;
                return promptLabel;
            }
        }

        private void OnEnable() => InteractionManager.Register(this);
        private void OnDisable() => InteractionManager.Unregister(this);
        public void HideLegacyPrompt() { }

        private void Update()
        {
            if (shimmerRenderer != null)
            {
                bool active = CanInteract;
                shimmerRenderer.enabled = active;
                if (active)
                {
                    Color c = shimmerRenderer.color;
                    float baseAlpha = IsAlreadyCompleted() ? 0.18f : 0.35f;
                    float amp = IsAlreadyCompleted() ? 0.12f : 0.25f;
                    c.a = baseAlpha + amp * Mathf.Sin(Time.time * 3.5f);
                    shimmerRenderer.color = c;
                }
            }
        }

        public void Interact()
        {
            if (echoData == null || EchoManager.Instance == null) return;

            bool wasAlreadyCompleted = IsAlreadyCompleted();

            if (!wasAlreadyCompleted && !string.IsNullOrEmpty(echoData.requiredItemId))
            {
                var inv = GameRoot.Instance != null ? GameRoot.Instance.Inventory : null;
                if (inv == null || !inv.HasItem(echoData.requiredItemId, 1))
                {
                    GameEventBus.RaiseToastRequested(blockedHint);
                    return;
                }
            }

            EchoManager.Instance.PlayEcho(echoData, transform.position, () =>
            {
                if (!wasAlreadyCompleted && postEchoDialogue != null && GameRoot.Instance?.Dialogue != null)
                {
                    GameRoot.Instance.Dialogue.StartDialogue(postEchoDialogue);
                }
            });
        }
    }
}
