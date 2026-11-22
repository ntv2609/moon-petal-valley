#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Moonpetal Valley - Step 18.fix 2
Fine-tune GardenEntranceArch sortingOrder (190 -> 140) and post collider Y-offset
so standing flush against the south side of arch posts never clips behind them.
Python 3 stdlib only.
"""

import os
import shutil
import sys
from datetime import datetime

PROJECT_ROOT = os.getcwd()
REL_PATH = "Assets/Editor/AutoBuilder_Step18_MasterFix.cs"
BACKUP_DIR = os.path.join(
    PROJECT_ROOT,
    "_deploy_backup",
    f"Step_18_fix2_{datetime.now().strftime('%Y%m%d_%H%M%S')}",
)


def main() -> int:
    full_path = os.path.join(PROJECT_ROOT, REL_PATH)
    if not os.path.isfile(full_path):
        print(f"[Step 18.fix 2] LỖI: Không tìm thấy {REL_PATH}")
        return 1

    dst = os.path.join(BACKUP_DIR, REL_PATH)
    os.makedirs(os.path.dirname(dst), exist_ok=True)
    shutil.copy2(full_path, dst)

    with open(full_path, "r", encoding="utf-8") as f:
        content = f.read().replace("\r\n", "\n")

    content = content.replace("sr.sortingOrder = 190;", "sr.sortingOrder = 140;")
    content = content.replace(
        'EnsureArchPostCollider(arch.transform, "ArchPost_Left",  new Vector3(-1.55f, -0.95f, 0f), new Vector2(0.45f, 0.45f));',
        'EnsureArchPostCollider(arch.transform, "ArchPost_Left",  new Vector3(-1.55f, -1.08f, 0f), new Vector2(0.52f, 0.48f));',
    )
    content = content.replace(
        'EnsureArchPostCollider(arch.transform, "ArchPost_Right", new Vector3( 1.55f, -0.95f, 0f), new Vector2(0.45f, 0.45f));',
        'EnsureArchPostCollider(arch.transform, "ArchPost_Right", new Vector3( 1.55f, -1.08f, 0f), new Vector2(0.52f, 0.48f));',
    )

    with open(full_path, "w", encoding="utf-8", newline="\n") as f:
        f.write(content)

    print(f"[Step 18.fix 2] Đã tinh chỉnh sortingOrder=140 và vị trí collider 2 cọc cổng trong {REL_PATH}.")
    print("Vào Unity chạy lại: Moonpetal -> Step 18 -> 2. Master Fix: Colliders, Arch Posts, Overlap & Unify Decor")
    return 0


if __name__ == "__main__":
    sys.exit(main())