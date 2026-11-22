using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step21
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string CropDewBarleyPath = "Assets/Data/Crops/Crop_DewBarley.asset";
        private const string WoodItemPath = "Assets/Data/Items/Item_item_branch_wood.asset";
        private const string LootDropPrefabPath = "Assets/Prefabs/Enemies/LootDrop.prefab";

        [MenuItem("Moonpetal/Step 21/1. Restore Resource Nodes & Upgrade Miu Cat + Barley Crops")]
        public static void BuildStep21()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU STEP 21 (KHÔI PHỤC CỎ DẠI, GỖ KHÔ, MÈO MIU & LÚA MẠCH) ===");
            AssetDatabase.Refresh();
            ReimportStep21Sprites();
            AssetDatabase.Refresh();

            UpgradeBarleyCropAsset();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            RebuildResourceNodesCleanly();
            UpgradeMiuCatInScene();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 21! Hãy chạy 'Moonpetal/Step 21/2. Validate Full MVP Parity' để kiểm toán 100%. ===");
        }

        [MenuItem("Moonpetal/Step 21/2. Validate Full MVP Parity (All Gameplay Objects)")]
        public static void ValidateFullParity()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU KIỂM TOÁN TOÀN BỘ MVP PARITY (STEP 1 -> STEP 21) ===");

            if (UnityEngine.Object.FindAnyObjectByType<PlayerController2D>() == null) { Debug.LogError("[MV] THIẾU Player_Wen."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>() == null) { Debug.LogError("[MV] THIẾU Lumi_Companion."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<WorldSpawnRegistry>() == null) { Debug.LogError("[MV] THIẾU WorldSpawnRegistry."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<BedInteractable>() == null) { Debug.LogError("[MV] THIẾU GreenhouseBed."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<SeedShelfInteractable>() == null) { Debug.LogError("[MV] THIẾU GreenhouseSeedShelf."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<WaterWellInteractable>() == null) { Debug.LogError("[MV] THIẾU VillageWaterWell."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>() == null) { Debug.LogError("[MV] THIẾU CentralPlantingSpot."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<WaterTileAnimator>() == null) { Debug.LogError("[MV] THIẾU AnimatedPondWater."); errors++; }

            // Tilemap collision
            GameObject obsGo = GameObject.Find("ObstacleTilemap");
            var tmCol = obsGo != null ? obsGo.GetComponent<TilemapCollider2D>() : null;
            if (tmCol == null || tmCol.shapeCount < 100) { Debug.LogError("[MV] ObstacleTilemap thiếu collider hợp lệ."); errors++; }
            if (GameObject.Find("MapBoundaryColliders") == null) { Debug.LogError("[MV] Thiếu MapBoundaryColliders."); errors++; }

            // 12 Farm Tiles + Crop 4 stages
            var farmTiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            if (farmTiles.Length < 12) { Debug.LogError($"[MV] Thiếu FarmTile ({farmTiles.Length}/12)."); errors++; }
            var crop = AssetDatabase.LoadAssetAtPath<CropData>(CropDewBarleyPath);
            if (crop == null || crop.stageSprites == null || crop.stageSprites.Length < 4 || crop.stageSprites[3] == null)
            {
                Debug.LogError("[MV] Crop_DewBarley thiếu 4 sprite giai đoạn.");
                errors++;
            }

            // 9 Resource Nodes (3 GreenhouseDebris + 6 DryBranch) - ALL must be active & visible!
            var nodes = UnityEngine.Object.FindObjectsByType<ResourceNodeController>(FindObjectsInactive.Include);
            int debrisCount = 0;
            int branchCount = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                var n = nodes[i];
                if (!n.gameObject.activeInHierarchy) { Debug.LogError("[MV] ResourceNode đang bị tắt: " + n.name); errors++; }
                var sr = n.GetComponentInChildren<SpriteRenderer>();
                var col = n.GetComponent<Collider2D>();
                if (sr == null || !sr.enabled || sr.sprite == null) { Debug.LogError("[MV] ResourceNode thiếu Sprite hiển thị: " + n.name); errors++; }
                if (col == null || !col.enabled) { Debug.LogError("[MV] ResourceNode thiếu Collider đang bật: " + n.name); errors++; }
                if (n.NodeType == ResourceNodeType.GreenhouseDebris) debrisCount++;
                else branchCount++;
            }
            if (debrisCount < 3 || branchCount < 6)
            {
                Debug.LogError($"[MV] Thiếu ResourceNodes (Cỏ dại Nhà Kính={debrisCount}/3, Nhánh gỗ khô={branchCount}/6).");
                errors++;
            }

            // 4 NPCs, 5 StoryProps, 2 Encounters, 8 Slimes, MiuCat, 6 BlightFogs, 4 Lanterns
            int npcCount = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include).Length;
            int propCount = UnityEngine.Object.FindObjectsByType<StoryPropInteractable>(FindObjectsInactive.Include).Length;
            int encCount = UnityEngine.Object.FindObjectsByType<BlightEncounterZone>(FindObjectsInactive.Include).Length;
            int slimeCount = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include).Length;
            int fogCount = UnityEngine.Object.FindObjectsByType<BlightFogZone>(FindObjectsInactive.Include).Length;
            int lanternCount = UnityEngine.Object.FindObjectsByType<VillageLantern>(FindObjectsInactive.Include).Length;

            if (npcCount < 4) { Debug.LogError($"[MV] Thiếu NPC ({npcCount}/4)."); errors++; }
            if (propCount < 5) { Debug.LogError($"[MV] Thiếu StoryProps ({propCount}/5)."); errors++; }
            if (encCount < 2) { Debug.LogError($"[MV] Thiếu BlightEncounterZone ({encCount}/2)."); errors++; }
            if (slimeCount < 8) { Debug.LogError($"[MV] Thiếu GloomSlime ({slimeCount}/8)."); errors++; }
            if (fogCount < 6) { Debug.LogError($"[MV] Thiếu BlightFogZone ({fogCount}/6)."); errors++; }
            if (lanternCount < 4) { Debug.LogError($"[MV] Thiếu VillageLantern ({lanternCount}/4)."); errors++; }
            if (GameObject.Find("MiuCat_Toby") == null) { Debug.LogError("[MV] Thiếu MiuCat_Toby ở Rìa Rừng Phía Đông."); errors++; }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + allGos[i].name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log($"[MV] VALIDATE FULL MVP PARITY: PASS 100% (0 lỗi | Cỏ dại={debrisCount}/3 | Gỗ khô={branchCount}/6 | Ruộng={farmTiles.Length}/12 | NPC={npcCount}/4 | StoryProps={propCount}/5 | Slime={slimeCount}/8 | MiuCat=OK | Fog={fogCount}/6 | Đèn={lanternCount}/4).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE FULL MVP PARITY: FAIL ({errors} lỗi).");
            }
        }

        private static void ImportPixelSprite(string path, float ppu)
        {
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) return;
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.filterMode = FilterMode.Point;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.alphaIsTransparency = true;
            imp.spritePixelsPerUnit = ppu;
            var st = new TextureImporterSettings();
            imp.ReadTextureSettings(st);
            st.spriteMeshType = SpriteMeshType.FullRect;
            st.spriteAlignment = (int)SpriteAlignment.Center;
            st.spritePivot = new Vector2(0.5f, 0.5f);
            imp.SetTextureSettings(st);
            imp.SaveAndReimport();
        }

        private static void ReimportStep21Sprites()
        {
            string[] paths =
            {
                "Assets/Art/Sprites/Environment/Props/prop_greenhouse_debris.png",
                "Assets/Art/Sprites/Environment/Props/prop_dry_branch.png",
                "Assets/Art/Sprites/Characters/spr_miu_cat.png",
                "Assets/Art/Sprites/Crops/spr_dew_barley_0.png",
                "Assets/Art/Sprites/Crops/spr_dew_barley_1.png",
                "Assets/Art/Sprites/Crops/spr_dew_barley_2.png",
                "Assets/Art/Sprites/Crops/spr_dew_barley_3.png",
            };
            foreach (string p in paths) ImportPixelSprite(p, 16f);
        }

        private static void UpgradeBarleyCropAsset()
        {
            var crop = AssetDatabase.LoadAssetAtPath<CropData>(CropDewBarleyPath);
            if (crop == null) return;
            Sprite[] stages =
            {
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Crops/spr_dew_barley_0.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Crops/spr_dew_barley_1.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Crops/spr_dew_barley_2.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Crops/spr_dew_barley_3.png"),
            };
            if (stages[0] != null)
            {
                crop.stageSprites = stages;
                EditorUtility.SetDirty(crop);
                Debug.Log("[MV] Đã nâng cấp 4 giai đoạn sprite Lúa Mạch Sương Mai.");
            }
        }

        private static void RebuildResourceNodesCleanly()
        {
            GameObject oldGroup = GameObject.Find("ResourceNodesGroup");
            if (oldGroup != null) UnityEngine.Object.DestroyImmediate(oldGroup);
            GameObject group = new GameObject("ResourceNodesGroup");

            Sprite debrisSpr = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Environment/Props/prop_greenhouse_debris.png");
            Sprite branchSpr = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Environment/Props/prop_dry_branch.png");
            ItemData woodItem = AssetDatabase.LoadAssetAtPath<ItemData>(WoodItemPath);
            GameObject lootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath);

            // 3 Greenhouse Debris piles in 100% clear open floor spots inside the Greenhouse (zero overlap with 12 FarmTiles!)
            Vector2[] debrisPositions =
            {
                new Vector2(-19.2f, 9.6f),
                new Vector2(-19.2f, 7.2f),
                new Vector2(-11.8f, 8.4f),
            };
            for (int i = 0; i < debrisPositions.Length; i++)
            {
                CreateResourceNode(group.transform, "GreenhouseDebris_" + (i + 1), debrisPositions[i], debrisSpr,
                    ResourceNodeType.GreenhouseDebris, "item_hoe", 1, woodItem, 1, lootPrefab, false);
            }

            // 6 Dry Branches in 100% clear walkable meadows (zero overlap with Hana's Bakery, Bach's Shed or trees!)
            Vector2[] branchPositions =
            {
                new Vector2(-11.2f, 2.8f),
                new Vector2(-3.2f, 2.8f),
                new Vector2(4.8f, 2.8f),
                new Vector2(7.2f, -0.6f),
                new Vector2(-7.2f, -2.6f),
                new Vector2(13.2f, 4.6f),
            };
            for (int i = 0; i < branchPositions.Length; i++)
            {
                CreateResourceNode(group.transform, "DryBranch_" + (i + 1), branchPositions[i], branchSpr,
                    ResourceNodeType.DryBranch, "item_hatchet", 2, woodItem, 2, lootPrefab, true);
            }

            Debug.Log("[MV] Đã khôi phục & nâng cấp art cho 3 cụm Cỏ dại đá vụn Nhà Kính + 6 Nhánh gỗ khô ngoài làng.");
        }

        private static void CreateResourceNode(
            Transform parent, string name, Vector2 pos, Sprite spr,
            ResourceNodeType type, string toolId, int hits, ItemData dropItem, int dropAmt, GameObject lootPrefab, bool regrow)
        {
            GameObject go = new GameObject(name);
            go.tag = "ResourceNode";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.85f, 0.65f);
            col.isTrigger = false;
            col.enabled = true;

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(go.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);

            var comp = go.AddComponent<ResourceNodeController>();
            var so = new SerializedObject(comp);
            so.FindProperty("nodeType").enumValueIndex = (int)type;
            so.FindProperty("requiredToolId").stringValue = toolId;
            so.FindProperty("maxHits").intValue = hits;
            so.FindProperty("dropItem").objectReferenceValue = dropItem;
            so.FindProperty("dropAmount").intValue = dropAmt;
            so.FindProperty("lootDropPrefab").objectReferenceValue = lootPrefab;
            so.FindProperty("regrowEachDay").boolValue = regrow;
            so.FindProperty("visualRenderer").objectReferenceValue = sr;
            so.FindProperty("nodeCollider").objectReferenceValue = col;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void UpgradeMiuCatInScene()
        {
            GameObject cat = GameObject.Find("MiuCat_Toby");
            Sprite catSpr = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Characters/spr_miu_cat.png");
            if (cat != null && catSpr != null)
            {
                var sr = cat.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = catSpr;
                    sr.color = Color.white;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-cat.transform.position.y * 100f);
                    EditorUtility.SetDirty(sr);
                }
                Debug.Log("[MV] Đã nâng cấp art cho chú mèo Miu (MiuCat_Toby).");
            }
        }
    }
}
