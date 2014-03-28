@if "%_echo%"=="" echo off

setlocal
call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

if "%CLR_SUPPORTS_WINFORMS%"=="false" ( goto Skip)

call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error


:Ok
echo Built fsharp %~f0 ok.
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%
