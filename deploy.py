# deploy.py — Moonpetal Valley — Step 5.fix 2
import os
import sys
import shutil
from pathlib import Path
from datetime import datetime

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

PROJECT_ROOT = Path.cwd()
ASSETS_DIR = PROJECT_ROOT / "Assets"
SETTINGS_DIR = PROJECT_ROOT / "ProjectSettings"

if not ASSETS_DIR.is_dir() or not SETTINGS_DIR.is_dir():
    print("[LỖI] Không tìm thấy thư mục 'Assets/' và 'ProjectSettings/' tại thư mục hiện tại.")
    sys.exit(1)

FILES = {
    "Assets/Scripts/UI/StatsHUDView.cs": r'''using UnityEngine;
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
''',
}

def deploy():
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_root = PROJECT_ROOT / "_deploy_backup" / f"Step_5_fix_2_{timestamp}"

    print("=== MOONPETAL VALLEY — DEPLOY STEP 5.FIX 2 ===")
    for rel_path, content in FILES.items():
        target_path = PROJECT_ROOT / rel_path
        target_path.parent.mkdir(parents=True, exist_ok=True)
        if target_path.exists():
            backup_target = backup_root / rel_path
            backup_target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(target_path, backup_target)

        normalized = content.replace("\r\n", "\n")
        with open(target_path, "w", encoding="utf-8", newline="\n") as f:
            f.write(normalized)
        print(f"  [GHI ĐÈ ] {rel_path} ({target_path.stat().st_size} bytes)")

    print("\n--- HƯỚNG DẪN TIẾP THEO ---")
    print("1. Quay lại Unity Editor (tắt Play Mode) và chờ compile xong.")
    print("2. Không cần chạy lại AutoBuilder — bấm Play trực tiếp và test Ép Kiệt Sức để xác nhận 0 warning!")
    print("3. Kiểm tra Hotbar 8 ô, Túi Đồ (Tab) và ăn Bánh Mì Nắng Ấm.")

if __name__ == "__main__":
    deploy()