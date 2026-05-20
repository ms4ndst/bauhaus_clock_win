@echo off
REM ============================================
REM Bauhaus Clock Screensaver - Build Script
REM ============================================

echo.
echo ============================================
echo  BAUHAUS CLOCK SCREENSAVER BUILD SCRIPT
echo ============================================
echo.

REM Check if dotnet is available
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: dotnet command not found!
    echo Please install .NET SDK from https://dotnet.microsoft.com/download
    echo.
    pause
    exit /b 1
)

echo [1/4] Cleaning previous build...
if exist "bin\Release" rmdir /s /q "bin\Release"
if exist "obj" rmdir /s /q "obj"

echo [2/4] Building project in Release mode...
dotnet build -c Release

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Build failed!
    echo Check the error messages above.
    pause
    exit /b 1
)

echo [3/4] Renaming executable to .scr...
cd bin\Release\net48
if exist "BauhausScreensaver.scr" del "BauhausScreensaver.scr"
ren "BauhausScreensaver.exe" "BauhausScreensaver.scr"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Failed to rename file!
    pause
    exit /b 1
)

echo [4/4] Build complete!
echo.
echo ============================================
echo  BUILD SUCCESSFUL!
echo ============================================
echo.
echo Output file: bin\Release\net48\BauhausScreensaver.scr
echo.
echo To install:
echo   1. Navigate to bin\Release\net48\
echo   2. Right-click BauhausScreensaver.scr
echo   3. Select "Install"
echo.
echo To test:
echo   - Right-click BauhausScreensaver.scr
echo   - Select "Test"
echo.
echo ============================================

cd ..\..\..

pause
