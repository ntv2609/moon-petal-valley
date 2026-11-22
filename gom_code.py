# gom_code.py — Gom toàn bộ code C# dự án Moonpetal Valley thành 1 file Markdown
import sys
from pathlib import Path
from datetime import datetime

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

PROJECT_ROOT = Path.cwd()
TARGET_DIRS = [
    PROJECT_ROOT / "Assets" / "Scripts",
    PROJECT_ROOT / "Assets" / "Editor",
]
OUTPUT_FILE = PROJECT_ROOT / "tat_ca_code_moonpetal.md"


def collect_cs_files():
    files = []
    for base_dir in TARGET_DIRS:
        if base_dir.is_dir():
            for path in sorted(base_dir.rglob("*.cs")):
                files.append(path)
    return files


def main():
    if not (PROJECT_ROOT / "Assets").is_dir():
        print("[LỖI] Không tìm thấy thư mục 'Assets/'. Hãy chạy script tại thư mục gốc dự án Unity.")
        sys.exit(1)

    cs_files = collect_cs_files()
    if not cs_files:
        print("[CẢNH BÁO] Không tìm thấy file .cs nào trong Assets/Scripts hoặc Assets/Editor.")
        sys.exit(0)

    total_lines = 0
    sections = []
    toc_lines = []

    for idx, file_path in enumerate(cs_files, start=1):
        rel = file_path.relative_to(PROJECT_ROOT).as_posix()
        content = file_path.read_text(encoding="utf-8", errors="replace").replace("\r\n", "\n").rstrip()
        line_count = content.count("\n") + 1 if content else 0
        total_lines += line_count

        toc_lines.append(f"{idx}. `{rel}` ({line_count} dòng)")
        sections.append(f"## {idx}. `{rel}`\n\n```csharp\n{content}\n```\n")

    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    header = (
        f"# MOONPETAL VALLEY — TỔNG HỢP MÃ NGUỒN C#\n\n"
        f"- **Thời điểm xuất:** {timestamp}\n"
        f"- **Tổng số file:** {len(cs_files)}\n"
        f"- **Tổng số dòng:** {total_lines}\n\n"
        f"## Danh sách file\n\n"
        + "\n".join(toc_lines)
        + "\n\n"
    )

    OUTPUT_FILE.write_text(header + "\n".join(sections), encoding="utf-8", newline="\n")
    size_kb = OUTPUT_FILE.stat().st_size / 1024
    print(f"[OK] Đã gom {len(cs_files)} file ({total_lines} dòng) -> {OUTPUT_FILE.name} ({size_kb:.1f} KB)")


if __name__ == "__main__":
    main()