#!/usr/bin/env python3
"""打包 dist 为 zip，便于上传到 Gitee / 网盘 / 托管平台。"""

from __future__ import annotations

import shutil
import subprocess
import sys
import zipfile
from pathlib import Path

QUIZ = Path(__file__).resolve().parents[1]
DIST = QUIZ / "dist"
ZIP_PATH = QUIZ / "quiz-site-upload.zip"
PREPARE = QUIZ / "scripts" / "prepare_static.py"


def ensure_dist() -> None:
    if DIST.exists() and (DIST / "index.html").exists():
        return
    r = subprocess.run([sys.executable, str(PREPARE)], cwd=str(QUIZ))
    if r.returncode != 0:
        sys.exit(r.returncode)


def make_zip() -> Path:
    ensure_dist()
    if ZIP_PATH.exists():
        ZIP_PATH.unlink()
    with zipfile.ZipFile(ZIP_PATH, "w", zipfile.ZIP_DEFLATED) as zf:
        for path in sorted(DIST.rglob("*")):
            if path.is_file():
                zf.write(path, path.relative_to(DIST).as_posix())
    size_mb = ZIP_PATH.stat().st_size / (1024 * 1024)
    print(f"  ZIP: {ZIP_PATH}")
    print(f"  大小: {size_mb:.2f} MB")
    return ZIP_PATH


if __name__ == "__main__":
    make_zip()
