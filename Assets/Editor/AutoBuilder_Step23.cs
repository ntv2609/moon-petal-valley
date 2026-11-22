using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step23
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";

        [MenuItem("Moonpetal/Step 23/1. Add Living Animated Main Menu Backdrop FX")]
        public static void BuildStep23()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            if (!File.Exists(MainMenuScenePath))
            {
                Debug.LogError("[MV] Không tìm thấy scene: " + MainMenuScenePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            GameObject canvasGo = GameObject.Find("MainMenuCanvas");
            if (canvasGo == null)
            {
                Debug.LogError("[MV] Không tìm thấy MainMenuCanvas trong MainMenu.unity.");
                return;
            }

            // Disable old world-space MenuPetalSpawner if present so our clean Canvas FX handles all petals smoothly
            GameObject oldSpawner = GameObject.Find("MenuPetalSpawner");
            if (oldSpawner != null) oldSpawner.SetActive(false);

            var living = canvasGo.GetComponent<MainMenuLivingBackdrop>();
            if (living == null) living = canvasGo.AddComponent<MainMenuLivingBackdrop>();

            var so = new SerializedObject(living);
            Transform bg = canvasGo.transform.Find("Backdrop");
            Transform logo = canvasGo.transform.Find("MainMenuLogo");
            if (bg != null) so.FindProperty("backdropRect").objectReferenceValue = bg.GetComponent<RectTransform>();
            if (logo != null) so.FindProperty("logoRect").objectReferenceValue = logo.GetComponent<RectTransform>();
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(canvasGo);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] === HOÀN TẤT STEP 23! Main Menu giờ đã có nền sống chuyển động (hoa bay, đom đóm, khói bếp, trăng sao & parallax). ===");
        }
    }
}
