@if "%_echo%"=="" echo off

setlocal
if EXIST build.ok DEL /f /q build.ok
call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto SKip
)

  
"%FSC%" %fsc_flags% -a -o:pack.dll xlibC.ml
if ERRORLEVEL 1 goto Error  

"%PEVERIFY%" pack.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags%    -o:test.exe -r:pack.dll main.fs
if ERRORLEVEL 1 goto Error  

"%PEVERIFY%" test.exe
@if ERRORLEVEL 1 goto Error

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
