#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Audio;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2R2
    {
        private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string MvpScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string WestSwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";

        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";
        private const string StormLanternActorPath = "Assets/Prefabs/Chapter2/Actor_StormLantern.prefab";

        private const string Echo1AssetPath = "Assets/Data/Chapter2/Echo_Ch2_StormLantern.asset";
        private const string DlgPostEcho1Path = "Assets/Data/Chapter2/Dialogues/Dlg_Ch2_PostEcho1.asset";

        private const string EnvArtRoot = "Assets/Art/Sprites/Environment/";
        private const string Ch2ArtRoot = "Assets/Art/Sprites/Chapter2/";
        private const string PulseRingPath = "Assets/Art/Sprites/UI/spr_pulse_ring.png";

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R2 - 1. Build WestSwamp Scene & Mist Portal")]
        public static void BuildStepCh2R2()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Ch2-R2] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Ch2-R2] === BẮT ĐẦU STEP CH2-R2: TẠO SCENE WESTSWAMP_CHAPTER2 & CỔNG SƯƠNG MÙ ===");

            BuildWestSwampScene();
            UpdateRiverbankSceneForSwampPortal();
            UpdateBuildSettingsTo5Scenes();
            UpdateMusicRulesAsset();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Ch2-R2] === HOÀN TẤT STEP CH2-R2! Chạy 'Step Ch2-R2 - 2. Validate Step Ch2-R2' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Chapter 2 Overhaul/Step Ch2-R2 - 2. Validate Step Ch2-R2")]
        public static void ValidateStepCh2R2()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-R2] === BẮT ĐẦU VALIDATE STEP CH2-R2 ===");

            bool hasSwampInBuild = false;
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path == WestSwampScenePath && EditorBuildSettings.scenes[i].enabled)
                    hasSwampInBuild = true;
            }
            if (!hasSwampInBuild)
            {
                Debug.LogError("[MV][Ch2-R2][FAIL] WestSwamp_Chapter2.unity chưa nằm trong Build Settings!");
                errors++;
            }

            // Validate WestSwamp_Chapter2.unity
            var swampScene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);
            if (!swampScene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R2][FAIL] Không mở được " + WestSwampScenePath);
                return;
            }

            if (Object.FindAnyObjectByType<PlayerController2D>() == null) { Debug.LogError("[MV][Ch2-R2][FAIL] WestSwamp thiếu Player_Wen."); errors++; }
            if (Object.FindAnyObjectByType<LumiCompanionController>() == null) { Debug.LogError("[MV][Ch2-R2][FAIL] WestSwamp thiếu Lumi_Companion."); errors++; }
            if (Object.FindAnyObjectByType<WorldSpawnRegistry>() == null) { Debug.LogError("[MV][Ch2-R2][FAIL] WestSwamp thiếu WorldSpawnRegistry."); errors++; }
            if (GameObject.Find("Portal_ToRiverbankWest") == null) { Debug.LogError("[MV][Ch2-R2][FAIL] WestSwamp thiếu Portal_ToRiverbankWest."); errors++; }
            if (GameObject.Find("EchoSpot_StormLantern") == null) { Debug.LogError("[MV][Ch2-R2][FAIL] WestSwamp thiếu EchoSpot_StormLantern."); errors++; }
            if (GameObject.Find("SwampNest_North") == null || GameObject.Find("SwampNest_West") == null || GameObject.Find("SwampNest_East") == null)
            {
                Debug.LogError("[MV][Ch2-R2][FAIL] WestSwamp thiếu 3 ổ sương mù (SwampNest_North/West/East).");
                errors++;
            }
            errors += CountMissingScriptsInActiveScene("WestSwamp_Chapter2");

            // Validate Riverbank_Chapter2.unity
            var riverScene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!riverScene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R2][FAIL] Không mở được " + Ch2ScenePath);
                return;
            }
            if (GameObject.Find("Portal_ToWestSwamp") == null) { Debug.LogError("[MV][Ch2-R2][FAIL] Riverbank thiếu Portal_ToWestSwamp."); errors++; }
            if (Object.FindAnyObjectByType<SwampMistBarrierController>() == null) { Debug.LogError("[MV][Ch2-R2][FAIL] Riverbank thiếu SwampMistBarrierController."); errors++; }
            if (GameObject.Find("SwampCombatZone") != null) { Debug.LogError("[MV][Ch2-R2][FAIL] Riverbank vẫn còn SwampCombatZone cũ!"); errors++; }
            if (GameObject.Find("EchoSpot_StormLantern") != null) { Debug.LogError("[MV][Ch2-R2][FAIL] Riverbank vẫn còn EchoSpot_StormLantern cũ!"); errors++; }
            errors += CountMissingScriptsInActiveScene("Riverbank_Chapter2");

            if (errors == 0)
                Debug.Log("[MV][Ch2-R2] VALIDATE PASS (0 lỗi) - WestSwamp_Chapter2 & Cổng Sương Mù Bờ Tây sẵn sàng (5 scenes Build Settings, 0 Missing Scripts)!");
            else
                Debug.LogError($"[MV][Ch2-R2] VALIDATE FAIL ({errors} lỗi).");
        }

        private static void BuildWestSwampScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Main Camera
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(13.5f, -7.8f, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.4f;
            cam.backgroundColor = new Color(0.05f, 0.11f, 0.12f, 1f);
            camGo.AddComponent<AudioListener>();
            var camFollow = camGo.AddComponent<CameraFollow2D>();

            // 2. EventSystem
            CreateMatchingEventSystem();

            // 3. GameRoot, Player_Wen, Lumi_Companion
            GameObject gameRootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (gameRootPrefab != null) PrefabUtility.InstantiatePrefab(gameRootPrefab);

            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            GameObject lumiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath);

            GameObject playerInst = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            playerInst.name = "Player_Wen";
            playerInst.transform.position = new Vector3(13.5f, -7.8f, 0f);

            GameObject lumiInst = (GameObject)PrefabUtility.InstantiatePrefab(lumiPrefab);
            lumiInst.name = "Lumi_Companion";
            lumiInst.transform.position = new Vector3(12.8f, -7.1f, 0f);
            var lumiCtrl = lumiInst.GetComponent<LumiCompanionController>();
            if (lumiCtrl != null)
            {
                var lumiSO = new SerializedObject(lumiCtrl);
                lumiSO.FindProperty("followTarget").objectReferenceValue = playerInst.transform;
                lumiSO.ApplyModifiedPropertiesWithoutUndo();
            }

            var camSO = new SerializedObject(camFollow);
            camSO.FindProperty("target").objectReferenceValue = playerInst.transform;
            camSO.FindProperty("minBounds").vector2Value = new Vector2(-14.5f, -8.5f);
            camSO.FindProperty("maxBounds").vector2Value = new Vector2(14.5f, 8.5f);
            camSO.ApplyModifiedPropertiesWithoutUndo();

            // 4. WorldSpawnRegistry
            BuildWestSwampSpawnRegistry();

            // 5. Tilemap 44x28 (Đầm lầy rộng, lối đi rêu phong, ao sương & rừng cây bao quanh)
            BuildWestSwampTilemaps();

            // 6. 3 Ổ Sương Mù (điểm spawn 3 đợt quái ở Step Ch2-R3)
            BuildSwampMistNests();

            // 7. Miếu Đèn Bão Cổ ở góc Tây-Bắc + EchoSpot_StormLantern
            BuildAncientLanternShrineAndEcho1();

            // 8. Cổng quay về Riverbank_Chapter2 (góc Đông-Nam)
            BuildReturnPortalToRiverbank();

            // 9. Sương mù trôi tự nhiên
            BuildAmbientSwampMist();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, WestSwampScenePath);
            Debug.Log("[MV][Ch2-R2] Đã tạo scene mới: " + WestSwampScenePath);
        }

        private static void BuildWestSwampSpawnRegistry()
        {
            GameObject go = new GameObject("WorldSpawnRegistry");
            var reg = go.AddComponent<WorldSpawnRegistry>();
            var so = new SerializedObject(reg);
            var list = so.FindProperty("spawnPoints");
            var defs = new[]
            {
                ("Spawn_SwampEntrance", "Lối Vào Đầm Lầy Bờ Tây", new Vector2(13.5f, -7.8f)),
                ("Spawn_SwampCenter", "Bãi Sương Trung Tâm", new Vector2(-1.0f, -0.5f)),
                ("Spawn_SwampShrine", "Miếu Đèn Bão Cổ", new Vector2(-11.5f, 5.5f))
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

        private static void BuildWestSwampTilemaps()
        {
            GameObject gridGo = new GameObject("WestSwampMapGrid");
            gridGo.AddComponent<Grid>();

            GameObject groundGo = new GameObject("GroundTilemap");
            groundGo.transform.SetParent(gridGo.transform, false);
            var groundTm = groundGo.AddComponent<Tilemap>();
            var gRen = groundGo.AddComponent<TilemapRenderer>();
            gRen.sortingLayerName = "Ground";
            gRen.sortingOrder = 0;

            GameObject obsGo = new GameObject("ObstacleTilemap");
            obsGo.transform.SetParent(gridGo.transform, false);
            obsGo.layer = 10;
            var obsTm = obsGo.AddComponent<Tilemap>();
            var oRen = obsGo.AddComponent<TilemapRenderer>();
            oRen.sortingLayerName = "WorldObjects";
            oRen.sortingOrder = 2;
            obsGo.AddComponent<TilemapCollider2D>();

            Sprite swampSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_swamp_ground.png");
            Sprite grassSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvArtRoot + "Tiles/tile_grass_0.png");
            Sprite pathSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvArtRoot + "Tiles/tile_path_0.png");
            Sprite treeSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvArtRoot + "Tiles/tile_tree.png");
            Sprite waterSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "tile_river_water_00.png");

            Tile swampTile = MakeTile(swampSpr != null ? swampSpr : grassSpr);
            Tile pathTile = MakeTile(pathSpr != null ? pathSpr : swampSpr);
            Tile treeTile = MakeTile(treeSpr);
            Tile poolTile = MakeTile(waterSpr);

            for (int x = -22; x <= 21; x++)
            {
                for (int y = -14; y <= 13; y++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    groundTm.SetTile(cell, swampTile);

                    // Rào cây dày bao quanh biên map
                    bool isBorder = (x <= -19 || x >= 19 || y <= -11 || y >= 10);
                    if (isBorder)
                    {
                        if (treeTile != null) obsTm.SetTile(cell, treeTile);
                        continue;
                    }

                    // Đường mòn rêu nối từ lối vào (Đông-Nam) -> trung tâm -> Miếu Đèn Bão (Tây-Bắc)
                    bool onEntryPath = (y >= -9 && y <= -7 && x >= 4 && x <= 16);
                    bool onMidPath = (x >= -2 && x <= 4 && y >= -7 && y <= 1);
                    bool onShrinePath = (y >= 1 && y <= 6 && x >= -15 && x <= -2);
                    if (onEntryPath || onMidPath || onShrinePath)
                    {
                        groundTm.SetTile(cell, pathTile);
                    }

                    // 3 ao nước đầm lầy nhỏ tạo địa hình né đòn (không chắn đường đi chính)
                    bool poolA = (x >= -15 && x <= -12 && y >= -8 && y <= -6);
                    bool poolB = (x >= 9 && x <= 12 && y >= 4 && y <= 6);
                    bool poolC = (x >= -6 && x <= -4 && y >= -8 && y <= -7);
                    if ((poolA || poolB || poolC) && poolTile != null)
                    {
                        obsTm.SetTile(cell, poolTile);
                    }
                }
            }
        }

        private static void BuildSwampMistNests()
        {
            GameObject nestsRoot = new GameObject("SwampMistNests");
            Sprite pulseSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);

            var nestDefs = new[]
            {
                ("SwampNest_North", new Vector3(-1.0f, 6.2f, 0f)),
                ("SwampNest_West", new Vector3(-11.5f, -1.5f, 0f)),
                ("SwampNest_East", new Vector3(7.5f, 3.2f, 0f))
            };

            for (int i = 0; i < nestDefs.Length; i++)
            {
                GameObject nest = new GameObject(nestDefs[i].Item1);
                nest.transform.SetParent(nestsRoot.transform, false);
                nest.transform.position = nestDefs[i].Item2;

                if (pulseSpr != null)
                {
                    var sr = nest.AddComponent<SpriteRenderer>();
                    sr.sprite = pulseSpr;
                    sr.color = new Color(0.55f, 0.30f, 0.78f, 0.48f);
                    sr.sortingLayerName = "Ground";
                    sr.sortingOrder = 5;
                    nest.transform.localScale = new Vector3(1.6f, 1.1f, 1f);
                }
            }
        }

        private static void BuildAncientLanternShrineAndEcho1()
        {
            GameObject shrineRoot = new GameObject("AncientLanternShrine");
            shrineRoot.transform.position = new Vector3(-13.5f, 6.8f, 0f);

            GameObject lanternPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(StormLanternActorPath);
            if (lanternPrefab != null)
            {
                GameObject lanternInst = (GameObject)PrefabUtility.InstantiatePrefab(lanternPrefab);
                lanternInst.name = "Shrine_StormLanternVisual";
                lanternInst.transform.SetParent(shrineRoot.transform, false);
                lanternInst.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            }

            EchoData echo1 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo1AssetPath);
            DialogueData postEcho1 = AssetDatabase.LoadAssetAtPath<DialogueData>(DlgPostEcho1Path);
            Sprite pulseSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);

            GameObject echoGo = new GameObject("EchoSpot_StormLantern");
            echoGo.transform.position = new Vector3(-13.5f, 6.4f, 0f);

            GameObject shimmer = new GameObject("ShimmerRing");
            shimmer.transform.SetParent(echoGo.transform, false);
            var sr = shimmer.AddComponent<SpriteRenderer>();
            sr.sprite = pulseSpr;
            sr.color = new Color(0.58f, 0.90f, 1f, 0.42f);
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = 16;

            var comp = echoGo.AddComponent<EchoSpotInteractable>();
            var so = new SerializedObject(comp);
            so.FindProperty("echoData").objectReferenceValue = echo1;
            so.FindProperty("postEchoDialogue").objectReferenceValue = postEcho1;
            so.FindProperty("promptLabel").stringValue = "Dùng Ánh Soi: Ký Ức Đèn Bão";
            so.FindProperty("blockedHint").stringValue = "Cần đủ Khung Đồng (Ông Kael) và Chao Kính (Đầm Lầy) để soi Dư Âm này.";
            so.FindProperty("shimmerRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildReturnPortalToRiverbank()
        {
            GameObject portalGo = new GameObject("Portal_ToRiverbankWest");
            portalGo.transform.position = new Vector3(15.8f, -8.0f, 0f);

            Sprite pulseSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);
            if (pulseSpr != null)
            {
                var sr = portalGo.AddComponent<SpriteRenderer>();
                sr.sprite = pulseSpr;
                sr.color = new Color(1f, 0.86f, 0.45f, 0.62f);
                sr.sortingLayerName = "Ground";
                sr.sortingOrder = 6;
                portalGo.transform.localScale = new Vector3(1.5f, 1.0f, 1f);
            }

            var trigger = portalGo.AddComponent<ScenePortalTrigger>();
            var so = new SerializedObject(trigger);
            so.FindProperty("targetSceneName").stringValue = Chapter2StateManager.Chapter2SceneName;
            so.FindProperty("targetSpawnId").stringValue = "Spawn_WestSwamp";
            so.FindProperty("requiredFlag").stringValue = "";
            so.FindProperty("useSwampUnlockGate").boolValue = false;
            so.FindProperty("promptLabel").stringValue = "Quay lại Bờ Tây (Bờ Sông)";
            so.FindProperty("interactRadius").floatValue = 2.4f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildAmbientSwampMist()
        {
            Sprite mistSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "ch2_river_mist_strip.png");
            if (mistSpr == null) return;

            GameObject mistParent = new GameObject("SwampAmbientMist");
            var spots = new[]
            {
                (new Vector3(-10f, 3f, 0f), 0.35f, 1.2f, 0.32f, 0.0f),
                (new Vector3(0f, -2f, 0f), 0.28f, 1.5f, 0.28f, 1.7f),
                (new Vector3(8f, 1f, 0f), 0.32f, 1.1f, 0.30f, 3.1f),
                (new Vector3(-5f, 7f, 0f), 0.25f, 1.3f, 0.34f, 4.4f)
            };

            for (int i = 0; i < spots.Length; i++)
            {
                GameObject m = new GameObject($"SwampMist_{i + 1}");
                m.transform.SetParent(mistParent.transform, false);
                m.transform.position = spots[i].Item1;
                m.transform.localScale = new Vector3(2.2f, 1.4f, 1f);
                var sr = m.AddComponent<SpriteRenderer>();
                sr.sprite = mistSpr;
                sr.color = new Color(0.75f, 0.88f, 0.92f, spots[i].Item4);
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = 250;
                var drifter = m.AddComponent<RiverMistDrifter>();
                drifter.Configure(spots[i].Item2, spots[i].Item3, spots[i].Item4, spots[i].Item5);
            }
        }

        private static void UpdateRiverbankSceneForSwampPortal()
        {
            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Ch2-R2] Không mở được " + Ch2ScenePath);
                return;
            }

            // 1. Dọn SwampCombatZone cũ & EchoSpot_StormLantern cũ khỏi Riverbank_Chapter2
            string[] obsoleteNames = { "SwampCombatZone", "SwampCombatZone_West", "EchoSpot_StormLantern" };
            for (int i = 0; i < obsoleteNames.Length; i++)
            {
                GameObject old = FindSceneObjectEvenIfInactive(obsoleteNames[i]);
                if (old != null)
                {
                    Object.DestroyImmediate(old);
                    Debug.Log("[MV][Ch2-R2] Đã gỡ object cũ khỏi Riverbank_Chapter2: " + obsoleteNames[i]);
                }
            }

            // 2. Tạo / cập nhật Portal_ToWestSwamp + SwampMistBarrier ở lối Tây-Bắc Bờ Tây
            GameObject oldPortal = FindSceneObjectEvenIfInactive("Portal_ToWestSwamp");
            if (oldPortal != null) Object.DestroyImmediate(oldPortal);

            Vector3 portalPos = new Vector3(-16.5f, 7.2f, 0f);
            GameObject portalGo = new GameObject("Portal_ToWestSwamp");
            portalGo.transform.position = portalPos;

            Sprite pulseSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);
            Sprite mistSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2ArtRoot + "ch2_river_mist_strip.png");

            GameObject glowGo = new GameObject("UnlockedPortalGlow");
            glowGo.transform.SetParent(portalGo.transform, false);
            if (pulseSpr != null)
            {
                var gSr = glowGo.AddComponent<SpriteRenderer>();
                gSr.sprite = pulseSpr;
                gSr.color = new Color(1f, 0.84f, 0.42f, 0.65f);
                gSr.sortingLayerName = "Ground";
                gSr.sortingOrder = 6;
                glowGo.transform.localScale = new Vector3(1.6f, 1.1f, 1f);
            }

            var trigger = portalGo.AddComponent<ScenePortalTrigger>();
            var tso = new SerializedObject(trigger);
            tso.FindProperty("targetSceneName").stringValue = Chapter2StateManager.WestSwampSceneName;
            tso.FindProperty("targetSpawnId").stringValue = "Spawn_SwampEntrance";
            tso.FindProperty("requiredFlag").stringValue = "";
            tso.FindProperty("useSwampUnlockGate").boolValue = true;
            tso.FindProperty("promptLabel").stringValue = "Tiến vào Đầm Lầy Bờ Tây";
            tso.FindProperty("lockedToast").stringValue = "Sương mù Đầm Lầy quá dày! Cần Khung Đồng Đèn Bão (Ông Kael) và Dầu Đèn Bão (Bà Mara) để mở lối.";
            tso.FindProperty("interactRadius").floatValue = 2.6f;
            tso.ApplyModifiedPropertiesWithoutUndo();

            // Rào sương mù trực quan chặn lối vào khi chưa mở khóa Stage 5
            GameObject oldBarrier = FindSceneObjectEvenIfInactive("SwampMistBarrier");
            if (oldBarrier != null) Object.DestroyImmediate(oldBarrier);

            GameObject barrierGo = new GameObject("SwampMistBarrier");
            barrierGo.transform.position = new Vector3(-16.5f, 6.6f, 0f);
            barrierGo.layer = 10;

            var blockerCol = barrierGo.AddComponent<BoxCollider2D>();
            blockerCol.size = new Vector2(4.2f, 1.2f);
            blockerCol.offset = new Vector2(0f, 0.1f);

            GameObject mistVis = new GameObject("MistWallVisuals");
            mistVis.transform.SetParent(barrierGo.transform, false);
            if (mistSpr != null)
            {
                for (int k = -1; k <= 1; k++)
                {
                    GameObject cloud = new GameObject($"MistCloud_{k + 2}");
                    cloud.transform.SetParent(mistVis.transform, false);
                    cloud.transform.localPosition = new Vector3(k * 1.35f, 0.2f + Mathf.Abs(k) * 0.15f, 0f);
                    cloud.transform.localScale = new Vector3(1.4f, 1.2f, 1f);
                    var mSr = cloud.AddComponent<SpriteRenderer>();
                    mSr.sprite = mistSpr;
                    mSr.color = new Color(0.62f, 0.78f, 0.86f, 0.78f);
                    mSr.sortingLayerName = "WorldObjects";
                    mSr.sortingOrder = 220;
                    var drifter = cloud.AddComponent<RiverMistDrifter>();
                    drifter.Configure(0.45f, 0.55f, 0.78f, k * 1.2f);
                }
            }

            var barrierCtrl = barrierGo.AddComponent<SwampMistBarrierController>();
            var bso = new SerializedObject(barrierCtrl);
            bso.FindProperty("mistVisualsRoot").objectReferenceValue = mistVis;
            bso.FindProperty("mistBlockerCollider").objectReferenceValue = blockerCol;
            bso.FindProperty("unlockedLanternGlow").objectReferenceValue = glowGo;
            bso.ApplyModifiedPropertiesWithoutUndo();

            // 3. Cập nhật Spawn_WestSwamp trong WorldSpawnRegistry của Riverbank_Chapter2
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
                        el.FindPropertyRelative("position").vector2Value = new Vector2(-16.5f, 5.2f);
                        el.FindPropertyRelative("displayName").stringValue = "Cửa Ngõ Đầm Lầy Bờ Tây";
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
                    el.FindPropertyRelative("displayName").stringValue = "Cửa Ngõ Đầm Lầy Bờ Tây";
                    el.FindPropertyRelative("position").vector2Value = new Vector2(-16.5f, 5.2f);
                }
                rso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(reg);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Ch2-R2] Đã cài Portal_ToWestSwamp + SwampMistBarrier vào Riverbank_Chapter2.unity.");
        }

        private static void UpdateBuildSettingsTo5Scenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(MvpScenePath, true),
                new EditorBuildSettingsScene(Ch2ScenePath, true),
                new EditorBuildSettingsScene(WestSwampScenePath, true)
            };
            Debug.Log("[MV][Ch2-R2] Đã cập nhật Build Settings gồm 5 scene (thêm WestSwamp_Chapter2.unity).");
        }

        private static void UpdateMusicRulesAsset()
        {
            string[] guids = AssetDatabase.FindAssets("t:MusicRulesSO");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                MusicRulesSO rules = AssetDatabase.LoadAssetAtPath<MusicRulesSO>(path);
                if (rules == null) continue;
                if (rules.riverSceneNames == null) rules.riverSceneNames = new List<string>();
                if (!rules.riverSceneNames.Contains(Chapter2StateManager.WestSwampSceneName))
                {
                    rules.riverSceneNames.Add(Chapter2StateManager.WestSwampSceneName);
                    EditorUtility.SetDirty(rules);
                    Debug.Log("[MV][Ch2-R2] Đã thêm WestSwamp_Chapter2 vào " + path);
                }
            }
        }

        private static Tile MakeTile(Sprite spr)
        {
            if (spr == null) return null;
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static void CreateMatchingEventSystem()
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            esGo.AddComponent<StandaloneInputModule>();
#endif
        }

        private static GameObject FindSceneObjectEvenIfInactive(string targetName)
        {
            var all = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != null && all[i].name == targetName) return all[i].gameObject;
            }
            return null;
        }

        private static int CountMissingScriptsInActiveScene(string sceneLabel)
        {
            int missing = 0;
            var all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < all.Length; i++)
            {
                int c = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(all[i]);
                if (c > 0)
                {
                    Debug.LogError($"[MV][Ch2-R2][FAIL] {sceneLabel} -> '{all[i].name}' có {c} Missing Script!");
                    missing += c;
                }
            }
            return missing;
        }
    }
}
#endif
