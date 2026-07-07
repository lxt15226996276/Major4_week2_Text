# Particle 粒子特效系统详解

## 思维导图总览

```
Particle 粒子特效系统
├── ParticleSystem 粒子系统（核心组件）
│   ├── main 模块：时长/循环/初速度/大小/颜色
│   ├── emission 模块：发射速率 / Burst 爆发
│   ├── shape 模块：发射形状（Cone/Sphere/…）
│   ├── colorOverLifetime / sizeOverLifetime
│   ├── velocityOverLifetime / forceOverLifetime
│   ├── collision / trails / subEmitters（进阶）
│   └── Play / Stop / Pause / Clear
│
├── ParticleSystemRenderer 渲染器
│   ├── material / mesh
│   ├── renderMode（Billboard / Mesh / …）
│   └── sortingOrder / sortingFudge
│
└── 预制体工作流
    ├── 编辑器调效果 → 存 Prefab
    ├── Instantiate 生成一次性特效
    └── StopAction.Destroy 播完自动销毁
```

---

## 一、三者关系

```
特效 Prefab（配置数据）  →  ParticleSystem（模拟器）  →  ParticleSystemRenderer（画到屏幕）
   编辑器里调参              挂在 GameObject 上              同对象上的渲染组件
```

| 对比其他系统 | 音频 | 视频 | **粒子** |
|--------------|------|------|----------|
| 资源 | AudioClip | VideoClip | **Particle Prefab**（整包配置） |
| 核心组件 | AudioSource | VideoPlayer | **ParticleSystem** |
| 输出 | AudioListener | Camera / RT | **Renderer 直接显示** |
| 命名空间 | UnityEngine | UnityEngine.Video | **UnityEngine**（内置） |
| 制作方式 | 导入音频 | 导入视频 | **编辑器可视化调参为主** |

---

## 二、ParticleSystem 核心 API

| 方法/属性 | 说明 |
|-----------|------|
| `Play()` | 开始发射/播放 |
| `Play(bool withChildren)` | 连同子粒子系统一起播 |
| `Stop()` | 停止（可配置是否清空已有粒子） |
| `Stop(bool withChildren, ParticleSystemStopBehavior)` | StopEmitting=停新发但旧粒子继续 |
| `Pause()` / `UnPause()` | 暂停/继续 |
| `Clear()` | 清除当前所有粒子 |
| `Emit(int count)` | 立刻额外喷射 count 个粒子（Instantaneous） |
| `isPlaying` | 是否正在播放 |
| `isPaused` | 是否暂停 |
| `isStopped` | 是否已停止 |
| `isEmitting` | 是否仍在发射新粒子 |
| `particleCount` | 当前存活粒子数 |
| `time` | 当前播放时间 |
| `main` | 主模块（duration、loop、startLifetime 等） |
| `emission` | 发射模块 |
| `shape` | 形状模块 |

```csharp
public class ExplosionFx : MonoBehaviour
{
    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Destroy; // 播完销毁对象
    }

    public void PlayExplosion()
    {
        ps.Play();
    }
}
```

---

## 三、main 主模块（最常用）

通过 `var main = ps.main;` 访问（**必须先存局部变量**，见第六节）。

| 属性 | 说明 |
|------|------|
| `duration` | 发射持续时间（秒） |
| `loop` | 是否循环 |
| `startLifetime` | 每个粒子存活时间 |
| `startSpeed` | 初速度 |
| `startSize` | 初大小 |
| `startColor` | 初颜色 |
| `startDelay` | 开始延迟 |
| `gravityModifier` | 重力影响（0=不受重力） |
| `maxParticles` | 最大粒子数上限 |
| `simulationSpace` | Local / World（父物体移动是否带走粒子） |
| `playOnAwake` | 激活时自动 Play |
| `stopAction` | 停止后：None / Disable / **Destroy** / Callback |

