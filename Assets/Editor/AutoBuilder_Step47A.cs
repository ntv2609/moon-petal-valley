#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step47A
    {
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        private const string PropDir = "Assets/Art/Sprites/Chapter2";
        private const string PropShavingsPath = PropDir + "/prop_clue_wood_shavings.png";
        private const string PropFootprintsPath = PropDir + "/prop_clue_mud_footprints.png";
        private const string PropNotebookPath = PropDir + "/prop_clue_otis_notebook.png";
        private const string PropCutCablePath = PropDir + "/prop_echo_cut_cable.png";
        private const string PropWorkbenchPath = PropDir + "/prop_relay_workbench.png";
        private const string LanternBrokenWestPath = "Assets/Art/Characters/actor_storm_lantern/actor_storm_lantern_broken_west_00.png";
        private const string PulseRingPath = "Assets/Art/Sprites/VFX/spr_pulse_ring.png";

        private const string DarkWoodPath = "Assets/Art/Sprites/UI/ui_panel_dark_wood.png";
        private const string BtnWoodPath = "Assets/Art/Sprites/UI/ui_button_wood.png";
        private const string BarFramePath = "Assets/Art/Sprites/UI/ui_bar_frame.png";
        private const string PetalIconPath = "Assets/Art/Sprites/Items/icon_item_petal_of_bond.png";

        private const string MenuWarmFontPath = "Assets/Art/Fonts/VN_MenuWarmFont.asset";
        private const string StoryFontPath = "Assets/Art/Fonts/VN_StoryFont.asset";
        private const string HudFontPath = "Assets/Art/Fonts/VN_HudCleanFont.asset";

        [MenuItem("Moonpetal/Step 47A/1. Build Chapter 2 Props & UI Art Polish")]
        public static void BuildStep47A()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Step47A] Không thể chạy khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV][Step47A] === BẮT ĐẦU STEP 47A: BỔ SUNG PROP CHƯƠNG 2 & ĐỒNG BỘ ART UI ===");

            // 1. Cấu hình TextureImporter (PPU=16, Point, Uncompressed, FullRect) cho 5 sprite mới
            ConfigurePropSprite(PropShavingsPath);
            ConfigurePropSprite(PropFootprintsPath);
            ConfigurePropSprite(PropNotebookPath);
            ConfigurePropSprite(PropCutCablePath);
            ConfigurePropSprite(PropWorkbenchPath);
            AssetDatabase.Refresh();

            Sprite sprShavings = AssetDatabase.LoadAssetAtPath<Sprite>(PropShavingsPath);
            Sprite sprFootprints = AssetDatabase.LoadAssetAtPath<Sprite>(PropFootprintsPath);
            Sprite sprNotebook = AssetDatabase.LoadAssetAtPath<Sprite>(PropNotebookPath);
            Sprite sprCutCable = AssetDatabase.LoadAssetAtPath<Sprite>(PropCutCablePath);
            Sprite sprWorkbench = AssetDatabase.LoadAssetAtPath<Sprite>(PropWorkbenchPath);
            Sprite sprLanternWest = AssetDatabase.LoadAssetAtPath<Sprite>(LanternBrokenWestPath);
            Sprite sprPulseRing = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);

            Sprite sprDarkWood = AssetDatabase.LoadAssetAtPath<Sprite>(DarkWoodPath);
            Sprite sprBtnWood = AssetDatabase.LoadAssetAtPath<Sprite>(BtnWoodPath);
            Sprite sprBarFrame = AssetDatabase.LoadAssetAtPath<Sprite>(BarFramePath);
            Sprite sprPetalIcon = AssetDatabase.LoadAssetAtPath<Sprite>(PetalIconPath);

            TMP_FontAsset menuFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(MenuWarmFontPath);
            TMP_FontAsset storyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(StoryFontPath);
            TMP_FontAsset hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(HudFontPath);

            // 2. Mở scene Riverbank_Chapter2.unity và cập nhật Props + Scene UIs
            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Step47A] Không mở được scene " + Ch2ScenePath);
                return;
            }

            ApplyClueSprite("Clue_WoodShavings", sprShavings, false);
            ApplyClueSprite("Clue_MudFootprints", sprFootprints, true);
            ApplyClueSprite("Clue_OtisNote", sprNotebook, false);

            ApplyEchoProp("EchoSpot_StormLantern", sprLanternWest, 1.0f);
            ApplyEchoProp("EchoSpot_BrokenPier", sprCutCable, 1.0f);

            ApplyRelayStationProp("RelayStation_East", sprWorkbench, sprPulseRing);
            ApplyRelayStationProp("RelayStation_West", sprWorkbench, sprPulseRing);

            WireLanternRelayHUDView(sprDarkWood, sprBarFrame, menuFont, hudFont);
            WireChapter2EndingStyler(sprDarkWood, sprBtnWood, sprPetalIcon, menuFont, storyFont, hudFont);
            SkinCh2IntroCanvas(sprDarkWood, sprBarFrame, menuFont, storyFont, hudFont);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            // 3. Nâng cấp EchoOverlayCanvas trong GameRoot.prefab
            SkinEchoOverlayInGameRoot(sprDarkWood, sprBarFrame, menuFont, storyFont, hudFont);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Step47A] === HOÀN TẤT STEP 47A! Chạy 'Moonpetal/Step 47A/2. Validate Step 47A' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 47A/2. Validate Step 47A")]
        public static void ValidateStep47A()
        {
            int errors = 0;
            Debug.Log("[MV][Step47A] === BẮT ĐẦU VALIDATE STEP 47A ===");

            string[] props = { PropShavingsPath, PropFootprintsPath, PropNotebookPath, PropCutCablePath, PropWorkbenchPath };
            for (int i = 0; i < props.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<Sprite>(props[i]) == null)
                {
                    Debug.LogError("[MV][Step47A][FAIL] Thiếu sprite: " + props[i]);
                    errors++;
                }
            }

            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV][Step47A][FAIL] Không mở được " + Ch2ScenePath);
                return;
            }

            errors += CheckSpriteAssigned("Clue_WoodShavings", PropShavingsPath);
            errors += CheckSpriteAssigned("Clue_MudFootprints", PropFootprintsPath);
            errors += CheckSpriteAssigned("Clue_OtisNote", PropNotebookPath);
            errors += CheckChildProp("EchoSpot_StormLantern", "PropVisual");
            errors += CheckChildProp("EchoSpot_BrokenPier", "PropVisual");
            errors += CheckSpriteAssigned("RelayStation_East", PropWorkbenchPath);
            errors += CheckSpriteAssigned("RelayStation_West", PropWorkbenchPath);

            LanternRelayHUDView hud = Object.FindAnyObjectByType<LanternRelayHUDView>();
            if (hud == null) { Debug.LogError("[MV][Step47A][FAIL] Thiếu LanternRelayHUDView."); errors++; }
            else
            {
                var so = new SerializedObject(hud);
                if (so.FindProperty("panelDarkWoodSprite").objectReferenceValue == null)
                { Debug.LogError("[MV][Step47A][FAIL] LanternRelayHUDView chưa gắn panelDarkWoodSprite."); errors++; }
            }

            Chapter2EndingUIStyler styler = Object.FindAnyObjectByType<Chapter2EndingUIStyler>();
            if (styler == null) { Debug.LogError("[MV][Step47A][FAIL] Thiếu Chapter2EndingUIStyler."); errors++; }

            GameObject introCanvas = GameObject.Find("Ch2IntroCanvas");
            if (introCanvas == null) { Debug.LogError("[MV][Step47A][FAIL] Thiếu Ch2IntroCanvas."); errors++; }
            else
            {
                Transform box = introCanvas.transform.Find("SubtitleBox");
                Image img = box != null ? box.GetComponent<Image>() : null;
                if (img == null || img.sprite == null)
                { Debug.LogError("[MV][Step47A][FAIL] Ch2IntroCanvas/SubtitleBox chưa gắn sprite 9-slice."); errors++; }
            }

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root != null)
            {
                Transform echoBox = root.transform.Find("EchoOverlayCanvas/EchoSubtitleBox");
                Image eImg = echoBox != null ? echoBox.GetComponent<Image>() : null;
                if (eImg == null || eImg.sprite == null)
                { Debug.LogError("[MV][Step47A][FAIL] GameRoot/EchoOverlayCanvas/EchoSubtitleBox chưa gắn sprite 9-slice."); errors++; }
                PrefabUtility.UnloadPrefabContents(root);
            }
            else
            {
                Debug.LogError("[MV][Step47A][FAIL] Không mở được GameRoot.prefab.");
                errors++;
            }

            if (errors == 0)
                Debug.Log("[MV][Step47A] VALIDATE PASS (0 lỗi) — 5 prop mới, 2 Echo, 2 Trạm Relay & 4 UI Chương 2 đã đồng bộ!");
            else
                Debug.LogError($"[MV][Step47A] VALIDATE FAIL ({errors} lỗi).");
        }

        private static void ConfigurePropSprite(string assetPath)
        {
            if (!File.Exists(assetPath))
            {
                Debug.LogError("[MV][Step47A] Không tìm thấy file ảnh: " + assetPath);
                return;
            }
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            TextureImporter imp = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (imp == null) return;

            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.spritePixelsPerUnit = 16f;
            imp.filterMode = FilterMode.Point;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.alphaIsTransparency = true;

            TextureImporterSettings settings = new TextureImporterSettings();
            imp.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            imp.SetTextureSettings(settings);
            imp.SaveAndReimport();
        }

        private static void ApplyClueSprite(string objName, Sprite spr, bool isGroundDecal)
        {
            GameObject go = GameObject.Find(objName);
            if (go == null || spr == null) return;

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color = Color.white;

            if (isGroundDecal)
            {
                sr.sortingLayerName = "GroundDecor";
                sr.sortingOrder = 15;
                YSortRenderer ysort = go.GetComponent<YSortRenderer>();
                if (ysort != null) Object.DestroyImmediate(ysort);
            }
            else
            {
                sr.sortingLayerName = "WorldObjects";
                YSortRenderer ysort = go.GetComponent<YSortRenderer>();
                if (ysort == null) ysort = go.AddComponent<YSortRenderer>();
                ysort.Configure(sr, false, true, 0.15f, 0);
            }
            EditorUtility.SetDirty(go);
        }

        private static void ApplyEchoProp(string echoObjName, Sprite propSpr, float scale)
        {
            GameObject go = GameObject.Find(echoObjName);
            if (go == null || propSpr == null) return;

            Transform tf = go.transform.Find("PropVisual");
            GameObject propGo = tf != null ? tf.gameObject : new GameObject("PropVisual");
            propGo.transform.SetParent(go.transform, false);
            propGo.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            propGo.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer sr = propGo.GetComponent<SpriteRenderer>();
            if (sr == null) sr = propGo.AddComponent<SpriteRenderer>();
            sr.sprite = propSpr;
            sr.color = Color.white;
            sr.sortingLayerName = "WorldObjects";

            YSortRenderer ysort = propGo.GetComponent<YSortRenderer>();
            if (ysort == null) ysort = propGo.AddComponent<YSortRenderer>();
            ysort.Configure(sr, false, true, 0.15f, 0);

            EditorUtility.SetDirty(go);
        }

        private static void ApplyRelayStationProp(string stationName, Sprite benchSpr, Sprite pulseSpr)
        {
            GameObject go = GameObject.Find(stationName);
            if (go == null || benchSpr == null) return;

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = benchSpr;
            sr.color = Color.white;
            sr.sortingLayerName = "WorldObjects";

            YSortRenderer ysort = go.GetComponent<YSortRenderer>();
            if (ysort == null) ysort = go.AddComponent<YSortRenderer>();
            ysort.Configure(sr, false, true, 0.15f, 0);

            Transform hlTf = go.transform.Find("ReadyHighlight");
            GameObject hlGo = hlTf != null ? hlTf.gameObject : new GameObject("ReadyHighlight");
            hlGo.transform.SetParent(go.transform, false);
            hlGo.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            hlGo.transform.localScale = new Vector3(1.15f, 1.15f, 1f);

            SpriteRenderer hlSr = hlGo.GetComponent<SpriteRenderer>();
            if (hlSr == null) hlSr = hlGo.AddComponent<SpriteRenderer>();
            hlSr.sprite = pulseSpr;
            hlSr.color = new Color(1f, 0.88f, 0.42f, 0.45f);
            hlSr.sortingLayerName = "GroundDecor";
            hlSr.sortingOrder = 18;
            hlSr.enabled = false;

            LanternRelayStation st = go.GetComponent<LanternRelayStation>();
            if (st != null)
            {
                SerializedObject so = new SerializedObject(st);
                SerializedProperty p = so.FindProperty("readyHighlightRenderer");
                if (p != null) p.objectReferenceValue = hlSr;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(go);
        }

        private static void WireLanternRelayHUDView(Sprite darkWood, Sprite barFrame, TMP_FontAsset menuFont, TMP_FontAsset hudFont)
        {
            LanternRelayHUDView hud = Object.FindAnyObjectByType<LanternRelayHUDView>();
            if (hud == null) return;
            SerializedObject so = new SerializedObject(hud);
            SetObj(so, "panelDarkWoodSprite", darkWood);
            SetObj(so, "barFrameSprite", barFrame);
            SetObj(so, "menuWarmFont", menuFont);
            SetObj(so, "hudCleanFont", hudFont);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(hud);
        }

        private static void WireChapter2EndingStyler(Sprite darkWood, Sprite btnWood, Sprite petalIcon, TMP_FontAsset menuFont, TMP_FontAsset storyFont, TMP_FontAsset hudFont)
        {
            Chapter2EndingController endCtrl = Object.FindAnyObjectByType<Chapter2EndingController>();
            if (endCtrl == null) return;

            Chapter2EndingUIStyler styler = endCtrl.GetComponent<Chapter2EndingUIStyler>();
            if (styler == null) styler = endCtrl.gameObject.AddComponent<Chapter2EndingUIStyler>();

            SerializedObject so = new SerializedObject(styler);
            SetObj(so, "panelDarkWoodSprite", darkWood);
            SetObj(so, "buttonWoodSprite", btnWood);
            SetObj(so, "petalIconSprite", petalIcon);
            SetObj(so, "menuWarmFont", menuFont);
            SetObj(so, "storyFont", storyFont);
            SetObj(so, "hudCleanFont", hudFont);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(endCtrl.gameObject);
        }

        private static void SkinCh2IntroCanvas(Sprite darkWood, Sprite barFrame, TMP_FontAsset menuFont, TMP_FontAsset storyFont, TMP_FontAsset hudFont)
        {
            GameObject canvasGo = GameObject.Find("Ch2IntroCanvas");
            if (canvasGo == null) return;

            Transform boxTf = canvasGo.transform.Find("SubtitleBox");
            if (boxTf != null)
            {
                RectTransform br = boxTf.GetComponent<RectTransform>();
                br.sizeDelta = new Vector2(0f, 126f);
                br.anchoredPosition = new Vector2(0f, 22f);

                Image img = boxTf.GetComponent<Image>();
                if (img != null && darkWood != null)
                {
                    img.sprite = darkWood;
                    img.type = Image.Type.Sliced;
                    img.pixelsPerUnitMultiplier = 0.48f;
                    img.color = Color.white;
                }

                Transform spTf = boxTf.Find("Speaker");
                if (spTf != null)
                {
                    TMP_Text sp = spTf.GetComponent<TMP_Text>();
                    if (sp != null)
                    {
                        if (menuFont != null) sp.font = menuFont;
                        sp.fontSize = 19f;
                        sp.color = new Color32(252, 224, 146, 255);
                    }
                }

                Transform bdTf = boxTf.Find("Body");
                if (bdTf != null)
                {
                    TMP_Text bd = bdTf.GetComponent<TMP_Text>();
                    if (bd != null)
                    {
                        if (storyFont != null) bd.font = storyFont;
                        bd.fontSize = 19f;
                        bd.color = new Color32(248, 240, 220, 255);
                    }
                }

                EnsureHintLabel(boxTf, hudFont);
            }

            SkinSkipBar(canvasGo.transform.Find("SkipBg"), barFrame);
            EditorUtility.SetDirty(canvasGo);
        }

        private static void SkinEchoOverlayInGameRoot(Sprite darkWood, Sprite barFrame, TMP_FontAsset menuFont, TMP_FontAsset storyFont, TMP_FontAsset hudFont)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Transform canvasTf = root.transform.Find("EchoOverlayCanvas");
            if (canvasTf != null)
            {
                Transform titleTf = canvasTf.Find("EchoTitle");
                if (titleTf != null)
                {
                    TMP_Text titleTmp = titleTf.GetComponent<TMP_Text>();
                    if (titleTmp != null)
                    {
                        if (menuFont != null) titleTmp.font = menuFont;
                        titleTmp.fontSize = 20f;
                        titleTmp.color = new Color32(246, 232, 184, 255);
                    }
                }

                Transform boxTf = canvasTf.Find("EchoSubtitleBox");
                if (boxTf != null)
                {
                    RectTransform br = boxTf.GetComponent<RectTransform>();
                    br.sizeDelta = new Vector2(0f, 128f);
                    br.anchoredPosition = new Vector2(0f, 22f);

                    Image img = boxTf.GetComponent<Image>();
                    if (img != null && darkWood != null)
                    {
                        img.sprite = darkWood;
                        img.type = Image.Type.Sliced;
                        img.pixelsPerUnitMultiplier = 0.48f;
                        img.color = Color.white;
                    }

                    Transform spTf = boxTf.Find("Speaker");
                    if (spTf != null)
                    {
                        TMP_Text sp = spTf.GetComponent<TMP_Text>();
                        if (sp != null)
                        {
                            if (menuFont != null) sp.font = menuFont;
                            sp.fontSize = 19f;
                            sp.color = new Color32(252, 224, 146, 255);
                        }
                    }

                    Transform bdTf = boxTf.Find("Body");
                    if (bdTf != null)
                    {
                        TMP_Text bd = bdTf.GetComponent<TMP_Text>();
                        if (bd != null)
                        {
                            if (storyFont != null) bd.font = storyFont;
                            bd.fontSize = 19f;
                            bd.color = new Color32(248, 240, 220, 255);
                        }
                    }

                    EnsureHintLabel(boxTf, hudFont);
                }

                SkinSkipBar(canvasTf.Find("SkipBg"), barFrame);
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void EnsureHintLabel(Transform boxTf, TMP_FontAsset hudFont)
        {
            Transform hTf = boxTf.Find("ContinueHint");
            GameObject hGo = hTf != null ? hTf.gameObject : new GameObject("ContinueHint", typeof(RectTransform), typeof(TextMeshProUGUI));
            hGo.transform.SetParent(boxTf, false);
            RectTransform rt = hGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 8f);
            rt.sizeDelta = new Vector2(-36f, 18f);

            TMP_Text tmp = hGo.GetComponent<TMP_Text>();
            if (hudFont != null) tmp.font = hudFont;
            tmp.fontSize = 12f;
            tmp.alignment = TextAlignmentOptions.Right;
            tmp.color = new Color32(216, 200, 166, 210);
            tmp.text = "[Space / Click] Tiếp tục  |  Giữ [Esc] Bỏ qua";
        }

        private static void SkinSkipBar(Transform skipBgTf, Sprite barFrame)
        {
            if (skipBgTf == null || barFrame == null) return;
            Image img = skipBgTf.GetComponent<Image>();
            if (img == null) return;
            img.sprite = barFrame;
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = 0.55f;
            img.color = new Color(0.86f, 0.80f, 0.72f, 0.92f);
        }

        private static void SetObj(SerializedObject so, string propName, Object val)
        {
            SerializedProperty p = so.FindProperty(propName);
            if (p != null && p.propertyType == SerializedPropertyType.ObjectReference)
                p.objectReferenceValue = val;
        }

        private static int CheckSpriteAssigned(string objName, string expectedAssetPath)
        {
            GameObject go = GameObject.Find(objName);
            SpriteRenderer sr = go != null ? go.GetComponent<SpriteRenderer>() : null;
            if (sr == null || sr.sprite == null)
            {
                Debug.LogError($"[MV][Step47A][FAIL] {objName} chưa có SpriteRenderer/Sprite.");
                return 1;
            }
            string actual = AssetDatabase.GetAssetPath(sr.sprite);
            if (actual != expectedAssetPath)
            {
                Debug.LogError($"[MV][Step47A][FAIL] {objName} đang dùng '{actual}', cần '{expectedAssetPath}'.");
                return 1;
            }
            return 0;
        }

        private static int CheckChildProp(string parentName, string childName)
        {
            GameObject parent = GameObject.Find(parentName);
            Transform child = parent != null ? parent.transform.Find(childName) : null;
            SpriteRenderer sr = child != null ? child.GetComponent<SpriteRenderer>() : null;
            if (sr == null || sr.sprite == null)
            {
                Debug.LogError($"[MV][Step47A][FAIL] {parentName}/{childName} chưa có SpriteRenderer/Sprite.");
                return 1;
            }
            return 0;
        }
    }
}
#endif
