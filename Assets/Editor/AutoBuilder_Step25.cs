using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Community;
using MoonpetalValley.Core;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step25
    {
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string MainScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string DarkWoodSpritePath = "Assets/Art/Sprites/UI/ui_panel_wood_dark.png";
        private const string HudFontPath = "Assets/Fonts/VN_HUDFont.asset";

        [MenuItem("Moonpetal/Step 25/1. Install Centralized Interaction Focus & Fix All Y-Sorting")]
        public static void InstallStep25()
        {
            EnsureSortingLayers();
            SetupGameRootInteractionUI();
            FixSceneSortingAndColliders();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Step25] HOAN TAT: Da gan InteractionManager (1 Prompt + Mui ten muc tieu + Doi bang R/Cuon) va chuan hoa Y-Sort toan bo map!");
        }

        private static void EnsureSortingLayers()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");

            string[] required = { "Ground", "GroundDecor", "WorldObjects", "Foreground", "UI" };
            var existing = new HashSet<string>();
            for (int i = 0; i < sortingLayers.arraySize; i++)
            {
                existing.Add(sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);
            }

            foreach (string layerName in required)
            {
                if (!existing.Contains(layerName))
                {
                    int idx = sortingLayers.arraySize;
                    sortingLayers.InsertArrayElementAtIndex(idx);
                    SerializedProperty el = sortingLayers.GetArrayElementAtIndex(idx);
                    el.FindPropertyRelative("name").stringValue = layerName;
                    el.FindPropertyRelative("uniqueID").intValue = Mathf.Abs(layerName.GetHashCode());
                    existing.Add(layerName);
                }
            }

            // Dam bao thu tu: Ground < GroundDecor < WorldObjects < Foreground < UI
            var orderList = new List<string>();
            for (int i = 0; i < sortingLayers.arraySize; i++)
            {
                orderList.Add(sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);
            }

            int idxWorld = orderList.IndexOf("WorldObjects");
            int idxDecor = orderList.IndexOf("GroundDecor");
            if (idxDecor > idxWorld && idxWorld >= 0)
            {
                sortingLayers.MoveArrayElement(idxDecor, idxWorld);
            }

            // Cap nhat lai thu tu Foreground nam sau WorldObjects va truoc UI
            orderList.Clear();
            for (int i = 0; i < sortingLayers.arraySize; i++)
                orderList.Add(sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);

            int idxFg = orderList.IndexOf("Foreground");
            int idxUi = orderList.IndexOf("UI");
            if (idxFg > idxUi && idxUi >= 0)
            {
                sortingLayers.MoveArrayElement(idxFg, idxUi);
            }

            tagManager.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log("[MV][Step25] Da dam bao thu tu Sorting Layers: Ground -> GroundDecor -> WorldObjects -> Foreground -> UI.");
        }

        private static void SetupGameRootInteractionUI()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath) == null)
            {
                Debug.LogError("[MV][Step25] Khong tim thay " + GameRootPrefabPath);
                return;
            }

            Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(DarkWoodSpritePath);
            TMP_FontAsset hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(HudFontPath);

            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            try
            {
                InteractionManager mgr = prefabRoot.GetComponent<InteractionManager>();
                if (mgr == null) mgr = prefabRoot.AddComponent<InteractionManager>();

                Canvas hudCanvas = null;
                foreach (Canvas c in prefabRoot.GetComponentsInChildren<Canvas>(true))
                {
                    if (c.name.Contains("ClockHUDCanvas") || c.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        hudCanvas = c;
                        break;
                    }
                }

                if (hudCanvas != null)
                {
                    Transform existing = hudCanvas.transform.Find("InteractionPromptBar");
                    GameObject barGo = existing != null ? existing.gameObject : new GameObject("InteractionPromptBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
                    if (existing == null) barGo.transform.SetParent(hudCanvas.transform, false);

                    RectTransform rt = barGo.GetComponent<RectTransform>();
                    rt.anchorMin = new Vector2(0.5f, 0f);
                    rt.anchorMax = new Vector2(0.5f, 0f);
                    rt.pivot = new Vector2(0.5f, 0f);
                    rt.anchoredPosition = new Vector2(0f, 126f);
                    rt.sizeDelta = new Vector2(580f, 44f);
                    rt.localScale = Vector3.one;

                    Image img = barGo.GetComponent<Image>();
                    if (panelSprite != null)
                    {
                        img.sprite = panelSprite;
                        img.type = Image.Type.Sliced;
                        img.pixelsPerUnitMultiplier = 0.50f;
                        img.color = new Color(1f, 1f, 1f, 0.96f);
                    }
                    else
                    {
                        img.color = new Color(0.16f, 0.11f, 0.08f, 0.92f);
                    }
                    img.raycastTarget = false;

                    CanvasGroup cg = barGo.GetComponent<CanvasGroup>();
                    if (cg == null) cg = barGo.AddComponent<CanvasGroup>();
                    cg.blocksRaycasts = false;
                    cg.interactable = false;

                    Transform txtTf = barGo.transform.Find("PromptLabel");
                    GameObject txtGo = txtTf != null ? txtTf.gameObject : new GameObject("PromptLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
                    if (txtTf == null) txtGo.transform.SetParent(barGo.transform, false);

                    RectTransform trt = txtGo.GetComponent<RectTransform>();
                    trt.anchorMin = Vector2.zero;
                    trt.anchorMax = Vector2.one;
                    trt.offsetMin = new Vector2(16f, 4f);
                    trt.offsetMax = new Vector2(-16f, -4f);

                    TextMeshProUGUI tmp = txtGo.GetComponent<TextMeshProUGUI>();
                    if (hudFont != null) tmp.font = hudFont;
                    tmp.fontSize = 20f;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.color = new Color32(252, 240, 206, 255);
                    tmp.text = "[E] Tương tác";
                    tmp.raycastTarget = false;

                    InteractionPromptUIView view = hudCanvas.GetComponent<InteractionPromptUIView>();
                    if (view == null) view = hudCanvas.gameObject.AddComponent<InteractionPromptUIView>();

                    SerializedObject vSo = new SerializedObject(view);
                    vSo.FindProperty("panelRoot").objectReferenceValue = barGo;
                    vSo.FindProperty("promptText").objectReferenceValue = tmp;
                    vSo.FindProperty("canvasGroup").objectReferenceValue = cg;
                    vSo.ApplyModifiedPropertiesWithoutUndo();

                    barGo.SetActive(false);

                    SerializedObject mSo = new SerializedObject(mgr);
                    mSo.FindProperty("promptUI").objectReferenceValue = view;
                    mSo.ApplyModifiedPropertiesWithoutUndo();

                    EditorUtility.SetDirty(view);
                    EditorUtility.SetDirty(mgr);
                }

                PrefabUtility.SaveAsPrefabAsset(prefabRoot, GameRootPrefabPath);
                Debug.Log("[MV][Step25] Da gan InteractionManager + InteractionPromptBar vao GameRoot.prefab.");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        private static void FixSceneSortingAndColliders()
        {
            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            int ySortCount = 0;
            int groundDecorCount = 0;

            // 1. Chuan hoa Tilemaps (Ground & Obstacles nam o Ground)
            foreach (TilemapRenderer tr in UnityEngine.Object.FindObjectsByType<TilemapRenderer>(FindObjectsInactive.Include))
            {
                if (tr.gameObject.name.Contains("Obstacle"))
                {
                    tr.sortingLayerName = "Ground";
                    tr.sortingOrder = -5000;
                }
                else
                {
                    tr.sortingLayerName = "Ground";
                }
                EditorUtility.SetDirty(tr);
            }

            // 2. Chuan hoa Bon Hoa Trung Tam (PlantingSpotController)
            PlantingSpotController spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            if (spot != null)
            {
                foreach (SpriteRenderer sr in spot.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    sr.sortingLayerName = "GroundDecor";
                    sr.sortingOrder = sr.gameObject.name.ToLowerInvariant().Contains("aura") ? 16 : 15;
                    EditorUtility.SetDirty(sr);
                }

                // Them collider tron gon cho loi da bon hoa de Wen di vong quanh tu nhien
                CircleCollider2D col = spot.GetComponent<CircleCollider2D>();
                if (col == null) col = spot.gameObject.AddComponent<CircleCollider2D>();
                col.isTrigger = false;
                col.radius = 0.68f;
                col.offset = new Vector2(0f, -0.05f);
                EditorUtility.SetDirty(col);
            }

            // 3. Quet toan bo SpriteRenderer trong Scene de phan tang chuan xac
            foreach (SpriteRenderer sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
            {
                if (sr == null) continue;
                string n = sr.gameObject.name.ToLowerInvariant();
                string parentName = sr.transform.parent != null ? sr.transform.parent.name.ToLowerInvariant() : "";

                // Bo qua UI World-Space, hieu ung tan cong/slash/pulse, va Bon Hoa da xu ly
                if (sr.sortingLayerName == "UI") continue;
                if (spot != null && sr.transform.IsChildOf(spot.transform)) continue;
                if (n.Contains("slash") || n.Contains("pulse") || n.Contains("hitflash")) continue;

                // Cong vom cao (GardenEntranceArch): nam o Foreground de Wen chui qua duoi vom
                if (n.Contains("gardenentrancearch") || n.Contains("arch_top"))
                {
                    sr.sortingLayerName = "Foreground";
                    sr.sortingOrder = 10;
                    EditorUtility.SetDirty(sr);
                    continue;
                }

                // Bong do duoi chan (GroundShadow): luon o GroundDecor phia tren nen dat & duoi chan nhan vat
                if (n.Contains("shadow"))
                {
                    sr.sortingLayerName = "GroundDecor";
                    sr.sortingOrder = 25;
                    EditorUtility.SetDirty(sr);
                    groundDecorCount++;
                    continue;
                }

                // Vat phang sat mat dat (khong bao gio che nhan vat)
                bool isFlatGround =
                    n.Contains("rug") || n.Contains("carpet") || n.Contains("pond") || n.Contains("water") ||
                    n.Contains("pier") || n.Contains("stepstone") || n.Contains("stepping") ||
                    n.Contains("soil") || n.Contains("farmtile") || n.Contains("tile_") ||
                    n.Contains("flower_") || n.Contains("wildflower") || n.Contains("grass") ||
                    n.Contains("petal") || n.Contains("mat_") || n.Contains("aura") || n.Contains("halo") ||
                    parentName.Contains("farmtile");

                // Ngoai le: cay trong (Crop) trong FarmTile van Y-Sort de cao len tren dat
                if (n.Contains("crop") && !n.Contains("soil"))
                {
                    isFlatGround = false;
                }

                if (isFlatGround)
                {
                    sr.sortingLayerName = "GroundDecor";
                    if (n.Contains("pond") || n.Contains("water")) sr.sortingOrder = 2;
                    else if (n.Contains("pier") || n.Contains("rug") || n.Contains("soil") || n.Contains("farmtile")) sr.sortingOrder = 6;
                    else if (n.Contains("aura") || n.Contains("halo")) sr.sortingOrder = 18;
                    else sr.sortingOrder = 10;

                    var oldSort = sr.GetComponent<YSortRenderer>();
                    if (oldSort != null) UnityEngine.Object.DestroyImmediate(oldSort);

                    EditorUtility.SetDirty(sr);
                    groundDecorCount++;
                    continue;
                }

                // Cac thuc the dung/cao -> WorldObjects + YSortRenderer theo chan (Foot Y)
                bool isPlayer = sr.GetComponentInParent<PlayerController2D>() != null;
                bool isNpc = sr.GetComponentInParent<NPCController>() != null;
                bool isLumi = sr.GetComponentInParent<LumiCompanionController>() != null;
                bool isDynamic = isPlayer || isNpc || isLumi || n.Contains("slime") || n.Contains("miucat") || n.Contains("loot");

                if (isPlayer)
                {
                    sr.sortingLayerName = "WorldObjects";
                    float fy = sr.sprite != null ? sr.bounds.min.y : (sr.transform.position.y - 0.45f);
                    sr.sortingOrder = Mathf.RoundToInt(-fy * 100f);
                    EditorUtility.SetDirty(sr);
                    continue;
                }

                YSortRenderer ysort = sr.GetComponent<YSortRenderer>();
                if (ysort == null) ysort = sr.gameObject.AddComponent<YSortRenderer>();
                int bias = isLumi ? 18 : 0;
                ysort.Configure(sr, isDynamic, true, 0f, bias);
                EditorUtility.SetDirty(ysort);
                EditorUtility.SetDirty(sr);
                ySortCount++;
            }

            // 4. An toan bo World-Space prompt cu tren cac doi tuong tuong tac
            foreach (MonoBehaviour mb in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {
                if (mb is IInteractable inter)
                {
                    inter.HideLegacyPrompt();
                    EditorUtility.SetDirty(mb);
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Step25] Scene updated: " + ySortCount + " YSortRenderers, " + groundDecorCount + " GroundDecor sprites.");
        }

        [MenuItem("Moonpetal/Step 25/2. Validate Interaction Focus & Y-Sorting")]
        public static void ValidateStep25()
        {
            int errors = 0;

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV][Step25][FAIL] Khong tim thay GameRoot.prefab.");
                errors++;
            }
            else
            {
                var mgr = prefab.GetComponent<InteractionManager>();
                if (mgr == null)
                {
                    Debug.LogError("[MV][Step25][FAIL] Thieu InteractionManager tren GameRoot.prefab.");
                    errors++;
                }
                else
                {
                    SerializedObject mSo = new SerializedObject(mgr);
                    if (mSo.FindProperty("promptUI").objectReferenceValue == null)
                    {
                        Debug.LogError("[MV][Step25][FAIL] InteractionManager.promptUI dang null.");
                        errors++;
                    }
                }
            }

            Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            PlantingSpotController spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            if (spot != null)
            {
                var sr = spot.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && sr.sortingLayerName != "GroundDecor")
                {
                    Debug.LogError("[MV][Step25][FAIL] PlantingSpot van chua o GroundDecor (" + sr.sortingLayerName + ").");
                    errors++;
                }
                if (spot.GetComponent<Collider2D>() == null)
                {
                    Debug.LogError("[MV][Step25][FAIL] PlantingSpot chua co Collider2D.");
                    errors++;
                }
            }

            int ySortTotal = UnityEngine.Object.FindObjectsByType<YSortRenderer>(FindObjectsInactive.Include).Length;
            if (ySortTotal < 5)
            {
                Debug.LogError("[MV][Step25][FAIL] So luong YSortRenderer qua it: " + ySortTotal);
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV][Step25] VALIDATE PASS: InteractionManager + 1 Prompt Bar + " + ySortTotal + " YSortRenderers + GroundDecor Bon Hoa deu hop le!");
            }
            else
            {
                Debug.LogError("[MV][Step25] VALIDATE FAIL (" + errors + " loi).");
            }
        }
    }
}
