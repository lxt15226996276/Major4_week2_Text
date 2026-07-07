# Coroutine 协程系统详解

## 思维导图总览

```
Coroutine 协程系统
├── 本质定位
│   ├── 挂在 MonoBehaviour 上的"可暂停函数"
│   ├── 不是多线程，仍在主线程执行
│   └── 返回值类型必须是 IEnumerator
│
├── 启动与停止
│   ├── StartCoroutine(IEnumerator)
│   ├── StartCoroutine(string methodName)
│   ├── StopCoroutine / StopAllCoroutines
│   └── 保存 Coroutine 引用以便精确停止
│
├── yield 等待指令
│   ├── yield return null（下一帧）
│   ├── WaitForSeconds / WaitForSecondsRealtime
│   ├── WaitForFixedUpdate / WaitForEndOfFrame
│   ├── WaitUntil / WaitWhile
│   └── yield return AsyncOperation（场景加载）
│
└── 典型场景
    ├── 延时执行 / 定时循环
    ├── 技能 CD / 装填
    ├── 淡入淡出 / 过场
    └── 异步加载 + 进度条
```

---

## 一、三者关系

```
MonoBehaviour（宿主）  →  StartCoroutine（启动）  →  IEnumerator 协程函数（yield 控制流程）
   必须挂游戏对象            返回 Coroutine 句柄           跨帧执行，遇 yield 暂停
```

| 对比 | 协程 | 普通 Update | Invoke |
|------|------|-------------|--------|
| 跨帧等待 | ✅ yield 自然写法 | ❌ 需自己计时 | ✅ 仅简单延时 |
| 传参 | ✅ 协程方法参数 | ✅ | ❌ 只能 string 方法名 |
| 循环逻辑 | ✅ while + yield | 每帧重复写 | InvokeRepeating |
| 停止控制 | StopCoroutine | enabled=false | CancelInvoke |
| 执行线程 | 主线程 | 主线程 | 主线程 |

> 协程不是独立类，而是 **MonoBehaviour 提供的机制** + **C# IEnumerator** 语法。

---

## 二、核心 API（MonoBehaviour 上）

| 方法 | 说明 |
|------|------|
| `StartCoroutine(IEnumerator routine)` | 启动协程，返回 `Coroutine` 引用（推荐） |
| `StartCoroutine(string methodName)` | 按方法名启动（无参；停止时也用方法名） |
| `StartCoroutine(string methodName, object value)` | 按方法名 + 1 个参数 |
| `StopCoroutine(Coroutine routine)` | 停止指定协程（需保存返回值） |
| `StopCoroutine(IEnumerator routine)` | 停止指定 IEnumerator |
| `StopCoroutine(string methodName)` | 停止同名方法启动的协程 |
| `StopAllCoroutines()` | 停止**该脚本**上所有协程 |

```csharp
public class ReloadSystem : MonoBehaviour
{
    Coroutine reloadCo;
    bool isReloading;

    public void StartReload()
    {
        if (isReloading) return;
        reloadCo = StartCoroutine(ReloadRoutine());
    }

    public void CancelReload()
    {
        if (reloadCo != null)
            StopCoroutine(reloadCo);
        isReloading = false;
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("装填中…");
        yield return new WaitForSeconds(2f);
        isReloading = false;
        Debug.Log("装填完成");
    }
}
```

---

## 三、协程函数写法（IEnumerator）

### 3.1 基本格式

```csharp
IEnumerator MyCoroutine()
{
    // 第 1 段：立即执行
    Debug.Log("开始");

    yield return null; // 暂停，下一帧从这里继续

    // 第 2 段
    Debug.Log("下一帧");

    yield return new WaitForSeconds(1f);

    // 第 3 段
    Debug.Log("1 秒后");
}
```

**规则：**
- 返回类型必须是 **`IEnumerator`**
- 体内必须有 **`yield return`**（否则不是合法协程）
- 用 **`StartCoroutine`** 启动，不能直接 `MyCoroutine()` 当普通方法指望它跨帧

### 3.2 带参数的协程

```csharp
IEnumerator SpawnWave(int count, float interval)
{
    for (int i = 0; i < count; i++)
    {
        SpawnEnemy();
        yield return new WaitForSeconds(interval);
    }
}

void Start()
{
    StartCoroutine(SpawnWave(5, 2f));
}
```

---

## 四、yield return 指令详解

| yield 语句 | 等待什么 | 受 timeScale 影响 | 典型用途 |
|------------|----------|-------------------|----------|
| `yield return null` | 下一帧 | 是（deltaTime=0 时仍下一帧） | 分帧处理、等一帧再执行 |
| `new WaitForSeconds(t)` | t 秒 | **是** | 技能 CD、装填、刷怪间隔 |
| `new WaitForSecondsRealtime(t)` | t 秒 | **否** | 暂停菜单倒计时 |
| `new WaitForFixedUpdate()` | 下一次 FixedUpdate | 是 | 物理相关分步 |
| `new WaitForEndOfFrame()` | 本帧渲染结束 | 是 | 截图、UI 布局后处理 |
| `new WaitUntil(() => cond)` | 条件为 true | — | 等玩家按键、等资源就绪 |
| `new WaitWhile(() => cond)` | 条件为 false | — | 等动画播完 flag |
| `yield return StartCoroutine(other)` | 另一个协程结束 | — | 协程序列（先 A 后 B） |
| `yield return asyncOperation` | AsyncOperation 完成 | — | **场景异步加载** |

