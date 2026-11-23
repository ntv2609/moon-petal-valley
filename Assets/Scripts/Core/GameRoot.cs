using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;
using MoonpetalValley.Narrative;
using MoonpetalValley.Community;
using MoonpetalValley.Audio;

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
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private DiaryManager diaryManager;
        [SerializeField] private QuestManager questManager;
        [SerializeField] private BundleManager bundleManager;
        [SerializeField] private SaveLoadManager saveLoadManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private RelationshipManager relationshipManager;

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
        public DialogueManager Dialogue => dialogueManager;
        public DiaryManager Diary => diaryManager;
        public QuestManager Quests => questManager;
        public BundleManager Bundle => bundleManager;
        public SaveLoadManager SaveLoad => saveLoadManager;
        public AudioManager Audio => audioManager;
        public RelationshipManager Relationships => relationshipManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (localization != null) localization.Initialize();
            if (relationshipManager == null)
            {
                relationshipManager = GetComponent<RelationshipManager>();
                if (relationshipManager == null) relationshipManager = gameObject.AddComponent<RelationshipManager>();
            }

            if (storyFlagManager != null) RegisterSaveable(storyFlagManager);
            if (timeManager != null) RegisterSaveable(timeManager);
            if (statsController != null) RegisterSaveable(statsController);
            if (inventoryManager != null) RegisterSaveable(inventoryManager);
            if (diaryManager != null) RegisterSaveable(diaryManager);
            if (bundleManager != null) RegisterSaveable(bundleManager);
            if (relationshipManager != null) RegisterSaveable(relationshipManager);

            Debug.Log("[MV] GameRoot khởi tạo thành công.");
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (saveable == null) return;
            if (!_saveables.Contains(saveable)) _saveables.Add(saveable);
        }

        public void UnregisterSaveable(ISaveable saveable)
        {
            if (saveable == null) return;
            _saveables.Remove(saveable);
        }

        public void PruneDestroyedSaveables()
        {
            for (int i = _saveables.Count - 1; i >= 0; i--)
            {
                if (_saveables[i] == null || (_saveables[i] is UnityEngine.Object uo && uo == null))
                {
                    _saveables.RemoveAt(i);
                }
            }
        }

        public IReadOnlyList<ISaveable> GetSaveables()
        {
            PruneDestroyedSaveables();
            return _saveables;
        }

        public void ResetForNewGame()
        {
            PruneDestroyedSaveables();
            var resettables = GetComponentsInChildren<IResettable>(true);
            for (int i = 0; i < resettables.Length; i++)
            {
                resettables[i].ResetForNewGame();
            }
            Quests?.RecomputeObjective();
            Debug.Log("[MV] GameRoot đã reset sạch toàn bộ trạng thái cho lượt Chơi Mới.");
        }
    }
}
