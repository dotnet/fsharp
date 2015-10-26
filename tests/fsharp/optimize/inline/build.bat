@if "%_echo%"=="" echo off

setlocal
if EXIST build.ok DEL /f /q build.ok
call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

"%FSC%" %fsc_flags% -g --optimize- --target:library -o:lib.dll lib.fs lib2.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -g --optimize- --target:library -o:lib3.dll -r:lib.dll lib3.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -g --optimize- -o:test.exe test.fs -r:lib.dll -r:lib3.dll
if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --optimize --target:library -o:lib--optimize.dll -g lib.fs  lib2.fs
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize --target:library -o:lib3--optimize.dll -r:lib--optimize.dll -g lib3.fs  
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fs -r:lib--optimize.dll  -r:lib3--optimize.dll
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
