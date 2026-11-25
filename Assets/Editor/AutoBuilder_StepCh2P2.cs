using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Combat;

namespace MoonpetalValley.EditorTools
{
    /// <summary>
    /// Step Ch2-P2:
    /// 1. Cau hinh PPU/Point filter cho toan bo 22 sprite pixel-art moi (Cau 3 trang thai, Den cau, Orb Stamina, Be da Dam Lay, Ban so bac Otis).
    /// 2. Dung lai BrokenVisuals, BridgeScaffoldingRoot (+ 3 progressivePlankSegments) va RepairedVisuals bang bo art cau moi co lan can & tru cau.
    /// 3. Gan StaminaOrbArtRegistrar (spr_orb_stamina + spr_orb_stamina_surge) vao Riverbank_Chapter2 & WestSwamp_Chapter2.
    /// 4. Cap nhat sprite prop_otis_notebook_desk cho Clue_OtisNote va gan be da prop_swamp_shrine_pedestal cho Mieu Dam Lay.
    /// </summary>
    public static class AutoBuilder_StepCh2P2
    {
        private const string RiverbankScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string WestSwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";

        private const string BridgeDir = "Assets/Art/Sprites/Chapter2/Bridge";
        private const string Ch2Dir = "Assets/Art/Sprites/Chapter2";
        private const string LanternDir = "Assets/Art/Characters/actor_bridge_lantern";

        private static readonly string[] BridgeSprites =
        {
            BridgeDir + "/bridge_plank_top.png",
            BridgeDir + "/bridge_plank_mid.png",
            BridgeDir + "/bridge_plank_bot.png",
            BridgeDir + "/bridge_broken_west.png",
            BridgeDir + "/bridge_broken_east.png",
            BridgeDir + "/bridge_scaf_top.png",
            BridgeDir + "/bridge_scaf_mid.png",
            BridgeDir + "/bridge_scaf_bot.png",
            BridgeDir + "/bridge_post_pillar.png",
        };

        private static readonly string[] PropSprites =
        {
            Ch2Dir + "/spr_orb_stamina.png",
            Ch2Dir + "/spr_orb_stamina_surge.png",
            Ch2Dir + "/prop_swamp_shrine_pedestal.png",
            Ch2Dir + "/prop_otis_notebook_desk.png",
        };

