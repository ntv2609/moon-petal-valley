using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

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

        private readonly List<ISaveable> _saveables = new List<ISaveable>();

        public GameStateManager State => gameStateManager;
        public StoryFlagManager Flags => storyFlagManager;
        public VNLocalization Loc => localization;
        public SceneLoader Scenes => sceneLoader;
        public FloatingToastUI Toast => toastUI;
        public RuntimeDebugOverlay DebugOverlay => debugOverlay;

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

            if (storyFlagManager != null)
            {
                RegisterSaveable(storyFlagManager);
            }

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
