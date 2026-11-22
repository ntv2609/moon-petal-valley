using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step13
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string NpcAnimRoot = "Assets/Art/Sprites/Animations/NPCs";

        private static readonly (string objName, string npcId)[] NpcEntries =
        {
            ("NPC_Bach", "npc_bach"),
            ("NPC_Hana", "npc_hana"),
            ("NPC_Toby", "npc_toby"),
            ("NPC_Otis", "npc_otis")
        };

        [MenuItem("Moonpetal/Step 13/1. Wire 4 NPCs Animations (Bach, Hana, Toby, Otis)")]
        public static void BuildStep13()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 13: ĐỒNG BỘ 4 NPC ===");
            AssetDatabase.Refresh();
            ForceReimportNpcSprites();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            foreach (var entry in NpcEntries)
            {
                Sprite[] idleF = LoadFrames(entry.npcId, "idle", 4);
                Sprite[] cheerF = LoadFrames(entry.npcId, "cheer", 4);

                // Cập nhật NPCData asset
                var npcData = AssetDatabase.LoadAssetAtPath<NPCData>("Assets/Data/NPCs/NPC_" + entry.npcId + ".asset");
                if (npcData != null && idleF[0] != null)
                {
                    npcData.worldSprite = idleF[0];
                    EditorUtility.SetDirty(npcData);
                }

                // Cập nhật GameObject trong scene
                GameObject go = GameObject.Find(entry.objName);
                if (go == null)
                {
                    Debug.LogError("[MV] Không tìm thấy " + entry.objName + " trong scene!");
                    continue;
                }

                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null && idleF[0] != null) sr.sprite = idleF[0];

                var ctrl = go.GetComponent<NPCController>();
                if (ctrl != null)
                {
                    var so = new SerializedObject(ctrl);
                    so.FindProperty("spriteRenderer").objectReferenceValue = sr;
                    AssignArray(so.FindProperty("idleFrames"), idleF);
                    AssignArray(so.FindProperty("cheerFrames"), cheerF);
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 13! Chạy 'Moonpetal/Step 13/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 13/Validate")]
        public static void ValidateStep13()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 13 ===");

            int framesOk = 0;
            foreach (var entry in NpcEntries)
            {
                foreach (string anim in new[] { "idle", "cheer" })
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string path = NpcAnimRoot + "/" + entry.npcId + "/" + anim + "_" + i.ToString("00") + ".png";
                        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        if (s == null)
                        {
                            Debug.LogError("[MV] THIẾU sprite NPC: " + path);
                            errors++;
                        }
                        else if (Mathf.RoundToInt(s.rect.width) != 32 || Mathf.RoundToInt(s.rect.height) != 32)
                        {
                            Debug.LogError("[MV] SAI kích thước FullRect trên " + path + ": " + s.rect.width + "x" + s.rect.height);
                            errors++;
                        }
                        else
                        {
                            framesOk++;
                        }
                    }
                }

                GameObject go = GameObject.Find(entry.objName);
                var ctrl = go != null ? go.GetComponent<NPCController>() : null;
                if (ctrl == null)
                {
                    Debug.LogError("[MV] Thiếu NPCController trên " + entry.objName);
                    errors++;
                }
                else
                {
                    var so = new SerializedObject(ctrl);
                    if (so.FindProperty("idleFrames").arraySize < 4 || so.FindProperty("cheerFrames").arraySize < 4)
                    {
                        Debug.LogError("[MV] " + entry.objName + " chưa được nối đủ 4 frame idle & 4 frame cheer.");
                        errors++;
                    }
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 13: PASS (0 lỗi). Cả 4 NPC (" + framesOk + "/32 frames FullRect 32x32) đã sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 13: FAIL (" + errors + " lỗi).");
            }
        }

        private static void ForceReimportNpcSprites()
        {
            if (!Directory.Exists(NpcAnimRoot)) return;
            foreach (string file in Directory.GetFiles(NpcAnimRoot, "*.png", SearchOption.AllDirectories))
            {
                string assetPath = file.Replace("\\", "/");
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 20;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.alphaIsTransparency = true;
                    importer.mipmapEnabled = false;

                    var settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    settings.spriteAlignment = (int)SpriteAlignment.Center;
                    settings.spriteGenerateFallbackPhysicsShape = false;
                    importer.SetTextureSettings(settings);
                    importer.SaveAndReimport();
                }
            }
        }

        private static Sprite[] LoadFrames(string npcId, string anim, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                string path = NpcAnimRoot + "/" + npcId + "/" + anim + "_" + i.ToString("00") + ".png";
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(path));
            }
            return list.ToArray();
        }

        private static void AssignArray(SerializedProperty prop, Sprite[] sprites)
        {
            if (prop == null) return;
            prop.ClearArray();
            for (int i = 0; i < sprites.Length; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            }
        }
    }
}
