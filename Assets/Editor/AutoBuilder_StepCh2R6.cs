#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2R6
    {
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string WestSwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";

        private const string EnvRoot = "Assets/Art/Sprites/Environment/";
        private const string Ch2Root = "Assets/Art/Sprites/Chapter2/";
        private const string Ch2OverhaulRoot = "Assets/Art/Sprites/Chapter2/Overhaul/";
        private const string SwampTilesRoot = "Assets/Data/Chapter2/SwampTiles";

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R6 - 1. Seamless Swamp Road & Real Pixel-Art WestSwamp")]
        public static void BuildStepCh2R6()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Ch2-R6] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Ch2-R6] === BẮT ĐẦU STEP CH2-R6: ĐƯỜNG NỐI LIỀN 2 MÀN HÌNH & ART THẬT CHO WESTSWAMP ===");

            RebuildWestSwampArtAndRoad();
            UpgradeRiverbankNorthWestRoad();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Ch2-R6] === HOÀN TẤT STEP CH2-R6! Chạy 'Step Ch2-R6 - 2. Validate Step Ch2-R6' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R6 - 2. Validate Step Ch2-R6")]
        public static void ValidateStepCh2R6()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-R6] === BẮT ĐẦU VALIDATE STEP CH2-R6 ===");

            // 1. Validate WestSwamp_Chapter2
            var swampScene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);
            if (!swampScene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R6][FAIL] Không mở được " + WestSwampScenePath);
                return;
            }

            if (GameObject.Find("WestSwampRealEnvironment") == null)
            {
                Debug.LogError("[MV][Ch2-R6][FAIL] WestSwamp thiếu WestSwampRealEnvironment!");
                errors++;
            }

            GameObject retPortal = GameObject.Find("Portal_ToRiverbankWest");
            if (retPortal == null)
            {
                Debug.LogError("[MV][Ch2-R6][FAIL] WestSwamp thiếu Portal_ToRiverbankWest!");
                errors++;
            }
            else
            {
                var pt = retPortal.GetComponent<ScenePortalTrigger>();
                var col = retPortal.GetComponent<BoxCollider2D>();
                if (pt == null || col == null || !col.isTrigger)
                {
                    Debug.LogError("[MV][Ch2-R6][FAIL] Portal_ToRiverbankWest thiếu ScenePortalTrigger hoặc BoxCollider2D (isTrigger)!");
                    errors++;
                }
                else
                {
                    var so = new SerializedObject(pt);
                    if (!so.FindProperty("autoTriggerOnWalk").boolValue)
                    {
                        Debug.LogError("[MV][Ch2-R6][FAIL] Portal_ToRiverbankWest chưa bật autoTriggerOnWalk!");
                        errors++;
                    }
                }
            }

            if (Object.FindAnyObjectByType<SwampWaveCombatController>() == null)
            {
                Debug.LogError("[MV][Ch2-R6][FAIL] WestSwamp thiếu SwampWaveCombatController!");
                errors++;
            }
            if (Object.FindAnyObjectByType<EchoSpotInteractable>() == null)
            {
                Debug.LogError("[MV][Ch2-R6][FAIL] WestSwamp thiếu EchoSpot_StormLantern!");
                errors++;
            }
            errors += CountMissingScripts("WestSwamp_Chapter2");

            // 2. Validate Riverbank_Chapter2
            var riverScene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!riverScene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R6][FAIL] Không mở được " + Ch2ScenePath);
                return;
            }

            GameObject toSwamp = GameObject.Find("Portal_ToWestSwamp");
            if (toSwamp == null)
            {
                Debug.LogError("[MV][Ch2-R6][FAIL] Riverbank thiếu Portal_ToWestSwamp!");
                errors++;
            }
            else
            {
                var pt = toSwamp.GetComponent<ScenePortalTrigger>();
                var col = toSwamp.GetComponent<BoxCollider2D>();
                if (pt == null || col == null || !col.isTrigger)
                {
                    Debug.LogError("[MV][Ch2-R6][FAIL] Portal_ToWestSwamp thiếu ScenePortalTrigger hoặc BoxCollider2D (isTrigger)!");
                    errors++;
                }
                else
                {
                    var so = new SerializedObject(pt);
                    if (!so.FindProperty("autoTriggerOnWalk").boolValue || !so.FindProperty("useSwampUnlockGate").boolValue)
                    {
                        Debug.LogError("[MV][Ch2-R6][FAIL] Portal_ToWestSwamp cần bật cả autoTriggerOnWalk và useSwampUnlockGate!");
                        errors++;
                    }
                }
            }
            errors += CountMissingScripts("Riverbank_Chapter2");

            if (errors == 0)
                Debug.Log("[MV][Ch2-R6] VALIDATE PASS (0 lỗi) - Đường nối đi bộ liền mạch 2 màn hình & Art pixel-art thật cho WestSwamp đã sẵn sàng!");
            else
                Debug.LogError($"[MV][Ch2-R6] VALIDATE FAIL ({errors} lỗi).");
        }

        private static void RebuildWestSwampArtAndRoad()
        {
            var scene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R6] Không mở được " + WestSwampScenePath);
                return;
            }

            EnsureFolder(SwampTilesRoot);

            // Load real project sprites
            Sprite sprSwamp = LoadFirstSprite(Ch2Root + "tile_swamp_ground.png", EnvRoot + "Tiles/tile_grass_0.png");
            Sprite sprGrass0 = LoadFirstSprite(EnvRoot + "Tiles/tile_grass_0.png", EnvRoot + "spr_tile_grass.png");
            Sprite sprGrass1 = LoadFirstSprite(EnvRoot + "Tiles/tile_grass_1.png", EnvRoot + "Tiles/tile_grass_0.png");
            Sprite sprGrass2 = LoadFirstSprite(EnvRoot + "Tiles/tile_grass_2.png", EnvRoot + "Tiles/tile_grass_0.png");
            Sprite sprPath0 = LoadFirstSprite(EnvRoot + "Tiles/tile_path_0.png", EnvRoot + "spr_tile_path.png");
            Sprite sprPath1 = LoadFirstSprite(EnvRoot + "Tiles/tile_path_1.png", EnvRoot + "Tiles/tile_path_0.png");
            Sprite sprWaterDeep = LoadFirstSprite(Ch2OverhaulRoot + "ch2_water_deep_00.png", Ch2Root + "tile_river_water_00.png");
            Sprite sprWaterShallow = LoadFirstSprite(Ch2OverhaulRoot + "ch2_water_shallow_00.png", Ch2Root + "tile_river_water_00.png");

            Sprite sprPine = LoadFirstSprite(EnvRoot + "Buildings/tree_hero_pine.png", EnvRoot + "Tiles/tile_tree.png");
            Sprite sprOak = LoadFirstSprite(EnvRoot + "Buildings/tree_hero_oak.png", EnvRoot + "Tiles/tile_tree.png");
            Sprite sprBush = LoadFirstSprite(EnvRoot + "Decor/decor_bush.png", EnvRoot + "spr_decor_bush.png");
            Sprite sprTall0 = LoadFirstSprite(EnvRoot + "Decor/decor_tall_grass_0.png", EnvRoot + "Decor/decor_bush.png");
            Sprite sprTall1 = LoadFirstSprite(EnvRoot + "Decor/decor_tall_grass_1.png", EnvRoot + "Decor/decor_tall_grass_0.png");
            Sprite sprRock = LoadFirstSprite(EnvRoot + "Decor/decor_rock.png", Ch2OverhaulRoot + "ch2_river_rock_0.png");
            Sprite sprRiverRock0 = LoadFirstSprite(Ch2OverhaulRoot + "ch2_river_rock_0.png", EnvRoot + "Decor/decor_rock.png");
            Sprite sprLily0 = LoadFirstSprite(Ch2OverhaulRoot + "ch2_lilypad_0.png", EnvRoot + "Decor/decor_lilypad.png");
            Sprite sprLily1 = LoadFirstSprite(Ch2OverhaulRoot + "ch2_lilypad_1.png", EnvRoot + "Decor/decor_lilypad.png");
            Sprite sprReeds = LoadFirstSprite(EnvRoot + "Decor/decor_reeds.png", EnvRoot + "Decor/decor_tall_grass_0.png");
            Sprite sprBranch = LoadFirstSprite(EnvRoot + "Props/prop_dry_branch.png", EnvRoot + "spr_prop_branch.png");
            Sprite sprDebris = LoadFirstSprite(EnvRoot + "Props/prop_greenhouse_debris.png", EnvRoot + "spr_prop_debris.png");
            Sprite sprGardenLantern = LoadFirstSprite(EnvRoot + "Decor/decor_garden_lantern.png", EnvRoot + "spr_prop_lantern.png");
            Sprite sprMist = LoadFirstSprite(Ch2OverhaulRoot + "ch2_river_mist_strip.png", Ch2Root + "ch2_river_mist_strip.png");

            Tile tSwamp = GetOrCreateTile(SwampTilesRoot + "/WS_SwampGround.asset", sprSwamp, Tile.ColliderType.None);
            Tile tGrass0 = GetOrCreateTile(SwampTilesRoot + "/WS_Grass0.asset", sprGrass0, Tile.ColliderType.None);
            Tile tGrass1 = GetOrCreateTile(SwampTilesRoot + "/WS_Grass1.asset", sprGrass1, Tile.ColliderType.None);
            Tile tGrass2 = GetOrCreateTile(SwampTilesRoot + "/WS_Grass2.asset", sprGrass2, Tile.ColliderType.None);
            Tile tPath0 = GetOrCreateTile(SwampTilesRoot + "/WS_Path0.asset", sprPath0, Tile.ColliderType.None);
            Tile tPath1 = GetOrCreateTile(SwampTilesRoot + "/WS_Path1.asset", sprPath1, Tile.ColliderType.None);
            Tile tWaterDeep = GetOrCreateTile(SwampTilesRoot + "/WS_WaterDeep.asset", sprWaterDeep, Tile.ColliderType.None);
            Tile tWaterShallow = GetOrCreateTile(SwampTilesRoot + "/WS_WaterShallow.asset", sprWaterShallow, Tile.ColliderType.None);
            Tile tBlock = GetOrCreateTile(SwampTilesRoot + "/WS_InvisibleBlock.asset", sprWaterDeep, Tile.ColliderType.Grid);

            // Find or build Grid/Tilemaps
            Grid grid = Object.FindAnyObjectByType<Grid>();
            if (grid == null)
            {
                GameObject gGo = new GameObject("Grid");
                grid = gGo.AddComponent<Grid>();
            }
            Tilemap groundTm = FindOrCreateTilemap(grid.transform, "GroundTilemap", "Ground", 0, false);
            Tilemap obsTm = FindOrCreateTilemap(grid.transform, "ObstacleTilemap", "Ground", 2, true);
            var obsRen = obsTm.GetComponent<TilemapRenderer>();
            if (obsRen != null) obsRen.enabled = false; // Collider vô hình, cây & đá vẽ bằng sprite thật có YSort!

            groundTm.ClearAllTiles();
            obsTm.ClearAllTiles();

            // Paint 44x28 Swamp Arena: South road entrance (x=-2..1, y=-14..-7), wide playable arena, NW Shrine (-12..-7, 4..8)
            for (int y = -14; y <= 13; y++)
            {
                for (int x = -22; x <= 21; x++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    bool isSouthRoadGap = (x >= -2 && x <= 1 && y <= -8);
                    bool isOuterBorder = (x <= -19 || x >= 18 || y >= 10 || (y <= -11 && !isSouthRoadGap));

                    // Organic flank water ponds (West pond & East pond) - keep center & shrine wide open for 70-enemy combat!
                    bool inEastPond = ((x - 13) * (x - 13) / 14f + (y - 3) * (y - 3) / 10f) <= 1.0f;
                    bool inWestPond = ((x + 15) * (x + 15) / 11f + (y + 6) * (y + 6) / 9f) <= 1.0f;
                    bool isWater = (inEastPond || inWestPond) && !isSouthRoadGap;

                    // Winding trail from South entrance (0, -13) -> Center (0, -1) -> NW Shrine (-9, 6)
                    bool isTrail =
                        (x >= -2 && x <= 1 && y >= -14 && y <= -2) ||
                        (y >= -2 && y <= 0 && x >= -7 && x <= 1) ||
                        (x >= -10 && x <= -7 && y >= 0 && y <= 6) ||
                        (y >= 4 && y <= 7 && x >= -12 && x <= -7);

                    int hash = Mathf.Abs(x * 73856093 ^ y * 19349663) % 10;

                    if (isWater)
                    {
                        bool deep = ((x - 13) * (x - 13) / 6f + (y - 3) * (y - 3) / 4f) <= 1.0f ||
                                    ((x + 15) * (x + 15) / 5f + (y + 6) * (y + 6) / 4f) <= 1.0f;
                        groundTm.SetTile(cell, deep ? tWaterDeep : tWaterShallow);
                        obsTm.SetTile(cell, tBlock);
                    }
                    else if (isTrail)
                    {
                        groundTm.SetTile(cell, hash < 7 ? tPath0 : tPath1);
                    }
                    else if (isOuterBorder)
                    {
                        groundTm.SetTile(cell, hash < 5 ? tGrass0 : (hash < 8 ? tGrass1 : tGrass2));
                        if (x == -22 || x == 21 || y == 13 || y == -14 || (x <= -19 || x >= 18 || y >= 10 || y <= -11))
                        {
                            if (!isSouthRoadGap) obsTm.SetTile(cell, tBlock);
                        }
                    }
                    else
                    {
                        // Playable swamp clearing: rich mix of swamp mud & mossy grass patches
                        bool mossyPatch = hash >= 7;
                        groundTm.SetTile(cell, mossyPatch ? (hash == 9 ? tGrass2 : tGrass1) : tSwamp);
                    }
                }
            }

            // Clear old procedural / code-drawn decor roots if any
            DestroyIfExists("WestSwampRealEnvironment");
            DestroyIfExists("SwampAmbientMist");
            DestroyIfExists("AncientLanternShrine");

            // Hide ugly code-drawn circles on SwampNest_* (preserve transforms until Step Ch2-R7 removes nests)
            string[] nestNames = { "SwampNest_North", "SwampNest_West", "SwampNest_East" };
            for (int i = 0; i < nestNames.Length; i++)
            {
                GameObject nest = GameObject.Find(nestNames[i]);
                if (nest != null)
                {
                    var srs = nest.GetComponentsInChildren<SpriteRenderer>(true);
                    for (int k = 0; k < srs.Length; k++) srs[k].enabled = false;
                }
            }

            // Build rich real-asset environment
            GameObject envRoot = new GameObject("WestSwampRealEnvironment");
            var rng = new System.Random(202602);

            // 1. Perimeter Pine & Oak forest wall framing the swamp + South road corridor
            for (int x = -21; x <= 20; x += 3)
            {
                SpawnProp(envRoot.transform, $"TreeNorth_{x}", (x % 2 == 0) ? sprPine : sprOak,
                    new Vector3(x + (float)(rng.NextDouble() * 0.8 - 0.4), 10.6f + (float)(rng.NextDouble() * 0.9), 0f),
                    Vector3.one * 1.12f, new Color(0.76f, 0.86f, 0.82f, 1f), true);

                if (x < -3 || x > 3)
                {
                    SpawnProp(envRoot.transform, $"TreeSouth_{x}", (x % 2 == 0) ? sprOak : sprPine,
                        new Vector3(x + (float)(rng.NextDouble() * 0.7 - 0.35), -11.4f - (float)(rng.NextDouble() * 0.6), 0f),
                        Vector3.one * 1.08f, new Color(0.74f, 0.84f, 0.80f, 1f), true);
                }
            }
            for (int y = -10; y <= 9; y += 3)
            {
                SpawnProp(envRoot.transform, $"TreeWest_{y}", (y % 2 == 0) ? sprPine : sprOak,
                    new Vector3(-19.5f - (float)(rng.NextDouble() * 0.8), y, 0f),
                    Vector3.one * 1.12f, new Color(0.74f, 0.84f, 0.82f, 1f), true);
                SpawnProp(envRoot.transform, $"TreeEast_{y}", (y % 2 == 0) ? sprOak : sprPine,
                    new Vector3(18.6f + (float)(rng.NextDouble() * 0.8), y, 0f),
                    Vector3.one * 1.12f, new Color(0.74f, 0.84f, 0.82f, 1f), true);
            }

            // Road entrance lanterns framing the South road from Riverbank_Chapter2
            SpawnProp(envRoot.transform, "SouthRoadLantern_L", sprGardenLantern, new Vector3(-2.6f, -9.8f, 0f), Vector3.one, Color.white, true);
            SpawnProp(envRoot.transform, "SouthRoadLantern_R", sprGardenLantern, new Vector3(2.4f, -9.8f, 0f), Vector3.one, Color.white, true);

            // 2. Pond lilypads, river rocks & reeds (using real Ch2 overhaul sprites)
            Vector3[] eastPondDecor = { new Vector3(11.5f, 3.2f, 0f), new Vector3(13.8f, 4.1f, 0f), new Vector3(14.6f, 2.0f, 0f), new Vector3(12.2f, 1.4f, 0f) };
            for (int i = 0; i < eastPondDecor.Length; i++)
                SpawnProp(envRoot.transform, $"EastPondLily_{i}", i % 2 == 0 ? sprLily0 : sprLily1, eastPondDecor[i], Vector3.one, Color.white, false, "Ground", 5);
            SpawnProp(envRoot.transform, "EastPondRock", sprRiverRock0, new Vector3(10.4f, 4.3f, 0f), Vector3.one, Color.white, true);
            SpawnProp(envRoot.transform, "EastPondReeds_1", sprReeds, new Vector3(10.2f, 1.6f, 0f), Vector3.one, Color.white, true);
            SpawnProp(envRoot.transform, "EastPondReeds_2", sprReeds, new Vector3(15.6f, 4.5f, 0f), Vector3.one, Color.white, true);

            Vector3[] westPondDecor = { new Vector3(-14.2f, -5.6f, 0f), new Vector3(-16.0f, -6.8f, 0f), new Vector3(-13.6f, -7.2f, 0f) };
            for (int i = 0; i < westPondDecor.Length; i++)
                SpawnProp(envRoot.transform, $"WestPondLily_{i}", i % 2 == 0 ? sprLily1 : sprLily0, westPondDecor[i], Vector3.one, Color.white, false, "Ground", 5);
            SpawnProp(envRoot.transform, "WestPondReeds_1", sprReeds, new Vector3(-12.2f, -5.0f, 0f), Vector3.one, Color.white, true);
            SpawnProp(envRoot.transform, "WestPondRock", sprRiverRock0, new Vector3(-12.6f, -7.8f, 0f), Vector3.one, Color.white, true);

            // 3. Natural swamp bushes, tall grass, fallen branches, rocks & moss debris scattered cleanly
            Vector3[] bushSpots = {
                new Vector3(-6.5f, -7.5f, 0f), new Vector3(6.2f, -7.2f, 0f), new Vector3(8.5f, -3.5f, 0f),
                new Vector3(-13.5f, 0.5f, 0f), new Vector3(4.5f, 7.2f, 0f), new Vector3(-2.5f, 7.6f, 0f),
                new Vector3(12.0f, -6.5f, 0f), new Vector3(-15.5f, 3.2f, 0f)
            };
            for (int i = 0; i < bushSpots.Length; i++)
                SpawnProp(envRoot.transform, $"SwampBush_{i}", sprBush, bushSpots[i], Vector3.one, new Color(0.82f, 0.92f, 0.86f, 1f), true);

            Vector3[] grassSpots = {
                new Vector3(-4.2f, -4.5f, 0f), new Vector3(4.8f, -4.2f, 0f), new Vector3(5.6f, 2.8f, 0f),
                new Vector3(-3.8f, 3.5f, 0f), new Vector3(1.8f, 5.2f, 0f), new Vector3(-9.5f, -3.6f, 0f),
                new Vector3(9.4f, -0.8f, 0f), new Vector3(-14.2f, 7.4f, 0f), new Vector3(7.2f, 6.4f, 0f)
            };
            for (int i = 0; i < grassSpots.Length; i++)
                SpawnProp(envRoot.transform, $"TallGrass_{i}", i % 2 == 0 ? sprTall0 : sprTall1, grassSpots[i], Vector3.one, Color.white, false, "Ground", 6);

            SpawnProp(envRoot.transform, "SwampBranch_1", sprBranch, new Vector3(5.4f, -1.8f, 0f), Vector3.one, Color.white, false, "Ground", 6);
            SpawnProp(envRoot.transform, "SwampBranch_2", sprBranch, new Vector3(-6.2f, -2.8f, 0f), Vector3.one, Color.white, false, "Ground", 6);
            SpawnProp(envRoot.transform, "SwampDebris_1", sprDebris, new Vector3(3.2f, 6.8f, 0f), Vector3.one, new Color(0.82f, 0.90f, 0.86f, 1f), true);
            SpawnProp(envRoot.transform, "SwampRock_1", sprRock, new Vector3(-13.8f, 6.8f, 0f), Vector3.one, Color.white, true);
            SpawnProp(envRoot.transform, "SwampRock_2", sprRock, new Vector3(7.6f, 7.6f, 0f), Vector3.one, Color.white, true);

            // 4. Ancient Storm Lantern Shrine at NW clearing (-9.5, 6.2) using real lantern & rock props
            GameObject shrine = new GameObject("AncientLanternShrine");
            shrine.transform.position = new Vector3(-9.5f, 6.2f, 0f);
            SpawnProp(shrine.transform, "ShrineLantern_L", sprGardenLantern, new Vector3(-1.8f, 0.3f, 0f), Vector3.one, Color.white, true);
            SpawnProp(shrine.transform, "ShrineLantern_R", sprGardenLantern, new Vector3(1.8f, 0.3f, 0f), Vector3.one, Color.white, true);
            SpawnProp(shrine.transform, "ShrineRock_Back", sprRock, new Vector3(0f, 1.35f, 0f), new Vector3(1.25f, 1.1f, 1f), new Color(0.82f, 0.88f, 0.92f, 1f), true);

            // Reposition EchoSpot_StormLantern right onto the Shrine pedestal
            EchoSpotInteractable echo = Object.FindAnyObjectByType<EchoSpotInteractable>();
            if (echo != null)
            {
                echo.transform.position = new Vector3(-9.5f, 6.0f, 0f);
                EditorUtility.SetDirty(echo);
            }

            // 5. Subtle drifting swamp mist ribbons
            if (sprMist != null)
            {
                GameObject mistRoot = new GameObject("SwampAmbientMist");
                Vector3[] mistPos = { new Vector3(-8f, 1.5f, 0f), new Vector3(5f, -2.5f, 0f), new Vector3(2f, 5.5f, 0f), new Vector3(-10f, -6.5f, 0f) };
                for (int i = 0; i < mistPos.Length; i++)
                {
                    GameObject mGo = new GameObject($"SwampMist_{i}");
                    mGo.transform.SetParent(mistRoot.transform, false);
                    mGo.transform.position = mistPos[i];
                    mGo.transform.localScale = new Vector3(1.8f, 1.15f, 1f);
                    var sr = mGo.AddComponent<SpriteRenderer>();
                    sr.sprite = sprMist;
                    sr.color = new Color(0.60f, 0.78f, 0.82f, 0.28f);
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = 180;
                    var drifter = mGo.AddComponent<RiverMistDrifter>();
                    drifter.Configure(0.55f, 0.35f, 0.28f, i * 1.5f);
                }
            }

            // 6. Rebuild Portal_ToRiverbankWest as seamless South Road Walk-Through Trigger (NO ring, NO [E])
            DestroyIfExists("Portal_ToRiverbankWest");
            GameObject retPortal = new GameObject("Portal_ToRiverbankWest");
            retPortal.transform.position = new Vector3(-0.5f, -12.1f, 0f);
            BoxCollider2D retCol = retPortal.AddComponent<BoxCollider2D>();
            retCol.isTrigger = true;
            retCol.size = new Vector2(4.8f, 1.4f);

            ScenePortalTrigger retTrig = retPortal.AddComponent<ScenePortalTrigger>();
            var rso = new SerializedObject(retTrig);
            rso.FindProperty("targetSceneName").stringValue = Chapter2StateManager.Chapter2SceneName;
            rso.FindProperty("targetSpawnId").stringValue = "Spawn_WestSwamp";
            rso.FindProperty("requiredFlag").stringValue = "";
            rso.FindProperty("useSwampUnlockGate").boolValue = false;
            rso.FindProperty("autoTriggerOnWalk").boolValue = true;
            rso.FindProperty("walkCooldown").floatValue = 0.85f;
            rso.FindProperty("promptLabel").stringValue = "Quay lại Bờ Tây Sông";
            rso.ApplyModifiedPropertiesWithoutUndo();

            // 7. Update Spawn Points + Player default start + Camera bounds
            WorldSpawnRegistry reg = Object.FindAnyObjectByType<WorldSpawnRegistry>();
            if (reg != null)
            {
                var sso = new SerializedObject(reg);
                var list = sso.FindProperty("spawnPoints");
                var defs = new[]
                {
                    ("Spawn_SwampEntrance", "Đường Vào Đầm Lầy Bờ Tây", new Vector2(-0.5f, -9.4f)),
                    ("Spawn_SwampCenter", "Bãi Sương Trung Tâm", new Vector2(0f, 0.5f)),
                    ("Spawn_SwampShrine", "Miếu Đèn Bão Cổ", new Vector2(-9.5f, 4.6f))
                };
                list.ClearArray();
                for (int i = 0; i < defs.Length; i++)
                {
                    list.InsertArrayElementAtIndex(i);
                    var el = list.GetArrayElementAtIndex(i);
                    el.FindPropertyRelative("id").stringValue = defs[i].Item1;
                    el.FindPropertyRelative("displayName").stringValue = defs[i].Item2;
                    el.FindPropertyRelative("position").vector2Value = defs[i].Item3;
                }
                sso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(reg);
            }

            PlayerController2D pc = Object.FindAnyObjectByType<PlayerController2D>();
            if (pc != null) pc.transform.position = new Vector3(-0.5f, -9.4f, 0f);
            LumiCompanionController lumi = Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null) lumi.transform.position = new Vector3(-1.3f, -8.8f, 0f);
            CameraFollow2D camFollow = Object.FindAnyObjectByType<CameraFollow2D>();
            if (camFollow != null)
            {
                var cso = new SerializedObject(camFollow);
                cso.FindProperty("minBounds").vector2Value = new Vector2(-14.5f, -8.8f);
                cso.FindProperty("maxBounds").vector2Value = new Vector2(14.5f, 7.6f);
                cso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(camFollow);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Ch2-R6] Đã dựng lại toàn bộ art WestSwamp_Chapter2 bằng 100% asset thật & lối đi bộ phía Nam!");
        }

        private static void UpgradeRiverbankNorthWestRoad()
        {
            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R6] Không mở được " + Ch2ScenePath);
                return;
            }

            // Paint a clear dirt road corridor leading North out of Riverbank_Chapter2 at x = -18..-15, y = 5..13
            Sprite sprPath0 = LoadFirstSprite(EnvRoot + "Tiles/tile_path_0.png", EnvRoot + "spr_tile_path.png");
            Sprite sprSwamp = LoadFirstSprite(Ch2Root + "tile_swamp_ground.png", EnvRoot + "Tiles/tile_path_0.png");
            Sprite sprGardenLantern = LoadFirstSprite(EnvRoot + "Decor/decor_garden_lantern.png", EnvRoot + "spr_prop_lantern.png");

            EnsureFolder(SwampTilesRoot);
            Tile tPath = GetOrCreateTile(SwampTilesRoot + "/WS_Path0.asset", sprPath0, Tile.ColliderType.None);
            Tile tSwamp = GetOrCreateTile(SwampTilesRoot + "/WS_SwampGround.asset", sprSwamp, Tile.ColliderType.None);

            Tilemap groundTm = FindTilemapByName("GroundTilemap");
            Tilemap obsTm = FindTilemapByName("ObstacleTilemap");
            if (groundTm != null)
            {
                for (int y = 5; y <= 13; y++)
                {
                    for (int x = -18; x <= -15; x++)
                    {
                        var cell = new Vector3Int(x, y, 0);
                        groundTm.SetTile(cell, y >= 10 ? tSwamp : tPath);
                        if (obsTm != null) obsTm.SetTile(cell, null);
                    }
                }
            }

            // Replace Portal_ToWestSwamp with seamless Walk-Through Road Exit at North edge (-16.5, 11.4) - NO glowing ring!
            DestroyIfExists("Portal_ToWestSwamp");
            GameObject portalGo = new GameObject("Portal_ToWestSwamp");
            portalGo.transform.position = new Vector3(-16.5f, 11.6f, 0f);
            BoxCollider2D pCol = portalGo.AddComponent<BoxCollider2D>();
            pCol.isTrigger = true;
            pCol.size = new Vector2(4.6f, 1.4f);

            ScenePortalTrigger trig = portalGo.AddComponent<ScenePortalTrigger>();
            var tso = new SerializedObject(trig);
            tso.FindProperty("targetSceneName").stringValue = Chapter2StateManager.WestSwampSceneName;
            tso.FindProperty("targetSpawnId").stringValue = "Spawn_SwampEntrance";
            tso.FindProperty("requiredFlag").stringValue = "";
            tso.FindProperty("useSwampUnlockGate").boolValue = true;
            tso.FindProperty("autoTriggerOnWalk").boolValue = true;
            tso.FindProperty("walkCooldown").floatValue = 0.85f;
            tso.FindProperty("promptLabel").stringValue = "Đường vào Đầm Lầy Bờ Tây";
            tso.FindProperty("lockedToast").stringValue = "Sương mù Đầm Lầy quá dày! Cần Khung Đồng Đèn Bão (Ông Kael) và Dầu Đèn Bão (Bà Mara) để mở lối.";
            tso.ApplyModifiedPropertiesWithoutUndo();

            // Place 2 guiding road lanterns framing the North-West Swamp road
            DestroyIfExists("SwampRoadGuideLanterns");
            GameObject guideRoot = new GameObject("SwampRoadGuideLanterns");
            SpawnProp(guideRoot.transform, "RoadLantern_L", sprGardenLantern, new Vector3(-18.6f, 9.2f, 0f), Vector3.one, Color.white, true);
            SpawnProp(guideRoot.transform, "RoadLantern_R", sprGardenLantern, new Vector3(-14.4f, 9.2f, 0f), Vector3.one, Color.white, true);

            // Re-align SwampMistBarrier across the North-West road at (-16.5, 7.2) & detach old ring glow
            SwampMistBarrierController barrier = Object.FindAnyObjectByType<SwampMistBarrierController>();
            if (barrier != null)
            {
                barrier.transform.position = new Vector3(-16.5f, 7.2f, 0f);
                var bso = new SerializedObject(barrier);
                bso.FindProperty("unlockedLanternGlow").objectReferenceValue = guideRoot;
                bso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(barrier);
            }

            // Update Spawn_WestSwamp right on the road south of the north exit (-16.5, 9.4)
            WorldSpawnRegistry reg = Object.FindAnyObjectByType<WorldSpawnRegistry>();
            if (reg != null)
            {
                var rso = new SerializedObject(reg);
                var list = rso.FindProperty("spawnPoints");
                bool updated = false;
                for (int i = 0; i < list.arraySize; i++)
                {
                    var el = list.GetArrayElementAtIndex(i);
                    if (el.FindPropertyRelative("id").stringValue == "Spawn_WestSwamp")
                    {
                        el.FindPropertyRelative("position").vector2Value = new Vector2(-16.5f, 9.4f);
                        el.FindPropertyRelative("displayName").stringValue = "Đường Lên Đầm Lầy Bờ Tây";
                        updated = true;
                        break;
                    }
                }
                if (!updated)
                {
                    int idx = list.arraySize;
                    list.InsertArrayElementAtIndex(idx);
                    var el = list.GetArrayElementAtIndex(idx);
                    el.FindPropertyRelative("id").stringValue = "Spawn_WestSwamp";
                    el.FindPropertyRelative("displayName").stringValue = "Đường Lên Đầm Lầy Bờ Tây";
                    el.FindPropertyRelative("position").vector2Value = new Vector2(-16.5f, 9.4f);
                }
                rso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(reg);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Ch2-R6] Đã nâng cấp đường mòn Tây-Bắc Riverbank_Chapter2 nối liền WestSwamp_Chapter2 (đi bộ tự fade mượt)!");
        }

        private static Sprite LoadFirstSprite(string primaryPath, string fallbackPath)
        {
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(primaryPath);
            if (s != null) return s;
            return AssetDatabase.LoadAssetAtPath<Sprite>(fallbackPath);
        }

        private static Tile GetOrCreateTile(string assetPath, Sprite sprite, Tile.ColliderType colType)
        {
            Tile t = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);
            if (t == null)
            {
                t = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(t, assetPath);
            }
            t.sprite = sprite;
            t.colliderType = colType;
            EditorUtility.SetDirty(t);
            return t;
        }

        private static Tilemap FindOrCreateTilemap(Transform gridParent, string name, string sortingLayer, int order, bool withCollider)
        {
            Transform existing = gridParent.Find(name);
            GameObject go = existing != null ? existing.gameObject : new GameObject(name);
            go.transform.SetParent(gridParent, false);
            Tilemap tm = go.GetComponent<Tilemap>();
            if (tm == null) tm = go.AddComponent<Tilemap>();
            TilemapRenderer tr = go.GetComponent<TilemapRenderer>();
            if (tr == null) tr = go.AddComponent<TilemapRenderer>();
            tr.sortingLayerName = sortingLayer;
            tr.sortingOrder = order;
            if (withCollider && go.GetComponent<TilemapCollider2D>() == null)
            {
                go.AddComponent<TilemapCollider2D>();
            }
            return tm;
        }

        private static Tilemap FindTilemapByName(string name)
        {
            var tms = Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include);
            for (int i = 0; i < tms.Length; i++)
                if (tms[i].gameObject.name == name) return tms[i];
            return null;
        }

        private static void SpawnProp(Transform parent, string name, Sprite spr, Vector3 pos, Vector3 scale, Color tint, bool useYSort, string layer = "WorldObjects", int order = 0)
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color = tint;
            sr.sortingLayerName = layer;
            sr.sortingOrder = useYSort ? Mathf.RoundToInt(-pos.y * 100f) : order;
            if (useYSort) go.AddComponent<YSortRenderer>();
        }

        private static void DestroyIfExists(string name)
        {
            GameObject go = GameObject.Find(name);
            if (go != null) Object.DestroyImmediate(go);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;
            string[] parts = folderPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static int CountMissingScripts(string sceneName)
        {
            int count = 0;
            var all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < all.Length; i++)
            {
                int c = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(all[i]);
                if (c > 0)
                {
                    Debug.LogError($"[MV][Ch2-R6][FAIL] {sceneName} -> '{all[i].name}' có {c} Missing Script!");
                    count += c;
                }
            }
            return count;
        }
    }
}
#endif
