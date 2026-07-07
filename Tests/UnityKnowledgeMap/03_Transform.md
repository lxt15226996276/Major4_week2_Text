# Transform 类详解

## 思维导图总览

```
Transform
├── 概念定义
│   ├── 每个 GameObject 必有且唯一的组件
│   ├── 继承：Component → Transform
│   └── 管理位置、旋转、缩放与父子层级
│
├── 位置 Position
│   ├── position（世界坐标）
│   ├── localPosition（本地坐标）
│   └── Translate() 平移
│
├── 旋转 Rotation
│   ├── rotation（世界，Quaternion）
│   ├── localRotation（本地）
│   ├── eulerAngles / localEulerAngles（欧拉角，度）
│   └── Rotate() / LookAt()
│
├── 缩放 Scale
│   ├── localScale（仅本地，无 worldScale 属性）
│   └── 负缩放可翻转模型
│
├── 父子层级
│   ├── parent / SetParent()
│   ├── childCount / GetChild()
│   ├── root（根 Transform）
│   └── 子物体继承父级变换
│
├── 方向向量
│   ├── forward / right / up（世界）
│   └── 与 local 对应属性
│
└── 坐标空间转换
    ├── TransformPoint / InverseTransformPoint
    ├── TransformDirection / InverseTransformDirection
    └── TransformVector / InverseTransformVector
```

---

## 一、概念定义

| 项目 | 说明 |
|------|------|
| **是什么** | 描述 GameObject 在 3D 空间中的位置、朝向、大小及层级关系 |
| **继承链** | `Component` → `Transform` |
| **特殊性** | 每个 GameObject 有且仅有一个 Transform，不可移除 |
| **访问方式** | `gameObject.transform` 或简写 `transform` |

---

## 二、位置（Position）

| 属性/方法 | 类型 | 说明 |
|-----------|------|------|
| `position` | Vector3 | 世界空间位置 |
| `localPosition` | Vector3 | 相对父对象的位置 |
| `Translate(Vector3)` | void | 沿各轴平移（默认 Space.Self） |
| `Translate(Vector3, Space)` | void | Self=本地轴，World=世界轴 |

```csharp
// 帧率无关移动（在 Update 中）
transform.Translate(Vector3.forward * speed * Time.deltaTime);

// 直接设置世界坐标
transform.position = new Vector3(0, 1, 0);
```

---

## 三、旋转（Rotation）

| 属性/方法 | 说明 |
|-----------|------|
| `rotation` | 世界旋转（Quaternion，推荐） |
| `localRotation` | 本地旋转 |
| `eulerAngles` | 世界欧拉角（度），可能万向节锁 |
| `Rotate(Vector3)` | 绕轴旋转 |
| `LookAt(Transform)` | 朝向目标 |

```csharp
// 平滑转向目标
Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

// 2D 常用：只绕 Y 轴
Vector3 dir = target.position - transform.position;
dir.y = 0;
transform.rotation = Quaternion.LookRotation(dir);
```

> **易错点**：修改 `eulerAngles` 可能产生万向节锁；旋转运算优先用 `Quaternion`。

---

## 四、缩放（Scale）

| 属性 | 说明 |
|------|------|
| `localScale` | 相对父对象的缩放，默认 (1,1,1) |
| `lossyScale` | 世界缩放（只读，含父级影响） |

> **注意**：非均匀缩放父对象会影响子物体碰撞体形状；UI 使用 `RectTransform` 而非 Transform。

---

## 五、父子层级

```csharp
// 设置父对象（worldPositionStays=true 保持世界坐标）
child.SetParent(parent, true);

// 脱离父级但保持世界位置
child.SetParent(null, true);

// 遍历子物体
for (int i = 0; i < transform.childCount; i++)
{
  Transform child = transform.GetChild(i);
}
```

| 概念 | 说明 |
|------|------|
| 本地坐标 | 相对父 Transform 的 position/rotation/scale |
| 世界坐标 | 场景全局空间中的实际值 |
| 继承规则 | 父移动/旋转/缩放会带动所有子物体 |

---

## 六、坐标空间转换

| 方法 | 作用 |
|------|------|
| `TransformPoint(localPoint)` | 本地点 → 世界点 |
| `InverseTransformPoint(worldPoint)` | 世界点 → 本地点 |
| `TransformDirection(dir)` | 本地方向 → 世界方向（不含位移） |
| `InverseTransformDirection(dir)` | 世界方向 → 本地方向 |

```csharp
// 在坦克前方 2 米处生成子弹（世界坐标）
Vector3 spawnPos = transform.TransformPoint(new Vector3(0, 0, 2));
Instantiate(bulletPrefab, spawnPos, transform.rotation);
```

---

## 七、方向向量

| 属性 | 含义（世界空间） |
|------|------------------|
| `forward` | 蓝色 Z 轴正方向 |
| `right` | 红色 X 轴正方向 |
| `up` | 绿色 Y 轴正方向 |

```csharp
transform.position += transform.forward * speed * Time.deltaTime;
```

---

## 八、注意要点与最佳实践

| 易错点 | 正确做法 |
|--------|----------|
| Update 中移动未乘 deltaTime | 乘以 `Time.deltaTime` |
| 直接改 eulerAngles 导致抖动 | 用 Quaternion.Slerp |
| 在子物体上改 position 以为是本地 | 明确用 localPosition 或 Space.Self |
| 相机跟随写在 Update | 写在 LateUpdate，避免抖动 |

---

## 九、典型应用场景

| 场景 | 用法 |
|------|------|
| 角色 WASD 移动 | Translate + forward/right |
| 炮台瞄准 | LookAt / LookRotation |
| 相机跟随 | LateUpdate 修改 position |
| 挂点生成特效 | TransformPoint |
| 武器挂到手上 | SetParent |
