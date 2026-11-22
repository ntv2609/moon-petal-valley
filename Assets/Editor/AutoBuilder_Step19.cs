using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Combat;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step19
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string BundlePath = "Assets/Data/Bundles/Bundle_ForgottenGarden.asset";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";

        [MenuItem("Moonpetal/Step 19/1. Apply Atmosphere, Tall Grass, Lanterns & Rename Ông Bach")]
        public static void BuildStep19()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 19 (CHẶNG 3 - KHÍ SẮC & ĐỔI TÊN ÔNG BACH) ===");
            AssetDatabase.Refresh();
            ForceReimportDecorFolder();
            AssetDatabase.Refresh();

            // 1. Rename "Ông Bách" -> "Ông Bach" across Assets & Prefab
            RenameBachInAssetsAndPrefab();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 2. Rename "Ông Bách" -> "Ông Bach" in Scene components & TMP prompts
            RenameBachInScene();

            // 3. Upgrade Village Lanterns + Build Tall Grass, Flower Planters & Soft Golden Sunbeams
            UpgradeVillageLanterns();
            BuildAtmosphereDetails();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 19 (CHẶNG 3)! Chạy 'Moonpetal/Step 19/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 19/Validate")]
        public static void ValidateStep19()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 19 ===");

            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            if (table != null)
            {
                int remaining = 0;
                for (int i = 0; i < table.entries.Count; i++)
                {
                    if (!string.IsNullOrEmpty(table.entries[i].vietnameseText) &&
                        (table.entries[i].vietnameseText.Contains("Bách") || table.entries[i].vietnameseText.Contains("bách")))
                    {
                        remaining++;
                    }
                }
                if (remaining > 0)
                {
                    Debug.LogError($"[MV] VNStringTable vẫn còn {remaining} chuỗi chứa 'Bách'.");
                    errors++;
                }
            }

            GameObject obsGo = GameObject.Find("ObstacleTilemap");
            var tmCol = obsGo != null ? obsGo.GetComponent<TilemapCollider2D>() : null;
            if (tmCol == null || tmCol.shapeCount < 100)
            {
                Debug.LogError("[MV] ObstacleTilemap collider bất thường! Hãy đảm bảo đã chạy Step 18 Master Fix.");
                errors++;
            }

            if (GameObject.Find("Step19AtmosphereGroup") == null)
            {
                Debug.LogError("[MV] Thiếu Step19AtmosphereGroup.");
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
                Debug.Log($"[MV] VALIDATE STEP 19: PASS (0 lỗi | Ông Bach=100% | Atmosphere=OK | ObstacleShapes={tmCol.shapeCount}).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 19: FAIL ({errors} lỗi).");
            }
        }

        private static string ReplaceBach(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace("Ông Bách", "Ông Bach")
                    .Replace("ông Bách", "ông Bach")
                    .Replace("bạn già Bách", "bạn già Bach");
        }

        private static Sprite LoadEnv(string rel) => AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel);

        private static void ForceReimportDecorFolder()
        {
            string folder = "Assets/Art/Sprites/Environment/Decor";
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

        private static void RenameBachInAssetsAndPrefab()
        {
            int count = 0;

            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            if (table != null)
            {
                for (int i = 0; i < table.entries.Count; i++)
                {
                    var e = table.entries[i];
                    string updated = ReplaceBach(e.vietnameseText);
                    if (updated != e.vietnameseText)
                    {
                        e.vietnameseText = updated;
                        table.entries[i] = e;
                        count++;
                    }
                }
                EditorUtility.SetDirty(table);
            }

            var bundle = AssetDatabase.LoadAssetAtPath<BundleData>(BundlePath);
            if (bundle != null)
            {
                bundle.bundleDescriptionKey = ReplaceBach(bundle.bundleDescriptionKey);
                EditorUtility.SetDirty(bundle);
            }

            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg != null)
            {
                for (int i = 0; i < cfg.spawnPoints.Count; i++)
                {
                    var sp = cfg.spawnPoints[i];
                    sp.displayName = ReplaceBach(sp.displayName);
                    cfg.spawnPoints[i] = sp;
                }
                EditorUtility.SetDirty(cfg);
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
                var tmps = root.GetComponentsInChildren<TMP_Text>(true);
                for (int i = 0; i < tmps.Length; i++)
                {
                    string updated = ReplaceBach(tmps[i].text);
                    if (updated != tmps[i].text)
                    {
                        tmps[i].text = updated;
                        count++;
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }

            Debug.Log($"[MV] Đã đổi tên 'Ông Bách' -> 'Ông Bach' tại {count} chuỗi trong Assets & GameRoot.prefab.");
        }

        private static void RenameBachInScene()
        {
            int count = 0;

            var tmps = UnityEngine.Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
            for (int i = 0; i < tmps.Length; i++)
            {
                string updated = ReplaceBach(tmps[i].text);
                if (updated != tmps[i].text)
                {
                    tmps[i].text = updated;
                    EditorUtility.SetDirty(tmps[i]);
                    count++;
                }
            }

            var props = UnityEngine.Object.FindObjectsByType<StoryPropInteractable>(FindObjectsInactive.Include);
            for (int i = 0; i < props.Length; i++)
            {
                var so = new SerializedObject(props[i]);
                foreach (string field in new[] { "dialogueSpeaker", "dialogueBody", "blockedHint" })
                {
                    var p = so.FindProperty(field);
                    if (p != null)
                    {
                        string u = ReplaceBach(p.stringValue);
                        if (u != p.stringValue) { p.stringValue = u; count++; }
                    }
                }
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(props[i]);
            }

            var zones = UnityEngine.Object.FindObjectsByType<BlightEncounterZone>(FindObjectsInactive.Include);
            for (int i = 0; i < zones.Length; i++)
            {
                var so = new SerializedObject(zones[i]);
                var p = so.FindProperty("completionToast");
                if (p != null)
                {
                    string u = ReplaceBach(p.stringValue);
                    if (u != p.stringValue) { p.stringValue = u; count++; }
                }
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(zones[i]);
            }

            var reg = UnityEngine.Object.FindAnyObjectByType<WorldSpawnRegistry>();
            if (reg != null)
            {
                var so = new SerializedObject(reg);
                var list = so.FindProperty("spawnPoints");
                for (int i = 0; i < list.arraySize; i++)
                {
                    var dn = list.GetArrayElementAtIndex(i).FindPropertyRelative("displayName");
                    if (dn != null)
                    {
                        string u = ReplaceBach(dn.stringValue);
                        if (u != dn.stringValue) { dn.stringValue = u; count++; }
                    }
                }
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(reg);
            }

            Debug.Log($"[MV] Đã đổi tên 'Ông Bách' -> 'Ông Bach' tại {count} vị trí trong Scene.");
        }

        private static void UpgradeVillageLanterns()
        {
            Sprite lanternSpr = LoadEnv("Decor/decor_garden_lantern.png");
            if (lanternSpr == null) return;

            var lanterns = UnityEngine.Object.FindObjectsByType<VillageLantern>(FindObjectsInactive.Include);
            for (int i = 0; i < lanterns.Length; i++)
            {
                var sr = lanterns[i].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = lanternSpr;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(lanterns[i].transform.position.y - 0.5f) * 100f);
                }
                lanterns[i].gameObject.layer = 10;
                var col = lanterns[i].GetComponent<BoxCollider2D>();
                if (col == null) col = lanterns[i].gameObject.AddComponent<BoxCollider2D>();
                col.size = new Vector2(0.45f, 0.35f);
                col.offset = new Vector2(0f, -0.55f);
                EditorUtility.SetDirty(lanterns[i].gameObject);
            }
            Debug.Log($"[MV] Đã nâng cấp {lanterns.Length} Đèn Lồng Làng (art mới + chân va chạm gọn).");
        }

        private static void BuildAtmosphereDetails()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) decorGroup = new GameObject("VillageDecorGroup");

            Transform old = decorGroup.transform.Find("Step19AtmosphereGroup");
            if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);

            GameObject group = new GameObject("Step19AtmosphereGroup");
            group.transform.SetParent(decorGroup.transform, false);

            Sprite tg0 = LoadEnv("Decor/decor_tall_grass_0.png");
            Sprite tg1 = LoadEnv("Decor/decor_tall_grass_1.png");
            Sprite planterSpr = LoadEnv("Decor/decor_flower_planter.png");
            Sprite sunSpr = LoadEnv("Decor/spr_sunbeam_glow.png");

            // 1. 24 Walkable Tall Grass Tufts across open meadows (FloorDecal, ZERO colliders)
            Vector3[] grassSpots =
            {
                new Vector3(-18.2f, 3.8f, 0f), new Vector3(-16.8f, 1.4f, 0f), new Vector3(-13.6f, 0.8f, 0f),
                new Vector3(-10.8f, 2.2f, 0f), new Vector3(-8.4f, 0.4f, 0f),  new Vector3(-14.6f, -3.6f, 0f),
                new Vector3(-11.4f, -6.2f, 0f), new Vector3(-15.2f, -10.4f, 0f), new Vector3(-9.6f, -12.4f, 0f),
                new Vector3(-4.2f, 12.2f, 0f), new Vector3(1.4f, 13.6f, 0f), new Vector3(6.8f, 11.4f, 0f),
                new Vector3(11.6f, 10.2f, 0f), new Vector3(14.8f, 8.6f, 0f), new Vector3(16.6f, 2.6f, 0f),
                new Vector3(12.2f, -0.8f, 0f), new Vector3(9.8f, -4.4f, 0f), new Vector3(13.8f, -8.8f, 0f),
                new Vector3(9.2f, -12.2f, 0f), new Vector3(-4.8f, -7.8f, 0f), new Vector3(4.8f, -7.8f, 0f),
                new Vector3(-4.6f, -11.4f, 0f), new Vector3(4.6f, -11.4f, 0f), new Vector3(2.8f, 0.8f, 0f),
            };
            for (int i = 0; i < grassSpots.Length; i++)
            {
                Sprite sp = (i % 2 == 0) ? tg0 : tg1;
                if (sp == null) continue;
                GameObject go = new GameObject($"TallGrass_{i}");
                go.transform.SetParent(group.transform, false);
                go.transform.position = grassSpots[i];
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sp;
                sr.sortingLayerName = "FloorDecal";
                sr.sortingOrder = 7;
            }

            // 2. 4 Cozy Wooden Flower Planters framing buildings & Greenhouse door
            Vector3[] planterSpots =
            {
                new Vector3(-17.4f, 4.4f, 0f), // Left of Greenhouse door
                new Vector3(-13.6f, 4.4f, 0f), // Right of Greenhouse door
                new Vector3(-8.8f, 3.8f, 0f),  // Front-left of Hana's Bakery
                new Vector3(-3.0f, -5.8f, 0f), // Front-right of Bach's Shed
            };
            for (int i = 0; i < planterSpots.Length; i++)
            {
                if (planterSpr == null) continue;
                GameObject go = new GameObject($"CozyFlowerPlanter_{i}");
                go.layer = 10;
                go.transform.SetParent(group.transform, false);
                go.transform.position = planterSpots[i];
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = planterSpr;
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = Mathf.RoundToInt(-(planterSpots[i].y - 0.35f) * 100f);
                var col = go.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1.25f, 0.55f);
                col.offset = new Vector2(0f, -0.15f);
            }

            // 3. 6 Soft Golden Sunbeam Pools (WeatherVFX, non-blocking)
            Vector3[] sunSpots =
            {
                new Vector3(-15.5f, 8.5f, 0f), // Greenhouse interior glow
                new Vector3(-3.0f, 12.6f, 0f), // Curved Pond shimmer
                new Vector3(-5.8f, 4.2f, 0f),  // Bakery plaza
                new Vector3(0.0f, 1.5f, 0f),   // Village Square
                new Vector3(0.0f, -8.5f, 0f),  // South Garden Shrine
                new Vector3(15.5f, 6.8f, 0f),  // Forest Edge glade
            };
            for (int i = 0; i < sunSpots.Length; i++)
            {
                if (sunSpr == null) continue;
                GameObject go = new GameObject($"SunbeamPool_{i}");
                go.transform.SetParent(group.transform, false);
                go.transform.position = sunSpots[i];
                go.transform.localScale = new Vector3(2.2f, 1.6f, 1f);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sunSpr;
                sr.sortingLayerName = "WeatherVFX";
                sr.sortingOrder = 1;
            }

            Debug.Log("[MV] Đã hoàn thiện Chặng 3: 24 cụm cỏ cao, 4 bồn hoa gỗ và 6 vệt nắng dịu toàn thung lũng.");
        }
    }
}