```csharp
// 等玩家按任意键继续
yield return new WaitUntil(() => Input.anyKeyDown);

// 异步加载场景 + 进度
AsyncOperation op = SceneManager.LoadSceneAsync("Level2");
op.allowSceneActivation = false;
while (op.progress < 0.9f)
{
    loadingBar.value = op.progress;
    yield return null;
}
yield return new WaitForSeconds(0.5f);
op.allowSceneActivation = true;
```

---

## 五、协程 vs Invoke vs Update 计时

| 需求 | 推荐方案 |
|------|----------|
| 2 秒后开一枪 | `WaitForSeconds(2f)` 或 `Invoke("Fire", 2f)` |
| 每 2 秒刷一只怪（可中途 Stop） | **协程 while + yield** |
| 每帧跟新 UI 血条 | **Update** |
| 装填 2 秒，可被中断 | **协程 + StopCoroutine** |
| 复杂多步骤过场 | **协程链** yield return StartCoroutine |

**[教材精华]** 协程比 Invoke 更灵活（参数、循环、中断）；比 Update 计时更清晰（不用自己累加 timer）。

---

## 六、生命周期与停止规则（**重难点**）

| 情况 | 协程是否继续 |
|------|--------------|
| `gameObject.SetActive(false)` | **停止**，且不会自动恢复 |
| 脚本 `enabled = false` | **继续运行** |
| `Destroy(gameObject)` | 停止 |
| `StopCoroutine` / `StopAllCoroutines` | 停止 |
| 场景卸载 | 停止 |

```csharp
void OnDisable()
{
    // 对象被禁用时，未完成的协程已停；重新 OnEnable 需重新 Start
    StopAllCoroutines(); // 可选：主动清理，避免重复启动
}
```

**[易错点]**
- 禁用再启用 GameObject 后，以为协程还在 → **需重新 StartCoroutine**
- 同一脚本多次 `StartCoroutine(SpawnLoop)` 不 Stop → 多个循环叠加

---

## 七、不是多线程（易错点）

```
协程 = 主线程上的"分帧执行"，不是 Thread，不是 async/await 线程池
```

- `WaitForSeconds` 期间 CPU 不会阻塞在空循环里，而是把控制权交还 Unity，下一帧/到点再继续
- 协程里仍不能做耗时阻塞操作（如大文件同步读、死循环），否则会卡死游戏

---

## 八、嵌套与序列

```csharp
IEnumerator PlayCutscene()
{
    yield return StartCoroutine(FadeOut(1f));   // 先淡出
    yield return StartCoroutine(ShowDialog());   // 再对话
    yield return StartCoroutine(FadeIn(1f));    // 再淡入
}

IEnumerator FadeOut(float t)
{
    float elapsed = 0f;
    while (elapsed < t)
    {
        elapsed += Time.deltaTime;
        canvasGroup.alpha = 1f - elapsed / t;
        yield return null;
    }
}
```

---

## 九、注意要点

| 易错点 | 说明 |
|--------|------|
| 直接调用 `MyCoroutine()` | 不会跨帧，必须 StartCoroutine |
| `WaitForSeconds(0)` | 用 `yield return null` 代替 |
| 忘记保存 Coroutine 引用 | 无法精确 Stop 某一个 |
| SetActive(false) 后期望协程继续 | 应改 enabled 或换对象承载 |
| 在协程里不设退出条件 | `while(true)` 必须能 Stop 或 break |
| 与 timeScale | WaitForSeconds 受暂停影响；UI 倒计时用 Realtime |
| 停止后访问已 Destroy 的对象 | Stop 后注意 null 检查 |

---

## 十、典型应用

| 场景 | 实现 |
|------|------|
| 每 N 秒刷怪 | `while(true) { Spawn(); yield return WaitForSeconds(N); }` |
| 射击冷却 | yield WaitForSeconds(cd)；期间 isCoolingDown=true |
| 装填/读条 | 协程 + StopCoroutine 可打断 |
| 受伤红屏闪烁 | 循环改 Color + yield null |
| 场景异步加载 | yield return LoadSceneAsync + 进度条 |
| 延时销毁 | `yield return WaitForSeconds(3f); Destroy(obj);` |
| 连发射击 | 协程 for 循环 + WaitForSeconds(射速间隔) |
| 淡入淡出 | while + Time.deltaTime 改 alpha + yield null |

---

## 十一、与 MonoBehaviour 生命周期对照

```
Awake/Start 里 StartCoroutine
    ↓
协程运行中（跨多个 Update 帧）
    ↓
OnDisable（SetActive false）→ 协程停止
OnDestroy → 协程停止
```

详细生命周期见 [01_MonoBehaviour.md](01_MonoBehaviour.md) 第三节；本文专注协程专精用法。

---

## 十二、官方参考

- [Manual: Coroutines](https://docs.unity3d.com/Manual/Coroutines.html)
- [API: MonoBehaviour.StartCoroutine](https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html)
- [API: YieldInstruction](https://docs.unity3d.com/ScriptReference/YieldInstruction.html)
- [API: WaitForSeconds](https://docs.unity3d.com/ScriptReference/WaitForSeconds.html)
