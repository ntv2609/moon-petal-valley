using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Farming {
    public enum FarmTileState {
        Untilled,
        TilledDry,
        Planted
    }

    public class FarmTileController : MonoBehaviour {
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
        public bool IsLuminous { get; private set; }

        public bool IsReadyToHarvest => State == FarmTileState.Planted && PlantedCrop != null && GrowthStage >= PlantedCrop.growDays;

        private void Start() { RefreshVisuals(); }

        private void Update() {
            if (IsLuminous && cropRenderer != null && cropRenderer.enabled) {
                float pulse = 0.86f + 0.14f * Mathf.Sin(Time.time * 5.2f + transform.position.x);
                cropRenderer.color = new Color(1f, 0.96f * pulse + 0.04f, 0.72f + 0.25f * pulse, 1f);
            }
        }

        public bool TryTill() {
            if (State != FarmTileState.Untilled) return false;
            State = FarmTileState.TilledDry;
            IsWatered = false;
            IsLuminous = false;
            RefreshVisuals();
            return true;
        }

        public bool TryPlant(CropData crop) {
            if (crop == null || State != FarmTileState.TilledDry) return false;
            State = FarmTileState.Planted;
            PlantedCrop = crop;
            GrowthStage = 0;
            IsLuminous = false;
            RefreshVisuals();
            return true;
        }

        public bool TryWater() {
            if (State == FarmTileState.Untilled || IsWatered) return false;
            IsWatered = true;
            if (State == FarmTileState.Planted && !IsLuminous && UnityEngine.Random.value < 0.18f) {
                IsLuminous = true;
                if (JuiceEffectsManager.Instance != null)
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.6f, "Phát Sáng!", new Color(1f, 0.94f, 0.45f));
            }
            RefreshVisuals();
            return true;
        }

        public bool TryApplyStarFertilizer() {
            if (State != FarmTileState.Planted || PlantedCrop == null) return false;
            if (IsReadyToHarvest && IsLuminous) return false;
            GrowthStage = PlantedCrop.growDays;
            IsWatered = true;
            IsLuminous = true;
            RefreshVisuals();
            if (JuiceEffectsManager.Instance != null)
                JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.75f, "Chín Tức Thì x2!", new Color(1f, 0.92f, 0.42f));
            return true;
        }

        public bool TryHarvest(out ItemData harvestedItem, out int yieldAmount) {
            harvestedItem = null;
            yieldAmount = 0;
            if (!IsReadyToHarvest) return false;

            harvestedItem = PlantedCrop.harvestedItem;
            int baseYield = Mathf.Max(1, PlantedCrop.harvestYield);
            yieldAmount = IsLuminous ? baseYield * 2 : baseYield;

            if (IsLuminous && JuiceEffectsManager.Instance != null) { JuiceEffectsManager.Instance.SpawnFloatingNumber(transform.position + Vector3.up * 0.8f, "Phát Sáng x" + yieldAmount + "!", new Color(1f, 0.94f, 0.45f)); }

            State = FarmTileState.TilledDry;
            PlantedCrop = null;
            GrowthStage = 0;
            IsWatered = false;
            IsLuminous = false;
            RefreshVisuals();
            return true;
        }

        public void OnNewDay() {
            if (State == FarmTileState.Planted && PlantedCrop != null) {
                if (IsWatered && GrowthStage < PlantedCrop.growDays) {
                    GrowthStage++;
                    if (!IsLuminous && UnityEngine.Random.value < 0.22f) IsLuminous = true;
                }
            }
            IsWatered = false;
            RefreshVisuals();
        }

        public void ForceWaterDebug() {
            if (State != FarmTileState.Untilled) {
                IsWatered = true;
                RefreshVisuals();
            }
        }

        public void RestoreState(FarmTileState state, CropData crop, int stage, bool watered, bool luminous = false) {
            State = state;
            PlantedCrop = crop;
            GrowthStage = stage;
            IsWatered = watered;
            IsLuminous = luminous && state == FarmTileState.Planted;
            RefreshVisuals();
        }

        public void RefreshVisuals() {
            if (soilRenderer != null) {
                if (State == FarmTileState.Untilled) soilRenderer.sprite = untilledSprite;
                else soilRenderer.sprite = IsWatered ? tilledWetSprite : tilledDrySprite;
            }

            if (cropRenderer != null) {
                if (State == FarmTileState.Planted && PlantedCrop != null && PlantedCrop.stageSprites != null && PlantedCrop.stageSprites.Length > 0) {
                    int maxIdx = PlantedCrop.stageSprites.Length - 1;
                    int idx = IsReadyToHarvest
                        ? maxIdx
                        : Mathf.Clamp(GrowthStage, 0, Mathf.Max(0, maxIdx - 1));
                    cropRenderer.sprite = PlantedCrop.stageSprites[idx];
                    cropRenderer.color = IsLuminous ? new Color(1f, 0.96f, 0.78f, 1f) : Color.white;
                    cropRenderer.enabled = true;
                }
                else {
                    cropRenderer.sprite = null;
                    cropRenderer.color = Color.white;
                    cropRenderer.enabled = false;
                }
            }
        }
    }
}
