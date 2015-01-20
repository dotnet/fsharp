@if "%_echo%"=="" echo off

setlocal 
rem there is no build.bat for this testcase, so don't check for build.ok

call %~d0%~p0..\..\..\config.bat

  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags%  --maxerrors:1 < test1.ml
  if NOT EXIST test.ok goto SetError

  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags%  --maxerrors:1 load1.fsx
  if NOT EXIST test.ok goto SetError

  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags%  --maxerrors:1 load2.fsx
  if NOT EXIST test.ok goto SetError

  REM Check we can also compile, for sanity's sake
  "%FSC%" load1.fsx
  @if ERRORLEVEL 1 goto Error

  REM Check we can also compile, for sanity's sake
  "%FSC%" load2.fsx
  @if ERRORLEVEL 1 goto Error


:Ok
echo Ran fsharp %~f0 ok.
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%

:SETERROR
set NonexistentErrorLevel 2> nul
goto Error
