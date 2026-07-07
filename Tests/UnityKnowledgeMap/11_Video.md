# Video 视频系统详解

## 思维导图总览

```
Video 视频系统
├── VideoClip 视频资源
│   ├── 导入设置：转码、分辨率、音频轨
│   └── width / height / length / frameRate
│
├── VideoPlayer 视频播放器
│   ├── clip / url 两种来源
│   ├── Play / Pause / Stop / Prepare
│   ├── renderMode 渲染目标
│   ├── isPlaying / isPrepared / time
│   ├── loop / playOnAwake / waitForFirstFrame
│   └── 事件：prepareCompleted / loopPointReached
│
└── 渲染目标（显示画面）
    ├── CameraFarPlane / CameraNearPlane（相机背景）
    ├── RenderTexture → RawImage（UI 全屏/面板）
    ├── MaterialOverride（3D 物体表面）
    └── APIOnly（仅取 texture 自行处理）
```

---

## 一、三者关系

```
VideoClip（数据）  →  VideoPlayer（播放器）  →  渲染目标（显示画面）
   MP4/MOV 等           挂在 GameObject 上         相机 / RenderTexture / 材质
```

| 对比音频系统 | 音频 | 视频 |
|--------------|------|------|
| 资源 | AudioClip | VideoClip |
| 播放组件 | AudioSource | VideoPlayer |
| 接收/输出 | AudioListener（耳朵） | Camera / RenderTexture / Material（眼睛看到的画面） |
| 命名空间 | UnityEngine | **UnityEngine.Video**（需 `using UnityEngine.Video;`） |

---

## 二、VideoPlayer 核心 API

| 方法/属性 | 说明 |
|-----------|------|
| `clip` | 当前播放的 VideoClip 资源 |
| `url` | 网络或本地路径（`source = VideoSource.Url` 时用） |
| `source` | `VideoSource.VideoClip` 或 `VideoSource.Url` |
| `Play()` | 开始播放 |
| `Pause()` | 暂停 |
| `Stop()` | 停止并回到开头 |
| `Prepare()` | 预加载（URL 模式常用，配合 `isPrepared`） |
| `isPlaying` | 是否正在播放 |
| `isPrepared` | 是否准备完成（URL 模式必等） |
| `time` | 当前播放时间（秒，可 seek） |
| `length` | 总时长（秒，只读） |
| `loop` | 是否循环 |
| `playbackSpeed` | 播放速度（1=正常） |
| `renderMode` | 渲染到何处（见第五节） |
| `targetCamera` | 渲染到相机时指定 Camera |
| `targetTexture` | 渲染到 RenderTexture |
| `texture` | 当前帧纹理（APIOnly 或自定义显示时用） |
| `audioOutputMode` | 音频输出方式（Direct / None 等） |
| `SetDirectAudioVolume(track, volume)` | 控制视频内嵌音频音量 |
| `playOnAwake` | 激活时自动播放 |
| `waitForFirstFrame` | 等第一帧加载完再播，避免黑屏闪一下 |

```csharp
using UnityEngine;
using UnityEngine.Video;

public class IntroVideo : MonoBehaviour
{
    public VideoClip introClip;
    VideoPlayer player;

    void Awake()
    {
        player = GetComponent<VideoPlayer>();
        player.clip = introClip;
        player.playOnAwake = false;
        player.loop = false;
        player.prepareCompleted += OnPrepared;
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.Play();
    }

    public void PlayIntro()
    {
        if (!player.isPrepared)
            player.Prepare();
        else
            player.Play();
    }
}
```

---

## 三、VideoClip

| 属性/信息 | 说明 |
|-----------|------|
| `width` / `height` | 视频分辨率（只读） |
| `length` | 时长（秒，double） |
| `frameRate` | 帧率 |
| `audioTrackCount` | 内嵌音轨数量 |
| `GetAudioChannelCount(i)` | 第 i 轨声道数 |
| `GetAudioSampleRate(i)` | 第 i 轨采样率 |

| 导入设置 | 说明 |
|----------|------|
| Transcode | 是否转码为目标平台格式 |
| Codec | 平台兼容编码（H.264 等） |
| Dimensions | 缩放分辨率，减小包体 |
| Audio | 是否保留音轨 |

