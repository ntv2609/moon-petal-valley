using UnityEngine;
using TMPro;
using MoonpetalValley.UI;
using MoonpetalValley.Player;

namespace MoonpetalValley.Core
{
    public class JuiceEffectsManager : MonoBehaviour
    {
        public static JuiceEffectsManager Instance { get; private set; }

        [SerializeField] private TMP_FontAsset vnFont;

        private void Awake()
        {
            Instance = this;
        }

        public void SpawnFloatingNumber(Vector3 worldPos, string text, Color color)
        {
            GameObject go = new GameObject("FloatingNumber");
            go.transform.position = worldPos;
            var comp = go.AddComponent<FloatingDamageText>();
            comp.Initialize(text, color, vnFont);
        }

        public void TriggerCameraShake(float intensity = 0.09f, float duration = 0.12f)
        {
            var cam = UnityEngine.Object.FindAnyObjectByType<CameraFollow2D>();
            if (cam != null)
            {
                cam.TriggerShake(intensity, duration);
            }
        }
    }
}
