using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Narrative;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.Chapter3
{
    public class Ch3ConditionalVisibility : MonoBehaviour
    {
        [SerializeField] private string requiredFlag = Chapter3StateManager.FlagCh2Completed;
        [SerializeField] private string blockedFlag = "";
        [SerializeField] private bool toggleColliders = true;

        private SpriteRenderer[] _renderers;
        private Collider2D[] _colliders;
        private NPCController[] _npcs;
        private EchoSpotInteractable[] _echoSpots;
        private bool _cached;

        private void Awake()
        {
            CacheComponents();
            RefreshVisibility();
        }

        private void OnEnable()
        {
            CacheComponents();
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += RefreshVisibility;
            RefreshVisibility();
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= RefreshVisibility;
        }

        private void Start()
        {
            RefreshVisibility();
        }

        private void CacheComponents()
        {
            if (_cached) return;
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
            _colliders = GetComponentsInChildren<Collider2D>(true);
            _npcs = GetComponentsInChildren<NPCController>(true);
            _echoSpots = GetComponentsInChildren<EchoSpotInteractable>(true);
            _cached = true;
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == requiredFlag || (!string.IsNullOrEmpty(blockedFlag) && flagId == blockedFlag))
            {
                RefreshVisibility();
            }
        }

        public void RefreshVisibility()
        {
            CacheComponents();
            bool visible = EvaluateVisible();

            if (_renderers != null)
            {
                for (int i = 0; i < _renderers.Length; i++)
                    if (_renderers[i] != null) _renderers[i].enabled = visible;
            }
            if (toggleColliders && _colliders != null)
            {
                for (int i = 0; i < _colliders.Length; i++)
                    if (_colliders[i] != null) _colliders[i].enabled = visible;
            }
            if (_npcs != null)
            {
                for (int i = 0; i < _npcs.Length; i++)
                    if (_npcs[i] != null) _npcs[i].enabled = visible;
            }
            if (_echoSpots != null)
            {
                for (int i = 0; i < _echoSpots.Length; i++)
                    if (_echoSpots[i] != null) _echoSpots[i].enabled = visible;
            }
        }

        private bool EvaluateVisible()
        {
            var flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            if (flags == null) return false;
            if (!string.IsNullOrEmpty(requiredFlag) && !flags.HasFlag(requiredFlag)) return false;
            if (!string.IsNullOrEmpty(blockedFlag) && flags.HasFlag(blockedFlag)) return false;
            return true;
        }
    }
}
