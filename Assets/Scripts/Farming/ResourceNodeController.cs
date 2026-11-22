using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Inventory;

namespace MoonpetalValley.Farming
{
    public enum ResourceNodeType
    {
        DryBranch,
        GreenhouseDebris
    }

    public class ResourceNodeController : MonoBehaviour
    {
        [SerializeField] private ResourceNodeType nodeType = ResourceNodeType.DryBranch;
        [SerializeField] private string requiredToolId = "item_hatchet";
        [SerializeField] private int maxHits = 2;
        [SerializeField] private ItemData dropItem;
        [SerializeField] private int dropAmount = 2;
        [SerializeField] private GameObject lootDropPrefab;
        [SerializeField] private bool regrowEachDay = true;
        [SerializeField] private SpriteRenderer visualRenderer;
        [SerializeField] private Collider2D nodeCollider;

        private int _currentHits;
        private bool _isDepleted;
        private float _shakeTimer;
        private Vector3 _baseLocalPos;

        public bool IsDepleted => _isDepleted;
        public ResourceNodeType NodeType => nodeType;

        private void Awake()
        {
            if (visualRenderer != null) _baseLocalPos = visualRenderer.transform.localPosition;
        }

        private void OnEnable()
        {
            GameEventBus.OnDayStarted += HandleDayStarted;
        }

        private void OnDisable()
        {
            GameEventBus.OnDayStarted -= HandleDayStarted;
        }

        private void Update()
        {
            if (_shakeTimer > 0f && visualRenderer != null)
            {
                _shakeTimer -= Time.deltaTime;
                float offset = Mathf.Sin(_shakeTimer * 55f) * 0.08f;
                visualRenderer.transform.localPosition = _baseLocalPos + new Vector3(offset, 0f, 0f);
                if (_shakeTimer <= 0f)
                {
                    visualRenderer.transform.localPosition = _baseLocalPos;
                }
            }
        }

        private void HandleDayStarted(int day)
        {
            if (regrowEachDay && _isDepleted)
            {
                RestoreNode();
            }
        }

        public bool TryHit(string toolId)
        {
            if (_isDepleted) return false;

            bool validTool = toolId == requiredToolId ||
                             (nodeType == ResourceNodeType.GreenhouseDebris && (toolId == "item_hoe" || toolId == "item_hatchet"));
            if (!validTool)
            {
                string needed = requiredToolId == "item_hatchet" ? "Rìu Nhỏ (phím 3)" : "Cuốc Đất (phím 1)";
                GameEventBus.RaiseToastRequested("Cần dùng " + needed + " để dọn vật thể này!");
                return false;
            }

            _currentHits++;
            _shakeTimer = 0.22f;

            if (_currentHits >= maxHits)
            {
                DepleteNode();
            }
            return true;
        }

        private void DepleteNode()
        {
            _isDepleted = true;
            if (visualRenderer != null) visualRenderer.enabled = false;
            if (nodeCollider != null) nodeCollider.enabled = false;

            if (dropItem != null && dropAmount > 0)
            {
                if (lootDropPrefab != null)
                {
                    for (int i = 0; i < dropAmount; i++)
                    {
                        Vector2 offset = new Vector2(Random.Range(-0.35f, 0.35f), Random.Range(-0.25f, 0.25f));
                        GameObject go = Instantiate(lootDropPrefab, transform.position + (Vector3)offset, Quaternion.identity);
                        var drop = go.GetComponent<WorldLootDrop>();
                        if (drop != null) drop.Initialize(dropItem, 1);
                    }
                }
                else if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
                {
                    GameRoot.Instance.Inventory.AddItem(dropItem, dropAmount, true);
                }
            }

            if (nodeType == ResourceNodeType.GreenhouseDebris)
            {
                CheckAllGreenhouseDebrisCleared();
            }
        }

        private void CheckAllGreenhouseDebrisCleared()
        {
            var allNodes = UnityEngine.Object.FindObjectsByType<ResourceNodeController>(FindObjectsInactive.Include);
            int remaining = 0;
            for (int i = 0; i < allNodes.Length; i++)
            {
                if (allNodes[i].NodeType == ResourceNodeType.GreenhouseDebris && !allNodes[i].IsDepleted)
                {
                    remaining++;
                }
            }

            if (remaining == 0 && GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag("flag_greenhouse_cleared", true);
                GameEventBus.RaiseToastRequested("Đã dọn sạch cỏ dại & đá vụn trong Nhà Kính!");
            }
        }

        public void RestoreNode()
        {
            _isDepleted = false;
            _currentHits = 0;
            if (visualRenderer != null) visualRenderer.enabled = true;
            if (nodeCollider != null) nodeCollider.enabled = true;
        }
    }
}
