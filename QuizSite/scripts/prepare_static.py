#!/usr/bin/env python3
"""
构建题库并打包静态站点 → QuizSite/dist/

仅含刷题所需的前端与 JSON 数据，可上传到 GitHub Pages / Cloudflare Pages 等。
"""

from __future__ import annotations

import json
import shutil
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path

QUIZ = Path(__file__).resolve().parents[1]
DIST = QUIZ / "dist"
BUILD_SCRIPT = QUIZ / "scripts" / "build_all.py"
CONFIG_PATH = QUIZ / "deploy.config.json"
CONFIG_EXAMPLE = QUIZ / "deploy.config.json.example"

COPY_FILES = ("index.html", "import.html", "share.html")
COPY_DIRS = ("css", "js", "data")


def load_config() -> dict:
    if CONFIG_PATH.exists():
        return json.loads(CONFIG_PATH.read_text(encoding="utf-8"))
    if CONFIG_EXAMPLE.exists():
        return json.loads(CONFIG_EXAMPLE.read_text(encoding="utf-8"))
    return {}


def run_build() -> None:
    print("[1/2] 构建题库…")
    r = subprocess.run(
        [sys.executable, str(BUILD_SCRIPT)],
        cwd=str(QUIZ),
    )
    if r.returncode != 0:
        sys.exit(r.returncode)


def count_questions() -> int:
    manifest = json.loads((QUIZ / "data" / "manifest.json").read_text(encoding="utf-8"))
    exam = 0
    interview = 0
    for track in manifest.get("tracks", []):
        if track.get("id") == "interview":
            interview = track.get("count", 0)
        elif track.get("id") == "exam":
            weeks = track.get("modes", {}).get("week", {}).get("options", [])
            exam = sum(o.get("count", 0) for o in weeks)
    return exam + interview


def prepare() -> Path:
    run_build()

    print("[2/2] 打包静态文件…")
    if DIST.exists():
        shutil.rmtree(DIST)
    DIST.mkdir(parents=True)

    for name in COPY_FILES:
        src = QUIZ / name
        if src.exists():
            shutil.copy2(src, DIST / name)

    for name in COPY_DIRS:
        src = QUIZ / name
        if src.is_dir():
            shutil.copytree(src, DIST / name)

    imports_dst = DIST / "imports"
    imports_dst.mkdir(exist_ok=True)
    template = QUIZ / "imports" / "template.csv"
    if template.exists():
        shutil.copy2(template, imports_dst / "template.csv")

    cfg = load_config()
    site = {
        "mode": "static",
        "builtAt": datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        "publicUrl": (cfg.get("publicUrl") or "").strip().rstrip("/"),
    }
    (DIST / "site.json").write_text(
        json.dumps(site, ensure_ascii=False, indent=2),
        encoding="utf-8",
    )
    (DIST / ".nojekyll").touch()

    total = count_questions()
    print()
    print("=" * 56)
    print("  静态站点已生成")
    print("=" * 56)
    print(f"  目录:   {DIST}")
    print(f"  题目:   约 {total} 题（含双轨道）")
    if site["publicUrl"]:
        print(f"  永久链接: {site['publicUrl']}/")
    else:
        print("  提示: 在 deploy.config.json 填写 publicUrl 后重新打包")
    print("=" * 56)
    return DIST


def main():
    prepare()


if __name__ == "__main__":
    main()
