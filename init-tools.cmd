@echo off
setlocal

set /p DOTNET_TOOLS_VERSION=<"%~dp0DotnetCLIToolsVersion.txt"
set DOTNET_TOOLS_PATH=%~dp0Tools\dotnet20
set dotnetexe=%DOTNET_TOOLS_PATH%\dotnet.exe

if not exist "%dotnetexe%" (
    echo powershell -ExecutionPolicy unrestricted -NoProfile -Command ".\scripts\dotnet-install.ps1 -InstallDir %DOTNET_TOOLS_PATH% -Architecture x64 -Version %DOTNET_TOOLS_VERSION% -NoPath true; exit $LastExitCode;"
         powershell -ExecutionPolicy unrestricted -NoProfile -Command ".\scripts\dotnet-install.ps1 -InstallDir %DOTNET_TOOLS_PATH% -Architecture x64 -Version %DOTNET_TOOLS_VERSION% -NoPath true; exit $LastExitCode;"
    if errorlevel 1 (
        echo ERROR: Could not install dotnet cli correctly.
        exit /b 1
    )
)
