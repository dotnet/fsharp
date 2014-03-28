@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.

call %~d0%~p0..\..\..\config.bat

call %~d0%~p0..\..\single-test-run.bat
exit /b %ERRORLEVEL%
