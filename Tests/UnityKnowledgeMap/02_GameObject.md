# GameObject 类详解

## 思维导图总览

```
GameObject
├── 概念定义
│   ├── Unity场景中所有实体的基类
│   ├── 继承关系：Object → GameObject
│   ├── 命名空间：UnityEngine
│   └── 本质：组件的容器
│
├── 创建与销毁
│   ├── 构造函数 new GameObject()
│   ├── Instantiate() 实例化预制体
│   ├── Destroy() 销毁对象
│   └── DestroyImmediate() 立即销毁
│
├── 组件管理
│   ├── AddComponent<T>()
│   ├── GetComponent<T>()
│   ├── GetComponentInChildren<T>()
│   ├── GetComponentInParent<T>()
│   ├── GetComponents<T>()
│   └── TryGetComponent<T>()
│
├── 激活状态
│   ├── activeSelf 自身激活状态
│   ├── activeInHierarchy 层级中实际激活状态
│   └── SetActive() 设置激活
│
├── 标签与层级
│   ├── tag 标签
│   ├── layer 层级
│   ├── CompareTag() 比较标签
│   └── LayerMask 层级遮罩
│
├── 查找对象
│   ├── Find() 按名称查找
│   ├── FindWithTag() 按标签查找
│   ├── FindGameObjectsWithTag() 查找所有同标签
│   └── FindObjectOfType<T>() 按类型查找
│
├── 重要属性
│   ├── transform
│   ├── name
│   ├── tag
│   ├── layer
│   ├── scene
│   └── isStatic
│
└── 注意要点
    ├── Find系列方法性能开销大
    ├── 销毁是延迟到帧末执行
    ├── 父对象禁用会影响子对象
    └── 预制体实例化的最佳实践
```

---

## 一、概念定义

| 项目 | 说明 |
|------|------|
| **是什么** | Unity 场景中所有实体的基础类，是组件（Component）的容器 |
| **继承链** | `System.Object` → `UnityEngine.Object` → `GameObject` |
| **命名空间** | `UnityEngine` |
| **核心作用** | 作为场景中可见/不可见对象的载体，通过挂载不同组件实现各种功能 |
| **本质理解** | GameObject 本身只是一个"空壳"，所有功能都由其上的 Component 提供 |

### 与 Component 的关系

```
GameObject（容器）
├── Transform（必须有，不可移除）
├── MeshRenderer（可选）
├── Collider（可选）
├── Rigidbody（可选）
├── 自定义脚本（可选）
└── ...其他组件
```

---

## 二、创建 GameObject

### 2.1 构造函数创建

```csharp
// 创建空 GameObject
GameObject obj = new GameObject();

// 创建并命名
GameObject obj = new GameObject("MyObject");

// 创建并添加组件
GameObject obj = new GameObject("Player", typeof(Rigidbody), typeof(BoxCollider));
```

| 构造函数 | 说明 |
|----------|------|
| `GameObject()` | 创建名为 "New Game Object" 的空对象 |
| `GameObject(string name)` | 创建指定名称的空对象 |
| `GameObject(string name, params Type[] components)` | 创建并添加指定组件 |

### 2.2 Instantiate 实例化

```csharp
// 基础实例化
GameObject clone = Instantiate(prefab);

// 指定位置和旋转
GameObject clone = Instantiate(prefab, position, rotation);

// 指定父对象
GameObject clone = Instantiate(prefab, parentTransform);

// 完整参数
GameObject clone = Instantiate(prefab, position, rotation, parentTransform);
```

| 重载方法 | 参数说明 |
|----------|----------|
| `Instantiate(Object original)` | 克隆原始对象 |
| `Instantiate(Object original, Transform parent)` | 克隆并设置父对象 |
| `Instantiate(Object original, Transform parent, bool worldPositionStays)` | worldPositionStays: true保持世界坐标，false使用本地坐标 |
| `Instantiate(Object original, Vector3 position, Quaternion rotation)` | 克隆并设置位置旋转 |
| `Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent)` | 完整参数版本 |

