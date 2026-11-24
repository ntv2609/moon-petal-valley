using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Combat;
using MoonpetalValley.Player;

namespace MoonpetalValley.Chapter2
{
    public class SwampWaveCombatController : MonoBehaviour
    {
        private enum EnemyKind { GloomSlime = 0, ThornCrawler = 1, BigGloomSlime = 2 }

        private class ActiveEnemyTrack
        {
            public GameObject go;
            public IDamageable dmg;
            public EnemyKind kind;
            public Vector3 lastPos;
        }

        [Header("Story & Reward")]
        [SerializeField] private string zoneClearedFlag = "flag_ch2_swamp_cleared";
        [SerializeField] private ItemData westLanternRewardItem;

        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject thornCrawlerPrefab;
        [SerializeField] private GameObject gloomSlimePrefab;
        [SerializeField] private GameObject bigGloomSlimePrefab;

        [Header("Full-Map Random Spawn Bounds")]
        [SerializeField] private Vector2 arenaMin = new Vector2(-16.5f, -7.2f);
        [SerializeField] private Vector2 arenaMax = new Vector2(15.5f, 8.2f);
        [SerializeField] private float triggerEnterMinY = -6.2f;
        [SerializeField] private float minDistFromPlayer = 4.6f;
        [SerializeField] private float maxDistFromPlayer = 11.2f;

        [Header("Visual & HUD Assets")]
        [SerializeField] private Sprite orbSprite;
        [SerializeField] private TMP_FontAsset hudFont;

        private readonly List<ActiveEnemyTrack> _activeEnemies = new List<ActiveEnemyTrack>();
        private readonly Queue<EnemyKind> _spawnQueue = new Queue<EnemyKind>();

        private int _currentWave;
        private int _waveTotalCount;
        private int _waveDefeatedCount;
        private int _maxConcurrent = 9;
        private float _spawnInterval = 0.85f;
        private float _spawnTimer;
        private bool _combatStarted;
        private bool _zoneCleared;
        private bool _transitioningWave;

        private Transform _playerTr;
        private Canvas _hudCanvas;
        private GameObject _hudBannerRoot;
        private TextMeshProUGUI _hudTitleText;
        private TextMeshProUGUI _hudDetailText;
        private Image _hudFillBar;

        private void Awake()
        {
            InstantRecoveryOrb.SetSharedAssets(orbSprite, hudFont);
        }

