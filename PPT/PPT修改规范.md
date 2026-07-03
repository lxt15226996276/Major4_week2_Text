# PPT 终版生成准则

> **金标准（用户手动另存终版）：** `01-第一单元画布和基础布局.pptx`  
> **内容来源：** `01-第一单元画布和基础布局（原版）.pptx`  
> **模板来源：** `标准.pptx`

---

## 一、第一单元终版：三文件总体对比

| 项目 | 终版（金标准） | 原版 | 标准模板 |
|------|---------------|------|---------|
| 页数 | **48** | 47 | 14 |
| 画幅 | **16:9**（12192000×6858000） | 4:3（9144000×5144135） | 16:9 |
| 文件大小 | **约 24.3 MB** | 约 30.4 MB | 约 1.7 MB |
| zip 部件数 | **349** | 412 | 160 |
| handoutMaster | **0** | 4 | 0 |
| zip 目录项 | **17** | 19 | 17 |
| 页眉 logo | **image4 = 标准振涛弘业**（MD5 一致） | 振涛教育（嵌在 logo 图） | 振涛弘业 |
| 封面布局 | slideLayout10（深色封面） | slideLayout55 | slideLayout1 |
| 内页布局 | slideLayout5 × 46 | slideLayout60 × 37 等 | slideLayout5 等 |
| 末页 | **感谢聆听**（slideLayout23） | THANKYOU（slideLayout61） | 感谢聆听（slideLayout23） |
| 品牌文案 | 「感谢聆听」×2；无「振涛教育」 | 无「感谢聆听」 | 「感谢聆听」×2 |

---

## 二、页码映射规则

```text
终版第 1 页  ← 标准模板第  1 页（深色封面，标题换成本单元名）
终版第 2 页  ← 标准模板第  2 页（课堂礼仪，整页复制，文本 100% 一致）
终版第 N 页  ← 原版第 N-1 页（N = 3 … 47，内容主体迁移）
终版第 48 页 ← 标准模板第 14 页（感谢聆听，整页替换原版末页）
```

**原版第 1 页内容**（标题「《unity开发实战》第一单元画布和布局」）→ **终版第 1 页封面标题**（非内页）。

---

## 三、逐页对照（终版 48 页）

| 终版 | 来源 | 布局 | 图片数 | 页面要点 / 与原版差异 |
|------|------|------|--------|----------------------|
| 1 | 标准·封面 | slideLayout10 | 1 | 标题：《unity开发实战》第一单元画布和布局；深色封面；背景 media=`image5.png`（原版封面为 image8）；右上角无旧 logo |
| 2 | 标准·礼仪 | slideLayout5 | 5 | **整页来自标准**；文本：课堂礼仪 / 起立 / 班长带领问好 / 讲师回礼 / 呐喊口号 / 整理工装 / 落座 |
| 3 | 原版 2 | slideLayout5 | 0 | 知识目标（UI 样式、策划案、画布属性、公共属性、文本控件） |
| 4 | 原版 3 | slideLayout5 | 0 | 能力目标（UI 布局、控件使用） |
| 5 | 原版 4 | slideLayout5 | 0 | 第一节课：游戏 UI（掌握程度：熟练） |
| 6 | 原版 5 | slideLayout5 | 1 | 课堂礼仪内页；原版 2 图→终版 1 图 |
| 7 | 原版 6 | slideLayout5 | 0 | 课程导入：什么是 UI / UI 作用 / 在哪见过 UI |
| 8 | 原版 7 | slideLayout5 | 3 | 课程导入（配图页） |
| 9 | 原版 8 | slideLayout5 | 0 | 课程导入-什么是 UI（UI 定义长文） |
| 10 | 原版 9 | slideLayout5 | 1 | 课程导入-Canvas 组件（定义 + GameObject 菜单） |
| 11 | 原版 10 | slideLayout5 | 1 | 课程导入-Canvas 组件（配图） |
| 12 | 原版 11 | slideLayout5 | 1 | 课程导入-Panel 组件 |
| 13 | 原版 12 | slideLayout5 | 1 | 课程导入-Image 组件 |
| 14 | 原版 13 | slideLayout5 | 0 | 知识总结：UI / UGUI / Canvas / Panel / Image |
| 15 | 原版 14 | slideLayout5 | 0 | 第二节课：画布（掌握程度：熟练） |
| 16 | 原版 15 | slideLayout5 | 2 | **「其他」页**；原版含「课堂礼仪 Classroom etiquette」副标题，终版仅保留「其他 + 进入上课状态…」；原版 3 图→终版 2 图 |
| 17 | 原版 16 | slideLayout5 | 0 | 课堂回顾：UI 是什么 / Canvas 组件 |
| 18 | 原版 17 | slideLayout5 | 1 | 课堂导入：如何控制 UI 元素的位置（终版文本略截断为「…的位？」，以金标准终版为准） |
| 19 | 原版 18 | slideLayout5 | 1 | CanvasScaler 组件讲解（定义） |
| 20–24 | 原版 19–23 | slideLayout5 | 各 1 | CanvasScaler 配图讲解 ×5 |
| 25 | 原版 24 | slideLayout5 | 0 | 课后作业：创建 UI / Button / 三种缩放模式 |
| 26 | 原版 25 | slideLayout5 | 0 | 课后作业：Canvas Scaler 三种缩放 / Match 属性 |
| 27 | 原版 26 | slideLayout5 | 0 | 第三节课：基础布局（掌握程度：熟练） |
| 28 | 原版 27 | slideLayout5 | 2 | **「其他」页**；同第 16 页，原版多「课堂礼仪」副标题；3 图→2 图 |
| 29 | 原版 28 | slideLayout5 | 0 | 课程回顾：上节课内容 |
| 30 | 原版 29 | slideLayout5 | 1 | 课程导入（配图） |
| 31 | 原版 30 | slideLayout5 | 1 | RectTransform 组件讲解（定义长文） |
| 32–37 | 原版 31–36 | slideLayout5 | 各 1–2 | RectTransform 配图讲解 ×6 |
| 38 | 原版 37 | slideLayout5 | 0 | 课程小结：锚点 / 支点 / 缩放 / 移动和位置 |
| 39 | 原版 38 | slideLayout5 | 0 | 第四节课：文本 Text（掌握程度：熟练） |
| 40 | 原版 39 | slideLayout5 | 2 | 课堂礼仪内页；3 图→2 图 |
| 41 | 原版 40 | slideLayout5 | 0 | 课堂回顾：基础布局 / 锚点 |
| 42 | 原版 41 | slideLayout5 | 1 | **课堂导入**；原版含「如何显示文字？hello world」，终版金标准仅保留「课堂导入 Leading In」（以终版为准） |
| 43 | 原版 42 | slideLayout5 | 1 | Text 组件讲解（定义） |
| 44–45 | 原版 43–44 | slideLayout5 | 各 1 | Text 组件配图 ×2 |
| 46 | 原版 45 | slideLayout5 | 0 | 知识总结：Font / 段落 / Rich Text / Shadow / Outline |
| 47 | 原版 46 | slideLayout5 | 0 | 课后作业：详见练习手册 |
| 48 | 标准·末页 | slideLayout23 | 3 | **整页来自标准**；「感谢聆听」+「文化启迪智慧 教育点亮人生」；替换原版「THANKYOU」 |

