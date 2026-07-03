"""Generate Exam02 MainRoom: large house interior with fixed material GUIDs & 3D props."""
import hashlib
import math
import os
import re

ROOT = os.path.dirname(os.path.abspath(__file__))
SCENES = os.path.join(ROOT, "Scenes")
MATERIALS = os.path.join(ROOT, "Materials")
TEXTURES = os.path.join(ROOT, "Textures")
MODELS_DIR = os.path.join(ROOT, "Models")

# ~960 sqm = 40m x 24m; ceiling = 4.5x player height (Unity default ~2m)
PLAYER_H = 2.0
RX, RZ = 20.0, 12.0
REF_RX, REF_RZ = RX, RZ
WALL_H = PLAYER_H * 4.5          # 9m — four+ player heights
DOOR_W, DOOR_H = 3.2, 3.0
WT = 0.25
CUBE = "{fileID: 10202, guid: 0000000000000000e000000000000000, type: 0}"
SHADER = "{fileID: 46, guid: 0000000000000000f000000000000000, type: 0}"

MODELS = {
    "Tables_Chairs": {
        "rel": "Models/OpenGameArtFurniture/Tables_Chairs.obj",
        "guid": "b2000000000000000000000000000001",
        "mesh_id": 4300000,
    },
    "Stand": {
        "rel": "../../Models/Mesh/Stand.obj",
        "guid": "6415a4464cfe176419955a2b1015d471",
        "mesh_id": 4300000,
    },
}

TEXTURE_FILES = [
    "Tex_WoodFloor.jpg",
    "Tex_Wall.jpg",
    "Tex_Carpet.jpg",
    "Tex_WoodCrate.jpg",
    "Tex_Brick.jpg",
    "Tex_Metal.jpg",
]

MAT_DEFS = {
    "Mat_WoodFloor.mat": ("Tex_WoodFloor.jpg", (12, 8), (1, 1, 1), 0.25),
    "Mat_Wallpaper.mat": ("Tex_Wall.jpg", (6, 3), (1, 1, 1), 0.15),
    "Mat_Carpet.mat": ("Tex_Carpet.jpg", (3, 2.5), (1, 1, 1), 0.1),
    "Mat_WoodFurniture.mat": ("Tex_WoodCrate.jpg", (2, 2), (1, 1, 1), 0.3),
    "Mat_Brick.mat": ("Tex_Brick.jpg", (2, 2), (1, 1, 1), 0.2),
    "Mat_Metal.mat": ("Tex_Metal.jpg", (1, 1), (0.9, 0.9, 0.95), 0.6),
    "Mat_Ceiling.mat": (None, (1, 1), (0.96, 0.94, 0.9), 0.05),
    "Mat_Treasure.mat": ("Tex_Metal.jpg", (1, 1), (0.85, 0.65, 0.15), 0.7),
    "Mat_Glass.mat": (None, (1, 1), (0.5, 0.72, 0.88), 0.85),
    "Mat_Picture.mat": (None, (1, 1), (0.55, 0.35, 0.2), 0.2),
    "Mat_Plant.mat": (None, (1, 1), (0.2, 0.55, 0.25), 0.15),
    "Mat_Sofa.mat": (None, (1, 1), (0.45, 0.22, 0.18), 0.2),
    "Mat_SofaCushion.mat": (None, (1, 1), (0.62, 0.28, 0.22), 0.15),
    "Mat_Door.mat": ("Tex_WoodCrate.jpg", (1, 2), (0.4, 0.26, 0.14), 0.25),
    "Mat_Wainscot.mat": ("Tex_WoodCrate.jpg", (4, 1), (0.35, 0.22, 0.12), 0.3),
}


def read_guid(meta_path):
    if not os.path.exists(meta_path):
        return None
    with open(meta_path, encoding="utf-8") as f:
        for line in f:
            if line.startswith("guid: "):
                return line.split()[1].strip()
    return None


def stable_guid(key):
    return hashlib.md5(key.encode("utf-8")).hexdigest()


def load_texture_guids():
    guids = {}
    for fname in TEXTURE_FILES:
        g = read_guid(os.path.join(TEXTURES, fname + ".meta"))
        guids[fname] = g or stable_guid("Exam02Tex:" + fname)
    return guids


