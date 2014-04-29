@if "%_echo%"=="" echo off

setlocal

call %~d0%~p0\..\..\..\config.bat

if EXIST provider.dll del provider.dll
if errorlevel 1 goto :Error

"%FSC%" --out:provided.dll -a ..\helloWorld\provided.fs
if errorlevel 1 goto :Error

"%FSC%" --out:provider.dll -a ..\helloWorld\provider.fsx
if errorlevel 1 goto :Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test1.dll -a test1.fsx
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2a.dll -a -r:test1.dll test2a.fsx
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2b.dll -a -r:test1.dll test2b.fsx
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test3.exe -r:test1.dll -r:test2a.dll -r:test2b.dll test3.fsx
if ERRORLEVEL 1 goto Error

:Ok
echo. > build.ok
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%