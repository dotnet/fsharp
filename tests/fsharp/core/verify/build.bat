@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

"%PEVERIFY%" "%FSCOREDLLPATH%"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" %FSCBinPath%\FSharp.Build.dll
@if ERRORLEVEL 1 goto Error



if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 

  REM Use /MD because this contains some P/Invoke code  
  "%PEVERIFY%" /MD %FSCBinPath%\FSharp.Compiler.dll
  @if ERRORLEVEL 1 goto Error

  REM Use /MD because this contains some P/Invoke code  
  "%PEVERIFY%" /MD %FSCBinPath%\FSharp.LanguageService.dll
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" %FSCBinPath%\FSharp.ProjectSystem.Base.dll
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" %FSCBinPath%\FSharp.ProjectSystem.dll
  @if ERRORLEVEL 1 goto Error


  "%PEVERIFY%" %FSCBinPath%\fsi.exe
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" %FSCBinPath%\FSharp.Compiler.Server.Shared.dll
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" %FSCBinPath%\FSharp.Compiler.Interactive.Settings.dll
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" /MD %FSCBinPath%\FSharp.VisualStudio.Session.dll
  @if ERRORLEVEL 1 goto Error

  REM Skipping remainder of test for FSI.EXE
  goto Ok

)

"%FSC%" %fsc_flags% -o:xmlverify.exe -g xmlverify.fs
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" xmlverify.exe
@if ERRORLEVEL 1 goto Error

REM == Calc correct path to FSharp.Core.dll no matter what arch we are on
call :SetFSCoreXMLPath "%FSCOREDLLPATH%"

%CLIX% xmlverify.exe "%FSHARPCOREXML%"
@if ERRORLEVEL 1 goto Error


:Ok
echo Passed fsharp %~f0 ok.
echo > build.ok
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%

:SetFSCoreXMLPath
set FSHARPCOREXML=%~dpn1.xml
goto :EOF