```csharp
var main = ps.main;
main.duration = 1.5f;
main.loop = false;
main.startLifetime = 0.8f;
main.startSpeed = 5f;
main.startSize = 0.3f;
main.startColor = Color.yellow;
main.simulationSpace = ParticleSystemSimulationSpace.World; // 爆炸常设 World
```

---

## 四、常用子模块

### 4.1 emission（发射）

| 属性 | 说明 |
|------|------|
| `enabled` | 是否启用发射 |
| `rateOverTime` | 每秒发射数量 |
| `rateOverDistance` | 按移动距离发射 |
| `SetBursts` / `burst` | 瞬间爆发（如 0 秒时喷 30 个） |

```csharp
var emission = ps.emission;
emission.enabled = true;
emission.rateOverTime = 20f;

// 0 秒瞬间爆发 50 个（适合开火闪光）
var burst = new ParticleSystem.Burst(0f, 50);
emission.SetBursts(new[] { burst });
```

### 4.2 shape（发射形状）

| 属性 | 说明 |
|------|------|
| `enabled` | 是否启用形状 |
| `shapeType` | Sphere / Cone / Box / Circle / Mesh… |
| `angle` | 圆锥角度 |
| `radius` | 球/圆半径 |
| `rotation` | 形状朝向 |

```csharp
var shape = ps.shape;
shape.enabled = true;
shape.shapeType = ParticleSystemShapeType.Cone;
shape.angle = 25f;
shape.radius = 0.1f;
```

### 4.3 生命周期变化（进阶常用）

| 模块 | 作用 |
|------|------|
| `colorOverLifetime` | 颜色渐变（如黄→红→透明） |
| `sizeOverLifetime` | 大小随时间变化 |
| `velocityOverLifetime` | 额外速度 |
| `rotationOverLifetime` | 旋转 |
| `noise` | 扰动/飘动 |
| `collision` | 与场景碰撞（火花落地） |
| `trails` | 拖尾 |
| `subEmitters` | 子粒子（爆炸→烟雾） |

---

## 五、ParticleSystemRenderer

| 属性 | 说明 |
|------|------|
| `material` | 粒子材质（常带透明/Additive Shader） |
| `renderMode` | Billboard / Stretch / HorizontalBillboard / Mesh |
| `sortingOrder` | 2D 排序 |
| `sortingFudge` | 微调深度排序 |
| `maxParticleSize` | 最大显示尺寸 |
| `lengthScale` | 拉伸模式长度 |

> 粒子看不见：先查 **Material** 是否正确、Renderer 是否启用、粒子 `startSize` 是否过小、相机是否对准。

---

## 六、模块 struct 写法（**重难点**）

官方说明：各模块（`main`、`emission` 等）是**指向原生代码的 struct 接口**，不是普通 C# struct。

```csharp
// ❌ 错误：不能直接链式赋值 emission.enabled
// ps.emission.enabled = true;  // 部分版本不编译

// ✅ 正确：先存局部变量再改
var emission = ps.emission;
emission.enabled = true;
emission.rateOverTime = 10f;

// ✅ 常量简写（不必 new MinMaxCurve）
var main = ps.main;
main.startLifetime = 2.0f;   // 等价于 Constant 模式
main.startSpeed = 5.0f;
```

**[易错点]** 改完模块**不需要**再赋回 `ps.main = main`；赋值会立即生效。

---

## 七、典型工作流：一次性特效

### 7.1 编辑器制作 → Prefab → 代码实例化（**最推荐**）

```csharp
public class TankFx : MonoBehaviour
{
    public ParticleSystem muzzleFlashPrefab;  // 枪口火花 Prefab
    public Transform firePoint;

    public void Fire()
    {
        // 生成后自动 Play（Prefab 上 playOnAwake=true）
        ParticleSystem fx = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
        // Prefab 设 stopAction=Destroy，播完自动销毁，无需手动 Destroy
    }
}
```

### 7.2 场景里常驻粒子，按需 Play/Stop

```csharp
public ParticleSystem engineSmoke; // Inspector 拖引用

void Update()
{
    if (isMoving && !engineSmoke.isPlaying)
        engineSmoke.Play();
    else if (!isMoving && engineSmoke.isPlaying)
        engineSmoke.Stop();
}
```

