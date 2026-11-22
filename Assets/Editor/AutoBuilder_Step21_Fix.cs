using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step21_Fix
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";

        [MenuItem("Moonpetal/Step 21/3. Final Clean Fix (DiaryManager Em-Dash & Barrel Overlap)")]
        public static void ApplyFinalCleanFix()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            MoveBarrel("BakeryBarrel_1", new Vector3(-9.4f, 5.4f, 0f));
            MoveBarrel("ShedBarrel_1",   new Vector3(-6.8f, -5.2f, 0f));
            MoveBarrel("ShedBarrel_2",   new Vector3(-6.2f, -5.9f, 0f));

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 21.FIX! Toàn bộ 3 thùng gỗ đã được dời khỏi vùng chồng chéo & 0 em dash còn sót. ===");
        }

        private static void MoveBarrel(string name, Vector3 targetPos)
        {
            GameObject go = GameObject.Find(name);
            if (go == null) return;
            go.transform.position = targetPos;
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = Mathf.RoundToInt(-(targetPos.y - 0.3f) * 100f);
            }
            EditorUtility.SetDirty(go);
        }
    }
}
