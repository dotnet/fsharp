@if "%_echo%"=="" echo off

setlocal

call %~d0%~p0..\..\..\..\config.bat

IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
    echo Test not supported except on Ultimate
    exit /b 0
)

set CONTROL_FAILURES_LOG=%~dp0..\ConsoleApplication1\bin\Debug\profile78\control_failures.log

..\ConsoleApplication1\bin\Debug\profile78\PortableTestEntry.exe
endlocal
exit /b %ERRORLEVEL%

:Skip
echo Skipped %~f0
endlocal
exit /b 0