def load_mat_guids():
    guids = {}
    for name in MAT_DEFS:
        g = read_guid(os.path.join(MATERIALS, name + ".meta"))
        guids[name] = g or stable_guid("Exam02Mat:" + name)
    return guids


TEXTURE_GUIDS = load_texture_guids()
MAT_GUIDS = load_mat_guids()


def scaled_pos(pos):
    return (pos[0] * RX / REF_RX, pos[1], pos[2] * RZ / REF_RZ)


def scaled_scale(scale):
    return (scale[0] * RX / REF_RX, scale[1], scale[2] * RZ / REF_RZ)


def tex_ref(filename):
    return f"{{fileID: 2800000, guid: {TEXTURE_GUIDS[filename]}, type: 3}}"


def mat_ref(name):
    return f"{{fileID: 2100000, guid: {MAT_GUIDS[name]}, type: 2}}"


def mesh_ref(model_key):
    m = MODELS[model_key]
    return f"{{fileID: {m['mesh_id']}, guid: {m['guid']}, type: 3}}"


def quat_y(degrees):
    half = math.radians(degrees) * 0.5
    return 0.0, math.sin(half), 0.0, math.cos(half)


def normalize_quat(x, y, z, w):
    mag = math.sqrt(x * x + y * y + z * z + w * w)
    if mag < 1e-8:
        return 0.0, 0.0, 0.0, 1.0
    return x / mag, y / mag, z / mag, w / mag


def write_texture_meta():
    tpl_path = os.path.join(TEXTURES, "Tex_WoodFloor.jpg.meta")
    if not os.path.exists(tpl_path):
        return
    tpl = open(tpl_path, encoding="utf-8").read()
    for fname, guid in TEXTURE_GUIDS.items():
        path = os.path.join(TEXTURES, fname)
        if not os.path.exists(path):
            continue
        meta = re.sub(r"^guid: .+$", f"guid: {guid}", tpl, count=1, flags=re.M)
        with open(path + ".meta", "w", encoding="utf-8") as f:
            f.write(meta)


def material_yaml(name, tex_file, tile, color, gloss):
    tex_block = ""
    if tex_file:
        tx, ty = tile
        tex_block = f"""    - _MainTex:
        m_Texture: {tex_ref(tex_file)}
        m_Scale: {{x: {tx}, y: {ty}}}
        m_Offset: {{x: 0, y: 0}}"""
    else:
        tex_block = """    - _MainTex:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}"""
    r, g, b = color
    return f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_Name: {name.replace('.mat', '')}
  m_Shader: {SHADER}
  m_ValidKeywords: []
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {{}}
  disabledShaderPasses: []
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
{tex_block}
    m_Floats:
    - _Glossiness: {gloss}
    - _Metallic: 0
    m_Colors:
    - _Color: {{r: {r}, g: {g}, b: {b}, a: 1}}
  m_BuildTextureStacks: []
