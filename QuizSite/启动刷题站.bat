@echo off
chcp 65001 >nul
echo.
echo  [提示] 此方式需要保持电脑开机，关机会无法访问。
echo  若希望关电脑也能用，请双击「上传云端.bat」，详见「云端说明.md」
echo.
pause
cd /d "%~dp0"
call start.bat
