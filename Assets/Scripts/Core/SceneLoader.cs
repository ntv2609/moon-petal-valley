using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoonpetalValley.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public string ActiveWorldScene => SceneManager.GetActiveScene().name;

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[MV] SceneLoader: tên scene rỗng.");
                return;
            }
            if (SceneManager.GetActiveScene().name == sceneName) return;

            Debug.Log("[MV] SceneLoader: Đang nạp scene " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }
}
