# MonoBehaviour 类详解

## 思维导图总览

```
MonoBehaviour
├── 概念定义
│   ├── Unity中所有脚本的基类
│   ├── 继承关系：Object → Component → Behaviour → MonoBehaviour
│   └── 命名空间：UnityEngine
│
├── 生命周期函数（按执行顺序）
│   ├── 初始化阶段
│   │   ├── Awake()
│   │   ├── OnEnable()
│   │   └── Start()
│   ├── 物理更新阶段
│   │   └── FixedUpdate()
│   ├── 逻辑更新阶段
│   │   ├── Update()
│   │   └── LateUpdate()
│   ├── 渲染阶段
│   │   ├── OnGUI()
│   │   ├── OnRenderObject()
│   │   └── OnPostRender()
│   ├── 禁用/销毁阶段
│   │   ├── OnDisable()
│   │   └── OnDestroy()
│   └── 其他回调
│       ├── OnApplicationPause()
│       ├── OnApplicationQuit()
│       └── OnApplicationFocus()
│
├── 协程系统
│   ├── StartCoroutine()
│   ├── StopCoroutine()
│   ├── StopAllCoroutines()
│   └── yield指令
│       ├── yield return null
│       ├── yield return new WaitForSeconds()
│       ├── yield return new WaitForFixedUpdate()
│       ├── yield return new WaitUntil()
│       ├── yield return new WaitWhile()
│       └── yield return new WaitForEndOfFrame()
│
├── 消息机制
│   ├── Invoke()
│   ├── InvokeRepeating()
│   ├── CancelInvoke()
│   ├── SendMessage()
│   ├── BroadcastMessage()
│   └── SendMessageUpwards()
│
├── 重要属性
│   ├── enabled
│   ├── gameObject
│   ├── transform
│   ├── tag
│   └── name
│
└── 注意要点
    ├── 不能用new创建，必须通过AddComponent
    ├── 生命周期函数不需要override
    ├── Awake vs Start 的区别
    ├── Update vs FixedUpdate 的使用场景
    └── 协程在对象禁用时会停止
```

---

## 一、概念定义

| 项目 | 说明 |
|------|------|
| **是什么** | Unity 中所有用户脚本的基类，提供了生命周期管理、协程、消息机制等核心功能 |
| **继承链** | `System.Object` → `UnityEngine.Object` → `Component` → `Behaviour` → `MonoBehaviour` |
| **命名空间** | `UnityEngine` |
| **核心作用** | 让 C# 脚本能够挂载到 GameObject 上，参与 Unity 的生命周期管理 |

---

## 二、生命周期函数详解

### 执行顺序图

```
场景加载
  │
  ▼
Awake() ──────────── 最早调用，用于初始化自身引用
  │
  ▼
OnEnable() ────────── 对象激活时调用
  │
  ▼
Start() ───────────── 第一次Update前调用，用于初始化依赖其他对象的逻辑
  │
  ▼
┌─────────────── 游戏循环 ───────────────┐
│                                         │
│  FixedUpdate() ── 固定时间步长（物理）    │
│       │                                 │
│       ▼                                 │
│  Update() ─────── 每帧调用（主逻辑）     │
│       │                                 │
│       ▼                                 │
│  LateUpdate() ── Update之后（相机跟随）  │
│                                         │
└─────────────────────────────────────────┘
  │
  ▼
OnDisable() ───────── 对象禁用时调用
  │
  ▼
OnDestroy() ───────── 对象销毁时调用
```

### 2.1 Awake()

```csharp
void Awake()
{
    // 在脚本实例被加载时调用（即使脚本未启用）
    // 用途：初始化自身变量、获取自身组件引用
}
```

| 特性 | 说明 |
|------|------|
| **调用时机** | 脚本实例加载时，在所有 Start() 之前 |
| **调用次数** | 仅一次 |
| **是否受 enabled 影响** | 否，即使脚本 disabled 也会调用 |
| **典型用途** | 初始化自身引用、单例模式初始化 |

### 2.2 OnEnable()

```csharp
void OnEnable()
{
    // 对象被激活时调用
    // 用途：注册事件、订阅委托
}
```

| 特性 | 说明 |
|------|------|
| **调用时机** | 对象从禁用变为启用时 |
| **调用次数** | 每次启用都会调用 |
| **典型用途** | 事件订阅、重置状态 |

### 2.3 Start()

```csharp
void Start()
{
    // 在第一次 Update 之前调用
    // 用途：初始化依赖其他对象的逻辑
}
```

| 特性 | 说明 |
|------|------|
| **调用时机** | 第一次 Update 之前，所有 Awake 之后 |
| **调用次数** | 仅一次 |
| **是否受 enabled 影响** | 是，脚本必须启用才会调用 |
| **典型用途** | 获取其他对象引用、初始化游戏逻辑 |

