# Camera 与渲染系统详解

## 思维导图总览

```
Camera & Rendering
├── Camera 摄像机
│   ├── 透视 / 正交投影
│   ├── fieldOfView / orthographicSize
│   ├── near/far clip plane
│   ├── cullingMask 渲染层
│   └── ScreenToWorldPoint / WorldToScreenPoint
│
├── Material 材质
│   ├── shader 着色器
│   ├── mainTexture / color
│   └── SetFloat / SetColor / SetTexture
│
├── Renderer 渲染器
│   ├── MeshRenderer（3D）
│   └── SpriteRenderer（2D）
│
└── Light 光照
    ├── Directional / Point / Spot
    ├── color / intensity
    └── shadows 阴影类型
```

---

## 一、Camera 摄像机

| 属性 | 说明 |
|------|------|
| `fieldOfView` | 透视视野角度（度） |
| `orthographic` | 是否正交投影 |
| `orthographicSize` | 正交半高（世界单位） |
| `nearClipPlane` / `farClipPlane` | 近远裁剪面 |
| `cullingMask` | 渲染哪些 Layer |
| `depth` | 多相机时渲染顺序 |
| `backgroundColor` | 清除背景色 |
| `targetTexture` | 渲染到 RenderTexture |

### 坐标转换

```csharp
// 屏幕坐标 → 世界坐标（常用于鼠标点击地面）
Vector3 screenPos = Input.mousePosition;
screenPos.z = 10f; // 到相机的距离
Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

// 世界 → 屏幕（UI 血条跟随）
Vector3 screenPoint = cam.WorldToScreenPoint(target.position);
```

### 主相机

```csharp
Camera cam = Camera.main; // 需 Tag 为 MainCamera
```

---

## 二、Material 材质

| 成员 | 说明 |
|------|------|
| `shader` | 使用的着色器 |
| `color` | 主颜色（\_Color） |
| `mainTexture` | 主贴图 |

```csharp
Renderer rend = GetComponent<Renderer>();
rend.material.color = Color.red;

// 避免内存泄漏：运行时频繁改色用 material（实例副本）
// 批量相同外观用 sharedMaterial
```

> **易错点**：访问 `.material` 会实例化一份新材质；大量对象用 `sharedMaterial` 或 MaterialPropertyBlock。

---

## 三、Renderer 渲染器

| 组件 | 用途 |
|------|------|
| MeshRenderer | 显示 3D 网格，需 MeshFilter |
| SkinnedMeshRenderer | 带骨骼动画的角色 |
| SpriteRenderer | 2D 精灵 |

| 属性 | 说明 |
|------|------|
| `material` / `materials` | 材质（可多个子材质） |
| `enabled` | 是否渲染 |

---

## 四、Light 光照

| 类型 | 特点 |
|------|------|
| Directional | 平行光，模拟太阳 |
| Point | 点光源，向四周 |
| Spot | 锥形聚光灯 |

| 属性 | 说明 |
|------|------|
| `color` | 光色 |
| `intensity` | 强度 |
| `shadows` | None / Hard / Soft |
| `range` | 点光/聚光影响距离 |

---

## 五、渲染管线概念（入门）

```
场景物体 → MeshFilter + MeshRenderer
         → 材质(Material) + 着色器(Shader)
         → Camera 按 cullingMask 拍摄
         → 输出到屏幕 / RenderTexture
```

---

## 六、典型应用

| 场景 | 用法 |
|------|------|
| 第三人称跟随 | 子相机或 LateUpdate 跟随 |
| 小地图相机 | 正交 + RenderTexture |
| 受伤闪红 | material.color 或 Shader 参数 |
| 夜视效果 | 后处理或替换 Shader |
| 点击地面移动 | ScreenToWorldPoint + Raycast |
