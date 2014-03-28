@if "%_echo%"=="" echo off

setlocal
dir build.ok > NUL ) || (
  @echo 'build.ok' not found.
  goto :ERROR
)

call %~d0%~p0..\..\..\config.bat

  if exist test.ok (del /f /q test.ok)
  %CLIX% pack%ILX_CONFIG%.exe
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

:SetError
set NonexistentErrorLevel 2> nul
goto Error
