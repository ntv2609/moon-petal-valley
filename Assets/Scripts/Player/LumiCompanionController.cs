using UnityEngine;

namespace MoonpetalValley.Player
{
    public class LumiCompanionController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private SpriteRenderer haloRenderer;
        [SerializeField] private Vector2 followOffset = new Vector2(-0.75f, 0.75f);
        [SerializeField] private float smoothSpeed = 6.5f;

        private float _bobTime;

        private void LateUpdate()
        {
            if (followTarget == null)
            {
                var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (player != null) followTarget = player.transform;
                else return;
            }

            Vector3 desired = followTarget.position + new Vector3(followOffset.x, followOffset.y, 0f);
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smoothSpeed);

            _bobTime += Time.deltaTime * 3.2f;
            if (visualTransform != null)
            {
                visualTransform.localPosition = new Vector3(0f, Mathf.Sin(_bobTime) * 0.14f, 0f);
            }

            if (haloRenderer != null)
            {
                float pulse = 0.85f + Mathf.Sin(_bobTime * 1.5f) * 0.15f;
                haloRenderer.transform.localScale = new Vector3(pulse, pulse, 1f);
            }
        }

        public void SetHaloRadiusScale(float scale)
        {
            if (haloRenderer != null)
            {
                haloRenderer.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
    }
}
