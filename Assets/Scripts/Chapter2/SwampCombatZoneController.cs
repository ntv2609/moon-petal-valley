using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;

namespace MoonpetalValley.Chapter2
{
    public class SwampCombatZoneController : MonoBehaviour
    {
        [SerializeField] private string zoneClearedFlag = "flag_ch2_swamp_cleared";
        [SerializeField] private ItemData westLanternRewardItem;
        [SerializeField] private List<GameObject> trackedEnemies = new List<GameObject>();

        private readonly HashSet<GameObject> _defeated = new HashSet<GameObject>();
        private bool _zoneCleared;

        private void OnEnable()
        {
            GameEventBus.OnSaveLoaded += SyncFromStoryFlags;
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnSaveLoaded -= SyncFromStoryFlags;
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
        }

        private void Start()
        {
            BindTrackedEnemies();
            SyncFromStoryFlags();
        }

        private void BindTrackedEnemies()
        {
            for (int i = 0; i < trackedEnemies.Count; i++)
            {
                GameObject go = trackedEnemies[i];
                if (go == null) continue;
                var crawler = go.GetComponent<ThornCrawlerController>();
                if (crawler != null) crawler.SetCombatZone(this);
                var bigSlime = go.GetComponent<BigGloomSlimeController>();
                if (bigSlime != null) bigSlime.SetCombatZone(this);
            }
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (flagId == zoneClearedFlag || flagId == Chapter2StateManager.FlagSwampUnlocked)
                SyncFromStoryFlags();
        }

        public void SyncFromStoryFlags()
        {
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            bool cleared = flags != null && flags.HasFlag(zoneClearedFlag);
            bool unlocked = Chapter2StateManager.Instance != null
                ? Chapter2StateManager.Instance.IsSwampUnlocked()
                : (flags != null && flags.HasFlag(Chapter2StateManager.FlagSwampUnlocked));

            _zoneCleared = cleared;

            if (cleared || !unlocked)
            {
                for (int i = 0; i < trackedEnemies.Count; i++)
                {
                    if (trackedEnemies[i] != null && trackedEnemies[i].activeSelf)
                        trackedEnemies[i].SetActive(false);
                }
            }
            else
            {
                _defeated.Clear();
                for (int i = 0; i < trackedEnemies.Count; i++)
                {
                    GameObject go = trackedEnemies[i];
                    if (go != null && !go.activeSelf) go.SetActive(true);
                }
                BindTrackedEnemies();
            }
        }

        public void NotifyEnemyDefeated(GameObject enemyGo)
        {
            if (_zoneCleared || enemyGo == null) return;
            _defeated.Add(enemyGo);
            int alive = 0;
            for (int i = 0; i < trackedEnemies.Count; i++)
            {
                GameObject go = trackedEnemies[i];
                if (go != null && go.activeInHierarchy && !_defeated.Contains(go))
                {
                    var dmg = go.GetComponent<IDamageable>();
                    if (dmg == null || !dmg.IsDefeated) alive++;
                }
            }
            if (alive == 0) CompleteSwampZone();
        }

        private void CompleteSwampZone()
        {
            _zoneCleared = true;
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
                GameRoot.Instance.Flags.SetFlag(zoneClearedFlag, true);
            if (westLanternRewardItem != null && GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                if (!GameRoot.Instance.Inventory.HasItem(westLanternRewardItem.itemID, 1))
                    GameRoot.Instance.Inventory.AddItem(westLanternRewardItem, 1, true);
            }
            GameEventBus.RaiseToastRequested("Đầm lầy Bờ Tây đã thanh tẩy! Bạn nhận được Chao Kính Đèn Bão - hãy soi Dư Âm ngay gần đó.");
        }
    }
}
