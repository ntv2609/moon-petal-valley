
@echo off
chcp 65001 >nul
cd /d "%~dp0"

title Moonpetal Valley - Gom Code

echo =====================================
echo        MOONPETAL VALLEY - GOM CODE
echo =====================================
echo.

python gom_code.py

echo.
echo =====================================
echo Da hoan thanh!
echo =====================================
pause
