#!/usr/bin/env python3
"""Apply standard template, insert fixed slide 2, auto-fit body content."""

from __future__ import annotations

import copy
import os
import re
import shutil
import zipfile
from pathlib import Path
from xml.etree import ElementTree as ET

PPT_DIR = Path(r"d:\Project\UnityProject\Major4_week2_Text\PPT")

OLD_W, OLD_H = 9144000, 5144135
NEW_W, NEW_H = 12192000, 6858000
SX = NEW_W / OLD_W
SY = NEW_H / OLD_H

FIXED_INSERT_AT = 2
STD_FIXED_SLIDE = 2
STD_LAST_SLIDE = 14

# Standard template content band: below blue header line, above page-number footer.
BLUE_LINE_BOTTOM = 795536
PAGE_FOOTER_TOP = 6257928
CONTENT_MARGIN_TOP = 150000
CONTENT_MARGIN_BOTTOM = 150000
CONTENT_MARGIN_X = 640000
MIN_FONT_SZ = 900
MAX_FONT_SZ = int(4800 * SY)
TEXT_HEIGHT_SAFETY = 0.88

P_NS = "http://schemas.openxmlformats.org/presentationml/2006/main"
A_NS = "http://schemas.openxmlformats.org/drawingml/2006/main"
R_NS = "http://schemas.openxmlformats.org/officeDocument/2006/relationships"
REL_NS = "http://schemas.openxmlformats.org/package/2006/relationships"
CT_NS = "http://schemas.openxmlformats.org/package/2006/content-types"

NS = {"p": P_NS, "a": A_NS, "r": R_NS}

TEMPLATE_PREFIXES = (
    "ppt/theme/",
    "ppt/slideMasters/",
    "ppt/slideLayouts/",
    "ppt/notesMasters/",
)

LAYOUT_GEOMETRY = {
    "slideLayout5.xml": {
        "title": (2927652, 188643, 9025169, 460375),
        "body": (239349, 1028733, 11521280, 4992555),
    },
    "slideLayout10.xml": {
        "ctrTitle": (914400, 2130425, 10363200, 1470025),
        "subTitle": (1828800, 3886200, 8534400, 1752600),
    },
    "slideLayout4.xml": {
        "title": (2927652, 188643, 9025169, 492443),
        "subTitle": (623392, 1028733, 9985109, 480053),
    },
}

OUTPUT_NAME = "01-第一单元画布和基础布局.pptx"
BRANDING_MEDIA_NAMES = ("image4.png",)


def find_files() -> tuple[Path, Path]:
    orig = None
    std = None
    for f in os.listdir(PPT_DIR):
        if not f.endswith(".pptx") or f.startswith("_") or f.startswith("~$"):
            continue
        path = PPT_DIR / f
        if "原版" in f:
            orig = path
        elif f.startswith("01-"):
            pass
        else:
            std = path
    if orig is None:
        for f in os.listdir(PPT_DIR):
            if f.endswith(".pptx") and f.startswith("01-") and not f.startswith("_"):
                orig = PPT_DIR / f
                break
    if std is None:
        raise FileNotFoundError("Standard template PPT not found")
    if orig is None:
        raise FileNotFoundError("Original PPT not found")
    return orig, std


def extract_zip(zpath: Path, dest: Path) -> None:
    if dest.exists():
        shutil.rmtree(dest)
    dest.mkdir(parents=True)
    with zipfile.ZipFile(zpath) as zf:
        zf.extractall(dest)


def collect_media(root: Path, rel_paths: list[Path]) -> set[str]:
    media: set[str] = set()
    for rel_path in rel_paths:
        if not rel_path.exists():
            continue
        tree = ET.parse(rel_path)
        for rel in tree.getroot():
            target = rel.get("Target", "")
            if target.startswith("../media/"):
                media.add("ppt/media/" + target.split("../media/", 1)[1])
    return media


def all_rels_under(root: Path, subdir: str) -> list[Path]:
    base = root / subdir
    return list(base.rglob("*.rels")) if base.exists() else []


def remove_template_parts(work: Path) -> None:
    for prefix in TEMPLATE_PREFIXES:
        target = work / prefix.rstrip("/")
        if target.exists():
            shutil.rmtree(target)


def copy_template_parts(std_work: Path, work: Path) -> None:
    for prefix in TEMPLATE_PREFIXES:
        src_dir = std_work / prefix.rstrip("/")
        dst_dir = work / prefix.rstrip("/")
        if src_dir.exists():
            shutil.copytree(src_dir, dst_dir)


def copy_media(std_work: Path, work: Path) -> None:
    """Copy standard-template media without overwriting original slide assets."""
    rel_files = []
    for prefix in TEMPLATE_PREFIXES:
        rel_files.extend(all_rels_under(std_work, prefix))
    media_files = collect_media(std_work, rel_files)
    for media in media_files:
        src = std_work / media
        dst = work / media
        if dst.exists():
            continue
        if src.exists():
            dst.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(src, dst)


def extract_block(text: str, tag: str) -> str | None:
    paired = re.search(rf"(<p:{tag}[^>]*>.*?</p:{tag}>)", text, flags=re.DOTALL)
    if paired:
        return paired.group(1)
    self_closing = re.search(rf"(<p:{tag}\b[^>]*/>)", text, flags=re.DOTALL)
    return self_closing.group(1) if self_closing else None


def replace_block(text: str, tag: str, new_block: str) -> str:
    paired = rf"<p:{tag}[^>]*>.*?</p:{tag}>"
    self_closing = rf"<p:{tag}\b[^>]*/>"
    if re.search(paired, text, flags=re.DOTALL):
        return re.sub(paired, new_block, text, count=1, flags=re.DOTALL)
    if re.search(self_closing, text, flags=re.DOTALL):
        return re.sub(self_closing, new_block, text, count=1, flags=re.DOTALL)
    raise ValueError(f"Missing <p:{tag}> block in presentation.xml")


def patch_presentation_xml(work: Path, std_work: Path) -> None:
    src_pres = work / "ppt" / "presentation.xml"
    std_pres = std_work / "ppt" / "presentation.xml"
    src_text = src_pres.read_text(encoding="utf-8")
    std_text = std_pres.read_text(encoding="utf-8")

    std_masters = extract_block(std_text, "sldMasterIdLst")
    std_size = extract_block(std_text, "sldSz")
    std_style = extract_block(std_text, "defaultTextStyle")
    if not std_masters or not std_size or not std_style:
        raise ValueError("Standard presentation.xml is missing required template blocks")

    src_text = replace_block(src_text, "sldMasterIdLst", std_masters)
    src_text = replace_block(
        src_text,
        "notesMasterIdLst",
        '<p:notesMasterIdLst><p:notesMasterId r:id="rId8"/></p:notesMasterIdLst>',
    )
    src_text = re.sub(
        r"<p:handoutMasterIdLst>.*?</p:handoutMasterIdLst>\s*",
        "",
        src_text,
        count=1,
        flags=re.DOTALL,
    )
    src_text = replace_block(src_text, "sldSz", std_size)
    src_text = replace_block(src_text, "defaultTextStyle", std_style)
    src_pres.write_text(src_text, encoding="utf-8")


def patch_presentation_rels(work: Path) -> None:
    rels_path = work / "ppt" / "_rels" / "presentation.xml.rels"
    tree = ET.parse(rels_path)
    root = tree.getroot()
    master_targets = {
        "rId1": "slideMasters/slideMaster1.xml",
        "rId3": "slideMasters/slideMaster2.xml",
        "rId4": "slideMasters/slideMaster3.xml",
    }
    to_remove = []
    for rel in root:
        rel_type = rel.get("Type", "")
        rel_id = rel.get("Id", "")
        if "handoutMaster" in rel_type:
            to_remove.append(rel)
        elif "slideMaster" in rel_type:
            if rel_id in master_targets:
                rel.set("Target", master_targets[rel_id])
            else:
                to_remove.append(rel)
        elif "notesMaster" in rel_type:
            rel.set("Target", "notesMasters/notesMaster1.xml")
        elif "theme" in rel_type and rel_id == "rId2":
            rel.set("Target", "theme/theme1.xml")
    for rel in to_remove:
        root.remove(rel)
    tree.write(rels_path, encoding="UTF-8", xml_declaration=True)


