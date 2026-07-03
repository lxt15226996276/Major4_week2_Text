#!/usr/bin/env python3
"""Patch pptx zip entries in-place — preserve unchanged local file records (WPS-safe)."""

from __future__ import annotations

import subprocess
import tempfile
from pathlib import Path

_PS = r"""
param([string]$PptxPath, [string]$UpdatesJson)
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$updates = Get-Content -LiteralPath $UpdatesJson -Raw -Encoding UTF8 | ConvertFrom-Json
$zip = [System.IO.Compression.ZipFile]::Open($PptxPath, [System.IO.Compression.ZipArchiveMode]::Update)
try {
    foreach ($u in $updates) {
        $name = $u.Name
        $bytes = [Convert]::FromBase64String($u.Base64)
        $existing = $zip.GetEntry($name)
        if ($existing) { $existing.Delete() }
        $entry = $zip.CreateEntry($name, [System.IO.Compression.CompressionLevel]::Optimal)
        $stream = $entry.Open()
        try { $stream.Write($bytes, 0, $bytes.Length) } finally { $stream.Close() }
    }
} finally { $zip.Dispose() }
"""


def patch_pptx_entries(pptx: Path, updates: dict[str, bytes]) -> None:
    """Update only listed zip parts; leave all other entries byte-identical."""
    if not updates:
        return
    import base64
    import json

    lines = []
    for name, data in updates.items():
        lines.append({"Name": name, "Base64": base64.b64encode(data).decode("ascii")})
    with tempfile.TemporaryDirectory() as td:
        td_path = Path(td)
        json_path = td_path / "updates.json"
        ps_path = td_path / "patch.ps1"
        ps_path.write_text(_PS, encoding="utf-8")
        json_path.write_text(json.dumps(lines), encoding="utf-8")
        subprocess.run(
            [
                "powershell",
                "-NoProfile",
                "-ExecutionPolicy",
                "Bypass",
                "-File",
                str(ps_path),
                "-PptxPath",
                str(pptx.resolve()),
                "-UpdatesJson",
                str(json_path),
            ],
            check=True,
        )


def patch_pptx_single(pptx: Path, part_name: str, data: bytes) -> None:
    patch_pptx_entries(pptx, {part_name: data})
