using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step43
    {
        private const string SceneCh2Path = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string Ch2DataRoot = "Assets/Data/Chapter2";
        private const string Ch2TilesRoot = "Assets/Data/Tiles/Ch2";
        private const string Ch2SpritesRoot = "Assets/Art/Sprites/Chapter2";
        private const string Ch2TerrainRoot = "Assets/Art/Sprites/Chapter2/Terrain";
        private const string Ch2OverhaulRoot = "Assets/Art/Sprites/Chapter2/Overhaul";
        private const string PortraitsRoot = "Assets/Art/Sprites/Portraits";
        private const string CharsRoot = "Assets/Art/Characters";

        [MenuItem("Moonpetal/Step 43/1. Apply Master Art, Build Kael, Mara & 3-Night Investigation")]
        public static void BuildStep43()
        {
            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 43.fix 5 (STARDEW RIVERBANK OVERHAUL + OTIS + LỀU TẠM) ===");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            ConfigureAllMasterArtImports();
            WirePortraitsIntoGameRoot();
            RefreshStep41Prefabs();
            CreateNPCsAndDialogues(out var kaelData, out var maraData, out var otisData, out var clue1Dlg, out var clue2Dlg, out var clue3Dlg);
            UpdateRiverbankScene(kaelData, maraData, otisData, clue1Dlg, clue2Dlg, clue3Dlg);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 43.fix 5! Hãy chạy Validate Step 43 để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 43/2. Validate Step 43")]
        public static void ValidateStep43()
        {
            int errors = 0;
            string[] mustSprites = new[]
            {
                Ch2SpritesRoot + "/prop_kael_workshop.png",
                Ch2SpritesRoot + "/prop_mara_hut.png",
                Ch2SpritesRoot + "/prop_nursery_east.png",
                Ch2SpritesRoot + "/prop_ferry_boat.png",
                Ch2SpritesRoot + "/prop_portal_sign.png",
                Ch2OverhaulRoot + "/ch2_water_deep_00.png",
                Ch2OverhaulRoot + "/ch2_water_shallow_00.png",
                Ch2OverhaulRoot + "/ch2_shore_east_left.png",
                Ch2OverhaulRoot + "/ch2_shore_west_right.png",
                Ch2OverhaulRoot + "/ch2_camp_tent.png",
                Ch2TerrainRoot + "/ch2_grass_east_0.png",
                Ch2TerrainRoot + "/ch2_grass_west_0.png",
                Ch2TerrainRoot + "/ch2_river_mist_strip.png",
                PortraitsRoot + "/portrait_kael.png",
                PortraitsRoot + "/portrait_mara.png",
            };
            foreach (var p in mustSprites)
            {
                if (AssetDatabase.LoadAssetAtPath<Sprite>(p) == null)
                {
                    Debug.LogError("[MV] VALIDATE FAIL: thiếu Sprite " + p);
                    errors++;
                }
            }

            var scene = EditorSceneManager.OpenScene(SceneCh2Path, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV] VALIDATE FAIL: không mở được " + SceneCh2Path);
                errors++;
            }
            else
            {
                string[] mustObjects =
                {
                    "House_KaelWorkshop", "House_EastNursery", "House_MaraDock",
                    "EastCampBed", "NPC_Otis_Ch2", "NPC_Kael", "NPC_Mara",
                    "FerryBoat_East", "FerryBoat_West", "Portal_ToVillage",
                    "RiversideWaitBench", "Clue_WoodShavings", "Clue_MudFootprints", "Clue_OtisNote",
                    "Ch2_EnvironmentDecor", "Ch2_RiverMist", "AnimatedRiverWater"
                };
                foreach (var n in mustObjects)
                {
                    if (GameObject.Find(n) == null)
                    {
                        Debug.LogError("[MV] VALIDATE FAIL: thiếu GameObject " + n + " trong Riverbank_Chapter2");
                        errors++;
                    }
                }
            }

            if (errors == 0)
                Debug.Log("[MV] VALIDATE STEP 43.fix 5 PASS (0 lỗi)! Map Bờ Sông uốn lượn, Bác Otis, Lều Tạm & Debug F1 Ch2 đã sẵn sàng.");
            else
                Debug.LogError("[MV] VALIDATE STEP 43.fix 5 CÓ " + errors + " LỖI.");
        }

        private static T EnsureComponent<T>(GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null) comp = go.AddComponent<T>();
            return comp;
        }

        // Độ cong S-curve tự nhiên của dòng sông theo tọa độ y (Unity Tilemap: y tăng là đi lên Bắc)
        // Cầu ở y = -1..1 giữ thẳng (-8..7) để khớp 100% RiverBridge & 6 cột đèn Step 45
        // Bắc (y >= 4) uốn nhẹ sang Tây (-1 -> -2); Nam (y <= -5) uốn nhẹ sang Đông (+1) tạo Vịnh Đò
        private static int GetRiverShift(int y)
        {
            if (y >= 9) return -2;
            if (y >= 5) return -1;
            if (y <= -6) return 1;
            return 0;
        }

        private static void ConfigureAllMasterArtImports()
        {
            ConfigureFolderSprites(Ch2SpritesRoot, 16f, FilterMode.Point);
            ConfigureSingleSprite(Ch2TerrainRoot + "/ch2_river_mist_strip.png", 16f, FilterMode.Bilinear);
            ConfigureFolderSprites(PortraitsRoot, 100f, FilterMode.Bilinear);

            string[] npcDirs = { "npc_kael", "npc_mara", "echo_kael_young", "echo_mara_young" };
            foreach (var d in npcDirs) ConfigureFolderSprites(CharsRoot + "/" + d, 20f, FilterMode.Point);

            string[] enemyDirs = { "enemy_thorn_crawler", "enemy_big_gloom_slime", "enemy_lantern_moth" };
            foreach (var d in enemyDirs) ConfigureFolderSprites(CharsRoot + "/" + d, 32f, FilterMode.Point);

            string[] propActorDirs = { "actor_storm_lantern", "actor_bridge_lantern" };
            foreach (var d in propActorDirs) ConfigureFolderSprites(CharsRoot + "/" + d, 24f, FilterMode.Point);
        }

        private static void ConfigureFolderSprites(string assetFolder, float ppu, FilterMode filterMode)
        {
            if (!AssetDatabase.IsValidFolder(assetFolder)) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { assetFolder });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("ch2_river_mist_strip.png")) continue;
                ConfigureSingleSprite(path, ppu, filterMode);
            }
        }

        private static void ConfigureSingleSprite(string path, float ppu, FilterMode filterMode)
        {
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) return;
            bool changed = false;
            if (imp.textureType != TextureImporterType.Sprite) { imp.textureType = TextureImporterType.Sprite; changed = true; }
            if (imp.spriteImportMode != SpriteImportMode.Single) { imp.spriteImportMode = SpriteImportMode.Single; changed = true; }
            if (Mathf.Abs(imp.spritePixelsPerUnit - ppu) > 0.01f) { imp.spritePixelsPerUnit = ppu; changed = true; }
            if (imp.filterMode != filterMode) { imp.filterMode = filterMode; changed = true; }
            if (imp.textureCompression != TextureImporterCompression.Uncompressed) { imp.textureCompression = TextureImporterCompression.Uncompressed; changed = true; }
            if (imp.mipmapEnabled) { imp.mipmapEnabled = false; changed = true; }
            if (!imp.alphaIsTransparency) { imp.alphaIsTransparency = true; changed = true; }

            var settings = new TextureImporterSettings();
            imp.ReadTextureSettings(settings);
            if (settings.spriteMeshType != SpriteMeshType.FullRect)
            {
                settings.spriteMeshType = SpriteMeshType.FullRect;
                imp.SetTextureSettings(settings);
                changed = true;
            }
            if (changed) imp.SaveAndReimport();
        }

        private static void WirePortraitsIntoGameRoot()
        {
            var kaelSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PortraitsRoot + "/portrait_kael.png");
            var maraSpr = AssetDatabase.LoadAssetAtPath<Sprite>(PortraitsRoot + "/portrait_mara.png");
            if (kaelSpr == null || maraSpr == null || !File.Exists(GameRootPrefabPath)) return;

            var root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            try
            {
                var dm = root.GetComponentInChildren<DialogueManager>(true);
                if (dm != null)
                {
                    var so = new SerializedObject(dm);
                    var listProp = so.FindProperty("extraPortraits");
                    if (listProp != null && listProp.isArray)
                    {
                        UpsertPortraitEntry(listProp, "kael", kaelSpr);
                        UpsertPortraitEntry(listProp, "Ông Kael", kaelSpr);
                        UpsertPortraitEntry(listProp, "mara", maraSpr);
                        UpsertPortraitEntry(listProp, "Bà Mara", maraSpr);
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void UpsertPortraitEntry(SerializedProperty listProp, string keyVal, Sprite spr)
        {
            for (int i = 0; i < listProp.arraySize; i++)
            {
                var elem = listProp.GetArrayElementAtIndex(i);
                var keyProp = elem.FindPropertyRelative("speakerKey");
                if (keyProp != null && string.Equals(keyProp.stringValue, keyVal, StringComparison.OrdinalIgnoreCase))
                {
                    var spProp = elem.FindPropertyRelative("portrait");
                    if (spProp != null) spProp.objectReferenceValue = spr;
                    return;
                }
            }
            int idx = listProp.arraySize;
            listProp.InsertArrayElementAtIndex(idx);
            var added = listProp.GetArrayElementAtIndex(idx);
            var k = added.FindPropertyRelative("speakerKey");
            var p = added.FindPropertyRelative("portrait");
            if (k != null) k.stringValue = keyVal;
            if (p != null) p.objectReferenceValue = spr;
        }

        private static void RefreshStep41Prefabs()
        {
            RefreshSinglePrefabSprite("Assets/Prefabs/Chapter2/Echo_KaelYoung.prefab", CharsRoot + "/echo_kael_young/echo_kael_young_idle_00.png");
            RefreshSinglePrefabSprite("Assets/Prefabs/Chapter2/Echo_MaraYoung.prefab", CharsRoot + "/echo_mara_young/echo_mara_young_idle_00.png");
            RefreshSinglePrefabSprite("Assets/Prefabs/Chapter2/Actor_StormLantern.prefab", CharsRoot + "/actor_storm_lantern/actor_storm_lantern_lit_00.png");
        }

        private static void RefreshSinglePrefabSprite(string prefabPath, string spritePath)
        {
            if (!File.Exists(prefabPath)) return;
            var spr = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (spr == null) return;
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                var sr = root.GetComponentInChildren<SpriteRenderer>(true);
                if (sr != null) sr.sprite = spr;
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void CreateNPCsAndDialogues(
            out NPCData kaelData, out NPCData maraData, out NPCData otisData,
            out DialogueData clue1Dlg, out DialogueData clue2Dlg, out DialogueData clue3Dlg)
        {
            Directory.CreateDirectory(Ch2DataRoot + "/NPCs");
            Directory.CreateDirectory(Ch2DataRoot + "/Dialogues");

            var kaelIdle = AssetDatabase.LoadAssetAtPath<Sprite>(CharsRoot + "/npc_kael/npc_kael_idle_00.png");
            var maraIdle = AssetDatabase.LoadAssetAtPath<Sprite>(CharsRoot + "/npc_mara/npc_mara_idle_00.png");
            var otisIdle = FindNPCFrame("npc_otis", "idle", 0);
            var kaelPort = AssetDatabase.LoadAssetAtPath<Sprite>(PortraitsRoot + "/portrait_kael.png");
            var maraPort = AssetDatabase.LoadAssetAtPath<Sprite>(PortraitsRoot + "/portrait_mara.png");
            var otisPort = FindActiveCh1Sprite("portrait_otis", PortraitsRoot + "/portrait_otis.png");

            var kaelIntro = CreateOrUpdateDialogue(Ch2DataRoot + "/Dialogues/Dlg_Ch2_Kael_Intro.asset", "dlg_ch2_kael_intro", "flag_ch2_met_kael", new[]
            {
                ("Ông Kael", "kael", "Này cô bé ở vườn ươm! Đứng xa mép cầu ra, ván mục hết rồi."),
                ("Wen", "wen", "Cháu là Wen. Cháu thấy cây cầu sang bờ Tây bị gãy, chú có biết chuyện gì xảy ra không ạ?"),
                ("Ông Kael", "kael", "Ba năm trước bão lớn cuốn phăng nhịp giữa. Có người bên kia sông cứ đòi vá tạm, tôi cản không nổi nên... bỏ luôn."),
                ("Ông Kael", "kael", "Nếu cháu tò mò thì đợi chập tối hãy đi quanh bờ Đông mà xem. Ban ngày sương che hết dấu vết rồi."),
            });

            var maraIntro = CreateOrUpdateDialogue(Ch2DataRoot + "/Dialogues/Dlg_Ch2_Mara_Intro.asset", "dlg_ch2_mara_intro", "flag_ch2_met_mara", new[]
            {
                ("Bà Mara", "mara", "Cháu sang được bờ Tây bằng phà gỗ à? Giỏi đấy, dòng nước hôm nay chảy xiết lắm."),
                ("Wen", "wen", "Cháu là Wen. Bà Mara, nghe nói cây cầu từng nối hai bờ từ lâu lắm rồi?"),
                ("Bà Mara", "mara", "Hồi trẻ tôi với ông Kael đóng nó bằng gỗ lim. Đêm bão năm ấy đèn bão tắt ngúm, ông ấy quay lưng bỏ mặc bến thuyền của tôi..."),
                ("Bà Mara", "mara", "Thôi, chuyện cũ rồi. Cháu đừng đi sâu vào đầm lầy phía Tây Bắc, Bò Gai ở đó hung dữ lắm."),
            });

            var otisCh2Dlg = CreateOrUpdateDialogue(Ch2DataRoot + "/Dialogues/Dlg_Ch2_Otis_Camp.asset", "dlg_ch2_otis_camp", "flag_ch2_met_otis", new[]
            {
                ("Bác Otis", "otis", "Chào Wen! Bác sang phụ dựng cái Lều Nghỉ Tạm cạnh Vườn Ươm Bờ Đông cho cháu tiện đi lại."),
                ("Wen", "wen", "Cảm ơn bác Otis! Vậy tối muộn cháu nghỉ ngay ở lều này được đúng không ạ?"),
                ("Bác Otis", "otis", "Đúng rồi, cứ bấm [E] vào lều trước 02:00 đêm là hồi đầy 100% Thể lực, khỏi chạy ngược về làng."),
                ("Bác Otis", "otis", "À, chuyện ông Kael với bà Mara hồi đêm bão 3 năm trước phức tạp lắm... Cháu chịu khó ngồi ghế đợi chập tối rồi tìm hiểu kỹ nhé."),
            });

            clue1Dlg = CreateOrUpdateDialogue(Ch2DataRoot + "/Dialogues/Dlg_Ch2_Clue1_Shavings.asset", "dlg_ch2_clue1", "flag_ch2_inv_night1_done", new[]
            {
                ("Wen", "wen", "Mấy phoi bào gỗ lim này còn rất mới... có ai đó vẫn lén đẽo ván cầu vào ban đêm."),
                ("Lumi", "lumi", "Gió sông mang mùi nhựa thông tươi từ phía xưởng ông Kael!"),
            });

            clue2Dlg = CreateOrUpdateDialogue(Ch2DataRoot + "/Dialogues/Dlg_Ch2_Clue2_Footprints.asset", "dlg_ch2_clue2", "flag_ch2_inv_night2_done", new[]
            {
                ("Wen", "wen", "Dấu ủng dính bùn đi từ bến phà thẳng ra tận mố cầu gãy... cả hai người đều âm thầm ra đây xem cầu mỗi đêm."),
                ("Lumi", "lumi", "Ánh đèn nhỏ trên bến bà Mara cứ chớp tắt như đang chờ ai đó trả lời."),
            });

            clue3Dlg = CreateOrUpdateDialogueWithChoice(
                Ch2DataRoot + "/Dialogues/Dlg_Ch2_Clue3_OtisNote.asset",
                "dlg_ch2_clue3",
                "flag_ch2_inv_night3_done",
                new[]
                {
                    ("Bác Otis", "otis", "Cháu tìm thấy sổ ghi chép cũ của bác rồi à? Đêm bão năm đó Kael chặt cáp giữ nhà trẻ bờ Đông rồi lao xuống sông cứu thuyền cho Mara tới bị thương nặng."),
                    ("Wen", "wen", "Hóa ra cả hai đều hiểu lầm nhau suốt 3 năm! Mình nên nói thẳng sự thật cho bà Mara hay để hai người tự thấy qua Ánh Soi?"),
                },
                "Nói thẳng sự thật từ lời bác Otis", "flag_ch2_choice_tell_truth",
                "Giữ kín chuyện cũ, để họ tự thấy qua Ánh Soi", "flag_ch2_choice_keep_secret"
            );

            kaelData = CreateOrUpdateNPC(Ch2DataRoot + "/NPCs/NPC_Kael.asset", "npc_kael", "Ông Kael", "Thợ Mộc Bờ Đông", kaelIdle, kaelPort, kaelIntro);
            maraData = CreateOrUpdateNPC(Ch2DataRoot + "/NPCs/NPC_Mara.asset", "npc_mara", "Bà Mara", "Chủ Bến Thuyền Bờ Tây", maraIdle, maraPort, maraIntro);
            otisData = CreateOrUpdateNPC(Ch2DataRoot + "/NPCs/NPC_Otis_Ch2.asset", "npc_otis_ch2", "Bác Otis", "Người Gác Vườn Ươm Tạm", otisIdle, otisPort, otisCh2Dlg);
        }

        private static NPCData CreateOrUpdateNPC(string path, string id, string displayName, string roleTitle, Sprite worldSpr, Sprite portraitSpr, DialogueData dlg)
        {
            var npc = AssetDatabase.LoadAssetAtPath<NPCData>(path);
            if (npc == null)
            {
                npc = ScriptableObject.CreateInstance<NPCData>();
                AssetDatabase.CreateAsset(npc, path);
            }
            npc.npcID = id;
            npc.displayNameKey = displayName;
            npc.roleTitleKey = roleTitle;
            npc.worldSprite = worldSpr;
            npc.portraitSprite = portraitSpr;
            npc.defaultDialogue = dlg;
            EditorUtility.SetDirty(npc);
            return npc;
        }

        private static DialogueData CreateOrUpdateDialogue(string path, string dlgId, string finishFlag, (string speaker, string id, string text)[] lines)
        {
            var dlg = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
            if (dlg == null)
            {
                dlg = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(dlg, path);
            }
            dlg.dialogueID = dlgId;
            dlg.setsStoryFlagOnFinish = finishFlag;
            dlg.lines = new List<DialogueLine>();
            for (int i = 0; i < lines.Length; i++)
            {
                dlg.lines.Add(new DialogueLine
                {
                    speakerNameKey = lines[i].speaker,
                    speakerId = lines[i].id,
                    textKey = lines[i].text,
                });
            }
            dlg.choices = new List<DialogueChoice>();
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static DialogueData CreateOrUpdateDialogueWithChoice(
            string path, string dlgId, string finishFlag,
            (string speaker, string id, string text)[] lines,
            string optA, string flagA, string optB, string flagB)
        {
            var dlg = CreateOrUpdateDialogue(path, dlgId, finishFlag, lines);
            dlg.choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceTextKey = optA, setsFlagOnPick = flagA },
                new DialogueChoice { choiceTextKey = optB, setsFlagOnPick = flagB },
            };
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static void UpdateRiverbankScene(
            NPCData kaelData, NPCData maraData, NPCData otisData,
            DialogueData clue1Dlg, DialogueData clue2Dlg, DialogueData clue3Dlg)
        {
            var scene = EditorSceneManager.OpenScene(SceneCh2Path, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[MV] Không mở được scene " + SceneCh2Path);
                return;
            }

            // 1. Dựng lại toàn bộ bản đồ sông uốn lượn chữ S + bờ sông hữu cơ + nước đa tầng động
            RebuildCurvedRiverAndTilemaps();

            // 2. Bố trí 3 công trình chính (6 phân khu chuẩn Stardew Valley) + Y-Sort & Collider chuẩn
            ConfigureHouseObject("House_KaelWorkshop", Ch2SpritesRoot + "/prop_kael_workshop.png",
                new Vector3(11.0f, 6.2f, 0f), new Vector2(4.2f, 1.8f), new Vector2(0f, -1.0f), 0.45f);
            ConfigureHouseObject("House_EastNursery", Ch2SpritesRoot + "/prop_nursery_east.png",
                new Vector3(19.0f, 6.0f, 0f), new Vector2(4.2f, 1.8f), new Vector2(0f, -1.0f), 0.45f);
            ConfigureHouseObject("House_MaraDock", Ch2SpritesRoot + "/prop_mara_hut.png",
                new Vector3(-13.5f, -3.2f, 0f), new Vector2(3.4f, 1.55f), new Vector2(0f, -0.88f), 0.45f);

            // 3. Cổng về Làng Chính & 2 Bến Đò Ngang (neo sát vịnh sông phía Nam)
            ConfigurePropSortAndCollider("Portal_ToVillage", Ch2SpritesRoot + "/prop_portal_sign.png",
                new Vector3(22.5f, 0.5f, 0f), new Vector2(0.8f, 0.6f), new Vector2(0f, -0.15f), 0.15f);
            ConfigurePropSortAndCollider("FerryBoat_East", Ch2SpritesRoot + "/prop_ferry_boat.png",
                new Vector3(8.6f, -7.5f, 0f), new Vector2(1.4f, 0.9f), new Vector2(0f, -0.15f), 0.15f);
            ConfigurePropSortAndCollider("FerryBoat_West", Ch2SpritesRoot + "/prop_ferry_boat.png",
                new Vector3(-6.6f, -7.5f, 0f), new Vector2(1.4f, 0.9f), new Vector2(0f, -0.15f), 0.15f);

            // Cập nhật điểm Spawn_EastCamp ngay trước cửa Lều Nghỉ Tạm
            UpdateSpawnRegistryPoint("Spawn_EastCamp", new Vector2(15.2f, -3.6f));
            UpdateSpawnRegistryPoint("Spawn_WestDock", new Vector2(-10.5f, -6.5f));

            // 4. Lều Nghỉ Tạm Bờ Đông (EastCampBed - BedInteractable)
            BuildEastCampBed(new Vector3(15.2f, -2.1f, 0f));

            // 5. Trang trí 6 phân khu Stardew + Đá cuội & Lá sen trên sông + Sương sông
            RebuildStardewRiverbankDecor();
            RebuildRiverMist();

            // 6. Đặt 3 NPC thật: Ông Kael, Bà Mara và Bác Otis (Hướng Otis-A)
            var kaelGo = CreateOrUpdateNPCObject("NPC_Kael", new Vector3(11.0f, 3.3f, 0f), "npc_kael", kaelData, false);
            EnsureComponent<Chapter2NPCSchedule>(kaelGo).Configure(new Vector2(11.0f, 3.3f), new Vector2(8.8f, 1.2f));

            var maraGo = CreateOrUpdateNPCObject("NPC_Mara", new Vector3(-13.2f, -5.2f, 0f), "npc_mara", maraData, false);
            EnsureComponent<Chapter2NPCSchedule>(maraGo).Configure(new Vector2(-13.2f, -5.2f), new Vector2(-9.2f, -0.8f));

            var otisGo = CreateOrUpdateNPCObject("NPC_Otis_Ch2", new Vector3(17.2f, 3.0f, 0f), "npc_otis", otisData, true);
            EnsureComponent<NPCApproachOnInteract>(otisGo);

            // 7. Ghế Đợi Hoàng Hôn ngắm sông ngay cạnh mố cầu Đông
            var bench = GameObject.Find("RiversideWaitBench");
            if (bench == null) bench = new GameObject("RiversideWaitBench");
            bench.transform.position = new Vector3(10.2f, -2.5f, 0f);
            bench.tag = "Interactable";
            bench.layer = 11;
            var benchSr = EnsureComponent<SpriteRenderer>(bench);
            benchSr.sprite = FindActiveCh1Sprite("prop_memory_bench", "Assets/Art/Sprites/Environment/Props/prop_memory_bench.png");
            benchSr.sortingLayerName = "WorldObjects";
            EnsureComponent<YSortRenderer>(bench).Configure(benchSr, false, true, 0.15f, 0);
            var benchCol = EnsureComponent<BoxCollider2D>(bench);
            benchCol.isTrigger = false;
            benchCol.size = new Vector2(1.5f, 0.65f);
            benchCol.offset = new Vector2(0f, -0.25f);
            EnsureComponent<WaitUntilDuskInteractable>(bench);

            // 8. Ba manh mối điều tra 3 đêm (đồng bộ stage 1, 2, 3)
            CreateOrUpdateClue("Clue_WoodShavings", new Vector3(9.4f, 2.4f, 0f),
                "Phoi bào gỗ lim mới", 1, "flag_ch2_met_kael", "flag_ch2_inv_night1_done", clue1Dlg);
            CreateOrUpdateClue("Clue_MudFootprints", new Vector3(-10.0f, -2.2f, 0f),
                "Dấu ủng dính bùn gần cầu", 2, "flag_ch2_inv_night1_done", "flag_ch2_inv_night2_done", clue2Dlg);
            CreateOrUpdateClue("Clue_OtisNote", new Vector3(16.0f, 3.6f, 0f),
                "Sổ ghi chép cũ của bác Otis", 3, "flag_ch2_inv_night2_done", "flag_ch2_inv_night3_done", clue3Dlg, "NPC_Otis_Ch2");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV] Đã lưu Riverbank_Chapter2.unity (Stardew Riverbank Overhaul + Bác Otis + Lều Tạm).");
        }

        private static void RebuildCurvedRiverAndTilemaps()
        {
            var groundGo = GameObject.Find("GroundTilemap");
            var obsGo = GameObject.Find("ObstacleTilemap");
            if (groundGo == null || obsGo == null) return;

            var groundTm = groundGo.GetComponent<Tilemap>();
            var obsTm = obsGo.GetComponent<Tilemap>();
            if (groundTm == null || obsTm == null) return;

            Directory.CreateDirectory(Ch2TilesRoot);

            Tile[] eastGrass = new Tile[4];
            Tile[] westGrass = new Tile[4];
            for (int i = 0; i < 4; i++)
            {
                eastGrass[i] = GetOrCreatePersistentTile(Ch2TilesRoot + $"/Ch2_GrassEast_{i}.asset", Ch2TerrainRoot + $"/ch2_grass_east_{i}.png", Tile.ColliderType.None);
                westGrass[i] = GetOrCreatePersistentTile(Ch2TilesRoot + $"/Ch2_GrassWest_{i}.asset", Ch2TerrainRoot + $"/ch2_grass_west_{i}.png", Tile.ColliderType.None);
            }

            Tile[] eastPath = new Tile[16];
            Tile[] westPath = new Tile[16];
            for (int m = 0; m < 16; m++)
            {
                eastPath[m] = GetOrCreatePersistentTile(Ch2TilesRoot + $"/Ch2_PathEast_m{m:D2}.asset", Ch2TerrainRoot + $"/ch2_path_east_m{m:D2}.png", Tile.ColliderType.None);
                westPath[m] = GetOrCreatePersistentTile(Ch2TilesRoot + $"/Ch2_PathWest_m{m:D2}.asset", Ch2TerrainRoot + $"/ch2_path_west_m{m:D2}.png", Tile.ColliderType.None);
            }

            Tile tWaterDeep = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_WaterDeep.asset", Ch2OverhaulRoot + "/ch2_water_deep_00.png", Tile.ColliderType.None);
            Tile tWaterShallow = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_WaterShallow.asset", Ch2OverhaulRoot + "/ch2_water_shallow_00.png", Tile.ColliderType.None);
            Tile tWaterBlock = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_WaterCollider.asset", Ch2OverhaulRoot + "/ch2_water_deep_00.png", Tile.ColliderType.Grid);

            Tile tShoreE = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreE.asset", Ch2OverhaulRoot + "/ch2_shore_east_left.png", Tile.ColliderType.None);
            Tile tShoreERL = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreE_RL.asset", Ch2OverhaulRoot + "/ch2_shore_east_bend_r_left.png", Tile.ColliderType.None);
            Tile tShoreERR = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreE_RR.asset", Ch2OverhaulRoot + "/ch2_shore_east_bend_r_right.png", Tile.ColliderType.None);
            Tile tShoreELL = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreE_LL.asset", Ch2OverhaulRoot + "/ch2_shore_east_bend_l_left.png", Tile.ColliderType.None);
            Tile tShoreELR = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreE_LR.asset", Ch2OverhaulRoot + "/ch2_shore_east_bend_l_right.png", Tile.ColliderType.None);

            Tile tShoreW = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreW.asset", Ch2OverhaulRoot + "/ch2_shore_west_right.png", Tile.ColliderType.None);
            Tile tShoreWRL = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreW_RL.asset", Ch2OverhaulRoot + "/ch2_shore_west_bend_r_left.png", Tile.ColliderType.None);
            Tile tShoreWRR = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreW_RR.asset", Ch2OverhaulRoot + "/ch2_shore_west_bend_r_right.png", Tile.ColliderType.None);
            Tile tShoreWLL = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreW_LL.asset", Ch2OverhaulRoot + "/ch2_shore_west_bend_l_left.png", Tile.ColliderType.None);
            Tile tShoreWLR = GetOrCreatePersistentTile(Ch2TilesRoot + "/Ch2_ShoreW_LR.asset", Ch2OverhaulRoot + "/ch2_shore_west_bend_l_right.png", Tile.ColliderType.None);

            // Xóa sạch ObstacleTilemap cũ (bỏ tường gạch tile_tree xấu ở viền) rồi vẽ lại chuẩn
            obsTm.ClearAllTiles();
            var obsRen = obsGo.GetComponent<TilemapRenderer>();
            if (obsRen != null) obsRen.enabled = false; // ObstacleTilemap chỉ làm collider vô hình cho nước/viền map!

            // Xóa nước động cũ và tạo mới 2 lớp nước động (Deep + Shallow)
            var oldWater = GameObject.Find("AnimatedRiverWater");
            if (oldWater != null) UnityEngine.Object.DestroyImmediate(oldWater);
            var waterRoot = new GameObject("AnimatedRiverWater");
            var deepRenderers = new List<SpriteRenderer>();
            var shallowRenderers = new List<SpriteRenderer>();
            var deep0 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_water_deep_00.png");
            var shallow0 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_water_shallow_00.png");

            // Đường đi uốn lượn tự nhiên nối 6 phân khu
            bool IsPathCell(int x, int y)
            {
                if (x <= -25 || x >= 25 || y <= -13 || y >= 13) return false;
                int s = GetRiverShift(y);
                int wl = -8 + s;
                int er = 7 + s;
                if (x >= wl - 1 && x <= er + 1) return false;

                // Trục chính Đông-Tây dẫn ra Cầu (y = -1..1)
                if (y >= -1 && y <= 1) return true;
                // Bờ Đông: nhánh lên Xưởng Kael (x = 11..12, y = 1..4) & nhánh lên Vườn Ươm/Otis (x = 17..18, y = 1..4)
                if ((x == 11 || x == 12) && y >= 1 && y <= 4) return true;
                if ((x == 17 || x == 18) && y >= 1 && y <= 4) return true;
                // Bờ Đông: đường cong xuống Lều Tạm & Bến Đò Nam
                if (x == 14 && y >= -5 && y <= -1) return true;
                if (y == -5 && x >= 11 && x <= 14) return true;
                if ((x == 11 || x == 12) && y >= -8 && y <= -5) return true;
                if (y == -8 && x >= 9 && x <= 12) return true;
                // Bờ Tây: đường nối cầu xuống Bến Mara & Bến Đò Tây + lối vào Đầm Lầy Bắc
                if ((x == -11 || x == -12) && y >= -8 && y <= 4) return true;
                if (y == -8 && x >= -12 && x <= -8) return true;
                if (y == 4 && x >= -16 && x <= -11) return true;
                return false;
            }

            // Các hàng chuyển khúc cong (bend row) giữa 2 đoạn sông
            // Vì Unity y tăng lên Bắc, đi từ trên (y+1) xuống dưới (y):
            // y = 8: shift trên (-2) -> shift dưới (-1) => dịch PHẢI (+1)
            // y = 4: shift trên (-1) -> shift dưới (0)  => dịch PHẢI (+1)
            // y = -6: shift trên (0) -> shift dưới (+1) => dịch PHẢI (+1)
            for (int y = -14; y < 14; y++)
            {
                bool isBendR = (y == 8 || y == 4 || y == -6);
                int sTop = (y == 8) ? -2 : (y == 4 ? -1 : (y == -6 ? 0 : GetRiverShift(y)));
                int wl = -8 + sTop;
                int er = 7 + sTop;
                bool isBridgeRow = (y >= -1 && y <= 1);

                for (int x = -26; x < 26; x++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    bool border = (x == -26 || x == 25 || y == -14 || y == 13);

                    if (isBendR)
                    {
                        if (x == wl) { groundTm.SetTile(cell, tShoreWRL); obsTm.SetTile(cell, tWaterBlock); continue; }
                        if (x == wl + 1) { groundTm.SetTile(cell, tShoreWRR); obsTm.SetTile(cell, tWaterBlock); continue; }
                        if (x == er) { groundTm.SetTile(cell, tShoreERL); obsTm.SetTile(cell, tWaterBlock); continue; }
                        if (x == er + 1) { groundTm.SetTile(cell, tShoreERR); obsTm.SetTile(cell, tWaterBlock); continue; }

                        if (x > wl + 1 && x < er)
                        {
                            bool shallow = (x == wl + 2 || x == er - 1);
                            groundTm.SetTile(cell, shallow ? tWaterShallow : tWaterDeep);
                            obsTm.SetTile(cell, tWaterBlock);
                            SpawnRiverWaterCell(waterRoot.transform, x, y, shallow ? shallow0 : deep0, shallow ? shallowRenderers : deepRenderers);
                            continue;
                        }
                    }
                    else
                    {
                        if (x == wl)
                        {
                            groundTm.SetTile(cell, tShoreW);
                            if (!isBridgeRow) obsTm.SetTile(cell, tWaterBlock);
                            continue;
                        }
                        if (x == er)
                        {
                            groundTm.SetTile(cell, tShoreE);
                            if (!isBridgeRow) obsTm.SetTile(cell, tWaterBlock);
                            continue;
                        }
                        if (x > wl && x < er)
                        {
                            bool shallow = (x == wl + 1 || x == er - 1);
                            groundTm.SetTile(cell, shallow ? tWaterShallow : tWaterDeep);
                            if (!isBridgeRow) obsTm.SetTile(cell, tWaterBlock);
                            if (!isBridgeRow || y == -1 || y == 1)
                                SpawnRiverWaterCell(waterRoot.transform, x, y, shallow ? shallow0 : deep0, shallow ? shallowRenderers : deepRenderers);
                            continue;
                        }
                    }

                    // Vùng đất Bờ Đông hoặc Bờ Tây
                    bool isWest = x < wl;
                    if (IsPathCell(x, y))
                    {
                        int mask = 0;
                        if (IsPathCell(x, y + 1)) mask |= 1;
                        if (IsPathCell(x + 1, y)) mask |= 2;
                        if (IsPathCell(x, y - 1)) mask |= 4;
                        if (IsPathCell(x - 1, y)) mask |= 8;
                        groundTm.SetTile(cell, isWest ? westPath[mask] : eastPath[mask]);
                    }
                    else
                    {
                        int h = Math.Abs(x * 73856093 ^ y * 19349663) % 4;
                        groundTm.SetTile(cell, isWest ? westGrass[h] : eastGrass[h]);
                    }

                    if (border) obsTm.SetTile(cell, tWaterBlock);
                }
            }

            groundTm.RefreshAllTiles();
            obsTm.RefreshAllTiles();

            // Gắn 2 WaterTileAnimator riêng cho nước sâu & nước nông
            AttachWaterAnimator(waterRoot, "DeepWaterAnim", "ch2_water_deep_", deepRenderers, 3.4f);
            AttachWaterAnimator(waterRoot, "ShallowWaterAnim", "ch2_water_shallow_", shallowRenderers, 2.6f);
        }

        private static void SpawnRiverWaterCell(Transform parent, int x, int y, Sprite spr, List<SpriteRenderer> list)
        {
            var go = new GameObject($"RiverCell_{x}_{y}");
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = "GroundDecor";
            sr.sortingOrder = 1;
            list.Add(sr);
        }

        private static void AttachWaterAnimator(GameObject parent, string childName, string prefix, List<SpriteRenderer> renderers, float fpsVal)
        {
            var go = new GameObject(childName);
            go.transform.SetParent(parent.transform, false);
            var anim = go.AddComponent<WaterTileAnimator>();
            var so = new SerializedObject(anim);
            var fProp = so.FindProperty("waterFrames");
            fProp.arraySize = 4;
            for (int i = 0; i < 4; i++)
                fProp.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>($"{Ch2OverhaulRoot}/{prefix}{i:D2}.png");
            var rProp = so.FindProperty("waterRenderers");
            rProp.arraySize = renderers.Count;
            for (int i = 0; i < renderers.Count; i++)
                rProp.GetArrayElementAtIndex(i).objectReferenceValue = renderers[i];
            var fpsProp = so.FindProperty("fps");
            if (fpsProp != null) fpsProp.floatValue = fpsVal;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Tile GetOrCreatePersistentTile(string tileAssetPath, string spritePath, Tile.ColliderType colType)
        {
            var spr = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (spr == null) return null;
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(tileAssetPath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tileAssetPath);
            }
            tile.sprite = spr;
            tile.colliderType = colType;
            EditorUtility.SetDirty(tile);
            return tile;
        }

        private static void ConfigureHouseObject(string objName, string spritePath, Vector3 pos, Vector2 colSize, Vector2 colOffset, float footYOffset)
        {
            var go = GameObject.Find(objName);
            if (go == null) return;
            go.transform.position = pos;
            var sr = EnsureComponent<SpriteRenderer>(go);
            var spr = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (spr != null) sr.sprite = spr;
            sr.sortingLayerName = "WorldObjects";

            var bc = EnsureComponent<BoxCollider2D>(go);
            bc.isTrigger = false;
            bc.size = colSize;
            bc.offset = colOffset;

            EnsureComponent<YSortRenderer>(go).Configure(sr, false, true, footYOffset, 0);
        }

        private static void ConfigurePropSortAndCollider(string objName, string spritePath, Vector3 pos, Vector2 colSize, Vector2 colOffset, float footYOffset)
        {
            var go = GameObject.Find(objName);
            if (go == null) return;
            go.transform.position = pos;
            var sr = go.GetComponentInChildren<SpriteRenderer>(true);
            var spr = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sr != null)
            {
                if (spr != null) sr.sprite = spr;
                sr.sortingLayerName = "WorldObjects";
                EnsureComponent<YSortRenderer>(go).Configure(sr, false, true, footYOffset, 0);
            }
            var bc = go.GetComponent<BoxCollider2D>();
            if (bc != null)
            {
                bc.size = colSize;
                bc.offset = colOffset;
            }
        }

        private static void BuildEastCampBed(Vector3 pos)
        {
            var go = GameObject.Find("EastCampBed");
            if (go == null) go = new GameObject("EastCampBed");
            go.transform.position = pos;
            go.tag = "Interactable";
            go.layer = 11;

            var sr = EnsureComponent<SpriteRenderer>(go);
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_camp_tent.png");
            sr.sortingLayerName = "WorldObjects";
            EnsureComponent<YSortRenderer>(go).Configure(sr, false, true, 0.35f, 0);

            var bc = EnsureComponent<BoxCollider2D>(go);
            bc.isTrigger = false;
            bc.size = new Vector2(2.3f, 1.15f);
            bc.offset = new Vector2(0f, -0.45f);

            EnsureComponent<BedInteractable>(go);
        }

        private static void UpdateSpawnRegistryPoint(string spawnId, Vector2 newPos)
        {
            var reg = UnityEngine.Object.FindAnyObjectByType<WorldSpawnRegistry>();
            if (reg == null) return;
            var so = new SerializedObject(reg);
            var list = so.FindProperty("spawnPoints");
            if (list == null || !list.isArray) return;
            for (int i = 0; i < list.arraySize; i++)
            {
                var el = list.GetArrayElementAtIndex(i);
                var idProp = el.FindPropertyRelative("id");
                if (idProp != null && idProp.stringValue == spawnId)
                {
                    var posProp = el.FindPropertyRelative("position");
                    if (posProp != null) posProp.vector2Value = newPos;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    return;
                }
            }
        }

        private static void RebuildRiverMist()
        {
            var old = GameObject.Find("Ch2_RiverMist");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            var mistSpr = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2TerrainRoot + "/ch2_river_mist_strip.png");
            if (mistSpr == null) return;

            var root = new GameObject("Ch2_RiverMist");
            (Vector2 pos, Vector3 scale, float speed, float range, float alpha, float phase)[] specs =
            {
                (new Vector2(-3.0f,  9.8f), new Vector3(1.5f, 1.1f, 1f), 0.22f, 2.4f, 0.36f, 0.0f),
                (new Vector2( 0.5f,  5.6f), new Vector3(1.3f, 1.0f, 1f), 0.26f, 2.0f, 0.30f, 1.3f),
                (new Vector2(-2.5f,  2.4f), new Vector3(1.4f, 1.0f, 1f), 0.20f, 2.2f, 0.28f, 2.5f),
                (new Vector2( 1.5f, -3.5f), new Vector3(1.5f, 1.1f, 1f), 0.24f, 2.4f, 0.32f, 3.7f),
                (new Vector2(-1.5f, -8.5f), new Vector3(1.4f, 1.0f, 1f), 0.28f, 2.0f, 0.30f, 4.9f),
                (new Vector2(-14.5f, 7.2f), new Vector3(1.7f, 1.2f, 1f), 0.18f, 2.8f, 0.40f, 5.8f),
            };

            for (int i = 0; i < specs.Length; i++)
            {
                var s = specs[i];
                var go = new GameObject($"RiverMist_{i:D2}");
                go.transform.SetParent(root.transform, false);
                go.transform.position = new Vector3(s.pos.x, s.pos.y, 0f);
                go.transform.localScale = s.scale;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = mistSpr;
                sr.sortingLayerName = "WeatherVFX";
                sr.sortingOrder = 5 + i;
                sr.color = new Color(1f, 1f, 1f, s.alpha);
                go.AddComponent<RiverMistDrifter>().Configure(s.speed, s.range, s.alpha, s.phase);
            }
        }

        private static void RebuildStardewRiverbankDecor()
        {
            var old = GameObject.Find("Ch2_EnvironmentDecor");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            var root = new GameObject("Ch2_EnvironmentDecor");

            var sprOak = FindActiveCh1Sprite("tree_hero_oak", "Assets/Art/Sprites/Environment/Buildings/tree_hero_oak.png");
            var sprPine = FindActiveCh1Sprite("tree_hero_pine", "Assets/Art/Sprites/Environment/Buildings/tree_hero_pine.png");
            var sprBlossom = FindActiveCh1Sprite("tree_hero_blossom", "Assets/Art/Sprites/Environment/Buildings/tree_hero_blossom.png");
            var sprBush = FindActiveCh1Sprite("decor_bush", "Assets/Art/Sprites/Environment/Decor/decor_bush.png");
            var sprRock = FindActiveCh1Sprite("decor_rock", "Assets/Art/Sprites/Environment/Decor/decor_rock.png");
            var sprReeds = FindActiveCh1Sprite("decor_reeds", "Assets/Art/Sprites/Environment/Decor/decor_reeds.png");
            var sprPier = FindActiveCh1Sprite("decor_pond_pier", "Assets/Art/Sprites/Environment/Decor/decor_pond_pier.png");
            var sprBarrel = FindActiveCh1Sprite("decor_barrel", "Assets/Art/Sprites/Environment/Decor/decor_barrel.png");
            var sprMush = FindActiveCh1Sprite("decor_mushrooms", "Assets/Art/Sprites/Environment/Decor/decor_mushrooms.png");
            var sprLantern = FindActiveCh1Sprite("decor_garden_lantern", "Assets/Art/Sprites/Environment/Decor/decor_garden_lantern.png");
            var sprTall0 = FindActiveCh1Sprite("decor_tall_grass_0", "Assets/Art/Sprites/Environment/Decor/decor_tall_grass_0.png");
            var sprTall1 = FindActiveCh1Sprite("decor_tall_grass_1", "Assets/Art/Sprites/Environment/Decor/decor_tall_grass_1.png");
            var sprStep0 = FindActiveCh1Sprite("decor_stepping_0", "Assets/Art/Sprites/Environment/Decor/decor_stepping_0.png");
            var sprWell = FindActiveCh1Sprite("prop_well", "Assets/Art/Sprites/Environment/Props/prop_well.png");
            var sprFlw0 = FindActiveCh1Sprite("decor_flowers_0", "Assets/Art/Sprites/Environment/Decor/decor_flowers_0.png");
            var sprFlw1 = FindActiveCh1Sprite("decor_flowers_1", "Assets/Art/Sprites/Environment/Decor/decor_flowers_1.png");
            var sprFlw2 = FindActiveCh1Sprite("decor_flowers_2", "Assets/Art/Sprites/Environment/Decor/decor_flowers_2.png");
            var sprShadow = FindActiveCh1Sprite("spr_ground_shadow", "Assets/Art/Sprites/Environment/Shadows/spr_ground_shadow.png");
            var sprRiverRock0 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_river_rock_0.png");
            var sprRiverRock1 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_river_rock_1.png");
            var sprLily0 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_lilypad_0.png");
            var sprLily1 = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_lilypad_1.png");
            var sprFence = AssetDatabase.LoadAssetAtPath<Sprite>(Ch2OverhaulRoot + "/ch2_wood_fence_h.png");

            // Khung rừng hữu cơ dọc viền map (thay thế hoàn toàn tường gạch tile_tree xấu)
            Vector2[] oakPos =
            {
                new(23.8f, 10.5f), new(20.5f, 11.2f), new(15.2f, 11.0f), new(8.8f, 10.8f),
                new(24.0f, 4.5f), new(23.8f, -5.0f), new(22.5f, -10.5f), new(16.8f, -11.2f),
                new(-11.8f, -10.8f), new(-18.5f, -10.2f), new(-23.5f, -7.5f)
            };
            foreach (var p in oakPos) SpawnDecor(root.transform, "Tree_Oak", sprOak, p, "WorldObjects", sprShadow, 1.4f, true, new Vector2(1.0f, 0.65f), new Vector2(0f, -0.85f), 0.30f);

            Vector2[] pinePos =
            {
                new(-14.5f, 11.2f), new(-19.5f, 11.0f), new(-23.8f, 9.2f), new(-24.0f, 4.5f),
                new(-23.8f, -1.5f), new(-17.8f, 2.2f), new(12.2f, -11.2f), new(24.0f, -8.5f)
            };
            foreach (var p in pinePos) SpawnDecor(root.transform, "Tree_Pine", sprPine, p, "WorldObjects", sprShadow, 1.3f, true, new Vector2(0.9f, 0.6f), new Vector2(0f, -0.85f), 0.30f);

            Vector2[] blosPos = { new(23.2f, 7.6f), new(15.0f, 6.8f), new(19.5f, -4.2f) };
            foreach (var p in blosPos) SpawnDecor(root.transform, "Tree_Blossom", sprBlossom, p, "WorldObjects", sprShadow, 1.4f, true, new Vector2(1.0f, 0.65f), new Vector2(0f, -0.85f), 0.30f);

            // Hàng rào gỗ Stardew viền sân Vườn Ươm & lối ra cầu
            float[] fenceXs = { 16.0f, 17.0f, 20.0f, 21.0f };
            foreach (var fx in fenceXs)
                SpawnDecor(root.transform, "Fence_Nursery", sprFence, new Vector2(fx, 1.7f), "WorldObjects", null, 0f, true, new Vector2(0.95f, 0.35f), new Vector2(0f, -0.15f), 0.15f);

            // Cầu gỗ Bến Đò Ngang (Vịnh Nam)
            SpawnDecor(root.transform, "Pier_EastFerry", sprPier, new Vector2(9.3f, -7.5f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);
            SpawnDecor(root.transform, "Pier_WestFerry", sprPier, new Vector2(-7.4f, -7.5f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);

            // Lau sậy ôm sát đường cong bờ sông Đông & Tây
            (float x, float y)[] reedPts =
            {
                (6.3f, 10.2f), (7.3f, 6.5f), (8.2f, 2.6f), (8.2f, -3.2f), (9.3f, -6.2f), (9.3f, -9.2f),
                (-10.8f, 10.2f), (-9.8f, 6.4f), (-8.8f, 2.8f), (-8.8f, -3.5f), (-7.8f, -6.2f), (-7.8f, -9.2f)
            };
            foreach (var rp in reedPts)
                SpawnDecor(root.transform, "Reeds", sprReeds, new Vector2(rp.x, rp.y), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);

            // Đá cuội nhô mặt nước & Hoa súng / Lá sen trên sông (tạo chiều sâu nước thực)
            SpawnDecor(root.transform, "RiverRock_N1", sprRiverRock0, new Vector2(-2.5f, 7.2f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);
            SpawnDecor(root.transform, "RiverRock_N2", sprRiverRock1, new Vector2(1.2f, 4.4f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);
            SpawnDecor(root.transform, "RiverRock_S1", sprRiverRock0, new Vector2(-1.4f, -4.2f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);
            SpawnDecor(root.transform, "RiverRock_S2", sprRiverRock1, new Vector2(3.4f, -10.2f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);

            Vector2[] lilyPts = { new(-6.8f, 6.2f), new(-7.2f, 3.2f), new(-5.5f, -5.6f), new(5.8f, -9.2f), new(-5.8f, -9.6f), new(4.5f, 7.8f) };
            for (int i = 0; i < lilyPts.Length; i++)
                SpawnDecor(root.transform, "LilyPad", (i % 2 == 0) ? sprLily1 : sprLily0, lilyPts[i], "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);

            // Bụi cây, đá, nấm đầm lầy, hoa dại & tiểu cảnh sinh hoạt
            Vector2[] bushPos = { new(14.5f, 8.2f), new(18.5f, -6.8f), new(-12.5f, 5.5f), new(-16.8f, -6.4f), new(21.2f, -2.5f), new(8.2f, -4.8f) };
            foreach (var p in bushPos) SpawnDecor(root.transform, "Bush", sprBush, p, "WorldObjects", null, 0f, true, new Vector2(0.75f, 0.45f), new Vector2(0f, -0.2f), 0.15f);

            Vector2[] rockPos = { new(8.5f, 7.4f), new(-10.5f, 6.8f), new(-20.8f, 1.5f), new(-14.8f, -8.8f) };
            foreach (var p in rockPos) SpawnDecor(root.transform, "Rock", sprRock, p, "WorldObjects", null, 0f, true, new Vector2(0.75f, 0.45f), new Vector2(0f, -0.15f), 0.15f);

            Vector2[] mushPos = { new(-15.5f, 5.2f), new(-18.8f, 7.6f), new(-21.2f, 6.0f), new(-13.5f, 8.2f) };
            foreach (var p in mushPos) SpawnDecor(root.transform, "Mushrooms", sprMush, p, "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);

            Vector2[] flwPos = { new(16.8f, 4.4f), new(21.2f, 3.2f), new(13.0f, -4.5f), new(-11.2f, -5.8f), new(12.2f, -2.0f), new(18.0f, -1.8f), new(20.5f, 2.2f) };
            Sprite[] flws = { sprFlw0, sprFlw1, sprFlw2 };
            for (int i = 0; i < flwPos.Length; i++)
                SpawnDecor(root.transform, "Flower", flws[i % flws.Length], flwPos[i], "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);

            Vector2[] tallPos = { new(10.8f, -6.2f), new(16.2f, -7.2f), new(-13.2f, 2.2f), new(-18.2f, -3.5f), new(9.2f, 4.0f) };
            for (int i = 0; i < tallPos.Length; i++)
                SpawnDecor(root.transform, "TallGrass", (i % 2 == 0) ? sprTall0 : sprTall1, tallPos[i], "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);

            SpawnDecor(root.transform, "Prop_NurseryWell", sprWell, new Vector2(16.2f, 6.4f), "WorldObjects", sprShadow, 1.0f, true, new Vector2(1.0f, 0.6f), new Vector2(0f, -0.35f), 0.25f);
            SpawnDecor(root.transform, "Barrel_Workshop", sprBarrel, new Vector2(13.6f, 5.4f), "WorldObjects", null, 0f, true, new Vector2(0.6f, 0.45f), new Vector2(0f, -0.18f), 0.15f);
            SpawnDecor(root.transform, "Barrel_Dock", sprBarrel, new Vector2(-11.2f, -3.6f), "WorldObjects", null, 0f, true, new Vector2(0.6f, 0.45f), new Vector2(0f, -0.18f), 0.15f);
            SpawnDecor(root.transform, "Lantern_Nursery", sprLantern, new Vector2(18.6f, 3.4f), "WorldObjects", null, 0f, true, new Vector2(0.45f, 0.35f), new Vector2(0f, -0.25f), 0.15f);
            SpawnDecor(root.transform, "Lantern_Workshop", sprLantern, new Vector2(8.8f, 4.2f), "WorldObjects", null, 0f, true, new Vector2(0.45f, 0.35f), new Vector2(0f, -0.25f), 0.15f);
            SpawnDecor(root.transform, "Lantern_Camp", sprLantern, new Vector2(13.4f, -2.4f), "WorldObjects", null, 0f, true, new Vector2(0.45f, 0.35f), new Vector2(0f, -0.25f), 0.15f);
            SpawnDecor(root.transform, "Lantern_Dock", sprLantern, new Vector2(-11.2f, -2.2f), "WorldObjects", null, 0f, true, new Vector2(0.45f, 0.35f), new Vector2(0f, -0.25f), 0.15f);

            SpawnDecor(root.transform, "Step_01", sprStep0, new Vector2(14.5f, 0.5f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);
            SpawnDecor(root.transform, "Step_02", sprStep0, new Vector2(17.5f, 0.5f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);
            SpawnDecor(root.transform, "Step_03", sprStep0, new Vector2(20.5f, 0.5f), "GroundDecor", null, 0f, false, Vector2.zero, Vector2.zero, 0f);
        }

        private static void SpawnDecor(
            Transform parent, string name, Sprite spr, Vector2 pos, string sortingLayer,
            Sprite shadowSpr, float shadowScale, bool solidCollider, Vector2 colSize, Vector2 colOffset, float footYOffset)
        {
            if (spr == null) return;
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = sortingLayer;

            if (sortingLayer == "WorldObjects")
                go.AddComponent<YSortRenderer>().Configure(sr, false, true, footYOffset, 0);
            else
                sr.sortingOrder = (name.StartsWith("RiverRock") || name.StartsWith("LilyPad")) ? 4 : Mathf.RoundToInt(-pos.y * 10f);

            if (solidCollider)
            {
                go.layer = 10;
                var bc = go.AddComponent<BoxCollider2D>();
                bc.isTrigger = false;
                bc.size = colSize;
                bc.offset = colOffset;
            }

            if (shadowSpr != null && shadowScale > 0f)
            {
                var sh = new GameObject("GroundShadow");
                sh.transform.SetParent(go.transform, false);
                sh.transform.localPosition = new Vector3(0f, -0.55f, 0f);
                sh.transform.localScale = new Vector3(shadowScale, shadowScale * 0.7f, 1f);
                var shSr = sh.AddComponent<SpriteRenderer>();
                shSr.sprite = shadowSpr;
                shSr.sortingLayerName = "Shadows";
                shSr.color = new Color(1f, 1f, 1f, 0.55f);
            }
        }

        private static Sprite FindActiveCh1Sprite(string exactName, string preferredPath)
        {
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(preferredPath);
            if (s != null) return s;
            string[] guids = AssetDatabase.FindAssets(exactName + " t:Sprite", new[] { "Assets/Art" });
            foreach (var g in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(g);
                if (Path.GetFileNameWithoutExtension(p).Equals(exactName, StringComparison.OrdinalIgnoreCase))
                    return AssetDatabase.LoadAssetAtPath<Sprite>(p);
            }
            return null;
        }

        private static Sprite FindNPCFrame(string folderName, string animName, int index)
        {
            string direct = $"Assets/Art/Sprites/Animations/NPCs/{folderName}/{animName}_{index:D2}.png";
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(direct);
            if (s != null) return s;
            string[] guids = AssetDatabase.FindAssets($"{animName}_{index:D2} t:Sprite", new[] { "Assets/Art" });
            foreach (var g in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(g).Replace("\\", "/");
                if (p.IndexOf(folderName, StringComparison.OrdinalIgnoreCase) >= 0)
                    return AssetDatabase.LoadAssetAtPath<Sprite>(p);
            }
            return null;
        }

        private static GameObject CreateOrUpdateNPCObject(string name, Vector3 pos, string actorFolder, NPCData data, bool isCh1NpcLayout)
        {
            var go = GameObject.Find(name);
            if (go == null) go = new GameObject(name);
            go.transform.position = pos;
            go.tag = "NPC";
            go.layer = 11;

            var sr = EnsureComponent<SpriteRenderer>(go);
            var idleFrames = isCh1NpcLayout ? LoadCh1NpcFrames(actorFolder, "idle", 4) : LoadFrames(actorFolder, "idle", 4);
            var cheerFrames = isCh1NpcLayout ? LoadCh1NpcFrames(actorFolder, "cheer", 4) : LoadFrames(actorFolder, "cheer", 4);
            if (idleFrames.Length > 0) sr.sprite = idleFrames[0];
            sr.sortingLayerName = "WorldObjects";

            EnsureComponent<YSortRenderer>(go).Configure(sr, true, true, 0.05f, 0);

            var oldCircle = go.GetComponent<CircleCollider2D>();
            if (oldCircle != null) UnityEngine.Object.DestroyImmediate(oldCircle);

            var col = EnsureComponent<BoxCollider2D>(go);
            col.isTrigger = false;
            col.size = new Vector2(0.55f, 0.40f);
            col.offset = new Vector2(0f, -0.35f);

            if (go.transform.Find("GroundShadow") == null)
            {
                var shadowSpr = FindActiveCh1Sprite("spr_ground_shadow", "Assets/Art/Sprites/Environment/Shadows/spr_ground_shadow.png");
                if (shadowSpr != null)
                {
                    var sh = new GameObject("GroundShadow");
                    sh.transform.SetParent(go.transform, false);
                    sh.transform.localPosition = new Vector3(0f, -0.58f, 0f);
                    sh.transform.localScale = new Vector3(0.75f, 0.55f, 1f);
                    var shSr = sh.AddComponent<SpriteRenderer>();
                    shSr.sprite = shadowSpr;
                    shSr.sortingLayerName = "Shadows";
                    shSr.color = new Color(1f, 1f, 1f, 0.55f);
                }
            }

            var npcComp = EnsureComponent<NPCController>(go);
            var so = new SerializedObject(npcComp);
            SetObj(so, "npcData", data);
            SetObj(so, "spriteRenderer", sr);
            SetSpriteArray(so, "idleFrames", idleFrames);
            SetSpriteArray(so, "cheerFrames", cheerFrames);
            so.ApplyModifiedPropertiesWithoutUndo();
            return go;
        }

        private static Sprite[] LoadFrames(string actorFolder, string animName, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                string path = $"{CharsRoot}/{actorFolder}/{actorFolder}_{animName}_{i:D2}.png";
                var s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }

        private static Sprite[] LoadCh1NpcFrames(string actorFolder, string animName, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                var s = FindNPCFrame(actorFolder, animName, i);
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }

        private static void SetSpriteArray(SerializedObject so, string propName, Sprite[] frames)
        {
            var p = so.FindProperty(propName);
            if (p == null || !p.isArray) return;
            p.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++)
                p.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
        }

        private static void CreateOrUpdateClue(string name, Vector3 pos, string title, int stage, string prevFlag, string doneFlag, DialogueData dlg, string approachNpc = "")
        {
            var go = GameObject.Find(name);
            if (go == null) go = new GameObject(name);
            go.transform.position = pos;
            go.tag = "Interactable";
            go.layer = 11;

            var sr = EnsureComponent<SpriteRenderer>(go);
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(CharsRoot + "/actor_storm_lantern/actor_storm_lantern_broken_east_00.png");
            sr.sortingLayerName = "WorldObjects";
            sr.color = new Color(0.95f, 0.9f, 0.7f, 0.95f);

            EnsureComponent<YSortRenderer>(go).Configure(sr, false, true, 0.15f, 0);

            var col = EnsureComponent<CircleCollider2D>(go);
            col.isTrigger = true;
            col.radius = 0.75f;

            EnsureComponent<InvestigationClueInteractable>(go).Configure(title, stage, prevFlag, doneFlag, dlg, true, approachNpc);
        }

        private static void SetObj(SerializedObject so, string prop, UnityEngine.Object val)
        {
            var p = so.FindProperty(prop);
            if (p != null && p.propertyType == SerializedPropertyType.ObjectReference) p.objectReferenceValue = val;
        }
    }
}
