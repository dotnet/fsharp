@if "%_echo%"=="" echo off

call %~d0%~p0..\..\..\..\config.bat

IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
    echo Test not supported except on Ultimate
    exit /b 0
)

IF EXIST test.exe (
   echo Running test.exe to warm up SQL
   test.exe > nul 2> nul
)

call %~d0%~p0..\..\..\single-test-run.bat

exit /b %ERRORLEVEL%