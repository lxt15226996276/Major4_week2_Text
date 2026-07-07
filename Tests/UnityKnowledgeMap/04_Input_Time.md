Time# Input 与 Time 系统详解

## 思维导图总览

```
Input & Time
├── Input 输入系统
│   ├── 键盘 GetKey / GetKeyDown / GetKeyUp
│   ├── 鼠标 GetMouseButton / position / scrollDelta
│   ├── 虚拟轴 GetAxis / GetAxisRaw
│   ├── 触摸 Input.touches（移动端）
│   └── Input Manager 配置（Edit → Project Settings）
│
└── Time 时间系统
    ├── deltaTime 帧间隔（Update 必用）
    ├── fixedDeltaTime 物理步长
    ├── time 游戏开始至今秒数
    ├── timeScale 时间缩放（0=暂停）
    ├── unscaledDeltaTime 不受 timeScale 影响
    └── frameCount / realtimeSinceStartup
```

---

## 一、Input 概念

| 项目 | 说明 |
|------|------|
| **命名空间** | `UnityEngine` |
| **类型** | 静态类，无需实例化 |
| **作用** | 读取键盘、鼠标、手柄、触摸等玩家输入 |
| **新版替代** | Unity Input System 包（本单元以经典 Input 为准） |

---

## 二、键盘输入

| 方法 | 调用时机 |
|------|----------|
| `GetKey(KeyCode)` | 按住期间每帧 true |
| `GetKeyDown(KeyCode)` | 按下那一帧 true |
| `GetKeyUp(KeyCode)` | 松开那一帧 true |

```csharp
void Update()
{
  if (Input.GetKeyDown(KeyCode.Space))
    Fire();

  if (Input.GetKey(KeyCode.W))
    MoveForward();
}
```

---

## 三、鼠标输入

| 方法/属性 | 说明 |
|-----------|------|
| `GetMouseButton(0/1/2)` | 左/右/中键按住 |
| `GetMouseButtonDown` | 按下瞬间 |
| `mousePosition` | 屏幕像素坐标（左下为原点） |
| `GetAxis("Mouse X")` | 水平移动量 |
| `GetAxis("Mouse Y")` | 垂直移动量 |
| `scrollDelta` | 滚轮增量 |

---

## 四、虚拟轴（Axis）

```csharp
float h = Input.GetAxis("Horizontal");   // -1 ~ 1，有过渡
float v = Input.GetAxisRaw("Vertical");  // -1/0/1，无过渡
```

| 对比 | GetAxis | GetAxisRaw |
|------|---------|------------|
| 返回值 | 平滑渐变 | 立即 -1/0/1 |
| 适用 | 角色移动、镜头 | 需要即时响应的操作 |

> 轴名称在 **Edit → Project Settings → Input Manager** 中配置。

---

## 五、Time 时间系统

### 5.1 核心属性

| 属性 | 说明 | 常用场景 |
|------|------|----------|
| `deltaTime` | 上一帧到本帧的时间（秒） | Update 中移动、旋转、计时 |
| `fixedDeltaTime` | 固定物理步长（默认 0.02s） | FixedUpdate 逻辑参考 |
| `time` | 游戏开始后的 scaled 时间 | 技能 CD、游戏计时 |
| `unscaledTime` | 不受 timeScale 影响的时间 | 暂停菜单动画 |
| `timeScale` | 时间流速（1=正常，0=暂停） | 慢动作、暂停 |
| `unscaledDeltaTime` | 不受 timeScale 的帧间隔 | 暂停界面 UI 动画 |
| `frameCount` | 已渲染帧数 | 调试 |
| `realtimeSinceStartup` | 真实时间（含暂停） | 性能测试 |

### 5.2 帧率无关移动

```csharp
void Update()
{
  // 无论 30fps 还是 144fps，每秒移动 speed 单位
  transform.Translate(Vector3.forward * speed * Time.deltaTime);
}
```

### 5.3 时间缩放

```csharp
Time.timeScale = 0f;    // 暂停（物理、动画、WaitForSeconds 均停）
Time.timeScale = 0.5f;  // 慢动作
Time.timeScale = 1f;    // 恢复正常
```

> **易错点**：`timeScale=0` 时 `Update` 仍执行，但 `Time.deltaTime` 为 0；`WaitForSeconds` 会停，`WaitForSecondsRealtime` 不会。

---

## 六、Update vs FixedUpdate 与 Time

| 函数 | 使用的时间 | 用途 |
|------|------------|------|
| Update | `Time.deltaTime` | 输入检测、非物理移动 |
| FixedUpdate | `Time.fixedDeltaTime` | Rigidbody 力、物理 |

```csharp
void Update()
{
  float h = Input.GetAxis("Horizontal"); // 输入放 Update
  moveInput = h;
}

void FixedUpdate()
{
  rb.velocity = new Vector3(moveInput * speed, rb.velocity.y, rb.velocity.z);
}
```

---

## 七、注意要点

| 易错点 | 说明 |
|--------|------|
| 输入写在 FixedUpdate | 可能丢按键，应放 Update |
| 移动未乘 deltaTime | 帧率越高移动越快 |
| 用 Time.time 做精确物理 | 应用 fixedDeltaTime 或 FixedUpdate |
| 暂停后 UI 不动 | UI 动画用 unscaledDeltaTime |

---

## 八、典型应用（坦克/射击类）

| 需求 | 实现 |
|------|------|
| WASD 移动 | GetAxis("Horizontal"/"Vertical") + deltaTime |
| 空格开火 | GetKeyDown(KeyCode.Space) |
| 鼠标瞄准 | mousePosition → ScreenToWorldPoint |
| 游戏暂停 | Time.timeScale = 0 |
| 射击冷却 | Time.time 比较上次射击时间 |
