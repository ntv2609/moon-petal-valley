using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.TimeSystem
{
    public class BedInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 1.9f;

        public bool CanInteract => gameObject.activeInHierarchy;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => new Vector3(transform.position.x, transform.position.y + 0.85f, 0f);
        public string InteractionPromptLabel => "Đi Ngủ (Qua Ngày Mới)";

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
            if (GameRoot.Instance != null && GameRoot.Instance.Sleep != null)
            {
                GameRoot.Instance.Sleep.TriggerNormalSleep();
            }
        }
    }
}