"""


class IdGen:
    def __init__(self, n=100001):
        self.n = n

    def next(self):
        v = self.n
        self.n += 1
        return v


ids = IdGen()
objects = []


def make_empty(name, pos, parent=0):
    gid, tid = ids.next(), ids.next()
    objects.append(dict(gid=gid, tid=tid, name=name, tag="Untagged", pos=scaled_pos(pos),
                        scale=(1, 1, 1), parent=parent, kind="empty", rot=(0, 0, 0, 1)))
    return tid


def make_cube(name, pos, scale, mat, parent=0, tag="Untagged", trigger=False, collider=True):
    gid, tid = ids.next(), ids.next()
    mf, mr = ids.next(), ids.next()
    bc = ids.next() if collider else None
    objects.append(dict(gid=gid, tid=tid, mf=mf, mr=mr, bc=bc, name=name, tag=tag,
                        pos=scaled_pos(pos), scale=scaled_scale(scale), parent=parent, kind="cube",
                        trigger=trigger, mat=mat, collider=collider, rot=(0, 0, 0, 1)))
    return tid


def make_model(name, pos, scale, mat, model_key, parent=0, tag="Untagged", rot_y=0, collider=True):
    gid, tid = ids.next(), ids.next()
    mf, mr = ids.next(), ids.next()
    bc = ids.next() if collider else None
    qx, qy, qz, qw = quat_y(rot_y)
    objects.append(dict(gid=gid, tid=tid, mf=mf, mr=mr, bc=bc, name=name, tag=tag,
                        pos=scaled_pos(pos), scale=scale, parent=parent, kind="model",
                        mat=mat, collider=collider, model_key=model_key, rot=(qx, qy, qz, qw)))
    return tid


def build_house():
    house = make_empty("House", (0, 0, 0))
    struct = make_empty("Structure", (0, 0, 0), house)
    decor = make_empty("Decoration", (0, 0, 0), house)
    furn = make_empty("Furniture", (0, 0, 0), house)
    wh2 = WALL_H / 2

    make_cube("WoodFloor", (0, 0, 0), (RX * 2, 0.12, RZ * 2), "Mat_WoodFloor.mat", house)
    make_cube("Ceiling", (0, WALL_H + 0.06, 0), (RX * 2, 0.1, RZ * 2), "Mat_Ceiling.mat", house, collider=False)

    beam_xs = [-RX * 0.75, -RX * 0.45, -RX * 0.15, RX * 0.15, RX * 0.45, RX * 0.75]
    for i, x in enumerate(beam_xs):
        make_cube(f"CeilingBeam_{i+1}", (x, WALL_H - 0.05, 0), (0.22, 0.14, RZ * 2 - 0.8),
                  "Mat_WoodFurniture.mat", decor, collider=False)

    make_cube("Wall_North", (0, wh2, RZ), (RX * 2, WALL_H, WT), "Mat_Wallpaper.mat", struct)
    make_cube("Wall_West", (-RX, wh2, 0), (WT, WALL_H, RZ * 2), "Mat_Wallpaper.mat", struct)
    make_cube("Wall_East", (RX, wh2, 0), (WT, WALL_H, RZ * 2), "Mat_Wallpaper.mat", struct)

    gap = DOOR_W / 2
    lw = RX - gap
    make_cube("Wall_South_L", (-(gap + lw / 2), wh2, -RZ), (lw, WALL_H, WT), "Mat_Wallpaper.mat", struct)
    make_cube("Wall_South_R", (gap + lw / 2, wh2, -RZ), (lw, WALL_H, WT), "Mat_Wallpaper.mat", struct)
    make_cube("DoorFrame_Top", (0, DOOR_H + (WALL_H - DOOR_H) / 2, -RZ + 0.04),
              (DOOR_W + 0.4, WALL_H - DOOR_H, WT + 0.08), "Mat_Door.mat", struct, collider=False)
    make_cube("DoorFrame_L", (-DOOR_W / 2 - 0.1, DOOR_H / 2, -RZ + 0.04),
              (0.14, DOOR_H, WT + 0.08), "Mat_Door.mat", struct, collider=False)
    make_cube("DoorFrame_R", (DOOR_W / 2 + 0.1, DOOR_H / 2, -RZ + 0.04),
              (0.14, DOOR_H, WT + 0.08), "Mat_Door.mat", struct, collider=False)
    make_cube("Door_Panel", (0, DOOR_H / 2, -RZ + 0.15), (DOOR_W - 0.2, DOOR_H - 0.15, 0.1),
              "Mat_Door.mat", struct, collider=False)

    win_cy = WALL_H * 0.42
    win_h = WALL_H * 0.32
    win_zs = [-RZ * 0.35, -RZ * 0.05, RZ * 0.25]
    for side, x, name in [("E", RX - 0.05, "East"), ("W", -RX + 0.05, "West")]:
        for zi, z in enumerate(win_zs):
            make_cube(f"Window_{name}_{zi+1}_Glass", (x, win_cy, z), (0.06, win_h, 2.4),
                      "Mat_Glass.mat", struct, collider=False)
            make_cube(f"Window_{name}_{zi+1}_FrameT", (x, win_cy + win_h * 0.55, z), (0.1, 0.12, 2.6),
                      "Mat_Door.mat", struct, collider=False)
            make_cube(f"Window_{name}_{zi+1}_FrameB", (x, win_cy - win_h * 0.55, z), (0.1, 0.12, 2.6),
                      "Mat_Door.mat", struct, collider=False)

    for name, pos, scale in [
        ("Wainscot_N", (0, 0.6, RZ - 0.14), (RX * 2 - 1, 1.2, 0.08)),
        ("Wainscot_S_L", (-(gap + lw / 2), 0.6, -RZ + 0.14), (lw, 1.2, 0.08)),
        ("Wainscot_S_R", (gap + lw / 2, 0.6, -RZ + 0.14), (lw, 1.2, 0.08)),
        ("Wainscot_W", (-RX + 0.14, 0.6, 0), (0.08, 1.2, RZ * 2 - 1)),
        ("Wainscot_E", (RX - 0.14, 0.6, 0), (0.08, 1.2, RZ * 2 - 1)),
    ]:
        make_cube(name, pos, scale, "Mat_Wainscot.mat", decor, collider=False)

    fp_z = RZ - 0.45
    make_cube("Fireplace_Base", (0, 0.45, fp_z), (3.0, 0.9, 0.6), "Mat_Brick.mat", decor, collider=False)
    make_cube("Fireplace_Column_L", (-1.2, WALL_H * 0.22, fp_z), (0.45, WALL_H * 0.44, 0.55), "Mat_Brick.mat", decor, collider=False)
    make_cube("Fireplace_Column_R", (1.2, WALL_H * 0.22, fp_z), (0.45, WALL_H * 0.44, 0.55), "Mat_Brick.mat", decor, collider=False)
    make_cube("Fireplace_Mantle", (0, WALL_H * 0.48, fp_z + 0.05), (3.4, 0.18, 0.4),
              "Mat_WoodFurniture.mat", decor, collider=False)

    pic_y = WALL_H * 0.48
    frames = [(-RX * 0.55, pic_y, fp_z - 0.2), (RX * 0.55, pic_y, fp_z - 0.2),
              (-RX + 0.2, pic_y, -RZ * 0.35), (-RX + 0.2, pic_y, RZ * 0.3),
              (RX - 0.2, pic_y, -RZ * 0.45), (RX - 0.2, pic_y, RZ * 0.2)]
    for i, (x, y, z) in enumerate(frames):
        make_cube(f"Picture_{i+1}", (x, y, z), (1.2, 0.9, 0.05), "Mat_Picture.mat", decor, collider=False)

    make_cube("Rug_Living", (-RX * 0.35, 0.07, RZ * 0.25), (RX * 0.55, 0.03, RZ * 0.45), "Mat_Carpet.mat", decor, collider=False)
    make_cube("Rug_Center", (RX * 0.35, 0.07, -RZ * 0.15), (RX * 0.45, 0.03, RZ * 0.4), "Mat_Carpet.mat", decor, collider=False)
    make_cube("Rug_Entry", (0, 0.07, -RZ * 0.55), (RX * 0.35, 0.03, RZ * 0.25), "Mat_Carpet.mat", decor, collider=False)

    make_cube("Sofa_Base", (-RX * 0.62, 0.35, RZ * 0.55), (RX * 0.35, 0.7, RZ * 0.14), "Mat_Sofa.mat", furn, collider=False)
    make_cube("Sofa_Back", (-RX * 0.62, 0.75, RZ * 0.68), (RX * 0.35, 0.5, RZ * 0.04), "Mat_Sofa.mat", furn, collider=False)
    make_cube("Sofa_Side", (-RX * 0.78, 0.35, RZ * 0.35), (RX * 0.07, 0.7, RZ * 0.28), "Mat_Sofa.mat", furn, collider=False)
    make_cube("Sofa_Cushion", (-RX * 0.62, 0.72, RZ * 0.55), (RX * 0.3, 0.15, RZ * 0.12), "Mat_SofaCushion.mat", furn, collider=False)

    make_model("DiningSet", (RX * 0.65, 0.0, RZ * 0.55), (1.0, 1.0, 1.0), "Mat_WoodFurniture.mat",
               "Tables_Chairs", furn, rot_y=90, collider=False)
    make_model("SideTable_Set", (-RX * 0.3, 0.0, RZ * 0.62), (0.8, 0.8, 0.8), "Mat_WoodFurniture.mat",
               "Tables_Chairs", furn, rot_y=-30, collider=False)

    make_cube("Bookshelf_L", (-RX * 0.82, 1.2, -RZ * 0.35), (0.55, 2.4, RZ * 0.28), "Mat_WoodFurniture.mat", furn, collider=False)
    make_cube("Bookshelf_R", (RX * 0.82, 1.2, RZ * 0.35), (0.55, 2.4, RZ * 0.28), "Mat_WoodFurniture.mat", furn, collider=False)
    make_model("DisplayStand", (RX * 0.78, 0.0, -RZ * 0.28), (1.8, 1.8, 1.8), "Mat_Metal.mat", "Stand", decor, collider=False)

    plant_corners = [(-RX * 0.78, RZ * 0.72), (RX * 0.78, -RZ * 0.72), (RX * 0.35, RZ * 0.72),
                     (-RX * 0.42, -RZ * 0.72), (-RX * 0.78, -RZ * 0.45), (RX * 0.78, RZ * 0.45)]
    for i, (x, z) in enumerate(plant_corners):
        make_cube(f"PlantPot_{i+1}", (x, 0.25, z), (0.5, 0.55, 0.5), "Mat_Brick.mat", decor, collider=False)
        make_cube(f"Plant_{i+1}", (x, 0.85, z), (0.8, 0.7, 0.8), "Mat_Plant.mat", decor, collider=False)

    for x in [-RX * 0.42, RX * 0.42]:
        make_cube(f"Column_{int(x)}", (x, WALL_H * 0.48, 0), (0.55, WALL_H * 0.96, 0.55), "Mat_Wallpaper.mat", decor, collider=False)

    crates = [
        ("Crate_01", (-RX * 0.55, 0.5, -RZ * 0.25), (1, 1, 1)),
        ("Crate_02", (RX * 0.55, 0.55, RZ * 0.12), (1.1, 1.1, 1.1)),
        ("Crate_03", (RX * 0.4, 0.45, -RZ * 0.55), (0.9, 0.9, 1.3)),
        ("Crate_04", (-RX * 0.48, 0.5, -RZ * 0.62), (1.2, 1.2, 1.2)),
        ("Crate_05", (RX * 0.62, 0.4, -RZ * 0.12), (1, 1.4, 1)),
        ("Crate_06", (-RX * 0.32, 0.5, RZ * 0.42), (1.3, 1, 1)),
        ("Barrel_01", (RX * 0.25, 0.55, -RZ * 0.38), (0.75, 1.1, 0.75)),
        ("Barrel_02", (-RX * 0.68, 0.55, RZ * 0.12), (0.75, 1.1, 0.75)),
    ]
    for name, pos, scale in crates:
        make_cube(name, pos, scale, "Mat_WoodFurniture.mat", furn)

    chest = make_empty("TreasureChest", (0, 0, 0), house)
    make_model("ChestPedestal", (0, 0.0, 0.0), (0.9, 0.9, 0.9), "Mat_Metal.mat", "Stand", chest, collider=False)
    make_cube("ChestBody", (0, 0.55, 0), (1.4, 0.8, 1.0), "Mat_Treasure.mat", chest, "Treasure", True)
    make_cube("ChestLid", (0, 1.03, -0.08), (1.45, 0.2, 0.95), "Mat_Metal.mat", chest, collider=False)
    make_cube("ChestBand", (0, 0.55, 0.52), (1.42, 0.15, 0.08), "Mat_Metal.mat", chest, collider=False)

    for cx, cz in [(0, 0), (-RX * 0.45, RZ * 0.3), (RX * 0.45, -RZ * 0.25)]:
        tag = "" if cx == 0 else f"_{int(abs(cx))}"
        make_cube(f"Chandelier{tag}", (cx, WALL_H - 0.3, cz), (1.0, 0.25, 1.0), "Mat_Metal.mat", decor, collider=False)
        make_cube(f"Chandelier_Chain{tag}", (cx, WALL_H - 1.2, cz), (0.1, 1.0, 0.1), "Mat_Metal.mat", decor, collider=False)
    make_empty("PlayerSpawn", (0, 0, -RZ * 0.65), house)
    return house


build_house()

light_gid, light_comp, light_tid = ids.next(), ids.next(), ids.next()
room1_gid, room1_comp, room1_tid = ids.next(), ids.next(), ids.next()
room2_gid, room2_comp, room2_tid = ids.next(), ids.next(), ids.next()
fire_gid, fire_comp, fire_tid = ids.next(), ids.next(), ids.next()
cam_gid, cam_comp, cam_audio, cam_tid = ids.next(), ids.next(), ids.next(), ids.next()

child_map = {}
for o in objects:
    child_map.setdefault(o["parent"], []).append(o["tid"])
roots = [o["tid"] for o in objects if o["parent"] == 0] + [light_tid, room1_tid, room2_tid, fire_tid, cam_tid]

SCENE_HEADER = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_AmbientSkyColor: {r: 0.28, g: 0.24, b: 0.22, a: 1}
  m_AmbientEquatorColor: {r: 0.2, g: 0.17, b: 0.15, a: 1}
  m_AmbientGroundColor: {r: 0.12, g: 0.1, b: 0.09, a: 1}
  m_AmbientIntensity: 1.15
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 0.8
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 0}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 1
    m_EnableRealtimeLightmaps: 0
  m_LightmapEditorSettings:
    serializedVersion: 12
    m_Resolution: 2
    m_BakeResolution: 40
    m_AtlasSize: 1024
    m_AO: 0
    m_AOMaxDistance: 1
    m_CompAOExponent: 1
    m_CompAOExponentDirect: 0
    m_ExtractAmbientOcclusion: 0
    m_Padding: 2
    m_LightmapParameters: {fileID: 0}
    m_LightmapsBakeMode: 1
    m_TextureCompression: 1
    m_FinalGather: 0
    m_FinalGatherFiltering: 1
    m_FinalGatherRayCount: 256
    m_ReflectionCompression: 2
    m_MixedBakeMode: 2
    m_BakeBackend: 1
    m_PVRSampling: 1
    m_PVRDirectSampleCount: 32
    m_PVRSampleCount: 512
    m_PVRBounces: 2
    m_PVREnvironmentSampleCount: 256
    m_PVREnvironmentReferencePointCount: 2048
    m_PVRFilteringMode: 1
    m_PVRDenoiserTypeDirect: 1
    m_PVRDenoiserTypeIndirect: 1
    m_PVRDenoiserTypeAO: 1
    m_PVRFilterTypeDirect: 0
    m_PVRFilterTypeIndirect: 0
    m_PVRFilterTypeAO: 0
    m_PVREnvironmentMIS: 1
    m_PVRCulling: 1
    m_PVRFilteringGaussRadiusDirect: 1
    m_PVRFilteringGaussRadiusIndirect: 5
    m_PVRFilteringGaussRadiusAO: 2
    m_PVRFilteringAtrousPositionSigmaDirect: 0.5
    m_PVRFilteringAtrousPositionSigmaIndirect: 2
    m_PVRFilteringAtrousPositionSigmaAO: 1
    m_ExportTrainingData: 0
    m_TrainingDataDestination: TrainingData
    m_LightProbeSampleCountMultiplier: 4
  m_LightingDataAsset: {fileID: 0}
  m_LightingSettings: {fileID: 0}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 3
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    buildHeightMesh: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {fileID: 0}
"""


