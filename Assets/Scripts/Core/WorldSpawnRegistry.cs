using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.Core
{
    public class WorldSpawnRegistry : MonoBehaviour
    {
        public static WorldSpawnRegistry Instance { get; private set; }

        [SerializeField] private List<SpawnPointDefinition> spawnPoints = new List<SpawnPointDefinition>();

        private void Awake()
        {
            Instance = this;
        }

        public bool TryGetSpawnPosition(string spawnId, out Vector2 position)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnPoints[i].id == spawnId)
                {
                    position = spawnPoints[i].position;
                    return true;
                }
            }
            position = Vector2.zero;
            return false;
        }

        public bool TeleportPlayerTo(string spawnId)
        {
            if (!TryGetSpawnPosition(spawnId, out Vector2 targetPos))
            {
                Debug.LogWarning("[MV] Không tìm thấy SpawnPoint: " + spawnId);
                return false;
            }

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy PlayerController2D để Teleport.");
                return false;
            }

            player.TeleportTo(targetPos);
            GameEventBus.RaiseToastRequested("Dịch chuyển tới: " + GetDisplayName(spawnId));
            return true;
        }

        public string GetDisplayName(string spawnId)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnPoints[i].id == spawnId) return spawnPoints[i].displayName;
            }
            return spawnId;
        }
    }
}
