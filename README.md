# Device Monitor

A native Windows graphical application that monitors plug-and-play devices connected to the PC in real-time. It accurately tracks connection and disconnection events, maintains a summary count of events per device, and saves the history locally so it is not lost when the program closes.

## How to Run (For Non-Developers)

The easiest way to run the application is to double-click the included batch scripts.

### Method 1: Run Directly (Requires .NET SDK)

1. Double-click on `start.bat`.
2. A terminal window will open, compile the program, and launch the graphical interface.
3. *Note:* This requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) to be installed on your computer.

### Method 2: Create a Standalone Executable (No .NET installation required for end-users)

If you want to share this application with a friend who doesn't have programming tools installed, you can generate a single `.exe` file that contains everything needed to run.

1. Double-click on `publish_single_exe.bat`.
2. The script will bundle the application and the .NET runtime into a single `.exe` file.
3. When it finishes, navigate to `DeviceMonitor\bin\Release\net8.0-windows\win-x64\publish\`.
4. You will find `DeviceMonitor.exe`. You can copy this single file to any Windows 64-bit machine and double-click it to run the application instantly!
