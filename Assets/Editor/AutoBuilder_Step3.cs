using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step3
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string TimeConfigPath = "Assets/Data/Config/TimeConfig.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 3/1. Build Time & Day-Night Lighting")]
        public static void BuildStep3()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 3 ===");

            Sprite lanternSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_lantern.png", 16, 24, PlaceholderGeneratorUtility.DrawLanternPostSprite);
            Sprite warmHaloSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_warm_halo.png", 32, 32, PlaceholderGeneratorUtility.DrawWarmHalo);

            UpgradeGameRootWithTimeAndLighting();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildVillageLanterns(lanternSpr, warmHaloSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 3! Chạy 'Moonpetal/Step 3/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 3/Validate")]
        public static void ValidateStep3()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 3 ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab");
                errors++;
            }
            else
            {
                var root = prefab.GetComponent<GameRoot>();
                var rootSO = new SerializedObject(root);
                if (rootSO.FindProperty("timeManager").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu tham chiếu timeManager.");
                    errors++;
                }

                var tm = prefab.GetComponent<TimeOfDayManager>();
                if (tm == null || new SerializedObject(tm).FindProperty("timeConfig").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] TimeOfDayManager thiếu TimeConfigSO.");
                    errors++;
                }

                var lighting = prefab.GetComponentInChildren<DayNightLightingController>(true);
                if (lighting == null || new SerializedObject(lighting).FindProperty("ambientTintImage").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] DayNightLightingController thiếu ambientTintImage.");
                    errors++;
                }

                var clock = prefab.GetComponentInChildren<ClockHUDView>(true);
                if (clock == null || new SerializedObject(clock).FindProperty("clockText").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] ClockHUDView thiếu clockText.");
                    errors++;
                }
            }

            var lanterns = UnityEngine.Object.FindObjectsByType<VillageLantern>(FindObjectsInactive.Include);
            if (lanterns.Length < 4)
            {
                Debug.LogError("[MV] Thiếu đèn lồng làng (cần 4, hiện có " + lanterns.Length + ").");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 3: PASS (0 lỗi). Chu kỳ Ngày/Đêm, Đồng hồ HUD và Đèn lồng làng sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 3: FAIL (" + errors + " lỗi).");
            }
        }

        private static void UpgradeGameRootWithTimeAndLighting()
        {
            TimeConfigSO timeConfig = AssetDatabase.LoadAssetAtPath<TimeConfigSO>(TimeConfigPath);
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();

            // 1. TimeOfDayManager
            var tm = root.GetComponent<TimeOfDayManager>();
            if (tm == null) tm = root.AddComponent<TimeOfDayManager>();
            var tmSO = new SerializedObject(tm);
            tmSO.FindProperty("timeConfig").objectReferenceValue = timeConfig;
            tmSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("timeManager").objectReferenceValue = tm;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // 2. DayNightOverlayCanvas (SortingOrder = 5, raycastTarget = false)
            Transform oldLighting = root.transform.Find("DayNightOverlayCanvas");
            if (oldLighting != null) UnityEngine.Object.DestroyImmediate(oldLighting.gameObject);

            GameObject tintCanvasGo = new GameObject("DayNightOverlayCanvas");
            tintCanvasGo.transform.SetParent(root.transform, false);
            var tintCanvas = tintCanvasGo.AddComponent<Canvas>();
            tintCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            tintCanvas.sortingOrder = 5;

            GameObject tintImgGo = new GameObject("AmbientTintImage");
            tintImgGo.transform.SetParent(tintCanvasGo.transform, false);
            var tintRect = tintImgGo.AddComponent<RectTransform>();
            tintRect.anchorMin = Vector2.zero;
            tintRect.anchorMax = Vector2.one;
            tintRect.offsetMin = Vector2.zero;
            tintRect.offsetMax = Vector2.zero;

            var tintImg = tintImgGo.AddComponent<Image>();
            tintImg.color = new Color(1f, 0.92f, 0.72f, 0.08f);
            tintImg.raycastTarget = false;

            var lightCtrl = tintCanvasGo.AddComponent<DayNightLightingController>();
            var lightSO = new SerializedObject(lightCtrl);
            lightSO.FindProperty("timeConfig").objectReferenceValue = timeConfig;
            lightSO.FindProperty("ambientTintImage").objectReferenceValue = tintImg;
            lightSO.ApplyModifiedPropertiesWithoutUndo();

            // 3. Clock HUD (Top-Right inside SystemOverlayCanvas or dedicated HUDCanvas SortingOrder = 20)
            Transform oldClock = root.transform.Find("ClockHUDCanvas");
            if (oldClock != null) UnityEngine.Object.DestroyImmediate(oldClock.gameObject);

            GameObject hudCanvasGo = new GameObject("ClockHUDCanvas");
            hudCanvasGo.transform.SetParent(root.transform, false);
            var hudCanvas = hudCanvasGo.AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.sortingOrder = 20;
            var scaler = hudCanvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject badgePanel = new GameObject("ClockBadgePanel");
            badgePanel.transform.SetParent(hudCanvasGo.transform, false);
            var bpRect = badgePanel.AddComponent<RectTransform>();
            bpRect.anchorMin = new Vector2(1f, 1f);
            bpRect.anchorMax = new Vector2(1f, 1f);
            bpRect.pivot = new Vector2(1f, 1f);
            bpRect.anchoredPosition = new Vector2(-24f, -24f);
            bpRect.sizeDelta = new Vector2(360f, 58f);
            var bpImg = badgePanel.AddComponent<Image>();
            bpImg.color = new Color(0.09f, 0.13f, 0.20f, 0.88f);

            GameObject dotGo = new GameObject("PhaseDot");
            dotGo.transform.SetParent(badgePanel.transform, false);
            var dotRect = dotGo.AddComponent<RectTransform>();
            dotRect.anchorMin = new Vector2(0f, 0.5f);
            dotRect.anchorMax = new Vector2(0f, 0.5f);
            dotRect.anchoredPosition = new Vector2(24f, 0f);
            dotRect.sizeDelta = new Vector2(18f, 18f);
            var dotImg = dotGo.AddComponent<Image>();
            dotImg.color = new Color(0.95f, 0.80f, 0.35f, 1f);

            GameObject txtGo = new GameObject("ClockText");
            txtGo.transform.SetParent(badgePanel.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = new Vector2(46f, 4f);
            txtRect.offsetMax = new Vector2(-12f, -4f);
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "Ngày 1 — 06:00 (Sáng)";
            tmp.fontSize = 21;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = Color.white;
            if (vnFont != null) tmp.font = vnFont;

            var clockView = badgePanel.AddComponent<ClockHUDView>();
            var clockSO = new SerializedObject(clockView);
            clockSO.FindProperty("clockText").objectReferenceValue = tmp;
            clockSO.FindProperty("phaseBadgeImage").objectReferenceValue = dotImg;
            clockSO.ApplyModifiedPropertiesWithoutUndo();

            // 4. Add 3 Time Debug Buttons to DebugPanelRoot
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 650f);

                Button btnPlus2H = EnsureDebugButton(debugPanel, "BtnTimePlus2H", "Thời gian: +2 Giờ", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));
                Button btnDusk = EnsureDebugButton(debugPanel, "BtnTimeSetDusk", "Đặt 19:50 (Chạng vạng -> Đêm)", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));
                Button btnNextDay = EnsureDebugButton(debugPanel, "BtnTimeNextDay", "Qua Ngày Mới (06:00)", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));

                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnTimePlus2H").objectReferenceValue = btnPlus2H;
                dbgSO.FindProperty("btnTimeSetDusk").objectReferenceValue = btnDusk;
                dbgSO.FindProperty("btnTimeNextDay").objectReferenceValue = btnNextDay;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã nâng cấp GameRoot.prefab với TimeOfDayManager, DayNightLighting & ClockHUD.");
        }

        private static void BuildVillageLanterns(Sprite lanternSpr, Sprite warmHaloSpr)
        {
            GameObject group = GameObject.Find("VillageLanterns");
            if (group != null) UnityEngine.Object.DestroyImmediate(group);

            group = new GameObject("VillageLanterns");
            Vector2[] positions =
            {
                new Vector2(-13.5f, 4.5f),   // Trước cửa Nhà Kính
                new Vector2(-2.5f, 4.5f),    // Quảng Trường Làng Chính
                new Vector2(2.5f, -3.0f),    // Cổng Vườn Ông Bách
                new Vector2(12.5f, 7.5f)     // Đường vào Rìa Rừng
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject lanternGo = new GameObject("Lantern_" + (i + 1));
                lanternGo.transform.SetParent(group.transform, false);
                lanternGo.transform.position = new Vector3(positions[i].x, positions[i].y, 0f);

                var sr = lanternGo.AddComponent<SpriteRenderer>();
                sr.sprite = lanternSpr;
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = Mathf.RoundToInt(-positions[i].y * 100f);

                GameObject glowGo = new GameObject("WarmGlow");
                glowGo.transform.SetParent(lanternGo.transform, false);
                glowGo.transform.localPosition = new Vector3(0.15f, 0.35f, 0f);
                glowGo.transform.localScale = new Vector3(2.4f, 2.4f, 1f);
                var glowSr = glowGo.AddComponent<SpriteRenderer>();
                glowSr.sprite = warmHaloSpr;
                glowSr.sortingLayerName = "WeatherVFX";
                glowSr.sortingOrder = 15;
                glowSr.color = new Color(1f, 0.85f, 0.45f, 0f);

                var comp = lanternGo.AddComponent<VillageLantern>();
                var so = new SerializedObject(comp);
                so.FindProperty("glowRenderer").objectReferenceValue = glowSr;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static Button EnsureDebugButton(Transform parent, string name, string label, TMP_FontAsset font, Color bgColor)
        {
            Transform existing = parent.Find(name);
            if (existing != null) return existing.GetComponent<Button>();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 36f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            GameObject txtGo = new GameObject("Label");
            txtGo.transform.SetParent(go.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            if (font != null) tmp.font = font;

            return btn;
        }
    }
}
