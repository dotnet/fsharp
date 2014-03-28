@if "%_echo%"=="" echo off

setlocal

REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)


rem fsc.exe building
    "%FSC%" %fsc_flags% --quotations-debug+ --optimize -o:test.exe -g test.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" test.exe 
    @if ERRORLEVEL 1 goto Error

    "%FSC%" %fsc_flags% --quotations-debug+ --optimize -o:test--optimize.exe -g test.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" test--optimize.exe 
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

