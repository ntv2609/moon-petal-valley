using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Combat;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step8
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string EnemySlimeDataPath = "Assets/Data/Enemies/Enemy_GloomSlime.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LootDropPrefabPath = "Assets/Prefabs/Enemies/LootDrop.prefab";
        private const string GloomSlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";

        [MenuItem("Moonpetal/Step 8/1. Build Combat, Slimes & Encounters")]
        public static void BuildStep8()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 8 ===");

            Sprite slimeSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Enemies/spr_gloom_slime.png", 16, 16, DrawGloomSlime);
            Sprite slashSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/UI/spr_slash_arc.png", 32, 32, DrawSlashArc);
            Sprite pulseSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/UI/spr_pulse_ring.png", 48, 48, DrawPulseRing);
            Sprite catSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_miu_cat.png", 16, 16, DrawMiuCat);
            Sprite hpPixel = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/UI/spr_white_pixel.png", 4, 4, t => PlaceholderGeneratorUtility.FillRect(t, 0, 0, 4, 4, Color.white));

            EnemyData slimeData = EnsureGloomSlimeData();
            GameObject slimePrefab = BuildGloomSlimePrefab(slimeSpr, hpPixel, slimeData);

            UpgradePlayerPrefabWithCombat(slashSpr, pulseSpr);
            UpgradeGameRootDebugWithSickle();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildCombatEncounters(slimePrefab, catSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 8! Chạy 'Moonpetal/Step 8/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 8/Validate")]
        public static void ValidateStep8()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 8 ===");

            if (AssetDatabase.LoadAssetAtPath<EnemyData>(EnemySlimeDataPath) == null)
            {
                Debug.LogError("[MV] THIẾU Enemy_GloomSlime.asset");
                errors++;
            }
            if (AssetDatabase.LoadAssetAtPath<GameObject>(GloomSlimePrefabPath) == null)
            {
                Debug.LogError("[MV] THIẾU GloomSlime.prefab");
                errors++;
            }
            if (UnityEngine.Object.FindAnyObjectByType<PlayerCombatController>() == null)
            {
                Debug.LogError("[MV] Player_Wen thiếu PlayerCombatController.");
                errors++;
            }

            var zones = UnityEngine.Object.FindObjectsByType<BlightEncounterZone>(FindObjectsInactive.Include);
            if (zones.Length < 2)
            {
                Debug.LogError("[MV] Thiếu BlightEncounterZone (cần 2, hiện có " + zones.Length + ").");
                errors++;
            }

            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            if (slimes.Length < 8)
            {
                Debug.LogError("[MV] Thiếu GloomSlime trong scene (cần 8 = 5 Vườn + 3 Rìa Rừng, hiện có " + slimes.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 8: PASS (0 lỗi). Liềm Ánh Trăng 120°, Lumi Pulse & 8 Gloom Slime sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 8: FAIL (" + errors + " lỗi).");
            }
        }

        private static EnemyData EnsureGloomSlimeData()
        {
            var data = AssetDatabase.LoadAssetAtPath<EnemyData>(EnemySlimeDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<EnemyData>();
                AssetDatabase.CreateAsset(data, EnemySlimeDataPath);
            }

            ItemData sap = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_sap.asset");
            ItemData dust = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_starlight_dust.asset");

            data.enemyID = "enemy_gloom_slime";
            data.enemyNameKey = "name_enemy_gloom_slime";
            data.maxHP = 45;
            data.moveSpeed = 2.8f;
            data.contactDamage = 10;
            data.attackPattern = EnemyAttackPattern.Jump;
            data.detectionRadius = 4.6f;
            data.attackInterval = 1.1f;
            data.appliesSlowOnHit = true;
            data.dropTable = new List<LootDropEntry>
            {
                new LootDropEntry { item = sap, minAmount = 1, maxAmount = 2, dropChance = 1.0f },
                new LootDropEntry { item = dust, minAmount = 1, maxAmount = 1, dropChance = 0.75f }
            };

            EditorUtility.SetDirty(data);
            return data;
        }

        private static GameObject BuildGloomSlimePrefab(Sprite slimeSpr, Sprite hpPixel, EnemyData data)
        {
            GameObject lootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath);

            GameObject root = new GameObject("GloomSlime");
            root.tag = "Enemy";
            root.layer = 9;

            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var col = root.AddComponent<CircleCollider2D>();
            col.radius = 0.42f;

            // Child trigger collider để gây sát thương chạm mà không làm kẹt Player
            GameObject hurtTrigger = new GameObject("ContactHitbox");
            hurtTrigger.layer = 13;
            hurtTrigger.transform.SetParent(root.transform, false);
            var trig = hurtTrigger.AddComponent<CircleCollider2D>();
            trig.radius = 0.48f;
            trig.isTrigger = true;

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(root.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = slimeSpr;
            sr.sortingLayerName = "WorldObjects";

            // Thanh máu nhỏ phía trên đầu Slime
            GameObject hpRoot = new GameObject("HPBarRoot");
            hpRoot.transform.SetParent(root.transform, false);
            hpRoot.transform.localPosition = new Vector3(0f, 0.65f, 0f);

            GameObject hpBg = new GameObject("Bg");
            hpBg.transform.SetParent(hpRoot.transform, false);
            hpBg.transform.localScale = new Vector3(3.2f, 0.45f, 1f);
            var bgSr = hpBg.AddComponent<SpriteRenderer>();
            bgSr.sprite = hpPixel; bgSr.color = new Color(0.12f, 0.08f, 0.16f, 0.85f);
            bgSr.sortingLayerName = "UI"; bgSr.sortingOrder = 20;

            GameObject hpFill = new GameObject("Fill");
            hpFill.transform.SetParent(hpRoot.transform, false);
            hpFill.transform.localScale = new Vector3(3.0f, 0.32f, 1f);
            var fillSr = hpFill.AddComponent<SpriteRenderer>();
            fillSr.sprite = hpPixel; fillSr.color = new Color(0.86f, 0.28f, 0.56f, 1f);
            fillSr.sortingLayerName = "UI"; fillSr.sortingOrder = 21;

            var ctrl = root.AddComponent<GloomSlimeController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("enemyData").objectReferenceValue = data;
            so.FindProperty("rb").objectReferenceValue = rb;
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.FindProperty("hpBarFill").objectReferenceValue = hpFill.transform;
            so.FindProperty("hpBarRoot").objectReferenceValue = hpRoot;
            so.FindProperty("lootDropPrefab").objectReferenceValue = lootPrefab;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, GloomSlimePrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static void UpgradePlayerPrefabWithCombat(Sprite slashSpr, Sprite pulseSpr)
        {
            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null) return;

            Transform oldSlash = root.transform.Find("SlashArcVisual");
            if (oldSlash != null) UnityEngine.Object.DestroyImmediate(oldSlash.gameObject);
            GameObject slashGo = new GameObject("SlashArcVisual");
            slashGo.transform.SetParent(root.transform, false);
            var slashSr = slashGo.AddComponent<SpriteRenderer>();
            slashSr.sprite = slashSpr;
            slashSr.sortingLayerName = "WeatherVFX";
            slashSr.sortingOrder = 30;
            slashSr.enabled = false;

            Transform oldPulse = root.transform.Find("PulseRingVisual");
            if (oldPulse != null) UnityEngine.Object.DestroyImmediate(oldPulse.gameObject);
            GameObject pulseGo = new GameObject("PulseRingVisual");
            pulseGo.transform.SetParent(root.transform, false);
            var pulseSr = pulseGo.AddComponent<SpriteRenderer>();
            pulseSr.sprite = pulseSpr;
            pulseSr.sortingLayerName = "WeatherVFX";
            pulseSr.sortingOrder = 28;
            pulseSr.enabled = false;

            var combat = root.GetComponent<PlayerCombatController>() ?? root.AddComponent<PlayerCombatController>();
            var so = new SerializedObject(combat);
            so.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            so.FindProperty("playerController").objectReferenceValue = root.GetComponent<PlayerController2D>();
            so.FindProperty("inputReader").objectReferenceValue = root.GetComponent<GameInputReader>();
            so.FindProperty("slashArcTransform").objectReferenceValue = slashGo.transform;
            so.FindProperty("slashArcRenderer").objectReferenceValue = slashSr;
            so.FindProperty("pulseRingTransform").objectReferenceValue = pulseGo.transform;
            so.FindProperty("pulseRingRenderer").objectReferenceValue = pulseSr;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void UpgradeGameRootDebugWithSickle()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 960f);
                Transform ex = debugPanel.Find("BtnGiveSickle");
                Button btn;
                if (ex != null) btn = ex.GetComponent<Button>();
                else
                {
                    GameObject go = new GameObject("BtnGiveSickle");
                    go.transform.SetParent(debugPanel, false);
                    go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
                    var img = go.AddComponent<Image>(); img.color = new Color(0.28f, 0.24f, 0.56f, 1f);
                    btn = go.AddComponent<Button>(); btn.targetGraphic = img;
                    GameObject txtGo = new GameObject("Label");
                    txtGo.transform.SetParent(go.transform, false);
                    var r = txtGo.AddComponent<RectTransform>();
                    r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
                    r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
                    var tmp = txtGo.AddComponent<TextMeshProUGUI>();
                    tmp.text = "Trao Liềm Ánh Trăng (Ô 4)";
                    tmp.fontSize = 15; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
                    if (vnFont != null) tmp.font = vnFont;
                }

                var so = new SerializedObject(debugCtrl);
                so.FindProperty("btnGiveSickle").objectReferenceValue = btn;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildCombatEncounters(GameObject slimePrefab, Sprite catSpr)
        {
            GameObject old = GameObject.Find("CombatEncounters");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject root = new GameObject("CombatEncounters");

            // 1. Cụm 5 Slime trong Vườn Ông Bách
            CreateEncounter(root.transform, "Encounter_BachGarden", "enc_bach_garden",
                "flag_garden_slimes_cleared",
                "Đã thanh tẩy hết 5 Bọt Sương Đen trong Vườn Ông Bách! Hãy bấm [E] lau sạch Chiếc Ghế Đôi!",
                slimePrefab,
                new[]
                {
                    new Vector2(-3.5f, -6.5f),
                    new Vector2(3.5f, -6.5f),
                    new Vector2(-3.8f, -10.5f),
                    new Vector2(3.8f, -10.5f),
                    new Vector2(0.0f, -12.0f)
                });

            // 2. Cụm 3 Slime vây Chú Mèo Miu ở Rìa Rừng Phía Đông
            GameObject forestEnc = CreateEncounter(root.transform, "Encounter_ForestCat", "enc_forest_cat",
                "flag_toby_cat_slimes_cleared",
                "Đã giải cứu chú mèo Miu khỏi 3 Bọt Sương Đen! Hãy về gặp Cậu Bé Toby [E]!",
                slimePrefab,
                new[]
                {
                    new Vector2(16.0f, 5.5f),
                    new Vector2(18.5f, 7.5f),
                    new Vector2(16.5f, 9.2f)
                });

            // Đặt Chú Mèo Miu dễ thương ở giữa cụm rừng
            GameObject catGo = new GameObject("MiuCat_Toby");
            catGo.transform.SetParent(forestEnc.transform, false);
            catGo.transform.position = new Vector3(17.5f, 7.2f, 0f);
            var catSr = catGo.AddComponent<SpriteRenderer>();
            catSr.sprite = catSpr;
            catSr.sortingLayerName = "WorldObjects";
            catSr.sortingOrder = Mathf.RoundToInt(-7.2f * 100f);
        }

        private static GameObject CreateEncounter(
            Transform parent, string name, string encId, string flag, string toast, GameObject slimePrefab, Vector2[] positions)
        {
            GameObject zoneGo = new GameObject(name);
            zoneGo.transform.SetParent(parent, false);
            var zone = zoneGo.AddComponent<BlightEncounterZone>();
            var spawned = new List<GloomSlimeController>();

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject sGo = (GameObject)PrefabUtility.InstantiatePrefab(slimePrefab);
                sGo.name = "GloomSlime_" + (i + 1);
                sGo.transform.SetParent(zoneGo.transform, false);
                sGo.transform.position = new Vector3(positions[i].x, positions[i].y, 0f);

                var ctrl = sGo.GetComponent<GloomSlimeController>();
                var cSO = new SerializedObject(ctrl);
                cSO.FindProperty("encounterZone").objectReferenceValue = zone;
                cSO.ApplyModifiedPropertiesWithoutUndo();
                spawned.Add(ctrl);
            }

            var zSO = new SerializedObject(zone);
            zSO.FindProperty("encounterId").stringValue = encId;
            zSO.FindProperty("completionFlag").stringValue = flag;
            zSO.FindProperty("completionToast").stringValue = toast;
            var prop = zSO.FindProperty("slimes");
            prop.ClearArray();
            for (int i = 0; i < spawned.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = spawned[i];
            }
            zSO.ApplyModifiedPropertiesWithoutUndo();
            return zoneGo;
        }

        private static void DrawGloomSlime(Texture2D t)
        {
            Color dark = new Color(0.20f, 0.14f, 0.30f, 1f);
            Color mid = new Color(0.34f, 0.22f, 0.48f, 1f);
            Color eye = new Color(1.0f, 0.88f, 0.30f, 1f);
            PlaceholderGeneratorUtility.FillRect(t, 3, 2, 10, 8, dark);
            PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 6, dark);
            PlaceholderGeneratorUtility.FillRect(t, 4, 4, 8, 6, mid);
            PlaceholderGeneratorUtility.FillRect(t, 5, 10, 6, 2, mid);
            PlaceholderGeneratorUtility.FillRect(t, 4, 6, 2, 2, eye);
            PlaceholderGeneratorUtility.FillRect(t, 10, 6, 2, 2, eye);
        }

        private static void DrawSlashArc(Texture2D t)
        {
            Vector2 c = new Vector2(10f, 16f);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    Vector2 p = new Vector2(x, y) - c;
                    float dist = p.magnitude;
                    float ang = Mathf.Abs(Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg);
                    if (dist >= 10f && dist <= 17f && ang <= 62f)
                    {
                        float fade = 1f - Mathf.Abs(dist - 13.5f) / 3.8f;
                        t.SetPixel(x, y, new Color(0.72f, 0.98f, 1f, Mathf.Clamp01(fade)));
                    }
                }
            }
        }

        private static void DrawPulseRing(Texture2D t)
        {
            Vector2 c = new Vector2(24f, 24f);
            for (int y = 0; y < 48; y++)
            {
                for (int x = 0; x < 48; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), c);
                    if (d >= 18f && d <= 23f)
                    {
                        float a = 1f - Mathf.Abs(d - 20.5f) / 2.8f;
                        t.SetPixel(x, y, new Color(0.62f, 0.98f, 0.96f, Mathf.Clamp01(a)));
                    }
                }
            }
        }

        private static void DrawMiuCat(Texture2D t)
        {
            Color fur = new Color(0.95f, 0.66f, 0.34f);
            Color cream = new Color(0.99f, 0.92f, 0.78f);
            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 8, 7, fur);
            PlaceholderGeneratorUtility.FillRect(t, 5, 3, 6, 4, cream);
            PlaceholderGeneratorUtility.FillRect(t, 4, 9, 2, 3, fur);
            PlaceholderGeneratorUtility.FillRect(t, 10, 9, 2, 3, fur);
            t.SetPixel(6, 7, Color.black);
            t.SetPixel(9, 7, Color.black);
            PlaceholderGeneratorUtility.FillRect(t, 12, 4, 2, 4, fur);
        }
    }
}
