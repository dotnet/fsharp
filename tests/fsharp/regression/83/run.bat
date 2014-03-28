@if "%_echo%"=="" echo off

setlocal
call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

if "%CLR_SUPPORTS_WINFORMS%"=="false" ( goto Skip )
if "%COMPLUS_Version%"=="v1.0.3705" ( goto Skip )

call %~d0%~p0..\..\single-test-run.bat
exit /b %ERRORLEVEL%

:Error
endlocal
exit /b %ERRORLEVEL%

