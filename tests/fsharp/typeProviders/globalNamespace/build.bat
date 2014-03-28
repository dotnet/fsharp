rem @if "%_echo%"=="" echo off

setlocal

call %~d0%~p0\..\..\..\config.bat

%CSC% /out:globalNamespaceTP.dll /debug+ /target:library /r:"%FSCOREDLLPATH%" globalNamespaceTP.cs
if ERRORLEVEL 1 goto :Error

"%FSC%" %fsc_flags% /debug+ /r:globalNamespaceTP.dll /optimize- test.fsx
if ERRORLEVEL 1 goto Error

:Ok
echo. > build.ok
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%


