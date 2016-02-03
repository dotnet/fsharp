@if "%_echo%"=="" echo off

setlocal 
dir build.ok > NUL ) || (
  @echo 'build.ok' not found.
  goto :ERROR
)

call %~d0%~p0..\..\..\config.bat

%CLIX% .\test.exe
if ERRORLEVEL 1 goto Error

:Ok
echo Ran fsharp %~f0 ok.
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%

