using UnityEngine;

namespace MoonpetalValley.Core
{
    public class BootstrapLoader : MonoBehaviour
    {
        [SerializeField] private string targetSceneName = "MoonpetalValley_MVP";

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Scenes != null)
            {
                GameRoot.Instance.Scenes.LoadScene(targetSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}
