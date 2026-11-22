using System.Collections;
using UnityEngine;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Narrative
{
    public class MiuCatInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueData reunionDialogue;
        [SerializeField] private float interactRadius = 2.15f;
        [SerializeField] private Vector3 homePositionNearToby = new Vector3(4.6f, 5.1f, 0f);
        [SerializeField] private float runSpeed = 4.8f;
        [SerializeField] private TMP_Text floatingPromptText;

        private SpriteRenderer _sr;
        private Collider2D _col;
        private Vector3 _basePos;
        private bool _isRunningHome;
        private bool _hasArrivedHome;

        public bool CanInteract => gameObject.activeInHierarchy && !_isRunningHome;
        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition => transform.position + new Vector3(0f, 0.85f, 0f);

        public string InteractionPromptLabel
        {
            get
            {
                if (_hasArrivedHome || HasFlag("flag_miu_comforted") || HasFlag("flag_toby_cat_saved"))
                    return "Vuốt ve Mèo Miu";
                if (HasFlag("flag_toby_cat_slimes_cleared"))
                    return "An ủi Mèo Miu";
                return "Mèo Miu (Đang sợ!)";
            }
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _basePos = transform.position;
            EnsureSolidCollider();
        }

        private void EnsureSolidCollider()
        {
            _col = GetComponent<Collider2D>();
            if (_col == null)
            {
                var cc = gameObject.AddComponent<CircleCollider2D>();
                cc.radius = 0.28f;
                cc.offset = new Vector2(0f, -0.08f);
                cc.isTrigger = false;
                _col = cc;
            }
            gameObject.layer = 11;
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnSaveLoaded += SyncWithStoryFlags;
            InteractionManager.Register(this);
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnSaveLoaded -= SyncWithStoryFlags;
            InteractionManager.Unregister(this);
        }

        private void Start()
        {
            HideLegacyPrompt();
            SyncWithStoryFlags();
        }

        public void HideLegacyPrompt()
        {
            if (floatingPromptText != null && floatingPromptText.gameObject.activeSelf)
                floatingPromptText.gameObject.SetActive(false);
        }

        private void SyncWithStoryFlags()
        {
            HideLegacyPrompt();
            if (HasFlag("flag_miu_comforted") || HasFlag("flag_toby_cat_saved"))
            {
                SnapToTobyHome();
            }
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value && (flagId == "flag_miu_comforted" || flagId == "flag_toby_cat_saved"))
            {
                if (!_isRunningHome && !_hasArrivedHome)
                    SnapToTobyHome();
            }
        }

        private void Update()
        {
            if (_isRunningHome) return;
            AnimateIdleCat();
        }

        public void Interact()
        {
            if (!CanInteract) return;
            HandleInteract();
        }

        private void HandleInteract()
        {
            if (!HasFlag("flag_toby_cat_slimes_cleared"))
            {
                GameEventBus.RaiseToastRequested("Mèo Miu đang run rẩy! Hãy dùng Liềm Ánh Trăng (4) dọn sạch 3 Bọt Sương Đen quanh đây trước.");
                return;
            }

            if (!HasFlag("flag_miu_comforted") && !HasFlag("flag_toby_cat_saved"))
            {
                if (reunionDialogue != null && GameRoot.Instance?.Dialogue != null)
                    GameRoot.Instance.Dialogue.StartDialogue(reunionDialogue, OnReunionFinished);
                else
                    OnReunionFinished();
                return;
            }

            GameRoot.Instance?.Audio?.PlayUIClick();
            GameEventBus.RaiseToastRequested("Mèo Miu dụi đầu vào tay bạn: 'Meo~ rừ rừ...'");
        }

        private void OnReunionFinished()
        {
            GameRoot.Instance?.Flags?.SetFlag("flag_miu_comforted", true);
            GameEventBus.RaiseToastRequested("Mèo Miu hết sợ rồi và đang chạy về với Cậu Bé Toby! Hãy quay lại gặp Toby [E].");
            StartCoroutine(RunHomeToTobyRoutine());
        }

        private IEnumerator RunHomeToTobyRoutine()
        {
            _isRunningHome = true;
            if (_col != null) _col.enabled = false;
            Vector3 target = homePositionNearToby;
            if (_sr != null) _sr.flipX = target.x < transform.position.x;

            float hopTimer = 0f;
            while (Vector2.Distance(transform.position, target) > 0.15f)
            {
                hopTimer += Time.deltaTime;
                Vector3 next = Vector3.MoveTowards(transform.position, target, runSpeed * Time.deltaTime);
                float hop = Mathf.Abs(Mathf.Sin(hopTimer * 14f)) * 0.16f;
                transform.position = new Vector3(next.x, next.y + hop * Time.deltaTime * 4f, 0f);
                yield return null;
            }

            SnapToTobyHome();
            if (_col != null) _col.enabled = true;
            _isRunningHome = false;
        }

        private void SnapToTobyHome()
        {
            transform.position = homePositionNearToby;
            _basePos = homePositionNearToby;
            _hasArrivedHome = true;
            if (_sr != null) _sr.flipX = false;
            if (_col != null) _col.enabled = true;
        }

        private void AnimateIdleCat()
        {
            if (!HasFlag("flag_toby_cat_slimes_cleared"))
            {
                float shiver = Mathf.Sin(Time.time * 26f) * 0.03f;
                transform.position = new Vector3(_basePos.x + shiver, _basePos.y, _basePos.z);
            }
            else
            {
                float bob = Mathf.Sin(Time.time * 3.8f) * 0.04f;
                transform.position = new Vector3(_basePos.x, _basePos.y + bob, _basePos.z);
            }
        }

        private bool HasFlag(string flagId)
        {
            return GameRoot.Instance != null && GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag(flagId);
        }
    }
}
