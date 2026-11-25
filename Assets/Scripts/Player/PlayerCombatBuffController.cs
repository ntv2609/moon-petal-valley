using UnityEngine;
using UnityEngine.InputSystem;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Inventory;

namespace MoonpetalValley.Player {
    public class PlayerCombatBuffController : MonoBehaviour {
        public static PlayerCombatBuffController Instance { get; private set; }

        public const string ItemDewTeaId = "item_dew_tea";
        public const string ItemMoonElixirId = "item_moon_elixir";
        public const string ItemNightfireCakeId = "item_nightfire_cake";
        public const string ItemSunshineBreadId = "item_sunshine_bread";

        private float _nightfireTimer;
        private float _feverDamageBonus;
        private float _feverCooldownMult = 1f;
        private float _forgeDamageBonus;
        private float _forgeSpeedBonus;
        private float _quickUseCooldown;

        public float NightfireRemaining => _nightfireTimer;
        public bool IsNightfireActive => _nightfireTimer > 0f;
        public float DamageMultiplier => 1f + (_nightfireTimer > 0f ? 0.40f : 0f) + _feverDamageBonus + _forgeDamageBonus;
        public float MoveSpeedMultiplier => 1f + (_nightfireTimer > 0f ? 0.25f : 0f) + _forgeSpeedBonus;
        public float CooldownMultiplier => Mathf.Clamp(_feverCooldownMult, 0.55f, 1.0f);

        private void Awake() {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        public void SetFeverModifiers(float damageBonus, float cooldownMult) {
            _feverDamageBonus = Mathf.Max(0f, damageBonus);
            _feverCooldownMult = Mathf.Clamp(cooldownMult, 0.55f, 1.0f);
        }

        public void SetForgeModifiers(float damageBonus, float speedBonus) {
            _forgeDamageBonus = Mathf.Max(0f, damageBonus);
            _forgeSpeedBonus = Mathf.Max(0f, speedBonus);
        }

        private void Update() {
            float dt = Time.deltaTime;
            if (_nightfireTimer > 0f) {
                _nightfireTimer = Mathf.Max(0f, _nightfireTimer - dt);
                if (_nightfireTimer <= 0f)
                    GameEventBus.RaiseToastRequested("Hiệu ứng Bánh Lửa Đêm đã kết thúc.");
            }
            if (_quickUseCooldown > 0f) _quickUseCooldown -= dt;

            if (GameRoot.Instance == null || GameRoot.Instance.State == null || !GameRoot.Instance.State.IsGameplayActive)
                return;

            if (_quickUseCooldown <= 0f) {
                if (IsKeyPressedThisFrame(Key.F, KeyCode.F)) TryQuickHeal();
                else if (IsKeyPressedThisFrame(Key.C, KeyCode.C)) TryQuickBuff();
            }
        }

        public bool TryQuickHeal() {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null || GameRoot.Instance.Stats == null)
                return false;

            var inv = GameRoot.Instance.Inventory;
            var stats = GameRoot.Instance.Stats;
            if (stats.CurrentLightHP >= stats.MaxLightHP && stats.CurrentStamina >= stats.EffectiveMaxStamina) {
                GameEventBus.RaiseToastRequested("Máu Ánh Sáng và Thể Lực đã đầy!");
                _quickUseCooldown = 0.4f;
                return false;
            }

            float hpRatio = (float)stats.CurrentLightHP / Mathf.Max(1, stats.MaxLightHP);
            string chosenId = null;
            if (hpRatio < 0.45f && CountItem(inv, ItemMoonElixirId) > 0) chosenId = ItemMoonElixirId;
            else if (CountItem(inv, ItemDewTeaId) > 0) chosenId = ItemDewTeaId;
            else if (CountItem(inv, ItemMoonElixirId) > 0) chosenId = ItemMoonElixirId;
            else if (CountItem(inv, ItemSunshineBreadId) > 0) chosenId = ItemSunshineBreadId;

            if (string.IsNullOrEmpty(chosenId)) {
                GameEventBus.RaiseToastRequested("Hết vật phẩm hồi máu! Hãy nấu Trà Sương hoặc Tinh Chất Ánh Trăng tại Vạc Vườn Ươm.");
                _quickUseCooldown = 0.5f;
                return false;
            }

            return ConsumeItemById(chosenId);
        }

