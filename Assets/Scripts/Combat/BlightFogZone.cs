using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Combat
{
    public class BlightFogZone : MonoBehaviour
    {
        [SerializeField] private string clearOnFlag = "flag_nv1_completed";
        [SerializeField] private SpriteRenderer fogRenderer;

        private bool _isClearing;
        private float _driftPhase;
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.position;
            _driftPhase = Random.Range(0f, 6.28f);
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                GameRoot.Instance.Flags.HasFlag(clearOnFlag))
            {
                gameObject.SetActive(false);
            }
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value && flagId == clearOnFlag)
            {
                _isClearing = true;
            }
        }

        private void Update()
        {
            _driftPhase += Time.deltaTime * 0.7f;
            transform.position = _startPos + new Vector3(Mathf.Sin(_driftPhase) * 0.35f, Mathf.Cos(_driftPhase * 0.8f) * 0.15f, 0f);

            if (_isClearing && fogRenderer != null)
            {
                Color c = fogRenderer.color;
                c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * 0.45f);
                fogRenderer.color = c;
                if (c.a <= 0.01f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
