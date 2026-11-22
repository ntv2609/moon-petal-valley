using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.Core
{
    public class InteractionManager : MonoBehaviour
    {
        public static InteractionManager Instance { get; private set; }

        [SerializeField] private InteractionPromptUIView promptUI;

        private static readonly List<IInteractable> _registry = new List<IInteractable>();
        private readonly List<IInteractable> _candidates = new List<IInteractable>();

        private PlayerController2D _player;
        private GameInputReader _input;
        private IInteractable _focusedTarget;
        private bool _manualSelectionLock;
        private float _interactCooldown;

        private GameObject _ringObj;
        private SpriteRenderer _ringSr;

        public bool HasFocusedInteractable => _focusedTarget != null;
        public int CandidateCount => _candidates.Count;
        public IInteractable FocusedTarget => _focusedTarget;

        public static void Register(IInteractable item)
        {
            if (item == null) return;
            if (!_registry.Contains(item))
            {
                _registry.Add(item);
            }
        }

        public static void Unregister(IInteractable item)
        {
            if (item == null) return;
            _registry.Remove(item);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            EnsureSelectionRing();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (_ringObj != null) Destroy(_ringObj);
        }

        private void Update()
        {
            if (_interactCooldown > 0f) _interactCooldown -= Time.unscaledDeltaTime;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                ClearFocus();
                return;
            }

            if (promptUI == null)
            {
                promptUI = GetComponentInChildren<InteractionPromptUIView>(true);
                if (promptUI == null) promptUI = UnityEngine.Object.FindAnyObjectByType<InteractionPromptUIView>();
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else
                {
                    ClearFocus();
                    return;
                }
            }

            RefreshCandidates();
            if (_candidates.Count == 0)
            {
                ClearFocus();
                return;
            }

            int currentIdx = _focusedTarget != null ? _candidates.IndexOf(_focusedTarget) : -1;
            if (currentIdx < 0)
            {
                _manualSelectionLock = false;
                currentIdx = 0;
            }
            else if (!_manualSelectionLock)
            {
                currentIdx = 0;
            }

            if (_candidates.Count > 1)
            {
                int cycleDir = ReadCycleInput();
                if (cycleDir != 0)
                {
                    currentIdx = (currentIdx + cycleDir + _candidates.Count) % _candidates.Count;
                    _manualSelectionLock = true;
                }
            }
            else
            {
                _manualSelectionLock = false;
                currentIdx = 0;
            }

            _focusedTarget = _candidates[currentIdx];

            if (promptUI != null)
            {
                promptUI.ShowPrompt(_focusedTarget.InteractionPromptLabel, currentIdx, _candidates.Count);
            }

            UpdateSelectionRing(_focusedTarget);

            if (_interactCooldown <= 0f && _input != null && _input.InteractPressed)
            {
                _interactCooldown = 0.16f;
                var target = _focusedTarget;
                target.Interact();
            }
        }

        private void RefreshCandidates()
        {
            _candidates.Clear();
            Vector2 playerPos = _player.transform.position;
            Vector2 facing = _player.FacingDirection;
            if (facing.sqrMagnitude < 0.01f) facing = Vector2.down;
            else facing.Normalize();

            for (int i = _registry.Count - 1; i >= 0; i--)
            {
                var item = _registry[i];
                if (item == null || (item is Object unityObj && unityObj == null))
                {
                    _registry.RemoveAt(i);
                    continue;
                }

                item.HideLegacyPrompt();
                if (!item.CanInteract) continue;

                float dist = Vector2.Distance(playerPos, item.InteractionWorldPosition);
                if (dist <= item.InteractRadius)
                {
                    _candidates.Add(item);
                }
            }

            if (_candidates.Count > 1)
            {
                _candidates.Sort((a, b) =>
                {
                    float sa = ComputeScore(a, playerPos, facing);
                    float sb = ComputeScore(b, playerPos, facing);
                    int cmp = sa.CompareTo(sb);
                    if (cmp != 0) return cmp;
                    return string.CompareOrdinal(a.InteractionPromptLabel, b.InteractionPromptLabel);
                });
            }
        }

        private static float ComputeScore(IInteractable item, Vector2 playerPos, Vector2 facing)
        {
            Vector2 delta = item.InteractionWorldPosition - playerPos;
            float dist = delta.magnitude;
            float dot = dist > 0.05f ? Vector2.Dot(facing, delta / dist) : 1f;
            return dist - dot * 0.78f;
        }

        private int ReadCycleInput()
        {
            int dir = 0;
#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (kb != null && kb.rKey.wasPressedThisFrame) dir = 1;
            if (dir == 0 && mouse != null)
            {
                float scroll = mouse.scroll.ReadValue().y;
                if (scroll > 10f) dir = -1;
                else if (scroll < -10f) dir = 1;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.R)) dir = 1;
            if (dir == 0)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (scroll > 0.1f) dir = -1;
                else if (scroll < -0.1f) dir = 1;
            }
