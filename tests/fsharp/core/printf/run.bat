@if "%_echo%"=="" echo off

set PERMUTATIONS=FSC_BASIC
call %~d0%~p0..\..\single-test-run.bat

exit /b %ERRORLEVEL%


