@if "%_echo%"=="" echo off

REM build.bat produces only dll's. Nothing to run

goto SKip

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
