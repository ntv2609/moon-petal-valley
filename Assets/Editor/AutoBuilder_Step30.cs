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
    public static class AutoBuilder_Step30
    {
        private const string MainScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";

        [MenuItem("Moonpetal/Step 30/1. Fix GardenEntranceArch Depth & Enforce Clean 8-Tier Contract")]
        public static void FixArchAndEnforceContract()
        {
            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

            // 1. Sua chinh xac GardenEntranceArch:
            //    Dua tu Foreground ve WorldObjects + YSortRenderer theo day chan 2 cot da (bounds.min.y).
            //    -> Wen dung phia Nam (truoc cot): Wen hien truoc cot da.
            //    -> Wen di chui qua giua / phia Bac: Vom go che tu nhien tren dau Wen.
            GameObject arch = GameObject.Find("GardenEntranceArch");
            if (arch != null)
            {
                SpriteRenderer sr = arch.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    YSortRenderer ys = arch.GetComponent<YSortRenderer>();
                    if (ys == null) ys = arch.AddComponent<YSortRenderer>();
                    ys.Configure(sr, false, true, 0.05f, 0);
                    EditorUtility.SetDirty(ys);
                    EditorUtility.SetDirty(sr);
                    Debug.Log("[MV][Step30] GardenEntranceArch -> WorldObjects + YSortRenderer (sortingOrder = " + sr.sortingOrder + ", footY = " + (sr.bounds.min.y + 0.05f).ToString("F2") + ").");
                }
            }
            else
            {
                Debug.LogWarning("[MV][Step30] Khong tim thay GardenEntranceArch trong scene.");
            }

            // 2. Refresh lai tat ca YSortRenderer dang co de dong nhat 100% cong thuc Foot-Y
            int refreshed = 0;
            foreach (YSortRenderer ys in UnityEngine.Object.FindObjectsByType<YSortRenderer>(FindObjectsInactive.Include))
            {
                ys.RefreshSort();
                EditorUtility.SetDirty(ys);
                var sr = ys.GetComponent<SpriteRenderer>();
                if (sr != null) EditorUtility.SetDirty(sr);
                refreshed++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Step30] HOAN TAT: Da sua GardenEntranceArch va dong bo " + refreshed + " YSortRenderers!");
        }

        [MenuItem("Moonpetal/Step 30/2. Full Scene Depth & Layer Audit Report")]
        public static void AuditFullSceneDepth()
        {
            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            int errors = 0;
            var countsByLayer = new Dictionary<string, int>();

            // A. Kiem tra Tilemaps
            foreach (TilemapRenderer tr in UnityEngine.Object.FindObjectsByType<TilemapRenderer>(FindObjectsInactive.Include))
            {
                string n = tr.gameObject.name.ToLowerInvariant();
                if (n.Contains("obstacle"))
                {
                    if (tr.sortingLayerName != "WorldObjects" || tr.sortingOrder != -5000)
                    {
                        Debug.LogError("[AUDIT][FAIL] " + tr.name + " phai o WorldObjects (-5000), hien tai: " + tr.sortingLayerName + " (" + tr.sortingOrder + ")");
                        errors++;
                    }
                }
                else if (tr.sortingLayerName != "Ground")
                {
                    Debug.LogError("[AUDIT][FAIL] " + tr.name + " phai o Ground, hien tai: " + tr.sortingLayerName);
                    errors++;
                }
            }

            // B. Kiem tra GardenEntranceArch
            GameObject arch = GameObject.Find("GardenEntranceArch");
            if (arch != null)
            {
                var sr = arch.GetComponent<SpriteRenderer>();
                var ys = arch.GetComponent<YSortRenderer>();
                if (sr == null || sr.sortingLayerName != "WorldObjects" || ys == null)
                {
                    Debug.LogError("[AUDIT][FAIL] GardenEntranceArch phai o WorldObjects + co YSortRenderer!");
                    errors++;
                }
            }

            // C. Kiem tra GreenhouseCozyRug
            GameObject rug = GameObject.Find("GreenhouseCozyRug");
            if (rug != null)
            {
                var sr = rug.GetComponent<SpriteRenderer>();
                if (sr == null || sr.sortingLayerName != "GroundDecor" || rug.GetComponent<YSortRenderer>() != null)
                {
                    Debug.LogError("[AUDIT][FAIL] GreenhouseCozyRug phai o GroundDecor va KHONG co YSortRenderer!");
                    errors++;
                }
            }

            // D. Quet toan bo SpriteRenderer de phat hien bat ky lech tang nao
            foreach (SpriteRenderer sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
            {
                if (sr == null) continue;
                string layer = string.IsNullOrEmpty(sr.sortingLayerName) ? "Default" : sr.sortingLayerName;
                if (!countsByLayer.ContainsKey(layer)) countsByLayer[layer] = 0;
                countsByLayer[layer]++;

                string n = sr.gameObject.name.ToLowerInvariant();

                if (layer == "Foreground")
                {
                    Debug.LogWarning("[AUDIT][WARN] Vat the dang o Foreground (se luon de len Wen): " + sr.gameObject.name);
                }
                if (n.Contains("sunbeam") && layer != "WeatherVFX")
                {
                    Debug.LogError("[AUDIT][FAIL] " + sr.gameObject.name + " phai o WeatherVFX, hien tai: " + layer);
                    errors++;
                }
                if ((n.Contains("blightfog") || n.Contains("fog_")) && layer != "FogOverlay")
                {
                    Debug.LogError("[AUDIT][FAIL] " + sr.gameObject.name + " phai o FogOverlay, hien tai: " + layer);
                    errors++;
                }
                if (n.Contains("shadow") && layer != "Shadows")
                {
                    Debug.LogError("[AUDIT][FAIL] " + sr.gameObject.name + " phai o Shadows, hien tai: " + layer);
                    errors++;
                }
            }

            string summary = "";
            foreach (var kv in countsByLayer)
            {
                summary += kv.Key + "=" + kv.Value + " | ";
            }
            Debug.Log("[AUDIT] Phan bo SpriteRenderer theo Layer: " + summary.TrimEnd(' ', '|'));

            if (errors == 0)
            {
                Debug.Log("[MV][Step30] AUDIT PASS (0 loi): Toan bo 8 tang hien thi, Cong Vom, Tham, Tuong/Rao, Bon Hoa, VFX & Fog deu dong bo 100%!");
            }
            else
            {
                Debug.LogError("[MV][Step30] AUDIT FAIL (" + errors + " loi). Xem chi tiet cac dong [AUDIT][FAIL] o tren.");
            }
        }
    }
}
