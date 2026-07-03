@echo off
chcp 65001 >nul
setlocal
cd /d "%~dp0"

set "GITEE_REPO=https://gitee.com/nebula---beijing-g/metaverse-quiz.git"
set "GITEE_BRANCH=master"

echo.
echo ========================================================
echo   刷题站 - Git 推送到码云（解决网页不能传文件夹）
echo ========================================================
echo.

where git >nul 2>&1
if errorlevel 1 (
  echo [未安装 Git] 网页上传不支持文件夹，需要 Git 一次性推送。
  echo.
  echo 请先安装 Git（约 2 分钟）：
  echo   https://git-scm.com/download/win
  echo 安装时勾选 "Add Git to PATH"，装完重新双击本脚本。
  echo.
  start "" "https://git-scm.com/download/win"
  pause
  exit /b 1
)

where python >nul 2>&1
if errorlevel 1 (
  echo [错误] 未找到 Python
  pause
  exit /b 1
)

python scripts\prepare_static.py
if errorlevel 1 pause & exit /b 1

set "WORKDIR=%TEMP%\quiz-gitee-push"
if exist "%WORKDIR%" rmdir /s /q "%WORKDIR%"
mkdir "%WORKDIR%"
xcopy /E /I /Y "dist\*" "%WORKDIR%\" >nul

cd /d "%WORKDIR%"
git init
git checkout -b %GITEE_BRANCH%
git config user.name "QuizSite"
git config user.email "quiz@local"

git remote remove origin 2>nul
git remote add origin %GITEE_REPO%

echo.
echo 正在拉取码云已有 README（若有）…
git pull origin %GITEE_BRANCH% --allow-unrelated-histories --no-edit 2>nul

git add -A
git commit -m "Upload quiz site" 2>nul
if errorlevel 1 (
  echo 没有新改动，或提交失败。
)

echo.
echo 正在推送到码云…
echo 若弹出登录框，用你的码云账号登录（或私人令牌）。
echo.
git push -u origin %GITEE_BRANCH%
if errorlevel 1 (
  echo.
  echo [失败] 推送未成功。常见原因：
  echo   1. 账号密码错误 — 码云需用「私人令牌」代替密码
  echo      设置 -^> 安全设置 -^> 私人令牌 -^> 生成新令牌
  echo   2. 没有仓库写权限
  echo.
  pause
  exit /b 1
)

echo.
echo ========================================================
echo   推送成功！
echo ========================================================
echo   仓库: %GITEE_REPO%
echo.
echo   下一步：码云仓库 -^> 概览/服务 -^> Gitee Pages -^> 启动
echo   或左侧「仓库设置」里找 Pages
echo ========================================================
echo.
pause
