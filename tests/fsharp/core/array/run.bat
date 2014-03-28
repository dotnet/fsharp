@if "%_echo%"=="" echo off

setlocal

call %~d0%~p0..\..\..\config.bat

call %~d0%~p0..\..\single-test-run.bat
exit /b %ERRORLEVEL%

:Skip
echo Skipped %~f0
endlocal
exit /b 0


