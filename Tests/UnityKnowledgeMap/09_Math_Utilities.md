# Math 与 Utilities 工具类详解

## 思维导图总览

```
Math & Utilities
├── Vector2 / Vector3 / Vector4
│   ├── x,y,z 分量与构造
│   ├── 运算符 + - * /
│   ├── magnitude / normalized / sqrMagnitude
│   ├── Dot / Cross / Distance
│   └── Lerp / MoveTowards
│
├── Quaternion 四元数
│   ├── identity / Euler / AngleAxis
│   ├── LookRotation / Slerp / Lerp
│   └── eulerAngles（慎用）
│
├── Mathf 数学函数
│   ├── Clamp / Lerp / InverseLerp
│   ├── Sin / Cos / Tan / Atan2
│   ├── Abs / Min / Max / Pow / Sqrt
│   └── PerlinNoise（噪声，非 Random）
│
├── Random 随机数（静态类）
│   ├── Range(int/float) 区间随机
│   ├── value（0~1）
│   ├── insideUnitSphere / insideUnitCircle
│   ├── onUnitSphere / rotation
│   └── InitState 固定种子
│
└── Debug 调试
    ├── Log / LogWarning / LogError
    ├── DrawLine / DrawRay
    └── Assert
```

---

## 一、Vector3（最常用）

| 成员 | 说明 |
|------|------|
| `Vector3.zero` | (0,0,0) |
| `Vector3.one` | (1,1,1) |
| `Vector3.up` / `forward` / `right` | 常用方向 |
| `magnitude` | 长度 |
| `normalized` | 单位向量 |
| `sqrMagnitude` | 长度平方（比较距离时更快） |

```csharp
Vector3 dir = (target.position - transform.position).normalized;
float dist = Vector3.Distance(a, b);

// 插值
Vector3 pos = Vector3.Lerp(start, end, t);
```

| 静态方法 | 用途 |
|----------|------|
| `Dot(a,b)` | 夹角、前后判断 |
| `Cross(a,b)` | 法线、垂直向量 |
| `Angle(from,to)` | 两向量夹角（度） |
| `Project(a, onNormal)` | 向量投影 |

---

## 二、Quaternion

```csharp
// 朝向目标
Quaternion rot = Quaternion.LookRotation(direction);

// 平滑旋转
transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);

// 绕轴旋转
transform.rotation = Quaternion.AngleAxis(90f, Vector3.up);
```

| 对比 | Quaternion | Euler 角 |
|------|------------|----------|
| 优点 | 无万向节锁、插值平滑 | 直观易读 |
| 缺点 | 不直观 | 可能锁死、插值异常 |

---

## 三、Mathf

```csharp
float hp = Mathf.Clamp(currentHp, 0, maxHp);
float t = Mathf.InverseLerp(0, maxHp, currentHp); // 映射到 0~1
float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

int dice = Mathf.RoundToInt(Random.Range(1f, 7f)); // 1~6
```

| 常用 | 说明 |
|------|------|
| `Clamp(value, min, max)` | 限制范围 |
| `Lerp(a, b, t)` | 线性插值 |
| `PingPong(t, length)` | 来回摆动 |
| `Approximately(a, b)` | 浮点近似相等 |
| `PerlinNoise(x, y)` | 连续噪声（地形/自然抖动，不是 Random） |

> **注意**：`Random.Range` 属于 **`UnityEngine.Random`**，不是 Mathf 的成员。

---

## 四、Random（UnityEngine.Random）

| 项目 | 说明 |
|------|------|
| **类型** | 静态类，不能实例化 |
| **官方** | [ScriptReference: Random](https://docs.unity3d.com/ScriptReference/Random.html) |
| **用途** | 伤害浮动、刷怪位置、掉落概率、子弹散布、随机音效 pitch |

### 4.1 核心 API

| API | 说明 |
|-----|------|
| `Random.Range(float min, float max)` | [min, max] **双端包含** |
| `Random.Range(int min, int max)` | [min, max) **上限不含** |
| `Random.value` | 0.0～1.0，常做概率 `if (Random.value < 0.3f)` |
| `Random.insideUnitSphere` | 球体内随机点 × 半径 = 刷怪范围 |
| `Random.insideUnitCircle` | 圆内随机 Vector2 |
| `Random.onUnitSphere` | 球面随机方向 |
| `Random.rotation` | 随机四元数旋转 |
| `Random.InitState(int seed)` | 固定种子，测试可复现 |

```csharp
// 整数骰子 1~6
int dice = Random.Range(1, 7);

// 数组随机索引
int idx = Random.Range(0, items.Length);

// 浮动伤害 10~20（含 20）
float dmg = Random.Range(10f, 20f);

// 30% 概率暴击
if (Random.value < 0.3f) Crit();

// 圆形范围刷怪
Vector3 pos = spawnCenter + Random.insideUnitSphere * radius;
```

### 4.2 Random vs System.Random（易错点）

| | UnityEngine.Random | System.Random |
|--|-------------------|---------------|
| 写法 | `Random.Range(0, 10)` | `new Random().Next(0, 10)` |
| 游戏脚本 | ✅ 推荐 | 少用 |
| 命名冲突 | 与 `using System;` 同时存在时写 `UnityEngine.Random` | — |

---

## 五、Debug

```csharp
Debug.Log("玩家血量：" + hp);
Debug.LogWarning("弹药不足");
Debug.LogError("配置缺失");

// 场景视图可见（仅 Play 模式，短时间）
Debug.DrawRay(transform.position, transform.forward * 5f, Color.green, 2f);
Debug.DrawLine(pointA, pointB, Color.red);

Debug.Assert(coin != null, "coin 未赋值");
```

---

## 六、Color 与 Ray

```csharp
Color c = new Color(1f, 0.5f, 0f, 1f); // RGBA 0~1
Color red = Color.red;

Ray ray = new Ray(origin, direction);
if (Physics.Raycast(ray, out RaycastHit hit, 100f)) { }
```

---

## 七、典型应用

| 场景 | 用法 |
|------|------|
| 判断敌人在前方 | Vector3.Dot(transform.forward, dir) > 0 |
| 环绕运动 | Sin/Cos + Time.time |
| 血条百分比 | InverseLerp |
| 调试射线 | Debug.DrawRay |
| 随机刷怪点 | Random.insideUnitSphere * radius |
| 随机伤害/暴击 | Random.Range / Random.value |
| 从数组随机一项 | prefabs[Random.Range(0, prefabs.Length)] |