def xf(tid, gid, pos, scale, parent, children, rot=(0, 0, 0, 1)):
    ch = "\n".join(f"  - {{fileID: {c}}}" for c in children) if children else ""
    qx, qy, qz, qw = rot
    return f"""--- !u!4 &{tid}
Transform:
  m_GameObject: {{fileID: {gid}}}
  serializedVersion: 2
  m_LocalRotation: {{x: {qx}, y: {qy}, z: {qz}, w: {qw}}}
  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}
  m_LocalScale: {{x: {scale[0]}, y: {scale[1]}, z: {scale[2]}}}
  m_ConstrainProportionsScale: 0
  m_Children:
{ch if ch else "  []"}
  m_Father: {{fileID: {parent}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
"""


def emit_obj(o):
    lines = [f"  - component: {{fileID: {o['tid']}}}"]
    if o["kind"] in ("cube", "model"):
        lines += [f"  - component: {{fileID: {o['mf']}}}", f"  - component: {{fileID: {o['mr']}}}"]
        if o.get("collider") and o.get("bc"):
            lines.append(f"  - component: {{fileID: {o['bc']}}}")
    out = [f"""--- !u!1 &{o['gid']}
GameObject:
  serializedVersion: 6
  m_Component:
{chr(10).join(lines)}
  m_Name: {o['name']}
  m_TagString: {o['tag']}
  m_IsActive: 1"""]
    out.append(xf(o["tid"], o["gid"], o["pos"], o["scale"], o["parent"],
                    child_map.get(o["tid"], []), o.get("rot", (0, 0, 0, 1))))
    if o["kind"] == "cube":
        mesh = CUBE
        out.append(f"--- !u!33 &{o['mf']}\nMeshFilter:\n  m_GameObject: {{fileID: {o['gid']}}}\n  m_Mesh: {mesh}")
        out.append(f"--- !u!23 &{o['mr']}\nMeshRenderer:\n  m_GameObject: {{fileID: {o['gid']}}}\n  m_Enabled: 1\n  m_Materials:\n  - {mat_ref(o['mat'])}")
        if o.get("collider"):
            t = 1 if o["trigger"] else 0
            out.append(f"--- !u!65 &{o['bc']}\nBoxCollider:\n  m_GameObject: {{fileID: {o['gid']}}}\n  m_IsTrigger: {t}\n  m_Enabled: 1\n  serializedVersion: 3\n  m_Size: {{x: 1, y: 1, z: 1}}\n  m_Center: {{x: 0, y: 0, z: 0}}")
    elif o["kind"] == "model":
        mesh = mesh_ref(o["model_key"])
        out.append(f"--- !u!33 &{o['mf']}\nMeshFilter:\n  m_GameObject: {{fileID: {o['gid']}}}\n  m_Mesh: {mesh}")
        out.append(f"--- !u!23 &{o['mr']}\nMeshRenderer:\n  m_GameObject: {{fileID: {o['gid']}}}\n  m_Enabled: 1\n  m_Materials:\n  - {mat_ref(o['mat'])}")
        if o.get("collider") and o.get("bc"):
            out.append(f"--- !u!65 &{o['bc']}\nBoxCollider:\n  m_GameObject: {{fileID: {o['gid']}}}\n  m_IsTrigger: 0\n  m_Enabled: 1\n  serializedVersion: 3\n  m_Size: {{x: 1, y: 1, z: 1}}\n  m_Center: {{x: 0, y: 0.5, z: 0}}")
    return "\n".join(out)


