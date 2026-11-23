using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step42
    {
        private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string MvpScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";
        private const string Ch2ArtRoot = "Assets/Art/Sprites/Chapter2/";
        private const string EnvArtRoot = "Assets/Art/Sprites/Environment/";

        [MenuItem("Moonpetal/Step 42/1. Build Riverbank Scene, Portal (CR-01) & Bridge")]
        public static void BuildStep42()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 42 (RIVERBANK CHAPTER 2 + CR-01 PORTAL) ===");
            AssetDatabase.Refresh();
            ReimportChapter2EnvSprites();

            // 1. CR-01: Đặt 1 cổng chuyển cảnh sang Chương 2 tại rìa Tây-Nam MoonpetalValley_MVP.unity
            InstallPortalInMvpScene();

            // 2. Dựng scene mới Riverbank_Chapter2.unity
            BuildRiverbankScene();

            // 3. Cập nhật Build Settings (0: Bootstrap, 1: MainMenu, 2: MoonpetalValley_MVP, 3: Riverbank_Chapter2)
            UpdateBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 42! Chạy 'Moonpetal/Step 42/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 42/Validate")]
        public static void ValidateStep42()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 42 ===");

            if (!File.Exists(Ch2ScenePath))
            {
                Debug.LogError("[MV] THIẾU scene: " + Ch2ScenePath);
                errors++;
            }

            bool hasInBuild = false;
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (s.path == Ch2ScenePath && s.enabled) hasInBuild = true;
            }
            if (!hasInBuild)
            {
                Debug.LogError("[MV] Riverbank_Chapter2.unity chưa nằm trong Build Settings.");
                errors++;
            }

            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (UnityEngine.Object.FindAnyObjectByType<PlayerController2D>() == null) { Debug.LogError("[MV] Riverbank thiếu Player_Wen."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>() == null) { Debug.LogError("[MV] Riverbank thiếu Lumi_Companion."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<RiverBridgeController>() == null) { Debug.LogError("[MV] Riverbank thiếu RiverBridgeController."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<Chapter2IntroCutscene>() == null) { Debug.LogError("[MV] Riverbank thiếu Chapter2IntroCutscene."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<FerryBoatInteractable>(FindObjectsInactive.Include).Length < 2)
            {
                Debug.LogError("[MV] Riverbank thiếu đủ 2 bến Đò Ngang (Đông/Tây).");
                errors++;
            }

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
                Debug.Log("[MV] VALIDATE STEP 42: PASS (0 lỗi). Scene Riverbank_Chapter2, Cổng CR-01, Cầu Gãy, Đò Ngang & Cutscene Mở Ch2 sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 42: FAIL (" + errors + " lỗi).");
            }
        }

        [MenuItem("Moonpetal/Debug/Unlock & Go To Riverbank (Play Mode)")]
        public static void DebugGoToRiverbank()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("[MV] Lệnh này chỉ chạy trong Play Mode.");
                return;
            }
            if (GameRoot.Instance != null)
            {
                GameRoot.Instance.Flags?.SetFlag("flag_lumi_awakened", true);
                GameRoot.Instance.Flags?.SetFlag("flag_tutorial_done", true);
                GameRoot.Instance.Flags?.SetFlag("flag_nv1_completed", true);
                if (Chapter2StateManager.Instance != null)
                    Chapter2StateManager.Instance.PendingSceneSpawnId = "Spawn_EastEntrance";
                GameRoot.Instance.Scenes?.LoadScene("Riverbank_Chapter2");
            }
        }

        [MenuItem("Moonpetal/Debug/Toggle Bridge Repaired (Play Mode)")]
        public static void DebugToggleBridge()
        {
            if (!EditorApplication.isPlaying || Chapter2StateManager.Instance == null)
            {
                Debug.LogWarning("[MV] Lệnh này chỉ chạy trong Play Mode.");
                return;
            }
            var cur = Chapter2StateManager.Instance.BridgeState;
            var next = cur == RiverBridgeState.Repaired ? RiverBridgeState.Broken : RiverBridgeState.Repaired;
            Chapter2StateManager.Instance.SetBridgeState(next);
            GameEventBus.RaiseToastRequested("Debug Cầu: " + next);
        }

        private static void ReimportChapter2EnvSprites()
        {
            string folder = "Assets/Art/Sprites/Chapter2";
            if (!Directory.Exists(folder)) return;
            foreach (string f in Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories))
            {
                string path = f.Replace("\\", "/");
                var imp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (imp == null) continue;
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.filterMode = FilterMode.Point;
                imp.textureCompression = TextureImporterCompression.Uncompressed;
                imp.mipmapEnabled = false;
                imp.alphaIsTransparency = true;
                imp.spritePixelsPerUnit = 16f;
                var st = new TextureImporterSettings();
                imp.ReadTextureSettings(st);
                st.spriteMeshType = SpriteMeshType.FullRect;
                st.spriteAlignment = (int)SpriteAlignment.Center;
                st.spritePivot = new Vector2(0.5f, 0.5f);
                imp.SetTextureSettings(st);
                imp.SaveAndReimport();
            }
        }

        private static void InstallPortalInMvpScene()
        {
            var scene = EditorSceneManager.OpenScene(MvpScenePath, OpenSceneMode.Single);

            // Idempotent: tìm hoặc tạo mới Portal_ToRiverbank
            GameObject old = GameObject.Find("Portal_ToRiverbank");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            Sprite signSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "prop_portal_sign.png");
            GameObject portal = new GameObject("Portal_ToRiverbank");
            portal.tag = "Interactable";
            portal.layer = 11;
            portal.transform.position = new Vector3(-11.5f, -3.5f, 0f);

            var sr = portal.AddComponent<SpriteRenderer>();
            sr.sprite = signSpr;
            sr.sortingLayerName = "WorldObjects";
            var ys = portal.AddComponent<YSortRenderer>();
            ys.Configure(sr, false, true, 0f, 0);

            var col = portal.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 0.6f);

            var trig = portal.AddComponent<ScenePortalTrigger>();
            var so = new SerializedObject(trig);
            so.FindProperty("targetSceneName").stringValue = "Riverbank_Chapter2";
            so.FindProperty("targetSpawnId").stringValue = "Spawn_EastEntrance";
            so.FindProperty("requiredFlag").stringValue = "flag_nv1_completed";
            so.FindProperty("promptLabel").stringValue = "Đi tới Bờ Sông (Chương 2)";
            so.ApplyModifiedPropertiesWithoutUndo();

            // Thêm Spawn_FromRiverbank vào WorldSpawnRegistry (idempotent)
            var reg = UnityEngine.Object.FindAnyObjectByType<WorldSpawnRegistry>();
            if (reg != null)
            {
                var rSo = new SerializedObject(reg);
                var list = rSo.FindProperty("spawnPoints");
                bool exists = false;
                for (int i = 0; i < list.arraySize; i++)
                {
                    if (list.GetArrayElementAtIndex(i).FindPropertyRelative("id").stringValue == "Spawn_FromRiverbank")
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    int idx = list.arraySize;
                    list.InsertArrayElementAtIndex(idx);
                    var el = list.GetArrayElementAtIndex(idx);
                    el.FindPropertyRelative("id").stringValue = "Spawn_FromRiverbank";
                    el.FindPropertyRelative("displayName").stringValue = "Lối Về Từ Bờ Sông";
                    el.FindPropertyRelative("position").vector2Value = new Vector2(-10.2f, -3.5f);
                    rSo.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV] CR-01: Đã đặt cổng Portal_ToRiverbank tại (-11.5, -3.5) trong MoonpetalValley_MVP.unity.");
        }

        private static void BuildRiverbankScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Main Camera
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(18f, 0.5f, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.4f;
            cam.backgroundColor = new Color(0.08f, 0.14f, 0.22f, 1f);
            camGo.AddComponent<AudioListener>();
            var camFollow = camGo.AddComponent<CameraFollow2D>();

            // 2. EventSystem khớp Input backend
            CreateMatchingEventSystem();

            // 3. GameRoot, Player_Wen, Lumi_Companion
            GameObject gameRootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (gameRootPrefab != null) PrefabUtility.InstantiatePrefab(gameRootPrefab);

            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            GameObject lumiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath);

            GameObject playerInst = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            playerInst.name = "Player_Wen";
            playerInst.transform.position = new Vector3(20.5f, 0.5f, 0f);

            GameObject lumiInst = (GameObject)PrefabUtility.InstantiatePrefab(lumiPrefab);
            lumiInst.name = "Lumi_Companion";
            lumiInst.transform.position = new Vector3(19.8f, 1.2f, 0f);
            var lumiSO = new SerializedObject(lumiInst.GetComponent<LumiCompanionController>());
            lumiSO.FindProperty("followTarget").objectReferenceValue = playerInst.transform;
            lumiSO.ApplyModifiedPropertiesWithoutUndo();

            var camSO = new SerializedObject(camFollow);
            camSO.FindProperty("target").objectReferenceValue = playerInst.transform;
            camSO.FindProperty("minBounds").vector2Value = new Vector2(-17.5f, -8.5f);
            camSO.FindProperty("maxBounds").vector2Value = new Vector2(17.5f, 8.5f);
            camSO.ApplyModifiedPropertiesWithoutUndo();

            // 4. WorldSpawnRegistry cho Riverbank_Chapter2
            BuildRiverbankSpawnRegistry();

            // 5. Bản đồ Tilemap 52x28 (Cỏ bờ Đông, Sông ở giữa, Đầm Lầy bờ Tây) + Nước sông động
            BuildRiverbankMapAndWater();

            // 6. Công trình Bờ Đông & Bờ Tây + Cổng quay về Làng Chính
            BuildRiverbankBuildingsAndPortal();

            // 7. Cầu Gãy -> Cầu Mới + Đò Ngang Tạm 2 bờ
            GameObject tempPlank = BuildBridgeAndFerries();

            // 8. Cutscene Mở Chương 2 (trigger tại đầu cầu Đông)
            BuildIntroCutsceneRig(tempPlank);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, Ch2ScenePath);
            Debug.Log("[MV] Đã tạo scene mới: " + Ch2ScenePath);
        }

        private static void BuildRiverbankSpawnRegistry()
        {
            GameObject go = new GameObject("WorldSpawnRegistry");
            var reg = go.AddComponent<WorldSpawnRegistry>();
            var so = new SerializedObject(reg);
            var list = so.FindProperty("spawnPoints");
            var defs = new[]
            {
                ("Spawn_EastEntrance", "Đầu Đường Bờ Đông", new Vector2(20.5f, 0.5f)),
                ("Spawn_EastCamp", "Lều Tạm Bờ Đông", new Vector2(14.5f, 2.5f)),
                ("Spawn_BridgeEast", "Mố Cầu Bờ Đông", new Vector2(9.2f, 0.5f)),
                ("Spawn_WestDock", "Bến Thuyền Cũ Bờ Tây", new Vector2(-11.5f, -5.5f)),
                ("Spawn_WestSwamp", "Đầm Lầy Bờ Tây", new Vector2(-18.5f, 4.5f))
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
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildRiverbankMapAndWater()
        {
            GameObject gridGo = new GameObject("RiverbankMapGrid");
            gridGo.AddComponent<Grid>();

            GameObject groundGo = new GameObject("GroundTilemap");
            groundGo.transform.SetParent(gridGo.transform, false);
            var groundTm = groundGo.AddComponent<Tilemap>();
            var gRen = groundGo.AddComponent<TilemapRenderer>();
            gRen.sortingLayerName = "Ground";

            GameObject obsGo = new GameObject("ObstacleTilemap");
            obsGo.transform.SetParent(gridGo.transform, false);
            obsGo.layer = 10;
            var obsTm = obsGo.AddComponent<Tilemap>();
            var oRen = obsGo.AddComponent<TilemapRenderer>();
            oRen.sortingLayerName = "WorldObjects";
            oRen.sortingOrder = -5000;
            obsGo.AddComponent<TilemapCollider2D>();

            Sprite grassSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvArtRoot + "Tiles/tile_grass_0.png");
            Sprite pathSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvArtRoot + "Tiles/tile_path_0.png");
            Sprite treeSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvArtRoot + "Tiles/tile_tree.png");
            Sprite swampSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_swamp_ground.png");
            Sprite water0 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_river_water_00.png");

            Tile tGrass = MakeTile(grassSpr);
            Tile tPath = MakeTile(pathSpr);
            Tile tTree = MakeTile(treeSpr);
            Tile tSwamp = MakeTile(swampSpr);
            Tile tWater = MakeTile(water0);

            var waterRenderers = new List<SpriteRenderer>();
            GameObject waterAnimRoot = new GameObject("AnimatedRiverWater");

            for (int y = -14; y < 14; y++)
            {
                for (int x = -26; x < 26; x++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    bool border = (x == -26 || x == 25 || y == -14 || y == 13);
                    bool isRiver = (x >= -8 && x <= 7);
                    bool isBridgeRow = (y >= -1 && y <= 1);
                    bool isMainPath = (y >= -1 && y <= 1 && !isRiver) || (x == 11 && y >= -8 && y <= 3) || (x == -11 && y >= -8 && y <= 4);

                    if (isRiver)
                    {
                        groundTm.SetTile(cell, tWater);
                        if (!isBridgeRow)
                        {
                            obsTm.SetTile(cell, tWater);
                        }
                        if (!isBridgeRow || y == -1 || y == 1)
                        {
                            GameObject wCell = new GameObject($"RiverCell_{x}_{y}");
                            wCell.transform.SetParent(waterAnimRoot.transform, false);
                            wCell.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                            var wsr = wCell.AddComponent<SpriteRenderer>();
                            wsr.sprite = water0;
                            wsr.sortingLayerName = "GroundDecor";
                            wsr.sortingOrder = 2;
                            waterRenderers.Add(wsr);
                        }
                    }
                    else if (x < -8)
                    {
                        groundTm.SetTile(cell, isMainPath ? tPath : tSwamp);
                    }
                    else
                    {
                        groundTm.SetTile(cell, isMainPath ? tPath : tGrass);
                    }

                    if (border && !isRiver)
                    {
                        obsTm.SetTile(cell, tTree);
                    }
                }
            }

            // WaterTileAnimator cho dòng sông 4 frame
            Sprite[] wFrames =
            {
                AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_river_water_00.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_river_water_01.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_river_water_02.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_river_water_03.png")
            };
            var wAnim = waterAnimRoot.AddComponent<WaterTileAnimator>();
            var wSO = new SerializedObject(wAnim);
            var fProp = wSO.FindProperty("waterFrames");
            fProp.arraySize = wFrames.Length;
            for (int i = 0; i < wFrames.Length; i++) fProp.GetArrayElementAtIndex(i).objectReferenceValue = wFrames[i];
            var rProp = wSO.FindProperty("waterRenderers");
            rProp.arraySize = waterRenderers.Count;
            for (int i = 0; i < waterRenderers.Count; i++) rProp.GetArrayElementAtIndex(i).objectReferenceValue = waterRenderers[i];
            wSO.FindProperty("fps").floatValue = 4.5f;
            wSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildRiverbankBuildingsAndPortal()
        {
            GameObject group = new GameObject("RiverbankStructures");

            SpawnBuilding(group.transform, "House_KaelWorkshop", Ch2ArtRoot + "prop_kael_workshop.png", new Vector3(12.0f, 5.2f, 0f), new Vector2(3.6f, 1.5f), new Vector2(0f, -0.7f));
            SpawnBuilding(group.transform, "House_EastNursery", Ch2ArtRoot + "prop_nursery_east.png", new Vector3(19.5f, 5.5f, 0f), new Vector2(3.2f, 1.4f), new Vector2(0f, -0.6f));
            SpawnBuilding(group.transform, "House_MaraDock", Ch2ArtRoot + "prop_mara_hut.png", new Vector3(-13.5f, -3.2f, 0f), new Vector2(3.2f, 1.4f), new Vector2(0f, -0.6f));

            // Cổng quay về Làng Chính (không khóa)
            GameObject portal = new GameObject("Portal_ToVillage");
            portal.tag = "Interactable";
            portal.layer = 11;
            portal.transform.SetParent(group.transform, false);
            portal.transform.position = new Vector3(22.8f, 0.5f, 0f);
            var sr = portal.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "prop_portal_sign.png");
            sr.sortingLayerName = "WorldObjects";
            portal.AddComponent<YSortRenderer>().Configure(sr, false, true, 0f, 0);
            portal.AddComponent<BoxCollider2D>().size = new Vector2(0.8f, 0.6f);

            var trig = portal.AddComponent<ScenePortalTrigger>();
            var so = new SerializedObject(trig);
            so.FindProperty("targetSceneName").stringValue = "MoonpetalValley_MVP";
            so.FindProperty("targetSpawnId").stringValue = "Spawn_FromRiverbank";
            so.FindProperty("requiredFlag").stringValue = "";
            so.FindProperty("promptLabel").stringValue = "Quay Về Làng Chính";
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SpawnBuilding(Transform parent, string name, string sprPath, Vector3 pos, Vector2 colSize, Vector2 colOffset)
        {
            GameObject go = new GameObject(name);
            go.layer = 10;
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(sprPath);
            sr.sortingLayerName = "WorldObjects";
            go.AddComponent<YSortRenderer>().Configure(sr, false, true, 0f, 0);
            var col = go.AddComponent<BoxCollider2D>();
            col.size = colSize;
            col.offset = colOffset;
        }

        private static GameObject BuildBridgeAndFerries()
        {
            Sprite plankSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_bridge_plank.png");
            Sprite brokenSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_bridge_broken.png");
            Sprite boatSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "prop_ferry_boat.png");

            GameObject bridgeRoot = new GameObject("RiverBridge");
            GameObject brokenRoot = new GameObject("BrokenVisuals");
            brokenRoot.transform.SetParent(bridgeRoot.transform, false);
            GameObject repairedRoot = new GameObject("RepairedVisuals");
            repairedRoot.transform.SetParent(bridgeRoot.transform, false);

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -8; x <= 7; x++)
                {
                    // Cầu hoàn chỉnh
                    GameObject repTile = new GameObject($"BridgeRep_{x}_{y}");
                    repTile.transform.SetParent(repairedRoot.transform, false);
                    repTile.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                    var rsr = repTile.AddComponent<SpriteRenderer>();
                    rsr.sprite = plankSpr;
                    rsr.sortingLayerName = "GroundDecor";
                    rsr.sortingOrder = 8;

                    // Cầu gãy: chỉ còn 2 đầu mố cầu (x <= -5 hoặc x >= 4)
                    if (x <= -5 || x >= 4)
                    {
                        GameObject brkTile = new GameObject($"BridgeBrk_{x}_{y}");
                        brkTile.transform.SetParent(brokenRoot.transform, false);
                        brkTile.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                        var bsr = brkTile.AddComponent<SpriteRenderer>();
                        bsr.sprite = (x == -5 || x == 4) ? brokenSpr : plankSpr;
                        bsr.sortingLayerName = "GroundDecor";
                        bsr.sortingOrder = 8;
                    }
                }
            }

            // Collider chặn giữa sông khi cầu gãy
            GameObject blocker = new GameObject("BridgeGapBlocker");
            blocker.layer = 10;
            blocker.transform.SetParent(bridgeRoot.transform, false);
            blocker.transform.position = new Vector3(0f, 0.5f, 0f);
            var bCol = blocker.AddComponent<BoxCollider2D>();
            bCol.size = new Vector2(8.5f, 3.4f);

            var ctrl = bridgeRoot.AddComponent<RiverBridgeController>();
            var bSO = new SerializedObject(ctrl);
            bSO.FindProperty("brokenVisualsRoot").objectReferenceValue = brokenRoot;
            bSO.FindProperty("repairedVisualsRoot").objectReferenceValue = repairedRoot;
            bSO.FindProperty("bridgeGapBlocker").objectReferenceValue = bCol;
            bSO.ApplyModifiedPropertiesWithoutUndo();

            // Tấm ván tạm bị nước cuốn trong Cutscene Mở Chương 2
            GameObject tempPlank = new GameObject("TempBridgePlank");
            tempPlank.transform.SetParent(bridgeRoot.transform, false);
            tempPlank.transform.position = new Vector3(3.5f, 0.5f, 0f);
            var tsr = tempPlank.AddComponent<SpriteRenderer>();
            tsr.sprite = plankSpr;
            tsr.sortingLayerName = "WorldObjects";
            tsr.sortingOrder = 50;

            // 2 Bến Đò Ngang Tạm (Đông <-> Tây)
            CreateFerryBoat("FerryBoat_East", boatSpr, new Vector3(9.2f, -7.5f, 0f), new Vector2(-10.5f, -7.5f),
                "Chèo Đò Sang Bờ Tây", "Bạn chèo chiếc đò gỗ sang Bến Thuyền Cũ Bờ Tây.");
            CreateFerryBoat("FerryBoat_West", boatSpr, new Vector3(-9.2f, -7.5f, 0f), new Vector2(10.5f, -7.5f),
                "Chèo Đò Về Bờ Đông", "Bạn chèo chiếc đò gỗ quay về Bờ Đông.");

            return tempPlank;
        }

        private static void CreateFerryBoat(string name, Sprite spr, Vector3 pos, Vector2 dest, string prompt, string toast)
        {
            GameObject go = new GameObject(name);
            go.tag = "Interactable";
            go.layer = 11;
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = "WorldObjects";
            go.AddComponent<YSortRenderer>().Configure(sr, false, true, 0f, 0);
            go.AddComponent<BoxCollider2D>().size = new Vector2(1.4f, 0.9f);

            var ferry = go.AddComponent<FerryBoatInteractable>();
            var so = new SerializedObject(ferry);
            so.FindProperty("destinationPosition").vector2Value = dest;
            so.FindProperty("promptLabel").stringValue = prompt;
            so.FindProperty("arrivalToast").stringValue = toast;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildIntroCutsceneRig(GameObject tempPlank)
        {
            TMP_FontAsset storyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_StoryFont.asset")
                ?? AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_DynamicFont.asset");

            GameObject triggerGo = new GameObject("Ch2IntroCutsceneTrigger");
            triggerGo.layer = 14; // TriggerZone
            triggerGo.transform.position = new Vector3(11.5f, 0.5f, 0f);
            var col = triggerGo.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(3.5f, 6.0f);

            GameObject canvasGo = new GameObject("Ch2IntroCanvas");
            canvasGo.transform.SetParent(triggerGo.transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 215;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
            var cg = canvasGo.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;

            GameObject box = new GameObject("SubtitleBox", typeof(RectTransform));
            box.transform.SetParent(canvasGo.transform, false);
            var br = box.GetComponent<RectTransform>();
            br.anchorMin = new Vector2(0.12f, 0f); br.anchorMax = new Vector2(0.88f, 0f);
            br.pivot = new Vector2(0.5f, 0f); br.anchoredPosition = new Vector2(0f, 22f); br.sizeDelta = new Vector2(0f, 108f);
            box.AddComponent<Image>().color = new Color(0.06f, 0.08f, 0.14f, 0.90f);

            GameObject spGo = new GameObject("Speaker", typeof(RectTransform));
            spGo.transform.SetParent(box.transform, false);
            var spr = spGo.GetComponent<RectTransform>();
            spr.anchorMin = new Vector2(0f, 1f); spr.anchorMax = new Vector2(1f, 1f);
            spr.pivot = new Vector2(0.5f, 1f); spr.anchoredPosition = new Vector2(0f, -10f); spr.sizeDelta = new Vector2(-32f, 26f);
            var spTmp = spGo.AddComponent<TextMeshProUGUI>();
            spTmp.font = storyFont; spTmp.fontSize = 18f; spTmp.fontStyle = FontStyles.Bold;
            spTmp.alignment = TextAlignmentOptions.Center; spTmp.color = new Color(0.98f, 0.86f, 0.54f, 1f);

            GameObject subGo = new GameObject("Body", typeof(RectTransform));
            subGo.transform.SetParent(box.transform, false);
            var sbr = subGo.GetComponent<RectTransform>();
            sbr.anchorMin = Vector2.zero; sbr.anchorMax = Vector2.one;
            sbr.offsetMin = new Vector2(22f, 12f); sbr.offsetMax = new Vector2(-22f, -38f);
            var subTmp = subGo.AddComponent<TextMeshProUGUI>();
            subTmp.font = storyFont; subTmp.fontSize = 19f;
            subTmp.alignment = TextAlignmentOptions.Center; subTmp.textWrappingMode = TextWrappingModes.Normal;
            subTmp.color = Color.white;

            GameObject skipBg = new GameObject("SkipBg", typeof(RectTransform));
            skipBg.transform.SetParent(canvasGo.transform, false);
            var skRt = skipBg.GetComponent<RectTransform>();
            skRt.anchorMin = new Vector2(1f, 1f); skRt.anchorMax = new Vector2(1f, 1f);
            skRt.pivot = new Vector2(1f, 1f); skRt.anchoredPosition = new Vector2(-24f, -24f); skRt.sizeDelta = new Vector2(180f, 10f);
            skipBg.AddComponent<Image>().color = new Color(0.2f, 0.25f, 0.35f, 0.7f);

            GameObject skipFillGo = new GameObject("SkipFill", typeof(RectTransform));
            skipFillGo.transform.SetParent(skipBg.transform, false);
            var sfr = skipFillGo.GetComponent<RectTransform>();
            sfr.anchorMin = Vector2.zero; sfr.anchorMax = Vector2.one; sfr.offsetMin = Vector2.zero; sfr.offsetMax = Vector2.zero;
            var skipFill = skipFillGo.AddComponent<Image>();
            skipFill.color = new Color(0.95f, 0.78f, 0.36f, 0.95f);
            skipFill.type = Image.Type.Filled;
            skipFill.fillMethod = Image.FillMethod.Horizontal;
            skipFill.fillAmount = 0f;
            skipBg.SetActive(false);

            var cut = triggerGo.AddComponent<Chapter2IntroCutscene>();
            var so = new SerializedObject(cut);
            so.FindProperty("rootGroup").objectReferenceValue = cg;
            so.FindProperty("speakerText").objectReferenceValue = spTmp;
            so.FindProperty("subtitleText").objectReferenceValue = subTmp;
            so.FindProperty("skipBarRoot").objectReferenceValue = skipBg;
            so.FindProperty("skipFillImage").objectReferenceValue = skipFill;
            so.FindProperty("tempPlankTransform").objectReferenceValue = tempPlank.transform;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Tile MakeTile(Sprite spr)
        {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static void CreateMatchingEventSystem()
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Type modType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (modType != null) { esGo.AddComponent(modType); return; }
#endif
            esGo.AddComponent<StandaloneInputModule>();
        }

        private static void UpdateBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(MvpScenePath, true),
                new EditorBuildSettingsScene(Ch2ScenePath, true)
            };
        }
    }
}
