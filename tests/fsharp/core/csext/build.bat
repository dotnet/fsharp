@if "%_echo%"=="" echo off

call %~d0%~p0..\..\..\..\tests\fsharp\single-test-build.bat

exit /b %ERRORLEVEL%

