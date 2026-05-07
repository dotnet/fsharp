@if not defined _echo @echo off
setlocal

:: F# 15.9 revival: simplified init-tools.cmd.
:: Original downloaded dotnet CLI from web and restored Microsoft.DotNet.BuildTools
:: from dead dotnet.myget.org/F/dotnet-buildtools feed. CI agent has Network
:: Isolation which makes both fail.
::
:: New behavior: use the already-restored Microsoft.DotNet.BuildTools package
:: (referenced via packages.config) to populate Tools/ by copying its lib/.

set TOOLRUNTIME_DIR=%~dp0Tools
set BUILD_TOOLS_PATH=%~dp0packages\microsoft.dotnet.buildtools\1.0.27-prerelease-01001-04\lib

if exist "%TOOLRUNTIME_DIR%\init-tools.completed0" (
    echo Tools are already initialized.
    exit /b 0
)

if NOT exist "%BUILD_TOOLS_PATH%\Build.Common.props" (
    echo ERROR: Microsoft.DotNet.BuildTools 1.0.27-prerelease-01001-04 not found at %BUILD_TOOLS_PATH%
    echo This package must be restored via packages.config ^(NuGet restore^).
    exit /b 1
)

if NOT exist "%TOOLRUNTIME_DIR%" mkdir "%TOOLRUNTIME_DIR%"
echo Copying %BUILD_TOOLS_PATH%\* to %TOOLRUNTIME_DIR%\
xcopy /E /I /Y /Q "%BUILD_TOOLS_PATH%" "%TOOLRUNTIME_DIR%" > nul
if errorlevel 1 (
    echo ERROR: xcopy failed.
    exit /b 1
)

echo Init-Tools.cmd completed for BuildTools 1.0.27-prerelease-01001-04 > "%TOOLRUNTIME_DIR%\init-tools.completed0"
echo Tools initialized.
exit /b 0
