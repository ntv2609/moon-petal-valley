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
            MoonSickleSlash,
            Combo2Reverse,
            Combo3Spin,
            DashStrike
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

        [Header("Tool Use - Gieo hạt / Thu hoạch / Tạp vụ (4 hướng)")]
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

        [Header("Moon Sickle Slash - Combo 1 (4 hướng)")]
        [SerializeField] private Sprite[] slashSouth;
        [SerializeField] private Sprite[] slashNorth;
        [SerializeField] private Sprite[] slashEast;
        [SerializeField] private Sprite[] slashWest;

        [Header("Combo 2 - Nguyệt Ngược (4 hướng)")]
        [SerializeField] private Sprite[] combo2South;
        [SerializeField] private Sprite[] combo2North;
        [SerializeField] private Sprite[] combo2East;
        [SerializeField] private Sprite[] combo2West;

        [Header("Combo 3 - Hoa Trăng Xoay 360 (4 hướng)")]
        [SerializeField] private Sprite[] combo3South;
        [SerializeField] private Sprite[] combo3North;
        [SerializeField] private Sprite[] combo3East;
        [SerializeField] private Sprite[] combo3West;

        [Header("Dash-Strike - Lướt Chém (4 hướng)")]
        [SerializeField] private Sprite[] dashStrikeSouth;
        [SerializeField] private Sprite[] dashStrikeNorth;
        [SerializeField] private Sprite[] dashStrikeEast;
        [SerializeField] private Sprite[] dashStrikeWest;

        [Header("Tốc độ khung hình (FPS)")]
        [SerializeField] private float idleFps = 5f;
        [SerializeField] private float walkFps = 10f;
        [SerializeField] private float dashFps = 18f;
        [SerializeField] private float toolFps = 11f;
        [SerializeField] private float hoeFps = 11f;
        [SerializeField] private float waterFps = 10f;
        [SerializeField] private float hatchetFps = 12f;
        [SerializeField] private float slashFps = 14f;
        [SerializeField] private float combo2Fps = 16f;
        [SerializeField] private float combo3Fps = 16f;
        [SerializeField] private float dashStrikeFps = 18f;

        private Facing4 _facing = Facing4.South;
        private AnimAction _currentAction = AnimAction.Idle;
        private bool _isOneShotLocked;
        private bool _cutsceneOverride;
        private int _frameIndex;
        private float _frameTimer;

        public bool HasValidSprites => idleSouth != null && idleSouth.Length > 0 && idleSouth[0] != null;
        public bool IsCutsceneOverride => _cutsceneOverride;
        public SpriteRenderer SpriteRendererRef => spriteRenderer;

        public Sprite GetCurrentSprite()
        {
            return spriteRenderer != null ? spriteRenderer.sprite : null;
        }

        private void Update()
        {
            if (_cutsceneOverride && HasValidSprites && spriteRenderer != null)
            {
                TickAnimation(Time.unscaledDeltaTime);
            }
        }

        public void BeginCutsceneWalk(Vector2 facingDir)
        {
            if (!HasValidSprites || spriteRenderer == null) return;
            _cutsceneOverride = true;
            _isOneShotLocked = false;
            UpdateFacingFromVector(facingDir);
            SwitchActionIfNeeded(AnimAction.Walk);
        }

        /// <summary>
        /// Diễn hoạt tạp vụ dùng chung (kéo ván, thu hoạch, nhặt, sửa chữa, chèo đò...) lặp mượt trong cutscene.
        /// </summary>
        public void BeginCutsceneChore(Vector2 facingDir)
        {
            if (!HasValidSprites || spriteRenderer == null) return;
            _cutsceneOverride = true;
            _isOneShotLocked = false;
            UpdateFacingFromVector(facingDir);
            SwitchActionIfNeeded(AnimAction.ToolUse);
        }

        public void BeginCutsceneIdle(Vector2 facingDir)
        {
            if (!HasValidSprites || spriteRenderer == null) return;
            _cutsceneOverride = true;
            _isOneShotLocked = false;
            UpdateFacingFromVector(facingDir);
            SwitchActionIfNeeded(AnimAction.Idle);
        }

        public void EndCutsceneOverride()
        {
            _cutsceneOverride = false;
            _isOneShotLocked = false;
            SwitchActionIfNeeded(AnimAction.Idle);
        }

        public void PlayGenericChore(Vector2 facingDir)
        {
            PlayToolAction(WenToolAnimType.GenericSeedHarvest, facingDir);
        }

        public void UpdateLocomotion(Vector2 moveInput, Vector2 facingDir, bool isDashing)
        {
            if (!HasValidSprites || spriteRenderer == null) return;
            if (_cutsceneOverride) return;

            spriteRenderer.flipX = false;

            if (isDashing && !(_isOneShotLocked && _currentAction == AnimAction.DashStrike))
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

        public void PlayComboSlash(int comboStep, Vector2 facingDir)
        {
            if (!HasValidSprites || spriteRenderer == null) return;
            UpdateFacingFromVector(facingDir);

            AnimAction act = comboStep switch
            {
                2 => AnimAction.Combo2Reverse,
                3 => AnimAction.Combo3Spin,
                _ => AnimAction.MoonSickleSlash
            };
            StartOneShot(act);
        }

        public void PlayDashStrike(Vector2 facingDir)
        {
            if (!HasValidSprites || spriteRenderer == null) return;
            UpdateFacingFromVector(facingDir);
            StartOneShot(AnimAction.DashStrike);
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
                AnimAction.ToolUse => _cutsceneOverride ? 7.5f : toolFps,
                AnimAction.Hoe => hoeFps,
                AnimAction.WateringCan => waterFps,
                AnimAction.Hatchet => hatchetFps,
                AnimAction.MoonSickleSlash => slashFps,
                AnimAction.Combo2Reverse => combo2Fps,
                AnimAction.Combo3Spin => combo3Fps,
                AnimAction.DashStrike => dashStrikeFps,
                _ => idleFps
            };
        }

        private Sprite[] PickDir(Facing4 dir, Sprite[] s, Sprite[] n, Sprite[] e, Sprite[] w, bool fallbackToSlash = false)
        {
            Sprite[] chosen = dir switch
            {
                Facing4.North => n,
                Facing4.East => e,
                Facing4.West => w,
                _ => s
            };
            if (chosen != null && chosen.Length > 0 && chosen[0] != null) return chosen;

            if (fallbackToSlash)
            {
                Sprite[] slashFallback = dir switch
                {
                    Facing4.North => slashNorth,
                    Facing4.East => slashEast,
                    Facing4.West => slashWest,
                    _ => slashSouth
                };
                if (slashFallback != null && slashFallback.Length > 0 && slashFallback[0] != null) return slashFallback;
            }

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
                AnimAction.Combo2Reverse => PickDir(dir, combo2South, combo2North, combo2East, combo2West, true),
                AnimAction.Combo3Spin => PickDir(dir, combo3South, combo3North, combo3East, combo3West, true),
                AnimAction.DashStrike => PickDir(dir, dashStrikeSouth, dashStrikeNorth, dashStrikeEast, dashStrikeWest, true),
                _ => PickDir(dir, idleSouth, idleNorth, idleEast, idleWest)
            };
        }
    }
}
