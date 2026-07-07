# Unity 常用的类 · 综合思维导图

> **参照来源（取其精华）**
> - 杜亚南《新印象 Unity2020游戏开发基础与实战》— 组件化思想、常用 API 入门
> - 马遥《Unity 3D完全自学教程》— 初识类关系、Inspector 与脚本挂载
> - 宣雨松《Unity 3D 游戏开发（第3版）》— 生命周期、Transform、物理基础
> - 陈俊宇《Unity 游戏开发从入门到精通》— GameObject/组件/预制体体系
> - 王磊《Unity 2022 游戏开发完全学习手册》— 新版 API、场景与资源管理
> - 吴亚峰《Unity 游戏开发实战（第2版）》— 实战类用法与常见模式
> - Unity 官方 Manual + Scripting API（**最终标准**）
>
> **已剔除的糟粕**：SendMessage 作为首选通信、运行时 DestroyImmediate、每帧 Find、JavaScript 脚本、OnGUI 做正式 UI、过度依赖 Resources 大包体、未说明 Object 伪 null 等教材过时写法。

---

## 使用频率图例

| 标记 | 含义 | 学习优先级 |
|------|------|------------|
| ⭐⭐⭐ | 几乎每个脚本都会用 | 第一周必须掌握 |
| ⭐⭐ | 项目常见，按模块必学 | 第二～三周 |
| ⭐ | 特定功能才用 | 遇到再学 |

---

## 思维导图总览

```
Unity 常用的类
│
├── 【第一梯队】对象体系 — 先搞懂"谁是谁" ★★★
│   ├── UnityEngine.Object ─── 所有 Unity 引用对象的基类
│   ├── GameObject ─────────── 场景里的"空壳容器"
│   ├── Component ──────────── 所有组件的基类
│   ├── Transform ──────────── 位置/旋转/缩放（必有组件）
│   └── MonoBehaviour ──────── 自定义脚本的基类
│
├── 【第二梯队】静态工具类 — 不用挂载，直接调用 ★★★
│   ├── Input ──────────────── 键盘/鼠标/轴输入
│   ├── Time ───────────────── deltaTime / timeScale
│   ├── Physics ────────────── 射线、重力、Overlap 检测
│   ├── Debug ──────────────── Log / DrawRay 调试
│   └── SceneManager ───────── 场景加载切换
│
├── 【第三梯队】数学与随机 — 坐标与计算 ★★★
│   ├── Vector2 / Vector3 ──── 位置、方向、距离
│   ├── Quaternion ─────────── 旋转（替代欧拉角）
│   ├── Mathf ──────────────── Clamp / Lerp / Sin / Atan2
│   └── Random ─────────────── Range / value / insideUnitSphere
│
├── 【第四梯队】物理组件 ★★
│   ├── Rigidbody ──────────── 刚体运动
│   ├── Collider 系列 ──────── 碰撞/触发
│   └── RaycastHit ─────────── 射线命中信息
│
├── 【第五梯队】渲染与资源 ★★
│   ├── Camera ─────────────── 观察与坐标转换
│   ├── Renderer / Material ── 显示外观
│   ├── MeshFilter + MeshRenderer（3D 显示组合）
│   └── Prefab + Instantiate ─ 预制体实例化（Object API）
│
├── 【第六梯队】音频 ★
│   ├── AudioClip ──────────── 音频资源
│   ├── AudioSource ────────── 播放组件
│   └── AudioListener ──────── 听者（挂主相机）
│
├── 【第六梯队+】视频 ★
│   ├── VideoClip ──────────── 视频资源（UnityEngine.Video）
│   ├── VideoPlayer ────────── 播放组件
│   └── RenderTexture / Camera ─ 画面输出目标
│
├── 【第六梯队+】粒子特效 ★
│   ├── ParticleSystem ─────── 粒子模拟（main/emission/shape）
│   ├── ParticleSystemRenderer 渲染粒子
│   └── Prefab + Play/Stop ─── 枪口火/爆炸/烟雾
│
├── 【脚本机制】协程 ★★
│   ├── StartCoroutine / StopCoroutine
│   ├── IEnumerator + yield return
│   └── WaitForSeconds / WaitUntil / AsyncOperation
│
├── 【第七梯队】UI（UnityEngine.UI）★
│   ├── Canvas ─────────────── UI 根
│   ├── RectTransform ──────── UI 布局变换
│   ├── EventSystem ────────── 点击事件总线
│   └── Button / Image / Text ─ 常用控件
│
└── 【第八梯队】数据与持久化 ★
    ├── PlayerPrefs ────────── 简单本地存档
    ├── ScriptableObject ───── 配置型数据资产
    └── Resources.Load ─────── 入门资源加载（进阶用 Addressables）
```

