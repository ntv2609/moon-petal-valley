using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Farming {
    public class FarmGridManager : MonoBehaviour, ISaveable {
        public static FarmGridManager Instance { get; private set; }

        [SerializeField] private List<FarmTileController> tiles = new List<FarmTileController>();
        [SerializeField] private CropData defaultCropData;
        [SerializeField] private List<CropData> cropCatalog = new List<CropData>();

        public string SaveKey {
            get {
                string sn = SceneManager.GetActiveScene().name;
                return (string.IsNullOrEmpty(sn) || sn == "MoonpetalValley_MVP") ? "farm_tiles" : ("farm_tiles_" + sn);
            }
        }

        [Serializable]
        private struct SavedTile {
            public string id;
            public int state;
            public string cropId;
            public int stage;
            public bool watered;
            public bool luminous;
        }

        [Serializable]
        private class FarmSaveWrapper { public List<SavedTile> savedTiles = new List<SavedTile>(); }

        private void Awake() { Instance = this; }

        private void OnEnable() { GameEventBus.OnDayStarted += HandleDayStarted; }

        private void OnDisable() { GameEventBus.OnDayStarted -= HandleDayStarted; }

        private void Start() {
            if (GameRoot.Instance != null) { GameRoot.Instance.RegisterSaveable(this); }
        }

        private void HandleDayStarted(int newDay) {
            int readyCount = 0;
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i] != null) {
                    tiles[i].OnNewDay();
                    if (tiles[i].IsReadyToHarvest) readyCount++;
                }
            }
            if (readyCount > 0) { GameEventBus.RaiseToastRequested("Trong vườn có " + readyCount + " cây đã chín! Bấm [E] để thu hoạch."); }
        }

        public CropData FindCropById(string cropId) {
            if (string.IsNullOrEmpty(cropId)) return null;
            if (cropCatalog != null) {
                for (int i = 0; i < cropCatalog.Count; i++) {
                    if (cropCatalog[i] != null && cropCatalog[i].cropID == cropId)
                        return cropCatalog[i];
                }
            }
            if (defaultCropData != null && defaultCropData.cropID == cropId) return defaultCropData;
            return defaultCropData;
        }

        public FarmTileController FindClosestTile(Vector2 worldPos, float maxDistance = 1.15f) {
            FarmTileController best = null;
            float bestDist = maxDistance;
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i] == null) continue;
                float d = Vector2.Distance(worldPos, tiles[i].transform.position);
                if (d < bestDist) {
                    bestDist = d;
                    best = tiles[i];
                }
            }
            return best;
        }

        public List<FarmTileController> FindTilesInRadius(Vector2 center, float radius) {
            var result = new List<FarmTileController>();
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i] == null) continue;
                if (Vector2.Distance(center, tiles[i].transform.position) <= radius)
                    result.Add(tiles[i]);
            }
            return result;
        }

        public int CountPlantedCrops(string cropId) {
            int count = 0;
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i] != null && tiles[i].State == FarmTileState.Planted &&
                    tiles[i].PlantedCrop != null && tiles[i].PlantedCrop.cropID == cropId) { count++; }
            }
            return count;
        }

        public void WaterAllTilesDebug() {
            int watered = 0;
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i] != null && tiles[i].State != FarmTileState.Untilled) {
                    tiles[i].ForceWaterDebug();
                    watered++;
                }
            }
            GameEventBus.RaiseToastRequested("Đã tưới nước toàn bộ " + watered + " ô đất đã cuốc!");
        }

        public string CaptureStateJson() {
            var wrapper = new FarmSaveWrapper();
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i] == null) continue;
                wrapper.savedTiles.Add(new SavedTile {
                    id = tiles[i].TileId,
                    state = (int)tiles[i].State,
                    cropId = tiles[i].PlantedCrop != null ? tiles[i].PlantedCrop.cropID : "",
                    stage = tiles[i].GrowthStage,
                    watered = tiles[i].IsWatered,
                    luminous = tiles[i].IsLuminous
                });
            }
            return JsonUtility.ToJson(wrapper);
        }

        public void RestoreStateJson(string json) {
            if (string.IsNullOrEmpty(json)) return;
            var wrapper = JsonUtility.FromJson<FarmSaveWrapper>(json);
            if (wrapper == null || wrapper.savedTiles == null) return;

            for (int i = 0; i < wrapper.savedTiles.Count; i++) {
                var st = wrapper.savedTiles[i];
                for (int j = 0; j < tiles.Count; j++) {
                    if (tiles[j] != null && tiles[j].TileId == st.id) {
                        CropData crop = !string.IsNullOrEmpty(st.cropId) ? FindCropById(st.cropId) : null;
                        tiles[j].RestoreState((FarmTileState)st.state, crop, st.stage, st.watered, st.luminous);
                        break;
                    }
                }
            }
        }

        private void OnDestroy() { if (GameRoot.Instance != null) GameRoot.Instance.UnregisterSaveable(this); }
    }
}
