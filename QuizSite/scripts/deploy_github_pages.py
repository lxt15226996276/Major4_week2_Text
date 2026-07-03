#!/usr/bin/env python3
"""
将 QuizSite/dist 部署到 GitHub Pages（gh-pages 分支）。

前提：已安装 Git，且本仓库已关联 GitHub remote（origin）。
首次使用需在 GitHub 仓库 Settings → Pages 选择 Deploy from branch → gh-pages / root。
"""

from __future__ import annotations

import json
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

QUIZ = Path(__file__).resolve().parents[1]
DIST = QUIZ / "dist"
PREPARE = QUIZ / "scripts" / "prepare_static.py"
CONFIG_PATH = QUIZ / "deploy.config.json"


def run(cmd: list[str], cwd: Path | None = None, check: bool = True) -> subprocess.CompletedProcess:
    return subprocess.run(cmd, cwd=str(cwd) if cwd else None, check=check, text=True)


def find_git_root(start: Path) -> Path | None:
    p = start.resolve()
    while True:
        if (p / ".git").exists():
            return p
        if p.parent == p:
            return None
        p = p.parent


def load_config() -> dict:
    if CONFIG_PATH.exists():
        return json.loads(CONFIG_PATH.read_text(encoding="utf-8"))
    return {}


def get_remote_url(repo_root: Path, remote: str = "origin") -> str:
    r = run(["git", "remote", "get-url", remote], cwd=repo_root, check=False)
    if r.returncode != 0 or not r.stdout.strip():
        raise RuntimeError(f"未找到 git remote「{remote}」。请先 git remote add origin <你的仓库地址>")
    return r.stdout.strip()


def guess_pages_url(remote_url: str) -> str:
    """从 origin URL 推测 GitHub Pages 地址（供提示用）。"""
    url = remote_url.rstrip("/")
    if url.endswith(".git"):
        url = url[:-4]
    if "github.com" not in url:
        return ""
    parts = url.replace("git@github.com:", "https://github.com/").split("github.com/")
    if len(parts) < 2:
        return ""
    path = parts[-1].strip("/")
    segs = path.split("/")
    if len(segs) >= 2:
        user, repo = segs[0], segs[1]
        return f"https://{user}.github.io/{repo}"
    return ""


def ensure_dist() -> None:
    if DIST.exists() and (DIST / "index.html").exists():
        return
    print("dist 不存在，先执行打包…")
    r = subprocess.run([sys.executable, str(PREPARE)], cwd=str(QUIZ))
    if r.returncode != 0:
        sys.exit(r.returncode)


def deploy() -> int:
    cfg = load_config()
    branch = cfg.get("githubBranch") or "gh-pages"
    remote = cfg.get("githubRemote") or "origin"
    commit_msg = cfg.get("commitMessage") or "Deploy quiz site"

    ensure_dist()
    repo_root = find_git_root(QUIZ)
    if not repo_root:
        print("错误：当前不在 Git 仓库内。请先把项目 push 到 GitHub，或用手动上传 dist 文件夹。")
        print("详见 QuizSite/DEPLOY.md")
        return 1

    try:
        remote_url = get_remote_url(repo_root, remote)
    except RuntimeError as e:
        print(f"错误：{e}")
        return 1

    print()
    print(f"  目标仓库: {remote_url}")
    print(f"  部署分支: {branch}")
    print()

    with tempfile.TemporaryDirectory(prefix="quiz-deploy-") as tmp:
        tmp_path = Path(tmp)
        shutil.copytree(DIST, tmp_path, dirs_exist_ok=True)

        run(["git", "init"], cwd=tmp_path)
        run(["git", "checkout", "-b", branch], cwd=tmp_path)
        run(["git", "add", "-A"], cwd=tmp_path)
        run(["git", "commit", "-m", commit_msg], cwd=tmp_path)
        run(["git", "remote", "add", "origin", remote_url], cwd=tmp_path)
        print("  正在推送到 GitHub…")
        run(["git", "push", "-f", "origin", branch], cwd=tmp_path)

    pages_url = (cfg.get("publicUrl") or "").strip().rstrip("/") or guess_pages_url(remote_url)
    print()
    print("=" * 60)
    print("  部署完成")
    print("=" * 60)
    if pages_url:
        print(f"  永久链接（启用 Pages 后）: {pages_url}/")
        print(f"  分享页: {pages_url}/share.html")
    print()
    print("  若链接尚不可用，请到 GitHub 仓库：")
    print("  Settings → Pages → Source: Deploy from branch")
    print(f"  Branch: {branch} / (root)")
    print()
    print("  建议把 publicUrl 写入 deploy.config.json，下次打包会写入 site.json。")
    print("=" * 60)
    return 0


def main():
    sys.exit(deploy())


if __name__ == "__main__":
    main()
