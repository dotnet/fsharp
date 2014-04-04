@if "%_echo%"=="" echo off

setlocal
if EXIST build.ok DEL /f /q build.ok
call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)



"%FSC%" %fsc_flags% -g --optimize- -o:test.exe test.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fs
if ERRORLEVEL 1 goto Error

:Ok
echo Built fsharp %~f0 ok.
echo. > build.ok
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
call %SCRIPT_ROOT%\ChompErr.bat %ERRORLEVEL% %~f0
endlocal
exit /b %ERRORLEVEL%
