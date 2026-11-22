# deploy.py — Moonpetal Valley — Step 6
import os
import sys
import shutil
from pathlib import Path
from datetime import datetime

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

PROJECT_ROOT = Path.cwd()
ASSETS_DIR = PROJECT_ROOT / "Assets"
SETTINGS_DIR = PROJECT_ROOT / "ProjectSettings"

if not ASSETS_DIR.is_dir() or not SETTINGS_DIR.is_dir():
    print("[LỖI] Không tìm thấy thư mục 'Assets/' và 'ProjectSettings/' tại thư mục hiện tại.")
    sys.exit(1)

FILES = {
    # =========================================================================
    # 1. FARM TILE CONTROLLER (UNTILLED -> TILLED -> PLANTED -> WATERED -> HARVEST)
    # =========================================================================
    "Assets/Scripts/Farming/FarmTileController.cs": r'''using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Farming
{
    public enum FarmTileState
    {
        Untilled,
        TilledDry,
        Planted
    }

    public class FarmTileController : MonoBehaviour
    {
        [SerializeField] private string tileId;
        [SerializeField] private SpriteRenderer soilRenderer;
        [SerializeField] private SpriteRenderer cropRenderer;
        [SerializeField] private Sprite untilledSprite;
        [SerializeField] private Sprite tilledDrySprite;
        [SerializeField] private Sprite tilledWetSprite;

        public string TileId => tileId;
        public FarmTileState State { get; private set; } = FarmTileState.Untilled;
        public CropData PlantedCrop { get; private set; }
        public int GrowthStage { get; private set; }
        public bool IsWatered { get; private set; }

        public bool IsReadyToHarvest => State == FarmTileState.Planted && PlantedCrop != null && GrowthStage >= PlantedCrop.growDays;

        private void Start()
        {
            RefreshVisuals();
        }

        public bool TryTill()
        {
            if (State != FarmTileState.Untilled) return false;
            State = FarmTileState.TilledDry;
            IsWatered = false;
            RefreshVisuals();
            return true;
        }

        public bool TryPlant(CropData crop)
        {
            if (crop == null || State != FarmTileState.TilledDry) return false;
            State = FarmTileState.Planted;
            PlantedCrop = crop;
            GrowthStage = 0;
            RefreshVisuals();
            return true;
        }

        public bool TryWater()
        {
            if (State == FarmTileState.Untilled || IsWatered) return false;
            IsWatered = true;
            RefreshVisuals();
            return true;
        }

        public bool TryHarvest(out ItemData harvestedItem, out int yieldAmount)
        {
            harvestedItem = null;
            yieldAmount = 0;
            if (!IsReadyToHarvest) return false;

            harvestedItem = PlantedCrop.harvestedItem;
            yieldAmount = Mathf.Max(1, PlantedCrop.harvestYield);

            State = FarmTileState.TilledDry;
            PlantedCrop = null;
            GrowthStage = 0;
            IsWatered = false;
            RefreshVisuals();
            return true;
        }

        public void OnNewDay()
        {
            if (State == FarmTileState.Planted && PlantedCrop != null)
            {
                if (IsWatered && GrowthStage < PlantedCrop.growDays)
                {
                    GrowthStage++;
                }
            }
            IsWatered = false;
            RefreshVisuals();
        }

        public void ForceWaterDebug()
        {
            if (State != FarmTileState.Untilled)
            {
                IsWatered = true;
                RefreshVisuals();
            }
        }

        public void RestoreState(FarmTileState state, CropData crop, int stage, bool watered)
        {
            State = state;
            PlantedCrop = crop;
            GrowthStage = stage;
            IsWatered = watered;
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            if (soilRenderer != null)
            {
                if (State == FarmTileState.Untilled) soilRenderer.sprite = untilledSprite;
                else soilRenderer.sprite = IsWatered ? tilledWetSprite : tilledDrySprite;
            }

            if (cropRenderer != null)
            {
                if (State == FarmTileState.Planted && PlantedCrop != null && PlantedCrop.stageSprites != null && PlantedCrop.stageSprites.Length > 0)
                {
                    int maxIdx = PlantedCrop.stageSprites.Length - 1;
                    int idx = IsReadyToHarvest
                        ? maxIdx
                        : Mathf.Clamp(GrowthStage, 0, Mathf.Max(0, maxIdx - 1));
                    cropRenderer.sprite = PlantedCrop.stageSprites[idx];
                    cropRenderer.enabled = true;
                }
                else
                {
                    cropRenderer.sprite = null;
                    cropRenderer.enabled = false;
                }
            }
        }
    }
}
''',

    # =========================================================================
    # 2. FARM GRID MANAGER (MANAGES ALL GREENHOUSE PLOTS, ISAVEABLE)
    # =========================================================================
    "Assets/Scripts/Farming/FarmGridManager.cs": r'''using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Farming
{
    public class FarmGridManager : MonoBehaviour, ISaveable
    {
        public static FarmGridManager Instance { get; private set; }

        [SerializeField] private List<FarmTileController> tiles = new List<FarmTileController>();
        [SerializeField] private CropData defaultCropData;

        public string SaveKey => "farm_tiles";

        [Serializable]
        private struct SavedTile
        {
            public string id;
            public int state;
            public string cropId;
            public int stage;
            public bool watered;
        }

        [Serializable]
        private class FarmSaveWrapper
        {
            public List<SavedTile> savedTiles = new List<SavedTile>();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GameEventBus.OnDayStarted += HandleDayStarted;
        }

        private void OnDisable()
        {
            GameEventBus.OnDayStarted -= HandleDayStarted;
        }

        private void Start()
        {
            if (GameRoot.Instance != null)
            {
                GameRoot.Instance.RegisterSaveable(this);
            }
        }

        private void HandleDayStarted(int newDay)
        {
            int readyCount = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    tiles[i].OnNewDay();
                    if (tiles[i].IsReadyToHarvest) readyCount++;
                }
            }
            if (readyCount > 0)
            {
                GameEventBus.RaiseToastRequested("Trong Nhà Kính có " + readyCount + " cây Lúa Mạch Sương Mai đã chín! Bấm [E] để thu hoạch.");
            }
        }

        public FarmTileController FindClosestTile(Vector2 worldPos, float maxDistance = 1.15f)
        {
            FarmTileController best = null;
            float bestDist = maxDistance;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) continue;
                float d = Vector2.Distance(worldPos, tiles[i].transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = tiles[i];
                }
            }
            return best;
        }

        public int CountPlantedCrops(string cropId)
        {
            int count = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null && tiles[i].State == FarmTileState.Planted &&
                    tiles[i].PlantedCrop != null && tiles[i].PlantedCrop.cropID == cropId)
                {
                    count++;
                }
            }
            return count;
        }

        public void WaterAllTilesDebug()
        {
            int watered = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null && tiles[i].State != FarmTileState.Untilled)
                {
                    tiles[i].ForceWaterDebug();
                    watered++;
                }
            }
            GameEventBus.RaiseToastRequested("Đã tưới nước toàn bộ " + watered + " ô đất đã cuốc!");
        }

        public string CaptureStateJson()
        {
            var wrapper = new FarmSaveWrapper();
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) continue;
                wrapper.savedTiles.Add(new SavedTile
                {
                    id = tiles[i].TileId,
                    state = (int)tiles[i].State,
                    cropId = tiles[i].PlantedCrop != null ? tiles[i].PlantedCrop.cropID : "",
                    stage = tiles[i].GrowthStage,
                    watered = tiles[i].IsWatered
                });
            }
            return JsonUtility.ToJson(wrapper);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var wrapper = JsonUtility.FromJson<FarmSaveWrapper>(json);
            if (wrapper == null || wrapper.savedTiles == null) return;

            for (int i = 0; i < wrapper.savedTiles.Count; i++)
            {
                var st = wrapper.savedTiles[i];
                for (int j = 0; j < tiles.Count; j++)
                {
                    if (tiles[j] != null && tiles[j].TileId == st.id)
                    {
                        CropData crop = !string.IsNullOrEmpty(st.cropId) ? defaultCropData : null;
                        tiles[j].RestoreState((FarmTileState)st.state, crop, st.stage, st.watered);
                        break;
                    }
                }
            }
        }
    }
}
''',

    # =========================================================================
    # 3. RENEWABLE RESOURCE NODE (DRY BRANCHES & GREENHOUSE DEBRIS)
    # =========================================================================
    "Assets/Scripts/Farming/ResourceNodeController.cs": r'''using UnityEngine;
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
''',

    # =========================================================================
    # 4. WATER WELL INTERACTABLE (REFILL WATERING CAN 20/20)
    # =========================================================================
    "Assets/Scripts/Farming/WaterWellInteractable.cs": r'''using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Farming
{
    public class WaterWellInteractable : MonoBehaviour
    {
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 2.0f;

        private PlayerController2D _player;
        private GameInputReader _input;

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
                {
                    GameRoot.Instance.Inventory.RefillWateringCan();
                }
            }
        }
    }
}
''',

    # =========================================================================
    # 5. SEED SHELF INTERACTABLE (ANTI-SOFTLOCK FREE SEEDS IN GREENHOUSE)
    # =========================================================================
    "Assets/Scripts/Farming/SeedShelfInteractable.cs": r'''using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Farming
{
    public class SeedShelfInteractable : MonoBehaviour
    {
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 1.9f;

        private PlayerController2D _player;
        private GameInputReader _input;

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                TryGiveBackupSeeds();
            }
        }

        private void TryGiveBackupSeeds()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            int seedsInBag = inv.GetItemCount("item_dew_barley_seed");
            int cropsInBag = inv.GetItemCount("item_dew_barley");
            int planted = FarmGridManager.Instance != null ? FarmGridManager.Instance.CountPlantedCrops("crop_dew_barley") : 0;
            int total = seedsInBag + cropsInBag + planted;

            if (total >= 6)
            {
                GameEventBus.RaiseToastRequested("Bạn đã có đủ Hạt/Cây Lúa Mạch Sương Mai rồi (" + total + "/6).");
                return;
            }

            int give = 6 - total;
            inv.AddItemById("item_dew_barley_seed", give, true);
        }
    }
}
''',

    # =========================================================================
    # 6. PLAYER TOOL CONTROLLER (HOE, WATERING CAN, HATCHET, PLANTING & HARVEST)
    # =========================================================================
    "Assets/Scripts/Player/PlayerToolController.cs": r'''using UnityEngine;
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

        private float _cooldownTimer;
        private float _swingVisualTimer;

        private void Update()
        {
            if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;

            if (_swingVisualTimer > 0f && visualTransform != null)
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

            // Bấm E để thu hoạch cây đã chín gần Wen
            if (inputReader.InteractPressed)
            {
                TryHarvestNearby();
            }

            // Bấm J / Chuột trái để dùng công cụ hoặc gieo hạt
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
                    TriggerSwingFeedback();
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

            // 1. Gieo hạt
            if (item.category == ItemCategory.Seed && item.linkedCropData != null)
            {
                if (FarmGridManager.Instance == null) return;
                var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                if (tile != null && tile.TryPlant(item.linkedCropData))
                {
                    GameRoot.Instance.Inventory.RemoveItem(item.itemID, 1);
                    TriggerSwingFeedback();
                    GameEventBus.RaiseToastRequested("Đã gieo " + GameRoot.Instance.Inventory.GetLocalizedItemName(item) + "!");
                }
                else
                {
                    GameEventBus.RaiseToastRequested("Hãy dùng Cuốc Đất (phím 1) xới ô đất trong Nhà Kính trước khi gieo hạt.");
                }
                return;
            }

            // 2. Cuốc Đất (xới đất hoặc đập rác Nhà Kính)
            if (item.itemID == "item_hoe")
            {
                int cost = statsConfig != null ? statsConfig.hoeStaminaCost : 4;
                if (TryHitResourceNode(targetPos, "item_hoe", cost)) return;

                if (FarmGridManager.Instance != null)
                {
                    var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                    if (tile != null && tile.TryTill())
                    {
                        GameRoot.Instance.Stats?.ConsumeStamina(cost);
                        TriggerSwingFeedback();
                    }
                }
                return;
            }

            // 3. Bình Tưới
            if (item.itemID == "item_watering_can")
            {
                if (FarmGridManager.Instance == null) return;
                var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                if (tile != null && tile.State != FarmTileState.Untilled && !tile.IsWatered)
                {
                    if (GameRoot.Instance.Inventory.ConsumeWaterCharge())
                    {
                        tile.TryWater();
                        int cost = statsConfig != null ? statsConfig.waterStaminaCost : 2;
                        GameRoot.Instance.Stats?.ConsumeStamina(cost);
                        TriggerSwingFeedback();
                    }
                }
                return;
            }

            // 4. Rìu Nhỏ (chặt nhánh cây khô hoặc dọn rác Nhà Kính)
            if (item.itemID == "item_hatchet")
            {
                int cost = statsConfig != null ? statsConfig.chopStaminaCost : 4;
                TryHitResourceNode(targetPos, "item_hatchet", cost);
            }
        }

        private bool TryHitResourceNode(Vector2 targetPos, string toolId, int staminaCost)
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
                        TriggerSwingFeedback();
                    }
                    return true;
                }
            }
            return false;
        }

        private void TriggerSwingFeedback()
        {
            _cooldownTimer = 0.30f;
            _swingVisualTimer = 0.14f;
        }
    }
}
''',

    # =========================================================================
    # 7. UPDATED RUNTIMEDEBUGOVERLAY.CS (FULL FILE — ADDS WATER ALL CROPS)
    # =========================================================================
    "Assets/Scripts/DebugTools/RuntimeDebugOverlay.cs": r'''using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
using MoonpetalValley.Farming;

namespace MoonpetalValley.DebugTools
{
    public class RuntimeDebugOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button btnTestToast;
        [SerializeField] private Button btnToggleTestFlag;
        [SerializeField] private Button btnClearFlags;
        [SerializeField] private Button btnTogglePause;
        [SerializeField] private Button btnTpGreenhouse;
        [SerializeField] private Button btnTpVillage;
        [SerializeField] private Button btnTpGarden;
        [SerializeField] private Button btnTpForest;
        [SerializeField] private Button btnTimePlus2H;
        [SerializeField] private Button btnTimeSetDusk;
        [SerializeField] private Button btnTimeNextDay;
        [SerializeField] private Button btnRestoreStats;
        [SerializeField] private Button btnForceExhaust;
        [SerializeField] private Button btnDamageLightHP;
        [SerializeField] private Button btnGiveTestKit;
        [SerializeField] private Button btnWaterAllCrops;

        private bool _isOpen;
        private float _refreshTimer;
        private const string TestFlagId = "flag_debug_step1_verified";

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnTestToast != null) btnTestToast.onClick.AddListener(OnClickTestToast);
            if (btnToggleTestFlag != null) btnToggleTestFlag.onClick.AddListener(OnClickToggleTestFlag);
            if (btnClearFlags != null) btnClearFlags.onClick.AddListener(OnClickClearFlags);
            if (btnTogglePause != null) btnTogglePause.onClick.AddListener(OnClickTogglePause);
            if (btnTpGreenhouse != null) btnTpGreenhouse.onClick.AddListener(() => TeleportTo("Spawn_Greenhouse"));
            if (btnTpVillage != null) btnTpVillage.onClick.AddListener(() => TeleportTo("Spawn_Village"));
            if (btnTpGarden != null) btnTpGarden.onClick.AddListener(() => TeleportTo("Spawn_BachGarden"));
            if (btnTpForest != null) btnTpForest.onClick.AddListener(() => TeleportTo("Spawn_ForestEdge"));

            if (btnTimePlus2H != null) btnTimePlus2H.onClick.AddListener(OnClickTimePlus2H);
            if (btnTimeSetDusk != null) btnTimeSetDusk.onClick.AddListener(OnClickTimeSetDusk);
            if (btnTimeNextDay != null) btnTimeNextDay.onClick.AddListener(OnClickTimeNextDay);

            if (btnRestoreStats != null) btnRestoreStats.onClick.AddListener(OnClickRestoreStats);
            if (btnForceExhaust != null) btnForceExhaust.onClick.AddListener(OnClickForceExhaust);
            if (btnDamageLightHP != null) btnDamageLightHP.onClick.AddListener(OnClickDamageLightHP);
            if (btnGiveTestKit != null) btnGiveTestKit.onClick.AddListener(OnClickGiveTestKit);
            if (btnWaterAllCrops != null) btnWaterAllCrops.onClick.AddListener(OnClickWaterAllCrops);
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged += OnGameStateUpdated;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged -= OnGameStateUpdated;
        }

        private void Update()
        {
            if (!Application.isEditor && !Debug.isDebugBuild) return;
            if (WasF1PressedThisFrame()) TogglePanel();

            if (_isOpen)
            {
                _refreshTimer += Time.unscaledDeltaTime;
                if (_refreshTimer >= 0.25f)
                {
                    _refreshTimer = 0f;
                    RefreshStatusText();
                }
            }
        }

        private bool WasF1PressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F1)) return true;
#endif
            return false;
        }

        public void TogglePanel()
        {
            _isOpen = !_isOpen;
            if (panelRoot != null) panelRoot.SetActive(_isOpen);
            if (_isOpen) RefreshStatusText();
        }

        private void TeleportTo(string spawnId)
        {
            if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo(spawnId);
                RefreshStatusText();
            }
        }

        private void OnClickTimePlus2H() { GameRoot.Instance?.Time?.AddHours(2f); RefreshStatusText(); }
        private void OnClickTimeSetDusk() { GameRoot.Instance?.Time?.SetHour(19.83f); RefreshStatusText(); }
        private void OnClickTimeNextDay() { GameRoot.Instance?.Time?.AdvanceToNextDay(); RefreshStatusText(); }
        private void OnClickRestoreStats() { GameRoot.Instance?.Stats?.RestoreStats(100, 100, true); RefreshStatusText(); }
        private void OnClickForceExhaust() { GameRoot.Instance?.Stats?.ConsumeStamina(999, true); RefreshStatusText(); }
        private void OnClickDamageLightHP() { GameRoot.Instance?.Stats?.TakeLightDamage(25, true); RefreshStatusText(); }

        private void OnClickGiveTestKit()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            inv.AddItemById("item_sunshine_bread", 2, false);
            inv.AddItemById("item_branch_wood", 10, false);
            inv.AddItemById("item_seed_bag_1", 1, false);
            inv.AddItemById("item_dew_barley_seed", 6, false);
            GameEventBus.RaiseToastRequested("Đã thêm Bộ Test Đồ (2 Bánh Mì, 10 Gỗ, 1 Túi Hạt, 6 Hạt Lúa Mạch)!");
            RefreshStatusText();
        }

        private void OnClickWaterAllCrops()
        {
            if (FarmGridManager.Instance != null) FarmGridManager.Instance.WaterAllTilesDebug();
            RefreshStatusText();
        }

        private void OnClickTestToast()
        {
            string msg = GameRoot.Instance != null && GameRoot.Instance.Loc != null
                ? GameRoot.Instance.Loc.Get("toast_step1_test")
                : "Kiểm thử thông báo tiếng Việt có dấu thành công!";
            GameEventBus.RaiseToastRequested(msg);
        }

        private void OnClickToggleTestFlag()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            bool next = !GameRoot.Instance.Flags.HasFlag(TestFlagId);
            GameRoot.Instance.Flags.SetFlag(TestFlagId, next);
            RefreshStatusText();
        }

        private void OnClickClearFlags() { GameRoot.Instance?.Flags?.ClearAllFlags(); RefreshStatusText(); }

        private void OnClickTogglePause()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            var next = GameRoot.Instance.State.IsGameplayActive ? GameState.PauseMenu : GameState.Gameplay;
            GameRoot.Instance.State.SetState(next);
            RefreshStatusText();
        }

        private void OnAnyStateUpdated(string _, bool __) => RefreshStatusText();
        private void OnGameStateUpdated(GameState _) => RefreshStatusText();

        public void RefreshStatusText()
        {
            if (statusText == null || GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("<b>BẢNG DEBUG MOONPETAL (F1)</b>");
            if (GameRoot.Instance.Time != null) sb.AppendLine(GameRoot.Instance.Time.GetFormattedClockString());
            if (GameRoot.Instance.Stats != null)
            {
                var st = GameRoot.Instance.Stats;
                sb.AppendLine(string.Format("HP: {0}/{1} | Stamina: {2}/{3}",
                    st.CurrentLightHP, st.MaxLightHP, st.CurrentStamina, st.EffectiveMaxStamina));
            }
            if (GameRoot.Instance.Inventory != null)
            {
                var inv = GameRoot.Instance.Inventory;
                sb.AppendLine("Gỗ: " + inv.GetItemCount("item_branch_wood") +
                              " | Lúa Mạch: " + inv.GetItemCount("item_dew_barley") +
                              " | Hạt: " + inv.GetItemCount("item_dew_barley_seed"));
            }
            statusText.text = sb.ToString();
        }
    }
}
''',

    # =========================================================================
    # 8. EDITOR — AUTOBUILDER STEP 6
    # =========================================================================
    "Assets/Editor/AutoBuilder_Step6.cs": r'''using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Farming;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step6
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string CropDewBarleyPath = "Assets/Data/Crops/Crop_DewBarley.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LootDropPrefabPath = "Assets/Prefabs/Enemies/LootDrop.prefab";

        [MenuItem("Moonpetal/Step 6/1. Build Farming, Crops & Resources")]
        public static void BuildStep6()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 6 ===");

            // 1. Tạo sprite đất, 4 giai đoạn Lúa Mạch Sương Mai, Giếng, Kệ Hạt, Nhánh Cây, Rác Nhà Kính
            Sprite soilUntilled = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_untilled.png", 16, 16, DrawUntilledSoil);
            Sprite soilDry = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_dry.png", 16, 16, DrawTilledDrySoil);
            Sprite soilWet = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_wet.png", 16, 16, DrawTilledWetSoil);

            Sprite[] barleyStages = new Sprite[4];
            barleyStages[0] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_0.png", 16, 16, DrawBarleyStage0);
            barleyStages[1] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_1.png", 16, 16, DrawBarleyStage1);
            barleyStages[2] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_2.png", 16, 16, DrawBarleyStage2);
            barleyStages[3] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_3.png", 16, 16, DrawBarleyStage3);

            Sprite wellSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_well.png", 16, 16, DrawWellSprite);
            Sprite shelfSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_shelf.png", 16, 16, DrawShelfSprite);
            Sprite branchSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_branch.png", 16, 16, DrawBranchSprite);
            Sprite debrisSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_debris.png", 16, 16, DrawDebrisSprite);

            // 2. Tạo Crop_DewBarley.asset & nối vào Item_item_dew_barley_seed
            CropData barleyCrop = EnsureDewBarleyCrop(barleyStages);

            // 3. Gắn PlayerToolController vào Player_Wen.prefab
            UpgradePlayerPrefabWithTools();

            // 4. Nâng cấp GameRoot.prefab thêm nút Debug Tưới Toàn Bộ Ruộng
            UpgradeGameRootDebugWithWaterAll();

            // 5. Dựng 12 ô đất, Kệ Hạt, Giếng Nước, 6 Nhánh Cây Khô, 3 Rác Nhà Kính trong Scene
            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildFarmGridInGreenhouse(barleyCrop, soilUntilled, soilDry, soilWet);
            BuildSeedShelfAndWaterWell(shelfSpr, wellSpr);
            BuildResourceNodes(branchSpr, debrisSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 6! Chạy 'Moonpetal/Step 6/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 6/Validate")]
        public static void ValidateStep6()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 6 ===");

            var crop = AssetDatabase.LoadAssetAtPath<CropData>(CropDewBarleyPath);
            if (crop == null || crop.stageSprites == null || crop.stageSprites.Length < 4)
            {
                Debug.LogError("[MV] Crop_DewBarley.asset thiếu dữ liệu hoặc chưa đủ 4 sprite giai đoạn.");
                errors++;
            }

            var seedItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley_seed.asset");
            if (seedItem == null || seedItem.linkedCropData == null)
            {
                Debug.LogError("[MV] Item_item_dew_barley_seed chưa nối linkedCropData.");
                errors++;
            }

            var farmMgr = UnityEngine.Object.FindAnyObjectByType<FarmGridManager>();
            if (farmMgr == null)
            {
                Debug.LogError("[MV] Thiếu FarmGridManager trong scene.");
                errors++;
            }

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            if (tiles.Length < 12)
            {
                Debug.LogError("[MV] Thiếu ô đất Nhà Kính (cần 12, hiện có " + tiles.Length + ").");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<PlayerToolController>() == null)
            {
                Debug.LogError("[MV] Player_Wen thiếu PlayerToolController.");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<WaterWellInteractable>() == null)
            {
                Debug.LogError("[MV] Thiếu WaterWellInteractable.");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<SeedShelfInteractable>() == null)
            {
                Debug.LogError("[MV] Thiếu SeedShelfInteractable.");
                errors++;
            }

            var nodes = UnityEngine.Object.FindObjectsByType<ResourceNodeController>(FindObjectsInactive.Include);
            if (nodes.Length < 9)
            {
                Debug.LogError("[MV] Thiếu ResourceNodeController (cần 9 = 6 Nhánh cây + 3 Rác Nhà Kính, hiện có " + nodes.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 6: PASS (0 lỗi). 12 Ô đất canh tác, Lúa Mạch 4 giai đoạn, Giếng, Kệ Hạt & Gỗ tái tạo sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 6: FAIL (" + errors + " lỗi).");
            }
        }

        private static CropData EnsureDewBarleyCrop(Sprite[] stages)
        {
            var crop = AssetDatabase.LoadAssetAtPath<CropData>(CropDewBarleyPath);
            if (crop == null)
            {
                crop = ScriptableObject.CreateInstance<CropData>();
                AssetDatabase.CreateAsset(crop, CropDewBarleyPath);
            }

            var seedItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley_seed.asset");
            var harvestItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley.asset");

            crop.cropID = "crop_dew_barley";
            crop.cropNameKey = "name_item_dew_barley";
            crop.seedItem = seedItem;
            crop.harvestedItem = harvestItem;
            crop.growDays = 2;
            crop.harvestYield = 1;
            crop.stageSprites = stages;
            EditorUtility.SetDirty(crop);

            if (seedItem != null)
            {
                seedItem.linkedCropData = crop;
                EditorUtility.SetDirty(seedItem);
            }
            return crop;
        }

        private static void UpgradePlayerPrefabWithTools()
        {
            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null) return;

            var toolCtrl = root.GetComponent<PlayerToolController>();
            if (toolCtrl == null) toolCtrl = root.AddComponent<PlayerToolController>();

            var so = new SerializedObject(toolCtrl);
            so.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            so.FindProperty("playerController").objectReferenceValue = root.GetComponent<PlayerController2D>();
            so.FindProperty("inputReader").objectReferenceValue = root.GetComponent<GameInputReader>();
            so.FindProperty("visualTransform").objectReferenceValue = root.transform.Find("Visual");
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void UpgradeGameRootDebugWithWaterAll()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 880f);
                Transform existing = debugPanel.Find("BtnWaterAllCrops");
                Button btn;
                if (existing != null)
                {
                    btn = existing.GetComponent<Button>();
                }
                else
                {
                    GameObject go = new GameObject("BtnWaterAllCrops");
                    go.transform.SetParent(debugPanel, false);
                    go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
                    var img = go.AddComponent<Image>();
                    img.color = new Color(0.18f, 0.42f, 0.62f, 1f);
                    btn = go.AddComponent<Button>();
                    btn.targetGraphic = img;

                    GameObject txtGo = new GameObject("Label");
                    txtGo.transform.SetParent(go.transform, false);
                    var r = txtGo.AddComponent<RectTransform>();
                    r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
                    r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
                    var tmp = txtGo.AddComponent<TextMeshProUGUI>();
                    tmp.text = "Tưới Toàn Bộ Ruộng Nhà Kính";
                    tmp.fontSize = 15; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
                    if (vnFont != null) tmp.font = vnFont;
                }

                var so = new SerializedObject(debugCtrl);
                so.FindProperty("btnWaterAllCrops").objectReferenceValue = btn;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildFarmGridInGreenhouse(CropData barleyCrop, Sprite untilled, Sprite dry, Sprite wet)
        {
            GameObject old = GameObject.Find("GreenhouseFarmGrid");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject gridGo = new GameObject("GreenhouseFarmGrid");
            var mgr = gridGo.AddComponent<FarmGridManager>();
            var tileList = new List<FarmTileController>();

            // 4 cột x 3 hàng gọn gàng trong Nhà Kính (x: -17..-14, y: 7..9)
            int idx = 0;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    idx++;
                    float wx = -17.5f + c * 1.15f;
                    float wy = 7.5f + r * 1.15f;

                    GameObject tGo = new GameObject("FarmTile_" + idx);
                    tGo.tag = "FarmTile";
                    tGo.transform.SetParent(gridGo.transform, false);
                    tGo.transform.position = new Vector3(wx, wy, 0f);

                    var soilSr = tGo.AddComponent<SpriteRenderer>();
                    soilSr.sprite = untilled;
                    soilSr.sortingLayerName = "FarmTile";
                    soilSr.sortingOrder = 5;

                    GameObject cropGo = new GameObject("CropVisual");
                    cropGo.transform.SetParent(tGo.transform, false);
                    cropGo.transform.localPosition = new Vector3(0f, 0.15f, 0f);
                    var cropSr = cropGo.AddComponent<SpriteRenderer>();
                    cropSr.sortingLayerName = "WorldObjects";
                    cropSr.sortingOrder = Mathf.RoundToInt(-wy * 100f) + 5;
                    cropSr.enabled = false;

                    var ctrl = tGo.AddComponent<FarmTileController>();
                    var so = new SerializedObject(ctrl);
                    so.FindProperty("tileId").stringValue = "gh_tile_" + idx;
                    so.FindProperty("soilRenderer").objectReferenceValue = soilSr;
                    so.FindProperty("cropRenderer").objectReferenceValue = cropSr;
                    so.FindProperty("untilledSprite").objectReferenceValue = untilled;
                    so.FindProperty("tilledDrySprite").objectReferenceValue = dry;
                    so.FindProperty("tilledWetSprite").objectReferenceValue = wet;
                    so.ApplyModifiedPropertiesWithoutUndo();

                    tileList.Add(ctrl);
                }
            }

            var mgrSO = new SerializedObject(mgr);
            mgrSO.FindProperty("defaultCropData").objectReferenceValue = barleyCrop;
            var prop = mgrSO.FindProperty("tiles");
            prop.ClearArray();
            for (int i = 0; i < tileList.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = tileList[i];
            }
            mgrSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildSeedShelfAndWaterWell(Sprite shelfSpr, Sprite wellSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            // 1. Kệ Hạt Giống trong Nhà Kính
            GameObject oldShelf = GameObject.Find("GreenhouseSeedShelf");
            if (oldShelf != null) UnityEngine.Object.DestroyImmediate(oldShelf);
            GameObject shelfGo = new GameObject("GreenhouseSeedShelf");
            shelfGo.tag = "Interactable";
            shelfGo.layer = 11;
            shelfGo.transform.position = new Vector3(-12.5f, 12.5f, 0f);
            var sSr = shelfGo.AddComponent<SpriteRenderer>();
            sSr.sprite = shelfSpr; sSr.sortingLayerName = "WorldObjects"; sSr.sortingOrder = Mathf.RoundToInt(-12.5f * 100f);
            shelfGo.AddComponent<BoxCollider2D>().size = new Vector2(0.9f, 0.9f);
            GameObject shelfPrompt = CreateWorldPrompt(shelfGo.transform, "[E] Lấy Hạt Lúa Mạch", vnFont);
            var shelfComp = shelfGo.AddComponent<SeedShelfInteractable>();
            var sSO = new SerializedObject(shelfComp);
            sSO.FindProperty("promptObject").objectReferenceValue = shelfPrompt;
            sSO.FindProperty("interactRadius").floatValue = 1.9f;
            sSO.ApplyModifiedPropertiesWithoutUndo();

            // 2. Giếng Nước cạnh ao ngoài Nhà Kính
            GameObject oldWell = GameObject.Find("VillageWaterWell");
            if (oldWell != null) UnityEngine.Object.DestroyImmediate(oldWell);
            GameObject wellGo = new GameObject("VillageWaterWell");
            wellGo.tag = "Interactable";
            wellGo.layer = 11;
            wellGo.transform.position = new Vector3(-7.0f, 10.5f, 0f);
            var wSr = wellGo.AddComponent<SpriteRenderer>();
            wSr.sprite = wellSpr; wSr.sortingLayerName = "WorldObjects"; wSr.sortingOrder = Mathf.RoundToInt(-10.5f * 100f);
            wellGo.AddComponent<BoxCollider2D>().size = new Vector2(0.95f, 0.95f);
            GameObject wellPrompt = CreateWorldPrompt(wellGo.transform, "[E] Múc đầy Bình Tưới", vnFont);
            var wellComp = wellGo.AddComponent<WaterWellInteractable>();
            var wSO = new SerializedObject(wellComp);
            wSO.FindProperty("promptObject").objectReferenceValue = wellPrompt;
            wSO.FindProperty("interactRadius").floatValue = 2.0f;
            wSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildResourceNodes(Sprite branchSpr, Sprite debrisSpr)
        {
            GameObject oldGroup = GameObject.Find("ResourceNodesGroup");
            if (oldGroup != null) UnityEngine.Object.DestroyImmediate(oldGroup);
            GameObject group = new GameObject("ResourceNodesGroup");

            ItemData woodItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_branch_wood.asset");
            GameObject lootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath);

            // 6 Nhánh Cây Khô ngoài làng (tự mọc lại mỗi sáng)
            Vector2[] branchPositions =
            {
                new Vector2(-9.5f, 4.5f),
                new Vector2(-4.5f, 6.5f),
                new Vector2(4.5f, 6.5f),
                new Vector2(6.5f, -0.5f),
                new Vector2(-6.5f, -2.5f),
                new Vector2(12.5f, 4.5f)
            };

            for (int i = 0; i < branchPositions.Length; i++)
            {
                CreateResourceNode(group.transform, "DryBranch_" + (i + 1), branchPositions[i], branchSpr,
                    ResourceNodeType.DryBranch, "item_hatchet", 2, woodItem, 2, lootPrefab, true);
            }

            // 3 Cụm Cỏ Dại / Đá Vụn trong Nhà Kính (dọn cho Mở Đầu Step 7)
            Vector2[] debrisPositions =
            {
                new Vector2(-18.5f, 9.5f),
                new Vector2(-18.5f, 7.5f),
                new Vector2(-12.5f, 7.5f)
            };

            for (int i = 0; i < debrisPositions.Length; i++)
            {
                CreateResourceNode(group.transform, "GreenhouseDebris_" + (i + 1), debrisPositions[i], debrisSpr,
                    ResourceNodeType.GreenhouseDebris, "item_hoe", 1, woodItem, 1, lootPrefab, false);
            }
        }

        private static void CreateResourceNode(
            Transform parent, string name, Vector2 pos, Sprite spr,
            ResourceNodeType type, string toolId, int hits, ItemData dropItem, int dropAmt, GameObject lootPrefab, bool regrow)
        {
            GameObject go = new GameObject(name);
            go.tag = "ResourceNode";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.85f, 0.75f);

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(go.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);

            var comp = go.AddComponent<ResourceNodeController>();
            var so = new SerializedObject(comp);
            so.FindProperty("nodeType").enumValueIndex = (int)type;
            so.FindProperty("requiredToolId").stringValue = toolId;
            so.FindProperty("maxHits").intValue = hits;
            so.FindProperty("dropItem").objectReferenceValue = dropItem;
            so.FindProperty("dropAmount").intValue = dropAmt;
            so.FindProperty("lootDropPrefab").objectReferenceValue = lootPrefab;
            so.FindProperty("regrowEachDay").boolValue = regrow;
            so.FindProperty("visualRenderer").objectReferenceValue = sr;
            so.FindProperty("nodeCollider").objectReferenceValue = col;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreateWorldPrompt(Transform parent, string label, TMP_FontAsset font)
        {
            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(parent, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 50;

            promptGo.GetComponent<RectTransform>().sizeDelta = new Vector2(260f, 44f);
            promptGo.AddComponent<Image>().color = new Color(0.08f, 0.12f, 0.18f, 0.88f);

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(promptGo.transform, false);
            var tRect = txtGo.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero; tRect.anchorMax = Vector2.one;
            tRect.offsetMin = Vector2.zero; tRect.offsetMax = Vector2.zero;
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label; tmp.fontSize = 21; tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.94f, 0.65f, 1f);
            if (font != null) tmp.font = font;
            return promptGo;
        }

        // Procedural 16x16 Pixel Art Sprites for Step 6
        private static void DrawUntilledSoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.46f, 0.34f, 0.22f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 4, 2, 2, new Color(0.38f, 0.28f, 0.18f));
            PlaceholderGeneratorUtility.FillRect(t, 10, 10, 2, 2, new Color(0.52f, 0.40f, 0.26f));
        }

        private static void DrawTilledDrySoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.40f, 0.26f, 0.14f));
            Color furrow = new Color(0.30f, 0.19f, 0.10f);
            PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 7, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 11, 12, 2, furrow);
        }

        private static void DrawTilledWetSoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.24f, 0.16f, 0.10f));
            Color furrow = new Color(0.16f, 0.10f, 0.06f);
            PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 7, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 11, 12, 2, furrow);
        }

        private static void DrawBarleyStage0(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 5, 4, 2, 2, new Color(0.76f, 0.86f, 0.45f));
            PlaceholderGeneratorUtility.FillRect(t, 9, 6, 2, 2, new Color(0.76f, 0.86f, 0.45f));
        }

        private static void DrawBarleyStage1(Texture2D t)
        {
            Color g = new Color(0.34f, 0.78f, 0.36f);
            PlaceholderGeneratorUtility.FillRect(t, 7, 2, 2, 6, g);
            PlaceholderGeneratorUtility.FillRect(t, 5, 6, 6, 2, g);
        }

        private static void DrawBarleyStage2(Texture2D t)
        {
            Color g = new Color(0.30f, 0.76f, 0.40f);
            Color tip = new Color(0.68f, 0.88f, 0.44f);
            PlaceholderGeneratorUtility.FillRect(t, 5, 2, 2, 9, g);
            PlaceholderGeneratorUtility.FillRect(t, 9, 2, 2, 10, g);
            PlaceholderGeneratorUtility.FillRect(t, 4, 10, 8, 3, tip);
        }

        private static void DrawBarleyStage3(Texture2D t)
        {
            Color stem = new Color(0.78f, 0.64f, 0.24f);
            Color gold = new Color(0.98f, 0.86f, 0.34f);
            Color dew = new Color(0.68f, 0.96f, 1.0f);
            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 2, 10, stem);
            PlaceholderGeneratorUtility.FillRect(t, 7, 2, 2, 11, stem);
            PlaceholderGeneratorUtility.FillRect(t, 10, 2, 2, 10, stem);
            PlaceholderGeneratorUtility.FillRect(t, 3, 9, 10, 5, gold);
            PlaceholderGeneratorUtility.FillRect(t, 5, 13, 2, 2, dew);
            PlaceholderGeneratorUtility.FillRect(t, 10, 12, 2, 2, dew);
        }

        private static void DrawWellSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 2, 12, 12, new Color(0.54f, 0.56f, 0.60f));
            PlaceholderGeneratorUtility.FillRect(t, 4, 4, 8, 8, new Color(0.20f, 0.56f, 0.86f));
            PlaceholderGeneratorUtility.FillRect(t, 5, 8, 4, 2, new Color(0.66f, 0.92f, 1f));
        }

        private static void DrawShelfSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 2, 14, 12, new Color(0.52f, 0.34f, 0.18f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 8, 4, 4, new Color(0.86f, 0.72f, 0.38f));
            PlaceholderGeneratorUtility.FillRect(t, 9, 8, 4, 4, new Color(0.52f, 0.84f, 0.44f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 3, new Color(0.72f, 0.56f, 0.32f));
        }

        private static void DrawBranchSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 6, 12, 4, new Color(0.46f, 0.28f, 0.14f));
            PlaceholderGeneratorUtility.FillRect(t, 6, 9, 3, 4, new Color(0.56f, 0.36f, 0.20f));
            PlaceholderGeneratorUtility.FillRect(t, 10, 4, 3, 3, new Color(0.38f, 0.22f, 0.10f));
        }

        private static void DrawDebrisSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 8, new Color(0.42f, 0.46f, 0.44f));
            PlaceholderGeneratorUtility.FillRect(t, 5, 8, 6, 5, new Color(0.28f, 0.48f, 0.26f));
            PlaceholderGeneratorUtility.FillRect(t, 4, 5, 4, 4, new Color(0.54f, 0.56f, 0.58f));
        }
    }
}
''',
}

