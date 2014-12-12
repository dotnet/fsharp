@if "%_echo%"=="" echo off
setlocal

if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

"%PEVERIFY%" "%FSCOREDLLPATH%"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" "%FSCOREDLL20PATH%"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" "%FSCOREDLLPORTABLEPATH%"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" "%FSCOREDLLNETCOREPATH%"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" "%FSCOREDLLNETCORE78PATH%"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" "%FSCOREDLLNETCORE259PATH%"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" "%FSCBinPath%\FSharp.Build.dll"
@if ERRORLEVEL 1 goto Error

REM Use /MD because this contains some P/Invoke code  
"%PEVERIFY%" /MD "%FSCBinPath%\FSharp.Compiler.dll"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" "%FSCBinPath%\fsi.exe"
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" "%FSCBinPath%\FSharp.Compiler.Interactive.Settings.dll"
@if ERRORLEVEL 1 goto Error

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