        private static readonly string[] LanternSprites =
        {
            LanternDir + "/lit_00.png",
            LanternDir + "/lit_01.png",
            LanternDir + "/lit_02.png",
            LanternDir + "/lit_03.png",
            LanternDir + "/flicker_00.png",
            LanternDir + "/flicker_01.png",
            LanternDir + "/flicker_02.png",
            LanternDir + "/off_00.png",
            LanternDir + "/off_01.png",
        };

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P2 - 1. Configure & Apply New Chapter 2 Art Pack")]
        public static void ApplyStepCh2P2()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // 1. Configure all 22 sprites
            for (int i = 0; i < BridgeSprites.Length; i++)
                ConfigureSprite(BridgeSprites[i], 16f, new Vector2(0.5f, 0.5f));
            for (int i = 0; i < PropSprites.Length; i++)
                ConfigureSprite(PropSprites[i], 16f, new Vector2(0.5f, 0.5f));
            for (int i = 0; i < LanternSprites.Length; i++)
                ConfigureSprite(LanternSprites[i], 24f, new Vector2(0.5f, 0.10f));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            Sprite sprOrb = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2Dir + "/spr_orb_stamina.png");
            Sprite sprSurge = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2Dir + "/spr_orb_stamina_surge.png");

            // 2. Apply to Riverbank_Chapter2
            ApplyToRiverbank(sprOrb, sprSurge);

            // 3. Apply to WestSwamp_Chapter2
            ApplyToWestSwamp(sprOrb, sprSurge);

            Debug.Log("[MV][Ch2-P2] Hoan tat Step Ch2-P2 (Bo Art cau 3 trang thai, den cau, orb Stamina, be da Dam Lay & ban so Otis).");
        }

        private static void ApplyToRiverbank(Sprite sprOrb, Sprite sprSurge)
        {
            if (!File.Exists(RiverbankScenePath))
            {
                Debug.LogError("[MV][Ch2-P2] Khong tim thay " + RiverbankScenePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(RiverbankScenePath, OpenSceneMode.Single);

            RiverBridgeController bridge = UnityEngine.Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge != null)
            {
                RebuildBridgeArt(bridge);
                AttachOrbRegistrar(bridge.gameObject, sprOrb, sprSurge);
            }
            else
            {
                Debug.LogError("[MV][Ch2-P2] Khong tim thay RiverBridgeController!");
            }

            // Cap nhat sprite Ban So Bac Otis cho Clue_OtisNote
            GameObject otisClue = GameObject.Find("Clue_OtisNote");
            Sprite deskSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2Dir + "/prop_otis_notebook_desk.png");
            if (otisClue != null && deskSpr != null)
            {
                SpriteRenderer sr = otisClue.GetComponent<SpriteRenderer>();
                if (sr == null) sr = otisClue.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = deskSpr;
                    sr.color = Color.white;
                    sr.sortingLayerName = "WorldObjects";
                    EditorUtility.SetDirty(sr);
                }
                otisClue.transform.localScale = Vector3.one;
                EditorUtility.SetDirty(otisClue);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void RebuildBridgeArt(RiverBridgeController bridge)
        {
            Sprite plankTop = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_plank_top.png");
            Sprite plankMid = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_plank_mid.png");
            Sprite plankBot = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_plank_bot.png");
            Sprite brkWest = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_broken_west.png");
            Sprite brkEast = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_broken_east.png");
            Sprite scafTop = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_scaf_top.png");
            Sprite scafMid = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_scaf_mid.png");
            Sprite scafBot = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_scaf_bot.png");
            Sprite postSpr = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_post_pillar.png");

            // --- A. BrokenVisuals ---
            GameObject brokenRoot = RecreateChild(bridge.transform, "BrokenVisuals");
            for (int y = -1; y <= 1; y++)
            {
                Sprite baseRow = y == 1 ? plankTop : (y == -1 ? plankBot : plankMid);
                for (int x = -8; x <= 7; x++)
                {
                    if (x > -5 && x < 4) continue;
                    Sprite useSpr = baseRow;
                    if (x == -5 && y == 0) useSpr = brkWest;
                    else if (x == 4 && y == 0) useSpr = brkEast;
                    else if (x == -5 && (y == 1 || y == -1)) useSpr = brkWest;
                    else if (x == 4 && (y == 1 || y == -1)) useSpr = brkEast;
                    CreateTile(brokenRoot.transform, $"Brk_{x}_{y}", useSpr, new Vector3(x + 0.5f, y + 0.5f, 0f), 8);
                }
            }
            AddCornerPillars(brokenRoot.transform, postSpr);

            // --- B. RepairedVisuals ---
            GameObject repairedRoot = RecreateChild(bridge.transform, "RepairedVisuals");
            for (int y = -1; y <= 1; y++)
            {
                Sprite rowSpr = y == 1 ? plankTop : (y == -1 ? plankBot : plankMid);
                for (int x = -8; x <= 7; x++)
                    CreateTile(repairedRoot.transform, $"Rep_{x}_{y}", rowSpr, new Vector3(x + 0.5f, y + 0.5f, 0f), 8);
            }
            AddCornerPillars(repairedRoot.transform, postSpr);
            // Tru cau giua nhip tao diem nhan
            CreateTile(repairedRoot.transform, "MidPost_NW", postSpr, new Vector3(-2.5f, 1.85f, 0f), 14, "WorldObjects");
            CreateTile(repairedRoot.transform, "MidPost_NE", postSpr, new Vector3(2.5f, 1.85f, 0f), 14, "WorldObjects");
            CreateTile(repairedRoot.transform, "MidPost_SW", postSpr, new Vector3(-2.5f, -0.75f, 0f), 14, "WorldObjects");
            CreateTile(repairedRoot.transform, "MidPost_SE", postSpr, new Vector3(2.5f, -0.75f, 0f), 14, "WorldObjects");

            // --- C. BridgeScaffoldingRoot (Unified full 3-row footprint + 3 progressive plank segments) ---
            GameObject scafRoot = RecreateChild(bridge.transform, "BridgeScaffoldingRoot");
            GameObject walkway = new GameObject("TempScaffoldingWalkway");
            walkway.transform.SetParent(scafRoot.transform, false);
            for (int y = -1; y <= 1; y++)
            {
                Sprite rowSpr = y == 1 ? scafTop : (y == -1 ? scafBot : scafMid);
                for (int x = -8; x <= 7; x++)
                    CreateTile(walkway.transform, $"Scaf_{x}_{y}", rowSpr, new Vector3(x + 0.5f, y + 0.5f, 0f), 6);
            }
            AddCornerPillars(walkway.transform, postSpr);

            GameObject seg1 = CreateProgressiveSegment(scafRoot.transform, "BuildSegment_1_East", plankTop, plankMid, plankBot, 3, 7);
            GameObject seg2 = CreateProgressiveSegment(scafRoot.transform, "BuildSegment_2_West", plankTop, plankMid, plankBot, -8, -3);
            GameObject seg3 = CreateProgressiveSegment(scafRoot.transform, "BuildSegment_3_Center", plankTop, plankMid, plankBot, -2, 2);
            seg1.SetActive(false);
            seg2.SetActive(false);
            seg3.SetActive(false);
            scafRoot.SetActive(false);

            SerializedObject bso = new SerializedObject(bridge);
            bso.FindProperty("brokenVisualsRoot").objectReferenceValue = brokenRoot;
            bso.FindProperty("repairedVisualsRoot").objectReferenceValue = repairedRoot;
            bso.FindProperty("scaffoldingVisualsRoot").objectReferenceValue = scafRoot;
            var segProp = bso.FindProperty("progressivePlankSegments");
            segProp.ClearArray();
            GameObject[] segs = { seg1, seg2, seg3 };
            for (int i = 0; i < segs.Length; i++)
            {
                segProp.InsertArrayElementAtIndex(i);
                segProp.GetArrayElementAtIndex(i).objectReferenceValue = segs[i];
            }
            bso.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bridge);
        }

        private static GameObject CreateProgressiveSegment(Transform parent, string name, Sprite top, Sprite mid, Sprite bot, int xMin, int xMax)
        {
            GameObject grp = new GameObject(name);
            grp.transform.SetParent(parent, false);
            for (int y = -1; y <= 1; y++)
            {
                Sprite rowSpr = y == 1 ? top : (y == -1 ? bot : mid);
                for (int x = xMin; x <= xMax; x++)
                    CreateTile(grp.transform, $"Seg_{x}_{y}", rowSpr, new Vector3(x + 0.5f, y + 0.5f, 0f), 9);
            }
            return grp;
        }

        private static void AddCornerPillars(Transform parent, Sprite postSpr)
        {
            if (postSpr == null) return;
            CreateTile(parent, "Post_NW", postSpr, new Vector3(-7.9f, 1.85f, 0f), 14, "WorldObjects");
            CreateTile(parent, "Post_NE", postSpr, new Vector3(7.9f, 1.85f, 0f), 14, "WorldObjects");
            CreateTile(parent, "Post_SW", postSpr, new Vector3(-7.9f, -0.75f, 0f), 14, "WorldObjects");
            CreateTile(parent, "Post_SE", postSpr, new Vector3(7.9f, -0.75f, 0f), 14, "WorldObjects");
        }

        private static void ApplyToWestSwamp(Sprite sprOrb, Sprite sprSurge)
        {
            if (!File.Exists(WestSwampScenePath)) return;
            var scene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);

            SwampWaveCombatController waveCtrl = UnityEngine.Object.FindAnyObjectByType<SwampWaveCombatController>();
            if (waveCtrl != null)
            {
                AttachOrbRegistrar(waveCtrl.gameObject, sprOrb, sprSurge);
            }

            // Gan be da co prop_swamp_shrine_pedestal duoi Mieu Den Bao Co neu co
            Sprite pedSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2Dir + "/prop_swamp_shrine_pedestal.png");
            GameObject shrine = GameObject.Find("AncientSwampShrine");
            if (shrine == null) shrine = GameObject.Find("SwampLanternShrine");
            if (shrine != null && pedSpr != null)
            {
                Transform oldPed = shrine.transform.Find("ShrineStonePedestal");
                if (oldPed != null) UnityEngine.Object.DestroyImmediate(oldPed.gameObject);
                GameObject ped = new GameObject("ShrineStonePedestal");
                ped.transform.SetParent(shrine.transform, false);
                ped.transform.localPosition = new Vector3(0f, -0.35f, 0f);
                var sr = ped.AddComponent<SpriteRenderer>();
                sr.sprite = pedSpr;
                sr.sortingLayerName = "GroundDecor";
                sr.sortingOrder = 12;
                EditorUtility.SetDirty(ped);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void AttachOrbRegistrar(GameObject target, Sprite sprOrb, Sprite sprSurge)
        {
            StaminaOrbArtRegistrar reg = target.GetComponent<StaminaOrbArtRegistrar>();
            if (reg == null) reg = target.AddComponent<StaminaOrbArtRegistrar>();
            SerializedObject rso = new SerializedObject(reg);
            rso.FindProperty("staminaOrbSprite").objectReferenceValue = sprOrb;
            rso.FindProperty("staminaSurgeSprite").objectReferenceValue = sprSurge;
            rso.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(reg);
        }

        private static GameObject RecreateChild(Transform parent, string name)
        {
            Transform old = parent.Find(name);
            if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            return go;
        }

        private static void CreateTile(Transform parent, string name, Sprite spr, Vector3 localPos, int order, string layer = "GroundDecor")
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = layer;
            sr.sortingOrder = order;
        }

        private static void ConfigureSprite(string path, float ppu, Vector2 pivot)
        {
            TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null)
            {
                Debug.LogWarning("[MV][Ch2-P2] Khong tim thay TextureImporter cho: " + path);
                return;
            }
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.spritePixelsPerUnit = ppu;
            imp.filterMode = FilterMode.Point;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            var settings = new TextureImporterSettings();
            imp.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = pivot;
            imp.SetTextureSettings(settings);
            imp.SaveAndReimport();
        }

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P2 - 2. Validate Step Ch2-P2")]
        public static void ValidateStepCh2P2()
        {
            int errors = 0;
            for (int i = 0; i < BridgeSprites.Length; i++)
                if (AssetDatabase.LoadAssetAtPath<Sprite>(BridgeSprites[i]) == null) { Debug.LogError("[MV][Ch2-P2][FAIL] Thieu sprite: " + BridgeSprites[i]); errors++; }
            for (int i = 0; i < PropSprites.Length; i++)
                if (AssetDatabase.LoadAssetAtPath<Sprite>(PropSprites[i]) == null) { Debug.LogError("[MV][Ch2-P2][FAIL] Thieu sprite: " + PropSprites[i]); errors++; }
            for (int i = 0; i < LanternSprites.Length; i++)
                if (AssetDatabase.LoadAssetAtPath<Sprite>(LanternSprites[i]) == null) { Debug.LogError("[MV][Ch2-P2][FAIL] Thieu sprite: " + LanternSprites[i]); errors++; }

            if (File.Exists(RiverbankScenePath))
            {
                EditorSceneManager.OpenScene(RiverbankScenePath, OpenSceneMode.Single);
                var bridge = UnityEngine.Object.FindAnyObjectByType<RiverBridgeController>();
                if (bridge == null) { Debug.LogError("[MV][Ch2-P2][FAIL] Thieu RiverBridgeController!"); errors++; }
                else if (bridge.GetComponent<StaminaOrbArtRegistrar>() == null) { Debug.LogError("[MV][Ch2-P2][FAIL] RiverBridge thieu StaminaOrbArtRegistrar!"); errors++; }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-P2] VALIDATE PASS (0 loi)! 22 sprite pixel-art moi da san sang va gan chuan.");
            else
                Debug.LogError($"[MV][Ch2-P2] VALIDATE FAIL ({errors} loi).");
        }
    }
}
