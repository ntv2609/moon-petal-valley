using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Data;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step17
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";

        [MenuItem("Moonpetal/Step 17/1. Apply Organic Terrain (16-State Rounded Paths, Curved Pond & Pier)")]
        public static void BuildStep17()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 17 (CHẶNG 1 - ĐỊA HÌNH HỮU CƠ) ===");
            AssetDatabase.Refresh();
            ForceReimportEnvironmentFolder();
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Update MapLayoutConfig with organic curved pond basin (6x4) & clean connected paths
            MapLayoutConfigSO cfg = UpdateMapConfigForOrganicPond();
            if (cfg != null)
            {
                RebuildOrganicTilemap(cfg);
                BuildSeamlessAnimatedPond();
                BuildOrganicSteppingStonesAndShoreline();
            }

            // 2. Keep GardenEntranceArch flush with fence
            GameObject arch = GameObject.Find("GardenEntranceArch");
            if (arch != null)
            {
                arch.transform.position = new Vector3(0.0f, -0.55f, 0f);
                arch.transform.localScale = new Vector3(1.22f, 0.96f, 1f);
                EditorUtility.SetDirty(arch);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 17 (CHẶNG 1)! Chạy 'Moonpetal/Step 17/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 17/Validate")]
        public static void ValidateStep17()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 17 ===");

            for (int m = 0; m < 16; m++)
            {
                string p = $"{EnvRoot}Paths/path_m{m:D2}.png";
                if (AssetDatabase.LoadAssetAtPath<Sprite>(p) == null)
                {
                    Debug.LogError("[MV] Thiếu path auto-tile: " + p);
                    errors++;
                }
            }
            for (int f = 0; f < 4; f++)
            {
                string p = $"{EnvRoot}Water/pond_organic_{f:D2}.png";
                if (AssetDatabase.LoadAssetAtPath<Sprite>(p) == null)
                {
                    Debug.LogError("[MV] Thiếu pond organic frame: " + p);
                    errors++;
                }
            }

            var waterAnim = UnityEngine.Object.FindAnyObjectByType<WaterTileAnimator>();
            if (waterAnim == null)
            {
                Debug.LogError("[MV] Thiếu WaterTileAnimator trên AnimatedPondWater.");
                errors++;
            }

            GameObject obsGo = GameObject.Find("ObstacleTilemap");
            if (obsGo == null || obsGo.GetComponent<CompositeCollider2D>() == null)
            {
                Debug.LogError("[MV] ObstacleTilemap thiếu CompositeCollider2D.");
                errors++;
            }

            if (GameObject.Find("Step17OrganicShoreline") == null)
            {
                Debug.LogError("[MV] Thiếu nhóm Step17OrganicShoreline (cầu gỗ, lau sậy & đá bước chân).");
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
                Debug.Log("[MV] VALIDATE STEP 17: PASS (0 lỗi | 16-State Rounded Paths=OK | Curved Organic Pond 4f=OK | CompositeCollider2D=OK).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 17: FAIL ({errors} lỗi).");
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

        private static Tile MakeTile(Sprite spr, Tile.ColliderType colType = Tile.ColliderType.Grid)
        {
            var t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.color = Color.white;
            t.colliderType = colType;
            return t;
        }

        private static MapLayoutConfigSO UpdateMapConfigForOrganicPond()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null)
            {
                Debug.LogError("[MV] Không tìm thấy MapLayoutConfig.asset!");
                return null;
            }

            // 32 rows x 44 cols (originX = -22, originY = -16)
            // Organic 6x4 curved pond basin at rows 1..4, cols 16..21 (x = -6..-1, y = 11..14)
            // Completely clear of Greenhouse (cols 1..12), Well (~x=-8,y=10.5), Village paths (row 7+) & Forest Edge
            string[] rows =
            {
                "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT",
                "T############....WWWW........TTTTTTTTTTTTTTT",
                "T#GGGGGGGGGG#...WWWWWW.......TT..........TTT",
                "T#GGGGGGGGGG#...WWWWWW.......T............TT",
                "T#GGGGGGGGGG#....WWWW........T....TTTT.....T",
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
                "T..............FFFFFPPPPFFFFF..............T",
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
            EditorUtility.SetDirty(cfg);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] Đã cập nhật MapLayoutConfig.asset với hồ nước cong 6x4 và trục đường thông thoáng.");
            return cfg;
        }

        private static bool IsChar(string[] rows, int r, int c, char target)
        {
            if (r < 0 || r >= rows.Length) return false;
            if (c < 0 || c >= rows[r].Length) return false;
            return rows[r][c] == target;
        }

        private static bool IsPathOrConnected(string[] rows, int r, int c)
        {
            if (r < 0 || r >= rows.Length) return false;
            if (c < 0 || c >= rows[r].Length) return false;
            char ch = rows[r][c];
            return ch == 'P' || ch == 'B' || ch == 'G';
        }

        private static Tile Pick6StateWallOrFence(
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

        private static void RebuildOrganicTilemap(MapLayoutConfigSO cfg)
        {
            Tile[] grassTiles = new Tile[4];
            for (int i = 0; i < 4; i++) grassTiles[i] = MakeTile(LoadEnv($"Tiles/tile_grass_{i}.png"));

            Tile[] pathMasks = new Tile[16];
            for (int m = 0; m < 16; m++) pathMasks[m] = MakeTile(LoadEnv($"Paths/path_m{m:D2}.png"));

            Tile[] ghTiles =
            {
                MakeTile(LoadEnv("Tiles/tile_gh_floor_0.png")),
                MakeTile(LoadEnv("Tiles/tile_gh_floor_1.png")),
            };

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
            Tile flowerbedTile = MakeTile(LoadEnv("Tiles/tile_flowerbed.png"));
            Tile waterColliderTile = MakeTile(LoadEnv("Water/tile_water_transparent.png"), Tile.ColliderType.Grid);

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
            var tmCol = obsGo.AddComponent<TilemapCollider2D>();
            var rb = obsGo.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            var compCol = obsGo.AddComponent<CompositeCollider2D>();
            compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
            compCol.generationType = CompositeCollider2D.GenerationType.Synchronous;
            tmCol.compositeOperation = Collider2D.CompositeOperation.Merge;

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
                            int mask = 0;
                            if (IsPathOrConnected(rows, r - 1, c)) mask |= 1;
                            if (IsPathOrConnected(rows, r, c + 1)) mask |= 2;
                            if (IsPathOrConnected(rows, r + 1, c)) mask |= 4;
                            if (IsPathOrConnected(rows, r, c - 1)) mask |= 8;
                            groundTm.SetTile(pos, pathMasks[mask]);
                            break;
                        case 'G':
                            groundTm.SetTile(pos, ghTiles[hash % ghTiles.Length]);
                            break;
                        case 'B':
                            groundTm.SetTile(pos, flowerbedTile);
                            break;
                        case '#':
                            groundTm.SetTile(pos, ghTiles[0]);
                            obsTm.SetTile(pos, Pick6StateWallOrFence(rows, r, c, '#', wallH, wallV, wallTL, wallTR, wallBL, wallBR));
                            break;
                        case 'F':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, Pick6StateWallOrFence(rows, r, c, 'F', fenceH, fenceV, fenceTL, fenceTR, fenceBL, fenceBR));
                            break;
                        case 'T':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, treeTile);
                            break;
                        case 'W':
                            // Seamless grass underneath + transparent collider so 96x64 curved pond renders smoothly!
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, waterColliderTile);
                            break;
                        default:
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            break;
                    }
                }
            }

            compCol.GenerateGeometry();
            Debug.Log("[MV] Đã dựng WorldMapGrid hữu cơ với 16-state Rounded Paths + CompositeCollider2D.");
        }

        private static void BuildSeamlessAnimatedPond()
        {
            Sprite[] frames =
            {
                LoadEnv("Water/pond_organic_00.png"),
                LoadEnv("Water/pond_organic_01.png"),
                LoadEnv("Water/pond_organic_02.png"),
                LoadEnv("Water/pond_organic_03.png"),
            };
            if (frames[0] == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy Water/pond_organic_00..03.png.");
                return;
            }

            GameObject oldWater = GameObject.Find("AnimatedPondWater");
            if (oldWater != null) UnityEngine.Object.DestroyImmediate(oldWater);

            GameObject root = new GameObject("AnimatedPondWater");
            // Center of cols 16..21 (x=-6..-1 -> center -3.0) and rows 1..4 (y=11..14 -> center 13.0)
            root.transform.position = new Vector3(-3.0f, 13.0f, 0f);

            var sr = root.AddComponent<SpriteRenderer>();
            sr.sprite = frames[0];
            sr.sortingLayerName = "FloorDecal";
            sr.sortingOrder = 5;

            var anim = root.AddComponent<WaterTileAnimator>();
            var so = new SerializedObject(anim);
            var fProp = so.FindProperty("waterFrames");
            fProp.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++) fProp.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            var rProp = so.FindProperty("waterRenderers");
            rProp.arraySize = 1;
            rProp.GetArrayElementAtIndex(0).objectReferenceValue = sr;
            so.FindProperty("fps").floatValue = 3.2f;
            so.ApplyModifiedPropertiesWithoutUndo();

            Debug.Log("[MV] Đã dựng hồ nước cong liền khối 96x64 (4 frame động) tại (-3.0, 13.0).");
        }

        private static void SpawnDecal(Transform parent, string name, Sprite spr, Vector3 pos, string layer = "FloorDecal", int order = 6)
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = layer;
            sr.sortingOrder = order;
        }

        private static void BuildOrganicSteppingStonesAndShoreline()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) decorGroup = new GameObject("VillageDecorGroup");

            Transform old = decorGroup.transform.Find("Step17OrganicShoreline");
            if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);

            GameObject group = new GameObject("Step17OrganicShoreline");
            group.transform.SetParent(decorGroup.transform, false);

            Sprite pierSpr = LoadEnv("Decor/decor_pond_pier.png");
            Sprite reedSpr = LoadEnv("Decor/decor_reeds.png");
            Sprite st0 = LoadEnv("Decor/decor_stepping_0.png");
            Sprite st1 = LoadEnv("Decor/decor_stepping_1.png");

            // Wooden Pond Pier at south shoreline of curved pond
            SpawnDecal(group.transform, "PondWoodenPier", pierSpr, new Vector3(-3.0f, 10.95f, 0f), "FloorDecal", 8);

            // Shoreline cattail reeds framing the curved pond banks
            SpawnDecal(group.transform, "PondReeds_NW", reedSpr, new Vector3(-5.8f, 14.3f, 0f), "WorldObjects", Mathf.RoundToInt(-14.3f * 100f));
            SpawnDecal(group.transform, "PondReeds_NE", reedSpr, new Vector3(-0.3f, 14.0f, 0f), "WorldObjects", Mathf.RoundToInt(-14.0f * 100f));
            SpawnDecal(group.transform, "PondReeds_SE", reedSpr, new Vector3(-0.4f, 11.4f, 0f), "WorldObjects", Mathf.RoundToInt(-11.4f * 100f));

            // Organic stepping stone connectors (Well -> Pond Pier, Pond Pier -> Village Path, Village -> Forest Edge, Greenhouse -> Well)
            Vector3[] stones =
            {
                new Vector3(-7.0f, 10.6f, 0f),
                new Vector3(-5.6f, 10.5f, 0f),
                new Vector3(-4.4f, 10.4f, 0f),
                new Vector3(-3.0f, 9.8f, 0f),
                new Vector3(-2.6f, 9.0f, 0f),
                new Vector3(-13.2f, 4.4f, 0f),
                new Vector3(-11.8f, 5.2f, 0f),
                new Vector3(-10.4f, 6.2f, 0f),
                new Vector3(-9.2f, 7.6f, 0f),
                new Vector3(-8.4f, 9.0f, 0f),
                new Vector3(9.4f, 7.5f, 0f),
                new Vector3(10.6f, 7.6f, 0f),
                new Vector3(11.8f, 7.4f, 0f),
                new Vector3(13.0f, 7.2f, 0f),
            };
            for (int i = 0; i < stones.Length; i++)
            {
                SpawnDecal(group.transform, $"SteppingStone_{i}", (i % 2 == 0) ? st0 : st1, stones[i], "FloorDecal", 4);
            }

            Debug.Log("[MV] Đã bố trí cầu gỗ ven hồ, lau sậy và 14 cụm đá bước chân kết nối mềm giữa các phân khu.");
        }
    }
}
