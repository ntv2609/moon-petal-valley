using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Inventory;
using MoonpetalValley.Player;
namespace MoonpetalValley.Chapter3
{
    public class NightMarketCraftingManager : MonoBehaviour {
        public static NightMarketCraftingManager Instance { get; private set; }
        public event Action<NightMarketRecipeDef> OnRecipeCrafted;
        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        private void OnDestroy() {
            if (Instance == this) Instance = null;
        }
        public bool CanCraft(NightMarketRecipeDef recipe, out string reason) {
            reason = "";
            if (recipe == null) {
                reason = "Công thức không hợp lệ.";
                return false;
            }
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) {
                reason = "Chưa sẵn sàng túi đồ.";
                return false;
            }
            StoryFlagManager flags = GameRoot.Instance.Flags;
            if (!string.IsNullOrEmpty(recipe.requiredStoryFlag) && (flags == null || !flags.HasFlag(recipe.requiredStoryFlag))) {
                reason = "Cần mở khóa công thức qua ký ức Lò Bánh Bà Hana.";
                return false;
            }
            InventoryManager inv = GameRoot.Instance.Inventory;
            for (int i = 0; i < recipe.ingredients.Count; i++) {
                RecipeIngredient ing = recipe.ingredients[i];
                if (ing == null || string.IsNullOrEmpty(ing.itemId)) continue;
                int have = inv.GetItemCount(ing.itemId);
                if (have < ing.amount) {
                    reason = "Thiếu " + ing.displayName + " (" + have + "/" + ing.amount + ").";
                    return false;
                }
            }
            PlayerStatsController stats = GameRoot.Instance.Stats;
            if (stats != null && recipe.staminaCost > 0 && stats.CurrentStamina < recipe.staminaCost) {
                reason = "Không đủ Thể lực (cần " + recipe.staminaCost + ").";
                return false;
            }
            return true;
        }
        public bool TryCraft(string recipeId) {
            Chapter3StateManager st = Chapter3StateManager.Instance;
            NightMarketConfigSO cfg = st != null ? st.Config : null;
            if (cfg == null) return false;
            NightMarketRecipeDef recipe = cfg.GetRecipeById(recipeId);
            if (!CanCraft(recipe, out string reason)) {
                if (!string.IsNullOrEmpty(reason))
                    GameEventBus.RaiseToastRequested(reason);
                return false;
            }
            InventoryManager inv = GameRoot.Instance.Inventory;
            for (int i = 0; i < recipe.ingredients.Count; i++) {
                RecipeIngredient ing = recipe.ingredients[i];
                if (ing != null && !string.IsNullOrEmpty(ing.itemId) && ing.amount > 0)
                    inv.RemoveItem(ing.itemId, ing.amount);
            }
            if (recipe.staminaCost > 0)
                GameRoot.Instance.Stats?.ConsumeStamina(recipe.staminaCost);
            inv.AddItemById(recipe.outputItemId, 1, false);
            st.RecordCraftedRecipe(recipe.recipeId);
            GameEventBus.RaiseToastRequested("Đã làm xong: " + recipe.displayName + "!");
            OnRecipeCrafted?.Invoke(recipe);
            return true;
        }
    }
}
