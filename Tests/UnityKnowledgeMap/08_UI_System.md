# UI 系统详解

## 思维导图总览

```
UI System (UnityEngine.UI)
├── Canvas 画布
│   ├── Render Mode：Screen Space / World Space
│   ├── Canvas Scaler 分辨率适配
│   └── GraphicRaycaster 射线检测
│
├── RectTransform 布局变换
│   ├── anchor / pivot / anchoredPosition
│   ├── sizeDelta / offsetMin/Max
│   └── 替代普通 Transform 用于 UI
│
├── EventSystem 事件系统
│   ├── StandaloneInputModule
│   └── 处理点击、拖拽、滚轮
│
└── 常用 UI 组件
    ├── Image / RawImage
    ├── Text / TextMeshPro
    ├── Button
    ├── Slider / Toggle
    └── ScrollRect
```

---

## 一、Canvas 画布

| Render Mode | 说明 |
|-------------|------|
| Screen Space - Overlay | 直接画在屏幕最上层 |
| Screen Space - Camera | 通过指定相机渲染 |
| World Space | 3D 世界中的 UI 面板 |

| 组件 | 作用 |
|------|------|
| Canvas | UI 根容器 |
| Canvas Scaler | 按屏幕分辨率缩放 UI |
| Graphic Raycaster | 使 UI 可点击 |

```csharp
// 动态改 UI 文字
using UnityEngine.UI;
textComponent.text = "得分：" + score;
```

---

## 二、RectTransform

| 概念 | 说明 |
|------|------|
| Anchor | 相对父级锚点（0~1） |
| Pivot | 自身轴心点 |
| anchoredPosition | 相对锚点的位置 |
| sizeDelta | 宽高增量 |
| offsetMin / offsetMax | 四边拉伸布局 |

> UI 元素**没有**普通 Transform 的 position 语义，改位置用 `anchoredPosition`。

---

## 三、EventSystem

- 场景中需有 **EventSystem** 对象（创建 UI 时自动添加）
- 负责把输入转为 UI 事件（点击、拖拽、滚轮）
- 与 3D 物体的 OnMouseDown 不同，UI 用 **IPointerClickHandler** 或 Button.onClick

```csharp
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
  public Button startBtn;

  void Start()
  {
    startBtn.onClick.AddListener(OnStartGame);
  }

  void OnStartGame()
  {
    SceneManager.LoadScene("Game");
  }
}
```

---

## 四、常用组件

| 组件 | 用途 |
|------|------|
| Image | 显示 Sprite 图片、填充血条 |
| Text (Legacy) | 简单文字（新项目推荐 TMP） |
| Button | 可点击按钮 + onClick |
| Slider | 血量条、音量条 |
| Toggle | 开关选项 |
| ScrollRect | 滚动列表 |
| InputField | 文本输入 |

---

## 五、注意要点

| 易错点 | 说明 |
|--------|------|
| UI 点不了 | 缺 EventSystem 或 GraphicRaycaster |
| UI 模糊 | Canvas Scaler 参考分辨率设置 |
| 3D 物体挡 UI | 检查 Raycast Target、相机深度 |
| 世界 UI 太小 | 调 RectTransform 缩放或 Canvas 尺寸 |

---

## 六、典型应用

| 场景 | 组件 |
|------|------|
| 主菜单 | Canvas + Button |
| 血条 | Slider / Image fill |
| 准星 | Screen Space Overlay Image |
| 伤害数字 | World Space Canvas + Text |
