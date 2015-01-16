@if "%_echo%"=="" echo off

setlocal
dir build.ok > NUL ) || (
  @echo 'build.ok' not found.
  goto :ERROR
)

call %~d0%~p0..\..\..\config.bat

REM fsi.exe testing


echo TestC

  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags% -r module1.dll module2.fsx
  if NOT EXIST test.ok goto SetError

REM fsc.exe testing

echo TestD
    if exist test.ok (del /f /q test.ok)
    %CLIX% module2.exe
    @if ERRORLEVEL 1 goto Error
    if NOT EXIST test.ok goto SetError

    if exist test.ok (del /f /q test.ok)
    %CLIX% module2-opt.exe
    @if ERRORLEVEL 1 goto Error
    if NOT EXIST test.ok goto SetError

    if exist test.ok (del /f /q test.ok)
    %CLIX% module2-staticlink.exe
    @if ERRORLEVEL 1 goto Error
    if NOT EXIST test.ok goto SetError

:Ok
echo Ran fsharp %~f0 ok.
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
echo Test Script Failed (perhaps test did not emit test.ok signal file?)
endlocal
exit /b %ERRORLEVEL%


:SETERROR
set NonexistentErrorLevel 2> nul
goto Error
