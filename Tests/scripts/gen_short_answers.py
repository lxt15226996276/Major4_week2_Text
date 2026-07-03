#!/usr/bin/env python3
"""从 exam-all.json 生成精简版答案文档。"""

import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
JSON_PATH = ROOT / "QuizSite" / "data" / "banks" / "exam-all.json"
OUT_MD = ROOT / "Tests" / "元宇宙专业四题库_精简答案.md"

# 手工精编：语义不变、尽量短
OVERRIDES: dict[str, str] = {
    "游戏中玩家与系统交互的视觉界面，如菜单、血条、按钮等。": "玩家与系统交互的界面，如菜单、血条、按钮。",
    "游戏 UI 需贴合游戏风格，动态效果更强，适配多输入设备，性能要求更高。": "贴合游戏风格，动效强，适配多输入，性能要求高。",
    "是所有 UI 元素的容器，所有 UI 组件必须依附于 Canvas 存在。": "UI 容器，所有 UI 须挂在 Canvas 下。",
    "屏幕覆盖模式、屏幕相机模式、世界空间模式；": "Screen Space-Overlay / Camera、World Space。",
    "专为 UI 设计，支持锚点和轴心，可随父级自动调整大小和位置。": "UI 专用，支持锚点/轴心，随父级自适应大小位置。",
    "普通 Transform 用于 3D/2D 对象；Rect Transform 用于 UI 元素。": "Transform→3D/2D 对象；RectTransform→UI。",
    "Canvas Scaler 组件是实现 UI 屏幕自适应的核心。": "Canvas Scaler 是屏幕自适应核心。",
    "决定 UI 元素与父级边缘的连接方式，影响元素随父级变化的调整方式。": "决定 UI 与父级边缘的连接及缩放方式。",
    "是元素旋转、缩放的中心点，也影响位置计算。": "旋转/缩放中心，影响位置计算。",
    "包括字体、字号、颜色、对齐方式、文本内容等。": "字体、字号、颜色、对齐、文本等。",
    "负责检测和处理用户输入，如点击、滑动等交互操作。": "检测处理点击、滑动等输入。",
    "按设定的参考分辨率缩放 UI，保持元素比例。": "按参考分辨率缩放，保持比例。",
    "确保 UI 元素在屏幕尺寸变化时能正确显示在预期位置。": "屏幕变化时 UI 仍显示在正确位置。",
    "用于显示 Sprite 图片，支持切片、九宫格等效果。": "显示 Sprite，支持九宫格等。",
    "RawImage 显示原始纹理，Image 显示 Sprite；RawImage 更适合动态纹理。": "RawImage→纹理；Image→Sprite；动态纹理用 RawImage。",
    "正常、高亮、按下、禁用四种状态。": "Normal / Highlighted / Pressed / Disabled。",
    "可在 Inspector 面板绑定脚本方法，或通过代码 AddListener 添加。": "Inspector 绑定或代码 AddListener。",
    "实现开关功能或多选功能，可通过 isOn 获取选中状态。": "开关/多选，isOn 读选中状态。",
    "管理多个 Toggle，实现单选效果（同一时间仅一个被选中）。": "管理 Toggle 实现单选。",
    "显示相机渲染纹理、动态生成的纹理等。": "显示 RenderTexture 或动态纹理。",
    "包括 None、Color Tint、Sprite Swap、Animation。": "None / Color Tint / Sprite Swap / Animation。",
    "使用带透明通道的圆形 Sprite，或添加 Mask 组件配合圆形遮罩。": "圆形 Sprite 或 Mask 遮罩。",
    "无 EventSystem、未勾选 Raycast Target、被遮挡、脚本错误等。": "缺 EventSystem、未勾 Raycast Target、被遮挡或脚本错。",
    "包括背景、勾选图标、Label 等元素。": "背景、勾选图标、Label 等。",
    "如 Simple、Sliced、Tiled、Filled 等。": "Simple / Sliced / Tiled / Filled。",
    "用于指定要显示的原始纹理。": "指定显示的纹理。",
    "在 Button 的 Color Tint 过渡模式下设置 Highlighted Color。": "Color Tint 模式设 Highlighted Color。",
    "会触发 onValue Changed 事件，可监听该事件做相应处理。": "触发 onValueChanged，监听处理。",
    "包括 Min Value、Max Value、Value、Whole Numbers 等。": "Min/Max Value、Value、Whole Numbers 等。",
    "Scrollbar 可作为 Scroll Rect 的滚动控制器，两者状态同步。": "Scrollbar 作 ScrollRect 控制器，状态同步。",
    "Mask 支持任意形状，性能较差；RectMask2d 仅支持矩形，性能更好。": "Mask 任意形状但慢；RectMask2D 仅矩形但快。",
    "包括 Viewport（可视区域）、Content（滚动内容）、滚动条。": "Viewport、Content、Scrollbar。",
    "通过模板测试，只显示遮罩范围内的内容，隐藏超出部分。": "模板测试，只显示遮罩内内容。",
    "设置滚动条的步长，使 Value 值按固定间隔变化。": "设步长，Value 按间隔变化。",
    "使用 RectMask2d 替代 Mask，开启对象池，限制子物体数量。": "用 RectMask2D、对象池，限制子项数量。",
    "当 Slider 的 Value 值发生变化时（拖动或代码修改）。": "Value 变化时（拖动或代码改）。",
    "包括 Unrestricted、Elastic、Clamped。": "Unrestricted / Elastic / Clamped。",
    "通常需要配合 Image 等具有图形的组件使用。": "需配合 Image 等图形组件。",
    "控制滚动条滑块的大小，反映内容占可视区域的比例。": "控制滑块大小，反映内容/可视区比例。",
    "控制滑块的滑动方向。": "控制滑动方向。",
    "性能优于 Mask，适合移动端，只支持矩形遮罩。": "比 Mask 快，适合移动端，仅矩形。",
    "包括 Label（显示选中项）、Template（下拉列表模板）、Item（列表项）。": "Label、Template、Item。",
    "有 Standard、Integer、Decimal、Password、Email Address 等。": "Standard / Integer / Decimal / Password / Email 等。",
    "决定 UI 元素是否能接收射线检测，影响交互响应。": "决定是否接收射线，影响交互。",
    "通过代码调用 AddOptions () 方法，传入选项列表。": "代码 AddOptions() 传列表。",
    "设置 “Character Limit” 属性，0 表示无限制。": "设 Character Limit，0=不限。",
    "减少射线检测计算量，提高 UI 交互性能，避免误触发。": "减射线计算，提性能，防误触。",
    "表示选中项在选项列表中的索引（从 0 开始）。": "选中项索引（从 0 起）。",
    "Caption Text 显示当前选中项；Item Text 是列表项的文本模板。": "Caption 显示选中项；Item 为列表项模板。",
    "当选中项变化时触发，可获取新选中项的索引。": "选中变化时触发，返回新索引。",
    "设为 true 时，输入框不可编辑，仅可查看。": "true 时只读。",
    "可通过 “Content Type” 限制，或监听事件自定义验证。": "Content Type 限制或事件自定义验证。",
    "作为下拉列表的模板，定义列表项的样式。": "下拉列表模板，定义项样式。",
    "确保输入内容符合预期格式，减少错误输入。": "限制格式，减少错输。",
    "创建 Canvas，添加背景、标题，设计服务器列表（用 Scroll Rect），添加按钮。": "建 Canvas→背景/标题→ScrollRect 列表→按钮。",
    "调用 SetActive () 方法，或修改 Canvas Group 的 alpha 和 interactable 属性。": "SetActive 或改 CanvasGroup alpha/interactable。",
    "利用父子对象层级管理，通过单例模式获取实例，用事件通知。": "父子层级 + 单例 + 事件通知。",
    "服务器列表用 Scroll Rect 适配数量，按钮和文本用锚点固定位置。": "列表用 ScrollRect，按钮/文本用锚点固定。",
    "给子面板添加 Panel 组件并勾选 “Block Raycasts”，或提高子面板 Sort Order。": "Panel 勾 Block Raycasts 或提高 Sort Order。",
    "请求数据时显示旋转图标，加载完成后隐藏。": "请求时显示 loading，完成隐藏。",
    "调整 Canvas Group 的 alpha 值，配合动画插件实现过渡。": "调 CanvasGroup alpha 做淡入淡出。",
    "绑定事件，点击时隐藏当前面板，显示上一级面板。": "绑事件，隐藏当前、显示上级。",
    "合理设置 Canvas Scaler 和各元素的锚点、布局组件。": "设好 Canvas Scaler、锚点、布局。",
    "通过代码动态实例化服务器项预制体，添加到 Content 中。": "实例化预制体加到 Content。",
    "存储对象动画数据的文件，包含时间轴上的位置、旋转、缩放等属性变化信息。": "存动画数据的资源，含位置/旋转/缩放等关键帧。",
    "包括 Once（单次播放）、Loop（循环）、PingPong（来回播放）、Clamp Forever（停在最后一帧）。": "Once / Loop / PingPong / Clamp Forever。",
    "选中对象后打开 Animation 窗口，点击 “Create” 按钮并命名保存，通过添加关键帧编辑动画。": "Animation 窗口 Create→命名→加关键帧。",
    "用于编辑属性随时间变化的曲线，可调整切线类型（如线性、平滑）优化动画过渡。": "编辑属性曲线，调切线优化过渡。",
    "用于创建、编辑动画剪辑，添加 / 删除关键帧，预览动画效果，调整动画曲线。": "创建/编辑 Clip、关键帧、预览、调曲线。",
    "Dopesheet 视图显示关键帧的时间分布，Curves 视图显示属性随时间变化的曲线形态。": "Dopesheet 看关键帧时间；Curves 看属性曲线。",
    "选中关键帧后按住 Ctrl 键拖动到目标时间点，或通过右键菜单 “Copy” 和 “Paste” 操作。": "Ctrl 拖动或 Copy/Paste 关键帧。",
    "减少动画剪辑中的关键帧数量，合并重复动画，使用对象池管理动画对象。": "减关键帧、合并重复动画、对象池。",
    "Animator 支持复杂状态机和动画混合，Animation 仅支持简单的动画剪辑播放。": "Animator→状态机/混合；Animation→简单播放。",
    "选中对象后在 Inspector 面板点击 “Add Component”，搜索并添加 “Animator” 组件。": "Inspector Add Component→Animator。",
    "指定关联的 Animator Controller，用于定义动画状态机逻辑。": "指定 Animator Controller。",
    "包括 Normal（正常更新）、Animate Physics（物理更新）、Unscaled Time（不受时间缩放影响）。": "Normal / Animate Physics / Unscaled Time。",
    "在 Animator Controller 中设置某个状态为 “Default State”，进入时自动播放。": "设 Default State，进入自动播。",
    "控制对象不可见时是否停止动画更新，优化性能。": "不可见时是否停更动画，优化性能。",
    "在 Animator 窗口中从源状态拖动到目标状态，创建过渡线并设置条件。": "源状态拖到目标状态，设过渡条件。",
    "设置源状态播放到指定比例（0-1）时允许过渡，确保动画播放完整。": "源动画播到指定比例才过渡。",
    "取消勾选 “Has Exit Time”，并设置过渡的 “Duration” 为 0。": "取消 Has Exit Time，Duration=0。",
    "设置过渡的中断源，如 “None”（不可中断）、“Current State”（当前状态可被中断）。": "设 Interruption Source 控制能否被中断。",
    "控制两个动画状态过渡的时间长度，影响混合平滑度。": "控制过渡时长，影响混合平滑度。",
    "使用 Animator.SetBool ()、SetTrigger ()、SetFloat () 等方法设置对应参数。": "SetBool/SetTrigger/SetFloat 等设参数。",
    "将多个相关状态归类管理，简化复杂状态机的结构，提高可读性。": "归类相关状态，简化状态机结构。",
    "用于混合多个动画（如基础动作 + 表情动画），通过权重控制各层影响程度。": "多层动画混合，权重控制影响。",
    "在 Animator 窗口的 Layers 面板中拖动权重滑块，范围为 0（无影响）到 1（完全影响）。": "Layers 面板拖权重 0~1。",
    "使子层与基础层同步状态，确保动画时间线一致。": "子层与基础层同步时间线。",
    "允许该层使用反向动力学（IK），用于骨骼目标定位（如角色看向目标）。": "启用 IK，如角色看向目标。",
    "模型需带有 Avatar，且骨骼结构相似，动画剪辑为人类动画。": "需 Avatar、骨骼相似、人形动画。",
    "为模型添加相同的 Avatar，将动画剪辑拖入 Animator Controller 即可复用。": "同 Avatar + 拖 Clip 到 Controller 复用。",
    "用于平滑混合多个相似动画（如不同速度的行走、跑步），基于参数值自动过渡。": "按参数平滑混合相似动画（如走/跑）。",
    "在 Animator 窗口右键选择 “Create State> From New Blend Tree”，并添加子动画。": "右键 Create State→Blend Tree，加子动画。",
    "用于动态调整骨骼末端位置（如手、脚），使角色与场景交互（如手触墙壁）。": "动态调手/脚等末端，实现场景交互。",
    "在 OnAnimatorIK () 回调中设置 Animator.SetLookAtPosition ()，并启用 IK Pass。": "OnAnimatorIK 中 SetLookAtPosition + IK Pass。",
    "使角色某个身体部位（如手）在动画播放时精确匹配目标位置（如拾取物品）。": "动画中身体部位精确对齐目标（如拾物）。",
    "目标位置、身体部位（如 HandLeft）、权重、开始和结束时间。": "目标位置、身体部位、权重、起止时间。",
    "在动画播放过程中调用 Animator.MatchTarget () 方法，指定目标参数。": "播放中调用 MatchTarget()。",
    "使用粒子系统制作火焰效果，设置短生命周期，通过对象池复用。": "粒子+短生命周期+对象池。",
    "通过技能按钮事件设置 Animator 触发器，播放对应技能动画，同时关联特效播放。": "按钮设 Trigger 播技能动画+特效。",
    "骨骼动画通过骨骼驱动贴图，文件小且灵活；帧动画是序列帧切换，适合简单效果。": "骨骼→驱动贴图，小且灵活；帧动画→序列帧。",
    "Unity 的 2D Animation 包、Spine、DragonBones 等。": "2D Animation / Spine / DragonBones。",
    "通过 ScaleX 翻转角色贴图，或制作左右两个方向的动画剪辑。": "ScaleX 翻转或左右两个 Clip。",
    "服务器权威模式：服务器计算动画状态，客户端同步播放；或客户端预测 + 服务器修正。": "服务器权威同步，或预测+修正。",
    "播放死亡动画时禁用移动和攻击，动画结束后触发角色消失或重生逻辑。": "播死亡动画时禁移动/攻击，结束后再消失或重生。",
    "使用粒子系统配合动画曲线控制粒子密度，通过参数切换天气状态。": "粒子+曲线控密度，参数切天气。",
    "使用 Animator IK 调整脚部位置，或通过射线检测动态修改角色 Y 轴位置。": "IK 调脚或射线改 Y 轴贴地。",
    "通过 Spine 等工具的皮肤系统，或在 Unity 中动态更换 SpriteRenderer 的 Sprite。": "Spine 换肤或换 SpriteRenderer Sprite。",
    "保持 UI 元素像素大小不变，屏幕越大显示内容越多。": "像素大小不变，大屏显示更多。",
    "监听 Slider 的 onValueChanged 事件，将 Value 值映射到音频源 volume 属性。": "监听 onValueChanged，映射到 AudioSource.volume。",
    "当输入框失去焦点或按下回车键时。": "失焦或按回车时。",
    "用 PlayerPrefs 存储服务器 ID，或上传至后端数据库。": "PlayerPrefs 存 ID 或上传后端。",
    "在动画剪辑中为摄像机的 “Field of View” 或 “Depth of Field” 属性添加关键帧。": "给 FOV 或景深属性加关键帧。",
    "使用简化的爆炸模型，减少粒子数量，设置动画播放完成后自动销毁对象。": "简化模型、减粒子、播完销毁。",
    "用于人形模型，关联骨骼映射信息，支持动画重定向。": "人形骨骼映射，支持动画重定向。",
    "通过 Animator 参数（如布尔值、触发器、浮点数）控制过渡条件。": "Bool/Trigger/Float 等参数控过渡。",
    "通过 Move () 返回的 CollisionFlags 判断。": "Move() 返回的 CollisionFlags。",
    "处理角色移动与碰撞，无需依赖刚体。": "角色移动碰撞，不依赖 Rigidbody。",
    "调用 Move () 方法，传入基于输入的位移向量（乘 Time.deltaTime）。": "Move(输入位移 * deltaTime)。",
    "检测跳跃输入，通过 isGrounded 判断在地面时施加向上位移。": "检测跳跃 + isGrounded，地面时向上位移。",
    "修改 Height 和 Center 属性，播放下蹲动画。": "改 Height/Center + 播下蹲动画。",
    "不在地面时，移动向量加向下重力加速度。": "空中时位移加重力。",
    "将输入向量转换为摄像机参考的世界方向。": "输入向量转摄像机世界方向。",
    "控制角色可跨越的最大台阶高度。": "可跨越的最大台阶高度。",
    "前者控制碰撞体半径，后者控制碰撞体高度。": "Radius→半径；Height→高度。",
    "检测到跳跃输入且角色在地面时，设置 Animator 触发器。": "跳跃输入且 isGrounded 时设 Trigger。",
    "获取移动速度，用 Animator.SetFloat () 传给状态机控制动画切换。": "速度 SetFloat 传给状态机。",
    "结合设备振动 API，输入达阈值时触发。": "输入达阈值调振动 API。",
    "左摇杆控移动，右摇杆控摄像机旋转。": "左摇杆移动，右摇杆转镜头。",
    "让角色 forward 方向跟随摇杆水平方向或移动方向。": "forward 跟随摇杆/移动方向。",
    "用 Mathf.Lerp () 或 SmoothDamp () 减少抖动。": "Lerp 或 SmoothDamp 平滑。",
    "添加 Joystick 控件，设置位置、外观和轴映射，关联移动脚本。": "加 Joystick，设外观/轴映射，绑移动脚本。",
    "用 Joystick.GetAxisValue () 获取 0-1 范围值。": "GetAxisValue() 取 0~1。",
    "X 轴（水平）和 Y 轴（垂直）。": "X 水平、Y 垂直。",
    "下载插件，通过 “Assets> Import Package” 导入项目。": "Assets→Import Package 导入。",
    "用并行状态机，允许多状态同时运行。": "并行状态机，多状态同跑。",
    "为源状态添加过渡条件，满足时执行退出和进入逻辑。": "设过渡条件，满足则切换。",
    "单元测试单状态逻辑，场景模拟条件检测过渡。": "单测状态逻辑，场景测过渡。",
    "超状态包含多个子状态，子状态是其下具体状态。": "超状态含多个子状态。",
    "在 OnUpdate 中加计时器，达标且条件满足时切换。": "OnUpdate 计时，到时且条件满足再切。",
    "可从任何状态过渡到目标状态的规则。": "任意状态可过渡到目标状态。",
    "通过定义状态及切换规则，管理对象在不同条件下的行为。": "定义状态与切换规则，管理行为。",
    "代码清晰、逻辑模块化，便于维护扩展，减少冗余。": "逻辑清晰模块化，易维护扩展。",
    "脚本获取行为状态，设置 Animator 参数控制动画切换。": "读行为状态，设 Animator 参数切动画。",
    "用 Random.Range () 生成随机时间。": "Random.Range 随机时间。",
    "巡逻路径、追击触发、攻击范围冷却、死亡状态等。": "巡逻、追击、攻击冷却、死亡等。",
    "距离小于攻击范围且不在冷却时，播放动画并判定伤害。": "进攻击范围且冷却完→播动画+伤害。",
    "添加球形碰撞器（触发器）或用 Vector3.Distance () 计算距离。": "球形 Trigger 或 Distance 算距离。",
    "一个怪物检测到玩家后，通知附近怪物共同攻击。": "一只发现玩家，通知附近协同攻击。",
    "待机、巡逻、追击、攻击、死亡等。": "Idle / Patrol / Chase / Attack / Die 等。",
    "用 NavMeshAgent 导航或添加碰撞检测重算路径。": "NavMeshAgent 或碰撞后重算路径。",
    "在 “Assets> Create” 中添加，修改模板默认内容。": "Assets→Create 加模板，改默认内容。",
    "用 GUILayout.Button () 创建，if 判断点击并执行逻辑。": "GUILayout.Button + if 点击执行。",
    "通过 EditorBuildSettings.scenes 获取并筛选启用场景。": "EditorBuildSettings.scenes 筛启用场景。",
    "继承 Editor，重写 OnInspectorGUI ()，用 [CustomEditor] 关联组件。": "继承 Editor + OnInspectorGUI + [CustomEditor]。",
    "在 MenuItem 路径中加标识，如 %（Ctrl）、&（Alt）等。": "MenuItem 路径加 %/& 等快捷键。",
    "继承 EditorWindow，重写 OnGUI ()，用 GetWindow () 显示。": "EditorWindow + OnGUI + GetWindow()。",
    "用 EditorPrefs 类的 Set 和 Get 方法。": "EditorPrefs Set/Get 持久化。",
    "调用 BuildPipeline.BuildPlayer () 传入相关参数。": "BuildPipeline.BuildPlayer() 打包。",
    "为特定组件显示自定义编辑界面，简化参数调整。": "自定义组件 Inspector，简化调参。",
    "自动合并字体纹理，整合文本字体信息。": "合并字体纹理，整合字体信息。",
    "初始化少量 Item，滑动时回收移出 Item 并复用。": "预建少量 Item，滑出回收复用。",
    "1. 固定父对象角落：按钮等固定元素；2. 拉伸父对象边缘：背景等自适应元素；3. 居中：标题等居中元素。": "角落固定（按钮）/ 边缘拉伸（背景）/ 居中（标题）。",
    "每帧先执行 Update，再执行协程 yield return 后逻辑。": "先 Update，再协程 yield 后逻辑。",
    "深度不同破坏渲染顺序，Unity 无法合批。": "深度不同打乱顺序，无法合批。",
    "避免动态操作打断合批，保证静态 UI 合批效果。": "防动态改动打断合批。",
    "1. 分帧更新；2. 视距剔除；3. 同一图集合并渲染。": "分帧更新、视距剔除、同图集合批。",
    "1. 减少重绘范围；2. 便于逻辑管理（如弹窗单独控制）。": "减重绘范围，便于分层管理。",
    "动态 UI 变化仅影响自身 Canvas，避免静态 UI 连带重绘。": "动态改只重绘自己 Canvas。",
    "受文本数量、字体样式、效果影响，可能单独触发渲染。": "文本量/字体/效果变化会增 Draw Call。",
    "减少不必要透明 UI，避免多层重叠，优先用不透明材质。": "少透明、少重叠，优先不透明。",
    "1. 低多边形模型；2. LOD 技术；3. 移除细节部件和多余材质。": "低模 + LOD + 减部件/材质。",
    "复用 UI 对象，避免频繁创建 / 销毁，减 GC 和性能开销。": "复用 UI，减创建销毁和 GC。",
    "问题：增渲染负担、提 Draw Call；优化：简化层级、减嵌套、禁无用元素。": "问题：层级深增 Draw Call；优化：扁平化、禁无用项。",
    "控一组 UI 的显示 / 交互，避免 SetActive 触发重绘。": "批量控显隐/交互，避免 SetActive 重绘。",
    "用 Profiler 监控 Draw Call、GC，确保帧率（如 60FPS+）和响应速度。": "Profiler 看 Draw Call/GC/帧率。",
    "合并小纹理为大纹理，减材质切换和 Draw Call。": "小图合并大图，减切换和 Draw Call。",
    "无需射线检测的纯显示元素（如纯图、纯文本）。": "纯展示图/文关闭 Raycast Target。",
    "1. 创 Canvas；2. 加 UI 元素；3. 调布局；4. 绑脚本；5. 调试。": "Canvas→加元素→布局→绑脚本→调试。",
    "合并同材质 / 纹理 UI 为一个 Draw Call；Unity 判断元素属性相同则一次性提交 GPU。": "同材质/纹理合并为一个 Draw Call 提交 GPU。",
    "分割遮罩内外 UI 为两部分，分别算 Draw Call。": "遮罩内外分开渲染，各计 Draw Call。",
    "1. 矩形遮罩（如滚动列表）用 RectMask2D；2. 非矩形（如圆形头像）用 Mask；3. 避嵌套。": "矩形→RectMask2D；异形→Mask；避免嵌套。",
    "URP、HDRP 等。": "URP / HDRP 等。",
    "分担 CPU 渲染压力，提效率。": "分担 CPU 渲染负载。",
    "按相机距离切换模型精度（远低近高）。": "远低近高切换模型精度。",
    "合批、复用纹理、移除无用碰撞体。": "合批、复用纹理、去无用碰撞体。",
    "不影响效果下，移除无用关键帧。": "不影响效果时删多余关键帧。",
    "增渲染复杂度，打断合批，提 Draw Call。": "增复杂度，打断合批，增 Draw Call。",
    "避免交叉排列，防合批失败。": "避免图文交叉，防合批失败。",
    "无需额外适配，多平台显示 / 操作一致。": "多平台显示操作一致。",
    "1. 设安全区域；2. 区分背景与异形屏适配元素；3. 异形屏算比例留空间。": "安全区 + 异形屏留边 + 背景/内容分层。",
    "加 Content Size Fitter，设 Horizontal/Vertical Fit 为 Preferred Size。": "Content Size Fitter，Fit=Preferred Size。",
    "1. 文本留 1.5 倍空间；2. 特殊字符 / 日期；3. 文化禁忌（如日本忌绿）。": "文本留 1.5 倍空间、特殊字符/日期、文化禁忌。",
    "1. 合批（同材质 / 纹理）；2. 用图集；3. 控层级和透明度。": "合批 + 图集 + 控层级/透明度。",
    "Canvas、UI 元素、RectTransform、Event System、Layout Groups。": "Canvas、UI 元素、RectTransform、EventSystem、Layout。",
    "1. Screen Space - Overlay；2. Screen Space - Camera；3. World Space。": "Overlay / Camera / World Space。",
    "对象池管理道具图标，复用防频繁创建 / 销毁。": "对象池复用道具图标。",
    "定义外观，控颜色 / 纹理 / 透明度，适配渲染管线。": "控颜色/纹理/透明度，适配管线。",
    "Canvas 内元素（位置 / 大小 / 内容）变化。": "Canvas 内元素位置/大小/内容变化。",
    "1. 动静分 Canvas；2. CanvasGroup 控显隐；3. 减动态元素更新。": "动静分 Canvas + CanvasGroup + 少更新。",
    "处理用户输入，分发点击 / 滑动等事件。": "处理并分发点击/滑动等事件。",
    "复用 UI 对象，减 GC 和性能开销。": "复用 UI，减 GC 和开销。",
    "对象行为随内部状态变；核心：状态变则行为变。": "状态变则行为变。",
    "抽象状态类、具体状态类、含状态属性的状态对象。": "抽象状态、具体状态、上下文对象。",
    "减 Canvas 重绘频率和 Draw Call 数量。": "减 Canvas 重绘和 Draw Call。",
    "为摄像机添加动画剪辑，通过关键帧记录不同时间点的位置和旋转信息，模拟镜头移动路径。": "给摄像机加 Clip，关键帧记录位置/旋转做漫游。",
}


