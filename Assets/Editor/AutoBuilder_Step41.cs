using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step41
    {
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string ItemDatabasePath = "Assets/Data/Config/ItemDatabase.asset";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string RelayConfigPath = "Assets/Data/Config/RelayLanternConfig.asset";
        private const string Echo1Path = "Assets/Data/Chapter2/Echo_Ch2_StormLantern.asset";
        private const string Echo2Path = "Assets/Data/Chapter2/Echo_Ch2_BrokenPier.asset";
        private const string KaelGhostPrefabPath = "Assets/Prefabs/Chapter2/Echo_KaelYoung.prefab";
        private const string MaraGhostPrefabPath = "Assets/Prefabs/Chapter2/Echo_MaraYoung.prefab";
        private const string StormLanternPrefabPath = "Assets/Prefabs/Chapter2/Actor_StormLantern.prefab";

        [MenuItem("Moonpetal/Step 41/1. Build Chapter 2 Foundation, Echo & Actors")]
        public static void BuildStep41()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 41 (CHƯƠNG 2 FOUNDATION + 3 ACTOR FINAL) ===");
            EnsureFolders();
            AssetDatabase.Refresh();

            if (!VerifyRequiredArtExists())
            {
                return;
            }

            ReimportChapter2Art();
            BuildChapter2Items();
            EnsureRelayLanternConfig();
            BuildTwoChapter2EchoAssets();

            GameObject kaelGhost = BuildEchoGhostPrefab(
                KaelGhostPrefabPath, "Echo_KaelYoung",
                "Assets/Art/Characters/echo_kael_young/echo_kael_young_idle",
                "Assets/Art/Characters/echo_kael_young/echo_kael_young_chop",
                4, 4, 4f, 8f, new Color(0.76f, 0.94f, 1.0f, 0.84f));

            GameObject maraGhost = BuildEchoGhostPrefab(
                MaraGhostPrefabPath, "Echo_MaraYoung",
                "Assets/Art/Characters/echo_mara_young/echo_mara_young_idle",
                "Assets/Art/Characters/echo_mara_young/echo_mara_young_hold_lamp",
                4, 4, 4f, 5f, new Color(1.0f, 0.92f, 0.78f, 0.86f));

            BuildStormLanternPrefab();
            UpgradeGameRootForChapter2(kaelGhost, maraGhost);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 41! Hãy chạy 'Moonpetal/Step 41/Validate' và 'Moonpetal/Debug/Asset Audit'. ===");
        }

        [MenuItem("Moonpetal/Step 41/Validate")]
        public static void ValidateStep41()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 41 ===");

            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            if (db == null || db.allItems.Count < 20)
            {
                Debug.LogError("[MV] ItemDatabase thiếu vật phẩm Chương 2 (cần >= 20, hiện có " + (db == null ? 0 : db.allItems.Count) + ").");
                errors++;
            }

            if (AssetDatabase.LoadAssetAtPath<RelayLanternConfigSO>(RelayConfigPath) == null)
            {
                Debug.LogError("[MV] THIẾU RelayLanternConfig.asset.");
                errors++;
            }

            var e1 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo1Path);
            var e2 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo2Path);
            if (e1 == null || e1.beats.Count < 3) { Debug.LogError("[MV] Echo_Ch2_StormLantern.asset không hợp lệ."); errors++; }
            if (e2 == null || e2.beats.Count < 2) { Debug.LogError("[MV] Echo_Ch2_BrokenPier.asset không hợp lệ."); errors++; }

            var root = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (root == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab.");
                errors++;
            }
            else
            {
                if (root.GetComponent<Chapter2StateManager>() == null) { Debug.LogError("[MV] GameRoot thiếu Chapter2StateManager."); errors++; }
                var em = root.GetComponent<EchoManager>();
                if (em == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu EchoManager.");
                    errors++;
                }
                else
                {
                    var so = new SerializedObject(em);
                    string[] req = { "overlayGroup", "tintOverlay", "titleText", "speakerText", "bodyText", "kaelGhostPrefab", "maraGhostPrefab" };
                    foreach (string f in req)
                    {
                        if (so.FindProperty(f).objectReferenceValue == null)
                        {
                            Debug.LogError("[MV] EchoManager thiếu tham chiếu: " + f);
                            errors++;
                        }
                    }
                }
            }

            errors += RunAssetAuditInternal(false);

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 41: PASS (0 lỗi). Nền tảng Chương 2, Dư Âm & 3 Actor FINAL sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 41: FAIL (" + errors + " lỗi).");
            }
        }

        [MenuItem("Moonpetal/Debug/Asset Audit")]
        public static void RunAssetAuditMenu()
        {
            Debug.Log("[MV] === BẮT ĐẦU ASSET AUDIT (CHƯƠNG 2 ACTORS) ===");
            int reds = RunAssetAuditInternal(true);
            if (reds == 0)
            {
                Debug.Log("[MV] ASSET AUDIT: 0 ĐỎ. Toàn bộ Prefab Actor Chương 2 đã đạt chuẩn FINAL!");
            }
            else
            {
                Debug.LogError("[MV] ASSET AUDIT: PHÁT HIỆN " + reds + " MỤC ĐỎ! Xem chi tiết ở trên.");
            }
        }

        [MenuItem("Moonpetal/Debug/Preview Echo 1 (Play Mode)")]
        public static void DebugPreviewEcho1()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("[MV] Lệnh Preview Echo 1 chỉ chạy trong Play Mode.");
                return;
            }
            var e1 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo1Path);
            if (EchoManager.Instance != null && e1 != null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<MoonpetalValley.Player.PlayerController2D>();
                Vector3 pos = p != null ? p.transform.position + new Vector3(0f, 1.2f, 0f) : Vector3.zero;
                EchoManager.Instance.PlayEcho(e1, pos);
            }
        }

        private static int RunAssetAuditInternal(bool verbose)
        {
            int reds = 0;
            string[] actorPrefabs = { KaelGhostPrefabPath, MaraGhostPrefabPath, StormLanternPrefabPath };

            foreach (string path in actorPrefabs)
            {
                GameObject pf = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (pf == null)
                {
                    Debug.LogError("[MV][ĐỎ] Thiếu Prefab Actor: " + path);
                    reds++;
                    continue;
                }
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(pf) > 0)
                {
                    Debug.LogError("[MV][ĐỎ] Missing Script trên: " + path);
                    reds++;
                }
                var sr = pf.GetComponentInChildren<SpriteRenderer>(true);
                if (sr == null || sr.sprite == null)
                {
                    Debug.LogError("[MV][ĐỎ] Actor thiếu Sprite: " + path);
                    reds++;
                }
                else if (sr.sprite.name.StartsWith("PLACEHOLDER_"))
                {
                    Debug.LogError("[MV][ĐỎ] Actor vẫn dùng sprite tạm PLACEHOLDER_: " + path);
                    reds++;
                }

                var ghost = pf.GetComponent<EchoGhostActor>();
                if (ghost != null)
                {
                    var so = new SerializedObject(ghost);
                    if (so.FindProperty("idleFrames").arraySize < 4 || so.FindProperty("actionFrames").arraySize < 4)
                    {
                        Debug.LogError("[MV][ĐỎ] EchoGhostActor thiếu đủ 4 frame animation: " + path);
                        reds++;
                    }
                }

                var lantern = pf.GetComponent<StormLanternActor>();
                if (lantern != null)
                {
                    var so = new SerializedObject(lantern);
                    if (so.FindProperty("brokenEastFrames").arraySize < 2 ||
                        so.FindProperty("brokenWestFrames").arraySize < 2 ||
                        so.FindProperty("litFrames").arraySize < 4)
                    {
                        Debug.LogError("[MV][ĐỎ] StormLanternActor thiếu frame animation: " + path);
                        reds++;
                    }
                }

                if (verbose) Debug.Log("[MV][OK] Actor FINAL: " + path);
            }
            return reds;
        }

        private static void EnsureFolders()
        {
            string[] dirs =
            {
                "Assets/Data/Chapter2",
                "Assets/Prefabs/Chapter2",
                "Assets/Art/Characters/echo_kael_young",
                "Assets/Art/Characters/echo_mara_young",
                "Assets/Art/Characters/actor_storm_lantern"
            };
            foreach (string d in dirs)
            {
                if (!Directory.Exists(d)) Directory.CreateDirectory(d);
            }
        }

        private static bool VerifyRequiredArtExists()
        {
            var required = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                required.Add($"Assets/Art/Characters/echo_kael_young/echo_kael_young_idle_{i:D2}.png");
                required.Add($"Assets/Art/Characters/echo_kael_young/echo_kael_young_chop_{i:D2}.png");
                required.Add($"Assets/Art/Characters/echo_mara_young/echo_mara_young_idle_{i:D2}.png");
                required.Add($"Assets/Art/Characters/echo_mara_young/echo_mara_young_hold_lamp_{i:D2}.png");
                required.Add($"Assets/Art/Characters/actor_storm_lantern/actor_storm_lantern_lit_{i:D2}.png");
            }
            for (int i = 0; i < 2; i++)
            {
                required.Add($"Assets/Art/Characters/actor_storm_lantern/actor_storm_lantern_broken_east_{i:D2}.png");
                required.Add($"Assets/Art/Characters/actor_storm_lantern/actor_storm_lantern_broken_west_{i:D2}.png");
            }

            bool ok = true;
            foreach (string f in required)
            {
                if (!File.Exists(f))
                {
                    Debug.LogError("[MV] BLOCKED: thiếu " + f);
                    ok = false;
                }
            }
            return ok;
        }

        private static void ReimportChapter2Art()
        {
            ImportFolder("Assets/Art/Characters/echo_kael_young", 20f);
            ImportFolder("Assets/Art/Characters/echo_mara_young", 20f);
            ImportFolder("Assets/Art/Characters/actor_storm_lantern", 16f);

            string[] ch2Icons =
            {
                "Assets/Art/Sprites/Items/icon_item_storm_lantern_east.png",
                "Assets/Art/Sprites/Items/icon_item_storm_lantern_west.png",
                "Assets/Art/Sprites/Items/icon_item_carpenter_hammer.png",
                "Assets/Art/Sprites/Items/icon_item_iron_nails_box.png",
                "Assets/Art/Sprites/Items/icon_item_petal_of_bond.png"
            };
            foreach (string p in ch2Icons) ImportSingleSprite(p, 16f);
        }

        private static void ImportFolder(string folder, float ppu)
        {
            if (!Directory.Exists(folder)) return;
            foreach (string f in Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories))
            {
                ImportSingleSprite(f.Replace("\\", "/"), ppu);
            }
        }

        private static void ImportSingleSprite(string assetPath, float ppu)
        {
            var imp = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (imp == null) return;
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.filterMode = FilterMode.Point;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.alphaIsTransparency = true;
            imp.spritePixelsPerUnit = ppu;
            var st = new TextureImporterSettings();
            imp.ReadTextureSettings(st);
            st.spriteMeshType = SpriteMeshType.FullRect;
            st.spriteAlignment = (int)SpriteAlignment.Center;
            st.spritePivot = new Vector2(0.5f, 0.5f);
            imp.SetTextureSettings(st);
            imp.SaveAndReimport();
        }

        private static void BuildChapter2Items()
        {
            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            if (db == null || table == null) return;

            UpsertItem(db, table, "item_storm_lantern_east", "Nửa Đèn Bão (Khung Đồng)",
                "Khung đồng cổ Ông Kael vẫn giữ trong xưởng mộc từ Lễ Trao Hạt đầu tiên.",
                ItemCategory.KeyItem, 1, true);

            UpsertItem(db, table, "item_storm_lantern_west", "Nửa Đèn Bão (Kính Chắn Gió)",
                "Phần kính chắn gió vớt được ở Đầm Lầy Bờ Tây sau khi dọn Bọt Sương Khổng Lồ.",
                ItemCategory.KeyItem, 1, true);

            UpsertItem(db, table, "item_carpenter_hammer", "Búa Thợ Mộc Của Kael",
                "Chiếc búa đằm tay Ông Kael nhờ chuyền sang cho Bà Mara giữa Đêm Chuyền Đèn.",
                ItemCategory.KeyItem, 1, true);

            UpsertItem(db, table, "item_iron_nails_box", "Hộp Đinh Đồng",
                "Hộp đinh chống nước mặn Bà Mara gửi sang cho Ông Kael cố định kèo cầu.",
                ItemCategory.KeyItem, 1, true);

            UpsertItem(db, table, "item_petal_of_bond", "Cánh Hoa Gắn Kết",
                "Cánh hoa Moonpetal thứ hai nở khi cây cầu hai bờ sáng đèn trở lại.",
                ItemCategory.MoonpetalShard, 1, true);

            EditorUtility.SetDirty(db);
            EditorUtility.SetDirty(table);
        }

        private static void UpsertItem(ItemDatabaseSO db, StringTableSO table, string id, string vnName, string vnDesc, ItemCategory cat, int maxStack, bool questProtected)
        {
            string assetPath = "Assets/Data/Items/Item_" + id + ".asset";
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemData>();
                AssetDatabase.CreateAsset(item, assetPath);
            }
            item.itemID = id;
            item.displayNameKey = "name_" + id;
            item.descriptionKey = "desc_" + id;
            item.icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Items/icon_" + id + ".png");
            item.category = cat;
            item.maxStack = maxStack;
            item.isQuestProtected = questProtected;
            EditorUtility.SetDirty(item);

            table.SetEntry(item.displayNameKey, vnName);
            table.SetEntry(item.descriptionKey, vnDesc);

            if (db.GetById(id) == null)
            {
                db.allItems.Add(item);
            }
        }

        private static void EnsureRelayLanternConfig()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<RelayLanternConfigSO>(RelayConfigPath);
            if (cfg == null)
            {
                cfg = ScriptableObject.CreateInstance<RelayLanternConfigSO>();
                AssetDatabase.CreateAsset(cfg, RelayConfigPath);
            }
            EditorUtility.SetDirty(cfg);
        }

        private static void BuildTwoChapter2EchoAssets()
        {
            var e1 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo1Path);
            if (e1 == null) { e1 = ScriptableObject.CreateInstance<EchoData>(); AssetDatabase.CreateAsset(e1, Echo1Path); }
            e1.echoID = "echo_ch2_storm_lantern";
            e1.echoTitle = "Chiếc Đèn Bão Lễ Trao Hạt Đầu Tiên";
            e1.requiredFlag = "flag_ch2_inv_night3_done";
            e1.requiredItemId = "item_storm_lantern_west";
            e1.completedFlag = "flag_ch2_echo1_lantern_done";
            e1.showKaelGhost = true;
            e1.showMaraGhost = true;
            e1.beats = new List<EchoBeat>
            {
                new EchoBeat
                {
                    speakerName = "Dư Âm Kael (trẻ)",
                    speakerId = "kael",
                    bodyText = "Này, giữ chặt khung đồng đi, cô vẽ hình trăng méo xệch rồi kìa.",
                    durationSeconds = 4.2f,
                    kaelPlaysAction = false,
                    maraPlaysAction = true,
                    overlayTint = new Color(0.32f, 0.24f, 0.12f, 0.34f)
                },
                new EchoBeat
                {
                    speakerName = "Dư Âm Mara (trẻ)",
                    speakerId = "mara",
                    bodyText = "Méo thì sao? Gió bão thổi mạnh tới đâu cũng chỉ cần đèn không tắt là được!",
                    durationSeconds = 4.4f,
                    kaelPlaysAction = false,
                    maraPlaysAction = true,
                    overlayTint = new Color(0.38f, 0.28f, 0.14f, 0.36f)
                },
                new EchoBeat
                {
                    speakerName = "Dư Âm Kael (trẻ)",
                    speakerId = "kael",
                    bodyText = "Vậy giao kèo nhé: tôi giữ đèn bờ Đông, cô giữ đèn bờ Tây. Không bao giờ để sông tối.",
                    durationSeconds = 4.6f,
                    kaelPlaysAction = false,
                    maraPlaysAction = true,
                    overlayTint = new Color(0.42f, 0.32f, 0.15f, 0.38f)
                }
            };
            EditorUtility.SetDirty(e1);

            var e2 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo2Path);
            if (e2 == null) { e2 = ScriptableObject.CreateInstance<EchoData>(); AssetDatabase.CreateAsset(e2, Echo2Path); }
            e2.echoID = "echo_ch2_broken_pier";
            e2.echoTitle = "Đêm Bão Ở Mố Cầu";
            e2.requiredFlag = "flag_ch2_echo1_lantern_done";
            e2.requiredItemId = "";
            e2.completedFlag = "flag_ch2_echo2_pier_done";
            e2.showKaelGhost = true;
            e2.showMaraGhost = false;
            e2.beats = new List<EchoBeat>
            {
                new EchoBeat
                {
                    speakerName = "Dư Âm Kael",
                    speakerId = "kael",
                    bodyText = "Dây cáp đang kéo sập kè nhà trẻ! Không chặt thì lũ nhỏ bờ Đông...",
                    durationSeconds = 4.2f,
                    kaelPlaysAction = true,
                    maraPlaysAction = false,
                    overlayTint = new Color(0.10f, 0.18f, 0.34f, 0.48f)
                },
                new EchoBeat
                {
                    speakerName = "Dư Âm Kael",
                    speakerId = "kael",
                    bodyText = "Mara! Thuyền của cô—! Đứng yên đó, tôi kéo lên—!",
                    durationSeconds = 4.4f,
                    kaelPlaysAction = true,
                    maraPlaysAction = false,
                    overlayTint = new Color(0.08f, 0.14f, 0.28f, 0.52f)
                }
            };
            EditorUtility.SetDirty(e2);
        }

        private static GameObject BuildEchoGhostPrefab(
            string prefabPath, string objName, string idlePrefix, string actionPrefix,
            int idleCount, int actionCount, float idleFps, float actionFps, Color tint)
        {
            Sprite[] idleF = LoadFrames(idlePrefix, idleCount);
            Sprite[] actF = LoadFrames(actionPrefix, actionCount);

            GameObject go = new GameObject(objName);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = idleF.Length > 0 ? idleF[0] : null;
            sr.sortingLayerName = "WeatherVFX";
            sr.sortingOrder = 25;
            sr.color = tint;

            var comp = go.AddComponent<EchoGhostActor>();
            var so = new SerializedObject(comp);
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            AssignSpriteArray(so.FindProperty("idleFrames"), idleF);
            AssignSpriteArray(so.FindProperty("actionFrames"), actF);
            so.FindProperty("idleFps").floatValue = idleFps;
            so.FindProperty("actionFps").floatValue = actionFps;
            so.FindProperty("ghostTint").colorValue = tint;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            UnityEngine.Object.DestroyImmediate(go);
            return saved;
        }

        private static GameObject BuildStormLanternPrefab()
        {
            Sprite[] eastF = LoadFrames("Assets/Art/Characters/actor_storm_lantern/actor_storm_lantern_broken_east", 2);
            Sprite[] westF = LoadFrames("Assets/Art/Characters/actor_storm_lantern/actor_storm_lantern_broken_west", 2);
            Sprite[] litF = LoadFrames("Assets/Art/Characters/actor_storm_lantern/actor_storm_lantern_lit", 4);

            GameObject go = new GameObject("Actor_StormLantern");
            go.tag = "Interactable";
            go.layer = 11;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = eastF.Length > 0 ? eastF[0] : null;
            sr.sortingLayerName = "WorldObjects";

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.7f, 0.7f);

            var ys = go.AddComponent<YSortRenderer>();
            ys.Configure(sr, false, true, 0f, 0);

            var comp = go.AddComponent<StormLanternActor>();
            var so = new SerializedObject(comp);
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            AssignSpriteArray(so.FindProperty("brokenEastFrames"), eastF);
            AssignSpriteArray(so.FindProperty("brokenWestFrames"), westF);
            AssignSpriteArray(so.FindProperty("litFrames"), litF);
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(go, StormLanternPrefabPath);
            UnityEngine.Object.DestroyImmediate(go);
            return saved;
        }

        private static void UpgradeGameRootForChapter2(GameObject kaelGhost, GameObject maraGhost)
        {
            TMP_FontAsset storyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_StoryFont.asset")
                ?? AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_DynamicFont.asset");
            TMP_FontAsset hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_HudCleanFont.asset")
                ?? storyFont;

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            try
            {
                if (root.GetComponent<Chapter2StateManager>() == null)
                {
                    root.AddComponent<Chapter2StateManager>();
                }

                Transform oldCanvas = root.transform.Find("EchoOverlayCanvas");
                if (oldCanvas != null) UnityEngine.Object.DestroyImmediate(oldCanvas.gameObject);

                GameObject canvasGo = new GameObject("EchoOverlayCanvas");
                canvasGo.transform.SetParent(root.transform, false);
                var canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 210;
                var scaler = canvasGo.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1280f, 720f);
                scaler.matchWidthOrHeight = 0.5f;
                var cg = canvasGo.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
                cg.blocksRaycasts = false;
                cg.interactable = false;

                GameObject tintGo = new GameObject("EchoTint", typeof(RectTransform));
                tintGo.transform.SetParent(canvasGo.transform, false);
                StretchFull(tintGo.GetComponent<RectTransform>());
                var tintImg = tintGo.AddComponent<Image>();
                tintImg.color = new Color(0.18f, 0.34f, 0.52f, 0.36f);
                tintImg.raycastTarget = false;

                GameObject titleGo = new GameObject("EchoTitle", typeof(RectTransform));
                titleGo.transform.SetParent(canvasGo.transform, false);
                var tRt = titleGo.GetComponent<RectTransform>();
                tRt.anchorMin = new Vector2(0.1f, 1f); tRt.anchorMax = new Vector2(0.9f, 1f);
                tRt.pivot = new Vector2(0.5f, 1f); tRt.anchoredPosition = new Vector2(0f, -24f); tRt.sizeDelta = new Vector2(0f, 36f);
                var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
                titleTmp.font = hudFont; titleTmp.fontSize = 18f; titleTmp.fontStyle = FontStyles.Bold;
                titleTmp.alignment = TextAlignmentOptions.Center;
                titleTmp.color = new Color(0.86f, 0.96f, 1f, 0.92f);

                GameObject boxGo = new GameObject("EchoSubtitleBox", typeof(RectTransform));
                boxGo.transform.SetParent(canvasGo.transform, false);
                var bRt = boxGo.GetComponent<RectTransform>();
                bRt.anchorMin = new Vector2(0.12f, 0f); bRt.anchorMax = new Vector2(0.88f, 0f);
                bRt.pivot = new Vector2(0.5f, 0f); bRt.anchoredPosition = new Vector2(0f, 24f); bRt.sizeDelta = new Vector2(0f, 112f);
                boxGo.AddComponent<Image>().color = new Color(0.07f, 0.09f, 0.16f, 0.88f);

                GameObject spGo = new GameObject("Speaker", typeof(RectTransform));
                spGo.transform.SetParent(boxGo.transform, false);
                var spRt = spGo.GetComponent<RectTransform>();
                spRt.anchorMin = new Vector2(0f, 1f); spRt.anchorMax = new Vector2(1f, 1f);
                spRt.pivot = new Vector2(0.5f, 1f); spRt.anchoredPosition = new Vector2(0f, -10f); spRt.sizeDelta = new Vector2(-36f, 28f);
                var spTmp = spGo.AddComponent<TextMeshProUGUI>();
                spTmp.font = storyFont; spTmp.fontSize = 18f; spTmp.fontStyle = FontStyles.Bold;
                spTmp.alignment = TextAlignmentOptions.Center;
                spTmp.color = new Color(0.98f, 0.86f, 0.54f, 1f);

                GameObject bodyGo = new GameObject("Body", typeof(RectTransform));
                bodyGo.transform.SetParent(boxGo.transform, false);
                var bdRt = bodyGo.GetComponent<RectTransform>();
                bdRt.anchorMin = Vector2.zero; bdRt.anchorMax = Vector2.one;
                bdRt.offsetMin = new Vector2(24f, 14f); bdRt.offsetMax = new Vector2(-24f, -40f);
                var bodyTmp = bodyGo.AddComponent<TextMeshProUGUI>();
                bodyTmp.font = storyFont; bodyTmp.fontSize = 19f;
                bodyTmp.alignment = TextAlignmentOptions.Center;
                bodyTmp.textWrappingMode = TextWrappingModes.Normal;
                bodyTmp.color = new Color(0.95f, 0.96f, 0.98f, 1f);

                GameObject skipBg = new GameObject("SkipBarBg", typeof(RectTransform));
                skipBg.transform.SetParent(canvasGo.transform, false);
                var sRt = skipBg.GetComponent<RectTransform>();
                sRt.anchorMin = new Vector2(1f, 1f); sRt.anchorMax = new Vector2(1f, 1f);
                sRt.pivot = new Vector2(1f, 1f); sRt.anchoredPosition = new Vector2(-24f, -24f); sRt.sizeDelta = new Vector2(180f, 10f);
                skipBg.AddComponent<Image>().color = new Color(0.2f, 0.25f, 0.35f, 0.7f);

                GameObject skipFillGo = new GameObject("SkipFill", typeof(RectTransform));
                skipFillGo.transform.SetParent(skipBg.transform, false);
                StretchFull(skipFillGo.GetComponent<RectTransform>());
                var skipFill = skipFillGo.AddComponent<Image>();
                skipFill.color = new Color(0.62f, 0.92f, 1f, 0.95f);
                skipFill.type = Image.Type.Filled;
                skipFill.fillMethod = Image.FillMethod.Horizontal;
                skipFill.fillAmount = 0f;
                skipBg.SetActive(false);

                var echoMgr = root.GetComponent<EchoManager>() ?? root.AddComponent<EchoManager>();
                var so = new SerializedObject(echoMgr);
                so.FindProperty("overlayGroup").objectReferenceValue = cg;
                so.FindProperty("tintOverlay").objectReferenceValue = tintImg;
                so.FindProperty("titleText").objectReferenceValue = titleTmp;
                so.FindProperty("speakerText").objectReferenceValue = spTmp;
                so.FindProperty("bodyText").objectReferenceValue = bodyTmp;
                so.FindProperty("skipBarRoot").objectReferenceValue = skipBg;
                so.FindProperty("skipFillImage").objectReferenceValue = skipFill;
                so.FindProperty("kaelGhostPrefab").objectReferenceValue = kaelGhost;
                so.FindProperty("maraGhostPrefab").objectReferenceValue = maraGhost;
                so.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static Sprite[] LoadFrames(string prefix, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>($"{prefix}_{i:D2}.png"));
            }
            return list.ToArray();
        }

        private static void AssignSpriteArray(SerializedProperty prop, Sprite[] sprites)
        {
            if (prop == null) return;
            prop.ClearArray();
            for (int i = 0; i < sprites.Length; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            }
        }

        private static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }
    }
}
