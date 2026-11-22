using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Audio;
using MoonpetalValley.UI;
using MoonpetalValley.Player;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step10
    {
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 10/1. Build Save-Load, Audio & Pause Menu")]
        public static void BuildStep10()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 10 ===");
            AssetDatabase.Refresh();
            UpgradeGameRootWithSaveAudioAndPause();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 10! Chạy 'Moonpetal/Step 10/Validate (Final MVP QA)' để kiểm toán toàn bộ MVP. ===");
        }

        [MenuItem("Moonpetal/Step 10/Validate (Final MVP QA)")]
        public static void ValidateFinalMVP()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU FINAL MVP QA VALIDATE (STEP 1 -> STEP 10) ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab");
                errors++;
            }
            else
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                string[] reqFields =
                {
                    "gameStateManager", "storyFlagManager", "localization", "sceneLoader",
                    "toastUI", "debugOverlay", "timeManager", "statsController",
                    "sleepController", "inventoryManager", "dialogueManager", "diaryManager",
                    "questManager", "bundleManager", "saveLoadManager", "audioManager"
                };
                foreach (string f in reqFields)
                {
                    if (rootSO.FindProperty(f).objectReferenceValue == null)
                    {
                        Debug.LogError("[MV] GameRoot thiếu tham chiếu: " + f);
                        errors++;
                    }
                }
                if (prefab.GetComponentInChildren<PauseMenuUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu PauseMenuUIView trong GameRoot.prefab.");
                    errors++;
                }
            }

            string[] requiredAudio =
            {
                "Assets/Audio/BGM/bgm_valley_cozy.wav",
                "Assets/Audio/SFX/sfx_ui_click.wav",
                "Assets/Audio/SFX/sfx_tool.wav",
                "Assets/Audio/SFX/sfx_slash.wav",
                "Assets/Audio/SFX/sfx_hit.wav",
                "Assets/Audio/SFX/sfx_pickup.wav",
                "Assets/Audio/SFX/sfx_bloom.wav"
            };
            foreach (string path in requiredAudio)
            {
                if (AssetDatabase.LoadAssetAtPath<AudioClip>(path) == null)
                {
                    Debug.LogError("[MV] THIẾU AudioClip: " + path);
                    errors++;
                }
            }

            if (UnityEngine.Object.FindAnyObjectByType<PlayerController2D>() == null) { Debug.LogError("[MV] Thiếu Player_Wen."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<PlayerCombatController>() == null) { Debug.LogError("[MV] Thiếu PlayerCombatController."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include).Length < 12) { Debug.LogError("[MV] Thiếu 12 FarmTile."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include).Length < 4) { Debug.LogError("[MV] Thiếu 4 NPC."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include).Length < 8) { Debug.LogError("[MV] Thiếu 8 GloomSlime."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>() == null) { Debug.LogError("[MV] Thiếu PlantingSpotController."); errors++; }

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
                Debug.Log("[MV] VALIDATE STEP 10 (FINAL MVP QA): PASS (0 lỗi). MOONPETAL VALLEY MVP ĐÃ HOÀN THIỆN 100%!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 10 (FINAL MVP QA): FAIL (" + errors + " lỗi).");
            }
        }

        [MenuItem("Moonpetal/Debug/Open Save Folder")]
        public static void OpenSaveFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        private static void UpgradeGameRootWithSaveAudioAndPause()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var saveMgr = root.GetComponent<SaveLoadManager>() ?? root.AddComponent<SaveLoadManager>();
            var audioMgr = root.GetComponent<AudioManager>() ?? root.AddComponent<AudioManager>();

            // Tạo 2 AudioSource con (BGM & SFX)
            Transform oldAudio = root.transform.Find("AudioSources");
            if (oldAudio != null) UnityEngine.Object.DestroyImmediate(oldAudio.gameObject);
            GameObject audioHost = new GameObject("AudioSources");
            audioHost.transform.SetParent(root.transform, false);
            var bgmSrc = audioHost.AddComponent<AudioSource>();
            bgmSrc.playOnAwake = false;
            var sfxSrc = audioHost.AddComponent<AudioSource>();
            sfxSrc.playOnAwake = false;

            var aSO = new SerializedObject(audioMgr);
            aSO.FindProperty("bgmSource").objectReferenceValue = bgmSrc;
            aSO.FindProperty("sfxSource").objectReferenceValue = sfxSrc;
            aSO.FindProperty("bgmCozyClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/bgm_valley_cozy.wav");
            aSO.FindProperty("sfxUiClick").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_ui_click.wav");
            aSO.FindProperty("sfxTool").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_tool.wav");
            aSO.FindProperty("sfxSlash").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_slash.wav");
            aSO.FindProperty("sfxHit").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_hit.wav");
            aSO.FindProperty("sfxPickup").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_pickup.wav");
            aSO.FindProperty("sfxBloom").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_bloom.wav");
            aSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("saveLoadManager").objectReferenceValue = saveMgr;
            rootSO.FindProperty("audioManager").objectReferenceValue = audioMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // Pause Menu UI trong ClockHUDCanvas
            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                Transform oldPause = hudCanvas.Find("PauseMenuHost");
                if (oldPause != null) UnityEngine.Object.DestroyImmediate(oldPause.gameObject);

                GameObject host = new GameObject("PauseMenuHost");
                host.transform.SetParent(hudCanvas, false);
                var hr = host.AddComponent<RectTransform>();
                hr.anchorMin = Vector2.zero; hr.anchorMax = Vector2.one;
                hr.offsetMin = Vector2.zero; hr.offsetMax = Vector2.zero;

                GameObject panel = new GameObject("PausePanelRoot");
                panel.transform.SetParent(host.transform, false);
                var pr = panel.AddComponent<RectTransform>();
                pr.anchorMin = new Vector2(0.5f, 0.5f); pr.anchorMax = new Vector2(0.5f, 0.5f);
                pr.sizeDelta = new Vector2(460f, 420f);
                panel.AddComponent<Image>().color = new Color(0.08f, 0.11f, 0.18f, 0.96f);

                var vlg = panel.AddComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(28, 28, 22, 22);
                vlg.spacing = 12f;
                vlg.childControlWidth = true;
                vlg.childControlHeight = false;

                var title = CreateText("Title", panel.transform, "TẠM DỪNG — MOONPETAL VALLEY", vnFont, 22, new Color(1f, 0.90f, 0.54f, 1f), 34f);
                var status = CreateText("SaveStatus", panel.transform, "Chưa có bản lưu", vnFont, 15, new Color(0.80f, 0.88f, 0.96f, 1f), 28f);

                var (btnResume, _) = CreateMenuBtn("BtnResume", panel.transform, "Tiếp Tục Chơi [Esc]", new Color(0.20f, 0.54f, 0.40f, 1f), vnFont);
                var (btnSave, _) = CreateMenuBtn("BtnSave", panel.transform, "Lưu Game (JSON)", new Color(0.22f, 0.42f, 0.66f, 1f), vnFont);
                var (btnLoad, _) = CreateMenuBtn("BtnLoad", panel.transform, "Tải Game Đã Lưu", new Color(0.22f, 0.42f, 0.66f, 1f), vnFont);
                var (btnMute, muteTxt) = CreateMenuBtn("BtnMute", panel.transform, "Âm Thanh: ĐANG BẬT", new Color(0.42f, 0.36f, 0.22f, 1f), vnFont);
                var (btnDel, _) = CreateMenuBtn("BtnDeleteSave", panel.transform, "Xóa File Lưu Game", new Color(0.54f, 0.22f, 0.24f, 1f), vnFont);

                var view = host.AddComponent<PauseMenuUIView>();
                var vSO = new SerializedObject(view);
                vSO.FindProperty("panelRoot").objectReferenceValue = panel;
                vSO.FindProperty("saveStatusText").objectReferenceValue = status;
                vSO.FindProperty("muteButtonText").objectReferenceValue = muteTxt;
                vSO.FindProperty("btnResume").objectReferenceValue = btnResume;
                vSO.FindProperty("btnSave").objectReferenceValue = btnSave;
                vSO.FindProperty("btnLoad").objectReferenceValue = btnLoad;
                vSO.FindProperty("btnMute").objectReferenceValue = btnMute;
                vSO.FindProperty("btnDeleteSave").objectReferenceValue = btnDel;
                vSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static TMP_Text CreateText(string name, Transform parent, string text, TMP_FontAsset font, float size, Color color, float height)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, height);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = TextAlignmentOptions.Center; tmp.color = color;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static (Button btn, TMP_Text label) CreateMenuBtn(string name, Transform parent, string text, Color bg, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 44f);
            var img = go.AddComponent<Image>(); img.color = bg;
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;

            GameObject tGo = new GameObject("Label");
            tGo.transform.SetParent(go.transform, false);
            var r = tGo.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
            var tmp = tGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = 18; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
            if (font != null) tmp.font = font;
            return (btn, tmp);
        }
    }
}
