using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2
{
    public class FerryBoatInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string requiredFlag = "flag_ch2_solo_bridge_failed";
        [SerializeField] private Vector2 destinationPosition;
        [SerializeField] private string promptLabel = "Chèo Đò Sang Bờ Tây";
        [SerializeField] private string arrivalToast = "Bạn chèo chiếc đò gỗ sang bờ bên kia.";
        [SerializeField] private float interactRadius = 2.2f;
        [SerializeField] private float sailSpeed = 5.4f;

        private static bool _isCrossing;
        private static CanvasGroup _fadeOverlay;

        public bool CanInteract
        {
            get
            {
                if (_isCrossing || !gameObject.activeInHierarchy) return false;
                if (Chapter2StateManager.Instance != null && Chapter2StateManager.Instance.IsRelayEventActive) return false;
                if (string.IsNullOrEmpty(requiredFlag)) return true;
                return GameRoot.Instance?.Flags != null && GameRoot.Instance.Flags.HasFlag(requiredFlag);
            }
        }

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 0.9f, 0f);
        public string InteractionPromptLabel => promptLabel;

        private void OnEnable() => InteractionManager.Register(this);
        private void OnDisable() => InteractionManager.Unregister(this);
        public void HideLegacyPrompt() { }

        public void Interact()
        {
            if (_isCrossing) return;
            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player == null) return;
            StartCoroutine(CrossRiverSequence(player));
        }

        private IEnumerator CrossRiverSequence(PlayerController2D player)
        {
            _isCrossing = true;
            GameRoot.Instance?.State?.SetState(GameState.Cutscene);
            GameRoot.Instance?.Audio?.PlayUIClick();

            var cam = UnityEngine.Object.FindAnyObjectByType<CameraFollow2D>();
            var wenAnim = player.GetComponent<WenSpriteAnimator>();
            var playerCol = player.GetComponent<Collider2D>();
            if (playerCol != null) playerCol.enabled = false;

            Vector3 boatOrigin = transform.position;
            float dirX = destinationPosition.x < boatOrigin.x ? -1f : 1f;
            Vector2 sailDir = new Vector2(dirX, 0f);

            // 1. Giữ camera cố định tại bờ xuất phát để thấy đò chở Wen trôi khuất màn hình
            Camera mainCam = cam != null ? cam.GetComponent<Camera>() : Camera.main;
            float halfViewWidth = 10.5f;
            if (mainCam != null && mainCam.orthographic)
            {
                halfViewWidth = mainCam.orthographicSize * mainCam.aspect;
                if (cam != null) cam.BeginCutsceneOverride(mainCam.transform.position, mainCam.orthographicSize);
            }
            float camCenterX = mainCam != null ? mainCam.transform.position.x : boatOrigin.x;
            float offscreenTargetX = camCenterX + dirX * (halfViewWidth + 2.2f);

            // 2. Wen ngồi lên đò & diễn hoạt chèo đò (Chore anim)
            if (wenAnim != null) wenAnim.BeginCutsceneChore(sailDir);

            float t = 0f;
            float maxSailTime = 3.2f;
            while (t < maxSailTime)
            {
                t += Time.unscaledDeltaTime;
                float nextX = transform.position.x + dirX * sailSpeed * Time.unscaledDeltaTime;
                float bobY = Mathf.Sin(t * 6.5f) * 0.08f;
                transform.position = new Vector3(nextX, boatOrigin.y + bobY, boatOrigin.z);
                player.TeleportTo(new Vector2(nextX, boatOrigin.y + 0.25f + bobY));

                bool pastEdge = dirX < 0f ? (nextX <= offscreenTargetX) : (nextX >= offscreenTargetX);
                if (pastEdge) break;
                yield return null;
            }

            // 3. Mờ màn hình chuyển sang bờ bên kia
            yield return FadeScreen(0f, 1f, 0.32f);

            // Đưa đò bờ xuất phát về lại vị trí neo cũ, chuyển Wen sang bờ đích
            transform.position = boatOrigin;
            if (wenAnim != null) wenAnim.EndCutsceneOverride();
            player.TeleportTo(destinationPosition);
            if (playerCol != null) playerCol.enabled = true;
            if (cam != null) cam.EndCutsceneOverride(true);

            yield return new WaitForSecondsRealtime(0.15f);
            yield return FadeScreen(1f, 0f, 0.35f);

            GameRoot.Instance?.State?.ReturnToGameplay();
            GameEventBus.RaiseToastRequested(arrivalToast);
            _isCrossing = false;
        }

        private static IEnumerator FadeScreen(float from, float to, float duration)
        {
            EnsureFadeOverlay();
            if (_fadeOverlay == null) yield break;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _fadeOverlay.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
                yield return null;
            }
            _fadeOverlay.alpha = to;
        }

        private static void EnsureFadeOverlay()
        {
            if (_fadeOverlay != null) return;
            var go = new GameObject("FerryFadeOverlayCanvas");
            var cv = go.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cv.sortingOrder = 240;
            var cg = go.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            var imgGo = new GameObject("FadeImage", typeof(RectTransform), typeof(Image));
            imgGo.transform.SetParent(go.transform, false);
            var rt = imgGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            imgGo.GetComponent<Image>().color = new Color(0.05f, 0.09f, 0.14f, 1f);

            _fadeOverlay = cg;
        }
    }
}
