using UnityEngine;

namespace MoonpetalValley.Core
{
    [DisallowMultipleComponent]
    public class YSortRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private bool isDynamic = false;
        [SerializeField] private bool useSpriteBottomBounds = true;
        [SerializeField] private float footYOffset = 0f;
        [SerializeField] private int orderBias = 0;

        public bool IsDynamic
        {
            get => isDynamic;
            set => isDynamic = value;
        }

        public float FootYOffset
        {
            get => footYOffset;
            set => footYOffset = value;
        }

        public int OrderBias
        {
            get => orderBias;
            set => orderBias = value;
        }

        private void Awake()
        {
            if (targetRenderer == null) targetRenderer = GetComponent<SpriteRenderer>();
            RefreshSort();
        }

        private void Start()
        {
            RefreshSort();
        }

        private void LateUpdate()
        {
            if (isDynamic)
            {
                RefreshSort();
            }
        }

        public void Configure(SpriteRenderer sr, bool dynamicSort, bool useBottomBounds = true, float yOffset = 0f, int bias = 0)
        {
            targetRenderer = sr != null ? sr : GetComponent<SpriteRenderer>();
            isDynamic = dynamicSort;
            useSpriteBottomBounds = useBottomBounds;
            footYOffset = yOffset;
            orderBias = bias;
            RefreshSort();
        }

        public void RefreshSort()
        {
            if (targetRenderer == null) targetRenderer = GetComponent<SpriteRenderer>();
            if (targetRenderer == null) return;

            targetRenderer.sortingLayerName = "WorldObjects";
            float footY = useSpriteBottomBounds && targetRenderer.sprite != null
                ? targetRenderer.bounds.min.y + footYOffset
                : transform.position.y + footYOffset;

            targetRenderer.sortingOrder = Mathf.RoundToInt(-footY * 100f) + orderBias;
        }
    }
}
