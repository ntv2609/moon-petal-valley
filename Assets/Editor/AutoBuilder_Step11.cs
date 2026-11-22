using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step11
    {
        private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 11/1. Build Main Menu, Polish & Visual Upgrade")]
        public static void BuildStep11()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 11 ===");

            Sprite petalSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/UI/spr_ambient_petal.png", 8, 8, DrawAmbientPetal);
            Sprite bakerySpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_house_bakery.png", 48, 40, DrawBakeryHouse);
            Sprite shedSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_house_shed.png", 40, 32, DrawGardenShed);
            Sprite flowerTuftSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_decor_flowers.png", 16, 16, DrawFlowerTuft);
            Sprite bushSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_decor_bush.png", 16, 16, DrawDecorBush);

            UpgradeGameRootWithJuice();
            BuildMainMenuScene(petalSpr);
            UpdateBootstrapToMainMenu();
            UpdateBuildSettings();

            var worldScene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildWorldPolishAndDecor(petalSpr, bakerySpr, shedSpr, flowerTuftSpr, bushSpr);
            EditorSceneManager.MarkSceneDirty(worldScene);
            EditorSceneManager.SaveScene(worldScene);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 11! Chạy 'Moonpetal/Step 11/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 11/Validate")]
        public static void ValidateStep11()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 11 ===");

            if (EditorBuildSettings.scenes.Length < 3 ||
                EditorBuildSettings.scenes[0].path != BootstrapScenePath ||
                EditorBuildSettings.scenes[1].path != MainMenuScenePath ||
                EditorBuildSettings.scenes[2].path != WorldScenePath)
            {
                Debug.LogError("[MV] Build Settings chưa đủ 3 scene: Bootstrap -> MainMenu -> MoonpetalValley_MVP.");
                errors++;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null || prefab.GetComponent<JuiceEffectsManager>() == null)
            {
                Debug.LogError("[MV] GameRoot.prefab thiếu JuiceEffectsManager.");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<AmbientPetalSpawner>() == null)
            {
                Debug.LogError("[MV] Thiếu AmbientPetalSpawner trong scene thế giới.");
                errors++;
            }

            if (GameObject.Find("VillageDecorGroup") == null)
            {
                Debug.LogError("[MV] Thiếu VillageDecorGroup (Tiệm Bánh, Chòi Vườn & Hoa cỏ).");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 11: PASS (0 lỗi). Main Menu, Damage Popups, Screen Shake, Cánh Hoa Bay & Kiến Trúc Làng sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 11: FAIL (" + errors + " lỗi).");
            }
        }

        private static void UpgradeGameRootWithJuice()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var juice = root.GetComponent<JuiceEffectsManager>() ?? root.AddComponent<JuiceEffectsManager>();
            var so = new SerializedObject(juice);
            so.FindProperty("vnFont").objectReferenceValue = vnFont;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildMainMenuScene(Sprite petalSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject gameRootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.backgroundColor = new Color(0.08f, 0.12f, 0.22f, 1f);
            camGo.AddComponent<AudioListener>();

            // EventSystem khớp Input backend
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Type modType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (modType != null) esGo.AddComponent(modType);
            else esGo.AddComponent<StandaloneInputModule>();
#else
            esGo.AddComponent<StandaloneInputModule>();
#endif

            PrefabUtility.InstantiatePrefab(gameRootPrefab);

            // Cánh hoa bay ở Main Menu
            GameObject spawnerGo = new GameObject("MenuPetalSpawner");
            var spawner = spawnerGo.AddComponent<AmbientPetalSpawner>();
            var spSO = new SerializedObject(spawner);
            spSO.FindProperty("petalSprite").objectReferenceValue = petalSpr;
            spSO.ApplyModifiedPropertiesWithoutUndo();

            // Canvas Main Menu (SortingOrder = 60, đè lên HUD khi ở Menu)
            GameObject canvasGo = new GameObject("MainMenuCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 60;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject bg = new GameObject("Backdrop");
            bg.transform.SetParent(canvasGo.transform, false);
            var bgR = bg.AddComponent<RectTransform>();
            bgR.anchorMin = Vector2.zero; bgR.anchorMax = Vector2.one;
            bgR.offsetMin = Vector2.zero; bgR.offsetMax = Vector2.zero;
            bg.AddComponent<Image>().color = new Color(0.06f, 0.10f, 0.19f, 0.94f);

            GameObject card = new GameObject("MenuCard");
            card.transform.SetParent(canvasGo.transform, false);
            var cR = card.AddComponent<RectTransform>();
            cR.anchorMin = new Vector2(0.5f, 0.5f); cR.anchorMax = new Vector2(0.5f, 0.5f);
            cR.sizeDelta = new Vector2(680f, 470f);
            card.AddComponent<Image>().color = new Color(0.10f, 0.15f, 0.25f, 0.95f);

            var vlg = card.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(48, 48, 36, 36);
            vlg.spacing = 16f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;

            CreateMenuText("Title", card.transform, "THUNG LŨNG HOA ÁNH TRĂNG", vnFont, 38, new Color(1f, 0.90f, 0.56f, 1f), 52f);
            CreateMenuText("Subtitle", card.transform, "MOONPETAL VALLEY — Cozy Action-Farming RPG (MVP v1.0)", vnFont, 18, new Color(0.72f, 0.92f, 0.96f, 1f), 30f);
            var subSave = CreateMenuText("SaveHint", card.transform, "Đang kiểm tra bản lưu...", vnFont, 16, new Color(0.84f, 0.88f, 0.94f, 1f), 28f);

            Button btnNew = CreateMenuButton("BtnNewGame", card.transform, "Chơi Mới", new Color(0.20f, 0.58f, 0.42f, 1f), vnFont);
            Button btnCont = CreateMenuButton("BtnContinue", card.transform, "Tiếp Tục Hành Trình", new Color(0.24f, 0.44f, 0.68f, 1f), vnFont);
            Button btnQuit = CreateMenuButton("BtnQuit", card.transform, "Thoát Game", new Color(0.50f, 0.22f, 0.26f, 1f), vnFont);

            var ctrl = canvasGo.AddComponent<MainMenuController>();
            var mSO = new SerializedObject(ctrl);
            mSO.FindProperty("btnNewGame").objectReferenceValue = btnNew;
            mSO.FindProperty("btnContinue").objectReferenceValue = btnCont;
            mSO.FindProperty("btnQuit").objectReferenceValue = btnQuit;
            mSO.FindProperty("continueSubtext").objectReferenceValue = subSave;
            mSO.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        }

        private static void UpdateBootstrapToMainMenu()
        {
            var scene = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            var loader = UnityEngine.Object.FindAnyObjectByType<BootstrapLoader>();
            if (loader != null)
            {
                var so = new SerializedObject(loader);
                so.FindProperty("targetSceneName").stringValue = "MainMenu";
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void UpdateBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(WorldScenePath, true)
            };
        }

        private static void BuildWorldPolishAndDecor(Sprite petalSpr, Sprite bakerySpr, Sprite shedSpr, Sprite flowerSpr, Sprite bushSpr)
        {
            GameObject oldSpawner = GameObject.Find("AmbientPetalSpawner");
            if (oldSpawner != null) UnityEngine.Object.DestroyImmediate(oldSpawner);
            GameObject spawnerGo = new GameObject("AmbientPetalSpawner");
            var spawner = spawnerGo.AddComponent<AmbientPetalSpawner>();
            var spSO = new SerializedObject(spawner);
            spSO.FindProperty("petalSprite").objectReferenceValue = petalSpr;
            spSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject oldDecor = GameObject.Find("VillageDecorGroup");
            if (oldDecor != null) UnityEngine.Object.DestroyImmediate(oldDecor);
            GameObject group = new GameObject("VillageDecorGroup");

            // Tiệm Bánh Bà Hana & Chòi Vườn Ông Bách
            SpawnDecor(group.transform, "House_HanaBakery", bakerySpr, new Vector2(-6.5f, 5.4f), true, new Vector2(2.6f, 1.4f));
            SpawnDecor(group.transform, "House_BachShed", shedSpr, new Vector2(-4.5f, -4.5f), true, new Vector2(2.1f, 1.2f));

            // 16 cụm hoa cỏ & bụi cây trang trí khắp 4 phân khu
            Vector2[] flowers =
            {
                new Vector2(-11.0f, 3.0f), new Vector2(-8.5f, 1.5f), new Vector2(-3.0f, 1.0f), new Vector2(3.5f, 1.2f),
                new Vector2(6.5f, 4.5f), new Vector2(-2.5f, -2.0f), new Vector2(4.0f, -2.2f), new Vector2(-5.0f, -9.5f),
                new Vector2(5.2f, -9.5f), new Vector2(13.5f, 9.0f), new Vector2(18.5f, 4.5f), new Vector2(14.5f, 2.0f)
            };
            for (int i = 0; i < flowers.Length; i++)
            {
                SpawnDecor(group.transform, "FlowerTuft_" + (i + 1), flowerSpr, flowers[i], false, Vector2.zero);
            }

            Vector2[] bushes =
            {
                new Vector2(-8.2f, 5.2f), new Vector2(-4.2f, 5.2f), new Vector2(4.2f, 5.2f), new Vector2(9.2f, 4.2f)
            };
            for (int i = 0; i < bushes.Length; i++)
            {
                SpawnDecor(group.transform, "DecorBush_" + (i + 1), bushSpr, bushes[i], true, new Vector2(0.75f, 0.65f));
            }
        }

        private static void SpawnDecor(Transform parent, string name, Sprite spr, Vector2 pos, bool solid, Vector2 colSize)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = solid ? "WorldObjects" : "FloorDecal";
            sr.sortingOrder = solid ? Mathf.RoundToInt(-pos.y * 100f) : 10;
            if (solid)
            {
                go.layer = 10;
                var col = go.AddComponent<BoxCollider2D>();
                col.size = colSize;
                col.offset = new Vector2(0f, -colSize.y * 0.25f);
            }
        }

        private static TMP_Text CreateMenuText(string name, Transform parent, string text, TMP_FontAsset font, float size, Color color, float height)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, height);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = TextAlignmentOptions.Center; tmp.color = color;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateMenuButton(string name, Transform parent, string text, Color bg, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 54f);
            var img = go.AddComponent<Image>(); img.color = bg;
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;

            GameObject tGo = new GameObject("Label");
            tGo.transform.SetParent(go.transform, false);
            var r = tGo.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
            var tmp = tGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = 22; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
            if (font != null) tmp.font = font;
            return btn;
        }

        private static void DrawAmbientPetal(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 2, 4, 4, Color.white);
            PlaceholderGeneratorUtility.FillRect(t, 1, 3, 6, 2, new Color(1f, 0.94f, 0.98f, 0.85f));
        }

        private static void DrawBakeryHouse(Texture2D t)
        {
            Color roof = new Color(0.76f, 0.32f, 0.28f);
            Color roofTrim = new Color(0.56f, 0.22f, 0.18f);
            Color wall = new Color(0.94f, 0.88f, 0.74f);
            Color wood = new Color(0.46f, 0.30f, 0.18f);
            Color win = new Color(1.0f, 0.86f, 0.44f);

            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 40, 20, wall);
            PlaceholderGeneratorUtility.FillRect(t, 2, 20, 44, 16, roof);
            PlaceholderGeneratorUtility.FillRect(t, 6, 34, 36, 4, roofTrim);
            PlaceholderGeneratorUtility.FillRect(t, 19, 2, 10, 14, wood);
            PlaceholderGeneratorUtility.FillRect(t, 8, 9, 8, 7, win);
            PlaceholderGeneratorUtility.FillRect(t, 32, 9, 8, 7, win);
            PlaceholderGeneratorUtility.FillRect(t, 16, 17, 16, 4, new Color(0.96f, 0.78f, 0.34f));
        }

        private static void DrawGardenShed(Texture2D t)
        {
            Color roof = new Color(0.32f, 0.46f, 0.36f);
            Color wall = new Color(0.62f, 0.46f, 0.30f);
            Color door = new Color(0.40f, 0.26f, 0.16f);
            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 32, 16, wall);
            PlaceholderGeneratorUtility.FillRect(t, 2, 16, 36, 13, roof);
            PlaceholderGeneratorUtility.FillRect(t, 15, 2, 10, 12, door);
        }

        private static void DrawFlowerTuft(Texture2D t)
        {
            Color g = new Color(0.24f, 0.52f, 0.26f);
            Color p1 = new Color(0.98f, 0.76f, 0.88f);
            Color p2 = new Color(0.98f, 0.90f, 0.44f);
            PlaceholderGeneratorUtility.FillRect(t, 3, 3, 2, 5, g);
            PlaceholderGeneratorUtility.FillRect(t, 9, 4, 2, 5, g);
            PlaceholderGeneratorUtility.FillRect(t, 2, 7, 4, 4, p1);
            PlaceholderGeneratorUtility.FillRect(t, 8, 8, 4, 4, p2);
        }

        private static void DrawDecorBush(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 2, 12, 10, new Color(0.18f, 0.44f, 0.24f));
            PlaceholderGeneratorUtility.FillRect(t, 4, 5, 8, 7, new Color(0.26f, 0.58f, 0.32f));
            PlaceholderGeneratorUtility.FillRect(t, 5, 8, 2, 2, new Color(0.98f, 0.72f, 0.84f));
            PlaceholderGeneratorUtility.FillRect(t, 10, 6, 2, 2, new Color(0.98f, 0.88f, 0.42f));
        }
    }
}
