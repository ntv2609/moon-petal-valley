using UnityEngine;

namespace MoonpetalValley.Player
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.16f;
        [SerializeField] private Vector2 minBounds = new Vector2(-14f, -11f);
        [SerializeField] private Vector2 maxBounds = new Vector2(14f, 11f);

        private Vector3 _velocity;
        private float _shakeTimer;
        private float _shakeIntensity;

        private bool _cutsceneOverride;
        private Vector3 _cutsceneTargetPos;
        private float _cutsceneTargetOrtho = 5f;
        private float _cutsceneSmoothSpeed = 2.4f;
        private float _defaultOrthoSize = 5f;
        private Camera _cam;

        public float DefaultOrthoSize => _defaultOrthoSize;
        public bool IsCutsceneOverride => _cutsceneOverride;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            if (_cam != null && _cam.orthographic)
            {
                _defaultOrthoSize = _cam.orthographicSize;
                _cutsceneTargetOrtho = _defaultOrthoSize;
            }
        }

        public void TriggerShake(float intensity, float duration)
        {
            _shakeIntensity = Mathf.Max(_shakeIntensity, intensity);
            _shakeTimer = Mathf.Max(_shakeTimer, duration);
        }

        public void BeginCutsceneOverride(Vector3 startPos, float startOrthoSize)
        {
            if (_cam == null) _cam = GetComponent<Camera>();
            _cutsceneOverride = true;
            _velocity = Vector3.zero;
            startPos.z = -10f;
            _cutsceneTargetPos = startPos;
            _cutsceneTargetOrtho = startOrthoSize;
            transform.position = startPos;
            if (_cam != null && _cam.orthographic)
            {
                _cam.orthographicSize = startOrthoSize;
            }
        }

        public void SetCutsceneTarget(Vector3 worldPos, float orthoSize, float smoothSpeed = 2.2f)
        {
            _cutsceneOverride = true;
            worldPos.z = -10f;
            _cutsceneTargetPos = worldPos;
            _cutsceneTargetOrtho = orthoSize;
            _cutsceneSmoothSpeed = Mathf.Max(0.25f, smoothSpeed);
        }

        public void EndCutsceneOverride(bool snapToPlayer = false)
        {
            _cutsceneOverride = false;
            _velocity = Vector3.zero;
            if (_cam == null) _cam = GetComponent<Camera>();
            if (_cam != null && _cam.orthographic)
            {
                _cam.orthographicSize = _defaultOrthoSize;
            }

            if (snapToPlayer)
            {
                if (target == null)
                {
                    var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                    if (player != null) target = player.transform;
                }
                if (target != null)
                {
                    float tx = Mathf.Clamp(target.position.x, minBounds.x, maxBounds.x);
                    float ty = Mathf.Clamp(target.position.y, minBounds.y, maxBounds.y);
                    transform.position = new Vector3(tx, ty, -10f);
                }
            }
        }

        private void LateUpdate()
        {
            if (_cutsceneOverride)
            {
                float t = 1f - Mathf.Exp(-_cutsceneSmoothSpeed * Time.unscaledDeltaTime);
                Vector3 nextPos = Vector3.Lerp(transform.position, _cutsceneTargetPos, t);
                nextPos.z = -10f;

                if (_shakeTimer > 0f)
                {
                    _shakeTimer -= Time.unscaledDeltaTime;
                    Vector2 offset = Random.insideUnitCircle * _shakeIntensity;
                    nextPos.x += offset.x;
                    nextPos.y += offset.y;
                    if (_shakeTimer <= 0f) _shakeIntensity = 0f;
                }

                transform.position = nextPos;
                if (_cam != null && _cam.orthographic)
                {
                    _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _cutsceneTargetOrtho, t);
                }
                return;
            }

            if (target == null)
            {
                var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (player != null) target = player.transform;
                else return;
            }

            float targetX = Mathf.Clamp(target.position.x, minBounds.x, maxBounds.x);
            float targetY = Mathf.Clamp(target.position.y, minBounds.y, maxBounds.y);
            Vector3 desired = new Vector3(targetX, targetY, -10f);

            Vector3 smoothed = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);

            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                Vector2 offset = Random.insideUnitCircle * _shakeIntensity;
                smoothed.x += offset.x;
                smoothed.y += offset.y;
                if (_shakeTimer <= 0f) _shakeIntensity = 0f;
            }

            transform.position = smoothed;
        }
    }
}
