using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using MoonpetalValley.Data;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_StepCh2X2
    {
        private const string ItemsArtDir = "Assets/Art/Sprites/Items";

        private static readonly string[] ExpectedIcons =
        {
            "icon_item_hoe.png",
            "icon_item_watering_can.png",
            "icon_item_hatchet.png",
            "icon_item_moon_sickle.png",
            "icon_item_moon_sickle_lv2.png",
            "icon_item_watering_can_lv2.png",
            "icon_item_nightwind_boots.png",
            "icon_item_dew_barley_seed.png",
            "icon_item_dew_barley.png",
            "icon_item_moonpetal_seed_core.png",
            "icon_item_seed_bag_1.png",
            "icon_item_seed_bag_2.png",
            "icon_item_seed_bag_3.png",
            "icon_item_river_daisy.png",
            "icon_item_moonbud_seed.png",
            "icon_item_moonbud_crop.png",
            "icon_item_firefly_chili_seed.png",
            "icon_item_firefly_chili.png",
            "icon_item_sunshine_bread.png",
            "icon_item_dew_tea.png",
            "icon_item_moon_elixir.png",
            "icon_item_nightfire_cake.png",
            "icon_item_star_fertilizer.png",
            "icon_item_branch_wood.png",
            "icon_item_dew_sap.png",
            "icon_item_starlight_dust.png",
            "icon_item_gloom_shard.png",
            "icon_item_carpenter_hammer.png",
            "icon_item_iron_nails_box.png",
            "icon_item_storm_lantern_east.png",
            "icon_item_storm_lantern_west.png",
            "icon_item_petal_of_hope.png",
            "icon_item_petal_of_bond.png",
            "icon_item_ch3_lantern_charm.png",
            "icon_item_ch3_paper_fish.png",
            "icon_item_ch3_river_tea.png",
            "icon_item_ch3_seed_cake.png",
            "icon_item_ch3_wish_card.png"
        };

        [MenuItem("Moonpetal/Step Ch2-X2/1. Apply 38 HD 32x32 Item Sprites & Sync ItemData", false, 535)]
        public static void ApplyStepCh2X2()
        {
            int configured = 0;
            for (int i = 0; i < ExpectedIcons.Length; i++)
            {
                string path = ItemsArtDir + "/" + ExpectedIcons[i];
                if (!File.Exists(path)) continue;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;
                ti.textureType = TextureImporterType.Sprite;
                ti.spriteImportMode = SpriteImportMode.Single;
                ti.spritePixelsPerUnit = 32f;
                ti.filterMode = FilterMode.Point;
                ti.textureCompression = TextureImporterCompression.Uncompressed;
                ti.alphaIsTransparency = true;
                ti.mipmapEnabled = false;
                var settings = new TextureImporterSettings();
                ti.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteAlignment = (int)SpriteAlignment.Center;
                settings.spritePivot = new Vector2(0.5f, 0.5f);
                ti.SetTextureSettings(settings);
                ti.SaveAndReimport();
                configured++;
            }

            AssetDatabase.Refresh();

            // Dong bo lai icon cho tat ca ItemData co san trong du an
            int syncedItems = 0;
            string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/Data" });
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
                if (item == null || string.IsNullOrEmpty(item.itemID)) continue;

                string expectedSpritePath = ItemsArtDir + "/icon_" + item.itemID + ".png";
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(expectedSpritePath);
                if (spr != null && item.icon != spr)
                {
                    item.icon = spr;
                    EditorUtility.SetDirty(item);
                    syncedItems++;
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[MV][Ch2-X2] HOAN TAT: Da cau hinh " + configured + "/38 HD 32x32 Item Sprites & dong bo " + syncedItems + " ItemData!");
            ValidateStepCh2X2();
        }

        [MenuItem("Moonpetal/Step Ch2-X2/2. Validate Step Ch2-X2", false, 536)]
        public static void ValidateStepCh2X2()
        {
            int errors = 0;
            Debug.Log("[MV][Ch2-X2] === BAT DAU VALIDATE STEP CH2-X2 ===");

            for (int i = 0; i < ExpectedIcons.Length; i++)
            {
                string path = ItemsArtDir + "/" + ExpectedIcons[i];
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (spr == null)
                {
                    Debug.LogError("[MV][Ch2-X2][FAIL] Thieu hoac chua import Sprite: " + path);
                    errors++;
                }
            }

            string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/Data" });
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
                if (item != null && item.icon == null)
                {
                    Debug.LogError("[MV][Ch2-X2][FAIL] ItemData bi thieu icon: " + assetPath);
                    errors++;
                }
            }

            if (errors == 0)
                Debug.Log("[MV][Ch2-X2] === VALIDATE PASS 100% (38/38 HD Item Sprites, 0 LOI) ===");
            else
                Debug.LogError("[MV][Ch2-X2] === VALIDATE FAIL (" + errors + " LOI) ===");
        }
    }
}
