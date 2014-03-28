@if "%_echo%"=="" echo off

call %~d0%~p0..\..\..\..\tests\fsharp\single-test-run.bat

exit /b %ERRORLEVEL%


