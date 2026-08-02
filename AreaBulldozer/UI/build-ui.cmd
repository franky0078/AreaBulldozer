@echo off
setlocal
cd /d "%~dp0"

if not exist node_modules (
    echo Installing Area Bulldozer UI dependencies...
    call npm install
    if errorlevel 1 goto :error
)

echo Building Area Bulldozer UI...
call npm run build
if errorlevel 1 goto :error

echo.
echo UI build completed successfully.
pause
exit /b 0

:error
echo.
echo UI build failed. Check the messages above.
pause
exit /b 1
