@echo off
chcp 65001 >nul
setlocal
cd /d "%~dp0"

echo.
echo ========================================================
echo   刷题站 - 国内可访问部署（码云 Gitee Pages）
echo ========================================================
echo.
echo  Cloudflare 在国内经常打不开，请改用码云（免费、国内快）
echo.

where python >nul 2>&1
if errorlevel 1 (
  echo [错误] 未找到 Python
  pause
  exit /b 1
)

python scripts\prepare_static.py
if errorlevel 1 pause & exit /b 1

python scripts\zip_dist.py
if errorlevel 1 pause & exit /b 1

echo.
echo --------------------------------------------------------
echo  按下面步骤操作（约 5 分钟，只需注册一次）：
echo.
echo  1. 打开 https://gitee.com 注册（手机号即可）
echo  2. 右上角 + 号 -^> 新建仓库
echo     名称填: major4-quiz
echo     勾选「公开」
echo  3. 进仓库 -^> 若网页不支持传文件夹：
echo     双击「推送到码云.bat」（需先安装 Git，见脚本提示）
echo  4. 仓库上方「服务」-^> Gitee Pages -^> 启动
echo     分支 master，目录 /（根目录）
echo  5. 等 1 分钟，得到链接：
echo     https://你的用户名.gitee.io/major4-quiz/
echo.
echo  详细说明见：国内部署.md
echo --------------------------------------------------------
echo.

start "" explorer "%~dp0dist"
start "" explorer /select,"%~dp0quiz-site-upload.zip"
start "" "https://gitee.com/projects/new"

pause
