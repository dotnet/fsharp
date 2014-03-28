@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)


"%FSC%" %fsc_flags% -a --optimize -o:lib.dll lib.mli lib.ml libv.ml
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib.dll
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% -a --optimize -r:lib.dll -o:lib2.dll lib2.mli lib2.ml lib3.ml
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib2.dll
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --optimize -r:lib.dll -r:lib2.dll -o:client.exe client.ml
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" client.exe
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

