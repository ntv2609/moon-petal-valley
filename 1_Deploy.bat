
@echo off
chcp 65001 >nul
cd /d "%~dp0"

title Moonpetal Valley - Deploy

echo =====================================
echo       MOONPETAL VALLEY - DEPLOY
echo =====================================
echo.

python deploy.py

echo.
echo =====================================
echo Da chay xong Deploy!
echo =====================================
pause