### 2.4 FixedUpdate()

```csharp
void FixedUpdate()
{
    // 固定时间间隔调用（默认0.02秒）
    // 用途：物理相关计算
    rb.AddForce(Vector3.forward * force);
}
```

| 特性 | 说明 |
|------|------|
| **调用时机** | 固定时间步长，与帧率无关 |
| **默认间隔** | 0.02秒（可在 Edit → Project Settings → Time 修改） |
| **典型用途** | Rigidbody 物理操作、力的施加 |
| **注意** | 不要在这里处理输入检测（可能丢失输入） |

### 2.5 Update()

```csharp
void Update()
{
    // 每帧调用一次
    // 用途：游戏主逻辑、输入检测、非物理移动
    float h = Input.GetAxis("Horizontal");
    transform.Translate(Vector3.right * h * speed * Time.deltaTime);
}
```

| 特性 | 说明 |
|------|------|
| **调用时机** | 每帧调用 |
| **调用频率** | 取决于帧率（不固定） |
| **典型用途** | 输入检测、非物理移动、游戏逻辑 |
| **注意** | 移动必须乘以 `Time.deltaTime` 保证帧率无关 |

### 2.6 LateUpdate()

```csharp
void LateUpdate()
{
    // 在所有 Update 执行完毕后调用
    // 用途：相机跟随、最终位置修正
    transform.position = target.position + offset;
}
```

| 特性 | 说明 |
|------|------|
| **调用时机** | 所有 Update() 执行完毕后 |
| **典型用途** | 相机跟随、骨骼动画后处理 |
| **优势** | 确保跟随目标已完成本帧移动 |

### 2.7 OnDisable() / OnDestroy()

```csharp
void OnDisable()
{
    // 对象禁用时调用
    // 用途：取消事件订阅、清理资源
}

void OnDestroy()
{
    // 对象销毁时调用
    // 用途：最终清理、保存数据
}
```

---

## 三、协程系统（Coroutine）

### 3.1 概念

协程是一种可以暂停执行并在之后恢复的特殊函数，用于处理需要跨帧执行的逻辑（如延时、动画、异步加载）。

### 3.2 核心方法

| 方法 | 签名 | 说明 |
|------|------|------|
| **StartCoroutine** | `Coroutine StartCoroutine(IEnumerator routine)` | 启动协程 |
| **StartCoroutine** | `Coroutine StartCoroutine(string methodName)` | 通过方法名启动（可被单独停止） |
| **StopCoroutine** | `void StopCoroutine(Coroutine routine)` | 停止指定协程 |
| **StopCoroutine** | `void StopCoroutine(string methodName)` | 通过方法名停止 |
| **StopAllCoroutines** | `void StopAllCoroutines()` | 停止该脚本所有协程 |

### 3.3 yield 指令详解

| yield 语句 | 含义 |
|------------|------|
| `yield return null` | 等待下一帧 |
| `yield return new WaitForSeconds(t)` | 等待 t 秒（受 Time.timeScale 影响） |
| `yield return new WaitForSecondsRealtime(t)` | 等待 t 秒（不受 timeScale 影响） |
| `yield return new WaitForFixedUpdate()` | 等待下一次 FixedUpdate |
| `yield return new WaitForEndOfFrame()` | 等待当前帧渲染完毕 |
| `yield return new WaitUntil(() => condition)` | 等待条件为 true |
| `yield return new WaitWhile(() => condition)` | 等待条件为 false |
| `yield return StartCoroutine(other)` | 等待另一个协程完成 |
| `yield return asyncOperation` | 等待异步操作完成（如场景加载） |

### 3.4 协程示例

```csharp
IEnumerator SpawnEnemies()
{
    while (true)
    {
        SpawnEnemy();
        yield return new WaitForSeconds(2f); // 每2秒生成一个敌人
    }
}

void Start()
{
    StartCoroutine(SpawnEnemies());
}
```

### 3.5 协程注意事项

- 协程不是多线程，仍在主线程执行
- GameObject 被禁用（SetActive(false)）时协程会停止且不会恢复
- 脚本 enabled = false 时协程仍会继续运行
- 对象被销毁时协程自动停止
- 避免在协程中使用 `yield return new WaitForSeconds(0)` ，用 `yield return null` 代替

---

## 四、消息与延时调用

### 4.1 Invoke 系列

| 方法 | 签名 | 说明 |
|------|------|------|
| **Invoke** | `void Invoke(string methodName, float time)` | 延时调用方法 |
| **InvokeRepeating** | `void InvokeRepeating(string methodName, float time, float repeatRate)` | 延时后重复调用 |
| **CancelInvoke** | `void CancelInvoke()` | 取消所有 Invoke |
| **CancelInvoke** | `void CancelInvoke(string methodName)` | 取消指定方法的 Invoke |
| **IsInvoking** | `bool IsInvoking(string methodName)` | 判断是否正在 Invoke |

