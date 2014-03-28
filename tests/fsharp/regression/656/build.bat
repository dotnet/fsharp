@if "%_echo%"=="" echo off

setlocal
if EXIST build.ok DEL /f /q build.ok
call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

  "%FSC%" %fsc_flags% -o:pack%ILX_CONFIG%.exe misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs
  if ERRORLEVEL 1 goto Error  

  "%PEVERIFY%" pack%ILX_CONFIG%.exe
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
endlocal
exit /b %ERRORLEVEL%
