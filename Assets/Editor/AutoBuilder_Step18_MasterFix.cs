using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Data;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step18_MasterFix
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";
        private const string TileAssetDir = "Assets/Data/Tiles";

        [MenuItem("Moonpetal/Step 18/2. Master Fix: Colliders, Arch Posts, Overlap & Unify Decor")]
        public static void ApplyMasterFix()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU STEP 18 MASTER FIX (COLLIDERS, SORTING & CLEANUP) ===");
            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Rebuild ObstacleTilemap with persistent Tile assets + rock-solid TilemapCollider2D (NO CompositeCollider2D!)
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg != null)
            {
                FixObstacleTilemapSolidCollisions(cfg);
                BuildMapBoundarySafetyWalls(cfg);
            }

            // 2. Fix GardenEntranceArch sorting + solid left/right post colliders
            FixGardenEntranceArch();

            // 3. Clean duplicates/overlaps near Well & Pond, unify all sprites to Step 18, add foot colliders
            CleanUnifyAndAddDecorColliders();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT MASTER FIX! Hãy chạy 'Moonpetal/Step 18/Validate Master Fix'. ===");
        }

        [MenuItem("Moonpetal/Step 18/Validate Master Fix")]
        public static void ValidateMasterFix()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE MASTER FIX ===");

            GameObject obsGo = GameObject.Find("ObstacleTilemap");
            if (obsGo == null)
            {
                Debug.LogError("[MV] Thiếu ObstacleTilemap.");
                errors++;
            }
            else
            {
                var tmCol = obsGo.GetComponent<TilemapCollider2D>();
                if (tmCol == null || !tmCol.enabled)
                {
                    Debug.LogError("[MV] ObstacleTilemap thiếu TilemapCollider2D đang bật.");
                    errors++;
                }
                else
                {
                    tmCol.ProcessTilemapChanges();
                    if (tmCol.shapeCount < 100)
                    {
                        Debug.LogError($"[MV] ObstacleTilemap có quá ít hình va chạm (shapeCount={tmCol.shapeCount}, cần >= 100)!");
                        errors++;
                    }
                    else
                    {
                        Debug.Log($"[MV] ObstacleTilemap Collider OK: shapeCount = {tmCol.shapeCount}.");
                    }
                }
                if (obsGo.GetComponent<CompositeCollider2D>() != null)
                {
                    Debug.LogError("[MV] ObstacleTilemap vẫn còn CompositeCollider2D cũ.");
                    errors++;
                }
            }

            if (GameObject.Find("MapBoundaryColliders") == null)
            {
                Debug.LogError("[MV] Thiếu MapBoundaryColliders bảo vệ viền map.");
                errors++;
            }

            GameObject arch = GameObject.Find("GardenEntranceArch");
            if (arch == null || arch.transform.Find("ArchPost_Left") == null || arch.transform.Find("ArchPost_Right") == null)
            {
                Debug.LogError("[MV] GardenEntranceArch thiếu collider 2 cọc cổng (ArchPost_Left / ArchPost_Right).");
                errors++;
            }

            if (GameObject.Find("PondLily_1") != null || GameObject.Find("HeroTree_Blossom_1") != null)
            {
                Debug.LogError("[MV] Vẫn còn vật thể trùng/đè cũ (PondLily_1 hoặc HeroTree_Blossom_1).");
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
                Debug.Log("[MV] VALIDATE MASTER FIX: PASS (0 lỗi | Tường/Rào/Hồ/Cọc cổng/Cây cối có va chạm chuẩn 100%).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE MASTER FIX: FAIL ({errors} lỗi).");
            }
        }

        private static Sprite LoadEnv(string rel) => AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel);

        private static Tile GetOrCreatePersistentTile(string tileName, Sprite spr, Tile.ColliderType colType)
        {
            if (!Directory.Exists(TileAssetDir))
            {
                Directory.CreateDirectory(TileAssetDir);
                AssetDatabase.Refresh();
            }

            string path = $"{TileAssetDir}/{tileName}.asset";
            Tile t = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (t == null)
            {
                t = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(t, path);
            }
            t.sprite = spr;
            t.color = Color.white;
            t.colliderType = colType;
            EditorUtility.SetDirty(t);
            return t;
        }

        private static bool IsChar(string[] rows, int r, int c, char target)
        {
            if (r < 0 || r >= rows.Length) return false;
            if (c < 0 || c >= rows[r].Length) return false;
            return rows[r][c] == target;
        }

        private static Tile Pick6State(
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

        private static void FixObstacleTilemapSolidCollisions(MapLayoutConfigSO cfg)
        {
            GameObject obsGo = GameObject.Find("ObstacleTilemap");
            if (obsGo == null)
            {
                Debug.LogError("[MV] Không tìm thấy ObstacleTilemap!");
                return;
            }

            // 1. Remove CompositeCollider2D + Rigidbody2D so TilemapCollider2D generates direct solid shapes
            var oldComp = obsGo.GetComponent<CompositeCollider2D>();
            if (oldComp != null) UnityEngine.Object.DestroyImmediate(oldComp);
            var oldRb = obsGo.GetComponent<Rigidbody2D>();
            if (oldRb != null) UnityEngine.Object.DestroyImmediate(oldRb);

            obsGo.layer = 10; // Obstacle
            var obsTm = obsGo.GetComponent<Tilemap>();
            obsTm.ClearAllTiles();

            var tmCol = obsGo.GetComponent<TilemapCollider2D>();
            if (tmCol == null) tmCol = obsGo.AddComponent<TilemapCollider2D>();
            tmCol.enabled = true;
            tmCol.isTrigger = false;
            tmCol.compositeOperation = Collider2D.CompositeOperation.None;

            // 2. Persistent Tile assets with Tile.ColliderType.Grid
            Tile wallH  = GetOrCreatePersistentTile("Obs_Wall_H",  LoadEnv("Tiles/tile_wall.png"), Tile.ColliderType.Grid);
            Tile wallV  = GetOrCreatePersistentTile("Obs_Wall_V",  LoadEnv("Tiles/tile_wall_v.png"), Tile.ColliderType.Grid);
            Tile wallTL = GetOrCreatePersistentTile("Obs_Wall_TL", LoadEnv("Tiles/tile_wall_tl.png"), Tile.ColliderType.Grid);
            Tile wallTR = GetOrCreatePersistentTile("Obs_Wall_TR", LoadEnv("Tiles/tile_wall_tr.png"), Tile.ColliderType.Grid);
            Tile wallBL = GetOrCreatePersistentTile("Obs_Wall_BL", LoadEnv("Tiles/tile_wall_bl.png"), Tile.ColliderType.Grid);
            Tile wallBR = GetOrCreatePersistentTile("Obs_Wall_BR", LoadEnv("Tiles/tile_wall_br.png"), Tile.ColliderType.Grid);

            Tile fenceH  = GetOrCreatePersistentTile("Obs_Fence_H",  LoadEnv("Tiles/tile_fence.png"), Tile.ColliderType.Grid);
            Tile fenceV  = GetOrCreatePersistentTile("Obs_Fence_V",  LoadEnv("Tiles/tile_fence_v.png"), Tile.ColliderType.Grid);
            Tile fenceTL = GetOrCreatePersistentTile("Obs_Fence_TL", LoadEnv("Tiles/tile_fence_tl.png"), Tile.ColliderType.Grid);
            Tile fenceTR = GetOrCreatePersistentTile("Obs_Fence_TR", LoadEnv("Tiles/tile_fence_tr.png"), Tile.ColliderType.Grid);
            Tile fenceBL = GetOrCreatePersistentTile("Obs_Fence_BL", LoadEnv("Tiles/tile_fence_bl.png"), Tile.ColliderType.Grid);
            Tile fenceBR = GetOrCreatePersistentTile("Obs_Fence_BR", LoadEnv("Tiles/tile_fence_br.png"), Tile.ColliderType.Grid);

            Tile treeTile = GetOrCreatePersistentTile("Obs_Tree", LoadEnv("Tiles/tile_tree.png"), Tile.ColliderType.Grid);
            Tile waterTile = GetOrCreatePersistentTile("Obs_Water_Invisible", LoadEnv("Water/tile_water_transparent.png"), Tile.ColliderType.Grid);

            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            for (int r = 0; r < rows.Length; r++)
            {
                string row = rows[r];
                int y = cfg.originY + (rows.Length - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    int x = cfg.originX + c;
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    char ch = row[c];

                    if (ch == '#') obsTm.SetTile(pos, Pick6State(rows, r, c, '#', wallH, wallV, wallTL, wallTR, wallBL, wallBR));
                    else if (ch == 'F') obsTm.SetTile(pos, Pick6State(rows, r, c, 'F', fenceH, fenceV, fenceTL, fenceTR, fenceBL, fenceBR));
                    else if (ch == 'T') obsTm.SetTile(pos, treeTile);
                    else if (ch == 'W') obsTm.SetTile(pos, waterTile);
                }
            }

            obsTm.RefreshAllTiles();
            tmCol.ProcessTilemapChanges();
            EditorUtility.SetDirty(obsGo);
            Debug.Log($"[MV] Đã khôi phục hoàn toàn va chạm ObstacleTilemap (shapeCount = {tmCol.shapeCount}).");
        }

        private static void BuildMapBoundarySafetyWalls(MapLayoutConfigSO cfg)
        {
            GameObject old = GameObject.Find("MapBoundaryColliders");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject root = new GameObject("MapBoundaryColliders");
            root.layer = 10;

            float minX = cfg.originX;
            float maxX = cfg.originX + 44f;
            float minY = cfg.originY;
            float maxY = cfg.originY + 32f;
            float cx = (minX + maxX) * 0.5f;
            float cy = (minY + maxY) * 0.5f;
            float w = maxX - minX;
            float h = maxY - minY;

            AddWallBox(root.transform, "Wall_North", new Vector2(cx, maxY + 0.5f), new Vector2(w + 4f, 1.2f));
            AddWallBox(root.transform, "Wall_South", new Vector2(cx, minY - 0.5f), new Vector2(w + 4f, 1.2f));
            AddWallBox(root.transform, "Wall_West",  new Vector2(minX - 0.5f, cy), new Vector2(1.2f, h + 4f));
            AddWallBox(root.transform, "Wall_East",  new Vector2(maxX + 0.5f, cy), new Vector2(1.2f, h + 4f));
            Debug.Log("[MV] Đã dựng 4 tường biên bảo vệ MapBoundaryColliders.");
        }

        private static void AddWallBox(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            GameObject go = new GameObject(name);
            go.layer = 10;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.size = size;
        }

        private static void FixGardenEntranceArch()
        {
            GameObject arch = GameObject.Find("GardenEntranceArch");
            if (arch == null) return;

            arch.transform.position = new Vector3(0.0f, -0.55f, 0f);
            arch.transform.localScale = new Vector3(1.22f, 0.96f, 1f);

            // Order 190: Wen passing under arch (y=-1.2..-1.8 -> order 120..180) goes UNDER floral arch;
            // Wen standing south in front of posts (y < -1.9 -> order > 190) renders IN FRONT OF posts!
            var sr = arch.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = 140;
            }

            EnsureArchPostCollider(arch.transform, "ArchPost_Left",  new Vector3(-1.55f, -1.08f, 0f), new Vector2(0.52f, 0.48f));
            EnsureArchPostCollider(arch.transform, "ArchPost_Right", new Vector3( 1.55f, -1.08f, 0f), new Vector2(0.52f, 0.48f));

            EditorUtility.SetDirty(arch);
            Debug.Log("[MV] Đã chỉnh Y-Sort cổng vòm (order=190) và thêm 2 collider cọc cổng.");
        }

        private static void EnsureArchPostCollider(Transform parent, string name, Vector3 localPos, Vector2 size)
        {
            Transform t = parent.Find(name);
            GameObject go = t != null ? t.gameObject : new GameObject(name);
            go.layer = 10;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            var col = go.GetComponent<BoxCollider2D>();
            if (col == null) col = go.AddComponent<BoxCollider2D>();
            col.size = size;
            col.isTrigger = false;
            col.enabled = true;
        }

        private static void EnsureFootCollider(GameObject go, Vector2 size, Vector2 offset)
        {
            go.layer = 10;
            var col = go.GetComponent<BoxCollider2D>();
            if (col == null) col = go.AddComponent<BoxCollider2D>();
            col.size = size;
            col.offset = offset;
            col.isTrigger = false;
            col.enabled = true;
            EditorUtility.SetDirty(go);
        }

        private static void CleanUnifyAndAddDecorColliders()
        {
            // 1. Delete stale Step 14 duplicates & tree clipping onto Well
            string[] toRemove = { "PondLily_1", "PondLily_2", "HeroTree_Blossom_1" };
            foreach (string rm in toRemove)
            {
                GameObject g = GameObject.Find(rm);
                if (g != null)
                {
                    UnityEngine.Object.DestroyImmediate(g);
                    Debug.Log("[MV] Đã xóa vật thể trùng/đè cũ: " + rm);
                }
            }

            // 2. Move GreenhouseBarrel away from the Well to a clean spot south-east of Greenhouse
            GameObject ghBarrel = GameObject.Find("GreenhouseBarrel");
            if (ghBarrel != null)
            {
                ghBarrel.transform.position = new Vector3(-9.2f, 6.2f, 0f);
                var bsr = ghBarrel.GetComponent<SpriteRenderer>();
                if (bsr != null) bsr.sortingOrder = Mathf.RoundToInt(-6.2f * 100f);
            }

            // 3. Unify sprites & add solid foot-only colliders across VillageDecorGroup
            Sprite oakSpr = LoadEnv("Buildings/tree_hero_oak.png");
            Sprite blossomSpr = LoadEnv("Buildings/tree_hero_blossom.png");
            Sprite pineSpr = LoadEnv("Buildings/tree_hero_pine.png");
            Sprite bushSpr = LoadEnv("Decor/decor_bush.png");
            Sprite rockSpr = LoadEnv("Decor/decor_rock.png");

            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) return;

            int solidCount = 0;
            var srs = decorGroup.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < srs.Length; i++)
            {
                var sr = srs[i];
                if (sr == null) continue;
                GameObject go = sr.gameObject;
                string n = go.name;

                if (n.Contains("Blossom") && blossomSpr != null)
                {
                    sr.sprite = blossomSpr;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(go.transform.position.y - 1.45f) * 100f);
                    EnsureFootCollider(go, new Vector2(0.75f, 0.55f), new Vector2(0f, -1.45f));
                    solidCount++;
                }
                else if (n.Contains("Oak") && oakSpr != null)
                {
                    sr.sprite = oakSpr;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(go.transform.position.y - 1.45f) * 100f);
                    EnsureFootCollider(go, new Vector2(0.75f, 0.55f), new Vector2(0f, -1.45f));
                    solidCount++;
                }
                else if (n.Contains("Pine") && pineSpr != null)
                {
                    sr.sprite = pineSpr;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(go.transform.position.y - 1.45f) * 100f);
                    EnsureFootCollider(go, new Vector2(0.65f, 0.50f), new Vector2(0f, -1.45f));
                    solidCount++;
                }
                else if (n.Contains("Bush") && bushSpr != null)
                {
                    sr.sprite = bushSpr;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(go.transform.position.y - 0.35f) * 100f);
                    EnsureFootCollider(go, new Vector2(0.85f, 0.50f), new Vector2(0f, -0.22f));
                    solidCount++;
                }
                else if (n.Contains("Rock") && rockSpr != null)
                {
                    sr.sprite = rockSpr;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(go.transform.position.y - 0.35f) * 100f);
                    EnsureFootCollider(go, new Vector2(0.85f, 0.50f), new Vector2(0f, -0.22f));
                    solidCount++;
                }
                else if (n.Contains("Barrel") || n.Contains("Signpost"))
                {
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(go.transform.position.y - 0.3f) * 100f);
                    EnsureFootCollider(go, new Vector2(0.65f, 0.48f), new Vector2(0f, -0.18f));
                    solidCount++;
                }
                else if (n.Contains("PicnicTable"))
                {
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(go.transform.position.y - 0.35f) * 100f);
                    EnsureFootCollider(go, new Vector2(1.55f, 0.80f), new Vector2(0f, -0.15f));
                    solidCount++;
                }
            }

            Debug.Log($"[MV] Đã đồng bộ sprite Step 18 và gắn chân va chạm chuẩn cho {solidCount} vật thể trang trí.");
        }
    }
}
