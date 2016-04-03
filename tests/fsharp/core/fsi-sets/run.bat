@if "%_echo%"=="" echo off

setlocal 
rem there is no build.bat for this testcase, so don't check for build.ok

call %~d0%~p0..\..\..\config.bat

  if exist test1.ok (del /f /q test1.ok)
  "%FSI%" %fsi_flags%                          < test1.fsx
  if NOT EXIST test1.ok goto SetError

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
