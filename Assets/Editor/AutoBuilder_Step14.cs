using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;
using MoonpetalValley.Community;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step14
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/Upgraded";

        [MenuItem("Moonpetal/Step 14/1. Upgrade World Tilemap, Buildings & Decor")]
        public static void BuildStep14()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 14: NÂNG CẤP CẢNH QUAN, TILEMAP & NHÀ CỬA ===");
            AssetDatabase.Refresh();
            ForceReimportEnvironmentSprites();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            UpgradeWorldTilemap();
            UpgradeFarmSoilSprites();
            UpgradeInteractiveProps();
            UpgradeVillageDecorAndBuildings();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 14! Chạy 'Moonpetal/Step 14/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 14/Validate")]
        public static void ValidateStep14()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 14 ===");

            string[] requiredSprites =
            {
                "Tiles/tile_grass_0.png", "Tiles/tile_grass_3.png",
                "Tiles/tile_path_0.png", "Tiles/tile_path_2.png",
                "Tiles/tile_gh_floor_0.png", "Tiles/tile_wall.png",
                "Tiles/tile_fence.png", "Tiles/tile_tree.png", "Tiles/tile_water.png",
                "Soil/soil_untilled.png", "Soil/soil_dry.png", "Soil/soil_wet.png",
                "Buildings/house_bakery.png", "Buildings/house_shed.png",
                "Buildings/otis_notice_board.png", "Buildings/tree_hero_blossom.png",
                "Props/prop_well.png", "Props/prop_bed.png",
                "Props/planting_spot_empty.png", "Props/planting_spot_bloomed.png"
            };

            foreach (string rel in requiredSprites)
            {
                if (LoadEnvSprite(rel) == null)
                {
                    Debug.LogError("[MV] THIẾU sprite cảnh quan: " + EnvRoot + "/" + rel);
                    errors++;
                }
            }

            if (GameObject.Find("WorldMapGrid") == null) { Debug.LogError("[MV] Thiếu WorldMapGrid."); errors++; }
            if (GameObject.Find("VillageDecorGroup") == null) { Debug.LogError("[MV] Thiếu VillageDecorGroup."); errors++; }
            if (GameObject.Find("House_HanaBakery") == null) { Debug.LogError("[MV] Thiếu House_HanaBakery."); errors++; }
            if (GameObject.Find("House_BachShed") == null) { Debug.LogError("[MV] Thiếu House_BachShed."); errors++; }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 14: PASS (0 lỗi). Tilemap đa biến thể, Tiệm Bánh, Chòi Vườn, Cây Hoa Anh Đào & Đạo cụ đã nâng cấp hoàn chỉnh!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 14: FAIL (" + errors + " lỗi).");
            }
        }

        private static Sprite LoadEnvSprite(string relPath)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "/" + relPath);
        }

        private static void ForceReimportEnvironmentSprites()
        {
            if (!Directory.Exists(EnvRoot)) return;
            foreach (string file in Directory.GetFiles(EnvRoot, "*.png", SearchOption.AllDirectories))
            {
                string assetPath = file.Replace("\\", "/");
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 16;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.alphaIsTransparency = true;
                    importer.mipmapEnabled = false;

                    var settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    settings.spriteAlignment = (int)SpriteAlignment.Center;
                    settings.spriteGenerateFallbackPhysicsShape = false;
                    importer.SetTextureSettings(settings);
                    importer.SaveAndReimport();
                }
            }
        }

        private static void UpgradeWorldTilemap()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null)
            {
                Debug.LogError("[MV] Không tìm thấy MapLayoutConfig.asset!");
                return;
            }

            GameObject existingGrid = GameObject.Find("WorldMapGrid");
            if (existingGrid != null) UnityEngine.Object.DestroyImmediate(existingGrid);

            GameObject gridGo = new GameObject("WorldMapGrid");
            gridGo.AddComponent<Grid>();

            GameObject groundGo = new GameObject("GroundTilemap");
            groundGo.transform.SetParent(gridGo.transform, false);
            var groundTm = groundGo.AddComponent<Tilemap>();
            var groundRenderer = groundGo.AddComponent<TilemapRenderer>();
            groundRenderer.sortingLayerName = "Ground";

            GameObject obsGo = new GameObject("ObstacleTilemap");
            obsGo.transform.SetParent(gridGo.transform, false);
            obsGo.layer = 10;
            var obsTm = obsGo.AddComponent<Tilemap>();
            var obsRenderer = obsGo.AddComponent<TilemapRenderer>();
            obsRenderer.sortingLayerName = "WorldObjects";
            obsRenderer.sortingOrder = -5000;
            obsGo.AddComponent<TilemapCollider2D>();

            Tile[] grassTiles =
            {
                CreateTile(LoadEnvSprite("Tiles/tile_grass_0.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_grass_1.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_grass_2.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_grass_3.png"))
            };
            Tile[] pathTiles =
            {
                CreateTile(LoadEnvSprite("Tiles/tile_path_0.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_path_1.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_path_2.png"))
            };
            Tile[] ghFloorTiles =
            {
                CreateTile(LoadEnvSprite("Tiles/tile_gh_floor_0.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_gh_floor_1.png"))
            };
            Tile tWall = CreateTile(LoadEnvSprite("Tiles/tile_wall.png"));
            Tile tFence = CreateTile(LoadEnvSprite("Tiles/tile_fence.png"));
            Tile tTree = CreateTile(LoadEnvSprite("Tiles/tile_tree.png"));
            Tile tWater = CreateTile(LoadEnvSprite("Tiles/tile_water.png"));
            Tile tFlowerbed = CreateTile(LoadEnvSprite("Tiles/tile_flowerbed.png"));

            string[] lines = cfg.asciiMap.Split('\n');
            int height = lines.Length;
            for (int r = 0; r < height; r++)
            {
                string row = lines[r].TrimEnd('\r');
                int y = cfg.originY + (height - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    int x = cfg.originX + c;
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    char ch = row[c];
                    int hash = Mathf.Abs(x * 73856093 ^ y * 19349663);

                    if (ch == 'G')
                    {
                        groundTm.SetTile(cell, ghFloorTiles[hash % ghFloorTiles.Length]);
                    }
                    else if (ch == 'P')
                    {
                        groundTm.SetTile(cell, pathTiles[hash % pathTiles.Length]);
                    }
                    else if (ch == 'B')
                    {
                        groundTm.SetTile(cell, tFlowerbed);
                    }
                    else
                    {
                        // Phân bổ cỏ tự nhiên: 55% cỏ nền, 45% cỏ 3 lá/hoa nhỏ
                        int gIdx = (hash % 10 < 5) ? 0 : (1 + (hash % 3));
                        groundTm.SetTile(cell, grassTiles[gIdx]);
                    }

                    if (ch == '#') obsTm.SetTile(cell, tWall);
                    else if (ch == 'F') obsTm.SetTile(cell, tFence);
                    else if (ch == 'T') obsTm.SetTile(cell, tTree);
                    else if (ch == 'W') obsTm.SetTile(cell, tWater);
                }
            }
        }

        private static Tile CreateTile(Sprite spr)
        {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static void UpgradeFarmSoilSprites()
        {
            Sprite untilled = LoadEnvSprite("Soil/soil_untilled.png");
            Sprite dry = LoadEnvSprite("Soil/soil_dry.png");
            Sprite wet = LoadEnvSprite("Soil/soil_wet.png");

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            for (int i = 0; i < tiles.Length; i++)
            {
                var so = new SerializedObject(tiles[i]);
                if (untilled != null) so.FindProperty("untilledSprite").objectReferenceValue = untilled;
                if (dry != null) so.FindProperty("tilledDrySprite").objectReferenceValue = dry;
                if (wet != null) so.FindProperty("tilledWetSprite").objectReferenceValue = wet;
                so.ApplyModifiedPropertiesWithoutUndo();

                var sr = tiles[i].GetComponent<SpriteRenderer>();
                if (sr != null && untilled != null) sr.sprite = untilled;
            }
        }

        private static void UpgradeInteractiveProps()
        {
            // 1. Giường Nhà Kính
            GameObject bed = GameObject.Find("GreenhouseBed");
            Sprite bedSpr = LoadEnvSprite("Props/prop_bed.png");
            if (bed != null && bedSpr != null)
            {
                var sr = bed.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = bedSpr;
            }

            // 2. Giếng Nước có mái ngói
            GameObject well = GameObject.Find("VillageWaterWell");
            Sprite wellSpr = LoadEnvSprite("Props/prop_well.png");
            if (well != null && wellSpr != null)
            {
                var sr = well.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = wellSpr;
            }

            // 3. Bồn Hoa Trung Tâm (Empty + Bloomed 48x48)
            var spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            Sprite emptySpr = LoadEnvSprite("Props/planting_spot_empty.png");
            Sprite bloomedSpr = LoadEnvSprite("Props/planting_spot_bloomed.png");
            if (spot != null && emptySpr != null && bloomedSpr != null)
            {
                var so = new SerializedObject(spot);
                so.FindProperty("emptySprite").objectReferenceValue = emptySpr;
                so.FindProperty("bloomedSprite").objectReferenceValue = bloomedSpr;
                so.ApplyModifiedPropertiesWithoutUndo();

                var sr = spot.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = emptySpr;
            }
        }

        private static void UpgradeVillageDecorAndBuildings()
        {
            GameObject oldDecor = GameObject.Find("VillageDecorGroup");
            if (oldDecor != null) UnityEngine.Object.DestroyImmediate(oldDecor);
            GameObject group = new GameObject("VillageDecorGroup");

            Sprite bakerySpr = LoadEnvSprite("Buildings/house_bakery.png");
            Sprite shedSpr = LoadEnvSprite("Buildings/house_shed.png");
            Sprite noticeSpr = LoadEnvSprite("Buildings/otis_notice_board.png");
            Sprite oakSpr = LoadEnvSprite("Buildings/tree_hero_oak.png");
            Sprite blossomSpr = LoadEnvSprite("Buildings/tree_hero_blossom.png");
            Sprite[] flowerSprs =
            {
                LoadEnvSprite("Decor/decor_flowers_0.png"),
                LoadEnvSprite("Decor/decor_flowers_1.png"),
                LoadEnvSprite("Decor/decor_flowers_2.png")
            };
            Sprite bushSpr = LoadEnvSprite("Decor/decor_bush.png");
            Sprite rockSpr = LoadEnvSprite("Decor/decor_rock.png");
            Sprite lilySpr = LoadEnvSprite("Decor/decor_lilypad.png");

            // 1. Kiến trúc chính: Tiệm Bánh Bà Hana, Chòi Vườn Ông Bách, Bảng Tin Bác Otis
            SpawnDecor(group.transform, "House_HanaBakery", bakerySpr, new Vector2(-6.5f, 5.8f), true, new Vector2(4.2f, 2.0f), new Vector2(0f, -0.7f));
            SpawnDecor(group.transform, "House_BachShed", shedSpr, new Vector2(-4.8f, -4.5f), true, new Vector2(3.2f, 1.7f), new Vector2(0f, -0.6f));
            SpawnDecor(group.transform, "OtisNoticeBoard", noticeSpr, new Vector2(2.8f, 6.2f), true, new Vector2(1.4f, 0.8f), new Vector2(0f, -0.4f));

            // 2. 6 Cây lớn (Cây Hoa Ánh Trăng hồng & Cây Sồi xanh) tạo điểm nhấn 4 phân khu
            SpawnDecor(group.transform, "HeroTree_Blossom_1", blossomSpr, new Vector2(-9.5f, 11.2f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Blossom_2", blossomSpr, new Vector2(5.8f, 9.2f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Blossom_3", blossomSpr, new Vector2(-5.5f, -11.8f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Blossom_4", blossomSpr, new Vector2(5.5f, -11.8f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Oak_1", oakSpr, new Vector2(13.2f, 10.2f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Oak_2", oakSpr, new Vector2(19.2f, 4.2f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));

            // 3. Hoa súng trên mặt ao cạnh Nhà Kính
            SpawnDecor(group.transform, "PondLily_1", lilySpr, new Vector2(-6.0f, 12.0f), false, Vector2.zero, Vector2.zero);
            SpawnDecor(group.transform, "PondLily_2", lilySpr, new Vector2(-5.1f, 11.9f), false, Vector2.zero, Vector2.zero);

            // 4. Đá rêu & bụi cây ven đường
            Vector2[] bushes =
            {
                new Vector2(-9.2f, 5.2f), new Vector2(-3.8f, 5.2f), new Vector2(4.4f, 5.4f),
                new Vector2(9.2f, 4.2f), new Vector2(-5.8f, -6.2f), new Vector2(5.8f, -6.2f)
            };
            for (int i = 0; i < bushes.Length; i++)
            {
                SpawnDecor(group.transform, "DecorBush_" + (i + 1), bushSpr, bushes[i], true, new Vector2(0.75f, 0.6f), new Vector2(0f, -0.15f));
            }

            Vector2[] rocks =
            {
                new Vector2(-10.5f, 9.0f), new Vector2(10.8f, 9.0f), new Vector2(14.0f, 3.0f), new Vector2(-7.8f, -13.0f)
            };
            for (int i = 0; i < rocks.Length; i++)
            {
                SpawnDecor(group.transform, "MossRock_" + (i + 1), rockSpr, rocks[i], true, new Vector2(0.7f, 0.55f), new Vector2(0f, -0.15f));
            }

            // 5. 18 cụm hoa dại đa màu rải đều khắp thung lũng
            Vector2[] flowers =
            {
                new Vector2(-11.0f, 3.0f), new Vector2(-8.5f, 1.5f), new Vector2(-3.0f, 1.0f), new Vector2(3.5f, 1.2f),
                new Vector2(6.5f, 4.5f), new Vector2(-2.5f, -2.0f), new Vector2(4.0f, -2.2f), new Vector2(-5.0f, -9.5f),
                new Vector2(5.2f, -9.5f), new Vector2(13.5f, 9.0f), new Vector2(18.5f, 4.5f), new Vector2(14.5f, 2.0f),
                new Vector2(-10.2f, 12.6f), new Vector2(-2.8f, 9.6f), new Vector2(8.8f, 8.2f), new Vector2(-2.6f, -13.2f),
                new Vector2(2.6f, -13.2f), new Vector2(11.5f, 6.2f)
            };
            for (int i = 0; i < flowers.Length; i++)
            {
                SpawnDecor(group.transform, "FlowerTuft_" + (i + 1), flowerSprs[i % flowerSprs.Length], flowers[i], false, Vector2.zero, Vector2.zero);
            }
        }

        private static void SpawnDecor(Transform parent, string name, Sprite spr, Vector2 pos, bool solid, Vector2 colSize, Vector2 colOffset)
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = solid ? "WorldObjects" : "FloorDecal";
            sr.sortingOrder = solid ? Mathf.RoundToInt(-pos.y * 100f) : 12;
            if (solid)
            {
                go.layer = 10;
                var col = go.AddComponent<BoxCollider2D>();
                col.size = colSize;
                col.offset = colOffset;
            }
        }
    }
}
