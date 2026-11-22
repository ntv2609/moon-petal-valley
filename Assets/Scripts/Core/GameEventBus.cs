using System;

namespace MoonpetalValley.Core
{
    public enum GameState
    {
        Booting,
        Gameplay,
        Dialogue,
        InventoryUI,
        BundleUI,
        DiaryUI,
        PauseMenu,
        DebugMenu,
        Cutscene,
        DayTransition
    }

    public static class GameEventBus
    {
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<string, bool> OnStoryFlagChanged;
        public static event Action OnSaveLoaded;
        public static event Action<string> OnToastRequested;
        public static event Action<int, float> OnTimeChanged;
        public static event Action<int> OnDayStarted;
        public static event Action<int, int> OnStaminaChanged;
        public static event Action<int, int> OnLightHPChanged;
        public static event Action<string, int> OnItemChanged;
        public static event Action<string> OnEnemyDefeated;
        public static event Action<string> OnQuestUpdated;

        public static void RaiseGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);
        public static void RaiseStoryFlagChanged(string flagId, bool value) => OnStoryFlagChanged?.Invoke(flagId, value);
        public static void RaiseSaveLoaded() => OnSaveLoaded?.Invoke();
        public static void RaiseToastRequested(string message) => OnToastRequested?.Invoke(message);
        public static void RaiseTimeChanged(int day, float hourFloat) => OnTimeChanged?.Invoke(day, hourFloat);
        public static void RaiseDayStarted(int day) => OnDayStarted?.Invoke(day);
        public static void RaiseStaminaChanged(int current, int max) => OnStaminaChanged?.Invoke(current, max);
        public static void RaiseLightHPChanged(int current, int max) => OnLightHPChanged?.Invoke(current, max);
        public static void RaiseItemChanged(string itemId, int newTotal) => OnItemChanged?.Invoke(itemId, newTotal);
        public static void RaiseEnemyDefeated(string enemyId) => OnEnemyDefeated?.Invoke(enemyId);
        public static void RaiseQuestUpdated(string questId) => OnQuestUpdated?.Invoke(questId);
    }
}
