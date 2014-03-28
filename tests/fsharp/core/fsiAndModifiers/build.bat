setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.

call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

if exist TestLibrary.dll (del /f /q TestLibrary.dll)
"%FSI%" %fsi_flags%  --maxerrors:1 < prepare.fsx
@if ERRORLEVEL 1 goto Error

:Ok
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%
