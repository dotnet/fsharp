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


    "%FSC%" %fsc_flags% -o:module1.dll --target:library module1.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" module1.dll 
    @if ERRORLEVEL 1 goto Error

    "%FSC%" %fsc_flags% -o:module2.exe -r:module1.dll module2.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" module2.exe 
    @if ERRORLEVEL 1 goto Error
    

    "%FSC%" %fsc_flags% -o:module1-opt.dll --target:library --optimize module1.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" module1-opt.dll 
    @if ERRORLEVEL 1 goto Error

    "%FSC%" %fsc_flags% -o:module2-opt.exe -r:module1-opt.dll --optimize module2.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" module2-opt.exe 
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

