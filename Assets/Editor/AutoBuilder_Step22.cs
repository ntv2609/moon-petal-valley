using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step22
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string LogoPath = "Assets/Art/Sprites/UI/ui_mainmenu_logo.png";
        private const string BtnWoodPath = "Assets/Art/Sprites/UI/ui_button_wood.png";
        private const string MenuFontAssetPath = "Assets/Art/Fonts/VN_MenuFont.asset";
        private const string MenuFontTtfCopy = "Assets/Art/Fonts/VN_MenuSerifBold.ttf";

        [MenuItem("Moonpetal/Step 22/1. Apply Stardew-Style Logo, Clean Main Menu & Serif Font")]
        public static void BuildStep22()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU STEP 22 (LOGO MOONPETAL VALLEY & CLEAN MAIN MENU) ===");
            AssetDatabase.Refresh();

            ImportLogoSprite();
            TMP_FontAsset menuFont = EnsureCozySerifMenuFont();
            RebuildCleanMainMenu(menuFont);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 22! Mở MainMenu và bấm Play để chiêm ngưỡng nhé! ===");
        }

        private static void ImportLogoSprite()
        {
            var imp = AssetImporter.GetAtPath(LogoPath) as TextureImporter;
            if (imp == null)
            {
                Debug.LogError("[MV] Không tìm thấy logo tại: " + LogoPath);
                return;
            }
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.filterMode = FilterMode.Bilinear;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.alphaIsTransparency = true;
            imp.spritePixelsPerUnit = 100f;
            var st = new TextureImporterSettings();
            imp.ReadTextureSettings(st);
            st.spriteMeshType = SpriteMeshType.FullRect;
            st.spriteAlignment = (int)SpriteAlignment.Center;
            st.spritePivot = new Vector2(0.5f, 0.5f);
            imp.SetTextureSettings(st);
            imp.SaveAndReimport();
        }

        private static TMP_FontAsset EnsureCozySerifMenuFont()
        {
            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(MenuFontAssetPath);
            if (existing != null) return existing;

            string[] candidates =
            {
                "C:/Windows/Fonts/georgiab.ttf",
                "C:/Windows/Fonts/georgia.ttf",
                "C:/Windows/Fonts/timesbd.ttf",
                "C:/Windows/Fonts/palab.ttf",
                "C:/Windows/Fonts/segoeuib.ttf",
            };

            Font srcFont = null;
            foreach (string sysPath in candidates)
            {
                if (File.Exists(sysPath))
                {
                    File.Copy(sysPath, MenuFontTtfCopy, true);
                    AssetDatabase.ImportAsset(MenuFontTtfCopy, ImportAssetOptions.ForceUpdate);
                    srcFont = AssetDatabase.LoadAssetAtPath<Font>(MenuFontTtfCopy);
                    if (srcFont != null)
                    {
                        Debug.Log("[MV] Đã nạp font Serif ấm áp cho Main Menu: " + sysPath);
                        break;
                    }
                }
            }

            if (srcFont == null)
            {
                return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_DynamicFont.asset");
            }

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(srcFont);
            if (fontAsset == null)
            {
                return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_DynamicFont.asset");
            }

            fontAsset.name = "VN_MenuFont";
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;

            AssetDatabase.CreateAsset(fontAsset, MenuFontAssetPath);
            if (fontAsset.material != null)
            {
                fontAsset.material.name = "VN_MenuFont Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, MenuFontAssetPath);
            }
            if (fontAsset.atlasTexture != null)
            {
                fontAsset.atlasTexture.name = "VN_MenuFont Atlas";
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, MenuFontAssetPath);
            }
            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            return fontAsset;
        }

        private static void RebuildCleanMainMenu(TMP_FontAsset menuFont)
        {
            if (!File.Exists(MainMenuScenePath))
            {
                Debug.LogError("[MV] Không tìm thấy " + MainMenuScenePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            Sprite logoSpr = AssetDatabase.LoadAssetAtPath<Sprite>(LogoPath);
            Sprite btnWood = AssetDatabase.LoadAssetAtPath<Sprite>(BtnWoodPath);

            GameObject canvasGo = GameObject.Find("MainMenuCanvas");
            if (canvasGo == null)
            {
                Debug.LogError("[MV] Không tìm thấy MainMenuCanvas.");
                return;
            }

            // 1. Create/Update floating Stardew-style MOONPETAL VALLEY Logo in upper twilight sky
            Transform oldLogo = canvasGo.transform.Find("MainMenuLogo");
            if (oldLogo != null) UnityEngine.Object.DestroyImmediate(oldLogo.gameObject);

            if (logoSpr != null)
            {
                GameObject logoGo = new GameObject("MainMenuLogo");
                logoGo.transform.SetParent(canvasGo.transform, false);
                var lr = logoGo.AddComponent<RectTransform>();
                lr.anchorMin = new Vector2(0.5f, 0.5f);
                lr.anchorMax = new Vector2(0.5f, 0.5f);
                lr.pivot = new Vector2(0.5f, 0.5f);
                lr.anchoredPosition = new Vector2(0f, 200f);
                lr.sizeDelta = new Vector2(820f, 360f);

                var lImg = logoGo.AddComponent<Image>();
                lImg.sprite = logoSpr;
                lImg.preserveAspect = true;
                lImg.raycastTarget = false;
                lImg.color = Color.white;
            }

            // 2. Clean MenuCard: remove dark box & remove Title, Subtitle, SaveHint!
            GameObject card = GameObject.Find("MenuCard");
            if (card != null)
            {
                // Make MenuCard completely transparent so the valley pond & dawn sky shine through!
                var cardImg = card.GetComponent<Image>();
                if (cardImg != null)
                {
                    cardImg.sprite = null;
                    cardImg.color = new Color(0f, 0f, 0f, 0f);
                    cardImg.raycastTarget = false;
                }

                var cr = card.GetComponent<RectTransform>();
                cr.anchorMin = new Vector2(0.5f, 0.5f);
                cr.anchorMax = new Vector2(0.5f, 0.5f);
                cr.anchoredPosition = new Vector2(0f, -210f);
                cr.sizeDelta = new Vector2(400f, 240f);

                var vlg = card.GetComponent<VerticalLayoutGroup>();
                if (vlg != null)
                {
                    vlg.padding = new RectOffset(10, 10, 6, 6);
                    vlg.spacing = 16f;
                    vlg.childAlignment = TextAnchor.MiddleCenter;
                }

                // Destroy unwanted text lines
                foreach (string unwanted in new[] { "Title", "Subtitle", "SaveHint" })
                {
                    Transform t = card.transform.Find(unwanted);
                    if (t != null) UnityEngine.Object.DestroyImmediate(t.gameObject);
                }

                // Clear continueSubtext reference on MainMenuController
                var ctrl = canvasGo.GetComponent<MainMenuController>();
                if (ctrl != null)
                {
                    var so = new SerializedObject(ctrl);
                    var prop = so.FindProperty("continueSubtext");
                    if (prop != null) prop.objectReferenceValue = null;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }

                // Style the 3 cozy wooden buttons with warm serif font & subtle drop shadow!
                StyleMenuButton(card.transform.Find("BtnNewGame"),  btnWood, new Color(0.86f, 1.00f, 0.88f, 1f), menuFont, 64f);
                StyleMenuButton(card.transform.Find("BtnContinue"), btnWood, new Color(0.90f, 0.96f, 1.00f, 1f), menuFont, 64f);
                StyleMenuButton(card.transform.Find("BtnQuit"),     btnWood, new Color(1.00f, 0.86f, 0.84f, 1f), menuFont, 60f);

                // Ensure Logo renders above Backdrop & beside MenuCard
                card.transform.SetAsLastSibling();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV] Đã làm sạch Main Menu: Logo MOONPETAL VALLEY + 3 nút gỗ chữ Serif ấm áp.");
        }

        private static void StyleMenuButton(Transform btnTf, Sprite btnWood, Color tint, TMP_FontAsset font, float height)
        {
            if (btnTf == null) return;
            var r = btnTf.GetComponent<RectTransform>();
            if (r != null) r.sizeDelta = new Vector2(360f, height);

            var img = btnTf.GetComponent<Image>();
            if (img != null && btnWood != null)
            {
                img.sprite = btnWood;
                img.type = Image.Type.Sliced;
                img.pixelsPerUnitMultiplier = 0.45f;
                img.color = tint;
            }

            Transform lbl = btnTf.Find("Label");
            if (lbl != null)
            {
                var tmp = lbl.GetComponent<TMP_Text>();
                if (tmp != null)
                {
                    if (font != null) tmp.font = font;
                    tmp.fontSize = 26;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.color = new Color(1.0f, 0.97f, 0.88f, 1f);
                    tmp.alignment = TextAlignmentOptions.Center;

                    // Crisp dark brown outline + shadow for storybook readability
                    var shadow = lbl.GetComponent<Shadow>();
                    if (shadow == null) shadow = lbl.gameObject.AddComponent<Shadow>();
                    shadow.effectColor = new Color(0.18f, 0.10f, 0.06f, 0.92f);
                    shadow.effectDistance = new Vector2(2f, -2f);
                }
            }
        }
    }
}
