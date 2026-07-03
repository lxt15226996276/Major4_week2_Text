# 永久公网部署说明

> **不想本机常开、怕占内存？** 请看 **[云端说明.md](./云端说明.md)** — 免费静态托管即可，不必买服务器。

把刷题站部署到 **GitHub Pages**（或其它静态托管）后，会得到**固定 https 链接**，同学收藏一次即可长期使用，无需每天换 `trycloudflare.com` 临时地址。

> **与本地 start.bat 的区别**  
> - `start.bat`：临时公网隧道，关电脑即失效，适合临时分享  
> - **静态部署**：永久链接，但**网页录入题库不可用**（需在本地改 Excel/imports 后重新部署）

---

## 方式一：一键部署到 GitHub Pages（推荐）

### 第一次（约 5 分钟）

1. **安装 Git**（若尚未安装）  
   https://git-scm.com/download/win

2. **把项目推到 GitHub**（私有/公开仓库均可）  
   ```powershell
   cd d:\Project\UnityProject\Major4_week2_Text
   git init
   git add .
   git commit -m "Add quiz site"
   git branch -M main
   git remote add origin https://github.com/你的用户名/你的仓库名.git
   git push -u origin main
   ```

3. **配置永久链接（可选但建议）**  
   ```powershell
   copy QuizSite\deploy.config.json.example QuizSite\deploy.config.json
   ```
   编辑 `deploy.config.json`，把 `publicUrl` 改成：  
   `https://你的用户名.github.io/你的仓库名`

4. **双击运行**  
   `QuizSite\部署到公网.bat`  
   或 `QuizSite\deploy.bat`

5. **在 GitHub 开启 Pages（仅首次）**  
   仓库 → **Settings** → **Pages**  
   - Source: **Deploy from a branch**  
   - Branch: **gh-pages** / **/(root)**  
   - Save  

6. 等待 1～3 分钟，打开：  
   `https://你的用户名.github.io/你的仓库名/`  
   分享页：`.../share.html`（可截图二维码发微信）

### 更新题库后

Excel 或 `imports/` 有改动时，再双击 **`部署到公网.bat`** 即可，链接不变。

---

## 方式二：只打包、手动上传

不想用 Git 推送时：

1. 双击 `QuizSite\pack-static.bat`
2. 得到文件夹 `QuizSite\dist\`
3. 上传到任意静态托管，例如：
   - [Cloudflare Pages](https://pages.cloudflare.com/) → Create project → Direct Upload → 拖入 `dist` 文件夹
   - [Netlify Drop](https://app.netlify.com/drop) → 拖入 `dist` 文件夹

上传完成后，把平台给的固定域名填进 `deploy.config.json` 的 `publicUrl`，再运行一次 `pack-static.bat`，重新上传即可让分享页显示正确链接。

---

## 方式三：推代码自动部署（可选）

仓库已关联 GitHub 时，推送 `main` 分支会自动构建并发布（见 `.github/workflows/deploy-quiz.yml`）。

同样需在 Settings → Pages 里选择 **gh-pages** 分支。

---

## 常见问题

| 问题 | 处理 |
|------|------|
| 部署后 404 | 确认 Pages 分支为 `gh-pages`，且根目录有 `index.html` |
| 分享页显示「服务器未启动」 | 填写 `deploy.config.json` 的 `publicUrl` 后重新打包部署 |
| 录入页无法提交 | 正常；静态站无后端。本地改题库 → 再部署 |
| 链接里有多余路径 | `publicUrl` 不要末尾斜杠，脚本会自动处理 |

---

## 文件说明

| 文件 | 作用 |
|------|------|
| `deploy.bat` / `部署到公网.bat` | 构建 + 推送到 GitHub Pages |
| `pack-static.bat` | 仅生成 `dist/` |
| `scripts/prepare_static.py` | 构建题库并打包静态站 |
| `scripts/deploy_github_pages.py` | 推送到 `gh-pages` 分支 |
| `deploy.config.json` | 本地配置永久链接（勿提交敏感信息） |
| `dist/` | 部署产物（自动生成，已 gitignore） |