def patch_content_types(work: Path, std_work: Path) -> None:
    ct_path = work / "[Content_Types].xml"
    tree = ET.parse(ct_path)
    root = tree.getroot()
    remove_prefixes = (
        "/ppt/theme/",
        "/ppt/slideMasters/",
        "/ppt/slideLayouts/",
        "/ppt/notesMasters/",
        "/ppt/handoutMasters/",
    )
    for node in list(root):
        part = node.get("PartName", "")
        if any(part.startswith(prefix) for prefix in remove_prefixes):
            root.remove(node)
    std_ct = ET.parse(std_work / "[Content_Types].xml")
    for node in std_ct.getroot():
        part = node.get("PartName", "")
        if any(part.startswith(prefix) for prefix in remove_prefixes):
            root.append(copy.deepcopy(node))
    sync_content_type_defaults(work, std_work, root)
    tree.write(ct_path, encoding="UTF-8", xml_declaration=True)
    fix_content_types_declaration(ct_path)


def _content_types_tag(node: ET.Element) -> str:
    return node.tag.split("}")[-1] if "}" in node.tag else node.tag


def sync_content_type_defaults(work: Path, std_work: Path, root: ET.Element | None = None) -> None:
    """Merge Default/Override entries from standard template so copied media (e.g. .wdp) is valid."""
    ct_path = work / "[Content_Types].xml"
    if root is None:
        tree = ET.parse(ct_path)
        root = tree.getroot()
        write_back = True
    else:
        tree = None
        write_back = False

    existing_defaults = {
        node.get("Extension", "").lower()
        for node in root
        if _content_types_tag(node) == "Default"
    }
    existing_overrides = {node.get("PartName", "") for node in root if _content_types_tag(node) == "Override"}

    std_ct = ET.parse(std_work / "[Content_Types].xml")
    for node in std_ct.getroot():
        tag = _content_types_tag(node)
        if tag == "Default":
            ext = node.get("Extension", "").lower()
            if ext and ext not in existing_defaults:
                root.append(copy.deepcopy(node))
                existing_defaults.add(ext)
        elif tag == "Override":
            part = node.get("PartName", "")
            if part.startswith("/ppt/media/") and (work / part.lstrip("/")).exists():
                if part not in existing_overrides:
                    root.append(copy.deepcopy(node))
                    existing_overrides.add(part)

    if write_back:
        tree.write(ct_path, encoding="UTF-8", xml_declaration=True)
        fix_content_types_declaration(ct_path)


def fix_content_types_declaration(path: Path) -> None:
    text = path.read_text(encoding="utf-8")
    text = text.replace("<?xml version='1.0' encoding='UTF-8'?>", '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>')
    path.write_text(text, encoding="utf-8")


def verify_package_content_types(work: Path) -> None:
    ct_text = (work / "[Content_Types].xml").read_text(encoding="utf-8").lower()
    missing = []
    media_dir = work / "ppt" / "media"
    if not media_dir.exists():
        return
    for file_path in media_dir.iterdir():
        if not file_path.is_file():
            continue
        part = "/" + file_path.relative_to(work).as_posix()
        if part.lower() in ct_text:
            continue
        ext = file_path.suffix.lstrip(".").lower()
        if ext and f'extension="{ext}"' in ct_text:
            continue
        missing.append(part)
    if missing:
        raise RuntimeError(f"Missing [Content_Types] entries: {missing[:8]}")


def scale_int(value: str | None, factor: float) -> str | None:
    if value is None:
        return None
    return str(int(round(int(value) * factor)))


def scale_xfrm(node: ET.Element, sx: float, sy: float) -> None:
    for tag in ("off", "ext", "chOff", "chExt"):
        el = node.find(f"a:{tag}", NS)
        if el is None:
            continue
        if tag in ("off", "chOff"):
            el.set("x", scale_int(el.get("x"), sx))
            el.set("y", scale_int(el.get("y"), sy))
        else:
            el.set("cx", scale_int(el.get("cx"), sx))
            el.set("cy", scale_int(el.get("cy"), sy))


def scale_geometry(root: ET.Element, sx: float, sy: float) -> None:
    for xfrm in root.iter(f"{{{A_NS}}}xfrm"):
        scale_xfrm(xfrm, sx, sy)


def set_xfrm(sp: ET.Element, x: int, y: int, cx: int, cy: int) -> None:
    sp_pr = sp.find("p:spPr", NS)
    if sp_pr is None:
        sp_pr = ET.SubElement(sp, f"{{{P_NS}}}spPr")
    xfrm = sp_pr.find("a:xfrm", NS)
    if xfrm is None:
        xfrm = ET.SubElement(sp_pr, f"{{{A_NS}}}xfrm")
    off = xfrm.find("a:off", NS)
    ext = xfrm.find("a:ext", NS)
    if off is None:
        off = ET.SubElement(xfrm, f"{{{A_NS}}}off")
    if ext is None:
        ext = ET.SubElement(xfrm, f"{{{A_NS}}}ext")
    off.set("x", str(x))
    off.set("y", str(y))
    ext.set("cx", str(cx))
    ext.set("cy", str(cy))


def shape_text(sp: ET.Element) -> str:
    parts = []
    for t in sp.iter(f"{{{A_NS}}}t"):
        if t.text:
            parts.append(t.text)
    return "".join(parts).strip()


def shape_name(sp: ET.Element) -> str:
    el = sp.find(".//p:cNvPr", NS)
    return el.get("name", "") if el is not None else ""


def placeholder_type(sp: ET.Element) -> str | None:
    ph = sp.find(".//p:ph", NS)
    if ph is None:
        return None
    ph_type = ph.get("type")
    if ph_type:
        return ph_type
    idx = ph.get("idx")
    if idx == "1":
        return "body"
    if idx in (None, "0"):
        return "title"
    return None


def shape_box(sp: ET.Element) -> tuple[int, int, int, int] | None:
    xfrm = sp.find(".//a:xfrm", NS)
    if xfrm is None:
        return None
    off = xfrm.find("a:off", NS)
    ext = xfrm.find("a:ext", NS)
    if off is None or ext is None:
        return None
    vals = [off.get("x"), off.get("y"), ext.get("cx"), ext.get("cy")]
    if any(v is None for v in vals):
        return None
    return tuple(int(v) for v in vals)


def is_old_logo_pic(sp: ET.Element) -> bool:
    if sp.tag != f"{{{P_NS}}}pic":
        return False
    box = shape_box(sp)
    if not box:
        return False
    x, y, cx, cy = box
    if x >= 6500000 and cx <= 2200000 and cy <= 700000:
        return True
    if cx <= 2200000 and cy <= 700000 and x >= 5500000:
        return True
    return False


def is_full_slide_background_pic(sp: ET.Element) -> bool:
    """Origin-anchored full-bleed picture (4:3 or 16:9 canvas)."""
    if sp.tag != f"{{{P_NS}}}pic":
        return False
    box = shape_box(sp)
    if not box:
        return False
    x, y, cx, cy = box
    if x > 150000 or y > 150000:
        return False
    w_ok = cx >= OLD_W * 0.92 or cx >= NEW_W * 0.92
    h_ok = cy >= OLD_H * 0.92 or cy >= NEW_H * 0.92
    return w_ok and h_ok


def normalize_cover_text(text: str) -> str:
    return re.sub(r"[\s\-—–·]", "", text)


def is_old_background_pic(sp: ET.Element) -> bool:
    if is_full_slide_background_pic(sp):
        return False
    if sp.tag != f"{{{P_NS}}}pic":
        return False
    box = shape_box(sp)
    if not box:
        return False
    _, _, cx, cy = box
    return cx >= 8500000 and cy >= 4800000


def is_old_decoration(sp: ET.Element) -> bool:
    if sp.tag != f"{{{P_NS}}}sp":
        return False
    return shape_name(sp).startswith("Rectangle") and not shape_text(sp)


def freeform_text_shapes(sp_tree: ET.Element) -> list[tuple[int, str, ET.Element]]:
    items = []
    for child in sp_tree:
        if child.tag != f"{{{P_NS}}}sp" or placeholder_type(child):
            continue
        text = shape_text(child)
        if not text:
            continue
        box = shape_box(child)
        y = box[1] if box else 0
        items.append((y, text, child))
    return sorted(items, key=lambda x: x[0])