def emit_light(gid, comp, tid, name, pos, ltype, color, intensity, range_val=0, shadows=0):
    sh = f"  m_Shadows:\n    m_Type: {shadows}" if shadows else "  m_Shadows:\n    m_Type: 0"
    rng = f"  m_Range: {range_val}" if range_val else ""
    sp = pos if REF_RX == RX and REF_RZ == RZ else scaled_pos(pos)
    return f"""--- !u!1 &{gid}
GameObject:
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {tid}}}
  - component: {{fileID: {comp}}}
  m_Name: {name}
  m_IsActive: 1
--- !u!108 &{comp}
Light:
  m_GameObject: {{fileID: {gid}}}
  m_Type: {ltype}
  m_Color: {{r: {color[0]}, g: {color[1]}, b: {color[2]}, a: 1}}
  m_Intensity: {intensity}
{rng}
{sh}
{xf(tid, gid, sp, (1, 1, 1), 0, [])}"""


def emit_camera():
    cp = (-RX * 0.85, WALL_H * 1.35, -RZ * 0.85)
    qx, qy, qz, qw = normalize_quat(0.38268343, 0.35355338, -0.1464466, 0.8535534)
    return f"""--- !u!1 &{cam_gid}
GameObject:
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {cam_tid}}}
  - component: {{fileID: {cam_comp}}}
  - component: {{fileID: {cam_audio}}}
  m_Name: Main Camera
  m_TagString: MainCamera
  m_IsActive: 1
--- !u!81 &{cam_audio}
AudioListener:
  m_GameObject: {{fileID: {cam_gid}}}
  m_Enabled: 1
--- !u!20 &{cam_comp}
Camera:
  m_GameObject: {{fileID: {cam_gid}}}
  m_Enabled: 1
  serializedVersion: 2
  m_ClearFlags: 1
  m_BackGroundColor: {{r: 0.1, g: 0.08, b: 0.07, a: 0}}
  m_FieldOfView: 60
  near clip plane: 0.3
  far clip plane: 200
  m_Depth: -1
--- !u!4 &{cam_tid}
Transform:
  m_GameObject: {{fileID: {cam_gid}}}
  serializedVersion: 2
  m_LocalRotation: {{x: {qx}, y: {qy}, z: {qz}, w: {qw}}}
  m_LocalPosition: {{x: {cp[0]}, y: {cp[1]}, z: {cp[2]}}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_Children: []
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 48, y: 35, z: 0}}"""


