@if "%_echo%"=="" echo off
call %~d0%~p0..\..\..\config.bat

IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
    echo Test not supported except on Ultimate
    exit /b 0
)

"%MSBUILDTOOLSPATH%\msbuild.exe" portablelibrary1.sln /p:Configuration=Debug

exit /b %ERRORLEVEL%