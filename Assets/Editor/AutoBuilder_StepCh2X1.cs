using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2X1
    {
        private const string RiverScenePath = "Assets/Scenes/Riverbank_Chapter2.unity";
        private const string SwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";
        private const string LanternDir = "Assets/Art/Characters/actor_bridge_lantern";

        private static readonly Vector3 EastStationPos = new Vector3(9.4f, 2.05f, 0f);
        private static readonly Vector3 WestStationPos = new Vector3(-9.8f, 2.05f, 0f);
        private static readonly Vector3 KaelRelayStandPos = new Vector3(9.4f, 2.55f, 0f);
        private static readonly Vector3 MaraRelayStandPos = new Vector3(-9.8f, 2.55f, 0f);

        [MenuItem("Moonpetal/Step Ch2-X1/1. Apply Bridge Lantern Art, Unblock Bridge & Swarm Aggro", false, 530)]
        public static void ApplyStepCh2X1()
        {
            ConfigureLanternImporters();
            AssetDatabase.Refresh();

            Sprite[] litFrames = LoadFrames("actor_bridge_lantern_lit_", 4);
            Sprite[] flickerFrames = LoadFrames("actor_bridge_lantern_flicker_", 3);
            Sprite[] offFrames = LoadFrames("actor_bridge_lantern_off_", 2);

            ApplyRiverbankFixes(litFrames, flickerFrames, offFrames);
            ApplySwampAggroTrigger();

            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Ch2-X1] HOAN TAT: Da cap nhat art 6 den cau + animation, doi ban/Kael/Mara khoi cua cau, va kich hoat Swamp Swarm Aggro!");
            ValidateStepCh2X1();
        }

        [MenuItem("Moonpetal/Step Ch2-X1/2. Validate Step Ch2-X1", false, 531)]
        public static void ValidateStepCh2X1()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-X1] === BAT DAU VALIDATE STEP CH2-X1 ===");

            for (int i = 0; i < 4; i++)
                if (AssetDatabase.LoadAssetAtPath<Sprite>(LanternDir + "/actor_bridge_lantern_lit_0" + i + ".png") == null)
                { Debug.LogError("[MV][Ch2-X1][FAIL] Thieu actor_bridge_lantern_lit_0" + i + ".png"); errors++; }
            for (int i = 0; i < 3; i++)
                if (AssetDatabase.LoadAssetAtPath<Sprite>(LanternDir + "/actor_bridge_lantern_flicker_0" + i + ".png") == null)
                { Debug.LogError("[MV][Ch2-X1][FAIL] Thieu actor_bridge_lantern_flicker_0" + i + ".png"); errors++; }
            for (int i = 0; i < 2; i++)
                if (AssetDatabase.LoadAssetAtPath<Sprite>(LanternDir + "/actor_bridge_lantern_off_0" + i + ".png") == null)
                { Debug.LogError("[MV][Ch2-X1][FAIL] Thieu actor_bridge_lantern_off_0" + i + ".png"); errors++; }

            Scene river = EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);
            BridgeLanternPost[] posts = Object.FindObjectsByType<BridgeLanternPost>(FindObjectsInactive.Include);
            if (posts.Length != 6)
            {
                Debug.LogError("[MV][Ch2-X1][FAIL] Riverbank_Chapter2 can 6 BridgeLanternPost, hien co: " + posts.Length);
                errors++;
            }
            for (int i = 0; i < posts.Length; i++)
            {
                SerializedObject pso = new SerializedObject(posts[i]);
                var litArr = pso.FindProperty("litFrames");
                var flkArr = pso.FindProperty("flickerFrames");
                var offArr = pso.FindProperty("offFrames");
                if (litArr == null || litArr.arraySize < 4 || flkArr == null || flkArr.arraySize < 3 || offArr == null || offArr.arraySize < 2)
                {
                    Debug.LogError("[MV][Ch2-X1][FAIL] " + posts[i].name + " chua gan du 9 frame animation den cau!");
                    errors++;
                }
            }

            GameObject stEast = GameObject.Find("RelayStation_East");
            GameObject stWest = GameObject.Find("RelayStation_West");
            if (stEast == null || stEast.transform.position.y < 1.75f)
            { Debug.LogError("[MV][Ch2-X1][FAIL] RelayStation_East chua duoc doi len le Bac (y >= 1.75)!"); errors++; }
            if (stWest == null || stWest.transform.position.y < 1.75f)
            { Debug.LogError("[MV][Ch2-X1][FAIL] RelayStation_West chua duoc doi len le Bac (y >= 1.75)!"); errors++; }

            LanternRelayManager relay = Object.FindAnyObjectByType<LanternRelayManager>();
            if (relay != null)
            {
                SerializedObject rso = new SerializedObject(relay);
                Vector3 kp = rso.FindProperty("kaelRelayPos").vector3Value;
                Vector3 mp = rso.FindProperty("maraRelayPos").vector3Value;
                if (kp.y < 1.9f || mp.y < 1.9f)
                { Debug.LogError("[MV][Ch2-X1][FAIL] kaelRelayPos/maraRelayPos van chan cua cau!"); errors++; }
            }

            Scene swamp = EditorSceneManager.OpenScene(SwampScenePath, OpenSceneMode.Single);
            SwampWaveCombatController wave = Object.FindAnyObjectByType<SwampWaveCombatController>();
            if (wave == null)
            {
                Debug.LogError("[MV][Ch2-X1][FAIL] WestSwamp_Chapter2 thieu SwampWaveCombatController!");
                errors++;
            }
            else
            {
                SerializedObject wso = new SerializedObject(wave);
                float trigY = wso.FindProperty("triggerEnterMinY").floatValue;
                if (trigY > -8.0f)
                {
                    Debug.LogError("[MV][Ch2-X1][FAIL] triggerEnterMinY van qua cao (" + trigY + "), can <= -8.0f!");
                    errors++;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-X1] === VALIDATE PASS 100% (0 LOI) ===");
            else
                Debug.LogError("[MV][Ch2-X1] === VALIDATE FAIL (" + errors + " LOI) ===");
        }

        private static void ConfigureLanternImporters()
        {
            string[] names =
            {
                "actor_bridge_lantern_lit_00.png", "actor_bridge_lantern_lit_01.png",
                "actor_bridge_lantern_lit_02.png", "actor_bridge_lantern_lit_03.png",
                "actor_bridge_lantern_flicker_00.png", "actor_bridge_lantern_flicker_01.png",
                "actor_bridge_lantern_flicker_02.png",
                "actor_bridge_lantern_off_00.png", "actor_bridge_lantern_off_01.png"
            };
            for (int i = 0; i < names.Length; i++)
            {
                string path = LanternDir + "/" + names[i];
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;
                ti.textureType = TextureImporterType.Sprite;
                ti.spriteImportMode = SpriteImportMode.Single;
                ti.spritePixelsPerUnit = 32f;
                ti.filterMode = FilterMode.Point;
                ti.textureCompression = TextureImporterCompression.Uncompressed;
                ti.alphaIsTransparency = true;
                var settings = new TextureImporterSettings();
                ti.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                settings.spritePivot = new Vector2(0.5f, 0f);
                ti.SetTextureSettings(settings);
                ti.SaveAndReimport();
            }
        }

        private static Sprite[] LoadFrames(string prefix, int count)
        {
            List<Sprite> list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                string p = LanternDir + "/" + prefix + "0" + i + ".png";
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(p);
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }

        private static void ApplyRiverbankFixes(Sprite[] litFrames, Sprite[] flickerFrames, Sprite[] offFrames)
        {
            Scene river = EditorSceneManager.OpenScene(RiverScenePath, OpenSceneMode.Single);

            BridgeLanternPost[] posts = Object.FindObjectsByType<BridgeLanternPost>(FindObjectsInactive.Include);
            for (int i = 0; i < posts.Length; i++)
            {
                BridgeLanternPost post = posts[i];
                SerializedObject pso = new SerializedObject(post);
                if (litFrames.Length > 0) pso.FindProperty("litSprite").objectReferenceValue = litFrames[0];
                if (flickerFrames.Length > 0) pso.FindProperty("dimSprite").objectReferenceValue = flickerFrames[0];
                if (offFrames.Length > 0) pso.FindProperty("outSprite").objectReferenceValue = offFrames[0];
                AssignSpriteArray(pso.FindProperty("litFrames"), litFrames);
                AssignSpriteArray(pso.FindProperty("flickerFrames"), flickerFrames);
                AssignSpriteArray(pso.FindProperty("offFrames"), offFrames);
                pso.FindProperty("animFps").floatValue = 6f;
                pso.ApplyModifiedPropertiesWithoutUndo();

                SpriteRenderer sr = post.GetComponent<SpriteRenderer>();
                if (sr != null && litFrames.Length > 0)
                {
                    sr.sprite = litFrames[0];
                    sr.sortingLayerName = "WorldObjects";
                }
                EditorUtility.SetDirty(post);
            }

            MoveStation("RelayStation_East", EastStationPos, 2.65f);
            MoveStation("RelayStation_West", WestStationPos, 2.65f);

            LanternRelayManager relay = Object.FindAnyObjectByType<LanternRelayManager>();
            if (relay != null)
            {
                SerializedObject rso = new SerializedObject(relay);
                rso.FindProperty("kaelRelayPos").vector3Value = KaelRelayStandPos;
                rso.FindProperty("maraRelayPos").vector3Value = MaraRelayStandPos;
                rso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(relay);
            }

            EnsureNpcClearOfBridge("NPC_Kael", new Vector3(11.0f, 3.3f, 0f));
            EnsureNpcClearOfBridge("NPC_Mara", new Vector3(-12.6f, -4.6f, 0f));

            EditorSceneManager.MarkSceneDirty(river);
            EditorSceneManager.SaveScene(river);
        }

        private static void MoveStation(string goName, Vector3 newPos, float interactRadius)
        {
            GameObject go = GameObject.Find(goName);
            if (go == null) return;
            go.transform.position = newPos;
            LanternRelayStation st = go.GetComponent<LanternRelayStation>();
            if (st != null)
            {
                SerializedObject sso = new SerializedObject(st);
                SerializedProperty radProp = sso.FindProperty("interactRadius");
                if (radProp != null) radProp.floatValue = interactRadius;
                sso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(st);
            }
            EditorUtility.SetDirty(go);
        }

        private static void EnsureNpcClearOfBridge(string npcName, Vector3 safeFallback)
        {
            GameObject npc = GameObject.Find(npcName);
            if (npc == null) return;
            Vector3 p = npc.transform.position;
            if (p.y > -1.2f && p.y < 1.6f && Mathf.Abs(p.x) <= 11.5f)
            {
                npc.transform.position = safeFallback;
                EditorUtility.SetDirty(npc);
            }
        }

        private static void ApplySwampAggroTrigger()
        {
            Scene swamp = EditorSceneManager.OpenScene(SwampScenePath, OpenSceneMode.Single);
            SwampWaveCombatController wave = Object.FindAnyObjectByType<SwampWaveCombatController>();
            if (wave != null)
            {
                SerializedObject wso = new SerializedObject(wave);
                wso.FindProperty("triggerEnterMinY").floatValue = -8.6f;
                wso.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(wave);
                EditorSceneManager.MarkSceneDirty(swamp);
                EditorSceneManager.SaveScene(swamp);
            }
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
    }
}
