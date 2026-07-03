@echo off
setlocal
set PYTHONUNBUFFERED=1
cd /d "%~dp0"

echo.
echo ========================================
echo   Quiz Site - Pack static files only
echo ========================================
echo.

where python >nul 2>&1
if errorlevel 1 (
  echo [ERROR] Python not found.
  pause
  exit /b 1
)

python scripts\prepare_static.py
echo.
echo Output folder: dist\
echo Upload dist\ to Cloudflare Pages / Netlify if not using GitHub.
echo.
pause
