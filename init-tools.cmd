@if not defined _echo @echo off
setlocal

set INIT_TOOLS_LOG=%~dp0init-tools.log
set TOOLRUNTIME_DIR=%~dp0Tools
set DOTNET_PATH=%TOOLRUNTIME_DIR%\dotnetcli\
set DOTNET_CMD=%DOTNET_PATH%dotnet.exe
set /P DOTNET_VERSION=< "%~dp0DotnetCLIVersion.txt"
set DOTNET_TOOLS_SEMAPHORE_DIR=%TOOLRUNTIME_DIR%\%DOTNET_VERSION%
set TOOLS_INIT_RETURN_CODE=0

:: if force option is specified then clean the tool runtime and build tools package directory to force it to get recreated
if [%1]==[force] (
  if exist "%TOOLRUNTIME_DIR%" rmdir /S /Q "%TOOLRUNTIME_DIR%"
)

echo 10

:: If sempahore exists do nothing
if exist "%DOTNET_TOOLS_SEMAPHORE_DIR%\init-tools.completed" (
  echo Tools are already initialized.
echo 15
  goto :DONE
)
echo 20

if exist "%TOOLRUNTIME_DIR%" rmdir /S /Q "%TOOLRUNTIME_DIR%"

echo 40

echo Installing dotnet cli...
if NOT exist "%DOTNET_PATH%" mkdir "%DOTNET_PATH%"
if [%PROCESSOR_ARCHITECTURE%]==[x86] (set DOTNET_ZIP_NAME=dotnet-dev-win-x86.%DOTNET_VERSION%.zip) else (set DOTNET_ZIP_NAME=dotnet-dev-win-x64.%DOTNET_VERSION%.zip)
set DOTNET_REMOTE_PATH=https://dotnetcli.blob.core.windows.net/dotnet/Sdk/%DOTNET_VERSION%/%DOTNET_ZIP_NAME%
set DOTNET_LOCAL_PATH=%DOTNET_PATH%%DOTNET_ZIP_NAME%
echo Installing '%DOTNET_REMOTE_PATH%' to '%DOTNET_LOCAL_PATH%' >> "%INIT_TOOLS_LOG%"
powershell -NoProfile -ExecutionPolicy unrestricted -Command "$retryCount = 0; $success = $false; do { try { $wc = New-Object System.Net.WebClient; $wc.Proxy = [System.Net.WebRequest]::DefaultWebProxy; $wc.Proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials; $wc.DownloadFile('%DOTNET_REMOTE_PATH%', '%DOTNET_LOCAL_PATH%'); $success = $true; } catch { if ($retryCount -ge 6) { throw; } else { $retryCount++; Start-Sleep -Seconds (5 * $retryCount); } } } while ($success -eq $false); Add-Type -Assembly 'System.IO.Compression.FileSystem' -ErrorVariable AddTypeErrors; if ($AddTypeErrors.Count -eq 0) { [System.IO.Compression.ZipFile]::ExtractToDirectory('%DOTNET_LOCAL_PATH%', '%DOTNET_PATH%') } else { (New-Object -com shell.application).namespace('%DOTNET_PATH%').CopyHere((new-object -com shell.application).namespace('%DOTNET_LOCAL_PATH%').Items(),16) }" >> "%INIT_TOOLS_LOG%"
if NOT exist "%DOTNET_LOCAL_PATH%" (
  echo ERROR: Could not install dotnet cli correctly. See '%INIT_TOOLS_LOG%' for more details.
  set TOOLS_INIT_RETURN_CODE=1
  goto :DONE
)

:: Create sempahore file
echo Done initializing tools.
mkdir %DOTNET_TOOLS_SEMAPHORE_DIR%
echo Init-Tools.cmd completed for DotnetCli Version: %DOTNET_VERSION% > "%DOTNET_TOOLS_SEMAPHORE_DIR%\init-tools.completed"

:DONE
exit /b %TOOLS_INIT_RETURN_CODE%

