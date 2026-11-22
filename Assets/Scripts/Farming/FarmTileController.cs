using System;
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
