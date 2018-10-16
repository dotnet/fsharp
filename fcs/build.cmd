@echo off

setlocal
pushd %~dp0%

if errorlevel 1 (
  endlocal
  exit /b %errorlevel%
)

.paket\paket.exe restore
if errorlevel 1 (
  endlocal
  exit /b %errorlevel%
)

packages\FAKE\tools\FAKE.exe build.fsx %*
if errorlevel 1 (
  endlocal
  exit /b %errorlevel%
)
endlocal
exit /b 0
