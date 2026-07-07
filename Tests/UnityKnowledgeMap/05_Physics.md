# Physics 物理系统详解

## 思维导图总览

```
Physics 物理系统
├── Rigidbody 刚体
│   ├── mass / drag / angularDrag
│   ├── useGravity / isKinematic
│   ├── velocity / angularVelocity
│   ├── AddForce / AddTorque
│   └── constraints 冻结轴
│
├── Collider 碰撞器
│   ├── Box / Sphere / Capsule / Mesh
│   ├── isTrigger 触发器
│   └── OnCollision / OnTrigger 回调
│
├── Physics 静态类
│   ├── Raycast 射线检测
│   ├── OverlapSphere 范围检测
│   └── gravity 全局重力
│
└── PhysicMaterial 物理材质
    ├── dynamicFriction / staticFriction
    └── bounciness 弹性
```

---

## 一、Rigidbody 刚体

| 属性 | 说明 |
|------|------|
| `mass` | 质量（kg），影响受力响应 |
| `drag` | 线性阻力 |
| `angularDrag` | 角阻力 |
| `useGravity` | 是否受重力 |
| `isKinematic` | true=不受物理力，可用代码驱动 |
| `velocity` | 线速度（直接改可瞬移，慎用） |
| `angularVelocity` | 角速度 |

### 常用方法

```csharp
// 持续力（适合火箭、加速）— 在 FixedUpdate 中调用
rb.AddForce(Vector3.forward * thrust, ForceMode.Force);

// 瞬时冲量（适合跳跃、爆炸）
rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

// 移动到目标（Kinematic 或配合 MovePosition）
rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
```

| ForceMode | 含义 |
|-----------|------|
| Force | 持续力，受质量影响 |
| Impulse | 瞬时冲量 |
| Acceleration | 忽略质量的加速度 |
| VelocityChange | 直接改变速度 |

---

## 二、Collider 碰撞器

| 类型 | 特点 |
|------|------|
| BoxCollider | 盒形，性能好 |
| SphereCollider | 球形，性能好 |
| CapsuleCollider | 胶囊，适合角色 |
| MeshCollider | 贴合网格，非凸或复杂网格开销大 |

| 属性 | 说明 |
|------|------|
| `isTrigger` | true=不产生物理碰撞，只触发 OnTrigger |
| `material` | PhysicMaterial 引用 |

### 碰撞回调（脚本挂在有 Collider 的对象上）

| 回调 | 条件 |
|------|------|
| `OnCollisionEnter/Stay/Exit` | 非 Trigger，双方有 Collider，至少一方有 Rigidbody |
| `OnTriggerEnter/Stay/Exit` | 至少一方 isTrigger=true |

```csharp
void OnTriggerEnter(Collider other)
{
  if (other.CompareTag("Enemy"))
    TakeDamage(10);
}
```

---

## 三、Physics 射线与检测

```csharp
// 从相机发射射线（射击命中检测）
Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
if (Physics.Raycast(ray, out RaycastHit hit, 100f))
{
  Debug.Log("击中：" + hit.collider.name);
  Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
}

// 球形范围检测
Collider[] hits = Physics.OverlapSphere(transform.position, 5f);
```

| 方法 | 用途 |
|------|------|
| `Raycast` | 子弹、鼠标拾取、视线检测 |
| `SphereCast` | 厚射线（胶囊体扫掠） |
| `OverlapSphere` | 爆炸范围、AOE |
| `Linecast` | 两点间是否有障碍 |

> 重载可传入 `layerMask` 过滤层级；`QueryTriggerInteraction` 控制是否检测 Trigger。

---

## 四、Layer 与碰撞矩阵

- **Edit → Project Settings → Physics → Layer Collision Matrix** 配置哪些层相互碰撞
- 射线、Overlap 用 `LayerMask` 过滤目标

```csharp
int enemyMask = LayerMask.GetMask("Enemy");
Physics.Raycast(ray, out hit, 100f, enemyMask);
```

---

## 五、注意要点

| 易错点 | 正确做法 |
|--------|----------|
| 在 Update 里 AddForce | 放 FixedUpdate |
| 快速移动物体穿模 | 碰撞检测模式 Continuous |
| Trigger 不触发 | 检查 isTrigger、Rigidbody、Layer 矩阵 |
| MeshCollider 做动态刚体 | 动态物体用 Primitive 或凸 Mesh |
| 改 transform.position 驱动 Dynamic RB | 用 AddForce 或 velocity |

---

## 六、典型应用

| 场景 | 方案 |
|------|------|
| 坦克移动 | Rigidbody + AddForce / velocity |
| 子弹命中 | Raycast 或 OnCollision |
| 拾取道具 | Trigger + CompareTag |
| 地面检测 | Raycast 向下短距离 |
| 手雷爆炸 | OverlapSphere + 力 |
