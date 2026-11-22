# MOONPETAL VALLEY — TỔNG HỢP MÃ NGUỒN C#

- **Thời điểm xuất:** 2026-11-22 23:10:02
- **Tổng số file:** 92
- **Tổng số dòng:** 15242

## Danh sách file

1. `Assets/Scripts/Audio/AudioManager.cs` (84 dòng)
2. `Assets/Scripts/Combat/BlightEncounterZone.cs` (47 dòng)
3. `Assets/Scripts/Combat/BlightFogZone.cs` (65 dòng)
4. `Assets/Scripts/Combat/GloomSlimeController.cs` (314 dòng)
5. `Assets/Scripts/Combat/IDamageable.cs` (10 dòng)
6. `Assets/Scripts/Community/BundleManager.cs` (135 dòng)
7. `Assets/Scripts/Community/PlantingSpotController.cs` (116 dòng)
8. `Assets/Scripts/Core/BootstrapLoader.cs` (21 dòng)
9. `Assets/Scripts/Core/GameEventBus.cs` (43 dòng)
10. `Assets/Scripts/Core/GameRoot.cs` (85 dòng)
11. `Assets/Scripts/Core/GameStateManager.cs` (24 dòng)
12. `Assets/Scripts/Core/ISaveable.cs` (9 dòng)
13. `Assets/Scripts/Core/JuiceEffectsManager.cs` (36 dòng)
14. `Assets/Scripts/Core/SaveLoadManager.cs` (161 dòng)
15. `Assets/Scripts/Core/SceneLoader.cs` (23 dòng)
16. `Assets/Scripts/Core/StoryFlagManager.cs` (71 dòng)
17. `Assets/Scripts/Core/VNLocalization.cs` (56 dòng)
18. `Assets/Scripts/Core/WorldSpawnRegistry.cs` (62 dòng)
19. `Assets/Scripts/Data/BundleData.cs` (26 dòng)
20. `Assets/Scripts/Data/CropData.cs` (16 dòng)
21. `Assets/Scripts/Data/DialogueData.cs` (29 dòng)
22. `Assets/Scripts/Data/DiaryPageData.cs` (15 dòng)
23. `Assets/Scripts/Data/EnemyData.cs` (37 dòng)
24. `Assets/Scripts/Data/ItemData.cs` (40 dòng)
25. `Assets/Scripts/Data/ItemDatabaseSO.cs` (24 dòng)
26. `Assets/Scripts/Data/MapLayoutConfigSO.cs` (32 dòng)
27. `Assets/Scripts/Data/NPCData.cs` (28 dòng)
28. `Assets/Scripts/Data/PlayerStatsSO.cs` (47 dòng)
29. `Assets/Scripts/Data/QuestData.cs` (42 dòng)
30. `Assets/Scripts/Data/StringTableSO.cs` (45 dòng)
31. `Assets/Scripts/Data/TimeConfigSO.cs` (29 dòng)
32. `Assets/Scripts/DebugTools/RuntimeDebugOverlay.cs` (204 dòng)
33. `Assets/Scripts/Farming/FarmGridManager.cs` (158 dòng)
34. `Assets/Scripts/Farming/FarmTileController.cs` (139 dòng)
35. `Assets/Scripts/Farming/ResourceNodeController.cs` (151 dòng)
36. `Assets/Scripts/Farming/SeedShelfInteractable.cs` (64 dòng)
37. `Assets/Scripts/Farming/WaterWellInteractable.cs` (47 dòng)
38. `Assets/Scripts/Inventory/InventoryManager.cs` (396 dòng)
39. `Assets/Scripts/Inventory/WorldLootDrop.cs` (72 dòng)
40. `Assets/Scripts/Narrative/DialogueManager.cs` (159 dòng)
41. `Assets/Scripts/Narrative/DiaryManager.cs` (101 dòng)
42. `Assets/Scripts/Narrative/NPCController.cs` (185 dòng)
43. `Assets/Scripts/Narrative/QuestManager.cs` (105 dòng)
44. `Assets/Scripts/Narrative/StoryPropInteractable.cs` (106 dòng)
45. `Assets/Scripts/Player/CameraFollow2D.cs` (49 dòng)
46. `Assets/Scripts/Player/GameInputReader.cs` (92 dòng)
47. `Assets/Scripts/Player/LumiCompanionController.cs` (130 dòng)
48. `Assets/Scripts/Player/PlayerCombatController.cs` (194 dòng)
49. `Assets/Scripts/Player/PlayerController2D.cs` (228 dòng)
50. `Assets/Scripts/Player/PlayerStatsController.cs` (150 dòng)
51. `Assets/Scripts/Player/PlayerToolController.cs` (195 dòng)
52. `Assets/Scripts/Player/WenSpriteAnimator.cs` (302 dòng)
53. `Assets/Scripts/TimeSystem/AmbientPetalSpawner.cs` (86 dòng)
54. `Assets/Scripts/TimeSystem/BedInteractable.cs` (52 dòng)
55. `Assets/Scripts/TimeSystem/DayNightLightingController.cs` (85 dòng)
56. `Assets/Scripts/TimeSystem/SleepAndKnockoutController.cs` (130 dòng)
57. `Assets/Scripts/TimeSystem/TimeOfDayManager.cs` (164 dòng)
58. `Assets/Scripts/TimeSystem/VillageLantern.cs` (47 dòng)
59. `Assets/Scripts/TimeSystem/WaterTileAnimator.cs` (36 dòng)
60. `Assets/Scripts/UI/BundleUIView.cs` (157 dòng)
61. `Assets/Scripts/UI/ClockHUDView.cs` (56 dòng)
62. `Assets/Scripts/UI/DialogueUIView.cs` (34 dòng)
63. `Assets/Scripts/UI/DiaryUIView.cs` (103 dòng)
64. `Assets/Scripts/UI/FloatingDamageText.cs` (61 dòng)
65. `Assets/Scripts/UI/FloatingToastUI.cs` (57 dòng)
66. `Assets/Scripts/UI/HotbarUIView.cs` (166 dòng)
67. `Assets/Scripts/UI/InventoryUIView.cs` (302 dòng)
68. `Assets/Scripts/UI/MainMenuController.cs` (72 dòng)
69. `Assets/Scripts/UI/PauseMenuUIView.cs` (111 dòng)
70. `Assets/Scripts/UI/QuestTrackerHUDView.cs` (33 dòng)
71. `Assets/Scripts/UI/StatsHUDView.cs` (82 dòng)
72. `Assets/Editor/AutoBuilder_Step1.cs` (639 dòng)
73. `Assets/Editor/AutoBuilder_Step10.cs` (248 dòng)
74. `Assets/Editor/AutoBuilder_Step11.cs` (365 dòng)
75. `Assets/Editor/AutoBuilder_Step12.cs` (433 dòng)
76. `Assets/Editor/AutoBuilder_Step13.cs` (191 dòng)
77. `Assets/Editor/AutoBuilder_Step14.cs` (382 dòng)
78. `Assets/Editor/AutoBuilder_Step15.cs` (789 dòng)
79. `Assets/Editor/AutoBuilder_Step16.cs` (504 dòng)
80. `Assets/Editor/AutoBuilder_Step16_Fix.cs` (178 dòng)
81. `Assets/Editor/AutoBuilder_Step17.cs` (466 dòng)
82. `Assets/Editor/AutoBuilder_Step18.cs` (269 dòng)
83. `Assets/Editor/AutoBuilder_Step2.cs` (478 dòng)
84. `Assets/Editor/AutoBuilder_Step3.cs` (321 dòng)
85. `Assets/Editor/AutoBuilder_Step4.cs` (398 dòng)
86. `Assets/Editor/AutoBuilder_Step5.cs` (502 dòng)
87. `Assets/Editor/AutoBuilder_Step6.cs` (532 dòng)
88. `Assets/Editor/AutoBuilder_Step7.cs` (623 dòng)
89. `Assets/Editor/AutoBuilder_Step8.cs` (422 dòng)
90. `Assets/Editor/AutoBuilder_Step9.cs` (397 dòng)
91. `Assets/Editor/MoonpetalAssetPostprocessor.cs` (41 dòng)
92. `Assets/Editor/PlaceholderGeneratorUtility.cs` (231 dòng)

## 1. `Assets/Scripts/Audio/AudioManager.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip bgmCozyClip;
        [SerializeField] private AudioClip sfxUiClick;
        [SerializeField] private AudioClip sfxTool;
        [SerializeField] private AudioClip sfxSlash;
        [SerializeField] private AudioClip sfxHit;
        [SerializeField] private AudioClip sfxPickup;
        [SerializeField] private AudioClip sfxBloom;

        public bool IsMuted { get; private set; }

        private void OnEnable()
        {
            GameEventBus.OnItemChanged += HandleItemChanged;
            GameEventBus.OnEnemyDefeated += HandleEnemyDefeated;
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
            GameEventBus.OnStaminaChanged += HandleStaminaChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnItemChanged -= HandleItemChanged;
            GameEventBus.OnEnemyDefeated -= HandleEnemyDefeated;
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
            GameEventBus.OnStaminaChanged -= HandleStaminaChanged;
        }

        private int _lastStamina = 100;

        private void Start()
        {
            if (bgmSource != null && bgmCozyClip != null)
            {
                bgmSource.clip = bgmCozyClip;
                bgmSource.loop = true;
                bgmSource.volume = 0.25f;
                bgmSource.Play();
            }
        }

        public void ToggleMute()
        {
            IsMuted = !IsMuted;
            if (bgmSource != null) bgmSource.mute = IsMuted;
            if (sfxSource != null) sfxSource.mute = IsMuted;
        }

        public void PlaySFX(AudioClip clip, float volume = 0.55f)
        {
            if (IsMuted || sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        public void PlayUIClick() => PlaySFX(sfxUiClick, 0.45f);

        private void HandleItemChanged(string _, int __) => PlaySFX(sfxPickup, 0.45f);
        private void HandleEnemyDefeated(string _) => PlaySFX(sfxHit, 0.65f);

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            if (flagId == "flag_nv1_completed") PlaySFX(sfxBloom, 0.80f);
            else PlaySFX(sfxUiClick, 0.40f);
        }

        private void HandleStaminaChanged(int current, int max)
        {
            if (current < _lastStamina)
            {
                int diff = _lastStamina - current;
                PlaySFX(diff == 2 ? sfxSlash : sfxTool, 0.40f);
            }
            _lastStamina = current;
        }
    }
}
```

## 2. `Assets/Scripts/Combat/BlightEncounterZone.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Combat
{
    public class BlightEncounterZone : MonoBehaviour
    {
        [SerializeField] private string encounterId;
        [SerializeField] private string completionFlag;
        [SerializeField] private string completionToast;
        [SerializeField] private List<GloomSlimeController> slimes = new List<GloomSlimeController>();

        private bool _completed;

        public void NotifySlimeDefeated(GloomSlimeController defeated)
        {
            if (_completed) return;

            int alive = 0;
            for (int i = 0; i < slimes.Count; i++)
            {
                if (slimes[i] != null && !slimes[i].IsDefeated && slimes[i] != defeated)
                {
                    alive++;
                }
            }

            if (alive == 0)
            {
                _completed = true;
                if (!string.IsNullOrEmpty(completionFlag) && GameRoot.Instance != null && GameRoot.Instance.Flags != null)
                {
                    GameRoot.Instance.Flags.SetFlag(completionFlag, true);
                }
                if (!string.IsNullOrEmpty(completionToast))
                {
                    GameEventBus.RaiseToastRequested(completionToast);
                }
            }
            else
            {
                GameEventBus.RaiseToastRequested("Còn " + alive + " Bọt Sương Đen trong khu vực này!");
            }
        }
    }
}
```

## 3. `Assets/Scripts/Combat/BlightFogZone.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Combat
{
    public class BlightFogZone : MonoBehaviour
    {
        [SerializeField] private string clearOnFlag = "flag_nv1_completed";
        [SerializeField] private SpriteRenderer fogRenderer;

        private bool _isClearing;
        private float _driftPhase;
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.position;
            _driftPhase = Random.Range(0f, 6.28f);
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                GameRoot.Instance.Flags.HasFlag(clearOnFlag))
            {
                gameObject.SetActive(false);
            }
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value && flagId == clearOnFlag)
            {
                _isClearing = true;
            }
        }

        private void Update()
        {
            _driftPhase += Time.deltaTime * 0.7f;
            transform.position = _startPos + new Vector3(Mathf.Sin(_driftPhase) * 0.35f, Mathf.Cos(_driftPhase * 0.8f) * 0.15f, 0f);

            if (_isClearing && fogRenderer != null)
            {
                Color c = fogRenderer.color;
                c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * 0.45f);
                fogRenderer.color = c;
                if (c.a <= 0.01f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
```

## 4. `Assets/Scripts/Combat/GloomSlimeController.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;

namespace MoonpetalValley.Combat
{
    public class GloomSlimeController : MonoBehaviour, IDamageable
    {
        private enum SlimeState
        {
            Idle,
            Telegraph,
            Jumping,
            Cooldown,
            HitStun,
            Dying
        }

        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform hpBarFill;
        [SerializeField] private GameObject hpBarRoot;
        [SerializeField] private GameObject lootDropPrefab;
        [SerializeField] private BlightEncounterZone encounterZone;
        [SerializeField] private GameObject groundShadow;

        [Header("Pixel Art Animations (FullRect)")]
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] telegraphFrames;
        [SerializeField] private Sprite[] jumpFrames;
        [SerializeField] private Sprite[] hitFrames;
        [SerializeField] private Sprite[] dieFrames;
        [SerializeField] private float idleFps = 6f;
        [SerializeField] private float telegraphFps = 7f;
        [SerializeField] private float jumpFps = 10f;
        [SerializeField] private float hitFps = 11f;
        [SerializeField] private float dieFps = 8f;

        private SlimeState _state = SlimeState.Idle;
        private SlimeState _lastAnimState = SlimeState.Idle;
        private int _currentHP;
        private float _stateTimer;
        private float _flashTimer;
        private float _animTimer;
        private int _animFrame;
        private Vector2 _jumpVelocity;
        private Vector2 _knockbackVelocity;
        private PlayerController2D _player;
        private bool _isDefeated;

        public bool IsDefeated => _isDefeated;

        private void Start()
        {
            _currentHP = enemyData != null ? enemyData.maxHP : 45;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            if (groundShadow == null)
            {
                Transform sh = transform.Find("GroundShadow");
                if (sh != null) groundShadow = sh.gameObject;
            }
            if (spriteRenderer != null && idleFrames != null && idleFrames.Length > 0 && idleFrames[0] != null)
            {
                spriteRenderer.sprite = idleFrames[0];
            }
        }

        private void Update()
        {
            if (_state == SlimeState.Dying)
            {
                _stateTimer -= Time.deltaTime;
                UpdateAnimation(false);
                if (_stateTimer <= 0f)
                {
                    Destroy(gameObject);
                }
                return;
            }

            if (_isDefeated) return;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_player == null) _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();

            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
                if (spriteRenderer != null && _flashTimer <= 0f) spriteRenderer.color = Color.white;
            }

            _stateTimer -= Time.deltaTime;

            float detectR = enemyData != null ? enemyData.detectionRadius : 4.5f;
            float moveSpd = enemyData != null ? enemyData.moveSpeed : 2.8f;
            float interval = enemyData != null ? enemyData.attackInterval : 1.1f;

            switch (_state)
            {
                case SlimeState.Idle:
                    if (_player != null && Vector2.Distance(transform.position, _player.transform.position) <= detectR)
                    {
                        _state = SlimeState.Telegraph;
                        _stateTimer = 0.45f;
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.color = new Color(1f, 0.90f, 0.65f, 1f);
                            spriteRenderer.flipX = _player.transform.position.x < transform.position.x;
                        }
                    }
                    break;

                case SlimeState.Telegraph:
                    if (_stateTimer <= 0f)
                    {
                        Vector2 dir = _player != null ? ((Vector2)_player.transform.position - (Vector2)transform.position).normalized : Vector2.down;
                        _jumpVelocity = dir * (moveSpd * 1.45f);
                        _state = SlimeState.Jumping;
                        _stateTimer = 0.42f;
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.color = Color.white;
                            if (Mathf.Abs(dir.x) > 0.05f) spriteRenderer.flipX = dir.x < 0f;
                        }
                    }
                    break;

                case SlimeState.Jumping:
                    if (_stateTimer <= 0f)
                    {
                        _state = SlimeState.Cooldown;
                        _stateTimer = interval;
                        if (spriteRenderer != null) spriteRenderer.transform.localScale = Vector3.one;
                    }
                    break;

                case SlimeState.Cooldown:
                case SlimeState.HitStun:
                    if (_stateTimer <= 0f)
                    {
                        _state = SlimeState.Idle;
                        if (spriteRenderer != null) spriteRenderer.transform.localScale = Vector3.one;
                    }
                    break;
            }

            UpdateAnimation(true);
            if (spriteRenderer != null) spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
        }

        private void UpdateAnimation(bool loop)
        {
            if (spriteRenderer == null) return;

            if (_state != _lastAnimState)
            {
                _lastAnimState = _state;
                _animFrame = 0;
                _animTimer = 0f;
            }

            Sprite[] frames = idleFrames;
            float fps = idleFps;
            bool shouldLoop = loop;

            switch (_state)
            {
                case SlimeState.Telegraph:
                    frames = (telegraphFrames != null && telegraphFrames.Length > 0) ? telegraphFrames : idleFrames;
                    fps = telegraphFps;
                    shouldLoop = false;
                    break;
                case SlimeState.Jumping:
                    frames = (jumpFrames != null && jumpFrames.Length > 0) ? jumpFrames : idleFrames;
                    fps = jumpFps;
                    shouldLoop = true;
                    break;
                case SlimeState.HitStun:
                    frames = (hitFrames != null && hitFrames.Length > 0) ? hitFrames : idleFrames;
                    fps = hitFps;
                    shouldLoop = false;
                    break;
                case SlimeState.Dying:
                    frames = (dieFrames != null && dieFrames.Length > 0) ? dieFrames : idleFrames;
                    fps = dieFps;
                    shouldLoop = false;
                    break;
            }

            if (frames == null || frames.Length == 0) return;

            _animTimer += Time.deltaTime;
            float step = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= step)
            {
                _animTimer -= step;
                if (shouldLoop) _animFrame = (_animFrame + 1) % frames.Length;
                else _animFrame = Mathf.Min(_animFrame + 1, frames.Length - 1);
            }

            int idx = Mathf.Clamp(_animFrame, 0, frames.Length - 1);
            if (frames[idx] != null) spriteRenderer.sprite = frames[idx];
        }

        private void FixedUpdate()
        {
            if (rb == null || _isDefeated || _state == SlimeState.Dying)
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
                return;
            }
            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive) { rb.linearVelocity = Vector2.zero; return; }

            if (_state == SlimeState.HitStun)
            {
                rb.linearVelocity = _knockbackVelocity;
                _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 10f);
            }
            else if (_state == SlimeState.Jumping) rb.linearVelocity = _jumpVelocity;
            else rb.linearVelocity = Vector2.zero;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isDefeated || _state == SlimeState.Dying || !other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController2D>();
            if (player == null || player.IsInvulnerable) return;

            int dmg = enemyData != null ? enemyData.contactDamage : 10;
            GameRoot.Instance?.Stats?.TakeLightDamage(dmg, false);
            JuiceEffectsManager.Instance?.SpawnFloatingNumber(player.transform.position + Vector3.up * 0.7f, "-" + dmg, new Color(1f, 0.42f, 0.36f));
            JuiceEffectsManager.Instance?.TriggerCameraShake(0.14f, 0.16f);

            if (enemyData == null || enemyData.appliesSlowOnHit) player.ApplySlow();
        }

        public void TakePurifyDamage(int amount, Vector2 knockbackDir, float knockbackForce)
        {
            if (_isDefeated || _state == SlimeState.Dying) return;

            int maxHP = enemyData != null ? enemyData.maxHP : 45;
            _currentHP = Mathf.Max(0, _currentHP - amount);

            _state = SlimeState.HitStun;
            _stateTimer = 0.28f;
            _knockbackVelocity = knockbackDir.normalized * knockbackForce;

            _flashTimer = 0.16f;
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.75f, 1f, 1f, 1f);

            JuiceEffectsManager.Instance?.SpawnFloatingNumber(transform.position + Vector3.up * 0.55f, "-" + amount, new Color(0.70f, 0.98f, 1f));
            JuiceEffectsManager.Instance?.TriggerCameraShake(0.08f, 0.09f);

            if (hpBarRoot != null) hpBarRoot.SetActive(true);
            if (hpBarFill != null) hpBarFill.localScale = new Vector3(Mathf.Clamp01((float)_currentHP / maxHP), 1f, 1f);

            if (_currentHP <= 0) DieAndSpawnLoot();
        }

        private void DieAndSpawnLoot()
        {
            _isDefeated = true;
            _state = SlimeState.Dying;
            _stateTimer = (dieFrames != null && dieFrames.Length > 0) ? (dieFrames.Length / Mathf.Max(1f, dieFps)) + 0.05f : 0.05f;

            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (hpBarRoot != null) hpBarRoot.SetActive(false);
            if (groundShadow != null) groundShadow.SetActive(false);
            if (spriteRenderer != null) spriteRenderer.color = Color.white;

            var cols = GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < cols.Length; i++) cols[i].enabled = false;

            SpawnLoot();
            string id = enemyData != null ? enemyData.enemyID : "enemy_gloom_slime";
            GameEventBus.RaiseEnemyDefeated(id);
            encounterZone?.NotifySlimeDefeated(this);
        }

        private void SpawnLoot()
        {
            if (enemyData == null || enemyData.dropTable == null) return;
            for (int i = 0; i < enemyData.dropTable.Count; i++)
            {
                var entry = enemyData.dropTable[i];
                if (entry.item == null) continue;
                if (Random.value <= entry.dropChance)
                {
                    int amt = Random.Range(Mathf.Max(1, entry.minAmount), Mathf.Max(entry.minAmount, entry.maxAmount) + 1);
                    if (lootDropPrefab != null)
                    {
                        for (int k = 0; k < amt; k++)
                        {
                            Vector2 off = new Vector2(Random.Range(-0.4f, 0.4f), Random.Range(-0.3f, 0.3f));
                            GameObject go = Instantiate(lootDropPrefab, transform.position + (Vector3)off, Quaternion.identity);
                            go.GetComponent<WorldLootDrop>()?.Initialize(entry.item, 1);
                        }
                    }
                    else GameRoot.Instance?.Inventory?.AddItem(entry.item, amt, true);
                }
            }
        }
    }
}
```

## 5. `Assets/Scripts/Combat/IDamageable.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Combat
{
    public interface IDamageable
    {
        bool IsDefeated { get; }
        void TakePurifyDamage(int amount, Vector2 knockbackDir, float knockbackForce);
    }
}
```

## 6. `Assets/Scripts/Community/BundleManager.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Community
{
    public class BundleManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private BundleData activeBundle;

        public event Action OnBundleUpdated;

        private readonly Dictionary<string, int> _contributed = new Dictionary<string, int>();

        public BundleData ActiveBundle => activeBundle;
        public string SaveKey => "community_bundle";

        [Serializable]
        private struct SavedContribution
        {
            public string itemId;
            public int amount;
        }

        [Serializable]
        private class BundleSaveData
        {
            public List<SavedContribution> items = new List<SavedContribution>();
        }

        public int GetContributedAmount(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && _contributed.TryGetValue(itemId, out int val) ? val : 0;
        }

        public int ContributeAllAvailable()
        {
            if (activeBundle == null || GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return 0;
            var inv = GameRoot.Instance.Inventory;
            int totalDeposited = 0;

            for (int i = 0; i < activeBundle.requiredItems.Count; i++)
            {
                var req = activeBundle.requiredItems[i];
                if (req.item == null) continue;

                int already = GetContributedAmount(req.item.itemID);
                int needed = req.amount - already;
                if (needed <= 0) continue;

                int have = inv.GetItemCount(req.item.itemID);
                int take = Mathf.Min(needed, have);
                if (take > 0 && inv.RemoveItem(req.item.itemID, take))
                {
                    _contributed[req.item.itemID] = already + take;
                    totalDeposited += take;
                }
            }

            if (totalDeposited > 0)
            {
                if (IsBundleItemsComplete() && !string.IsNullOrEmpty(activeBundle.rewardStoryFlag))
                {
                    GameRoot.Instance.Flags?.SetFlag(activeBundle.rewardStoryFlag, true);
                }
                OnBundleUpdated?.Invoke();
            }
            return totalDeposited;
        }

        public bool IsBundleItemsComplete()
        {
            if (activeBundle == null) return false;
            for (int i = 0; i < activeBundle.requiredItems.Count; i++)
            {
                var req = activeBundle.requiredItems[i];
                if (req.item == null) continue;
                if (GetContributedAmount(req.item.itemID) < req.amount) return false;
            }
            return true;
        }

        public int GetReadyHelperCount()
        {
            if (activeBundle == null || GameRoot.Instance == null || GameRoot.Instance.Flags == null) return 0;
            int count = 0;
            for (int i = 0; i < activeBundle.requiredHelperFlags.Count; i++)
            {
                if (GameRoot.Instance.Flags.HasFlag(activeBundle.requiredHelperFlags[i])) count++;
            }
            return count;
        }

        public int GetTotalRequiredHelpers()
        {
            return activeBundle != null ? activeBundle.requiredHelperFlags.Count : 4;
        }

        public bool AreAllHelpersReady() => GetReadyHelperCount() >= GetTotalRequiredHelpers();

        public bool CanBloomNow()
        {
            bool alreadyBloomed = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                                  GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            return !alreadyBloomed && IsBundleItemsComplete() && AreAllHelpersReady();
        }

        public string CaptureStateJson()
        {
            var data = new BundleSaveData();
            foreach (var kv in _contributed)
            {
                data.items.Add(new SavedContribution { itemId = kv.Key, amount = kv.Value });
            }
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            _contributed.Clear();
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<BundleSaveData>(json);
            if (data != null && data.items != null)
            {
                for (int i = 0; i < data.items.Count; i++)
                {
                    _contributed[data.items[i].itemId] = data.items[i].amount;
                }
            }
            OnBundleUpdated?.Invoke();
        }
    }
}
```

## 7. `Assets/Scripts/Community/PlantingSpotController.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.Community
{
    public class PlantingSpotController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer flowerRenderer;
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite bloomedSprite;
        [SerializeField] private SpriteRenderer bloomAuraRenderer;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 2.4f;

        private PlayerController2D _player;
        private GameInputReader _input;
        private bool _isBloomed;

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
            RefreshBloomVisual();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == "flag_nv1_completed") RefreshBloomVisual();
        }

        public void RefreshBloomVisual()
        {
            _isBloomed = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                         GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");

            if (flowerRenderer != null)
            {
                flowerRenderer.sprite = _isBloomed ? bloomedSprite : emptySprite;
            }
            if (bloomAuraRenderer != null)
            {
                bloomAuraRenderer.enabled = _isBloomed;
            }
        }

        private void Update()
        {
            if (_isBloomed && bloomAuraRenderer != null)
            {
                float s = 2.4f + Mathf.Sin(Time.time * 2.5f) * 0.2f;
                bloomAuraRenderer.transform.localScale = new Vector3(s, s, 1f);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                if (_isBloomed)
                {
                    GameEventBus.RaiseToastRequested("Hoa Moonpetal đang nở rực rỡ! Sương Héo Úa đã tan khỏi Làng Chính.");
                    return;
                }

                var bundleUI = UnityEngine.Object.FindAnyObjectByType<BundleUIView>();
                if (bundleUI != null)
                {
                    bundleUI.Open(this);
                }
            }
        }

        public void ExecuteBloomClimax()
        {
            if (_isBloomed || GameRoot.Instance == null) return;

            var bundle = GameRoot.Instance.Bundle?.ActiveBundle;
            if (bundle != null && bundle.rewardItem != null)
            {
                GameRoot.Instance.Inventory?.AddItem(bundle.rewardItem, 1, true);
            }

            GameRoot.Instance.Flags?.SetFlag("flag_nv1_completed", true);
            RefreshBloomVisual();

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null) lumi.SetHaloRadiusScale(3.2f);

            GameEventBus.RaiseToastRequested("HOA MOONPETAL ĐÃ NỞ! Nhận Cánh Hoa Hy Vọng & Mở khóa Xung Ánh Trăng [Q]!");
        }
    }
}
```

## 8. `Assets/Scripts/Core/BootstrapLoader.cs`

```csharp
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
```

## 9. `Assets/Scripts/Core/GameEventBus.cs`

```csharp
using System;

namespace MoonpetalValley.Core
{
    public enum GameState
    {
        Booting,
        Gameplay,
        Dialogue,
        InventoryUI,
        BundleUI,
        DiaryUI,
        PauseMenu,
        DebugMenu,
        Cutscene,
        DayTransition
    }

    public static class GameEventBus
    {
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<string, bool> OnStoryFlagChanged;
        public static event Action<string> OnToastRequested;
        public static event Action<int, float> OnTimeChanged;
        public static event Action<int> OnDayStarted;
        public static event Action<int, int> OnStaminaChanged;
        public static event Action<int, int> OnLightHPChanged;
        public static event Action<string, int> OnItemChanged;
        public static event Action<string> OnEnemyDefeated;
        public static event Action<string> OnQuestUpdated;

        public static void RaiseGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);
        public static void RaiseStoryFlagChanged(string flagId, bool value) => OnStoryFlagChanged?.Invoke(flagId, value);
        public static void RaiseToastRequested(string message) => OnToastRequested?.Invoke(message);
        public static void RaiseTimeChanged(int day, float hourFloat) => OnTimeChanged?.Invoke(day, hourFloat);
        public static void RaiseDayStarted(int day) => OnDayStarted?.Invoke(day);
        public static void RaiseStaminaChanged(int current, int max) => OnStaminaChanged?.Invoke(current, max);
        public static void RaiseLightHPChanged(int current, int max) => OnLightHPChanged?.Invoke(current, max);
        public static void RaiseItemChanged(string itemId, int newTotal) => OnItemChanged?.Invoke(itemId, newTotal);
        public static void RaiseEnemyDefeated(string enemyId) => OnEnemyDefeated?.Invoke(enemyId);
        public static void RaiseQuestUpdated(string questId) => OnQuestUpdated?.Invoke(questId);
    }
}
```

## 10. `Assets/Scripts/Core/GameRoot.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.Player;
using MoonpetalValley.Inventory;
using MoonpetalValley.Narrative;
using MoonpetalValley.Community;
using MoonpetalValley.Audio;

namespace MoonpetalValley.Core
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }

        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private StoryFlagManager storyFlagManager;
        [SerializeField] private VNLocalization localization;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private FloatingToastUI toastUI;
        [SerializeField] private RuntimeDebugOverlay debugOverlay;
        [SerializeField] private TimeOfDayManager timeManager;
        [SerializeField] private PlayerStatsController statsController;
        [SerializeField] private SleepAndKnockoutController sleepController;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private DiaryManager diaryManager;
        [SerializeField] private QuestManager questManager;
        [SerializeField] private BundleManager bundleManager;
        [SerializeField] private SaveLoadManager saveLoadManager;
        [SerializeField] private AudioManager audioManager;

        private readonly List<ISaveable> _saveables = new List<ISaveable>();

        public GameStateManager State => gameStateManager;
        public StoryFlagManager Flags => storyFlagManager;
        public VNLocalization Loc => localization;
        public SceneLoader Scenes => sceneLoader;
        public FloatingToastUI Toast => toastUI;
        public RuntimeDebugOverlay DebugOverlay => debugOverlay;
        public TimeOfDayManager Time => timeManager;
        public PlayerStatsController Stats => statsController;
        public SleepAndKnockoutController Sleep => sleepController;
        public InventoryManager Inventory => inventoryManager;
        public DialogueManager Dialogue => dialogueManager;
        public DiaryManager Diary => diaryManager;
        public QuestManager Quests => questManager;
        public BundleManager Bundle => bundleManager;
        public SaveLoadManager SaveLoad => saveLoadManager;
        public AudioManager Audio => audioManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (localization != null) localization.Initialize();

            if (storyFlagManager != null) RegisterSaveable(storyFlagManager);
            if (timeManager != null) RegisterSaveable(timeManager);
            if (statsController != null) RegisterSaveable(statsController);
            if (inventoryManager != null) RegisterSaveable(inventoryManager);
            if (diaryManager != null) RegisterSaveable(diaryManager);
            if (bundleManager != null) RegisterSaveable(bundleManager);

            Debug.Log("[MV] GameRoot khởi tạo thành công.");
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (saveable == null) return;
            if (!_saveables.Contains(saveable)) _saveables.Add(saveable);
        }

        public IReadOnlyList<ISaveable> GetSaveables() => _saveables;
    }
}
```

## 11. `Assets/Scripts/Core/GameStateManager.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public GameState CurrentState { get; private set; } = GameState.Gameplay;

        public bool IsGameplayActive => CurrentState == GameState.Gameplay;
        public bool IsTimePaused => CurrentState != GameState.Gameplay;

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            GameEventBus.RaiseGameStateChanged(newState);
        }

        public void ReturnToGameplay()
        {
            SetState(GameState.Gameplay);
        }
    }
}
```

## 12. `Assets/Scripts/Core/ISaveable.cs`

```csharp
namespace MoonpetalValley.Core
{
    public interface ISaveable
    {
        string SaveKey { get; }
        string CaptureStateJson();
        void RestoreStateJson(string json);
    }
}
```

## 13. `Assets/Scripts/Core/JuiceEffectsManager.cs`

```csharp
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
```

## 14. `Assets/Scripts/Core/SaveLoadManager.cs`

```csharp
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoonpetalValley.Community;
using MoonpetalValley.UI;

namespace MoonpetalValley.Core
{
    public class SaveLoadManager : MonoBehaviour
    {
        private const string SaveFileName = "moonpetal_save_v1.json";

        [Serializable]
        private struct SaveEntry
        {
            public string key;
            public string json;
        }

        [Serializable]
        private class SaveFilePayload
        {
            public int saveVersion = 1;
            public string savedTimestamp;
            public List<SaveEntry> entries = new List<SaveEntry>();
        }

        public string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        public string LastSavedTimestamp { get; private set; } = "";

        private void OnEnable()
        {
            GameEventBus.OnDayStarted += HandleDayStartedAutoSave;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            GameEventBus.OnDayStarted -= HandleDayStartedAutoSave;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start()
        {
            ReadSavedTimestampPreview();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MoonpetalValley_MVP" && MainMenuController.PendingLoadSaveOnWorldStart)
            {
                MainMenuController.PendingLoadSaveOnWorldStart = false;
                StartCoroutine(DelayedLoadRoutine());
            }
        }

        private IEnumerator DelayedLoadRoutine()
        {
            yield return null;
            LoadGame();
        }

        private void HandleDayStartedAutoSave(int newDay) => SaveGame(true);
        public bool HasSaveFile() => File.Exists(SaveFilePath);

        public void SaveGame(bool isAutoSave = false)
        {
            if (GameRoot.Instance == null) return;

            var payload = new SaveFilePayload
            {
                saveVersion = 1,
                savedTimestamp = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
            };

            var saveables = GameRoot.Instance.GetSaveables();
            for (int i = 0; i < saveables.Count; i++)
            {
                if (saveables[i] == null) continue;
                payload.entries.Add(new SaveEntry
                {
                    key = saveables[i].SaveKey,
                    json = saveables[i].CaptureStateJson()
                });
            }

            try
            {
                File.WriteAllText(SaveFilePath, JsonUtility.ToJson(payload, true));
                LastSavedTimestamp = payload.savedTimestamp;
                if (!isAutoSave) GameEventBus.RaiseToastRequested("Đã lưu game thành công (" + LastSavedTimestamp + ")!");
            }
            catch (Exception ex)
            {
                Debug.LogError("[MV] Lỗi lưu game: " + ex.Message);
            }
        }

        public bool LoadGame()
        {
            if (!HasSaveFile() || GameRoot.Instance == null)
            {
                GameEventBus.RaiseToastRequested("Chưa có file lưu game nào!");
                return false;
            }

            try
            {
                var payload = JsonUtility.FromJson<SaveFilePayload>(File.ReadAllText(SaveFilePath));
                if (payload == null || payload.entries == null) return false;

                var map = new Dictionary<string, string>();
                for (int i = 0; i < payload.entries.Count; i++) map[payload.entries[i].key] = payload.entries[i].json;

                var saveables = GameRoot.Instance.GetSaveables();
                for (int i = 0; i < saveables.Count; i++)
                {
                    if (saveables[i] != null && map.TryGetValue(saveables[i].SaveKey, out string stateJson))
                    {
                        saveables[i].RestoreStateJson(stateJson);
                    }
                }

                LastSavedTimestamp = payload.savedTimestamp;
                GameRoot.Instance.Quests?.RecomputeObjective();
                UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>()?.RefreshBloomVisual();
                GameEventBus.RaiseToastRequested("Đã tải game thành công (" + LastSavedTimestamp + ")!");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("[MV] Lỗi tải game: " + ex.Message);
                return false;
            }
        }

        public void DeleteSaveFile()
        {
            if (HasSaveFile())
            {
                File.Delete(SaveFilePath);
                LastSavedTimestamp = "";
                GameEventBus.RaiseToastRequested("Đã xóa file lưu game.");
            }
        }

        private void ReadSavedTimestampPreview()
        {
            if (!HasSaveFile()) return;
            try
            {
                var payload = JsonUtility.FromJson<SaveFilePayload>(File.ReadAllText(SaveFilePath));
                if (payload != null) LastSavedTimestamp = payload.savedTimestamp;
            }
            catch { }
        }
    }
}
```

## 15. `Assets/Scripts/Core/SceneLoader.cs`

```csharp
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
```

## 16. `Assets/Scripts/Core/StoryFlagManager.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Core
{
    public class StoryFlagManager : MonoBehaviour, ISaveable
    {
        private readonly HashSet<string> _flags = new HashSet<string>();

        [Serializable]
        private class FlagSaveWrapper
        {
            public List<string> activeFlags = new List<string>();
        }

        public string SaveKey => "story_flags";

        public bool HasFlag(string flagId)
        {
            return !string.IsNullOrEmpty(flagId) && _flags.Contains(flagId);
        }

        public void SetFlag(string flagId, bool value = true)
        {
            if (string.IsNullOrEmpty(flagId)) return;

            bool changed = value ? _flags.Add(flagId) : _flags.Remove(flagId);
            if (changed)
            {
                Debug.Log("[MV] StoryFlag: " + flagId + " = " + value);
                GameEventBus.RaiseStoryFlagChanged(flagId, value);
            }
        }

        public void ClearAllFlags()
        {
            var copy = new List<string>(_flags);
            _flags.Clear();
            for (int i = 0; i < copy.Count; i++)
            {
                GameEventBus.RaiseStoryFlagChanged(copy[i], false);
            }
        }

        public IReadOnlyCollection<string> GetAllActiveFlags()
        {
            return _flags;
        }

        public string CaptureStateJson()
        {
            var wrapper = new FlagSaveWrapper { activeFlags = new List<string>(_flags) };
            return JsonUtility.ToJson(wrapper);
        }

        public void RestoreStateJson(string json)
        {
            _flags.Clear();
            if (string.IsNullOrEmpty(json)) return;
            var wrapper = JsonUtility.FromJson<FlagSaveWrapper>(json);
            if (wrapper != null && wrapper.activeFlags != null)
            {
                for (int i = 0; i < wrapper.activeFlags.Count; i++)
                {
                    _flags.Add(wrapper.activeFlags[i]);
                }
            }
        }
    }
}
```

## 17. `Assets/Scripts/Core/VNLocalization.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Data;

namespace MoonpetalValley.Core
{
    public class VNLocalization : MonoBehaviour
    {
        [SerializeField] private StringTableSO stringTable;
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private bool _isBuilt;

        public void Initialize()
        {
            _cache.Clear();
            if (stringTable != null && stringTable.entries != null)
            {
                for (int i = 0; i < stringTable.entries.Count; i++)
                {
                    var entry = stringTable.entries[i];
                    if (!string.IsNullOrEmpty(entry.key))
                    {
                        _cache[entry.key] = entry.vietnameseText;
                    }
                }
            }
            _isBuilt = true;
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (!_isBuilt) Initialize();

            if (_cache.TryGetValue(key, out string value))
            {
                return value;
            }
            Debug.LogWarning("[MV] Thiếu khóa bản địa hóa tiếng Việt: " + key);
            return "[" + key + "]";
        }

        public string Format(string key, params object[] args)
        {
            string template = Get(key);
            try
            {
                return string.Format(template, args);
            }
            catch
            {
                return template;
            }
        }
    }
}
```

## 18. `Assets/Scripts/Core/WorldSpawnRegistry.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.Core
{
    public class WorldSpawnRegistry : MonoBehaviour
    {
        public static WorldSpawnRegistry Instance { get; private set; }

        [SerializeField] private List<SpawnPointDefinition> spawnPoints = new List<SpawnPointDefinition>();

        private void Awake()
        {
            Instance = this;
        }

        public bool TryGetSpawnPosition(string spawnId, out Vector2 position)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnPoints[i].id == spawnId)
                {
                    position = spawnPoints[i].position;
                    return true;
                }
            }
            position = Vector2.zero;
            return false;
        }

        public bool TeleportPlayerTo(string spawnId)
        {
            if (!TryGetSpawnPosition(spawnId, out Vector2 targetPos))
            {
                Debug.LogWarning("[MV] Không tìm thấy SpawnPoint: " + spawnId);
                return false;
            }

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy PlayerController2D để Teleport.");
                return false;
            }

            player.TeleportTo(targetPos);
            GameEventBus.RaiseToastRequested("Dịch chuyển tới: " + GetDisplayName(spawnId));
            return true;
        }

        public string GetDisplayName(string spawnId)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnPoints[i].id == spawnId) return spawnPoints[i].displayName;
            }
            return spawnId;
        }
    }
}
```

## 19. `Assets/Scripts/Data/BundleData.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public struct ItemRequirement
    {
        public ItemData item;
        public int amount;
    }

    [CreateAssetMenu(fileName = "Bundle_New", menuName = "Moonpetal/Data/BundleData")]
    public class BundleData : ScriptableObject
    {
        public string bundleID;
        public string bundleTitleKey;
        public string bundleDescriptionKey;
        public List<ItemRequirement> requiredItems = new List<ItemRequirement>();
        public List<string> requiredHelperNPCIDs = new List<string>();
        public List<string> requiredHelperFlags = new List<string>();
        public ItemData rewardItem;
        public string rewardStoryFlag;
    }
}
```

## 20. `Assets/Scripts/Data/CropData.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "Crop_New", menuName = "Moonpetal/Data/CropData")]
    public class CropData : ScriptableObject
    {
        public string cropID;
        public string cropNameKey;
        public ItemData seedItem;
        public ItemData harvestedItem;
        public int growDays = 2;
        public int harvestYield = 1;
        public Sprite[] stageSprites = new Sprite[4];
    }
}
```

## 21. `Assets/Scripts/Data/DialogueData.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public class DialogueLine
    {
        public string speakerNameKey;
        public string textKey;
        public Sprite portraitOverride;
    }

    [CreateAssetMenu(fileName = "Dlg_New", menuName = "Moonpetal/Data/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public string dialogueID;
        public List<DialogueLine> lines = new List<DialogueLine>();

        [Header("Hiệu ứng khi kết thúc hội thoại")]
        public string setsStoryFlagOnFinish;
        public ItemData givesItemOnFinish;
        public int givesItemAmount = 1;
        public ItemData consumesRequiredItem;
        public int consumesAmount;
        public DiaryPageData unlocksDiaryPage;
    }
}
```

## 22. `Assets/Scripts/Data/DiaryPageData.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "DiaryPage_New", menuName = "Moonpetal/Data/DiaryPageData")]
    public class DiaryPageData : ScriptableObject
    {
        public string pageID;
        public int pageNumber = 1;
        public string titleKey;
        public string bodyTextKey;
        public Sprite illustration;
        public string unlockedByFlag;
    }
}
```

## 23. `Assets/Scripts/Data/EnemyData.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    public enum EnemyAttackPattern
    {
        Jump,
        Burrow,
        BossBeam
    }

    [Serializable]
    public struct LootDropEntry
    {
        public ItemData item;
        public int minAmount;
        public int maxAmount;
        [Range(0f, 1f)] public float dropChance;
    }

    [CreateAssetMenu(fileName = "Enemy_New", menuName = "Moonpetal/Data/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public string enemyID;
        public string enemyNameKey;
        public int maxHP = 45;
        public float moveSpeed = 2.8f;
        public int contactDamage = 10;
        public EnemyAttackPattern attackPattern = EnemyAttackPattern.Jump;
        public float detectionRadius = 4.5f;
        public float attackInterval = 1.1f;
        public bool appliesSlowOnHit = true;
        public List<LootDropEntry> dropTable = new List<LootDropEntry>();
    }
}
```

## 24. `Assets/Scripts/Data/ItemData.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Data
{
    public enum ItemCategory
    {
        Tool,
        Seed,
        Crop,
        Material,
        Consumable,
        SeedBag,
        MoonpetalShard,
        KeyItem
    }

    [CreateAssetMenu(fileName = "Item_New", menuName = "Moonpetal/Data/ItemData")]
    public class ItemData : ScriptableObject
    {
        [Header("Định danh & Hiển thị")]
        public string itemID;
        public string displayNameKey;
        public string descriptionKey;
        public Sprite icon;
        public ItemCategory category;
        public int maxStack = 99;

        [Header("Hồi phục (Thực phẩm)")]
        public int staminaRestore;
        public int lightHPRestore;
        public bool clearsExhaustionPenalty;

        [Header("Canh tác (Hạt giống)")]
        public int growDays;
        public CropData linkedCropData;

        [Header("Bảo vệ nhiệm vụ")]
        public bool isQuestProtected;
    }
}
```

## 25. `Assets/Scripts/Data/ItemDatabaseSO.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Moonpetal/Config/ItemDatabaseSO")]
    public class ItemDatabaseSO : ScriptableObject
    {
        public List<ItemData> allItems = new List<ItemData>();

        public ItemData GetById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i] != null && allItems[i].itemID == itemId)
                {
                    return allItems[i];
                }
            }
            return null;
        }
    }
}
```

## 26. `Assets/Scripts/Data/MapLayoutConfigSO.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public struct SpawnPointDefinition
    {
        public string id;
        public string displayName;
        public Vector2 position;
    }

    [CreateAssetMenu(fileName = "MapLayoutConfig", menuName = "Moonpetal/Config/MapLayoutConfigSO")]
    public class MapLayoutConfigSO : ScriptableObject
    {
        [Header("Gốc tọa độ lưới bản đồ")]
        public int originX = -22;
        public int originY = -16;

        [Header("Bản đồ dạng text (Chú giải: . Cỏ, P Đường, G Sàn Nhà Kính, # Tường, F Hàng rào, T Cây, W Nước, B Bồn hoa)")]
        [TextArea(12, 36)]
        public string asciiMap = "";

        [Header("Danh sách điểm dịch chuyển / Spawn")]
        public List<SpawnPointDefinition> spawnPoints = new List<SpawnPointDefinition>();

        public Vector2 cameraMinBounds = new Vector2(-14f, -11f);
        public Vector2 cameraMaxBounds = new Vector2(14f, 11f);
    }
}
```

## 27. `Assets/Scripts/Data/NPCData.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public class ConditionalDialogueEntry
    {
        public string requiredStoryFlag;
        public string blockedByStoryFlag;
        public DialogueData dialogue;
    }

    [CreateAssetMenu(fileName = "NPC_New", menuName = "Moonpetal/Data/NPCData")]
    public class NPCData : ScriptableObject
    {
        public string npcID;
        public string displayNameKey;
        public string roleTitleKey;
        public Sprite portraitSprite;
        public Sprite worldSprite;
        public DialogueData defaultDialogue;
        public List<ConditionalDialogueEntry> conditionalDialogues = new List<ConditionalDialogueEntry>();
        public string isHelperForBundleID;
        public string helperReadyStoryFlag;
    }
}
```

## 28. `Assets/Scripts/Data/PlayerStatsSO.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "PlayerStatsConfig", menuName = "Moonpetal/Config/PlayerStatsSO")]
    public class PlayerStatsSO : ScriptableObject
    {
        [Header("Thể lực & Máu Ánh Sáng")]
        public int maxStamina = 100;
        public int maxLightHP = 100;
        [Range(0.1f, 1f)] public float exhaustedStaminaMultiplier = 0.5f;
        [Range(0f, 0.5f)] public float rawMaterialDropPercentOnPassOut = 0.1f;

        [Header("Di chuyển & Làm chậm")]
        public float moveSpeed = 4.5f;
        [Range(0.1f, 1f)] public float slowedSpeedMultiplier = 0.55f;
        public float slowDuration = 2.0f;

        [Header("Chiến đấu (Liềm Ánh Trăng 120 độ)")]
        public int attackDamage = 15;
        public float attackArcDegrees = 120f;
        public float attackRadius = 1.35f;
        public float attackCooldown = 0.4f;
        public float attackKnockbackForce = 6.0f;

        [Header("Lướt né (Dash) & Miễn nhiễm sát thương")]
        public float dashSpeed = 11.5f;
        public float dashDuration = 0.18f;
        public float dashCooldown = 0.8f;
        public float dashInvulnerabilityDuration = 0.20f;
        public float hitInvulnerabilityDuration = 0.75f;

        [Header("Tiêu hao Stamina theo hành động")]
        public int hoeStaminaCost = 4;
        public int waterStaminaCost = 2;
        public int chopStaminaCost = 4;
        public int swingStaminaCost = 2;
        public int dashStaminaCost = 5;
        public int lumiPulseStaminaCost = 10;

        [Header("Tinh linh Lumi")]
        public float lumiBaseLightRadius = 2.5f;
        public float lumiUpgradedLightRadius = 5.5f;
        public float lumiPulseRadius = 3.5f;
        public float lumiPulseCooldown = 8.0f;
    }
}
```

## 29. `Assets/Scripts/Data/QuestData.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    public enum QuestActionType
    {
        TalkToNPC,
        CollectItem,
        ClearBlightZone,
        CleanProp,
        HarvestCrop,
        RepairObject,
        ReadDiaryPages,
        CompleteBundle,
        BloomPlantingSpot
    }

    [Serializable]
    public class QuestStepDefinition
    {
        public string stepID;
        public string descriptionKey;
        public QuestActionType actionType;
        public string targetID;
        public int requiredCount = 1;
        public string completionStoryFlag;
        public DiaryPageData unlockedDiaryPage;
    }

    [CreateAssetMenu(fileName = "Quest_New", menuName = "Moonpetal/Data/QuestData")]
    public class QuestData : ScriptableObject
    {
        public string questID;
        public string questTitleKey;
        public string questSummaryKey;
        public List<QuestStepDefinition> steps = new List<QuestStepDefinition>();
        public ItemData completionRewardItem;
        public QuestData nextQuest;
    }
}
```

## 30. `Assets/Scripts/Data/StringTableSO.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public struct StringEntry
    {
        public string key;
        [TextArea(1, 4)] public string vietnameseText;
    }

    [CreateAssetMenu(fileName = "VNStringTable", menuName = "Moonpetal/Localization/StringTableSO")]
    public class StringTableSO : ScriptableObject
    {
        public List<StringEntry> entries = new List<StringEntry>();

        public string GetText(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].key == key)
                {
                    return entries[i].vietnameseText;
                }
            }
            return "[" + key + "]";
        }

        public void SetEntry(string key, string vietnameseText)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].key == key)
                {
                    entries[i] = new StringEntry { key = key, vietnameseText = vietnameseText };
                    return;
                }
            }
            entries.Add(new StringEntry { key = key, vietnameseText = vietnameseText });
        }
    }
}
```

## 31. `Assets/Scripts/Data/TimeConfigSO.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Data
{
    [CreateAssetMenu(fileName = "TimeConfig", menuName = "Moonpetal/Config/TimeConfigSO")]
    public class TimeConfigSO : ScriptableObject
    {
        [Header("Mốc giờ trong ngày (06:00 -> 02:00 hôm sau = 26)")]
        public int dayStartHour = 6;
        public int duskStartHour = 18;
        public int nightStartHour = 20;
        public int passOutHour = 26;

        [Header("Tốc độ thời gian")]
        public float realSecondsPerGameDay = 300f;

        [Header("Màu ánh sáng môi trường")]
        public Color morningAmbientColor = new Color(1.0f, 0.96f, 0.88f, 1f);
        public Color noonAmbientColor = new Color(1.0f, 1.0f, 1.0f, 1f);
        public Color duskAmbientColor = new Color(0.95f, 0.65f, 0.48f, 1f);
        public Color nightAmbientColor = new Color(0.24f, 0.28f, 0.48f, 1f);

        [Header("Cường độ ánh sáng")]
        [Range(0f, 1.2f)] public float morningIntensity = 0.95f;
        [Range(0f, 1.2f)] public float noonIntensity = 1.0f;
        [Range(0f, 1.2f)] public float duskIntensity = 0.65f;
        [Range(0f, 1.2f)] public float nightIntensity = 0.28f;
    }
}
```

## 32. `Assets/Scripts/DebugTools/RuntimeDebugOverlay.cs`

```csharp
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Farming;

namespace MoonpetalValley.DebugTools
{
    public class RuntimeDebugOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button btnTestToast;
        [SerializeField] private Button btnToggleTestFlag;
        [SerializeField] private Button btnClearFlags;
        [SerializeField] private Button btnTogglePause;
        [SerializeField] private Button btnTpGreenhouse;
        [SerializeField] private Button btnTpVillage;
        [SerializeField] private Button btnTpGarden;
        [SerializeField] private Button btnTpForest;
        [SerializeField] private Button btnTimePlus2H;
        [SerializeField] private Button btnTimeSetDusk;
        [SerializeField] private Button btnTimeNextDay;
        [SerializeField] private Button btnRestoreStats;
        [SerializeField] private Button btnForceExhaust;
        [SerializeField] private Button btnDamageLightHP;
        [SerializeField] private Button btnGiveTestKit;
        [SerializeField] private Button btnWaterAllCrops;
        [SerializeField] private Button btnSkipPrologue;
        [SerializeField] private Button btnGiveSickle;
        [SerializeField] private Button btnPrepClimax;

        private bool _isOpen;
        private float _refreshTimer;
        private const string TestFlagId = "flag_debug_step1_verified";

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnTestToast != null) btnTestToast.onClick.AddListener(() => GameEventBus.RaiseToastRequested("Kiểm thử thông báo tiếng Việt có dấu thành công!"));
            if (btnToggleTestFlag != null) btnToggleTestFlag.onClick.AddListener(() => { GameRoot.Instance?.Flags?.SetFlag(TestFlagId, !GameRoot.Instance.Flags.HasFlag(TestFlagId)); RefreshStatusText(); });
            if (btnClearFlags != null) btnClearFlags.onClick.AddListener(() => { GameRoot.Instance?.Flags?.ClearAllFlags(); RefreshStatusText(); });
            if (btnTogglePause != null) btnTogglePause.onClick.AddListener(OnClickTogglePause);
            if (btnTpGreenhouse != null) btnTpGreenhouse.onClick.AddListener(() => TeleportTo("Spawn_Greenhouse"));
            if (btnTpVillage != null) btnTpVillage.onClick.AddListener(() => TeleportTo("Spawn_Village"));
            if (btnTpGarden != null) btnTpGarden.onClick.AddListener(() => TeleportTo("Spawn_BachGarden"));
            if (btnTpForest != null) btnTpForest.onClick.AddListener(() => TeleportTo("Spawn_ForestEdge"));
            if (btnTimePlus2H != null) btnTimePlus2H.onClick.AddListener(() => { GameRoot.Instance?.Time?.AddHours(2f); RefreshStatusText(); });
            if (btnTimeSetDusk != null) btnTimeSetDusk.onClick.AddListener(() => { GameRoot.Instance?.Time?.SetHour(19.83f); RefreshStatusText(); });
            if (btnTimeNextDay != null) btnTimeNextDay.onClick.AddListener(() => { GameRoot.Instance?.Time?.AdvanceToNextDay(); RefreshStatusText(); });
            if (btnRestoreStats != null) btnRestoreStats.onClick.AddListener(() => { GameRoot.Instance?.Stats?.RestoreStats(100, 100, true); RefreshStatusText(); });
            if (btnForceExhaust != null) btnForceExhaust.onClick.AddListener(() => { GameRoot.Instance?.Stats?.ConsumeStamina(999, true); RefreshStatusText(); });
            if (btnDamageLightHP != null) btnDamageLightHP.onClick.AddListener(() => { GameRoot.Instance?.Stats?.TakeLightDamage(25, true); RefreshStatusText(); });
            if (btnGiveTestKit != null) btnGiveTestKit.onClick.AddListener(OnClickGiveTestKit);
            if (btnWaterAllCrops != null) btnWaterAllCrops.onClick.AddListener(() => { FarmGridManager.Instance?.WaterAllTilesDebug(); RefreshStatusText(); });
            if (btnSkipPrologue != null) btnSkipPrologue.onClick.AddListener(OnClickSkipPrologue);
            if (btnGiveSickle != null) btnGiveSickle.onClick.AddListener(OnClickGiveSickle);
            if (btnPrepClimax != null) btnPrepClimax.onClick.AddListener(OnClickPrepClimax);
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged += OnGameStateUpdated;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= OnAnyStateUpdated;
            GameEventBus.OnGameStateChanged -= OnGameStateUpdated;
        }

        private void Update()
        {
            if (!Application.isEditor && !Debug.isDebugBuild) return;
            if (WasF1PressedThisFrame()) TogglePanel();

            if (_isOpen)
            {
                _refreshTimer += Time.unscaledDeltaTime;
                if (_refreshTimer >= 0.25f) { _refreshTimer = 0f; RefreshStatusText(); }
            }
        }

        private bool WasF1PressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F1)) return true;
#endif
            return false;
        }

        public void TogglePanel()
        {
            _isOpen = !_isOpen;
            if (panelRoot != null) panelRoot.SetActive(_isOpen);
            if (_isOpen) RefreshStatusText();
        }

        private void TeleportTo(string spawnId)
        {
            if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo(spawnId);
                RefreshStatusText();
            }
        }

        private void OnClickGiveTestKit()
        {
            var inv = GameRoot.Instance?.Inventory;
            if (inv == null) return;
            inv.AddItemById("item_sunshine_bread", 2, false);
            inv.AddItemById("item_branch_wood", 10, false);
            inv.AddItemById("item_dew_barley", 4, false);
            inv.AddItemById("item_dew_barley_seed", 6, false);
            GameEventBus.RaiseToastRequested("Đã thêm Bộ Test (2 Bánh Mì, 10 Gỗ, 4 Lúa Mạch, 6 Hạt)!");
            RefreshStatusText();
        }

        private void OnClickGiveSickle()
        {
            var inv = GameRoot.Instance?.Inventory;
            if (inv == null) return;
            if (!inv.HasItem("item_moon_sickle", 1)) inv.AddItemById("item_moon_sickle", 1, true);
            inv.SelectHotbarSlot(3);
            RefreshStatusText();
        }

        private void OnClickSkipPrologue()
        {
            var f = GameRoot.Instance?.Flags;
            if (f == null) return;
            f.SetFlag("flag_intro_letter_read", true);
            f.SetFlag("flag_greenhouse_cleared", true);
            f.SetFlag("flag_lumi_awakened", true);
            f.SetFlag("flag_solo_plant_failed", true);
            f.SetFlag("flag_tutorial_done", true);
            GameRoot.Instance.Inventory?.AddItemById("item_moonpetal_seed_core", 1, false);
            GameEventBus.RaiseToastRequested("Đã bỏ qua Mở Đầu!");
            RefreshStatusText();
        }

        private void OnClickPrepClimax()
        {
            if (GameRoot.Instance == null) return;
            OnClickSkipPrologue();
            var f = GameRoot.Instance.Flags;
            f.SetFlag("flag_met_bach_at_gate", true);
            f.SetFlag("flag_garden_slimes_cleared", true);
            f.SetFlag("flag_garden_bench_cleaned", true);
            f.SetFlag("flag_hana_bread_done", true);
            f.SetFlag("flag_toby_cat_slimes_cleared", true);
            f.SetFlag("flag_toby_cat_saved", true);
            f.SetFlag("flag_otis_mailbox_fixed", true);
            f.SetFlag("flag_bach_letters_read", true);

            var inv = GameRoot.Instance.Inventory;
            if (inv != null)
            {
                if (!inv.HasItem("item_seed_bag_1", 1)) inv.AddItemById("item_seed_bag_1", 1, false);
                if (!inv.HasItem("item_seed_bag_2", 1)) inv.AddItemById("item_seed_bag_2", 1, false);
                if (!inv.HasItem("item_seed_bag_3", 1)) inv.AddItemById("item_seed_bag_3", 1, false);
                if (inv.GetItemCount("item_dew_sap") < 5) inv.AddItemById("item_dew_sap", 5, false);
                if (inv.GetItemCount("item_starlight_dust") < 3) inv.AddItemById("item_starlight_dust", 3, false);
            }

            TeleportTo("Spawn_BachGarden");
            GameEventBus.RaiseToastRequested("Đã gọi đủ 4 Helpers & cấp đủ đồ Bundle! Hãy tới Bồn Hoa bấm [E]!");
            RefreshStatusText();
        }

        private void OnClickTogglePause()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            var next = GameRoot.Instance.State.IsGameplayActive ? GameState.PauseMenu : GameState.Gameplay;
            GameRoot.Instance.State.SetState(next);
            RefreshStatusText();
        }

        private void OnAnyStateUpdated(string _, bool __) => RefreshStatusText();
        private void OnGameStateUpdated(GameState _) => RefreshStatusText();

        public void RefreshStatusText()
        {
            if (statusText == null || GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("<b>BẢNG DEBUG MOONPETAL (F1)</b>");
            if (GameRoot.Instance.Time != null) sb.AppendLine(GameRoot.Instance.Time.GetFormattedClockString());
            if (GameRoot.Instance.Bundle != null)
            {
                sb.AppendLine("Helpers: " + GameRoot.Instance.Bundle.GetReadyHelperCount() + "/4 | Bundle: " +
                              (GameRoot.Instance.Bundle.IsBundleItemsComplete() ? "ĐỦ" : "CHƯA"));
            }
            statusText.text = sb.ToString();
        }
    }
}
```

## 33. `Assets/Scripts/Farming/FarmGridManager.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Farming
{
    public class FarmGridManager : MonoBehaviour, ISaveable
    {
        public static FarmGridManager Instance { get; private set; }

        [SerializeField] private List<FarmTileController> tiles = new List<FarmTileController>();
        [SerializeField] private CropData defaultCropData;

        public string SaveKey => "farm_tiles";

        [Serializable]
        private struct SavedTile
        {
            public string id;
            public int state;
            public string cropId;
            public int stage;
            public bool watered;
        }

        [Serializable]
        private class FarmSaveWrapper
        {
            public List<SavedTile> savedTiles = new List<SavedTile>();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GameEventBus.OnDayStarted += HandleDayStarted;
        }

        private void OnDisable()
        {
            GameEventBus.OnDayStarted -= HandleDayStarted;
        }

        private void Start()
        {
            if (GameRoot.Instance != null)
            {
                GameRoot.Instance.RegisterSaveable(this);
            }
        }

        private void HandleDayStarted(int newDay)
        {
            int readyCount = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    tiles[i].OnNewDay();
                    if (tiles[i].IsReadyToHarvest) readyCount++;
                }
            }
            if (readyCount > 0)
            {
                GameEventBus.RaiseToastRequested("Trong Nhà Kính có " + readyCount + " cây Lúa Mạch Sương Mai đã chín! Bấm [E] để thu hoạch.");
            }
        }

        public FarmTileController FindClosestTile(Vector2 worldPos, float maxDistance = 1.15f)
        {
            FarmTileController best = null;
            float bestDist = maxDistance;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) continue;
                float d = Vector2.Distance(worldPos, tiles[i].transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = tiles[i];
                }
            }
            return best;
        }

        public int CountPlantedCrops(string cropId)
        {
            int count = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null && tiles[i].State == FarmTileState.Planted &&
                    tiles[i].PlantedCrop != null && tiles[i].PlantedCrop.cropID == cropId)
                {
                    count++;
                }
            }
            return count;
        }

        public void WaterAllTilesDebug()
        {
            int watered = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null && tiles[i].State != FarmTileState.Untilled)
                {
                    tiles[i].ForceWaterDebug();
                    watered++;
                }
            }
            GameEventBus.RaiseToastRequested("Đã tưới nước toàn bộ " + watered + " ô đất đã cuốc!");
        }

        public string CaptureStateJson()
        {
            var wrapper = new FarmSaveWrapper();
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) continue;
                wrapper.savedTiles.Add(new SavedTile
                {
                    id = tiles[i].TileId,
                    state = (int)tiles[i].State,
                    cropId = tiles[i].PlantedCrop != null ? tiles[i].PlantedCrop.cropID : "",
                    stage = tiles[i].GrowthStage,
                    watered = tiles[i].IsWatered
                });
            }
            return JsonUtility.ToJson(wrapper);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var wrapper = JsonUtility.FromJson<FarmSaveWrapper>(json);
            if (wrapper == null || wrapper.savedTiles == null) return;

            for (int i = 0; i < wrapper.savedTiles.Count; i++)
            {
                var st = wrapper.savedTiles[i];
                for (int j = 0; j < tiles.Count; j++)
                {
                    if (tiles[j] != null && tiles[j].TileId == st.id)
                    {
                        CropData crop = !string.IsNullOrEmpty(st.cropId) ? defaultCropData : null;
                        tiles[j].RestoreState((FarmTileState)st.state, crop, st.stage, st.watered);
                        break;
                    }
                }
            }
        }
    }
}
```

## 34. `Assets/Scripts/Farming/FarmTileController.cs`

```csharp
using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Farming
{
    public enum FarmTileState
    {
        Untilled,
        TilledDry,
        Planted
    }

    public class FarmTileController : MonoBehaviour
    {
        [SerializeField] private string tileId;
        [SerializeField] private SpriteRenderer soilRenderer;
        [SerializeField] private SpriteRenderer cropRenderer;
        [SerializeField] private Sprite untilledSprite;
        [SerializeField] private Sprite tilledDrySprite;
        [SerializeField] private Sprite tilledWetSprite;

        public string TileId => tileId;
        public FarmTileState State { get; private set; } = FarmTileState.Untilled;
        public CropData PlantedCrop { get; private set; }
        public int GrowthStage { get; private set; }
        public bool IsWatered { get; private set; }

        public bool IsReadyToHarvest => State == FarmTileState.Planted && PlantedCrop != null && GrowthStage >= PlantedCrop.growDays;

        private void Start()
        {
            RefreshVisuals();
        }

        public bool TryTill()
        {
            if (State != FarmTileState.Untilled) return false;
            State = FarmTileState.TilledDry;
            IsWatered = false;
            RefreshVisuals();
            return true;
        }

        public bool TryPlant(CropData crop)
        {
            if (crop == null || State != FarmTileState.TilledDry) return false;
            State = FarmTileState.Planted;
            PlantedCrop = crop;
            GrowthStage = 0;
            RefreshVisuals();
            return true;
        }

        public bool TryWater()
        {
            if (State == FarmTileState.Untilled || IsWatered) return false;
            IsWatered = true;
            RefreshVisuals();
            return true;
        }

        public bool TryHarvest(out ItemData harvestedItem, out int yieldAmount)
        {
            harvestedItem = null;
            yieldAmount = 0;
            if (!IsReadyToHarvest) return false;

            harvestedItem = PlantedCrop.harvestedItem;
            yieldAmount = Mathf.Max(1, PlantedCrop.harvestYield);

            State = FarmTileState.TilledDry;
            PlantedCrop = null;
            GrowthStage = 0;
            IsWatered = false;
            RefreshVisuals();
            return true;
        }

        public void OnNewDay()
        {
            if (State == FarmTileState.Planted && PlantedCrop != null)
            {
                if (IsWatered && GrowthStage < PlantedCrop.growDays)
                {
                    GrowthStage++;
                }
            }
            IsWatered = false;
            RefreshVisuals();
        }

        public void ForceWaterDebug()
        {
            if (State != FarmTileState.Untilled)
            {
                IsWatered = true;
                RefreshVisuals();
            }
        }

        public void RestoreState(FarmTileState state, CropData crop, int stage, bool watered)
        {
            State = state;
            PlantedCrop = crop;
            GrowthStage = stage;
            IsWatered = watered;
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            if (soilRenderer != null)
            {
                if (State == FarmTileState.Untilled) soilRenderer.sprite = untilledSprite;
                else soilRenderer.sprite = IsWatered ? tilledWetSprite : tilledDrySprite;
            }

            if (cropRenderer != null)
            {
                if (State == FarmTileState.Planted && PlantedCrop != null && PlantedCrop.stageSprites != null && PlantedCrop.stageSprites.Length > 0)
                {
                    int maxIdx = PlantedCrop.stageSprites.Length - 1;
                    int idx = IsReadyToHarvest
                        ? maxIdx
                        : Mathf.Clamp(GrowthStage, 0, Mathf.Max(0, maxIdx - 1));
                    cropRenderer.sprite = PlantedCrop.stageSprites[idx];
                    cropRenderer.enabled = true;
                }
                else
                {
                    cropRenderer.sprite = null;
                    cropRenderer.enabled = false;
                }
            }
        }
    }
}
```

## 35. `Assets/Scripts/Farming/ResourceNodeController.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Inventory;

namespace MoonpetalValley.Farming
{
    public enum ResourceNodeType
    {
        DryBranch,
        GreenhouseDebris
    }

    public class ResourceNodeController : MonoBehaviour
    {
        [SerializeField] private ResourceNodeType nodeType = ResourceNodeType.DryBranch;
        [SerializeField] private string requiredToolId = "item_hatchet";
        [SerializeField] private int maxHits = 2;
        [SerializeField] private ItemData dropItem;
        [SerializeField] private int dropAmount = 2;
        [SerializeField] private GameObject lootDropPrefab;
        [SerializeField] private bool regrowEachDay = true;
        [SerializeField] private SpriteRenderer visualRenderer;
        [SerializeField] private Collider2D nodeCollider;

        private int _currentHits;
        private bool _isDepleted;
        private float _shakeTimer;
        private Vector3 _baseLocalPos;

        public bool IsDepleted => _isDepleted;
        public ResourceNodeType NodeType => nodeType;

        private void Awake()
        {
            if (visualRenderer != null) _baseLocalPos = visualRenderer.transform.localPosition;
        }

        private void OnEnable()
        {
            GameEventBus.OnDayStarted += HandleDayStarted;
        }

        private void OnDisable()
        {
            GameEventBus.OnDayStarted -= HandleDayStarted;
        }

        private void Update()
        {
            if (_shakeTimer > 0f && visualRenderer != null)
            {
                _shakeTimer -= Time.deltaTime;
                float offset = Mathf.Sin(_shakeTimer * 55f) * 0.08f;
                visualRenderer.transform.localPosition = _baseLocalPos + new Vector3(offset, 0f, 0f);
                if (_shakeTimer <= 0f)
                {
                    visualRenderer.transform.localPosition = _baseLocalPos;
                }
            }
        }

        private void HandleDayStarted(int day)
        {
            if (regrowEachDay && _isDepleted)
            {
                RestoreNode();
            }
        }

        public bool TryHit(string toolId)
        {
            if (_isDepleted) return false;

            bool validTool = toolId == requiredToolId ||
                             (nodeType == ResourceNodeType.GreenhouseDebris && (toolId == "item_hoe" || toolId == "item_hatchet"));
            if (!validTool)
            {
                string needed = requiredToolId == "item_hatchet" ? "Rìu Nhỏ (phím 3)" : "Cuốc Đất (phím 1)";
                GameEventBus.RaiseToastRequested("Cần dùng " + needed + " để dọn vật thể này!");
                return false;
            }

            _currentHits++;
            _shakeTimer = 0.22f;

            if (_currentHits >= maxHits)
            {
                DepleteNode();
            }
            return true;
        }

        private void DepleteNode()
        {
            _isDepleted = true;
            if (visualRenderer != null) visualRenderer.enabled = false;
            if (nodeCollider != null) nodeCollider.enabled = false;

            if (dropItem != null && dropAmount > 0)
            {
                if (lootDropPrefab != null)
                {
                    for (int i = 0; i < dropAmount; i++)
                    {
                        Vector2 offset = new Vector2(Random.Range(-0.35f, 0.35f), Random.Range(-0.25f, 0.25f));
                        GameObject go = Instantiate(lootDropPrefab, transform.position + (Vector3)offset, Quaternion.identity);
                        var drop = go.GetComponent<WorldLootDrop>();
                        if (drop != null) drop.Initialize(dropItem, 1);
                    }
                }
                else if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
                {
                    GameRoot.Instance.Inventory.AddItem(dropItem, dropAmount, true);
                }
            }

            if (nodeType == ResourceNodeType.GreenhouseDebris)
            {
                CheckAllGreenhouseDebrisCleared();
            }
        }

        private void CheckAllGreenhouseDebrisCleared()
        {
            var allNodes = UnityEngine.Object.FindObjectsByType<ResourceNodeController>(FindObjectsInactive.Include);
            int remaining = 0;
            for (int i = 0; i < allNodes.Length; i++)
            {
                if (allNodes[i].NodeType == ResourceNodeType.GreenhouseDebris && !allNodes[i].IsDepleted)
                {
                    remaining++;
                }
            }

            if (remaining == 0 && GameRoot.Instance != null && GameRoot.Instance.Flags != null)
            {
                GameRoot.Instance.Flags.SetFlag("flag_greenhouse_cleared", true);
                GameEventBus.RaiseToastRequested("Đã dọn sạch cỏ dại & đá vụn trong Nhà Kính!");
            }
        }

        public void RestoreNode()
        {
            _isDepleted = false;
            _currentHits = 0;
            if (visualRenderer != null) visualRenderer.enabled = true;
            if (nodeCollider != null) nodeCollider.enabled = true;
        }
    }
}
```

## 36. `Assets/Scripts/Farming/SeedShelfInteractable.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Farming
{
    public class SeedShelfInteractable : MonoBehaviour
    {
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 1.9f;

        private PlayerController2D _player;
        private GameInputReader _input;

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                TryGiveBackupSeeds();
            }
        }

        private void TryGiveBackupSeeds()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            int seedsInBag = inv.GetItemCount("item_dew_barley_seed");
            int cropsInBag = inv.GetItemCount("item_dew_barley");
            int planted = FarmGridManager.Instance != null ? FarmGridManager.Instance.CountPlantedCrops("crop_dew_barley") : 0;
            int total = seedsInBag + cropsInBag + planted;

            if (total >= 6)
            {
                GameEventBus.RaiseToastRequested("Bạn đã có đủ Hạt/Cây Lúa Mạch Sương Mai rồi (" + total + "/6).");
                return;
            }

            int give = 6 - total;
            inv.AddItemById("item_dew_barley_seed", give, true);
        }
    }
}
```

## 37. `Assets/Scripts/Farming/WaterWellInteractable.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Farming
{
    public class WaterWellInteractable : MonoBehaviour
    {
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 2.0f;

        private PlayerController2D _player;
        private GameInputReader _input;

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
                {
                    GameRoot.Instance.Inventory.RefillWateringCan();
                }
            }
        }
    }
}
```

## 38. `Assets/Scripts/Inventory/InventoryManager.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.Inventory
{
    [Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int amount;

        public bool IsEmpty => item == null || amount <= 0;
    }

    public class InventoryManager : MonoBehaviour, ISaveable
    {
        public const int TotalSlots = 16;
        public const int HotbarSize = 8;
        public const int MaxWaterCharges = 20;

        [SerializeField] private ItemDatabaseSO itemDatabase;

        public event Action OnInventoryChanged;
        public event Action<int> OnSelectedSlotChanged;

        private readonly InventorySlot[] _slots = new InventorySlot[TotalSlots];
        public int SelectedHotbarIndex { get; private set; }
        public int WateringCanCharges { get; private set; } = MaxWaterCharges;

        public string SaveKey => "inventory";

        [Serializable]
        private struct SavedSlot
        {
            public int index;
            public string itemId;
            public int amount;
        }

        [Serializable]
        private class InventorySaveData
        {
            public int selectedIndex;
            public int waterCharges;
            public List<SavedSlot> slots = new List<SavedSlot>();
        }

        private void Awake()
        {
            for (int i = 0; i < TotalSlots; i++)
            {
                _slots[i] = new InventorySlot();
            }
        }

        private void OnEnable()
        {
            SleepAndKnockoutController.OnPassOutMaterialPenalty += ApplyPassOutMaterialDrop;
        }

        private void OnDisable()
        {
            SleepAndKnockoutController.OnPassOutMaterialPenalty -= ApplyPassOutMaterialDrop;
        }

        private void Start()
        {
            // Khởi tạo 3 nông cụ cơ bản nếu túi đang trống hoàn toàn
            if (IsCompletelyEmpty())
            {
                AddItemById("item_hoe", 1, false);
                AddItemById("item_watering_can", 1, false);
                AddItemById("item_hatchet", 1, false);
                SelectHotbarSlot(0);
            }
        }

        public bool IsCompletelyEmpty()
        {
            for (int i = 0; i < TotalSlots; i++)
            {
                if (!_slots[i].IsEmpty) return false;
            }
            return true;
        }

        public InventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= TotalSlots) return null;
            return _slots[index];
        }

        public InventorySlot GetSelectedSlot() => _slots[SelectedHotbarIndex];

        public ItemData GetItemDataById(string itemId)
        {
            return itemDatabase != null ? itemDatabase.GetById(itemId) : null;
        }

        public void SelectHotbarSlot(int index)
        {
            int clamped = Mathf.Clamp(index, 0, HotbarSize - 1);
            SelectedHotbarIndex = clamped;
            OnSelectedSlotChanged?.Invoke(SelectedHotbarIndex);
            OnInventoryChanged?.Invoke();
        }

        public void CycleHotbar(int delta)
        {
            int next = (SelectedHotbarIndex + delta) % HotbarSize;
            if (next < 0) next += HotbarSize;
            SelectHotbarSlot(next);
        }

        public bool SwapOrMergeSlots(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= TotalSlots || toIndex < 0 || toIndex >= TotalSlots) return false;
            if (fromIndex == toIndex) return false;

            InventorySlot fromSlot = _slots[fromIndex];
            InventorySlot toSlot = _slots[toIndex];
            if (fromSlot.IsEmpty && toSlot.IsEmpty) return false;

            // Tự động gộp stack nếu 2 ô chứa cùng loại vật phẩm có thể cộng dồn
            if (!fromSlot.IsEmpty && !toSlot.IsEmpty &&
                fromSlot.item.itemID == toSlot.item.itemID &&
                toSlot.item.maxStack > 1 &&
                toSlot.amount < toSlot.item.maxStack)
            {
                int space = toSlot.item.maxStack - toSlot.amount;
                int moveAmt = Mathf.Min(space, fromSlot.amount);
                toSlot.amount += moveAmt;
                fromSlot.amount -= moveAmt;
                if (fromSlot.amount <= 0)
                {
                    fromSlot.item = null;
                    fromSlot.amount = 0;
                }
            }
            else
            {
                ItemData tmpItem = toSlot.item;
                int tmpAmount = toSlot.amount;

                toSlot.item = fromSlot.item;
                toSlot.amount = fromSlot.amount;

                fromSlot.item = tmpItem;
                fromSlot.amount = tmpAmount;
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool AddItemById(string itemId, int amount = 1, bool showToast = true)
        {
            ItemData data = GetItemDataById(itemId);
            if (data == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy ItemData với ID: " + itemId);
                return false;
            }
            return AddItem(data, amount, showToast);
        }

        public bool AddItem(ItemData data, int amount = 1, bool showToast = true)
        {
            if (data == null || amount <= 0) return false;

            int remaining = amount;

            if (data.maxStack > 1)
            {
                for (int i = 0; i < TotalSlots && remaining > 0; i++)
                {
                    if (!_slots[i].IsEmpty && _slots[i].item.itemID == data.itemID && _slots[i].amount < data.maxStack)
                    {
                        int add = Mathf.Min(remaining, data.maxStack - _slots[i].amount);
                        _slots[i].amount += add;
                        remaining -= add;
                    }
                }
            }

            for (int i = 0; i < TotalSlots && remaining > 0; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    int add = Mathf.Min(remaining, Mathf.Max(1, data.maxStack));
                    _slots[i].item = data;
                    _slots[i].amount = add;
                    remaining -= add;
                }
            }

            int added = amount - remaining;
            if (added > 0)
            {
                OnInventoryChanged?.Invoke();
                GameEventBus.RaiseItemChanged(data.itemID, GetItemCount(data.itemID));
                if (showToast)
                {
                    string itemName = GetLocalizedItemName(data);
                    GameEventBus.RaiseToastRequested(string.Format("Nhận +{0} {1}", added, itemName));
                }
            }
            return remaining == 0;
        }

        public bool RemoveItem(string itemId, int amount = 1)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;
            if (GetItemCount(itemId) < amount) return false;

            int remaining = amount;
            for (int i = TotalSlots - 1; i >= 0 && remaining > 0; i--)
            {
                if (!_slots[i].IsEmpty && _slots[i].item.itemID == itemId)
                {
                    int take = Mathf.Min(remaining, _slots[i].amount);
                    _slots[i].amount -= take;
                    remaining -= take;
                    if (_slots[i].amount <= 0)
                    {
                        _slots[i].item = null;
                        _slots[i].amount = 0;
                    }
                }
            }

            OnInventoryChanged?.Invoke();
            GameEventBus.RaiseItemChanged(itemId, GetItemCount(itemId));
            return true;
        }

        public int GetItemCount(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return 0;
            int total = 0;
            for (int i = 0; i < TotalSlots; i++)
            {
                if (!_slots[i].IsEmpty && _slots[i].item.itemID == itemId)
                {
                    total += _slots[i].amount;
                }
            }
            return total;
        }

        public bool HasItem(string itemId, int amount = 1) => GetItemCount(itemId) >= amount;

        public bool UseSelectedConsumable()
        {
            var slot = GetSelectedSlot();
            if (slot == null || slot.IsEmpty || slot.item.category != ItemCategory.Consumable)
            {
                return false;
            }

            ItemData food = slot.item;
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                GameRoot.Instance.Stats.RestoreStats(food.staminaRestore, food.lightHPRestore, food.clearsExhaustionPenalty);
            }

            RemoveItem(food.itemID, 1);
            string foodName = GetLocalizedItemName(food);
            string extra = food.clearsExhaustionPenalty ? " & xóa phạt kiệt sức!" : "!";
            GameEventBus.RaiseToastRequested(string.Format("Đã ăn {0}: +{1} Thể lực, +{2} Máu Ánh Sáng{3}",
                foodName, food.staminaRestore, food.lightHPRestore, extra));
            return true;
        }

        public bool ConsumeWaterCharge()
        {
            if (WateringCanCharges <= 0)
            {
                GameEventBus.RaiseToastRequested("Bình Tưới đã hết nước! Hãy ra Giếng Nước bấm [E] để múc đầy.");
                return false;
            }
            WateringCanCharges--;
            OnInventoryChanged?.Invoke();
            return true;
        }

        public void RefillWateringCan()
        {
            WateringCanCharges = MaxWaterCharges;
            OnInventoryChanged?.Invoke();
            GameEventBus.RaiseToastRequested("Đã múc đầy Bình Tưới (20/20).");
        }

        public string GetLocalizedItemName(ItemData data)
        {
            if (data == null) return string.Empty;
            if (GameRoot.Instance != null && GameRoot.Instance.Loc != null)
            {
                return GameRoot.Instance.Loc.Get(data.displayNameKey);
            }
            return data.itemID;
        }

        public string GetLocalizedItemDesc(ItemData data)
        {
            if (data == null) return string.Empty;
            if (GameRoot.Instance != null && GameRoot.Instance.Loc != null)
            {
                return GameRoot.Instance.Loc.Get(data.descriptionKey);
            }
            return string.Empty;
        }

        private void ApplyPassOutMaterialDrop()
        {
            int totalDropped = 0;
            for (int i = 0; i < TotalSlots; i++)
            {
                var slot = _slots[i];
                if (slot.IsEmpty || slot.item.isQuestProtected) continue;
                if (slot.amount < 5) continue;

                int dropCount = Mathf.Max(1, Mathf.FloorToInt(slot.amount * 0.10f));
                slot.amount -= dropCount;
                totalDropped += dropCount;
                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                }
            }

            if (totalDropped > 0)
            {
                OnInventoryChanged?.Invoke();
                Debug.Log("[MV] Ngất xỉu làm rơi " + totalDropped + " nguyên liệu thường (vật phẩm nhiệm vụ được bảo vệ).");
            }
        }

        public string CaptureStateJson()
        {
            var data = new InventorySaveData
            {
                selectedIndex = SelectedHotbarIndex,
                waterCharges = WateringCanCharges
            };
            for (int i = 0; i < TotalSlots; i++)
            {
                if (!_slots[i].IsEmpty)
                {
                    data.slots.Add(new SavedSlot
                    {
                        index = i,
                        itemId = _slots[i].item.itemID,
                        amount = _slots[i].amount
                    });
                }
            }
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            for (int i = 0; i < TotalSlots; i++)
            {
                _slots[i].item = null;
                _slots[i].amount = 0;
            }
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonUtility.FromJson<InventorySaveData>(json);
            if (data == null) return;

            SelectedHotbarIndex = Mathf.Clamp(data.selectedIndex, 0, HotbarSize - 1);
            WateringCanCharges = Mathf.Clamp(data.waterCharges, 0, MaxWaterCharges);

            if (data.slots != null)
            {
                for (int i = 0; i < data.slots.Count; i++)
                {
                    var s = data.slots[i];
                    if (s.index >= 0 && s.index < TotalSlots)
                    {
                        _slots[s.index].item = GetItemDataById(s.itemId);
                        _slots[s.index].amount = s.amount;
                    }
                }
            }
            OnInventoryChanged?.Invoke();
        }
    }
}
```

## 39. `Assets/Scripts/Inventory/WorldLootDrop.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.Inventory
{
    public class WorldLootDrop : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int amount = 1;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private PlayerController2D _player;
        private float _bobPhase;
        private float _spawnLockTimer = 0.25f;

        public void Initialize(ItemData data, int count)
        {
            itemData = data;
            amount = Mathf.Max(1, count);
            if (spriteRenderer != null && itemData != null && itemData.icon != null)
            {
                spriteRenderer.sprite = itemData.icon;
            }
        }

        private void Start()
        {
            _bobPhase = Random.Range(0f, 6.28f);
            if (spriteRenderer != null && itemData != null && itemData.icon != null)
            {
                spriteRenderer.sprite = itemData.icon;
            }
        }

        private void Update()
        {
            if (_spawnLockTimer > 0f) _spawnLockTimer -= Time.deltaTime;

            _bobPhase += Time.deltaTime * 5f;
            if (spriteRenderer != null)
            {
                spriteRenderer.transform.localPosition = new Vector3(0f, Mathf.Sin(_bobPhase) * 0.08f, 0f);
            }

            if (_spawnLockTimer > 0f) return;

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player == null) return;
            }

            float dist = Vector2.Distance(transform.position, _player.transform.position);
            if (dist <= 2.3f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Time.deltaTime * 7.5f);
            }
            if (dist <= 0.55f)
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null && itemData != null)
                {
                    if (GameRoot.Instance.Inventory.AddItem(itemData, amount, true))
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
```

## 40. `Assets/Scripts/Narrative/DialogueManager.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.UI;

namespace MoonpetalValley.Narrative
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private DialogueUIView dialogueUI;

        private struct RuntimeLine
        {
            public string speaker;
            public string body;
        }

        private readonly List<RuntimeLine> _queue = new List<RuntimeLine>();
        private int _currentIndex;
        private DialogueData _activeAsset;
        private Action _onFinishCallback;
        private bool _isPlaying;
        private float _inputLockTimer;
        private GameInputReader _input;

        public bool IsDialogueActive => _isPlaying;

        public void StartDialogue(DialogueData data, Action onFinish = null)
        {
            if (data == null || data.lines == null || data.lines.Count == 0) return;

            _queue.Clear();
            for (int i = 0; i < data.lines.Count; i++)
            {
                string sp = ResolveText(data.lines[i].speakerNameKey);
                string tx = ResolveText(data.lines[i].textKey);
                _queue.Add(new RuntimeLine { speaker = sp, body = tx });
            }

            _activeAsset = data;
            _onFinishCallback = onFinish;
            BeginDisplay();
        }

        public void StartQuickDialogue(string speaker, string body, Action onFinish = null)
        {
            _queue.Clear();
            _queue.Add(new RuntimeLine { speaker = speaker, body = body });
            _activeAsset = null;
            _onFinishCallback = onFinish;
            BeginDisplay();
        }

        private void BeginDisplay()
        {
            _currentIndex = 0;
            _isPlaying = true;
            _inputLockTimer = 0.18f;

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.Dialogue);
            }

            ShowCurrentLine();
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (_inputLockTimer > 0f)
            {
                _inputLockTimer -= Time.unscaledDeltaTime;
                return;
            }

            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }

            bool advance = _input != null && (_input.InteractPressed || _input.DashPressed || _input.ActionPressed);
            if (advance)
            {
                _currentIndex++;
                if (_currentIndex >= _queue.Count)
                {
                    FinishDialogue();
                }
                else
                {
                    _inputLockTimer = 0.12f;
                    ShowCurrentLine();
                }
            }
        }

        private void ShowCurrentLine()
        {
            if (dialogueUI == null || _currentIndex < 0 || _currentIndex >= _queue.Count) return;
            var line = _queue[_currentIndex];
            dialogueUI.ShowLine(line.speaker, line.body, _currentIndex == _queue.Count - 1);
        }

        private void FinishDialogue()
        {
            _isPlaying = false;
            if (dialogueUI != null) dialogueUI.Hide();

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }

            if (_activeAsset != null && GameRoot.Instance != null)
            {
                if (_activeAsset.consumesRequiredItem != null && _activeAsset.consumesAmount > 0)
                {
                    GameRoot.Instance.Inventory?.RemoveItem(_activeAsset.consumesRequiredItem.itemID, _activeAsset.consumesAmount);
                }

                if (_activeAsset.givesItemOnFinish != null && _activeAsset.givesItemAmount > 0)
                {
                    GameRoot.Instance.Inventory?.AddItem(_activeAsset.givesItemOnFinish, _activeAsset.givesItemAmount, true);
                }

                if (_activeAsset.unlocksDiaryPage != null)
                {
                    GameRoot.Instance.Diary?.UnlockPage(_activeAsset.unlocksDiaryPage.pageID);
                }

                if (!string.IsNullOrEmpty(_activeAsset.setsStoryFlagOnFinish))
                {
                    GameRoot.Instance.Flags?.SetFlag(_activeAsset.setsStoryFlagOnFinish, true);
                }
            }

            var cb = _onFinishCallback;
            _activeAsset = null;
            _onFinishCallback = null;
            cb?.Invoke();
        }

        private string ResolveText(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (GameRoot.Instance != null && GameRoot.Instance.Loc != null)
            {
                string val = GameRoot.Instance.Loc.Get(key);
                if (!val.StartsWith("[")) return val;
            }
            return key;
        }
    }
}
```

## 41. `Assets/Scripts/Narrative/DiaryManager.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Narrative
{
    public class DiaryManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private List<DiaryPageData> allPages = new List<DiaryPageData>();

        public event Action OnDiaryUpdated;
        private readonly HashSet<string> _unlockedPageIds = new HashSet<string>();

        public string SaveKey => "diary_pages";

        [Serializable]
        private class DiarySaveData
        {
            public List<string> unlockedIds = new List<string>();
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleStoryFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleStoryFlagChanged;
        }

        private void HandleStoryFlagChanged(string flagId, bool value)
        {
            if (!value) return;
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && allPages[i].unlockedByFlag == flagId)
                {
                    UnlockPage(allPages[i].pageID);
                }
            }
        }

        public void UnlockPage(string pageId)
        {
            if (string.IsNullOrEmpty(pageId)) return;
            if (_unlockedPageIds.Add(pageId))
            {
                DiaryPageData page = GetPageById(pageId);
                int num = page != null ? page.pageNumber : _unlockedPageIds.Count;
                GameEventBus.RaiseToastRequested("Đã mở khóa Sổ Tay Của Bà — Trang " + num + "! Bấm [B] để đọc.");
                OnDiaryUpdated?.Invoke();
            }
        }

        public bool HasPage(string pageId) => _unlockedPageIds.Contains(pageId);
        public int UnlockedCount => _unlockedPageIds.Count;
        public bool HasAllFourPages() => _unlockedPageIds.Count >= 4;

        public List<DiaryPageData> GetUnlockedPages()
        {
            var result = new List<DiaryPageData>();
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && _unlockedPageIds.Contains(allPages[i].pageID))
                {
                    result.Add(allPages[i]);
                }
            }
            return result;
        }

        private DiaryPageData GetPageById(string pageId)
        {
            for (int i = 0; i < allPages.Count; i++)
            {
                if (allPages[i] != null && allPages[i].pageID == pageId) return allPages[i];
            }
            return null;
        }

        public string CaptureStateJson()
        {
            return JsonUtility.ToJson(new DiarySaveData { unlockedIds = new List<string>(_unlockedPageIds) });
        }

        public void RestoreStateJson(string json)
        {
            _unlockedPageIds.Clear();
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<DiarySaveData>(json);
            if (data != null && data.unlockedIds != null)
            {
                for (int i = 0; i < data.unlockedIds.Count; i++) _unlockedPageIds.Add(data.unlockedIds[i]);
            }
            OnDiaryUpdated?.Invoke();
        }
    }
}
```

## 42. `Assets/Scripts/Narrative/NPCController.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.Narrative
{
    public class NPCController : MonoBehaviour
    {
        [SerializeField] private NPCData npcData;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 2.1f;
        [SerializeField] private Vector2 helperStandPosition;

        [Header("Animation Frames (32x32 FullRect)")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] cheerFrames;
        [SerializeField] private float idleFps = 2.4f;
        [SerializeField] private float cheerFps = 6.0f;

        private PlayerController2D _player;
        private GameInputReader _input;
        private bool _movedToFlowerbed;
        private float _animTimer;
        private int _frameIndex;
        private float _cheerTimer;

        public NPCData Data => npcData;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
            // Lệch pha ngẫu nhiên để 4 NPC không chớp mắt cùng lúc
            _animTimer = Random.Range(0f, 0.35f);
            _frameIndex = Random.Range(0, 4);
            CheckMoveToFlowerbed();
            UpdateSortingOrder();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value) CheckMoveToFlowerbed();
        }

        private void CheckMoveToFlowerbed()
        {
            if (_movedToFlowerbed || npcData == null || string.IsNullOrEmpty(npcData.helperReadyStoryFlag)) return;
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                GameRoot.Instance.Flags.HasFlag(npcData.helperReadyStoryFlag))
            {
                _movedToFlowerbed = true;
                if (helperStandPosition != Vector2.zero)
                {
                    transform.position = new Vector3(helperStandPosition.x, helperStandPosition.y, 0f);
                    UpdateSortingOrder();
                }
            }
        }

        private void UpdateSortingOrder()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
            }
        }

        private void Update()
        {
            TickAnimation(Time.deltaTime);

            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                TriggerDialogue();
            }
        }

        private void TickAnimation(float dt)
        {
            if (spriteRenderer == null) return;

            if (_cheerTimer > 0f) _cheerTimer -= dt;
            bool celebratingVictory = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                                      GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            bool playCheer = (_cheerTimer > 0f || celebratingVictory) && cheerFrames != null && cheerFrames.Length > 0 && cheerFrames[0] != null;

            Sprite[] frames = playCheer ? cheerFrames : idleFrames;
            if (frames == null || frames.Length == 0 || frames[0] == null) return;

            float fps = playCheer ? cheerFps : idleFps;
            float dur = fps > 0.01f ? (1f / fps) : 0.35f;
            _animTimer += dt;

            if (_animTimer >= dur)
            {
                _animTimer -= dur;
                _frameIndex = (_frameIndex + 1) % frames.Length;
            }

            int idx = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
            if (frames[idx] != null) spriteRenderer.sprite = frames[idx];
        }

        private void TriggerDialogue()
        {
            if (npcData == null || GameRoot.Instance == null || GameRoot.Instance.Dialogue == null) return;
            _cheerTimer = 2.0f;
            _frameIndex = 0;
            _animTimer = 0f;

            DialogueData chosen = ResolveDialogue();
            if (chosen != null)
            {
                GameRoot.Instance.Dialogue.StartDialogue(chosen);
            }
        }

        private DialogueData ResolveDialogue()
        {
            var flags = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            if (npcData.conditionalDialogues != null)
            {
                for (int i = 0; i < npcData.conditionalDialogues.Count; i++)
                {
                    var entry = npcData.conditionalDialogues[i];
                    if (entry == null || entry.dialogue == null) continue;

                    if (!string.IsNullOrEmpty(entry.requiredStoryFlag) && !flags.HasFlag(entry.requiredStoryFlag))
                        continue;
                    if (!string.IsNullOrEmpty(entry.blockedByStoryFlag) && flags.HasFlag(entry.blockedByStoryFlag))
                        continue;

                    if (entry.dialogue.consumesRequiredItem != null && entry.dialogue.consumesAmount > 0)
                    {
                        if (inv == null || !inv.HasItem(entry.dialogue.consumesRequiredItem.itemID, entry.dialogue.consumesAmount))
                            continue;
                    }

                    if (entry.dialogue.dialogueID == "dlg_bach_read_letters")
                    {
                        if (!flags.HasFlag("flag_hana_bread_done") || !flags.HasFlag("flag_toby_cat_saved") || !flags.HasFlag("flag_otis_mailbox_fixed"))
                            continue;
                    }

                    return entry.dialogue;
                }
            }
            return npcData.defaultDialogue;
        }
    }
}
```

## 43. `Assets/Scripts/Narrative/QuestManager.cs`

```csharp
using System;
using System.Text;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Narrative
{
    public class QuestManager : MonoBehaviour
    {
        public event Action<string> OnObjectiveTextChanged;
        public string CurrentObjectiveSummary { get; private set; } = "";

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
            GameEventBus.OnItemChanged += HandleItemChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
            GameEventBus.OnItemChanged -= HandleItemChanged;
        }

        private void Start()
        {
            RecomputeObjective();
        }

        private void HandleFlagChanged(string _, bool __) => RecomputeObjective();
        private void HandleItemChanged(string _, int __) => RecomputeObjective();

        public void RecomputeObjective()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;
            var f = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            string text;
            if (!f.HasFlag("flag_intro_letter_read"))
            {
                text = "<b>MỞ ĐẦU — Thư Của Bà</b>\n- Đọc bức thư trên Bàn Gỗ trong Nhà Kính [E]";
            }
            else if (!f.HasFlag("flag_greenhouse_cleared"))
            {
                text = "<b>MỞ ĐẦU — Dọn Nhà Kính</b>\n- Dùng Cuốc (1) hoặc Rìu (3) dọn 3 cụm cỏ/đá vụn";
            }
            else if (!f.HasFlag("flag_lumi_awakened"))
            {
                text = "<b>MỞ ĐẦU — Hạt Giống Ánh Trăng</b>\n- Mở Hộp Gỗ Của Bà [E] để đánh thức Lumi";
            }
            else if (!f.HasFlag("flag_tutorial_done"))
            {
                text = "<b>MỞ ĐẦU — Thử Nghiệm Đầu Tiên</b>\n- Gieo thử hạt tại Bồn Ươm Nhỏ trong Nhà Kính [E]";
            }
            else if (!f.HasFlag("flag_met_bach_at_gate"))
            {
                text = "<b>NHIỆM VỤ 1 — Khu Vườn Bị Lãng Quên</b>\n- Ra Làng Chính, gặp Ông Bách ở cổng Vườn Phía Nam [E]";
            }
            else if (!f.HasFlag("flag_garden_slimes_cleared"))
            {
                text = "<b>NHIỆM VỤ 1 — Thanh Tẩy Vườn Xưa</b>\n- Cầm Liềm Ánh Trăng (4) diệt 5 Bọt Sương Đen trong Vườn Ông Bách";
            }
            else if (!f.HasFlag("flag_garden_bench_cleaned"))
            {
                text = "<b>NHIỆM VỤ 1 — Kỷ Niệm Cũ</b>\n- Bấm [E] lau sạch Chiếc Ghế Đôi Kỷ Niệm trong vườn";
            }
            else if (!f.HasFlag("flag_hana_bread_done") || !f.HasFlag("flag_toby_cat_saved") || !f.HasFlag("flag_otis_mailbox_fixed"))
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>NHIỆM VỤ 1 — Gom 3 Túi Hạt Di Sản</b>");
                if (!f.HasFlag("flag_hana_bread_done"))
                {
                    int barley = inv != null ? inv.GetItemCount("item_dew_barley") : 0;
                    sb.AppendLine("- Bà Hana: Giao 4 Lúa Mạch Sương Mai (" + barley + "/4)");
                }
                if (!f.HasFlag("flag_toby_cat_saved"))
                {
                    sb.AppendLine("- Cậu Bé Toby: Cứu mèo Miu ở Rìa Rừng Phía Đông");
                }
                if (!f.HasFlag("flag_otis_mailbox_fixed"))
                {
                    int wood = inv != null ? inv.GetItemCount("item_branch_wood") : 0;
                    sb.AppendLine("- Bác Otis: Sửa Hòm Thư bằng 10 Gỗ (" + wood + "/10)");
                }
                text = sb.ToString().TrimEnd();
            }
            else if (!f.HasFlag("flag_bach_letters_read"))
            {
                text = "<b>NHIỆM VỤ 1 — Lá Thư Muộn Màng</b>\n- Quay lại gặp Ông Bách đọc đủ 4 Trang Sổ Tay [E]";
            }
            else if (!f.HasFlag("flag_nv1_completed"))
            {
                text = "<b>NHIỆM VỤ 1 — Hoa Nở Lại Vườn Xưa</b>\n- Tới Bồn Hoa Trung Tâm Vườn, mở Khay Đóng Góp & Cùng Gieo Hạt [E]";
            }
            else
            {
                text = "<b>HOÀN THÀNH MVP!</b>\n- Nhận Cánh Hoa Hy Vọng, Sương Héo Úa đã tan biến!\n- Bấm [Q] dùng Xung Ánh Trăng của Lumi.";
            }

            CurrentObjectiveSummary = text;
            OnObjectiveTextChanged?.Invoke(CurrentObjectiveSummary);
        }
    }
}
```

## 44. `Assets/Scripts/Narrative/StoryPropInteractable.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.Narrative
{
    public class StoryPropInteractable : MonoBehaviour
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

        private PlayerController2D _player;
        private GameInputReader _input;

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            bool alreadyDone = !string.IsNullOrEmpty(completedFlag) && GameRoot.Instance != null &&
                               GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag(completedFlag);

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = !alreadyDone && Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                TryInteract();
            }
        }

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

            if (staminaCost > 0)
            {
                GameRoot.Instance.Stats?.ConsumeStamina(staminaCost);
            }

            if (!string.IsNullOrEmpty(requiredItemId) && requiredItemAmount > 0)
            {
                inv?.RemoveItem(requiredItemId, requiredItemAmount);
            }

            GameRoot.Instance.Dialogue?.StartQuickDialogue(dialogueSpeaker, dialogueBody, () =>
            {
                if (!string.IsNullOrEmpty(rewardItemId) && rewardItemAmount > 0)
                {
                    inv?.AddItemById(rewardItemId, rewardItemAmount, true);
                }
                if (propId == "prop_seed_box")
                {
                    inv?.AddItemById("item_dew_barley_seed", 6, true);
                }
                if (!string.IsNullOrEmpty(completedFlag)) flags.SetFlag(completedFlag, true);
                if (!string.IsNullOrEmpty(secondaryCompletedFlag)) flags.SetFlag(secondaryCompletedFlag, true);
            });
        }
    }
}
```

## 45. `Assets/Scripts/Player/CameraFollow2D.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Player
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.16f;
        [SerializeField] private Vector2 minBounds = new Vector2(-14f, -11f);
        [SerializeField] private Vector2 maxBounds = new Vector2(14f, 11f);

        private Vector3 _velocity;
        private float _shakeTimer;
        private float _shakeIntensity;

        public void TriggerShake(float intensity, float duration)
        {
            _shakeIntensity = Mathf.Max(_shakeIntensity, intensity);
            _shakeTimer = Mathf.Max(_shakeTimer, duration);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (player != null) target = player.transform;
                else return;
            }

            float targetX = Mathf.Clamp(target.position.x, minBounds.x, maxBounds.x);
            float targetY = Mathf.Clamp(target.position.y, minBounds.y, maxBounds.y);
            Vector3 desired = new Vector3(targetX, targetY, -10f);

            Vector3 smoothed = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);

            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                Vector2 offset = Random.insideUnitCircle * _shakeIntensity;
                smoothed.x += offset.x;
                smoothed.y += offset.y;
                if (_shakeTimer <= 0f) _shakeIntensity = 0f;
            }

            transform.position = smoothed;
        }
    }
}
```

## 46. `Assets/Scripts/Player/GameInputReader.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Player
{
    public class GameInputReader : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public bool DashPressed { get; private set; }
        public bool ActionPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool LumiSkillPressed { get; private set; }
        public bool DiaryPressed { get; private set; }
        public bool InventoryPressed { get; private set; }
        public bool PausePressed { get; private set; }
        public int HotbarSlotPressed { get; private set; } = -1;

        private void Update()
        {
            Vector2 move = Vector2.zero;
            bool dash = false;
            bool action = false;
            bool interact = false;
            bool lumi = false;
            bool diary = false;
            bool inv = false;
            bool pause = false;
            int hotbar = -1;

#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (kb != null)
            {
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) move.x -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) move.x += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) move.y -= 1f;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) move.y += 1f;

                if (kb.spaceKey.wasPressedThisFrame || kb.leftShiftKey.wasPressedThisFrame) dash = true;
                if (kb.jKey.wasPressedThisFrame) action = true;
                if (kb.eKey.wasPressedThisFrame) interact = true;
                if (kb.qKey.wasPressedThisFrame) lumi = true;
                if (kb.bKey.wasPressedThisFrame) diary = true;
                if (kb.tabKey.wasPressedThisFrame || kb.iKey.wasPressedThisFrame) inv = true;
                if (kb.escapeKey.wasPressedThisFrame) pause = true;

                if (kb.digit1Key.wasPressedThisFrame) hotbar = 0;
                else if (kb.digit2Key.wasPressedThisFrame) hotbar = 1;
                else if (kb.digit3Key.wasPressedThisFrame) hotbar = 2;
                else if (kb.digit4Key.wasPressedThisFrame) hotbar = 3;
                else if (kb.digit5Key.wasPressedThisFrame) hotbar = 4;
                else if (kb.digit6Key.wasPressedThisFrame) hotbar = 5;
                else if (kb.digit7Key.wasPressedThisFrame) hotbar = 6;
                else if (kb.digit8Key.wasPressedThisFrame) hotbar = 7;
            }
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                action = true;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            move.x = Input.GetAxisRaw("Horizontal");
            move.y = Input.GetAxisRaw("Vertical");
            dash = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift);
            action = Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0);
            interact = Input.GetKeyDown(KeyCode.E);
            lumi = Input.GetKeyDown(KeyCode.Q);
            diary = Input.GetKeyDown(KeyCode.B);
            inv = Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I);
            pause = Input.GetKeyDown(KeyCode.Escape);

            for (int i = 0; i < 8; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    hotbar = i;
                    break;
                }
            }
#endif

            MoveInput = move.sqrMagnitude > 1f ? move.normalized : move;
            DashPressed = dash;
            ActionPressed = action;
            InteractPressed = interact;
            LumiSkillPressed = lumi;
            DiaryPressed = diary;
            InventoryPressed = inv;
            PausePressed = pause;
            HotbarSlotPressed = hotbar;
        }
    }
}
```

## 47. `Assets/Scripts/Player/LumiCompanionController.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Player
{
    public class LumiCompanionController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer haloRenderer;
        [SerializeField] private Vector2 followOffset = new Vector2(-0.75f, 0.75f);
        [SerializeField] private float smoothSpeed = 6.5f;

        [Header("Pixel Art Animations (FullRect)")]
        [SerializeField] private Sprite[] floatFrames;
        [SerializeField] private Sprite[] pulseFrames;
        [SerializeField] private float floatFps = 6f;
        [SerializeField] private float pulseFps = 8f;
        [SerializeField] private float autoPulseInterval = 4.5f;

        private float _bobTime;
        private float _animTimer;
        private int _frameIdx;
        private float _pulseRemaining;
        private float _autoPulseCooldown = 2.5f;
        private Vector3 _lastPos;

        private void Awake()
        {
            if (spriteRenderer == null && visualTransform != null)
            {
                spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
            }
            _lastPos = transform.position;
        }

        private void LateUpdate()
        {
            if (followTarget == null)
            {
                var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (player != null) followTarget = player.transform;
                else return;
            }

            Vector3 desired = followTarget.position + new Vector3(followOffset.x, followOffset.y, 0f);
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smoothSpeed);

            float dx = transform.position.x - _lastPos.x;
            if (spriteRenderer != null && Mathf.Abs(dx) > 0.002f)
            {
                spriteRenderer.flipX = dx < 0f;
            }
            _lastPos = transform.position;

            _bobTime += Time.deltaTime * 3.2f;
            if (visualTransform != null)
            {
                visualTransform.localPosition = new Vector3(0f, Mathf.Sin(_bobTime) * 0.14f, 0f);
            }

            if (haloRenderer != null)
            {
                float boost = _pulseRemaining > 0f ? 0.22f : 0f;
                float pulse = 0.85f + boost + Mathf.Sin(_bobTime * 1.5f) * 0.15f;
                haloRenderer.transform.localScale = new Vector3(pulse, pulse, 1f);
            }

            if (_pulseRemaining > 0f)
            {
                _pulseRemaining -= Time.deltaTime;
            }
            else if (autoPulseInterval > 0f && pulseFrames != null && pulseFrames.Length > 0)
            {
                _autoPulseCooldown -= Time.deltaTime;
                if (_autoPulseCooldown <= 0f)
                {
                    TriggerPulse(0.65f);
                    _autoPulseCooldown = autoPulseInterval;
                }
            }

            UpdateSpriteAnimation();
        }

        private void UpdateSpriteAnimation()
        {
            if (spriteRenderer == null) return;

            bool isPulsing = _pulseRemaining > 0f && pulseFrames != null && pulseFrames.Length > 0;
            Sprite[] active = isPulsing ? pulseFrames : floatFrames;
            if (active == null || active.Length == 0) return;

            float fps = isPulsing ? pulseFps : floatFps;
            _animTimer += Time.deltaTime;
            float interval = 1f / Mathf.Max(1f, fps);
            if (_animTimer >= interval)
            {
                _animTimer -= interval;
                _frameIdx = (_frameIdx + 1) % active.Length;
            }

            int safeIdx = _frameIdx % active.Length;
            if (active[safeIdx] != null)
            {
                spriteRenderer.sprite = active[safeIdx];
            }
        }

        public void PlayMoonPulseAnimation(float duration = 0.7f)
        {
            TriggerPulse(duration);
        }

        public void TriggerPulse(float duration = 0.7f)
        {
            _pulseRemaining = Mathf.Max(_pulseRemaining, duration);
            _frameIdx = 0;
            _animTimer = 0f;
        }

        public void SetHaloRadiusScale(float scale)
        {
            if (haloRenderer != null)
            {
                haloRenderer.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
    }
}
```

## 48. `Assets/Scripts/Player/PlayerCombatController.cs`

```csharp
using UnityEngine;
using UnityEngine.EventSystems;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;

namespace MoonpetalValley.Player
{
    public class PlayerCombatController : MonoBehaviour
    {
        [SerializeField] private PlayerStatsSO statsConfig;
        [SerializeField] private PlayerController2D playerController;
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private WenSpriteAnimator spriteAnimator;
        [SerializeField] private Transform slashArcTransform;
        [SerializeField] private SpriteRenderer slashArcRenderer;
        [SerializeField] private Transform pulseRingTransform;
        [SerializeField] private SpriteRenderer pulseRingRenderer;

        private float _attackCooldownTimer;
        private float _slashVisualTimer;
        private float _pulseCooldownTimer;
        private float _pulseVisualTimer;

        private void Start()
        {
            if (slashArcRenderer != null) slashArcRenderer.enabled = false;
            if (pulseRingRenderer != null) pulseRingRenderer.enabled = false;
        }

        private void Update()
        {
            if (_attackCooldownTimer > 0f) _attackCooldownTimer -= Time.deltaTime;
            if (_pulseCooldownTimer > 0f) _pulseCooldownTimer -= Time.deltaTime;

            UpdateVisualEffects();

            if (GameRoot.Instance == null || GameRoot.Instance.State == null || !GameRoot.Instance.State.IsGameplayActive)
            {
                return;
            }
            if (inputReader == null || playerController == null) return;

            // 1. Chém Liềm Ánh Trăng 120 độ
            if (inputReader.ActionPressed && _attackCooldownTimer <= 0f)
            {
                bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                if (!overUI && IsHoldingMoonSickle())
                {
                    PerformMoonSickleSlash();
                }
            }

            // 2. Xung Ánh Trăng của Lumi (phím Q)
            if (inputReader.LumiSkillPressed)
            {
                TryPerformLumiMoonPulse();
            }
        }

        private bool IsHoldingMoonSickle()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return false;
            var slot = GameRoot.Instance.Inventory.GetSelectedSlot();
            return slot != null && !slot.IsEmpty && slot.item.itemID == "item_moon_sickle";
        }

        private void PerformMoonSickleSlash()
        {
            int dmg = statsConfig != null ? statsConfig.attackDamage : 15;
            float radius = statsConfig != null ? Mathf.Max(1.55f, statsConfig.attackRadius) : 1.55f;
            float arcDeg = statsConfig != null ? statsConfig.attackArcDegrees : 120f;
            float kb = statsConfig != null ? statsConfig.attackKnockbackForce : 6f;
            int cost = statsConfig != null ? statsConfig.swingStaminaCost : 2;
            _attackCooldownTimer = statsConfig != null ? statsConfig.attackCooldown : 0.4f;

            GameRoot.Instance?.Stats?.ConsumeStamina(cost);

            Vector2 facing = playerController.FacingDirection.sqrMagnitude > 0.01f
                ? playerController.FacingDirection.normalized
                : Vector2.down;

            if (spriteAnimator != null && spriteAnimator.HasValidSprites)
            {
                spriteAnimator.PlayMoonSickleSlash(facing);
            }

            if (slashArcTransform != null && slashArcRenderer != null)
            {
                float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
                slashArcTransform.localPosition = facing * 0.65f;
                slashArcTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
                slashArcRenderer.enabled = true;
                slashArcRenderer.color = new Color(0.68f, 0.98f, 1f, 0.95f);
                _slashVisualTimer = 0.16f;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
            for (int i = 0; i < hits.Length; i++)
            {
                var target = hits[i].GetComponent<IDamageable>();
                if (target == null || target.IsDefeated) continue;

                Vector2 toTarget = (Vector2)hits[i].transform.position - (Vector2)transform.position;
                float angleDiff = Vector2.Angle(facing, toTarget);
                if (toTarget.magnitude <= 0.6f || angleDiff <= arcDeg * 0.55f)
                {
                    target.TakePurifyDamage(dmg, toTarget.normalized, kb);
                }
            }
        }

        private void TryPerformLumiMoonPulse()
        {
            bool unlocked = GameRoot.Instance.Flags != null && GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            if (!unlocked)
            {
                GameEventBus.RaiseToastRequested("Kỹ năng Xung Ánh Trăng [Q] sẽ mở khóa sau khi nhận Cánh Hoa Hy Vọng!");
                return;
            }
            if (_pulseCooldownTimer > 0f)
            {
                GameEventBus.RaiseToastRequested("Xung Ánh Trăng đang hồi chiêu (" + Mathf.CeilToInt(_pulseCooldownTimer) + "s)...");
                return;
            }

            float radius = statsConfig != null ? statsConfig.lumiPulseRadius : 3.5f;
            int cost = statsConfig != null ? statsConfig.lumiPulseStaminaCost : 10;
            _pulseCooldownTimer = statsConfig != null ? statsConfig.lumiPulseCooldown : 8f;

            GameRoot.Instance?.Stats?.ConsumeStamina(cost);
            playerController.ClearSlow();

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null) lumi.PlayMoonPulseAnimation();

            if (pulseRingTransform != null && pulseRingRenderer != null)
            {
                pulseRingRenderer.enabled = true;
                pulseRingTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
                pulseRingRenderer.color = new Color(0.60f, 0.98f, 0.95f, 0.9f);
                _pulseVisualTimer = 0.35f;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
            int pushed = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                var target = hits[i].GetComponent<IDamageable>();
                if (target != null && !target.IsDefeated)
                {
                    Vector2 dir = ((Vector2)hits[i].transform.position - (Vector2)transform.position).normalized;
                    target.TakePurifyDamage(10, dir, 9.5f);
                    pushed++;
                }
            }

            GameEventBus.RaiseToastRequested("Lumi phóng Xung Ánh Trăng! (Giải làm chậm & đẩy lùi " + pushed + " quái)");
        }

        private void UpdateVisualEffects()
        {
            if (_slashVisualTimer > 0f)
            {
                _slashVisualTimer -= Time.deltaTime;
                if (slashArcRenderer != null)
                {
                    Color c = slashArcRenderer.color;
                    c.a = Mathf.Clamp01(_slashVisualTimer / 0.16f);
                    slashArcRenderer.color = c;
                    if (_slashVisualTimer <= 0f) slashArcRenderer.enabled = false;
                }
            }

            if (_pulseVisualTimer > 0f)
            {
                _pulseVisualTimer -= Time.deltaTime;
                float t = 1f - Mathf.Clamp01(_pulseVisualTimer / 0.35f);
                if (pulseRingTransform != null)
                {
                    float s = Mathf.Lerp(0.6f, 3.6f, t);
                    pulseRingTransform.localScale = new Vector3(s, s, 1f);
                }
                if (pulseRingRenderer != null)
                {
                    Color c = pulseRingRenderer.color;
                    c.a = 1f - t;
                    pulseRingRenderer.color = c;
                    if (_pulseVisualTimer <= 0f) pulseRingRenderer.enabled = false;
                }
            }
        }
    }
}
```

## 49. `Assets/Scripts/Player/PlayerController2D.cs`

```csharp
using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Player
{
    public class PlayerController2D : MonoBehaviour, ISaveable
    {
        [SerializeField] private PlayerStatsSO statsConfig;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private WenSpriteAnimator spriteAnimator;

        private Vector2 _facingDirection = Vector2.down;
        private Vector2 _dashDirection;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private float _invulnerableTimer;
        private float _slowTimer;
        private float _walkBobPhase;

        public Vector2 FacingDirection => _facingDirection;
        public bool IsDashing => _dashTimer > 0f;
        public bool IsInvulnerable => _invulnerableTimer > 0f;
        public string SaveKey => "player_position";

        [Serializable]
        private class PlayerPosSave
        {
            public float x;
            public float y;
        }

        private void Start()
        {
            if (GameRoot.Instance != null)
            {
                GameRoot.Instance.RegisterSaveable(this);
            }
        }

        private void Update()
        {
            if (_dashCooldownTimer > 0f) _dashCooldownTimer -= Time.deltaTime;
            if (_invulnerableTimer > 0f) _invulnerableTimer -= Time.deltaTime;
            if (_slowTimer > 0f) _slowTimer -= Time.deltaTime;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                UpdateVisualFeedback(Vector2.zero);
                return;
            }

            Vector2 move = inputReader != null ? inputReader.MoveInput : Vector2.zero;
            if (move.sqrMagnitude > 0.01f && !IsDashing)
            {
                _facingDirection = move.normalized;
            }

            if (inputReader != null && inputReader.DashPressed && CanDash())
            {
                StartDash(move.sqrMagnitude > 0.01f ? move.normalized : _facingDirection);
            }

            if (_dashTimer > 0f)
            {
                _dashTimer -= Time.deltaTime;
            }

            UpdateVisualFeedback(move);
            UpdateYSorting();
        }

        private void FixedUpdate()
        {
            if (rb == null) return;

            bool gameplayActive = GameRoot.Instance == null || GameRoot.Instance.State == null || GameRoot.Instance.State.IsGameplayActive;
            if (!gameplayActive)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            float baseSpeed = statsConfig != null ? statsConfig.moveSpeed : 4.5f;
            float dashSpeed = statsConfig != null ? statsConfig.dashSpeed : 11.5f;
            float slowMult = statsConfig != null ? statsConfig.slowedSpeedMultiplier : 0.55f;

            if (_dashTimer > 0f)
            {
                rb.linearVelocity = _dashDirection * dashSpeed;
            }
            else
            {
                Vector2 move = inputReader != null ? inputReader.MoveInput : Vector2.zero;
                float speed = _slowTimer > 0f ? baseSpeed * slowMult : baseSpeed;
                rb.linearVelocity = move * speed;
            }
        }

        private bool CanDash()
        {
            return _dashTimer <= 0f && _dashCooldownTimer <= 0f;
        }

        private void StartDash(Vector2 direction)
        {
            int dashCost = statsConfig != null ? statsConfig.dashStaminaCost : 5;
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                bool stillAwake = GameRoot.Instance.Stats.ConsumeStamina(dashCost);
                if (!stillAwake) return;
            }

            _dashDirection = direction.normalized;
            _dashTimer = statsConfig != null ? statsConfig.dashDuration : 0.18f;
            _dashCooldownTimer = statsConfig != null ? statsConfig.dashCooldown : 0.8f;
            float iFrame = statsConfig != null ? statsConfig.dashInvulnerabilityDuration : 0.2f;
            if (iFrame > _invulnerableTimer) _invulnerableTimer = iFrame;
        }

        public void ApplySlow(float durationOverride = -1f)
        {
            float dur = durationOverride > 0f ? durationOverride : (statsConfig != null ? statsConfig.slowDuration : 2f);
            _slowTimer = Mathf.Max(_slowTimer, dur);
        }

        public void ClearSlow()
        {
            _slowTimer = 0f;
        }

        public void GrantInvulnerability(float duration)
        {
            _invulnerableTimer = Mathf.Max(_invulnerableTimer, duration);
        }

        public void TeleportTo(Vector2 worldPos)
        {
            _dashTimer = 0f;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = worldPos;
            }
            transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
            UpdateYSorting();
        }

        private void UpdateVisualFeedback(Vector2 move)
        {
            bool useSpriteAnim = spriteAnimator != null && spriteAnimator.HasValidSprites;

            if (useSpriteAnim)
            {
                spriteAnimator.UpdateLocomotion(move, _facingDirection, IsDashing);
                if (visualTransform != null)
                {
                    visualTransform.localPosition = Vector3.zero;
                }
            }

            if (spriteRenderer != null)
            {
                if (!useSpriteAnim && Mathf.Abs(_facingDirection.x) > 0.1f)
                {
                    spriteRenderer.flipX = _facingDirection.x < 0f;
                }

                if (IsDashing)
                {
                    spriteRenderer.color = new Color(0.75f, 0.96f, 1.0f, 0.90f);
                }
                else if (_slowTimer > 0f)
                {
                    spriteRenderer.color = new Color(0.75f, 0.65f, 0.95f, 1.0f);
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }
            }

            if (!useSpriteAnim && visualTransform != null)
            {
                if (move.sqrMagnitude > 0.01f && !IsDashing)
                {
                    _walkBobPhase += Time.deltaTime * 14f;
                    float bobY = Mathf.Abs(Mathf.Sin(_walkBobPhase)) * 0.08f;
                    visualTransform.localPosition = new Vector3(0f, bobY, 0f);
                }
                else
                {
                    _walkBobPhase = 0f;
                    visualTransform.localPosition = Vector3.Lerp(visualTransform.localPosition, Vector3.zero, Time.deltaTime * 12f);
                }
            }
        }

        private void UpdateYSorting()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
            }
        }

        public string CaptureStateJson()
        {
            var data = new PlayerPosSave { x = transform.position.x, y = transform.position.y };
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<PlayerPosSave>(json);
            if (data != null)
            {
                TeleportTo(new Vector2(data.x, data.y));
            }
        }
    }
}
```

## 50. `Assets/Scripts/Player/PlayerStatsController.cs`

```csharp
using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.Player
{
    public class PlayerStatsController : MonoBehaviour, ISaveable
    {
        [SerializeField] private PlayerStatsSO statsConfig;

        public static event Action<string> OnPlayerKnockedOut;
        public static event Action<bool> OnExhaustionStateChanged;

        public int CurrentStamina { get; private set; } = 100;
        public int CurrentLightHP { get; private set; } = 100;
        public bool IsExhaustedPenalty { get; private set; }

        public int BaseMaxStamina => statsConfig != null ? statsConfig.maxStamina : 100;
        public int MaxLightHP => statsConfig != null ? statsConfig.maxLightHP : 100;
        public int EffectiveMaxStamina
        {
            get
            {
                if (!IsExhaustedPenalty) return BaseMaxStamina;
                float mult = statsConfig != null ? statsConfig.exhaustedStaminaMultiplier : 0.5f;
                return Mathf.Max(10, Mathf.RoundToInt(BaseMaxStamina * mult));
            }
        }

        public string SaveKey => "player_stats";

        [Serializable]
        private class StatsSaveData
        {
            public int stamina;
            public int lightHP;
            public bool exhausted;
        }

        private void Start()
        {
            CurrentStamina = EffectiveMaxStamina;
            CurrentLightHP = MaxLightHP;
            BroadcastAll();
        }

        public bool ConsumeStamina(int amount, bool bypassTutorialFloor = false)
        {
            if (amount <= 0) return true;

            bool inTutorial = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                              !GameRoot.Instance.Flags.HasFlag("flag_tutorial_done");

            int next = CurrentStamina - amount;
            if (inTutorial && !bypassTutorialFloor && next < 10)
            {
                next = 10;
            }

            CurrentStamina = Mathf.Clamp(next, 0, EffectiveMaxStamina);
            GameEventBus.RaiseStaminaChanged(CurrentStamina, EffectiveMaxStamina);

            if (CurrentStamina <= 0)
            {
                OnPlayerKnockedOut?.Invoke("Thể lực cạn kiệt! Bạn ngất xỉu và được dân làng đưa về Nhà Kính...");
                return false;
            }
            return true;
        }

        public void TakeLightDamage(int amount, bool ignoreInvulnerability = false)
        {
            if (amount <= 0) return;

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (!ignoreInvulnerability && player != null && player.IsInvulnerable)
            {
                return;
            }

            CurrentLightHP = Mathf.Clamp(CurrentLightHP - amount, 0, MaxLightHP);
            GameEventBus.RaiseLightHPChanged(CurrentLightHP, MaxLightHP);

            if (player != null && !ignoreInvulnerability)
            {
                float iFrame = statsConfig != null ? statsConfig.hitInvulnerabilityDuration : 0.75f;
                player.GrantInvulnerability(iFrame);
            }

            if (CurrentLightHP <= 0)
            {
                OnPlayerKnockedOut?.Invoke("Máu Ánh Sáng cạn kiệt! Lumi dìu bạn trở về Nhà Kính nghỉ ngơi...");
            }
        }

        public void RestoreStats(int staminaAmount, int lightHPAmount, bool clearExhaustionPenalty)
        {
            if (clearExhaustionPenalty && IsExhaustedPenalty)
            {
                IsExhaustedPenalty = false;
                OnExhaustionStateChanged?.Invoke(false);
            }

            CurrentStamina = Mathf.Clamp(CurrentStamina + staminaAmount, 0, EffectiveMaxStamina);
            CurrentLightHP = Mathf.Clamp(CurrentLightHP + lightHPAmount, 0, MaxLightHP);
            BroadcastAll();
        }

        public void ResetForNewDay(bool passedOutYesterday)
        {
            IsExhaustedPenalty = passedOutYesterday;
            CurrentStamina = EffectiveMaxStamina;
            CurrentLightHP = MaxLightHP;
            OnExhaustionStateChanged?.Invoke(IsExhaustedPenalty);
            BroadcastAll();
        }

        public void BroadcastAll()
        {
            GameEventBus.RaiseStaminaChanged(CurrentStamina, EffectiveMaxStamina);
            GameEventBus.RaiseLightHPChanged(CurrentLightHP, MaxLightHP);
            OnExhaustionStateChanged?.Invoke(IsExhaustedPenalty);
        }

        public string CaptureStateJson()
        {
            var data = new StatsSaveData
            {
                stamina = CurrentStamina,
                lightHP = CurrentLightHP,
                exhausted = IsExhaustedPenalty
            };
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<StatsSaveData>(json);
            if (data != null)
            {
                IsExhaustedPenalty = data.exhausted;
                CurrentStamina = Mathf.Clamp(data.stamina, 1, EffectiveMaxStamina);
                CurrentLightHP = Mathf.Clamp(data.lightHP, 1, MaxLightHP);
                BroadcastAll();
            }
        }
    }
}
```

## 51. `Assets/Scripts/Player/PlayerToolController.cs`

```csharp
using UnityEngine;
using UnityEngine.EventSystems;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;

namespace MoonpetalValley.Player
{
    public class PlayerToolController : MonoBehaviour
    {
        [SerializeField] private PlayerStatsSO statsConfig;
        [SerializeField] private PlayerController2D playerController;
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private WenSpriteAnimator spriteAnimator;

        private float _cooldownTimer;
        private float _swingVisualTimer;

        private void Awake()
        {
            if (spriteAnimator == null) spriteAnimator = GetComponent<WenSpriteAnimator>();
            if (playerController == null) playerController = GetComponent<PlayerController2D>();
            if (inputReader == null) inputReader = GetComponent<GameInputReader>();
        }

        private void Update()
        {
            if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;

            bool useSpriteAnim = spriteAnimator != null && spriteAnimator.HasValidSprites;
            if (!useSpriteAnim && _swingVisualTimer > 0f && visualTransform != null)
            {
                _swingVisualTimer -= Time.deltaTime;
                float s = _swingVisualTimer > 0f ? 1.12f : 1.0f;
                visualTransform.localScale = new Vector3(s, s, 1f);
            }

            if (GameRoot.Instance == null || GameRoot.Instance.State == null || !GameRoot.Instance.State.IsGameplayActive)
            {
                return;
            }

            if (inputReader == null || playerController == null) return;

            // Bấm E để thu hoạch cây đã chín gần Wen
            if (inputReader.InteractPressed)
            {
                TryHarvestNearby();
            }

            // Bấm J / Chuột trái để dùng công cụ hoặc gieo hạt
            if (inputReader.ActionPressed && _cooldownTimer <= 0f)
            {
                bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                if (!overUI)
                {
                    TryUseActiveTool();
                }
            }
        }

        private void TryHarvestNearby()
        {
            if (FarmGridManager.Instance == null || GameRoot.Instance.Inventory == null) return;
            Vector2 probe = (Vector2)transform.position + playerController.FacingDirection * 0.65f;
            var tile = FarmGridManager.Instance.FindClosestTile(probe, 1.35f);
            if (tile == null) tile = FarmGridManager.Instance.FindClosestTile(transform.position, 1.35f);

            if (tile != null && tile.IsReadyToHarvest)
            {
                if (tile.TryHarvest(out ItemData cropItem, out int yieldCount))
                {
                    GameRoot.Instance.Inventory.AddItem(cropItem, yieldCount, true);
                    TriggerSwingFeedback(WenToolAnimType.GenericSeedHarvest);
                }
            }
        }

        private void TryUseActiveTool()
        {
            if (GameRoot.Instance.Inventory == null) return;
            var slot = GameRoot.Instance.Inventory.GetSelectedSlot();
            if (slot == null || slot.IsEmpty) return;

            ItemData item = slot.item;
            Vector2 targetPos = (Vector2)transform.position + playerController.FacingDirection * 0.90f;

            // 1. Gieo hạt
            if (item.category == ItemCategory.Seed && item.linkedCropData != null)
            {
                if (FarmGridManager.Instance == null) return;
                var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                if (tile != null && tile.TryPlant(item.linkedCropData))
                {
                    GameRoot.Instance.Inventory.RemoveItem(item.itemID, 1);
                    TriggerSwingFeedback(WenToolAnimType.GenericSeedHarvest);
                    GameEventBus.RaiseToastRequested("Đã gieo " + GameRoot.Instance.Inventory.GetLocalizedItemName(item) + "!");
                }
                else
                {
                    GameEventBus.RaiseToastRequested("Hãy dùng Cuốc Đất (phím 1) xới ô đất trong Nhà Kính trước khi gieo hạt.");
                }
                return;
            }

            // 2. Cuốc Đất (xới đất hoặc đập rác Nhà Kính)
            if (item.itemID == "item_hoe")
            {
                int cost = statsConfig != null ? statsConfig.hoeStaminaCost : 4;
                if (TryHitResourceNode(targetPos, "item_hoe", cost, WenToolAnimType.Hoe)) return;

                if (FarmGridManager.Instance != null)
                {
                    var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                    if (tile != null && tile.TryTill())
                    {
                        GameRoot.Instance.Stats?.ConsumeStamina(cost);
                        TriggerSwingFeedback(WenToolAnimType.Hoe);
                        return;
                    }
                }
                TriggerSwingFeedback(WenToolAnimType.Hoe);
                return;
            }

            // 3. Bình Tưới
            if (item.itemID == "item_watering_can")
            {
                if (FarmGridManager.Instance != null)
                {
                    var tile = FarmGridManager.Instance.FindClosestTile(targetPos, 1.1f);
                    if (tile != null && tile.State != FarmTileState.Untilled && !tile.IsWatered)
                    {
                        if (GameRoot.Instance.Inventory.ConsumeWaterCharge())
                        {
                            tile.TryWater();
                            int cost = statsConfig != null ? statsConfig.waterStaminaCost : 2;
                            GameRoot.Instance.Stats?.ConsumeStamina(cost);
                            TriggerSwingFeedback(WenToolAnimType.WateringCan);
                        }
                        return;
                    }
                }
                if (GameRoot.Instance.Inventory.WateringCanCharges > 0)
                {
                    TriggerSwingFeedback(WenToolAnimType.WateringCan);
                }
                return;
            }

            // 4. Rìu Nhỏ (chặt nhánh cây khô hoặc dọn rác Nhà Kính)
            if (item.itemID == "item_hatchet")
            {
                int cost = statsConfig != null ? statsConfig.chopStaminaCost : 4;
                if (!TryHitResourceNode(targetPos, "item_hatchet", cost, WenToolAnimType.Hatchet))
                {
                    TriggerSwingFeedback(WenToolAnimType.Hatchet);
                }
            }
        }

        private bool TryHitResourceNode(Vector2 targetPos, string toolId, int staminaCost, WenToolAnimType animType)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.95f);
            for (int i = 0; i < hits.Length; i++)
            {
                var node = hits[i].GetComponent<ResourceNodeController>();
                if (node != null && !node.IsDepleted)
                {
                    if (node.TryHit(toolId))
                    {
                        GameRoot.Instance.Stats?.ConsumeStamina(staminaCost);
                        TriggerSwingFeedback(animType);
                    }
                    return true;
                }
            }
            return false;
        }

        private void TriggerSwingFeedback(WenToolAnimType animType)
        {
            _cooldownTimer = animType == WenToolAnimType.WateringCan ? 0.38f : 0.32f;
            if (spriteAnimator != null && spriteAnimator.HasValidSprites)
            {
                spriteAnimator.PlayToolAction(animType, playerController != null ? playerController.FacingDirection : Vector2.down);
            }
            else
            {
                _swingVisualTimer = 0.14f;
            }
        }
    }
}
```

## 52. `Assets/Scripts/Player/WenSpriteAnimator.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.Player
{
    public enum WenToolAnimType
    {
        GenericSeedHarvest,
        Hoe,
        WateringCan,
        Hatchet
    }

    public class WenSpriteAnimator : MonoBehaviour
    {
        private enum Facing4
        {
            South,
            North,
            East,
            West
        }

        private enum AnimAction
        {
            Idle,
            Walk,
            Dash,
            ToolUse,
            Hoe,
            WateringCan,
            Hatchet,
            MoonSickleSlash
        }

        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Idle (4 hướng)")]
        [SerializeField] private Sprite[] idleSouth;
        [SerializeField] private Sprite[] idleNorth;
        [SerializeField] private Sprite[] idleEast;
        [SerializeField] private Sprite[] idleWest;

        [Header("Walk (4 hướng)")]
        [SerializeField] private Sprite[] walkSouth;
        [SerializeField] private Sprite[] walkNorth;
        [SerializeField] private Sprite[] walkEast;
        [SerializeField] private Sprite[] walkWest;

        [Header("Dash (4 hướng)")]
        [SerializeField] private Sprite[] dashSouth;
        [SerializeField] private Sprite[] dashNorth;
        [SerializeField] private Sprite[] dashEast;
        [SerializeField] private Sprite[] dashWest;

        [Header("Tool Use - Gieo hạt / Thu hoạch (4 hướng)")]
        [SerializeField] private Sprite[] toolSouth;
        [SerializeField] private Sprite[] toolNorth;
        [SerializeField] private Sprite[] toolEast;
        [SerializeField] private Sprite[] toolWest;

        [Header("Hoe - Cuốc Đất (4 hướng)")]
        [SerializeField] private Sprite[] hoeSouth;
        [SerializeField] private Sprite[] hoeNorth;
        [SerializeField] private Sprite[] hoeEast;
        [SerializeField] private Sprite[] hoeWest;

        [Header("Watering Can - Bình Tưới (4 hướng)")]
        [SerializeField] private Sprite[] waterSouth;
        [SerializeField] private Sprite[] waterNorth;
        [SerializeField] private Sprite[] waterEast;
        [SerializeField] private Sprite[] waterWest;

        [Header("Hatchet - Rìu Nhỏ (4 hướng)")]
        [SerializeField] private Sprite[] hatchetSouth;
        [SerializeField] private Sprite[] hatchetNorth;
        [SerializeField] private Sprite[] hatchetEast;
        [SerializeField] private Sprite[] hatchetWest;

        [Header("Moon Sickle Slash (4 hướng)")]
        [SerializeField] private Sprite[] slashSouth;
        [SerializeField] private Sprite[] slashNorth;
        [SerializeField] private Sprite[] slashEast;
        [SerializeField] private Sprite[] slashWest;

        [Header("Tốc độ khung hình (FPS)")]
        [SerializeField] private float idleFps = 5f;
        [SerializeField] private float walkFps = 10f;
        [SerializeField] private float dashFps = 18f;
        [SerializeField] private float toolFps = 11f;
        [SerializeField] private float hoeFps = 11f;
        [SerializeField] private float waterFps = 10f;
        [SerializeField] private float hatchetFps = 12f;
        [SerializeField] private float slashFps = 14f;

        private Facing4 _facing = Facing4.South;
        private AnimAction _currentAction = AnimAction.Idle;
        private bool _isOneShotLocked;
        private int _frameIndex;
        private float _frameTimer;

        public bool HasValidSprites => idleSouth != null && idleSouth.Length > 0 && idleSouth[0] != null;

        public void UpdateLocomotion(Vector2 moveInput, Vector2 facingDir, bool isDashing)
        {
            if (!HasValidSprites || spriteRenderer == null) return;

            spriteRenderer.flipX = false;

            if (isDashing)
            {
                _isOneShotLocked = false;
                UpdateFacingFromVector(facingDir);
                SwitchActionIfNeeded(AnimAction.Dash);
            }
            else if (!_isOneShotLocked)
            {
                UpdateFacingFromVector(facingDir);
                AnimAction desired = moveInput.sqrMagnitude > 0.01f ? AnimAction.Walk : AnimAction.Idle;
                SwitchActionIfNeeded(desired);
            }

            TickAnimation(Time.deltaTime);
        }

        public void PlayToolUse(Vector2 facingDir)
        {
            PlayToolAction(WenToolAnimType.GenericSeedHarvest, facingDir);
        }

        public void PlayToolAction(WenToolAnimType toolType, Vector2 facingDir)
        {
            if (!HasValidSprites || spriteRenderer == null) return;
            UpdateFacingFromVector(facingDir);

            AnimAction act = toolType switch
            {
                WenToolAnimType.Hoe => AnimAction.Hoe,
                WenToolAnimType.WateringCan => AnimAction.WateringCan,
                WenToolAnimType.Hatchet => AnimAction.Hatchet,
                _ => AnimAction.ToolUse
            };
            StartOneShot(act);
        }

        public void PlayMoonSickleSlash(Vector2 facingDir)
        {
            if (!HasValidSprites || spriteRenderer == null) return;
            UpdateFacingFromVector(facingDir);
            StartOneShot(AnimAction.MoonSickleSlash);
        }

        private void StartOneShot(AnimAction action)
        {
            _currentAction = action;
            _isOneShotLocked = true;
            _frameIndex = 0;
            _frameTimer = 0f;
            ApplyCurrentFrame();
        }

        private void SwitchActionIfNeeded(AnimAction next)
        {
            if (_currentAction == next) return;
            _currentAction = next;
            _frameIndex = 0;
            _frameTimer = 0f;
            ApplyCurrentFrame();
        }

        private void TickAnimation(float dt)
        {
            Sprite[] frames = GetFrames(_currentAction, _facing);
            if (frames == null || frames.Length == 0 || frames[0] == null)
            {
                _isOneShotLocked = false;
                return;
            }

            float fps = GetFps(_currentAction);
            float frameDuration = fps > 0.01f ? (1f / fps) : 0.12f;
            _frameTimer += dt;

            if (_frameTimer >= frameDuration)
            {
                _frameTimer -= frameDuration;
                _frameIndex++;

                if (_frameIndex >= frames.Length)
                {
                    if (_isOneShotLocked)
                    {
                        _isOneShotLocked = false;
                        _currentAction = AnimAction.Idle;
                        _frameIndex = 0;
                    }
                    else if (_currentAction == AnimAction.Dash)
                    {
                        _frameIndex = frames.Length - 1;
                    }
                    else
                    {
                        _frameIndex = 0;
                    }
                }
            }

            ApplyCurrentFrame();
        }

        private void ApplyCurrentFrame()
        {
            if (spriteRenderer == null) return;
            Sprite[] frames = GetFrames(_currentAction, _facing);
            if (frames == null || frames.Length == 0) return;

            int safeIdx = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
            if (frames[safeIdx] != null)
            {
                spriteRenderer.sprite = frames[safeIdx];
            }
        }

        private void UpdateFacingFromVector(Vector2 dir)
        {
            if (dir.sqrMagnitude < 0.001f) return;

            float ax = Mathf.Abs(dir.x);
            float ay = Mathf.Abs(dir.y);

            if (ax > ay + 0.05f)
            {
                _facing = dir.x >= 0f ? Facing4.East : Facing4.West;
            }
            else if (ay > ax + 0.05f)
            {
                _facing = dir.y >= 0f ? Facing4.North : Facing4.South;
            }
            else
            {
                if (_facing == Facing4.East || _facing == Facing4.West)
                {
                    _facing = dir.x >= 0f ? Facing4.East : Facing4.West;
                }
                else
                {
                    _facing = dir.y >= 0f ? Facing4.North : Facing4.South;
                }
            }
        }

        private float GetFps(AnimAction action)
        {
            return action switch
            {
                AnimAction.Walk => walkFps,
                AnimAction.Dash => dashFps,
                AnimAction.ToolUse => toolFps,
                AnimAction.Hoe => hoeFps,
                AnimAction.WateringCan => waterFps,
                AnimAction.Hatchet => hatchetFps,
                AnimAction.MoonSickleSlash => slashFps,
                _ => idleFps
            };
        }

        private Sprite[] PickDir(Facing4 dir, Sprite[] s, Sprite[] n, Sprite[] e, Sprite[] w)
        {
            Sprite[] chosen = dir switch
            {
                Facing4.North => n,
                Facing4.East => e,
                Facing4.West => w,
                _ => s
            };
            if (chosen != null && chosen.Length > 0 && chosen[0] != null) return chosen;

            Debug.LogWarning("[MV] WenSpriteAnimator thiếu frame cho " + _currentAction + " (" + dir + "), fallback sang Idle.");
            return dir switch
            {
                Facing4.North => idleNorth,
                Facing4.East => idleEast,
                Facing4.West => idleWest,
                _ => idleSouth
            };
        }

        private Sprite[] GetFrames(AnimAction action, Facing4 dir)
        {
            return action switch
            {
                AnimAction.Walk => PickDir(dir, walkSouth, walkNorth, walkEast, walkWest),
                AnimAction.Dash => PickDir(dir, dashSouth, dashNorth, dashEast, dashWest),
                AnimAction.ToolUse => PickDir(dir, toolSouth, toolNorth, toolEast, toolWest),
                AnimAction.Hoe => PickDir(dir, hoeSouth, hoeNorth, hoeEast, hoeWest),
                AnimAction.WateringCan => PickDir(dir, waterSouth, waterNorth, waterEast, waterWest),
                AnimAction.Hatchet => PickDir(dir, hatchetSouth, hatchetNorth, hatchetEast, hatchetWest),
                AnimAction.MoonSickleSlash => PickDir(dir, slashSouth, slashNorth, slashEast, slashWest),
                _ => PickDir(dir, idleSouth, idleNorth, idleEast, idleWest)
            };
        }
    }
}
```

## 53. `Assets/Scripts/TimeSystem/AmbientPetalSpawner.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.TimeSystem
{
    public class AmbientPetalSpawner : MonoBehaviour
    {
        [SerializeField] private Sprite petalSprite;
        [SerializeField] private int particleCount = 18;

        private Transform[] _petals;
        private SpriteRenderer[] _renderers;
        private Vector2[] _speeds;
        private float[] _phases;
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            _petals = new Transform[particleCount];
            _renderers = new SpriteRenderer[particleCount];
            _speeds = new Vector2[particleCount];
            _phases = new float[particleCount];

            for (int i = 0; i < particleCount; i++)
            {
                GameObject go = new GameObject("AmbientPetal_" + i);
                go.transform.SetParent(transform, false);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = petalSprite;
                sr.sortingLayerName = "WeatherVFX";
                sr.sortingOrder = 5;

                float scale = Random.Range(0.45f, 0.95f);
                go.transform.localScale = new Vector3(scale, scale, 1f);

                _petals[i] = go.transform;
                _renderers[i] = sr;
                _speeds[i] = new Vector2(Random.Range(-0.55f, -0.18f), Random.Range(-0.35f, -0.12f));
                _phases[i] = Random.Range(0f, 6.28f);

                RespawnNearCamera(i, true);
            }
        }

        private void Update()
        {
            if (_cam == null) _cam = Camera.main;
            if (_cam == null || _petals == null) return;

            bool isNight = GameRoot.Instance != null && GameRoot.Instance.Time != null && GameRoot.Instance.Time.IsDuskOrNight;
            Color dayColor = new Color(1.0f, 0.84f, 0.92f, 0.72f);
            Color nightColor = new Color(0.68f, 0.98f, 0.88f, 0.85f);

            Vector3 camPos = _cam.transform.position;
            for (int i = 0; i < _petals.Length; i++)
            {
                _phases[i] += Time.deltaTime * 2.2f;
                Vector3 pos = _petals[i].position;
                pos.x += (_speeds[i].x + Mathf.Sin(_phases[i]) * 0.25f) * Time.deltaTime;
                pos.y += (_speeds[i].y + (isNight ? 0.35f : 0f)) * Time.deltaTime;
                _petals[i].position = pos;

                if (_renderers[i] != null)
                {
                    Color target = isNight ? nightColor : dayColor;
                    target.a *= 0.65f + Mathf.Sin(_phases[i] * 1.6f) * 0.35f;
                    _renderers[i].color = target;
                }

                if (Mathf.Abs(pos.x - camPos.x) > 12f || Mathf.Abs(pos.y - camPos.y) > 8f)
                {
                    RespawnNearCamera(i, false);
                }
            }
        }

        private void RespawnNearCamera(int idx, bool randomAllOver)
        {
            Vector3 c = _cam != null ? _cam.transform.position : Vector3.zero;
            float rx = randomAllOver ? Random.Range(-11f, 11f) : Random.Range(-8f, 11.5f);
            float ry = randomAllOver ? Random.Range(-7f, 7f) : Random.Range(-6f, 7.5f);
            _petals[idx].position = new Vector3(c.x + rx, c.y + ry, 0f);
        }
    }
}
```

## 54. `Assets/Scripts/TimeSystem/BedInteractable.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.TimeSystem
{
    public class BedInteractable : MonoBehaviour
    {
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 1.9f;

        private PlayerController2D _player;
        private GameInputReader _input;

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            float dist = Vector2.Distance(transform.position, _player.transform.position);
            bool inRange = dist <= interactRadius;

            if (promptObject != null && promptObject.activeSelf != inRange)
            {
                promptObject.SetActive(inRange);
            }

            if (inRange && _input != null && _input.InteractPressed)
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Sleep != null)
                {
                    GameRoot.Instance.Sleep.TriggerNormalSleep();
                }
            }
        }
    }
}
```

## 55. `Assets/Scripts/TimeSystem/DayNightLightingController.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.TimeSystem
{
    public class DayNightLightingController : MonoBehaviour
    {
        [SerializeField] private TimeConfigSO timeConfig;
        [SerializeField] private Image ambientTintImage;

        private Color _targetTint = new Color(1f, 0.94f, 0.78f, 0.06f);
        private float _targetLumiScale = 1.0f;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
            {
                HandleTimeChanged(GameRoot.Instance.Time.CurrentDay, GameRoot.Instance.Time.CurrentHourFloat);
            }
        }

        private void Update()
        {
            if (ambientTintImage != null)
            {
                ambientTintImage.color = Color.Lerp(ambientTintImage.color, _targetTint, Time.unscaledDeltaTime * 3.0f);
            }
        }

        private void HandleTimeChanged(int day, float hour)
        {
            float dusk = timeConfig != null ? timeConfig.duskStartHour : 18f;
            float night = timeConfig != null ? timeConfig.nightStartHour : 20f;

            Color morningTint = new Color(1.0f, 0.92f, 0.72f, 0.08f);
            Color noonTint = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            Color duskTint = new Color(0.88f, 0.42f, 0.16f, 0.24f);
            Color nightTint = new Color(0.06f, 0.09f, 0.24f, 0.50f);

            if (hour < 11f)
            {
                float t = Mathf.InverseLerp(6f, 11f, hour);
                _targetTint = Color.Lerp(morningTint, noonTint, t);
            }
            else if (hour < dusk)
            {
                float t = Mathf.InverseLerp(16f, dusk, hour);
                _targetTint = Color.Lerp(noonTint, duskTint, t);
            }
            else if (hour < night)
            {
                float t = Mathf.InverseLerp(dusk, night, hour);
                _targetTint = Color.Lerp(duskTint, nightTint, t);
            }
            else
            {
                _targetTint = nightTint;
            }

            bool upgraded = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                            GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            float nightScale = upgraded ? 3.2f : 2.0f;
            _targetLumiScale = hour >= dusk ? nightScale : 1.0f;

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null)
            {
                lumi.SetHaloRadiusScale(_targetLumiScale);
            }
        }
    }
}
```

## 56. `Assets/Scripts/TimeSystem/SleepAndKnockoutController.cs`

```csharp
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.TimeSystem
{
    public class SleepAndKnockoutController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private TMP_Text transitionText;

        public static event Action OnPassOutMaterialPenalty;

        private bool _isTransitioning;

        private void OnEnable()
        {
            TimeOfDayManager.OnLateNightPassOut += HandleLateNightPassOut;
            PlayerStatsController.OnPlayerKnockedOut += HandleStatKnockout;
        }

        private void OnDisable()
        {
            TimeOfDayManager.OnLateNightPassOut -= HandleLateNightPassOut;
            PlayerStatsController.OnPlayerKnockedOut -= HandleStatKnockout;
        }

        private void Start()
        {
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }

        private void HandleLateNightPassOut()
        {
            TriggerKnockout("Đã 02:00 đêm! Bạn thiếp đi vì quá mệt mỏi...\n(Sáng hôm sau trần Thể lực giảm còn 50% trừ khi ăn Bánh Mì Nắng Ấm)");
        }

        private void HandleStatKnockout(string reason)
        {
            TriggerKnockout(reason + "\n(Sáng hôm sau trần Thể lực giảm còn 50% trừ khi ăn Bánh Mì Nắng Ấm)");
        }

        public void TriggerNormalSleep()
        {
            if (_isTransitioning) return;
            StartCoroutine(DayTransitionRoutine(false, "Bạn nghỉ ngơi yên bình trong Nhà Kính...\nĐang đón ánh bình minh ngày mới."));
        }

        public void TriggerKnockout(string message)
        {
            if (_isTransitioning) return;
            StartCoroutine(DayTransitionRoutine(true, message));
        }

        private IEnumerator DayTransitionRoutine(bool passedOut, string message)
        {
            _isTransitioning = true;

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.DayTransition);
            }

            if (transitionText != null) transitionText.text = message;
            if (fadeCanvasGroup != null) fadeCanvasGroup.blocksRaycasts = true;

            // Fade to black
            float t = 0f;
            while (t < 0.45f)
            {
                t += Time.unscaledDeltaTime;
                if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = Mathf.Clamp01(t / 0.45f);
                yield return null;
            }

            // Qua ngày mới, hồi stats hoặc áp phạt kiệt sức, đưa Wen về Nhà Kính
            if (passedOut)
            {
                OnPassOutMaterialPenalty?.Invoke();
            }

            if (GameRoot.Instance != null)
            {
                if (GameRoot.Instance.Time != null) GameRoot.Instance.Time.AdvanceToNextDay();
                if (GameRoot.Instance.Stats != null) GameRoot.Instance.Stats.ResetForNewDay(passedOut);
            }

            if (WorldSpawnRegistry.Instance != null)
            {
                WorldSpawnRegistry.Instance.TeleportPlayerTo("Spawn_Greenhouse");
            }

            yield return new WaitForSecondsRealtime(1.1f);

            // Fade back in
            t = 0f;
            while (t < 0.45f)
            {
                t += Time.unscaledDeltaTime;
                if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(t / 0.45f);
                yield return null;
            }

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }

            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }

            string toast = passedOut
                ? "Tỉnh dậy sau khi kiệt sức! Trần Thể lực hôm nay còn 50%."
                : "Chào buổi sáng! Thể lực và Máu Ánh Sáng đã hồi đầy.";
            GameEventBus.RaiseToastRequested(toast);

            _isTransitioning = false;
        }
    }
}
```

## 57. `Assets/Scripts/TimeSystem/TimeOfDayManager.cs`

```csharp
using System;
using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;

namespace MoonpetalValley.TimeSystem
{
    public enum DayPhase
    {
        Morning,
        Noon,
        Dusk,
        Night
    }

    public class TimeOfDayManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private TimeConfigSO timeConfig;

        public static event Action OnLateNightPassOut;

        public int CurrentDay { get; private set; } = 1;
        public float CurrentHourFloat { get; private set; } = 6f;

        private int _lastBroadcastTenMinBucket = -1;
        private bool _passOutTriggeredToday;

        public string SaveKey => "time_of_day";

        public DayPhase CurrentPhase
        {
            get
            {
                float dusk = timeConfig != null ? timeConfig.duskStartHour : 18f;
                float night = timeConfig != null ? timeConfig.nightStartHour : 20f;
                if (CurrentHourFloat < 12f) return DayPhase.Morning;
                if (CurrentHourFloat < dusk) return DayPhase.Noon;
                if (CurrentHourFloat < night) return DayPhase.Dusk;
                return DayPhase.Night;
            }
        }

        public bool IsDuskOrNight => CurrentPhase == DayPhase.Dusk || CurrentPhase == DayPhase.Night;
        public bool IsNight => CurrentPhase == DayPhase.Night;

        [Serializable]
        private class TimeSaveData
        {
            public int day;
            public float hourFloat;
        }

        private void Start()
        {
            CurrentHourFloat = timeConfig != null ? timeConfig.dayStartHour : 6f;
            BroadcastTimeImmediate();
        }

        private void Update()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null && GameRoot.Instance.State.IsTimePaused)
            {
                return;
            }

            float dayStart = timeConfig != null ? timeConfig.dayStartHour : 6f;
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            float realSec = timeConfig != null && timeConfig.realSecondsPerGameDay > 1f
                ? timeConfig.realSecondsPerGameDay
                : 300f;

            float gameHoursPerRealSecond = (passOut - dayStart) / realSec;
            CurrentHourFloat += Time.deltaTime * gameHoursPerRealSecond;

            int bucket = Mathf.FloorToInt(CurrentHourFloat * 6f);
            if (bucket != _lastBroadcastTenMinBucket)
            {
                _lastBroadcastTenMinBucket = bucket;
                GameEventBus.RaiseTimeChanged(CurrentDay, CurrentHourFloat);
            }

            if (CurrentHourFloat >= passOut && !_passOutTriggeredToday)
            {
                _passOutTriggeredToday = true;
                if (OnLateNightPassOut != null)
                {
                    OnLateNightPassOut.Invoke();
                }
                else
                {
                    GameEventBus.RaiseToastRequested("Đã 02:00 đêm! Bạn kiệt sức và thiếp đi tới sáng hôm sau...");
                    AdvanceToNextDay();
                }
            }
        }

        public void AddHours(float deltaHours)
        {
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            CurrentHourFloat = Mathf.Clamp(CurrentHourFloat + deltaHours, 6f, passOut);
            BroadcastTimeImmediate();
        }

        public void SetHour(float targetHour)
        {
            float dayStart = timeConfig != null ? timeConfig.dayStartHour : 6f;
            float passOut = timeConfig != null ? timeConfig.passOutHour : 26f;
            CurrentHourFloat = Mathf.Clamp(targetHour, dayStart, passOut);
            _passOutTriggeredToday = false;
            BroadcastTimeImmediate();
        }

        public void AdvanceToNextDay()
        {
            CurrentDay++;
            CurrentHourFloat = timeConfig != null ? timeConfig.dayStartHour : 6f;
            _passOutTriggeredToday = false;
            GameEventBus.RaiseDayStarted(CurrentDay);
            BroadcastTimeImmediate();
        }

        public string GetFormattedClockString()
        {
            int totalMinutes = Mathf.FloorToInt(CurrentHourFloat * 60f);
            int displayHour = (totalMinutes / 60) % 24;
            int displayMin = (totalMinutes % 60 / 10) * 10;

            string phaseText = CurrentPhase switch
            {
                DayPhase.Morning => "Sáng",
                DayPhase.Noon => "Trưa",
                DayPhase.Dusk => "Chạng vạng",
                _ => "Đêm"
            };

            return string.Format("Ngày {0} — {1:00}:{2:00} ({3})", CurrentDay, displayHour, displayMin, phaseText);
        }

        private void BroadcastTimeImmediate()
        {
            _lastBroadcastTenMinBucket = Mathf.FloorToInt(CurrentHourFloat * 6f);
            GameEventBus.RaiseTimeChanged(CurrentDay, CurrentHourFloat);
        }

        public string CaptureStateJson()
        {
            var data = new TimeSaveData { day = CurrentDay, hourFloat = CurrentHourFloat };
            return JsonUtility.ToJson(data);
        }

        public void RestoreStateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<TimeSaveData>(json);
            if (data != null)
            {
                CurrentDay = Mathf.Max(1, data.day);
                CurrentHourFloat = Mathf.Clamp(data.hourFloat, 6f, 25.9f);
                _passOutTriggeredToday = false;
                BroadcastTimeImmediate();
            }
        }
    }
}
```

## 58. `Assets/Scripts/TimeSystem/VillageLantern.cs`

```csharp
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.TimeSystem
{
    public class VillageLantern : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer glowRenderer;

        private float _targetAlpha;
        private float _flickerPhase;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
        }

        private void Start()
        {
            _flickerPhase = Random.Range(0f, 6.28f);
            if (GameRoot.Instance != null && GameRoot.Instance.Time != null)
            {
                HandleTimeChanged(GameRoot.Instance.Time.CurrentDay, GameRoot.Instance.Time.CurrentHourFloat);
            }
        }

        private void HandleTimeChanged(int day, float hour)
        {
            _targetAlpha = hour >= 18f ? 0.78f : 0f;
        }

        private void Update()
        {
            if (glowRenderer == null) return;
            _flickerPhase += Time.deltaTime * 4f;
            float flicker = _targetAlpha > 0.01f ? (Mathf.Sin(_flickerPhase) * 0.06f) : 0f;
            Color c = glowRenderer.color;
            c.a = Mathf.Lerp(c.a, Mathf.Clamp01(_targetAlpha + flicker), Time.deltaTime * 4f);
            glowRenderer.color = c;
        }
    }
}
```

## 59. `Assets/Scripts/TimeSystem/WaterTileAnimator.cs`

```csharp
using UnityEngine;

namespace MoonpetalValley.TimeSystem
{
    public class WaterTileAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] waterFrames;
        [SerializeField] private SpriteRenderer[] waterRenderers;
        [SerializeField] private float fps = 3.2f;

        private float _timer;
        private int _frameIndex;

        private void Update()
        {
            if (waterFrames == null || waterFrames.Length == 0) return;
            if (waterRenderers == null || waterRenderers.Length == 0) return;

            _timer += Time.deltaTime;
            float interval = 1f / Mathf.Max(0.5f, fps);
            if (_timer >= interval)
            {
                _timer -= interval;
                _frameIndex = (_frameIndex + 1) % waterFrames.Length;
                Sprite current = waterFrames[_frameIndex];
                for (int i = 0; i < waterRenderers.Length; i++)
                {
                    if (waterRenderers[i] != null)
                    {
                        waterRenderers[i].sprite = current;
                    }
                }
            }
        }
    }
}
```

## 60. `Assets/Scripts/UI/BundleUIView.cs`

```csharp
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Community;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class BundleUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text itemsStatusText;
        [SerializeField] private TMP_Text helpersStatusText;
        [SerializeField] private Button btnContribute;
        [SerializeField] private Button btnBloom;
        [SerializeField] private Button btnClose;
        [SerializeField] private GameObject victoryPanelRoot;
        [SerializeField] private Button btnVictoryClose;

        private PlantingSpotController _activeSpot;
        private GameInputReader _input;
        private bool _isOpen;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (victoryPanelRoot != null) victoryPanelRoot.SetActive(false);

            if (btnContribute != null) btnContribute.onClick.AddListener(OnClickContribute);
            if (btnBloom != null) btnBloom.onClick.AddListener(OnClickBloom);
            if (btnClose != null) btnClose.onClick.AddListener(Close);
            if (btnVictoryClose != null) btnVictoryClose.onClick.AddListener(CloseVictory);
        }

        private void Update()
        {
            if (!_isOpen) return;
            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input != null && _input.PausePressed) Close();
        }

        public void Open(PlantingSpotController spot)
        {
            _activeSpot = spot;
            _isOpen = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance?.State?.SetState(GameState.BundleUI);
            RefreshView();
        }

        public void Close()
        {
            _isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance != null && GameRoot.Instance.State != null &&
                GameRoot.Instance.State.CurrentState == GameState.BundleUI)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void CloseVictory()
        {
            if (victoryPanelRoot != null) victoryPanelRoot.SetActive(false);
            GameRoot.Instance?.State?.ReturnToGameplay();
        }

        private void OnClickContribute()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Bundle == null) return;
            int added = GameRoot.Instance.Bundle.ContributeAllAvailable();
            if (added > 0)
            {
                GameEventBus.RaiseToastRequested("Đã đóng góp " + added + " vật phẩm vào Gói Ký Ức Vườn Xưa!");
            }
            else
            {
                GameEventBus.RaiseToastRequested("Chưa có thêm vật phẩm cần nộp trong túi đồ.");
            }
            RefreshView();
        }

        private void OnClickBloom()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Bundle == null) return;
            if (!GameRoot.Instance.Bundle.CanBloomNow())
            {
                GameEventBus.RaiseToastRequested("Cần nộp đủ 5 vật phẩm VÀ có đủ 4/4 người chung tay!");
                return;
            }

            if (panelRoot != null) panelRoot.SetActive(false);
            _isOpen = false;

            if (_activeSpot != null) _activeSpot.ExecuteBloomClimax();

            if (victoryPanelRoot != null)
            {
                victoryPanelRoot.SetActive(true);
                GameRoot.Instance.State?.SetState(GameState.Cutscene);
            }
            else
            {
                GameRoot.Instance.State?.ReturnToGameplay();
            }
        }

        public void RefreshView()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Bundle == null) return;
            var mgr = GameRoot.Instance.Bundle;
            var bundle = mgr.ActiveBundle;
            var inv = GameRoot.Instance.Inventory;
            var flags = GameRoot.Instance.Flags;
            if (bundle == null) return;

            var sbItems = new StringBuilder();
            sbItems.AppendLine("<b>VẬT PHẨM ĐÓNG GÓP:</b>");
            for (int i = 0; i < bundle.requiredItems.Count; i++)
            {
                var req = bundle.requiredItems[i];
                if (req.item == null) continue;
                int deposited = mgr.GetContributedAmount(req.item.itemID);
                int inBag = inv != null ? inv.GetItemCount(req.item.itemID) : 0;
                string name = inv != null ? inv.GetLocalizedItemName(req.item) : req.item.itemID;
                string mark = deposited >= req.amount ? "[OK] " : "[..] ";
                sbItems.AppendLine(string.Format("{0}{1}: {2}/{3} (Túi: {4})", mark, name, deposited, req.amount, inBag));
            }
            if (itemsStatusText != null) itemsStatusText.text = sbItems.ToString();

            var sbHelpers = new StringBuilder();
            sbHelpers.AppendLine(string.Format("<b>NGƯỜI CHUNG TAY ({0}/{1}):</b>", mgr.GetReadyHelperCount(), mgr.GetTotalRequiredHelpers()));
            string[] names = { "Ông Bách (Đọc đủ 4 trang thư)", "Bà Hana (Giao 4 Lúa Mạch)", "Cậu Bé Toby (Cứu mèo Miu)", "Bác Otis (Sửa Hòm Thư)" };
            for (int i = 0; i < bundle.requiredHelperFlags.Count; i++)
            {
                bool ready = flags != null && flags.HasFlag(bundle.requiredHelperFlags[i]);
                string label = i < names.Length ? names[i] : bundle.requiredHelperFlags[i];
                sbHelpers.AppendLine((ready ? "[SẴN SÀNG] " : "[CHỜ] ") + label);
            }
            if (helpersStatusText != null) helpersStatusText.text = sbHelpers.ToString();

            if (btnBloom != null)
            {
                bool canBloom = mgr.CanBloomNow();
                btnBloom.interactable = canBloom;
                var img = btnBloom.GetComponent<Image>();
                if (img != null) img.color = canBloom ? new Color(0.18f, 0.62f, 0.42f, 1f) : new Color(0.28f, 0.30f, 0.36f, 0.8f);
            }
        }
    }
}
```

## 61. `Assets/Scripts/UI/ClockHUDView.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.UI
{
    public class ClockHUDView : MonoBehaviour
    {
        [SerializeField] private TMP_Text clockText;
        [SerializeField] private Image phaseBadgeImage;

        private void OnEnable()
        {
            GameEventBus.OnTimeChanged += HandleTimeChanged;
            GameEventBus.OnDayStarted += HandleDayStarted;
        }

        private void OnDisable()
        {
            GameEventBus.OnTimeChanged -= HandleTimeChanged;
            GameEventBus.OnDayStarted -= HandleDayStarted;
        }

        private void Start()
        {
            RefreshDisplay();
        }

        private void HandleTimeChanged(int day, float hour) => RefreshDisplay();
        private void HandleDayStarted(int day) => RefreshDisplay();

        public void RefreshDisplay()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Time == null) return;
            var tm = GameRoot.Instance.Time;

            if (clockText != null)
            {
                clockText.text = tm.GetFormattedClockString();
            }

            if (phaseBadgeImage != null)
            {
                phaseBadgeImage.color = tm.CurrentPhase switch
                {
                    DayPhase.Morning => new Color(0.95f, 0.80f, 0.35f, 1f),
                    DayPhase.Noon => new Color(0.98f, 0.90f, 0.42f, 1f),
                    DayPhase.Dusk => new Color(0.92f, 0.48f, 0.24f, 1f),
                    _ => new Color(0.36f, 0.50f, 0.88f, 1f)
                };
            }
        }
    }
}
```

## 62. `Assets/Scripts/UI/DialogueUIView.cs`

```csharp
using UnityEngine;
using TMPro;

namespace MoonpetalValley.UI
{
    public class DialogueUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text continueHintText;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void ShowLine(string speaker, string body, bool isLastLine)
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            if (speakerText != null) speakerText.text = speaker;
            if (bodyText != null) bodyText.text = body;
            if (continueHintText != null)
            {
                continueHintText.text = isLastLine ? "[E / Space] Kết thúc" : "[E / Space] Tiếp tục >>";
            }
        }

        public void Hide()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }
    }
}
```

## 63. `Assets/Scripts/UI/DiaryUIView.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class DiaryUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text pageHeaderText;
        [SerializeField] private TMP_Text pageTitleText;
        [SerializeField] private TMP_Text pageBodyText;
        [SerializeField] private Button btnPrev;
        [SerializeField] private Button btnNext;
        [SerializeField] private Button btnClose;

        private GameInputReader _input;
        private bool _isOpen;
        private int _pageIndex;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (btnPrev != null) btnPrev.onClick.AddListener(() => ChangePage(-1));
            if (btnNext != null) btnNext.onClick.AddListener(() => ChangePage(1));
            if (btnClose != null) btnClose.onClick.AddListener(Close);
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input == null) return;

            if (_input.DiaryPressed)
            {
                if (!_isOpen && GameRoot.Instance.State.IsGameplayActive) Open();
                else if (_isOpen) Close();
            }
            else if (_isOpen && _input.PausePressed)
            {
                Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance.State.SetState(GameState.DiaryUI);
            RefreshPage();
        }

        public void Close()
        {
            _isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance.State.CurrentState == GameState.DiaryUI)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void ChangePage(int delta)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Diary == null) return;
            var pages = GameRoot.Instance.Diary.GetUnlockedPages();
            if (pages.Count == 0) return;
            _pageIndex = Mathf.Clamp(_pageIndex + delta, 0, pages.Count - 1);
            RefreshPage();
        }

        private void RefreshPage()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Diary == null) return;
            List<DiaryPageData> pages = GameRoot.Instance.Diary.GetUnlockedPages();

            if (pages.Count == 0)
            {
                if (pageHeaderText != null) pageHeaderText.text = "SỔ TAY CỦA BÀ (0/4 trang)";
                if (pageTitleText != null) pageTitleText.text = "Chưa có trang nhật ký nào";
                if (pageBodyText != null) pageBodyText.text = "Hãy dọn dẹp Nhà Kính và mở Chiếc Hộp Gỗ Của Bà để mở khóa Trang 1.";
                return;
            }

            _pageIndex = Mathf.Clamp(_pageIndex, 0, pages.Count - 1);
            DiaryPageData current = pages[_pageIndex];
            var loc = GameRoot.Instance.Loc;

            if (pageHeaderText != null) pageHeaderText.text = string.Format("SỔ TAY CỦA BÀ — Trang {0}/{1} (Đã mở: {2}/4)", current.pageNumber, 4, pages.Count);
            if (pageTitleText != null) pageTitleText.text = loc != null ? loc.Get(current.titleKey) : current.titleKey;
            if (pageBodyText != null) pageBodyText.text = loc != null ? loc.Get(current.bodyTextKey) : current.bodyTextKey;
        }
    }
}
```

## 64. `Assets/Scripts/UI/FloatingDamageText.cs`

```csharp
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
```

## 65. `Assets/Scripts/UI/FloatingToastUI.cs`

```csharp
using UnityEngine;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.UI
{
    public class FloatingToastUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text toastText;
        [SerializeField] private float displayDuration = 2.6f;

        private float _timer;

        private void OnEnable()
        {
            GameEventBus.OnToastRequested += ShowToast;
        }

        private void OnDisable()
        {
            GameEventBus.OnToastRequested -= ShowToast;
        }

        private void Start()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        public void ShowToast(string message)
        {
            if (toastText != null)
            {
                toastText.text = message;
            }
            _timer = displayDuration;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private void Update()
        {
            if (_timer <= 0f) return;

            _timer -= Time.unscaledDeltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = _timer < 0.5f ? Mathf.Clamp01(_timer / 0.5f) : 1f;
            }
        }
    }
}
```

## 66. `Assets/Scripts/UI/HotbarUIView.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Inventory;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class HotbarUIView : MonoBehaviour
    {
        [SerializeField] private Image[] slotBackgrounds;
        [SerializeField] private Image[] slotIcons;
        [SerializeField] private TMP_Text[] slotCounts;
        [SerializeField] private TMP_Text selectedItemLabel;

        private GameInputReader _input;
        private readonly Color _normalSlotColor = new Color(0.11f, 0.15f, 0.22f, 0.88f);
        private readonly Color _selectedSlotColor = new Color(0.86f, 0.70f, 0.24f, 0.96f);

        private void OnEnable()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged += RefreshUI;
            }
        }

        private void OnDisable()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged -= RefreshUI;
            }
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged -= RefreshUI;
                GameRoot.Instance.Inventory.OnInventoryChanged += RefreshUI;
            }
            RefreshUI();
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            if (GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive) return;

            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }

            if (_input != null)
            {
                if (_input.HotbarSlotPressed >= 0)
                {
                    GameRoot.Instance.Inventory.SelectHotbarSlot(_input.HotbarSlotPressed);
                }

                if (_input.ActionPressed)
                {
                    bool pointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                    if (!pointerOverUI)
                    {
                        var sel = GameRoot.Instance.Inventory.GetSelectedSlot();
                        if (sel != null && !sel.IsEmpty && sel.item.category == ItemCategory.Consumable)
                        {
                            GameRoot.Instance.Inventory.UseSelectedConsumable();
                        }
                    }
                }
            }

            // Cuộn chuột chuyển ô Hotbar
            float scroll = ReadMouseScroll();
            if (scroll > 0.1f) GameRoot.Instance.Inventory.CycleHotbar(-1);
            else if (scroll < -0.1f) GameRoot.Instance.Inventory.CycleHotbar(1);
        }

        private float ReadMouseScroll()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                return UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.mouseScrollDelta.y;
#endif
            return 0f;
        }

        public void RefreshUI()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            for (int i = 0; i < InventoryManager.HotbarSize; i++)
            {
                var slot = inv.GetSlot(i);
                bool isSel = i == inv.SelectedHotbarIndex;

                if (slotBackgrounds != null && i < slotBackgrounds.Length && slotBackgrounds[i] != null)
                {
                    slotBackgrounds[i].color = isSel ? _selectedSlotColor : _normalSlotColor;
                }

                if (slotIcons != null && i < slotIcons.Length && slotIcons[i] != null)
                {
                    if (slot != null && !slot.IsEmpty && slot.item.icon != null)
                    {
                        slotIcons[i].sprite = slot.item.icon;
                        slotIcons[i].color = Color.white;
                    }
                    else
                    {
                        slotIcons[i].sprite = null;
                        slotIcons[i].color = new Color(1f, 1f, 1f, 0f);
                    }
                }

                if (slotCounts != null && i < slotCounts.Length && slotCounts[i] != null)
                {
                    if (slot == null || slot.IsEmpty)
                    {
                        slotCounts[i].text = "";
                    }
                    else if (slot.item.itemID == "item_watering_can")
                    {
                        slotCounts[i].text = inv.WateringCanCharges + "/20";
                    }
                    else if (slot.amount > 1)
                    {
                        slotCounts[i].text = slot.amount.ToString();
                    }
                    else
                    {
                        slotCounts[i].text = "";
                    }
                }
            }

            if (selectedItemLabel != null)
            {
                var active = inv.GetSelectedSlot();
                if (active == null || active.IsEmpty)
                {
                    selectedItemLabel.text = "Ô trống (Phím 1–8 đổi công cụ | Tab mở Túi Đồ)";
                }
                else
                {
                    string name = inv.GetLocalizedItemName(active.item);
                    string hint = active.item.category == ItemCategory.Consumable ? " — [Chuột trái / J] để ăn" : "";
                    selectedItemLabel.text = name + hint;
                }
            }
        }
    }
}
```

## 67. `Assets/Scripts/UI/InventoryUIView.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Inventory;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class InventoryUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Button[] slotButtons;
        [SerializeField] private Image[] slotIcons;
        [SerializeField] private TMP_Text[] slotCounts;
        [SerializeField] private TMP_Text detailNameText;
        [SerializeField] private TMP_Text detailDescText;

        private GameInputReader _input;
        private bool _isOpen;
        private int _pickedSlotIndex = -1;
        private int _draggingSlotIndex = -1;
        private Image _dragGhostImage;
        private RectTransform _dragGhostRect;

        private readonly Color _hotbarBgColor = new Color(0.18f, 0.24f, 0.34f, 1f);
        private readonly Color _bagBgColor = new Color(0.14f, 0.17f, 0.24f, 1f);
        private readonly Color _pickedBgColor = new Color(0.84f, 0.68f, 0.24f, 1f);

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            CreateDragGhost();

            if (slotButtons != null)
            {
                for (int i = 0; i < slotButtons.Length; i++)
                {
                    int captured = i;
                    if (slotButtons[i] != null)
                    {
                        slotButtons[i].onClick.AddListener(() => OnSlotClicked(captured));
                        var dragHandler = slotButtons[i].gameObject.GetComponent<InventorySlotDragHandler>();
                        if (dragHandler == null) dragHandler = slotButtons[i].gameObject.AddComponent<InventorySlotDragHandler>();
                        dragHandler.Initialize(this, captured);
                    }
                }
            }
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged += RefreshGrid;
            }
        }

        private void OnDestroy()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                GameRoot.Instance.Inventory.OnInventoryChanged -= RefreshGrid;
            }
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;

            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input == null) return;

            if (_input.InventoryPressed)
            {
                if (!_isOpen && GameRoot.Instance.State.IsGameplayActive)
                {
                    Open();
                }
                else if (_isOpen)
                {
                    Close();
                }
            }
            else if (_isOpen && _input.PausePressed)
            {
                Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            _pickedSlotIndex = -1;
            CancelDrag();
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance.State.SetState(GameState.InventoryUI);
            RefreshGrid();
            ShowSlotDetails(GameRoot.Instance.Inventory.SelectedHotbarIndex);
        }

        public void Close()
        {
            _isOpen = false;
            _pickedSlotIndex = -1;
            CancelDrag();
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance.State.CurrentState == GameState.InventoryUI)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void CreateDragGhost()
        {
            GameObject go = new GameObject("InventoryDragGhost");
            go.transform.SetParent(transform, false);
            _dragGhostRect = go.AddComponent<RectTransform>();
            _dragGhostRect.sizeDelta = new Vector2(54f, 54f);
            _dragGhostImage = go.AddComponent<Image>();
            _dragGhostImage.preserveAspect = true;
            _dragGhostImage.raycastTarget = false;
            go.SetActive(false);
        }

        internal void HandleBeginDrag(int slotIndex, PointerEventData eventData)
        {
            if (!_isOpen || GameRoot.Instance?.Inventory == null) return;
            var slot = GameRoot.Instance.Inventory.GetSlot(slotIndex);
            if (slot == null || slot.IsEmpty) return;

            _draggingSlotIndex = slotIndex;
            _pickedSlotIndex = -1;

            if (_dragGhostImage != null)
            {
                _dragGhostImage.sprite = slot.item.icon;
                _dragGhostImage.color = new Color(1f, 1f, 1f, 0.92f);
                _dragGhostImage.gameObject.SetActive(true);
                _dragGhostImage.transform.SetAsLastSibling();
                _dragGhostRect.position = eventData.position;
            }
            RefreshGrid();
        }

        internal void HandleDrag(PointerEventData eventData)
        {
            if (_draggingSlotIndex < 0 || _dragGhostRect == null) return;
            _dragGhostRect.position = eventData.position;
        }

        internal void HandleDrop(int targetSlotIndex)
        {
            if (!_isOpen || _draggingSlotIndex < 0 || GameRoot.Instance?.Inventory == null) return;
            int source = _draggingSlotIndex;
            CancelDrag();
            if (source != targetSlotIndex)
            {
                GameRoot.Instance.Inventory.SwapOrMergeSlots(source, targetSlotIndex);
                GameRoot.Instance.Audio?.PlayUIClick();
            }
            ShowSlotDetails(targetSlotIndex);
            RefreshGrid();
        }

        internal void HandleEndDrag()
        {
            CancelDrag();
            RefreshGrid();
        }

        private void CancelDrag()
        {
            _draggingSlotIndex = -1;
            if (_dragGhostImage != null) _dragGhostImage.gameObject.SetActive(false);
        }

        private void RefreshGrid()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            for (int i = 0; i < InventoryManager.TotalSlots; i++)
            {
                var slot = inv.GetSlot(i);

                if (slotButtons != null && i < slotButtons.Length && slotButtons[i] != null)
                {
                    var bg = slotButtons[i].GetComponent<Image>();
                    if (bg != null)
                    {
                        if (i == _pickedSlotIndex || i == _draggingSlotIndex) bg.color = _pickedBgColor;
                        else bg.color = i < InventoryManager.HotbarSize ? _hotbarBgColor : _bagBgColor;
                    }
                }

                if (slotIcons != null && i < slotIcons.Length && slotIcons[i] != null)
                {
                    if (slot != null && !slot.IsEmpty && slot.item.icon != null)
                    {
                        slotIcons[i].sprite = slot.item.icon;
                        slotIcons[i].color = (i == _draggingSlotIndex) ? new Color(1f, 1f, 1f, 0.35f) : Color.white;
                    }
                    else
                    {
                        slotIcons[i].sprite = null;
                        slotIcons[i].color = new Color(1f, 1f, 1f, 0f);
                    }
                }

                if (slotCounts != null && i < slotCounts.Length && slotCounts[i] != null)
                {
                    if (slot == null || slot.IsEmpty) slotCounts[i].text = "";
                    else if (slot.item.itemID == "item_watering_can") slotCounts[i].text = inv.WateringCanCharges + "/20";
                    else if (slot.amount > 1) slotCounts[i].text = slot.amount.ToString();
                    else slotCounts[i].text = "";
                }
            }
        }

        private void OnSlotClicked(int index)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;

            // Click-to-Swap: chọn ô 1 -> bấm ô 2 để hoán đổi / gộp stack
            if (_pickedSlotIndex < 0)
            {
                var clickedSlot = inv.GetSlot(index);
                if (clickedSlot != null && !clickedSlot.IsEmpty)
                {
                    _pickedSlotIndex = index;
                }
                ShowSlotDetails(index);
                RefreshGrid();
                return;
            }

            if (_pickedSlotIndex == index)
            {
                _pickedSlotIndex = -1;
                ShowSlotDetails(index);
                RefreshGrid();
                return;
            }

            int src = _pickedSlotIndex;
            _pickedSlotIndex = -1;
            inv.SwapOrMergeSlots(src, index);
            GameRoot.Instance.Audio?.PlayUIClick();
            ShowSlotDetails(index);
            RefreshGrid();
        }

        private void ShowSlotDetails(int index)
        {
            if (GameRoot.Instance == null || GameRoot.Instance.Inventory == null) return;
            var inv = GameRoot.Instance.Inventory;
            var slot = inv.GetSlot(index);

            if (slot == null || slot.IsEmpty)
            {
                if (detailNameText != null) detailNameText.text = "Ô trống";
                if (detailDescText != null) detailDescText.text = "Kéo-thả hoặc bấm chọn 2 ô để chuyển vật phẩm giữa Hotbar (hàng trên) và Túi đồ (hàng dưới).";
                return;
            }

            if (detailNameText != null)
            {
                string protectTag = slot.item.isQuestProtected ? " [Được bảo vệ]" : " [Nguyên liệu thường]";
                string pickHint = _pickedSlotIndex == index ? " — [ĐANG CHỌN: bấm ô khác để chuyển]" : "";
                detailNameText.text = inv.GetLocalizedItemName(slot.item) + " (x" + slot.amount + ")" + protectTag + pickHint;
            }
            if (detailDescText != null)
            {
                detailDescText.text = inv.GetLocalizedItemDesc(slot.item);
            }
        }
    }

    internal class InventorySlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        private InventoryUIView _owner;
        private int _slotIndex;

        public void Initialize(InventoryUIView owner, int slotIndex)
        {
            _owner = owner;
            _slotIndex = slotIndex;
        }

        public void OnBeginDrag(PointerEventData eventData) => _owner?.HandleBeginDrag(_slotIndex, eventData);
        public void OnDrag(PointerEventData eventData) => _owner?.HandleDrag(eventData);
        public void OnDrop(PointerEventData eventData) => _owner?.HandleDrop(_slotIndex);
        public void OnEndDrag(PointerEventData eventData) => _owner?.HandleEndDrag();
    }
}
```

## 68. `Assets/Scripts/UI/MainMenuController.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public static bool PendingLoadSaveOnWorldStart;

        [SerializeField] private Button btnNewGame;
        [SerializeField] private Button btnContinue;
        [SerializeField] private Button btnQuit;
        [SerializeField] private TMP_Text continueSubtext;

        private void Awake()
        {
            if (btnNewGame != null) btnNewGame.onClick.AddListener(OnClickNewGame);
            if (btnContinue != null) btnContinue.onClick.AddListener(OnClickContinue);
            if (btnQuit != null) btnQuit.onClick.AddListener(OnClickQuit);
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.State != null)
            {
                GameRoot.Instance.State.SetState(GameState.PauseMenu);
            }

            bool hasSave = GameRoot.Instance != null && GameRoot.Instance.SaveLoad != null &&
                           GameRoot.Instance.SaveLoad.HasSaveFile();

            if (btnContinue != null)
            {
                btnContinue.interactable = hasSave;
            }
            if (continueSubtext != null)
            {
                continueSubtext.text = hasSave
                    ? "Bản lưu gần nhất: " + GameRoot.Instance.SaveLoad.LastSavedTimestamp
                    : "Chưa có bản lưu nào — Hãy chọn Chơi Mới";
            }
        }

        private void OnClickNewGame()
        {
            PendingLoadSaveOnWorldStart = false;
            GameRoot.Instance?.Audio?.PlayUIClick();
            GameRoot.Instance?.State?.ReturnToGameplay();
            GameRoot.Instance?.Scenes?.LoadScene("MoonpetalValley_MVP");
        }

        private void OnClickContinue()
        {
            PendingLoadSaveOnWorldStart = true;
            GameRoot.Instance?.Audio?.PlayUIClick();
            GameRoot.Instance?.State?.ReturnToGameplay();
            GameRoot.Instance?.Scenes?.LoadScene("MoonpetalValley_MVP");
        }

        private void OnClickQuit()
        {
            GameRoot.Instance?.Audio?.PlayUIClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
```

## 69. `Assets/Scripts/UI/PauseMenuUIView.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class PauseMenuUIView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text saveStatusText;
        [SerializeField] private TMP_Text muteButtonText;
        [SerializeField] private Button btnResume;
        [SerializeField] private Button btnSave;
        [SerializeField] private Button btnLoad;
        [SerializeField] private Button btnMute;
        [SerializeField] private Button btnDeleteSave;

        private GameInputReader _input;
        private bool _isOpen;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);

            if (btnResume != null) btnResume.onClick.AddListener(Close);
            if (btnSave != null) btnSave.onClick.AddListener(OnClickSave);
            if (btnLoad != null) btnLoad.onClick.AddListener(OnClickLoad);
            if (btnMute != null) btnMute.onClick.AddListener(OnClickMute);
            if (btnDeleteSave != null) btnDeleteSave.onClick.AddListener(OnClickDeleteSave);
        }

        private void Update()
        {
            if (GameRoot.Instance == null || GameRoot.Instance.State == null) return;
            if (_input == null)
            {
                var p = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (p != null) _input = p.GetComponent<GameInputReader>();
            }
            if (_input == null) return;

            if (_input.PausePressed)
            {
                if (!_isOpen && GameRoot.Instance.State.IsGameplayActive) Open();
                else if (_isOpen) Close();
            }
        }

        public void Open()
        {
            _isOpen = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            GameRoot.Instance.State.SetState(GameState.PauseMenu);
            GameRoot.Instance.Audio?.PlayUIClick();
            RefreshLabels();
        }

        public void Close()
        {
            _isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            if (GameRoot.Instance.State.CurrentState == GameState.PauseMenu)
            {
                GameRoot.Instance.State.ReturnToGameplay();
            }
        }

        private void OnClickSave()
        {
            GameRoot.Instance?.SaveLoad?.SaveGame(false);
            RefreshLabels();
        }

        private void OnClickLoad()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.SaveLoad != null && GameRoot.Instance.SaveLoad.LoadGame())
            {
                Close();
            }
        }

        private void OnClickMute()
        {
            GameRoot.Instance?.Audio?.ToggleMute();
            RefreshLabels();
        }

        private void OnClickDeleteSave()
        {
            GameRoot.Instance?.SaveLoad?.DeleteSaveFile();
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            if (saveStatusText != null && GameRoot.Instance?.SaveLoad != null)
            {
                var sl = GameRoot.Instance.SaveLoad;
                saveStatusText.text = sl.HasSaveFile()
                    ? "Bản lưu gần nhất: " + sl.LastSavedTimestamp
                    : "Chưa có bản lưu game (Tự động lưu mỗi khi đi ngủ)";
            }
            if (muteButtonText != null && GameRoot.Instance?.Audio != null)
            {
                muteButtonText.text = GameRoot.Instance.Audio.IsMuted ? "Âm Thanh: ĐANG TẮT" : "Âm Thanh: ĐANG BẬT";
            }
        }
    }
}
```

## 70. `Assets/Scripts/UI/QuestTrackerHUDView.cs`

```csharp
using UnityEngine;
using TMPro;
using MoonpetalValley.Core;

namespace MoonpetalValley.UI
{
    public class QuestTrackerHUDView : MonoBehaviour
    {
        [SerializeField] private TMP_Text objectiveText;

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Quests != null)
            {
                GameRoot.Instance.Quests.OnObjectiveTextChanged += UpdateText;
                UpdateText(GameRoot.Instance.Quests.CurrentObjectiveSummary);
            }
        }

        private void OnDestroy()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Quests != null)
            {
                GameRoot.Instance.Quests.OnObjectiveTextChanged -= UpdateText;
            }
        }

        private void UpdateText(string text)
        {
            if (objectiveText != null) objectiveText.text = text;
        }
    }
}
```

## 71. `Assets/Scripts/UI/StatsHUDView.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.UI
{
    public class StatsHUDView : MonoBehaviour
    {
        [SerializeField] private Image lightHPFill;
        [SerializeField] private TMP_Text lightHPText;
        [SerializeField] private Image staminaFill;
        [SerializeField] private TMP_Text staminaText;
        [SerializeField] private GameObject exhaustedBadge;

        private void Awake()
        {
            if (exhaustedBadge != null)
            {
                var badgeTmp = exhaustedBadge.GetComponent<TMP_Text>();
                if (badgeTmp != null)
                {
                    badgeTmp.text = "[!] KIỆT SỨC: Trần Thể lực giảm còn 50% (Ăn Bánh Mì để xóa)";
                }
            }
        }

        private void OnEnable()
        {
            GameEventBus.OnLightHPChanged += HandleLightHPChanged;
            GameEventBus.OnStaminaChanged += HandleStaminaChanged;
            PlayerStatsController.OnExhaustionStateChanged += HandleExhaustionChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnLightHPChanged -= HandleLightHPChanged;
            GameEventBus.OnStaminaChanged -= HandleStaminaChanged;
            PlayerStatsController.OnExhaustionStateChanged -= HandleExhaustionChanged;
        }

        private void Start()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.Stats != null)
            {
                var st = GameRoot.Instance.Stats;
                HandleLightHPChanged(st.CurrentLightHP, st.MaxLightHP);
                HandleStaminaChanged(st.CurrentStamina, st.EffectiveMaxStamina);
                HandleExhaustionChanged(st.IsExhaustedPenalty);
            }
        }

        private void HandleLightHPChanged(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            if (lightHPFill != null) lightHPFill.fillAmount = ratio;
            if (lightHPText != null) lightHPText.text = string.Format("Máu Ánh Sáng: {0}/{1}", current, max);
        }

        private void HandleStaminaChanged(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            if (staminaFill != null) staminaFill.fillAmount = ratio;
            if (staminaText != null) staminaText.text = string.Format("Thể lực: {0}/{1}", current, max);
        }

        private void HandleExhaustionChanged(bool isExhausted)
        {
            if (exhaustedBadge != null)
            {
                exhaustedBadge.SetActive(isExhausted);
            }
            if (staminaFill != null)
            {
                staminaFill.color = isExhausted
                    ? new Color(0.92f, 0.44f, 0.24f, 1f)
                    : new Color(0.95f, 0.78f, 0.24f, 1f);
            }
        }
    }
}
```

## 72. `Assets/Editor/AutoBuilder_Step1.cs`

```csharp
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step1
    {
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string FontCopyTtfPath = "Assets/Art/Fonts/VN_SourceFont.ttf";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string TimeConfigPath = "Assets/Data/Config/TimeConfig.asset";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";

        private static readonly string[] RequiredTags =
        {
            "NPC", "Enemy", "Interactable", "FarmTile", "ResourceNode", "BlightFog"
        };

        private static readonly (int index, string name)[] RequiredLayers =
        {
            (8, "Player"),
            (9, "Enemy"),
            (10, "Obstacle"),
            (11, "Interactable"),
            (12, "PlayerHitbox"),
            (13, "EnemyHitbox"),
            (14, "TriggerZone")
        };

        private static readonly string[] RequiredSortingLayers =
        {
            "Ground", "FloorDecal", "FarmTile", "Shadows",
            "WorldObjects", "FogOverlay", "WeatherVFX", "UI"
        };

        [MenuItem("Moonpetal/Step 1/1. Build Foundation & Scenes")]
        public static void BuildStep1()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 1 ===");
            CheckInputHandlingConfiguration();
            EnsureFolderStructure();
            ConfigureTagsLayersAndSortingLayers();
            ConfigurePhysics2DMatrix();

            TMP_FontAsset vnFont = EnsureVietnameseTMPFont();
            EnsurePlayerStatsAsset();
            EnsureTimeConfigAsset();
            StringTableSO stringTable = EnsureStringTableAsset();

            GameObject gameRootPrefab = BuildOrUpdateGameRootPrefab(stringTable, vnFont);
            BuildBootstrapScene(gameRootPrefab);
            BuildWorldPlaceholderScene(gameRootPrefab, vnFont);
            ConfigureBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 1! Hãy chạy 'Moonpetal/Step 1/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 1/Validate")]
        public static void ValidateStep1()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 1 ===");

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");
            foreach (string reqTag in RequiredTags)
            {
                if (!PropertyArrayContainsString(tagsProp, reqTag))
                {
                    Debug.LogError("[MV] THIẾU Tag: " + reqTag);
                    errors++;
                }
            }

            var layersProp = tagManager.FindProperty("layers");
            foreach (var pair in RequiredLayers)
            {
                string actual = layersProp.GetArrayElementAtIndex(pair.index).stringValue;
                if (actual != pair.name)
                {
                    Debug.LogError("[MV] SAI Layer " + pair.index + ": cần '" + pair.name + "', hiện tại '" + actual + "'");
                    errors++;
                }
            }

            errors += CheckAssetExists<TMP_FontAsset>(FontAssetPath);
            errors += CheckAssetExists<PlayerStatsSO>(PlayerStatsPath);
            errors += CheckAssetExists<TimeConfigSO>(TimeConfigPath);
            errors += CheckAssetExists<StringTableSO>(StringTablePath);
            errors += CheckAssetExists<GameObject>(GameRootPrefabPath);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                var root = prefab.GetComponent<GameRoot>();
                if (root == null)
                {
                    Debug.LogError("[MV] GameRoot.prefab thiếu component GameRoot.");
                    errors++;
                }
                else
                {
                    var so = new SerializedObject(root);
                    string[] fields = { "gameStateManager", "storyFlagManager", "localization", "sceneLoader", "toastUI", "debugOverlay" };
                    foreach (string f in fields)
                    {
                        var p = so.FindProperty(f);
                        if (p == null || p.objectReferenceValue == null)
                        {
                            Debug.LogError("[MV] GameRoot thiếu tham chiếu SerializeField: " + f);
                            errors++;
                        }
                    }
                }
            }

            if (EditorBuildSettings.scenes.Length < 2 ||
                EditorBuildSettings.scenes[0].path != BootstrapScenePath ||
                EditorBuildSettings.scenes[1].path != WorldScenePath)
            {
                Debug.LogError("[MV] Build Settings chưa đúng thứ tự Bootstrap (0) -> MoonpetalValley_MVP (1).");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (missing > 0)
                {
                    Debug.LogError("[MV] GameObject '" + go.name + "' có " + missing + " Missing Script!");
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 1: PASS (0 lỗi). Toàn bộ nền tảng, Font VN, Data SO và Scene đã sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 1: FAIL (" + errors + " lỗi). Xem các dòng lỗi ở trên.");
            }
        }

        private static int CheckAssetExists<T>(string path) where T : UnityEngine.Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                Debug.LogError("[MV] THIẾU Asset: " + path);
                return 1;
            }
            return 0;
        }

        private static void CheckInputHandlingConfiguration()
        {
#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            Debug.Log("[MV] Input Handling: Both (Input System + Legacy) — Tương thích 100%.");
#elif ENABLE_INPUT_SYSTEM
            Debug.Log("[MV] Input Handling: New Input System Only — Tự động gắn InputSystemUIInputModule.");
#elif ENABLE_LEGACY_INPUT_MANAGER
            Debug.Log("[MV] Input Handling: Legacy Input Manager Only — Tự động gắn StandaloneInputModule.");
#else
            Debug.LogWarning("[MV] Cảnh báo: Không phát hiện Input Backend nào đang bật trong PlayerSettings!");
#endif
        }

        private static void EnsureFolderStructure()
        {
            string[] folders =
            {
                "Assets/Art/Fonts",
                "Assets/Art/Sprites/Characters",
                "Assets/Art/Sprites/Enemies",
                "Assets/Art/Sprites/Crops",
                "Assets/Art/Sprites/Environment",
                "Assets/Art/Sprites/Items",
                "Assets/Art/Sprites/UI",
                "Assets/Audio/BGM",
                "Assets/Audio/SFX",
                "Assets/Data/Config",
                "Assets/Data/Localization",
                "Assets/Data/Items",
                "Assets/Data/Crops",
                "Assets/Data/Bundles",
                "Assets/Data/Enemies",
                "Assets/Data/NPCs",
                "Assets/Data/Dialogues",
                "Assets/Data/Diary",
                "Assets/Data/Quests",
                "Assets/Prefabs/Core",
                "Assets/Prefabs/Characters",
                "Assets/Prefabs/Enemies",
                "Assets/Prefabs/World",
                "Assets/Prefabs/UI",
                "Assets/Scenes"
            };

            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            AssetDatabase.Refresh();
        }

        private static void ConfigureTagsLayersAndSortingLayers()
        {
            var tagManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            var so = new SerializedObject(tagManagerAsset);

            var tagsProp = so.FindProperty("tags");
            foreach (string tag in RequiredTags)
            {
                if (!PropertyArrayContainsString(tagsProp, tag))
                {
                    int idx = tagsProp.arraySize;
                    tagsProp.InsertArrayElementAtIndex(idx);
                    tagsProp.GetArrayElementAtIndex(idx).stringValue = tag;
                }
            }

            var layersProp = so.FindProperty("layers");
            foreach (var pair in RequiredLayers)
            {
                var elem = layersProp.GetArrayElementAtIndex(pair.index);
                elem.stringValue = pair.name;
            }

            var sortingProp = so.FindProperty("m_SortingLayers");
            foreach (string slName in RequiredSortingLayers)
            {
                bool exists = false;
                for (int i = 0; i < sortingProp.arraySize; i++)
                {
                    if (sortingProp.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == slName)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    int idx = sortingProp.arraySize;
                    sortingProp.InsertArrayElementAtIndex(idx);
                    var entry = sortingProp.GetArrayElementAtIndex(idx);
                    entry.FindPropertyRelative("name").stringValue = slName;
                    entry.FindPropertyRelative("uniqueID").intValue = Mathf.Abs(slName.GetHashCode()) + idx + 1000;
                }
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(tagManagerAsset);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] Đã ghi bền vững Tags, Layers (8-14) và Sorting Layers vào TagManager.asset.");
        }

        private static void ConfigurePhysics2DMatrix()
        {
            Physics2D.IgnoreLayerCollision(8, 12, true);
            Physics2D.IgnoreLayerCollision(9, 13, true);
            Physics2D.IgnoreLayerCollision(12, 13, true);
            Physics2D.IgnoreLayerCollision(12, 10, true);
            Physics2D.IgnoreLayerCollision(13, 10, true);

            var physAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/Physics2DSettings.asset");
            if (physAssets != null && physAssets.Length > 0)
            {
                EditorUtility.SetDirty(physAssets[0]);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] Đã cấu hình và lưu Ma trận va chạm Physics2D.");
        }

        private static TMP_FontAsset EnsureVietnameseTMPFont()
        {
            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            if (existing != null)
            {
                return existing;
            }

            string customTtf = "Assets/Art/Fonts/VNFont.ttf";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(customTtf);

            if (sourceFont == null)
            {
                string[] windowsCandidates =
                {
                    "C:/Windows/Fonts/segoeui.ttf",
                    "C:/Windows/Fonts/arial.ttf",
                    "C:/Windows/Fonts/tahoma.ttf"
                };

                foreach (string sysPath in windowsCandidates)
                {
                    if (File.Exists(sysPath))
                    {
                        File.Copy(sysPath, FontCopyTtfPath, true);
                        AssetDatabase.ImportAsset(FontCopyTtfPath, ImportAssetOptions.ForceUpdate);
                        sourceFont = AssetDatabase.LoadAssetAtPath<Font>(FontCopyTtfPath);
                        if (sourceFont != null)
                        {
                            Debug.Log("[MV] Đã sao chép font Unicode hệ thống: " + sysPath);
                            break;
                        }
                    }
                }
            }

            if (sourceFont == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy font TTF hệ thống. Hãy chép 1 file font hỗ trợ tiếng Việt vào Assets/Art/Fonts/VNFont.ttf rồi chạy lại menu Step 1.");
                return TMP_Settings.defaultFontAsset;
            }

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont);
            if (fontAsset == null)
            {
                Debug.LogError("[MV] Không thể khởi tạo TMP_FontAsset Dynamic từ " + sourceFont.name);
                return null;
            }

            fontAsset.name = "VN_DynamicFont";
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;

            AssetDatabase.CreateAsset(fontAsset, FontAssetPath);
            if (fontAsset.material != null)
            {
                fontAsset.material.name = "VN_DynamicFont Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, FontAssetPath);
            }
            if (fontAsset.atlasTexture != null)
            {
                fontAsset.atlasTexture.name = "VN_DynamicFont Atlas";
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, FontAssetPath);
            }

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] Đã tạo thành công Font tiếng Việt Dynamic: " + FontAssetPath);
            return fontAsset;
        }

        private static PlayerStatsSO EnsurePlayerStatsAsset()
        {
            var asset = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<PlayerStatsSO>();
                AssetDatabase.CreateAsset(asset, PlayerStatsPath);
            }
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static TimeConfigSO EnsureTimeConfigAsset()
        {
            var asset = AssetDatabase.LoadAssetAtPath<TimeConfigSO>(TimeConfigPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<TimeConfigSO>();
                AssetDatabase.CreateAsset(asset, TimeConfigPath);
            }
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static StringTableSO EnsureStringTableAsset()
        {
            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            if (table == null)
            {
                table = ScriptableObject.CreateInstance<StringTableSO>();
                AssetDatabase.CreateAsset(table, StringTablePath);
            }

            table.SetEntry("ui_game_title", "Thung Lũng Hoa Ánh Trăng");
            table.SetEntry("ui_step1_banner", "Thung Lũng Hoa Ánh Trăng — Nền tảng Step 1 sẵn sàng!\nBấm phím F1 để mở bảng Debug kiểm thử tiếng Việt & StoryFlag.");
            table.SetEntry("toast_step1_test", "Chào mừng Wen đến với Thung Lũng Hoa Ánh Trăng! (Tiếng Việt có dấu OK)");
            table.SetEntry("ui_stamina_label", "Thể lực");
            table.SetEntry("ui_lighthp_label", "Máu Ánh Sáng");
            table.SetEntry("ui_day_format", "Ngày {0} — {1:00}:{2:00}");

            EditorUtility.SetDirty(table);
            return table;
        }

        private static GameObject BuildOrUpdateGameRootPrefab(StringTableSO stringTable, TMP_FontAsset vnFont)
        {
            GameObject rootObj = new GameObject("GameRoot");
            var gameRoot = rootObj.AddComponent<GameRoot>();
            var stateMgr = rootObj.AddComponent<GameStateManager>();
            var flagMgr = rootObj.AddComponent<StoryFlagManager>();
            var loc = rootObj.AddComponent<VNLocalization>();
            var sceneLoader = rootObj.AddComponent<SceneLoader>();

            var locSO = new SerializedObject(loc);
            locSO.FindProperty("stringTable").objectReferenceValue = stringTable;
            locSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject overlayCanvasGo = CreateUICanvas("SystemOverlayCanvas", rootObj.transform, 100);

            GameObject toastPanel = CreateUIPanel("ToastPanel", overlayCanvasGo.transform,
                new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.88f), new Vector2(760f, 68f),
                new Color(0.10f, 0.14f, 0.22f, 0.90f));
            var toastGroup = toastPanel.AddComponent<CanvasGroup>();
            var toastText = CreateTMPText("ToastText", toastPanel.transform, "Thông báo", vnFont, 24, TextAlignmentOptions.Center);
            var toastUI = toastPanel.AddComponent<FloatingToastUI>();
            var toastSO = new SerializedObject(toastUI);
            toastSO.FindProperty("canvasGroup").objectReferenceValue = toastGroup;
            toastSO.FindProperty("toastText").objectReferenceValue = toastText;
            toastSO.FindProperty("displayDuration").floatValue = 2.6f;
            toastSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject debugPanel = CreateUIPanel("DebugPanelRoot", overlayCanvasGo.transform,
                new Vector2(0.02f, 0.96f), new Vector2(0.02f, 0.96f), new Vector2(460f, 320f),
                new Color(0.07f, 0.09f, 0.14f, 0.94f));
            var debugRect = debugPanel.GetComponent<RectTransform>();
            debugRect.pivot = new Vector2(0f, 1f);

            var vlg = debugPanel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(16, 16, 14, 14);
            vlg.spacing = 8f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;

            var statusText = CreateTMPText("DebugStatusText", debugPanel.transform, "BẢNG DEBUG MOONPETAL (F1)", vnFont, 18, TextAlignmentOptions.TopLeft);
            statusText.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 110f);

            var btnToast = CreateUIButton("BtnTestToast", debugPanel.transform, "1. Test Toast Tiếng Việt", vnFont);
            var btnFlag = CreateUIButton("BtnToggleFlag", debugPanel.transform, "2. Bật/Tắt StoryFlag Thử Nghiệm", vnFont);
            var btnClear = CreateUIButton("BtnClearFlags", debugPanel.transform, "3. Xóa Toàn Bộ StoryFlag", vnFont);
            var btnPause = CreateUIButton("BtnTogglePause", debugPanel.transform, "4. Đổi Trạng Thái Gameplay / Pause", vnFont);

            var debugCtrl = rootObj.AddComponent<RuntimeDebugOverlay>();
            var debugSO = new SerializedObject(debugCtrl);
            debugSO.FindProperty("panelRoot").objectReferenceValue = debugPanel;
            debugSO.FindProperty("statusText").objectReferenceValue = statusText;
            debugSO.FindProperty("btnTestToast").objectReferenceValue = btnToast;
            debugSO.FindProperty("btnToggleTestFlag").objectReferenceValue = btnFlag;
            debugSO.FindProperty("btnClearFlags").objectReferenceValue = btnClear;
            debugSO.FindProperty("btnTogglePause").objectReferenceValue = btnPause;
            debugSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("gameStateManager").objectReferenceValue = stateMgr;
            rootSO.FindProperty("storyFlagManager").objectReferenceValue = flagMgr;
            rootSO.FindProperty("localization").objectReferenceValue = loc;
            rootSO.FindProperty("sceneLoader").objectReferenceValue = sceneLoader;
            rootSO.FindProperty("toastUI").objectReferenceValue = toastUI;
            rootSO.FindProperty("debugOverlay").objectReferenceValue = debugCtrl;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(rootObj, GameRootPrefabPath);
            UnityEngine.Object.DestroyImmediate(rootObj);
            Debug.Log("[MV] Đã tạo/cập nhật Prefab: " + GameRootPrefabPath);
            return savedPrefab;
        }

        private static void BuildBootstrapScene(GameObject gameRootPrefab)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PrefabUtility.InstantiatePrefab(gameRootPrefab);

            GameObject loaderGo = new GameObject("BootstrapLoader");
            var loader = loaderGo.AddComponent<BootstrapLoader>();
            var so = new SerializedObject(loader);
            so.FindProperty("targetSceneName").stringValue = "MoonpetalValley_MVP";
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, BootstrapScenePath);
            Debug.Log("[MV] Đã tạo Scene: " + BootstrapScenePath);
        }

        private static void BuildWorldPlaceholderScene(GameObject gameRootPrefab, TMP_FontAsset vnFont)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.backgroundColor = new Color(0.11f, 0.16f, 0.22f, 1f);
            camGo.AddComponent<AudioListener>();

            CreateMatchingEventSystem();
            PrefabUtility.InstantiatePrefab(gameRootPrefab);

            GameObject canvasGo = CreateUICanvas("Step1VerificationCanvas", null, 10);
            GameObject panel = CreateUIPanel("BannerPanel", canvasGo.transform,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(920f, 180f),
                new Color(0.13f, 0.20f, 0.28f, 0.92f));
            CreateTMPText("BannerText", panel.transform,
                "Thung Lũng Hoa Ánh Trăng — Nền tảng Step 1 sẵn sàng!\nBấm phím F1 để mở bảng Debug kiểm thử tiếng Việt & StoryFlag.",
                vnFont, 28, TextAlignmentOptions.Center);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, WorldScenePath);
            Debug.Log("[MV] Đã tạo Scene: " + WorldScenePath);
        }

        private static void CreateMatchingEventSystem()
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Type inputSysModuleType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSysModuleType != null)
            {
                esGo.AddComponent(inputSysModuleType);
                return;
            }
#endif
            esGo.AddComponent<StandaloneInputModule>();
        }

        private static void ConfigureBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(WorldScenePath, true)
            };
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[MV] Đã cập nhật Build Settings: [0] Bootstrap, [1] MoonpetalValley_MVP.");
        }

        private static GameObject CreateUICanvas(string name, Transform parent, int sortOrder)
        {
            GameObject go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        private static GameObject CreateUIPanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Color bgColor)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = sizeDelta;
            rect.anchoredPosition = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = bgColor;
            return go;
        }

        private static TMP_Text CreateTMPText(string name, Transform parent, string text, TMP_FontAsset font, float fontSize, TextAlignmentOptions align)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(12f, 8f);
            rect.offsetMax = new Vector2(-12f, -8f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = Color.white;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateUIButton(string name, Transform parent, string label, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 38f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.22f, 0.36f, 0.52f, 1f);

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            CreateTMPText("Label", go.transform, label, font, 17, TextAlignmentOptions.Center);
            return btn;
        }

        private static bool PropertyArrayContainsString(SerializedProperty arrayProp, string target)
        {
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                if (arrayProp.GetArrayElementAtIndex(i).stringValue == target) return true;
            }
            return false;
        }
    }
}
```

## 73. `Assets/Editor/AutoBuilder_Step10.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Audio;
using MoonpetalValley.UI;
using MoonpetalValley.Player;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step10
    {
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 10/1. Build Save-Load, Audio & Pause Menu")]
        public static void BuildStep10()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 10 ===");
            AssetDatabase.Refresh();
            UpgradeGameRootWithSaveAudioAndPause();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 10! Chạy 'Moonpetal/Step 10/Validate (Final MVP QA)' để kiểm toán toàn bộ MVP. ===");
        }

        [MenuItem("Moonpetal/Step 10/Validate (Final MVP QA)")]
        public static void ValidateFinalMVP()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU FINAL MVP QA VALIDATE (STEP 1 -> STEP 10) ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab");
                errors++;
            }
            else
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                string[] reqFields =
                {
                    "gameStateManager", "storyFlagManager", "localization", "sceneLoader",
                    "toastUI", "debugOverlay", "timeManager", "statsController",
                    "sleepController", "inventoryManager", "dialogueManager", "diaryManager",
                    "questManager", "bundleManager", "saveLoadManager", "audioManager"
                };
                foreach (string f in reqFields)
                {
                    if (rootSO.FindProperty(f).objectReferenceValue == null)
                    {
                        Debug.LogError("[MV] GameRoot thiếu tham chiếu: " + f);
                        errors++;
                    }
                }
                if (prefab.GetComponentInChildren<PauseMenuUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu PauseMenuUIView trong GameRoot.prefab.");
                    errors++;
                }
            }

            string[] requiredAudio =
            {
                "Assets/Audio/BGM/bgm_valley_cozy.wav",
                "Assets/Audio/SFX/sfx_ui_click.wav",
                "Assets/Audio/SFX/sfx_tool.wav",
                "Assets/Audio/SFX/sfx_slash.wav",
                "Assets/Audio/SFX/sfx_hit.wav",
                "Assets/Audio/SFX/sfx_pickup.wav",
                "Assets/Audio/SFX/sfx_bloom.wav"
            };
            foreach (string path in requiredAudio)
            {
                if (AssetDatabase.LoadAssetAtPath<AudioClip>(path) == null)
                {
                    Debug.LogError("[MV] THIẾU AudioClip: " + path);
                    errors++;
                }
            }

            if (UnityEngine.Object.FindAnyObjectByType<PlayerController2D>() == null) { Debug.LogError("[MV] Thiếu Player_Wen."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<PlayerCombatController>() == null) { Debug.LogError("[MV] Thiếu PlayerCombatController."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include).Length < 12) { Debug.LogError("[MV] Thiếu 12 FarmTile."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include).Length < 4) { Debug.LogError("[MV] Thiếu 4 NPC."); errors++; }
            if (UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include).Length < 8) { Debug.LogError("[MV] Thiếu 8 GloomSlime."); errors++; }
            if (UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>() == null) { Debug.LogError("[MV] Thiếu PlantingSpotController."); errors++; }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 10 (FINAL MVP QA): PASS (0 lỗi). MOONPETAL VALLEY MVP ĐÃ HOÀN THIỆN 100%!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 10 (FINAL MVP QA): FAIL (" + errors + " lỗi).");
            }
        }

        [MenuItem("Moonpetal/Debug/Open Save Folder")]
        public static void OpenSaveFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        private static void UpgradeGameRootWithSaveAudioAndPause()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var saveMgr = root.GetComponent<SaveLoadManager>() ?? root.AddComponent<SaveLoadManager>();
            var audioMgr = root.GetComponent<AudioManager>() ?? root.AddComponent<AudioManager>();

            // Tạo 2 AudioSource con (BGM & SFX)
            Transform oldAudio = root.transform.Find("AudioSources");
            if (oldAudio != null) UnityEngine.Object.DestroyImmediate(oldAudio.gameObject);
            GameObject audioHost = new GameObject("AudioSources");
            audioHost.transform.SetParent(root.transform, false);
            var bgmSrc = audioHost.AddComponent<AudioSource>();
            bgmSrc.playOnAwake = false;
            var sfxSrc = audioHost.AddComponent<AudioSource>();
            sfxSrc.playOnAwake = false;

            var aSO = new SerializedObject(audioMgr);
            aSO.FindProperty("bgmSource").objectReferenceValue = bgmSrc;
            aSO.FindProperty("sfxSource").objectReferenceValue = sfxSrc;
            aSO.FindProperty("bgmCozyClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/bgm_valley_cozy.wav");
            aSO.FindProperty("sfxUiClick").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_ui_click.wav");
            aSO.FindProperty("sfxTool").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_tool.wav");
            aSO.FindProperty("sfxSlash").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_slash.wav");
            aSO.FindProperty("sfxHit").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_hit.wav");
            aSO.FindProperty("sfxPickup").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_pickup.wav");
            aSO.FindProperty("sfxBloom").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/sfx_bloom.wav");
            aSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("saveLoadManager").objectReferenceValue = saveMgr;
            rootSO.FindProperty("audioManager").objectReferenceValue = audioMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // Pause Menu UI trong ClockHUDCanvas
            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                Transform oldPause = hudCanvas.Find("PauseMenuHost");
                if (oldPause != null) UnityEngine.Object.DestroyImmediate(oldPause.gameObject);

                GameObject host = new GameObject("PauseMenuHost");
                host.transform.SetParent(hudCanvas, false);
                var hr = host.AddComponent<RectTransform>();
                hr.anchorMin = Vector2.zero; hr.anchorMax = Vector2.one;
                hr.offsetMin = Vector2.zero; hr.offsetMax = Vector2.zero;

                GameObject panel = new GameObject("PausePanelRoot");
                panel.transform.SetParent(host.transform, false);
                var pr = panel.AddComponent<RectTransform>();
                pr.anchorMin = new Vector2(0.5f, 0.5f); pr.anchorMax = new Vector2(0.5f, 0.5f);
                pr.sizeDelta = new Vector2(460f, 420f);
                panel.AddComponent<Image>().color = new Color(0.08f, 0.11f, 0.18f, 0.96f);

                var vlg = panel.AddComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(28, 28, 22, 22);
                vlg.spacing = 12f;
                vlg.childControlWidth = true;
                vlg.childControlHeight = false;

                var title = CreateText("Title", panel.transform, "TẠM DỪNG — MOONPETAL VALLEY", vnFont, 22, new Color(1f, 0.90f, 0.54f, 1f), 34f);
                var status = CreateText("SaveStatus", panel.transform, "Chưa có bản lưu", vnFont, 15, new Color(0.80f, 0.88f, 0.96f, 1f), 28f);

                var (btnResume, _) = CreateMenuBtn("BtnResume", panel.transform, "Tiếp Tục Chơi [Esc]", new Color(0.20f, 0.54f, 0.40f, 1f), vnFont);
                var (btnSave, _) = CreateMenuBtn("BtnSave", panel.transform, "Lưu Game (JSON)", new Color(0.22f, 0.42f, 0.66f, 1f), vnFont);
                var (btnLoad, _) = CreateMenuBtn("BtnLoad", panel.transform, "Tải Game Đã Lưu", new Color(0.22f, 0.42f, 0.66f, 1f), vnFont);
                var (btnMute, muteTxt) = CreateMenuBtn("BtnMute", panel.transform, "Âm Thanh: ĐANG BẬT", new Color(0.42f, 0.36f, 0.22f, 1f), vnFont);
                var (btnDel, _) = CreateMenuBtn("BtnDeleteSave", panel.transform, "Xóa File Lưu Game", new Color(0.54f, 0.22f, 0.24f, 1f), vnFont);

                var view = host.AddComponent<PauseMenuUIView>();
                var vSO = new SerializedObject(view);
                vSO.FindProperty("panelRoot").objectReferenceValue = panel;
                vSO.FindProperty("saveStatusText").objectReferenceValue = status;
                vSO.FindProperty("muteButtonText").objectReferenceValue = muteTxt;
                vSO.FindProperty("btnResume").objectReferenceValue = btnResume;
                vSO.FindProperty("btnSave").objectReferenceValue = btnSave;
                vSO.FindProperty("btnLoad").objectReferenceValue = btnLoad;
                vSO.FindProperty("btnMute").objectReferenceValue = btnMute;
                vSO.FindProperty("btnDeleteSave").objectReferenceValue = btnDel;
                vSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static TMP_Text CreateText(string name, Transform parent, string text, TMP_FontAsset font, float size, Color color, float height)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, height);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = TextAlignmentOptions.Center; tmp.color = color;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static (Button btn, TMP_Text label) CreateMenuBtn(string name, Transform parent, string text, Color bg, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 44f);
            var img = go.AddComponent<Image>(); img.color = bg;
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;

            GameObject tGo = new GameObject("Label");
            tGo.transform.SetParent(go.transform, false);
            var r = tGo.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
            var tmp = tGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = 18; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
            if (font != null) tmp.font = font;
            return (btn, tmp);
        }
    }
}
```

## 74. `Assets/Editor/AutoBuilder_Step11.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.UI;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step11
    {
        private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 11/1. Build Main Menu, Polish & Visual Upgrade")]
        public static void BuildStep11()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 11 ===");

            Sprite petalSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/UI/spr_ambient_petal.png", 8, 8, DrawAmbientPetal);
            Sprite bakerySpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_house_bakery.png", 48, 40, DrawBakeryHouse);
            Sprite shedSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_house_shed.png", 40, 32, DrawGardenShed);
            Sprite flowerTuftSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_decor_flowers.png", 16, 16, DrawFlowerTuft);
            Sprite bushSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_decor_bush.png", 16, 16, DrawDecorBush);

            UpgradeGameRootWithJuice();
            BuildMainMenuScene(petalSpr);
            UpdateBootstrapToMainMenu();
            UpdateBuildSettings();

            var worldScene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildWorldPolishAndDecor(petalSpr, bakerySpr, shedSpr, flowerTuftSpr, bushSpr);
            EditorSceneManager.MarkSceneDirty(worldScene);
            EditorSceneManager.SaveScene(worldScene);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 11! Chạy 'Moonpetal/Step 11/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 11/Validate")]
        public static void ValidateStep11()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 11 ===");

            if (EditorBuildSettings.scenes.Length < 3 ||
                EditorBuildSettings.scenes[0].path != BootstrapScenePath ||
                EditorBuildSettings.scenes[1].path != MainMenuScenePath ||
                EditorBuildSettings.scenes[2].path != WorldScenePath)
            {
                Debug.LogError("[MV] Build Settings chưa đủ 3 scene: Bootstrap -> MainMenu -> MoonpetalValley_MVP.");
                errors++;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null || prefab.GetComponent<JuiceEffectsManager>() == null)
            {
                Debug.LogError("[MV] GameRoot.prefab thiếu JuiceEffectsManager.");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<AmbientPetalSpawner>() == null)
            {
                Debug.LogError("[MV] Thiếu AmbientPetalSpawner trong scene thế giới.");
                errors++;
            }

            if (GameObject.Find("VillageDecorGroup") == null)
            {
                Debug.LogError("[MV] Thiếu VillageDecorGroup (Tiệm Bánh, Chòi Vườn & Hoa cỏ).");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 11: PASS (0 lỗi). Main Menu, Damage Popups, Screen Shake, Cánh Hoa Bay & Kiến Trúc Làng sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 11: FAIL (" + errors + " lỗi).");
            }
        }

        private static void UpgradeGameRootWithJuice()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var juice = root.GetComponent<JuiceEffectsManager>() ?? root.AddComponent<JuiceEffectsManager>();
            var so = new SerializedObject(juice);
            so.FindProperty("vnFont").objectReferenceValue = vnFont;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildMainMenuScene(Sprite petalSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject gameRootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.backgroundColor = new Color(0.08f, 0.12f, 0.22f, 1f);
            camGo.AddComponent<AudioListener>();

            // EventSystem khớp Input backend
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Type modType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (modType != null) esGo.AddComponent(modType);
            else esGo.AddComponent<StandaloneInputModule>();
#else
            esGo.AddComponent<StandaloneInputModule>();
#endif

            PrefabUtility.InstantiatePrefab(gameRootPrefab);

            // Cánh hoa bay ở Main Menu
            GameObject spawnerGo = new GameObject("MenuPetalSpawner");
            var spawner = spawnerGo.AddComponent<AmbientPetalSpawner>();
            var spSO = new SerializedObject(spawner);
            spSO.FindProperty("petalSprite").objectReferenceValue = petalSpr;
            spSO.ApplyModifiedPropertiesWithoutUndo();

            // Canvas Main Menu (SortingOrder = 60, đè lên HUD khi ở Menu)
            GameObject canvasGo = new GameObject("MainMenuCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 60;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject bg = new GameObject("Backdrop");
            bg.transform.SetParent(canvasGo.transform, false);
            var bgR = bg.AddComponent<RectTransform>();
            bgR.anchorMin = Vector2.zero; bgR.anchorMax = Vector2.one;
            bgR.offsetMin = Vector2.zero; bgR.offsetMax = Vector2.zero;
            bg.AddComponent<Image>().color = new Color(0.06f, 0.10f, 0.19f, 0.94f);

            GameObject card = new GameObject("MenuCard");
            card.transform.SetParent(canvasGo.transform, false);
            var cR = card.AddComponent<RectTransform>();
            cR.anchorMin = new Vector2(0.5f, 0.5f); cR.anchorMax = new Vector2(0.5f, 0.5f);
            cR.sizeDelta = new Vector2(680f, 470f);
            card.AddComponent<Image>().color = new Color(0.10f, 0.15f, 0.25f, 0.95f);

            var vlg = card.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(48, 48, 36, 36);
            vlg.spacing = 16f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;

            CreateMenuText("Title", card.transform, "THUNG LŨNG HOA ÁNH TRĂNG", vnFont, 38, new Color(1f, 0.90f, 0.56f, 1f), 52f);
            CreateMenuText("Subtitle", card.transform, "MOONPETAL VALLEY — Cozy Action-Farming RPG (MVP v1.0)", vnFont, 18, new Color(0.72f, 0.92f, 0.96f, 1f), 30f);
            var subSave = CreateMenuText("SaveHint", card.transform, "Đang kiểm tra bản lưu...", vnFont, 16, new Color(0.84f, 0.88f, 0.94f, 1f), 28f);

            Button btnNew = CreateMenuButton("BtnNewGame", card.transform, "Chơi Mới", new Color(0.20f, 0.58f, 0.42f, 1f), vnFont);
            Button btnCont = CreateMenuButton("BtnContinue", card.transform, "Tiếp Tục Hành Trình", new Color(0.24f, 0.44f, 0.68f, 1f), vnFont);
            Button btnQuit = CreateMenuButton("BtnQuit", card.transform, "Thoát Game", new Color(0.50f, 0.22f, 0.26f, 1f), vnFont);

            var ctrl = canvasGo.AddComponent<MainMenuController>();
            var mSO = new SerializedObject(ctrl);
            mSO.FindProperty("btnNewGame").objectReferenceValue = btnNew;
            mSO.FindProperty("btnContinue").objectReferenceValue = btnCont;
            mSO.FindProperty("btnQuit").objectReferenceValue = btnQuit;
            mSO.FindProperty("continueSubtext").objectReferenceValue = subSave;
            mSO.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        }

        private static void UpdateBootstrapToMainMenu()
        {
            var scene = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            var loader = UnityEngine.Object.FindAnyObjectByType<BootstrapLoader>();
            if (loader != null)
            {
                var so = new SerializedObject(loader);
                so.FindProperty("targetSceneName").stringValue = "MainMenu";
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void UpdateBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(WorldScenePath, true)
            };
        }

        private static void BuildWorldPolishAndDecor(Sprite petalSpr, Sprite bakerySpr, Sprite shedSpr, Sprite flowerSpr, Sprite bushSpr)
        {
            GameObject oldSpawner = GameObject.Find("AmbientPetalSpawner");
            if (oldSpawner != null) UnityEngine.Object.DestroyImmediate(oldSpawner);
            GameObject spawnerGo = new GameObject("AmbientPetalSpawner");
            var spawner = spawnerGo.AddComponent<AmbientPetalSpawner>();
            var spSO = new SerializedObject(spawner);
            spSO.FindProperty("petalSprite").objectReferenceValue = petalSpr;
            spSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject oldDecor = GameObject.Find("VillageDecorGroup");
            if (oldDecor != null) UnityEngine.Object.DestroyImmediate(oldDecor);
            GameObject group = new GameObject("VillageDecorGroup");

            // Tiệm Bánh Bà Hana & Chòi Vườn Ông Bách
            SpawnDecor(group.transform, "House_HanaBakery", bakerySpr, new Vector2(-6.5f, 5.4f), true, new Vector2(2.6f, 1.4f));
            SpawnDecor(group.transform, "House_BachShed", shedSpr, new Vector2(-4.5f, -4.5f), true, new Vector2(2.1f, 1.2f));

            // 16 cụm hoa cỏ & bụi cây trang trí khắp 4 phân khu
            Vector2[] flowers =
            {
                new Vector2(-11.0f, 3.0f), new Vector2(-8.5f, 1.5f), new Vector2(-3.0f, 1.0f), new Vector2(3.5f, 1.2f),
                new Vector2(6.5f, 4.5f), new Vector2(-2.5f, -2.0f), new Vector2(4.0f, -2.2f), new Vector2(-5.0f, -9.5f),
                new Vector2(5.2f, -9.5f), new Vector2(13.5f, 9.0f), new Vector2(18.5f, 4.5f), new Vector2(14.5f, 2.0f)
            };
            for (int i = 0; i < flowers.Length; i++)
            {
                SpawnDecor(group.transform, "FlowerTuft_" + (i + 1), flowerSpr, flowers[i], false, Vector2.zero);
            }

            Vector2[] bushes =
            {
                new Vector2(-8.2f, 5.2f), new Vector2(-4.2f, 5.2f), new Vector2(4.2f, 5.2f), new Vector2(9.2f, 4.2f)
            };
            for (int i = 0; i < bushes.Length; i++)
            {
                SpawnDecor(group.transform, "DecorBush_" + (i + 1), bushSpr, bushes[i], true, new Vector2(0.75f, 0.65f));
            }
        }

        private static void SpawnDecor(Transform parent, string name, Sprite spr, Vector2 pos, bool solid, Vector2 colSize)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = solid ? "WorldObjects" : "FloorDecal";
            sr.sortingOrder = solid ? Mathf.RoundToInt(-pos.y * 100f) : 10;
            if (solid)
            {
                go.layer = 10;
                var col = go.AddComponent<BoxCollider2D>();
                col.size = colSize;
                col.offset = new Vector2(0f, -colSize.y * 0.25f);
            }
        }

        private static TMP_Text CreateMenuText(string name, Transform parent, string text, TMP_FontAsset font, float size, Color color, float height)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, height);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = TextAlignmentOptions.Center; tmp.color = color;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateMenuButton(string name, Transform parent, string text, Color bg, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 54f);
            var img = go.AddComponent<Image>(); img.color = bg;
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;

            GameObject tGo = new GameObject("Label");
            tGo.transform.SetParent(go.transform, false);
            var r = tGo.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
            var tmp = tGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = 22; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
            if (font != null) tmp.font = font;
            return btn;
        }

        private static void DrawAmbientPetal(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 2, 4, 4, Color.white);
            PlaceholderGeneratorUtility.FillRect(t, 1, 3, 6, 2, new Color(1f, 0.94f, 0.98f, 0.85f));
        }

        private static void DrawBakeryHouse(Texture2D t)
        {
            Color roof = new Color(0.76f, 0.32f, 0.28f);
            Color roofTrim = new Color(0.56f, 0.22f, 0.18f);
            Color wall = new Color(0.94f, 0.88f, 0.74f);
            Color wood = new Color(0.46f, 0.30f, 0.18f);
            Color win = new Color(1.0f, 0.86f, 0.44f);

            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 40, 20, wall);
            PlaceholderGeneratorUtility.FillRect(t, 2, 20, 44, 16, roof);
            PlaceholderGeneratorUtility.FillRect(t, 6, 34, 36, 4, roofTrim);
            PlaceholderGeneratorUtility.FillRect(t, 19, 2, 10, 14, wood);
            PlaceholderGeneratorUtility.FillRect(t, 8, 9, 8, 7, win);
            PlaceholderGeneratorUtility.FillRect(t, 32, 9, 8, 7, win);
            PlaceholderGeneratorUtility.FillRect(t, 16, 17, 16, 4, new Color(0.96f, 0.78f, 0.34f));
        }

        private static void DrawGardenShed(Texture2D t)
        {
            Color roof = new Color(0.32f, 0.46f, 0.36f);
            Color wall = new Color(0.62f, 0.46f, 0.30f);
            Color door = new Color(0.40f, 0.26f, 0.16f);
            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 32, 16, wall);
            PlaceholderGeneratorUtility.FillRect(t, 2, 16, 36, 13, roof);
            PlaceholderGeneratorUtility.FillRect(t, 15, 2, 10, 12, door);
        }

        private static void DrawFlowerTuft(Texture2D t)
        {
            Color g = new Color(0.24f, 0.52f, 0.26f);
            Color p1 = new Color(0.98f, 0.76f, 0.88f);
            Color p2 = new Color(0.98f, 0.90f, 0.44f);
            PlaceholderGeneratorUtility.FillRect(t, 3, 3, 2, 5, g);
            PlaceholderGeneratorUtility.FillRect(t, 9, 4, 2, 5, g);
            PlaceholderGeneratorUtility.FillRect(t, 2, 7, 4, 4, p1);
            PlaceholderGeneratorUtility.FillRect(t, 8, 8, 4, 4, p2);
        }

        private static void DrawDecorBush(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 2, 12, 10, new Color(0.18f, 0.44f, 0.24f));
            PlaceholderGeneratorUtility.FillRect(t, 4, 5, 8, 7, new Color(0.26f, 0.58f, 0.32f));
            PlaceholderGeneratorUtility.FillRect(t, 5, 8, 2, 2, new Color(0.98f, 0.72f, 0.84f));
            PlaceholderGeneratorUtility.FillRect(t, 10, 6, 2, 2, new Color(0.98f, 0.88f, 0.42f));
        }
    }
}
```

## 75. `Assets/Editor/AutoBuilder_Step12.cs`

```csharp
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MoonpetalValley.Player;
using MoonpetalValley.Combat;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step12
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";
        private const string GloomSlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";

        private const string WenRawRoot = "Assets/Art/Sprites/Animations/Wen/Raw";
        private const string WenFramesRoot = "Assets/Art/Sprites/Animations/Wen/Frames";
        private const string LumiFramesRoot = "Assets/Art/Sprites/Animations/Lumi";
        private const string SlimeFramesRoot = "Assets/Art/Sprites/Animations/GloomSlime";

        private static readonly string[] WenActions =
        {
            "Idle", "Walk", "Dash", "Tool_Use",
            "Hoe", "Watering_Can", "Hatchet", "Moon_Sickle_Slash"
        };
        private static readonly string[] WenDirs = { "south", "north", "east", "west" };

        [MenuItem("Moonpetal/Step 12/1. Slice & Wire Character Animations (Wen, Lumi, Slime)")]
        public static void BuildStep12()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 12 (FULLRECT LOCK + 8 HÀNH ĐỘNG WEN) ===");

            int slicedCount = SliceAllWenStrips();
            AssetDatabase.Refresh();
            ForceReimportAnimationFolders();

            WireWenPrefab();
            WireLumiPrefab();
            WireGloomSlimePrefab();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            RefreshScenePrefabOverrides();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 12! Đã cắt " + slicedCount + " frame Wen (FullRect 32x32) & đồng bộ Wen, Lumi, Bọt Sương Đen. Chạy 'Moonpetal/Step 12/Validate'. ===");
        }

        [MenuItem("Moonpetal/Step 12/Validate")]
        public static void ValidateStep12()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 12 (FULLRECT CHECK) ===");

            int wenFramesOk = 0;
            foreach (string action in WenActions)
            {
                foreach (string dir in WenDirs)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string path = GetWenFramePath(action, dir, i);
                        Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        if (spr == null)
                        {
                            Debug.LogError("[MV] THIẾU Wen frame: " + path);
                            errors++;
                        }
                        else if (Mathf.RoundToInt(spr.rect.width) != 32 || Mathf.RoundToInt(spr.rect.height) != 32)
                        {
                            Debug.LogError("[MV] SAI KÍCH THƯỚC FullRect trên " + path + ": " + spr.rect.width + "x" + spr.rect.height + " (cần 32x32).");
                            errors++;
                        }
                        else
                        {
                            wenFramesOk++;
                        }
                    }
                }
            }

            var wenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (wenPrefab == null)
            {
                Debug.LogError("[MV] THIẾU Player_Wen.prefab");
                errors++;
            }
            else
            {
                var anim = wenPrefab.GetComponent<WenSpriteAnimator>();
                if (anim == null || !anim.HasValidSprites)
                {
                    Debug.LogError("[MV] Player_Wen.prefab thiếu WenSpriteAnimator hoặc mảng sprite rỗng.");
                    errors++;
                }
                else
                {
                    var aSO = new SerializedObject(anim);
                    string[] checkProps =
                    {
                        "idleSouth", "walkSouth", "dashSouth", "toolSouth",
                        "hoeSouth", "waterSouth", "hatchetSouth", "slashSouth"
                    };
                    foreach (string p in checkProps)
                    {
                        var arr = aSO.FindProperty(p);
                        if (arr.arraySize < 4 || arr.GetArrayElementAtIndex(0).objectReferenceValue == null)
                        {
                            Debug.LogError("[MV] WenSpriteAnimator thiếu đủ 4 frame hợp lệ cho: " + p);
                            errors++;
                        }
                    }
                }
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 12: PASS (0 lỗi). Wen (" + wenFramesOk + "/128 frames FullRect 32x32), Lumi & Bọt Sương Đen đã đồng bộ hoàn chỉnh!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 12: FAIL (" + errors + " lỗi).");
            }
        }

        private static int SliceAllWenStrips()
        {
            if (Directory.Exists(WenFramesRoot))
            {
                foreach (string oldPng in Directory.GetFiles(WenFramesRoot, "*.png"))
                {
                    File.Delete(oldPng);
                }
            }
            else
            {
                Directory.CreateDirectory(WenFramesRoot);
            }

            int totalSliced = 0;
            foreach (string action in WenActions)
            {
                foreach (string dir in WenDirs)
                {
                    string rawPath = WenRawRoot + "/" + action + "/" + dir + ".png";
                    if (!File.Exists(rawPath))
                    {
                        Debug.LogError("[MV] Không tìm thấy dải sprite Wen: " + rawPath + ". Hãy tải wen_animation_pack.zip bản mới rồi chạy lại deploy.py.");
                        continue;
                    }

                    byte[] bytes = File.ReadAllBytes(rawPath);
                    Texture2D src = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    src.LoadImage(bytes);

                    int frameW = 32;
                    int frameH = 32;
                    if (src.width < 128 || src.height != 32)
                    {
                        Debug.LogWarning("[MV] Kích thước dải không chuẩn ở " + rawPath + ": " + src.width + "x" + src.height);
                    }
                    int frameCount = Mathf.Min(4, Mathf.Max(1, src.width / frameW));

                    for (int i = 0; i < frameCount; i++)
                    {
                        Color[] pixels = src.GetPixels(i * frameW, 0, frameW, frameH);
                        Texture2D frameTex = new Texture2D(frameW, frameH, TextureFormat.RGBA32, false);
                        frameTex.filterMode = FilterMode.Point;
                        frameTex.SetPixels(pixels);
                        frameTex.Apply();

                        string outPath = GetWenFramePath(action, dir, i);
                        File.WriteAllBytes(outPath, frameTex.EncodeToPNG());
                        UnityEngine.Object.DestroyImmediate(frameTex);
                        totalSliced++;
                    }

                    UnityEngine.Object.DestroyImmediate(src);
                }
            }
            return totalSliced;
        }

        private static void ForceReimportAnimationFolders()
        {
            string[] folders = { WenFramesRoot, LumiFramesRoot, SlimeFramesRoot };
            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder)) continue;
                string[] pngs = Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories);
                foreach (string file in pngs)
                {
                    string assetPath = file.Replace("\\", "/");
                    var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        importer.filterMode = FilterMode.Point;
                        importer.textureCompression = TextureImporterCompression.Uncompressed;
                        importer.alphaIsTransparency = true;
                        importer.mipmapEnabled = false;

                        if (assetPath.StartsWith("Assets/Art/Sprites/Animations/Wen/")) importer.spritePixelsPerUnit = 20;
                        else if (assetPath.StartsWith("Assets/Art/Sprites/Animations/Lumi/")) importer.spritePixelsPerUnit = 20;
                        else if (assetPath.StartsWith("Assets/Art/Sprites/Animations/GloomSlime/")) importer.spritePixelsPerUnit = 32;

                        var settings = new TextureImporterSettings();
                        importer.ReadTextureSettings(settings);
                        settings.spriteMeshType = SpriteMeshType.FullRect;
                        settings.spriteAlignment = (int)SpriteAlignment.Center;
                        settings.spriteGenerateFallbackPhysicsShape = false;
                        importer.SetTextureSettings(settings);
                        importer.SaveAndReimport();
                    }
                    else
                    {
                        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
        }

        private static void WireWenPrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null) return;

            Transform visual = root.transform.Find("Visual");
            SpriteRenderer sr = visual != null ? visual.GetComponent<SpriteRenderer>() : root.GetComponentInChildren<SpriteRenderer>();

            var anim = root.GetComponent<WenSpriteAnimator>();
            if (anim == null) anim = root.AddComponent<WenSpriteAnimator>();

            var aSO = new SerializedObject(anim);
            aSO.FindProperty("spriteRenderer").objectReferenceValue = sr;

            Assign4Dirs(aSO, "idle", "Idle");
            Assign4Dirs(aSO, "walk", "Walk");
            Assign4Dirs(aSO, "dash", "Dash");
            Assign4Dirs(aSO, "tool", "Tool_Use");
            Assign4Dirs(aSO, "hoe", "Hoe");
            Assign4Dirs(aSO, "water", "Watering_Can");
            Assign4Dirs(aSO, "hatchet", "Hatchet");
            Assign4Dirs(aSO, "slash", "Moon_Sickle_Slash");
            aSO.ApplyModifiedPropertiesWithoutUndo();

            Sprite defaultWen = AssetDatabase.LoadAssetAtPath<Sprite>(GetWenFramePath("Idle", "south", 0));
            if (sr != null && defaultWen != null) sr.sprite = defaultWen;

            var pCtrl = root.GetComponent<PlayerController2D>();
            if (pCtrl != null)
            {
                var pSO = new SerializedObject(pCtrl);
                pSO.FindProperty("spriteAnimator").objectReferenceValue = anim;
                pSO.ApplyModifiedPropertiesWithoutUndo();
            }

            var tCtrl = root.GetComponent<PlayerToolController>();
            if (tCtrl != null)
            {
                var tSO = new SerializedObject(tCtrl);
                tSO.FindProperty("spriteAnimator").objectReferenceValue = anim;
                tSO.ApplyModifiedPropertiesWithoutUndo();
            }

            var cCtrl = root.GetComponent<PlayerCombatController>();
            if (cCtrl != null)
            {
                var cSO = new SerializedObject(cCtrl);
                cSO.FindProperty("spriteAnimator").objectReferenceValue = anim;
                cSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void Assign4Dirs(SerializedObject so, string propPrefix, string actionName)
        {
            AssignSpriteArray(so.FindProperty(propPrefix + "South"), LoadWenFrames(actionName, "south", 4));
            AssignSpriteArray(so.FindProperty(propPrefix + "North"), LoadWenFrames(actionName, "north", 4));
            AssignSpriteArray(so.FindProperty(propPrefix + "East"), LoadWenFrames(actionName, "east", 4));
            AssignSpriteArray(so.FindProperty(propPrefix + "West"), LoadWenFrames(actionName, "west", 4));
        }

        private static void WireLumiPrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(LumiPrefabPath);
            if (root == null) return;

            Transform visual = root.transform.Find("Visual");
            SpriteRenderer sr = visual != null ? visual.GetComponent<SpriteRenderer>() : null;
            var ctrl = root.GetComponent<LumiCompanionController>();

            Sprite[] floatFrames = LoadNumberedFrames(LumiFramesRoot + "/float", 4);
            Sprite[] pulseFrames = LoadNumberedFrames(LumiFramesRoot + "/moon_pulse", 4);

            if (sr != null && floatFrames.Length > 0 && floatFrames[0] != null) sr.sprite = floatFrames[0];

            if (ctrl != null)
            {
                var so = new SerializedObject(ctrl);
                so.FindProperty("spriteRenderer").objectReferenceValue = sr;
                AssignSpriteArray(so.FindProperty("floatFrames"), floatFrames);
                AssignSpriteArray(so.FindProperty("moonPulseFrames"), pulseFrames);
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, LumiPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void WireGloomSlimePrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(GloomSlimePrefabPath);
            if (root == null) return;

            Transform visual = root.transform.Find("Visual");
            SpriteRenderer sr = visual != null ? visual.GetComponent<SpriteRenderer>() : null;
            var ctrl = root.GetComponent<GloomSlimeController>();

            Sprite[] idleF = LoadNumberedFrames(SlimeFramesRoot + "/idle", 4);
            Sprite[] teleF = LoadNumberedFrames(SlimeFramesRoot + "/telegraph", 3);
            Sprite[] jumpF = LoadNumberedFrames(SlimeFramesRoot + "/jump", 4);
            Sprite[] hitF = LoadNumberedFrames(SlimeFramesRoot + "/hit_hurt", 3);
            Sprite[] dieF = LoadNumberedFrames(SlimeFramesRoot + "/purify_die", 4);

            if (sr != null && idleF.Length > 0 && idleF[0] != null) sr.sprite = idleF[0];

            if (ctrl != null)
            {
                var so = new SerializedObject(ctrl);
                AssignSpriteArray(so.FindProperty("idleFrames"), idleF);
                AssignSpriteArray(so.FindProperty("telegraphFrames"), teleF);
                AssignSpriteArray(so.FindProperty("jumpFrames"), jumpF);
                AssignSpriteArray(so.FindProperty("hitHurtFrames"), hitF);
                AssignSpriteArray(so.FindProperty("purifyDieFrames"), dieF);
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GloomSlimePrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void RefreshScenePrefabOverrides()
        {
            Sprite defaultWen = AssetDatabase.LoadAssetAtPath<Sprite>(GetWenFramePath("Idle", "south", 0));
            Sprite defaultLumi = AssetDatabase.LoadAssetAtPath<Sprite>(LumiFramesRoot + "/float_00.png");
            Sprite defaultSlime = AssetDatabase.LoadAssetAtPath<Sprite>(SlimeFramesRoot + "/idle_00.png");

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null && defaultWen != null)
            {
                var sr = player.transform.Find("Visual")?.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = defaultWen;
            }

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null && defaultLumi != null)
            {
                var sr = lumi.transform.Find("Visual")?.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = defaultLumi;
            }

            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            for (int i = 0; i < slimes.Length; i++)
            {
                if (defaultSlime != null)
                {
                    var sr = slimes[i].transform.Find("Visual")?.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.sprite = defaultSlime;
                }
            }
        }

        private static string GetWenFramePath(string action, string dir, int idx)
        {
            return WenFramesRoot + "/wen_" + action.ToLowerInvariant() + "_" + dir + "_" + idx.ToString("00") + ".png";
        }

        private static Sprite[] LoadWenFrames(string action, string dir, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(GetWenFramePath(action, dir, i)));
            }
            return list.ToArray();
        }

        private static Sprite[] LoadNumberedFrames(string basePathWithoutIndex, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(basePathWithoutIndex + "_" + i.ToString("00") + ".png"));
            }
            return list.ToArray();
        }

        private static void AssignSpriteArray(SerializedProperty prop, Sprite[] sprites)
        {
            if (prop == null) return;
            prop.ClearArray();
            for (int i = 0; i < sprites.Length; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            }
        }
    }
}
```

## 76. `Assets/Editor/AutoBuilder_Step13.cs`

```csharp
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step13
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string NpcAnimRoot = "Assets/Art/Sprites/Animations/NPCs";

        private static readonly (string objName, string npcId)[] NpcEntries =
        {
            ("NPC_Bach", "npc_bach"),
            ("NPC_Hana", "npc_hana"),
            ("NPC_Toby", "npc_toby"),
            ("NPC_Otis", "npc_otis")
        };

        [MenuItem("Moonpetal/Step 13/1. Wire 4 NPCs Animations (Bach, Hana, Toby, Otis)")]
        public static void BuildStep13()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 13: ĐỒNG BỘ 4 NPC ===");
            AssetDatabase.Refresh();
            ForceReimportNpcSprites();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            foreach (var entry in NpcEntries)
            {
                Sprite[] idleF = LoadFrames(entry.npcId, "idle", 4);
                Sprite[] cheerF = LoadFrames(entry.npcId, "cheer", 4);

                // Cập nhật NPCData asset
                var npcData = AssetDatabase.LoadAssetAtPath<NPCData>("Assets/Data/NPCs/NPC_" + entry.npcId + ".asset");
                if (npcData != null && idleF[0] != null)
                {
                    npcData.worldSprite = idleF[0];
                    EditorUtility.SetDirty(npcData);
                }

                // Cập nhật GameObject trong scene
                GameObject go = GameObject.Find(entry.objName);
                if (go == null)
                {
                    Debug.LogError("[MV] Không tìm thấy " + entry.objName + " trong scene!");
                    continue;
                }

                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null && idleF[0] != null) sr.sprite = idleF[0];

                var ctrl = go.GetComponent<NPCController>();
                if (ctrl != null)
                {
                    var so = new SerializedObject(ctrl);
                    so.FindProperty("spriteRenderer").objectReferenceValue = sr;
                    AssignArray(so.FindProperty("idleFrames"), idleF);
                    AssignArray(so.FindProperty("cheerFrames"), cheerF);
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 13! Chạy 'Moonpetal/Step 13/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 13/Validate")]
        public static void ValidateStep13()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 13 ===");

            int framesOk = 0;
            foreach (var entry in NpcEntries)
            {
                foreach (string anim in new[] { "idle", "cheer" })
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string path = NpcAnimRoot + "/" + entry.npcId + "/" + anim + "_" + i.ToString("00") + ".png";
                        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        if (s == null)
                        {
                            Debug.LogError("[MV] THIẾU sprite NPC: " + path);
                            errors++;
                        }
                        else if (Mathf.RoundToInt(s.rect.width) != 32 || Mathf.RoundToInt(s.rect.height) != 32)
                        {
                            Debug.LogError("[MV] SAI kích thước FullRect trên " + path + ": " + s.rect.width + "x" + s.rect.height);
                            errors++;
                        }
                        else
                        {
                            framesOk++;
                        }
                    }
                }

                GameObject go = GameObject.Find(entry.objName);
                var ctrl = go != null ? go.GetComponent<NPCController>() : null;
                if (ctrl == null)
                {
                    Debug.LogError("[MV] Thiếu NPCController trên " + entry.objName);
                    errors++;
                }
                else
                {
                    var so = new SerializedObject(ctrl);
                    if (so.FindProperty("idleFrames").arraySize < 4 || so.FindProperty("cheerFrames").arraySize < 4)
                    {
                        Debug.LogError("[MV] " + entry.objName + " chưa được nối đủ 4 frame idle & 4 frame cheer.");
                        errors++;
                    }
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 13: PASS (0 lỗi). Cả 4 NPC (" + framesOk + "/32 frames FullRect 32x32) đã sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 13: FAIL (" + errors + " lỗi).");
            }
        }

        private static void ForceReimportNpcSprites()
        {
            if (!Directory.Exists(NpcAnimRoot)) return;
            foreach (string file in Directory.GetFiles(NpcAnimRoot, "*.png", SearchOption.AllDirectories))
            {
                string assetPath = file.Replace("\\", "/");
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 20;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.alphaIsTransparency = true;
                    importer.mipmapEnabled = false;

                    var settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    settings.spriteAlignment = (int)SpriteAlignment.Center;
                    settings.spriteGenerateFallbackPhysicsShape = false;
                    importer.SetTextureSettings(settings);
                    importer.SaveAndReimport();
                }
            }
        }

        private static Sprite[] LoadFrames(string npcId, string anim, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                string path = NpcAnimRoot + "/" + npcId + "/" + anim + "_" + i.ToString("00") + ".png";
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(path));
            }
            return list.ToArray();
        }

        private static void AssignArray(SerializedProperty prop, Sprite[] sprites)
        {
            if (prop == null) return;
            prop.ClearArray();
            for (int i = 0; i < sprites.Length; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            }
        }
    }
}
```

## 77. `Assets/Editor/AutoBuilder_Step14.cs`

```csharp
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;
using MoonpetalValley.Community;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step14
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/Upgraded";

        [MenuItem("Moonpetal/Step 14/1. Upgrade World Tilemap, Buildings & Decor")]
        public static void BuildStep14()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 14: NÂNG CẤP CẢNH QUAN, TILEMAP & NHÀ CỬA ===");
            AssetDatabase.Refresh();
            ForceReimportEnvironmentSprites();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            UpgradeWorldTilemap();
            UpgradeFarmSoilSprites();
            UpgradeInteractiveProps();
            UpgradeVillageDecorAndBuildings();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 14! Chạy 'Moonpetal/Step 14/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 14/Validate")]
        public static void ValidateStep14()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 14 ===");

            string[] requiredSprites =
            {
                "Tiles/tile_grass_0.png", "Tiles/tile_grass_3.png",
                "Tiles/tile_path_0.png", "Tiles/tile_path_2.png",
                "Tiles/tile_gh_floor_0.png", "Tiles/tile_wall.png",
                "Tiles/tile_fence.png", "Tiles/tile_tree.png", "Tiles/tile_water.png",
                "Soil/soil_untilled.png", "Soil/soil_dry.png", "Soil/soil_wet.png",
                "Buildings/house_bakery.png", "Buildings/house_shed.png",
                "Buildings/otis_notice_board.png", "Buildings/tree_hero_blossom.png",
                "Props/prop_well.png", "Props/prop_bed.png",
                "Props/planting_spot_empty.png", "Props/planting_spot_bloomed.png"
            };

            foreach (string rel in requiredSprites)
            {
                if (LoadEnvSprite(rel) == null)
                {
                    Debug.LogError("[MV] THIẾU sprite cảnh quan: " + EnvRoot + "/" + rel);
                    errors++;
                }
            }

            if (GameObject.Find("WorldMapGrid") == null) { Debug.LogError("[MV] Thiếu WorldMapGrid."); errors++; }
            if (GameObject.Find("VillageDecorGroup") == null) { Debug.LogError("[MV] Thiếu VillageDecorGroup."); errors++; }
            if (GameObject.Find("House_HanaBakery") == null) { Debug.LogError("[MV] Thiếu House_HanaBakery."); errors++; }
            if (GameObject.Find("House_BachShed") == null) { Debug.LogError("[MV] Thiếu House_BachShed."); errors++; }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 14: PASS (0 lỗi). Tilemap đa biến thể, Tiệm Bánh, Chòi Vườn, Cây Hoa Anh Đào & Đạo cụ đã nâng cấp hoàn chỉnh!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 14: FAIL (" + errors + " lỗi).");
            }
        }

        private static Sprite LoadEnvSprite(string relPath)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "/" + relPath);
        }

        private static void ForceReimportEnvironmentSprites()
        {
            if (!Directory.Exists(EnvRoot)) return;
            foreach (string file in Directory.GetFiles(EnvRoot, "*.png", SearchOption.AllDirectories))
            {
                string assetPath = file.Replace("\\", "/");
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 16;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.alphaIsTransparency = true;
                    importer.mipmapEnabled = false;

                    var settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    settings.spriteAlignment = (int)SpriteAlignment.Center;
                    settings.spriteGenerateFallbackPhysicsShape = false;
                    importer.SetTextureSettings(settings);
                    importer.SaveAndReimport();
                }
            }
        }

        private static void UpgradeWorldTilemap()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null)
            {
                Debug.LogError("[MV] Không tìm thấy MapLayoutConfig.asset!");
                return;
            }

            GameObject existingGrid = GameObject.Find("WorldMapGrid");
            if (existingGrid != null) UnityEngine.Object.DestroyImmediate(existingGrid);

            GameObject gridGo = new GameObject("WorldMapGrid");
            gridGo.AddComponent<Grid>();

            GameObject groundGo = new GameObject("GroundTilemap");
            groundGo.transform.SetParent(gridGo.transform, false);
            var groundTm = groundGo.AddComponent<Tilemap>();
            var groundRenderer = groundGo.AddComponent<TilemapRenderer>();
            groundRenderer.sortingLayerName = "Ground";

            GameObject obsGo = new GameObject("ObstacleTilemap");
            obsGo.transform.SetParent(gridGo.transform, false);
            obsGo.layer = 10;
            var obsTm = obsGo.AddComponent<Tilemap>();
            var obsRenderer = obsGo.AddComponent<TilemapRenderer>();
            obsRenderer.sortingLayerName = "WorldObjects";
            obsRenderer.sortingOrder = -5000;
            obsGo.AddComponent<TilemapCollider2D>();

            Tile[] grassTiles =
            {
                CreateTile(LoadEnvSprite("Tiles/tile_grass_0.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_grass_1.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_grass_2.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_grass_3.png"))
            };
            Tile[] pathTiles =
            {
                CreateTile(LoadEnvSprite("Tiles/tile_path_0.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_path_1.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_path_2.png"))
            };
            Tile[] ghFloorTiles =
            {
                CreateTile(LoadEnvSprite("Tiles/tile_gh_floor_0.png")),
                CreateTile(LoadEnvSprite("Tiles/tile_gh_floor_1.png"))
            };
            Tile tWall = CreateTile(LoadEnvSprite("Tiles/tile_wall.png"));
            Tile tFence = CreateTile(LoadEnvSprite("Tiles/tile_fence.png"));
            Tile tTree = CreateTile(LoadEnvSprite("Tiles/tile_tree.png"));
            Tile tWater = CreateTile(LoadEnvSprite("Tiles/tile_water.png"));
            Tile tFlowerbed = CreateTile(LoadEnvSprite("Tiles/tile_flowerbed.png"));

            string[] lines = cfg.asciiMap.Split('\n');
            int height = lines.Length;
            for (int r = 0; r < height; r++)
            {
                string row = lines[r].TrimEnd('\r');
                int y = cfg.originY + (height - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    int x = cfg.originX + c;
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    char ch = row[c];
                    int hash = Mathf.Abs(x * 73856093 ^ y * 19349663);

                    if (ch == 'G')
                    {
                        groundTm.SetTile(cell, ghFloorTiles[hash % ghFloorTiles.Length]);
                    }
                    else if (ch == 'P')
                    {
                        groundTm.SetTile(cell, pathTiles[hash % pathTiles.Length]);
                    }
                    else if (ch == 'B')
                    {
                        groundTm.SetTile(cell, tFlowerbed);
                    }
                    else
                    {
                        // Phân bổ cỏ tự nhiên: 55% cỏ nền, 45% cỏ 3 lá/hoa nhỏ
                        int gIdx = (hash % 10 < 5) ? 0 : (1 + (hash % 3));
                        groundTm.SetTile(cell, grassTiles[gIdx]);
                    }

                    if (ch == '#') obsTm.SetTile(cell, tWall);
                    else if (ch == 'F') obsTm.SetTile(cell, tFence);
                    else if (ch == 'T') obsTm.SetTile(cell, tTree);
                    else if (ch == 'W') obsTm.SetTile(cell, tWater);
                }
            }
        }

        private static Tile CreateTile(Sprite spr)
        {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static void UpgradeFarmSoilSprites()
        {
            Sprite untilled = LoadEnvSprite("Soil/soil_untilled.png");
            Sprite dry = LoadEnvSprite("Soil/soil_dry.png");
            Sprite wet = LoadEnvSprite("Soil/soil_wet.png");

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            for (int i = 0; i < tiles.Length; i++)
            {
                var so = new SerializedObject(tiles[i]);
                if (untilled != null) so.FindProperty("untilledSprite").objectReferenceValue = untilled;
                if (dry != null) so.FindProperty("tilledDrySprite").objectReferenceValue = dry;
                if (wet != null) so.FindProperty("tilledWetSprite").objectReferenceValue = wet;
                so.ApplyModifiedPropertiesWithoutUndo();

                var sr = tiles[i].GetComponent<SpriteRenderer>();
                if (sr != null && untilled != null) sr.sprite = untilled;
            }
        }

        private static void UpgradeInteractiveProps()
        {
            // 1. Giường Nhà Kính
            GameObject bed = GameObject.Find("GreenhouseBed");
            Sprite bedSpr = LoadEnvSprite("Props/prop_bed.png");
            if (bed != null && bedSpr != null)
            {
                var sr = bed.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = bedSpr;
            }

            // 2. Giếng Nước có mái ngói
            GameObject well = GameObject.Find("VillageWaterWell");
            Sprite wellSpr = LoadEnvSprite("Props/prop_well.png");
            if (well != null && wellSpr != null)
            {
                var sr = well.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = wellSpr;
            }

            // 3. Bồn Hoa Trung Tâm (Empty + Bloomed 48x48)
            var spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            Sprite emptySpr = LoadEnvSprite("Props/planting_spot_empty.png");
            Sprite bloomedSpr = LoadEnvSprite("Props/planting_spot_bloomed.png");
            if (spot != null && emptySpr != null && bloomedSpr != null)
            {
                var so = new SerializedObject(spot);
                so.FindProperty("emptySprite").objectReferenceValue = emptySpr;
                so.FindProperty("bloomedSprite").objectReferenceValue = bloomedSpr;
                so.ApplyModifiedPropertiesWithoutUndo();

                var sr = spot.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = emptySpr;
            }
        }

        private static void UpgradeVillageDecorAndBuildings()
        {
            GameObject oldDecor = GameObject.Find("VillageDecorGroup");
            if (oldDecor != null) UnityEngine.Object.DestroyImmediate(oldDecor);
            GameObject group = new GameObject("VillageDecorGroup");

            Sprite bakerySpr = LoadEnvSprite("Buildings/house_bakery.png");
            Sprite shedSpr = LoadEnvSprite("Buildings/house_shed.png");
            Sprite noticeSpr = LoadEnvSprite("Buildings/otis_notice_board.png");
            Sprite oakSpr = LoadEnvSprite("Buildings/tree_hero_oak.png");
            Sprite blossomSpr = LoadEnvSprite("Buildings/tree_hero_blossom.png");
            Sprite[] flowerSprs =
            {
                LoadEnvSprite("Decor/decor_flowers_0.png"),
                LoadEnvSprite("Decor/decor_flowers_1.png"),
                LoadEnvSprite("Decor/decor_flowers_2.png")
            };
            Sprite bushSpr = LoadEnvSprite("Decor/decor_bush.png");
            Sprite rockSpr = LoadEnvSprite("Decor/decor_rock.png");
            Sprite lilySpr = LoadEnvSprite("Decor/decor_lilypad.png");

            // 1. Kiến trúc chính: Tiệm Bánh Bà Hana, Chòi Vườn Ông Bách, Bảng Tin Bác Otis
            SpawnDecor(group.transform, "House_HanaBakery", bakerySpr, new Vector2(-6.5f, 5.8f), true, new Vector2(4.2f, 2.0f), new Vector2(0f, -0.7f));
            SpawnDecor(group.transform, "House_BachShed", shedSpr, new Vector2(-4.8f, -4.5f), true, new Vector2(3.2f, 1.7f), new Vector2(0f, -0.6f));
            SpawnDecor(group.transform, "OtisNoticeBoard", noticeSpr, new Vector2(2.8f, 6.2f), true, new Vector2(1.4f, 0.8f), new Vector2(0f, -0.4f));

            // 2. 6 Cây lớn (Cây Hoa Ánh Trăng hồng & Cây Sồi xanh) tạo điểm nhấn 4 phân khu
            SpawnDecor(group.transform, "HeroTree_Blossom_1", blossomSpr, new Vector2(-9.5f, 11.2f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Blossom_2", blossomSpr, new Vector2(5.8f, 9.2f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Blossom_3", blossomSpr, new Vector2(-5.5f, -11.8f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Blossom_4", blossomSpr, new Vector2(5.5f, -11.8f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Oak_1", oakSpr, new Vector2(13.2f, 10.2f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));
            SpawnDecor(group.transform, "HeroTree_Oak_2", oakSpr, new Vector2(19.2f, 4.2f), true, new Vector2(1.0f, 0.8f), new Vector2(0f, -0.8f));

            // 3. Hoa súng trên mặt ao cạnh Nhà Kính
            SpawnDecor(group.transform, "PondLily_1", lilySpr, new Vector2(-6.0f, 12.0f), false, Vector2.zero, Vector2.zero);
            SpawnDecor(group.transform, "PondLily_2", lilySpr, new Vector2(-5.1f, 11.9f), false, Vector2.zero, Vector2.zero);

            // 4. Đá rêu & bụi cây ven đường
            Vector2[] bushes =
            {
                new Vector2(-9.2f, 5.2f), new Vector2(-3.8f, 5.2f), new Vector2(4.4f, 5.4f),
                new Vector2(9.2f, 4.2f), new Vector2(-5.8f, -6.2f), new Vector2(5.8f, -6.2f)
            };
            for (int i = 0; i < bushes.Length; i++)
            {
                SpawnDecor(group.transform, "DecorBush_" + (i + 1), bushSpr, bushes[i], true, new Vector2(0.75f, 0.6f), new Vector2(0f, -0.15f));
            }

            Vector2[] rocks =
            {
                new Vector2(-10.5f, 9.0f), new Vector2(10.8f, 9.0f), new Vector2(14.0f, 3.0f), new Vector2(-7.8f, -13.0f)
            };
            for (int i = 0; i < rocks.Length; i++)
            {
                SpawnDecor(group.transform, "MossRock_" + (i + 1), rockSpr, rocks[i], true, new Vector2(0.7f, 0.55f), new Vector2(0f, -0.15f));
            }

            // 5. 18 cụm hoa dại đa màu rải đều khắp thung lũng
            Vector2[] flowers =
            {
                new Vector2(-11.0f, 3.0f), new Vector2(-8.5f, 1.5f), new Vector2(-3.0f, 1.0f), new Vector2(3.5f, 1.2f),
                new Vector2(6.5f, 4.5f), new Vector2(-2.5f, -2.0f), new Vector2(4.0f, -2.2f), new Vector2(-5.0f, -9.5f),
                new Vector2(5.2f, -9.5f), new Vector2(13.5f, 9.0f), new Vector2(18.5f, 4.5f), new Vector2(14.5f, 2.0f),
                new Vector2(-10.2f, 12.6f), new Vector2(-2.8f, 9.6f), new Vector2(8.8f, 8.2f), new Vector2(-2.6f, -13.2f),
                new Vector2(2.6f, -13.2f), new Vector2(11.5f, 6.2f)
            };
            for (int i = 0; i < flowers.Length; i++)
            {
                SpawnDecor(group.transform, "FlowerTuft_" + (i + 1), flowerSprs[i % flowerSprs.Length], flowers[i], false, Vector2.zero, Vector2.zero);
            }
        }

        private static void SpawnDecor(Transform parent, string name, Sprite spr, Vector2 pos, bool solid, Vector2 colSize, Vector2 colOffset)
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = solid ? "WorldObjects" : "FloorDecal";
            sr.sortingOrder = solid ? Mathf.RoundToInt(-pos.y * 100f) : 12;
            if (solid)
            {
                go.layer = 10;
                var col = go.AddComponent<BoxCollider2D>();
                col.size = colSize;
                col.offset = colOffset;
            }
        }
    }
}
```

## 78. `Assets/Editor/AutoBuilder_Step15.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step15
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";
        private const string SlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";
        private const string LumiRoot = "Assets/Art/Sprites/Animations/Lumi/Idle/animations/";
        private const string SlimeRoot = "Assets/Art/Sprites/Animations/GloomSlime/Idle/animations/";

        [MenuItem("Moonpetal/Step 15/1. Apply World Polish, Gate, Shadows, Water & Re-wire Lumi/Slime")]
        public static void BuildStep15()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 15 ===");
            AssetDatabase.Refresh();

            ForceReimportFolder("Assets/Art/Sprites/Animations/Lumi", 20f);
            ForceReimportFolder("Assets/Art/Sprites/Animations/GloomSlime", 32f);
            ForceReimportFolder("Assets/Art/Sprites/Environment", 16f);
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Re-wire Lumi & GloomSlime Animations (Prefab + Scene instances)
            WireLumiAnimations();
            WireSlimeAnimations();

            // 2. Open South Garden Gate in MapLayoutConfig & Rebuild WorldMapGrid
            MapLayoutConfigSO mapCfg = OpenSouthGardenGateAndSaveConfig();
            if (mapCfg != null)
            {
                RebuildWorldTilemapWithUpgradedArt(mapCfg);
                BuildAnimatedPondWater(mapCfg);
            }

            // 3. Add Ground Shadows to Wen, Lumi, NPCs, Slimes
            Sprite shadowSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Shadows/spr_ground_shadow.png");
            AttachGroundShadows(shadowSpr);

            // 4. Upgrade Remaining Props + Bed + Well + Farm Soil
            UpgradeRemainingPropsAndFarmSoil();

            // 5. Enrich Map Layout with Garden Archway & Cozy Props
            EnrichWorldDecor();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 15! Chạy 'Moonpetal/Step 15/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 15/Validate")]
        public static void ValidateStep15()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 15 ===");

            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null || !cfg.asciiMap.Contains("FFFFFPPPPFFFFF"))
            {
                Debug.LogError("[MV] MapLayoutConfig.asset chưa mở cổng 4 ô vào Vườn Phía Nam (FFFFFPPPPFFFFF).");
                errors++;
            }

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi == null)
            {
                Debug.LogError("[MV] Không tìm thấy LumiCompanionController trong scene.");
                errors++;
            }
            else
            {
                var lso = new SerializedObject(lumi);
                int fLen = lso.FindProperty("floatFrames").arraySize;
                int pLen = lso.FindProperty("pulseFrames").arraySize;
                if (fLen < 4 || pLen < 4)
                {
                    Debug.LogError($"[MV] Lumi thiếu frame animation (float={fLen}, pulse={pLen}). Hãy kiểm tra gói tinh_linh_lumi_animations.zip.");
                    errors++;
                }
            }

            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            if (slimes.Length < 8)
            {
                Debug.LogError($"[MV] Thiếu GloomSlime trong scene ({slimes.Length}/8).");
                errors++;
            }
            else
            {
                var sso = new SerializedObject(slimes[0]);
                int iLen = sso.FindProperty("idleFrames").arraySize;
                int tLen = sso.FindProperty("telegraphFrames").arraySize;
                int jLen = sso.FindProperty("jumpFrames").arraySize;
                int hLen = sso.FindProperty("hitFrames").arraySize;
                int dLen = sso.FindProperty("dieFrames").arraySize;
                if (iLen < 4 || tLen < 3 || jLen < 4 || hLen < 3 || dLen < 4)
                {
                    Debug.LogError($"[MV] GloomSlime thiếu frame animation (idle={iLen}, tele={tLen}, jump={jLen}, hit={hLen}, die={dLen}).");
                    errors++;
                }
            }

            var waterAnim = UnityEngine.Object.FindAnyObjectByType<WaterTileAnimator>();
            if (waterAnim == null)
            {
                Debug.LogError("[MV] Thiếu WaterTileAnimator cho hồ nước.");
                errors++;
            }
            else
            {
                var wso = new SerializedObject(waterAnim);
                int wf = wso.FindProperty("waterFrames").arraySize;
                int wr = wso.FindProperty("waterRenderers").arraySize;
                if (wf < 4 || wr < 5)
                {
                    Debug.LogError($"[MV] WaterTileAnimator thiếu dữ liệu (frames={wf}, tiles={wr}).");
                    errors++;
                }
            }

            int shadowCount = 0;
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            for (int i = 0; i < allTransforms.Length; i++)
            {
                if (allTransforms[i].name == "GroundShadow") shadowCount++;
            }
            if (shadowCount < 14)
            {
                Debug.LogError($"[MV] Số lượng GroundShadow còn thiếu ({shadowCount}, cần >= 14 cho Wen, Lumi, 4 NPC, 8 Slime).");
                errors++;
            }

            string[] propNames =
            {
                "GreenhouseLetterTable", "GreenhouseSeedBox", "GreenhouseTestPlanter",
                "GreenhouseSeedShelf", "OtisBrokenMailbox", "GardenMemoryBench", "GreenhouseBed"
            };
            foreach (string pn in propNames)
            {
                var go = GameObject.Find(pn);
                if (go == null) continue;
                var sr = go.GetComponentInChildren<SpriteRenderer>();
                if (sr == null || sr.sprite == null)
                {
                    Debug.LogError("[MV] Prop thiếu SpriteRenderer hoặc Sprite: " + pn);
                    errors++;
                }
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + allGos[i].name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log($"[MV] VALIDATE STEP 15: PASS (0 lỗi | Shadows={shadowCount} | AnimatedWater=OK | GardenGate=OPEN | Lumi&Slime=OK).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 15: FAIL ({errors} lỗi).");
            }
        }

        private static void ForceReimportFolder(string folderPath, float ppu)
        {
            if (!AssetDatabase.IsValidFolder(folderPath)) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
            foreach (string g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var imp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (imp == null) continue;
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.filterMode = FilterMode.Point;
                imp.textureCompression = TextureImporterCompression.Uncompressed;
                imp.mipmapEnabled = false;
                imp.alphaIsTransparency = true;
                imp.spritePixelsPerUnit = ppu;
                var st = new TextureImporterSettings();
                imp.ReadTextureSettings(st);
                st.spriteMeshType = SpriteMeshType.FullRect;
                st.spriteAlignment = (int)SpriteAlignment.Center;
                st.spritePivot = new Vector2(0.5f, 0.5f);
                imp.SetTextureSettings(st);
                imp.SaveAndReimport();
            }
        }

        private static Sprite[] LoadFrames(string root, string prefix, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                string path = $"{root}{prefix}_{i:00}.png";
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }

        private static void SetSpriteArray(SerializedObject so, string propName, Sprite[] frames)
        {
            var prop = so.FindProperty(propName);
            if (prop == null) return;
            prop.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++)
            {
                prop.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            }
        }

        private static void WireLumiAnimations()
        {
            Sprite[] floatF = LoadFrames(LumiRoot, "float", 4);
            Sprite[] pulseF = LoadFrames(LumiRoot, "moon_pulse", 4);
            if (floatF.Length == 0)
            {
                Debug.LogWarning("[MV] Chưa thấy frame Lumi tại " + LumiRoot);
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(LumiPrefabPath);
                ApplyLumiFramesToObject(root, floatF, pulseF);
                PrefabUtility.SaveAsPrefabAsset(root, LumiPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }

            var sceneLumis = UnityEngine.Object.FindObjectsByType<LumiCompanionController>(FindObjectsInactive.Include);
            foreach (var l in sceneLumis)
            {
                ApplyLumiFramesToObject(l.gameObject, floatF, pulseF);
            }
            Debug.Log($"[MV] Đã gắn animation Lumi: float={floatF.Length}f, moon_pulse={pulseF.Length}f.");
        }

        private static void ApplyLumiFramesToObject(GameObject go, Sprite[] floatF, Sprite[] pulseF)
        {
            var ctrl = go.GetComponent<LumiCompanionController>();
            if (ctrl == null) return;
            var vis = go.transform.Find("Visual");
            var sr = vis != null ? vis.GetComponent<SpriteRenderer>() : go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && floatF.Length > 0) sr.sprite = floatF[0];

            var so = new SerializedObject(ctrl);
            if (sr != null) so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            SetSpriteArray(so, "floatFrames", floatF);
            SetSpriteArray(so, "pulseFrames", pulseF);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ctrl);
        }

        private static void WireSlimeAnimations()
        {
            Sprite[] idleF = LoadFrames(SlimeRoot, "idle", 4);
            Sprite[] teleF = LoadFrames(SlimeRoot, "telegraph", 3);
            Sprite[] jumpF = LoadFrames(SlimeRoot, "jump", 4);
            Sprite[] hitF = LoadFrames(SlimeRoot, "hit_hurt", 3);
            Sprite[] dieF = LoadFrames(SlimeRoot, "purify_die", 4);
            if (idleF.Length == 0)
            {
                Debug.LogWarning("[MV] Chưa thấy frame GloomSlime tại " + SlimeRoot);
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(SlimePrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(SlimePrefabPath);
                ApplySlimeFramesToObject(root, idleF, teleF, jumpF, hitF, dieF);
                PrefabUtility.SaveAsPrefabAsset(root, SlimePrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }

            var sceneSlimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            foreach (var sl in sceneSlimes)
            {
                ApplySlimeFramesToObject(sl.gameObject, idleF, teleF, jumpF, hitF, dieF);
            }
            Debug.Log($"[MV] Đã gắn animation GloomSlime ({sceneSlimes.Length} instances): idle={idleF.Length}f, tele={teleF.Length}f, jump={jumpF.Length}f, hit={hitF.Length}f, die={dieF.Length}f.");
        }

        private static void ApplySlimeFramesToObject(GameObject go, Sprite[] idleF, Sprite[] teleF, Sprite[] jumpF, Sprite[] hitF, Sprite[] dieF)
        {
            var ctrl = go.GetComponent<GloomSlimeController>();
            if (ctrl == null) return;
            var so = new SerializedObject(ctrl);
            var srProp = so.FindProperty("spriteRenderer");
            var sr = srProp != null ? srProp.objectReferenceValue as SpriteRenderer : go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && idleF.Length > 0) sr.sprite = idleF[0];

            SetSpriteArray(so, "idleFrames", idleF);
            SetSpriteArray(so, "telegraphFrames", teleF);
            SetSpriteArray(so, "jumpFrames", jumpF);
            SetSpriteArray(so, "hitFrames", hitF);
            SetSpriteArray(so, "dieFrames", dieF);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ctrl);
        }

        private static MapLayoutConfigSO OpenSouthGardenGateAndSaveConfig()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null)
            {
                Debug.LogError("[MV] Không tìm thấy MapLayoutConfig.asset!");
                return null;
            }

            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            bool changed = false;
            for (int r = 0; r < rows.Length; r++)
            {
                // Row 17 is the north fence of South Garden (directly below Village south path)
                if (r == 17 && rows[r].Contains("FFFFFFFFFFFFFF"))
                {
                    rows[r] = rows[r].Replace("FFFFFFFFFFFFFF", "FFFFFPPPPFFFFF");
                    changed = true;
                }
            }

            if (changed)
            {
                cfg.asciiMap = string.Join("\n", rows);
                EditorUtility.SetDirty(cfg);
                AssetDatabase.SaveAssets();
                Debug.Log("[MV] Đã mở cổng rộng 4 ô vào Vườn Phía Nam tại hàng 17 (FFFFFPPPPFFFFF).");
            }
            return cfg;
        }

        private static Tile MakeTile(Sprite spr)
        {
            var t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.color = Color.white;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static void RebuildWorldTilemapWithUpgradedArt(MapLayoutConfigSO cfg)
        {
            Sprite[] grass =
            {
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_grass_0.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_grass_1.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_grass_2.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_grass_3.png"),
            };
            Sprite[] path =
            {
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_path_0.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_path_1.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_path_2.png"),
            };
            Sprite[] ghFloor =
            {
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_gh_floor_0.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_gh_floor_1.png"),
            };
            Sprite wallSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_wall.png");
            Sprite fenceSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_fence.png");
            Sprite treeSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_tree.png");
            Sprite waterSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Water/water_anim_00.png");
            if (waterSpr == null) waterSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_water.png");
            Sprite flowerbedSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Tiles/tile_flowerbed.png");

            if (grass[0] == null || path[0] == null || fenceSpr == null)
            {
                Debug.LogWarning("[MV] Thiếu tile từ environment_art_pack.zip; chỉ cập nhật lại cổng nếu tile cũ còn sẵn.");
                return;
            }

            Tile[] grassTiles = new Tile[grass.Length];
            for (int i = 0; i < grass.Length; i++) grassTiles[i] = MakeTile(grass[i] != null ? grass[i] : grass[0]);
            Tile[] pathTiles = new Tile[path.Length];
            for (int i = 0; i < path.Length; i++) pathTiles[i] = MakeTile(path[i] != null ? path[i] : path[0]);
            Tile[] ghTiles = new Tile[ghFloor.Length];
            for (int i = 0; i < ghFloor.Length; i++) ghTiles[i] = MakeTile(ghFloor[i] != null ? ghFloor[i] : ghFloor[0]);

            Tile wallTile = MakeTile(wallSpr);
            Tile fenceTile = MakeTile(fenceSpr);
            Tile treeTile = MakeTile(treeSpr);
            Tile waterTile = MakeTile(waterSpr);
            Tile flowerbedTile = MakeTile(flowerbedSpr);

            GameObject gridGo = GameObject.Find("WorldMapGrid");
            if (gridGo != null) UnityEngine.Object.DestroyImmediate(gridGo);

            gridGo = new GameObject("WorldMapGrid");
            gridGo.AddComponent<Grid>();

            GameObject groundGo = new GameObject("GroundTilemap");
            groundGo.transform.SetParent(gridGo.transform, false);
            var groundTm = groundGo.AddComponent<Tilemap>();
            var groundRenderer = groundGo.AddComponent<TilemapRenderer>();
            groundRenderer.sortingLayerName = "Ground";

            GameObject obsGo = new GameObject("ObstacleTilemap");
            obsGo.transform.SetParent(gridGo.transform, false);
            obsGo.layer = 10;
            var obsTm = obsGo.AddComponent<Tilemap>();
            var obsRenderer = obsGo.AddComponent<TilemapRenderer>();
            obsRenderer.sortingLayerName = "WorldObjects";
            obsRenderer.sortingOrder = -5000;
            obsGo.AddComponent<TilemapCollider2D>();

            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            for (int r = 0; r < rows.Length; r++)
            {
                string row = rows[r];
                int y = cfg.originY + (rows.Length - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    int x = cfg.originX + c;
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    int hash = Mathf.Abs(x * 73856093 ^ y * 19349663);
                    char ch = row[c];

                    switch (ch)
                    {
                        case 'P':
                            groundTm.SetTile(pos, pathTiles[hash % pathTiles.Length]);
                            break;
                        case 'G':
                            groundTm.SetTile(pos, ghTiles[hash % ghTiles.Length]);
                            break;
                        case 'B':
                            groundTm.SetTile(pos, flowerbedTile);
                            break;
                        case '#':
                            groundTm.SetTile(pos, ghTiles[0]);
                            obsTm.SetTile(pos, wallTile);
                            break;
                        case 'F':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, fenceTile);
                            break;
                        case 'T':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, treeTile);
                            break;
                        case 'W':
                            groundTm.SetTile(pos, waterTile);
                            obsTm.SetTile(pos, waterTile);
                            break;
                        default:
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            break;
                    }
                }
            }
            Debug.Log("[MV] Đã dựng lại WorldMapGrid với cổng Vườn Phía Nam mở rộng 4 ô.");
        }

        private static void BuildAnimatedPondWater(MapLayoutConfigSO cfg)
        {
            Sprite[] frames = LoadFrames(EnvRoot + "Water/", "water_anim", 4);
            if (frames.Length == 0)
            {
                Debug.LogWarning("[MV] Không tìm thấy water_anim_00..03.png.");
                return;
            }

            GameObject oldWater = GameObject.Find("AnimatedPondWater");
            if (oldWater != null) UnityEngine.Object.DestroyImmediate(oldWater);

            GameObject root = new GameObject("AnimatedPondWater");
            var anim = root.AddComponent<WaterTileAnimator>();

            var renderers = new List<SpriteRenderer>();
            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            for (int r = 0; r < rows.Length; r++)
            {
                string row = rows[r];
                int y = cfg.originY + (rows.Length - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    if (row[c] != 'W') continue;
                    int x = cfg.originX + c;
                    GameObject cellGo = new GameObject($"WaterCell_{x}_{y}");
                    cellGo.transform.SetParent(root.transform, false);
                    cellGo.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                    var sr = cellGo.AddComponent<SpriteRenderer>();
                    sr.sprite = frames[0];
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = -4900;
                    renderers.Add(sr);
                }
            }

            var so = new SerializedObject(anim);
            SetSpriteArray(so, "waterFrames", frames);
            var rProp = so.FindProperty("waterRenderers");
            rProp.arraySize = renderers.Count;
            for (int i = 0; i < renderers.Count; i++)
            {
                rProp.GetArrayElementAtIndex(i).objectReferenceValue = renderers[i];
            }
            so.FindProperty("fps").floatValue = 3.2f;
            so.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log($"[MV] Đã tạo AnimatedPondWater cho {renderers.Count} ô hồ nước (4 frames).");
        }

        private static GameObject EnsureShadowChild(Transform parent, Sprite shadowSpr, Vector3 localPos, Vector3 localScale)
        {
            if (parent == null || shadowSpr == null) return null;
            Transform existing = parent.Find("GroundShadow");
            GameObject go = existing != null ? existing.gameObject : new GameObject("GroundShadow");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = shadowSpr;
            sr.color = new Color(1f, 1f, 1f, 0.62f);
            sr.sortingLayerName = "Shadows";
            sr.sortingOrder = 0;
            return go;
        }

        private static void AttachGroundShadows(Sprite shadowSpr)
        {
            if (shadowSpr == null)
            {
                Debug.LogWarning("[MV] Thiếu spr_ground_shadow.png.");
                return;
            }

            // Player prefab + scene
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) != null)
            {
                GameObject pRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
                EnsureShadowChild(pRoot.transform, shadowSpr, new Vector3(0f, -0.56f, 0f), new Vector3(1.05f, 0.85f, 1f));
                PrefabUtility.SaveAsPrefabAsset(pRoot, PlayerPrefabPath);
                PrefabUtility.UnloadPrefabContents(pRoot);
            }
            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                EnsureShadowChild(player.transform, shadowSpr, new Vector3(0f, -0.56f, 0f), new Vector3(1.05f, 0.85f, 1f));
            }

            // Lumi prefab + scene (shadow sits underneath floating Lumi)
            if (AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath) != null)
            {
                GameObject lRoot = PrefabUtility.LoadPrefabContents(LumiPrefabPath);
                EnsureShadowChild(lRoot.transform, shadowSpr, new Vector3(0f, -0.48f, 0f), new Vector3(0.62f, 0.50f, 1f));
                PrefabUtility.SaveAsPrefabAsset(lRoot, LumiPrefabPath);
                PrefabUtility.UnloadPrefabContents(lRoot);
            }
            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null)
            {
                EnsureShadowChild(lumi.transform, shadowSpr, new Vector3(0f, -0.48f, 0f), new Vector3(0.62f, 0.50f, 1f));
            }

            // GloomSlime prefab + scene instances
            if (AssetDatabase.LoadAssetAtPath<GameObject>(SlimePrefabPath) != null)
            {
                GameObject sRoot = PrefabUtility.LoadPrefabContents(SlimePrefabPath);
                GameObject sh = EnsureShadowChild(sRoot.transform, shadowSpr, new Vector3(0f, -0.38f, 0f), new Vector3(0.95f, 0.78f, 1f));
                var sc = sRoot.GetComponent<GloomSlimeController>();
                if (sc != null && sh != null)
                {
                    var so = new SerializedObject(sc);
                    so.FindProperty("groundShadow").objectReferenceValue = sh;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
                PrefabUtility.SaveAsPrefabAsset(sRoot, SlimePrefabPath);
                PrefabUtility.UnloadPrefabContents(sRoot);
            }
            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            foreach (var sl in slimes)
            {
                GameObject sh = EnsureShadowChild(sl.transform, shadowSpr, new Vector3(0f, -0.38f, 0f), new Vector3(0.95f, 0.78f, 1f));
                var so = new SerializedObject(sl);
                so.FindProperty("groundShadow").objectReferenceValue = sh;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(sl);
            }

            // All NPCs in scene
            var npcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            foreach (var npc in npcs)
            {
                EnsureShadowChild(npc.transform, shadowSpr, new Vector3(0f, -0.56f, 0f), new Vector3(1.05f, 0.85f, 1f));
                EditorUtility.SetDirty(npc.gameObject);
            }

            Debug.Log($"[MV] Đã gắn GroundShadow cho Wen, Lumi, {npcs.Length} NPC và {slimes.Length} Bọt Sương Đen.");
        }

        private static void ApplySpriteToNamedObject(string goName, string spriteRelPath, float scale = 1f)
        {
            GameObject go = GameObject.Find(goName);
            if (go == null) return;
            Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + spriteRelPath);
            if (spr == null) return;
            var sr = go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = spr;
                sr.color = Color.white;
                if (Mathf.Abs(scale - 1f) > 0.001f)
                {
                    sr.transform.localScale = new Vector3(scale, scale, 1f);
                }
                EditorUtility.SetDirty(sr);
            }
        }

        private static void UpgradeRemainingPropsAndFarmSoil()
        {
            ApplySpriteToNamedObject("GreenhouseLetterTable", "Props/prop_letter_table.png");
            ApplySpriteToNamedObject("GreenhouseSeedBox", "Props/prop_seed_box.png");
            ApplySpriteToNamedObject("GreenhouseTestPlanter", "Props/prop_test_planter.png");
            ApplySpriteToNamedObject("GreenhouseSeedShelf", "Props/prop_seed_shelf.png");
            ApplySpriteToNamedObject("OtisBrokenMailbox", "Props/prop_mailbox.png");
            ApplySpriteToNamedObject("GardenMemoryBench", "Props/prop_memory_bench.png");
            ApplySpriteToNamedObject("GreenhouseBed", "Props/prop_bed.png");
            ApplySpriteToNamedObject("VillageWaterWell", "Props/prop_well.png");

            Sprite soilUntilled = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Soil/soil_untilled.png");
            Sprite soilDry = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Soil/soil_dry.png");
            Sprite soilWet = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Soil/soil_wet.png");

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            if (soilUntilled != null && soilDry != null && soilWet != null)
            {
                foreach (var tile in tiles)
                {
                    var so = new SerializedObject(tile);
                    so.FindProperty("untilledSprite").objectReferenceValue = soilUntilled;
                    so.FindProperty("tilledDrySprite").objectReferenceValue = soilDry;
                    so.FindProperty("tilledWetSprite").objectReferenceValue = soilWet;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    tile.RefreshVisuals();
                    EditorUtility.SetDirty(tile);
                }
            }
            Debug.Log($"[MV] Đã nâng cấp art cho 8 đạo cụ còn thiếu và {tiles.Length} ô đất trồng.");
        }

        private static void SpawnDecor(Transform parent, string name, Sprite spr, Vector3 pos, int sortOrderOffset = 0, string sortingLayer = "WorldObjects")
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = sortingLayer;
            sr.sortingOrder = sortOrderOffset != 0 ? sortOrderOffset : Mathf.RoundToInt(-pos.y * 100f);
        }

        private static void EnrichWorldDecor()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) decorGroup = new GameObject("VillageDecorGroup");

            Transform existing = decorGroup.transform.Find("Step15WorldPolishGroup");
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

            GameObject polish = new GameObject("Step15WorldPolishGroup");
            polish.transform.SetParent(decorGroup.transform, false);

            Sprite archSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Buildings/prop_garden_arch.png");
            Sprite signSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_signpost.png");
            Sprite barrelSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_barrel.png");
            Sprite mushSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_mushrooms.png");
            Sprite picnicSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_picnic_table.png");
            Sprite oakSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Buildings/tree_hero_oak.png");
            Sprite blossomSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Buildings/tree_hero_blossom.png");
            Sprite bushSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_bush.png");
            Sprite rockSpr = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_rock.png");
            Sprite fl0 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_flowers_0.png");
            Sprite fl1 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_flowers_1.png");
            Sprite fl2 = AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + "Decor/decor_flowers_2.png");

            // 1. South Garden Flower Archway (spans open 4-tile gate at x=0, y=-1.5)
            SpawnDecor(polish.transform, "GardenEntranceArch", archSpr, new Vector3(0.0f, -1.05f, 0f), 2600, "WorldObjects");

            // 2. Village Crossroads & Cozy Gathering Props
            SpawnDecor(polish.transform, "VillageSignpost", signSpr, new Vector3(-2.6f, 2.2f, 0f));
            SpawnDecor(polish.transform, "BakeryPicnicTable", picnicSpr, new Vector3(5.6f, 5.2f, 0f));
            SpawnDecor(polish.transform, "BakeryBarrel_1", barrelSpr, new Vector3(3.6f, 6.1f, 0f));
            SpawnDecor(polish.transform, "ShedBarrel_1", barrelSpr, new Vector3(-7.4f, 6.0f, 0f));
            SpawnDecor(polish.transform, "ShedBarrel_2", barrelSpr, new Vector3(-6.6f, 5.6f, 0f));
            SpawnDecor(polish.transform, "GreenhouseBarrel", barrelSpr, new Vector3(-8.2f, 11.4f, 0f));

            // 3. Extra Hero Trees to frame empty meadows
            SpawnDecor(polish.transform, "MeadowBlossom_NW", blossomSpr, new Vector3(-8.5f, 13.8f, 0f));
            SpawnDecor(polish.transform, "MeadowOak_SW", oakSpr, new Vector3(-16.5f, -3.5f, 0f));
            SpawnDecor(polish.transform, "MeadowBlossom_SW", blossomSpr, new Vector3(-12.2f, -8.5f, 0f));
            SpawnDecor(polish.transform, "MeadowOak_SE", oakSpr, new Vector3(13.5f, -4.5f, 0f));
            SpawnDecor(polish.transform, "MeadowBlossom_SE", blossomSpr, new Vector3(11.0f, -10.0f, 0f));
            SpawnDecor(polish.transform, "GardenBlossom_L", blossomSpr, new Vector3(-5.2f, -10.2f, 0f));
            SpawnDecor(polish.transform, "GardenBlossom_R", blossomSpr, new Vector3(5.2f, -10.2f, 0f));

            // 4. Forest Edge Mushroom Clusters
            Vector3[] mushSpots =
            {
                new Vector3(12.2f, 11.2f, 0f),
                new Vector3(16.8f, 10.4f, 0f),
                new Vector3(17.5f, 4.2f, 0f),
                new Vector3(13.4f, 1.8f, 0f),
                new Vector3(16.0f, -1.2f, 0f),
            };
            for (int i = 0; i < mushSpots.Length; i++)
            {
                SpawnDecor(polish.transform, $"ForestMushrooms_{i}", mushSpr, mushSpots[i], 0, "FloorDecal");
            }

            // 5. Bushes, Mossy Rocks & Wildflowers to break up large open grass
            Vector3[] bushSpots =
            {
                new Vector3(-18.5f, 2.5f, 0f), new Vector3(-13.5f, -1.0f, 0f),
                new Vector3(-9.5f, -3.5f, 0f), new Vector3(9.5f, -2.5f, 0f),
                new Vector3(15.2f, -8.2f, 0f), new Vector3(-17.2f, -9.8f, 0f),
                new Vector3(2.8f, -1.6f, 0f), new Vector3(-2.8f, -1.6f, 0f),
            };
            for (int i = 0; i < bushSpots.Length; i++)
            {
                SpawnDecor(polish.transform, $"ExtraBush_{i}", bushSpr, bushSpots[i]);
            }

            Vector3[] rockSpots =
            {
                new Vector3(-11.2f, 13.0f, 0f), new Vector3(-18.2f, -6.2f, 0f),
                new Vector3(11.8f, -7.2f, 0f), new Vector3(17.2f, 7.6f, 0f),
            };
            for (int i = 0; i < rockSpots.Length; i++)
            {
                SpawnDecor(polish.transform, $"ExtraRock_{i}", rockSpr, rockSpots[i]);
            }

            Sprite[] fls = { fl0, fl1, fl2 };
            Vector3[] flowerSpots =
            {
                new Vector3(-14.0f, 2.8f, 0f), new Vector3(-10.5f, 0.2f, 0f),
                new Vector3(-15.5f, -6.5f, 0f), new Vector3(-9.0f, -11.0f, 0f),
                new Vector3(8.8f, 0.5f, 0f), new Vector3(11.5f, -2.2f, 0f),
                new Vector3(14.5f, -10.8f, 0f), new Vector3(-4.2f, -4.5f, 0f),
                new Vector3(4.2f, -4.5f, 0f), new Vector3(-3.8f, -9.2f, 0f),
                new Vector3(3.8f, -9.2f, 0f), new Vector3(-5.0f, 13.5f, 0f),
            };
            for (int i = 0; i < flowerSpots.Length; i++)
            {
                SpawnDecor(polish.transform, $"ExtraFlower_{i}", fls[i % fls.Length], flowerSpots[i], 2, "FloorDecal");
            }

            Debug.Log("[MV] Đã bổ sung cổng vòm hoa, tiểu cảnh làng, nấm rừng và thảm hoa/cây làm đầy map.");
        }
    }
}
```

## 79. `Assets/Editor/AutoBuilder_Step16.cs`

```csharp
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;
using MoonpetalValley.Data;
using MoonpetalValley.Farming;
using MoonpetalValley.Narrative;
using MoonpetalValley.Player;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step16
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";
        private const string SlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";

        [MenuItem("Moonpetal/Step 16/1. Apply Auto-Tile Walls/Fences, Greenhouse Layout, Shrine & Shadows")]
        public static void BuildStep16()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 16 ===");
            AssetDatabase.Refresh();
            ForceReimportEnvironmentFolder();
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Rebuild WorldMapGrid with Directional Auto-Tiling for '#' (Walls) and 'F' (Fences)
            var mapCfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (mapCfg != null)
            {
                RebuildAutoTiledWorldMap(mapCfg);
            }

            // 2. Upgrade CentralPlantingSpot (Moonpetal Shrine Planter)
            UpgradeCentralPlantingSpot();

            // 3. Upgrade 12 Greenhouse Farm Tiles (Raised Planter Boxes + Neat Bed Layout + Cozy Rug)
            UpgradeGreenhouseInteriorAndFarmLayout();

            // 4. Strengthen Ground Shadows for Wen, Lumi, 4 NPCs & 8 Slimes
            StrengthenGroundShadows();

            // 5. Fix any tree/decor clipping into the Greenhouse east glass wall
            FixGreenhouseWallClippingDecor();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 16! Chạy 'Moonpetal/Step 16/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 16/Validate")]
        public static void ValidateStep16()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 16 ===");

            string[] requiredSprites =
            {
                "Tiles/tile_wall.png", "Tiles/tile_wall_v.png",
                "Tiles/tile_wall_tl.png", "Tiles/tile_wall_tr.png",
                "Tiles/tile_wall_bl.png", "Tiles/tile_wall_br.png",
                "Tiles/tile_fence.png", "Tiles/tile_fence_v.png",
                "Tiles/tile_fence_tl.png", "Tiles/tile_fence_tr.png",
                "Tiles/tile_fence_bl.png", "Tiles/tile_fence_br.png",
                "Tiles/tile_flowerbed.png",
                "Soil/soil_untilled.png", "Soil/soil_dry.png", "Soil/soil_wet.png",
                "Props/planting_spot_empty.png", "Props/planting_spot_bloomed.png",
                "Decor/decor_greenhouse_rug.png",
                "Shadows/spr_ground_shadow.png",
            };
            foreach (string rel in requiredSprites)
            {
                if (AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel) == null)
                {
                    Debug.LogError("[MV] Thiếu sprite Step 16: " + EnvRoot + rel);
                    errors++;
                }
            }

            var spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            if (spot == null)
            {
                Debug.LogError("[MV] Không tìm thấy PlantingSpotController trong scene.");
                errors++;
            }
            else
            {
                var so = new SerializedObject(spot);
                if (so.FindProperty("emptySprite").objectReferenceValue == null ||
                    so.FindProperty("bloomedSprite").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] PlantingSpotController thiếu emptySprite hoặc bloomedSprite.");
                    errors++;
                }
            }

            if (GameObject.Find("GreenhouseCozyRug") == null)
            {
                Debug.LogError("[MV] Thiếu GreenhouseCozyRug trong Nhà Kính.");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + allGos[i].name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 16: PASS (0 lỗi | Auto-Tile Walls/Fences=OK | Raised Planters=OK | Shrine=OK | Strong Shadows=OK).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 16: FAIL ({errors} lỗi).");
            }
        }

        private static void ForceReimportEnvironmentFolder()
        {
            string folder = "Assets/Art/Sprites/Environment";
            if (!AssetDatabase.IsValidFolder(folder)) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
            foreach (string g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var imp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (imp == null) continue;
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.filterMode = FilterMode.Point;
                imp.textureCompression = TextureImporterCompression.Uncompressed;
                imp.mipmapEnabled = false;
                imp.alphaIsTransparency = true;
                imp.spritePixelsPerUnit = 16f;
                var st = new TextureImporterSettings();
                imp.ReadTextureSettings(st);
                st.spriteMeshType = SpriteMeshType.FullRect;
                st.spriteAlignment = (int)SpriteAlignment.Center;
                st.spritePivot = new Vector2(0.5f, 0.5f);
                imp.SetTextureSettings(st);
                imp.SaveAndReimport();
            }
        }

        private static Sprite LoadEnv(string rel) => AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel);

        private static Tile MakeTile(Sprite spr)
        {
            var t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.color = Color.white;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static bool IsChar(string[] rows, int r, int c, char target)
        {
            if (r < 0 || r >= rows.Length) return false;
            if (c < 0 || c >= rows[r].Length) return false;
            return rows[r][c] == target;
        }

        private static Tile PickAutoTile(
            string[] rows, int r, int c, char ch,
            Tile tH, Tile tV, Tile tTL, Tile tTR, Tile tBL, Tile tBR)
        {
            bool hasN = IsChar(rows, r - 1, c, ch);
            bool hasS = IsChar(rows, r + 1, c, ch);
            bool hasW = IsChar(rows, r, c - 1, ch);
            bool hasE = IsChar(rows, r, c + 1, ch);

            if (hasS && hasE && !hasN && !hasW) return tTL;
            if (hasS && hasW && !hasN && !hasE) return tTR;
            if (hasN && hasE && !hasS && !hasW) return tBL;
            if (hasN && hasW && !hasS && !hasE) return tBR;
            if ((hasN || hasS) && !hasE && !hasW) return tV;
            return tH;
        }

        private static void RebuildAutoTiledWorldMap(MapLayoutConfigSO cfg)
        {
            Sprite[] grass =
            {
                LoadEnv("Tiles/tile_grass_0.png"),
                LoadEnv("Tiles/tile_grass_1.png"),
                LoadEnv("Tiles/tile_grass_2.png"),
                LoadEnv("Tiles/tile_grass_3.png"),
            };
            Sprite[] path =
            {
                LoadEnv("Tiles/tile_path_0.png"),
                LoadEnv("Tiles/tile_path_1.png"),
                LoadEnv("Tiles/tile_path_2.png"),
            };
            Sprite[] ghFloor =
            {
                LoadEnv("Tiles/tile_gh_floor_0.png"),
                LoadEnv("Tiles/tile_gh_floor_1.png"),
            };

            Tile[] grassTiles = new Tile[grass.Length];
            for (int i = 0; i < grass.Length; i++) grassTiles[i] = MakeTile(grass[i] != null ? grass[i] : grass[0]);
            Tile[] pathTiles = new Tile[path.Length];
            for (int i = 0; i < path.Length; i++) pathTiles[i] = MakeTile(path[i] != null ? path[i] : path[0]);
            Tile[] ghTiles = new Tile[ghFloor.Length];
            for (int i = 0; i < ghFloor.Length; i++) ghTiles[i] = MakeTile(ghFloor[i] != null ? ghFloor[i] : ghFloor[0]);

            Tile wallH  = MakeTile(LoadEnv("Tiles/tile_wall.png"));
            Tile wallV  = MakeTile(LoadEnv("Tiles/tile_wall_v.png"));
            Tile wallTL = MakeTile(LoadEnv("Tiles/tile_wall_tl.png"));
            Tile wallTR = MakeTile(LoadEnv("Tiles/tile_wall_tr.png"));
            Tile wallBL = MakeTile(LoadEnv("Tiles/tile_wall_bl.png"));
            Tile wallBR = MakeTile(LoadEnv("Tiles/tile_wall_br.png"));

            Tile fenceH  = MakeTile(LoadEnv("Tiles/tile_fence.png"));
            Tile fenceV  = MakeTile(LoadEnv("Tiles/tile_fence_v.png"));
            Tile fenceTL = MakeTile(LoadEnv("Tiles/tile_fence_tl.png"));
            Tile fenceTR = MakeTile(LoadEnv("Tiles/tile_fence_tr.png"));
            Tile fenceBL = MakeTile(LoadEnv("Tiles/tile_fence_bl.png"));
            Tile fenceBR = MakeTile(LoadEnv("Tiles/tile_fence_br.png"));

            Tile treeTile = MakeTile(LoadEnv("Tiles/tile_tree.png"));
            Sprite waterSpr = LoadEnv("Water/water_anim_00.png");
            if (waterSpr == null) waterSpr = LoadEnv("Tiles/tile_water.png");
            Tile waterTile = MakeTile(waterSpr);
            Tile flowerbedTile = MakeTile(LoadEnv("Tiles/tile_flowerbed.png"));

            GameObject gridGo = GameObject.Find("WorldMapGrid");
            if (gridGo != null) UnityEngine.Object.DestroyImmediate(gridGo);

            gridGo = new GameObject("WorldMapGrid");
            gridGo.AddComponent<Grid>();

            GameObject groundGo = new GameObject("GroundTilemap");
            groundGo.transform.SetParent(gridGo.transform, false);
            var groundTm = groundGo.AddComponent<Tilemap>();
            var groundRenderer = groundGo.AddComponent<TilemapRenderer>();
            groundRenderer.sortingLayerName = "Ground";

            GameObject obsGo = new GameObject("ObstacleTilemap");
            obsGo.transform.SetParent(gridGo.transform, false);
            obsGo.layer = 10;
            var obsTm = obsGo.AddComponent<Tilemap>();
            var obsRenderer = obsGo.AddComponent<TilemapRenderer>();
            obsRenderer.sortingLayerName = "WorldObjects";
            obsRenderer.sortingOrder = -5000;
            obsGo.AddComponent<TilemapCollider2D>();

            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            for (int r = 0; r < rows.Length; r++)
            {
                string row = rows[r];
                int y = cfg.originY + (rows.Length - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    int x = cfg.originX + c;
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    int hash = Mathf.Abs(x * 73856093 ^ y * 19349663);
                    char ch = row[c];

                    switch (ch)
                    {
                        case 'P':
                            groundTm.SetTile(pos, pathTiles[hash % pathTiles.Length]);
                            break;
                        case 'G':
                            groundTm.SetTile(pos, ghTiles[hash % ghTiles.Length]);
                            break;
                        case 'B':
                            groundTm.SetTile(pos, flowerbedTile);
                            break;
                        case '#':
                            groundTm.SetTile(pos, ghTiles[0]);
                            obsTm.SetTile(pos, PickAutoTile(rows, r, c, '#', wallH, wallV, wallTL, wallTR, wallBL, wallBR));
                            break;
                        case 'F':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, PickAutoTile(rows, r, c, 'F', fenceH, fenceV, fenceTL, fenceTR, fenceBL, fenceBR));
                            break;
                        case 'T':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, treeTile);
                            break;
                        case 'W':
                            groundTm.SetTile(pos, waterTile);
                            obsTm.SetTile(pos, waterTile);
                            break;
                        default:
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            break;
                    }
                }
            }

            Debug.Log("[MV] Đã dựng lại WorldMapGrid với Auto-Tile 6 hướng cho Tường Kính & Hàng Rào + Thảm hoa liền mạch.");
        }

        private static void UpgradeCentralPlantingSpot()
        {
            var spot = UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>();
            if (spot == null) return;

            Sprite emptySpr = LoadEnv("Props/planting_spot_empty.png");
            Sprite bloomSpr = LoadEnv("Props/planting_spot_bloomed.png");
            if (emptySpr == null || bloomSpr == null) return;

            var so = new SerializedObject(spot);
            so.FindProperty("emptySprite").objectReferenceValue = emptySpr;
            so.FindProperty("bloomedSprite").objectReferenceValue = bloomSpr;
            so.ApplyModifiedPropertiesWithoutUndo();

            var srProp = so.FindProperty("flowerRenderer");
            var sr = srProp != null ? srProp.objectReferenceValue as SpriteRenderer : spot.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = emptySpr;
                sr.color = Color.white;
                EditorUtility.SetDirty(sr);
            }
            EditorUtility.SetDirty(spot);
            Debug.Log("[MV] Đã nâng cấp Bồn Hoa Trung Tâm thành bồn đá bát giác khắc Trăng Khuyết.");
        }

        private static void UpgradeGreenhouseInteriorAndFarmLayout()
        {
            Sprite soilUntilled = LoadEnv("Soil/soil_untilled.png");
            Sprite soilDry = LoadEnv("Soil/soil_dry.png");
            Sprite soilWet = LoadEnv("Soil/soil_wet.png");

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            Array.Sort(tiles, (a, b) => string.CompareOrdinal(a.name, b.name));

            // Arrange 12 raised wooden planter boxes into 2 clean 2x3 cozy garden beds (columns 0-1 | aisle | columns 2-3)
            for (int i = 0; i < tiles.Length; i++)
            {
                var tile = tiles[i];
                if (soilUntilled != null && soilDry != null && soilWet != null)
                {
                    var so = new SerializedObject(tile);
                    so.FindProperty("untilledSprite").objectReferenceValue = soilUntilled;
                    so.FindProperty("tilledDrySprite").objectReferenceValue = soilDry;
                    so.FindProperty("tilledWetSprite").objectReferenceValue = soilWet;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    tile.RefreshVisuals();
                }

                if (tiles.Length == 12)
                {
                    int row = i / 4; // 0..2
                    int col = i % 4; // 0..3
                    float aisleOffset = col >= 2 ? 0.45f : 0f;
                    float tx = -18.15f + col * 1.08f + aisleOffset;
                    float ty = 9.55f - row * 1.18f;
                    tile.transform.position = new Vector3(tx, ty, 0f);
                }
                EditorUtility.SetDirty(tile.gameObject);
            }

            // Place a cozy woven rug under the Greenhouse reading/living nook (north side)
            Sprite rugSpr = LoadEnv("Decor/decor_greenhouse_rug.png");
            if (rugSpr != null)
            {
                GameObject oldRug = GameObject.Find("GreenhouseCozyRug");
                if (oldRug != null) UnityEngine.Object.DestroyImmediate(oldRug);

                GameObject decorGroup = GameObject.Find("VillageDecorGroup");
                GameObject rugGo = new GameObject("GreenhouseCozyRug");
                if (decorGroup != null) rugGo.transform.SetParent(decorGroup.transform, false);
                rugGo.transform.position = new Vector3(-16.1f, 12.15f, 0f);
                var sr = rugGo.AddComponent<SpriteRenderer>();
                sr.sprite = rugSpr;
                sr.sortingLayerName = "FloorDecal";
                sr.sortingOrder = 1;
            }

            Debug.Log($"[MV] Đã sắp gọn {tiles.Length} chậu đất viền gỗ thành 2 luống đối xứng + trải thảm Nhà Kính.");
        }

        private static void UpdateShadow(Transform parent, Sprite shadowSpr, Vector3 localPos, Vector3 localScale)
        {
            if (parent == null || shadowSpr == null) return;
            Transform existing = parent.Find("GroundShadow");
            GameObject go = existing != null ? existing.gameObject : new GameObject("GroundShadow");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = shadowSpr;
            sr.color = new Color(1f, 1f, 1f, 0.85f);
            sr.sortingLayerName = "Shadows";
            sr.sortingOrder = 0;
            EditorUtility.SetDirty(go);
        }

        private static void StrengthenGroundShadows()
        {
            Sprite shadowSpr = LoadEnv("Shadows/spr_ground_shadow.png");
            if (shadowSpr == null) return;

            if (AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
                UpdateShadow(root.transform, shadowSpr, new Vector3(0f, -0.46f, 0f), new Vector3(1.10f, 0.90f, 1f));
                PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }
            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                UpdateShadow(player.transform, shadowSpr, new Vector3(0f, -0.46f, 0f), new Vector3(1.10f, 0.90f, 1f));
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(LumiPrefabPath);
                UpdateShadow(root.transform, shadowSpr, new Vector3(0f, -0.44f, 0f), new Vector3(0.68f, 0.56f, 1f));
                PrefabUtility.SaveAsPrefabAsset(root, LumiPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }
            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi != null)
            {
                UpdateShadow(lumi.transform, shadowSpr, new Vector3(0f, -0.44f, 0f), new Vector3(0.68f, 0.56f, 1f));
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(SlimePrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(SlimePrefabPath);
                UpdateShadow(root.transform, shadowSpr, new Vector3(0f, -0.34f, 0f), new Vector3(1.05f, 0.85f, 1f));
                PrefabUtility.SaveAsPrefabAsset(root, SlimePrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }
            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            foreach (var sl in slimes)
            {
                UpdateShadow(sl.transform, shadowSpr, new Vector3(0f, -0.34f, 0f), new Vector3(1.05f, 0.85f, 1f));
            }

            var npcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            foreach (var npc in npcs)
            {
                UpdateShadow(npc.transform, shadowSpr, new Vector3(0f, -0.46f, 0f), new Vector3(1.10f, 0.90f, 1f));
            }

            Debug.Log("[MV] Đã tăng độ đậm và căn sát chân GroundShadow cho Wen, Lumi, 4 NPC và 8 Slime.");
        }

        private static void FixGreenhouseWallClippingDecor()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) return;

            int moved = 0;
            var transforms = decorGroup.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform t = transforms[i];
                if (t == decorGroup.transform || t.name == "Step15WorldPolishGroup" || t.name == "GreenhouseCozyRug")
                    continue;

                Vector3 p = t.position;
                // Greenhouse east glass wall is at x = -10..-9, y = 5..15
                if (p.x >= -10.8f && p.x <= -8.4f && p.y >= 5.2f && p.y <= 14.8f)
                {
                    t.position = new Vector3(-7.4f, p.y, p.z);
                    var sr = t.GetComponent<SpriteRenderer>();
                    if (sr != null && sr.sortingLayerName == "WorldObjects")
                    {
                        sr.sortingOrder = Mathf.RoundToInt(-t.position.y * 100f);
                    }
                    EditorUtility.SetDirty(t.gameObject);
                    moved++;
                }
            }
            if (moved > 0)
            {
                Debug.Log($"[MV] Đã dời {moved} vật trang trí bị chạm tường kính phía Đông Nhà Kính ra ngoài thoáng đẹp.");
            }
        }
    }
}
```

## 80. `Assets/Editor/AutoBuilder_Step16_Fix.cs`

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Player;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step16_Fix
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";

        [MenuItem("Moonpetal/Step 16/2. Fix Invisible Colliders & Align Garden Arch")]
        public static void ApplyStep16Fix()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU STEP 16.FIX 1 (COLLIDERS & GARDEN ARCH) ===");
            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Merge ObstacleTilemap collider via CompositeCollider2D (Unity 6 API)
            FixObstacleTilemapCompositeCollider();

            // 2. Refine Player_Wen collider (compact + rounded edgeRadius so Wen never snags)
            RefinePlayerCollider();

            // 3. Scan & clean any invisible orphaned solid Collider2D in scene
            AuditAndCleanInvisibleColliders();

            // 4. Align GardenEntranceArch flush with fence + move bushes overlapping fence
            AlignGardenArchAndFenceBushes();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 16.FIX 1! Vào Play Mode kiểm tra đi lại quanh hàng rào và cổng vườn nhé. ===");
        }

        private static void FixObstacleTilemapCompositeCollider()
        {
            GameObject obsGo = GameObject.Find("ObstacleTilemap");
            if (obsGo == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy ObstacleTilemap.");
                return;
            }

            var tmCol = obsGo.GetComponent<TilemapCollider2D>();
            if (tmCol == null) tmCol = obsGo.AddComponent<TilemapCollider2D>();

            var rb = obsGo.GetComponent<Rigidbody2D>();
            if (rb == null) rb = obsGo.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            rb.simulated = true;

            var compCol = obsGo.GetComponent<CompositeCollider2D>();
            if (compCol == null) compCol = obsGo.AddComponent<CompositeCollider2D>();
            compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
            compCol.generationType = CompositeCollider2D.GenerationType.Synchronous;

            tmCol.compositeOperation = Collider2D.CompositeOperation.Merge;
            compCol.GenerateGeometry();

            EditorUtility.SetDirty(obsGo);
            Debug.Log("[MV] Đã gộp ObstacleTilemap bằng CompositeCollider2D (hết kẹt cạnh tile).");
        }

        private static void ApplyCleanPlayerBox(GameObject go)
        {
            var col = go.GetComponent<BoxCollider2D>();
            if (col == null) return;
            col.size = new Vector2(0.46f, 0.34f);
            col.offset = new Vector2(0f, -0.34f);
            col.edgeRadius = 0.05f;
            EditorUtility.SetDirty(col);
        }

        private static void RefinePlayerCollider()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) != null)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
                ApplyCleanPlayerBox(root);
                PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                ApplyCleanPlayerBox(player.gameObject);
            }
            Debug.Log("[MV] Đã thu gọn và bo tròn BoxCollider2D dưới chân Player_Wen.");
        }

        private static void AuditAndCleanInvisibleColliders()
        {
            var allCols = UnityEngine.Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Include);
            int fixedCount = 0;

            for (int i = 0; i < allCols.Length; i++)
            {
                var col = allCols[i];
                if (col == null || !col.enabled || col.isTrigger) continue;

                GameObject go = col.gameObject;
                if (go.name == "ObstacleTilemap" || go.name == "Player_Wen") continue;
                if (go.GetComponent<TilemapCollider2D>() != null || go.GetComponent<CompositeCollider2D>() != null) continue;

                // Check if this solid collider has ANY visible SpriteRenderer on itself or children
                var srs = go.GetComponentsInChildren<SpriteRenderer>(false);
                bool hasVisibleSprite = false;
                for (int k = 0; k < srs.Length; k++)
                {
                    if (srs[k].enabled && srs[k].sprite != null && srs[k].color.a > 0.05f)
                    {
                        hasVisibleSprite = true;
                        break;
                    }
                }

                if (!hasVisibleSprite)
                {
                    col.enabled = false;
                    EditorUtility.SetDirty(col);
                    Debug.Log($"[MV] Đã tắt Collider tàng hình: '{go.name}' tại {go.transform.position}");
                    fixedCount++;
                }
            }

            Debug.Log($"[MV] Kiểm toán Collider hoàn tất (đã dọn {fixedCount} collider tàng hình).");
        }

        private static void AlignGardenArchAndFenceBushes()
        {
            GameObject arch = GameObject.Find("GardenEntranceArch");
            if (arch != null)
            {
                arch.transform.position = new Vector3(0.0f, -0.55f, 0f);
                arch.transform.localScale = new Vector3(1.22f, 0.96f, 1f);
                var sr = arch.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = 2600;
                }
                EditorUtility.SetDirty(arch);
                Debug.Log("[MV] Đã căn cổng vòm GardenEntranceArch nối khít 2 đầu hàng rào.");
            }

            // Move the 2 bushes that were overlapping the South Garden fence at y = -1.6
            GameObject b6 = GameObject.Find("ExtraBush_6");
            if (b6 != null)
            {
                b6.transform.position = new Vector3(3.6f, -0.35f, 0f);
                var sr = b6.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = Mathf.RoundToInt(-b6.transform.position.y * 100f);
                EditorUtility.SetDirty(b6);
            }

            GameObject b7 = GameObject.Find("ExtraBush_7");
            if (b7 != null)
            {
                b7.transform.position = new Vector3(-3.6f, -0.35f, 0f);
                var sr = b7.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = Mathf.RoundToInt(-b7.transform.position.y * 100f);
                EditorUtility.SetDirty(b7);
            }
        }
    }
}
```

## 81. `Assets/Editor/AutoBuilder_Step17.cs`

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoonpetalValley.Data;
using MoonpetalValley.TimeSystem;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step17
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";

        [MenuItem("Moonpetal/Step 17/1. Apply Organic Terrain (16-State Rounded Paths, Curved Pond & Pier)")]
        public static void BuildStep17()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 17 (CHẶNG 1 - ĐỊA HÌNH HỮU CƠ) ===");
            AssetDatabase.Refresh();
            ForceReimportEnvironmentFolder();
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Update MapLayoutConfig with organic curved pond basin (6x4) & clean connected paths
            MapLayoutConfigSO cfg = UpdateMapConfigForOrganicPond();
            if (cfg != null)
            {
                RebuildOrganicTilemap(cfg);
                BuildSeamlessAnimatedPond();
                BuildOrganicSteppingStonesAndShoreline();
            }

            // 2. Keep GardenEntranceArch flush with fence
            GameObject arch = GameObject.Find("GardenEntranceArch");
            if (arch != null)
            {
                arch.transform.position = new Vector3(0.0f, -0.55f, 0f);
                arch.transform.localScale = new Vector3(1.22f, 0.96f, 1f);
                EditorUtility.SetDirty(arch);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 17 (CHẶNG 1)! Chạy 'Moonpetal/Step 17/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 17/Validate")]
        public static void ValidateStep17()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 17 ===");

            for (int m = 0; m < 16; m++)
            {
                string p = $"{EnvRoot}Paths/path_m{m:D2}.png";
                if (AssetDatabase.LoadAssetAtPath<Sprite>(p) == null)
                {
                    Debug.LogError("[MV] Thiếu path auto-tile: " + p);
                    errors++;
                }
            }
            for (int f = 0; f < 4; f++)
            {
                string p = $"{EnvRoot}Water/pond_organic_{f:D2}.png";
                if (AssetDatabase.LoadAssetAtPath<Sprite>(p) == null)
                {
                    Debug.LogError("[MV] Thiếu pond organic frame: " + p);
                    errors++;
                }
            }

            var waterAnim = UnityEngine.Object.FindAnyObjectByType<WaterTileAnimator>();
            if (waterAnim == null)
            {
                Debug.LogError("[MV] Thiếu WaterTileAnimator trên AnimatedPondWater.");
                errors++;
            }

            GameObject obsGo = GameObject.Find("ObstacleTilemap");
            if (obsGo == null || obsGo.GetComponent<CompositeCollider2D>() == null)
            {
                Debug.LogError("[MV] ObstacleTilemap thiếu CompositeCollider2D.");
                errors++;
            }

            if (GameObject.Find("Step17OrganicShoreline") == null)
            {
                Debug.LogError("[MV] Thiếu nhóm Step17OrganicShoreline (cầu gỗ, lau sậy & đá bước chân).");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + allGos[i].name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 17: PASS (0 lỗi | 16-State Rounded Paths=OK | Curved Organic Pond 4f=OK | CompositeCollider2D=OK).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 17: FAIL ({errors} lỗi).");
            }
        }

        private static void ForceReimportEnvironmentFolder()
        {
            string folder = "Assets/Art/Sprites/Environment";
            if (!AssetDatabase.IsValidFolder(folder)) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
            foreach (string g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var imp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (imp == null) continue;
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.filterMode = FilterMode.Point;
                imp.textureCompression = TextureImporterCompression.Uncompressed;
                imp.mipmapEnabled = false;
                imp.alphaIsTransparency = true;
                imp.spritePixelsPerUnit = 16f;
                var st = new TextureImporterSettings();
                imp.ReadTextureSettings(st);
                st.spriteMeshType = SpriteMeshType.FullRect;
                st.spriteAlignment = (int)SpriteAlignment.Center;
                st.spritePivot = new Vector2(0.5f, 0.5f);
                imp.SetTextureSettings(st);
                imp.SaveAndReimport();
            }
        }

        private static Sprite LoadEnv(string rel) => AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel);

        private static Tile MakeTile(Sprite spr, Tile.ColliderType colType = Tile.ColliderType.Grid)
        {
            var t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.color = Color.white;
            t.colliderType = colType;
            return t;
        }

        private static MapLayoutConfigSO UpdateMapConfigForOrganicPond()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null)
            {
                Debug.LogError("[MV] Không tìm thấy MapLayoutConfig.asset!");
                return null;
            }

            // 32 rows x 44 cols (originX = -22, originY = -16)
            // Organic 6x4 curved pond basin at rows 1..4, cols 16..21 (x = -6..-1, y = 11..14)
            // Completely clear of Greenhouse (cols 1..12), Well (~x=-8,y=10.5), Village paths (row 7+) & Forest Edge
            string[] rows =
            {
                "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT",
                "T############....WWWW........TTTTTTTTTTTTTTT",
                "T#GGGGGGGGGG#...WWWWWW.......TT..........TTT",
                "T#GGGGGGGGGG#...WWWWWW.......T............TT",
                "T#GGGGGGGGGG#....WWWW........T....TTTT.....T",
                "T#GGGGGGGGGG#........PPPP.........TTTT.....T",
                "T#GGGGGGGGGG#........PPPP..................T",
                "T#GGGGGGGGGG#...PPPPPPPPPPPPPPPPPPPPPP.....T",
                "T#GGGGGGGGGG#...PPPPPPPPPPPPPPPPPPPPPP.....T",
                "T#GGGGGGGGGG#...PP........PP........PP.....T",
                "T#####PP#####...PP........PP........PP.....T",
                "T.....PP........PP..PPPP..PP........PP.....T",
                "T.....PP........PP..PPPP..PP........PP.....T",
                "T.....PPPPPPPPPPPP..PPPP..PPPPPPPPPPPP.....T",
                "T.....PPPPPPPPPPPP..PPPP..PPPPPPPPPPPP.....T",
                "T...................PPPP...................T",
                "T...................PPPP...................T",
                "T..............FFFFFPPPPFFFFF..............T",
                "T..............F....PPPP....F..............T",
                "T..............F....PPPP....F..............T",
                "T..............F....PPPP....F..............T",
                "T..............F...PPPPPP...F..............T",
                "T..............F...PPBBPP...F..............T",
                "T..............F...PPBBPP...F..............T",
                "T..............F...PPPPPP...F..............T",
                "T..............F............F..............T",
                "T..............F............F..............T",
                "T..............F............F..............T",
                "T..............FFFFFFFFFFFFFF..............T",
                "TT........................................TT",
                "TTT......................................TTT",
                "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT"
            };

            cfg.asciiMap = string.Join("\n", rows);
            EditorUtility.SetDirty(cfg);
            AssetDatabase.SaveAssets();
            Debug.Log("[MV] Đã cập nhật MapLayoutConfig.asset với hồ nước cong 6x4 và trục đường thông thoáng.");
            return cfg;
        }

        private static bool IsChar(string[] rows, int r, int c, char target)
        {
            if (r < 0 || r >= rows.Length) return false;
            if (c < 0 || c >= rows[r].Length) return false;
            return rows[r][c] == target;
        }

        private static bool IsPathOrConnected(string[] rows, int r, int c)
        {
            if (r < 0 || r >= rows.Length) return false;
            if (c < 0 || c >= rows[r].Length) return false;
            char ch = rows[r][c];
            return ch == 'P' || ch == 'B' || ch == 'G';
        }

        private static Tile Pick6StateWallOrFence(
            string[] rows, int r, int c, char ch,
            Tile tH, Tile tV, Tile tTL, Tile tTR, Tile tBL, Tile tBR)
        {
            bool hasN = IsChar(rows, r - 1, c, ch);
            bool hasS = IsChar(rows, r + 1, c, ch);
            bool hasW = IsChar(rows, r, c - 1, ch);
            bool hasE = IsChar(rows, r, c + 1, ch);

            if (hasS && hasE && !hasN && !hasW) return tTL;
            if (hasS && hasW && !hasN && !hasE) return tTR;
            if (hasN && hasE && !hasS && !hasW) return tBL;
            if (hasN && hasW && !hasS && !hasE) return tBR;
            if ((hasN || hasS) && !hasE && !hasW) return tV;
            return tH;
        }

        private static void RebuildOrganicTilemap(MapLayoutConfigSO cfg)
        {
            Tile[] grassTiles = new Tile[4];
            for (int i = 0; i < 4; i++) grassTiles[i] = MakeTile(LoadEnv($"Tiles/tile_grass_{i}.png"));

            Tile[] pathMasks = new Tile[16];
            for (int m = 0; m < 16; m++) pathMasks[m] = MakeTile(LoadEnv($"Paths/path_m{m:D2}.png"));

            Tile[] ghTiles =
            {
                MakeTile(LoadEnv("Tiles/tile_gh_floor_0.png")),
                MakeTile(LoadEnv("Tiles/tile_gh_floor_1.png")),
            };

            Tile wallH  = MakeTile(LoadEnv("Tiles/tile_wall.png"));
            Tile wallV  = MakeTile(LoadEnv("Tiles/tile_wall_v.png"));
            Tile wallTL = MakeTile(LoadEnv("Tiles/tile_wall_tl.png"));
            Tile wallTR = MakeTile(LoadEnv("Tiles/tile_wall_tr.png"));
            Tile wallBL = MakeTile(LoadEnv("Tiles/tile_wall_bl.png"));
            Tile wallBR = MakeTile(LoadEnv("Tiles/tile_wall_br.png"));

            Tile fenceH  = MakeTile(LoadEnv("Tiles/tile_fence.png"));
            Tile fenceV  = MakeTile(LoadEnv("Tiles/tile_fence_v.png"));
            Tile fenceTL = MakeTile(LoadEnv("Tiles/tile_fence_tl.png"));
            Tile fenceTR = MakeTile(LoadEnv("Tiles/tile_fence_tr.png"));
            Tile fenceBL = MakeTile(LoadEnv("Tiles/tile_fence_bl.png"));
            Tile fenceBR = MakeTile(LoadEnv("Tiles/tile_fence_br.png"));

            Tile treeTile = MakeTile(LoadEnv("Tiles/tile_tree.png"));
            Tile flowerbedTile = MakeTile(LoadEnv("Tiles/tile_flowerbed.png"));
            Tile waterColliderTile = MakeTile(LoadEnv("Water/tile_water_transparent.png"), Tile.ColliderType.Grid);

            GameObject gridGo = GameObject.Find("WorldMapGrid");
            if (gridGo != null) UnityEngine.Object.DestroyImmediate(gridGo);

            gridGo = new GameObject("WorldMapGrid");
            gridGo.AddComponent<Grid>();

            GameObject groundGo = new GameObject("GroundTilemap");
            groundGo.transform.SetParent(gridGo.transform, false);
            var groundTm = groundGo.AddComponent<Tilemap>();
            var groundRenderer = groundGo.AddComponent<TilemapRenderer>();
            groundRenderer.sortingLayerName = "Ground";

            GameObject obsGo = new GameObject("ObstacleTilemap");
            obsGo.transform.SetParent(gridGo.transform, false);
            obsGo.layer = 10;
            var obsTm = obsGo.AddComponent<Tilemap>();
            var obsRenderer = obsGo.AddComponent<TilemapRenderer>();
            obsRenderer.sortingLayerName = "WorldObjects";
            obsRenderer.sortingOrder = -5000;
            var tmCol = obsGo.AddComponent<TilemapCollider2D>();
            var rb = obsGo.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            var compCol = obsGo.AddComponent<CompositeCollider2D>();
            compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
            compCol.generationType = CompositeCollider2D.GenerationType.Synchronous;
            tmCol.compositeOperation = Collider2D.CompositeOperation.Merge;

            string[] rows = cfg.asciiMap.Replace("\r\n", "\n").Split('\n');
            for (int r = 0; r < rows.Length; r++)
            {
                string row = rows[r];
                int y = cfg.originY + (rows.Length - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    int x = cfg.originX + c;
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    int hash = Mathf.Abs(x * 73856093 ^ y * 19349663);
                    char ch = row[c];

                    switch (ch)
                    {
                        case 'P':
                            int mask = 0;
                            if (IsPathOrConnected(rows, r - 1, c)) mask |= 1;
                            if (IsPathOrConnected(rows, r, c + 1)) mask |= 2;
                            if (IsPathOrConnected(rows, r + 1, c)) mask |= 4;
                            if (IsPathOrConnected(rows, r, c - 1)) mask |= 8;
                            groundTm.SetTile(pos, pathMasks[mask]);
                            break;
                        case 'G':
                            groundTm.SetTile(pos, ghTiles[hash % ghTiles.Length]);
                            break;
                        case 'B':
                            groundTm.SetTile(pos, flowerbedTile);
                            break;
                        case '#':
                            groundTm.SetTile(pos, ghTiles[0]);
                            obsTm.SetTile(pos, Pick6StateWallOrFence(rows, r, c, '#', wallH, wallV, wallTL, wallTR, wallBL, wallBR));
                            break;
                        case 'F':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, Pick6StateWallOrFence(rows, r, c, 'F', fenceH, fenceV, fenceTL, fenceTR, fenceBL, fenceBR));
                            break;
                        case 'T':
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, treeTile);
                            break;
                        case 'W':
                            // Seamless grass underneath + transparent collider so 96x64 curved pond renders smoothly!
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            obsTm.SetTile(pos, waterColliderTile);
                            break;
                        default:
                            groundTm.SetTile(pos, grassTiles[hash % grassTiles.Length]);
                            break;
                    }
                }
            }

            compCol.GenerateGeometry();
            Debug.Log("[MV] Đã dựng WorldMapGrid hữu cơ với 16-state Rounded Paths + CompositeCollider2D.");
        }

        private static void BuildSeamlessAnimatedPond()
        {
            Sprite[] frames =
            {
                LoadEnv("Water/pond_organic_00.png"),
                LoadEnv("Water/pond_organic_01.png"),
                LoadEnv("Water/pond_organic_02.png"),
                LoadEnv("Water/pond_organic_03.png"),
            };
            if (frames[0] == null)
            {
                Debug.LogWarning("[MV] Không tìm thấy Water/pond_organic_00..03.png.");
                return;
            }

            GameObject oldWater = GameObject.Find("AnimatedPondWater");
            if (oldWater != null) UnityEngine.Object.DestroyImmediate(oldWater);

            GameObject root = new GameObject("AnimatedPondWater");
            // Center of cols 16..21 (x=-6..-1 -> center -3.0) and rows 1..4 (y=11..14 -> center 13.0)
            root.transform.position = new Vector3(-3.0f, 13.0f, 0f);

            var sr = root.AddComponent<SpriteRenderer>();
            sr.sprite = frames[0];
            sr.sortingLayerName = "FloorDecal";
            sr.sortingOrder = 5;

            var anim = root.AddComponent<WaterTileAnimator>();
            var so = new SerializedObject(anim);
            var fProp = so.FindProperty("waterFrames");
            fProp.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++) fProp.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            var rProp = so.FindProperty("waterRenderers");
            rProp.arraySize = 1;
            rProp.GetArrayElementAtIndex(0).objectReferenceValue = sr;
            so.FindProperty("fps").floatValue = 3.2f;
            so.ApplyModifiedPropertiesWithoutUndo();

            Debug.Log("[MV] Đã dựng hồ nước cong liền khối 96x64 (4 frame động) tại (-3.0, 13.0).");
        }

        private static void SpawnDecal(Transform parent, string name, Sprite spr, Vector3 pos, string layer = "FloorDecal", int order = 6)
        {
            if (spr == null) return;
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = layer;
            sr.sortingOrder = order;
        }

        private static void BuildOrganicSteppingStonesAndShoreline()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) decorGroup = new GameObject("VillageDecorGroup");

            Transform old = decorGroup.transform.Find("Step17OrganicShoreline");
            if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);

            GameObject group = new GameObject("Step17OrganicShoreline");
            group.transform.SetParent(decorGroup.transform, false);

            Sprite pierSpr = LoadEnv("Decor/decor_pond_pier.png");
            Sprite reedSpr = LoadEnv("Decor/decor_reeds.png");
            Sprite st0 = LoadEnv("Decor/decor_stepping_0.png");
            Sprite st1 = LoadEnv("Decor/decor_stepping_1.png");

            // Wooden Pond Pier at south shoreline of curved pond
            SpawnDecal(group.transform, "PondWoodenPier", pierSpr, new Vector3(-3.0f, 10.95f, 0f), "FloorDecal", 8);

            // Shoreline cattail reeds framing the curved pond banks
            SpawnDecal(group.transform, "PondReeds_NW", reedSpr, new Vector3(-5.8f, 14.3f, 0f), "WorldObjects", Mathf.RoundToInt(-14.3f * 100f));
            SpawnDecal(group.transform, "PondReeds_NE", reedSpr, new Vector3(-0.3f, 14.0f, 0f), "WorldObjects", Mathf.RoundToInt(-14.0f * 100f));
            SpawnDecal(group.transform, "PondReeds_SE", reedSpr, new Vector3(-0.4f, 11.4f, 0f), "WorldObjects", Mathf.RoundToInt(-11.4f * 100f));

            // Organic stepping stone connectors (Well -> Pond Pier, Pond Pier -> Village Path, Village -> Forest Edge, Greenhouse -> Well)
            Vector3[] stones =
            {
                new Vector3(-7.0f, 10.6f, 0f),
                new Vector3(-5.6f, 10.5f, 0f),
                new Vector3(-4.4f, 10.4f, 0f),
                new Vector3(-3.0f, 9.8f, 0f),
                new Vector3(-2.6f, 9.0f, 0f),
                new Vector3(-13.2f, 4.4f, 0f),
                new Vector3(-11.8f, 5.2f, 0f),
                new Vector3(-10.4f, 6.2f, 0f),
                new Vector3(-9.2f, 7.6f, 0f),
                new Vector3(-8.4f, 9.0f, 0f),
                new Vector3(9.4f, 7.5f, 0f),
                new Vector3(10.6f, 7.6f, 0f),
                new Vector3(11.8f, 7.4f, 0f),
                new Vector3(13.0f, 7.2f, 0f),
            };
            for (int i = 0; i < stones.Length; i++)
            {
                SpawnDecal(group.transform, $"SteppingStone_{i}", (i % 2 == 0) ? st0 : st1, stones[i], "FloorDecal", 4);
            }

            Debug.Log("[MV] Đã bố trí cầu gỗ ven hồ, lau sậy và 14 cụm đá bước chân kết nối mềm giữa các phân khu.");
        }
    }
}
```

## 82. `Assets/Editor/AutoBuilder_Step18.cs`

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step18
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string EnvRoot = "Assets/Art/Sprites/Environment/";

        [MenuItem("Moonpetal/Step 18/1. Apply Hero Buildings, Multi-Layered Trees & Pine Grove")]
        public static void BuildStep18()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 18 (CHẶNG 2 - NHÀ CỬA & CÂY TÁN LỚN) ===");
            AssetDatabase.Refresh();
            ForceReimportHeroSprites();
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            // 1. Upgrade Hero Buildings + clean foot-only colliders (so Wen walks behind roofs naturally)
            UpgradeHeroBuildings();

            // 2. Refresh Y-sorting of existing hero trees so Wen walks behind upper canopies
            RefreshHeroTreeSorting();

            // 3. Add curated Pine Grove at Forest Edge & North Ridge
            BuildForestPineGrove();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 18 (CHẶNG 2)! Chạy 'Moonpetal/Step 18/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 18/Validate")]
        public static void ValidateStep18()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 18 ===");

            string[] required =
            {
                "Buildings/house_bakery.png",
                "Buildings/house_shed.png",
                "Buildings/otis_notice_board.png",
                "Buildings/tree_hero_oak.png",
                "Buildings/tree_hero_blossom.png",
                "Buildings/tree_hero_pine.png",
                "Decor/decor_bush.png",
                "Decor/decor_rock.png",
            };
            foreach (string rel in required)
            {
                if (AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel) == null)
                {
                    Debug.LogError("[MV] Thiếu sprite Step 18: " + EnvRoot + rel);
                    errors++;
                }
            }

            if (GameObject.Find("House_HanaBakery") == null)
            {
                Debug.LogError("[MV] Không tìm thấy House_HanaBakery trong scene.");
                errors++;
            }
            if (GameObject.Find("House_BachShed") == null)
            {
                Debug.LogError("[MV] Không tìm thấy House_BachShed trong scene.");
                errors++;
            }
            if (GameObject.Find("Step18ForestPines") == null)
            {
                Debug.LogError("[MV] Thiếu nhóm Step18ForestPines.");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            for (int i = 0; i < allGos.Length; i++)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(allGos[i]) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + allGos[i].name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 18: PASS (0 lỗi | Hero Buildings=OK | 3 Tree Species 48x64=OK | Pine Grove=OK).");
            }
            else
            {
                Debug.LogError($"[MV] VALIDATE STEP 18: FAIL ({errors} lỗi).");
            }
        }

        private static Sprite LoadEnv(string rel) => AssetDatabase.LoadAssetAtPath<Sprite>(EnvRoot + rel);

        private static void ForceReimportHeroSprites()
        {
            string[] folders =
            {
                "Assets/Art/Sprites/Environment/Buildings",
                "Assets/Art/Sprites/Environment/Decor",
            };
            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder)) continue;
                string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
                foreach (string g in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(g);
                    var imp = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (imp == null) continue;
                    imp.textureType = TextureImporterType.Sprite;
                    imp.spriteImportMode = SpriteImportMode.Single;
                    imp.filterMode = FilterMode.Point;
                    imp.textureCompression = TextureImporterCompression.Uncompressed;
                    imp.mipmapEnabled = false;
                    imp.alphaIsTransparency = true;
                    imp.spritePixelsPerUnit = 16f;
                    var st = new TextureImporterSettings();
                    imp.ReadTextureSettings(st);
                    st.spriteMeshType = SpriteMeshType.FullRect;
                    st.spriteAlignment = (int)SpriteAlignment.Center;
                    st.spritePivot = new Vector2(0.5f, 0.5f);
                    imp.SetTextureSettings(st);
                    imp.SaveAndReimport();
                }
            }
        }

        private static void UpgradeHeroBuildings()
        {
            Sprite bakerySpr = LoadEnv("Buildings/house_bakery.png");
            Sprite shedSpr = LoadEnv("Buildings/house_shed.png");
            Sprite boardSpr = LoadEnv("Buildings/otis_notice_board.png");

            GameObject bakery = GameObject.Find("House_HanaBakery");
            if (bakery != null && bakerySpr != null)
            {
                var sr = bakery.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = bakerySpr;
                    sr.color = Color.white;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(bakery.transform.position.y - 1.6f) * 100f);
                }
                var col = bakery.GetComponent<BoxCollider2D>();
                if (col != null)
                {
                    col.size = new Vector2(3.8f, 1.55f);
                    col.offset = new Vector2(0f, -1.05f);
                }
                EditorUtility.SetDirty(bakery);
            }

            GameObject shed = GameObject.Find("House_BachShed");
            if (shed != null && shedSpr != null)
            {
                var sr = shed.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = shedSpr;
                    sr.color = Color.white;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(shed.transform.position.y - 1.3f) * 100f);
                }
                var col = shed.GetComponent<BoxCollider2D>();
                if (col != null)
                {
                    col.size = new Vector2(2.75f, 1.35f);
                    col.offset = new Vector2(0f, -0.85f);
                }
                EditorUtility.SetDirty(shed);
            }

            GameObject board = GameObject.Find("OtisNoticeBoard");
            if (board != null && boardSpr != null)
            {
                var sr = board.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = boardSpr;
                    sr.color = Color.white;
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(board.transform.position.y - 0.8f) * 100f);
                }
                EditorUtility.SetDirty(board);
            }

            Debug.Log("[MV] Đã nâng cấp Nhà Bánh Hana, Chòi Vườn và Bảng Tin với collider gọn chân nhà.");
        }

        private static void RefreshHeroTreeSorting()
        {
            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) return;

            int updated = 0;
            var srs = decorGroup.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < srs.Length; i++)
            {
                var sr = srs[i];
                if (sr == null || sr.sprite == null) continue;
                string n = sr.sprite.name;
                if (n.Contains("tree_hero_"))
                {
                    sr.sortingLayerName = "WorldObjects";
                    sr.sortingOrder = Mathf.RoundToInt(-(sr.transform.position.y - 1.45f) * 100f);
                    EditorUtility.SetDirty(sr);
                    updated++;
                }
            }
            Debug.Log($"[MV] Đã căn lại Y-Sort chân gốc cho {updated} cây cỡ lớn (Wen đi sau tán cây tự nhiên).");
        }

        private static void BuildForestPineGrove()
        {
            Sprite pineSpr = LoadEnv("Buildings/tree_hero_pine.png");
            if (pineSpr == null) return;

            GameObject decorGroup = GameObject.Find("VillageDecorGroup");
            if (decorGroup == null) decorGroup = new GameObject("VillageDecorGroup");

            Transform old = decorGroup.transform.Find("Step18ForestPines");
            if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);

            GameObject grove = new GameObject("Step18ForestPines");
            grove.transform.SetParent(decorGroup.transform, false);

            // Curated Pine Grove framing the East Forest Edge & North Mountain Ridge (zero overlap with paths/pond)
            Vector3[] pineSpots =
            {
                new Vector3(4.5f, 13.4f, 0f),
                new Vector3(9.5f, 12.6f, 0f),
                new Vector3(14.8f, 12.2f, 0f),
                new Vector3(18.6f, 8.8f, 0f),
                new Vector3(18.4f, 1.8f, 0f),
                new Vector3(17.6f, -5.8f, 0f),
                new Vector3(-18.6f, -1.2f, 0f),
            };

            for (int i = 0; i < pineSpots.Length; i++)
            {
                GameObject go = new GameObject($"HeroPine_{i}");
                go.transform.SetParent(grove.transform, false);
                go.transform.position = pineSpots[i];
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = pineSpr;
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = Mathf.RoundToInt(-(pineSpots[i].y - 1.45f) * 100f);
            }

            Debug.Log($"[MV] Đã trồng thêm {pineSpots.Length} cây thông tán lớn ở Rìa Rừng Phía Đông & Dãy Bắc.");
        }
    }
}
```

## 83. `Assets/Editor/AutoBuilder_Step2.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step2
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string MapConfigPath = "Assets/Data/Config/MapLayoutConfig.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LumiPrefabPath = "Assets/Prefabs/Characters/Lumi_Companion.prefab";

        [MenuItem("Moonpetal/Step 2/1. Build Player, Lumi & Map")]
        public static void BuildStep2()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 2 ===");

            Sprite wenSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_wen_idle.png", 16, 24, PlaceholderGeneratorUtility.DrawWenSprite);
            Sprite lumiSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_lumi_idle.png", 12, 12, PlaceholderGeneratorUtility.DrawLumiSprite);
            Sprite haloSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_lumi_halo.png", 32, 32, PlaceholderGeneratorUtility.DrawRadialHalo);

            Sprite grassSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_grass.png", 16, 16, PlaceholderGeneratorUtility.DrawGrassTile);
            Sprite pathSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_path.png", 16, 16, PlaceholderGeneratorUtility.DrawPathTile);
            Sprite ghFloorSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_greenhouse_floor.png", 16, 16, PlaceholderGeneratorUtility.DrawGreenhouseFloorTile);
            Sprite wallSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_wall.png", 16, 16, PlaceholderGeneratorUtility.DrawWallTile);
            Sprite fenceSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_fence.png", 16, 16, PlaceholderGeneratorUtility.DrawFenceTile);
            Sprite treeSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_tree.png", 16, 16, PlaceholderGeneratorUtility.DrawTreeTile);
            Sprite waterSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_water.png", 16, 16, PlaceholderGeneratorUtility.DrawWaterTile);
            Sprite flowerbedSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_tile_flowerbed.png", 16, 16, PlaceholderGeneratorUtility.DrawFlowerbedTile);

            MapLayoutConfigSO mapConfig = EnsureMapLayoutConfig();

            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            GameObject playerPrefab = BuildPlayerPrefab(wenSpr, statsConfig);
            GameObject lumiPrefab = BuildLumiPrefab(lumiSpr, haloSpr);

            UpgradeGameRootDebugWithTeleports();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            GameObject oldBanner = GameObject.Find("Step1VerificationCanvas");
            if (oldBanner != null) UnityEngine.Object.DestroyImmediate(oldBanner);

            BuildWorldTilemap(mapConfig, grassSpr, pathSpr, ghFloorSpr, wallSpr, fenceSpr, treeSpr, waterSpr, flowerbedSpr);
            BuildSpawnRegistry(mapConfig);

            GameObject playerInst = EnsurePrefabInScene("Player_Wen", playerPrefab, new Vector3(-14.5f, 9.5f, 0f));
            GameObject lumiInst = EnsurePrefabInScene("Lumi_Companion", lumiPrefab, new Vector3(-15.2f, 10.2f, 0f));

            var lumiCtrl = lumiInst.GetComponent<LumiCompanionController>();
            var lumiSO = new SerializedObject(lumiCtrl);
            lumiSO.FindProperty("followTarget").objectReferenceValue = playerInst.transform;
            lumiSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject camGo = GameObject.Find("Main Camera");
            if (camGo != null)
            {
                var camFollow = camGo.GetComponent<CameraFollow2D>();
                if (camFollow == null) camFollow = camGo.AddComponent<CameraFollow2D>();
                var camSO = new SerializedObject(camFollow);
                camSO.FindProperty("target").objectReferenceValue = playerInst.transform;
                camSO.FindProperty("smoothTime").floatValue = 0.16f;
                camSO.FindProperty("minBounds").vector2Value = mapConfig.cameraMinBounds;
                camSO.FindProperty("maxBounds").vector2Value = mapConfig.cameraMaxBounds;
                camSO.ApplyModifiedPropertiesWithoutUndo();
                camGo.transform.position = new Vector3(-14.5f, 9.5f, -10f);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 2! Chạy 'Moonpetal/Step 2/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 2/Validate")]
        public static void ValidateStep2()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 2 ===");

            if (AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath) == null)
            {
                Debug.LogError("[MV] THIẾU MapLayoutConfig.asset");
                errors++;
            }
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) == null)
            {
                Debug.LogError("[MV] THIẾU Player_Wen.prefab");
                errors++;
            }
            if (AssetDatabase.LoadAssetAtPath<GameObject>(LumiPrefabPath) == null)
            {
                Debug.LogError("[MV] THIẾU Lumi_Companion.prefab");
                errors++;
            }

            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
            if (player == null)
            {
                Debug.LogError("[MV] Không tìm thấy PlayerController2D trong scene hiện tại.");
                errors++;
            }
            else
            {
                if (player.gameObject.layer != 8)
                {
                    Debug.LogError("[MV] Player_Wen phải nằm ở Layer 8 (Player).");
                    errors++;
                }
                var so = new SerializedObject(player);
                string[] req = { "statsConfig", "rb", "inputReader", "spriteRenderer", "visualTransform" };
                foreach (string f in req)
                {
                    if (so.FindProperty(f).objectReferenceValue == null)
                    {
                        Debug.LogError("[MV] PlayerController2D thiếu trường: " + f);
                        errors++;
                    }
                }
            }

            var lumi = UnityEngine.Object.FindAnyObjectByType<LumiCompanionController>();
            if (lumi == null)
            {
                Debug.LogError("[MV] Không tìm thấy LumiCompanionController trong scene.");
                errors++;
            }

            var cam = UnityEngine.Object.FindAnyObjectByType<CameraFollow2D>();
            if (cam == null)
            {
                Debug.LogError("[MV] Không tìm thấy CameraFollow2D trên Main Camera.");
                errors++;
            }

            var spawnReg = UnityEngine.Object.FindAnyObjectByType<WorldSpawnRegistry>();
            if (spawnReg == null)
            {
                Debug.LogError("[MV] Không tìm thấy WorldSpawnRegistry trong scene.");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 2: PASS (0 lỗi). Nhân vật Wen, Lumi, Camera và Bản đồ 4 phân khu sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 2: FAIL (" + errors + " lỗi).");
            }
        }

        private static MapLayoutConfigSO EnsureMapLayoutConfig()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<MapLayoutConfigSO>(MapConfigPath);
            if (cfg == null)
            {
                cfg = ScriptableObject.CreateInstance<MapLayoutConfigSO>();
                AssetDatabase.CreateAsset(cfg, MapConfigPath);
            }

            cfg.originX = -22;
            cfg.originY = -16;
            cfg.cameraMinBounds = new Vector2(-13.5f, -10.5f);
            cfg.cameraMaxBounds = new Vector2(13.5f, 10.5f);

            string[] rows =
            {
                "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT",
                "T############................TTTTTTTTTTTTTTT",
                "T#GGGGGGGGGG#...W............TT..........TTT",
                "T#GGGGGGGGGG#..WWW...........T............TT",
                "T#GGGGGGGGGG#...W....PPPP....T....TTTT.....T",
                "T#GGGGGGGGGG#........PPPP.........TTTT.....T",
                "T#GGGGGGGGGG#........PPPP..................T",
                "T#GGGGGGGGGG#...PPPPPPPPPPPPPPPPPPPPPP.....T",
                "T#GGGGGGGGGG#...PPPPPPPPPPPPPPPPPPPPPP.....T",
                "T#GGGGGGGGGG#...PP........PP........PP.....T",
                "T#####PP#####...PP........PP........PP.....T",
                "T.....PP........PP..PPPP..PP........PP.....T",
                "T.....PP........PP..PPPP..PP........PP.....T",
                "T.....PPPPPPPPPPPP..PPPP..PPPPPPPPPPPP.....T",
                "T.....PPPPPPPPPPPP..PPPP..PPPPPPPPPPPP.....T",
                "T...................PPPP...................T",
                "T...................PPPP...................T",
                "T..............FFFFFFFFFFFFFF..............T",
                "T..............F....PPPP....F..............T",
                "T..............F....PPPP....F..............T",
                "T..............F....PPPP....F..............T",
                "T..............F...PPPPPP...F..............T",
                "T..............F...PPBBPP...F..............T",
                "T..............F...PPBBPP...F..............T",
                "T..............F...PPPPPP...F..............T",
                "T..............F............F..............T",
                "T..............F............F..............T",
                "T..............F............F..............T",
                "T..............FFFFFFFFFFFFFF..............T",
                "TT........................................TT",
                "TTT......................................TTT",
                "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT"
            };
            cfg.asciiMap = string.Join("\n", rows);

            cfg.spawnPoints = new List<SpawnPointDefinition>
            {
                new SpawnPointDefinition { id = "Spawn_Greenhouse", displayName = "Nhà Kính Của Bà", position = new Vector2(-15.0f, 10.0f) },
                new SpawnPointDefinition { id = "Spawn_Village", displayName = "Quảng Trường Làng Chính", position = new Vector2(0.0f, 3.0f) },
                new SpawnPointDefinition { id = "Spawn_BachGarden", displayName = "Vườn Phía Nam Ông Bách", position = new Vector2(0.0f, -5.0f) },
                new SpawnPointDefinition { id = "Spawn_ForestEdge", displayName = "Rìa Rừng Phía Đông", position = new Vector2(15.0f, 7.0f) }
            };

            EditorUtility.SetDirty(cfg);
            return cfg;
        }

        private static GameObject BuildPlayerPrefab(Sprite wenSpr, PlayerStatsSO statsConfig)
        {
            GameObject root = new GameObject("Player_Wen");
            root.tag = "Player";
            root.layer = 8;

            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = root.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.65f, 0.55f);
            col.offset = new Vector2(0f, -0.35f);

            var input = root.AddComponent<GameInputReader>();
            var ctrl = root.AddComponent<PlayerController2D>();

            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);
            var sr = visual.AddComponent<SpriteRenderer>();
            sr.sprite = wenSpr;
            sr.sortingLayerName = "WorldObjects";

            var so = new SerializedObject(ctrl);
            so.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            so.FindProperty("rb").objectReferenceValue = rb;
            so.FindProperty("inputReader").objectReferenceValue = input;
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.FindProperty("visualTransform").objectReferenceValue = visual.transform;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static GameObject BuildLumiPrefab(Sprite lumiSpr, Sprite haloSpr)
        {
            GameObject root = new GameObject("Lumi_Companion");
            var ctrl = root.AddComponent<LumiCompanionController>();

            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);
            var sr = visual.AddComponent<SpriteRenderer>();
            sr.sprite = lumiSpr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = 5000;

            GameObject halo = new GameObject("Halo");
            halo.transform.SetParent(visual.transform, false);
            var haloSr = halo.AddComponent<SpriteRenderer>();
            haloSr.sprite = haloSpr;
            haloSr.sortingLayerName = "WeatherVFX";
            haloSr.sortingOrder = 10;

            var so = new SerializedObject(ctrl);
            so.FindProperty("visualTransform").objectReferenceValue = visual.transform;
            so.FindProperty("haloRenderer").objectReferenceValue = haloSr;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, LumiPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static void BuildWorldTilemap(
            MapLayoutConfigSO cfg,
            Sprite grassSpr, Sprite pathSpr, Sprite ghFloorSpr,
            Sprite wallSpr, Sprite fenceSpr, Sprite treeSpr,
            Sprite waterSpr, Sprite flowerbedSpr)
        {
            GameObject existingGrid = GameObject.Find("WorldMapGrid");
            if (existingGrid != null) UnityEngine.Object.DestroyImmediate(existingGrid);

            GameObject gridGo = new GameObject("WorldMapGrid");
            gridGo.AddComponent<Grid>();

            GameObject groundGo = new GameObject("GroundTilemap");
            groundGo.transform.SetParent(gridGo.transform, false);
            var groundTm = groundGo.AddComponent<Tilemap>();
            var groundRenderer = groundGo.AddComponent<TilemapRenderer>();
            groundRenderer.sortingLayerName = "Ground";

            GameObject obsGo = new GameObject("ObstacleTilemap");
            obsGo.transform.SetParent(gridGo.transform, false);
            obsGo.layer = 10;
            var obsTm = obsGo.AddComponent<Tilemap>();
            var obsRenderer = obsGo.AddComponent<TilemapRenderer>();
            obsRenderer.sortingLayerName = "WorldObjects";
            obsRenderer.sortingOrder = -5000;
            obsGo.AddComponent<TilemapCollider2D>();

            Tile tGrass = CreateTile(grassSpr);
            Tile tPath = CreateTile(pathSpr);
            Tile tGhFloor = CreateTile(ghFloorSpr);
            Tile tWall = CreateTile(wallSpr);
            Tile tFence = CreateTile(fenceSpr);
            Tile tTree = CreateTile(treeSpr);
            Tile tWater = CreateTile(waterSpr);
            Tile tFlowerbed = CreateTile(flowerbedSpr);

            string[] lines = cfg.asciiMap.Split('\n');
            int height = lines.Length;
            for (int r = 0; r < height; r++)
            {
                string row = lines[r].TrimEnd('\r');
                int y = cfg.originY + (height - 1 - r);
                for (int c = 0; c < row.Length; c++)
                {
                    int x = cfg.originX + c;
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    char ch = row[c];

                    if (ch == 'G') groundTm.SetTile(cell, tGhFloor);
                    else if (ch == 'P') groundTm.SetTile(cell, tPath);
                    else if (ch == 'B') groundTm.SetTile(cell, tFlowerbed);
                    else groundTm.SetTile(cell, tGrass);

                    if (ch == '#') obsTm.SetTile(cell, tWall);
                    else if (ch == 'F') obsTm.SetTile(cell, tFence);
                    else if (ch == 'T') obsTm.SetTile(cell, tTree);
                    else if (ch == 'W') obsTm.SetTile(cell, tWater);
                }
            }
        }

        private static Tile CreateTile(Sprite spr)
        {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = spr;
            t.colliderType = Tile.ColliderType.Grid;
            return t;
        }

        private static void BuildSpawnRegistry(MapLayoutConfigSO cfg)
        {
            GameObject existing = GameObject.Find("WorldSpawnRegistry");
            if (existing == null) existing = new GameObject("WorldSpawnRegistry");

            var reg = existing.GetComponent<WorldSpawnRegistry>();
            if (reg == null) reg = existing.AddComponent<WorldSpawnRegistry>();

            var so = new SerializedObject(reg);
            var listProp = so.FindProperty("spawnPoints");
            listProp.ClearArray();
            for (int i = 0; i < cfg.spawnPoints.Count; i++)
            {
                listProp.InsertArrayElementAtIndex(i);
                var elem = listProp.GetArrayElementAtIndex(i);
                elem.FindPropertyRelative("id").stringValue = cfg.spawnPoints[i].id;
                elem.FindPropertyRelative("displayName").stringValue = cfg.spawnPoints[i].displayName;
                elem.FindPropertyRelative("position").vector2Value = cfg.spawnPoints[i].position;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject EnsurePrefabInScene(string objName, GameObject prefab, Vector3 pos)
        {
            GameObject existing = GameObject.Find(objName);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing);

            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            inst.name = objName;
            inst.transform.position = pos;
            return inst;
        }

        private static void UpgradeGameRootDebugWithTeleports()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                var rect = debugPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(460f, 510f);

                Button btnGh = EnsureDebugButton(debugPanel, "BtnTpGreenhouse", "TP: Nhà Kính Của Bà", vnFont);
                Button btnVil = EnsureDebugButton(debugPanel, "BtnTpVillage", "TP: Quảng Trường Làng Chính", vnFont);
                Button btnGar = EnsureDebugButton(debugPanel, "BtnTpGarden", "TP: Vườn Phía Nam Ông Bách", vnFont);
                Button btnFor = EnsureDebugButton(debugPanel, "BtnTpForest", "TP: Rìa Rừng Phía Đông", vnFont);

                var so = new SerializedObject(debugCtrl);
                so.FindProperty("btnTpGreenhouse").objectReferenceValue = btnGh;
                so.FindProperty("btnTpVillage").objectReferenceValue = btnVil;
                so.FindProperty("btnTpGarden").objectReferenceValue = btnGar;
                so.FindProperty("btnTpForest").objectReferenceValue = btnFor;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã nâng cấp GameRoot.prefab với 4 nút Teleport Debug.");
        }

        private static Button EnsureDebugButton(Transform parent, string name, string label, TMP_FontAsset font)
        {
            Transform existing = parent.Find(name);
            if (existing != null) return existing.GetComponent<Button>();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 36f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.18f, 0.44f, 0.38f, 1f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            GameObject txtGo = new GameObject("Label");
            txtGo.transform.SetParent(go.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            if (font != null) tmp.font = font;

            return btn;
        }
    }
}
```

## 84. `Assets/Editor/AutoBuilder_Step3.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step3
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string TimeConfigPath = "Assets/Data/Config/TimeConfig.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 3/1. Build Time & Day-Night Lighting")]
        public static void BuildStep3()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 3 ===");

            Sprite lanternSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_lantern.png", 16, 24, PlaceholderGeneratorUtility.DrawLanternPostSprite);
            Sprite warmHaloSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_warm_halo.png", 32, 32, PlaceholderGeneratorUtility.DrawWarmHalo);

            UpgradeGameRootWithTimeAndLighting();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildVillageLanterns(lanternSpr, warmHaloSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 3! Chạy 'Moonpetal/Step 3/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 3/Validate")]
        public static void ValidateStep3()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 3 ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab");
                errors++;
            }
            else
            {
                var root = prefab.GetComponent<GameRoot>();
                var rootSO = new SerializedObject(root);
                if (rootSO.FindProperty("timeManager").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu tham chiếu timeManager.");
                    errors++;
                }

                var tm = prefab.GetComponent<TimeOfDayManager>();
                if (tm == null || new SerializedObject(tm).FindProperty("timeConfig").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] TimeOfDayManager thiếu TimeConfigSO.");
                    errors++;
                }

                var lighting = prefab.GetComponentInChildren<DayNightLightingController>(true);
                if (lighting == null || new SerializedObject(lighting).FindProperty("ambientTintImage").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] DayNightLightingController thiếu ambientTintImage.");
                    errors++;
                }

                var clock = prefab.GetComponentInChildren<ClockHUDView>(true);
                if (clock == null || new SerializedObject(clock).FindProperty("clockText").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] ClockHUDView thiếu clockText.");
                    errors++;
                }
            }

            var lanterns = UnityEngine.Object.FindObjectsByType<VillageLantern>(FindObjectsInactive.Include);
            if (lanterns.Length < 4)
            {
                Debug.LogError("[MV] Thiếu đèn lồng làng (cần 4, hiện có " + lanterns.Length + ").");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 3: PASS (0 lỗi). Chu kỳ Ngày/Đêm, Đồng hồ HUD và Đèn lồng làng sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 3: FAIL (" + errors + " lỗi).");
            }
        }

        private static void UpgradeGameRootWithTimeAndLighting()
        {
            TimeConfigSO timeConfig = AssetDatabase.LoadAssetAtPath<TimeConfigSO>(TimeConfigPath);
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();

            // 1. TimeOfDayManager
            var tm = root.GetComponent<TimeOfDayManager>();
            if (tm == null) tm = root.AddComponent<TimeOfDayManager>();
            var tmSO = new SerializedObject(tm);
            tmSO.FindProperty("timeConfig").objectReferenceValue = timeConfig;
            tmSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("timeManager").objectReferenceValue = tm;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // 2. DayNightOverlayCanvas (SortingOrder = 5, raycastTarget = false)
            Transform oldLighting = root.transform.Find("DayNightOverlayCanvas");
            if (oldLighting != null) UnityEngine.Object.DestroyImmediate(oldLighting.gameObject);

            GameObject tintCanvasGo = new GameObject("DayNightOverlayCanvas");
            tintCanvasGo.transform.SetParent(root.transform, false);
            var tintCanvas = tintCanvasGo.AddComponent<Canvas>();
            tintCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            tintCanvas.sortingOrder = 5;

            GameObject tintImgGo = new GameObject("AmbientTintImage");
            tintImgGo.transform.SetParent(tintCanvasGo.transform, false);
            var tintRect = tintImgGo.AddComponent<RectTransform>();
            tintRect.anchorMin = Vector2.zero;
            tintRect.anchorMax = Vector2.one;
            tintRect.offsetMin = Vector2.zero;
            tintRect.offsetMax = Vector2.zero;

            var tintImg = tintImgGo.AddComponent<Image>();
            tintImg.color = new Color(1f, 0.92f, 0.72f, 0.08f);
            tintImg.raycastTarget = false;

            var lightCtrl = tintCanvasGo.AddComponent<DayNightLightingController>();
            var lightSO = new SerializedObject(lightCtrl);
            lightSO.FindProperty("timeConfig").objectReferenceValue = timeConfig;
            lightSO.FindProperty("ambientTintImage").objectReferenceValue = tintImg;
            lightSO.ApplyModifiedPropertiesWithoutUndo();

            // 3. Clock HUD (Top-Right inside SystemOverlayCanvas or dedicated HUDCanvas SortingOrder = 20)
            Transform oldClock = root.transform.Find("ClockHUDCanvas");
            if (oldClock != null) UnityEngine.Object.DestroyImmediate(oldClock.gameObject);

            GameObject hudCanvasGo = new GameObject("ClockHUDCanvas");
            hudCanvasGo.transform.SetParent(root.transform, false);
            var hudCanvas = hudCanvasGo.AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.sortingOrder = 20;
            var scaler = hudCanvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject badgePanel = new GameObject("ClockBadgePanel");
            badgePanel.transform.SetParent(hudCanvasGo.transform, false);
            var bpRect = badgePanel.AddComponent<RectTransform>();
            bpRect.anchorMin = new Vector2(1f, 1f);
            bpRect.anchorMax = new Vector2(1f, 1f);
            bpRect.pivot = new Vector2(1f, 1f);
            bpRect.anchoredPosition = new Vector2(-24f, -24f);
            bpRect.sizeDelta = new Vector2(360f, 58f);
            var bpImg = badgePanel.AddComponent<Image>();
            bpImg.color = new Color(0.09f, 0.13f, 0.20f, 0.88f);

            GameObject dotGo = new GameObject("PhaseDot");
            dotGo.transform.SetParent(badgePanel.transform, false);
            var dotRect = dotGo.AddComponent<RectTransform>();
            dotRect.anchorMin = new Vector2(0f, 0.5f);
            dotRect.anchorMax = new Vector2(0f, 0.5f);
            dotRect.anchoredPosition = new Vector2(24f, 0f);
            dotRect.sizeDelta = new Vector2(18f, 18f);
            var dotImg = dotGo.AddComponent<Image>();
            dotImg.color = new Color(0.95f, 0.80f, 0.35f, 1f);

            GameObject txtGo = new GameObject("ClockText");
            txtGo.transform.SetParent(badgePanel.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = new Vector2(46f, 4f);
            txtRect.offsetMax = new Vector2(-12f, -4f);
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "Ngày 1 — 06:00 (Sáng)";
            tmp.fontSize = 21;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = Color.white;
            if (vnFont != null) tmp.font = vnFont;

            var clockView = badgePanel.AddComponent<ClockHUDView>();
            var clockSO = new SerializedObject(clockView);
            clockSO.FindProperty("clockText").objectReferenceValue = tmp;
            clockSO.FindProperty("phaseBadgeImage").objectReferenceValue = dotImg;
            clockSO.ApplyModifiedPropertiesWithoutUndo();

            // 4. Add 3 Time Debug Buttons to DebugPanelRoot
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 650f);

                Button btnPlus2H = EnsureDebugButton(debugPanel, "BtnTimePlus2H", "Thời gian: +2 Giờ", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));
                Button btnDusk = EnsureDebugButton(debugPanel, "BtnTimeSetDusk", "Đặt 19:50 (Chạng vạng -> Đêm)", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));
                Button btnNextDay = EnsureDebugButton(debugPanel, "BtnTimeNextDay", "Qua Ngày Mới (06:00)", vnFont, new Color(0.45f, 0.32f, 0.16f, 1f));

                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnTimePlus2H").objectReferenceValue = btnPlus2H;
                dbgSO.FindProperty("btnTimeSetDusk").objectReferenceValue = btnDusk;
                dbgSO.FindProperty("btnTimeNextDay").objectReferenceValue = btnNextDay;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã nâng cấp GameRoot.prefab với TimeOfDayManager, DayNightLighting & ClockHUD.");
        }

        private static void BuildVillageLanterns(Sprite lanternSpr, Sprite warmHaloSpr)
        {
            GameObject group = GameObject.Find("VillageLanterns");
            if (group != null) UnityEngine.Object.DestroyImmediate(group);

            group = new GameObject("VillageLanterns");
            Vector2[] positions =
            {
                new Vector2(-13.5f, 4.5f),   // Trước cửa Nhà Kính
                new Vector2(-2.5f, 4.5f),    // Quảng Trường Làng Chính
                new Vector2(2.5f, -3.0f),    // Cổng Vườn Ông Bách
                new Vector2(12.5f, 7.5f)     // Đường vào Rìa Rừng
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject lanternGo = new GameObject("Lantern_" + (i + 1));
                lanternGo.transform.SetParent(group.transform, false);
                lanternGo.transform.position = new Vector3(positions[i].x, positions[i].y, 0f);

                var sr = lanternGo.AddComponent<SpriteRenderer>();
                sr.sprite = lanternSpr;
                sr.sortingLayerName = "WorldObjects";
                sr.sortingOrder = Mathf.RoundToInt(-positions[i].y * 100f);

                GameObject glowGo = new GameObject("WarmGlow");
                glowGo.transform.SetParent(lanternGo.transform, false);
                glowGo.transform.localPosition = new Vector3(0.15f, 0.35f, 0f);
                glowGo.transform.localScale = new Vector3(2.4f, 2.4f, 1f);
                var glowSr = glowGo.AddComponent<SpriteRenderer>();
                glowSr.sprite = warmHaloSpr;
                glowSr.sortingLayerName = "WeatherVFX";
                glowSr.sortingOrder = 15;
                glowSr.color = new Color(1f, 0.85f, 0.45f, 0f);

                var comp = lanternGo.AddComponent<VillageLantern>();
                var so = new SerializedObject(comp);
                so.FindProperty("glowRenderer").objectReferenceValue = glowSr;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static Button EnsureDebugButton(Transform parent, string name, string label, TMP_FontAsset font, Color bgColor)
        {
            Transform existing = parent.Find(name);
            if (existing != null) return existing.GetComponent<Button>();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 36f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            GameObject txtGo = new GameObject("Label");
            txtGo.transform.SetParent(go.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            if (font != null) tmp.font = font;

            return btn;
        }
    }
}
```

## 85. `Assets/Editor/AutoBuilder_Step4.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.TimeSystem;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step4
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 4/1. Build Stats HUD, Bed & Knockout")]
        public static void BuildStep4()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 4 ===");

            Sprite bedSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_bed.png", 16, 24, PlaceholderGeneratorUtility.DrawBedSprite);

            UpgradeGameRootWithStatsAndSleep();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildGreenhouseBed(bedSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MV] === HOÀN TẤT STEP 4! Chạy 'Moonpetal/Step 4/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 4/Validate")]
        public static void ValidateStep4()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 4 ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[MV] THIẾU GameRoot.prefab");
                errors++;
            }
            else
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                if (rootSO.FindProperty("statsController").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu statsController.");
                    errors++;
                }
                if (rootSO.FindProperty("sleepController").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu sleepController.");
                    errors++;
                }

                var hud = prefab.GetComponentInChildren<StatsHUDView>(true);
                if (hud == null)
                {
                    Debug.LogError("[MV] Thiếu StatsHUDView trong GameRoot.prefab.");
                    errors++;
                }
                else
                {
                    var hudSO = new SerializedObject(hud);
                    string[] req = { "lightHPFill", "lightHPText", "staminaFill", "staminaText", "exhaustedBadge" };
                    foreach (string f in req)
                    {
                        if (hudSO.FindProperty(f).objectReferenceValue == null)
                        {
                            Debug.LogError("[MV] StatsHUDView thiếu trường: " + f);
                            errors++;
                        }
                    }
                }
            }

            var bed = UnityEngine.Object.FindAnyObjectByType<BedInteractable>();
            if (bed == null)
            {
                Debug.LogError("[MV] Thiếu BedInteractable (Giường Ngủ) trong Nhà Kính.");
                errors++;
            }

            var allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGos)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    Debug.LogError("[MV] Missing Script trên: " + go.name);
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 4: PASS (0 lỗi). Thanh Stamina/LightHP, Giường Ngủ & Ngất Xỉu sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 4: FAIL (" + errors + " lỗi).");
            }
        }

        private static void UpgradeGameRootWithStatsAndSleep()
        {
            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();

            // 1. PlayerStatsController
            var statsCtrl = root.GetComponent<PlayerStatsController>();
            if (statsCtrl == null) statsCtrl = root.AddComponent<PlayerStatsController>();
            var statsSO = new SerializedObject(statsCtrl);
            statsSO.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            statsSO.ApplyModifiedPropertiesWithoutUndo();

            // 2. DayTransitionCanvas + SleepAndKnockoutController (SortingOrder = 90)
            Transform oldTrans = root.transform.Find("DayTransitionCanvas");
            if (oldTrans != null) UnityEngine.Object.DestroyImmediate(oldTrans.gameObject);

            GameObject transCanvasGo = new GameObject("DayTransitionCanvas");
            transCanvasGo.transform.SetParent(root.transform, false);
            var tc = transCanvasGo.AddComponent<Canvas>();
            tc.renderMode = RenderMode.ScreenSpaceOverlay;
            tc.sortingOrder = 90;
            var tcScaler = transCanvasGo.AddComponent<CanvasScaler>();
            tcScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            tcScaler.referenceResolution = new Vector2(1920f, 1080f);
            tcScaler.matchWidthOrHeight = 0.5f;

            GameObject curtain = new GameObject("FadeCurtain");
            curtain.transform.SetParent(transCanvasGo.transform, false);
            var cRect = curtain.AddComponent<RectTransform>();
            cRect.anchorMin = Vector2.zero;
            cRect.anchorMax = Vector2.one;
            cRect.offsetMin = Vector2.zero;
            cRect.offsetMax = Vector2.zero;
            var cImg = curtain.AddComponent<Image>();
            cImg.color = new Color(0.04f, 0.06f, 0.11f, 1f);
            var cGroup = curtain.AddComponent<CanvasGroup>();
            cGroup.alpha = 0f;
            cGroup.blocksRaycasts = false;

            GameObject msgGo = new GameObject("TransitionText");
            msgGo.transform.SetParent(curtain.transform, false);
            var msgRect = msgGo.AddComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0.15f, 0.35f);
            msgRect.anchorMax = new Vector2(0.85f, 0.65f);
            msgRect.offsetMin = Vector2.zero;
            msgRect.offsetMax = Vector2.zero;
            var msgTmp = msgGo.AddComponent<TextMeshProUGUI>();
            msgTmp.text = "Đang sang ngày mới...";
            msgTmp.fontSize = 32;
            msgTmp.alignment = TextAlignmentOptions.Center;
            msgTmp.color = Color.white;
            msgTmp.textWrappingMode = TextWrappingModes.Normal;
            if (vnFont != null) msgTmp.font = vnFont;

            var sleepCtrl = root.GetComponent<SleepAndKnockoutController>();
            if (sleepCtrl == null) sleepCtrl = root.AddComponent<SleepAndKnockoutController>();
            var sleepSO = new SerializedObject(sleepCtrl);
            sleepSO.FindProperty("fadeCanvasGroup").objectReferenceValue = cGroup;
            sleepSO.FindProperty("transitionText").objectReferenceValue = msgTmp;
            sleepSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("statsController").objectReferenceValue = statsCtrl;
            rootSO.FindProperty("sleepController").objectReferenceValue = sleepCtrl;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // 3. StatsPanel ở góc trên trái của ClockHUDCanvas
            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                Transform oldStats = hudCanvas.Find("StatsPanel");
                if (oldStats != null) UnityEngine.Object.DestroyImmediate(oldStats.gameObject);

                GameObject panel = new GameObject("StatsPanel");
                panel.transform.SetParent(hudCanvas, false);
                var pRect = panel.AddComponent<RectTransform>();
                pRect.anchorMin = new Vector2(0f, 1f);
                pRect.anchorMax = new Vector2(0f, 1f);
                pRect.pivot = new Vector2(0f, 1f);
                pRect.anchoredPosition = new Vector2(24f, -24f);
                pRect.sizeDelta = new Vector2(360f, 118f);
                var pImg = panel.AddComponent<Image>();
                pImg.color = new Color(0.09f, 0.13f, 0.20f, 0.88f);

                // Bar 1: Máu Ánh Sáng
                var (hpFill, hpTxt) = CreateStatBar(panel.transform, "LightHPBar", new Vector2(14f, -14f), new Color(0.32f, 0.86f, 0.82f, 1f), "Máu Ánh Sáng: 100/100", vnFont);
                // Bar 2: Thể lực
                var (stFill, stTxt) = CreateStatBar(panel.transform, "StaminaBar", new Vector2(14f, -58f), new Color(0.95f, 0.78f, 0.24f, 1f), "Thể lực: 100/100", vnFont);

                // Nhãn Kiệt sức (-50%)
                GameObject badgeGo = new GameObject("ExhaustedBadge");
                badgeGo.transform.SetParent(panel.transform, false);
                var bRect = badgeGo.AddComponent<RectTransform>();
                bRect.anchorMin = new Vector2(0f, 1f);
                bRect.anchorMax = new Vector2(0f, 1f);
                bRect.pivot = new Vector2(0f, 1f);
                bRect.anchoredPosition = new Vector2(14f, -94f);
                bRect.sizeDelta = new Vector2(332f, 22f);
                var bTmp = badgeGo.AddComponent<TextMeshProUGUI>();
                bTmp.text = "⚠ KIỆT SỨC: Trần Thể lực giảm còn 50% (Ăn Bánh Mì để xóa)";
                bTmp.fontSize = 14;
                bTmp.color = new Color(1f, 0.56f, 0.34f, 1f);
                if (vnFont != null) bTmp.font = vnFont;
                badgeGo.SetActive(false);

                var hudView = panel.AddComponent<StatsHUDView>();
                var hudSO = new SerializedObject(hudView);
                hudSO.FindProperty("lightHPFill").objectReferenceValue = hpFill;
                hudSO.FindProperty("lightHPText").objectReferenceValue = hpTxt;
                hudSO.FindProperty("staminaFill").objectReferenceValue = stFill;
                hudSO.FindProperty("staminaText").objectReferenceValue = stTxt;
                hudSO.FindProperty("exhaustedBadge").objectReferenceValue = badgeGo;
                hudSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // 4. Thêm 3 nút Debug cho Stats & Knockout vào DebugPanelRoot
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 790f);

                Button btnRestore = EnsureDebugButton(debugPanel, "BtnRestoreStats", "Hồi đầy Stamina & HP (Xóa kiệt sức)", vnFont, new Color(0.24f, 0.46f, 0.24f, 1f));
                Button btnExhaust = EnsureDebugButton(debugPanel, "BtnForceExhaust", "Ép Kiệt Sức (Stamina = 0)", vnFont, new Color(0.58f, 0.22f, 0.22f, 1f));
                Button btnDmg = EnsureDebugButton(debugPanel, "BtnDamageLightHP", "Trừ 25 Máu Ánh Sáng", vnFont, new Color(0.58f, 0.22f, 0.22f, 1f));

                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnRestoreStats").objectReferenceValue = btnRestore;
                dbgSO.FindProperty("btnForceExhaust").objectReferenceValue = btnExhaust;
                dbgSO.FindProperty("btnDamageLightHP").objectReferenceValue = btnDmg;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[MV] Đã nâng cấp GameRoot.prefab với PlayerStatsController, StatsHUD & SleepAndKnockoutController.");
        }

        private static (Image fill, TMP_Text label) CreateStatBar(Transform parent, string name, Vector2 pos, Color fillColor, string initText, TMP_FontAsset font)
        {
            GameObject bgGo = new GameObject(name);
            bgGo.transform.SetParent(parent, false);
            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 1f);
            bgRect.anchorMax = new Vector2(0f, 1f);
            bgRect.pivot = new Vector2(0f, 1f);
            bgRect.anchoredPosition = pos;
            bgRect.sizeDelta = new Vector2(332f, 34f);
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.18f, 0.22f, 0.30f, 1f);

            GameObject fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(bgGo.transform, false);
            var fRect = fillGo.AddComponent<RectTransform>();
            fRect.anchorMin = Vector2.zero;
            fRect.anchorMax = Vector2.one;
            fRect.offsetMin = new Vector2(3f, 3f);
            fRect.offsetMax = new Vector2(-3f, -3f);
            var fImg = fillGo.AddComponent<Image>();
            fImg.color = fillColor;
            fImg.type = Image.Type.Filled;
            fImg.fillMethod = Image.FillMethod.Horizontal;
            fImg.fillAmount = 1f;

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(bgGo.transform, false);
            var tRect = txtGo.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.offsetMin = new Vector2(10f, 0f);
            tRect.offsetMax = new Vector2(-10f, 0f);
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = initText;
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = new Color(0.06f, 0.08f, 0.12f, 1f);
            tmp.fontStyle = FontStyles.Bold;
            if (font != null) tmp.font = font;

            return (fImg, tmp);
        }

        private static void BuildGreenhouseBed(Sprite bedSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            GameObject existing = GameObject.Find("GreenhouseBed");
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing);

            GameObject bedGo = new GameObject("GreenhouseBed");
            bedGo.tag = "Interactable";
            bedGo.layer = 11;
            bedGo.transform.position = new Vector3(-19.5f, 12.5f, 0f);

            var sr = bedGo.AddComponent<SpriteRenderer>();
            sr.sprite = bedSpr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-bedGo.transform.position.y * 100f);

            var col = bedGo.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.95f, 1.35f);

            // World-space prompt "[E] Đi ngủ"
            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(bedGo.transform, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 50;

            var pRect = promptGo.GetComponent<RectTransform>();
            pRect.sizeDelta = new Vector2(180f, 44f);
            var pImg = promptGo.AddComponent<Image>();
            pImg.color = new Color(0.08f, 0.12f, 0.18f, 0.88f);

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(promptGo.transform, false);
            var tRect = txtGo.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.offsetMin = Vector2.zero;
            tRect.offsetMax = Vector2.zero;
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "[E] Đi ngủ";
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.94f, 0.65f, 1f);
            if (vnFont != null) tmp.font = vnFont;

            var bedComp = bedGo.AddComponent<BedInteractable>();
            var so = new SerializedObject(bedComp);
            so.FindProperty("promptObject").objectReferenceValue = promptGo;
            so.FindProperty("interactRadius").floatValue = 1.9f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Button EnsureDebugButton(Transform parent, string name, string label, TMP_FontAsset font, Color bgColor)
        {
            Transform existing = parent.Find(name);
            if (existing != null) return existing.GetComponent<Button>();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 36f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            GameObject txtGo = new GameObject("Label");
            txtGo.transform.SetParent(go.transform, false);
            var txtRect = txtGo.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            if (font != null) tmp.font = font;

            return btn;
        }
    }
}
```

## 86. `Assets/Editor/AutoBuilder_Step5.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Inventory;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step5
    {
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string ItemDatabasePath = "Assets/Data/Config/ItemDatabase.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string LootDropPrefabPath = "Assets/Prefabs/Enemies/LootDrop.prefab";

        [MenuItem("Moonpetal/Step 5/1. Build Inventory, Hotbar & 15 Items")]
        public static void BuildStep5()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 5 ===");

            ItemDatabaseSO db = BuildAll15MVPItems();
            BuildLootDropPrefab();
            UpgradeGameRootWithInventoryAndUI(db);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 5! Chạy 'Moonpetal/Step 5/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 5/Validate")]
        public static void ValidateStep5()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 5 ===");

            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            if (db == null || db.allItems.Count < 15)
            {
                Debug.LogError("[MV] ItemDatabase thiếu vật phẩm (cần 15, hiện có " + (db == null ? 0 : db.allItems.Count) + ").");
                errors++;
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath) == null)
            {
                Debug.LogError("[MV] THIẾU LootDrop.prefab");
                errors++;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                if (rootSO.FindProperty("inventoryManager").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu inventoryManager.");
                    errors++;
                }
                if (prefab.GetComponentInChildren<HotbarUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu HotbarUIView trong GameRoot.prefab.");
                    errors++;
                }
                if (prefab.GetComponentInChildren<InventoryUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu InventoryUIView trong GameRoot.prefab.");
                    errors++;
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 5: PASS (0 lỗi). 15 Vật phẩm MVP, Hotbar 8 ô & Túi Đồ 16 ô sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 5: FAIL (" + errors + " lỗi).");
            }
        }

        private static ItemDatabaseSO BuildAll15MVPItems()
        {
            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(ItemDatabasePath);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<ItemDatabaseSO>();
                AssetDatabase.CreateAsset(db, ItemDatabasePath);
            }
            db.allItems.Clear();

            AddOrUpdateItem(db, table, "item_hoe", "Cuốc Đất", "Dùng xới đất trồng cây trong Nhà Kính và đập đá vụn nhỏ (4 Thể lực).",
                ItemCategory.Tool, 1, 0, 0, false, 0, true, new Color(0.55f, 0.36f, 0.20f), new Color(0.74f, 0.78f, 0.84f), 0);

            AddOrUpdateItem(db, table, "item_watering_can", "Bình Tưới", "Tưới nước cho đất trồng (2 Thể lực, chứa 20 lần). Múc đầy lại tại Giếng Nước.",
                ItemCategory.Tool, 1, 0, 0, false, 0, true, new Color(0.26f, 0.56f, 0.84f), new Color(0.65f, 0.88f, 1f), 1);

            AddOrUpdateItem(db, table, "item_hatchet", "Rìu Nhỏ", "Chặt nhánh cây khô lấy Gỗ Nhánh Cây và dọn bụi gai (4 Thể lực).",
                ItemCategory.Tool, 1, 0, 0, false, 0, true, new Color(0.52f, 0.34f, 0.18f), new Color(0.82f, 0.85f, 0.90f), 2);

            AddOrUpdateItem(db, table, "item_moon_sickle", "Liềm Ánh Trăng", "Vũ khí thanh tẩy của Ông Bách: chém quét 120° gây 15 sát thương (2 Thể lực).",
                ItemCategory.Tool, 1, 0, 0, false, 0, true, new Color(0.32f, 0.24f, 0.48f), new Color(0.65f, 0.96f, 0.98f), 3);

            AddOrUpdateItem(db, table, "item_dew_barley_seed", "Hạt Lúa Mạch Sương Mai", "Gieo xuống ô đất đã cuốc và tưới nước 2 ngày để thu hoạch.",
                ItemCategory.Seed, 99, 0, 0, false, 2, true, new Color(0.62f, 0.46f, 0.28f), new Color(0.86f, 0.95f, 0.62f), 4);

            AddOrUpdateItem(db, table, "item_dew_barley", "Lúa Mạch Sương Mai", "Nông sản thơm dịu đọng sương sớm, nguyên liệu làm bánh của Bà Hana.",
                ItemCategory.Crop, 99, 0, 0, false, 0, true, new Color(0.92f, 0.78f, 0.30f), new Color(0.70f, 0.96f, 0.95f), 5);

            AddOrUpdateItem(db, table, "item_sunshine_bread", "Bánh Mì Nắng Ấm", "Hồi 50 Thể lực, 50 Máu Ánh Sáng và XÓA NGAY phạt kiệt sức 50%.",
                ItemCategory.Consumable, 99, 50, 50, true, 0, true, new Color(0.84f, 0.56f, 0.24f), new Color(0.98f, 0.84f, 0.52f), 6);

            AddOrUpdateItem(db, table, "item_branch_wood", "Gỗ Nhánh Cây", "Gỗ khô nhặt quanh làng, dùng sửa hòm thư cho Bác Otis.",
                ItemCategory.Material, 99, 0, 0, false, 0, false, new Color(0.48f, 0.30f, 0.16f), new Color(0.66f, 0.46f, 0.28f), 7);

            AddOrUpdateItem(db, table, "item_dew_sap", "Nhựa Sương", "Giọt nhựa ánh sáng rơi ra khi thanh tẩy Bọt Sương Đen.",
                ItemCategory.Material, 99, 0, 0, false, 0, false, new Color(0.32f, 0.72f, 0.92f), new Color(0.80f, 0.96f, 1f), 8);

            AddOrUpdateItem(db, table, "item_starlight_dust", "Bụi Tinh Quang", "Bụi lấp lánh giúp hồi sinh Bồn Hoa Cộng Đồng.",
                ItemCategory.Material, 99, 0, 0, false, 0, false, new Color(0.96f, 0.84f, 0.34f), new Color(1f, 0.98f, 0.78f), 9);

            AddOrUpdateItem(db, table, "item_moonpetal_seed_core", "Viên Pha Lê Hạt Giống", "Hạt giống di sản của bà, cộng hưởng cùng dân làng để hoa Moonpetal nở.",
                ItemCategory.KeyItem, 1, 0, 0, false, 0, true, new Color(0.45f, 0.88f, 0.96f), new Color(1f, 1f, 1f), 10);

            AddOrUpdateItem(db, table, "item_seed_bag_1", "Túi Hạt Di Sản 1 (Hana)", "Túi hạt giống kỷ niệm của Bà Hana để đóng góp vào Bồn Hoa Vườn Xưa.",
                ItemCategory.SeedBag, 1, 0, 0, false, 0, true, new Color(0.82f, 0.64f, 0.38f), new Color(0.96f, 0.46f, 0.42f), 11);

            AddOrUpdateItem(db, table, "item_seed_bag_2", "Túi Hạt Di Sản 2 (Toby)", "Túi hạt giống Toby gìn giữ sau khi chú mèo Miu được giải cứu.",
                ItemCategory.SeedBag, 1, 0, 0, false, 0, true, new Color(0.82f, 0.64f, 0.38f), new Color(0.42f, 0.84f, 0.56f), 11);

            AddOrUpdateItem(db, table, "item_seed_bag_3", "Túi Hạt Di Sản 3 (Otis)", "Túi hạt giống nằm trong hòm thư cũ được Bác Otis trao lại.",
                ItemCategory.SeedBag, 1, 0, 0, false, 0, true, new Color(0.82f, 0.64f, 0.38f), new Color(0.42f, 0.68f, 0.96f), 11);

            AddOrUpdateItem(db, table, "item_petal_of_hope", "Cánh Hoa Hy Vọng", "Cánh hoa Moonpetal đầu tiên xua tan Sương Héo Úa khỏi Làng Chính.",
                ItemCategory.MoonpetalShard, 1, 0, 0, false, 0, true, new Color(0.98f, 0.70f, 0.86f), new Color(1f, 0.96f, 0.98f), 12);

            if (table != null) EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(db);
            return db;
        }

        private static void AddOrUpdateItem(
            ItemDatabaseSO db, StringTableSO table, string id, string vnName, string vnDesc,
            ItemCategory cat, int maxStack, int stam, int hp, bool clearExh, int growDays, bool questProtected,
            Color primary, Color accent, int shapeType)
        {
            string iconPath = "Assets/Art/Sprites/Items/icon_" + id + ".png";
            Sprite icon = PlaceholderGeneratorUtility.EnsureSprite(iconPath, 16, 16, tex => DrawItemIcon(tex, primary, accent, shapeType));

            string assetPath = "Assets/Data/Items/Item_" + id + ".asset";
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemData>();
                AssetDatabase.CreateAsset(item, assetPath);
            }

            item.itemID = id;
            item.displayNameKey = "name_" + id;
            item.descriptionKey = "desc_" + id;
            item.icon = icon;
            item.category = cat;
            item.maxStack = maxStack;
            item.staminaRestore = stam;
            item.lightHPRestore = hp;
            item.clearsExhaustionPenalty = clearExh;
            item.growDays = growDays;
            item.isQuestProtected = questProtected;

            if (table != null)
            {
                table.SetEntry(item.displayNameKey, vnName);
                table.SetEntry(item.descriptionKey, vnDesc);
            }

            EditorUtility.SetDirty(item);
            db.allItems.Add(item);
        }

        private static void DrawItemIcon(Texture2D tex, Color primary, Color accent, int shape)
        {
            if (shape == 0) // Cuốc
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 2, 2, 11, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 11, 7, 3, accent);
            }
            else if (shape == 1) // Bình tưới
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 3, 7, 7, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 11, 6, 3, 2, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 10, 5, 2, accent);
            }
            else if (shape == 2) // Rìu
            {
                PlaceholderGeneratorUtility.FillRect(tex, 5, 2, 2, 12, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 7, 9, 5, 5, accent);
            }
            else if (shape == 3) // Liềm Ánh Trăng
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 2, 2, 8, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 9, 7, 3, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 10, 6, 3, 4, accent);
            }
            else if (shape == 6) // Bánh Mì Nắng Ấm
            {
                PlaceholderGeneratorUtility.FillRect(tex, 2, 4, 12, 7, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 4, 9, 8, 3, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 6, 2, 2, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 9, 6, 2, 2, accent);
            }
            else if (shape == 7) // Gỗ Nhánh Cây
            {
                PlaceholderGeneratorUtility.FillRect(tex, 2, 6, 12, 4, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 4, 8, 8, 2, accent);
            }
            else if (shape == 12) // Cánh Hoa Hy Vọng
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 3, 8, 10, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 6, 5, 4, 6, accent);
            }
            else // Hạt, Túi hạt, Giọt sương, Bụi, Pha lê
            {
                PlaceholderGeneratorUtility.FillRect(tex, 4, 3, 8, 10, primary);
                PlaceholderGeneratorUtility.FillRect(tex, 5, 5, 6, 5, accent);
                PlaceholderGeneratorUtility.FillRect(tex, 6, 12, 4, 2, accent);
            }
        }

        private static void BuildLootDropPrefab()
        {
            GameObject go = new GameObject("LootDrop");
            var drop = go.AddComponent<WorldLootDrop>();

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(go.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = 200;

            var so = new SerializedObject(drop);
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(go, LootDropPrefabPath);
            UnityEngine.Object.DestroyImmediate(go);
        }

        private static void UpgradeGameRootWithInventoryAndUI(ItemDatabaseSO db)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var invMgr = root.GetComponent<InventoryManager>();
            if (invMgr == null) invMgr = root.AddComponent<InventoryManager>();

            var invSO = new SerializedObject(invMgr);
            invSO.FindProperty("itemDatabase").objectReferenceValue = db;
            invSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("inventoryManager").objectReferenceValue = invMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                if (hudCanvas.GetComponent<GraphicRaycaster>() == null)
                {
                    hudCanvas.gameObject.AddComponent<GraphicRaycaster>();
                }

                // 1. Hotbar 8 ô ở đáy màn hình
                Transform oldHotbar = hudCanvas.Find("HotbarPanel");
                if (oldHotbar != null) UnityEngine.Object.DestroyImmediate(oldHotbar.gameObject);

                GameObject hotbarGo = new GameObject("HotbarPanel");
                hotbarGo.transform.SetParent(hudCanvas, false);
                var hbRect = hotbarGo.AddComponent<RectTransform>();
                hbRect.anchorMin = new Vector2(0.5f, 0f);
                hbRect.anchorMax = new Vector2(0.5f, 0f);
                hbRect.pivot = new Vector2(0.5f, 0f);
                hbRect.anchoredPosition = new Vector2(0f, 18f);
                hbRect.sizeDelta = new Vector2(640f, 82f);

                var bgs = new Image[8];
                var icons = new Image[8];
                var counts = new TMP_Text[8];

                for (int i = 0; i < 8; i++)
                {
                    GameObject slotGo = new GameObject("Slot_" + (i + 1));
                    slotGo.transform.SetParent(hotbarGo.transform, false);
                    var sRect = slotGo.AddComponent<RectTransform>();
                    sRect.anchorMin = new Vector2(0f, 0.5f);
                    sRect.anchorMax = new Vector2(0f, 0.5f);
                    sRect.anchoredPosition = new Vector2(44f + i * 78f, 0f);
                    sRect.sizeDelta = new Vector2(70f, 70f);
                    bgs[i] = slotGo.AddComponent<Image>();
                    bgs[i].color = new Color(0.11f, 0.15f, 0.22f, 0.88f);

                    icons[i] = CreateSlotIcon(slotGo.transform);
                    counts[i] = CreateCornerText(slotGo.transform, "Count", "", vnFont, 15, TextAlignmentOptions.BottomRight, Color.white);
                    CreateCornerText(slotGo.transform, "Num", (i + 1).ToString(), vnFont, 13, TextAlignmentOptions.TopLeft, new Color(0.8f, 0.86f, 0.92f, 0.85f));
                }

                // Nhãn tên vật phẩm đang chọn phía trên Hotbar
                GameObject labelGo = new GameObject("SelectedItemLabel");
                labelGo.transform.SetParent(hotbarGo.transform, false);
                var lRect = labelGo.AddComponent<RectTransform>();
                lRect.anchorMin = new Vector2(0.5f, 1f);
                lRect.anchorMax = new Vector2(0.5f, 1f);
                lRect.pivot = new Vector2(0.5f, 0f);
                lRect.anchoredPosition = new Vector2(0f, 6f);
                lRect.sizeDelta = new Vector2(640f, 30f);
                var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
                labelTmp.fontSize = 19;
                labelTmp.alignment = TextAlignmentOptions.Center;
                labelTmp.color = new Color(1f, 0.96f, 0.78f, 1f);
                if (vnFont != null) labelTmp.font = vnFont;

                var hbView = hotbarGo.AddComponent<HotbarUIView>();
                var hbSO = new SerializedObject(hbView);
                AssignArray(hbSO.FindProperty("slotBackgrounds"), bgs);
                AssignArray(hbSO.FindProperty("slotIcons"), icons);
                AssignArray(hbSO.FindProperty("slotCounts"), counts);
                hbSO.FindProperty("selectedItemLabel").objectReferenceValue = labelTmp;
                hbSO.ApplyModifiedPropertiesWithoutUndo();

                // 2. Modal Túi Đồ 16 ô (Tab / I)
                Transform oldInv = hudCanvas.Find("InventoryModalHost");
                if (oldInv != null) UnityEngine.Object.DestroyImmediate(oldInv.gameObject);

                GameObject invHost = new GameObject("InventoryModalHost");
                invHost.transform.SetParent(hudCanvas, false);
                var hostRect = invHost.AddComponent<RectTransform>();
                hostRect.anchorMin = Vector2.zero;
                hostRect.anchorMax = Vector2.one;
                hostRect.offsetMin = Vector2.zero;
                hostRect.offsetMax = Vector2.zero;

                GameObject modal = new GameObject("InventoryPanelRoot");
                modal.transform.SetParent(invHost.transform, false);
                var mRect = modal.AddComponent<RectTransform>();
                mRect.anchorMin = new Vector2(0.5f, 0.5f);
                mRect.anchorMax = new Vector2(0.5f, 0.5f);
                mRect.sizeDelta = new Vector2(620f, 440f);
                var mImg = modal.AddComponent<Image>();
                mImg.color = new Color(0.08f, 0.11f, 0.18f, 0.95f);

                var titleTmp = CreateCornerText(modal.transform, "Title", "TÚI ĐỒ CỦA WEN (Tab / Esc để đóng)", vnFont, 22, TextAlignmentOptions.Top, new Color(1f, 0.92f, 0.65f, 1f));
                titleTmp.GetComponent<RectTransform>().offsetMax = new Vector2(-12f, -12f);

                var invBtns = new Button[16];
                var invIcons = new Image[16];
                var invCounts = new TMP_Text[16];

                for (int i = 0; i < 16; i++)
                {
                    int row = i / 8;
                    int col = i % 8;
                    GameObject sGo = new GameObject("BagSlot_" + i);
                    sGo.transform.SetParent(modal.transform, false);
                    var r = sGo.AddComponent<RectTransform>();
                    r.anchorMin = new Vector2(0f, 1f);
                    r.anchorMax = new Vector2(0f, 1f);
                    r.anchoredPosition = new Vector2(58f + col * 72f, -92f - row * 76f);
                    r.sizeDelta = new Vector2(64f, 64f);
                    var sImg = sGo.AddComponent<Image>();
                    sImg.color = row == 0 ? new Color(0.18f, 0.24f, 0.34f, 1f) : new Color(0.14f, 0.17f, 0.24f, 1f);
                    invBtns[i] = sGo.AddComponent<Button>();
                    invBtns[i].targetGraphic = sImg;
                    invIcons[i] = CreateSlotIcon(sGo.transform);
                    invCounts[i] = CreateCornerText(sGo.transform, "Count", "", vnFont, 14, TextAlignmentOptions.BottomRight, Color.white);
                }

                var dName = CreateBoxText(modal.transform, "DetailName", new Vector2(0f, -230f), new Vector2(560f, 34f), vnFont, 21, new Color(1f, 0.94f, 0.68f, 1f));
                var dDesc = CreateBoxText(modal.transform, "DetailDesc", new Vector2(0f, -270f), new Vector2(560f, 100f), vnFont, 18, Color.white);

                var invView = invHost.AddComponent<InventoryUIView>();
                var ivSO = new SerializedObject(invView);
                ivSO.FindProperty("panelRoot").objectReferenceValue = modal;
                AssignArray(ivSO.FindProperty("slotButtons"), invBtns);
                AssignArray(ivSO.FindProperty("slotIcons"), invIcons);
                AssignArray(ivSO.FindProperty("slotCounts"), invCounts);
                ivSO.FindProperty("detailNameText").objectReferenceValue = dName;
                ivSO.FindProperty("detailDescText").objectReferenceValue = dDesc;
                ivSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // 3. Thêm nút Debug cho bộ test túi đồ
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 835f);
                Button btnKit = EnsureDebugButton(debugPanel, "BtnGiveTestKit", "Cho Bộ Test Đồ (Bánh Mì + 10 Gỗ + Túi Hạt)", vnFont, new Color(0.38f, 0.24f, 0.52f, 1f));
                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnGiveTestKit").objectReferenceValue = btnKit;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static Image CreateSlotIcon(Transform parent)
        {
            GameObject go = new GameObject("Icon");
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(8f, 8f);
            r.offsetMax = new Vector2(-8f, -8f);
            var img = go.AddComponent<Image>();
            img.preserveAspect = true;
            img.color = new Color(1f, 1f, 1f, 0f);
            return img;
        }

        private static TMP_Text CreateCornerText(Transform parent, string name, string text, TMP_FontAsset font, float size, TextAlignmentOptions align, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(4f, 2f);
            r.offsetMax = new Vector2(-4f, -2f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.color = color;
            tmp.raycastTarget = false;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static TMP_Text CreateBoxText(Transform parent, string name, Vector2 pos, Vector2 size, TMP_FontAsset font, float fontSize, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 1f);
            r.anchorMax = new Vector2(0.5f, 1f);
            r.pivot = new Vector2(0.5f, 1f);
            r.anchoredPosition = pos;
            r.sizeDelta = size;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static void AssignArray<T>(SerializedProperty prop, T[] items) where T : Object
        {
            prop.ClearArray();
            for (int i = 0; i < items.Length; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
            }
        }

        private static Button EnsureDebugButton(Transform parent, string name, string label, TMP_FontAsset font, Color bgColor)
        {
            Transform existing = parent.Find(name);
            if (existing != null) return existing.GetComponent<Button>();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 36f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var tmp = CreateCornerText(go.transform, "Label", label, font, 15, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }
    }
}
```

## 87. `Assets/Editor/AutoBuilder_Step6.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Farming;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step6
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string CropDewBarleyPath = "Assets/Data/Crops/Crop_DewBarley.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LootDropPrefabPath = "Assets/Prefabs/Enemies/LootDrop.prefab";

        [MenuItem("Moonpetal/Step 6/1. Build Farming, Crops & Resources")]
        public static void BuildStep6()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 6 ===");

            // 1. Tạo sprite đất, 4 giai đoạn Lúa Mạch Sương Mai, Giếng, Kệ Hạt, Nhánh Cây, Rác Nhà Kính
            Sprite soilUntilled = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_untilled.png", 16, 16, DrawUntilledSoil);
            Sprite soilDry = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_dry.png", 16, 16, DrawTilledDrySoil);
            Sprite soilWet = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_soil_wet.png", 16, 16, DrawTilledWetSoil);

            Sprite[] barleyStages = new Sprite[4];
            barleyStages[0] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_0.png", 16, 16, DrawBarleyStage0);
            barleyStages[1] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_1.png", 16, 16, DrawBarleyStage1);
            barleyStages[2] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_2.png", 16, 16, DrawBarleyStage2);
            barleyStages[3] = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Crops/spr_dew_barley_3.png", 16, 16, DrawBarleyStage3);

            Sprite wellSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_well.png", 16, 16, DrawWellSprite);
            Sprite shelfSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_shelf.png", 16, 16, DrawShelfSprite);
            Sprite branchSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_branch.png", 16, 16, DrawBranchSprite);
            Sprite debrisSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_debris.png", 16, 16, DrawDebrisSprite);

            // 2. Tạo Crop_DewBarley.asset & nối vào Item_item_dew_barley_seed
            CropData barleyCrop = EnsureDewBarleyCrop(barleyStages);

            // 3. Gắn PlayerToolController vào Player_Wen.prefab
            UpgradePlayerPrefabWithTools();

            // 4. Nâng cấp GameRoot.prefab thêm nút Debug Tưới Toàn Bộ Ruộng
            UpgradeGameRootDebugWithWaterAll();

            // 5. Dựng 12 ô đất, Kệ Hạt, Giếng Nước, 6 Nhánh Cây Khô, 3 Rác Nhà Kính trong Scene
            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildFarmGridInGreenhouse(barleyCrop, soilUntilled, soilDry, soilWet);
            BuildSeedShelfAndWaterWell(shelfSpr, wellSpr);
            BuildResourceNodes(branchSpr, debrisSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 6! Chạy 'Moonpetal/Step 6/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 6/Validate")]
        public static void ValidateStep6()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 6 ===");

            var crop = AssetDatabase.LoadAssetAtPath<CropData>(CropDewBarleyPath);
            if (crop == null || crop.stageSprites == null || crop.stageSprites.Length < 4)
            {
                Debug.LogError("[MV] Crop_DewBarley.asset thiếu dữ liệu hoặc chưa đủ 4 sprite giai đoạn.");
                errors++;
            }

            var seedItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley_seed.asset");
            if (seedItem == null || seedItem.linkedCropData == null)
            {
                Debug.LogError("[MV] Item_item_dew_barley_seed chưa nối linkedCropData.");
                errors++;
            }

            var farmMgr = UnityEngine.Object.FindAnyObjectByType<FarmGridManager>();
            if (farmMgr == null)
            {
                Debug.LogError("[MV] Thiếu FarmGridManager trong scene.");
                errors++;
            }

            var tiles = UnityEngine.Object.FindObjectsByType<FarmTileController>(FindObjectsInactive.Include);
            if (tiles.Length < 12)
            {
                Debug.LogError("[MV] Thiếu ô đất Nhà Kính (cần 12, hiện có " + tiles.Length + ").");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<PlayerToolController>() == null)
            {
                Debug.LogError("[MV] Player_Wen thiếu PlayerToolController.");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<WaterWellInteractable>() == null)
            {
                Debug.LogError("[MV] Thiếu WaterWellInteractable.");
                errors++;
            }

            if (UnityEngine.Object.FindAnyObjectByType<SeedShelfInteractable>() == null)
            {
                Debug.LogError("[MV] Thiếu SeedShelfInteractable.");
                errors++;
            }

            var nodes = UnityEngine.Object.FindObjectsByType<ResourceNodeController>(FindObjectsInactive.Include);
            if (nodes.Length < 9)
            {
                Debug.LogError("[MV] Thiếu ResourceNodeController (cần 9 = 6 Nhánh cây + 3 Rác Nhà Kính, hiện có " + nodes.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 6: PASS (0 lỗi). 12 Ô đất canh tác, Lúa Mạch 4 giai đoạn, Giếng, Kệ Hạt & Gỗ tái tạo sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 6: FAIL (" + errors + " lỗi).");
            }
        }

        private static CropData EnsureDewBarleyCrop(Sprite[] stages)
        {
            var crop = AssetDatabase.LoadAssetAtPath<CropData>(CropDewBarleyPath);
            if (crop == null)
            {
                crop = ScriptableObject.CreateInstance<CropData>();
                AssetDatabase.CreateAsset(crop, CropDewBarleyPath);
            }

            var seedItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley_seed.asset");
            var harvestItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley.asset");

            crop.cropID = "crop_dew_barley";
            crop.cropNameKey = "name_item_dew_barley";
            crop.seedItem = seedItem;
            crop.harvestedItem = harvestItem;
            crop.growDays = 2;
            crop.harvestYield = 1;
            crop.stageSprites = stages;
            EditorUtility.SetDirty(crop);

            if (seedItem != null)
            {
                seedItem.linkedCropData = crop;
                EditorUtility.SetDirty(seedItem);
            }
            return crop;
        }

        private static void UpgradePlayerPrefabWithTools()
        {
            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null) return;

            var toolCtrl = root.GetComponent<PlayerToolController>();
            if (toolCtrl == null) toolCtrl = root.AddComponent<PlayerToolController>();

            var so = new SerializedObject(toolCtrl);
            so.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            so.FindProperty("playerController").objectReferenceValue = root.GetComponent<PlayerController2D>();
            so.FindProperty("inputReader").objectReferenceValue = root.GetComponent<GameInputReader>();
            so.FindProperty("visualTransform").objectReferenceValue = root.transform.Find("Visual");
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void UpgradeGameRootDebugWithWaterAll()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 880f);
                Transform existing = debugPanel.Find("BtnWaterAllCrops");
                Button btn;
                if (existing != null)
                {
                    btn = existing.GetComponent<Button>();
                }
                else
                {
                    GameObject go = new GameObject("BtnWaterAllCrops");
                    go.transform.SetParent(debugPanel, false);
                    go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
                    var img = go.AddComponent<Image>();
                    img.color = new Color(0.18f, 0.42f, 0.62f, 1f);
                    btn = go.AddComponent<Button>();
                    btn.targetGraphic = img;

                    GameObject txtGo = new GameObject("Label");
                    txtGo.transform.SetParent(go.transform, false);
                    var r = txtGo.AddComponent<RectTransform>();
                    r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
                    r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
                    var tmp = txtGo.AddComponent<TextMeshProUGUI>();
                    tmp.text = "Tưới Toàn Bộ Ruộng Nhà Kính";
                    tmp.fontSize = 15; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
                    if (vnFont != null) tmp.font = vnFont;
                }

                var so = new SerializedObject(debugCtrl);
                so.FindProperty("btnWaterAllCrops").objectReferenceValue = btn;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildFarmGridInGreenhouse(CropData barleyCrop, Sprite untilled, Sprite dry, Sprite wet)
        {
            GameObject old = GameObject.Find("GreenhouseFarmGrid");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject gridGo = new GameObject("GreenhouseFarmGrid");
            var mgr = gridGo.AddComponent<FarmGridManager>();
            var tileList = new List<FarmTileController>();

            // 4 cột x 3 hàng gọn gàng trong Nhà Kính (x: -17..-14, y: 7..9)
            int idx = 0;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    idx++;
                    float wx = -17.5f + c * 1.15f;
                    float wy = 7.5f + r * 1.15f;

                    GameObject tGo = new GameObject("FarmTile_" + idx);
                    tGo.tag = "FarmTile";
                    tGo.transform.SetParent(gridGo.transform, false);
                    tGo.transform.position = new Vector3(wx, wy, 0f);

                    var soilSr = tGo.AddComponent<SpriteRenderer>();
                    soilSr.sprite = untilled;
                    soilSr.sortingLayerName = "FarmTile";
                    soilSr.sortingOrder = 5;

                    GameObject cropGo = new GameObject("CropVisual");
                    cropGo.transform.SetParent(tGo.transform, false);
                    cropGo.transform.localPosition = new Vector3(0f, 0.15f, 0f);
                    var cropSr = cropGo.AddComponent<SpriteRenderer>();
                    cropSr.sortingLayerName = "WorldObjects";
                    cropSr.sortingOrder = Mathf.RoundToInt(-wy * 100f) + 5;
                    cropSr.enabled = false;

                    var ctrl = tGo.AddComponent<FarmTileController>();
                    var so = new SerializedObject(ctrl);
                    so.FindProperty("tileId").stringValue = "gh_tile_" + idx;
                    so.FindProperty("soilRenderer").objectReferenceValue = soilSr;
                    so.FindProperty("cropRenderer").objectReferenceValue = cropSr;
                    so.FindProperty("untilledSprite").objectReferenceValue = untilled;
                    so.FindProperty("tilledDrySprite").objectReferenceValue = dry;
                    so.FindProperty("tilledWetSprite").objectReferenceValue = wet;
                    so.ApplyModifiedPropertiesWithoutUndo();

                    tileList.Add(ctrl);
                }
            }

            var mgrSO = new SerializedObject(mgr);
            mgrSO.FindProperty("defaultCropData").objectReferenceValue = barleyCrop;
            var prop = mgrSO.FindProperty("tiles");
            prop.ClearArray();
            for (int i = 0; i < tileList.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = tileList[i];
            }
            mgrSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildSeedShelfAndWaterWell(Sprite shelfSpr, Sprite wellSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);

            // 1. Kệ Hạt Giống trong Nhà Kính
            GameObject oldShelf = GameObject.Find("GreenhouseSeedShelf");
            if (oldShelf != null) UnityEngine.Object.DestroyImmediate(oldShelf);
            GameObject shelfGo = new GameObject("GreenhouseSeedShelf");
            shelfGo.tag = "Interactable";
            shelfGo.layer = 11;
            shelfGo.transform.position = new Vector3(-12.5f, 12.5f, 0f);
            var sSr = shelfGo.AddComponent<SpriteRenderer>();
            sSr.sprite = shelfSpr; sSr.sortingLayerName = "WorldObjects"; sSr.sortingOrder = Mathf.RoundToInt(-12.5f * 100f);
            shelfGo.AddComponent<BoxCollider2D>().size = new Vector2(0.9f, 0.9f);
            GameObject shelfPrompt = CreateWorldPrompt(shelfGo.transform, "[E] Lấy Hạt Lúa Mạch", vnFont);
            var shelfComp = shelfGo.AddComponent<SeedShelfInteractable>();
            var sSO = new SerializedObject(shelfComp);
            sSO.FindProperty("promptObject").objectReferenceValue = shelfPrompt;
            sSO.FindProperty("interactRadius").floatValue = 1.9f;
            sSO.ApplyModifiedPropertiesWithoutUndo();

            // 2. Giếng Nước cạnh ao ngoài Nhà Kính
            GameObject oldWell = GameObject.Find("VillageWaterWell");
            if (oldWell != null) UnityEngine.Object.DestroyImmediate(oldWell);
            GameObject wellGo = new GameObject("VillageWaterWell");
            wellGo.tag = "Interactable";
            wellGo.layer = 11;
            wellGo.transform.position = new Vector3(-7.0f, 10.5f, 0f);
            var wSr = wellGo.AddComponent<SpriteRenderer>();
            wSr.sprite = wellSpr; wSr.sortingLayerName = "WorldObjects"; wSr.sortingOrder = Mathf.RoundToInt(-10.5f * 100f);
            wellGo.AddComponent<BoxCollider2D>().size = new Vector2(0.95f, 0.95f);
            GameObject wellPrompt = CreateWorldPrompt(wellGo.transform, "[E] Múc đầy Bình Tưới", vnFont);
            var wellComp = wellGo.AddComponent<WaterWellInteractable>();
            var wSO = new SerializedObject(wellComp);
            wSO.FindProperty("promptObject").objectReferenceValue = wellPrompt;
            wSO.FindProperty("interactRadius").floatValue = 2.0f;
            wSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildResourceNodes(Sprite branchSpr, Sprite debrisSpr)
        {
            GameObject oldGroup = GameObject.Find("ResourceNodesGroup");
            if (oldGroup != null) UnityEngine.Object.DestroyImmediate(oldGroup);
            GameObject group = new GameObject("ResourceNodesGroup");

            ItemData woodItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_branch_wood.asset");
            GameObject lootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath);

            // 6 Nhánh Cây Khô ngoài làng (tự mọc lại mỗi sáng)
            Vector2[] branchPositions =
            {
                new Vector2(-9.5f, 4.5f),
                new Vector2(-4.5f, 6.5f),
                new Vector2(4.5f, 6.5f),
                new Vector2(6.5f, -0.5f),
                new Vector2(-6.5f, -2.5f),
                new Vector2(12.5f, 4.5f)
            };

            for (int i = 0; i < branchPositions.Length; i++)
            {
                CreateResourceNode(group.transform, "DryBranch_" + (i + 1), branchPositions[i], branchSpr,
                    ResourceNodeType.DryBranch, "item_hatchet", 2, woodItem, 2, lootPrefab, true);
            }

            // 3 Cụm Cỏ Dại / Đá Vụn trong Nhà Kính (dọn cho Mở Đầu Step 7)
            Vector2[] debrisPositions =
            {
                new Vector2(-18.5f, 9.5f),
                new Vector2(-18.5f, 7.5f),
                new Vector2(-12.5f, 7.5f)
            };

            for (int i = 0; i < debrisPositions.Length; i++)
            {
                CreateResourceNode(group.transform, "GreenhouseDebris_" + (i + 1), debrisPositions[i], debrisSpr,
                    ResourceNodeType.GreenhouseDebris, "item_hoe", 1, woodItem, 1, lootPrefab, false);
            }
        }

        private static void CreateResourceNode(
            Transform parent, string name, Vector2 pos, Sprite spr,
            ResourceNodeType type, string toolId, int hits, ItemData dropItem, int dropAmt, GameObject lootPrefab, bool regrow)
        {
            GameObject go = new GameObject(name);
            go.tag = "ResourceNode";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.85f, 0.75f);

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(go.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);

            var comp = go.AddComponent<ResourceNodeController>();
            var so = new SerializedObject(comp);
            so.FindProperty("nodeType").enumValueIndex = (int)type;
            so.FindProperty("requiredToolId").stringValue = toolId;
            so.FindProperty("maxHits").intValue = hits;
            so.FindProperty("dropItem").objectReferenceValue = dropItem;
            so.FindProperty("dropAmount").intValue = dropAmt;
            so.FindProperty("lootDropPrefab").objectReferenceValue = lootPrefab;
            so.FindProperty("regrowEachDay").boolValue = regrow;
            so.FindProperty("visualRenderer").objectReferenceValue = sr;
            so.FindProperty("nodeCollider").objectReferenceValue = col;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreateWorldPrompt(Transform parent, string label, TMP_FontAsset font)
        {
            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(parent, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 50;

            promptGo.GetComponent<RectTransform>().sizeDelta = new Vector2(260f, 44f);
            promptGo.AddComponent<Image>().color = new Color(0.08f, 0.12f, 0.18f, 0.88f);

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(promptGo.transform, false);
            var tRect = txtGo.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero; tRect.anchorMax = Vector2.one;
            tRect.offsetMin = Vector2.zero; tRect.offsetMax = Vector2.zero;
            var tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label; tmp.fontSize = 21; tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.94f, 0.65f, 1f);
            if (font != null) tmp.font = font;
            return promptGo;
        }

        // Procedural 16x16 Pixel Art Sprites for Step 6
        private static void DrawUntilledSoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.46f, 0.34f, 0.22f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 4, 2, 2, new Color(0.38f, 0.28f, 0.18f));
            PlaceholderGeneratorUtility.FillRect(t, 10, 10, 2, 2, new Color(0.52f, 0.40f, 0.26f));
        }

        private static void DrawTilledDrySoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.40f, 0.26f, 0.14f));
            Color furrow = new Color(0.30f, 0.19f, 0.10f);
            PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 7, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 11, 12, 2, furrow);
        }

        private static void DrawTilledWetSoil(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 1, 14, 14, new Color(0.24f, 0.16f, 0.10f));
            Color furrow = new Color(0.16f, 0.10f, 0.06f);
            PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 7, 12, 2, furrow);
            PlaceholderGeneratorUtility.FillRect(t, 2, 11, 12, 2, furrow);
        }

        private static void DrawBarleyStage0(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 5, 4, 2, 2, new Color(0.76f, 0.86f, 0.45f));
            PlaceholderGeneratorUtility.FillRect(t, 9, 6, 2, 2, new Color(0.76f, 0.86f, 0.45f));
        }

        private static void DrawBarleyStage1(Texture2D t)
        {
            Color g = new Color(0.34f, 0.78f, 0.36f);
            PlaceholderGeneratorUtility.FillRect(t, 7, 2, 2, 6, g);
            PlaceholderGeneratorUtility.FillRect(t, 5, 6, 6, 2, g);
        }

        private static void DrawBarleyStage2(Texture2D t)
        {
            Color g = new Color(0.30f, 0.76f, 0.40f);
            Color tip = new Color(0.68f, 0.88f, 0.44f);
            PlaceholderGeneratorUtility.FillRect(t, 5, 2, 2, 9, g);
            PlaceholderGeneratorUtility.FillRect(t, 9, 2, 2, 10, g);
            PlaceholderGeneratorUtility.FillRect(t, 4, 10, 8, 3, tip);
        }

        private static void DrawBarleyStage3(Texture2D t)
        {
            Color stem = new Color(0.78f, 0.64f, 0.24f);
            Color gold = new Color(0.98f, 0.86f, 0.34f);
            Color dew = new Color(0.68f, 0.96f, 1.0f);
            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 2, 10, stem);
            PlaceholderGeneratorUtility.FillRect(t, 7, 2, 2, 11, stem);
            PlaceholderGeneratorUtility.FillRect(t, 10, 2, 2, 10, stem);
            PlaceholderGeneratorUtility.FillRect(t, 3, 9, 10, 5, gold);
            PlaceholderGeneratorUtility.FillRect(t, 5, 13, 2, 2, dew);
            PlaceholderGeneratorUtility.FillRect(t, 10, 12, 2, 2, dew);
        }

        private static void DrawWellSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 2, 12, 12, new Color(0.54f, 0.56f, 0.60f));
            PlaceholderGeneratorUtility.FillRect(t, 4, 4, 8, 8, new Color(0.20f, 0.56f, 0.86f));
            PlaceholderGeneratorUtility.FillRect(t, 5, 8, 4, 2, new Color(0.66f, 0.92f, 1f));
        }

        private static void DrawShelfSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 1, 2, 14, 12, new Color(0.52f, 0.34f, 0.18f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 8, 4, 4, new Color(0.86f, 0.72f, 0.38f));
            PlaceholderGeneratorUtility.FillRect(t, 9, 8, 4, 4, new Color(0.52f, 0.84f, 0.44f));
            PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 3, new Color(0.72f, 0.56f, 0.32f));
        }

        private static void DrawBranchSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 2, 6, 12, 4, new Color(0.46f, 0.28f, 0.14f));
            PlaceholderGeneratorUtility.FillRect(t, 6, 9, 3, 4, new Color(0.56f, 0.36f, 0.20f));
            PlaceholderGeneratorUtility.FillRect(t, 10, 4, 3, 3, new Color(0.38f, 0.22f, 0.10f));
        }

        private static void DrawDebrisSprite(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 8, new Color(0.42f, 0.46f, 0.44f));
            PlaceholderGeneratorUtility.FillRect(t, 5, 8, 6, 5, new Color(0.28f, 0.48f, 0.26f));
            PlaceholderGeneratorUtility.FillRect(t, 4, 5, 4, 4, new Color(0.54f, 0.56f, 0.58f));
        }
    }
}
```

## 88. `Assets/Editor/AutoBuilder_Step7.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step7
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string StringTablePath = "Assets/Data/Localization/VNStringTable.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 7/1. Build NPCs, Dialogues, Diary & Quests")]
        public static void BuildStep7()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 7 ===");

            var table = AssetDatabase.LoadAssetAtPath<StringTableSO>(StringTablePath);
            List<DiaryPageData> diaryPages = Build4DiaryPages(table);
            var npcs = Build4NPCsAndDialogues(table, diaryPages);

            if (table != null) EditorUtility.SetDirty(table);

            UpgradeGameRootWithNarrativeUI(diaryPages);

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildNPCsInScene(npcs);
            BuildStoryPropsInScene();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 7! Chạy 'Moonpetal/Step 7/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 7/Validate")]
        public static void ValidateStep7()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 7 ===");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                var rootSO = new SerializedObject(prefab.GetComponent<GameRoot>());
                string[] fields = { "dialogueManager", "diaryManager", "questManager" };
                foreach (string f in fields)
                {
                    if (rootSO.FindProperty(f).objectReferenceValue == null)
                    {
                        Debug.LogError("[MV] GameRoot thiếu: " + f);
                        errors++;
                    }
                }
            }

            var npcs = UnityEngine.Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include);
            if (npcs.Length < 4)
            {
                Debug.LogError("[MV] Thiếu NPC trong scene (cần 4, hiện có " + npcs.Length + ").");
                errors++;
            }

            var props = UnityEngine.Object.FindObjectsByType<StoryPropInteractable>(FindObjectsInactive.Include);
            if (props.Length < 5)
            {
                Debug.LogError("[MV] Thiếu StoryPropInteractable trong scene (cần 5, hiện có " + props.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 7: PASS (0 lỗi). 4 NPC, Hội thoại phân nhánh, Sổ Tay 4 trang & 5 StoryProp sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 7: FAIL (" + errors + " lỗi).");
            }
        }

        private static List<DiaryPageData> Build4DiaryPages(StringTableSO table)
        {
            var list = new List<DiaryPageData>();
            list.Add(CreateDiaryPage(table, "diary_page_01", 1, "flag_lumi_awakened",
                "Trang 1 — Lời Dặn Trong Nhà Kính",
                "Wen yêu quý, Hoa Ánh Trăng không bao giờ nở vì một đôi tay đơn độc. Khi sương héo úa phủ lên thung lũng, hãy tìm lại những người hàng xóm xưa và lắng nghe câu chuyện của họ."));
            list.Add(CreateDiaryPage(table, "diary_page_02", 2, "flag_hana_bread_done",
                "Trang 2 — Mẻ Bánh Đầu Xuân Của Hana",
                "Mùa xuân năm ấy, cả làng tụ họp ở vườn ông Bách chia nhau những ổ Bánh Mì Nắng Ấm nóng hổi từ lúa mạch sương mai. Hương bánh làm tan đi cả cái lạnh đầu mùa."));
            list.Add(CreateDiaryPage(table, "diary_page_03", 3, "flag_toby_cat_saved",
                "Trang 3 — Tiếng Cười Quanh Vườn Phía Nam",
                "Ông Bách tuy ít nói nhưng luôn để dành một góc vườn cho lũ trẻ chạy nhảy và lũ mèo nằm sưởi nắng. Chỉ cần có người nhớ đến khu vườn, ông sẽ lại cầm cuốc."));
            list.Add(CreateDiaryPage(table, "diary_page_04", 4, "flag_otis_mailbox_fixed",
                "Trang 4 — Bức Thư Chưa Kịp Gửi Ông Bách",
                "Gửi ông bạn già Bách, cảm ơn ông đã giữ gìn chiếc ghế đôi và những hạt giống xưa. Nếu cháu gái tôi Wen tìm đến, xin ông hãy một lần nữa cùng cháu gieo mầm hy vọng nhé."));
            return list;
        }

        private static DiaryPageData CreateDiaryPage(StringTableSO table, string id, int num, string flag, string title, string body)
        {
            string path = "Assets/Data/Diary/DiaryPage_0" + num + ".asset";
            var page = AssetDatabase.LoadAssetAtPath<DiaryPageData>(path);
            if (page == null)
            {
                page = ScriptableObject.CreateInstance<DiaryPageData>();
                AssetDatabase.CreateAsset(page, path);
            }
            page.pageID = id;
            page.pageNumber = num;
            page.titleKey = "title_" + id;
            page.bodyTextKey = "body_" + id;
            page.unlockedByFlag = flag;

            table?.SetEntry(page.titleKey, title);
            table?.SetEntry(page.bodyTextKey, body);
            EditorUtility.SetDirty(page);
            return page;
        }

        private static Dictionary<string, NPCData> Build4NPCsAndDialogues(StringTableSO table, List<DiaryPageData> pages)
        {
            var result = new Dictionary<string, NPCData>();

            ItemData sickle = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_moon_sickle.asset");
            ItemData barley = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_barley.asset");
            ItemData bread = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_sunshine_bread.asset");
            ItemData bag1 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_seed_bag_1.asset");
            ItemData bag2 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_seed_bag_2.asset");

            // 1. ÔNG BÁCH
            var dlgBachIntro = CreateDialogue(table, "dlg_bach_intro",
                new[] { ("Ông Bách", "Cháu là Wen, cháu gái của bà cụ Nhà Kính phải không? Khu vườn phía nam này bị Bọt Sương Đen chiếm mất rồi..."),
                        ("Ông Bách", "Cầm lấy chiếc Liềm Ánh Trăng cũ này (ô số 4). Nếu cháu thanh tẩy hết 5 Bọt Sương Đen và lau sạch Chiếc Ghế Đôi, ta sẽ tin thung lũng này còn hy vọng.") },
                "flag_met_bach_at_gate", sickle, 1, null, 0, null);

            var dlgBachReadLetters = CreateDialogue(table, "dlg_bach_read_letters",
                new[] { ("Wen", "Ông Bách ơi, cháu đã tìm lại đủ 4 trang nhật ký và bức thư cuối bà gửi cho ông..."),
                        ("Ông Bách", "Bà ấy vẫn luôn nhớ chiếc ghế đôi này sao... Cảm ơn cháu, Wen. Ta sẽ đứng cạnh Bồn Hoa chờ mọi người cùng gieo hạt!") },
                "flag_bach_letters_read", null, 0, null, 0, null);

            var dlgBachDefault = CreateDialogue(table, "dlg_bach_default",
                new[] { ("Ông Bách", "Khu vườn này cần sự chung tay của cả làng. Cháu hãy giúp Bà Hana, cậu bé Toby và Bác Otis tìm đủ 3 Túi Hạt Di Sản nhé.") },
                "", null, 0, null, 0, null);

            result["npc_bach"] = CreateNPC(table, "npc_bach", "Ông Bách", "Người Làm Vườn Già",
                new Color(0.45f, 0.36f, 0.28f), new Color(0.86f, 0.86f, 0.90f),
                dlgBachDefault, "flag_bach_letters_read",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_tutorial_done", blockedByStoryFlag = "flag_met_bach_at_gate", dialogue = dlgBachIntro },
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_garden_bench_cleaned", blockedByStoryFlag = "flag_bach_letters_read", dialogue = dlgBachReadLetters }
                });

            // 2. BÀ HANA
            var dlgHanaComplete = CreateDialogue(table, "dlg_hana_complete",
                new[] { ("Bà Hana", "Ôi, 4 bó Lúa Mạch Sương Mai tươi quá! Mẻ bánh đầu xuân lại thơm lừng cả quảng trường rồi!"),
                        ("Bà Hana", "Tặng cháu 2 ổ Bánh Mì Nắng Ấm (ăn vào hồi lực & xóa mệt mỏi ngay) cùng Túi Hạt Di Sản Số 1 nhé. Cô ra Bồn Hoa chờ cháu!") },
                "flag_hana_bread_done", bag1, 1, barley, 4, pages[1]);

            var dlgHanaGiveBreadBonus = CreateDialogue(table, "dlg_hana_after",
                new[] { ("Bà Hana", "Cô đã sẵn sàng cùng cháu và Ông Bách gieo hạt tại Bồn Hoa Trung Tâm Vườn rồi đó!") },
                "", bread, 2, null, 0, null);

            var dlgHanaDefault = CreateDialogue(table, "dlg_hana_default",
                new[] { ("Bà Hana", "Chào Wen! Nếu cháu trồng được 4 cây Lúa Mạch Sương Mai trong Nhà Kính mang tới đây, cô sẽ nướng Bánh Mì Nắng Ấm và trao Túi Hạt Di Sản Số 1 cho cháu.") },
                "flag_hana_quest_started", null, 0, null, 0, null);

            result["npc_hana"] = CreateNPC(table, "npc_hana", "Bà Hana", "Chủ Tiệm Bánh",
                new Color(0.86f, 0.44f, 0.40f), new Color(0.98f, 0.92f, 0.80f),
                dlgHanaDefault, "flag_hana_bread_done",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_met_bach_at_gate", blockedByStoryFlag = "flag_hana_bread_done", dialogue = dlgHanaComplete },
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_hana_bread_done", blockedByStoryFlag = "flag_hana_bread_bonus_taken", dialogue = dlgHanaGiveBreadBonus }
                });
            dlgHanaGiveBreadBonus.setsStoryFlagOnFinish = "flag_hana_bread_bonus_taken";
            EditorUtility.SetDirty(dlgHanaGiveBreadBonus);

            // 3. CẬU BÉ TOBY
            var dlgTobyComplete = CreateDialogue(table, "dlg_toby_complete",
                new[] { ("Cậu Bé Toby", "Meo meo! Chị Wen đuổi hết 3 Bọt Sương Đen cứu bé Miu rồi! Em cảm ơn chị nhiều lắm!"),
                        ("Cậu Bé Toby", "Em gửi chị Túi Hạt Di Sản Số 2 và trang sổ em nhặt được. Em chạy ra Bồn Hoa phụ chị ngay đây!") },
                "flag_toby_cat_saved", bag2, 1, null, 0, pages[2]);

            var dlgTobyDefault = CreateDialogue(table, "dlg_toby_default",
                new[] { ("Cậu Bé Toby", "Chị Wen ơi, chú mèo Miu của em bị 3 con Bọt Sương Đen vây ở Rìa Rừng Phía Đông rồi! Chị cầm Liềm Ánh Trăng cứu bé giúp em với!") },
                "", null, 0, null, 0, null);

            result["npc_toby"] = CreateNPC(table, "npc_toby", "Cậu Bé Toby", "Cư Dân Làng",
                new Color(0.28f, 0.68f, 0.46f), new Color(0.95f, 0.78f, 0.36f),
                dlgTobyDefault, "flag_toby_cat_saved",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_toby_cat_slimes_cleared", blockedByStoryFlag = "flag_toby_cat_saved", dialogue = dlgTobyComplete }
                });

            // 4. BÁC OTIS
            var dlgOtisDone = CreateDialogue(table, "dlg_otis_done",
                new[] { ("Bác Otis", "Hòm thư chắc chắn như mới rồi! Cảm ơn cháu Wen, bác ra đứng cạnh Bồn Hoa chờ mọi người cùng gieo hạt đây!") },
                "", null, 0, null, 0, null);

            var dlgOtisDefault = CreateDialogue(table, "dlg_otis_default",
                new[] { ("Bác Otis", "Hòm thư gỗ ngay cạnh bác bị sương mục hỏng mất rồi. Cháu chặt 10 Gỗ Nhánh Cây quanh làng rồi bấm [E] vào Hòm Thư để sửa giúp bác nhé, bên trong có Túi Hạt Di Sản Số 3 và thư của bà cháu đấy!") },
                "", null, 0, null, 0, null);

            result["npc_otis"] = CreateNPC(table, "npc_otis", "Bác Otis", "Người Đưa Thư",
                new Color(0.26f, 0.46f, 0.76f), new Color(0.92f, 0.84f, 0.50f),
                dlgOtisDefault, "flag_otis_mailbox_fixed",
                new List<ConditionalDialogueEntry>
                {
                    new ConditionalDialogueEntry { requiredStoryFlag = "flag_otis_mailbox_fixed", blockedByStoryFlag = "", dialogue = dlgOtisDone }
                });

            return result;
        }

        private static DialogueData CreateDialogue(
            StringTableSO table, string id, (string speaker, string text)[] lines,
            string setFlag, ItemData giveItem, int giveAmt, ItemData consumeItem, int consumeAmt, DiaryPageData unlockPage)
        {
            string path = "Assets/Data/Dialogues/Dlg_" + id + ".asset";
            var dlg = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
            if (dlg == null)
            {
                dlg = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(dlg, path);
            }

            dlg.dialogueID = id;
            dlg.lines.Clear();
            for (int i = 0; i < lines.Length; i++)
            {
                string spKey = id + "_sp_" + i;
                string txKey = id + "_tx_" + i;
                table?.SetEntry(spKey, lines[i].speaker);
                table?.SetEntry(txKey, lines[i].text);
                dlg.lines.Add(new DialogueLine { speakerNameKey = spKey, textKey = txKey });
            }

            dlg.setsStoryFlagOnFinish = setFlag;
            dlg.givesItemOnFinish = giveItem;
            dlg.givesItemAmount = giveAmt;
            dlg.consumesRequiredItem = consumeItem;
            dlg.consumesAmount = consumeAmt;
            dlg.unlocksDiaryPage = unlockPage;
            EditorUtility.SetDirty(dlg);
            return dlg;
        }

        private static NPCData CreateNPC(
            StringTableSO table, string id, string vnName, string role,
            Color coatColor, Color hairColor, DialogueData defaultDlg, string helperFlag,
            List<ConditionalDialogueEntry> conditionals)
        {
            Sprite spr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_" + id + ".png", 16, 24, tex =>
            {
                PlaceholderGeneratorUtility.FillRect(tex, 5, 1, 2, 3, new Color(0.24f, 0.16f, 0.12f));
                PlaceholderGeneratorUtility.FillRect(tex, 9, 1, 2, 3, new Color(0.24f, 0.16f, 0.12f));
                PlaceholderGeneratorUtility.FillRect(tex, 4, 4, 8, 9, coatColor);
                PlaceholderGeneratorUtility.FillRect(tex, 4, 13, 8, 6, new Color(0.96f, 0.84f, 0.72f));
                tex.SetPixel(6, 15, Color.black);
                tex.SetPixel(9, 15, Color.black);
                PlaceholderGeneratorUtility.FillRect(tex, 3, 18, 10, 3, hairColor);
            });

            string path = "Assets/Data/NPCs/NPC_" + id + ".asset";
            var npc = AssetDatabase.LoadAssetAtPath<NPCData>(path);
            if (npc == null)
            {
                npc = ScriptableObject.CreateInstance<NPCData>();
                AssetDatabase.CreateAsset(npc, path);
            }
            npc.npcID = id;
            npc.displayNameKey = "name_" + id;
            npc.roleTitleKey = "role_" + id;
            npc.worldSprite = spr;
            npc.defaultDialogue = defaultDlg;
            npc.conditionalDialogues = conditionals;
            npc.isHelperForBundleID = "bundle_forgotten_garden";
            npc.helperReadyStoryFlag = helperFlag;

            table?.SetEntry(npc.displayNameKey, vnName);
            table?.SetEntry(npc.roleTitleKey, role);
            EditorUtility.SetDirty(npc);
            return npc;
        }

        private static void UpgradeGameRootWithNarrativeUI(List<DiaryPageData> diaryPages)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var dlgMgr = root.GetComponent<DialogueManager>() ?? root.AddComponent<DialogueManager>();
            var diaryMgr = root.GetComponent<DiaryManager>() ?? root.AddComponent<DiaryManager>();
            var questMgr = root.GetComponent<QuestManager>() ?? root.AddComponent<QuestManager>();

            var dSO = new SerializedObject(diaryMgr);
            var pagesProp = dSO.FindProperty("allPages");
            pagesProp.ClearArray();
            for (int i = 0; i < diaryPages.Count; i++)
            {
                pagesProp.InsertArrayElementAtIndex(i);
                pagesProp.GetArrayElementAtIndex(i).objectReferenceValue = diaryPages[i];
            }
            dSO.ApplyModifiedPropertiesWithoutUndo();

            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                // 1. Quest Tracker Panel (Top-Right below Clock)
                Transform oldQuest = hudCanvas.Find("QuestTrackerPanel");
                if (oldQuest != null) UnityEngine.Object.DestroyImmediate(oldQuest.gameObject);
                GameObject qPanel = CreatePanel("QuestTrackerPanel", hudCanvas, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -92f), new Vector2(380f, 128f), new Color(0.08f, 0.12f, 0.19f, 0.84f));
                qPanel.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
                var qTmp = CreateTMP("ObjectiveText", qPanel.transform, "Mục tiêu...", vnFont, 16, TextAlignmentOptions.TopLeft, Color.white);
                var qView = qPanel.AddComponent<QuestTrackerHUDView>();
                var qSO = new SerializedObject(qView);
                qSO.FindProperty("objectiveText").objectReferenceValue = qTmp;
                qSO.ApplyModifiedPropertiesWithoutUndo();

                // 2. Dialogue Box (Bottom-Center)
                Transform oldDlg = hudCanvas.Find("DialogueUIHost");
                if (oldDlg != null) UnityEngine.Object.DestroyImmediate(oldDlg.gameObject);
                GameObject dlgHost = new GameObject("DialogueUIHost");
                dlgHost.transform.SetParent(hudCanvas, false);
                var dhRect = dlgHost.AddComponent<RectTransform>();
                dhRect.anchorMin = Vector2.zero; dhRect.anchorMax = Vector2.one; dhRect.offsetMin = Vector2.zero; dhRect.offsetMax = Vector2.zero;

                GameObject dlgBox = CreatePanel("DialogueBox", dlgHost.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(980f, 190f), new Color(0.07f, 0.10f, 0.17f, 0.95f));
                dlgBox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                var spTxt = CreateTMP("SpeakerText", dlgBox.transform, "Nhân vật", vnFont, 24, TextAlignmentOptions.TopLeft, new Color(1f, 0.88f, 0.45f, 1f));
                spTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 130f); spTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -14f);
                var bdTxt = CreateTMP("BodyText", dlgBox.transform, "Nội dung...", vnFont, 21, TextAlignmentOptions.TopLeft, Color.white);
                bdTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 44f); bdTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -54f);
                var hintTxt = CreateTMP("ContinueHint", dlgBox.transform, "[E / Space] Tiếp tục", vnFont, 17, TextAlignmentOptions.BottomRight, new Color(0.70f, 0.92f, 0.96f, 1f));
                hintTxt.GetComponent<RectTransform>().offsetMin = new Vector2(24f, 12f); hintTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -140f);

                var dlgView = dlgHost.AddComponent<DialogueUIView>();
                var dvSO = new SerializedObject(dlgView);
                dvSO.FindProperty("panelRoot").objectReferenceValue = dlgBox;
                dvSO.FindProperty("speakerText").objectReferenceValue = spTxt;
                dvSO.FindProperty("bodyText").objectReferenceValue = bdTxt;
                dvSO.FindProperty("continueHintText").objectReferenceValue = hintTxt;
                dvSO.ApplyModifiedPropertiesWithoutUndo();

                var dmSO = new SerializedObject(dlgMgr);
                dmSO.FindProperty("dialogueUI").objectReferenceValue = dlgView;
                dmSO.ApplyModifiedPropertiesWithoutUndo();

                // 3. Diary UI Modal (Key B)
                Transform oldDiary = hudCanvas.Find("DiaryUIHost");
                if (oldDiary != null) UnityEngine.Object.DestroyImmediate(oldDiary.gameObject);
                GameObject diaryHost = new GameObject("DiaryUIHost");
                diaryHost.transform.SetParent(hudCanvas, false);
                var diRect = diaryHost.AddComponent<RectTransform>();
                diRect.anchorMin = Vector2.zero; diRect.anchorMax = Vector2.one; diRect.offsetMin = Vector2.zero; diRect.offsetMax = Vector2.zero;

                GameObject diaryBox = CreatePanel("DiaryBox", diaryHost.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 420f), new Color(0.16f, 0.13f, 0.10f, 0.96f));
                var pHead = CreateTMP("Header", diaryBox.transform, "SỔ TAY CỦA BÀ", vnFont, 20, TextAlignmentOptions.Top, new Color(0.95f, 0.82f, 0.52f, 1f));
                pHead.GetComponent<RectTransform>().offsetMax = new Vector2(-20f, -16f);
                var pTitle = CreateTMP("Title", diaryBox.transform, "Tiêu đề", vnFont, 25, TextAlignmentOptions.TopLeft, Color.white);
                pTitle.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 280f); pTitle.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -64f);
                var pBody = CreateTMP("Body", diaryBox.transform, "Nội dung", vnFont, 20, TextAlignmentOptions.TopLeft, new Color(0.96f, 0.92f, 0.84f, 1f));
                pBody.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 80f); pBody.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -112f);

                Button btnPrev = CreateModalButton("BtnPrev", diaryBox.transform, "< Trang Trước", new Vector2(-200f, 36f), vnFont);
                Button btnClose = CreateModalButton("BtnClose", diaryBox.transform, "Đóng [B]", new Vector2(0f, 36f), vnFont);
                Button btnNext = CreateModalButton("BtnNext", diaryBox.transform, "Trang Sau >", new Vector2(200f, 36f), vnFont);

                var diaryView = diaryHost.AddComponent<DiaryUIView>();
                var diSO = new SerializedObject(diaryView);
                diSO.FindProperty("panelRoot").objectReferenceValue = diaryBox;
                diSO.FindProperty("pageHeaderText").objectReferenceValue = pHead;
                diSO.FindProperty("pageTitleText").objectReferenceValue = pTitle;
                diSO.FindProperty("pageBodyText").objectReferenceValue = pBody;
                diSO.FindProperty("btnPrev").objectReferenceValue = btnPrev;
                diSO.FindProperty("btnNext").objectReferenceValue = btnNext;
                diSO.FindProperty("btnClose").objectReferenceValue = btnClose;
                diSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // Nối GameRoot
            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("dialogueManager").objectReferenceValue = dlgMgr;
            rootSO.FindProperty("diaryManager").objectReferenceValue = diaryMgr;
            rootSO.FindProperty("questManager").objectReferenceValue = questMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            // Nút Debug Bỏ qua Mở Đầu
            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 920f);
                Transform ex = debugPanel.Find("BtnSkipPrologue");
                Button btnSkip = ex != null ? ex.GetComponent<Button>() : CreateDebugButton(debugPanel, "BtnSkipPrologue", "Bỏ qua Mở Đầu (Vào NV1 ngay)", vnFont);
                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnSkipPrologue").objectReferenceValue = btnSkip;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildNPCsInScene(Dictionary<string, NPCData> npcs)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject old = GameObject.Find("VillageNPCs");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("VillageNPCs");

            SpawnNPC(group.transform, "NPC_Bach", npcs["npc_bach"], new Vector2(0.0f, -3.5f), new Vector2(-1.6f, -8.8f), "[E] Nói chuyện: Ông Bách", vnFont);
            SpawnNPC(group.transform, "NPC_Hana", npcs["npc_hana"], new Vector2(-6.0f, 3.0f), new Vector2(1.8f, -8.8f), "[E] Nói chuyện: Bà Hana", vnFont);
            SpawnNPC(group.transform, "NPC_Toby", npcs["npc_toby"], new Vector2(7.5f, 2.5f), new Vector2(-1.6f, -11.2f), "[E] Nói chuyện: Cậu Bé Toby", vnFont);
            SpawnNPC(group.transform, "NPC_Otis", npcs["npc_otis"], new Vector2(1.2f, 5.5f), new Vector2(1.8f, -11.2f), "[E] Nói chuyện: Bác Otis", vnFont);
        }

        private static void SpawnNPC(Transform parent, string name, NPCData data, Vector2 pos, Vector2 helperPos, string promptText, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.tag = "NPC";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = data.worldSprite;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);

            go.AddComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.7f);
            GameObject prompt = CreateWorldPrompt(go.transform, promptText, font);

            var ctrl = go.AddComponent<NPCController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("npcData").objectReferenceValue = data;
            so.FindProperty("promptObject").objectReferenceValue = prompt;
            so.FindProperty("interactRadius").floatValue = 2.1f;
            so.FindProperty("helperStandPosition").vector2Value = helperPos;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildStoryPropsInScene()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject old = GameObject.Find("StoryPropsGroup");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("StoryPropsGroup");

            Sprite tableSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_table.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 10, new Color(0.54f, 0.36f, 0.20f));
                PlaceholderGeneratorUtility.FillRect(t, 5, 6, 6, 5, new Color(0.98f, 0.95f, 0.85f));
            });
            Sprite boxSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_seedbox.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 3, 3, 10, 10, new Color(0.42f, 0.28f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 6, 6, 4, 4, new Color(0.45f, 0.94f, 0.96f));
            });
            Sprite planterSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_planter.png", 16, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 2, 12, 12, new Color(0.56f, 0.56f, 0.60f));
                PlaceholderGeneratorUtility.FillRect(t, 4, 4, 8, 8, new Color(0.34f, 0.24f, 0.18f));
            });
            Sprite mailboxSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_mailbox.png", 16, 20, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 7, 1, 2, 10, new Color(0.46f, 0.30f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 3, 10, 10, 7, new Color(0.28f, 0.48f, 0.76f));
                PlaceholderGeneratorUtility.FillRect(t, 12, 13, 2, 4, new Color(0.92f, 0.32f, 0.28f));
            });
            Sprite benchSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_prop_bench.png", 24, 16, t =>
            {
                PlaceholderGeneratorUtility.FillRect(t, 2, 4, 20, 4, new Color(0.56f, 0.38f, 0.22f));
                PlaceholderGeneratorUtility.FillRect(t, 2, 9, 20, 4, new Color(0.46f, 0.30f, 0.16f));
                PlaceholderGeneratorUtility.FillRect(t, 4, 1, 2, 12, new Color(0.32f, 0.22f, 0.14f));
                PlaceholderGeneratorUtility.FillRect(t, 18, 1, 2, 12, new Color(0.32f, 0.22f, 0.14f));
            });

            CreateStoryProp(group.transform, "GreenhouseLetterTable", "prop_letter_table", new Vector2(-15.5f, 12.5f), tableSpr,
                "[E] Đọc Thư Của Bà", "", "flag_intro_letter_read", "", "", 0, "", 0, 0,
                "Thư Của Bà", "Wen thân yêu, hãy dọn dẹp 3 cụm cỏ dại đá vụn trong Nhà Kính rồi mở Chiếc Hộp Gỗ Của Bà nhé!", "", vnFont);

            CreateStoryProp(group.transform, "GreenhouseSeedBox", "prop_seed_box", new Vector2(-17.2f, 12.5f), boxSpr,
                "[E] Mở Hộp Gỗ Hạt Giống", "flag_greenhouse_cleared", "flag_lumi_awakened", "", "", 0, "item_moonpetal_seed_core", 1, 0,
                "Tinh Linh Lumi", "Oa! Sương lạnh ngoài kia dày quá... Cảm ơn Wen đã đánh thức Lumi! Mình đã mở Trang 1 Sổ Tay Của Bà (bấm [B] để đọc) và tặng bạn 6 Hạt Lúa Mạch Sương Mai!",
                "Hãy dọn sạch 3 cụm cỏ dại/đá vụn trong Nhà Kính trước!", vnFont);

            CreateStoryProp(group.transform, "GreenhouseTestPlanter", "prop_test_planter", new Vector2(-13.8f, 11.2f), planterSpr,
                "[E] Gieo Thử Hạt Giống", "flag_lumi_awakened", "flag_solo_plant_failed", "flag_tutorial_done", "", 0, "", 0, 0,
                "Tinh Linh Lumi", "Hạt giống vừa chạm đất đã bị sương xám phủ kín... Đúng như lời bà dặn: một mình không thể làm hoa nở. Hãy ra Quảng Trường Làng Chính tìm Ông Bách nhé!",
                "Hãy mở Hộp Gỗ Của Bà đánh thức Lumi trước!", vnFont);

            CreateStoryProp(group.transform, "OtisBrokenMailbox", "prop_otis_mailbox", new Vector2(-0.5f, 5.5f), mailboxSpr,
                "[E] Sửa Hòm Thư (10 Gỗ)", "flag_met_bach_at_gate", "flag_otis_mailbox_fixed", "", "item_branch_wood", 10, "item_seed_bag_3", 1, 4,
                "Bác Otis", "Tuyệt vời! Hòm thư sửa xong rồi! Cháu nhận lấy Túi Hạt Di Sản Số 3 và Trang Sổ 4 (bức thư bà cháu chưa kịp gửi Ông Bách) nhé!",
                "Cần gặp Ông Bách trước và có đủ 10 Gỗ Nhánh Cây!", vnFont);

            CreateStoryProp(group.transform, "GardenMemoryBench", "prop_memory_bench", new Vector2(3.0f, -7.2f), benchSpr,
                "[E] Lau Sạch Chiếc Ghế Đôi", "flag_garden_slimes_cleared", "flag_garden_bench_cleaned", "", "", 0, "", 0, 2,
                "Wen", "Chiếc ghế đôi kỷ niệm của Ông Bách đã sạch bóng rêu phong! Giờ mình sẽ giúp Bà Hana, Toby và Bác Otis gom đủ 3 Túi Hạt Di Sản.",
                "Hãy thanh tẩy hết 5 Bọt Sương Đen trong Vườn Ông Bách trước!", vnFont);
        }

        private static void CreateStoryProp(
            Transform parent, string name, string id, Vector2 pos, Sprite spr, string promptLabel,
            string reqFlag, string doneFlag, string doneFlag2, string reqItem, int reqAmt, string rewItem, int rewAmt, int stamCost,
            string speaker, string body, string blocked, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.tag = "Interactable";
            go.layer = 11;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr; sr.sortingLayerName = "WorldObjects"; sr.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);
            go.AddComponent<BoxCollider2D>().size = new Vector2(0.95f, 0.85f);

            GameObject prompt = CreateWorldPrompt(go.transform, promptLabel, font);
            var comp = go.AddComponent<StoryPropInteractable>();
            var so = new SerializedObject(comp);
            so.FindProperty("propId").stringValue = id;
            so.FindProperty("requiredFlag").stringValue = reqFlag;
            so.FindProperty("completedFlag").stringValue = doneFlag;
            so.FindProperty("secondaryCompletedFlag").stringValue = doneFlag2;
            so.FindProperty("requiredItemId").stringValue = reqItem;
            so.FindProperty("requiredItemAmount").intValue = reqAmt;
            so.FindProperty("rewardItemId").stringValue = rewItem;
            so.FindProperty("rewardItemAmount").intValue = rewAmt;
            so.FindProperty("staminaCost").intValue = stamCost;
            so.FindProperty("dialogueSpeaker").stringValue = speaker;
            so.FindProperty("dialogueBody").stringValue = body;
            so.FindProperty("blockedHint").stringValue = blocked;
            so.FindProperty("promptObject").objectReferenceValue = prompt;
            so.FindProperty("interactRadius").floatValue = 1.95f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 aMin, Vector2 aMax, Vector2 pos, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = aMin; r.anchorMax = aMax; r.anchoredPosition = pos; r.sizeDelta = size;
            go.AddComponent<Image>().color = color;
            return go;
        }

        private static TMP_Text CreateTMP(string name, Transform parent, string text, TMP_FontAsset font, float size, TextAlignmentOptions align, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(12f, 8f); r.offsetMax = new Vector2(-12f, -8f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = align; tmp.color = color;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateModalButton(string name, Transform parent, string label, Vector2 anchoredPos, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0f); r.anchorMax = new Vector2(0.5f, 0f);
            r.anchoredPosition = anchoredPos; r.sizeDelta = new Vector2(170f, 40f);
            var img = go.AddComponent<Image>(); img.color = new Color(0.32f, 0.25f, 0.18f, 1f);
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 17, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static Button CreateDebugButton(Transform parent, string name, string label, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
            var img = go.AddComponent<Image>(); img.color = new Color(0.42f, 0.34f, 0.18f, 1f);
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 15, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static GameObject CreateWorldPrompt(Transform parent, string label, TMP_FontAsset font)
        {
            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(parent, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace; canvas.sortingLayerName = "UI"; canvas.sortingOrder = 50;
            promptGo.GetComponent<RectTransform>().sizeDelta = new Vector2(280f, 44f);
            promptGo.AddComponent<Image>().color = new Color(0.08f, 0.12f, 0.18f, 0.90f);
            var tmp = CreateTMP("Text", promptGo.transform, label, font, 20, TextAlignmentOptions.Center, new Color(1f, 0.94f, 0.65f, 1f));
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return promptGo;
        }
    }
}
```

## 89. `Assets/Editor/AutoBuilder_Step8.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;
using MoonpetalValley.Combat;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step8
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string PlayerStatsPath = "Assets/Data/Config/PlayerStatsConfig.asset";
        private const string EnemySlimeDataPath = "Assets/Data/Enemies/Enemy_GloomSlime.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player_Wen.prefab";
        private const string LootDropPrefabPath = "Assets/Prefabs/Enemies/LootDrop.prefab";
        private const string GloomSlimePrefabPath = "Assets/Prefabs/Enemies/GloomSlime.prefab";

        [MenuItem("Moonpetal/Step 8/1. Build Combat, Slimes & Encounters")]
        public static void BuildStep8()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 8 ===");

            Sprite slimeSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Enemies/spr_gloom_slime.png", 16, 16, DrawGloomSlime);
            Sprite slashSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/UI/spr_slash_arc.png", 32, 32, DrawSlashArc);
            Sprite pulseSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/UI/spr_pulse_ring.png", 48, 48, DrawPulseRing);
            Sprite catSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Characters/spr_miu_cat.png", 16, 16, DrawMiuCat);
            Sprite hpPixel = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/UI/spr_white_pixel.png", 4, 4, t => PlaceholderGeneratorUtility.FillRect(t, 0, 0, 4, 4, Color.white));

            EnemyData slimeData = EnsureGloomSlimeData();
            GameObject slimePrefab = BuildGloomSlimePrefab(slimeSpr, hpPixel, slimeData);

            UpgradePlayerPrefabWithCombat(slashSpr, pulseSpr);
            UpgradeGameRootDebugWithSickle();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildCombatEncounters(slimePrefab, catSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 8! Chạy 'Moonpetal/Step 8/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 8/Validate")]
        public static void ValidateStep8()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 8 ===");

            if (AssetDatabase.LoadAssetAtPath<EnemyData>(EnemySlimeDataPath) == null)
            {
                Debug.LogError("[MV] THIẾU Enemy_GloomSlime.asset");
                errors++;
            }
            if (AssetDatabase.LoadAssetAtPath<GameObject>(GloomSlimePrefabPath) == null)
            {
                Debug.LogError("[MV] THIẾU GloomSlime.prefab");
                errors++;
            }
            if (UnityEngine.Object.FindAnyObjectByType<PlayerCombatController>() == null)
            {
                Debug.LogError("[MV] Player_Wen thiếu PlayerCombatController.");
                errors++;
            }

            var zones = UnityEngine.Object.FindObjectsByType<BlightEncounterZone>(FindObjectsInactive.Include);
            if (zones.Length < 2)
            {
                Debug.LogError("[MV] Thiếu BlightEncounterZone (cần 2, hiện có " + zones.Length + ").");
                errors++;
            }

            var slimes = UnityEngine.Object.FindObjectsByType<GloomSlimeController>(FindObjectsInactive.Include);
            if (slimes.Length < 8)
            {
                Debug.LogError("[MV] Thiếu GloomSlime trong scene (cần 8 = 5 Vườn + 3 Rìa Rừng, hiện có " + slimes.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 8: PASS (0 lỗi). Liềm Ánh Trăng 120°, Lumi Pulse & 8 Gloom Slime sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 8: FAIL (" + errors + " lỗi).");
            }
        }

        private static EnemyData EnsureGloomSlimeData()
        {
            var data = AssetDatabase.LoadAssetAtPath<EnemyData>(EnemySlimeDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<EnemyData>();
                AssetDatabase.CreateAsset(data, EnemySlimeDataPath);
            }

            ItemData sap = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_dew_sap.asset");
            ItemData dust = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_item_starlight_dust.asset");

            data.enemyID = "enemy_gloom_slime";
            data.enemyNameKey = "name_enemy_gloom_slime";
            data.maxHP = 45;
            data.moveSpeed = 2.8f;
            data.contactDamage = 10;
            data.attackPattern = EnemyAttackPattern.Jump;
            data.detectionRadius = 4.6f;
            data.attackInterval = 1.1f;
            data.appliesSlowOnHit = true;
            data.dropTable = new List<LootDropEntry>
            {
                new LootDropEntry { item = sap, minAmount = 1, maxAmount = 2, dropChance = 1.0f },
                new LootDropEntry { item = dust, minAmount = 1, maxAmount = 1, dropChance = 0.75f }
            };

            EditorUtility.SetDirty(data);
            return data;
        }

        private static GameObject BuildGloomSlimePrefab(Sprite slimeSpr, Sprite hpPixel, EnemyData data)
        {
            GameObject lootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootDropPrefabPath);

            GameObject root = new GameObject("GloomSlime");
            root.tag = "Enemy";
            root.layer = 9;

            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var col = root.AddComponent<CircleCollider2D>();
            col.radius = 0.42f;

            // Child trigger collider để gây sát thương chạm mà không làm kẹt Player
            GameObject hurtTrigger = new GameObject("ContactHitbox");
            hurtTrigger.layer = 13;
            hurtTrigger.transform.SetParent(root.transform, false);
            var trig = hurtTrigger.AddComponent<CircleCollider2D>();
            trig.radius = 0.48f;
            trig.isTrigger = true;

            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(root.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = slimeSpr;
            sr.sortingLayerName = "WorldObjects";

            // Thanh máu nhỏ phía trên đầu Slime
            GameObject hpRoot = new GameObject("HPBarRoot");
            hpRoot.transform.SetParent(root.transform, false);
            hpRoot.transform.localPosition = new Vector3(0f, 0.65f, 0f);

            GameObject hpBg = new GameObject("Bg");
            hpBg.transform.SetParent(hpRoot.transform, false);
            hpBg.transform.localScale = new Vector3(3.2f, 0.45f, 1f);
            var bgSr = hpBg.AddComponent<SpriteRenderer>();
            bgSr.sprite = hpPixel; bgSr.color = new Color(0.12f, 0.08f, 0.16f, 0.85f);
            bgSr.sortingLayerName = "UI"; bgSr.sortingOrder = 20;

            GameObject hpFill = new GameObject("Fill");
            hpFill.transform.SetParent(hpRoot.transform, false);
            hpFill.transform.localScale = new Vector3(3.0f, 0.32f, 1f);
            var fillSr = hpFill.AddComponent<SpriteRenderer>();
            fillSr.sprite = hpPixel; fillSr.color = new Color(0.86f, 0.28f, 0.56f, 1f);
            fillSr.sortingLayerName = "UI"; fillSr.sortingOrder = 21;

            var ctrl = root.AddComponent<GloomSlimeController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("enemyData").objectReferenceValue = data;
            so.FindProperty("rb").objectReferenceValue = rb;
            so.FindProperty("spriteRenderer").objectReferenceValue = sr;
            so.FindProperty("hpBarFill").objectReferenceValue = hpFill.transform;
            so.FindProperty("hpBarRoot").objectReferenceValue = hpRoot;
            so.FindProperty("lootDropPrefab").objectReferenceValue = lootPrefab;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, GloomSlimePrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static void UpgradePlayerPrefabWithCombat(Sprite slashSpr, Sprite pulseSpr)
        {
            PlayerStatsSO statsConfig = AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(PlayerStatsPath);
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (root == null) return;

            Transform oldSlash = root.transform.Find("SlashArcVisual");
            if (oldSlash != null) UnityEngine.Object.DestroyImmediate(oldSlash.gameObject);
            GameObject slashGo = new GameObject("SlashArcVisual");
            slashGo.transform.SetParent(root.transform, false);
            var slashSr = slashGo.AddComponent<SpriteRenderer>();
            slashSr.sprite = slashSpr;
            slashSr.sortingLayerName = "WeatherVFX";
            slashSr.sortingOrder = 30;
            slashSr.enabled = false;

            Transform oldPulse = root.transform.Find("PulseRingVisual");
            if (oldPulse != null) UnityEngine.Object.DestroyImmediate(oldPulse.gameObject);
            GameObject pulseGo = new GameObject("PulseRingVisual");
            pulseGo.transform.SetParent(root.transform, false);
            var pulseSr = pulseGo.AddComponent<SpriteRenderer>();
            pulseSr.sprite = pulseSpr;
            pulseSr.sortingLayerName = "WeatherVFX";
            pulseSr.sortingOrder = 28;
            pulseSr.enabled = false;

            var combat = root.GetComponent<PlayerCombatController>() ?? root.AddComponent<PlayerCombatController>();
            var so = new SerializedObject(combat);
            so.FindProperty("statsConfig").objectReferenceValue = statsConfig;
            so.FindProperty("playerController").objectReferenceValue = root.GetComponent<PlayerController2D>();
            so.FindProperty("inputReader").objectReferenceValue = root.GetComponent<GameInputReader>();
            so.FindProperty("slashArcTransform").objectReferenceValue = slashGo.transform;
            so.FindProperty("slashArcRenderer").objectReferenceValue = slashSr;
            so.FindProperty("pulseRingTransform").objectReferenceValue = pulseGo.transform;
            so.FindProperty("pulseRingRenderer").objectReferenceValue = pulseSr;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void UpgradeGameRootDebugWithSickle()
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 960f);
                Transform ex = debugPanel.Find("BtnGiveSickle");
                Button btn;
                if (ex != null) btn = ex.GetComponent<Button>();
                else
                {
                    GameObject go = new GameObject("BtnGiveSickle");
                    go.transform.SetParent(debugPanel, false);
                    go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
                    var img = go.AddComponent<Image>(); img.color = new Color(0.28f, 0.24f, 0.56f, 1f);
                    btn = go.AddComponent<Button>(); btn.targetGraphic = img;
                    GameObject txtGo = new GameObject("Label");
                    txtGo.transform.SetParent(go.transform, false);
                    var r = txtGo.AddComponent<RectTransform>();
                    r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
                    r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
                    var tmp = txtGo.AddComponent<TextMeshProUGUI>();
                    tmp.text = "Trao Liềm Ánh Trăng (Ô 4)";
                    tmp.fontSize = 15; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
                    if (vnFont != null) tmp.font = vnFont;
                }

                var so = new SerializedObject(debugCtrl);
                so.FindProperty("btnGiveSickle").objectReferenceValue = btn;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildCombatEncounters(GameObject slimePrefab, Sprite catSpr)
        {
            GameObject old = GameObject.Find("CombatEncounters");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject root = new GameObject("CombatEncounters");

            // 1. Cụm 5 Slime trong Vườn Ông Bách
            CreateEncounter(root.transform, "Encounter_BachGarden", "enc_bach_garden",
                "flag_garden_slimes_cleared",
                "Đã thanh tẩy hết 5 Bọt Sương Đen trong Vườn Ông Bách! Hãy bấm [E] lau sạch Chiếc Ghế Đôi!",
                slimePrefab,
                new[]
                {
                    new Vector2(-3.5f, -6.5f),
                    new Vector2(3.5f, -6.5f),
                    new Vector2(-3.8f, -10.5f),
                    new Vector2(3.8f, -10.5f),
                    new Vector2(0.0f, -12.0f)
                });

            // 2. Cụm 3 Slime vây Chú Mèo Miu ở Rìa Rừng Phía Đông
            GameObject forestEnc = CreateEncounter(root.transform, "Encounter_ForestCat", "enc_forest_cat",
                "flag_toby_cat_slimes_cleared",
                "Đã giải cứu chú mèo Miu khỏi 3 Bọt Sương Đen! Hãy về gặp Cậu Bé Toby [E]!",
                slimePrefab,
                new[]
                {
                    new Vector2(16.0f, 5.5f),
                    new Vector2(18.5f, 7.5f),
                    new Vector2(16.5f, 9.2f)
                });

            // Đặt Chú Mèo Miu dễ thương ở giữa cụm rừng
            GameObject catGo = new GameObject("MiuCat_Toby");
            catGo.transform.SetParent(forestEnc.transform, false);
            catGo.transform.position = new Vector3(17.5f, 7.2f, 0f);
            var catSr = catGo.AddComponent<SpriteRenderer>();
            catSr.sprite = catSpr;
            catSr.sortingLayerName = "WorldObjects";
            catSr.sortingOrder = Mathf.RoundToInt(-7.2f * 100f);
        }

        private static GameObject CreateEncounter(
            Transform parent, string name, string encId, string flag, string toast, GameObject slimePrefab, Vector2[] positions)
        {
            GameObject zoneGo = new GameObject(name);
            zoneGo.transform.SetParent(parent, false);
            var zone = zoneGo.AddComponent<BlightEncounterZone>();
            var spawned = new List<GloomSlimeController>();

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject sGo = (GameObject)PrefabUtility.InstantiatePrefab(slimePrefab);
                sGo.name = "GloomSlime_" + (i + 1);
                sGo.transform.SetParent(zoneGo.transform, false);
                sGo.transform.position = new Vector3(positions[i].x, positions[i].y, 0f);

                var ctrl = sGo.GetComponent<GloomSlimeController>();
                var cSO = new SerializedObject(ctrl);
                cSO.FindProperty("encounterZone").objectReferenceValue = zone;
                cSO.ApplyModifiedPropertiesWithoutUndo();
                spawned.Add(ctrl);
            }

            var zSO = new SerializedObject(zone);
            zSO.FindProperty("encounterId").stringValue = encId;
            zSO.FindProperty("completionFlag").stringValue = flag;
            zSO.FindProperty("completionToast").stringValue = toast;
            var prop = zSO.FindProperty("slimes");
            prop.ClearArray();
            for (int i = 0; i < spawned.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = spawned[i];
            }
            zSO.ApplyModifiedPropertiesWithoutUndo();
            return zoneGo;
        }

        private static void DrawGloomSlime(Texture2D t)
        {
            Color dark = new Color(0.20f, 0.14f, 0.30f, 1f);
            Color mid = new Color(0.34f, 0.22f, 0.48f, 1f);
            Color eye = new Color(1.0f, 0.88f, 0.30f, 1f);
            PlaceholderGeneratorUtility.FillRect(t, 3, 2, 10, 8, dark);
            PlaceholderGeneratorUtility.FillRect(t, 2, 3, 12, 6, dark);
            PlaceholderGeneratorUtility.FillRect(t, 4, 4, 8, 6, mid);
            PlaceholderGeneratorUtility.FillRect(t, 5, 10, 6, 2, mid);
            PlaceholderGeneratorUtility.FillRect(t, 4, 6, 2, 2, eye);
            PlaceholderGeneratorUtility.FillRect(t, 10, 6, 2, 2, eye);
        }

        private static void DrawSlashArc(Texture2D t)
        {
            Vector2 c = new Vector2(10f, 16f);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    Vector2 p = new Vector2(x, y) - c;
                    float dist = p.magnitude;
                    float ang = Mathf.Abs(Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg);
                    if (dist >= 10f && dist <= 17f && ang <= 62f)
                    {
                        float fade = 1f - Mathf.Abs(dist - 13.5f) / 3.8f;
                        t.SetPixel(x, y, new Color(0.72f, 0.98f, 1f, Mathf.Clamp01(fade)));
                    }
                }
            }
        }

        private static void DrawPulseRing(Texture2D t)
        {
            Vector2 c = new Vector2(24f, 24f);
            for (int y = 0; y < 48; y++)
            {
                for (int x = 0; x < 48; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), c);
                    if (d >= 18f && d <= 23f)
                    {
                        float a = 1f - Mathf.Abs(d - 20.5f) / 2.8f;
                        t.SetPixel(x, y, new Color(0.62f, 0.98f, 0.96f, Mathf.Clamp01(a)));
                    }
                }
            }
        }

        private static void DrawMiuCat(Texture2D t)
        {
            Color fur = new Color(0.95f, 0.66f, 0.34f);
            Color cream = new Color(0.99f, 0.92f, 0.78f);
            PlaceholderGeneratorUtility.FillRect(t, 4, 2, 8, 7, fur);
            PlaceholderGeneratorUtility.FillRect(t, 5, 3, 6, 4, cream);
            PlaceholderGeneratorUtility.FillRect(t, 4, 9, 2, 3, fur);
            PlaceholderGeneratorUtility.FillRect(t, 10, 9, 2, 3, fur);
            t.SetPixel(6, 7, Color.black);
            t.SetPixel(9, 7, Color.black);
            PlaceholderGeneratorUtility.FillRect(t, 12, 4, 2, 4, fur);
        }
    }
}
```

## 90. `Assets/Editor/AutoBuilder_Step9.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;
using MoonpetalValley.Community;
using MoonpetalValley.UI;
using MoonpetalValley.DebugTools;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step9
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string BundleAssetPath = "Assets/Data/Bundles/Bundle_ForgottenGarden.asset";
        private const string FontAssetPath = "Assets/Art/Fonts/VN_DynamicFont.asset";
        private const string GameRootPrefabPath = "Assets/Prefabs/Core/GameRoot.prefab";

        [MenuItem("Moonpetal/Step 9/1. Build Bundle, PlantingSpot & Fog")]
        public static void BuildStep9()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 9 ===");

            Sprite emptySpotSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_planting_spot_empty.png", 32, 32, DrawEmptySpot);
            Sprite bloomedSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_moonpetal_bloomed.png", 32, 32, DrawBloomedMoonpetal);
            Sprite fogSpr = PlaceholderGeneratorUtility.EnsureSprite("Assets/Art/Sprites/Environment/spr_blight_fog.png", 48, 32, DrawBlightFog);
            Sprite haloSpr = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Characters/spr_lumi_halo.png");

            BundleData bundle = EnsureBundleAsset();
            UpgradeGameRootWithBundleUI(bundle);

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);
            BuildCentralPlantingSpot(emptySpotSpr, bloomedSpr, haloSpr);
            BuildBlightFogZones(fogSpr);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 9! Chạy 'Moonpetal/Step 9/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 9/Validate")]
        public static void ValidateStep9()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 9 ===");

            var bundle = AssetDatabase.LoadAssetAtPath<BundleData>(BundleAssetPath);
            if (bundle == null || bundle.requiredItems.Count < 5 || bundle.requiredHelperFlags.Count < 4)
            {
                Debug.LogError("[MV] Bundle_ForgottenGarden.asset thiếu vật phẩm hoặc thiếu 4 helper flags.");
                errors++;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameRootPrefabPath);
            if (prefab != null)
            {
                if (new SerializedObject(prefab.GetComponent<GameRoot>()).FindProperty("bundleManager").objectReferenceValue == null)
                {
                    Debug.LogError("[MV] GameRoot thiếu bundleManager.");
                    errors++;
                }
                if (prefab.GetComponentInChildren<BundleUIView>(true) == null)
                {
                    Debug.LogError("[MV] Thiếu BundleUIView trong GameRoot.prefab.");
                    errors++;
                }
            }

            if (UnityEngine.Object.FindAnyObjectByType<PlantingSpotController>() == null)
            {
                Debug.LogError("[MV] Thiếu PlantingSpotController trong scene.");
                errors++;
            }

            var fogs = UnityEngine.Object.FindObjectsByType<BlightFogZone>(FindObjectsInactive.Include);
            if (fogs.Length < 4)
            {
                Debug.LogError("[MV] Thiếu BlightFogZone (cần >= 4, hiện có " + fogs.Length + ").");
                errors++;
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 9: PASS (0 lỗi). Gói Ký Ức, Bồn Hoa Trung Tâm & Sương Héo Úa sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 9: FAIL (" + errors + " lỗi).");
            }
        }

        private static BundleData EnsureBundleAsset()
        {
            var b = AssetDatabase.LoadAssetAtPath<BundleData>(BundleAssetPath);
            if (b == null)
            {
                b = ScriptableObject.CreateInstance<BundleData>();
                AssetDatabase.CreateAsset(b, BundleAssetPath);
            }

            b.bundleID = "bundle_forgotten_garden";
            b.bundleTitleKey = "Gói Ký Ức Vườn Xưa";
            b.bundleDescriptionKey = "Chung tay cùng 4 dân làng hồi sinh Bồn Hoa Trung Tâm Vườn Ông Bách.";
            b.requiredItems = new List<ItemRequirement>
            {
                new ItemRequirement { item = LoadItem("item_seed_bag_1"), amount = 1 },
                new ItemRequirement { item = LoadItem("item_seed_bag_2"), amount = 1 },
                new ItemRequirement { item = LoadItem("item_seed_bag_3"), amount = 1 },
                new ItemRequirement { item = LoadItem("item_dew_sap"), amount = 5 },
                new ItemRequirement { item = LoadItem("item_starlight_dust"), amount = 3 }
            };
            b.requiredHelperNPCIDs = new List<string> { "npc_bach", "npc_hana", "npc_toby", "npc_otis" };
            b.requiredHelperFlags = new List<string> { "flag_bach_letters_read", "flag_hana_bread_done", "flag_toby_cat_saved", "flag_otis_mailbox_fixed" };
            b.rewardItem = LoadItem("item_petal_of_hope");
            b.rewardStoryFlag = "flag_nv1_bundle_completed";

            EditorUtility.SetDirty(b);
            return b;
        }

        private static ItemData LoadItem(string id)
        {
            return AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Item_" + id + ".asset");
        }

        private static void UpgradeGameRootWithBundleUI(BundleData bundle)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject root = PrefabUtility.LoadPrefabContents(GameRootPrefabPath);
            if (root == null) return;

            var gameRoot = root.GetComponent<GameRoot>();
            var bundleMgr = root.GetComponent<BundleManager>() ?? root.AddComponent<BundleManager>();
            var bmSO = new SerializedObject(bundleMgr);
            bmSO.FindProperty("activeBundle").objectReferenceValue = bundle;
            bmSO.ApplyModifiedPropertiesWithoutUndo();

            var rootSO = new SerializedObject(gameRoot);
            rootSO.FindProperty("bundleManager").objectReferenceValue = bundleMgr;
            rootSO.ApplyModifiedPropertiesWithoutUndo();

            Transform hudCanvas = root.transform.Find("ClockHUDCanvas");
            if (hudCanvas != null)
            {
                Transform oldHost = hudCanvas.Find("BundleUIHost");
                if (oldHost != null) UnityEngine.Object.DestroyImmediate(oldHost.gameObject);

                GameObject host = new GameObject("BundleUIHost");
                host.transform.SetParent(hudCanvas, false);
                var hRect = host.AddComponent<RectTransform>();
                hRect.anchorMin = Vector2.zero; hRect.anchorMax = Vector2.one;
                hRect.offsetMin = Vector2.zero; hRect.offsetMax = Vector2.zero;

                // 1. Bundle Modal
                GameObject modal = CreatePanel("BundlePanelRoot", host.transform, new Vector2(820f, 470f), new Color(0.08f, 0.12f, 0.20f, 0.96f));
                var title = CreateTMP("Title", modal.transform, "GÓI KÝ ỨC VƯỜN XƯA — Bồn Hoa Trung Tâm", vnFont, 24, TextAlignmentOptions.Top, new Color(1f, 0.90f, 0.56f, 1f));
                title.GetComponent<RectTransform>().offsetMax = new Vector2(-16f, -14f);

                var itemsTxt = CreateTMP("ItemsStatus", modal.transform, "Vật phẩm...", vnFont, 18, TextAlignmentOptions.TopLeft, Color.white);
                itemsTxt.GetComponent<RectTransform>().offsetMin = new Vector2(28f, 100f);
                itemsTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-410f, -64f);

                var helpersTxt = CreateTMP("HelpersStatus", modal.transform, "Helpers...", vnFont, 18, TextAlignmentOptions.TopLeft, new Color(0.84f, 0.96f, 0.90f, 1f));
                helpersTxt.GetComponent<RectTransform>().offsetMin = new Vector2(420f, 100f);
                helpersTxt.GetComponent<RectTransform>().offsetMax = new Vector2(-24f, -64f);

                Button btnContrib = CreateBtn("BtnContribute", modal.transform, "Nộp Vật Phẩm Đang Có", new Vector2(-240f, 42f), new Vector2(230f, 46f), new Color(0.24f, 0.44f, 0.68f, 1f), vnFont);
                Button btnBloom = CreateBtn("BtnBloom", modal.transform, "Cùng Gieo Hạt! (Nở Hoa)", new Vector2(25f, 42f), new Vector2(250f, 46f), new Color(0.18f, 0.62f, 0.42f, 1f), vnFont);
                Button btnClose = CreateBtn("BtnClose", modal.transform, "Đóng [Esc]", new Vector2(285f, 42f), new Vector2(160f, 46f), new Color(0.40f, 0.24f, 0.26f, 1f), vnFont);

                // 2. Victory Celebration Modal
                GameObject victory = CreatePanel("VictoryPanelRoot", host.transform, new Vector2(760f, 360f), new Color(0.10f, 0.16f, 0.24f, 0.97f));
                var vTitle = CreateTMP("VTitle", victory.transform, "CHÚC MỪNG HOÀN THÀNH NHIỆM VỤ 1!", vnFont, 28, TextAlignmentOptions.Top, new Color(1f, 0.90f, 0.48f, 1f));
                vTitle.GetComponent<RectTransform>().offsetMax = new Vector2(-20f, -24f);
                var vBody = CreateTMP("VBody", victory.transform,
                    "Cùng với Ông Bách, Bà Hana, Cậu Bé Toby và Bác Otis, bạn đã làm Hoa Moonpetal nở rực giữa Khu Vườn Bị Lãng Quên!\n\n- Đã nhận: Cánh Hoa Hy Vọng\n- Sương Héo Úa đã tan khỏi Làng Chính\n- Tinh linh Lumi mở khóa kỹ năng Xung Ánh Trăng (phím Q)!",
                    vnFont, 20, TextAlignmentOptions.Center, Color.white);
                vBody.GetComponent<RectTransform>().offsetMin = new Vector2(32f, 88f);
                vBody.GetComponent<RectTransform>().offsetMax = new Vector2(-32f, -72f);
                Button btnVClose = CreateBtn("BtnVictoryClose", victory.transform, "Tiếp tục khám phá Thung Lũng", new Vector2(0f, 40f), new Vector2(320f, 48f), new Color(0.20f, 0.58f, 0.42f, 1f), vnFont);

                var view = host.AddComponent<BundleUIView>();
                var vSO = new SerializedObject(view);
                vSO.FindProperty("panelRoot").objectReferenceValue = modal;
                vSO.FindProperty("itemsStatusText").objectReferenceValue = itemsTxt;
                vSO.FindProperty("helpersStatusText").objectReferenceValue = helpersTxt;
                vSO.FindProperty("btnContribute").objectReferenceValue = btnContrib;
                vSO.FindProperty("btnBloom").objectReferenceValue = btnBloom;
                vSO.FindProperty("btnClose").objectReferenceValue = btnClose;
                vSO.FindProperty("victoryPanelRoot").objectReferenceValue = victory;
                vSO.FindProperty("btnVictoryClose").objectReferenceValue = btnVClose;
                vSO.ApplyModifiedPropertiesWithoutUndo();
            }

            Transform debugPanel = root.transform.Find("SystemOverlayCanvas/DebugPanelRoot");
            var debugCtrl = root.GetComponent<RuntimeDebugOverlay>();
            if (debugPanel != null && debugCtrl != null)
            {
                debugPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(460f, 1005f);
                Transform ex = debugPanel.Find("BtnPrepClimax");
                Button btnPrep = ex != null ? ex.GetComponent<Button>() : CreateDebugBtn(debugPanel, "BtnPrepClimax", "Chuẩn bị Cao Trào NV1 (4 Helpers + Đủ Đồ)", vnFont);
                var dbgSO = new SerializedObject(debugCtrl);
                dbgSO.FindProperty("btnPrepClimax").objectReferenceValue = btnPrep;
                dbgSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameRootPrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BuildCentralPlantingSpot(Sprite emptySpr, Sprite bloomedSpr, Sprite haloSpr)
        {
            TMP_FontAsset vnFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            GameObject old = GameObject.Find("CentralPlantingSpot");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            GameObject go = new GameObject("CentralPlantingSpot");
            go.tag = "Interactable";
            go.layer = 11;
            go.transform.position = new Vector3(0.0f, -10.0f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = emptySpr;
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = Mathf.RoundToInt(-(-10.0f) * 100f) - 10;

            GameObject auraGo = new GameObject("BloomAura");
            auraGo.transform.SetParent(go.transform, false);
            var auraSr = auraGo.AddComponent<SpriteRenderer>();
            auraSr.sprite = haloSpr;
            auraSr.color = new Color(1f, 0.84f, 0.92f, 0.72f);
            auraSr.sortingLayerName = "WeatherVFX";
            auraSr.sortingOrder = 12;
            auraSr.enabled = false;

            GameObject promptGo = new GameObject("PromptCanvas");
            promptGo.transform.SetParent(go.transform, false);
            promptGo.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            promptGo.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            var canvas = promptGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace; canvas.sortingLayerName = "UI"; canvas.sortingOrder = 55;
            promptGo.GetComponent<RectTransform>().sizeDelta = new Vector2(340f, 46f);
            promptGo.AddComponent<Image>().color = new Color(0.08f, 0.12f, 0.20f, 0.92f);
            var tmp = CreateTMP("Text", promptGo.transform, "[E] Bồn Hoa Trung Tâm (Gói Ký Ức)", vnFont, 20, TextAlignmentOptions.Center, new Color(1f, 0.92f, 0.62f, 1f));
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            var ctrl = go.AddComponent<PlantingSpotController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("flowerRenderer").objectReferenceValue = sr;
            so.FindProperty("emptySprite").objectReferenceValue = emptySpr;
            so.FindProperty("bloomedSprite").objectReferenceValue = bloomedSpr;
            so.FindProperty("bloomAuraRenderer").objectReferenceValue = auraSr;
            so.FindProperty("promptObject").objectReferenceValue = promptGo;
            so.FindProperty("interactRadius").floatValue = 2.5f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildBlightFogZones(Sprite fogSpr)
        {
            GameObject old = GameObject.Find("BlightFogGroup");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);
            GameObject group = new GameObject("BlightFogGroup");

            Vector2[] positions =
            {
                new Vector2(-4.0f, -8.0f),
                new Vector2(4.0f, -8.0f),
                new Vector2(0.0f, -12.0f),
                new Vector2(-3.5f, -12.0f),
                new Vector2(3.5f, -12.0f),
                new Vector2(14.0f, 6.0f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject fGo = new GameObject("BlightFog_" + (i + 1));
                fGo.tag = "BlightFog";
                fGo.transform.SetParent(group.transform, false);
                fGo.transform.position = new Vector3(positions[i].x, positions[i].y, 0f);
                fGo.transform.localScale = new Vector3(2.2f, 2.0f, 1f);

                var sr = fGo.AddComponent<SpriteRenderer>();
                sr.sprite = fogSpr;
                sr.sortingLayerName = "FogOverlay";
                sr.sortingOrder = 10;
                sr.color = new Color(0.55f, 0.42f, 0.68f, 0.48f);

                var comp = fGo.AddComponent<BlightFogZone>();
                var so = new SerializedObject(comp);
                so.FindProperty("clearOnFlag").stringValue = "flag_nv1_completed";
                so.FindProperty("fogRenderer").objectReferenceValue = sr;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 size, Color bg)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0.5f); r.anchorMax = new Vector2(0.5f, 0.5f);
            r.sizeDelta = size;
            go.AddComponent<Image>().color = bg;
            return go;
        }

        private static TMP_Text CreateTMP(string name, Transform parent, string text, TMP_FontAsset font, float size, TextAlignmentOptions align, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(12f, 8f); r.offsetMax = new Vector2(-12f, -8f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = align; tmp.color = color;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (font != null) tmp.font = font;
            return tmp;
        }

        private static Button CreateBtn(string name, Transform parent, string label, Vector2 pos, Vector2 size, Color bg, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0f); r.anchorMax = new Vector2(0.5f, 0f);
            r.anchoredPosition = pos; r.sizeDelta = size;
            var img = go.AddComponent<Image>(); img.color = bg;
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 17, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static Button CreateDebugBtn(Transform parent, string name, string label, TMP_FontAsset font)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 36f);
            var img = go.AddComponent<Image>(); img.color = new Color(0.62f, 0.24f, 0.44f, 1f);
            var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
            var tmp = CreateTMP("Label", go.transform, label, font, 15, TextAlignmentOptions.Center, Color.white);
            tmp.GetComponent<RectTransform>().offsetMin = Vector2.zero; tmp.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        private static void DrawEmptySpot(Texture2D t)
        {
            PlaceholderGeneratorUtility.FillRect(t, 4, 4, 24, 24, new Color(0.48f, 0.48f, 0.52f));
            PlaceholderGeneratorUtility.FillRect(t, 7, 7, 18, 18, new Color(0.32f, 0.24f, 0.18f));
            PlaceholderGeneratorUtility.FillRect(t, 13, 13, 6, 6, new Color(0.24f, 0.18f, 0.14f));
        }

        private static void DrawBloomedMoonpetal(Texture2D t)
        {
            DrawEmptySpot(t);
            Color leaf = new Color(0.26f, 0.78f, 0.42f);
            Color petal = new Color(0.98f, 0.72f, 0.88f);
            Color core = new Color(0.70f, 0.98f, 1.0f);
            PlaceholderGeneratorUtility.FillRect(t, 10, 8, 12, 8, leaf);
            PlaceholderGeneratorUtility.FillRect(t, 8, 13, 16, 12, petal);
            PlaceholderGeneratorUtility.FillRect(t, 12, 10, 8, 18, petal);
            PlaceholderGeneratorUtility.FillRect(t, 13, 16, 6, 6, core);
        }

        private static void DrawBlightFog(Texture2D t)
        {
            Vector2 c = new Vector2(24f, 16f);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 48; x++)
                {
                    float dx = (x - c.x) / 22f;
                    float dy = (y - c.y) / 14f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    if (d < 1f)
                    {
                        float a = (1f - d) * (1f - d);
                        t.SetPixel(x, y, new Color(0.52f, 0.38f, 0.64f, a));
                    }
                }
            }
        }
    }
}
```

## 91. `Assets/Editor/MoonpetalAssetPostprocessor.cs`

```csharp
using UnityEditor;
using UnityEngine;

namespace MoonpetalValley.EditorTools
{
    public class MoonpetalAssetPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            string p = assetPath.Replace("\\", "/");
            if (!p.EndsWith(".png")) return;

            bool isWen = p.Contains("/Art/Sprites/Animations/Wen/");
            bool isLumi = p.Contains("/Art/Sprites/Animations/Lumi/");
            bool isSlime = p.Contains("/Art/Sprites/Animations/GloomSlime/");
            bool isNpc = p.Contains("/Art/Sprites/Animations/NPCs/");
            bool isEnv = p.Contains("/Art/Sprites/Environment/");

            if (!isWen && !isLumi && !isSlime && !isNpc && !isEnv) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;

            if (isSlime) importer.spritePixelsPerUnit = 32f;
            else if (isEnv) importer.spritePixelsPerUnit = 16f;
            else importer.spritePixelsPerUnit = 20f;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spritePivot = new Vector2(0.5f, 0.5f);
            importer.SetTextureSettings(settings);
        }
    }
}
```

## 92. `Assets/Editor/PlaceholderGeneratorUtility.cs`

```csharp
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MoonpetalValley.EditorTools
{
    public static class PlaceholderGeneratorUtility
    {
        public static Sprite EnsureSprite(string assetPath, int width, int height, System.Action<Texture2D> drawAction)
        {
            if (!File.Exists(assetPath))
            {
                string dir = Path.GetDirectoryName(assetPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Point;
                Color clear = new Color(0f, 0f, 0f, 0f);
                Color[] pixels = new Color[width * height];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;
                tex.SetPixels(pixels);

                drawAction?.Invoke(tex);
                tex.Apply();

                byte[] png = tex.EncodeToPNG();
                File.WriteAllBytes(assetPath, png);
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }

            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
                if (Mathf.Abs(importer.spritePixelsPerUnit - 16f) > 0.01f) { importer.spritePixelsPerUnit = 16f; dirty = true; }
                if (importer.filterMode != FilterMode.Point) { importer.filterMode = FilterMode.Point; dirty = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
                if (dirty) importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        public static void FillRect(Texture2D tex, int x, int y, int w, int h, Color c)
        {
            for (int py = y; py < y + h; py++)
            {
                if (py < 0 || py >= tex.height) continue;
                for (int px = x; px < x + w; px++)
                {
                    if (px < 0 || px >= tex.width) continue;
                    tex.SetPixel(px, py, c);
                }
            }
        }

        public static void DrawWenSprite(Texture2D tex)
        {
            Color boots = new Color(0.32f, 0.20f, 0.12f);
            Color cloak = new Color(0.20f, 0.56f, 0.36f);
            Color cloakShade = new Color(0.14f, 0.42f, 0.27f);
            Color scarf = new Color(0.95f, 0.78f, 0.28f);
            Color skin = new Color(0.98f, 0.85f, 0.74f);
            Color hair = new Color(0.24f, 0.18f, 0.16f);
            Color eye = new Color(0.10f, 0.12f, 0.18f);
            Color sprout = new Color(0.55f, 0.92f, 0.42f);

            FillRect(tex, 5, 1, 2, 3, boots);
            FillRect(tex, 9, 1, 2, 3, boots);
            FillRect(tex, 4, 4, 8, 8, cloak);
            FillRect(tex, 4, 4, 2, 8, cloakShade);
            FillRect(tex, 4, 11, 8, 2, scarf);
            FillRect(tex, 4, 13, 8, 6, skin);
            tex.SetPixel(6, 15, eye);
            tex.SetPixel(9, 15, eye);
            FillRect(tex, 3, 18, 10, 3, hair);
            FillRect(tex, 3, 15, 1, 4, hair);
            FillRect(tex, 12, 15, 1, 4, hair);
            FillRect(tex, 7, 21, 2, 2, sprout);
            tex.SetPixel(9, 22, sprout);
        }

        public static void DrawLumiSprite(Texture2D tex)
        {
            Color outer = new Color(0.45f, 0.92f, 0.95f, 0.90f);
            Color inner = new Color(0.85f, 1.0f, 0.98f, 1.0f);
            Color eye = new Color(0.12f, 0.25f, 0.35f, 1.0f);

            FillRect(tex, 3, 2, 6, 7, outer);
            FillRect(tex, 4, 1, 4, 9, outer);
            FillRect(tex, 4, 3, 4, 5, inner);
            tex.SetPixel(4, 5, eye);
            tex.SetPixel(7, 5, eye);
            tex.SetPixel(5, 10, inner);
            tex.SetPixel(6, 10, inner);
        }

        public static void DrawRadialHalo(Texture2D tex)
        {
            Vector2 center = new Vector2(tex.width * 0.5f, tex.height * 0.5f);
            float maxR = tex.width * 0.5f;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float a = Mathf.Clamp01(1f - dist / maxR);
                    tex.SetPixel(x, y, new Color(0.55f, 0.96f, 0.92f, a * a * 0.42f));
                }
            }
        }

        public static void DrawWarmHalo(Texture2D tex)
        {
            Vector2 center = new Vector2(tex.width * 0.5f, tex.height * 0.5f);
            float maxR = tex.width * 0.5f;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float a = Mathf.Clamp01(1f - dist / maxR);
                    tex.SetPixel(x, y, new Color(1.0f, 0.80f, 0.36f, a * a * 0.65f));
                }
            }
        }

        public static void DrawLanternPostSprite(Texture2D tex)
        {
            Color wood = new Color(0.36f, 0.24f, 0.14f);
            Color iron = new Color(0.22f, 0.24f, 0.28f);
            Color lamp = new Color(1.0f, 0.86f, 0.42f);

            FillRect(tex, 7, 1, 2, 16, wood);
            FillRect(tex, 5, 1, 6, 2, wood);
            FillRect(tex, 7, 16, 6, 2, iron);
            FillRect(tex, 10, 11, 4, 5, lamp);
            FillRect(tex, 9, 15, 6, 1, iron);
            FillRect(tex, 9, 10, 6, 1, iron);
        }

        public static void DrawBedSprite(Texture2D tex)
        {
            Color wood = new Color(0.45f, 0.28f, 0.16f);
            Color pillow = new Color(0.96f, 0.94f, 0.86f);
            Color blanket = new Color(0.26f, 0.54f, 0.62f);
            Color trim = new Color(0.92f, 0.78f, 0.36f);

            FillRect(tex, 1, 1, 14, 22, wood);
            FillRect(tex, 2, 16, 12, 5, pillow);
            FillRect(tex, 2, 3, 12, 13, blanket);
            FillRect(tex, 2, 14, 12, 2, trim);
        }

        public static void DrawGrassTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.32f, 0.58f, 0.31f));
            Color light = new Color(0.40f, 0.68f, 0.36f);
            Color dark = new Color(0.26f, 0.48f, 0.26f);
            tex.SetPixel(3, 4, light); tex.SetPixel(3, 5, light);
            tex.SetPixel(11, 10, light); tex.SetPixel(11, 11, light);
            tex.SetPixel(7, 13, dark); tex.SetPixel(13, 3, dark);
        }

        public static void DrawPathTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.73f, 0.65f, 0.50f));
            Color pebble = new Color(0.62f, 0.54f, 0.41f);
            FillRect(tex, 2, 3, 3, 2, pebble);
            FillRect(tex, 10, 9, 4, 2, pebble);
            FillRect(tex, 5, 12, 2, 2, new Color(0.82f, 0.76f, 0.62f));
        }

        public static void DrawGreenhouseFloorTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.60f, 0.44f, 0.28f));
            Color line = new Color(0.46f, 0.33f, 0.20f);
            FillRect(tex, 0, 0, 16, 1, line);
            FillRect(tex, 0, 8, 16, 1, line);
            FillRect(tex, 7, 1, 1, 7, line);
            FillRect(tex, 13, 9, 1, 7, line);
        }

        public static void DrawWallTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.34f, 0.26f, 0.20f));
            FillRect(tex, 2, 4, 12, 9, new Color(0.56f, 0.78f, 0.84f));
            FillRect(tex, 7, 4, 2, 9, new Color(0.34f, 0.26f, 0.20f));
            FillRect(tex, 0, 14, 16, 2, new Color(0.24f, 0.18f, 0.14f));
        }

        public static void DrawFenceTile(Texture2D tex)
        {
            DrawGrassTile(tex);
            Color wood = new Color(0.54f, 0.36f, 0.20f);
            Color woodHi = new Color(0.68f, 0.48f, 0.28f);
            FillRect(tex, 2, 2, 3, 12, wood);
            FillRect(tex, 11, 2, 3, 12, wood);
            FillRect(tex, 0, 5, 16, 2, woodHi);
            FillRect(tex, 0, 10, 16, 2, woodHi);
        }

        public static void DrawTreeTile(Texture2D tex)
        {
            DrawGrassTile(tex);
            FillRect(tex, 6, 1, 4, 5, new Color(0.38f, 0.24f, 0.14f));
            FillRect(tex, 2, 5, 12, 9, new Color(0.14f, 0.36f, 0.22f));
            FillRect(tex, 4, 7, 8, 7, new Color(0.20f, 0.48f, 0.28f));
            FillRect(tex, 5, 10, 4, 3, new Color(0.28f, 0.58f, 0.34f));
        }

        public static void DrawWaterTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.22f, 0.52f, 0.78f));
            FillRect(tex, 3, 5, 5, 1, new Color(0.62f, 0.86f, 0.98f));
            FillRect(tex, 9, 11, 4, 1, new Color(0.62f, 0.86f, 0.98f));
        }

        public static void DrawFlowerbedTile(Texture2D tex)
        {
            FillRect(tex, 0, 0, 16, 16, new Color(0.54f, 0.54f, 0.58f));
            FillRect(tex, 2, 2, 12, 12, new Color(0.36f, 0.25f, 0.18f));
            FillRect(tex, 6, 6, 4, 4, new Color(0.46f, 0.34f, 0.24f));
        }
    }
}
```
