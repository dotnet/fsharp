REM == %1 --> assembly

%1 > %1.out

echo ..\..\..\testenv\bin\ILComparer.exe "%1.bsl" "%1.out"
..\..\..\testenv\bin\ILComparer.exe "%1.bsl" "%1.out"
exit /b %ERRORLEVEL%

