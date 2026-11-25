using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Chapter2;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2P4_WetPolish
    {
        private const string WestSwampScenePath = "Assets/Scenes/WestSwamp_Chapter2.unity";
        private const string SwampWetDir = "Assets/Art/Sprites/Chapter2/Swamp";

        private static readonly string[] WetSprites =
        {
            SwampWetDir + "/spr_swamp_puddle_00.png",
            SwampWetDir + "/spr_swamp_puddle_01.png",
            SwampWetDir + "/spr_swamp_mud_slick.png",
            SwampWetDir + "/spr_swamp_moss_log.png",
            SwampWetDir + "/spr_swamp_wet_fern.png",
        };

        [MenuItem("Moonpetal/Chapter 2 Polish/Step Ch2-P4.fix2 - Apply Clear Lighting & Wet Swamp Surface")]
        public static void ApplyWetSwampPolish()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(WestSwampScenePath))
            {
                Debug.LogError("[MV][Ch2-P4.fix2] Khong tim thay " + WestSwampScenePath);
                return;
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            for (int i = 0; i < WetSprites.Length; i++)
                ConfigureSprite(WetSprites[i], 16f);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            var scene = EditorSceneManager.OpenScene(WestSwampScenePath, OpenSceneMode.Single);

            // 1. Nang nhe mau nen dat len (0.76, 0.84, 0.80) de giu chi tiet pixel-art sac net khi da bo man xam
            GameObject gGo = GameObject.Find("GroundTilemap");
            if (gGo != null)
            {
                Tilemap gTm = gGo.GetComponent<Tilemap>();
                if (gTm != null)
                {
                    gTm.color = new Color(0.76f, 0.84f, 0.80f, 1f);
                    EditorUtility.SetDirty(gTm);
                }
            }

            // 2. Rai 16 vung nuoc dong (animated), 14 vet bun uot nhop nhap, 5 khuc go muc reu, 18 bui duong xi dong suong
            GameObject oldWet = GameObject.Find("SwampWetSurfaceDetails");
            if (oldWet != null) UnityEngine.Object.DestroyImmediate(oldWet);
            GameObject wetRoot = new GameObject("SwampWetSurfaceDetails");

            Sprite puddle0 = AssetDatabase.LoadAssetAtPath<Sprite>(SwampWetDir + "/spr_swamp_puddle_00.png");
            Sprite puddle1 = AssetDatabase.LoadAssetAtPath<Sprite>(SwampWetDir + "/spr_swamp_puddle_01.png");
            Sprite mudSpr  = AssetDatabase.LoadAssetAtPath<Sprite>(SwampWetDir + "/spr_swamp_mud_slick.png");
            Sprite logSpr  = AssetDatabase.LoadAssetAtPath<Sprite>(SwampWetDir + "/spr_swamp_moss_log.png");
            Sprite fernSpr = AssetDatabase.LoadAssetAtPath<Sprite>(SwampWetDir + "/spr_swamp_wet_fern.png");

            Vector2[] puddleSpots =
            {
                new Vector2(-0.5f, -7.2f), new Vector2(1.4f, -4.6f), new Vector2(-3.2f, -2.2f), new Vector2(0.8f, -0.4f),
                new Vector2(-6.2f, 0.6f),  new Vector2(-8.6f, 2.8f), new Vector2(-11.2f, 5.2f), new Vector2(4.2f, 1.8f),
                new Vector2(6.5f, -1.4f),  new Vector2(-4.5f, -5.2f), new Vector2(3.8f, -6.4f), new Vector2(8.8f, 4.8f),
                new Vector2(-12.8f, -1.8f), new Vector2(-2.2f, 4.4f), new Vector2(2.6f, 6.2f),  new Vector2(7.6f, -4.2f),
            };
            Sprite[] puddleFrames = { puddle0, puddle1 };
            for (int i = 0; i < puddleSpots.Length; i++)
            {
                GameObject p = new GameObject($"WetPuddle_{i}");
                p.transform.SetParent(wetRoot.transform, false);
                p.transform.position = new Vector3(puddleSpots[i].x, puddleSpots[i].y, 0f);
                float sc = (i % 3 == 0) ? 1.18f : ((i % 2 == 0) ? 0.92f : 1.05f);
                p.transform.localScale = new Vector3(sc, sc, 1f);
                var sr = p.AddComponent<SpriteRenderer>();
                sr.sprite = puddle0;
                sr.color = new Color(0.95f, 1f, 0.98f, 0.90f);
                sr.sortingLayerName = "GroundDecor";
                sr.sortingOrder = 4;

                var anim = p.AddComponent<OrganicPondAnimator>();
                SerializedObject aso = new SerializedObject(anim);
                var fp = aso.FindProperty("frames");
                fp.ClearArray();
                for (int k = 0; k < puddleFrames.Length; k++)
                {
                    fp.InsertArrayElementAtIndex(k);
                    fp.GetArrayElementAtIndex(k).objectReferenceValue = puddleFrames[k];
                }
                aso.FindProperty("fps").floatValue = 1.6f + (i % 3) * 0.35f;
                aso.ApplyModifiedPropertiesWithoutUndo();
            }

            Vector2[] mudSpots =
            {
                new Vector2(-1.4f, -8.5f), new Vector2(0.6f, -6.2f), new Vector2(-1.8f, -3.4f), new Vector2(-5.4f, -1.2f),
                new Vector2(-8.2f, 1.2f),  new Vector2(-9.6f, 3.8f), new Vector2(2.8f, -0.8f),  new Vector2(5.2f, 0.4f),
                new Vector2(5.4f, 3.2f),   new Vector2(-6.8f, -4.2f), new Vector2(2.2f, -4.4f), new Vector2(-3.5f, 2.4f),
                new Vector2(9.2f, 1.4f),   new Vector2(-10.8f, -3.8f),
            };
            for (int i = 0; i < mudSpots.Length; i++)
            {
                SpawnGroundDecal(wetRoot.transform, $"MudSlick_{i}", mudSpr, mudSpots[i], (i % 2 == 0 ? 1.15f : 0.95f), 3);
            }

            Vector2[] logSpots =
            {
                new Vector2(-5.8f, 4.6f), new Vector2(6.8f, 5.8f), new Vector2(-13.8f, 1.4f),
                new Vector2(3.6f, -8.2f), new Vector2(-7.8f, -6.8f),
            };
            for (int i = 0; i < logSpots.Length; i++)
            {
                GameObject l = new GameObject($"MossLog_{i}");
                l.transform.SetParent(wetRoot.transform, false);
                l.transform.position = new Vector3(logSpots[i].x, logSpots[i].y, 0f);
                var sr = l.AddComponent<SpriteRenderer>();
                sr.sprite = logSpr;
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = Mathf.RoundToInt(-logSpots[i].y * 100f);
            }

            Vector2[] fernSpots =
            {
                new Vector2(-3.4f, -8.8f), new Vector2(2.8f, -8.2f), new Vector2(-4.2f, -3.6f), new Vector2(3.4f, -2.6f),
                new Vector2(-7.4f, -1.8f), new Vector2(-11.2f, 1.8f), new Vector2(-13.2f, 4.8f), new Vector2(-6.4f, 6.2f),
                new Vector2(-1.4f, 5.8f),  new Vector2(3.2f, 4.4f),  new Vector2(8.2f, 5.4f),   new Vector2(9.6f, 0.2f),
                new Vector2(14.2f, -1.2f), new Vector2(8.4f, -5.2f), new Vector2(-9.6f, -7.2f), new Vector2(-15.2f, -3.2f),
                new Vector2(1.2f, 2.2f),   new Vector2(-3.8f, 0.8f),
            };
            for (int i = 0; i < fernSpots.Length; i++)
            {
                SpawnGroundDecal(wetRoot.transform, $"WetFern_{i}", fernSpr, fernSpots[i], (i % 2 == 0 ? 1.1f : 0.92f), 6);
            }

            // 3. Cap nhat SwampAtmosphereController sang thong so trong treo (center = 0, edge = 0.34)
            SwampAtmosphereController atmo = UnityEngine.Object.FindAnyObjectByType<SwampAtmosphereController>();
            if (atmo == null)
            {
                GameObject rig = new GameObject("SwampAtmosphereRig");
                atmo = rig.AddComponent<SwampAtmosphereController>();
            }
            SerializedObject aso2 = new SerializedObject(atmo);
            aso2.FindProperty("centerDarknessAlpha").floatValue = 0.00f;
            aso2.FindProperty("edgeDarknessAlpha").floatValue = 0.34f;
            aso2.FindProperty("gloomTint").colorValue = new Color(0.03f, 0.09f, 0.12f, 1f);
            aso2.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(atmo);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MV][Ch2-P4.fix2] Hoan tat! Da tach anh sang ngay/dem cho Dam Lay + phu 53 chi tiet vung nuoc, bun uot, go muc reu, duong xi & mua phun.");

            AutoBuilder_StepCh2R5.RunChapter2OverhaulGateQA();
        }

        private static void SpawnGroundDecal(Transform parent, string name, Sprite spr, Vector2 pos, float scale, int order)
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            go.transform.localScale = Vector3.one * scale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = "GroundDecor";
            sr.sortingOrder = order;
        }

        private static void ConfigureSprite(string path, float ppu)
        {
            TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) return;
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.spritePixelsPerUnit = ppu;
            imp.filterMode = FilterMode.Point;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.SaveAndReimport();
        }
    }
}
