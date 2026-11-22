using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.Inventory
{
    public class WorldLootDrop : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int amount = 1;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private PlayerController2D _player;
        private float _bobPhase;
        private float _spawnLockTimer = 0.25f;

        public void Initialize(ItemData data, int count)
        {
            itemData = data;
            amount = Mathf.Max(1, count);
            if (spriteRenderer != null && itemData != null && itemData.icon != null)
            {
                spriteRenderer.sprite = itemData.icon;
            }
        }

        private void Start()
        {
            _bobPhase = Random.Range(0f, 6.28f);
            if (spriteRenderer != null && itemData != null && itemData.icon != null)
            {
                spriteRenderer.sprite = itemData.icon;
            }
        }

        private void Update()
        {
            if (_spawnLockTimer > 0f) _spawnLockTimer -= Time.deltaTime;

            _bobPhase += Time.deltaTime * 5f;
            if (spriteRenderer != null)
            {
                spriteRenderer.transform.localPosition = new Vector3(0f, Mathf.Sin(_bobPhase) * 0.08f, 0f);
            }

            if (_spawnLockTimer > 0f) return;

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player == null) return;
            }

            float dist = Vector2.Distance(transform.position, _player.transform.position);
            if (dist <= 2.3f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Time.deltaTime * 7.5f);
            }
            if (dist <= 0.55f)
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null && itemData != null)
                {
                    if (GameRoot.Instance.Inventory.AddItem(itemData, amount, true))
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
