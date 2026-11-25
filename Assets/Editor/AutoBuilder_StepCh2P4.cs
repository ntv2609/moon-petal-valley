using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2P4
    {
        private const string WestSwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";
        private const string Ch2TerrainRoot = "Assets/Art/Sprites/Chapter2/Terrain";
        private const string Ch2TilesRoot = "Assets/Data/Tiles/Chapter2";
        private const string Ch2SpritesRoot = "Assets/Art/Sprites/Chapter2";
        private const string Ch2OverhaulRoot = "Assets/Art/Sprites/Chapter2/Overhaul";
        private const string EnvWaterRoot = "Assets/Art/Sprites/Environment/Water";
        private const string EnvBuildingsRoot = "Assets/Art/Sprites/Environment/Buildings";
        private const string EnvUpgradedBldRoot = "Assets/Art/Sprites/Environment/Upgraded/Buildings";
        private const string EnvDecorRoot = "Assets/Art/Sprites/Environment/Decor";
        private const string EnvPropsRoot = "Assets/Art/Sprites/Environment/Props";
        private const string EnvRoot = "Assets/Art/Sprites/Environment";

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P4 - 1. Rebuild Dark Organic WestSwamp")]
        public static void ApplyStepCh2P4()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(WestSwampScenePath))
            {
                Debug.LogError("[MV][Ch2-P4] Khong tim thay scene: " + WestSwampScenePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);

            // 1. Repaint ground & border tilemaps (ZERO square water tiles! Pure mossy ground + 16-state curved road)
            RebuildSwampTilemaps();

            // 2. Rebuild organic animated ponds, dense multi-layer dark forest canopy, moss, reeds, rocks & shrine pedestal
            RebuildSwampEnvironment();

            // 3. Install persistent SwampAtmosphereController (dark damp canopy gloom + torchlight halo)
            InstallSwampAtmosphere();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Ch2-P4] Hoan tat Step Ch2-P4 (Dam lay toi am, ho bo cong pond_organic, rung ram & duong 16 trang thai).");
        }

        private static bool IsSwampPathCell(int x, int y)
        {
            // 1. Cong Nam noi 1:1 voi Riverbank_Chapter2 (rong 4 o: x = -2..1, y = -14..-2)
            if (x >= -2 && x <= 1 && y >= -14 && y <= -2) return true;
            // 2. Duong uon mem qua bai chien dau trung tam
            if (x >= -7 && x <= 2 && y >= -2 && y <= 0) return true;
            // 3. Nhanh len Mieu Den Bao Co Tay Bac (-12..-7, 4..7)
            if (x >= -10 && x <= -7 && y >= 0 && y <= 6) return true;
            if (x >= -12 && x <= -7 && y >= 4 && y <= 7) return true;
            // 4. Nhanh phu vong sang phia Dong cho chien dau rong rai
            if (x >= 1 && x <= 6 && y >= -2 && y <= 1) return true;
            if (x >= 4 && x <= 6 && y >= 1 && y <= 4) return true;
            return false;
        }

        private static void RebuildSwampTilemaps()
        {
            Tilemap groundTm = FindTilemapByName("GroundTilemap");
            Tilemap obsTm = FindTilemapByName("ObstacleTilemap");
            if (groundTm == null)
            {
                Debug.LogError("[MV][Ch2-P4] Khong tim thay GroundTilemap trong WestSwamp_Chapter2!");
                return;
            }

            if (!Directory.Exists(Ch2TilesRoot))
                Directory.CreateDirectory(Ch2TilesRoot);

            Tile[] westGrass = new Tile[4];
            for (int i = 0; i < 4; i++)
                westGrass[i] = GetOrCreateTile($"{Ch2TilesRoot}/Ch2_GrassWest_{i}.asset", $"{Ch2TerrainRoot}/ch2_grass_west_{i}.png", Tile.ColliderType.None);

            Tile[] westPath = new Tile[16];
            for (int m = 0; m < 16; m++)
                westPath[m] = GetOrCreateTile($"{Ch2TilesRoot}/Ch2_PathWest_m{m:D2}.asset", $"{Ch2TerrainRoot}/ch2_path_west_m{m:D2}.png", Tile.ColliderType.None);

            Tile tBlock = GetOrCreateTile($"{Ch2TilesRoot}/WS_BorderBlock.asset", EnvWaterRoot + "/tile_water_transparent.png", Tile.ColliderType.Grid);

            groundTm.ClearAllTiles();
            if (obsTm != null) obsTm.ClearAllTiles();

            // Nhuom nen dat dam lay toi va am hon ro ret so voi ben ngoai
            groundTm.color = new Color(0.58f, 0.68f, 0.62f, 1f);

            for (int y = -14; y <= 13; y++)
            {
                for (int x = -22; x <= 21; x++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    bool isSouthRoadGap = (x >= -2 && x <= 1 && y <= -8);
                    bool isOuterBorder = (x <= -19 || x >= 18 || y >= 10 || (y <= -11 && !isSouthRoadGap));

                    if (IsSwampPathCell(x, y))
                    {
                        int mask = 0;
                        if (IsSwampPathCell(x, y + 1)) mask |= 1;
                        if (IsSwampPathCell(x + 1, y)) mask |= 2;
                        if (y == -14 && x >= -2 && x <= 1) mask |= 4; // noi lien xuong Riverbank
                        else if (IsSwampPathCell(x, y - 1)) mask |= 4;
                        if (IsSwampPathCell(x - 1, y)) mask |= 8;

                        groundTm.SetTile(cell, westPath[mask] != null ? westPath[mask] : westGrass[0]);
                    }
                    else
                    {
                        int hash = Mathf.Abs(x * 73856093 ^ y * 19349663) % 4;
                        groundTm.SetTile(cell, westGrass[hash] != null ? westGrass[hash] : westGrass[0]);
                    }

                    if (obsTm != null && isOuterBorder && tBlock != null)
                        obsTm.SetTile(cell, tBlock);
                }
            }

            EditorUtility.SetDirty(groundTm);
            if (obsTm != null) EditorUtility.SetDirty(obsTm);
        }

        private static void RebuildSwampEnvironment()
        {
            DestroyIfExists("SwampEnvironmentDecor");
            DestroyIfExists("SwampAmbientMist");
            DestroyIfExists("SwampOrganicPonds");

            // Load existing sprites from Sprites.zip paths
            Sprite sprPine = LoadFirstSprite(EnvUpgradedBldRoot + "/tree_hero_pine.png", EnvBuildingsRoot + "/tree_hero_pine.png");
            Sprite sprOak = LoadFirstSprite(EnvUpgradedBldRoot + "/tree_hero_oak.png", EnvBuildingsRoot + "/tree_hero_oak.png");
            Sprite sprBush = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_bush.png");
            Sprite sprReeds = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_reeds.png");
            Sprite sprMush = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_mushrooms.png");
            Sprite sprRock = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_rock.png");
            Sprite sprTall0 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_tall_grass_0.png");
            Sprite sprTall1 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_tall_grass_1.png");
            Sprite sprStep0 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_stepping_0.png");
            Sprite sprStep1 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_stepping_1.png");
            Sprite sprLantern = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_garden_lantern.png");
            Sprite sprBranch = AssetDatabase.LoadAssetAtPath<Sprite>(EnvPropsRoot + "/prop_dry_branch.png");
            Sprite sprShadow = AssetDatabase.LoadAssetAtPath<Sprite>(EnvPropsRoot + "/spr_ground_shadow.png");
            Sprite sprLily0 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_lilypad_0.png");
            Sprite sprLily1 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_lilypad_1.png");
            Sprite sprRivRock = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_river_rock_0.png");
            Sprite sprMist = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2TerrainRoot + "/ch2_river_mist_strip.png");
            Sprite sprBlightFog = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "/spr_blight_fog.png");
            Sprite sprPedestal = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2SpritesRoot + "/prop_swamp_shrine_pedestal.png");

            Sprite[] pondFrames = new Sprite[4];
            for (int i = 0; i < 4; i++)
                pondFrames[i] = AssetDatabase.LoadAssetAtPath<Sprite>($"{EnvWaterRoot}/pond_organic_0{i}.png");

            // --- 1. 3 Organic Curved Animated Ponds (ZERO square pools!) ---
            GameObject pondRoot = new GameObject("SwampOrganicPonds");
            CreateOrganicPond(pondRoot.transform, "Pond_EastBog", pondFrames, new Vector3(12.6f, 2.8f, 0f), 1.38f, sprLily0, sprLily1, sprReeds, sprRivRock);
            CreateOrganicPond(pondRoot.transform, "Pond_SouthWestBog", pondFrames, new Vector3(-13.2f, -5.6f, 0f), 1.25f, sprLily1, sprLily0, sprReeds, sprRivRock);
            CreateOrganicPond(pondRoot.transform, "Pond_SouthEastPool", pondFrames, new Vector3(10.8f, -6.2f, 0f), 1.05f, sprLily0, sprLily1, sprReeds, sprRivRock);

            // --- 2. Dense Multi-Layer Dark Forest Canopy & Damp Ground Props ---
            GameObject envRoot = new GameObject("SwampEnvironmentDecor");
            Color darkPineA = new Color(0.42f, 0.54f, 0.48f, 1f);
            Color darkPineB = new Color(0.48f, 0.60f, 0.52f, 1f);
            Color darkOak = new Color(0.45f, 0.56f, 0.46f, 1f);

            // Double-staggered dense perimeter canopy (top, bottom except South road x=-2..1, left, right)
            for (int x = -21; x <= 20; x += 2)
            {
                bool even = (x % 4 == 0);
                SpawnProp(envRoot.transform, $"CanopyTopA_{x}", even ? sprPine : sprOak, new Vector3(x + 0.3f, 11.4f + (even ? 0.4f : -0.3f), 0f),
                    Vector3.one * 1.18f, even ? darkPineA : darkOak, true, false, Vector2.zero, Vector2.zero, sprShadow, 1.4f);
                SpawnProp(envRoot.transform, $"CanopyTopB_{x}", sprPine, new Vector3(x - 0.5f, 9.8f + (even ? -0.2f : 0.3f), 0f),
                    Vector3.one * 1.08f, darkPineB, true, false, Vector2.zero, Vector2.zero, sprShadow, 1.2f);

                if (x < -4 || x > 3)
                {
                    SpawnProp(envRoot.transform, $"CanopyBotA_{x}", even ? sprOak : sprPine, new Vector3(x + 0.2f, -11.4f + (even ? 0.3f : -0.3f), 0f),
                        Vector3.one * 1.15f, even ? darkOak : darkPineA, true, false, Vector2.zero, Vector2.zero, sprShadow, 1.3f);
                }
            }
            for (int y = -10; y <= 9; y += 2)
            {
                bool even = (y % 4 == 0);
                SpawnProp(envRoot.transform, $"CanopyLeftA_{y}", even ? sprPine : sprOak, new Vector3(-20.2f + (even ? 0.5f : -0.4f), y, 0f),
                    Vector3.one * 1.18f, even ? darkPineA : darkOak, true, false, Vector2.zero, Vector2.zero, sprShadow, 1.35f);
                SpawnProp(envRoot.transform, $"CanopyLeftB_{y}", sprPine, new Vector3(-18.6f + (even ? -0.3f : 0.4f), y + 0.7f, 0f),
                    Vector3.one * 1.06f, darkPineB, true, false, Vector2.zero, Vector2.zero, sprShadow, 1.2f);

                SpawnProp(envRoot.transform, $"CanopyRightA_{y}", even ? sprOak : sprPine, new Vector3(19.2f + (even ? -0.4f : 0.5f), y, 0f),
                    Vector3.one * 1.18f, even ? darkOak : darkPineA, true, false, Vector2.zero, Vector2.zero, sprShadow, 1.35f);
                SpawnProp(envRoot.transform, $"CanopyRightB_{y}", sprPine, new Vector3(17.6f + (even ? 0.3f : -0.4f), y + 0.6f, 0f),
                    Vector3.one * 1.06f, darkPineB, true, false, Vector2.zero, Vector2.zero, sprShadow, 1.2f);
            }

            // Interior ancient trees (small trunk collider so Wen can fight & weave underneath dense canopies!)
            Vector2[] interiorTrees =
            {
                new Vector2(-15.5f, 3.2f), new Vector2(-15.8f, 7.6f), new Vector2(-4.2f, 7.2f),
                new Vector2(3.8f, 7.6f), new Vector2(10.2f, 7.4f), new Vector2(15.2f, 7.2f),
                new Vector2(-6.5f, -7.8f), new Vector2(5.2f, -7.6f), new Vector2(-15.2f, -1.2f),
                new Vector2(15.6f, -2.4f), new Vector2(2.6f, 5.2f),
            };
            for (int i = 0; i < interiorTrees.Length; i++)
            {
                Sprite tSpr = (i % 2 == 0) ? sprPine : sprOak;
                SpawnProp(envRoot.transform, $"InteriorSwampTree_{i}", tSpr, interiorTrees[i], Vector3.one * 1.10f,
                    i % 2 == 0 ? darkPineB : darkOak, true, true, new Vector2(0.85f, 0.55f), new Vector2(0f, -1.2f), sprShadow, 1.35f);
            }

            // South Entrance Guide Lanterns (matching Riverbank_Chapter2 North exit!)
            SpawnProp(envRoot.transform, "SouthRoadLantern_L", sprLantern, new Vector3(-2.6f, -9.8f, 0f), Vector3.one, Color.white, true, true, new Vector2(0.42f, 0.32f), new Vector2(0f, -0.25f), null, 0f);
            SpawnProp(envRoot.transform, "SouthRoadLantern_R", sprLantern, new Vector3(2.4f, -9.8f, 0f), Vector3.one, Color.white, true, true, new Vector2(0.42f, 0.32f), new Vector2(0f, -0.25f), null, 0f);

            // Damp swamp floor details: mushrooms, tall grass, bushes, dry branches, mossy rocks, stepping stones
            Color dampTint = new Color(0.68f, 0.78f, 0.72f, 1f);
            Vector2[] bushSpots = { new Vector2(-5.4f, 2.6f), new Vector2(7.6f, 4.4f), new Vector2(-11.4f, 0.8f), new Vector2(6.8f, -3.4f), new Vector2(-4.8f, -5.8f), new Vector2(14.8f, -4.2f) };
            for (int i = 0; i < bushSpots.Length; i++)
                SpawnProp(envRoot.transform, $"SwampBush_{i}", sprBush, bushSpots[i], Vector3.one, dampTint, true, true, new Vector2(0.75f, 0.45f), new Vector2(0f, -0.15f), null, 0f);

            Vector2[] mushSpots = { new Vector2(-3.6f, -4.2f), new Vector2(4.2f, -4.6f), new Vector2(-12.2f, 3.6f), new Vector2(8.4f, 5.8f), new Vector2(-7.8f, -3.2f), new Vector2(1.8f, 3.8f) };
            for (int i = 0; i < mushSpots.Length; i++)
                SpawnProp(envRoot.transform, $"SwampMush_{i}", sprMush, mushSpots[i], Vector3.one, new Color(0.84f, 0.94f, 0.88f, 1f), false, false, Vector2.zero, Vector2.zero, null, 0f, "GroundDecor", 6);

            Vector2[] grassSpots = { new Vector2(-6.2f, 5.4f), new Vector2(0.4f, 4.6f), new Vector2(7.8f, 1.2f), new Vector2(-3.8f, -7.2f), new Vector2(4.6f, -5.8f), new Vector2(-11.2f, -2.4f), new Vector2(8.6f, -1.8f) };
            for (int i = 0; i < grassSpots.Length; i++)
                SpawnProp(envRoot.transform, $"TallGrass_{i}", i % 2 == 0 ? sprTall0 : sprTall1, grassSpots[i], Vector3.one, dampTint, false, false, Vector2.zero, Vector2.zero, null, 0f, "GroundDecor", 5);

            Vector2[] stepSpots = { new Vector2(7.2f, 3.6f), new Vector2(8.0f, 2.6f), new Vector2(8.6f, 1.4f), new Vector2(8.2f, 0.2f) };
            for (int i = 0; i < stepSpots.Length; i++)
                SpawnProp(envRoot.transform, $"SwampStep_{i}", i % 2 == 0 ? sprStep0 : sprStep1, stepSpots[i], Vector3.one, dampTint, false, false, Vector2.zero, Vector2.zero, null, 0f, "GroundDecor", 4);

            SpawnProp(envRoot.transform, "SwampBranch_1", sprBranch, new Vector3(5.6f, -2.4f, 0f), Vector3.one, dampTint, false, false, Vector2.zero, Vector2.zero, null, 0f, "GroundDecor", 5);
            SpawnProp(envRoot.transform, "SwampBranch_2", sprBranch, new Vector3(-6.4f, -3.6f, 0f), Vector3.one, dampTint, false, false, Vector2.zero, Vector2.zero, null, 0f, "GroundDecor", 5);
            SpawnProp(envRoot.transform, "SwampRock_1", sprRock, new Vector3(-13.8f, 6.8f, 0f), Vector3.one, dampTint, true, true, new Vector2(0.75f, 0.45f), new Vector2(0f, -0.12f), null, 0f);
            SpawnProp(envRoot.transform, "SwampRock_2", sprRock, new Vector3(7.4f, 7.2f, 0f), Vector3.one, dampTint, true, true, new Vector2(0.75f, 0.45f), new Vector2(0f, -0.12f), null, 0f);

            // --- 3. Upgrade AncientLanternShrine with our new Mossy Stone Pedestal ---
            GameObject shrine = GameObject.Find("AncientLanternShrine");
            if (shrine != null && sprPedestal != null)
            {
                Transform oldPed = shrine.transform.Find("ShrineStonePedestal");
                if (oldPed != null) UnityEngine.Object.DestroyImmediate(oldPed.gameObject);
                GameObject ped = new GameObject("ShrineStonePedestal");
                ped.transform.SetParent(shrine.transform, false);
                ped.transform.localPosition = new Vector3(0f, -0.25f, 0f);
                var sr = ped.AddComponent<SpriteRenderer>();
                sr.sprite = sprPedestal;
                sr.sortingLayerName = "GroundDecor";
                sr.sortingOrder = 14;
            }

            // --- 4. Drifting Swamp Mist & Low Ground Fog ---
            GameObject mistRoot = new GameObject("SwampAmbientMist");
            if (sprMist != null)
            {
                float[] mistYs = { -7.5f, -3.0f, 1.5f, 5.5f, 8.8f };
                for (int i = 0; i < mistYs.Length; i++)
                {
                    GameObject mGo = new GameObject($"SwampMist_{i}");
                    mGo.transform.SetParent(mistRoot.transform, false);
                    mGo.transform.position = new Vector3((i % 2 == 0 ? -4f : 4f), mistYs[i], 0f);
                    mGo.transform.localScale = new Vector3(3.4f, 1.4f, 1f);
                    var sr = mGo.AddComponent<SpriteRenderer>();
                    sr.sprite = sprMist;
                    sr.color = new Color(0.45f, 0.65f, 0.68f, 0.26f);
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = 250;
                    mGo.AddComponent<RiverMistDrifter>();
                }
            }
            if (sprBlightFog != null)
            {
                Vector2[] fogSpots = { new Vector2(-10f, -2f), new Vector2(10f, 1f), new Vector2(-2f, 5f), new Vector2(4f, -6f) };
                for (int i = 0; i < fogSpots.Length; i++)
                {
                    GameObject fGo = new GameObject($"SwampFogPatch_{i}");
                    fGo.transform.SetParent(mistRoot.transform, false);
                    fGo.transform.position = new Vector3(fogSpots[i].x, fogSpots[i].y, 0f);
                    fGo.transform.localScale = new Vector3(4.5f, 3.2f, 1f);
                    var sr = fGo.AddComponent<SpriteRenderer>();
                    sr.sprite = sprBlightFog;
                    sr.color = new Color(0.24f, 0.42f, 0.44f, 0.20f);
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = 245;
                }
            }
        }

        private static void CreateOrganicPond(Transform parent, string name, Sprite[] frames, Vector3 pos, float scale,
            Sprite lilyA, Sprite lilyB, Sprite reeds, Sprite rock)
        {
            if (frames == null || frames[0] == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * scale;
            go.layer = 10; // Obstacle

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = frames[0];
            sr.color = new Color(0.46f, 0.64f, 0.58f, 0.96f); // dark murky swamp water tint
            sr.sortingLayerName = "GroundDecor";
            sr.sortingOrder = 3;

            var anim = go.AddComponent<OrganicPondAnimator>();
            SerializedObject aso = new SerializedObject(anim);
            var fProp = aso.FindProperty("frames");
            fProp.ClearArray();
            for (int i = 0; i < frames.Length; i++)
            {
                fProp.InsertArrayElementAtIndex(i);
                fProp.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            }
            aso.FindProperty("fps").floatValue = 3.5f;
            aso.ApplyModifiedPropertiesWithoutUndo();

            // Smooth rounded capsule collider matching curved organic pond shape
            var cap = go.AddComponent<CapsuleCollider2D>();
            cap.direction = CapsuleDirection2D.Horizontal;
            cap.size = new Vector2(4.6f, 2.7f);
            cap.offset = Vector2.zero;

            // Pond surface lilypads & shoreline reeds/rocks
            if (lilyA != null) CreateChildDecor(go.transform, "LilyA", lilyA, new Vector3(-0.9f, 0.35f, 0f), 6);
            if (lilyB != null) CreateChildDecor(go.transform, "LilyB", lilyB, new Vector3(1.1f, -0.4f, 0f), 6);
            if (reeds != null)
            {
                CreateChildDecor(go.transform, "ReedsL", reeds, new Vector3(-2.3f, -0.8f, 0f), 8);
                CreateChildDecor(go.transform, "ReedsR", reeds, new Vector3(2.2f, 0.9f, 0f), 8);
            }
            if (rock != null) CreateChildDecor(go.transform, "BankRock", rock, new Vector3(-1.8f, 1.1f, 0f), 7);
        }

        private static void CreateChildDecor(Transform parent, string name, Sprite spr, Vector3 localPos, int order)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(parent, false);
            c.transform.localPosition = localPos;
            var sr = c.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color = new Color(0.76f, 0.86f, 0.80f, 1f);
            sr.sortingLayerName = "GroundDecor";
            sr.sortingOrder = order;
        }

        private static void InstallSwampAtmosphere()
        {
            DestroyIfExists("SwampAtmosphereRig");
            GameObject rig = new GameObject("SwampAtmosphereRig");
            rig.AddComponent<SwampAtmosphereController>();
            EditorUtility.SetDirty(rig);
        }

        private static void SpawnProp(Transform parent, string name, Sprite spr, Vector3 pos, Vector3 scale, Color tint,
            bool useYSort, bool hasCol, Vector2 colSize, Vector2 colOffset, Sprite shadowSpr, float shadowScale,
            string layer = "WorldObjects", int fixedOrder = 0)
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color = tint;
            sr.sortingLayerName = layer;
            sr.sortingOrder = useYSort ? Mathf.RoundToInt(-pos.y * 100f) : fixedOrder;

            if (hasCol)
            {
                go.layer = 10;
                var col = go.AddComponent<BoxCollider2D>();
                col.size = colSize;
                col.offset = colOffset;
            }
            if (shadowSpr != null && shadowScale > 0f)
            {
                GameObject sh = new GameObject("Shadow");
                sh.transform.SetParent(go.transform, false);
                sh.transform.localPosition = new Vector3(0f, colOffset.y - 0.1f, 0f);
                sh.transform.localScale = Vector3.one * shadowScale;
                var ssr = sh.AddComponent<SpriteRenderer>();
                ssr.sprite = shadowSpr;
                ssr.color = new Color(0.05f, 0.10f, 0.10f, 0.45f);
                ssr.sortingLayerName = "GroundDecor";
                ssr.sortingOrder = 2;
            }
        }

        private static Sprite LoadFirstSprite(string pathA, string pathB)
        {
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(pathA);
            return s != null ? s : AssetDatabase.LoadAssetAtPath<Sprite>(pathB);
        }

        private static Tilemap FindTilemapByName(string name)
        {
            GameObject go = GameObject.Find(name);
            return go != null ? go.GetComponent<Tilemap>() : null;
        }

        private static Tile GetOrCreateTile(string assetPath, string spritePath, Tile.ColliderType colType)
        {
            Tile t = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);
            Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (t == null)
            {
                t = ScriptableObject.CreateInstance<Tile>();
                t.sprite = spr;
                t.colliderType = colType;
                AssetDatabase.CreateAsset(t, assetPath);
            }
            else
            {
                if (spr != null) t.sprite = spr;
                t.colliderType = colType;
                EditorUtility.SetDirty(t);
            }
            return t;
        }

        private static void DestroyIfExists(string name)
        {
            GameObject go = GameObject.Find(name);
            if (go != null) UnityEngine.Object.DestroyImmediate(go);
        }

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P4 - 2. Validate Step Ch2-P4 & Full 5-Scene Gate QA")]
        public static void ValidateStepCh2P4AndRunGateQA()
        {
            int errors = 0;
            if (!File.Exists(WestSwampScenePath))
            {
                Debug.LogError("[MV][Ch2-P4][FAIL] Khong tim thay " + WestSwampScenePath);
                return;
            }

            EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);

            var ponds = UnityEngine.Object.FindObjectsByType<OrganicPondAnimator>(FindObjectsInactive.Include);
            if (ponds.Length < 3)
            {
                Debug.LogError($"[MV][Ch2-P4][FAIL] Can it nhat 3 ho bo cong OrganicPondAnimator, tim thay {ponds.Length}!");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<SwampAtmosphereController>() == null)
            {
                Debug.LogError("[MV][Ch2-P4][FAIL] Thieu SwampAtmosphereController (lop phu toi am dam lay)!");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<SwampWaveCombatController>() == null)
            {
                Debug.LogError("[MV][Ch2-P4][FAIL] Thieu SwampWaveCombatController!");
                errors++;
            }

            if (GameObject.Find("Portal_ToRiverbankWest") == null)
            {
                Debug.LogError("[MV][Ch2-P4][FAIL] Thieu Portal_ToRiverbankWest!");
                errors++;
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-P4] VALIDATE PASS (0 loi)! Dam lay toi am & ho bo cong huu co dat chuan. Dang chay Full 5-Scene Gate QA...");
            else
                Debug.LogError($"[MV][Ch2-P4] VALIDATE FAIL ({errors} loi).");

            AutoBuilder_StepCh2R5.RunChapter2OverhaulGateQA();
        }
    }
}
