@echo off
setlocal
set PYTHONUNBUFFERED=1
cd /d "%~dp0"

echo.
echo ========================================
echo   Quiz Site - Starting (public URL)
echo ========================================
echo.

where python >nul 2>&1
if errorlevel 1 (
  echo [ERROR] Python not found.
  echo Install: https://www.python.org/downloads/
  echo Check "Add Python to PATH" when installing.
  pause
  exit /b 1
)

echo [1/2] Building question banks...
python scripts\build_all.py
if errorlevel 1 (
  echo [ERROR] Build failed.
  pause
  exit /b 1
)

echo.
echo [2/2] Starting server...
echo.
echo IMPORTANT:
echo   - Copy the https:// link below and send to WeChat
echo   - Do NOT send start.bat (phones cannot open .bat)
echo   - Share page: https://xxx/share.html
echo.

python scripts\server.py %*
pause