### 内页文本保真度（脚本自动迁移 vs 金标准）

- **45 / 46 页**（终版 3–47 对应原版 2–46）：文本与原版 **100% 一致**（字符级）
- **3 页偏低相似**（终版 16、28、42）：主要为 **副标题/提问句在套模板时被裁掉或重排**，金标准终版以当前文件为准
- **4 页图片数减少**（终版 6、16、28、40）：各少 1 张装饰图，主体内容图保留

---

## 四、与标准模板的关系（仅 3 页来自标准）

| 标准页 | 终版页 | 处理方式 |
|--------|--------|---------|
| slide 1 | 终版 1 | 保留深色封面版式；**标题替换**为单元名；封面 media 换为 `image5.png`；去右上角旧 logo |
| slide 2 | 终版 2 | **原样插入**（课堂礼仪，5 张配图，文本不改） |
| slide 14 | 终版 48 | **原样替换**原版末页（感谢聆听 + 底部 slogan） |

标准模板其余页（知识回顾、课程目标、占位讲解页等）**不插入**；内页全部来自原版内容 + 标准白底母版 slideLayout5。

---

## 五、品牌与视觉规范（终版实测）

| 元素 | 终版要求 | 来源 |
|------|---------|------|
| 页眉 logo | `ppt/media/image4.png` = 标准模板同款（振涛弘业） | 标准 |
| 封面背景 | slide1 → 深色全幅背景图 **仅此一页** | 原版封面 media |
| 封面标题 | 原版 slide1 标题（去重后 2 个文本框） | 原版 slide1 |
| **内页背景** | **白底 + 标准母版顶栏蓝线 + 页眉振涛弘业** | 标准母版 slideLayout5 |
| 末页 | 感谢聆听 + 文化启迪智慧 教育点亮人生 | 标准 slide14 |
| 禁止出现 | 振涛教育、ZHENTAO EDUCATION、THANKYOU 末页 | — |
| 禁止出现 | **内页全幅深色背景图**（原版封面图不得复制到内页） | — |

### 5.1 背景分层（批量必检）

```text
第 1 页（封面）  slideLayout10  →  可保留原版深色全幅背景图 + 标题文本
第 2 页（礼仪）  slideLayout5   →  100% 标准白底模板（5 张礼仪配图）
第 3…N-1 页     slideLayout5   →  白底 + 振涛弘业页眉 + 原版正文/配图
最后一页        slideLayout23  →  标准「感谢聆听」
```

**常见误判：** 把原版第 1 页的深色背景图当作「模板」套到全部页面。  
**正确做法：** 仅 `slideLayout10` 保留全幅背景图；`slideLayout5/4/9` 内页必须 **丢弃** 原点锚定的全幅 `<p:pic>`（`x,y ≤ 150000` 且宽高 ≥ 92% 画布）。

### 5.2 封面标题去重

