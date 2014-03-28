@if "%_echo%"=="" echo off

call %~d0%~p0..\..\single-test-run.bat

exit /b %ERRORLEVEL%