        private void OnEnable()
        {
            GameEventBus.OnSaveLoaded += SyncFromFlags;
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnSaveLoaded -= SyncFromFlags;
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void Start()
        {
            SyncFromFlags();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (flagId == zoneClearedFlag) SyncFromFlags();
        }

        public void SyncFromFlags()
        {
            StoryFlagManager flags = GameRoot.Instance != null ? GameRoot.Instance.Flags : null;
            _zoneCleared = flags != null && flags.HasFlag(zoneClearedFlag);

            if (_zoneCleared)
            {
                ClearAllSpawnedEnemies();
                _combatStarted = false;
                SetWaveHudVisible(false);
            }
            else
            {
                _combatStarted = false;
                _currentWave = 0;
                _transitioningWave = false;
                _spawnQueue.Clear();
                ClearAllSpawnedEnemies();
                SetWaveHudVisible(false);
            }
        }

        private void Update()
        {
            if (_zoneCleared) return;

            if (_playerTr == null)
            {
                PlayerController2D pc = Object.FindAnyObjectByType<PlayerController2D>();
                if (pc != null) _playerTr = pc.transform;
            }
            if (_playerTr == null) return;

            if (!_combatStarted)
            {
                // Khi bước qua đầu con đường vào bãi sương đầm lầy -> kích hoạt ngay trận chiến 70 quái!
                if (_playerTr.position.y >= triggerEnterMinY)
                {
                    BeginSwampGauntlet();
                }
                return;
            }

            PollActiveEnemies();

            if (_transitioningWave) return;

            if (_spawnQueue.Count > 0 && _activeEnemies.Count < _maxConcurrent)
            {
                _spawnTimer -= Time.deltaTime;
                if (_spawnTimer <= 0f)
                {
                    _spawnTimer = _spawnInterval;
                    EnemyKind nextKind = _spawnQueue.Dequeue();
                    Vector3 spawnPos = PickRandomWalkableSpawnPosition();
                    StartCoroutine(SpawnWithMistPuff(nextKind, spawnPos));
                }
            }

            if (_spawnQueue.Count == 0 && _activeEnemies.Count == 0 && _waveDefeatedCount >= _waveTotalCount && _waveTotalCount > 0)
            {
                HandleWaveCleared();
            }
        }

        private void BeginSwampGauntlet()
        {
            _combatStarted = true;
            BuildWaveHudIfNeeded();
            SetWaveHudVisible(true);
            GameEventBus.RaiseToastRequested("Bãi lầy rung chuyển! Hàng chục bóng sương đang trồi lên từ khắp mọi phía!");
            StartWave(1);
        }

        private void StartWave(int waveNum)
        {
            _currentWave = waveNum;
            _waveDefeatedCount = 0;
            _spawnQueue.Clear();
            _transitioningWave = false;

            if (waveNum == 1)
            {
                // Đợt 1: 18 quái (11 GloomSlime + 7 ThornCrawler)
                _maxConcurrent = 9;
                _spawnInterval = 0.85f;
                for (int i = 0; i < 6; i++)
                {
                    _spawnQueue.Enqueue(EnemyKind.GloomSlime);
                    _spawnQueue.Enqueue(EnemyKind.ThornCrawler);
                    if (i < 5) _spawnQueue.Enqueue(EnemyKind.GloomSlime);
                }
                _spawnQueue.Enqueue(EnemyKind.ThornCrawler);
            }
            else if (waveNum == 2)
            {
                // Đợt 2: 24 quái (12 ThornCrawler + 12 GloomSlime)
                _maxConcurrent = 12;
                _spawnInterval = 0.72f;
                for (int i = 0; i < 12; i++)
                {
                    _spawnQueue.Enqueue(i % 2 == 0 ? EnemyKind.ThornCrawler : EnemyKind.GloomSlime);
                    _spawnQueue.Enqueue(i % 2 == 0 ? EnemyKind.GloomSlime : EnemyKind.ThornCrawler);
                }
            }
            else
            {
                // Đợt 3: 28 quái (2 BigGloomSlime Thủ Lĩnh + 14 ThornCrawler + 12 GloomSlime)
                _maxConcurrent = 12;
                _spawnInterval = 0.66f;
                _spawnQueue.Enqueue(EnemyKind.BigGloomSlime);
                for (int i = 0; i < 12; i++)
                {
                    _spawnQueue.Enqueue(EnemyKind.ThornCrawler);
                    _spawnQueue.Enqueue(EnemyKind.GloomSlime);
                    if (i == 6) _spawnQueue.Enqueue(EnemyKind.BigGloomSlime);
                }
                _spawnQueue.Enqueue(EnemyKind.ThornCrawler);
                _spawnQueue.Enqueue(EnemyKind.ThornCrawler);
            }

            _waveTotalCount = _spawnQueue.Count;
            _spawnTimer = 0.2f;
            UpdateWaveHudText();

            // Mở màn đợt mới bằng 4 con trồi lên tức thì từ nhiều hướng
            int openingBurst = Mathf.Min(4, _spawnQueue.Count);
            for (int i = 0; i < openingBurst; i++)
            {
                EnemyKind kind = _spawnQueue.Dequeue();
                StartCoroutine(SpawnWithMistPuff(kind, PickRandomWalkableSpawnPosition()));
            }
        }

        private Vector3 PickRandomWalkableSpawnPosition()
        {
            Vector3 playerPos = _playerTr != null ? _playerTr.position : Vector3.zero;

            for (int attempt = 0; attempt < 36; attempt++)
            {
                float x = Random.Range(arenaMin.x, arenaMax.x);
                float y = Random.Range(arenaMin.y, arenaMax.y);
                Vector2 candidate = new Vector2(x, y);

                // Tránh 2 ao nước đầm lầy Đông & Tây
                if (((x - 13f) * (x - 13f) / 17f + (y - 3f) * (y - 3f) / 13f) <= 1.0f) continue;
                if (((x + 15f) * (x + 15f) / 14f + (y + 6f) * (y + 6f) / 12f) <= 1.0f) continue;

                float dist = Vector2.Distance(candidate, playerPos);
                float maxAllowed = attempt < 24 ? maxDistFromPlayer : 17.5f;
                if (dist < minDistFromPlayer || dist > maxAllowed) continue;

                // Tránh vật cản rắn (TilemapCollider2D / cây / đá)
                bool blocked = false;
                var hits = Physics2D.OverlapCircleAll(candidate, 0.52f);
                for (int h = 0; h < hits.Length; h++)
                {
                    if (hits[h] != null && !hits[h].isTrigger)
                    {
                        blocked = true;
                        break;
                    }
                }
                if (blocked) continue;

                return new Vector3(x, y, 0f);
            }

            // Fallback an toàn quanh người chơi trong vùng trung tâm
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float fx = Mathf.Clamp(playerPos.x + Mathf.Cos(angle) * 5.8f, arenaMin.x + 1f, arenaMax.x - 1f);
            float fy = Mathf.Clamp(playerPos.y + Mathf.Sin(angle) * 4.8f, arenaMin.y + 1f, arenaMax.y - 1f);
            return new Vector3(fx, fy, 0f);
        }

        private IEnumerator SpawnWithMistPuff(EnemyKind kind, Vector3 pos)
        {
            // Khói tím gọn tan nhanh 0.42s báo hiệu quái trồi lên từ bùn lầy (không dùng vòng cố định)
            GameObject puff = new GameObject("SwampEmergeMist");
            puff.transform.position = pos;
            var sr = puff.AddComponent<SpriteRenderer>();
            sr.sprite = orbSprite;
            sr.color = kind == EnemyKind.BigGloomSlime
                ? new Color(0.95f, 0.30f, 0.85f, 0.85f)
                : new Color(0.56f, 0.34f, 0.86f, 0.72f);
            sr.sortingLayerName = "WorldObjects";
            sr.sortingOrder = 165;

            float t = 0f;
            float dur = 0.42f;
            float targetScale = kind == EnemyKind.BigGloomSlime ? 1.85f : 1.15f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float p = t / dur;
                puff.transform.localScale = Vector3.one * Mathf.Lerp(0.25f, targetScale, p);
                Color c = sr.color;
                c.a = (1f - p * 0.7f) * 0.78f;
                sr.color = c;
                yield return null;
            }
            Destroy(puff);

            if (_zoneCleared) yield break;

            GameObject prefab = kind == EnemyKind.BigGloomSlime ? bigGloomSlimePrefab
                : (kind == EnemyKind.ThornCrawler ? thornCrawlerPrefab : gloomSlimePrefab);
            if (prefab == null) prefab = gloomSlimePrefab != null ? gloomSlimePrefab : thornCrawlerPrefab;
            if (prefab == null) yield break;

            GameObject inst = Instantiate(prefab, pos, Quaternion.identity);
            IDamageable dmg = inst.GetComponent<IDamageable>();
            _activeEnemies.Add(new ActiveEnemyTrack
            {
                go = inst,
                dmg = dmg,
                kind = kind,
                lastPos = pos
            });
        }

