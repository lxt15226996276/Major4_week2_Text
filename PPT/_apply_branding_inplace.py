#!/usr/bin/env python3
"""Replace 振涛教育 branding with 振涛弘业 in place (text + header logo)."""

from __future__ import annotations

import shutil
import sys
import zipfile
from pathlib import Path

PPT_DIR = Path(r"d:\Project\UnityProject\Major4_week2_Text\PPT")
TARGET = PPT_DIR / "01-第一单元画布和基础布局.pptx"
STD = PPT_DIR / "标准.pptx"
BACKUP = PPT_DIR / "_01-第一单元画布和基础布局.pptx.bak"

sys.path.insert(0, str(PPT_DIR))
from _pptx_zip import patch_pptx_entries  # noqa: E402

TEXT_REPLACEMENTS = (
    ("振涛教育", "振涛弘业"),
    ("ZHENTAO EDUCATION", "ZHENTAO HONGYE"),
    ("Zhentao Education", "Zhentao Hongye"),
    ("zhentao education", "zhentao hongye"),
)
GBK_REPLACEMENTS = (("振涛教育".encode("gbk"), "振涛弘业".encode("gbk")),)
BRANDING_MEDIA = ("image4.png",)


def replace_bytes(data: bytes) -> tuple[bytes, int]:
    count = 0
    out = data
    for old, new in GBK_REPLACEMENTS:
        n = out.count(old)
        if n:
            out = out.replace(old, new)
            count += n
    try:
        text = out.decode("utf-8")
    except UnicodeDecodeError:
        return out, count
    updated = text
    for old, new in TEXT_REPLACEMENTS:
        n = updated.count(old)
        if n:
            updated = updated.replace(old, new)
            count += n
    if updated != text:
        out = updated.encode("utf-8")
    return out, count


def load_standard_branding_media(std: Path) -> dict[str, bytes]:
    media: dict[str, bytes] = {}
    with zipfile.ZipFile(std) as z:
        for name in BRANDING_MEDIA:
            path = f"ppt/media/{name}"
            if path in z.namelist():
                media[name] = z.read(path)
    return media


def apply_branding_to_work(work: Path, std_work: Path) -> tuple[int, int]:
    """Replace branding text/media inside extracted work dir (no zip rewrite)."""
    std_media: dict[str, bytes] = {}
    for name in BRANDING_MEDIA:
        media_path = std_work / "ppt" / "media" / name
        if media_path.exists():
            std_media[name] = media_path.read_bytes()

    text_count = 0
    for xml_path in work.rglob("*.xml"):
        new_data, n = replace_bytes(xml_path.read_bytes())
        if n:
            xml_path.write_bytes(new_data)
            text_count += n

    media_count = 0
    for media_name, data in std_media.items():
        target = work / "ppt" / "media" / media_name
        if target.exists() and target.read_bytes() != data:
            target.write_bytes(data)
            media_count += 1
    return text_count, media_count


def apply_branding(pptx: Path, std: Path) -> tuple[int, int]:
    std_media = load_standard_branding_media(std)
    text_count = 0
    media_count = 0
    updates: dict[str, bytes] = {}

    with zipfile.ZipFile(pptx, "r") as zin:
        for item in zin.infolist():
            name = item.filename
            if name.endswith("/"):
                continue
            data = zin.read(name)
            if name.endswith(".xml"):
                new_data, n = replace_bytes(data)
                if n:
                    updates[name] = new_data
                    text_count += n
            elif name.startswith("ppt/media/"):
                media_name = name.split("/")[-1]
                if media_name in std_media and data != std_media[media_name]:
                    updates[name] = std_media[media_name]
                    media_count += 1

    for media_name, data in std_media.items():
        part = f"ppt/media/{media_name}"
        if part not in updates:
            with zipfile.ZipFile(pptx) as z:
                if part in z.namelist() and z.read(part) != data:
                    updates[part] = data
                    media_count += 1

    patch_pptx_entries(pptx, updates)
    return text_count, media_count


def main() -> None:
    if not TARGET.exists():
        raise FileNotFoundError(TARGET)
    if not STD.exists():
        raise FileNotFoundError(STD)

    if not BACKUP.exists():
        shutil.copy2(TARGET, BACKUP)
        print(f"Backup: {BACKUP.name}")

    text_count, media_count = apply_branding(TARGET, STD)
    print(f"Updated: {TARGET.name}")
    print(f"  XML text replacements: {text_count}")
    print(f"  Logo media replaced: {media_count} ({', '.join(BRANDING_MEDIA)})")


if __name__ == "__main__":
    main()
