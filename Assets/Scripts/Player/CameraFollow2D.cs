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

        public void TriggerShake(float intensity, float duration)
        {
            _shakeIntensity = Mathf.Max(_shakeIntensity, intensity);
            _shakeTimer = Mathf.Max(_shakeTimer, duration);
        }

        private void LateUpdate()
        {
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