        private void PollActiveEnemies()
        {
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var tr = _activeEnemies[i];
                if (tr.go != null) tr.lastPos = tr.go.transform.position;

                bool dead = tr.go == null || !tr.go.activeInHierarchy || (tr.dmg != null && tr.dmg.IsDefeated);
                if (dead)
                {
                    _activeEnemies.RemoveAt(i);
                    _waveDefeatedCount++;
                    bool isBoss = tr.kind == EnemyKind.BigGloomSlime;
                    InstantRecoveryOrb.TrySpawnRandomDrop(tr.lastPos, isBoss ? 1f : 0.42f, isBoss);
                    UpdateWaveHudText();
                }
            }
        }

        private void HandleWaveCleared()
        {
            if (_currentWave < 3)
            {
                _transitioningWave = true;
                StartCoroutine(NextWaveCountdownRoutine(_currentWave + 1));
            }
            else
            {
                CompleteSwampGauntlet();
            }
        }

        private IEnumerator NextWaveCountdownRoutine(int nextWave)
        {
            string waveName = nextWave == 2 ? "Đợt 2: Vây Hãm Bãi Lầy (24 quái)!" : "Đợt 3: Cơn Thịnh Nộ Đầm Lầy (28 quái + 2 Thủ Lĩnh)!";
            GameEventBus.RaiseToastRequested($"Xong Đợt {_currentWave}! Chuẩn bị {waveName}");
            UpdateWaveHudText($"CHUẨN BỊ ĐỢT {nextWave}/3", "Thu nhặt Ngọc Ánh Sáng & sẵn sàng!");
            yield return new WaitForSeconds(2.4f);
            if (!_zoneCleared) StartWave(nextWave);
        }