### 2.3 CreatePrimitive 创建基本体

```csharp
// 创建基本几何体
GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
```

---

## 三、销毁 GameObject

### 3.1 Destroy

```csharp
// 立即标记销毁（实际在帧末执行）
Destroy(gameObject);

// 延迟销毁
Destroy(gameObject, 5f); // 5秒后销毁

// 销毁组件（不销毁对象）
Destroy(GetComponent<Rigidbody>());
```

| 方法 | 说明 |
|------|------|
| `Destroy(Object obj)` | 标记对象在当前帧末销毁 |
| `Destroy(Object obj, float t)` | 延迟 t 秒后销毁 |

### 3.2 DestroyImmediate

```csharp
// 立即销毁（仅用于编辑器脚本）
DestroyImmediate(gameObject);
```

> **警告**：`DestroyImmediate` 仅应在编辑器脚本中使用，运行时使用可能导致问题。

### 3.3 DontDestroyOnLoad

```csharp
void Awake()
{
    // 切换场景时不销毁此对象
    DontDestroyOnLoad(gameObject);
}
```

---

## 四、组件管理

### 4.1 添加组件

```csharp
// 泛型方式（推荐）
Rigidbody rb = gameObject.AddComponent<Rigidbody>();

// Type 方式
Component comp = gameObject.AddComponent(typeof(Rigidbody));
```

### 4.2 获取组件

| 方法 | 搜索范围 | 返回值 |
|------|----------|--------|
| `GetComponent<T>()` | 当前对象 | 单个组件或 null |
| `GetComponents<T>()` | 当前对象 | 组件数组 |
| `GetComponentInChildren<T>()` | 当前对象 + 所有子对象 | 单个组件或 null |
| `GetComponentsInChildren<T>()` | 当前对象 + 所有子对象 | 组件数组 |
| `GetComponentInParent<T>()` | 当前对象 + 所有父对象 | 单个组件或 null |
| `GetComponentsInParent<T>()` | 当前对象 + 所有父对象 | 组件数组 |
| `TryGetComponent<T>(out T)` | 当前对象 | bool + out 参数 |

```csharp
// 基础获取
Rigidbody rb = GetComponent<Rigidbody>();

// 安全获取（推荐，避免 null 检查开销）
if (TryGetComponent<Rigidbody>(out var rb))
{
    rb.AddForce(Vector3.up);
}

// 获取子对象中的组件（包含自身）
Renderer[] renderers = GetComponentsInChildren<Renderer>();

// 获取子对象中的组件（不包含禁用的对象）
Renderer[] activeRenderers = GetComponentsInChildren<Renderer>(false);

// 获取子对象中的组件（包含禁用的对象）
Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);
```

### 4.3 组件存在性检查

```csharp
// 方式1：直接判空
if (GetComponent<Rigidbody>() != null)
{
    // 有 Rigidbody
}

// 方式2：TryGetComponent（性能更好，Unity 2019.2+）
if (TryGetComponent<Rigidbody>(out var rb))
{
    rb.AddForce(Vector3.up);
}
```

---

## 五、激活状态

### 5.1 属性说明

| 属性 | 类型 | 说明 |
|------|------|------|
| `activeSelf` | bool (只读) | 对象自身的激活状态 |
| `activeInHierarchy` | bool (只读) | 对象在层级中的实际激活状态 |

### 5.2 activeSelf vs activeInHierarchy

```
Parent (activeSelf=true, activeInHierarchy=true)
└── Child (activeSelf=true, activeInHierarchy=true)
    └── GrandChild (activeSelf=true, activeInHierarchy=true)

Parent (activeSelf=false, activeInHierarchy=false)  ← 父对象禁用
└── Child (activeSelf=true, activeInHierarchy=false)  ← 自身启用但实际禁用
    └── GrandChild (activeSelf=true, activeInHierarchy=false)
```

### 5.3 SetActive

```csharp
// 激活对象
gameObject.SetActive(true);

// 禁用对象
gameObject.SetActive(false);
```

