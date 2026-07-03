#!/usr/bin/env python3
"""Remove top-right cover logo from slide1 background only."""

from __future__ import annotations

import io
import re
import sys
import zipfile
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter

PPT_DIR = Path(r"d:\Project\UnityProject\Major4_week2_Text\PPT")
DEFAULT_PPTX = PPT_DIR / "01-第一单元画布和基础布局.pptx"

sys.path.insert(0, str(PPT_DIR))
from _pptx_zip import patch_pptx_single  # noqa: E402

# Top-right branding block on 1920x1080 cover (logo + black backing).
ERASE_BOX = (940, 8, 1910, 310)
# Plain dark texture source strip (left of logo, same height).
PATCH_SRC = (520, 8, 930, 310)


def slide1_cover_media_from_rels(rels: str) -> str:
    for m in re.finditer(
        r'Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/image" '
        r'Target="\.\./media/([^"]+)"',
        rels,
    ):
        return m.group(1)
    raise FileNotFoundError("slide1 cover image not found")


def slide1_cover_media(parts: dict[str, bytes]) -> str:
    return slide1_cover_media_from_rels(
        parts["ppt/slides/_rels/slide1.xml.rels"].decode("utf-8")
    )


def remove_top_right_logo(cover: Image.Image) -> Image.Image:
    out = cover.convert("RGBA").copy()
    w, h = out.size
    ex0, ey0, ex1, ey1 = ERASE_BOX
    sx0, sy0, sx1, sy1 = PATCH_SRC

    # Scale boxes if image size differs from 1920x1080 reference.
    if w != 1920 or h != 1080:
        sx = w / 1920
        sy = h / 1080
        ex0, ey0, ex1, ey1 = (int(ex0 * sx), int(ey0 * sy), int(ex1 * sx), int(ey1 * sy))
        sx0, sy0, sx1, sy1 = (int(sx0 * sx), int(sy0 * sy), int(sx1 * sx), int(sy1 * sy))

    patch = out.crop((sx0, sy0, sx1, sy1))
    target_w = ex1 - ex0
    target_h = ey1 - ey0
    patch = patch.resize((target_w, target_h), Image.Resampling.LANCZOS)
    patch = patch.filter(ImageFilter.GaussianBlur(radius=0.6))

    out.paste(patch, (ex0, ey0))

    # Soft edge blend on left side of patch to hide seam.
    blend_w = min(36, target_w // 5)
    base = cover.convert("RGBA")
    for x in range(blend_w):
        alpha = x / max(blend_w - 1, 1)
        px_x = ex0 + x
        for y in range(ey0, ey1):
            pr, pg, pb, pa = patch.getpixel((x, y - ey0))
            br, bg, bb, ba = base.getpixel((px_x, y))
            out.putpixel(
                (px_x, y),
                (
                    int(br * (1 - alpha) + pr * alpha),
                    int(bg * (1 - alpha) + pg * alpha),
                    int(bb * (1 - alpha) + pb * alpha),
                    255,
                ),
            )
    return out


def remove_cover_logo_from_work(work: Path) -> str:
    """Patch slide1 cover image inside extracted work dir (no zip rewrite)."""
    rels_path = work / "ppt" / "slides" / "_rels" / "slide1.xml.rels"
    media = slide1_cover_media_from_rels(rels_path.read_text(encoding="utf-8"))
    media_path = work / "ppt" / "media" / media
    cover = Image.open(media_path).convert("RGBA")
    cleaned = remove_top_right_logo(cover)
    cleaned.save(media_path, format="PNG")
    return media


def remove_cover_logo_from_pptx(pptx: Path) -> str:
    with zipfile.ZipFile(pptx) as z:
        parts = {item.filename: z.read(item.filename) for item in z.infolist()}

    media = slide1_cover_media(parts)
    cover = Image.open(io.BytesIO(parts[f"ppt/media/{media}"])).convert("RGBA")
    cleaned = remove_top_right_logo(cover)

    buf = io.BytesIO()
    cleaned.save(buf, format="PNG")
    write_pptx_image(pptx, media, buf.getvalue())
    return media


def write_pptx_image(pptx: Path, media_name: str, png_bytes: bytes) -> None:
    part = f"ppt/media/{media_name}"
    with zipfile.ZipFile(pptx) as z:
        if part not in z.namelist():
            raise FileNotFoundError(part)
    patch_pptx_single(pptx, part, png_bytes)


def main() -> None:
    if not DEFAULT_PPTX.exists():
        raise FileNotFoundError(DEFAULT_PPTX)
    media = remove_cover_logo_from_pptx(DEFAULT_PPTX)
    print(f"Removed top-right logo from {media} in {DEFAULT_PPTX.name}")


if __name__ == "__main__":
    main()