### 7.3 纯代码 Emit（少量补充喷射）

```csharp
var ps = GetComponent<ParticleSystem>();
var emitParams = new ParticleSystem.EmitParams();
emitParams.position = hitPoint;
ps.Emit(emitParams, 10); // 在命中点喷 10 个
```

---

## 八、Stop 行为与自动销毁

| ParticleSystemStopBehavior | 说明 |
|----------------------------|------|
| `StopEmitting` | 停止新发，已有粒子继续直到 lifetime 结束 |
| `StopEmittingAndClear` | 立刻停并清空 |

| ParticleSystemStopAction | 说明 |
|--------------------------|------|
| `None` | 停止后无动作 |
| `Disable` | 禁用 GameObject |
| `Destroy` | **销毁 GameObject**（一次性 FX 常用） |
| `Callback` | 触发 `onStop` 事件 |

**[推荐]** 子弹命中、爆炸等一次性特效 Prefab：`loop=false` + `stopAction=Destroy`。

---

## 九、Local vs World 空间（易错点）

| simulationSpace | 行为 | 适用 |
|-----------------|------|------|
| **Local** | 粒子跟随发射器移动 | 引擎尾焰、附在角色上的光 |
| **World** | 粒子留在世界坐标 | 爆炸、弹壳、地面烟雾 |

---

## 十、注意要点

| 易错点 | 说明 |
|--------|------|
| 模块未存局部变量 | `emission.enabled` 等要先 `var emission = ps.emission` |
| 一次性 FX 不 Destroy | 频繁 Instantiate 不销毁 → 内存/性能问题 |
| loop=true 用于爆炸 | 爆炸应 loop=false |
| World/Local 搞反 | 移动坦克上爆炸应 World |
| 材质不透明 | 火花/烟雾用透明或 Additive 材质 |
| 粒子过大/过小 | 调 startSize 与 Renderer maxParticleSize |
| 与 VFX Graph 混淆 | **入门学 ParticleSystem**；VFX Graph 是高级节点式系统 |
| 过度代码调参 | 复杂效果应在**编辑器**调好存 Prefab，脚本只 Play/Stop |

---

## 十一、典型应用

| 场景 | 实现 |
|------|------|
| 枪口火光 | muzzleFlash Prefab + Instantiate + stopAction.Destroy |
| 爆炸 | 0s Burst + colorOverLifetime 黄→透明 + World 空间 |
| 引擎冒烟 | loop=true + rateOverTime + 移动时 Play |
| 命中火花 | collision 模块 或 Emit(hitPoint) |
| 魔法轨迹 | trails 模块 |
| 脚步尘土 | rateOverDistance + 地面 Layer |
| 拾取闪光 | loop=false + 短 duration + 子物体缩放 |

---

## 十二、与 Audio / Video 对照速查

| 需求 | 音频 | 视频 | 粒子 |
|------|------|------|------|
| 资源形式 | AudioClip | VideoClip | **Particle Prefab** |
| 播放 | source.Play() | player.Play() | **ps.Play()** |
| 一次性 | PlayOneShot | 播完 loopPointReached | **Instantiate + stopAction.Destroy** |
| 循环 | loop=true | player.loop | **main.loop=true** |
| 主要制作 | 导入文件 | 导入文件 | **编辑器调 ParticleSystem** |
| 输出 | 声音 | 画面 | **3D/2D 粒子渲染** |

---

## 十三、官方参考

- [Manual: Particle System](https://docs.unity3d.com/Manual/ParticleSystems.html)
- [API: ParticleSystem](https://docs.unity3d.com/ScriptReference/ParticleSystem.html)
- [API: ParticleSystem.MainModule](https://docs.unity3d.com/ScriptReference/ParticleSystem.MainModule.html)
- [API: ParticleSystemRenderer](https://docs.unity3d.com/ScriptReference/ParticleSystemRenderer.html)
