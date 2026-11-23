using System;
using System.Collections;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2
{
    public class NPCApproachOnInteract : MonoBehaviour
    {
        [SerializeField] private float runSpeed = 9.2f;
        [SerializeField] private float returnSpeed = 4.6f;
        [SerializeField] private float stopDistance = 1.25f;
        [SerializeField] private float maxApproachStartDistance = 8.0f;
        [SerializeField] private float offscreenMargin = 1.4f;
        [SerializeField] private float safetyTimeout = 4.5f;

        private Vector3 _homePosition;
        private bool _homeCaptured;
        private bool _isApproaching;
        private Coroutine _activeRoutine;
        private SpriteRenderer _spriteRenderer;

        public bool IsApproaching => _isApproaching;

        private void Awake()
        {
            CaptureHomeIfNeeded();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void CaptureHomeIfNeeded()
        {
            if (!_homeCaptured)
            {
                _homePosition = transform.position;
                _homeCaptured = true;
            }
        }

        public void RunToPlayerAndExecute(Action onArrived, bool spawnFromOffscreen = false)
        {
            CaptureHomeIfNeeded();
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_activeRoutine != null)
            {
                StopCoroutine(_activeRoutine);
                _activeRoutine = null;
            }

            PlayerController2D player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null && spawnFromOffscreen)
            {
                // Đặt ra ngoài mép màn hình ngay lập tức cùng frame để không nháy hình tại nhà
                PositionOutsideCameraEdge(player.transform.position);
            }

            _activeRoutine = StartCoroutine(ApproachCoroutine(onArrived, spawnFromOffscreen));
        }

        public void ReturnHome()
        {
            if (!_homeCaptured || !gameObject.activeInHierarchy) return;

            if (_activeRoutine != null)
            {
                StopCoroutine(_activeRoutine);
                _activeRoutine = null;
            }

            _activeRoutine = StartCoroutine(ReturnHomeCoroutine());
        }

        private IEnumerator ApproachCoroutine(Action onArrived, bool spawnFromOffscreen)
        {
            _isApproaching = true;

            PlayerController2D player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player == null)
            {
                _isApproaching = false;
                _activeRoutine = null;
                onArrived?.Invoke();
                yield break;
            }

            GameStateManager stateMgr = GameRoot.Instance != null ? GameRoot.Instance.State : null;
            GameState prevState = stateMgr != null ? stateMgr.CurrentState : GameState.Gameplay;
            if (stateMgr != null)
            {
                stateMgr.SetState(GameState.Cutscene);
            }

            Vector3 wenPos = player.transform.position;
            if (!spawnFromOffscreen)
            {
                Vector3 toNpc = transform.position - wenPos;
                float dist = toNpc.magnitude;
                if (dist > maxApproachStartDistance && dist > 0.01f)
                {
                    transform.position = wenPos + toNpc.normalized * maxApproachStartDistance;
                }
            }

            Vector3 deltaFromWen = transform.position - wenPos;
            float sideX = deltaFromWen.x >= 0f ? 1f : -1f;
            Vector3 targetPos = new Vector3(wenPos.x + sideX * stopDistance, wenPos.y, transform.position.z);

            float elapsed = 0f;
            Vector3 logicalPos = transform.position;

            while (elapsed < safetyTimeout)
            {
                float dt = Time.unscaledDeltaTime;
                Vector3 toTarget = targetPos - logicalPos;
                toTarget.z = 0f;
                float remaining = toTarget.magnitude;
                if (remaining <= 0.08f)
                {
                    logicalPos = targetPos;
                    break;
                }

                float step = runSpeed * dt;
                Vector3 dir = toTarget / remaining;
                logicalPos += dir * Mathf.Min(step, remaining);

                elapsed += dt;
                float bob = Mathf.Abs(Mathf.Sin(elapsed * 16f)) * 0.09f;
                transform.position = new Vector3(logicalPos.x, logicalPos.y + bob, logicalPos.z);

                UpdateFacingAndSorting(dir.x, logicalPos.y);
                yield return null;
            }

            transform.position = logicalPos;
            UpdateFacingAndSorting(wenPos.x - logicalPos.x, logicalPos.y);

            if (stateMgr != null && stateMgr.CurrentState == GameState.Cutscene)
            {
                stateMgr.SetState(prevState == GameState.Cutscene ? GameState.Gameplay : prevState);
            }

            _isApproaching = false;
            _activeRoutine = null;
            onArrived?.Invoke();
        }

        private IEnumerator ReturnHomeCoroutine()
        {
            Vector3 logicalPos = transform.position;
            float elapsed = 0f;
            float maxReturnTime = 8.0f;

            while (elapsed < maxReturnTime)
            {
                float dt = Time.unscaledDeltaTime;
                Vector3 toHome = _homePosition - logicalPos;
                toHome.z = 0f;
                float remaining = toHome.magnitude;
                if (remaining <= 0.08f)
                {
                    logicalPos = _homePosition;
                    break;
                }

                float step = returnSpeed * dt;
                Vector3 dir = toHome / remaining;
                logicalPos += dir * Mathf.Min(step, remaining);

                elapsed += dt;
                float bob = Mathf.Abs(Mathf.Sin(elapsed * 11f)) * 0.05f;
                transform.position = new Vector3(logicalPos.x, logicalPos.y + bob, logicalPos.z);

                UpdateFacingAndSorting(dir.x, logicalPos.y);
                yield return null;
            }

            transform.position = _homePosition;
            UpdateFacingAndSorting(-1f, _homePosition.y);
            _activeRoutine = null;
        }

        private void PositionOutsideCameraEdge(Vector3 wenPos)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                cam = UnityEngine.Object.FindAnyObjectByType<Camera>();
            }

            float halfWidth = 10.5f;
            float camX = wenPos.x;
            if (cam != null && cam.orthographic)
            {
                halfWidth = cam.orthographicSize * cam.aspect;
                camX = cam.transform.position.x;
            }

            float spawnX = camX + halfWidth + offscreenMargin;
            float spawnY = wenPos.y + 0.35f;
            transform.position = new Vector3(spawnX, spawnY, transform.position.z);
        }

        private void UpdateFacingAndSorting(float horizontalDir, float logicalY)
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null) return;

            if (Mathf.Abs(horizontalDir) > 0.02f)
            {
                _spriteRenderer.flipX = horizontalDir < 0f;
            }
            _spriteRenderer.sortingOrder = Mathf.RoundToInt(-logicalY * 100f);
        }
    }
}
