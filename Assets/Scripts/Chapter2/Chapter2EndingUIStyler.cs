using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MoonpetalValley.Chapter2
{
    /// <summary>
    /// Dong bo hoa art giao dien Ch2CompletionCanvas (duoc Chapter2EndingController tao runtime)
    /// theo phong cach khung go/giay co 9-slice + font VN cua Chuong 1.
    /// </summary>
    public class Chapter2EndingUIStyler : MonoBehaviour
    {
        [Header("UI Skin (Step 47A - Cozy Wood Style)")]
        [SerializeField] private Sprite panelDarkWoodSprite;
        [SerializeField] private Sprite buttonWoodSprite;
        [SerializeField] private Sprite petalIconSprite;
        [SerializeField] private TMP_FontAsset menuWarmFont;
        [SerializeField] private TMP_FontAsset storyFont;
        [SerializeField] private TMP_FontAsset hudCleanFont;

        private GameObject _styledCanvas;

        private void LateUpdate()
        {
            if (Chapter2EndingController.Instance == null || !Chapter2EndingController.Instance.IsPlaying)
            {
                _styledCanvas = null;
                return;
            }

            GameObject canvasGo = GameObject.Find("Ch2CompletionCanvas");
            if (canvasGo == null || canvasGo == _styledCanvas) return;

            ApplyCozySkin(canvasGo);
            _styledCanvas = canvasGo;
        }

        private void ApplyCozySkin(GameObject canvasGo)
        {
            // 1. Tim modal panel (khong phai full-screen backdrop)
            RectTransform modalRt = null;
            Image[] images = canvasGo.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                Image img = images[i];
                if (img == null || img.GetComponent<Button>() != null) continue;
                RectTransform rt = img.rectTransform;
                bool isFullScreen = rt.anchorMin == Vector2.zero && rt.anchorMax == Vector2.one;
                if (!isFullScreen && rt.sizeDelta.x >= 400f && rt.sizeDelta.y >= 260f)
                {
                    modalRt = rt;
                    if (panelDarkWoodSprite != null)
                    {
                        img.sprite = panelDarkWoodSprite;
                        img.type = Image.Type.Sliced;
                        img.pixelsPerUnitMultiplier = 0.45f;
                        img.color = Color.white;
                    }
                    if (img.GetComponent<Shadow>() == null)
                    {
                        Shadow sh = img.gameObject.AddComponent<Shadow>();
                        sh.effectColor = new Color(0f, 0f, 0f, 0.65f);
                        sh.effectDistance = new Vector2(0f, -6f);
                    }
                    break;
                }
            }

            // 2. Gan bieu tuong Canh Hoa Gan Ket phia tren cung modal
            if (modalRt != null && petalIconSprite != null && modalRt.Find("PetalHeaderEmblem") == null)
            {
                GameObject emblemGo = new GameObject("PetalHeaderEmblem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                emblemGo.transform.SetParent(modalRt, false);
                RectTransform ert = emblemGo.GetComponent<RectTransform>();
                ert.anchorMin = new Vector2(0.5f, 1f);
                ert.anchorMax = new Vector2(0.5f, 1f);
                ert.pivot = new Vector2(0.5f, 0.5f);
                ert.anchoredPosition = new Vector2(0f, 10f);
                ert.sizeDelta = new Vector2(56f, 56f);
                Image eImg = emblemGo.GetComponent<Image>();
                eImg.sprite = petalIconSprite;
                eImg.preserveAspect = true;
                eImg.raycastTarget = false;
            }

            // 3. Nang cap tat ca Button thanh ui_button_wood 9-slice
            Button[] buttons = canvasGo.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                Image bImg = buttons[i].GetComponent<Image>();
                if (bImg != null && buttonWoodSprite != null)
                {
                    bImg.sprite = buttonWoodSprite;
                    bImg.type = Image.Type.Sliced;
                    bImg.pixelsPerUnitMultiplier = 0.50f;
                    bImg.color = new Color(1f, 0.96f, 0.88f, 1f);
                }
                TMP_Text btnLbl = buttons[i].GetComponentInChildren<TMP_Text>(true);
                if (btnLbl != null)
                {
                    if (hudCleanFont != null) btnLbl.font = hudCleanFont;
                    btnLbl.color = new Color32(255, 246, 222, 255);
                    btnLbl.fontStyle = FontStyles.Bold;
                }
            }

            // 4. Dong bo font VN + mau chu am ap cho tieu de va noi dung
            TMP_Text[] texts = canvasGo.GetComponentsInChildren<TMP_Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                TMP_Text t = texts[i];
                if (t == null || t.GetComponentInParent<Button>() != null) continue;
                if (t.fontSize >= 24f)
                {
                    if (menuWarmFont != null) t.font = menuWarmFont;
                    t.color = new Color32(252, 224, 146, 255);
                }
                else
                {
                    if (storyFont != null) t.font = storyFont;
                    t.color = new Color32(248, 240, 220, 255);
                }
            }
        }
    }
}
