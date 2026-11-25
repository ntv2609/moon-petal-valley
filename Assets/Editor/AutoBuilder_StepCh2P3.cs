using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Core;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2P3
    {
        private const string RiverbankScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string Ch2TerrainRoot = "Assets/Art/Sprites/Chapter2/Terrain";
        private const string Ch2TilesRoot = "Assets/Data/Tiles/Chapter2";
        private const string Ch2SpritesRoot = "Assets/Art/Sprites/Chapter2";
        private const string Ch2OverhaulRoot = "Assets/Art/Sprites/Chapter2/Overhaul";
        private const string EnvTreesRoot = "Assets/Art/Sprites/Environment/Trees";
        private const string EnvDecorRoot = "Assets/Art/Sprites/Environment/Decor";
        private const string EnvPropsRoot = "Assets/Art/Sprites/Environment/Props";

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P3 - 1. Rebuild Mara West Bank & Relocate Otis Desk")]
        public static void ApplyStepCh2P3()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(RiverbankScenePath))
            {
                Debug.LogError("[MV][Ch2-P3] Khong tim thay scene: " + RiverbankScenePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(RiverbankScenePath, OpenSceneMode.Single);

            // 1. Paint seamless 16-state West Bank terrain & road network
            RebuildWestBankTilemaps();

            // 2. Clean old West Bank props & rebuild cohesive Mara's Yard + Forest Archway
            RebuildWestBankLayoutAndDecor();

            // 3. Align Portal_ToWestSwamp, SwampMistBarrier, Guide Lanterns & Spawn_WestSwamp to x = -14.5
            AlignSwampEntranceGate();

            // 4. Relocate Clue_OtisNote onto East Nursery porch (21.2, 3.2)
            RelocateOtisNotebookDesk();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Ch2-P3] Hoan tat Step Ch2-P3 (Bo Tay nha ba Mara noi khop 1:1 cong Dam Lay + Ban so bac Otis sat Vuon Uom Dong).");
        }

        private static bool IsWestPathCell(int x, int y)
        {
            // Chi xu ly Bo Tay (x = -25..-9)
            if (x < -25 || x > -9 || y < -13 || y > 13) return false;

            // 1. Duong noi dau cau Tay (x = -14..-9, y = -1..+1) -> khop 100% RiverBridge (y = -1..+1)
            if (x >= -14 && x <= -9 && y >= -1 && y <= 1) return true;

            // 2. Nhanh xuong Ben Do Tay (x = -12..-10, y = -8..-1) -> khop FerryBoat_West (-9.2, -7.5) & Spawn_WestDock (-10.5, -6.5)
            if (x >= -12 && x <= -10 && y >= -8 && y <= -1) return true;

            // 3. San nha ba Mara am cung (x = -17..-12, y = -6..-4) & loi vao cua nha (-16..-14, y = -4..-3)
            if (x >= -17 && x <= -12 && y >= -6 && y <= -4) return true;
            if (x >= -16 && x <= -14 && y >= -4 && y <= -3) return true;

            // 4. Duong rung uon mem tu nga ba cau len Cong Dam Lay Bac (khop 1:1 4-tile-wide x = -16..-13, y = 6..13)
            if (x >= -14 && x <= -12 && y >= 1 && y <= 4) return true;
            if (x >= -15 && x <= -12 && y >= 4 && y <= 5) return true;
            if (x >= -16 && x <= -13 && y >= 5 && y <= 13) return true;

            return false;
        }

        private static void RebuildWestBankTilemaps()
        {
            Tilemap groundTm = FindTilemapByName("GroundTilemap");
            Tilemap obsTm = FindTilemapByName("ObstacleTilemap");
            if (groundTm == null)
            {
                Debug.LogError("[MV][Ch2-P3] Khong tim thay GroundTilemap!");
                return;
            }

            if (!Directory.Exists(Ch2TilesRoot))
                Directory.CreateDirectory(Ch2TilesRoot);

            Tile[] westGrass = new Tile[4];
            for (int i = 0; i < 4; i++)
                westGrass[i] = GetOrCreateTile($"{Ch2TilesRoot}/Ch2_GrassWest_{i}.asset", $"{Ch2TerrainRoot}/ch2_grass_west_{i}.png");

            Tile[] westPath = new Tile[16];
            for (int m = 0; m < 16; m++)
                westPath[m] = GetOrCreateTile($"{Ch2TilesRoot}/Ch2_PathWest_m{m:D2}.asset", $"{Ch2TerrainRoot}/ch2_path_west_m{m:D2}.png");

            Tile tTreeBorder = obsTm != null ? GetExistingObstacleTile(obsTm) : null;

            for (int y = -14; y <= 13; y++)
            {
                for (int x = -26; x <= -9; x++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    bool isNorthExitOpening = (y >= 12 && x >= -16 && x <= -13);
                    bool isBorder = (x == -26 || y == -14 || (y == 13 && !isNorthExitOpening));

                    if (IsWestPathCell(x, y))
                    {
                        int mask = 0;
                        // North: neu y==13 trong cua ra Dam Lay, coi phia bac van la duong de mep khong bi dong nap!
                        if (y == 13 && x >= -16 && x <= -13) mask |= 1;
                        else if (IsWestPathCell(x, y + 1)) mask |= 1;

                        // East: tai dau cau (x==-9, y=-1..1) noi lien vao cau nen coi phia Dong co duong
                        if (x == -9 && y >= -1 && y <= 1) mask |= 2;
                        else if (IsWestPathCell(x + 1, y)) mask |= 2;

                        if (IsWestPathCell(x, y - 1)) mask |= 4;
                        if (IsWestPathCell(x - 1, y)) mask |= 8;

                        groundTm.SetTile(cell, westPath[mask] != null ? westPath[mask] : westGrass[0]);
                        if (obsTm != null) obsTm.SetTile(cell, null);
                    }
                    else
                    {
                        int hash = Mathf.Abs(x * 73856093 ^ y * 19349663) % 4;
                        groundTm.SetTile(cell, westGrass[hash] != null ? westGrass[hash] : westGrass[0]);
                        if (obsTm != null)
                        {
                            if (isBorder && tTreeBorder != null)
                                obsTm.SetTile(cell, tTreeBorder);
                            else if (!isBorder)
                                obsTm.SetTile(cell, null);
                        }
                    }
                }
            }

            EditorUtility.SetDirty(groundTm);
            if (obsTm != null) EditorUtility.SetDirty(obsTm);
        }

        private static void RebuildWestBankLayoutAndDecor()
        {
            // 1. Di chuyen House_MaraDock & NPC_Mara ve san moi hai hoa (khong chan duong len Dam Lay)
            GameObject maraHouse = GameObject.Find("House_MaraDock");
            if (maraHouse != null)
            {
                maraHouse.transform.position = new Vector3(-15.5f, -1.6f, 0f);
                EditorUtility.SetDirty(maraHouse);
            }

            GameObject maraNpc = GameObject.Find("NPC_Mara");
            if (maraNpc != null)
            {
                maraNpc.transform.position = new Vector3(-14.6f, -4.6f, 0f);
                EditorUtility.SetDirty(maraNpc);
            }

            // Can chinh lai Clue_MudFootprints tren con duong Bo Tay cho tu nhien
            GameObject mudClue = GameObject.Find("Clue_MudFootprints");
            if (mudClue != null)
            {
                mudClue.transform.position = new Vector3(-11.2f, -2.6f, 0f);
                EditorUtility.SetDirty(mudClue);
            }

            // 2. Don sach cac vat trang tri cu nam tren Bo Tay (x < -8.6) trong Step43_WorldDecor de tranh cay moc giua duong moi
            GameObject oldDecorRoot = GameObject.Find("Step43_WorldDecor");
            if (oldDecorRoot != null)
            {
                var toDelete = new List<GameObject>();
                for (int i = 0; i < oldDecorRoot.transform.childCount; i++)
                {
                    Transform ch = oldDecorRoot.transform.GetChild(i);
                    if (ch.position.x < -8.6f && !ch.name.Contains("Pier_WestFerry"))
                        toDelete.Add(ch.gameObject);
                }
                for (int i = 0; i < toDelete.Count; i++)
                    UnityEngine.Object.DestroyImmediate(toDelete[i]);
            }

            // 3. Dung lai he thong cay coi, hang rao, den, nam & lau say Bo Tay tinh te trong WestBank_Ch2P3_Decor
            DestroyIfExists("WestBank_Ch2P3_Decor");
            GameObject root = new GameObject("WestBank_Ch2P3_Decor");

            Sprite sprPine = AssetDatabase.LoadAssetAtPath<Sprite>(EnvTreesRoot + "/tree_hero_pine.png");
            Sprite sprOak = AssetDatabase.LoadAssetAtPath<Sprite>(EnvTreesRoot + "/tree_hero_oak.png");
            Sprite sprBush = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_bush.png");
            Sprite sprReeds = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_reeds.png");
            Sprite sprMush = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_mushrooms.png");
            Sprite sprRock = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_rock.png");
            Sprite sprLantern = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_garden_lantern.png");
            Sprite sprFence = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_wood_fence_h.png");
            Sprite sprBench = AssetDatabase.LoadAssetAtPath<Sprite>(EnvPropsRoot + "/prop_memory_bench.png");
            Sprite sprShadow = AssetDatabase.LoadAssetAtPath<Sprite>(EnvPropsRoot + "/spr_ground_shadow.png");

            Color pineTint = new Color(0.84f, 0.90f, 0.86f, 1f);
            Color oakTint = new Color(0.86f, 0.92f, 0.84f, 1f);

            // Hang thong & soi dong khung 2 ben Cong Dam Lay (x = -16..-13, y = 6..13) -> khop 1:1 voi WestSwamp_Chapter2!
            Vector2[] archwayPines =
            {
                new Vector2(-18.6f, 12.2f), new Vector2(-17.8f, 9.8f), new Vector2(-18.4f, 7.2f), new Vector2(-19.5f, 4.6f),
                new Vector2(-10.8f, 12.0f), new Vector2(-11.2f, 9.5f), new Vector2(-10.5f, 6.8f), new Vector2(-9.8f, 4.4f),
                new Vector2(-22.2f, 11.4f), new Vector2(-23.0f, 7.8f), new Vector2(-21.8f, 3.2f),
                new Vector2(-22.5f, -1.5f), new Vector2(-21.4f, -6.2f), new Vector2(-23.2f, -10.4f),
                new Vector2(-17.6f, -10.8f), new Vector2(-13.8f, -11.2f), new Vector2(-10.4f, -11.4f),
            };
            for (int i = 0; i < archwayPines.Length; i++)
            {
                Sprite treeSpr = (i % 3 == 0 && sprOak != null) ? sprOak : sprPine;
                SpawnDecor(root.transform, $"WestTree_{i}", treeSpr, archwayPines[i], treeSpr == sprOak ? oakTint : pineTint,
                    true, new Vector2(0.95f, 0.65f), new Vector2(0f, -1.15f), sprShadow, 1.35f);
            }

            // Hang rao go nho xinh om theo mep Nam san nha ba Mara
            float[] fenceXs = { -17.8f, -16.8f, -15.8f, -13.6f };
            for (int i = 0; i < fenceXs.Length; i++)
                SpawnDecor(root.transform, $"MaraFence_{i}", sprFence, new Vector2(fenceXs[i], -7.4f), Color.white,
                    true, new Vector2(0.95f, 0.35f), new Vector2(0f, -0.15f), null, 0f);

            // Ghe go & den san nha ba Mara
            SpawnDecor(root.transform, "MaraYardBench", sprBench, new Vector2(-17.8f, -4.6f), Color.white,
                true, new Vector2(1.2f, 0.45f), new Vector2(0f, -0.15f), sprShadow, 0.8f);
            SpawnDecor(root.transform, "MaraYardLantern", sprLantern, new Vector2(-12.6f, -3.2f), Color.white,
                true, new Vector2(0.42f, 0.32f), new Vector2(0f, -0.25f), null, 0f);

            // Bui ram, da reu, lau say & nam diem xuyet doc bo song Tay & ven duong rung
            Vector2[] bushes = { new Vector2(-19.2f, 1.4f), new Vector2(-18.6f, -2.2f), new Vector2(-19.5f, -7.8f), new Vector2(-11.4f, 2.8f), new Vector2(-16.6f, -8.8f) };
            for (int i = 0; i < bushes.Length; i++)
                SpawnDecor(root.transform, $"WestBush_{i}", sprBush, bushes[i], new Color(0.85f, 0.92f, 0.86f, 1f), true, new Vector2(0.7f, 0.45f), new Vector2(0f, -0.15f), null, 0f);

            Vector2[] reeds = { new Vector2(-8.9f, 3.2f), new Vector2(-8.8f, 6.4f), new Vector2(-8.9f, -3.6f), new Vector2(-8.8f, -5.4f), new Vector2(-8.9f, -9.5f) };
            for (int i = 0; i < reeds.Length; i++)
                SpawnDecor(root.transform, $"WestReeds_{i}", sprReeds, reeds[i], Color.white, false, Vector2.zero, Vector2.zero, null, 0f);

            Vector2[] mushrooms = { new Vector2(-17.4f, 6.2f), new Vector2(-11.8f, 8.2f), new Vector2(-17.2f, 10.6f), new Vector2(-19.2f, -4.2f) };
            for (int i = 0; i < mushrooms.Length; i++)
                SpawnDecor(root.transform, $"WestMush_{i}", sprMush, mushrooms[i], Color.white, false, Vector2.zero, Vector2.zero, null, 0f);

            Vector2[] rocks = { new Vector2(-18.8f, -0.4f), new Vector2(-10.2f, 5.2f), new Vector2(-20.4f, 8.8f) };
            for (int i = 0; i < rocks.Length; i++)
                SpawnDecor(root.transform, $"WestRock_{i}", sprRock, rocks[i], new Color(0.86f, 0.90f, 0.92f, 1f), true, new Vector2(0.75f, 0.45f), new Vector2(0f, -0.12f), null, 0f);
        }

        private static void AlignSwampEntranceGate()
        {
            // 1. Portal_ToWestSwamp tai chinh giua cua rung Bac (x = -14.5, y = 11.9)
            DestroyIfExists("Portal_ToWestSwamp");
            GameObject portalGo = new GameObject("Portal_ToWestSwamp");
            portalGo.transform.position = new Vector3(-14.5f, 11.9f, 0f);
            BoxCollider2D pCol = portalGo.AddComponent<BoxCollider2D>();
            pCol.isTrigger = true;
            pCol.size = new Vector2(4.4f, 1.5f);

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

            // 2. Cap den dan loi 2 ben cong rung (-16.9 va -12.1)
            Sprite sprLantern = AssetDatabase.LoadAssetAtPath<Sprite>(EnvDecorRoot + "/decor_garden_lantern.png");
            DestroyIfExists("SwampRoadGuideLanterns");
            GameObject guideRoot = new GameObject("SwampRoadGuideLanterns");
            SpawnDecor(guideRoot.transform, "RoadLantern_L", sprLantern, new Vector2(-16.9f, 7.6f), Color.white, true, new Vector2(0.42f, 0.32f), new Vector2(0f, -0.25f), null, 0f);
            SpawnDecor(guideRoot.transform, "RoadLantern_R", sprLantern, new Vector2(-12.1f, 7.6f), Color.white, true, new Vector2(0.42f, 0.32f), new Vector2(0f, -0.25f), null, 0f);

            // 3. Can chinh SwampMistBarrier chan ngang cong rung tai (-14.5, 8.2)
            SwampMistBarrierController barrier = UnityEngine.Object.FindAnyObjectByType<SwampMistBarrierController>();
            if (barrier != null)
            {
                barrier.transform.position = new Vector3(-14.5f, 8.2f, 0f);
                var bso = new SerializedObject(barrier);
                bso.FindProperty("unlockedLanternGlow").objectReferenceValue = guideRoot;
                bso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(barrier);
            }

            // 4. Cap nhat Spawn_WestSwamp nam ngay chinh giua duong phia Nam cua cong (-14.5, 9.7)
            WorldSpawnRegistry reg = UnityEngine.Object.FindAnyObjectByType<WorldSpawnRegistry>();
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
                        el.FindPropertyRelative("position").vector2Value = new Vector2(-14.5f, 9.7f);
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
                    el.FindPropertyRelative("position").vector2Value = new Vector2(-14.5f, 9.7f);
                }
                rso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(reg);
            }
        }

        private static void RelocateOtisNotebookDesk()
        {
            GameObject otisClue = GameObject.Find("Clue_OtisNote");
            if (otisClue == null)
            {
                Debug.LogWarning("[MV][Ch2-P3] Khong tim thay Clue_OtisNote trong Riverbank_Chapter2!");
                return;
            }

            // Dat Ban So Bac Otis ngay sat hien Vuon Uom Dong (House_EastNursery o 19.5, 5.5; NPC_Otis o 17.2, 3.0)
            otisClue.transform.position = new Vector3(21.2f, 3.2f, 0f);
            otisClue.transform.localScale = Vector3.one;

            Sprite deskSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2SpritesRoot + "/prop_otis_notebook_desk.png");
            SpriteRenderer sr = otisClue.GetComponent<SpriteRenderer>();
            if (sr == null) sr = otisClue.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && deskSpr != null)
            {
                sr.sprite = deskSpr;
                sr.color = Color.white;
                sr.sortingLayerName = "WorldObjects";
                EditorUtility.SetDirty(sr);
            }

            BoxCollider2D col = otisClue.GetComponent<BoxCollider2D>();
            if (col == null) col = otisClue.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.85f, 1.55f);
            col.offset = Vector2.zero;

            EditorUtility.SetDirty(otisClue);
        }

        private static void SpawnDecor(Transform parent, string name, Sprite spr, Vector2 pos, Color tint,
            bool hasCol, Vector2 colSize, Vector2 colOffset, Sprite shadowSpr, float shadowScale)
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color = tint;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);

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
                ssr.color = new Color(1f, 1f, 1f, 0.38f);
                ssr.sortingLayerName = "GroundDecor";
                ssr.sortingOrder = 4;
            }
        }

        private static Tilemap FindTilemapByName(string name)
        {
            GameObject go = GameObject.Find(name);
            return go != null ? go.GetComponent<Tilemap>() : null;
        }

        private static Tile GetExistingObstacleTile(Tilemap obsTm)
        {
            var bounds = obsTm.cellBounds;
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Tile t = obsTm.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (t != null && t.name.ToLower().Contains("tree")) return t;
                }
            }
            return null;
        }

        private static Tile GetOrCreateTile(string assetPath, string spritePath)
        {
            Tile t = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);
            Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (t == null)
            {
                t = ScriptableObject.CreateInstance<Tile>();
                t.sprite = spr;
                t.colliderType = Tile.ColliderType.None;
                AssetDatabase.CreateAsset(t, assetPath);
            }
            else if (spr != null && t.sprite != spr)
            {
                t.sprite = spr;
                EditorUtility.SetDirty(t);
            }
            return t;
        }

        private static void DestroyIfExists(string name)
        {
            GameObject go = GameObject.Find(name);
            if (go != null) UnityEngine.Object.DestroyImmediate(go);
        }

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P3 - 2. Validate Step Ch2-P3")]
        public static void ValidateStepCh2P3()
        {
            int errors = 0;
            if (!File.Exists(RiverbankScenePath))
            {
                Debug.LogError("[MV][Ch2-P3][FAIL] Khong tim thay " + RiverbankScenePath);
                return;
            }

            EditorSceneManager.OpenScene(RiverbankScenePath, OpenSceneMode.Single);

            GameObject portal = GameObject.Find("Portal_ToWestSwamp");
            if (portal == null || Mathf.Abs(portal.transform.position.x - (-14.5f)) > 0.25f)
            {
                Debug.LogError("[MV][Ch2-P3][FAIL] Portal_ToWestSwamp chua can giua cong duong (-14.5)!");
                errors++;
            }

            GameObject otisClue = GameObject.Find("Clue_OtisNote");
            if (otisClue == null || Vector2.Distance(otisClue.transform.position, new Vector2(21.2f, 3.2f)) > 0.3f)
            {
                Debug.LogError("[MV][Ch2-P3][FAIL] Clue_OtisNote chua duoc doi ve sat hien Vuon Uom Dong (21.2, 3.2)!");
                errors++;
            }

            if (GameObject.Find("WestBank_Ch2P3_Decor") == null)
            {
                Debug.LogError("[MV][Ch2-P3][FAIL] Thieu nhom WestBank_Ch2P3_Decor!");
                errors++;
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-P3] VALIDATE PASS (0 loi)! Bo Tay nha ba Mara & Ban so bac Otis da chuan khop.");
            else
                Debug.LogError($"[MV][Ch2-P3] VALIDATE FAIL ({errors} loi).");
        }
    }
}
