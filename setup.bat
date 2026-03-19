@echo off
setlocal

echo ========================================
echo Decrypt - First Time Setup
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
set DATA_ACCESS_CSPROJ=%BACKEND_DIR%\Decrypt.DataAccess\Decrypt.DataAccess.csproj
set IMPORT_PROJECT=%ROOT%Backend\MockLoader\MockLoader.csproj


if not exist "%API_PROJECT%" (
    echo [ERROR] Backend project not found: %API_PROJECT%
    pause
    exit /b 1
)

if not exist "%IMPORT_PROJECT%" (
    echo [ERROR] Import utility project not found: %IMPORT_PROJECT%
    pause
    exit /b 1
)

if not exist "%FRONTEND_DIR%\package.json" (
    echo [ERROR] Frontend package.json not found in: %FRONTEND_DIR%
    pause
    exit /b 1
)

echo [1/5] Restoring backend solution...
pushd "%BACKEND_DIR%"
dotnet restore
if errorlevel 1 (
    echo [ERROR] dotnet restore failed.
    pause
    exit /b 1
)

echo.
echo [2/5] Building backend...
dotnet build -c Release --no-restore
if errorlevel 1 (
    echo [ERROR] dotnet build failed.
    pause
    exit /b 1
)


eecho [3/5] Starting backend once to create and migrate database...
start "Decrypt API Setup" /min cmd /c "dotnet run --project "%API_PROJECT%""

echo Waiting for backend startup...
timeout /t 10 /nobreak >nul

echo.
echo [4/5] Running import utility...
dotnet run --project "%IMPORT_PROJECT%" -c Release
if errorlevel 1 (
    echo [ERROR] Import utility failed.
    echo Backend window may still be running. Please close it manually if needed.
    pause
    exit /b 1
)

echo.
echo Stopping temporary backend process...
taskkill /FI "WINDOWTITLE eq Decrypt API Setup" /T /F >nul 2>nul

echo.
echo [5/5] Installing frontend dependencies...
pushd "%FRONTEND_DIR%"
call npm install
if errorlevel 1 (
    echo [ERROR] npm install failed.
    popd
    pause
    exit /b 1
)
popd

echo.
echo ========================================
echo Setup completed successfully.
echo ========================================
pause
exit /b 0