using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step31
    {
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string DarkWoodPath = "Assets/Art/Sprites/UI/ui_panel_dark_wood.png";
        private const string MenuFontPath = "Assets/Art/Fonts/VN_MenuFont.asset";
        private const string StoryFontPath = "Assets/Art/Fonts/VN_StoryFont.asset";
        private const string StoryFontTtf = "Assets/Art/Fonts/VN_StorySerif.ttf";
        private const string HudCleanFontPath = "Assets/Art/Fonts/VN_HudCleanFont.asset";
        private const string HudCleanFontTtf = "Assets/Art/Fonts/VN_HudClean.ttf";
        private const string FallbackFontPath = "Assets/Art/Fonts/VN_DynamicFont.asset";

        [MenuItem("Moonpetal/Step 31/1. Upgrade Fonts, Low-Stat Flashing, Spacing & Knockout UI")]
        public static void ApplyStep31Polish()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Khong the chay AutoBuilder khi dang Play Mode hoac dang bien dich.");
                return;
            }

            TMP_FontAsset storyFont = EnsureFontAsset(StoryFontPath, StoryFontTtf, "VN_StoryFont", new[]
            {
                "C:/Windows/Fonts/georgia.ttf",
                "C:/Windows/Fonts/pala.ttf",
                "C:/Windows/Fonts/times.ttf",
                "C:/Windows/Fonts/segoeui.ttf"
            });

            TMP_FontAsset hudCleanFont = EnsureFontAsset(HudCleanFontPath, HudCleanFontTtf, "VN_HudCleanFont", new[]
            {
                "C:/Windows/Fonts/trebuc.ttf",
                "C:/Windows/Fonts/verdana.ttf",
                "C:/Windows/Fonts/tahoma.ttf",
                "C:/Windows/Fonts/segoeui.ttf"
            });

            TMP_FontAsset menuBoldFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(MenuFontPath);
            if (menuBoldFont == null) menuBoldFont = storyFont;

            Sprite darkWood = AssetDatabase.LoadAssetAtPath<Sprite>(DarkWoodPath);

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null)
            {
                Debug.LogError("[MV][Step31] Khong tim thay " + GameRootPrefabPath);
                return;
            }

            try
            {
                Transform hud = root.transform.Find("ClockHUDCanvas");
                if (hud != null)
                {
                    UpgradeDialogueFonts(hud, menuBoldFont, storyFont);
                    UpgradeQuestTrackerFont(hud, hudCleanFont);
                    RedesignExhaustedBadge(hud, darkWood, hudCleanFont);
                    FixPromptAndHotbarSpacing(root, hud, darkWood, hudCleanFont);
                }

                UpgradeDayTransitionScreen(root, darkWood, menuBoldFont, storyFont);

                PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
                Debug.Log("[MV][Step31] HOAN TAT: Da nang cap Font Hop Thoai, Hop Nhiem Vu, Canh bao Kiet suc, Khoang cach Prompt/Hotbar & Man hinh Ngat xiu!");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static TMP_FontAsset EnsureFontAsset(string assetPath, string ttfCopyPath, string assetName, string[] candidates)
        {
            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (existing != null) return existing;

            Font srcFont = null;
            foreach (string sysPath in candidates)
            {
                if (File.Exists(sysPath))
                {
                    File.Copy(sysPath, ttfCopyPath, true);
                    AssetDatabase.ImportAsset(ttfCopyPath, ImportAssetOptions.ForceUpdate);
                    srcFont = AssetDatabase.LoadAssetAtPath<Font>(ttfCopyPath);
                    if (srcFont != null)
                    {
                        Debug.Log("[MV][Step31] Da nap font tieng Viet " + assetName + " tu: " + sysPath);
                        break;
                    }
                }
            }

            if (srcFont == null) return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FallbackFontPath);

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(srcFont);
            if (fontAsset == null) return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FallbackFontPath);

            fontAsset.name = assetName;
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            AssetDatabase.CreateAsset(fontAsset, assetPath);
            if (fontAsset.material != null)
            {
                fontAsset.material.name = assetName + " Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, assetPath);
            }
            if (fontAsset.atlasTexture != null)
            {
                fontAsset.atlasTexture.name = assetName + " Atlas";
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, assetPath);
            }
            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            return fontAsset;
        }

        private static void UpgradeDialogueFonts(Transform hud, TMP_FontAsset boldFont, TMP_FontAsset storyFont)
        {
            Transform dlgBox = hud.Find("DialogueUIHost/DialogueBox");
            if (dlgBox == null) return;

            var sp = dlgBox.Find("SpeakerText")?.GetComponent<TMP_Text>();
            if (sp != null)
            {
                if (boldFont != null) sp.font = boldFont;
                sp.fontSize = 26f;
                sp.fontStyle = FontStyles.Bold;
                sp.color = new Color32(142, 54, 24, 255);
                EditorUtility.SetDirty(sp);
            }

            var bd = dlgBox.Find("BodyText")?.GetComponent<TMP_Text>();
            if (bd != null)
            {
                if (storyFont != null) bd.font = storyFont;
                bd.fontSize = 22f;
                bd.lineSpacing = 8f;
                bd.color = new Color32(52, 32, 22, 255);
                EditorUtility.SetDirty(bd);
            }

            var ht = dlgBox.Find("ContinueHint")?.GetComponent<TMP_Text>();
            if (ht != null)
            {
                if (storyFont != null) ht.font = storyFont;
                ht.fontSize = 16f;
                ht.fontStyle = FontStyles.Italic;
                ht.color = new Color32(116, 74, 44, 255);
                EditorUtility.SetDirty(ht);
            }
        }

        private static void UpgradeQuestTrackerFont(Transform hud, TMP_FontAsset hudFont)
        {
            Transform questP = hud.Find("QuestTrackerPanel");
            if (questP == null) return;

            var tmp = questP.Find("ObjectiveText")?.GetComponent<TMP_Text>();
            if (tmp != null)
            {
                if (hudFont != null) tmp.font = hudFont;
                tmp.fontSize = 17f;
                tmp.lineSpacing = 6f;
                tmp.color = new Color32(250, 240, 215, 255);
                var sh = tmp.GetComponent<Shadow>() ?? tmp.gameObject.AddComponent<Shadow>();
                sh.effectColor = new Color(0.08f, 0.05f, 0.03f, 0.85f);
                sh.effectDistance = new Vector2(1.2f, -1.2f);
                EditorUtility.SetDirty(tmp);
            }
        }

        private static void RedesignExhaustedBadge(Transform hud, Sprite darkWood, TMP_FontAsset hudFont)
        {
            Transform statsP = hud.Find("StatsPanel");
            if (statsP == null) return;
            StatsHUDView statsView = statsP.GetComponent<StatsHUDView>();

            Transform oldInside = statsP.Find("ExhaustedBadge");
            if (oldInside != null) UnityEngine.Object.DestroyImmediate(oldInside.gameObject);

            var spRect = statsP.GetComponent<RectTransform>();
            if (spRect != null) spRect.sizeDelta = new Vector2(376f, 108f);

            Transform existingCard = hud.Find("ExhaustedWarningCard");
            GameObject cardGo = existingCard != null
                ? existingCard.gameObject
                : new GameObject("ExhaustedWarningCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            if (existingCard == null) cardGo.transform.SetParent(hud, false);

            var cr = cardGo.GetComponent<RectTransform>();
            cr.anchorMin = new Vector2(0f, 1f);
            cr.anchorMax = new Vector2(0f, 1f);
            cr.pivot = new Vector2(0f, 1f);
            cr.anchoredPosition = new Vector2(24f, -142f);
            cr.sizeDelta = new Vector2(376f, 54f);
            cr.localScale = Vector3.one;

            var cImg = cardGo.GetComponent<Image>();
            if (darkWood != null)
            {
                cImg.sprite = darkWood;
                cImg.type = Image.Type.Sliced;
                cImg.pixelsPerUnitMultiplier = 0.55f;
                cImg.color = new Color(1.0f, 0.86f, 0.78f, 0.96f);
            }
            else
            {
                cImg.color = new Color(0.28f, 0.12f, 0.08f, 0.94f);
            }
            cImg.raycastTarget = false;

            Transform txtTf = cardGo.transform.Find("WarningText");
            GameObject txtGo = txtTf != null ? txtTf.gameObject : new GameObject("WarningText", typeof(RectTransform), typeof(TextMeshProUGUI));
            if (txtTf == null) txtGo.transform.SetParent(cardGo.transform, false);

            var tr = txtGo.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = new Vector2(16f, 6f);
            tr.offsetMax = new Vector2(-16f, -6f);

            var tmp = txtGo.GetComponent<TextMeshProUGUI>();
            if (hudFont != null) tmp.font = hudFont;
            tmp.fontSize = 14.5f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.lineSpacing = 2f;
            tmp.color = new Color32(255, 198, 128, 255);
            tmp.text = "KIỆT SỨC: Trần Thể lực còn 50%\nĂn Bánh Mì Nắng Ấm để hồi phục";
            tmp.raycastTarget = false;

            cardGo.SetActive(false);

            if (statsView != null)
            {
                var so = new SerializedObject(statsView);
                so.FindProperty("exhaustedBadge").objectReferenceValue = cardGo;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(statsView);
            }
        }

        private static Transform FindInteractionPromptBar(GameObject root)
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "InteractionPromptBar") return t;
            }
            return null;
        }

        private static void FixPromptAndHotbarSpacing(GameObject root, Transform hud, Sprite darkWood, TMP_FontAsset hudFont)
        {
            // Tim InteractionPromptBar o bat ky Canvas nao va chuyen gon ve ClockHUDCanvas
            Transform bar = FindInteractionPromptBar(root);
            if (bar != null)
            {
                if (bar.parent != hud)
                {
                    bar.SetParent(hud, false);
                }

                var br = bar.GetComponent<RectTransform>();
                br.anchorMin = new Vector2(0.5f, 0f);
                br.anchorMax = new Vector2(0.5f, 0f);
                br.pivot = new Vector2(0.5f, 0f);
                br.anchoredPosition = new Vector2(0f, 158f);
                br.sizeDelta = new Vector2(580f, 46f);
                br.localScale = Vector3.one;

                var bImg = bar.GetComponent<Image>();
                if (bImg != null && darkWood != null)
                {
                    bImg.sprite = darkWood;
                    bImg.type = Image.Type.Sliced;
                    bImg.pixelsPerUnitMultiplier = 0.55f;
                    bImg.color = Color.white;
                }

                var lbl = bar.Find("PromptLabel")?.GetComponent<TMP_Text>();
                if (lbl != null)
                {
                    if (hudFont != null) lbl.font = hudFont;
                    lbl.fontSize = 19f;
                    lbl.color = new Color32(252, 242, 212, 255);
                    EditorUtility.SetDirty(lbl);
                }
                EditorUtility.SetDirty(bar.gameObject);
            }

            Transform itemLbl = hud.Find("HotbarPanel/SelectedItemLabel");
            if (itemLbl != null)
            {
                var tmp = itemLbl.GetComponent<TMP_Text>();
                if (tmp != null)
                {
                    if (hudFont != null) tmp.font = hudFont;
                    tmp.fontSize = 18f;
                    tmp.fontStyle = FontStyles.Bold;
                    var sh = itemLbl.GetComponent<Shadow>() ?? itemLbl.gameObject.AddComponent<Shadow>();
                    sh.effectColor = new Color(0.08f, 0.05f, 0.03f, 0.90f);
                    sh.effectDistance = new Vector2(1.5f, -1.5f);
                    EditorUtility.SetDirty(tmp);
                }
            }
        }

        private static void UpgradeDayTransitionScreen(GameObject root, Sprite darkWood, TMP_FontAsset boldFont, TMP_FontAsset storyFont)
        {
            Transform curtain = root.transform.Find("DayTransitionCanvas/FadeCurtain");
            if (curtain == null) return;

            var cImg = curtain.GetComponent<Image>();
            if (cImg != null) cImg.color = new Color(0.05f, 0.07f, 0.13f, 0.96f);

            Transform cardTf = curtain.Find("TransitionCard");
            GameObject cardGo = cardTf != null
                ? cardTf.gameObject
                : new GameObject("TransitionCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            if (cardTf == null) cardGo.transform.SetParent(curtain, false);

            var cr = cardGo.GetComponent<RectTransform>();
            cr.anchorMin = new Vector2(0.5f, 0.5f);
            cr.anchorMax = new Vector2(0.5f, 0.5f);
            cr.pivot = new Vector2(0.5f, 0.5f);
            cr.anchoredPosition = Vector2.zero;
            cr.sizeDelta = new Vector2(860f, 290f);
            cr.localScale = Vector3.one;

            var cardImg = cardGo.GetComponent<Image>();
            if (darkWood != null)
            {
                cardImg.sprite = darkWood;
                cardImg.type = Image.Type.Sliced;
                cardImg.pixelsPerUnitMultiplier = 0.45f;
                cardImg.color = Color.white;
            }
            else
            {
                cardImg.color = new Color(0.14f, 0.10f, 0.08f, 0.96f);
            }

            Transform titleTf = cardGo.transform.Find("TransitionTitle");
            GameObject titleGo = titleTf != null ? titleTf.gameObject : new GameObject("TransitionTitle", typeof(RectTransform), typeof(TextMeshProUGUI));
            if (titleTf == null) titleGo.transform.SetParent(cardGo.transform, false);

            var tr = titleGo.GetComponent<RectTransform>();
            tr.anchorMin = new Vector2(0f, 1f);
            tr.anchorMax = new Vector2(1f, 1f);
            tr.pivot = new Vector2(0.5f, 1f);
            tr.anchoredPosition = new Vector2(0f, -34f);
            tr.sizeDelta = new Vector2(-80f, 48f);

            var titleTmp = titleGo.GetComponent<TextMeshProUGUI>();
            if (boldFont != null) titleTmp.font = boldFont;
            titleTmp.fontSize = 30f;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = new Color32(252, 224, 146, 255);
            titleTmp.text = "NGHỈ NGƠI TRONG NHÀ KÍNH";

            Transform msgTf = curtain.Find("TransitionText") ?? cardGo.transform.Find("TransitionText");
            TMP_Text msgTmp = null;
            if (msgTf != null)
            {
                msgTf.SetParent(cardGo.transform, false);
                var mr = msgTf.GetComponent<RectTransform>();
                mr.anchorMin = Vector2.zero;
                mr.anchorMax = Vector2.one;
                mr.offsetMin = new Vector2(56f, 36f);
                mr.offsetMax = new Vector2(-56f, -96f);

                msgTmp = msgTf.GetComponent<TMP_Text>();
                if (msgTmp != null)
                {
                    if (storyFont != null) msgTmp.font = storyFont;
                    msgTmp.fontSize = 24f;
                    msgTmp.lineSpacing = 12f;
                    msgTmp.alignment = TextAlignmentOptions.Center;
                    msgTmp.color = new Color32(252, 244, 224, 255);
                    EditorUtility.SetDirty(msgTmp);
                }
            }

            var sleepCtrl = root.GetComponent<SleepAndKnockoutController>();
            if (sleepCtrl != null)
            {
                var so = new SerializedObject(sleepCtrl);
                so.FindProperty("transitionTitleText").objectReferenceValue = titleTmp;
                if (msgTmp != null) so.FindProperty("transitionText").objectReferenceValue = msgTmp;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(sleepCtrl);
            }
        }

        [MenuItem("Moonpetal/Step 31/2. Validate Step 31 UI Polish")]
        public static void ValidateStep31()
        {
            int errors = 0;

            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(StoryFontPath) == null)
            {
                Debug.LogError("[MV][Step31][FAIL] Thieu " + StoryFontPath);
                errors++;
            }
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(HudCleanFontPath) == null)
            {
                Debug.LogError("[MV][Step31][FAIL] Thieu " + HudCleanFontPath);
                errors++;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                Transform bar = FindInteractionPromptBar(prefab);
                if (bar == null || bar.GetComponent<RectTransform>().anchoredPosition.y < 150f)
                {
                    Debug.LogError("[MV][Step31][FAIL] InteractionPromptBar chua duoc tach thoang (y < 150).");
                    errors++;
                }

                if (prefab.transform.Find("ClockHUDCanvas/ExhaustedWarningCard") == null)
                {
                    Debug.LogError("[MV][Step31][FAIL] Thieu ExhaustedWarningCard ben duoi StatsPanel.");
                    errors++;
                }

                if (prefab.transform.Find("DayTransitionCanvas/FadeCurtain/TransitionCard/TransitionTitle") == null)
                {
                    Debug.LogError("[MV][Step31][FAIL] Thieu TransitionCard / TransitionTitle trong DayTransitionCanvas.");
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV][Step31] VALIDATE PASS: VN_StoryFont, VN_HudCleanFont, ExhaustedWarningCard, Prompt Spacing & Knockout Card deu hop le 100%!");
            }
            else
            {
                Debug.LogError("[MV][Step31] VALIDATE FAIL (" + errors + " loi).");
            }
        }
    }
}
