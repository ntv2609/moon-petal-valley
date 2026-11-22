using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step9
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string BundleAssetPath = "Assets/Data/Bundles/Bundle_ForgottenGarden.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 9/1. Build Bundle, PlantingSpot & Fog")]
        public static void BuildStep9()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 9 ===");

            Sprite emptySpotSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_planting_spot_empty.png", 32, 32, DrawEmptySpot);
            Sprite bloomedSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_moonpetal_bloomed.png", 32, 32, DrawBloomedMoonpetal);
            Sprite fogSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_blight_fog.png", 48, 32, DrawBlightFog);
            Sprite haloSpr = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Characters/spr_lumi_halo.png");

            BundleData bundle = EnsureBundleAsset();
            UpgradeGameRootWithBundleUI(bundle);

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildCentralPlantingSpot(emptySpotSpr, bloomedSpr, haloSpr);
            BuildBlightFogZones(fogSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 9! Chạy 'Moonpetal/Step 9/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 9/Validate")]
        public static void ValidateStep9()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 9 ===");

            var bundle = AssetDatabase.LoadAssetAtPath<BundleData>(BundleAssetPath);
            if (bundle == null || bundle.requiredItems.Count < 5 || bundle.requiredHelperFlags.Count < 4)
            {
                Debug.LogError("[MV] Bundle_ForgottenGarden.asset thiếu vật phẩm hoặc thiếu 4 helper flags.");
                errors++;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                if (new SerializedObject(prefab.GetComponent<GameRoot>()).FindProperty("bundleManager").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu bundleManager.");
                    errors++;
                }
                if (prefab.GetComponentInChildren<BundleUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu BundleUIView trong GameRoot.prefab.");
                    errors++;
                }
            }

            if (UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>() == null)
            {
                Debug.LogError("[MV] Thiếu PlantingSpotController trong scene.");
                errors++;
            }

            var fogs = UnityEngine.Object.FindObjectsByType<BlightFogZone>(FindObjectsInactive.Include);
            if (fogs.Length < 4)
            {
                Debug.LogError("[MV] Thiếu BlightFogZone (cần >= 4, hiện có " + fogs.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 9: PASS (0 lỗi). Gói Ký Ức, Bồn Hoa Trung Tâm & Sương Héo Úa sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 9: FAIL (" + errors + " lỗi).");
            }
        }

        private static BundleData EnsureBundleAsset()
        {
            var b = AssetDatabase.LoadAssetAtPath<BundleData>(BundleAssetPath);
            if (b == null)
            {
                b = ScriptableObject.CreateInstance<BundleData>();
                AssetDatabase.CreateAsset(b, BundleAssetPath);
            }

            b.bundleID = "bundle_forgotten_garden";
            b.bundleTitleKey = "Gói Ký Ức Vườn Xưa";
            b.bundleDescriptionKey = "Chung tay cùng 4 dân làng hồi sinh Bồn Hoa Trung Tâm Vườn Ông Bách.";
            b.requiredItems = new List<ItemRequirement>
            {
                new ItemRequirement { item = LoadItem("item_seed_bag_1"), amount = 1 },
                new ItemRequirement { item = LoadItem("item_seed_bag_2"), amount = 1 },
                new ItemRequirement { item = LoadItem("item_seed_bag_3"), amount = 1 },
                new ItemRequirement { item = LoadItem("item_dew_sap"), amount = 5 },
                new ItemRequirement { item = LoadItem("item_starlight_dust"), amount = 3 }
            };
            b.requiredHelperNPCIDs = new List<string> { "npc_bach", "npc_hana", "npc_toby", "npc_otis" };
            b.requiredHelperFlags = new List<string> { "flag_bach_letters_read", "flag_hana_bread_done", "flag_toby_cat_saved", "flag_otis_mailbox_fixed" };
            b.rewardItem = LoadItem("item_petal_of_hope");
            b.rewardStoryFlag = "flag_nv1_bundle_completed";

            EditorUtility.SetDirty(b);
            return b;
        }

        private static ItemData LoadItem(string id)
        {
            return AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_" + id + ".asset");
        }

        private static void UpgradeGameRootWithBundleUI(BundleData bundle)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var bundleMgr = root.GetComponent<BundleManager>() ?? root.AddComponent<BundleManager>();
            var bmSO = new SerializedObject(bundleMgr);
            bmSO.FindProperty("activeBundle").objectReferenceValue = bundle;
            bmSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("bundleManager").objectReferenceValue = bundleMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                Transform oldHost = hudCanvas.Find("BundleUIHost");
                if (oldHost != null) UnityEngine.Object.DestroyImmediate(oldHost.gameObject);

                GameObject host = new GameObject("BundleUIHost");
                host.transform.SetParent(hudCanvas, false);
                var hRect = host.AddComponent<RectTransform>();
                hRect.anchorMin = Vector2.zero; hRect.anchorMax = Vector2.one;
                hRect.offsetMin = Vector2.zero; hRect.offsetMax = Vector2.zero;

                // 1. Bundle Modal
                GameObject modal = CreatePanel("BundlePanelRoot", host.transform, new Vector2(820f, 470f), new Color(0.08f, 0.12f, 0.20f, 0.96f));
                var title = CreateTMP("Title", modal.transform, "GÓI KÝ ỨC VƯỜN XƯA — Bồn Hoa Trung Tâm", vnFont, 24, TextAlignmentOptions.Top, new Color(1f, 0.90f, 0.56f, 1f));
                title.GetComponent<RectTransform>().offsetMax = new Vector2(-16f, -14f);

                var itemsTxt = CreateTMP("ItemsStatus", modal.transform, "Vật phẩm...", vnFont, 18, TextAlignmentOptions.TopLeft, Color.white);
                itemsTxt.GetComponent<RectTransform>().offsetMin = new Vector2(28f, 100f);
                itemsTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-410f, -64f);

                var helpersTxt = CreateTMP("HelpersStatus", modal.transform, "Helpers...", vnFont, 18, TextAlignmentOptions.TopLeft, new Color(0.84f, 0.96f, 0.90f, 1f));
                helpersTxt.GetComponent<RectTransform>().offsetMin = new Vector2(420f, 100f);
                helpersTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -64f);

                Button btnContrib = CreateBtn("BtnContribute", modal.transform, "Nộp Vật Phẩm Đang Có", new Vector2(-240f, 42f), new Vector2(230f, 46f), new Color(0.24f, 0.44f, 0.68f, 1f), vnFont);
                Button btnBloom = CreateBtn("BtnBloom", modal.transform, "Cùng Gieo Hạt! (Nở Hoa)", new Vector2(25f, 42f), new Vector2(250f, 46f), new Color(0.18f, 0.62f, 0.42f, 1f), vnFont);
                Button btnClose = CreateBtn("BtnClose", modal.transform, "Đóng [Esc]", new Vector2(285f, 42f), new Vector2(160f, 46f), new Color(0.40f, 0.24f, 0.26f, 1f), vnFont);

                // 2. Victory Celebration Modal
                GameObject victory = CreatePanel("VictoryPanelRoot", host.transform, new Vector2(760f, 360f), new Color(0.10f, 0.16f, 0.24f, 0.97f));
                var vTitle = CreateTMP("VTitle", victory.transform, "CHÚC MỪNG HOÀN THÀNH NHIỆM VỤ 1!", vnFont, 28, TextAlignmentOptions.Top, new Color(1f, 0.90f, 0.48f, 1f));
                vTitle.GetComponent<RectTransform>().offsetMax = new Vector2(-20f, -24f);
                var vBody = CreateTMP("VBody", victory.transform,
                    "Cùng với Ông Bách, Bà Hana, Cậu Bé Toby và Bác Otis, bạn đã làm Hoa Moonpetal nở rực giữa Khu Vườn Bị Lãng Quên!\n\n- Đã nhận: Cánh Hoa Hy Vọng\n- Sương Héo Úa đã tan khỏi Làng Chính\n- Tinh linh Lumi mở khóa kỹ năng Xung Ánh Trăng (phím Q)!",
                    vnFont, 20, TextAlignmentOptions.Center, Color.white);
                vBody.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 88f);
                vBody.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -72f);
                Button btnVClose = CreateBtn("BtnVictoryClose", victory.transform, "Tiếp tục khám phá Thung Lũng", new Vector2(0f, 40f), new Vector2(320f, 48f), new Color(0.20f, 0.58f, 0.42f, 1f), vnFont);

                var view = host.AddComponent<BundleUIView>();
                var vSO = new SerializedObject(view);
                vSO.FindProperty("panelRoot").objectReferenceValue = modal;
                vSO.FindProperty("itemsStatusText").objectReferenceValue = itemsTxt;
                vSO.FindProperty("helpersStatusText").objectReferenceValue = helpersTxt;
                vSO.FindProperty("btnContribute").objectReferenceValue = btnContrib;
                vSO.FindProperty("btnBloom").objectReferenceValue = btnBloom;
                vSO.FindProperty("btnClose").objectReferenceValue = btnClose;
                vSO.FindProperty("victoryPanelRoot").objectReferenceValue = victory;
                vSO.FindProperty("btnVictoryClose").objectReferenceValue = btnVClose;
                vSO.ApplyModifiedPropertiesWithoutUndo();
            }

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 1005f);
                Transform ex = debugPanel.Find("BtnPrepClimax");
                Button btnPrep = ex != null ? ex.GetComponent<Button>() : CreateDebugBtn(debugPanel, "BtnPrepClimax", "Chuẩn bị Cao Trào NV1 (4 Helpers + Đủ Đồ)", vnFont);
                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnPrepClimax").objectReferenceValue = btnPrep;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildCentralPlantingSpot(Sprite emptySpr, Sprite bloomedSpr, Sprite haloSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject old = GameObject.Find("CentralPlantingSpot");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject go = new GameObject("CentralPlantingSpot");
            go.tag = "Interactable";
            go.layer = 11;
            go.transform.position = new Vector3(0.0f, -10.0f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = emptySpr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-(-10.0f) * 100f) - 10;

            GameObject auraGo = new GameObject("BloomAura");
            auraGo.transform.SetParent(go.transform, false);
            var auraSr = auraGo.AddComponent<SpriteRenderer>();
            auraSr.sprite = haloSpr;
            auraSr.color = new Color(1f, 0.84f, 0.92f, 0.72f);
            auraSr.sortingLayerName = "WeatherVFX";
            auraSr.sortingOrder = 12;
            auraSr.enabled = false;

            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(go.transform, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace; canvas.sortingLayerName = "UI"; canvas.sortingOrder = 55;
            promptGo.GetComponent<RectTransform>().sizeDelta = new Vector2(340f, 46f);
            promptGo.AddComponent<Image>().color = new Color(0.08f, 0.12f, 0.20f, 0.92f);
            var tmp = CreateTMP("Text", promptGo.transform, "[E] Bồn Hoa Trung Tâm (Gói Ký Ức)", vnFont, 20, TextAlignmentOptions.Center, new Color(1f, 0.92f, 0.62f, 1f));
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            var ctrl = go.AddComponent<PlantingSpotController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("flowerRenderer").objectReferenceValue = sr;
            so.FindProperty("emptySprite").objectReferenceValue = emptySpr;
            so.FindProperty("bloomedSprite").objectReferenceValue = bloomedSpr;
            so.FindProperty("bloomAuraRenderer").objectReferenceValue = auraSr;
            so.FindProperty("promptObject").objectReferenceValue = promptGo;
            so.FindProperty("interactRadius").floatValue = 2.5f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildBlightFogZones(Sprite fogSpr)
        {
            GameObject old = GameObject.Find("BlightFogGroup");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("BlightFogGroup");

            Vector2[] positions =
            {
                new Vector2(-4.0f, -8.0f),
                new Vector2(4.0f, -8.0f),
                new Vector2(0.0f, -12.0f),
                new Vector2(-3.5f, -12.0f),
                new Vector2(3.5f, -12.0f),
                new Vector2(14.0f, 6.0f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject fGo = new GameObject("BlightFog_" + (i + 1));
                fGo.tag = "BlightFog";
                fGo.transform.SetParent(group.transform, false);
                fGo.transform.position = new Vector3(positions[i].x, positions[i].y, 0f);
                fGo.transform.localScale = new Vector3(2.2f, 2.0f, 1f);

                var sr = fGo.AddComponent<SpriteRenderer>();
                sr.sprite = fogSpr;
                sr.sortingLayerName = "FogOverlay";
                sr.sortingOrder = 10;
                sr.color = new Color(0.55f, 0.42f, 0.68f, 0.48f);

                var comp = fGo.AddComponent<BlightFogZone>();
                var so = new SerializedObject(comp);
                so.FindProperty("clearOnFlag").stringValue = "flag_nv1_completed";
                so.FindProperty("fogRenderer").objectReferenceValue = sr;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 size, Color bg)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0.5f); r.anchorMax = new Vector2(0.5f, 0.5f);
            r.sizeDelta = size;
            go.AddComponent<Image>().color = bg;
            return go;
        }

        private static TMP_Text CreateTMP(string name, Transform parent, string text, TMP_FontAsset font, float size, TextAlignmentOptions align, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(12f, 8f); r.offsetMax = new Vector2(-12f, -8f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = align; tmp.color = color;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateBtn(string name, Transform parent, string label, Vector2 pos, Vector2 size, Color bg, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0f); r.anchorMax = new Vector2(0.5f, 0f);
            r.anchoredPosition = pos; r.sizeDelta = size;
            var img = go.AddComponent<Image>(); img.color = bg;
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 17, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static Button CreateDebugBtn(Transform parent, string name, string label, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
            var img = go.AddComponent<Image>(); img.color = new Color(0.62f, 0.24f, 0.44f, 1f);
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 15, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static void DrawEmptySpot(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 4, 4, 24, 24, new Color(0.48f, 0.48f, 0.52f));
            PlaceholderGeneratorUtility.FillRect(t, 7, 7, 18, 18, new Color(0.32f, 0.24f, 0.18f));
            PlaceholderGeneratorUtility.FillRect(t, 13, 13, 6, 6, new Color(0.24f, 0.18f, 0.14f));
        }

        private static void DrawBloomedMoonpetal(Texture2D t)
        {
            DrawEmptySpot(t);
            Color leaf = new Color(0.26f, 0.78f, 0.42f);
            Color petal = new Color(0.98f, 0.72f, 0.88f);
            Color core = new Color(0.70f, 0.98f, 1.0f);
            PlaceholderGeneratorUtility.FillRect(t, 10, 8, 12, 8, leaf);
            PlaceholderGeneratorUtility.FillRect(t, 8, 13, 16, 12, petal);
            PlaceholderGeneratorUtility.FillRect(t, 12, 10, 8, 18, petal);
            PlaceholderGeneratorUtility.FillRect(t, 13, 16, 6, 6, core);
        }

        private static void DrawBlightFog(Texture2D t)
        {
            Vector2 c = new Vector2(24f, 16f);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 48; x++)
                {
                    float dx = (x - c.x) / 22f;
                    float dy = (y - c.y) / 14f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    if (d < 1f)
                    {
                        float a = (1f - d) * (1f - d);
                        t.SetPixel(x, y, new Color(0.52f, 0.38f, 0.64f, a));
                    }
                }
            }
        }
    }
}
