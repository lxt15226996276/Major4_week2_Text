# btn_Heart_1~4 — 参照 btn_技能图标2 四态规范（见 _gen_skill_btn_states.py）
from PIL import Image, ImageEnhance, ImageOps, ImageFilter, ImageDraw
import colorsys
import os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SKILL_REF = os.path.join(ROOT, "Assets", "Images", "003-taskother", "btn_技能图标2_1.png")
HEART_SRC = os.path.join(ROOT, "Assets", "UI", "Sprite", "Heart.png")
OUTPUT_DIR = os.path.join(ROOT, "Assets", "UI", "Sprite")

OUTPUT_SIZE = 256
RATIO_NORMAL = 1.00
RATIO_HOVER = 1.10
RATIO_PRESSED = 0.96
RATIO_DISABLED = 0.98
FILL_NORMAL = 0.60
FILL_HOVER = FILL_NORMAL * RATIO_HOVER
FILL_PRESSED = FILL_NORMAL * RATIO_PRESSED
FILL_DISABLED = FILL_NORMAL * RATIO_DISABLED
OFFSET_PRESSED_Y = 2


def rgba(img):
    return img.convert("RGBA") if img.mode != "RGBA" else img


def shift_yellow_to_red(ref):
    """将技能图标的黄色内圈改为治疗系红色光晕，保留金色外框。"""
    ref = rgba(ref)
    out = Image.new("RGBA", ref.size)
    px = ref.load()
    opx = out.load()
    w, h = ref.size
    cx, cy = w / 2, h / 2
    max_r = min(cx, cy)

    for y in range(h):
        for x in range(w):
            r, g, b, a = px[x, y]
            if a < 8:
                opx[x, y] = (0, 0, 0, 0)
                continue

            dist = ((x - cx) ** 2 + (y - cy) ** 2) ** 0.5
            norm = dist / max_r
            hsv = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
            hue, sat, val = hsv

            # 外圈金属框：保留原色
            if norm > 0.82:
                opx[x, y] = (r, g, b, a)
                continue

            # 中心黑色剪影：清空以便贴入红心
            if val < 0.22 and sat < 0.35 and norm < 0.55:
                opx[x, y] = (0, 0, 0, 0)
                continue

            # 内圈光晕：按半径混合为红/粉 radial glow
            glow = max(0.0, 1.0 - norm * 1.15)
            center_boost = max(0.0, 1.0 - norm * 2.2)
            nr = int(min(255, 90 + 120 * glow + 80 * center_boost))
            ng = int(min(255, 15 + 25 * glow + 20 * center_boost))
            nb = int(min(255, 25 + 35 * glow + 30 * center_boost))
            alpha = min(255, int(a * (0.75 + 0.25 * glow)))
            opx[x, y] = (nr, ng, nb, alpha)

    return out


def colorize_heart(heart):
    """将原始 Heart 图标着色为带高光的红色。"""
    heart = rgba(heart)
    alpha = heart.split()[3]
    gray = ImageOps.grayscale(heart)

    # 反转亮度：原图暗部变心形主体
    inverted = ImageOps.invert(gray)
    colored = ImageOps.colorize(inverted, black="#8B1020", white="#FF5070", mid="#E82040")
    colored = rgba(colored)

    # 左上高光
    w, h = colored.size
    highlight = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(highlight)
    draw.ellipse([w * 0.18, h * 0.12, w * 0.52, h * 0.48], fill=(255, 210, 220, 90))
    colored = Image.alpha_composite(colored, highlight)
    colored.putalpha(alpha)
    return colored


def paste_scaled(src, canvas_size, fill_ratio, offset_y=0):
    src = rgba(src)
    sw, sh = src.size
    target = int(canvas_size * fill_ratio)
    scale = target / max(sw, sh)
    nw, nh = max(1, int(sw * scale)), max(1, int(sh * scale))
    scaled = src.resize((nw, nh), Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", (canvas_size, canvas_size), (0, 0, 0, 0))
    x = (canvas_size - nw) // 2
    y = (canvas_size - nh) // 2 + offset_y
    canvas.paste(scaled, (x, y), scaled)
    return canvas


def add_glow(img, radius=6, strength=0.38, color=(255, 180, 190)):
    w, h = img.size
    alpha = img.split()[3]
    glow = alpha.filter(ImageFilter.GaussianBlur(radius))
    layer = Image.new("RGBA", (w, h), (*color, 0))
    layer.putalpha(glow)
    layer = ImageEnhance.Brightness(layer).enhance(strength)
    base = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    return Image.alpha_composite(Image.alpha_composite(base, layer), img)


def build_source_icon():
    ref = rgba(Image.open(SKILL_REF))
    heart = colorize_heart(Image.open(HEART_SRC))

    frame = shift_yellow_to_red(ref)
    size = ref.size[0]
    heart_target = int(size * 0.36)
    heart_scaled = heart.resize(
        (heart_target, heart_target),
        Image.Resampling.LANCZOS,
    )

    # 微光晕让红心更醒目
    heart_glow = add_glow(heart_scaled, radius=3, strength=0.55, color=(255, 100, 120))
    hx = (size - heart_target) // 2
    hy = (size - heart_target) // 2 + 2
    layer = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    layer.paste(heart_glow, (hx, hy), heart_glow)
    return Image.alpha_composite(frame, layer)


def make_states(src):
    s = OUTPUT_SIZE
    normal = paste_scaled(src, s, FILL_NORMAL)

    hi = paste_scaled(src, s, FILL_HOVER)
    hi = ImageEnhance.Brightness(hi).enhance(1.15)
    hi = ImageEnhance.Color(hi).enhance(1.12)
    hi = ImageEnhance.Contrast(hi).enhance(1.04)
    hi = add_glow(hi, radius=5, strength=0.35, color=(255, 200, 210))

    pressed = paste_scaled(src, s, FILL_PRESSED, offset_y=OFFSET_PRESSED_Y)
    pressed = ImageEnhance.Brightness(pressed).enhance(0.88)
    pressed = ImageEnhance.Contrast(pressed).enhance(1.06)

    dis = paste_scaled(src, s, FILL_DISABLED)
    r, g, b, a = dis.split()
    gray = ImageOps.grayscale(dis).split()[0]
    dis = Image.merge("RGBA", (gray, gray, gray, a))
    dis = ImageEnhance.Brightness(dis).enhance(0.70)

    return [normal, hi, pressed, dis]


def main():
    assert FILL_HOVER > FILL_NORMAL > FILL_PRESSED, "尺寸顺序错误"
    os.makedirs(OUTPUT_DIR, exist_ok=True)

    source = build_source_icon()
    for i, state in enumerate(make_states(source), start=1):
        out = os.path.join(OUTPUT_DIR, f"btn_Heart_{i}.png")
        state.save(out, "PNG")
        print("WROTE", out)


if __name__ == "__main__":
    main()
