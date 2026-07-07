# UnityEngine 核心类知识点思维导图

## 推荐入口（综合版）

**[00_Unity常用类.md](00_Unity常用类.md)** — 综合六本教材 + 官方文档精华，标注 **使用频率 ⭐ / 重难点 / 易错点 / 糟粕剔除**，适合复习与周考。

## 总览索引

```
UnityEngine 核心类体系
│
├── 01_MonoBehaviour ─────── Unity脚本生命周期基石
│   ├── 生命周期函数
│   ├── 协程系统
│   └── 消息机制
│
├── 02_GameObject ─────────── 场景实体容器
│   ├── 创建与销毁
│   ├── 组件管理
│   └── 标签与层级
│
├── 03_Transform ──────────── 空间变换核心
│   ├── 位置/旋转/缩放
│   ├── 父子关系
│   └── 坐标空间转换
│
├── 04_Input & Time ────────── 输入与时间系统
│   ├── 键盘/鼠标/触摸输入
│   ├── 轴输入系统
│   └── 时间控制与帧率
│
├── 05_Physics ─────────────── 物理引擎系统
│   ├── Rigidbody 刚体
│   ├── Collider 碰撞器
│   ├── 射线检测
│   └── 物理材质
│
├── 06_Camera & Rendering ──── 摄像机与渲染
│   ├── Camera 摄像机
│   ├── Material 材质
│   └── Light 光照
│
├── 07_Audio ───────────────── 音频系统
│   ├── AudioSource 音频源
│   ├── AudioListener 监听器
│   └── AudioClip 音频片段
│
├── 11_Video ───────────────── 视频系统
│   ├── VideoPlayer 播放器
│   ├── VideoClip 视频资源
│   └── RenderTexture / 渲染目标
│
├── 12_Particle ────────────── 粒子特效系统
│   ├── ParticleSystem 模拟
│   ├── ParticleSystemRenderer 渲染
│   └── Prefab + Play/Stop 工作流
│
├── 13_Coroutine ───────────── 协程系统
│   ├── StartCoroutine / StopCoroutine
│   ├── IEnumerator + yield return
│   └── WaitForSeconds / 场景异步加载
│
├── 08_UI System ───────────── UI系统
│   ├── Canvas 画布
│   ├── RectTransform
│   ├── EventSystem
│   └── 常用UI组件
│
├── 09_Math & Utilities ────── 数学与工具类
│   ├── Vector2/Vector3
│   ├── Quaternion 四元数
│   ├── Mathf 数学函数
│   └── Random 随机数
│
└── 10_Data & Scene ────────── 数据持久化与场景管理
    ├── PlayerPrefs
    ├── SceneManager
    └── Resources/Addressables
│
└── Animation/ ─────────────── 动画专题详解（对标 00_Unity常用类 格式）
    ├── 01 Animation Legacy / Clip / K帧
    ├── 02 Animator / Avatar / Controller
    ├── 03 状态机 / Layer / Mask
    ├── 04 BlendTree / IK / MatchTarget
    └── 05 射击/RPG/2D 实战
```

## 类继承关系总览

```
System.Object
├── UnityEngine.Object
│   ├── GameObject
│   ├── Component
│   │   ├── Behaviour
│   │   │   ├── MonoBehaviour (用户脚本基类)
│   │   │   ├── Camera
│   │   │   ├── AudioSource
│   │   │   ├── AudioListener
│   │   │   └── Collider
│   │   │       ├── BoxCollider
│   │   │       ├── SphereCollider
│   │   │       ├── CapsuleCollider
│   │   │       └── MeshCollider
│   │   ├── Transform
│   │   ├── Rigidbody
│   │   ├── ParticleSystem
│   │   └── Renderer
│   │       ├── MeshRenderer
│   │       ├── ParticleSystemRenderer
│   │       └── SpriteRenderer
│   ├── ScriptableObject
│   ├── Material
│   ├── Texture
│   │   └── Texture2D
│   ├── Sprite
│   ├── AudioClip
│   └── Shader
│
├── Vector2 (struct)
├── Vector3 (struct)
├── Vector4 (struct)
├── Quaternion (struct)
├── Color (struct)
└── Ray (struct)
```

## 文档列表（已全部生成）

| 文件 | 内容 | 核心类 | 状态 |
|------|------|--------|------|
| [00_Unity常用类.md](00_Unity常用类.md) | **综合思维导图（教材+官方）** | 全部常用类 | ✅ |
| [01_MonoBehaviour.md](01_MonoBehaviour.md) | 脚本生命周期 | MonoBehaviour | ✅ |
| [02_GameObject.md](02_GameObject.md) | 游戏对象 | GameObject, Object | ✅ |
| [03_Transform.md](03_Transform.md) | 空间变换 | Transform | ✅ |
| [04_Input_Time.md](04_Input_Time.md) | 输入与时间 | Input, Time | ✅ |
| [05_Physics.md](05_Physics.md) | 物理系统 | Rigidbody, Collider, Physics | ✅ |
| [06_Camera_Rendering.md](06_Camera_Rendering.md) | 摄像机与渲染 | Camera, Material, Light | ✅ |
| [07_Audio.md](07_Audio.md) | 音频系统 | AudioSource, AudioClip | ✅ |
| [11_Video.md](11_Video.md) | 视频系统 | VideoPlayer, VideoClip | ✅ |
| [12_ParticleSystem.md](12_ParticleSystem.md) | 粒子特效 | ParticleSystem, ParticleSystemRenderer | ✅ |
| [13_Coroutine.md](13_Coroutine.md) | 协程系统 | StartCoroutine, IEnumerator, yield | ✅ |
| [08_UI_System.md](08_UI_System.md) | UI系统 | Canvas, RectTransform, EventSystem | ✅ |
| [09_Math_Utilities.md](09_Math_Utilities.md) | 数学与工具 | Vector3, Quaternion, Mathf, Random, Debug | ✅ |
| [10_Data_Scene.md](10_Data_Scene.md) | 数据与场景 | PlayerPrefs, SceneManager | ✅ |

### 动画专题（[Animation/](Animation/) · 格式对标 [00_Unity常用类.md](00_Unity常用类.md)）

| 文件 | 内容 | 核心类 | 状态 |
|------|------|--------|------|
| [01_Animation动画深入.md](Animation/01_Animation动画深入.md) | Animation Legacy | Animation, AnimationClip | ✅ |
| [02_Animator动画.md](Animation/02_Animator动画.md) | Mecanim 系统 | Animator, Avatar, Controller | ✅ |
| [03_动画状态机一.md](Animation/03_动画状态机一.md) | 状态切换与分层 | State, Transition, Layer | ✅ |
| [04_动画状态机二.md](Animation/04_动画状态机二.md) | 混合树与 IK | BlendTree, IK, MatchTarget | ✅ |
| [05_常见游戏动画设置.md](Animation/05_常见游戏动画设置.md) | 三类游戏实战 | 射击/RPG/2D | ✅ |

## 交互版思维导图

浏览器打开：[UnityEngine_MindMap.html](../Exams/Paper01/UnityEngine_MindMap.html)（支持搜索、分类筛选、展开/折叠）

## 学习建议

1. **入门阶段**：先掌握 MonoBehaviour → GameObject → Transform → Input/Time
2. **进阶阶段**：深入 Physics → Camera → Audio → UI
3. **高级阶段**：精通 Math工具 → 数据管理 → 性能优化
4. **动画专题（单元06～10）**：[Animation/](Animation/) 目录 01 → 05 顺序阅读
