using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public struct SpawnPointDefinition
    {
        public string id;
        public string displayName;
        public Vector2 position;
    }

    [CreateAssetMenu(fileName = "MapLayoutConfig", menuName = "Moonpetal/Config/MapLayoutConfigSO")]
    public class MapLayoutConfigSO : ScriptableObject
    {
        [Header("Gốc tọa độ lưới bản đồ")]
        public int originX = -22;
        public int originY = -16;

        [Header("Bản đồ dạng text (Chú giải: . Cỏ, P Đường, G Sàn Nhà Kính, # Tường, F Hàng rào, T Cây, W Nước, B Bồn hoa)")]
        [TextArea(12, 36)]
        public string asciiMap = "";

        [Header("Danh sách điểm dịch chuyển / Spawn")]
        public List<SpawnPointDefinition> spawnPoints = new List<SpawnPointDefinition>();

        public Vector2 cameraMinBounds = new Vector2(-14f, -11f);
        public Vector2 cameraMaxBounds = new Vector2(14f, 11f);
    }
}
