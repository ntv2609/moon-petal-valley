using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools {
    public static class AutoBuilder_StepCh2X5 {
        private const string RiverScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string BridgeDir = "Assets/Art/Sprites/Chapter2/Bridge";

        private static readonly string[] CatwalkSprites = {
            BridgeDir + "/bridge_catwalk_rope_mid.png",
            BridgeDir + "/bridge_catwalk_guard_top.png",
            BridgeDir + "/bridge_catwalk_guard_bot.png",
            BridgeDir + "/bridge_catwalk_anchor_west.png",
            BridgeDir + "/bridge_catwalk_anchor_east.png"
        };

        [MenuItem("Moonpetal/Step Ch2-X5/1. Build Rope Catwalk & Progressive Bridge Art", false, 560)]
        public static void ApplyStepCh2X5() {
            for (int i = 0; i < CatwalkSprites.Length; i++)
                ConfigureSprite(CatwalkSprites[i], 16f);
            AssetDatabase.Refresh();

            Scene scene = EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);
            RiverBridgeController bridge = Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge == null) {
                Debug.LogError("[MV][Ch2-X5] Khong tim thay RiverBridgeController trong " + RiverScenePath);
                return;
            }

            RebuildScaffoldingWithRopeCatwalk(bridge);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Ch2-X5] HOAN TAT: Da dung Cau Khi Day Thung tam (0%) + 3 nhip cau go tien trinh (35%/70%/95%)!");
            ValidateStepCh2X5();
        }

        [MenuItem("Moonpetal/Step Ch2-X5/2. Validate Step Ch2-X5", false, 561)]
        public static void ValidateStepCh2X5() {
            int errors = 0;
            Debug.Log("[MV][Ch2-X5] === BAT DAU VALIDATE STEP CH2-X5 ===");

            for (int i = 0; i < CatwalkSprites.Length; i++) {
                if (AssetDatabase.LoadAssetAtPath<Sprite>(CatwalkSprites[i]) == null) {
                    Debug.LogError("[MV][Ch2-X5][FAIL] Thieu sprite: " + CatwalkSprites[i]);
                    errors++;
                }
            }

            EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);
            RiverBridgeController bridge = Object.FindAnyObjectByType<RiverBridgeController>();
            if (bridge == null) {
                Debug.LogError("[MV][Ch2-X5][FAIL] Thieu RiverBridgeController!");
                errors++;
            } else {
                SerializedObject bso = new SerializedObject(bridge);
                GameObject scaf = bso.FindProperty("scaffoldingVisualsRoot").objectReferenceValue as GameObject;
                if (scaf == null || scaf.transform.Find("RopeCatwalkSpan") == null) {
                    Debug.LogError("[MV][Ch2-X5][FAIL] BridgeScaffoldingRoot thieu RopeCatwalkSpan!");
                    errors++;
                }
                var segs = bso.FindProperty("progressivePlankSegments");
                if (segs == null || segs.arraySize < 3) {
                    Debug.LogError("[MV][Ch2-X5][FAIL] Thieu 3 progressivePlankSegments!");
                    errors++;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-X5] === VALIDATE PASS 100% (0 LOI) ===");
            else
                Debug.LogError("[MV][Ch2-X5] === VALIDATE FAIL (" + errors + " LOI) ===");
        }

        private static void RebuildScaffoldingWithRopeCatwalk(RiverBridgeController bridge) {
            Sprite plankTop = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_plank_top.png");
            Sprite plankMid = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_plank_mid.png");
            Sprite plankBot = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_plank_bot.png");
            Sprite brkWest = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_broken_west.png");
            Sprite brkEast = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_broken_east.png");
            Sprite postSpr = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_post_pillar.png");

            Sprite ropeMid = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_catwalk_rope_mid.png");
            Sprite guardTop = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_catwalk_guard_top.png");
            Sprite guardBot = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_catwalk_guard_bot.png");
            Sprite anchorWest = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_catwalk_anchor_west.png");
            Sprite anchorEast = AssetDatabase.LoadAssetAtPath<Sprite>(BridgeDir + "/bridge_catwalk_anchor_east.png");

            GameObject scafRoot = RecreateChild(bridge.transform, "BridgeScaffoldingRoot");

            // 1. Keep broken West (x=-8..-5) and East (x=4..7) ends intact so 0% visibly starts from the broken bridge!
            GameObject brokenEnds = new GameObject("BrokenEndsBase");
            brokenEnds.transform.SetParent(scafRoot.transform, false);
            for (int y = -1; y <= 1; y++) {
                Sprite baseRow = y == 1 ? plankTop : (y == -1 ? plankBot : plankMid);
                for (int x = -8; x <= 7; x++) {
                    if (x > -5 && x < 4) continue;
                    Sprite useSpr = baseRow;
                    if (x == -5) useSpr = brkWest != null ? brkWest : baseRow;
                    else if (x == 4) useSpr = brkEast != null ? brkEast : baseRow;
                    CreateTile(brokenEnds.transform, $"End_{x}_{y}", useSpr, new Vector3(x + 0.5f, y + 0.5f, 0f), 8);
                }
            }
            AddCornerPillars(brokenEnds.transform, postSpr);

            // 2. Narrow Rope Catwalk across the broken gap (x = -4..3) with open river water visible at y=1 and y=-1!
            GameObject ropeSpan = new GameObject("RopeCatwalkSpan");
            ropeSpan.transform.SetParent(scafRoot.transform, false);
            for (int x = -4; x <= 3; x++) {
                Sprite midSpr = x == -4 ? anchorWest : (x == 3 ? anchorEast : ropeMid);
                CreateTile(ropeSpan.transform, $"CatwalkMid_{x}", midSpr, new Vector3(x + 0.5f, 0.5f, 0f), 7);
                CreateTile(ropeSpan.transform, $"CatwalkTop_{x}", guardTop, new Vector3(x + 0.5f, 1.5f, 0f), 7);
                CreateTile(ropeSpan.transform, $"CatwalkBot_{x}", guardBot, new Vector3(x + 0.5f, -0.5f, 0f), 7);
            }

            // 3. Three progressive hardwood deck segments on top of the rope catwalk (35% -> 70% -> 95%)
            GameObject seg1 = CreateProgressiveSegment(scafRoot.transform, "BuildSegment_1_East", plankTop, plankMid, plankBot, 1, 4);
            GameObject seg2 = CreateProgressiveSegment(scafRoot.transform, "BuildSegment_2_West", plankTop, plankMid, plankBot, -4, -1);
            GameObject seg3 = CreateProgressiveSegment(scafRoot.transform, "BuildSegment_3_Center", plankTop, plankMid, plankBot, 0, 0);
            seg1.SetActive(false);
            seg2.SetActive(false);
            seg3.SetActive(false);
            scafRoot.SetActive(false);

            SerializedObject bso = new SerializedObject(bridge);
            bso.FindProperty("scaffoldingVisualsRoot").objectReferenceValue = scafRoot;
            var segProp = bso.FindProperty("progressivePlankSegments");
            segProp.ClearArray();
            GameObject[] segs = { seg1, seg2, seg3 };
            for (int i = 0; i < segs.Length; i++) {
                segProp.InsertArrayElementAtIndex(i);
                segProp.GetArrayElementAtIndex(i).objectReferenceValue = segs[i];
            }
            bso.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bridge);
        }

        private static GameObject CreateProgressiveSegment(Transform parent, string name, Sprite top, Sprite mid, Sprite bot, int xMin, int xMax) {
            GameObject grp = new GameObject(name);
            grp.transform.SetParent(parent, false);
            for (int y = -1; y <= 1; y++) {
                Sprite rowSpr = y == 1 ? top : (y == -1 ? bot : mid);
                for (int x = xMin; x <= xMax; x++)
                    CreateTile(grp.transform, $"Seg_{x}_{y}", rowSpr, new Vector3(x + 0.5f, y + 0.5f, 0f), 10);
            }
            return grp;
        }

        private static void AddCornerPillars(Transform parent, Sprite postSpr) {
            if (postSpr == null) return;
            CreateTile(parent, "Post_NW", postSpr, new Vector3(-7.5f, 1.85f, 0f), 14, "WorldObjects");
            CreateTile(parent, "Post_NE", postSpr, new Vector3(6.5f, 1.85f, 0f), 14, "WorldObjects");
            CreateTile(parent, "Post_SW", postSpr, new Vector3(-7.5f, -0.75f, 0f), 14, "WorldObjects");
            CreateTile(parent, "Post_SE", postSpr, new Vector3(6.5f, -0.75f, 0f), 14, "WorldObjects");
        }

        private static GameObject RecreateChild(Transform parent, string name) {
            Transform old = parent.Find(name);
            if (old != null) Object.DestroyImmediate(old.gameObject);
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void CreateTile(Transform parent, string name, Sprite spr, Vector3 localPos, int order, string layer = "Ground") {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = layer;
            sr.sortingOrder = order;
        }

        private static void ConfigureSprite(string path, float ppu) {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) return;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = ppu;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.alphaIsTransparency = true;
            var s = new TextureImporterSettings();
            ti.ReadTextureSettings(s);
            s.spriteMeshType = SpriteMeshType.FullRect;
            s.spriteAlignment = (int)SpriteAlignment.Center;
            s.spritePivot = new Vector2(0.5f, 0.5f);
            ti.SetTextureSettings(s);
            ti.SaveAndReimport();
        }
    }
}
