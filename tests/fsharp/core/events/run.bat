@if "%_echo%"=="" echo off

setlocal 
dir build.ok > NUL ) || (
  @echo 'build.ok' not found.
  goto :ERROR
)

call %~d0%~p0..\..\..\config.bat

if exist test.ok (del /f /q test.ok)

%CLIX% "%FSI%" test.fs && (
dir test.ok > NUL 2>&1 ) || (
@echo :FSI failed;
goto Error
set ERRORMSG=%ERRORMSG% FSI failed;
)

%CLIX% .\testcs.exe
if ERRORLEVEL 1 goto Error

:Ok
echo Ran fsharp %~f0 ok.
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%

