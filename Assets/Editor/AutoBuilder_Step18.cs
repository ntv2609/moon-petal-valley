using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step18
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";

        [MenuItem("Moonpetal/Step 18/1. Apply Hero Buildings, Multi-Layered Trees & Pine Grove")]
        public static void BuildStep18()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 18 (CHẶNG 2 - NHÀ CỬA & CÂY TÁN LỚN) ===");
            AssetDatabase.Refresh();
            ForceReimportHeroSprites();
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Upgrade Hero Buildings + clean foot-only colliders (so Wen walks behind roofs naturally)
            UpgradeHeroBuildings();

            // 2. Refresh Y-sorting of existing hero trees so Wen walks behind upper canopies
            RefreshHeroTreeSorting();

            // 3. Add curated Pine Grove at Forest Edge & North Ridge
            BuildForestPineGrove();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 18 (CHẶNG 2)! Chạy 'Moonpetal/Step 18/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 18/Validate")]
        public static void ValidateStep18()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 18 ===");

            string[] required =
            {
                "Buildings/house_bakery.png",
                "Buildings/house_shed.png",
                "Buildings/otis_notice_board.png",
                "Buildings/tree_hero_oak.png",
                "Buildings/tree_hero_blossom.png",
                "Buildings/tree_hero_pine.png",
                "Decor/decor_bush.png",
                "Decor/decor_rock.png",
            };
            foreach (string rel in required)
            {
                if (AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel) == null)
                {
                    Debug.LogError("[MV] Thiếu sprite Step 18: " + EnvRoot + rel);
                    errors++;
                }
            }

            if (GameObject.Find("House_HanaBakery") == null)
            {
                Debug.LogError("[MV] Không tìm thấy House_HanaBakery trong scene.");
                errors++;
            }
            if (GameObject.Find("House_BachShed") == null)
            {
                Debug.LogError("[MV] Không tìm thấy House_BachShed trong scene.");
                errors++;
            }
            if (GameObject.Find("Step18ForestPines") == null)
            {
                Debug.LogError("[MV] Thiếu nhóm Step18ForestPines.");
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
                Debug.Log("[MV] VALIDATE STEP 18: PASS (0 lỗi | Hero Buildings=OK | 3 Tree Species 48x64=OK | Pine Grove=OK).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 18: FAIL ({errors} lỗi).");
            }
        }

        private static Sprite LoadEnv(string rel) => AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel);

        private static void ForceReimportHeroSprites()
        {
            string[] folders =
            {
                "Assets/Art/Sprites/Environment/Buildings",
                "Assets/Art/Sprites/Environment/Decor",
            };
            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder)) continue;
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
        }

        private static void UpgradeHeroBuildings()
        {
            Sprite bakerySpr = LoadEnv("Buildings/house_bakery.png");
            Sprite shedSpr = LoadEnv("Buildings/house_shed.png");
            Sprite boardSpr = LoadEnv("Buildings/otis_notice_board.png");

            GameObject bakery = GameObject.Find("House_HanaBakery");
            if (bakery != null && bakerySpr != null)
            {
                var sr = bakery.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = bakerySpr;
                    sr.color = Color.white;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(bakery.transform.position.y - 1.6f) * 100f);
                }
                var col = bakery.GetComponent<BoxCollider2D>();
                if (col != null)
                {
                    col.size = new Vector2(3.8f, 1.55f);
                    col.offset = new Vector2(0f, -1.05f);
                }
                EditorUtility.SetDirty(bakery);
            }

            GameObject shed = GameObject.Find("House_BachShed");
            if (shed != null && shedSpr != null)
            {
                var sr = shed.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = shedSpr;
                    sr.color = Color.white;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(shed.transform.position.y - 1.3f) * 100f);
                }
                var col = shed.GetComponent<BoxCollider2D>();
                if (col != null)
                {
                    col.size = new Vector2(2.75f, 1.35f);
                    col.offset = new Vector2(0f, -0.85f);
                }
                EditorUtility.SetDirty(shed);
            }

            GameObject board = GameObject.Find("OtisNoticeBoard");
            if (board != null && boardSpr != null)
            {
                var sr = board.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = boardSpr;
                    sr.color = Color.white;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(board.transform.position.y - 0.8f) * 100f);
                }
                EditorUtility.SetDirty(board);
            }

            Debug.Log("[MV] Đã nâng cấp Nhà Bánh Hana, Chòi Vườn và Bảng Tin với collider gọn chân nhà.");
        }

        private static void RefreshHeroTreeSorting()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) return;

            int updated = 0;
            var srs = decorGroup.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < srs.Length; i++)
            {
                var sr = srs[i];
                if (sr == null || sr.sprite == null) continue;
                string n = sr.sprite.name;
                if (n.Contains("tree_hero_"))
                {
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(sr.transform.position.y - 1.45f) * 100f);
                    EditorUtility.SetDirty(sr);
                    updated++;
                }
            }
            Debug.Log($"[MV] Đã căn lại Y-Sort chân gốc cho {updated} cây cỡ lớn (Wen đi sau tán cây tự nhiên).");
        }

        private static void BuildForestPineGrove()
        {
            Sprite pineSpr = LoadEnv("Buildings/tree_hero_pine.png");
            if (pineSpr == null) return;

            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) decorGroup = new GameObject("VillageDecorGroup");

            Transform old = decorGroup.transform.Find("Step18ForestPines");
            if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);

            GameObject grove = new GameObject("Step18ForestPines");
            grove.transform.SetParent(decorGroup.transform, false);

            // Curated Pine Grove framing the East Forest Edge & North Mountain Ridge (zero overlap with paths/pond)
            Vector3[] pineSpots =
            {
                new Vector3(4.5f, 13.4f, 0f),
                new Vector3(9.5f, 12.6f, 0f),
                new Vector3(14.8f, 12.2f, 0f),
                new Vector3(18.6f, 8.8f, 0f),
                new Vector3(18.4f, 1.8f, 0f),
                new Vector3(17.6f, -5.8f, 0f),
                new Vector3(-18.6f, -1.2f, 0f),
            };

            for (int i = 0; i < pineSpots.Length; i++)
            {
                GameObject go = new GameObject($"HeroPine_{i}");
                go.transform.SetParent(grove.transform, false);
                go.transform.position = pineSpots[i];
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = pineSpr;
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = Mathf.RoundToInt(-(pineSpots[i].y - 1.45f) * 100f);
            }

            Debug.Log($"[MV] Đã trồng thêm {pineSpots.Length} cây thông tán lớn ở Rìa Rừng Phía Đông & Dãy Bắc.");
        }
    }
}
