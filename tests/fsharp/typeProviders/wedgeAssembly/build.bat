@if "%_echo%"=="" echo off

setlocal

call %~d0%~p0\..\..\..\config.bat

if EXIST provider.dll del provider.dll
if errorlevel 1 goto :Error

if EXIST provided.dll del provided.dll
if errorlevel 1 goto :Error

"%FSC%" --out:provided.dll -a ..\helloWorld\provided.fs
if errorlevel 1 goto :Error

if EXIST providedJ.dll del providedJ.dll
if errorlevel 1 goto :Error

"%FSC%" --out:providedJ.dll -a ..\helloWorld\providedJ.fs
if errorlevel 1 goto :Error

if EXIST providedK.dll del providedK.dll
if errorlevel 1 goto :Error

"%FSC%" --out:providedK.dll -a ..\helloWorld\providedK.fs
if errorlevel 1 goto :Error

"%FSC%" --out:provider.dll -a ..\helloWorld\provider.fsx
if errorlevel 1 goto :Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2a.dll -a test2a.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2b.dll -a test2b.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test3.exe test3.fsx
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2a-with-sig.dll -a test2a.fsi test2a.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2b-with-sig.dll -a test2b.fsi test2b.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test3-with-sig.exe --define:SIGS test3.fsx
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2a-with-sig-restricted.dll -a test2a-restricted.fsi test2a.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2b-with-sig-restricted.dll -a test2b-restricted.fsi test2b.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test3-with-sig-restricted.exe --define:SIGS_RESTRICTED test3.fsx
if ERRORLEVEL 1 goto Error

:Ok
echo. > build.ok
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%