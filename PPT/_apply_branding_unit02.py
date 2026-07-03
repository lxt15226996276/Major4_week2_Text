#!/usr/bin/env python3
"""Step 3 for unit 2 — same flow as _apply_branding_inplace.py (unit 1)."""

from __future__ import annotations

import shutil
import sys
from pathlib import Path

PPT_DIR = Path(r"d:\Project\UnityProject\Major4_week2_Text\PPT")

sys.path.insert(0, str(PPT_DIR))
from _apply_branding_inplace import apply_branding  # noqa: E402
from _unit02_common import OUTPUT, STD, TEMP, resolve_pptx, try_finalize_output  # noqa: E402


def patch_branding(pptx: Path) -> tuple[int, int]:
    try:
        return apply_branding(pptx, STD)
    except PermissionError:
        alt = TEMP if pptx == OUTPUT else OUTPUT
        if alt.exists() and alt != pptx:
            alt.unlink()
        shutil.copy2(pptx, alt)
        result = apply_branding(alt, STD)
        print(f"Target locked in WPS. Patched copy: {alt.name}")
        return result


def main() -> None:
    if not STD.exists():
        raise FileNotFoundError(STD)
    target = resolve_pptx()
    text_n, media_n = patch_branding(target)
    try_finalize_output(target)
    print(f"Step 3 done: {target.name}")
    print(f"  XML text replacements: {text_n}")
    print(f"  Logo media replaced: {media_n}")


if __name__ == "__main__":
    main()
