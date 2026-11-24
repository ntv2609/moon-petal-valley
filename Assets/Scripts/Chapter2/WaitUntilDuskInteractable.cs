using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.Chapter2
{
    public class WaitUntilDuskInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private float targetDuskHour = 18.5f;
        [SerializeField] private float interactRadius = 2.0f;
        [SerializeField] private string promptIdle = "Ngồi nghỉ chờ đến chập tối (18:30)";
        [SerializeField] private string promptAlreadyNight = "Trời đã vào tối rồi";

        public bool CanInteract
        {
            get
            {
                if (!gameObject.activeInHierarchy) return false;
                if (Chapter2StateManager.Instance != null)
                {
                    if (Chapter2StateManager.Instance.IsRelayEventActive) return false;
                    return Chapter2StateManager.Instance.IsWaitBenchUnlocked();
                }
                return true;
            }
        }

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 0.9f, 0f);

        public string InteractionPromptLabel
        {
            get
            {
                TimeOfDayManager tm = GameRoot.Instance != null ? GameRoot.Instance.Time : UnityEngine.Object.FindAnyObjectByType<TimeOfDayManager>();
                if (tm != null && tm.CurrentHourFloat >= targetDuskHour)
                    return promptAlreadyNight;
                return promptIdle;
            }
        }

        private void OnEnable()
        {
            InteractionManager.Register(this);
        }

        private void OnDisable()
        {
            InteractionManager.Unregister(this);
        }

        public void HideLegacyPrompt() { }

        public void Interact()
        {
            if (!CanInteract) return;
            TimeOfDayManager tm = GameRoot.Instance != null ? GameRoot.Instance.Time : UnityEngine.Object.FindAnyObjectByType<TimeOfDayManager>();
            if (tm == null) return;

            if (tm.CurrentHourFloat >= targetDuskHour)
            {
                GameEventBus.RaiseToastRequested("Trời đã vào tối, bạn có thể đi điều tra hoặc bắt đầu sự kiện đêm nay.");
                return;
            }

            tm.SetHour(targetDuskHour);
            GameEventBus.RaiseToastRequested("Đã ngồi nghỉ đến 18:30 (Hoàng hôn). Sương đêm bắt đầu phủ xuống bờ sông.");
            Debug.Log("[MV] WaitBench: Đã tua thời gian tới 18:30 (Hoàng hôn).");
        }
    }
}
