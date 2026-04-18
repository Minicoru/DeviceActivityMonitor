@echo off
echo Publishing Device Monitor as a Standalone Executable...
echo.

:: Check if dotnet is installed
where dotnet >nul 2>nul
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK is not installed on this system.
    echo Please download and install the .NET 8 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

:: Navigate to project directory
cd DeviceMonitor

echo Publishing to single executable...
:: Publish command to create a self-contained, single-file exe for 64-bit Windows
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Publishing failed.
    pause
    exit /b 1
)

echo.
echo =======================================================
echo Publish successful!
echo You can find the standalone DeviceMonitor.exe in:
echo DeviceMonitor\bin\Release\net8.0-windows\win-x64\publish\
echo =======================================================
echo.
pause
