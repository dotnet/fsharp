@if "%_echo%"=="" echo off

setlocal
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error


if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

if "%CLR_SUPPORTS_GENERICS%"=="false" ( goto Skip)
if "%CLR_SUPPORTS_SYSTEM_WEB%"=="false" ( goto Skip)

"%FSC%" %fsc_flags% -r:Category.dll -a -o:petshop.dll Category.ml
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" petshop.dll
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
