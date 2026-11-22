using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step24
    {
        private const string PortraitFolder = "Assets/Art/Sprites/Portraits";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        private static readonly string[] PortraitFiles =
        {
            "portrait_wen.png",
            "portrait_lumi.png",
            "portrait_bach.png",
            "portrait_hana.png",
            "portrait_toby.png",
            "portrait_otis.png"
        };

        [MenuItem("Moonpetal/Step 24/1. Wire 6 Standing Character Portraits to Dialogue UI & NPCs")]
        public static void WireStandingPortraits()
        {
            ConfigurePortraitImporters();

            Sprite sprWen = LoadPortrait("portrait_wen.png");
            Sprite sprLumi = LoadPortrait("portrait_lumi.png");
            Sprite sprBach = LoadPortrait("portrait_bach.png");
            Sprite sprHana = LoadPortrait("portrait_hana.png");
            Sprite sprToby = LoadPortrait("portrait_toby.png");
            Sprite sprOtis = LoadPortrait("portrait_otis.png");

            if (sprWen == null || sprLumi == null || sprBach == null || sprHana == null || sprToby == null || sprOtis == null)
            {
                Debug.LogError("[MV][Step24] Thieu it nhat 1 anh trong Assets/Art/Sprites/Portraits/. Hay kiem tra lai deploy.py.");
                return;
            }

            // 1. Cap nhat GameRoot.prefab
            if (AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath) == null)
            {
                Debug.LogError("[MV][Step24] Khong tim thay " + GameRootPrefabPath);
                return;
            }

            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            try
            {
                WireDialogueHierarchy(prefabRoot, sprWen, sprLumi, sprBach, sprHana, sprToby, sprOtis);
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, GameRootPrefabPath);
                Debug.Log("[MV][Step24] Da cap nhat StandingPortrait + 6 Sprite tren GameRoot.prefab.");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }

            // 2. Cap nhat NPCData.portraitSprite cho 4 NPC
            WireNPCDataPortraits(sprBach, sprHana, sprToby, sprOtis);

            // 3. Dong bo luon GameRoot trong scene dang mo neu co
            DialogueManager sceneDmg = UnityEngine.Object.FindAnyObjectByType<DialogueManager>();
            if (sceneDmg != null && !PrefabUtility.IsPartOfPrefabAsset(sceneDmg.gameObject))
            {
                WireDialogueHierarchy(sceneDmg.transform.root.gameObject, sprWen, sprLumi, sprBach, sprHana, sprToby, sprOtis);
                EditorSceneManager.MarkSceneDirty(sceneDmg.gameObject.scene);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV][Step24] HOAN TAT GAN 6 STANDING CHARACTER PORTRAITS!");
        }

        private static void ConfigurePortraitImporters()
        {
            foreach (string fname in PortraitFiles)
            {
                string path = PortraitFolder + "/" + fname;
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool changed = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; changed = true; }
                if (importer.spriteImportMode != SpriteImportMode.Single) { importer.spriteImportMode = SpriteImportMode.Single; changed = true; }
                if (Mathf.Abs(importer.spritePixelsPerUnit - 100f) > 0.01f) { importer.spritePixelsPerUnit = 100f; changed = true; }
                if (importer.filterMode != FilterMode.Bilinear) { importer.filterMode = FilterMode.Bilinear; changed = true; }
                if (importer.mipmapEnabled) { importer.mipmapEnabled = false; changed = true; }
                if (!importer.alphaIsTransparency) { importer.alphaIsTransparency = true; changed = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; changed = true; }

                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                if (settings.spriteMeshType != SpriteMeshType.FullRect)
                {
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    importer.SetTextureSettings(settings);
                    changed = true;
                }

                if (changed)
                {
                    importer.SaveAndReimport();
                }
            }
            AssetDatabase.Refresh();
        }

        private static Sprite LoadPortrait(string fname)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(PortraitFolder + "/" + fname);
        }

        private static void WireDialogueHierarchy(
            GameObject root,
            Sprite sprWen, Sprite sprLumi, Sprite sprBach,
            Sprite sprHana, Sprite sprToby, Sprite sprOtis)
        {
            DialogueUIView uiView = root.GetComponentInChildren<DialogueUIView>(true);
            if (uiView != null)
            {
                SerializedObject uiSo = new SerializedObject(uiView);
                GameObject panelRoot = uiSo.FindProperty("panelRoot").objectReferenceValue as GameObject;
                Transform parentTf = panelRoot != null ? panelRoot.transform : uiView.transform;

                Transform existing = parentTf.Find("StandingPortrait");
                GameObject portraitGo;
                if (existing != null)
                {
                    portraitGo = existing.gameObject;
                }
                else
                {
                    portraitGo = new GameObject("StandingPortrait", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    portraitGo.transform.SetParent(parentTf, false);
                }

                RectTransform rt = portraitGo.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(1f, 0f);
                rt.anchoredPosition = new Vector2(-16f, -4f);
                rt.sizeDelta = new Vector2(320f, 380f);
                rt.localScale = Vector3.one;

                Image img = portraitGo.GetComponent<Image>();
                img.sprite = sprBach;
                img.color = Color.white;
                img.preserveAspect = true;
                img.raycastTarget = false;

                portraitGo.SetActive(false);

                uiSo.FindProperty("standingPortraitImage").objectReferenceValue = img;
                uiSo.FindProperty("standingPortraitRect").objectReferenceValue = rt;
                uiSo.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(uiView);
            }
            else
            {
                Debug.LogWarning("[MV][Step24] Khong tim thay DialogueUIView trong " + root.name);
            }

            DialogueManager dmg = root.GetComponentInChildren<DialogueManager>(true);
            if (dmg != null)
            {
                SerializedObject dmgSo = new SerializedObject(dmg);
                dmgSo.FindProperty("portraitWen").objectReferenceValue = sprWen;
                dmgSo.FindProperty("portraitLumi").objectReferenceValue = sprLumi;
                dmgSo.FindProperty("portraitBach").objectReferenceValue = sprBach;
                dmgSo.FindProperty("portraitHana").objectReferenceValue = sprHana;
                dmgSo.FindProperty("portraitToby").objectReferenceValue = sprToby;
                dmgSo.FindProperty("portraitOtis").objectReferenceValue = sprOtis;
                dmgSo.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(dmg);
            }
            else
            {
                Debug.LogWarning("[MV][Step24] Khong tim thay DialogueManager trong " + root.name);
            }
        }

        private static void WireNPCDataPortraits(Sprite sprBach, Sprite sprHana, Sprite sprToby, Sprite sprOtis)
        {
            string[] guids = AssetDatabase.FindAssets("t:NPCData", new[] { "Assets/Data" });
            int updated = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                NPCData npc = AssetDatabase.LoadAssetAtPath<NPCData>(path);
                if (npc == null) continue;

                string key = ((npc.npcID ?? "") + " " + npc.name).ToLowerInvariant();
                Sprite match = null;
                if (key.Contains("bach")) match = sprBach;
                else if (key.Contains("hana")) match = sprHana;
                else if (key.Contains("toby")) match = sprToby;
                else if (key.Contains("otis")) match = sprOtis;

                if (match != null)
                {
                    npc.portraitSprite = match;
                    EditorUtility.SetDirty(npc);
                    updated++;
                }
            }
            Debug.Log("[MV][Step24] Da cap nhat portraitSprite cho " + updated + " NPCData assets.");
        }

        [MenuItem("Moonpetal/Step 24/2. Validate Standing Portraits")]
        public static void ValidateStandingPortraits()
        {
            int errors = 0;

            foreach (string fname in PortraitFiles)
            {
                Sprite s = LoadPortrait(fname);
                if (s == null)
                {
                    Debug.LogError("[MV][Step24][FAIL] Thieu Sprite: " + PortraitFolder + "/" + fname);
                    errors++;
                }
            }

            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefabAsset == null)
            {
                Debug.LogError("[MV][Step24][FAIL] Khong tim thay GameRoot.prefab.");
                errors++;
            }
            else
            {
                DialogueUIView ui = prefabAsset.GetComponentInChildren<DialogueUIView>(true);
                if (ui == null)
                {
                    Debug.LogError("[MV][Step24][FAIL] Thieu DialogueUIView tren GameRoot.prefab.");
                    errors++;
                }
                else
                {
                    SerializedObject uiSo = new SerializedObject(ui);
                    if (uiSo.FindProperty("standingPortraitImage").objectReferenceValue == null)
                    {
                        Debug.LogError("[MV][Step24][FAIL] DialogueUIView.standingPortraitImage dang null.");
                        errors++;
                    }
                    if (uiSo.FindProperty("standingPortraitRect").objectReferenceValue == null)
                    {
                        Debug.LogError("[MV][Step24][FAIL] DialogueUIView.standingPortraitRect dang null.");
                        errors++;
                    }
                }

                DialogueManager dmg = prefabAsset.GetComponentInChildren<DialogueManager>(true);
                if (dmg == null)
                {
                    Debug.LogError("[MV][Step24][FAIL] Thieu DialogueManager tren GameRoot.prefab.");
                    errors++;
                }
                else
                {
                    SerializedObject dmgSo = new SerializedObject(dmg);
                    string[] props = { "portraitWen", "portraitLumi", "portraitBach", "portraitHana", "portraitToby", "portraitOtis" };
                    foreach (string p in props)
                    {
                        if (dmgSo.FindProperty(p).objectReferenceValue == null)
                        {
                            Debug.LogError("[MV][Step24][FAIL] DialogueManager." + p + " dang null.");
                            errors++;
                        }
                    }
                }
            }

            string[] npcGuids = AssetDatabase.FindAssets("t:NPCData", new[] { "Assets/Data" });
            int npcWithPortrait = 0;
            foreach (string g in npcGuids)
            {
                NPCData npc = AssetDatabase.LoadAssetAtPath<NPCData>(AssetDatabase.GUIDToAssetPath(g));
                if (npc != null && npc.portraitSprite != null) npcWithPortrait++;
            }
            if (npcWithPortrait < 4)
            {
                Debug.LogError("[MV][Step24][FAIL] Chi co " + npcWithPortrait + "/4 NPCData co portraitSprite.");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV][Step24] VALIDATE PASS: Du 6 Standing Portraits, DialogueUIView, DialogueManager va 4/4 NPCData deu hop le!");
            }
            else
            {
                Debug.LogError("[MV][Step24] VALIDATE FAIL (" + errors + " loi).");
            }
        }
    }
}
