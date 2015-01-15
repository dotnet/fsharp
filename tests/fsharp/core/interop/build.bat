@if "%_echo%"=="" echo off

setlocal

if EXIST build.ok DEL /f /q build.ok
rd /S /Q obj
del /f /q *.pdb *.xml *.config *.dll *.exe
call %~d0%~p0..\..\..\config.bat

"%MSBUILDTOOLSPATH%\msbuild.exe" PCL.fsproj
@if ERRORLEVEL 1 goto Error
"%MSBUILDTOOLSPATH%\msbuild.exe" User.fsproj
@if ERRORLEVEL 1 goto Error

%PEVERIFY% User.exe
@if ERRORLEVEL 1 goto Error

:Ok
echo Built fsharp %~f0 ok.
echo. > build.ok
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%
