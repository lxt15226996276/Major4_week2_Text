#!/usr/bin/env python3
"""
刷题网站服务器 — 默认开启公网访问，任意 WiFi 均可打开

用法：
  python QuizSite/scripts/server.py              # 公网 + 局域网
  python QuizSite/scripts/server.py --lan-only   # 仅局域网
  python QuizSite/scripts/server.py --port 9000
"""

from __future__ import annotations

import argparse
import json
import socket
import subprocess
import sys
from datetime import datetime, timezone
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import urlparse

QUIZ = Path(__file__).resolve().parents[1]
IMPORTS = QUIZ / "imports"
BUILD_SCRIPT = QUIZ / "scripts" / "build_all.py"
sys.path.insert(0, str(QUIZ / "scripts"))
from tunnel import get_public_url, start_public_tunnel, stop_public_tunnel  # noqa: E402


def get_lan_ips() -> list[str]:
    ips = set()
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(("8.8.8.8", 80))
        ips.add(s.getsockname()[0])
        s.close()
    except OSError:
        pass
    try:
        for info in socket.getaddrinfo(socket.gethostname(), None, socket.AF_INET):
            ips.add(info[4][0])
    except OSError:
        pass
    ips.discard("127.0.0.1")
    return sorted(ips)


def run_build() -> tuple[bool, str]:
    try:
        r = subprocess.run(
            [sys.executable, str(BUILD_SCRIPT)],
            capture_output=True,
            text=True,
            encoding="utf-8",
            cwd=str(QUIZ),
            timeout=120,
        )
        return r.returncode == 0, ((r.stdout or "") + (r.stderr or "")).strip()
    except Exception as e:
        return False, str(e)


class QuizHandler(SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=str(QUIZ), **kwargs)

    def log_message(self, fmt, *args):
        print(f"[{self.log_date_time_string()}] {self.address_string()} {fmt % args}")

    def end_headers(self):
        self.send_header("Cache-Control", "no-cache")
        self.send_header("Access-Control-Allow-Origin", "*")
        super().end_headers()

    def do_OPTIONS(self):
        self.send_response(204)
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
        self.send_header("Access-Control-Allow-Headers", "Content-Type")
        self.end_headers()

    def do_GET(self):
        path = urlparse(self.path).path
        if path == "/api/info":
            port = self.server.server_address[1]
            public = get_public_url()
            lan_urls = [f"http://{ip}:{port}" for ip in get_lan_ips()]
            local = f"http://127.0.0.1:{port}"
            urls = ([public] if public else []) + lan_urls + [local]
            self._json_response(
                {
                    "port": port,
                    "publicUrl": public,
                    "lanIps": get_lan_ips(),
                    "urls": urls,
                    "importPage": f"{public or local}/import.html",
                }
            )
            return
        super().do_GET()

    def do_POST(self):
        path = urlparse(self.path).path
        length = int(self.headers.get("Content-Length", 0))
        body = self.rfile.read(length) if length else b""

        if path == "/api/import":
            self._handle_import(body)
        elif path == "/api/rebuild":
            ok, msg = run_build()
            self._json_response({"ok": ok, "message": msg})
        else:
            self.send_error(404)

    def _handle_import(self, body: bytes):
        try:
            data = json.loads(body.decode("utf-8"))
        except json.JSONDecodeError:
            self._json_response({"ok": False, "message": "JSON 格式错误"}, 400)
            return

        name = data.get("name") or "自定义题库"
        questions = data.get("questions") or []
        if not questions:
            self._json_response({"ok": False, "message": "题目列表为空"}, 400)
            return

        IMPORTS.mkdir(parents=True, exist_ok=True)
        safe = "".join(c if c.isalnum() or c in "-_" else "_" for c in name)[:50]
        ts = datetime.now().strftime("%Y%m%d_%H%M%S")
        filename = f"custom_{safe}_{ts}.json"
        payload = {"id": f"custom-{ts}", "name": name, "type": "custom", "questions": questions}
        (IMPORTS / filename).write_text(
            json.dumps(payload, ensure_ascii=False, indent=2), encoding="utf-8"
        )
        ok, msg = run_build()
        self._json_response(
            {
                "ok": ok,
                "message": f"已保存 {filename}，共 {len(questions)} 题"
                + ("" if ok else f"；构建警告: {msg}"),
                "file": filename,
                "count": len(questions),
            }
        )

    def _json_response(self, data: dict, status: int = 200):
        payload = json.dumps(data, ensure_ascii=False).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(payload)))
        self.end_headers()
        self.wfile.write(payload)


def main():
    parser = argparse.ArgumentParser(description="QuizSite 刷题服务器（默认公网可访问）")
    parser.add_argument("--port", "-p", type=int, default=8080)
    parser.add_argument("--host", default="0.0.0.0")
    parser.add_argument("--lan-only", action="store_true", help="仅局域网，不创建公网链接")
    parser.add_argument("--public", action="store_true", help=argparse.SUPPRESS)  # 兼容旧参数
    args = parser.parse_args()

    if not (QUIZ / "data" / "manifest.json").exists():
        print("首次运行，正在构建题库…")
        run_build()

    public_url = None
    if not args.lan_only:
        public_url = start_public_tunnel(args.port)

    server = ThreadingHTTPServer((args.host, args.port), QuizHandler)

    print()
    print("=" * 60)
    print("  随机刷题网站已启动")
    print("=" * 60)
    if public_url:
        print()
        print("  ★ 公网地址（发给任何人，不同 WiFi 也能打开）：")
        print(f"     {public_url}")
        print(f"  分享页:   {public_url}/share.html  （含二维码，可截图发微信）")
        print()
        print("  ┌─────────────────────────────────────────────────────┐")
        print("  │  注意：不要发 start.bat 给手机！手机打不开 bat。   │")
        print("  │  请复制上面的 https 链接 或 分享页二维码 发到微信。 │")
        print("  └─────────────────────────────────────────────────────┘")
    print(f"  本机:   http://127.0.0.1:{args.port}")
    for ip in get_lan_ips():
        print(f"  局域网: http://{ip}:{args.port}")
    print(f"  录入:   {(public_url or f'http://127.0.0.1:{args.port}')}/import.html")
    print()
    if public_url:
        print("  把「公网地址」复制发给同学即可；关窗口或 Ctrl+C 后链接失效。")
    else:
        print("  公网链接未创建成功，目前仅本机/局域网可访问。")
        print("  可安装 cloudflared 后重试: winget install Cloudflare.cloudflared")
    print("  按 Ctrl+C 停止")
    print("=" * 60)
    print()

    try:
        server.serve_forever()
    except KeyboardInterrupt:
        print("\n已停止")
    finally:
        stop_public_tunnel()
        server.server_close()


if __name__ == "__main__":
    main()
