@if "%_echo%"=="" echo off

setlocal 
dir build.ok > NUL ) || (
  @echo 'build.ok' not found.
  goto :ERROR
)

call %~d0%~p0..\..\..\config.bat


REM only a valid test if generics supported
%CLIX% .\main--optimize.exe
if ERRORLEVEL 1 goto Error

%CLIX% .\main.exe
if ERRORLEVEL 1 goto Error

:Ok
echo Ran fsharp %~f0 ok.
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
echo Test Script Failed (perhaps test did not emit test.ok signal file?)
endlocal
exit /b %ERRORLEVEL%

:SetError
set NonexistentErrorLevel 2> nul
goto Error

