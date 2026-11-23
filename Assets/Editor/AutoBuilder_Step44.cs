using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step44
    {
        private const string Ch2ScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string Ch2DataRoot = "Assets/Data/Chapter2";
        private const string EnemiesDataRoot = "Assets/Data/Enemies";
        private const string EnemiesPrefabRoot = "Assets/Prefabs/Enemies";
        private const string CharsRoot = "Assets/Art/Characters";
        private const string VNTablePath = "Assets/Data/Localization/VNStringTable.asset";

        private const string ThornCrawlerDir = CharsRoot + "/enemy_thorn_crawler";
        private const string BigSlimeDir = CharsRoot + "/enemy_big_gloom_slime";
        private const string ThornCrawlerDataPath = EnemiesDataRoot + "/Enemy_ThornCrawler.asset";
        private const string BigSlimeDataPath = EnemiesDataRoot + "/Enemy_BigGloomSlime.asset";
        private const string ThornCrawlerPrefabPath = EnemiesPrefabRoot + "/Enemy_ThornCrawler.prefab";
        private const string BigSlimePrefabPath = EnemiesPrefabRoot + "/Enemy_BigGloomSlime.prefab";
        private const string GloomSlimePrefabPath = EnemiesPrefabRoot + "/GloomSlime.prefab";
        private const string LootDropPrefabPath = EnemiesPrefabRoot + "/LootDrop.prefab";

        private const string Echo1AssetPath = Ch2DataRoot + "/Echo_Ch2_StormLantern.asset";
        private const string Echo2AssetPath = Ch2DataRoot + "/Echo_Ch2_BrokenPier.asset";
        private const string DlgPostEcho1Path = Ch2DataRoot + "/Dialogues/Dlg_Ch2_PostEcho1.asset";
        private const string DlgPostEcho2Path = Ch2DataRoot + "/Dialogues/Dlg_Ch2_PostEcho2.asset";

        private const string WhitePixelPath = "Assets/Art/Sprites/UI/spr_white_pixel.png";
        private const string PulseRingPath = "Assets/Art/Sprites/UI/spr_pulse_ring.png";
        private const string ShadowPath = "Assets/Art/Sprites/Environment/Shadows/spr_ground_shadow.png";
        private const string StormLanternItemPath = "Assets/Data/Items/Item_item_storm_lantern_west.asset";

        [MenuItem("Moonpetal/Step 44/1. Build Swamp Combat & 2 Echo Spots (Ch2)")]
        public static void BuildStep44()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }
            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 44 (FIX 2: SPRITE GỐC PPU 32 + COMBAT JUICE CH1) ===");
            EnsureFolder(EnemiesDataRoot);
            EnsureFolder(EnemiesPrefabRoot);
            EnsureFolder(Ch2DataRoot + "/Dialogues");

            ConfigureEnemyFolderPPU32(ThornCrawlerDir);
            ConfigureEnemyFolderPPU32(BigSlimeDir);
            AssetDatabase.Refresh();

            var thornFrames = DiscoverEnemyFrames(ThornCrawlerDir);
            var bigSlimeFrames = DiscoverEnemyFrames(BigSlimeDir);
            EnemyData thornData = EnsureThornCrawlerData();
            EnemyData bigSlimeData = EnsureBigGloomSlimeData();

            GameObject thornPrefab = BuildThornCrawlerPrefab(thornData, thornFrames);
            GameObject bigSlimePrefab = BuildBigGloomSlimePrefab(bigSlimeData, bigSlimeFrames);
            DialogueData postEcho1 = EnsurePostEcho1Dialogue();
            DialogueData postEcho2 = EnsurePostEcho2Dialogue();

            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            BuildSwampZoneInScene(thornPrefab, bigSlimePrefab);
            BuildEchoSpotsInScene(postEcho1, postEcho2);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 44 (FIX 2)! Chạy 'Moonpetal/Step 44/2. Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 44/2. Validate")]
        public static void ValidateStep44()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 44 (FIX 2) ===");
            var scene = EditorSceneManager.OpenScene(Ch2ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid()) { Debug.LogError("[MV] Không mở được scene " + Ch2ScenePath); return; }

            errors += AuditEnemyPrefab(ThornCrawlerPrefabPath, typeof(ThornCrawlerController), "Enemy_ThornCrawler");
            errors += AuditEnemyPrefab(BigSlimePrefabPath, typeof(BigGloomSlimeController), "Enemy_BigGloomSlime");

            if (UnityEngine.Object.FindAnyObjectByType<SwampCombatZoneController>() == null)
            { Debug.LogError("[MV] THIẾU SwampCombatZoneController trong Riverbank_Chapter2."); errors++; }
            var crawlers = UnityEngine.Object.FindObjectsByType<ThornCrawlerController>(FindObjectsInactive.Include);
            if (crawlers.Length < 3) { Debug.LogError("[MV] Thiếu ThornCrawler (cần >= 3, có " + crawlers.Length + ")."); errors++; }
            var bigSlimes = UnityEngine.Object.FindObjectsByType<BigGloomSlimeController>(FindObjectsInactive.Include);
            if (bigSlimes.Length < 1) { Debug.LogError("[MV] Thiếu BigGloomSlime (cần >= 1)."); errors++; }
            var echoes = UnityEngine.Object.FindObjectsByType<EchoSpotInteractable>(FindObjectsInactive.Include);
            if (echoes.Length < 2) { Debug.LogError("[MV] Thiếu EchoSpotInteractable (cần >= 2, có " + echoes.Length + ")."); errors++; }
            if (AssetDatabase.LoadAssetAtPath<DialogueData>(DlgPostEcho1Path) == null) { Debug.LogError("[MV] THIẾU Dlg_Ch2_PostEcho1.asset"); errors++; }
            if (AssetDatabase.LoadAssetAtPath<DialogueData>(DlgPostEcho2Path) == null) { Debug.LogError("[MV] THIẾU Dlg_Ch2_PostEcho2.asset"); errors++; }

            if (errors == 0) Debug.Log("[MV] VALIDATE STEP 44 (FIX 2): PASS (0 lỗi). Sprite gốc PPU=32, không Animator, Combat Juice & 2 Echo sẵn sàng!");
            else Debug.LogError("[MV] VALIDATE STEP 44 (FIX 2): FAIL (" + errors + " lỗi).");
        }

        private struct EnemyFrameSet
        {
            public Sprite[] idle, moveOrBurrow, telegraph, attack, hit, die;
        }

        private static void ConfigureEnemyFolderPPU32(string assetFolder)
        {
            if (!AssetDatabase.IsValidFolder(assetFolder)) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { assetFolder });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) continue;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool changed = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; changed = true; }
                if (importer.spriteImportMode != SpriteImportMode.Single) { importer.spriteImportMode = SpriteImportMode.Single; changed = true; }
                if (Mathf.Abs(importer.spritePixelsPerUnit - 32f) > 0.01f) { importer.spritePixelsPerUnit = 32f; changed = true; }
                if (importer.filterMode != FilterMode.Point) { importer.filterMode = FilterMode.Point; changed = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; changed = true; }
                if (importer.mipmapEnabled) { importer.mipmapEnabled = false; changed = true; }

                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                if (settings.spriteMeshType != SpriteMeshType.FullRect)
                {
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    importer.SetTextureSettings(settings);
                    changed = true;
                }
                if (changed) importer.SaveAndReimport();
            }
        }

        private static EnemyFrameSet DiscoverEnemyFrames(string assetFolder)
        {
            var all = new List<(string path, string lowerName, Sprite sprite)>();
            if (AssetDatabase.IsValidFolder(assetFolder))
            {
                string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { assetFolder });
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (spr != null) all.Add((path, Path.GetFileNameWithoutExtension(path).ToLowerInvariant(), spr));
                }
            }
            all.Sort((a, b) => string.Compare(a.path, b.path, StringComparison.OrdinalIgnoreCase));

            Sprite[] Filter(params string[] keywords)
            {
                var list = new List<Sprite>();
                for (int i = 0; i < all.Count; i++)
                    for (int k = 0; k < keywords.Length; k++)
                        if (all[i].lowerName.Contains(keywords[k])) { list.Add(all[i].sprite); break; }
                return list.ToArray();
            }

            Sprite[] idle = Filter("idle");
            if (idle.Length == 0) idle = Filter("south", "east", "front");
            if (idle.Length == 0 && all.Count > 0) idle = new[] { all[0].sprite };

            Sprite[] move = Filter("burrow", "dig", "move", "walk", "jump"); if (move.Length == 0) move = idle;
            Sprite[] tele = Filter("telegraph", "charge", "windup"); if (tele.Length == 0) tele = idle;
            Sprite[] atk = Filter("attack", "emerge", "slam", "wave", "lunge"); if (atk.Length == 0) atk = idle;
            Sprite[] hit = Filter("hit", "hurt", "damage"); if (hit.Length == 0) hit = idle;
            Sprite[] die = Filter("die", "purify", "death"); if (die.Length == 0) die = idle;

            Debug.Log("[MV] Frame discovery " + assetFolder + ": total=" + all.Count
                + " | idle=" + idle.Length + " move=" + move.Length + " tele=" + tele.Length
                + " atk=" + atk.Length + " hit=" + hit.Length + " die=" + die.Length);
            return new EnemyFrameSet { idle = idle, moveOrBurrow = move, telegraph = tele, attack = atk, hit = hit, die = die };
        }

        private static EnemyData EnsureThornCrawlerData()
        {
            var data = AssetDatabase.LoadAssetAtPath<EnemyData>(ThornCrawlerDataPath);
            if (data == null) { data = ScriptableObject.CreateInstance<EnemyData>(); AssetDatabase.CreateAsset(data, ThornCrawlerDataPath); }
            ItemData sap = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_sap.asset");
            ItemData dust = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_starlight_dust.asset");

            data.enemyID = "enemy_thorn_crawler"; data.enemyNameKey = "name_enemy_thorn_crawler";
            data.maxHP = 55; data.moveSpeed = 2.6f; data.contactDamage = 12;
            data.attackPattern = EnemyAttackPattern.Burrow;
            data.detectionRadius = 5.2f; data.attackInterval = 1.25f; data.appliesSlowOnHit = true;
            data.burrowDuration = 1.15f; data.telegraphDuration = 0.52f;
            data.dropTable = new List<LootDropEntry>();
            if (sap != null) data.dropTable.Add(new LootDropEntry { item = sap, minAmount = 1, maxAmount = 2, dropChance = 1f });
            if (dust != null) data.dropTable.Add(new LootDropEntry { item = dust, minAmount = 1, maxAmount = 1, dropChance = 0.7f });
            EditorUtility.SetDirty(data);
            return data;
        }

        private static EnemyData EnsureBigGloomSlimeData()
        {
            var data = AssetDatabase.LoadAssetAtPath<EnemyData>(BigSlimeDataPath);
            if (data == null) { data = ScriptableObject.CreateInstance<EnemyData>(); AssetDatabase.CreateAsset(data, BigSlimeDataPath); }
            ItemData westLantern = AssetDatabase.LoadAssetAtPath<ItemData>(StormLanternItemPath);
            ItemData dust = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_starlight_dust.asset");

            data.enemyID = "enemy_big_gloom_slime"; data.enemyNameKey = "name_enemy_big_gloom_slime";
            data.maxHP = 120; data.moveSpeed = 1.75f; data.contactDamage = 15;
            data.attackPattern = EnemyAttackPattern.BigSlimeWave;
            data.detectionRadius = 6.0f; data.attackInterval = 1.6f; data.appliesSlowOnHit = true;
            data.telegraphDuration = 0.70f; data.splitHpRatio = 0.5f; data.mudWaveRadius = 2.5f;
            data.dropTable = new List<LootDropEntry>();
            if (westLantern != null) data.dropTable.Add(new LootDropEntry { item = westLantern, minAmount = 1, maxAmount = 1, dropChance = 1f });
            if (dust != null) data.dropTable.Add(new LootDropEntry { item = dust, minAmount = 2, maxAmount = 3, dropChance = 1f });
            EditorUtility.SetDirty(data);
            return data;
        }

        private static GameObject BuildThornCrawlerPrefab(EnemyData data, EnemyFrameSet frames)
        {
            Sprite hpPixel = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePixelPath);
            Sprite pulseSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);
            Sprite shadowSpr = AssetDatabase.LoadAssetAtPath<Sprite>(ShadowPath);
            GameObject lootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath);

            GameObject root = new GameObject("Enemy_ThornCrawler"); root.tag = "Enemy"; root.layer = 9;
            var rb = root.AddComponent<Rigidbody2D>(); rb.gravityScale = 0f; rb.freezeRotation = true;
            root.AddComponent<CircleCollider2D>().radius = 0.40f;

            GameObject hurtTrigger = new GameObject("ContactHitbox"); hurtTrigger.layer = 13; hurtTrigger.transform.SetParent(root.transform, false);
            var trig = hurtTrigger.AddComponent<CircleCollider2D>(); trig.radius = 0.46f; trig.isTrigger = true;

            GameObject shadowGo = BuildGroundShadow(root.transform, shadowSpr, new Vector3(0f, -0.34f, 0f), new Vector3(0.95f, 0.75f, 1f));
            SpriteRenderer ringSr = BuildRingVfx(root.transform, "TelegraphRing", pulseSpr, new Vector3(0f, -0.15f, 0f));

            GameObject vis = new GameObject("Visual"); vis.transform.SetParent(root.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = (frames.idle != null && frames.idle.Length > 0) ? frames.idle[0] : null;
            sr.sortingLayerName = "WorldObjects";

            BuildTwoLayerHpBar(root.transform, hpPixel, 0.72f, 3.2f, 3.0f, 0.45f, 0.32f, out GameObject hpRoot, out Transform hpFill);

            var ctrl = root.AddComponent<ThornCrawlerController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("enemyData").objectReferenceValue = data;
            so.FindProperty("rb").objectReferenceValue = rb;
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.FindProperty("hpBarFill").objectReferenceValue = hpFill;
            so.FindProperty("hpBarRoot").objectReferenceValue = hpRoot;
            so.FindProperty("lootDropPrefab").objectReferenceValue = lootPrefab;
            so.FindProperty("groundShadow").objectReferenceValue = shadowGo;
            so.FindProperty("telegraphRingRenderer").objectReferenceValue = ringSr;
            AssignSpriteArray(so.FindProperty("idleFrames"), frames.idle);
            AssignSpriteArray(so.FindProperty("burrowFrames"), frames.moveOrBurrow);
            AssignSpriteArray(so.FindProperty("telegraphFrames"), frames.telegraph);
            AssignSpriteArray(so.FindProperty("attackFrames"), frames.attack);
            AssignSpriteArray(so.FindProperty("hitFrames"), frames.hit);
            AssignSpriteArray(so.FindProperty("dieFrames"), frames.die);
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, ThornCrawlerPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static GameObject BuildBigGloomSlimePrefab(EnemyData data, EnemyFrameSet frames)
        {
            Sprite hpPixel = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePixelPath);
            Sprite pulseSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);
            Sprite shadowSpr = AssetDatabase.LoadAssetAtPath<Sprite>(ShadowPath);
            GameObject lootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath);
            GameObject miniSlime = AssetDatabase.LoadAssetAtPath<GameObject>(GloomSlimePrefabPath);

            GameObject root = new GameObject("Enemy_BigGloomSlime"); root.tag = "Enemy"; root.layer = 9;
            var rb = root.AddComponent<Rigidbody2D>(); rb.gravityScale = 0f; rb.freezeRotation = true; rb.mass = 3f;
            root.AddComponent<CircleCollider2D>().radius = 0.62f;

            GameObject hurtTrigger = new GameObject("ContactHitbox"); hurtTrigger.layer = 13; hurtTrigger.transform.SetParent(root.transform, false);
            var trig = hurtTrigger.AddComponent<CircleCollider2D>(); trig.radius = 0.68f; trig.isTrigger = true;

            GameObject shadowGo = BuildGroundShadow(root.transform, shadowSpr, new Vector3(0f, -0.52f, 0f), new Vector3(1.55f, 1.15f, 1f));
            SpriteRenderer ringSr = BuildRingVfx(root.transform, "WaveRing", pulseSpr, new Vector3(0f, -0.25f, 0f));

            GameObject vis = new GameObject("Visual"); vis.transform.SetParent(root.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = (frames.idle != null && frames.idle.Length > 0) ? frames.idle[0] : null;
            sr.sortingLayerName = "WorldObjects";

            BuildTwoLayerHpBar(root.transform, hpPixel, 1.05f, 4.4f, 4.2f, 0.50f, 0.36f, out GameObject hpRoot, out Transform hpFill);

            var ctrl = root.AddComponent<BigGloomSlimeController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("enemyData").objectReferenceValue = data;
            so.FindProperty("rb").objectReferenceValue = rb;
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.FindProperty("hpBarFill").objectReferenceValue = hpFill;
            so.FindProperty("hpBarRoot").objectReferenceValue = hpRoot;
            so.FindProperty("lootDropPrefab").objectReferenceValue = lootPrefab;
            so.FindProperty("miniSlimePrefab").objectReferenceValue = miniSlime;
            so.FindProperty("groundShadow").objectReferenceValue = shadowGo;
            so.FindProperty("waveRingRenderer").objectReferenceValue = ringSr;
            AssignSpriteArray(so.FindProperty("idleFrames"), frames.idle);
            AssignSpriteArray(so.FindProperty("moveFrames"), frames.moveOrBurrow);
            AssignSpriteArray(so.FindProperty("telegraphFrames"), frames.telegraph);
            AssignSpriteArray(so.FindProperty("attackFrames"), frames.attack);
            AssignSpriteArray(so.FindProperty("hitFrames"), frames.hit);
            AssignSpriteArray(so.FindProperty("dieFrames"), frames.die);
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, BigSlimePrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static GameObject BuildGroundShadow(Transform parent, Sprite shadowSpr, Vector3 localPos, Vector3 scale)
        {
            GameObject sh = new GameObject("GroundShadow");
            sh.transform.SetParent(parent, false); sh.transform.localPosition = localPos; sh.transform.localScale = scale;
            var sr = sh.AddComponent<SpriteRenderer>();
            sr.sprite = shadowSpr; sr.color = new Color(0.08f, 0.06f, 0.14f, 0.45f);
            sr.sortingLayerName = "GroundDecor"; sr.sortingOrder = 10;
            return sh;
        }

        private static SpriteRenderer BuildRingVfx(Transform parent, string name, Sprite pulseSpr, Vector3 localPos)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false); go.transform.localPosition = localPos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = pulseSpr; sr.sortingLayerName = "GroundDecor"; sr.sortingOrder = 15; sr.enabled = false;
            return sr;
        }

        private static void BuildTwoLayerHpBar(Transform parent, Sprite hpPixel, float yOffset,
            float bgW, float fillW, float bgH, float fillH, out GameObject hpRoot, out Transform hpFill)
        {
            hpRoot = new GameObject("HPBarRoot");
            hpRoot.transform.SetParent(parent, false); hpRoot.transform.localPosition = new Vector3(0f, yOffset, 0f);

            GameObject hpBg = new GameObject("Bg");
            hpBg.transform.SetParent(hpRoot.transform, false); hpBg.transform.localScale = new Vector3(bgW, bgH, 1f);
            var bgSr = hpBg.AddComponent<SpriteRenderer>();
            bgSr.sprite = hpPixel; bgSr.color = new Color(0.12f, 0.08f, 0.16f, 0.85f);
            bgSr.sortingLayerName = "UI"; bgSr.sortingOrder = 20;

            GameObject hpFillGo = new GameObject("Fill");
            hpFillGo.transform.SetParent(hpRoot.transform, false); hpFillGo.transform.localScale = new Vector3(fillW, fillH, 1f);
            var fillSr = hpFillGo.AddComponent<SpriteRenderer>();
            fillSr.sprite = hpPixel; fillSr.color = new Color(0.86f, 0.28f, 0.56f, 1f);
            fillSr.sortingLayerName = "UI"; fillSr.sortingOrder = 21;
            hpFill = hpFillGo.transform;
        }

        private static void BuildSwampZoneInScene(GameObject thornPrefab, GameObject bigSlimePrefab)
        {
            GameObject oldZone = GameObject.Find("SwampCombatZone");
            if (oldZone != null) UnityEngine.Object.DestroyImmediate(oldZone);

            GameObject zoneGo = new GameObject("SwampCombatZone");
            zoneGo.transform.position = new Vector3(-14.5f, 6.2f, 0f);
            var zoneCtrl = zoneGo.AddComponent<SwampCombatZoneController>();

            var tracked = new List<GameObject>();
            Vector3[] crawlerPos = { new Vector3(-12.2f, 4.6f, 0f), new Vector3(-16.4f, 5.4f, 0f), new Vector3(-14.0f, 8.2f, 0f) };
            for (int i = 0; i < crawlerPos.Length; i++)
            {
                GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(thornPrefab);
                inst.name = "ThornCrawler_" + (i + 1);
                inst.transform.SetParent(zoneGo.transform, true); inst.transform.position = crawlerPos[i];
                var c = inst.GetComponent<ThornCrawlerController>();
                if (c != null) { var cso = new SerializedObject(c); cso.FindProperty("combatZone").objectReferenceValue = zoneCtrl; cso.ApplyModifiedPropertiesWithoutUndo(); }
                tracked.Add(inst);
            }

            GameObject bossInst = (GameObject)PrefabUtility.InstantiatePrefab(bigSlimePrefab);
            bossInst.name = "BigGloomSlime_WestSwamp";
            bossInst.transform.SetParent(zoneGo.transform, true); bossInst.transform.position = new Vector3(-17.2f, 7.6f, 0f);
            var bc = bossInst.GetComponent<BigGloomSlimeController>();
            if (bc != null) { var bso = new SerializedObject(bc); bso.FindProperty("combatZone").objectReferenceValue = zoneCtrl; bso.ApplyModifiedPropertiesWithoutUndo(); }
            tracked.Add(bossInst);

            ItemData westLantern = AssetDatabase.LoadAssetAtPath<ItemData>(StormLanternItemPath);
            var zso = new SerializedObject(zoneCtrl);
            zso.FindProperty("zoneClearedFlag").stringValue = "flag_ch2_swamp_cleared";
            zso.FindProperty("westLanternRewardItem").objectReferenceValue = westLantern;
            var listProp = zso.FindProperty("trackedEnemies"); listProp.ClearArray();
            for (int i = 0; i < tracked.Count; i++) { listProp.InsertArrayElementAtIndex(i); listProp.GetArrayElementAtIndex(i).objectReferenceValue = tracked[i]; }
            zso.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildEchoSpotsInScene(DialogueData postEcho1, DialogueData postEcho2)
        {
            EchoData echo1 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo1AssetPath);
            EchoData echo2 = AssetDatabase.LoadAssetAtPath<EchoData>(Echo2AssetPath);
            Sprite pulseSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PulseRingPath);

            CreateOrUpdateEchoSpot("EchoSpot_StormLantern", new Vector3(-17.2f, 8.6f, 0f), echo1, postEcho1,
                "Dùng Ánh Soi lên Nửa Đèn Bão Cổ", "Cần lấy Nửa Đèn Bão Cổ (Tây) từ Slime Sương Lớn trước.", pulseSpr);
            CreateOrUpdateEchoSpot("EchoSpot_BrokenPier", new Vector3(-8.5f, 0.3f, 0f), echo2, postEcho2,
                "Dùng Ánh Soi tại Mố Cầu Bờ Tây", "Hãy soi Dư Âm Nửa Đèn Bão Cổ ở Đầm Lầy trước.", pulseSpr);
        }

        private static void CreateOrUpdateEchoSpot(string objName, Vector3 pos, EchoData data, DialogueData postDlg,
            string prompt, string blocked, Sprite shimmerSpr)
        {
            GameObject old = GameObject.Find(objName);
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject go = new GameObject(objName); go.transform.position = pos;
            GameObject shimmer = new GameObject("ShimmerVFX");
            shimmer.transform.SetParent(go.transform, false); shimmer.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
            var sr = shimmer.AddComponent<SpriteRenderer>();
            sr.sprite = shimmerSpr; sr.color = new Color(0.62f, 0.94f, 1f, 0.55f);
            sr.sortingLayerName = "GroundDecor"; sr.sortingOrder = 16;

            var comp = go.AddComponent<EchoSpotInteractable>();
            var so = new SerializedObject(comp);
            so.FindProperty("echoData").objectReferenceValue = data;
            so.FindProperty("promptLabel").stringValue = prompt;
            so.FindProperty("blockedHint").stringValue = blocked;
            so.FindProperty("interactRadius").floatValue = 2.3f;
            so.FindProperty("postEchoDialogue").objectReferenceValue = postDlg;
            so.FindProperty("shimmerRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static DialogueData EnsurePostEcho1Dialogue()
        {
            return CreateOrUpdateDialogue(DlgPostEcho1Path, "dlg_ch2_post_echo1", "flag_ch2_echo1_lantern_done", new[]
            {
                ("Lumi", "lumi", "Ánh đèn trong ký ức ấm quá... Ông Kael và bà Mara từng tự tay làm chiếc đèn này cho Lễ Trao Hạt đầu tiên đấy!"),
                ("Wen", "wen", "Còn một Dư Âm nữa ở mố cầu bờ Tây. Mình ra đó xem chuyện gì đã xảy ra vào đêm bão năm ấy.")
            });
        }

        private static DialogueData EnsurePostEcho2Dialogue()
        {
            return CreateOrUpdateDialogue(DlgPostEcho2Path, "dlg_ch2_post_echo2", "flag_ch2_echo2_pier_done", new[]
            {
                ("Lumi", "lumi", "Ông Kael không hề bỏ mặc bà Mara... Ông chặt cáp để cứu nhà trẻ bờ Đông, rồi lập tức lao ra bến cứu thuyền!"),
                ("Wen", "wen", "Mình đã hiểu trọn vẹn sự thật đêm bão rồi. Mình cần đưa hai người đối mặt để nối lại cây cầu.")
            });
        }

        private static DialogueData CreateOrUpdateDialogue(string assetPath, string dialogueId, string setsFlag,
            (string speakerName, string speakerId, string text)[] lines)
        {
            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(VNTablePath);
            var dlg = AssetDatabase.LoadAssetAtPath<DialogueData>(assetPath);
            if (dlg == null) { dlg = ScriptableObject.CreateInstance<DialogueData>(); AssetDatabase.CreateAsset(dlg, assetPath); }

            dlg.dialogueID = dialogueId; dlg.setsStoryFlagOnFinish = setsFlag;
            dlg.lines = new List<DialogueLine>();
            for (int i = 0; i < lines.Length; i++)
            {
                string spkKey = dialogueId + "_spk_" + i;
                string txtKey = dialogueId + "_txt_" + i;
                if (table != null) { table.SetEntry(spkKey, lines[i].speakerName); table.SetEntry(txtKey, lines[i].text); }
                dlg.lines.Add(new DialogueLine { speakerNameKey = spkKey, textKey = txtKey, speakerId = lines[i].speakerId });
            }
            if (table != null) EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static int AuditEnemyPrefab(string prefabPath, Type controllerType, string label)
        {
            int errs = 0;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) { Debug.LogError("[MV] THIẾU prefab " + prefabPath); return 1; }
            if (prefab.GetComponent<Animator>() != null || prefab.GetComponentInChildren<Animator>() != null)
            { Debug.LogError("[MV] " + label + " vẫn còn Animator (phải gỡ bỏ để không ghi đè SpriteRenderer.sprite)."); errs++; }
            Transform vis = prefab.transform.Find("Visual");
            SpriteRenderer sr = vis != null ? vis.GetComponent<SpriteRenderer>() : null;
            if (sr == null || sr.sprite == null) { Debug.LogError("[MV] " + label + " thiếu SpriteRenderer hoặc sprite null."); errs++; }
            var ctrl = prefab.GetComponent(controllerType);
            if (ctrl == null) { Debug.LogError("[MV] " + label + " thiếu controller " + controllerType.Name); errs++; }
            else
            {
                var idleProp = new SerializedObject(ctrl).FindProperty("idleFrames");
                if (idleProp == null || idleProp.arraySize == 0) { Debug.LogError("[MV] " + label + " có idleFrames rỗng."); errs++; }
            }
            return errs;
        }

        private static void AssignSpriteArray(SerializedProperty prop, Sprite[] sprites)
        {
            if (prop == null) return;
            prop.ClearArray();
            if (sprites == null) return;
            for (int i = 0; i < sprites.Length; i++) { prop.InsertArrayElementAtIndex(i); prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i]; }
        }

        private static void EnsureFolder(string assetPath)
        {
            string[] parts = assetPath.Split('/');
            string cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }
    }
}
