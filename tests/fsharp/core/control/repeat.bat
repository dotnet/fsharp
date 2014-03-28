@if "%_echo%"=="" echo off

set fsi_flags= --tailcalls
:REPEAT

call %~d0%~p0..\..\single-test-run.bat

if errorlevel 1 goto :ERROR

goto :REPEAT
exit /b %ERRORLEVEL%


