@echo off
setlocal
set PYTHONUNBUFFERED=1
cd /d "%~dp0"

echo.
echo ========================================
echo   Quiz Site - Static Deploy (GitHub Pages)
echo ========================================
echo.

where python >nul 2>&1
if errorlevel 1 (
  echo [ERROR] Python not found.
  pause
  exit /b 1
)

where git >nul 2>&1
if errorlevel 1 (
  echo [ERROR] Git not found. Install: https://git-scm.com/download/win
  echo Or see DEPLOY.md for manual upload of the dist folder.
  pause
  exit /b 1
)

if not exist "deploy.config.json" (
  echo [TIP] Copy deploy.config.json.example to deploy.config.json
  echo       and set your permanent publicUrl after first deploy.
  echo.
)

python scripts\prepare_static.py
if errorlevel 1 (
  echo [ERROR] Build failed.
  pause
  exit /b 1
)

echo.
python scripts\deploy_github_pages.py
if errorlevel 1 (
  echo.
  echo Deploy failed. See DEPLOY.md for help.
  pause
  exit /b 1
)

echo.
pause
