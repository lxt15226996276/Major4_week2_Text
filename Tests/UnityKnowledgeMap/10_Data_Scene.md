# 数据持久化与场景管理详解

## 思维导图总览

```
Data & Scene
├── PlayerPrefs 本地键值存储
│   ├── SetInt / SetFloat / SetString
│   ├── GetInt / GetFloat / GetString
│   ├── HasKey / DeleteKey / DeleteAll
│   └── Save() 强制写入磁盘
│
├── SceneManager 场景管理
│   ├── LoadScene / LoadSceneAsync
│   ├── GetActiveScene / sceneCount
│   └── sceneLoaded 事件
│
├── Resources 资源加载（旧）
│   ├── Resources.Load<T>()
│   └── 需放在 Resources 文件夹
│
└── 预制体 Prefab
    ├── 编辑器制作 → 拖入场景或 Instantiate
    └── 变体 Prefab Variant
```

---

## 一、PlayerPrefs

| 方法 | 说明 |
|------|------|
| `SetInt(key, value)` | 存整数 |
| `GetInt(key, defaultValue)` | 读整数 |
| `SetFloat` / `GetFloat` | 浮点 |
| `SetString` / `GetString` | 字符串 |
| `HasKey(key)` | 是否存在 |
| `DeleteKey(key)` | 删除单项 |
| `DeleteAll()` | 清空全部 |
| `Save()` | 立即保存（部分平台自动，移动端建议调） |

```csharp
// 保存最高分
PlayerPrefs.SetInt("HighScore", score);
PlayerPrefs.Save();

// 读取（第二个参数为默认值）
int best = PlayerPrefs.GetInt("HighScore", 0);
```

> **适用**：设置、音量、关卡进度等少量数据。  
> **不适用**：大量存档、敏感数据（明文存储）。

---

## 二、SceneManager

```csharp
using UnityEngine.SceneManagement;

// 同步加载（会卡顿）
SceneManager.LoadScene("Level2");

// 按 buildIndex 加载
SceneManager.LoadScene(1);

// 异步加载（推荐）
StartCoroutine(LoadLevelAsync());

IEnumerator LoadLevelAsync()
{
  AsyncOperation op = SceneManager.LoadSceneAsync("Level2");
  op.allowSceneActivation = false; // 可卡在 0.9 显示加载条

  while (!op.isDone)
  {
    float progress = Mathf.Clamp01(op.progress / 0.9f);
    loadingBar.value = progress;
    if (op.progress >= 0.9f && Input.anyKeyDown)
      op.allowSceneActivation = true;
    yield return null;
  }
}
```

| API | 说明 |
|-----|------|
| `LoadScene(name/index)` | 同步切换 |
| `LoadSceneAsync` | 异步切换 |
| `GetActiveScene()` | 当前场景信息 |
| `sceneCountInBuildSettings` | Build Settings 中场景数 |

> 场景须加入 **File → Build Settings → Scenes In Build**。

---

## 三、Resources 加载

```csharp
GameObject prefab = Resources.Load<GameObject>("Prefabs/Enemy");
AudioClip clip = Resources.Load<AudioClip>("Audio/BGM");
```

| 优点 | 缺点 |
|------|------|
| 简单 | Resources 文件夹增大包体、启动扫描慢 |
| 无需 Addressables | 官方推荐 Addressables 管理资源 |

---

## 四、预制体 Prefab

| 概念 | 说明 |
|------|------|
| Prefab | 可复用的 GameObject 模板 |
| 实例化 | `Instantiate(prefab)` |
| 变体 | 在基础 Prefab 上改部分属性 |

```csharp
[SerializeField] GameObject bulletPrefab;

void Fire()
{
  Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
}
```

---

## 五、DontDestroyOnLoad 与跨场景

```csharp
void Awake()
{
  if (Instance == null)
  {
    Instance = this;
    DontDestroyOnLoad(gameObject); // 切场景不销毁
  }
  else
    Destroy(gameObject);
}
```

---

## 六、注意要点

| 易错点 | 说明 |
|--------|------|
| 场景名拼写错误 | 与 Build Settings 中一致 |
| 异步加载未 yield | 用协程或 op.completed |
| PlayerPrefs 键名乱 | 用常量类统一管理 key |
| Resources 路径错 | 不含扩展名，相对 Resources 文件夹 |

---

## 七、典型应用

| 场景 | 方案 |
|------|------|
| 关卡切换 | LoadSceneAsync + 加载 UI |
| 音量设置 | PlayerPrefs |
| 子弹/敌人 | Prefab + Instantiate |
| 全局音效管理器 | DontDestroyOnLoad 单例 |
