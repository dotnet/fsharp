@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

"%FSC%" %fsc_flags% --optimize -r:jstm.dll -a chan.fsi chan.fs -o:dbwlib2--optimize.dll
@if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" dbwlib2--optimize.dll
  @if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --optimize -r:jstm.dll -r:dbwlib2--optimize.dll main.fs -o:main--optimize.exe
@if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" dbwlib2--optimize.dll
  @if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% -r:jstm.dll -a -o:dbwlib2.dll  chan.fsi chan.fs 
@if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" dbwlib2.dll
  @if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -r:jstm.dll -r:dbwlib2.dll -o:main.exe main.fs
@if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" main.exe
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