| 影响 | 说明 |
|------|------|
| 渲染 | 禁用后不渲染 |
| Update | 禁用后不调用 |
| 协程 | 禁用后停止（不会自动恢复） |
| 碰撞 | 禁用后不参与碰撞检测 |
| OnEnable/OnDisable | 状态改变时调用 |

---

## 六、标签与层级

### 6.1 Tag 标签

```csharp
// 设置标签
gameObject.tag = "Player";

// 获取标签
string tag = gameObject.tag;

// 比较标签（推荐，不产生 GC）
if (gameObject.CompareTag("Enemy"))
{
    // 是敌人
}

// 不推荐的比较方式（产生 GC）
if (gameObject.tag == "Enemy") { }
```

### 6.2 Layer 层级

```csharp
// 设置层级（使用层级索引）
gameObject.layer = 8;

// 使用层级名称设置
gameObject.layer = LayerMask.NameToLayer("Enemy");

// 获取层级名称
string layerName = LayerMask.LayerToName(gameObject.layer);
```

### 6.3 LayerMask 层级遮罩

```csharp
// 创建包含指定层的遮罩
LayerMask mask = LayerMask.GetMask("Player", "Enemy");

// 在射线检测中使用
if (Physics.Raycast(ray, out hit, 100f, mask))
{
    // 只检测 Player 和 Enemy 层
}

// 位运算创建遮罩
int playerLayer = LayerMask.NameToLayer("Player");
int mask = 1 << playerLayer; // 只包含 Player 层
int inverseMask = ~mask;     // 排除 Player 层
```

---

## 七、查找 GameObject

### 7.1 按名称查找

```csharp
// 查找场景中指定名称的对象（性能差，避免频繁调用）
GameObject player = GameObject.Find("Player");

// 支持路径查找
GameObject weapon = GameObject.Find("Player/Hand/Weapon");
```

### 7.2 按标签查找

```csharp
// 查找单个对象
GameObject player = GameObject.FindWithTag("Player");
// 或
GameObject player = GameObject.FindGameObjectWithTag("Player");

// 查找所有同标签对象
GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
```

### 7.3 按类型查找

```csharp
// 查找场景中第一个指定类型的对象
PlayerController player = FindObjectOfType<PlayerController>();

// 查找所有指定类型的对象
Enemy[] enemies = FindObjectsOfType<Enemy>();

// 包含非激活对象（Unity 2020.1+）
Enemy[] allEnemies = FindObjectsOfType<Enemy>(true);

// Unity 2021+ 新 API（性能更好）
PlayerController player = FindFirstObjectByType<PlayerController>();
Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
```

### 7.4 查找方法性能对比

| 方法 | 性能 | 适用场景 |
|------|------|----------|
| `Find()` | 差 | 仅在初始化时使用 |
| `FindWithTag()` | 中 | 比 Find 快，但仍应缓存结果 |
| `FindGameObjectsWithTag()` | 中 | 需要所有同标签对象时 |
| `FindObjectOfType<T>()` | 差 | 仅在初始化时使用 |
| 直接引用（Inspector 拖拽） | 最佳 | 首选方式 |
| 单例模式 | 好 | 管理器类 |

---

## 八、重要属性一览

| 属性 | 类型 | 说明 |
|------|------|------|
| `transform` | Transform | 该对象的 Transform 组件（每个对象必有） |
| `name` | string | 对象名称 |
| `tag` | string | 对象标签 |
| `layer` | int | 对象所在层级（0-31） |
| `scene` | Scene | 对象所属场景 |
| `sceneCullingMask` | ulong | 场景剔除遮罩 |
| `isStatic` | bool | 是否为静态对象 |
| `hideFlags` | HideFlags | 隐藏标志（编辑器相关） |

---

## 九、静态方法汇总