---

## 一、类继承关系（官方标准 · 必背）

```
System.Object                          ← C# 语言基类
└── UnityEngine.Object                 ← ★★★ Unity 引用对象基类
        ├── GameObject                 ← ★★★ 场景实体容器
        ├── Component                  ← ★★★ 组件基类
        │     ├── Transform            ← ★★★ 必有，不可删
        │     ├── Rigidbody            ← ★★  物理
        │     ├── ParticleSystem       ← ★   粒子
        │     ├── Collider (子类)      ← ★★  碰撞
        │     ├── Renderer (子类)      ← ★★  渲染
        │     │     └── ParticleSystemRenderer
        │     └── Behaviour
        │           ├── MonoBehaviour  ← ★★★ 你的脚本
        │           ├── Camera         ← ★★
        │           ├── AudioSource    ← ★
        │           ├── AudioListener  ← ★
        │           └── VideoPlayer    ← ★  (UnityEngine.Video)
        ├── Material / Texture / AudioClip / VideoClip / Sprite …  ← 资源类
        └── ScriptableObject           ← ★  数据配置资产

结构体（不继承 Object）：Vector2/3、Quaternion、Color、Ray、RaycastHit
静态类（不能实例化）：Input、Time、Physics、Debug、SceneManager、Mathf、Random、PlayerPrefs
```

**[重难点]** GameObject 是容器，功能全在 Component 上；MonoBehaviour 是 Component 的一种，你的脚本必须挂到 GameObject 上才能运行。

**[易错点]** 不能用 `new MonoBehaviour()`；不能 `new GameObject` 替代 Prefab 工作流（可以 new，但项目里 90% 用 Instantiate）。

---

## 二、第一梯队 · 核心对象类（⭐⭐⭐）

### 2.1 UnityEngine.Object

