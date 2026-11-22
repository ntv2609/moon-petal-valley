using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step4
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 4/1. Build Stats HUD, Bed & Knockout")]
        public static void BuildStep4()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 4 ===");

            Sprite bedSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_bed.png", 16, 24, PlaceholderGeneratorUtility.DrawBedSprite);

            UpgradeGameRootWithStatsAndSleep();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildGreenhouseBed(bedSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 4! Chạy 'Moonpetal/Step 4/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 4/Validate")]
        public static void ValidateStep4()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 4 ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab");
                errors++;
            }
            else
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                if (rootSO.FindProperty("statsController").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu statsController.");
                    errors++;
                }
                if (rootSO.FindProperty("sleepController").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu sleepController.");
                    errors++;
                }

                var hud = prefab.GetComponentInChildren<StatsHUDView>(true);
                if (hud == null)
                {
                    Debug.LogError("[MV] Thiếu StatsHUDView trong GameRoot.prefab.");
                    errors++;
                }
                else
                {
                    var hudSO = new SerializedObject(hud);
                    string[] req = { "lightHPFill", "lightHPText", "staminaFill", "staminaText", "exhaustedBadge" };
                    foreach (string f in req)
                    {
                        if (hudSO.FindProperty(f).objectReferenceValue == null)
                        {
                            Debug.LogError("[MV] StatsHUDView thiếu trường: " + f);
                            errors++;
                        }
                    }
                }
            }

            var bed = UnityEngine.Object.FindAnyObjectByType<BedInteractable>();
            if (bed == null)
            {
                Debug.LogError("[MV] Thiếu BedInteractable (Giường Ngủ) trong Nhà Kính.");
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
                Debug.Log("[MV] VALIDATE STEP 4: PASS (0 lỗi). Thanh Stamina/LightHP, Giường Ngủ & Ngất Xỉu sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 4: FAIL (" + errors + " lỗi).");
            }
        }

        private static void UpgradeGameRootWithStatsAndSleep()
        {
            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();

            // 1. PlayerStatsController
            var statsCtrl = root.GetComponent<PlayerStatsController>();
            if (statsCtrl == null) statsCtrl = root.AddComponent<PlayerStatsController>();
            var statsSO = new SerializedObject(statsCtrl);
            statsSO.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            statsSO.ApplyModifiedPropertiesWithoutUndo();

            // 2. DayTransitionCanvas + SleepAndKnockoutController (SortingOrder = 90)
            Transform oldTrans = root.transform.Find("DayTransitionCanvas");
            if (oldTrans != null) UnityEngine.Object.DestroyImmediate(oldTrans.gameObject);

            GameObject transCanvasGo = new GameObject("DayTransitionCanvas");
            transCanvasGo.transform.SetParent(root.transform, false);
            var tc = transCanvasGo.AddComponent<Canvas>();
            tc.renderMode = RenderMode.ScreenSpaceOverlay;
            tc.sortingOrder = 90;
            var tcScaler = transCanvasGo.AddComponent<CanvasScaler>();
            tcScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            tcScaler.referenceResolution = new Vector2(1920f, 1080f);
            tcScaler.matchWidthOrHeight = 0.5f;

            GameObject curtain = new GameObject("FadeCurtain");
            curtain.transform.SetParent(transCanvasGo.transform, false);
            var cRect = curtain.AddComponent<RectTransform>();
            cRect.anchorMin = Vector2.zero;
            cRect.anchorMax = Vector2.one;
            cRect.offsetMin = Vector2.zero;
            cRect.offsetMax = Vector2.zero;
            var cImg = curtain.AddComponent<Image>();
            cImg.color = new Color(0.04f, 0.06f, 0.11f, 1f);
            var cGroup = curtain.AddComponent<CanvasGroup>();
            cGroup.alpha = 0f;
            cGroup.blocksRaycasts = false;

            GameObject msgGo = new GameObject("TransitionText");
            msgGo.transform.SetParent(curtain.transform, false);
            var msgRect = msgGo.AddComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0.15f, 0.35f);
            msgRect.anchorMax = new Vector2(0.85f, 0.65f);
            msgRect.offsetMin = Vector2.zero;
            msgRect.offsetMax = Vector2.zero;
            var msgTmp = msgGo.AddComponent<TextMeshProUGUI>();
            msgTmp.text = "Đang sang ngày mới...";
            msgTmp.fontSize = 32;
            msgTmp.alignment = TextAlignmentOptions.Center;
            msgTmp.color = Color.white;
            msgTmp.textWrappingMode = TextWrappingModes.Normal;
            if (vnFont != null) msgTmp.font = vnFont;

            var sleepCtrl = root.GetComponent<SleepAndKnockoutController>();
            if (sleepCtrl == null) sleepCtrl = root.AddComponent<SleepAndKnockoutController>();
            var sleepSO = new SerializedObject(sleepCtrl);
            sleepSO.FindProperty("fadeCanvasGroup").objectReferenceValue = cGroup;
            sleepSO.FindProperty("transitionText").objectReferenceValue = msgTmp;
            sleepSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("statsController").objectReferenceValue = statsCtrl;
            rootSO.FindProperty("sleepController").objectReferenceValue = sleepCtrl;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // 3. StatsPanel ở góc trên trái của ClockHUDCanvas
            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                Transform oldStats = hudCanvas.Find("StatsPanel");
                if (oldStats != null) UnityEngine.Object.DestroyImmediate(oldStats.gameObject);

                GameObject panel = new GameObject("StatsPanel");
                panel.transform.SetParent(hudCanvas, false);
                var pRect = panel.AddComponent<RectTransform>();
                pRect.anchorMin = new Vector2(0f, 1f);
                pRect.anchorMax = new Vector2(0f, 1f);
                pRect.pivot = new Vector2(0f, 1f);
                pRect.anchoredPosition = new Vector2(24f, -24f);
                pRect.sizeDelta = new Vector2(360f, 118f);
                var pImg = panel.AddComponent<Image>();
                pImg.color = new Color(0.09f, 0.13f, 0.20f, 0.88f);

                // Bar 1: Máu Ánh Sáng
                var (hpFill, hpTxt) = CreateStatBar(panel.transform, "LightHPBar", new Vector2(14f, -14f), new Color(0.32f, 0.86f, 0.82f, 1f), "Máu Ánh Sáng: 100/100", vnFont);
                // Bar 2: Thể lực
                var (stFill, stTxt) = CreateStatBar(panel.transform, "StaminaBar", new Vector2(14f, -58f), new Color(0.95f, 0.78f, 0.24f, 1f), "Thể lực: 100/100", vnFont);

                // Nhãn Kiệt sức (-50%)
                GameObject badgeGo = new GameObject("ExhaustedBadge");
                badgeGo.transform.SetParent(panel.transform, false);
                var bRect = badgeGo.AddComponent<RectTransform>();
                bRect.anchorMin = new Vector2(0f, 1f);
                bRect.anchorMax = new Vector2(0f, 1f);
                bRect.pivot = new Vector2(0f, 1f);
                bRect.anchoredPosition = new Vector2(14f, -94f);
                bRect.sizeDelta = new Vector2(332f, 22f);
                var bTmp = badgeGo.AddComponent<TextMeshProUGUI>();
                bTmp.text = "⚠ KIỆT SỨC: Trần Thể lực giảm còn 50% (Ăn Bánh Mì để xóa)";
                bTmp.fontSize = 14;
                bTmp.color = new Color(1f, 0.56f, 0.34f, 1f);
                if (vnFont != null) bTmp.font = vnFont;
                badgeGo.SetActive(false);

                var hudView = panel.AddComponent<StatsHUDView>();
                var hudSO = new SerializedObject(hudView);
                hudSO.FindProperty("lightHPFill").objectReferenceValue = hpFill;
                hudSO.FindProperty("lightHPText").objectReferenceValue = hpTxt;
                hudSO.FindProperty("staminaFill").objectReferenceValue = stFill;
                hudSO.FindProperty("staminaText").objectReferenceValue = stTxt;
                hudSO.FindProperty("exhaustedBadge").objectReferenceValue = badgeGo;
                hudSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // 4. Thêm 3 nút Debug cho Stats & Knockout vào DebugPanelRoot
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 790f);

                Button btnRestore = EnsureDebugButton(debugPanel, "BtnRestoreStats", "Hồi đầy Stamina & HP (Xóa kiệt sức)", vnFont, new Color(0.24f, 0.46f, 0.24f, 1f));
                Button btnExhaust = EnsureDebugButton(debugPanel, "BtnForceExhaust", "Ép Kiệt Sức (Stamina = 0)", vnFont, new Color(0.58f, 0.22f, 0.22f, 1f));
                Button btnDmg = EnsureDebugButton(debugPanel, "BtnDamageLightHP", "Trừ 25 Máu Ánh Sáng", vnFont, new Color(0.58f, 0.22f, 0.22f, 1f));

                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnRestoreStats").objectReferenceValue = btnRestore;
                dbgSO.FindProperty("btnForceExhaust").objectReferenceValue = btnExhaust;
                dbgSO.FindProperty("btnDamageLightHP").objectReferenceValue = btnDmg;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã nâng cấp GameRoot.prefab với PlayerStatsController, StatsHUD & SleepAndKnockoutController.");
        }

        private static (Image fill, TMP_Text label) CreateStatBar(Transform parent, string name, Vector2 pos, Color fillColor, string initText, TMP_FontAsset font)
        {
            GameObject bgGo = new GameObject(name);
            bgGo.transform.SetParent(parent, false);
            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 1f);
            bgRect.anchorMax = new Vector2(0f, 1f);
            bgRect.pivot = new Vector2(0f, 1f);
            bgRect.anchoredPosition = pos;
            bgRect.sizeDelta = new Vector2(332f, 34f);
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.18f, 0.22f, 0.30f, 1f);

            GameObject fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(bgGo.transform, false);
            var fRect = fillGo.AddComponent<RectTransform>();
            fRect.anchorMin = Vector2.zero;
            fRect.anchorMax = Vector2.one;
            fRect.offsetMin = new Vector2(3f, 3f);
            fRect.offsetMax = new Vector2(-3f, -3f);
            var fImg = fillGo.AddComponent<Image>();
            fImg.color = fillColor;
            fImg.type = Image.Type.Filled;
            fImg.fillMethod = Image.FillMethod.Horizontal;
            fImg.fillAmount = 1f;

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(bgGo.transform, false);
            var tRect = txtGo.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.offsetMin = new Vector2(10f, 0f);
            tRect.offsetMax = new Vector2(-10f, 0f);
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = initText;
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = new Color(0.06f, 0.08f, 0.12f, 1f);
            tmp.fontStyle = FontStyles.Bold;
            if (font != null) tmp.font = font;

            return (fImg, tmp);
        }

        private static void BuildGreenhouseBed(Sprite bedSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            GameObject existing = GameObject.Find("GreenhouseBed");
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing);

            GameObject bedGo = new GameObject("GreenhouseBed");
            bedGo.tag = "Interactable";
            bedGo.layer = 11;
            bedGo.transform.position = new Vector3(-19.5f, 12.5f, 0f);

            var sr = bedGo.AddComponent<SpriteRenderer>();
            sr.sprite = bedSpr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-bedGo.transform.position.y * 100f);

            var col = bedGo.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.95f, 1.35f);

            // World-space prompt "[E] Đi ngủ"
            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(bedGo.transform, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 50;

            var pRect = promptGo.GetComponent<RectTransform>();
            pRect.sizeDelta = new Vector2(180f, 44f);
            var pImg = promptGo.AddComponent<Image>();
            pImg.color = new Color(0.08f, 0.12f, 0.18f, 0.88f);

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(promptGo.transform, false);
            var tRect = txtGo.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.offsetMin = Vector2.zero;
            tRect.offsetMax = Vector2.zero;
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "[E] Đi ngủ";
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.94f, 0.65f, 1f);
            if (vnFont != null) tmp.font = vnFont;

            var bedComp = bedGo.AddComponent<BedInteractable>();
            var so = new SerializedObject(bedComp);
            so.FindProperty("promptObject").objectReferenceValue = promptGo;
            so.FindProperty("interactRadius").floatValue = 1.9f;
            so.ApplyModifiedPropertiesWithoutUndo();
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
