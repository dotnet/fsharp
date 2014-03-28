@if "%_echo%"=="" echo off

setlocal

call %~d0%~p0\..\..\..\config.bat

"%PEVERIFY%" test2a.dll
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test2b.dll
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test3.exe
@if ERRORLEVEL 1 goto Error

test3.exe
@if ERRORLEVEL 1 goto Error


:Ok
echo Ran fsharp %~f0 ok.
endlocal
exit /b 0

:Error
echo Test Script Failed (perhaps test did not emit test.ok signal file?)
endlocal
exit /b %ERRORLEVEL%
