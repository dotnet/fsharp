rem @if "%_echo%"=="" echo off

setlocal

call %~d0%~p0\..\..\..\config.bat

if EXIST magic.dll del magic.dll
if errorlevel 1 goto :Error

"%FSC%" --out:magic.dll -a magic.fs --keyfile:magic.snk
if errorlevel 1 goto :Error

if EXIST provider.dll del provider.dll
if ERRORLEVEL 1 goto :Error

%CSC% /out:provider.dll /target:library "/r:%FSCOREDLLPATH%" /r:magic.dll provider.cs
if ERRORLEVEL 1 goto :Error

"%FSC%" %fsc_flags% /debug+ /r:provider.dll /optimize- test.fsx
if ERRORLEVEL 1 goto Error

:Ok
echo. > build.ok
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%