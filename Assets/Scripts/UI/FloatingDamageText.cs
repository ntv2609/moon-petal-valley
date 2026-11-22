using UnityEngine;
using TMPro;

namespace MoonpetalValley.UI
{
    public class FloatingDamageText : MonoBehaviour
    {
        private TMP_Text _tmp;
        private float _lifeTimer;
        private float _maxLife = 0.75f;
        private Vector3 _velocity;

        public void Initialize(string text, Color color, TMP_FontAsset font)
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 80;

            var rect = gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(140f, 50f);
            transform.localScale = new Vector3(0.018f, 0.018f, 1f);

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(transform, false);
            var tr = txtGo.AddComponent<RectTransform>();
            tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

            _tmp = txtGo.AddComponent<TextMeshProUGUI>();
            _tmp.text = text;
            _tmp.fontSize = 34;
            _tmp.fontStyle = FontStyles.Bold;
            _tmp.alignment = TextAlignmentOptions.Center;
            _tmp.color = color;
            if (font != null) _tmp.font = font;

            _lifeTimer = _maxLife;
            _velocity = new Vector3(Random.Range(-0.4f, 0.4f), 1.45f, 0f);
        }

        private void Update()
        {
            _lifeTimer -= Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;
            _velocity.y = Mathf.Lerp(_velocity.y, 0.25f, Time.deltaTime * 4f);

            if (_tmp != null)
            {
                Color c = _tmp.color;
                c.a = Mathf.Clamp01(_lifeTimer / (_maxLife * 0.6f));
                _tmp.color = c;
            }

            if (_lifeTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
