@echo off
setlocal

echo ========================================
echo Decrypt - Start Application
echo ========================================
echo.

where dotnet >nul 2>nul
if errorlevel 1 (
    echo [ERROR] .NET SDK is not installed or not in PATH.
    pause
    exit /b 1
)

where npm >nul 2>nul
if errorlevel 1 (
    echo [ERROR] npm is not installed or not in PATH.
    pause
    exit /b 1
)

set ROOT=%~dp0
set BACKEND_DIR=%ROOT%Backend
set FRONTEND_DIR=%ROOT%Frontend
set API_PROJECT=%ROOT%Backend\Decrypt.Api\Decrypt.Api.csproj



if not exist "%API_PROJECT%" (
    echo [ERROR] Backend project not found: %API_PROJECT%
    pause
    exit /b 1
)

if not exist "%FRONTEND_DIR%\package.json" (
    echo [ERROR] Frontend package.json not found in: %FRONTEND_DIR%
    pause
    exit /b 1
)

echo Starting backend...
start "Decrypt API" cmd /k "dotnet run --project "%API_PROJECT%""

echo Starting frontend...
start "Decrypt UI" cmd /k "cd /d "%FRONTEND_DIR%" && npm start"

echo Opening browser...
timeout /t 5 /nobreak >nul
start http://localhost:3000

echo.
echo Backend and frontend were started in separate windows.
exit /b 0