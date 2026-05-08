@if not defined _echo @echo off
setlocal

:: F# 15.9 revival: simplified init-tools.cmd.
:: Original downloaded from dotnet.myget.org/F/dotnet-buildtools (DEAD).
:: New: use already-restored Microsoft.DotNet.BuildTools package + download
:: dotnet 2.1.300 SDK from official Microsoft CDN.

set TOOLRUNTIME_DIR=%~dp0Tools
set BUILD_TOOLS_PATH=%~dp0packages\Microsoft.DotNet.BuildTools.1.0.27-prerelease-01001-04\lib
set DOTNET20_PATH=%TOOLRUNTIME_DIR%\dotnet20
set DOTNET_SDK_VERSION=2.1.300
set DOTNET_SDK_URL=https://dotnetcli.azureedge.net/dotnet/Sdk/%DOTNET_SDK_VERSION%/dotnet-sdk-%DOTNET_SDK_VERSION%-win-x64.zip

if exist "%TOOLRUNTIME_DIR%\init-tools.completed0" (
    echo Tools are already initialized.
    exit /b 0
)

:: Step 1: Copy BuildTools lib to Tools/
if NOT exist "%BUILD_TOOLS_PATH%\Build.Common.props" (
    echo ERROR: Microsoft.DotNet.BuildTools 1.0.27-prerelease-01001-04 not found at %BUILD_TOOLS_PATH%
    echo This package must be restored via packages.config ^(NuGet restore^).
    exit /b 1
)

if NOT exist "%TOOLRUNTIME_DIR%" mkdir "%TOOLRUNTIME_DIR%"
echo Copying BuildTools lib to %TOOLRUNTIME_DIR%\
xcopy /E /I /Y /Q "%BUILD_TOOLS_PATH%" "%TOOLRUNTIME_DIR%" > nul
if errorlevel 1 (
    echo ERROR: xcopy of BuildTools lib failed.
    exit /b 1
)

:: Step 2: Download dotnet 2.1.300 SDK to Tools/dotnet20/
if exist "%DOTNET20_PATH%\dotnet.exe" (
    echo Tools\dotnet20\dotnet.exe already exists.
) else (
    if NOT exist "%DOTNET20_PATH%" mkdir "%DOTNET20_PATH%"
    set DOTNET_SDK_ZIP=%TEMP%\dotnet-sdk-%DOTNET_SDK_VERSION%-win-x64.zip
    echo Downloading dotnet %DOTNET_SDK_VERSION% SDK from %DOTNET_SDK_URL% ...
    powershell -NoProfile -ExecutionPolicy Bypass -Command ^
      "$ProgressPreference='SilentlyContinue'; Invoke-WebRequest -Uri '%DOTNET_SDK_URL%' -OutFile '%TEMP%\dotnet-sdk-%DOTNET_SDK_VERSION%-win-x64.zip' -UseBasicParsing"
    if errorlevel 1 (
        echo ERROR: Failed to download dotnet %DOTNET_SDK_VERSION% SDK.
        exit /b 1
    )
    echo Extracting dotnet SDK to %DOTNET20_PATH% ...
    powershell -NoProfile -ExecutionPolicy Bypass -Command ^
      "Add-Type -AssemblyName System.IO.Compression.FileSystem; [System.IO.Compression.ZipFile]::ExtractToDirectory('%TEMP%\dotnet-sdk-%DOTNET_SDK_VERSION%-win-x64.zip', '%DOTNET20_PATH%')"
    if errorlevel 1 (
        echo ERROR: Failed to extract dotnet SDK.
        exit /b 1
    )
    del "%TEMP%\dotnet-sdk-%DOTNET_SDK_VERSION%-win-x64.zip" 2>nul
)

if NOT exist "%DOTNET20_PATH%\dotnet.exe" (
    echo ERROR: dotnet.exe still missing at %DOTNET20_PATH% after extraction.
    exit /b 1
)

:: Step 3: Copy MicroBuild.Core .props/.targets to Tools/ (Build.Common.targets imports them)
set MICROBUILD_PATH=%~dp0packages\MicroBuild.Core.0.2.0\build
if exist "%MICROBUILD_PATH%\MicroBuild.Core.targets" (
    echo Copying MicroBuild.Core targets to %TOOLRUNTIME_DIR%\
    xcopy /Y /Q "%MICROBUILD_PATH%\*.props" "%TOOLRUNTIME_DIR%\" > nul
    xcopy /Y /Q "%MICROBUILD_PATH%\*.targets" "%TOOLRUNTIME_DIR%\" > nul
) else (
    echo WARNING: MicroBuild.Core.0.2.0 not at %MICROBUILD_PATH%
)

echo Init-Tools.cmd completed for BuildTools 1.0.27-prerelease-01001-04 + dotnet SDK %DOTNET_SDK_VERSION% > "%TOOLRUNTIME_DIR%\init-tools.completed0"
echo Tools initialized.
exit /b 0
