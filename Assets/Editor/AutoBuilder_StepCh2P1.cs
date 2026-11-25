using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Player;

namespace MoonpetalValley.EditorTools
{
    /// <summary>
    /// Step Ch2-P1:
    /// 1. Khoa chat lan can BridgeRail_North/South khop mat van cau & collider chan Wen (chong lo lung tren nuoc).
    /// 2. Dong bo do rong TempScaffoldingWalkway (y = -0.75..+1.75) khop 100% khung cau.
    /// 3. Dat CollisionDetectionMode2D.Continuous cho Player_Wen trong Riverbank_Chapter2 & WestSwamp_Chapter2.
    /// </summary>
    public static class AutoBuilder_StepCh2P1
    {
        private const string RiverbankScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string WestSwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";
        private const string PlankSpritePath = "Assets/Art/Sprites/Chapter2/tile_bridge_plank.png";
        private const string BrokenSpritePath = "Assets/Art/Sprites/Chapter2/tile_bridge_broken.png";

        // Unified Bridge Rail Geometry (aligned with Player_Wen BoxCollider2D size=(0.46,0.34), offset=(0,-0.34)):
        // - North rail center y=+1.48, height=0.56 => bottom edge y=+1.20 => Wen max pos.y = 1.37 => visual feet max y = +0.91 (inside planks)
        // - South rail center y=-0.48, height=0.56 => top edge y=-0.20    => Wen min pos.y = 0.31 => visual feet min y = -0.15 (inside planks)
        // - Span x = -8.6..+8.6 (width 17.2) anchors cleanly into both West and East riverbanks.
        private static readonly Vector3 NorthRailPos = new Vector3(0.0f, 1.48f, 0f);
        private static readonly Vector3 SouthRailPos = new Vector3(0.0f, -0.48f, 0f);
        private static readonly Vector2 RailSize = new Vector2(17.2f, 0.56f);

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P1 - 1. Apply Bridge Anti-Float & Continuous Physics")]
        public static void ApplyStepCh2P1()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            ApplyToRiverbank();
            ApplyContinuousPhysicsToScene(WestSwampScenePath);

            Debug.Log("[MV][Ch2-P1] Hoan tat Step Ch2-P1 (Stamina-only orbs + khoa lan can cau chong lo lung).");
        }

