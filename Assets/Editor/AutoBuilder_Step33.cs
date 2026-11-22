using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step33
    {
        private const string SCENE_PATH = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MIU_DLG_PATH = "Assets/Data/Dialogues/dlg_miu_reunion.asset";
        private const string BLOOM_DLG_PATH = "Assets/Data/Dialogues/dlg_bloom_climax.asset";

        [MenuItem("Moonpetal/Build/Step 33 - Opening Cutscene, Miu Reunion & Bloom Climax")]
        public static void BuildStep33()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            EnsureFolder("Assets/Data", "Dialogues");

            DialogueData miuDlg = CreateOrUpdateDialogue(MIU_DLG_PATH, "dlg_miu_reunion", "flag_miu_comforted", new[]
            {
                ("Mèo Miu", "Meo... meo? Mấy cục bọt đen xấu xí biến mất hết rồi sao?"),
                ("Tinh Linh Lumi", "Khịt khịt! Hết mùi ẩm mốc rồi nhóc lông cam ơi. Chị Wen quét sạch bách rồi!"),
                ("Wen", "Lại đây nào Miu, không sao nữa rồi. Toby đang lo sốt vó ngoài làng kia kìa."),
                ("Mèo Miu", "Meo~! Rừ rừ... (Miu dụi đầu vào tay bạn rồi nhảy tót về hướng Làng Chính!)")
            });

            DialogueData bloomDlg = CreateOrUpdateDialogue(BLOOM_DLG_PATH, "dlg_bloom_climax", "", new[]
            {
                ("Tinh Linh Lumi", "Woa! Nhìn kìa Wen! Cánh hoa Moonpetal đang mở ra... làn sương tím tan sạch luôn rồi!"),
                ("Cậu Bé Toby", "Siêu ngầu luôn chị Wen ơi! Ánh trăng bay đầy khu vườn thật đấy! Miu cũng đang nhảy lên kìa!"),
                ("Bà Hana", "Ôi chao... Bao nhiêu năm rồi tôi mới lại ngửi thấy hương Moonpetal hòa với mùi bánh nướng ấm áp thế này."),
                ("Bác Otis", "E hèm... Theo ghi chép bưu cục, đây chính thức là ngày đẹp trời nhất của thung lũng trong ba năm qua."),
                ("Ông Bach", "Chậc. Bọn trẻ các cô cậu làm ầm ĩ quá đấy... Nhưng mà... cảm ơn cháu, Wen. Bà cháu hẳn đang mỉm cười."),
                ("Wen", "Cháu không làm một mình đâu ông ạ. Nhờ có cả thung lũng chung tay, khu vườn mới sống lại được."),
                ("Tinh Linh Lumi", "Nào, cầm lấy Cánh Hoa Hy Vọng đi Wen! Từ giờ cậu có thể bấm [Q] dùng Xung Ánh Trăng bất cứ lúc nào!")
            });

            var scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);

            TMP_FontAsset storyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_StoryFont.asset");
            if (storyFont == null) storyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_DynamicFont.asset");

            TMP_FontAsset hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_HudCleanFont.asset");
            if (hudFont == null) hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_DynamicFont.asset");

            BuildOrUpdateIntroCutsceneCanvas(storyFont, hudFont);
            WireMiuCat(miuDlg, hudFont);
            WireBundleBloomClimax(bloomDlg);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("[MV][Step33] Build thanh cong: IntroCutsceneCanvas (Tap Space + UI thu cong) + MiuCat + Bloom Climax.");
            ValidateStep33();
        }

        private static void BuildOrUpdateIntroCutsceneCanvas(TMP_FontAsset storyFont, TMP_FontAsset hudFont)
        {
            GameObject existing = GameObject.Find("IntroCutsceneCanvas");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject canvasGo = new GameObject("IntroCutsceneCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 220;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();
            var cg = canvasGo.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            Color barColor = new Color(0.06f, 0.05f, 0.08f, 0.99f);
            Color trimGold = new Color(0.62f, 0.50f, 0.32f, 0.55f);

            // Fade overlay
            GameObject fadeGo = CreateUIObject("FadeOverlay", canvasGo.transform);
            StretchFull(fadeGo.GetComponent<RectTransform>());
            var fadeImg = fadeGo.AddComponent<Image>();
            fadeImg.color = new Color(0.05f, 0.04f, 0.07f, 0f);
            fadeImg.raycastTarget = false;

            // Top letterbox + vien vang dong manh mep duoi
            GameObject topBar = CreateUIObject("TopLetterbox", canvasGo.transform);
            var topRt = topBar.GetComponent<RectTransform>();
            topRt.anchorMin = new Vector2(0f, 1f);
            topRt.anchorMax = new Vector2(1f, 1f);
            topRt.pivot = new Vector2(0.5f, 1f);
            topRt.anchoredPosition = Vector2.zero;
            topRt.sizeDelta = new Vector2(0f, 0f);
            var topImg = topBar.AddComponent<Image>();
            topImg.color = barColor;

            GameObject topTrim = CreateUIObject("TopTrimLine", topBar.transform);
            var topTrimRt = topTrim.GetComponent<RectTransform>();
            topTrimRt.anchorMin = new Vector2(0f, 0f);
            topTrimRt.anchorMax = new Vector2(1f, 0f);
            topTrimRt.pivot = new Vector2(0.5f, 0f);
            topTrimRt.anchoredPosition = Vector2.zero;
            topTrimRt.sizeDelta = new Vector2(0f, 2f);
            topTrim.AddComponent<Image>().color = trimGold;

            // Bottom letterbox + vien vang dong manh mep tren
            GameObject botBar = CreateUIObject("BottomLetterbox", canvasGo.transform);
            var botRt = botBar.GetComponent<RectTransform>();
            botRt.anchorMin = new Vector2(0f, 0f);
            botRt.anchorMax = new Vector2(1f, 0f);
            botRt.pivot = new Vector2(0.5f, 0f);
            botRt.anchoredPosition = Vector2.zero;
            botRt.sizeDelta = new Vector2(0f, 0f);
            var botImg = botBar.AddComponent<Image>();
            botImg.color = barColor;

            GameObject botTrim = CreateUIObject("BottomTrimLine", botBar.transform);
            var botTrimRt = botTrim.GetComponent<RectTransform>();
            botTrimRt.anchorMin = new Vector2(0f, 1f);
            botTrimRt.anchorMax = new Vector2(1f, 1f);
            botTrimRt.pivot = new Vector2(0.5f, 1f);
            botTrimRt.anchoredPosition = Vector2.zero;
            botTrimRt.sizeDelta = new Vector2(0f, 2f);
            botTrim.AddComponent<Image>().color = trimGold;

            // Nhom phu de (co CanvasGroup rieng de fade-in mem mai)
            GameObject subGroupGo = CreateUIObject("SubtitleGroup", canvasGo.transform);
            var subGroupRt = subGroupGo.GetComponent<RectTransform>();
            subGroupRt.anchorMin = new Vector2(0f, 0f);
            subGroupRt.anchorMax = new Vector2(1f, 0f);
            subGroupRt.pivot = new Vector2(0.5f, 0f);
            subGroupRt.anchoredPosition = Vector2.zero;
            subGroupRt.sizeDelta = new Vector2(0f, 96f);
            var subGroupCg = subGroupGo.AddComponent<CanvasGroup>();

            // Speaker label
            GameObject spGo = CreateUIObject("SpeakerLabel", subGroupGo.transform);
            var spRt = spGo.GetComponent<RectTransform>();
            spRt.anchorMin = new Vector2(0.14f, 0f);
            spRt.anchorMax = new Vector2(0.86f, 0f);
            spRt.pivot = new Vector2(0.5f, 0f);
            spRt.anchoredPosition = new Vector2(0f, 60f);
            spRt.sizeDelta = new Vector2(0f, 24f);
            var spTmp = spGo.AddComponent<TextMeshProUGUI>();
            spTmp.font = storyFont;
            spTmp.fontSize = 17f;
            spTmp.fontStyle = FontStyles.Bold;
            spTmp.alignment = TextAlignmentOptions.Center;
            spTmp.color = new Color(0.91f, 0.79f, 0.50f, 1f);
            spTmp.text = "";

            // Subtitle text
            GameObject subGo = CreateUIObject("SubtitleText", subGroupGo.transform);
            var subRt = subGo.GetComponent<RectTransform>();
            subRt.anchorMin = new Vector2(0.12f, 0f);
            subRt.anchorMax = new Vector2(0.88f, 0f);
            subRt.pivot = new Vector2(0.5f, 0f);
            subRt.anchoredPosition = new Vector2(0f, 14f);
            subRt.sizeDelta = new Vector2(0f, 44f);
            var subTmp = subGo.AddComponent<TextMeshProUGUI>();
            subTmp.font = storyFont;
            subTmp.fontSize = 20f;
            subTmp.alignment = TextAlignmentOptions.Center;
            subTmp.textWrappingMode = TextWrappingModes.Normal;
            subTmp.color = new Color(0.95f, 0.93f, 0.87f, 1f);
            subTmp.text = "";

            // Goi y goc phai duoi: "[Space] Tiep tuc"
            GameObject contGo = CreateUIObject("ContinueHintText", canvasGo.transform);
            var contRt = contGo.GetComponent<RectTransform>();
            contRt.anchorMin = new Vector2(1f, 0f);
            contRt.anchorMax = new Vector2(1f, 0f);
            contRt.pivot = new Vector2(1f, 0f);
            contRt.anchoredPosition = new Vector2(-24f, 14f);
            contRt.sizeDelta = new Vector2(160f, 22f);
            var contTmp = contGo.AddComponent<TextMeshProUGUI>();
            contTmp.font = hudFont;
            contTmp.fontSize = 13f;
            contTmp.alignment = TextAlignmentOptions.Right;
            contTmp.color = new Color(0.85f, 0.80f, 0.68f, 0.75f);
            contTmp.text = "[Space] Tiếp tục";

            // Goc phai tren: Giu [Space] / [Esc] de bo qua (thanh tien trinh chi hien khi dang giu phim)
            GameObject skipContainer = CreateUIObject("SkipContainer", canvasGo.transform);
            var skipRt = skipContainer.GetComponent<RectTransform>();
            skipRt.anchorMin = new Vector2(1f, 1f);
            skipRt.anchorMax = new Vector2(1f, 1f);
            skipRt.pivot = new Vector2(1f, 1f);
            skipRt.anchoredPosition = new Vector2(-24f, -20f);
            skipRt.sizeDelta = new Vector2(260f, 34f);

            GameObject skipHintGo = CreateUIObject("SkipHintText", skipContainer.transform);
            var hintRt = skipHintGo.GetComponent<RectTransform>();
            hintRt.anchorMin = new Vector2(0f, 0.36f);
            hintRt.anchorMax = new Vector2(1f, 1f);
            hintRt.offsetMin = Vector2.zero;
            hintRt.offsetMax = Vector2.zero;
            var hintTmp = skipHintGo.AddComponent<TextMeshProUGUI>();
            hintTmp.font = hudFont;
            hintTmp.fontSize = 13f;
            hintTmp.alignment = TextAlignmentOptions.Right;
            hintTmp.color = new Color(0.82f, 0.79f, 0.71f, 0.78f);
            hintTmp.text = "Giữ [Space] / [Esc] để bỏ qua";

            GameObject barBg = CreateUIObject("SkipBarBg", skipContainer.transform);
            var bgRt = barBg.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0.28f, 0f);
            bgRt.anchorMax = new Vector2(1f, 0.22f);
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;
            var bgImg = barBg.AddComponent<Image>();
            bgImg.color = new Color(0.32f, 0.27f, 0.20f, 0.55f);

            GameObject barFill = CreateUIObject("SkipBarFill", barBg.transform);
            StretchFull(barFill.GetComponent<RectTransform>());
            var fillImg = barFill.AddComponent<Image>();
            fillImg.color = new Color(0.85f, 0.66f, 0.31f, 0.95f);
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImg.fillAmount = 0f;
            barBg.SetActive(false);

            var mgr = canvasGo.AddComponent<IntroCutsceneManager>();
            var so = new SerializedObject(mgr);
            so.FindProperty("cutsceneRootGroup").objectReferenceValue = cg;
            so.FindProperty("fadeOverlay").objectReferenceValue = fadeImg;
            so.FindProperty("topLetterbox").objectReferenceValue = topRt;
            so.FindProperty("bottomLetterbox").objectReferenceValue = botRt;
            so.FindProperty("speakerText").objectReferenceValue = spTmp;
            so.FindProperty("subtitleText").objectReferenceValue = subTmp;
            so.FindProperty("subtitleGroup").objectReferenceValue = subGroupCg;
            so.FindProperty("skipBarRoot").objectReferenceValue = barBg;
            so.FindProperty("skipFillImage").objectReferenceValue = fillImg;
            so.FindProperty("skipHintText").objectReferenceValue = hintTmp;
            so.FindProperty("continueHintText").objectReferenceValue = contTmp;

            GameObject seedBox = GameObject.Find("GreenhouseSeedBox");
            if (seedBox != null)
            {
                so.FindProperty("seedBoxTransform").objectReferenceValue = seedBox.transform;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireMiuCat(DialogueData miuDlg, TMP_FontAsset hudFont)
        {
            GameObject catGo = GameObject.Find("MiuCat_Toby");
            if (catGo == null)
            {
                Debug.LogWarning("[MV][Step33] Khong tim thay MiuCat_Toby trong scene.");
                return;
            }

            var comp = catGo.GetComponent<MiuCatInteractable>();
            if (comp == null) comp = catGo.AddComponent<MiuCatInteractable>();

            Transform oldPrompt = catGo.transform.Find("MiuFloatingPrompt");
            if (oldPrompt != null) Object.DestroyImmediate(oldPrompt.gameObject);

            GameObject promptGo = new GameObject("MiuFloatingPrompt");
            promptGo.transform.SetParent(catGo.transform, false);
            promptGo.transform.localPosition = new Vector3(0f, 0.82f, 0f);
            var tmp = promptGo.AddComponent<TextMeshPro>();
            tmp.font = hudFont;
            tmp.fontSize = 2.6f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.97f, 0.85f, 1f);
            tmp.text = "[E] Mèo Miu";
            tmp.sortingLayerID = SortingLayer.NameToID("UI");
            tmp.sortingOrder = 20;
            promptGo.SetActive(false);

            Vector3 homePos = new Vector3(4.6f, 5.1f, 0f);
            GameObject toby = GameObject.Find("NPC_Toby");
            if (toby != null)
            {
                homePos = toby.transform.position + new Vector3(1.1f, -0.25f, 0f);
            }

            var so = new SerializedObject(comp);
            so.FindProperty("reunionDialogue").objectReferenceValue = miuDlg;
            so.FindProperty("homePositionNearToby").vector3Value = homePos;
            so.FindProperty("floatingPromptText").objectReferenceValue = tmp;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireBundleBloomClimax(DialogueData bloomDlg)
        {
            var bundleUI = Object.FindAnyObjectByType<BundleUIView>(FindObjectsInactive.Include);
            if (bundleUI == null)
            {
                Debug.LogWarning("[MV][Step33] Khong tim thay BundleUIView trong scene.");
                return;
            }

            var so = new SerializedObject(bundleUI);
            var prop = so.FindProperty("bloomClimaxDialogue");
            if (prop != null)
            {
                prop.objectReferenceValue = bloomDlg;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static DialogueData CreateOrUpdateDialogue(string path, string id, string flagOnFinish, (string sp, string tx)[] lines)
        {
            var asset = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.dialogueID = id;
            asset.setsStoryFlagOnFinish = flagOnFinish;
            asset.lines = new List<DialogueLine>();
            for (int i = 0; i < lines.Length; i++)
            {
                asset.lines.Add(new DialogueLine
                {
                    speakerNameKey = lines[i].sp,
                    textKey = lines[i].tx
                });
            }

            EditorUtility.SetDirty(asset);
            return asset;
        }

        [MenuItem("Moonpetal/Validate/Step 33")]
        public static void ValidateStep33()
        {
            int fails = 0;

            var cutscene = Object.FindAnyObjectByType<IntroCutsceneManager>(FindObjectsInactive.Include);
            if (cutscene == null)
            {
                Debug.LogError("[MV][Step33][FAIL] Thieu IntroCutsceneManager trong scene.");
                fails++;
            }

            var miu = Object.FindAnyObjectByType<MiuCatInteractable>(FindObjectsInactive.Include);
            if (miu == null)
            {
                Debug.LogError("[MV][Step33][FAIL] Thieu MiuCatInteractable tren MiuCat_Toby.");
                fails++;
            }

            var miuDlg = AssetDatabase.LoadAssetAtPath<DialogueData>(MIU_DLG_PATH);
            var bloomDlg = AssetDatabase.LoadAssetAtPath<DialogueData>(BLOOM_DLG_PATH);
            if (miuDlg == null || miuDlg.lines.Count < 4)
            {
                Debug.LogError("[MV][Step33][FAIL] Asset dlg_miu_reunion.asset khong hop le.");
                fails++;
            }
            if (bloomDlg == null || bloomDlg.lines.Count < 6)
            {
                Debug.LogError("[MV][Step33][FAIL] Asset dlg_bloom_climax.asset khong hop le.");
                fails++;
            }

            var bundleUI = Object.FindAnyObjectByType<BundleUIView>(FindObjectsInactive.Include);
            if (bundleUI != null)
            {
                var so = new SerializedObject(bundleUI);
                var prop = so.FindProperty("bloomClimaxDialogue");
                if (prop == null || prop.objectReferenceValue == null)
                {
                    Debug.LogError("[MV][Step33][FAIL] BundleUIView chua duoc gan bloomClimaxDialogue.");
                    fails++;
                }
            }

            if (fails == 0)
            {
                Debug.Log("[MV][Step33] VALIDATE PASS (0 loi)!");
            }
            else
            {
                Debug.LogError("[MV][Step33] VALIDATE FAIL (" + fails + " loi).");
            }
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string full = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(full))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
