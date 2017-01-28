@if "%_echo%"=="" echo off 

setlocal enableDelayedExpansion

SET DOTNETPATH_FILE=%TMP%\dotnetsdk-%RANDOM%.txt

powershell -ExecutionPolicy Unrestricted "scripts\use_dotnet_core_sdk.ps1" -OutPath "%DOTNETPATH_FILE%"
@if ERRORLEVEL 1 echo Error: .NET Core Sdk install failed  && goto :failure

set /p DOTNETSDK_DIR=<%DOTNETPATH_FILE%
goto :success

:failure
endlocal
exit /b 1

:success
endlocal & (
  @REM add DOTNETSDK_DIR to PATH, if set
  if not '%DOTNETSDK_DIR%' == '' ( 
    SET "PATH=%DOTNETSDK_DIR%;%PATH%" 
  )
)

exit /b 0
