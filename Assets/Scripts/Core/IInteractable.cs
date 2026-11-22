using UnityEngine;

namespace MoonpetalValley.Core
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        float InteractRadius { get; }
        Vector2 InteractionWorldPosition { get; }
        Vector3 PointerAnchorPosition { get; }
        string InteractionPromptLabel { get; }
        void Interact();
        void HideLegacyPrompt();
    }
}
