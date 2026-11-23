using UnityEngine;

namespace MoonpetalValley.Chapter2
{
    [DisallowMultipleComponent]
    public class RiverMistDrifter : MonoBehaviour
    {
        [SerializeField] private float driftSpeed = 0.28f;
        [SerializeField] private float driftRange = 2.4f;
        [SerializeField] private float baseAlpha = 0.42f;
        [SerializeField] private float alphaPulse = 0.12f;
        [SerializeField] private float pulseSpeed = 0.55f;
        [SerializeField] private float phaseOffset = 0f;

        private SpriteRenderer sr;
        private Vector3 originPos;

        public void Configure(float speed, float range, float alpha, float phase)
        {
            driftSpeed = speed;
            driftRange = range;
            baseAlpha = alpha;
            phaseOffset = phase;
        }

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            originPos = transform.position;
        }

        private void Update()
        {
            float t = Time.time;
            float dx = Mathf.Sin(t * driftSpeed + phaseOffset) * driftRange;
            float dy = Mathf.Cos(t * driftSpeed * 0.5f + phaseOffset) * 0.14f;
            transform.position = new Vector3(originPos.x + dx, originPos.y + dy, originPos.z);

            if (sr != null)
            {
                Color c = sr.color;
                c.a = Mathf.Clamp01(baseAlpha + Mathf.Sin(t * pulseSpeed + phaseOffset) * alphaPulse);
                sr.color = c;
            }
        }
    }
}
