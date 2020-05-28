REM == %1 --> assembly

ildasm /TEXT /LINENUM /NOBAR "%~nx1" >"%~n1.il"
IF NOT ERRORLEVEL 0 exit 1

echo %~dp0..\..\..\testenv\bin\ILComparer.exe "%~n1.il.bsl" "%~n1.il"
     %~dp0..\..\..\testenv\bin\ILComparer.exe "%~n1.il.bsl" "%~n1.il"
exit /b %ERRORLEVEL%