#endif
            return dir;
        }

        private void ClearFocus()
        {
            _focusedTarget = null;
            _candidates.Clear();
            _manualSelectionLock = false;
            if (promptUI != null) promptUI.HidePrompt();
            if (_ringObj != null && _ringObj.activeSelf) _ringObj.SetActive(false);
        }

        private void EnsureSelectionRing()
        {
            if (_ringObj != null) return;
            _ringObj = new GameObject("InteractionFocusRing");
            DontDestroyOnLoad(_ringObj);
            _ringSr = _ringObj.AddComponent<SpriteRenderer>();
            _ringSr.sprite = CreateSoftGroundRingSprite();
            // Shadows (order 5): hien ro tren nen dat/tham/bon hoa & bong do, nhung van o duoi chan nhan vat!
            _ringSr.sortingLayerName = "Shadows";
            _ringSr.sortingOrder = 5;
            _ringObj.SetActive(false);
        }

        private void UpdateSelectionRing(IInteractable target)
        {
            EnsureSelectionRing();
            if (_ringObj == null || _ringSr == null || target == null) return;
            if (!_ringObj.activeSelf) _ringObj.SetActive(true);

            Vector2 pos = target.InteractionWorldPosition;
            float footY = pos.y - 0.36f;
            float widthScale = 1.0f;

            if (target is Component comp && comp != null)
            {
                SpriteRenderer sr = comp.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    Bounds b = sr.bounds;
                    if (b.size.x > 1.8f && b.size.y > 1.6f)
                    {
                        footY = b.center.y - 0.08f;
                        widthScale = Mathf.Clamp(b.size.x * 0.92f, 1.3f, 2.35f);
                    }
                    else
                    {
                        footY = b.min.y + 0.06f;
                        widthScale = Mathf.Clamp(b.size.x * 1.12f, 0.88f, 1.55f);
                    }
                }
            }

            float pulse = 1f + Mathf.Sin(Time.unscaledTime * 4.2f) * 0.04f;
            float alpha = 0.62f + Mathf.Sin(Time.unscaledTime * 4.2f) * 0.18f;

            _ringObj.transform.position = new Vector3(pos.x, footY, 0f);
            _ringObj.transform.localScale = new Vector3(widthScale * pulse, widthScale * pulse, 1f);
            _ringSr.color = new Color(1f, 0.95f, 0.72f, alpha);
        }

        private static Sprite CreateSoftGroundRingSprite()
        {
            const int w = 64;
            const int h = 32;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            float cx = (w - 1) * 0.5f;
            float cy = (h - 1) * 0.5f;
            float rx = w * 0.44f;
            float ry = h * 0.40f;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);

                    float ring = Mathf.Exp(-Mathf.Pow((d - 0.90f) / 0.085f, 2f));
                    float innerSoft = (d < 0.90f) ? Mathf.SmoothStep(0f, 0.18f, d / 0.90f) : 0f;
                    float a = Mathf.Clamp01(ring * 0.95f + innerSoft * 0.45f);
                    if (d > 1.08f) a = 0f;

                    tex.SetPixel(x, y, new Color(1f, 0.96f, 0.80f, a));
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 64f);
        }
    }
}
