using UnityEngine;
using UnityEngine.EventSystems;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;

namespace MoonpetalValley.Player
{
    public class PlayerToolController : MonoBehaviour
    {
        [SerializeField] private PlayerStatsSO statsConfig;
        [SerializeField] private PlayerController2D playerController;
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private WenSpriteAnimator spriteAnimator;

        private float _cooldownTimer;
        private float _swingVisualTimer;

        private void Awake()
        {
            if (spriteAnimator == null) spriteAnimator = GetComponent<WenSpriteAnimator>();
            if (playerController == null) playerController = GetComponent<PlayerController2D>();
            if (inputReader == null) inputReader = GetComponent<GameInputReader>();
        }

        private void Update()
        {
            if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;

            bool useSpriteAnim = spriteAnimator != null && spriteAnimator.HasValidSprites;
            if (!useSpriteAnim && _swingVisualTimer > 0f && visualTransform != null)
            {
                _swingVisualTimer -= Time.deltaTime;
                float s = _swingVisualTimer > 0f ? 1.12f : 1.0f;
                visualTransform.localScale = new Vector3(s, s, 1f);
            }

            if (GameRoot.Instance == null || GameRoot.Instance.State == null || !GameRoot.Instance.State.IsGameplayActive)
            {
                return;
            }

            if (inputReader == null || playerController == null) return;

            // Bam E de thu hoach cay da chin gan Wen (chi khi khong dang nham NPC / do vat tuong tac)
            if (inputReader.InteractPressed)
            {
                bool hasFocused = InteractionManager.Instance != null && InteractionManager.Instance.HasFocusedInteractable;
                if (!hasFocused)
                {
                    TryHarvestNearby();
                }
            }

            // Bam J / Chuot trai de dung cong cu hoac gieo hat
            if (inputReader.ActionPressed && _cooldownTimer <= 0f)
            {
                bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                if (!overUI)
                {
                    TryUseActiveTool();
                }
            }
        }

        private void TryHarvestNearby()
        {
            if (FarmGridManager.Instance == null || GameRoot.Instance.Inventory == null) return;
            Vector2 probe = (Vector2)transform.position + playerController.FacingDirection * 0.65f;
            var tile = FarmGridManager.Instance.FindClosestTile(probe, 1.35f);
            if (tile == null) tile = FarmGridManager.Instance.FindClosestTile(transform.position, 1.35f);

            if (tile != null && tile.IsReadyToHarvest)
            {
                if (tile.TryHarvest(out ItemData cropItem, out int yieldCount))
                {
                    GameRoot.Instance.Inventory.AddItem(cropItem, yieldCount, true);
                    TriggerSwingFeedback(WenToolAnimType.GenericSeedHarvest);
                }
            }
        }

        private void TryUseActiveTool()
        {
            if (GameRoot.Instance.Inventory == null) return;
            var slot = GameRoot.Instance.Inventory.GetSelectedSlot();
            if (slot == null || slot.IsEmpty) return;

            ItemData item = slot.item;
            Vector2 targetPos = (Vector2)transform.position + playerController.FacingDirection * 0.90f;

            // 0. Phan Bon Tinh Bui (chin tuc thi + hoa Phat Sang x2)
            if (item.itemID == "item_star_fertilizer")
            {
                if (FarmGridManager.Instance != null)
                {
                    var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.2f);
                    if (tile != null && tile.TryApplyStarFertilizer())
                    {
                        GameRoot.Instance.Inventory.RemoveItem(item.itemID, 1);
                        TriggerSwingFeedback(WenToolAnimType.GenericSeedHarvest);
                        GameEventBus.RaiseToastRequested("Phân Bón Tinh Bụi phát sáng! Cây chín tức thì và x2 sản lượng!");
                        return;
                    }
                }
                GameEventBus.RaiseToastRequested("Hãy đứng cạnh cây đã gieo hạt để rắc Phân Bón Tinh Bụi!");
                return;
            }

            // 0b. An/uong truc tiep Consumable dang cam tren Hotbar
            if (item.category == ItemCategory.Consumable && PlayerCombatBuffController.Instance != null)
            {
                if (PlayerCombatBuffController.Instance.ConsumeItemById(item.itemID))
                    _cooldownTimer = 0.35f;
                return;
            }

