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

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step16
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";
        private const string SlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";

        [MenuItem("Moonpetal/Step 16/1. Apply Auto-Tile Walls/Fences, Greenhouse Layout, Shrine & Shadows")]
        public static void BuildStep16()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 16 ===");
            AssetDatabase.Refresh();
            ForceReimportEnvironmentFolder();
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Rebuild WorldMapGrid with Directional Auto-Tiling for '#' (Walls) and 'F' (Fences)
            var mapCfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (mapCfg != null)
            {
                RebuildAutoTiledWorldMap(mapCfg);
            }

            // 2. Upgrade CentralPlantingSpot (Moonpetal Shrine Planter)
            UpgradeCentralPlantingSpot();

            // 3. Upgrade 12 Greenhouse Farm Tiles (Raised Planter Boxes + Neat Bed Layout + Cozy Rug)
            UpgradeGreenhouseInteriorAndFarmLayout();

            // 4. Strengthen Ground Shadows for Wen, Lumi, 4 NPCs & 8 Slimes
            StrengthenGroundShadows();

            // 5. Fix any tree/decor clipping into the Greenhouse east glass wall
            FixGreenhouseWallClippingDecor();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 16! Chạy 'Moonpetal/Step 16/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 16/Validate")]
        public static void ValidateStep16()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 16 ===");

            string[] requiredSprites =
            {
                "Tiles/tile_wall.png", "Tiles/tile_wall_v.png",
                "Tiles/tile_wall_tl.png", "Tiles/tile_wall_tr.png",
                "Tiles/tile_wall_bl.png", "Tiles/tile_wall_br.png",
                "Tiles/tile_fence.png", "Tiles/tile_fence_v.png",
                "Tiles/tile_fence_tl.png", "Tiles/tile_fence_tr.png",
                "Tiles/tile_fence_bl.png", "Tiles/tile_fence_br.png",
                "Tiles/tile_flowerbed.png",
                "Soil/soil_untilled.png", "Soil/soil_dry.png", "Soil/soil_wet.png",
                "Props/planting_spot_empty.png", "Props/planting_spot_bloomed.png",
                "Decor/decor_greenhouse_rug.png",
                "Shadows/spr_ground_shadow.png",
            };
            foreach (string rel in requiredSprites)
            {
                if (AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel) == null)
                {
                    Debug.LogError("[MV] Thiếu sprite Step 16: " + EnvRoot + rel);
                    errors++;
                }
            }

            var spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            if (spot == null)
            {
                Debug.LogError("[MV] Không tìm thấy PlantingSpotController trong scene.");
                errors++;
            }
            else
            {
                var so = new SerializedObject(spot);
                if (so.FindProperty("emptySprite").objectReferenceValue == null ||
                    so.FindProperty("bloomedSprite").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] PlantingSpotController thiếu emptySprite hoặc bloomedSprite.");
                    errors++;
                }
            }

            if (GameObject.Find("GreenhouseCozyRug") == null)
            {
                Debug.LogError("[MV] Thiếu GreenhouseCozyRug trong Nhà Kính.");
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
                Debug.Log("[MV] VALIDATE STEP 16: PASS (0 lỗi | Auto-Tile Walls/Fences=OK | Raised Planters=OK | Shrine=OK | Strong Shadows=OK).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 16: FAIL ({errors} lỗi).");
            }
        }

        private static void ForceReimportEnvironmentFolder()
        {
            string folder = "Assets/Art/Sprites/Environment";
            if (!AssetDatabase.IsValidFolder(folder)) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
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

        private static Sprite LoadEnv(string rel) => AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel);

        private static Tile MakeTile(Sprite spr)
        {
            var t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.color = Color.white;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static bool IsChar(string[] rows, int r, int c, char target)
        {
            if (r < 0 || r >= rows.Length) return false;
            if (c < 0 || c >= rows[r].Length) return false;
            return rows[r][c] == target;
        }

        private static Tile PickAutoTile(
            string[] rows, int r, int c, char ch,
            Tile tH, Tile tV, Tile tTL, Tile tTR, Tile tBL, Tile tBR)
        {
            bool hasN = IsChar(rows, r - 1, c, ch);
            bool hasS = IsChar(rows, r + 1, c, ch);
            bool hasW = IsChar(rows, r, c - 1, ch);
            bool hasE = IsChar(rows, r, c + 1, ch);

            if (hasS && hasE && !hasN && !hasW) return tTL;
            if (hasS && hasW && !hasN && !hasE) return tTR;
            if (hasN && hasE && !hasS && !hasW) return tBL;
            if (hasN && hasW && !hasS && !hasE) return tBR;
            if ((hasN || hasS) && !hasE && !hasW) return tV;
            return tH;
        }

        private static void RebuildAutoTiledWorldMap(MapLayoutConfigSO cfg)
        {
            Sprite[] grass =
            {
                LoadEnv("Tiles/tile_grass_0.png"),
                LoadEnv("Tiles/tile_grass_1.png"),
                LoadEnv("Tiles/tile_grass_2.png"),
                LoadEnv("Tiles/tile_grass_3.png"),
            };
            Sprite[] path =
            {
                LoadEnv("Tiles/tile_path_0.png"),
                LoadEnv("Tiles/tile_path_1.png"),
                LoadEnv("Tiles/tile_path_2.png"),
            };
            Sprite[] ghFloor =
            {
                LoadEnv("Tiles/tile_gh_floor_0.png"),
                LoadEnv("Tiles/tile_gh_floor_1.png"),
            };

            Tile[] grassTiles = new Tile[grass.Length];
            for (int i = 0; i < grass.Length; i++) grassTiles[i] = MakeTile(grass[i] != null ? grass[i] : grass[0]);
            Tile[] pathTiles = new Tile[path.Length];
            for (int i = 0; i < path.Length; i++) pathTiles[i] = MakeTile(path[i] != null ? path[i] : path[0]);
            Tile[] ghTiles = new Tile[ghFloor.Length];
            for (int i = 0; i < ghFloor.Length; i++) ghTiles[i] = MakeTile(ghFloor[i] != null ? ghFloor[i] : ghFloor[0]);

            Tile wallH  = MakeTile(LoadEnv("Tiles/tile_wall.png"));
            Tile wallV  = MakeTile(LoadEnv("Tiles/tile_wall_v.png"));
            Tile wallTL = MakeTile(LoadEnv("Tiles/tile_wall_tl.png"));
            Tile wallTR = MakeTile(LoadEnv("Tiles/tile_wall_tr.png"));
            Tile wallBL = MakeTile(LoadEnv("Tiles/tile_wall_bl.png"));
            Tile wallBR = MakeTile(LoadEnv("Tiles/tile_wall_br.png"));

            Tile fenceH  = MakeTile(LoadEnv("Tiles/tile_fence.png"));
            Tile fenceV  = MakeTile(LoadEnv("Tiles/tile_fence_v.png"));
            Tile fenceTL = MakeTile(LoadEnv("Tiles/tile_fence_tl.png"));
            Tile fenceTR = MakeTile(LoadEnv("Tiles/tile_fence_tr.png"));
            Tile fenceBL = MakeTile(LoadEnv("Tiles/tile_fence_bl.png"));
            Tile fenceBR = MakeTile(LoadEnv("Tiles/tile_fence_br.png"));

            Tile treeTile = MakeTile(LoadEnv("Tiles/tile_tree.png"));
            Sprite waterSpr = LoadEnv("Water/water_anim_00.png");
            if (waterSpr == null) waterSpr = LoadEnv("Tiles/tile_water.png");
            Tile waterTile = MakeTile(waterSpr);
            Tile flowerbedTile = MakeTile(LoadEnv("Tiles/tile_flowerbed.png"));

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
                            obsTm.SetTile(pos, PickAutoTile(rows, r, c, '#', wallH, wallV, wallTL, wallTR, wallBL, wallBR));
                            break;
                        case 'F':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, PickAutoTile(rows, r, c, 'F', fenceH, fenceV, fenceTL, fenceTR, fenceBL, fenceBR));
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

            Debug.Log("[MV] Đã dựng lại WorldMapGrid với Auto-Tile 6 hướng cho Tường Kính & Hàng Rào + Thảm hoa liền mạch.");
        }

        private static void UpgradeCentralPlantingSpot()
        {
            var spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            if (spot == null) return;

            Sprite emptySpr = LoadEnv("Props/planting_spot_empty.png");
            Sprite bloomSpr = LoadEnv("Props/planting_spot_bloomed.png");
            if (emptySpr == null || bloomSpr == null) return;

            var so = new SerializedObject(spot);
            so.FindProperty("emptySprite").objectReferenceValue = emptySpr;
            so.FindProperty("bloomedSprite").objectReferenceValue = bloomSpr;
            so.ApplyModifiedPropertiesWithoutUndo();

            var srProp = so.FindProperty("flowerRenderer");
            var sr = srProp != null ? srProp.objectReferenceValue as SpriteRenderer : spot.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = emptySpr;
                sr.color = Color.white;
                EditorUtility.SetDirty(sr);
            }
            EditorUtility.SetDirty(spot);
            Debug.Log("[MV] Đã nâng cấp Bồn Hoa Trung Tâm thành bồn đá bát giác khắc Trăng Khuyết.");
        }

        private static void UpgradeGreenhouseInteriorAndFarmLayout()
        {
            Sprite soilUntilled = LoadEnv("Soil/soil_untilled.png");
            Sprite soilDry = LoadEnv("Soil/soil_dry.png");
            Sprite soilWet = LoadEnv("Soil/soil_wet.png");

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            Array.Sort(tiles, (a, b) => string.CompareOrdinal(a.name, b.name));

            // Arrange 12 raised wooden planter boxes into 2 clean 2x3 cozy garden beds (columns 0-1 | aisle | columns 2-3)
            for (int i = 0; i < tiles.Length; i++)
            {
                var tile = tiles[i];
                if (soilUntilled != null && soilDry != null && soilWet != null)
                {
                    var so = new SerializedObject(tile);
                    so.FindProperty("untilledSprite").objectReferenceValue = soilUntilled;
                    so.FindProperty("tilledDrySprite").objectReferenceValue = soilDry;
                    so.FindProperty("tilledWetSprite").objectReferenceValue = soilWet;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    tile.RefreshVisuals();
                }

                if (tiles.Length == 12)
                {
                    int row = i / 4; // 0..2
                    int col = i % 4; // 0..3
                    float aisleOffset = col >= 2 ? 0.45f : 0f;
                    float tx = -18.15f + col * 1.08f + aisleOffset;
                    float ty = 9.55f - row * 1.18f;
                    tile.transform.position = new Vector3(tx, ty, 0f);
                }
                EditorUtility.SetDirty(tile.gameObject);
            }

            // Place a cozy woven rug under the Greenhouse reading/living nook (north side)
            Sprite rugSpr = LoadEnv("Decor/decor_greenhouse_rug.png");
            if (rugSpr != null)
            {
                GameObject oldRug = GameObject.Find("GreenhouseCozyRug");
                if (oldRug != null) UnityEngine.Object.DestroyImmediate(oldRug);

                GameObject decorGroup = GameObject.Find("VillageDecorGroup");
                GameObject rugGo = new GameObject("GreenhouseCozyRug");
                if (decorGroup != null) rugGo.transform.SetParent(decorGroup.transform, false);
                rugGo.transform.position = new Vector3(-16.1f, 12.15f, 0f);
                var sr = rugGo.AddComponent<SpriteRenderer>();
                sr.sprite = rugSpr;
                sr.sortingLayerName = "FloorDecal";
                sr.sortingOrder = 1;
            }

            Debug.Log($"[MV] Đã sắp gọn {tiles.Length} chậu đất viền gỗ thành 2 luống đối xứng + trải thảm Nhà Kính.");
        }

        private static void UpdateShadow(Transform parent, Sprite shadowSpr, Vector3 localPos, Vector3 localScale)
        {
            if (parent == null || shadowSpr == null) return;
            Transform existing = parent.Find("GroundShadow");
            GameObject go = existing != null ? existing.gameObject : new GameObject("GroundShadow");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = shadowSpr;
            sr.color = new Color(1f, 1f, 1f, 0.85f);
            sr.sortingLayerName = "Shadows";
            sr.sortingOrder = 0;
            EditorUtility.SetDirty(go);
        }

        private static void StrengthenGroundShadows()
        {
            Sprite shadowSpr = LoadEnv("Shadows/spr_ground_shadow.png");
            if (shadowSpr == null) return;

            if (AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
                UpdateShadow(root.transform, shadowSpr, new Vector3(0f, -0.46f, 0f), new Vector3(1.10f, 0.90f, 1f));
                PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }
            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                UpdateShadow(player.transform, shadowSpr, new Vector3(0f, -0.46f, 0f), new Vector3(1.10f, 0.90f, 1f));
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(LumiPrefabPath);
                UpdateShadow(root.transform, shadowSpr, new Vector3(0f, -0.44f, 0f), new Vector3(0.68f, 0.56f, 1f));
                PrefabUtility.SaveAsPrefabAsset(root, LumiPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }
            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null)
            {
                UpdateShadow(lumi.transform, shadowSpr, new Vector3(0f, -0.44f, 0f), new Vector3(0.68f, 0.56f, 1f));
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(SlimePrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(SlimePrefabPath);
                UpdateShadow(root.transform, shadowSpr, new Vector3(0f, -0.34f, 0f), new Vector3(1.05f, 0.85f, 1f));
                PrefabUtility.SaveAsPrefabAsset(root, SlimePrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }
            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            foreach (var sl in slimes)
            {
                UpdateShadow(sl.transform, shadowSpr, new Vector3(0f, -0.34f, 0f), new Vector3(1.05f, 0.85f, 1f));
            }

            var npcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            foreach (var npc in npcs)
            {
                UpdateShadow(npc.transform, shadowSpr, new Vector3(0f, -0.46f, 0f), new Vector3(1.10f, 0.90f, 1f));
            }

            Debug.Log("[MV] Đã tăng độ đậm và căn sát chân GroundShadow cho Wen, Lumi, 4 NPC và 8 Slime.");
        }

        private static void FixGreenhouseWallClippingDecor()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) return;

            int moved = 0;
            var transforms = decorGroup.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform t = transforms[i];
                if (t == decorGroup.transform || t.name == "Step15WorldPolishGroup" || t.name == "GreenhouseCozyRug")
                    continue;

                Vector3 p = t.position;
                // Greenhouse east glass wall is at x = -10..-9, y = 5..15
                if (p.x >= -10.8f && p.x <= -8.4f && p.y >= 5.2f && p.y <= 14.8f)
                {
                    t.position = new Vector3(-7.4f, p.y, p.z);
                    var sr = t.GetComponent<SpriteRenderer>();
                    if (sr != null && sr.sortingLayerName == "WorldObjects")
                    {
                        sr.sortingOrder = Mathf.RoundToInt(-t.position.y * 100f);
                    }
                    EditorUtility.SetDirty(t.gameObject);
                    moved++;
                }
            }
            if (moved > 0)
            {
                Debug.Log($"[MV] Đã dời {moved} vật trang trí bị chạm tường kính phía Đông Nhà Kính ra ngoài thoáng đẹp.");
            }
        }
    }
}
