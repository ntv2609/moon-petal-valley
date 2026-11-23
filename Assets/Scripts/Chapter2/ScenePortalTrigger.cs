using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Chapter2
{
    public class ScenePortalTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private string targetSceneName = "Riverbank_Chapter2";
        [SerializeField] private string targetSpawnId = "Spawn_EastEntrance";
        [SerializeField] private string requiredFlag = "flag_nv1_completed";
        [SerializeField] private string promptLabel = "Đi tới Bờ Sông (Chương 2)";
        [SerializeField] private string lockedToast = "Sương dày vẫn chặn lối xuống Bờ Sông. Hãy hoàn thành phục hồi Vườn Phía Nam trước!";
        [SerializeField] private float interactRadius = 2.3f;

        public bool CanInteract => gameObject.activeInHierarchy;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 1.0f, 0f);
        public string InteractionPromptLabel => promptLabel;

        private void OnEnable() => InteractionManager.Register(this);
        private void OnDisable() => InteractionManager.Unregister(this);
        public void HideLegacyPrompt() { }

        public void Interact()
        {
            if (GameRoot.Instance == null) return;
            if (!string.IsNullOrEmpty(requiredFlag))
            {
                if (GameRoot.Instance.Flags == null || !GameRoot.Instance.Flags.HasFlag(requiredFlag))
                {
                    GameEventBus.RaiseToastRequested(lockedToast);
                    return;
                }
            }

            // Khi từ Chương 2 quay về Làng Chính, đảm bảo cờ mở đầu Ch1 đã bật để không chạy lại Cutscene New Game
            if (targetSceneName == "MoonpetalValley_MVP" && GameRoot.Instance.Flags != null)
            {
                if (!GameRoot.Instance.Flags.HasFlag("flag_intro_cutscene_done"))
                    GameRoot.Instance.Flags.SetFlag("flag_intro_cutscene_done", true);
                if (!GameRoot.Instance.Flags.HasFlag("flag_tutorial_done"))
                    GameRoot.Instance.Flags.SetFlag("flag_tutorial_done", true);
            }

            if (Chapter2StateManager.Instance != null)
                Chapter2StateManager.Instance.PendingSceneSpawnId = targetSpawnId;

            if (GameRoot.Instance.Audio != null) GameRoot.Instance.Audio.PlayUIClick();
            if (GameRoot.Instance.Scenes != null) GameRoot.Instance.Scenes.LoadScene(targetSceneName);
        }
    }
}
