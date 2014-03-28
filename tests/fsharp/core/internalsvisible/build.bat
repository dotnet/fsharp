if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)

REM Test internals visible
echo == Compiling F# Library
"%FSC%" %fsc_flags% --version:1.2.3 --keyfile:key.snk -a --optimize -o:library.dll library.fsi library.fs
@if ERRORLEVEL 1 goto Error

echo == Verifying F# Library
"%PEVERIFY%" library.dll
@if ERRORLEVEL 1 goto Error

echo == Compiling C# Library
%CSC% /target:library /keyfile:key.snk /out:librarycs.dll librarycs.cs
@if ERRORLEVEL 1 goto Error

echo == Verifying C# Library
"%PEVERIFY%" librarycs.dll
@if ERRORLEVEL 1 goto Error

echo == Compiling F# main referencing C# and F# libraries
"%FSC%" %fsc_flags% --version:1.2.3 --keyfile:key.snk --optimize -r:library.dll -r:librarycs.dll -o:main.exe main.fs
@if ERRORLEVEL 1 goto Error

echo == Verifying F# main
"%PEVERIFY%" main.exe
@if ERRORLEVEL 1 goto Error

echo == Run F# main. Quick test!
main.exe
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

