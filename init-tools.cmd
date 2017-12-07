@if not defined _echo @echo off
setlocal

set INIT_TOOLS_LOG=%~dp0init-tools.log
set PACKAGES_DIR=%~dp0packages\
set TOOLRUNTIME_DIR=%~dp0Tools

set DOTNET_PATH=%TOOLRUNTIME_DIR%\dotnetcli\
set DOTNET_CMD=%DOTNET_PATH%dotnet.exe

set DOTNET_TOOLS_PATH=%TOOLRUNTIME_DIR%\dotnet20\
set DOTNET_TOOLS_CMD=%DOTNET_TOOLS_PATH%dotnet.exe

if [%BUILDTOOLS_SOURCE%]==[] set BUILDTOOLS_SOURCE=https://dotnet.myget.org/F/dotnet-buildtools/api/v3/index.json
set /P BUILDTOOLS_VERSION=< "%~dp0BuildToolsVersion.txt"
set BUILD_TOOLS_PATH=%PACKAGES_DIR%microsoft.dotnet.buildtools\%BUILDTOOLS_VERSION%\lib\
set PROJECT_JSON_PATH=%TOOLRUNTIME_DIR%\%BUILDTOOLS_VERSION%
set PROJECT_JSON_FILE=%PROJECT_JSON_PATH%\project.json
set PROJECT_JSON_CONTENTS={ "dependencies": { "Microsoft.DotNet.BuildTools": "%BUILDTOOLS_VERSION%" , "Microsoft.DotNet.BuildTools.Coreclr": "1.0.4-prerelease"}, "frameworks": { "dnxcore50": { } } }
set BUILD_TOOLS_SEMAPHORE=%PROJECT_JSON_PATH%\init-tools.completed0
set TOOLS_INIT_RETURN_CODE=0

:: if force option is specified then clean the tool runtime and build tools package directory to force it to get recreated
if [%1]==[force] (
  if exist "%TOOLRUNTIME_DIR%" rmdir /S /Q "%TOOLRUNTIME_DIR%"
  if exist "%PACKAGES_DIR%microsoft.dotnet.buildtools" rmdir /S /Q "%PACKAGES_DIR%microsoft.dotnet.buildtools"
)

set /p DOTNET_TOOLS_VERSION=< "%~dp0DotnetCLIToolsVersion.txt"
if not exist "%DOTNET_TOOLS_PATH%\sdk\%DOTNET_TOOLS_VERSION%" (
  :: dotnet cli doesn't yet exist, delete the semaphore
  del "%BUILD_TOOLS_SEMAPHORE%" >NUL 2>&1
)

:: If sempahore exists do nothing
if exist "%BUILD_TOOLS_SEMAPHORE%" (
  echo Tools are already initialized.
  goto :DONE
)

if exist "%TOOLRUNTIME_DIR%" rmdir /S /Q "%TOOLRUNTIME_DIR%"

:: Download Nuget.exe
if NOT exist "%PACKAGES_DIR%NuGet.exe" (
  if NOT exist "%PACKAGES_DIR%" mkdir "%PACKAGES_DIR%"
  powershell -NoProfile -ExecutionPolicy unrestricted -Command "$wc = New-Object System.Net.WebClient; $wc.Proxy = [System.Net.WebRequest]::DefaultWebProxy; $wc.Proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials; $wc.DownloadFile('https://www.nuget.org/nuget.exe', '%PACKAGES_DIR%NuGet.exe')
)

if NOT exist "%PROJECT_JSON_PATH%" mkdir "%PROJECT_JSON_PATH%"
echo %PROJECT_JSON_CONTENTS% > "%PROJECT_JSON_FILE%"
echo Running %0 > "%INIT_TOOLS_LOG%"

if exist "%DOTNET_TOOLS_PATH%" goto :afterdotnettoolsrestore

