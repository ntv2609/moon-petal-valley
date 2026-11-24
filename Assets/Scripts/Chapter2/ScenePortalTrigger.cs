using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2
{
    public class ScenePortalTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private string targetSceneName = "Riverbank_Chapter2";
        [SerializeField] private string targetSpawnId = "Spawn_EastEntrance";
        [SerializeField] private string requiredFlag = "flag_nv1_completed";
        [SerializeField] private bool useSwampUnlockGate;
        [SerializeField] private bool autoTriggerOnWalk;
        [SerializeField] private float walkCooldown = 0.85f;
        [SerializeField] private string promptLabel = "Đi tới Bờ Sông (Chương 2)";
        [SerializeField] private string lockedToast = "Sương dày vẫn chặn lối xuống Bờ Sông. Hãy hoàn thành phục hồi Vườn Phía Nam trước!";
        [SerializeField] private float interactRadius = 2.3f;

        private static float _lastSceneLoadedTime = -10f;
        private static bool _isTransitioning;
        private static bool _hookedSceneLoaded;
        private float _lastLockedToastTime = -10f;

        public bool CanInteract
        {
            get
            {
                if (autoTriggerOnWalk) return false;
                if (!gameObject.activeInHierarchy || _isTransitioning) return false;
                if (Chapter2StateManager.Instance != null && Chapter2StateManager.Instance.IsRelayEventActive)
                    return false;
                return true;
            }
        }

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 1.0f, 0f);
        public string InteractionPromptLabel => promptLabel;

        private void Awake()
        {
            if (!_hookedSceneLoaded)
            {
                _hookedSceneLoaded = true;
                SceneManager.sceneLoaded += OnAnySceneLoaded;
            }
        }

        private static void OnAnySceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _lastSceneLoadedTime = Time.unscaledTime;
            _isTransitioning = false;
            SceneRoadFadeOverlay.FadeInIfPresent();
        }

        private void OnEnable()
        {
            if (!autoTriggerOnWalk)
                InteractionManager.Register(this);
        }

        private void OnDisable()
        {
            InteractionManager.Unregister(this);
        }

        public void HideLegacyPrompt() { }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!autoTriggerOnWalk || _isTransitioning || other == null) return;
            if (Time.unscaledTime - _lastSceneLoadedTime < walkCooldown) return;
            if (Chapter2StateManager.Instance != null && Chapter2StateManager.Instance.IsRelayEventActive) return;

            bool isPlayer = other.CompareTag("Player") || other.GetComponentInParent<PlayerController2D>() != null;
            if (!isPlayer) return;

            TryTriggerTransition(true);
        }

        public void Interact()
        {
            if (autoTriggerOnWalk || !CanInteract) return;
            TryTriggerTransition(false);
        }

        private void TryTriggerTransition(bool useFade)
        {
            if (GameRoot.Instance == null || _isTransitioning) return;

            if (useSwampUnlockGate)
            {
                bool swampOpen = Chapter2StateManager.Instance != null && Chapter2StateManager.Instance.IsSwampUnlocked();
                if (!swampOpen)
                {
                    if (Time.unscaledTime - _lastLockedToastTime >= 2.0f)
                    {
                        _lastLockedToastTime = Time.unscaledTime;
                        GameEventBus.RaiseToastRequested(lockedToast);
                    }
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(requiredFlag))
            {
                if (GameRoot.Instance.Flags == null || !GameRoot.Instance.Flags.HasFlag(requiredFlag))
                {
                    if (Time.unscaledTime - _lastLockedToastTime >= 2.0f)
                    {
                        _lastLockedToastTime = Time.unscaledTime;
                        GameEventBus.RaiseToastRequested(lockedToast);
                    }
                    return;
                }
            }

            if (targetSceneName == "MoonpetalValley_MVP" && GameRoot.Instance.Flags != null)
            {
                if (!GameRoot.Instance.Flags.HasFlag("flag_intro_cutscene_done"))
                    GameRoot.Instance.Flags.SetFlag("flag_intro_cutscene_done", true);
                if (!GameRoot.Instance.Flags.HasFlag("flag_tutorial_done"))
                    GameRoot.Instance.Flags.SetFlag("flag_tutorial_done", true);
            }

            if (Chapter2StateManager.Instance != null)
                Chapter2StateManager.Instance.PendingSceneSpawnId = targetSpawnId;

            if (useFade)
            {
                _isTransitioning = true;
                SceneRoadFadeOverlay.TransitionToScene(targetSceneName);
            }
            else
            {
                if (GameRoot.Instance.Audio != null) GameRoot.Instance.Audio.PlayUIClick();
                if (GameRoot.Instance.Scenes != null) GameRoot.Instance.Scenes.LoadScene(targetSceneName);
                else SceneManager.LoadScene(targetSceneName);
            }
        }
    }

    internal class SceneRoadFadeOverlay : MonoBehaviour
    {
        private static SceneRoadFadeOverlay _instance;
        private CanvasGroup _group;
        private Coroutine _activeCo;

        private static SceneRoadFadeOverlay EnsureInstance()
        {
            if (_instance != null) return _instance;

            GameObject go = new GameObject("SceneRoadFadeOverlay");
            DontDestroyOnLoad(go);
            Canvas cv = go.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cv.sortingOrder = 9999;
            CanvasGroup cg = go.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            GameObject imgGo = new GameObject("Black", typeof(RectTransform), typeof(Image));
            imgGo.transform.SetParent(go.transform, false);
            RectTransform rt = imgGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            Image img = imgGo.GetComponent<Image>();
            img.color = new Color(0.03f, 0.04f, 0.06f, 1f);
            img.raycastTarget = false;

            _instance = go.AddComponent<SceneRoadFadeOverlay>();
            _instance._group = cg;
            return _instance;
        }

        public static void TransitionToScene(string sceneName)
        {
            var inst = EnsureInstance();
            if (inst._activeCo != null) inst.StopCoroutine(inst._activeCo);
            inst._activeCo = inst.StartCoroutine(inst.FadeOutAndLoad(sceneName));
        }

        public static void FadeInIfPresent()
        {
            if (_instance == null || _instance._group == null) return;
            if (_instance._group.alpha <= 0.01f) return;
            if (_instance._activeCo != null) _instance.StopCoroutine(_instance._activeCo);
            _instance._activeCo = _instance.StartCoroutine(_instance.FadeInRoutine());
        }

        private IEnumerator FadeOutAndLoad(string sceneName)
        {
            float dur = 0.24f;
            float t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                _group.alpha = Mathf.Clamp01(t / dur);
                yield return null;
            }
            _group.alpha = 1f;

            if (GameRoot.Instance != null && GameRoot.Instance.Scenes != null)
                GameRoot.Instance.Scenes.LoadScene(sceneName);
            else
                SceneManager.LoadScene(sceneName);
        }

        private IEnumerator FadeInRoutine()
        {
            yield return null;
            float dur = 0.28f;
            float t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                _group.alpha = 1f - Mathf.Clamp01(t / dur);
                yield return null;
            }
            _group.alpha = 0f;
            _activeCo = null;
        }
    }
}
