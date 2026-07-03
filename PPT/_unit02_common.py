#!/usr/bin/env python3
"""Shared paths and WPS-open-safe target resolution for unit 2 pipeline."""

from __future__ import annotations

import shutil
from pathlib import Path

PPT_DIR = Path(r"d:\Project\UnityProject\Major4_week2_Text\PPT")
ORIG = PPT_DIR / "TextPPT" / "02-第二单元常用控件（一）（原版）.pptx"
OUTPUT = PPT_DIR / "TextPPT" / "02-第二单元常用控件（一）.pptx"
TEMP = PPT_DIR / "_new_02-第二单元常用控件（一）.pptx"
STD = PPT_DIR / "标准.pptx"


def resolve_pptx() -> Path:
    """Pick the pptx to patch when WPS may keep the target file open."""
    if TEMP.exists() and OUTPUT.exists():
        return TEMP if TEMP.stat().st_mtime >= OUTPUT.stat().st_mtime else OUTPUT
    if TEMP.exists():
        return TEMP
    if OUTPUT.exists():
        return OUTPUT
    raise FileNotFoundError(f"Neither {OUTPUT.name} nor {TEMP.name} exists. Run step 1 first.")


def try_finalize_output(from_path: Path) -> None:
    if from_path != TEMP or not TEMP.exists():
        return
    try:
        if OUTPUT.exists():
            OUTPUT.unlink()
        shutil.move(str(TEMP), str(OUTPUT))
        print(f"Replaced target: {OUTPUT.name}")
    except PermissionError:
        print(f"WPS still has {OUTPUT.name} open. Using {TEMP.name} — replace manually when ready.")
