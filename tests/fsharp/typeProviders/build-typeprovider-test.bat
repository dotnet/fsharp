@if "%_echo%"=="" echo off

setlocal

call %~d0%~p0\..\..\config.bat

if EXIST provided.dll del provided.dll
if ERRORLEVEL 1 goto :Error

%FSC% --out:provided.dll -a ..\helloWorld\provided.fs
if errorlevel 1 goto :Error


if EXIST providedJ.dll del providedJ.dll
if ERRORLEVEL 1 goto :Error

%FSC% --out:providedJ.dll -a ..\helloWorld\providedJ.fs
if errorlevel 1 goto :Error

if EXIST providedK.dll del providedK.dll
if ERRORLEVEL 1 goto :Error

%FSC% --out:providedK.dll -a ..\helloWorld\providedK.fs
if errorlevel 1 goto :Error



if EXIST provider.dll del provider.dll
if ERRORLEVEL 1 goto :Error

%FSC% --out:provider.dll -a provider.fsx
if ERRORLEVEL 1 goto :Error

call %~d0%~p0..\single-test-build.bat
goto :Ok

:Ok
echo. > build.ok
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%


