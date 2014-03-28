@if "%_echo%"=="" echo off
call %~d0%~p0..\..\..\config.bat

IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
    echo Test not supported except on Ultimate
    exit /b 0
)

call %~d0%~p0..\..\..\single-test-run.bat

exit /b %ERRORLEVEL%


