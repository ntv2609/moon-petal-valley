using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MoonpetalValley.Audio;
using MoonpetalValley.Chapter3;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step49 {
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string VillageScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string RiverScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string ItemDatabasePath = "Assets/Data/Config/ItemDatabase.asset";
        private const string ItemsDir = "Assets/Data/Items";
        private const string Ch3ConfigDir = "Assets/Resources/Moonpetal/Chapter3";
        private const string Ch3ConfigPath = Ch3ConfigDir + "/NightMarketConfig.asset";
        private const string Ch3MusicDir = "Assets/Audio/Music/Ch3";
        private const string TracksDir = "Assets/Resources/Moonpetal/Audio/Tracks";
        private const string LibraryPath = "Assets/Resources/Moonpetal/Audio/MusicLibrary.asset";
        private static readonly (string path, int ppu)[] AllCh3Sprites = {
            ("Assets/Art/Sprites/Items/icon_item_river_daisy.png", 32),
            ("Assets/Art/Sprites/Items/icon_item_ch3_seed_cake.png", 32),
            ("Assets/Art/Sprites/Items/icon_item_ch3_river_tea.png", 32),
            ("Assets/Art/Sprites/Items/icon_item_ch3_paper_fish.png", 32),
            ("Assets/Art/Sprites/Items/icon_item_ch3_wish_card.png", 32),
            ("Assets/Art/Sprites/Items/icon_item_ch3_lantern_charm.png", 32),
            ("Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_mini_hoe.png", 32),
            ("Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_wooden_spoon.png", 32),
            ("Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_glow_paper_fish.png", 32),
            ("Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_post_stamp.png", 32),
            ("Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_wood_spyglass.png", 32),
            ("Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_carpenter_pencil.png", 32),
            ("Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_net_needle.png", 32),
            ("Assets/Art/Sprites/Chapter3/Stalls/stall_bach_farm.png", 32),
            ("Assets/Art/Sprites/Chapter3/Stalls/stall_hana_bakery.png", 32),
            ("Assets/Art/Sprites/Chapter3/Stalls/stall_toby_paperfish.png", 32),
            ("Assets/Art/Sprites/Chapter3/Stalls/stall_otis_post.png", 32),
            ("Assets/Art/Sprites/Chapter3/Stalls/stall_linh_scout.png", 32),
            ("Assets/Art/Sprites/Chapter3/Stalls/stall_kael_woodcraft.png", 32),
            ("Assets/Art/Sprites/Chapter3/Stalls/stall_mara_rivertea.png", 32),
            ("Assets/Art/Sprites/Chapter3/Props/prop_ch3_workbench.png", 32),
            ("Assets/Art/Sprites/Chapter3/Props/prop_ch3_river_daisy_plant.png", 32),
            ("Assets/Art/Sprites/Chapter3/Props/prop_ch3_festival_banner.png", 32),
            ("Assets/Art/Sprites/Chapter3/Props/prop_ch3_echo_oven_glow.png", 32),
            ("Assets/Art/Sprites/Chapter3/Props/prop_ch3_garland_lanterns.png", 32),
            ("Assets/Art/Sprites/Chapter3/UI/ui_ch3_warmth_heart.png", 32),
            ("Assets/Art/Sprites/Chapter3/UI/ui_ch3_keepsake_book_icon.png", 32),
        };
        [MenuItem("Moonpetal/Step 49/1. Build Chapter 3 Core, Auto-Import 27 Art & Bind T09 Audio", false, 490)]
        public static void BuildStep49() {
            AssetDatabase.Refresh();
            int importedSprites = ConfigureAllChapter3SpriteImporters();
            int itemsBound = CreateAndRegisterChapter3Items();
            NightMarketConfigSO config = CreateOrUpdateNightMarketConfig();
            AttachManagersToGameRootPrefab(config);
            PlaceWorkbenchInVillageScene();
            PlaceRiverDaisiesInRiverScene();
            bool t09Bound = BindT09Audio();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(string.Format(
                "[MV][Step49] BUILD THANH CONG! ImportedArt={0}/27, Ch3Items={1}/6, Recipes={2}, Keepsakes={3}, T09Bound={4}",
                importedSprites, itemsBound,
                config != null ? config.recipes.Count : 0,
                config != null ? config.keepsakes.Count : 0,
                t09Bound));
        }
        [MenuItem("Moonpetal/Step 49/2. Validate Step 49", false, 491)]
        public static void ValidateStep49() {
            int errors = 0;
            int warnings = 0;
            for (int i = 0; i < AllCh3Sprites.Length; i++) {
                string path = AllCh3Sprites[i].path;
                Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sp == null) {
                    Debug.LogError("[MV][Step49][FAIL] Thieu Sprite: " + path);
                    errors++;
                }
            }
            ItemDatabaseSO db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            string[] itemIds = {
                "item_river_daisy", "item_ch3_seed_cake", "item_ch3_river_tea",
                "item_ch3_paper_fish", "item_ch3_wish_card", "item_ch3_lantern_charm"
            };
            for (int i = 0; i < itemIds.Length; i++) {
                ItemData item = db != null ? db.GetById(itemIds[i]) : null;
                if (item == null || item.icon == null) {
                    Debug.LogError("[MV][Step49][FAIL] Thieu ItemData hoac Icon trong ItemDatabase: " + itemIds[i]);
                    errors++;
                }
            }
            NightMarketConfigSO cfg = AssetDatabase.LoadAssetAtPath<NightMarketConfigSO>(Ch3ConfigPath);
            if (cfg == null || cfg.recipes.Count < 5 || cfg.keepsakes.Count < 7) {
                Debug.LogError("[MV][Step49][FAIL] NightMarketConfigSO chua du 5 cong thuc / 7 Ky Vat.");
                errors++;
            }
            else {
                for (int i = 0; i < cfg.keepsakes.Count; i++) {
                    if (cfg.keepsakes[i].icon == null) {
                        Debug.LogError("[MV][Step49][FAIL] Keepsake thieu icon: " + cfg.keepsakes[i].keepsakeId);
                        errors++;
                    }
                }
            }
            GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (root == null || root.GetComponent<Chapter3StateManager>() == null ||
                root.GetComponent<NightMarketCraftingManager>() == null ||
                root.GetComponent<PrepWorkbenchUIView>() == null) {
                Debug.LogError("[MV][Step49][FAIL] GameRoot.prefab thieu component Chapter 3.");
                errors++;
            }
            MusicTrackSO t09 = AssetDatabase.LoadAssetAtPath<MusicTrackSO>(TracksDir + "/MV_Music_T09_NightMarket.asset");
            if (t09 == null || t09.clip == null || t09.isDormant) {
                Debug.LogWarning("[MV][Step49][WARN] MV_Music_T09_NightMarket chua co clip (hay chep T09.wav vao Assets/Audio/Music/Ch3/ roi Build lai).");
                warnings++;
            }
            errors += CountMissingScriptsInScene(VillageScenePath);
            errors += CountMissingScriptsInScene(RiverScenePath);
            if (errors == 0)
                Debug.Log(string.Format("[MV][Step49] VALIDATE PASS (0 loi, {0} canh bao).", warnings));
            else
                Debug.LogError(string.Format("[MV][Step49] VALIDATE FAIL ({0} loi, {1} canh bao).", errors, warnings));
        }
        private static int ConfigureAllChapter3SpriteImporters() {
            int count = 0;
            for (int i = 0; i < AllCh3Sprites.Length; i++) {
                string path = AllCh3Sprites[i].path;
                int ppu = AllCh3Sprites[i].ppu;
                TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (imp == null) continue;
                bool dirty = false;
                if (imp.textureType != TextureImporterType.Sprite) { imp.textureType = TextureImporterType.Sprite; dirty = true; }
                if (imp.spriteImportMode != SpriteImportMode.Single) { imp.spriteImportMode = SpriteImportMode.Single; dirty = true; }
                if (Mathf.Abs(imp.spritePixelsPerUnit - ppu) > 0.01f) { imp.spritePixelsPerUnit = ppu; dirty = true; }
                if (imp.filterMode != FilterMode.Point) { imp.filterMode = FilterMode.Point; dirty = true; }
                if (imp.textureCompression != TextureImporterCompression.Uncompressed) { imp.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
                if (imp.mipmapEnabled) { imp.mipmapEnabled = false; dirty = true; }
                if (!imp.alphaIsTransparency) { imp.alphaIsTransparency = true; dirty = true; }
                TextureImporterSettings tis = new TextureImporterSettings();
                imp.ReadTextureSettings(tis);
                if (tis.spriteMeshType != SpriteMeshType.FullRect) {
                    tis.spriteMeshType = SpriteMeshType.FullRect;
                    imp.SetTextureSettings(tis);
                    dirty = true;
                }
                if (dirty) imp.SaveAndReimport();
                count++;
            }
            return count;
        }
        private static int CreateAndRegisterChapter3Items() {
            EnsureFolder(ItemsDir);
            ItemDatabaseSO db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            var defs = new[] {
                ("item_river_daisy", "Hoa Cúc Sông", "Đóa hoa cúc trắng thơm dịu hái dọc bờ sông Ánh Trăng.", ItemCategory.Material, 0, "Assets/Art/Sprites/Items/icon_item_river_daisy.png"),
                ("item_ch3_seed_cake", "Bánh Hạt Mầm Bà Hana", "Bánh nướng thơm bùi theo công thức cũ của Bà Ngoại và Bà Hana.", ItemCategory.Consumable, 25, "Assets/Art/Sprites/Items/icon_item_ch3_seed_cake.png"),
                ("item_ch3_river_tea", "Trà Cúc Sông Ấm", "Tách trà thảo mộc ấm bụng dành cho đêm gió sông.", ItemCategory.Consumable, 20, "Assets/Art/Sprites/Items/icon_item_ch3_river_tea.png"),
                ("item_ch3_paper_fish", "Cá Giấy Phát Sáng", "Đèn cá giấy gấp tay tỏa ánh vàng dịu cho gian hàng Toby.", ItemCategory.Material, 0, "Assets/Art/Sprites/Items/icon_item_ch3_paper_fish.png"),
                ("item_ch3_wish_card", "Thiệp Ước Nguyện", "Tấm thiệp buộc chỉ đỏ để dân làng viết điều ước đầu mùa.", ItemCategory.Material, 0, "Assets/Art/Sprites/Items/icon_item_ch3_wish_card.png"),
                ("item_ch3_lantern_charm", "Bùa Đèn Lồng Nhỏ", "Chiếc đèn lồng mini đan dây gửi gắm bình an.", ItemCategory.Material, 0, "Assets/Art/Sprites/Items/icon_item_ch3_lantern_charm.png"),
            };
            int bound = 0;
            for (int i = 0; i < defs.Length; i++) {
                var d = defs[i];
                string assetPath = ItemsDir + "/Item_" + d.Item1 + ".asset";
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
                if (item == null) {
                    item = ScriptableObject.CreateInstance<ItemData>();
                    AssetDatabase.CreateAsset(item, assetPath);
                }
                item.itemID = d.Item1;
                item.displayNameKey = d.Item2;
                item.descriptionKey = d.Item3;
                item.category = d.Item4;
                item.maxStack = 99;
                item.staminaRestore = d.Item5;
                item.icon = AssetDatabase.LoadAssetAtPath<Sprite>(d.Item6);
                EditorUtility.SetDirty(item);
                if (db != null && !db.allItems.Contains(item)) {
                    db.allItems.Add(item);
                    EditorUtility.SetDirty(db);
                }
                if (item.icon != null) bound++;
            }
            return bound;
        }
        private static NightMarketConfigSO CreateOrUpdateNightMarketConfig() {
            EnsureFolder(Ch3ConfigDir);
            NightMarketConfigSO cfg = AssetDatabase.LoadAssetAtPath<NightMarketConfigSO>(Ch3ConfigPath);
            if (cfg == null) {
                cfg = ScriptableObject.CreateInstance<NightMarketConfigSO>();
                AssetDatabase.CreateAsset(cfg, Ch3ConfigPath);
            }
            cfg.serviceDurationSeconds = 100f;
            cfg.warmthBonusThreshold = 70f;
            cfg.targetPrepItems = 5;
            cfg.recipes = new List<NightMarketRecipeDef> {
                MakeRecipe("rcp_seed_cake", "item_ch3_seed_cake", "Bánh Hạt Mầm Bà Hana", "Vị bánh tuổi thơ từ lò gạch cũ.", 3,
                    ("item_dew_barley", "Lúa Mạch Sương Mai", 2), ("item_dew_sap", "Nhựa Sương", 1)),
                MakeRecipe("rcp_river_tea", "item_ch3_river_tea", "Trà Cúc Sông Ấm", "Hương cúc bờ sông ấm lòng đêm chợ.", 2,
                    ("item_river_daisy", "Hoa Cúc Sông", 2), ("item_dew_sap", "Nhựa Sương", 1)),
                MakeRecipe("rcp_paper_fish", "item_ch3_paper_fish", "Cá Giấy Phát Sáng", "Món đồ chơi phát sáng Toby thích nhất.", 3,
                    ("item_branch_wood", "Gỗ Nhánh", 2), ("item_starlight_dust", "Bụi Ánh Sao", 1)),
                MakeRecipe("rcp_wish_card", "item_ch3_wish_card", "Thiệp Ước Nguyện", "Gửi điều ước lên dây đèn cầu gỗ.", 2,
                    ("item_branch_wood", "Gỗ Nhánh", 1), ("item_river_daisy", "Hoa Cúc Sông", 1)),
                MakeRecipe("rcp_lantern_charm", "item_ch3_lantern_charm", "Bùa Đèn Lồng Nhỏ", "Ngọn lửa nhỏ giữ ấm bàn tay mùa sương.", 4,
                    ("item_branch_wood", "Gỗ Nhánh", 2), ("item_dew_sap", "Nhựa Sương", 1), ("item_starlight_dust", "Bụi Ánh Sao", 1))
            };
            cfg.keepsakes = new List<KeepsakeDef> {
                MakeKeepsake("ks_mini_hoe", "npc_bach", "Ông Bach", "Móng Cuốc Nhỏ", "Kỷ vật từ khu vườn hồi sinh.", "Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_mini_hoe.png"),
                MakeKeepsake("ks_wooden_spoon", "npc_hana", "Bà Hana", "Chiếc Thìa Gỗ", "Thìa khuấy bột từ thời Bà Ngoại.", "Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_wooden_spoon.png"),
                MakeKeepsake("ks_glow_paper_fish", "npc_toby", "Cậu Bé Toby", "Con Cá Giấy Phát Sáng", "Toby tự tay gấp tặng Wen.", "Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_glow_paper_fish.png"),
                MakeKeepsake("ks_post_stamp", "npc_otis", "Bác Otis", "Con Dấu Bưu Điện", "Dấu sáp hình Cánh Hoa Ánh Trăng.", "Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_post_stamp.png"),
                MakeKeepsake("ks_wood_spyglass", "npc_linh", "Linh", "Ống Nhòm Gỗ", "Chiếc ống nhòm theo Linh qua rừng Bắc.", "Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_wood_spyglass.png"),
                MakeKeepsake("ks_carpenter_pencil", "npc_kael", "Ông Kael", "Cây Bút Chì Thợ Mộc", "Bút chì đánh dấu từng nhịp cầu mới.", "Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_carpenter_pencil.png"),
                MakeKeepsake("ks_net_needle", "npc_mara", "Bà Mara", "Chiếc Kim Vá Lưới", "Kim vá lưới gắn bó nửa đời bên sông.", "Assets/Art/Sprites/Chapter3/Keepsakes/icon_ks_net_needle.png"),
            };
            EditorUtility.SetDirty(cfg);
            return cfg;
        }
        private static NightMarketRecipeDef MakeRecipe(string id, string outId, string title, string flavor, int stamina, params (string itemId, string name, int amt)[] ings) {
            var r = new NightMarketRecipeDef { recipeId = id, outputItemId = outId, displayName = title, flavorText = flavor, staminaCost = stamina, requiredStoryFlag = "" };
            for (int i = 0; i < ings.Length; i++) r.ingredients.Add(new RecipeIngredient { itemId = ings[i].itemId, displayName = ings[i].name, amount = ings[i].amt });
            return r;
        }
        private static KeepsakeDef MakeKeepsake(string id, string npcId, string npcName, string title, string note, string iconPath) {
            return new KeepsakeDef {
                keepsakeId = id,
                npcId = npcId,
                npcDisplayName = npcName,
                displayName = title,
                storyNote = note,
                icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath)
            };
        }
        private static void AttachManagersToGameRootPrefab(NightMarketConfigSO config) {
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (prefabRoot == null) {
                Debug.LogError("[MV][Step49] Khong tim thay " + GameRootPrefabPath);
                return;
            }
            Chapter3StateManager st = prefabRoot.GetComponent<Chapter3StateManager>();
            if (st == null) st = prefabRoot.AddComponent<Chapter3StateManager>();
            SerializedObject stSo = new SerializedObject(st);
            stSo.FindProperty("config").objectReferenceValue = config;
            stSo.ApplyModifiedPropertiesWithoutUndo();
            if (prefabRoot.GetComponent<NightMarketCraftingManager>() == null)
                prefabRoot.AddComponent<NightMarketCraftingManager>();
            PrepWorkbenchUIView ui = prefabRoot.GetComponent<PrepWorkbenchUIView>();
            if (ui == null) ui = prefabRoot.AddComponent<PrepWorkbenchUIView>();
            SerializedObject uiSo = new SerializedObject(ui);
            uiSo.FindProperty("vnFont").objectReferenceValue = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/VN_HudCleanFont.asset");
            uiSo.FindProperty("panelWoodSprite").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_panel_dark_wood.png");
            uiSo.FindProperty("buttonWoodSprite").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/ui_button_wood.png");
            uiSo.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
        private static void PlaceWorkbenchInVillageScene() {
            Scene scene = EditorSceneManager.OpenScene(VillageScenePath, OpenSceneMode.Single);
            GameObject wb = GameObject.Find("Ch3PrepWorkbench");
            if (wb == null) wb = new GameObject("Ch3PrepWorkbench");
            wb.transform.position = new Vector3(1.8f, -1.2f, 0f);
            SpriteRenderer sr = wb.GetComponent<SpriteRenderer>();
            if (sr == null) sr = wb.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Chapter3/Props/prop_ch3_workbench.png");
            sr.sortingOrder = 5;
            BoxCollider2D col = wb.GetComponent<BoxCollider2D>();
            if (col == null) col = wb.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1.8f, 1.1f);
            col.isTrigger = false;
            if (wb.GetComponent<Ch3PrepWorkbenchInteractable>() == null)
                wb.AddComponent<Ch3PrepWorkbenchInteractable>();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        private static void PlaceRiverDaisiesInRiverScene() {
            Scene scene = EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);
            GameObject root = GameObject.Find("Ch3RiverDaisiesRoot");
            if (root == null) root = new GameObject("Ch3RiverDaisiesRoot");
            Sprite daisySp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Chapter3/Props/prop_ch3_river_daisy_plant.png");
            Vector3[] positions = {
                new Vector3(6.5f, 3.2f, 0f),
                new Vector3(8.2f, -2.8f, 0f),
                new Vector3(-7.4f, 2.6f, 0f),
                new Vector3(-9.0f, -3.1f, 0f),
            };
            for (int i = 0; i < positions.Length; i++) {
                string nodeName = "RiverDaisyNode_" + (i + 1);
                Transform existing = root.transform.Find(nodeName);
                GameObject go = existing != null ? existing.gameObject : new GameObject(nodeName);
                go.transform.SetParent(root.transform, false);
                go.transform.position = positions[i];
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr == null) sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = daisySp;
                sr.sortingOrder = 4;
                CircleCollider2D col = go.GetComponent<CircleCollider2D>();
                if (col == null) col = go.AddComponent<CircleCollider2D>();
                col.radius = 0.6f;
                col.isTrigger = true;
                if (go.GetComponent<RiverDaisyForageNode>() == null)
                    go.AddComponent<RiverDaisyForageNode>();
            }
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        private static bool BindT09Audio() {
            string[] stems = { "MV_Music_T09_NightMarket", "T09" };
            string[] exts = { ".wav", ".ogg", ".mp3" };
            AudioClip clip = null;
            string clipPath = null;
            for (int s = 0; s < stems.Length && clip == null; s++) {
                for (int e = 0; e < exts.Length && clip == null; e++) {
                    string candidate = Ch3MusicDir + "/" + stems[s] + exts[e];
                    clip = AssetDatabase.LoadAssetAtPath<AudioClip>(candidate);
                    if (clip != null) clipPath = candidate;
                }
            }
            if (clip == null) return false;
            AudioImporter imp = AssetImporter.GetAtPath(clipPath) as AudioImporter;
            if (imp != null) {
                AudioImporterSampleSettings sample = imp.defaultSampleSettings;
                sample.compressionFormat = AudioCompressionFormat.Vorbis;
                sample.quality = 0.7f;
                sample.loadType = AudioClipLoadType.Streaming;
                imp.defaultSampleSettings = sample;
                imp.SaveAndReimport();
                clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);
            }
            MusicTrackSO track = AssetDatabase.LoadAssetAtPath<MusicTrackSO>(TracksDir + "/MV_Music_T09_NightMarket.asset");
            if (track == null) return false;
            track.clip = clip;
            track.isDormant = false;
            EditorUtility.SetDirty(track);
            MusicLibrarySO lib = AssetDatabase.LoadAssetAtPath<MusicLibrarySO>(LibraryPath);
            if (lib != null) EditorUtility.SetDirty(lib);
            return true;
        }
        private static int CountMissingScriptsInScene(string scenePath) {
            Scene sc = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            int missing = 0;
            GameObject[] roots = sc.GetRootGameObjects();
            for (int r = 0; r < roots.Length; r++) {
                Transform[] all = roots[r].GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < all.Length; i++) {
                    int cnt = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(all[i].gameObject);
                    if (cnt > 0) {
                        Debug.LogError("[MV][Step49][FAIL] Missing Script trong " + sc.name + " tren: " + all[i].name);
                        missing += cnt;
                    }
                }
            }
            return missing;
        }
        private static void EnsureFolder(string assetPath) {
            string[] parts = assetPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++) {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
