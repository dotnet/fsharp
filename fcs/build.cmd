@echo off

setlocal
pushd %~dp0%

dotnet tool restore

if errorlevel 1 (
  endlocal
  exit /b %errorlevel%
)

dotnet paket restore
if errorlevel 1 (
  endlocal
  exit /b %errorlevel%
)

:: don't care if this fails
dotnet build-server shutdown >NUL 2>&1

dotnet fake build -t %*

if errorlevel 1 (
  endlocal
  exit /b %errorlevel%
)
endlocal
exit /b 0
