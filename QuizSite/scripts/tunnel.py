#!/usr/bin/env python3
"""公网隧道：让不同 WiFi / 不同城市的人也能访问本地刷题站。"""

from __future__ import annotations

import platform
import re
import shutil
import subprocess
import sys
import threading
import time
import urllib.request
from pathlib import Path

QUIZ = Path(__file__).resolve().parents[1]
BIN = QUIZ / "bin"

URL_PATTERNS = [
    re.compile(r"https://[a-z0-9-]+\.trycloudflare\.com", re.I),
    re.compile(r"https://[a-z0-9-]+\.loca\.lt", re.I),
    re.compile(r"https://[a-z0-9-]+\.ngrok-free\.app", re.I),
    re.compile(r"https://[a-z0-9-]+\.ngrok\.io", re.I),
]


def _cloudflared_path() -> Path | None:
    found = shutil.which("cloudflared")
    if found:
        return Path(found)
    local = BIN / ("cloudflared.exe" if platform.system() == "Windows" else "cloudflared")
    return local if local.exists() else None


def _download_cloudflared() -> Path | None:
    system = platform.system()
    machine = platform.machine().lower()
    if system == "Windows":
        url = "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe"
        dest = BIN / "cloudflared.exe"
    elif system == "Darwin":
        url = (
            "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-darwin-arm64.tgz"
            if "arm" in machine
            else "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-darwin-amd64.tgz"
        )
        dest = BIN / "cloudflared"
    else:
        url = "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64"
        dest = BIN / "cloudflared"

    if dest.exists():
        return dest

    BIN.mkdir(parents=True, exist_ok=True)
    print("  正在下载 cloudflared（公网隧道，约 20MB，仅首次）…", flush=True)
    try:
        if system == "Darwin" and url.endswith(".tgz"):
            import tarfile
            import io

            with urllib.request.urlopen(url, timeout=120) as resp:
                data = resp.read()
            with tarfile.open(fileobj=io.BytesIO(data), mode="r:gz") as tar:
                member = tar.getmembers()[0]
                tar.extract(member, BIN)
                extracted = BIN / member.name
                extracted.rename(dest)
        else:
            urllib.request.urlretrieve(url, dest)
        if system != "Windows":
            dest.chmod(0o755)
        print(f"  已保存 → {dest}")
        return dest
    except Exception as e:
        print(f"  cloudflared 下载失败: {e}")
        return None


def _parse_url(text: str) -> str | None:
    for pat in URL_PATTERNS:
        m = pat.search(text)
        if m:
            return m.group(0)
    return None


def _start_cloudflared(port: int) -> tuple[subprocess.Popen | None, str | None]:
    exe = _cloudflared_path() or _download_cloudflared()
    if not exe:
        return None, None

    proc = subprocess.Popen(
        [str(exe), "tunnel", "--url", f"http://127.0.0.1:{port}"],
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
        encoding="utf-8",
        errors="replace",
        bufsize=1,
    )
    deadline = time.time() + 45
    while time.time() < deadline:
        line = proc.stdout.readline() if proc.stdout else ""
        if not line and proc.poll() is not None:
            break
        url = _parse_url(line)
        if url:
            return proc, url
        time.sleep(0.05)
    proc.kill()
    return None, None


def _start_localtunnel(port: int) -> tuple[subprocess.Popen | None, str | None]:
    npx = shutil.which("npx")
    if not npx:
        return None, None
    proc = subprocess.Popen(
        [npx, "--yes", "localtunnel", "--port", str(port)],
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
        encoding="utf-8",
        errors="replace",
        bufsize=1,
    )
    deadline = time.time() + 60
    while time.time() < deadline:
        line = proc.stdout.readline() if proc.stdout else ""
        if not line and proc.poll() is not None:
            break
        url = _parse_url(line)
        if url:
            return proc, url
        time.sleep(0.05)
    proc.kill()
    return None, None


class PublicTunnel:
    def __init__(self):
        self.process: subprocess.Popen | None = None
        self.url: str | None = None
        self.provider: str | None = None

    def start(self, port: int) -> str | None:
        print("  正在创建公网链接（任意 WiFi / 流量均可访问）…", flush=True)

        self.process, self.url = _start_cloudflared(port)
        if self.url:
            self.provider = "cloudflare"
            return self.url

        self.process, self.url = _start_localtunnel(port)
        if self.url:
            self.provider = "localtunnel"
            return self.url

        print()
        print("  ⚠ 未能自动创建公网链接。可手动安装后重试：")
        print("     winget install Cloudflare.cloudflared")
        print("     或安装 Node.js 后使用 npx localtunnel")
        return None

    def stop(self):
        if self.process and self.process.poll() is None:
            self.process.terminate()
            try:
                self.process.wait(timeout=5)
            except subprocess.TimeoutExpired:
                self.process.kill()


_tunnel: PublicTunnel | None = None
_tunnel_lock = threading.Lock()


def start_public_tunnel(port: int) -> str | None:
    global _tunnel
    with _tunnel_lock:
        if _tunnel and _tunnel.url:
            return _tunnel.url
        _tunnel = PublicTunnel()
        return _tunnel.start(port)


def get_public_url() -> str | None:
    return _tunnel.url if _tunnel else None


def stop_public_tunnel():
    global _tunnel
    with _tunnel_lock:
        if _tunnel:
            _tunnel.stop()
            _tunnel = None
