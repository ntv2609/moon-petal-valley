using UnityEngine;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Narrative
{
    public class StoryPropInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string propId;
        [SerializeField] private string requiredFlag;
        [SerializeField] private string completedFlag;
        [SerializeField] private string secondaryCompletedFlag;
        [SerializeField] private string requiredItemId;
        [SerializeField] private int requiredItemAmount;
        [SerializeField] private string rewardItemId;
        [SerializeField] private int rewardItemAmount;
        [SerializeField] private int staminaCost;
        [SerializeField] private string dialogueSpeaker;
        [SerializeField] private string dialogueBody;
        [SerializeField] private string blockedHint;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 1.9f;

        private string _cachedLabel;
        private SpriteRenderer _sr;

        public bool CanInteract
        {
            get
            {
                if (!gameObject.activeInHierarchy) return false;
                bool alreadyDone = !string.IsNullOrEmpty(completedFlag) && GameRoot.Instance != null &&
                                   GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag(completedFlag);
                return !alreadyDone;
            }
        }

        public float InteractRadius => interactRadius;
        public Vector2 InteractionWorldPosition => transform.position;
        public Vector3 PointerAnchorPosition
        {
            get
            {
                if (_sr == null) _sr = GetComponentInChildren<SpriteRenderer>();
                float topY = _sr != null && _sr.sprite != null ? _sr.bounds.max.y + 0.18f : transform.position.y + 0.85f;
                return new Vector3(transform.position.x, topY, 0f);
            }
        }

        public string InteractionPromptLabel
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachedLabel)) return _cachedLabel;
                _cachedLabel = ResolveLabel();
                return _cachedLabel;
            }
        }

        private void OnEnable() => InteractionManager.Register(this);
        private void OnDisable() => InteractionManager.Unregister(this);
        private void Start() => HideLegacyPrompt();

        public void HideLegacyPrompt()
        {
            if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
        }

        private string ResolveLabel()
        {
            if (promptObject != null)
            {
                var tmp = promptObject.GetComponentInChildren<TMP_Text>(true);
                if (tmp != null && !string.IsNullOrWhiteSpace(tmp.text))
                {
                    string raw = tmp.text.Trim();
                    if (raw.StartsWith("[E]")) raw = raw.Substring(3).Trim();
                    if (!string.IsNullOrEmpty(raw)) return raw;
                }
            }
            if (propId == "prop_seed_box") return "Mở Hộp Gỗ Của Bà";
            if (!string.IsNullOrEmpty(dialogueSpeaker)) return "Tương tác: " + dialogueSpeaker;
            return "Tương tác";
        }

        public void Interact() => TryInteract();

        private void TryInteract()
        {
            if (GameRoot.Instance == null) return;
            var flags = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            if (!string.IsNullOrEmpty(requiredFlag) && !flags.HasFlag(requiredFlag))
            {
                GameEventBus.RaiseToastRequested(string.IsNullOrEmpty(blockedHint) ? "Chưa đủ điều kiện để tương tác." : blockedHint);
                return;
            }

            if (!string.IsNullOrEmpty(requiredItemId) && requiredItemAmount > 0)
            {
                if (inv == null || !inv.HasItem(requiredItemId, requiredItemAmount))
                {
                    int have = inv != null ? inv.GetItemCount(requiredItemId) : 0;
                    GameEventBus.RaiseToastRequested(blockedHint + " (Hiện có " + have + "/" + requiredItemAmount + ")");
                    return;
                }
            }

            if (staminaCost > 0) GameRoot.Instance.Stats?.ConsumeStamina(staminaCost);
            if (!string.IsNullOrEmpty(requiredItemId) && requiredItemAmount > 0) inv?.RemoveItem(requiredItemId, requiredItemAmount);

            if (propId == "prop_seed_box" || completedFlag == "flag_lumi_awakened")
            {
                var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
                if (lumi != null) lumi.PlayAwakenFromBoxAnimation(transform.position);
            }

            var lines = BuildRichPropLines();
            GameRoot.Instance.Dialogue?.StartQuickLines(lines, () =>
            {
                if (!string.IsNullOrEmpty(rewardItemId) && rewardItemAmount > 0)
                    inv?.AddItemById(rewardItemId, rewardItemAmount, true);
                if (propId == "prop_seed_box")
                    inv?.AddItemById("item_dew_barley_seed", 6, true);
                if (!string.IsNullOrEmpty(completedFlag)) flags.SetFlag(completedFlag, true);
                if (!string.IsNullOrEmpty(secondaryCompletedFlag)) flags.SetFlag(secondaryCompletedFlag, true);
            });
        }

        private (string speaker, string body)[] BuildRichPropLines()
        {
            if (propId == "prop_letter_table")
            {
                return new[]
                {
                    ("Thư Của Bà", "Wen của bà, nếu cháu đọc được dòng này thì ổ khóa cũ vẫn chưa làm khó được cháu."),
                    ("Thư Của Bà", "Bà có gửi một người bạn nhỏ ngủ tạm trong Hộp Gỗ bên cạnh. Đừng để nó thức dậy giữa đống cỏ đá kẻo nó dỗi đấy."),
                    ("Wen", "Người bạn nhỏ? Bà lại bày trò rồi... Nhưng nhìn quanh thì đúng là phải dọn 3 cụm cỏ đá này trước thật.")
                };
            }
            if (propId == "prop_seed_box")
            {
                return new[]
                {
                    ("Tinh Linh Lumi", "Ưm... nắng lên rồi à bà ơi? Hôm nay tưới hoa... Ơ? Khoan!"),
                    ("Tinh Linh Lumi", "Khịt khịt... Mùi đất quen lắm, nhưng sao bà lại... trẻ ra thế này?!"),
                    ("Wen", "À, mình không phải bà. Mình là Wen, cháu gái của bà."),
                    ("Tinh Linh Lumi", "...Cháu gái? Thế... bà đâu rồi?"),
                    ("Wen", "Bà đi xa rồi, Lumi ạ. Trước khi đi, bà dặn mình về chăm lại Nhà Kính."),
                    ("Tinh Linh Lumi", "Hừm... Thế thì Wen phải giữ lời đấy nhé. Cầm lấy Viên Hạt Giống Ánh Trăng trong hộp này đi."),
                    ("Tinh Linh Lumi", "Còn 6 Hạt Lúa Mạch Sương Mai ở đáy hộp nữa. Lumi không tặng đâu nhé, là bà cất phần đấy!"),
                    ("Wen", "Ừ, mình giữ kỹ đây. Giờ mình thử gieo Viên Hạt Ánh Trăng vào Bồn Ươm Nhỏ bên kia xem sao?")
                };
            }
            if (propId == "prop_test_planter")
            {
                return new[]
                {
                    ("Wen", "Ủa? Vừa chạm đất đã tắt sáng rồi... Mình làm sai chỗ nào à?"),
                    ("Tinh Linh Lumi", "Khịt khịt... Không phải đất hỏng đâu. Là vì trong đây chỉ có mỗi hơi ấm của Wen thôi."),
                    ("Tinh Linh Lumi", "Hoa Moonpetal ích kỷ lắm, trồng một mình nó không chịu nở đâu. Lúa Mạch Sương Mai thì trồng bình thường được."),
                    ("Wen", "Trồng một mình không nở... Vậy chắc phải ra Làng Chính hỏi xem có ai còn nhớ cách trồng cùng bà không."),
                    ("Tinh Linh Lumi", "Đi thì đi! Nhưng ra ngoài trời nhiều mùi ẩm mốc lắm đấy, Wen đi sát Lumi vào!")
                };
            }
            if (propId == "prop_memory_bench")
            {
                return new[]
                {
                    ("Wen", "Phù... lớp rêu dày thật. Ủa, trên tay vịn gỗ có khắc hai chữ cái mờ mờ..."),
                    ("Tinh Linh Lumi", "Khịt khịt! Mùi trà hoa cúc cũ! Ngày xưa bà hay ngồi đây uống trà với ai ấy nhỉ?"),
                    ("Ông Bach", "Lau xong rồi thì thôi, đừng có soi mói chuyện người già!"),
                    ("Wen", "Cháu biết rồi ạ. Xem ra muốn ông mở lòng thì phải tìm lại những người hàng xóm cũ trước đã.")
                };
            }
            if (propId == "prop_otis_mailbox")
            {
                return new[]
                {
                    ("Bác Otis", "E hèm... Khớp mộng gỗ chắc chắn như mới rồi! Cảm ơn cháu Wen."),
                    ("Bác Otis", "Đây là Túi Hạt Di Sản Số 3, còn đây là lá thư bà cháu viết cho Ông Bach. Cháu mang tới vườn giúp bác nhé."),
                    ("Wen", "Cháu hiểu rồi. Chắc Ông Bach đã chờ lá thư này lâu lắm.")
                };
            }
            return new[] { (dialogueSpeaker, dialogueBody) };
        }
    }
}
