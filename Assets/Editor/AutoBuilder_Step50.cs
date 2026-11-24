using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.Chapter2;
using MoonpetalValley.Chapter3;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step50
    {
        private const string MvpScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string LinhAnimDir = "Assets/Art/Sprites/Animations/NPCs/Linh";
        private const string LinhPortraitPath = "Assets/Art/Sprites/Portraits/portrait_linh.png";
        private const string GrandmaPortraitPath = "Assets/Art/Sprites/Portraits/portrait_grandma_young.png";
        private const string OvenGlowPath = "Assets/Art/Sprites/Chapter3/Props/prop_ch3_echo_oven_glow.png";
        private const string Ch3DataRoot = "Assets/ScriptableObjects/Chapter3";
        private const string Ch3PrefabRoot = "Assets/Prefabs/Chapter3";

        [MenuItem("Moonpetal/Step 50/1. Build NPC Linh, Bakery Echo & 3-Day Prep")]
        public static void BuildStep50()
        {
            EnsureDir(Ch3DataRoot + "/NPCs");
            EnsureDir(Ch3DataRoot + "/Dialogues");
            EnsureDir(Ch3DataRoot + "/Echoes");
            EnsureDir(Ch3PrefabRoot);

            ImportLinhAndGhostArt();
            AssetDatabase.Refresh();

            Sprite[] linhIdle = LoadLinhFrames("idle_");
            Sprite[] linhCheer = LoadLinhFrames("cheer_");
            if (linhCheer.Length == 0) linhCheer = linhIdle;
            Sprite linhPortrait = LoadLinhPortrait(linhIdle);
            Sprite ovenGlow = AssetDatabase.LoadAssetAtPath<Sprite>(OvenGlowPath);

            ItemData riverDaisy = AssetDatabase.LoadAssetAtPath<ItemData>(Ch3DataRoot + "/Items/item_river_daisy.asset");

            // 1. Build 4 stage dialogues for Linh
            var dlgStage1 = CreateOrUpdateDialogue(
                Ch3DataRoot + "/Dialogues/Dlg_Linh_Stage1_Intro.asset",
                "dlg_linh_stage1_intro",
                new[]
                {
                    ("Linh", "linh", "Chào chị Wen! Em là Linh, người sưu tầm thảo mộc vừa qua cầu mới sửa sáng nay."),
                    ("Wen", "wen", "Chào mừng em tới Moonpetal! Nghe nói em muốn góp một gian hàng cho Chợ Đêm?"),
                    ("Linh", "linh", "Dạ vâng! Em hái sẵn ít Cúc Bờ Sông bên bờ nước, chị cầm làm nguyên liệu thử nhé."),
                    ("Lumi", "lumi", "Nhớ ghé lò bánh bà Hana nữa nhé Wen, tớ thấy có ánh Dư Âm ấm áp đang phát sáng ở đó!")
                },
                setsFlag: Chapter3StateManager.FlagCh3Started,
                rewardItem: riverDaisy,
                rewardAmount: 2
            );

            var dlgStage2 = CreateOrUpdateDialogue(
                Ch3DataRoot + "/Dialogues/Dlg_Linh_Stage2_Prep.asset",
                "dlg_linh_stage2_prep",
                new[]
                {
                    ("Linh", "linh", "Mọi người trong làng ai cũng hào hứng chuẩn bị cho Chợ Đêm đầu tiên sau nhiều năm."),
                    ("Linh", "linh", "Chị cứ chế tác hàng ở bàn gỗ nhé, mệt quá thì về ngủ sớm kẻo kiệt sức đó!")
                }
            );

            var dlgStage3 = CreateOrUpdateDialogue(
                Ch3DataRoot + "/Dialogues/Dlg_Linh_Stage3_Day3Ready.asset",
                "dlg_linh_stage3_day3_ready",
                new[]
                {
                    ("Linh", "linh", "Sáng nay cả làng lén dựng xong hết bảy gian hàng trên cầu rồi chị Wen ơi!"),
                    ("Linh", "linh", "Tối nay khi nào sẵn sàng, chị ra cầu bờ sông để khai mạc Chợ Đêm nha!")
                }
            );

            var dlgStage4 = CreateOrUpdateDialogue(
                Ch3DataRoot + "/Dialogues/Dlg_Linh_Stage4_PostCh3.asset",
                "dlg_linh_stage4_post_ch3",
                new[]
                {
                    ("Linh", "linh", "Chợ Đêm hôm ấy ấm áp quá chừng... Em quyết định ở lại Moonpetal mở tiệm thảo mộc luôn!"),
                    ("Wen", "wen", "Thung lũng có thêm em là vui nhất rồi, Linh.")
                }
            );

            // 2. Build NPC_Linh ScriptableObject
            var linhData = CreateOrUpdateAsset<NPCData>(Ch3DataRoot + "/NPCs/NPC_Linh.asset");
            linhData.npcID = "npc_linh";
            linhData.displayNameKey = "Linh";
            linhData.roleTitleKey = "Nhà Thảo Mộc Lữ Hành";
            linhData.portraitSprite = linhPortrait;
            linhData.worldSprite = linhIdle.Length > 0 ? linhIdle[0] : linhPortrait;
            linhData.defaultDialogue = dlgStage1;
            linhData.conditionalDialogues = new List<ConditionalDialogueEntry>
            {
                new ConditionalDialogueEntry { requiredStoryFlag = Chapter3StateManager.FlagCh3Completed, dialogue = dlgStage4 },
                new ConditionalDialogueEntry { requiredStoryFlag = Chapter3StateManager.FlagCh3StallsBuiltDay3, dialogue = dlgStage3 },
                new ConditionalDialogueEntry { requiredStoryFlag = Chapter3StateManager.FlagCh3Started, dialogue = dlgStage2 }
            };
            EditorUtility.SetDirty(linhData);

            // 3. Build Bakery Echo Ghosts & EchoData
            var ghostGrandma = CreateOrUpdateGhostPrefab(
                Ch3PrefabRoot + "/Echo_GrandmaYoung.prefab",
                "Assets/Art/Characters/echo_grandma_young",
                ovenGlow,
                new Color(1.0f, 0.88f, 0.62f, 0.82f)
            );
            var ghostHana = CreateOrUpdateGhostPrefab(
                Ch3PrefabRoot + "/Echo_HanaYoung.prefab",
                "Assets/Art/Characters/echo_hana_young",
                ovenGlow,
                new Color(0.82f, 0.94f, 1.0f, 0.82f)
            );

            var echoOven = CreateOrUpdateAsset<EchoData>(Ch3DataRoot + "/Echoes/Echo_Ch3_HanaOven.asset");
            echoOven.echoID = "echo_ch3_hana_oven";
            echoOven.echoTitle = "Lò Bánh Đêm Hội Năm Xưa";
            echoOven.requiredFlag = Chapter3StateManager.FlagCh2Completed;
            echoOven.requiredItemId = "";
            echoOven.completedFlag = Chapter3StateManager.FlagCh3HanaEchoDone;
            echoOven.showKaelGhost = true;
            echoOven.showMaraGhost = true;
            echoOven.customGhostAPrefab = ghostGrandma;
            echoOven.customGhostBPrefab = ghostHana;
            echoOven.beats = new List<EchoBeat>
            {
                new EchoBeat
                {
                    speakerName = "Bà Nội (Thời Trẻ)", speakerId = "mara",
                    bodyText = "Hana à, mẻ bánh mật ong trăng non chín vàng rồi! Kẻo dân làng đợi ngoài cầu đó.",
                    durationSeconds = 4.2f, kaelPlaysAction = true, maraPlaysAction = false,
                    overlayTint = new Color(0.28f, 0.20f, 0.12f, 0.36f)
                },
                new EchoBeat
                {
                    speakerName = "Bà Hana (Thời Trẻ)", speakerId = "hana",
                    bodyText = "Chị yên tâm, em phết thêm chút mật cánh trăng cho thơm khắp cả bờ sông luôn!",
                    durationSeconds = 4.2f, kaelPlaysAction = false, maraPlaysAction = true,
                    overlayTint = new Color(0.30f, 0.22f, 0.14f, 0.36f)
                },
                new EchoBeat
                {
                    speakerName = "Bà Nội (Thời Trẻ)", speakerId = "mara",
                    bodyText = "Chợ Đêm không cần giàu sang, chỉ cần lò còn ấm và mọi người còn nhớ đường về với nhau.",
                    durationSeconds = 4.5f, kaelPlaysAction = true, maraPlaysAction = false,
                    overlayTint = new Color(0.24f, 0.24f, 0.32f, 0.38f)
                },
                new EchoBeat
                {
                    speakerName = "Bà Hana (Thời Trẻ)", speakerId = "hana",
                    bodyText = "Sau này nếu cầu có cũ, lò có nguội... rồi sẽ có người nhóm lại ngọn lửa này thôi chị nhỉ?",
                    durationSeconds = 4.5f, kaelPlaysAction = false, maraPlaysAction = true,
                    overlayTint = new Color(0.26f, 0.22f, 0.16f, 0.36f)
                },
                new EchoBeat
                {
                    speakerName = "Lumi", speakerId = "lumi",
                    bodyText = "Ánh sáng từ lò bánh vẫn còn ấm tới tận hôm nay... Bà nội và bà Hana đã chờ ngày này lâu lắm rồi.",
                    durationSeconds = 4.2f, kaelPlaysAction = false, maraPlaysAction = false,
                    overlayTint = new Color(0.18f, 0.30f, 0.44f, 0.34f)
                }
            };
            EditorUtility.SetDirty(echoOven);

            var postEchoDlg = CreateOrUpdateDialogue(
                Ch3DataRoot + "/Dialogues/Dlg_Ch3_PostHanaEcho.asset",
                "dlg_ch3_post_hana_echo",
                new[]
                {
                    ("Wen", "wen", "Thì ra trước đây bà nội và bà Hana cùng nhau nướng bánh cho mỗi dịp Chợ Đêm..."),
                    ("Lumi", "lumi", "Giờ tới lượt chúng ta thắp lại ánh đèn trên cầu bờ sông rồi đó Wen!")
                }
            );

            AssetDatabase.SaveAssets();

            // 4. Patch GameRoot.prefab (Portrait Linh + Chapter3PrepController)
            WireGameRootPrefab(linhPortrait);

            // 5. Place NPC_Linh and EchoSpot_HanaOven in MoonpetalValley_MVP.unity
            WireMvpScene(linhData, linhIdle, linhCheer, echoOven, postEchoDlg, ovenGlow);

            AssetDatabase.SaveAssets();
            Debug.Log("[Step50] Build thanh cong: NPC_Linh, EchoSpot_HanaOven, Chapter3PrepController!");
        }

        [MenuItem("Moonpetal/Step 50/2. Validate Step 50")]
        public static void ValidateStep50()
        {
            var errors = new List<string>();
            CheckAsset<NPCData>(Ch3DataRoot + "/NPCs/NPC_Linh.asset", errors);
            CheckAsset<DialogueData>(Ch3DataRoot + "/Dialogues/Dlg_Linh_Stage1_Intro.asset", errors);
            CheckAsset<DialogueData>(Ch3DataRoot + "/Dialogues/Dlg_Linh_Stage2_Prep.asset", errors);
            CheckAsset<DialogueData>(Ch3DataRoot + "/Dialogues/Dlg_Linh_Stage3_Day3Ready.asset", errors);
            CheckAsset<DialogueData>(Ch3DataRoot + "/Dialogues/Dlg_Linh_Stage4_PostCh3.asset", errors);
            CheckAsset<EchoData>(Ch3DataRoot + "/Echoes/Echo_Ch3_HanaOven.asset", errors);
            CheckAsset<DialogueData>(Ch3DataRoot + "/Dialogues/Dlg_Ch3_PostHanaEcho.asset", errors);
            CheckAsset<GameObject>(Ch3PrefabRoot + "/Echo_GrandmaYoung.prefab", errors);
            CheckAsset<GameObject>(Ch3PrefabRoot + "/Echo_HanaYoung.prefab", errors);

            if (File.Exists(GameRootPrefabPath))
            {
                var root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
                try
                {
                    if (root.GetComponentInChildren<Chapter3PrepController>(true) == null)
                        errors.Add("Thieu Chapter3PrepController tren GameRoot.prefab");
                    var dm = root.GetComponentInChildren<DialogueManager>(true);
                    if (dm == null)
                        errors.Add("Thieu DialogueManager tren GameRoot.prefab");
                    else
                    {
                        var so = new SerializedObject(dm);
                        var list = so.FindProperty("extraPortraits");
                        bool hasLinh = false;
                        if (list != null && list.isArray)
                        {
                            for (int i = 0; i < list.arraySize; i++)
                            {
                                var k = list.GetArrayElementAtIndex(i).FindPropertyRelative("speakerKey");
                                if (k != null && string.Equals(k.stringValue, "linh", StringComparison.OrdinalIgnoreCase))
                                    hasLinh = true;
                            }
                        }
                        if (!hasLinh) errors.Add("Chua dang ky portrait 'linh' trong DialogueManager.extraPortraits");
                    }
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
            else errors.Add("Khong tim thay GameRoot.prefab");

            var scene = EditorSceneManager.OpenScene(MvpScenePath, OpenSceneMode.Single);
            var npcLinh = GameObject.Find("NPC_Linh");
            if (npcLinh == null) errors.Add("Thieu GameObject NPC_Linh trong MoonpetalValley_MVP");
            else
            {
                if (npcLinh.GetComponent<NPCController>() == null) errors.Add("NPC_Linh thieu NPCController");
                if (npcLinh.GetComponent<Ch3ConditionalVisibility>() == null) errors.Add("NPC_Linh thieu Ch3ConditionalVisibility");
            }

            var echoSpot = GameObject.Find("EchoSpot_HanaOven");
            if (echoSpot == null) errors.Add("Thieu GameObject EchoSpot_HanaOven trong MoonpetalValley_MVP");
            else
            {
                if (echoSpot.GetComponent<EchoSpotInteractable>() == null) errors.Add("EchoSpot_HanaOven thieu EchoSpotInteractable");
                if (echoSpot.GetComponent<Ch3ConditionalVisibility>() == null) errors.Add("EchoSpot_HanaOven thieu Ch3ConditionalVisibility");
            }

            int missingScripts = 0;
            foreach (var go in scene.GetRootGameObjects())
            {
                foreach (var tr in go.GetComponentsInChildren<Transform>(true))
                    missingScripts += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(tr.gameObject);
            }
            if (missingScripts > 0) errors.Add("Phat hien " + missingScripts + " Missing Script trong MoonpetalValley_MVP");

            if (errors.Count == 0)
                Debug.Log("[Step50][VALIDATE] PASS 100% - NPC_Linh, EchoSpot_HanaOven, Chapter3PrepController & 0 Missing Scripts!");
            else
                Debug.LogError("[Step50][VALIDATE] FAIL (" + errors.Count + " loi):\n- " + string.Join("\n- ", errors));
        }

        private static void ImportLinhAndGhostArt()
        {
            ImportFolderSprites(LinhAnimDir, 20f, FilterMode.Point);
            ImportFolderSprites("Assets/Art/Characters/echo_grandma_young", 20f, FilterMode.Point);
            ImportFolderSprites("Assets/Art/Characters/echo_hana_young", 20f, FilterMode.Point);
            if (File.Exists(LinhPortraitPath))
                ConfigureTexture(LinhPortraitPath, 100f, FilterMode.Bilinear);
            if (File.Exists(GrandmaPortraitPath))
                ConfigureTexture(GrandmaPortraitPath, 100f, FilterMode.Bilinear);
        }

        private static void ImportFolderSprites(string folder, float ppu, FilterMode filter)
        {
            if (!Directory.Exists(folder)) return;
            foreach (string file in Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly))
                ConfigureTexture(file.Replace("\\", "/"), ppu, filter);
        }

        private static void ConfigureTexture(string assetPath, float ppu, FilterMode filter)
        {
            var ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (ti == null) return;
            bool dirty = false;
            if (ti.textureType != TextureImporterType.Sprite) { ti.textureType = TextureImporterType.Sprite; dirty = true; }
            if (ti.spriteImportMode != SpriteImportMode.Single) { ti.spriteImportMode = SpriteImportMode.Single; dirty = true; }
            if (Mathf.Abs(ti.spritePixelsPerUnit - ppu) > 0.01f) { ti.spritePixelsPerUnit = ppu; dirty = true; }
            if (ti.filterMode != filter) { ti.filterMode = filter; dirty = true; }
            if (ti.textureCompression != TextureImporterCompression.Uncompressed) { ti.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
            var settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);
            if (settings.spriteMeshType != SpriteMeshType.FullRect)
            {
                settings.spriteMeshType = SpriteMeshType.FullRect;
                ti.SetTextureSettings(settings);
                dirty = true;
            }
            if (dirty) ti.SaveAndReimport();
        }

        private static Sprite[] LoadLinhFrames(string prefix)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < 4; i++)
            {
                var s = AssetDatabase.LoadAssetAtPath<Sprite>(LinhAnimDir + "/" + prefix + "0" + i + ".png");
                if (s != null) list.Add(s);
            }
            if (list.Count == 0 && prefix == "idle_")
            {
                var south = AssetDatabase.LoadAssetAtPath<Sprite>(LinhAnimDir + "/south.png");
                if (south != null) list.Add(south);
            }
            return list.ToArray();
        }

        private static Sprite LoadLinhPortrait(Sprite[] fallbackIdle)
        {
            var p = AssetDatabase.LoadAssetAtPath<Sprite>(LinhPortraitPath);
            if (p != null) return p;
            return (fallbackIdle != null && fallbackIdle.Length > 0) ? fallbackIdle[0] : null;
        }

        private static DialogueData CreateOrUpdateDialogue(
            string path, string id, (string speaker, string speakerId, string body)[] lines,
            string setsFlag = "", ItemData rewardItem = null, int rewardAmount = 0)
        {
            var dlg = CreateOrUpdateAsset<DialogueData>(path);
            dlg.dialogueID = id;
            dlg.lines = new List<DialogueLine>();
            foreach (var l in lines)
            {
                dlg.lines.Add(new DialogueLine
                {
                    speakerNameKey = l.speaker,
                    speakerId = l.speakerId,
                    textKey = l.body
                });
            }
            dlg.setsStoryFlagOnFinish = setsFlag;
            dlg.givesItemOnFinish = rewardItem;
            dlg.givesItemAmount = rewardItem != null ? Mathf.Max(1, rewardAmount) : 0;
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static GameObject CreateOrUpdateGhostPrefab(string prefabPath, string artFolder, Sprite fallbackSprite, Color tint)
        {
            var idleList = new List<Sprite>();
            var actionList = new List<Sprite>();
            if (Directory.Exists(artFolder))
            {
                var files = Directory.GetFiles(artFolder, "*.png", SearchOption.TopDirectoryOnly);
                Array.Sort(files, StringComparer.OrdinalIgnoreCase);
                foreach (var f in files)
                {
                    string fn = Path.GetFileName(f).ToLowerInvariant();
                    var sp = AssetDatabase.LoadAssetAtPath<Sprite>(f.Replace("\\", "/"));
                    if (sp == null) continue;
                    if (fn.Contains("_action_")) actionList.Add(sp);
                    else if (fn.Contains("_idle_")) idleList.Add(sp);
                }
                if (idleList.Count == 0)
                {
                    var south = AssetDatabase.LoadAssetAtPath<Sprite>(artFolder + "/south.png");
                    if (south != null) idleList.Add(south);
                }
            }
            if (idleList.Count == 0 && fallbackSprite != null) idleList.Add(fallbackSprite);
            if (actionList.Count == 0) actionList.AddRange(idleList);

            var go = new GameObject(Path.GetFileNameWithoutExtension(prefabPath));
            try
            {
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sortingLayerName = "Entities";
                sr.sprite = idleList.Count > 0 ? idleList[0] : null;
                sr.color = tint;

                go.AddComponent<YSortRenderer>();
                var actor = go.AddComponent<EchoGhostActor>();
                var so = new SerializedObject(actor);
                so.FindProperty("spriteRenderer").objectReferenceValue = sr;
                SetSpriteArray(so.FindProperty("idleFrames"), idleList.ToArray());
                SetSpriteArray(so.FindProperty("actionFrames"), actionList.ToArray());
                so.FindProperty("ghostTint").colorValue = tint;
                so.ApplyModifiedPropertiesWithoutUndo();

                return PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }

        private static void WireGameRootPrefab(Sprite linhPortrait)
        {
            if (!File.Exists(GameRootPrefabPath)) return;
            var root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            try
            {
                if (root.GetComponentInChildren<Chapter3PrepController>(true) == null)
                    root.AddComponent<Chapter3PrepController>();

                var dm = root.GetComponentInChildren<DialogueManager>(true);
                if (dm != null && linhPortrait != null)
                {
                    var so = new SerializedObject(dm);
                    var listProp = so.FindProperty("extraPortraits");
                    if (listProp != null && listProp.isArray)
                    {
                        UpsertPortrait(listProp, "linh", linhPortrait);
                        UpsertPortrait(listProp, "Linh", linhPortrait);
                        var gmPortrait = AssetDatabase.LoadAssetAtPath<Sprite>(GrandmaPortraitPath);
                        if (gmPortrait != null)
                        {
                            UpsertPortrait(listProp, "grandma_young", gmPortrait);
                            UpsertPortrait(listProp, "Bà Nội (Thời Trẻ)", gmPortrait);
                        }
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        private static void UpsertPortrait(SerializedProperty listProp, string keyVal, Sprite spr)
        {
            for (int i = 0; i < listProp.arraySize; i++)
            {
                var elem = listProp.GetArrayElementAtIndex(i);
                var k = elem.FindPropertyRelative("speakerKey");
                if (k != null && string.Equals(k.stringValue, keyVal, StringComparison.OrdinalIgnoreCase))
                {
                    var p = elem.FindPropertyRelative("portrait");
                    if (p != null) p.objectReferenceValue = spr;
                    return;
                }
            }
            int idx = listProp.arraySize;
            listProp.InsertArrayElementAtIndex(idx);
            var added = listProp.GetArrayElementAtIndex(idx);
            added.FindPropertyRelative("speakerKey").stringValue = keyVal;
            added.FindPropertyRelative("portrait").objectReferenceValue = spr;
            added.FindPropertyRelative("alignLeft").boolValue = false;
            added.FindPropertyRelative("isFloating").boolValue = false;
        }

        private static void WireMvpScene(NPCData linhData, Sprite[] linhIdle, Sprite[] linhCheer, EchoData echoOven, DialogueData postEchoDlg, Sprite ovenGlow)
        {
            var scene = EditorSceneManager.OpenScene(MvpScenePath, OpenSceneMode.Single);

            // NPC_Linh near village square (hidden until flag_ch2_completed)
            var npcGo = GameObject.Find("NPC_Linh") ?? new GameObject("NPC_Linh");
            npcGo.transform.position = new Vector3(3.8f, 1.8f, 0f);
            var sr = GetOrAdd<SpriteRenderer>(npcGo);
            sr.sortingLayerName = "Entities";
            if (linhIdle != null && linhIdle.Length > 0) sr.sprite = linhIdle[0];

            GetOrAdd<YSortRenderer>(npcGo);
            var col = GetOrAdd<BoxCollider2D>(npcGo);
            col.size = new Vector2(0.8f, 0.5f);
            col.offset = new Vector2(0f, -0.35f);

            var npc = GetOrAdd<NPCController>(npcGo);
            var soNpc = new SerializedObject(npc);
            soNpc.FindProperty("npcData").objectReferenceValue = linhData;
            soNpc.FindProperty("spriteRenderer").objectReferenceValue = sr;
            soNpc.FindProperty("interactRadius").floatValue = 2.2f;
            SetSpriteArray(soNpc.FindProperty("idleFrames"), linhIdle);
            SetSpriteArray(soNpc.FindProperty("cheerFrames"), linhCheer);
            soNpc.ApplyModifiedPropertiesWithoutUndo();

            var visNpc = GetOrAdd<Ch3ConditionalVisibility>(npcGo);
            var soVisNpc = new SerializedObject(visNpc);
            soVisNpc.FindProperty("requiredFlag").stringValue = Chapter3StateManager.FlagCh2Completed;
            soVisNpc.ApplyModifiedPropertiesWithoutUndo();

            // EchoSpot_HanaOven near House_HanaBakery (-6.5, 5.8)
            var echoGo = GameObject.Find("EchoSpot_HanaOven") ?? new GameObject("EchoSpot_HanaOven");
            echoGo.transform.position = new Vector3(-5.0f, 3.8f, 0f);
            var esr = GetOrAdd<SpriteRenderer>(echoGo);
            esr.sortingLayerName = "Entities";
            esr.sprite = ovenGlow;
            esr.color = new Color(1f, 0.94f, 0.78f, 0.85f);

            GetOrAdd<YSortRenderer>(echoGo);
            var spot = GetOrAdd<EchoSpotInteractable>(echoGo);
            var soSpot = new SerializedObject(spot);
            soSpot.FindProperty("echoData").objectReferenceValue = echoOven;
            soSpot.FindProperty("promptLabel").stringValue = "Soi Dư Âm Lò Bánh Bà Hana";
            soSpot.FindProperty("blockedHint").stringValue = "Hoàn thành Chương 2 để soi Dư Âm này.";
            soSpot.FindProperty("interactRadius").floatValue = 2.4f;
            soSpot.FindProperty("postEchoDialogue").objectReferenceValue = postEchoDlg;
            soSpot.FindProperty("shimmerRenderer").objectReferenceValue = esr;
            soSpot.ApplyModifiedPropertiesWithoutUndo();

            var visEcho = GetOrAdd<Ch3ConditionalVisibility>(echoGo);
            var soVisEcho = new SerializedObject(visEcho);
            soVisEcho.FindProperty("requiredFlag").stringValue = Chapter3StateManager.FlagCh2Completed;
            soVisEcho.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static T CreateOrUpdateAsset<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            var inst = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(inst, path);
            return inst;
        }

        private static T GetOrAdd<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : go.AddComponent<T>();
        }

        private static void SetSpriteArray(SerializedProperty prop, Sprite[] sprites)
        {
            if (prop == null || !prop.isArray) return;
            int count = sprites != null ? sprites.Length : 0;
            prop.arraySize = count;
            for (int i = 0; i < count; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
        }

        private static void CheckAsset<T>(string path, List<string> errors) where T : UnityEngine.Object
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
                errors.Add("Thieu asset: " + path);
        }

        private static void EnsureDir(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}
