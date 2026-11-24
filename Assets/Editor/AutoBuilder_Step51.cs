using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Chapter3;
using MoonpetalValley.Core;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step51
    {
        private const string RiverScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string OverlayRootName = "NightMarketOverlayRoot";
        private const string StallsDir = "Assets/Art/Sprites/Chapter3/Stalls/";
        private const string PropsDir = "Assets/Art/Sprites/Chapter3/Props/";
        private const string Ch3UiDir = "Assets/Art/Sprites/Chapter3/UI/";
        private const string UiDir = "Assets/Art/Sprites/UI/";
        private const string FontsDir = "Assets/Art/Fonts/";

        private struct StallSpec
        {
            public string id;
            public string objName;
            public string villagerName;
            public string spriteFile;
            public Vector3 stallPos;
            public Vector3 spotOffset;
        }

        // Bố cục Chợ Đêm ven sông: giữ mặt cầu thông thoáng làm lối dạo treo đèn lồng,
        // đặt 4 quầy trên bãi cỏ Bờ Đông và 3 quầy trên bãi cỏ Bờ Tây (mọi quầy quay chính diện hướng Nam).
        private static readonly StallSpec[] AllStalls =
        {
            // Bờ Đông (4 quầy)
            new StallSpec { id = "stall_bach", objName = "Stall_Bach_Farm", villagerName = "Ông Bach", spriteFile = "stall_bach_farm.png", stallPos = new Vector3(14.2f, 3.0f, 0f), spotOffset = new Vector3(0f, -1.35f, 0f) },
            new StallSpec { id = "stall_hana", objName = "Stall_Hana_Bakery", villagerName = "Bà Hana", spriteFile = "stall_hana_bakery.png", stallPos = new Vector3(18.0f, -1.2f, 0f), spotOffset = new Vector3(0f, -1.35f, 0f) },
            new StallSpec { id = "stall_kael", objName = "Stall_Kael_Woodcraft", villagerName = "Ông Kael", spriteFile = "stall_kael_woodcraft.png", stallPos = new Vector3(12.0f, -4.2f, 0f), spotOffset = new Vector3(0f, -1.35f, 0f) },
            new StallSpec { id = "stall_otis", objName = "Stall_Otis_Post", villagerName = "Bác Otis", spriteFile = "stall_otis_post.png", stallPos = new Vector3(15.6f, -5.8f, 0f), spotOffset = new Vector3(0f, -1.35f, 0f) },
            // Bờ Tây (3 quầy)
            new StallSpec { id = "stall_toby", objName = "Stall_Toby_PaperFish", villagerName = "Bé Toby", spriteFile = "stall_toby_paperfish.png", stallPos = new Vector3(-11.2f, 2.6f, 0f), spotOffset = new Vector3(0f, -1.35f, 0f) },
            new StallSpec { id = "stall_linh", objName = "Stall_Linh_Scout", villagerName = "Linh", spriteFile = "stall_linh_scout.png", stallPos = new Vector3(-14.8f, 2.6f, 0f), spotOffset = new Vector3(0f, -1.35f, 0f) },
            new StallSpec { id = "stall_mara", objName = "Stall_Mara_RiverTea", villagerName = "Bà Mara", spriteFile = "stall_mara_rivertea.png", stallPos = new Vector3(-17.2f, -2.4f, 0f), spotOffset = new Vector3(0f, -1.35f, 0f) }
        };

        [MenuItem("Moonpetal/Step 51/1. Build Night Market Bridge Overlay & 100s Service", false, 510)]
        public static void BuildStep51()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV][Step51] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            AssetDatabase.Refresh();
            EnsureStallSpritesImported();
            var scene = EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);

            GameObject existing = GameObject.Find(OverlayRootName);
            if (existing != null)
                Object.DestroyImmediate(existing);

            GameObject root = new GameObject(OverlayRootName);
            NightMarketOverlayController overlayCtrl = root.AddComponent<NightMarketOverlayController>();
            NightMarketServiceManager serviceMgr = root.AddComponent<NightMarketServiceManager>();
            NightMarketHUDView hudView = root.AddComponent<NightMarketHUDView>();

            GameObject visuals = new GameObject("VisualsAndStalls");
            visuals.transform.SetParent(root.transform, false);

            Sprite bannerSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PropsDir + "prop_ch3_festival_banner.png");
            Sprite garlandSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PropsDir + "prop_ch3_garland_lanterns.png");
            Sprite heartSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch3UiDir + "ui_ch3_warmth_heart.png");
            Sprite panelWoodSpr = AssetDatabase.LoadAssetAtPath<Sprite>(UiDir + "ui_panel_dark_wood.png");
            Sprite barFrameSpr = AssetDatabase.LoadAssetAtPath<Sprite>(UiDir + "ui_bar_frame.png");
            TMP_FontAsset warmFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontsDir + "VN_MenuWarmFont.asset");
            TMP_FontAsset cleanFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontsDir + "VN_HudCleanFont.asset");

            // 1. Festival Banner & Start Trigger at East Bridge Entrance
            GameObject bannerGo = new GameObject("FestivalBanner_EastEntrance");
            bannerGo.transform.SetParent(visuals.transform, false);
            bannerGo.transform.position = new Vector3(8.5f, 2.35f, 0f);
            SpriteRenderer bsr = bannerGo.AddComponent<SpriteRenderer>();
            bsr.sprite = bannerSpr;
            bsr.sortingLayerName = "WorldObjects";
            YSortRenderer bys = bannerGo.AddComponent<YSortRenderer>();
            bys.Configure(bsr, false, true, 0f, 0);

            GameObject triggerGo = new GameObject("NightMarketStartTrigger");
            triggerGo.transform.SetParent(visuals.transform, false);
            triggerGo.transform.position = new Vector3(8.5f, 0.85f, 0f);
            NightMarketStartTriggerInteractable startTrig = triggerGo.AddComponent<NightMarketStartTriggerInteractable>();
            SerializedObject tso = new SerializedObject(startTrig);
            tso.FindProperty("serviceManager").objectReferenceValue = serviceMgr;
            tso.ApplyModifiedPropertiesWithoutUndo();

            // 2. 3 Overhead Garland Lanterns along Repaired Bridge
            float[] garlandXs = { -4.8f, 0.0f, 4.8f };
            for (int i = 0; i < garlandXs.Length; i++)
            {
                GameObject gGo = new GameObject("BridgeGarland_" + (i + 1));
                gGo.transform.SetParent(visuals.transform, false);
                gGo.transform.position = new Vector3(garlandXs[i], 1.95f, 0f);
                SpriteRenderer gsr = gGo.AddComponent<SpriteRenderer>();
                gsr.sprite = garlandSpr;
                gsr.sortingLayerName = "WorldObjects";
                gsr.sortingOrder = 120;
            }

            // 3. 7 Villager Stalls + 7 NightMarketCustomerSpots
            List<NightMarketCustomerSpot> spots = new List<NightMarketCustomerSpot>();
            int boundStalls = 0;
            for (int i = 0; i < AllStalls.Length; i++)
            {
                StallSpec spec = AllStalls[i];
                GameObject stallGo = new GameObject(spec.objName);
                stallGo.transform.SetParent(visuals.transform, false);
                stallGo.transform.position = spec.stallPos;

                Sprite stallSpr = AssetDatabase.LoadAssetAtPath<Sprite>(StallsDir + spec.spriteFile);
                SpriteRenderer ssr = stallGo.AddComponent<SpriteRenderer>();
                ssr.sprite = stallSpr;
                ssr.sortingLayerName = "WorldObjects";
                if (stallSpr != null) boundStalls++;

                BoxCollider2D stallCol = stallGo.AddComponent<BoxCollider2D>();
                stallCol.size = new Vector2(2.35f, 1.05f);
                stallCol.offset = new Vector2(0f, -0.55f);

                YSortRenderer sys = stallGo.AddComponent<YSortRenderer>();
                sys.Configure(ssr, false, true, -0.95f, 0);

                // Customer interaction spot in front of stall
                GameObject spotGo = new GameObject("CustomerSpot_" + spec.id);
                spotGo.transform.SetParent(stallGo.transform, false);
                spotGo.transform.localPosition = spec.spotOffset;

                GameObject badgeGo = new GameObject("OrderHeartBadge");
                badgeGo.transform.SetParent(stallGo.transform, false);
                badgeGo.transform.localPosition = new Vector3(0f, 1.35f, 0f);
                badgeGo.transform.localScale = new Vector3(0.65f, 0.65f, 1f);
                SpriteRenderer badgeSr = badgeGo.AddComponent<SpriteRenderer>();
                badgeSr.sprite = heartSpr;
                badgeSr.sortingLayerName = "WorldObjects";
                badgeSr.sortingOrder = 220;
                badgeSr.enabled = false;

                NightMarketCustomerSpot spot = spotGo.AddComponent<NightMarketCustomerSpot>();
                SerializedObject sso = new SerializedObject(spot);
                sso.FindProperty("stallId").stringValue = spec.id;
                sso.FindProperty("villagerName").stringValue = spec.villagerName;
                sso.FindProperty("requestBadgeRenderer").objectReferenceValue = badgeSr;
                sso.FindProperty("serviceManager").objectReferenceValue = serviceMgr;
                sso.ApplyModifiedPropertiesWithoutUndo();
                spots.Add(spot);
            }

            // 4. Wire OverlayController, ServiceManager, HUDView
            SerializedObject oso = new SerializedObject(overlayCtrl);
            oso.FindProperty("visualsRoot").objectReferenceValue = visuals;
            oso.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject mso = new SerializedObject(serviceMgr);
            mso.FindProperty("hudView").objectReferenceValue = hudView;
            SerializedProperty spotsProp = mso.FindProperty("customerSpots");
            spotsProp.ClearArray();
            for (int i = 0; i < spots.Count; i++)
            {
                spotsProp.InsertArrayElementAtIndex(i);
                spotsProp.GetArrayElementAtIndex(i).objectReferenceValue = spots[i];
            }
            mso.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject hso = new SerializedObject(hudView);
            hso.FindProperty("panelDarkWoodSprite").objectReferenceValue = panelWoodSpr;
            hso.FindProperty("barFrameSprite").objectReferenceValue = barFrameSpr;
            hso.FindProperty("warmthHeartSprite").objectReferenceValue = heartSpr;
            hso.FindProperty("menuWarmFont").objectReferenceValue = warmFont;
            hso.FindProperty("hudCleanFont").objectReferenceValue = cleanFont;
            hso.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(string.Format(
                "[MV][Step51] BUILD THANH CONG! Stalls={0}/7, Banner={1}, Garland={2}, WarmthHeart={3}, Spots={4}",
                boundStalls, bannerSpr != null, garlandSpr != null, heartSpr != null, spots.Count));
        }

        private static void EnsureStallSpritesImported()
        {
            bool changed = false;
            for (int i = 0; i < AllStalls.Length; i++)
            {
                string path = StallsDir + AllStalls[i].spriteFile;
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;
                bool dirty = false;
                if (ti.textureType != TextureImporterType.Sprite) { ti.textureType = TextureImporterType.Sprite; dirty = true; }
                if (ti.spriteImportMode != SpriteImportMode.Single) { ti.spriteImportMode = SpriteImportMode.Single; dirty = true; }
                if (Mathf.Abs(ti.spritePixelsPerUnit - 32f) > 0.01f) { ti.spritePixelsPerUnit = 32f; dirty = true; }
                if (ti.filterMode != FilterMode.Point) { ti.filterMode = FilterMode.Point; dirty = true; }
                if (ti.textureCompression != TextureImporterCompression.Uncompressed) { ti.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
                if (!ti.alphaIsTransparency) { ti.alphaIsTransparency = true; dirty = true; }
                TextureImporterSettings tis = new TextureImporterSettings();
                ti.ReadTextureSettings(tis);
                if (tis.spriteMeshType != SpriteMeshType.FullRect) { tis.spriteMeshType = SpriteMeshType.FullRect; dirty = true; }
                if (dirty)
                {
                    ti.SetTextureSettings(tis);
                    ti.SaveAndReimport();
                    changed = true;
                }
            }
            if (changed) AssetDatabase.Refresh();
        }

        [MenuItem("Moonpetal/Step 51/2. Validate Step 51", false, 511)]
        public static void ValidateStep51()
        {
            int errors = 0;
            EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);

            NightMarketOverlayController overlay = Object.FindAnyObjectByType<NightMarketOverlayController>();
            NightMarketServiceManager mgr = Object.FindAnyObjectByType<NightMarketServiceManager>();
            NightMarketHUDView hud = Object.FindAnyObjectByType<NightMarketHUDView>();
            NightMarketStartTriggerInteractable trig = Object.FindAnyObjectByType<NightMarketStartTriggerInteractable>();
            NightMarketCustomerSpot[] spots = Object.FindObjectsByType<NightMarketCustomerSpot>(FindObjectsInactive.Include);

            if (overlay == null) { Debug.LogError("[MV][Step51][FAIL] Thieu NightMarketOverlayController."); errors++; }
            if (mgr == null) { Debug.LogError("[MV][Step51][FAIL] Thieu NightMarketServiceManager."); errors++; }
            if (hud == null) { Debug.LogError("[MV][Step51][FAIL] Thieu NightMarketHUDView."); errors++; }
            if (trig == null) { Debug.LogError("[MV][Step51][FAIL] Thieu NightMarketStartTriggerInteractable."); errors++; }
            if (spots.Length != 7) { Debug.LogError("[MV][Step51][FAIL] Can dung 7 NightMarketCustomerSpot, hien co: " + spots.Length); errors++; }

            for (int i = 0; i < AllStalls.Length; i++)
            {
                GameObject go = GameObject.Find(AllStalls[i].objName);
                SpriteRenderer sr = go != null ? go.GetComponent<SpriteRenderer>() : null;
                if (sr == null || sr.sprite == null)
                {
                    Debug.LogError("[MV][Step51][FAIL] Gian hang thieu Sprite: " + AllStalls[i].objName);
                    errors++;
                }
            }

            GameObject banner = GameObject.Find("FestivalBanner_EastEntrance");
            SpriteRenderer bsr = banner != null ? banner.GetComponent<SpriteRenderer>() : null;
            if (bsr == null || bsr.sprite == null)
            {
                Debug.LogError("[MV][Step51][FAIL] Thieu Sprite cho FestivalBanner_EastEntrance.");
                errors++;
            }

            GameObject[] allGos = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]) > 0)
                {
                    Debug.LogError("[MV][Step51][FAIL] Missing Script tren: " + allGos[i].name);
                    errors++;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Step51] VALIDATE PASS (0 loi). 7 Gian Hang, Banner, Garland, HUD & 100s Service san sang!");
            else
                Debug.LogError("[MV][Step51] VALIDATE FAIL (" + errors + " loi).");
        }

        [MenuItem("Moonpetal/Step 51/3. Debug Jump To Chợ Đêm (Play Mode)", false, 512)]
        public static void DebugJumpToNightMarket()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("[MV][Step51] Lệnh này chỉ chạy trong Play Mode.");
                return;
            }
            if (GameRoot.Instance == null) return;

            StoryFlagManager flags = GameRoot.Instance.Flags;
            if (flags != null)
            {
                flags.SetFlag("flag_lumi_awakened", true);
                flags.SetFlag("flag_tutorial_done", true);
                flags.SetFlag("flag_nv1_completed", true);
                flags.SetFlag(Chapter3StateManager.FlagCh2Completed, true);
                flags.SetFlag(Chapter3StateManager.FlagCh3Started, true);
                flags.SetFlag(Chapter3StateManager.FlagCh3StallsBuiltDay3, true);
                flags.SetFlag(Chapter3StateManager.FlagCh3MarketDone, false);
                flags.SetFlag(Chapter3StateManager.FlagCh3WarmthBonus, false);
            }

            if (Chapter2StateManager.Instance != null)
            {
                Chapter2StateManager.Instance.SetBridgeState(RiverBridgeState.Repaired);
                Chapter2StateManager.Instance.PendingSceneSpawnId = "Spawn_BridgeEast";
            }

            if (GameRoot.Instance.Time != null)
                GameRoot.Instance.Time.SetHour(18.5f);

            if (GameRoot.Instance.Inventory != null)
            {
                string[] goods =
                {
                    "item_ch3_seed_cake", "item_ch3_river_tea", "item_ch3_paper_fish",
                    "item_ch3_wish_card", "item_ch3_lantern_charm"
                };
                for (int i = 0; i < goods.Length; i++)
                    GameRoot.Instance.Inventory.AddItemById(goods[i], 2, false);
            }

            if (SceneManager.GetActiveScene().name == "Riverbank_Chapter2")
            {
                if (WorldSpawnRegistry.Instance != null)
                    WorldSpawnRegistry.Instance.TeleportPlayerTo("Spawn_BridgeEast");
                NightMarketOverlayController overlay = Object.FindAnyObjectByType<NightMarketOverlayController>();
                if (overlay != null) overlay.RefreshVisibility();
            }
            else if (GameRoot.Instance.Scenes != null)
            {
                GameRoot.Instance.Scenes.LoadScene("Riverbank_Chapter2");
            }

            Debug.Log("[MV][Step51] Đã nhảy tới Bờ Sông Chợ Đêm (Ngày 3, 18:30, Cầu đã sửa, +2 mỗi món Ch3).");
        }
    }
}