| 方法 | 说明 |
|------|------|
| `CreatePrimitive(PrimitiveType)` | 创建基本几何体 |
| `Find(string name)` | 按名称查找 |
| `FindWithTag(string tag)` | 按标签查找单个 |
| `FindGameObjectsWithTag(string tag)` | 按标签查找所有 |
| `FindObjectOfType<T>()` | 按类型查找单个 |
| `FindObjectsOfType<T>()` | 按类型查找所有 |
| `Instantiate()` | 实例化（继承自 Object） |
| `Destroy()` | 销毁（继承自 Object） |
| `DontDestroyOnLoad()` | 跨场景保留（继承自 Object） |

---

## 十、注意要点与最佳实践

### 常见陷阱

1. **Find 系列方法性能问题**
```csharp
// 错误：每帧查找
void Update()
{
    GameObject player = GameObject.Find("Player"); // 性能差！
    player.transform.position = newPos;
}

// 正确：缓存引用
private GameObject player;
void Start()
{
    player = GameObject.Find("Player");
}
void Update()
{
    player.transform.position = newPos;
}
```

2. **Destroy 是延迟执行的**
```csharp
Destroy(gameObject);
Debug.Log(gameObject.name); // 仍然可以访问！（本帧内）

// 如果需要立即判断
if (gameObject != null) // Destroy 后 == null 返回 true
{
    // 但实际对象还存在直到帧末
}
```

3. **禁用父对象影响子对象**
```csharp
parent.SetActive(false);
// 所有子对象的 activeInHierarchy 都变为 false
// 子对象的协程会停止
// 子对象的 Update 不再调用
```

4. **GetComponent 返回 null 的处理**
```csharp
// 推荐使用 TryGetComponent
if (TryGetComponent<Rigidbody>(out var rb))
{
    rb.AddForce(Vector3.up);
}
else
{
    Debug.LogWarning("Missing Rigidbody!");
}
```

### 最佳实践

1. **优先使用 Inspector 引用而非 Find**
```csharp
// 在 Inspector 中拖拽赋值
[SerializeField] private GameObject player;
```

2. **使用对象池而非频繁 Instantiate/Destroy**
```csharp
// 对于频繁创建销毁的对象（如子弹），使用对象池
public class BulletPool : MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(bulletPrefab);
    }
    
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

3. **合理使用标签和层级**
- Tag：用于识别对象类型（Player, Enemy, Pickup）
- Layer：用于物理碰撞过滤、相机渲染过滤、射线检测过滤

4. **组件引用缓存**
```csharp
private Rigidbody rb;
private Animator anim;

void Awake()
{
    rb = GetComponent<Rigidbody>();
    anim = GetComponent<Animator>();
}
```

---

## 十一、典型应用场景

| 场景 | 使用的功能 |
|------|------------|
| 动态生成敌人 | `Instantiate()` + 预制体 |
| 子弹命中后销毁 | `Destroy(gameObject)` |
| 单例管理器 | `DontDestroyOnLoad()` |
| 碰撞检测过滤 | Layer + LayerMask |
| 拾取物品识别 | Tag + `CompareTag()` |
| 禁用/启用 UI 面板 | `SetActive()` |
| 获取玩家引用 | `FindWithTag("Player")` 或 Inspector 引用 |
| 批量处理同类对象 | `FindGameObjectsWithTag()` |

---

## 十二、与 UnityEngine.Object 的关系

GameObject 继承自 `UnityEngine.Object`，因此拥有以下特性：

| 继承的成员 | 说明 |
|------------|------|
| `name` | 对象名称 |
| `hideFlags` | 隐藏标志 |
| `GetInstanceID()` | 获取唯一实例 ID |
| `ToString()` | 返回对象名称 |
| `Destroy()` | 静态销毁方法 |
| `Instantiate()` | 静态实例化方法 |
| `DontDestroyOnLoad()` | 跨场景保留 |
| `FindObjectOfType()` | 按类型查找 |

### 特殊的 null 比较

```csharp
GameObject obj = ...;
Destroy(obj);

// Unity 重写了 == 运算符
if (obj == null) // true（即使 C# 对象还存在）
{
    // Unity 认为已销毁
}

// 真正的 null 检查（不推荐）
if (ReferenceEquals(obj, null))
{
    // C# 层面的 null
}
```
