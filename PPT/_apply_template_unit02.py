#!/usr/bin/env python3
"""Step 1 for unit 2 — same flow as _apply_template.py (unit 1)."""

from __future__ import annotations

import shutil
import sys
from pathlib import Path

PPT_DIR = Path(r"d:\Project\UnityProject\Major4_week2_Text\PPT")
ORIG = PPT_DIR / "TextPPT" / "02-第二单元常用控件（一）（原版）.pptx"
OUTPUT = PPT_DIR / "TextPPT" / "02-第二单元常用控件（一）.pptx"
STD = PPT_DIR / "标准.pptx"
TEMP = PPT_DIR / "_new_02-第二单元常用控件（一）.pptx"

sys.path.insert(0, str(PPT_DIR))
from _apply_template import apply_template_to  # noqa: E402


def main() -> None:
    if not ORIG.exists():
        raise FileNotFoundError(ORIG)
    if not STD.exists():
        raise FileNotFoundError(STD)
    if TEMP.exists():
        TEMP.unlink()

    slide_count = apply_template_to(ORIG, TEMP, STD)
    try:
        if OUTPUT.exists():
            OUTPUT.unlink()
        shutil.move(str(TEMP), str(OUTPUT))
        print(f"Step 1 done: {OUTPUT.name} ({slide_count} slides)")
    except PermissionError:
        print(f"Target open in WPS. Saved: {TEMP.name} ({slide_count} slides)")


if __name__ == "__main__":
    main()
