# Audio 音频系统详解

## 思维导图总览

```
Audio 音频系统
├── AudioClip 音频资源
│   ├── 导入设置：3D/2D、压缩格式
│   └── LoadType 加载方式
│
├── AudioSource 音频源（发声）
│   ├── clip / Play / Stop / Pause
│   ├── volume / pitch / loop
│   ├── spatialBlend 2D↔3D
│   ├── PlayOneShot 叠加播放
│   └── PlayOnAwake / mute
│
└── AudioListener 监听器（耳朵）
    ├── 通常挂在 Main Camera 上
    └── 场景中只能有一个生效
```

---

## 一、三者关系

```
AudioClip（数据）  →  AudioSource（播放器）  →  AudioListener（接收者/玩家耳朵）
     WAV/MP3              挂在发声物体上              挂在主相机上
```

---

## 二、AudioSource 核心 API

| 方法/属性 | 说明 |
|-----------|------|
| `clip` | 当前音频片段 |
| `Play()` | 播放 clip |
| `PlayOneShot(clip)` | 叠加播放，不打断当前音乐 |
| `Stop()` | 停止 |
| `Pause()` / `UnPause()` | 暂停/继续 |
| `volume` | 音量 0~1 |
| `pitch` | 音调（1=正常） |
| `loop` | 是否循环 |
| `spatialBlend` | 0=2D，1=3D |
| `minDistance` / `maxDistance` | 3D 衰减距离 |
| `isPlaying` | 是否正在播放 |

```csharp
public class GunAudio : MonoBehaviour
{
  public AudioClip fireClip;
  public AudioClip reloadClip;
  AudioSource source;

  void Awake() => source = GetComponent<AudioSource>();

  public void PlayFire()
  {
    source.PlayOneShot(fireClip); // 连射可重叠
  }
}
```

---

## 三、AudioClip

| 导入设置 | 说明 |
|----------|------|
| Force To Mono | 3D 音效常勾 mono |
| Load In Background | 异步加载 |
| Compression Format | PCM / Vorbis 等 |
| Load Type | Decompress On Load / Compressed In Memory / Streaming |

---

## 四、AudioListener

- 挂在 **Main Camera** 或玩家上
- **全场景只能有一个** AudioListener 组件生效
- 多个时会报警告，3D 音效定位异常

---

## 五、2D vs 3D 音效

| 类型 | spatialBlend | 场景 |
|------|--------------|------|
| UI/背景音乐 | 0 | BGM、按钮点击 |
| 世界音效 | 1 | 枪声、脚步、爆炸 |

---

## 六、注意要点

| 易错点 | 说明 |
|--------|------|
| 多个 AudioListener | 只保留主相机上一个 |
| Play 打断 BGM | 短音效用 PlayOneShot |
| 未赋值 clip 就 Play | 先检查 null |
| 大音频 Decompress On Load | 用 Streaming 减内存 |

---

## 七、典型应用

| 场景 | 实现 |
|------|------|
| 射击音效 | PlayOneShot |
| 背景音乐 | 单独 AudioSource + loop |
| 3D 爆炸 | spatialBlend=1 + 3D 设置 |
| 音量设置 | AudioListener.volume 或混音器 |