        private void CompleteSwampGauntlet()
        {
            _zoneCleared = true;
            _combatStarted = false;
            ClearAllSpawnedEnemies();

            if (GameRoot.Instance != null && GameRoot.Instance.Inventory != null)
            {
                if (westLanternRewardItem != null)
                    GameRoot.Instance.Inventory.AddItem(westLanternRewardItem, 1, true);
                else
                    GameRoot.Instance.Inventory.AddItemById("item_storm_lantern_west", 1, true);
            }

            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null)
                GameRoot.Instance.Flags.SetFlag(zoneClearedFlag, true);

            if (GameRoot.Instance != null && GameRoot.Instance.Audio != null)
                GameRoot.Instance.Audio.PlayUIClick();

            UpdateWaveHudText("ĐẦM LẦY ĐÃ ĐƯỢC THANH TẨY!", "Đã nhận Chao Kính Đèn Bão -> Tiến lên Miếu Đèn Cổ (Tây-Bắc) bấm [E] soi Dư Âm 1!");
            GameEventBus.RaiseToastRequested("Thanh tẩy toàn bộ 70 bóng sương! Bạn đã thu được Chao Kính Đèn Bão Cổ!");
            StartCoroutine(HideHudDelayed(6.0f));
        }

        private IEnumerator HideHudDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetWaveHudVisible(false);
        }

        private void ClearAllSpawnedEnemies()
        {
            for (int i = 0; i < _activeEnemies.Count; i++)
            {
                if (_activeEnemies[i].go != null) Destroy(_activeEnemies[i].go);
            }
            _activeEnemies.Clear();
        }

        private void BuildWaveHudIfNeeded()
        {
            if (_hudBannerRoot != null) return;

            GameObject cvGo = new GameObject("SwampWaveHUDCanvas");
            _hudCanvas = cvGo.AddComponent<Canvas>();
            _hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _hudCanvas.sortingOrder = 140;
            CanvasScaler cs = cvGo.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);

            _hudBannerRoot = new GameObject("WaveBanner", typeof(RectTransform), typeof(Image));
            _hudBannerRoot.transform.SetParent(cvGo.transform, false);
            RectTransform brt = _hudBannerRoot.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 1f);
            brt.anchorMax = new Vector2(0.5f, 1f);
            brt.pivot = new Vector2(0.5f, 1f);
            brt.anchoredPosition = new Vector2(0f, -18f);
            brt.sizeDelta = new Vector2(560f, 88f);
            _hudBannerRoot.GetComponent<Image>().color = new Color(0.08f, 0.10f, 0.16f, 0.88f);

            TMP_FontAsset fnt = hudFont != null ? hudFont : TMP_Settings.defaultFontAsset;

            GameObject tGo = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            tGo.transform.SetParent(_hudBannerRoot.transform, false);
            RectTransform trt = tGo.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0f, 0.56f);
            trt.anchorMax = new Vector2(1f, 0.96f);
            trt.offsetMin = new Vector2(14f, 0f);
            trt.offsetMax = new Vector2(-14f, 0f);
            _hudTitleText = tGo.GetComponent<TextMeshProUGUI>();
            _hudTitleText.font = fnt;
            _hudTitleText.fontSize = 22f;
            _hudTitleText.color = new Color(1f, 0.88f, 0.48f, 1f);
            _hudTitleText.alignment = TextAlignmentOptions.Center;

            GameObject dGo = new GameObject("Detail", typeof(RectTransform), typeof(TextMeshProUGUI));
            dGo.transform.SetParent(_hudBannerRoot.transform, false);
            RectTransform drt = dGo.GetComponent<RectTransform>();
            drt.anchorMin = new Vector2(0f, 0.24f);
            drt.anchorMax = new Vector2(1f, 0.58f);
            drt.offsetMin = new Vector2(14f, 0f);
            drt.offsetMax = new Vector2(-14f, 0f);
            _hudDetailText = dGo.GetComponent<TextMeshProUGUI>();
            _hudDetailText.font = fnt;
            _hudDetailText.fontSize = 16f;
            _hudDetailText.color = new Color(0.88f, 0.94f, 1f, 1f);
            _hudDetailText.alignment = TextAlignmentOptions.Center;

            GameObject bgBar = new GameObject("BarBG", typeof(RectTransform), typeof(Image));
            bgBar.transform.SetParent(_hudBannerRoot.transform, false);
            RectTransform bgrt = bgBar.GetComponent<RectTransform>();
            bgrt.anchorMin = new Vector2(0.06f, 0.08f);
            bgrt.anchorMax = new Vector2(0.94f, 0.20f);
            bgrt.offsetMin = Vector2.zero;
            bgrt.offsetMax = Vector2.zero;
            bgBar.GetComponent<Image>().color = new Color(0.16f, 0.20f, 0.28f, 0.95f);

            GameObject fillBar = new GameObject("BarFill", typeof(RectTransform), typeof(Image));
            fillBar.transform.SetParent(bgBar.transform, false);
            RectTransform frt = fillBar.GetComponent<RectTransform>();
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = Vector2.zero;
            frt.offsetMax = Vector2.zero;
            _hudFillBar = fillBar.GetComponent<Image>();
            _hudFillBar.color = new Color(0.62f, 0.36f, 0.95f, 1f);
            _hudFillBar.type = Image.Type.Filled;
            _hudFillBar.fillMethod = Image.FillMethod.Horizontal;
        }

        private void SetWaveHudVisible(bool visible)
        {
            if (_hudBannerRoot != null) _hudBannerRoot.SetActive(visible);
        }

        private void UpdateWaveHudText(string customTitle = null, string customDetail = null)
        {
            if (_hudBannerRoot == null) return;
            if (!string.IsNullOrEmpty(customTitle))
            {
                _hudTitleText.text = customTitle;
                _hudDetailText.text = customDetail ?? "";
                if (_hudFillBar != null) _hudFillBar.fillAmount = 1f;
                return;
            }

            string waveTitle = _currentWave == 1 ? "ĐỢT 1/3 - SƯƠNG THỨC GIẤC"
                : (_currentWave == 2 ? "ĐỢT 2/3 - VÂY HÃM BÃI LẦY" : "ĐỢT 3/3 - CƠN THỊNH NỘ ĐẦM LẦY");
            int remain = Mathf.Max(0, _waveTotalCount - _waveDefeatedCount);
            _hudTitleText.text = waveTitle;
            _hudDetailText.text = $"Đã hạ: {_waveDefeatedCount}/{_waveTotalCount} | Còn lại: {remain} | Đang trên sân: {_activeEnemies.Count}";
            if (_hudFillBar != null)
                _hudFillBar.fillAmount = _waveTotalCount > 0 ? (float)_waveDefeatedCount / _waveTotalCount : 0f;
        }
    }
}
