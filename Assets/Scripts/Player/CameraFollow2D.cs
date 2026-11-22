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

            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        }
    }
}
