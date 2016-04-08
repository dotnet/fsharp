@if "%_echo%"=="" echo off

if /I '%PERMUTATIONS%' == 'FSC_CORECLR' (
    call %~d0%~p0..\..\single-test-build.bat
) else (
    set PERMUTATIONS=FSC_BASIC
    call %~d0%~p0..\..\single-test-build.bat)

exit /b %ERRORLEVEL%