        private static void ApplyToRiverbank()
        {
            if (!System.IO.File.Exists(RiverbankScenePath))
            {
                Debug.LogError("[MV][Ch2-P1] Khong tim thay scene: " + RiverbankScenePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(RiverbankScenePath, OpenSceneMode.Single);

            RiverBridgeController bridge = UnityEngine.Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge == null)
            {
                Debug.LogError("[MV][Ch2-P1] Khong tim thay RiverBridgeController trong " + RiverbankScenePath);
                return;
            }

            // 1. Kep chat BridgeRail_North / BridgeRail_South
            BoxCollider2D northRail = EnsureBridgeRail(bridge.transform, "BridgeRail_North", NorthRailPos, RailSize);
            BoxCollider2D southRail = EnsureBridgeRail(bridge.transform, "BridgeRail_South", SouthRailPos, RailSize);

            SerializedObject bso = new SerializedObject(bridge);
            bso.FindProperty("bridgeRailNorth").objectReferenceValue = northRail;
            bso.FindProperty("bridgeRailSouth").objectReferenceValue = southRail;
            bso.ApplyModifiedPropertiesWithoutUndo();

            // 2. Mo rong TempScaffoldingWalkway de phu kin khung cau (y = -0.75..+1.75) khong con khoang trong nuoc
            Transform scafRoot = bridge.transform.Find("BridgeScaffoldingRoot");
            if (scafRoot != null)
            {
                Sprite plankSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PlankSpritePath);
                Sprite brokenSpr = AssetDatabase.LoadAssetAtPath<Sprite>(BrokenSpritePath);
                Sprite beamSpr = plankSpr != null ? plankSpr : brokenSpr;

                Transform oldWalk = scafRoot.Find("TempScaffoldingWalkway");
                if (oldWalk != null) UnityEngine.Object.DestroyImmediate(oldWalk.gameObject);

                GameObject walkway = new GameObject("TempScaffoldingWalkway");
                walkway.transform.SetParent(scafRoot, false);
                walkway.transform.SetAsFirstSibling();

                // 2 hang van gian giao song song phu tu y = -0.75 den +1.75 (center y = +0.50)
                float[] rowYs = { -0.15f, 0.50f, 1.15f };
                for (int r = 0; r < rowYs.Length; r++)
                {
                    Color tint = r == 1
                        ? new Color(0.76f, 0.66f, 0.54f, 0.96f)
                        : new Color(0.62f, 0.52f, 0.42f, 0.94f);
                    for (int x = -8; x <= 7; x++)
                    {
                        GameObject tile = new GameObject($"ScafDeck_{x}_{r}");
                        tile.transform.SetParent(walkway.transform, false);
                        tile.transform.localPosition = new Vector3(x + 0.5f, rowYs[r], 0f);
                        tile.transform.localScale = new Vector3(0.98f, 0.72f, 1f);
                        var sr = tile.AddComponent<SpriteRenderer>();
                        sr.sprite = beamSpr;
                        sr.color = tint;
                        sr.sortingLayerName = "GroundDecor";
                        sr.sortingOrder = 5;
                    }
                }

                // Thanh nep lan can tam doc mep Bac & Nam (de nguoi choi thay ro ranh gioi an toan)
                BuildTempRailStrip(walkway.transform, "TempRailVisual_North", beamSpr, 1.62f);
                BuildTempRailStrip(walkway.transform, "TempRailVisual_South", beamSpr, -0.62f);
            }

            EditorUtility.SetDirty(bridge);

            // 3. Dat Continuous Collision cho Player_Wen neu co trong scene
            SetPlayerContinuousPhysics();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Ch2-P1] Da cap nhat BridgeRail_North/South + TempScaffoldingWalkway trong Riverbank_Chapter2.");
        }

        private static void ApplyContinuousPhysicsToScene(string scenePath)
        {
            if (!System.IO.File.Exists(scenePath)) return;
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            if (SetPlayerContinuousPhysics())
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        private static bool SetPlayerContinuousPhysics()
        {
            PlayerController2D pc = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (pc == null) return false;
            Rigidbody2D rb = pc.GetComponent<Rigidbody2D>();
            if (rb == null) return false;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            EditorUtility.SetDirty(rb);
            return true;
        }

        private static void BuildTempRailStrip(Transform parent, string name, Sprite spr, float yPos)
        {
            GameObject strip = new GameObject(name);
            strip.transform.SetParent(parent, false);
            for (int x = -8; x <= 7; x++)
            {
                GameObject seg = new GameObject($"Rail_{x}");
                seg.transform.SetParent(strip.transform, false);
                seg.transform.localPosition = new Vector3(x + 0.5f, yPos, 0f);
                seg.transform.localScale = new Vector3(1.0f, 0.22f, 1f);
                var sr = seg.AddComponent<SpriteRenderer>();
                sr.sprite = spr;
                sr.color = new Color(0.38f, 0.27f, 0.18f, 1f);
                sr.sortingLayerName = "GroundDecor";
                sr.sortingOrder = 10;
            }
        }

        private static BoxCollider2D EnsureBridgeRail(Transform parent, string name, Vector3 localPos, Vector2 size)
        {
            Transform tr = parent.Find(name);
            GameObject go = tr != null ? tr.gameObject : new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.layer = 10; // Obstacle layer
            BoxCollider2D col = go.GetComponent<BoxCollider2D>();
            if (col == null) col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = size;
            col.offset = Vector2.zero;
            col.enabled = true;
            EditorUtility.SetDirty(go);
            return col;
        }

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P1 - 2. Validate Step Ch2-P1")]
        public static void ValidateStepCh2P1()
        {
            int errors = 0;

            if (!System.IO.File.Exists(RiverbankScenePath))
            {
                Debug.LogError("[MV][Ch2-P1][FAIL] Khong tim thay " + RiverbankScenePath);
                return;
            }

            EditorSceneManager.OpenScene(RiverbankScenePath, OpenSceneMode.Single);
            RiverBridgeController bridge = UnityEngine.Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge == null)
            {
                Debug.LogError("[MV][Ch2-P1][FAIL] Thieu RiverBridgeController!");
                errors++;
            }
            else
            {
                var bso = new SerializedObject(bridge);
                var north = bso.FindProperty("bridgeRailNorth").objectReferenceValue as BoxCollider2D;
                var south = bso.FindProperty("bridgeRailSouth").objectReferenceValue as BoxCollider2D;
                if (north == null || south == null)
                {
                    Debug.LogError("[MV][Ch2-P1][FAIL] Thieu BridgeRail_North hoac BridgeRail_South!");
                    errors++;
                }
                else
                {
                    if (north.size.x < 16.5f || south.size.x < 16.5f)
                    {
                        Debug.LogError("[MV][Ch2-P1][FAIL] BridgeRail chua phu kin 2 dau mo cau (size.x < 16.5)!");
                        errors++;
                    }
                    float northBottom = north.transform.localPosition.y - north.size.y * 0.5f;
                    float southTop = south.transform.localPosition.y + south.size.y * 0.5f;
                    if (northBottom > 1.25f || southTop < -0.25f)
                    {
                        Debug.LogError($"[MV][Ch2-P1][FAIL] Khe lan can qua rong (northBottom={northBottom:F2}, southTop={southTop:F2})!");
                        errors++;
                    }
                }
            }

            // Kiem tra enum RecoveryOrbType khong con LightHP
            string[] orbNames = System.Enum.GetNames(typeof(MoonpetalValley.Combat.RecoveryOrbType));
            for (int i = 0; i < orbNames.Length; i++)
            {
                if (orbNames[i].Contains("HP"))
                {
                    Debug.LogError("[MV][Ch2-P1][FAIL] RecoveryOrbType van con muc hoi HP: " + orbNames[i]);
                    errors++;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-P1] VALIDATE PASS (0 loi)! Stamina-only orbs & Bridge Anti-Float Physics dat chuan.");
            else
                Debug.LogError($"[MV][Ch2-P1] VALIDATE FAIL ({errors} loi).");
        }
    }
}