        public bool TryQuickBuff() {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return false;
            var inv = GameRoot.Instance.Inventory;
            if (CountItem(inv, ItemNightfireCakeId) <= 0) {
                GameEventBus.RaiseToastRequested("Chưa có Bánh Lửa Đêm! Trồng Ớt Đom Đóm và nấu tại Vạc Vườn Ươm.");
                _quickUseCooldown = 0.5f;
                return false;
            }
            return ConsumeItemById(ItemNightfireCakeId);
        }

        public bool ConsumeItemById(string itemId) {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null || GameRoot.Instance.Stats == null)
                return false;

            var inv = GameRoot.Instance.Inventory;
            ItemData data = inv.GetItemDataById(itemId);
            if (data == null || CountItem(inv, itemId) <= 0) return false;

            inv.RemoveItem(itemId, 1);
            _quickUseCooldown = 0.55f;

            var stats = GameRoot.Instance.Stats;
            var player = Object.FindAnyObjectByType<PlayerController2D>();
            Vector3 popPos = player != null ? player.transform.position + Vector3.up * 0.9f : Vector3.zero;

            if (itemId == ItemMoonElixirId) {
                stats.RestoreStats(20, 85, true);
                if (player != null) player.GrantInvulnerability(2.0f);
                if (JuiceEffectsManager.Instance != null)
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(popPos, "+85 Máu & Bất Tử!", new Color(0.55f, 0.94f, 1f));
                GameEventBus.RaiseToastRequested("Đã uống Tinh Chất Ánh Trăng: +85 Máu, +20 Thể Lực & 2s Bất Tử!");
            }
            else if (itemId == ItemDewTeaId) {
                stats.RestoreStats(15, 45, true);
                if (JuiceEffectsManager.Instance != null)
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(popPos, "+45 Máu!", new Color(0.55f, 1f, 0.72f));
                GameEventBus.RaiseToastRequested("Đã uống Trà Sương Ấm: +45 Máu & +15 Thể Lực!");
            }
            else if (itemId == ItemNightfireCakeId) {
                _nightfireTimer = 45f;
                stats.RestoreStats(20, 10, false);
                if (JuiceEffectsManager.Instance != null)
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(popPos, "Bánh Lửa Đêm! +40% DMG", new Color(1f, 0.65f, 0.28f));
                GameEventBus.RaiseToastRequested("Bánh Lửa Đêm kích hoạt 45s: +40% Sát Thương & +25% Tốc Độ!");
            }
            else {
                int stam = Mathf.Max(0, data.staminaRestore);
                int hp = Mathf.Max(0, data.lightHPRestore);
                stats.RestoreStats(stam, hp, data.clearsExhaustionPenalty);
                if (JuiceEffectsManager.Instance != null)
                    JuiceEffectsManager.Instance.SpawnFloatingNumber(popPos, hp > 0 ? ("+" + hp + " Máu") : ("+" + stam + " Thể Lực"), new Color(1f, 0.92f, 0.55f));
            }

            if (GameRoot.Instance.Audio != null) GameRoot.Instance.Audio.PlayUIClick();
            return true;
        }

        public static int CountItem(InventoryManager inv, string itemId) {
            if (inv == null || string.IsNullOrEmpty(itemId)) return 0;
            int total = 0;
            for (int i = 0; i < InventoryManager.TotalSlots; i++) {
                var sl = inv.GetSlot(i);
                if (sl != null && !sl.IsEmpty && sl.item.itemID == itemId)
                    total += sl.amount;
            }
            return total;
        }

        private static bool IsKeyPressedThisFrame(Key newKey, KeyCode legacyKey) {
            if (Keyboard.current != null && Keyboard.current[newKey].wasPressedThisFrame)
                return true;
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(legacyKey);
#else
            return false;
#endif
        }
    }
}
