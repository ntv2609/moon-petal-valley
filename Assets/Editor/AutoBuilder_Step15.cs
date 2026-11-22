using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step15
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";
        private const string SlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";
        private const string LumiRoot = "Assets/Art/Sprites/Animations/Lumi/Idle/animations/";
        private const string SlimeRoot = "Assets/Art/Sprites/Animations/GloomSlime/Idle/animations/";

        [MenuItem("Moonpetal/Step 15/1. Apply World Polish, Gate, Shadows, Water & Re-wire Lumi/Slime")]
        public static void BuildStep15()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 15 ===");
            AssetDatabase.Refresh();

            ForceReimportFolder("Assets/Art/Sprites/Animations/Lumi", 20f);
            ForceReimportFolder("Assets/Art/Sprites/Animations/GloomSlime", 32f);
            ForceReimportFolder("Assets/Art/Sprites/Environment", 16f);
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Re-wire Lumi & GloomSlime Animations (Prefab + Scene instances)
            WireLumiAnimations();
            WireSlimeAnimations();

            // 2. Open South Garden Gate in MapLayoutConfig & Rebuild WorldMapGrid
            MapLayoutConfigSO mapCfg = OpenSouthGardenGateAndSaveConfig();
            if (mapCfg != null)
            {
                RebuildWorldTilemapWithUpgradedArt(mapCfg);
                BuildAnimatedPondWater(mapCfg);
            }

            // 3. Add Ground Shadows to Wen, Lumi, NPCs, Slimes
            Sprite shadowSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Shadows/spr_ground_shadow.png");
            AttachGroundShadows(shadowSpr);

            // 4. Upgrade Remaining Props + Bed + Well + Farm Soil
            UpgradeRemainingPropsAndFarmSoil();

            // 5. Enrich Map Layout with Garden Archway & Cozy Props
            EnrichWorldDecor();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 15! Chạy 'Moonpetal/Step 15/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 15/Validate")]
        public static void ValidateStep15()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 15 ===");

            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null || !cfg.asciiMap.Contains("FFFFFPPPPFFFFF"))
            {
                Debug.LogError("[MV] MapLayoutConfig.asset chưa mở cổng 4 ô vào Vườn Phía Nam (FFFFFPPPPFFFFF).");
                errors++;
            }

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi == null)
            {
                Debug.LogError("[MV] Không tìm thấy LumiCompanionController trong scene.");
                errors++;
            }
            else
            {
                var lso = new SerializedObject(lumi);
                int fLen = lso.FindProperty("floatFrames").arraySize;
                int pLen = lso.FindProperty("pulseFrames").arraySize;
                if (fLen < 4 || pLen < 4)
                {
                    Debug.LogError($"[MV] Lumi thiếu frame animation (float={fLen}, pulse={pLen}). Hãy kiểm tra gói tinh_linh_lumi_animations.zip.");
                    errors++;
                }
            }

            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            if (slimes.Length < 8)
            {
                Debug.LogError($"[MV] Thiếu GloomSlime trong scene ({slimes.Length}/8).");
                errors++;
            }
            else
            {
                var sso = new SerializedObject(slimes[0]);
                int iLen = sso.FindProperty("idleFrames").arraySize;
                int tLen = sso.FindProperty("telegraphFrames").arraySize;
                int jLen = sso.FindProperty("jumpFrames").arraySize;
                int hLen = sso.FindProperty("hitFrames").arraySize;
                int dLen = sso.FindProperty("dieFrames").arraySize;
                if (iLen < 4 || tLen < 3 || jLen < 4 || hLen < 3 || dLen < 4)
                {
                    Debug.LogError($"[MV] GloomSlime thiếu frame animation (idle={iLen}, tele={tLen}, jump={jLen}, hit={hLen}, die={dLen}).");
                    errors++;
                }
            }

            var waterAnim = UnityEngine.Object.FindAnyObjectByType<WaterTileAnimator>();
            if (waterAnim == null)
            {
                Debug.LogError("[MV] Thiếu WaterTileAnimator cho hồ nước.");
                errors++;
            }
            else
            {
                var wso = new SerializedObject(waterAnim);
                int wf = wso.FindProperty("waterFrames").arraySize;
                int wr = wso.FindProperty("waterRenderers").arraySize;
                if (wf < 4 || wr < 5)
                {
                    Debug.LogError($"[MV] WaterTileAnimator thiếu dữ liệu (frames={wf}, tiles={wr}).");
                    errors++;
                }
            }

            int shadowCount = 0;
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            for (int i = 0; i < allTransforms.Length; i++)
            {
                if (allTransforms[i].name == "GroundShadow") shadowCount++;
            }
            if (shadowCount < 14)
            {
                Debug.LogError($"[MV] Số lượng GroundShadow còn thiếu ({shadowCount}, cần >= 14 cho Wen, Lumi, 4 NPC, 8 Slime).");
                errors++;
            }

            string[] propNames =
            {
                "GreenhouseLetterTable", "GreenhouseSeedBox", "GreenhouseTestPlanter",
                "GreenhouseSeedShelf", "OtisBrokenMailbox", "GardenMemoryBench", "GreenhouseBed"
            };
            foreach (string pn in propNames)
            {
                var go = GameObject.Find(pn);
                if (go == null) continue;
                var sr = go.GetComponentInChildren<SpriteRenderer>();
                if (sr == null || sr.sprite == null)
                {
                    Debug.LogError("[MV] Prop thiếu SpriteRenderer hoặc Sprite: " + pn);
                    errors++;
                }
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
                Debug.Log($"[MV] VALIDATE STEP 15: PASS (0 lỗi | Shadows={shadowCount} | AnimatedWater=OK | GardenGate=OPEN | Lumi&Slime=OK).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 15: FAIL ({errors} lỗi).");
            }
        }

        private static void ForceReimportFolder(string folderPath, float ppu)
        {
            if (!AssetDatabase.IsValidFolder(folderPath)) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
            foreach (string g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var imp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (imp == null) continue;
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
        }

        private static Sprite[] LoadFrames(string root, string prefix, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                string path = $"{root}{prefix}_{i:00}.png";
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }

        private static void SetSpriteArray(SerializedObject so, string propName, Sprite[] frames)
        {
            var prop = so.FindProperty(propName);
            if (prop == null) return;
            prop.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++)
            {
                prop.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            }
        }

        private static void WireLumiAnimations()
        {
            Sprite[] floatF = LoadFrames(LumiRoot, "float", 4);
            Sprite[] pulseF = LoadFrames(LumiRoot, "moon_pulse", 4);
            if (floatF.Length == 0)
            {
                Debug.LogWarning("[MV] Chưa thấy frame Lumi tại " + LumiRoot);
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(LumiPrefabPath);
                ApplyLumiFramesToObject(root, floatF, pulseF);
                PrefabUtility.SaveAsPrefabAsset(root, LumiPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }

            var sceneLumis = UnityEngine.Object.FindObjectsByType<LumiCompanionController>(FindObjectsInactive.Include);
            foreach (var l in sceneLumis)
            {
                ApplyLumiFramesToObject(l.gameObject, floatF, pulseF);
            }
            Debug.Log($"[MV] Đã gắn animation Lumi: float={floatF.Length}f, moon_pulse={pulseF.Length}f.");
        }

        private static void ApplyLumiFramesToObject(GameObject go, Sprite[] floatF, Sprite[] pulseF)
        {
            var ctrl = go.GetComponent<LumiCompanionController>();
            if (ctrl == null) return;
            var vis = go.transform.Find("Visual");
            var sr = vis != null ? vis.GetComponent<SpriteRenderer>() : go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && floatF.Length > 0) sr.sprite = floatF[0];

            var so = new SerializedObject(ctrl);
            if (sr != null) so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            SetSpriteArray(so, "floatFrames", floatF);
            SetSpriteArray(so, "pulseFrames", pulseF);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ctrl);
        }

        private static void WireSlimeAnimations()
        {
            Sprite[] idleF = LoadFrames(SlimeRoot, "idle", 4);
            Sprite[] teleF = LoadFrames(SlimeRoot, "telegraph", 3);
            Sprite[] jumpF = LoadFrames(SlimeRoot, "jump", 4);
            Sprite[] hitF = LoadFrames(SlimeRoot, "hit_hurt", 3);
            Sprite[] dieF = LoadFrames(SlimeRoot, "purify_die", 4);
            if (idleF.Length == 0)
            {
                Debug.LogWarning("[MV] Chưa thấy frame GloomSlime tại " + SlimeRoot);
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(SlimePrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(SlimePrefabPath);
                ApplySlimeFramesToObject(root, idleF, teleF, jumpF, hitF, dieF);
                PrefabUtility.SaveAsPrefabAsset(root, SlimePrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }

            var sceneSlimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            foreach (var sl in sceneSlimes)
            {
                ApplySlimeFramesToObject(sl.gameObject, idleF, teleF, jumpF, hitF, dieF);
            }
            Debug.Log($"[MV] Đã gắn animation GloomSlime ({sceneSlimes.Length} instances): idle={idleF.Length}f, tele={teleF.Length}f, jump={jumpF.Length}f, hit={hitF.Length}f, die={dieF.Length}f.");
        }

        private static void ApplySlimeFramesToObject(GameObject go, Sprite[] idleF, Sprite[] teleF, Sprite[] jumpF, Sprite[] hitF, Sprite[] dieF)
        {
            var ctrl = go.GetComponent<GloomSlimeController>();
            if (ctrl == null) return;
            var so = new SerializedObject(ctrl);
            var srProp = so.FindProperty("spriteRenderer");
            var sr = srProp != null ? srProp.objectReferenceValue as SpriteRenderer : go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && idleF.Length > 0) sr.sprite = idleF[0];

            SetSpriteArray(so, "idleFrames", idleF);
            SetSpriteArray(so, "telegraphFrames", teleF);
            SetSpriteArray(so, "jumpFrames", jumpF);
            SetSpriteArray(so, "hitFrames", hitF);
            SetSpriteArray(so, "dieFrames", dieF);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ctrl);
        }

        private static MapLayoutConfigSO OpenSouthGardenGateAndSaveConfig()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null)
            {
                Debug.LogError("[MV] Không tìm thấy MapLayoutConfig.asset!");
                return null;
            }

            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            bool changed = false;
            for (int r = 0; r < rows.Length; r++)
            {
                // Row 17 is the north fence of South Garden (directly below Village south path)
                if (r == 17 && rows[r].Contains("FFFFFFFFFFFFFF"))
                {
                    rows[r] = rows[r].Replace("FFFFFFFFFFFFFF", "FFFFFPPPPFFFFF");
                    changed = true;
                }
            }

            if (changed)
            {
                cfg.asciiMap = string.Join("\n", rows);
                EditorUtility.SetDirty(cfg);
                AssetDatabase.SaveAssets();
                Debug.Log("[MV] Đã mở cổng rộng 4 ô vào Vườn Phía Nam tại hàng 17 (FFFFFPPPPFFFFF).");
            }
            return cfg;
        }

        private static Tile MakeTile(Sprite spr)
        {
            var t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.color = Color.white;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static void RebuildWorldTilemapWithUpgradedArt(MapLayoutConfigSO cfg)
        {
            Sprite[] grass =
            {
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_grass_0.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_grass_1.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_grass_2.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_grass_3.png"),
            };
            Sprite[] path =
            {
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_path_0.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_path_1.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_path_2.png"),
            };
            Sprite[] ghFloor =
            {
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_gh_floor_0.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_gh_floor_1.png"),
            };
            Sprite wallSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_wall.png");
            Sprite fenceSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_fence.png");
            Sprite treeSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_tree.png");
            Sprite waterSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Water/water_anim_00.png");
            if (waterSpr == null) waterSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_water.png");
            Sprite flowerbedSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_flowerbed.png");

            if (grass[0] == null || path[0] == null || fenceSpr == null)
            {
                Debug.LogWarning("[MV] Thiếu tile từ environment_art_pack.zip; chỉ cập nhật lại cổng nếu tile cũ còn sẵn.");
                return;
            }

            Tile[] grassTiles = new Tile[grass.Length];
            for (int i = 0; i < grass.Length; i++) grassTiles[i] = MakeTile(grass[i] != null ? grass[i] : grass[0]);
            Tile[] pathTiles = new Tile[path.Length];
            for (int i = 0; i < path.Length; i++) pathTiles[i] = MakeTile(path[i] != null ? path[i] : path[0]);
            Tile[] ghTiles = new Tile[ghFloor.Length];
            for (int i = 0; i < ghFloor.Length; i++) ghTiles[i] = MakeTile(ghFloor[i] != null ? ghFloor[i] : ghFloor[0]);

            Tile wallTile = MakeTile(wallSpr);
            Tile fenceTile = MakeTile(fenceSpr);
            Tile treeTile = MakeTile(treeSpr);
            Tile waterTile = MakeTile(waterSpr);
            Tile flowerbedTile = MakeTile(flowerbedSpr);

            GameObject gridGo = GameObject.Find("WorldMapGrid");
            if (gridGo != null) UnityEngine.Object.DestroyImmediate(gridGo);

            gridGo = new GameObject("WorldMapGrid");
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

            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            for (int r = 0; r < rows.Length; r++)
            {
                string row = rows[r];
                int y = cfg.originY + (rows.Length - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    int x = cfg.originX + c;
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    int hash = Mathf.Abs(x * 73856093 ^ y * 19349663);
                    char ch = row[c];

                    switch (ch)
                    {
                        case 'P':
                            groundTm.SetTile(pos, pathTiles[hash % pathTiles.Length]);
                            break;
                        case 'G':
                            groundTm.SetTile(pos, ghTiles[hash % ghTiles.Length]);
                            break;
                        case 'B':
                            groundTm.SetTile(pos, flowerbedTile);
                            break;
                        case '#':
                            groundTm.SetTile(pos, ghTiles[0]);
                            obsTm.SetTile(pos, wallTile);
                            break;
                        case 'F':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, fenceTile);
                            break;
                        case 'T':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, treeTile);
                            break;
                        case 'W':
                            groundTm.SetTile(pos, waterTile);
                            obsTm.SetTile(pos, waterTile);
                            break;
                        default:
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            break;
                    }
                }
            }
            Debug.Log("[MV] Đã dựng lại WorldMapGrid với cổng Vườn Phía Nam mở rộng 4 ô.");
        }

        private static void BuildAnimatedPondWater(MapLayoutConfigSO cfg)
        {
            Sprite[] frames = LoadFrames(EnvRoot + "Water/", "water_anim", 4);
            if (frames.Length == 0)
            {
                Debug.LogWarning("[MV] Không tìm thấy water_anim_00..03.png.");
                return;
            }

            GameObject oldWater = GameObject.Find("AnimatedPondWater");
            if (oldWater != null) UnityEngine.Object.DestroyImmediate(oldWater);

            GameObject root = new GameObject("AnimatedPondWater");
            var anim = root.AddComponent<WaterTileAnimator>();

            var renderers = new List<SpriteRenderer>();
            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            for (int r = 0; r < rows.Length; r++)
            {
                string row = rows[r];
                int y = cfg.originY + (rows.Length - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    if (row[c] != 'W') continue;
                    int x = cfg.originX + c;
                    GameObject cellGo = new GameObject($"WaterCell_{x}_{y}");
                    cellGo.transform.SetParent(root.transform, false);
                    cellGo.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                    var sr = cellGo.AddComponent<SpriteRenderer>();
                    sr.sprite = frames[0];
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = -4900;
                    renderers.Add(sr);
                }
            }

            var so = new SerializedObject(anim);
            SetSpriteArray(so, "waterFrames", frames);
            var rProp = so.FindProperty("waterRenderers");
            rProp.arraySize = renderers.Count;
            for (int i = 0; i < renderers.Count; i++)
            {
                rProp.GetArrayElementAtIndex(i).objectReferenceValue = renderers[i];
            }
            so.FindProperty("fps").floatValue = 3.2f;
            so.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log($"[MV] Đã tạo AnimatedPondWater cho {renderers.Count} ô hồ nước (4 frames).");
        }

        private static GameObject EnsureShadowChild(Transform parent, Sprite shadowSpr, Vector3 localPos, Vector3 localScale)
        {
            if (parent == null || shadowSpr == null) return null;
            Transform existing = parent.Find("GroundShadow");
            GameObject go = existing != null ? existing.gameObject : new GameObject("GroundShadow");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = shadowSpr;
            sr.color = new Color(1f, 1f, 1f, 0.62f);
            sr.sortingLayerName = "Shadows";
            sr.sortingOrder = 0;
            return go;
        }

        private static void AttachGroundShadows(Sprite shadowSpr)
        {
            if (shadowSpr == null)
            {
                Debug.LogWarning("[MV] Thiếu spr_ground_shadow.png.");
                return;
            }

            // Player prefab + scene
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) != null)
            {
                GameObject pRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
                EnsureShadowChild(pRoot.transform, shadowSpr, new Vector3(0f, -0.56f, 0f), new Vector3(1.05f, 0.85f, 1f));
                PrefabUtility.SaveAsPrefabAsset(pRoot, PlayerPrefabPath);
                PrefabUtility.UnloadPrefabContents(pRoot);
            }
            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                EnsureShadowChild(player.transform, shadowSpr, new Vector3(0f, -0.56f, 0f), new Vector3(1.05f, 0.85f, 1f));
            }

            // Lumi prefab + scene (shadow sits underneath floating Lumi)
            if (AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath) != null)
            {
                GameObject lRoot = PrefabUtility.LoadPrefabContents(LumiPrefabPath);
                EnsureShadowChild(lRoot.transform, shadowSpr, new Vector3(0f, -0.48f, 0f), new Vector3(0.62f, 0.50f, 1f));
                PrefabUtility.SaveAsPrefabAsset(lRoot, LumiPrefabPath);
                PrefabUtility.UnloadPrefabContents(lRoot);
            }
            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null)
            {
                EnsureShadowChild(lumi.transform, shadowSpr, new Vector3(0f, -0.48f, 0f), new Vector3(0.62f, 0.50f, 1f));
            }

            // GloomSlime prefab + scene instances
            if (AssetDatabase.LoadAssetAtPath<GameObject>(SlimePrefabPath) != null)
            {
                GameObject sRoot = PrefabUtility.LoadPrefabContents(SlimePrefabPath);
                GameObject sh = EnsureShadowChild(sRoot.transform, shadowSpr, new Vector3(0f, -0.38f, 0f), new Vector3(0.95f, 0.78f, 1f));
                var sc = sRoot.GetComponent<GloomSlimeController>();
                if (sc != null && sh != null)
                {
                    var so = new SerializedObject(sc);
                    so.FindProperty("groundShadow").objectReferenceValue = sh;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
                PrefabUtility.SaveAsPrefabAsset(sRoot, SlimePrefabPath);
                PrefabUtility.UnloadPrefabContents(sRoot);
            }
            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            foreach (var sl in slimes)
            {
                GameObject sh = EnsureShadowChild(sl.transform, shadowSpr, new Vector3(0f, -0.38f, 0f), new Vector3(0.95f, 0.78f, 1f));
                var so = new SerializedObject(sl);
                so.FindProperty("groundShadow").objectReferenceValue = sh;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(sl);
            }

            // All NPCs in scene
            var npcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            foreach (var npc in npcs)
            {
                EnsureShadowChild(npc.transform, shadowSpr, new Vector3(0f, -0.56f, 0f), new Vector3(1.05f, 0.85f, 1f));
                EditorUtility.SetDirty(npc.gameObject);
            }

            Debug.Log($"[MV] Đã gắn GroundShadow cho Wen, Lumi, {npcs.Length} NPC và {slimes.Length} Bọt Sương Đen.");
        }

        private static void ApplySpriteToNamedObject(string goName, string spriteRelPath, float scale = 1f)
        {
            GameObject go = GameObject.Find(goName);
            if (go == null) return;
            Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + spriteRelPath);
            if (spr == null) return;
            var sr = go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = spr;
                sr.color = Color.white;
                if (Mathf.Abs(scale - 1f) > 0.001f)
                {
                    sr.transform.localScale = new Vector3(scale, scale, 1f);
                }
                EditorUtility.SetDirty(sr);
            }
        }

        private static void UpgradeRemainingPropsAndFarmSoil()
        {
            ApplySpriteToNamedObject("GreenhouseLetterTable", "Props/prop_letter_table.png");
            ApplySpriteToNamedObject("GreenhouseSeedBox", "Props/prop_seed_box.png");
            ApplySpriteToNamedObject("GreenhouseTestPlanter", "Props/prop_test_planter.png");
            ApplySpriteToNamedObject("GreenhouseSeedShelf", "Props/prop_seed_shelf.png");
            ApplySpriteToNamedObject("OtisBrokenMailbox", "Props/prop_mailbox.png");
            ApplySpriteToNamedObject("GardenMemoryBench", "Props/prop_memory_bench.png");
            ApplySpriteToNamedObject("GreenhouseBed", "Props/prop_bed.png");
            ApplySpriteToNamedObject("VillageWaterWell", "Props/prop_well.png");

            Sprite soilUntilled = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Soil/soil_untilled.png");
            Sprite soilDry = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Soil/soil_dry.png");
            Sprite soilWet = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Soil/soil_wet.png");

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            if (soilUntilled != null && soilDry != null && soilWet != null)
            {
                foreach (var tile in tiles)
                {
                    var so = new SerializedObject(tile);
                    so.FindProperty("untilledSprite").objectReferenceValue = soilUntilled;
                    so.FindProperty("tilledDrySprite").objectReferenceValue = soilDry;
                    so.FindProperty("tilledWetSprite").objectReferenceValue = soilWet;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    tile.RefreshVisuals();
                    EditorUtility.SetDirty(tile);
                }
            }
            Debug.Log($"[MV] Đã nâng cấp art cho 8 đạo cụ còn thiếu và {tiles.Length} ô đất trồng.");
        }

        private static void SpawnDecor(Transform parent, string name, Sprite spr, Vector3 pos, int sortOrderOffset = 0, string sortingLayer = "WorldObjects")
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = sortingLayer;
            sr.sortingOrder = sortOrderOffset != 0 ? sortOrderOffset : Mathf.RoundToInt(-pos.y * 100f);
        }

        private static void EnrichWorldDecor()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) decorGroup = new GameObject("VillageDecorGroup");

            Transform existing = decorGroup.transform.Find("Step15WorldPolishGroup");
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

            GameObject polish = new GameObject("Step15WorldPolishGroup");
            polish.transform.SetParent(decorGroup.transform, false);

            Sprite archSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Buildings/prop_garden_arch.png");
            Sprite signSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_signpost.png");
            Sprite barrelSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_barrel.png");
            Sprite mushSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_mushrooms.png");
            Sprite picnicSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_picnic_table.png");
            Sprite oakSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Buildings/tree_hero_oak.png");
            Sprite blossomSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Buildings/tree_hero_blossom.png");
            Sprite bushSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_bush.png");
            Sprite rockSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_rock.png");
            Sprite fl0 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_flowers_0.png");
            Sprite fl1 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_flowers_1.png");
            Sprite fl2 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_flowers_2.png");

            // 1. South Garden Flower Archway (spans open 4-tile gate at x=0, y=-1.5)
            SpawnDecor(polish.transform, "GardenEntranceArch", archSpr, new Vector3(0.0f, -1.05f, 0f), 2600, "WorldObjects");

            // 2. Village Crossroads & Cozy Gathering Props
            SpawnDecor(polish.transform, "VillageSignpost", signSpr, new Vector3(-2.6f, 2.2f, 0f));
            SpawnDecor(polish.transform, "BakeryPicnicTable", picnicSpr, new Vector3(5.6f, 5.2f, 0f));
            SpawnDecor(polish.transform, "BakeryBarrel_1", barrelSpr, new Vector3(3.6f, 6.1f, 0f));
            SpawnDecor(polish.transform, "ShedBarrel_1", barrelSpr, new Vector3(-7.4f, 6.0f, 0f));
            SpawnDecor(polish.transform, "ShedBarrel_2", barrelSpr, new Vector3(-6.6f, 5.6f, 0f));
            SpawnDecor(polish.transform, "GreenhouseBarrel", barrelSpr, new Vector3(-8.2f, 11.4f, 0f));

            // 3. Extra Hero Trees to frame empty meadows
            SpawnDecor(polish.transform, "MeadowBlossom_NW", blossomSpr, new Vector3(-8.5f, 13.8f, 0f));
            SpawnDecor(polish.transform, "MeadowOak_SW", oakSpr, new Vector3(-16.5f, -3.5f, 0f));
            SpawnDecor(polish.transform, "MeadowBlossom_SW", blossomSpr, new Vector3(-12.2f, -8.5f, 0f));
            SpawnDecor(polish.transform, "MeadowOak_SE", oakSpr, new Vector3(13.5f, -4.5f, 0f));
            SpawnDecor(polish.transform, "MeadowBlossom_SE", blossomSpr, new Vector3(11.0f, -10.0f, 0f));
            SpawnDecor(polish.transform, "GardenBlossom_L", blossomSpr, new Vector3(-5.2f, -10.2f, 0f));
            SpawnDecor(polish.transform, "GardenBlossom_R", blossomSpr, new Vector3(5.2f, -10.2f, 0f));

            // 4. Forest Edge Mushroom Clusters
            Vector3[] mushSpots =
            {
                new Vector3(12.2f, 11.2f, 0f),
                new Vector3(16.8f, 10.4f, 0f),
                new Vector3(17.5f, 4.2f, 0f),
                new Vector3(13.4f, 1.8f, 0f),
                new Vector3(16.0f, -1.2f, 0f),
            };
            for (int i = 0; i < mushSpots.Length; i++)
            {
                SpawnDecor(polish.transform, $"ForestMushrooms_{i}", mushSpr, mushSpots[i], 0, "FloorDecal");
            }

            // 5. Bushes, Mossy Rocks & Wildflowers to break up large open grass
            Vector3[] bushSpots =
            {
                new Vector3(-18.5f, 2.5f, 0f), new Vector3(-13.5f, -1.0f, 0f),
                new Vector3(-9.5f, -3.5f, 0f), new Vector3(9.5f, -2.5f, 0f),
                new Vector3(15.2f, -8.2f, 0f), new Vector3(-17.2f, -9.8f, 0f),
                new Vector3(2.8f, -1.6f, 0f), new Vector3(-2.8f, -1.6f, 0f),
            };
            for (int i = 0; i < bushSpots.Length; i++)
            {
                SpawnDecor(polish.transform, $"ExtraBush_{i}", bushSpr, bushSpots[i]);
            }

            Vector3[] rockSpots =
            {
                new Vector3(-11.2f, 13.0f, 0f), new Vector3(-18.2f, -6.2f, 0f),
                new Vector3(11.8f, -7.2f, 0f), new Vector3(17.2f, 7.6f, 0f),
            };
            for (int i = 0; i < rockSpots.Length; i++)
            {
                SpawnDecor(polish.transform, $"ExtraRock_{i}", rockSpr, rockSpots[i]);
            }

            Sprite[] fls = { fl0, fl1, fl2 };
            Vector3[] flowerSpots =
            {
                new Vector3(-14.0f, 2.8f, 0f), new Vector3(-10.5f, 0.2f, 0f),
                new Vector3(-15.5f, -6.5f, 0f), new Vector3(-9.0f, -11.0f, 0f),
                new Vector3(8.8f, 0.5f, 0f), new Vector3(11.5f, -2.2f, 0f),
                new Vector3(14.5f, -10.8f, 0f), new Vector3(-4.2f, -4.5f, 0f),
                new Vector3(4.2f, -4.5f, 0f), new Vector3(-3.8f, -9.2f, 0f),
                new Vector3(3.8f, -9.2f, 0f), new Vector3(-5.0f, 13.5f, 0f),
            };
            for (int i = 0; i < flowerSpots.Length; i++)
            {
                SpawnDecor(polish.transform, $"ExtraFlower_{i}", fls[i % fls.Length], flowerSpots[i], 2, "FloorDecal");
            }

            Debug.Log("[MV] Đã bổ sung cổng vòm hoa, tiểu cảnh làng, nấm rừng và thảm hoa/cây làm đầy map.");
        }
    }
}