```csharp
// 2秒后调用 Explode 方法
Invoke("Explode", 2f);

// 1秒后开始，每0.5秒调用一次 Fire
InvokeRepeating("Fire", 1f, 0.5f);
```

### 4.2 SendMessage 系列

| 方法 | 作用范围 | 说明 |
|------|----------|------|
| **SendMessage** | 当前 GameObject | 调用该对象上所有脚本中的指定方法 |
| **BroadcastMessage** | 当前对象 + 所有子对象 | 向下广播 |
| **SendMessageUpwards** | 当前对象 + 所有父对象 | 向上发送 |

```csharp
// 调用当前对象上所有脚本的 TakeDamage 方法
gameObject.SendMessage("TakeDamage", 10f);

// 参数说明：
// methodName: 方法名（字符串）
// value: 传递的参数（可选）
// options: SendMessageOptions.RequireReceiver（默认，找不到方法报错）
//          SendMessageOptions.DontRequireReceiver（找不到不报错）
```

> **注意**：SendMessage 使用反射，性能较差，建议用接口、事件或委托替代。

---

## 五、重要属性

| 属性 | 类型 | 说明 | 来源 |
|------|------|------|------|
| `enabled` | bool | 是否启用该脚本 | Behaviour |
| `gameObject` | GameObject | 该脚本附着的游戏对象 | Component |
| `transform` | Transform | 该对象的 Transform 组件（缓存引用） | Component |
| `tag` | string | 对象的标签 | Component |
| `name` | string | 对象名称 | Object |
| `isActiveAndEnabled` | bool | 对象激活且脚本启用 | Behaviour |

---

## 六、常用继承方法

| 方法 | 来源 | 说明 |
|------|------|------|
| `GetComponent<T>()` | Component | 获取同对象上的组件 |
| `GetComponentInChildren<T>()` | Component | 获取子对象上的组件 |
| `GetComponentInParent<T>()` | Component | 获取父对象上的组件 |
| `GetComponents<T>()` | Component | 获取同对象上所有同类型组件 |
| `Destroy(obj)` | Object | 销毁对象 |
| `Instantiate(obj)` | Object | 实例化对象 |
| `DontDestroyOnLoad(obj)` | Object | 切换场景时不销毁 |
| `FindObjectOfType<T>()` | Object | 查找场景中指定类型对象 |

---

## 七、Awake vs Start 对比

| 对比项 | Awake | Start |
|--------|-------|-------|
| 调用时机 | 脚本实例加载时 | 第一次 Update 前 |
| 执行顺序 | 所有 Awake 先于所有 Start | 在 Awake 之后 |
| 是否受 enabled 影响 | 否 | 是 |
| 推荐用途 | 初始化自身（单例、缓存组件） | 初始化依赖外部的逻辑 |
| 能否保证其他对象已初始化 | 不能 | 能（其他对象的 Awake 已执行） |

---

## 八、注意要点与最佳实践

### 常见陷阱

1. **不能用 new 创建 MonoBehaviour**
```csharp
// 错误！
MyScript script = new MyScript();

// 正确
MyScript script = gameObject.AddComponent<MyScript>();
```

2. **构造函数不可靠** - 不要在构造函数中写逻辑，使用 Awake/Start

3. **Update 中的性能问题**
```csharp
// 差：每帧 GetComponent 开销大
void Update()
{
    GetComponent<Rigidbody>().AddForce(Vector3.up);
}

// 好：缓存引用
private Rigidbody rb;
void Awake()
{
    rb = GetComponent<Rigidbody>();
}
void FixedUpdate()
{
    rb.AddForce(Vector3.up);
}
```

4. **空引用检查** - 在使用外部引用前务必判空

### 最佳实践

- Awake 中初始化自身，Start 中初始化依赖
- 缓存频繁使用的组件引用
- 物理操作放 FixedUpdate，输入检测放 Update
- 相机跟随放 LateUpdate
- 用协程替代 Invoke（更灵活、可传参、可控制）
- 用事件/委托替代 SendMessage（性能更好、类型安全）

---

## 九、典型应用场景

| 场景 | 使用的生命周期/功能 |
|------|---------------------|
| 玩家控制器 | Update（输入）、FixedUpdate（物理移动） |
| 相机跟随 | LateUpdate |
| 定时生成敌人 | 协程 / InvokeRepeating |
| 单例管理器 | Awake + DontDestroyOnLoad |
| UI 管理 | OnEnable/OnDisable（事件订阅） |
| 对象池 | Awake（预生成）、OnEnable/OnDisable（重用） |
| 场景过渡 | 协程 + AsyncOperation |
