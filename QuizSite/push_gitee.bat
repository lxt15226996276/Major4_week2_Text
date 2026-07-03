@echo off
setlocal
cd /d "%~dp0"

set GITEE_REPO=https://gitee.com/nebula---beijing-g/metaverse-quiz.git
set GITEE_BRANCH=master

echo.
echo ========================================
echo   Push Quiz Site to Gitee
echo ========================================
echo.

where git >nul 2>&1
if errorlevel 1 (
  echo Git not found. Install from https://git-scm.com/download/win
  pause
  exit /b 1
)

where python >nul 2>&1
if errorlevel 1 (
  echo Python not found.
  pause
  exit /b 1
)

python scripts\prepare_static.py
if errorlevel 1 pause & exit /b 1

set WORKDIR=%TEMP%\quiz-gitee-push
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

echo Pulling remote master (merge README if any)...
git fetch origin %GITEE_BRANCH% 2>nul
git pull origin %GITEE_BRANCH% --allow-unrelated-histories --no-edit
if errorlevel 1 (
  echo Pull failed, will force push quiz site files...
)

git add -A
git commit -m "Upload quiz site" 2>nul

echo.
echo Pushing to Gitee...
echo Login: Gitee username + personal access token as password
echo.
git push -u origin %GITEE_BRANCH%
if errorlevel 1 (
  echo.
  echo Normal push rejected, trying force push...
  git push -u origin %GITEE_BRANCH% --force
)

if errorlevel 1 (
  echo.
  echo Push failed. Create token: Gitee Settings - Security - Private Token
  pause
  exit /b 1
)

echo.
echo Done! Enable Gitee Pages in repo settings.
echo.
pause
