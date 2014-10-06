call %~d0%~p0..\..\..\config.bat

if exist test.ok (del /f /q test.ok)

%CLIX% .\test.exe
if ERRORLEVEL 1 goto Error

:Ok
echo Ran fsharp %~f0 ok.
endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%

