@if "%_echo%"=="" echo off

setlocal

call %~d0%~p0..\..\..\config.bat

"%PEVERIFY%" magic.dll
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" provider.dll
@if ERRORLEVEL 1 goto Error


"%PEVERIFY%" test.exe
@if ERRORLEVEL 1 goto Error

test.exe
@if ERRORLEVEL 1 goto Error

endlocal
exit /b 0
