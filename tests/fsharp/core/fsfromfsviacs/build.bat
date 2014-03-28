@if "%_echo%"=="" echo off

setlocal
if EXIST build.ok DEL /f /q build.ok
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.

call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

"%FSC%" %fsc_flags% -a -o:lib.dll -g lib.ml
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib.dll 
@if ERRORLEVEL 1 goto Error

%CSC% /nologo /target:library /r:"%FSCOREDLLPATH%" /r:lib.dll /out:lib2.dll lib2.cs 
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -r:lib.dll -r:lib2.dll -o:test.exe -g test.fsx
@if ERRORLEVEL 1 goto Error

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

