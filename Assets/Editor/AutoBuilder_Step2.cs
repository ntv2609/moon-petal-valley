using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step2
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";

        [MenuItem("Moonpetal/Step 2/1. Build Player, Lumi & Map")]
        public static void BuildStep2()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 2 ===");

            Sprite wenSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_wen_idle.png", 16, 24, PlaceholderGeneratorUtility.DrawWenSprite);
            Sprite lumiSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_lumi_idle.png", 12, 12, PlaceholderGeneratorUtility.DrawLumiSprite);
            Sprite haloSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_lumi_halo.png", 32, 32, PlaceholderGeneratorUtility.DrawRadialHalo);

            Sprite grassSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_grass.png", 16, 16, PlaceholderGeneratorUtility.DrawGrassTile);
            Sprite pathSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_path.png", 16, 16, PlaceholderGeneratorUtility.DrawPathTile);
            Sprite ghFloorSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_greenhouse_floor.png", 16, 16, PlaceholderGeneratorUtility.DrawGreenhouseFloorTile);
            Sprite wallSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_wall.png", 16, 16, PlaceholderGeneratorUtility.DrawWallTile);
            Sprite fenceSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_fence.png", 16, 16, PlaceholderGeneratorUtility.DrawFenceTile);
            Sprite treeSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_tree.png", 16, 16, PlaceholderGeneratorUtility.DrawTreeTile);
            Sprite waterSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_water.png", 16, 16, PlaceholderGeneratorUtility.DrawWaterTile);
            Sprite flowerbedSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_flowerbed.png", 16, 16, PlaceholderGeneratorUtility.DrawFlowerbedTile);

            MapLayoutConfigSO mapConfig = EnsureMapLayoutConfig();

            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            GameObject playerPrefab = BuildPlayerPrefab(wenSpr, statsConfig);
            GameObject lumiPrefab = BuildLumiPrefab(lumiSpr, haloSpr);

            UpgradeGameRootDebugWithTeleports();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            GameObject oldBanner = GameObject.Find("Step1VerificationCanvas");
            if (oldBanner != null) UnityEngine.Object.DestroyImmediate(oldBanner);

            BuildWorldTilemap(mapConfig, grassSpr, pathSpr, ghFloorSpr, wallSpr, fenceSpr, treeSpr, waterSpr, flowerbedSpr);
            BuildSpawnRegistry(mapConfig);

            GameObject playerInst = EnsurePrefabInScene("Player_Wen", playerPrefab, new Vector3(-14.5f, 9.5f, 0f));
            GameObject lumiInst = EnsurePrefabInScene("Lumi_Companion", lumiPrefab, new Vector3(-15.2f, 10.2f, 0f));

            var lumiCtrl = lumiInst.GetComponent<LumiCompanionController>();
            var lumiSO = new SerializedObject(lumiCtrl);
            lumiSO.FindProperty("followTarget").objectReferenceValue = playerInst.transform;
            lumiSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject camGo = GameObject.Find("Main Camera");
            if (camGo != null)
            {
                var camFollow = camGo.GetComponent<CameraFollow2D>();
                if (camFollow == null) camFollow = camGo.AddComponent<CameraFollow2D>();
                var camSO = new SerializedObject(camFollow);
                camSO.FindProperty("target").objectReferenceValue = playerInst.transform;
                camSO.FindProperty("smoothTime").floatValue = 0.16f;
                camSO.FindProperty("minBounds").vector2Value = mapConfig.cameraMinBounds;
                camSO.FindProperty("maxBounds").vector2Value = mapConfig.cameraMaxBounds;
                camSO.ApplyModifiedPropertiesWithoutUndo();
                camGo.transform.position = new Vector3(-14.5f, 9.5f, -10f);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 2! Chạy 'Moonpetal/Step 2/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 2/Validate")]
        public static void ValidateStep2()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 2 ===");

            if (AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath) == null)
            {
                Debug.LogError("[MV] THIẾU MapLayoutConfig.asset");
                errors++;
            }
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) == null)
            {
                Debug.LogError("[MV] THIẾU Player_Wen.prefab");
                errors++;
            }
            if (AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath) == null)
            {
                Debug.LogError("[MV] THIẾU Lumi_Companion.prefab");
                errors++;
            }

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player == null)
            {
                Debug.LogError("[MV] Không tìm thấy PlayerController2D trong scene hiện tại.");
                errors++;
            }
            else
            {
                if (player.gameObject.layer != 8)
                {
                    Debug.LogError("[MV] Player_Wen phải nằm ở Layer 8 (Player).");
                    errors++;
                }
                var so = new SerializedObject(player);
                string[] req = { "statsConfig", "rb", "inputReader", "spriteRenderer", "visualTransform" };
                foreach (string f in req)
                {
                    if (so.FindProperty(f).objectReferenceValue == null)
                    {
                        Debug.LogError("[MV] PlayerController2D thiếu trường: " + f);
                        errors++;
                    }
                }
            }

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi == null)
            {
                Debug.LogError("[MV] Không tìm thấy LumiCompanionController trong scene.");
                errors++;
            }

            var cam = UnityEngine.Object.FindAnyObjectByType<CameraFollow2D>();
            if (cam == null)
            {
                Debug.LogError("[MV] Không tìm thấy CameraFollow2D trên Main Camera.");
                errors++;
            }

            var spawnReg = UnityEngine.Object.FindAnyObjectByType<WorldSpawnRegistry>();
            if (spawnReg == null)
            {
                Debug.LogError("[MV] Không tìm thấy WorldSpawnRegistry trong scene.");
                errors++;
            }

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
                Debug.Log("[MV] VALIDATE STEP 2: PASS (0 lỗi). Nhân vật Wen, Lumi, Camera và Bản đồ 4 phân khu sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 2: FAIL (" + errors + " lỗi).");
            }
        }

        private static MapLayoutConfigSO EnsureMapLayoutConfig()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null)
            {
                cfg = ScriptableObject.CreateInstance<MapLayoutConfigSO>();
                AssetDatabase.CreateAsset(cfg, MapConfigPath);
            }

            cfg.originX = -22;
            cfg.originY = -16;
            cfg.cameraMinBounds = new Vector2(-13.5f, -10.5f);
            cfg.cameraMaxBounds = new Vector2(13.5f, 10.5f);

            string[] rows =
            {
                "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT",
                "T############................TTTTTTTTTTTTTTT",
                "T#GGGGGGGGGG#...W............TT..........TTT",
                "T#GGGGGGGGGG#..WWW...........T............TT",
                "T#GGGGGGGGGG#...W....PPPP....T....TTTT.....T",
                "T#GGGGGGGGGG#........PPPP.........TTTT.....T",
                "T#GGGGGGGGGG#........PPPP..................T",
                "T#GGGGGGGGGG#...PPPPPPPPPPPPPPPPPPPPPP.....T",
                "T#GGGGGGGGGG#...PPPPPPPPPPPPPPPPPPPPPP.....T",
                "T#GGGGGGGGGG#...PP........PP........PP.....T",
                "T#####PP#####...PP........PP........PP.....T",
                "T.....PP........PP..PPPP..PP........PP.....T",
                "T.....PP........PP..PPPP..PP........PP.....T",
                "T.....PPPPPPPPPPPP..PPPP..PPPPPPPPPPPP.....T",
                "T.....PPPPPPPPPPPP..PPPP..PPPPPPPPPPPP.....T",
                "T...................PPPP...................T",
                "T...................PPPP...................T",
                "T..............FFFFFFFFFFFFFF..............T",
                "T..............F....PPPP....F..............T",
                "T..............F....PPPP....F..............T",
                "T..............F....PPPP....F..............T",
                "T..............F...PPPPPP...F..............T",
                "T..............F...PPBBPP...F..............T",
                "T..............F...PPBBPP...F..............T",
                "T..............F...PPPPPP...F..............T",
                "T..............F............F..............T",
                "T..............F............F..............T",
                "T..............F............F..............T",
                "T..............FFFFFFFFFFFFFF..............T",
                "TT........................................TT",
                "TTT......................................TTT",
                "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT"
            };
            cfg.asciiMap = string.Join("\n", rows);

            cfg.spawnPoints = new List<SpawnPointDefinition>
            {
                new SpawnPointDefinition { id = "Spawn_Greenhouse", displayName = "Nhà Kính Của Bà", position = new Vector2(-15.0f, 10.0f) },
                new SpawnPointDefinition { id = "Spawn_Village", displayName = "Quảng Trường Làng Chính", position = new Vector2(0.0f, 3.0f) },
                new SpawnPointDefinition { id = "Spawn_BachGarden", displayName = "Vườn Phía Nam Ông Bách", position = new Vector2(0.0f, -5.0f) },
                new SpawnPointDefinition { id = "Spawn_ForestEdge", displayName = "Rìa Rừng Phía Đông", position = new Vector2(15.0f, 7.0f) }
            };

            EditorUtility.SetDirty(cfg);
            return cfg;
        }

        private static GameObject BuildPlayerPrefab(Sprite wenSpr, PlayerStatsSO statsConfig)
        {
            GameObject root = new GameObject("Player_Wen");
            root.tag = "Player";
            root.layer = 8;

            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = root.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.65f, 0.55f);
            col.offset = new Vector2(0f, -0.35f);

            var input = root.AddComponent<GameInputReader>();
            var ctrl = root.AddComponent<PlayerController2D>();

            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);
            var sr = visual.AddComponent<SpriteRenderer>();
            sr.sprite = wenSpr;
            sr.sortingLayerName = "WorldObjects";

            var so = new SerializedObject(ctrl);
            so.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            so.FindProperty("rb").objectReferenceValue = rb;
            so.FindProperty("inputReader").objectReferenceValue = input;
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.FindProperty("visualTransform").objectReferenceValue = visual.transform;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static GameObject BuildLumiPrefab(Sprite lumiSpr, Sprite haloSpr)
        {
            GameObject root = new GameObject("Lumi_Companion");
            var ctrl = root.AddComponent<LumiCompanionController>();

            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);
            var sr = visual.AddComponent<SpriteRenderer>();
            sr.sprite = lumiSpr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = 5000;

            GameObject halo = new GameObject("Halo");
            halo.transform.SetParent(visual.transform, false);
            var haloSr = halo.AddComponent<SpriteRenderer>();
            haloSr.sprite = haloSpr;
            haloSr.sortingLayerName = "WeatherVFX";
            haloSr.sortingOrder = 10;

            var so = new SerializedObject(ctrl);
            so.FindProperty("visualTransform").objectReferenceValue = visual.transform;
            so.FindProperty("haloRenderer").objectReferenceValue = haloSr;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, LumiPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static void BuildWorldTilemap(
            MapLayoutConfigSO cfg,
            Sprite grassSpr, Sprite pathSpr, Sprite ghFloorSpr,
            Sprite wallSpr, Sprite fenceSpr, Sprite treeSpr,
            Sprite waterSpr, Sprite flowerbedSpr)
        {
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

            Tile tGrass = CreateTile(grassSpr);
            Tile tPath = CreateTile(pathSpr);
            Tile tGhFloor = CreateTile(ghFloorSpr);
            Tile tWall = CreateTile(wallSpr);
            Tile tFence = CreateTile(fenceSpr);
            Tile tTree = CreateTile(treeSpr);
            Tile tWater = CreateTile(waterSpr);
            Tile tFlowerbed = CreateTile(flowerbedSpr);

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

                    if (ch == 'G') groundTm.SetTile(cell, tGhFloor);
                    else if (ch == 'P') groundTm.SetTile(cell, tPath);
                    else if (ch == 'B') groundTm.SetTile(cell, tFlowerbed);
                    else groundTm.SetTile(cell, tGrass);

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

        private static void BuildSpawnRegistry(MapLayoutConfigSO cfg)
        {
            GameObject existing = GameObject.Find("WorldSpawnRegistry");
            if (existing == null) existing = new GameObject("WorldSpawnRegistry");

            var reg = existing.GetComponent<WorldSpawnRegistry>();
            if (reg == null) reg = existing.AddComponent<WorldSpawnRegistry>();

            var so = new SerializedObject(reg);
            var listProp = so.FindProperty("spawnPoints");
            listProp.ClearArray();
            for (int i = 0; i < cfg.spawnPoints.Count; i++)
            {
                listProp.InsertArrayElementAtIndex(i);
                var elem = listProp.GetArrayElementAtIndex(i);
                elem.FindPropertyRelative("id").stringValue = cfg.spawnPoints[i].id;
                elem.FindPropertyRelative("displayName").stringValue = cfg.spawnPoints[i].displayName;
                elem.FindPropertyRelative("position").vector2Value = cfg.spawnPoints[i].position;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject EnsurePrefabInScene(string objName, GameObject prefab, Vector3 pos)
        {
            GameObject existing = GameObject.Find(objName);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing);

            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            inst.name = objName;
            inst.transform.position = pos;
            return inst;
        }

        private static void UpgradeGameRootDebugWithTeleports()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                var rect = debugPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(460f, 510f);

                Button btnGh = EnsureDebugButton(debugPanel, "BtnTpGreenhouse", "TP: Nhà Kính Của Bà", vnFont);
                Button btnVil = EnsureDebugButton(debugPanel, "BtnTpVillage", "TP: Quảng Trường Làng Chính", vnFont);
                Button btnGar = EnsureDebugButton(debugPanel, "BtnTpGarden", "TP: Vườn Phía Nam Ông Bách", vnFont);
                Button btnFor = EnsureDebugButton(debugPanel, "BtnTpForest", "TP: Rìa Rừng Phía Đông", vnFont);

                var so = new SerializedObject(debugCtrl);
                so.FindProperty("btnTpGreenhouse").objectReferenceValue = btnGh;
                so.FindProperty("btnTpVillage").objectReferenceValue = btnVil;
                so.FindProperty("btnTpGarden").objectReferenceValue = btnGar;
                so.FindProperty("btnTpForest").objectReferenceValue = btnFor;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã nâng cấp GameRoot.prefab với 4 nút Teleport Debug.");
        }

        private static Button EnsureDebugButton(Transform parent, string name, string label, TMP_FontAsset font)
        {
            Transform existing = parent.Find(name);
            if (existing != null) return existing.GetComponent<Button>();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 36f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.18f, 0.44f, 0.38f, 1f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            GameObject txtGo = new GameObject("Label");
            txtGo.transform.SetParent(go.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            if (font != null) tmp.font = font;

            return btn;
        }
    }
}
