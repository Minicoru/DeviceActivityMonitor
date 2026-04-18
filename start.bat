@echo off
echo Starting Device Monitor...
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

:: Navigate to the project directory and run
cd DeviceMonitor
echo Building and running the application...
dotnet run

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] The application encountered an error and stopped.
    pause
)
