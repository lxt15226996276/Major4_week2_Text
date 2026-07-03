@echo off
chcp 65001 >nul
setlocal
set PYTHONUNBUFFERED=1
cd /d "%~dp0"

echo.
echo ========================================================
echo   刷题站 - 打包并准备上传云端（Cloudflare / Netlify）
echo ========================================================
echo.
echo  说明：部署到云端后，同学刷题时不占用你电脑内存。
echo        本脚本只在「打包题库」时运行几十秒。
echo.

where python >nul 2>&1
if errorlevel 1 (
  echo [错误] 未找到 Python，请先安装 Python。
  pause
  exit /b 1
)

python scripts\prepare_static.py
if errorlevel 1 (
  echo [错误] 打包失败。
  pause
  exit /b 1
)

echo.
echo --------------------------------------------------------
echo  下一步（任选一种免费平台）：
echo.
echo  [A] Cloudflare Pages（推荐，固定 https 链接）
echo      1. 浏览器打开: https://dash.cloudflare.com/
echo      2. Workers and Pages -^> Create -^> Pages -^> Upload assets
echo      3. 把下面打开的 dist 文件夹拖进去上传
echo.
echo  [B] Netlify Drop
echo      打开 https://app.netlify.com/drop 拖入 dist 文件夹
echo.
echo  详细图文见 QuizSite\云端说明.md
echo --------------------------------------------------------
echo.

start "" explorer "%~dp0dist"
timeout /t 2 >nul
start "" "https://dash.cloudflare.com/?to=/:account/workers-and-pages"

pause