| 项目 | 内容 |
|------|------|
| **是什么** | 所有可被 Unity 编辑器引用、可拖进 Inspector 的对象基类 |
| **官方** | [Manual: Object](https://docs.unity3d.com/Manual/class-Object.html) · [API: Object](https://docs.unity3d.com/ScriptReference/Object.html) |
| **常用静态方法** | `Instantiate` · `Destroy` · `DontDestroyOnLoad` · `FindFirstObjectByType` |

**[重难点] Unity 伪 null**
```csharp
Destroy(obj);
if (obj == null) { }        // true — Unity 重载了 ==
if (obj?.name) { }          // 不安全！?. 和 ?? 不能正确识别已 Destroy 的对象
if (ReferenceEquals(obj, null)) { } // false — C# 层对象还在
```

**[易错点]** Destroy 是**帧末**才真正销毁，当帧内还能访问。

---

### 2.2 GameObject ⭐⭐⭐

| 常用 API | 频率 | 用途 |
|----------|------|------|
| `AddComponent<T>()` | ⭐⭐⭐ | 运行时加组件 |
| `GetComponent<T>()` | ⭐⭐⭐ | 取组件（Awake 缓存） |
| `TryGetComponent<T>(out T)` | ⭐⭐⭐ | 安全取组件 |
| `Instantiate(prefab)` | ⭐⭐⭐ | 生成子弹/敌人 |
| `Destroy(gameObject)` | ⭐⭐⭐ | 销毁自己 |
| `SetActive(bool)` | ⭐⭐⭐ | 显示/隐藏 |
| `CompareTag("Player")` | ⭐⭐ | 碰撞识别 |
| `Find` / `FindWithTag` | ⭐ | **仅初始化时**用 |

**[重难点] activeSelf vs activeInHierarchy**
- 父物体禁用 → 子物体 `activeSelf` 可能 true，但 `activeInHierarchy` 为 false，Update 不执行。

**[易错点]**
- 每帧 `Find("Player")` → 性能灾难
- `tag == "Enemy"` 产生 GC → 改用 `CompareTag`
- 禁用 GameObject 后协程**停止且不恢复**

---

### 2.3 Component ⭐⭐⭐

| 属性/方法 | 说明 |
|-----------|------|
| `gameObject` | 组件所在对象 |
| `transform` | 快捷访问 Transform（缓存优化） |
| `tag` / `GetComponent` 系列 | 继承给所有组件 |

**[教材精华·吴/陈]** 组件模式 = 拼装功能，而非继承巨型父类。

---

### 2.4 Transform ⭐⭐⭐

| 分类 | 常用成员 | 频率 |
|------|----------|------|
| 位置 | `position` · `localPosition` · `Translate` | ⭐⭐⭐ |
| 旋转 | `rotation` · `LookAt` · `Rotate` | ⭐⭐⭐ |
| 缩放 | `localScale` | ⭐⭐ |
| 层级 | `SetParent` · `GetChild` · `parent` | ⭐⭐⭐ |
| 转换 | `TransformPoint` · `InverseTransformPoint` | ⭐⭐ |
| 方向 | `forward` · `right` · `up` | ⭐⭐⭐ |

**[重难点]**
```csharp
// Update 中移动必须帧率无关
transform.Translate(Vector3.forward * speed * Time.deltaTime);
// 在物体前方 2 米生成 — 用 TransformPoint 而非手动算
Vector3 spawn = transform.TransformPoint(0, 0, 2);
```

**[易错点]**
- 改 `eulerAngles` 导致万向节锁 → 用 `Quaternion`
- 相机跟随写 Update → 应写 **LateUpdate**

---

### 2.5 MonoBehaviour ⭐⭐⭐

| 分类 | 内容 | 频率 |
|------|------|------|
| 生命周期 | Awake → OnEnable → Start → Update/FixedUpdate/LateUpdate → OnDisable → OnDestroy | ⭐⭐⭐ |
| 继承属性 | `enabled` · `gameObject` · `transform` | ⭐⭐⭐ |
| 协程 | `StartCoroutine` · `yield return null/WaitForSeconds` | ⭐⭐ |
| 延时 | `Invoke` / `InvokeRepeating` | ⭐ |

**[重难点] 生命周期分工（官方 + 宣/杜）**

| 函数 | 调用特点 | 写什么 |
|------|----------|--------|
| `Awake` | 最早，不受 enabled 影响 | 缓存 GetComponent、单例初始化 |
| `Start` | 第一次 Update 前 | 依赖其他对象的初始化 |
| `Update` | 每帧 | 输入检测、非物理移动 |
| `FixedUpdate` | 固定 0.02s | Rigidbody 力、物理 |
| `LateUpdate` | Update 之后 | 相机跟随 |

**[易错点]**
- 输入放 FixedUpdate → 丢按键
- 物理放 Update → 不稳定
- `new` 创建脚本 → 错误，必须 `AddComponent`

**[糟粕剔除]** SendMessage / BroadcastMessage — API 仍存在，**不作为推荐方案**；用 public 方法、事件、接口代替。

---

## 三、第二梯队 · 静态工具类（⭐⭐⭐）

### 3.1 Input ⭐⭐⭐

| API | 场景 |
|-----|------|
| `GetKeyDown(KeyCode.Space)` | 开火、跳跃（按一次） |
| `GetKey(KeyCode.W)` | 持续按住 |
| `GetAxis("Horizontal")` | WASD 平滑移动 |
| `GetAxisRaw("Vertical")` | 需要即时响应 |
| `mousePosition` | 鼠标指向 |

**[易错点]** 轴名称要在 Input Manager 配置；新版项目可选 **Input System 包**，本导图以经典 Input 为准（教材共性 + 周考常用）。

---

### 3.2 Time ⭐⭐⭐

| 属性 | 用途 | 频率 |
|------|------|------|
| `deltaTime` | Update 移动/旋转/计时 | ⭐⭐⭐ |
| `fixedDeltaTime` | FixedUpdate 参考 | ⭐⭐ |
| `time` | 游戏内计时（技能 CD） | ⭐⭐ |
| `timeScale` | 暂停(0) / 慢动作 | ⭐⭐ |
| `unscaledDeltaTime` | 暂停时 UI 动画 | ⭐ |

**[重难点]** 凡是在 Update 里改变 Transform 或做累加，**几乎都要 × deltaTime**。

---

### 3.3 Physics ⭐⭐

| API | 用途 |
|-----|------|
| `Raycast(ray, out hit, dist, layerMask)` | 射击命中、地面检测 |
| `OverlapSphere(pos, r)` | 爆炸范围 |
| `gravity` | 全局重力向量 |

**[易错点]** 射线要设 LayerMask；Trigger 不参与 Raycast 除非指定 QueryTriggerInteraction。

---

### 3.4 Debug ⭐⭐⭐

`Log` / `LogWarning` / `LogError` / `DrawRay` — 初学每天必用，发布前清理 Log。

---

### 3.5 SceneManager ⭐⭐

```csharp
using UnityEngine.SceneManagement;
SceneManager.LoadScene("Level2");           // 同步，会卡
SceneManager.LoadSceneAsync("Level2");        // 异步，配合加载条
```

**[易错点]** 场景必须加入 Build Settings；异步加载要用协程或 `AsyncOperation`。

---

## 四、第三梯队 · 数学结构体与 Random（⭐⭐⭐）

| 类 | 类型 | 最常用的 5 个 | 易错点 |
|----|------|---------------|--------|
| **Vector3** | 结构体 | zero/up/forward · normalized · Distance · Lerp · Dot | 忘记 normalized 就方向错误 |
| **Quaternion** | 结构体 | LookRotation · Slerp · AngleAxis · identity | 直接改 eulerAngles 插值会抖 |
| **Mathf** | 静态类 | Clamp · Lerp · InverseLerp · Atan2 · PerlinNoise | PerlinNoise 与 Range 用途不同 |
| **Random** | 静态类 | Range · value · insideUnitSphere · onUnitSphere · InitState | 整数 Range 上限不含；勿与 System.Random 混 |
| **Color** | 结构体 | RGBA 0~1 · Color.red | 255 与 0~1 混用 |
| **Ray / RaycastHit** | 结构体 | 配合 Physics.Raycast | hit.collider 可能为 null |

**[教材精华·马/王]** 入门阶段不必推 Quaternion 数学公式，会 `LookRotation` + `Slerp` 即可。

---

### 4.1 Random（UnityEngine.Random）⭐⭐⭐

| 项目 | 内容 |
|------|------|
| **是什么** | Unity 提供的**静态随机数工具类**，用于游戏内概率、刷怪、掉落、散射等 |
| **官方** | [API: Random](https://docs.unity3d.com/ScriptReference/Random.html) |
| **命名空间** | `UnityEngine`（脚本顶部有 `using UnityEngine` 时直接写 `Random.xxx`） |
| **与 Mathf 区别** | **Mathf** 做数学运算（Clamp、三角函数）；**Random** 专门产生随机数，二者分工不同 |

#### 常用 API

| API | 说明 | 频率 |
|-----|------|------|
| `Random.Range(min, max)` | 取范围内随机数（见下方 int/float 规则） | ⭐⭐⭐ |
| `Random.value` | 返回 0.0～1.0 的 float | ⭐⭐⭐ |
| `Random.insideUnitSphere` | 半径 1 球体内的随机 Vector3 | ⭐⭐ |
| `Random.insideUnitCircle` | 半径 1 圆内的随机 Vector2（XZ/XY 平面常用） | ⭐⭐ |
| `Random.onUnitSphere` | 球面上的随机单位方向 | ⭐⭐ |
| `Random.rotation` | 均匀随机旋转（Quaternion） | ⭐ |
| `Random.InitState(seed)` | 固定种子，便于复现同一随机序列 | ⭐ |
| `Random.state` | 读/写随机状态（高级） | ⭐ |

#### Random.Range 规则（**重难点**）

```csharp
// float：min、max 两端都包含 [min, max]
float damage = Random.Range(10f, 20f);   // 10.0 ~ 20.0

// int：min 包含，max 不包含 [min, max)
int index = Random.Range(0, 3);          // 只会是 0、1、2
int dice  = Random.Range(1, 7);          // 1~6 点（吴/宣教材骰子范例）

// 从数组随机一项
int i = Random.Range(0, enemies.Length);
GameObject pick = enemies[i];
```

| 重载 | 区间 | 典型用途 |
|------|------|----------|
| `Range(float, float)` | [min, max] 双端含 | 伤害浮动、射速偏差 |
| `Range(int, int)` | [min, max) 上限不含 | 数组下标、整数骰子、随机枚举 |

#### 代码示例

```csharp
// 在圆形区域内随机一点（刷怪、爆炸碎片）
Vector3 offset = Random.insideUnitSphere * spawnRadius;
Vector3 spawnPos = center.position + offset;

// 随机概率（30% 掉落）
if (Random.value < 0.3f)
    DropItem();

// 炮口散布：在 forward 附近随机偏转
Vector3 spread = Random.insideUnitSphere * 0.1f;
Vector3 fireDir = (transform.forward + spread).normalized;

// 需要可复现随机（测试/回放）时
Random.InitState(12345);
```

#### Random vs System.Random（**易错点**）

| 对比 | `UnityEngine.Random` | `System.Random` |
|------|----------------------|-----------------|
| 调用方式 | `Random.Range(0, 10)` | `new Random().Next(0, 10)` |
| Unity 项目推荐 | ✅ 游戏逻辑首选 | 一般不用，除非纯 C# 库 |
| 与 Mathf | 常配合 `Mathf.RoundToInt(Random.Range(...))` | 无直接关系 |

> 写了 `using System;` 且又写 `Random.Range` 时，若报错歧义，写全名 **`UnityEngine.Random.Range`**。

#### 典型应用场景

| 场景 | 写法 |
|------|------|
| 随机伤害 | `Random.Range(minDmg, maxDmg)` |
| 随机刷怪位置 | `center + Random.insideUnitSphere * r` |
| 抽奖 / 暴击 | `Random.value < 0.2f` |
| 随机播放音效变调 | `source.pitch = Random.Range(0.9f, 1.1f)` |
| 从多个预制体选一个 | `prefabs[Random.Range(0, prefabs.Length)]` |

---

## 五、第四梯队 · 物理类（⭐⭐）

### Rigidbody

| 成员 | 说明 |
|------|------|
| `AddForce(v, ForceMode.Force)` | FixedUpdate 持续力 |
| `AddForce(v, ForceMode.Impulse)` | 跳跃/爆炸冲量 |
| `velocity` | 设速度（慎用瞬移感） |
| `isKinematic` | true=代码驱动，不受力 |
| `useGravity` | 是否受重力 |

### Collider

| 要点 | 说明 |
|------|------|
| `isTrigger=true` | 触发 OnTriggerEnter，无物理弹开 |
| `OnCollisionEnter` | 非 Trigger 物理碰撞 |
| Box/Sphere/Capsule | 常用，性能好 |
| MeshCollider | 静态场景可以，动态刚体慎用 |

**[重难点]** 碰撞回调条件：至少一方有 Rigidbody（或 kinematic 规则），Layer 矩阵要配对。

**[易错点]** 在 Update 里 `AddForce`；用 `transform.position` 驱动 Dynamic 刚体 → 应用力或 MovePosition。

---

## 六、第五梯队 · 渲染与资源（⭐⭐）

| 类 | 作用 | 频率 |
|----|------|------|
| **Camera** | 观察场景；ScreenToWorldPoint / WorldToScreenPoint | ⭐⭐⭐ |
| **MeshFilter** | 持有网格数据 | ⭐⭐ |
| **MeshRenderer** | 显示网格 + Material | ⭐⭐ |
| **Material** | 颜色/贴图/Shader 参数 | ⭐⭐ |
| **Light** | 平行光/点光/聚光 | ⭐ |
| **Prefab + Instantiate** | 复用对象模板 | ⭐⭐⭐ |

**[易错点]** 运行时改 `renderer.material` 会**复制材质**导致内存涨 → 大量对象用 sharedMaterial 或 MaterialPropertyBlock。

---

## 七、第六梯队 · 音频（⭐）

```
AudioClip（资源）→ AudioSource.Play/PlayOneShot → AudioListener（主相机上唯一）
```

| 要点 | 说明 |
|------|------|
| `PlayOneShot` | 短音效叠加，不打断 BGM |
| `spatialBlend` | 0=2D UI/BGM，1=3D 世界音 |
| 全场景只能有 **一个** AudioListener | 多相机项目易踩坑 |

---

## 七点五、第六梯队+ · 视频（⭐）

```
VideoClip（资源）→ VideoPlayer（播放器）→ 渲染目标（Camera / RenderTexture / Material）
     MP4 等              挂 GameObject 上          UI 常用 RawImage + RenderTexture
```

| 要点 | 说明 |
|------|------|
| 命名空间 | `using UnityEngine.Video;` |
| `VideoClip` 模式 | 拖资源 → `player.clip` → `Play()` |
| `Url` 模式 | 先 `Prepare()`，等 `isPrepared` 或 `prepareCompleted` 再播 |
| `renderMode` | CameraFarPlane 过场 / **RenderTexture+RawImage** UI 视频 |
| `loopPointReached` | 播完回调，切场景或关闭面板 |
| 内嵌音频 | `audioOutputMode` + `SetDirectAudioVolume` |

**[易错点]** URL 未 Prepare 黑屏；UI 视频忘把 RenderTexture 赋给 RawImage；与 AudioListener 不同，视频没有“Listener”，靠 renderMode 输出画面。

详见 [11_Video.md](11_Video.md)

---

## 七点六、第六梯队+ · 粒子特效（⭐）

```
Particle Prefab（配置） → ParticleSystem（模拟） → ParticleSystemRenderer（渲染到屏幕）
   编辑器调参              Play/Stop/Emit            材质 + Billboard
```

| 要点 | 说明 |
|------|------|
| 制作方式 | **编辑器调 ParticleSystem 存 Prefab**，脚本只控制播放 |
| `main` 模块 | duration · loop · startLifetime · startSpeed · startSize · simulationSpace |
| `emission` | rateOverTime 持续发射；Burst 瞬间爆发 |
| `shape` | Cone/Sphere 等发射形状 |
| 一次性 FX | `Instantiate(prefab)` + `stopAction = Destroy` |
| 空间 | 爆炸用 **World**；引擎尾焰用 **Local** |

**[易错点]** 模块要先 `var emission = ps.emission` 再改；爆炸勿 loop=true；Instantiate 后不 Destroy 会堆内存。

详见 [12_ParticleSystem.md](12_ParticleSystem.md)

---

## 七点七、协程 Coroutine（⭐⭐）

```
MonoBehaviour → StartCoroutine → IEnumerator 函数 → yield return 等待
```

| 要点 | 说明 |
|------|------|
| 本质 | **主线程**上分帧执行，不是多线程 |
| 写法 | 返回 `IEnumerator`，体内 `yield return` |
| 启动 | `StartCoroutine(MyCo())`；不能直接调 MyCo() 指望跨帧 |
| 停止 | `StopCoroutine(co)` / `StopAllCoroutines()`，建议保存 `Coroutine` 引用 |
| 常用等待 | `null` 下一帧 · `WaitForSeconds` 秒 · `WaitUntil` 条件 |
| 场景加载 | `yield return LoadSceneAsync(...)` |

**[易错点]** SetActive(false) 协程停且不恢复；enabled=false 协程仍跑；WaitForSeconds 受 timeScale 影响。

详见 [13_Coroutine.md](13_Coroutine.md)

---

## 八、第七梯队 · UI 类（⭐）

| 类 | 作用 |
|----|------|
| **Canvas** | UI 根；Render Mode 决定 Overlay/Camera/World |
| **RectTransform** | 替代 Transform 做锚点/布局 |
| **EventSystem** | 没有它 Button 点不了 |
| **Button** | onClick.AddListener |
| **Image / Slider / Text(TMP)** | 显示与交互 |

**[糟粕剔除]** 不要用 **OnGUI** 做正式游戏 UI（仅调试/极老项目）。

---

## 九、第八梯队 · 数据类（⭐）

| 类 | 适用 | 不适用 |
|----|------|--------|
| **PlayerPrefs** | 音量、最高分、关卡进度 | 大量存档、敏感数据 |
| **ScriptableObject** | 武器表、敌人配置 | 需要运行时改场景层级 |
| **Resources.Load** | 小项目快速加载 | 大项目（用 Addressables） |

---

## 十、按学习阶段的使用频率排序

### 第一周（坦克/射击类周考必会）⭐⭐⭐

```
MonoBehaviour → Transform → Input → Time → GameObject
→ Instantiate/Destroy → GetComponent → Debug.Log
→ Vector3 → Quaternion.LookRotation → Random.Range
```

### 第二周 ⭐⭐

```
Rigidbody → Collider → Physics.Raycast
→ Camera → CompareTag/LayerMask
→ StartCoroutine → SceneManager
```

### 第三周及以后 ⭐

```
AudioSource → VideoPlayer → ParticleSystem → Canvas/UI → PlayerPrefs
→ ScriptableObject → Material/Renderer
```

---

## 十一、教材 vs 官方 · 对照速查

| 知识点 | 教材常见写法 | 官方/工程推荐 |
|--------|--------------|---------------|
| 找对象 | `GameObject.Find` | Inspector 拖拽 / 单例 / 注册表 |
| 组件通信 | SendMessage | 公开方法 / 事件 / 接口 |
| 销毁 | DestroyImmediate | Destroy（运行期） |
| 旋转 | 直接改 eulerAngles | Quaternion.Slerp |
| 移动 | transform.position += v | += v * Time.deltaTime |
| 输入 | 仅 GetKey | GetKeyDown 区分单次/持续 |
| 资源 | 全部 Resources | Prefab + 按需 Addressables |
| 空判断 | obj == null | Unity 对象用 ==；注意伪 null |

---

## 十二、一张图串起写脚本时的类关系

```
你写的 TankController : MonoBehaviour
        │
        ├── transform (Transform)  ← 移动/旋转/开火点
        ├── gameObject (GameObject) ← SetActive/Destroy/CompareTag
        │
        ├── Update 里：
        │     Input.GetAxis + Time.deltaTime
        │     → transform.Translate / Rotate
        │
        ├── FixedUpdate 里：
        │     Rigidbody.AddForce
        │
        ├── 开火：
        │     Instantiate(bulletPrefab, firePoint.position, firePoint.rotation)
        │
        └── 命中检测：
              Physics.Raycast → RaycastHit.collider
```

---

## 十三、与分模块详解文档的对应

| 本篇章节 | 详细文档 |
|----------|----------|
| MonoBehaviour | [01_MonoBehaviour.md](01_MonoBehaviour.md) |
| GameObject / Object | [02_GameObject.md](02_GameObject.md) |
| Transform | [03_Transform.md](03_Transform.md) |
| Input / Time | [04_Input_Time.md](04_Input_Time.md) |
| Physics / Rigidbody / Collider | [05_Physics.md](05_Physics.md) |
| Camera / Material / Light | [06_Camera_Rendering.md](06_Camera_Rendering.md) |
| Audio | [07_Audio.md](07_Audio.md) |
| Video | [11_Video.md](11_Video.md) |
| Particle | [12_ParticleSystem.md](12_ParticleSystem.md) |
| Coroutine | [13_Coroutine.md](13_Coroutine.md) |
| UI | [08_UI_System.md](08_UI_System.md) |
| Vector3 / Quaternion / Mathf / Random / Debug | [09_Math_Utilities.md](09_Math_Utilities.md) |
| PlayerPrefs / SceneManager / Prefab | [10_Data_Scene.md](10_Data_Scene.md) |

---

## 十四、自测清单（掌握常用类的最低标准）

- [ ] 能画出 Object → GameObject → Component → MonoBehaviour 继承链
- [ ] 说清 Awake / Start / Update / FixedUpdate 区别与使用场景
- [ ] 解释 Unity 对象 Destroy 后 `== null` 为 true 的原因
- [ ] 会用 GetComponent + Awake 缓存，不在 Update 里重复 Get
- [ ] 移动/旋转代码带 `Time.deltaTime`
- [ ] 会用 Instantiate 和 Destroy
- [ ] 会用 Input.GetAxis + Transform 做角色/坦克移动
- [ ] 知道 CompareTag 优于字符串比较 tag
- [ ] 知道 Physics.Raycast 基本用法
- [ ] 知道 Camera.main.ScreenToWorldPoint 是干什么的
- [ ] 会用 `Random.Range`（分清 int 与 float 的区间规则）
- [ ] 知道 `Random` 是 UnityEngine 的静态类，不是 Mathf 的成员
