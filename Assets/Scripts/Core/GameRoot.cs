using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;

namespace MoonpetalValley.Core
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }

        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private StoryFlagManager storyFlagManager;
        [SerializeField] private VNLocalization localization;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private FloatingToastUI toastUI;
        [SerializeField] private RuntimeDebugOverlay debugOverlay;
        [SerializeField] private TimeOfDayManager timeManager;
        [SerializeField] private PlayerStatsController statsController;
        [SerializeField] private SleepAndKnockoutController sleepController;
        [SerializeField] private InventoryManager inventoryManager;

        private readonly List<ISaveable> _saveables = new List<ISaveable>();

        public GameStateManager State => gameStateManager;
        public StoryFlagManager Flags => storyFlagManager;
        public VNLocalization Loc => localization;
        public SceneLoader Scenes => sceneLoader;
        public FloatingToastUI Toast => toastUI;
        public RuntimeDebugOverlay DebugOverlay => debugOverlay;
        public TimeOfDayManager Time => timeManager;
        public PlayerStatsController Stats => statsController;
        public SleepAndKnockoutController Sleep => sleepController;
        public InventoryManager Inventory => inventoryManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (localization != null)
            {
                localization.Initialize();
            }

            if (storyFlagManager != null) RegisterSaveable(storyFlagManager);
            if (timeManager != null) RegisterSaveable(timeManager);
            if (statsController != null) RegisterSaveable(statsController);
            if (inventoryManager != null) RegisterSaveable(inventoryManager);

            Debug.Log("[MV] GameRoot khởi tạo thành công.");
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (saveable == null) return;
            if (!_saveables.Contains(saveable))
            {
                _saveables.Add(saveable);
            }
        }

        public IReadOnlyList<ISaveable> GetSaveables()
        {
            return _saveables;
        }
    }
}