def is_title_candidate(sp: ET.Element, layout_file: str) -> bool:
    if sp.tag != f"{{{P_NS}}}sp":
        return False
    if placeholder_type(sp):
        return False
    text = shape_text(sp)
    if not text or len(text) > 80:
        return False
    box = shape_box(sp)
    if not box:
        return False
    _, y, _, _ = box
    if layout_file == "slideLayout10.xml":
        return False
    return y <= 900000


def clone_title_placeholder(
    text: str,
    ph_type: str,
    geom: tuple[int, int, int, int],
    shape_id: int,
) -> ET.Element:
    x, y, cx, cy = geom
    sp = ET.Element(f"{{{P_NS}}}sp")
    nv = ET.SubElement(sp, f"{{{P_NS}}}nvSpPr")
    ET.SubElement(nv, f"{{{P_NS}}}cNvPr", {"id": str(shape_id), "name": f"标题 {shape_id}"})
    ET.SubElement(nv, f"{{{P_NS}}}cNvSpPr")
    nv_pr = ET.SubElement(nv, f"{{{P_NS}}}nvPr")
    ET.SubElement(nv_pr, f"{{{P_NS}}}ph", {"type": ph_type})
    set_xfrm(sp, x, y, cx, cy)
    tx_body = ET.SubElement(sp, f"{{{P_NS}}}txBody")
    ET.SubElement(tx_body, f"{{{A_NS}}}bodyPr")
    p = ET.SubElement(tx_body, f"{{{A_NS}}}p")
    r = ET.SubElement(p, f"{{{A_NS}}}r")
    ET.SubElement(r, f"{{{A_NS}}}rPr", {"lang": "zh-CN", "altLang": "en-US"})
    t = ET.SubElement(r, f"{{{A_NS}}}t")
    t.text = text
    ET.SubElement(p, f"{{{A_NS}}}endParaRPr", {"lang": "zh-CN", "altLang": "en-US"})
    return sp


def get_safe_content_zone(layout_file: str) -> tuple[int, int, int, int]:
    """Content must stay below the blue line and above the bottom-right page footer."""
    bx, _, bw, _ = LAYOUT_GEOMETRY[layout_file]["body"]
    top = BLUE_LINE_BOTTOM + CONTENT_MARGIN_TOP
    bottom = PAGE_FOOTER_TOP - CONTENT_MARGIN_BOTTOM
    height = max(bottom - top, 1_000_000)
    left = bx + CONTENT_MARGIN_X
    width = bw - 2 * CONTENT_MARGIN_X
    return (left, top, width, height)


def paragraph_text(p: ET.Element) -> str:
    return "".join(t.text or "" for t in p.iter(f"{{{A_NS}}}t"))


