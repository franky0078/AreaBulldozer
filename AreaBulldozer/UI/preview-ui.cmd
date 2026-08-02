@echo off
setlocal
cd /d "%~dp0"

if not exist "node_modules\webpack-dev-server" (
    echo Installing Area Bulldozer preview dependencies...
    call npm install --no-audit --no-fund
    if errorlevel 1 (
        echo.
        echo npm install failed.
        pause
        exit /b 1
    )
)

echo Starting Area Bulldozer browser preview...
call npm run preview
endlocal