部分单元原版封面存在重复文本框（如第二单元 sp0/sp3 同为「《unity开发实战》」）。  
`process_slide_xml` 在 `slideLayout10` 按 **归一化文本** 去重，终版封面应只有 **2 个** 文本形状（书名 + 单元名），与第一单元终版一致。

---

## 5.3 WPS 打开时不关文件（批量强制）

用户常在 WPS 中预览 PPT，**不得要求关闭 WPS** 才能跑脚本。

| 步骤 | 目标路径被占用时 |
|------|-----------------|
| 步骤 1 | 先写入 `PPT/_new_{输出文件名}.pptx`，再尝试替换正式路径 |
| 步骤 2/3 | 优先处理 `_new_` 副本；若 patch 失败则复制到 `_new_` 再 patch |
| 交付 | 控制台打印实际写入路径；用户可在 WPS 中「关闭后替换」或直接用 `_new_` 文件 |

第二单元临时文件：`PPT/_new_02-第二单元常用控件（一）.pptx`

---

## 六、金标准包结构（WPS 手动另存后）

| 指标 | 第一单元终版 |
|------|-------------|
| zip parts | **349** |
| handoutMaster | **0** |
| zip 目录项 | **17**（`_rels/`、`docProps/`、`ppt/` 及各子目录） |
| 可打开性 | WPS 直接打开，无修复对话框 |

> 脚本直出约 357 parts / 26.8 MB；**349 parts / 24.3 MB 的用户另存版才是交付金标准**。

---

## 七、生成流程（三步，禁止 `_process_unit`）

```bash
python PPT/_apply_template.py
python PPT/_remove_cover_logo.py
python PPT/_apply_branding_inplace.py
# 最后：WPS 打开 → 确认 → 另存为（与金标准包结构对齐）
```

第二单元同构三脚本：`_apply_template_unit02.py` → `_remove_cover_logo_unit02.py` → `_apply_branding_unit02.py`

步骤 2/3 使用 `_pptx_zip.py`（.NET ZipFile.Update），**禁止** `ZipFile('w')` 整包重压缩。

---

## 八、错误记录（摘要）

| # | 问题 | 结论 |
|---|------|------|
| 16 | 第二单元误删 `tag75.xml` | orphan 扫描须含 presentation.xml.rels |
| 17 | 脚本版 WPS 打不开；手动另存后可开 | 步骤 2/3 禁止整包重压缩；去掉 tag1/2/15 |
| 18 | 金标准对照基准确立 | 以用户另存终版（349 parts）为准；见第三节逐页表 |
| **19** | **内页出现深色全幅背景** | 原版封面背景图被复制到 `slideLayout5` 内页；内页须 **丢弃** `is_full_slide_background_pic`；仅 `slideLayout10` 保留 |
| **20** | **封面标题重叠** | 原版封面重复文本框未去重；`slideLayout10` 按归一化文本 dedupe，终版封面 sp=2 |
| **21** | **WPS 打开时脚本失败** | 步骤 1→`_new_*.pptx`；步骤 2/3→优先/回退 `_new_`；不要求用户关 WPS |

### 8.1 批量修改前自检（每单元）

- [ ] 第 1 页 thumbnail：深色封面；第 2 页起 thumbnail：**白底 + 顶栏 logo**
- [ ] zip 内页（slide3+）无原点全幅背景 `<p:pic>`
- [ ] 封面 `slide1.xml` 文本形状 ≤ 2 组（无重叠书名/单元名）
- [ ] 第 2 页 = 标准课堂礼仪（文本 100% 一致）
- [ ] 末页 = 标准感谢聆听（slideLayout23）
- [ ] `image4.png` MD5 与 `标准.pptx` 一致
- [ ] WPS 直接打开无修复对话框（脚本版通过后建议用户另存对齐 349 parts）

---

## 九、脚本清单

| 文件 | 用途 |
|------|------|
| `_apply_template.py` | 步骤 1 |
| `_remove_cover_logo.py` | 步骤 2 |
| `_apply_branding_inplace.py` | 步骤 3 |
| `_pptx_zip.py` | 步骤 2/3 原地 patch |
| `_apply_template_unit02.py` 等 | 第二单元镜像 |
| `_unit02_common.py` | 第二单元路径解析 + WPS 占用回退 |

**交付前：** WPS 双击打开无修复提示；页眉为振涛弘业；第 2 页礼仪、末页感谢聆听与标准一致；**内页白底、封面 alone 深色**。

---

## 十、第二单元（进行中）

| 项目 | 值 |
|------|-----|
| 原版 | `TextPPT/02-第二单元常用控件（一）（原版）.pptx`（68 页 → 终版 69 页） |
| 输出 | `TextPPT/02-第二单元常用控件（一）.pptx` |
| WPS 占用备选 | `_new_02-第二单元常用控件（一）.pptx` |
| 原版封面布局 | slideLayout1（深色全幅图）→ 终版 slideLayout10 |
| 原版内页布局 | slideLayout1（深色母版）→ 终版 slideLayout5（白底） |

```bash
python PPT/_apply_template_unit02.py
python PPT/_remove_cover_logo_unit02.py
python PPT/_apply_branding_unit02.py
```
