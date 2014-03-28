rem @if "%_echo%"=="" echo off

setlocal

call %~d0%~p0\..\..\..\config.bat

if EXIST magic.dll del magic.dll
if errorlevel 1 goto :Error

%FSC% --out:magic.dll -a magic.fs --keyfile:magic.snk
if errorlevel 1 goto :Error

REM == If we are running this test on a lab machine, we may not be running from an elev cmd prompt
REM == In that case, ADMIN_PIPE is set to the tool to invoke the command elevated.
IF DEFINED ADMIN_PIPE %ADMIN_PIPE% %GACUTIL% /if magic.dll
if errorlevel 1 goto :Error

if EXIST provider.dll del provider.dll
if ERRORLEVEL 1 goto :Error

%CSC% /out:provider.dll /target:library /r:"%FSCOREDLLPATH%" /r:magic.dll provider.cs
if ERRORLEVEL 1 goto :Error

%GACUTIL% /if magic.dll

"%FSC%" %fsc_flags% /debug+ /r:provider.dll /optimize- test.fsx
if ERRORLEVEL 1 goto Error


:Ok
echo. > build.ok
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%