            // 1. Gieo hat
            if (item.category == ItemCategory.Seed && item.linkedCropData != null)
            {
                if (FarmGridManager.Instance == null) return;
                var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                if (tile != null && tile.TryPlant(item.linkedCropData))
                {
                    GameRoot.Instance.Inventory.RemoveItem(item.itemID, 1);
                    TriggerSwingFeedback(WenToolAnimType.GenericSeedHarvest);
                    GameEventBus.RaiseToastRequested("Đã gieo " + GameRoot.Instance.Inventory.GetLocalizedItemName(item) + "!");
                }
                else
                {
                    GameEventBus.RaiseToastRequested("Hãy dùng Cuốc Đất (phím 1) xới ô đất trong Nhà Kính trước khi gieo hạt.");
                }
                return;
            }

            // 2. Cuoc Dat (xoi dat hoac dap rac Nha Kinh)
            if (item.itemID == "item_hoe")
            {
                int cost = statsConfig != null ? statsConfig.hoeStaminaCost : 4;
                if (TryHitResourceNode(targetPos, "item_hoe", cost, WenToolAnimType.Hoe)) return;

                if (FarmGridManager.Instance != null)
                {
                    var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                    if (tile != null && tile.TryTill())
                    {
                        GameRoot.Instance.Stats?.ConsumeStamina(cost);
                        TriggerSwingFeedback(WenToolAnimType.Hoe);
                        return;
                    }
                }
                TriggerSwingFeedback(WenToolAnimType.Hoe);
                return;
            }

            // 3. Binh Tuoi
            if (item.itemID == "item_watering_can" || item.itemID == "item_watering_can_lv2")
            {
                if (FarmGridManager.Instance != null)
                {
                    var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                    if (tile != null && tile.State != FarmTileState.Untilled && !tile.IsWatered)
                    {
                        if (GameRoot.Instance.Inventory.ConsumeWaterCharge())
                        {
                            bool isAoE = item.itemID == "item_watering_can_lv2"
                                || (GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag("flag_forge_watering_lv2"));
                            if (isAoE)
                            {
                                var areaTiles = FarmGridManager.Instance.FindTilesInRadius(tile.transform.position, 1.65f);
                                for (int k = 0; k < areaTiles.Count; k++)
                                    if (areaTiles[k] != null && areaTiles[k].State != FarmTileState.Untilled && !areaTiles[k].IsWatered)
                                        areaTiles[k].TryWater();
                                if (JuiceEffectsManager.Instance != null)
                                    JuiceEffectsManager.Instance.SpawnFloatingNumber(tile.transform.position + Vector3.up * 0.7f, "TƯỚI 3x3!", new Color(0.45f, 0.94f, 1f));
                            }
                            else
                            {
                                tile.TryWater();
                            }
                            int cost = statsConfig != null ? statsConfig.waterStaminaCost : 2;
                            GameRoot.Instance.Stats?.ConsumeStamina(cost);
                            TriggerSwingFeedback(WenToolAnimType.WateringCan);
                        }
                        return;
                    }
                }
                if (GameRoot.Instance.Inventory.WateringCanCharges > 0)
                {
                    TriggerSwingFeedback(WenToolAnimType.WateringCan);
                }
                return;
            }

            // 4. Riu Nho (chat nhanh cay kho hoac don rac Nha Kinh)
            if (item.itemID == "item_hatchet")
            {
                int cost = statsConfig != null ? statsConfig.chopStaminaCost : 4;
                if (!TryHitResourceNode(targetPos, "item_hatchet", cost, WenToolAnimType.Hatchet))
                {
                    TriggerSwingFeedback(WenToolAnimType.Hatchet);
                }
            }
        }

        private bool TryHitResourceNode(Vector2 targetPos, string toolId, int staminaCost, WenToolAnimType animType)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.95f);
            for (int i = 0; i < hits.Length; i++)
            {
                var node = hits[i].GetComponent<ResourceNodeController>();
                if (node != null && !node.IsDepleted)
                {
                    if (node.TryHit(toolId))
                    {
                        GameRoot.Instance.Stats?.ConsumeStamina(staminaCost);
                        TriggerSwingFeedback(animType);
                    }
                    return true;
                }
            }
            return false;
        }

        private void TriggerSwingFeedback(WenToolAnimType animType)
        {
            _cooldownTimer = animType == WenToolAnimType.WateringCan ? 0.38f : 0.32f;
            if (spriteAnimator != null && spriteAnimator.HasValidSprites)
            {
                spriteAnimator.PlayToolAction(animType, playerController != null ? playerController.FacingDirection : Vector2.down);
            }
            else
            {
                _swingVisualTimer = 0.14f;
            }
        }
    }
}
