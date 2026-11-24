using System;
using System.Collections.Generic;
using UnityEngine;
namespace MoonpetalValley.Chapter3
{
    [Serializable]
    public class RecipeIngredient {
        public string itemId;
        public string displayName;
        public int amount = 1;
    }
    [Serializable]
    public class NightMarketRecipeDef {
        public string recipeId;
        public string outputItemId;
        public string displayName;
        public string flavorText;
        public int staminaCost = 2;
        public string requiredStoryFlag;
        public List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
    }
    [Serializable]
    public class KeepsakeDef {
        public string keepsakeId;
        public string npcId;
        public string npcDisplayName;
        public string displayName;
        public string storyNote;
        public Sprite icon;
    }
    [CreateAssetMenu(fileName = "NightMarketConfig", menuName = "Moonpetal/Chapter3/NightMarketConfigSO")]
    public class NightMarketConfigSO : ScriptableObject {
        [Header("Cong thuc chuan bi Cho Dem")]
        public List<NightMarketRecipeDef> recipes = new List<NightMarketRecipeDef>();
        [Header("7 Ky Vat Chuong 3 (So Ky Vat)")]
        public List<KeepsakeDef> keepsakes = new List<KeepsakeDef>();
        [Header("Cau hinh Phien Cho Dem")]
        public float serviceDurationSeconds = 100f;
        public float warmthBonusThreshold = 70f;
        public int targetPrepItems = 5;
        public NightMarketRecipeDef GetRecipeById(string recipeId) {
            if (string.IsNullOrEmpty(recipeId)) return null;
            for (int i = 0; i < recipes.Count; i++) {
                if (recipes[i] != null && recipes[i].recipeId == recipeId)
                    return recipes[i];
            }
            return null;
        }
        public KeepsakeDef GetKeepsakeById(string keepsakeId) {
            if (string.IsNullOrEmpty(keepsakeId)) return null;
            for (int i = 0; i < keepsakes.Count; i++) {
                if (keepsakes[i] != null && keepsakes[i].keepsakeId == keepsakeId)
                    return keepsakes[i];
            }
            return null;
        }
    }
}
