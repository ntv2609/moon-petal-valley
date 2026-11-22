using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Player;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step16_Fix
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";

        [MenuItem("Moonpetal/Step 16/2. Fix Invisible Colliders & Align Garden Arch")]
        public static void ApplyStep16Fix()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU STEP 16.FIX 1 (COLLIDERS & GARDEN ARCH) ===");
            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Merge ObstacleTilemap collider via CompositeCollider2D (Unity 6 API)
            FixObstacleTilemapCompositeCollider();

            // 2. Refine Player_Wen collider (compact + rounded edgeRadius so Wen never snags)
            RefinePlayerCollider();

            // 3. Scan & clean any invisible orphaned solid Collider2D in scene
            AuditAndCleanInvisibleColliders();

            // 4. Align GardenEntranceArch flush with fence + move bushes overlapping fence
            AlignGardenArchAndFenceBushes();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 16.FIX 1! Vào Play Mode kiểm tra đi lại quanh hàng rào và cổng vườn nhé. ===");
        }

        private static void FixObstacleTilemapCompositeCollider()
        {
            GameObject obsGo = GameObject.Find("ObstacleTilemap");
            if (obsGo == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy ObstacleTilemap.");
                return;
            }

            var tmCol = obsGo.GetComponent<TilemapCollider2D>();
            if (tmCol == null) tmCol = obsGo.AddComponent<TilemapCollider2D>();

            var rb = obsGo.GetComponent<Rigidbody2D>();
            if (rb == null) rb = obsGo.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            rb.simulated = true;

            var compCol = obsGo.GetComponent<CompositeCollider2D>();
            if (compCol == null) compCol = obsGo.AddComponent<CompositeCollider2D>();
            compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
            compCol.generationType = CompositeCollider2D.GenerationType.Synchronous;

            tmCol.compositeOperation = Collider2D.CompositeOperation.Merge;
            compCol.GenerateGeometry();

            EditorUtility.SetDirty(obsGo);
            Debug.Log("[MV] Đã gộp ObstacleTilemap bằng CompositeCollider2D (hết kẹt cạnh tile).");
        }

        private static void ApplyCleanPlayerBox(GameObject go)
        {
            var col = go.GetComponent<BoxCollider2D>();
            if (col == null) return;
            col.size = new Vector2(0.46f, 0.34f);
            col.offset = new Vector2(0f, -0.34f);
            col.edgeRadius = 0.05f;
            EditorUtility.SetDirty(col);
        }

        private static void RefinePlayerCollider()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
                ApplyCleanPlayerBox(root);
                PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                ApplyCleanPlayerBox(player.gameObject);
            }
            Debug.Log("[MV] Đã thu gọn và bo tròn BoxCollider2D dưới chân Player_Wen.");
        }

        private static void AuditAndCleanInvisibleColliders()
        {
            var allCols = UnityEngine.Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Include);
            int fixedCount = 0;

            for (int i = 0; i < allCols.Length; i++)
            {
                var col = allCols[i];
                if (col == null || !col.enabled || col.isTrigger) continue;

                GameObject go = col.gameObject;
                if (go.name == "ObstacleTilemap" || go.name == "Player_Wen") continue;
                if (go.GetComponent<TilemapCollider2D>() != null || go.GetComponent<CompositeCollider2D>() != null) continue;

                // Check if this solid collider has ANY visible SpriteRenderer on itself or children
                var srs = go.GetComponentsInChildren<SpriteRenderer>(false);
                bool hasVisibleSprite = false;
                for (int k = 0; k < srs.Length; k++)
                {
                    if (srs[k].enabled && srs[k].sprite != null && srs[k].color.a > 0.05f)
                    {
                        hasVisibleSprite = true;
                        break;
                    }
                }

                if (!hasVisibleSprite)
                {
                    col.enabled = false;
                    EditorUtility.SetDirty(col);
                    Debug.Log($"[MV] Đã tắt Collider tàng hình: '{go.name}' tại {go.transform.position}");
                    fixedCount++;
                }
            }

            Debug.Log($"[MV] Kiểm toán Collider hoàn tất (đã dọn {fixedCount} collider tàng hình).");
        }

        private static void AlignGardenArchAndFenceBushes()
        {
            GameObject arch = GameObject.Find("GardenEntranceArch");
            if (arch != null)
            {
                arch.transform.position = new Vector3(0.0f, -0.55f, 0f);
                arch.transform.localScale = new Vector3(1.22f, 0.96f, 1f);
                var sr = arch.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = 2600;
                }
                EditorUtility.SetDirty(arch);
                Debug.Log("[MV] Đã căn cổng vòm GardenEntranceArch nối khít 2 đầu hàng rào.");
            }

            // Move the 2 bushes that were overlapping the South Garden fence at y = -1.6
            GameObject b6 = GameObject.Find("ExtraBush_6");
            if (b6 != null)
            {
                b6.transform.position = new Vector3(3.6f, -0.35f, 0f);
                var sr = b6.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = Mathf.RoundToInt(-b6.transform.position.y * 100f);
                EditorUtility.SetDirty(b6);
            }

            GameObject b7 = GameObject.Find("ExtraBush_7");
            if (b7 != null)
            {
                b7.transform.position = new Vector3(-3.6f, -0.35f, 0f);
                var sr = b7.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = Mathf.RoundToInt(-b7.transform.position.y * 100f);
                EditorUtility.SetDirty(b7);
            }
        }
    }
}