def condense(answer: str) -> str:
    answer = answer.strip().rstrip("；;")
    if answer in OVERRIDES:
        return OVERRIDES[answer]
    # 通用规则
    s = answer
    s = re.sub(r"^负责", "", s)
    s = re.sub(r"^用于", "", s)
    s = re.sub(r"^包括", "", s)
    s = re.sub(r"^通过", "", s)
    s = re.sub(r"通常需要", "需", s)
    s = re.sub(r"可以通过", "可", s)
    s = re.sub(r"可以通过", "可", s)
    s = re.sub(r"（[^）]{8,}）", "", s)  # 去掉较长括号说明
    s = re.sub(r"\s+", "", s)
    s = s.replace("等方法", "")
    s = s.replace("等方式", "")
    if len(s) >= len(answer) - 2:
        return answer  # 没缩短则保留原文
    return s


def main():
    data = json.loads(JSON_PATH.read_text(encoding="utf-8"))
    questions = data["questions"]

    seen: set[str] = set()
    lines = [
        "# 元宇宙专业四题库 · 精简答案",
        "",
        f"> 共 {len(questions)} 题，去重后 {len({q['question'] for q in questions})} 题。语义不变，便于背诵。",
        "",
    ]

    current_week = ""
    current_unit = ""

    for i, q in enumerate(questions, 1):
        week = q.get("weekName", "")
        unit = q.get("category", "")
        if week != current_week:
            current_week = week
            lines += ["", f"## {week}", ""]
            current_unit = ""
        if unit != current_unit:
            current_unit = unit
            lines += [f"### {unit}", ""]

        qtext = q["question"]
        orig = q["answer"]
        short = OVERRIDES.get(orig, condense(orig))

        # 去重：同题只保留第一次（带完整上下文）
        key = qtext
        dup_mark = ""
        if key in seen:
            dup_mark = " *(重复)*"
        else:
            seen.add(key)

        lines.append(f"**{i}. {qtext}**{dup_mark}")
        lines.append(f"- 原答：{orig}")
        lines.append(f"- 精简：{short}")
        lines.append("")

    OUT_MD.write_text("\n".join(lines), encoding="utf-8")
    print(f"Wrote {OUT_MD} ({len(questions)} items)")


if __name__ == "__main__":
    main()
