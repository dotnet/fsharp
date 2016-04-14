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


    "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" test.exe 
    @if ERRORLEVEL 1 goto Error

    "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" test--optimize.exe 
    @if ERRORLEVEL 1 goto Error

REM == 
REM == Disabled until Don checks in the missing files
REM == 
GOTO :SkipMultiPartTests
    "%FSC%" %fsc_flags% -a -o:testlib.dll  -r:System.Data.Services.Client.dll -r:FSharp.Data.TypeProviders.dll -g test-part1.fs    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" testlib.dll
    @if ERRORLEVEL 1 goto Error

    "%FSC%" %fsc_flags% -r:testlib.dll -o:testapp.exe  -r:System.Data.Services.Client.dll -g test-part2.fs
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" testapp.exe 
    @if ERRORLEVEL 1 goto Error

    "%FSC%" %fsc_flags% -o:testtwoparts.exe -r:System.Data.Services.Client.dll -r:FSharp.Data.TypeProviders.dll -g test-part1.fs test-part2.fs 
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" testtwoparts.exe 
    @if ERRORLEVEL 1 goto Error

:SkipMultiPartTests

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

