@if "%_echo%"=="" echo off

call %~d0%~p0..\..\single-test-build.bat

exit /b %ERRORLEVEL%