def estimate_content_height(sp: ET.Element, box_w: int, font_sz: int, ln_pct: int) -> int:
    l_ins = r_ins = 91440
    usable_w = max(box_w - l_ins - r_ins, 100000)
    char_w = max(int(font_sz / 100.0 * 12700 * 1.08), 12700)
    line_h = max(int((font_sz / 100.0) * (ln_pct / 100000.0) * 12700), 12700)
    total = 0
    for p in sp.iter(f"{{{A_NS}}}p"):
        text = paragraph_text(p)
        if not text.strip():
            continue
        chars = max(len(text), 1)
        lines = max(1, (chars * char_w + usable_w - 1) // usable_w)
        total += lines * line_h
    return max(total, line_h)


def apply_font_size_all(sp: ET.Element, sz: int) -> None:
    sz_str = str(sz)
    for tag in (f"{{{A_NS}}}rPr", f"{{{A_NS}}}endParaRPr", f"{{{A_NS}}}defRPr"):
        for rpr in sp.iter(tag):
            rpr.set("sz", sz_str)
    for lvl in sp.iter(f"{{{A_NS}}}lvl"):
        for def_rpr in lvl.iter(f"{{{A_NS}}}defRPr"):
            def_rpr.set("sz", sz_str)


def set_line_spacing_all(sp: ET.Element, ln_pct: int) -> None:
    for p in sp.iter(f"{{{A_NS}}}p"):
        ppr = p.find("a:pPr", NS)
        if ppr is None:
            ppr = ET.SubElement(p, f"{{{A_NS}}}pPr")
        for ln in list(ppr):
            if ln.tag == f"{{{A_NS}}}lnSpc":
                ppr.remove(ln)
        ln_spc = ET.SubElement(ppr, f"{{{A_NS}}}lnSpc")
        spc_pct = ET.SubElement(ln_spc, f"{{{A_NS}}}spcPct")
        spc_pct.set("val", str(ln_pct))


def normalize_paragraph_indents(sp: ET.Element) -> None:
    for p in sp.iter(f"{{{A_NS}}}p"):
        ppr = p.find("a:pPr", NS)
        if ppr is None:
            continue
        if ppr.find("a:buAutoNum", NS) is not None or ppr.find("a:buChar", NS) is not None:
            ppr.set("marL", "457200")
            ppr.set("indent", "-228600")


def shrink_text_to_fit(sp: ET.Element, box_w: int, box_h: int) -> None:
    """Explicitly shrink fonts/line spacing so text stays inside the box in WPS."""
    t_ins = b_ins = 91440
    avail_h = max(int((box_h - t_ins - b_ins) * TEXT_HEIGHT_SAFETY), 100000)
    start_sz = max(get_default_font_sz(sp), int(2000 * SY))
    start_sz = min(start_sz, MAX_FONT_SZ)
    for ln_pct in (115000, 120000, 130000, 140000, 150000):
        for sz in range(start_sz, MIN_FONT_SZ - 1, -50):
            if estimate_content_height(sp, box_w, sz, ln_pct) <= avail_h:
                apply_font_size_all(sp, sz)
                set_line_spacing_all(sp, ln_pct)
                normalize_paragraph_indents(sp)
                return
    apply_font_size_all(sp, MIN_FONT_SZ)
    set_line_spacing_all(sp, 110000)
    normalize_paragraph_indents(sp)


def get_default_font_sz(sp: ET.Element) -> int:
    sizes = [int(r.get("sz")) for r in sp.iter(f"{{{A_NS}}}rPr") if r.get("sz")]
    if sizes:
        return max(sizes)
    return 2000


def content_overflows(sp: ET.Element, box_w: int, box_h: int) -> bool:
    sz = get_default_font_sz(sp)
    ln_pct = 150000
    for p in sp.iter(f"{{{A_NS}}}p"):
        ppr = p.find("a:pPr", NS)
        if ppr is not None:
            spc = ppr.find("a:lnSpc/a:spcPct", NS)
            if spc is not None and spc.get("val"):
                ln_pct = int(spc.get("val"))
                break
    return estimate_content_height(sp, box_w, sz, ln_pct) > max(box_h - 182880, 100000) * TEXT_HEIGHT_SAFETY


def is_empty_placeholder(sp: ET.Element) -> bool:
    if sp.tag != f"{{{P_NS}}}sp":
        return False
    if placeholder_type(sp) is None:
        return False
    return not shape_text(sp)


def scaled_box(orig_box: tuple[int, int, int, int]) -> tuple[int, int, int, int]:
    ox, oy, ocx, ocy = orig_box
    return int(ox * SX), int(oy * SY), int(ocx * SX), int(ocy * SY)


def clamp_box_to_safe_zone(
    nx: int, ny: int, ncx: int, ncy: int, zone: tuple[int, int, int, int]
) -> tuple[int, int, int, int]:
    zx, zy, zw, zh = zone
    if ncx > zw or ncy > zh:
        scale = min(zw / max(ncx, 1), zh / max(ncy, 1), 1.0)
        ncx = max(int(ncx * scale), 100000)
        ncy = max(int(ncy * scale), 100000)
    if nx < zx:
        nx = zx
    if ny < zy:
        ny = zy
    if nx + ncx > zx + zw:
        nx = max(zx, zx + zw - ncx)
    if ny + ncy > zy + zh:
        ny = max(zy, zy + zh - ncy)
    return nx, ny, ncx, ncy


def resolve_content_overlap(nodes: list[ET.Element], safe_zone: tuple[int, int, int, int]) -> None:
    zx, zy, zw, zh = safe_zone
    ordered = sorted(
        (n for n in nodes if shape_box(n)),
        key=lambda n: shape_box(n)[1],
    )
    for i, upper in enumerate(ordered):
        ub = shape_box(upper)
        if not ub:
            continue
        _, uy, _, uh = ub
        for lower in ordered[i + 1 :]:
            lb = shape_box(lower)
            if not lb or not boxes_overlap(ub, lb):
                continue
            lx, _, lw, lh = lb
            new_y = uy + uh + 80000
            if new_y >= zy + zh:
                continue
            new_h = lh
            if new_y + new_h > zy + zh:
                new_h = max(zy + zh - new_y, 100000)
            set_xfrm(lower, lx, new_y, lw, new_h)
            if lower.tag == f"{{{P_NS}}}sp" and shape_text(lower):
                tb = shape_box(lower)
                if tb and content_overflows(lower, tb[2], tb[3]):
                    shrink_text_to_fit(lower, tb[2], tb[3])
            lb = shape_box(lower)
            if lb:
                ub = lb


def map_content_box(
    orig_box: tuple[int, int, int, int],
    safe_zone: tuple[int, int, int, int],
    *,
    full_width_table: bool = False,
) -> tuple[int, int, int, int]:
    """1:1 scale from original slide, then fit inside the template safe band."""
    ox, oy, ocx, ocy = orig_box
    zx, zy, zw, zh = safe_zone
    rw = ocx / OLD_W
    nx, ny, ncx, ncy = scaled_box(orig_box)
    if full_width_table and rw >= 0.88:
        ncx = zw
        nx = zx
        ncy = max(int(ocy * SY), 100000)
        ny = max(int(oy * SY), zy)
        if ny + ncy > zy + zh:
            ny = zy + zh - ncy
        if ny < zy:
            ncy = max(zy + zh - zy, 100000)
            ny = zy
    return clamp_box_to_safe_zone(nx, ny, ncx, ncy, safe_zone)


def is_table_like_pic(node: ET.Element, orig_box: tuple[int, int, int, int]) -> bool:
    if node.tag != f"{{{P_NS}}}pic":
        return False
    _, _, ocx, ocy = orig_box
    return ocx / OLD_W >= 0.88 and ocy / OLD_H >= 0.28


def boxes_overlap(a: tuple[int, int, int, int], b: tuple[int, int, int, int]) -> bool:
    ax, ay, aw, ah = a
    bx, by, bw, bh = b
    return ax < bx + bw and ax + aw > bx and ay < by + bh and ay + ah > by


def resolve_text_pic_overlap(nodes: list[ET.Element]) -> None:
    pics = [n for n in nodes if n.tag == f"{{{P_NS}}}pic"]
    texts = [n for n in nodes if n.tag == f"{{{P_NS}}}sp" and shape_text(n)]
    for text in texts:
        tb = shape_box(text)
        if not tb:
            continue
        tx, ty, tw, th = tb
        for pic in pics:
            pb = shape_box(pic)
            if not pb or not boxes_overlap(tb, pb):
                continue
            px, _, pw, _ = pb
            if px <= tx + tw * 0.55:
                continue
            max_w = px - tx - 120000
            if max_w < 400000:
                continue
            set_xfrm(text, tx, ty, max_w, th)
            tb = shape_box(text)
            if tb and content_overflows(text, tb[2], tb[3]):
                shrink_text_to_fit(text, tb[2], tb[3])


def scale_font_sizes(sp: ET.Element, factor: float = SY) -> None:
    for tag in (f"{{{A_NS}}}rPr", f"{{{A_NS}}}endParaRPr", f"{{{A_NS}}}defRPr"):
        for rpr in sp.iter(tag):
            if rpr.get("sz"):
                rpr.set("sz", str(max(int(int(rpr.get("sz")) * factor), 600)))


def apply_proportional_layout(node: ET.Element, safe_zone: tuple[int, int, int, int]) -> None:
    box = shape_box(node)
    if not box:
        return
    fitted = map_content_box(
        box,
        safe_zone,
        full_width_table=is_table_like_pic(node, box),
    )
    set_xfrm(node, *fitted)
    if node.tag != f"{{{P_NS}}}sp" or not shape_text(node):
        return
    scale_font_sizes(node)
    if not any(r.get("sz") for r in node.iter(f"{{{A_NS}}}rPr")):
        apply_font_size_all(node, int(2000 * SY))
    _, oy, _, ocy = box
    ry = (oy + ocy / 2) / OLD_H
    if 0.32 <= ry <= 0.68:
        tx_body = node.find("p:txBody", NS)
        if tx_body is not None:
            body_pr = tx_body.find("a:bodyPr", NS)
            if body_pr is None:
                body_pr = ET.SubElement(tx_body, f"{{{A_NS}}}bodyPr")
            body_pr.set("anchor", "ctr")
    orig_scaled = scaled_box(box)
    kept_scale = (
        fitted[2] >= orig_scaled[2] * 0.92
        and fitted[3] >= orig_scaled[3] * 0.92
    )
    if not kept_scale and content_overflows(node, fitted[2], fitted[3]):
        shrink_text_to_fit(node, fitted[2], fitted[3])


def insert_proportional_body_nodes(
    insert_nodes: list[ET.Element],
    body_shape: ET.Element | None,
    body_nodes: list[ET.Element],
    safe_zone: tuple[int, int, int, int],
) -> None:
    nodes: list[ET.Element] = []
    if body_shape is not None:
        nodes.append(body_shape)
    nodes.extend(body_nodes)
    nodes.sort(
        key=lambda n: (
            1 if n.tag == f"{{{P_NS}}}sp" and shape_text(n) else 0,
            shape_box(n)[1] if shape_box(n) else 0,
        )
    )
    laid_out: list[ET.Element] = []
    for node in nodes:
        if node.find(".//p:ph", NS) is not None:
            ph = node.find(".//p:ph", NS)
            if ph is not None and not ph.get("type"):
                ph.set("type", "body")
        apply_proportional_layout(node, safe_zone)
        laid_out.append(node)
    resolve_text_pic_overlap(laid_out)
    resolve_content_overlap(laid_out, safe_zone)
    insert_nodes.extend(laid_out)


def target_layout(old_layout: str, slide_num: int, slide_text: str) -> str:
    if old_layout in ("slideLayout55.xml", "slideLayout1.xml") and slide_num == 1:
        return "slideLayout10.xml"
    if old_layout == "slideLayout61.xml":
        return "slideLayout4.xml"
    if old_layout in ("slideLayout11.xml", "slideLayout14.xml"):
        return "slideLayout4.xml"
    if old_layout == "slideLayout60.xml":
        return "slideLayout5.xml"
    if old_layout == "slideLayout7.xml":
        return "slideLayout5.xml"
    if old_layout == "slideLayout1.xml":
        return "slideLayout5.xml"
    if old_layout == "slideLayout49.xml":
        if "休息" in slide_text:
            return "slideLayout9.xml"
        return "slideLayout5.xml"
    return "slideLayout5.xml"


def process_slide_xml(slide_xml: bytes, layout_file: str, slide_num: int) -> bytes:
    root = ET.fromstring(slide_xml)
    c_sld = root.find("p:cSld", NS)
    if c_sld is None:
        return slide_xml
    sp_tree = c_sld.find("p:spTree", NS)
    if sp_tree is None:
        return slide_xml

    title_texts: list[str] = []
    keep_nodes: list[ET.Element] = []
    body_nodes: list[ET.Element] = []
    body_shape: ET.Element | None = None
    max_id = 1

    if layout_file == "slideLayout10.xml":
        seen_cover_text: set[str] = set()
        for _, text, node in freeform_text_shapes(sp_tree):
            key = normalize_cover_text(text)
            if not key or key in seen_cover_text:
                sp_tree.remove(node)
                continue
            seen_cover_text.add(key)
            body_nodes.append(copy.deepcopy(node))
            sp_tree.remove(node)

    for child in list(sp_tree):
        if child.tag in (f"{{{P_NS}}}nvGrpSpPr", f"{{{P_NS}}}grpSpPr"):
            keep_nodes.append(child)
            sp_tree.remove(child)
            continue
        cnv = child.find(".//p:cNvPr", NS)
        if cnv is not None and cnv.get("id"):
            max_id = max(max_id, int(cnv.get("id")))

        if layout_file == "slideLayout10.xml" and is_full_slide_background_pic(child):
            body_nodes.insert(0, copy.deepcopy(child))
            sp_tree.remove(child)
            continue

        if is_old_logo_pic(child):
            sp_tree.remove(child)
            continue

        if is_old_background_pic(child) or is_old_decoration(child):
            sp_tree.remove(child)
            continue

        if is_empty_placeholder(child):
            sp_tree.remove(child)
            continue

        if is_title_candidate(child, layout_file):
            text = shape_text(child)
            if text and text not in title_texts:
                title_texts.append(text)
            sp_tree.remove(child)
            continue

        ph = placeholder_type(child)
        if layout_file == "slideLayout5.xml" and ph == "body":
            body_shape = copy.deepcopy(child)
            sp_tree.remove(child)
            continue

        if layout_file == "slideLayout5.xml" and child.tag == f"{{{P_NS}}}sp" and ph is None and shape_text(child):
            body_nodes.append(copy.deepcopy(child))
            sp_tree.remove(child)
            continue

        if layout_file in ("slideLayout5.xml", "slideLayout4.xml", "slideLayout9.xml"):
            if child.tag == f"{{{P_NS}}}pic" and is_full_slide_background_pic(child):
                sp_tree.remove(child)
                continue

        if layout_file == "slideLayout5.xml" and child.tag in (f"{{{P_NS}}}pic", f"{{{P_NS}}}grpSp"):
            body_nodes.append(copy.deepcopy(child))
            sp_tree.remove(child)
            continue

        if layout_file == "slideLayout4.xml" and child.tag == f"{{{P_NS}}}sp" and ph is None and shape_text(child):
            body_nodes.append(copy.deepcopy(child))
            sp_tree.remove(child)
            continue

        if layout_file == "slideLayout10.xml" and child.tag in (f"{{{P_NS}}}pic", f"{{{P_NS}}}grpSp"):
            body_nodes.append(copy.deepcopy(child))
            sp_tree.remove(child)
            continue

        if child.tag in (f"{{{P_NS}}}sp", f"{{{P_NS}}}pic", f"{{{P_NS}}}grpSp"):
            sp_tree.remove(child)
            continue

        keep_nodes.append(child)

    insert_nodes: list[ET.Element] = []
    safe_zone = get_safe_content_zone("slideLayout5.xml")

    if layout_file == "slideLayout10.xml":
        cover_zone = (0, 0, NEW_W, NEW_H)
        insert_proportional_body_nodes(insert_nodes, None, body_nodes, cover_zone)
    elif layout_file == "slideLayout4.xml":
        title = title_texts[0] if title_texts else ""
        sub = title_texts[1] if len(title_texts) > 1 else ""
        if title:
            insert_nodes.append(
                clone_title_placeholder(title, "title", LAYOUT_GEOMETRY["slideLayout4.xml"]["title"], max_id + 1)
            )
            max_id += 1
        if sub:
            insert_nodes.append(
                clone_title_placeholder(sub, "subTitle", LAYOUT_GEOMETRY["slideLayout4.xml"]["subTitle"], max_id + 1)
            )
        insert_proportional_body_nodes(insert_nodes, body_shape, body_nodes, safe_zone)
    elif layout_file == "slideLayout5.xml":
        title = title_texts[0] if title_texts else ""
        if title:
            insert_nodes.append(
                clone_title_placeholder(title, "title", LAYOUT_GEOMETRY["slideLayout5.xml"]["title"], max_id + 1)
            )
        insert_proportional_body_nodes(insert_nodes, body_shape, body_nodes, safe_zone)

    for node in insert_nodes:
        sp_tree.append(node)
    for node in keep_nodes:
        sp_tree.append(node)

    ET.register_namespace("p", P_NS)
    ET.register_namespace("a", A_NS)
    ET.register_namespace("r", R_NS)
    return xml_bytes(root)


def process_slides(work: Path) -> None:
    rels_dir = work / "ppt" / "slides" / "_rels"
    slides_dir = work / "ppt" / "slides"
    for rel_file in sorted(rels_dir.glob("slide*.xml.rels")):
        slide_num = int(re.search(r"slide(\d+)", rel_file.name).group(1))
        rel_text = rel_file.read_text(encoding="utf-8")
        old_layout = re.search(r"slideLayouts/(slideLayout\d+\.xml)", rel_text)
        if not old_layout:
            continue
        old_layout_name = old_layout.group(1)
        slide_path = slides_dir / f"slide{slide_num}.xml"
        slide_text = slide_path.read_text(encoding="utf-8")
        new_layout = target_layout(old_layout_name, slide_num, slide_text)
        rel_text = rel_text.replace(f"../slideLayouts/{old_layout_name}", f"../slideLayouts/{new_layout}")
        rel_file.write_text(rel_text, encoding="utf-8")
        updated = process_slide_xml(slide_path.read_bytes(), new_layout, slide_num)
        slide_path.write_bytes(updated)
    strip_cover_notes_rels(work)


def strip_cover_notes_rels(work: Path) -> None:
    """Cover slide should not reference notes (matches unit-1 final)."""
    rel_path = work / "ppt" / "slides" / "_rels" / "slide1.xml.rels"
    if not rel_path.exists():
        return
    text = rel_path.read_text(encoding="utf-8")
    new_text = re.sub(
        r'<Relationship[^>]*relationships/notesSlide"[^>]*/>\s*',
        "",
        text,
    )
    if new_text != text:
        rel_path.write_text(new_text, encoding="utf-8")


def max_slide_num(work: Path) -> int:
    slides_dir = work / "ppt" / "slides"
    nums = [int(re.search(r"slide(\d+)", f.name).group(1)) for f in slides_dir.glob("slide*.xml")]
    return max(nums)


def shift_slides_up(work: Path, from_num: int) -> None:
    slides_dir = work / "ppt" / "slides"
    rels_dir = slides_dir / "_rels"
    pres_rels_path = work / "ppt" / "_rels" / "presentation.xml.rels"
    last = max_slide_num(work)

    rels_tree = ET.parse(pres_rels_path)
    rels_root = rels_tree.getroot()
    for rel in rels_root:
        if not rel.get("Type", "").endswith("/slide"):
            continue
        target = rel.get("Target", "")
        m = re.match(r"slides/slide(\d+)\.xml", target)
        if m and int(m.group(1)) >= from_num:
            rel.set("Target", f"slides/slide{int(m.group(1)) + 1}.xml")
    rels_tree.write(pres_rels_path, encoding="UTF-8", xml_declaration=True)

    for i in range(last, from_num - 1, -1):
        src = slides_dir / f"slide{i}.xml"
        dst = slides_dir / f"slide{i + 1}.xml"
        if src.exists():
            shutil.move(str(src), str(dst))
        rel_src = rels_dir / f"slide{i}.xml.rels"
        rel_dst = rels_dir / f"slide{i + 1}.xml.rels"
        if rel_src.exists():
            shutil.move(str(rel_src), str(rel_dst))
        ensure_content_type(work, f"/ppt/slides/slide{i + 1}.xml", "application/vnd.openxmlformats-officedocument.presentationml.slide+xml")


def allocate_unique_media_name(work: Path, ext: str = ".png") -> str:
    media_dir = work / "ppt" / "media"
    nums = []
    if media_dir.exists():
        for part_file in media_dir.iterdir():
            match = re.match(r"image(\d+)\.", part_file.name)
            if match:
                nums.append(int(match.group(1)))
    return f"image{max(nums, default=0) + 1}{ext}"


def slide_referenced_media(work: Path) -> set[str]:
    media: set[str] = set()
    rels_dir = work / "ppt" / "slides" / "_rels"
    if not rels_dir.exists():
        return media
    for rel_file in rels_dir.glob("slide*.xml.rels"):
        text = rel_file.read_text(encoding="utf-8")
        for name in re.findall(r'Target="\.\./media/([^"]+)"', text):
            media.add(name)
    return media


def resolve_slide_media_conflicts(
    work: Path,
    reserved_names: tuple[str, ...] = BRANDING_MEDIA_NAMES,
) -> dict[str, str]:
    """Rename slide content media that collide with template branding asset names."""
    renames: dict[str, str] = {}
    slide_media = slide_referenced_media(work)
    rels_dir = work / "ppt" / "slides" / "_rels"

    for name in reserved_names:
        if name not in slide_media:
            continue
        src = work / "ppt" / "media" / name
        if not src.exists():
            continue
        new_name = allocate_unique_media_name(work, Path(name).suffix or ".png")
        shutil.copy2(src, work / "ppt" / "media" / new_name)
        renames[name] = new_name
        for rel_file in rels_dir.glob("slide*.xml.rels"):
            text = rel_file.read_text(encoding="utf-8")
            new_text = text.replace(f"../media/{name}", f"../media/{new_name}")
            if new_text != text:
                rel_file.write_text(new_text, encoding="utf-8")
    return renames


def xml_bytes(root: ET.Element) -> bytes:
    raw = ET.tostring(root, encoding="utf-8", xml_declaration=True)
    return fix_xml_decl_bytes(raw)


def fix_xml_decl_bytes(raw: bytes) -> bytes:
    return re.sub(
        br"<\?xml version='1\.0' encoding='utf-8'\?>",
        b'<?xml version="1.0" encoding="UTF-8" standalone="yes"?>',
        raw,
        count=1,
    )


def fix_xml_decl_text(text: str) -> str:
    return re.sub(
        r"<\?xml version='1\.0' encoding='utf-8'\?>",
        '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>',
        text,
        count=1,
    )


def normalize_xml_files(work: Path) -> None:
    """WPS requires double-quote XML declarations with standalone=yes on slide parts."""
    for xml_path in work.rglob("*.xml"):
        text = xml_path.read_text(encoding="utf-8")
        fixed = fix_xml_decl_text(text)
        if fixed != text:
            xml_path.write_text(fixed, encoding="utf-8")


def normalize_work_crlf(work: Path) -> None:
    for xml_path in work.rglob("*.xml"):
        text = xml_path.read_text(encoding="utf-8")
        fixed = text.replace("\r\n", "\n").replace("\r", "\n").replace("\n", "\r\n")
        if fixed != text:
            xml_path.write_text(fixed, encoding="utf-8")
    for rels_path in work.rglob("*.rels"):
        text = rels_path.read_text(encoding="utf-8")
        fixed = text.replace("\r\n", "\n").replace("\r", "\n").replace("\n", "\r\n")
        if fixed != text:
            rels_path.write_text(fixed, encoding="utf-8")


def remove_orphan_ins_parts(work: Path) -> None:
    """Remove unused *_ins* duplicate parts left from older pipeline runs."""
    referenced: set[str] = set()
    for rels_path in work.rglob("*.rels"):
        text = rels_path.read_text(encoding="utf-8")
        base = rels_path.parent.parent.as_posix()
        if "_rels" in rels_path.as_posix():
            base = str(Path(rels_path.as_posix().rsplit("/_rels/", 1)[0]))
        for target in re.findall(r'Target="([^"]+)"', text):
            if target.startswith("../"):
                resolved = (Path(base) / target).as_posix()
                while "/../" in resolved:
                    resolved = re.sub(r"[^/]+/\.\./", "", resolved, count=1)
                referenced.add(resolved.replace("\\", "/"))
            elif not target.startswith("http"):
                referenced.add(target.lstrip("/"))

    for ins_path in list(work.rglob("*_ins*")):
        if not ins_path.is_file():
            continue
        rel = ins_path.relative_to(work).as_posix()
        if rel not in referenced:
            ins_path.unlink()


def max_numbered_part(work: Path, subdir: str, prefix: str) -> int:
    part_dir = work / "ppt" / subdir
    if not part_dir.exists():
        return 0
    nums = []
    for part_file in part_dir.glob(f"{prefix}*.xml"):
        match = re.match(rf"{prefix}(\d+)\.xml$", part_file.name)
        if match:
            nums.append(int(match.group(1)))
    return max(nums) if nums else 0


def allocate_unique_part_path(work: Path, rel_path: str) -> str:
    rel_path = rel_path.replace("\\", "/")
    part = Path(rel_path)
    folder = part.parent.name
    match = re.match(r"(notesSlide|tag)(\d+)\.xml$", part.name)
    if match:
        prefix = match.group(1)
        next_num = max_numbered_part(work, folder, prefix) + 1
        return f"ppt/{folder}/{prefix}{next_num}.xml"
    base = part.stem
    suffix = 1
    while (work / part.parent / f"{base}_ins{suffix}.xml").exists():
        suffix += 1
    return str(part.parent / f"{base}_ins{suffix}.xml").replace("\\", "/")


def part_to_rels_target(part_path: str) -> str:
    part_path = part_path.replace("\\", "/")
    if part_path.startswith("ppt/"):
        return "../" + part_path[4:]
    return part_path


def copy_part_from_std(
    work: Path,
    std_work: Path,
    rel_path: str,
    *,
    unique_if_exists: bool = False,
) -> str:
    rel_path = rel_path.replace("\\", "/")
    src = std_work / rel_path
    if not src.exists():
        return rel_path
    dst = work / rel_path
    if dst.exists():
        if "ppt/media/" in rel_path or "ppt/slideLayouts/" in rel_path or "ppt/slideMasters/" in rel_path:
            return rel_path
        if unique_if_exists:
            rel_path = allocate_unique_part_path(work, rel_path)
            dst = work / rel_path
    dst.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(src, dst)
    ensure_content_type_from_std(work, std_work, "/" + rel_path)
    return rel_path


def shift_notes_slides_up(work: Path, from_slide: int) -> None:
    """Make room for inserted slide's notesSlide1 by shifting existing notes up by 1."""
    notes_dir = work / "ppt" / "notesSlides"
    if not notes_dir.exists():
        return
    max_n = max_numbered_part(work, "notesSlides", "notesSlide")
    if max_n == 0:
        return

    notes_rels_dir = notes_dir / "_rels"
    ct_path = work / "[Content_Types].xml"
    ct_text = ct_path.read_text(encoding="utf-8")
    for n in range(max_n, 0, -1):
        src = notes_dir / f"notesSlide{n}.xml"
        if src.exists():
            shutil.move(str(src), str(notes_dir / f"notesSlide{n + 1}.xml"))
        src_rels = notes_rels_dir / f"notesSlide{n}.xml.rels"
        if src_rels.exists():
            shutil.move(str(src_rels), str(notes_rels_dir / f"notesSlide{n + 1}.xml.rels"))
        ct_text = ct_text.replace(
            f'/ppt/notesSlides/notesSlide{n}.xml"',
            f'/ppt/notesSlides/notesSlide{n + 1}.xml"',
        )
    ct_path.write_text(ct_text, encoding="utf-8")

    slides_rels_dir = work / "ppt" / "slides" / "_rels"
    max_slide = max_slide_num(work)
    for slide_num in range(1, max_slide + 1):
        rel_path = slides_rels_dir / f"slide{slide_num}.xml.rels"
        if not rel_path.exists():
            continue
        text = rel_path.read_text(encoding="utf-8")
        for n in range(max_n, 0, -1):
            text = text.replace(
                f"../notesSlides/notesSlide{n}.xml",
                f"../notesSlides/notesSlide{n + 1}.xml",
            )
        rel_path.write_text(text, encoding="utf-8")


def copy_related_parts(work: Path, std_work: Path, rels_file: Path, *, unique_if_exists: bool = False) -> None:
    if not rels_file.exists():
        return
    tree = ET.parse(rels_file)
    copied: set[str] = set()

    def copy_once(rel_path: str) -> str:
        if rel_path in copied:
            return rel_path
        copied.add(rel_path)
        return copy_part_from_std(work, std_work, rel_path, unique_if_exists=unique_if_exists)

    changed = False
    for rel in tree.getroot():
        target = rel.get("Target", "")
        if not target.startswith("../"):
            continue
        part = "ppt/" + target.replace("../", "", 1)
        final_part = copy_once(part)
        if final_part != part:
            rel.set("Target", part_to_rels_target(final_part))
            changed = True
        if final_part.endswith(".xml"):
            nested_std = std_work / Path(part).parent / "_rels" / (Path(part).name + ".rels")
            if nested_std.exists():
                final_nested = str(Path(final_part).parent / "_rels" / (Path(final_part).name + ".rels")).replace("\\", "/")
                if final_nested not in copied:
                    dst_nested = work / final_nested
                    dst_nested.parent.mkdir(parents=True, exist_ok=True)
                    shutil.copy2(nested_std, dst_nested)
                    copied.add(final_nested)
                nested_tree = ET.parse(nested_std)
                for nested_rel in nested_tree.getroot():
                    nested_target = nested_rel.get("Target", "")
                    if nested_target.startswith("../"):
                        nested_rel_part = str(Path(final_part).parent / nested_target.replace("../", "", 1)).replace("\\", "/")
                        copy_once(nested_rel_part)
    if changed:
        tree.write(rels_file, encoding="UTF-8", xml_declaration=True)


def remove_content_type(work: Path, part_name: str) -> None:
    ct_path = work / "[Content_Types].xml"
    tree = ET.parse(ct_path)
    root = tree.getroot()
    for node in list(root):
        if node.get("PartName") == part_name:
            root.remove(node)
    tree.write(ct_path, encoding="UTF-8", xml_declaration=True)


def remove_handout_masters(work: Path) -> None:
    """Remove handout masters — WPS-saved unit-1 final has none; std template has none."""
    hm_dir = work / "ppt" / "handoutMasters"
    if hm_dir.exists():
        shutil.rmtree(hm_dir)

    pres_path = work / "ppt" / "presentation.xml"
    pres_text = pres_path.read_text(encoding="utf-8")
    pres_text = re.sub(
        r"<p:handoutMasterIdLst>.*?</p:handoutMasterIdLst>\s*",
        "",
        pres_text,
        count=1,
        flags=re.DOTALL,
    )
    pres_path.write_text(pres_text, encoding="utf-8")

    rels_path = work / "ppt" / "_rels" / "presentation.xml.rels"
    rels_tree = ET.parse(rels_path)
    rels_root = rels_tree.getroot()
    for rel in list(rels_root):
        if "handoutMaster" in rel.get("Type", ""):
            rels_root.remove(rel)
    rels_tree.write(rels_path, encoding="UTF-8", xml_declaration=True)

    ct_path = work / "[Content_Types].xml"
    ct_tree = ET.parse(ct_path)
    ct_root = ct_tree.getroot()
    for node in list(ct_root):
        part = node.get("PartName", "")
        if part.startswith("/ppt/handoutMasters/"):
            ct_root.remove(node)
    ct_tree.write(ct_path, encoding="UTF-8", xml_declaration=True)


def remove_wps_legacy_tags(work: Path) -> None:
    """Match WPS manual save on unit-1: remove tag1/tag2 (std) and tag15 (presentation)."""
    rel_edits = {
        work / "ppt" / "_rels" / "presentation.xml.rels": ("tags/tag15.xml",),
        work / "ppt" / "slideMasters" / "_rels" / "slideMaster1.xml.rels": ("../tags/tag2.xml",),
        work / "ppt" / "slideLayouts" / "_rels" / "slideLayout3.xml.rels": ("../tags/tag1.xml",),
    }
    for rels_path, drop_targets in rel_edits.items():
        if not rels_path.exists():
            continue
        tree = ET.parse(rels_path)
        root = tree.getroot()
        changed = False
        for rel in list(root):
            if rel.get("Target", "") in drop_targets:
                root.remove(rel)
                changed = True
        if changed:
            tree.write(rels_path, encoding="UTF-8", xml_declaration=True)

    for tag in ("tag1.xml", "tag2.xml", "tag15.xml"):
        tag_path = work / "ppt" / "tags" / tag
        if tag_path.exists():
            tag_path.unlink()
            remove_content_type(work, f"/ppt/tags/{tag}")


def remove_orphan_notes_and_tags(work: Path) -> None:
    """Drop notes/tags not referenced by any .rels (incl. presentation/masters)."""
    referenced_notes: set[str] = set()
    referenced_tags: set[str] = set()
    for rels_path in work.rglob("*.rels"):
        text = rels_path.read_text(encoding="utf-8")
        referenced_notes.update(re.findall(r"notesSlides/([^\"]+)", text))
        referenced_tags.update(re.findall(r"tags/([^\"]+)", text))

    notes_dir = work / "ppt" / "notesSlides"
    if notes_dir.exists():
        for notes_file in list(notes_dir.glob("notesSlide*.xml")):
            if notes_file.name not in referenced_notes:
                notes_file.unlink()
                rels = notes_dir / "_rels" / f"{notes_file.name}.rels"
                if rels.exists():
                    rels.unlink()
                remove_content_type(work, f"/ppt/notesSlides/{notes_file.name}")

    tags_dir = work / "ppt" / "tags"
    if tags_dir.exists():
        for tag_file in list(tags_dir.glob("tag*.xml")):
            if tag_file.name not in referenced_tags:
                tag_file.unlink()
                remove_content_type(work, f"/ppt/tags/{tag_file.name}")

    prune_stale_relationships(work)


def prune_stale_relationships(work: Path) -> None:
    """Remove Relationship entries whose Target part no longer exists."""
    for rels_path in work.rglob("*.rels"):
        if "_rels" in rels_path.as_posix():
            base = rels_path.parent.parent
        else:
            base = rels_path.parent
        tree = ET.parse(rels_path)
        root = tree.getroot()
        changed = False
        for rel in list(root):
            target = rel.get("Target", "")
            if not target or target.startswith("http"):
                continue
            resolved = (base / target).as_posix().replace("\\", "/")
            while "/../" in resolved:
                resolved = re.sub(r"[^/]+/\.\./", "", resolved, count=1)
            part = work / resolved
            if not part.exists():
                root.remove(rel)
                changed = True
        if changed:
            tree.write(rels_path, encoding="UTF-8", xml_declaration=True)


ZIP_DIRECTORY_ENTRIES = (
    "_rels/",
    "docProps/",
    "ppt/",
    "ppt/_rels/",
    "ppt/media/",
    "ppt/notesMasters/",
    "ppt/notesMasters/_rels/",
    "ppt/notesSlides/",
    "ppt/notesSlides/_rels/",
    "ppt/slideLayouts/",
    "ppt/slideLayouts/_rels/",
    "ppt/slideMasters/",
    "ppt/slideMasters/_rels/",
    "ppt/slides/",
    "ppt/slides/_rels/",
    "ppt/tags/",
    "ppt/theme/",
)


def ensure_content_type(work: Path, part_name: str, content_type: str) -> None:
    ct_path = work / "[Content_Types].xml"
    text = ct_path.read_text(encoding="utf-8")
    if part_name in text:
        return
    insert = f'<Override PartName="{part_name}" ContentType="{content_type}"/>'
    if "</Types>" in text:
        text = text.replace("</Types>", insert + "</Types>", 1)
    else:
        text = text.replace("</ns0:Types>", insert + "</ns0:Types>", 1)
    ct_path.write_text(text, encoding="utf-8")


def ensure_content_type_from_std(work: Path, std_work: Path, part_name: str) -> None:
    std_ct = ET.parse(std_work / "[Content_Types].xml")
    for node in std_ct.getroot():
        if node.get("PartName") == part_name:
            ensure_content_type(work, part_name, node.get("ContentType", ""))
            return
    ext = Path(part_name).suffix.lstrip(".").lower()
    for node in std_ct.getroot():
        if node.tag.endswith("Default") and node.get("Extension", "").lower() == ext:
            return


def insert_fixed_standard_slide(work: Path, std_work: Path, insert_at: int, std_slide: int) -> None:
    shift_slides_up(work, insert_at)
    shift_notes_slides_up(work, insert_at)

    std_slide_xml = std_work / "ppt" / "slides" / f"slide{std_slide}.xml"
    std_slide_rels = std_work / "ppt" / "slides" / "_rels" / f"slide{std_slide}.xml.rels"
    dst_slide = work / "ppt" / "slides" / f"slide{insert_at}.xml"
    dst_rels = work / "ppt" / "slides" / "_rels" / f"slide{insert_at}.xml.rels"
    shutil.copy2(std_slide_xml, dst_slide)
    shutil.copy2(std_slide_rels, dst_rels)
    copy_related_parts(work, std_work, dst_rels, unique_if_exists=True)

    ensure_content_type(work, f"/ppt/slides/slide{insert_at}.xml", "application/vnd.openxmlformats-officedocument.presentationml.slide+xml")

    rels_path = work / "ppt" / "_rels" / "presentation.xml.rels"
    rels_tree = ET.parse(rels_path)
    rels_root = rels_tree.getroot()
    max_rid = max(int(rel.get("Id", "rId0")[3:]) for rel in rels_root if rel.get("Id", "").startswith("rId"))
    new_rid = f"rId{max_rid + 1}"
    ET.SubElement(
        rels_root,
        "Relationship",
        {
            "Id": new_rid,
            "Type": "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide",
            "Target": f"slides/slide{insert_at}.xml",
        },
    )
    rels_tree.write(rels_path, encoding="UTF-8", xml_declaration=True)
    rebuild_sld_id_list(work)


def replace_with_standard_slide(work: Path, std_work: Path, target_num: int, std_slide: int) -> None:
    """Overwrite an existing slide with a standard-template slide (no renumbering)."""
    std_slide_xml = std_work / "ppt" / "slides" / f"slide{std_slide}.xml"
    std_slide_rels = std_work / "ppt" / "slides" / "_rels" / f"slide{std_slide}.xml.rels"
    dst_slide = work / "ppt" / "slides" / f"slide{target_num}.xml"
    dst_rels = work / "ppt" / "slides" / "_rels" / f"slide{target_num}.xml.rels"
    shutil.copy2(std_slide_xml, dst_slide)
    shutil.copy2(std_slide_rels, dst_rels)
    copy_related_parts(work, std_work, dst_rels, unique_if_exists=True)
    ensure_content_type(
        work,
        f"/ppt/slides/slide{target_num}.xml",
        "application/vnd.openxmlformats-officedocument.presentationml.slide+xml",
    )


def rebuild_sld_id_list(work: Path) -> None:
    pres_path = work / "ppt" / "presentation.xml"
    rels_path = work / "ppt" / "_rels" / "presentation.xml.rels"
    pres_text = pres_path.read_text(encoding="utf-8")
    rels_tree = ET.parse(rels_path)
    rels_root = rels_tree.getroot()

    old_ids = {rid: sid for sid, rid in re.findall(r'<p:sldId id="(\d+)" r:id="(rId\d+)"/>', pres_text)}
    slide_rels = []
    for rel in rels_root:
        if rel.get("Type", "").endswith("/slide"):
            slide_rels.append((rel.get("Id"), rel.get("Target")))
    slide_rels.sort(key=lambda x: int(re.search(r"slide(\d+)", x[1]).group(1)))

    next_id = max((int(v) for v in old_ids.values()), default=256) + 1
    entries = []
    for rid, _target in slide_rels:
        sid = old_ids.get(rid, str(next_id))
        if rid not in old_ids:
            next_id += 1
        entries.append(f'<p:sldId id="{sid}" r:id="{rid}"/>')

    pres_text = re.sub(
        r"<p:sldIdLst>.*?</p:sldIdLst>",
        "<p:sldIdLst>" + "".join(entries) + "</p:sldIdLst>",
        pres_text,
        count=1,
        flags=re.DOTALL,
    )
    pres_path.write_text(pres_text, encoding="utf-8")


def fix_namespaced_rels(path: Path) -> None:
    if not path.exists():
        return
    text = path.read_text(encoding="utf-8")
    if "ns0:" not in text:
        return
    text = text.replace("ns0:", "")
    text = text.replace(
        'xmlns:ns0="http://schemas.openxmlformats.org/package/2006/relationships"',
        'xmlns="http://schemas.openxmlformats.org/package/2006/relationships"',
    )
    text = text.replace("<?xml version='1.0' encoding='UTF-8'?>", '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>')
    path.write_text(text, encoding="utf-8")


def fix_package_namespaces(work: Path) -> None:
    ct_path = work / "[Content_Types].xml"
    text = ct_path.read_text(encoding="utf-8")
    text = text.replace("ns0:", "")
    text = text.replace(
        'xmlns:ns0="http://schemas.openxmlformats.org/package/2006/content-types"',
        'xmlns="http://schemas.openxmlformats.org/package/2006/content-types"',
    )
    ct_path.write_text(text, encoding="utf-8")
    for rels in work.rglob("*.rels"):
        fix_namespaced_rels(rels)


def assert_no_shared_slide_parts(work: Path) -> None:
    slides_rels_dir = work / "ppt" / "slides" / "_rels"
    for needle in ("notesSlide", "/tags/"):
        counts: dict[str, int] = {}
        for rel_path in slides_rels_dir.glob("slide*.xml.rels"):
            text = rel_path.read_text(encoding="utf-8")
            for target in re.findall(r'Target="([^"]+)"', text):
                if needle in target:
                    counts[target] = counts.get(target, 0) + 1
        shared = [(target, count) for target, count in counts.items() if count > 1]
        if shared:
            raise RuntimeError(f"Shared {needle} references: {shared[:5]}")


def build_pptx(source_dir: Path, output_path: Path) -> None:
    """Pack work dir — include WPS-style directory entries like unit-1 final."""
    if output_path.exists():
        output_path.unlink()
    with zipfile.ZipFile(output_path, "w", zipfile.ZIP_DEFLATED) as zf:
        written: set[str] = set()
        for entry in ZIP_DIRECTORY_ENTRIES:
            zf.writestr(entry, b"")
            written.add(entry)
        for file_path in sorted(source_dir.rglob("*")):
            if file_path.is_file():
                arc = file_path.relative_to(source_dir).as_posix()
                if arc not in written:
                    zf.write(file_path, arc)
                    written.add(arc)


def apply_template_to(orig_path: Path, out_path: Path, std_path: Path) -> int:
    """Apply template from orig; write new pptx (step 1 of unit-1 pipeline)."""
    work = out_path.parent / f"_work_{orig_path.stem}"
    std_work = out_path.parent / "_work_std_batch"
    try:
        extract_zip(orig_path, work)
        if not std_work.exists():
            extract_zip(std_path, std_work)

        remove_template_parts(work)
        copy_template_parts(std_work, work)
        copy_media(std_work, work)
        remove_education_logos_from_masters(work)
        patch_presentation_xml(work, std_work)
        patch_presentation_rels(work)
        patch_content_types(work, std_work)
        process_slides(work)
        resolve_slide_media_conflicts(work)
        insert_fixed_standard_slide(work, std_work, FIXED_INSERT_AT, STD_FIXED_SLIDE)
        last_num = max_slide_num(work)
        replace_with_standard_slide(work, std_work, last_num, STD_LAST_SLIDE)
        normalize_xml_files(work)
        remove_orphan_ins_parts(work)
        remove_handout_masters(work)
        remove_orphan_notes_and_tags(work)
        remove_wps_legacy_tags(work)
        prune_stale_relationships(work)
        normalize_work_crlf(work)
        sync_content_type_defaults(work, std_work)
        fix_package_namespaces(work)
        assert_no_shared_slide_parts(work)
        verify_package_content_types(work)

        slide_count = max_slide_num(work)
        build_pptx(work, out_path)
        return slide_count
    finally:
        if work.exists():
            shutil.rmtree(work)
        if std_work.exists():
            shutil.rmtree(std_work)


def remove_education_logos_from_masters(work: Path) -> None:
    """Drop legacy top-right logo pictures from slide masters/layouts."""
    for xml_path in list(work.rglob("ppt/slideMasters/*.xml")) + list(work.rglob("ppt/slideLayouts/*.xml")):
        try:
            root = ET.parse(xml_path).getroot()
        except ET.ParseError:
            continue
        sp_tree = root.find(".//p:spTree", NS)
        if sp_tree is None:
            continue
        changed = False
        for child in list(sp_tree):
            if child.tag == f"{{{P_NS}}}pic" and is_old_logo_pic(child):
                sp_tree.remove(child)
                changed = True
        if changed:
            ET.register_namespace("p", P_NS)
            ET.register_namespace("a", A_NS)
            ET.register_namespace("r", R_NS)
            tree = ET.ElementTree(root)
            tree.write(xml_path, encoding="UTF-8", xml_declaration=True)


def apply_template() -> None:
    orig_path, std_path = find_files()
    out_path = PPT_DIR / OUTPUT_NAME

    temp_out = PPT_DIR / f"_new_{OUTPUT_NAME}"
    if temp_out.exists():
        temp_out.unlink()
    slide_count = apply_template_to(orig_path, temp_out, std_path)

    std_work = PPT_DIR / "_work_std_batch"
    if std_work.exists():
        shutil.rmtree(std_work)

    try:
        if out_path.exists():
            out_path.unlink()
        shutil.move(str(temp_out), str(out_path))
        print(f"Rebuilt: {out_path} ({slide_count} slides)")
    except PermissionError:
        print(f"Target file is open. Saved to: {temp_out} ({slide_count} slides)")
        print("Close WPS/PPT and rename the file, or run again.")


if __name__ == "__main__":
    apply_template()
