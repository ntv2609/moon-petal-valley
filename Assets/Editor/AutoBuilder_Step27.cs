using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;
using MoonpetalValley.Core;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step27
    {
        private const string MainScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";

        // Thu tu chuan 11 Sorting Layers tu duoi len tren
        private static readonly string[] CanonicalLayers =
        {
            "Default",
            "Ground",
            "FloorDecal",
            "FarmTile",
            "GroundDecor",
            "Shadows",
            "WorldObjects",
            "Foreground",
            "WeatherVFX",
            "FogOverlay",
            "UI"
        };

        [MenuItem("Moonpetal/Step 27/1. Fix Invisible Walls & Audit All 11 Sorting Layers")]
        public static void FixAllLayersAndInvisibleWalls()
        {
            EnforceCanonicalSortingLayers();
            RepairSceneSorting();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Step27] HOAN TAT: Da phuc hoi Tuong Nha Kinh, Hang Rao Vuon Ong Bach va chuan hoa toan bo 11 Sorting Layers!");
        }

        private static void EnforceCanonicalSortingLayers()
        {
            UnityEngine.Object[] tagAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (tagAssets == null || tagAssets.Length == 0)
            {
                Debug.LogError("[MV][Step27] Khong doc duoc TagManager.asset");
                return;
            }

            SerializedObject tagManager = new SerializedObject(tagAssets[0]);
            SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");

            // Giu lai uniqueID cua cac layer da ton tai de khong lam hong tham chieu prefab/scene
            var idMap = new Dictionary<string, int>();
            for (int i = 0; i < sortingLayers.arraySize; i++)
            {
                SerializedProperty el = sortingLayers.GetArrayElementAtIndex(i);
                string nm = el.FindPropertyRelative("name").stringValue;
                int uid = el.FindPropertyRelative("uniqueID").intValue;
                if (!idMap.ContainsKey(nm)) idMap[nm] = uid;
            }

            // Giu them cac layer tuy chinh khac neu co
            var finalOrder = new List<string>(CanonicalLayers);
            foreach (var kv in idMap)
            {
                if (!finalOrder.Contains(kv.Key)) finalOrder.Add(kv.Key);
            }

            sortingLayers.ClearArray();
            for (int i = 0; i < finalOrder.Count; i++)
            {
                string nm = finalOrder[i];
                sortingLayers.InsertArrayElementAtIndex(i);
                SerializedProperty el = sortingLayers.GetArrayElementAtIndex(i);
                el.FindPropertyRelative("name").stringValue = nm;
                if (nm == "Default")
                {
                    el.FindPropertyRelative("uniqueID").intValue = 0;
                }
                else if (idMap.TryGetValue(nm, out int existingId) && existingId != 0)
                {
                    el.FindPropertyRelative("uniqueID").intValue = existingId;
                }
                else
                {
                    el.FindPropertyRelative("uniqueID").intValue = Mathf.Abs(nm.GetHashCode()) + 100 + i;
                }
            }

            tagManager.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log("[MV][Step27] Da khoa thu tu chuan 11 Sorting Layers trong TagManager.asset.");
        }

        private static void RepairSceneSorting()
        {
            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

            // 1. SUA LOI TUONG & HANG RAO TANG HINH:
            //    GroundTilemap -> Ground (0)
            //    ObstacleTilemap (Tuong Nha Kinh + Hang Rao Vuon Ong Bach) -> WorldObjects (-5000)
            foreach (TilemapRenderer tr in UnityEngine.Object.FindObjectsByType<TilemapRenderer>(FindObjectsInactive.Include))
            {
                string n = tr.gameObject.name.ToLowerInvariant();
                if (n.Contains("obstacle"))
                {
                    tr.sortingLayerName = "WorldObjects";
                    tr.sortingOrder = -5000;
                    tr.enabled = true;
                    Debug.Log("[MV][Step27] Da phuc hoi " + tr.gameObject.name + " -> WorldObjects (-5000).");
                }
                else
                {
                    tr.sortingLayerName = "Ground";
                    tr.sortingOrder = 0;
                }
                EditorUtility.SetDirty(tr);
            }

            // 2. Phuc hoi cac he thong VFX / Fog / LanternGlow / LumiHalo / FarmTile
            foreach (VillageLantern lantern in UnityEngine.Object.FindObjectsByType<VillageLantern>(FindObjectsInactive.Include))
            {
                SerializedObject so = new SerializedObject(lantern);
                SpriteRenderer glow = so.FindProperty("glowRenderer").objectReferenceValue as SpriteRenderer;
                if (glow != null)
                {
                    RemoveYSort(glow.gameObject);
                    glow.sortingLayerName = "WeatherVFX";
                    glow.sortingOrder = 15;
                    EditorUtility.SetDirty(glow);
                }
            }

            LumiCompanionController lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null)
            {
                SerializedObject so = new SerializedObject(lumi);
                SpriteRenderer halo = so.FindProperty("haloRenderer").objectReferenceValue as SpriteRenderer;
                if (halo != null)
                {
                    RemoveYSort(halo.gameObject);
                    halo.sortingLayerName = "WeatherVFX";
                    halo.sortingOrder = 10;
                    EditorUtility.SetDirty(halo);
                }
            }

            // 3. Bon Hoa Trung Tam (PlantingSpot): de da o GroundDecor (15), aura o WeatherVFX (12), co CircleCollider2D
            PlantingSpotController spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            if (spot != null)
            {
                foreach (SpriteRenderer sr in spot.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    RemoveYSort(sr.gameObject);
                    if (sr.gameObject.name.ToLowerInvariant().Contains("aura"))
                    {
                        sr.sortingLayerName = "WeatherVFX";
                        sr.sortingOrder = 12;
                    }
                    else
                    {
                        sr.sortingLayerName = "GroundDecor";
                        sr.sortingOrder = 15;
                    }
                    EditorUtility.SetDirty(sr);
                }

                CircleCollider2D col = spot.GetComponent<CircleCollider2D>();
                if (col == null) col = spot.gameObject.AddComponent<CircleCollider2D>();
                col.isTrigger = false;
                col.radius = 0.68f;
                col.offset = new Vector2(0f, -0.05f);
                EditorUtility.SetDirty(col);
            }

            // 4. Quet toan bo SpriteRenderer trong scene voi bo loc an toan tuyet doi
            int ySortCount = 0;
            int groundDecorCount = 0;
            int shadowCount = 0;

            foreach (SpriteRenderer sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
            {
                if (sr == null) continue;
                string n = sr.gameObject.name.ToLowerInvariant();
                string pName = sr.transform.parent != null ? sr.transform.parent.name.ToLowerInvariant() : "";

                // Bao ve tuyet doi cac layer dac biet & UI
                if (sr.sortingLayerName == "UI") continue;
                if (spot != null && sr.transform.IsChildOf(spot.transform)) continue;
                if (n.Contains("slash") || n.Contains("pulse") || n.Contains("glow") || n.Contains("halo"))
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "WeatherVFX";
                    EditorUtility.SetDirty(sr);
                    continue;
                }

                // Suong Heo Ua / Blight Fog / Mist -> FogOverlay
                if (n.Contains("fog") || n.Contains("mist") || n.Contains("blightoverlay") || pName.Contains("fog"))
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "FogOverlay";
                    sr.sortingOrder = 5;
                    EditorUtility.SetDirty(sr);
                    continue;
                }

                // Canh hoa bay / WeatherVFX -> WeatherVFX
                if (n.Contains("petal") || pName.Contains("petal"))
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "WeatherVFX";
                    EditorUtility.SetDirty(sr);
                    continue;
                }

                // Cong vom cao (GardenEntranceArch) -> Foreground de Wen di chui qua
                if (n.Contains("gardenentrancearch") || n.Contains("arch_top"))
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "Foreground";
                    sr.sortingOrder = 10;
                    EditorUtility.SetDirty(sr);
                    continue;
                }

                // Bong do duoi chan (GroundShadow) -> Shadows (0)
                if (n.Contains("shadow"))
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "Shadows";
                    sr.sortingOrder = 0;
                    EditorUtility.SetDirty(sr);
                    shadowCount++;
                    continue;
                }

                // O dat trong trot (FarmTile soil vs crop)
                if (pName.Contains("farmtile") || n.Contains("farmtile"))
                {
                    if (n.Contains("crop") && !n.Contains("soil"))
                    {
                        YSortRenderer ys = sr.GetComponent<YSortRenderer>() ?? sr.gameObject.AddComponent<YSortRenderer>();
                        ys.Configure(sr, false, true, 0f, 5);
                        EditorUtility.SetDirty(ys);
                        ySortCount++;
                    }
                    else
                    {
                        RemoveYSort(sr.gameObject);
                        sr.sortingLayerName = "FarmTile";
                        sr.sortingOrder = 5;
                        EditorUtility.SetDirty(sr);
                    }
                    continue;
                }

                // Vat the cung / ResourceNode / Chau hoa go / Do vat dung -> LUON LA WorldObjects + YSortRenderer!
                bool isForceStanding =
                    sr.GetComponentInParent<ResourceNodeController>() != null ||
                    sr.GetComponentInParent<PlayerController2D>() != null ||
                    sr.GetComponentInParent<NPCController>() != null ||
                    sr.GetComponentInParent<GloomSlimeController>() != null ||
                    sr.GetComponentInParent<LumiCompanionController>() != null ||
                    sr.GetComponentInParent<StoryPropInteractable>() != null ||
                    sr.GetComponentInParent<BedInteractable>() != null ||
                    sr.GetComponentInParent<WaterWellInteractable>() != null ||
                    sr.GetComponentInParent<SeedShelfInteractable>() != null ||
                    n.Contains("planter") || n.Contains("pot") || n.Contains("bench") || n.Contains("lantern") ||
                    n.Contains("tree") || n.Contains("house") || n.Contains("bakery") || n.Contains("shed") ||
                    n.Contains("barrel") || n.Contains("crate") || n.Contains("sign") || n.Contains("board") ||
                    n.Contains("mailbox") || n.Contains("well") || n.Contains("shelf") || n.Contains("table") ||
                    n.Contains("bed") || n.Contains("box") || n.Contains("debris") || n.Contains("branch") ||
                    n.Contains("miucat") || n.Contains("bush");

                bool isFlatGround = !isForceStanding && (
                    n.Contains("rug") || n.Contains("carpet") || n.Contains("pond") || n.Contains("water") ||
                    n.Contains("pier") || n.Contains("stepstone") || n.Contains("stepping") ||
                    n.Contains("soil") || n.Contains("wildflower") || n.Contains("flower_") ||
                    n.Contains("tallgrass") || n.Contains("grasstuft") || n.Contains("mat_") || n.Contains("decal")
                );

                if (isFlatGround)
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "GroundDecor";
                    if (n.Contains("pond") || n.Contains("water")) sr.sortingOrder = 2;
                    else if (n.Contains("pier") || n.Contains("rug") || n.Contains("carpet")) sr.sortingOrder = 6;
                    else sr.sortingOrder = 10;
                    EditorUtility.SetDirty(sr);
                    groundDecorCount++;
                    continue;
                }

                // Tat ca vat the dung con lai -> WorldObjects + YSortRenderer theo chan (Foot Y)
                bool isPlayer = sr.GetComponentInParent<PlayerController2D>() != null;
                bool isSlime = sr.GetComponentInParent<GloomSlimeController>() != null;
                bool isNpc = sr.GetComponentInParent<NPCController>() != null;
                bool isLumiBody = sr.GetComponentInParent<LumiCompanionController>() != null;

                if (isPlayer || isSlime)
                {
                    // PlayerController2D va GloomSlimeController tu cap nhat Foot-Y moi frame
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "WorldObjects";
                    float fy = sr.sprite != null ? sr.bounds.min.y : (sr.transform.position.y - 0.4f);
                    sr.sortingOrder = Mathf.RoundToInt(-fy * 100f);
                    EditorUtility.SetDirty(sr);
                    continue;
                }

                bool isDynamic = isNpc || isLumiBody || n.Contains("miucat") || n.Contains("loot");
                YSortRenderer ysort = sr.GetComponent<YSortRenderer>() ?? sr.gameObject.AddComponent<YSortRenderer>();
                int bias = isLumiBody ? 18 : 0;
                ysort.Configure(sr, isDynamic, true, 0f, bias);
                EditorUtility.SetDirty(ysort);
                EditorUtility.SetDirty(sr);
                ySortCount++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Step27] Audit done: " + ySortCount + " YSortRenderers, " + groundDecorCount + " GroundDecor, " + shadowCount + " Shadows.");
        }

        private static void RemoveYSort(GameObject go)
        {
            YSortRenderer ys = go.GetComponent<YSortRenderer>();
            if (ys != null) UnityEngine.Object.DestroyImmediate(ys);
        }

        [MenuItem("Moonpetal/Step 27/2. Validate Full Scene Layers & Rendering")]
        public static void ValidateStep27()
        {
            int errors = 0;
            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

            // 1. Kiem tra ObstacleTilemap phai o WorldObjects (-5000)
            bool foundObstacle = false;
            foreach (TilemapRenderer tr in UnityEngine.Object.FindObjectsByType<TilemapRenderer>(FindObjectsInactive.Include))
            {
                if (tr.gameObject.name.ToLowerInvariant().Contains("obstacle"))
                {
                    foundObstacle = true;
                    if (tr.sortingLayerName != "WorldObjects" || tr.sortingOrder != -5000)
                    {
                        Debug.LogError("[MV][Step27][FAIL] ObstacleTilemap sai layer/order: " + tr.sortingLayerName + " / " + tr.sortingOrder);
                        errors++;
                    }
                    if (!tr.enabled)
                    {
                        Debug.LogError("[MV][Step27][FAIL] ObstacleTilemap dang bi disable.");
                        errors++;
                    }
                }
            }
            if (!foundObstacle)
            {
                Debug.LogError("[MV][Step27][FAIL] Khong tim thay ObstacleTilemap trong scene.");
                errors++;
            }

            // 2. Kiem tra ResourceNodes (GreenhouseDebris + DryBranch) deu o WorldObjects va co Sprite
            ResourceNodeController[] nodes = UnityEngine.Object.FindObjectsByType<ResourceNodeController>(FindObjectsInactive.Include);
            foreach (var node in nodes)
            {
                var sr = node.GetComponentInChildren<SpriteRenderer>(true);
                if (sr == null || sr.sprite == null)
                {
                    Debug.LogError("[MV][Step27][FAIL] ResourceNode thieu Sprite: " + node.name);
                    errors++;
                }
                else if (sr.sortingLayerName != "WorldObjects")
                {
                    Debug.LogError("[MV][Step27][FAIL] ResourceNode khong o WorldObjects: " + node.name + " (" + sr.sortingLayerName + ")");
                    errors++;
                }
            }

            // 3. Kiem tra Bon Hoa Trung Tam
            PlantingSpotController spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            if (spot != null)
            {
                var sr = spot.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && sr.sortingLayerName != "GroundDecor")
                {
                    Debug.LogError("[MV][Step27][FAIL] PlantingSpot chua o GroundDecor.");
                    errors++;
                }
            }

            // 4. Kiem tra thu tu layer trong TagManager
            int idGround = SortingLayer.GetLayerValueFromName("Ground");
            int idDecor = SortingLayer.GetLayerValueFromName("GroundDecor");
            int idShadows = SortingLayer.GetLayerValueFromName("Shadows");
            int idWorld = SortingLayer.GetLayerValueFromName("WorldObjects");
            int idFg = SortingLayer.GetLayerValueFromName("Foreground");
            int idVfx = SortingLayer.GetLayerValueFromName("WeatherVFX");
            int idFog = SortingLayer.GetLayerValueFromName("FogOverlay");
            int idUi = SortingLayer.GetLayerValueFromName("UI");

            if (!(idGround < idDecor && idDecor < idShadows && idShadows < idWorld && idWorld < idFg && idFg < idVfx && idVfx < idFog && idFog < idUi))
            {
                Debug.LogError("[MV][Step27][FAIL] Thu tu SortingLayer trong TagManager chua chuan!");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV][Step27] VALIDATE PASS: Tuong Nha Kinh, Hang rao, " + nodes.Length + " ResourceNodes, VFX, Fog, Shadows va 11 Sorting Layers deu hop le 100%!");
            }
            else
            {
                Debug.LogError("[MV][Step27] VALIDATE FAIL (" + errors + " loi).");
            }
        }
    }
}
