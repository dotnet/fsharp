@if "%_echo%"=="" echo off
goto ok

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.

call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

REM  NOTE that this test does not do anything.
REM  PEVERIFY not needed

:Ok
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%