def write_mat_meta(path, guid):
    if os.path.exists(path + ".meta"):
        return
    with open(path + ".meta", "w", encoding="utf-8") as f:
        f.write(f"fileFormatVersion: 2\nguid: {guid}\nNativeFormatImporter:\n  mainObjectFileID: 2100000\n")


def verify_scene_guids(scene_text):
    refs = set(re.findall(r"guid: ([0-9a-f]{32})", scene_text))
    missing = []
    for g in refs:
        if g in ("0000000000000000e000000000000000", "0000000000000000f000000000000000"):
            continue
        found = False
        for folder in (MATERIALS, TEXTURES, ROOT, os.path.join(ROOT, "..", "..", "Models")):
            if not os.path.isdir(folder):
                continue
            for dirpath, _, filenames in os.walk(folder):
                for fn in filenames:
                    if fn.endswith(".meta") and read_guid(os.path.join(dirpath, fn)) == g:
                        found = True
                        break
                if found:
                    break
            if found:
                break
        if not found:
            missing.append(g)
    return missing


def main():
    os.makedirs(SCENES, exist_ok=True)
    os.makedirs(MATERIALS, exist_ok=True)
    write_texture_meta()
    for name, (tex, tile, color, gloss) in MAT_DEFS.items():
        p = os.path.join(MATERIALS, name)
        with open(p, "w", encoding="utf-8") as f:
            f.write(material_yaml(name, tex, tile, color, gloss))
        write_mat_meta(p, MAT_GUIDS[name])

    parts = [SCENE_HEADER]
    for o in objects:
        parts.append(emit_obj(o))
    parts += [
        emit_light(light_gid, light_comp, light_tid, "Directional Light", (0, 8, 0), 1, (1, 0.94, 0.82), 0.55, shadows=2),
        emit_light(room1_gid, room1_comp, room1_tid, "RoomLight_Center", (0, WALL_H * 0.72, 0), 2, (1, 0.86, 0.62), 1.8, int(WALL_H * 3.5)),
        emit_light(room2_gid, room2_comp, room2_tid, "RoomLight_Fireplace", (0, WALL_H * 0.55, RZ * 0.65), 2, (1, 0.55, 0.3), 1.0, int(WALL_H * 1.6)),
        emit_light(fire_gid, fire_comp, fire_tid, "FireGlow", (0, 1.0, RZ * 0.7), 2, (1, 0.4, 0.15), 0.6, int(WALL_H * 0.9)),
        emit_camera(),
        "--- !u!1660057539 &9223372036854775807\nSceneRoots:\n  m_ObjectHideFlags: 0\n  m_Roots:",
    ]
    for tid in roots:
        parts.append(f"  - {{fileID: {tid}}}")

    scene_text = "\n".join(parts) + "\n"
    scene_path = os.path.join(SCENES, "MainRoom.unity")
    with open(scene_path, "w", encoding="utf-8") as f:
        f.write(scene_text)

    missing = verify_scene_guids(scene_text)
    model_count = sum(1 for o in objects if o["kind"] == "model")
    print(f"Done: {RX*2:.0f}x{RZ*2:.0f}m floor, {WALL_H:.1f}m height ({RX*2*RZ*2:.0f}sqm), {len(objects)} objects, {model_count} 3D models")
    print("Material GUIDs synced from .meta files")
    if missing:
        print("WARNING missing GUID refs:", ", ".join(missing))
    else:
        print("All scene GUID references verified OK")


if __name__ == "__main__":
    main()
