@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

REM only a valid test if generics supported

  "%FSC%" %fsc_flags% -a -o:test.dll -g test.fs
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" test.dll
  @if ERRORLEVEL 1 goto Error


  %CSC% /r:"%FSCOREDLLPATH%" /reference:test.dll /debug+ testcs.cs
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" testcs.exe
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