> 官方支持格式因平台而异，详见 [Video file compatibility](https://docs.unity3d.com/Manual/VideoSources-FileCompatibility.html)。

---

## 四、两种视频来源

### 4.1 VideoClip 模式（项目内资源，推荐）

```csharp
player.source = VideoSource.VideoClip;
player.clip = myClip;
player.Play();
```

### 4.2 URL 模式（流媒体 / 本地文件路径）

```csharp
player.source = VideoSource.Url;
player.url = "https://example.com/video.mp4";
// 或本地：Application.streamingAssetsPath + "/intro.mp4"

player.prepareCompleted += vp => vp.Play();
player.Prepare(); // URL 必须先 Prepare，等 isPrepared
```

| 对比 | VideoClip | URL |
|------|-----------|-----|
| 适用 | 包内过场、CG | 网络流、StreamingAssets |
| 播放前 | 一般直接 Play | 必须 Prepare + 等 isPrepared |
| 属性可读 | 导入后即可知 width/length | Prepare 完成后才准确 |

**[易错点]** URL 未 Prepare 就 Play，或 `length`/`width` 为 0 — 先订阅 `prepareCompleted` 或轮询 `isPrepared`。

---

## 五、渲染模式（renderMode）

| VideoRenderMode | 说明 | 典型场景 |
|-----------------|------|----------|
| **CameraFarPlane** | 画在相机远裁剪面（背景） | 全屏过场 CG |
| **CameraNearPlane** | 画在相机近裁剪面（前景） | 全屏遮罩视频 |
| **RenderTexture** | 输出到 RenderTexture | **UI RawImage 播视频** |
| **MaterialOverride** | 替换 Renderer 上某材质 | 电视屏幕、广告牌 |
| **APIOnly** | 不自动渲染，用 `player.texture` 自取 | 自定义 Shader 显示 |

### UI 播视频（最常用套路）

```
VideoPlayer (renderMode = RenderTexture)
    → targetTexture = 某 RenderTexture
    → RawImage.texture = 同一张 RenderTexture
```

```csharp
using UnityEngine.UI;

public VideoPlayer player;
public RawImage videoScreen;
public RenderTexture rt;

void Start()
{
    player.renderMode = VideoRenderMode.RenderTexture;
    player.targetTexture = rt;
    videoScreen.texture = rt;
    player.clip = myClip;
    player.Play();
}
```

---

## 六、事件回调

| 事件 | 触发时机 |
|------|----------|
| `prepareCompleted` | Prepare 完成，可以安全 Play |
| `started` | 开始播放 |
| `loopPointReached` | 播放到结尾（loop=true 时会循环触发） |
| `errorReceived` | 加载/解码错误 |

```csharp
void OnEnable()
{
    player.loopPointReached += OnVideoEnd;
}

void OnVideoEnd(VideoPlayer vp)
{
    // 过场结束，进游戏
    gameObject.SetActive(false);
    LoadGameScene();
}

void OnDisable()
{
    player.loopPointReached -= OnVideoEnd;
}
```

---

## 七、视频内嵌音频

| 设置 | 说明 |
|------|------|
| `audioOutputMode = Direct` | 视频音轨直接输出（类似 AudioSource） |
| `audioOutputMode = None` | 静音，仅画面 |
| `SetDirectAudioVolume(0, 0.8f)` | 第 0 轨音量 |
| `controlledAudioTrackCount` | 控制的音轨数 |

> 若项目已有 BGM，过场视频注意与 AudioSource 混音；可临时降低 BGM 音量。

---

## 八、注意要点

| 易错点 | 说明 |
|--------|------|
| 忘记 `using UnityEngine.Video` | VideoPlayer / VideoClip 在该命名空间 |
| URL 未 Prepare | 黑屏、length=0、无法播放 |
| UI 视频黑屏 | 检查 RenderTexture 是否赋给 RawImage |
| 相机模式没设 targetCamera | renderMode 为 Camera* 时必须指定相机 |
| 平台 codec 不兼容 | 导入时 Transcode，真机测试 |
| 视频太大 | 压缩分辨率、缩短时长、StreamingAssets + URL |
| 事件未取消订阅 | OnDisable 里 `-=` 防止重复注册 |
| 与 AudioListener 混淆 | 视频**没有** Listener；画面靠 renderMode 输出 |

---

## 九、典型应用

| 场景 | 实现 |
|------|------|
| 开场 Logo 动画 | VideoPlayer + CameraFarPlane + loop=false |
| 关卡过场 CG | VideoClip + prepareCompleted → Play → loopPointReached 切场景 |
| 设置界面预览 | RenderTexture + RawImage |
| 游戏内电视 | MaterialOverride 到屏幕 MeshRenderer |
| 网络教学视频 | VideoSource.Url + Prepare + errorReceived |
| 跳过过场 | `player.Stop()` + 卸载或 SetActive(false) |

---

## 十、与 Audio 系统对照速查

| 需求 | 音频 | 视频 |
|------|------|------|
| 资源拖 Inspector | AudioClip | VideoClip |
| 播放组件 | AudioSource | VideoPlayer |
| 播放入口 | source.Play() | player.Play() |
| 叠加短音效 | PlayOneShot | 不适用（一般单路视频） |
| 输出目标 | AudioListener | Camera / RT / Material |
| 异步加载 | Load In Background | Prepare + isPrepared |
| 播完回调 | 协程等 clip.length | loopPointReached 事件 |