echo Installing dotnet OLD VERSION OF THE cli...
echo ==========================================
echo This is temporary until we build using the new dotnetcli
echo The dotnet cli is a large file it may take a few minutes ...
echo powershell -ExecutionPolicy unrestricted -NoProfile -Command ".\scripts\dotnet-install.ps1 -InstallDir %DOTNET_TOOLS_PATH% -Architecture x64 -Version %DOTNET_TOOLS_VERSION% -NoPath true; exit $LastExitCode;"
powershell -ExecutionPolicy unrestricted -NoProfile -Command ".\scripts\dotnet-install.ps1 -InstallDir %DOTNET_TOOLS_PATH% -Architecture x64 -Version %DOTNET_TOOLS_VERSION% -NoPath true; exit $LastExitCode;"
if errorlevel 1 (
   echo ERROR: Could not install dotnet cli correctly.
   set TOOLS_INIT_RETURN_CODE=1
   goto :DONE
)
:afterdotnettoolsrestore

set /p DOTNET_VERSION=< "%~dp0DotnetCLIVersion.txt"
if exist "%DOTNET_CMD%" goto :afterdotnetrestore

echo Installing dotnet cli...
echo The dotnet cli is a large file it may take a few minutes ...
echo powershell -ExecutionPolicy unrestricted -NoProfile -Command ".\scripts\dotnet-install.ps1 -InstallDir %DOTNET_PATH% -Architecture x64 -Version %DOTNET_VERSION% -NoPath true; exit $LastExitCode;"
powershell -ExecutionPolicy unrestricted -NoProfile -Command ".\scripts\dotnet-install.ps1 -InstallDir %DOTNET_PATH% -Architecture x64 -Version %DOTNET_VERSION% -NoPath true; exit $LastExitCode;"
if errorlevel 1 (
   echo ERROR: Could not install dotnet cli correctly.
   set TOOLS_INIT_RETURN_CODE=1
   goto :DONE
)
:afterdotnetrestore

if exist "%BUILD_TOOLS_PATH%" goto :afterbuildtoolsrestore
echo Restoring BuildTools version %BUILDTOOLS_VERSION%...
echo Running: "%DOTNET_CMD%" restore "%PROJECT_JSON_FILE%" --packages "%PACKAGES_DIR% " --source "%BUILDTOOLS_SOURCE%" >> "%INIT_TOOLS_LOG%"
call "%DOTNET_CMD%" restore "%PROJECT_JSON_FILE%" --packages "%PACKAGES_DIR% " --source "%BUILDTOOLS_SOURCE%" >> "%INIT_TOOLS_LOG%"
if NOT exist "%BUILD_TOOLS_PATH%init-tools.cmd" (
  echo ERROR: Could not restore build tools correctly. See '%INIT_TOOLS_LOG%' for more details.
  set TOOLS_INIT_RETURN_CODE=1
  goto :DONE
)

:afterbuildtoolsrestore

echo Initializing BuildTools ...
echo Running: "%BUILD_TOOLS_PATH%init-tools.cmd" "%~dp0" "%DOTNET_CMD%" "%TOOLRUNTIME_DIR%" >> "%INIT_TOOLS_LOG%"
call "%BUILD_TOOLS_PATH%init-tools.cmd" "%~dp0" "%DOTNET_CMD%" "%TOOLRUNTIME_DIR%" >> "%INIT_TOOLS_LOG%"

echo Updating CLI NuGet Frameworks map...
robocopy "%TOOLRUNTIME_DIR%" "%DOTNET_PATH%\sdk\%DOTNET_VERSION%" NuGet.Frameworks.dll /XO >> "%INIT_TOOLS_LOG%"
set UPDATE_CLI_ERRORLEVEL=%ERRORLEVEL%
if %UPDATE_CLI_ERRORLEVEL% GTR 1 (
  echo ERROR: Failed to update Nuget for CLI {Error level %UPDATE_CLI_ERRORLEVEL%}. Please check '%INIT_TOOLS_LOG%' for more details. 1>&2
  exit /b %UPDATE_CLI_ERRORLEVEL%
)

:: Create sempahore file
echo Done initializing tools.
echo Init-Tools.cmd completed for BuildTools Version: %BUILDTOOLS_VERSION% > "%BUILD_TOOLS_SEMAPHORE%"

:DONE

exit /b %TOOLS_INIT_RETURN_CODE%

