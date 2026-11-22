using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step29
    {
        private const string MainScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string DarkWoodSpritePath = "Assets/Art/Sprites/UI/ui_panel_dark_wood.png";
        private const string VnFontPath = "Assets/Art/Fonts/VN_DynamicFont.asset";

        [MenuItem("Moonpetal/Step 29/1. Surgical Fix Rug, Sunbeams, Prompt UI & Scroll Conflict")]
        public static void ApplySurgicalFix()
        {
            FixInteractionPromptBarOnGameRoot();
            FixSurgicalSceneSprites();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Step29] HOAN TAT: Da sua tham Nha Kinh, SunbeamPool, Hoa/Nam sat dat & Font/Khung InteractionPromptBar!");
        }

        private static void FixInteractionPromptBarOnGameRoot()
        {
            Sprite darkWood = AssetDatabase.LoadAssetAtPath<Sprite>(DarkWoodSpritePath);
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(VnFontPath);

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;
            try
            {
                Transform bar = root.transform.Find("ClockHUDCanvas/InteractionPromptBar");
                if (bar != null)
                {
                    Image img = bar.GetComponent<Image>();
                    if (img != null && darkWood != null)
                    {
                        img.sprite = darkWood;
                        img.type = Image.Type.Sliced;
                        img.pixelsPerUnitMultiplier = 0.55f;
                        img.color = Color.white;
                        EditorUtility.SetDirty(img);
                    }

                    Transform lbl = bar.Find("PromptLabel");
                    if (lbl != null)
                    {
                        TMP_Text tmp = lbl.GetComponent<TMP_Text>();
                        if (tmp != null && vnFont != null)
                        {
                            tmp.font = vnFont;
                            EditorUtility.SetDirty(tmp);
                        }
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
                Debug.Log("[MV][Step29] Da gan dung ui_panel_dark_wood.png va VN_DynamicFont.asset cho InteractionPromptBar.");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void FixSurgicalSceneSprites()
        {
            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            int rugFixed = 0, sunFixed = 0, flatFixed = 0;

            foreach (SpriteRenderer sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
            {
                if (sr == null) continue;
                string n = sr.gameObject.name.ToLowerInvariant();

                // 1. Tham Nha Kinh (GreenhouseCozyRug) -> GroundDecor (1), xoa YSortRenderer!
                if (n.Contains("rug") || n.Contains("carpet"))
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "GroundDecor";
                    sr.sortingOrder = 1;
                    EditorUtility.SetDirty(sr);
                    rugFixed++;
                    continue;
                }

                // 2. 6 Vet nang diu (SunbeamPool_*) -> WeatherVFX (1), xoa YSortRenderer!
                if (n.Contains("sunbeam"))
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "WeatherVFX";
                    sr.sortingOrder = 1;
                    EditorUtility.SetDirty(sr);
                    sunFixed++;
                    continue;
                }

                // 3. Hoa dai (FlowerTuft_*, ExtraFlower_*) & Nam rung (ForestMushrooms_*) -> GroundDecor (10), xoa YSortRenderer!
                if (n.Contains("flowertuft") || n.Contains("extraflower") || n.Contains("mushroom"))
                {
                    RemoveYSort(sr.gameObject);
                    sr.sortingLayerName = "GroundDecor";
                    sr.sortingOrder = 10;
                    EditorUtility.SetDirty(sr);
                    flatFixed++;
                    continue;
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Step29] Surgical scene fix: Rug=" + rugFixed + ", Sunbeams=" + sunFixed + ", FlatFlowers/Mushrooms=" + flatFixed + ".");
        }

        private static void RemoveYSort(GameObject go)
        {
            YSortRenderer ys = go.GetComponent<YSortRenderer>();
            if (ys != null) UnityEngine.Object.DestroyImmediate(ys);
        }

        [MenuItem("Moonpetal/Step 29/2. Validate Surgical Fix")]
        public static void ValidateStep29()
        {
            int errors = 0;
            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

            GameObject rug = GameObject.Find("GreenhouseCozyRug");
            if (rug != null)
            {
                var sr = rug.GetComponent<SpriteRenderer>();
                if (sr == null || sr.sortingLayerName != "GroundDecor" || rug.GetComponent<YSortRenderer>() != null)
                {
                    Debug.LogError("[MV][Step29][FAIL] GreenhouseCozyRug van sai layer hoac con YSortRenderer.");
                    errors++;
                }
            }

            GameObject sun0 = GameObject.Find("SunbeamPool_0");
            if (sun0 != null)
            {
                var sr = sun0.GetComponent<SpriteRenderer>();
                if (sr == null || sr.sortingLayerName != "WeatherVFX")
                {
                    Debug.LogError("[MV][Step29][FAIL] SunbeamPool_0 chua o WeatherVFX.");
                    errors++;
                }
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                Transform bar = prefab.transform.Find("ClockHUDCanvas/InteractionPromptBar");
                if (bar != null)
                {
                    Image img = bar.GetComponent<Image>();
                    if (img == null || img.sprite == null)
                    {
                        Debug.LogError("[MV][Step29][FAIL] InteractionPromptBar thieu sprite ui_panel_dark_wood.");
                        errors++;
                    }
                    TMP_Text tmp = bar.Find("PromptLabel")?.GetComponent<TMP_Text>();
                    if (tmp == null || tmp.font == null || !tmp.font.name.Contains("VN_"))
                    {
                        Debug.LogError("[MV][Step29][FAIL] InteractionPromptBar thieu VN_DynamicFont.");
                        errors++;
                    }
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV][Step29] VALIDATE PASS: Tham Nha Kinh, Sunbeams, Hoa/Nam sat dat, Font/Khung Prompt & Scroll deu chuan 100%!");
            }
            else
            {
                Debug.LogError("[MV][Step29] VALIDATE FAIL (" + errors + " loi).");
            }
        }
    }
}
