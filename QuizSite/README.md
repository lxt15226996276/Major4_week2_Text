# 随机刷题网站

**两种访问方式：**

| 方式 | 链接 | 你的电脑要常开吗 |
|------|------|------------------|
| **上传云端（推荐）** | 固定 `https://xxx.pages.dev` | **不用** |
| **临时隧道** | 每次新的 `trycloudflare.com` | 要 |

云端说明见 **[云端说明.md](./云端说明.md)**，双击 **`上传云端.bat`** 打包后拖到 Cloudflare（免费）。

永久部署（GitHub Pages）见 **[DEPLOY.md](./DEPLOY.md)**。

---

## 临时启动（本地 + 公网隧道）

双击 **`start.bat`**，或：

```powershell
python QuizSite/scripts/build_all.py
python QuizSite/scripts/server.py
```

终端会出现类似：

```
★ 公网地址（发给任何人，不同 WiFi 也能打开）：
   https://xxxx.trycloudflare.com
```

**把这个链接发给任何人即可。**

> 首次启动会自动下载 cloudflared（约 20MB，仅一次）。  
> 关窗口或 Ctrl+C 后公网链接失效；下次启动会生成新链接。

## 仅局域网（不建公网）

```powershell
python QuizSite/scripts/server.py --lan-only
```

## 录入题库

- **本地服务器模式**：浏览器打开 `公网地址/import.html`
- **永久静态站**：在本地改 Excel 或 `imports/` 后运行 `部署到公网.bat` 重新发布

## 题库来源

| 题库 | 题数 |
|------|------|
| 元宇宙专业四 · 全部试卷 | 255 |
| 大厂 Unity 面试题 | 79 |

更新 `Tests/元宇宙专业四题库.xlsx` 后重新运行 `build_all.py` 或 `部署到公网.bat`。