def deploy():
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_root = PROJECT_ROOT / "_deploy_backup" / f"Step_6_{timestamp}"
    backed_up_count = 0

    print("=== MOONPETAL VALLEY — DEPLOY STEP 6 ===")
    for rel_path, content in FILES.items():
        target_path = PROJECT_ROOT / rel_path
        target_path.parent.mkdir(parents=True, exist_ok=True)

        status = "TẠO MỚI"
        if target_path.exists():
            backup_target = backup_root / rel_path
            backup_target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(target_path, backup_target)
            backed_up_count += 1
            status = "GHI ĐÈ "

        normalized = content.replace("\r\n", "\n")
        with open(target_path, "w", encoding="utf-8", newline="\n") as f:
            f.write(normalized)

        size_bytes = target_path.stat().st_size
        print(f"  [{status}] {rel_path} ({size_bytes} bytes)")

        if rel_path.endswith(".cs"):
            open_braces = normalized.count("{")
            close_braces = normalized.count("}")
            if open_braces != close_braces:
                print(f"  [CẢNH BÁO] Lệch dấu ngoặc trong {rel_path}: '{{'={open_braces}, '}}'={close_braces}")

    if backed_up_count > 0:
        print(f"[BACKUP] Đã sao lưu {backed_up_count} file cũ tại: {backup_root}")

    print("\n--- HƯỚNG DẪN TIẾP THEO ---")
    print("1. Quay lại Unity Editor (tắt Play Mode) và chờ compile xong (0 lỗi, 0 warning).")
    print("2. Chạy menu: Moonpetal/Step 6/1. Build Farming, Crops & Resources.")
    print("3. Chạy menu: Moonpetal/Step 6/Validate rồi bấm Play kiểm thử trồng Lúa Mạch & chặt gỗ!")

if __name__ == "__main__":
    deploy()