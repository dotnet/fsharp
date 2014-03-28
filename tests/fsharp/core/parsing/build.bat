@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)


REM This is a single-empty-line test file. 

  "%FSC%" %fsc_flags% -a -o:crlf.dll -g crlf.ml
  @if ERRORLEVEL 1 goto Error

REM Another simple test

  "%FSC%" %fsc_flags% -o:toplet.exe -g toplet.ml
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" toplet.exe
  @if ERRORLEVEL 1 goto Error


:Ok
echo Built fsharp %~f0 ok.
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%

