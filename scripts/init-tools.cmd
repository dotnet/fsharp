@echo off
setlocal enabledelayedexpansion

set /p DOTNET_TOOLS_VERSION=<"%~dp0..\DotnetCLIToolsVersion.txt"
set DOTNET_TOOLS_PATH=%~dp0..\artifacts\toolset\dotnet
set dotnetexe=%DOTNET_TOOLS_PATH%\dotnet.exe
set sdksentinel=%DOTNET_TOOLS_PATH%\sdk-version.txt

:: remove an old copy of the SDK
set cleanup_existing=
if exist "%sdksentinel%" (
    set /p INSTALLED_SDK_VERSION=<"%sdksentinel%"
    if not "%DOTNET_TOOLS_VERSION%" == "!INSTALLED_SDK_VERSION!" (
        :: wrong version installed, clean it up
        set cleanup_existing=1
        
    ) else (
        echo Found up-to-date SDK.
    )
) else (
    set cleanup_existing=1
)

if "!cleanup_existing!" == "1" (
    echo Removing stale SDK.
    rmdir /s /q "%DOTNET_TOOLS_PATH%"
)

:: download and install install SDK
if not exist "%dotnetexe%" (
    echo powershell -ExecutionPolicy unrestricted -NoProfile -Command ".\scripts\dotnet-install.ps1 -InstallDir %DOTNET_TOOLS_PATH% -Architecture x64 -Version %DOTNET_TOOLS_VERSION% -NoPath true; exit $LastExitCode;"
         powershell -ExecutionPolicy unrestricted -NoProfile -Command ".\scripts\dotnet-install.ps1 -InstallDir %DOTNET_TOOLS_PATH% -Architecture x64 -Version %DOTNET_TOOLS_VERSION% -NoPath true; exit $LastExitCode;"
    if errorlevel 1 (
        echo ERROR: Could not install dotnet cli correctly.
        exit /b 1
    )
    echo %DOTNET_TOOLS_VERSION%>"%sdksentinel%"
)
