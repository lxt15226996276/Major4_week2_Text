#!/usr/bin/env python3
"""Step 2 for unit 2 — same flow as _remove_cover_logo.py (unit 1)."""

from __future__ import annotations

import shutil
import sys
from pathlib import Path

PPT_DIR = Path(r"d:\Project\UnityProject\Major4_week2_Text\PPT")

sys.path.insert(0, str(PPT_DIR))
from _remove_cover_logo import remove_cover_logo_from_pptx  # noqa: E402
from _unit02_common import OUTPUT, TEMP, resolve_pptx, try_finalize_output  # noqa: E402


def patch_cover_logo(pptx: Path) -> str:
    try:
        return remove_cover_logo_from_pptx(pptx)
    except PermissionError:
        alt = TEMP if pptx == OUTPUT else OUTPUT
        if alt.exists() and alt != pptx:
            alt.unlink()
        shutil.copy2(pptx, alt)
        media = remove_cover_logo_from_pptx(alt)
        print(f"Target locked in WPS. Patched copy: {alt.name}")
        return media


def main() -> None:
    target = resolve_pptx()
    media = patch_cover_logo(target)
    try_finalize_output(target)
    print(f"Step 2 done: removed cover logo from {media} in {target.name}")


if __name__ == "__main__":
    main()
